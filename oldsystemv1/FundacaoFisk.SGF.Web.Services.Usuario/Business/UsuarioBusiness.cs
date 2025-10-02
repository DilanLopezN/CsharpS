using System;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using FundacaoFisk.SGF.Web.Services.Usuario.Model;
using FundacaoFisk.SGF.Web.Services.Usuario.Comum;
using FundacaoFisk.SGF.Web.Services.Usuario.DataAccess;
using System.Transactions;
using Componentes.Utils;
using Componentes.GenericBusiness;
using Componentes.Utils.Messages;
using System.Security.Cryptography;
using System.Text;
using System.Data.Entity;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.GenericModel.Partial;

namespace FundacaoFisk.SGF.Web.Services.Usuario.Business {
    public class UsuarioBusiness : IUsuarioBusiness
    {
        private IUsuarioDataAccess DataAccess { get; set; }
        private IUsuarioEmpresaDataAccess DataAccessUserEmpresa { get; set; }
        private IDireitoUsuarioDataAccess DataAccessDireitoUser { get; set; }
        private ISysGrupoUsuarioDataAccess DataAccessGrupoUser { get; set; }

        public UsuarioBusiness(IUsuarioDataAccess dataAccess, IUsuarioEmpresaDataAccess dataAccessUserEmpresa, IDireitoUsuarioDataAccess dataAccessDireitoUser
            , ISysGrupoUsuarioDataAccess dataAccessGrupoUser)
        {
            if ((dataAccess == null) || (dataAccessUserEmpresa == null) || (dataAccessDireitoUser == null) ||
                (dataAccessGrupoUser == null))
            {
                throw new ArgumentNullException("repository");
            }

            DataAccess = dataAccess;
            DataAccessUserEmpresa = dataAccessUserEmpresa;
            DataAccessDireitoUser = dataAccessDireitoUser;
            DataAccessGrupoUser = dataAccessGrupoUser;
        }

        public void configuraUsuario(int cdUsuario, int cd_empresa) {
            // Configura os codigos do usuário para auditorias dos DataAccess:
            ((SGFWebContext)this.DataAccess.DB()).IdUsuario = cdUsuario;
            ((SGFWebContext)this.DataAccess.DB()).cd_empresa = cd_empresa;
        }

        public void sincronizaContexto(DbContext db)
        {
            //this.DataAccess.sincronizaContexto(db);
            //this.DataAccessUserEmpresa.sincronizaContexto(db);
            //this.DataAccessDireitoUser.sincronizaContexto(db);
            //this.DataAccessGrupoUser.sincronizaContexto(db);
        }

        public IEnumerable<UsuarioWebSGF> getUsuarios(int cd_escola){
            return DataAccess.getUsuarios(cd_escola);
        }

        public IEnumerable<UsuarioWebSGF> GetUsuario()
        {
            return DataAccess.GetUsuario();
        }

        public bool verifUsuarioAdmin(int cd_usuario)
        {
            return DataAccess.verifUsuarioAdmin(cd_usuario);
        }

        public UsuarioWebSGF PostUsuario(UsuarioWebSGF usuario)
        {
            usuario.dc_senha_usuario = GeraSenhaHashSHA1(usuario.dc_senha_usuario);
            usuario.PessoaFisica = DataAccess.findIdPessoa(usuario.cd_pessoa);
            usuario = DataAccess.add(usuario, false);
            return usuario;
        }

        public string GeraSenhaHashSHA1(string senha)
        {
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            senha = senha + senha + senha;
            byte[] data = System.Text.Encoding.ASCII.GetBytes(senha);
            byte[] hash = sha1.ComputeHash(data);
            StringBuilder strigBuilder = new StringBuilder();
            foreach (var item in hash)
            {
                strigBuilder.Append(item.ToString("X2"));
            }
            return strigBuilder.ToString().ToUpper();
        }

        public UsuarioWebSGF PutUsuario(UsuarioWebSGF usuario)
        {
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                UsuarioWebSGF usuarioContext = DataAccess.findById(usuario.cd_usuario, false);
                if (usuario.dc_senha_usuario != null && usuario.dc_senha_usuario.Length > 0)
                {
                    usuarioContext.dc_senha_usuario = GeraSenhaHashSHA1(usuario.dc_senha_usuario);
                    usuarioContext.dt_expiracao_senha = DateTime.UtcNow.Date;
                }
                //Se o usuario estava bloqueado e foi desbloqueado, zerar o número de tentativas
                if (usuarioContext.id_bloqueado && !usuario.id_bloqueado)
                    usuario.nm_tentativa = 0;

                //Retono com os campos que sofreram mudanças.
                if (usuario.Direitos != null)
                    crudDireitosUsuario(usuario.Direitos.ToList(), usuario.cd_usuario);
                if (usuario.Grupos != null)
                    crudGrupoUsuarioUsuario(usuario.Grupos.ToList(), usuario.cd_usuario);

                usuarioContext = ChangeValueUsuario(usuarioContext, usuario);
                // E atualiza.
                DataAccess.saveChanges(false);
                usuario = usuarioContext;
                transaction.Complete();
            }
            return usuario;
        }

        public UsuarioWebSGF ChangeValueUsuario(UsuarioWebSGF usuario, UsuarioWebSGF usuarioChange)
        {
            usuario.cd_pessoa = usuarioChange.cd_pessoa;
            usuario.cd_usuario = usuarioChange.cd_usuario;
            usuario.id_master = usuarioChange.id_master;
            usuario.id_administrador = usuarioChange.id_administrador;
            usuario.id_bloqueado = usuarioChange.id_bloqueado;
            usuario.id_trocar_senha = usuarioChange.id_trocar_senha;
            usuario.nm_tentativa = usuarioChange.nm_tentativa;
            usuario.id_usuario_ativo = usuarioChange.id_usuario_ativo;
            usuario.id_admin = usuarioChange.id_admin;
            usuario.no_login = usuarioChange.no_login;
            if (usuarioChange.dt_expiracao_senha > usuario.dt_expiracao_senha)
                usuario.dt_expiracao_senha = usuarioChange.dt_expiracao_senha;
            if (usuarioChange.dc_senha_usuario != null && usuarioChange.dc_senha_usuario.Length > 0 && usuario.dc_senha_usuario != GeraSenhaHashSHA1(usuarioChange.dc_senha_usuario))
                usuario.dc_senha_usuario = GeraSenhaHashSHA1(usuarioChange.dc_senha_usuario);
            if (usuario.PessoaFisica != null && usuarioChange.PessoaFisica != null)
                if (usuario.PessoaFisica.no_pessoa != usuarioChange.PessoaFisica.no_pessoa)
                    usuario.PessoaFisica.no_pessoa = usuarioChange.PessoaFisica.no_pessoa;
            return usuario;
        }

        public void verificaSenha(string senha, bool alteracaoUsuario, string login)
        {
            // A senha deve ter no minimo 6 caracteres, deverá conter: 1 numero, 1 letra maiuscula, 1 letra minuscula 
            if(!(String.IsNullOrEmpty(senha) && alteracaoUsuario) &&
                ((senha.Length < 6) ||
                    !senha.Any(c => char.IsDigit(c)) ||
                    !senha.Any(c => char.IsUpper(c)) ||
                    !senha.Any(c => char.IsLower(c)) ||
                    senha.ToLower().Equals(login.ToLower())
                ))
                throw new UsuarioBusinessException(Messages.msgNovaSenhaInvalida, null, UsuarioBusinessException.TipoErro.ERRO_SENHA_INVALIDA, false);
        }

        public AlterarSenhaStatus PutUsuarioSenha(string login, AlterarSenha senhas, bool zerarTrocarSenha)
        {
            if (senhas.NovaSenha == senhas.ConfirmaNovaSenha)
            {
                verificaSenha(senhas.NovaSenha, false, login);
                verificaSenha(senhas.ConfirmaNovaSenha, false, login);

                UsuarioWebSGF usuario = DataAccess.GetUsuarioByLogin(login).First();
                usuario.dt_expiracao_senha = DateTime.UtcNow;
                if (zerarTrocarSenha)
                    usuario.id_trocar_senha = false;
                if (usuario.dc_senha_usuario == GeraSenhaHashSHA1(senhas.SenhaAtual))
                {
                    usuario.dc_senha_usuario = GeraSenhaHashSHA1(senhas.NovaSenha);
                    DataAccess.saveChanges(false);
                    //DataAccess.PutUsuarioSenha(usuario);
                    return AlterarSenhaStatus.OK;
                }
                else
                    throw new UsuarioBusinessException(Messages.msgInfPassIncorret, null, UsuarioBusinessException.TipoErro.ERRO_SENHA_INVALIDA, false);
                
            }
            else
                throw new UsuarioBusinessException(Messages.msgErrorPass, null, UsuarioBusinessException.TipoErro.ERRO_SENHA_INVALIDA, false);
        }

        public IEnumerable<UsuarioWebSGF> GetUsuarioByLogin(string login)
        {
            return DataAccess.GetUsuarioByLogin(login);
        }

        public UsuarioWebSGF GetUsuarioAuthenticateByLogin(string login) {
            UsuarioWebSGF usuario;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                usuario =  DataAccess.GetUsuarioAuthenticateByLogin(login).FirstOrDefault();
                transaction.Complete();
            }
            if(usuario != null && usuario.id_bloqueado)
                throw new UsuarioBusinessException(Messages.msgUsuarioBloqueado, null, UsuarioBusinessException.TipoErro.USUARIO_BLOQUEADO, false);
            return usuario;
        }

        public void  verifyExistLoginOK(string login, string nomePessoa)
        {
            // Verifica se o login postado pelo usuário ainda não existe:
            bool valido = DataAccess.isValidLogin(login);
            if (!valido)
            {
                // Monta as combinações de sugestão de login: <primeiro nome> <nome_sobrenome> <nome.sobrenome> <nome + primeira letra de cada sobrenome>
                if (!String.IsNullOrEmpty(nomePessoa))
                {
                    String[] tokens = nomePessoa.Split(' ');
                    if (tokens == null)
                        tokens = new String[] { nomePessoa };
                    List<string> Combinacoes = new List<string>();
                    string retorno = "";

                    if (tokens.Length == 1)
                        Combinacoes.Add(tokens[0]);
                    else
                    {
                        string primeiraCombinacao = tokens[0];

                        for (int i = 1; i < tokens.Length; i++)
                        {
                            Combinacoes.Add(primeiraCombinacao + "_" + tokens[i]);
                            Combinacoes.Add(primeiraCombinacao + "." + tokens[i]);
                            if (!string.IsNullOrEmpty(tokens[i]))
                                Combinacoes.Add(primeiraCombinacao + tokens[i].Substring(0, 1));
                        }

                        Combinacoes.Add(primeiraCombinacao);
                    }

                    foreach (var token in Combinacoes)
                    {
                        valido = false;
                        valido = DataAccess.isValidLogin(token);
                        if (valido)
                            retorno = retorno + ", " + token;
                    }
                    if (retorno.Length == 0)
                        throw new UsuarioBusinessException(Messages.msgUserExistSemSugestoes, null, 
                                                           FundacaoFisk.SGF.Web.Services.Usuario.Business.UsuarioBusinessException.TipoErro.ERRO_USUARIOEXISTENTE, false);
                    if (!String.IsNullOrEmpty(retorno))
                        retorno = retorno.Substring(1, retorno.Length - 1);
                    throw new UsuarioBusinessException(string.Format(Messages.msgUserExist, "{0}", retorno), null, 
                                                       FundacaoFisk.SGF.Web.Services.Usuario.Business.UsuarioBusinessException.TipoErro.ERRO_COMBINACOES, false);
                }
                else
                {
                    throw new UsuarioBusinessException(Messages.msgUserExistSemSugestoes , null, FundacaoFisk.SGF.Web.Services.Usuario.Business.UsuarioBusinessException.TipoErro.ERRO_USUARIOEXISTENTE, false);
                }
            }
        }

        public IEnumerable<UsuarioUISearch> GetUsuarioSearch(SearchParameters parametros, string descricao, string nome, bool inicio, bool? status, string usuarioLogado, Int32[] codEscolas, int cd_empresa, bool master, bool sysAdmin, bool filtroSysAdmin)
        {
            IEnumerable<UsuarioUISearch> retorno = new List<UsuarioUISearch>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if(parametros.sort == null)
                    parametros.sort = "no_pessoa";
                parametros.sort = parametros.sort.Replace("ativo", "id_usuario_ativo");
                parametros.sort = parametros.sort.Replace("Master", "id_master");
                parametros.sort = parametros.sort.Replace("sysAdmin", "id_admin");
                bool masterGeral = DataAccess.VerificarMasterGeral(usuarioLogado);
                retorno = DataAccess.GetUsuarioSearch(parametros, descricao, nome, inicio, status, masterGeral, codEscolas, cd_empresa,master,sysAdmin, filtroSysAdmin);
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<UsuarioUISearch> getUsuarioSearchFK(SearchParameters parametros, string descricao, string nome, bool inicio, int cd_empresa)
        {
            if (parametros.sort == null)
                parametros.sort = "no_pessoa";
            parametros.sort = parametros.sort.Replace("ativo", "id_usuario_ativo");
            parametros.sort = parametros.sort.Replace("Master", "id_master");
            parametros.sort = parametros.sort.Replace("sysAdmin", "id_admin");
            return DataAccess.getUsuarioSearchFK(parametros, descricao, nome, inicio, cd_empresa);
        }

        public UsuarioWebSGF getusuarioForEdit(int idUser)
        {
            return DataAccess.getusuarioForEdit(idUser);
        }

        public bool VerificarMasterGeral(int cdUsuario)
        {
            return DataAccess.VerificarMasterGeral(cdUsuario);
        }

        public void incrementaNmTentativa(UsuarioWebSGF usuario, int maxTentativa, bool senhaValida)
        {
            usuario = DataAccess.findById(usuario.cd_usuario, false);
            if (!senhaValida)
            {
                usuario.nm_tentativa = (byte)(usuario.nm_tentativa + 1);
                if (usuario.nm_tentativa >= maxTentativa)
                    usuario.id_bloqueado = true;
            }
            else
                usuario.nm_tentativa = 0;
            DataAccess.saveChanges(false);
            if ((maxTentativa - usuario.nm_tentativa) == 2)
                throw new UsuarioBusinessException(Messages.msgInfoDuasTentativasSenha, null, UsuarioBusinessException.TipoErro.ERRO_FALTA_DUAS_TENTATIVAS_SENHA, false);

        }

        //Valida envia o email e persiste o novo email na base de dados.
        public UsuarioWebSGF alterarSenhaUsuario(int tamanhoSenha, SendEmail sendEmail)
        {
            UsuarioWebSGF usuario = new UsuarioWebSGF();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                usuario = DataAccess.existsUsuarioByLoginEmail(sendEmail.login, sendEmail.email);
                if (usuario == null)
                    throw new UsuarioBusinessException(Messages.msgErroEnviarEmail, null, FundacaoFisk.SGF.Web.Services.Usuario.Business.UsuarioBusinessException.TipoErro.ERRO_USUARIO_NOT_EXISTENTE, false);
                if (usuario != null)
                {
                    //Faz as consistêcias necessárias para o enviar email
                    if (sendEmail.destinatario == null || sendEmail.destinatario == "")
                        throw new UsuarioBusinessException(Messages.msgRemetenteNotEnc, null,
                                                                  FundacaoFisk.SGF.Web.Services.Usuario.Business.UsuarioBusinessException.TipoErro.ERRO_REMENTENTE_NOT_EXISTENTE, false);
                    if(tamanhoSenha < 6)
                    throw new UsuarioBusinessException(Messages.msgTamanhoSenhaMenor, null,
                                                        FundacaoFisk.SGF.Web.Services.Usuario.Business.UsuarioBusinessException.TipoErro.ERRO_TAMANHO_SENHA_MENOR, false);
              
                    if(tamanhoSenha > 10)
                        throw new UsuarioBusinessException(Messages.msgTamanhoSenhaMaior, null,
                                                        FundacaoFisk.SGF.Web.Services.Usuario.Business.UsuarioBusinessException.TipoErro.ERRO_USUARIO_NOT_EXISTENTE, false);

                    //Gera a senha aleatória
                    string senha = GerarSenhaAleatorio.gerarSenha(tamanhoSenha);
                    
                    sendEmail.assunto = "Envio de nova senha";
                    string nomesEscolas = "";
                    string unidade = "Unidade:";
                    List<Escola> empresasUsu = usuario.EmpresasUsuario.ToList();
                    if (empresasUsu != null && empresasUsu.Count() > 0)
                        nomesEscolas = empresasUsu[0].dc_reduzido_pessoa;
                    if (empresasUsu != null && empresasUsu.Count() > 1)
                    {
                        for (int i = 1; i < empresasUsu.Count(); i++)
                            nomesEscolas = nomesEscolas + ", " + empresasUsu[i].dc_reduzido_pessoa;
                        unidade = "Unidades:";
                    }
                    if (usuario.id_master && empresasUsu.Count() <= 0)
                    {
                        unidade = "Unidades:";
                        nomesEscolas = "Todas";
                    }
                    //sendEmail.mensagem = sendEmail.login + " sua nova senha é: " + senha;
                    sendEmail.mensagem = "<div><p class='MsoNormal'><u></u>&nbsp;<u></u></p><p class='MsoNormal'><span style='font-size:10.0pt'><img class='CToWUd' src='http://www.fisk.com.br/img/fisk-logo.jpg'></span><br>" +
                                         "<br><span style='font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;'>Olá! Suas credenciais para acessar<span style='color:#1f497d'> o SGF</span> são:<br><br>" +
                                         "<b>Usuário:</b> " + sendEmail.login  +" <br><b>Senha:</b> " + senha + " <br><b>"+ unidade + "</b> " + nomesEscolas + "<br><br>O acesso pode ser feito através do endereço <a href='http://www.fisk.com.br/sgf' target='_blank'>" +
                                         "www.fisk.com.br/sgf</a> <br><br>Atenciosamente,<span style='color:#1f497d'><br><br></span><br><b>FISK - Centro de Ensino</b> <br><a href='tel:0800%20773%203475' value='+558007733475' target='_blank'>0800 773 3475</a> <br>" +
                                         "<a href='http://www.fisk.com.br' target='_blank'>www.fisk.com.br</a><span style='color:#1f497d'><u></u><u></u></span></span></p><div class='yj6qo'></div><div class='adL'></div></div>";
                    if(!SendEmail.EnviarEmail(sendEmail))
                        throw new UsuarioBusinessException(Messages.msgErroEnviarEmail, null,
                                                    FundacaoFisk.SGF.Web.Services.Usuario.Business.UsuarioBusinessException.TipoErro.ERRO_ENVIO_EMAIL, false);
                    
                    //Gera o hash da senha do usuário
                    string senhaHash = GeraSenhaHashSHA1(senha);

                    if (usuario.id_usuario_ativo)
                    {
                        usuario.id_trocar_senha = true;
                        usuario.id_bloqueado = false;
                        usuario.nm_tentativa = 0;
                    }

                    usuario.dc_senha_usuario = senhaHash;
                    DataAccess.saveChanges(false);
                    
                }
                transaction.Complete();
            }
         
            return usuario;
        }
        public void crudEmpresasUsuario(List<UsuarioEmpresaSGF> usuarioEmpresaView, int cd_usuario)
        {
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED, DataAccessUserEmpresa.DB(), TransactionScopeBuilder.TransactionTime.MODEREATE))
            {

                IEnumerable<UsuarioEmpresaSGF> escolasView = usuarioEmpresaView.ToList();
                List<int> cdsEscolasView = escolasView.Select(o => o.cd_pessoa_empresa).ToList();

                IEnumerable<int> codEscolasBase = DataAccessUserEmpresa.findAllUsuarioEmpresaByUsuarioIds(cd_usuario).ToList();
                List<UsuarioEmpresaSGF> usuarioEmpresaContext = DataAccessUserEmpresa.findAllUsuarioEmpresaByUsuario(cd_usuario).ToList();



                if ((escolasView != null))
                {
                    IEnumerable<int> codEscolasVW = escolasView.Select(ev => ev.cd_pessoa_empresa);

                    //Insere as Escolas da view que foram adcionadas na grade e não estão na base de dados
                    IEnumerable<int> escolasInserir = codEscolasVW.Except(codEscolasBase);
                    

                    if (escolasInserir != null && escolasInserir.Count() > 0)
                    {
                        foreach (var escola in escolasInserir)
                        {
                            UsuarioEmpresaSGF escolaInserir = escolasView.Where(x => x.cd_pessoa_empresa == escola).FirstOrDefault();
                            if(escolaInserir != null)
                            {
                                DataAccessUserEmpresa.addContext(escolaInserir, false);
                            } 
                            
                        }

                        DataAccessUserEmpresa.saveChanges(false);
                    }


                    //Deletar as escolas que estão no banco e não estão na view.
                    IEnumerable<int> deletarEscolas = codEscolasBase.Except(codEscolasVW);
                    if (deletarEscolas != null && deletarEscolas.Count() > 0)
                    {
                        foreach (var cdEscolaDel in deletarEscolas)
                        {
                            
                            UsuarioEmpresaSGF escolaDel = usuarioEmpresaContext.Where(x => x.cd_pessoa_empresa == cdEscolaDel).FirstOrDefault();
                            if (escolaDel != null)
                            {
                                DataAccessUserEmpresa.deleteContext(escolaDel, false);
                            }
                        }
                        DataAccessUserEmpresa.saveChanges(false);
                    }
                }


                
                DataAccessUserEmpresa.saveChanges(false);
                transaction.Complete();
            }
        }

        public void crudDireitosUsuario(List<SysDireitoUsuario> sysDireitoUsuarioView, int cd_usuario)
        {
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                List<SysDireitoUsuario> sysDireitoUsuarioContext = DataAccessDireitoUser.findAllDireitosUsuarioByUsuario(cd_usuario).ToList();
                IEnumerable<SysDireitoUsuario> sysDireitoUsuarioComCodigo = from it in sysDireitoUsuarioView
                                                                            where it.cd_direito_usuario != 0
                                                                            select it;
                IEnumerable<SysDireitoUsuario> sysDireitoUsuarioDeleted = sysDireitoUsuarioContext.Where(tc => !sysDireitoUsuarioComCodigo.Any(tv => tc.cd_direito_usuario == tv.cd_direito_usuario));

                if (sysDireitoUsuarioDeleted != null)
                    foreach (var item in sysDireitoUsuarioDeleted)
                        if (item != null)
                            DataAccessDireitoUser.delete(item, false);

                foreach (var item in sysDireitoUsuarioView)
                {
                    // Novos horários da turma:
                    if (item.cd_direito_usuario == 0)
                    {
                        item.cd_usuario = cd_usuario;
                        DataAccessDireitoUser.add(item, false);
                    }
                }
                DataAccessDireitoUser.saveChanges(false);
                transaction.Complete();
            }
        }

        public void crudGrupoUsuarioUsuario(List<SysGrupoUsuario> gruposUsuarioView, int cd_usuario)
        {
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                List<SysGrupoUsuario> gruposUsuarioContext = DataAccessGrupoUser.findAllGrupoUsuarioByUsuario(cd_usuario).ToList();
                IEnumerable<SysGrupoUsuario> gruposUsuarioComCodigo = from it in gruposUsuarioView
                                                                      where it.cd_grupo_usuario != 0
                                                                      select it;
                IEnumerable<SysGrupoUsuario> gruposUsuarioDeleted = gruposUsuarioContext.Where(tc => !gruposUsuarioComCodigo.Any(tv => tc.cd_grupo_usuario == tv.cd_grupo_usuario));

                if (gruposUsuarioDeleted != null)
                    foreach (var item in gruposUsuarioDeleted)
                        if (item != null)
                            DataAccessGrupoUser.delete(item, false);

                foreach (var item in gruposUsuarioView)
                {
                    // Novos horários da turma:
                    if (item.cd_grupo_usuario == 0)
                    {
                        item.cd_usuario = cd_usuario;
                        DataAccessGrupoUser.add(item, false);
                    }
                }
                DataAccessGrupoUser.saveChanges(false);
                transaction.Complete();
            }
        }

        public bool VerificarSupervisorByEscola(int cd_login, int cd_pessoa_empresa)
        {
            return DataAccess.VerificarSupervisorByEscola(cd_login, cd_pessoa_empresa);
        }
        public List<UsuarioWebSGF> findUsuarioByGrupo(int cdEmpresa, int cdGrupo) {
            return DataAccess.findUsuarioByGrupo(cdEmpresa, cdGrupo);
        }

        public bool verificarSysAdmin(string login)
        {
            return DataAccess.verificarSysAdmin(login);
        }

        public bool verificaExisteSysAdminAtivosEscolas(int[] cdEscolas, string no_login)
        {
            return DataAccess.verificaExisteSysAdminAtivosEscolas(cdEscolas, no_login);
        }

        public bool verificaExisteSysAdminAtivosEscolas(string no_login)
        {
            return DataAccess.verificaExisteSysAdminAtivosEscolas(no_login);
        }
        public IEnumerable<UsuarioUISearch> getUsuarioSearchGeralFK(SearchParameters parametros, string descricao, string nome, bool inicio, int cd_empresa)
        {
            IEnumerable<UsuarioUISearch> retorno = new List<UsuarioUISearch>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "no_pessoa";
                parametros.sort = parametros.sort.Replace("ativo", "id_usuario_ativo");
                parametros.sort = parametros.sort.Replace("Master", "id_master");
                parametros.sort = parametros.sort.Replace("sysAdmin", "id_admin");
                retorno = DataAccess.getUsuarioSearchGeralFK(parametros, descricao, nome, inicio, cd_empresa);
                transaction.Complete();
            }
            return retorno;
        }
        public IEnumerable<UsuarioUISearch> getUsuarioSearchAtendenteFK(SearchParameters parametros, string descricao, string nome, bool inicio, int cd_empresa)
        {
            IEnumerable<UsuarioUISearch> retorno = new List<UsuarioUISearch>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "no_pessoa";
                parametros.sort = parametros.sort.Replace("ativo", "id_usuario_ativo");
                parametros.sort = parametros.sort.Replace("Master", "id_master");
                parametros.sort = parametros.sort.Replace("sysAdmin", "id_admin");
                retorno = DataAccess.getUsuarioSearchAtendenteFK(parametros, descricao, nome, inicio, cd_empresa);
                transaction.Complete();
            }
            return retorno;
        }
        public bool VerificarMasterGeral(string login)
        {
            return DataAccess.VerificarMasterGeral(login);
        }

        public IEnumerable<UsuarioWebSGF> findUsuarioByEmpresaLogin(int cdEmpresa, int codUsuario, bool admGeral, bool? ativo, int? cdGrupo)
        {
            return DataAccess.findUsuarioByEmpresaLogin(cdEmpresa, codUsuario, admGeral, ativo, cdGrupo);
        }

        public UsuarioWebSGF findUsuarioById(int cd_usuario)
        {
            return DataAccess.findById(cd_usuario, false);
        }

        public UsuarioAreaRestritaUI GetEmailUsuario(int usuario)
        {
            return DataAccess.GetEmailUsuario(usuario);
        }

        public Boolean DeleteUsuario(List<UsuarioWebSGF> usuariosWebSGF)
        {
            return DataAccess.DeleteUsuario(usuariosWebSGF);
        }

        public int getIdUsuario(string login)
        {
            return DataAccess.getIdUsuario(login);
        }

        public UsuarioUISearch getUsuarioFromViewGrid(int cd_usuario, int countCdEmpresas, bool masterGeral)
        {
            return DataAccess.getUsuarioFromViewGrid(cd_usuario, countCdEmpresas, masterGeral);
        }
        public bool verificarTravaProfessor(int cdPessoa, int cdEscola)
        {
            return DataAccess.verificarTravaProfessor(cdPessoa, cdEscola);
        }
    }
}
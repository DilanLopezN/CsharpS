using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Componentes.GenericBusiness;
using System.Transactions;
using Componentes.Utils;
using Componentes.Utils.Messages;
using FundacaoFisk.SGF.Utils;
using FundacaoFisk.SGF.Web.Services.Empresa.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Empresa.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using FundacaoFisk.SGF.Web.Services.Empresa.Model;


namespace FundacaoFisk.SGF.Web.Services.Empresa.Business {
    using FundacaoFisk.SGF.GenericModel;
    using FundacaoFisk.SGF.Web.Services.Usuario.Comum;
    using FundacaoFisk.SGF.Web.Services.Usuario.Model;
    using FundacaoFisk.SGF.Web.Services.Pessoa.Comum.Business;
    using FundacaoFisk.SGF.Web.Services.Pessoa.Business;
    using FundacaoFisk.SGF.Web.Services.Empresa.Model;
    using FundacaoFisk.SGF.Web.Services.Usuario.Business;
    using FundacaoFisk.SGF.Web.Services.Log.Comum.IBusiness;
    using System.Data.Entity;
    using Componentes.GenericBusiness.Comum;

    public class EmpresaBusiness : IEmpresaBusiness {
        public IEmpresaDataAccess Dao { get; set; }

        private IUsuarioDataAccess DataAccessUsuario { get; set; }
        private IPessoaBusiness BusinessPessoa { get; set; }
        private IUsuarioBusiness BusinessUsuario { get; set; }
        public ILogGeralBusiness BusinessLogGeral { get; set; }
        public IApiNewCyberFuncionarioBusiness BusinessApiNewCyber { get; set; }
        public IApiAreaRestritaBusiness BusinessApiAreRestrita { get; set; }

        #region Empresa

        public EmpresaBusiness(IEmpresaDataAccess dao, IPessoaBusiness businessPessoa, 
            IUsuarioBusiness businessUsuario, ILogGeralBusiness businessLogGeral, IApiNewCyberFuncionarioBusiness businessApiNewCyber,
            IApiAreaRestritaBusiness businessApiAreRestrita, IUsuarioDataAccess dataAccessUsuario)
        {
            if(dao == null || businessPessoa == null || businessUsuario == null || businessLogGeral == null || businessApiNewCyber == null || businessApiAreRestrita == null || dataAccessUsuario == null)
                throw new ArgumentNullException();
            Dao = dao;
            BusinessPessoa = businessPessoa;
            BusinessUsuario = businessUsuario;
            BusinessLogGeral = businessLogGeral;
            BusinessApiNewCyber = businessApiNewCyber;
            BusinessApiAreRestrita = businessApiAreRestrita;
            DataAccessUsuario = dataAccessUsuario;
        }

        public void configuraUsuario(int cdUsuario, int empresa) {
            // Configura os codigos do usuário para auditorias dos DataAccess:
            ((SGFWebContext)this.Dao.DB()).IdUsuario = cdUsuario;
            ((SGFWebContext)this.Dao.DB()).cd_empresa = empresa;
            ((SGFWebContext)this.DataAccessUsuario.DB()).IdUsuario = cdUsuario;
            ((SGFWebContext)this.DataAccessUsuario.DB()).cd_empresa = empresa;

            BusinessPessoa.configuraUsuario(cdUsuario, empresa);
            this.BusinessUsuario.configuraUsuario(cdUsuario, empresa);
            BusinessApiNewCyber.configuraUsuario(cdUsuario, empresa);
            BusinessApiAreRestrita.configuraUsuario(cdUsuario, empresa);
        }


        public void sincronizaContexto(DbContext db)
        {
            //this.Dao.sincronizaContexto(db);
            //this.BusinessPessoa.sincronizaContexto(db);
            //this.BusinessUsuario.sincronizaContexto(db);
            //this.BusinessLogGeral.sincronizaContexto(db);
        }

        public bool getEmpresaPropria(int cd_empresa)
        {
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                bool retorno = Dao.getEmpresaPropria(cd_empresa);
                transaction.Complete();

                return retorno;
            }
        }

        public Escola getHorarioFunc(int cd_empresa) {
            return Dao.getHorarioFunc(cd_empresa);
        }

        public bool deleteAllEmpresa(List<Escola> empresas)
        {
            bool deleted = false;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                List<EscolaApiCyberBdUI> listaEmpresasDelete = new List<EscolaApiCyberBdUI>();
                if (BusinessApiNewCyber.aplicaApiCyber())
                {
                    //cria uma lista com as escolas a serem deletadas 
                    preencheListaEscolasApiCyberDelete(empresas, listaEmpresasDelete);
                }

                for (int i = 0; i < empresas.Count(); i++)
                    BusinessLogGeral.getLogGeralByEmpresa(empresas[i].cd_pessoa);
                Dao.deleteAllEmpresa(empresas);
                deleted = true;

                if (BusinessApiNewCyber.aplicaApiCyber())
                {
                    foreach (EscolaApiCyberBdUI escolaApiCyber in listaEmpresasDelete)
                    {
                        if (escolaApiCyber != null)
                        {
                            //Se a escola tem o nm_cliente_integracao e existe no cyber
                            if (escolaApiCyber.nm_cliente_integracao != null && escolaApiCyber.nm_cliente_integracao > 0 && existeUnidade((int)escolaApiCyber.nm_cliente_integracao))
                            {
                                //Chama a api cyber com o comando (INATIVA_UNIDADE)
                                executaCyberInativaUnidade((int)escolaApiCyber.nm_cliente_integracao);

                            }
                        }
                    }
                }

                transaction.Complete();
            }
            return deleted;
        }

        private void preencheListaEscolasApiCyberDelete(List<Escola> empresas, List<EscolaApiCyberBdUI> listaEmpresasDelete)
        {
            foreach (Escola empresa in empresas)
            {
                if (empresa != null)
                {
                    EscolaApiCyberBdUI escolaApiCyber = Dao.getEscola(empresa.cd_pessoa);
                    if (escolaApiCyber != null)
                    {
                        listaEmpresasDelete.Add(escolaApiCyber);
                    }
                }
            }
        }

        public void executaCyberInativaUnidade(int codigo)
        {
            string result = BusinessApiNewCyber.postExecutaCyber(ConfigurationManager.AppSettings["enderecoApiNewCyber"],
                ApiCyberComandosNames.INATIVA_UNIDADE, ConfigurationManager.AppSettings["chaveApiNewCyber"], String.Format("codigo={0}", codigo));
        }

        public bool existeUnidade(int codigo)
        {
            return BusinessApiNewCyber.verificaRegistro(ConfigurationManager.AppSettings["enderecoApiNewCyber"],
                ApiCyberComandosNames.VISUALIZA_UNIDADE, ConfigurationManager.AppSettings["chaveApiNewCyber"], codigo);
        }

        public EscolaApiCyberBdUI getEscola(int cd_escola)
        {
            return Dao.getEscola(cd_escola);
        }

        public Escola existsEmpresaWithCNPJ(string cnpj)
        {
            return Dao.existsEmpresaWithCNPJ(cnpj);
        }

        public int insertPessoaWithEmpresa(int cd_empresa, int? nmIntegracaoCliente, int? nm_empresa_integracao, TimeSpan? hrInicial, TimeSpan? hrFinal, DateTime? dt_abertura, DateTime? dt_inicio)
        {
            return Dao.insertPessoaWithEmpresa(cd_empresa, nmIntegracaoCliente, nm_empresa_integracao, hrInicial, hrFinal, dt_abertura, dt_inicio);
        }

        public List<EmpresaSession> findEmpresaSessionByLogin(string login, bool ehMaster, bool ativos)
        {
            return this.findEmpresaSessionByLogin(login, ehMaster, ativos, TransactionScopeBuilder.TransactionType.COMMITED);
        }

        public List<EmpresaSession> findEmpresaSessionByLogin(string login, bool ehMaster, bool ativos, TransactionScopeBuilder.TransactionType TransactionType)
        {
            List<EmpresaSession> retorno = new List<EmpresaSession>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionType))
            {
                retorno = Dao.findEmpresaSessionByLogin(login, ehMaster, ativos).ToList<EmpresaSession>();
                transaction.Complete();
            }
            return retorno;
        }

        public EmpresaSession findEmpresaSessionById(int id_empresa, int cd_usuario, bool is_master, bool is_master_geral)
        {
            EmpresaSession retorno = new EmpresaSession();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = Dao.findEmpresaSessionById(id_empresa, cd_usuario, is_master, is_master_geral);
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<Escola> findAllEmpresaByUsuario(int codUsuario)
        {
            return Dao.findAllEmpresaByUsuario(codUsuario);
        }

        public List<EmpresaSession> findAllEmpresaSession()
        {
            return this.findAllEmpresaSession(TransactionScopeBuilder.TransactionType.COMMITED).ToList<EmpresaSession>();
        }

        public List<EmpresaSession> findAllEmpresaSession(TransactionScopeBuilder.TransactionType transactionType)
        {
            List<EmpresaSession> retorno = new List<EmpresaSession>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(transactionType, null, TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                retorno = Dao.findAllEmpresaSession().ToList<EmpresaSession>();
                transaction.Complete();
            }
            return retorno;
        }

        public Escola findByIdEmpresa(int id)
        {
            return Dao.findByIdEmpresa(id);
        }

        public EmpresaSession findSessionByIdEmpresa(int id)
        {

            EmpresaSession retorno = new EmpresaSession();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = Dao.findSessionByIdEmpresa(id);
                transaction.Complete();
            }
            return retorno;
        }

        public List<Escola> findAllEmpresaAnterior(int codUsuario, bool masterGeral, int cdEmpresaAnt)
        {
            return Dao.findAllEmpresaAnterior(codUsuario, masterGeral, cdEmpresaAnt).ToList<Escola>();
        }

        public List<Escola> findAllEmpresaColigada(int cdEscola)
        {
            return Dao.findAllEmpresaColigada(cdEscola).ToList<Escola>();
        }

    public IEnumerable<Escola> findAllEmpresa()
        {
            return Dao.findAllEmpresa();
        }

        public bool postEmpresaPessoa(PessoaEscola pessoaEmp)
        {
            return Dao.addEmpresaPessoa(pessoaEmp);
        }

        public bool postEmpresaPessoaBiblioteca(PessoaEscola pessoaEmp)
        {
            bool add = false;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {

                add = Dao.addEmpresaPessoa(pessoaEmp);
                transaction.Complete();
            }
            return add;
            
        }

        public IEnumerable<EmpresaUI> findAllEmpresaByUsuario(SearchParameters parametros, string login, string desc, int cdItem)
        {
            IEnumerable<EmpresaUI> retorno = new List<EmpresaUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "no_pessoa";
                bool master = BusinessUsuario.VerificarMasterGeral(login);
                retorno = Dao.findAllEmpresaByUsuario(parametros, login, master, desc, cdItem);
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<EmpresaUIUsuario> findAllEmpresaByLoginPag(SearchParameters parametros, string login, List<int> empresas, String nome, string fantasia, string cnpj, bool inicio, bool editUser)
        {
            IEnumerable<EmpresaUIUsuario> retorno = new List<EmpresaUIUsuario>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "dc_reduzido_pessoa";
                parametros.sort = parametros.sort.Replace("no_pessoa", "dc_reduzido_pessoa");
                parametros.sort = parametros.sort.Replace("nm_cpf_cgc_dependente", "nm_cpf_cgc");
                parametros.sort = parametros.sort.Replace("dta_cadastro", "dt_cadastramento");

                bool master = BusinessUsuario.VerificarMasterGeral(login);
                retorno = Dao.findAllEmpresaByLoginPag(parametros, login, master, empresas, nome, fantasia, cnpj, inicio, editUser);
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<EmpresaUIUsuario> findAllEmpresaTransferencia(SearchParameters parametros, int cd_empresa, String nome, string fantasia, string cnpj, bool inicio)
        {
            IEnumerable<EmpresaUIUsuario> retorno = new List<EmpresaUIUsuario>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "dc_reduzido_pessoa";
                parametros.sort = parametros.sort.Replace("no_pessoa", "dc_reduzido_pessoa");
                parametros.sort = parametros.sort.Replace("nm_cpf_cgc_dependente", "nm_cpf_cgc");
                parametros.sort = parametros.sort.Replace("dta_cadastro", "dt_cadastramento");

                retorno = Dao.findAllEmpresaTransferencia(parametros, cd_empresa, nome, fantasia, cnpj, inicio);
                transaction.Complete();
            }
            return retorno;
        }



        public IEnumerable<PessoaSearchUI> findAllEmpresasByUsuarioPag(SearchParameters parametros, string login, List<int> empresas, String nome, string fantasia, string cnpj, bool inicio, bool editUser, int cdEscola)
        {
            IEnumerable<PessoaSearchUI> retorno = new List<PessoaSearchUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "dc_reduzido_pessoa";
                parametros.sort = parametros.sort.Replace("no_pessoa", "dc_reduzido_pessoa");
                parametros.sort = parametros.sort.Replace("nm_cpf_cgc_dependente", "nm_cpf_cgc");
                parametros.sort = parametros.sort.Replace("dta_cadastro", "dt_cadastramento");

                bool master = BusinessUsuario.VerificarMasterGeral(login);
                retorno = Dao.findAllEmpresasByUsuarioPag(parametros, login, master, empresas, nome, fantasia, cnpj, inicio, editUser, cdEscola);
                transaction.Complete();
            }
            return retorno;
        }

        public List<int> findAllEmpresasByUser(int cd_user)
        {
            List<int> retorno = new List<int>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {

                retorno = Dao.findAllEmpresasByUser(cd_user);
                transaction.Complete();
            }
            return retorno;
        }

        public List<int> findAllEmpresasByUsuario(string login,  int cdEscola)
        {
            List<int> retorno = new List<int>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {

                bool master = BusinessUsuario.VerificarMasterGeral(login);
                retorno = Dao.findAllEmpresasByUsuario(login, master, cdEscola);
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<PessoaSearchComboUI> findAllEmpresasUsuarioComboFK(string login, int cdEscola)
        {
            IEnumerable<PessoaSearchComboUI> retorno = new List<PessoaSearchComboUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {

                bool master = BusinessUsuario.VerificarMasterGeral(login);
                retorno = Dao.findAllEmpresasUsuarioComboFK(login, master, cdEscola);
                transaction.Complete();
            }
            return retorno;
        }

        public int findQuantidadeEmpresasVinculadasUsuario(string login, int cdEscola)
        {
            int retorno = 0;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                

                bool master = BusinessUsuario.VerificarMasterGeral(login);
                retorno = Dao.findQuantidadeEmpresasVinculadasUsuario(login, master, cdEscola);
                transaction.Complete();
            }
            return retorno;
        }

        public string findByNomeEscolaComboReportView(int cdEscolaCombo)
        {
            string retorno ;

            retorno = Dao.findByNomeEscolaComboReportView(cdEscolaCombo);

            return retorno;
        }


        public IEnumerable<EmpresaUI> getEmpresaGrupo(int cd_grupo)
        {
            return Dao.getEmpresaGrupo(cd_grupo);
        }

        public IEnumerable<EmpresaUI> getEmpresaHasGrupoMaster(int cd_grupo_master)
        {
            return Dao.getEmpresaHasGrupoMaster(cd_grupo_master);
        }

        public string getEmailEscola(int cd_empresa)
        {
            return Dao.getEmailEscola(cd_empresa);
        }

        #endregion

        #region usuário

        public IEnumerable<Escola> findAllEmpresaByUsuario(string login) {
            bool masterGeral = BusinessUsuario.VerificarMasterGeral(login);
            return Dao.findAllEmpresaByUsuario(login, masterGeral);
        }

        public IEnumerable<Escola> findAllEmpresaByCdUsuario(int cd_usuario)
        {
            bool masterGeral = BusinessUsuario.VerificarMasterGeral(cd_usuario);
            return Dao.findAllEmpresaByCdUsuario(cd_usuario, masterGeral);
        }

        public bool VerificarMasterGeral(string login) {
            return BusinessUsuario.VerificarMasterGeral(login);
        }
        public bool VerificarMasterGeral(int cdUsuario)
        {
            return BusinessUsuario.VerificarMasterGeral(cdUsuario);
        }

        public IEnumerable<UsuarioWebSGF> findUsuarioByEmpresaLogin(int cdEmpresa, int codUsuario, bool admGeral, bool? ativo, int? cdGrupo) {
            return BusinessUsuario.findUsuarioByEmpresaLogin(cdEmpresa, codUsuario, admGeral, ativo, cdGrupo);
        }

        public bool DeleteUsuario(List<UsuarioWebSGF> usuariosWebSGF) {
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                List<UsuarioWebSGF> usuariosDeleteAreaRestrita = new List<UsuarioWebSGF>();
                foreach (var user in usuariosWebSGF)
                {
                    if (user != null)
                    {
                        if (user.id_area_resrtrita != null)
                        {
                            usuariosDeleteAreaRestrita.Add(user);
                        }
                    }
                }

                var retorno = BusinessUsuario.DeleteUsuario(usuariosWebSGF);

                if (usuariosDeleteAreaRestrita != null && usuariosDeleteAreaRestrita.Count > 0)
                {
                    DeleteUsuarioApiAreaRestrita(usuariosDeleteAreaRestrita);
                }
                 
                transaction.Complete();
                return retorno;
            }
        }

        public int getIdUsuario(string login) {
            return BusinessUsuario.getIdUsuario(login);
        }

        public bool verifUsuarioAdmin(int cd_usuario)
        {
            return BusinessUsuario.verifUsuarioAdmin(cd_usuario);
        }

        public UsuarioUISearch PostInsertUsuario(UsuarioWebSGF usuario, PessoaFisicaSGF pessoaFisica, int cdEmp)
        {
            this.sincronizaContexto(Dao.DB());
            usuario.dt_expiracao_senha = DateTime.Now.Date;
            UsuarioUISearch usuarioUISearch = new UsuarioUISearch();
            int qtdEscolasUsuario = 0;
            if (usuario.id_admin)
            {
                List<UsuarioEmpresaSGF> userEmp = usuario.Empresas.ToList();
                int[] cdEscolas = new int[userEmp.Count()];
                for (int i = 0; i < userEmp.Count; i++)
                    cdEscolas[i] = userEmp[i].cd_pessoa_empresa;
                bool existSysAdmin = BusinessUsuario.verificaExisteSysAdminAtivosEscolas(cdEscolas, usuario.no_login);
                if (existSysAdmin)
                    throw new UsuarioBusinessException(Messages.msgUserSysAdminExistEscolas, null, UsuarioBusinessException.TipoErro.ERRO_JA_EXISTE_SYSADMIN_ESCOLAS, false);
            }
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                BusinessUsuario.verificaSenha(usuario.dc_senha_usuario, false, usuario.no_login);
                if (!usuario.id_admin)
                {
                    if (usuario.cd_pessoa == 0)
                    {
                        var ExistPessoaCpfBase = BusinessPessoa.ExistsPessoByCpf(pessoaFisica.nm_cpf);
                        if (ExistPessoaCpfBase != null && ExistPessoaCpfBase.cd_pessoa > 0 && ExistPessoaCpfBase.cd_pessoa != pessoaFisica.cd_pessoa)
                            throw new PessoaBusinessException(Messages.msgExistPersonCpfBase + " " + ExistPessoaCpfBase.no_pessoa, null, FundacaoFisk.SGF.Web.Services.Pessoa.Business.PessoaBusinessException.TipoErro.ERRO_CPFJAEXISTENTE, false);
                        BusinessPessoa.PostPessoaEPessoaFisica(pessoaFisica);
                    }
                    usuario.cd_pessoa = pessoaFisica.cd_pessoa;
                    if (usuario.Empresas != null && usuario.Empresas.Count() > 0)
                    {
                        foreach (UsuarioEmpresaSGF esc in usuario.Empresas)
                        {
                            var pessoaEsc = new PessoaEscola
                            {
                                cd_escola = esc.cd_pessoa_empresa,
                                cd_pessoa = (int)usuario.cd_pessoa
                            };
                            if (pessoaEsc.cd_escola <= 0 || pessoaEsc.cd_pessoa <= 0)
                                throw new PessoaBusinessException("Não é possível inserir usuário sem estar vinculado com uma pessoa." + "  -  " + "cd_pessoa: " + pessoaEsc.cd_escola + " cd_pessoa_empresa: " + pessoaEsc.cd_escola, null, FundacaoFisk.SGF.Web.Services.Pessoa.Business.PessoaBusinessException.TipoErro.ERRO_CONVERSAO_DATA, false);
                            Dao.addEmpresaPessoa(pessoaEsc);
                        }
                    }
                }
                BusinessUsuario.PostUsuario(usuario);

                //Api Area Restrita CadastrarUsuario
                CadastrarUsuarioApiAreaRestrita(usuario, cdEmp);

                if (usuario.Empresas != null && usuario.Empresas.Count() > 0)
                    qtdEscolasUsuario = usuario.Empresas.Count();
                usuarioUISearch = BusinessUsuario.getUsuarioFromViewGrid(usuario.cd_usuario, qtdEscolasUsuario, usuario.isMasterUserLogado);
                transaction.Complete();
            }
            
            //usuarioUISearch = UsuarioUISearch.fromUsuarioWeb(usuarioWeb);
            return usuarioUISearch;
        }

        public MenusAreaRestritaRetorno getMenusAreaRestrita()
        {
            MenusAreaRestritaRetorno menus = null;
            if (BusinessApiAreRestrita.aplicaApiAreaRestrita() == true)
            {

                TokenAreaRestritaUI token = BusinessApiAreRestrita.ObterToken("listagemMenus", "");
                if (token != null)
                {
                     menus = BusinessApiAreRestrita.ListagemDosMenus(token.access_token);

                    
                }
                else
                {
                    menus =  null;
                }

                //libera o token
                BusinessApiAreRestrita.Logout(token.access_token, "GetMenusAreaRestrita", "");

            }
            else
            {
                menus = null;
            }

            return menus;

        }

        private void CadastrarUsuarioApiAreaRestrita(UsuarioWebSGF usuario, int cdEmp)
        {
            if (BusinessApiAreRestrita.aplicaApiAreaRestrita() == true)
            {
                

                TokenAreaRestritaUI token = BusinessApiAreRestrita.ObterToken( "inserirUsuario", usuario.no_login);

                if (token != null)
                {

                    
                    //verifica se o usuario já existe
                    //UserAreaRestritaUI user = BusinessApiAreRestrita.getDetalhesUsuario(urlApiAreaRestrita, token.access_token, usuario.cd_usuario.ToString());

                    
                        EscolaApiAreaRestritaBdUI escola = Dao.getEscolaApiAreaRestrita(cdEmp);
                        if (escola != null && escola.nm_cliente_integracao != null && escola.nm_cliente_integracao > 0 && usuario.nameAreaRestrita != null && usuario.emailAreaRestrita != null)
                        {
                            UserAreaRestritaUI userAreaRestritaSave = new UserAreaRestritaUI();
                            userAreaRestritaSave.name = usuario.nameAreaRestrita;
                            userAreaRestritaSave.email = usuario.emailAreaRestrita;
                            userAreaRestritaSave.id_fisk_sgf = usuario.cd_usuario.ToString();
                            userAreaRestritaSave.password = usuario.dc_senha_usuario;
                            userAreaRestritaSave.id_fisk_franchisee = escola.nm_cliente_integracao.ToString();
                            userAreaRestritaSave.menus = new List<string>();

                            if (usuario != null && usuario.menusAreaRestrita != null && usuario.menusAreaRestrita.Count > 0)
                            {
                                //adiciona os menus
                                foreach (string menu in usuario.menusAreaRestrita)
                                {
                                    userAreaRestritaSave.menus.Add(menu);
                                }
                            }
                            

                            
                            UserAreaRestritaCreateRetorno userCreated = BusinessApiAreRestrita.criarUsuario(token.access_token, userAreaRestritaSave);
                            if (userCreated != null && userCreated.user != null && userCreated.user.id > 0)
                            {
                                UsuarioWebSGF usuarioBd = DataAccessUsuario.findById(usuario.cd_usuario, false);

                                //Preenche o id_area_restrita -> descomentar quando tiver o campo
                                if (usuarioBd != null)
                                {
                                    usuarioBd.id_area_resrtrita = userCreated.user.id;
                                    DataAccessUsuario.saveChanges(false);
                                }
                            }
                        }
                    

                    

                    //libera o token
                    BusinessApiAreRestrita.Logout(token.access_token, "InserirUsuario", usuario.no_login);
                   
                }
            }
        }

        private void EditarUsuarioApiAreaRestrita(UsuarioWebSGF usuario, int cdEmp)
        {
            if (BusinessApiAreRestrita.aplicaApiAreaRestrita() == true)
            {
               
                TokenAreaRestritaUI token = BusinessApiAreRestrita.ObterToken("inserirUsuario", usuario.no_login);

                if (token != null)
                {


                    UsuarioWebSGF usuarioBd = DataAccessUsuario.findById(usuario.cd_usuario, false);

                    if (usuarioBd != null)
                    {

                    
                        if (usuarioBd.id_area_resrtrita != null)
                        {
                            //verifica se o usuario já existe lá
                            UserAreaRestritaDetalheRetorno user = BusinessApiAreRestrita.getDetalhesUsuario(token.access_token, usuarioBd.id_area_resrtrita.ToString());

                            if (user != null && user.user != null)
                            {

                                EscolaApiAreaRestritaBdUI escola = Dao.getEscolaApiAreaRestrita(cdEmp);
                                if (escola != null && escola.nm_cliente_integracao != null && escola.nm_cliente_integracao > 0 )
                                {
                                    UserAreaRestritaUI userAreaRestritaSave = new UserAreaRestritaUI();
                                    userAreaRestritaSave.name = usuario.nameAreaRestrita != user.user.name ? usuario.nameAreaRestrita : null;
                                    userAreaRestritaSave.email = usuario.emailAreaRestrita != user.user.email ? usuario.emailAreaRestrita : null;
                                    userAreaRestritaSave.id_fisk_sgf = usuario.cd_usuario.ToString();
                                    userAreaRestritaSave.password = !String.IsNullOrEmpty(usuario.dc_senha_usuario) ? usuario.dc_senha_usuario : null;
                                    userAreaRestritaSave.id_fisk_franchisee = escola.nm_cliente_integracao.ToString();
                                    userAreaRestritaSave.menus = new List<string>();

                                    if (usuario != null && usuario.menusAreaRestrita != null && usuario.menusAreaRestrita.Count > 0)
                                    {
                                        //adiciona os menus
                                        foreach (string menu in usuario.menusAreaRestrita)
                                        {
                                            userAreaRestritaSave.menus.Add(menu);
                                        }
                                    }
                                    

                                    UserAreaRestritaUpdateRetorno userUpdated = BusinessApiAreRestrita.updateUsuario(token.access_token, (int)usuarioBd.id_area_resrtrita, userAreaRestritaSave);

                                }
                            }
                            else
                            {
                                if (String.IsNullOrEmpty(usuario.dc_senha_usuario))
                                {
                                    usuario.dc_senha_usuario = usuarioBd.dc_senha_usuario;
                                }
                                CadastrarUsuarioApiAreaRestrita(usuario, cdEmp);
                            }

                        }
                        else
                        {
                                if (String.IsNullOrEmpty(usuario.dc_senha_usuario))
                                {
                                    usuario.dc_senha_usuario = usuarioBd.dc_senha_usuario;
                                }
                                CadastrarUsuarioApiAreaRestrita(usuario, cdEmp);
                        }

                    }

                    //libera o token
                    BusinessApiAreRestrita.Logout(token.access_token, "AtualizarUsuario", usuario.no_login);
                    
                }
            }
        }

        public void DeleteUsuarioApiAreaRestrita(List<UsuarioWebSGF> usuariosDeleteApi)
        {
            if (BusinessApiAreRestrita.aplicaApiAreaRestrita() == true)
            {
               

                TokenAreaRestritaUI token = BusinessApiAreRestrita.ObterToken( "DeleteUsuarios", String.Join(",", usuariosDeleteApi.Select(x => x.no_login)));

                if (token != null)
                {
                    foreach (UsuarioWebSGF usuarioWebSgf in usuariosDeleteApi)
                    {
                        
                        //verifica se o usuario  existe no banco de lá
                        UserAreaRestritaDetalheRetorno userAreaRestrita = BusinessApiAreRestrita.getDetalhesUsuario( token.access_token, usuarioWebSgf.id_area_resrtrita.ToString());

                        if (userAreaRestrita != null)
                        {

                            UserAreaRestritaUI userDeleted = BusinessApiAreRestrita.deleteUsuarioAreaRestrita( token.access_token, usuarioWebSgf.id_area_resrtrita.ToString());


                        }
                        
                        
                    }
                    //libera o token
                    BusinessApiAreRestrita.Logout(token.access_token, "DeleteUsuarios", String.Join(",", usuariosDeleteApi.Select(x => x.no_login)));
                }
            }
        }

        public UsuarioUISearch PostEditUsuario(UsuarioWebSGF usuario, int cdEmp)
        {
            UsuarioWebSGF usuarioWebSGF = new UsuarioWebSGF();
            UsuarioUISearch usuarioUISearch = new UsuarioUISearch();
            int qtdEscolasUsuario = 0;

            this.sincronizaContexto(Dao.DB());
            if (usuario.id_admin && usuario.id_usuario_ativo)
            {
                bool existSysAdmin = false;
                List<UsuarioEmpresaSGF> userEmp = new List<UsuarioEmpresaSGF>();
                if (usuario.Empresas != null)
                    userEmp = usuario.Empresas.ToList();
                int[] cdEscolas = new int[userEmp.Count()];
                for (int i = 0; i < userEmp.Count; i++)
                    cdEscolas[i] = userEmp[i].cd_pessoa_empresa;
                if (usuario.Empresas != null)
                    existSysAdmin = BusinessUsuario.verificaExisteSysAdminAtivosEscolas(cdEscolas, usuario.no_login);
                else
                    existSysAdmin = BusinessUsuario.verificaExisteSysAdminAtivosEscolas(usuario.no_login);
                if (existSysAdmin)
                    throw new UsuarioBusinessException(Messages.msgUserSysAdminExistEscolas, null, UsuarioBusinessException.TipoErro.ERRO_JA_EXISTE_SYSADMIN_ESCOLAS, false);
            }
            //using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.READ_COMMITED, Dao.DB(), TransactionScopeBuilder.TransactionTime.HIGHT))
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED, Dao.DB(), TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                //Verifica se era normal e passou a ser sysAdmin
                bool usuarioContextAdmin = BusinessUsuario.verifUsuarioAdmin(usuario.cd_usuario);
                if (usuario.id_admin && !usuarioContextAdmin)
                    throw new UsuarioBusinessException(Messages.msgErroAlterUsuarioForSysAdmin, null, UsuarioBusinessException.TipoErro.ERRO_SYSADMIN_USUARIO, false);
                //Verifica se era usuário sysAdmin e passou a ser normal
                if (!usuario.id_admin && usuarioContextAdmin)
                    throw new UsuarioBusinessException(Messages.msgErroAlterSysAdmin, null, UsuarioBusinessException.TipoErro.ERRO_USUARIO_COMUM_SYSADMIN, false);
                BusinessUsuario.verificaSenha(usuario.dc_senha_usuario, true, usuario.no_login);
                if (!usuario.id_admin)
                {
                    if (usuario.cd_pessoa == 0)
                    {
                        var ExistPessoaCpfBase = BusinessPessoa.ExistsPessoByCpf(usuario.PessoaFisica.nm_cpf);
                        if (ExistPessoaCpfBase != null && ExistPessoaCpfBase.cd_pessoa > 0 && ExistPessoaCpfBase.cd_pessoa != usuario.cd_pessoa)
                            throw new PessoaBusinessException(Messages.msgExistPersonCpfBase + " " + ExistPessoaCpfBase.no_pessoa, null, FundacaoFisk.SGF.Web.Services.Pessoa.Business.PessoaBusinessException.TipoErro.ERRO_CPFJAEXISTENTE, false);
                        BusinessPessoa.PostPessoaEPessoaFisica(usuario.PessoaFisica);
                    }

                    var pessoaEsc = new PessoaEscola
                    {
                        cd_escola = cdEmp,
                        cd_pessoa = (int)usuario.cd_pessoa
                        //Altera somente a referência e não a pessoa inteira.
                    };
                    usuario.PessoaFisica = null;
                    Dao.addEmpresaPessoa(pessoaEsc);
                }
                else
                    usuario.cd_pessoa = null;
                BusinessUsuario.PutUsuario(usuario);
                if (usuario.Empresas != null)
                {
                    qtdEscolasUsuario = usuario.Empresas.Count();
                    BusinessUsuario.crudEmpresasUsuario(usuario.Empresas.ToList(), usuario.cd_usuario);
                }

                //Api area restrita UpdateUsuario
                EditarUsuarioApiAreaRestrita(usuario, cdEmp);

                transaction.Complete();
            }
            usuarioUISearch = BusinessUsuario.getUsuarioFromViewGrid(usuario.cd_usuario, qtdEscolasUsuario, usuario.isMasterUserLogado);
            return usuarioUISearch;
        }
        #endregion

    }
}

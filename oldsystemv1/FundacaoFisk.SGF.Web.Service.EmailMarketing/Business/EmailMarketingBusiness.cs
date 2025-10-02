using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Services.EmailMarketing.Business;
//using FundacaoFisk.SGF.Web.Service.EmailMarketing.Model;
using FundacaoFisk.SGF.Web.Services.EmailMarketing.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.EmailMarketing.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IBusiness;
using System.Data.Entity.Infrastructure;
using FundacaoFisk.SGF.Web.Service.EmailMarketing.Model;
using Componentes.GenericBusiness;
using FundacaoFisk.SGF.Web.Services.Log.Comum.IBusiness;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Data;

namespace FundacaoFisk.SGF.Web.Services.EmailMarketing.Business
{
    public class EmailMarketingBusiness : IEmailMarketingBusiness
    {
        /// <summary>
        /// Declaração de Interfaces
        /// </summary>
        public IListaEnderecoMalaDataAccess DaoListaEnderecoMala { get; set; }
        public IListaNaoInscritoDataAccess DaoListaNaoInscrito { get; set; }
        public IMalaDiretaDataAccess DaoMalaDireta { get; set; }
        private ILogGeralBusiness BusinessLogGeral { get; set; }
        
        /// <summary>
        /// Método Construtor do Dao
        /// </summary>

        public EmailMarketingBusiness(ILogGeralBusiness businessLogGeral, IListaEnderecoMalaDataAccess daoListaEnderecoMala, IListaNaoInscritoDataAccess daoListaNaoInscrito, IMalaDiretaDataAccess daoMalaDireta)
        {
            if (daoListaEnderecoMala == null || businessLogGeral == null || daoListaNaoInscrito == null)
                throw new ArgumentNullException("EmailMarketingBusiness");
            DaoListaEnderecoMala = daoListaEnderecoMala;
            DaoListaNaoInscrito = daoListaNaoInscrito;
            DaoMalaDireta = daoMalaDireta;
            BusinessLogGeral = businessLogGeral;
        }

        public void configuraUsuario(int cdUsuario, int cd_empresa)
        {
            ((SGFWebContext)this.DaoListaEnderecoMala.DB()).IdUsuario = ((SGFWebContext)this.DaoListaNaoInscrito.DB()).IdUsuario = cdUsuario;
            ((SGFWebContext)this.DaoListaEnderecoMala.DB()).cd_empresa = ((SGFWebContext)this.DaoListaNaoInscrito.DB()).cd_empresa = cd_empresa;
            BusinessLogGeral.configuraUsuario(cdUsuario, cd_empresa);
        }

        public void sincronizarContextos(DbContext dbContext)
        {
            //this.DaoListaEnderecoMala.sincronizaContexto(dbContext);
            //this.DaoListaNaoInscrito.sincronizaContexto(dbContext);
            //BusinessLogGeral.sincronizaContexto(dbContext);
        }

        #region Lista de endereços

        public IEnumerable<ListaNaoInscrito> getListagemEnderecos(int cd_empresa, string no_pessoa, int status, string email, byte id_tipo_cadastro)
        {
            IEnumerable<ListaNaoInscrito> retorno = new List<ListaNaoInscrito>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED, DaoListaEnderecoMala.DB(), TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                retorno = DaoListaEnderecoMala.getListagemEnderecos(cd_empresa, no_pessoa, status, email, id_tipo_cadastro).ToList();
            }
            return retorno;
        }

        public IEnumerable<RptListagemEndereco> getRptListagemEnderecos(int cd_mala_direta, int cd_empresa, string no_pessoa, int status, string email, byte id_tipo_cadastro)
        {
            List<RptListagemEndereco> retorno = new List<RptListagemEndereco>();
            
            this.sincronizarContextos(DaoListaEnderecoMala.DB());
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED, DaoListaEnderecoMala.DB(), TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                if (cd_mala_direta > 0)
                    retorno = DaoListaEnderecoMala.getRptListagemEnderecosMalaDireta(cd_empresa, cd_mala_direta).ToList();
                else
                    retorno = DaoListaEnderecoMala.getRptListagemEnderecos(cd_empresa, no_pessoa, status, email, id_tipo_cadastro).ToList();
                transaction.Complete();
            }
            return retorno;
        }

        #endregion

        #region Lista não inscrito

        public IEnumerable<ListaNaoInscrito> getListaNaoIncritoEnderecos(int cd_empresa, string no_pessoa, int status, string email, byte id_tipo_cadastro)
        {
            IEnumerable<ListaNaoInscrito> retorno = new List<ListaNaoInscrito>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED, DaoListaNaoInscrito.DB(), TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                retorno = DaoListaNaoInscrito.getListaNaoIncritoEnderecos(cd_empresa, no_pessoa, status, email, id_tipo_cadastro).ToList();
            }
            return retorno;
        }

        public bool crudListaNaoIncritoEndereco(int cd_escola, List<ListaNaoInscrito> listaEnderecosView)
        {
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                this.sincronizarContextos(DaoListaNaoInscrito.DB());
                List<ListaNaoInscrito> listaEnderecosContext = DaoListaNaoInscrito.getListaNaoIncritoEscola(cd_escola).ToList();
                IEnumerable<ListaNaoInscrito> followUpEscolaComCodigo = from hpts in listaEnderecosView
                                                                        where hpts.cd_lista_nao_inscrito != 0
                                                                        select hpts;
                IEnumerable<ListaNaoInscrito> istaEnderecosDeleted = listaEnderecosContext.Where(tc => !followUpEscolaComCodigo.Any(tv => tc.cd_lista_nao_inscrito == tv.cd_lista_nao_inscrito));

                if (istaEnderecosDeleted != null)
                    foreach (var item in istaEnderecosDeleted)
                        if (item != null)
                            DaoListaNaoInscrito.deleteContext(item, false);

                foreach (var item in listaEnderecosView)
                {
                    if (item.cd_lista_nao_inscrito == 0)
                    {
                        item.cd_escola = cd_escola;
                        DaoListaNaoInscrito.addContext(item, false);
                    }
                }
                DaoListaNaoInscrito.saveChanges(false);
                transaction.Complete();
            }
            return true;
        }

        public bool retirarEmailListaEndereco(int cd_empresa, int cd_cadastro, int id_cadastro)
        {
            if (!DaoListaNaoInscrito.jaExisteNaoIncrito(cd_empresa, cd_cadastro, id_cadastro) && DaoListaEnderecoMala.existeEmailListagemEscola(cd_empresa, cd_cadastro, id_cadastro))
                DaoListaNaoInscrito.add(new ListaNaoInscrito { cd_escola = cd_empresa, cd_cadastro = cd_cadastro, id_cadastro = (byte)id_cadastro }, false);
            return true;
        }

        #endregion

        #region Compor Mensagens
        public bool postComporMensagemEnviar(MalaDireta mala_direta)
        {
            bool retorno = false;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED, DaoListaNaoInscrito.DB(), TransactionScopeBuilder.TransactionTime.HIGHT))
            {
                this.sincronizarContextos(DaoListaNaoInscrito.DB());               

                //Inclui a nova mala direta:
                mala_direta.dh_mala_direta = DateTime.UtcNow;
                mala_direta.dt_mala_direta = DateTime.Now.Date;
                mala_direta.id_tipo_mala = 1;

                //Cria a auditoria individual:
                SGFWebContext dbComp = new SGFWebContext();
                BusinessLogGeral.geraLogIndividual(mala_direta.cd_escola, mala_direta.cd_usuario.Value, Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["MalaDireta"].ToString()), (int)FundacaoFisk.SGF.GenericModel.TipoLog.TipoLogEnum.INCLUSAO, mala_direta.cd_mala_direta,
                    mala_direta, null);
                
                mala_direta = DaoMalaDireta.add(mala_direta, false);
                if (mala_direta.cd_mala_direta > 0)
                    retorno = true;

                //Inclui os novos não inscritos selecionados neste momento:
                List<ListaNaoInscrito> listaNaoInscritoEscola = DaoListaNaoInscrito.getListaNaoIncritoEscola(mala_direta.cd_escola).ToList();
                IEnumerable<ListaEnderecoMala> listaEnderecosNaoInscritos = mala_direta.ListasEnderecoMalaComNaoInscritos != null ? mala_direta.ListasEnderecoMalaComNaoInscritos.Where(lc => 
                        !mala_direta.ListasEnderecoMala.Where(ls => ls.cd_cadastro == lc.cd_cadastro && ls.id_cadastro == lc.id_cadastro).Any()
                        && !listaNaoInscritoEscola.Where(lnie => lnie.cd_cadastro == lc.cd_cadastro && lnie.id_cadastro == lc.id_cadastro && lnie.cd_escola == mala_direta.cd_escola).Any()
                    ) : new List<ListaEnderecoMala>();
                List<ListaNaoInscrito> listasNaoIncritos = listaEnderecosNaoInscritos.Select(x => new ListaNaoInscrito() { 
                    cd_escola = mala_direta.cd_escola, 
                    id_cadastro = x.id_cadastro,
                    cd_cadastro = x.cd_cadastro
                }).ToList();

                foreach(ListaNaoInscrito listaNaoInscrito in listasNaoIncritos)
                    DaoListaNaoInscrito.add(listaNaoInscrito, false);

                DaoMalaDireta.saveChanges(false);
                transaction.Complete();
            }
            //Envia as mensagens:
            SendEmail sendEmail = new SendEmail();

            sendEmail.assunto = mala_direta.dc_assunto;
            SendEmail.configurarEmailSection(sendEmail);
            if(!mala_direta.nm_enviados.HasValue)
                mala_direta.nm_enviados = 0;

            InserirDocumentosAnexo(sendEmail, mala_direta);
            List<ListaEnderecoMala> listasEnderecoMala = mala_direta.ListasEnderecoMala.ToList();
            for (int i = 0; i < listasEnderecoMala.Count; i++)
            {
                ListaEnderecoMala listaEnderecoMala = listasEnderecoMala[i];
                sendEmail.destinatario = listaEnderecoMala.dc_email_cadastro;

                //Realiza a macrosubstituição:
                sendEmail.mensagem = mala_direta.tx_msg_completa;
                string[] nomes = listaEnderecoMala.no_pessoa.Split(' ');
                string primeiroNome = "";
                if (nomes.Length > 0)
                    primeiroNome = nomes[0];
                if (!String.IsNullOrEmpty(sendEmail.mensagem)){
                    sendEmail.mensagem = sendEmail.mensagem.Replace("#nomecompleto#", listaEnderecoMala.no_pessoa);
                    sendEmail.mensagem = sendEmail.mensagem.Replace("#primeironome#", primeiroNome);
                    sendEmail.mensagem = sendEmail.mensagem.Replace("#linksair#", listaEnderecoMala.url_nao_inscrito);
                }

                if (SendEmail.EnviarEmail(sendEmail))
                    mala_direta.nm_enviados++;

                if (i % 100 == 0)
                {
                    using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED, DaoListaNaoInscrito.DB(), TransactionScopeBuilder.TransactionTime.HIGHT))
                    {
                        MalaDireta malaDiretaContext = DaoMalaDireta.findById(mala_direta.cd_mala_direta, false);

                        malaDiretaContext.nm_enviados = mala_direta.nm_enviados;
                        DaoMalaDireta.saveChanges(false);
                        transaction.Complete();
                    }
                }
            }
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED, DaoListaNaoInscrito.DB(), TransactionScopeBuilder.TransactionTime.HIGHT))
            {
                MalaDireta malaDiretaContext = DaoMalaDireta.findById(mala_direta.cd_mala_direta, false);

                malaDiretaContext.nm_enviados = mala_direta.nm_enviados;
                DaoMalaDireta.saveChanges(false);
                transaction.Complete();
            }
            return retorno;
        }

        public int salvarMalaDiretaEtiqueta(MalaDireta mala_direta)
        {
            //bool retorno = false;
            int cd_mala_direta = 0;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED, DaoListaNaoInscrito.DB(), TransactionScopeBuilder.TransactionTime.HIGHT))
            {

                //Inclui a nova mala direta:
                mala_direta.dh_mala_direta = DateTime.UtcNow;
                mala_direta.dt_mala_direta = DateTime.Now.Date;
                mala_direta.id_tipo_mala = 2; // Etiqueta;
                mala_direta.nm_enviados = 0;

                //Cria a auditoria individual:
                SGFWebContext dbComp = new SGFWebContext();
               /* BusinessLogGeral.geraLogIndividual(mala_direta.cd_escola, mala_direta.cd_usuario.Value, Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["MalaDireta"].ToString()), (int)FundacaoFisk.SGF.GenericModel.TipoLog.TipoLogEnum.INCLUSAO, mala_direta.cd_mala_direta,
                    mala_direta, null);*/

                mala_direta = DaoMalaDireta.add(mala_direta, false);
                if (mala_direta.cd_mala_direta > 0)
                    cd_mala_direta = mala_direta.cd_mala_direta;

                //Inclui os novos não inscritos selecionados neste momento:
                List<ListaNaoInscrito> listaNaoInscritoEscola = DaoListaNaoInscrito.getListaNaoIncritoEscola(mala_direta.cd_escola).ToList();
                IEnumerable<ListaEnderecoMala> listaEnderecosNaoInscritos = mala_direta.ListasEnderecoMalaComNaoInscritos != null ? mala_direta.ListasEnderecoMalaComNaoInscritos.Where(lc =>
                        !mala_direta.ListasEnderecoMala.Where(ls => ls.cd_cadastro == lc.cd_cadastro && ls.id_cadastro == lc.id_cadastro).Any()
                        && !listaNaoInscritoEscola.Where(lnie => lnie.cd_cadastro == lc.cd_cadastro && lnie.id_cadastro == lc.id_cadastro && lnie.cd_escola == mala_direta.cd_escola).Any()
                    ) : new List<ListaEnderecoMala>();
                List<ListaNaoInscrito> listasNaoIncritos = listaEnderecosNaoInscritos.Select(x => new ListaNaoInscrito()
                {
                    cd_escola = mala_direta.cd_escola,
                    id_cadastro = x.id_cadastro,
                    cd_cadastro = x.cd_cadastro
                }).ToList();

                foreach (ListaNaoInscrito listaNaoInscrito in listasNaoIncritos)
                    DaoListaNaoInscrito.add(listaNaoInscrito, false);

                DaoMalaDireta.saveChanges(false);
                transaction.Complete();
            }
           
            //using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED, DaoListaNaoInscrito.DB(), TransactionScopeBuilder.TransactionTime.HIGHT))
            //{
            //    MalaDireta malaDiretaContext = DaoMalaDireta.findById(mala_direta.cd_mala_direta, false);

            //    malaDiretaContext.nm_enviados = mala_direta.nm_enviados;
            //    DaoMalaDireta.saveChanges(false);
            //    transaction.Complete();
            //}
            return cd_mala_direta;
        }

        public DataTable gerarEtiqueta(int cd_mala_direta) 
        {
            try
            {
                return DaoMalaDireta.gerarEtiqueta(cd_mala_direta);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void InserirDocumentosAnexo(SendEmail sendMail, MalaDireta mala_direta)
        {
            try
            {
                string pathArquivoAnexo = ConfigurationManager.AppSettings["caminhoUploads"] + "\\MailMarketing\\Anexos\\" + mala_direta.cd_escola;
                if ((mala_direta.nome_arquivos_anexo != null && mala_direta.nome_arquivos_anexo.Count > 0) && Directory.Exists(pathArquivoAnexo))
                {
                    // criar caminho aonde arquivos serão salvos.
                    string novoCaminhoAnexo = pathArquivoAnexo + "\\" + mala_direta.cd_mala_direta;
                    DirectoryInfo dicInfo = new DirectoryInfo(novoCaminhoAnexo);
                    if (!dicInfo.Exists) // se diretorio não existir, criar.
                        dicInfo.Create();

                    sendMail.Anexos = new Dictionary<string, Stream>();
                    foreach (var nm_arq in mala_direta.nome_arquivos_anexo)
                    {
                        var pathOrigem = Path.Combine(pathArquivoAnexo + "\\", nm_arq.Key);
                        var pathDestino = Path.Combine(novoCaminhoAnexo + "\\", nm_arq.Value);

                        if (File.Exists(pathOrigem) && Directory.Exists(novoCaminhoAnexo))
                        {
                            File.Move(pathOrigem, pathDestino);
                            Thread.Sleep(2000); // aguarda 2 segundos, para criação do arquivo.   

                            using (var stream = File.Open(pathDestino, FileMode.Open, FileAccess.Write, FileShare.Read))
                            {
                                sendMail.Anexos.Add(nm_arq.Value, stream);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public MalaDireta visualizarEtiqueta(MalaDireta mala_direta)
        {
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED, DaoListaNaoInscrito.DB(), TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                this.sincronizarContextos(DaoListaNaoInscrito.DB());

                //Pesquisa a lista de não inscritos:
                List<ListaNaoInscrito> listaNaoInscritos = DaoListaNaoInscrito.getListaNaoIncritoEscola(mala_direta.cd_escola).ToList();

                //Pesquisa a lista dos inscritos:
                bool is_prospect = false, is_aluno = false, is_cliente = false, is_pessoa_relacionada = false, is_funcionario_professor = false, is_alunos_inadimplentes = false;

                foreach (MalaDiretaCadastro malaDiretaCadastro in mala_direta.MalasDiretaCadastro)
                    switch (malaDiretaCadastro.id_cadastro)
                    {
                        case (byte)MalaDireta.TipoCadastro.PROSPECT:
                            is_prospect = true;
                            break;
                        case (byte)MalaDireta.TipoCadastro.ALUNO:
                            is_aluno = true;
                            break;
                        case (byte)MalaDireta.TipoCadastro.CLIENTE:
                            is_cliente = true;
                            break;
                        case (byte)MalaDireta.TipoCadastro.PESSOARELACIONADA:
                            is_pessoa_relacionada = true;
                            break;
                        case (byte)MalaDireta.TipoCadastro.FUNCIONARIOPROFESSOR:
                            is_funcionario_professor = true;
                            break;
                        case (byte)MalaDireta.TipoCadastro.ALUNOINADIMPLENTE:
                            is_alunos_inadimplentes = true;
                            break;
                    }
                List<ListaEnderecoMala> listaEnderecosMala = new List<ListaEnderecoMala>();

                if (is_prospect)
                    listaEnderecosMala.AddRange(DaoListaEnderecoMala.getListagemEnderecosComporMensagemProspect(mala_direta).ToList());
                if (is_aluno)
                    listaEnderecosMala.AddRange(DaoListaEnderecoMala.getListagemEnderecosComporMensagemAluno(mala_direta).ToList());
                if (is_cliente)
                    listaEnderecosMala.AddRange(DaoListaEnderecoMala.getListagemEnderecosComporMensagemCliente(mala_direta).ToList());
                if (is_pessoa_relacionada)
                    listaEnderecosMala.AddRange(DaoListaEnderecoMala.getListagemEnderecosComporMensagemPessoaRelacionada(mala_direta, false).ToList());
                if (is_funcionario_professor)
                    listaEnderecosMala.AddRange(DaoListaEnderecoMala.getListEndComporMsgFuncProfissao(mala_direta).ToList());
                if(is_alunos_inadimplentes)
                    listaEnderecosMala.AddRange(DaoListaEnderecoMala.getListagemEndAlunosInadimplentes(mala_direta).ToList());

                var listaEnderecosMalaComEndereco = new List<ListaEnderecoMala>();
                foreach (var listaMala in listaEnderecosMala)
                {
                    if (DaoListaEnderecoMala.existEnderecoPrincipalPessoa(listaMala.cd_cadastro))
                        listaEnderecosMalaComEndereco.Add(listaMala);
                }

                if (is_prospect)
                {
                    mala_direta.ListasEnderecoMala = listaEnderecosMalaComEndereco.OrderBy(x => x.no_pessoa).ToList();
                }

                if (is_aluno) {
                    mala_direta.ListasEnderecoMala = listaEnderecosMalaComEndereco.OrderBy(x => x.no_pessoa).ToList();
                }

                if (is_cliente) {
                    mala_direta.ListasEnderecoMala = listaEnderecosMalaComEndereco.OrderBy(x => x.no_pessoa).ToList();
                }

                if (is_pessoa_relacionada) {
                    mala_direta.ListasEnderecoMala = listaEnderecosMalaComEndereco.OrderBy(x => x.no_pessoa).ToList();
                }

                if (is_funcionario_professor) {
                    mala_direta.ListasEnderecoMala = listaEnderecosMalaComEndereco.OrderBy(x => x.no_pessoa).ToList();
                }

                if (is_alunos_inadimplentes) {
                    mala_direta.ListasEnderecoMala = listaEnderecosMalaComEndereco.OrderBy(x => x.no_pessoa).ToList();
                }

                transaction.Complete();    
            }
            
            return mala_direta;
        }

        public MalaDireta postComporMensagem(MalaDireta mala_direta)
        {
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED, DaoListaNaoInscrito.DB(), TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                this.sincronizarContextos(DaoListaNaoInscrito.DB());

                //Pesquisa a lista de não inscritos:
                List<ListaNaoInscrito> listaNaoInscritos = DaoListaNaoInscrito.getListaNaoIncritoEscola(mala_direta.cd_escola).ToList();
                
                //Pesquisa a lista dos inscritos:
                bool is_prospect = false, is_aluno = false, is_cliente = false, is_pessoa_relacionada = false, is_funcionario_professor = false;

                foreach (MalaDiretaCadastro malaDiretaCadastro in mala_direta.MalasDiretaCadastro)
                    switch (malaDiretaCadastro.id_cadastro)
                    {
                        case (byte)MalaDireta.TipoCadastro.PROSPECT:
                            is_prospect = true;
                            break;
                        case (byte)MalaDireta.TipoCadastro.ALUNO:
                            is_aluno = true;
                            break;
                        case (byte)MalaDireta.TipoCadastro.CLIENTE:
                            is_cliente = true;
                            break;
                        case (byte)MalaDireta.TipoCadastro.PESSOARELACIONADA:
                            is_pessoa_relacionada = true;
                            break;
                        case (byte)MalaDireta.TipoCadastro.FUNCIONARIOPROFESSOR:
                            is_funcionario_professor = true;
                            break;
                    }
                List<ListaEnderecoMala> listaEnderecosMala = new List<ListaEnderecoMala>();
                
                if(is_prospect)
                    listaEnderecosMala.AddRange(DaoListaEnderecoMala.getListagemEnderecosComporMensagemProspect(mala_direta).ToList());
                if(is_aluno)
                    listaEnderecosMala.AddRange(DaoListaEnderecoMala.getListagemEnderecosComporMensagemAluno(mala_direta).ToList());
                if (is_cliente)
                    listaEnderecosMala.AddRange(DaoListaEnderecoMala.getListagemEnderecosComporMensagemCliente(mala_direta).ToList());
                if (is_pessoa_relacionada)
                    listaEnderecosMala.AddRange(DaoListaEnderecoMala.getListagemEnderecosComporMensagemPessoaRelacionada(mala_direta).ToList());
                if (is_funcionario_professor)
                    listaEnderecosMala.AddRange(DaoListaEnderecoMala.getListEndComporMsgFuncProfissao(mala_direta).ToList());

                //Na base de dados o email é não nulo:
                listaEnderecosMala = listaEnderecosMala.Where(le => !String.IsNullOrEmpty(le.dc_email_cadastro)).ToList();

                //Remove os não inscritos dos inscritos:
                if (listaNaoInscritos != null && listaNaoInscritos.Count > 0)
                    listaEnderecosMala = listaEnderecosMala.Where(em => !listaNaoInscritos.Where(ni => ni.cd_cadastro == em.cd_cadastro && ni.id_cadastro == em.id_cadastro).Any()).ToList();
                mala_direta.ListasEnderecoMala = listaEnderecosMala.OrderBy(x => x.no_pessoa).ToList();

                transaction.Complete();
            }
            return mala_direta;
        }
        #endregion

        #region Mala Direta

        public IEnumerable<MalaDireta> searchHistoricoMalaDireta(Componentes.Utils.SearchParameters parametros, string dc_assunto, DateTime? dt_mala_direta, int cd_empresa, int id_tipo_mala)
        {
            IEnumerable<MalaDireta> retorno = new List<MalaDireta>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                {
                    parametros.sort = "dt_mala_direta";
                    parametros.sortOrder = Componentes.Utils.SortDirection.Descending;
                }
                parametros.sort = parametros.sort.Replace("dta_mala_direta", "dt_mala_direta");
                retorno = DaoMalaDireta.searchHistoricoMalaDireta(parametros, dc_assunto, dt_mala_direta, cd_empresa, id_tipo_mala);
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<ListaNaoInscrito> getListagemEnderecosMalaDireta(int cd_empresa, int cd_mala_direta)
        {
            IEnumerable<ListaNaoInscrito> retorno = new List<ListaNaoInscrito>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = DaoListaEnderecoMala.getListagemEnderecosMalaDireta(cd_empresa, cd_mala_direta).ToList();
            }
            return retorno;
        }
        public IEnumerable<MalaDireta> getMalaDiretaPorAluno(Componentes.Utils.SearchParameters parametros, int cd_pessoa, int cd_empresa, string assunto, DateTime? dtaIni, DateTime? dtaFim)
        {
            IEnumerable<MalaDireta> retorno = new List<MalaDireta>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "dc_assunto";
                parametros.sort = parametros.sort.Replace("dta_mala_direta", "dt_mala_direta");
                retorno =  DaoMalaDireta.getMalaDiretaPorAluno(parametros, cd_pessoa, cd_empresa, assunto, dtaIni, dtaFim);
                transaction.Complete();
            }
            return retorno;
        }
        public MalaDireta getEditViewMalaDireta(int cd_mala_direta, int cd_empresa)
        {
            MalaDireta retorno = new MalaDireta();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = DaoMalaDireta.getMalaDiretaEditView(cd_mala_direta, cd_empresa);
                if (retorno != null)
                {
                    retorno.MalasDiretaCurso = DaoMalaDireta.getCursosMalaDireta(cd_mala_direta, cd_empresa).ToList();
                    retorno.MalasDiretaProduto = DaoMalaDireta.getProdutosMalaDireta(cd_mala_direta, cd_empresa).ToList();
                    retorno.MalasDiretaPeriodo = DaoMalaDireta.getPeriodosMalaDireta(cd_mala_direta, cd_empresa).ToList();
                    retorno.MalasDiretaCadastro = DaoMalaDireta.getTiposPessoaMalaDireta(cd_mala_direta, cd_empresa).ToList();
                }
                transaction.Complete();
            }
            return retorno;
        }

        #endregion

        #region CartaoPostal
        public MalaDireta getMalaDiretaForView(int cd_mala_direta, int cd_empresa)
        {
            MalaDireta retorno = new MalaDireta();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = DaoMalaDireta.getMalaDiretaForView(cd_mala_direta, cd_empresa);
                transaction.Complete();
            }
            return retorno;
        }
        #endregion
    }
}

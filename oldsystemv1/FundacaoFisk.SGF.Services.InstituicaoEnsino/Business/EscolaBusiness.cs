using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using Componentes.GenericBusiness;
using FundacaoFisk.SGF.GenericModel;
using System.Transactions;
using Componentes.GenericModel;
using FundacaoFisk.SGF.Web.Services.Usuario.Comum;
using FundacaoFisk.SGF.Web.Services.Usuario.Business;
using Componentes.Utils;
using FundacaoFisk.SGF.Services.InstituicaoEnsino.Model;
using FundacaoFisk.SGF.Web.Services.Auth.Comum;
using FundacaoFisk.SGF.Web.Services.Auth.Business;
using FundacaoFisk.SGF.Web.Services.Pessoa.Comum.Business;
using FundacaoFisk.SGF.Web.Services.InstituicaoEnsino.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.InstituicaoEnsino.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Pessoa.Business;
using Componentes.Utils.Messages;
using FundacaoFisk.SGF.GenericModel.Partial;
using FundacaoFisk.SGF.Services.Coordenacao.Business;
using FundacaoFisk.SGF.Utils;
using FundacaoFisk.SGF.Utils.Messages;
using FundacaoFisk.SGF.Web.Services.Empresa.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Pessoa.Comum.DataAccess;
using FundacaoFisk.SGF.Web.Services.Pessoa.DataAccess;
using FundacaoFisk.SGF.Web.Services.Usuario.Model;
using Messages = FundacaoFisk.SGF.Utils.Messages.Messages;
using log4net;

namespace FundacaoFisk.SGF.Web.Services.InstituicaoEnsino.Business
{
    using FundacaoFisk.SGF.Web.Services.Secretaria.Comum;
    using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IBusiness;
    using FundacaoFisk.SGF.Web.Services.Secretaria.Model;
    using System.Globalization;
    using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IBusiness;
    using FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess;
    using FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess;
    using FundacaoFisk.SGF.Web.Services.Biblioteca.Comum.IBusiness;
    using FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess;
    using FundacaoFisk.SGF.Web.Services.Financeiro.Model;
    using FundacaoFisk.SGF.Web.Services.Secretaria.Business;
    using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;
    using FundacaoFisk.SGF.Web.Services.Coordenacao.Business;
    using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
    using FundacaoFisk.SGF.Web.Services.Financeiro.Business;
    using System.Data.Entity;
    using System.Collections;
    using FundacaoFisk.SGF.Web.Service.Biblioteca.Model;
    using System.Data.Entity.Infrastructure;
    using Componentes.GenericBusiness.Comum;
    using FundacaoFisk.SGF.Web.Services.Secretaria.Comum.IDataAccess;
    using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess;
    using System.IO;
    using System.Diagnostics.Contracts;
    using static FundacaoFisk.SGF.GenericModel.Turma;
    using System.Data;
    using FundacaoFisk.SGF.Web.Services.InstituicaoEnsino.DataAccess;
    using log4net.Repository.Hierarchy;
    using FundacaoFisk.SGF.Services.InstituicaoEnsino.Controllers;
    using log4net;

    public class EscolaBusiness : IEscolaBusiness
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(EscolaBusiness));

        /// <summary>
        /// Declarações de  interfaces
        /// </summary>
        public IEscolaDataAccess Dao { get; set; }
        public IUsuarioDataAccess DaoUsuario { get; set; }
        private IUsuarioBusiness BusinessUsuario { get; set; }
        private IPessoaBusiness BusinessPessoa { get; set; }
        private ILocalidadeBusiness BusinessLoc { get; set; }
        public IParametrosDataAccess DaoParametro { get; set; }
        public ISysAppDataAccess DaoSysApp { get; set; }
        private IEmpresaBusiness BusinessEmpresa { get; set; }
        private IAuthBusiness BusinessAuth { get; set; }
        private IAlunoBusiness BusinessAluno { get; set; }
        private ICoordenacaoBusiness BusinessCoordenacao { get; set; }
        private ITurmaBusiness BusinessTurma { get; set; }
        public IMatriculaBusiness BusinessMatricula { get; set; }
        public IFinanceiroBusiness BusinessFinanceiro { get; set; }
        public ISecretariaBusiness BusinessSecretaria { get; set; }
        public IBibliotecaBusiness BusinessBiblioteca { get; set; }
        public ICursoBusiness BusinessCurso { get; set; }
        public IFiscalBusiness BusinessFiscal { get; set; }
        public IAditamentoDataAccess DataAccessAditamento { get; set; }
        public IMovimentoDataAccess DataAccessMovimento { get; set; }
 		public IPessoaEscolaDataAccess DataAccessPessoaEscola { get; set; }
        public IItemMovimentoDataAccess DataAccessItemMovimento { get; set; }
        public IItemEscolaDataAccess DataAccessItemEscola { get; set; }
        public ITipoItemDataAccess DataAccessTipoItem { get; set; }
        public ITipoNotaFiscalDataAccess DataAccessTipoNotaFiscal { get; set; }
        public IFinanceiroBusiness BusinessFinan { get; set; }
        public IPlanoTituloDataAccess DataAccessPlanoTitulo { get; set; }
        public ITituloDataAccess DataAccessTitulo { get; set; }
        public ILocalMovtoDataAccess DataAccessLocalMovtoDataAccess { get; set; }
        public ITurmaEscolaEmpresaDataAccess DataAccessTurmaEscola { get; set; }
        public IAtividadeEscolaAtividadeEmpresaDataAccess DataAccessAtividadeEscola { get; set; }

        public IApiNewCyberBusiness BusinessApiNewCyber { get; set; }

        public IApiPromocaoIntercambioBusiness BusinessApiPromocaoIntercambio { get; set; }
        public IEmpresaValorServicoDataAccess DataAccessEmpresaValorServico { get; set; }

        public IApiAreaRestritaBusiness BusinessApiAreRestrita { get; set; }
        /// <summary>
        ///  Declaração de constantes
        /// </summary>
        const int ADD = 1; int EDIT = 2;


        /// <summary>
        /// Métodos construtor DAO
        /// </summary>
        /// <param name="Dao"></param>
        /// <param name="businessUsuario"></param>
        /// <param name="DaoUsuario"></param>
        /// <param name="businessPessoa"></param>


        public EscolaBusiness(IEscolaDataAccess dao, IUsuarioDataAccess daoUsuario, IUsuarioBusiness businessUsuario,
                              IPessoaBusiness businessPessoa,
                              ILocalidadeBusiness businessLocalidade,
                              IAuthBusiness businessAuth, IParametrosDataAccess daoParametro, IEmpresaBusiness businessEmpresa,
                              IAlunoBusiness businessAluno, ICoordenacaoBusiness businessCoordenacao, IMatriculaBusiness businessMatricula,
                              IFinanceiroBusiness businessFinanceiro, ITurmaBusiness businessTurma, ISecretariaBusiness businessSecretaria,
                              IBibliotecaBusiness businessBiblioteca, ICursoBusiness businessCurso, IFiscalBusiness businessFiscal,
                              ISysAppDataAccess daoSysApp, IAditamentoDataAccess dataAccessAditamento, IMovimentoDataAccess dataAccessMovimento, IPessoaEscolaDataAccess dataAccessPessoaEscola,
                              IItemMovimentoDataAccess dataAccessItemMovimento, IItemEscolaDataAccess dataAccessItemEscola, ITipoItemDataAccess dataAccessTipoItem,
                              ITipoNotaFiscalDataAccess dataAccessTipoNotaFiscal, IFinanceiroBusiness businessFinan, IPlanoTituloDataAccess dataAccessPlanoTitulo,
                              ITituloDataAccess dataAccessTitulo, ILocalMovtoDataAccess dataAccessLocalMovto, ITurmaEscolaEmpresaDataAccess dataAccessTurmaEscola, 
                              IAtividadeEscolaAtividadeEmpresaDataAccess dataAccessAtividadeEscola, IApiNewCyberBusiness businessApiNewCyber, IApiPromocaoIntercambioBusiness businessApiPromocaoIntercambio,
                              IEmpresaValorServicoDataAccess dataAccessEmpresaValorServico, IApiAreaRestritaBusiness businessApiAreRestrita)
        {
            if (dao == null || daoUsuario == null || businessUsuario == null || businessPessoa == null || businessLocalidade == null || businessEmpresa == null ||
                businessAluno == null || businessCoordenacao == null || businessMatricula == null || businessFinanceiro == null || businessTurma == null
                || businessSecretaria == null || businessBiblioteca == null || businessCurso == null || businessFiscal == null || daoSysApp == null ||
                dataAccessAditamento == null || dataAccessMovimento == null || dataAccessItemMovimento == null || dataAccessPessoaEscola == null  || dataAccessItemEscola == null || 
                dataAccessTipoItem == null || dataAccessTipoNotaFiscal == null || businessFinan == null || dataAccessPlanoTitulo == null ||
                dataAccessTitulo == null || dataAccessLocalMovto == null || dataAccessTurmaEscola == null || dataAccessAtividadeEscola == null
                || businessApiNewCyber == null || businessApiPromocaoIntercambio == null || dataAccessEmpresaValorServico == null || businessApiAreRestrita == null)

                throw new ArgumentNullException();
            Dao = dao;
            DaoUsuario = daoUsuario;
            BusinessPessoa = businessPessoa;
            BusinessUsuario = businessUsuario;
            BusinessEmpresa = businessEmpresa;
            BusinessLoc = businessLocalidade;
            BusinessAuth = businessAuth;
            DaoParametro = daoParametro;
            BusinessAluno = businessAluno;
            BusinessCoordenacao = businessCoordenacao;
            BusinessMatricula = businessMatricula;
            BusinessFinanceiro = businessFinanceiro;
            BusinessTurma = businessTurma;
            BusinessSecretaria = businessSecretaria;
            BusinessBiblioteca = businessBiblioteca;
            BusinessCurso = businessCurso;
            BusinessFiscal = businessFiscal;
            DaoSysApp = daoSysApp;
            this.DataAccessAditamento = dataAccessAditamento;
            this.DataAccessMovimento = dataAccessMovimento;
   			DataAccessPessoaEscola = dataAccessPessoaEscola;
            this.DataAccessItemMovimento = dataAccessItemMovimento;
            this.DataAccessItemEscola = dataAccessItemEscola;
            this.DataAccessTipoItem = dataAccessTipoItem;
            this.DataAccessTipoNotaFiscal = dataAccessTipoNotaFiscal;
            this.BusinessFinan = businessFinan;
            this.DataAccessPlanoTitulo = dataAccessPlanoTitulo;
            this.DataAccessTitulo = dataAccessTitulo;
            this.DataAccessLocalMovtoDataAccess = dataAccessLocalMovto;
            this.DataAccessTurmaEscola = dataAccessTurmaEscola;
            this.DataAccessAtividadeEscola = dataAccessAtividadeEscola;
            this.BusinessApiNewCyber = businessApiNewCyber;
            this.BusinessApiPromocaoIntercambio = businessApiPromocaoIntercambio;
            DataAccessEmpresaValorServico = dataAccessEmpresaValorServico;
            this.BusinessApiAreRestrita = businessApiAreRestrita;
        }

        // Configura os codigos do usuário para auditorias dos DataAccess
        public void configuraUsuario(int cdUsuario, int cd_empresa)
        {
            ((SGFWebContext)this.Dao.DB()).IdUsuario = ((SGFWebContext)this.DaoParametro.DB()).IdUsuario = ((SGFWebContext)this.DaoSysApp.DB()).IdUsuario = ((SGFWebContext)this.DataAccessAditamento.DB()).IdUsuario = cdUsuario;
            ((SGFWebContext)this.Dao.DB()).cd_empresa = ((SGFWebContext)this.DaoParametro.DB()).cd_empresa = ((SGFWebContext)this.DaoSysApp.DB()).cd_empresa = ((SGFWebContext)this.DataAccessAditamento.DB()).cd_empresa =
            ((SGFWebContext)this.DataAccessMovimento.DB()).cd_empresa = ((SGFWebContext)this.DataAccessPessoaEscola.DB()).cd_empresa = ((SGFWebContext)this.DataAccessItemMovimento.DB()).cd_empresa = ((SGFWebContext)this.DataAccessItemEscola.DB()).cd_empresa =
            ((SGFWebContext)this.DataAccessLocalMovtoDataAccess.DB()).cd_empresa = ((SGFWebContext)this.DataAccessTurmaEscola.DB()).cd_empresa = ((SGFWebContext)this.DataAccessAtividadeEscola.DB()).cd_empresa = 
            ((SGFWebContext)this.DataAccessEmpresaValorServico.DB()).cd_empresa = cd_empresa; 
            BusinessUsuario.configuraUsuario(cdUsuario, cd_empresa);
            BusinessPessoa.configuraUsuario(cdUsuario, cd_empresa);
            BusinessLoc.configuraUsuario(cdUsuario, cd_empresa);
            BusinessAuth.configuraUsuario(cdUsuario, cd_empresa);
            BusinessMatricula.configuraUsuario(cdUsuario, cd_empresa);
            BusinessFinanceiro.configuraUsuario(cdUsuario, cd_empresa);
            BusinessSecretaria.configuraUsuario(cdUsuario, cd_empresa);
            BusinessBiblioteca.configuraUsuario(cdUsuario, cd_empresa);
            BusinessCurso.configuraUsuario(cdUsuario, cd_empresa);
            BusinessEmpresa.configuraUsuario(cdUsuario, cd_empresa);
            BusinessFiscal.configuraUsuario(cdUsuario, cd_empresa);
            BusinessTurma.configuraUsuario(cdUsuario, cd_empresa);
            BusinessCoordenacao.configuraUsuario(cdUsuario, cd_empresa);
            BusinessAluno.configuraUsuario(cdUsuario, cd_empresa);
            BusinessApiAreRestrita.configuraUsuario(cdUsuario, cd_empresa);
        }

        public void sincronizarContextos(DbContext dbContext)
        {
            //this.Dao.sincronizaContexto(dbContext);
            //this.DaoUsuario.sincronizaContexto(dbContext);
            //this.DaoParametro.sincronizaContexto(dbContext);
            //this.DaoSysApp.sincronizaContexto(dbContext);
            //BusinessAluno.sincronizaContexto(dbContext);
            //BusinessCoordenacao.sincronizarContextos(dbContext);
            //BusinessTurma.sincronizaContexto(dbContext);
            //BusinessMatricula.sincronizarContextos(dbContext);
            //BusinessFinanceiro.sincronizarContextos(dbContext);
            //BusinessSecretaria.sincronizarContextos(dbContext);
            //BusinessBiblioteca.sincronizarContextos(dbContext);
            //BusinessCurso.sincronizarContexto(dbContext);
            //BusinessFiscal.sincronizarContextos(dbContext);
            //this.DataAccessAditamento.sincronizaContexto(dbContext);
            //this.DataAccessMovimento.sincronizaContexto(dbContext);
        }

        #region escola

        public int getCodigoFranquia(int cd_escola, int id_aplicacao)
        {
            int? cd_franquia = Dao.getCodigoFranquia(cd_escola, id_aplicacao);

            if (cd_franquia.HasValue)
                return cd_franquia.Value;
            else
                throw new EscolaBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroCodigoFranquiaInexistente, null, EscolaBusinessException.TipoErro.ERRO_CODIGO_FRANQUIA_INEXISTENTE, false);
        }

        public void verificaHorarioFuncionamentoEscola(TimeSpan hr_inicial, TimeSpan hr_final, TimeSpan hr_servidor)
        {
            try
            {
                BusinessAuth.verificaHorarioLogin(hr_inicial, hr_final, hr_servidor);
            }
            catch (AuthBusinessException ex)
            {
                if (ex.tipoErro == AuthBusinessException.TipoErro.HORARIO_LOGIN_ULTRAPASSADO)
                    throw new BusinessException(Utils.Messages.Messages.msgHorarioAcessoUsuario, ex, false);
            }
        }

        public IEnumerable<EscolaUI> getDescEscola(SearchParameters parametros, string desc, bool inicio, bool? status, string cnpj, string fantasia, int cdUsuario)
        {
            IEnumerable<EscolaUI> retorno = new List<EscolaUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "no_pessoa";
                parametros.sort = parametros.sort.Replace("dt_cadastro", "dt_cadastramento");
                parametros.sort = parametros.sort.Replace("escola_ativa", "id_pessoa_ativa");

                bool master = BusinessUsuario.VerificarMasterGeral(cdUsuario);
                if (master)
                    cdUsuario = 0;

                retorno = Dao.getEscolaByDesc(parametros, desc, inicio, status, cnpj, fantasia, cdUsuario);
                transaction.Complete();
            }
            return retorno;
        }

        /// <summary>
        /// Adiciona uma escola
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public EscolaUI addEscola(EscolaUI entity, List<RelacionamentoSGF> relacionamentos, int cdEscola, string fullPath)
        {
            Escola escola = new Escola();
            EscolaUI escolaUI = new EscolaUI();
            PessoaEscola pessoaEsc = new PessoaEscola();
            List<RelacionamentoSGF> listRelacionamentosCoordenadorBdCurrent = new List<RelacionamentoSGF>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                //Verifica se já existe  uma escola com o cnpj informado, caso exista o sistema gera uma exeção do contrario  ele verifica se esta passando código da "pessoa juridica" e fará a persistência.
                var existPessoaEscolaCNPJBases = BusinessEmpresa.existsEmpresaWithCNPJ(entity.pessoaJuridica.pessoaJuridica.dc_num_cgc);
                if (existPessoaEscolaCNPJBases != null && existPessoaEscolaCNPJBases.cd_pessoa > 0)
                    throw new PessoaBusinessException(Utils.Messages.Messages.msgExisteCNPJEscola + " " + existPessoaEscolaCNPJBases.no_pessoa + '.', null, FundacaoFisk.SGF.Web.Services.Pessoa.Business.PessoaBusinessException.TipoErro.ERRO_CNPJJAEXISTENTE, false);

                if (entity.pessoaJuridica.pessoaJuridica != null && entity.pessoaJuridica.pessoaJuridica.cd_pessoa > 0)
                {
                    var insert = BusinessEmpresa.insertPessoaWithEmpresa(entity.pessoaJuridica.pessoaJuridica.cd_pessoa, entity.nm_cliente_integracao,
                        entity.nm_escola_integracao, entity.hr_inicial, entity.hr_final, entity.dt_abertura, entity.dt_inicio);
                    escola.copy(entity.pessoaJuridica.pessoaJuridica);
                    escola.id_pessoa_empresa = true;
                    escola.id_escola_ativa = true;
                    escola.id_exportado = false;
                    escola.dc_reduzido_pessoa = entity.pessoaJuridica.pessoaJuridica.dc_reduzido_pessoa;
                    escola.dt_abertura = entity.dt_abertura;
                    escola.dt_inicio = entity.dt_inicio;
                    escola.id_empresa_propria = entity.id_empresa_propria;
                    escola.nm_dia_gerar_nfs = entity.nm_dia_gerar_nfs;
                }
                else
                {
                    escola.copy(entity.pessoaJuridica.pessoaJuridica);
                    escola.hr_final = entity.hr_final;
                    escola.hr_inicial = entity.hr_inicial;
                    escola.id_exportado = false;
                    escola.id_pessoa_empresa = true;
                    escola.id_escola_ativa = true;
                    escola.dc_reduzido_pessoa = entity.pessoaJuridica.pessoaJuridica.dc_reduzido_pessoa;
                    escola.nm_natureza_pessoa = (int)PessoaSGF.TipoPessoa.JURIDICA;
                    escola.dt_abertura = entity.dt_abertura;
                    escola.dt_inicio = entity.dt_inicio;
                    escola.cd_empresa = entity.cd_empresa;
                    escola.nm_empresa_integracao = entity.nm_escola_integracao;
                    escola.nm_cliente_integracao = entity.nm_cliente_integracao;
                    escola.id_empresa_propria = entity.id_empresa_propria;
                    escola.nm_dia_gerar_nfs = entity.nm_dia_gerar_nfs;
                    escola = Dao.add(escola, false);

                    
                }
                //Pega o código da escola inserida para fazer a persistência do endereço
                if (entity.pessoaJuridica.endereco.cd_loc_cidade > 0)
                {
                    entity.pessoaJuridica.endereco.cd_pessoa = escola.cd_pessoa;
                    var endereco = BusinessLoc.PostEndereco(entity.pessoaJuridica.endereco);
                    //Pega o código do endereço principal e edita a escola
                    escola.cd_endereco_principal = endereco.cd_endereco;
                    Dao.edit(escola, false);
                }
                //Outros endereços
                if (entity.pessoaJuridica.enderecos != null)
                {
                    BusinessPessoa.setOutrosEnderecos(entity.pessoaJuridica.enderecos.ToList(), escola.cd_pessoa, escola.cd_endereco_principal);
                }
                //Outros Contatos
                if (entity.pessoaJuridica.telefones != null)
                {
                    BusinessPessoa.setOutrosContatos(entity.pessoaJuridica.telefones.ToList(), escola.cd_pessoa);
                }
                // Relacionamento
                List<RelacionamentoSGF> listaRelacionamento = relacionamentos;
                if (relacionamentos != null)
                {
                    BusinessPessoa.setRelacionamentos(listaRelacionamento, escola.cd_pessoa, false);
                    for (int i = 0; i < listaRelacionamento.Count; i++)
                    {
                        pessoaEsc = new PessoaEscola
                        {
                            cd_escola = cdEscola,
                            cd_pessoa = listaRelacionamento[i].cd_pessoa_filho
                        };
                        BusinessEmpresa.postEmpresaPessoa(pessoaEsc);
                    }
                }

                //Contatos Principais
                BusinessPessoa.addEditTipoContato(entity.pessoaJuridica.telefone, escola.cd_pessoa, ADD, (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.TELEFONE, null, null);
                BusinessPessoa.addEditTipoContato(entity.pessoaJuridica.email, escola.cd_pessoa, ADD, (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL, null, null);
                BusinessPessoa.addEditTipoContato(entity.pessoaJuridica.site, escola.cd_pessoa, ADD, (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.SITE, null, null);
                BusinessPessoa.addEditTipoContato(entity.pessoaJuridica.celular, escola.cd_pessoa, ADD, (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.CELULAR, entity.pessoaJuridica.cd_operadora, null);


                //Se a escola tem nm_cliente_integracao e não existe no cyber
                if (BusinessApiNewCyber.aplicaApiCyber())
                {
                    //pega os relacionamentos atuais de coordenador no banco
                    listRelacionamentosCoordenadorBdCurrent = BusinessApiNewCyber.findRelacionamentosCoordenadorByEmpresa(escola.cd_pessoa);

                    //pega a lista de relacionamentos de coordenador atual, se algum não existir no cyber chama o cadastra_coordenador
                    if(listRelacionamentosCoordenadorBdCurrent != null && listRelacionamentosCoordenadorBdCurrent.Count > 0)
                    {
                        foreach (RelacionamentoSGF item in listRelacionamentosCoordenadorBdCurrent)
                        {
                            PessoaCoordenadorCyberBdUI pessoaCoordenador = BusinessApiNewCyber.findPessoaCoordenadorCyberByCdPessoa(item.cd_pessoa_filho, item.cd_pessoa_pai);

                            if (pessoaCoordenador != null)
                            {
                                verificaTipoFuncionarioPostApiCyber(pessoaCoordenador);
                            }


                        }
                    }

                    if (escola.nm_cliente_integracao != null && escola.nm_cliente_integracao > 0 && !existeUnidade((int)escola.nm_cliente_integracao))
                    {
                        //Valida os parametros e chama a api cyber com o comando(CADASTRA_UNIDADE)
                        cadastraUnidadeApiCyber(entity, escola);
                    }
                }
                

                //Parâmetros
                var parametro = insertParametro(entity, escola.cd_pessoa);

                escolaUI = setEscolaUI(entity, escola);

                var telefone = entity.pessoaJuridica.telefone;
                double? juros = parametro.pc_juros_dia;
                double? multa = parametro.pc_multa;
                double? taxaBiblioteca = parametro.pc_taxa_dia_biblioteca;

                escolaUI = EscolaUI.fromEscolaUI(escolaUI, telefone, parametro, juros, multa, taxaBiblioteca);
                if (!string.IsNullOrEmpty(entity.nome_assinatura_certificado))
                    gravarArquivoCertificadoFuncionario(fullPath, escola);
                transaction.Complete();
            }
            return escolaUI;

        }

        private void cadastraUnidadeApiCyber(EscolaUI entity, Escola escola)
        {
            string parametros = ValidaParametros(entity, escola, ConfigurationManager.AppSettings["enderecoApiNewCyber"], ApiCyberComandosNames.CADASTRA_UNIDADE, "");

            string result = BusinessApiNewCyber.postExecutaCyber(ConfigurationManager.AppSettings["enderecoApiNewCyber"],
                ApiCyberComandosNames.CADASTRA_UNIDADE, ConfigurationManager.AppSettings["chaveApiNewCyber"], parametros);
        }

        private string ValidaParametros(EscolaUI entity, Escola escola, string url, string comando, string parametros)
        {
            EnderecoSGF esc = null;

            //se o cd_endereco da view for 0
            if (entity.pessoaJuridica.endereco.cd_endereco <= 0)
            {
                //pega do banco
                int? cod_endereco_prinicpal = BusinessEmpresa.findByIdEmpresa(entity.pessoaJuridica.pessoaJuridica.cd_pessoa).cd_endereco_principal;
                if (cod_endereco_prinicpal != null && cod_endereco_prinicpal > 0)
                {
                    esc = BusinessLoc.getEnderecoByCdEndereco((int)cod_endereco_prinicpal);
                }
            }
            else
            {
                esc = BusinessLoc.getEnderecoByCdEndereco(entity.pessoaJuridica.endereco.cd_endereco);
            }

            

            if (entity == null || escola == null || esc == null)
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberParametrosNulos, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_PARAMETROS_NULOS, false);
            }

            if (escola.nm_cliente_integracao == null || escola.nm_cliente_integracao <= 0)
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberNmIntegracaoNuloOuMenorIgualZero, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_NM_INTEGRACAO_NULO_OU_MENOR_IGUAL_ZERO, false);
            }

            //Valida PessoaJuridica
            if (entity.pessoaJuridica == null)
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberPessoaJuridicaNulaOuVazia, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_PESSOA_JURIDICA_NULA_OU_VAZIA, false);
            }
            else if(entity.pessoaJuridica.pessoaJuridica == null)
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberPessoaJuridicaNulaOuVazia, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_PESSOA_JURIDICA_NULA_OU_VAZIA, false);
            }
            else if(String.IsNullOrEmpty(entity.pessoaJuridica.pessoaJuridica.dc_reduzido_pessoa))
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberPessoaJuridicaNulaOuVazia, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_PESSOA_JURIDICA_NULA_OU_VAZIA, false);
            }

            //Valida Email
            if (String.IsNullOrEmpty(entity.pessoaJuridica.email))
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberEmailPessoaJuridicaNulaOuVazia, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_EMAIL_PESSOA_JURIDICA_NULA_OU_VAZIA, false);
            }

            //Valida Cidade
            if (esc.Cidade == null)
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberCidadeNulaOuVazia, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_CIDADE_NULA_OU_VAZIA, false);
            }
            else if (String.IsNullOrEmpty(esc.Cidade.no_localidade))
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberCidadeNulaOuVazia, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_CIDADE_NULA_OU_VAZIA, false);
            }

            //Valida Estado
            if (esc.Estado == null)
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberEstadoNuloOuVazio, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_ESTADO_NULO_OU_VAZIO, false);
                
            }
            else if (esc.Estado.Estado == null)
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberEstadoNuloOuVazio, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_ESTADO_NULO_OU_VAZIO, false);
            } 
            else if (String.IsNullOrEmpty(esc.Estado.Estado.sg_estado))
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberEstadoNuloOuVazio, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_ESTADO_NULO_OU_VAZIO, false);
            }


            string listaParams = "";



             listaParams = string.Format("codigo={0},nome_unidade={1},email={2},cidade={3},estado={4}", escola.nm_cliente_integracao, entity.pessoaJuridica.pessoaJuridica.no_pessoa, entity.pessoaJuridica.email, esc.Cidade.no_localidade, esc.Estado.Estado.sg_estado);
            return listaParams;
        }

        public EscolaUI editEscola(EscolaUI entity, List<RelacionamentoSGF> relacionamentos, int cdEscola, bool isMasterGeral, string fullPath)
        {

            EnderecoSGF enderecoPrincipal = new EnderecoSGF();
            Escola escola = new Escola();
            EscolaUI escolaUI = new EscolaUI();
            PessoaEscola pessoaEsc = new PessoaEscola();
            EscolaApiCyberBdUI escolaBdOld = new EscolaApiCyberBdUI();
            List<int> listIdsRelacionamentosCoordenadorBdOld = new List<int>();
            List<int> listIdsRelacionamentosCoordenadorBdCurrent = new List<int>();
            
            List<RelacionamentoSGF> listRelacionamentosCoordenadorBdOld = new List<RelacionamentoSGF>();
            List<RelacionamentoSGF> listRelacionamentosCoordenadorBdCurrent = new List<RelacionamentoSGF>();

            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                if (BusinessApiNewCyber.aplicaApiCyber())
                {
                    //Pega a escola do banco pra verificar se alterou (apiCyber)
                    escolaBdOld = BusinessApiNewCyber.getEscola(entity.pessoaJuridica.pessoaJuridica.cd_pessoa);
                    listRelacionamentosCoordenadorBdOld = BusinessApiNewCyber.findRelacionamentosCoordenadorByEmpresa(entity.pessoaJuridica.pessoaJuridica.cd_pessoa);

                }
                    

                //Verifica se existe usário ativo se verdadeiro gera um erro.
                //bool existeAluno = false;
                //if (entity.id_pessoa_ativa == false)
                //{
                //    existeAluno = BusinessAluno.existeAlunoMatOrRemEscola(entity.pessoaJuridica.pessoaJuridica.cd_pessoa);
                //    if (existeAluno)
                //        throw new EscolaBusinessException(string.Format(Utils.Messages.Messages.msgExistAlunoAtivoEsc), null, EscolaBusinessException.TipoErro.ERRO_ALUNOMATESCOLA, false);
                ////}

                //Verifica se já existe  uma escola com o cnpj informado, caso exista o sistema gera uma exeção do contrario  ele verifica se esta passando código da "pessoa juridica" e fará a persistência.
                var existPessoaEscolaCNPJBases = BusinessEmpresa.existsEmpresaWithCNPJ(entity.pessoaJuridica.pessoaJuridica.dc_num_cgc);
                var escolaBase = BusinessEmpresa.findByIdEmpresa(cdEscola);
                if (entity.pessoaJuridica.pessoaJuridica != null && entity.pessoaJuridica.pessoaJuridica.cd_pessoa > 0 && existPessoaEscolaCNPJBases == null && escolaBase == null)
                {
                    int insert = BusinessEmpresa.insertPessoaWithEmpresa(entity.pessoaJuridica.pessoaJuridica.cd_pessoa, entity.nm_cliente_integracao, entity.nm_escola_integracao, entity.hr_inicial, entity.hr_final, entity.dt_abertura, entity.dt_inicio);
                    escola.cd_pessoa = entity.pessoaJuridica.pessoaJuridica.cd_pessoa;
                    escola.id_pessoa_empresa = true;
                    escola.id_exportado = false;
                    escola.id_escola_ativa = true;
                    escola.dc_reduzido_pessoa = entity.pessoaJuridica.pessoaJuridica.dc_reduzido_pessoa;
                    escola.dt_abertura = entity.dt_abertura;
                    escola.dt_inicio = entity.dt_inicio;
                    if (!isMasterGeral)
                        throw new EscolaBusinessException(string.Format(Utils.Messages.Messages.msgUsuarioSemPermissao), null, EscolaBusinessException.TipoErro.ERRO_USUARIO_SEM_PERMISSAO, false);
                }

                //endereço 
                if (isMasterGeral)
                {
                    int? cod_endereco_prinicpal = BusinessEmpresa.findByIdEmpresa(entity.pessoaJuridica.pessoaJuridica.cd_pessoa).cd_endereco_principal;
                    BusinessPessoa.persistirEndereco(entity.pessoaJuridica.endereco, entity.pessoaJuridica.pessoaJuridica.cd_pessoa, cod_endereco_prinicpal);
                    if (existPessoaEscolaCNPJBases != null && entity.pessoaJuridica.endereco != null && entity.pessoaJuridica.endereco.cd_endereco > 0)
                    {
                        existPessoaEscolaCNPJBases.cd_endereco_principal = entity.pessoaJuridica.endereco.cd_endereco;
                        cod_endereco_prinicpal = entity.pessoaJuridica.endereco.cd_endereco;
                    }

                    // telefone
                    TelefoneSGF TelefoneExists = new TelefoneSGF();
                    TelefoneExists = BusinessPessoa.FindTypeTelefonePrincipal(entity.pessoaJuridica.pessoaJuridica.cd_pessoa, (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.TELEFONE);
                    BusinessPessoa.addEditTipoContato(entity.pessoaJuridica.telefone, entity.pessoaJuridica.pessoaJuridica.cd_pessoa, EDIT, (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.TELEFONE, entity.pessoaJuridica.cd_operadora, TelefoneExists);

                    //site
                    TelefoneSGF SiteExists = new TelefoneSGF();
                    SiteExists = BusinessPessoa.FindTypeTelefonePrincipal(entity.pessoaJuridica.pessoaJuridica.cd_pessoa, (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.SITE);
                    BusinessPessoa.addEditTipoContato(entity.pessoaJuridica.site, entity.pessoaJuridica.pessoaJuridica.cd_pessoa, EDIT, (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.SITE, null, SiteExists);
                    //email
                    TelefoneSGF EmailExists = new TelefoneSGF();
                    EmailExists = BusinessPessoa.FindTypeTelefonePrincipal(entity.pessoaJuridica.pessoaJuridica.cd_pessoa, (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL);
                    BusinessPessoa.addEditTipoContato(entity.pessoaJuridica.email, entity.pessoaJuridica.pessoaJuridica.cd_pessoa, EDIT, (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL, null, EmailExists);
                    //celular
                    TelefoneSGF CelularExists = new TelefoneSGF();

                    CelularExists = BusinessPessoa.FindTypeTelefonePrincipal(entity.pessoaJuridica.pessoaJuridica.cd_pessoa, (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.CELULAR);
                    BusinessPessoa.addEditTipoContato(entity.pessoaJuridica.celular, entity.pessoaJuridica.pessoaJuridica.cd_pessoa, EDIT, (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.CELULAR, entity.pessoaJuridica.cd_operadora, CelularExists);

                    //Outros Contatos
                    BusinessPessoa.setOutrosContatos(entity.pessoaJuridica.telefones.ToList(), entity.pessoaJuridica.pessoaJuridica.cd_pessoa);
                    //Outros endereços
                    BusinessPessoa.setOutrosEnderecos(entity.pessoaJuridica.enderecos.ToList(), entity.pessoaJuridica.pessoaJuridica.cd_pessoa, cod_endereco_prinicpal);

                    if (entity.id_empresa_propria == true && entity.nm_cliente_integracao == 21 && entity.id_empresa_valor_servico != null)
                    {
                        crudEmpresaValorServico(entity.empresaValorServico, (int)entity.id_empresa_valor_servico);
                    }

                }//if masterGeral

                
                

                //Parâmetros             
                Parametro ParametroExists = new Parametro();
                ParametroExists = DaoParametro.getParametrosByEscola(entity.pessoaJuridica.pessoaJuridica.cd_pessoa);
                IEnumerable<PlanoConta> planos = BusinessCoordenacao.getPlanoContaByIdEscola(entity.pessoaJuridica.pessoaJuridica.cd_pessoa);
                if (planos != null && planos.Count() > 0 && ParametroExists != null && ParametroExists.nm_niveis_plano_contas != null && ParametroExists.nm_niveis_plano_contas != entity.parametro.nm_niveis_plano_contas)
                    throw new EscolaBusinessException(string.Format(Utils.Messages.Messages.msgNotUpdateParametro, entity.pessoaJuridica.pessoaJuridica.dc_reduzido_pessoa), null, EscolaBusinessException.TipoErro.ERRO_NIVEL_PARAMETRO, false);
                else ParametroExists = editParametro(entity, ParametroExists, isMasterGeral);

                List<RelacionamentoSGF> listaRelacionamento = relacionamentos;
                if (relacionamentos != null)
                {
                    BusinessPessoa.setRelacionamentos(listaRelacionamento, entity.pessoaJuridica.pessoaJuridica.cd_pessoa, false);
                    for (int i = 0; i < listaRelacionamento.Count; i++)
                    {
                        pessoaEsc = new PessoaEscola
                        {
                            cd_escola = cdEscola,
                            cd_pessoa = listaRelacionamento[i].cd_pessoa_filho
                        };
                        BusinessEmpresa.postEmpresaPessoa(pessoaEsc);
                    }
                }
                escola = changeValueEscola(entity, escola, isMasterGeral);
                escola.nome_temp_assinatura_certificado = entity.nome_temp_assinatura_certificado;
                Dao.saveChanges(false);

                //Retorna dados para view 
                var telefone = entity.pessoaJuridica.telefone;
                escolaUI = setEscolaUI(entity, escola);
                double? juros = 0;
                double? multa = 0;
                double? taxaBiblioteca = 0;
                ParametroExists = DaoParametro.getParametrosByEscola(escola.cd_pessoa);
                if (ParametroExists != null)
                {
                    juros = ParametroExists.pc_juros_dia;
                    multa = ParametroExists.pc_multa;
                    taxaBiblioteca = ParametroExists.pc_taxa_dia_biblioteca;

                    if (ParametroExists.cd_plano_conta_tax != null)
                        escolaUI.dc_plano_taxa = BusinessFinanceiro.getDescPlanoContaByEscola(escola.cd_pessoa, (int)ParametroExists.cd_plano_conta_tax);

                    if (ParametroExists.cd_plano_conta_mat != null)
                        escolaUI.dc_plano_mat = BusinessFinanceiro.getDescPlanoContaByEscola(escola.cd_pessoa, (int)ParametroExists.cd_plano_conta_mat);

                    if (ParametroExists.cd_plano_conta_juros != null)
                        escolaUI.dc_plano_juros = BusinessFinanceiro.getDescPlanoContaByEscola(escola.cd_pessoa, (int)ParametroExists.cd_plano_conta_juros);

                    if (ParametroExists.cd_plano_conta_multa != null)
                        escolaUI.dc_plano_multa = BusinessFinanceiro.getDescPlanoContaByEscola(escola.cd_pessoa, (int)ParametroExists.cd_plano_conta_multa);

                    if (ParametroExists.cd_plano_conta_desc != null)
                        escolaUI.dc_plano_desconto = BusinessFinanceiro.getDescPlanoContaByEscola(escola.cd_pessoa, (int)ParametroExists.cd_plano_conta_desc);

                    if (ParametroExists.cd_plano_conta_taxbco != null)
                        escolaUI.dc_plano_taxa_bco = BusinessFinanceiro.getDescPlanoContaByEscola(escola.cd_pessoa, (int)ParametroExists.cd_plano_conta_taxbco);

                    if (ParametroExists.cd_plano_conta_material != null)
                        escolaUI.dc_plano_material = BusinessFinanceiro.getDescPlanoContaByEscola(escola.cd_pessoa, (int)ParametroExists.cd_plano_conta_material);
                }
                escolaUI = EscolaUI.fromEscolaUI(escolaUI, telefone, ParametroExists, juros, multa, taxaBiblioteca);
                if (!string.IsNullOrEmpty(entity.nome_assinatura_certificado))
                    gravarArquivoCertificadoFuncionario(fullPath, escola);

                if (BusinessApiNewCyber.aplicaApiCyber())
                {
                    //pega os relacionamentos atuais de coordenador no banco
                    listRelacionamentosCoordenadorBdCurrent = BusinessApiNewCyber.findRelacionamentosCoordenadorByEmpresa(entity.pessoaJuridica.pessoaJuridica.cd_pessoa);

                    //preenche a lista de ids de relacionamentos antigos
                    if(listRelacionamentosCoordenadorBdOld != null && listRelacionamentosCoordenadorBdOld.Count > 0)
                    {
                        listIdsRelacionamentosCoordenadorBdOld = listRelacionamentosCoordenadorBdOld.Select(x => x.cd_relacionamento).ToList();
                    }

                    //preeche a lista de ids de relacionamentos atuais
                    if (listRelacionamentosCoordenadorBdCurrent != null &&
                        listRelacionamentosCoordenadorBdCurrent.Count > 0)
                    {
                        listIdsRelacionamentosCoordenadorBdCurrent = listRelacionamentosCoordenadorBdCurrent.Select(x => x.cd_relacionamento).ToList();
                    }

                    //acha quem foi deletado e inativa no cyber
                    /*if (listRelacionamentosCoordenadorBdOld != null && listRelacionamentosCoordenadorBdOld.Count > 0)
                    {
                        IEnumerable<int> idsCoordenadoresDeletados = listIdsRelacionamentosCoordenadorBdOld.Except(listIdsRelacionamentosCoordenadorBdCurrent);
                        //chama o inativa coordenador
                        foreach (int id in idsCoordenadoresDeletados)
                        {
                            RelacionamentoSGF relDeleteCyber = listRelacionamentosCoordenadorBdOld.Where(x => x.cd_relacionamento == id).FirstOrDefault();
                            if(relDeleteCyber != null)
                            {
                                executaCyberInativaFuncionario(relDeleteCyber.cd_pessoa_filho);
                            }
                        }
                        
                    }*/

                    
                    

                    EscolaApiCyberBdUI escolaBdCurrent = BusinessApiNewCyber.getEscola(entity.pessoaJuridica.pessoaJuridica.cd_pessoa);

                    //Se a escola tem nm_cliente integracao e existe no cyber
                    if (escolaBdCurrent.nm_cliente_integracao != null && escolaBdCurrent.nm_cliente_integracao > 0 && existeUnidade((int)escolaBdCurrent.nm_cliente_integracao))
                    {
                        //Chama a api cyber com o comando (ATUALIZA_UNIDADE)
                        verificaAlterouCamposExecutaCyberAtualizaUnidade(escolaBdOld, escolaBdCurrent);

                        //se desativou e a escola existe no cyber
                        if ((escolaBdOld.escola_ativa != escolaBdCurrent.escola_ativa && escolaBdOld.escola_ativa == true))
                        {
                            //Chama a api cyber com o comando (INATIVA_UNIDADE)
                            executaCyberInativaUnidade((int)escolaBdCurrent.nm_cliente_integracao);
                        }
                        //se ativou e a escola existe no cyber
                        else if ((escolaBdOld.escola_ativa != escolaBdCurrent.escola_ativa && escolaBdOld.escola_ativa == false))
                        {
                            //Chama a api cyber com o comando (ATIVA_UNIDADE)
                            executaCyberAtivaUnidade((int)escolaBdCurrent.nm_cliente_integracao);
                        }
                    }else if (escolaBdCurrent.nm_cliente_integracao != null && escolaBdCurrent.nm_cliente_integracao > 0 && !existeUnidade((int)escolaBdCurrent.nm_cliente_integracao))
                    {
                        //Valida os parametros e chama a api cyber com o comando(CADASTRA_UNIDADE)
                        cadastraUnidadeApiCyber(entity, escola);
                    }


                    //pega a lista de relacionamentos de coordenador atual, se algum não existir no cyber chama o cadastra_coordenador
                    if (listRelacionamentosCoordenadorBdCurrent != null && listRelacionamentosCoordenadorBdCurrent.Count > 0)
                    {
                        foreach (RelacionamentoSGF item in listRelacionamentosCoordenadorBdCurrent)
                        {
                            PessoaCoordenadorCyberBdUI pessoaCoordenador = BusinessApiNewCyber.findPessoaCoordenadorCyberByCdPessoa(item.cd_pessoa_filho, item.cd_pessoa_pai);

                            if (pessoaCoordenador != null)
                            {
                                verificaTipoFuncionarioPostApiCyber(pessoaCoordenador);
                            }


                        }
                    }
                }

                transaction.Complete();
            }
            return escolaUI;
        }

        public void crudEmpresaValorServico(List<EmpresaValorServico> empresaValorServicosView, int cd_escola)
        {
            List<EmpresaValorServico> empresaValorServicosContext = DataAccessEmpresaValorServico.getEmpresaValorServicoByEscola(cd_escola).ToList();
            if (empresaValorServicosView != null)
            {

                IEnumerable<EmpresaValorServico> empresaValorServicosComCodigo = from evs in empresaValorServicosView
                                                                             where evs.cd_empresa_valor_servico > 0 && evs.cd_pessoa_empresa == cd_escola
                                                                             select evs;
                IEnumerable<EmpresaValorServico> empresaValorServicoDeleted = empresaValorServicosContext.Where(tc => !empresaValorServicosComCodigo.Any(tv => tc.cd_empresa_valor_servico == tv.cd_empresa_valor_servico));

                if (empresaValorServicoDeleted != null) 
                    foreach (var item in empresaValorServicoDeleted)
                    {
                        if (item != null)
                            DataAccessEmpresaValorServico.delete(item, false);
                    }

                foreach (var item in empresaValorServicosView)
                {
                    EmpresaValorServico retorno = null;

                    // Novos horários da turma:
                    if (item.cd_empresa_valor_servico == 0)
                    {
                        item.cd_pessoa_empresa = cd_escola;
                        retorno = DataAccessEmpresaValorServico.add(item, false);

                    }
                    //Alteração dos horários da turma:
                    else
                    {
                        var empresaValorServicoEdit = empresaValorServicosContext.Where(hc => hc.cd_empresa_valor_servico == item.cd_empresa_valor_servico).FirstOrDefault();
                        if (empresaValorServicoEdit != null && empresaValorServicoEdit.cd_empresa_valor_servico > 0)
                        {
                            

                            retorno = saveEmpresaValorServicoContext(empresaValorServicoEdit, item);
                            
                        }
                    }
                }
            }
            else
            {
                if (empresaValorServicosContext != null)
                {
                    foreach (var item in empresaValorServicosContext)
                    {
                        var deletarEmpresaValorServico = DataAccessEmpresaValorServico.getEmpresaValorServicoByIdAndEscola(item.cd_empresa_valor_servico, item.cd_pessoa_empresa); ;
                        if (deletarEmpresaValorServico != null)
                        {
                            DataAccessEmpresaValorServico.delete(deletarEmpresaValorServico, false);
                        }
                            
                    }
                }
            }
        }

        public static EmpresaValorServico changeValueEmpresaValorServicoContext(EmpresaValorServico empresaValorServicoContext, EmpresaValorServico empresaValorServicoView)
        {
            empresaValorServicoContext.cd_pessoa_empresa = empresaValorServicoView.cd_pessoa_empresa;
            empresaValorServicoContext.dt_inicio_valor = empresaValorServicoView.dt_inicio_valor;
            empresaValorServicoContext.vl_unitario_servico = empresaValorServicoView.vl_unitario_servico;

            return empresaValorServicoContext;
        }


        public EmpresaValorServico saveEmpresaValorServicoContext(EmpresaValorServico empresaValorServicoContext, EmpresaValorServico empresaValorServicoView)
        {
            empresaValorServicoContext = changeValueEmpresaValorServicoContext(empresaValorServicoContext, empresaValorServicoView);
            DataAccessEmpresaValorServico.saveChanges(false);
            return empresaValorServicoContext;
        }

        public void verificaTipoFuncionarioPostApiCyber(PessoaCoordenadorCyberBdUI coordenadorCyberBd)
        {
            string parametros = "";

            
            if (coordenadorCyberBd.tipo_funcionario == (byte)(byte) RelacionamentoDataAccess.TipoRelacionamento.COORDENADOR)
            {
                
                if (!existeFuncionario(("O" + coordenadorCyberBd.codigo), ApiCyberComandosNames.VISUALIZA_COORDENADOR))
                {
                    parametros = validaParametrosCadastraCoordenador(coordenadorCyberBd, ConfigurationManager.AppSettings["enderecoApiNewCyber"], ApiCyberComandosNames.CADASTRA_COORDENADOR, "");

                    executaCyberCadastraFuncionario(parametros, ApiCyberComandosNames.CADASTRA_COORDENADOR);
                }
                else
                {
                    //Se já existe chama o ativa coordenador, pois a api do cyber não retorna se o coordenador está ativo ou não, assim garante que todos vão estar ativos, mesmo que tenho sido desativados
                    executaCiberAtivacaoInativacaoFuncionario(coordenadorCyberBd.codigo, ApiCyberComandosNames.ATIVA_COORDENADOR);
                }
            }

        }	



		private bool existeFuncionario(string codigo, string comando)
        {
            return BusinessApiNewCyber.verificaRegistroFuncionario(ConfigurationManager.AppSettings["enderecoApiNewCyber"],
                comando, ConfigurationManager.AppSettings["chaveApiNewCyber"], codigo);
        }

        private void executaCyberCadastraFuncionario(string parametros, string comando)
        {
            string result = BusinessApiNewCyber.postExecutaCyber(ConfigurationManager.AppSettings["enderecoApiNewCyber"],
                comando, ConfigurationManager.AppSettings["chaveApiNewCyber"], parametros);
        }

        

        private string validaParametrosCadastraCoordenador(PessoaCoordenadorCyberBdUI entity, string url, string comando, string parametros)
        {


            //valida codigo funcionario
            if (entity == null)
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberParametrosNulos, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_PARAMETROS_NULOS, false);
            }

            if (entity.codigo <= 0)
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberCdFuncionarioMenorIgualZero, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_COD_FUNCIONARIO_MENOR_IGUAL_ZERO, false);
            }


            //Valida id_unidade
            if (entity.id_unidade <= 0)
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberNmIntegracaoNuloOuMenorIgualZero, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_NM_INTEGRACAO_NULO_OU_MENOR_IGUAL_ZERO, false);
            }

            //Valida nome e email

            if (String.IsNullOrEmpty(entity.nome))
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberNomeFuncionarioNuloVazio, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_NOME_FUNCIONARIO_NULO_VAZIO, false);
            }

            if (String.IsNullOrEmpty(entity.email))
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberEmailFuncionarioNuloVazio, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_EMAIL_PESSOA_FISICA_NULA_OU_VAZIA, false);
            }


            string listaParams = "";
            listaParams = string.Format("nome={0},id_unidade={1},codigo={2},email={3}", entity.nome, entity.id_unidade, entity.codigo, entity.email);
            return listaParams;
        }

        public void executaCyberInativaFuncionario(int codigo)
        {
            
            if (existeFuncionario(("O" + codigo), ApiCyberComandosNames.VISUALIZA_COORDENADOR))
            {
                executaCiberAtivacaoInativacaoFuncionario(codigo, ApiCyberComandosNames.INATIVA_COORDENADOR);
            }
        }

        private void executaCiberAtivacaoInativacaoFuncionario(int codigo, string comando)
        {
            string result = BusinessApiNewCyber.postExecutaCyber(ConfigurationManager.AppSettings["enderecoApiNewCyber"],
                comando, ConfigurationManager.AppSettings["chaveApiNewCyber"], String.Format("codigo={0}", codigo));
        }


        public void verificaAlterouCamposExecutaCyberAtualizaUnidade(EscolaApiCyberBdUI escolabdOld, EscolaApiCyberBdUI escolaBdCurrent)
        {
            
            //Valida os parametros do banco
            //string parametrosBd = ValidaParametrosBDEdicao(escolaBd, ConfigurationManager.AppSettings["enderecoApiNewCyber"], ApiCyberComandosNames.ATUALIZA_UNIDADE, "");

            //Valida os parametros
            string parametrosView = ValidaParametrosEdicao(escolabdOld, escolaBdCurrent, ConfigurationManager.AppSettings["enderecoApiNewCyber"], ApiCyberComandosNames.ATUALIZA_UNIDADE, "", escolaBdCurrent.nm_cliente_integracao);

            //Verifica se modificou chama o executa cyber
            if (escolabdOld.nome_unidade != escolaBdCurrent.nome_unidade ||
                escolabdOld.email != escolaBdCurrent.email ||
                escolabdOld.cd_cidade != escolaBdCurrent.cd_cidade ||
                escolabdOld.cd_estado != escolaBdCurrent.cd_estado)
            {
                string result = BusinessApiNewCyber.postExecutaCyber(ConfigurationManager.AppSettings["enderecoApiNewCyber"],
                    ApiCyberComandosNames.ATUALIZA_UNIDADE, ConfigurationManager.AppSettings["chaveApiNewCyber"], parametrosView);
            }

          
        }

        public void executaCyberInativaUnidade(int codigo)
        {
            string result = BusinessApiNewCyber.postExecutaCyber(ConfigurationManager.AppSettings["enderecoApiNewCyber"],
                ApiCyberComandosNames.INATIVA_UNIDADE, ConfigurationManager.AppSettings["chaveApiNewCyber"], String.Format("codigo={0}", codigo));
        }

        public void executaCyberAtivaUnidade(int codigo)
        {
            string result = BusinessApiNewCyber.postExecutaCyber(ConfigurationManager.AppSettings["enderecoApiNewCyber"],
                ApiCyberComandosNames.ATIVA_UNIDADE, ConfigurationManager.AppSettings["chaveApiNewCyber"], String.Format("codigo={0}", codigo));
        }

        public bool existeUnidade(int codigo)
        {
            return BusinessApiNewCyber.verificaRegistro(ConfigurationManager.AppSettings["enderecoApiNewCyber"],
                ApiCyberComandosNames.VISUALIZA_UNIDADE, ConfigurationManager.AppSettings["chaveApiNewCyber"], codigo);
        }

        private string ValidaParametrosEdicao(EscolaApiCyberBdUI escolaBdOld, EscolaApiCyberBdUI escolaBdCurrent, string url, string comando, string parametros, int? codigoAntigo)
        {
            if (escolaBdCurrent == null )
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberParametrosNulos, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_PARAMETROS_NULOS, false);
            }

            //valida nm_integracao
            if (escolaBdCurrent.nm_cliente_integracao == null || escolaBdCurrent.nm_cliente_integracao <= 0)
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberNmIntegracaoNuloOuMenorIgualZero, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_NM_INTEGRACAO_NULO_OU_MENOR_IGUAL_ZERO, false);
            }

            //valida nome unidade
            if (String.IsNullOrEmpty(escolaBdCurrent.nome_unidade))
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberPessoaJuridicaNulaOuVazia, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_PESSOA_JURIDICA_NULA_OU_VAZIA, false);
            }

            //Valida Email
            if (String.IsNullOrEmpty(escolaBdCurrent.email))
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberEmailPessoaJuridicaNulaOuVazia, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_EMAIL_PESSOA_JURIDICA_NULA_OU_VAZIA, false);
            }

            
            if (String.IsNullOrEmpty(escolaBdCurrent.cidade))
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberCidadeNulaOuVazia, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_CIDADE_NULA_OU_VAZIA, false);
            }

            //Valida Estado
           
            if (String.IsNullOrEmpty(escolaBdCurrent.estado))
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberEstadoNuloOuVazio, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_ESTADO_NULO_OU_VAZIO, false);
            }


            string listaParams = "";

            
            if(escolaBdOld.nm_cliente_integracao != null && escolaBdOld.nm_cliente_integracao > 0 && escolaBdOld.nm_cliente_integracao != escolaBdCurrent.nm_cliente_integracao)
            {
                escolaBdCurrent.codigo_antigo = (int)escolaBdOld.nm_cliente_integracao;
            }
            else
            {
                escolaBdCurrent.codigo_antigo = (int)escolaBdCurrent.nm_cliente_integracao;
            }

            listaParams = string.Format("codigo={0},codigo_antigo={1},nome_unidade={2},email={3},cidade={4},estado={5}", escolaBdCurrent.nm_cliente_integracao, escolaBdCurrent.codigo_antigo, escolaBdCurrent.nome_unidade, escolaBdCurrent.email, escolaBdCurrent.cidade, escolaBdCurrent.estado);
            return listaParams;
        }

        


        private static EscolaUI setEscolaUI(EscolaUI entity, Escola escola)
        {
            EscolaUI escolaUI = new EscolaUI
            {
                cd_pessoa = escola.cd_pessoa,
                no_pessoa = escola.no_pessoa,
                dc_num_cgc = entity.pessoaJuridica.pessoaJuridica.dc_num_cgc,
                id_pessoa_ativa = entity.id_pessoa_ativa,
                ext_img_pessoa = entity.pessoaJuridica.pessoaJuridica.ext_img_pessoa,
                hr_final = escola.hr_final,
                hr_inicial = escola.hr_inicial,
                dt_abertura = escola.dt_abertura,
                dt_inicio = escola.dt_inicio,
                dt_cadastramento = entity.pessoaJuridica.pessoaJuridica.dt_cadastramento,
                dc_reduzido_pessoa = entity.pessoaJuridica.pessoaJuridica.dc_reduzido_pessoa,
                cd_empresa = entity.cd_empresa,
                nm_cliente_integracao = entity.nm_cliente_integracao,
                nm_escola_integracao = entity.nm_escola_integracao,
                nm_patrimonio = entity.nm_patrimonio,
                nm_investimento = entity.nm_investimento
            };
            return escolaUI;
        }

        private Escola changeValueEscola(EscolaUI entity, Escola escola, bool isMasterGeral)
        {
            escola = Dao.findById(entity.pessoaJuridica.pessoaJuridica.cd_pessoa, false);
            //escola.copy(entity.pessoaJuridica.pessoaJuridica);

            if (!isMasterGeral)
                if ((escola.nm_dia_gerar_nf_servico != entity.nm_dia_gerar_nf_servico)
                    || (escola.id_empresa_internacional != entity.id_empresa_internacional)
                    )
                    throw new EscolaBusinessException(Utils.Messages.Messages.msgUsuarioSemPermissao + " ", null, EscolaBusinessException.TipoErro.ERRO_USUARIO_SEM_PERMISSAO, false);

            if ((escola.hr_final > entity.hr_final) || (escola.hr_inicial < entity.hr_inicial))  //Verificar apenas se tiver diminuindo o horário
            {
                bool existHorary = Dao.verificaHorarioOcupado(entity.pessoaJuridica.pessoaJuridica.cd_pessoa, (TimeSpan)entity.hr_inicial, (TimeSpan)entity.hr_final);
                if (existHorary)
                {
                    string descricaoHorario = BusinessSecretaria.retornaDescricaoHorarioOcupado(entity.pessoaJuridica.pessoaJuridica.cd_pessoa, (TimeSpan)entity.hr_inicial, (TimeSpan)entity.hr_final);
                    throw new EscolaBusinessException(string.Format(Utils.Messages.Messages.msgHorarioOcupadoEscola, descricaoHorario), null, EscolaBusinessException.TipoErro.ERRO_HORARIO_OCUPADO, false);
                }
            }
            escola.hr_final = entity.hr_final;
            escola.hr_inicial = entity.hr_inicial;
            escola.dt_inicio = entity.dt_inicio;
            escola.dt_abertura = entity.dt_abertura;
            escola.id_escola_ativa = entity.id_pessoa_ativa;
            escola.dc_reduzido_pessoa = entity.pessoaJuridica.pessoaJuridica.dc_reduzido_pessoa;
            escola.cd_empresa = entity.cd_empresa;
            escola.nm_cliente_integracao = entity.nm_cliente_integracao;
            escola.nm_empresa_integracao = entity.nm_escola_integracao;
            escola.nm_investimento = entity.nm_investimento;
            escola.nm_patrimonio = entity.nm_patrimonio;

            escola.cd_atividade_principal = entity.pessoaJuridica.pessoaJuridica.cd_atividade_principal;
            escola.cd_papel_principal = entity.pessoaJuridica.pessoaJuridica.cd_papel_principal;
            escola.cd_pessoa = entity.pessoaJuridica.pessoaJuridica.cd_pessoa;
            escola.cd_tipo_sociedade = entity.pessoaJuridica.pessoaJuridica.cd_tipo_sociedade;
            escola.dc_num_cgc = entity.pessoaJuridica.pessoaJuridica.dc_num_cgc;
            escola.dc_num_cnpj_cnab = entity.pessoaJuridica.pessoaJuridica.dc_num_cnpj_cnab;
            escola.dc_num_insc_estadual = entity.pessoaJuridica.pessoaJuridica.dc_num_insc_estadual;
            escola.dc_num_insc_municipal = entity.pessoaJuridica.pessoaJuridica.dc_num_insc_municipal;
            escola.dc_reduzido_pessoa = entity.pessoaJuridica.pessoaJuridica.dc_reduzido_pessoa;
            escola.dc_registro_junta_comercial = entity.pessoaJuridica.pessoaJuridica.dc_registro_junta_comercial;
            escola.dt_baixa = entity.pessoaJuridica.pessoaJuridica.dt_baixa;
            escola.dt_registro_junta_comercial = entity.pessoaJuridica.pessoaJuridica.dt_registro_junta_comercial;
            escola.no_pessoa = entity.pessoaJuridica.pessoaJuridica.no_pessoa;
            escola.txt_obs_pessoa = entity.pessoaJuridica.pessoaJuridica.txt_obs_pessoa;
            escola.id_empresa_propria = entity.id_empresa_propria;
            escola.nm_dia_gerar_nf_servico = entity.nm_dia_gerar_nf_servico;
            escola.id_empresa_internacional = entity.id_empresa_internacional;
            escola.nm_dia_gerar_nfs = entity.nm_dia_gerar_nfs;
            if (escola.nome_assinatura_certificado != entity.nome_assinatura_certificado)
                escola.nome_assinatura_certificado_anterior = escola.nome_assinatura_certificado;
            escola.nome_assinatura_certificado = entity.nome_assinatura_certificado;
            if (!string.IsNullOrEmpty(entity.pessoaJuridica.pessoaJuridica.ext_img_pessoa))
            {
                if (!string.IsNullOrEmpty(escola.ext_img_pessoa))
                {
                    if (escola.ext_img_pessoa != entity.pessoaJuridica.pessoaJuridica.ext_img_pessoa && entity.pessoaJuridica.pessoaJuridica.img_pessoa != null && entity.pessoaJuridica.pessoaJuridica.img_pessoa.Count() > 0)
                    {
                        escola.ext_img_pessoa = entity.pessoaJuridica.pessoaJuridica.ext_img_pessoa;
                        escola.img_pessoa = entity.pessoaJuridica.pessoaJuridica.img_pessoa;
                    }
                }
                else
                {
                    escola.ext_img_pessoa = entity.pessoaJuridica.pessoaJuridica.ext_img_pessoa;
                    escola.img_pessoa = entity.pessoaJuridica.pessoaJuridica.img_pessoa;
                }
            }
            else
            {
                if (escola.ext_img_pessoa != null || (escola.img_pessoa != null && escola.img_pessoa.Count() > 0))
                {
                    escola.img_pessoa = null;
                    escola.ext_img_pessoa = "";
                }
            }
            return escola;
        }

        public void gravarArquivoCertificadoFuncionario(string fullPath, Escola escola)
        {
            HashSet<char> invalidFileNameChars = new HashSet<char>(Path.GetInvalidFileNameChars());
            HashSet<char> invalidPathChars = new HashSet<char>(Path.GetInvalidPathChars());
            if (escola.nome_temp_assinatura_certificado != null)
            {
                string imagemCertificadoTemp = fullPath + "//" + escola.nome_temp_assinatura_certificado;
                fullPath += "//Arquivos";
                string imagemCertificado = fullPath + "//" + escola.cd_pessoa + "//Assinatura//" + escola.nome_assinatura_certificado;
                string pathAssinatura = fullPath + "//" + escola.cd_pessoa + "//Assinatura//";
                //Caso não exista a pasta com o código da escola, criar.
                if (!System.IO.Directory.Exists(fullPath + "//" + escola.cd_pessoa))
                    System.IO.Directory.CreateDirectory(fullPath + "//" + escola.cd_pessoa);
                //Caso não exista a pasta com o código do professor, criar.
                if (!System.IO.Directory.Exists(pathAssinatura))
                    System.IO.Directory.CreateDirectory(pathAssinatura);

                if (escola.nome_assinatura_certificado.Any(c => invalidFileNameChars.Contains(c)))
                    throw new FuncionarioBusinessException(Utils.Messages.Messages.msgErroCaractInvalidosImagem + escola.nome_assinatura_certificado,
                        null, FuncionarioBusinessException.TipoErro.ERRO_CERTIFICADO_JA_EXISTE, false);
                if (fullPath.Any(c => invalidPathChars.Contains(c)))
                    throw new FuncionarioBusinessException(Utils.Messages.Messages.msgErroCaractInvalidosPathArquivoImagem + escola.nome_assinatura_certificado,
                        null, FuncionarioBusinessException.TipoErro.ERRO_CERTIFICADO_JA_EXISTE, false);
                //string pathContratosEscola = caminho_relatorios + "/Contratos/" + cdEscola;
                if (System.IO.File.Exists(imagemCertificado))
                    System.IO.File.Delete(imagemCertificado);
                //throw new FuncionarioBusinessException(Utils.Messages.Messages.msgErroImagemJaCadastrado, null, FuncionarioBusinessException.TipoErro.ERRO_CERTIFICADO_JA_EXISTE, false);
                //System.IO.File.Delete(documentoContrato);
                System.IO.File.Move(imagemCertificadoTemp, imagemCertificado);
                if (System.IO.File.Exists(imagemCertificadoTemp))
                    System.IO.File.Delete(imagemCertificadoTemp);
                if (!string.IsNullOrEmpty(escola.nome_assinatura_certificado_anterior))
                    if (System.IO.File.Exists(fullPath + "//" + escola.cd_pessoa + "//Assinatura//" + escola.nome_assinatura_certificado_anterior))
                        System.IO.File.Delete(fullPath + "//" + escola.cd_pessoa + "//Assinatura//" + escola.nome_assinatura_certificado_anterior);
            }
        }

        /// <summary>
        /// Regra para deletar a escola
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool deleteEscola(Escola entity)
        {
            bool deletado;
            Escola escolaDelete = new Escola();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                EscolaApiCyberBdUI escolaApiCyberBdUi = null;
                if (BusinessApiNewCyber.aplicaApiCyber())
                {
                    //Busca a escola que vai ser deletada
                    escolaApiCyberBdUi = BusinessEmpresa.getEscola(entity.cd_pessoa);
                }

                entity.cd_endereco_principal = null;
                Dao.edit(entity, false);
                deletado = Dao.delete(entity, false);


                if (BusinessApiNewCyber.aplicaApiCyber())
                {
                    if (escolaApiCyberBdUi.nm_cliente_integracao != null && escolaApiCyberBdUi.nm_cliente_integracao > 0 && existeUnidade((int)escolaApiCyberBdUi.nm_cliente_integracao))
                    {
                        //Chama a api cyber com o comando (INATIVA_UNIDADE)
                        executaCyberInativaUnidade((int)escolaApiCyberBdUi.nm_cliente_integracao);

                    }
                }
                

                transaction.Complete();
            }
            return deletado;
        }

        public byte getNivelPlanoContasEscola(int cd_pessoa_empresa)
        {
            return DaoParametro.getParametrosByEscola(cd_pessoa_empresa).nm_niveis_plano_contas ?? 0;
        }

        public IEnumerable<PessoaSearchUI> getSearchEscolas(SearchParameters parametros, string desc, string cnpj, string fantasia, bool inicio)
        {
            IEnumerable<PessoaSearchUI> retorno = new List<PessoaSearchUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "no_pessoa";
                parametros.sort = parametros.sort.Replace("dt_cadastro", "dt_cadastramento");
                parametros.sort = parametros.sort.Replace("escola_ativa", "id_pessoa_ativa");
                retorno = Dao.getSearchEscolas(parametros, desc, cnpj, fantasia, inicio);
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<PessoaSearchUI> getEscolaNotWithItem(SearchParameters parametros, string desc, string cnpj, string fantasia, int cd_item, bool inicio)
        {
            if (parametros.sort == null)
                parametros.sort = "no_pessoa";
            parametros.sort = parametros.sort.Replace("dt_cadastro", "dt_cadastramento");
            parametros.sort = parametros.sort.Replace("escola_ativa", "id_pessoa_ativa");
            return Dao.getEscolaNotWithItem(parametros, desc, cnpj, fantasia, cd_item, inicio);
        }

        public IEnumerable<PessoaSearchUI> getEscolaNotWithKit(SearchParameters parametros, string desc, List<int> empresas, string cnpj, string fantasia, int cd_item, bool inicio)
        {
            if (parametros.sort == null)
                parametros.sort = "dc_reduzido_pessoa";
            parametros.sort = parametros.sort.Replace("no_pessoa", "dc_reduzido_pessoa");
            parametros.sort = parametros.sort.Replace("dt_cadastro", "dt_cadastramento");
            parametros.sort = parametros.sort.Replace("escola_ativa", "id_pessoa_ativa");
            return Dao.getEscolaNotWithKit(parametros, desc, empresas, cnpj, fantasia, cd_item, inicio);
        }

        public IEnumerable<PessoaSearchUI> getEscolaHasItem(int cd_item)
        {
            return Dao.getEscolaHasItem(cd_item);
        }

        public IEnumerable<TurmaEscolaSearchUI> getTurmasEscolatWithTurma(int cd_turma)
        {
            return DataAccessTurmaEscola.getTurmasEscolatWithTurma(cd_turma);
        }

        public IEnumerable<AtividadeEscolaAtividadeSearchUI> getAtividadeEscolatWithAtividade(int cd_atividade_extra)
        {
            return DataAccessAtividadeEscola.getAtividadeEscolatWithAtividade(cd_atividade_extra);
        }

        public IEnumerable<PessoaSearchUI> getEscolaHasTpDesc(int cdTpDesc)
        {
            return Dao.getEscolaHasTpDesc(cdTpDesc);
        }

        public bool verificaHorarioOcupado(int cd_pessoa_escola, TimeSpan hr_ini, TimeSpan hr_fim)
        {
            return verificaHorarioOcupado(cd_pessoa_escola, hr_ini, hr_fim);
        }

        public EscolaUI getEscolaForEdit(int cd_pessoa_empresa)
        {
            return Dao.getEscolaForEdit(cd_pessoa_empresa);
        }

        public string verifcaEstadoEscAluno(int cd_escola, int cd_pessoa, int tipoMovto)
        {
            bool? igual = Dao.verifcaEstadoEscAluno(cd_escola, cd_pessoa);
            if (!igual.HasValue)
                throw new FinanceiroBusinessException(Utils.Messages.Messages.msgErroEstadoEscPessoa, null, FinanceiroBusinessException.TipoErro.ERRO_NAO_EXISTE_ESTADO_ESC_PESSOA, false);
            string tipo_cfop = "";
            if ((int)Movimento.TipoMovimentoEnum.ENTRADA == tipoMovto)
            {
                if (!igual.Value)
                    tipo_cfop = (int)Movimento.CfOPEnum.ENTRADAFORAESTADO + "";
                else
                    tipo_cfop = (int)Movimento.CfOPEnum.ENTRADADENTROESTADO + "";
            }
            else
            {
                if (!igual.Value)
                    tipo_cfop = (int)Movimento.CfOPEnum.SAIDAFORADOESTADO + "";
                else
                    tipo_cfop = (int)Movimento.CfOPEnum.SAIDADENTROESTADO + "";
            }
            return tipo_cfop;
        }

        public IEnumerable<PessoaSearchUI> getSearchEscolasFKFollowUp(SearchParameters parametros, string desc, string cnpj, string fantasia, bool inicio,
            List<int> cdsEmpresa, bool masterGeral, int? cd_estado, int? cd_cidade)
        {
            IEnumerable<PessoaSearchUI> retorno = new List<PessoaSearchUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "no_pessoa";
                parametros.sort = parametros.sort.Replace("dt_cadastro", "dt_cadastramento");
                parametros.sort = parametros.sort.Replace("escola_ativa", "id_pessoa_ativa");
                retorno = Dao.getSearchEscolasFKFollowUp(parametros, desc, cnpj, fantasia, inicio, cdsEmpresa, masterGeral, cd_estado, cd_cidade);
                transaction.Complete();
            }
            return retorno;
        }

        public bool getEmpresaPropria(int cd_escola)
        {
            return Dao.getEmpresaPropria(cd_escola);
        }

        #endregion

        #region Parametros

        //*****Métodos para parametro*****\\
        /// <summary>
        /// Insere parametros para a escola
        /// </summary>
        /// <param name="entity"></param>

        public Parametro getParametrosByEscola(int cdEscola)
        {
            Parametro parametroEscola = DaoParametro.getParametrosByEscola(cdEscola);
            if (parametroEscola == null || parametroEscola.nm_niveis_plano_contas == null || parametroEscola.nm_niveis_plano_contas <= 0)
                throw new EscolaBusinessException(Utils.Messages.Messages.msgErroEscolaSemPlanoContas + " ", null, EscolaBusinessException.TipoErro.ERRO_NIVEIS_PLANO_CONTAS, false);
            return parametroEscola;
        }

        public Parametro getParametrosMatricula(int cdEscola)
        {
            return DaoParametro.getParametrosMatricula(cdEscola);
        }

        public Parametro getParametrosPlanoTxMatricula(int cdEscola)
        {
            return DaoParametro.getParametrosPlanoTxMatricula(cdEscola);
        }

        public int getParametroNiveisPlanoContas(int cd_escola)
        {
            int? parametro = DaoParametro.getParametroNiveisPlanoContas(cd_escola);

            if (!parametro.HasValue || parametro.Value == 0)
                throw new EscolaBusinessException(Utils.Messages.Messages.msgErroEscolaSemPlanoContas2 + " ", null, EscolaBusinessException.TipoErro.ERRO_NIVEIS_PLANO_CONTAS, false);

            return parametro.Value;
        }

        public Parametro insertParametro(EscolaUI entity, int cdEscola)
        {
            entity.parametro.cd_pessoa_escola = cdEscola;
            return DaoParametro.add(entity.parametro, false);
        }

        public Parametro editParametro(EscolaUI entity, Parametro parametroExists, bool isMasterGeral)
        {
            Parametro parametro = new Parametro();

            if (parametroExists == null)
            {
                entity.parametro.cd_pessoa_escola = entity.pessoaJuridica.pessoaJuridica.cd_pessoa;
                parametro = DaoParametro.add(entity.parametro, false);
            }
            else
            {
                parametroExists = changeValuesParametros(entity, parametroExists, isMasterGeral);
                DaoParametro.saveChanges(false);
            }
            return parametro;
        }

        private static Parametro changeValuesParametros(EscolaUI entity, Parametro parametroExists, bool isMasterGeral)
        {
            if (!isMasterGeral)
                if ((entity.parametro.id_nro_contrato_automatico != parametroExists.id_nro_contrato_automatico)
                    || (entity.parametro.id_requer_plano_contas_mov != parametroExists.id_requer_plano_contas_mov)
                    || (entity.parametro.id_financeiro_negativo != parametroExists.id_financeiro_negativo)
                    || (entity.parametro.nm_niveis_plano_contas != parametroExists.nm_niveis_plano_contas)
                    || (entity.parametro.cd_item_servico != parametroExists.cd_item_servico)
                    || (entity.parametro.cd_tipo_nf_servico != parametroExists.cd_tipo_nf_servico)
                    || (entity.parametro.cd_plano_conta_servico != parametroExists.cd_plano_conta_servico)
                    || (entity.parametro.cd_politica_servico != parametroExists.cd_politica_servico))
                    throw new EscolaBusinessException(Utils.Messages.Messages.msgUsuarioSemPermissao + " ", null, EscolaBusinessException.TipoErro.ERRO_USUARIO_SEM_PERMISSAO, false);

            parametroExists.copy(entity.parametro);

            parametroExists.cd_plano_conta_mat = entity.parametro.cd_plano_conta_mat;
            parametroExists.cd_plano_conta_tax = entity.parametro.cd_plano_conta_tax;
            parametroExists.cd_plano_conta_taxbco = entity.parametro.cd_plano_conta_taxbco;
            parametroExists.cd_plano_conta_trf = entity.parametro.cd_plano_conta_trf;
            parametroExists.cd_plano_conta_material = entity.parametro.cd_plano_conta_material;

            parametroExists.pc_juros_dia = entity.parametro.pc_juros_dia;
            parametroExists.cd_pessoa_escola = entity.pessoaJuridica.pessoaJuridica.cd_pessoa;
            parametroExists.nm_dia_vencimento = entity.parametro.nm_dia_vencimento;
            parametroExists.cd_local_movto = entity.parametro.cd_local_movto;

            parametroExists.id_emitir_nf_servico = entity.parametro.id_emitir_nf_servico;
            parametroExists.id_emitir_nf_mercantil = entity.parametro.id_emitir_nf_mercantil;
            parametroExists.id_numero_nf_automatico = entity.parametro.id_numero_nf_automatico;
            parametroExists.id_regime_tributario = entity.parametro.id_regime_tributario;
            parametroExists.nm_nf_servico = entity.parametro.nm_nf_servico;
            parametroExists.dc_serie_nf_servico = entity.parametro.dc_serie_nf_servico;
            parametroExists.nm_nf_mercantil = entity.parametro.nm_nf_mercantil;
            parametroExists.dc_serie_nf_mercantil = entity.parametro.dc_serie_nf_mercantil;
            parametroExists.cd_item_taxa_matricula = entity.parametro.cd_item_taxa_matricula;
            parametroExists.cd_item_mensalidade = entity.parametro.cd_item_mensalidade;
            parametroExists.cd_item_biblioteca = entity.parametro.cd_item_biblioteca;
            parametroExists.cd_tipo_nf_matricula = entity.parametro.cd_tipo_nf_matricula;
            parametroExists.cd_tipo_nf_material = entity.parametro.cd_tipo_nf_material;
            parametroExists.cd_tipo_nf_material_saida = entity.parametro.cd_tipo_nf_material_saida;
            parametroExists.cd_tipo_nf_biblioteca = entity.parametro.cd_tipo_nf_biblioteca;
            parametroExists.cd_politica_comercial_nf = entity.parametro.cd_politica_comercial_nf;
            parametroExists.id_gerar_financeiro_contrato = entity.parametro.id_gerar_financeiro_contrato;
            parametroExists.pc_aliquota_ap_saida = entity.parametro.pc_aliquota_ap_saida;
            parametroExists.pc_aliquota_ap_servico = entity.parametro.pc_aliquota_ap_servico;
            parametroExists.nm_dias_titulos_abertos = entity.parametro.nm_dias_titulos_abertos;

            return parametroExists;
        }

        public Parametro getParametrosBaixa(int cd_pessoa_escola)
        {
            Parametro retorno = new Parametro();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = DaoParametro.getParametrosBaixa(cd_pessoa_escola);
                transaction.Complete();
            }
            return retorno;
        }

        public int? getLocalMovto(int cd_escola)
        {
            return DaoParametro.getLocalMovto(cd_escola);
        }

        public Parametro getParametrosMovimento(int cd_escola)
        {
            return DaoParametro.getParametrosMovimento(cd_escola);
        }

        public Parametro getParametros(int cdEscola)
        {
            return DaoParametro.getParametrosByEscola(cdEscola);
        }

        public bool getIdBloquearVendasSemEstoque(int cd_empresa)
        {
            return DaoParametro.getIdBloquearVendasSemEstoque(cd_empresa);
        }

        public Parametro insertParametro(Parametro parametro)
        {
            Parametro newParametro = new Parametro();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                newParametro = DaoParametro.getParametrosByEscola(parametro.cd_pessoa_escola);

                IEnumerable<PlanoConta> planos = BusinessCoordenacao.getPlanoContaByIdEscola(parametro.cd_pessoa_escola);

                if (newParametro != null)
                {
                    if (planos != null && planos.Count() > 0 && newParametro.nm_niveis_plano_contas != null && newParametro.nm_niveis_plano_contas != parametro.nm_niveis_plano_contas)
                        throw new EscolaBusinessException(string.Format(Utils.Messages.Messages.msgNotUpdateParametro, planos.Select(s => s.EmpresaPlanoConta.no_pessoa)), null, EscolaBusinessException.TipoErro.ERRO_NIVEL_PARAMETRO, false);
                    else
                    {
                        newParametro.copy(parametro);
                        DaoParametro.saveChanges(false);
                    }
                }
                else newParametro = DaoParametro.add(parametro, false);

                transaction.Complete();
            }
            return newParametro;
        }

        public bool getIdBloquearliqTituloAnteriorAberto(int cd_empresa)
        {
            return DaoParametro.getIdBloquearliqTituloAnteriorAberto(cd_empresa);
        }

        public byte? getParametroNiviesPlanoConta(int cdEscola)
        {
            return DaoParametro.getParametroNiviesPlanoConta(cdEscola);
        }

        public int getParametroMovimentoRetroativo(int cd_escola)
        {
            return DaoParametro.getParametroMovimentoRetroativo(cd_escola);
        }

        public DateTime getParametrosPrevDevolucao(int cd_escola, DateTime dataEmprestimo)
        {
            byte? dias = DaoParametro.getParametrosPrevDevolucao(cd_escola);
            if (dias.HasValue)
                dataEmprestimo = dataEmprestimo.AddDays(dias.Value);
            return dataEmprestimo;
        }

        private Movimento gerarNumeroNFMovimento(Movimento movimento)
        {
            if ((movimento.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.SAIDA ||
                 movimento.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.SERVICO ||
                 movimento.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.DEVOLUCAO) &&
                DaoParametro.getParametroNumeracaoAutoNF(movimento.cd_pessoa_empresa) && movimento.id_nf_escola)
            {
                if (movimento.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.SAIDA || movimento.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.DEVOLUCAO)
                {
                    Parametro param = DaoParametro.getNumeroESerieNFPorTipo(Movimento.TipoMovimentoEnum.SAIDA, movimento.cd_pessoa_empresa);
                    movimento.nm_movimento = param.nm_nf_mercantil;
                    movimento.dc_serie_movimento = param.dc_serie_nf_mercantil;
                }
                else
                {
                    int tipoMovimentoServico = (int)Movimento.TipoMovimentoEnum.SAIDA;
                    if (movimento.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.SERVICO && movimento.cd_tipo_nota_fiscal.HasValue)
                        tipoMovimentoServico = (int)BusinessFiscal.getTipoMvtoTpNF(movimento.cd_tipo_nota_fiscal.Value);
                    if (movimento.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.SERVICO && tipoMovimentoServico == (int)Movimento.TipoMovimentoEnum.SAIDA)
                    {
                        Parametro param = DaoParametro.getNumeroESerieNFPorTipo(Movimento.TipoMovimentoEnum.SERVICO, movimento.cd_pessoa_empresa);
                        movimento.nm_movimento = param.nm_nf_servico;
                        movimento.dc_serie_movimento = param.dc_serie_nf_servico;
                    }
                }
            }
            return movimento;
        }

        public byte getParametroRegimeTrib(int cd_escola)
        {
            return DaoParametro.getParametroRegimeTrib(cd_escola);
        }

        public int getParametroNmFaltasAluno(int cd_escola)
        {
            return DaoParametro.getParametroNmFaltasAluno(cd_escola);
        }

        public bool getImprimir3BoletosPagina(int cd_escola)
        {
            return DaoParametro.getImprimir3BoletosPagina(cd_escola);
        }

        public byte? getTipoNumeroContrato(int cd_empresa)
        {
            return DaoParametro.getTipoNumeroContrato(cd_empresa);
        }

        public bool getParametroHabilitacaoProfessor(int cd_escola)
        {
            return DaoParametro.getParametroHabilitacaoProfessor(cd_escola);
        }

        #endregion

        #region usuário

        public IEnumerable<EscolaUI> findEscolasSecionadas(int cdItem, int cd_usuario)
        {
            bool master = BusinessUsuario.VerificarMasterGeral(cd_usuario);
            return Dao.findEscolasSecionadas(cdItem, cd_usuario, master);
        }

        public UsuarioUISearch PostInsertUsuario(UsuarioWebSGF usuario, PessoaFisicaSGF pessoaFisica, int cdEmp, List<Horario> horarios)
        {
            sincronizarContextos(Dao.DB());
            UsuarioUISearch usuarioUISearch = null;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                usuarioUISearch = BusinessEmpresa.PostInsertUsuario(usuario, pessoaFisica, cdEmp);
                if (horarios != null)
                    crudHorarioUsuario(horarios, usuarioUISearch.cd_usuario, cdEmp, false);
                transaction.Complete();
            }
            return usuarioUISearch;
        }

        public UsuarioUISearch PostEditUsuario(UsuarioWebSGF usuario, int cdEmp, List<Horario> horarios)
        {
            sincronizarContextos(Dao.DB());
            UsuarioUISearch usuarioUISearch = null;
            //using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            //{

                usuarioUISearch = BusinessEmpresa.PostEditUsuario(usuario, cdEmp);
                if (horarios != null)
                    crudHorarioUsuario(horarios, usuarioUISearch.cd_usuario, cdEmp, false);
            //    transaction.Complete();
            //}
            return usuarioUISearch;
        }

        public bool DeleteUsuario(List<UsuarioWebSGF> listUsuario, int cd_empresa)
        {
            sincronizarContextos(Dao.DB());
            bool retorno = false;
            List<int> cdUsers = listUsuario.Select(x => x.cd_usuario).ToList();

            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                List<UsuarioWebSGF> listaUsersContext = Dao.getUsuariosById(cdUsers).ToList();
                List<UsuarioWebSGF> idsUsuariosDeleteAreaRestrita = new List<UsuarioWebSGF>();
                if (listaUsersContext != null && listaUsersContext.Count() > 0)
                {
                    foreach (var user in listaUsersContext)
                    {
                        if (user != null)
                        {
                            crudHorarioUsuario(new List<Horario>(), user.cd_usuario, cd_empresa, true);
                            Dao.deleteUsuarioContext(user);
                            if (user.id_area_resrtrita != null)
                            {
                                idsUsuariosDeleteAreaRestrita.Add(user);
                            }
                        }
                    }
                    Dao.saveChanges(false);

                    if (idsUsuariosDeleteAreaRestrita != null && idsUsuariosDeleteAreaRestrita.Count > 0)
                    {
                        BusinessEmpresa.DeleteUsuarioApiAreaRestrita(idsUsuariosDeleteAreaRestrita);
                    }
                    
                   

                    retorno = true;
                }
                transaction.Complete();
            }
            return retorno;
        }

        public void crudHorarioUsuario(List<Horario> horariosView, int cd_usuario, int cd_empresa, bool deleteUser)
        {
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                Horario horario = new Horario();
                List<Horario> horarioContext = new List<Horario>();
                if (!deleteUser)
                    horarioContext = BusinessSecretaria.getHorarioByEscolaForRegistro(cd_empresa, cd_usuario, Horario.Origem.USUARIO).ToList();
                else
                    horarioContext = BusinessSecretaria.getHorarioByEscolaForRegistro(0, cd_usuario, Horario.Origem.USUARIO).ToList();
                IEnumerable<Horario> horarioComCodigo = from hpts in horariosView
                                                        where hpts.cd_horario != 0
                                                        select hpts;
                List<Horario> horarioDeleted = horarioContext.Where(tc => !horarioComCodigo.Any(tv => tc.cd_horario == tv.cd_horario)).ToList();
                if (horarioDeleted.Count() > 0)
                {
                    foreach (var item in horarioDeleted)
                    {
                        //var deletarHorarioProfessor = (from hp in horarioContext where hp.cd_horario == item.cd_horario select hp).FirstOrDefault();
                        if (item != null)
                        {
                            //DataAccessHorario.sincronizaContexto(DataAccessAluno.DB());
                            BusinessSecretaria.deleteHorario(item);
                        }
                    }
                }
                foreach (var item in horariosView)
                {
                    if (item.cd_horario.Equals(null) || item.cd_horario == 0)
                    {
                        item.cd_pessoa_escola = cd_empresa;
                        item.cd_registro = cd_usuario;
                        item.dt_hora_ini = new TimeSpan(item.startTime.Hour, item.startTime.Minute, 0);
                        item.dt_hora_fim = new TimeSpan(item.endTime.Hour, item.endTime.Minute, 0);
                        item.id_origem = (int)Horario.Origem.USUARIO;
                        BusinessSecretaria.addHorario(item);
                    }
                    else
                    {
                        var horarioUser = (from hp in horarioContext where hp.cd_horario == item.cd_horario select hp).FirstOrDefault();
                        if (horarioUser != null && horarioUser.cd_horario > 0)
                        {
                            //DataAccessHorario.sincronizaContexto(DataAccessAluno.DB());
                            item.cd_registro = horarioUser.cd_registro;
                            //horarioUser = Horario.changeValueHorario(horarioUser, item);
                            BusinessSecretaria.editHorarioContext(horarioUser, item);
                        }
                    }
                }
                transaction.Complete();
            }
        }

        public IEnumerable<UsuarioUISearch> getUsuarioSearchFKFollowUp(SearchParameters parametros, string descricao, string nome, bool inicio, int cd_empresa,
                                                               int cd_usuario_logado, int tipoPesq, string usuariologado, Int32[] codEscolas)
        {
            IEnumerable<UsuarioUISearch> retorno = new List<UsuarioUISearch>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "no_pessoa";
                parametros.sort = parametros.sort.Replace("ativo", "id_usuario_ativo");
                parametros.sort = parametros.sort.Replace("Master", "id_master");
                parametros.sort = parametros.sort.Replace("sysAdmin", "id_admin");
                parametros.sort = parametros.sort.Replace("Administrador", "id_administrador");
                retorno = Dao.getUsuarioSearchFKFollowUp(parametros, descricao, nome, inicio, cd_empresa, cd_usuario_logado, tipoPesq, usuariologado, codEscolas);
                transaction.Complete();
            }
            return retorno;
        }

        #endregion

        #region Sala

        public List<Sala> verficarSalasDisponiveisPorEscola(int cd_escola)
        {
            Escola escola = Dao.findById(cd_escola, false);
            return null;
        }

        #endregion

        #region Matricula

        public Contrato PostMatricula(Contrato contrato, string pathContratosEscola, int cdUsuario, int fusoHorario)
        {
            string nmNomeContratoDigitalizadoRetorno = "";
            string msgProcedureGerarNota = "";
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED, DaoParametro.DB(), TransactionScopeBuilder.TransactionTime.DEFAULT))
            {
                sincronizarContextos(DaoParametro.DB());
                Parametro parametros = getParametrosMatricula(contrato.cd_pessoa_escola);
                if (parametros.id_nro_contrato_automatico == true)
                {
                    contrato.nm_contrato = BusinessMatricula.getNroUltimoContrato(parametros.nm_ultimo_contrato, contrato.cd_pessoa_escola) + 1;
                    if (!contrato.nm_contrato.HasValue || contrato.nm_contrato <= 0)
                        contrato.nm_contrato = 1;
                }
                if (parametros.id_tipo_numero_contrato == (int)FundacaoFisk.SGF.GenericModel.Parametro.TipoNumeroMatricula.IGUAL_CONTRATO
                    || parametros.id_tipo_numero_contrato == null) //Default 1 no banco de dados.
                    contrato.nm_matricula_contrato = contrato.nm_contrato;
                else
                    if (parametros.id_tipo_numero_contrato == (int)FundacaoFisk.SGF.GenericModel.Parametro.TipoNumeroMatricula.INCREMENTAL_AUTOMATICO)
                    {
                        contrato.nm_matricula_contrato = BusinessMatricula.getUltimoNroMatricula(parametros.nm_ultimo_matricula, contrato.cd_pessoa_escola) + 1;
                        if (!contrato.nm_matricula_contrato.HasValue || contrato.nm_matricula_contrato <= 0)
                            contrato.nm_matricula_contrato = 1;
                    }

                if (!contrato.nm_contrato.HasValue || contrato.nm_contrato.Value <= 0 || !contrato.nm_matricula_contrato.HasValue || contrato.nm_matricula_contrato.Value <= 0)
                    throw new MatriculaBusinessException(Utils.Messages.Messages.msgErroContratoNulo, null, MatriculaBusinessException.TipoErro.ERRO_NRO_CONTRATO_NULO, false); //Este erro ocorre quando troca o parametro da escola quando alguém já solicitou a inclusão do contrato na tela.

                if (contrato.AlunoTurma != null)
                {
                    List<AlunoTurma> alunoTurmas = new List<AlunoTurma>();
                    int countPPT = 0;
                    int cdCurso = contrato.cd_curso_atual;

                    foreach (AlunoTurma alunoTurmaPpt in contrato.AlunoTurma)
                    {
                        alunoTurmaPpt.Turma = BusinessTurma.findTurmasByIdAndCdEscola(alunoTurmaPpt.cd_turma, contrato.cd_pessoa_escola);
                        if (alunoTurmaPpt.Turma.id_turma_ppt)
                        {
                            if (alunoTurmaPpt.CursoContrato != null)
                                cdCurso = alunoTurmaPpt.CursoContrato.cd_curso;
                            alunoTurmaPpt.Turma = BusinessCoordenacao.GerarTurmaFilha(contrato, alunoTurmaPpt.cd_turma, cdCurso);
                            alunoTurmaPpt.cd_turma = alunoTurmaPpt.Turma.cd_turma;
                            alunoTurmas.Add(alunoTurmaPpt);
                            countPPT = countPPT + 1;
                        }
                    }

                    if (countPPT > 0)
                    {
                        contrato.AlunoTurma = null;
                        contrato.AlunoTurma = alunoTurmas;
                    }
                    List<AlunoTurma> alunoAguardando = contrato.AlunoTurma.ToList();
                    int countAguardando = 0;
                    foreach (AlunoTurma alunoTurma in contrato.AlunoTurma)
                        if (alunoTurma.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Aguardando)
                        {
                            alunoTurma.nm_matricula_turma = contrato.nm_matricula_contrato;
                            alunoAguardando.Add(alunoTurma);
                            countAguardando = countAguardando + 1;
                        }

                    if (countAguardando == 1)
                    {
                        alunoAguardando[0].Turma = BusinessTurma.findTurmasByIdAndCdEscola(alunoAguardando[0].cd_turma, contrato.cd_pessoa_escola);
                        if (alunoAguardando[0].Turma != null && alunoAguardando[0].Turma.dt_termino_turma.HasValue)
                            throw new MatriculaBusinessException(Utils.Messages.Messages.msgErroMatTurmaEnc, null, MatriculaBusinessException.TipoErro.ERRO_TURMA_ENCERRADA, false);
                        //Vagas
                        TurmaSearch turma = BusinessTurma.getTurmaByCodMudancaOuEncerramento(alunoAguardando[0].cd_turma, contrato.cd_pessoa_escola);
                        if (turma != null && turma.considera_vagas == true)
                        {
                            int vagasDisp = turma.vagas_disponiveis - turma.nro_alunos;
                            if (vagasDisp < 1)
                                throw new TurmaBusinessException(string.Format(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroVagasTurma), null, TurmaBusinessException.TipoErro.ERRO_NUMERO_VAGAS, false);
                        }
                    }
                }
                //Não informado o responsável e não foi cadastrado, usar aluo.
                if (contrato.cd_pessoa_responsavel == 0 && contrato.pessoaFisicaResponsavel == null && contrato.pessoajuridicaResponsavel == null)
                    contrato.cd_pessoa_responsavel = contrato.cd_pessoa_aluno;
                int cd_pessoa_responsavel_cad = 0;
                //Não informado o responsável e foi cadastrado um.
                if (contrato.pessoaFisicaResponsavel != null || contrato.pessoajuridicaResponsavel != null)
                    cd_pessoa_responsavel_cad = inserirResponsavelMatricula(contrato);
                //Não informado o responsável, usar o responsável cadastrado.
                if (contrato.cd_pessoa_responsavel == 0)
                    contrato.cd_pessoa_responsavel = cd_pessoa_responsavel_cad;
                
                //Pega e altera o nome do arquivo digitalizado adicionando prefixo numeroContrato_
                string nome_contrato_digitalizado = (!String.IsNullOrEmpty(contrato.nm_arquivo_digitalizado)? (contrato.nm_contrato + "__" + contrato.nm_arquivo_digitalizado): contrato.nm_arquivo_digitalizado);
                string nome_contrato_digitalizado_temporario = contrato.nm_arquivo_digitalizado_temporario;
                contrato.nm_arquivo_digitalizado = nome_contrato_digitalizado;
                contrato = BusinessMatricula.PostMatricula(contrato, cdUsuario, fusoHorario);
                msgProcedureGerarNota = contrato.msgProcedureGerarNota;
                DateTime dtEmis = new DateTime();
                if (contrato.dt_matricula_contrato != null)
                    dtEmis = (DateTime)contrato.dt_matricula_contrato;
                if (contrato.titulos != null && contrato.titulos.Count() > 0 && parametros.id_gerar_financeiro_contrato)
                {
                    foreach (Titulo t in contrato.titulos.ToList())
                    {
                        if (t.cd_pessoa_responsavel == 0 && cd_pessoa_responsavel_cad > 0)
                            t.cd_pessoa_responsavel = cd_pessoa_responsavel_cad;
                        if (!t.dh_cadastro_titulo.HasValue)
                            t.dh_cadastro_titulo = DateTime.Now.ToUniversalTime();
                        if (t.cd_pessoa_empresa <= 0)
                            t.cd_pessoa_empresa = contrato.cd_pessoa_escola;
                        if (!t.id_status_titulo.HasValue)
                            t.id_status_titulo = 1;
                        if (!t.id_natureza_titulo.HasValue)
                            t.id_natureza_titulo = 1;
                        if (t.cd_titulo == 0 && t.vl_titulo != t.vl_saldo_titulo)
                            t.vl_saldo_titulo = t.vl_titulo;
                        t.nm_titulo = contrato.nm_contrato;
                        t.cd_origem_titulo = contrato.cd_contrato;
                        t.dt_emissao_titulo = t.dt_emissao_titulo.Date;
                        t.dt_vcto_titulo = t.dt_vcto_titulo.Date;
                        if (t.cd_local_movto <= 0 && parametros.cd_local_movto.HasValue)
                            t.cd_local_movto = parametros.cd_local_movto.Value;
                    }
                    BusinessFinanceiro.addTitulos(contrato.titulos.ToList());
                }
                if (!contrato.id_ajuste_manual)
                {
                    DateTime dt_baixa_titulo = contrato.dt_matricula_contrato.HasValue ? contrato.dt_matricula_contrato.Value : DateTime.Now.Date;
                    if (contrato.pc_desconto_bolsa > 0)
                        BusinessFinanceiro.gerarBaixaParcialBolsaTitulos(contrato.titulos.ToList(), contrato.pc_desconto_bolsa, dt_baixa_titulo, true);
                }
                parametros = DaoParametro.getParametrosByEscola(contrato.cd_pessoa_escola);
                parametros.nm_ultimo_matricula = contrato.nm_matricula_contrato.Value;
                parametros.nm_ultimo_contrato = contrato.nm_contrato.Value;
                DaoParametro.saveChanges(false);

                contrato.nm_arquivo_digitalizado = nome_contrato_digitalizado;
                contrato.nm_arquivo_digitalizado_temporario = nome_contrato_digitalizado_temporario;
                nmNomeContratoDigitalizadoRetorno = contrato.nm_arquivo_digitalizado;
                SalvarArquivoContratoDigitalizado(contrato, pathContratosEscola);

                if (BusinessApiNewCyber.aplicaApiCyber())
                {
                    //Pega a lista de alunos (ativos ou rematriculados) do contrato
                    List<AlunoTurma> listaAlunoTurmaCurrent = BusinessAluno.getAlunoTurmaByCdContrato(contrato.cd_contrato); //getAlunoTurmaByCdContrato(contrato.cd_contrato);

                    foreach (AlunoTurma alunoTurma in listaAlunoTurmaCurrent)
                    {

                        //monta o objeto alunoCyber para chamar apicyber
                        AlunoApiCyberBdUI alunoCyberCurrent = new AlunoApiCyberBdUI();
                        alunoCyberCurrent = BusinessAluno.findAlunoApiCyber(alunoTurma.cd_aluno, alunoTurma.cd_turma, contrato.cd_contrato);

                        //se aluno ativo ou rematriculado existe e tem o id_unidade e não existe no cyber-> chama a apicyber com comando (CADASTRA_ALUNO)
                        if (alunoCyberCurrent != null && (alunoCyberCurrent.id_unidade != null && alunoCyberCurrent.id_unidade > 0) &&
                            !existeAluno(alunoCyberCurrent.codigo))
                        {
                            cadastraAlunoApiCyber(alunoCyberCurrent, ApiCyberComandosNames.CADASTRA_ALUNO);
                        }

                        //Ativa o aluno no cyber caso esteja rematriculado
                        if (alunoTurma.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado)
                        {
                            executaCyberAtivaAluno(alunoTurma.cd_aluno);
                        }

                        //monta o objeto livroAlunoCyber para chamar a apicyber
                        LivroAlunoApiCyberBdUI livroAlunoCyberCurrent = new LivroAlunoApiCyberBdUI();
                        livroAlunoCyberCurrent = BusinessAluno.findLivroAlunoApiCyber(alunoTurma.cd_aluno, alunoTurma.cd_turma, contrato.cd_contrato);

                        //se livro existe no bd e tem o codigo_livro -> (aluno e grupo) existe no cyber -> livroAluno não existe no cyber -> chama a apicyber com comando (CADASTRA_LIVROALUNO)
                        if (livroAlunoCyberCurrent != null && livroAlunoCyberCurrent.codigo_unidade != null && livroAlunoCyberCurrent.codigo_livro > 0 &&
                            existeAluno(livroAlunoCyberCurrent.codigo_aluno) &&
                            existeGrupoByCodigoGrupo(livroAlunoCyberCurrent.codigo_grupo) &&
                            !existeLivroAlunoByCodAluno(livroAlunoCyberCurrent.codigo_aluno, livroAlunoCyberCurrent.codigo_grupo, livroAlunoCyberCurrent.codigo_livro))
                        {
                            cadastraLivroAlunoApiCyber(livroAlunoCyberCurrent, ApiCyberComandosNames.CADASTRA_LIVROALUNO);
                        }
                    }
                }

                if (BusinessApiPromocaoIntercambio.aplicaApiPromocao() && (contrato.id_tipo_matricula == (int)Contrato.TipoMatricula.MATRICULA || contrato.id_tipo_matricula == (int)Contrato.TipoMatricula.REMATRICULA))
                {
                    
                    //monta o objeto
                    PromocaoIntercambioParams alunoPromocaoIntercambioCurrent = new PromocaoIntercambioParams();
                    alunoPromocaoIntercambioCurrent = BusinessAluno.findAlunoApiPromocaoIntercambio(contrato.cd_aluno, (contrato.id_tipo_matricula == (int)Contrato.TipoMatricula.MATRICULA ? 2 : 3));

                    BusinessApiPromocaoIntercambio.ValidaParametros(alunoPromocaoIntercambioCurrent);
                    string codigo_promocao = BusinessApiPromocaoIntercambio.postExecutaRequestPromocaoIntercambio(alunoPromocaoIntercambioCurrent);

                    if (!string.IsNullOrEmpty(codigo_promocao))
                    {
                        
                            PessoaPromocao pessoaPromocao = new PessoaPromocao();
                            pessoaPromocao.cd_pessoa = contrato.cd_aluno;
                            pessoaPromocao.id_tipo_pessoa = 1;
                            pessoaPromocao.dc_promocao = codigo_promocao;
                            BusinessSecretaria.addPessoaPromocao(pessoaPromocao);

                    }
                    


                }


                transaction.Complete();
            }
            contrato = BusinessMatricula.getMatriculaByIdVI(contrato.cd_contrato, contrato.cd_pessoa_escola);
            contrato.msgProcedureGerarNota = msgProcedureGerarNota;
            List<AlunoTurma> novoAlunoTurma = BusinessAluno.findAlunoTurmasByContratoEscola(contrato.cd_contrato, contrato.cd_pessoa_escola);
            if (novoAlunoTurma != null && novoAlunoTurma.Count() > 0)
                contrato.AlunoTurma = novoAlunoTurma;

            contrato.nm_arquivo_digitalizado = nmNomeContratoDigitalizadoRetorno;
            return contrato;
        }


        public int getQtdDiarioTurma(int cd_turma, DateTime dt_aula)
        {
            return BusinessTurma.getQtdDiarioTurma(cd_turma, dt_aula);
        }
        public bool existeAluno(int codigo)
        {
            return BusinessApiNewCyber.verificaRegistro(ConfigurationManager.AppSettings["enderecoApiNewCyber"],
                ApiCyberComandosNames.VISUALIZA_ALUNO, ConfigurationManager.AppSettings["chaveApiNewCyber"], codigo);
        }

        public bool existeGrupoByCodigoGrupo(int codigo_grupo)
        {
            return BusinessApiNewCyber.verificaRegistroGrupos(ConfigurationManager.AppSettings["enderecoApiNewCyber"],
                ApiCyberComandosNames.VISUALIZA_GRUPO, ConfigurationManager.AppSettings["chaveApiNewCyber"], codigo_grupo);
        }

        private void cadastraAlunoApiCyber(AlunoApiCyberBdUI alunoCyberCurrent, string comando)
        {

            string parametros = "";

            parametros = validaParametrosCyberCadastroAluno(alunoCyberCurrent, ConfigurationManager.AppSettings["enderecoApiNewCyber"], comando, "");

            string result = BusinessApiNewCyber.postExecutaCyber(ConfigurationManager.AppSettings["enderecoApiNewCyber"],
                comando, ConfigurationManager.AppSettings["chaveApiNewCyber"], parametros);

        }

        public void executaCyberAtivaAluno(int codigo)
        {
            string result = BusinessApiNewCyber.postExecutaCyber(ConfigurationManager.AppSettings["enderecoApiNewCyber"],
                ApiCyberComandosNames.ATIVA_ALUNO, ConfigurationManager.AppSettings["chaveApiNewCyber"], String.Format("codigo={0}", codigo));
        }

        private void cadastraLivroAlunoApiCyber(LivroAlunoApiCyberBdUI livroAlunoCyberCurrent, string comando)
        {

            string parametros = "";

            parametros = validaParametrosCyberCadastroLivroAluno(livroAlunoCyberCurrent, ConfigurationManager.AppSettings["enderecoApiNewCyber"], comando, "");

            string result = BusinessApiNewCyber.postExecutaCyber(ConfigurationManager.AppSettings["enderecoApiNewCyber"],
                comando, ConfigurationManager.AppSettings["chaveApiNewCyber"], parametros);

        }

        private string validaParametrosCyberCadastroAluno(AlunoApiCyberBdUI entity, string url, string comando, string parametros)
        {


            if (entity == null)
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberParametrosNulos, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_PARAMETROS_NULOS, false);
            }

            //valida codigo do grupo
            if (entity.codigo <= 0)
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberCodigoGrupoMenorIgualZero, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_CODIGO_GRUPO_MENOR_IGUAL_ZERO, false);
            }
            
            //valida id_unidade
            if (entity.id_unidade == null || entity.id_unidade <= 0)
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberNmIntegracaoNuloOuMenorIgualZero, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_NM_INTEGRACAO_NULO_OU_MENOR_IGUAL_ZERO, false);
            }
            //valida nome 
            if (String.IsNullOrEmpty(entity.nome))
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberNomeGrupoNuloVazio, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_NOME_GRUPO_NULO_VAZIO, false);
            }

            //valida nome 
            else if (String.IsNullOrEmpty(entity.email))
            {
                throw new ApiNewCyberAlunoException(string.Format(Utils.Messages.Messages.ErroApiCyberEmailAlunoNuloVazio, url, comando, parametros), null, ApiNewCyberAlunoException.TipoErro.ERRO_EMAIL_PESSOA_FISICA_NULA_OU_VAZIA, false);
            }


            string listaParams = "";
            listaParams = string.Format("nome={0},id_unidade={1},codigo={2},email={3}", entity.nome, entity.id_unidade, entity.codigo, entity.email);
            return listaParams;
        }

        private string validaParametrosCyberCadastroLivroAluno(LivroAlunoApiCyberBdUI entity, string url, string comando, string parametros)
        {


            if (entity == null)
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberParametrosNulos, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_PARAMETROS_NULOS, false);
            }


            if (entity.codigo_aluno <= 0)
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberCdAlunoMenorIgualZero, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_COD_ALUNO_MENOR_IGUAL_ZERO, false);
            }

            if (entity.codigo_grupo <= 0)
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberCodigoGrupoMenorIgualZero, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_CODIGO_GRUPO_MENOR_IGUAL_ZERO, false);
            }

            if (entity.codigo_livro <= 0)
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberCodigoGrupoMenorIgualZero, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_CODIGO_LIVRO_MENOR_IGUAL_ZERO, false);
            }


            string listaParams = "";
            listaParams = string.Format("codigo_aluno={0},codigo_grupo={1},codigo_livro={2}", entity.codigo_aluno, entity.codigo_grupo, entity.codigo_livro);
            return listaParams;
        }

        private void SalvarArquivoContratoDigitalizado(Contrato contrato, string pathContratosEscola)
        {

            string documentoContratoTemp = "";
            bool masterGeral = BusinessEmpresa.VerificarMasterGeral(contrato.cd_usuario);

            try
            {
                HashSet<char> invalidFileNameChars = new HashSet<char>(Path.GetInvalidFileNameChars());
                HashSet<char> invalidPathChars = new HashSet<char>(Path.GetInvalidPathChars());
                if (!String.IsNullOrEmpty(contrato.nm_arquivo_digitalizado))
                {
                    string documentoContrato = "";
                    //if (masterGeral)
                    //{
                    //    documentoContrato = pathContratosEscola + "/" + contrato.nm_arquivo_digitalizado;
                    //    documentoContratoTemp = pathContratosEscola + "Temp" + "/" + contrato.nm_arquivo_digitalizado_temporario;
                    //}
                    //else
                    //{
                        documentoContrato = pathContratosEscola + "/" + contrato.cd_pessoa_escola + "/" + contrato.nm_arquivo_digitalizado;
                        documentoContratoTemp = pathContratosEscola + "Temp" + "/" + contrato.cd_pessoa_escola + "/" + contrato.nm_arquivo_digitalizado_temporario;
                    //}

                    if (contrato.nm_arquivo_digitalizado.Any(c => invalidFileNameChars.Contains(c)))
                        throw new SecretariaBusinessException(Utils.Messages.Messages.msgErroCaractInvalidosNomeArquivo + contrato.nm_arquivo_digitalizado, null, SecretariaBusinessException.TipoErro.ERRO_LAYOUT_JA_EXISTE, false);
                    if (pathContratosEscola.Any(c => invalidPathChars.Contains(c)))
                        throw new SecretariaBusinessException(Utils.Messages.Messages.msgErroCaractInvalidosPathArquivo + contrato.nm_arquivo_digitalizado, null, SecretariaBusinessException.TipoErro.ERRO_LAYOUT_JA_EXISTE, false);
                    //string pathContratosEscola = caminho_relatorios + "/Contratos/" + cdEscola;
                    if (System.IO.File.Exists(documentoContrato))
                        throw new SecretariaBusinessException(Utils.Messages.Messages.msgNoLayoutJaCadastrado, null, SecretariaBusinessException.TipoErro.ERRO_LAYOUT_JA_EXISTE, false);
                    //System.IO.File.Delete(documentoContrato);
                    //cria o diretorio
                    DirectoryInfo diContratos = new DirectoryInfo((pathContratosEscola + "/" + contrato.cd_pessoa_escola + "/"));
                    if (!diContratos.Exists)
                        diContratos.Create();

                    System.IO.File.Move(documentoContratoTemp, documentoContrato);
                    if (System.IO.File.Exists(documentoContratoTemp))
                        System.IO.File.Delete(documentoContratoTemp);
                }
            }
            catch (Exception exe)
            {
                //if (masterGeral)
                //    documentoContratoTemp = pathContratosEscola + "Temp" + "/" + contrato.nm_arquivo_digitalizado_temporario;
                //else
                    documentoContratoTemp = pathContratosEscola + "Temp" + "/" + contrato.cd_pessoa_escola + "/" + contrato.nm_arquivo_digitalizado_temporario;

                if (System.IO.File.Exists(documentoContratoTemp))
                    System.IO.File.Delete(documentoContratoTemp);

                throw exe;
            }

        }

        public Contrato postAlterarMatricula(Contrato contrato, bool castMatricula, string pathContratosEscola, int cdUsuario, int fusoHorario)
        {
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED, DaoParametro.DB(), TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                sincronizarContextos(DaoParametro.DB());
                Movimento novoMovimento = new Movimento();
                int cd_pessoa_responsavel_cad = 0;
                string msgProcedureGerarNota = "";
                //Não informado o responsável e foi cadastrado um.
                List<AlunoTurma> listaAlunoTurmaOld = new List<AlunoTurma>();
                if (BusinessApiNewCyber.aplicaApiCyber())
                {
                    //Pega a lista de alunos (ativos ou rematriculados) do contrato antes de alterar
                    listaAlunoTurmaOld = BusinessAluno.getAlunoTurmaByCdContrato(contrato.cd_contrato);
                }
                SGFWebContext db = new SGFWebContext();

                novoMovimento = BusinessFiscal.getMovimentoEditOrigem(contrato.cd_contrato, Int32.Parse(db.LISTA_ORIGEM_LOGS["Contrato"].ToString()), contrato.cd_pessoa_escola, (int)Movimento.TipoMovimentoEnum.SAIDA);
                if (novoMovimento != null && novoMovimento.cd_movimento > 0)
                {
                    Contrato dadosContrato = BusinessMatricula.getMatriculaForMovimento(contrato.cd_contrato, contrato.cd_pessoa_escola);
                    if (dadosContrato.cd_curso_atual != contrato.cd_curso_atual || dadosContrato.id_tipo_contrato != contrato.id_tipo_contrato)
                    {
                        throw new FinanceiroBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgNotUpdateContratoWithNota, null, FinanceiroBusinessException.TipoErro.ERRO_ALTERAR_CONTRATO_WITH_NOTA_MATERIAL, false);
                    }
                    if (dadosContrato.CursoContrato.Count() == 0 || dadosContrato.CursoContrato.Count() == 0)
                    {
                        throw new FinanceiroBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgNotUpdateContratoWithNota, null, FinanceiroBusinessException.TipoErro.ERRO_ALTERAR_CONTRATO_WITH_NOTA_MATERIAL, false);
                    }
                    foreach(CursoContrato d in dadosContrato.CursoContrato.ToList())
                    {
                        foreach (CursoContrato c in contrato.CursoContrato.ToList())
                        {
                            if (d.cd_curso_contrato == c.cd_curso_contrato){
                                if (d.cd_curso != c.cd_curso)
                                    throw new FinanceiroBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgNotUpdateContratoWithNota, null, FinanceiroBusinessException.TipoErro.ERRO_ALTERAR_CONTRATO_WITH_NOTA_MATERIAL, false);
                            }
                        }
                    }
                }

                if (contrato.pessoaFisicaResponsavel != null || contrato.pessoajuridicaResponsavel != null)
                    cd_pessoa_responsavel_cad = inserirResponsavelMatricula(contrato);
                if (contrato.cd_pessoa_responsavel == 0)
                    contrato.cd_pessoa_responsavel = cd_pessoa_responsavel_cad;
                Parametro parametros = getParametrosMatricula(contrato.cd_pessoa_escola);
                if (parametros != null && contrato.titulos != null && contrato.titulos.Count() > 0 && parametros.id_gerar_financeiro_contrato)
                    foreach (Titulo t in contrato.titulos.ToList())
                    {
                        if (t.cd_pessoa_responsavel == 0 && cd_pessoa_responsavel_cad > 0)
                            t.cd_pessoa_responsavel = cd_pessoa_responsavel_cad;
                        if (!t.dh_cadastro_titulo.HasValue)
                            t.dh_cadastro_titulo = DateTime.Now.ToUniversalTime();
                        if (t.cd_pessoa_empresa <= 0)
                            t.cd_pessoa_empresa = contrato.cd_pessoa_escola;
                        if (!t.id_status_titulo.HasValue)
                            t.id_status_titulo = 1;
                        if (!t.id_natureza_titulo.HasValue)
                            t.id_natureza_titulo = 1;
                        t.dt_emissao_titulo = t.dt_emissao_titulo.Date;
                        t.dt_vcto_titulo = t.dt_vcto_titulo.Date;
                        if (t.cd_local_movto <= 0 && parametros.cd_local_movto.HasValue)
                            t.cd_local_movto = parametros.cd_local_movto.Value;
                    }
                // Caso exista titulos para a matrícula, mas o parametro id_gerar_financeiro_contrato não estiver marcado
                //deletar títulos do contrato
                List<Titulo> titulosContext = BusinessFinanceiro.getTitulosByContratoTodosDados(contrato.cd_contrato, contrato.cd_pessoa_escola).ToList();
                if (titulosContext.Count() > 0 && !parametros.id_gerar_financeiro_contrato)
                {
                    foreach (var delTitulo in titulosContext)
                    {
                        if (delTitulo.id_status_cnab != (int)Titulo.StatusCnabTitulo.INICIAL && delTitulo.id_status_cnab != (int)Titulo.StatusCnabTitulo.CONFIRMADO_PEDIDO_BAIXA)
                            throw new FinanceiroBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgNotUpdateTituloEnviadoCNAB, null, FinanceiroBusinessException.TipoErro.ERRO_TITULO_ENVIADO_CNAB, false);
                        else
                            if (delTitulo.id_status_cnab == (int)Titulo.StatusCnabTitulo.INICIAL && delTitulo.id_cnab_contrato)
                                throw new FinanceiroBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgNotUpdateTituloBoleto, null, FinanceiroBusinessException.TipoErro.ERRO_TITULO_EMITIDO_BOLETO, false);
                        if (delTitulo.vl_liquidacao_titulo > 0)
                            throw new FinanceiroBusinessException(string.Format(Utils.Messages.Messages.msgNotExcTituloComBaixa), null, FinanceiroBusinessException.TipoErro.ERRO_TITULO_COM_BAIXA, false);
                        if (delTitulo.cd_plano_conta_tit != null && delTitulo.cd_plano_conta_tit > 0)
                        {
                            PlanoTitulo planoTitulo = BusinessFinanceiro.getPlanoTituloByTitulo(delTitulo.cd_titulo, (int)delTitulo.cd_plano_conta_tit, (int)delTitulo.cd_pessoa_empresa);
                            if (planoTitulo.cd_plano_conta > 0)
                                BusinessFinanceiro.deletePlanoTitulo(planoTitulo);
                        }
                        BusinessFinanceiro.deletarTitulo(delTitulo, contrato.cd_pessoa_escola);

                    }
                }

                if (contrato.AlunoTurma != null)
                {
                    List<AlunoTurma> alunoTurmas = new List<AlunoTurma>();
                    int countPPT = 0;
                    int cdCurso = contrato.cd_curso_atual;
                    foreach (AlunoTurma alunoTurmaPpt in contrato.AlunoTurma)
                    {
                        alunoTurmaPpt.Turma = BusinessTurma.findTurmasByIdAndCdEscola(alunoTurmaPpt.cd_turma, contrato.cd_pessoa_escola);
                        if (alunoTurmaPpt.Turma.id_turma_ppt)
                        {
                            if (alunoTurmaPpt.CursoContrato != null)
                                cdCurso = alunoTurmaPpt.CursoContrato.cd_curso;
                            alunoTurmaPpt.Turma = BusinessCoordenacao.GerarTurmaFilha(contrato, alunoTurmaPpt.cd_turma, cdCurso);
                            alunoTurmaPpt.cd_turma = alunoTurmaPpt.Turma.cd_turma;
                            alunoTurmas.Add(alunoTurmaPpt);
                            countPPT = countPPT + 1;
                        }
                    }

                    if (countPPT > 0)
                    {
                        contrato.AlunoTurma = null;
                        contrato.AlunoTurma = alunoTurmas;
                    }
                }

                contrato = BusinessMatricula.editContrato(contrato, pathContratosEscola, cdUsuario, fusoHorario);
                if (castMatricula)
                {
                    msgProcedureGerarNota = contrato.msgProcedureGerarNota;
                    contrato = BusinessMatricula.getMatriculaByIdVI(contrato.cd_contrato, contrato.cd_pessoa_escola);
                    contrato.DescontoContrato = DataAccessAditamento.getDescontosAplicadosAditamento(contrato.cd_contrato, contrato.cd_pessoa_escola).ToList();
                    contrato.msgProcedureGerarNota = msgProcedureGerarNota;
                }

                if (BusinessApiNewCyber.aplicaApiCyber())
                {
                    //Pega a lista de alunos (ativos ou rematriculados) do contrato após alterar
                    List<AlunoTurma> listaAlunoTurmaCurrent = BusinessAluno.getAlunoTurmaByCdContrato(contrato.cd_contrato); //getAlunoTurmaByCdContrato(contrato.cd_contrato);
                    //Pega os novos alunos (ativos ou rematriculados) adicionados  após alterar
                    //List<AlunoTurma> listaAlunoAlunoTurmaAdd = listaAlunoTurmaCurrent.Where(tc => !listaAlunoTurmaOld.Any(tv => tc.cd_aluno_turma == tv.cd_aluno_turma && tc.cd_aluno == tv.cd_aluno && tc.cd_turma == tv.cd_turma)).ToList();


                    foreach (AlunoTurma alunoTurma in listaAlunoTurmaCurrent)
                    {

                        //monta o objeto alunoCyber para chamar apicyber
                        AlunoApiCyberBdUI alunoCyberCurrent = new AlunoApiCyberBdUI();
                        alunoCyberCurrent = BusinessAluno.findAlunoApiCyber(alunoTurma.cd_aluno, alunoTurma.cd_turma, contrato.cd_contrato);

                        //se aluno ativo ou rematriculado existe e tem o id_unidade e não existe no cyber -> chama a apicyber com comando (CADASTRA_ALUNO)
                        if (alunoCyberCurrent != null && (alunoCyberCurrent.id_unidade != null && alunoCyberCurrent.id_unidade > 0) &&
                            !existeAluno(alunoCyberCurrent.codigo))
                        {
                            cadastraAlunoApiCyber(alunoCyberCurrent, ApiCyberComandosNames.CADASTRA_ALUNO);
                        }



                        //monta o objeto livroAlunoCyber para chamar a apicyber
                        LivroAlunoApiCyberBdUI livroAlunoCyberCurrent = new LivroAlunoApiCyberBdUI();
                        livroAlunoCyberCurrent = BusinessAluno.findLivroAlunoApiCyber(alunoTurma.cd_aluno, alunoTurma.cd_turma, contrato.cd_contrato);

                        //se livro existe no bd e tem o codigo_livro -> (aluno e grupo) existe no cyber -> livroAluno não existe no cyber -> chama a apicyber com comando (CADASTRA_LIVROALUNO)
                        if (livroAlunoCyberCurrent != null && livroAlunoCyberCurrent.codigo_livro > 0 &&
                            existeAluno(livroAlunoCyberCurrent.codigo_aluno) &&
                            existeGrupoByCodigoGrupo(livroAlunoCyberCurrent.codigo_grupo) &&
                            !existeLivroAlunoByCodAluno(livroAlunoCyberCurrent.codigo_aluno, livroAlunoCyberCurrent.codigo_grupo, livroAlunoCyberCurrent.codigo_livro))
                        {
                            cadastraLivroAlunoApiCyber(livroAlunoCyberCurrent, ApiCyberComandosNames.CADASTRA_LIVROALUNO);
                        }
                    }

                    foreach (AlunoTurma alunoTurma in listaAlunoTurmaCurrent)
                    {
                        //Ativa o aluno no cyber caso esteja rematriculado
                        if (existeAluno(alunoTurma.cd_aluno) && alunoTurma.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado)
                        {
                            executaCyberAtivaAluno(alunoTurma.cd_aluno);
                        }
                    }
                }

                transaction.Complete();
            }
            return contrato;
        }

        public DocumentoDigitalizadoEditUI postAtualizarDocumentoDigitalizado(DocumentoDigitalizadoEditUI contrato, string pathContratosEscola)
        {
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED, DaoParametro.DB(), TransactionScopeBuilder.TransactionTime.DEFAULT))
            {
                sincronizarContextos(DaoParametro.DB());

                contrato = BusinessMatricula.editDocumentoDigitalizado(contrato, pathContratosEscola);
                
                contrato = BusinessMatricula.getDocumentoDigitalizadoUpdated(contrato.cd_contrato, contrato.cd_pessoa_escola);
                

                transaction.Complete();
            }
            return contrato;
        }

        public PacoteCertificadoUI postAtualizarPacoteCertificado(PacoteCertificadoUI contrato)
        {
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED, DaoParametro.DB(), TransactionScopeBuilder.TransactionTime.DEFAULT))
            {
                sincronizarContextos(DaoParametro.DB());

                contrato = BusinessMatricula.editPacoteCertificado(contrato);

                transaction.Complete();
            }
            return contrato;
        }

        private int inserirResponsavelMatricula(Contrato contrato)
        {
            int cd_pessoa_responsavel_cad = 0;

            RelacionamentoSGF relac = new RelacionamentoSGF
            {
                cd_pessoa_pai = contrato.cd_pessoa_aluno,
                cd_papel_filho = (int)PapelSGF.TipoPapelSGF.RESPONSAVEL,
                cd_papel_pai = (int)PapelSGF.TipoPapelSGF.ALUNORESPONSAVEL
            };
            if (contrato.pessoaFisicaResponsavel != null)
            {
                relac.PessoaFilho = contrato.pessoaFisicaResponsavel;
                relac.PessoaFilho.nm_natureza_pessoa = (int)PessoaSGF.TipoPessoa.FISICA;
                if (relac.PessoaFilho.PessoaPaiRelacionamento != null && relac.PessoaFilho.PessoaPaiRelacionamento.Count > 0 &&
                    relac.PessoaFilho.PessoaPaiRelacionamento.FirstOrDefault().cd_qualif_relacionamento > 0)
                    relac.cd_qualif_relacionamento = relac.PessoaFilho.PessoaPaiRelacionamento.FirstOrDefault().cd_qualif_relacionamento;
                relac.PessoaFilho.PessoaPaiRelacionamento = new List<RelacionamentoSGF>();
            }
            else
            {
                relac.PessoaFilho = contrato.pessoajuridicaResponsavel;
                relac.PessoaFilho.nm_natureza_pessoa = (int)PessoaSGF.TipoPessoa.JURIDICA;
                if (relac.PessoaFilho.PessoaPaiRelacionamento != null && relac.PessoaFilho.PessoaPaiRelacionamento.Count > 0 &&
                    relac.PessoaFilho.PessoaPaiRelacionamento.FirstOrDefault().cd_qualif_relacionamento > 0)
                    relac.cd_qualif_relacionamento = relac.PessoaFilho.PessoaPaiRelacionamento.FirstOrDefault().cd_qualif_relacionamento;
                relac.PessoaFilho.PessoaPaiRelacionamento = new List<RelacionamentoSGF>();
            }
            cd_pessoa_responsavel_cad = BusinessPessoa.inserirPessoaFromRelacionamento(relac, contrato.cd_aluno).cd_pessoa_filho;
            BusinessPessoa.addRelacionamento(relac);
            BusinessEmpresa.postEmpresaPessoa(new PessoaEscola { cd_escola = contrato.cd_pessoa_escola, cd_pessoa = cd_pessoa_responsavel_cad });
            return cd_pessoa_responsavel_cad;
        }

        public List<Titulo> gerarTitulosGrid(Contrato contrato)
        {
            int cdEscola = 0;
            int cd_origem_titulo = 0;
            bool existeBaixa = false;
            if (contrato.titulos.Count() > 0)
            {
                cdEscola = (int)contrato.titulos.FirstOrDefault().cd_pessoa_empresa;
                cd_origem_titulo = (int)contrato.titulos.FirstOrDefault().cd_origem_titulo;
            }

            if (contrato.titulos.Any(x => x.pc_bolsa > 0 || x.cd_origem_titulo > 0))
                existeBaixa = BusinessFinanceiro.verificaTituloOrContratoBaixaEfetuada(cd_origem_titulo, cdEscola, 0);
            else
                if (cd_origem_titulo > 0)
                    existeBaixa = BusinessFinanceiro.getTituloBaixadoContrato(cd_origem_titulo, cdEscola, 0);
            if (existeBaixa)
                throw new FinanceiroBusinessException(string.Format(Utils.Messages.Messages.msgTituloBaixado), null,
                          FinanceiroBusinessException.TipoErro.ERRO_TITULO_COM_BAIXA, false);
            if (BusinessFinanceiro.verificaTituloEnviadoBoletoOuContrato(cd_origem_titulo, cdEscola, null))
                throw new FinanceiroBusinessException(string.Format(Utils.Messages.Messages.msgNotUpdateTituloBoleto), null,
                         FinanceiroBusinessException.TipoErro.ERRO_TITULO_COM_BAIXA, false);
            if (BusinessFinanceiro.verificaTituloEnviadoCnabOuContrato(cd_origem_titulo, cdEscola, null))
                throw new FinanceiroBusinessException(string.Format(Utils.Messages.Messages.msgErroAlteracaoTituloEnviadoCnab), null,
                         FinanceiroBusinessException.TipoErro.ERRO_TITULO_COM_BAIXA, false);

            Parametro parametro = cdEscola > 0 ? getParametrosMatricula(cdEscola) : new Parametro();
            bool? diaUtil = cdEscola > 0 ? parametro.id_dia_util_vencimento : false;
            byte? nmDia = cdEscola > 0 ? parametro.nm_dia_vencimento : 0;
            bool alterarVencimento = cdEscola > 0 ? parametro.id_alterar_venc_final_semana : false;

            List<Titulo> titulosNovos = BusinessCoordenacao.gerarTitulos(contrato, diaUtil, nmDia, alterarVencimento);
            
            return titulosNovos;
        }

        public List<Titulo> alterarLocalMovtoTitulos(List<Titulo> titulosAlterarLocalMovto, int nm_parcelas_mensalidade, int cd_politica_comercial, int cd_pessoa_empresa)
        {
            if (nm_parcelas_mensalidade == 0 && cd_politica_comercial > 0)
            {
                nm_parcelas_mensalidade = BusinessFinanceiro.getPoliticaComercialById(cd_politica_comercial, cd_pessoa_empresa).nm_parcelas;
                foreach (var titulos in titulosAlterarLocalMovto) { titulos.cd_pessoa_empresa = cd_pessoa_empresa; }
            }
            return BusinessCoordenacao.alterarLocalMovtoTitulos(titulosAlterarLocalMovto, nm_parcelas_mensalidade);
        }

        public List<Titulo> gerarTitulosAditamento(Contrato contrato)
        {
            Titulo titulo = contrato.titulos.FirstOrDefault();
            Parametro parametro = getParametrosMatricula((int)titulo.cd_pessoa_empresa);
            List<Titulo> titulosNovos = BusinessCoordenacao.gerarTitulosAditamento(contrato, parametro);
            return titulosNovos;
        }

        public bool existeAdtAdicionarParcelaBaixado(List<Titulo> titulos, Titulo tituloViewAdt)
        {
            return BusinessCoordenacao.existeAdtAdicionarParcelaBaixado(titulos, tituloViewAdt);
        }

        //método utilizado na Matricula para carregar os dias de pagamento e o valores.
        public OpcoesPagamentoUI getSugestaoDiaOpcoesPgto(int cd_escola, DateTime data_matricula, int? cd_curso, int? cd_duracao, int? cd_regime)
        {
            OpcoesPagamentoUI retorno = new OpcoesPagamentoUI();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                Parametro parametro = DaoParametro.getParametrosOpcaoPagamento(cd_escola);
                retorno = BusinessCoordenacao.calculaSugestaoOpcoesPgto(parametro, data_matricula, cd_escola, cd_curso, cd_duracao, cd_regime);
                transaction.Complete();
            }
            return retorno;
        }

        public void simularBaixaContrato(Contrato contrato, ref BaixaTitulo baixa, int cd_escola)
        {
            Parametro parametro = DaoParametro.getParametrosBaixa(cd_escola);
            BusinessCoordenacao.simularBaixaContrato(contrato, ref baixa, parametro);
        }

        public List<TituloCnab> simularBaixaTituloCnab(List<TituloCnab> titulos, DateTime data_baixa, int cd_escola, bool contaSegura)
        {
            Parametro parametro = DaoParametro.getParametrosBaixa(cd_escola);

            if (titulos != null)
            {
                int nm_baixa = 1;
                foreach (TituloCnab titulo in titulos)
                {
                    bool existe_desconto_dia_vencimento = false;
                    BaixaTitulo baixa = new BaixaTitulo();
                    baixa.dt_baixa_titulo = data_baixa;
                    titulo.Titulo.cd_pessoa_empresa = cd_escola;
                    //titulo.Titulo = BusinessFinanceiro.getTituloBaixaFinanSimulacao(titulo.cd_titulo, cd_escola);
                    BusinessCoordenacao.simularBaixaTitulo(titulo.Titulo, ref baixa, parametro, cd_escola, contaSegura, true);
                    baixa.nm_baixa = nm_baixa;
                    nm_baixa += 1;

                    //Cria a mensagem para emissão no boleto:
                    titulo.tx_mensagem_cnab = "";
                    decimal vl_titulo = titulo.Titulo.vl_saldo_titulo;
                    decimal vl_material_titulo = titulo.Titulo.vl_material_titulo;
                    foreach (DictionaryEntry politica in baixa.sl_politicas)
                    {
                        //TODO: Eduardo - Juntar com o código do CoordenacaoBusiness para unificar e não deixar repetido:
                        //Repete as regras da simulação para cada uma das políticas:
                        decimal percentual_desconto = (decimal)politica.Value;
                        if (percentual_desconto > 100)
                            percentual_desconto = 100;

                        decimal percentual_aplicado = 0;
                        if ((vl_material_titulo > 0 && percentual_desconto > 0))
                        {
                            if ((vl_titulo - vl_material_titulo) < 0)
                                vl_titulo = 0;
                            else
                                vl_titulo -= vl_material_titulo;
                        }
                        if (vl_titulo > 0)
                        {
                            percentual_aplicado = baixa.vl_desconto_baixa_calculado * 100 / vl_titulo;

                            if (parametro.per_desconto_maximo.HasValue && (decimal)parametro.per_desconto_maximo.Value < percentual_aplicado)
                            {
                                percentual_desconto = (decimal)parametro.per_desconto_maximo.Value;
                                percentual_desconto = Decimal.Round(percentual_desconto * vl_titulo / 100, 2);
                            }
                        }
                        if (((DateTime)politica.Key).CompareTo(titulo.Titulo.dt_vcto_titulo) <= 0)
                        {
                            titulo.tx_mensagem_cnab += "Para pagamento até " + String.Format("{0:dd/MM/yyyy}", politica.Key) + ": R$" + string.Format("{0:#,0.00}",
                                (titulo.Titulo.vl_saldo_titulo - Decimal.Round((vl_titulo * percentual_desconto / 100 + baixa.soma_valores_desconto), 2))) + ".\n";
                            titulo.DescontoTituloCNAB.Add(new DescontoTituloCNAB
                            {
                                vl_desconto = Decimal.Round((vl_titulo * (decimal)percentual_desconto / 100), 2),
                                dt_desconto = DateTime.Parse(politica.Key.ToString()).Date
                            });
                            if (((DateTime)politica.Key).CompareTo(titulo.Titulo.dt_vcto_titulo.Date) == 0)
                            {
                                existe_desconto_dia_vencimento = true;
                                break;
                            }
                        }
                        else if (!existe_desconto_dia_vencimento)
                        {
                            if (parametro.id_permitir_desc_apos_politica)
                                titulo.tx_mensagem_cnab += "Para pagamento até " + String.Format("{0:dd/MM/yyyy}", titulo.Titulo.dt_vcto_titulo) + ": R$" + string.Format("{0:#,0.00}",
                                (titulo.Titulo.vl_saldo_titulo - Decimal.Round((vl_titulo * percentual_desconto / 100 + baixa.soma_valores_desconto), 2))) + ".\n";
                            else
                                titulo.tx_mensagem_cnab += "Para pagamento até " + String.Format("{0:dd/MM/yyyy}", titulo.Titulo.dt_vcto_titulo) + ": R$" + string.Format("{0:#,0.00}",
                                    (titulo.Titulo.vl_saldo_titulo )) + ".\n";

                            if (parametro.id_permitir_desc_apos_politica)
                                titulo.DescontoTituloCNAB.Add(new DescontoTituloCNAB
                            {
                                vl_desconto = Decimal.Round((vl_titulo * (decimal)percentual_desconto / 100), 2),
                                dt_desconto = ((DateTime)politica.Key).Date
                            });
                            existe_desconto_dia_vencimento = true;
                            break;
                        }

                        //Retorna valor inicial após calculos.
                        vl_titulo = titulo.Titulo.vl_saldo_titulo;
                    }

                    double percentual_juros = parametro.pc_juros_dia.HasValue ? parametro.pc_juros_dia.Value : 0;
                    double percentual_multa = parametro.pc_multa.HasValue ? parametro.pc_multa.Value : 0;
                    if (titulo.Titulo.pc_multa_titulo > 0 || titulo.Titulo.pc_juros_titulo > 0)
                    {
                        percentual_juros = titulo.Titulo.pc_juros_titulo;
                        percentual_multa = titulo.Titulo.pc_multa_titulo;
                    }
                    titulo.pc_juros_titulo = (double)Decimal.Round((decimal)percentual_juros, 4);
                    titulo.pc_multa_titulo = (double)Decimal.Round((decimal)percentual_multa, 4);
                    

                    if (!existe_desconto_dia_vencimento)
                    {
                        if (parametro.id_permitir_desc_apos_politica || baixa.sl_politicas.Count == 0)
                            titulo.DescontoTituloCNAB.Add(new DescontoTituloCNAB
                        {
                            vl_desconto = Decimal.Round(baixa.vl_desconto_baixa_calculado, 2),
                            dt_desconto = titulo.dt_vencimento_titulo.HasValue ? titulo.dt_vencimento_titulo.Value.Date : new DateTime()
                        });
                        if (parametro.id_permitir_desc_apos_politica || baixa.sl_politicas.Count == 0)
                            titulo.tx_mensagem_cnab += "Para pagamento até " + titulo.Titulo.dt_vcto + ": R$" + string.Format("{0:#,0.00}",
                            titulo.Titulo.vl_saldo_titulo - baixa.vl_desconto_baixa_calculado) + ".\n";
                        else
                            titulo.tx_mensagem_cnab += "Para pagamento até " + titulo.Titulo.dt_vcto + ": R$" + string.Format("{0:#,0.00}",
                            titulo.Titulo.vl_saldo_titulo) + ".\n";
                    }
                    titulo.tx_mensagem_cnab += "Para pagamento após " + titulo.Titulo.dt_vcto + ": juros de R$" + string.Format("{0:#,0.00}", Decimal.Round((titulo.Titulo.vl_saldo_titulo * (decimal)percentual_juros / 100), 2)) + " ao dia e multa de R$" + string.Format("{0:#,0.00}", Decimal.Round((titulo.Titulo.vl_saldo_titulo * (decimal)percentual_multa / 100), 2)) + ".";
                }
            }
            return titulos;
        }

        public List<DescontoTituloCarne> getTituloCarnePorContratoSubReport(int cdTitulo, int cdEscola, bool contaSegura)
        {
            Parametro parametro = this.getParametrosBaixa(cdEscola);
            List<DescontoTituloCarne> listaDescontosCarne = new List<DescontoTituloCarne>();

            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                bool existe_desconto_dia_vencimento = false;
                BaixaTitulo baixa = new BaixaTitulo();
                Titulo titulo = BusinessFinanceiro.getTituloBaixaFinanSimulacao(new List<int>() { cdTitulo }, cdEscola, null, TituloDataAccess.TipoConsultaTituloEnum.HAS_SIMULACAO).FirstOrDefault();
                if (titulo != null && titulo.cd_titulo > 0)
                {
                    DescontoTituloCarne descontoTituloCarne = null;

                    baixa.dt_baixa_titulo = DateTime.Now.Date;
                    BusinessCoordenacao.simularBaixaTitulo(titulo, ref baixa, parametro, cdEscola , contaSegura, true);
                    if (baixa.sl_politicas.Count > 0)
                        foreach (DictionaryEntry politica in baixa.sl_politicas)
                        {
                            //TODO: Eduardo - Juntar com o código do CoordenacaoBusiness para unificar e não deixar repetido:
                            //Repete as regras da simulação para cada uma das políticas:
                            decimal percentual_desconto = (decimal)politica.Value;
                            if (percentual_desconto > 100)
                                percentual_desconto = 100;

                            descontoTituloCarne = new DescontoTituloCarne();
                            decimal vl_titulo = titulo.vl_saldo_titulo;
                            decimal vl_material_titulo = titulo.vl_material_titulo;

                            if ((vl_material_titulo > 0 && percentual_desconto > 0))
                            {
                                if ((vl_titulo - vl_material_titulo) < 0)
                                    vl_titulo = 0;
                                else
                                    vl_titulo -= vl_material_titulo;
                            }

                            if (titulo.dc_tipo_titulo != "TM" && titulo.dc_tipo_titulo != "TA")
                            {
                                //descontoTituloCarne.vl_desconto = titulo.vl_saldo_titulo - titulo.vl_saldo_titulo / 100 * percentual_desconto;
                                descontoTituloCarne.vl_desconto = (titulo.vl_saldo_titulo - (vl_titulo * percentual_desconto / 100) - baixa.soma_valores_desconto);
                            }

                            if ((titulo.vl_saldo_titulo - descontoTituloCarne.vl_desconto) < 0)
                                descontoTituloCarne.vl_desconto = titulo.vl_saldo_titulo;

                            if (((DateTime)politica.Key).CompareTo(titulo.dt_vcto_titulo) <= 0)
                            {
                                descontoTituloCarne.dt_desconto = DateTime.Parse(String.Format("{0:dd/MM/yyyy}", politica.Key));
                                descontoTituloCarne.dc_desconto = "Até \n" + String.Format("{0:dd/MM/yyyy}", politica.Key);
                                listaDescontosCarne.Add(descontoTituloCarne);
                                if (((DateTime)politica.Key).CompareTo(titulo.dt_vcto_titulo.Date) == 0)
                                {
                                    existe_desconto_dia_vencimento = true;
                                    break;
                                }
                            }
                            else if (!existe_desconto_dia_vencimento)
                            {
                                descontoTituloCarne.dt_desconto = DateTime.Parse(String.Format("{0:dd/MM/yyyy}", titulo.dt_vcto_titulo));
                                descontoTituloCarne.dc_desconto = "Até \n" + String.Format("{0:dd/MM/yyyy}", titulo.dt_vcto_titulo);
                                existe_desconto_dia_vencimento = true;

                                listaDescontosCarne.Add(descontoTituloCarne);
                                break;
                            }
                        }
                    if (!existe_desconto_dia_vencimento)
                    {
                        descontoTituloCarne = new DescontoTituloCarne();
                        descontoTituloCarne.vl_desconto = titulo.vl_saldo_titulo - baixa.vl_desconto_baixa_calculado;
                        descontoTituloCarne.dc_desconto = "Até \n" + String.Format("{0:dd/MM/yyyy}", titulo.dt_vcto_titulo);
                        descontoTituloCarne.dt_desconto = titulo.dt_vcto_titulo;

                        listaDescontosCarne.Add(descontoTituloCarne);
                    }
                    descontoTituloCarne = new DescontoTituloCarne();
                    //descontoTituloCarne.vl_desconto = titulo.vl_saldo_titulo - baixa.vl_desconto_baixa_calculado;
                    descontoTituloCarne.dc_desconto = "Após \n" + String.Format("{0:dd/MM/yyyy}", titulo.dt_vcto_titulo);
                    descontoTituloCarne.dt_desconto = titulo.dt_vcto_titulo;

                    listaDescontosCarne.Add(descontoTituloCarne);

                }
                transaction.Complete();
            }
            return listaDescontosCarne;
        }

        public List<BaixaTitulo> simularBaixaTituloLeitura(List<Titulo> titulos, DateTime data_baixa, int cd_escola, bool contaSegura)
        {
            List<BaixaTitulo> listaBaixas = new List<BaixaTitulo>();
            List<Titulo> titulosCheque = new List<Titulo>();
            titulosCheque = titulos.Where(x => x.cd_tipo_financeiro == (int)TipoFinanceiro.TiposFinanceiro.CHEQUE).ToList();
            List<int> cdTitulos = new List<int>();
            cdTitulos = titulosCheque.Select(x => x.cd_titulo).ToList();


            var TROCA_FINANCEIRA = 129;
            List<Titulo> titulosChequeTrocaFinanceira = new List<Titulo>();
            titulosChequeTrocaFinanceira = titulos.Where(t => t.id_origem_titulo == TROCA_FINANCEIRA && t.cd_tipo_financeiro == (int)TipoFinanceiro.TiposFinanceiro.CHEQUE).ToList();
            List<int> cdTitulosTrocaFinanceira = new List<int>();
            cdTitulosTrocaFinanceira = titulosChequeTrocaFinanceira.Select(t => t.cd_titulo).ToList();


            //Aumenta o tempo de pesquisa para 10 min. Tratamento para evitar o erro: System.Data.SqlClient.SqlException: Timeout expired.  The timeout period elapsed prior to completion of the operation or the server is not responding. 
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED, DaoParametro.DB(), TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                if (cdTitulos.Count() > 0)
                {
                    SGFWebContext dbContext = new SGFWebContext();
                    int cd_origem_contrato = Int32.Parse(dbContext.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                    List<Cheque> cheques = BusinessFinanceiro.getChequesByTitulosContrato(cdTitulos, cd_escola);
                    foreach (var t in titulosCheque.Where(x => x.cd_tipo_financeiro == (int)TipoFinanceiro.TiposFinanceiro.CHEQUE).ToList())
                    {
                        if (t.id_origem_titulo == cd_origem_contrato)
                            t.Cheque = cheques.Where(x => x.cd_contrato == t.cd_origem_titulo).FirstOrDefault();
                        else
                            t.Cheque = cheques.Where(x => x.cd_movimento == t.cd_origem_titulo).FirstOrDefault();
                        //if (titulo != null)
                        //    titulo.Cheque = c;
                    }
                }

                if (cdTitulosTrocaFinanceira.Count() > 0)
                {
                    List<Cheque> cheques = BusinessFinanceiro.getChequesByTitulosContrato(cdTitulos, cd_escola);

                    foreach (var t in titulosChequeTrocaFinanceira)
                    {
                        t.Cheque = BusinessFinanceiro.getChequeTransacaoTrocaFinanceira(t.cd_titulo, cd_escola);
                    }
                }

                listaBaixas = simularBaixaTitulo(titulos, data_baixa, cd_escola, contaSegura);
                transaction.Complete();
            }
            return listaBaixas;
        }


        public List<BaixaTitulo> simularBaixaTitulo(List<Titulo> titulos, DateTime data_baixa, int cd_escola, bool contaSegura)
        {
            List<BaixaTitulo> listaBaixas = new List<BaixaTitulo>();
            this.sincronizarContextos(DaoParametro.DB());
            Parametro parametro = DaoParametro.getParametrosBaixa(cd_escola);

            if (titulos != null)
            {
                int nm_baixa = 1;
                foreach (Titulo titulo in titulos)
                {
                    BaixaTitulo baixa = new BaixaTitulo();
                    baixa.dt_baixa_titulo = data_baixa;
                    BusinessCoordenacao.simularBaixaTitulo(titulo, ref baixa, parametro, cd_escola, contaSegura, false);
                    baixa.nm_baixa = nm_baixa;
                    if (titulo.id_natureza_titulo == 1)
                        baixa.obsBaixaTitulo = titulo.natureza + " de " + titulo.nomeResponsavel;
                    else
                        baixa.obsBaixaTitulo = titulo.natureza + " para " + titulo.nomeResponsavel;
                    nm_baixa += 1;
                    listaBaixas.Add(baixa);
                }
            }
            return listaBaixas;
        }

        public List<Titulo> getTituloByPessoa(SearchParameters parametros, int cd_pessoa, int cd_escola, TituloDataAccess.TipoConsultaTituloEnum tipo, bool contaSeg)
        {
            List<Titulo> retorno = new List<Titulo>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                bool ordenaValorTitulo = false;
                Parametro parametro = DaoParametro.getParametrosBaixa(cd_escola);
                DateTime data_baixa = DateTime.UtcNow.Date;

                if ("vlSaldoTitulo".Equals(parametros.sort))
                    ordenaValorTitulo = true;

                List<Titulo> listaTitulos = BusinessFinanceiro.getTituloByPessoa(parametros, cd_pessoa, cd_escola, tipo, contaSeg).ToList();

                for (int i = 0; i < listaTitulos.Count; i++)
                {
                    BaixaTitulo baixa = new BaixaTitulo();

                    baixa.dt_baixa_titulo = data_baixa;
                    listaTitulos[i].cd_pessoa_empresa = cd_escola;
                    BusinessCoordenacao.simularBaixaTitulo(listaTitulos[i], ref baixa, parametro, cd_escola, contaSeg, false);
                    listaTitulos[i].vl_saldo_titulo = baixa.vl_liquidacao_baixa;
                }

                if (ordenaValorTitulo && parametros.sortOrder == SortDirection.Ascending)
                    listaTitulos = listaTitulos.OrderBy(t => t.vl_saldo_titulo).ToList();
                else if (ordenaValorTitulo)
                    listaTitulos = listaTitulos.OrderByDescending(t => t.vl_saldo_titulo).ToList();

                retorno = listaTitulos;
                transaction.Complete();
            }
            return retorno;
        }

        //Este método é utilizado para o retorna da turma e assim populando os campos da view, tiramos todos as requisições da view assincônas.
        public ContratoUI getContratoTurma(int cd_pessoa_escola, int cd_curso, int cd_duracao, int cd_produto, int cd_regime, DateTime dta_matricula)
        {
            ContratoUI contratoUI;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                Parametro parametro = new Parametro();
                parametro = DaoParametro.getParametrosMatricula(cd_pessoa_escola);
                Valores valores = BusinessFinanceiro.getValoresForMatricula(cd_pessoa_escola, cd_curso, cd_duracao, cd_regime, dta_matricula);
                List<Curso> cursos = BusinessCurso.getCursos(CursoDataAccess.TipoConsultaCursoEnum.HAS_ATIVOPROD, null, cd_produto, null).ToList();
                List<Banco> bancos = BusinessFinanceiro.getAllBanco().ToList();
                List<LocalMovto> localMovto = BusinessFinanceiro.getLocalMovtoByEscola(cd_pessoa_escola, 0, true).ToList();
                OpcoesPagamentoUI opcoesPagamento = opcoesPagamento = BusinessCoordenacao.calculaSugestaoOpcoesPgto(parametro, dta_matricula, cd_pessoa_escola, cd_curso, cd_duracao, cd_regime);
                List<Duracao> duracoes = BusinessCoordenacao.getDuracoes(DuracaoDataAccess.TipoConsultaDuracaoEnum.HAS_ATIVO, cd_duracao, null).ToList();
                List<Produto> produtos = BusinessCoordenacao.findProduto(ProdutoDataAccess.TipoConsultaProdutoEnum.HAS_ATIVO, cd_produto, null).ToList();
                List<Regime> regimes = BusinessCoordenacao.getRegimes(RegimeDataAccess.TipoConsultaRegimeEnum.HAS_ATIVO, cd_regime).ToList();
                List<NomeContrato> nomesContrato = BusinessSecretaria.getNomesContrato(NomeContratoDataAccess.TipoConsultaNomeContratoEnum.HAS_ATIVO_MATRICULA, null, cd_pessoa_escola).ToList();

                contratoUI = new ContratoUI
                {
                    valores = valores,
                    opcoesPagamento = opcoesPagamento,
                    duracoes = duracoes,
                    produtos = produtos,
                    cursos = cursos,
                    regimes = regimes,
                    nomesContrato = nomesContrato,
                    bancos = bancos,
                    parametro = parametro,
                    localMovto = localMovto
                };
                transaction.Complete();
            }
            return contratoUI;
        }

        #endregion

        #region Desistência

        public DesistenciaUI baixarTitulosInserirDesistencia(DesistenciaUI desistenciaUI, List<Titulo> list, DateTime data_baixa, int cd_escola, int cd_liquidacao, int cd_local_movto, bool contaSeg)
        {
            DesistenciaUI desistencia = new DesistenciaUI();
            int qtd_diario = 0;
            int qtd_faltas = 0;
            IEnumerable<AlunoEvento> alunoEventos = new List<AlunoEvento>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                //Leonardo Silva ->27/06/2022 chamado: 337032 -> "Deixar travado como estava se existir alguma avaliação com nota lançada, verificar se nm_nota_aluno e nm_nota_aluno_2 são nulas, para todos os alunos), aí sim pode liberar"
                validarDesistencia(desistenciaUI, cd_escola);

                LivroAlunoApiCyberBdUI livroAlunoTurmaApiCyber = null;

                if (BusinessApiNewCyber.aplicaApiCyber())
                {
                     livroAlunoTurmaApiCyber = BusinessAluno.findLivroAlunoTurmaApiCyber(desistenciaUI.cd_aluno, desistenciaUI.cd_turma, cd_escola);
                }

                if (desistenciaUI.id_tipo_desistencia == (int)DesistenciaDataAccess.tipoDesistencia.DESISTENCIA)
                {
                    qtd_diario = BusinessCoordenacao.returnDiarioByDataDesistencia(desistenciaUI.dt_desistencia, cd_escola, desistenciaUI.cd_aluno, desistenciaUI.cd_turma, (int)DesistenciaDataAccess.tipoDesistencia.DESISTENCIA);
                    alunoEventos = BusinessCoordenacao.getEventosAlunoByDataDesistencia(desistenciaUI.cd_aluno, desistenciaUI.cd_turma, cd_escola, desistenciaUI.dt_desistencia);
                    qtd_faltas = alunoEventos.Count();
                }


                //TODO: Verificar essa regra com o PO. Deleta os evendo posterior a data de desistência do aluno
                if (alunoEventos != null && alunoEventos.Count() > 0)
                    BusinessCoordenacao.deleteAllAlunoEvento(alunoEventos.ToList());

                desistencia = BusinessSecretaria.addDesistencia(desistenciaUI, cd_escola, qtd_diario, qtd_faltas, false);

                if (desistenciaUI.id_tipo_desistencia == (int)DesistenciaDataAccess.tipoDesistencia.CANCELAMENTO)
                {
                    qtd_diario = BusinessCoordenacao.returnDiarioByDataDesistencia(desistenciaUI.dt_desistencia, cd_escola, desistenciaUI.cd_aluno, desistenciaUI.cd_turma, (int)DesistenciaDataAccess.tipoDesistencia.CANCELAMENTO);
                    BusinessSecretaria.alterarAlunoTurma(desistencia, cd_escola, qtd_diario, qtd_faltas);
                }
                if (list != null && list.ToList().Count() > 0)
                    simularTitulosIncluirTransacao(desistenciaUI.cd_usuario, desistenciaUI.chequeTransacao, list, data_baixa, cd_escola, cd_liquidacao, cd_local_movto, contaSeg);
                //Opções de encerramento
                if (BusinessTurma.verificaSeTurmaEFilhaPersonalizada(desistenciaUI.cd_turma, cd_escola))
                {
                    switch (desistenciaUI.id_tipo_desistencia)
                    {
                        case (byte)DesistenciaDataAccess.tipoDesistencia.DESISTENCIA:
                            List<Turma> turmas = new List<Turma>();
                            turmas.Add(new Turma { cd_turma = desistenciaUI.cd_turma, dt_termino_turma = desistencia.dt_desistencia.Date });
                            BusinessTurma.editTurmaEncerramento(turmas, desistenciaUI.cd_usuario, desistenciaUI.fuso);
                            break;
                        case (byte)DesistenciaDataAccess.tipoDesistencia.CANCELAMENTO:
                            TurmaSearch turmaEncerrada = BusinessTurma.postCancelarEncerramento(new Turma { cd_turma = desistenciaUI.cd_turma }, cd_escola, desistenciaUI.cd_usuario);
                            break;
                    }
                }

                if (BusinessApiNewCyber.aplicaApiCyber())
                {
                    if (desistenciaUI.id_tipo_desistencia == (int)DesistenciaDataAccess.tipoDesistencia.DESISTENCIA)
                    {

                        if (livroAlunoTurmaApiCyber != null && existeLivroAlunoByCodAluno(livroAlunoTurmaApiCyber.codigo_aluno, livroAlunoTurmaApiCyber.codigo_grupo, livroAlunoTurmaApiCyber.codigo_livro))
                        {
                            //chama a api cyber com o comanco (DELETA_LIVRO_ALUNO)
                            deletaLivroAlunoApiCyber(livroAlunoTurmaApiCyber, ApiCyberComandosNames.DELETA_LIVROALUNO);
                        }

                    }
                }

                transaction.Complete();
            }
            return desistencia;
        }

        public bool existeLivroAlunoByCodAluno(int codigo_aluno, int codigo_grupo, int codigo_livro)
        {
            return BusinessApiNewCyber.verificaRegistroLivroAlunos(ConfigurationManager.AppSettings["enderecoApiNewCyber"],
                ApiCyberComandosNames.VISUALIZA_LIVROALUNO, ConfigurationManager.AppSettings["chaveApiNewCyber"], codigo_aluno, codigo_grupo, codigo_livro);
        }

        private void deletaLivroAlunoApiCyber(LivroAlunoApiCyberBdUI livroAlunoCyberCurrent, string comando)
        {

            string parametros = "";

            //valida e retorna os parametros para a requisicao cyber
            parametros = validaParametrosCyberDeletaLivroAluno(livroAlunoCyberCurrent, ConfigurationManager.AppSettings["enderecoApiNewCyber"], comando, "");

            string result = BusinessApiNewCyber.postExecutaCyber(ConfigurationManager.AppSettings["enderecoApiNewCyber"],
                comando, ConfigurationManager.AppSettings["chaveApiNewCyber"], parametros);

        }

        private string validaParametrosCyberDeletaLivroAluno(LivroAlunoApiCyberBdUI entity, string url, string comando, string parametros)
        {


            if (entity == null)
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberParametrosNulos, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_PARAMETROS_NULOS, false);
            }


            if (entity.codigo_aluno <= 0)
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberCdAlunoMenorIgualZero, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_COD_ALUNO_MENOR_IGUAL_ZERO, false);
            }

            if (entity.codigo_grupo <= 0)
            {
                throw new ApiNewCyberException(string.Format(Utils.Messages.Messages.ErroApiCyberCodigoGrupoMenorIgualZero, url, comando, parametros), null, ApiNewCyberException.TipoErro.ERRO_CODIGO_GRUPO_MENOR_IGUAL_ZERO, false);
            }


            string listaParams = "";
            listaParams = string.Format("codigo_aluno={0},codigo_grupo={1}", entity.codigo_aluno, entity.codigo_grupo);
            return listaParams;
        }

        public DesistenciaUI baixarTitulosEditarDesistencia(DesistenciaUI desistenciaUI, List<Titulo> list, DateTime data_baixa, int cd_escola, int cd_liquidacao, int cd_local_movto, bool contaSeg)
        {
            DesistenciaUI desistencia = new DesistenciaUI();
            int qtd_diario = 0;

            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                BusinessSecretaria.sincronizarContextos(DaoParametro.DB());
                BusinessFinanceiro.sincronizarContextos(DaoParametro.DB());
                BusinessCoordenacao.sincronizarContextos(DaoParametro.DB());
                LivroAlunoApiCyberBdUI livroAlunoTurmaApiCyber = null;
                if (BusinessApiNewCyber.aplicaApiCyber())
                {
                    livroAlunoTurmaApiCyber = BusinessAluno.findLivroAlunoTurmaApiCyber(desistenciaUI.cd_aluno, desistenciaUI.cd_turma, cd_escola);
                }

                //Leonardo Silva ->24/06/2022 chamado: 337032 -> "Deixar travado como estava se existir alguma avaliação com nota lançada, verificar se nm_nota_aluno e nm_nota_aluno_2 são nulas, para todos os alunos), aí sim pode liberar"
                validarDesistencia(desistenciaUI, cd_escola);
                if (desistenciaUI.id_tipo_desistencia == (int)DesistenciaDataAccess.tipoDesistencia.DESISTENCIA)
                    qtd_diario = BusinessCoordenacao.returnDiarioByDataDesistencia(desistenciaUI.dt_desistencia, cd_escola, desistenciaUI.cd_aluno, desistenciaUI.cd_turma, (int)DesistenciaDataAccess.tipoDesistencia.DESISTENCIA);

                if (desistenciaUI.id_tipo_desistencia == (int)DesistenciaDataAccess.tipoDesistencia.CANCELAMENTO)
                    qtd_diario = BusinessCoordenacao.returnDiarioByDataDesistencia(desistenciaUI.dt_desistencia, cd_escola, desistenciaUI.cd_aluno, desistenciaUI.cd_turma, (int)DesistenciaDataAccess.tipoDesistencia.CANCELAMENTO);

                desistencia = BusinessSecretaria.editDesistencia(desistenciaUI, cd_escola, qtd_diario, false);
                if (list != null && list.ToList().Count() > 0)
                    simularTitulosIncluirTransacao(desistenciaUI.cd_usuario, desistenciaUI.chequeTransacao, list, data_baixa, cd_escola, cd_liquidacao, cd_local_movto, contaSeg);

                if (BusinessApiNewCyber.aplicaApiCyber())
                {
                    if (desistencia.id_tipo_desistencia == (int)DesistenciaDataAccess.tipoDesistencia.DESISTENCIA)
                    {

                        if (livroAlunoTurmaApiCyber != null && existeLivroAlunoByCodAluno(livroAlunoTurmaApiCyber.codigo_aluno, livroAlunoTurmaApiCyber.codigo_grupo, livroAlunoTurmaApiCyber.codigo_livro))
                        {
                            //chama a api cyber com o comanco (DELETA_LIVRO_ALUNO)
                            deletaLivroAlunoApiCyber(livroAlunoTurmaApiCyber, ApiCyberComandosNames.DELETA_LIVROALUNO);
                        }

                    }
                }
                 
                transaction.Complete();
            }
            return desistencia;
        }

        private void validarDesistencia(DesistenciaUI desistenciaUI, int cd_escola)
        {
            bool existsAvaliacao = BusinessTurma.existsAvaliacaoTurmaByDesistencia(desistenciaUI.dt_desistencia.ToLocalTime().Date, cd_escola, desistenciaUI.cd_aluno, desistenciaUI.cd_turma);
            if (existsAvaliacao)
                throw new EscolaBusinessException(Utils.Messages.Messages.msgExisteAvaliacaoAlunoData, null, EscolaBusinessException.TipoErro.ERRO_EXISTE_AVALICAO, false);
        }

        public MudancasInternas postMudancaTurma(MudancasInternas mudanca, bool contaSeg)
        {
            var data_baixa = DateTime.Now.Date;
            if (mudanca.titulos != null)
                simularTitulosIncluirTransacao(mudanca.cd_usuario, mudanca.chequeTransacao, mudanca.titulos.ToList(), data_baixa, mudanca.cd_escola, mudanca.cd_tipo_liquidacao, mudanca.cd_local_movto, contaSeg);

            mudanca.chequeTransacao = null;
            var mudancaRetorno = BusinessCoordenacao.postMudancaTurma(mudanca);

            return mudancaRetorno;
        }

        private void simularTitulosIncluirTransacao(int cd_usuario, ChequeTransacao chequeTransacao, List<Titulo> list, DateTime data_baixa, int cd_escola, int cd_liquidacao, int cd_local_movto, bool contaSeg)
        {
            List<BaixaTitulo> baixas = simularBaixaTitulo(list, data_baixa, cd_escola, contaSeg);
            TransacaoFinanceira transacao = new TransacaoFinanceira();
            if (baixas != null && baixas.ToList().Count() > 0)
            {

                transacao.dt_tran_finan = data_baixa.ToLocalTime();
                transacao.cd_tipo_liquidacao = cd_liquidacao;
                transacao.cd_local_movto = cd_local_movto;
                transacao.cd_pessoa_empresa = cd_escola;
                if (transacao.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.CANCELAMENTO)
                    transacao.cd_local_movto = getLocalMovto(transacao.cd_pessoa_empresa);

                if (transacao.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.CHEQUE_PRE_DATADO || transacao.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.CHEQUE_AVISTA)
                {
                    if (chequeTransacao != null)
                    {
                        if (!Cheque.validarDadosCheque(chequeTransacao.Cheque, false))
                            throw new FinanceiroBusinessException(string.Format(Utils.Messages.Messages.msgErroNExisteAlteracaoCheque), null, FinanceiroBusinessException.TipoErro.ERRO_DADOS_CHEQUE, false);
                        transacao.ChequeTransacaoFinanceira.Add(new ChequeTransacao
                        {
                            dt_bom_para = chequeTransacao.dt_bom_para.Date,
                            nm_cheque = chequeTransacao.nm_cheque,
                            Cheque = chequeTransacao.Cheque
                        });
                    }
                    else
                    {

                        foreach (BaixaTitulo b in baixas)
                        {
                            if (!b.Titulo.Cheque.dt_bom_para.HasValue && b.Titulo.dt_vcto_titulo != null)
                                b.Titulo.Cheque.dt_bom_para = b.Titulo.dt_vcto_titulo;

                            if (!Cheque.validarDadosCheque(b.Titulo.Cheque, true))
                                throw new FinanceiroBusinessException(string.Format(Utils.Messages.Messages.msgErroNExisteAlteracaoCheque),
                                    null, FinanceiroBusinessException.TipoErro.ERRO_DADOS_CHEQUE, false);

                            b.ChequeBaixa.Add(new ChequeBaixa
                            {
                                dt_bom_para = b.Titulo.Cheque.dt_bom_para.Value.Date,
                                nm_cheque = b.Titulo.Cheque.nm_primeiro_cheque,
                                cd_cheque = b.Titulo.Cheque.cd_cheque
                            });
                            b.Titulo.Cheque = null;
                        }
                    }
                }
                foreach (var bt in baixas)
                {
                    bt.cd_usuario = cd_usuario;
                }

                transacao.Baixas = baixas;
                BusinessFinanceiro.postIncluirTransacao(transacao, false);
            }
        }

        // ANTIGO CANCELAR ENCERRAMENTO - ANTES DA PROCEDURE sp_cancelar_rematricula 
        // É DESISTENCIA, MAS DESISTENCIA CHAMA O CANCELCAR ENCERRAMENTO ANTIGO....
        // COM A PROCEDURE sp_cancelar_rematricula  NAO PASSA AQUI, VOU DEIXAR CASO OUTRO LUGAR CHAME
        public bool deletarDesistencia(List<DesistenciaUI> desistencias, int cd_escola, int cd_usuario)
        {
            bool deleted = false;
            int qtd_diario = 0;
            DateTime? maiorDataDesistencia = null;

            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                BusinessSecretaria.sincronizarContextos(DaoParametro.DB());
                BusinessCoordenacao.sincronizarContextos(DaoParametro.DB());

                for (int i = 0; i < desistencias.Count(); i++)
                {
                    bool turmaFilhaPersonalizada = BusinessTurma.verificaSeTurmaEFilhaPersonalizada(desistencias[i].cd_turma, cd_escola);
                    maiorDataDesistencia = BusinessSecretaria.getMaiorDataDesistencia(desistencias[i], cd_escola);
                    int qtd_desistenciaBD = BusinessSecretaria.retornaQuantidadeDesistencia(desistencias[i].cd_turma, desistencias[i].cd_aluno, cd_escola, desistencias[i].cd_aluno_turma);

                    //Ordena a lista para pegar a desistência com o maior código.
                    desistencias.Sort((d1, d2) => d2.cd_desistencia.CompareTo(d1.cd_desistencia));
                    if (qtd_desistenciaBD == 1)
                    {
                        Desistencia desistenciaDelete = BusinessSecretaria.findByIdDesistencia(desistencias[i].cd_desistencia);
                        HistoricoAluno historicoDeletar = BusinessSecretaria.GetHistoricoAlunoPorDesistencia(desistencias[i].cd_desistencia);
                        HistoricoAluno historicoDeletarCopy = new HistoricoAluno();
                        historicoDeletarCopy.copy(historicoDeletar);
                        if (historicoDeletar != null)
                            BusinessSecretaria.deleteHistoricoAluno(historicoDeletar);
                        deleted = BusinessSecretaria.deleteDesistencia(desistenciaDelete);

                        if (desistencias[i].id_tipo_desistencia == (int)DesistenciaDataAccess.tipoDesistencia.DESISTENCIA)
                        {
                            qtd_diario = BusinessCoordenacao.returnDiarioByDataDesistencia(maiorDataDesistencia, cd_escola, desistencias[i].cd_aluno, desistencias[i].cd_turma, (int)DesistenciaDataAccess.tipoDesistencia.CANCELAMENTO);
                            if (turmaFilhaPersonalizada)
                                BusinessTurma.postCancelarEncerramento(new Turma { cd_turma = desistencias[i].cd_turma }, cd_escola, cd_usuario);
                        }

                        if (desistencias[i].id_tipo_desistencia == (int)DesistenciaDataAccess.tipoDesistencia.CANCELAMENTO)
                        {
                            qtd_diario = BusinessCoordenacao.returnDiarioByDataDesistencia(maiorDataDesistencia, cd_escola, desistencias[i].cd_aluno, desistencias[i].cd_turma, (int)DesistenciaDataAccess.tipoDesistencia.DESISTENCIA);
                            if (turmaFilhaPersonalizada)
                            {
                                DateTime dataEncerramento = desistencias[i].dt_desistencia.Date;
                                DateTime? dataHistoricoCancelamento = BusinessSecretaria.buscarDataHistoricoDesistenciaAlteriorCancelamento(desistencias[i].cd_aluno, desistencias[i].cd_turma, historicoDeletarCopy.dt_historico, historicoDeletarCopy.nm_sequencia);
                                if (dataHistoricoCancelamento.HasValue)
                                    dataEncerramento = dataHistoricoCancelamento.Value.Date;
                                List<Turma> turmas = new List<Turma>();
                                turmas.Add(new Turma { cd_turma = desistencias[i].cd_turma, dt_termino_turma = dataEncerramento });
                                BusinessTurma.editTurmaEncerramento(turmas, cd_usuario, desistencias[i].fuso);
                            }
                        }

                        //altera a situação do aluno na turma 
                        int[] alunos = { desistencias[i].cd_aluno };
                        AlunoTurma alunoTurma = BusinessAluno.findAlunosTurma(desistencias[i].cd_turma, cd_escola, alunos).FirstOrDefault();
                        alunoTurma.cd_situacao_aluno_turma = alunoTurma.cd_situacao_aluno_origem;//(int)AlunoTurma.SituacaoAlunoTurma.Ativo;
                        //Alunos aguardando matrícula não tem situação anterior.
                        if (alunoTurma.cd_situacao_aluno_turma == null)
                            alunoTurma.cd_situacao_aluno_turma = (int)AlunoTurma.SituacaoAlunoTurma.Aguardando;
                        alunoTurma.nm_aulas_dadas = qtd_diario;
                        BusinessAluno.editAlunoTurma(alunoTurma);
                    }
                    else
                    {
                        Desistencia desistenciaDeletar = BusinessSecretaria.findByIdDesistencia(desistencias[i].cd_desistencia);
                        Desistencia ultimaDesistencia = BusinessSecretaria.retornaDesistenciaMax(desistencias[i], cd_escola);
                        HistoricoAluno historicoDeletar = BusinessSecretaria.GetHistoricoAlunoPorDesistencia(desistencias[i].cd_desistencia);

                        if (historicoDeletar != null)
                            BusinessSecretaria.deleteHistoricoAluno(historicoDeletar);

                        if ((ultimaDesistencia != null) && (ultimaDesistencia.cd_desistencia == desistencias[i].cd_desistencia))
                        {
                            deleted = BusinessSecretaria.deleteDesistencia(desistenciaDeletar);
                        }
                        else
                            throw new SecretariaBusinessException(Utils.Messages.Messages.msgExistDesistenciaPosterior + " ", null, SecretariaBusinessException.TipoErro.ERRO_DESISTENCIA_POSTERIOR, false);


                        if (desistencias[i].id_tipo_desistencia == (int)DesistenciaDataAccess.tipoDesistencia.DESISTENCIA)
                            qtd_diario = BusinessCoordenacao.returnDiarioByDataDesistencia(maiorDataDesistencia, cd_escola, desistencias[i].cd_aluno, desistencias[i].cd_turma, (int)DesistenciaDataAccess.tipoDesistencia.CANCELAMENTO);

                        if (desistencias[i].id_tipo_desistencia == (int)DesistenciaDataAccess.tipoDesistencia.CANCELAMENTO)
                            qtd_diario = BusinessCoordenacao.returnDiarioByDataDesistencia(maiorDataDesistencia, cd_escola, desistencias[i].cd_aluno, desistencias[i].cd_turma, (int)DesistenciaDataAccess.tipoDesistencia.DESISTENCIA);

                        int[] alunos = { desistencias[i].cd_aluno };
                        AlunoTurma alunoTurma = BusinessAluno.findAlunosTurma(desistencias[i].cd_turma, cd_escola, alunos).FirstOrDefault();

                        byte situacaoAlunoTurma = (byte)alunoTurma.cd_situacao_aluno_origem;

                        if (desistenciaDeletar.id_tipo_desistencia == (int)DesistenciaDataAccess.tipoDesistencia.CANCELAMENTO)
                            situacaoAlunoTurma = (byte)AlunoTurma.SituacaoAlunoTurma.Desistente;
                        //if (desistenciaDeletar.id_tipo_desistencia == (int)DesistenciaDataAccess.tipoDesistencia.CANCELAMENTONAOREMATRICULA)
                        //    situacaoAlunoTurma = (byte)AlunoTurma.SituacaoAlunoTurma.NaoRematriculado;


                        alunoTurma.cd_situacao_aluno_turma = situacaoAlunoTurma;

                        //alunoTurma.cd_situacao_aluno_origem = (byte)alunoTurma.cd_situacao_aluno_origem;

                        alunoTurma.nm_aulas_dadas = qtd_diario;
                        BusinessAluno.editAlunoTurma(alunoTurma);

                        if (desistencias[i].id_tipo_desistencia == (int)DesistenciaDataAccess.tipoDesistencia.DESISTENCIA)
                            if (turmaFilhaPersonalizada)
                                BusinessTurma.postCancelarEncerramento(new Turma { cd_turma = desistencias[i].cd_turma }, cd_escola, cd_usuario);

                        if (desistencias[i].id_tipo_desistencia == (int)DesistenciaDataAccess.tipoDesistencia.CANCELAMENTO)
                            if (turmaFilhaPersonalizada)
                            {
                                DateTime dataEncerramento = desistencias[i].dt_desistencia.Date;
                                DateTime? dataHistoricoCancelamento = BusinessSecretaria.buscarDataHistoricoDesistenciaAlteriorCancelamento(desistencias[i].cd_aluno, desistencias[i].cd_turma, ultimaDesistencia.dt_desistencia, historicoDeletar.nm_sequencia);
                                if (dataHistoricoCancelamento.HasValue)
                                    dataEncerramento = dataHistoricoCancelamento.Value.Date;
                                List<Turma> turmas = new List<Turma>();
                                turmas.Add(new Turma { cd_turma = desistencias[i].cd_turma, dt_termino_turma = dataEncerramento });
                                BusinessTurma.editTurmaEncerramento(turmas, cd_usuario, desistencias[i].fuso);
                            }
                    }

                }
                transaction.Complete();
            }
            return deleted;
        }

        #endregion

        #region Biblioteca

        public Emprestimo getEmprestimo(int cd_biblioteca, int cd_escola)
        {
            //Busca os parâmetros da escola de número de dias e taxa de multa:
            Parametro parametro = DaoParametro.getParametrosBiblioteca(cd_escola);

            return BusinessBiblioteca.getEmprestimo(parametro, cd_biblioteca, cd_escola);
        }

        public Emprestimo addEmprestimo(Emprestimo emprestimo, int cd_escola)
        {
            Emprestimo emp = new Emprestimo();
            //BusinessFinanceiro.sincronizarContextos(DaoParametro.DB());
            //BusinessBiblioteca.sincronizarContextos(DaoParametro.DB());
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {

                int saldo = BusinessFinanceiro.getSaldoItem(emprestimo.cd_item, emprestimo.dt_emprestimo, cd_escola);

                //Verificar se existe fechamento anterior a essa data
                //Por data de Emprestimo
                Fechamento fechamentoEmp = BusinessFinanceiro.getFechamentoByDta(emprestimo.dt_emprestimo, cd_escola);
                //Por data de devolução
                Fechamento fechamentoDev = new Fechamento();
                if (emprestimo != null && emprestimo.dt_devolucao != null)
                    fechamentoDev = BusinessFinanceiro.getFechamentoByDta(emprestimo.dt_devolucao.Value, cd_escola);
                //Se existir fechamento pela data de emprestimo ou pela data de devolução verifica se algum dos fechamentos são por balanço OU 
                // se o parametro de bloquear movimento retroativo está marcado
                if ((fechamentoEmp != null && fechamentoEmp.cd_fechamento > 0) || (fechamentoDev != null && fechamentoDev.cd_fechamento > 0))
                {
                    if (fechamentoEmp.id_balanco || (fechamentoDev != null && fechamentoDev.id_balanco))
                        throw new FinanceiroBusinessException(string.Format(Utils.Messages.Messages.msgErroInclBibliFechBalanco), null, FinanceiroBusinessException.TipoErro.ERRO_EXISTE_FECHAMENTO_BALANCO, false);
                    //Verifica parâmetro 
                    bool bloqueado = DaoParametro.getParametroBloquearMovtoRetroativoEst(cd_escola);
                    if (bloqueado)
                        throw new FinanceiroBusinessException(string.Format(Utils.Messages.Messages.msgErroInclBibliFech), null, FinanceiroBusinessException.TipoErro.ERRO_EXISTE_FECHAMENTO, false);
                }


                BusinessEmpresa.postEmpresaPessoaBiblioteca(new PessoaEscola
                {
                    cd_escola = cd_escola,
                    cd_pessoa = emprestimo.cd_pessoa
                });
                
                emp = BusinessBiblioteca.addEmprestimo(emprestimo, cd_escola, saldo);
                transaction.Complete();
            }
            return emp;
        }

        public Emprestimo postEditEmprestimo(Emprestimo emprestimo, int cd_escola)
        {
            Emprestimo emp = new Emprestimo();
            this.sincronizarContextos(DaoParametro.DB());
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                SGFWebContext db = new SGFWebContext();
                bool existeTaxa = BusinessFiscal.existeMovimentoByOrigem(emprestimo.cd_biblioteca, Int32.Parse(db.LISTA_ORIGEM_LOGS["Emprestimo"].ToString()));
                if (existeTaxa)
                    throw new FinanceiroBusinessException(string.Format(Utils.Messages.Messages.msgErroAlterarExcluirBiblioComTaxa, "alterar"), null, FinanceiroBusinessException.TipoErro.ERRO_EXISTE_TAXA, false);
                //Verificar qual das datas está sendo modificada
                Emprestimo bibliotecaDB = BusinessBiblioteca.getEmprestimoById(emprestimo.cd_biblioteca);
                //Se tiver alterando para colocar data de devolução, deve verificar apenas a data de devolução
                if (bibliotecaDB.dt_devolucao == null && emprestimo.dt_devolucao != null)
                {
                    //Por data de devolução
                    Fechamento fechamentoDev = BusinessFinanceiro.getFechamentoByDta(emprestimo.dt_devolucao.Value, cd_escola);
                    if (fechamentoDev != null && fechamentoDev.cd_fechamento > 0)
                    {
                        if (fechamentoDev.id_balanco)
                            throw new FinanceiroBusinessException(string.Format(Utils.Messages.Messages.msgErroUpdateBibliFechBalanco), null, FinanceiroBusinessException.TipoErro.ERRO_EXISTE_FECHAMENTO_BALANCO, false);
                        //Verifica parâmetro 
                        bool bloqueado = DaoParametro.getParametroBloquearMovtoRetroativoEst(cd_escola);
                        if (bloqueado)
                            throw new FinanceiroBusinessException(string.Format(Utils.Messages.Messages.msgErroUpdateBibliFech), null, FinanceiroBusinessException.TipoErro.ERRO_EXISTE_FECHAMENTO, false);
                    }
                }
                else
                {
                    //Verificar se existe fechamento anterior a essa data
                    //Por data de Emprestimo
                    Fechamento fechamentoEmp = BusinessFinanceiro.getFechamentoByDta(emprestimo.dt_emprestimo, cd_escola);
                    //Por data de devolução
                    Fechamento fechamentoDev = new Fechamento();
                    if (emprestimo != null && emprestimo.dt_devolucao != null)
                        fechamentoDev = BusinessFinanceiro.getFechamentoByDta(emprestimo.dt_devolucao.Value, cd_escola);
                    //Se existir fechamento pela data de emprestimo ou pela data de devolução verifica se algum dos fechamentos são por balanço OU 
                    // se o parametro de bloquear movimento retroativo está marcado
                    if ((fechamentoEmp != null && fechamentoEmp.cd_fechamento > 0) || (fechamentoDev != null && fechamentoDev.cd_fechamento > 0))
                    {
                        if (fechamentoEmp.id_balanco || (fechamentoDev != null && fechamentoDev.id_balanco))
                            throw new FinanceiroBusinessException(string.Format(Utils.Messages.Messages.msgErroInclBibliFechBalanco), null, FinanceiroBusinessException.TipoErro.ERRO_EXISTE_FECHAMENTO_BALANCO, false);
                        //Verifica parâmetro 
                        bool bloqueado = DaoParametro.getParametroBloquearMovtoRetroativoEst(cd_escola);
                        if (bloqueado)
                            throw new FinanceiroBusinessException(string.Format(Utils.Messages.Messages.msgErroInclBibliFech), null, FinanceiroBusinessException.TipoErro.ERRO_EXISTE_FECHAMENTO, false);
                    }
                }
                emp = BusinessBiblioteca.postEditEmprestimo(emprestimo, cd_escola);
                BusinessEmpresa.postEmpresaPessoa(new PessoaEscola
                {
                    cd_escola = cd_escola,
                    cd_pessoa = emprestimo.cd_pessoa
                });
                transaction.Complete();
            }
            return emp;
        }

        public bool deleteEmprestimos(List<Emprestimo> emprestimos, int cd_escola)
        {
            bool movto = false;
            BusinessFinanceiro.sincronizarContextos(DaoParametro.DB());
            BusinessBiblioteca.sincronizarContextos(DaoParametro.DB());
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                foreach (Emprestimo emp in emprestimos)
                {
                    Emprestimo emprestimo = BusinessBiblioteca.getEmprestimoById(emp.cd_biblioteca);
                    SGFWebContext db = new SGFWebContext();
                    bool existeTaxa = BusinessFiscal.existeMovimentoByOrigem(emprestimo.cd_biblioteca, Int32.Parse(db.LISTA_ORIGEM_LOGS["Emprestimo"].ToString()));
                    if (existeTaxa)
                        throw new FinanceiroBusinessException(string.Format(Utils.Messages.Messages.msgErroAlterarExcluirBiblioComTaxa, "alterar"), null, FinanceiroBusinessException.TipoErro.ERRO_EXISTE_TAXA, false);
                    //Verificar se existe fechamento anterior a essa data
                    //Por data de Emprestimo
                    Fechamento fechamentoEmp = BusinessFinanceiro.getFechamentoByDta(emprestimo.dt_emprestimo, cd_escola);
                    //Por data de devolução
                    Fechamento fechamentoDev = new Fechamento();
                    if (emprestimo != null && emprestimo.dt_devolucao != null)
                        fechamentoDev = BusinessFinanceiro.getFechamentoByDta(emprestimo.dt_devolucao.Value, cd_escola);
                    //Se existir fechamento pela data de emprestimo ou pela data de devolução verifica se algum dos fechamentos são por balanço OU 
                    // se o parametro de bloquear movimento retroativo está marcado
                    if ((fechamentoEmp != null && fechamentoEmp.cd_fechamento > 0) || (fechamentoDev != null && fechamentoDev.cd_fechamento > 0))
                    {
                        if (fechamentoEmp.id_balanco || (fechamentoDev != null && fechamentoDev.id_balanco))
                            throw new FinanceiroBusinessException(string.Format(Utils.Messages.Messages.msgErroDelBibliFechBalanco), null, FinanceiroBusinessException.TipoErro.ERRO_EXISTE_FECHAMENTO_BALANCO, false);
                        //Verifica parâmetro 
                        bool bloqueado = DaoParametro.getParametroBloquearMovtoRetroativoEst(cd_escola);
                        if (bloqueado)
                            throw new FinanceiroBusinessException(string.Format(Utils.Messages.Messages.msgErroDelBibliFech), null, FinanceiroBusinessException.TipoErro.ERRO_EXISTE_FECHAMENTO, false);
                    }

                }
                movto = BusinessBiblioteca.deleteEmprestimos(emprestimos, cd_escola);
                transaction.Complete();
            }
            return movto;
        }

        public Emprestimo postRenovarEmprestimo(Emprestimo emprestimo, int cd_escola)
        {
            Emprestimo bibliotecaDB = new Emprestimo();
            this.sincronizarContextos(DaoParametro.DB());
            //Item item = emprestimo.Item;
            //PessoaSGF pessoa = emprestimo.Pessoa;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                SGFWebContext db = new SGFWebContext();
                bool existeTaxa = BusinessFiscal.existeMovimentoByOrigem(emprestimo.cd_biblioteca, Int32.Parse(db.LISTA_ORIGEM_LOGS["Emprestimo"].ToString()));
                if (existeTaxa)
                    throw new FinanceiroBusinessException(string.Format(Utils.Messages.Messages.msgErroAlterarExcluirBiblioComTaxa, "alterar"), null, FinanceiroBusinessException.TipoErro.ERRO_EXISTE_TAXA, false);
                //Verificar qual das datas está sendo modificada
                bibliotecaDB = BusinessBiblioteca.getEmprestimoById(emprestimo.cd_biblioteca);
                bibliotecaDB.dt_prevista_devolucao = emprestimo.dt_prevista_devolucao;
                //Se tiver alterando para colocar data de devolução, deve verificar apenas a data de devolução
                DaoParametro.saveChanges(false);
                transaction.Complete();
            }
            bibliotecaDB.Item = emprestimo.Item;
            bibliotecaDB.Pessoa = emprestimo.Pessoa;
            return bibliotecaDB;
        }

        #endregion

        #region Movimento

        public List<Titulo> gerarTitulosMovimento(Titulo tituloDefault, Movimento movimento)
        //variaveis de calcular dias uteis.
        {
            List<Titulo> titulos = new List<Titulo>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                IEnumerable<Feriado> feriadosEscola = null;
                DateTime dtaVencTituloAnterior = tituloDefault.dt_vcto_titulo;
                int mesProximoAno = 0;
                Parametro parametro = DaoParametro.getParametrosMovimento(movimento.cd_pessoa_empresa);
                //paramtros movimento
                Decimal valorTotal = movimento.ItensMovimento.Sum(x => x.vl_liquido_item);

                Titulo titulo = new Titulo();
                PoliticaComercial politica = BusinessFinanceiro.getPoliticaComercialById(movimento.cd_politica_comercial, movimento.cd_pessoa_empresa);
                int? localMovto = DaoParametro.getLocalMovto(movimento.cd_pessoa_empresa);
                //Pesquisa e vincula localMovimento
                if (localMovto != null)
                {
                    if (tituloDefault.cd_tipo_financeiro == (int)FundacaoFisk.SGF.GenericModel.TipoFinanceiro.TiposFinanceiro.CARTAO)
                    {
                        tituloDefault.LocalMovto = BusinessFinanceiro.getAllLocalMovtoCartaoSemPai(movimento.cd_pessoa_empresa).OrderBy(l => l.cd_local_movto).FirstOrDefault();
                        if (tituloDefault.LocalMovto == null)
                        {
                            throw new FinanceiroBusinessException(string.Format(Utils.Messages.Messages.msgErroNaoExisteLocalMovtoCartao), null,
                                FinanceiroBusinessException.TipoErro.ERRO_NAO_EXISTE_LOCALMOVTO_CARTAO, false);
                        }
                        tituloDefault.cd_local_movto = tituloDefault.LocalMovto.cd_local_movto;
                    }
                    else
                    {
                        tituloDefault.LocalMovto = BusinessFinanceiro.findLocalMovtoById(movimento.cd_pessoa_empresa, (int)localMovto);
                        tituloDefault.cd_local_movto = tituloDefault.LocalMovto.cd_local_movto;
                    }
                    

                    // Se Tipo financeiro for Cartão e origem movimento Entrada ou Saida;
                    if (tituloDefault.cd_tipo_financeiro == (int)FundacaoFisk.SGF.GenericModel.TipoFinanceiro.TiposFinanceiro.CARTAO &&
                        (movimento.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.ENTRADA || movimento.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.SAIDA))
                    {
                        //DateTime dataVencimentoTituloCartao = tituloDefault.dt_emissao_titulo;
                        tituloDefault.cd_pessoa_empresa = movimento.cd_pessoa_empresa;
                        //BusinessCoordenacao.aplicarTaxaBancaria(tituloDefault, politica.nm_parcelas, ref dataVencimentoTituloCartao);
                    }
                }
                else
                    throw new FinanceiroBusinessException(Utils.Messages.Messages.msgErroLocalMovtoObrigatorioParametroEscola, null, FinanceiroBusinessException.TipoErro.ERRO_ESCOLA_NOT_EXIST_LOCALMOVTO, true);

                int? sequenciaCheque = null;
                string stringCheque = "";
                if (tituloDefault.dc_num_documento_titulo != null)
                {
                    int legDoc = tituloDefault.dc_num_documento_titulo.Length;
                    for (int p = 0; p <= legDoc; p++)
                    {
                        sequenciaCheque = BusinessCoordenacao.isNumber(tituloDefault.dc_num_documento_titulo.Substring(p, (legDoc - p)));
                        if (sequenciaCheque.HasValue && sequenciaCheque > 0)
                        {
                            stringCheque = tituloDefault.dc_num_documento_titulo.Substring(0, p);
                            break;
                        }
                    }
                }
                //Gerando Titulos parcelas iguais.
                if (politica.id_parcela_igual)
                {
                    int idNew = 0;
                    if (politica.nm_parcelas != 0)
                    {
                        decimal parcelaLiquida = decimal.Round(valorTotal / politica.nm_parcelas, 2);
                        DateTime dataOpcao = new DateTime();

                        for (int i = 0; i < politica.nm_parcelas; i++)
                        {
                            mesProximoAno = mesProximoAno >= 12 ? 0 : mesProximoAno;
                            idNew++;
                            titulo = new Titulo();
                            titulo.cd_pessoa_empresa = movimento.cd_pessoa_empresa;
                            titulo.copy(tituloDefault);
                            titulo.dt_vcto_titulo = titulo.dt_vcto_titulo.ToLocalTime().Date;
                            titulo.dt_emissao_titulo = titulo.dt_emissao_titulo.ToLocalTime().Date;
                            titulo.dh_cadastro_titulo = DateTime.Now.ToUniversalTime();
                            titulo.vl_saldo_titulo = parcelaLiquida;
                            titulo.vl_titulo = parcelaLiquida;
                            titulo.nm_parcela_titulo = (byte)(i + 1);
                            titulo.id = idNew;
                            if (tituloDefault.dc_num_documento_titulo != null && sequenciaCheque.HasValue)
                            {
                                //incrementa a parte numerica
                                int proximoCheque = sequenciaCheque.Value + i;
                                int qtd_numeros = tituloDefault.dc_num_documento_titulo.Length - stringCheque.Length;
                                if (qtd_numeros > 0)
                                    titulo.dc_num_documento_titulo = stringCheque + proximoCheque.ToString().PadLeft(qtd_numeros, '0');
                                else
                                    titulo.dc_num_documento_titulo = stringCheque + proximoCheque.ToString();

                            }
                            if (politica.id_vencimento_fixo)
                            { //Fixo (sempre mês):
                                int mesProximo = titulo.dt_vcto_titulo.Month;
                                int anoProximoVenc = titulo.dt_vcto_titulo.Year;
                                if (i > 0)
                                {
                                    //calcula a quantidade de meses de intervalo.
                                    mesProximo = (i * politica.nm_intervalo_parcela);
                                    anoProximoVenc = dtaVencTituloAnterior.Month == 12 ? dtaVencTituloAnterior.Year + 1 : dtaVencTituloAnterior.Year;


                                    DateTime dataVenci = new DateTime(titulo.dt_vcto_titulo.Year, titulo.dt_vcto_titulo.Month, titulo.dt_vcto_titulo.Day);
                                    dataVenci = dataVenci.AddMonths(mesProximo);
                                    mesProximo = dataVenci.Month;
                                }
                                dataOpcao = corrigeProximoVencimentoCalculado(titulo, mesProximo, anoProximoVenc);
                            }
                            else
                            { //Não fixo:
                                dataOpcao = titulo.dt_vcto_titulo;
                                if (politica.nm_periodo_intervalo == (byte)PoliticaComercial.TipoPeriodoIntervalo.MES)
                                    dataOpcao = dataOpcao.AddMonths(i); //dataOpcao.AddDays((double)politica.nm_intervalo_parcela * 30 * i);
                                else if (politica.nm_periodo_intervalo == (byte)PoliticaComercial.TipoPeriodoIntervalo.DIAS)
                                    dataOpcao = dataOpcao.AddDays((double)politica.nm_intervalo_parcela * i);
                            }
                            //TODO:Deivid
                            if (movimento.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.SAIDA && parametro.id_alterar_venc_final_semana)
                            {
                                DateTime dataOpcaoOri = dataOpcao;
                                BusinessCoordenacao.pulaFeriadoEFinalSemana(ref dataOpcao, titulo.cd_pessoa_empresa, ref feriadosEscola);
                                if ((dataOpcaoOri.Month > dataOpcao.Month) || (dataOpcao.Month > dataOpcaoOri.Month))
                                {
                                    dataOpcao = dataOpcaoOri;
                                    BusinessCoordenacao.pulaFeriadoEFinalSemana(ref dataOpcao, titulo.cd_pessoa_empresa, ref feriadosEscola, false);
                                }

                            }
                            titulo.dt_vcto_titulo = dataOpcao.Date;
                            dtaVencTituloAnterior = titulo.dt_vcto_titulo;
                            titulos.Add(titulo);
                        }
                    }
                }
                //Gerando títulos parcelas diferentes:
                else
                {
                    int idNew = 0;
                    DateTime dataOpcao = tituloDefault.dt_vcto_titulo;

                    if (politica.ItemPolitica != null && politica.ItemPolitica.Count() > 0)
                    {
                        List<ItemPolitica> itensPolitica = politica.ItemPolitica.ToList();
                        //foreach (ItemPolitica itemPolitica in politica.ItemPolitica)
                        for (int i = 0; i < itensPolitica.Count; i++)
                        {
                            ItemPolitica itemPolitica = itensPolitica[i];
                            mesProximoAno = mesProximoAno >= 12 ? 0 : mesProximoAno;
                            idNew++;
                            decimal vlParcela = decimal.Round((valorTotal * (decimal)itemPolitica.pc_politica) / 100, 2);
                            titulo = new Titulo();
                            titulo.copy(tituloDefault);
                            titulo.cd_pessoa_empresa = movimento.cd_pessoa_empresa;
                            titulo.dt_vcto_titulo = titulo.dt_vcto_titulo.Date;
                            titulo.dt_emissao_titulo = titulo.dt_emissao_titulo.Date;
                            titulo.dh_cadastro_titulo = DateTime.Now.ToUniversalTime();
                            titulo.vl_saldo_titulo = vlParcela;
                            titulo.vl_titulo = vlParcela;
                            titulo.nm_parcela_titulo = (byte)(idNew);
                            titulo.id = idNew;
                            if (tituloDefault.dc_num_documento_titulo != null && sequenciaCheque.HasValue)
                            {
                                //incrementa a parte numerica
                                int proximoCheque = sequenciaCheque.Value + idNew;
                                int qtd_numeros = tituloDefault.dc_num_documento_titulo.Length - stringCheque.Length;
                                if (qtd_numeros > 0)
                                    titulo.dc_num_documento_titulo = stringCheque + proximoCheque.ToString().PadLeft(qtd_numeros, '0');
                                else
                                    titulo.dc_num_documento_titulo = stringCheque + proximoCheque.ToString();

                            }
                            if (!politica.id_vencimento_fixo)
                            { //Não Fixo:
                                if (itemPolitica.nm_dias_politica == null)
                                    throw new FinanceiroBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgPoliticaComercialInconsistente, null, FinanceiroBusinessException.TipoErro.ERRO_POLITICA_COMERCIAL_INCONSISTENTE, true);
                                dataOpcao = dataOpcao.AddDays((double)itemPolitica.nm_dias_politica);
                            }
                            else
                            { //Fixo:
                                int mesProximo = (titulo.dt_vcto_titulo.Month + i) > 12 ? mesProximoAno = (mesProximoAno + 1) : titulo.dt_vcto_titulo.Month + i;
                                int anoProximoVenc = dtaVencTituloAnterior.Month == 12 ? dtaVencTituloAnterior.Year + 1 : dtaVencTituloAnterior.Year;
                                dataOpcao = corrigeProximoVencimentoCalculado(titulo, mesProximo, anoProximoVenc);

                                if (movimento.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.SAIDA && parametro.id_alterar_venc_final_semana)
                                {
                                    BusinessCoordenacao.pulaFeriadoEFinalSemana(ref dataOpcao, titulo.cd_pessoa_empresa, ref feriadosEscola);
                                    if ((titulo.dt_vcto_titulo.Month > dataOpcao.Month) || (dataOpcao.Month > titulo.dt_vcto_titulo.Month))
                                    {
                                        dataOpcao = titulo.dt_vcto_titulo.ToLocalTime().Date;
                                        BusinessCoordenacao.pulaFeriadoEFinalSemana(ref dataOpcao, titulo.cd_pessoa_empresa, ref feriadosEscola, false);
                                    }
                                }
                            }

                            if (movimento.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.SAIDA && parametro.id_alterar_venc_final_semana)
                            {
                                BusinessCoordenacao.pulaFeriadoEFinalSemana(ref dataOpcao, titulo.cd_pessoa_empresa, ref feriadosEscola);
                                if ((titulo.dt_vcto_titulo.Month > dataOpcao.Month) || (dataOpcao.Month > titulo.dt_vcto_titulo.Month))
                                {
                                    dataOpcao = titulo.dt_vcto_titulo.ToLocalTime().Date;
                                    BusinessCoordenacao.pulaFeriadoEFinalSemana(ref dataOpcao, titulo.cd_pessoa_empresa, ref feriadosEscola, false);
                                }
                            }
                            titulo.dt_vcto_titulo = dataOpcao.Date;
                            dtaVencTituloAnterior = titulo.dt_vcto_titulo;
                            titulos.Add(titulo);
                        }
                    }
                }
                if ((politica.id_parcela_igual && politica.nm_parcelas != 0) || !politica.id_parcela_igual)
                {
                    //Verifica se existe diferença de centavos e aplica na 1° parcela.
                    Decimal vlCalcTotalTitulos = titulos.Sum(x => x.vl_titulo);
                    if (titulos.Count > 0 && vlCalcTotalTitulos != valorTotal)
                    {
                        if (vlCalcTotalTitulos < valorTotal)
                        {
                            titulos.FirstOrDefault().vl_titulo -= vlCalcTotalTitulos - valorTotal;
                            titulos.FirstOrDefault().vl_saldo_titulo -= vlCalcTotalTitulos - valorTotal;
                        }
                        else
                        {
                            titulos.FirstOrDefault().vl_titulo -= vlCalcTotalTitulos - valorTotal;
                            titulos.FirstOrDefault().vl_saldo_titulo -= vlCalcTotalTitulos - valorTotal;
                        }
                    }
                }

                if (tituloDefault.cd_tipo_financeiro == (int)FundacaoFisk.SGF.GenericModel.TipoFinanceiro.TiposFinanceiro.CARTAO &&
                    (movimento.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.ENTRADA || movimento.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.SAIDA ||
                     movimento.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.SERVICO))
                {
                    
                    DateTime dataVencimentoTituloCartao = tituloDefault.dt_emissao_titulo;
                    foreach (var objTitulo in titulos)
                    {
                        BusinessCoordenacao.aplicarTaxaBancaria(objTitulo, (int)titulo.nm_parcela_titulo, ref dataVencimentoTituloCartao, null);
                    }
                }

                transaction.Complete();
            }
            return titulos;
        }

        // Quando o dia calculado não é valido (exemplo: 31/02/2015) este método corrige para um dia válido (exemplo: 02/03/2015, sem ano bissexto).
        private DateTime corrigeProximoVencimentoCalculado(Titulo titulo, int mesProximo, int anoProximoVenc)
        {
            int diasMes = titulo.dt_vcto_titulo.Day;
            DateTime dataOpcao = new DateTime();

            do
            {
                DateTime dateValue;
                string novaData = diasMes + "/" + mesProximo + "/" + anoProximoVenc;
                if (DateTime.TryParse(novaData, out dateValue))
                {
                    dataOpcao = new DateTime(anoProximoVenc, mesProximo, diasMes);
                    diasMes = 0;
                }
                else
                    diasMes--;
            } while (diasMes > 0);

            return dataOpcao;
        }

        public MovimentoUI addMovimento(Movimento movimento)
        {
            this.sincronizarContextos(DaoParametro.DB());
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {

                //movimento = gerarNumeroNFMovimento(movimento);
                //Verificar se existe fechamento anterior a essa data
                Fechamento fechamento = BusinessFinanceiro.getFechamentoByDta(movimento.dt_mov_movimento, movimento.cd_pessoa_empresa);

                if (fechamento != null && fechamento.cd_fechamento > 0)
                {
                    if (fechamento.id_balanco)
                        throw new FinanceiroBusinessException(string.Format(Utils.Messages.Messages.msgErroInclMvtoFechBalanco), null, FinanceiroBusinessException.TipoErro.ERRO_EXISTE_FECHAMENTO_BALANCO, false);
                    //Verifica parâmetro 
                    bool bloqueado = DaoParametro.getParametroBloquearMovtoRetroativoEst(movimento.cd_pessoa_empresa);
                    if (bloqueado)
                        throw new FinanceiroBusinessException(string.Format(Utils.Messages.Messages.msgErroInclMvtoFech), null, FinanceiroBusinessException.TipoErro.ERRO_EXISTE_FECHAMENTO, false);
                }
                movimento = BusinessFiscal.addMovimento(movimento);
                PessoaEscola pessoaEsc = new PessoaEscola
                {
                    cd_escola = movimento.cd_pessoa_empresa,
                    cd_pessoa = movimento.cd_pessoa
                };
                BusinessEmpresa.postEmpresaPessoa(pessoaEsc);
                transaction.Complete();
            }
            return BusinessFiscal.getMovimentoReturnGrade(movimento.cd_movimento, movimento.cd_pessoa_empresa);
        }

        public void cancelarNFServico(Movimento movimento, bool id_empresa_propria)
        {
            this.sincronizarContextos(Dao.DB());
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED, Dao.DB()))
            {
                if (movimento.id_tipo_movimento == (byte)Movimento.TipoMovimentoEnum.SAIDA)
                {
                    Movimento movimentoContext = BusinessFiscal.getMovimentoById(movimento.cd_movimento);
                    movimentoContext.cd_pessoa_empresa = movimento.cd_pessoa_empresa;
                    movimentoContext.dc_justificativa_nf = movimento.dc_justificativa_nf;

                    if (movimentoContext.id_material_didatico == true && movimentoContext.cd_origem_movimento > 0)
                    {
                        throw new FinanceiroBusinessException(Utils.Messages.Messages.msgErrorDesprocessarMovimentoMaterialWithCdOrigem, null,
                            FinanceiroBusinessException.TipoErro.ERRO_CANCEL_NOTA_MATERIAL_WITH_CONTRATO, false);
                    }
                    

                    //Verificar se existe fechamento anterior a essa data
                    Fechamento fechamento = BusinessFinanceiro.getFechamentoByDta(movimentoContext.dt_mov_movimento, movimento.cd_pessoa_empresa);

                    if (fechamento != null && fechamento.cd_fechamento > 0)
                    {
                        if (fechamento.id_balanco)
                            throw new FinanceiroBusinessException(string.Format(Utils.Messages.Messages.msgErroUpdateMvtoFechBalanco), null, FinanceiroBusinessException.TipoErro.ERRO_EXISTE_FECHAMENTO_BALANCO, false);
                        //Verifica parâmetro 
                        bool bloqueado = DaoParametro.getParametroBloquearMovtoRetroativoEst(movimento.cd_pessoa_empresa);
                        if (bloqueado)
                            throw new FinanceiroBusinessException(string.Format(Utils.Messages.Messages.msgErroUpdateMvtoFech), null, FinanceiroBusinessException.TipoErro.ERRO_EXISTE_FECHAMENTO, false);
                    }
                }
                BusinessFiscal.cancelarNFServico(movimento.cd_pessoa_empresa, movimento.cd_movimento, movimento.dc_justificativa_nf, id_empresa_propria);
                transaction.Complete();
            }
            BusinessFiscal.spEnviarMasterSaf(movimento.cd_movimento);
        }

        public MovimentoUI editMovimento(Movimento movimento)
        {
            this.sincronizarContextos(DaoParametro.DB());
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                //Verificar se existe fechamento anterior a essa data
                Fechamento fechamento = BusinessFinanceiro.getFechamentoByDta(movimento.dt_mov_movimento, movimento.cd_pessoa_empresa);

                if (fechamento != null && fechamento.cd_fechamento > 0)
                {
                    if (fechamento.id_balanco)
                        throw new FinanceiroBusinessException(string.Format(Utils.Messages.Messages.msgErroUpdateMvtoFechBalanco), null, FinanceiroBusinessException.TipoErro.ERRO_EXISTE_FECHAMENTO_BALANCO, false);
                    //Verifica parâmetro 
                    bool bloqueado = DaoParametro.getParametroBloquearMovtoRetroativoEst(movimento.cd_pessoa_empresa);
                    if (bloqueado)
                        throw new FinanceiroBusinessException(string.Format(Utils.Messages.Messages.msgErroUpdateMvtoFech), null, FinanceiroBusinessException.TipoErro.ERRO_EXISTE_FECHAMENTO, false);
                }
                movimento = BusinessFiscal.editMovimento(movimento);
                PessoaEscola pessoaEsc = new PessoaEscola
                {
                    cd_escola = movimento.cd_pessoa_empresa,
                    cd_pessoa = movimento.cd_pessoa
                };
                BusinessEmpresa.postEmpresaPessoa(pessoaEsc);
                transaction.Complete();
            }
            return BusinessFiscal.getMovimentoReturnGrade(movimento.cd_movimento, movimento.cd_pessoa_empresa);
        }

        public bool deleteMovimentos(int[] cdMovimentos, int cd_empresa)
        {
            this.sincronizarContextos(DaoParametro.DB());
            bool movto = false;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                foreach (int cdMovto in cdMovimentos)
                {
                    Movimento movimento = BusinessFiscal.getMovtoById(cdMovto);
                    //Verificar se existe fechamento anterior a essa data
                    Fechamento fechamento = BusinessFinanceiro.getFechamentoByDta(movimento.dt_mov_movimento, movimento.cd_pessoa_empresa);

                    if (fechamento != null && fechamento.cd_fechamento > 0)
                    {
                        if (fechamento.id_balanco)
                            throw new FinanceiroBusinessException(string.Format(Utils.Messages.Messages.msgErroDelMvtoFechBalanco), null, FinanceiroBusinessException.TipoErro.ERRO_EXISTE_FECHAMENTO_BALANCO, false);
                        //Verifica parâmetro 
                        bool bloqueado = DaoParametro.getParametroBloquearMovtoRetroativoEst(movimento.cd_pessoa_empresa);
                        if (bloqueado)
                            throw new FinanceiroBusinessException(string.Format(Utils.Messages.Messages.msgErroDelMvtoFech), null, FinanceiroBusinessException.TipoErro.ERRO_EXISTE_FECHAMENTO, false);
                    }
                }
                movto = BusinessFiscal.deleteMovimentos(cdMovimentos, cd_empresa);
                transaction.Complete();
            }
            return movto;
        }
        public List<Movimento> getMovimentosbyOrigem(int cd_contrato, int cd_escola)
        {
            SGFWebContext db = new SGFWebContext();
            List<Movimento> movimentos = new List<Movimento>();
            movimentos = BusinessFiscal.getMovimentosbyOrigem(cd_contrato, Int32.Parse(db.LISTA_ORIGEM_LOGS["Contrato"].ToString()), cd_escola);
            return movimentos;
        }

        public void enviaPromocaoAlunoMatricula(int cd_aluno, int cd_contrato, int id_tipo_matricula)
        {
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                if (BusinessApiPromocaoIntercambio.aplicaApiPromocao() && (id_tipo_matricula == (int)Contrato.TipoMatricula.MATRICULA || id_tipo_matricula == (int)Contrato.TipoMatricula.REMATRICULA))
                {

                    //monta o objeto
                    PromocaoIntercambioParams alunoPromocaoIntercambioCurrent = new PromocaoIntercambioParams();
                    alunoPromocaoIntercambioCurrent = BusinessAluno.findAlunoApiPromocaoIntercambio(cd_aluno, (id_tipo_matricula == (int)Contrato.TipoMatricula.MATRICULA ? 2 : 3));

                    BusinessApiPromocaoIntercambio.ValidaParametros(alunoPromocaoIntercambioCurrent);
                    string codigo_promocao = BusinessApiPromocaoIntercambio.postExecutaRequestPromocaoIntercambio(alunoPromocaoIntercambioCurrent);

                    if (!string.IsNullOrEmpty(codigo_promocao))
                    {

                        PessoaPromocao pessoaPromocao = new PessoaPromocao();
                        pessoaPromocao.cd_pessoa = cd_aluno;
                        pessoaPromocao.id_tipo_pessoa = 1;
                        pessoaPromocao.dc_promocao = codigo_promocao;
                        BusinessSecretaria.addPessoaPromocao(pessoaPromocao);

                    }

                }
                transaction.Complete();
            }
        }

        public Movimento getMontaNFMaterial(int cd_contrato, int cd_escola)
        {
            Movimento novoMovimento = new Movimento();
            SGFWebContext db = new SGFWebContext();
            novoMovimento = BusinessFiscal.getMovimentoEditOrigem(cd_contrato, Int32.Parse(db.LISTA_ORIGEM_LOGS["Contrato"].ToString()), cd_escola, (int)Movimento.TipoMovimentoEnum.SAIDA);
            //Movimento será gerado pela procedure chamada na matricula
            //if (novoMovimento == null || novoMovimento.cd_movimento <= 0)
            //{
            //    //- Parâmetro: plano de contas, númeração automatica, ultimo número, tipo de nota fiscal
            //    Parametro parametro = getParametrosMovimento(cd_escola);
            //    Contrato dadosContrato = BusinessMatricula.getMatriculaForMovimento(cd_contrato, cd_escola);


            //    if (parametro != null && parametro.cd_pessoa_escola > 0 && dadosContrato != null && dadosContrato.cd_contrato > 0)
            //    {
            //        IEnumerable<ItemUI> materiais = BusinessFinanceiro.listItensMaterial(dadosContrato.cd_curso_atual, cd_escola);
            //        List<ItemMovimento> itensMovimento = new List<ItemMovimento>();
            //        //nota fiscal
            //        AliquotaUF aliquota = new AliquotaUF();
            //        TipoNotaFiscal tpNF = new TipoNotaFiscal();

            //        if (parametro.id_emitir_nf_mercantil && parametro.cd_tipo_nf_material.HasValue && parametro.cd_politica_comercial_nf.HasValue)
            //        {
            //            tpNF = BusinessFiscal.getTipoNFById(parametro.cd_tipo_nf_material.Value);
            //            if (dadosContrato.cd_uf_aluno != null && dadosContrato.cd_uf_aluno > 0)
            //                aliquota = BusinessFinanceiro.getAliquotaUFByEscDes(cd_escola, dadosContrato.cd_uf_aluno.Value);
            //        }

            //        decimal somaVlICMS = 0;
            //        decimal somaVlBaseICMS = 0;
            //        decimal somaVlBasePIS = 0;
            //        decimal somaVlBaseCOFINS = 0;
            //        decimal somaVlBaseIPI = 0;
            //        string CFOP = "";
            //        int? cdCfop = null;

            //        short nm_CFOP = 0;

            //        if (parametro.id_emitir_nf_mercantil && tpNF != null && tpNF.cd_tipo_nota_fiscal > 0)
            //        {
            //            CFOP cfopContext = BusinessFiscal.getCFOPByTpNF(tpNF.cd_tipo_nota_fiscal);
            //            if (cfopContext != null)
            //                cdCfop = cfopContext.cd_cfop;
            //            nm_CFOP = cfopContext.nm_cfop;
            //            //regra CFOP
            //            CFOP = verifcaEstadoEscAluno(cd_escola, dadosContrato.cd_pessoa_responsavel, (int)Movimento.TipoMovimentoEnum.SAIDA);
            //        }
            //        if (materiais != null && materiais.Count() > 0)
            //            foreach (ItemUI i in materiais)
            //            {
            //                ItemMovimento itemMovto = new ItemMovimento
            //                {
            //                    cd_item = i.cd_item,
            //                    dc_item_movimento = i.no_item,
            //                    qt_item_movimento = 1,
            //                    vl_unitario_item = (double)decimal.Round((decimal)i.vl_item, 2),
            //                    vl_total_item = i.vl_item,
            //                    vl_liquido_item = i.vl_item,
            //                    vl_acrescimo_item = 0,
            //                    vl_desconto_item = 0,
            //                    cd_plano_conta = i.cd_plano_conta,
            //                    dc_plano_conta = i.desc_plano_conta,
            //                    pc_desconto_item = 0,
            //                    dc_cfop = CFOP,
            //                    cd_cfop = cdCfop,
            //                    nm_cfop = nm_CFOP,
            //                    id_nf_item = parametro.id_emitir_nf_mercantil
            //                    //Dados Fiscais
            //                };

            //                if (parametro.id_emitir_nf_mercantil)
            //                {


            //                    int cd_situacao = 0;
            //                    byte id_forma_tributacao = 0;
            //                    double reducao = 0;
            //                    if (tpNF != null && tpNF.cd_tipo_nota_fiscal > 0)
            //                    {
            //                        int situacao_NF = tpNF.cd_situacao_tributaria.HasValue ? tpNF.cd_situacao_tributaria.Value : 0;
            //                        SituacaoTributaria situacaoTrib = new SituacaoTributaria();
            //                        if (tpNF.id_regime_tributario > 0)
            //                            situacaoTrib = BusinessFinanceiro.getSituacaoTributariaItem(i.cd_grupo_estoque, tpNF.id_regime_tributario, situacao_NF);
            //                        else
            //                            situacaoTrib = BusinessFinanceiro.getSituacaoTributariaFormaTrib(situacao_NF);
            //                        if (situacaoTrib.cd_situacao_tributaria > 0)
            //                        {
            //                            cd_situacao = situacaoTrib.cd_situacao_tributaria;
            //                            id_forma_tributacao = situacaoTrib.id_forma_tributacao;
            //                        }
            //                        itemMovto.cd_situacao_tributaria_ICMS = cd_situacao;
            //                        reducao = tpNF.pc_reducao;
            //                    }



            //                    if (aliquota != null && aliquota.cd_aliquota_uf > 0 && id_forma_tributacao != (int)SituacaoTributaria.FormaTrib.ISENTO)
            //                        itemMovto.pc_aliquota_ICMS = aliquota.pc_aliq_icms_padrao;
            //                    else
            //                        itemMovto.pc_aliquota_ICMS = 0;
            //                    itemMovto.cd_situacao_tributaria_PIS = (int)SituacaoTributaria.SitTrib.OPERACAO_SEM_INCIDENCIA_CONT_PIS;
            //                    itemMovto.cd_situacao_tributaria_COFINS = (int)SituacaoTributaria.SitTrib.OPERACAO_SEM_INCIDENCIA_CONT_COFINS;
            //                    itemMovto.vl_base_calculo_COFINS_item = itemMovto.vl_total_item;
            //                    itemMovto.vl_base_calculo_PIS_item = itemMovto.vl_total_item;
            //                    itemMovto.vl_base_calculo_IPI_item = itemMovto.vl_total_item;
            //                    double baseCalculoComReducao = (double)itemMovto.vl_total_item;
            //                    if (reducao > 0)
            //                        baseCalculoComReducao = (double)decimal.Round((decimal)(baseCalculoComReducao - (baseCalculoComReducao * reducao) / 100), 2);
            //                    if (itemMovto.pc_aliquota_ICMS > 0)
            //                        itemMovto.vl_base_calculo_ICMS_item = (decimal)baseCalculoComReducao;

            //                    itemMovto.vl_ICMS_item = decimal.Round((itemMovto.vl_base_calculo_ICMS_item * (decimal)itemMovto.pc_aliquota_ICMS) / 100, 2);

            //                }
            //                somaVlICMS = +itemMovto.vl_ICMS_item;
            //                somaVlBaseICMS = +itemMovto.vl_base_calculo_ICMS_item;
            //                somaVlBasePIS = +itemMovto.vl_base_calculo_PIS_item;
            //                somaVlBaseCOFINS = +itemMovto.vl_base_calculo_COFINS_item;
            //                somaVlBaseIPI = +itemMovto.vl_base_calculo_IPI_item;

            //                itensMovimento.Add(itemMovto);
            //            }
            //        int politicaComercial = 0;
            //        if (parametro.cd_politica_comercial_nf != null && parametro.cd_politica_comercial_nf > 0)
            //            politicaComercial = parametro.cd_politica_comercial_nf.Value;

            //        novoMovimento = new Movimento()
            //        {
            //            cd_aluno = dadosContrato.cd_aluno,
            //            no_aluno = dadosContrato.no_pessoa,
            //            cd_pessoa = dadosContrato.cd_pessoa_responsavel,
            //            cd_pessoa_aluno = dadosContrato.cd_pessoa_aluno,
            //            no_pessoa = dadosContrato.no_responsavel,
            //            id_tipo_movimento = (byte)Movimento.TipoMovimentoEnum.SAIDA,
            //            dt_vcto_movimento = DateTime.Now.Date,
            //            dt_emissao_movimento = DateTime.Now.Date,
            //            dt_mov_movimento = DateTime.Now.Date,
            //            cd_pessoa_empresa = cd_escola,
            //            cd_politica_comercial = politicaComercial,
            //            dc_politica_comercial = parametro.desc_politica_comercial_nf,
            //            ItensMovimento = itensMovimento,
            //            id_nf = parametro.id_emitir_nf_mercantil,
            //            cd_tipo_financeiro = (byte)Movimento.TipoFinanceiroEnum.TITULO,
            //            id_origem_movimento = Int32.Parse(db.LISTA_ORIGEM_LOGS["Contrato"].ToString()),
            //            cd_origem_movimento = cd_contrato,
            //            cd_sit_trib_ICMS = tpNF.cd_situacao_tributaria,
            //            nm_matricula_contrato = dadosContrato.nm_matricula_contrato

            //        };
            //        if (parametro.id_emitir_nf_mercantil)
            //        {
            //            novoMovimento.id_nf_escola = true;
            //            if (tpNF != null && tpNF.cd_tipo_nota_fiscal > 0)
            //            {
            //                novoMovimento.cd_tipo_nota_fiscal = tpNF.cd_tipo_nota_fiscal;
            //                novoMovimento.dc_tipo_nota = tpNF.dc_tipo_nota_fiscal;
            //                novoMovimento.tx_obs_fiscal = tpNF.tx_obs_tipo_nota;
            //                novoMovimento.dc_cfop_nf = CFOP;
            //                novoMovimento.cd_cfop_nf = cdCfop;
            //                novoMovimento.nm_cfop = nm_CFOP;
            //                novoMovimento.TipoNF = tpNF;
            //            }
            //            novoMovimento.id_status_nf = (byte)Movimento.StatusNFEnum.ABERTO;
            //            novoMovimento.vl_base_calculo_ICMS_nf = somaVlBaseICMS;
            //            novoMovimento.vl_base_calculo_PIS_nf = somaVlBasePIS;
            //            novoMovimento.vl_base_calculo_COFINS_nf = somaVlBaseCOFINS;
            //            novoMovimento.vl_base_calculo_IPI_nf = somaVlBaseIPI;
            //            novoMovimento.vl_ICMS_nf = somaVlICMS;


            //        }
            //    }
            //}
            return novoMovimento;
        }

        public Movimento getMontaNFBiblioteca(int cd_biblioteca, int cd_escola)
        {

            Movimento novoMovimento = new Movimento();
            SGFWebContext db = new SGFWebContext();
            novoMovimento = BusinessFiscal.getMovimentoEditOrigem(cd_biblioteca, Int32.Parse(db.LISTA_ORIGEM_LOGS["Emprestimo"].ToString()), cd_escola, (int)Movimento.TipoMovimentoEnum.SERVICO);
            if (novoMovimento == null || novoMovimento.cd_movimento <= 0)
            {
                //- Parâmetro: plano de contas, tipo de nota fiscal
                Parametro parametro = getParametrosMovimento(cd_escola);
                EmprestimoSearch dadosBiblioteca = BusinessBiblioteca.getEmprestimoById(cd_biblioteca, cd_escola);


                if (parametro != null && parametro.cd_pessoa_escola > 0)
                {
                    if (parametro.id_emitir_nf_servico &&
                        (!parametro.cd_item_biblioteca.HasValue || parametro.cd_item_biblioteca <= 0) &&
                        (!parametro.cd_tipo_nf_biblioteca.HasValue || parametro.cd_tipo_nf_biblioteca <= 0))
                        throw new FinanceiroBusinessException(Utils.Messages.Messages.msgErroNFTaxaItemBiblio, null, FinanceiroBusinessException.TipoErro.ERRO_NAO_EXISTE_PARAMETROS_BIBLIO, false);
                    if (dadosBiblioteca != null && dadosBiblioteca.cd_biblioteca > 0)
                    {
                        int cd_plano = 0;
                        string subgrupo = "";
                        if (parametro.cd_item_biblioteca.HasValue)
                        {
                            ItemSubgrupo plano = BusinessFinanceiro.getSubGrupoPlano(parametro.cd_item_biblioteca.Value, (byte)ItemSubgrupo.TipoSubgGrupo.SERVICO_SAIDA, cd_escola);
                            if (plano != null)
                            {
                                cd_plano = plano.cd_plano_conta;
                                subgrupo = plano.no_subgrupo;
                            }
                        }
                        List<ItemMovimento> itensMovimento = new List<ItemMovimento>();
                        //nota fiscal
                        double? aliqISS = 0;
                        TipoNotaFiscal tpNF = new TipoNotaFiscal();

                        if (parametro.id_emitir_nf_servico && parametro.cd_tipo_nf_biblioteca.HasValue && parametro.cd_politica_comercial_nf.HasValue)
                        {
                            tpNF = BusinessFiscal.getTipoNFById(parametro.cd_tipo_nf_biblioteca.Value);
                            if (tpNF.id_regime_tributario == (int)Parametro.REGIME_TRIBUTARIO.REGIME_NORMAL)
                                aliqISS = BusinessFinanceiro.getISSEscola(cd_escola);
                        }

                        decimal somaVlISS = 0;
                        decimal somaVlBaseISS = 0;
                        string CFOP = "";
                        int? cdCfop = null;
                        short nm_CFOP = 0;

                        if (parametro.id_emitir_nf_servico && tpNF != null && tpNF.cd_tipo_nota_fiscal > 0)
                        {

                            CFOP cfopContext = BusinessFiscal.getCFOPByTpNF(tpNF.cd_tipo_nota_fiscal);
                            nm_CFOP = cfopContext.nm_cfop;
                            if (cfopContext != null)
                                cdCfop = cfopContext.cd_cfop;
                            //regra CFOP
                            CFOP = verifcaEstadoEscAluno(cd_escola, dadosBiblioteca.cd_pessoa, (int)Movimento.TipoMovimentoEnum.SERVICO);

                        }
                        if (parametro.cd_item_biblioteca.HasValue && parametro.cd_item_biblioteca.Value > 0)
                        {
                            decimal valorMulta = 0;
                            if (dadosBiblioteca.vl_multa_emprestimo.HasValue)
                                valorMulta = dadosBiblioteca.vl_multa_emprestimo.Value;
                            ItemMovimento itemMovto = new ItemMovimento
                            {
                                cd_item = parametro.cd_item_biblioteca.Value,
                                dc_item_movimento = parametro.desc_item_biblioteca,
                                qt_item_movimento = 1,
                                vl_unitario_item = (double)valorMulta,
                                vl_total_item = valorMulta,
                                vl_liquido_item = valorMulta,
                                vl_acrescimo_item = 0,
                                vl_desconto_item = 0,
                                cd_plano_conta = cd_plano,
                                dc_plano_conta = subgrupo,
                                pc_desconto_item = 0,
                                cd_cfop = cdCfop,
                                dc_cfop = CFOP,
                                nm_cfop = nm_CFOP,
                                id_nf_item = parametro.id_emitir_nf_mercantil
                            };
                            if (parametro.id_emitir_nf_servico)
                            {
                                if (aliqISS.HasValue)
                                    itemMovto.pc_aliquota_ISS = aliqISS.Value;
                                itemMovto.vl_base_calculo_ISS_item = itemMovto.vl_total_item;
                                itemMovto.vl_ISS_item = (itemMovto.vl_base_calculo_ISS_item * (decimal)itemMovto.pc_aliquota_ISS) / 100;
                            }
                            somaVlISS = +itemMovto.vl_ISS_item;
                            somaVlBaseISS = +itemMovto.vl_base_calculo_ISS_item;
                            itensMovimento.Add(itemMovto);
                        }
                        int politicaComercial = 0;
                        if (parametro.cd_politica_comercial_nf != null && parametro.cd_politica_comercial_nf > 0)
                            politicaComercial = parametro.cd_politica_comercial_nf.Value;

                        novoMovimento = new Movimento()
                        {
                            cd_pessoa = dadosBiblioteca.cd_pessoa,
                            no_pessoa = dadosBiblioteca.no_pessoa,
                            id_tipo_movimento = (byte)Movimento.TipoMovimentoEnum.SERVICO,
                            dt_vcto_movimento = DateTime.Now.Date,
                            dt_emissao_movimento = DateTime.Now.Date,
                            dt_mov_movimento = DateTime.Now.Date,
                            cd_pessoa_empresa = cd_escola,
                            cd_politica_comercial = politicaComercial,
                            dc_politica_comercial = parametro.desc_politica_comercial_nf,
                            ItensMovimento = itensMovimento,
                            id_nf = parametro.id_emitir_nf_servico,
                            cd_tipo_financeiro = (byte)Movimento.TipoFinanceiroEnum.TITULO,
                            tx_obs_movimento = "Taxa de Biblioteca",
                            id_origem_movimento = Int32.Parse(db.LISTA_ORIGEM_LOGS["Emprestimo"].ToString()),
                            cd_origem_movimento = cd_biblioteca

                        };
                        if (parametro.id_emitir_nf_servico)
                        {
                            novoMovimento.id_nf_escola = true;
                            if (tpNF != null && tpNF.cd_tipo_nota_fiscal > 0)
                            {
                                novoMovimento.cd_tipo_nota_fiscal = tpNF.cd_tipo_nota_fiscal;
                                novoMovimento.dc_tipo_nota = tpNF.dc_tipo_nota_fiscal;
                                novoMovimento.tx_obs_fiscal = tpNF.tx_obs_tipo_nota;
                                novoMovimento.dc_cfop_nf = CFOP;
                                novoMovimento.cd_cfop_nf = cdCfop;
                                novoMovimento.nm_cfop = nm_CFOP;
                                novoMovimento.TipoNF = tpNF;
                            }
                            novoMovimento.id_status_nf = (byte)Movimento.StatusNFEnum.ABERTO;
                            novoMovimento.vl_base_calculo_ISS_nf = somaVlBaseISS;
                            novoMovimento.vl_ISS_nf = somaVlISS;
                        }
                    }
                }
            }
            return novoMovimento;
        }

        public List<Movimento> getGerarNFFaturamento(List<Titulo> titulos, int cd_escola, bool empresaPropria)
        {
            List<Movimento> retorno = new List<Movimento>();
            Movimento novoMovimento = new Movimento();
            SGFWebContext db = new SGFWebContext();
            if (empresaPropria)
                throw new FinanceiroBusinessException(Utils.Messages.Messages.msgEscolaPropriaNaoEmiteNFFaturamento, null, FinanceiroBusinessException.TipoErro.ERRO_EXISTE_NF_MAT, false);
            //- Parâmetro: plano de contas, tipo de nota fiscal
            Parametro parametro = getParametrosMovimento(cd_escola);
            int origemTitulo = Int32.Parse(db.LISTA_ORIGEM_LOGS["Titulo"].ToString());
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                if (parametro.id_emitir_nf_servico)
                {
                    if (!parametro.id_numero_nf_automatico)
                        throw new FinanceiroBusinessException(Utils.Messages.Messages.msgErroNrAutomaticaNFS, null, FinanceiroBusinessException.TipoErro.ERRO_NUMERACAO_AUTOMATICA_NF, false);
                    if ((!parametro.cd_item_taxa_matricula.HasValue || parametro.cd_item_taxa_matricula <= 0) &&
                        (!parametro.cd_tipo_nf_matricula.HasValue || parametro.cd_tipo_nf_matricula <= 0))
                        throw new FinanceiroBusinessException(Utils.Messages.Messages.msgErroNFTaxaParametro, null, FinanceiroBusinessException.TipoErro.ERRO_NAO_EXISTE_PARAMETROS_TAXA, false);
                    if ((!parametro.cd_item_mensalidade.HasValue || parametro.cd_item_mensalidade <= 0) &&
                        (!parametro.cd_tipo_nf_matricula.HasValue || parametro.cd_tipo_nf_matricula <= 0))
                        throw new FinanceiroBusinessException(Utils.Messages.Messages.msgErroNFParametroMens, null, FinanceiroBusinessException.TipoErro.ERRO_NAO_EXISTE_PARAMETROS_MAT, false);
                    if (!parametro.cd_politica_comercial_nf.HasValue || parametro.cd_politica_comercial_nf <= 0)
                        throw new FinanceiroBusinessException(Utils.Messages.Messages.msgErroNFParametroPolCom, null, FinanceiroBusinessException.TipoErro.ERRO_POLITICA_COM_BAIXA, false);
                    List<int> cdTitulos = titulos.Select(t => t.cd_titulo).ToList();
                    bool existeMovtoForTit = BusinessFiscal.existeMovimentoForTit(cd_escola, cdTitulos);
                    if (existeMovtoForTit)
                        throw new FinanceiroBusinessException(Utils.Messages.Messages.msgErroFatNF, null, FinanceiroBusinessException.TipoErro.ERRO_EXISTE_NF_MAT, false);
                    ItemSubgrupo planoTaxa = new ItemSubgrupo();
                    ItemSubgrupo planoMensalidade = new ItemSubgrupo();
                    if (parametro.id_requer_plano_contas_mov.HasValue)
                    {
                        bool existeTituloTaxa = titulos.Where(t => (t.dc_tipo_titulo == Titulo.TipoTitulo.TA.ToString() || t.dc_tipo_titulo == Titulo.TipoTitulo.TM.ToString())).Any();
                        bool existeTituloMensalidade = titulos.Where(t => (t.dc_tipo_titulo == Titulo.TipoTitulo.MA.ToString() || t.dc_tipo_titulo == (string)Titulo.TipoTitulo.ME.ToString() ||
                                                            t.dc_tipo_titulo == Titulo.TipoTitulo.MM.ToString() || t.dc_tipo_titulo == Titulo.TipoTitulo.AA.ToString() ||
                                                            t.dc_tipo_titulo == Titulo.TipoTitulo.AD.ToString() || t.dc_tipo_titulo == Titulo.TipoTitulo.PP.ToString())).Any();

                        if (existeTituloTaxa)
                            planoTaxa = BusinessFinanceiro.getSubGrupoPlano(parametro.cd_item_taxa_matricula.Value, (byte)ItemSubgrupo.TipoSubgGrupo.SERVICO_SAIDA, cd_escola);

                        if (existeTituloMensalidade)

                            planoMensalidade = BusinessFinanceiro.getSubGrupoPlano(parametro.cd_item_mensalidade.Value, (byte)ItemSubgrupo.TipoSubgGrupo.SERVICO_SAIDA, cd_escola);
                        if ((existeTituloTaxa && (planoTaxa == null || planoTaxa.cd_plano_conta <= 0)) || (existeTituloMensalidade && (planoMensalidade == null || planoMensalidade.cd_plano_conta <= 0)))
                            throw new FinanceiroBusinessException(Utils.Messages.Messages.msgErroPlanoItemTaxaMens, null, FinanceiroBusinessException.TipoErro.ERRO_NAO_EXISTE_PLANO_ITEM, false);

                    }
                    foreach (Titulo c in titulos)
                    {

                        Titulo dadosTitulo = BusinessFinanceiro.getTituloBaixaFinan(c.cd_titulo, cd_escola, TituloDataAccess.TipoConsultaTituloEnum.HAS_TITULO_MOVIMENTO_NF);


                        if (parametro != null && parametro.cd_pessoa_escola > 0 && dadosTitulo != null && dadosTitulo.cd_titulo > 0 &&
                            dadosTitulo.id_origem_titulo == Int32.Parse(db.LISTA_ORIGEM_LOGS["Contrato"].ToString()))
                        {

                            List<ItemMovimento> itensMovimento = new List<ItemMovimento>();
                            //nota fiscal
                            double? aliqISS = 0;
                            TipoNotaFiscal tpNF = new TipoNotaFiscal();
                            tpNF = BusinessFiscal.getTipoNFById(parametro.cd_tipo_nf_matricula.Value);
                            if (tpNF.id_regime_tributario == (int)Parametro.REGIME_TRIBUTARIO.REGIME_NORMAL)
                                aliqISS = BusinessFinanceiro.getISSEscola(cd_escola);

                            decimal somaVlISS = 0;
                            decimal somaVlBaseISS = 0;
                            int cd_item = 0;
                            string no_item = "";
                            int? cd_plano = null;
                            string subgrupo = "";
                            if (Titulo.TipoTitulo.TM.ToString().Equals(dadosTitulo.dc_tipo_titulo) || Titulo.TipoTitulo.TA.ToString().Equals(dadosTitulo.dc_tipo_titulo))
                            {
                                cd_item = parametro.cd_item_taxa_matricula.Value;
                                no_item = parametro.desc_item_taxa_matricula;
                                cd_plano = planoTaxa.cd_plano_conta;
                                subgrupo = planoTaxa.no_subgrupo;
                            }
                            else
                            {
                                cd_item = parametro.cd_item_mensalidade.Value;
                                no_item = parametro.desc_item_mensalidade;
                                cd_plano = planoMensalidade.cd_plano_conta;
                                subgrupo = planoMensalidade.no_subgrupo;
                            }
                            string CFOP = "";
                            short nmCFOP = 0;
                            int cdCfop = 0;
                            if (tpNF != null && tpNF.cd_tipo_nota_fiscal > 0)
                            {
                                CFOP cfopContext = BusinessFiscal.getCFOPByTpNF(tpNF.cd_tipo_nota_fiscal);
                                nmCFOP = cfopContext.nm_cfop;
                                cdCfop = cfopContext != null ? cfopContext.cd_cfop : 0;
                                CFOP = tpNF.dc_CFOP;
                                //regra CFOP
                                CFOP = verifcaEstadoEscAluno(cd_escola, dadosTitulo.cd_pessoa_responsavel, (int)Movimento.TipoMovimentoEnum.SERVICO);
                            }


                            ItemMovimento itemMovto = new ItemMovimento
                            {

                                cd_item = cd_item,
                                dc_item_movimento = no_item,
                                qt_item_movimento = 1,
                                vl_unitario_item = (double)dadosTitulo.vl_titulo,
                                vl_total_item = dadosTitulo.vl_titulo,
                                vl_liquido_item = dadosTitulo.vl_titulo,
                                vl_acrescimo_item = 0,
                                vl_desconto_item = 0,
                                cd_plano_conta = cd_plano,
                                dc_plano_conta = subgrupo,
                                pc_desconto_item = 0,
                                dc_cfop = CFOP,
                                cd_cfop = cdCfop,
                                nm_cfop = nmCFOP
                            };
                            if (aliqISS.HasValue)
                                itemMovto.pc_aliquota_ISS = aliqISS.Value;
                            itemMovto.vl_base_calculo_ISS_item = itemMovto.vl_total_item;
                            itemMovto.vl_ISS_item = Decimal.Round((itemMovto.vl_base_calculo_ISS_item * (decimal)itemMovto.pc_aliquota_ISS) / 100, 2);
                            somaVlISS = +itemMovto.vl_ISS_item;
                            somaVlBaseISS = +itemMovto.vl_base_calculo_ISS_item;
                            itensMovimento.Add(itemMovto);
                            //“Mensalidade ou Taxa de Matricula(referente ao Titulo número xxx parcela xxx)”.
                            string tipoMensaliade = "Mensalidade";
                            if (dadosTitulo.dc_tipo_titulo == "TA" || dadosTitulo.dc_tipo_titulo == "TM")
                                tipoMensaliade = "Taxa de matrícula";
                            string observacao = tipoMensaliade + " (referente ao título número " + dadosTitulo.nm_titulo + " parcela " + dadosTitulo.nm_parcela_titulo;


                            novoMovimento = new Movimento()
                            {
                                cd_pessoa = dadosTitulo.cd_pessoa_responsavel,
                                no_pessoa = dadosTitulo.nomeResponsavel,
                                id_tipo_movimento = (byte)Movimento.TipoMovimentoEnum.SERVICO,
                                dt_vcto_movimento = DateTime.Now.Date,
                                dt_emissao_movimento = DateTime.Now.Date,
                                dt_mov_movimento = DateTime.Now.Date,
                                cd_pessoa_empresa = cd_escola,
                                cd_politica_comercial = parametro.cd_politica_comercial_nf.Value,
                                dc_politica_comercial = parametro.desc_politica_comercial_nf,
                                ItensMovimento = itensMovimento,
                                id_nf = parametro.id_emitir_nf_servico,
                                cd_tipo_financeiro = (byte)Movimento.TipoFinanceiroEnum.TITULO,
                                tx_obs_movimento = observacao

                            };
                            novoMovimento.id_nf_escola = true;
                            if (tpNF != null && tpNF.cd_tipo_nota_fiscal > 0)
                            {
                                novoMovimento.cd_tipo_nota_fiscal = tpNF.cd_tipo_nota_fiscal;
                                novoMovimento.dc_tipo_nota = tpNF.dc_tipo_nota_fiscal;
                                novoMovimento.tx_obs_fiscal = tpNF.tx_obs_tipo_nota;
                                novoMovimento.dc_cfop_nf = CFOP;
                                novoMovimento.cd_cfop_nf = cdCfop;
                                novoMovimento.nm_cfop = nmCFOP;
                            }
                            novoMovimento.id_status_nf = (byte)Movimento.StatusNFEnum.ABERTO;
                            novoMovimento.vl_base_calculo_ISS_nf = somaVlBaseISS;
                            novoMovimento.vl_ISS_nf = somaVlISS;
                            novoMovimento.id_origem_movimento = origemTitulo;
                            novoMovimento.cd_origem_movimento = c.cd_titulo;
                            retorno.Add(novoMovimento);
                        }
                        //Incluindo registro
                        MovimentoUI movimentoInc = addMovimento(novoMovimento);
                        //Alterando para processado
                        processarNF(cd_escola, movimentoInc.cd_movimento, empresaPropria);
                    }
                }
                else
                    throw new FinanceiroBusinessException(Utils.Messages.Messages.msgErroNotConfigParametroNF, null, FinanceiroBusinessException.TipoErro.ERRO_NAO_CONFIG_PARAMETRO_NF, false);

                transaction.Complete();
            }
            return retorno;
        }

        public Movimento getMontaNFBaixaFinanceira(int cd_baixa_titulo, int cd_escola)
        {
            Movimento novoMovimento = new Movimento();
            SGFWebContext db = new SGFWebContext();

            //- Parâmetro: plano de contas, tipo de nota fiscal
            Parametro parametro = getParametrosMovimento(cd_escola);
            int origemTitulo = Int32.Parse(db.LISTA_ORIGEM_LOGS["BaixaTitulo"].ToString());
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                novoMovimento = BusinessFiscal.getMovimentoEditOrigem(cd_baixa_titulo, origemTitulo, cd_escola, (int)Movimento.TipoMovimentoEnum.SERVICO);
                if (novoMovimento == null || novoMovimento.cd_movimento <= 0)
                {
                    Titulo titulo = BusinessFinanceiro.getTituloBaixaFinanMovimentoNF(cd_baixa_titulo, cd_escola);
                    if (parametro.id_emitir_nf_servico)
                    {
                        if (titulo != null && varificarRegrasNFTituloMatricula(cd_baixa_titulo, origemTitulo, cd_escola, parametro, titulo))
                        {
                            List<ItemMovimento> itensMovimento = new List<ItemMovimento>();
                            int cd_plano = 0;
                            string subgrupo = "";
                            int cd_item = 0;
                            string no_item = "";
                            if (Titulo.TipoTitulo.TM.ToString().Equals(titulo.dc_tipo_titulo) || Titulo.TipoTitulo.TA.ToString().Equals(titulo.dc_tipo_titulo))
                            {
                                cd_item = parametro.cd_item_taxa_matricula.Value;
                                no_item = parametro.desc_item_taxa_matricula;
                            }
                            else
                            {
                                cd_item = parametro.cd_item_mensalidade.Value;
                                no_item = parametro.desc_item_mensalidade;
                            }
                            if (cd_item > 0)
                            {
                                ItemSubgrupo plano = BusinessFinanceiro.getSubGrupoPlano(cd_item, (byte)ItemSubgrupo.TipoSubgGrupo.SERVICO_SAIDA, cd_escola);
                                if (plano != null)
                                {
                                    cd_plano = plano.cd_plano_conta;
                                    subgrupo = plano.no_subgrupo;
                                }
                            }
                            //nota fiscal
                            double? aliqISS = 0;
                            TipoNotaFiscal tpNF = new TipoNotaFiscal();
                            tpNF = BusinessFiscal.getTipoNFById(parametro.cd_tipo_nf_matricula.Value);
                            if (tpNF.id_regime_tributario == (int)Parametro.REGIME_TRIBUTARIO.REGIME_NORMAL)
                                aliqISS = BusinessFinanceiro.getISSEscola(cd_escola);

                            decimal somaVlISS = 0;
                            decimal somaVlBaseISS = 0;

                            string CFOP = "";
                            short nmCFOP = 0;
                            int? cdCfop = null;
                            if (tpNF != null && tpNF.cd_tipo_nota_fiscal > 0)
                            {
                                CFOP cfopContext = BusinessFiscal.getCFOPByTpNF(tpNF.cd_tipo_nota_fiscal);
                                nmCFOP = cfopContext.nm_cfop;
                                if (cfopContext != null)
                                    cdCfop = cfopContext.cd_cfop;
                                CFOP = tpNF.dc_CFOP;
                                //regra CFOP
                                CFOP = verifcaEstadoEscAluno(cd_escola, titulo.cd_pessoa_responsavel, (int)Movimento.TipoMovimentoEnum.SERVICO);
                            }
                            ItemMovimento itemMovto = new ItemMovimento
                            {

                                cd_item = cd_item,
                                dc_item_movimento = no_item,
                                qt_item_movimento = 1,
                                vl_unitario_item = (double)titulo.vl_titulo,
                                vl_total_item = titulo.vl_titulo,
                                vl_liquido_item = titulo.vl_titulo,
                                vl_acrescimo_item = 0,
                                vl_desconto_item = 0,
                                cd_plano_conta = cd_plano,
                                dc_plano_conta = subgrupo,
                                pc_desconto_item = 0,
                                dc_cfop = CFOP,
                                cd_cfop = cdCfop,
                                nm_cfop = nmCFOP
                            };
                            if (aliqISS.HasValue)
                                itemMovto.pc_aliquota_ISS = aliqISS.Value;
                            itemMovto.vl_base_calculo_ISS_item = itemMovto.vl_total_item;
                            itemMovto.vl_ISS_item = Decimal.Round((itemMovto.vl_base_calculo_ISS_item * (decimal)itemMovto.pc_aliquota_ISS) / 100, 2);
                            somaVlISS = +itemMovto.vl_ISS_item;
                            somaVlBaseISS = +itemMovto.vl_base_calculo_ISS_item;
                            itensMovimento.Add(itemMovto);
                            //“Mensalidade ou Taxa de Matricula(referente ao Titulo número xxx parcela xxx)”.
                            string tipoMensaliade = "Mensalidade";
                            if (titulo.dc_tipo_titulo == "TA" || titulo.dc_tipo_titulo == "TM")
                                tipoMensaliade = "Taxa de matrícula";
                            string observacao = tipoMensaliade + " (referente ao título número " + titulo.nm_titulo + " parcela " + titulo.nm_parcela_titulo;


                            novoMovimento = new Movimento()
                            {
                                cd_pessoa = titulo.cd_pessoa_responsavel,
                                no_pessoa = titulo.nomeResponsavel,//Veriricar pesquisa no select
                                id_tipo_movimento = (byte)Movimento.TipoMovimentoEnum.SERVICO,
                                dt_vcto_movimento = DateTime.Now.Date,
                                dt_emissao_movimento = DateTime.Now.Date,
                                dt_mov_movimento = DateTime.Now.Date,
                                cd_pessoa_empresa = cd_escola,
                                cd_politica_comercial = parametro.cd_politica_comercial_nf.Value,
                                dc_politica_comercial = parametro.desc_politica_comercial_nf,
                                ItensMovimento = itensMovimento,
                                id_nf = parametro.id_emitir_nf_servico,
                                cd_tipo_financeiro = (byte)Movimento.TipoFinanceiroEnum.TITULO,
                                tx_obs_movimento = observacao

                            };
                            novoMovimento.id_nf_escola = true;
                            if (tpNF != null && tpNF.cd_tipo_nota_fiscal > 0)
                            {
                                novoMovimento.cd_tipo_nota_fiscal = tpNF.cd_tipo_nota_fiscal;
                                novoMovimento.dc_tipo_nota = tpNF.dc_tipo_nota_fiscal;
                                novoMovimento.tx_obs_fiscal = tpNF.tx_obs_tipo_nota;
                                novoMovimento.dc_cfop_nf = CFOP;
                                novoMovimento.cd_cfop_nf = cdCfop;
                                novoMovimento.nm_cfop = nmCFOP;
                            }
                            novoMovimento.id_status_nf = (byte)Movimento.StatusNFEnum.ABERTO;
                            novoMovimento.vl_base_calculo_ISS_nf = somaVlBaseISS;
                            novoMovimento.vl_ISS_nf = somaVlISS;
                            novoMovimento.id_origem_movimento = origemTitulo;
                            novoMovimento.cd_origem_movimento = cd_baixa_titulo;
                            novoMovimento.TipoNF = tpNF;
                        }
                    }
                    else
                        throw new FinanceiroBusinessException(Utils.Messages.Messages.msgErroNotConfigParametroNF, null, FinanceiroBusinessException.TipoErro.ERRO_NAO_CONFIG_PARAMETRO_NF, false);
                }
                transaction.Complete();
            }
            return novoMovimento;
        }

        private bool varificarRegrasNFTituloMatricula(int cd_registro, int id_origem_registro, int cd_escola, Parametro parametro, Titulo tituloContext)
        {
            SGFWebContext db = new SGFWebContext();
            if (!parametro.id_numero_nf_automatico)
                throw new FinanceiroBusinessException(Utils.Messages.Messages.msgErroNrAutomaticaNFS, null, FinanceiroBusinessException.TipoErro.ERRO_NUMERACAO_AUTOMATICA_NF, false);

            if (parametro != null && parametro.cd_pessoa_escola > 0 && tituloContext != null && tituloContext.cd_titulo > 0 &&
                tituloContext.id_origem_titulo == Int32.Parse(db.LISTA_ORIGEM_LOGS["Contrato"].ToString()))
            {
                if ((Titulo.TipoTitulo.TM.ToString().Equals(tituloContext.dc_tipo_titulo) || Titulo.TipoTitulo.TA.ToString().Equals(tituloContext.dc_tipo_titulo)) &&
                    (!parametro.cd_item_taxa_matricula.HasValue || parametro.cd_item_taxa_matricula <= 0) &&
                    (!parametro.cd_tipo_nf_matricula.HasValue || parametro.cd_tipo_nf_matricula <= 0))
                    throw new FinanceiroBusinessException(Utils.Messages.Messages.msgErroNFTaxaParametro, null, FinanceiroBusinessException.TipoErro.ERRO_NAO_EXISTE_PARAMETROS_TAXA, false);
                if ((!Titulo.TipoTitulo.TM.ToString().Equals(tituloContext.dc_tipo_titulo) && !Titulo.TipoTitulo.TA.ToString().Equals(tituloContext.dc_tipo_titulo)) &&
                    (!parametro.cd_item_mensalidade.HasValue || parametro.cd_item_mensalidade <= 0) &&
                    (!parametro.cd_tipo_nf_matricula.HasValue || parametro.cd_tipo_nf_matricula <= 0))
                    throw new FinanceiroBusinessException(Utils.Messages.Messages.msgErroNFParametroMens, null, FinanceiroBusinessException.TipoErro.ERRO_NAO_EXISTE_PARAMETROS_MAT, false);
                if (!parametro.cd_politica_comercial_nf.HasValue || parametro.cd_politica_comercial_nf <= 0)
                    throw new FinanceiroBusinessException(Utils.Messages.Messages.msgErroNFParametroPolCom, null, FinanceiroBusinessException.TipoErro.ERRO_POLITICA_COM_BAIXA, false);
            }
            return true;
        }

        public bool verificarGeracaoNFSBaixa(int cd_baixa, int cd_escola)
        {
            if (!DaoParametro.getEmitirNFServico(cd_escola))
                throw new FinanceiroBusinessException(Utils.Messages.Messages.msgErroNotConfigParametroNF, null, FinanceiroBusinessException.TipoErro.ERRO_NAO_CONFIG_PARAMETRO_NF, false);
            if (!BusinessFinanceiro.verificarTituloOrigemMatricula(cd_baixa, cd_escola))
                throw new FinanceiroBusinessException(Utils.Messages.Messages.msgErroOrigemTituloMatricula, null, FinanceiroBusinessException.TipoErro.ERRO_ORIGEM_TITULO, false);

            return true;
        }

        public TipoNotaFiscal postTpNF(TipoNotaFiscal tipo, int cd_escola)
        {
            TipoNotaFiscal retornar = new TipoNotaFiscal();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                if (tipo.id_regime_tributario == 0)
                    tipo.id_regime_tributario = DaoParametro.getParametroRegimeTrib(cd_escola);
                retornar = BusinessFiscal.postTpNF(tipo);
                transaction.Complete();
            }

            return retornar;
        }

        public TipoNotaFiscal putTpNF(TipoNotaFiscal tipo, int cdEscola)
        {
            TipoNotaFiscal retornar = new TipoNotaFiscal();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                TipoNotaFiscal tipoNFContext = BusinessFiscal.getTipoNFById(tipo.cd_tipo_nota_fiscal);
                if (tipoNFContext.id_regime_tributario != tipo.id_regime_tributario)
                {
                    bool existeParametro = DaoParametro.existeParametroTpNF(tipo.cd_tipo_nota_fiscal);
                    if (existeParametro)
                        throw new FinanceiroBusinessException(Utils.Messages.Messages.msgErroTpNFParametro, null, FinanceiroBusinessException.TipoErro.ERRO_PARAMETRO_TIPO_NF, false);
                    bool existeMovimento = BusinessFiscal.existeMovimentoTpNF(tipo.cd_tipo_nota_fiscal);
                    if (existeMovimento)
                        throw new FinanceiroBusinessException(Utils.Messages.Messages.msgErroTpNFMovimento, null, FinanceiroBusinessException.TipoErro.ERRO_MOVTO_TIPO_NF, false);
                }
                if (tipo.id_regime_tributario == 0)
                    tipo.id_regime_tributario = DaoParametro.getParametroRegimeTrib(cdEscola);
                retornar = BusinessFiscal.putTpNF(tipo);
                transaction.Complete();
            }
            return retornar;
        }

        public DadosNF postDadosNF(DadosNF dado, int cd_escola)
        {
            DadosNF retornar = new DadosNF();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                if (!dado.id_regime_tributario.HasValue || dado.id_regime_tributario == 0)
                    dado.id_regime_tributario = DaoParametro.getParametroRegimeTrib(cd_escola);
                retornar = BusinessFinanceiro.postDadosNF(dado);
                transaction.Complete();
            }

            return retornar;
        }

        public DadosNF putDadosNF(DadosNF dado, int cdEscola)
        {
            DadosNF retornar = new DadosNF();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                if (!dado.id_regime_tributario.HasValue || dado.id_regime_tributario == 0)
                    dado.id_regime_tributario = DaoParametro.getParametroRegimeTrib(cdEscola);
                retornar = BusinessFiscal.putDadosNF(dado);
                transaction.Complete();
            }
            return retornar;
        }

        public bool processarNF(int cdEscola, int cd_movimento, bool empresaPropria)
        {
            sincronizarContextos(DaoParametro.DB());
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                Movimento movtoContext = BusinessFiscal.processarNF(cdEscola, cd_movimento, empresaPropria);
                movtoContext = gerarNumeroNFMovimento(movtoContext);
                int tipoMovimento = movtoContext.id_tipo_movimento;
                Parametro parametros = DaoParametro.getParametrosByEscola(movtoContext.cd_pessoa_empresa);
                List<Titulo> titulosMovt = BusinessFinanceiro.getTitulosByMovimento(movtoContext.cd_movimento, movtoContext.cd_pessoa_empresa).ToList();
                if (movtoContext.id_nf_escola && DaoParametro.getParametroNumeracaoAutoNF(movtoContext.cd_pessoa_empresa))
                    if (tipoMovimento == (int)Movimento.TipoMovimentoEnum.SAIDA || tipoMovimento == (int)Movimento.TipoMovimentoEnum.SERVICO || tipoMovimento == (int)Movimento.TipoMovimentoEnum.DEVOLUCAO)
                    {
                        if (tipoMovimento == (int)Movimento.TipoMovimentoEnum.SAIDA || tipoMovimento == (int)Movimento.TipoMovimentoEnum.DEVOLUCAO)
                            parametros.nm_nf_mercantil = movtoContext.nm_movimento.Value;
                        else
                            parametros.nm_nf_servico = movtoContext.nm_movimento.Value;
                        if (titulosMovt != null && titulosMovt.Count() > 0)
                            foreach (Titulo t in titulosMovt)
                                t.nm_titulo = movtoContext.nm_movimento;
                    }
                DaoParametro.saveChanges(false);
                transaction.Complete();
            }
            return true;
        }


        public MovimentoUI processarNFMovimento(int cdEscola, int cd_movimento, bool empresaPropria, Movimento movimento)
        {
            sincronizarContextos(DaoParametro.DB());
            MovimentoUI mUi = new MovimentoUI();
            bool enviar_master_saf = false;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.READ_COMMITED, DataAccessMovimento.DB(), TransactionScopeBuilder.TransactionTime.MODEREATE))							
            {
                Fechamento fechamento = BusinessFinanceiro.getFechamentoByDta(movimento.dt_mov_movimento, movimento.cd_pessoa_empresa);

                if (fechamento != null && fechamento.cd_fechamento > 0)
                {
                    if (fechamento.id_balanco)
                        throw new FinanceiroBusinessException(string.Format(Utils.Messages.Messages.msgErroInclMvtoFechBalanco), null, FinanceiroBusinessException.TipoErro.ERRO_EXISTE_FECHAMENTO_BALANCO, false);
                    //Verifica parâmetro 
                    bool bloqueado = DaoParametro.getParametroBloquearMovtoRetroativoEst(movimento.cd_pessoa_empresa);
                    if (bloqueado)
                        throw new FinanceiroBusinessException(string.Format(Utils.Messages.Messages.msgErroInclMvtoFech), null, FinanceiroBusinessException.TipoErro.ERRO_EXISTE_FECHAMENTO, false);
                }

                string name = DataAccessMovimento.getMovimentoWithItensName(cd_movimento, cdEscola);
                movimento.no_pessoa = name;
                if (movimento.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.ENTRADA)
                realizaMovimento(movimento);

                Movimento movtoContext = BusinessFiscal.processarNF(cdEscola, cd_movimento, empresaPropria);
                movtoContext.no_pessoa = BusinessAluno.getPessoaById(movtoContext.cd_pessoa).no_pessoa;
                bool preencherParametroMercantil = false;
                //LBM O número já foi gerado na procedure que gera a nota de material didático.
                if ((movtoContext.nm_movimento == null) || (!movtoContext.id_material_didatico && !movimento.ItensMovimento.Where(x => x.Item.id_voucher_carga).Any()))
                {
                    preencherParametroMercantil = true;
                    movtoContext = gerarNumeroNFMovimento(movtoContext);
                }
                if (movtoContext.id_nf_escola && DaoParametro.getParametroNumeracaoAutoNF(movtoContext.cd_pessoa_empresa) &&
                    (movtoContext.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.SAIDA || movtoContext.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.SERVICO ||
                    movtoContext.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.DEVOLUCAO))
                {
                    Parametro parametros = DaoParametro.getParametrosByEscola(movtoContext.cd_pessoa_empresa);
                    List<Titulo> titulosMovt = BusinessFinanceiro.getTitulosByMovimento(movtoContext.cd_movimento, movtoContext.cd_pessoa_empresa).ToList();
                    List<Kardex> kardexs = BusinessFinanceiro.getKardexItensMovimentoNF(movtoContext.cd_movimento, movtoContext.cd_pessoa_empresa).ToList();
                    if (movtoContext.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.SAIDA || movtoContext.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.DEVOLUCAO)
                    {
                        if (preencherParametroMercantil) parametros.nm_nf_mercantil = movtoContext.nm_movimento.Value;
                    }
                    else
                        if (preencherParametroMercantil) parametros.nm_nf_servico = movtoContext.nm_movimento.Value;
                    if (titulosMovt != null && titulosMovt.Count() > 0)
                        foreach (Titulo t in titulosMovt)
                            t.nm_titulo = movtoContext.nm_movimento;
                    if (kardexs != null && kardexs.Count() > 0)
                        foreach (Kardex k in kardexs)
                        {
                            k.tx_obs_kardex = Movimento.gerarObservacaoKardex(movtoContext);
                            k.nm_documento = movtoContext.nm_movimento == null ? "" : movtoContext.nm_movimento.ToString();
                        }
                    enviar_master_saf = empresaPropria;
                }
                DaoParametro.saveChanges(false);
                transaction.Complete();
               
               
            }

            mUi = BusinessFiscal.getMovimentoReturnGrade(cd_movimento, cdEscola);


            if (mUi != null && mUi.cd_movimento > 0)
            {
                if (enviar_master_saf)
                    mUi.envio_masterSaf_empresa_propira = BusinessFiscal.spEnviarMasterSaf(cd_movimento);
            }
            else
            {
                throw new FiscalBusinessException(Messages.msgErrorProccessNF, null, FiscalBusinessException.TipoErro.ERRO_PROCESSAR_NF, false);
            }

            //MovimentoUI mUi = new MovimentoUI();
            //mUi = BusinessFiscal.getMovimentoReturnGrade(cd_movimento, cdEscola);
            //if (enviar_master_saf)
            //    mUi.envio_masterSaf_empresa_propira = BusinessFiscal.spEnviarMasterSaf(cd_movimento);
            return mUi;
        }


        public Movimento gerarTitulosMovimentoPostNf(int cdEscola, int cd_movimento, int cd_tipo_movimento)
        {
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                SGFWebContext db = new SGFWebContext();
                Movimento movtoContext = DataAccessMovimento.getMovimentoWithItens(cd_movimento, cdEscola);
                Titulo tituloDefault = new Titulo();
                tituloDefault.cd_pessoa_responsavel = movtoContext.cd_pessoa;
                tituloDefault.cd_origem_titulo = 0;
                tituloDefault.cd_pessoa_titulo = movtoContext.cd_pessoa;
                tituloDefault.cd_tipo_financeiro = movtoContext.cd_tipo_financeiro;
                tituloDefault.cd_local_movto = 0;
                tituloDefault.cd_plano_conta_tit = 0;
                tituloDefault.nomeResponsavel = movtoContext.no_pessoa;
                tituloDefault.tipoDoc = movtoContext.dc_tipo_nota;
                tituloDefault.descLocalMovto = "";
                tituloDefault.dc_tipo_titulo = "NF";
                tituloDefault.nm_titulo = movtoContext.nm_movimento;
                tituloDefault.nm_parcela_titulo = 0;
                tituloDefault.dc_num_documento_titulo = null;
                tituloDefault.dt_emissao_titulo = movtoContext.dt_emissao_movimento;
                tituloDefault.dt_vcto_titulo = movtoContext.dt_vcto_movimento;
                tituloDefault.vl_titulo = 0;
                tituloDefault.vl_saldo_titulo = 0;
                tituloDefault.id_origem_titulo = Int32.Parse(db.LISTA_ORIGEM_LOGS["Movimento"].ToString());
                tituloDefault.id_status_titulo = (int)Movimento.StatusNFEnum.ABERTO;

                tituloDefault.id_natureza_titulo =
                    (movtoContext.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.SAIDA ||
                     movtoContext.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.SERVICO)
                        ? (byte?)Titulo.NaturezaTitulo.RECEBER : (byte?)Titulo.NaturezaTitulo.PAGAR;
                tituloDefault.dh_cadastro_titulo = null;

                List<Titulo> titulosMovtGerado = new List<Titulo>(gerarTitulosMovimento(tituloDefault, movtoContext)); 
                //Parametro parametros = DaoParametro.getParametrosByEscola(movtoContext.cd_pessoa_empresa);
                //List<Titulo> titulosMovt = BusinessFinanceiro.getTitulosByMovimento(movtoContext.cd_movimento, movtoContext.cd_pessoa_empresa).ToList();
                List<Kardex> kardexs = BusinessFinanceiro.getKardexItensMovimentoNF(movtoContext.cd_movimento, movtoContext.cd_pessoa_empresa).ToList();
                movtoContext.titulos = titulosMovtGerado;
                

                transaction.Complete();
                return movtoContext;
            }

        }


        public void realizaMovimento(Movimento movimento)
        {
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.READ_COMMITED))
            {
                

                //var movtoEdit = editMovimento(movimento);

                if (movimento.id_nf)
                {
                    if (movimento.ItensMovimento != null && movimento.ItensMovimento.Count() > 0)
                    {
                        crudItensMovimento(movimento.ItensMovimento.ToList(), movimento);
                    }


                    if (movimento.cd_tipo_financeiro == (int)TipoFinanceiro.TiposFinanceiro.CHEQUE)
                    {
                        foreach (var titulo in movimento.titulos)
                        {
                            titulo.dc_num_documento_titulo = movimento.Cheques.FirstOrDefault().nm_primeiro_cheque + "";
                        }


                    }

                    if (movimento != null && movimento.titulos.Count() > 0)
                    {
                        if (BusinessFiscal.verificarTipoNotaFiscalPermiteMovimentoFinanceiro(movimento.cd_tipo_nota_fiscal.Value))
                        {
                            crudTitulosMovimento(movimento.titulos, movimento);

                        }
                    }


                    BusinessEmpresa.postEmpresaPessoa(new PessoaEscola
                    {
                        cd_escola = movimento.cd_pessoa_empresa,
                        cd_pessoa = movimento.cd_pessoa
                    });
                }

                transaction.Complete();
            }

        }

        private void crudTitulosMovimento(List<Titulo> titulosView, Movimento movimento)
        {
            this.sincronizarContextos(DataAccessMovimento.DB());
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.READ_COMMITED))
            {
                List<Titulo> titulosAdd = new List<Titulo>();
                List<Titulo> titulosContext = DataAccessTitulo.getTitulosByMovimento(movimento.cd_movimento, movimento.cd_pessoa_empresa).ToList();
                IEnumerable<Titulo> titulosComCodigo = titulosView.Where(x => x.cd_titulo != 0);
                //IEnumerable<Titulo> titulosDeleted = titulosContext.Where(tc => !titulosComCodigo.Any(tv => tc.cd_titulo == tv.cd_titulo));
                //remover o titulo e o plano conta correspondente.
                //BusinessFinan.deleteAllTitulo(titulosDeleted.ToList(), movimento.cd_pessoa_empresa);

                foreach (Titulo item in titulosView)
                {
                    if (item.vl_titulo <= 0)
                        throw new FinanceiroBusinessException(Messages.msgErroNaoExisteSaldoTitulo, null, FinanceiroBusinessException.TipoErro.ERRO_NAO_EXISTE_SALDO_TITULO, false);

                    if (item.cd_titulo == 0)
                    {
                        item.nm_titulo = movimento.nm_movimento;
                        item.cd_pessoa_empresa = movimento.cd_pessoa_empresa;
                        item.cd_origem_titulo = movimento.cd_movimento;
                        item.dt_vcto_titulo = item.dt_vcto_titulo.Date;
                        item.dt_emissao_titulo = item.dt_emissao_titulo.Date;
                        titulosAdd.Add(item);
                    }
                    else
                    {
                        var titulo = titulosContext.Where(hc => hc.cd_titulo == item.cd_titulo).FirstOrDefault();
                        if (titulo != null && titulo.cd_titulo > 0)
                        {
                            if (titulo.id_status_cnab != (int)Titulo.StatusCnabTitulo.INICIAL && titulo.id_status_cnab != (int)Titulo.StatusCnabTitulo.CONFIRMADO_PEDIDO_BAIXA)
                                throw new FinanceiroBusinessException(Messages.msgNotUpdateTituloEnviadoCNAB, null, FinanceiroBusinessException.TipoErro.ERRO_TITULO_ENVIADO_CNAB, false);
                            item.nm_titulo = movimento.nm_movimento;
                            titulo = Titulo.changeValuesTituloEditMovimento(titulo, item);
                            if (titulo.vl_titulo != titulo.vl_saldo_titulo)
                                if (titulo.vl_liquidacao_titulo > 0 && DataAccessTitulo.DB().Entry(titulo).State == System.Data.Entity.EntityState.Modified)
                                    throw new FinanceiroBusinessException(string.Format(Utils.Messages.Messages.msgErroUpdateTituloBaixa), null,
                                                                          FinanceiroBusinessException.TipoErro.ERRO_TITULO_COM_BAIXA, false);
                            if (DataAccessTitulo.DB().Entry(titulo).Property(p => p.vl_titulo).IsModified)
                            {
                                PlanoTitulo pt = DataAccessPlanoTitulo.getPlanoTituloByCdTitulo(titulo.cd_titulo, movimento.cd_pessoa_empresa);
                                if (pt != null && pt.cd_plano_conta > 0)
                                    pt.vl_plano_titulo = titulo.vl_titulo;
                            }
                        }
                    }
                }
                if (titulosAdd.Count() > 0)
                    addTitulosERateoMovimento(titulosAdd, movimento.ItensMovimento.ToList(), movimento.cd_movimento, movimento.cd_pessoa_empresa, movimento.nm_movimento);
                DataAccessTitulo.saveChanges(false);
                DataAccessPlanoTitulo.saveChanges(false);
                transaction.Complete();
            }
        }

        private void crudItensMovimento(List<ItemMovimento> itensMovimentoView, Movimento movimento)
        {
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.READ_COMMITED))
            {
                this.sincronizarContextos(DataAccessMovimento.DB());

                List<ItemMovimento> itemMovimentoContext = DataAccessItemMovimento.getItensMovimentoByMovimento(movimento.cd_movimento, movimento.cd_pessoa_empresa).ToList();
                IEnumerable<ItemMovimento> itemMovimentoComCodigo = from it in itensMovimentoView
                                                                    where it.cd_item_movimento != 0
                                                                    select it;

                foreach (var item in itensMovimentoView)
                {
                    ItemEscola itemServico = DataAccessItemEscola.findItemEscolabyId(item.cd_item, movimento.cd_pessoa_empresa);

                    if (itemServico == null)
                    {
                        ItemEscola novoItemEscola = new ItemEscola();
                        novoItemEscola.cd_item = item.cd_item;
                        novoItemEscola.cd_pessoa_escola = movimento.cd_pessoa_empresa;

                        itemServico = DataAccessItemEscola.add(novoItemEscola, false);
                    }

                    //Valor do Kardex: quando for venda será o valor do custo do item escola, quando for compra será o valor unitário do item
                    Kardex kardexView = criaNovoKardex(item, movimento, itemServico);

                    item.cd_movimento = movimento.cd_movimento;
                   
                    //Incluindo Kardex
                    //Regras de estoque serão verificadas para itens que movimentam estoque
                    incluirKardexOfMovimento(item, movimento, kardexView, itemServico);
                    
                }
                DataAccessItemMovimento.saveChanges(false);
                DataAccessItemEscola.saveChanges(false);
                transaction.Complete();
            }
        }

        private List<Titulo> addTitulosERateoMovimento(List<Titulo> titulos, List<ItemMovimento> itensMovto, int cd_movimento, int cd_empresa, int? nm_movimento)
        {
            List<Titulo> newTitulos = new List<Titulo>();
            decimal totolGeral = itensMovto.Sum(x => x.vl_liquido_item);
            List<ItemMovimento> itensPlanoDiferentes = itensMovto.GroupBy(i => i.cd_plano_conta).Select(group => group.First()).ToList();
            foreach (Titulo t in titulos)
            {
                var planoConta = false;
                t.nm_titulo = nm_movimento.HasValue ? nm_movimento : cd_movimento;
                t.LocalMovto = null;
                t.cd_pessoa_empresa = cd_empresa;
                t.cd_origem_titulo = cd_movimento;
                t.vl_titulo = Decimal.Round(t.vl_titulo, 2);
                t.vl_saldo_titulo = Decimal.Round(t.vl_saldo_titulo, 2);
                var tituloAdd = DataAccessTitulo.addContext(t, false);
                newTitulos.Add(tituloAdd);

                foreach (ItemMovimento it in itensPlanoDiferentes)
                {
                    if (it.cd_plano_conta == null || it.cd_plano_conta == 0)
                        planoConta = false;
                    else
                    {
                        decimal vlPlanoTitulo = 0;
                        decimal somaVlItensPlano = itensMovto.Where(i => i.cd_plano_conta == it.cd_plano_conta).Sum(x => x.vl_liquido_item);
                        if (t.vl_titulo > 0)
                            vlPlanoTitulo = (somaVlItensPlano / totolGeral) * t.vl_titulo;
                        else
                            vlPlanoTitulo = 0;
                        PlanoTitulo pTitulo = new PlanoTitulo
                        {
                            cd_titulo = t.cd_titulo,
                            cd_plano_conta = (int)it.cd_plano_conta,
                            vl_plano_titulo = Decimal.Round(vlPlanoTitulo, 2)
                        };
                        t.PlanoTitulo.Add(pTitulo);
                    }
                }
                //Verifica se existe diferença de centavos e aplica na 1° parcela.
                if (planoConta)
                {
                    Decimal vlTotalplanosTitulo = (Decimal)t.PlanoTitulo.Sum(x => x.vl_plano_titulo);
                    if (vlTotalplanosTitulo != t.vl_titulo)
                        if (vlTotalplanosTitulo < t.vl_titulo)
                            t.PlanoTitulo.FirstOrDefault().vl_plano_titulo -= vlTotalplanosTitulo - t.vl_titulo;

                        else
                            t.PlanoTitulo.FirstOrDefault().vl_plano_titulo -= vlTotalplanosTitulo - t.vl_titulo;
                }
            }
            DataAccessTitulo.saveChanges(false);
            //DataAccessPlanoTitulo.saveChanges(false);
            return newTitulos;
        }

        public bool postAlterarLocalMovtoTitulosNFFechada(int cd_escola, List<Titulo> titulos)
        {
            bool retornar = false;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {

                List<Titulo> titulosSemBaixa = DataAccessTitulo.getTitulosSemBaixa(cd_escola, titulos);

                foreach (Titulo t in titulosSemBaixa)
                {
                    Titulo tituloUpdate = titulos.Where(x => x.cd_titulo == t.cd_titulo).FirstOrDefault();

                    t.cd_local_movto = tituloUpdate.cd_local_movto;
                    t.pc_taxa_cartao = tituloUpdate.pc_taxa_cartao;
                    t.nm_dias_cartao = tituloUpdate.nm_dias_cartao;


                    DataAccessTitulo.edit(t, false);
                }

                DataAccessTitulo.saveChanges(false);
                transaction.Complete();
                retornar = true;
                return retornar;
            }
        }



        private void incluirKardexOfMovimento(ItemMovimento item, Movimento movimento, Kardex kardexView, ItemEscola itemServico)
        {
            if (DataAccessTipoItem.verificaMovimentarEstoque(item.cd_item) && (!movimento.id_nf || DataAccessTipoNotaFiscal.verificarTipoNotaFiscalPermiteMovimentoEstoque(item.cd_movimento)))
            {
                int tipoMvto = movimento.id_tipo_movimento;
                if ((movimento.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.DEVOLUCAO || movimento.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.SERVICO) && movimento.cd_tipo_nota_fiscal.HasValue)
                    tipoMvto = (int)DataAccessTipoNotaFiscal.getTipoMvtoTpNF(movimento.cd_tipo_nota_fiscal.Value);
                if (tipoMvto == (int)Movimento.TipoMovimentoEnum.SAIDA)
                {
                    itemServico.qt_estoque -= item.qt_item_movimento;
                    //itemServico.vl_item = decimal.Round(item.vl_liquido_item / item.qt_item_movimento, 2);
                    itemServico.vl_item = decimal.Round((decimal)item.vl_unitario_item, 2);

                    if (movimento.id_bloquear_venda_sem_estoque && itemServico.qt_estoque < 0)
                        throw new FinanceiroBusinessException(string.Format(Utils.Messages.Messages.msgEstoqueNegativoItemMovimento), null,
                                                    FinanceiroBusinessException.TipoErro.ERRO_ESTOQUE_NEGATIVO, false);
                }
                if (tipoMvto == (int)Movimento.TipoMovimentoEnum.ENTRADA)
                {
                    itemServico.qt_estoque += item.qt_item_movimento;
                    itemServico.vl_custo = decimal.Round(item.vl_liquido_item / item.qt_item_movimento, 2);
                }
                //Tipo de movimento de despesa não geram kardex
                if (tipoMvto != (int)Movimento.TipoMovimentoEnum.DESPESA)
                {
                    //Atualiza quantidade do item no estoque.
                    kardexView.cd_registro_origem = item.cd_item_movimento;
                    BusinessFinan.addKardex(kardexView);
                }
            }
        }

        private void deletarKardexOfMovimento(ItemMovimento item, Movimento movimento, bool houve_troca_tipo)
        {
            if ((DataAccessTipoItem.verificaMovimentarEstoque(item.cd_item) && (!movimento.id_nf ||
                                                                                DataAccessTipoNotaFiscal.verificarTipoNotaFiscalPermiteMovimentoEstoque(item.cd_movimento) || houve_troca_tipo)))
            {
                SGFWebContext cdb = new SGFWebContext();
                Kardex kardex = BusinessFinan.getKardexByOrigem((byte)cdb.LISTA_ORIGEM_LOGS["ItemMovimento"], item.cd_item_movimento).FirstOrDefault();
                //Atualiza quantidade do item no estoque.
                ItemEscola itemServico = DataAccessItemEscola.findItemEscolabyId(item.cd_item, movimento.cd_pessoa_empresa);
                int tipoMovimento = movimento.id_tipo_movimento;
                //Se for devolução, o tipo do movimento deve ser o oposto do tipo de movimento da NF Devolvida, ou seja, quando devolver uma NF Saida, a NF será de entrada e vice-versa
                if ((movimento.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.DEVOLUCAO || movimento.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.SERVICO) && movimento.cd_tipo_nota_fiscal.HasValue)
                    tipoMovimento = (int)DataAccessTipoNotaFiscal.getTipoMvtoTpNF(movimento.cd_tipo_nota_fiscal.Value);
                if (tipoMovimento == (int)Movimento.TipoMovimentoEnum.SAIDA)
                    itemServico.qt_estoque += item.qt_item_movimento;
                if (tipoMovimento == (int)Movimento.TipoMovimentoEnum.ENTRADA)
                {
                    itemServico.qt_estoque -= item.qt_item_movimento;
                    //TODO: Deivid voltar ao ultimo preço aplicado
                    //itemServico.vl_item = item.vl_liquido_item;
                }
                if (kardex != null && kardex.cd_registro_origem > 0)
                    BusinessFinan.deleteKardex(kardex);
            }
        }

        private Kardex criaNovoKardex(ItemMovimento item, Movimento movimento, ItemEscola itemServico)
        {
            Kardex kardexView = new Kardex();
            int tipoMovimento = movimento.id_tipo_movimento;
            if ((movimento.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.DEVOLUCAO || movimento.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.SERVICO) && movimento.cd_tipo_nota_fiscal.HasValue)
                tipoMovimento = (int)DataAccessTipoNotaFiscal.getTipoMvtoTpNF(movimento.cd_tipo_nota_fiscal.Value);
            if (tipoMovimento != (int)Movimento.TipoMovimentoEnum.DESPESA)
            {
                SGFWebContext cdb = new SGFWebContext();
                decimal vlCompra = 0;

                if (tipoMovimento == (int)Movimento.TipoMovimentoEnum.ENTRADA)
                {
                    if (item.vl_liquido_item > 0)
                        vlCompra = item.vl_liquido_item;
                }
                else
                if (itemServico != null && itemServico.vl_custo > 0 && item.qt_item_movimento > 0)
                    vlCompra = itemServico.vl_custo * item.qt_item_movimento;
                if(tipoMovimento == (int)Movimento.TipoMovimentoEnum.DEVOLUCAO && movimento.cd_nota_fiscal.HasValue)
                {
                    Movimento movimentoDevolucao = BusinessFiscal.getMovimentoById((int)movimento.cd_nota_fiscal);
                    tipoMovimento = 3 - movimentoDevolucao.id_tipo_movimento;
                }
                kardexView = new Kardex
                {
                    cd_pessoa_empresa = movimento.cd_pessoa_empresa,
                    cd_item = item.cd_item,
                    cd_origem = (byte)cdb.LISTA_ORIGEM_LOGS["ItemMovimento"],
                    cd_registro_origem = item.cd_item_movimento,
                    dt_kardex = movimento.dt_mov_movimento,
                    id_tipo_movimento = (byte)tipoMovimento,
                    qtd_kardex = item.qt_item_movimento,
                    nm_documento = movimento.nm_movimento + "",
                    tx_obs_kardex = Movimento.gerarObservacaoKardex(movimento),
                    vl_kardex = vlCompra
                };
            }
            return kardexView;
        }

        #endregion

        #region Posição Financeira
        public IEnumerable<RptReceberPagar> receberPagarStoreProcedure(int cdEscola, DateTime pDtaI, DateTime pDtaF, int pForn, DateTime pDtaBase, byte pNatureza, int pPlanoContas, int ordem, bool cSegura, int cdTpLiq, int cdTpFinan, int tipo, string situacoes, int cdTurma)
        {
            List<RptReceberPagar> lista = new List<RptReceberPagar>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED, null, TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                lista = BusinessFinanceiro.receberPagarStoreProcedure(cdEscola, pDtaI, pDtaF, pForn, pDtaBase, pNatureza, pPlanoContas, ordem, cSegura, cdTpLiq, cdTpFinan, tipo, situacoes, cdTurma).ToList();
                List<Titulo> titulos = BusinessFinanceiro.getTituloBaixaFinanSimulacao(lista.Select(x => x.cd_titulo).ToList(), cdEscola, null, TituloDataAccess.TipoConsultaTituloEnum.HAS_SIMULACAO);

                for (int i = 0; i < lista.Count; i++)
                {

                    BaixaTitulo baixa = new BaixaTitulo();
                    Titulo titulo = titulos.Where(x => x.cd_titulo == lista[i].cd_titulo).FirstOrDefault();
                    Parametro parametro = DaoParametro.getParametrosBaixa(cdEscola);

                    //if (lista[i].dc_tipo_titulo != "TM" && lista[i].dc_tipo_titulo != "TA")
                    //{
                        baixa.dt_baixa_titulo = pDtaBase;
                        BusinessCoordenacao.simularBaixaTitulo(titulo, ref baixa, parametro, cdEscola, true, false);
                        lista[i].vl_saldo = baixa.vl_liquidacao_baixa;
                        lista[i].pc_desc = baixa.pc_juros_calc;
                        lista[i].pc_juros = baixa.pc_juros_calc;
                        lista[i].pc_multa = baixa.pc_multa_calc;
                        lista[i].pc_pont = baixa.pc_pontualidade;
                        lista[i].vl_acr = baixa.vl_juros_calculado;
                        lista[i].vl_desc = baixa.vl_desconto_baixa_calculado;
                        lista[i].vl_multa = baixa.vl_multa_calculada;
                    //}
                    //else
                    //{
                    //    lista[i].vl_saldo = 0;
                    //    lista[i].pc_desc = 0;
                    //    lista[i].pc_juros = 0;
                    //    lista[i].pc_multa = 0;
                    //    lista[i].pc_pont = 0;
                    //    lista[i].vl_acr = 0;
                    //    lista[i].vl_desc = 0;
                    //    lista[i].vl_multa = 0;
                    //}
                    //lista[i].dc_descontos = baixa.dc_descontos;

                    /*falta os descontos do titulo 
                     *       set @des_desconto = @des_desconto + @desc  + ' - ' + convert(varchar(5),@desconto) + 
                             case when @vl_desconto = 0 then '%' else '' end + CHAR(13)
                     */
                    List<TelefoneSGF> listaTelefonesAluno = BusinessPessoa.getAllTelefonesContatosByPessoa(lista[i].cd_pessoa).ToList();
                    List<TelefoneSGF> listaTelefonesResponsavel = BusinessPessoa.getAllTelefonesContatosByPessoa(lista[i].cd_responsavel).ToList();

                    lista[i].nm_telefone_contatos_aluno = TelefoneSGF.getTelefones(listaTelefonesAluno);
                    lista[i].nm_telefone_contatos_resp = TelefoneSGF.getTelefones(listaTelefonesResponsavel);
                    lista[i].dc_descontos = baixa.des_desconto;

                    //Refaz o rateamento dos valores na visão de plano de contas:
                    if (ordem == 2 && lista[i].vl_plano_titulo.HasValue)
                    { //Visão de plano de contas
                        if (lista[i].vl_saldo_titulo != 0)
                        {
                            lista[i].vl_acr = lista[i].vl_plano_titulo.Value * lista[i].vl_acr / lista[i].vl_saldo_titulo;
                            lista[i].vl_desc = lista[i].vl_plano_titulo.Value * lista[i].vl_desc / lista[i].vl_saldo_titulo;
                            lista[i].vl_multa = lista[i].vl_plano_titulo.Value * lista[i].vl_multa / lista[i].vl_saldo_titulo;
                            lista[i].vl_saldo = lista[i].vl_plano_titulo.Value * lista[i].vl_saldo / lista[i].vl_saldo_titulo;
                        }
                        lista[i].vl_saldo_titulo = lista[i].vl_plano_titulo.Value;
                    }
                }
                transaction.Complete();
            }
            return lista;
        }
        #endregion

        #region Trasação Financeira

        public TransacaoFinanceira postIncluirTransacao(TransacaoFinanceira transacao)
        {
            this.sincronizarContextos(DaoParametro.DB());
            transacao.movimentoRetroativo = getParametroMovimentoRetroativo(transacao.cd_pessoa_empresa);
            if (transacao.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.CANCELAMENTO)
                transacao.cd_local_movto = getLocalMovto(transacao.cd_pessoa_empresa);
            transacao.id_liquidacao_tit_ant_aberto = getIdBloquearliqTituloAnteriorAberto(transacao.cd_pessoa_empresa);
            if (((transacao.cd_tipo_liquidacao_old == (int)TipoLiquidacao.TipoLiqui.CHEQUE_PRE_DATADO && transacao.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.TROCA_FINANCEIRA) || transacao.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.CHEQUE_PRE_DATADO || transacao.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.CHEQUE_AVISTA)||
                (transacao.cd_tipo_liquidacao_old == (int)TipoLiquidacao.TipoLiqui.CHEQUE_AVISTA && transacao.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.TROCA_FINANCEIRA))
            {
                if (transacao.cheque != null)
                {
                    if (!Cheque.validarDadosCheque(transacao.cheque, true))
                        throw new FinanceiroBusinessException(string.Format(Utils.Messages.Messages.msgErroFaltaInforCheque), null, FinanceiroBusinessException.TipoErro.ERRO_DADOS_CHEQUE, false);
                    transacao.ChequeTransacaoFinanceira.Add(new ChequeTransacao
                    {
                        dt_bom_para = ((DateTime)transacao.cheque.dt_bom_para).Date,
                        nm_cheque = transacao.cheque.nm_primeiro_cheque,
                        Cheque = transacao.cheque
                    });
                }
                else
                {
                    foreach (BaixaTitulo b in transacao.Baixas)
                    {

                        if (b.Titulo.Cheque != null){
                            if (b.Titulo.Cheque.dt_bom_para == null && b.Titulo.dt_vcto_titulo != null)
                                b.Titulo.Cheque.dt_bom_para = b.Titulo.dt_vcto_titulo;
                            if (string.IsNullOrEmpty(b.Titulo.Cheque.nm_primeiro_cheque) && !string.IsNullOrEmpty(b.Titulo.dc_num_documento_titulo))
                                b.Titulo.Cheque.nm_primeiro_cheque = b.Titulo.dc_num_documento_titulo;

                            if (!Cheque.validarDadosCheque(b.Titulo.Cheque, true))
                                throw new FinanceiroBusinessException(string.Format(Utils.Messages.Messages.msgErroFaltaInforCheque), null, FinanceiroBusinessException.TipoErro.ERRO_DADOS_CHEQUE, false);

                            if (!DataAccessMovimento.verificaTipoFinanceiroMovimento(b.Titulo.cd_titulo, b.Titulo.dc_tipo_titulo, transacao.cd_pessoa_empresa))
                                throw new FinanceiroBusinessException(string.Format(Utils.Messages.Messages.msgErroTipoFinanceiroDiferente), null, FinanceiroBusinessException.TipoErro.ERRO_TIPO_FINANCEIRO_DIFERENTE, false);
                            if (b.Titulo.dc_tipo_titulo == "AD"){
                                Cheque cheque = new Cheque();
                                b.Titulo.Cheque.cd_contrato = b.Titulo.cd_origem_titulo;
                                cheque = BusinessFinanceiro.getChequeByContrato((int)b.Titulo.cd_origem_titulo);
                                if (cheque != null && cheque.cd_cheque > 0)
                                {
                                    cheque.copy(b.Titulo.Cheque);
                                    BusinessFinanceiro.editCheque(cheque);
                                }
                                else {
                                    BusinessFinanceiro.addCheque(b.Titulo.Cheque);
                                    cheque = BusinessFinanceiro.getChequeByContrato((int)b.Titulo.cd_origem_titulo);
                                }
                                b.Titulo.Cheque.cd_cheque = cheque.cd_cheque;
                            }else
                            {
                                
                                if(b.Titulo.Cheque.cd_cheque == 0)
                                {
                                    TransacaoFinanceira tran = BusinessFinanceiro.getTransacaoFinanceira((int)b.Titulo.cd_origem_titulo, transacao.cd_pessoa_empresa);

                                    if (tran != null)
                                    {
                                        Cheque cheque = BusinessFinanceiro.getChequeTransacao((int)b.Titulo.cd_origem_titulo, transacao.cd_pessoa_empresa);

                                        if(cheque == null)
                                        {
                                            cheque = BusinessFinanceiro.addCheque(b.Titulo.Cheque);

                                            ChequeTransacao chequeTran = new ChequeTransacao();
                                            chequeTran.cd_cheque = cheque.cd_cheque;
                                            chequeTran.cd_tran_finan = (int) b.Titulo.cd_origem_titulo;
                                            chequeTran.dt_bom_para = (DateTime)cheque.dt_bom_para;
                                            chequeTran.nm_cheque = cheque.nm_primeiro_cheque;
                                            BusinessFinanceiro.addChequeTransacao(chequeTran);

                                            b.Titulo.Cheque = BusinessFinanceiro.getChequeTransacao((int)b.Titulo.cd_origem_titulo, transacao.cd_pessoa_empresa); ;

                                        }
                                        
                                    }


                                    
                                }
                                

                            }

                            b.ChequeBaixa.Add(new ChequeBaixa
                            {
                                dt_bom_para = ((DateTime)b.Titulo.Cheque.dt_bom_para).Date,
                                nm_cheque = b.Titulo.Cheque.nm_primeiro_cheque,
                                cd_cheque = b.Titulo.Cheque.cd_cheque
                            });
                        }
                        else
                            throw new FinanceiroBusinessException(string.Format(Utils.Messages.Messages.msgErroFaltaInforCheque), null, FinanceiroBusinessException.TipoErro.ERRO_DADOS_CHEQUE, false);
                    }
                }
            }

            TransacaoFinanceira transacaoFinanceira = new TransacaoFinanceira();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED, DaoParametro.DB(), TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                Parametro parametros = DaoParametro.getParametrosByEscola(transacao.cd_pessoa_empresa);

                transacaoFinanceira = BusinessFinanceiro.postIncluirTransacao(transacao, false, false, parametros);
                if (transacaoFinanceira.Baixas != null)
                    parametros.nm_ultimo_recibo = transacaoFinanceira.Baixas.OrderByDescending(x => x.nm_recibo).FirstOrDefault().nm_recibo;
                DaoParametro.saveChanges(false);
                transaction.Complete();
            }
            return transacaoFinanceira;
        }


        public TransacaoFinanceira editTransacao(TransacaoFinanceira transacao)
        {
            this.sincronizarContextos(DaoParametro.DB());
            if (transacao.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.CHEQUE_PRE_DATADO || transacao.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.CHEQUE_AVISTA)
            {
                if (transacao.cheque != null)
                {
                    if (!Cheque.validarDadosCheque(transacao.cheque, true))
                        throw new FinanceiroBusinessException(string.Format(Utils.Messages.Messages.msgErroFaltaInforCheque), null, FinanceiroBusinessException.TipoErro.ERRO_DADOS_CHEQUE, false);
                    transacao.ChequeTransacaoFinanceira.Add(new ChequeTransacao
                    {
                        dt_bom_para = ((DateTime)transacao.cheque.dt_bom_para).Date,
                        nm_cheque = transacao.cheque.nm_primeiro_cheque,
                        Cheque = transacao.cheque
                    });
                }
                else
                {
                    foreach (BaixaTitulo b in transacao.Baixas)
                    {
                        if (b.Titulo.Cheque != null){
                            if (b.Titulo.Cheque.dt_bom_para == null && b.Titulo.dt_vcto_titulo != null)
                                b.Titulo.Cheque.dt_bom_para = b.Titulo.dt_vcto_titulo;
                            if (string.IsNullOrEmpty(b.Titulo.Cheque.nm_primeiro_cheque) && !string.IsNullOrEmpty(b.Titulo.dc_num_documento_titulo))
                                b.Titulo.Cheque.nm_primeiro_cheque = b.Titulo.dc_num_documento_titulo;

                            if (!Cheque.validarDadosCheque(b.Titulo.Cheque, true))
                                throw new FinanceiroBusinessException(string.Format(Utils.Messages.Messages.msgErroFaltaInforCheque), null, FinanceiroBusinessException.TipoErro.ERRO_DADOS_CHEQUE, false);

                            if (!DataAccessMovimento.verificaTipoFinanceiroMovimento(b.Titulo.cd_titulo, b.Titulo.dc_tipo_titulo, transacao.cd_pessoa_empresa))
                                throw new FinanceiroBusinessException(string.Format(Utils.Messages.Messages.msgErroTipoFinanceiroDiferente), null, FinanceiroBusinessException.TipoErro.ERRO_TIPO_FINANCEIRO_DIFERENTE, false);
                            if (b.Titulo.dc_tipo_titulo == "AD"){
                                Cheque cheque = new Cheque();
                                b.Titulo.Cheque.cd_contrato = b.Titulo.cd_origem_titulo;
                                cheque = BusinessFinanceiro.getChequeByContrato((int)b.Titulo.cd_origem_titulo);
                                if (cheque != null && cheque.cd_cheque > 0)
                                {
                                    cheque.copy(b.Titulo.Cheque);
                                    BusinessFinanceiro.editCheque(cheque);
                                }
                                else {
                                    BusinessFinanceiro.addCheque(b.Titulo.Cheque);
                                    cheque = BusinessFinanceiro.getChequeByContrato((int)b.Titulo.cd_origem_titulo);
                                }
                                b.Titulo.Cheque.cd_cheque = cheque.cd_cheque;
                            }
                            b.ChequeBaixa.Add(new ChequeBaixa
                            {
                                dt_bom_para = ((DateTime)b.Titulo.Cheque.dt_bom_para).Date,
                                nm_cheque = b.Titulo.Cheque.nm_primeiro_cheque,
                                cd_cheque = b.Titulo.Cheque.cd_cheque
                            });
                        }
                        else
                            throw new FinanceiroBusinessException(string.Format(Utils.Messages.Messages.msgErroFaltaInforCheque), null, FinanceiroBusinessException.TipoErro.ERRO_DADOS_CHEQUE, false);
                    }
                }
            }
            else
            {
                transacao.cheque = null;
                transacao.ChequeTransacaoFinanceira = null;
                foreach (BaixaTitulo b in transacao.Baixas)
                {
                    b.Titulo.Cheque = null;
                    b.ChequeBaixa = null;
                }

            }

            TransacaoFinanceira transacaoFinanceira = new TransacaoFinanceira();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED, DaoParametro.DB(), TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                Parametro parametros = DaoParametro.getParametrosByEscola(transacao.cd_pessoa_empresa);
                transacaoFinanceira = BusinessFinanceiro.editTransacao(transacao, false, parametros);
                transaction.Complete();
            }
            return transacaoFinanceira;
        }
        #endregion

        #region Diário de Aula

        public ProgramacoesTurmaSemDiarioAula getProgramacoesTurmasSemDiarioAula(int cd_turma, int cd_escola)
        {
            ProgramacoesTurmaSemDiarioAula retorno = new ProgramacoesTurmaSemDiarioAula();

            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED, DaoParametro.DB(), TransactionScopeBuilder.TransactionTime.MODEREATE))
            {

                byte? qtd_aulas_sem_material = DaoParametro.getParametroNmAulasSemMaterial(cd_escola);
                Byte nm_dias_titulos_abertos = DaoParametro.getParametroNmDiasTitulosAbertos(cd_escola);
                if (qtd_aulas_sem_material == null)
                    qtd_aulas_sem_material = 0;
                retorno = BusinessTurma.getProgramacoesTurmasSemDiarioAula(cd_turma, cd_escola, (int)qtd_aulas_sem_material, nm_dias_titulos_abertos);

                transaction.Complete();
            }
            return retorno;
        }

        public ProgramacoesTurmaSemDiarioAula verificarAlunosPendenciaMaterialDidaticoCurso(int cd_turma, int cd_escola, DateTime dt_programacao_turma)
        {
            ProgramacoesTurmaSemDiarioAula retorno = new ProgramacoesTurmaSemDiarioAula();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED, DaoParametro.DB(), TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                byte? qtd_aulas_sem_material = DaoParametro.getParametroNmAulasSemMaterial(cd_escola);
                if (qtd_aulas_sem_material == null)
                    qtd_aulas_sem_material = 0;
                retorno = BusinessTurma.verificarAlunosPendenciaMaterialDidaticoCurso((int)qtd_aulas_sem_material, cd_turma, cd_escola, dt_programacao_turma);
            }
            return retorno;
        }

        #endregion

        #region Follow-Up

        public IEnumerable<PessoaSearchUI> getEscolasFollowUp(int cd_follow_up, int cd_escola)
        {
            return Dao.getEscolasFollowUp(cd_follow_up, cd_escola);
        }


        #endregion

        #region Mail Marketing

        public String getRodapeSysApp()
        {
            return DaoSysApp.getRodapeSysApp();
        }

        public SysApp getConfigEmailMarketingSysApp()
        {
            return DaoSysApp.getConfigEmailMarketingSysApp();
        }

        public string getVersoCartaoPostal()
        {
            return DaoSysApp.getVersoCartaoPostal();
        }

        public SysApp putConfigEmailMarketingSysApp(SysApp sysApp)
        {
            SysApp alterarReg = DaoSysApp.getSysApp();
            alterarReg.tx_msg_email = sysApp.tx_msg_email;
            alterarReg.tx_verso_cartao_postal = sysApp.tx_verso_cartao_postal;
            DaoSysApp.saveChanges(false);
            return alterarReg;
        }

        #endregion

        #region Reajuste Anual

        public ReajusteAnual abrirFecharReajusteAnual(ReajusteAnual reajuste, int cd_usuario)
        {
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED, DaoParametro.DB(), TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                this.sincronizarContextos(DaoParametro.DB());
                ReajusteAnual reajusteContext = BusinessFinanceiro.getReajusteAnualFull(reajuste.cd_reajuste_anual, reajuste.cd_pessoa_escola);
                if (reajusteContext.id_status_reajuste == (int)ReajusteAnual.StatusReajuste.ABERTO)
                {
                    processarEDesprocessarReajusteAnual(reajusteContext, ReajusteAnual.StatusReajuste.FECHADO, cd_usuario);
                    reajusteContext.id_status_reajuste = (byte)ReajusteAnual.StatusReajuste.FECHADO;
                }
                else
                {
                    processarEDesprocessarReajusteAnual(reajusteContext, ReajusteAnual.StatusReajuste.ABERTO, cd_usuario);
                    reajusteContext.id_status_reajuste = (byte)ReajusteAnual.StatusReajuste.ABERTO;
                }
                DaoParametro.saveChanges(false);
                transaction.Complete();
            }
            return BusinessFinanceiro.getReajusteAnualGridView(reajuste.cd_reajuste_anual, reajuste.cd_pessoa_escola);
        }

        public void processarEDesprocessarReajusteAnual(ReajusteAnual reajuste, ReajusteAnual.StatusReajuste status, int cd_usuario)
        {
            if (status == ReajusteAnual.StatusReajuste.FECHADO)
            {
                if (!reajuste.dt_final_vencimento.HasValue)
                    reajuste.dt_final_vencimento = DateTime.MaxValue;
                List<Titulo> titulos = BusinessFinanceiro.getTitulosReajusteAnual(reajuste.cd_pessoa_escola, reajuste.cd_reajuste_anual, reajuste.dt_inicial_vencimento, reajuste.dt_final_vencimento).ToList();
                if (reajuste.dt_final_vencimento == DateTime.MaxValue)
                    reajuste.dt_final_vencimento = null;
                var grupoContrato = titulos.GroupBy(p => p.cd_origem_titulo).Select(g => new { cd_origem_titulo = g.Key.Value, Titulos = g }).ToList();
                foreach (var grupo in grupoContrato)
                {
                    decimal vl_total_pc_bolsa_aplicado = 0;
                    TransacaoFinanceira trasFina = new TransacaoFinanceira();
                    List<int> cds_titulos_reaj = grupo.Titulos.Select(x => x.cd_titulo).ToList();
                    List<Titulo> titulosContext = BusinessFinanceiro.getTitulosContrato(cds_titulos_reaj, reajuste.cd_pessoa_escola).ToList();
                    Contrato contrato = BusinessMatricula.getContratoReajusteAnual((int)grupo.cd_origem_titulo, reajuste.cd_pessoa_escola);
                    List<BaixaTitulo> baixaBolsaTitulos = BusinessFinanceiro.getBaixaTitulosBolsaContrato(contrato.cd_contrato, reajuste.cd_pessoa_escola, cds_titulos_reaj).ToList();
                    foreach (Titulo t in titulosContext)
                    {
                        decimal vl_pc_bolsa_aplicado = 0;
                        Titulo tituloGrupo = (Titulo)grupo.Titulos.Where(x => x.cd_titulo == t.cd_titulo).FirstOrDefault().Clone();
                        tituloGrupo.vl_saldo_titulo = t.vl_saldo_titulo;
                        tituloGrupo.vl_titulo = t.vl_titulo;
                        //Retira o valor do material para fazer o cálculo
                        decimal vl_titulo = t.vl_material_titulo == 0 ? t.vl_titulo : t.vl_titulo - t.vl_material_titulo;
                        if (reajuste.vl_reajuste_anual > 0)
                            t.vl_titulo += reajuste.vl_reajuste_anual;
                        else
                            t.vl_titulo = vl_titulo + decimal.Round(vl_titulo * (decimal)reajuste.pc_reajuste_anual / 100, 2);
                        if ((decimal)contrato.pc_desconto_bolsa > 0)
                        {
                            vl_pc_bolsa_aplicado = decimal.Round(t.vl_titulo * (decimal)contrato.pc_desconto_bolsa / 100, 2);
                            if (t.BaixaTitulo != null && t.BaixaTitulo.Count() > 0)
                            {
                                t.vl_liquidacao_titulo = vl_pc_bolsa_aplicado;
                                BaixaTitulo b = t.BaixaTitulo.FirstOrDefault();
                                b.vl_liquidacao_baixa = vl_pc_bolsa_aplicado;
                                b.vl_principal_baixa = t.vl_material_titulo > 0 ? t.vl_titulo + t.vl_material_titulo : t.vl_titulo;
                                b.vl_baixa_saldo_titulo = vl_pc_bolsa_aplicado;
                                b.vl_desconto_baixa_calculado = t.vl_titulo - vl_pc_bolsa_aplicado;
                            }
                        }
                        if (t.vl_material_titulo > 0)
                            t.vl_titulo += t.vl_material_titulo;
                        t.vl_saldo_titulo = t.vl_titulo - vl_pc_bolsa_aplicado;
                        if (t.PlanoTitulo != null && t.PlanoTitulo.Count() > 0)
                            t.PlanoTitulo.FirstOrDefault().vl_plano_titulo = t.vl_titulo;
                        t.ReajustesTitulos.Add(new ReajusteTitulo
                        {
                            cd_reajuste_anual = reajuste.cd_reajuste_anual,
                            cd_titulo = t.cd_titulo,
                            vl_original_saldo_titulo = tituloGrupo.vl_saldo_titulo,
                            vl_original_titulo = tituloGrupo.vl_titulo,
                            vl_reajustado_titulo = t.vl_saldo_titulo,
                            cd_turma = tituloGrupo.cd_turma_titulo > 0 ? tituloGrupo.cd_turma_titulo : null,
                            cd_curso = tituloGrupo.cd_curso > 0 ? tituloGrupo.cd_curso : null,
                            cd_aluno = tituloGrupo.cd_aluno > 0 ? (Nullable<int>)tituloGrupo.cd_aluno : null
                        });
                        vl_total_pc_bolsa_aplicado += vl_pc_bolsa_aplicado;
                    }

                    //DataAccessAditamento.saveChanges(false);
                    decimal vl_saldo_titulo_outras_parcelas = BusinessFinanceiro.getSaldoContratoParaReajusteAnual(reajuste.cd_pessoa_escola, (decimal)contrato.pc_desconto_bolsa, contrato.cd_contrato, cds_titulos_reaj);
                    decimal vl_aditivo_calc = titulosContext.Sum(x => x.vl_saldo_titulo - x.vl_material_titulo);
                    Aditamento adt = new Aditamento()
                    {
                        cd_contrato = contrato.cd_contrato,
                        cd_nome_contrato = reajuste.cd_nome_contrato,
                        cd_reajuste_anual = reajuste.cd_reajuste_anual,
                        dt_aditamento = DateTime.UtcNow,
                        dt_inicio_aditamento = reajuste.dt_inicial_vencimento.Date,
                        cd_usuario = cd_usuario,
                        id_tipo_data_inicio = (byte)Aditamento.TipoDataInicioEnum.ESCOLHER_DATA,
                        id_tipo_aditamento = (int)Aditamento.TipoAditamento.REAJUSTE_ANUAL,
                        vl_aditivo = decimal.Round(vl_aditivo_calc + vl_saldo_titulo_outras_parcelas + vl_total_pc_bolsa_aplicado, 2),
                        tx_obs_aditamento = "Gerado pelo Reajuste Anual número " + reajuste.cd_reajuste_anual,
                        cd_tipo_financeiro = contrato.cd_tipo_financeiro
                    };
                    DataAccessAditamento.addContext(adt, false);
                }
                DataAccessAditamento.saveChanges(false);
            }
            else
            {
                if (BusinessFinanceiro.verificaTitulosFechamentoReajusteAnual(reajuste.cd_pessoa_escola, reajuste.cd_reajuste_anual))
                    throw new FinanceiroBusinessException(Utils.Messages.Messages.msgErroTituloFechadoReajusteAnual, null, FinanceiroBusinessException.TipoErro.ERRO_TITULO_FECHADO_REAJUSTE_ANUAL, false);
                //List<int> cdsContratos = BusinessFinanceiro.getCodigoContratoTitulosReajusteAnual(reajuste.cd_pessoa_escola, reajuste.cd_reajuste_anual);
                if (BusinessMatricula.verificaAditamentoAposReajusteAnual(reajuste.cd_pessoa_escola, reajuste.cd_reajuste_anual))
                    throw new FinanceiroBusinessException(Utils.Messages.Messages.msgErroAditamentoAposReajusteAnual, null, FinanceiroBusinessException.TipoErro.ERRO_TITULO_FECHADO_REAJUSTE_ANUAL, false);
                BusinessFinanceiro.reverterAlteracaoTituloReajusteAnual(reajuste.cd_pessoa_escola, reajuste.cd_reajuste_anual);
                BusinessMatricula.deletarAditamentosReajusteAnual(reajuste.cd_pessoa_escola, reajuste.cd_reajuste_anual);
                DaoParametro.saveChanges(false);
            }
        }

        public bool postMontaNFMaterial(Contrato contrato)
        {
            bool retorno = false;
            bool executar = false;

            if (executar)
            {
                List<NotasVendaMaterialUI> Notas = new List<NotasVendaMaterialUI>();
                if (contrato.CursoContrato != null && contrato.CursoContrato.Count() > 0)
                    foreach (CursoContrato d in contrato.CursoContrato.Where(x => x.cd_curso == contrato.cd_curso_atual).ToList())
                    {
                        int existeNota = BusinessFinanceiro.findNotaAluno(contrato.cd_aluno, d.cd_curso);
                        //Se existir Nota já gerada em outra matrícula não gera novamente
                        if (existeNota == 0 || existeNota == 2 && contrato.cd_regime_atual == 2)
                        {
                            if (contrato.notas_material_didatico != null && contrato.notas_material_didatico.Count() > 0)
                                Notas = contrato.notas_material_didatico.Where(x => x.cd_curso == d.cd_curso && !x.id_venda_futura).ToList();
                            if (Notas.Count() == 0 || existeNota == 2)
                            {
                                retorno = false;
                            }
                        }
                        if (existeNota == 1) retorno = true;
                    }
                return retorno;
            }
            else
            throw new EscolaBusinessException("Opção não mais disponível", null, EscolaBusinessException.TipoErro.ERRO_OPCAO_INVALIDA, false);
        }

        public DataTable getLoginEscola(DateTime dt_analise, bool id_login, byte id_matricula)
        {
            return Dao.getLoginEscola(dt_analise, id_login, id_matricula);
        }

        #endregion
    }
}

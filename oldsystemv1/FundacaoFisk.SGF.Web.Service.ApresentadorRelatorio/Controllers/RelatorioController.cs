using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Secretaria.Business;
using FundacaoFisk.SGF.Web.Services.Secretaria.Comum;
using FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess;
using log4net;
using FundacaoFisk.SGF.Web.Services.ApresentadorRelatorio;
using System.Collections;
using FundacaoFisk.SGF.Utils;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Business;
using FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess;
using FundacaoFisk.SGF.Web.Services.InstituicaoEnsino.Business;
using FundacaoFisk.SGF.Web.Services.InstituicaoEnsino.DataAccess;
using FundacaoFisk.SGF.Web.Services.Usuario.Business;
using FundacaoFisk.SGF.Web.Services.Usuario.DataAccess;
using FundacaoFisk.SGF.Web.Services.Pessoa.Business;
using FundacaoFisk.SGF.Web.Services.Pessoa.DataAccess;
using FundacaoFisk.SGF.Web.Services.Usuario.Comum;
using FundacaoFisk.SGF.Web.Services.Auth.Business;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Financeiro.Business;
using FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess;
using System.Web;
using FundacaoFisk.SGF.Web.Services.Pessoa.Comum.Business;
using FundacaoFisk.SGF.Web.Services.Pessoa.Comum.DataAccess;
using Componentes.GenericController;
using FundacaoFisk.SGF.Web.Services.InstituicaoEnsino.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Empresa.Business;
using FundacaoFisk.SGF.Web.Services.Empresa.DataAccess;
using FundacaoFisk.SGF.Web.Services.Empresa.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Financeiro.Model;
using System.Globalization;
using Word = Microsoft.Office.Interop.Word;
using System.IO;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Biblioteca.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Biblioteca.Business;
using FundacaoFisk.SGF.Web.Services.Biblioteca.DataAccess;
using FundacaoFisk.SGF.Web.Services.Log.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Log.Business;
using FundacaoFisk.SGF.Web.Services.Log.DataAccess;
using FundacaoFisk.SGF.Web.Services.CNAB.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.CNAB.Business;
using FundacaoFisk.SGF.Web.Services.CNAB.DataAccess;
using FundacaoFisk.SGF.Web.Services.CNAB.Model;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;
using FundacaoFisk.SGF.Utils.Messages;
using Componentes.GenericBusiness;
using FundacaoFisk.SGF.Web.Services.Log;
using FundacaoFisk.SGF.Web.Services.EmailMarketing.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.EmailMarketing.Business;
using FundacaoFisk.SGF.Web.Service.EmailMarketing.DataAccess;
using FundacaoFisk.SGF.Web.Service.EmailMarketing.Model;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Services.Coordenacao.DataAccess;
using System.Data.SqlClient;
using DataTable = System.Data.DataTable;
using FundacaoFisk.SGF.Services.CNAB.DataAccess;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using FundacaoFisk.SGF.Services.Coordenacao.Business;
using Paragraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;
using Table = DocumentFormat.OpenXml.Wordprocessing.Table;
using FundacaoFisk.SGF.Web.Services.ApresentadorRelatorio.Controllers;
using FundacaoFisk.SGF.Services.InstituicaoEnsino.DataAccess;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using Microsoft.Office.Interop.Word;

namespace FundacaoFisk.SGF.Web.Services.ApresentadorRelatorio
{


    public class RelatorioController : ComponentesApiController
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(RelatorioController));
        private ISecretariaBusiness BusinessSecretaria { get; set; }
        private IPermissaoBusiness BusinessPermissao { get; set; }
        private IEscolaBusiness BusinessEscola { get; set; }
        private ICoordenacaoBusiness BusinessCoordenacao { get; set; }
        private IFinanceiroBusiness BusinessFinanceiro { get; set; }
        private IUsuarioBusiness BusinessUsuario { get; set; }
        private ILocalidadeBusiness BusinessLocalidade { get; set; }
        private IPessoaBusiness BusinessPessoa { get; set; }
        private IProfessorBusiness BusinessProfessor { get; set; }
        private IEmpresaBusiness BusinessEmpresa { get; set; }
        private ICursoBusiness BusinessCurso { get; set; }
        private IAlunoBusiness BusinessAluno { get; set; }
        private IFuncionarioBusiness BusinessFuncionario { get; set; }
        private ITurmaBusiness BusinessTurma { get; set; }
        private IMatriculaBusiness BusinessMatricula { get; set; }
        private IBibliotecaBusiness BusinessBiblioteca { get; set; }
        private ICnabBusiness BusinessCnab { get; set; }
        private IFiscalBusiness BusinessFiscal { get; set; }
        private ILogGeralBusiness BusinessLogGeral { get; set; }
        private IEmailMarketingBusiness BusinessMarketing { get; set; }
        private IApiNewCyberBusiness BusinessApiNewCyber { get; set; }
        private IApiNewCyberAlunoBusiness BusinessApiNewCyberAluno { get; set; }
        private IApiAreaRestritaBusiness BusinessApiAreaRestrita { get; set; }
        private IApiNewCyberFuncionarioBusiness BusinessApiNewCyberFuncionario { get; set; }
        private IApiPromocaoIntercambioBusiness BusinessApiPromocaoIntercambio { get; set; }
        private IApiPromocaoIntercambioProspectBussiness BusinessApiPromocaoIntercambioProspect { get; set; }
        
        private SGFWebContext db = new SGFWebContext();

        public RelatorioController()
        {
            BusinessApiNewCyber = new ApiNewCyberBusiness(new TurmaDataAccess(), new EmpresaDataAccess());

            BusinessApiNewCyberAluno = new ApiNewCyberAlunoBusiness();

            BusinessApiNewCyberFuncionario = new ApiNewCyberFuncionarioBusiness();

            BusinessApiPromocaoIntercambio = new ApiPromocaoIntercambioBusiness(new TurmaDataAccess(), new EmpresaDataAccess());
            BusinessApiPromocaoIntercambioProspect = new ApiPromocaoIntercambioProspectBusiness();

            BusinessLocalidade = new LocalidadeBusiness(new LocalidadeDataAccess(), new PaisDataAccess(), new EstadoDataAccess(),
                    new TipoEnderecoDataAccess(), new ClasseTelefoneDataAccess(), new TipoLogradouroDataAccess(), new TipoTelefoneDataAccess(),
                    new OperadoraDataAccess(), new AtividadeDataAccess(), new EnderecoDataAccess());

            BusinessPessoa = new PessoaBusiness(new FundacaoFisk.SGF.Web.Services.Pessoa.DataAccess.PessoaDataAccess(), new TelefoneDataAccess(),
                new EstadoCivilDataAccess(), new TratamentoPessoaDataAccess(), new OrgaoExpedidorDataAccess(), new PapelDataAccess(),
                new RelacionamentoDataAccess(),
                BusinessLocalidade, new PessoaFisicaDataAccess());
            BusinessUsuario = new UsuarioBusiness(new UsuarioDataAccess(), new UsuarioEmpresaDataAccess(), new DireitoUsuarioDataAccess(),
                new SysGrupoUsuarioDataAccess());
            ILogGeralBusiness BusinessLogGeral = new LogGeralBusiness(new LogGeralDataAccess(), new AtributosDataAccess());
            BusinessApiAreaRestrita = new ApiAreaRestritaBusiness(new MenuDataAccess(), new GrupoDataAccess(), new SysDireitoGrupoDataAccess(), new SysGrupoUsuarioDataAccess());
            BusinessEmpresa = new EmpresaBusiness(new EmpresaDataAccess(), BusinessPessoa, BusinessUsuario, BusinessLogGeral, BusinessApiNewCyberFuncionario, BusinessApiAreaRestrita, new UsuarioDataAccess());
            BusinessPermissao = new PermissaoBusiness(new MenuDataAccess(), new GrupoDataAccess(), new SysDireitoGrupoDataAccess(), new SysGrupoUsuarioDataAccess());
            
            BusinessFinanceiro = new FinanceiroBusiness(new GrupoEstoqueDataAccess(), new MovimentacaoFinanceiraDataAccess(),
                new TipoLiquidacaoDataAccess(), new TipoFinanceiroDataAccess(), new TipoDescontoDataAccess(), new ItemDataAccess(), new ItemKitDataAccess(),
                new PoliticaDescontoDataAccess(), new ItemEscolaDataAccess(), new TipoItemDataAccess(), new BibliotecaDataAccess(),
                new TabelaPrecoDataAccess(), new GrupoContaDataAccess(), new SubgrupoContaDataAccess(), new PlanoContasDataAccess(),
                new ChequeDataAccess(), new BancoDataAccess(), new TituloDataAccess(), new PlanoTituloDataAccess(), new LocalMovtoDataAccess(),
                new TransacaoFinanceiraDataAccess(), new BaixaTituloDataAccess(), new ContaCorrenteDataAccess(), new MovimentoDataAccess(), new PoliticaComercialDataAccess(),
                new ItemMovimentoDataAccess(), new KardexDataAccess(), new ItemPoliticaDataAccess(), new DiasPoliticaDataAccess(), new FechamentoDataAccess(), new SaldoItemDataAccess(),
                new TipoDescontoEscolaDataAccess(), new ItemSubgrupoDataAccess(), new SituacaoTributariaDataAccess(), new AliquotaUFDataAccess(), 
                new DadosNFDataAccess(), BusinessEmpresa, new PoliticaAlunoDataAccess(), new PoliticaTurmaDataAccess(), new ObsSaldoCaixaDataAccess(), new ReajusteAnualDataAccess(),
                new ReajusteAlunoDataAccess(), new ReajusteCursoDataAccess(), new ReajusteTurmaDataAccess(), new ReajusteTituloDataAccess(), new ChequeTransacaoDataAccess() ,
                new ChequeBaixaDataAccess(), new TaxaBancariaDataAccess(), new TituloAditamentoDataAccess(), new BaixaAutomaticaDataAccess(), new TitulosBaixaAutomaticaDataAccess(),
                new OrgaoFinanceiroDataAccess());

            BusinessFiscal = new FiscalBusiness(new ItemMovimentoDataAccess(), new ItemMovimentoKitDataAccess(), new ItemMovItemKitDataAccess(), new MovimentoDataAccess(), new ItemEscolaDataAccess(),
                            new TituloDataAccess(), new PlanoTituloDataAccess(), new TipoItemDataAccess(), BusinessFinanceiro, new CFOPDataAccess(), new TipoNotaFiscalDataAccess(), BusinessEmpresa);
            BusinessAluno = new AlunoBusiness(new AlunoDataAccess(), BusinessLocalidade, BusinessPessoa,
              new AlunoMotivoMatriculaDataAccess(), new HorarioDataAccess(), new AlunoTurmaDataAccess(), new AlunoBolsaDataAccess(),
              new HistoricoAlunoDataAccess(), BusinessEmpresa, BusinessFiscal, BusinessFinanceiro, new MatriculaDataAccess(), new ProspectDataAccess(), new PessoaRafDataAccess(), BusinessApiNewCyberAluno, new FichaSaudeDataAccess());
            BusinessSecretaria = new SecretariaBusiness(new EscolaridadeDataAccess(), new MidiaDataAccess(), new TipoContatoDataAccess(),
                new MotivoMatriculaDataAccess(), new MotivoNaoMatriculaDataAccess(), new MotivoBolsaDataAccess(),
                new MotivoCancelamentoBolsaDataAccess(), BusinessLocalidade, BusinessPessoa, new JsonTesteDataAccess(),
                new ProspectDataAccess(), new ProspectDiaDataAccess(), new ProspectProdutoDataAccess(), new ProspectPeriodoDataAccess(), new FollowUpDataAccess(),
                new ProspectMotivoNaoMatriculaDataAccess(), BusinessEmpresa, new NomeContratoDataAccess(), new HistoricoAlunoDataAccess(), new DesistenciaDataAccess(), BusinessAluno, BusinessFinanceiro,
                new AcaoFollowupDataAccess(), new FollowUpEscolaDataAccess(), new FollowUpUsuarioDataAccess(), new AnoEscolarDataAccess(),
                new MatriculaDataAccess(), new GeraNotasXmlDataAccess(), //new DataAccessSms(), new DataAccessMensagemPadraoSms(), 
                new AlunoDataAccess(), new OrgaoFinanceiroDataAccess(), 
                new AlunoRestricaoDataAccess(), BusinessApiNewCyberAluno, new MotivoTransferenciaDataAccess(),new TransferenciaAlunoDataAccess(), BusinessApiPromocaoIntercambioProspect, new PessoaPromocaoDataAccess());
            BusinessMatricula = new MatriculaBusiness(new MatriculaDataAccess(),  BusinessFinanceiro, new DescontoContratoDataAccess(),
                                                      new TaxaMatriculaDataAccess(), new AditamentoDataAccess(), BusinessSecretaria, new ProspectDataAccess(),
                                                      new AditamentoBolsaDataAccess(), new CursoContratoDataAccess(), new AlunoTurmaDataAccess());
            BusinessFuncionario = new FuncionarioBusiness(new FuncionarioDataAccess(), BusinessPessoa, new FuncionarioComissaoDataAccess());
            BusinessProfessor = new ProfessorBusiness(BusinessPessoa, new ProfessorDataAccess(), BusinessSecretaria,
                BusinessFuncionario, new HorarioProfessorTurmaDataAccess(), new ProfessorTurmaDataAccess(), new ProdutoFuncionarioDataAccess(), new ProdutoDataAccess(), new AlunoDataAccess(), new FuncionarioDataAccess(), BusinessApiNewCyberFuncionario);
            BusinessCurso = new CursoBusiness(BusinessFinanceiro, new Coordenacao.DataAccess.CursoDataAccess());
            BusinessTurma = new TurmaBusiness(new TurmaDataAccess(), BusinessSecretaria, new FeriadoDataAccess(),
                                   new ProfessorTurmaDataAccess(), new ProgramacaoTurmaDataAccess(), new DiarioAulaDataAccess(), BusinessProfessor,
                                   new AvaliacaoTurmaDataAccess(), new AvaliacaoDataAccess(), new AvaliacaoAlunoDataAccess(),
                                   BusinessAluno, new FeriadoDesconsideradoDataAccess(), BusinessFinanceiro, new AvaliacaoAlunoParticipacaoDataAccess(), BusinessLogGeral, BusinessApiNewCyber);

            BusinessCoordenacao = new CoordenacaoBusiness(new EstagioDataAccess(), new ConceitoDataAccess(), new DuracaoDataAccess(), new EventoDataAccess(),
                new ModalidadeDataAccess(), new MotivoDesistenciaDataAccess(), new MotivoFaltaDataAccess(), new ProdutoDataAccess(),
                new RegimeDataAccess(), new SalaDataAccess(), new ControleFaltasDataAccess(), new ControleFaltasAlunoDataAccess(), new TipoAtividadeExtraDataAccess(),  new CriterioAvaliacaoDataAccess(),
                new TipoAvaliacaoDataAccess(), new ProgramacaoCursoDataAccess(), new AvaliacaoTurmaDataAccess(),
                new AtividadeExtraDataAccess(), new AtividadeAlunoDataAccess(), new AtividadeCursoDataAccess(), new AvaliacaoCursoDataAccess(), 
                BusinessFinanceiro, BusinessAluno, BusinessTurma, BusinessCurso, BusinessSecretaria, new AvaliacaoDataAccess(), new AlunoEventoDataAccess(), new ProgramacaoTurmaDataAccess(),
                BusinessMatricula, new ItemProgramacaoCursoDataAccess(), new AulaPersonalizadaDataAccess(), new AulaPersonalizadaAlunoDataAccess(),
                new AvaliacaoParticipacaoDataAccess(), new AvaliacaoParticipacaoVincDataAccess(), new ParticipacaoDataAccess(), new VideoDataAccess(), 
                new NivelDataAccess(), new FaqDataAccess(),new CalendarioEventoDataAccess(), new CalendarioAcademicoDataAccess(), new CargaProfessorDataAccess(), 
                new AvaliacaoAlunoParticipacaoDataAccess(), new CircularDataAccess(), new TituloAditamentoDataAccess(), new AulaReposicaoDataAccess(), new AlunoAulaReposicaoDataAccess(), new TurmaEscolaDataAccess(), new AtividadeEscolaAtividadeDataAccess(), new AtividadeProspectDataAccess(),
                new AtividadeRecorrenciaDataAccess(), BusinessApiNewCyber, new MensageAvaliacaoDataAccess(), new MensagemAvaliacaoAlunoDataAccess(), new PerdaMaterialDataAccess());

            BusinessBiblioteca = new BibliotecaBusiness(BusinessFinanceiro, new EmprestimoDataAccess());
            BusinessCnab = new CnabBusiness(new CarteiraCnabDataAccess(), new CnabDataAccess(), new RetornoCNABDataAccess(), new TituloCnabDataAccess(), BusinessFinanceiro, new TituloRetornoCNABDataAccess(), new DespesaTituloCnabDataAccess(), new TituloDataAccess());
            BusinessEscola = new EscolaBusiness(new EscolaDataAccess(), new UsuarioDataAccess(), BusinessUsuario,
                BusinessPessoa, BusinessLocalidade, new AuthBusiness(BusinessUsuario, BusinessPermissao, new AesCryptoHelper()),
                new ParametrosDataAccess(), BusinessEmpresa, BusinessAluno, BusinessCoordenacao, BusinessMatricula, BusinessFinanceiro, BusinessTurma, BusinessSecretaria,
                BusinessBiblioteca, BusinessCurso, BusinessFiscal, new SysAppDataAccess(), new AditamentoDataAccess(), new MovimentoDataAccess(), new PessoaEscolaDataAccess(), new ItemMovimentoDataAccess(),  
                new ItemEscolaDataAccess(), new TipoItemDataAccess(), new TipoNotaFiscalDataAccess(), BusinessFinanceiro, new PlanoTituloDataAccess(), new TituloDataAccess(), new LocalMovtoDataAccess(), 
                new TurmaEscolaEmpresaDataAccess(), new AtividadeEscolaAtividadeEmpresaDataAccess(), BusinessApiNewCyber, BusinessApiPromocaoIntercambio, new EmpresaValorServicoDataAccess(), BusinessApiAreaRestrita);
            BusinessMarketing = new EmailMarketingBusiness(BusinessLogGeral, new ListaEnderecoMalaDataAccess(), new ListaNaoInscritoDataAccess(), new MalaDiretaDataAccess());
        }

        public IEnumerable<TO> GetSource(TipoRelatorioSGFEnum enumTipoRelatorio, Hashtable parametros)
        {
            IEnumerable<TO> retorno = null;
            try
            {
                SearchParameters parametrosSearch = new SearchParameters(0, int.MaxValue, int.MaxValue);
                var sortOrder = (int)SortDirection.Ascending;

                if (parametros["so"] != null && (string)parametros["so"] != "")
                    sortOrder = int.Parse(parametros["so"] + "");

                parametrosSearch.sort = parametros["cs"] + "";
                parametrosSearch.sort = !string.Empty.Equals(parametrosSearch.sort) ? parametrosSearch.sort : null;
                if ((int)SortDirection.Ascending == sortOrder)
                    parametrosSearch.sortOrder = SortDirection.Ascending;
                else
                    parametrosSearch.sortOrder = SortDirection.Descending;

                switch (enumTipoRelatorio)
                {
                    //********Auxiliares Pessoas********\\
                    case TipoRelatorioSGFEnum.PaisSearch:
                        var descricao = parametros["descricao"] + "";
                        var inicio = bool.Parse(parametros["inicio"] + "");
                        retorno = BusinessLocalidade.GetPaisSearch(parametrosSearch, descricao, inicio);
                        break;
                    case TipoRelatorioSGFEnum.EstadoSearch:
                        descricao = parametros["descricao"] + "";
                        inicio = bool.Parse(parametros["inicio"] + "");
                        var cdPais = int.Parse(parametros["cdPais"] + "");
                        retorno = BusinessLocalidade.GetEstadoSearch(parametrosSearch, descricao, inicio, cdPais);
                        break;
                    case TipoRelatorioSGFEnum.CidadeSearch:
                        descricao = parametros["descricao"] + "";
                        inicio = bool.Parse(parametros["inicio"] + "");
                        int nmMunicipio = int.Parse(parametros["nmMunicipio"] + "");
                        int cdEstado = int.Parse(parametros["cdEstado"] + "");
                        retorno = BusinessLocalidade.GetCidadeSearch(parametrosSearch, descricao, inicio, nmMunicipio, cdEstado);
                        break;
                    case TipoRelatorioSGFEnum.BairroSearch:
                        descricao = parametros["descricao"] + "";
                        inicio = bool.Parse(parametros["inicio"] + "");
                        int cd_cidade_bairro = int.Parse(parametros["cd_cidade"] + "");
                        retorno = BusinessLocalidade.GetBairroSearch(parametrosSearch, descricao, inicio, cd_cidade_bairro);
                        break;
                    case TipoRelatorioSGFEnum.DistritoSearch:
                        descricao = parametros["descricao"] + "";
                        inicio = bool.Parse(parametros["inicio"] + "");
                        int cd_cidade_Distrito = int.Parse(parametros["cd_cidade"] + "");
                        retorno = BusinessLocalidade.GetDistritoSearch(parametrosSearch, descricao, inicio, cd_cidade_Distrito);
                        break;
                    case TipoRelatorioSGFEnum.OperadoraSearch:
                        descricao = parametros["descricao"] + "";
                        inicio = bool.Parse(parametros["inicio"] + "");
                        var status = int.Parse(parametros["status"] + "");
                        retorno = BusinessLocalidade.GetOperadoraSearch(parametrosSearch, descricao, inicio, base.getStatus(status));
                        break;
                    case TipoRelatorioSGFEnum.AtividadeSearch:
                        descricao = parametros["descricao"] + "";
                        inicio = bool.Parse(parametros["inicio"] + "");
                        status = int.Parse(parametros["status"] + "");
                        var natureza = int.Parse(parametros["natureza"] + "");
                        var cnae = parametros["cnae"] + "";
                        retorno = BusinessLocalidade.GetAtividadeSearch(parametrosSearch, descricao, inicio, base.getStatus(status), natureza, cnae);
                        break;
                    case TipoRelatorioSGFEnum.TipoLogradouroSearch:
                        descricao = parametros["descricao"] + "";
                        inicio = bool.Parse(parametros["inicio"] + "");
                        retorno = BusinessLocalidade.GetTipoLogradouroSearch(parametrosSearch, descricao, inicio);
                        break;
                    case TipoRelatorioSGFEnum.TipoEnderecoSearch:
                        descricao = parametros["descricao"] + "";
                        inicio = bool.Parse(parametros["inicio"] + "");
                        retorno = BusinessLocalidade.GetTipoEnderecoSearch(parametrosSearch, descricao, inicio);
                        break;
                    case TipoRelatorioSGFEnum.ClasseTelefoneSearch:
                        descricao = parametros["descricao"] + "";
                        inicio = bool.Parse(parametros["inicio"] + "");
                        retorno = BusinessLocalidade.GetClasseTelefoneSearch(parametrosSearch, descricao, inicio);
                        break;
                    case TipoRelatorioSGFEnum.TipoTelefoneSearch:
                        descricao = parametros["descricao"] + "";
                        inicio = bool.Parse(parametros["inicio"] + "");
                        retorno = BusinessLocalidade.GetTipoTelefoneSearch(parametrosSearch, descricao, inicio);
                        break;
                    case TipoRelatorioSGFEnum.Logradouro:
                        descricao = parametros["descricao"] + "";
                        inicio = bool.Parse(parametros["inicio"] + "");
                        var cd_estado = int.Parse(parametros["cd_estado"] + "");
                        var cd_cidade = int.Parse(parametros["cd_cidade"] + "");
                        var cd_bairro = int.Parse(parametros["cd_bairro"] + "");
                        var cep = parametros["cep"] + "";
                        retorno = BusinessLocalidade.getLogradouroSearch(parametrosSearch, descricao, inicio, cd_estado, cd_cidade, cd_bairro, cep);
                        break;
                    //********Secretaria********\\
                    case TipoRelatorioSGFEnum.EscolaridadeSearch:
                        descricao = parametros["descricao"] + "";
                        inicio = bool.Parse(parametros["inicio"] + "");
                        status = int.Parse(parametros["status"] + "");
                        retorno = BusinessSecretaria.GetEscolaridadeSearch(parametrosSearch, descricao, inicio, base.getStatus(status));
                        break;
                    case TipoRelatorioSGFEnum.CursoSearch:
                        descricao = parametros["descricao"] + "";
                        inicio = bool.Parse(parametros["inicio"] + "");
                        status = int.Parse(parametros["status"] + "");
                        int? produto = null;
                        int? estagio = null;
                        int? modalidade = null;
                        int? nivel = null;
                        try
                        {
                            produto = int.Parse(parametros["produto"] + "");
                            estagio = int.Parse(parametros["estagio"] + "");
                            nivel = int.Parse(parametros["nivel"] + "");
                        }
                        catch
                        {
                        }
                        try
                        {
                            modalidade = int.Parse(parametros["modalidade"] + "");
                        }
                        catch
                        {

                        }

                        retorno = BusinessCurso.getCursoSearch(parametrosSearch, descricao, inicio, base.getStatus(status), produto, estagio, modalidade, nivel, null, null);
                        break;
                    case TipoRelatorioSGFEnum.GrupoSearch:
                        descricao = parametros["descricao"] + "";
                        inicio = bool.Parse(parametros["inicio"] + "");
                        int cdEsc = int.Parse(parametros["cdEsc"] + "");
                        int tipoGP = int.Parse(parametros["tipo"] + "");

                        retorno = BusinessPermissao.GetGrupoSearch(parametrosSearch, descricao, inicio, cdEsc, tipoGP);
                        break;
                    case TipoRelatorioSGFEnum.MidiaSearch:
                        descricao = parametros["descricao"] + "";
                        inicio = bool.Parse(parametros["inicio"] + "");
                        status = int.Parse(parametros["status"] + "");

                        retorno = BusinessSecretaria.GetMidiaSearch(parametrosSearch, descricao, inicio, base.getStatus(status));
                        break;
                    case TipoRelatorioSGFEnum.TipoContatoSearch:
                        descricao = parametros["descricao"] + "";
                        inicio = bool.Parse(parametros["inicio"] + "");
                        status = int.Parse(parametros["status"] + "");

                        retorno = BusinessSecretaria.GetTipoContatoSearch(parametrosSearch, descricao, inicio, base.getStatus(status));
                        break;
                    case TipoRelatorioSGFEnum.MtvMatSearch:
                        descricao = parametros["descricao"] + "";
                        inicio = bool.Parse(parametros["inicio"] + "");
                        status = int.Parse(parametros["status"] + "");

                        retorno = BusinessSecretaria.GetMotivoMatriculaSearch(parametrosSearch, descricao, inicio, base.getStatus(status));
                        break;
                    case TipoRelatorioSGFEnum.MtvNMatSearch:
                        descricao = parametros["descricao"] + "";
                        inicio = bool.Parse(parametros["inicio"] + "");
                        status = int.Parse(parametros["status"] + "");

                        retorno = BusinessSecretaria.GetMotivoNaoMatriculaSearch(parametrosSearch, descricao, inicio, base.getStatus(status));
                        break;
                    case TipoRelatorioSGFEnum.MtvBolsaSearch:
                        descricao = parametros["descricao"] + "";
                        inicio = bool.Parse(parametros["inicio"] + "");
                        status = int.Parse(parametros["status"] + "");

                        retorno = BusinessSecretaria.GetMotivoBolsaSearch(parametrosSearch, descricao, inicio, base.getStatus(status));
                        break;
                    case TipoRelatorioSGFEnum.MtvCancelBolsaSearch:
                        descricao = parametros["descricao"] + "";
                        inicio = bool.Parse(parametros["inicio"] + "");
                        status = int.Parse(parametros["status"] + "");

                        retorno = BusinessSecretaria.GetMotivoCancelBolsaSearch(parametrosSearch, descricao, inicio, base.getStatus(status));
                        break;
                    case TipoRelatorioSGFEnum.AcaoFollowUp:
                        descricao = parametros["descricao"] + "";
                        inicio = bool.Parse(parametros["inicio"] + "");
                        status = int.Parse(parametros["status"] + "");

                        retorno = BusinessSecretaria.GetAcaoFollowUpSearch(parametrosSearch, descricao, inicio, base.getStatus(status));
                        break;
                    case TipoRelatorioSGFEnum.AnoEscolar:
                        descricao = parametros["descricao"] + "";
                        inicio = bool.Parse(parametros["inicio"] + "");
                        status = int.Parse(parametros["status"] + "");
                        var cdEscolaridade = 0;

                        if ((string)parametros["cdEscolaridade"] != "")
                            cdEscolaridade = int.Parse(parametros["cdEscolaridade"] + "");

                        retorno = BusinessSecretaria.GetAnoEscolarSearch(parametrosSearch, cdEscolaridade, descricao, inicio, base.getStatus(status));
                        break;
                    case TipoRelatorioSGFEnum.MatriculaSearch:
                        string descAluno = parametros["descAluno"] + "";
                        string descTurma = parametros["descTurma"] + "";
                        inicio = bool.Parse(parametros["inicio"] + "");
                        bool semTurma = bool.Parse(parametros["semTurma"] + "");
                        int situacaoTurma = int.Parse(parametros["situacaoTurma"] + "");
                        int nmContrato = int.Parse(parametros["nmContrato"] + "");
                        int nm_matricula = int.Parse(parametros["nm_matricula"] + "");
                        int tipo = int.Parse(parametros["tipo"] + "");
                        DateTime? dtaIni = null;
                        if (!string.IsNullOrEmpty(parametros["dtaInicio"].ToString()))
                            dtaIni = DateTime.Parse(parametros["dtaInicio"] + "");
                        DateTime? dtaFim = null;
                        if (!string.IsNullOrEmpty(parametros["dtaFim"].ToString()))
                            dtaFim = DateTime.Parse(parametros["dtaFim"] + "");
                        bool filtraMat = bool.Parse(parametros["filtraMat"] + "");
                        bool filtraDtaInicio = bool.Parse(parametros["filtraDtaInicio"] + "");
                        bool filtraDtaFim = bool.Parse(parametros["filtraDtaFim"] + "");
                        bool renegocia = bool.Parse(parametros["renegocia"] + "");
                        bool transf = bool.Parse(parametros["transf"] + "");
                        bool retornoEsc = bool.Parse(parametros["retornoEsc"] + "");
                        int? cdNomeContrato = null;
                        if (!string.IsNullOrEmpty(parametros["cdNomeContrato"].ToString()))
                            cdNomeContrato = int.Parse(parametros["cdNomeContrato"] + "");
                        cdEsc = int.Parse(parametros["cdEscola"] + "");
                        int? cd_ano_escolar = null;
                        if (!string.IsNullOrEmpty(parametros["cd_ano_escolar"].ToString()))
                            cd_ano_escolar = int.Parse(parametros["cd_ano_escolar"] + "");
                        int? cdContratoAnterior = null;
                        if (!string.IsNullOrEmpty(parametros["cdContratoAnterior"].ToString()))
                            cdContratoAnterior = int.Parse(parametros["cdContratoAnterior"] + "");
                        byte tipoC = byte.Parse(parametros["tipoC"] + "");
                        int statusC = int.Parse(parametros["status"] + "");

                        int vinculado = int.Parse(parametros["vinculado"] + "");

                        retorno = BusinessMatricula.getMatriculaSearch(parametrosSearch, descAluno, descTurma, inicio, semTurma, situacaoTurma, nmContrato, tipo,
                                                                        dtaIni, dtaFim, filtraMat, filtraDtaInicio, filtraDtaFim, cdEsc, renegocia, transf, retornoEsc, 
                                                                        cdNomeContrato, nm_matricula, cd_ano_escolar, cdContratoAnterior, tipoC, getStatus(statusC), vinculado);
                        break;
                    case TipoRelatorioSGFEnum.DesistenciaSearch:
                        int cd_turma = int.Parse(parametros["cd_turma"] + "");
                        int cd_aluno = int.Parse(parametros["cd_aluno"] + "");
                        int cd_professor = int.Parse(parametros["cd_professor"] + "");
                        int cd_produto = int.Parse(parametros["cd_produto"] + "");
                        int cd_pessoa_escola = int.Parse(parametros["cd_pessoa_escola"] + "");
                        int cd_motivo_desistencia = int.Parse(parametros["cd_motivo_desistencia"] + "");
                        int cd_tipo = int.Parse(parametros["cd_tipo"] + "");
                        string cursos = parametros["cursos"] + "";
                        List<int> cdsCurso = new List<int>();
                        if (!String.IsNullOrEmpty(cursos))
                            cdsCurso = cursos.Split('|').Select(Int32.Parse).ToList();
                        dtaIni = null;
                        if (parametros["dta_ini"] != null)
                            dtaIni = DateTime.Parse(parametros["dta_ini"] + "");
                        dtaFim = null;
                        if (parametros["dta_fim"] != null)
                            dtaFim = DateTime.Parse(parametros["dta_fim"] + "");

                        retorno = BusinessSecretaria.getDesistenciaSearchUI(parametrosSearch, cd_turma, cd_aluno, cd_pessoa_escola, cd_motivo_desistencia, cd_tipo, dtaIni,
                            dtaFim, cd_produto, cd_professor, cdsCurso);
                        break;
                    case TipoRelatorioSGFEnum.ConceitoSearch:
                        descricao = parametros["descricao"] + "";
                        inicio = bool.Parse(parametros["inicio"] + "");
                        status = int.Parse(parametros["status"] + "");
                        int codp = int.Parse(parametros["codp"] + "");

                        retorno = BusinessCoordenacao.getDescConceito(parametrosSearch, descricao, inicio, base.getStatus(status), codp);
                        break;
                    case TipoRelatorioSGFEnum.SalaSearch:
                        descricao = parametros["descricao"] + "";
                        inicio = bool.Parse(parametros["inicio"] + "");
                        status = int.Parse(parametros["status"] + "");
                        cdEsc = int.Parse(parametros["cdEscola"] + "");
                        bool salaOnline = parametros["online"] != null ? bool.Parse(parametros["online"] + "") : false;

                        retorno = BusinessCoordenacao.getDescSala(parametrosSearch, descricao, inicio, base.getStatus(status), cdEsc, salaOnline);
                        break;
                    case TipoRelatorioSGFEnum.EventoSearch:
                        descricao = parametros["descricao"] + "";
                        inicio = bool.Parse(parametros["inicio"] + "");
                        status = int.Parse(parametros["status"] + "");

                        retorno = BusinessCoordenacao.getDescEvento(parametrosSearch, descricao, inicio, base.getStatus(status));
                        break;
                    case TipoRelatorioSGFEnum.ProdutoSearch:
                        descricao = parametros["descricao"] + "";
                        var abrev = parametros["abrev"] + "";
                        inicio = bool.Parse(parametros["inicio"] + "");
                        status = int.Parse(parametros["status"] + "");

                        retorno = BusinessCoordenacao.getDescProduto(parametrosSearch, descricao, abrev, inicio, base.getStatus(status));
                        break;
                    case TipoRelatorioSGFEnum.EstagioSearch:
                        descricao = parametros["descricao"] + "";
                        abrev = parametros["abrev"] + "";
                        inicio = bool.Parse(parametros["inicio"] + "");
                        status = int.Parse(parametros["status"] + "");
                        var cdProd = int.Parse(parametros["codp"] + "");

                        retorno = BusinessCoordenacao.getDescEstagio(parametrosSearch, descricao, abrev, inicio, base.getStatus(status), cdProd);
                        break;
                    case TipoRelatorioSGFEnum.DuracaoSearch:
                        descricao = parametros["descricao"] + "";
                        inicio = bool.Parse(parametros["inicio"] + "");
                        status = int.Parse(parametros["status"] + "");

                        retorno = BusinessCoordenacao.getDescDuracao(parametrosSearch, descricao, inicio, base.getStatus(status));
                        break;
                    case TipoRelatorioSGFEnum.AtividadeExtraSearch:
                        int cdAtividadeExtra = int.Parse(parametros["cdAtividadeExtra"] + "");
                        int cdEscola = int.Parse(parametros["cdEscola"] + "");

                        retorno = BusinessCoordenacao.searchAtividadeAlunoReport(cdAtividadeExtra, cdEscola);
                        break;
                    case TipoRelatorioSGFEnum.AulasReposicaoSearch:
                        int cdAulaReposicao = int.Parse(parametros["cdAulaReposicao"] + "");
                        int cdEscolaAulaReposicao = int.Parse(parametros["cdEscola"] + "");

                        retorno = BusinessCoordenacao.searchAlunoAulaReposicao(cdAulaReposicao, cdEscolaAulaReposicao);
                        break;
                    case TipoRelatorioSGFEnum.TipoAtividadeExtraSearch:
                        descricao = parametros["descricao"] + "";
                        inicio = bool.Parse(parametros["inicio"] + "");
                        status = int.Parse(parametros["status"] + "");

                        retorno = BusinessCoordenacao.getDescTipoAtv(parametrosSearch, descricao, inicio, base.getStatus(status));
                        break;
                    case TipoRelatorioSGFEnum.ParticipacaoSearch:
                        descricao = parametros["descricao"] + "";
                        inicio = bool.Parse(parametros["inicio"] + "");
                        status = int.Parse(parametros["status"] + "");

                        retorno = BusinessCoordenacao.getParticipacaoSearch(parametrosSearch, descricao, inicio, base.getStatus(status));
                        break;
                    case TipoRelatorioSGFEnum.NivelSearch:
                        descricao = parametros["descricao"] + "";
                        inicio = bool.Parse(parametros["inicio"] + "");
                        status = int.Parse(parametros["status"] + "");

                        retorno = BusinessCoordenacao.getNivelSearch(parametrosSearch, descricao, inicio, base.getStatus(status));
                        break;
                    case TipoRelatorioSGFEnum.MensagemAvaliacaoSearch:
                        descricao = parametros["descricao"] + "";
                        inicio = bool.Parse(parametros["inicio"] + "");
                        status = int.Parse(parametros["status"] + "");
                        int? cursoMensagemAvaliacao = null;
                        int? produtoMensagemAvaliacao = null;
                        if (!string.IsNullOrEmpty(parametros["produto"] + "")) { produtoMensagemAvaliacao = int.Parse(parametros["produto"] + ""); }
                        if (!string.IsNullOrEmpty(parametros["curso"] + "")) { cursoMensagemAvaliacao = int.Parse(parametros["curso"] + ""); }
                        

                        retorno = BusinessCoordenacao.getMensagemAvaliacaoSearch(parametrosSearch, descricao, inicio, base.getStatus(status), produtoMensagemAvaliacao, cursoMensagemAvaliacao);
                        break;
                    case TipoRelatorioSGFEnum.CalendarioEvento:
                        var cd_escola_cal_evento = int.Parse(parametros["cdEscola"] + "");
                        var dc_titulo_evento = parametros["dc_titulo_evento"] + "";
                        inicio = bool.Parse(parametros["inicio"] + "");

                        bool? ativo_cal_evento = null;
                        if (!string.IsNullOrEmpty(parametros["status"] + ""))
                            ativo_cal_evento = bool.Parse(parametros["status"] + "");

                        var dt_inicial_evento = parametros["dt_inicial_evento"] + "";
                        var dt_final_evento = parametros["dt_final_evento"] + "";
                        var hh_inicial_evento = parametros["hh_inicial_evento"] + "";
                        var hh_final_evento = parametros["hh_final_evento"] + "";

                        retorno = BusinessCoordenacao.obterCalendarioEventosPorFiltros(parametrosSearch, cd_escola_cal_evento, dc_titulo_evento, inicio, ativo_cal_evento, 
                            dt_inicial_evento, dt_final_evento, hh_inicial_evento, hh_final_evento);
                        break;
                    case TipoRelatorioSGFEnum.CalendarioAcademico:
                        var cd_escola_cal_academico = int.Parse(parametros["cdEscola"] + "");

                        bool? ativo_cal_academico = null;
                        if (!string.IsNullOrEmpty(parametros["status"] + ""))
                            ativo_cal_academico = bool.Parse(parametros["status"] + "");

                        int tipo_calendario = int.Parse(parametros["tipo_calendario"] + "");
                        retorno = BusinessCoordenacao.obterCalendarioAcademicosPorFiltros(parametrosSearch, cd_escola_cal_academico, tipo_calendario, ativo_cal_academico, true);
                        break;
                    case TipoRelatorioSGFEnum.CargaProfessorSearch:
                        int qtd_minutos_duracao_carga = int.Parse(parametros["qtd_minutos_duracao"] + "");
                        int cd_escola_cp = int.Parse(parametros["cd_escola"] + "");

                        retorno = BusinessCoordenacao.getCargaProfessorSearch(parametrosSearch, qtd_minutos_duracao_carga, cd_escola_cp);
                        break;
                    case TipoRelatorioSGFEnum.MotivoDesistenciaSearch:
                        descricao = parametros["descricao"] + "";
                        inicio = bool.Parse(parametros["inicio"] + "");
                        status = int.Parse(parametros["status"] + "");
                        bool isCancelamento = bool.Parse(parametros["isCancelamento"] + "");
                        retorno = BusinessCoordenacao.getDescMotivoDesistencia(parametrosSearch, descricao, inicio, base.getStatus(status), isCancelamento);
                        break;
                    case TipoRelatorioSGFEnum.MotivoFaltaSearch:
                        descricao = parametros["descricao"] + "";
                        inicio = bool.Parse(parametros["inicio"] + "");
                        status = int.Parse(parametros["status"] + "");

                        retorno = BusinessCoordenacao.getDescMotivoFalta(parametrosSearch, descricao, inicio, base.getStatus(status));
                        break;
                    case TipoRelatorioSGFEnum.ModalidadeSearch:
                        descricao = parametros["descricao"] + "";
                        inicio = bool.Parse(parametros["inicio"] + "");
                        status = int.Parse(parametros["status"] + "");

                        retorno = BusinessCoordenacao.getDescModalidade(parametrosSearch, descricao, inicio, base.getStatus(status));
                        break;
                    case TipoRelatorioSGFEnum.RegimeSearch:
                        descricao = parametros["descricao"] + "";
                        abrev = parametros["abrev"] + "";
                        inicio = bool.Parse(parametros["inicio"] + "");
                        status = int.Parse(parametros["status"] + "");

                        retorno = BusinessCoordenacao.getDescRegime(parametrosSearch, descricao, abrev, inicio, base.getStatus(status));
                        break;
                    case TipoRelatorioSGFEnum.FeriadoSearch:
                        descricao = parametros["descricao"] + "";
                        inicio = bool.Parse(parametros["inicio"] + "");
                        status = int.Parse(parametros["status"] + "");
                        cdEscola = int.Parse(parametros["cdEscola"] + "");
                        int Ano = int.Parse(parametros["Ano"] + "");
                        int Mes = int.Parse(parametros["Mes"] + "");
                        int Dia = int.Parse(parametros["Dia"] + "");
                        int AnoFim = int.Parse(parametros["AnoFim"] + "");
                        int MesFim = int.Parse(parametros["MesFim"] + "");
                        int DiaFim = int.Parse(parametros["DiaFim"] + "");
                        int somenteAnoPes = int.Parse(parametros["SomenteAno"] + "");
                        bool? somente_ano = getStatus(somenteAnoPes);
                        bool idFeriadoAtivo = bool.Parse(parametros["idFeriadoAtivo"] + "");

                        retorno = BusinessCoordenacao.getDescFeriado(parametrosSearch, descricao, inicio, base.getStatus(status), cdEscola, Ano, Mes, Dia, AnoFim, MesFim, DiaFim, somente_ano, idFeriadoAtivo);
                        break;
                    case TipoRelatorioSGFEnum.CriterioAvaliacaoSearch:
                        descricao = parametros["descricao"] + "";
                        abrev = parametros["abrev"] + "";
                        String abreviado = parametros["abreviado"] + "";
                        inicio = bool.Parse(parametros["inicio"] + "");
                        status = int.Parse(parametros["status"] + "");
                        int conceito = int.Parse(parametros["conceito"] + "");
                        bool IsParticipacao = bool.Parse(parametros["IsParticipacao"] + "");
                        retorno = BusinessCoordenacao.getCriterioAvaliacaoSearch(parametrosSearch, descricao, abrev, inicio, base.getStatus(status), base.getStatus(conceito), IsParticipacao);
                        break;
                    case TipoRelatorioSGFEnum.TipoAvaliacaoSearch:
                        descricao = parametros["descricao"] + "";
                        inicio = bool.Parse(parametros["inicio"] + "");
                        status = int.Parse(parametros["status"] + "");
                        int tipoAvalicao = int.Parse(parametros["tipoAvalicao"] + "");
                        int criterio = int.Parse(parametros["criterio"] + "");
                        int cdCursoPes = int.Parse(parametros["cdCurso"] + "");
                        int cdProdutoPes = int.Parse(parametros["cdProduto"] + "");

                        retorno = BusinessCoordenacao.getTipoAvaliacaoSearch(parametrosSearch, descricao, inicio, base.getStatus(status), tipoAvalicao, criterio, cdCursoPes, cdProdutoPes);
                        break;
                    case TipoRelatorioSGFEnum.AvaliacaoSearch:
                        descricao = parametros["descricao"] + "";
                        inicio = bool.Parse(parametros["inicio"] + "");
                        status = int.Parse(parametros["status"] + "");
                        tipoAvalicao = int.Parse(parametros["tipoAvalicao"] + "");
                        criterio = int.Parse(parametros["criterio"] + "");

                        retorno = BusinessTurma.searchAvaliacao(parametrosSearch, descricao, tipoAvalicao, criterio, inicio, base.getStatus(status));
                        break;

                    case TipoRelatorioSGFEnum.ModeloProgramacaoCursoSearch:
                        int? cdCurso = int.Parse(parametros["cdCurso"] + "");
                        int? cdDuracao = int.Parse(parametros["cdDuracao"] + "");
                        cdEscola = int.Parse(parametros["cdEscola"] + "");

                        retorno = BusinessCoordenacao.getProgramacaoCursoSearch(parametrosSearch, cdCurso, cdDuracao, cdEscola);
                        break;
                    case TipoRelatorioSGFEnum.ProgramacaoCursoSearch:
                        cdCurso = int.Parse(parametros["cdCurso"] + "");
                        cdDuracao = int.Parse(parametros["cdDuracao"] + "");

                        retorno = BusinessCoordenacao.getProgramacaoCursoSearch(parametrosSearch, cdCurso, cdDuracao, null);
                        break;
                    //********Financeiro********\\
                    case TipoRelatorioSGFEnum.GrupoEstoqueSearch:
                        descricao = parametros["descricao"] + "";
                        inicio = bool.Parse(parametros["inicio"] + "");
                        status = int.Parse(parametros["status"] + "");
                        var categoria = int.Parse(parametros["categoria"] + "");

                        retorno = BusinessFinanceiro.getGrupoEstoqueSearch(parametrosSearch, descricao, inicio, base.getStatus(status), categoria);
                        break;
                    case TipoRelatorioSGFEnum.MovimentacaoFinanceiraSearch:
                        descricao = parametros["descricao"] + "";
                        inicio = bool.Parse(parametros["inicio"] + "");
                        status = int.Parse(parametros["status"] + "");

                        retorno = BusinessFinanceiro.getMovimentacaoFinanceiraSearch(parametrosSearch, descricao, inicio, base.getStatus(status));
                        break;
                    case TipoRelatorioSGFEnum.TipoDescontoSearch:
                        descricao = parametros["descricao"] + "";
                        inicio = bool.Parse(parametros["inicio"] + "");
                        status = int.Parse(parametros["status"] + "");
                        var incideBaixa = int.Parse(parametros["incideBaixa"] + "");
                        var pparc = int.Parse(parametros["pparc"] + "");
                        decimal? percentual = null;
                        if (!string.IsNullOrEmpty(parametros["percentual"].ToString()))
                            percentual = decimal.Parse(parametros["percentual"] + "");
                        cdEscola = int.Parse(parametros["cdEscola"] + "");
                        retorno = BusinessFinanceiro.getTipoDescontoSearch(parametrosSearch, descricao, inicio, base.getStatus(status), base.getStatus(incideBaixa), base.getStatus(pparc), percentual, cdEscola);
                        break;
                    case TipoRelatorioSGFEnum.TipoFinanceiroSearch:
                        descricao = parametros["descricao"] + "";
                        inicio = bool.Parse(parametros["inicio"] + "");
                        status = int.Parse(parametros["status"] + "");

                        retorno = BusinessFinanceiro.getTipoFinanceiroSearch(parametrosSearch, descricao, inicio, base.getStatus(status));
                        break;
                    case TipoRelatorioSGFEnum.TipoLiquidacaoSearch:
                        descricao = parametros["descricao"] + "";
                        inicio = bool.Parse(parametros["inicio"] + "");
                        status = int.Parse(parametros["status"] + "");
                        retorno = BusinessFinanceiro.getTipoLiquidacaoSearch(parametrosSearch, descricao, inicio, base.getStatus(status));
                        break;
                    case TipoRelatorioSGFEnum.PoliticaDescontoSearch:
                        var cdTurma = int.Parse(parametros["cdTurma"] + "");
                        var cdAluno = int.Parse(parametros["cdAluno"] + "");
                        dtaIni = null;
                        if (!string.IsNullOrEmpty(parametros["dtaIni"].ToString()))
                            dtaIni = DateTime.Parse(parametros["dtaIni"] + "");
                        dtaFim = null;
                        if (!string.IsNullOrEmpty(parametros["dtaFim"].ToString()))
                            dtaFim = DateTime.Parse(parametros["dtaFim"] + "");
                        var ativo = int.Parse(parametros["ativo"] + "");
                        cdEsc = int.Parse(parametros["cdEsc"] + "");

                        retorno = BusinessFinanceiro.getPoliticaDescontoSearch(parametrosSearch, cdTurma, cdAluno, dtaIni, dtaFim, base.getStatus(ativo), cdEsc);
                        break;

                    case TipoRelatorioSGFEnum.GrupoContaSearch:
                        descricao = parametros["descricao"] + "";
                        inicio = bool.Parse(parametros["inicio"] + "");
                        var tipoGrupo = int.Parse(parametros["tipoGrupo"] + "");
                        retorno = BusinessFinanceiro.getGrupoContaSearch(parametrosSearch, descricao, inicio, tipoGrupo);
                        break;

                    case TipoRelatorioSGFEnum.SubgrupoContaSearch:
                        descricao = parametros["descricao"] + "";
                        inicio = bool.Parse(parametros["inicio"] + "");
                        var cdGrupo = int.Parse(parametros["cdGrupo"] + "");
                        tipo = int.Parse(parametros["tipo"] + "");
                        retorno = SubGrupoSort.parseSubGrupoForSubgrupoContaUI(BusinessFinanceiro.getSubgrupoContaSearch(parametrosSearch, descricao, inicio, cdGrupo, SubgrupoConta.parseTipoNivel(tipo)));
                        break;

                    case TipoRelatorioSGFEnum.BancoSearch:
                        var nome = parametros["nome"] + "";
                        var nmBanco = parametros["nmBanco"] + "";
                        inicio = bool.Parse(parametros["inicio"] + "");

                        retorno = BusinessFinanceiro.getBancoSearch(parametrosSearch, nome, nmBanco, inicio);
                        break;
                    case TipoRelatorioSGFEnum.CarteiraCnabSearch:
                        nome = parametros["nome"] + "";
                        inicio = bool.Parse(parametros["inicio"] + "");
                        var banco = int.Parse(parametros["banco"] + "");
                        status = int.Parse(parametros["status"] + "");
                        retorno = BusinessCnab.getCarteiraCnabSearch(parametrosSearch, nome, inicio, banco, getStatus(status));
                        break;
                    case TipoRelatorioSGFEnum.OrgaoFinanceiroSearch:
                        descricao = parametros["descricao"] + "";
                        inicio = bool.Parse(parametros["inicio"] + "");
                        status = int.Parse(parametros["status"] + "");

                        retorno = BusinessFinanceiro.getOrgaoFinanceiroSearch(parametrosSearch, descricao, inicio, base.getStatus(status));
                        break;
                    case TipoRelatorioSGFEnum.TabelaCursoSearch:
                        int cd_curso = int.Parse(parametros["cdCurso"] + "");
                        int cd_duracao = int.Parse(parametros["cdDuracao"] + "");
                        int cd_regime = int.Parse(parametros["cdRegime"] + "");
                        DateTime? dtaCad = null;
                        if (!string.IsNullOrEmpty(parametros["dtaCad"].ToString()))
                            dtaCad = DateTime.Parse(parametros["dtaCad"] + "");
                        cdEscola = int.Parse(parametros["cdEscola"] + "");
                        var codProduto = int.Parse(parametros["codProduto"] + "");

                        retorno = BusinessFinanceiro.GetTabelaPrecoSearch(parametrosSearch, cd_curso, cd_duracao, cd_regime, dtaCad, cdEscola, codProduto);
                        break;
                    case TipoRelatorioSGFEnum.LocalMovtoSearch:

                        status = int.Parse(parametros["status"] + "");
                        cdEscola = int.Parse(parametros["cdEscola"] + "");
                        nome = parametros["nome"] + "";
                        nmBanco = parametros["nmBanco"] + "";
                        inicio = bool.Parse(parametros["inicio"] + "");
                        tipo = int.Parse(parametros["tipo"] + "");
                        string pessoa = parametros["pessoa"] + "";
                        int pessoaUsuario = int.Parse(parametros["pessoaUsuario"] + "");

                        retorno = BusinessFinanceiro.getLocalMovtoSearch(parametrosSearch, cdEscola, nome, nmBanco, inicio, getStatus(status), tipo, pessoa, pessoaUsuario);
                        break;
                    case TipoRelatorioSGFEnum.PoliticaComercialSearch:

                        bool? ativoPol = null;
                        if (!string.IsNullOrEmpty(parametros["ativo"].ToString()))
                            ativoPol = bool.Parse(parametros["ativo"] + "");
                        cdEscola = int.Parse(parametros["cdEscola"] + "");
                        descricao = parametros["descricao"] + "";
                        inicio = bool.Parse(parametros["inicio"] + "");
                        bool parcIguais = bool.Parse(parametros["parcIguais"] + "");
                        bool vencFixo = bool.Parse(parametros["vencFixo"] + "");


                        retorno = BusinessFinanceiro.getPoliticaComercialSearch(parametrosSearch, descricao, inicio, ativoPol, parcIguais, vencFixo, cdEscola);
                        break;
                    case TipoRelatorioSGFEnum.FechamentoEstoque:
                        int? ano = null;
                        if (parametros["ano"] != null && (string)parametros["ano"] != "" && int.Parse(parametros["ano"] + "") > 0)
                            ano = int.Parse(parametros["ano"] + "");
                        int? mes = null;
                        if ((string)parametros["mes"] != "" && parametros["mes"] != null && int.Parse(parametros["mes"] + "") > 0)
                            mes = int.Parse(parametros["mes"] + "");
                        bool balanco = bool.Parse(parametros["balanco"] + "");
                        cdEsc = int.Parse(parametros["cd_escola"] + "");
                        string dt_inic = parametros["dta_ini"] + "";
                        string dt_fin = parametros["dta_fim"] + "";

                        DateTime dtaInicial = DateTime.MinValue;
                        DateTime dtaFinal = DateTime.MaxValue;

                        if (!String.IsNullOrEmpty(dt_inic))
                            dtaInicial = DateTime.Parse(dt_inic);
                        if (!String.IsNullOrEmpty(dt_fin))
                            dtaFinal = DateTime.Parse(dt_fin);
                        retorno = BusinessFinanceiro.getFechamentoSearch(parametrosSearch, ano, mes, balanco, dtaInicial, dtaFinal, cdEsc);
                        break;
                    case TipoRelatorioSGFEnum.TipoNotaFiscalSearch:
                        var desc = parametros["desc"] + "";
                        var natOp = parametros["natOp"] + "";
                        inicio = bool.Parse(parametros["inicio"] + "");
                        status = int.Parse(parametros["status"] + "");
                        bool? dev = null;
                        if (!string.IsNullOrEmpty(parametros["devolucao"].ToString()))
                            dev = bool.Parse(parametros["devolucao"] + "");
                        cdEsc = int.Parse(parametros["cd_escola"] + "");
                        byte id_regime_trib = byte.Parse(parametros["id_regime_trib"] + "");
                        retorno = BusinessFiscal.getTipoNotaFiscalSearch(parametrosSearch, desc, natOp, inicio, getStatus(status), 0, dev, cdEsc, id_regime_trib, false);
                        break;
                    case TipoRelatorioSGFEnum.AliquotaUFSearch:
                        int estadoOri = int.Parse(parametros["cdEstadoOri"] + "");
                        int estadoDes = int.Parse(parametros["cdEstadoDest"] + "");
                        double? aliquota = null;
                        if (!string.IsNullOrEmpty(parametros["aliquota"].ToString()))
                            aliquota = double.Parse(parametros["aliquota"] + "");

                        retorno = BusinessFinanceiro.getAliquotaUFSearch(parametrosSearch, estadoOri, estadoDes, aliquota); ;
                        break;
                    case TipoRelatorioSGFEnum.DadosNFSearch:
                        cd_cidade = int.Parse(parametros["cdCidade"] + "");
                        natOp = parametros["natOp"] + "";
                        aliquota = null;
                        if (!string.IsNullOrEmpty(parametros["aliquota"].ToString()))
                            aliquota = double.Parse(parametros["aliquota"] + "");
                        id_regime_trib = byte.Parse(parametros["id_regime"] + "");
                        retorno = BusinessFinanceiro.getDadosNFSearch(parametrosSearch, cd_cidade, natOp, aliquota, id_regime_trib); ;
                        break;


                    //********Instituição de Ensino********\\
                    case TipoRelatorioSGFEnum.UsuarioSearch:
                        descricao = parametros["descricao"] + "";
                        var nomePessoa = parametros["nomePessoa"] + "";
                        inicio = bool.Parse(parametros["inicio"] + "");
                        status = int.Parse(parametros["status"] + "");
                        int escola = int.Parse(parametros["escola"] + "");
                        Int32[] cdEscolas = new Int32[0];
                        var stringEscolas = parametros["cdEsc"] + "";
                        var login = parametros["login"] + "";
                        var master = bool.Parse(parametros["isMaster"] + "");
                        var sysAdmin = bool.Parse(parametros["sysAdmin"] + "");
                        var pesqSysAdmin = bool.Parse(parametros["pesqSysAdmin"] + "");
                        if (!string.IsNullOrEmpty(stringEscolas))
                        {
                            string[] arrayEscolas = stringEscolas.Split(',');
                            Int32[] escolasUsuario = new Int32[arrayEscolas.Count()];

                            if (arrayEscolas.Count() > 0)
                            {
                                for (int i = 0; i < arrayEscolas.Count(); i++)
                                    if (!string.IsNullOrEmpty(arrayEscolas[i]))
                                    {
                                        escolasUsuario[i] = int.Parse(arrayEscolas[i]);
                                    }
                                cdEscolas = escolasUsuario;
                            }
                        }
                        retorno = BusinessUsuario.GetUsuarioSearch(parametrosSearch, descricao, nomePessoa, inicio, base.getStatus(status), login, cdEscolas, escola, master, sysAdmin, pesqSysAdmin);
                        break;
                    case TipoRelatorioSGFEnum.EscolaSearch:
                        descricao = parametros["descricao"] + "";
                        inicio = bool.Parse(parametros["inicio"] + "");
                        status = int.Parse(parametros["status"] + "");
                        var apelido = parametros["apelido"] + "";
                        var cnpj = parametros["cnpjCpf"] + "";
                        int cdUsuario = int.Parse(parametros["cdUsuario"] + "");
                        retorno = BusinessEscola.getDescEscola(parametrosSearch, descricao, inicio, base.getStatus(status), cnpj, apelido, cdUsuario);
                        break;
                    case TipoRelatorioSGFEnum.PessoaSearch:
                        var descricaoPessoa = parametros["nome"] + "";
                        var apelidoPessoa = parametros["apelido"] + "";
                        var tipoPessoa = int.Parse(parametros["tipoPessoa"] + "");
                        var cpfCnpj = parametros["cnpjCpf"] + "";
                        int? papel = null;
                        if (parametros["papel"] != null && !String.Empty.Equals(parametros["papel"]))
                            papel = int.Parse(parametros["papel"] + "");
                        int sexo = int.Parse(parametros["sexo"] + "");
                        inicio = bool.Parse(parametros["inicio"] + "");
                        var cdEscolaPessoa = int.Parse(parametros["cdEscola"] + "");

                        retorno = BusinessAluno.GetPessoaSearch(parametrosSearch, descricaoPessoa, apelidoPessoa, tipoPessoa, cpfCnpj, papel, sexo, inicio, cdEscolaPessoa, FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess.AlunoDataAccess.TipoPessoaEnum.PESSOA_RELACIONADA);
                        break;
                    case TipoRelatorioSGFEnum.FuncionarioSearch:
                        var descricaoFunc = parametros["nome"] + "";
                        var apelidoFunc = parametros["apelido"] + "";
                        int statusFunc = int.Parse(parametros["status"] + "");
                        var cpfFunc = parametros["cpf"] + "";
                        var inicioFunc = bool.Parse(parametros["inicio"] + "");
                        var tipoFunc = byte.Parse(parametros["tipo"] + "");
                        var cdEscolaFunc = int.Parse(parametros["cdEscola"] + "");
                        sexo = int.Parse(parametros["sexo"] + "");
                        int cdAtividade = int.Parse(parametros["cdAtividade"] + "");
                        int coordenador = int.Parse(parametros["coordenador"] + "");
                        int colaboradorCyber = int.Parse(parametros["colaboradorCyber"] + "");
                        retorno = BusinessProfessor.getSearchFuncionario(parametrosSearch, descricaoFunc, apelidoFunc, getStatus(statusFunc), cpfFunc, inicioFunc, tipoFunc, cdEscolaFunc, sexo, cdAtividade, coordenador, colaboradorCyber);
                        break;
                    case TipoRelatorioSGFEnum.ItemServicoSearch:
                        descricao = parametros["descricao"] + "";
                        inicio = bool.Parse(parametros["inicio"] + "");
                        status = int.Parse(parametros["status"] + "");
                        var tipoItem = int.Parse(parametros["tipoItem"] + "");
                        var grupoItem = int.Parse(parametros["grupoItem"] + "");
                        cdEscola = int.Parse(parametros["cdEscola"] + "");
                        var isMaster = bool.Parse(parametros["isMaster"] + "");
                        categoria = int.Parse(parametros["categoria"] + "");
                        bool esc = bool.Parse(parametros["escola"] + "");
                        bool contaSegura = bool.Parse(parametros["contaSegura"] + "");
                        retorno = BusinessFinanceiro.getItemSearch(parametrosSearch, descricao, inicio, getStatus(status), tipoItem, grupoItem, cdEscola, isMaster, false, false, false, categoria, esc, contaSegura);
                        break;
                    case TipoRelatorioSGFEnum.KitSearch:
                        descricao = parametros["descricao"] + "";
                        inicio = bool.Parse(parametros["inicio"] + "");
                        status = int.Parse(parametros["status"] + "");
                        var tipoKit = int.Parse(parametros["tipoItem"] + "");
                        var grupoKit = int.Parse(parametros["grupoItem"] + "");
                        cdEscola = int.Parse(parametros["cdEscola"] + "");
                        var isMasterGeral = bool.Parse(parametros["isMaster"] + "");
                        categoria = int.Parse(parametros["categoria"] + "");
                        bool esco = bool.Parse(parametros["escola"] + "");
                        bool contaSeg = bool.Parse(parametros["contaSegura"] + "");
                        retorno = BusinessFinanceiro.getKitSearch(parametrosSearch, descricao, inicio, getStatus(status), tipoKit, grupoKit, cdEscola, isMasterGeral, false, false, false, categoria, esco, contaSeg);
                        break;
                    case TipoRelatorioSGFEnum.ControleFaltasSearch:
                        descricao = parametros["descricao"] + "";
                        cd_turma = int.Parse(parametros["cd_turma"] + "");
                        cd_aluno = int.Parse(parametros["cd_aluno"] + "");
                        var assinatura = int.Parse(parametros["assinatura"] + "");
/*
                        DateTime? dtaInicial = DateTime.MinValue;
                        DateTime? dtaFinal = DateTime.MaxValue;
                        if (dataIni.HasValue)
                            dtaInicial = Convert.ToDateTime(parametros["dataIni"].ToString());

                        if (dataFim.HasValue)
                            dtaFinal = Convert.ToDateTime(parametros["dataIni"].ToString());*/
                      
                        DateTime? dataIniAux = DateTime.MinValue;;
                        if (!string.IsNullOrEmpty(parametros["dataFim"].ToString()))
                        {
                            dataIniAux = DateTime.Parse(parametros["dataFim"] + "");
                        }


                        DateTime? dataFimAux = DateTime.MaxValue;
                        if (!string.IsNullOrEmpty(parametros["dataFim"].ToString()))
                        {
                            dataFimAux = DateTime.Parse(parametros["dataFim"] + "");
                        }

                        cdEscola = int.Parse(parametros["cdEscola"] + "");

                        retorno = BusinessCoordenacao.getControleFaltasSearch(parametrosSearch, descricao, cd_turma, cd_aluno, assinatura, dataIniAux, dataFimAux, cdEscola);
                        break;

                    case TipoRelatorioSGFEnum.EnviarTransferenciaAlunoSearch:
                        int? cd_unidade_destino = null;
#pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
                        if (parametros["cd_unidade_destino"] != "" && parametros["cd_unidade_destino"] != null && int.Parse(parametros["cd_unidade_destino"] + "") > 0)
                        {
                            cd_unidade_destino =  int.Parse(parametros["cd_unidade_destino"] + "");
                        }
#pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast


                        cd_aluno = int.Parse(parametros["cd_aluno"] + "");
                        var nm_raf = parametros["nm_raf"] + "";
                        var cpf = parametros["cpf"] + "";
                        var status_transferencia = int.Parse(parametros["status_transferencia"] + "");

                        DateTime? dataInicialParamAux = null;
                        if (!string.IsNullOrEmpty(parametros["dataIni"].ToString()))
                        {
                            dataInicialParamAux = DateTime.Parse(parametros["dataIni"] + "");
                        }


                        DateTime? dataFinalParamAux = null;
                        if (!string.IsNullOrEmpty(parametros["dataFim"].ToString()))
                        {
                            dataFinalParamAux = DateTime.Parse(parametros["dataFim"] + "");
                        }

                        cdEscola = int.Parse(parametros["cdEscola"] + "");

                        retorno = BusinessSecretaria.getEnviarTransferenciaAlunoSearch(parametrosSearch, cdEscola, cd_unidade_destino, cd_aluno, nm_raf, cpf, status_transferencia, dataFinalParamAux, dataFinalParamAux);
                        break;
                    case TipoRelatorioSGFEnum.ReceberTransferenciaAlunoSearch:
                         int? cd_unidade_origem = null;
#pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
                        if (parametros["cd_unidade_origem"] != "" && parametros["cd_unidade_origem"] != null && int.Parse(parametros["cd_unidade_origem"] + "") > 0)
                        {
                            cd_unidade_origem = int.Parse(parametros["cd_unidade_origem"] + "");
                        }
#pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast


                        string no_aluno = parametros["no_aluno"] + "";
                         nm_raf = parametros["nm_raf"] + "";
                         cpf = parametros["cpf"] + "";
                         status_transferencia = int.Parse(parametros["status_transferencia"] + "");

                         dataInicialParamAux = null;
                        if (!string.IsNullOrEmpty(parametros["dataIni"].ToString()))
                        {
                            dataInicialParamAux = DateTime.Parse(parametros["dataIni"] + "");
                        }


                         dataFinalParamAux = null;
                        if (!string.IsNullOrEmpty(parametros["dataFim"].ToString()))
                        {
                            dataFinalParamAux = DateTime.Parse(parametros["dataFim"] + "");
                        }

                        cdEscola = int.Parse(parametros["cdEscola"] + "");

                        retorno = BusinessSecretaria.getReceberTransferenciaAlunoSearch(parametrosSearch, cdEscola, cd_unidade_origem, no_aluno, nm_raf, cpf, status_transferencia, dataFinalParamAux, dataFinalParamAux);
                        break;

                    case TipoRelatorioSGFEnum.AlunoSearch:
                        var descricaoAluno = parametros["nome"] + "";
                        var apelidoAluno = parametros["apelido"] + "";
                        int statusAluno = int.Parse(parametros["status"] + "");
                        var cdSituacao = parametros["cdSituacoes"] + "";
                        var cpfAluno = parametros["cnpjCpf"] + "";
                        var inicioAluno = bool.Parse(parametros["inicio"] + "");
                        var cdEscolaAluno = int.Parse(parametros["cdEscola"] + "");
                        sexo = int.Parse(parametros["sexo"] + "");
                        semTurma = bool.Parse(parametros["semTurma"] + "");
                        bool movido = bool.Parse(parametros["movido"] + "");
                        int tipoAluno = int.Parse(parametros["tipoAluno"] + "");
                        string[] situacao = cdSituacao.Split('|');
                        List<int> cdsSituacoes = new List<int>();
                        for (int i = 0; i < situacao.Count(); i++)
                            cdsSituacoes.Add(Int32.Parse(situacao[i]));
                        bool matriculasem = bool.Parse(parametros["matriculasem"] + "");
                        retorno = BusinessAluno.getAlunoSearch(parametrosSearch, descricaoAluno, apelidoAluno, inicioAluno, getStatus(statusAluno), cdEscolaAluno, cpfAluno, cdsSituacoes, sexo, semTurma, movido, tipoAluno, matriculasem, false);
                        break;
                    case TipoRelatorioSGFEnum.AlunoRel:
                        descricaoAluno = parametros["nome"] + "";
                        int cdResp = int.Parse(parametros["cdResp"] + "");
                        string telefone = parametros["telefone"] + "";
                        string email = parametros["email"] + "";
                        statusAluno = int.Parse(parametros["status"] + "");
                        cdEscolaAluno = int.Parse(parametros["cdEscola"] + "");
                        int cd_midia = int.Parse(parametros["cd_midia"] + "");
                        dtaIni = null;
                        if (!string.IsNullOrEmpty(parametros["dtaIni"].ToString()))
                            dtaIni = DateTime.Parse(parametros["dtaIni"] + "");

                        dtaFim = null;
                        if (!string.IsNullOrEmpty(parametros["dtaFinal"].ToString()))
                            dtaFim = DateTime.Parse(parametros["dtaFinal"] + "");
                        var cdSituacaorel = parametros["situacaoAlunoTurma"] + "";
                        string[] situacaorel = cdSituacaorel.Split('|');
                        List<int> cdsSituacoesrel = new List<int>();
                        for (int i = 0; i < situacaorel.Count(); i++)
                            cdsSituacoesrel.Add(Int32.Parse(situacaorel[i]));
                        bool exibirenderecoA = bool.Parse(parametros["exibirEnderecos"] + "");
                        retorno = BusinessAluno.getRelAluno(descricaoAluno, cdResp, telefone, email, getStatus(statusAluno), cdEscolaAluno, dtaIni, dtaFim, cd_midia, cdsSituacoesrel, exibirenderecoA);
                        break;
                    case TipoRelatorioSGFEnum.AtividadeExtra:
                        DateTime? dataIni = null;
                        if (!string.IsNullOrEmpty(parametros["dataIni"].ToString()))
                            dataIni = DateTime.Parse(parametros["dataIni"] + "");

                        DateTime? dataFim = null;
                        if (!string.IsNullOrEmpty(parametros["dataFim"].ToString()))
                            dataFim = DateTime.Parse(parametros["dataFim"] + "");

                        TimeSpan? hrInicial = null;
                        if (!string.IsNullOrEmpty(parametros["hrInicial"].ToString()))
                            hrInicial = TimeSpan.Parse(parametros["hrInicial"] + "");

                        TimeSpan? hrFinal = null;
                        if (!string.IsNullOrEmpty(parametros["hrFinal"].ToString()))
                            hrFinal = TimeSpan.Parse(parametros["hrFinal"] + "");

                        int? tipoAtividade = null;
                        if (!string.IsNullOrEmpty(parametros["tipoAtividade"].ToString()))
                            tipoAtividade = int.Parse(parametros["tipoAtividade"] + "");

                        int? curso = null;
                        if (!string.IsNullOrEmpty(parametros["curso"].ToString()))
                            curso = int.Parse(parametros["curso"] + "");

                        int? responsavel = null;
                        if (!string.IsNullOrEmpty(parametros["responsavel"].ToString()))
                            responsavel = int.Parse(parametros["responsavel"] + "");

                        produto = null;
                        if (!string.IsNullOrEmpty(parametros["produto"].ToString()))
                            produto = int.Parse(parametros["produto"] + "");

                        int? aluno = null;
                        if (!string.IsNullOrEmpty(parametros["aluno"].ToString()))
                            aluno = int.Parse(parametros["aluno"] + "");

                        byte lancada = 2;
                        lancada = byte.Parse(parametros["lancada"] + "");

                        cdEscola = int.Parse(parametros["cdEscola"] + "");
                        int cd_escola_combo_atividade_extra = int.Parse(parametros["cd_escola_combo"] + "");
                        retorno = BusinessCoordenacao.searchAtividadeExtra(parametrosSearch, dataIni, dataFim, hrInicial, hrFinal, tipoAtividade, curso, responsavel, produto, aluno, lancada, cdEscola, cd_escola_combo_atividade_extra);
                        break;
                    case TipoRelatorioSGFEnum.Prospect:
                        nomePessoa = parametros["nome"] + "";
                        inicio = bool.Parse(parametros["inicio"] + "");
                        email = parametros["email"] + "";
                        escola = int.Parse(parametros["escola"] + "");

                        dataIni = null;
                        if (!string.IsNullOrEmpty(parametros["dataIni"].ToString()))
                            dataIni = DateTime.Parse(parametros["dataIni"] + "");

                        dataFim = null;
                        if (!string.IsNullOrEmpty(parametros["dataFim"].ToString()))
                            dataFim = DateTime.Parse(parametros["dataFim"] + "");

                        //ativo = (string)parametros["ativo"] == "false" ? 0 : 1;//int.Parse(parametros["ativo"] + ""); LBM Fórmula errada pois vem 2
                        ativo = int.Parse(parametros["ativo"] + "");
                        bool alunoCheck = bool.Parse(parametros["aluno"] + "");

                        int testeClassificacaoMatriculaOnlineCheck = int.Parse(parametros["testeClassificacaoMatriculaOnline"] + "");


                        retorno = BusinessSecretaria.GetProspectSearch(parametrosSearch, nomePessoa, inicio, email, escola, dataIni, dataFim, base.getStatus(ativo), alunoCheck, testeClassificacaoMatriculaOnlineCheck);

                        break;
                    case TipoRelatorioSGFEnum.AvaliacaoTurmaSearch:
                        var idTurma = int.Parse(parametros["idTurma"] + "");
                        var idTipo = int.Parse(parametros["idTipoAvaliacao"] + "");
                        escola = int.Parse(parametros["cdEscola"] + "");
                        cdUsuario = int.Parse(parametros["cdUsuario"] + "");
                        tipoAvalicao = int.Parse(parametros["cd_tipo_avaliacao"] + "");
                        criterio = int.Parse(parametros["cd_criterio_avaliacao"] + "");
                        cd_curso = int.Parse(parametros["cd_curso"] + "");
                        int cd_funcionario = int.Parse(parametros["cd_funcionario"] + "");
                        isMaster = bool.Parse(parametros["isMaster"] + "");
                        dataIni = null;
                        if (!string.IsNullOrEmpty(parametros["dataInicial"].ToString()) && parametros["dataInicial"].ToString() != "null")
                            dataIni = DateTime.Parse(parametros["dataInicial"] + "");
                        dataFim = null;
                        if (!string.IsNullOrEmpty(parametros["dataFinal"].ToString()) && parametros["dataFinal"].ToString() != "null")
                            dataFim = DateTime.Parse(parametros["dataFinal"] + "");
                        int cd_escola_combo_avaliacao = int.Parse(parametros["cd_escola_combo"] + "");
                        retorno = BusinessTurma.searchAvaliacaoTurma(parametrosSearch, idTurma, getStatus(idTipo), escola, cdUsuario, tipoAvalicao, criterio, cd_curso, cd_funcionario, dataIni, dataFim, isMaster, cd_escola_combo_avaliacao);
                        break;
                    case TipoRelatorioSGFEnum.Turma:
                        string descricaoT = parametros["descricao"] + "";
                        string apelido_turma = parametros["apelido"] + "";
                        bool inicioT = bool.Parse(parametros["inicio"] + "");
                        int tipoTurma = int.Parse(parametros["tipoTurma"] + "");
                        int cdCursoT = int.Parse(parametros["cdCurso"] + "");
                        int cdDuracaoT = int.Parse(parametros["cdDuracao"] + "");
                        int cdProduto = int.Parse(parametros["cdProduto"] + "");
                        situacaoTurma = int.Parse(parametros["situacaoTurma"] + "");
                        int cdProfessor = int.Parse(parametros["cdProfessor"] + "");
                        int cd_escola = int.Parse(parametros["cd_escola"] + "");
                        int prog = int.Parse(parametros["prog"] + "");
                        bool turmasFilhas = bool.Parse(parametros["turmasFilhas"] + "");
                        cdAluno = int.Parse(parametros["cdAluno"] + "");
                        string dtaInicialT = parametros["dtInicial"] + "";
                        string dtaFinalT = parametros["dtFinal"] + "";
                        int? cd_turma_ppt = (parametros["cdTurmaPPT"] + "") != null ? int.Parse(parametros["cdTurmaPPT"] + "") : 0;
                        DateTime? dtInicial = String.IsNullOrEmpty(dtaInicialT) ? null : (DateTime?)DateTime.Parse(dtaInicialT, new CultureInfo("pt-br", false));
                        DateTime? dtFinal = String.IsNullOrEmpty(dtaFinalT) ? null : (DateTime?)DateTime.Parse(dtaFinalT, new CultureInfo("pt-br", false));
                        bool semContrato = bool.Parse(parametros["semContrato"] + "");
                        bool profTurmasAtuais = bool.Parse(parametros["ProfTurmasAtuais"] + "");
                        int cd_escola_combo_turma = int.Parse(parametros["cd_escola_combo"] + "");
                        int ckOnLine = (parametros["ckOnLine"]!= null)? int.Parse(parametros["ckOnLine"] + ""): 0;
                        string dias = parametros["dias"] + "";
                        int cd_search_sala = (parametros["cd_search_sala"] + "") != null ? int.Parse(parametros["cd_search_sala"] + ""): 0;
                        int cd_search_sala_online = (parametros["cd_search_sala_online"] + "") != null ? int.Parse(parametros["cd_search_sala_online"] + ""): 0;
                        bool ckSearchSemSala = (parametros["ckSearchSemSala"] + "") != null ? bool.Parse(parametros["ckSearchSemSala"] + "") : false;
                        bool ckSearchSemAluno = (parametros["ckSearchSemAluno"] + "") != null ? bool.Parse(parametros["ckSearchSemAluno"] + "") : false;
                        retorno = BusinessTurma.searchTurma(parametrosSearch, descricaoT, apelido_turma, inicioT, tipoTurma, cdCursoT, cdDuracaoT,
                            cdProduto, situacaoTurma, cdProfessor, ProgramacaoTurma.parseTipoConsultaProg(prog), cd_escola, turmasFilhas, cdAluno, 0, dtInicial,
                            dtFinal, cd_turma_ppt, semContrato, (int)TurmaDataAccess.TipoConsultaTurmaEnum.HAS_PESQ_PRINC_TURMA, null, null, profTurmasAtuais, cd_search_sala,
                            cd_search_sala_online, ckSearchSemSala, ckSearchSemAluno, null, 
                            cd_escola_combo_turma, 0, ckOnLine, dias);
                        break;
                    case TipoRelatorioSGFEnum.DiarioAulaSearch:
                        var cd_turmaDiario = int.Parse(parametros["cd_turma"] + "");
                        string no_profDiario = parametros["no_professor"] + "";
                        byte cd_tipo_aula = byte.Parse(parametros["cd_tipo_aula"] + "");
                        byte statusDiario = byte.Parse(parametros["status"] + "");
                        byte presProf = byte.Parse(parametros["presProf"] + "");
                        bool substituto = bool.Parse(parametros["substituto"] + "");
                        inicio = bool.Parse(parametros["inicio"] + "");
                        string dtaInicialDiario = parametros["dtInicial"] + "";
                        string dtaFinalDiario = parametros["dtFinal"] + "";
                        DateTime? dtInicialDiario = String.IsNullOrEmpty(dtaInicialDiario) ? null : (DateTime?)DateTime.Parse(dtaInicialDiario, new CultureInfo("pt-br", false));
                        DateTime? dtFinalDiario = String.IsNullOrEmpty(dtaFinalDiario) ? null : (DateTime?)DateTime.Parse(dtaFinalDiario, new CultureInfo("pt-br", false));
                        int? cdProf = (parametros["cdProf"] + "") != null ? int.Parse(parametros["cdProf"] + "") : 0;
                        escola = int.Parse(parametros["cd_escola"] + "");
                        int cd_escola_combo_diario_aula = int.Parse(parametros["cd_escola_combo"] + "");
                        retorno = BusinessCoordenacao.searchDiarioAula(parametrosSearch, cd_turmaDiario, no_profDiario, cd_tipo_aula, statusDiario, presProf, substituto,
                                  inicio, dtInicialDiario, dtFinalDiario, escola, cdProf, cd_escola_combo_diario_aula);
                        break;
                    case TipoRelatorioSGFEnum.BibliotecaSearch:
                        cd_escola = int.Parse(parametros["cd_escola"] + "");
                        int? cd_pessoa = null;
                        if (!"".Equals(parametros["cd_pessoa"] + ""))
                            cd_pessoa = int.Parse(parametros["cd_pessoa"] + "");
                        int? cd_item = null;
                        if (!"".Equals(parametros["cd_item"] + ""))
                            cd_item = int.Parse(parametros["cd_item"] + "");
                        bool? pendentes = null;
                        if (!"".Equals(parametros["pendentes"] + ""))
                            pendentes = bool.Parse(parametros["pendentes"] + "");
                        bool? emprestimos = null;
                        if (!"".Equals(parametros["emprestimos"] + ""))
                            emprestimos = bool.Parse(parametros["emprestimos"] + "");
                        bool? devolucao = null;
                        if (!"".Equals(parametros["devolucao"] + ""))
                            devolucao = bool.Parse(parametros["devolucao"] + "");

                        string dt_inicial = parametros["dt_inicial"] + "";
                        string dt_final = parametros["dt_final"] + "";

                        DateTime? dta_inicial = null;
                        DateTime? dta_final = null;

                        if (!String.IsNullOrEmpty(dt_inicial))
                            dta_inicial = DateTime.Parse(dt_inicial);
                        if (!String.IsNullOrEmpty(dt_final))
                            dta_final = DateTime.Parse(dt_final);

                        retorno = BusinessBiblioteca.getEmprestimoSearch(parametrosSearch, cd_pessoa, cd_item, pendentes, dta_inicial, dta_final, emprestimos, devolucao, cd_escola);
                        break;
                    case TipoRelatorioSGFEnum.ContaCorrente:
                        int cd_origem = int.Parse(parametros["cdOrigem"] + "");
                        int cd_destino = int.Parse(parametros["cdDestino"] + "");
                        byte idEntraSai = byte.Parse(parametros["entraSaida"] + "");
                        int cd_movimento = int.Parse(parametros["cdMovimento"] + "");
                        int cd_plano_conta = int.Parse(parametros["cdPlanoConta"] + "");
                        bool IsContaSegura = bool.Parse(parametros["contaSegura"] + "");
                        dt_inicial = parametros["dta_ini"] + "";
                        dt_final = parametros["dta_fim"] + "";

                        DateTime dta_Inicial = DateTime.MinValue;
                        DateTime dta_Final = DateTime.MaxValue;

                        if (!String.IsNullOrEmpty(dt_inicial))
                            dta_Inicial = DateTime.Parse(dt_inicial);
                        if (!String.IsNullOrEmpty(dt_final))
                            dta_Final = DateTime.Parse(dt_final);
                        pessoaUsuario = int.Parse(parametros["pessoaUsuario"] + "");

                        cd_pessoa_escola = int.Parse(parametros["cd_pessoa_escola"] + "");
                        retorno = BusinessFinanceiro.getContaCorreteSearch(parametrosSearch, cd_pessoa_escola, cd_origem, cd_destino, idEntraSai, cd_movimento, cd_plano_conta, dta_Inicial, dta_Final, pessoaUsuario, IsContaSegura);
                        break;
                    case TipoRelatorioSGFEnum.Movimento:
                        int id_tipo_movimento = int.Parse(parametros["tipoMovimento"] + "");
                        int cd_pessoa_movimento = int.Parse(parametros["cdPessoa"] + "");
                        int cd_item_movimento = int.Parse(parametros["cdItem"] + "");
                        int cd_plano_conta_movimento = int.Parse(parametros["cdPlanoConta"] + "");
                        int numero = int.Parse(parametros["numero"] + "");
                        string serie = (parametros["serie"] + "");
                        bool emissao = bool.Parse(parametros["emissao"] + "");
                        bool movimento = bool.Parse(parametros["movimento"] + "");
                        bool nf = bool.Parse(parametros["notaFiscal"] + "");
                        dt_inicial = parametros["dtInicial"] + "";
                        dt_final = parametros["dtFinal"] + "";
                        int statusNF = int.Parse(parametros["statusNF"] + "");
                        bool isContaSegura = bool.Parse(parametros["contaSegura"] + "");
                        bool isFutura = bool.Parse(parametros["id_venda_futura"] + "");
                        DateTime dta_Inicial_movto = DateTime.MinValue;
                        DateTime dta_Final_movto = DateTime.MaxValue;

                        if (!String.IsNullOrEmpty(dt_inicial))
                            dta_Inicial_movto = DateTime.Parse(dt_inicial);
                        if (!String.IsNullOrEmpty(dt_final))
                            dta_Final_movto = DateTime.Parse(dt_final);

                        cd_pessoa_escola = int.Parse(parametros["cdEmpresa"] + "");
                        retorno = BusinessFiscal.searchMovimento(parametrosSearch, id_tipo_movimento, cd_pessoa_movimento, cd_item_movimento, cd_plano_conta_movimento, numero, serie, cd_pessoa_escola, emissao, movimento,
                                                                     dta_Inicial_movto, dta_Final_movto, nf, statusNF, isContaSegura, 0, null,isFutura);
                        break;
                    case TipoRelatorioSGFEnum.CnabBoleto:
                        int cd_carteira = int.Parse(parametros["cd_carteira"] + "");
                        int cd_aluno_cnab = int.Parse(parametros["cd_aluno"] + "");
                        int cd_responsavel_cnab = int.Parse(parametros["cd_responsavel"] + "");
                        int cd_usuario = int.Parse(parametros["cd_usuario"] + "");
                        byte tipo_cnab = byte.Parse(parametros["tipo_cnab"] + "");
                        int statusCnab = int.Parse(parametros["status"] + "");
                        bool emissaoCnab = bool.Parse(parametros["emissao"] + "");
                        bool vencimentoCnab = bool.Parse(parametros["vencimento"] + "");
                        string nossoNumero = parametros["nossoNumero"] + "";
                        int? nro_contrato = null;
                        bool icnab = bool.Parse(parametros["icnab"] + "");
                        bool iboleto = bool.Parse(parametros["iboleto"] + "");
                        if ((string)parametros["nro_contrato"] != "")
                            nro_contrato = int.Parse(parametros["nro_contrato"] + "");

                        dt_inicial = parametros["dtInicial"] + "";
                        dt_final = parametros["dtFinal"] + "";

                        DateTime dta_Inicial_cnab = DateTime.MinValue;
                        DateTime dta_final_cnab = DateTime.MaxValue;

                        if (!String.IsNullOrEmpty(dt_inicial))
                            dta_Inicial_cnab = DateTime.Parse(dt_inicial);
                        if (!String.IsNullOrEmpty(dt_final))
                            dta_final_cnab = DateTime.Parse(dt_final);

                            cd_pessoa_escola = int.Parse(parametros["cdEmpresa"] + "");
                            retorno = BusinessCnab.searchCnab(parametrosSearch, cd_carteira, cd_usuario, tipo_cnab, statusCnab, dta_Inicial_cnab, dta_final_cnab, emissaoCnab, vencimentoCnab, nossoNumero, nro_contrato, cd_pessoa_escola, icnab, iboleto, cd_responsavel_cnab, cd_aluno_cnab);
                        break;
                    case TipoRelatorioSGFEnum.RetornoCNAB:

                        cd_carteira = int.Parse(parametros["cd_carteira"] + "");
                        cd_usuario = int.Parse(parametros["cd_usuario"] + "");
                        statusCnab = int.Parse(parametros["status"] + "");
                        int cd_aluno_retorno_cnab = int.Parse(parametros["cd_aluno"] + "");
                        int cd_responsavel_retorno_cnab = int.Parse(parametros["cd_responsavel"] + "");
                        string descRetorno = (parametros["emissao"] + "");

                        dt_inicial = parametros["dtInicial"] + "";
                        dt_final = parametros["dtFinal"] + "";

                        dta_Inicial_cnab = DateTime.MinValue;
                        dta_final_cnab = DateTime.MaxValue;
                        string noNumero = parametros["nossoNumero"] + "";

                        if (!String.IsNullOrEmpty(dt_inicial))
                            dta_Inicial_cnab = DateTime.Parse(dt_inicial);
                        if (!String.IsNullOrEmpty(dt_final))
                            dta_final_cnab = DateTime.Parse(dt_final);

                        cd_pessoa_escola = int.Parse(parametros["cdEmpresa"] + "");
                        retorno = BusinessCnab.searchRetornoCNAB(parametrosSearch, cd_carteira, cd_usuario, statusCnab, descRetorno, dta_Inicial_cnab, dta_final_cnab, noNumero, cd_pessoa_escola, cd_responsavel_retorno_cnab, cd_aluno_retorno_cnab);
                        break;
                    case TipoRelatorioSGFEnum.FollowUp:
                        byte id_tipo_follow = byte.Parse(parametros["id_tipo_follow"] + "");
                        int cd_usuario_org = int.Parse(parametros["cd_usuario_org"] + "");
                        int cd_usuario_destino = int.Parse(parametros["cd_usuario_destino"] + "");
                        int cd_prospect = int.Parse(parametros["cd_prospect"] + "");
                        int cd_aluno_followUp = int.Parse(parametros["cd_aluno"] + "");
                        int cd_acao = int.Parse(parametros["cd_acao"] + "");
                        int cd_usuario_logado = int.Parse(parametros["cd_usuario_logado"] + "");
                        int resolvido = int.Parse(parametros["resolvido"] + "");
                        int lido = int.Parse(parametros["lido"] + "");
                        bool data = bool.Parse(parametros["data"] + "");
                        bool proximo_contato = bool.Parse(parametros["proximo_contato"] + "");
                        bool id_usuario_adm = bool.Parse(parametros["id_usuario_adm"] + "");
                        bool usuario_login_master = bool.Parse(parametros["usuario_login_master"] + "");
                        dt_inicial = parametros["dtInicial"] + "";
                        dt_final = parametros["dtFinal"] + "";
                        cd_pessoa_escola = int.Parse(parametros["cdEmpresa"] + "");
                        DateTime dta_Inicial_followUp = DateTime.MinValue;
                        DateTime dta_final_followUp = DateTime.MaxValue;
                        dta_Inicial_followUp = DateTime.MinValue;
                        dta_final_followUp = DateTime.MaxValue;
                        if (!String.IsNullOrEmpty(dt_inicial))
                            dta_Inicial_followUp = DateTime.Parse(dt_inicial);
                        if (!String.IsNullOrEmpty(dt_final))
                            dta_final_followUp = DateTime.Parse(dt_final);
                        retorno = BusinessSecretaria.getFollowUpSearch(parametrosSearch, cd_pessoa_escola, id_tipo_follow, cd_usuario_org, cd_usuario_destino, cd_prospect, cd_acao, resolvido, lido,
                            data, proximo_contato, dta_Inicial_followUp, dta_final_followUp, id_usuario_adm, cd_usuario_logado, cd_aluno_followUp, usuario_login_master);
                        break;
                    case TipoRelatorioSGFEnum.AulaPersonalizadaSearch:

                        dt_inicial = parametros["dataIni"] + "";
                        DateTime? dataIninial = String.IsNullOrEmpty(dt_inicial) ? null : (DateTime?)DateTime.Parse(dt_inicial, new CultureInfo("pt-br", false));
                        dt_final = parametros["dataFim"] + "";
                        DateTime? dataFinal = String.IsNullOrEmpty(dt_final) ? null : (DateTime?)DateTime.Parse(dt_final, new CultureInfo("pt-br", false));
                        var hhInicial = parametros["hrInicial"] + "";
                        TimeSpan? horaInicial = String.IsNullOrEmpty(hhInicial) ? null : (TimeSpan?)TimeSpan.Parse(hhInicial, new CultureInfo("pt-br", false));
                        var hhFinal = parametros["hrFinal"] + "";
                        TimeSpan? horaFinal = String.IsNullOrEmpty(hhFinal) ? null : (TimeSpan?)TimeSpan.Parse(hhFinal, new CultureInfo("pt-br", false));
                        cdProduto = (parametros["cdProduto"] + "") != null ? int.Parse(parametros["cdProduto"] + "") : 0;
                        cdProfessor = (parametros["cdProfessor"] + "") != null ? int.Parse(parametros["cdProfessor"] + "") : 0;
                        int? cdSala = (parametros["cdSala"] + "") != null ? int.Parse(parametros["cdSala"] + "") : 0;
                        cdAluno = (!String.IsNullOrEmpty(parametros["cdAluno"] + "")) ? int.Parse(parametros["cdAluno"] + "") : 0;
                        cdEscola = int.Parse(parametros["cdEscola"] + "");
                        bool participou = bool.Parse(parametros["participou"] + "");
                        retorno = BusinessCoordenacao.searchAulaPersonalizada(parametrosSearch, dataIninial, dataFinal, horaInicial, horaFinal, cdProduto, cdProfessor, cdSala, cdAluno, participou, cdEscola);
                        break;

                    case TipoRelatorioSGFEnum.AvaliacaoParticipacaoSearch:
                        ativo = int.Parse(parametros["ativo"] + "");
                        int cdCriterio = int.Parse(parametros["cdCriterio"] + "");
                        int cdParticipacao = int.Parse(parametros["cdParticipacao"] + "");
                        cdProduto = int.Parse(parametros["cdProduto"] + "");
                        cdEscola = int.Parse(parametros["cd_escola"] + "");
                        retorno = BusinessCoordenacao.searchAvaliacaoParticipacao(parametrosSearch, cdCriterio, cdParticipacao, cdProduto, base.getStatus(ativo), cdEscola);
                        break;
                    case TipoRelatorioSGFEnum.ReajusteAnualSearch:
                        int cd_empresa = int.Parse(parametros["cd_empresa"] + "");
                        cd_usuario = int.Parse(parametros["cd_usuario"] + "");
                        int cd_reajuste_anual = int.Parse(parametros["cd_reajuste_anual"] + "");
                        int id_status = int.Parse(parametros["id_status"] + "");
                        dt_inicial = parametros["dt_inicial"] + "";
                        dt_final = parametros["dt_final"] + "";
                        bool id_cadastro = bool.Parse(parametros["id_cadastro"] + "");
                        bool id_vcto_inicial = bool.Parse(parametros["id_vcto_inicial"] + "");

                        DateTime? dataInicial = String.IsNullOrEmpty(dt_inicial) ? null : (DateTime?)DateTime.Parse(dt_inicial, new CultureInfo("pt-br", false));
                        dataFinal = String.IsNullOrEmpty(dt_final) ? null : (DateTime?)DateTime.Parse(dt_final, new CultureInfo("pt-br", false));

                        retorno = BusinessFinanceiro.getReajusteAnualSearch(parametrosSearch, cd_empresa, cd_usuario, id_status, dataInicial, dataFinal, id_cadastro, id_vcto_inicial, cd_reajuste_anual);
                        break;
                    case TipoRelatorioSGFEnum.AlunosCartaQuitacao:
                        int cd_empresaParam = int.Parse(parametros["cdEscola"] + "");
                        int anoParam = int.Parse(parametros["ano"] + "");
                        int cdPessoaParam = int.Parse(parametros["cd_pessoa"] + "");

                        retorno = BusinessAluno.findAlunoCartaQuitacao(parametrosSearch, cd_empresaParam, anoParam, cdPessoaParam);
                        break;
                    case TipoRelatorioSGFEnum.PerdaMaterialSearch:
                        int cd_empresaPerdaMaterial = int.Parse(parametros["cdEscola"] + "");
                        int? cd_alunoPerdaMaterial = parametros["cd_aluno"] != null ? int.Parse(parametros["cd_aluno"] + "") : 0;
                        int? nm_contratoPerdaMaterial = parametros["nm_contrato"] != null ? int.Parse(parametros["nm_contrato"] + "") : 0;
                        int? cd_movimentoPerdaMaterial = parametros["cd_movimento"] != null ? int.Parse(parametros["cd_movimento"] + "") : 0;
                        int? cd_itemPerdaMaterial = parametros["cd_item"] != null ? int.Parse(parametros["cd_item"] + "") : 0;
                        string dtInicialPerdaMaterial = parametros["dtInicial"] + "";
                        string dtFinalPerdaMaterial = parametros["dtFinal"] + "";
                        int statusPerdaPerdaMaterial = parametros["status"] != null ? int.Parse(parametros["status"] + "") : 0;

                        DateTime? dataInicialPerdaMaterial = String.IsNullOrEmpty(dtInicialPerdaMaterial) ? null : (DateTime?)DateTime.Parse(dtFinalPerdaMaterial, new CultureInfo("pt-br", false));
                        DateTime? dataFinalPerdaMaterial = String.IsNullOrEmpty(dtFinalPerdaMaterial) ? null : (DateTime?)DateTime.Parse(dtFinalPerdaMaterial, new CultureInfo("pt-br", false));

                        retorno = BusinessCoordenacao.getPerdaMaterialSearch(parametrosSearch, cd_alunoPerdaMaterial, nm_contratoPerdaMaterial, cd_movimentoPerdaMaterial, cd_itemPerdaMaterial, dataInicialPerdaMaterial, dataFinalPerdaMaterial, statusPerdaPerdaMaterial, cd_empresaPerdaMaterial).ToList();
                        break;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return retorno;
        }

        public IEnumerable<RptReceberPagar> getSourceReceberPagar(int cdEscola, DateTime pDtaI, DateTime pDtaF, int pForn, DateTime pDtaBase, byte pNatureza, int pPlanoContas, int ordem, bool cSegura, int cdTpLiq, int cdTpFinan, int tipo, string situacoes, int cdTurma)
        {
            return BusinessEscola.receberPagarStoreProcedure(cdEscola, pDtaI, pDtaF, pForn, pDtaBase, pNatureza, pPlanoContas, ordem, cSegura, cdTpLiq, cdTpFinan, tipo, situacoes, cdTurma);
        }

        public IEnumerable<RptRecebidaPaga> getSourceRecebidaPaga(int cdEscola, DateTime pDtaI, DateTime pDtaF, int pForn, byte pNatureza, int pPlanoContas, bool pMostraCCManual, bool cSegura, int cdTpLiq, int cdTpFinan, int tipo, string situacoes, int cdTurma, bool? ckCancelamento, int cdLocal)
        {
            return BusinessFinanceiro.recebidaPagaStoreProcedure(cdEscola, pDtaI, pDtaF, pForn, pNatureza, pPlanoContas, pMostraCCManual, cSegura, cdTpLiq, cdTpFinan, tipo, situacoes, cdTurma, ckCancelamento, cdLocal);
        }

        public IEnumerable<ObservacaoBaixaUI> getObservacoesBaixaCancelamento(int cdEscola, int cd_baixa_titulo)
        {
            return BusinessFinanceiro.getObservacoesBaixaCancelamento(cdEscola, cd_baixa_titulo);
        }

        public IEnumerable<RptBalanceteMensal> getSourceBalanceteMensal(int cdEscola, int mes, int ano, int nivel, int nivel_analisar, bool mostrar_contas, bool conta_segura)
        {
            return BusinessFinanceiro.getBalanceteMensal(cdEscola, mes, ano, nivel, nivel_analisar, mostrar_contas, conta_segura);
        }


        public AtividadeExtra GetSourceAtividadeExtraReportView(int cdAtividadeExtra)
        {
            return BusinessCoordenacao.findByIdAtividadeExtraFull(cdAtividadeExtra);
        }

        public IEnumerable<AlunosSemTituloGeradoUI> GetSourceAlunosSemTituloGeradoReportView(int vl_mes, int ano, int cd_turma, string situacoes, int cd_escola)
        {
            return BusinessFinanceiro.GetAlunosSemTituloGerado(vl_mes, ano, cd_turma, situacoes, cd_escola);
            //return BusinessCoordenacao.findByIdAtividadeExtraFull(cdAtividadeExtra);
            //return null;
        }

        public string GetSourceNomeEscolaComboReportView(int cdEscolaCombo)
        {
            return BusinessEmpresa.findByNomeEscolaComboReportView(cdEscolaCombo);
        }

        public AulaReposicao GetSourceAulaReposicaoViewReportView(int cdAulaReposicao)
        {
            return BusinessCoordenacao.findByIdAulaReposicaoViewFull(cdAulaReposicao);
        }

        public List<sp_RptAtividadeExtra_Result> GetSourceAtividadeExtra(Nullable<int> cd_escola, Nullable<System.DateTime> dta_ini, Nullable<System.DateTime> dta_fim, Nullable<int> cd_produto, Nullable<int> cd_curso, Nullable<int> cd_funcionario, Nullable<int> cd_aluno, Nullable<byte> id_participacao, Nullable<byte> id_lancada)
        {
            return BusinessCoordenacao.getReportAtividadeExtra(cd_escola, dta_ini, dta_fim, cd_produto, cd_curso, cd_funcionario, cd_aluno, id_participacao, id_lancada);
        }

        public List<sp_RptAulaReposicao_Result> GetSourceAulaReposicao(Nullable<int> cd_escola, Nullable<System.DateTime> dta_ini, Nullable<System.DateTime> dta_fim, Nullable<int> cd_turma, Nullable<int> cd_funcionario, Nullable<int> cd_aluno, Nullable<byte> id_participacao)
        {
            return BusinessCoordenacao.getReportAulaReposicao(cd_escola, dta_ini, dta_fim, cd_turma, cd_funcionario, cd_aluno, id_participacao);
        }
        public List<AlunoRel> GetSourceAlunoCliente(string descricaoAluno, int cdResp, string telefone, string email, int statusAluno, int cdEscolaAluno, DateTime? dtaIni, DateTime? dtaFim, int cd_midia, List<int> cdsSituacoesrel, bool exibirenderecoA)
        {
            return BusinessAluno.getRelAluno(descricaoAluno, cdResp, telefone, email, getStatus(statusAluno), cdEscolaAluno, dtaIni, dtaFim, cd_midia, cdsSituacoesrel, exibirenderecoA).ToList();
        }

        public List<sp_RptControleFaltas_Result> GetSourceControleFaltasResults(Nullable<int> cd_tipo, int cd_escola,
            Nullable<int> cd_curso, Nullable<int> cd_nivel,
            Nullable<int> cd_produto, Nullable<int> cd_professor, Nullable<int> cd_turma, Nullable<int> cd_sit_turma,
            string cd_sit_aluno, string dt_inicial,
            string dt_final, bool quebrarpagina)
        {
            return BusinessCoordenacao.getReportControleFaltasResults(cd_tipo, cd_escola, cd_curso, cd_nivel, cd_produto, cd_professor, cd_turma, cd_sit_turma, cd_sit_aluno, dt_inicial, dt_final, quebrarpagina);
        }

        public DataTable GetAlunos(int cd_aluno, int Tipo, string produtos, string statustitulo)
        {
            return BusinessSecretaria.getAlunos(cd_aluno, Tipo, produtos, statustitulo);
        }

        public List<sp_RptHistoricoAlunoM_Result> GetRtpHistoricoAlunoM(int cdAluno)
        {
            return BusinessSecretaria.getRtpHistoricoAlunoM(cdAluno);
        }

        public List<st_RptFaixaEtaria_Result> GetRtpFaixaEtaria(int cd_escola, int tipo, int idade, int idade_max, int cd_turma)
        {
            return BusinessSecretaria.getRtpFaixaEtaria(cd_escola, tipo, idade, idade_max, cd_turma);
        }

        public DataTable GetRtpFaixaEtariaDT(int cd_escola, int tipo, int idade, int idade_max, int cd_turma)
        {
            return BusinessSecretaria.getRtpFaixaEtariaDT(cd_escola, tipo, idade, idade_max, cd_turma);
        }

        public List<FollowUpRptUI> GetRtpFollowUp(byte id_tipo_follow, int cd_usuario_org, string no_usuario_org, int resolvido, int lido, DateTime? dtaIni, DateTime? dtaFinal, int cd_escola)
        {
            return BusinessSecretaria.GetRtpFollowUp(id_tipo_follow, cd_usuario_org, no_usuario_org, resolvido, lido, dtaIni, dtaFinal, cd_escola);
        }

        public List<FollowUpRptUI> GetSubRtpFollowUp(byte id_tipo_follow, int cd_usuario_org, string no_usuario_org, int resolvido, int lido, DateTime? dtaIni, DateTime? dtaFinal, int cd_escola)
        {
            return BusinessSecretaria.GetRtpFollowUp(id_tipo_follow, cd_usuario_org, no_usuario_org, resolvido, lido, dtaIni, dtaFinal, cd_escola);
        }

        public List<sp_RptAtividadeExtraAluno_Result> GetSourceAtividadeExtraAluno(Nullable<int> cd_atividade_extra, Nullable<int> cd_aluno, Nullable<byte> id_participou, Nullable<byte> id_lancada, Nullable<int> cd_escola)
        {
            return BusinessCoordenacao.getReportAtividadeExtraAluno(cd_atividade_extra, cd_aluno, id_participou, id_lancada, cd_escola);
        }

        public List<sp_RptAulaReposicaoAluno_Result> GetSourceAulaReposicaoAluno(Nullable<int> cd_aula_reposicao, Nullable<int> cd_aluno, Nullable<byte> id_participou)
        {
            return BusinessCoordenacao.getReportAulaReposicaoAluno(cd_aula_reposicao, cd_aluno, id_participou);
        }

        public List<TipoAtividadeExtra> GetSourceTipoAtividadeExtraAluno(Nullable<int> cd_tipo_atividade_extra, Nullable<int> cd_escola)
        {
            return BusinessCoordenacao.getTipoAtividade(true, cd_tipo_atividade_extra, cd_escola, TipoAtividadeExtraDataAccess.TipoConsultaAtivExtraEnum.HAS_DIARIO_AULA).ToList();
        }

        //public List<sp_RptInventario_Result> GetSourceRptInventario(Nullable<int> cd_escola, Nullable<System.DateTime> dt_analise, Nullable<byte> id_valor)
        public DataTable GetSourceRptInventario(Nullable<int> cd_escola, Nullable<System.DateTime> dt_analise, Nullable<byte> id_valor, string tipoItem)
        {
            return BusinessFinanceiro.getRtpInventario(cd_escola, dt_analise, id_valor, tipoItem);
        }
        public DataTable GetSourceRptAlunoRestricao(Nullable<int> cd_escola, Nullable<int> cd_orgao, Nullable<System.DateTime> dt_inicio, Nullable<System.DateTime> dt_final, Nullable<byte> tipodata)
        {
            return BusinessFinanceiro.getRptAlunoRestricao(cd_escola, cd_orgao, dt_inicio, dt_final, tipodata);
        }
        public DataTable GetSourceRptAvaliacao(int cd_turma, int cdCurso, int cdProduto, int cdEscola, int cdFuncionario, int tipoTurma, byte sitTurma, DateTime? pDtIni, DateTime? pDtFim, bool isConceito)
        {
            return BusinessCoordenacao.getRptAvaliacao(cd_turma, cdCurso, cdProduto, cdEscola, cdFuncionario, tipoTurma, sitTurma, pDtIni, pDtFim, isConceito);
        }
        public DataTable GetSourceRptAvaliacaoTurma(int cd_turma)
        {
            return BusinessCoordenacao.getRptAvaliacaoTurma(cd_turma);
        }
        public DataTable GetSourceRptAvaliacaoTurmaConceito(int cd_turma)
        {
            return BusinessCoordenacao.getRptAvaliacaoTurmaConceito(cd_turma);
        }

        public MemoryStream gerarDocWordOpenXml(int cd_escola, int cd_contrato, bool conta_segura, string caminho_relatorio, string file_name, ref int? nm_cliente_integracao, ref int? nm_contrato, ref string no_aluno)
        {
            string caminho_relatorios = caminho_relatorio;
            string nome_temp_guid = string.Empty;//nome do arquivo temporario para deletar depois
            string nome_contrato = ""; // "crwContrato_0001 - CONTRATO ALUNO ESTAGIO - NORMAL";
            Object oTemplatePath = ""; // caminho_relatorios + "/Contratos/" + cd_escola + "/" + nome_contrato + ".dotx";  
            string iText = "";
            var pathContrato = "";
            var caminho_temp_contrato = "";
            NomeContrato noCont = BusinessSecretaria.getNomeContratoAditamentoMatricula(cd_contrato, cd_escola);
            Table tab = null;

            //Consultas e definição dos caminhos (já existiam)
            if (noCont != null && noCont.cd_nome_contrato > 0)
            {
                nome_contrato = noCont.no_relatorio;
                if (noCont.cd_pessoa_escola == null)
                    pathContrato = caminho_relatorios + "/Contratos/" + noCont.no_relatorio;
                else
                    pathContrato = caminho_relatorios + "/Contratos/" + noCont.cd_pessoa_escola + "/" + noCont.no_relatorio;
            }
            else
                throw new MatriculaBusinessException(Utils.Messages.Messages.msgNotExistLayoutNoCont, null, MatriculaBusinessException.TipoErro.ERRO_NAO_EXISTE_LAYOUT_NOME_CONTRATO, false);

            //Pesquisa os dados do contrato em banco:
            Contrato contrato = BusinessMatricula.getContratoImpressao(cd_escola, cd_contrato);
            Parametro parametro = BusinessEscola.getParametrosBaixa(cd_escola);
            List<Titulo> titulosAbertos = BusinessFinanceiro.getTituloBaixaFinanSimulacao(new List<int>(), cd_escola, cd_contrato, TituloDataAccess.TipoConsultaTituloEnum.HAS_TITULOS_ABERTO_SIMULACAO);
            List<BaixaTitulo> baixasSimuladas = new List<BaixaTitulo>();
            List<BaixaTitulo.DiasPoliticaAntecipacao> descontosPoliticaTitulos = new List<BaixaTitulo.DiasPoliticaAntecipacao>();
            List<Aditamento> aditamentos = new List<Aditamento>();
            byte? nm_dia_vcto_desconto = null;
            if (contrato.Aditamento != null && contrato.Aditamento.Count() > 0)
            {
                aditamentos = contrato.Aditamento.ToList();

                nm_dia_vcto_desconto = aditamentos.OrderBy(a => a.dt_aditamento).Last().nm_dia_vcto_desconto;
                if (!nm_dia_vcto_desconto.HasValue)
                    throw new MatriculaBusinessException(Messages.msgErroDiaVencDescAditaNaoInformado, null,
                        MatriculaBusinessException.TipoErro.ERRO_DIA_VENC_ADITA_NAO_INFORMADO, false);
            }


            foreach (Titulo t in titulosAbertos)
            {
                BaixaTitulo baixa = new BaixaTitulo();
                if (nm_dia_vcto_desconto.HasValue)
                {
                    DateTime data_vcto_desconto = new DateTime();
                    //Caso não exista o dia, por exemplo, dia 31, tenta ainda o dia 30, 29 e 28:
                    bool encontrou_dia = false;
                    for (int k = 0; k < 4 && !encontrou_dia; k++)
                    {
                        try
                        {
                            data_vcto_desconto = new DateTime(t.dt_vcto_titulo.Year, t.dt_vcto_titulo.Month,
                                (int)nm_dia_vcto_desconto - k);
                            encontrou_dia = true;
                        }
                        catch (System.ArgumentOutOfRangeException)
                        {
                            encontrou_dia = false;
                        }
                    }

                    baixa.dt_baixa_titulo = data_vcto_desconto > t.dt_vcto_titulo ? t.dt_vcto_titulo.Date : data_vcto_desconto;
                }
                else
                    baixa.dt_baixa_titulo = t.dt_vcto_titulo.Date;
                BusinessCoordenacao.simularBaixaTitulo(t, ref baixa, parametro, cd_escola, conta_segura, true);
                t.BaixaTitulo.Add(baixa);
                if (baixa.diasPoliticaAntecipacao != null)
                {
                    descontosPoliticaTitulos.AddRange(baixa.diasPoliticaAntecipacao);
                    baixasSimuladas.Add(baixa);
                }
            }

            if (contrato != null)
            {
                bool houveErro = true;
                int macro = 0;
                try
                {

                    //Consultas e definição dos caminhos (já existiam)
                    nm_cliente_integracao = contrato.Escola.nm_cliente_integracao;
                    nm_contrato = contrato.nm_contrato;
                    no_aluno = contrato.Aluno.AlunoPessoaFisica.no_pessoa;
                    List<Titulo> titulosContratoContext = BusinessFinanceiro.getTitulosByContratoLeitura(contrato.cd_contrato, cd_escola).ToList();
                    int[] statusCnabTitulo = new int[] { (int)Titulo.StatusCnabTitulo.INICIAL, (int)Titulo.StatusCnabTitulo.CONFIRMADO_PEDIDO_BAIXA };
                    //cria o arquivo temporario do template(dotx)
                    caminho_temp_contrato = criarArquivoTempContratoOpenXml(pathContrato, caminho_relatorios, ref nome_temp_guid);
                    oTemplatePath = caminho_temp_contrato;
                    //byte[] result = null;
                    //converte o template do arquivo temporario em um byte de array
                    byte[] templateBytes = System.IO.File.ReadAllBytes(caminho_temp_contrato);
                    //cria um instancia de memoria para jogar o arquivo
                    using (MemoryStream templateStream = new MemoryStream())
                    {
                        //joga array de bytes do template(docx) temporario na memoria
                        templateStream.Write(templateBytes, 0, (int)templateBytes.Length);

                        //Começa o processamento do arquivo com o OpenXml (.Open -> abre o arquivo na memoria)
                        using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(templateStream, true))
                        {
                            //muda o tipo para documento (necessário para converter de dotx para docx)
                            wordDoc.ChangeDocumentType(WordprocessingDocumentType.Document);

                            //Pega a lista com o nome de todas as macros do arquivo(ex: NomeEscola -> <<NomeEscola>>)
                            List<string> merginFields = wordDoc.GetMergeFields().ToList().Select(x => x.InnerText).ToList();
                            List<string> simpleFields = wordDoc.MainDocumentPart.RootElement.Descendants<SimpleField>().ToList().Select(x => x.Instruction.Value).ToList();
                            List<FieldCode> fieldsC = new List<FieldCode>();
                            foreach (var fieldText in (merginFields.Union(simpleFields)))    //Colocar Concat se for usar o fieldDuplo
                            {
                                //Todas as macros no xml começam com o texto(" MERGEFIELD") -> ex: "... MERGEFIELD NomeEscola ..."
                                //com isso da pra navegar por todas macros com switch case(igual ao interop)
                                if (fieldText.StartsWith(" MERGEFIELD"))
                                {
                                    //Quebra o nome tirando o texto (MERGEFIELD) deixando apenas o nome para ser usado no swich

                                    // THE TEXT COMES IN THE FORMAT OF
                                    // MERGEFIELD  MyFieldName  \\* MERGEFORMAT
                                    // THIS HAS TO BE EDITED TO GET ONLY THE FIELDNAME "MyFieldName"
                                    Int32 endMerge = fieldText.IndexOf("\\");
                                    Int32 fieldNameLength = fieldText.Length - endMerge;
                                    String fieldName = fieldText.Substring(11, endMerge - 11);

                                    // GIVES THE FIELDNAMES AS THE USER HAD ENTERED IN .dot FILE
                                    fieldName = fieldName.Trim();
                                    String fieldDuplo = fieldName;

                                    //LBM Tentativa para repetir as Tags mas contornadas.
                                    endMerge = fieldDuplo.IndexOf("@");
                                    if (endMerge > 0)
                                        fieldDuplo = fieldDuplo.Substring(0, endMerge);

                                    // **** FIELD REPLACEMENT IMPLEMENTATION GOES HERE ****//
                                    // THE PROGRAMMER CAN HAVE HIS OWN IMPLEMENTATIONS HERE
                                    //myMergeField.Select();
                                    fieldsC = wordDoc.GetMergeFields(fieldName).ToList();
                                    //inicio do switch com as macros
                                    switch (fieldDuplo)  //Colocar fieldDuplo, se for usar
                                    {
                                        case "NomeEscola"://Nome_Escola
                                            macro = 1;
                                            string dc_reduzido_pessoa = contrato.Escola.dc_reduzido_pessoa;
                                            //Substitui a macro pelo dado das consulta do banco
                                            iText = String.IsNullOrEmpty(dc_reduzido_pessoa) ? " " : dc_reduzido_pessoa;
                                            if(fieldsC != null && fieldsC.Count() > 0)
                                                wordDoc.GetMergeFields(fieldName).ReplaceWithText(iText, true);
                                            else
                                                wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(iText, true);
                                            break;
                                        case "RazaoSocial": //Razao_social
                                            macro = 2;
                                            string no_pessoa = contrato.Escola.no_pessoa;
                                            iText = String.IsNullOrEmpty(no_pessoa) ? " " : no_pessoa;
                                            if(fieldsC != null && fieldsC.Count() > 0)
                                                wordDoc.GetMergeFields(fieldName).ReplaceWithText(iText);
                                            else
                                                wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(iText);
                                            break;
                                        case "CNPJEscola": //Cnpj_escola
                                            macro = 3;
                                            string dc_num_cgc = contrato.Escola.dc_num_cgc;
                                            iText = String.IsNullOrEmpty(dc_num_cgc) ? " " : dc_num_cgc;
                                            if(fieldsC != null && fieldsC.Count() > 0)
                                                wordDoc.GetMergeFields(fieldName).ReplaceWithText(iText);
                                            else
                                                wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(iText);
                                            break;
                                        case "EnderecoEscola": //End_Escola
                                            macro = 4;
                                            string no_localidade = contrato.Escola.EnderecoPrincipal.Logradouro.no_localidade;
                                            if (!String.IsNullOrEmpty(no_localidade))
                                            {
                                                if (!String.IsNullOrEmpty(contrato.Escola.EnderecoPrincipal.dc_num_endereco))
                                                    no_localidade += " Nº " + contrato.Escola.EnderecoPrincipal.dc_num_endereco;
                                                if (contrato.Escola.EnderecoPrincipal.Logradouro != null && contrato.Escola.EnderecoPrincipal.TipoLogradouro != null)
                                                    no_localidade = contrato.Escola.EnderecoPrincipal.TipoLogradouro.no_tipo_logradouro + " " + no_localidade;
                                            }
                                            else
                                                no_localidade = " ";
                                            iText = no_localidade;
                                            if(fieldsC != null && fieldsC.Count() > 0)
                                                wordDoc.GetMergeFields(fieldName).ReplaceWithText(iText);
                                            else
                                                wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(iText);
                                            break;
                                        case "CidadeEstadoEscola": //cidade_estado
                                            macro = 5;
                                            no_localidade = contrato.Escola.EnderecoPrincipal.Cidade.no_localidade;
                                            if (!String.IsNullOrEmpty(no_localidade))
                                            {
                                                if (!String.IsNullOrEmpty(contrato.Escola.EnderecoPrincipal.Estado.no_localidade))
                                                    no_localidade += " - " + contrato.Escola.EnderecoPrincipal.Estado.no_localidade;
                                            }
                                            else
                                                no_localidade = " ";
                                            iText = no_localidade;
                                            if(fieldsC != null && fieldsC.Count() > 0)
                                                wordDoc.GetMergeFields(fieldName).ReplaceWithText(iText);
                                            else
                                                wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(iText);
                                            break;

                                        case "NomeResponsavel": //Nom_resp
                                            macro = 6;
                                            no_pessoa = contrato.PessoaResponsavel.no_pessoa;
                                            iText = String.IsNullOrEmpty(no_pessoa) ? " " : no_pessoa;
                                            if(fieldsC != null && fieldsC.Count() > 0)
                                                wordDoc.GetMergeFields(fieldName).ReplaceWithText(iText, true);
                                            else
                                                wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(iText, true);
                                            break;
                                        case "RGResponsavel": //Rg_resp
                                            macro = 7;
                                            string nm_doc_identidade = contrato.PessoaResponsavel.nm_doc_identidade;
                                            iText = String.IsNullOrEmpty(nm_doc_identidade) ? " " : nm_doc_identidade;
                                            if(fieldsC != null && fieldsC.Count() > 0)
                                                wordDoc.GetMergeFields(fieldName).ReplaceWithText(iText, true);
                                            else
                                                wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(iText, true);
                                            break;
                                        case "CPFCNPJResponsavel": //Cpf_resp
                                            macro = 8;
                                            string nm_cpf_cgc = contrato.PessoaResponsavel.nm_cpf_cgc;
                                            iText = String.IsNullOrEmpty(nm_cpf_cgc) ? " " : nm_cpf_cgc;
                                            if(fieldsC != null && fieldsC.Count() > 0)
                                                wordDoc.GetMergeFields(fieldName).ReplaceWithText(iText, true);
                                            else
                                                wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(iText, true);
                                            break;

                                        case "TituloRGResponsavel": //
                                            macro = 9;
                                            iText = contrato.PessoaResponsavel.nm_natureza_pessoa == (int)PessoaSGF.TipoPessoa.FISICA ? "RG" : "";
                                            if(fieldsC != null && fieldsC.Count() > 0)
                                                wordDoc.GetMergeFields(fieldName).ReplaceWithText(iText);
                                            else
                                                wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(iText);
                                            break;
                                        case "TituloCPFouCNPJResponsavel": //
                                            macro = 10;
                                            iText = contrato.PessoaResponsavel.nm_natureza_pessoa == (int)PessoaSGF.TipoPessoa.FISICA ? "CPF" : "CNPJ";
                                            if(fieldsC != null && fieldsC.Count() > 0)
                                                wordDoc.GetMergeFields(fieldName).ReplaceWithText(iText);
                                            else
                                                wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(iText);
                                            break;
                                        case "TelefoneResponsavel": //tel_resp
                                            macro = 11;
                                            string dc_fone_mail = contrato.PessoaResponsavel.Telefone.dc_fone_mail;
                                            iText = String.IsNullOrEmpty(dc_fone_mail) ? " " : dc_fone_mail;
                                            if(fieldsC != null && fieldsC.Count() > 0)
                                                wordDoc.GetMergeFields(fieldName).ReplaceWithText(iText, true);
                                            else
                                                wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(iText, true);
                                            break;
                                        case "EnderecoResponsavel": //End_resp
                                            macro = 12;
                                            string enderecoCompleto = contrato.PessoaResponsavel.EnderecoPrincipal.enderecoCompleto;
                                            iText = String.IsNullOrEmpty(enderecoCompleto) ? " " : enderecoCompleto;
                                            if(fieldsC != null && fieldsC.Count() > 0)
                                                wordDoc.GetMergeFields(fieldName).ReplaceWithText(iText, true);
                                            else
                                                wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(iText, true);
                                            break;
                                        case "EmailResponsavel": //Email_responsavel
                                            macro = 13;
                                            string emailR = "";
                                            emailR = contrato.e_mail_responsavel; 
                                            iText = String.IsNullOrEmpty(emailR) ? " " : emailR;
                                            if (fieldsC != null && fieldsC.Count() > 0)
                                                wordDoc.GetMergeFields(fieldName).ReplaceWithText(iText, true);
                                            else
                                                wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(iText, true);
                                            break;
                                        case "CelularResponsavel": //TelCel
                                            macro = 14;
                                            string celularR = "";
                                            celularR = contrato.celular_responsavel;
                                            iText = String.IsNullOrEmpty(celularR) ? " " : celularR;
                                            if (fieldsC != null && fieldsC.Count() > 0)
                                                wordDoc.GetMergeFields(fieldName).ReplaceWithText(iText, true);
                                            else
                                                wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(iText, true);
                                            break;

                                        case "NomeAluno": //Nome_aluno
                                            macro = 15;
                                            string nomeAluno = contrato.Aluno.AlunoPessoaFisica.no_pessoa;
                                            iText = String.IsNullOrEmpty(nomeAluno) ? " " : nomeAluno;
                                            if(fieldsC != null && fieldsC.Count() > 0)
                                                wordDoc.GetMergeFields(fieldName).ReplaceWithText(iText, true);
                                            else
                                                wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(iText, true);
                                            break;

                                        case "TelelfoneAluno": //Tel_aluno - ( {qyRptContrato_001;1.num_tel_aluno} + ( if (isnull({qyRptContrato_001;1.num_telefone_contato_aluno}) or ({qyRptContrato_001;1.num_telefone_contato_aluno} = '')) then ''    else ( ' - ' + totext({qyRptContrato_001;1.num_telefone_contato_aluno}) +                    (  if (isnull({qyRptContrato_001;1.num_ramal1_aluno}) or ({qyRptContrato_001;1.num_ramal1_aluno} = '')) then ''                        else ('  ' + {qyRptContrato_001;1.num_ramal1_aluno}) ) ) ))
                                            macro = 16;
                                            dc_fone_mail = contrato.Aluno.AlunoPessoaFisica.Telefone.dc_fone_mail;
                                            iText = String.IsNullOrEmpty(dc_fone_mail) ? " " : dc_fone_mail;
                                            if(fieldsC != null && fieldsC.Count() > 0)
                                                wordDoc.GetMergeFields(fieldName).ReplaceWithText(iText, true);
                                            else
                                                wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(iText, true);
                                            break;

                                        case "RGAluno": //Rg_aluno
                                            macro = 17;
                                            nm_doc_identidade = contrato.Aluno.AlunoPessoaFisica.nm_doc_identidade;
                                            iText = String.IsNullOrEmpty(nm_doc_identidade) ? " " : nm_doc_identidade;
                                            if(fieldsC != null && fieldsC.Count() > 0)
                                                wordDoc.GetMergeFields(fieldName).ReplaceWithText(iText, true);
                                            else
                                                wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(iText, true);
                                            break;

                                        case "CPFAluno": //Cpf_aluno
                                            macro = 18;
                                            string nm_cpf = contrato.Aluno.AlunoPessoaFisica.nm_cpf;
                                            iText = String.IsNullOrEmpty(nm_cpf) ? " " : nm_cpf;
                                            if(fieldsC != null && fieldsC.Count() > 0)
                                                wordDoc.GetMergeFields(fieldName).ReplaceWithText(iText, true);
                                            else
                                                wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(iText, true);
                                            break;
                                        case "EstadoCivilAluno": //Est_civil_aluno
                                            macro = 19;
                                            string estadoCivil = contrato.Aluno.AlunoPessoaFisica.nm_sexo == (byte)PessoaSGF.Sexo.FEMININO ? contrato.Aluno.AlunoPessoaFisica.EstadoCivil.dc_estado_civil_fem : contrato.Aluno.AlunoPessoaFisica.EstadoCivil.dc_estado_civil_masc;
                                            iText = String.IsNullOrEmpty(estadoCivil) ? " " : estadoCivil;
                                            if(fieldsC != null && fieldsC.Count() > 0)
                                                wordDoc.GetMergeFields(fieldName).ReplaceWithText(iText, true);
                                            else
                                                wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(iText, true);
                                            break;
                                        case "DataNascimentoAluno": //Dta_nasc
                                            macro = 20;
                                            string dtaNascimento = contrato.Aluno.AlunoPessoaFisica.dtaNascimento;
                                            iText = String.IsNullOrEmpty(dtaNascimento) ? " " : dtaNascimento;
                                            if(fieldsC != null && fieldsC.Count() > 0)
                                                wordDoc.GetMergeFields(fieldName).ReplaceWithText(iText, true);
                                            else
                                                wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(iText, true);
                                            break;
                                        case "EnderecoAluno": //End_aluno
                                            macro = 21;
                                            enderecoCompleto = contrato.Aluno.AlunoPessoaFisica.EnderecoPrincipal.enderecoCompleto;
                                            iText = String.IsNullOrEmpty(enderecoCompleto) ? " " : enderecoCompleto;
                                            if(fieldsC != null && fieldsC.Count() > 0)
                                                wordDoc.GetMergeFields(fieldName).ReplaceWithText(iText, true);
                                            else
                                                wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(iText, true);
                                            break;
                                        case "EmailAluno": //Email_aluno
                                            macro = 22;
                                            string email = "";
                                            email = contrato.Aluno.AlunoPessoaFisica.email;
                                            iText = String.IsNullOrEmpty(email) ? " " : email;
                                            if(fieldsC != null && fieldsC.Count() > 0)
                                                wordDoc.GetMergeFields(fieldName).ReplaceWithText(iText, true);
                                            else
                                                wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(iText, true);
                                            break;
                                        case "CelularAluno": //TelCel
                                            macro = 23;
                                            string celular = "";
                                            celular = contrato.Aluno.AlunoPessoaFisica.celular;
                                            iText = String.IsNullOrEmpty(celular) ? " " : celular;
                                            if(fieldsC != null && fieldsC.Count() > 0)
                                                wordDoc.GetMergeFields(fieldName).ReplaceWithText(iText, true);
                                            else
                                                wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(iText, true);
                                            break;

                                        case "TituloCurso": //curso_label - ( if ({qyRptContrato_001;1.nom_idioma} in ['Inglês', 'Espanhol']) then 'ESTÁGIO: ' else  'MÓDULO: ' )
                                            macro = 24;
                                            List<string> listaProdutos = new List<string>();
                                            listaProdutos.Add("Inglês");
                                            listaProdutos.Add("Espanhol");
                                            iText = listaProdutos.Contains(contrato.Produto.no_produto) ? "Estágio" : "Módulo";
                                            if(fieldsC != null && fieldsC.Count() > 0)
                                                wordDoc.GetMergeFields(fieldName).ReplaceWithText(iText, true);
                                            else
                                                wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(iText, true);
                                            break;
                                        case "Curso": //curso
                                            macro = 25;
                                            string no_curso = contrato.Curso.no_curso;
                                            iText = String.IsNullOrEmpty(no_curso) ? " " : no_curso;
                                            if(fieldsC != null && fieldsC.Count() > 0)
                                                wordDoc.GetMergeFields(fieldName).ReplaceWithText(iText, true);
                                            else
                                                wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(iText, true);
                                            break;
                                        case "ComplementoCursoComMinutosTurma": //Turma60Minutos - if ({qyRptContrato_001;1.ind_P60} = 1) then 'TURMA DE 60 MINUTOS' else if ({qyRptContrato_001;1.ind_R60} = 1) then 'TURMA DE 60 MINUTOS' else if ({qyRptContrato_001;1.qtd_minutos_div} > 0) then ('TURMA DE ' + ToText({qyRptContrato_001;1.qtd_minutos_div}, 0) + ' MINUTOS') else ''
                                            macro = 26;
                                            string complemento = " ";
                                            no_curso = contrato.Curso.no_curso;

                                            if (no_curso.Contains("R60") || no_curso.Contains("R60"))
                                                complemento += "TURMA DE 60 MINUTOS";
                                            else if (contrato.qtd_minutos_turma > 0)
                                                complemento += "TURMA DE " + contrato.qtd_minutos_turma + " MINUTOS";
                                            iText = complemento;
                                            if(fieldsC != null && fieldsC.Count() > 0)
                                                wordDoc.GetMergeFields(fieldName).ReplaceWithText(iText);
                                            else
                                                wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(iText);
                                            break;

                                        case "DiasHorariosCurso": //HorariosTurmaProc.rpt
                                            macro = 27;
                                            string dias_horarios = "";

                                            if (contrato.hashtableHorarios != null)
                                                dias_horarios = Horario.getDescricaoCompletaHorarios(contrato.hashtableHorarios);
                                            iText = String.IsNullOrEmpty(dias_horarios) ? " " : dias_horarios;
                                            if(fieldsC != null && fieldsC.Count() > 0)
                                                wordDoc.GetMergeFields(fieldName).ReplaceWithText(iText);
                                            else
                                                wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(iText);
                                            break;
                                        case "Dias": //HorariosTurmaProc.rpt
                                            #region Dias
                                            macro = 28;
                                            dias_horarios = "";
                                            if (contrato.hashtableHorarios != null)
                                            {
                                                List<Horario> listaHorariosOrdenados = new List<Horario>();
                                                foreach (DictionaryEntry entry in contrato.hashtableHorarios)
                                                    listaHorariosOrdenados.Add(new Horario { desc_concat_dias_semena = entry.Value.ToString() });

                                                listaHorariosOrdenados = listaHorariosOrdenados.OrderBy(h => h.desc_concat_dias_semena).ThenBy(h => h.no_registro).ToList();
                                                foreach (Horario entry in listaHorariosOrdenados)
                                                {
                                                    string[] array = entry.desc_concat_dias_semena.Split(',');
                                                    for (int i = 0; i < array.Length; i++)
                                                        array[i] = Horario.getDiaSemanaPorDia(array[i]);
                                                    entry.desc_concat_dias_semena = string.Join(", ", array);
                                                }

                                                List<string> listaStrHorariosOrdenados = (from h in listaHorariosOrdenados orderby h.id_dia_semana ascending select h.desc_concat_dias_semena).Distinct().ToList();
                                                foreach (string s in listaStrHorariosOrdenados)
                                                    dias_horarios += "; " + s;

                                                if (dias_horarios.Length >= 2)
                                                    dias_horarios = dias_horarios.Substring(2, dias_horarios.Length - 2);
                                            }
                                            iText = String.IsNullOrEmpty(dias_horarios) ? " " : dias_horarios;
                                            if(fieldsC != null && fieldsC.Count() > 0)
                                                wordDoc.GetMergeFields(fieldName).ReplaceWithText(iText);
                                            else
                                                wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(iText);
                                            #endregion
                                            break;
                                        case "HorariosCurso":
                                            macro = 29;
                                            dias_horarios = "";
                                            if (contrato.hashtableHorarios != null)
                                                dias_horarios = Horario.getDescricaoSimplificadaHorarios(contrato.hashtableHorarios);
                                            iText = String.IsNullOrEmpty(dias_horarios) ? " " : dias_horarios;
                                            if(fieldsC != null && fieldsC.Count() > 0)
                                                wordDoc.GetMergeFields(fieldName).ReplaceWithText(iText);
                                            else
                                                wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(iText);
                                            break;
                                        case "DataInicioAulas": //Ini_Aula
                                            macro = 30;
                                            string dtInicialContrato = contrato.dtInicialContrato;
                                            iText = String.IsNullOrEmpty(dtInicialContrato) ? " " : dtInicialContrato;
                                            if(fieldsC != null && fieldsC.Count() > 0)
                                                wordDoc.GetMergeFields(fieldName).ReplaceWithText(iText);
                                            else
                                                wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(iText);
                                            break;
                                        case "MatriculaRematricula": //Mat_Remat
                                            macro = 31;
                                            string vlMatriculaContrato = contrato.vlMatriculaContrato;
                                            iText = String.IsNullOrEmpty(vlMatriculaContrato) ? " " : vlMatriculaContrato;
                                            if(fieldsC != null && fieldsC.Count() > 0)
                                                wordDoc.GetMergeFields(fieldName).ReplaceWithText(iText);
                                            else
                                                wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(iText);
                                            break;
                                        case "ValorSemDesconto":
                                            macro = 32;
                                            decimal vlMaterialMatricula = 0;
                                            decimal vlSemDesconto = contrato.vl_curso_contrato / contrato.nm_parcelas_mensalidade;
                                            byte nm_parcelas_material = 0;

                                            if (contrato.nm_parcelas_material > 0)
                                            {
                                                nm_parcelas_material = (byte)contrato.nm_parcelas_material;
                                                vlMaterialMatricula = (decimal)contrato.vl_material_contrato;
                                                if (nm_parcelas_material > 0) //Evitar divisão por zero
                                                    if(fieldsC != null && fieldsC.Count() > 0)
                                                        wordDoc.GetMergeFields(fieldName).ReplaceWithText(string.Format("{0:#,0.00}", decimal.Round(vlSemDesconto + vlMaterialMatricula / nm_parcelas_material, 2)));
                                                    else
                                                        wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(string.Format("{0:#,0.00}", decimal.Round(vlSemDesconto + vlMaterialMatricula / nm_parcelas_material, 2)));
                                                else
                                                    if(fieldsC != null && fieldsC.Count() > 0)
                                                        wordDoc.GetMergeFields(fieldName).ReplaceWithText(string.Format("{0:#,0.00}", decimal.Round(vlSemDesconto, 2)));
                                                    else
                                                        wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(string.Format("{0:#,0.00}", decimal.Round(vlSemDesconto, 2)));
                                            }
                                            else
                                                if(fieldsC != null && fieldsC.Count() > 0)
                                                    wordDoc.GetMergeFields(fieldName).ReplaceWithText(string.Format("{0:#,0.00}", decimal.Round(vlSemDesconto, 2)));
                                                else
                                                    wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(string.Format("{0:#,0.00}", decimal.Round(vlSemDesconto, 2)));
                                            break;
                                        case "NroVencimento": //dta_venc
                                            macro = 33;
                                            iText = contrato.nm_dia_vcto + " ";
                                            if(fieldsC != null && fieldsC.Count() > 0)
                                                wordDoc.GetMergeFields(fieldName).ReplaceWithText(iText);
                                            else
                                                wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(iText);
                                            break;
                                        case "NroVencimentoComDesconto": //dta_venc_desc - des_vencto_com_desconto
                                            macro = 34;
                                            byte? dtaVctoAditamento = 0;
                                            if (aditamentos.Count > 0)
                                                dtaVctoAditamento = aditamentos.OrderBy(a => a.dt_aditamento).Last().nm_dia_vcto_desconto;
                                            iText = !dtaVctoAditamento.HasValue ? " " : dtaVctoAditamento + "";
                                            if(fieldsC != null && fieldsC.Count() > 0)
                                                wordDoc.GetMergeFields(fieldName).ReplaceWithText(iText);
                                            else
                                                wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(iText);
                                            break;
                                        case "ValorComDesconto": //val_com_desc
                                            #region Valor Com Desconto
                                            macro = 35;
                                            nm_parcelas_material = 0;
                                            string valor_com_desconto = " ";
                                            vlMaterialMatricula = 0;
                                            //bool aplicarMaterialBaixa;
                                            BaixaTitulo baixa = new BaixaTitulo();
                                            if (contrato.vl_parcela_contrato > 0)
                                            {
                                                //Caso quando não é aditamento
                                                Aditamento aditamento = new Aditamento();
                                                if (aditamentos != null && aditamentos.Count()>0)
                                                    aditamento = aditamentos.OrderBy(a => a.dt_aditamento).Last();
                                                Titulo titulo = new Titulo();
                                                //Caso o contrato não seja de aditamento o valor informado será o número de parcelas do contrato.
                                                //if (aditamento.id_tipo_aditamento == null || aditamentos.Count <= 0)
                                                if(titulosAbertos.Count() == 0)
                                                {
                                                    BusinessEscola.simularBaixaContrato(contrato, ref baixa, cd_escola);
                                                }
                                                else
                                                {
                                                    //Se for de aditamento com o tipo diferente de “Adicionar parcelas”, será a quantidade de títulos em aberto.
                                                    if ((aditamento != null && aditamento.id_tipo_aditamento.HasValue && aditamento.id_tipo_aditamento.Value != (byte)Aditamento.TipoAditamento.ADICIONAR_PARCELAS) || (aditamento == null || !aditamento.id_tipo_aditamento.HasValue))
                                                    {
                                                        //titulosContratoContext = Titulo.revisarSaldoTituloBolsaContrato(titulosContratoContext.ToList(), false, 0, false);
                                                        //Antes estava titulosContratoContext ao inves de titulosAbertos
                                                        List<Titulo> titulosAbertosContrato = titulosAbertos.Where(x => x.id_status_titulo == (int)Titulo.StatusTitulo.ABERTO &&
                                                                                                                                statusCnabTitulo.Contains(x.id_status_cnab) &&
                                                                                                                                //Como Titulos com baixas parciais não são escolhidos não precisamos testar o valor
                                                                                                                                //por causa das baixas motivo bolsa
                                                                                                                                //x.vl_titulo == x.vl_saldo_titulo &&
                                                                                                                                x.dc_tipo_titulo != "TM" && x.dc_tipo_titulo != "TA" &&
                                                                                                                                x.dc_tipo_titulo != "AD" && x.dc_tipo_titulo != "AA").ToList();
                                                        titulo = titulosAbertosContrato.OrderBy(x => x.nm_parcela_titulo).FirstOrDefault();
                                                        if (titulo != null)
                                                            titulo.BaixaTitulo = titulosAbertosContrato.OrderBy(x => x.nm_parcela_titulo).FirstOrDefault().BaixaTitulo;
                                                        //if (titulo != null)
                                                        //{
                                                        //    nm_dia_vcto_desconto = aditamento.nm_dia_vcto_desconto;
                                                        //    if (nm_dia_vcto_desconto.HasValue)
                                                        //    {
                                                        //        DateTime data_vcto_desconto = new DateTime();
                                                        //        //Caso não exista o dia, por exemplo, dia 31, tenta ainda o dia 30, 29 e 28:
                                                        //        bool encontrou_dia = false;
                                                        //        for (int k = 0; k < 4 && !encontrou_dia; k++)
                                                        //        {
                                                        //            try
                                                        //            {
                                                        //                data_vcto_desconto = new DateTime(titulo.dt_vcto_titulo.Year, titulo.dt_vcto_titulo.Month,
                                                        //                    (int)nm_dia_vcto_desconto - k);
                                                        //                encontrou_dia = true;
                                                        //            }
                                                        //            catch (System.ArgumentOutOfRangeException)
                                                        //            {
                                                        //                encontrou_dia = false;
                                                        //            }
                                                        //        }

                                                        //        baixa.dt_baixa_titulo = data_vcto_desconto;
                                                        //    }
                                                        //    else
                                                        //        throw new MatriculaBusinessException(Messages.msgErroDiaVencDescAditaNaoInformado, null,
                                                        //            MatriculaBusinessException.TipoErro.ERRO_DIA_VENC_ADITA_NAO_INFORMADO, false);
                                                        //}
                                                        //BusinessCoordenacao.simularBaixaTitulo(titulo, ref baixa, parametro, cd_escola, false, false);
                                                    }
                                                    else //Caso seja aditamento com tipo “Adicionar Parcelas” o valor será o número de títulos informado no Aditivo.
                                                    if (aditamento != null && aditamento.id_tipo_aditamento.HasValue && aditamento.id_tipo_aditamento.Value == (byte)Aditamento.TipoAditamento.ADICIONAR_PARCELAS)
                                                    {
                                                        List<Titulo> titulosAbertosContrato = titulosAbertos.Where(x => x.id_status_titulo == (int)Titulo.StatusTitulo.ABERTO &&
                                                                                                                                statusCnabTitulo.Contains(x.id_status_cnab) &&
                                                                                                                                x.vl_titulo == x.vl_saldo_titulo &&
                                                                                                                                (x.dc_tipo_titulo == "AD" || x.dc_tipo_titulo == "AA")).ToList();
                                                        //titulosAbertosContrato = titulosAbertosContrato.OrderBy(x => x.nm_parcela_titulo).ToList();
                                                        titulo = titulosAbertosContrato.OrderBy(x => x.nm_parcela_titulo).FirstOrDefault();
                                                        if (titulo != null) 
                                                        { 
                                                            titulo.BaixaTitulo = titulosAbertosContrato.OrderBy(x => x.nm_parcela_titulo).FirstOrDefault().BaixaTitulo;
                                                            //if (titulosAbertosContrato != null && titulosAbertosContrato.Count() > 0)
                                                            //{
                                                            //aplicarMaterialBaixa = false;
                                                            //    BusinessCoordenacao.simularBaixaTitulo(titulosAbertosContrato.FirstOrDefault(), ref baixa,
                                                            //        parametro, cd_escola, false, false);
                                                            //}
                                                        }
                                                    }
                                                }
                                                Decimal valorbaixaDesc = (titulosAbertos.Count() == 0 || titulo == null) ? baixa.vl_liquidacao_baixa : titulo.BaixaTitulo.FirstOrDefault().vl_liquidacao_baixa;
                                                valor_com_desconto = string.Format("{0:#,0.00}", decimal.Round(valorbaixaDesc, 2));
                                             //   if (aplicarMaterialBaixa) //o valor da baixa já computa o valor do material
                                             //   {
                                             //       if (contrato.vl_material_contrato > 0)
	                                            //    {
	                                            //        nm_parcelas_material = (byte)contrato.nm_parcelas_material;
	                                            //        vlMaterialMatricula = (decimal)contrato.vl_material_contrato;
	                                            //    }
	                                            //    if (baixa != null && nm_parcelas_material > 0)
	                                            //        valor_com_desconto = string.Format("{0:#,0.00}", decimal.Round(valorbaixaDesc + vlMaterialMatricula / nm_parcelas_material, 2));
	                                            //    else
	                                            //        valor_com_desconto = string.Format("{0:#,0.00}", decimal.Round(valorbaixaDesc, 2));
	                                            //}
                                             //   else
                                             //       valor_com_desconto = string.Format("{0:#,0.00}", decimal.Round(valorbaixaDesc, 2));
                                            }
                                            iText = valor_com_desconto + " ";
                                            if(fieldsC != null && fieldsC.Count() > 0)
                                                wordDoc.GetMergeFields(fieldName).ReplaceWithText(iText);
                                            else
                                                wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(iText);
                                            #endregion
                                            break;
                                        case "NroParcelasMaterial":
                                            macro = 36;
                                            nm_parcelas_material = (byte)contrato.nm_parcelas_material;
                                            iText = nm_parcelas_material + "";
                                            if(fieldsC != null && fieldsC.Count() > 0)
                                                wordDoc.GetMergeFields(fieldName).ReplaceWithText(iText);
                                            else
                                                wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(iText);
                                            break;
                                        case "ValorMaterial":
                                            macro = 37;
                                            vlMaterialMatricula = (decimal)(contrato.vl_material_contrato == null ? 0 : contrato.vl_material_contrato);
                                            iText = string.Format("{0:#,0.00}", decimal.Round(vlMaterialMatricula, 2));
                                            if(fieldsC != null && fieldsC.Count() > 0)
                                                wordDoc.GetMergeFields(fieldName).ReplaceWithText(iText);
                                            else
                                                wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(iText);
	                                        break;
        		                        case "ValorComDescontoMaterial":
                                            macro = 38;
                                            decimal vl_parcela_liq_material = (decimal)(contrato.vl_parcela_liq_material == null ? 0 : contrato.vl_parcela_liq_material);
                                            iText = string.Format("{0:#,0.00}", decimal.Round(vl_parcela_liq_material, 2));
                                            if(fieldsC != null && fieldsC.Count() > 0)
                                                wordDoc.GetMergeFields(fieldName).ReplaceWithText(iText);
                                            else
                                                wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(iText);
        	                                break;
	       		                        case "ValorCurso":
                                            macro = 39;
                                            iText = string.Format("{0:#,0.00}", contrato.vl_curso_contrato);
                                            if(fieldsC != null && fieldsC.Count() > 0)
                                                wordDoc.GetMergeFields(fieldName).ReplaceWithText(iText);
                                            else
                                                wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(iText);
                                            break;
                                        case "NroParcelasCurso":
                                            macro = 40;
                                            iText = contrato.nm_parcelas_mensalidade + "";
;                                            if(fieldsC != null && fieldsC.Count() > 0)
                                                wordDoc.GetMergeFields(fieldName).ReplaceWithText(iText);
                                            else
                                                wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(iText);
                                            break;
                                        case "BolsaMaterial":
                                            macro = 41;
                                            decimal pc_bolsa_material = 0;
                                            pc_bolsa_material = (decimal)contrato.pc_bolsa_material;
                                            iText = string.Format("{0:#,0.00}", decimal.Round(pc_bolsa_material, 2));
                                            if(fieldsC != null && fieldsC.Count() > 0)
                                                wordDoc.GetMergeFields(fieldName).ReplaceWithText(iText);
                                            else
                                                wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(iText);
                                            break;
                                        case "GradeCursos":
                                            #region CursosContrato
                                            macro = 42;
                                            TableGrid tableGrid = OpenXmlWordHelpers.CreateTableGrid(4);
                                            int li = 0;
                                            List<CursoContrato> listaCursoContrato = BusinessMatricula.getCursoContrato(contrato.cd_contrato);
                                            listaCursoContrato = listaCursoContrato != null && listaCursoContrato.Count > 0 ? listaCursoContrato.OrderBy(x => x.cd_curso_ordem).ToList() : listaCursoContrato;
                                            if (listaCursoContrato != null && listaCursoContrato.Count > 0)
                                            {
                                                for (int i = 1; i <= listaCursoContrato.Count; i++)
                                                {
                                                    //Instancia a tabela(OpenXml)
                                                    if (li == 0)
                                                    {
                                                        tab = new Table();
                                                        OpenXmlWordHelpers.SetTableStyle(tab);
                                                        TableGrid tableGrid1 = new TableGrid(tableGrid.CloneNode(true));
                                                        tab.AppendChild(tableGrid1);
                                                    }
                                                    string ccomplemento = " ";
                                                    string no_cursoc = listaCursoContrato[li].no_curso;

                                                    if (no_cursoc.Contains("R60"))
                                                        ccomplemento += "TURMA DE 60 MINUTOS";
                                                    else if (contrato.qtd_minutos_turma > 0)
                                                        ccomplemento += "TURMA DE " + contrato.qtd_minutos_turma + " MINUTOS";
                                                    string dc_regime_turmac = string.IsNullOrEmpty(contrato.dc_regime_turma) ? " " : contrato.dc_regime_turma;
                                                    string dcmes = MesExtenso((int)listaCursoContrato[li].nm_mes_curso_inicial);
                                                    //Adiciona celulas na linha(header ->colunas)
                                                    // Linha 1
                                                    TableRow row1 = new TableRow(new TableRowProperties(new TableRowHeight { Val = 340, HeightType = HeightRuleValues.AtLeast }));
                                                    row1.Append(OpenXmlWordHelpers.ConfigureWidthSpan(OpenXmlWordHelpers.CreateCell2R("CURSO: ", no_cursoc + "  " + ccomplemento), "100%", true, 4));
                                                    tab.AppendChild(row1);
                                                    //Linha 2
                                                    TableRow row2 = new TableRow(new TableRowProperties(new TableRowHeight { Val = 340, HeightType = HeightRuleValues.AtLeast }));
                                                    row2.Append(OpenXmlWordHelpers.ConfigureWidthPct(OpenXmlWordHelpers.CreateCell2R("MODALIDADE: ", dc_regime_turmac), "33%"));
                                                    row2.Append(OpenXmlWordHelpers.ConfigureWidthSpan(OpenXmlWordHelpers.CreateCell2R("INICIO: ", MesExtenso((int)listaCursoContrato[li].nm_mes_curso_inicial) + listaCursoContrato[li].nm_ano_curso_inicial), "34%", true, 2));
                                                    row2.Append(OpenXmlWordHelpers.ConfigureWidthPct(OpenXmlWordHelpers.CreateCell2R("TÉRMINO: ", MesExtenso((int)listaCursoContrato[li].nm_mes_curso_final) + listaCursoContrato[li].nm_ano_curso_final), "33%"));
                                                    tab.AppendChild(row2);
                                                    //Linha 3
                                                    TableRow row3 = new TableRow(new TableRowProperties(new TableRowHeight { Val = 340, HeightType = HeightRuleValues.AtLeast }));
                                                    row3.Append(OpenXmlWordHelpers.ConfigureWidthSpan(OpenXmlWordHelpers.CreateCell2R("PARCELAS: ", listaCursoContrato[li].nm_parcelas_mensalidade + " x " + string.Format("{0:#,0.00}",listaCursoContrato[li].vl_parcela_contrato)), "50%", true, 2));
                                                    row3.Append(OpenXmlWordHelpers.ConfigureWidthSpan(OpenXmlWordHelpers.CreateCell2R("TIPO FINANCEIRO: ", listaCursoContrato[li].no_tipo_financeiro), "50%", true, 2));
                                                    tab.AppendChild(row3);
                                                    //Linha 4
                                                    TableRow row4 = new TableRow(new TableRowProperties(new TableRowHeight { Val = 340, HeightType = HeightRuleValues.AtLeast }));
                                                    if (listaCursoContrato[li].nm_parcelas_material > 0 && !(bool)listaCursoContrato[li].id_valor_incluso)
                                                    {
                                                        row4.Append(OpenXmlWordHelpers.ConfigureWidthPct(OpenXmlWordHelpers.CreateCell2R("MATERIAL: ", listaCursoContrato[li].nm_parcelas_material + " x " + string.Format("{0:#,0.00}", listaCursoContrato[li].vl_parcela_material) + ((bool)listaCursoContrato[li].id_valor_incluso ? "  Incluso" : "")), "33%"));
                                                        row4.Append(OpenXmlWordHelpers.ConfigureWidthSpan(OpenXmlWordHelpers.CreateCell4R("BOLSA: ", (listaCursoContrato[li].pc_bolsa_material > 0 ? string.Format("{0:#,0.0}", listaCursoContrato[li].pc_bolsa_material) + "%" : "0% "), " PARCELA LÍQUIDA: ", string.Format("{0:#,0.00}", listaCursoContrato[li].vl_parcela_liq_material)), "34%", true, 2));
                                                        row4.Append(OpenXmlWordHelpers.ConfigureWidthPct(OpenXmlWordHelpers.CreateCell2R("TOTAL: ", (listaCursoContrato[li].vl_parcela_liq_material == listaCursoContrato[li].vl_parcela_material ? string.Format("{0:#,0.00}", listaCursoContrato[li].vl_material_contrato) : string.Format("{0:#,0.00}", Math.Round((decimal)(listaCursoContrato[li].vl_parcela_liq_material * listaCursoContrato[li].nm_parcelas_material), 2, MidpointRounding.AwayFromZero))) + ((bool)listaCursoContrato[li].id_incorporar_valor_material ? "  Incorporado" : "")), "33%"));
                                                    }
                                                    else
                                                    {
                                                        row4.Append(OpenXmlWordHelpers.ConfigureWidthPct(OpenXmlWordHelpers.CreateCell2R("MATERIAL: ", ((bool)listaCursoContrato[li].id_valor_incluso ? "  Incluso" : "")), "33%"));
                                                        row4.Append(OpenXmlWordHelpers.ConfigureWidthSpan(OpenXmlWordHelpers.CreateCell4R("BOLSA:    ", " ", "    PARCELA LÍQUIDA: ", ""), "34%", true, 2));
                                                        row4.Append(OpenXmlWordHelpers.ConfigureWidthPct(OpenXmlWordHelpers.CreateCell2R("TOTAL: ", listaCursoContrato[li].vl_material_contrato == 0 ? "" : string.Format("{0:#,0.00}", listaCursoContrato[li].vl_material_contrato) + ((bool)listaCursoContrato[li].id_incorporar_valor_material ? "  Incorporado" : "")), "33%"));
                                                    }

                                                    tab.AppendChild(row4);
                                                    if (li < listaCursoContrato.Count - 1)
                                                    {
                                                        // Linha 5
                                                        TableRow row5 = new TableRow(new TableRowProperties(new TableRowHeight {Val = 340, HeightType = HeightRuleValues.AtLeast}));
                                                        row5.Append(OpenXmlWordHelpers.ConfigureWidthSpan(OpenXmlWordHelpers.CreateCell(" "), "100%", true, 4));
                                                        tab.AppendChild(row5);
                                                    }
                                                    li++;

                                                    // Propriedade da Row, Ver depois
                                                    //tab.Rows.Height = 20;
                                                    // Estas Formatações são do parágrafo veremos no futuro
                                                    //tab.Range.Font.Size = 8;
                                                    //tab.Range.Font.Name = "Arial Narrow";
                                                    //tab.Cell(1, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                                                    //tab.Cell(1, 1).VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalTop;
                                                    //tab.PreferredWidthType = Word.WdPreferredWidthType.wdPreferredWidthPoints;
                                                    //tab.PreferredWidth = 530;
                                                    //tab.AllowPageBreaks = true;
                                                    //tab.AllowAutoFit = false;

                                                    //Insere na tabela após a macro
                                                    //substitui a macro por texto
                                                }
                                                //Table tab1 = new Table(tab.CloneNode(true));
                                                //wordDoc.MainDocumentPart.Document.Body.AppendChild(tab);
                                                //if (li < listaCursoContrato.Count - 1)
                                                //{
                                                //    Paragraph para = wordDoc.MainDocumentPart.Document.Body.AppendChild(new Paragraph());
                                                //    Run run = para.AppendChild(new Run(new Text(" ")));
                                                //}
                                                if (fieldsC != null && fieldsC.Count() > 0)
                                                {
                                                    wordDoc.GetMergeFields(fieldName).FirstOrDefault().GetParagraph().InsertAfterSelf(tab);
                                                    wordDoc.GetMergeFields(fieldName).ReplaceWithText(" ");
                                                }
                                                else
                                                {
                                                    wordDoc.GetSimpleFields(fieldName).FirstOrDefault().GetParagraph().InsertAfterSelf(tab);
                                                    wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(" ");
                                                }
                                            }
                                            else
                                            {
                                                iText = " ";
                                                if (fieldsC != null && fieldsC.Count() > 0)
                                                    wordDoc.GetMergeFields(fieldName).ReplaceWithText(iText);
                                                else
                                                    wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(iText);
                                            }
                                            #endregion
                                        break;
                                        case "GradeValoresParcelas":
                                            macro = 43;
                                            List<Titulo> listaTitulos = BusinessFinanceiro.getTitulosContrato(contrato.cd_contrato);
                                            #region  ValoresParcelas
                                            if (listaTitulos != null && listaTitulos.Count > 0)
                                            {
                                                //Instancia a tabela(OpenXml)
                                                Table tableValoresParcelas = new Table();
                                                //Instancia um Linha da tabela(OpenXml)
                                                TableGrid tableGridV = OpenXmlWordHelpers.CreateTableGrid(5);
                                                OpenXmlWordHelpers.SetTableStyle(tableValoresParcelas);
                                                tableValoresParcelas.AppendChild(tableGridV);

                                                TableRow row = null;
                                                row = new TableRow();
                                                row.Append(OpenXmlWordHelpers.ConfigureWidth(OpenXmlWordHelpers.CreateCell("VENCIMENTO" ), "3000"));
                                                row.Append(OpenXmlWordHelpers.ConfigureWidth(OpenXmlWordHelpers.CreateCell("DIA" ), "3000"));
                                                row.Append(OpenXmlWordHelpers.ConfigureWidth(OpenXmlWordHelpers.CreateCell("MATERIAL (R$)" ), "2000"));
                                                row.Append(OpenXmlWordHelpers.ConfigureWidth(OpenXmlWordHelpers.CreateCell("PARCELA (R$)" ), "200"));
                                                row.Append(OpenXmlWordHelpers.ConfigureWidth(OpenXmlWordHelpers.CreateCell("TOTAL (R$)" ), "200"));
                                                //Adicona a linha na tabela
                                                tableValoresParcelas.AppendChild(row);

                                                nm_parcelas_material = 0;
                                                vl_parcela_liq_material = 0;
                                                int i, row_aux;

                                                if (contrato.vl_material_contrato > 0)
                                                {
                                                    nm_parcelas_material = (byte)contrato.nm_parcelas_material;
                                                    vl_parcela_liq_material = (decimal)contrato.vl_parcela_liq_material;
                                                }

                                                for (i = 0, row_aux = 2; i < listaTitulos.Count; i++, row_aux++)
                                                {
                                                    row = new TableRow();
                                                    row.Append(OpenXmlWordHelpers.CreateCell(listaTitulos[i].nm_parcela_titulo + "º"));
                                                    row.Append(OpenXmlWordHelpers.CreateCell(listaTitulos[i].dt_vcto_titulo.ToString("dd/MM/yyyy")));
                                                    if (nm_parcelas_material > 0)
                                                    {
                                                        row.Append(OpenXmlWordHelpers.CreateCell(string.Format("{0:#,0.00}", listaTitulos[i].vl_material_titulo)));
                                                        row.Append(OpenXmlWordHelpers.CreateCell(string.Format("{0:#,0.00}", listaTitulos[i].vl_titulo - listaTitulos[i].vl_material_titulo)));
                                                        row.Append(OpenXmlWordHelpers.CreateCell(string.Format("{0:#,0.00}", listaTitulos[i].vl_titulo)));
                                                        //nm_parcelas_material -= 1;
                                                    }
                                                    else
                                                    {
                                                        row.Append(OpenXmlWordHelpers.CreateCell(""));
                                                        row.Append(OpenXmlWordHelpers.CreateCell(string.Format("{0:#,0.00}", listaTitulos[i].vl_titulo)));
                                                        row.Append(OpenXmlWordHelpers.CreateCell(string.Format("{0:#,0.00}", listaTitulos[i].vl_titulo)));
                                                    }

                                                    tableValoresParcelas.AppendChild(row);
                                                }
                                                row = new TableRow();
                                                row.Append(OpenXmlWordHelpers.CreateCell(""));
                                                row.Append(OpenXmlWordHelpers.CreateCell("Total:"));
                                                if (contrato.vl_material_contrato > 0)
                                                {
                                                    //if ((bool)contrato.id_valor_incluso && contrato.vl_material_contrato > 0)
                                                        row.Append(OpenXmlWordHelpers.CreateCell(string.Format("{0:#,0.00}", contrato.vl_material_contrato)));
                                                    //else
                                                    //    row.Append(OpenXmlWordHelpers.CreateCell(string.Format("{0:#,0.00}", contrato.nm_parcelas_material * vl_parcela_liq_material)));
                                                }
                                                else
                                                    row.Append(OpenXmlWordHelpers.CreateCell(""));

                                                if ((bool)contrato.id_incorporar_valor_material && contrato.vl_material_contrato > 0)
                                                    row.Append(OpenXmlWordHelpers.CreateCell(string.Format("{0:#,0.00}", listaTitulos.Sum(t => t.vl_titulo) -
                                                     contrato.vl_material_contrato)));
                                                else
                                                    row.Append(OpenXmlWordHelpers.CreateCell(string.Format("{0:#,0.00}", listaTitulos.Sum(t => t.vl_titulo))));

                                                row.Append(OpenXmlWordHelpers.CreateCell(string.Format("{0:#,0.00}", listaTitulos.Sum(t => t.vl_titulo))));

                                                tableValoresParcelas.AppendChild(row);

                                                //Insere na tabela após a macro
                                                //substitui a macro por texto
                                                if (fieldsC != null && fieldsC.Count() > 0)
                                                {
                                                    wordDoc.GetMergeFields(fieldName).FirstOrDefault().GetParagraph().InsertAfterSelf(tableValoresParcelas);
                                                    wordDoc.GetMergeFields(fieldName).ReplaceWithText(" ");
                                                }
                                                else
                                                {
                                                    wordDoc.GetSimpleFields(fieldName).FirstOrDefault().GetParagraph().InsertAfterSelf(tableValoresParcelas);
                                                    wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(" ");
                                                }
                                            }
                                            else
                                                if (fieldsC != null && fieldsC.Count() > 0)
                                                {
                                                    wordDoc.GetMergeFields(fieldName).ReplaceWithText(" ");
                                                }
                                                else
                                                {
                                                    wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(" ");
                                                }
                                            #endregion
                                            break;
                                        case "OpcoesPagamento": //Tipopagto_carne
                                            macro = 44;
                                            string dc_tipo_financeiro = contrato.TipoFinanceiro.dc_tipo_financeiro;
                                            iText = String.IsNullOrEmpty(dc_tipo_financeiro) ? " " : dc_tipo_financeiro;
                                            if (fieldsC != null && fieldsC.Count() > 0)
                                                wordDoc.GetMergeFields(fieldName).ReplaceWithText(iText);
                                            else
                                                wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(iText);
                                            break;
                                        case "NroPrevisaoDias": //PrevisaoDias
                                            macro = 45;
                                            string nmPrevisaoInicial = " ";
                                            if (aditamentos != null && aditamentos.Count > 0)
                                                nmPrevisaoInicial = aditamentos.OrderBy(a => a.dt_aditamento).Last().nm_previsao_inicial + "";
                                            else
                                                nmPrevisaoInicial = contrato.nm_previsao_inicial.ToString();
                                            iText = String.IsNullOrEmpty(nmPrevisaoInicial) ? " " : nmPrevisaoInicial;
                                            if (fieldsC != null && fieldsC.Count() > 0)
                                                wordDoc.GetMergeFields(fieldName).ReplaceWithText(iText);
                                            else
                                                wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(iText);
                                            break;
                                        case "SexoF":
                                            macro = 46;
                                            string tipoSexoF = " ";
                                            if (contrato.Aluno.AlunoPessoaFisica.nm_sexo != null && contrato.Aluno.AlunoPessoaFisica.nm_sexo == (byte)PessoaSGF.Sexo.FEMININO)
                                                tipoSexoF = "X";
                                            iText = tipoSexoF;
                                            if (fieldsC != null && fieldsC.Count() > 0)
                                                wordDoc.GetMergeFields(fieldName).ReplaceWithText(tipoSexoF, true);
                                            else
                                                wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(tipoSexoF, true);
                                            break;
                                        case "SexoM":
                                            macro = 47;
                                            string tipoSexoM = " ";
                                            if (contrato.Aluno.AlunoPessoaFisica.nm_sexo != null && contrato.Aluno.AlunoPessoaFisica.nm_sexo == (byte)PessoaSGF.Sexo.MASCULINO)
                                                tipoSexoM = "X";
                                            iText = tipoSexoM;
                                            if (fieldsC != null && fieldsC.Count() > 0)
                                                wordDoc.GetMergeFields(fieldName).ReplaceWithText(iText, true);
                                            else
                                                wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(iText, true);
                                            break;
                                        case "DataFimTurma":
                                            macro = 48;
                                            iText = contrato.dt_fim_turma == null ? " " : String.Format("{0:dd/MM/yyyy}", contrato.dt_fim_turma);
                                            if (fieldsC != null && fieldsC.Count() > 0)
                                                wordDoc.GetMergeFields(fieldName).ReplaceWithText(iText);
                                            else
                                                wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(iText);
                                            break;
                                        case "DataInicioAdt":
                                            macro = 49;
                                            string DtaIniAditamento = "";
                                            if (aditamentos != null && aditamentos.Count > 0)
                                                DtaIniAditamento = Aditamento.getDescricaoDataInicioAdt(aditamentos.OrderBy(a => a.dt_aditamento).Last());
                                            iText = String.IsNullOrEmpty(DtaIniAditamento) ? " " : DtaIniAditamento;
                                            if (fieldsC != null && fieldsC.Count() > 0)
                                                wordDoc.GetMergeFields(fieldName).ReplaceWithText(iText);
                                            else
                                                wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(iText);
                                            break;
                                        case "NroParcelas":
                                            #region Nro Parcelas
                                            macro = 50;
                                            string nroParcelas = "";
                                            Aditamento aditamentoNroPac = new Aditamento();

                                            if (aditamentos != null && aditamentos.Count() > 0)
                                                aditamentoNroPac = aditamentos.OrderBy(a => a.dt_aditamento).Last();

                                            //Caso o contrato não seja de aditamento o valor informado será o número de parcelas do contrato.
                                            if (aditamentoNroPac!=null && aditamentoNroPac.id_tipo_aditamento == null || aditamentos.Count <= 0)
                                                nroParcelas = contrato.nm_parcelas_mensalidade + " ";
                                            else
                                            {
                                                //Se for de aditamento com o tipo diferente de “Adicionar parcelas”, será a quantidade de títulos em aberto.
                                                if (aditamentoNroPac != null && aditamentoNroPac.id_tipo_aditamento.HasValue && aditamentoNroPac.id_tipo_aditamento.Value != (byte)Aditamento.TipoAditamento.ADICIONAR_PARCELAS)
                                                {
                                                    titulosContratoContext = Titulo.revisarSaldoTituloBolsaContrato(titulosContratoContext.ToList(), false, 0, false);
                                                    List<Titulo> titulosAbertosContrato = titulosContratoContext.Where(x => x.id_status_titulo == (int)Titulo.StatusTitulo.ABERTO &&
                                                                                                                            statusCnabTitulo.Contains(x.id_status_cnab) &&
                                                                                                                            x.vl_titulo == x.vl_saldo_titulo &&
                                                                                                                            x.dc_tipo_titulo != "TM" && x.dc_tipo_titulo != "TA" &&
                                                                                                                            x.dc_tipo_titulo != "AD" && x.dc_tipo_titulo != "AA").ToList();
                                                    nroParcelas = titulosAbertosContrato.Count() + " ";
                                                }
                                                else //Caso seja aditamento com tipo “Adicionar Parcelas” o valor será o número de títulos informado no Aditivo.
                                                    nroParcelas = contrato.aditamentoMaxData.nm_titulos_aditamento + " ";
                                            }

                                            iText = nroParcelas + " ";
                                            if (fieldsC != null && fieldsC.Count() > 0)
                                                wordDoc.GetMergeFields(fieldName).ReplaceWithText(iText);
                                            else
                                                wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(iText);
                                            #endregion
                                            break;
                                        case "NroParcelasTotal":
                                            #region Nro ParcelasTotal
                                            macro = 51;
                                            //string nroParcelasT = "";
                                            aditamentoNroPac = new Aditamento();

                                            if (aditamentos != null && aditamentos.Count()>0)
                                                aditamentoNroPac = aditamentos.OrderBy(a => a.dt_aditamento).Last();

                                            //Caso o contrato não seja de aditamento o valor informado será o número de parcelas do contrato.
                                            if (aditamentoNroPac != null && aditamentoNroPac.id_tipo_aditamento == null || aditamentos.Count <= 0)
                                                nroParcelas = contrato.nm_parcelas_mensalidade + " ";
                                            else
                                            {
                                                nroParcelas = contrato.nm_parcelas_mensalidade + aditamentoNroPac.nm_titulos_aditamento + " ";
                                            }

                                            iText = nroParcelas + " ";
                                            if (fieldsC != null && fieldsC.Count() > 0)
                                                wordDoc.GetMergeFields(fieldName).ReplaceWithText(iText);
                                            else
                                                wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(iText);
                                            #endregion
                                            break;
                                        case "DataInicioAdtExtenso":
                                            macro = 52;
                                            string DtaIniAdtExtenso = "";
                                            if (aditamentos != null && aditamentos.Count > 0)
                                            {
                                                CultureInfo culture = new CultureInfo("pt-BR");
                                                DateTimeFormatInfo dtfi = culture.DateTimeFormat;
                                                DateTime dtaIni = DateTime.MinValue;
                                                if (aditamentos.OrderBy(a => a.dt_aditamento).Last().dt_inicio_aditamento != null)
                                                    dtaIni = (DateTime)aditamentos.OrderBy(a => a.dt_aditamento).Last().dt_inicio_aditamento;
                                                if (dtaIni == DateTime.MinValue)
                                                    DtaIniAdtExtenso = "";
                                                else
                                                    DtaIniAdtExtenso = dtaIni.Day + " de " + culture.TextInfo.ToTitleCase(dtfi.GetMonthName(dtaIni.Month)) + " de " + dtaIni.Year;
                                            }
                                            iText = String.IsNullOrEmpty(DtaIniAdtExtenso) ? " " : DtaIniAdtExtenso;
                                            if (fieldsC != null && fieldsC.Count() > 0)
                                                wordDoc.GetMergeFields(fieldName).ReplaceWithText(iText);
                                            else
                                                wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(iText);
                                            break;
                                        case "AnoCorrente":
                                            macro = 53;
                                            string anoCoorente = DateTime.Now.Year + "";
                                            iText = anoCoorente;
                                            if (fieldsC != null && fieldsC.Count() > 0)
                                                wordDoc.GetMergeFields(fieldName).ReplaceWithText(iText);
                                            else
                                                wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(iText);
                                            break;
                                        case "DataCorrenteExtenso":
                                            macro = 54;
                                            string DtaCorrenteExtenso = contrato.dtaCorrenteExtenso;
                                            iText = String.IsNullOrEmpty(DtaCorrenteExtenso) ? " " : DtaCorrenteExtenso;
                                            if (fieldsC != null && fieldsC.Count() > 0)
                                                wordDoc.GetMergeFields(fieldName).ReplaceWithText(iText);
                                            else
                                                wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(iText);
                                            break;
                                        case "TipoAditamento":
                                            macro = 55;
                                            var tipoAdt = "";
                                            if (aditamentos != null && aditamentos.Count > 0)
                                                tipoAdt = aditamentos.OrderBy(a => a.dt_aditamento).Last().dc_tipo_aditamento;
                                            iText = String.IsNullOrEmpty(tipoAdt) ? tipoAdt : tipoAdt;
                                            if (fieldsC != null && fieldsC.Count() > 0)
                                                wordDoc.GetMergeFields(fieldName).ReplaceWithText(iText);
                                            else
                                                wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(iText);
                                            break;
                                        case "VencimentosTitulosComDesc":
                                            #region Vencimentos Titulos Com Desc
                                            macro = 56;

                                            var descVencComTitulos = "";
                                            // Esse valor só irá aparecer caso haja algum desconto no contrato.
                                            if (contrato.vl_desconto_contrato > 0)
                                            {
                                                Aditamento aditamentoVencTitulos = new Aditamento();
                                                if (aditamentos != null && aditamentos.Count > 0)
                                                    aditamentoVencTitulos = aditamentos.OrderBy(a => a.dt_aditamento).Last();

                                                //Caso o contrato não seja de aditamento concatenar o vencimento de todos os títulos da matrícula, e separados por vírgula.
                                                if (aditamentoVencTitulos != null && aditamentoVencTitulos.id_tipo_aditamento == null || aditamentos.Count <= 0)
                                                    descVencComTitulos = Titulo.concatenarVencimentosTitulo(titulosContratoContext);
                                                else
                                                {
                                                    List<Titulo> titulosEmAbertoMatricula = new List<Titulo>();
                                                    titulosContratoContext = Titulo.revisarSaldoTituloBolsaContrato(titulosContratoContext, false, 0, false);

                                                    //Se for de aditamento com o tipo diferente de “Adicionar parcelas”, concatenar o vencimento de todos os títulos em aberto da matrícula, e separados por vírgula.
                                                    if (aditamentoVencTitulos != null && aditamentoVencTitulos.id_tipo_aditamento.HasValue &&
                                                        aditamentoVencTitulos.id_tipo_aditamento.Value != (byte)Aditamento.TipoAditamento.ADICIONAR_PARCELAS)
                                                        titulosEmAbertoMatricula = titulosContratoContext.Where(x => x.id_status_titulo == (int)Titulo.StatusTitulo.ABERTO &&
                                                                                                                     statusCnabTitulo.Contains(x.id_status_cnab) &&
                                                                                                                     x.vl_titulo == x.vl_saldo_titulo &&
                                                                                                                     x.dc_tipo_titulo != "TM" && x.dc_tipo_titulo != "TA" &&
                                                                                                                     x.dc_tipo_titulo != "AD" && x.dc_tipo_titulo != "AA").ToList();
                                                    else //Caso seja aditamento com tipo “Adicionar Parcelas”, concatenar o vencimento de todos os títulos do tipo do aditamento, abertos e separados por vírgula.
                                                        titulosEmAbertoMatricula = titulosContratoContext.Where(x => (x.dc_tipo_titulo == "AA" || x.dc_tipo_titulo == "AD") &&
                                                                                                                     x.id_status_titulo == (int)Titulo.StatusTitulo.ABERTO && statusCnabTitulo.Contains(x.id_status_cnab) &&
                                                                                                                     x.vl_titulo == x.vl_saldo_titulo).ToList();
                                                    descVencComTitulos = Titulo.concatenarVencimentosTitulo(titulosEmAbertoMatricula.ToList());
                                                }
                                            }
                                            iText = String.IsNullOrEmpty(descVencComTitulos) ? " " : descVencComTitulos;
                                            if (fieldsC != null && fieldsC.Count() > 0)
                                                wordDoc.GetMergeFields(fieldName).ReplaceWithText(iText);
                                            else
                                                wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(iText);
                                            #endregion
                                            break;
                                        case "VencimentosTitulosSemDesc":
                                            #region Vencimentos títulos sem desconto
                                            macro = 57;

                                            var descVencSemTitulos = "";
                                            // Esse valor só irá aparecer caso haja algum desconto no contrato.
                                            if (contrato.vl_desconto_contrato <= 0)
                                            {
                                                Aditamento aditamentoVencTitSemDesc = new Aditamento();

                                                if (aditamentos != null && aditamentos.Count()> 0)
                                                    aditamentoVencTitSemDesc = aditamentos.OrderBy(a => a.dt_aditamento).Last();

                                                //Caso o contrato não seja de aditamento concatenar o vencimento de todos os títulos da matrícula, e separados por vírgula.
                                                if (aditamentoVencTitSemDesc != null && aditamentoVencTitSemDesc.id_tipo_aditamento == null || aditamentos.Count <= 0)
                                                    descVencSemTitulos = Titulo.concatenarVencimentosTitulo(titulosContratoContext);
                                                else
                                                {
                                                    List<Titulo> titulosEmAbertoMatricula = new List<Titulo>();
                                                    titulosContratoContext = Titulo.revisarSaldoTituloBolsaContrato(titulosContratoContext, false, 0, false);

                                                    //Se for de aditamento com o tipo diferente de “Adicionar parcelas”, concatenar o vencimento de todos os títulos em aberto da matrícula, e separados por vírgula.
                                                    if (aditamentoVencTitSemDesc != null && aditamentoVencTitSemDesc.id_tipo_aditamento.HasValue &&
                                                        aditamentoVencTitSemDesc.id_tipo_aditamento.Value != (byte)Aditamento.TipoAditamento.ADICIONAR_PARCELAS)
                                                        titulosEmAbertoMatricula = titulosContratoContext.Where(x => x.id_status_titulo == (int)Titulo.StatusTitulo.ABERTO &&
                                                                                                                     statusCnabTitulo.Contains(x.id_status_cnab) &&
                                                                                                                     x.vl_titulo == x.vl_saldo_titulo &&
                                                                                                                     x.dc_tipo_titulo != "TM" && x.dc_tipo_titulo != "TA" &&
                                                                                                                     x.dc_tipo_titulo != "AD" && x.dc_tipo_titulo != "AA").ToList();
                                                    else //Caso seja aditamento com tipo “Adicionar Parcelas”, concatenar o vencimento de todos os títulos do tipo do aditamento, abertos e separados por vírgula.
                                                        titulosEmAbertoMatricula = titulosContratoContext.Where(x => (x.dc_tipo_titulo == "AA" || x.dc_tipo_titulo == "AD") &&
                                                                                                                     x.id_status_titulo == (int)Titulo.StatusTitulo.ABERTO && statusCnabTitulo.Contains(x.id_status_cnab) &&
                                                                                                                     x.vl_titulo == x.vl_saldo_titulo).ToList();
                                                    descVencSemTitulos = Titulo.concatenarVencimentosTitulo(titulosEmAbertoMatricula.ToList());
                                                }
                                            }
                                            iText = String.IsNullOrEmpty(descVencSemTitulos) ? " " : descVencSemTitulos;
                                            if (fieldsC != null && fieldsC.Count() > 0)
                                                wordDoc.GetMergeFields(fieldName).ReplaceWithText(iText);
                                            else
                                                wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(iText);
                                            #endregion
                                            break;
                                        case "DataFimContrato":
                                            macro = 58;
                                            string DtFinalContrato = contrato.dtFinalContrato;
                                            iText = String.IsNullOrEmpty(DtFinalContrato) ? " " : DtFinalContrato;
                                            if (fieldsC != null && fieldsC.Count() > 0)
                                                wordDoc.GetMergeFields(fieldName).ReplaceWithText(iText);
                                            else
                                                wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(iText);
                                            break;
                                        case "DataMatriculaContrato":
                                            macro = 59;
                                            string DtMatriculaContrato = contrato.dtMatriculaContrato;
                                            iText = String.IsNullOrEmpty(DtMatriculaContrato) ? " " : DtMatriculaContrato;
                                            if (fieldsC != null && fieldsC.Count() > 0)
                                                wordDoc.GetMergeFields(fieldName).ReplaceWithText(iText);
                                            else
                                                wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(iText);
                                            break;
                                        case "DataNascResponsavel":
                                            macro = 60;
                                            string DtNascResponsavel = contrato.dtNascResponsavel;
                                            iText = String.IsNullOrEmpty(DtNascResponsavel) ? " " : DtNascResponsavel;
                                            if (fieldsC != null && fieldsC.Count() > 0)
                                                wordDoc.GetMergeFields(fieldName).ReplaceWithText(iText);
                                            else
                                                wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(iText);
                                            break;
                                        case "NumeroContrato":
                                            macro = 61;
                                            int? nmContrato = contrato.nm_contrato;
                                            iText = nmContrato != null ? nmContrato + "" : "";
                                            if (fieldsC != null && fieldsC.Count() > 0)
                                                wordDoc.GetMergeFields(fieldName).ReplaceWithText(iText);
                                            else
                                                wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(iText);
                                            break;
                                        case "Observacao":
                                            macro = 62;
                                            string obsContrato = contrato.Aditamento.FirstOrDefault().tx_obs_aditamento;
                                            iText = string.IsNullOrEmpty(obsContrato) ? " " : obsContrato;
                                            if (fieldsC != null && fieldsC.Count() > 0)
                                                wordDoc.GetMergeFields(fieldName).ReplaceWithText(iText);
                                            else
                                                wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(iText);
                                            break;
                                        case "ParcelaLiquida":
                                            macro = 63;
                                            string vlParcelaLiquida = contrato.vlParcelaLiquida;
                                            iText = string.IsNullOrEmpty(vlParcelaLiquida) ? " " : vlParcelaLiquida;
                                            if (fieldsC != null && fieldsC.Count() > 0)
                                                wordDoc.GetMergeFields(fieldName).ReplaceWithText(iText);
                                            else
                                                wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(iText);
                                            break;
                                        case "TipoFinanceiroTaxa":
                                            macro = 64;
                                            string dc_tipo_financ_taxa = contrato.dc_tipo_financeiro_taxa;
                                            iText = string.IsNullOrEmpty(dc_tipo_financ_taxa) ? " " : dc_tipo_financ_taxa;
                                            if (fieldsC != null && fieldsC.Count() > 0)
                                                wordDoc.GetMergeFields(fieldName).ReplaceWithText(iText);
                                            else
                                                wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(iText);
                                            break;
                                        case "TipoMatricula":
                                            macro = 65;
                                            string dc_tipo_matricula = contrato.dc_tipo_matricula;
                                            iText = string.IsNullOrEmpty(dc_tipo_matricula) ? " " : dc_tipo_matricula;
                                            if (fieldsC != null && fieldsC.Count() > 0)
                                                wordDoc.GetMergeFields(fieldName).ReplaceWithText(iText);
                                            else
                                                wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(iText);
                                            break;
                                        case "Modalidade":
                                            macro = 66;
                                            string dc_regime_turma = contrato.dc_regime_turma;
                                            iText = string.IsNullOrEmpty(dc_regime_turma) ? " " : dc_regime_turma;
                                            if (fieldsC != null && fieldsC.Count() > 0)
                                                wordDoc.GetMergeFields(fieldName).ReplaceWithText(iText);
                                            else
                                                wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(iText);
                                            break;
                                        case "GradeValoresDescontosAntecipa":
                                            #region Grade Valores Descontos Antecipação
                                            macro = 67;

                                            var grupoDescontosAntecipacao = descontosPoliticaTitulos.GroupBy(x => new { x.cd_politica_desconto, x.nm_dia_limite_politica })
                                                .Select(x => new BaixaTitulo.DiasPoliticaAntecipacao
                                                {
                                                    cd_politica_desconto = x.Key.cd_politica_desconto,
                                                    nm_dia_limite_politica = x.Key.nm_dia_limite_politica
                                                }).ToList();
                                            //if (grupoDescontosAntecipacao != null && grupoDescontosAntecipacao.Count > 0)
                                            //    grupoDescontosAntecipacao = descontosPoliticaTitulos.Where(x => grupoDescontosAntecipacao.Select(c => c.cd_politica_desconto).Contains(x.cd_politica_desconto))
                                            //        .GroupBy(x
                                            if (grupoDescontosAntecipacao != null && grupoDescontosAntecipacao.Count > 0)
                                            {

                                                Table tableDescontosAntecipacao = new Table();
                                                //Instancia um Linha da tabela(OpenXml)
                                                TableGrid tableGridA = OpenXmlWordHelpers.CreateTableGrid(3);
                                                OpenXmlWordHelpers.SetTableStyle(tableDescontosAntecipacao);
                                                tableDescontosAntecipacao.AppendChild(tableGridA);

                                                TableRow row = null;
                                                row = new TableRow();


                                                //row.Append(OpenXmlWordHelpers.ConfigureWidth(OpenXmlWordHelpers.CreateCell(""), "20"));
                                                row.Append(OpenXmlWordHelpers.ConfigureWidth(OpenXmlWordHelpers.CreateCell("Dia Venc. Desconto por Antecipação"), "3000"));
                                                row.Append(OpenXmlWordHelpers.ConfigureWidth(OpenXmlWordHelpers.CreateCell("DIA"), "3000"));
                                                row.Append(OpenXmlWordHelpers.ConfigureWidth(OpenXmlWordHelpers.CreateCell("(%)"), "2000"));

                                                tableDescontosAntecipacao.Append(row);

                                                int i, row_aux_desc_ant;

                                                for (i = 0, row_aux_desc_ant = 2; i < grupoDescontosAntecipacao.Count; i++, row_aux_desc_ant++)
                                                {
                                                    row = new TableRow();
                                                    //row.Append(OpenXmlWordHelpers.ConfigureBorder(OpenXmlWordHelpers.CreateCell("")));

                                                    var _dataPolitica = descontosPoliticaTitulos.Where(x => x.cd_politica_desconto == grupoDescontosAntecipacao[i].cd_politica_desconto &&
                                                                                                                     x.nm_dia_limite_politica == grupoDescontosAntecipacao[i].nm_dia_limite_politica)
                                                                                                                     .FirstOrDefault().Data_politica;
                                                    DateTime _dataVencimentoMat = Contrato.gerarDataVencimentoMatricula(contrato.nm_dia_vcto, contrato.nm_mes_vcto, contrato.nm_ano_vcto);
                                                    if (_dataPolitica < _dataVencimentoMat)
                                                        _dataPolitica = _dataVencimentoMat;

                                                    row.Append(OpenXmlWordHelpers.CreateCell("A partir de " + _dataPolitica.ToString("dd/MM/yyyy")));
                                                    //tab.Cell(row, 2).Range.Text = "A partir de " + _dataPolitica.ToString("dd/MM/yyyy");
                                                    row.Append(OpenXmlWordHelpers.CreateCell(descontosPoliticaTitulos.Where(x => x.cd_politica_desconto == grupoDescontosAntecipacao[i].cd_politica_desconto &&
                                                                                                                                                                    x.nm_dia_limite_politica == grupoDescontosAntecipacao[i].nm_dia_limite_politica)
                                                                                                                                                                    .FirstOrDefault().nm_dia_limite_politica.ToString()));
                                                    //tab.Cell(row, 3).Range.Text = descontosPoliticaTitulos.Where(x => x.cd_politica_desconto == grupoDescontosAntecipacao[i].cd_politica_desconto &&
                                                    //                                                                 x.nm_dia_limite_politica == grupoDescontosAntecipacao[i].nm_dia_limite_politica)
                                                    //                                                                 .FirstOrDefault().nm_dia_limite_politica.ToString();
                                                    row.Append(OpenXmlWordHelpers.CreateCell(string.Format("{0,00}", descontosPoliticaTitulos.Where(x => x.cd_politica_desconto == grupoDescontosAntecipacao[i].cd_politica_desconto &&
                                                                                                                                                                                            x.nm_dia_limite_politica == grupoDescontosAntecipacao[i].nm_dia_limite_politica)
                                                                                                                                                                                 .FirstOrDefault().pc_pontualidade)));
                                                    tableDescontosAntecipacao.Append(row);
                                                }
                                                if (fieldsC != null && fieldsC.Count() > 0)
                                                {
                                                    wordDoc.GetMergeFields(fieldName).FirstOrDefault().GetParagraph().InsertAfterSelf(tableDescontosAntecipacao);
                                                    wordDoc.GetMergeFields(fieldName).ReplaceWithText(" ");
                                                }
                                                else
                                                {
                                                    wordDoc.GetSimpleFields(fieldName).FirstOrDefault().GetParagraph().InsertAfterSelf(tableDescontosAntecipacao);
                                                    wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(" ");
                                                }
                                            }
                                            else
                                            if (fieldsC != null && fieldsC.Count() > 0)
                                            {
                                                wordDoc.GetMergeFields(fieldName).ReplaceWithText("Não informado");
                                            }
                                            else
                                            {
                                                wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS("Não informado");
                                            }
                                            #endregion
                                            break;
                                        case "GradeDescontosContrato":
                                            macro = 68;
                                            List<DescontoContrato> listaDescontosContrato = BusinessMatricula.getDescontosAplicadosContratoOrAditamento(contrato.cd_contrato, cd_escola).ToList();

                                            #region  Grade Descontos Contrato

                                            if (listaDescontosContrato != null && listaDescontosContrato.Count > 0)
                                            {

                                                //wordDoc.GetMergeFields(fieldName).ReplaceWithText(" ");
                                                //wordApp.Selection.TypeText(" ");
                                                Table tableDescontosContrato = new Table();
                                                TableGrid tableGridD = OpenXmlWordHelpers.CreateTableGrid(4);
                                                OpenXmlWordHelpers.SetTableStyle(tableDescontosContrato);
                                                tableDescontosContrato.AppendChild(tableGridD);

                                                TableRow row = null;
                                                row = new TableRow();


                                                //row.Append(OpenXmlWordHelpers.ConfigureWidth(OpenXmlWordHelpers.CreateCell(""), "20"));
                                                row.Append(OpenXmlWordHelpers.ConfigureWidth(OpenXmlWordHelpers.CreateCell("Descrição do(s) Desconto(s)"), "3000"));
                                                row.Append(OpenXmlWordHelpers.ConfigureWidth(OpenXmlWordHelpers.CreateCell("Dia Venc. Tipos de Desconto"), "3000"));
                                                row.Append(OpenXmlWordHelpers.ConfigureWidth(OpenXmlWordHelpers.CreateCell("%/R$"), "2000"));
                                                row.Append(OpenXmlWordHelpers.ConfigureWidth(OpenXmlWordHelpers.CreateCell("Parcela(s)"), "135"));

                                                tableDescontosContrato.Append(row);

                                                int i, row_desc_cont;

                                                for (i = 0, row_desc_cont = 2; i < listaDescontosContrato.Count; i++, row_desc_cont++)
                                                {
                                                    row = new TableRow();
                                                    //row.Append(OpenXmlWordHelpers.ConfigureBorder(OpenXmlWordHelpers.CreateCell("")));
                                                    row.Append(OpenXmlWordHelpers.CreateCell(listaDescontosContrato[i].dc_tipo_desconto));
                                                    //tab.Cell(row, 2).Range.Text = listaDescontosContrato[i].dc_tipo_desconto;
                                                    row.Append(OpenXmlWordHelpers.CreateCell(contrato.nm_dia_vcto.ToString()));
                                                    //tab.Cell(row, 3).Range.Text = contrato.nm_dia_vcto.ToString();
                                                    row.Append(OpenXmlWordHelpers.CreateCell(string.Format("{0,00}", listaDescontosContrato[i].pc_desconto_contrato > 0 ?
                                                        listaDescontosContrato[i].pc_desconto_contrato + "%" :
                                                        "R$ " + listaDescontosContrato[i].vl_desconto_contrato)));
                                                    //tab.Cell(row, 4).Range.Text = string.Format("{0,00}", listaDescontosContrato[i].pc_desconto_contrato > 0 ?
                                                    //                                                       listaDescontosContrato[i].pc_desconto_contrato + "%" :
                                                    //                                                      "R$ " + listaDescontosContrato[i].vl_desconto_contrato);
                                                    row.Append(OpenXmlWordHelpers.CreateCell(DescontoContrato.gerarDescDescontosContrato(titulosAbertos, listaDescontosContrato[i].nm_parcela_ini,
                                                                                                                                listaDescontosContrato[i].nm_parcela_fim)));
                                                    //tab.Cell(row, 5).Range.Text = DescontoContrato.gerarDescDescontosContrato(titulosAbertos, listaDescontosContrato[i].nm_parcela_ini,
                                                    //                                                                          listaDescontosContrato[i].nm_parcela_fim);
                                                    tableDescontosContrato.Append(row);
                                                }
                                                if (fieldsC != null && fieldsC.Count() > 0)
                                                {
                                                    wordDoc.GetMergeFields(fieldName).FirstOrDefault().GetParagraph().InsertAfterSelf(tableDescontosContrato);
                                                    wordDoc.GetMergeFields(fieldName).ReplaceWithText(" ");
                                                }
                                                else
                                                {
                                                    wordDoc.GetSimpleFields(fieldName).FirstOrDefault().GetParagraph().InsertAfterSelf(tableDescontosContrato);
                                                    wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(" ");
                                                }
                                            }
                                            else
                                            if (fieldsC != null && fieldsC.Count() > 0)
                                            {
                                                wordDoc.GetMergeFields(fieldName).ReplaceWithText("Não informado");
                                            }
                                            else
                                            {
                                                wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS("Não informado");
                                            }
                                            #endregion
                                            break;
                                        case "GradeValoresLiquidos":
                                            #region  Valores Liquidos
                                            macro = 69;

                                            //wordDoc.GetMergeFields(fieldName).ReplaceWithText(" ");
                                            //wordApp.Selection.TypeText(" ");
                                            var grupoDescontosAntecipacaoVL = descontosPoliticaTitulos.OrderBy(x => x.Data_politica)
                                                .GroupBy(x => new { x.cd_politica_desconto })
                                                .Select(x => new BaixaTitulo.DiasPoliticaAntecipacao
                                                {
                                                    cd_politica_desconto = x.Key.cd_politica_desconto,
                                                    qtd_politica = x.Where(g => g.cd_politica_desconto == x.Key.cd_politica_desconto)
                                                        .GroupBy(gx => new { gx.cd_politica_desconto, gx.nm_dia_limite_politica }).Count()
                                                }).ToList();

                                            var qtdTitulosAbertosPolitica = titulosAbertos.Where(x => x.BaixaTitulo.Any(b => b.vl_desconto_baixa > 0)).OrderBy(x => x.cd_titulo).OrderBy(z => z.cd_titulo).ToList();

                                            Table tableValoresLiquidos = new Table();
                                            TableGrid tableGridL = OpenXmlWordHelpers.CreateTableGrid(4);
                                            OpenXmlWordHelpers.SetTableStyle(tableValoresLiquidos);
                                            tableValoresLiquidos.AppendChild(tableGridL);

                                            TableRow rowValoresLiquidos = null;
                                            if (grupoDescontosAntecipacaoVL != null && grupoDescontosAntecipacaoVL.Count > 0)
                                            {
                                                //var tab = wordApp.Selection.Tables.Add(rngFieldCode, titulosAbertos.Where(x => x.BaixaTitulo.Any(b => b.diasPoliticaAntecipacao != null &&
                                                //                                                                                                      b.diasPoliticaAntecipacao.Any())).Count() +
                                                //                                                     (grupoDescontosAntecipacaoVL.Count * 3), qtd_maior_dias_politica + 4);

                                                int i = 0, row = 3; //, iRowColumn = 2;
                                                foreach (var group in grupoDescontosAntecipacaoVL)
                                                {
                                                    var groupDias = descontosPoliticaTitulos.Where(x => x.cd_politica_desconto == group.cd_politica_desconto).OrderBy(x => x.Data_politica)
                                                    .GroupBy(x => new { x.cd_politica_desconto, x.nm_dia_limite_politica })
                                                    .Select(x => new BaixaTitulo.DiasPoliticaAntecipacao
                                                    {
                                                        cd_politica_desconto = x.Key.cd_politica_desconto,
                                                        nm_dia_limite_politica = x.Key.nm_dia_limite_politica
                                                    }).ToList();
                                                    var titulosAbertosPolitica = titulosAbertos.Where(x => x.BaixaTitulo.Any(b => b.diasPoliticaAntecipacao != null &&
                                                                                                                                  b.diasPoliticaAntecipacao.Any(dp =>
                                                                                                                                      dp.cd_politica_desconto == group.cd_politica_desconto))).OrderBy(x => x.cd_titulo).ToList();


                                                    var _dataPolitica = descontosPoliticaTitulos.Where(x => x.cd_politica_desconto == group.cd_politica_desconto)
                                                        .FirstOrDefault().Data_politica;
                                                    DateTime _dataVencimentoMat = Contrato.gerarDataVencimentoMatricula(contrato.nm_dia_vcto, contrato.nm_mes_vcto, contrato.nm_ano_vcto);
                                                    if (_dataPolitica < _dataVencimentoMat)
                                                        _dataPolitica = _dataVencimentoMat;

                                                    rowValoresLiquidos = new TableRow();
                                                    rowValoresLiquidos.Append(OpenXmlWordHelpers.ConfigureWidth(OpenXmlWordHelpers.CreateCell("A partir de " + _dataPolitica.ToString("dd/MM/yyyy")), "3000"));
                                                    //tab.Cell(iRowColumn - 1, 2).Range.Text = "A partir de " + _dataPolitica.ToString("dd/MM/yyyy");
                                                    rowValoresLiquidos.Append(OpenXmlWordHelpers.ConfigureWidth(OpenXmlWordHelpers.CreateCell("Parcela(s) do(s) Desconto(s)"), "3000"));
                                                    //tab.Cell(iRowColumn, 2).Range.Text = "Parcela(s) do(s) Desconto(s)";
                                                    rowValoresLiquidos.Append(OpenXmlWordHelpers.ConfigureWidth(OpenXmlWordHelpers.CreateCell("MATERIAL (R$)"), "1500"));
                                                    //tab.Cell(iRowColumn, 3).Range.Text = "MATERIAL (R$)";

                                                    foreach (var diasPolitica in groupDias)
                                                    {

                                                        rowValoresLiquidos.Append(OpenXmlWordHelpers.ConfigureWidth(OpenXmlWordHelpers.CreateCell("Valor da(s) Parcela(s) (R$) até dia " + diasPolitica.nm_dia_limite_politica), "3000"));

                                                    }

                                                    byte datap = groupDias.OrderBy(x => x.nm_dia_limite_politica).Last().nm_dia_limite_politica;
                                                    rowValoresLiquidos.Append(OpenXmlWordHelpers.ConfigureWidth(OpenXmlWordHelpers.CreateCell("Após o dia " + datap.ToString()), "1500"));
                                                    //tab.Cell(iRowColumn, iClm + 1).Range.Text = "Até o dia " + contrato.nm_dia_vcto;
                                                    tableValoresLiquidos.Append(rowValoresLiquidos);

                                                    int columnDias = 3;
                                                    List<BaixaTitulo.DiasPoliticaAntecipacao> listaTotais = new List<BaixaTitulo.DiasPoliticaAntecipacao>();
                                                    for (i = 0; i < titulosAbertosPolitica.Count(); i++, row++)
                                                    {
                                                        rowValoresLiquidos = new TableRow();
                                                        rowValoresLiquidos.Append(OpenXmlWordHelpers.CreateCell(""));
                                                        rowValoresLiquidos.Append(OpenXmlWordHelpers.CreateCell(titulosAbertosPolitica[i].nm_parcela_titulo + "º"));
                                                        if (titulosAbertosPolitica[i].vl_material_titulo > 0)
                                                        {
                                                            rowValoresLiquidos.Append(OpenXmlWordHelpers.CreateCell(string.Format("{0:#,0.00}", titulosAbertosPolitica[i].vl_material_titulo)));
                                                            //tab.Cell(row, 3).Range.Text = string.Format("{0:#,0.00}", titulosAbertosPolitica[i].vl_material_titulo);
                                                            //tab.Cell(row, 3).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                                        }
                                                        else
                                                        {
                                                            rowValoresLiquidos.Append(OpenXmlWordHelpers.CreateCell(""));
                                                        }
                                                        //tab.Cell(row, 2).Range.Text = titulosAbertosPolitica[i].nm_parcela_titulo + "º";
                                                        //tab.Cell(row, 2).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                                        foreach (var diasG in groupDias)
                                                        {
                                                            columnDias++;
                                                            var pc_desconto = (descontosPoliticaTitulos.Where(x => x.cd_politica_desconto == diasG.cd_politica_desconto &&
                                                                                                                   x.nm_dia_limite_politica == diasG.nm_dia_limite_politica &&
                                                                                                                   x.cd_titulo == titulosAbertosPolitica[i].cd_titulo).FirstOrDefault() != null
                                                                ? (descontosPoliticaTitulos.Where(x => x.cd_politica_desconto == diasG.cd_politica_desconto &&
                                                                                                       x.nm_dia_limite_politica == diasG.nm_dia_limite_politica &&
                                                                                                       x.cd_titulo == titulosAbertosPolitica[i].cd_titulo).FirstOrDefault().pc_pontualidade_total)
                                                                : 0);
                                                            var vl_titulo_calc = titulosAbertosPolitica[i].vl_saldo_titulo -
                                                                                 ((titulosAbertosPolitica[i].vl_saldo_titulo - titulosAbertosPolitica[i].vl_material_titulo) / 100 * pc_desconto);
                                                            rowValoresLiquidos.Append(OpenXmlWordHelpers.CreateCell(string.Format("{0:#,0.00}", vl_titulo_calc)));

                                                            listaTotais.Add(new BaixaTitulo.DiasPoliticaAntecipacao
                                                            {
                                                                nm_coluna = columnDias,
                                                                cd_titulo = titulosAbertosPolitica[i].cd_titulo,
                                                                vl_titulo = vl_titulo_calc,
                                                                nm_dia_limite_politica = diasG.nm_dia_limite_politica
                                                            });
                                                        }
                                                        Decimal desconto_baixa = descontosPoliticaTitulos.Where(x => x.cd_politica_desconto == group.cd_politica_desconto &&
                                                                                                                     x.cd_titulo == titulosAbertosPolitica[i].cd_titulo).FirstOrDefault().pc_desconto_baixa;
                                                        var vl_titulo_ate_vencimento = titulosAbertosPolitica[i].vl_saldo_titulo -
                                                                                       ((titulosAbertosPolitica[i].vl_saldo_titulo - titulosAbertosPolitica[i].vl_material_titulo) / 100 * desconto_baixa);

                                                        rowValoresLiquidos.Append(OpenXmlWordHelpers.CreateCell(string.Format("{0:#,0.00}", vl_titulo_ate_vencimento)));
                                                        //tab.Cell(row, columnDias + 1).Range.Text = string.Format("{0:#,0.00}", vl_titulo_ate_vencimento);
                                                        //tab.Cell(row, columnDias + 1).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                                        listaTotais.Add(new BaixaTitulo.DiasPoliticaAntecipacao { nm_coluna = columnDias + 1, cd_titulo = titulosAbertosPolitica[i].cd_titulo, vl_titulo = vl_titulo_ate_vencimento, nm_dia_limite_politica = 0 });
                                                        columnDias = 3;
                                                        //tab.Cell(row, 1).Borders.Enable = 0;
                                                        tableValoresLiquidos.Append(rowValoresLiquidos);
                                                    }
                                                    rowValoresLiquidos = new TableRow();
                                                    rowValoresLiquidos.Append(OpenXmlWordHelpers.CreateCell(""));
                                                    rowValoresLiquidos.Append(OpenXmlWordHelpers.CreateCell("Total:"));
                                                    //tab.Cell(row, 2).Range.Text = "Total:";
                                                    //tab.Cell(row, 2).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                                    if (contrato.vl_material_contrato > 0)
                                                    {
                                                        if ((bool)contrato.id_valor_incluso && contrato.vl_material_contrato > 0)
                                                            rowValoresLiquidos.Append(OpenXmlWordHelpers.CreateCell(string.Format("{0:#,0.00}", contrato.vl_material_contrato)));
                                                        else
                                                            rowValoresLiquidos.Append(OpenXmlWordHelpers.CreateCell(string.Format("{0:#,0.00}", contrato.nm_parcelas_material *
                                                                contrato.vl_parcela_liq_material)));
                                                    }
                                                    else
                                                        rowValoresLiquidos.Append(OpenXmlWordHelpers.CreateCell(""));

                                                    if (listaTotais != null && listaTotais.Count() > 0)
                                                    {
                                                        var groupTotaisTitulo = listaTotais.GroupBy(x => new { x.cd_politica_desconto, x.nm_dia_limite_politica, x.nm_coluna })
                                                            .Select(x => new BaixaTitulo.DiasPoliticaAntecipacao
                                                            {
                                                                cd_politica_desconto = x.Key.cd_politica_desconto,
                                                                nm_dia_limite_politica = x.Key.nm_dia_limite_politica,
                                                                nm_coluna = x.Key.nm_coluna
                                                            }).ToList();
                                                        foreach (var g in groupTotaisTitulo)
                                                        {
                                                            rowValoresLiquidos.Append(OpenXmlWordHelpers.CreateCell(string.Format("{0:#,0.00}", listaTotais.Where(x => x.nm_dia_limite_politica == g.nm_dia_limite_politica).Sum(x => x.vl_titulo))));
                                                            //tab.Cell(row, g.nm_coluna).Range.Text = string.Format("{0:#,0.00}", listaTotais.Where(x => x.nm_dia_limite_politica == g.nm_dia_limite_politica).Sum(x => x.vl_titulo));
                                                            //tab.Cell(row, g.nm_coluna).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                                        }
                                                    }
                                                    tableValoresLiquidos.Append(rowValoresLiquidos);

                                                }
                                                if (fieldsC != null && fieldsC.Count() > 0)
                                                {
                                                    wordDoc.GetMergeFields(fieldName).FirstOrDefault().GetParagraph().InsertAfterSelf(tableValoresLiquidos);
                                                    wordDoc.GetMergeFields(fieldName).ReplaceWithText(" ");
                                                }
                                                else
                                                {
                                                    wordDoc.GetSimpleFields(fieldName).FirstOrDefault().GetParagraph().InsertAfterSelf(tableValoresLiquidos);
                                                    wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(" ");
                                                }
                                            }
                                            else if (grupoDescontosAntecipacaoVL.Count <= 0 &&
                                                baixasSimuladas != null && baixasSimuladas.Count > 0 &&
                                                qtdTitulosAbertosPolitica != null && qtdTitulosAbertosPolitica.Count > 0)
                                            {

                                                //Table tableValoresLiquidos = new Table();

                                                //TableRow rowValoresLiquidos = null;

                                                int i = 0, row = 2; //, iRowColumn = 1;

                                                var groupDias = baixasSimuladas.ToList();

                                                var titulosAbertosPolitica = titulosAbertos.Where(x => x.BaixaTitulo.Any(b => b.vl_desconto_baixa > 0)).OrderBy(x => x.cd_titulo).OrderBy(z => z.cd_titulo).ToList();


                                                //int iClm = 3;

                                                DateTime _dataVencimentoMat = Contrato.gerarDataVencimentoMatricula(contrato.nm_dia_vcto, contrato.nm_mes_vcto, contrato.nm_ano_vcto);

                                                rowValoresLiquidos = new TableRow();
                                                rowValoresLiquidos.Append(OpenXmlWordHelpers.ConfigureWidth(OpenXmlWordHelpers.CreateCell("Parcela(s) do(s) Desconto(s)"), "3000"));
                                                rowValoresLiquidos.Append(OpenXmlWordHelpers.ConfigureWidth(OpenXmlWordHelpers.CreateCell("MATERIAL (R$)"), "1500"));
                                                rowValoresLiquidos.Append(OpenXmlWordHelpers.ConfigureWidth(OpenXmlWordHelpers.CreateCell("Até o dia " + contrato.nm_dia_vcto), "1500"));
                                                tableValoresLiquidos.Append(rowValoresLiquidos);


                                                List<decimal> listaTotais = new List<decimal>();
                                                List<decimal> listaValoresMaterialTitulo = new List<decimal>();
                                                for (i = 0; i < titulosAbertosPolitica.Count(); i++, row++)
                                                {
                                                    rowValoresLiquidos = new TableRow();
                                                    rowValoresLiquidos.Append(OpenXmlWordHelpers.CreateCell(titulosAbertosPolitica[i].nm_parcela_titulo + "º"));
                                                    //tab.Cell(row, 2).Range.Text = titulosAbertosPolitica[i].nm_parcela_titulo + "º";
                                                    //tab.Cell(row, 2).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;


                                                    if (titulosAbertosPolitica[i].vl_material_titulo > 0)
                                                    {
                                                        rowValoresLiquidos.Append(OpenXmlWordHelpers.CreateCell(string.Format("{0:#,0.00}", titulosAbertosPolitica[i].vl_material_titulo)));
                                                        //tab.Cell(row, 3).Range.Text = string.Format("{0:#,0.00}", titulosAbertosPolitica[i].vl_material_titulo);
                                                        //tab.Cell(row, 3).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                                        listaValoresMaterialTitulo.Add(titulosAbertosPolitica[i].vl_material_titulo);
                                                    }
                                                    else
                                                        rowValoresLiquidos.Append(OpenXmlWordHelpers.CreateCell(""));
                                                    BaixaTitulo baixa_simulada_aux = baixasSimuladas.Where(b => b.cd_titulo == titulosAbertosPolitica[i].cd_titulo).FirstOrDefault();

                                                    var vl_titulo_ate_vencimento = baixa_simulada_aux.vl_liquidacao_baixa - titulosAbertosPolitica[i].vl_material_titulo;
                                                    rowValoresLiquidos.Append(OpenXmlWordHelpers.CreateCell(string.Format("{0:#,0.00}", vl_titulo_ate_vencimento)));
                                                    //tab.Cell(row, columnDias + 1).Range.Text = string.Format("{0:#,0.00}", vl_titulo_ate_vencimento);
                                                    //tab.Cell(row, columnDias + 1).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                                    listaTotais.Add(vl_titulo_ate_vencimento);
                                                    //columnDias = 3;
                                                    //tab.Cell(row, 1).Borders.Enable = 0;

                                                    tableValoresLiquidos.Append(rowValoresLiquidos);
                                                }

                                                rowValoresLiquidos = new TableRow();

                                                rowValoresLiquidos.Append(OpenXmlWordHelpers.CreateCell("Total:"));
                                                //tab.Cell(row, 2).Range.Text = "Total:";
                                                //tab.Cell(row, 2).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;

                                                if (listaValoresMaterialTitulo.Count > 0)
                                                {
                                                    rowValoresLiquidos.Append(OpenXmlWordHelpers.CreateCell(string.Format("{0:#,0.00}", listaValoresMaterialTitulo.Sum())));
                                                    //tab.Cell(row, 3).Range.Text = string.Format("{0:#,0.00}", listaValoresMaterialTitulo.Sum());
                                                }
                                                else
                                                {
                                                    rowValoresLiquidos.Append(OpenXmlWordHelpers.CreateCell("0,00"));
                                                    //tab.Cell(row, 3).Range.Text = "0,00";
                                                }


                                                //tab.Cell(row, 3).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                                //Colunas Totalizadores 

                                                if (listaTotais != null && listaTotais.Count() > 0)
                                                {
                                                    rowValoresLiquidos.Append(OpenXmlWordHelpers.CreateCell(string.Format("{0:#,0.00}", listaTotais.Sum())));
                                                    //tab.Cell(row, 4).Range.Text = string.Format("{0:#,0.00}", listaTotais.Sum());
                                                    //tab.Cell(row, 4).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                                }

                                                tableValoresLiquidos.Append(rowValoresLiquidos);
                                                if (fieldsC != null && fieldsC.Count() > 0)
                                                {
                                                    wordDoc.GetMergeFields(fieldName).FirstOrDefault().GetParagraph().InsertAfterSelf(tableValoresLiquidos);
                                                    wordDoc.GetMergeFields(fieldName).ReplaceWithText(" ");
                                                }
                                                else
                                                {
                                                    wordDoc.GetSimpleFields(fieldName).FirstOrDefault().GetParagraph().InsertAfterSelf(tableValoresLiquidos);
                                                    wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(" ");
                                                }
                                            }
                                            else
                                            if (fieldsC != null && fieldsC.Count() > 0)
                                            {
                                                wordDoc.GetMergeFields(fieldName).ReplaceWithText(" Não informado");
                                            }
                                            else
                                            {
                                                wordDoc.GetSimpleFields(fieldName).ReplaceWithTextS(" Não informado");
                                            }
                                            #endregion
                                            break;
                                        default:
                                            throw new SecretariaBusinessException(string.Format(Utils.Messages.Messages.msgParametroNaoEsperado, "&lt;" + fieldName + "&gt;"), null, SecretariaBusinessException.TipoErro.ERRO_PARAMETRO_NAO_ECONTRADO, false);
                                    }
                                }

                            }
                            //Salva as modificações em memoria
                            wordDoc.MainDocumentPart.Document.Save();
                            houveErro = false;

                        }

                        //Salva o arquivo em memoria na pasta tempContratos com o formato .docx
                        File.WriteAllBytes(Path.Combine(caminho_relatorio, "TempContratos") + "/" + nome_temp_guid + ".docx", templateStream.ToArray());
                    }

                    //iTextSharp(Converter para pdf-> lê o xml do arquivo .docx e converte para pdf)
                    //Obs: arquivo sem tabela está convertendo(tem que mexer para coverter as tabelas)
                    //DocxToPdf.DocxToPdf convertor = new DocxToPdf.DocxToPdf((Path.Combine(caminho_relatorio, "TempContratos") + "/" + nome_temp_guid + ".docx"), (Path.Combine(caminho_relatorio, "TempContratos") + "/" + nome_temp_guid + ".docx").Replace(".docx", ".pdf"));

                    //le o arquivo .pdf salvo e retorna para o usuario(download)
                    byte[] data = File.ReadAllBytes(Path.Combine(caminho_relatorio, "TempContratos") + "/" + nome_temp_guid + ".docx");
                    MemoryStream stream = new MemoryStream(data);
                    deletarArquivoTempContrato(Path.Combine(caminho_relatorio, "TempContratos") + "/" + nome_temp_guid);// + ".pdf");

                    return stream;
                }
                //catch (Exception e)
                //{
                //    Console.WriteLine(e);
                //    throw;
                //}
                finally
                {
                    // depois que finaliza deleta todos os arquivos temporarios criados
                    //deletarArquivoTempContrato(Path.Combine(caminho_relatorio, "TempContratos") + "/" + nome_temp_guid  + ".docx");
                    deletarArquivoTempContrato(Path.Combine(caminho_relatorio, "TempContratos") + "/" + nome_temp_guid + ".dotx");
                    deletarArquivoTempContrato(Path.Combine(caminho_relatorio, "TempContratos") + "/" + nome_temp_guid);// + ".pdf");
                    if (houveErro)
                        throw new CoordenacaoBusinessException(Messages.msgErroImprimirContrato + " - " + macro.ToString(), null, CoordenacaoBusinessException.TipoErro.ERRO_IMPRIMIR_CONTRATO, false);
                }
            }
            else
                throw new SecretariaBusinessException(Messages.msgRegNotEnc, null, SecretariaBusinessException.TipoErro.ERRO_PARAMETRO_NAO_ECONTRADO, false);
        }

        public MemoryStream postImprimirContrato(int cd_escola, int cd_contrato, bool conta_segura, string caminho_relatorios, string file_name, ref int? nm_cliente_integracao, ref int? nm_contrato, ref string no_aluno)
        {
            string nome_contrato = ""; // "crwContrato_0001 - CONTRATO ALUNO ESTAGIO - NORMAL";
            Object oTemplatePath = ""; // caminho_relatorios + "/Contratos/" + cd_escola + "/" + nome_contrato + ".dotx";  
            Object oMissing = System.Reflection.Missing.Value;
            Object doNotSaveChanges = Microsoft.Office.Interop.Word.WdSaveOptions.wdDoNotSaveChanges;
            var pathContrato = "";
            var caminho_temp_contrato = "";
            NomeContrato noCont = BusinessSecretaria.getNomeContratoAditamentoMatricula(cd_contrato, cd_escola);

            if (noCont != null && noCont.cd_nome_contrato > 0)
            {
                nome_contrato = noCont.no_relatorio;
                if (noCont.cd_pessoa_escola == null)
                    pathContrato = caminho_relatorios + "/Contratos/" + noCont.no_relatorio;
                else
                    pathContrato = caminho_relatorios + "/Contratos/" + noCont.cd_pessoa_escola + "/" + noCont.no_relatorio;                
            }
            else
                throw new MatriculaBusinessException(Utils.Messages.Messages.msgNotExistLayoutNoCont, null, MatriculaBusinessException.TipoErro.ERRO_NAO_EXISTE_LAYOUT_NOME_CONTRATO, false);

            //Pesquisa os dados do contrato em banco:
            Contrato contrato = BusinessMatricula.getContratoImpressao(cd_escola, cd_contrato);
            Parametro parametro = BusinessEscola.getParametrosBaixa(cd_escola);
            List<Titulo> titulosAbertos = BusinessFinanceiro.getTituloBaixaFinanSimulacao(new List<int>(), cd_escola, cd_contrato, TituloDataAccess.TipoConsultaTituloEnum.HAS_TITULOS_ABERTO_SIMULACAO);
            List<BaixaTitulo> baixasSimuladas = new List<BaixaTitulo>();
            List<BaixaTitulo.DiasPoliticaAntecipacao> descontosPoliticaTitulos = new List<BaixaTitulo.DiasPoliticaAntecipacao>();
            foreach (Titulo t in titulosAbertos)
            {
                BaixaTitulo baixa = new BaixaTitulo();
                baixa.dt_baixa_titulo = DateTime.Now.Date;
                BusinessCoordenacao.simularBaixaTitulo(t, ref baixa, parametro, cd_escola, conta_segura, true);
                t.BaixaTitulo.Add(baixa);
                if (baixa.diasPoliticaAntecipacao != null)
                {
                    descontosPoliticaTitulos.AddRange(baixa.diasPoliticaAntecipacao);
                    baixasSimuladas.Add(baixa);
                }
            }
            if (contrato != null)
            {
                nm_cliente_integracao = contrato.Escola.nm_cliente_integracao;
                nm_contrato = contrato.nm_contrato;
                no_aluno = contrato.Aluno.AlunoPessoaFisica.no_pessoa;
                List<Titulo> titulosContratoContext = BusinessFinanceiro.getTitulosByContratoLeitura(contrato.cd_contrato, cd_escola).ToList();
                int[] statusCnabTitulo = new int[] { (int)Titulo.StatusCnabTitulo.INICIAL, (int)Titulo.StatusCnabTitulo.CONFIRMADO_PEDIDO_BAIXA };

                caminho_temp_contrato = criarArquivoTempContrato(pathContrato, caminho_relatorios);
                oTemplatePath = caminho_temp_contrato;
                Word.Application wordApp = null;
                Microsoft.Office.Interop.Word.Document wordDoc = null;

                try
                {
                    // instância objetos e processo WORD.                    
                    wordApp = new Word.Application();
                    wordDoc = new Microsoft.Office.Interop.Word.Document();
                    //---------------------

                    wordDoc = wordApp.Documents.Add(ref oTemplatePath, ref oMissing, ref oMissing, ref oMissing);
                    List<Aditamento> aditamentos = new List<Aditamento>();
                    if (contrato.Aditamento != null)
                        aditamentos = contrato.Aditamento.ToList();
                    foreach (Word.Field myMergeField in wordDoc.Fields)
                    {
                        Word.Range rngFieldCode = myMergeField.Code;
                        String fieldText = rngFieldCode.Text;
                        Word.Table tab = null;

                        // ONLY GETTING THE MAILMERGE FIELDS
                        if (fieldText.StartsWith(" MERGEFIELD"))
                        {
                            // THE TEXT COMES IN THE FORMAT OF
                            // MERGEFIELD  MyFieldName  \\* MERGEFORMAT
                            // THIS HAS TO BE EDITED TO GET ONLY THE FIELDNAME "MyFieldName"
                            Int32 endMerge = fieldText.IndexOf("\\");
                            Int32 fieldNameLength = fieldText.Length - endMerge;
                            String fieldName = fieldText.Substring(11, endMerge - 11);

                            // GIVES THE FIELDNAMES AS THE USER HAD ENTERED IN .dot FILE
                            fieldName = fieldName.Trim();
                            // **** FIELD REPLACEMENT IMPLEMENTATION GOES HERE ****//
                            // THE PROGRAMMER CAN HAVE HIS OWN IMPLEMENTATIONS HERE
                            myMergeField.Select();

                            switch (fieldName)
                            {
                                case "NomeEscola":
                                    string dc_reduzido_pessoa = contrato.Escola.dc_reduzido_pessoa;
                                    wordApp.Selection.TypeText(String.IsNullOrEmpty(dc_reduzido_pessoa) ? " " : dc_reduzido_pessoa);

                                    break;
                                case "RazaoSocial": //Nome_escola
                                    string no_pessoa = contrato.Escola.no_pessoa;
                                    wordApp.Selection.TypeText(String.IsNullOrEmpty(no_pessoa) ? " " : no_pessoa);
                                    break;
                                case "CNPJEscola": //Cnpj_escola
                                    string dc_num_cgc = contrato.Escola.dc_num_cgc;
                                    wordApp.Selection.TypeText(String.IsNullOrEmpty(dc_num_cgc) ? " " : dc_num_cgc);
                                    break;
                                case "EnderecoEscola": //End_Escola
                                    string no_localidade = contrato.Escola.EnderecoPrincipal.Logradouro.no_localidade;
                                    if (!String.IsNullOrEmpty(no_localidade))
                                    {
                                        if (!String.IsNullOrEmpty(contrato.Escola.EnderecoPrincipal.dc_num_endereco))
                                            no_localidade += " Nº " + contrato.Escola.EnderecoPrincipal.dc_num_endereco;
                                        if (contrato.Escola.EnderecoPrincipal.Logradouro != null && contrato.Escola.EnderecoPrincipal.TipoLogradouro != null)
                                            no_localidade = contrato.Escola.EnderecoPrincipal.TipoLogradouro.no_tipo_logradouro + " " + no_localidade;
                                    }
                                    else
                                        no_localidade = " ";
                                    wordApp.Selection.TypeText(no_localidade);
                                    break;
                                case "CidadeEstadoEscola": //cidade_estado
                                    no_localidade = contrato.Escola.EnderecoPrincipal.Cidade.no_localidade;
                                    if (!String.IsNullOrEmpty(no_localidade))
                                    {
                                        if (!String.IsNullOrEmpty(contrato.Escola.EnderecoPrincipal.Estado.no_localidade))
                                            no_localidade += " - " + contrato.Escola.EnderecoPrincipal.Estado.no_localidade;
                                    }
                                    else
                                        no_localidade = " ";
                                    wordApp.Selection.TypeText(no_localidade);
                                    break;

                                case "NomeResponsavel": //Nom_resp
                                    no_pessoa = contrato.PessoaResponsavel.no_pessoa;
                                    wordApp.Selection.TypeText(String.IsNullOrEmpty(no_pessoa) ? " " : no_pessoa);
                                    break;
                                case "RGResponsavel": //Rg_resp
                                    string nm_doc_identidade = contrato.PessoaResponsavel.nm_doc_identidade;
                                    wordApp.Selection.TypeText(String.IsNullOrEmpty(nm_doc_identidade) ? " " : nm_doc_identidade);
                                    break;
                                case "CPFCNPJResponsavel": //Cpf_resp
                                    string nm_cpf_cgc = contrato.PessoaResponsavel.nm_cpf_cgc;
                                    wordApp.Selection.TypeText(String.IsNullOrEmpty(nm_cpf_cgc) ? " " : nm_cpf_cgc);
                                    break;

                                case "TituloRGResponsavel": //
                                    wordApp.Selection.TypeText(contrato.PessoaResponsavel.nm_natureza_pessoa == (int)PessoaSGF.TipoPessoa.FISICA ? "RG" : "");
                                    break;
                                case "TituloCPFouCNPJResponsavel": //
                                    wordApp.Selection.TypeText(contrato.PessoaResponsavel.nm_natureza_pessoa == (int)PessoaSGF.TipoPessoa.FISICA ? "CPF" : "CNPJ");
                                    break;
                                case "TelefoneResponsavel": //tel_resp
                                    string dc_fone_mail = contrato.PessoaResponsavel.Telefone.dc_fone_mail;
                                    wordApp.Selection.TypeText(String.IsNullOrEmpty(dc_fone_mail) ? " " : dc_fone_mail);
                                    break;
                                case "EnderecoResponsavel": //End_resp
                                    string enderecoCompleto = contrato.PessoaResponsavel.EnderecoPrincipal.enderecoCompleto;
                                    wordApp.Selection.TypeText(String.IsNullOrEmpty(enderecoCompleto) ? " " : enderecoCompleto);
                                    break;
                                case "EmailResponsavel": //Email_Responsavel
                                    string emailR = "";
                                    emailR = contrato.pessoaFisicaResponsavel.email;
                                    wordApp.Selection.TypeText(String.IsNullOrEmpty(emailR) ? " " : emailR);
                                    break;
                                case "CelularResponsavel": //TelCel
                                    string celularR = "";
                                    celularR = contrato.pessoaFisicaResponsavel.celular;
                                    wordApp.Selection.TypeText(String.IsNullOrEmpty(celularR) ? " " : celularR);
                                    break;

                                case "NomeAluno": //Nome_aluno
                                    string nomeAluno = contrato.Aluno.AlunoPessoaFisica.no_pessoa;
                                    wordApp.Selection.TypeText(String.IsNullOrEmpty(nomeAluno) ? " " : nomeAluno);
                                    break;

                                case "TelelfoneAluno": //Tel_aluno - ( {qyRptContrato_001;1.num_tel_aluno} + ( if (isnull({qyRptContrato_001;1.num_telefone_contato_aluno}) or ({qyRptContrato_001;1.num_telefone_contato_aluno} = '')) then ''    else ( ' - ' + totext({qyRptContrato_001;1.num_telefone_contato_aluno}) +                    (  if (isnull({qyRptContrato_001;1.num_ramal1_aluno}) or ({qyRptContrato_001;1.num_ramal1_aluno} = '')) then ''                        else ('  ' + {qyRptContrato_001;1.num_ramal1_aluno}) ) ) ))
                                    dc_fone_mail = contrato.Aluno.AlunoPessoaFisica.Telefone.dc_fone_mail;
                                    wordApp.Selection.TypeText(String.IsNullOrEmpty(dc_fone_mail) ? " " : dc_fone_mail);
                                    break;

                                case "RGAluno": //Rg_aluno
                                    nm_doc_identidade = contrato.Aluno.AlunoPessoaFisica.nm_doc_identidade;
                                    wordApp.Selection.TypeText(String.IsNullOrEmpty(nm_doc_identidade) ? " " : nm_doc_identidade);
                                    break;

                                case "CPFAluno": //Cpf_aluno
                                    string nm_cpf = contrato.Aluno.AlunoPessoaFisica.nm_cpf;
                                    wordApp.Selection.TypeText(String.IsNullOrEmpty(nm_cpf) ? " " : nm_cpf);
                                    break;
                                case "EstadoCivilAluno": //Est_civil_aluno
                                    string estadoCivil = contrato.Aluno.AlunoPessoaFisica.nm_sexo == (byte)PessoaSGF.Sexo.FEMININO ? contrato.Aluno.AlunoPessoaFisica.EstadoCivil.dc_estado_civil_fem : contrato.Aluno.AlunoPessoaFisica.EstadoCivil.dc_estado_civil_masc;
                                    wordApp.Selection.TypeText(String.IsNullOrEmpty(estadoCivil) ? " " : estadoCivil);
                                    break;
                                case "DataNascimentoAluno": //Dta_nasc
                                    string dtaNascimento = contrato.Aluno.AlunoPessoaFisica.dtaNascimento;
                                    wordApp.Selection.TypeText(String.IsNullOrEmpty(dtaNascimento) ? " " : dtaNascimento);
                                    break;
                                case "EnderecoAluno": //End_aluno
                                    enderecoCompleto = contrato.Aluno.AlunoPessoaFisica.EnderecoPrincipal.enderecoCompleto;
                                    wordApp.Selection.TypeText(String.IsNullOrEmpty(enderecoCompleto) ? " " : enderecoCompleto);
                                    break;
                                case "EmailAluno": //Email_aluno
                                    string email = "";
                                    email = contrato.Aluno.AlunoPessoaFisica.email;
                                    wordApp.Selection.TypeText(String.IsNullOrEmpty(email) ? " " : email);
                                    break;
                                case "CelularAluno": //TelCel
                                    string celular = "";
                                    celular = contrato.Aluno.AlunoPessoaFisica.celular;
                                    wordApp.Selection.TypeText(String.IsNullOrEmpty(celular) ? " " : celular);
                                    break;

                                case "TituloCurso": //curso_label - ( if ({qyRptContrato_001;1.nom_idioma} in ['Inglês', 'Espanhol']) then 'ESTÁGIO: ' else  'MÓDULO: ' )
                                    List<string> listaProdutos = new List<string>();
                                    listaProdutos.Add("Inglês");
                                    listaProdutos.Add("Espanhol");
                                    wordApp.Selection.TypeText(listaProdutos.Contains(contrato.Produto.no_produto) ? "Estágio" : "Módulo");
                                    break;
                                case "Curso": //curso
                                    string no_curso = contrato.Curso.no_curso;
                                    wordApp.Selection.TypeText(String.IsNullOrEmpty(no_curso) ? " " : no_curso);
                                    break;
                                case "ComplementoCursoComMinutosTurma": //Turma60Minutos - if ({qyRptContrato_001;1.ind_P60} = 1) then 'TURMA DE 60 MINUTOS' else if ({qyRptContrato_001;1.ind_R60} = 1) then 'TURMA DE 60 MINUTOS' else if ({qyRptContrato_001;1.qtd_minutos_div} > 0) then ('TURMA DE ' + ToText({qyRptContrato_001;1.qtd_minutos_div}, 0) + ' MINUTOS') else ''
                                    string complemento = " ";
                                    no_curso = contrato.Curso.no_curso;

                                    if (no_curso.Contains("R60") || no_curso.Contains("R60"))
                                        complemento += "TURMA DE 60 MINUTOS";
                                    else if (contrato.qtd_minutos_turma > 0)
                                        complemento += "TURMA DE " + contrato.qtd_minutos_turma + " MINUTOS";

                                    wordApp.Selection.TypeText(complemento);
                                    break;

                                case "DiasHorariosCurso": //HorariosTurmaProc.rpt
                                    string dias_horarios = "";

                                    if (contrato.hashtableHorarios != null)
                                        dias_horarios = Horario.getDescricaoCompletaHorarios(contrato.hashtableHorarios);

                                    wordApp.Selection.TypeText(String.IsNullOrEmpty(dias_horarios) ? " " : dias_horarios);

                                    break;
                                case "Dias": //HorariosTurmaProc.rpt
                                    #region Dias
                                    dias_horarios = "";
                                    if (contrato.hashtableHorarios != null)
                                    {
                                        List<Horario> listaHorariosOrdenados = new List<Horario>();
                                        foreach (DictionaryEntry entry in contrato.hashtableHorarios)
                                            listaHorariosOrdenados.Add(new Horario { desc_concat_dias_semena = entry.Value.ToString() });

                                        listaHorariosOrdenados = listaHorariosOrdenados.OrderBy(h => h.desc_concat_dias_semena).ThenBy(h => h.no_registro).ToList();
                                        foreach (Horario entry in listaHorariosOrdenados)
                                        {
                                            string[] array = entry.desc_concat_dias_semena.Split(',');
                                            for (int i = 0; i < array.Length; i++)
                                                array[i] = Horario.getDiaSemanaPorDia(array[i]);
                                            entry.desc_concat_dias_semena = string.Join(", ", array);
                                        }

                                        List<string> listaStrHorariosOrdenados = (from h in listaHorariosOrdenados orderby h.id_dia_semana ascending select h.desc_concat_dias_semena).Distinct().ToList();
                                        foreach (string s in listaStrHorariosOrdenados)
                                            dias_horarios += "; " + s;

                                        if (dias_horarios.Length >= 2)
                                            dias_horarios = dias_horarios.Substring(2, dias_horarios.Length - 2);
                                    }
                                    wordApp.Selection.TypeText(String.IsNullOrEmpty(dias_horarios) ? " " : dias_horarios);
                                    #endregion
                                    break;
                                case "HorariosCurso":
                                    dias_horarios = "";
                                    if (contrato.hashtableHorarios != null)
                                        dias_horarios = Horario.getDescricaoSimplificadaHorarios(contrato.hashtableHorarios);

                                    wordApp.Selection.TypeText(String.IsNullOrEmpty(dias_horarios) ? " " : dias_horarios);
                                    break;
                                case "DataInicioAulas": //Ini_Aula
                                    string dtInicialContrato = contrato.dtInicialContrato;
                                    wordApp.Selection.TypeText(String.IsNullOrEmpty(dtInicialContrato) ? " " : dtInicialContrato);
                                    break;
                                case "MatriculaRematricula": //Mat_Remat
                                    string vlMatriculaContrato = contrato.vlMatriculaContrato;
                                    wordApp.Selection.TypeText(String.IsNullOrEmpty(vlMatriculaContrato) ? " " : vlMatriculaContrato);
                                    break;
                                case "ValorSemDesconto":
                                    decimal vlMaterialMatricula = 0;
                                    decimal vlSemDesconto = contrato.vl_curso_contrato / contrato.nm_parcelas_mensalidade;
                                    byte nm_parcelas_material = 0;

                                    if (contrato.nm_parcelas_material > 0)
                                    {
                                        nm_parcelas_material = (byte)contrato.nm_parcelas_material;
                                        vlMaterialMatricula = (decimal)contrato.vl_material_contrato;
                                        if (nm_parcelas_material > 0) //Evitar divisão por zero
                                            wordApp.Selection.TypeText(string.Format("{0:#,0.00}", decimal.Round(vlSemDesconto + vlMaterialMatricula / nm_parcelas_material, 2)));
                                        else
                                            wordApp.Selection.TypeText(string.Format("{0:#,0.00}", decimal.Round(vlSemDesconto, 2)));
                                    }
                                    else
                                        wordApp.Selection.TypeText(string.Format("{0:#,0.00}", decimal.Round(vlSemDesconto, 2)));
                                    break;
                                case "NroVencimento": //dta_venc
                                    wordApp.Selection.TypeText(contrato.nm_dia_vcto + " ");
                                    break;
                                case "NroVencimentoComDesconto": //dta_venc_desc - des_vencto_com_desconto
                                    byte? dtaVctoAditamento = 0;
                                    if (aditamentos.Count > 0)
                                        dtaVctoAditamento = aditamentos.OrderBy(a => a.dt_aditamento).Last().nm_dia_vcto_desconto;
                                    wordApp.Selection.TypeText(!dtaVctoAditamento.HasValue ? " " : dtaVctoAditamento + "");
                                    break;

                                case "ValorComDesconto": //val_com_desc
                                    #region Valor Com Desconto
                                    nm_parcelas_material = 0;
                                    string valor_com_desconto = " ";
                                    vlMaterialMatricula = 0;
                                    bool aplicarMaterialBaixa = true;
                                    BaixaTitulo baixa = new BaixaTitulo();
                                    if (contrato.vl_parcela_contrato > 0)
                                    {
                                        //Caso quando não é aditamento
                                        Aditamento aditamento = new Aditamento();
                                        if (aditamentos != null)
                                            aditamentos.OrderBy(a => a.dt_aditamento).Last();

                                        //Caso o contrato não seja de aditamento o valor informado será o número de parcelas do contrato.
                                        if (aditamento.id_tipo_aditamento == null || aditamentos.Count <= 0)
                                        { BusinessEscola.simularBaixaContrato(contrato, ref baixa, cd_escola); }
                                        else
                                        {
                                            //Se for de aditamento com o tipo diferente de “Adicionar parcelas”, será a quantidade de títulos em aberto.
                                            if (aditamento != null && aditamento.id_tipo_aditamento.HasValue && aditamento.id_tipo_aditamento.Value != (byte)Aditamento.TipoAditamento.ADICIONAR_PARCELAS)
                                            {
                                                titulosContratoContext = Titulo.revisarSaldoTituloBolsaContrato(titulosContratoContext.ToList(), false, 0,false);
                                                List<Titulo> titulosAbertosContrato = titulosContratoContext.Where(x => x.id_status_titulo == (int)Titulo.StatusTitulo.ABERTO &&
                                                                                                                    statusCnabTitulo.Contains(x.id_status_cnab) &&
                                                                                                                    x.vl_titulo == x.vl_saldo_titulo &&
                                                                                                                    x.dc_tipo_titulo != "TM" && x.dc_tipo_titulo != "TA" &&
                                                                                                                    x.dc_tipo_titulo != "AD" && x.dc_tipo_titulo != "AA").ToList();
                                                BusinessCoordenacao.simularBaixaTitulo(titulosAbertosContrato.OrderBy(x => x.nm_parcela_titulo).FirstOrDefault(), ref baixa,
                                                    parametro, cd_escola, false, false);
                                            }
                                            else //Caso seja aditamento com tipo “Adicionar Parcelas” o valor será o número de títulos informado no Aditivo.
                                            {
                                                List<Titulo> titulosAbertosContrato = titulosContratoContext.Where(x => x.id_status_titulo == (int)Titulo.StatusTitulo.ABERTO &&
                                                                                                                    statusCnabTitulo.Contains(x.id_status_cnab) &&
                                                                                                                    x.vl_titulo == x.vl_saldo_titulo &&
                                                                                                                    x.dc_tipo_titulo == "AD" && x.dc_tipo_titulo == "AA").ToList();
                                                titulosAbertosContrato = titulosAbertosContrato.OrderBy(x => x.nm_parcela_titulo).ToList();
                                                if (titulosAbertosContrato != null && titulosAbertosContrato.Count() > 0)
                                                {
                                                    aplicarMaterialBaixa = false;
                                                    BusinessCoordenacao.simularBaixaTitulo(titulosAbertosContrato.FirstOrDefault(), ref baixa,
                                                    parametro, cd_escola, false, false);
                                                }
                                            }
                                        }
                                        if (aplicarMaterialBaixa)
                                        {
                                            if (contrato.vl_material_contrato > 0)
                                            {
                                                nm_parcelas_material = (byte)contrato.nm_parcelas_material;
                                                vlMaterialMatricula = (decimal)contrato.vl_material_contrato;
                                            }
                                            if (baixa != null && nm_parcelas_material > 0)
                                                valor_com_desconto = string.Format("{0:#,0.00}", decimal.Round(baixa.vl_liquidacao_baixa + vlMaterialMatricula / nm_parcelas_material, 2));
                                            else
                                                valor_com_desconto = string.Format("{0:#,0.00}", decimal.Round(baixa.vl_liquidacao_baixa, 2));
                                        }
                                    }
                                    wordApp.Selection.TypeText(valor_com_desconto + " ");
                                    #endregion
                                    break;
                                case "NroParcelasMaterial":
                                    nm_parcelas_material = (byte)contrato.nm_parcelas_material;

                                    wordApp.Selection.TypeText(nm_parcelas_material + "");
                                    break;
                                case "ValorMaterial":
                                    vlMaterialMatricula = (decimal)contrato.vl_material_contrato;
                                    wordApp.Selection.TypeText(string.Format("{0:#,0.00}", decimal.Round(vlMaterialMatricula, 2)));
                                    break;
                                case "ValorComDescontoMaterial":
                                        decimal vl_parcela_liq_material = (decimal)contrato.vl_parcela_liq_material;
                                    wordApp.Selection.TypeText(string.Format("{0:#,0.00}", decimal.Round(vl_parcela_liq_material, 2)));
                                    break;
                                case "ValorCurso":
                                    wordApp.Selection.TypeText(string.Format("{0:#,0.00}", contrato.vl_curso_contrato));
                                    break;
                                case "NroParcelasCurso":
                                    wordApp.Selection.TypeText(contrato.nm_parcelas_mensalidade + "");
                                    break;
                                case "BolsaMaterial":
                                    decimal pc_bolsa_material = 0;
                                        pc_bolsa_material = (decimal)contrato.pc_bolsa_material;
                                    wordApp.Selection.TypeText(string.Format("{0:#,0.00}", decimal.Round(pc_bolsa_material, 2)));
                                    break;
                                case "GradeCursos":
                                    #region CursosContrato
//                                    wordApp.ActiveDocument.Content.ParagraphFormat.SpaceAfter = 0;
                                    int li = 0;
                                    object missing = System.Reflection.Missing.Value;
                                    List<CursoContrato> listaCursoContrato = BusinessMatricula.getCursoContrato(contrato.cd_contrato);
                                    if (listaCursoContrato != null && listaCursoContrato.Count > 0)
                                    {
                                        //wordApp.Selection.TypeText(" ");
                                        if (li == 0)
                                            tab = wordApp.ActiveDocument.Content.Tables.Add(rngFieldCode, 4, 1); 
                                        tab.Rows.Height = 20;
                                        tab.Range.Font.Size = 8;
                                        tab.Range.Font.Name = "Arial Narrow";

                                        tab.Cell(1, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                                        tab.Cell(2, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                                        tab.Cell(3, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                                        tab.Cell(4, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;

                                        tab.Cell(1, 1).VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalTop;
                                        tab.Cell(2, 1).VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalTop;
                                        tab.Cell(3, 1).VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalTop;
                                        tab.Cell(4, 1).VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalTop;

                                        tab.Cell(2, 1).Split(1, 3);
                                        tab.Cell(3, 1).Split(1, 2);
                                        tab.Cell(4, 1).Split(1, 3);
                                        string ccomplemento = " ";
                                        string no_cursoc = listaCursoContrato[li].no_curso;

                                        if (no_cursoc.Contains("R60"))
                                            ccomplemento += "TURMA DE 60 MINUTOS";
                                        else if (contrato.qtd_minutos_turma > 0)
                                            ccomplemento += "TURMA DE " + contrato.qtd_minutos_turma + " MINUTOS";
//                                        string dc_regime_turmac = contrato.dc_regime_turma;
                                        string dc_regime_turmac = string.IsNullOrEmpty(contrato.dc_regime_turma) ? " " : contrato.dc_regime_turma;
                                        string dcmes = MesExtenso((int)listaCursoContrato[li].nm_mes_curso_inicial);
                                        tab.Cell(1, 1).Range.Text = "CURSO: " + no_cursoc + "  " + ccomplemento;
                                        tab.Cell(2, 1).Range.Text = "MODALIDADE: " + dc_regime_turmac;
                                        tab.Cell(2, 2).Range.Text = "INICIO: " + MesExtenso((int)listaCursoContrato[li].nm_mes_curso_inicial) + listaCursoContrato[li].nm_ano_curso_inicial;
                                        tab.Cell(2, 3).Range.Text = "TÉRMINO: " + MesExtenso((int)listaCursoContrato[li].nm_mes_curso_final) + listaCursoContrato[li].nm_ano_curso_final;
                                        tab.Cell(3, 1).Range.Text = "PARCELAS: " + listaCursoContrato[li].nm_parcelas_mensalidade;
                                        tab.Cell(3, 2).Range.Text = "TIPO FINANCEIRO: " + listaCursoContrato[li].TipoFinanceiro.dc_tipo_financeiro;
                                        tab.Cell(4, 1).Range.Text = "MATERIAL: " + listaCursoContrato[li].vl_material_contrato;
                                        tab.Cell(4, 2).Range.Text = "BOLSA: " + (listaCursoContrato[li].pc_bolsa_material > 0 ? listaCursoContrato[li].pc_bolsa_material + "%" : "") + "PARCELA LÍQUIDA: " + listaCursoContrato[li].vl_parcela_liquida;
                                        tab.Cell(4, 3).Range.Text = "TOTAL: " + listaCursoContrato[li].vl_curso_contrato;

                                        tab.Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleDot;//     .wdLineStyleSingle;
                                        tab.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
                                        tab.Borders.OutsideLineWidth = Word.WdLineWidth.wdLineWidth025pt;
                                        //tab.Cell(1, 2).Borders.Enable = 1;

                                        tab.PreferredWidthType = Word.WdPreferredWidthType.wdPreferredWidthPoints;
                                        tab.PreferredWidth = 530;

                                        tab.AllowPageBreaks = true;
                                        tab.AllowAutoFit = false;
                                        Word.Paragraph para2 = wordApp.ActiveDocument.Content.Paragraphs.Add(ref missing);
                                        para2.Range.Text = " ";
                                        para2.Range.InsertParagraphAfter();
                                        if (li < listaCursoContrato.Count - 1)
                                            tab = wordApp.ActiveDocument.Content.Tables.Add(para2.Range, 4, 1);
                                        li++;
                                    }
                                    else
                                        wordApp.Selection.TypeText(" ");
                                    #endregion
                                    break;
                                case "GradeValoresParcelas":
                                    List<Titulo> listaTitulos = BusinessFinanceiro.getTitulosContrato(contrato.cd_contrato);
                                    #region  ValoresParcelas
                                    if (listaTitulos != null && listaTitulos.Count > 0)
                                    {
                                        wordApp.Selection.TypeText(" ");
                                        tab = wordApp.Selection.Tables.Add(rngFieldCode, listaTitulos.Count + 2, 6);
                                        tab.Columns[1].Width = 20;
                                        tab.Columns[2].Width = 105;
                                        tab.Columns[3].Width = 105;
                                        tab.Columns[4].Width = 105;
                                        tab.Columns[5].Width = 105;

                                        tab.Cell(1, 2).Range.Text = "VENCIMENTO";
                                        tab.Cell(1, 3).Range.Text = "DIA";
                                        tab.Cell(1, 4).Range.Text = "MATERIAL (R$)";
                                        tab.Cell(1, 5).Range.Text = "PARCELA (R$)";
                                        tab.Cell(1, 6).Range.Text = "TOTAL (R$)";

                                        tab.Cell(1, 2).Range.Bold = 1;
                                        tab.Cell(1, 3).Range.Bold = 1;
                                        tab.Cell(1, 4).Range.Bold = 1;
                                        tab.Cell(1, 5).Range.Bold = 1;
                                        tab.Cell(1, 6).Range.Bold = 1;

                                        tab.Cell(1, 2).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                        tab.Cell(1, 3).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                        tab.Cell(1, 4).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                        tab.Cell(1, 5).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                        tab.Cell(1, 6).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;

                                        tab.Cell(1, 2).VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalTop;
                                        tab.Cell(1, 3).VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalTop;
                                        tab.Cell(1, 4).VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalTop;
                                        tab.Cell(1, 5).VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalTop;
                                        tab.Cell(1, 6).VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalTop;

                                        tab.Borders.InsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleSingle;
                                        tab.Borders.OutsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleSingle;

                                        tab.Cell(1, 1).Borders.Enable = 0;
                                        tab.Cell(1, 2).Borders.Enable = 1;

                                        tab.PreferredWidthType = Word.WdPreferredWidthType.wdPreferredWidthPoints;
                                        tab.PreferredWidth = 440;

                                        //tab.TopPadding = 4;
                                        //tab.BottomPadding = 4;
                                        //tab.LeftPadding = 50;
                                        //tab.RightPadding = 50;
                                        //tab.Spacing = 2;
                                        tab.AllowPageBreaks = true;
                                        tab.AllowAutoFit = false;

                                        nm_parcelas_material = 0;
                                        vl_parcela_liq_material = 0;
                                        int i, row;

                                        if (contrato.vl_material_contrato > 0)
                                        {
                                            nm_parcelas_material = (byte)contrato.nm_parcelas_material;
                                            vl_parcela_liq_material = (decimal)contrato.vl_parcela_liq_material;
                                        }

                                        for (i = 0, row = 2; i < listaTitulos.Count; i++, row++)
                                        {
                                            tab.Cell(row, 2).Range.Text = listaTitulos[i].nm_parcela_titulo + "º";
                                            tab.Cell(row, 3).Range.Text = listaTitulos[i].dt_vcto_titulo.ToString("dd/MM/yyyy");
                                            if (nm_parcelas_material > 0)
                                            {
                                                tab.Cell(row, 4).Range.Text = string.Format("{0:#,0.00}", vl_parcela_liq_material);
                                                tab.Cell(row, 5).Range.Text = string.Format("{0:#,0.00}", listaTitulos[i].vl_titulo - vl_parcela_liq_material);
                                                tab.Cell(row, 6).Range.Text = string.Format("{0:#,0.00}", listaTitulos[i].vl_saldo_titulo);
                                                nm_parcelas_material -= 1;
                                            }
                                            else
                                            {
                                                tab.Cell(row, 4).Range.Text = "";
                                                tab.Cell(row, 5).Range.Text = string.Format("{0:#,0.00}", listaTitulos[i].vl_titulo);
                                                tab.Cell(row, 6).Range.Text = string.Format("{0:#,0.00}", listaTitulos[i].vl_saldo_titulo);
                                            }
                                            tab.Cell(row, 1).Borders.Enable = 0;
                                            tab.Cell(row, 2).Borders.Enable = 1;

                                            tab.Cell(row, 2).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                            tab.Cell(row, 3).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                            tab.Cell(row, 4).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                            tab.Cell(row, 5).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                            tab.Cell(row, 6).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                        }
                                        tab.Cell(row, 2).Range.Text = "";
                                        tab.Cell(row, 3).Range.Text = "Total:";
                                        if (contrato.vl_material_contrato > 0)
                                        {
                                            if ((bool)contrato.id_valor_incluso && contrato.vl_material_contrato > 0)
                                                tab.Cell(row, 4).Range.Text = string.Format("{0:#,0.00}", contrato.vl_material_contrato);
                                            else
                                                tab.Cell(row, 4).Range.Text = string.Format("{0:#,0.00}", contrato.nm_parcelas_material *
                                                    contrato.vl_parcela_liq_material);
                                        }
                                        else
                                            tab.Cell(row, 4).Range.Text = "";

                                        if (contrato.vl_material_contrato > 0)
                                            tab.Cell(row, 5).Range.Text = string.Format("{0:#,0.00}", listaTitulos.Sum(t => t.vl_titulo) -
                                                contrato.nm_parcelas_material * contrato.vl_parcela_liq_material);
                                        else
                                            tab.Cell(row, 5).Range.Text = string.Format("{0:#,0.00}", listaTitulos.Sum(t => t.vl_titulo));

                                        tab.Cell(row, 6).Range.Text = string.Format("{0:#,0.00}", listaTitulos.Sum(t => t.vl_saldo_titulo));

                                        tab.Cell(row, 1).Borders.Enable = 0;
                                        tab.Cell(row, 2).Borders.Enable = 1;

                                        tab.Cell(row, 2).Range.Bold = 1;
                                        tab.Cell(row, 3).Range.Bold = 1;
                                        tab.Cell(row, 4).Range.Bold = 1;
                                        tab.Cell(row, 5).Range.Bold = 1;
                                        tab.Cell(row, 6).Range.Bold = 1;

                                        tab.Cell(row, 2).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                        tab.Cell(row, 3).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                        tab.Cell(row, 4).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                        tab.Cell(row, 5).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                        tab.Cell(row, 6).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                    }
                                    else
                                        wordApp.Selection.TypeText(" ");
                                    #endregion
                                    break;
                                case "OpcoesPagamento": //Tipopagto_carne
                                    string dc_tipo_financeiro = contrato.TipoFinanceiro.dc_tipo_financeiro;
                                    wordApp.Selection.TypeText(String.IsNullOrEmpty(dc_tipo_financeiro) ? " " : dc_tipo_financeiro);

                                    break;
                                case "NroPrevisaoDias": //PrevisaoDias
                                    string nmPrevisaoInicial = " ";
                                    if (aditamentos.Count > 0)
                                        nmPrevisaoInicial = aditamentos.OrderBy(a => a.dt_aditamento).Last().nm_previsao_inicial + "";

                                    wordApp.Selection.TypeText(String.IsNullOrEmpty(nmPrevisaoInicial) ? " " : nmPrevisaoInicial);
                                    break;
                                case "SexoF":
                                    string tipoSexoF = " ";
                                    if (contrato.Aluno.AlunoPessoaFisica.nm_sexo != null && contrato.Aluno.AlunoPessoaFisica.nm_sexo == (byte)PessoaSGF.Sexo.FEMININO)
                                        tipoSexoF = "X";
                                    wordApp.Selection.TypeText(tipoSexoF);
                                    break;
                                case "SexoM":
                                    string tipoSexoM = " ";
                                    if (contrato.Aluno.AlunoPessoaFisica.nm_sexo != null && contrato.Aluno.AlunoPessoaFisica.nm_sexo == (byte)PessoaSGF.Sexo.MASCULINO)
                                        tipoSexoM = "X";

                                    wordApp.Selection.TypeText(tipoSexoM);
                                    break;
                                case "DataFimTurma":
                                    wordApp.Selection.TypeText(contrato.dt_fim_turma == null ? " " : String.Format("{0:dd/MM/yyyy}", contrato.dt_fim_turma));
                                    break;
                                case "DataInicioAdt":
                                    string DtaIniAditamento = "";
                                    if (aditamentos.Count > 0)
                                        DtaIniAditamento = Aditamento.getDescricaoDataInicioAdt(aditamentos.OrderBy(a => a.dt_aditamento).Last());
                                    wordApp.Selection.TypeText(String.IsNullOrEmpty(DtaIniAditamento) ? " " : DtaIniAditamento);
                                    break;
                                case "NroParcelas":
                                    #region Nro Parcelas
                                    string nroParcelas = "";
                                    Aditamento aditamentoNroPac = new Aditamento();

                                    if (aditamentos != null)
                                        aditamentos.OrderBy(a => a.dt_aditamento).Last();

                                    //Caso o contrato não seja de aditamento o valor informado será o número de parcelas do contrato.
                                    if (aditamentoNroPac.id_tipo_aditamento == null || aditamentos.Count <= 0)
                                        nroParcelas = contrato.nm_parcelas_mensalidade + " ";
                                    else
                                    {
                                        //Se for de aditamento com o tipo diferente de “Adicionar parcelas”, será a quantidade de títulos em aberto.
                                        if (aditamentoNroPac != null && aditamentoNroPac.id_tipo_aditamento.HasValue && aditamentoNroPac.id_tipo_aditamento.Value != (byte)Aditamento.TipoAditamento.ADICIONAR_PARCELAS)
                                        {
                                            titulosContratoContext = Titulo.revisarSaldoTituloBolsaContrato(titulosContratoContext.ToList(), false, 0,false);
                                            List<Titulo> titulosAbertosContrato = titulosContratoContext.Where(x => x.id_status_titulo == (int)Titulo.StatusTitulo.ABERTO &&
                                                                                                                statusCnabTitulo.Contains(x.id_status_cnab) &&
                                                                                                                x.vl_titulo == x.vl_saldo_titulo &&
                                                                                                                x.dc_tipo_titulo != "TM" && x.dc_tipo_titulo != "TA" &&
                                                                                                                x.dc_tipo_titulo != "AD" && x.dc_tipo_titulo != "AA").ToList();
                                            nroParcelas = titulosAbertosContrato.Count() + " ";
                                        }
                                        else //Caso seja aditamento com tipo “Adicionar Parcelas” o valor será o número de títulos informado no Aditivo.
                                            nroParcelas = contrato.aditamentoMaxData.nm_titulos_aditamento + " ";
                                    }
                                    wordApp.Selection.TypeText(nroParcelas + " ");
                                    #endregion
                                    break;
                                case "DataInicioAdtExtenso":
                                    string DtaIniAdtExtenso = "";
                                    if (aditamentos.Count > 0)
                                    {
                                        CultureInfo culture = new CultureInfo("pt-BR");
                                        DateTimeFormatInfo dtfi = culture.DateTimeFormat;
                                        DateTime dtaIni = DateTime.MinValue;
                                        if (aditamentos.OrderBy(a => a.dt_aditamento).Last().dt_inicio_aditamento != null)
                                            dtaIni = (DateTime)aditamentos.OrderBy(a => a.dt_aditamento).Last().dt_inicio_aditamento;
                                        if (dtaIni == DateTime.MinValue)
                                            DtaIniAdtExtenso = "";
                                        else
                                            DtaIniAdtExtenso = dtaIni.Day + " de " + culture.TextInfo.ToTitleCase(dtfi.GetMonthName(dtaIni.Month)) + " de " + dtaIni.Year;
                                    }
                                    wordApp.Selection.TypeText(String.IsNullOrEmpty(DtaIniAdtExtenso) ? " " : DtaIniAdtExtenso);
                                    break;
                                case "AnoCorrente":
                                    string anoCoorente = DateTime.Now.Year + "";
                                    wordApp.Selection.TypeText(anoCoorente);
                                    break;
                                case "DataCorrenteExtenso":
                                    string DtaCorrenteExtenso = contrato.dtaCorrenteExtenso;
                                    wordApp.Selection.TypeText(String.IsNullOrEmpty(DtaCorrenteExtenso) ? " " : DtaCorrenteExtenso);
                                    break;
                                case "TipoAditamento":
                                    var tipoAdt = "";
                                    if (aditamentos.Count > 0)
                                        tipoAdt = aditamentos.OrderBy(a => a.dt_aditamento).Last().dc_tipo_aditamento;
                                    wordApp.Selection.TypeText(String.IsNullOrEmpty(tipoAdt) ? tipoAdt : tipoAdt);
                                    break;
                                case "VencimentosTitulosComDesc":
                                    #region Vencimentos Titulos Com Desc
                                    var descVencComTitulos = "";
                                    // Esse valor só irá aparecer caso haja algum desconto no contrato.
                                    if (contrato.vl_desconto_contrato > 0)
                                    {
                                        Aditamento aditamentoVencTitulos = new Aditamento();
                                        if (aditamentos != null)
                                            aditamentoVencTitulos = aditamentos.OrderBy(a => a.dt_aditamento).Last();

                                        //Caso o contrato não seja de aditamento concatenar o vencimento de todos os títulos da matrícula, e separados por vírgula.
                                        if (aditamentoVencTitulos.id_tipo_aditamento == null || aditamentos.Count <= 0)
                                            descVencComTitulos = Titulo.concatenarVencimentosTitulo(titulosContratoContext);
                                        else
                                        {
                                            List<Titulo> titulosEmAbertoMatricula = new List<Titulo>();
                                            titulosContratoContext = Titulo.revisarSaldoTituloBolsaContrato(titulosContratoContext, false, 0, false);

                                            //Se for de aditamento com o tipo diferente de “Adicionar parcelas”, concatenar o vencimento de todos os títulos em aberto da matrícula, e separados por vírgula.
                                            if (aditamentoVencTitulos != null && aditamentoVencTitulos.id_tipo_aditamento.HasValue &&
                                                aditamentoVencTitulos.id_tipo_aditamento.Value != (byte)Aditamento.TipoAditamento.ADICIONAR_PARCELAS)
                                                titulosEmAbertoMatricula = titulosContratoContext.Where(x => x.id_status_titulo == (int)Titulo.StatusTitulo.ABERTO &&
                                                                                                                statusCnabTitulo.Contains(x.id_status_cnab) &&
                                                                                                                x.vl_titulo == x.vl_saldo_titulo &&
                                                                                                                x.dc_tipo_titulo != "TM" && x.dc_tipo_titulo != "TA" &&
                                                                                                                x.dc_tipo_titulo != "AD" && x.dc_tipo_titulo != "AA").ToList();
                                            else //Caso seja aditamento com tipo “Adicionar Parcelas”, concatenar o vencimento de todos os títulos do tipo do aditamento, abertos e separados por vírgula.
                                                titulosEmAbertoMatricula = titulosContratoContext.Where(x => (x.dc_tipo_titulo == "AA" || x.dc_tipo_titulo == "AD") &&
                                                                        x.id_status_titulo == (int)Titulo.StatusTitulo.ABERTO && statusCnabTitulo.Contains(x.id_status_cnab) &&
                                                                        x.vl_titulo == x.vl_saldo_titulo).ToList();
                                            descVencComTitulos = Titulo.concatenarVencimentosTitulo(titulosEmAbertoMatricula.ToList());
                                        }
                                    }
                                    wordApp.Selection.TypeText(String.IsNullOrEmpty(descVencComTitulos) ? " " : descVencComTitulos);
                                    #endregion
                                    break;
                                case "VencimentosTitulosSemDesc":
                                    #region Vencimentos títulos sem desconto
                                    var descVencSemTitulos = "";
                                    // Esse valor só irá aparecer caso haja algum desconto no contrato.
                                    if (contrato.vl_desconto_contrato <= 0)
                                    {
                                        Aditamento aditamentoVencTitSemDesc = new Aditamento();

                                        if (aditamentos != null)
                                            aditamentoVencTitSemDesc = aditamentos.OrderBy(a => a.dt_aditamento).Last();

                                        //Caso o contrato não seja de aditamento concatenar o vencimento de todos os títulos da matrícula, e separados por vírgula.
                                        if (aditamentoVencTitSemDesc.id_tipo_aditamento == null || aditamentos.Count <= 0)
                                            descVencSemTitulos = Titulo.concatenarVencimentosTitulo(titulosContratoContext);
                                        else
                                        {
                                            List<Titulo> titulosEmAbertoMatricula = new List<Titulo>();
                                            titulosContratoContext = Titulo.revisarSaldoTituloBolsaContrato(titulosContratoContext, false, 0, false);

                                            //Se for de aditamento com o tipo diferente de “Adicionar parcelas”, concatenar o vencimento de todos os títulos em aberto da matrícula, e separados por vírgula.
                                            if (aditamentoVencTitSemDesc != null && aditamentoVencTitSemDesc.id_tipo_aditamento.HasValue &&
                                                aditamentoVencTitSemDesc.id_tipo_aditamento.Value != (byte)Aditamento.TipoAditamento.ADICIONAR_PARCELAS)
                                                titulosEmAbertoMatricula = titulosContratoContext.Where(x => x.id_status_titulo == (int)Titulo.StatusTitulo.ABERTO &&
                                                                                                                statusCnabTitulo.Contains(x.id_status_cnab) &&
                                                                                                                x.vl_titulo == x.vl_saldo_titulo &&
                                                                                                                x.dc_tipo_titulo != "TM" && x.dc_tipo_titulo != "TA" &&
                                                                                                                x.dc_tipo_titulo != "AD" && x.dc_tipo_titulo != "AA").ToList();
                                            else //Caso seja aditamento com tipo “Adicionar Parcelas”, concatenar o vencimento de todos os títulos do tipo do aditamento, abertos e separados por vírgula.
                                                titulosEmAbertoMatricula = titulosContratoContext.Where(x => (x.dc_tipo_titulo == "AA" || x.dc_tipo_titulo == "AD") &&
                                                                        x.id_status_titulo == (int)Titulo.StatusTitulo.ABERTO && statusCnabTitulo.Contains(x.id_status_cnab) &&
                                                                        x.vl_titulo == x.vl_saldo_titulo).ToList();
                                            descVencSemTitulos = Titulo.concatenarVencimentosTitulo(titulosEmAbertoMatricula.ToList());
                                        }
                                    }
                                    wordApp.Selection.TypeText(String.IsNullOrEmpty(descVencSemTitulos) ? " " : descVencSemTitulos);
                                    #endregion
                                    break;
                                case "DataFimContrato":
                                    string DtFinalContrato = contrato.dtFinalContrato;
                                    wordApp.Selection.TypeText(String.IsNullOrEmpty(DtFinalContrato) ? " " : DtFinalContrato);
                                    break;
                                case "DataMatriculaContrato":
                                    string DtMatriculaContrato = contrato.dtMatriculaContrato;
                                    wordApp.Selection.TypeText(String.IsNullOrEmpty(DtMatriculaContrato) ? " " : DtMatriculaContrato);
                                    break;
                                case "DataNascResponsavel":
                                    string DtNascResponsavel = contrato.dtNascResponsavel;
                                    wordApp.Selection.TypeText(String.IsNullOrEmpty(DtNascResponsavel) ? " " : DtNascResponsavel);
                                    break;
                                case "NumeroContrato":
                                    int? nmContrato = contrato.nm_contrato;
                                    wordApp.Selection.TypeText(nmContrato != null ? nmContrato + "" : "");
                                    break;
                                case "Observacao":
                                    string obsContrato = contrato.Aditamento.FirstOrDefault().tx_obs_aditamento;
                                    wordApp.Selection.TypeText(string.IsNullOrEmpty(obsContrato) ? " " : obsContrato);
                                    break;
                                case "ParcelaLiquida":
                                    string vlParcelaLiquida = contrato.vlParcelaLiquida;
                                    wordApp.Selection.TypeText(string.IsNullOrEmpty(vlParcelaLiquida) ? " " : vlParcelaLiquida);
                                    break;
                                case "TipoFinanceiroTaxa":
                                    string dc_tipo_financ_taxa = contrato.dc_tipo_financeiro_taxa;
                                    wordApp.Selection.TypeText(string.IsNullOrEmpty(dc_tipo_financ_taxa) ? " " : dc_tipo_financ_taxa);
                                    break;
                                case "TipoMatricula":
                                    string dc_tipo_matricula = contrato.dc_tipo_matricula;
                                    wordApp.Selection.TypeText(string.IsNullOrEmpty(dc_tipo_matricula) ? " " : dc_tipo_matricula);
                                    break;
                                case "Modalidade":
                                    string dc_regime_turma = contrato.dc_regime_turma;
                                    wordApp.Selection.TypeText(string.IsNullOrEmpty(dc_regime_turma) ? " " : dc_regime_turma);
                                    break;
                                case "GradeValoresDescontosAntecipa":
                                    #region Grade Valores Descontos Antecipação
                                    var grupoDescontosAntecipacao = descontosPoliticaTitulos.GroupBy(x => new { x.cd_politica_desconto, x.nm_dia_limite_politica })
                                                                                                   .Select(x => new BaixaTitulo.DiasPoliticaAntecipacao
                                                                                                   {
                                                                                                       cd_politica_desconto = x.Key.cd_politica_desconto,
                                                                                                       nm_dia_limite_politica = x.Key.nm_dia_limite_politica
                                                                                                   }).ToList();
                                    //if (grupoDescontosAntecipacao != null && grupoDescontosAntecipacao.Count > 0)
                                    //    grupoDescontosAntecipacao = descontosPoliticaTitulos.Where(x => grupoDescontosAntecipacao.Select(c => c.cd_politica_desconto).Contains(x.cd_politica_desconto))
                                    //        .GroupBy(x
                                    if (grupoDescontosAntecipacao != null && grupoDescontosAntecipacao.Count > 0)
                                    {
                                        wordApp.Selection.TypeText(" ");
                                        tab = wordApp.Selection.Tables.Add(rngFieldCode, grupoDescontosAntecipacao.Count + 1, 4);

                                        tab.Columns[1].Width = 20;
                                        tab.Columns[2].Width = 145;
                                        tab.Columns[3].Width = 130;
                                        tab.Columns[4].Width = 145;
                                        tab.Cell(1, 2).Range.Text = "Dia Venc. Desconto por Antecipação";
                                        tab.Cell(1, 3).Range.Text = "Dia";
                                        tab.Cell(1, 4).Range.Text = "(%)";
                                        tab.Cell(1, 2).Range.Bold = 1;
                                        tab.Cell(1, 3).Range.Bold = 1;
                                        tab.Cell(1, 4).Range.Bold = 1;
                                        tab.Cell(1, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                        tab.Cell(1, 3).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                        tab.Cell(1, 4).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                        tab.Cell(1, 2).VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalTop;
                                        tab.Cell(1, 3).VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalTop;
                                        tab.Cell(1, 4).VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalTop;
                                        tab.Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
                                        tab.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
                                        tab.Cell(1, 1).Borders.Enable = 0;
                                        tab.Cell(1, 2).Borders.Enable = 1;
                                        tab.PreferredWidthType = Word.WdPreferredWidthType.wdPreferredWidthPoints;
                                        tab.PreferredWidth = 440;
                                        tab.AllowPageBreaks = true;
                                        tab.AllowAutoFit = false;

                                        int i, row;

                                        for (i = 0, row = 2; i < grupoDescontosAntecipacao.Count; i++, row++)
                                        {
                                            var _dataPolitica = descontosPoliticaTitulos.Where(x => x.cd_politica_desconto == grupoDescontosAntecipacao[i].cd_politica_desconto &&
                                                                                                             x.nm_dia_limite_politica == grupoDescontosAntecipacao[i].nm_dia_limite_politica)
                                                                                                             .FirstOrDefault().Data_politica;
                                            DateTime _dataVencimentoMat = Contrato.gerarDataVencimentoMatricula(contrato.nm_dia_vcto, contrato.nm_mes_vcto, contrato.nm_ano_vcto);
                                            if (_dataPolitica < _dataVencimentoMat)
                                                _dataPolitica = _dataVencimentoMat;
                                            tab.Cell(row, 2).Range.Text = "A partir de " + _dataPolitica.ToString("dd/MM/yyyy");
                                            tab.Cell(row, 3).Range.Text = descontosPoliticaTitulos.Where(x => x.cd_politica_desconto == grupoDescontosAntecipacao[i].cd_politica_desconto &&
                                                                                                             x.nm_dia_limite_politica == grupoDescontosAntecipacao[i].nm_dia_limite_politica)
                                                                                                             .FirstOrDefault().nm_dia_limite_politica.ToString();
                                            tab.Cell(row, 4).Range.Text = string.Format("{0,00}", descontosPoliticaTitulos.Where(x => x.cd_politica_desconto == grupoDescontosAntecipacao[i].cd_politica_desconto &&
                                                                                                             x.nm_dia_limite_politica == grupoDescontosAntecipacao[i].nm_dia_limite_politica)
                                                                                                             .FirstOrDefault().pc_pontualidade);
                                            tab.Cell(row, 1).Borders.Enable = 0;
                                            tab.Cell(row, 2).Borders.Enable = 1;
                                            tab.Cell(row, 2).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                            tab.Cell(row, 3).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                            tab.Cell(row, 4).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                        }
                                        //tab.Cell(row, 2).Range.Text = "";
                                        //tab.Cell(row, 3).Range.Text = "Total:";
                                        tab.Cell(row, 1).Borders.Enable = 0;
                                        tab.Cell(row, 2).Borders.Enable = 1;
                                        tab.Cell(row, 2).Range.Bold = 1;
                                        //tab.Cell(row, 3).Range.Bold = 1;
                                        tab.Cell(row, 2).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                        //tab.Cell(row, 3).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                    }
                                    else
                                        wordApp.Selection.TypeText("Não informado");
                                    #endregion
                                    break;
                                case "GradeDescontosContrato":
                                    List<DescontoContrato> listaDescontosContrato = BusinessMatricula.getDescontosAplicadosContratoOrAditamento(contrato.cd_contrato, cd_escola).ToList();
                                    #region  Grade Descontos Contrato
                                    if (listaDescontosContrato != null && listaDescontosContrato.Count > 0)
                                    {

                                        wordApp.Selection.TypeText(" ");
                                        tab = wordApp.Selection.Tables.Add(rngFieldCode, listaDescontosContrato.Count + 1, 5);
                                        tab.Columns[1].Width = 20;
                                        tab.Columns[2].Width = 135;
                                        tab.Columns[3].Width = 100;
                                        tab.Columns[4].Width = 50;
                                        tab.Columns[5].Width = 135;
                                        tab.Cell(1, 2).Range.Text = "Descrição do(s) Desconto(s)";
                                        tab.Cell(1, 3).Range.Text = "Dia Venc. Tipos de Desconto";
                                        tab.Cell(1, 4).Range.Text = "%/R$";
                                        tab.Cell(1, 5).Range.Text = "Parcela(s)";
                                        tab.Cell(1, 2).Range.Bold = 1;
                                        tab.Cell(1, 3).Range.Bold = 1;
                                        tab.Cell(1, 4).Range.Bold = 1;
                                        tab.Cell(1, 5).Range.Bold = 1;
                                        tab.Cell(1, 2).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                        tab.Cell(1, 3).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                        tab.Cell(1, 4).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                        tab.Cell(1, 5).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                        tab.Cell(1, 2).VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalTop;
                                        tab.Cell(1, 3).VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalTop;
                                        tab.Cell(1, 4).VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalTop;
                                        tab.Cell(1, 5).VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalTop;
                                        tab.Borders.InsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleSingle;
                                        tab.Borders.OutsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleSingle;
                                        tab.Cell(1, 1).Borders.Enable = 0;
                                        tab.Cell(1, 2).Borders.Enable = 1;
                                        tab.PreferredWidthType = Word.WdPreferredWidthType.wdPreferredWidthPoints;
                                        tab.PreferredWidth = 440;
                                        tab.AllowPageBreaks = true;
                                        tab.AllowAutoFit = false;

                                        int i, row;

                                        for (i = 0, row = 2; i < listaDescontosContrato.Count; i++, row++)
                                        {
                                            tab.Cell(row, 2).Range.Text = listaDescontosContrato[i].dc_tipo_desconto;
                                            tab.Cell(row, 3).Range.Text = contrato.nm_dia_vcto.ToString();
                                            tab.Cell(row, 4).Range.Text = string.Format("{0,00}", listaDescontosContrato[i].pc_desconto_contrato > 0 ?
                                                                                                   listaDescontosContrato[i].pc_desconto_contrato + "%" :
                                                                                                  "R$ " + listaDescontosContrato[i].vl_desconto_contrato);
                                            tab.Cell(row, 5).Range.Text = DescontoContrato.gerarDescDescontosContrato(titulosAbertos, listaDescontosContrato[i].nm_parcela_ini,
                                                                                                                      listaDescontosContrato[i].nm_parcela_fim);
                                            tab.Cell(row, 1).Borders.Enable = 0;
                                            tab.Cell(row, 2).Borders.Enable = 1;
                                            tab.Cell(row, 2).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                            tab.Cell(row, 3).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                            tab.Cell(row, 4).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                            tab.Cell(row, 5).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                        }
                                        //tab.Cell(row, 2).Range.Text = "";
                                        //tab.Cell(row, 3).Range.Text = "Total:";
                                        tab.Cell(row, 1).Borders.Enable = 0;
                                        tab.Cell(row, 2).Borders.Enable = 1;

                                        tab.Cell(row, 2).Range.Bold = 1;
                                        tab.Cell(row, 3).Range.Bold = 1;
                                        tab.Cell(row, 4).Range.Bold = 1;
                                        tab.Cell(row, 5).Range.Bold = 1;

                                        //tab.Cell(row, 3).Range.Bold = 1;
                                        tab.Cell(row, 2).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                        tab.Cell(row, 3).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                        tab.Cell(row, 4).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                        tab.Cell(row, 5).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                        //tab.Cell(row, 3).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                    }
                                    else
                                        wordApp.Selection.TypeText("Não informado");
                                    #endregion
                                    break;
                                case "GradeValoresLiquidos":
                                    #region  Valores Liquidos

                                    wordApp.Selection.TypeText(" ");
                                    var grupoDescontosAntecipacaoVL = descontosPoliticaTitulos.OrderBy(x => x.Data_politica)
                                        .GroupBy(x => new { x.cd_politica_desconto })
                                        .Select(x => new BaixaTitulo.DiasPoliticaAntecipacao
                                        {
                                            cd_politica_desconto = x.Key.cd_politica_desconto,
                                            qtd_politica = x.Where(g => g.cd_politica_desconto == x.Key.cd_politica_desconto)
                                                            .GroupBy(gx => new { gx.cd_politica_desconto, gx.nm_dia_limite_politica }).Count()
                                        }).ToList();

                                    var qtdTitulosAbertosPolitica = titulosAbertos.Where(x => x.BaixaTitulo.Any(b => b.vl_desconto_baixa > 0)).OrderBy(x => x.cd_titulo).OrderBy(z => z.cd_titulo).ToList();

                                    if (grupoDescontosAntecipacaoVL != null && grupoDescontosAntecipacaoVL.Count > 0)
                                    {
                                        int qtd_maior_dias_politica = grupoDescontosAntecipacaoVL.Max(x => x.qtd_politica);
                                        //Todos títulos com desconto de antecipação, e a maior quantidade de dias politica, e a quantidade de linhas sera: quantidade de títulos + quantidade de politicas * 2(coluna de descrição da politica e descrição de cada coluna)
                                        tab = wordApp.Selection.Tables.Add(rngFieldCode, titulosAbertos.Where(x => x.BaixaTitulo.Any(b => b.diasPoliticaAntecipacao != null &&
                                                                                                                       b.diasPoliticaAntecipacao.Any())).Count() +
                                                                                                                      (grupoDescontosAntecipacaoVL.Count * 3), qtd_maior_dias_politica + 4);
                                        int i = 0, row = 3, iRowColumn = 2;
                                        foreach (var group in grupoDescontosAntecipacaoVL)
                                        {
                                            var groupDias = descontosPoliticaTitulos.Where(x => x.cd_politica_desconto == group.cd_politica_desconto).OrderBy(x => x.Data_politica)
                                                                                    .GroupBy(x => new { x.cd_politica_desconto, x.nm_dia_limite_politica })
                                                                                                   .Select(x => new BaixaTitulo.DiasPoliticaAntecipacao
                                                                                                   {
                                                                                                       cd_politica_desconto = x.Key.cd_politica_desconto,
                                                                                                       nm_dia_limite_politica = x.Key.nm_dia_limite_politica
                                                                                                   }).ToList();
                                            var titulosAbertosPolitica = titulosAbertos.Where(x => x.BaixaTitulo.Any(b => b.diasPoliticaAntecipacao!= null &&
                                                b.diasPoliticaAntecipacao.Any(dp =>
                                                dp.cd_politica_desconto == group.cd_politica_desconto))).OrderBy(x => x.cd_titulo).ToList();

                                            //int rowTotal = 3 + groupDias.Count();
                                            //Para pular as 2 primeiras linhas na segunda rodada de criação de dias politica
                                            if (row > 3)
                                                iRowColumn = row + 1;
                                            if (row == 3)
                                            {
                                                tab.Columns[1].Width = 20;
                                                tab.Columns[2].Width = 70;
                                                tab.Columns[3].Width = 70;
                                            }
                                            int iClm = 3;

                                            //tab.Cell(iRowColumn - 1, 1).Merge(tab.Cell(iRowColumn - 1, 2));
                                            foreach (var diasPolitica in groupDias)
                                            {
                                                iClm++;
                                                if (row == 3)
                                                {
                                                    //tab.Columns[iClm].Width = 240 / qtd_maior_dias_politica;
                                                    //tab.Columns[iClm].Width = 105;
                                                }
                                                tab.Cell(iRowColumn, iClm).Range.Text = "Valor da(s) Parcela(s) (R$) até dia " + diasPolitica.nm_dia_limite_politica;
                                                tab.Cell(iRowColumn, iClm).Range.Bold = 1;
                                                tab.Cell(iRowColumn, iClm).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                                tab.Cell(iRowColumn, iClm).VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalTop;
                                                tab.Cell(iRowColumn - 1, 3).Merge(tab.Cell(iRowColumn - 1, 4));
                                            }
                                            tab.Cell(iRowColumn - 1, 2).Merge(tab.Cell(iRowColumn - 1, 3));
                                            var _dataPolitica = descontosPoliticaTitulos.Where(x => x.cd_politica_desconto == group.cd_politica_desconto)
                                                                                                                        .FirstOrDefault().Data_politica;
                                            DateTime _dataVencimentoMat = Contrato.gerarDataVencimentoMatricula(contrato.nm_dia_vcto, contrato.nm_mes_vcto, contrato.nm_ano_vcto);
                                            if (_dataPolitica < _dataVencimentoMat)
                                                _dataPolitica = _dataVencimentoMat;
                                            tab.Cell(iRowColumn - 1, 2).Range.Text = "A partir de " + _dataPolitica.ToString("dd/MM/yyyy");
                                            tab.Cell(iRowColumn, 2).Range.Text = "Parcela(s) do(s) Desconto(s)";
                                            tab.Cell(iRowColumn, 3).Range.Text = "MATERIAL (R$)";
                                            tab.Cell(iRowColumn, iClm + 1).Range.Text = "Até o dia " + contrato.nm_dia_vcto;
                                            tab.Cell(iRowColumn, iClm + 1).Range.Bold = 1;
                                            tab.Cell(iRowColumn, 2).Range.Bold = 1;
                                            tab.Cell(iRowColumn, 3).Range.Bold = 1;
                                            tab.Cell(iRowColumn, 2).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                            tab.Cell(iRowColumn, 3).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                            tab.Cell(iRowColumn, iClm + 1).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                            tab.Cell(iRowColumn, 2).VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalTop;
                                            tab.Cell(iRowColumn, 3).VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalTop;
                                            tab.Cell(iRowColumn, iClm + 1).VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalTop;
                                            tab.Borders.InsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleSingle;
                                            tab.Borders.OutsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleSingle;
                                            //tab.Cell(iRowColumn, 1).Borders.Enable = 0;
                                            //tab.Cell(iRowColumn, 2).Borders.Enable = 1;
                                            tab.PreferredWidthType = Word.WdPreferredWidthType.wdPreferredWidthPoints;
                                            tab.PreferredWidth = 440;
                                            tab.AllowPageBreaks = true;
                                            tab.AllowAutoFit = false;
                                            int columnDias = 3;
                                            if (row > 3)
                                                row += 2;
                                            List<BaixaTitulo.DiasPoliticaAntecipacao> listaTotais = new List<BaixaTitulo.DiasPoliticaAntecipacao>();
                                            for (i = 0; i < titulosAbertosPolitica.Count(); i++, row++)
                                            {
                                                tab.Cell(row, 2).Range.Text = titulosAbertosPolitica[i].nm_parcela_titulo + "º";
                                                tab.Cell(row, 2).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                                foreach (var diasG in groupDias)
                                                {
                                                    columnDias++;
                                                    var pc_desconto = (descontosPoliticaTitulos.Where(x => x.cd_politica_desconto == diasG.cd_politica_desconto &&
                                                                       x.nm_dia_limite_politica == diasG.nm_dia_limite_politica &&
                                                                       x.cd_titulo == titulosAbertosPolitica[i].cd_titulo).FirstOrDefault() != null ? (descontosPoliticaTitulos.Where(x => x.cd_politica_desconto == diasG.cd_politica_desconto &&
                                                                       x.nm_dia_limite_politica == diasG.nm_dia_limite_politica &&
                                                                       x.cd_titulo == titulosAbertosPolitica[i].cd_titulo).FirstOrDefault().pc_pontualidade_total) : 0);
                                                    var vl_titulo_calc = titulosAbertosPolitica[i].vl_saldo_titulo -
                                                        ((titulosAbertosPolitica[i].vl_saldo_titulo - titulosAbertosPolitica[i].vl_material_titulo) / 100 * pc_desconto);
                                                    tab.Cell(row, columnDias).Range.Text = string.Format("{0:#,0.00}", vl_titulo_calc);
                                                    tab.Cell(row, columnDias).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                                    //tab.Cell(row, columnDias).Range.Text = string.Format("{0:#,0.00}", vl_titulo_calc);
                                                    //tab.Cell(row, columnDias).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                                    listaTotais.Add(new BaixaTitulo.DiasPoliticaAntecipacao { nm_coluna = columnDias, cd_titulo = titulosAbertosPolitica[i].cd_titulo, 
                                                                                                              vl_titulo = vl_titulo_calc, nm_dia_limite_politica = diasG.nm_dia_limite_politica });
                                                }
                                                if (titulosAbertosPolitica[i].vl_material_titulo > 0)
                                                {
                                                    tab.Cell(row, 3).Range.Text = string.Format("{0:#,0.00}", titulosAbertosPolitica[i].vl_material_titulo);
                                                    tab.Cell(row, 3).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                                }
                                                Decimal desconto_baixa = descontosPoliticaTitulos.Where(x => x.cd_politica_desconto == group.cd_politica_desconto &&
                                                                       x.cd_titulo == titulosAbertosPolitica[i].cd_titulo).FirstOrDefault().pc_desconto_baixa;
                                                var vl_titulo_ate_vencimento = titulosAbertosPolitica[i].vl_saldo_titulo -
                                                        ((titulosAbertosPolitica[i].vl_saldo_titulo - titulosAbertosPolitica[i].vl_material_titulo) / 100 * desconto_baixa);
                                                tab.Cell(row, columnDias + 1).Range.Text = string.Format("{0:#,0.00}", vl_titulo_ate_vencimento);
                                                tab.Cell(row, columnDias + 1).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                                listaTotais.Add(new BaixaTitulo.DiasPoliticaAntecipacao { nm_coluna = columnDias + 1, cd_titulo = titulosAbertosPolitica[i].cd_titulo, vl_titulo = vl_titulo_ate_vencimento, nm_dia_limite_politica = 0 });
                                                columnDias = 3;
                                                //tab.Cell(row, 1).Borders.Enable = 0;
                                                
                                            }
                                            if (contrato.vl_material_contrato > 0)
                                            {
                                                if ((bool)contrato.id_valor_incluso && contrato.vl_material_contrato > 0)
                                                    tab.Cell(row, 3).Range.Text = string.Format("{0:#,0.00}", contrato.vl_material_contrato);
                                                else
                                                    tab.Cell(row, 3).Range.Text = string.Format("{0:#,0.00}", contrato.nm_parcelas_material *
                                                        contrato.vl_parcela_liq_material);
                                            }
                                            else
                                                tab.Cell(row, 3).Range.Text = "";
                                            tab.Cell(row, 3).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                            //Campos de total dos descontos
                                            //Colunas Totalizadores de cada politica de desconto.
                                            tab.Cell(row, 2).Range.Text = "Total:";
                                            tab.Cell(row, 2).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                            if (listaTotais != null && listaTotais.Count() > 0)
                                            {
                                                var groupTotaisTitulo = listaTotais.GroupBy(x => new { x.cd_politica_desconto, x.nm_dia_limite_politica, x.nm_coluna })
                                                                                                       .Select(x => new BaixaTitulo.DiasPoliticaAntecipacao
                                                                                                       {
                                                                                                           cd_politica_desconto = x.Key.cd_politica_desconto,
                                                                                                           nm_dia_limite_politica = x.Key.nm_dia_limite_politica,
                                                                                                           nm_coluna = x.Key.nm_coluna
                                                                                                       }).ToList();
                                                foreach (var g in groupTotaisTitulo)
                                                {
                                                    tab.Cell(row, g.nm_coluna).Range.Text = string.Format("{0:#,0.00}", listaTotais.Where(x=> x.nm_dia_limite_politica == g.nm_dia_limite_politica).Sum(x=> x.vl_titulo));
                                                    tab.Cell(row, g.nm_coluna).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                                }
                                            }

                                        }
                                        foreach (Word.Row r in tab.Rows)
                                        {
                                            tab.Cell(r.Index, 1).Borders.Enable = 0;
                                            tab.Cell(r.Index, 2).Borders.Enable = 1;
                                        }
                                    }

                                    else if (grupoDescontosAntecipacaoVL.Count <= 0 &&
                                             baixasSimuladas != null && baixasSimuladas.Count > 0 &&
                                             qtdTitulosAbertosPolitica != null && qtdTitulosAbertosPolitica.Count > 0)
                                    {
                                        tab = wordApp.Selection.Tables.Add(rngFieldCode, titulosAbertos.Where(x => x.BaixaTitulo.Any(b => b.vl_desconto_baixa > 0)).Count() +
                                                                                                                      (2),  4);
                                        int i = 0, row = 2, iRowColumn = 1;
                                        //foreach (var group in baixasSimuladas)
                                       // {
                                            var groupDias = baixasSimuladas.ToList();
                                            
                                            var titulosAbertosPolitica = titulosAbertos.Where(x => x.BaixaTitulo.Any(b => b.vl_desconto_baixa > 0)).OrderBy(x => x.cd_titulo).OrderBy(z=> z.cd_titulo).ToList();

                                            if (row > 3)
                                                iRowColumn = row + 1;
                                            if (row == 2)
                                            {
                                                tab.Columns[1].Width = 20;
                                                tab.Columns[2].Width = 70;
                                                tab.Columns[3].Width = 70;
                                            }
                                            int iClm = 3;

                                            
                                            //tab.Cell(iRowColumn - 1, 2).Merge(tab.Cell(iRowColumn - 1, 3));

                                            DateTime _dataVencimentoMat = Contrato.gerarDataVencimentoMatricula(contrato.nm_dia_vcto, contrato.nm_mes_vcto, contrato.nm_ano_vcto);
                                           

                                            tab.Cell(iRowColumn, 2).Range.Text = "Parcela(s) do(s) Desconto(s)";
                                            tab.Cell(iRowColumn, 3).Range.Text = "MATERIAL (R$)";
                                            tab.Cell(iRowColumn, iClm + 1).Range.Text = "Até o dia " + contrato.nm_dia_vcto;
                                            tab.Cell(iRowColumn, iClm + 1).Range.Bold = 1;
                                            tab.Cell(iRowColumn, 2).Range.Bold = 1;
                                            tab.Cell(iRowColumn, 3).Range.Bold = 1;
                                            tab.Cell(iRowColumn, 2).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                            tab.Cell(iRowColumn, 3).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                            tab.Cell(iRowColumn, iClm + 1).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                            tab.Cell(iRowColumn, 2).VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalTop;
                                            tab.Cell(iRowColumn, 3).VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalTop;
                                            tab.Cell(iRowColumn, iClm + 1).VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalTop;
                                            tab.Borders.InsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleSingle;
                                            tab.Borders.OutsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleSingle;
                                            //tab.Cell(iRowColumn, 1).Borders.Enable = 0;
                                            //tab.Cell(iRowColumn, 2).Borders.Enable = 1;
                                            tab.PreferredWidthType = Word.WdPreferredWidthType.wdPreferredWidthPoints;
                                            tab.PreferredWidth = 440;
                                            tab.AllowPageBreaks = true;
                                            tab.AllowAutoFit = false;
                                            int columnDias = 3;
                                            if (row > 3)
                                                row += 2;
                                            List<decimal> listaTotais = new List<decimal>();
                                            List<decimal> listaValoresMaterialTitulo = new List<decimal>();
                                            for (i = 0; i < titulosAbertosPolitica.Count(); i++, row++)
                                            {
                                                tab.Cell(row, 2).Range.Text = titulosAbertosPolitica[i].nm_parcela_titulo + "º";
                                                tab.Cell(row, 2).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;

                                               
                                                if (titulosAbertosPolitica[i].vl_material_titulo > 0)
                                                {
                                                    tab.Cell(row, 3).Range.Text = string.Format("{0:#,0.00}", titulosAbertosPolitica[i].vl_material_titulo);
                                                    tab.Cell(row, 3).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                                    listaValoresMaterialTitulo.Add(titulosAbertosPolitica[i].vl_material_titulo);
                                                }
                                                BaixaTitulo baixa_simulada_aux = baixasSimuladas.Where(b => b.cd_titulo == titulosAbertosPolitica[i].cd_titulo).FirstOrDefault();

                                                var vl_titulo_ate_vencimento = baixa_simulada_aux.vl_liquidacao_baixa - titulosAbertosPolitica[i].vl_material_titulo;
                                                tab.Cell(row, columnDias + 1).Range.Text = string.Format("{0:#,0.00}", vl_titulo_ate_vencimento);
                                                tab.Cell(row, columnDias + 1).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                                listaTotais.Add(vl_titulo_ate_vencimento);
                                                columnDias = 3;
                                                //tab.Cell(row, 1).Borders.Enable = 0;

                                            }

                                            if (listaValoresMaterialTitulo.Count > 0)
                                            {
                                                tab.Cell(row, 3).Range.Text = string.Format("{0:#,0.00}", listaValoresMaterialTitulo.Sum());
                                            }
                                            else
                                            {
                                                tab.Cell(row, 3).Range.Text = "0,00";
                                            }


                                            tab.Cell(row, 3).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                            //Colunas Totalizadores 
                                            tab.Cell(row, 2).Range.Text = "Total:";
                                            tab.Cell(row, 2).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                            if (listaTotais != null && listaTotais.Count() > 0)
                                            {
                                                    tab.Cell(row, 4).Range.Text = string.Format("{0:#,0.00}", listaTotais.Sum());
                                                    tab.Cell(row, 4).Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                            }

                                        //}
                                        foreach (Word.Row r in tab.Rows)
                                        {
                                            tab.Cell(r.Index, 1).Borders.Enable = 0;
                                            tab.Cell(r.Index, 2).Borders.Enable = 1;
                                        }
                                    }
                                    else
                                        wordApp.Selection.TypeText(" Não informado");
                                    #endregion
                                    break;
                                default:
                                    throw new SecretariaBusinessException(string.Format(Utils.Messages.Messages.msgParametroNaoEsperado, "&lt;" + fieldName + "&gt;"), null, SecretariaBusinessException.TipoErro.ERRO_PARAMETRO_NAO_ECONTRADO, false);
                            }
                        }
                    }
                    wordDoc.SaveAs(caminho_relatorios + "/" + file_name + ".pdf", Word.WdExportFormat.wdExportFormatPDF);
                    //wordApp.Documents.Open("myFile.doc");
                }
                //catch (Exception ex)
                //{
                //    return ex;
                //}
                finally
                {
                    fecharInstanciasWord(ref oMissing, ref doNotSaveChanges, ref wordApp, ref wordDoc);
                    deletarArquivoTempContrato(caminho_temp_contrato);
                }
            }
            else
                throw new SecretariaBusinessException(Messages.msgRegNotEnc, null, SecretariaBusinessException.TipoErro.ERRO_PARAMETRO_NAO_ECONTRADO, false);
            byte[] data = File.ReadAllBytes(caminho_relatorios + "/" + file_name + ".pdf");
            MemoryStream stream = new MemoryStream(data);

            return stream;
        }

        private string MesExtenso(int Mes)
        {
            string dcMes = "";
            switch (Mes)
            {
                case 1:
                    dcMes = "Jan/";
                    break;
                case 2:
                    dcMes = "Fev/";
                    break;
                case 3:
                    dcMes = "Mar/";
                    break;
                case 4:
                    dcMes = "Abr/";
                    break;
                case 5:
                    dcMes = "Mai/";
                    break;
                case 6:
                    dcMes = "Jun/";
                    break;
                case 7:
                    dcMes = "Jul/";
                    break;
                case 8:
                    dcMes = "Ago/";
                    break;
                case 9:
                    dcMes = "Set/";
                    break;
                case 10:
                    dcMes = "Out/";
                    break;
                case 11:
                    dcMes = "Nov/";
                    break;
                case 12:
                    dcMes = "Dez/";
                    break;
            }
            return dcMes;
        }

        private void deletarArquivoTempContrato(string caminho_temp_contrato)
        {
            if (File.Exists(caminho_temp_contrato))
            {
                File.Delete(caminho_temp_contrato);
            }
        }

        private string criarArquivoTempContrato(string antigo_caminho_contrato, string caminho_relatorios)
        {
            string novo_caminho_contrato = Path.Combine(caminho_relatorios, "TempContratos");
            if(caminho_relatorios.StartsWith(@"\\"))
                novo_caminho_contrato = Path.Combine(caminho_relatorios.Replace(@"\\", @"\").Replace(@"\\", @"\"), "TempContratos").Insert(0, @"\");

            string novo_nome_contrato = Guid.NewGuid().ToString();

            FileInfo arquivo_temp_contrato = new FileInfo(antigo_caminho_contrato);
            if (arquivo_temp_contrato.Exists)
            {
                if (!Directory.Exists(novo_caminho_contrato))
                {
                    Directory.CreateDirectory(novo_caminho_contrato);
                }
                arquivo_temp_contrato.CopyTo(Path.Combine(novo_caminho_contrato, novo_nome_contrato + arquivo_temp_contrato.Extension));
                logger.Error(string.Format("novo_caminho_contrato: - {0}", Path.Combine(novo_caminho_contrato, novo_nome_contrato + arquivo_temp_contrato.Extension)));
            }

            var caminho_contrato_OTemplate = Path.Combine(novo_caminho_contrato, novo_nome_contrato + arquivo_temp_contrato.Extension);
            logger.Error(string.Format("caminho_contrato_OTemplate: - {0}", caminho_contrato_OTemplate));
            return caminho_contrato_OTemplate;
        }

        private string criarArquivoTempContratoOpenXml(string antigo_caminho_contrato, string caminho_relatorios, ref string nome_temp_guid)
        {
            string novo_caminho_contrato = Path.Combine(caminho_relatorios, "TempContratos");
            if (caminho_relatorios.StartsWith(@"\\"))
                novo_caminho_contrato = Path.Combine(caminho_relatorios.Replace(@"\\", @"\").Replace(@"\\", @"\"), "TempContratos").Insert(0, @"\");

            string novo_nome_contrato = Guid.NewGuid().ToString();
            nome_temp_guid = novo_nome_contrato;

            FileInfo arquivo_temp_contrato = new FileInfo(antigo_caminho_contrato);
            if (arquivo_temp_contrato.Exists)
            {
                if (!Directory.Exists(novo_caminho_contrato))
                {
                    Directory.CreateDirectory(novo_caminho_contrato);
                }
                arquivo_temp_contrato.CopyTo(Path.Combine(novo_caminho_contrato, novo_nome_contrato + arquivo_temp_contrato.Extension));
                logger.Error(string.Format("novo_caminho_contrato: - {0}", Path.Combine(novo_caminho_contrato, novo_nome_contrato + arquivo_temp_contrato.Extension)));
            }

            var caminho_contrato_OTemplate = Path.Combine(novo_caminho_contrato, novo_nome_contrato + arquivo_temp_contrato.Extension);
            logger.Error(string.Format("caminho_contrato_OTemplate: - {0}", caminho_contrato_OTemplate));
            return caminho_contrato_OTemplate;
        }

        private static void fecharInstanciasWord(ref Object oMissing, ref Object doNotSaveChanges, ref Word.Application wordApp, ref Word.Document wordDoc)
        {
            //wordApp.Visible = false;
            //wordApp.ScreenUpdating = false;
            //wordApp.Application.DDETerminateAll(); (Ocasiona possíveis erros em ambiente de produção).
            // Release all Interop objects.
            if (wordDoc != null) 
            {
                wordDoc.Close(ref doNotSaveChanges, ref oMissing, ref oMissing);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(wordDoc);
            }
            if (wordApp != null)
            {
                wordApp.Application.Quit();             
                System.Runtime.InteropServices.Marshal.ReleaseComObject(wordApp);
            }
            //System.Runtime.InteropServices.Marshal.FinalReleaseComObject(wordApp); (Ocasiona possíveis erros em ambiente de produção).
            wordDoc = null;
            wordApp = null;
            GC.Collect();
        }

        public Recibo getSourceReciboBaixaProspect(int cd_prospect, int cd_empresa)
        {
            Recibo recibo = BusinessSecretaria.getReciboByProspect(cd_prospect, cd_empresa);
            recibo.dc_extenso = ConverterExtenso.toExtenso(recibo.vl_liquidacao_baixa);
            return recibo;
        }

        public Recibo GetSourceReciboBaixa(int cd_baixa_titulo, int cd_empresa)
        {
            Recibo recibo = BusinessFinanceiro.getReciboByBaixa(cd_baixa_titulo, cd_empresa);
            recibo.dc_extenso = ConverterExtenso.toExtenso(recibo.vl_liquidacao_baixa);
            return recibo;
        }

        public ReciboAgrupadoUI GetSourceReciboAgrupado(string cds_titulos_selecionados, int cd_empresa)
        {
            ReciboAgrupadoUI recibo = BusinessFinanceiro.getReciboAgrupado(cds_titulos_selecionados, cd_empresa);
            return recibo;
        }

        public ReciboPagamentoUI getReciboPagemento(int cd_baixa_titulo, int cd_empresa)
        {
            var reciboPgt = BusinessFinanceiro.getReciboPagamentoByBaixa(cd_baixa_titulo, cd_empresa);
            return reciboPgt;
        }

        public ReciboConfirmacaoUI getReciboConfirmacaoByContrato(int cd_contrato, int cd_empresa)
        {
            var reciboPgt = BusinessFinanceiro.getReciboConfirmacaoByContrato(cd_contrato, cd_empresa);
            return reciboPgt;
        }

        public List<ReciboConfirmacaoParcelasUI> getParcelasReciboConfirmacaoByContrato(int cd_contrato, int cd_empresa)
        {
            var reciboPgt = BusinessFinanceiro.getParcelasReciboConfirmacaoByContrato(cd_contrato, cd_empresa);
            return reciboPgt;
        }

        public List<ReciboConfirmacaoParcelasUI> getParcelasReciboConfirmacaoByMovimento(int cd_movimento, int cd_empresa)
        {
            var reciboPgt = BusinessFinanceiro.getParcelasReciboConfirmacaoByMovimento(cd_movimento, cd_empresa);
            return reciboPgt;
        }

        public ReciboConfirmacaoUI getReciboConfirmacaoByMovimento(int cd_movimento, int cd_empresa)
        {
            var reciboPgt = BusinessFinanceiro.getReciboConfirmacaoByMovimento(cd_movimento, cd_empresa);
            return reciboPgt;
        }

        public IEnumerable<ContaCorrenteUI> getReportContaCorrente(int cdEscola, DateTime pDtaI, DateTime pDtaF, int pLocalMovto, int tipoLiquidacao, bool contaSegura, bool isMaster)
        {
            return BusinessFinanceiro.getRelatorioContaCorrente(cdEscola, pDtaI, pDtaF, pLocalMovto, tipoLiquidacao, contaSegura, isMaster);
        }

        public Espelho getSourceEspelhoMovimento(int cd_movimento, int cd_empresa)
        {
            return BusinessFiscal.getEspelhoMovimento(cd_movimento, cd_empresa);
        }

        public List<Espelho> getSourceCopiaEspelhoMovimento(int cd_movimento, int cd_empresa)
        {
            List<Espelho> retorno = BusinessFiscal.getSourceCopiaEspelhoMovimento(cd_movimento, cd_empresa);
            while (retorno.Count <= 18)
            {
                Espelho espelho = new Espelho();
                espelho.cd_movimento = cd_movimento;
                retorno.Add(espelho);
            }
            return retorno;
        }

        public IEnumerable<ItemMovimento> getSourceItensMovimento(int cd_movimento, int cd_empresa)
        {
            return BusinessFiscal.getItensMovimento(cd_movimento, cd_empresa);
        }

        public IEnumerable<Titulo> getSourceTitulosMovimento(int cd_movimento, int cd_empresa)
        {
            return BusinessFinanceiro.getTitulosGridByMovimento(cd_movimento, cd_empresa, 0);
        }

        public List<DescontoTituloCarne> getTituloCarnePorContratoSubReport(int cdTitulo, int cdEscola, bool contaSegura)
        {
            return BusinessEscola.getTituloCarnePorContratoSubReport(cdTitulo, cdEscola, contaSegura);
        }

        public IEnumerable<RptPlanoTitulo> getPlanosContaPosicaoFinanceira(int cd_titulo)
        {
            return BusinessFinanceiro.getPlanosContaPosicaoFinanceira(cd_titulo);
        }

        public IEnumerable<ContaCorrente> getObservacoesCCBaixa(int? cd_baixa_titulo, int? cd_conta_corrente)
        {
            return BusinessFinanceiro.getObservacoesCCBaixa(cd_baixa_titulo, cd_conta_corrente);
        }

        public IEnumerable<KardexUI> st_Rptkardex(int cd_pessoa_escola, int cd_item, DateTime dt_ini, DateTime dt_fim, int cd_grupo, byte tipo, bool isApenasItensMovimento)
        {
            return BusinessFinanceiro.st_Rptkardex(cd_pessoa_escola, cd_item, dt_ini, dt_fim, cd_grupo, tipo, isApenasItensMovimento);
        }

        public IEnumerable<RptItemFechamento> rptItemWithSaldoItem(int cd_pessoa_escola, int cd_item, int cd_grupo, byte cd_tipo, int ano, int mes, bool isContagem)
        {
            return BusinessFinanceiro.rptItemWithSaldoItem(cd_pessoa_escola, cd_item, cd_grupo, cd_tipo, ano, mes, isContagem);
        }

        public IEnumerable<RptItemFechamento> rptItemSaldoBiblioteca(int cd_pessoa_escola, int cd_item, int cd_grupo, byte cd_tipo)
        {
            return BusinessFinanceiro.rptItemSaldoBiblioteca(cd_pessoa_escola, cd_item, cd_grupo, cd_tipo);
        }

        public IEnumerable<SaldoFinanceiro> rptSaldoFinanceiro(int cd_empresa, DateTime dta_base, byte tipoLocal, bool liquidacao)
        {
            return BusinessFinanceiro.getRelatorioSaldoFinanceiro(cd_empresa, dta_base, tipoLocal, liquidacao);
        }

        public IEnumerable<RptTitulosCnab> getSourceTituloRetornoCNAB(int cd_retorno_cnab)
        {
            var titulos = BusinessCnab.getTituloRetornoCNAB(cd_retorno_cnab);
            if (titulos.Count > 0)
            {
                return titulos.Where(x => x.Titulo != null).Select(x => new RptTitulosCnab()
                {
                    id_tipo_retorno = x.id_tipo_retorno,
                    dc_nosso_numero = x.dc_nosso_numero,
                    dt_emissao = x.Titulo.dt_emissao_titulo.ToString("dd/MM/yyyy"),
                    dt_vencimento = x.Titulo.dt_vcto_titulo.ToString("dd/MM/yyyy"),
                    nm_aluno = x.Titulo.Pessoa.no_pessoa,
                    no_responsavel = x.Titulo.PessoaResponsavel.no_pessoa,
                    nm_parcela = Convert.ToInt32(x.Titulo.nm_parcela_titulo),
                    nm_titulo = x.Titulo.nm_titulo.HasValue ? x.Titulo.nm_titulo.Value : 0,
                    dt_liquidacao_titulo = x.Titulo.dt_liquidacao_titulo != null ? x.Titulo.dt_liquidacao_titulo.Value.ToString("dd/MM/yyyy") : "",
                    Valor = x.Titulo.vl_titulo,
                    valorTotalDespesa = x.DespesaTituloCnab.Sum(d => d.vl_despesa),
                    vl_liquidacao_titulo = x.Titulo.vl_liquidacao_titulo,
                    vl_desconto_titulo = x.Titulo.vl_desconto_titulo,
                    vl_saldo_titulo = x.Titulo.vl_saldo_titulo
                }).ToList();
            }
            else
                return new List<RptTitulosCnab>();
        }

        public IEnumerable<CarneUI> getCarnePorContrato(int cdContrato, int cdEscola, bool contaSegura)
        {
            Parametro parametro = BusinessEscola.getParametrosBaixa(cdEscola);
            return BusinessCoordenacao.getCarnePorContrato(cdContrato, cdEscola, parametro, contaSegura, 0, 0);
        }
        public IEnumerable<ContaCorrenteUI> getFechamentoCaixaLocalMovto(DateTime dta_fechamento, int tipoLiquidacao, int cdUsuario, int cdEscola, byte tipoLocal, bool mostrarZerados)
        {
            return BusinessFinanceiro.getFechamentoCaixaLocalMovto(cdEscola, dta_fechamento, tipoLiquidacao, cdUsuario, tipoLocal, mostrarZerados);
        }
        public IEnumerable<ContaCorrenteUI> getFechamentoCaixaLocalMovtoRel(int cd_pessoa_escola, DateTime dta_fechamento, int cdUsuario, byte tipoLocal, bool mostrarZerados)
        {
            return BusinessFinanceiro.getFechamentoCaixaLocalMovtoRel(cd_pessoa_escola, dta_fechamento, cdUsuario, tipoLocal, mostrarZerados);
        }
        public ObsSaldoCaixa getObsSaldoCaixaConsolidado(int cdEscola, DateTime dt_saldo_caixa, int cd_usuario)
        {
            return BusinessFinanceiro.getObsSaldoCaixaConsolidado(cdEscola, dt_saldo_caixa, cd_usuario);
        }
        public IEnumerable<ContaCorrenteUI> getFechamentoCaixaTpLiquidacao(DateTime dta_fechamento, int cdUsuario, int cdEscola, byte tipoLocal, bool mostrarZerados)
        {
            return BusinessFinanceiro.getFechamentoCaixaTpLiquidacao(cdEscola, dta_fechamento, cdUsuario, tipoLocal, mostrarZerados);
        }
        public IEnumerable<CarneUI> getCarnePorMovimentos(int cdMovimento, int cdEscola, bool contaSegura)
        {
            Parametro parametro = BusinessEscola.getParametrosBaixa(cdEscola);
            return BusinessCoordenacao.getCarnePorMovimentos(cdMovimento, cdEscola, parametro, contaSegura);
        }

        public IEnumerable<RptTitulosCnab> getSourceTituloCNAB(int cd_cnab)
        {
            var titulos = BusinessCnab.getTituloCNAB(cd_cnab);
            if (titulos.Count > 0)
            {
                return titulos.Where(x => x.Titulo != null).Select(x => new RptTitulosCnab()
                {
                    dc_nosso_numero = x.dc_nosso_numero_titulo,
                    dt_emissao = x.Titulo != null ? x.Titulo.dt_emissao_titulo.ToString("dd/MM/yyyy") : "",
                    dt_vencimento = x.Titulo != null ? x.Titulo.dt_vcto_titulo.ToString("dd/MM/yyyy") : "",
                    nm_aluno = x.Titulo != null ? x.Titulo.Pessoa.no_pessoa : "",
                    no_responsavel = x.Titulo != null ? x.Titulo.PessoaResponsavel.no_pessoa : "",
                    nm_parcela = x.Titulo != null ? Convert.ToInt32(x.Titulo.nm_parcela_titulo) : 0,
                    nm_titulo = x.Titulo != null && x.Titulo.nm_titulo.HasValue ? x.Titulo.nm_titulo.Value : 0,
                    Valor = x.Titulo != null ? x.Titulo.vl_titulo : 0,
                    id_status_titulo_cnab = x.id_status_cnab_titulo,
                    vl_desconto_1 = x.DescontoTituloCNAB != null && x.DescontoTituloCNAB.Count() > 0 ? x.DescontoTituloCNAB.FirstOrDefault().vl_desconto : 0,
                    vl_desconto_2 = x.DescontoTituloCNAB != null && x.DescontoTituloCNAB.Count() > 0 && x.Titulo.vl_titulo > 0 ? (x.Titulo.vl_titulo - x.DescontoTituloCNAB.FirstOrDefault().vl_desconto) : x.Titulo.vl_titulo,
                    no_local_movto = x.Titulo.LocalMovto.no_local_movto,
                    no_arquivo_remessa = x.Cnab.no_arquivo_remessa,
                    no_turma = x.no_turma_titulo
                }).ToList();
            }
            else
                return new List<RptTitulosCnab>();
        }

        public IEnumerable<ReportProspect> getRptProspectAtendido(int cd_escola, int cdMotivoNaoMatricula, int cFuncionario, int cdProduto, DateTime? pDtaI, DateTime? pDtaF, int cd_midia, List<int> periodos, int cd_faixa_etaria)
        {
            return BusinessSecretaria.getProspectAtendido(cd_escola, cdMotivoNaoMatricula, cFuncionario, cdProduto, pDtaI, pDtaF, cd_midia, periodos, cd_faixa_etaria);
        }

        public IEnumerable<AlunoEventoReport> getRelatorioEventos(int cd_escola, int? cd_turma, int? cd_professor, int? cd_evento, int? qtd_faltas, bool falta_consecultiva, bool mais_turma_pagina,
                DateTime? dt_inicial, DateTime? dt_final)
        {
            return BusinessCoordenacao.getRelatorioEventos(cd_escola, cd_turma, cd_professor, cd_evento, qtd_faltas, falta_consecultiva, mais_turma_pagina, dt_inicial, dt_final);
        }

        public IEnumerable<DiarioAulaProgramcoesReport> getRelatorioDiarioAulaProgramacoes(int cd_escola, int? cd_turma, bool mais_turma_pagina, DateTime? dt_inicial, DateTime? dt_final, bool lancada)
        {
            return BusinessCoordenacao.getRelatorioDiarioAulaProgramacoes(cd_escola, cd_turma, mais_turma_pagina, dt_inicial, dt_final, lancada);
        }

        public IEnumerable<ReportProspect> getProspectAtendidoMatricula(int cd_escola, int cdMotivoNaoMatricula, int cFuncionario, int cdProduto, DateTime? pDtaI, DateTime? pDtaF, int cd_midia, List<int> periodos)
        {
            return BusinessSecretaria.getProspectAtendidoMatricula(cd_escola, cdMotivoNaoMatricula, cFuncionario, cdProduto, pDtaI, pDtaF, cd_midia, periodos);
        }
        public IEnumerable<MatriculaRel> getMatriculaAnalitico(int cd_empresa, int cd_turma, int cd_aluno, string situacaoAlunoTurma, bool semTurma, bool tranferencia, bool retorno, int situacaoContrato, DateTime? dtIni, DateTime? dtFim, int cdAtendente, bool bolsaCem, bool contratoDigitalizado, int? cdProduto, string noProduto, bool exibirEnderecos, int vinculado)
        {
            string[] situacao = situacaoAlunoTurma.Split('|');
            List<int> cdsSituacoes = new List<int>();
            for (int i = 0; i < situacao.Count(); i++)
                cdsSituacoes.Add(Int32.Parse(situacao[i]));
            return BusinessMatricula.getMatriculaAnalitico(cd_empresa, cd_turma, cd_aluno, cdsSituacoes, semTurma, tranferencia, retorno, situacaoContrato, dtIni, dtFim, cdAtendente, bolsaCem, contratoDigitalizado, cdProduto, noProduto, exibirEnderecos, vinculado);
        }
        public IEnumerable<MatriculaRel> getMatriculaPorMotivo(int cd_empresa, int cd_turma, int cd_aluno, string situacaoAlunoTurma, bool semTurma, bool tranferencia, bool retorno, int situacaoContrato, DateTime? dtIni, DateTime? dtFim, int vinculado)
        {
            string[] situacao = situacaoAlunoTurma.Split('|');
            List<int> cdsSituacoes = new List<int>();
            for (int i = 0; i < situacao.Count(); i++)
                cdsSituacoes.Add(Int32.Parse(situacao[i]));
            return BusinessMatricula.getMatriculaPorMotivo(cd_empresa, cd_turma, cd_aluno, cdsSituacoes, semTurma, tranferencia, retorno, situacaoContrato, dtIni, dtFim, vinculado);
        }
        public DataTable getMatriculaOutros(int cd_escola, int cd_produto, DateTime? dta_ini, DateTime? dta_fim, byte qtd_max)
        {
            return BusinessMatricula.getMatriculaOutros(cd_escola,  cd_produto, dta_ini, dta_fim, qtd_max);
        }
        public DataTable getLoginEscola(DateTime dt_analise, bool id_login, byte id_matricula)
        {
            return BusinessEscola.getLoginEscola(dt_analise, id_login, id_matricula);
        }

        public IEnumerable<ReportDiarioAula> getRelatorioDiarioAula(int cd_escola, int cd_turma, int cd_professor, DateTime dt_inicial, DateTime dt_final)
        {
            IEnumerable<ReportDiarioAula> retorno = BusinessTurma.getRelatorioDiarioAula(cd_escola, cd_turma, cd_professor, dt_inicial, dt_final);
            if (retorno == null || retorno.Count() <= 0)
                throw new BusinessException(Messages.msgRegNotEnc, null, false);

            return retorno;
        }

        public IEnumerable<ProgramacaoTurma> getProgramacoesTurmaVisto(int cd_escola, int cd_turma, int cd_professor, DateTime? dt_inicial, DateTime dt_final)
        {
            return BusinessTurma.getProgramacoesTurma(cd_escola, cd_turma, cd_professor, dt_inicial, dt_final);
        }

        public IEnumerable<ReportProgramacaoTurmaAluno> getProgramacoesTurma(int cd_escola, int cd_turma, int cd_professor, DateTime? dt_inicial, DateTime dt_final,
            bool infoPresenca, AlunoTurma.FiltroSituacaoAlunoTurma situacao_aluno = AlunoTurma.FiltroSituacaoAlunoTurma.Todos)
        {
            List<ReportProgramacaoTurmaAluno> retorno = new List<ReportProgramacaoTurmaAluno>();

            //Foi eliminado pois está fazendo só a turma pai
            /*ProgramacaoTurmaAbaUI programacaoAba = BusinessTurma.verificaProgramacaoTurma(cd_turma, cd_escola);
            if (programacaoAba == null || (programacaoAba != null && programacaoAba.Programacoes == null) || (programacaoAba != null && programacaoAba.Programacoes != null && programacaoAba.Programacoes.Count == 0))
            {
                return retorno;
            }*/
            IEnumerable<Aluno> listaAluno = BusinessAluno.getAlunosTurmaAtivosDiarioAula(cd_escola, cd_turma, dt_inicial, dt_final, situacao_aluno);
            IEnumerable<ProgramacaoTurma> listaProgramacoes = BusinessTurma.getProgramacoesTurma(cd_escola, cd_turma, cd_professor, dt_inicial, dt_final);

            if (listaAluno != null && listaAluno.Count() > 0)
                if (listaProgramacoes != null && listaProgramacoes.Count() > 0)
                    foreach (Aluno aluno in listaAluno)
                    foreach (ProgramacaoTurma programacao in listaProgramacoes)
                    {
                        Aluno alunoInTurma = BusinessCoordenacao.getAlunoIsTurmaInDate(cd_turma, aluno.cd_aluno, cd_escola, programacao.dta_programacao_turma);

                        AlunoEvento evento = BusinessCoordenacao.getEventosRtpDiarioAula(cd_escola, aluno.cd_aluno, cd_professor, programacao.dta_programacao_turma);
                        string eventoSigla = "";
                        if (infoPresenca == true && alunoInTurma != null && programacao.id_aula_dada == true && programacao.id_prog_cancelada == false)
                        {
                            if (evento == null)
                            {
                                eventoSigla = "P";
                            }else if (evento.cd_evento == 1)
                            {
                                eventoSigla = "F";
                            }else if (evento.cd_evento == 2)
                            {
                                eventoSigla = "F/J";
                            }

                        }
                        retorno.Add(new ReportProgramacaoTurmaAluno()
                        {
                            cd_aluno = aluno.cd_aluno,
                            no_aluno = aluno.nomeAluno,
                            dia_mes = programacao.dia_mes,
                            dia_mes_hor_min = programacao.dia_mes_hor_min,
                            dt_programacao = programacao.dta_programacao_turma,
                            is_turma_regular = programacao.is_turma_regular,
                            lancada = programacao.id_aula_dada,
                            cancelada = programacao.id_prog_cancelada,
                            evento = eventoSigla

                        });
                    }
                            


            return retorno;
        }

        public IEnumerable<Aluno> getAlunosTurmaAtivosDiarioAulaReport(int cd_turma, int cd_pessoa_escola, DateTime? dt_inicial, DateTime dt_final)
        {
            return BusinessAluno.getAlunosTurmaAtivosDiarioAulaReport(cd_pessoa_escola, cd_turma, dt_inicial, dt_final);
        }

        public IEnumerable<TurmaSearch> getRptTurmas(int tipoTurma, int cdCurso, int cdDuracao, int cdProduto, int cdProfessor, int prog, bool turmasFilhas,
           DateTime? pDtaI, DateTime? pDtaF, DateTime? pDtaFimI, DateTime? pDtaFimF, int cd_escola, int situacaoTurma, string situacaoAlunoTurma, int tipoOnline, string dias)
        {
            string[] situacao = situacaoAlunoTurma.Split('|');
            List<int> cdsSituacoes = new List<int>();
            for (int i = 0; i < situacao.Count(); i++)
                cdsSituacoes.Add(Int32.Parse(situacao[i]));

            return BusinessTurma.getRptTurmas(tipoTurma, cdCurso, cdDuracao, cdProduto, cdProfessor, prog, turmasFilhas, pDtaI, pDtaF, pDtaFimI, pDtaFimF, cd_escola, situacaoTurma, cdsSituacoes, tipoOnline, dias);
        }

        public IEnumerable<TurmaSearch> getRptTurmasAEncerrar(int tipoTurma, int cdCurso, int cdDuracao, int cdProduto, int cdProfessor, int tipoProg, bool turmasFilhas,
           DateTime? pDtaI, DateTime? pDtaF, DateTime? pDtaFimI, DateTime? pDtaFimF, int cd_escola, int tipoOnline, string dias)
        {
            return BusinessTurma.getRptTurmasAEncerrar(tipoTurma, cdCurso, cdDuracao, cdProduto, cdProfessor, tipoProg, turmasFilhas, pDtaI, pDtaF, pDtaFimI, pDtaFimF, cd_escola, tipoOnline, dias);
        }

        public IEnumerable<TurmaSearch> getRptTurmasNovas(int tipoTurma, int cdCurso, int cdDuracao, int cdProduto, int cdProfessor, int tipoProg, bool turmasFilhas,
           DateTime? pDtaI, DateTime? pDtaF, int cd_escola, int tipoOnline, string dias)
        {
            return BusinessTurma.getRptTurmasNovas(tipoTurma, cdCurso, cdDuracao, cdProduto, cdProfessor, tipoProg, turmasFilhas, pDtaI, pDtaF, cd_escola, tipoOnline, dias);
        }

        public IEnumerable<AlunoSearchUI> getRptAlunosTurma(int cd_turma, bool id_turma_ppt, string cd_situacao_aluno_turma, int cdEscolaAluno)
        {
            string[] situacao = cd_situacao_aluno_turma.Split('|');
            List<int> cdsSituacoes = new List<int>();
            for (int i = 0; i < situacao.Count(); i++)
                cdsSituacoes.Add(Int32.Parse(situacao[i]));
            return BusinessAluno.getRptAlunosTurma(cd_turma, id_turma_ppt, cdsSituacoes, cdEscolaAluno);
        }

        public IEnumerable<AlunoSearchUI> getRptAlunosTurmaEncerrar(int cd_turma, bool id_turma_ppt, DateTime dtaIniAula, DateTime dtaFim, int cdEscolaAluno)
        {
            
            return BusinessAluno.getRptAlunosTurmaEncerrar(cd_turma, id_turma_ppt, dtaIniAula,  dtaFim, cdEscolaAluno);
        }

        public IEnumerable<AlunoSearchUI> getRptAlunosTurmaPPTEncerrar(int cd_turma, bool id_turma_ppt, DateTime dtaIniAula, DateTime dtaFim, int cdEscolaAluno)
        {

            return BusinessAluno.getRptAlunosTurmaPPTEncerrar(cd_turma, id_turma_ppt, dtaIniAula, dtaFim, cdEscolaAluno);
        }

        public IEnumerable<AlunoSearchUI> getRptAlunosTurmaNova(int cd_turma, bool id_turma_ppt, DateTime dtaIniAula, Nullable<DateTime> dtaFim, int cdEscolaAluno)
        {

            return BusinessAluno.getRptAlunosTurmaNova(cd_turma, id_turma_ppt, dtaIniAula, dtaFim, cdEscolaAluno);
        }

        public IEnumerable<AlunoSearchUI> getRptAlunosTurmaPPTNova(int cd_turma, bool id_turma_ppt, DateTime dtaIniAula, DateTime dtaFim, int cdEscolaAluno)
        {

            return BusinessAluno.getRptAlunosTurmaPPTNova(cd_turma, id_turma_ppt, dtaIniAula, dtaFim, cdEscolaAluno);
        }

        public IEnumerable<AlunoSearchUI> getRptAlunosTurmaEncerrar(List<int> cdTurmas,DateTime? pDtaFimI, DateTime? pDtaFimF, int cd_escola, int tipoTurma)
        {
            return BusinessAluno.getRptAlunosTurmaEncerrar(cdTurmas,pDtaFimI, pDtaFimF, cd_escola, tipoTurma);
        }

        public IEnumerable<AlunoSearchUI> getRptAlunosTurmasNovas(List<int> cdTurmas, DateTime? pDtaI, DateTime? pDtaF, int cd_escola, int tipoTurma)
        {
            return BusinessAluno.getRptAlunosTurmasNovas(cdTurmas, pDtaI, pDtaF, cd_escola, tipoTurma);
        }

        public IEnumerable<TurmaSearch> getRptTurmasProgAula(int cd_turma, int cd_escola, DateTime? pDtaI, DateTime? pDtaF)
        {
            return BusinessTurma.getRptTurmasProgAula(cd_turma, cd_escola, pDtaI, pDtaF);
        }

        public IEnumerable<ReportTurmaMatriculaMaterial> getRptTurmasMatriculaMaterial(int cd_escola, int cd_turma, int cd_aluno, int cd_item, int nm_contrato, DateTime? pDtaI, DateTime? pDtaF)
        {
            return BusinessTurma.getRptTurmasMatriculaMaterial(cd_escola, cd_turma, cd_aluno, cd_item, nm_contrato, pDtaI, pDtaF);
        }

        public IEnumerable<ProgramacaoTurma> getSubRptProgAulasTurma(int cd_turma, int cd_escola, ProgramacaoTurmaDataAccess.TipoConsultaProgTurmaEnum tipoConsulta, bool idMostrarFeriado)
        {
            return BusinessTurma.getProgramacaoTurmaByTurma(TransactionScopeBuilder.TransactionType.UNCOMMITED, cd_turma, cd_escola, tipoConsulta, idMostrarFeriado);
        }

        public IEnumerable<ReportProspect> getComparativoProspectAtendido(int cd_escola, int cdMotivoNaoMatricula, int cFuncionario, int cdProduto, DateTime? pDtaI, DateTime? pDtaF, int cd_midia, List<int> periodos)
        {
            return BusinessSecretaria.getComparativoProspectAtendido(cd_escola, cdMotivoNaoMatricula, cFuncionario, cdProduto, pDtaI, pDtaF, cd_midia, periodos);
        }

        public List<PessoaSGF> getListaAniversariantes(int cd_escola, int tipo, int cd_turma, int mes, int dia)
        {
            return BusinessAluno.getListaAniversariantes(cd_escola, tipo, cd_turma, mes, dia);
        }

        public List<ChequeUI> getRptChequesAbertos(int cd_empresa, int cd_pessoa_aluno, int cd_banco, string emitente, bool liquidados, string nm_cheque, string vl_titulo, int nm_agencia,
            int nm_ccorrente, DateTime? dt_ini_bPara, DateTime? dt_fim_bPara, DateTime? dt_ini, DateTime? dt_fim, bool emissao, bool liquidacao, byte natureza)
        {
            return BusinessFinanceiro.getRptChequesAbertos(cd_empresa, cd_pessoa_aluno, cd_banco, emitente, liquidados, nm_cheque, vl_titulo, nm_agencia, nm_ccorrente, dt_ini_bPara,
                dt_fim_bPara, dt_ini, dt_fim, emissao, liquidacao, natureza).ToList();
        }

        public List<ChequeUI> getRptChequesLiquidados(int cd_empresa, int cd_pessoa_aluno, int cd_banco, string emitente, bool liquidados, string nm_cheque, string vl_titulo, int nm_agencia,
            int nm_ccorrente, DateTime? dt_ini_bPara, DateTime? dt_fim_bPara, DateTime? dt_ini, DateTime? dt_fim, bool emissao, bool liquidacao, byte natureza)
        {
            return BusinessFinanceiro.getRptChequesLiquidados(cd_empresa, cd_pessoa_aluno, cd_banco, emitente, liquidados, nm_cheque, vl_titulo, nm_agencia, nm_ccorrente, dt_ini_bPara,
                dt_fim_bPara, dt_ini, dt_fim, emissao, liquidacao, natureza).ToList();
        }

        public IEnumerable<RptBolsistas> getBolsistas(int cdEscola, int cd_aluno, int cd_turma, bool cancelamento, decimal? per_bolsa, int cd_motivo_bolsa, DateTime? dtIniComunicado,
                                                        DateTime? dtFimComunicado, DateTime? dtIni, DateTime? dtFim, bool periodo_ini, bool periodo_cancel)
        {
            List<RptBolsistas> bolsistasAval = new List<RptBolsistas>();
            IEnumerable<RptBolsistas> bolsistas = BusinessAluno.getBolsistas(cdEscola, cd_aluno, cd_turma, cancelamento, per_bolsa, cd_motivo_bolsa, dtIniComunicado, dtFimComunicado, dtIni, dtFim, periodo_ini, periodo_cancel);
            foreach (RptBolsistas rpt in bolsistas)
            {

                IEnumerable<RptBolsistasAval> avaliacoes = BusinessTurma.getAvaliacoesBolsista(rpt.cd_aluno, cdEscola, cd_turma);
                foreach (RptBolsistasAval rptAval in avaliacoes)
                {

                    RptBolsistas bol = new RptBolsistas
                    {
                        cd_turma = rptAval.cd_turma,
                        dc_tipo_avaliacao1 = rptAval.dc_tipo_avaliacao1,
                        dc_tipo_avaliacao2 = rptAval.dc_tipo_avaliacao2,
                        dc_tipo_avaliacao3 = rptAval.dc_tipo_avaliacao3,
                        nm_nota_aluno1 = rptAval.nm_nota_aluno1,
                        nm_nota_aluno2 = rptAval.nm_nota_aluno2,
                        nm_nota_aluno3 = rptAval.nm_nota_aluno3,
                        no_turma = rptAval.no_turma,
                        dt_ini_turma = rptAval.dt_ini_turma,
                        nr_faltas = rptAval.nr_faltas,
                        nm_nf = rptAval.nm_nf,
                        vl_parcela = rptAval.vl_parcela,
                        dt_transferencia = rptAval.dt_transferencia,
                        tx_obs_transferencia = rptAval.tx_obs_transferencia,
                        no_produto = rptAval.no_produto
                    };
                    rpt.vl_parcela = rptAval.vl_parcela;
                    bol.copy(rpt);
                    bolsistasAval.Add(bol);
                }
            }
            IEnumerable<RptBolsistas> bolSemTurma = bolsistas.Where(b => !bolsistasAval.Where(t => t.cd_aluno == b.cd_aluno).Any());
            foreach (RptBolsistas b in bolSemTurma)
                bolsistasAval.Add(b);

            return bolsistasAval;
        }

        public List<ReportControleSala> getRptControleSala(TimeSpan? hIni, TimeSpan? hFim, int cd_turma, int cd_professor, int cd_sala, List<int> diasSemana, int cd_escola)
        {
            TimeSpan hIniP = new TimeSpan(06, 00, 00);
            TimeSpan hFImP = new TimeSpan(23, 00, 00);
            if (hIni.HasValue)
                hIniP = (TimeSpan)hIni;
            if (hFim.HasValue)
                hFImP = (TimeSpan)hFim;
            List<ReportControleSala> listaProfesssoresSala = BusinessCoordenacao.getHorariosRptControleSala(hIniP, hFImP, cd_turma, cd_professor, cd_sala, diasSemana, cd_escola);
            List<Sala> salasEscola = BusinessCoordenacao.getSalas(cd_sala > 0 ? cd_sala : 0, cd_escola, SalaDataAccess.TipoConsultaDuracaoEnum.HAS_ATIVO).ToList();
            List<ReportControleSala> listaControleSalas = new List<ReportControleSala>();
            while (hIniP <= hFImP)
            {
                if (salasEscola != null)
                {
                    foreach (Sala s in salasEscola)
                        listaControleSalas.Add(new ReportControleSala
                        {

                            nm_hora = hIniP.Hours * 60 + hIniP.Minutes,
                            cd_sala = s.cd_sala,
                            no_sala = s.no_sala,
                            horaMin = hIniP
                        });
                }
                hIniP = new TimeSpan(hIniP.Hours, hIniP.Minutes + 5, 00);
            }
            if (listaProfesssoresSala != null && listaControleSalas != null)
                foreach (ReportControleSala lp in listaProfesssoresSala)
                {
                    //ReportControleSala sala = listaControleSalas.Where(x => x.cd_sala == lp.cd_sala && (x.nm_hora >= lp.hora_ini.Hours && x.nm_hora <= lp.hora_fim.Hours)).FirstOrDefault();
                    foreach (var ctrlSalas in listaControleSalas)
                    {
                        //(hora_ini * 60) >= ((nm_hora * 60) + 5) && (hora_inicial * 60) <= ((nm_hora + 1) * 60) || (hora_final * 60) >= ((nm_hora * 60) + 5) && (hora_final * 60) <= ((nm_hora + 1) * 60)
                        //ctrlSalas.nm_hora >= lp.hora_ini.Hours && ctrlSalas.nm_hora <= (lp.hora_fim.Hours > lp.hora_ini.Hours ? (lp.hora_fim.Hours - 1) : lp.hora_fim.Hours)
                        //if ((ctrlSalas.cd_sala == lp.cd_sala && ((lp.hora_ini.Hours * 60) + lp.hora_ini.Minutes + 5) >= ((ctrlSalas.nm_hora * 60) + 5) && ((lp.hora_ini.Hours * 60) + lp.hora_ini.Minutes + 5) <= (((ctrlSalas.nm_hora + 1) * 60) - 5)) || (((lp.hora_fim.Hours * 60) + lp.hora_fim.Minutes) >= ((ctrlSalas.nm_hora * 60) + 5) && ((lp.hora_fim.Hours * 60) + lp.hora_fim.Minutes) <= (((ctrlSalas.nm_hora + 1) * 60) - 5))) {
                        if ((ctrlSalas.cd_sala == lp.cd_sala && 
                                (
                                    ((lp.hora_ini.Hours * 60) + lp.hora_ini.Minutes) <= ctrlSalas.nm_hora && 
                                     ctrlSalas.nm_hora <= ((lp.hora_fim.Hours * 60) + lp.hora_fim.Minutes
                                     )
                                )
                                //((((lp.hora_ini.Hours * 60) + lp.hora_ini.Minutes) >= ctrlSalas.nm_hora && ((lp.hora_ini.Hours * 60) + lp.hora_ini.Minutes) <= ((ctrlSalas.nm_hora + 5))) || 
                                //(((lp.hora_fim.Hours * 60) + lp.hora_fim.Minutes) >= (ctrlSalas.nm_hora + 5) && ((lp.hora_fim.Hours * 60) + lp.hora_fim.Minutes) <= (((ctrlSalas.nm_hora + 5))))
                                //)
                            )
                            )
                        {
                            lp.nm_hora = ctrlSalas.nm_hora;
                            lp.horaMin = ctrlSalas.horaMin;
                            ctrlSalas.copy(lp);
                        }
                            
                    }
                    //if (sala != null)
                    //sala.copy(lp);
                }
            listaControleSalas = listaControleSalas.OrderBy(x => x.nm_hora).ToList();
            return listaControleSalas;
        }

        public List<ReportControleSala> getRptControleSalaCores(TimeSpan? hIni, TimeSpan? hFim, int cd_turma, int cd_professor, int cd_sala, List<int> diasSemana, int cd_escola)
        {
            TimeSpan hIniP = new TimeSpan(06, 00, 00);
            TimeSpan hFImP = new TimeSpan(23, 00, 00);
            if (hIni.HasValue)
                hIniP = (TimeSpan)hIni;
            if (hFim.HasValue)
                hFImP = (TimeSpan)hFim;
            List<ReportControleSala> listaProfesssoresSala = BusinessCoordenacao.getHorariosRptControleSala(hIniP, hFImP, cd_turma, cd_professor, cd_sala, diasSemana, cd_escola);
            List<Sala> salasEscola = BusinessCoordenacao.getSalas(cd_sala > 0 ? cd_sala : 0, cd_escola, SalaDataAccess.TipoConsultaDuracaoEnum.HAS_ATIVO).ToList();
            List<ReportControleSala> listaControleSalas = new List<ReportControleSala>();
            List<ReportControleSala> coresListaControleSalas = new List<ReportControleSala>();
            while (hIniP <= hFImP)
            {
                if (salasEscola != null)
                {
                    foreach (Sala s in salasEscola)
                        listaControleSalas.Add(new ReportControleSala
                        {

                            nm_hora = hIniP.Hours,
                            cd_sala = s.cd_sala,
                            no_sala = s.no_sala
                        });
                }
                hIniP = new TimeSpan(hIniP.Hours + 1, 00, 00);
            }
            List<string> cores = listaProfesssoresSala.Select(x => x.no_tipo_cor).Distinct().ToList();

            if (listaProfesssoresSala != null && listaControleSalas != null)
                foreach (ReportControleSala lp in listaProfesssoresSala)
                {
                    //ReportControleSala sala = listaControleSalas.Where(x => x.cd_sala == lp.cd_sala && (x.nm_hora >= lp.hora_ini.Hours && x.nm_hora <= lp.hora_fim.Hours)).FirstOrDefault();
                    foreach (var ctrlSalas in listaControleSalas)
                    {
                        if (ctrlSalas.cd_sala == lp.cd_sala && ctrlSalas.nm_hora >= lp.hora_ini.Hours && ctrlSalas.nm_hora <= (lp.hora_fim.Hours > lp.hora_ini.Hours  ? (lp.hora_fim.Hours - 1) : lp.hora_fim.Hours))
                        {
                            coresListaControleSalas.Add(lp);
                        }
                    }
                    //if (sala != null)
                    //sala.copy(lp);
                }
            listaControleSalas = coresListaControleSalas.OrderBy(x => x.nm_hora).ToList();
            return listaControleSalas;
        }

        public IEnumerable<RptListagemEndereco> getRptListagemEnderecos(int cd_mala_direta, int cd_empresa, string no_pessoa, int status, string email, byte id_tipo_cadastro)
        {
            return BusinessMarketing.getRptListagemEnderecos(cd_mala_direta, cd_empresa, no_pessoa, status, email, id_tipo_cadastro);
        }

        public List<ItemMovimento> getItensMovimentoRecibo(int cd_movimento, int cd_empresa)
        {
            List<ItemMovimento> retorno = BusinessFiscal.getItensMovimentoReciboLeitura(cd_movimento, cd_empresa).ToList();
            while (retorno.Count <= 11)
            {
                ItemMovimento itemMovimento = new ItemMovimento();
                itemMovimento.cd_movimento = cd_movimento;
                retorno.Add(itemMovimento);
            }
            return retorno;
        }

        public IEnumerable<ReportPagamentoProfessor> getRptPagamentoProfessores(int cd_tipo_relatorio, int cd_empresa, int cd_professor, DateTime? dt_ini, DateTime? dt_fim)
        {
            return BusinessProfessor.getRptPagamentoProfessores(cd_tipo_relatorio, cd_empresa, cd_professor, dt_ini, dt_fim);
        }

        public IEnumerable<ReportPagamentoProfessor> getRptPagamentoProfessoresFaltas(int cd_tipo_relatorio, int cd_empresa, int cd_professor, DateTime? dt_ini, DateTime? dt_fim)
        {
            return BusinessProfessor.getRptPagamentoProfessoresFaltas(cd_tipo_relatorio, cd_empresa, cd_professor, dt_ini, dt_fim);
        }

        public IEnumerable<ReportPagamentoProfessor> getRptPagamentoProfessoresObs(int cd_tipo_relatorio, int cd_empresa, int cd_professor, DateTime? dt_ini, DateTime? dt_fim)
        {
            return BusinessProfessor.getRptPagamentoProfessoresObs(cd_tipo_relatorio, cd_empresa, cd_professor, dt_ini, dt_fim);
        }

        public DataTable getRptMediaAlunos(int cd_escola, int cd_turma, int tipoTurma, int cdCurso, int cdProduto, int pesOpcao, int pesTipoAluno,
                decimal? vl_media, DateTime dtInicial, DateTime dtFinal)
        {
            return BusinessAluno.getRptMediaAlunos(cd_escola, cd_turma, tipoTurma, cdCurso, cdProduto, pesOpcao, pesTipoAluno, vl_media, dtInicial, dtFinal);
        }

        public IEnumerable<AulaPersonalizadaReport> getReportAulaPersonalizada(int cd_empresa, int cd_aluno, int? cd_produto, int? cd_curso, DateTime? dt_inicial_agend, DateTime? dt_final_agend,
                DateTime? dt_inicial_lanc, DateTime? dt_final_lanc, TimeSpan? hr_inicial_agend, TimeSpan? hr_final_agend, TimeSpan? hr_inicial_lanc, TimeSpan? hr_final_lanc)
        {
            return BusinessCoordenacao.getReportAulaPersonalizada(cd_empresa, cd_aluno, cd_produto, cd_curso, dt_inicial_agend, dt_final_agend, dt_inicial_lanc,
                dt_final_lanc, hr_inicial_agend, hr_final_agend, hr_inicial_lanc, hr_final_lanc);
        }

        public bool retirarEmailListaEndereco(int cd_empresa, int cd_cadastro, int id_cadastro)
        {
            return BusinessMarketing.retirarEmailListaEndereco(cd_empresa, cd_cadastro, id_cadastro);
        }
        public List<ReportPercentualTerminoEstagio> getRptPercentualTerminoEstagio(int cd_professor, DateTime? dt_ini, DateTime? dt_fim, int cd_escola)
        {
            return BusinessTurma.getRptPercentualTerminoEstagio(cd_professor, dt_ini, dt_fim, cd_escola);
        }

        public IEnumerable<ReportPercentualTerminoEstagio> getRptAlunosProximoCurso(int cd_escola, int cd_turma)
        {
            return BusinessAluno.getRptAlunosProximoCurso(cd_escola, cd_turma);
        }

        public IEnumerable<FuncionarioComissao> getRptComissaoSecretarias(int cd_funcionario, int cd_produto, int cd_empresa, DateTime? dt_ini, DateTime? dt_fim)
        {
            return BusinessProfessor.getRptComissaoSecretarias(cd_funcionario, cd_produto, cd_empresa, dt_ini, dt_fim);
        }

        public IEnumerable<RptContVendasMaterial> getRptContVendasMaterial(int cd_escola, int cd_aluno, int cd_item, DateTime dt_inicial, DateTime dt_final, int cd_turma, bool semmaterial)
        {
            return BusinessFinanceiro.getRptContVendasMaterial(cd_escola, cd_aluno, cd_item, dt_inicial, dt_final, cd_turma, semmaterial);
        }
    }
}

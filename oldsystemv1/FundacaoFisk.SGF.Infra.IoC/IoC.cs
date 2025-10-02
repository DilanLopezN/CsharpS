using System.Web.Mvc;
using SimpleInjector;
using SimpleInjector.Integration.Web.Mvc;
using FundacaoFisk.SGF.Web.Services.Biblioteca.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Biblioteca.Business;
using FundacaoFisk.SGF.Web.Services.Biblioteca.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Biblioteca.DataAccess;
using FundacaoFisk.SGF.Web.Services.EmailMarketing.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.EmailMarketing.Business;
using FundacaoFisk.SGF.Web.Services.EmailMarketing.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Service.EmailMarketing.DataAccess;
using FundacaoFisk.SGF.Web.Services.Auth.Comum;
using FundacaoFisk.SGF.Web.Services.Auth.Business;
using FundacaoFisk.SGF.Web.Services.CNAB.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.CNAB.DataAccess;
using FundacaoFisk.SGF.Web.Services.CNAB.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.CNAB.Business;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Business;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess;
using FundacaoFisk.SGF.Web.Services.Empresa.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Empresa.DataAccess;
using FundacaoFisk.SGF.Web.Services.Empresa.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Empresa.Business;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Financeiro.Business;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess;
using FundacaoFisk.SGF.Web.Services.InstituicaoEnsino.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.InstituicaoEnsino.DataAccess;
using FundacaoFisk.SGF.Web.Services.Log.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Log.DataAccess;
using FundacaoFisk.SGF.Web.Services.Log.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Pessoa.DataAccess;
using FundacaoFisk.SGF.Web.Services.Pessoa.Comum.DataAccess;
using FundacaoFisk.SGF.Web.Services.Log.Business;
using FundacaoFisk.SGF.Web.Services.Pessoa.Business;
using FundacaoFisk.SGF.Web.Services.Pessoa.Comum.Business;
using FundacaoFisk.SGF.Web.Services.Secretaria.Comum;
using FundacaoFisk.SGF.Web.Services.Secretaria.Business;
using FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess;
using FundacaoFisk.SGF.Web.Services.Secretaria.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Secretaria.Comum.IDataAcess;
using FundacaoFisk.SGF.Web.Services.Usuario.Comum;
using FundacaoFisk.SGF.Web.Services.Usuario.DataAccess;
using FundacaoFisk.SGF.Web.Services.Usuario.Business;
using FundacaoFisk.SGF.Web.Services.InstituicaoEnsino.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.InstituicaoEnsino.Business;
using System;
using Componentes.Utils;
using System.Web.Http;
using Componentes.GenericDataAccess.Comum;
using FundacaoFisk.SGF.GenericModel;
using SimpleInjector.Integration.Web;
using Microsoft.Practices.ServiceLocation;
using FundacaoFisk.SGF.Services.Coordenacao.DataAccess;
using FundacaoFisk.SGF.Services.Coordenacao.Comum.IDataAccess;
using FundacaoFisk.SGF.Services.CNAB.DataAccess;
using FundacaoFisk.SGF.Services.CNAB.Comum.IDataAccess;
using FundacaoFisk.SGF.Services.Coordenacao.Business;
using FundacaoFisk.SGF.Services.InstituicaoEnsino.DataAccess;

namespace FundacaoFisk.SGF.Infra.IoC
{
    public static class IoC
    {
        public static void Start(Container container)
        {
            if (container == null)
                container = new Container();
            //GenericRepository
            //container.RegisterPerWebRequest<SGFWebContext>();
            //container.Register<SGFWebContext>(() => new SGFWebContext(), new WebRequestLifestyle());
            //container.RegisterPerWebRequest<SGFWebContext>();
            //container.Register<SGFWebContext, SGFWebContext>(new WebRequestLifestyle());
            container.Register<IBibliotecaBusiness, BibliotecaBusiness>(new WebRequestLifestyle());
            container.Register<IEmprestimoDataAccess, EmprestimoDataAccess>(new WebRequestLifestyle());
            container.Register<IEmailMarketingBusiness, EmailMarketingBusiness>(new WebRequestLifestyle());
            container.Register<IListaEnderecoMalaDataAccess, ListaEnderecoMalaDataAccess>(new WebRequestLifestyle());
            container.Register<IListaNaoInscritoDataAccess, ListaNaoInscritoDataAccess>(new WebRequestLifestyle());
            container.Register<IMalaDiretaDataAccess, MalaDiretaDataAccess>(new WebRequestLifestyle());
            container.Register<IAuthBusiness, AuthBusiness>(new WebRequestLifestyle());
            container.Register<IAesCryptoHelper, AesCryptoHelper>(new WebRequestLifestyle());
            container.Register<ICnabDataAccess, CnabDataAccess>(new WebRequestLifestyle());
            container.Register<ITituloCnabDataAccess, TituloCnabDataAccess>(new WebRequestLifestyle());
            container.Register<IBoletoBusiness, BoletoBusiness>(new WebRequestLifestyle());
            container.Register<ICnabBusiness, CnabBusiness>(new WebRequestLifestyle());
            container.Register<ICarteiraCnabDataAccess, CarteiraCnabDataAccess>(new WebRequestLifestyle());
            container.Register<IRetornoCNABDataAccess, RetornoCNABDataAccess>(new WebRequestLifestyle());
            container.Register<ITituloRetornoCnabDataAccess, TituloRetornoCNABDataAccess>(new WebRequestLifestyle());
            container.Register<IFichaSaudeDataAccess, FichaSaudeDataAccess>(new WebRequestLifestyle());

            // Business
            container.Register<ICoordenacaoBusiness, CoordenacaoBusiness>(new WebRequestLifestyle());
            container.Register<ICursoBusiness, CursoBusiness>(new WebRequestLifestyle());
            container.Register<IProfessorBusiness, ProfessorBusiness>(new WebRequestLifestyle());
            container.Register<ITurmaBusiness, TurmaBusiness>(new WebRequestLifestyle());
            container.Register<IApiNewCyberBusiness, ApiNewCyberBusiness>(new WebRequestLifestyle());
            container.Register<IApiNewCyberAlunoBusiness, ApiNewCyberAlunoBusiness>(new WebRequestLifestyle());
            container.Register<IApiNewCyberFuncionarioBusiness, ApiNewCyberFuncionarioBusiness>(new WebRequestLifestyle());
            container.Register<IApiPromocaoIntercambioBusiness, ApiPromocaoIntercambioBusiness>(new WebRequestLifestyle());
            container.Register<IApiPromocaoIntercambioProspectBussiness, ApiPromocaoIntercambioProspectBusiness>(new WebRequestLifestyle());
            container.Register<IPessoaPromocaoDataAccess, PessoaPromocaoDataAccess>(new WebRequestLifestyle());
            container.Register<IEmpresaValorServicoDataAccess, EmpresaValorServicoDataAccess>(new WebRequestLifestyle());


            //DataAccess
            container.Register<IProdutoFuncionarioDataAccess, ProdutoFuncionarioDataAccess>(new WebRequestLifestyle());
            container.Register<IVideoDataAccess, VideoDataAccess>(new WebRequestLifestyle());
            container.Register<IFaqDataAccess, FaqDataAccess>(new WebRequestLifestyle());
            container.Register<ISalaDataAccess, SalaDataAccess>(new WebRequestLifestyle());
            container.Register<IControleFaltasDataAccess, ControleFaltasDataAccess>(new WebRequestLifestyle());
            container.Register<IControleFaltasAlunoDataAccess, ControleFaltasAlunoDataAccess>(new WebRequestLifestyle());
            container.Register<IEventoDataAccess, EventoDataAccess>(new WebRequestLifestyle());
            container.Register<IProdutoDataAccess, ProdutoDataAccess>(new WebRequestLifestyle());
            container.Register<IDuracaoDataAccess, DuracaoDataAccess>(new WebRequestLifestyle());
            container.Register<ITipoAtividadeExtraDataAccess, TipoAtividadeExtraDataAccess>(new WebRequestLifestyle());
            container.Register<IMotivoDesistenciaDataAccess, MotivoDesistenciaDataAccess>(new WebRequestLifestyle());
            container.Register<IMotivoFaltaDataAccess, MotivoFaltaDataAccess>(new WebRequestLifestyle());
            container.Register<IModalidadeDataAccess, ModalidadeDataAccess>(new WebRequestLifestyle());
            container.Register<IRegimeDataAccess, RegimeDataAccess>(new WebRequestLifestyle());
            container.Register<IConceitoDataAccess, ConceitoDataAccess>(new WebRequestLifestyle());
            container.Register<IFeriadoDataAccess, FeriadoDataAccess>(new WebRequestLifestyle());
            container.Register<IEstagioDataAccess, EstagioDataAccess>(new WebRequestLifestyle());
            container.Register<ICursoDataAccess, CursoDataAccess>(new WebRequestLifestyle());
            container.Register<ITurmaDataAccess, TurmaDataAccess>(new WebRequestLifestyle());
            container.Register<ICriterioAvaliacaoDataAccess, CriterioAvaliacaoDataAccess>(new WebRequestLifestyle());
            container.Register<ITipoAvaliacaoDataAccess, TipoAvaliacaoDataAccess>(new WebRequestLifestyle());
            container.Register<IAvaliacaoDataAccess, AvaliacaoDataAccess>(new WebRequestLifestyle());
            container.Register<IProgramacaoCursoDataAccess, ProgramacaoCursoDataAccess>(new WebRequestLifestyle());
            container.Register<IItemProgramacaoCursoDataAccess, ItemProgramacaoCursoDataAccess>(new WebRequestLifestyle());
            container.Register<IProfessorDataAccess, ProfessorDataAccess>(new WebRequestLifestyle());
            container.Register<IAulaReposicaoDataAccess, AulaReposicaoDataAccess>(new WebRequestLifestyle());
            container.Register<IAlunoAulaReposicaoDataAccess, AlunoAulaReposicaoDataAccess>(new WebRequestLifestyle());
            container.Register<IAtividadeExtraDataAccess, AtividadeExtraDataAccess>(new WebRequestLifestyle());
            container.Register<IAtividadeAlunoDataAccess, AtividadeAlunoDataAccess>(new WebRequestLifestyle());
            container.Register<IAtividadeRecorrenciaDataAccess, AtividadeRecorrenciaDataAccess>(new WebRequestLifestyle());
            container.Register<IAtividadeProspectDataAccess, AtividadeProspectDataAccess>(new WebRequestLifestyle());
            container.Register<IAtividadeCursoDataAccess, AtividadeCursoDataAccess>(new WebRequestLifestyle());
            container.Register<IAvaliacaoCursoDataAccess, AvaliacaoCursoDataAccess>(new WebRequestLifestyle());
            container.Register<IAvaliacaoTurmaDataAccess, AvaliacaoTurmaDataAccess>(new WebRequestLifestyle());
            container.Register<IAvaliacaoAlunoDataAccess, AvaliacaoAlunoDataAccess>(new WebRequestLifestyle());
            container.Register<IProfessorTurmaDataAccess, ProfessorTurmaDataAccess>(new WebRequestLifestyle());
            container.Register<IProgramacaoTurmaDataAccess, ProgramacaoTurmaDataAccess>(new WebRequestLifestyle());
            container.Register<IDiarioAulaDataAccess, DiarioAulaDataAccess>(new WebRequestLifestyle());
            container.Register<IHorarioProfessorTurmaDataAccess, HorarioProfessorTurmaDataAccess>(new WebRequestLifestyle());
            container.Register<IFeriadoDesconsideradoDataAccess, FeriadoDesconsideradoDataAccess>(new WebRequestLifestyle());
            container.Register<IAlunoEventoDataAccess, AlunoEventoDataAccess>(new WebRequestLifestyle());
            container.Register<IAulaPersonalizadaDataAccess, AulaPersonalizadaDataAccess>(new WebRequestLifestyle());
            container.Register<IAulaPersonalizadaAlunoDataAccess, AulaPersonalizadaAlunoDataAccess>(new WebRequestLifestyle());
            container.Register<IAvaliacaoParticipacaoVincDataAccess, AvaliacaoParticipacaoVincDataAccess>(new WebRequestLifestyle());
            container.Register<IAvaliacaoParticipacaoDataAccess, AvaliacaoParticipacaoDataAccess>(new WebRequestLifestyle());
            container.Register<IParticipacaoDataAccess, ParticipacaoDataAccess>(new WebRequestLifestyle());
            container.Register<INivelDataAccess, NivelDataAccess>(new WebRequestLifestyle());
            container.Register<ICalendarioEvento, CalendarioEventoDataAccess>(new WebRequestLifestyle());
            container.Register<ICalendarioAcademico, CalendarioAcademicoDataAccess>(new WebRequestLifestyle());
            container.Register<IAvaliacaoAlunoParticipacaoDataAccess, AvaliacaoAlunoParticipacaoDataAccess>(new WebRequestLifestyle());
            container.Register<ICargaProfessorDataAccess, CargaProfessorDataAccess>(new WebRequestLifestyle());
            container.Register<IMensagemAvaliacaoDataAccess, MensageAvaliacaoDataAccess>(new WebRequestLifestyle());
            container.Register<IMensagemAvaliacaoAlunoDataAccess, MensagemAvaliacaoAlunoDataAccess>(new WebRequestLifestyle());
            container.Register<IPessoaRafDataAccess, PessoaRafDataAccess>(new WebRequestLifestyle());
            container.Register<IApiAreaRestritaBusiness, ApiAreaRestritaBusiness>(new WebRequestLifestyle());
            container.Register<ITransferenciaAlunoDataAccess, TransferenciaAlunoDataAccess>(new WebRequestLifestyle());
            container.Register<IPerdaMaterialDataAccess, PerdaMaterialDataAccess>(new WebRequestLifestyle());

            container.Register<IEmpresaDataAccess, EmpresaDataAccess>(new WebRequestLifestyle());
            container.Register<IFuncionarioDataAccess, FuncionarioDataAccess>(new WebRequestLifestyle());
            container.Register<IFuncionarioComissaoDataAccess, FuncionarioComissaoDataAccess>(new WebRequestLifestyle());

            container.Register<IEmpresaBusiness, EmpresaBusiness>(new WebRequestLifestyle());
            container.Register<IFuncionarioBusiness, FuncionarioBusiness>(new WebRequestLifestyle());

            // Business
            container.Register<IFinanceiroBusiness, FinanceiroBusiness>(new WebRequestLifestyle());
            container.Register<IFiscalBusiness, FiscalBusiness>(new WebRequestLifestyle());

            // DataAccess
            container.Register<IGrupoEstoqueDataAccess, GrupoEstoqueDataAccess>(new WebRequestLifestyle());
            container.Register<IMovimentacaoFinanceiraDataAccess, MovimentacaoFinanceiraDataAccess>(new WebRequestLifestyle());
            container.Register<ITipoDescontoDataAccess, TipoDescontoDataAccess>(new WebRequestLifestyle());
            container.Register<ITipoFinanceiroDataAccess, TipoFinanceiroDataAccess>(new WebRequestLifestyle());
            container.Register<ITipoLiquidacaoDataAccess, TipoLiquidacaoDataAccess>(new WebRequestLifestyle());
            container.Register<IOrgaoFinanceiroDataAccess, OrgaoFinanceiroDataAccess>(new WebRequestLifestyle());
            container.Register<IAlunoRestricaoDataAccess, AlunoRestricaoDataAccess>(new WebRequestLifestyle());
            container.Register<IItemDataAccess, ItemDataAccess>(new WebRequestLifestyle());
            container.Register<IItemKitDataAccess, ItemKitDataAccess>(new WebRequestLifestyle());
            container.Register<IItemMovItemKitDataAccess, ItemMovItemKitDataAccess>(new WebRequestLifestyle());
            container.Register<IPoliticaDescontoDataAccess, PoliticaDescontoDataAccess>(new WebRequestLifestyle());
            container.Register<IItemEscolaDataAccess, ItemEscolaDataAccess>(new WebRequestLifestyle());
            container.Register<ITipoItemDataAccess, TipoItemDataAccess>(new WebRequestLifestyle());
            container.Register<IBibliotecaDataAccess, BibliotecaDataAccess>(new WebRequestLifestyle());
            container.Register<ITabelaPrecoDataAccess, TabelaPrecoDataAccess>(new WebRequestLifestyle());
            container.Register<IGrupoContaDataAccess, GrupoContaDataAccess>(new WebRequestLifestyle());
            container.Register<ISubgrupoContaDataAccess, SubgrupoContaDataAccess>(new WebRequestLifestyle());
            container.Register<IPlanoContasDataAccess, PlanoContasDataAccess>(new WebRequestLifestyle());
            container.Register<IChequeDataAccess, ChequeDataAccess>(new WebRequestLifestyle());
            container.Register<IBancoDataAccess, BancoDataAccess>(new WebRequestLifestyle());
            container.Register<ITituloDataAccess, TituloDataAccess>(new WebRequestLifestyle());
            container.Register<ILocalMovtoDataAccess, LocalMovtoDataAccess>(new WebRequestLifestyle());
            container.Register<ITransacaoFinanceiraDataAccess, TransacaoFinanceiraDataAccess>(new WebRequestLifestyle());
            container.Register<IBaixaTituloDataAccess, BaixaTituloDataAccess>(new WebRequestLifestyle());
            container.Register<IBaixaAutomaticaDataAccess, BaixaAutomaticaDataAccess>(new WebRequestLifestyle());
            container.Register<ITitulosBaixaAutomaticaDataAccess, TitulosBaixaAutomaticaDataAccess>(new WebRequestLifestyle());
            container.Register<IContaCorrenteDataAccess, ContaCorrenteDataAccess>(new WebRequestLifestyle());
            container.Register<IMovimentoDataAccess, MovimentoDataAccess>(new WebRequestLifestyle());
            container.Register<IPlanoTituloDataAccess, PlanoTituloDataAccess>(new WebRequestLifestyle());
            container.Register<IPoliticaComercialDataAccess, PoliticaComercialDataAccess>(new WebRequestLifestyle());
            container.Register<IItemMovimentoDataAccess, ItemMovimentoDataAccess>(new WebRequestLifestyle());
            container.Register<IKardexDataAccess, KardexDataAccess>(new WebRequestLifestyle());
            container.Register<IItemPoliticaDataAccess, ItemPoliticaDataAccess>(new WebRequestLifestyle());
            container.Register<IDiasPoliticaDataAccess, DiasPoliticaDataAccess>(new WebRequestLifestyle());
            container.Register<IFechamentoDataAccess, FechamentoDataAccess>(new WebRequestLifestyle());
            container.Register<ISaldoItemDataAccess, SaldoItemDataAccess>(new WebRequestLifestyle());
            container.Register<ITipoDescontoEscolaDataAccess, TipoDescontoEscolaDataAccess>(new WebRequestLifestyle());
            container.Register<IItemSubgrupoDataAccess, ItemSubgrupoDataAccess>(new WebRequestLifestyle());
            container.Register<ITipoNotaFiscalDataAccess, TipoNotaFiscalDataAccess>(new WebRequestLifestyle());
            container.Register<ISituacaoTributariaDataAccess, SituacaoTributariaDataAccess>(new WebRequestLifestyle());
            container.Register<IAliquotaUFDataAccess, AliquotaUFDataAccess>(new WebRequestLifestyle());
            container.Register<IDadosNFDataAccess, DadosNFDataAccess>(new WebRequestLifestyle());
            container.Register<ICFOPDataAccess, CFOPDataAccess>(new WebRequestLifestyle());
            container.Register<IPoliticaAlunoDataAccess, PoliticaAlunoDataAccess>(new WebRequestLifestyle());
            container.Register<IPoliticaTurmaDataAccess, PoliticaTurmaDataAccess>(new WebRequestLifestyle());
            container.Register<IObsSaldoCaixa, ObsSaldoCaixaDataAccess>(new WebRequestLifestyle());
            container.Register<IReajusteAnualDataAccess, ReajusteAnualDataAccess>(new WebRequestLifestyle());
            container.Register<IReajusteAlunoDataAccess, ReajusteAlunoDataAccess>(new WebRequestLifestyle());
            container.Register<IReajusteTurmaDataAccess, ReajusteTurmaDataAccess>(new WebRequestLifestyle());
            container.Register<IReajusteCursoDataAccess, ReajusteCursoDataAccess>(new WebRequestLifestyle());
            container.Register<IReajusteTituloDataAccess, ReajusteTituloDataAccess>(new WebRequestLifestyle());
            container.Register<IChequeTransacaoDataAccess, ChequeTransacaoDataAccess>(new WebRequestLifestyle());
            container.Register<IChequeBaixaDataAccess, ChequeBaixaDataAccess>(new WebRequestLifestyle());
            container.Register<IPessoaCartaDataAccess, PessoaCartaDataAccess>(new WebRequestLifestyle());
            //Escola
            container.Register<IEscolaDataAccess, EscolaDataAccess>(new WebRequestLifestyle());
            container.Register<ITurmaEscolaDataAccess, TurmaEscolaDataAccess>(new WebRequestLifestyle());
            container.Register<IAtividadeEscolaAtividadeDataAccess, AtividadeEscolaAtividadeDataAccess>(new WebRequestLifestyle());
            container.Register<ITurmaEscolaEmpresaDataAccess, TurmaEscolaEmpresaDataAccess>(new WebRequestLifestyle());
            container.Register<IAtividadeEscolaAtividadeEmpresaDataAccess, AtividadeEscolaAtividadeEmpresaDataAccess>(new WebRequestLifestyle());
            container.Register<IEscolaBusiness, EscolaBusiness>(new WebRequestLifestyle());

            //parametros
            container.Register<IParametrosDataAccess, ParametrosDataAccess>(new WebRequestLifestyle());
            container.Register<ISysAppDataAccess, SysAppDataAccess>(new WebRequestLifestyle());

            container.Register<ILogGeralDataAccess, LogGeralDataAccess>(new WebRequestLifestyle());
            container.Register<ILogGeralDetalheDataAccess, LogGeralDetalheDataAccess>(new WebRequestLifestyle());
            container.Register<IAtributosDataAccess, AtributosDataAccess>(new WebRequestLifestyle());

            container.Register<ILogGeralBusiness, LogGeralBusiness>(new WebRequestLifestyle());
            container.Register<IPessoaDataAccess, PessoaDataAccess>(new WebRequestLifestyle());
            container.Register<IPessoaEscolaDataAccess, PessoaEscolaDataAccess>(new WebRequestLifestyle());
            container.Register<IPapelDataAccess, PapelDataAccess>(new WebRequestLifestyle());
            container.Register<ITipoLogradouroDataAccess, TipoLogradouroDataAccess>(new WebRequestLifestyle());
            container.Register<ILocalidadeDataAccess, LocalidadeDataAccess>(new WebRequestLifestyle());
            container.Register<IPaisDataAccess, PaisDataAccess>(new WebRequestLifestyle());
            container.Register<IEstadoDataAccess, EstadoDataAccess>(new WebRequestLifestyle());
            container.Register<ITipoEnderecoDataAccess, TipoEnderecoDataAccess>(new WebRequestLifestyle());
            container.Register<IClasseTelefoneDataAccess, ClasseTelefoneDataAccess>(new WebRequestLifestyle());
            container.Register<ITipoTelefoneDataAccess, TipoTelefoneDataAccess>(new WebRequestLifestyle());
            container.Register<IOperadoraDataAccess, OperadoraDataAccess>(new WebRequestLifestyle());
            container.Register<IPessoaBusiness, PessoaBusiness>(new WebRequestLifestyle());
            container.Register<ILocalidadeBusiness, LocalidadeBusiness>(new WebRequestLifestyle());
            container.Register<IPessoaFisicaDataAccess, PessoaFisicaDataAccess>(new WebRequestLifestyle());
            //Telefone
            container.Register<ITelefoneDataAccess, TelefoneDataAccess>(new WebRequestLifestyle());
            //Endereco
            container.Register<IEnderecoDataAccess, EnderecoDataAccess>(new WebRequestLifestyle());
            //Atividade
            container.Register<IAtividadeDataAccess, AtividadeDataAccess>(new WebRequestLifestyle());
            //Estado Civil
            container.Register<IEstadoCivilDataAccess, EstadoCivilDataAccess>(new WebRequestLifestyle());
            //Orgão Expedidor
            container.Register<IOrgaoExpedidorDataAccess, OrgaoExpedidorDataAccess>(new WebRequestLifestyle());
            //Tratamento Pessoa
            container.Register<ITratamentoPessoaDataAccess, TratamentoPessoaDataAccess>(new WebRequestLifestyle());
            //Relacionamento
            container.Register<IRelacionamentoDataAccess, RelacionamentoDataAccess>(new WebRequestLifestyle());

            //Businness
            container.Register<ISecretariaBusiness, SecretariaBusiness>(new WebRequestLifestyle());
            container.Register<IAlunoBusiness, AlunoBusiness>(new WebRequestLifestyle());
            container.Register<IMatriculaBusiness, MatriculaBusiness>(new WebRequestLifestyle());
            //DataAcces
            container.Register<IEscolaridadeDataAccess, EscolaridadeDataAccess>(new WebRequestLifestyle());
            container.Register<IMidiaDataAccess, MidiaDataAccess>(new WebRequestLifestyle());
            container.Register<ITipoContatoDataAccess, TipoContatoDataAccess>(new WebRequestLifestyle());
            container.Register<IMotivoMatriculaDataAccess, MotivoMatriculaDataAccess>(new WebRequestLifestyle());
            container.Register<IMotivoNaoMatriculaDataAccess, MotivoNaoMatriculaDataAccess>(new WebRequestLifestyle());
            container.Register<IMotivoBolsaDataAccess, MotivoBolsaDataAccess>(new WebRequestLifestyle());
            container.Register<IMotivoCancelamentoBolsaDataAccess, MotivoCancelamentoBolsaDataAccess>(new WebRequestLifestyle());
            container.Register<IAlunoDataAccess, AlunoDataAccess>(new WebRequestLifestyle());
            container.Register<IHorarioDataAccess, HorarioDataAccess>(new WebRequestLifestyle());
            container.Register<IAlunoMotivoMatriculaDataAccess, AlunoMotivoMatriculaDataAccess>(new WebRequestLifestyle());
            container.Register<IJsonTesteDataAccess, JsonTesteDataAccess>(new WebRequestLifestyle());
            container.Register<IProspectDataAccess, ProspectDataAccess>(new WebRequestLifestyle());
            container.Register<IProspectDiaDataAccess, ProspectDiaDataAccess>(new WebRequestLifestyle());
            container.Register<IProspectProdutoDataAccess, ProspectProdutoDataAccess>(new WebRequestLifestyle());
            container.Register<IProspectPeriodoDataAccess, ProspectPeriodoDataAccess>(new WebRequestLifestyle());
            container.Register<IFollowUpDataAccess, FollowUpDataAccess>(new WebRequestLifestyle());
            container.Register<IProspectMotivoNaoMatriculaDataAccess, ProspectMotivoNaoMatriculaDataAccess>(new WebRequestLifestyle());
            container.Register<IMatriculaDataAccess, MatriculaDataAccess>(new WebRequestLifestyle());
            container.Register<ICursoContratoDataAccess, CursoContratoDataAccess>(new WebRequestLifestyle());
            container.Register<IGeraNotasXmlDataAccess, GeraNotasXmlDataAccess>(new WebRequestLifestyle());
            container.Register<IAlunoTurmaDataAccess, AlunoTurmaDataAccess>(new WebRequestLifestyle());
            container.Register<INomeContratoDataAccess, NomeContratoDataAccess>(new WebRequestLifestyle());
            container.Register<IDescontoContratoDataAccess, DescontoContratoDataAccess>(new WebRequestLifestyle());
            container.Register<ITaxaMatriculaDataAccess, TaxaMatriculaDataAccess>(new WebRequestLifestyle());
            container.Register<IAditamentoDataAccess, AditamentoDataAccess>(new WebRequestLifestyle());
            container.Register<IAditamentoBolsaDataAccess, AditamentoBolsaDataAccess>(new WebRequestLifestyle());
            container.Register<IHistoricoAlunoDataAccess, HistoricoAlunoDataAccess>(new WebRequestLifestyle());
            container.Register<IAlunoBolsaDataAccess, AlunoBolsaDataAccess>(new WebRequestLifestyle());
            container.Register<IDesistenciaDataAccess, DesistenciaDataAccess>(new WebRequestLifestyle());
            container.Register<IAcaoFollowupDataAccess, AcaoFollowupDataAccess>(new WebRequestLifestyle());
            container.Register<IFollowUpEscolaDataAccess, FollowUpEscolaDataAccess>(new WebRequestLifestyle());
            container.Register<IFollowUpUsuarioDataAccess, FollowUpUsuarioDataAccess>(new WebRequestLifestyle());
            container.Register<IAnoEscolarDataAccess, AnoEscolarDataAccess>(new WebRequestLifestyle());
            container.Register<IUsuarioBusiness, UsuarioBusiness>(new WebRequestLifestyle());
            container.Register<IPermissaoBusiness, PermissaoBusiness>(new WebRequestLifestyle());
            container.Register<IUsuarioDataAccess, UsuarioDataAccess>(new WebRequestLifestyle());
            container.Register<IGrupoDataAccess, GrupoDataAccess>(new WebRequestLifestyle());
            container.Register<IMenuDataAccess, MenuDataAccess>(new WebRequestLifestyle());
            container.Register<ISysDireitoGrupoDataAccess, SysDireitoGrupoDataAccess>(new WebRequestLifestyle());
            container.Register<ISysGrupoUsuarioDataAccess, SysGrupoUsuarioDataAccess>(new WebRequestLifestyle());
            container.Register<IUsuarioEmpresaDataAccess, UsuarioEmpresaDataAccess>(new WebRequestLifestyle());
            container.Register<IDireitoUsuarioDataAccess, DireitoUsuarioDataAccess>(new WebRequestLifestyle());

            container.Register<IDespesaTituloCnabDataAccess, DespesaTituloCnabDataAccess>(new WebRequestLifestyle());
            container.Register<ITaxaBancariaDataAccess, TaxaBancariaDataAccess>(new WebRequestLifestyle());
            container.Register<IMotivoTransferenciaDataAccess, MotivoTransferenciaDataAccess>(new WebRequestLifestyle());

            //CIRCULAR
            container.Register<ICircular, CircularDataAccess>(new WebRequestLifestyle());

            //ITEM MOVIMENTO KIT
            container.Register<IItemMovimentoKitDataAccess, ItemMovimentoKitDataAccess>(new WebRequestLifestyle());


            //Manager Reposiory
            //container.Register<IManagerRepository, ManagerRepositoryHttp>(new WebRequestLifestyle());
            container.Register<IManagerRepository, ManagerRepositoryHttp>(new WebRequestLifestyle());

            //DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));
            ServiceLocator.SetLocatorProvider(() => new SimpleInjectorServiceLocatorAdapter(container));

            //TITULO ADITAMENTO
            container.Register<ITituloAditamento, TituloAditamentoDataAccess>(new WebRequestLifestyle());

            // SMS
            //container.Register<ISmsDataAcccess, DataAccessSms>(new WebRequestLifestyle());
            //container.Register<ISmsMensagempadraoDataAccess, DataAccessMensagemPadraoSms>(new WebRequestLifestyle());
            
        }

        public static bool ExisteIOC() {
            bool retorno = false;
            try
            {
                var gerenciador = ServiceLocator.Current.GetInstance<IBibliotecaBusiness>()
                                 as BibliotecaBusiness;
                if (gerenciador != null)
                    retorno = true;
            }
            catch (Exception) {
            }

            return retorno;
        }

        public static T GetInstance<T>() 
        {
            var gerenciador = ServiceLocator.Current.GetInstance<T>();
            return gerenciador;
        }
    }
}
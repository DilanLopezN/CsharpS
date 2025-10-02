using MvcTurbine.ComponentModel;
using FundacaoFisk.SGF.Web.Services.Financeiro.Business;
using FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess;
using FundacaoFisk.SGF.GenericModel;
using System;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Registration
{
    public class FinanceiroServiceRegistration : IServiceRegistration
    {
        public void Register(IServiceLocator locator)
        {
            // Business
            locator.Register<IFinanceiroBusiness, FinanceiroBusiness>();
            locator.Register<IFiscalBusiness, FiscalBusiness>();

            // DataAccess
            locator.Register<IGrupoEstoqueDataAccess, GrupoEstoqueDataAccess>();
            locator.Register<IMovimentacaoFinanceiraDataAccess, MovimentacaoFinanceiraDataAccess>();
            locator.Register<ITipoDescontoDataAccess, TipoDescontoDataAccess>();
            locator.Register<ITipoFinanceiroDataAccess, TipoFinanceiroDataAccess>();
            locator.Register<ITipoLiquidacaoDataAccess, TipoLiquidacaoDataAccess>();
            locator.Register<IItemDataAccess, ItemDataAccess>();
            locator.Register<IPoliticaDescontoDataAccess, PoliticaDescontoDataAccess>();
            locator.Register<IItemEscolaDataAccess, ItemEscolaDataAccess>();
            locator.Register<ITipoItemDataAccess, TipoItemDataAccess>();
            locator.Register<IBibliotecaDataAccess, BibliotecaDataAccess>();
            locator.Register<ITabelaPrecoDataAccess, TabelaPrecoDataAccess>();
            locator.Register<IGrupoContaDataAccess, GrupoContaDataAccess>();
            locator.Register<ISubgrupoContaDataAccess, SubgrupoContaDataAccess>();
            locator.Register<IPlanoContasDataAccess, PlanoContasDataAccess>();
            locator.Register<IChequeDataAccess, ChequeDataAccess>();
            locator.Register<IBancoDataAccess, BancoDataAccess>();
            locator.Register<ITituloDataAccess, TituloDataAccess>();
            locator.Register<ILocalMovtoDataAccess, LocalMovtoDataAccess>();
            locator.Register<ITransacaoFinanceiraDataAccess, TransacaoFinanceiraDataAccess>();
            locator.Register<IBaixaTituloDataAccess, BaixaTituloDataAccess>();
            locator.Register<IContaCorrenteDataAccess, ContaCorrenteDataAccess>();
            locator.Register<IMovimentoDataAccess, MovimentoDataAccess>();
            locator.Register<IPlanoTituloDataAccess, PlanoTituloDataAccess>();
            locator.Register<IPoliticaComercialDataAccess, PoliticaComercialDataAccess>();
            locator.Register<IItemMovimentoDataAccess, ItemMovimentoDataAccess>();
            locator.Register<IKardexDataAccess, KardexDataAccess>();
            locator.Register<IItemPoliticaDataAccess, ItemPoliticaDataAccess>();
            locator.Register<IDiasPoliticaDataAccess, DiasPoliticaDataAccess>();
            locator.Register<IFechamentoDataAccess, FechamentoDataAccess>();
            locator.Register<ISaldoItemDataAccess, SaldoItemDataAccess>();
            locator.Register<ITipoDescontoEscolaDataAccess, TipoDescontoEscolaDataAccess>();
            locator.Register<IItemSubgrupoDataAccess, ItemSubgrupoDataAccess>();
            locator.Register<ITipoNotaFiscalDataAccess, TipoNotaFiscalDataAccess>();
            locator.Register<ISituacaoTributariaDataAccess, SituacaoTributariaDataAccess>();
            locator.Register<IAliquotaUFDataAccess, AliquotaUFDataAccess>();
            locator.Register<IDadosNFDataAccess, DadosNFDataAccess>();
            locator.Register<ICFOPDataAccess, CFOPDataAccess>();
            locator.Register<IPoliticaAlunoDataAccess, PoliticaAlunoDataAccess>();
            locator.Register<IPoliticaTurmaDataAccess, PoliticaTurmaDataAccess>();
            locator.Register<IObsSaldoCaixa, ObsSaldoCaixaDataAccess>();
            locator.Register<IReajusteAnualDataAccess, ReajusteAnualDataAccess>();
            locator.Register<IReajusteAlunoDataAccess, ReajusteAlunoDataAccess>();
            locator.Register<IReajusteTurmaDataAccess, ReajusteTurmaDataAccess>();
            locator.Register<IReajusteCursoDataAccess, ReajusteCursoDataAccess>();
            locator.Register<IReajusteTituloDataAccess, ReajusteTituloDataAccess>();
            locator.Register<IChequeTransacaoDataAccess, ChequeTransacaoDataAccess>();
            locator.Register<IChequeBaixaDataAccess, ChequeBaixaDataAccess>();
        }
    }
}

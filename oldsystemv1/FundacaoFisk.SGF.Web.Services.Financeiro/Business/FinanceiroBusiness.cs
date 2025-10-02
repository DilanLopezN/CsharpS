using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Threading.Tasks;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IBusiness;
using FundacaoFisk.SGF.GenericModel;
using log4net;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Business
{
    using FundacaoFisk.SGF.Web.Services.Financeiro.Model;
    using System.Transactions;
    using FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess;
    using FundacaoFisk.SGF.Utils.Messages;
    using System.Data.Entity;
    using System.Collections;
    using Componentes.GenericDataAccess.GenericException;
    using System.Data.Entity.Core.Objects;
    using Componentes.GenericBusiness.Excepion;
    using FundacaoFisk.SGF.Web.Services.Empresa.Comum.IBusiness;

    using Newtonsoft.Json;
    using System.Data.Entity.Infrastructure;
    using FundacaoFisk.SGF.Web.Services.Empresa.Model;
    using Componentes.GenericBusiness;
    using Componentes.GenericModel;
    using System.Globalization;
    using System.Data.SqlTypes;

    public class FinanceiroBusiness : IFinanceiroBusiness
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(FinanceiroBusiness));
        const int BIBLIOTECA = 3;

        /// <summary> 
        ///Declaração de Interfaces
        /// </summary>
        public IGrupoEstoqueDataAccess DataAccessGrupoEstoque { get; set; }
        public IMovimentacaoFinanceiraDataAccess DataAccessMovimentacaoFinanceira { get; set; }
        public ITipoLiquidacaoDataAccess DataAccessTipoLiquidacao { get; set; }
        public ITipoFinanceiroDataAccess DataAccessTipoFinanceiro { get; set; }
        public ITipoDescontoDataAccess DataAccessTipoDesconto { get; set; }
        public IItemDataAccess DataAccessItem { get; set; }
        public IItemKitDataAccess DataAccessItemKit { get; set; }
        public IPoliticaDescontoDataAccess DataAccessPoliticaDesconto { get; set; }
        public IDiasPoliticaDataAccess DataAccessDiasPolitica { get; set; }
        public ITabelaPrecoDataAccess DataAccessTabelaPreco { get; set; }
        public IItemEscolaDataAccess DataAccessItemEscola { get; set; }
        public ITipoItemDataAccess DataAccessTipoItem { get; set; }
        public IBibliotecaDataAccess DataAccessBiblioteca { get; set; }
        public IGrupoContaDataAccess DataAccessGrupoConta { get; set; }
        public ISubgrupoContaDataAccess DataAccessSubgrupoConta { get; set; }
        public IPlanoContasDataAccess DataAccessPlanoConta { get; set; }
        public IBancoDataAccess DataAccessBanco { get; set; }
        public ITituloDataAccess DataAccessTitulo { get; set; }
        public IPlanoTituloDataAccess DataAccessPlanoTitulo { get; set; }
        public ILocalMovtoDataAccess DataAccessLocalMovto { get; set; }
        public ITransacaoFinanceiraDataAccess DataAccessTransacaoFinanceira { get; set; }
        public IBaixaTituloDataAccess DataAccessBaixaFinan { get; set; }
        public IContaCorrenteDataAccess DataAccessContaCorrente { get; set; }
        public IPoliticaComercialDataAccess DataAccessPoliticaComercial { get; set; }
        public IKardexDataAccess DataAccessKardex { get; set; }
        public IItemPoliticaDataAccess DataAccessItemPolitica { get; set; }
        public IFechamentoDataAccess DataAccessFechamento { get; set; }
        public ISaldoItemDataAccess DataAccessSaldoItem { get; set; }
        public ITipoDescontoEscolaDataAccess DataAccessTipoDescontoEscola { get; set; }
        public IItemSubgrupoDataAccess DataAccessItemSubgrupo { get; set; }
        public ISituacaoTributariaDataAccess DataAccessSituacaoTributaria { get; set; }
        public IAliquotaUFDataAccess DataAccessAliquotaUF { get; set; }
        public IDadosNFDataAccess DataAccessDadosNF { get; set; }
        public IEmpresaBusiness BusinessEmpresa { get; set; }
        public IPoliticaTurmaDataAccess DataAccessPoliticaTurma { get; set; }
        public IPoliticaAlunoDataAccess DataAccessPoliticaAluno { get; set; }
        public IObsSaldoCaixa DataAccessObsSaldoCaixa { get; set; }
        public IReajusteAnualDataAccess DataAccessReajusteAnual { get; set; }
        public IReajusteAlunoDataAccess DataAccessReajusteAluno { get; set; }
        public IReajusteTurmaDataAccess DataAccessReajusteTurma { get; set; }
        public IReajusteCursoDataAccess DataAccessReajusteCurso { get; set; }
        public IReajusteTituloDataAccess DataAccessReajusteTitulo { get; set; }
        public IChequeTransacaoDataAccess DataAccessChequeTransacao { get; set; }
        public IChequeBaixaDataAccess DataAccessChequeBaixa { get; set; }
        public IChequeDataAccess DataAccessCheque { get; set; }
        public IMovimentoDataAccess DataAccessMovimento { get; set; }
        public ITaxaBancariaDataAccess DataAccessTaxaBancaria { get; set; }
        public ITituloAditamento DataAccessTituloAditamento { get; set; }
        public IBaixaAutomaticaDataAccess DataAccessBaixaAutomatica { get; set; }
        public ITitulosBaixaAutomaticaDataAccess DataAccessTitulosBaixaAutomatica { get; set; }
        public IOrgaoFinanceiroDataAccess DataAccessOrgaoFinanceiro { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataAccessGrupoEstoque"></param>
        /// <param name="dataAccessMovimentacaoFinanceira"></param>
        /// <param name="dataAccessTipoLiquidacao"></param>
        /// <param name="dataAccessTipoFinanceiro"></param>
        /// <param name="dataAccessTipoDesconto"></param>
        /// <param name="dataAccessItem"></param>
        /// <param name="dataAccessPoliticaDesconto"></param>
        /// <param name="dataAccessItemEscola"></param>
        /// <param name="dataAccessTipoItem"></param>
        /// <param name="dataAccessBiblioteca"></param>
        /// <param name="dataAccessTabelaPreco"></param>
        /// <param name="dataAccessGrupoConta"></param>
        /// <param name="dataAccessSubgrupoConta"></param>
        /// <param name="dataAccessPlanoConta"></param>
        /// <param name="dataAccessCheque"></param>
        /// <param name="dataAccessBanco"></param>
        /// <param name="dataAccessTitulo"></param>
        /// <param name="dataAccessPlanoTitulo"></param>
        /// <param name="dataAccessLocalMovto"></param>
        /// <param name="dataAccessTransacaoFinanceira"></param>
        /// <param name="dataAccessBaixaFinan"></param>
        /// <param name="dataAccessContaCorrente"></param>
        /// <param name="dataAccessMovimento"></param>
        /// <param name="dataAccessPoliticaComercial"></param>
        /// <param name="dataAccessItemMovimento"></param>
        /// <param name="dataAccessKardex"></param>
        /// <param name="dataAccessBaixaAutomatica"></param>
        /// <param name="dataAccessTitulosBaixaAutomatica"></param>
        /// 
        public FinanceiroBusiness(IGrupoEstoqueDataAccess dataAccessGrupoEstoque, IMovimentacaoFinanceiraDataAccess dataAccessMovimentacaoFinanceira,
                                  ITipoLiquidacaoDataAccess dataAccessTipoLiquidacao, ITipoFinanceiroDataAccess dataAccessTipoFinanceiro,
                                  ITipoDescontoDataAccess dataAccessTipoDesconto, IItemDataAccess dataAccessItem, IItemKitDataAccess dataAccessItemKit,
                                  IPoliticaDescontoDataAccess dataAccessPoliticaDesconto, IItemEscolaDataAccess dataAccessItemEscola,
                                  ITipoItemDataAccess dataAccessTipoItem, IBibliotecaDataAccess dataAccessBiblioteca, ITabelaPrecoDataAccess dataAccessTabelaPreco,
                                  IGrupoContaDataAccess dataAccessGrupoConta, ISubgrupoContaDataAccess dataAccessSubgrupoConta,
                                  IPlanoContasDataAccess dataAccessPlanoConta, IChequeDataAccess dataAccessCheque, IBancoDataAccess dataAccessBanco,
                                  ITituloDataAccess dataAccessTitulo, IPlanoTituloDataAccess dataAccessPlanoTitulo, ILocalMovtoDataAccess dataAccessLocalMovto, ITransacaoFinanceiraDataAccess dataAccessTransacaoFinanceira,
                                  IBaixaTituloDataAccess dataAccessBaixaFinan, IContaCorrenteDataAccess dataAccessContaCorrente, IMovimentoDataAccess dataAccessMovimento,
                                  IPoliticaComercialDataAccess dataAccessPoliticaComercial, IItemMovimentoDataAccess dataAccessItemMovimento, IKardexDataAccess dataAccessKardex,
                                  IItemPoliticaDataAccess dataAccessItemPolitica, IDiasPoliticaDataAccess dataAccessDiasPolitica, IFechamentoDataAccess dataAccessFechamento,
                                  ISaldoItemDataAccess dataAccessSaldoItem, ITipoDescontoEscolaDataAccess dataAccessTipoDescontoEscola,
                                  IItemSubgrupoDataAccess dataAccessItemSubgrupo, ISituacaoTributariaDataAccess dataAccessSituacaoTributaria, IAliquotaUFDataAccess dataAccessAliquotaUF, IDadosNFDataAccess dataAccessDadosNF,
                                  IEmpresaBusiness empresaBusiness, IPoliticaAlunoDataAccess dataAccessPoliticaAluno, IPoliticaTurmaDataAccess dataAccessPoliticaTurma, IObsSaldoCaixa dataAccessObsSaldoCaixa,
                                  IReajusteAnualDataAccess dataAccessReajusteAnual, IReajusteAlunoDataAccess dataAccessReajusteAluno, IReajusteCursoDataAccess dataAccessReajusteCurso,
                                  IReajusteTurmaDataAccess dataAccessReajusteTurma, IReajusteTituloDataAccess dataAccessReajusteTitulo, IChequeTransacaoDataAccess dataAccessChequeTransacao,
                                  IChequeBaixaDataAccess dataAccessChequeBaixa, ITaxaBancariaDataAccess dataAccessTaxaBancaria, ITituloAditamento dataAccessTituloAditamento,
                                  IBaixaAutomaticaDataAccess dataAccessBaixaAutomatica, ITitulosBaixaAutomaticaDataAccess dataAccessTitulosBaixaAutomatica,
                                  IOrgaoFinanceiroDataAccess dataAccessOrgaoFinanceiro)
        {
            if (dataAccessGrupoEstoque == null || dataAccessMovimentacaoFinanceira == null || dataAccessTipoLiquidacao == null
                || dataAccessTipoFinanceiro == null || dataAccessTipoDesconto == null || dataAccessTipoDescontoEscola == null || dataAccessItem == null || dataAccessItemKit == null
                || dataAccessPoliticaDesconto == null || dataAccessItemEscola == null || dataAccessTipoItem == null
                || dataAccessBiblioteca == null || dataAccessTabelaPreco == null || dataAccessGrupoConta == null
                || dataAccessGrupoConta == null || dataAccessPlanoConta == null || dataAccessCheque == null || dataAccessBanco == null
                || dataAccessTitulo == null || dataAccessPlanoTitulo == null || dataAccessLocalMovto == null || dataAccessBaixaFinan == null || dataAccessContaCorrente == null
                || dataAccessMovimento == null || dataAccessPoliticaComercial == null || dataAccessItemMovimento == null || dataAccessKardex == null || dataAccessItemPolitica == null
                || dataAccessDiasPolitica == null || dataAccessFechamento == null || dataAccessSaldoItem == null || dataAccessItemSubgrupo == null
                || dataAccessSituacaoTributaria == null || dataAccessAliquotaUF == null || dataAccessDadosNF == null || empresaBusiness == null || dataAccessPoliticaAluno == null
                || dataAccessPoliticaTurma == null || dataAccessObsSaldoCaixa == null || dataAccessReajusteAnual == null || dataAccessReajusteAluno == null
                || dataAccessReajusteTurma == null || dataAccessReajusteCurso == null || dataAccessReajusteTitulo == null || dataAccessChequeTransacao == null
                || dataAccessChequeBaixa == null || dataAccessTaxaBancaria == null || dataAccessTituloAditamento == null || dataAccessBaixaAutomatica == null || dataAccessTitulosBaixaAutomatica == null ||
                   dataAccessOrgaoFinanceiro == null)
            {
                throw new ArgumentNullException("repository");
            }
            this.DataAccessGrupoEstoque = dataAccessGrupoEstoque;
            this.DataAccessTipoLiquidacao = dataAccessTipoLiquidacao;
            this.DataAccessTipoFinanceiro = dataAccessTipoFinanceiro;
            this.DataAccessTipoDesconto = dataAccessTipoDesconto;
            this.DataAccessTipoDescontoEscola = dataAccessTipoDescontoEscola;
            this.DataAccessMovimentacaoFinanceira = dataAccessMovimentacaoFinanceira;
            this.DataAccessItem = dataAccessItem;
            this.DataAccessItemKit = dataAccessItemKit;
            this.DataAccessPoliticaDesconto = dataAccessPoliticaDesconto;
            this.DataAccessItemEscola = dataAccessItemEscola;
            this.DataAccessTipoItem = dataAccessTipoItem;
            this.DataAccessBiblioteca = dataAccessBiblioteca;
            this.DataAccessTabelaPreco = dataAccessTabelaPreco;
            this.DataAccessGrupoConta = dataAccessGrupoConta;
            this.DataAccessSubgrupoConta = dataAccessSubgrupoConta;
            this.DataAccessPlanoConta = dataAccessPlanoConta;
            this.DataAccessCheque = dataAccessCheque;
            this.DataAccessBanco = dataAccessBanco;
            this.DataAccessTitulo = dataAccessTitulo;
            this.DataAccessPlanoTitulo = dataAccessPlanoTitulo;
            this.DataAccessLocalMovto = dataAccessLocalMovto;
            this.DataAccessTransacaoFinanceira = dataAccessTransacaoFinanceira;
            this.DataAccessBaixaFinan = dataAccessBaixaFinan;
            this.DataAccessContaCorrente = dataAccessContaCorrente;
            this.DataAccessPoliticaComercial = dataAccessPoliticaComercial;
            this.DataAccessKardex = dataAccessKardex;
            this.DataAccessItemPolitica = dataAccessItemPolitica;
            this.DataAccessDiasPolitica = dataAccessDiasPolitica;
            this.DataAccessFechamento = dataAccessFechamento;
            this.DataAccessSaldoItem = dataAccessSaldoItem;
            this.DataAccessItemSubgrupo = dataAccessItemSubgrupo;
            this.DataAccessSituacaoTributaria = dataAccessSituacaoTributaria;
            this.DataAccessAliquotaUF = dataAccessAliquotaUF;
            this.DataAccessDadosNF = dataAccessDadosNF;
            this.BusinessEmpresa = empresaBusiness;
            this.DataAccessPoliticaAluno = dataAccessPoliticaAluno;
            this.DataAccessPoliticaTurma = dataAccessPoliticaTurma;
            this.DataAccessObsSaldoCaixa = dataAccessObsSaldoCaixa;
            this.DataAccessReajusteAnual = dataAccessReajusteAnual;
            this.DataAccessReajusteAluno = dataAccessReajusteAluno;
            this.DataAccessReajusteTurma = dataAccessReajusteTurma;
            this.DataAccessReajusteCurso = dataAccessReajusteCurso;
            this.DataAccessReajusteTitulo = dataAccessReajusteTitulo;
            this.DataAccessChequeTransacao = dataAccessChequeTransacao;
            this.DataAccessChequeBaixa = dataAccessChequeBaixa;
            this.DataAccessMovimento = dataAccessMovimento;
            this.DataAccessTaxaBancaria = dataAccessTaxaBancaria;
            this.DataAccessTituloAditamento = dataAccessTituloAditamento;
            this.DataAccessBaixaAutomatica = dataAccessBaixaAutomatica;
            this.DataAccessTitulosBaixaAutomatica = dataAccessTitulosBaixaAutomatica;
            this.DataAccessOrgaoFinanceiro = dataAccessOrgaoFinanceiro;
        }

        public void configuraUsuario(int cdUsuario, int cd_empresa)
        {
            // Configura os codigos do usuário para auditorias dos DataAccess:
            ((SGFWebContext)this.DataAccessGrupoEstoque.DB()).IdUsuario = ((SGFWebContext)this.DataAccessTipoLiquidacao.DB()).IdUsuario = ((SGFWebContext)this.DataAccessTipoFinanceiro.DB()).IdUsuario =
            ((SGFWebContext)this.DataAccessTipoDesconto.DB()).IdUsuario = ((SGFWebContext)this.DataAccessMovimentacaoFinanceira.DB()).IdUsuario = ((SGFWebContext)this.DataAccessItem.DB()).IdUsuario = ((SGFWebContext)this.DataAccessItemKit.DB()).IdUsuario =
            ((SGFWebContext)this.DataAccessPoliticaDesconto.DB()).IdUsuario = ((SGFWebContext)this.DataAccessItemEscola.DB()).IdUsuario =
            ((SGFWebContext)this.DataAccessTipoItem.DB()).IdUsuario = ((SGFWebContext)this.DataAccessBiblioteca.DB()).IdUsuario =
            ((SGFWebContext)this.DataAccessTabelaPreco.DB()).IdUsuario = ((SGFWebContext)this.DataAccessPlanoConta.DB()).IdUsuario =
            ((SGFWebContext)this.DataAccessCheque.DB()).IdUsuario = ((SGFWebContext)this.DataAccessLocalMovto.DB()).IdUsuario =
            ((SGFWebContext)this.DataAccessBanco.DB()).IdUsuario = ((SGFWebContext)this.DataAccessTitulo.DB()).IdUsuario = ((SGFWebContext)this.DataAccessPlanoTitulo.DB()).IdUsuario =
            ((SGFWebContext)DataAccessTransacaoFinanceira.DB()).IdUsuario = ((SGFWebContext)DataAccessBaixaFinan.DB()).IdUsuario =
            ((SGFWebContext)DataAccessKardex.DB()).IdUsuario = ((SGFWebContext)DataAccessPoliticaComercial.DB()).IdUsuario =
            ((SGFWebContext)DataAccessContaCorrente.DB()).IdUsuario = ((SGFWebContext)DataAccessItemPolitica.DB()).IdUsuario = ((SGFWebContext)DataAccessDiasPolitica.DB()).IdUsuario =
            ((SGFWebContext)DataAccessFechamento.DB()).IdUsuario = ((SGFWebContext)this.DataAccessTipoDescontoEscola.DB()).IdUsuario = ((SGFWebContext)this.DataAccessItemSubgrupo.DB()).IdUsuario =
            ((SGFWebContext)this.DataAccessSubgrupoConta.DB()).IdUsuario = ((SGFWebContext)DataAccessSituacaoTributaria.DB()).IdUsuario =
            ((SGFWebContext)DataAccessAliquotaUF.DB()).IdUsuario = ((SGFWebContext)DataAccessDadosNF.DB()).IdUsuario = ((SGFWebContext)DataAccessPoliticaAluno.DB()).IdUsuario =
            ((SGFWebContext)DataAccessPoliticaTurma.DB()).IdUsuario = ((SGFWebContext)DataAccessObsSaldoCaixa.DB()).IdUsuario =
            ((SGFWebContext)DataAccessReajusteAluno.DB()).IdUsuario = ((SGFWebContext)DataAccessReajusteAnual.DB()).IdUsuario =
            ((SGFWebContext)DataAccessReajusteTurma.DB()).IdUsuario = ((SGFWebContext)DataAccessReajusteCurso.DB()).IdUsuario =
            ((SGFWebContext)DataAccessReajusteTitulo.DB()).IdUsuario = ((SGFWebContext)DataAccessChequeTransacao.DB()).IdUsuario =
            ((SGFWebContext)DataAccessChequeBaixa.DB()).IdUsuario = ((SGFWebContext)DataAccessMovimento.DB()).IdUsuario =
            ((SGFWebContext)DataAccessTaxaBancaria.DB()).IdUsuario = ((SGFWebContext)DataAccessTituloAditamento.DB()).IdUsuario =
            ((SGFWebContext)DataAccessBaixaAutomatica.DB()).IdUsuario = ((SGFWebContext)DataAccessTitulosBaixaAutomatica.DB()).IdUsuario =
             ((SGFWebContext)DataAccessOrgaoFinanceiro.DB()).IdUsuario = cdUsuario;

            ((SGFWebContext)this.DataAccessGrupoEstoque.DB()).cd_empresa = ((SGFWebContext)this.DataAccessTipoLiquidacao.DB()).cd_empresa = ((SGFWebContext)this.DataAccessTipoFinanceiro.DB()).cd_empresa =
            ((SGFWebContext)this.DataAccessTipoDesconto.DB()).cd_empresa = ((SGFWebContext)this.DataAccessMovimentacaoFinanceira.DB()).cd_empresa = ((SGFWebContext)this.DataAccessItem.DB()).cd_empresa = ((SGFWebContext)this.DataAccessItemKit.DB()).cd_empresa =
            ((SGFWebContext)this.DataAccessPoliticaDesconto.DB()).cd_empresa = ((SGFWebContext)this.DataAccessItemEscola.DB()).cd_empresa =
            ((SGFWebContext)this.DataAccessPoliticaDesconto.DB()).cd_empresa = ((SGFWebContext)this.DataAccessItemEscola.DB()).cd_empresa =
            ((SGFWebContext)this.DataAccessTipoItem.DB()).cd_empresa = ((SGFWebContext)this.DataAccessBiblioteca.DB()).cd_empresa =
            ((SGFWebContext)this.DataAccessTabelaPreco.DB()).cd_empresa = ((SGFWebContext)this.DataAccessPlanoConta.DB()).cd_empresa =
            ((SGFWebContext)this.DataAccessCheque.DB()).cd_empresa = ((SGFWebContext)this.DataAccessLocalMovto.DB()).cd_empresa =
            ((SGFWebContext)this.DataAccessBanco.DB()).cd_empresa = ((SGFWebContext)this.DataAccessTitulo.DB()).cd_empresa = ((SGFWebContext)DataAccessTransacaoFinanceira.DB()).cd_empresa =
            ((SGFWebContext)DataAccessBaixaFinan.DB()).cd_empresa =
            ((SGFWebContext)DataAccessKardex.DB()).cd_empresa = ((SGFWebContext)DataAccessPoliticaComercial.DB()).cd_empresa =
            ((SGFWebContext)DataAccessContaCorrente.DB()).cd_empresa = ((SGFWebContext)DataAccessItemPolitica.DB()).cd_empresa = ((SGFWebContext)DataAccessDiasPolitica.DB()).cd_empresa =
            ((SGFWebContext)DataAccessFechamento.DB()).cd_empresa = ((SGFWebContext)DataAccessTipoDescontoEscola.DB()).cd_empresa = ((SGFWebContext)DataAccessItemSubgrupo.DB()).cd_empresa =
            ((SGFWebContext)DataAccessSituacaoTributaria.DB()).cd_empresa = ((SGFWebContext)this.DataAccessSubgrupoConta.DB()).cd_empresa =
            ((SGFWebContext)DataAccessPoliticaAluno.DB()).cd_empresa = ((SGFWebContext)DataAccessPoliticaTurma.DB()).cd_empresa = cd_empresa;
            ((SGFWebContext)DataAccessAliquotaUF.DB()).cd_empresa = ((SGFWebContext)DataAccessDadosNF.DB()).cd_empresa =
            ((SGFWebContext)DataAccessObsSaldoCaixa.DB()).cd_empresa = ((SGFWebContext)DataAccessReajusteAnual.DB()).cd_empresa =
            ((SGFWebContext)DataAccessReajusteAluno.DB()).cd_empresa = ((SGFWebContext)DataAccessReajusteTurma.DB()).cd_empresa = ((SGFWebContext)DataAccessReajusteCurso.DB()).cd_empresa =
            ((SGFWebContext)DataAccessReajusteTitulo.DB()).cd_empresa = ((SGFWebContext)DataAccessChequeTransacao.DB()).cd_empresa =
            ((SGFWebContext)DataAccessChequeBaixa.DB()).cd_empresa = ((SGFWebContext)DataAccessMovimento.DB()).cd_empresa =
            ((SGFWebContext)DataAccessTaxaBancaria.DB()).cd_empresa = ((SGFWebContext)DataAccessTituloAditamento.DB()).cd_empresa =
            ((SGFWebContext)DataAccessBaixaAutomatica.DB()).cd_empresa = ((SGFWebContext)DataAccessTitulosBaixaAutomatica.DB()).cd_empresa =
             ((SGFWebContext)DataAccessOrgaoFinanceiro.DB()).cd_empresa = cd_empresa; 
            BusinessEmpresa.configuraUsuario(cdUsuario, cd_empresa);
        }

        public void sincronizarContextos(DbContext dbContext)
        {
            //this.DataAccessTransacaoFinanceira.sincronizaContexto(dbContext);
            //this.DataAccessItemEscola.sincronizaContexto(dbContext);
            //this.DataAccessKardex.sincronizaContexto(dbContext);
            //this.DataAccessItem.sincronizaContexto(dbContext);
            //this.DataAccessFechamento.sincronizaContexto(dbContext);
            //this.DataAccessGrupoEstoque.sincronizaContexto(dbContext);
            //this.DataAccessMovimentacaoFinanceira.sincronizaContexto(dbContext);
            //this.DataAccessTipoLiquidacao.sincronizaContexto(dbContext);
            //this.DataAccessTipoFinanceiro.sincronizaContexto(dbContext);
            //this.DataAccessTipoDesconto.sincronizaContexto(dbContext);
            //this.DataAccessPoliticaDesconto.sincronizaContexto(dbContext);
            //this.DataAccessDiasPolitica.sincronizaContexto(dbContext);
            //this.DataAccessTabelaPreco.sincronizaContexto(dbContext);
            //this.DataAccessTipoItem.sincronizaContexto(dbContext);
            //this.DataAccessBiblioteca.sincronizaContexto(dbContext);
            //this.DataAccessGrupoConta.sincronizaContexto(dbContext);
            //this.DataAccessSubgrupoConta.sincronizaContexto(dbContext);
            //this.DataAccessPlanoConta.sincronizaContexto(dbContext);
            //this.DataAccessCheque.sincronizaContexto(dbContext);
            //this.DataAccessBanco.sincronizaContexto(dbContext);
            //this.DataAccessTitulo.sincronizaContexto(dbContext);
            //this.DataAccessPlanoTitulo.sincronizaContexto(dbContext);
            //this.DataAccessLocalMovto.sincronizaContexto(dbContext);
            //this.DataAccessBaixaFinan.sincronizaContexto(dbContext);
            //this.DataAccessContaCorrente.sincronizaContexto(dbContext);
            //this.DataAccessPoliticaComercial.sincronizaContexto(dbContext);
            //this.DataAccessItemPolitica.sincronizaContexto(dbContext);
            //this.DataAccessSaldoItem.sincronizaContexto(dbContext);
            //this.DataAccessSituacaoTributaria.sincronizaContexto(dbContext);
            //this.DataAccessTipoDescontoEscola.sincronizaContexto(dbContext);
            //this.DataAccessItemSubgrupo.sincronizaContexto(dbContext);
            //this.DataAccessAliquotaUF.sincronizaContexto(dbContext);
            //this.DataAccessDadosNF.sincronizaContexto(dbContext);
            //this.DataAccessPoliticaTurma.sincronizaContexto(dbContext);
            //this.DataAccessPoliticaAluno.sincronizaContexto(dbContext);
            //this.DataAccessObsSaldoCaixa.sincronizaContexto(dbContext);
            //this.DataAccessReajusteAnual.sincronizaContexto(dbContext);
            //this.DataAccessReajusteAluno.sincronizaContexto(dbContext);
            //this.DataAccessReajusteTurma.sincronizaContexto(dbContext);
            //this.DataAccessReajusteCurso.sincronizaContexto(dbContext);
            //this.DataAccessReajusteTitulo.sincronizaContexto(dbContext);
            //this.DataAccessChequeTransacao.sincronizaContexto(dbContext);
            //this.DataAccessChequeBaixa.sincronizaContexto(dbContext);
            //this.DataAccessMovimento.sincronizaContexto(dbContext);
            //BusinessEmpresa.sincronizaContexto(dbContext);
        }

        #region Cadastros Básicos

        #region Grupo de Estoque

        public GrupoEstoque getGrupoEstoqueById(int id)
        {
            return DataAccessGrupoEstoque.findById(id, false);
        }

        public GrupoEstoque postGrupoEstoque(GrupoEstoque grupoEstoque, bool masterGeral)
        {
            // A categoria Privada só poderá ser incluída, editada ou deletada pelo usuário master
            if (!masterGeral)
                if (grupoEstoque.id_categoria_grupo == (int)GrupoEstoque.CategoriaEnum.PRIVADA)
                    throw new FinanceiroBusinessException(Utils.Messages.Messages.msgErroGrupoItemPrivado, null, FinanceiroBusinessException.TipoErro.ERRO_CATEGORIA_GRUPO_ITEM, false);
            DataAccessGrupoEstoque.add(grupoEstoque, false);
            return grupoEstoque;
        }
        public GrupoEstoque putGrupoEstoque(GrupoEstoque grupoEstoque, bool masterGeral)
        {
            GrupoEstoque grupoOld = DataAccessGrupoEstoque.findById(grupoEstoque.cd_grupo_estoque, false);
            // A categoria Privada só poderá ser incluída, editada ou deletada pelo usuário master
            if (!masterGeral)
            {

                if (grupoEstoque.id_categoria_grupo == (int)GrupoEstoque.CategoriaEnum.PRIVADA ||
                    (grupoOld.id_categoria_grupo == (int)GrupoEstoque.CategoriaEnum.PRIVADA && grupoEstoque.id_categoria_grupo == (int)GrupoEstoque.CategoriaEnum.PUBLICA))
                    throw new FinanceiroBusinessException(Utils.Messages.Messages.msgErroGrupoItemPrivado, null, FinanceiroBusinessException.TipoErro.ERRO_CATEGORIA_GRUPO_ITEM, false);
            }
            grupoOld.copy(grupoEstoque);
            DataAccessGrupoEstoque.saveChanges(false);
            //DataAccessGrupoEstoque.edit(grupoEstoque, false);
            return grupoOld;
        }
        public bool deleteGrupoEstoque(GrupoEstoque grupoEstoque, bool masterGeral)
        {
            bool deleted = false;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                // A categoria Privada só poderá ser incluída, editada ou deletada pelo usuário master
                if (!masterGeral)
                    if (grupoEstoque.id_categoria_grupo == (int)GrupoEstoque.CategoriaEnum.PRIVADA)
                        throw new FinanceiroBusinessException(Utils.Messages.Messages.msgErroGrupoItemPrivado, null, FinanceiroBusinessException.TipoErro.ERRO_CATEGORIA_GRUPO_ITEM, false);
                GrupoEstoque e = DataAccessGrupoEstoque.findById(grupoEstoque.cd_grupo_estoque, false);
                deleted = DataAccessGrupoEstoque.delete(e, false);
                transaction.Complete();
                return deleted;
            }
        }
        public IEnumerable<GrupoEstoque> getGrupoEstoqueSearch(SearchParameters parametros, string descricao, bool inicio, bool? status, int categoria)
        {
            IEnumerable<GrupoEstoque> retorno = new List<GrupoEstoque>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "no_grupo_estoque";
                parametros.sort = parametros.sort.Replace("grupo_estoque_ativo", "id_grupo_estoque_ativo");
                parametros.sort = parametros.sort.Replace("categoria_grupo", "id_categoria_grupo");
                parametros.sort = parametros.sort.Replace("eliminar_inventario", "id_eliminar_inventario");
                retorno = DataAccessGrupoEstoque.GetGrupoEstoqueSearch(parametros, descricao, inicio, status, categoria);
                transaction.Complete();
            }
            return retorno;
        }

        public bool deleteAllGrupo(List<GrupoEstoque> grupos, bool masterGeral)
        {
            // A categoria Master só poderá ser incluída, editada ou deletada pelo usuário master
            if (!masterGeral)
                foreach (GrupoEstoque grupoEstoque in grupos)
                    if (grupoEstoque.id_categoria_grupo == (int)GrupoEstoque.CategoriaEnum.PRIVADA)
                        throw new FinanceiroBusinessException(Utils.Messages.Messages.msgErroGrupoItemPrivado, null, FinanceiroBusinessException.TipoErro.ERRO_CATEGORIA_GRUPO_ITEM, false);
            return DataAccessGrupoEstoque.deleteAllGrupo(grupos);
        }

        public List<GrupoEstoque> findAllGrupoAtivo(int cdGrupo, bool isMasterGeral)
        {
            return DataAccessGrupoEstoque.findAllGrupoAtivo(cdGrupo, isMasterGeral);
        }

        public List<GrupoEstoque> findAllGrupoWithItem(int cd_pessoa_escola, bool isMaster)
        {
            List<GrupoEstoque> lista = new List<GrupoEstoque>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED, DataAccessGrupoEstoque.DB(), TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                lista = DataAccessGrupoEstoque.findAllGrupoWithItem(cd_pessoa_escola, isMaster).ToList();
                transaction.Complete();
            }
            return lista;
        }

        #endregion

        #region MovimentacaoFinanceira

        public MovimentacaoFinanceira getMovimentacaoFinanceiraById(int id)
        {
            return DataAccessMovimentacaoFinanceira.findById(id, false);
        }

        public MovimentacaoFinanceira postMovimentacaoFinanceira(MovimentacaoFinanceira movimentacaofinanceira)
        {
            DataAccessMovimentacaoFinanceira.add(movimentacaofinanceira, false);
            return movimentacaofinanceira;
        }
        public MovimentacaoFinanceira putMovimentacaoFinanceira(MovimentacaoFinanceira movimentacaofinanceira)
        {
            if (movimentacaofinanceira.cd_movimentacao_financeira >= 1 && movimentacaofinanceira.cd_movimentacao_financeira <= 5)
                throw new RegistroProprietarioBusinessException(Componentes.Utils.Messages.Messages.msgErroRegProp, null, RegistroProprietarioBusinessException.TipoErro.REGISTRO_PROPRIETARIO, false);

            DataAccessMovimentacaoFinanceira.edit(movimentacaofinanceira, false);
            return movimentacaofinanceira;
        }

        public bool deleteMovimentacaoFinanceira(MovimentacaoFinanceira movimentacaofinanceira)
        {
            bool deleted = false;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                if (movimentacaofinanceira.cd_movimentacao_financeira >= 1 && movimentacaofinanceira.cd_movimentacao_financeira <= 5)
                    throw new RegistroProprietarioBusinessException(Componentes.Utils.Messages.Messages.msgErroRegProp, null, RegistroProprietarioBusinessException.TipoErro.REGISTRO_PROPRIETARIO, false);

                MovimentacaoFinanceira e = DataAccessMovimentacaoFinanceira.findById(movimentacaofinanceira.cd_movimentacao_financeira, false);
                deleted = DataAccessMovimentacaoFinanceira.delete(e, false);
                transaction.Complete();
                return deleted;
            }
        }

        public IEnumerable<MovimentacaoFinanceira> getMovimentacaoFinanceiraSearch(SearchParameters parametros, string descricao, bool inicio, bool? status)
        {
            IEnumerable<MovimentacaoFinanceira> retorno = new List<MovimentacaoFinanceira>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "dc_movimentacao_financeira";
                parametros.sort = parametros.sort.Replace("mov_financeira_ativa", "id_mov_financeira_ativa");
                retorno = DataAccessMovimentacaoFinanceira.GetMovimentacaoFinanceiraSearch(parametros, descricao, inicio, status);
                transaction.Complete();
            }
            return retorno;
        }

        public bool deleteAllMovimentacao(List<MovimentacaoFinanceira> movimentacoes)
        {
            foreach (MovimentacaoFinanceira e in movimentacoes)
                if (e.cd_movimentacao_financeira >= 1 && e.cd_movimentacao_financeira <= 5)
                    throw new RegistroProprietarioBusinessException(Componentes.Utils.Messages.Messages.msgErroRegProp, null, RegistroProprietarioBusinessException.TipoErro.REGISTRO_PROPRIETARIO, false);

            return DataAccessMovimentacaoFinanceira.deleteAllMovimentacao(movimentacoes);
        }

        public IEnumerable<MovimentacaoFinanceira> getMovimentacaoWithContaCorrente(int cd_pessoa_escola)
        {
            return DataAccessMovimentacaoFinanceira.getMovimentacaoWithContaCorrente(cd_pessoa_escola);
        }

        public IEnumerable<MovimentacaoFinanceira> getMovimentacaoAtivaWithConta(int cd_pessoa_escola, bool ativos)
        {
            return DataAccessMovimentacaoFinanceira.getMovimentacaoAtivaWithConta(cd_pessoa_escola, ativos);
        }

        public IEnumerable<MovimentacaoFinanceira> getMovimentacaoAtivaWithConta(int cd_pessoa_escola, int cd_movimentacao_financeira)
        {
            return DataAccessMovimentacaoFinanceira.getMovimentacaoAtivaWithConta(cd_pessoa_escola, cd_movimentacao_financeira);
        }

        public IEnumerable<MovimentacaoFinanceira> getMovimentacaoTransferencia(int cd_pessoa_escola, int cd_movimentacao_financeira)
        {
            return DataAccessMovimentacaoFinanceira.getMovimentacaoTransferencia(cd_pessoa_escola, cd_movimentacao_financeira);
        }

        #endregion

        #region Tipo Liquidação

        public TipoLiquidacao getTipoLiquidacaoById(int id)
        {
            return DataAccessTipoLiquidacao.findById(id, false);
        }

        public TipoLiquidacao postTipoLiquidacao(TipoLiquidacao tipoliquidacao)
        {
            DataAccessTipoLiquidacao.add(tipoliquidacao, false);
            return tipoliquidacao;
        }

        public TipoLiquidacao putTipoLiquidacao(TipoLiquidacao tipoliquidacao)
        {
            if (tipoliquidacao.cd_tipo_liquidacao >= 1 && tipoliquidacao.cd_tipo_liquidacao <= 7 || tipoliquidacao.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA ||
                tipoliquidacao.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA_ADITIVO)
                throw new RegistroProprietarioBusinessException(Componentes.Utils.Messages.Messages.msgErroRegProp, null, RegistroProprietarioBusinessException.TipoErro.REGISTRO_PROPRIETARIO, false);

            DataAccessTipoLiquidacao.edit(tipoliquidacao, false);
            return tipoliquidacao;
        }

        public bool deleteTipoLiquidacao(TipoLiquidacao tipoliquidacao)
        {
            if (tipoliquidacao.cd_tipo_liquidacao >= 1 && tipoliquidacao.cd_tipo_liquidacao <= 7 || tipoliquidacao.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA ||
                tipoliquidacao.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA_ADITIVO)
                throw new RegistroProprietarioBusinessException(Componentes.Utils.Messages.Messages.msgErroRegProp, null, RegistroProprietarioBusinessException.TipoErro.REGISTRO_PROPRIETARIO, false);

            TipoLiquidacao e = DataAccessTipoLiquidacao.findById(tipoliquidacao.cd_tipo_liquidacao, false);
            var deleted = DataAccessTipoLiquidacao.delete(e, false);
            return deleted;
        }

        public IEnumerable<TipoLiquidacao> getTipoLiquidacaoSearch(SearchParameters parametros, string descricao, bool inicio, bool? status)
        {
            IEnumerable<TipoLiquidacao> retorno = new List<TipoLiquidacao>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "dc_tipo_liquidacao";
                parametros.sort = parametros.sort.Replace("tipo_liquidacao_ativa", "id_tipo_liquidacao_ativa");
                retorno = DataAccessTipoLiquidacao.GetTipoLiquidacaoSearch(parametros, descricao, inicio, status);
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<OrgaoFinanceiro> getOrgaoFinanceiroSearch(SearchParameters parametros, string descricao, bool inicio, bool? status)
        {
            IEnumerable<OrgaoFinanceiro> retorno = new List<OrgaoFinanceiro>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "dc_orgao_financeiro";
                parametros.sort = parametros.sort.Replace("id_orgao_ativo", "id_orgao_financeiro_ativo");
                retorno = DataAccessOrgaoFinanceiro.getOrgaoFinanceiroSearch(parametros, descricao, inicio, status);
                transaction.Complete();
            }
            return retorno;
        }

        public OrgaoFinanceiro postOrgaoFinanceiro(OrgaoFinanceiro orgaoFinanceiro)
        {
            DataAccessOrgaoFinanceiro.add(orgaoFinanceiro, false);
            return orgaoFinanceiro;
        }

        public OrgaoFinanceiro putOrgaoFinanceiro(OrgaoFinanceiro orgaoFinanceiro)
        {
            //if (orgaoFinanceiro.cd_orgao_financeiro >= 1 && orgaoFinanceiro.cd_orgao_financeiro <= 4)
            //    throw new RegistroProprietarioBusinessException(Componentes.Utils.Messages.Messages.msgErroRegProp, null, RegistroProprietarioBusinessException.TipoErro.REGISTRO_PROPRIETARIO, false);

            DataAccessOrgaoFinanceiro.edit(orgaoFinanceiro, false);
            return orgaoFinanceiro;
        }


        public bool deleteAllOrgaoFinanceiro(List<OrgaoFinanceiro> orgaosFinanceiros)
        {
            //foreach (OrgaoFinanceiro e in orgaosFinanceiros)
            //    if (e.cd_orgao_financeiro >= 1 && e.cd_orgao_financeiro <= 4)
            //        throw new RegistroProprietarioBusinessException(Componentes.Utils.Messages.Messages.msgErroRegProp, null, RegistroProprietarioBusinessException.TipoErro.REGISTRO_PROPRIETARIO, false);

            return DataAccessOrgaoFinanceiro.deleteAllOrgaoFinanceiro(orgaosFinanceiros);
        }
        public IEnumerable<OrgaoFinanceiro> getAllOrgaoFinanceiro()
        {
            IEnumerable<OrgaoFinanceiro> retorno = new List<OrgaoFinanceiro>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = DataAccessOrgaoFinanceiro.getAllOrgaoFinanceiro();
                transaction.Complete();
            }
            return retorno;
        }

        public bool deleteAllTipoLiquidacao(List<TipoLiquidacao> tiposLiquidacao)
        {
            bool deleted = false;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                foreach (TipoLiquidacao e in tiposLiquidacao)
                    if (e.cd_tipo_liquidacao >= 1 && e.cd_tipo_liquidacao <= 7)
                        throw new RegistroProprietarioBusinessException(Componentes.Utils.Messages.Messages.msgErroRegProp, null, RegistroProprietarioBusinessException.TipoErro.REGISTRO_PROPRIETARIO, false);

                deleted = DataAccessTipoLiquidacao.deleteAllTipoLiquidacao(tiposLiquidacao);
                transaction.Complete();
                return deleted;
            }
        }

        public IEnumerable<TipoLiquidacao> getTipoLiquidacao(TipoLiquidacaoDataAccess.TipoConsultaTipoLiquidacaoEnum hasDependente, int? cd_tipo_liquidacao)
        {
            IEnumerable<TipoLiquidacao> retorno = new List<TipoLiquidacao>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = DataAccessTipoLiquidacao.getTipoLiquidacao(hasDependente, cd_tipo_liquidacao).ToList();
                transaction.Complete();
            }
            return retorno;
        }

        #endregion

        #region Tipo Financeiro

        public TipoFinanceiro getTipoFinanceiroById(int id)
        {
            return DataAccessTipoFinanceiro.findById(id, false);
        }

        public TipoFinanceiro postTipoFinanceiro(TipoFinanceiro tipofinanceiro)
        {
            DataAccessTipoFinanceiro.add(tipofinanceiro, false);
            return tipofinanceiro;
        }

        public TipoFinanceiro putTipoFinanceiro(TipoFinanceiro tipofinanceiro)
        {
            if (tipofinanceiro.cd_tipo_financeiro >= 1 && tipofinanceiro.cd_tipo_financeiro <= 4)
                throw new RegistroProprietarioBusinessException(Componentes.Utils.Messages.Messages.msgErroRegProp, null, RegistroProprietarioBusinessException.TipoErro.REGISTRO_PROPRIETARIO, false);

            DataAccessTipoFinanceiro.edit(tipofinanceiro, false);
            return tipofinanceiro;
        }

        public bool deleteTipoFinanceiro(TipoFinanceiro tipofinanceiro)
        {
            bool deleted = false;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                if (tipofinanceiro.cd_tipo_financeiro >= 1 && tipofinanceiro.cd_tipo_financeiro <= 4)
                    throw new RegistroProprietarioBusinessException(Componentes.Utils.Messages.Messages.msgErroRegProp, null, RegistroProprietarioBusinessException.TipoErro.REGISTRO_PROPRIETARIO, false);

                TipoFinanceiro e = DataAccessTipoFinanceiro.findById(tipofinanceiro.cd_tipo_financeiro, false);
                deleted = DataAccessTipoFinanceiro.delete(e, false);
                transaction.Complete();
                return deleted;
            }
        }

        public IEnumerable<TipoFinanceiro> getTipoFinanceiroSearch(SearchParameters parametros, string descricao, bool inicio, bool? status)
        {
            IEnumerable<TipoFinanceiro> retorno = new List<TipoFinanceiro>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "dc_tipo_financeiro";
                parametros.sort = parametros.sort.Replace("tipo_financeiro_ativo", "id_tipo_financeiro_ativo");
                retorno = DataAccessTipoFinanceiro.GetTipoFinanceiroSearch(parametros, descricao, inicio, status);
                transaction.Complete();
            }
            return retorno;
        }

        public bool deleteAllTipoFinanceiro(List<TipoFinanceiro> tiposFinanceiros)
        {
            foreach (TipoFinanceiro e in tiposFinanceiros)
                if (e.cd_tipo_financeiro >= 1 && e.cd_tipo_financeiro <= 4)
                    throw new RegistroProprietarioBusinessException(Componentes.Utils.Messages.Messages.msgErroRegProp, null, RegistroProprietarioBusinessException.TipoErro.REGISTRO_PROPRIETARIO, false);

            return DataAccessTipoFinanceiro.deleteAllTipoFinanceiro(tiposFinanceiros);
        }

        public List<TipoFinanceiro> getTipoFinanceiroAtivo()
        {
            return DataAccessTipoFinanceiro.getTipoFinanceiroAtivo();
        }

        public IEnumerable<TipoFinanceiro> getTipoFinanceiroMovimento(int cd_tipo_finan, int cd_empresa, int id_tipo_movto)
        {
            return DataAccessTipoFinanceiro.getTipoFinanceiroMovimento(cd_tipo_finan, cd_empresa, id_tipo_movto);
        }

        public IEnumerable<TipoFinanceiro> getTipoFinanceiro(int cd_tipo_finan, TipoFinanceiroDataAccess.TipoConsultaTipoFinanEnum tipoConsulta)
        {
            return DataAccessTipoFinanceiro.getTipoFinanceiro(cd_tipo_finan, tipoConsulta);
        }

        #endregion

        #region Tipo Desconto

        public TipoDesconto getTipoDescontoByIdComTipoDescontoEscola(int id)
        {
            return DataAccessTipoDesconto.getTipoDescontoByIdComTipoDescontoEscola(id);
        }

        public TipoDesconto getTipoDescontoComTipoDescontoEscola(int cd_escola, int cd_tipo_desconto)
        {
            return DataAccessTipoDesconto.getTipoDescontoComTipoDescontoEscola(cd_escola, cd_tipo_desconto);
        }

        public TipoDescontoUI postTipoDesconto(TipoDescontoUI tipoDesconto, int cdEscola, ICollection<Escola> escolasUsuario, bool isMasterGeral)
        {
            TipoDesconto novoTipo = new TipoDesconto();
            TipoDescontoUI tipoDescontoRet = new TipoDescontoUI();

            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED, DataAccessTipoDesconto.DB(), TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                this.sincronizarContextos(DataAccessTipoDesconto.DB());
                //Persistir um item
                novoTipo.copy(tipoDesconto);
                novoTipo.id_master = isMasterGeral;
                TipoDescontoEscola tipoDescontoEscola = new TipoDescontoEscola();
                if (escolasUsuario != null && escolasUsuario.Count() > 0)
                {
                    foreach (var escolas in escolasUsuario)
                        novoTipo.TiposDescontoEscola.Add(
                            new TipoDescontoEscola
                            {
                                cd_pessoa_escola = escolas.cd_pessoa,
                                cd_tipo_desconto = novoTipo.cd_tipo_desconto,
                                id_incide_baixa = tipoDesconto.id_incide_baixa,
                                id_incide_parcela_1 = tipoDesconto.id_incide_parcela_1,
                                id_tipo_desconto_ativo = tipoDesconto.id_tipo_desconto_ativo,
                                pc_desconto = tipoDesconto.pc_desconto
                            });
                }
                bool existeEsc = DataAccessTipoDesconto.getTipoDescontoNomeEsc(tipoDesconto.dc_tipo_desconto, cdEscola);
                if (existeEsc)
                    throw new FinanceiroBusinessException(Utils.Messages.Messages.msgExisteTpDescEsc, null, FinanceiroBusinessException.TipoErro.ERRO_EXISTE_TP_DESCONTO_ESC, false);
                bool existeEmOutraEsc = DataAccessTipoDesconto.getTipoDescontoNome(tipoDesconto.dc_tipo_desconto);
                if (existeEmOutraEsc)
                    throw new FinanceiroBusinessException(Utils.Messages.Messages.msgExisteTpDesc, null, FinanceiroBusinessException.TipoErro.ERRO_EXISTE_TP_DESCONTO, false);
                novoTipo = DataAccessTipoDesconto.add(novoTipo, false);



                transaction.Complete();
            }
            tipoDescontoRet = DataAccessTipoDesconto.getTipoDescontoUIById(novoTipo.cd_tipo_desconto, cdEscola, isMasterGeral);

            return tipoDescontoRet;
        }

        public TipoDescontoUI putTipoDesconto(TipoDescontoUI tipoDesconto, int cdEscola, ICollection<Escola> escolasUsuario, bool isMasterGeral, ICollection<EmpresaSession> listEmp)
        {
            TipoDescontoUI tpDescUI = new TipoDescontoUI();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                this.sincronizarContextos(DataAccessTipoDesconto.DB());
                TipoDesconto tpsDes = new TipoDesconto();
                List<int> cdsListEmpUsuario = listEmp.Select(o => o.cd_pessoa).ToList();
                tpsDes = DataAccessTipoDesconto.findById(tipoDesconto.cd_tipo_desconto, false);
                IEnumerable<Escola> escolasView = escolasUsuario.ToList();
                List<int> cdsEscolasView = escolasView.Select(o => o.cd_pessoa).ToList();
                IEnumerable<int> codEscolasBase = DataAccessTipoDescontoEscola.getTipoDescontoWithEscola(tpsDes.cd_tipo_desconto).ToList();

                TipoDescontoEscola tpDescEsc = DataAccessTipoDescontoEscola.findTpDescEscolabyId(tpsDes.cd_tipo_desconto, cdEscola);
                bool existeEscolaLogin;
                if (tipoDesconto.hasClickEscola)
                    existeEscolaLogin = escolasView.Where(e => e.cd_pessoa == cdEscola).Any();
                else
                    existeEscolaLogin = codEscolasBase.Where(e => e == cdEscola).Any();
                if (isMasterGeral && (tpDescEsc == null || tpDescEsc.cd_tipo_desconto <= 0) && !existeEscolaLogin)
                    throw new FinanceiroBusinessException(Utils.Messages.Messages.msgErroMasterVincularDesEsc, null, FinanceiroBusinessException.TipoErro.ERRO_MASTER_SEM_ESCOLA_LOGADA_VINCULADA, false);
                if (tpsDes.dc_tipo_desconto != tipoDesconto.dc_tipo_desconto)
                {
                    if (!isMasterGeral)
                    {
                        bool escolasDif;
                        if (tipoDesconto.hasClickEscola)
                            escolasDif = cdsEscolasView.Where(e => !cdsListEmpUsuario.Contains(e)).Count() > 0;
                        else
                            escolasDif = codEscolasBase.Where(e => !cdsListEmpUsuario.Contains(e)).Count() > 0;
                        if (escolasDif)
                            throw new FinanceiroBusinessException(Utils.Messages.Messages.msgErroTpDescAlterado, null, FinanceiroBusinessException.TipoErro.ERRO_TP_DESC_USADO_OUTRAS_ESC, false);
                    }
                    bool master = DataAccessTipoDesconto.getTipoDescontoMaster(tipoDesconto.cd_tipo_desconto);
                    if (master != isMasterGeral)
                        if (master && !isMasterGeral)
                            throw new FinanceiroBusinessException(Utils.Messages.Messages.msgErroEdicaoFundacao, null, FinanceiroBusinessException.TipoErro.ERRO_TP_DESC_INCLUIDO_USUARIO_COMUM, false);
                        else
                            if (!master && isMasterGeral)
                                throw new FinanceiroBusinessException(Utils.Messages.Messages.msgErroTpDescMaster, null, FinanceiroBusinessException.TipoErro.ERRO_TP_DESC_INCLUIDO_MASTER, false);

                }

                tpsDes.dc_tipo_desconto = tipoDesconto.dc_tipo_desconto;
                DataAccessTipoDesconto.saveChanges(false);
                if (tpDescEsc != null && tpDescEsc.cd_tipo_desconto > 0)
                {
                    tpDescEsc.id_incide_baixa = tipoDesconto.id_incide_baixa;
                    tpDescEsc.id_incide_parcela_1 = tipoDesconto.id_incide_parcela_1;
                    tpDescEsc.id_tipo_desconto_ativo = tipoDesconto.id_tipo_desconto_ativo;
                    tpDescEsc.pc_desconto = tipoDesconto.pc_desconto;
                    DataAccessTipoDescontoEscola.saveChanges(false);
                }
                //Se não for master geral e não existir na TipoDescontoEscola, criar
                if (!isMasterGeral && (tpDescEsc == null || tpDescEsc.cd_tipo_desconto <= 0))
                {
                    TipoDescontoEscola novo = new TipoDescontoEscola
                                    {
                                        cd_pessoa_escola = cdEscola,
                                        cd_tipo_desconto = tipoDesconto.cd_tipo_desconto,
                                        id_incide_baixa = tipoDesconto.id_incide_baixa,
                                        id_incide_parcela_1 = tipoDesconto.id_incide_parcela_1,
                                        id_tipo_desconto_ativo = tipoDesconto.id_tipo_desconto_ativo,
                                        pc_desconto = tipoDesconto.pc_desconto
                                    };

                    DataAccessTipoDescontoEscola.add(novo, false);
                }

                if ((escolasView != null) && (tipoDesconto.hasClickEscola))
                {
                    IEnumerable<int> codEscolasVW = escolasView.Select(ev => ev.cd_pessoa);

                    //Insere as Escolas da view que foram adcionadas na grade e não estão na base de dados
                    IEnumerable<int> escolasInserirVW = codEscolasVW.Except(codEscolasBase);
                    TipoDescontoEscola novoTpDescEscola = new TipoDescontoEscola();
                    if (isMasterGeral)
                    {
                        List<TipoDescontoEscola> tpDescAlterar = DataAccessTipoDescontoEscola.getTpDescEscolaByTpDesc(tipoDesconto.cd_tipo_desconto).ToList();

                        foreach (var itemAlterar in tpDescAlterar)
                            if (itemAlterar.cd_pessoa_escola == cdEscola)
                            {
                                itemAlterar.id_incide_baixa = tipoDesconto.id_incide_baixa;
                                itemAlterar.id_incide_parcela_1 = tipoDesconto.id_incide_parcela_1;
                                itemAlterar.id_tipo_desconto_ativo = tipoDesconto.id_tipo_desconto_ativo;
                                itemAlterar.pc_desconto = tipoDesconto.pc_desconto;
                            }
                        DataAccessTipoDescontoEscola.saveChanges(false);
                    }

                    if (escolasInserirVW != null && escolasInserirVW.Count() > 0)
                    {
                        foreach (int cdItemEscolaVW in escolasInserirVW)
                        {
                            novoTpDescEscola = new TipoDescontoEscola
                            {
                                cd_pessoa_escola = cdItemEscolaVW,
                                cd_tipo_desconto = tipoDesconto.cd_tipo_desconto,
                                id_incide_baixa = tipoDesconto.id_incide_baixa,
                                id_incide_parcela_1 = tipoDesconto.id_incide_parcela_1,
                                id_tipo_desconto_ativo = tipoDesconto.id_tipo_desconto_ativo,
                                pc_desconto = tipoDesconto.pc_desconto
                            };
                            DataAccessTipoDescontoEscola.addContext(novoTpDescEscola, false);
                        }
                        DataAccessTipoDescontoEscola.saveChanges(false);
                    }

                    //Deletar as escolas que estão no banco e não estão na view.
                    IEnumerable<int> deletarEscolas = codEscolasBase.Except(codEscolasVW);
                    if (deletarEscolas != null && deletarEscolas.Count() > 0)
                    {
                        foreach (var itemEscola in deletarEscolas)
                        {
                            //verifica se existe matricula usando o desconto, se existir, não permitir excluir
                            bool existe = DataAccessTipoDescontoEscola.existeContatoDesconto(tipoDesconto.cd_tipo_desconto, itemEscola);
                            if (existe)
                                throw new FinanceiroBusinessException(Utils.Messages.Messages.msgErroTpDescContrato, null, FinanceiroBusinessException.TipoErro.ERRO_EXISTE_CONTATO_DESCONTO, false);
                            TipoDescontoEscola itemEscolaDeletar = DataAccessTipoDescontoEscola.findTpDescEscolabyId(tipoDesconto.cd_tipo_desconto, itemEscola);
                            DataAccessTipoDescontoEscola.deleteContext(itemEscolaDeletar, false);
                        }
                        DataAccessTipoDescontoEscola.saveChanges(false);
                    }
                }

                tpDescUI = DataAccessTipoDesconto.getTipoDescontoUIById(tipoDesconto.cd_tipo_desconto, cdEscola, isMasterGeral);
                transaction.Complete();
            }
            return tpDescUI;

            //DataAccessTipoDesconto.edit(tipodesconto, false);
            //return tipodesconto;
        }
        public bool deleteTipoDesconto(TipoDesconto tipodesconto)
        {
            bool deleted = false;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                this.sincronizarContextos(DataAccessTipoDesconto.DB());
                //verifica se existe matricula usando o desconto, se existir, não permitir excluir
                bool existe = DataAccessTipoDescontoEscola.existeContatoDesconto(tipodesconto.cd_tipo_desconto, 0);
                if (existe)
                    throw new FinanceiroBusinessException(Utils.Messages.Messages.msgErroTpDescContrato, null, FinanceiroBusinessException.TipoErro.ERRO_EXISTE_CONTATO_DESCONTO, false);
                TipoDesconto e = DataAccessTipoDesconto.findById(tipodesconto.cd_tipo_desconto, false);
                deleted = DataAccessTipoDesconto.delete(e, false);
                transaction.Complete();
                return deleted;
            }
        }
        public IEnumerable<TipoDescontoUI> getTipoDescontoSearch(SearchParameters parametros, string descricao, bool inicio, bool? status, bool? incideBaixa, bool? pparc, decimal? percentual, int cdEscola)
        {
            IEnumerable<TipoDescontoUI> retorno = new List<TipoDescontoUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "dc_tipo_desconto";
                parametros.sort = parametros.sort.Replace("tipo_desconto_ativo", "id_tipo_desconto_ativo");
                parametros.sort = parametros.sort.Replace("incide_baixa", "id_incide_baixa");
                parametros.sort = parametros.sort.Replace("incide_parcela_1", "id_incide_parcela_1");
                parametros.sort = parametros.sort.Replace("pc_desc", "pc_desconto");
                retorno = DataAccessTipoDesconto.GetTipoDescontoSearch(parametros, descricao, inicio, status, incideBaixa, pparc, percentual, cdEscola);
                transaction.Complete();
            }
            return retorno;
        }

        public bool deleteAllTipoDesconto(List<TipoDescontoUI> tiposDesconto, bool isMasterGeral, int cdEscola, ICollection<EmpresaSession> listEmp)
        {
            bool deleted = false;
            bool deletedTp = false;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {

                foreach (TipoDescontoUI tp in tiposDesconto)
                {
                    List<TipoDescontoEscola> tipoDescontoEsc = DataAccessTipoDescontoEscola.getTpDescEscolaByTpDesc(tp.cd_tipo_desconto).ToList();

                    if (tipoDescontoEsc != null && tipoDescontoEsc.Count() > 0)
                    {
                        if (!isMasterGeral)
                        {
                            //Se tiver só a escola logada, deletar TipoDescontoEscola e TipoDesconto
                            if (tipoDescontoEsc.Count() == 1)
                            {

                                verificarTipoDescontoMaster(tp.cd_tipo_desconto, isMasterGeral);
                                TipoDescontoEscola tpDescDeletar = DataAccessTipoDescontoEscola.findTpDescEscolabyId(tp.cd_tipo_desconto, cdEscola);
                                
                                //verifica se existe matricula usando o desconto, se existir, não permitir excluir
                                bool existe = DataAccessTipoDescontoEscola.existeContatoDesconto(tp.cd_tipo_desconto, cdEscola);
                                if (existe)
                                    throw new FinanceiroBusinessException(Utils.Messages.Messages.msgErroUsandoTipoDesconto, null, FinanceiroBusinessException.TipoErro.ERRO_USANDO_TIPO_DESCONTO, false);

                                if (tpDescDeletar != null && tpDescDeletar.cd_pessoa_escola > 0)
                                    deletedTp = DataAccessTipoDescontoEscola.delete(tpDescDeletar, false);
                                else
                                    //ERRO: TIPO DE DESCONTO NÃO PERTENCE A ESCOLA LOGADA
                                    throw new FinanceiroBusinessException(Utils.Messages.Messages.msgErroTpDescEscola, null, FinanceiroBusinessException.TipoErro.ERRO_EXCLUIR_TP_DESC_ESCOLA, false);

                                TipoDesconto delTipo = DataAccessTipoDesconto.findById(tp.cd_tipo_desconto, false);

                                deleted = DataAccessTipoDesconto.delete(delTipo, false);
                            }
                            else
                                //Se não tiver nenhuma escola, deeleta o Tipo Desconto
                                if (tipoDescontoEsc.Count() == 0)
                                {

                                    verificarTipoDescontoMaster(tp.cd_tipo_desconto, isMasterGeral);
                                    TipoDesconto deletar = DataAccessTipoDesconto.findById(tp.cd_tipo_desconto, false);
                                    deleted = DataAccessTipoDesconto.delete(deletar, false);
                                }
                                else
                                {
                                    //Se tiver mais de uma escola e não for master, deleta só a ligação na escola logada (Não deleta tipo desconto)
                                    TipoDescontoEscola tpDescDeletar = DataAccessTipoDescontoEscola.findTpDescEscolabyId(tp.cd_tipo_desconto, cdEscola);

                                    //verifica se existe matricula usando o desconto, se existir, não permitir excluir
                                    bool existe = DataAccessTipoDescontoEscola.existeContatoDesconto(tp.cd_tipo_desconto, cdEscola);
                                    if (existe)
                                        throw new FinanceiroBusinessException(Utils.Messages.Messages.msgErroUsandoTipoDesconto, null, FinanceiroBusinessException.TipoErro.ERRO_USANDO_TIPO_DESCONTO, false);

                                    if (tpDescDeletar != null && tpDescDeletar.cd_pessoa_escola > 0)
                                        deletedTp = DataAccessTipoDescontoEscola.delete(tpDescDeletar, false);
                                    else
                                        //ERRO: TIPO DE DESCONTO NÃO PERTENCE A ESCOLA LOGADA
                                        throw new FinanceiroBusinessException(Utils.Messages.Messages.msgErroTpDescEscola, null, FinanceiroBusinessException.TipoErro.ERRO_EXCLUIR_TP_DESC_ESCOLA, false);
                                }
                        }
                        else
                        {
                            //Se tiver só 1 escola, deletar TipoDescontoEscola e TipoDesconto
                            if (tipoDescontoEsc.Count() == 1)
                            {

                                verificarTipoDescontoMaster(tp.cd_tipo_desconto, isMasterGeral);
                                TipoDescontoEscola tpDescDeletar = DataAccessTipoDescontoEscola.findTpDescEscolabyId(tp.cd_tipo_desconto, tipoDescontoEsc[0].cd_pessoa_escola);

                                //verifica se existe matricula usando o desconto, se existir, não permitir excluir
                                bool existe = DataAccessTipoDescontoEscola.existeContatoDesconto(tp.cd_tipo_desconto, cdEscola);
                                if (existe)
                                    throw new FinanceiroBusinessException(Utils.Messages.Messages.msgErroUsandoTipoDesconto, null, FinanceiroBusinessException.TipoErro.ERRO_USANDO_TIPO_DESCONTO, false);

                                if (tpDescDeletar != null && tpDescDeletar.cd_pessoa_escola > 0)
                                    deletedTp = DataAccessTipoDescontoEscola.delete(tpDescDeletar, false);

                                TipoDesconto delTipo = DataAccessTipoDesconto.findById(tp.cd_tipo_desconto, false);
                                deleted = DataAccessTipoDesconto.delete(delTipo, false);
                            }
                            else
                            {

                                verificarTipoDescontoMaster(tp.cd_tipo_desconto, isMasterGeral);
                                if ((tp.escolas == null || tp.escolas.Count() == 0) && tp.hasClickEscola)
                                {
                                    foreach (TipoDescontoEscola tpe in tipoDescontoEsc)
                                    {
                                        TipoDescontoEscola tpDescDeletar = DataAccessTipoDescontoEscola.findTpDescEscolabyId(tp.cd_tipo_desconto, tpe.cd_pessoa_escola);

                                        //verifica se existe matricula usando o desconto, se existir, não permitir excluir
                                        bool existe = DataAccessTipoDescontoEscola.existeContatoDesconto(tp.cd_tipo_desconto, cdEscola);
                                        if (existe)
                                            throw new FinanceiroBusinessException(Utils.Messages.Messages.msgErroUsandoTipoDesconto, null, FinanceiroBusinessException.TipoErro.ERRO_USANDO_TIPO_DESCONTO, false);

                                        if (tpDescDeletar != null && tpDescDeletar.cd_pessoa_escola > 0)
                                            deletedTp = DataAccessTipoDescontoEscola.delete(tpDescDeletar, false);
                                    }
                                    TipoDesconto deletar = DataAccessTipoDesconto.getTipoDescontoByIdComTipoDescontoEscola(tp.cd_tipo_desconto);
                                    deleted = DataAccessTipoDesconto.delete(deletar, false);
                                }
                                else
                                {
                                    if (tipoDescontoEsc.Count() > 1)
                                        //Msg de erro falando q precisa deletar as escolas do item primeiro, caso queira desvincular o tipo de desconto de alguma escola, deletar pela grade "Escola" e salvar
                                        throw new FinanceiroBusinessException(Utils.Messages.Messages.msgErroTpDesc, null, FinanceiroBusinessException.TipoErro.ERRO_EXCLUIR_TP_DESC, false);
                                    else
                                    {
                                        if (tipoDescontoEsc.Count() == 1)
                                            deletedTp = DataAccessTipoDescontoEscola.delete(tipoDescontoEsc[0], false);
                                        TipoDesconto deletar = DataAccessTipoDesconto.getTipoDescontoByIdComTipoDescontoEscola(tp.cd_tipo_desconto);
                                        deleted = DataAccessTipoDesconto.delete(deletar, false);
                                    }
                                }
                            }

                        }
                    }
                    else
                    {
                        verificarTipoDescontoMaster(tp.cd_tipo_desconto, isMasterGeral);
                        TipoDesconto deletar = DataAccessTipoDesconto.getTipoDescontoByIdComTipoDescontoEscola(tp.cd_tipo_desconto);
                        deleted = DataAccessTipoDesconto.delete(deletar, false);
                    }

                }
                transaction.Complete();
                return deleted || deletedTp;
            }
        }
        private void verificarTipoDescontoMaster(int cd_tipo_desconto, bool isMasterGeral)
        {

            bool master = DataAccessTipoDesconto.getTipoDescontoMaster(cd_tipo_desconto);
            if (master != isMasterGeral)
                if (master && !isMasterGeral)
                    throw new FinanceiroBusinessException(Utils.Messages.Messages.msgErroEdicaoFundacao, null, FinanceiroBusinessException.TipoErro.ERRO_TP_DESC_INCLUIDO_USUARIO_COMUM, false);
                else
                    if (!master && isMasterGeral)
                        throw new FinanceiroBusinessException(Utils.Messages.Messages.msgErroDelTpDescMaster, null, FinanceiroBusinessException.TipoErro.ERRO_TP_DESC_INCLUIDO_MASTER, false);
        }
        #endregion

        #region Grupo de Contas

        public GrupoConta getGrupoContaById(int id)
        {
            return DataAccessGrupoConta.findById(id, false);
        }

        public GrupoConta postGrupoConta(GrupoConta grupoConta)
        {
            DataAccessGrupoConta.add(grupoConta, false);
            return grupoConta;
        }
        public GrupoConta putGrupoConta(GrupoConta grupoConta)
        {
            if (grupoConta.cd_grupo_conta >= 1 && grupoConta.cd_grupo_conta <= 3)
                throw new RegistroProprietarioBusinessException(Componentes.Utils.Messages.Messages.msgErroRegProp, null, RegistroProprietarioBusinessException.TipoErro.REGISTRO_PROPRIETARIO, false);

            DataAccessGrupoConta.edit(grupoConta, false);
            return grupoConta;
        }
        public bool deleteGrupoConta(GrupoConta grupoConta)
        {
            bool deleted = false;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                if (grupoConta.cd_grupo_conta >= 1 && grupoConta.cd_grupo_conta <= 3)
                    throw new RegistroProprietarioBusinessException(Componentes.Utils.Messages.Messages.msgErroRegProp, null, RegistroProprietarioBusinessException.TipoErro.REGISTRO_PROPRIETARIO, false);

                GrupoConta e = DataAccessGrupoConta.findById(grupoConta.cd_grupo_conta, false);
                deleted = DataAccessGrupoConta.delete(e, false);
                transaction.Complete();
                return deleted;
            }
        }
        public IEnumerable<GrupoConta> getGrupoContaSearch(SearchParameters parametros, string descricao, bool inicio, int tipoGrupo)
        {
            IEnumerable<GrupoConta> retorno = new List<GrupoConta>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "nm_ordem_grupo";
                parametros.sort = parametros.sort.Replace("tipo", "id_tipo_grupo_conta");
                retorno = DataAccessGrupoConta.GetGrupoContaSearch(parametros, descricao, inicio, tipoGrupo);
                transaction.Complete();
            }
            return retorno;
        }

        public bool deleteAllGrupoConta(List<GrupoConta> grupoConta)
        {
            foreach (GrupoConta e in grupoConta)
                if (e.cd_grupo_conta >= 1 && e.cd_grupo_conta <= 3)
                    throw new RegistroProprietarioBusinessException(Componentes.Utils.Messages.Messages.msgErroRegProp, null, RegistroProprietarioBusinessException.TipoErro.REGISTRO_PROPRIETARIO, false);

            return DataAccessGrupoConta.deleteAllGrupoConta(grupoConta);
        }

        public IEnumerable<GrupoConta> getAllGrupoConta()
        {
            return DataAccessGrupoConta.findAll(false).OrderBy(x => x.no_grupo_conta);
        }

        public IEnumerable<GrupoConta> getListaContas(int cd_grupo_conta, string no_subgrupo_conta, bool inicio, int nivel, int tipoPlanoConta, int cd_pessoa_empresa)
        {
            IEnumerable<GrupoConta> retorno = DataAccessGrupoConta.getListaContas(cd_grupo_conta, no_subgrupo_conta, inicio, nivel, tipoPlanoConta, cd_pessoa_empresa);

            DataAccessGrupoConta = new GrupoContaDataAccess();
            return retorno;
        }

        public IEnumerable<GrupoConta> getGrupoContasWithPlanoContas(int cd_pessoa_empresa)
        {
            return DataAccessGrupoConta.getGrupoContasWithPlanoContas(cd_pessoa_empresa);
        }

        public bool getGrupoContasWhitOutPlanoContas(byte nivel, int cd_pessoa_empresa)
        {
            return DataAccessGrupoConta.getGrupoContasWhitOutPlanoContas(nivel, cd_pessoa_empresa);
        }

        public IEnumerable<GrupoConta> getPlanoContasTreeSearch(int cd_escola, bool busca_somente_ativo, bool conta_segura, string descricao, bool inicio)
        {
            List<GrupoConta> retorno = DataAccessGrupoConta.getPlanoContasTreeSearch(cd_escola, busca_somente_ativo, conta_segura, descricao, inicio).ToList();

            //Ordena os grupos de contas:
            retorno = retorno.OrderBy(gc => gc.nm_ordem_grupo).ThenBy(gc => gc.id_tipo_grupo_conta).ThenBy(gc => gc.no_grupo_conta).ToList();

            //Ordena os subgrupos de conta recursivamente:
            for (int i = retorno.Count() - 1; i >= 0; i--)
            {
                List<SubgrupoConta> subGrupos = new List<SubgrupoConta>();
                bool excluiu = false;

                if (conta_segura)
                    subGrupos = retorno[i].SubGrupos.ToList();
                else
                {
                    subGrupos = retorno[i].SubGrupos.Where(sg => sg.SubgrupoPlanoConta == null || sg.SubgrupoPlanoConta.Count == 0 || sg.SubgrupoPlanoConta.Where(spc => !spc.id_conta_segura).Any()).ToList();
                    if (subGrupos.Count == 0)
                    {
                        retorno.Remove(retorno[i]);
                        excluiu = true;
                    }
                }
                if (!excluiu)
                {
                    ordenaSubgrupos(ref subGrupos, conta_segura);
                    retorno[i].SubGrupos = subGrupos;
                }
            }

            return retorno;
        }

        private void ordenaSubgrupos(ref List<SubgrupoConta> subGrupos, bool conta_segura)
        {
            if (subGrupos != null && subGrupos.Count > 0)
            {
                subGrupos = subGrupos.OrderBy(sg => sg.nm_ordem_subgrupo).ThenBy(sg => sg.no_subgrupo_conta).ToList();
                for (int i = subGrupos.Count - 1; i >= 0; i--)
                {
                    List<SubgrupoConta> subGruposFilhos = subGrupos[i].SubgruposFilhos.ToList();

                    if (subGruposFilhos.Count() > 0)
                    {
                        ordenaSubgrupos(ref subGruposFilhos, conta_segura);
                        if (conta_segura)
                            subGrupos[i].SubgruposFilhos = subGruposFilhos;
                        else
                        {
                            subGrupos[i].SubgruposFilhos = subGruposFilhos.Where(sg => sg.SubgrupoPlanoConta == null || sg.SubgrupoPlanoConta.Count == 0 || sg.SubgrupoPlanoConta.Where(spc => !spc.id_conta_segura).Any()).ToList();
                            if (subGrupos[i].SubgruposFilhos.Count == 0)
                                subGrupos.Remove(subGrupos[i]);
                        }
                    }
                }
            }
        }

        public IEnumerable<GrupoConta> getPlanoContasWithMovimento(int cd_pessoa_empresa, int tipoMovimento, string descricao, bool inicio)
        {
            List<GrupoConta> retorno = DataAccessGrupoConta.getPlanoContasTreeSearchWhitMovimento(cd_pessoa_empresa, tipoMovimento, descricao, inicio).ToList();

            //Ordena os grupos de contas:
            retorno = retorno.OrderBy(gc => gc.nm_ordem_grupo).ThenBy(gc => gc.id_tipo_grupo_conta).ThenBy(gc => gc.no_grupo_conta).ToList();

            //Ordena os subgrupos de conta recursivamente:
            for (int i = 0; i < retorno.Count(); i++)
            {
                List<SubgrupoConta> subGrupos = retorno[i].SubGrupos.ToList();
                ordenaSubgrupos(ref subGrupos, true);
                retorno[i].SubGrupos = subGrupos;
            }

            return retorno;
        }

        public IEnumerable<GrupoConta> getPlanoContasTreeSearchWhitContaCorrente(int cd_escola, string descricao, bool inicio)
        {
            List<GrupoConta> retorno = DataAccessGrupoConta.getPlanoContasTreeSearchWhitContaCorrente(cd_escola, descricao, inicio).ToList();

            //Ordena os grupos de contas:
            retorno = retorno.OrderBy(gc => gc.nm_ordem_grupo).ThenBy(gc => gc.id_tipo_grupo_conta).ThenBy(gc => gc.no_grupo_conta).ToList();

            //Ordena os subgrupos de conta recursivamente:
            for (int i = 0; i < retorno.Count(); i++)
            {
                List<SubgrupoConta> subGrupos = retorno[i].SubGrupos.ToList();
                ordenaSubgrupos(ref subGrupos, true);
                retorno[i].SubGrupos = subGrupos;
            }
            return retorno;
        }

        public IEnumerable<GrupoConta> getSubgrupoContaSearchFK(string descricao, bool inicio, int cdGrupo, SubgrupoConta.TipoNivelConsulta tipo, bool contaSegura, int cdEscola)
        {
            List<GrupoConta> retorno = DataAccessGrupoConta.getSubgrupoContaSearchFK(descricao, inicio, cdGrupo, tipo, contaSegura, cdEscola).ToList();

            //Ordena os grupos de contas:
            retorno = retorno.OrderBy(gc => gc.nm_ordem_grupo).ThenBy(gc => gc.id_tipo_grupo_conta).ThenBy(gc => gc.no_grupo_conta).ToList();

            //Ordena os subgrupos de conta recursivamente:
            for (int i = retorno.Count() - 1; i >= 0; i--)
            {
                List<SubgrupoConta> subGrupos = new List<SubgrupoConta>();
                bool excluiu = false;

                subGrupos = retorno[i].SubGrupos.Where(sg => sg.SubgrupoPlanoConta == null || sg.SubgrupoPlanoConta.Count == 0 || sg.SubgrupoPlanoConta.Where(spc => !spc.id_conta_segura).Any()).ToList();
                if (subGrupos.Count == 0)
                {
                    retorno.Remove(retorno[i]);
                    excluiu = true;
                }
                if (!excluiu)
                {
                    ordenaSubgrupos(ref subGrupos, false);
                    retorno[i].SubGrupos = subGrupos;
                }
            }

            return retorno;
        }

        #endregion

        #region Subgrupo de Contas

        public SubgrupoConta getSubgrupoContaById(int id)
        {
            return DataAccessSubgrupoConta.findById(id, false);
        }

        public SubGrupoSort postSubgrupoConta(SubgrupoConta subgrupoConta)
        {
            SubGrupoSort retorno = new SubGrupoSort();
            subgrupoConta = DataAccessSubgrupoConta.add(subgrupoConta, false);
            if (subgrupoConta.cd_subgrupo_pai != null && subgrupoConta.cd_subgrupo_pai > 0)
                retorno = DataAccessSubgrupoConta.getSubgrupoPorNivelEId(SubgrupoConta.TipoNivelConsulta.DOIS_NIVEIS, subgrupoConta.cd_subgrupo_conta);
            else
                retorno = DataAccessSubgrupoConta.getSubgrupoPorNivelEId(SubgrupoConta.TipoNivelConsulta.UM_NIVEL, subgrupoConta.cd_subgrupo_conta);
            //var grupo = DataAccessGrupoConta.(subgrupoConta.cd_grupo_conta, false);
            //subgrupoConta.SubgrupoContaGrupo = grupo;
            return retorno;
        }

        public SubGrupoSort putSubgrupoConta(SubgrupoConta subgrupoContaView)
        {
            SubGrupoSort retorno = new SubGrupoSort();
            SubgrupoConta subgrupoContaContext = DataAccessSubgrupoConta.findById(subgrupoContaView.cd_subgrupo_conta, false);
            if ((subgrupoContaContext.cd_subgrupo_pai == null && subgrupoContaView.cd_subgrupo_pai > 0) || (subgrupoContaContext.cd_subgrupo_pai > 0 && subgrupoContaView.cd_subgrupo_pai == null))
                throw new FinanceiroBusinessException(Utils.Messages.Messages.msgNaoPossivelMudarNivelSubGrupo, null, FinanceiroBusinessException.TipoErro.ERRO_NIVEL_SUBGRUPO, false);
            subgrupoContaContext.no_subgrupo_conta = subgrupoContaView.no_subgrupo_conta;
            subgrupoContaContext.nm_ordem_subgrupo = subgrupoContaView.nm_ordem_subgrupo;
            subgrupoContaContext.dc_cod_integracao_plano = subgrupoContaView.dc_cod_integracao_plano;
            DataAccessSubgrupoConta.saveChanges(false);
            if (subgrupoContaView.cd_subgrupo_pai != null && subgrupoContaView.cd_subgrupo_pai > 0)
                retorno = DataAccessSubgrupoConta.getSubgrupoPorNivelEId(SubgrupoConta.TipoNivelConsulta.DOIS_NIVEIS, subgrupoContaView.cd_subgrupo_conta);
            else
                retorno = DataAccessSubgrupoConta.getSubgrupoPorNivelEId(SubgrupoConta.TipoNivelConsulta.UM_NIVEL, subgrupoContaView.cd_subgrupo_conta);
            return retorno;
        }

        public bool deleteSubgrupoConta(SubgrupoConta subgrupoConta)
        {
            bool deleted = false;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                SubgrupoConta e = DataAccessSubgrupoConta.findById(subgrupoConta.cd_subgrupo_conta, false);
                deleted = DataAccessSubgrupoConta.delete(e, false);
                transaction.Complete();
                return deleted;
            }
        }

        public IEnumerable<SubGrupoSort> getSubgrupoContaSearch(SearchParameters parametros, string descricao, bool inicio, int cdGrupo, SubgrupoConta.TipoNivelConsulta tipo)
        {
            IEnumerable<SubGrupoSort> retorno = new List<SubGrupoSort>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "nm_ordem_subgrupo";
                retorno = DataAccessSubgrupoConta.GetSubgrupoContaSearch(parametros, descricao, inicio, cdGrupo, tipo).ToList();
                transaction.Complete();
            }
            return retorno;
        }

        public bool deleteAllSubgrupoConta(List<SubgrupoConta> subgruposConta)
        {
            this.sincronizarContextos(DataAccessSubgrupoConta.DB());
            bool retorno = false;
            if (subgruposConta != null && subgruposConta.Count() > 0)
            {
                List<SubgrupoConta> subGrupos = DataAccessSubgrupoConta.getSubgruposContaAll(subgruposConta.Select(x => x.cd_subgrupo_conta).ToList()).ToList();
                using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
                {
                    foreach (var sg in subGrupos)
                        retorno = DataAccessSubgrupoConta.delete(sg, false);
                    transaction.Complete();
                }
            }
            return retorno;
        }

        public IEnumerable<SubgrupoConta> getAllSubgrupoConta()
        {
            return DataAccessSubgrupoConta.findAll(false);
        }

        public IEnumerable<SubgrupoConta> getSubgruposPorCodGrupoContas(int cdGrupoContas)
        {
            return DataAccessSubgrupoConta.getSubgruposPorCodGrupoContas(cdGrupoContas);
        }

        #endregion

        #region Banco

        public IEnumerable<Banco> getAllBanco()
        {
            return DataAccessBanco.findAll(false);
        }
        public Banco getBancobyId(int cdBanco)
        {
            Banco retorno = new Banco();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = DataAccessBanco.findById(cdBanco, false);
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<Banco> getBancoSearch(SearchParameters parametros, string nome, string nmBanco, bool inicio)
        {
            IEnumerable<Banco> retorno = new List<Banco>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "no_banco";
                retorno = DataAccessBanco.getBancoSearch(parametros, nome, nmBanco, inicio);
                transaction.Complete();
            }
            return retorno;
        }

        public Banco postBanco(Banco banco)
        {
            banco = DataAccessBanco.add(banco, false);
            return banco;
        }

        public Banco putBanco(Banco banco)
        {
            banco = DataAccessBanco.edit(banco, false);
            return banco;
        }
        public bool deleteAllBanco(List<Banco> bancos)
        {
            bool retorno = true;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                foreach (Banco b in bancos)
                {
                    Banco bancoDel = DataAccessBanco.findById(b.cd_banco, false);
                    retorno = DataAccessBanco.delete(bancoDel, false);
                }
                transaction.Complete();
            }
            return retorno;
        }
        public IEnumerable<Banco> getBancoCarteira()
        {
            return DataAccessBanco.getBancoCarteira();
        }

        public IEnumerable<Banco> getBancosTituloCheque(int cd_empresa)
        {
            return DataAccessBanco.getBancosTituloCheque(cd_empresa);
        }
        #endregion

       #endregion

        #region Item

        public IEnumerable<ItemUI> getItemSearch(SearchParameters parametros, string descricao, bool inicio, bool? status, int? tipoItem, int? cdGrupoItem, int? cdEscola, bool isMasterGeral, bool estoque, bool biblioteca, bool comEstoque, int categoria, bool todas_escolas, bool contaSegura)
        {
            IEnumerable<ItemUI> listaItem = new List<ItemUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED, DataAccessItem.DB(), TransactionScopeBuilder.TransactionTime.MODEREATE))
            {

                if (parametros.sort == null)
                    parametros.sort = "no_item";
                parametros.sort = parametros.sort.Replace("item_ativo", "id_item_ativo");
                parametros.sort = parametros.sort.Replace("categoria_grupo", "id_categoria_grupo");
                parametros.sort = parametros.sort.Replace("pc_aliquota_iss", "iss");
                parametros.sort = parametros.sort.Replace("pc_aliquota_icms", "icms");

                listaItem = DataAccessItem.getItemSearch(parametros, descricao, inicio, status, tipoItem, cdGrupoItem, cdEscola, isMasterGeral, estoque, biblioteca, comEstoque, categoria, todas_escolas, contaSegura).ToList();
                transaction.Complete();
            }
            return listaItem;
        }
        public IEnumerable<ItemUI> getItemSearchAlunosemAula(SearchParameters parametros, string descricao, bool inicio, bool? status, int? tipoItem, int? cdGrupoItem, int? cdEscola, bool isMasterGeral, bool estoque, bool biblioteca, bool comEstoque, int categoria, bool todas_escolas, bool contaSegura)
        {
            IEnumerable<ItemUI> listaItem = new List<ItemUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED, DataAccessItem.DB(), TransactionScopeBuilder.TransactionTime.MODEREATE))
            {

                if (parametros.sort == null)
                    parametros.sort = "no_item";
                parametros.sort = parametros.sort.Replace("item_ativo", "id_item_ativo");
                parametros.sort = parametros.sort.Replace("categoria_grupo", "id_categoria_grupo");
                parametros.sort = parametros.sort.Replace("pc_aliquota_iss", "iss");
                parametros.sort = parametros.sort.Replace("pc_aliquota_icms", "icms");

                listaItem = DataAccessItem.getItemSearchAlunosemAula(parametros, descricao, inicio, status, tipoItem, cdGrupoItem, cdEscola, isMasterGeral, estoque, biblioteca, comEstoque, categoria, todas_escolas, contaSegura).ToList();
                transaction.Complete();
            }
            return listaItem;
        }

        public IEnumerable<ItemKitUI> getKitSearch(SearchParameters parametros, string descricao, bool inicio, bool? status, int? tipoItem, int? cdGrupoItem, int? cdEscola, bool isMasterGeral, bool estoque, bool biblioteca, bool comEstoque, int categoria, bool todas_escolas, bool contaSegura)
        {
            IEnumerable<ItemKitUI> listaItem = new List<ItemKitUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED, DataAccessItem.DB(), TransactionScopeBuilder.TransactionTime.MODEREATE))
            {

                if (parametros.sort == null)
                    parametros.sort = "no_item";
                parametros.sort = parametros.sort.Replace("item_ativo", "id_item_ativo");
                parametros.sort = parametros.sort.Replace("categoria_grupo", "id_categoria_grupo");
                parametros.sort = parametros.sort.Replace("pc_aliquota_iss", "iss");
                parametros.sort = parametros.sort.Replace("pc_aliquota_icms", "icms");

                listaItem = DataAccessItem.getKitSearch(parametros, descricao, inicio, status, tipoItem, cdGrupoItem, cdEscola, isMasterGeral, estoque, biblioteca, comEstoque, categoria, todas_escolas, contaSegura).ToList();
                transaction.Complete();
            }
            return listaItem;
        }

        public IEnumerable<ItemUI> getItemSearch(SearchParameters parametros, string descricao, bool inicio, bool? ativo, int tipoItem, int cdGrupoItem, int cdEscola, bool comEstoque, int categoria)
        {
            IEnumerable<ItemUI> listaItem = new List<ItemUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED, DataAccessItem.DB(), TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                if (parametros.sort == null)
                    parametros.sort = "no_item";
                parametros.sort = parametros.sort.Replace("id_item_ativo", "item_ativo");
                parametros.sort = parametros.sort.Replace("pc_aliquota_iss", "iss");
                parametros.sort = parametros.sort.Replace("pc_aliquota_icms", "icms");

                listaItem = DataAccessItem.getItemSearch(parametros, descricao, inicio, ativo, tipoItem, cdGrupoItem, cdEscola, comEstoque, categoria).ToList();
                transaction.Complete();
            }
            return listaItem;
        }

        public IEnumerable<ItemUI> getItemCurso(int cdCurso, int? cdEscola, bool isMasterGeral)
        {
            return DataAccessItem.getItemCurso(cdCurso, cdEscola);
        }

        public IEnumerable<Item> getItensByIds(List<Item> itens)
        {
            return DataAccessItem.getItensByIds(itens);
        }

        public bool deleteItemByCurso(int cd_curso)
        {
            bool deleted = false;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                deleted = DataAccessItem.deleteItemByCurso(cd_curso);
                transaction.Complete();
                return deleted;
            }
        }

        public List<KitUI> getItensKit(int idKit)
        {
            List<KitUI> result;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                result = DataAccessItem.getItensKit(idKit);
                transaction.Complete();
                return result;
            }
        }

        public ItemUI addItemEstoque(ItemUI item, int cdEscola, ICollection<Escola> escolasUsuario, bool isMasterGeral)
        {
            Item novoItem = new Item();
            ItemUI itemUI = new ItemUI();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED, DataAccessItem.DB(), TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                DataAccessItemEscola.sincronizaContexto(DataAccessItem.DB());
                if (item != null)
                {
                    GrupoEstoque grupo = getGrupoEstoqueById(item.cd_grupo_estoque);
                    if (grupo != null && grupo.cd_grupo_estoque > 0)
                        if (!isMasterGeral)
                        {
                            if (grupo.id_categoria_grupo == (int)GrupoEstoque.CategoriaEnum.PRIVADA)
                                throw new FinanceiroBusinessException(Utils.Messages.Messages.msgErroGrupoItemPrivado, null, FinanceiroBusinessException.TipoErro.ERRO_CATEGORIA_GRUPO_ITEM, false);
                        }
                        else
                        {
                            if (grupo.id_categoria_grupo == (int)GrupoEstoque.CategoriaEnum.PUBLICA)
                                throw new FinanceiroBusinessException(Utils.Messages.Messages.msgMasterPersistirPrivado, null, FinanceiroBusinessException.TipoErro.ERRO_CATEGORIA_GRUPO_ITEM, false);
                        }
                    bool existeEsc = DataAccessItem.getItemNomeEsc(item.no_item, cdEscola);
                    if (existeEsc)
                        throw new FinanceiroBusinessException(Utils.Messages.Messages.msgExisteItemEsc, null, FinanceiroBusinessException.TipoErro.ERRO_EXISTE_ITEM_ESC, false);
                    bool existeEmOutraEsc = DataAccessItem.getItemNome(item.no_item);
                    if (existeEmOutraEsc)
                        throw new FinanceiroBusinessException(Utils.Messages.Messages.msgExisteItem, null, FinanceiroBusinessException.TipoErro.ERRO_EXISTE_ITEM, false);

                    //Persistir um item
                    novoItem.copy(item);
                    ItemEscola itemEscola = new ItemEscola();
                    List<ItemEscola> itensEscola = new List<ItemEscola>();
                    if (escolasUsuario != null && escolasUsuario.Count() > 0)
                    {
                        foreach (var escolas in escolasUsuario)
                            if (escolas.cd_pessoa == cdEscola)
                            {
                                novoItem.ItemEscola.Add(
                                    new ItemEscola
                                    {
                                        cd_pessoa_escola = escolas.cd_pessoa,
                                        cd_item = novoItem.cd_item,
                                        qt_estoque = item.qt_estoque
                                    });
                            }
                            else
                            {
                                novoItem.ItemEscola.Add(
                                new ItemEscola
                                {
                                    cd_pessoa_escola = escolas.cd_pessoa,
                                    cd_item = novoItem.cd_item
                                });
                            }
                    }

                    ItemKit itemKit = new ItemKit();
                    List<ItemKit> itensKit = new List<ItemKit>();
                    if (item.itemKit != null && item.itemKit.Count() > 0)
                    {
                        foreach (var kit in item.itemKit)
                        {
                            novoItem.ItemKit.Add(
                                new ItemKit
                                {
                                    cd_item = kit.cd_item,
                                    cd_item_kit = novoItem.cd_item,
                                    qt_item_kit = kit.qt_item_kit
                                });
                        }
                    }

                    int existeTpIgual = 0;
                    if (item.itemSubgrupo != null && item.itemSubgrupo.Count() > 0)
                        existeTpIgual = item.itemSubgrupo.Where(tv => item.itemSubgrupo.Where(x => x.cd_item != tv.cd_item && x.id_tipo_movimento == tv.id_tipo_movimento).Any()).Count();
                    if (existeTpIgual > 1)
                        throw new FinanceiroBusinessException(Utils.Messages.Messages.msgErroItemTipoSubgrupoIguais, null, FinanceiroBusinessException.TipoErro.ERRO_TIPO_SUBGRUPO_ITEM_IGUAIS, false);

                    novoItem.ItemSubgrupos = item.itemSubgrupo;
                    DataAccessItem.add(novoItem, false);

                    //Persistir Biblioteca segundo regras do item
                    Biblioteca biblioteca = new Biblioteca();
                    if (novoItem.cd_item > 0 && novoItem.cd_tipo_item == BIBLIOTECA)
                    {
                        biblioteca = returnBiblioteca(item, novoItem.cd_item);
                        DataAccessBiblioteca.add(biblioteca, false);
                    }

                    //Retorna os valores item para escola logada
                    if (item != null && cdEscola > 0 && novoItem.cd_item > 0 && !isMasterGeral)
                        itemEscola = retonarItemEscola(item, cdEscola, novoItem.cd_item);

                    if (item.qt_estoque != 0)
                    {
                        SGFWebContext cdb = new SGFWebContext();
                        //Kardex de inclusão 
                        Kardex kardex = new Kardex();
                        //Cria o registro de kardex de fechamento:
                        kardex.cd_pessoa_empresa = cdEscola;
                        kardex.cd_item = novoItem.cd_item;
                        kardex.cd_origem = (byte)cdb.LISTA_ORIGEM_LOGS["Item"];
                        kardex.cd_registro_origem = novoItem.cd_item;
                        kardex.dt_kardex = DateTime.Now.Date;
                        kardex.id_tipo_movimento = item.qt_estoque > 0 ? (byte)Kardex.TipoMovimento.ENTRADA : (byte)Kardex.TipoMovimento.SAIDA;
                        kardex.qtd_kardex = Math.Abs(item.qt_estoque);
                        kardex.nm_documento = novoItem.cd_item + "";
                        kardex.tx_obs_kardex = "Alteração feita diretamente pela tela de item.";
                        kardex.vl_kardex = item.vl_custo;
                        kardex = DataAccessKardex.add(kardex, false);
                    }
                    //Setando valores para retornar na grade  
                    string grupoEstoque = item.no_grupo_estoque;
                    string tipoItem = item.dc_tipo_item;
                    if (novoItem.cd_grupo_estoque > 0)
                        novoItem.GrupoEstoque = DataAccessGrupoEstoque.findById(novoItem.cd_grupo_estoque, false);
                    //if (novoItem.cd_plano_conta > 0)
                    //    novoItem.desc_plano_conta = DataAccessItem.getPlanoContaByItem(novoItem.cd_item);
                    itemUI = DataAccessItem.getItemUIbyId(novoItem.cd_item, cdEscola);
                    //itemUI = ItemUI.fromItem(novoItem, tipoItem, grupoEstoque, itemEscola, biblioteca, item.desc_plano_conta);
                }
                transaction.Complete();
            }
            return itemUI;
        }

        private static Biblioteca returnBiblioteca(ItemUI item, int cdItem)
        {
            Biblioteca biblioteca = new Biblioteca
            {
                cd_item = cdItem,
                dc_assunto = item.dc_assunto,
                dc_local = item.dc_local,
                dc_titulo = item.dc_titulo,
                no_autor = item.no_autor
            };
            return biblioteca;
        }

        //Delete all item
        public bool deleteAllItem(List<Item> itens, bool isMasterGeral, int cdEscola)
        {
            bool deleted = false;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                for (int i = 0; i < itens.Count(); i++)
                {
                    Item item = DataAccessItem.findById(itens[i].cd_item, false);
                    if (!isMasterGeral)
                    {
                        GrupoEstoque grupo = getGrupoEstoqueById(item.cd_grupo_estoque);
                        if (grupo != null && grupo.cd_grupo_estoque > 0 && grupo.id_categoria_grupo == (int)GrupoEstoque.CategoriaEnum.PRIVADA)
                            throw new FinanceiroBusinessException(Utils.Messages.Messages.msgErroGrupoItemPrivado, null, FinanceiroBusinessException.TipoErro.ERRO_CATEGORIA_GRUPO_ITEM, false);
                    }

                    if (isMasterGeral)
                    {
                        GrupoEstoque grupo = getGrupoEstoqueById(item.cd_grupo_estoque);
                        if (grupo != null && grupo.cd_grupo_estoque > 0 && grupo.id_categoria_grupo == (int)GrupoEstoque.CategoriaEnum.PUBLICA)
                            throw new FinanceiroBusinessException(Utils.Messages.Messages.msgMasterPersistirPrivado, null, FinanceiroBusinessException.TipoErro.ERRO_CATEGORIA_GRUPO_ITEM, false);
                    }

                    if (isMasterGeral)
                    {
                        List<ItemEscola> itensEsc = DataAccessItemEscola.getItensEscolaByItem(item.cd_item).ToList();
                        if (itensEsc != null && itensEsc.Count() > 0)
                        {
                            List<int> cdsEscola = itensEsc.Select(e => e.cd_pessoa_escola).ToList();
                            bool existeKardexItemEscs = DataAccessKardex.existeKardexItemEscolas(item.cd_item, cdsEscola);
                            if (existeKardexItemEscs)
                                throw new FinanceiroBusinessException(Utils.Messages.Messages.msgErroItemEscKardex, null, FinanceiroBusinessException.TipoErro.ERRO_ITEM_POSSUI_KARDEX, false);
                            foreach (ItemEscola itemEscola in itensEsc)
                            {
                                ItemEscola itemDeletar = DataAccessItemEscola.findItemEscolabyId(itemEscola.cd_item, itemEscola.cd_pessoa_escola);
                                if (itemDeletar != null && itemDeletar.cd_item_escola > 0)
                                {
                                    SGFWebContext cdb = new SGFWebContext();
                                    Kardex kardex = DataAccessKardex.getKardexByOrigemItem((byte)cdb.LISTA_ORIGEM_LOGS["Item"], itemEscola.cd_item, itemEscola.cd_item, itemEscola.cd_pessoa_escola);
                                    if (kardex != null && kardex.cd_kardex > 0)
                                        DataAccessKardex.delete(kardex, false);
                                    if (item.cd_tipo_item == BIBLIOTECA)
                                    {
                                        Biblioteca biblioteca = DataAccessBiblioteca.findById(item.cd_item, false);
                                        if (biblioteca != null)
                                            DataAccessBiblioteca.delete(biblioteca, false);
                                    }
                                }
                            }
                        }

                        bool itemParametro = DataAccessItem.getexisteParametroItem(item.cd_item);
                        bool existeKardexItem = DataAccessKardex.existeKardexItem(item.cd_item);
                        if (!itemParametro && !existeKardexItem)
                            DataAccessItem.delete(item, false);
                    }
                    else
                    {
                        ItemEscola itemDeletar = DataAccessItemEscola.findItemEscolabyId(item.cd_item, cdEscola);
                        if (itemDeletar != null && itemDeletar.cd_item_escola > 0)
                        {

                            SGFWebContext cdb = new SGFWebContext();
                            Kardex kardex = DataAccessKardex.getKardexByOrigemItem((byte)cdb.LISTA_ORIGEM_LOGS["Item"], itemDeletar.cd_item, itemDeletar.cd_item, itemDeletar.cd_pessoa_escola);
                            if (kardex != null && kardex.cd_kardex > 0)
                                DataAccessKardex.delete(kardex, false);
                            bool existeKardexItemEsc = DataAccessKardex.existeKardexItemEsc(item.cd_item, cdEscola);
                            if (existeKardexItemEsc)
                                throw new FinanceiroBusinessException(Utils.Messages.Messages.msgErroItemEscKardex, null, FinanceiroBusinessException.TipoErro.ERRO_ITEM_POSSUI_KARDEX, false);
                            if (item.cd_tipo_item == BIBLIOTECA)
                            {
                                Biblioteca biblioteca = DataAccessBiblioteca.findById(item.cd_item, false);
                                if (biblioteca != null)
                                    DataAccessBiblioteca.delete(biblioteca, false);
                            }
                            DataAccessItemEscola.delete(itemDeletar, false);
                        }
                        else
                            throw new FinanceiroBusinessException(Utils.Messages.Messages.msgErroDelItemOutraEsc, null, FinanceiroBusinessException.TipoErro.ERRO_ITEM_USADO_OUTRAS_ESC, false);
                        //Se não tiver sendo usado em outra escola, deleta o item
                        bool existeItem = DataAccessItemEscola.getExisteItensEscolaByItem(item.cd_item);
                        bool itemParametro = DataAccessItem.getexisteParametroItem(item.cd_item);
                        if (!existeItem && !itemParametro)
                            DataAccessItem.delete(item, false);
                    }
                    deleted = true;
                }
                transaction.Complete();
            }
            return deleted;
        }

        /// <summary>
        /// Regra para editar o item 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="isMaster"></param>
        /// <param name="cdEscola"></param>
        /// <param name="escolas"></param>
        /// <returns></returns>
        public ItemUI editarItemEstoque(ItemUI item, int cdEscola, ICollection<Escola> escolas, bool isMasterGeral, ICollection<EmpresaSession> listEmp)
        {
            Item itemBase = new Item();
            ItemUI itemUI = new ItemUI();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                Item itens = new Item();
                GrupoEstoque grupo = new GrupoEstoque();

                List<int> cdsListEmpUsuario = listEmp.Select(o => o.cd_pessoa).ToList();
                IEnumerable<Escola> escolasView = escolas.ToList();
                List<int> cdsEscolasView = escolasView.Select(o => o.cd_pessoa).ToList();

                IEnumerable<int> codEscolasBase = DataAccessItemEscola.getItensWithEscola(item.cd_item).ToList();


                if (isMasterGeral)
                {
                    grupo = getGrupoEstoqueById(item.cd_grupo_estoque);
                    if (grupo != null && grupo.cd_grupo_estoque > 0 && grupo.id_categoria_grupo == (int)GrupoEstoque.CategoriaEnum.PUBLICA)
                        throw new FinanceiroBusinessException(Utils.Messages.Messages.msgMasterPersistirPrivado, null, FinanceiroBusinessException.TipoErro.ERRO_CATEGORIA_GRUPO_ITEM, false);
                }
                itens = DataAccessItem.getItemEdit(item.cd_item);

                //Verifica se alterou os itens
                bool deletouPlanoConta = false;
                bool incluiuPlanoConta = false;
                bool alterouPlanoConta = false;
                if (itens.ItemSubgrupos != null && itens.ItemSubgrupos.Count() > 0 && item.itemSubgrupo != null && item.itemSubgrupo.Count() > 0)
                    deletouPlanoConta = itens.ItemSubgrupos.Where(sv => !item.itemSubgrupo.Where(s => s.cd_item_subgrupo == sv.cd_item_subgrupo).Any()).Any();
                if (item.itemSubgrupo != null && item.itemSubgrupo.Count() > 0)
                    incluiuPlanoConta = item.itemSubgrupo.Where(sv => !itens.ItemSubgrupos.Where(s => s.cd_item_subgrupo == sv.cd_item_subgrupo).Any()).Any();
                if (itens.ItemSubgrupos != null && itens.ItemSubgrupos.Count() > 0 && item.itemSubgrupo != null && item.itemSubgrupo.Count() > 0)
                    alterouPlanoConta = itens.ItemSubgrupos.Where(sv => item.itemSubgrupo.Where(s => s.cd_item_subgrupo == sv.cd_item_subgrupo && s.id_tipo_movimento == sv.id_tipo_movimento && s.cd_subgrupo_conta != sv.cd_subgrupo_conta).Any()).Any();

                if (item.cd_tipo_item != itens.cd_tipo_item ||
                    item.cd_grupo_estoque != itens.cd_grupo_estoque ||
                    item.no_item != itens.no_item ||
                    item.id_item_ativo != itens.id_item_ativo ||
                    item.id_material_didatico != itens.id_material_didatico ||
                    item.id_voucher_carga != itens.id_voucher_carga ||
                    item.qt_horas_carga != itens.qt_horas_carga ||
                    item.dc_sgl_item != itens.dc_sgl_item ||
                    item.cd_origem_fiscal != itens.cd_origem_fiscal ||
                    item.dc_classificacao_fiscal != itens.dc_classificacao_fiscal ||
                    (item.pc_aliquota_icms != itens.pc_aliquota_icms && (item.pc_aliquota_icms > 0 && itens.pc_aliquota_icms.HasValue || item.pc_aliquota_icms.HasValue && itens.pc_aliquota_icms > 0)) ||
                    (item.pc_aliquota_iss != itens.pc_aliquota_iss && (item.pc_aliquota_iss > 0 && itens.pc_aliquota_iss.HasValue || item.pc_aliquota_iss.HasValue && itens.pc_aliquota_iss > 0)) ||
                    item.cd_integracao != itens.cd_integracao ||
                    item.dc_codigo_barra != itens.dc_codigo_barra ||
                    deletouPlanoConta || incluiuPlanoConta || alterouPlanoConta)
                {
                    if (!isMasterGeral)
                    {
                        bool escolasDif;
                        if (item.hasClickEscola)
                            escolasDif = cdsEscolasView.Where(e => !cdsListEmpUsuario.Contains(e)).Count() > 0;
                        else
                            escolasDif = codEscolasBase.Where(e => !cdsListEmpUsuario.Contains(e)).Count() > 0;
                        if (escolasDif)
                            throw new FinanceiroBusinessException(Utils.Messages.Messages.msgErroItemAlterado, null, FinanceiroBusinessException.TipoErro.ERRO_ITEM_USADO_OUTRAS_ESC, false);
                    }
                }


                int categoriaGrupo = itens.id_categoria_grupo;
                ItemEscola itemEsc = DataAccessItemEscola.findItemEscolabyId(item.cd_item, cdEscola);

                if (itemEsc == null || itemEsc.cd_item_escola == 0)
                {
                    ItemEscola escLogada = new ItemEscola
                    {
                        cd_pessoa_escola = cdEscola,
                        cd_item = itens.cd_item
                    };
                    DataAccessItemEscola.add(escLogada, false);
                }


                if ((escolasView != null) && (item.hasClickEscola))
                {
                    IEnumerable<int> codEscolasVW = escolasView.Select(ev => ev.cd_pessoa);

                    //Insere as Escolas da view que foram adcionadas na grade e não estão na base de dados
                    IEnumerable<int> escolasInserir = codEscolasVW.Except(codEscolasBase);
                    ItemEscola novoItemEscola = new ItemEscola();
                    if (isMasterGeral)
                    {
                        //Incluindo plano de contas nos títulos de acordo com os subgrupos escolhidos
                        int i = 0;
                        int[] cdEscolas = new int[escolas.Count()];
                        foreach (var c in escolas)
                        {
                            cdEscolas[i] = c.cd_pessoa;
                            i++;
                        }
                        //planosEscolas = DataAccessPlanoConta.getPlanoContaNiveis(item.cd_subgrupo_conta, item.cd_subgrupo_conta_2, cdEscolas);

                        //Editando plano de contas
                        if (itens.cd_subgrupo_conta != item.cd_subgrupo_conta || itens.cd_subgrupo_conta_2 != item.cd_subgrupo_conta_2)
                        {
                            List<ItemEscola> itensAlterar = DataAccessItemEscola.getItemEscolaByItem(item.cd_item).ToList();

                            int j = 0;
                            int[] cdEscolasAlt = new int[codEscolasBase.Count()];
                            foreach (var c in codEscolasBase)
                            {
                                cdEscolasAlt[j] = c;
                                j++;
                            }
                            DataAccessItemEscola.saveChanges(false);
                        }
                    }

                    if (escolasInserir != null && escolasInserir.Count() > 0)
                    {
                        foreach (var itemEscola in escolasInserir)
                        {
                            //int? cdPlanoConta = null;
                            if (itemEscola == cdEscola)
                            {
                                novoItemEscola = new ItemEscola
                                                {
                                                    cd_pessoa_escola = itemEscola,
                                                    cd_item = itens.cd_item
                                                };
                            }
                            else
                            {
                                novoItemEscola = new ItemEscola
                                {
                                    cd_pessoa_escola = itemEscola,
                                    cd_item = itens.cd_item
                                    //cd_plano_conta = cdPlanoConta
                                };
                            }
                            DataAccessItemEscola.addContext(novoItemEscola, false);
                        }

                        DataAccessItemEscola.saveChanges(false);
                    }


                    //Deletar as escolas que estão no banco e não estão na view.
                    IEnumerable<int> deletarEscolas = codEscolasBase.Except(codEscolasVW);
                    if (deletarEscolas != null && deletarEscolas.Count() > 0)
                    {
                        foreach (var itemEscola in deletarEscolas)
                        {
                            ItemEscola itemEscolaDeletar = DataAccessItemEscola.findItemEscolabyId(itens.cd_item, itemEscola);
                            DataAccessItemEscola.deleteContext(itemEscolaDeletar, false);
                        }
                        DataAccessItemEscola.saveChanges(false);
                    }
                }

                //Editar itens subgrupo
                if (itens.ItemSubgrupos != null && itens.ItemSubgrupos.Count() > 0 && item.itemSubgrupo != null && item.itemSubgrupo.Count() > 0)
                {
                    List<ItemSubgrupo> itensDeleted = itens.ItemSubgrupos.Where(tc => !item.itemSubgrupo.Any(tv => tc.cd_item_subgrupo == tv.cd_item_subgrupo)).ToList();
                    if (itensDeleted != null && itensDeleted.Count() > 0)
                        foreach (ItemSubgrupo ip in itensDeleted)
                        {
                            ItemSubgrupo itemDelete = DataAccessItemSubgrupo.findById(ip.cd_item_subgrupo, false);
                            itens.ItemSubgrupos.Remove(itemDelete);
                            DataAccessItemSubgrupo.delete(itemDelete, false);
                        }
                }

                //Alterando ou Incluindo novos itens
                if (item.itemSubgrupo != null && item.itemSubgrupo.Count() > 0)
                    foreach (ItemSubgrupo ip in item.itemSubgrupo)
                    {
                        int existeTpIgual = item.itemSubgrupo.Where(tv => tv.id_tipo_movimento == ip.id_tipo_movimento).Count();
                        if (existeTpIgual > 1)
                            throw new FinanceiroBusinessException(Utils.Messages.Messages.msgErroItemTipoSubgrupoIguais, null, FinanceiroBusinessException.TipoErro.ERRO_TIPO_SUBGRUPO_ITEM_IGUAIS, false);
                        //int existeSubgIgual = item.itemSubgrupo.Where(tv => tv.cd_subgrupo_conta == ip.cd_subgrupo_conta).Count();
                        //if (existeSubgIgual > 1)
                        //    throw new FinanceiroBusinessException(Utils.Messages.Messages.msgErroItemSubgrupoIguais, null, FinanceiroBusinessException.TipoErro.ERRO_SUBGRUPO_ITEM_IGUAIS, false);
                        ItemSubgrupo itemAlterar = new ItemSubgrupo();
                        if (itens.ItemSubgrupos != null && itens.ItemSubgrupos.Count() > 0)
                            itemAlterar = itens.ItemSubgrupos.Where(tc => tc.cd_item_subgrupo == ip.cd_item_subgrupo).FirstOrDefault();
                        if (itemAlterar == null || itemAlterar.cd_item_subgrupo <= 0)
                        {
                            ip.cd_item = item.cd_item;
                            ip.cd_item_subgrupo = 0;
                            DataAccessItemSubgrupo.add(ip, false);
                        }
                        else
                        {

                            itemAlterar.cd_subgrupo_conta = ip.cd_subgrupo_conta;
                            itemAlterar.id_tipo_movimento = ip.id_tipo_movimento;
                            DataAccessItemSubgrupo.saveChanges(false);
                        }
                    }
                if (categoriaGrupo <= 0 && (!isMasterGeral))
                    throw new FinanceiroBusinessException(Utils.Messages.Messages.msgCategoriaItemZero, null, FinanceiroBusinessException.TipoErro.ERRO_CATEGORIA_GRUPO_ITEM, false);

                //Se for master ou não for master e a categoria for publica, fazer a alteração 
                if (isMasterGeral || (!isMasterGeral && categoriaGrupo == (int)GrupoEstoque.CategoriaEnum.PUBLICA))
                {
                    itens = changeValuesItem(item);
                    //regras para item do tipo de biblioteca
                    persistenciaBiblioteca(item, itens.cd_tipo_item);
                }
                itemUI = DataAccessItem.getItemUIbyId(item.cd_item, cdEscola);
                transaction.Complete();
            }
            return itemUI;
        }

        /// <summary>
        /// Regra para editar o item 
        /// </summary>
        /// <param name="Kit"></param>
        /// <param name="isMaster"></param>
        /// <param name="cdEscola"></param>
        /// <param name="escolas"></param>
        /// <returns></returns>
        public ItemUI editarKitEstoque(ItemUI item, int cdEscola, ICollection<Escola> escolas, bool isMasterGeral, ICollection<EmpresaSession> listEmp)
        {
            Item itemBase = new Item();
            ItemUI itemUI = new ItemUI();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                Item itens = new Item();
                GrupoEstoque grupo = new GrupoEstoque();

                List<int> cdsListEmpUsuario = listEmp.Select(o => o.cd_pessoa).ToList();
                IEnumerable<Escola> escolasView = escolas.ToList();
                List<int> cdsEscolasView = escolasView.Select(o => o.cd_pessoa).ToList();

                IEnumerable<int> codEscolasBase = DataAccessItemEscola.getItensWithEscola(item.cd_item).ToList();


                if (isMasterGeral)
                {
                    grupo = getGrupoEstoqueById(item.cd_grupo_estoque);
                    if (grupo != null && grupo.cd_grupo_estoque > 0 && grupo.id_categoria_grupo == (int)GrupoEstoque.CategoriaEnum.PUBLICA)
                        throw new FinanceiroBusinessException(Utils.Messages.Messages.msgMasterPersistirPrivado, null, FinanceiroBusinessException.TipoErro.ERRO_CATEGORIA_GRUPO_ITEM, false);
                }
                itens = DataAccessItem.getItemEdit(item.cd_item);

                //Verifica se alterou os itens
                bool deletouPlanoConta = false;
                bool incluiuPlanoConta = false;
                bool alterouPlanoConta = false;

                if (item.cd_tipo_item != itens.cd_tipo_item ||
                    item.cd_grupo_estoque != itens.cd_grupo_estoque ||
                    item.no_item != itens.no_item ||
                    item.id_item_ativo != itens.id_item_ativo ||
                    item.dc_sgl_item != itens.dc_sgl_item ||
                    item.cd_origem_fiscal != itens.cd_origem_fiscal ||
                    item.dc_classificacao_fiscal != itens.dc_classificacao_fiscal ||
                    (item.pc_aliquota_icms != itens.pc_aliquota_icms && (item.pc_aliquota_icms > 0 && itens.pc_aliquota_icms.HasValue || item.pc_aliquota_icms.HasValue && itens.pc_aliquota_icms > 0)) ||
                    (item.pc_aliquota_iss != itens.pc_aliquota_iss && (item.pc_aliquota_iss > 0 && itens.pc_aliquota_iss.HasValue || item.pc_aliquota_iss.HasValue && itens.pc_aliquota_iss > 0)) ||
                    item.cd_integracao != itens.cd_integracao ||
                    item.dc_codigo_barra != itens.dc_codigo_barra ||
                    deletouPlanoConta || incluiuPlanoConta || alterouPlanoConta)
                {
                    if (!isMasterGeral)
                    {
                        bool escolasDif;
                        if (item.hasClickEscola)
                            escolasDif = cdsEscolasView.Where(e => !cdsListEmpUsuario.Contains(e)).Count() > 0;
                        else
                            escolasDif = codEscolasBase.Where(e => !cdsListEmpUsuario.Contains(e)).Count() > 0;
                        if (escolasDif)
                            throw new FinanceiroBusinessException(Utils.Messages.Messages.msgErroItemAlterado, null, FinanceiroBusinessException.TipoErro.ERRO_ITEM_USADO_OUTRAS_ESC, false);
                    }
                }


                int categoriaGrupo = itens.id_categoria_grupo;
                ItemEscola itemEsc = DataAccessItemEscola.findItemEscolabyId(item.cd_item, cdEscola);

                if (itemEsc == null || itemEsc.cd_item_escola == 0)
                {
                    ItemEscola escLogada = new ItemEscola
                    {
                        cd_pessoa_escola = cdEscola,
                        cd_item = itens.cd_item
                    };
                    DataAccessItemEscola.add(escLogada, false);
                }

                List<ItemKit> itensKitsInserir = new List<ItemKit>();
                List<ItemKit> itensKitsEditar = new List<ItemKit>();
                List<int> cdItemKitsView = item.itemKit.Select(ev => ev.cd_item).ToList();
                List<int> cdItensKitsBd = itens.ItemKit.Select(ev => ev.cd_item).ToList();

                List<int> listaInserir = cdItemKitsView.Except(cdItensKitsBd).ToList();
                List<int> listaDeletar = cdItensKitsBd.Except(cdItemKitsView).ToList();

                foreach (int cdKit in listaInserir)
                {
                    ItemKit itemKitInserir = item.itemKit.Where(c=> c.cd_item == cdKit).FirstOrDefault();
                    itensKitsInserir.Add(
                        new ItemKit
                        {
                            cd_item = itemKitInserir.cd_item,
                            cd_item_kit = item.cd_item,
                            qt_item_kit = itemKitInserir.qt_item_kit
                        });
                }

                

                foreach (ItemKit itemKit in itensKitsInserir)
                {
                    DataAccessItemKit.add(itemKit, false);
                }

                DataAccessItemKit.saveChanges(false);



                if ((escolasView != null) && (item.hasClickEscola))
                {
                    IEnumerable<int> codEscolasVW = escolasView.Select(ev => ev.cd_pessoa);

                    //Insere as Escolas da view que foram adcionadas na grade e não estão na base de dados
                    IEnumerable<int> escolasInserir = codEscolasVW.Except(codEscolasBase);
                    ItemEscola novoItemEscola = new ItemEscola();
                    if (isMasterGeral)
                    {
                        //Incluindo plano de contas nos títulos de acordo com os subgrupos escolhidos
                        int i = 0;
                        int[] cdEscolas = new int[escolas.Count()];
                        foreach (var c in escolas)
                        {
                            cdEscolas[i] = c.cd_pessoa;
                            i++;
                        }
                        //planosEscolas = DataAccessPlanoConta.getPlanoContaNiveis(item.cd_subgrupo_conta, item.cd_subgrupo_conta_2, cdEscolas);

                        //Editando plano de contas
                        if (itens.cd_subgrupo_conta != item.cd_subgrupo_conta || itens.cd_subgrupo_conta_2 != item.cd_subgrupo_conta_2)
                        {
                            List<ItemEscola> itensAlterar = DataAccessItemEscola.getItemEscolaByItem(item.cd_item).ToList();

                            int j = 0;
                            int[] cdEscolasAlt = new int[codEscolasBase.Count()];
                            foreach (var c in codEscolasBase)
                            {
                                cdEscolasAlt[j] = c;
                                j++;
                            }
                            DataAccessItemEscola.saveChanges(false);
                        }
                    }

                    if (escolasInserir != null && escolasInserir.Count() > 0)
                    {
                        foreach (var itemEscola in escolasInserir)
                        {
                            //int? cdPlanoConta = null;
                            if (itemEscola == cdEscola)
                            {
                                novoItemEscola = new ItemEscola
                                {
                                    cd_pessoa_escola = itemEscola,
                                    cd_item = itens.cd_item
                                };
                            }
                            else
                            {
                                novoItemEscola = new ItemEscola
                                {
                                    cd_pessoa_escola = itemEscola,
                                    cd_item = itens.cd_item
                                    //cd_plano_conta = cdPlanoConta
                                };
                            }
                            DataAccessItemEscola.addContext(novoItemEscola, false);
                        }

                        DataAccessItemEscola.saveChanges(false);
                    }


                    //Deletar as escolas que estão no banco e não estão na view.
                    IEnumerable<int> deletarEscolas = codEscolasBase.Except(codEscolasVW);
                    if (deletarEscolas != null && deletarEscolas.Count() > 0)
                    {
                        foreach (var itemEscola in deletarEscolas)
                        {
                            ItemEscola itemEscolaDeletar = DataAccessItemEscola.findItemEscolabyId(itens.cd_item, itemEscola);
                            DataAccessItemEscola.deleteContext(itemEscolaDeletar, false);
                        }
                        DataAccessItemEscola.saveChanges(false);
                    }

                    
                }

                if (listaDeletar != null && listaDeletar.Count() > 0)
                {
                    foreach (var itemKit in listaDeletar)
                    {
                        ItemKit itemKitDeletar = DataAccessItemKit.getKitByCdItem(itemKit);
                        DataAccessItemKit.deleteContext(itemKitDeletar, false);
                    }
                    DataAccessItemKit.saveChanges(false);
                }

                //Editar itens subgrupo
                if (itens.ItemSubgrupos != null && itens.ItemSubgrupos.Count() > 0 && item.itemSubgrupo != null && item.itemSubgrupo.Count() > 0)
                {
                    List<ItemSubgrupo> itensDeleted = itens.ItemSubgrupos.Where(tc => !item.itemSubgrupo.Any(tv => tc.cd_item_subgrupo == tv.cd_item_subgrupo)).ToList();
                    if (itensDeleted != null && itensDeleted.Count() > 0)
                        foreach (ItemSubgrupo ip in itensDeleted)
                        {
                            ItemSubgrupo itemDelete = DataAccessItemSubgrupo.findById(ip.cd_item_subgrupo, false);
                            itens.ItemSubgrupos.Remove(itemDelete);
                            DataAccessItemSubgrupo.delete(itemDelete, false);
                        }
                }

                //Alterando ou Incluindo novos itens
                if (item.itemSubgrupo != null && item.itemSubgrupo.Count() > 0)
                    foreach (ItemSubgrupo ip in item.itemSubgrupo)
                    {
                        int existeTpIgual = item.itemSubgrupo.Where(tv => tv.id_tipo_movimento == ip.id_tipo_movimento).Count();
                        if (existeTpIgual > 1)
                            throw new FinanceiroBusinessException(Utils.Messages.Messages.msgErroItemTipoSubgrupoIguais, null, FinanceiroBusinessException.TipoErro.ERRO_TIPO_SUBGRUPO_ITEM_IGUAIS, false);
                        //int existeSubgIgual = item.itemSubgrupo.Where(tv => tv.cd_subgrupo_conta == ip.cd_subgrupo_conta).Count();
                        //if (existeSubgIgual > 1)
                        //    throw new FinanceiroBusinessException(Utils.Messages.Messages.msgErroItemSubgrupoIguais, null, FinanceiroBusinessException.TipoErro.ERRO_SUBGRUPO_ITEM_IGUAIS, false);
                        ItemSubgrupo itemAlterar = new ItemSubgrupo();
                        if (itens.ItemSubgrupos != null && itens.ItemSubgrupos.Count() > 0)
                            itemAlterar = itens.ItemSubgrupos.Where(tc => tc.cd_item_subgrupo == ip.cd_item_subgrupo).FirstOrDefault();
                        if (itemAlterar == null || itemAlterar.cd_item_subgrupo <= 0)
                        {
                            ip.cd_item = item.cd_item;
                            ip.cd_item_subgrupo = 0;
                            DataAccessItemSubgrupo.add(ip, false);
                        }
                        else
                        {

                            itemAlterar.cd_subgrupo_conta = ip.cd_subgrupo_conta;
                            itemAlterar.id_tipo_movimento = ip.id_tipo_movimento;
                            DataAccessItemSubgrupo.saveChanges(false);
                        }
                    }
                if (categoriaGrupo <= 0 && (!isMasterGeral))
                    throw new FinanceiroBusinessException(Utils.Messages.Messages.msgCategoriaItemZero, null, FinanceiroBusinessException.TipoErro.ERRO_CATEGORIA_GRUPO_ITEM, false);

                //Se for master ou não for master e a categoria for publica, fazer a alteração 
                if (isMasterGeral || (!isMasterGeral && categoriaGrupo == (int)GrupoEstoque.CategoriaEnum.PUBLICA))
                {
                    itens = changeValuesItem(item);
                    //regras para item do tipo de biblioteca
                    persistenciaBiblioteca(item, itens.cd_tipo_item);
                }
                itemUI = DataAccessItem.getItemUIbyId(item.cd_item, cdEscola);
                transaction.Complete();
            }
            return itemUI;
        }

        //se o usuario for master geral, ele vai poder gravar o item escola para todas as escolas, mas os valores desse item fica a critério de cada escola.
        private ItemEscola retonarItemEscolaSemValores(ItemUI item, int cdEscola, int cdItem)
        {
            ItemEscola itemEscolaRetorno = new ItemEscola();
            var itemEscolaBase = DataAccessItemEscola.findItemEscolabyId(cdItem, cdEscola);
            if (itemEscolaBase == null)
            {
                itemEscolaRetorno = setValueItemEscola(item, cdItem, cdEscola);
                itemEscolaRetorno = DataAccessItemEscola.add(itemEscolaRetorno, false);
            }
            return itemEscolaRetorno;
        }

        //Retorna valor do ItemEscola
        private ItemEscola retonarItemEscola(ItemUI item, int cdEscola, int cdItem)
        {
            ItemEscola itemEscolaRetorno = new ItemEscola();
            var itemEscolaBase = DataAccessItemEscola.findItemEscolabyId(cdItem, cdEscola);
            if (itemEscolaBase != null)
            {
                itemEscolaBase.qt_estoque = item.qt_estoque;
                itemEscolaBase.vl_custo = item.vl_custo;
                itemEscolaBase.vl_item = item.vl_item;
                itemEscolaBase.cd_plano_conta = item.cd_plano_conta > 0 ? item.cd_plano_conta : null;
                DataAccessItemEscola.saveChanges(false);
                return itemEscolaBase;
            }
            else
            {
                itemEscolaRetorno = setValueItemEscola(item, cdItem, cdEscola);
                itemEscolaRetorno = DataAccessItemEscola.add(itemEscolaRetorno, false);
                return itemEscolaRetorno;
            }
        }


        //Seta valor para a itemEscola
        private ItemEscola setValueItemEscola(ItemUI item, int cdItem, int cdEscolas)
        {
            ItemEscola itemEscola = new ItemEscola
            {
                cd_item = cdItem,
                cd_pessoa_escola = cdEscolas,
                vl_custo = item.vl_custo,
                qt_estoque = item.qt_estoque,
                vl_item = item.vl_item,
                cd_plano_conta = item.cd_plano_conta > 0 ? item.cd_plano_conta : null
            };
            return itemEscola;
        }


        //Persisência da biblioteca
        private Biblioteca persistenciaBiblioteca(ItemUI item, int itemBiblioteca)
        {
            Biblioteca biblioteca = new Biblioteca();
            bool isItemBiblioteca = DataAccessBiblioteca.findItemBibliotecaById(item.cd_item);
            if (itemBiblioteca == BIBLIOTECA && item.cd_tipo_item == BIBLIOTECA && isItemBiblioteca)
            {
                Biblioteca bibliotecaBase = new Biblioteca();
                bibliotecaBase = DataAccessBiblioteca.findById(item.cd_item, false);
                bibliotecaBase.dc_assunto = item.dc_assunto;
                bibliotecaBase.dc_local = item.dc_local;
                bibliotecaBase.dc_titulo = item.dc_titulo;
                bibliotecaBase.no_autor = item.no_autor;
                DataAccessBiblioteca.saveChanges(false);
            }
            else
            {
                if (isItemBiblioteca)
                {
                    var bibliotecaBase = DataAccessBiblioteca.findById(item.cd_item, false);
                    DataAccessBiblioteca.delete(bibliotecaBase, false);
                }
                else
                {
                    if (item.cd_tipo_item == BIBLIOTECA)
                    {
                        biblioteca = returnBiblioteca(item, item.cd_item);
                        DataAccessBiblioteca.add(biblioteca, false);
                    }
                }
            }
            return biblioteca;
        }

        //Changevalues do item
        private Item changeValuesItem(ItemUI item)
        {
            Item itemBase = DataAccessItem.findById(item.cd_item, false);
            itemBase.cd_grupo_estoque = item.cd_grupo_estoque;
            itemBase.cd_integracao = item.cd_integracao;
            itemBase.cd_origem_fiscal = item.cd_origem_fiscal;
            itemBase.cd_tipo_item = item.cd_tipo_item;
            itemBase.dc_classificacao_fiscal = item.dc_classificacao_fiscal;
            itemBase.dc_classificacao = item.dc_classificacao;
            itemBase.dc_codigo_barra = item.dc_codigo_barra;
            itemBase.dc_sgl_item = item.dc_sgl_item;
            itemBase.id_item_ativo = item.id_item_ativo;
            itemBase.id_material_didatico = item.id_material_didatico;
            itemBase.id_voucher_carga = item.id_voucher_carga;
            itemBase.qt_horas_carga = item.qt_horas_carga;
            itemBase.no_item = item.no_item;
            itemBase.pc_aliquota_icms = item.pc_aliquota_icms;
            itemBase.pc_aliquota_iss = item.pc_aliquota_iss;
            itemBase.cd_subgrupo_conta = item.cd_subgrupo_conta > 0 ? item.cd_subgrupo_conta : null;
            itemBase.cd_subgrupo_conta_2 = item.cd_subgrupo_conta_2 > 0 ? item.cd_subgrupo_conta_2 : null;
            itemBase.cd_cest = item.cd_cest;
            changeItensKit(itemBase.ItemKit, item.itemKit);
            
            //itemBase.cd_plano_conta = item.cd_plano_conta;
            DataAccessItem.saveChanges(false);
            return itemBase;
        }

        private void changeItensKit(ICollection<ItemKit> itemItemKitBase, ICollection<ItemKit> itemItemKit)
        {
            foreach (ItemKit i in itemItemKitBase)
            {
                foreach (var j in itemItemKit)
                {
                    if (i.cd_iitem_kit == j.cd_iitem_kit)
                    {
                        i.qt_item_kit = j.qt_item_kit;
                    }
                } 
            }
        }


        //Changevalues do item
        private ItemEscola changeValuesItemEscola(ItemUI item, ItemEscola itemBase, int cd_escola_logada, int cd_escola_item)
        {
            if (cd_escola_logada == cd_escola_item)
            {
                itemBase = DataAccessItemEscola.findItemEscolabyId(item.cd_item, cd_escola_item);
                itemBase.vl_custo = item.vl_custo;
                itemBase.qt_estoque = item.qt_estoque;
                itemBase.vl_item = item.vl_item;
                DataAccessItemEscola.saveChanges(false);
            }
            else
                retonarItemEscola(item, cd_escola_logada, itemBase.cd_item);

            return itemBase;
        }

        public IEnumerable<ItemUI> getItemSearchEstoque(SearchParameters parametros, string descricao, bool inicio, bool? ativo, int tipoItem, int cdGrupoItem, int cdEscola, bool comEstoque, List<int> cdItens, bool isMaster, int ano, int mes)
        {
            IEnumerable<ItemUI> retorno = new List<ItemUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "no_item";
                parametros.sort = parametros.sort.Replace("id_item_ativo", "item_ativo");
                parametros.sort = parametros.sort.Replace("pc_aliquota_iss", "iss");
                parametros.sort = parametros.sort.Replace("pc_aliquota_icms", "icms");

                retorno = DataAccessItem.getItemSearchEstoque(parametros, descricao, inicio, ativo, tipoItem, cdGrupoItem, cdEscola, comEstoque, cdItens, isMaster, ano,  mes);
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<RptItemFechamento> rptItemWithSaldoItem(int cd_pessoa_escola, int cd_item, int cd_grupo, byte cd_tipo, int ano, int mes, bool isContagem)
        {
            List<RptItemFechamento> listaFechameto = new List<RptItemFechamento>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (!isContagem)
                    listaFechameto = DataAccessItem.rptItemWithSaldoItem(cd_pessoa_escola, cd_item, cd_grupo, cd_tipo, ano, mes).ToList();
                else
                {
                    List<Item> itens = DataAccessItem.listItensWithEstoque(cd_pessoa_escola, cd_item, cd_grupo, cd_tipo).ToList();
                    itens = itens.GroupBy(i => new { i.GrupoEstoque.cd_grupo_estoque, i.cd_item }).Select(group => group.First()).ToList();

                    //Calcula a última data do mês de fechamento:
                    DateTime dataLimite = new DateTime(ano, mes, DateTime.DaysInMonth(ano, mes));

                    foreach (Item item in itens)
                    {
                        //Transforma um item em saldo de item:
                        RptItemFechamento itemFechamento = new RptItemFechamento
                        {
                            cd_grupo_estoque = item.GrupoEstoque.cd_grupo_estoque,
                            cd_item = item.cd_item,
                            cd_tipo_item = item.cd_tipo_item,
                            dc_grupo = item.GrupoEstoque.no_grupo_estoque,
                            dc_item = item.no_item,
                            nm_ano_fechamento = (short)ano,
                            nm_mes_fechamento = (byte)mes,
                            qt_entrada = 0,
                            qt_saida = 0,
                            saldo = 0,
                            saldo_atual = item.qt_estoque,
                            vl_custo = 0,
                            vl_item = 0
                        };

                        //Calcula o valor do saldo do item:
                        itemFechamento.saldo = DataAccessKardex.getSaldoItem(item.cd_item, dataLimite, cd_pessoa_escola);
                        itemFechamento.vl_custo = DataAccessItemEscola.getValorCusto(item.cd_item, cd_pessoa_escola);
                        itemFechamento.vl_item = (itemFechamento.saldo) * (itemFechamento.vl_custo);
                        listaFechameto.Add(itemFechamento);
                    }
                }
                transaction.Complete();
            }
            return listaFechameto;
        }

        public IEnumerable<RptItemFechamento> rptItemSaldoBiblioteca(int cd_pessoa_escola, int cd_item, int cd_grupo, byte cd_tipo)
        {
            List<RptItemFechamento> itensBiblioteca = new List<RptItemFechamento>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                DateTime dt_kardex = DataAccessKardex.getMaxMovimentoKardex(cd_pessoa_escola);
                itensBiblioteca = DataAccessItem.rptItemSaldoBiblioteca(cd_pessoa_escola, cd_item, cd_grupo, cd_tipo, dt_kardex).ToList();

                itensBiblioteca = itensBiblioteca.GroupBy(i => new { i.cd_grupo_estoque, i.cd_item }).Select(group => group.First()).ToList();
                transaction.Complete();
            }
            return itensBiblioteca;
        }

        public IEnumerable<ItemUI> listItensMaterial(int cd_curso, int cd_escola)
        {
            return DataAccessItem.listItensMaterial(cd_curso, cd_escola);
        }

        public ItemUI getItemUIbyId(int cd_item, int cdEscola)
        {
            return DataAccessItem.getItemUIbyId(cd_item, cdEscola);
        }

        public int quantidadeItensMaterialCurso(int cd_turma, int cd_escola)
        {
            return DataAccessItem.quantidadeItensMaterialCurso(cd_turma, cd_escola);
        }

        #endregion

        #region Politica Desconto
        public PoliticaDesconto getPoliticaEdit(int id, int cdEscola)
        {
            PoliticaDesconto politica = getPoliticaDescontoById(id, cdEscola);
            politica.DiasPolitica = GetDiasPoliticaById(id, cdEscola).ToList();
            politica.PoliticasAlunos = DataAccessPoliticaAluno.getAlunoPolitica(id, cdEscola).ToList();
            politica.PoliticasTurmas = DataAccessPoliticaTurma.getTurmaPolitica(id, cdEscola).ToList();
            politica.qtd_alunos = politica.PoliticasAlunos.Count();
            politica.qtd_turmas = politica.PoliticasTurmas.Count();

            return politica;
        }

        public PoliticaDesconto getPoliticaDescontoById(int id, int cdEscola)
        {
            return DataAccessPoliticaDesconto.GetPoliticaDescontoById(id, cdEscola);
        }

        public PoliticaDescontoUI postPoliticaDesconto(PoliticaDesconto politicaDesconto)
        {
            PoliticaDescontoUI politicaUI = new PoliticaDescontoUI();
            this.sincronizarContextos(DataAccessPoliticaDesconto.DB());
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                ICollection<DiasPolitica> dias = new List<DiasPolitica>();
                ICollection<PoliticaTurma> turmas = new List<PoliticaTurma>();
                
                bool existe = DataAccessPoliticaDesconto.existPolIgual(politicaDesconto);
                if (existe)
                    throw new FinanceiroBusinessException(Componentes.Utils.Messages.Messages.msgErrorRegistroDuplicado, null, FinanceiroBusinessException.TipoErro.ERRO_POLITICA_FINAN_DUPLICADA, false);

                dias = politicaDesconto.DiasPolitica;
                IEnumerable<PoliticaTurma> politicasTurmaView = politicaDesconto.PoliticasTurmas;
                IEnumerable<PoliticaAluno> politicasAlunoView = politicaDesconto.PoliticasAlunos;

                if (politicasTurmaView != null && politicasTurmaView.Count() <= 0 && politicasAlunoView != null && politicasAlunoView.Count() <= 0)
                {
                    bool existeBaixaAnterior = DataAccessBaixaFinan.verificaBaixaAposDataPol(politicaDesconto.cd_pessoa_escola, politicaDesconto.dt_inicial_politica);
                    if (existeBaixaAnterior)
                        throw new FinanceiroBusinessException(Messages.msgErroPolDescBaixas, null, FinanceiroBusinessException.TipoErro.ERRO_POLITICA_COM_BAIXA, false);
                }


                politicaDesconto.PoliticasTurmas = null;
                politicaDesconto.PoliticasAlunos = null;
                politicaDesconto.EscolaPoliticaDesc = null;
                politicaDesconto.DiasPolitica = null;

                politicaDesconto = DataAccessPoliticaDesconto.add(politicaDesconto, false);

                persistirPoliticaTurma(politicasTurmaView, politicaDesconto.cd_politica_desconto, politicaDesconto.cd_pessoa_escola, politicaDesconto.dt_inicial_politica);
                persistirPoliticaAluno(politicasAlunoView, politicaDesconto.cd_politica_desconto, politicaDesconto.cd_pessoa_escola, politicaDesconto.dt_inicial_politica);

                foreach (var c in dias)
                {
                    c.cd_politica_desconto = politicaDesconto.cd_politica_desconto;
                    c.DiasPoliticaDesc = null;
                    DataAccessPoliticaDesconto.addDiasPolitica(c);
                }
                var retornar = DataAccessPoliticaDesconto.getPoliticaDesconto(politicaDesconto.cd_pessoa_escola, politicaDesconto.cd_politica_desconto);
                politicaUI = retornar;
                transaction.Complete();
            }
            return politicaUI;
        }

        public bool deletePoliticaDesconto(PoliticaDesconto politicaDesconto)
        {
            PoliticaDesconto e = DataAccessPoliticaDesconto.findById(politicaDesconto.cd_pessoa_escola, false);
            var deleted = DataAccessPoliticaDesconto.delete(e, false);
            return deleted;
        }

        public IEnumerable<PoliticaDescontoUI> getPoliticaDescontoSearch(SearchParameters parametros, int cdTurma, int cdAluno, DateTime? dtaIni, DateTime? dtaFim, bool? ativo, int cdEscola)
        {
            IEnumerable<PoliticaDescontoUI> retorno = new List<PoliticaDescontoUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "dt_inicial";
                parametros.sort = "cd_politica_desconto";
                parametros.sort = parametros.sort.Replace("politica_desconto_ativo", "id_ativo");

                retorno = DataAccessPoliticaDesconto.GetPoliticaDescontoSearch(parametros, cdTurma, cdAluno, dtaIni, dtaFim, ativo, cdEscola);
                transaction.Complete();
            }
            return retorno;
        }

        public bool deleteAllPoliticaDesconto(List<PoliticaDesconto> politicasDesconto, int cdEscola)
        {
            bool deleted = false;
            this.sincronizarContextos(DataAccessPoliticaDesconto.DB());
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                foreach (PoliticaDesconto p in politicasDesconto)
                {
                    //Verifica se o desconto por antecipação está sendo usado:
                    if (!DataAccessPoliticaDesconto.verificaBaixaTituloByPolitica(p.cd_politica_desconto, cdEscola))
                        throw new FinanceiroBusinessException(Messages.msgPoliticaComBaixa, null, FinanceiroBusinessException.TipoErro.ERRO_POLITICA_COM_BAIXA, false);
                    List<DiasPolitica> listDiasPol = DataAccessDiasPolitica.GetDiasPoliticaById(p.cd_politica_desconto, cdEscola);
                    if (listDiasPol != null && listDiasPol.Count() > 0)
                        foreach (DiasPolitica dias in listDiasPol)
                        {
                            DiasPolitica diasPol = DataAccessDiasPolitica.findById(dias.cd_dias_politica, false);
                            DataAccessDiasPolitica.delete(diasPol, false);
                        }
                    //Deleta Aluno
                    IEnumerable<PoliticaAluno> politcaAlunosContext = DataAccessPoliticaAluno.getAlunoPoliticaFull(p.cd_politica_desconto, cdEscola);
                    foreach (var item in politcaAlunosContext)
                        DataAccessPoliticaAluno.deleteContext(item, false);

                    //Deleta Turma
                    IEnumerable<PoliticaTurma> politcaTurmasContext = DataAccessPoliticaTurma.getTurmaPoliticaFull(p.cd_politica_desconto, cdEscola);
                    foreach (var item in politcaTurmasContext)
                        DataAccessPoliticaTurma.deleteContext(item, false);


                    PoliticaDesconto pol = DataAccessPoliticaDesconto.GetPoliticaDescontoById(p.cd_politica_desconto, cdEscola);
                    if (pol != null && pol.cd_politica_desconto > 0)
                        deleted = DataAccessPoliticaDesconto.delete(pol, false);
                }
                transaction.Complete();
                return deleted;
            }
        }

        public IEnumerable<DiasPolitica> GetDiasPoliticaById(int cdPolitica, int cdEscola)
        {
            return DataAccessDiasPolitica.GetDiasPoliticaById(cdPolitica, cdEscola);
        }

        public IEnumerable<AlunosSemTituloGeradoUI> GetAlunosSemTituloGerado(int vl_mes, int ano, int cd_turma, string situacoes, int cd_escola)
        {
            return DataAccessTitulo.GetAlunosSemTituloGerado(vl_mes, ano, cd_turma, situacoes, cd_escola);
        }

        public PoliticaDescontoUI postAlterarPoliticaDesconto(PoliticaDesconto politica)
        {
            PoliticaDescontoUI politicaUI = new PoliticaDescontoUI();
            this.sincronizarContextos(DataAccessPoliticaDesconto.DB());
            
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                PoliticaDesconto polDesc = DataAccessPoliticaDesconto.GetPoliticaDescontoById(politica.cd_politica_desconto, politica.cd_pessoa_escola);
                bool existe = DataAccessPoliticaDesconto.existPolIgual(politica);
                List<DiasPolitica> diasContext = DataAccessDiasPolitica.GetDiasPoliticaById(politica.cd_politica_desconto, politica.cd_pessoa_escola);
                if (existe)
                    throw new FinanceiroBusinessException(Componentes.Utils.Messages.Messages.msgErrorRegistroDuplicado, null, FinanceiroBusinessException.TipoErro.ERRO_POLITICA_FINAN_DUPLICADA, false);

                bool existDiasPolDel = diasContext.Where(tc => !politica.DiasPolitica.Where(d => tc.cd_dias_politica == d.cd_dias_politica).Any()).Any();
                bool existDiasPolInsert = politica.DiasPolitica.Where(tc => !diasContext.Where(d => tc.cd_dias_politica == d.cd_dias_politica).Any()).Any();
                bool existDiasPolAlterou = politica.DiasPolitica.Where(tc => diasContext.Where(d => tc.cd_dias_politica == d.cd_dias_politica &&
                                                                                                    (tc.nm_dia_limite_politica != d.nm_dia_limite_politica ||
                                                                                                    tc.pc_desconto != d.pc_desconto)).Any()).Any();
                bool alterouData = politica.dt_inicial_politica != polDesc.dt_inicial_politica ? true : false;
                if (existDiasPolDel || existDiasPolInsert || existDiasPolAlterou || alterouData)
                    //Verifica se o desconto por antecipação está sendo usado:
                    if (!DataAccessPoliticaDesconto.verificaBaixaTituloByPolitica(politica.cd_politica_desconto, politica.cd_pessoa_escola))
                        throw new FinanceiroBusinessException(Messages.msgPoliticaComBaixa, null, FinanceiroBusinessException.TipoErro.ERRO_POLITICA_COM_BAIXA, false);
                persistirPoliticaAluno(politica.PoliticasAlunos, politica.cd_politica_desconto, politica.cd_pessoa_escola, politica.dt_inicial_politica);
                persistirPoliticaTurma(politica.PoliticasTurmas, politica.cd_politica_desconto, politica.cd_pessoa_escola, politica.dt_inicial_politica);


                // Editando dias do desconto por antecipação
                List<DiasPolitica> itensDeleted = diasContext.Where(tc => !politica.DiasPolitica.Any(tv => tc.cd_dias_politica == tv.cd_dias_politica)).ToList();
                if (itensDeleted != null && itensDeleted.Count() > 0)
                    foreach (DiasPolitica ip in itensDeleted)
                    {
                        DiasPolitica item = DataAccessDiasPolitica.findById(ip.cd_dias_politica, false);
                        item.DiasPoliticaDesc = null;
                        DataAccessDiasPolitica.delete(item, false);
                    }
                //Alterando ou Incluindo novos itens

                foreach (DiasPolitica ip in politica.DiasPolitica)
                    if (ip.cd_dias_politica == 0)
                    {
                        ip.cd_politica_desconto = politica.cd_politica_desconto;
                        DataAccessDiasPolitica.add(ip, false);
                    }
                    else
                    {
                        DiasPolitica item = diasContext.Where(tc => tc.cd_dias_politica == ip.cd_dias_politica).FirstOrDefault();
                        item.nm_dia_limite_politica = ip.nm_dia_limite_politica;
                        item.pc_desconto = ip.pc_desconto;
                        DataAccessDiasPolitica.saveChanges(false);
                    }
                politica.DiasPolitica = null;
                politica.PoliticasAlunos = null;
                politica.PoliticasTurmas = null;


                polDesc.copy(politica);
                DataAccessPoliticaDesconto.saveChanges(false);
                var retornar = DataAccessPoliticaDesconto.getPoliticaDesconto(politica.cd_pessoa_escola, politica.cd_politica_desconto);
                politicaUI = retornar;
                transaction.Complete();
                return politicaUI;
            }

        }

        public PoliticaDescontoUI getPoliticaDesconto(int cdEscola, int cd_politica_desconto)
        {
            return DataAccessPoliticaDesconto.getPoliticaDesconto(cdEscola, cd_politica_desconto);
        }

        public PoliticaDesconto getPoliticaDescontoByTurmaAluno(int cd_turma, int cd_aluno, DateTime dt_vcto_titulo)
        {
            return DataAccessPoliticaDesconto.getPoliticaDescontoByTurmaAluno(cd_turma, cd_aluno, dt_vcto_titulo);
        }

        public PoliticaDesconto getPoliticaDescontoByAluno(int cd_aluno, DateTime dt_vcto_titulo)
        {
            return DataAccessPoliticaDesconto.getPoliticaDescontoByAluno(cd_aluno, dt_vcto_titulo);
        }

        public PoliticaDesconto getPoliticaDescontoByTurma(int cd_turma, DateTime dt_vcto_titulo)
        {
            return DataAccessPoliticaDesconto.getPoliticaDescontoByTurma(cd_turma, dt_vcto_titulo);
        }

        public PoliticaDesconto getPoliticaDescontoByEscola(int cd_pessoa_escola, DateTime dt_vcto_titulo)
        {
            return DataAccessPoliticaDesconto.getPoliticaDescontoByEscola(cd_pessoa_escola, dt_vcto_titulo);
        }
        public int getCriarDataPoliticaContrato(int cdTurma, int cdAluno, DateTime dataVencto, int cdEscola)
        {
            int dataVencimento = 0;
            PoliticaDesconto politica = null;
            if (cdTurma > 0) //Desconto do aluno e turma
                politica = getPoliticaDescontoByTurmaAluno(cdTurma, cdAluno, dataVencto);
            if (politica == null) //Desconto do aluno
                politica = getPoliticaDescontoByAluno(cdAluno, dataVencto);
            if (politica == null && cdTurma > 0) //Desconto da turma
                politica = getPoliticaDescontoByTurma(cdTurma, dataVencto);
            if (politica == null) //Desconto da escola
                politica = getPoliticaDescontoByEscola(cdEscola, dataVencto);

            if (politica != null && politica.DiasPolitica.Count() > 0)
            {
                List<DiasPolitica> dias = politica.DiasPolitica.OrderBy(o => o.nm_dia_limite_politica).ToList();
                foreach (DiasPolitica d in dias)
                {
                    try
                    {
                        if (d.nm_dia_limite_politica > 0)
                            dataVencimento = new DateTime(dataVencto.Year, dataVencto.Month, d.nm_dia_limite_politica).Day;
                    }
                    catch
                    {
                        //DateTime com o último dia do mês
                        int ultimoDiaDoMes = new DateTime(dataVencto.Year, dataVencto.Month, DateTime.DaysInMonth(dataVencto.Year, dataVencto.Month)).Day;
                        if ((d.nm_dia_limite_politica - 1) > ultimoDiaDoMes)
                        {
                            var dia = (d.nm_dia_limite_politica - 1);
                            for (int i = 0; i < ultimoDiaDoMes; i++)
                            {
                                if (dia <= ultimoDiaDoMes)
                                {
                                    dataVencimento = dia;
                                    break;
                                }
                                dia -= 1;
                            }
                        }
                        else
                        {
                            dataVencimento = new DateTime(dataVencto.Year, dataVencto.Month, (d.nm_dia_limite_politica - 1)).Day;
                        }
                    }
                }
            }
            else
                dataVencimento = dataVencto.Day;


            return dataVencimento;
        }

        private void persistirPoliticaTurma(IEnumerable<PoliticaTurma> politicaTurmasView, int cd_politica_desconto, int cdEscola, DateTime dtPol)
        {
            //Pega as politicas da turma na base de dados
            IEnumerable<PoliticaTurma> politcaTurmasContext = DataAccessPoliticaTurma.getTurmaPoliticaFull(cd_politica_desconto, cdEscola);
            //Pega as politicas da view

            if (politicaTurmasView != null)
            {
                //Incluir politica turma                
                List<PoliticaTurma> novasPoliticaTurmas = politicaTurmasView.Where(p => !politcaTurmasContext.Where(c => c.cd_turma == p.cd_turma).Any()).ToList();
                foreach (var novaPoliticaTurma in novasPoliticaTurmas)
                {
                    bool existeBaixaTurma = DataAccessBaixaFinan.verificaBaixaTituloByTurma(novaPoliticaTurma.cd_turma, cdEscola, dtPol);
                    if (existeBaixaTurma)
                        throw new FinanceiroBusinessException(Messages.msgErroTurmaBaixa, null, FinanceiroBusinessException.TipoErro.ERRO_POLITICA_COM_BAIXA, false);
                    novaPoliticaTurma.cd_politica_desconto = cd_politica_desconto;
                    DataAccessPoliticaTurma.addContext(novaPoliticaTurma, false);
                }
                //Deleta o registro que esta na base mas não esta na view.
                List<PoliticaTurma> excluirPoliticaTurmas = politcaTurmasContext.Where(p => !politicaTurmasView.Where(c => c.cd_turma == p.cd_turma).Any()).ToList();

                foreach (var item in excluirPoliticaTurmas)
                {
                    bool existeBaixaPolTurma = DataAccessBaixaFinan.verificaBaixaTituloByPolTurma(item.cd_turma, cdEscola, cd_politica_desconto);
                    if (existeBaixaPolTurma)
                        throw new FinanceiroBusinessException(Messages.msgErroTurmaPolBaixa, null, FinanceiroBusinessException.TipoErro.ERRO_POLITICA_COM_BAIXA, false);
                    DataAccessPoliticaTurma.deleteContext(item, false);
                }
            }
            else
            {
                if (politcaTurmasContext != null)
                {
                    foreach (var item in politcaTurmasContext)
                    {
                        bool existeBaixaPolTurma = DataAccessBaixaFinan.verificaBaixaTituloByPolTurma(item.cd_turma, cdEscola, cd_politica_desconto);
                        if (existeBaixaPolTurma)
                            throw new FinanceiroBusinessException(Messages.msgErroTurmaPolBaixa, null, FinanceiroBusinessException.TipoErro.ERRO_POLITICA_COM_BAIXA, false);
                        DataAccessPoliticaTurma.deleteContext(item, false);
                    }
                }
            }
            DataAccessPoliticaTurma.saveChanges(false);
        }

        private void persistirPoliticaAluno(IEnumerable<PoliticaAluno> politicaAlunosView, int cd_politica_desconto, int cdEscola, DateTime dtPol)
        {
            //Pega as politicas da Aluno na base de dados
            IEnumerable<PoliticaAluno> politcaAlunosContext = DataAccessPoliticaAluno.getAlunoPoliticaFull(cd_politica_desconto, cdEscola);
            //Pega as politicas da view
            if (politicaAlunosView != null)
            {
                //Incluir politica Aluno
                List<PoliticaAluno> novasPoliticaAlunos = politicaAlunosView.Where(p => !politcaAlunosContext.Where(c => c.cd_aluno == p.cd_aluno).Any()).ToList();
                foreach (var novaPoliticaAluno in novasPoliticaAlunos)
                {
                    bool existeBaixaAluno = DataAccessBaixaFinan.verificaBaixaTituloByAluno(novaPoliticaAluno.cd_aluno, cdEscola, dtPol);
                    if (existeBaixaAluno)
                        throw new FinanceiroBusinessException(Messages.msgErroAlunoBaixa, null, FinanceiroBusinessException.TipoErro.ERRO_POLITICA_COM_BAIXA, false);
                    novaPoliticaAluno.cd_politica_desconto = cd_politica_desconto;
                    DataAccessPoliticaAluno.addContext(novaPoliticaAluno, false);
                }

                //Deleta o registro que esta na base mas não esta na view.
                List<PoliticaAluno> excluirPoliticaAlunos = politcaAlunosContext.Where(p => !politicaAlunosView.Where(c => c.cd_aluno == p.cd_aluno).Any()).ToList();

                foreach (var item in excluirPoliticaAlunos)
                {
                    bool existeBaixaPolAluno = DataAccessBaixaFinan.verificaBaixaTituloByPolAluno(item.cd_aluno, cdEscola, cd_politica_desconto);
                    if (existeBaixaPolAluno)
                        throw new FinanceiroBusinessException(Messages.msgErroAlunoPolBaixa, null, FinanceiroBusinessException.TipoErro.ERRO_POLITICA_COM_BAIXA, false);
                    DataAccessPoliticaAluno.deleteContext(item, false);
                }

            }
            else
            {
                if (politcaAlunosContext != null)
                {
                    foreach (var item in politcaAlunosContext)
                    {
                        bool existeBaixaPolAluno = DataAccessBaixaFinan.verificaBaixaTituloByPolAluno(item.cd_aluno, cdEscola, cd_politica_desconto);
                        if (existeBaixaPolAluno)
                            throw new FinanceiroBusinessException(Messages.msgErroAlunoPolBaixa, null, FinanceiroBusinessException.TipoErro.ERRO_POLITICA_COM_BAIXA, false);
                        DataAccessPoliticaAluno.deleteContext(item, false);
                    }
                }
            }
            DataAccessPoliticaAluno.saveChanges(false);
        }

        #endregion

        #region Item Escola

        public ItemEscola addItemEscola(ItemEscola itemEscola)
        {
            return DataAccessItemEscola.add(itemEscola, false);
        }

        public ItemEscola findItemEscolabyId(int cdItem, int cd_escola)
        {
            return DataAccessItemEscola.findItemEscolabyId(cdItem, cd_escola);
        }

        public ICollection<ItemEscola> getItensWithEscola(int cdItem, int cdUsuario)
        {
            return DataAccessItemEscola.getItensWithEscola(cdItem, cdUsuario);
        }

        #endregion

        #region Tipo Item

        public IEnumerable<TipoItem> getAllTipoItem(int? tipoMovimento)
        {
            return DataAccessTipoItem.getTipoItemSearch(tipoMovimento);
        }

        public IEnumerable<TipoItem> getTipoItemMovimento(TipoItemDataAccess.TipoConsultaTipoItemEnum tipoConsulta)
        {
            return DataAccessTipoItem.getTipoItemMovimento(tipoConsulta);
        }
        public IEnumerable<TipoItem> getTipoItemMovimentoWithItem(int cd_pessoa_escola, bool isMaster)
        {
            IEnumerable<TipoItem> lista = new List<TipoItem>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED, DataAccessTipoItem.DB(), TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                lista = DataAccessTipoItem.getTipoItemMovimentoWithItem(cd_pessoa_escola, isMaster).ToList();
                transaction.Complete();
            }
            return lista;
        }

        public IEnumerable<TipoItem> getTipoItemMovimentoEstoque()
        {
            return DataAccessTipoItem.getTipoItemMovimentoEstoque();
        }
        #endregion

        #region Biblioteca

        public Biblioteca addBiblioteca(Biblioteca biblioteca)
        {
            return DataAccessBiblioteca.add(biblioteca, false);
        }

        public bool findItemBibliotecaById(int cdItem)
        {
            return DataAccessBiblioteca.findItemBibliotecaById(cdItem);
        }
        #endregion

        #region Tabela de Preço

        public int? getNroParcelas(int cd_escola, int cd_curso, int cd_regime, int cd_duracao, DateTime data_matricula)
        {
            return DataAccessTabelaPreco.getNroParcelas(cd_escola, cd_curso, cd_regime, cd_duracao, data_matricula);
        }

        public IEnumerable<TabelaPrecoUI> GetTabelaPrecoSearch(SearchParameters parametros, int cdCurso, int cdDuracao, int cdRegime, DateTime? dtaCad, int cdEscola, int cdProduto)
        {
            IEnumerable<TabelaPrecoUI> retorno = new List<TabelaPrecoUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "ordem";

                parametros.sort = parametros.sort.Replace("dt_tabela", "dta_tabela_preco");
                parametros.sort = parametros.sort.Replace("vlAula", "vl_aula");
                parametros.sort = parametros.sort.Replace("vl_parc", "vl_parcela");
                parametros.sort = parametros.sort.Replace("vl_mat", "vl_matricula");

                IEnumerable<TabelaPrecoUI> tabela = DataAccessTabelaPreco.GetTabelaPrecoSearch(parametros, cdCurso, cdDuracao, cdRegime, dtaCad, cdEscola, cdProduto);
                List<TabelaPrecoUI> tabelaLista = tabela.ToList();
                for (int i = 0; i < tabela.Count(); i++)
                    tabelaLista[i].vl_total = tabelaLista[i].nm_parcelas * tabelaLista[i].vl_parcela;
                retorno = tabelaLista;
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<TabelaPrecoUI> GetHistoricoTabelaPreco(SearchParameters parametros, int cdCurso, int cdDuracao, int cdRegime, int cdEscola)
        {
            if (parametros.sort == null)
            {
                parametros.sort = "dta_tabela_preco";
                parametros.sortOrder = SortDirection.Descending;
            }
            parametros.sort = parametros.sort.Replace("dt_tabela", "dta_tabela_preco");
            //  parametros.sort = parametros.sort.Replace("vl_ttl", "vl_total");
            parametros.sort = parametros.sort.Replace("vl_parc", "vl_parcela");
            parametros.sort = parametros.sort.Replace("vl_mat", "vl_matricula");

            IEnumerable<TabelaPrecoUI> tabela = DataAccessTabelaPreco.GetHistoricoTabelaPreco(parametros, cdCurso, cdDuracao, cdRegime, cdEscola);
            List<TabelaPrecoUI> tabelaLista = tabela.ToList();
            for (int i = 0; i < tabela.Count(); i++)
                tabelaLista[i].vl_total = tabelaLista[i].nm_parcelas * tabelaLista[i].vl_parcela;

            return tabelaLista;
        }

        public TabelaPreco getTabelaPrecoById(int id)
        {
            return DataAccessTabelaPreco.findById(id, false);
        }

        public TabelaPrecoUI postTabelaPreco(TabelaPreco tabelaPreco)
        {
            TabelaPrecoUI tabelaUI = new TabelaPrecoUI();

            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                tabelaPreco = DataAccessTabelaPreco.add(tabelaPreco, false);

                tabelaUI = DataAccessTabelaPreco.GetTabelaPrecoById(tabelaPreco.cd_tabela_preco, tabelaPreco.cd_pessoa_escola);
                tabelaUI.vl_total = tabelaUI.nm_parcelas * tabelaUI.vl_parcela;


                DataAccessTabelaPreco.saveChanges(true);
                transaction.Complete();
            }
            return tabelaUI;
        }

        public bool deleteTabelaPreco(TabelaPreco tabela)
        {
            bool deleted = false;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                TabelaPreco e = DataAccessTabelaPreco.findById(tabela.cd_tabela_preco, false);
                deleted = DataAccessTabelaPreco.delete(e, false);
                transaction.Complete();
                return deleted;
            }
        }

        public bool deleteAllTabelaPreco(List<TabelaPreco> tabelas, int cdEscola)
        {
            bool deleted = false;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                if (tabelas != null && tabelas.Count() > 0)
                    foreach (TabelaPreco t in tabelas)
                    {
                        TabelaPreco tabela = DataAccessTabelaPreco.GetTabelaById(t.cd_tabela_preco, cdEscola);
                        if (tabela != null)
                            deleted = DataAccessTabelaPreco.delete(tabela, false);
                    }

                //deleted = DataAccessTabelaPreco.deleteAllTabelaPreco(tabelas);
                transaction.Complete();
                return deleted;
            }
        }

        public TabelaPrecoUI postAlterarTabelaPreco(TabelaPreco tabela)
        {
            TabelaPrecoUI tabelaUI = new TabelaPrecoUI();
            TabelaPreco antiga = DataAccessTabelaPreco.findById(tabela.cd_tabela_preco, false);
            antiga.copy(tabela, true);
            tabela = DataAccessTabelaPreco.edit(antiga, false);
            tabelaUI = DataAccessTabelaPreco.GetTabelaPrecoById(tabela.cd_tabela_preco, tabela.cd_pessoa_escola);
            tabelaUI.vl_total = tabelaUI.nm_parcelas * tabelaUI.vl_parcela;
            return tabelaUI;
        }

        public Valores getValoresForMatricula(int cd_pessoa_escola, int cd_curso, int cd_duracao, int cd_regime, DateTime dta_matricula)
        {
            Valores valores = new Valores();
            TabelaPreco tabelaPreco = new TabelaPreco();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                tabelaPreco = DataAccessTabelaPreco.getValoresForMatricula(cd_pessoa_escola, cd_curso, cd_duracao, cd_regime, dta_matricula);
                transaction.Complete();
            }

            if (tabelaPreco != null)
                valores = new Valores
                {
                    nm_parcelas = tabelaPreco.nm_parcelas,
                    vl_aula = tabelaPreco.vl_aula,
                    vl_matricula = tabelaPreco.vl_matricula,
                    vl_parcela = tabelaPreco.vl_parcela,
                    vl_total = tabelaPreco.nm_parcelas * tabelaPreco.vl_parcela
                };

            return valores;
        }

        #endregion

        #region Plano Contas

        public IEnumerable<RptPlanoTitulo> getPlanosContaPosicaoFinanceira(int cd_titulo)
        {
            IEnumerable<RptPlanoTitulo> retorno = new List<RptPlanoTitulo>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = DataAccessPlanoTitulo.getPlanosContaPosicaoFinanceira(cd_titulo).ToList().OrderBy(pt => pt.no_subgrupo_conta);
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<PlanoConta> getPlanoContasSearch(int cd_pessoa_empresa)
        {
            return DataAccessPlanoConta.getPlanoContasSearch(cd_pessoa_empresa);
        }

        public PlanoContaUI addPlanoConta(ICollection<PlanoConta> planosContas, int tipoPlano, int cd_pessoa_empresa, byte nivel, out bool was_persisted, bool isContaSegura)
        {
            PlanoConta planoConta = new PlanoConta();
            PlanoContaUI planoContaUI = new PlanoContaUI();
            PlanoConta planoContaBD = new PlanoConta();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                was_persisted = false;

                if (!isContaSegura)
                {
                    bool existsConta = planosContas.Any(pl => pl.id_conta_segura == true);
                    if (existsConta)
                        throw new FinanceiroBusinessException(Utils.Messages.Messages.msgNotPersistirContaSegura, null, FinanceiroBusinessException.TipoErro.ERRO_PERMISSAO_PLANO_CONTAS_SEGURA, false);
                }

                if (tipoPlano == (int)PlanoContasDataAccess.TipoPlanoContaConsulta.MEU_PLANO_CONTA && planosContas != null && planosContas.Count() > 0)
                {
                    foreach (var plano in planosContas)
                    {
                        planoContaBD = DataAccessPlanoConta.confirmSubGrupoHasPlanoByIdSubgrupo(plano.cd_subgrupo_conta, cd_pessoa_empresa);
                        //planoContaBD = DataAccessPlanoConta.findById(plano.cd_plano_conta, false);
                        if (planoContaBD != null && planoContaBD.cd_plano_conta > 0)
                        {
                            int count = changeValuesPlanoConta(planoContaBD, plano, cd_pessoa_empresa);
                            if (count > 0) was_persisted = true;
                        }
                        else
                        {
                            planoConta = new PlanoConta
                            {
                                cd_pessoa_empresa = cd_pessoa_empresa,
                                cd_subgrupo_conta = plano.cd_subgrupo_conta,
                                id_ativo = plano.id_ativo,
                                id_conta_segura = plano.id_conta_segura,
                                id_tipo_conta = plano.id_tipo_conta == 0 ? null : plano.id_tipo_conta
                            };
                            planoConta = DataAccessPlanoConta.addContext(planoConta, false);
                            was_persisted = DataAccessPlanoConta.saveChanges(false) > 0;

                            //Atualiza os planos de contas dos itens 
                            IEnumerable<ItemEscola> popularPlano = DataAccessItemEscola.getItemComSubgrupoByEscola(plano.cd_subgrupo_conta, cd_pessoa_empresa);
                            if (popularPlano != null && popularPlano.Count() > 0 && plano.id_ativo)
                            {
                                foreach (ItemEscola item in popularPlano)
                                    item.cd_plano_conta = planoConta.cd_plano_conta;
                                DataAccessItemEscola.saveChanges(false);
                            }
                        }
                    }
                }
                planoContaUI.nivel_plano_conta = nivel;
                bool hasGrupoContaDisp = DataAccessGrupoConta.getGrupoContasWhitOutPlanoContas((byte)nivel, cd_pessoa_empresa);
                var existsPlanoConta = DataAccessGrupoConta.getPlanoContasTreeSearch(cd_pessoa_empresa, false, false, "", false).Count() > 0;
                if (!existsPlanoConta)
                    planoContaUI.hasGrupoSubGrupoDisponivel = hasGrupoContaDisp;
                else
                    planoContaUI.hasGrupoSubGrupoDisponivel = false;
                transaction.Complete();
            }
            return planoContaUI;
        }

        public PlanoContaUI excluirPlanoConta(ICollection<PlanoConta> planosContas, int tipoPlano, int cd_pessoa_empresa, int nivel, out bool was_deleted)
        {
            PlanoContaUI planoContaUI = new PlanoContaUI();
            PlanoConta planoContaBD = new PlanoConta();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                was_deleted = false;

                if (tipoPlano == (int)PlanoContasDataAccess.TipoPlanoContaConsulta.DISPONIVEIS && planosContas != null && planosContas.Count() > 0)
                {
                    foreach (var plano in planosContas)
                    {
                        planoContaBD = DataAccessPlanoConta.confirmSubGrupoHasPlanoByIdSubgrupo(plano.cd_subgrupo_conta, cd_pessoa_empresa);
                        if (planoContaBD != null)
                            was_deleted = DataAccessPlanoConta.delete(planoContaBD, false);
                    }
                }
                planoContaUI.nivel_plano_conta = nivel;
                bool hasGrupoContaDisp = DataAccessGrupoConta.getGrupoContasWhitOutPlanoContas((byte)nivel, cd_pessoa_empresa);
                var existsPlanoConta = DataAccessGrupoConta.getPlanoContasTreeSearch(cd_pessoa_empresa, false, false, "", false).Count() > 0;
                if (!existsPlanoConta)
                    planoContaUI.hasGrupoSubGrupoDisponivel = hasGrupoContaDisp;
                else
                    planoContaUI.hasGrupoSubGrupoDisponivel = false;
                transaction.Complete();
            }
            return planoContaUI;

        }

        public PlanoConta confirmSubGrupoHasPlanoByIdSubgrupo(int cd_sub_grupo, int cd_pessoa_empresa)
        {
            return DataAccessPlanoConta.confirmSubGrupoHasPlanoByIdSubgrupo(cd_sub_grupo, cd_pessoa_empresa);
        }

        private int changeValuesPlanoConta(PlanoConta planoContaBD, PlanoConta planoContaView, int cd_pessoa_empresa)
        {
            //Se o plano de contas estava inativo e passou para ativo, verificar se existe item que devia usar esse subgtupo
            if (!planoContaBD.id_ativo && planoContaView.id_ativo)
            {
                IEnumerable<ItemEscola> popularPlano = DataAccessItemEscola.getItemComSubgrupoByEscola(planoContaView.cd_subgrupo_conta, cd_pessoa_empresa);
                if (popularPlano != null && popularPlano.Count() > 0)
                {
                    foreach (ItemEscola item in popularPlano)
                        item.cd_plano_conta = planoContaView.cd_plano_conta;
                    DataAccessItemEscola.saveChanges(false);
                }
            }
            planoContaBD.cd_pessoa_empresa = cd_pessoa_empresa;
            planoContaBD.cd_subgrupo_conta = planoContaView.cd_subgrupo_conta;
            planoContaBD.id_ativo = planoContaView.id_ativo;
            planoContaBD.id_tipo_conta = planoContaView.id_tipo_conta;
            planoContaBD.id_conta_segura = planoContaView.id_conta_segura;

            return DataAccessPlanoConta.saveChanges(false);
        }

        public string getDescPlanoContaByEscola(int cd_pessoa_empresa, int cd_plano_conta)
        {
            return DataAccessPlanoConta.getDescPlanoContaByEscola(cd_pessoa_empresa, cd_plano_conta);
        }

        #endregion

        #region Cheque

        public Cheque getChequeByContrato(int id)
        {
            return DataAccessCheque.getChequeByContrato(id);
        }

        public Cheque getChequeByContratoPesq(int id)
        {
            Cheque cheque = new Cheque();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                cheque = DataAccessCheque.getChequeByContrato(id);
                transaction.Complete();
            }
            return cheque;
        }

        public bool excluirCheque(Cheque cheque)
        {
            bool ok = false;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                ok = DataAccessCheque.delete(cheque, false);
                transaction.Complete();
            }
            return ok;

        }

        public Cheque addCheque(Cheque cheque)
        {
            DataAccessCheque.add(cheque, false);
            return cheque;
        }

        public ChequeTransacao addChequeTransacao(ChequeTransacao chequeTran)
        {
            DataAccessChequeTransacao.add(chequeTran, false);
            return chequeTran;
        }

        public Cheque getChequeById(int cd_cheque)
        {

            return DataAccessCheque.findById(cd_cheque, false); 
        }


        public bool deleteCheque(Cheque cheque)
        {
            bool deleted = false;
            deleted = DataAccessCheque.delete(cheque, false);
            return deleted;
        }

        public Cheque editCheque(Cheque cheque)
        {
            DataAccessCheque.edit(cheque, false);
            return cheque;

        }

        public ChequeTransacao editChequeTransacao(ChequeTransacao chequeTran)
        {
            DataAccessChequeTransacao.edit(chequeTran, false);
            return chequeTran;

        }

        public List<Cheque> getChequesByTitulosContrato(List<int> cdTitulos, int cd_empresa)
        {
            return DataAccessCheque.getChequesByTitulosContrato(cdTitulos, cd_empresa);
        }

        public Cheque getChequeTransacao(int cd_tran_finan, int cd_empresa)
        {
            return DataAccessCheque.getChequeTransacao(cd_tran_finan, cd_empresa);
        }
        #endregion

        #region Título

        public IEnumerable<RptReceberPagar> receberPagarStoreProcedure(int cdEscola, DateTime pDtaI, DateTime pDtaF, int pForn, DateTime pDtaBase, byte pNatureza, int pPlanoContas, int ordem, bool cSegura, int cdTpLiq, int cdTpFinan, int tipo, string situacoes, int cdTurma)
        {
            IEnumerable<RptReceberPagar> result = new List<RptReceberPagar>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED, DataAccessTitulo.DB(), TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                result = DataAccessTitulo.receberPagarStoreProcedure(cdEscola, pDtaI, pDtaF, pForn, pDtaBase, pNatureza, pPlanoContas, ordem, cSegura, cdTpLiq, cdTpFinan, tipo, situacoes, cdTurma).ToList();
                transaction.Complete();
            }
            return result;
        }

        public IEnumerable<ObservacaoBaixaUI> getObservacoesBaixaCancelamento(int cdEscola, int cd_baixa_titulo)
        {
            IEnumerable<ObservacaoBaixaUI> result = new List<ObservacaoBaixaUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED, DataAccessTitulo.DB(), TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                result = DataAccessTitulo.getObservacoesBaixaCancelamento(cdEscola, cd_baixa_titulo).ToList();
                transaction.Complete();
            }
            return result;
        }

        public IEnumerable<RptRecebidaPaga> recebidaPagaStoreProcedure(int cdEscola, DateTime pDtaI, DateTime pDtaF, int pForn, byte pNatureza, int pPlanoContas, bool pMostraCCManual, bool cSegura, int cdTpLiq, int cdTpFinan, int tipo, string situacoes, int cdTurma, bool? ckCancelamento, int cdLocal)
        {
            List<RptRecebidaPaga> retorno = new List<RptRecebidaPaga>();

            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED, DataAccessTitulo.DB(), TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                retorno = DataAccessTitulo.recebidaPagaStoreProcedure(cdEscola, pDtaI, pDtaF, pForn, pNatureza, pPlanoContas, cSegura, cdTpLiq, cdTpFinan, tipo, situacoes, cdTurma, ckCancelamento, cdLocal).ToList();


                if (pMostraCCManual && (cdTpLiq != (int)TipoLiquidacao.TipoLiqui.CANCELAMENTO && (ckCancelamento == null || ckCancelamento == false)))
                    retorno.AddRange(DataAccessContaCorrente.recebidaPagaStoreProcedure(cdEscola, pDtaI, pDtaF, pForn, pNatureza, pPlanoContas, cSegura, cdTpLiq, cdTpFinan, tipo, situacoes, cdTurma, cdLocal).ToList());

                //Refaz o rateamento dos valores na visão de plano de contas:
                //for (int i = 0; i < retorno.Count; i++)
                //    if (retorno[i].vl_plano_titulo.HasValue)
                //    { //Visão de plano de contas
                //        if (retorno[i].vl_titulo.HasValue && retorno[i].vl_titulo.Value != 0)
                //        {
                //            //retorno[i].vl_acr_baixa = retorno[i].vl_plano_titulo.Value * retorno[i].vl_acr_baixa / retorno[i].vl_titulo.Value;
                //            retorno[i].vl_desconto_baixa = retorno[i].vl_plano_titulo.Value * retorno[i].vl_desconto_baixa / retorno[i].vl_titulo.Value;
                //            retorno[i].vl_multa_baixa = retorno[i].vl_plano_titulo.Value * retorno[i].vl_multa_baixa / retorno[i].vl_titulo.Value;
                //            retorno[i].vl_saldo_titulo = retorno[i].vl_plano_titulo.Value * retorno[i].vl_saldo_titulo / retorno[i].vl_titulo.Value;
                //            retorno[i].vl_liquidacao_baixa = retorno[i].vl_plano_titulo.Value * retorno[i].vl_liquidacao_baixa / retorno[i].vl_titulo.Value;
                //            retorno[i].vl_juros_baixa = retorno[i].vl_plano_titulo.Value * retorno[i].vl_juros_baixa / retorno[i].vl_titulo.Value;
                //            retorno[i].vl_desconto_baixa = retorno[i].vl_plano_titulo.Value * retorno[i].vl_desconto_baixa / retorno[i].vl_titulo.Value;
                //            retorno[i].vl_saldo_titulo = retorno[i].vl_plano_titulo.Value * retorno[i].vl_saldo_titulo / retorno[i].vl_titulo.Value;
                //        }
                //        retorno[i].vl_principal_baixa = retorno[i].vl_titulo = retorno[i].vl_plano_titulo.Value;
                //    }

                transaction.Complete();
            }
            return retorno;
        }

        public List<Titulo> addTitulos(List<Titulo> titulos)
        {
            List<Titulo> newTitulos = new List<Titulo>();
            DateTime maxDate = new DateTime(2079, 06, 06);
            DateTime minDate = new DateTime(1900, 01, 01);
            foreach (Titulo t in titulos)
            {
                if (t.dt_emissao_titulo.Date <= minDate)
                    throw new FinanceiroBusinessException(Messages.msgErroDtaEmisTituloSuperior, null, FinanceiroBusinessException.TipoErro.ERRO_DATA_EMISSAO_TITULO_SUPERIOR, false);
                if (t.dt_emissao_titulo.Date >= maxDate)
                    throw new FinanceiroBusinessException(Messages.msgErroDtaEmisTituloInf, null, FinanceiroBusinessException.TipoErro.ERRO_DATA_EMISSAO_TITULO_INFERIOR, false);

                if (t.dt_vcto_titulo.Date <= minDate)
                    throw new FinanceiroBusinessException(Messages.msgErroDtaVencTituloSuperior, null, FinanceiroBusinessException.TipoErro.ERRO_DATA_VENCIMENTO_TITULO_SUPERIOR, false);
                if (t.dt_vcto_titulo.Date >= maxDate)
                    throw new FinanceiroBusinessException(Messages.msgErroDtaVencTituloInf, null, FinanceiroBusinessException.TipoErro.ERRO_DATA_VENCIMENTO_TITULO_INFERIOR, false);
                t.dt_emissao_titulo = t.dt_emissao_titulo.Date;
                t.dt_vcto_titulo = t.dt_vcto_titulo.Date;
                t.vl_titulo = Decimal.Round(t.vl_titulo, 2);
                t.vl_saldo_titulo = Decimal.Round(t.vl_saldo_titulo, 2);
                int? cdPlano = t.cd_plano_conta_tit;
                t.cd_plano_conta_tit = null;
                if (t.LocalMovto != null)
                {
                    t.LocalMovto = null;
                }
                
                var tituloAdd = DataAccessTitulo.add(t, false);
                newTitulos.Add(tituloAdd);

                if (cdPlano > 0)
                {
                    PlanoTitulo pTitulo = new PlanoTitulo
                    {
                        cd_titulo = tituloAdd.cd_titulo,
                        cd_plano_conta = (int)cdPlano,
                        vl_plano_titulo = tituloAdd.vl_titulo
                    };
                    DataAccessPlanoTitulo.add(pTitulo, false);
                }
            }

            return newTitulos;
        }

        public List<Titulo> editTitulos(List<Titulo> titulos)
        {
            List<Titulo> newTitulos = new List<Titulo>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                foreach (Titulo t in titulos)
                {
                    t.vl_titulo = Decimal.Round(t.vl_titulo, 2);
                    t.vl_saldo_titulo = Decimal.Round(t.vl_saldo_titulo, 2);

                    if (t.cd_titulo > 0 && t.tituloEdit)
                    {
                        Titulo tituloContext = DataAccessTitulo.findById(t.cd_titulo, false);
                        if (tituloContext != null && tituloContext.id_status_cnab != (int)Titulo.StatusCnabTitulo.INICIAL && tituloContext.id_status_cnab != (int)Titulo.StatusCnabTitulo.CONFIRMADO_PEDIDO_BAIXA)
                            throw new FinanceiroBusinessException(Messages.msgNotUpdateTituloEnviadoCNAB, null, FinanceiroBusinessException.TipoErro.ERRO_TITULO_ENVIADO_CNAB, false);
                        if (tituloContext != null && tituloContext.id_cnab_contrato && tituloContext.id_status_cnab == (int)Titulo.StatusCnabTitulo.INICIAL)
                            throw new FinanceiroBusinessException(Messages.msgNotUpdateTituloBoleto, null, FinanceiroBusinessException.TipoErro.ERRO_TITULO_EMITIDO_BOLETO, false);
                        tituloContext.copy(t);
                        tituloContext.LocalMovto = null;
                        newTitulos.Add(DataAccessTitulo.edit(tituloContext, false));

                        //
                        PlanoTitulo planoTitulo = DataAccessPlanoTitulo.getPlanoTituloByCdTitulo(tituloContext.cd_titulo, tituloContext.cd_pessoa_empresa);
                        if (planoTitulo != null && planoTitulo.cd_plano_titulo > 0)
                        {
                            planoTitulo.vl_plano_titulo = t.vl_titulo;
                            //planoTitulo.cd_plano_conta = t.cd_plano_conta_tit.Value;
                            DataAccessPlanoTitulo.saveChanges(false);
                        }
                    }
                    else if (t.cd_titulo == 0)
                    {
                        int? cdPlano = t.cd_plano_conta_tit;
                        t.cd_plano_conta_tit = null;
                        newTitulos.Add(DataAccessTitulo.add(t, false));
                        if (cdPlano != null && cdPlano > 0)
                        {
                            PlanoTitulo pTitulo = new PlanoTitulo
                            {
                                cd_titulo = t.cd_titulo,
                                cd_plano_conta = (int)cdPlano,
                                vl_plano_titulo = t.vl_titulo
                            };
                            DataAccessPlanoTitulo.add(pTitulo, false);
                        }
                    }
                }
                transaction.Complete();
            }
            return newTitulos;
        }

        public List<Titulo> editTitulosContrato(List<Titulo> titulos, Contrato contratoView, double pc_bolsa)
        {
            List<Titulo> newTitulos = new List<Titulo>();
            bool existeBolsaContrato = false;
            bool zerarAditamentoBolsa = (contratoView.aditamentoMaxData != null && 
                contratoView.aditamentoMaxData.id_tipo_aditamento == (int)Aditamento.TipoAditamento.ADITIVO_BOLSA && contratoView.pc_desconto_bolsa == 0);

            if (zerarAditamentoBolsa || contratoView.pc_desconto_bolsa > 0 || pc_bolsa > 0)
                existeBolsaContrato = true;
            this.sincronizarContextos(DataAccessBaixaFinan.DB());
            List<BaixaTitulo> baixasBolsaContext = new List<BaixaTitulo>();
            if (zerarAditamentoBolsa || contratoView.pc_desconto_bolsa > 0 || pc_bolsa > 0)
                baixasBolsaContext = DataAccessBaixaFinan.getBaixaTitulosBolsaContrato(contratoView.cd_contrato, contratoView.cd_pessoa_escola).ToList();

            foreach (Titulo t in titulos)
            {
                if(t.vl_titulo <= 0)
                    throw new FinanceiroBusinessException(Messages.msgErroNaoExisteSaldoTitulo, null, FinanceiroBusinessException.TipoErro.ERRO_NAO_EXISTE_SALDO_TITULO, false);

                t.vl_titulo = Decimal.Round(t.vl_titulo, 2);
                t.vl_saldo_titulo = Decimal.Round(t.vl_saldo_titulo, 2);
                Titulo tituloContext = DataAccessTitulo.findById(t.cd_titulo, false);

                if (tituloContext != null && t.alterou_local_movto)
                {
                    tituloContext.cd_local_movto = t.cd_local_movto;
                    tituloContext.pc_taxa_cartao = t.pc_taxa_cartao;
                    tituloContext.vl_taxa_cartao = t.vl_taxa_cartao;
                    tituloContext.nm_dias_cartao = t.nm_dias_cartao;
                    tituloContext.dt_vcto_titulo = t.dt_vcto_titulo;
                }
                if (t.cd_titulo > 0 && t.tituloEdit)
                {
                    if (tituloContext != null)
                    {
                        if (tituloContext != null && tituloContext.id_status_cnab != (int)Titulo.StatusCnabTitulo.INICIAL && tituloContext.id_status_cnab != (int)Titulo.StatusCnabTitulo.CONFIRMADO_PEDIDO_BAIXA)
                            throw new FinanceiroBusinessException(Messages.msgNotUpdateTituloEnviadoCNAB, null, FinanceiroBusinessException.TipoErro.ERRO_TITULO_ENVIADO_CNAB, false);
                        if (tituloContext != null && tituloContext.id_cnab_contrato && tituloContext.id_status_cnab == (int)Titulo.StatusCnabTitulo.INICIAL)
                            throw new FinanceiroBusinessException(Messages.msgNotUpdateTituloBoleto, null, FinanceiroBusinessException.TipoErro.ERRO_TITULO_EMITIDO_BOLETO, false);
                        bool existeBaixa = false;
                        if (existeBolsaContrato)
                            existeBaixa = verificaTituloOrContratoBaixaEfetuada(contratoView.cd_contrato, contratoView.cd_pessoa_escola, tituloContext.cd_titulo);
                        else
                            if (t.vl_liquidacao_titulo > 0)
                                existeBaixa = true;
                        if (existeBaixa)
                            throw new FinanceiroBusinessException(string.Format(Utils.Messages.Messages.msgErroUpdateTituloBaixa), null,
                                      FinanceiroBusinessException.TipoErro.ERRO_TITULO_COM_BAIXA, false);
            
                        if (!contratoView.id_ajuste_manual && existeBolsaContrato)
                        {
                            BaixaTitulo baixaBContext = baixasBolsaContext.Where(x => x.cd_titulo == t.cd_titulo).FirstOrDefault();
                            if (baixaBContext != null)
                            {
                                tituloContext = voltarEstadoAnteriorTitulo(baixaBContext, baixaBContext.cd_tran_finan, contratoView.cd_pessoa_escola, tituloContext);
                                DataAccessBaixaFinan.deleteContext(baixaBContext, false);
                            }
                        }
            
                        tituloContext = Titulo.changeValuesTituloEditMatricula(tituloContext, t);
                        tituloContext.LocalMovto = null;
                        newTitulos.Add(tituloContext);
                        PlanoTitulo planoTitulo = DataAccessPlanoTitulo.getPlanoTituloByCdTitulo(tituloContext.cd_titulo, tituloContext.cd_pessoa_empresa);
                        if (planoTitulo != null && planoTitulo.cd_plano_titulo > 0)
                            planoTitulo.vl_plano_titulo = t.vl_titulo;
                    }
                }
                else if (t.cd_titulo == 0)
                {
                    int? cdPlano = t.cd_plano_conta_tit;
                    t.cd_plano_conta_tit = null;
                    if (t.LocalMovto != null)
                    {
                        t.LocalMovto = null;
                    }

                    newTitulos.Add(DataAccessTitulo.add(t, false));
                    if (cdPlano != null && cdPlano > 0)
                    {
                        PlanoTitulo pTitulo = new PlanoTitulo
                        {
                            cd_titulo = t.cd_titulo,
                            cd_plano_conta = (int)cdPlano,
                            vl_plano_titulo = t.vl_titulo
                        };
                        DataAccessPlanoTitulo.addContext(pTitulo, false);
                    }
                }
                else if (t.localMovEdit)
                {
                    if (t.cd_local_movto != tituloContext.cd_local_movto)
                        tituloContext.cd_local_movto = t.cd_local_movto;
                }
            }
            DataAccessTitulo.saveChanges(false);
            DataAccessPlanoTitulo.saveChanges(false);
            DataAccessBaixaFinan.saveChanges(false);            
            return newTitulos;
        }

        public void adicionaTituloAditamento(List<Titulo> titulos, Contrato contratoView)
        {
            if (titulos.Count > 0)
            {
                if (contratoView.aditamentoMaxData != null)
                {
                    var titulosAditamentoDelete = DataAccessTituloAditamento.ObterTitulosAditamentoPorId(contratoView.aditamentoMaxData.cd_aditamento);
                    if (titulosAditamentoDelete.Count() > 0)
                        DataAccessTituloAditamento.deleteRange(titulosAditamentoDelete.ToList(), false);
                }
            }

            
            foreach (Titulo t in titulos)
            {
                var existeTituloNaTabelaTituloAditamento = DataAccessTituloAditamento.ExisteTitulo(t.cd_titulo);
                if (!existeTituloNaTabelaTituloAditamento && t.dc_tipo_titulo == "AD")
                {
                    var tituloAditamento = new TituloAditamento
                    {
                        cd_aditamento = contratoView.aditamentoMaxData.cd_aditamento,
                        cd_titulo = t.cd_titulo
                    };

                    DataAccessTituloAditamento.addContext(tituloAditamento, false);
                }
            }

            DataAccessTituloAditamento.saveChanges(false);
        }

        public void deleteAllTitulo(List<Titulo> titulos, int cd_escola)
        {
            this.sincronizarContextos(DataAccessTitulo.DB());
            if (titulos != null && titulos.Count() > 0)
            {
                int[] cdTitulos = new int[titulos.Count()];
                int i = 0;
                List<PlanoTitulo> planosTitulos = new List<PlanoTitulo>();
                foreach (var c in titulos)
                {
                    cdTitulos[i] = c.cd_titulo;
                    i++;
                }
                planosTitulos = DataAccessPlanoTitulo.getPlanoTituloByTitulos(cdTitulos, cd_escola).ToList();

                foreach (PlanoTitulo planoTitulo in planosTitulos)
                    DataAccessPlanoTitulo.deleteContext(planoTitulo, false);

                if (titulos != null)
                    foreach (Titulo t in titulos)
                    {
                        if (t.id_status_cnab != (int)Titulo.StatusCnabTitulo.INICIAL && t.id_status_cnab != (int)Titulo.StatusCnabTitulo.CONFIRMADO_PEDIDO_BAIXA)
                            throw new FinanceiroBusinessException(Messages.msgNotUpdateTituloEnviadoCNAB, null, FinanceiroBusinessException.TipoErro.ERRO_TITULO_ENVIADO_CNAB, false);
                        if (t.id_cnab_contrato && t.id_status_cnab == (int)Titulo.StatusCnabTitulo.INICIAL)
                            throw new FinanceiroBusinessException(Messages.msgNotUpdateTituloBoleto, null, FinanceiroBusinessException.TipoErro.ERRO_TITULO_EMITIDO_BOLETO, false);
                        if (t.vl_liquidacao_titulo > 0)
                            throw new FinanceiroBusinessException(string.Format(Utils.Messages.Messages.msgNotExcTituloComBaixa), null,
                                                                   FinanceiroBusinessException.TipoErro.ERRO_TITULO_COM_BAIXA, false);
                        DataAccessTitulo.deleteContext(t, false);
                    }
                DataAccessPlanoTitulo.saveChanges(false);
                DataAccessTitulo.saveChanges(false);
            }
        }

        public bool deleteAllTitulo(int cd_contrato, int cd_escola, bool deletarBaixasBolsa)
        {
            bool retorno = true;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED, DataAccessTitulo.DB()))
            {
                this.sincronizarContextos(DataAccessTitulo.DB());
                if (deletarBaixasBolsa)
                    deletarBaixasBolsaTituloContrato(cd_contrato, cd_escola);
                List<Titulo> titulosContext = DataAccessTitulo.getTitulosByContratoTodosDados(cd_contrato, cd_escola).ToList();
                if (titulosContext != null && titulosContext.Count() > 0)
                {
                    foreach (var delTitulo in titulosContext)
                    {
                        if (delTitulo.id_status_cnab != (int)Titulo.StatusCnabTitulo.INICIAL && delTitulo.id_status_cnab != (int)Titulo.StatusCnabTitulo.CONFIRMADO_PEDIDO_BAIXA)
                            throw new FinanceiroBusinessException(Messages.msgNotUpdateTituloEnviadoCNAB, null, FinanceiroBusinessException.TipoErro.ERRO_TITULO_ENVIADO_CNAB, false);
                        if (delTitulo.id_cnab_contrato && delTitulo.id_status_cnab == (int)Titulo.StatusCnabTitulo.INICIAL)
                            throw new FinanceiroBusinessException(Messages.msgNotUpdateTituloBoleto, null, FinanceiroBusinessException.TipoErro.ERRO_TITULO_EMITIDO_BOLETO, false);
                        bool existeBaixa = false;
                        //VERIFICA SE OS TÍTULOS POSSUEM BAIXA
                        if (deletarBaixasBolsa)
                            existeBaixa = verificaTituloOrContratoBaixaEfetuada(cd_contrato, cd_escola, 0);
                        else
                            existeBaixa = getTituloBaixadoContrato(cd_contrato, cd_escola, 0);
                        if (existeBaixa)
                            throw new FinanceiroBusinessException(string.Format(Utils.Messages.Messages.msgNotExcTituloComBaixa), null, FinanceiroBusinessException.TipoErro.ERRO_TITULO_COM_BAIXA, false);
                        retorno = !DataAccessTitulo.delete(delTitulo, false);
                    }
                }
                transaction.Complete();
            }
            return retorno;
        }

        public void deletarTitulosEdicaoMatricula(Contrato contratoView, double pcBolsa)
        {
            List<Titulo> listaTituloView = contratoView.titulos.ToList();
            bool existeBolsaContrato = false;
            if (contratoView.pc_desconto_bolsa > 0 || pcBolsa > 0)
                existeBolsaContrato = true;
            //Verifica os títulos que tinha anteriormente, para ver se algum foi deletado
            int cd_contrato = contratoView.cd_contrato;
            if (contratoView.pc_desconto_bolsa == 100 || pcBolsa == 100)
                cd_contrato = -1 * contratoView.cd_contrato;
            List<Titulo> titulosContext = DataAccessTitulo.getTitulosByContratoTodosDados(cd_contrato, contratoView.cd_pessoa_escola).ToList();
            List<Titulo> titulosDeleted = titulosContext.Where(tc => !listaTituloView.Any(tv => tc.cd_titulo == tv.cd_titulo)).ToList();
            List<TituloCodUI> titulosCod = titulosDeleted.Select(o => new TituloCodUI
            {
                cd_titulo = o.cd_titulo,
                nm_titulo = o.nm_titulo
            }).ToList();

            var Json = JsonConvert.SerializeObject(titulosCod);
            var mensagem = DataAccessTitulo.delTitulosContrato(cd_contrato, existeBolsaContrato, Json);
            if (mensagem == "msgNotUpdateTituloEnviadoCNAB1")
                 throw new FinanceiroBusinessException(Messages.msgNotUpdateTituloEnviadoCNAB, null, FinanceiroBusinessException.TipoErro.ERRO_TITULO_ENVIADO_CNAB, false);
            if (mensagem == "msgNotUpdateTituloEnviadoCNAB2")
                 throw new FinanceiroBusinessException(Messages.msgNotUpdateTituloEnviadoCNAB, null, FinanceiroBusinessException.TipoErro.ERRO_TITULO_ENVIADO_CNAB, false);
            if (mensagem == "msgNotExcTituloComBaixa")
                throw new FinanceiroBusinessException(string.Format(Utils.Messages.Messages.msgNotExcTituloComBaixa), null, FinanceiroBusinessException.TipoErro.ERRO_TITULO_COM_BAIXA, false);
        }

        public void deletarBaixasBolsaTituloContrato(int cd_contrato, int cd_escola)
        {
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED, DataAccessBaixaFinan.DB()))
            {
                this.sincronizarContextos(DataAccessBaixaFinan.DB());
                List<BaixaTitulo> baixaContext = DataAccessBaixaFinan.getBaixaTitulosBolsaContrato(cd_contrato, cd_escola).ToList();
                if (baixaContext != null && baixaContext.Count() > 0)
                    foreach (var b in baixaContext)
                        DataAccessBaixaFinan.delete(b, false);
                transaction.Complete();
            }
        }

        public void gerarBaixaParcialBolsaTitulos(List<Titulo> titulos, double pc_bolsa, DateTime dt_emissao_titulo, bool nova_matricula, bool aditivoBolsa)
        {
            List<Titulo> titulos_ME_AD = titulos.Where(tit => tit.dc_tipo_titulo == "ME" || tit.dc_tipo_titulo == "AD").ToList();
            if (titulos_ME_AD != null && titulos_ME_AD.Count() > 0)
                using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED, DataAccessBaixaFinan.DB(), TransactionScopeBuilder.TransactionTime.DEFAULT))
                {
                    this.sincronizarContextos(DataAccessTitulo.DB());
                    TransacaoFinanceira transacao = new TransacaoFinanceira();
                    transacao.dt_tran_finan = dt_emissao_titulo.Date;
                    foreach (Titulo t in titulos_ME_AD)
                    {
                        if ((t.vl_titulo - t.vl_material_titulo) > 0 && pc_bolsa > 0 && nova_matricula || t.tituloEdit)
                        {
                            //t.nomeResponsavel = no_responsavel;
                            decimal vl_baixa = decimal.Round((t.vl_titulo - t.vl_material_titulo) * (decimal)pc_bolsa / 100, 2);
                            BaixaTitulo baixa = new BaixaTitulo();
                            transacao.cd_pessoa_empresa = t.cd_pessoa_empresa;
                            transacao.cd_tipo_liquidacao = (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA;

                            baixa.cd_titulo = t.cd_titulo;
                            baixa.dt_baixa_titulo = transacao.dt_tran_finan.Value;
                            baixa.id_baixa_parcial = true;
                            baixa.vl_liquidacao_baixa = vl_baixa;
                            baixa.vl_principal_baixa = t.vl_titulo;
                            baixa.vl_desconto_baixa = t.vl_titulo - vl_baixa;
                            //baixa.tx_obs_baixa = "Baixa gerada automaticamente para o prospect " + nome + ".";
                            baixa.Titulo = t;
                            transacao.Baixas.Add(baixa);
                        }
                    }
                    this.postIncluirTransacao(transacao, true);
                    transaction.Complete();
                }
        }

        public List<Titulo> getTitulosContrato(int cd_contrato)
        {
            return DataAccessTitulo.getTitulosContrato(cd_contrato);
        }

        public List<Titulo> getTitulosByContrato(int cdContrato, int cdEscola)
        {
            return DataAccessTitulo.getTitulosByContrato(cdContrato, cdEscola);
        }

        public int getQtdContratoNaoMultiploDiferenteCartaoCheque(int cdContrato, int cdEscola)
        {
            return DataAccessTitulo.getQtdContratoNaoMultiploDiferenteCartaoCheque(cdContrato, cdEscola);
        }

        public int getQtdMovimentoDiferenteCartaoCheque(int cdMovimento, int cdEscola)
        {
            return DataAccessTitulo.getQtdMovimentoDiferenteCartaoCheque(cdMovimento, cdEscola);
        }

        public int getQtdTitulosSemBaixaTipoCartaoOuCheque(int cdContrato, int cdEscola)
        {
            return DataAccessTitulo.getQtdTitulosSemBaixaTipoCartaoOuCheque(cdContrato, cdEscola);
        }

        public int getQtdTitulosMovimentoSemBaixaTipoCartaoOuCheque(int cdMovimento, int cdEscola)
        {
            return DataAccessTitulo.getQtdTitulosMovimentoSemBaixaTipoCartaoOuCheque(cdMovimento, cdEscola);
        }

        public List<Titulo> getTitulosByContratoLeitura(int cdContrato, int cdEscola)
        {
            List<Titulo> retorno = new List<Titulo>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = DataAccessTitulo.getTitulosByContrato(cdContrato, cdEscola);
                transaction.Complete();
            }
            return retorno;
        }

        public Titulo getTituloAbertoByAditamento(int cd_contrato)
        {
            Titulo titulo = new Titulo();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                titulo = DataAccessTitulo.getTituloAbertoByAditamento(cd_contrato);
                transaction.Complete();
            }
            return titulo;
        }

        public Titulo getTituloByContrato(int cd_contrato, int nro_parcela)
        {
            Titulo titulo = new Titulo();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                titulo = DataAccessTitulo.getTituloByContrato(cd_contrato, nro_parcela);
                transaction.Complete();
            }
            return titulo;
        }

        public List<Titulo> getTitulosByRenegociacao(int cdContrato, int cdAluno, int cdEscola, int cdProduto)
        {
            return DataAccessTitulo.getTitulosByRenegociacao(cdContrato, cdAluno, cdEscola, cdProduto);
        }

        public decimal getValorParcela(bool primeira_parcela, int cd_contrato, int cd_escola)
        {
            decimal vlParcela = new decimal();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                vlParcela = DataAccessTitulo.getValorParcela(cd_contrato, cd_escola, primeira_parcela);
                transaction.Complete();
            }
            return vlParcela;
        }

        public bool deletarTitulo(Titulo titulo, int cdEscola)
        {
            bool deleted = false;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED, DataAccessPlanoTitulo.DB()))
            {
                this.sincronizarContextos(DataAccessPlanoTitulo.DB());
                if (titulo != null && titulo.cd_titulo > 0)
                {
                    PlanoTitulo delPlanoTitulo = DataAccessPlanoTitulo.getPlanoTituloByCdTitulo(titulo.cd_titulo, cdEscola);
                    if (delPlanoTitulo != null && delPlanoTitulo.cd_plano_titulo > 0)
                        DataAccessPlanoTitulo.delete(delPlanoTitulo, false);

                    deleted = DataAccessTitulo.delete(titulo, false);
                }
                transaction.Complete();
                return deleted;
            }
        }

        public IEnumerable<Titulo> getTitulosByContratoTodosDados(int cdContrato, int cdEscola)
        {
            return DataAccessTitulo.getTitulosByContratoTodosDados(cdContrato, cdEscola);
        }

        public bool getTituloBaixadoContrato(int cd_contrato, int cdEscola, int cdTitulo)
        {
            return DataAccessTitulo.getTituloBaixadoContrato(cd_contrato, cdEscola, cdTitulo);
        }

        public bool verificaTituloOrContratoBaixaEfetuada(int cd_contrato, int cdEscola, int cdTitulo)
        {
            return DataAccessTitulo.verificaTituloOrContratoBaixaEfetuada(cd_contrato, cdEscola, cdTitulo);
        }

        public bool verificaTituloEnviadoCnabOuContrato(int cd_contrato, int cdEscola, List<int> cdsTitulo)
        {
            return DataAccessTitulo.verificaTituloEnviadoCnabOuContrato(cd_contrato, cdEscola, cdsTitulo);
        }

        public bool verificaTituloEnviadoBoletoOuContrato(int cd_contrato, int cdEscola, List<int> cdsTitulo)
        {
            return DataAccessTitulo.verificaTituloEnviadoBoletoOuContrato(cd_contrato, cdEscola, cdsTitulo);
        }

        public TipoDesconto getTipoDescontoByContrato(int cd_desconto_contrato)
        {
            return DataAccessTipoDesconto.getTipoDescontoByContrato(cd_desconto_contrato);
        }

        public IEnumerable<Titulo> searchTitulo(SearchParameters parametros, int cd_pessoa_empresa, int cd_pessoa, bool responsavel, bool inicio, int locMov, int natureza, int status, int? numeroTitulo,
                                               int? parcelaTitulo, string valorTitulo, DateTime? dtInicial, DateTime? dtFinal, bool emissao, bool vencimento, bool baixa, int locMovBaixa,
                                               int cdTipoLiquidacao, bool contaSegura, byte tipoTitulo, string nossoNumero, int cnabStatus, int? nro_recibo, int? cd_turma, List<int> cd_situacoes_aluno,
                                               int? cd_tipo_financeiro)
        {
            IEnumerable<Titulo> retorno = new List<Titulo>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED, DataAccessTitulo.DB(), TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                if (parametros.sort == null)
                    parametros.sort = "nm_titulo";
                parametros.sort = parametros.sort.Replace("dt_emissao", "dt_emissao_titulo");
                parametros.sort = parametros.sort.Replace("dt_vcto", "dt_vcto_titulo");
                parametros.sort = parametros.sort.Replace("vlTitulo", "vl_titulo");
                parametros.sort = parametros.sort.Replace("vlSaldoTitulo", "vl_saldo_titulo");
                parametros.sort = parametros.sort.Replace("natureza", "id_status_titulo");
                parametros.sort = parametros.sort.Replace("nomeResponsavel", "no_pessoa");
                parametros.sort = parametros.sort.Replace("tipoDoc", "cd_tipo_financeiro");
                parametros.sort = parametros.sort.Replace("statusTitulo", "id_status_titulo");


                List<Titulo> listaTitulos = DataAccessTitulo.searchTitulo(parametros, cd_pessoa_empresa, cd_pessoa, responsavel, inicio, locMov, natureza, status, numeroTitulo, parcelaTitulo, valorTitulo, dtInicial,
                    dtFinal, emissao, vencimento, baixa, locMovBaixa, cdTipoLiquidacao, contaSegura, tipoTitulo, nossoNumero, cnabStatus, nro_recibo, cd_turma, cd_situacoes_aluno, cd_tipo_financeiro).ToList();

                Parametro parametro = DataAccessPoliticaTurma.getParametrosBaixa(cd_pessoa_empresa);
                DateTime data_baixa = DateTime.UtcNow.Date;


               
                for (int i = 0; i < listaTitulos.Count; i++)
                {
                    if (listaTitulos[i].id_status_titulo == (int) Titulo.StatusTitulo.ABERTO)
                    {
                        BaixaTitulo baixaTitulo = new BaixaTitulo();

                        baixaTitulo.dt_baixa_titulo = data_baixa;
                        listaTitulos[i].cd_pessoa_empresa = cd_pessoa_empresa;
                        this.simularBaixaTitulo(listaTitulos[i], ref baixaTitulo, parametro, cd_pessoa_empresa, true, false);
                        listaTitulos[i].vl_saldo_corrigido = baixaTitulo.vl_liquidacao_baixa;
                    }
                   
                }

                retorno = listaTitulos.AsEnumerable();

                transaction.Complete();
            }
            return retorno;
        }

        public void simularBaixaTitulo(Titulo titulo, ref BaixaTitulo baixa, Parametro parametro, int cd_escola,
            bool contaSegura, bool gerarMensagem)
        {
            //Busca o contrato do titulo:
            SGFWebContext db = new SGFWebContext();
            Contrato contrato = null;
            if (!contaSegura)
                this.verificarTituloContaSegura(titulo.cd_titulo, cd_escola);

            if (titulo.vl_saldo_titulo > 0)
            {
                if (titulo.cd_origem_titulo.HasValue && titulo.id_origem_titulo ==
                    Int32.Parse(db.LISTA_ORIGEM_LOGS["Contrato"].ToString()))
                    contrato = DataAccessPoliticaTurma.getContratoBaixa(cd_escola, titulo.cd_origem_titulo.Value);
                //baixa.dt_baixa_titulo = DateTime.UtcNow;
                List<BaixaTitulo> baixasParcial = this.getBaixasTransacaoFinan(0, 0, titulo.cd_titulo,
                    cd_escola, BaixaTituloDataAccess.TipoConsultaBaixaEnum.HAS_BAIXAS_PARCIAIS_TITULO).ToList();
                baixasParcial = baixasParcial.Where(x =>
                    x.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA &&
                    x.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA_ADITIVO).ToList();
                DateTime dtaBaixa = baixa.dt_baixa_titulo;
                List<BaixaTitulo> baixasParcialDia = baixasParcial.Where(b => b.dt_baixa_titulo == dtaBaixa).ToList();
                if (baixasParcial.Count() > 0)
                {
                    if (baixasParcialDia.Count() > 0)
                    {
                        this.calcularBaixaTitulo(contrato, titulo, ref baixa, parametro, gerarMensagem, false);
                        decimal vl_desconto = baixa.vl_desconto_baixa_calculado;
                        baixa.vl_multa_baixa = baixasParcialDia.Sum(p => p.vl_multa_calculada);
                        baixa.vl_juros_baixa = baixasParcialDia.Sum(p => p.vl_juros_calculado);
                        baixa.vl_liquidacao_baixa = (baixa.vl_principal_baixa + baixa.vl_multa_baixa +
                                                     baixa.vl_juros_baixa - vl_desconto);
                        baixa.vl_juros_calculado = 0;
                        baixa.vl_multa_calculada = 0;
                    }
                    else
                    {
                        var dtVctoOri = titulo.dt_vcto_titulo;
                        bool baixaParcAposVenc = false;
                        DateTime dtaPrimeiraBaixaParcial = baixasParcial.OrderBy(d => d.dt_baixa_titulo)
                            .FirstOrDefault().dt_baixa_titulo;
                        if (dtaPrimeiraBaixaParcial > dtVctoOri)
                        {
                            titulo.dt_vcto_titulo = dtaPrimeiraBaixaParcial;
                            baixaParcAposVenc = true;
                        }

                        this.calcularBaixaTitulo(contrato, titulo, ref baixa, parametro, gerarMensagem, false);
                        decimal multa = baixa.vl_multa_calculada;
                        ////Aplicar o desconto quando não existir juros e multa.
                        decimal vl_desconto = baixa.vl_desconto_baixa_calculado;
                        if (baixaParcAposVenc && baixasParcial.Sum(p => p.vl_multa_calculada) > 0)
                            multa = baixasParcial.Sum(p => p.vl_multa_calculada);
                        if (titulo.dt_vcto_titulo > baixa.dt_baixa_titulo)
                        {
                            baixa.vl_multa_baixa = 0;
                            baixa.vl_juros_baixa = 0;
                        }
                        else
                        {
                            baixa.vl_multa_baixa = multa;
                            baixa.vl_juros_baixa =
                                baixa.vl_juros_calculado + baixasParcial.Sum(p => p.vl_juros_calculado);
                        }

                        //  baixa.vl_juros_calculado = 0;
                        //  baixa.vl_multa_calculada = 0;
                        baixa.vl_liquidacao_baixa = (baixa.vl_principal_baixa + baixa.vl_multa_baixa +
                                                     baixa.vl_juros_baixa - vl_desconto);
                        titulo.dt_vcto_titulo = dtVctoOri;
                    }

                }
                else
                {
                    this.calcularBaixaTitulo(contrato, titulo, ref baixa, parametro, gerarMensagem, false);
                    baixa.vl_multa_baixa = baixa.vl_multa_calculada;
                    baixa.vl_juros_baixa = baixa.vl_juros_calculado;
                }
            }
            else
            {
                List<BaixaTitulo> baixas = this.getBaixasTransacaoFinan(0, 0, titulo.cd_titulo, cd_escola,
                    BaixaTituloDataAccess.TipoConsultaBaixaEnum.HAS_BAIXAS_PARCIAIS_TITULO).ToList();
                if (baixas != null && baixas.Count > 0)
                    foreach (BaixaTitulo b in baixas)
                    {
                        baixa.vl_multa_baixa += b.vl_multa_calculada - b.vl_multa_baixa;
                        baixa.vl_juros_baixa += b.vl_juros_calculado - b.vl_juros_baixa;
                        baixa.vl_liquidacao_baixa += (b.vl_multa_calculada - b.vl_multa_baixa) +
                                                     (b.vl_juros_calculado - b.vl_juros_baixa);
                        baixa.cd_titulo = b.cd_titulo;
                        titulo.cd_titulo = b.cd_titulo;
                    }

                baixa.cd_titulo = titulo.cd_titulo;
                //Fazer a pesquisa da baixa e somar o juros com a multa no lugar do vl_liquidacao_baixa.
                //Mudar o metodo que fecha o titulo para somar o juros e a multa calculado e comparar com a mesma liquidada.
                //baixa.vl_multa_baixa = baixa.vl_multa_calculada;
                //baixa.vl_juros_baixa = baixa.vl_juros_calculado;
            }

            //Copia os atributos do titulo para a baixa, para mostrar na grade:
            baixa.nm_titulo = titulo.nm_titulo;
            baixa.nm_parcela_titulo = titulo.nm_parcela_titulo;
            baixa.dt_vcto_titulo = titulo.dt_vcto;
            baixa.id_natureza_titulo = titulo.id_natureza_titulo;
            baixa.Titulo = titulo;
        }

        /* Método que calcula a baixa do título. Os objetos de entrada:
        * contrato.AlunoTurma > com o relacionamento da turma ativa do contrato.
        * contrato.AlunoTurma[0].cd_turma
        * contrato.cd_aluno
        * baixa.dt_baixa_titulo
        */
        private void calcularBaixaTitulo(Contrato contrato, Titulo titulo, ref BaixaTitulo baixa, Parametro parametro,
            bool gerarMensagem, bool usar_valor_titulo)
        {
            SGFWebContext db = new SGFWebContext();
            IEnumerable<Feriado> feriadosEscola = null;
            baixa.diasPoliticaAntecipacao = new List<BaixaTitulo.DiasPoliticaAntecipacao>();
            int cd_turma = 0;

            try
            {
                //1-Verificar se o titulo é de contrato de matrícula
                if (contrato != null &&
                    titulo.id_origem_titulo == Int32.Parse(db.LISTA_ORIGEM_LOGS["Contrato"].ToString()))
                {
                    if (contrato.AlunoTurma != null)
                    {
                        List<AlunoTurma> listaAlunoTurma = contrato.AlunoTurma.ToList();
                        //Se o contrato tiver turma:
                        if (listaAlunoTurma.Count > 0)
                            cd_turma = listaAlunoTurma[0].cd_turma;
                    }
                }
            }
            catch (System.NullReferenceException exe)
            {
                throw new NullReferenceException("1-Verificar se o titulo é de contrato de matrícula", exe);
            }

            //4-Verificar Descontos do Contrato que incidem na baixa e estão ativos:
            decimal soma_valores_desconto = 0;
            decimal percentual_desconto = 0;
            double percentual_juros = 0;
            double percentual_multa = 0;
            decimal percentual_valor = 0;
            decimal vl_liquido = titulo.vl_saldo_titulo;
            if ((titulo.vl_saldo_titulo - titulo.vl_material_titulo) < 0)
                vl_liquido = 0;
            else
                vl_liquido -= titulo.vl_material_titulo;

            try
            {
                baixa.pc_pontualidade = 0;
                if (contrato != null && contrato.DescontoContrato != null && titulo != null &&
                    titulo.id_natureza_titulo == (int)Titulo.NaturezaTitulo.RECEBER
                    && !Titulo.TipoTitulo.TM.ToString().Equals(titulo.dc_tipo_titulo) &&
                    !Titulo.TipoTitulo.TA.ToString().Equals(titulo.dc_tipo_titulo))
                {
                    List<DescontoContrato> descontosContrato = contrato.DescontoContrato.ToList();
                    Aditamento aditamento = new Aditamento();
                    if (contrato.Aditamento != null && contrato.Aditamento.Count() > 0) aditamento = contrato.Aditamento.FirstOrDefault();
                    for (int i = 0; i < descontosContrato.Count; i++)
                    {
                        DescontoContrato descontoContrato = descontosContrato[i];
                        descontoContrato = (DescontoContrato)descontoContrato.Clone();
                        if (descontoContrato.id_desconto_ativo && descontoContrato.id_incide_baixa
                        ) // && (titulo.nm_parcela_titulo == 1))
                        {
                            if ((titulo.dt_vcto_titulo >= ((aditamento == null || aditamento.dt_vencto_inicial == null || !aditamento.id_tipo_aditamento.HasValue || (aditamento.id_tipo_aditamento.Value != (byte)Aditamento.TipoAditamento.CONCESSAO_DESCONTO) && aditamento.id_tipo_aditamento.Value != (byte)Aditamento.TipoAditamento.PERDA_DESCONTO) ? titulo.dt_vcto_titulo : aditamento.dt_vencto_inicial.Value.Date)) &&
                               ((descontoContrato.nm_parcela_ini == 0 && descontoContrato.nm_parcela_fim == 0) ||
                                (descontoContrato.nm_parcela_ini > 0 && descontoContrato.nm_parcela_fim == 0 &&
                                 titulo.nm_parcela_titulo >= descontoContrato.nm_parcela_ini) ||
                                (descontoContrato.nm_parcela_ini == 0 && descontoContrato.nm_parcela_fim > 0 &&
                                 titulo.nm_parcela_titulo <= descontoContrato.nm_parcela_fim) ||
                                (titulo.nm_parcela_titulo >= descontoContrato.nm_parcela_ini &&
                                 titulo.nm_parcela_titulo <= descontoContrato.nm_parcela_fim)))
                            {
                                soma_valores_desconto += descontoContrato.vl_desconto_contrato;
                                baixa.des_desconto = baixa.des_desconto + descontoContrato.valor_desconto;
                                percentual_valor = ((descontoContrato.vl_desconto_contrato == 0 || vl_liquido == 0) ? 0 :
                                        (descontoContrato.vl_desconto_contrato / vl_liquido * 100));
                                percentual_valor = percentual_valor == 0 ? descontoContrato.pc_desconto_contrato : percentual_valor;

                                if (parametro != null && parametro.id_somar_descontos_financeiros)
                                    percentual_desconto += percentual_valor;
                                else
                                    percentual_desconto =
                                        100 - ((1 - percentual_valor / 100) *
                                               (1 - percentual_desconto / 100)) * 100;
                            }
                        }
                    }
                }
            }
            catch (System.NullReferenceException exe)
            {
                throw new NullReferenceException(
                    "4-Verificar Descontos do Contrato que incidem na baixa e estão ativos", exe);
            }

            DateTime dataVencOriginal = titulo.dt_vcto_titulo;
            DateTime dataVenc = titulo.dt_vcto_titulo;

            try
            {
                //5-Calcular data de Vencimento:

                if (parametro != null && parametro.id_alterar_venc_final_semana)
                    pulaFeriadoEFinalSemana(ref dataVenc, titulo.cd_pessoa_empresa, ref feriadosEscola);
                titulo.dt_vcto_titulo = dataVenc;
            }
            catch (System.NullReferenceException exe)
            {
                throw new NullReferenceException("5-Calcular data de Vencimento", exe);
            }

            int nro_dias = (baixa.dt_baixa_titulo - titulo.dt_vcto_titulo).Days;

            if (parametro != null && !parametro.id_alterar_venc_final_semana)
                if (!parametro.id_juros_final_semana)
                {
                    pulaFeriadoEFinalSemana(ref dataVenc, titulo.cd_pessoa_empresa, ref feriadosEscola);
                    nro_dias = (baixa.dt_baixa_titulo - dataVenc).Days;
                }
                else
                {
                    pulaFeriadoEFinalSemana(ref dataVenc, titulo.cd_pessoa_empresa, ref feriadosEscola);
                    if ((baixa.dt_baixa_titulo - dataVenc).Days > 0)
                        nro_dias = (baixa.dt_baixa_titulo - dataVencOriginal).Days;
                    else
                        nro_dias = 0;
                }

            //6-Data da Baixa maior que a data de vencimento
            if (baixa.dt_baixa_titulo.CompareTo(titulo.dt_vcto_titulo) > 0)
            {
                try
                {
                    baixa.pc_pontualidade = 0;
                    percentual_desconto = 0;
                    baixa.des_desconto = String.Empty;
                    soma_valores_desconto = 0;
                    double nm_dias_carencia =
                        parametro.nm_dias_carencia.HasValue ? (double)parametro.nm_dias_carencia : 0;
                    if (!(baixa.dt_baixa_titulo.CompareTo(titulo.dt_vcto_titulo.AddDays(nm_dias_carencia)) <= 0)
                        && parametro.id_cobrar_juros_multa.HasValue && parametro.id_cobrar_juros_multa.Value
                        && titulo.id_natureza_titulo == (int)Titulo.NaturezaTitulo.RECEBER && nro_dias > 0)
                    {
                        if (parametro.pc_juros_dia.HasValue)
                            percentual_juros = parametro.pc_juros_dia.Value;
                        percentual_juros = nro_dias * percentual_juros;
                        if (parametro.pc_multa.HasValue)
                        {
                            //percentual_juros += parametro.pc_multa.Value;
                            percentual_multa = parametro.pc_multa.Value;
                        }
                    }
                }
                catch (System.NullReferenceException exe)
                {
                    throw new NullReferenceException("6-Data da Baixa maior que a data de vencimento", exe);
                }
            }

            //7-Data da Baixa menor ou igual ao Vencimento do item 4:
            else if (contrato != null && contrato.cd_aluno > 0 &&
                     titulo.id_natureza_titulo == (int)Titulo.NaturezaTitulo.RECEBER
                     && !Titulo.TipoTitulo.TM.ToString().Equals(titulo.dc_tipo_titulo) &&
                     !Titulo.TipoTitulo.TA.ToString().Equals(titulo.dc_tipo_titulo))
            {
                try
                {
                    //2-Verificar existência de Desconto por Antecipação:
                    PoliticaDesconto politica = null;
                    politica = this.getPoliticaDescontoByTurmaAluno(cd_turma, contrato.cd_aluno,
                        titulo.dt_vcto_titulo);
                    if (politica != null && cd_turma > 0) //Desconto do aluno e turma
                        politica = this.getPoliticaDescontoByTurmaAluno(cd_turma, contrato.cd_aluno,
                            titulo.dt_vcto_titulo);
                    if (politica == null) //Desconto do aluno
                        politica = this.getPoliticaDescontoByAluno(contrato.cd_aluno,
                            titulo.dt_vcto_titulo);
                    if (politica == null) //Desconto da turma
                        politica = this.getPoliticaDescontoByTurma(cd_turma, titulo.dt_vcto_titulo);
                    if (politica == null) //Desconto da escola
                        politica = this.getPoliticaDescontoByEscola(contrato.cd_pessoa_escola,
                            titulo.dt_vcto_titulo);

                    //Se não achar a política, sempre vai considerar o desconto do contrato.
                    decimal percentual_anterior = percentual_desconto;
                    if (politica != null && politica.DiasPolitica != null)
                    {
                        List<DiasPolitica> dias = politica.DiasPolitica.ToList();
                        bool encontrou_politica = false;
                        double percentual_politica = 0;
                        for (int i = 0; i < dias.Count && (!encontrou_politica || gerarMensagem); i++)
                        {
                            DateTime data_desconto = new DateTime();

                            //Caso não exista o dia, por exemplo, dia 31, tenta ainda o dia 30, 29 e 28:
                            bool encontrou_dia = false;
                            for (int k = 0; k < 3 && !encontrou_dia; k++)
                            {
                                try
                                {
                                    data_desconto = new DateTime(titulo.dt_vcto_titulo.Year,
                                        titulo.dt_vcto_titulo.Month, dias[i].nm_dia_limite_politica - k);
                                    encontrou_dia = true;
                                }
                                catch (System.ArgumentOutOfRangeException)
                                {
                                    encontrou_dia = false;
                                }
                            }

                            if (parametro != null && parametro.id_alterar_venc_final_semana)
                                pulaFeriadoEFinalSemana(ref data_desconto, titulo.cd_pessoa_empresa,
                                    ref feriadosEscola);

                            //Se achar a política com percentual diferente de zero e a data da baixa for menor ou igual a data da política, sempre vai considerar o desconto do contrato e o desconto da política.
                            //Se achar a política com percentual diferente de zero e a data da baixa for maior que a data da política e tiver marcado vai considerar o desconto do contrato.
                            if (dias[i].pc_desconto.HasValue && dias[i].pc_desconto > 0)
                            {
                                percentual_politica = System.Convert.ToDouble(dias[i].pc_desconto.Value);
                                if (baixa.dt_baixa_titulo.CompareTo(data_desconto) <= 0)
                                {
                                    baixa.pc_pontualidade = System.Convert.ToDouble(dias[i].pc_desconto.Value);
                                    baixa.cd_politica_desconto = politica.cd_politica_desconto;

                                    //Aplica o percentual de pontualidade com o percentual de desconto:
                                    if (parametro.id_somar_descontos_financeiros)
                                        percentual_desconto += (decimal)baixa.pc_pontualidade;
                                    else
                                        percentual_desconto =
                                            100 - (((1 - percentual_desconto / 100) *
                                                    (1 - ((decimal)baixa.pc_pontualidade) / 100))) * 100;

                                    baixa.des_desconto = baixa.des_desconto + baixa.valor_desconto;

                                    encontrou_politica = true;

                                    if (gerarMensagem)
                                    {
                                        if (!baixa.sl_politicas.Contains(data_desconto))
                                        {
                                            baixa.sl_politicas[data_desconto] = percentual_desconto;
                                            baixa.diasPoliticaAntecipacao.Add(new BaixaTitulo.DiasPoliticaAntecipacao
                                            {
                                                cd_politica_desconto = politica.cd_politica_desconto,
                                                Data_politica = politica.dt_inicial_politica,
                                                nm_dia_limite_politica = dias[i].nm_dia_limite_politica,
                                                pc_pontualidade = (decimal)dias[i].pc_desconto.Value,
                                                pc_pontualidade_total = percentual_desconto,
                                                cd_titulo = titulo.cd_titulo,
                                                pc_desconto_baixa = percentual_anterior
                                            });
                                        }

                                        percentual_desconto = percentual_anterior;
                                    }
                                }
                            }
                        }

                        //Se encontrar uma política com percentual igual a zero, sempre considerar o desconto do contrato.
                        //Se achar a política com percentual diferente de zero e a data da baixa for maior que a data da política e tiver desmarcado vai zerar todos os descontos do contrato e da política. 
                        if ((!encontrou_politica || gerarMensagem) && percentual_politica != 0 &&
                            !parametro.id_permitir_desc_apos_politica)
                        {
                            percentual_desconto = 0;
                            soma_valores_desconto = 0;
                            baixa.des_desconto = String.Empty;
                        }
                    }
                }
                catch (System.NullReferenceException exe)
                {
                    throw new NullReferenceException("7-Data da Baixa menor ou igual ao Vencimento do item 4", exe);
                }
            }


            try
            {
                if (percentual_desconto > 100)
                    percentual_desconto = 100;
                decimal vl_titulo = titulo.vl_saldo_titulo;
                decimal vl_material_titulo = titulo.vl_material_titulo;
                if (usar_valor_titulo)
                    vl_titulo = titulo.vl_titulo -
                                (Math.Round(
                                    (decimal)titulo.pc_bolsa * (titulo.vl_titulo - titulo.vl_material_titulo) / 100,
                                    2, MidpointRounding.AwayFromZero));
                if ((usar_valor_titulo && vl_material_titulo > 0) ||
                    (vl_material_titulo > 0 && percentual_desconto > 0))
                {
                    if ((vl_titulo - vl_material_titulo) < 0)
                        vl_titulo = 0;
                    else
                        vl_titulo -= vl_material_titulo;
                }

                //8-Cálculos Finais:
                baixa.vl_desconto_baixa_calculado =
                    Decimal.Round(percentual_desconto * vl_titulo / 100, 2);
                baixa.soma_valores_desconto = soma_valores_desconto;
                if (baixa.vl_desconto_baixa_calculado < 0) //Caso tem 100% + desconto em valor
                    baixa.vl_desconto_baixa_calculado = vl_titulo;

                decimal percentual_aplicado = 0;

                if (vl_titulo > 0)
                {
                    percentual_aplicado = baixa.vl_desconto_baixa_calculado * 100 / vl_titulo;

                    if (parametro.per_desconto_maximo.HasValue &&
                        (decimal)parametro.per_desconto_maximo.Value < percentual_aplicado)
                    {
                        percentual_desconto = (decimal)parametro.per_desconto_maximo.Value;
                        baixa.vl_desconto_baixa_calculado = Decimal.Round(percentual_desconto * vl_titulo / 100, 2);
                    }
                }

                if (!usar_valor_titulo && vl_material_titulo > 0 && percentual_desconto > 0)
                {
                    if (vl_titulo > 0)
                        vl_titulo += vl_material_titulo;
                    else
                        vl_titulo = titulo.vl_saldo_titulo;

                }

                decimal vl_acrescimo = 0;
                if ((titulo.pc_multa_titulo > 0 || titulo.pc_juros_titulo > 0) &&
                    baixa.dt_baixa_titulo.CompareTo(titulo.dt_vcto_titulo) > 0)
                {
                    percentual_juros = titulo.pc_juros_titulo * nro_dias;
                    percentual_multa = titulo.pc_multa_titulo;
                }

                if (parametro.id_somar_descontos_financeiros)
                {
                    vl_acrescimo = Decimal.Round((decimal)percentual_juros * vl_titulo / 100, 2);
                    baixa.vl_multa_calculada = Decimal.Round((decimal)percentual_multa * vl_titulo / 100, 2);
                }
                else
                {
                    vl_acrescimo =
                        Decimal.Round(
                            (decimal)percentual_juros * (vl_titulo - baixa.vl_desconto_baixa_calculado) / 100, 2);
                    baixa.vl_multa_calculada =
                        Decimal.Round(
                            (decimal)percentual_multa * (vl_titulo - baixa.vl_desconto_baixa_calculado) / 100, 2);
                }

                baixa.vl_desconto_baixa = baixa.vl_desconto_baixa_calculado;
                baixa.vl_juros_calculado = vl_acrescimo;
                baixa.vl_liquidacao_baixa = vl_titulo + vl_acrescimo + baixa.vl_multa_calculada -
                                            baixa.vl_desconto_baixa_calculado;
                baixa.vl_acr = vl_acrescimo + baixa.vl_multa_calculada;
                baixa.vl_principal_baixa = vl_titulo;
                baixa.cd_titulo = titulo.cd_titulo;
                baixa.pc_juros_calc = percentual_juros;
                baixa.pc_multa_calc = percentual_multa;
                baixa.id_somar_descontos_financeiros = parametro.id_somar_descontos_financeiros;
            }
            catch (System.NullReferenceException exe)
            {
                throw new NullReferenceException("8-Cálculos Finais", exe);
            }
        }


        public void pulaFeriadoEFinalSemana(ref DateTime data_opcao, int cd_escola,
            ref IEnumerable<Feriado> feriadosEscola)
        {
            pulaFeriadoEFinalSemana(ref data_opcao, cd_escola, ref feriadosEscola, true);
        }

        public void pulaFeriadoEFinalSemana(ref DateTime data_opcao, int cd_escola,
            ref IEnumerable<Feriado> feriadosEscola, bool addDias)
        {
            Feriado proximo_feriado = null;
            do
            {
                //Pula a data de feriado não financeiro:
                if (proximo_feriado != null)
                {

                    if (addDias)
                    {
                        data_opcao = new DateTime(proximo_feriado.aa_feriado_fim.Value,
                            proximo_feriado.mm_feriado_fim.Value,
                            proximo_feriado.aa_feriado_fim.HasValue
                                ? proximo_feriado.dd_feriado_fim.Value
                                : data_opcao.Year);
                        data_opcao = data_opcao.AddDays(1);
                    }
                    else
                    {
                        data_opcao = new DateTime(proximo_feriado.aa_feriado.Value, proximo_feriado.mm_feriado,
                            proximo_feriado.aa_feriado.HasValue ? proximo_feriado.dd_feriado : data_opcao.Year);
                        data_opcao = data_opcao.AddDays(-1);
                    }
                }

                proximo_feriado =
                    this.getFeriadosDentroOuAposData(cd_escola, data_opcao, true, ref feriadosEscola, addDias);
                // Enquanto tiver interceção da data com o feriado financeiro:
            } while (proximo_feriado != null
                     && ((proximo_feriado.aa_feriado.HasValue && proximo_feriado.aa_feriado_fim.HasValue
                                                              && DateTime.Compare(data_opcao,
                                                                  new DateTime((int)proximo_feriado.aa_feriado,
                                                                      (int)proximo_feriado.mm_feriado,
                                                                      (int)proximo_feriado.dd_feriado)) >= 0
                                                              && DateTime.Compare(data_opcao,
                                                                  new DateTime((int)proximo_feriado.aa_feriado_fim,
                                                                      (int)proximo_feriado.mm_feriado_fim,
                                                                      (int)proximo_feriado.dd_feriado_fim)) <= 0)
                         ||
                         (!proximo_feriado.aa_feriado.HasValue && !proximo_feriado.aa_feriado_fim.HasValue
                                                               && DateTime.Compare(data_opcao,
                                                                   new DateTime((int)data_opcao.Year,
                                                                       (int)proximo_feriado.mm_feriado,
                                                                       (int)proximo_feriado.dd_feriado)) >= 0
                                                               && DateTime.Compare(data_opcao,
                                                                   new DateTime((int)data_opcao.Year,
                                                                       (int)proximo_feriado.mm_feriado_fim,
                                                                       (int)proximo_feriado.dd_feriado_fim)) <= 0)));

            if (data_opcao.DayOfWeek == DayOfWeek.Saturday || data_opcao.DayOfWeek == DayOfWeek.Sunday)
            {
                while (data_opcao.DayOfWeek == DayOfWeek.Saturday || data_opcao.DayOfWeek == DayOfWeek.Sunday)
                    if (addDias)
                        data_opcao = data_opcao.AddDays(1);
                    else
                        data_opcao = data_opcao.AddDays(-1);
                pulaFeriadoEFinalSemana(ref data_opcao, cd_escola, ref feriadosEscola, addDias);
            }
        }


        public Feriado getFeriadosDentroOuAposData(int cd_escola, DateTime ultima_data, bool feriado_financeiro, ref IEnumerable<Feriado> feriadosEscola, bool addDias)
        {
            Feriado retorno = null;

            if (feriadosEscola == null)
                feriadosEscola = DataAccessPoliticaTurma.getFeriadosEscola(cd_escola, feriado_financeiro).ToList();

            if (feriadosEscola.Count() > 0)
            {
                IEnumerable<Feriado> cloneFeriadosEscola = feriadosEscola.ToList();

                cloneFeriadosEscola = cloneFeriadosEscola.Select(x => new Feriado
                {
                    aa_feriado = x.aa_feriado.HasValue ? x.aa_feriado : short.Parse(ultima_data.Year + ""),
                    aa_feriado_fim = x.aa_feriado_fim.HasValue ? x.aa_feriado_fim : short.Parse(ultima_data.Year + ""),
                    dd_feriado = x.dd_feriado,
                    dd_feriado_fim = x.dd_feriado_fim,
                    mm_feriado = x.mm_feriado,
                    mm_feriado_fim = x.mm_feriado_fim,
                    dc_feriado = x.dc_feriado,
                    cod_feriado = x.cod_feriado
                });

                List<Feriado> listaAuxiliar = new List<Feriado>();
                List<Feriado> listFeriadoSemAno = cloneFeriadosEscola.ToList();
                for (int i = listFeriadoSemAno.Count - 1; i >= 0; i--)
                {
                    try
                    {
                        DateTime data = new DateTime((int)listFeriadoSemAno[i].aa_feriado_fim, (int)listFeriadoSemAno[i].mm_feriado_fim, (int)listFeriadoSemAno[i].dd_feriado_fim);
                        if (addDias)
                        {
                            if (ultima_data.CompareTo(data) <= 0)
                                listaAuxiliar.Add(listFeriadoSemAno[i]);
                        }
                        else
                        {
                            if (ultima_data.CompareTo(data) >= 0)
                                listaAuxiliar.Add(listFeriadoSemAno[i]);
                        }
                    }
                    catch (Exception e)
                    {
                        logger.Warn("Erro ao transformar a data - aa_feriado_fim: " + listFeriadoSemAno[i].aa_feriado_fim + "mm_feriado_fim: " + listFeriadoSemAno[i].mm_feriado_fim
                            + "dd_feriado_fim: " + listFeriadoSemAno[i].dd_feriado_fim, e);
                    }
                }

                IEnumerable<Feriado> listaResultante = from feriado in listaAuxiliar
                                                       orderby feriado.aa_feriado, feriado.mm_feriado, feriado.dd_feriado
                                                       select feriado;
                if (addDias)
                    retorno = listaResultante.FirstOrDefault();
                else
                    retorno = listaResultante.LastOrDefault();

            }
            return retorno;
        }

        public IEnumerable<Titulo> searchTituloCnab(TituloUI titulo)
        {
            IEnumerable<Titulo> retorno = new List<Titulo>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = DataAccessTitulo.searchTituloCnab(titulo);
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<Titulo> getTitulosForBaixaAutomatica(SearchParameters parametros, TituloUI titulo)
        {
            IEnumerable<Titulo> retorno = new List<Titulo>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "cd_titulo";

                //parametros.sort = parametros.sort.Replace("dt_liquidacao", "dt_liquidacao_titulo");
                //parametros.sort = parametros.sort.Replace("vlSaldoTitulo", "vl_saldo_titulo");
                //parametros.sort = parametros.sort.Replace("vlLiquidacaoBaixa", "vl_liquidacao_titulo");
                //parametros.sort = parametros.sort.Replace("dt_vcto", "dt_vcto_titulo");

                retorno = DataAccessTitulo.getTitulosForBaixaAutomatica(parametros, titulo);
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<Titulo> getTitulosForBaixaAutomaticaCheque(TituloChequeUI titulo)
        {
            IEnumerable<Titulo> retorno = new List<Titulo>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = DataAccessTitulo.getTitulosForBaixaAutomaticaCheque(titulo);
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<BaixaAutomatica> listarBaixaAutomaticasEfetuadasCheque(SearchParameters parametros, BaixaAutomaticaUI baixaAutomaticaChequeUI)
        {
            IEnumerable<BaixaAutomatica> retorno = new List<BaixaAutomatica>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "cd_baixa_automatica";

                retorno = DataAccessBaixaAutomatica.listarBaixaAutomaticasEfetuadasCheque(parametros, baixaAutomaticaChequeUI);
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<BaixaAutomatica> listarBaixaAutomaticasEfetuadasCartao(SearchParameters parametros, BaixaAutomaticaUI baixaAutomaticaChequeUI)
        {
            IEnumerable<BaixaAutomatica> retorno = new List<BaixaAutomatica>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "cd_baixa_automatica";

                retorno = DataAccessBaixaAutomatica.listarBaixaAutomaticasEfetuadasCartao(parametros, baixaAutomaticaChequeUI);
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<BaixaEfetuadaChequeUI> getBaixasEfetuadasForBaixaAutomaticaCheque(SearchParameters parametros, BaixaAutomaticaUI baixaAutomaticaChequeUI)
        {
            IEnumerable<BaixaEfetuadaChequeUI> retorno = new List<BaixaEfetuadaChequeUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "cd_baixa_titulo";

                retorno = DataAccessBaixaFinan.getBaixasEfetuadasForBaixaAutomaticaCheque(parametros, baixaAutomaticaChequeUI);
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<BaixaEfetuadaChequeUI> getBaixasEfetuadasForBaixaAutomaticaCartao(SearchParameters parametros, BaixaAutomaticaUI baixaAutomaticaCartaoUI)
        {
            IEnumerable<BaixaEfetuadaChequeUI> retorno = new List<BaixaEfetuadaChequeUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "cd_baixa_titulo";

                retorno = DataAccessBaixaFinan.getBaixasEfetuadasForBaixaAutomaticaCartao(parametros, baixaAutomaticaCartaoUI);
                transaction.Complete();
            }
            return retorno;
        }

        public int gerarBaixaAutomatica(BaixaAutomaticaUI baixaAutomaticaUi)
        {
            int ret = 0;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (baixaAutomaticaUi.cds_titulos == null || baixaAutomaticaUi.cds_titulos.Count == 0)
                {
                    throw new FinanceiroBusinessException(Messages.msgNotFoundTituloBaixaAutomatica, null, FinanceiroBusinessException.TipoErro.ERRO_NAO_EXISTE_TITULO_BAIXA_AUTOMATICA, false);
                }

                #region Salvar Baixa Automatica
                BaixaAutomatica novaBaixaAutomatica = new BaixaAutomatica();
                novaBaixaAutomatica = setaValoresBaixaAutomatica(baixaAutomaticaUi, novaBaixaAutomatica);
                novaBaixaAutomatica = DataAccessBaixaAutomatica.add(novaBaixaAutomatica, false);
                #endregion

                #region Salvar Titulos Baixa Automatica

                if (novaBaixaAutomatica == null)
                {
                    throw new FinanceiroBusinessException(Messages.msgNotFoundTituloBaixaAutomatica, null, FinanceiroBusinessException.TipoErro.ERRO_NAO_EXISTE_TITULO_BAIXA_AUTOMATICA, false);
                }
                else
                {
                    foreach (int cdTitulo in baixaAutomaticaUi.cds_titulos)
                    {
                        TitulosBaixaAutomatica novoTitulosBaixaAutomatica = new TitulosBaixaAutomatica();
                        novoTitulosBaixaAutomatica.cd_titulos_baixa_automatica = 0;
                        novoTitulosBaixaAutomatica.cd_baixa_automatica = novaBaixaAutomatica.cd_baixa_automatica;
                        novoTitulosBaixaAutomatica.cd_titulo = cdTitulo;
                        DataAccessTitulosBaixaAutomatica.add(novoTitulosBaixaAutomatica, false);
                    }
                }

                #endregion

                #region Chamar procedure sp_gerar_baixa_automática
                ret = DataAccessTitulo.gerarBaixaAutomaticaProcedure(novaBaixaAutomatica.cd_baixa_automatica, novaBaixaAutomatica.cd_usuario);
                #endregion


                transaction.Complete();
            }
            return ret;
        }

        private BaixaAutomatica setaValoresBaixaAutomatica(BaixaAutomaticaUI baixaAutomaticaUi, BaixaAutomatica novaBaixaAutomatica)
        {

            novaBaixaAutomatica.cd_baixa_automatica = 0;
            novaBaixaAutomatica.cd_escola = baixaAutomaticaUi.cd_escola;
            novaBaixaAutomatica.cd_local_movto = baixaAutomaticaUi.cd_local_movto;
            novaBaixaAutomatica.cd_usuario = baixaAutomaticaUi.cd_usuario;
            novaBaixaAutomatica.cd_cartao_credito = baixaAutomaticaUi.cd_cartao_credito;
            novaBaixaAutomatica.dt_inicial = baixaAutomaticaUi.dt_inicial.HasValue ? baixaAutomaticaUi.dt_inicial.Value.Date : (DateTime?)null;
            novaBaixaAutomatica.dt_final = baixaAutomaticaUi.dt_final.Date;
            novaBaixaAutomatica.dh_baixa_automatica = baixaAutomaticaUi.dh_baixa_automatica;
            novaBaixaAutomatica.id_tipo = baixaAutomaticaUi.id_tipo;
            novaBaixaAutomatica.id_trocar_local = baixaAutomaticaUi.id_trocar_local;

            return novaBaixaAutomatica;
        }

        public List<TituloCnab> searchTituloCnabGrade(TituloUI titulo)
        {
            return DataAccessTitulo.searchTituloCnabGrade(titulo);
        }

        public List<Titulo> getTituloBaixaFinanSimulacao(List<int> cdsTitulo, int cd_pessoa_empresa, int? cd_registro_origem, TituloDataAccess.TipoConsultaTituloEnum tipoConsulta)
        {
            List<Titulo> retorno = new List<Titulo>();
            retorno = DataAccessTitulo.getTituloBaixaFinanSimulacao(cdsTitulo, cd_pessoa_empresa, cd_registro_origem, tipoConsulta);
            return retorno;
        }

        public Titulo getTituloBaixaFinan(int cd_titulo, int cd_pessoa_empresa, TituloDataAccess.TipoConsultaTituloEnum tipoConsulta)
        {
            return DataAccessTitulo.getTituloBaixaFinan(cd_titulo, cd_pessoa_empresa, tipoConsulta);
        }

        public Titulo getTituloBaixaFinan(string dc_nosso_numero, int cd_pessoa_empresa, int cd_local_movto)
        {
            return DataAccessTitulo.getTituloBaixaFinan(dc_nosso_numero, cd_pessoa_empresa, cd_local_movto);
        }

        public List<Titulo> getTitulosBaixaFinan(List<string> dc_nosso_numero, int cd_pessoa_empresa, int cd_local_movto)
        {
            return DataAccessTitulo.getTitulosBaixaFinan(dc_nosso_numero, cd_pessoa_empresa, cd_local_movto);
        }

        public bool updateNossoNumeroTitulo(int numeroDocumento, int cd_pessoa_empresa, int cd_local_movto, string nossoNumero)
        {
            bool retorno = false;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                retorno = DataAccessTitulo.updateNossoNumeroTitulo(numeroDocumento, cd_pessoa_empresa, cd_local_movto, nossoNumero);
                transaction.Complete();
            }

            return retorno;
        }

        public Titulo editTitulosBaixaFinan(Titulo titulo)
        {
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                Titulo tituloContext = DataAccessTitulo.getTituloBaixaFinan(titulo.cd_titulo, (int)titulo.cd_pessoa_empresa, TituloDataAccess.TipoConsultaTituloEnum.HAS_EDIT_TITULO);
                if (this.verificaTituloOrContratoBaixaEfetuada((int)tituloContext.cd_origem_titulo, titulo.cd_pessoa_empresa, titulo.cd_titulo) || 
                    (tituloContext != null && tituloContext.id_status_cnab != (int)Titulo.StatusCnabTitulo.INICIAL && tituloContext.id_status_cnab != (int)Titulo.StatusCnabTitulo.CONFIRMADO_PEDIDO_BAIXA) 
                    )
                    throw new FinanceiroBusinessException(Messages.msgNotUpdateTituloEnviadoCNAB, null, FinanceiroBusinessException.TipoErro.ERRO_TITULO_ENVIADO_CNAB, false);
                if (tituloContext != null && tituloContext.id_cnab_contrato && tituloContext.id_status_cnab == (int)Titulo.StatusCnabTitulo.INICIAL)
                    throw new FinanceiroBusinessException(Messages.msgNotUpdateTituloBoleto, null, FinanceiroBusinessException.TipoErro.ERRO_TITULO_EMITIDO_BOLETO, false);
                tituloContext = Titulo.changeValuesTituloEditBaixaFinan(tituloContext, titulo);
                titulo = tituloContext;
                DataAccessTitulo.saveChanges(false);
                transaction.Complete();
            }
            return titulo;
            //return DataAccessTitulo.getTituloBaixaFinan(titulo.cd_titulo, (int)titulo.cd_pessoa_empresa, TituloDataAccess.TipoConsultaTituloEnum.HAS_TITULO_GRADE); ;
        }

        public IEnumerable<Titulo> getTituloByPessoa(SearchParameters parametros, int cd_pessoa, int cd_escola, TituloDataAccess.TipoConsultaTituloEnum tipo, bool contaSeg)
        {
            IEnumerable<Titulo> retorno = new List<Titulo>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "nm_titulo";

                parametros.sort = parametros.sort.Replace("dt_liquidacao", "dt_liquidacao_titulo");
                parametros.sort = parametros.sort.Replace("vlSaldoTitulo", "vl_saldo_titulo");
                parametros.sort = parametros.sort.Replace("vlLiquidacaoBaixa", "vl_liquidacao_titulo");
                parametros.sort = parametros.sort.Replace("dt_vcto", "dt_vcto_titulo");

                List<Titulo> listaTitulos = DataAccessTitulo.getTituloByPessoa(parametros, cd_pessoa, cd_escola, tipo, contaSeg).ToList();

                for (int i = 0; i < listaTitulos.Count; i++)
                    if (tipo != TituloDataAccess.TipoConsultaTituloEnum.TITULO_ABERTO)
                        listaTitulos[i].nm_atraso = "";

                retorno = listaTitulos;
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<Titulo> getTituloByPessoaResponsavel(int cd_pessoa_titulo, int cd_escola, int cd_contrato, bool contaSeg)
        {
            IEnumerable<Titulo> retorno = new List<Titulo>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = DataAccessTitulo.getTituloByPessoaResponsavel(cd_pessoa_titulo, cd_escola, cd_contrato, contaSeg);
                transaction.Complete();
            }
            return retorno;
        }

        public Titulo voltarEstadoAnteriorTitulo(BaixaTitulo baixa, int cd_tran_finan, int cd_pessoa_empresa, Titulo titulo)
        {
            if (titulo == null)
                titulo = DataAccessTitulo.getTituloBaixaFinan(baixa.cd_titulo, cd_pessoa_empresa, TituloDataAccess.TipoConsultaTituloEnum.HAS_EDIT_TITULO);
            titulo.dt_liquidacao_titulo = null;
            titulo.id_status_titulo = (int)Titulo.StatusTitulo.ABERTO;

            if (baixa.id_baixa_parcial)
                titulo.vl_saldo_titulo += baixa.vl_baixa_saldo_titulo;
            else
            {
                titulo.vl_desconto_titulo -= baixa.vl_desconto_baixa;
                titulo.vl_saldo_titulo += baixa.vl_baixa_saldo_titulo;
            }

            //if (Decimal.Round(titulo.vl_titulo, 2) != Decimal.Round(titulo.vl_saldo_titulo, 2))
            if (baixa.id_baixa_parcial)
            {
                //titulo.id_status_cnab = (int)Titulo.StatusCnabTitulo.PEDIDO_BAIXA;
                List<BaixaTitulo> baixasParcial = DataAccessBaixaFinan.getBaixasTransacaoFinan(cd_tran_finan, baixa.cd_baixa_titulo, baixa.cd_titulo, cd_pessoa_empresa,
                                                                                         BaixaTituloDataAccess.TipoConsultaBaixaEnum.HAS_TODAS_BAIXAS_PARCIAL_EXETO_BAIXA_EXCLUIDA).ToList();
                BaixaTitulo penultimaBaixa = baixasParcial.OrderByDescending(o => o.dt_baixa_titulo).FirstOrDefault();
                if (penultimaBaixa != null)
                    titulo.dt_liquidacao_titulo = penultimaBaixa.dt_baixa_titulo.Date;
            }
            else
            {
                if (titulo.id_status_cnab == (int)Titulo.StatusCnabTitulo.BAIXA_MANUAL)
                {
                    bool existeCNAB = DataAccessTitulo.existCnabTitulo(baixa.cd_titulo, cd_pessoa_empresa);
                    bool existeTituloRetornoCnab = DataAccessTitulo.existTituloRetornoCnab(titulo.cd_titulo);
                    if (existeCNAB)
                    {
                        //if exists(select 1 from t_titulo_retorno_cnab where cd_titulo = titulo.cd_titulo and id_tipo_retorno = 2)
                          //titulo.id_status_cnab = (int)Titulo.StatusCnabTitulo.CONFIRMADO_ENVIO;
                        if (existeTituloRetornoCnab)
                            titulo.id_status_cnab = (int)Titulo.StatusCnabTitulo.CONFIRMADO_ENVIO;
                        else
                          titulo.id_status_cnab = (int)Titulo.StatusCnabTitulo.ENVIADO_GERADO;
                    }
                    else
                        titulo.id_status_cnab = (int)Titulo.StatusCnabTitulo.INICIAL;
                }

            }
            titulo.vl_liquidacao_titulo -= baixa.vl_baixa_saldo_titulo;
            titulo.vl_multa_titulo -= baixa.vl_multa_calculada;
            titulo.vl_juros_titulo -= baixa.vl_juros_calculado;
            titulo.vl_multa_liquidada -= baixa.vl_multa_baixa;
            titulo.vl_juros_liquidado -= baixa.vl_juros_baixa;
            titulo.vl_desconto_juros -= baixa.vl_desc_juros_baixa;
            titulo.vl_desconto_multa -= baixa.vl_desc_multa_baixa;

            return titulo;
        }

        public Titulo aplicarTituloTaxaBancaria(Titulo titulo)
        {
            DateTime dtEmissao = titulo.dt_emissao_titulo;
            aplicarTaxaBancaria(titulo, 1, ref dtEmissao);
            return titulo;
        }
            public Titulo aplicarBaixaTitulo(BaixaTitulo baixa, Titulo titulo)
        {
            //Atualiza o saldo do título:
            titulo.dt_liquidacao_titulo = baixa.dt_baixa_titulo.Date;

            if (baixa.id_baixa_parcial)
            {
                decimal vl_saldo_titulo = baixa.Titulo.vl_saldo_titulo;
                vl_saldo_titulo -= (baixa.vl_principal_baixa - baixa.vl_desconto_baixa);
                baixa.vl_desconto_baixa_calculado = baixa.vl_desconto_baixa;
                baixa.vl_desconto_baixa = 0;
                if (baixa.vl_desconto_baixa <= 0 && vl_saldo_titulo == 0 && (baixa.vl_principal_baixa - baixa.vl_liquidacao_baixa) > 0)
                {
                    vl_saldo_titulo = baixa.Titulo.vl_saldo_titulo - baixa.vl_liquidacao_baixa;
                }
                baixa.Titulo.vl_saldo_titulo = vl_saldo_titulo;
            }
            else
            {
                //Verifica se por acaso o pagamento veio menor que o saldo do título e não foi informado o deconto, logo o título não foi pago inteiramente, está sendo feito uma baixa parcial.
                if ((baixa.vl_desconto_baixa > 0 && decimal.Round((baixa.vl_desconto_baixa + baixa.vl_liquidacao_baixa), 2) < baixa.Titulo.vl_saldo_titulo) ||
                   (baixa.vl_desconto_baixa <= 0 && baixa.vl_liquidacao_baixa < baixa.Titulo.vl_saldo_titulo))
                {
                    if (baixa.vl_desconto_baixa > 0)
                        throw new FinanceiroBusinessException(Messages.msgErroCalculosBaixaTitulo, null, FinanceiroBusinessException.TipoErro.ERRO_CALCULO_APLICAR_BAIXA_TITULO, false);
                    baixa.id_baixa_parcial = true;
                    baixa.vl_desconto_baixa_calculado = baixa.vl_desconto_baixa;
                    baixa.vl_desconto_baixa = 0;
                    baixa.Titulo.vl_saldo_titulo -= baixa.vl_liquidacao_baixa;
                }
                else
                {
                    baixa.Titulo.vl_desconto_titulo += baixa.vl_desconto_baixa;
                    baixa.Titulo.vl_saldo_titulo -= baixa.vl_principal_baixa;
                }
            }
            titulo.vl_liquidacao_titulo += baixa.vl_principal_baixa - titulo.vl_saldo_titulo;
            baixa.vl_baixa_saldo_titulo = baixa.vl_principal_baixa - titulo.vl_saldo_titulo;
            titulo.vl_multa_titulo += baixa.vl_multa_calculada;
            titulo.vl_multa_liquidada += baixa.vl_multa_baixa;
            titulo.vl_juros_titulo += baixa.vl_juros_calculado;
            titulo.vl_juros_liquidado += baixa.vl_juros_baixa;
            titulo.vl_desconto_juros += baixa.vl_desc_juros_baixa;
            titulo.vl_desconto_multa += baixa.vl_desc_multa_baixa;


            //Arredondando os valores para gravar na base de dados
            titulo.vl_saldo_titulo = Decimal.Round(titulo.vl_saldo_titulo, 2);
            titulo.vl_liquidacao_titulo = Decimal.Round(titulo.vl_liquidacao_titulo, 2);
            baixa.vl_baixa_saldo_titulo = Decimal.Round(baixa.vl_baixa_saldo_titulo, 2);
            baixa.vl_liquidacao_baixa = Decimal.Round(baixa.vl_liquidacao_baixa);
            //Verifica se algum título já se encontra fechado e o atualiza:
            if (titulo.vl_saldo_titulo == 0 && (titulo.vl_juros_titulo + titulo.vl_multa_titulo) <= (titulo.vl_juros_liquidado + titulo.vl_multa_liquidada))
                titulo.id_status_titulo = (int)Titulo.StatusTitulo.FECHADO;
            else
                titulo.id_status_titulo = (int)Titulo.StatusTitulo.ABERTO;
            if (baixa.Titulo.id_status_cnab == (int)Titulo.StatusCnabTitulo.ENVIADO_GERADO || baixa.Titulo.id_status_cnab == (int)Titulo.StatusCnabTitulo.CONFIRMADO_ENVIO)
                if (DataAccessLocalMovto.verificaLocalTituloTemCNAB(baixa.Titulo.cd_local_movto, (int)baixa.Titulo.cd_pessoa_empresa))
                    baixa.Titulo.id_status_cnab = (int)Titulo.StatusCnabTitulo.BAIXA_MANUAL;
            return titulo;
        }

        public List<Titulo> getTitulosGridByMovimento(int cd_movto, int cd_empresa, int? cd_aluno)
        {
            List<Titulo> retorno = new List<Titulo>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = DataAccessTitulo.getTitulosGridByMovimento(cd_movto, cd_empresa, cd_aluno);
                transaction.Complete();
            }
            return retorno;
        }

        public bool verificarTituloContaSegura(int cd_titulo, int cd_empresa)
        {
            //Se o usuário não tiver permissão de conta segura, o título não deve ser mostrado
            bool contaSeg = DataAccessTitulo.verificarTituloContaSegura(cd_titulo, cd_empresa);
            if (contaSeg)
                throw new FinanceiroBusinessException(Utils.Messages.Messages.msgUsuarioSemPermissao, null, FinanceiroBusinessException.TipoErro.ERRO_PERMISSAO, false);
            return contaSeg;
        }

        public IEnumerable<Titulo> getTitulosByOrigem(int cdOrigemTitulo, int idOrigemTitulo, int cd_empresa)
        {
            return DataAccessTitulo.getTitulosByOrigem(cdOrigemTitulo, idOrigemTitulo, cd_empresa);
        }

        public bool verificarStatusCnabTitulo(int[] cdTituls, int cd_empresa, int cd_cnab, byte id_tipo_cnab)
        {
            return DataAccessTitulo.verificarStatusCnabTitulo(cdTituls, cd_empresa, cd_cnab, id_tipo_cnab);
        }

        public IEnumerable<Titulo> getDadosAdicionaisTituloParaCnab(int[] cdTitulos, int cd_empresa)
        {
            return DataAccessTitulo.getDadosAdicionaisTituloParaCnab(cdTitulos, cd_empresa);
        }

        public IEnumerable<Titulo> getTitulosByCnab(int cd_cnab, int cd_empresa)
        {
            return DataAccessTitulo.getTitulosByCnab(cd_cnab, cd_empresa);
        }

        public void trocarStatusCnabTitulos(int[] cdCnabs, int cd_empresa, Titulo.StatusCnabTitulo statusTitulo, int? cd_contrato = null)
        {
            List<Titulo> titulos = DataAccessTitulo.getTitulosByCnab(cdCnabs, cd_empresa).ToList();
            if (titulos != null)
            {
                foreach (var t in titulos)
                {
                    //TODO - Uilian Silva
                    //NÃO ALTERAR STATUS CNAB (Matricula/titulos/emitido cnab)
                    //Não alterar status dos titulos para Emitido cnab
                    //quando CNAB tiver cd_contrato, pois os boletos não
                    //foram enviados para o banco; E gravar id_cnab_contrato para true;
                    if ((cd_contrato != null && cd_contrato > 0) &&
                        statusTitulo == Titulo.StatusCnabTitulo.ENVIADO_GERADO && !t.id_cnab_contrato)
                    {
                        t.id_status_cnab = 0;
                        t.id_cnab_contrato = true;
                    }
                    else
                    {
                        t.id_status_cnab = (byte)statusTitulo;
                    }
                }
            }
            DataAccessTitulo.saveChanges(false);
        }

        public string getResponsavelTitulo(int cd_titulo, int cd_pessoa_empresa)
        {
            return DataAccessTitulo.getResponsavelTitulo(cd_titulo, cd_pessoa_empresa);
        }

        public IEnumerable<Titulo> getTitulosAbertoContrato(int cd_empresa, int cd_contrato)
        {
            return DataAccessTitulo.getTitulosAbertoContrato(cd_empresa, cd_contrato);
        }

        public Decimal getSaldoTitulosMatricula(int cd_empresa, int cd_contrato)
        {
            return DataAccessTitulo.getSaldoTitulosMatricula(cd_empresa, cd_contrato);
        }

        public IEnumerable<CarneUI> getCarnePorMovimentos(int cdMovimento, int cdEscola)
        {
            List<CarneUI> listCarne = new List<CarneUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                listCarne = DataAccessTitulo.getCarnePorMovimentos(cdMovimento, cdEscola).ToList();
                transaction.Complete();
            }
            return listCarne;
        }

        public IEnumerable<CarneUI> getCarnePorContrato(int cdContrato, int cdEscola, int parcIniCarne, int parcFimCarne)
        {
            List<CarneUI> listCarne = new List<CarneUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                listCarne = DataAccessTitulo.getCarnePorContrato(cdContrato, cdEscola, parcIniCarne, parcFimCarne).ToList();
                transaction.Complete();
            }
            return listCarne;
        }

        public IEnumerable<Titulo> searchTituloFaturamento(SearchParameters parametros, int cd_pessoa_empresa, int cd_pessoa, bool responsavel, bool inicio, int? numeroTitulo,
                                               int? parcelaTitulo, string valorTitulo, DateTime? dtInicial, DateTime? dtFinal, bool contaSegura, byte tipoTitulo)
        {
            IEnumerable<Titulo> retorno = new List<Titulo>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "nm_titulo";
                parametros.sort = parametros.sort.Replace("dt_emissao", "dt_emissao_titulo");
                parametros.sort = parametros.sort.Replace("dt_vcto", "dt_vcto_titulo");
                parametros.sort = parametros.sort.Replace("vlTitulo", "vl_titulo");
                parametros.sort = parametros.sort.Replace("vlSaldoTitulo", "vl_saldo_titulo");
                parametros.sort = parametros.sort.Replace("nomeResponsavel", "no_pessoa");
                parametros.sort = parametros.sort.Replace("statusTitulo", "id_status_titulo");
                // parametros.sort = parametros.sort.Replace("nomeResponsavel", "PessoaResponsavel.no_pessoa");
                retorno = DataAccessTitulo.searchTituloFaturamento(parametros, cd_pessoa_empresa, cd_pessoa, responsavel, inicio, numeroTitulo, parcelaTitulo, valorTitulo, dtInicial, dtFinal, contaSegura, tipoTitulo);
                transaction.Complete();
            }
            return retorno;
        }

        public Titulo getTituloBaixaFinanMovimentoNF(int cd_baixa_titulo, int cd_pessoa_empresa)
        {
            return DataAccessTitulo.getTituloBaixaFinanMovimentoNF(cd_baixa_titulo, cd_pessoa_empresa);
        }

        public IEnumerable<Titulo> getTitulosByMovimento(int cd_movto, int cd_empresa)
        {
            return DataAccessTitulo.getTitulosByMovimento(cd_movto, cd_empresa);
        }

        public List<Titulo> getTitulosAbertosImpressaoAdt(int cdContrato, int cdEscola)
        {
            List<Titulo> retorno = new List<Titulo>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = DataAccessTitulo.getTitulosAbertosImpressaoAdt(cdContrato, cdEscola);
                transaction.Complete();
            }
            return retorno;
        }

        public List<Titulo> getTitulosAbertosAdicionarParcImpressaoAdt(int cdContrato, int cdEscola, int qtd_titulos_adt)
        {
            List<Titulo> retorno = new List<Titulo>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = DataAccessTitulo.getTitulosAbertosAdicionarParcImpressaoAdt(cdContrato, cdEscola, qtd_titulos_adt);
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<ChequeUI> getRptChequesAbertos(int cd_empresa, int cd_pessoa_aluno, int cd_banco, string emitente, bool liquidados, string nm_cheques, string vl_titulo, int nm_agencia,
            int nm_ccorrente, DateTime? dt_ini_bPara, DateTime? dt_fim_bPara, DateTime? dt_ini, DateTime? dt_fim, bool emissao, bool liquidacao, byte natureza)
        {
            List<ChequeUI> retorno = new List<ChequeUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = DataAccessTitulo.getRptChequesAbertos(cd_empresa, cd_pessoa_aluno, cd_banco, emitente, liquidados,
                    nm_cheques, vl_titulo, nm_agencia, nm_ccorrente, dt_ini_bPara, dt_fim_bPara, dt_ini, dt_fim, emissao, liquidacao,  natureza).ToList();
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<ChequeUI> getRptChequesLiquidados(int cd_empresa, int cd_pessoa_aluno, int cd_banco, string emitente, bool liquidados, string nm_cheques, string vl_titulo, int nm_agencia,
   int nm_ccorrente, DateTime? dt_ini_bPara, DateTime? dt_fim_bPara, DateTime? dt_ini, DateTime? dt_fim, bool emissao, bool liquidacao, byte natureza)
        {
            List<ChequeUI> retorno = new List<ChequeUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = DataAccessTitulo.getRptChequesLiquidados(cd_empresa, cd_pessoa_aluno, cd_banco, emitente, liquidados,
                    nm_cheques, vl_titulo, nm_agencia, nm_ccorrente, dt_ini_bPara, dt_fim_bPara, dt_ini, dt_fim, emissao, liquidacao, natureza).ToList();
                transaction.Complete();
            }
            return retorno;
        }

        public bool verificaTituloVencido(int cdPessoa, DateTime dataHoje, int cd_escola)
        {
            return DataAccessTitulo.verificaTituloVencido(cdPessoa, dataHoje, cd_escola);
        }

        public IEnumerable<Titulo> getTitulosContrato(List<int> cdTitulos, int cd_escola)
        {
            return DataAccessTitulo.getTitulosContrato(cdTitulos, cd_escola);
        }

        public List<int> getAlunosQuePossuemTitulosAbertoMes(List<int> cdPessoaAlunos, int cd_empresa, DateTime dt_diario)
        {
            return DataAccessTitulo.getAlunosQuePossuemTitulosAbertoMes(cdPessoaAlunos, cd_empresa, dt_diario);
        }

        #endregion

        #region Baixa

        public IEnumerable<BaixaTitulo> getBaixaTituloByIdTitulo(int cd_titulo, int cd_pessoa_empresa)
        {
            return DataAccessBaixaFinan.getBaixaTituloByIdTitulo(cd_titulo, cd_pessoa_empresa);
        }

        public IEnumerable<BaixaTitulo> getBaixasTransacaoFinan(int cd_transacao_finan, int cd_baixa_titulo, int cd_titulo, int cd_pessoa_empresa, BaixaTituloDataAccess.TipoConsultaBaixaEnum tipoConsulta)
        {
            List<BaixaTitulo> listBaixaTitulo = new List<BaixaTitulo>();
            listBaixaTitulo = DataAccessBaixaFinan.getBaixasTransacaoFinan(cd_transacao_finan, cd_baixa_titulo, cd_titulo, cd_pessoa_empresa, tipoConsulta).ToList();
            return listBaixaTitulo;
        }

        public Recibo getReciboByBaixa(int cd_baixa, int cd_empresa)
        {
            Recibo retorno = new Recibo();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = DataAccessBaixaFinan.getReciboByBaixa(cd_baixa, cd_empresa);
                transaction.Complete();
            }
            return retorno;
        }

        public ReciboAgrupadoUI getReciboAgrupado(string cds_titulos_selecionados, int cd_empresa)
        {
            ReciboAgrupadoUI retorno = new ReciboAgrupadoUI();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {

                retorno = DataAccessBaixaFinan.getReciboAgrupado(cds_titulos_selecionados, cd_empresa);
                transaction.Complete();
            }
            return retorno;
        }

        public ReciboPagamentoUI getReciboPagamentoByBaixa(int cd_baixa, int cd_empresa)
        {
            ReciboPagamentoUI retorno = new ReciboPagamentoUI();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = DataAccessBaixaFinan.getReciboPagamentoByBaixa(cd_baixa, cd_empresa);
                transaction.Complete();
            }
            return retorno;
        }

		 public ReciboPagamentoUI getVerificaReciboPagamentoByBaixa(int cd_baixa, int cd_empresa)
        {
            ReciboPagamentoUI retorno = new ReciboPagamentoUI();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = DataAccessBaixaFinan.getVerificaReciboPagamentoByBaixa(cd_baixa, cd_empresa);
                transaction.Complete();
            }
            return retorno;
        }

        public bool validaReciboAgrupadoAlunosDiferentes(List<int> cds_titulos_selecionados, int cd_empresa)
        {
            bool retorno;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = DataAccessBaixaFinan.validaReciboAgrupadoAlunosResponsaveisDiferentes(cds_titulos_selecionados, cd_empresa, (int)BaixaTituloDataAccess.TipoValidacaoReciboAgrupadoEnum.ALUNO);
                transaction.Complete();
            }
            return retorno;
        }

        public bool validaReciboAgrupadoResponsaveisDiferentes(List<int> cds_titulos_selecionados, int cd_empresa)
        {
            bool retorno;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = DataAccessBaixaFinan.validaReciboAgrupadoAlunosResponsaveisDiferentes(cds_titulos_selecionados, cd_empresa, (int)BaixaTituloDataAccess.TipoValidacaoReciboAgrupadoEnum.RESPONSAVEL);
                transaction.Complete();
            }
            return retorno;
        }

        public ReciboConfirmacaoUI getReciboConfirmacaoByContrato(int cd_contrato, int cd_empresa)
        {
            ReciboConfirmacaoUI retorno = new ReciboConfirmacaoUI();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                //DataAccessTitulo
                retorno = DataAccessTitulo.getReciboConfirmacaoByContrato(cd_contrato, cd_empresa); 
                transaction.Complete();
            }
            return retorno;
        }

        public ReciboConfirmacaoUI getReciboConfirmacaoByMovimento(int cd_movimento, int cd_empresa)
        {
            ReciboConfirmacaoUI retorno = new ReciboConfirmacaoUI();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                //DataAccessTitulo
                retorno = DataAccessTitulo.getReciboConfirmacaoByMovimento(cd_movimento, cd_empresa);
                transaction.Complete();
            }
            return retorno;
        }

        public List<ReciboConfirmacaoParcelasUI> getParcelasReciboConfirmacaoByContrato(int cd_contrato, int cd_empresa)
        {
            List<ReciboConfirmacaoParcelasUI> retorno = new List<ReciboConfirmacaoParcelasUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                //DataAccessTitulo
                retorno = DataAccessTitulo.getParcelasReciboConfirmacaoByContrato(cd_contrato, cd_empresa);
                transaction.Complete();
            }
            return retorno;
        }

        public List<ReciboConfirmacaoParcelasUI> getParcelasReciboConfirmacaoByMovimento(int cd_movimento, int cd_empresa)
        {
            List<ReciboConfirmacaoParcelasUI> retorno = new List<ReciboConfirmacaoParcelasUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                //DataAccessTitulo
                retorno = DataAccessTitulo.getParcelasReciboConfirmacaoByMovimento(cd_movimento, cd_empresa);
                transaction.Complete();
            }
            return retorno;
        }

        public bool deleteBaixaTitulo(BaixaTitulo baixa)
        {
            return DataAccessBaixaFinan.delete(baixa, false);
        }

        public bool verificarTituloOrigemMatricula(int cd_baixa, int cd_escola)
        {
            if (!DataAccessBaixaFinan.verificarTituloOrigemMatricula(cd_baixa, cd_escola))
                throw new FinanceiroBusinessException(Utils.Messages.Messages.msgErroOrigemTituloMatricula, null, FinanceiroBusinessException.TipoErro.ERRO_ORIGEM_TITULO, false);
            return true;
        }

        public IEnumerable<BaixaTitulo> getBaixaTitulosBolsaContrato(int cd_contrato, int cd_pessoa_empresa)
        {
            return DataAccessBaixaFinan.getBaixaTitulosBolsaContrato(cd_contrato, cd_pessoa_empresa);
        }


        public IEnumerable<BaixaTitulo> getBaixaTitulosBolsaContrato(int cd_contrato, int cd_pessoa_empresa, List<int> cdTitulos)
        {
            return DataAccessBaixaFinan.getBaixaTitulosBolsaContrato(cd_contrato, cd_pessoa_empresa, cdTitulos);
        }
        #endregion

        #region Local Movimento
        public LocalMovto getLocalMovimentoWithPessoaBanco(int cd_local_movto)
        {
            return DataAccessLocalMovto.getLocalMovimentoWithPessoaBanco(cd_local_movto);
        }

        public LocalMovto findCodigoClienteForCnab(int cd_empresa, int cd_local_movto)
        {
            return DataAccessLocalMovto.findCodigoClienteForCnab(cd_empresa, cd_local_movto);
        }

        public List<LocalMovto> getLocalMovtoByEscola(int cdEscola, int cd_local_movto, bool semcarteira)
        {
            return DataAccessLocalMovto.getLocalMovtoByEscola(cdEscola, cd_local_movto, semcarteira);
        }

        public List<LocalMovto> getAllLocalMovtoByEscola(int cdEscola, int cd_local_movto)
        {
            return DataAccessLocalMovto.getAllLocalMovtoByEscola(cdEscola, cd_local_movto);
        }

        public IEnumerable<LocalMovto> getAllLocalMovtoBanco(int cdEscola)
        {
            return DataAccessLocalMovto.getAllLocalMovtoBanco(cdEscola);
        }

        public List<LocalMovto> getLocalMovtoCdEEsc(int cdEscola, int? cdLocalMovto)
        {
            List<LocalMovto> retorno = new List<LocalMovto>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = DataAccessLocalMovto.getLocalMovtoCdEEsc(cdEscola, cdLocalMovto);
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<LocalMovtoUI> getLocalMovtoSearch(SearchParameters parametros, int cdEscola, string nome, string nmBanco, bool inicio, bool? status, int tipo, string pessoa, int cd_pessoa_usuario)
        {
            IEnumerable<LocalMovtoUI> retorno = new List<LocalMovtoUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "no_local_movto";
                parametros.sort = parametros.sort.Replace("desc_tipo_local", "nm_tipo_local");
                parametros.sort = parametros.sort.Replace("conta_conjunta", "id_conta_conjunta");
                parametros.sort = parametros.sort.Replace("local_ativo", "id_local_ativo");
                retorno = DataAccessLocalMovto.getLocalMovtoSearch(parametros, cdEscola, nome, nmBanco, inicio, status, tipo, pessoa, cd_pessoa_usuario);
                transaction.Complete();
            }
            return retorno;
        }

        public LocalMovtoUI getLocalMovtoById(int cdEscola, int cdLocalMovto)
        {
            return DataAccessLocalMovto.getLocalMovtoById(cdEscola, cdLocalMovto);
        }

        public LocalMovtoUI getLocalByTitulo(int cdEscola, int cd_local_movto)
        {
            return DataAccessLocalMovto.getLocalByTitulo(cdEscola, cd_local_movto);
        }

        public bool deleteAllLocalMovto(List<LocalMovto> locais)
        {
            try
            {
                bool retorno = true;
                using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
                {
                    foreach (LocalMovto l in locais)
                    {
                        LocalMovto localDel = DataAccessLocalMovto.findById(l.cd_local_movto, false);
                        deletaAllTaxaBancaria(localDel.cd_local_movto);

                        retorno = DataAccessLocalMovto.delete(localDel, false);
                    }
                    transaction.Complete();
                }
                return retorno;

            }
            catch (Exception ex)
            {
                if (ex.InnerException != null &&
                    ex.InnerException.InnerException != null &&
                    ex.InnerException.InnerException != null &&
                    (ex.InnerException.InnerException.Message.Contains("gatilho") ||
                     ex.InnerException.InnerException.Message.Contains("trigger")))
                {

                    throw new FinanceiroBusinessException(ex.InnerException.InnerException.Message.Replace("A transação foi encerrada no gatilho. O lote foi anulado.", "."),
                        null, FinanceiroBusinessException.TipoErro.ERRO_TRIGGER_DELETAR_LOCAL_MOVIMENTO, false);
                }

                throw ex;
            }
            
        }

        private void deletaAllTaxaBancaria(int cd_local_mvto)
        {
            var taxas = DataAccessTaxaBancaria.findAll(false).Where(l => l.cd_local_movto == cd_local_mvto).ToList();
            DataAccessTaxaBancaria.deleteRange(taxas, false);
        }

        public LocalMovtoUI postLocalMovto(LocalMovto local)
        {
            try
            {
                LocalMovtoUI retornar = new LocalMovtoUI();
                using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
                {

                    local = DataAccessLocalMovto.add(local, false);
                    if (local.cd_pessoa_local.HasValue)
                        BusinessEmpresa.postEmpresaPessoa(new PessoaEscola
                        {
                            cd_escola = local.cd_pessoa_empresa,
                            cd_pessoa = (int)local.cd_pessoa_local
                        });
                    if (local.cd_pessoa_banco.HasValue)
                        BusinessEmpresa.postEmpresaPessoa(new PessoaEscola
                        {
                            cd_escola = local.cd_pessoa_empresa,
                            cd_pessoa = (int)local.cd_pessoa_banco
                        });
                    transaction.Complete();
                }
                retornar = DataAccessLocalMovto.getLocalMovtoById(local.cd_pessoa_empresa, local.cd_local_movto);
                return retornar;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null &&
                    ex.InnerException.InnerException != null &&
                    ex.InnerException.InnerException != null &&
                    (ex.InnerException.InnerException.Message.Contains("gatilho") ||
                     ex.InnerException.InnerException.Message.Contains("trigger")))
                {

                    throw new FinanceiroBusinessException(ex.InnerException.InnerException.Message.Replace("A transação foi encerrada no gatilho. O lote foi anulado.", "."),
                        null, FinanceiroBusinessException.TipoErro.ERRO_TRIGGER_INSERIR_LOCAL_MOVIMENTO, false);
                }

                throw ex;
            }

        }

        public LocalMovtoUI putLocalMovto(LocalMovto local)
        {
            LocalMovtoUI retornar = new LocalMovtoUI();
            try
            {
                using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
                {
                    LocalMovto localContext = DataAccessLocalMovto.findLocalMovtoById(local.cd_pessoa_empresa, local.cd_local_movto);
                    //Verifica se a carteira foi alterado e é usada em algum CNAB
                    if (local.cd_carteira_cnab != localContext.cd_carteira_cnab && localContext.cd_carteira_cnab > 0)
                    {
                        bool existeCNAB = verificaCarteiraCnab(localContext.cd_carteira_cnab.Value);
                        if (existeCNAB)
                            throw new FinanceiroBusinessException(string.Format(Utils.Messages.Messages.msgErroCarteiraUsadaCnab), null, FinanceiroBusinessException.TipoErro.ERRO_CARTEIRA_USADA_CNAB, false);
                    }
                    
                    localContext.copy(local);
                    localContext.cd_local_banco = local.cd_local_banco;
                    localContext.TaxaBancaria = null;
                    local.cd_pessoa_local = local.cd_pessoa_local == 0 ? null : local.cd_pessoa_local;
                    localContext.cd_pessoa_local = local.cd_pessoa_local;
                    localContext.nm_digito_cedente = local.nm_digito_cedente;
                    local.cd_pessoa_banco = local.cd_pessoa_banco == 0 ? null : local.cd_pessoa_banco;
                    localContext.cd_pessoa_banco = local.cd_pessoa_banco;
                    localContext.nm_sequencia = local.nm_sequencia;

                    DataAccessLocalMovto.saveChanges(false);

                    local.TaxaBancaria = local.TaxaBancaria == null ? new List<TaxaBancaria>() : local.TaxaBancaria;
                    alterarTaxaBancaria(local.TaxaBancaria, localContext.cd_local_movto);

                    if (local.cd_pessoa_local.HasValue)
                        BusinessEmpresa.postEmpresaPessoa(new PessoaEscola
                        {
                            cd_escola = local.cd_pessoa_empresa,
                            cd_pessoa = (int)local.cd_pessoa_local
                        });
                    if (local.cd_pessoa_banco.HasValue)
                        BusinessEmpresa.postEmpresaPessoa(new PessoaEscola
                        {
                            cd_escola = local.cd_pessoa_empresa,
                            cd_pessoa = (int)local.cd_pessoa_banco
                        });
                    
                    transaction.Complete();
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null &&
                    ex.InnerException.InnerException != null &&
                    ex.InnerException.InnerException != null &&
                    (ex.InnerException.InnerException.Message.Contains("gatilho") ||
                     ex.InnerException.InnerException.Message.Contains("trigger")))
                {

                    throw new FinanceiroBusinessException(ex.InnerException.InnerException.Message.Replace("A transação foi encerrada no gatilho. O lote foi anulado.", "."),
                        null, FinanceiroBusinessException.TipoErro.ERRO_TRIGGER_ALTERAR_LOCAL_MOVIMENTO, false);
                }

                throw ex;
            }
            retornar = DataAccessLocalMovto.getLocalMovtoById(local.cd_pessoa_empresa, local.cd_local_movto);
            return retornar;
        }

        private void alterarTaxaBancaria(ICollection<TaxaBancaria> taxaBancariaView, int cd_local_movto)
        {
            var taxasContext = DataAccessTaxaBancaria.findAll(false).Where(t => t.cd_local_movto == cd_local_movto).ToList();

            List<TaxaBancaria> insertTaxaBancaria = taxaBancariaView.Where(taxaView => !taxasContext.Any(taxa => taxa.cd_taxa_bancaria == taxaView.cd_taxa_bancaria)).ToList();
            List<TaxaBancaria> deleteTaxas = taxasContext.Where(taxa => !taxaBancariaView.Any(taxaView => taxaView.cd_taxa_bancaria == taxa.cd_taxa_bancaria)).ToList();
            List<TaxaBancaria> editTaxaBancaria = taxaBancariaView.Where(taxaView => !insertTaxaBancaria.Any(taxa => taxa.cd_taxa_bancaria == taxaView.cd_taxa_bancaria)).ToList();

            if (deleteTaxas.Count > 0)
                DataAccessTaxaBancaria.deleteRange(deleteTaxas, false);

            foreach (var taxaBancaria in insertTaxaBancaria)
            {
                taxaBancaria.cd_local_movto = cd_local_movto;
                DataAccessTaxaBancaria.add(taxaBancaria, false);
            }

            if (editTaxaBancaria.Count > 0)
            {
                
                foreach (var taxaBancView in editTaxaBancaria)
                {
                    var taxaBancariaContext = taxasContext.Where(taxa => taxa.cd_taxa_bancaria == taxaBancView.cd_taxa_bancaria).FirstOrDefault();
                    if (taxaBancariaContext != null)
                    {
                        taxaBancariaContext.nm_dias = taxaBancView.nm_dias;
                        taxaBancariaContext.nm_parcela = taxaBancView.nm_parcela;
                        taxaBancariaContext.pc_taxa = taxaBancView.pc_taxa;
                    }
                }
            }
            DataAccessTaxaBancaria.saveChanges(false);
        }

        public List<LocalMovto> getLocalMovimentoSomenteLeitura(int cdEscola, int cd_loc_mvto, LocalMovtoDataAccess.TipoConsultaLocalMovto tipoConsulta, int cd_pessoa_usuario)
        {
            List<LocalMovto> retorno = new List<LocalMovto>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = DataAccessLocalMovto.getLocalMovimento(cdEscola, cd_loc_mvto, tipoConsulta, cd_pessoa_usuario);
            }
            return retorno;
        }

        public List<LocalMovto> getLocalMovimentoSomenteLeituraComFiltrosTrocaFinanceira(int cdEscola, int cd_loc_mvto, int cd_tipo_financeiro, LocalMovtoDataAccess.TipoConsultaLocalMovto tipoConsulta, int cd_pessoa_usuario)
        {
            List<LocalMovto> retorno = new List<LocalMovto>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = DataAccessLocalMovto.getLocalMovimentoComFiltrosTrocaFinanceira(cdEscola, cd_loc_mvto, cd_tipo_financeiro, tipoConsulta, cd_pessoa_usuario);
            }
            return retorno;
        }

        public List<LocalMovto> getLocalMovtoBaixa(int cd_escola, int? cd_loc_mvto, int natureza, int[] listPessoas, int cd_pessoa_usuario)
        {
            return DataAccessLocalMovto.getLocalMovtoBaixa(cd_escola, cd_loc_mvto, natureza, listPessoas, cd_pessoa_usuario);
        }

        public IEnumerable<LocalMovto> getAllLocalMovto(int cdEscola, bool isOrigem, int cd_pessoa_usuario)
        {
            return DataAccessLocalMovto.getAllLocalMovto(cdEscola, isOrigem, cd_pessoa_usuario);
        }

        public IEnumerable<LocalMovto> getLocalMovtoAtivosWithConta(int cdEscola, bool isOrigem, bool ativos, int cdLocalMovto)
        {
            return DataAccessLocalMovto.getLocalMovtoAtivosWithConta(cdEscola, isOrigem, ativos, cdLocalMovto);
        }

        public IEnumerable<LocalMovto> getLocalMovtoAtivosWithCodigo(int cdEscola, bool isOrigem, int cd_local)
        {
            return DataAccessLocalMovto.getLocalMovtoAtivosWithCodigo(cdEscola, isOrigem, cd_local);
        }

        public LocalMovto findLocalMovtoComCarteira(int cdEscola, int cdLocalMovto)
        {
            return DataAccessLocalMovto.findLocalMovtoComCarteira(cdEscola, cdLocalMovto);
        }

        public LocalMovto findLocalMovtoById(int cdEscola, int cdLocalMovto)
        {
            return DataAccessLocalMovto.findLocalMovtoById(cdEscola, cdLocalMovto);
        }

        public IEnumerable<LocalMovimentoWithContaUI> getLocalMovtoWithContaByEscola(int cdEscola, int cd_pessoa_usuario)
        {
            return DataAccessLocalMovto.getLocalMovtoWithContaByEscola(cdEscola, cd_pessoa_usuario);
        }

        public bool verificaCarteiraCnab(int cdCarteira)
        {
            return DataAccessLocalMovto.verificaCarteiraCnab(cdCarteira);
        }

        public long getNossoNumeroLocalMovimento(int cd_escola, int cd_local_movto)
        {
            return DataAccessLocalMovto.getNossoNumeroLocalMovimento(cd_escola, cd_local_movto);
        }

        public IEnumerable<LocalMovto> getLocalMovtoAtivosWithContaUsuario(int cdEscola, bool isOrigem, int cdLocalMovto, int cd_pessoa_usuario)
        {
            return DataAccessLocalMovto.getLocalMovtoAtivosWithContaUsuario(cdEscola, isOrigem, cdLocalMovto, cd_pessoa_usuario);
        }

        public IEnumerable<LocalMovto> getLocalMovtoProspect(int cdEscola, int cd_loc_mvto, int cd_pessoa_usuario)
        {
            return DataAccessLocalMovto.getLocalMovtoProspect(cdEscola, cd_loc_mvto, cd_pessoa_usuario);
        }

        public IEnumerable<LocalMovto> getAllLocalMovtoCartao(int cdEscola)
        {
            return DataAccessLocalMovto.getAllLocalMovtoCartao(cdEscola);
        }

        public IEnumerable<LocalMovto> getAllLocalMovtoCartaoSemPai(int cdEscola)
        {
            return DataAccessLocalMovto.getAllLocalMovtoCartaoSemPai(cdEscola);
        }

        public IEnumerable<LocalMovto> getAllLocalMovtoCartaoComPai(int cdEscola)
        {
            return DataAccessLocalMovto.getAllLocalMovtoCartaoComPai(cdEscola);
        }

        public IEnumerable<LocalMovto> getAllLocalMovtoTipoCartao(int cdEscola, int cd_tipo_liquidacao, int cd_local_movto, int cd_pessoa_usuario)
        {
            return DataAccessLocalMovto.getAllLocalMovtoTipoCartao(cdEscola, cd_tipo_liquidacao, cd_local_movto, cd_pessoa_usuario);
        }

        public IEnumerable<LocalMovto> getAllLocalMovtoCheque(int cdEscola)
        {
            return DataAccessLocalMovto.getAllLocalMovtoCheque(cdEscola);
        }
        #endregion

        #region Transação Financeira

        public TransacaoFinanceira postIncluirTransacao(TransacaoFinanceira transacao, bool baixarBolsaAutomatica)
        {
            return postIncluirTransacao(transacao, baixarBolsaAutomatica, false, null);
        }

        public TransacaoFinanceira postIncluirTransacao(TransacaoFinanceira transacao, bool baixarBolsaAutomatica, bool eh_cnab, Parametro parametros)
        {
            if (parametros == null)
            {
                parametros = DataAccessPoliticaTurma.getParametrosEscola(transacao.cd_pessoa_empresa);
            }

            TransacaoFinanceira transacaoFinanceira = new TransacaoFinanceira();
            if (transacao.cd_tipo_liquidacao.HasValue && transacao.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.CANCELAMENTO && (transacao.cd_local_movto == null || transacao.cd_local_movto == 0))
                throw new FinanceiroBusinessException(string.Format(Utils.Messages.Messages.msgNotExistLocalMvtoEscola),
                    null, FinanceiroBusinessException.TipoErro.ERRO_ESCOLA_NOT_EXIST_LOCALMOVTO, false);
            if ((transacao.dt_tran_finan.HasValue && transacao.dt_tran_finan != null && DateTime.Compare((DateTime)transacao.dt_tran_finan, new DateTime(1900, 1, 1)) < 0))
                throw new FinanceiroBusinessException(Messages.msgInfoDataMinValueTransacao, null, FinanceiroBusinessException.TipoErro.ERRO_MINDATE_SMALLDATETIME, false);
            if (!baixarBolsaAutomatica && transacao.cd_tipo_liquidacao.HasValue && transacao.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA &&
                verificarTitulosBaixaBolsaContrato(transacao.Baixas.ToList(), transacao.cd_pessoa_empresa,false))
                throw new FinanceiroBusinessException(string.Format(Utils.Messages.Messages.msgBaixaMotivoBolsaContratoSemAjusteManual),
                    null, FinanceiroBusinessException.TipoErro.ERRO_BAIXA_BOLSA_CONTRATO, false);
            if (!baixarBolsaAutomatica && transacao.cd_tipo_liquidacao.HasValue && transacao.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA_ADITIVO &&
                verificarTitulosBaixaBolsaContrato(transacao.Baixas.ToList(), transacao.cd_pessoa_empresa, true))
                throw new FinanceiroBusinessException(string.Format(Utils.Messages.Messages.msgBaixaAditivoBolsaContratoSemAjusteManual),
                    null, FinanceiroBusinessException.TipoErro.ERRO_BAIXA_BOLSA_CONTRATO, false);
            this.sincronizarContextos(DataAccessTitulo.DB());
            DateTime data = new DateTime();
            DateTime dataCorrente = DateTime.Now.Date;
            if (transacao.dt_tran_finan.HasValue)
            {
                data = ((DateTime)transacao.dt_tran_finan).Date;
                transacao.dt_tran_finan = data;
            }
            MovimentarContaReatroativa(transacao.isSupervisor, transacao.movimentoRetroativo, data, dataCorrente);
            int nm_recibo =  DataAccessBaixaFinan.getUltimoNroRecibo(null, transacao.cd_pessoa_empresa);

            if (transacao.Baixas != null && transacao.id_liquidacao_tit_ant_aberto)
                verificaSeExisteTituloAnteriorAberto(transacao.Baixas.ToList(), transacao.cd_pessoa_empresa);

            Hashtable hashResponsaveis = new Hashtable();
            if (transacao.Baixas != null)
                foreach (BaixaTitulo baixa in transacao.Baixas)
                {
                    if (baixa == null || baixa.Titulo == null)
                        throw new Exception(Messages.msgBaixaSemTitulo, null);
                    hashResponsaveis[baixa.Titulo.cd_titulo] = baixa.Titulo.nomeResponsavel;
                    baixa.cd_local_movto = transacao.cd_local_movto;
                    baixa.cd_tipo_liquidacao = transacao.cd_tipo_liquidacao;
                    baixa.Titulo = DataAccessTitulo.findById(baixa.cd_titulo, false);
                    if (baixa.Titulo == null)
                        throw new FinanceiroBusinessException(Messages.msgErroTituloNaoEncontrado, null, FinanceiroBusinessException.TipoErro.ERRO_CALCULO_APLICAR_BAIXA_TITULO, false);
                    if (baixa.Titulo.id_status_cnab == (int)Titulo.StatusCnabTitulo.ENVIADO_GERADO || baixa.Titulo.id_status_cnab == (int)Titulo.StatusCnabTitulo.CONFIRMADO_ENVIO)
                        if (!eh_cnab && DataAccessLocalMovto.verificaLocalTituloTemCNAB(baixa.Titulo.cd_local_movto, (int)baixa.Titulo.cd_pessoa_empresa))
                            baixa.Titulo.id_status_cnab = (int)Titulo.StatusCnabTitulo.BAIXA_MANUAL;

                    if (baixa.dt_baixa_titulo != null)
                    {
                        baixa.dt_baixa_titulo = baixa.dt_baixa_titulo.Date;
                        //Atualiza o saldo do título:
                        baixa.Titulo.dt_liquidacao_titulo = baixa.dt_baixa_titulo.Date;
                    }
                    if (baixa.id_baixa_parcial)
                    {
                        baixa.Titulo.vl_saldo_titulo -= (baixa.vl_principal_baixa - baixa.vl_desconto_baixa);
                        baixa.vl_desconto_baixa_calculado = baixa.vl_desconto_baixa;
                        baixa.vl_desconto_baixa = 0;
                    }
                    else
                    {
                        //Caso de tratamento de segurança - Verifica se por acaso o pagamento veio menor que o saldo do título e não foi informado o deconto, logo o título não foi pago inteiramente, está sendo feito uma baixa parcial.
                        if ((baixa.vl_desconto_baixa > 0 && decimal.Round((baixa.vl_desconto_baixa + baixa.vl_liquidacao_baixa), 2) < baixa.Titulo.vl_saldo_titulo) ||
                           (baixa.vl_desconto_baixa <= 0 && baixa.vl_liquidacao_baixa < baixa.Titulo.vl_saldo_titulo))
                        {
                            if (baixa.vl_desconto_baixa > 0)
                                throw new FinanceiroBusinessException(Messages.msgErroCalculosBaixaTitulo, null, FinanceiroBusinessException.TipoErro.ERRO_CALCULO_APLICAR_BAIXA_TITULO, false);
                            baixa.id_baixa_parcial = true;
                            baixa.vl_desconto_baixa_calculado = baixa.vl_principal_baixa - baixa.vl_liquidacao_baixa;
                            baixa.vl_desconto_baixa = 0;
                            baixa.Titulo.vl_saldo_titulo -= baixa.vl_liquidacao_baixa;
                        }
                        else //Caso normal, esperado que venha do front-end:
                        {
                            baixa.Titulo.vl_desconto_titulo += baixa.vl_desconto_baixa;
                            baixa.Titulo.vl_saldo_titulo -= baixa.vl_principal_baixa;
                        }
                    }
                    baixa.Titulo.vl_liquidacao_titulo += baixa.vl_principal_baixa - baixa.Titulo.vl_saldo_titulo;
                    baixa.vl_baixa_saldo_titulo = baixa.vl_principal_baixa - baixa.Titulo.vl_saldo_titulo;
                    baixa.Titulo.vl_multa_titulo += baixa.vl_multa_calculada;
                    baixa.Titulo.vl_juros_titulo += baixa.vl_juros_calculado;
                    //baixa.Titulo.vl_liquidacao_titulo += baixa.vl_liquidacao_baixa;
                    baixa.Titulo.vl_multa_liquidada += baixa.vl_multa_baixa;
                    baixa.Titulo.vl_juros_liquidado += baixa.vl_juros_baixa;
                    baixa.Titulo.vl_desconto_juros += baixa.vl_desc_juros_baixa;
                    baixa.Titulo.vl_desconto_multa += baixa.vl_desc_multa_baixa;
                    //se tiver valor na taxa do cartao, respeita o valor que veio
                    if (baixa.vl_taxa_cartao == 0)
                    {
                        baixa.vl_taxa_cartao = (baixa.vl_liquidacao_baixa * (decimal)baixa.Titulo.pc_taxa_cartao) / 100;
                    }
                    

                    //Arredondando os valores para gravar na base de dados
                    baixa.Titulo.vl_saldo_titulo = Decimal.Round(baixa.Titulo.vl_saldo_titulo, 2);
                    baixa.Titulo.vl_liquidacao_titulo = Decimal.Round(baixa.Titulo.vl_liquidacao_titulo, 2);
                    baixa.vl_baixa_saldo_titulo = Decimal.Round(baixa.vl_baixa_saldo_titulo, 2);
                    baixa.vl_liquidacao_baixa = Decimal.Round(baixa.vl_liquidacao_baixa, 2);
                    baixa.vl_taxa_cartao = Decimal.Round(baixa.vl_taxa_cartao, 2, MidpointRounding.AwayFromZero);

                    //Verifica se algum título já se encontra fechado e o atualiza:
                    //if (baixa.Titulo.vl_saldo_titulo == 0 && baixa.Titulo.vl_juros_titulo == baixa.Titulo.vl_juros_liquidado && baixa.Titulo.vl_multa_titulo == baixa.Titulo.vl_multa_liquidada)
                    if (baixa.Titulo.vl_saldo_titulo == 0 &&
                       (baixa.Titulo.vl_juros_titulo + baixa.Titulo.vl_multa_titulo) <= (baixa.Titulo.vl_juros_liquidado + baixa.Titulo.vl_multa_liquidada))
                        baixa.Titulo.id_status_titulo = (int)Titulo.StatusTitulo.FECHADO;
                    else
                        baixa.Titulo.id_status_titulo = (int)Titulo.StatusTitulo.ABERTO;

                    if (baixa.Titulo.vl_saldo_titulo < 0)
                        throw new FinanceiroBusinessException(Messages.msgErroSaldoNegativo, null, FinanceiroBusinessException.TipoErro.ERRO_TITULO_SALDO_NEGATIVO, false);
                    nm_recibo++;
                    baixa.nm_recibo = nm_recibo;

                    if ((baixa.vl_desconto_baixa == 0 && !baixa.cd_politica_desconto.HasValue) || baixa.id_baixa_parcial)
                        baixa.cd_politica_desconto = null;

                    DataAccessTitulo.edit(baixa.Titulo, false);
                    baixa.Titulo = null;

                }

            transacaoFinanceira = DataAccessTransacaoFinanceira.add(transacao, false);
            if (transacao.Baixas != null && (transacaoFinanceira.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.CANCELAMENTO &&
                                             transacaoFinanceira.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA &&
                                             transacaoFinanceira.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA_ADITIVO &&
                                             transacaoFinanceira.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.DESCONTO_FOLHA_PAGAMENTO))
            {
                List<BaixaTitulo> baixaList = transacao.Baixas.ToList();
                for (int i = 0; i < baixaList.Count; i++)
                {
                    BaixaTitulo baixa = baixaList[i];
                    if (baixa.Titulo != null)
                    {
                        string responsavel = hashResponsaveis[baixa.Titulo.cd_titulo] + "";

                        if (transacao.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.TROCA_FINANCEIRA)
                        {
                            gerarContaCorrente(baixa, responsavel, transacao, false, parametros);
                        }
                        else
                        {
                            DateTime? bom_para = (transacao.cheque != null ? transacao.cheque.dt_bom_para: null);
                            gerarTituloTroca(baixaList[i].Titulo, baixa.dt_baixa_titulo, baixa.vl_liquidacao_baixa, transacao.cd_tipo_liquidacao_old,transacaoFinanceira.cd_tran_finan, bom_para, transacao.cd_local_movto);
                        }
                    }
                }
            }
            IEnumerable<Titulo> titulosBaixa = getTitulosByTransacaoFinanceira(transacaoFinanceira.cd_pessoa_empresa, transacaoFinanceira.cd_tran_finan);

            if (titulosBaixa != null)
                transacaoFinanceira.titulosBaixa = titulosBaixa.ToList();
            return transacaoFinanceira;
        }

        private void gerarTituloTroca(Titulo titulo, DateTime dt_baixa_titulo, decimal vl_liquidacao_baixa, int cd_tipo_liquidacao_old, int cd_tran_finan, DateTime? dt_bom_para, int? cd_local_movto)
        {
            int TRANSACAO_FINANCEIRA = 129;
            Titulo titulotroca = new Titulo();
            titulotroca.Cheque = titulo.Cheque;
            titulotroca.id_origem_titulo = TRANSACAO_FINANCEIRA;
            titulotroca.cd_origem_titulo = cd_tran_finan;
            titulotroca.cd_pessoa_empresa = titulo.cd_pessoa_empresa;
            titulotroca.cd_pessoa_titulo = titulo.cd_pessoa_titulo;
            titulotroca.dh_cadastro_titulo = DateTime.Now.ToUniversalTime();
            titulotroca.cd_pessoa_responsavel = titulo.cd_pessoa_responsavel;
            titulotroca.dt_emissao_titulo = dt_baixa_titulo;
            titulotroca.dt_vcto_titulo = dt_baixa_titulo;
            titulotroca.nm_titulo = titulo.nm_titulo;
            titulotroca.nm_parcela_titulo = titulo.nm_parcela_titulo;
            titulotroca.id_natureza_titulo = titulo.id_natureza_titulo;
            titulotroca.dc_tipo_titulo = titulo.dc_tipo_titulo;
            titulotroca.vl_titulo = vl_liquidacao_baixa;
            titulotroca.vl_saldo_titulo = vl_liquidacao_baixa;
            titulotroca.id_status_titulo = 1;
            if (cd_tipo_liquidacao_old == (int)TipoLiquidacao.TipoLiqui.CHEQUE_PRE_DATADO || cd_tipo_liquidacao_old == (int)TipoLiquidacao.TipoLiqui.CHEQUE_AVISTA){
                titulotroca.dt_vcto_titulo = (DateTime)dt_bom_para;
                titulotroca.cd_tipo_financeiro = (int)TipoFinanceiro.TiposFinanceiro.CHEQUE;
                titulotroca.cd_local_movto = (int)cd_local_movto;
            }
            if (cd_tipo_liquidacao_old == (int)TipoLiquidacao.TipoLiqui.CARTAO_CREDITO || cd_tipo_liquidacao_old == (int)TipoLiquidacao.TipoLiqui.CARTAO_DEBITO){
                titulotroca.cd_tipo_financeiro = (int)TipoFinanceiro.TiposFinanceiro.CARTAO;
                titulotroca.cd_local_movto = DataAccessLocalMovto.findByLocal(cd_local_movto,  cd_tipo_liquidacao_old);

                DateTime dataVencimentoTituloCartao = dt_baixa_titulo;

                aplicarTaxaBancaria( titulotroca, 1, ref dataVencimentoTituloCartao);
               
            }

            titulotroca = DataAccessTitulo.add(titulotroca, false);
            // A trigger de insert vai ratear o plano titulo
        }



        public void aplicarTaxaBancaria(Titulo objTitulo, int nm_parcelas_mensalidade, ref DateTime dataVencimentoTituloCartao)
        {
            //List<LocalMovto> locaisMovtoCartao = new List<LocalMovto>();
            List<LocalMovto> locaisMovtoCartaoNmParcelasIguais = new List<LocalMovto>();
            List<LocalMovto> locaisMovtoCartaoNmParcelasDiferentes = new List<LocalMovto>();
            var taxaBancariaAplicar = new TaxaBancaria();

            var taxaBancariaAplicarNmParcelasIguais = new TaxaBancaria();
            var taxaBancariaAplicarNmParcelasDiferentes = new TaxaBancaria();

            var localMovtoAplicar = new LocalMovto();
            var local = new LocalMovtoUI();

            local = getLocalByTitulo(objTitulo.cd_pessoa_empresa, objTitulo.cd_local_movto);

            if (local != null && (local.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_CREDITO ||
                local.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_DEBITO))
            {
                taxaBancariaAplicar = local.taxaBancaria.Where(t => t.nm_parcela == nm_parcelas_mensalidade).OrderBy(x => x.cd_taxa_bancaria).FirstOrDefault();
                taxaBancariaAplicarNmParcelasDiferentes = local.taxaBancaria.Where(t => t.nm_parcela == nm_parcelas_mensalidade).OrderBy(x => x.cd_taxa_bancaria).FirstOrDefault();

                localMovtoAplicar.cd_local_movto = local.cd_local_movto;
                localMovtoAplicar.no_local_movto = local.no_local_movto;
                localMovtoAplicar.cd_pessoa_empresa = local.cd_pessoa_empresa;
                //Se não tiver taxa bancaria seta pc_taxa e nm_dia = 0
                if (local.taxaBancaria.Count() == 0)
                {
                    preencheLocalETaxaMovt(ref objTitulo, localMovtoAplicar, 0, 0, local.cd_local_movto, local.no_local_movto);
                    
                }
                else if (local.taxaBancaria.Count() > 0 && taxaBancariaAplicar == null) //Se tiver taxa bancaria e for diferente do nm_parcela, busca a maxima dos valores que são <= ao nm_parcela 
                {
                    var taxaslocalMenorIgualNmParcelas = local.taxaBancaria
                        .Where(t => t.nm_parcela <= nm_parcelas_mensalidade).OrderBy(x => x.cd_taxa_bancaria);
                    if (taxaslocalMenorIgualNmParcelas != null && taxaslocalMenorIgualNmParcelas.Count() > 0)
                    {
                        var valorlocalMaxMenorIgualNmParcelas = taxaslocalMenorIgualNmParcelas.Max(b => b.nm_parcela);
                        if (valorlocalMaxMenorIgualNmParcelas > 0)
                        {
                            taxaBancariaAplicar = local.taxaBancaria.Where(g => g.nm_parcela == valorlocalMaxMenorIgualNmParcelas).FirstOrDefault();
                        }
                        else
                        {
                            taxaBancariaAplicar = null;
                        }

                    }
                    else
                    {
                        taxaBancariaAplicar = null;
                    }

                    
                   
                    if (taxaBancariaAplicar == null) // Se nao encontrou a maxima entre os que são <=, busca a maxima geral
                    {

                        var taxasLocalNmParcelas = local.taxaBancaria
                            .Where(t => t.nm_parcela <= nm_parcelas_mensalidade).OrderBy(x => x.cd_taxa_bancaria);
                        if (taxasLocalNmParcelas != null && taxasLocalNmParcelas.Count() > 0)
                        {
                            var valorLocalMaxNmParcelas = taxasLocalNmParcelas.Max(b => b.nm_parcela);
                            if (valorLocalMaxNmParcelas > 0)
                            {
                                taxaBancariaAplicar = local.taxaBancaria.Where(g => g.nm_parcela == valorLocalMaxNmParcelas).OrderBy(x => x.cd_taxa_bancaria).FirstOrDefault();
                                preencheLocalETaxa(ref objTitulo, localMovtoAplicar, taxaBancariaAplicar.pc_taxa, taxaBancariaAplicar.nm_dias, local.cd_local_movto, local.no_local_movto, ref dataVencimentoTituloCartao);

                            }
                            else
                            {
                                taxaBancariaAplicar = null;
                            }
                        }
                        else
                        {
                            taxaBancariaAplicar = null;
                        }

                        
                    }
                    else // Seta a maxima entre os que são <=
                    {
                        preencheLocalETaxa(ref objTitulo, localMovtoAplicar, taxaBancariaAplicar.pc_taxa, taxaBancariaAplicar.nm_dias, local.cd_local_movto, local.no_local_movto, ref dataVencimentoTituloCartao);

                    }

                }
                else if (local.taxaBancaria.Count() > 0 && taxaBancariaAplicar != null) //Se tiver taxa bancaria e for igual seta a taxa que encontrou
                {
                    preencheLocalETaxa(ref objTitulo, localMovtoAplicar, taxaBancariaAplicar.pc_taxa, taxaBancariaAplicar.nm_dias, local.cd_local_movto, local.no_local_movto, ref dataVencimentoTituloCartao);
                   
                }

            }
            else
            { 
                localMovtoAplicar = getAllLocalMovtoCartaoSemPai(objTitulo.cd_pessoa_empresa).OrderBy(l => l.cd_local_movto).FirstOrDefault();
                if (localMovtoAplicar == null)
                {
                    throw new FinanceiroBusinessException(string.Format(Utils.Messages.Messages.msgErroNaoExisteLocalMovtoCartao), null,
                        FinanceiroBusinessException.TipoErro.ERRO_NAO_EXISTE_LOCALMOVTO_CARTAO, false);
                }

                taxaBancariaAplicar = localMovtoAplicar.TaxaBancaria.Where(t => t.nm_parcela == nm_parcelas_mensalidade)
                    .OrderBy(x => x.cd_taxa_bancaria).FirstOrDefault();

                //Se não tiver taxa bancaria seta pc_taxa e nm_dia = 0
                if (localMovtoAplicar.TaxaBancaria.Count() == 0)
                {
                    preencheLocalETaxaMovt(ref objTitulo, localMovtoAplicar, 0, 0, localMovtoAplicar.cd_local_movto, localMovtoAplicar.no_local_movto);
                    
                }
                else if (localMovtoAplicar.TaxaBancaria.Count() > 0 && taxaBancariaAplicar == null) //Se tiver taxa bancaria e for diferente do nm_parcela, busca a maxima dos valores que são <= ao nm_parcela 
                {
                    var taxasMenorIgualNmParcelas = localMovtoAplicar.TaxaBancaria
                        .Where(t => t.nm_parcela <= nm_parcelas_mensalidade).OrderBy(x => x.cd_taxa_bancaria);
                    if(taxasMenorIgualNmParcelas != null && taxasMenorIgualNmParcelas.Count() > 0)
                    {
                        var valorMaxMenorIgualNmParcelas = taxasMenorIgualNmParcelas.Max(b => b.nm_parcela);
                        if (valorMaxMenorIgualNmParcelas > 0)
                        {
                            taxaBancariaAplicar = localMovtoAplicar.TaxaBancaria.Where(g => g.nm_parcela == valorMaxMenorIgualNmParcelas).FirstOrDefault();

                        }
                        else
                        {
                            taxaBancariaAplicar = null;
                        }

                    }else
                    {
                        taxaBancariaAplicar = null;
                    }

                    
                    

                    if (taxaBancariaAplicar == null) // Se nao encontrou a taxa maxima entre os que são <=, busca a maxima geral
                    {
                        var taxasNmParcelas = localMovtoAplicar.TaxaBancaria
                            .Where(t => t.nm_parcela <= nm_parcelas_mensalidade).OrderBy(x => x.cd_taxa_bancaria);
                        if(taxasNmParcelas != null && taxasNmParcelas.Count() > 0)
                        {
                            var valorMaxNmParcelas = taxasNmParcelas.Max(b => b.nm_parcela);
                            if (valorMaxNmParcelas > 0)
                            {
                                taxaBancariaAplicar = localMovtoAplicar.TaxaBancaria.Where(g => g.nm_parcela == valorMaxNmParcelas).OrderBy(x => x.cd_taxa_bancaria).FirstOrDefault();
                                preencheLocalETaxa(ref objTitulo, localMovtoAplicar, taxaBancariaAplicar.pc_taxa, taxaBancariaAplicar.nm_dias, localMovtoAplicar.cd_local_movto, localMovtoAplicar.no_local_movto, ref dataVencimentoTituloCartao);
                            }
                            else
                            {
                                taxaBancariaAplicar = null;
                            }
                        }else
                        {
                            taxaBancariaAplicar = null;
                        }
                        
                    }
                    else // Seta a maxima entre os que são <=
                    {
                        preencheLocalETaxa(ref objTitulo, localMovtoAplicar, taxaBancariaAplicar.pc_taxa, taxaBancariaAplicar.nm_dias, localMovtoAplicar.cd_local_movto, localMovtoAplicar.no_local_movto, ref dataVencimentoTituloCartao);
                    }

                }
                else if (localMovtoAplicar.TaxaBancaria.Count() > 0 && taxaBancariaAplicar != null) //Se tiver taxa bancaria e for igual nm_parcela seta a taxa que encontrou
                {
                    preencheLocalETaxa(ref objTitulo, localMovtoAplicar, taxaBancariaAplicar.pc_taxa, taxaBancariaAplicar.nm_dias, localMovtoAplicar.cd_local_movto, localMovtoAplicar.no_local_movto, ref dataVencimentoTituloCartao);
                   
                }
                
            }

        }

        public Cheque getChequeTransacaoTrocaFinanceira(int cd_titulo, int cdEscola)
        {
            return DataAccessTitulo.getChequeTransacaoTrocaFinanceira(cd_titulo, cdEscola).FirstOrDefault();
             
        }

        public void alterarResponsavelTitulos(int cd_contrato, int cd_escola, List<Titulo> titulos)
        {
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED, DataAccessTitulo.DB()))
            {
                List<Titulo> titulosBd = DataAccessTitulo.getTitulosByContratoEscola(cd_contrato, cd_escola, titulos.Select(x=> x.cd_titulo).ToList());

                foreach(Titulo item in titulosBd)
                {
                    item.cd_pessoa_responsavel = titulos.Where(x => x.cd_titulo == item.cd_titulo).FirstOrDefault().cd_pessoa_responsavel;
                }

                DataAccessTitulo.saveChanges(false);
                transaction.Complete();

            }
        }

        public void alterarDtVctoTitulos(int cd_contrato, int cd_escola, List<Titulo> titulos)
        {
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED, DataAccessTitulo.DB()))
            {
                List<Titulo> titulosBd = DataAccessTitulo.getTitulosByContratoEscola(cd_contrato, cd_escola, titulos.Select(x => x.cd_titulo).ToList());

                foreach (Titulo item in titulosBd)
                {
                    item.dt_vcto_titulo = titulos.Where(x => x.cd_titulo == item.cd_titulo).FirstOrDefault().dt_vcto_titulo;
                }

                DataAccessTitulo.saveChanges(false);
                transaction.Complete();

            }
        }

        private void preencheLocalETaxa(ref Titulo objTitulo, LocalMovto localMovtoAplicar, double pc_taxa, byte nm_dias, int cd_local_movto,
            string no_local_movto, ref DateTime dataVencimentoTituloCartao)
        {
            localMovtoAplicar.cd_local_movto = localMovtoAplicar.cd_local_movto;
            objTitulo.pc_taxa_cartao = pc_taxa;
            objTitulo.nm_dias_cartao = nm_dias;
            objTitulo.cd_local_movto = cd_local_movto;
            objTitulo.descLocalMovto = no_local_movto;
            objTitulo.vl_taxa_cartao = Math.Round((decimal)objTitulo.pc_taxa_cartao * (objTitulo.vl_titulo / (decimal)100.0), 2, MidpointRounding.AwayFromZero);

            objTitulo.dt_vcto_titulo = dataVencimentoTituloCartao.AddDays(objTitulo.nm_dias_cartao);
            dataVencimentoTituloCartao = dataVencimentoTituloCartao.AddDays(objTitulo.nm_dias_cartao);
            

        }

         private void preencheLocalETaxaMovt(ref Titulo objTitulo, LocalMovto localMovtoAplicar, double pc_taxa, byte nm_dias, int cd_local_movto,
            string no_local_movto)
        {
            localMovtoAplicar.cd_local_movto = localMovtoAplicar.cd_local_movto;
            objTitulo.pc_taxa_cartao = pc_taxa;
            objTitulo.nm_dias_cartao = nm_dias;
            objTitulo.cd_local_movto = cd_local_movto;
            objTitulo.descLocalMovto = no_local_movto;
            objTitulo.vl_taxa_cartao = Math.Round((decimal)objTitulo.pc_taxa_cartao * (objTitulo.vl_titulo / (decimal)100.0), 2, MidpointRounding.AwayFromZero);
        }

        public TransacaoFinanceira editTransacao(TransacaoFinanceira transacao, bool aperacao_sistema)
        {
            return editTransacao(transacao, false, null);
        }

        public TransacaoFinanceira editTransacao(TransacaoFinanceira transacao, bool aperacao_sistema, Parametro parametros)
        {
            if (parametros == null)
            {
                parametros = DataAccessPoliticaTurma.getParametrosEscola(transacao.cd_pessoa_empresa);
            }

            ComponentesWebContext dbComp = new ComponentesWebContext();
            int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Contrato"].ToString());
            this.sincronizarContextos(DataAccessTitulo.DB());
            TransacaoFinanceira transContext = DataAccessTransacaoFinanceira.getTransacaoFinanceira(transacao.cd_tran_finan, transacao.cd_pessoa_empresa);
            List<BaixaTitulo> baixasTransContext;
            if (transContext.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.CHEQUE_AVISTA || transContext.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.CHEQUE_PRE_DATADO)
            {
                transContext.ChequeTransacaoFinanceira = DataAccessChequeTransacao.getChequeTrasacao(transacao.cd_tran_finan).ToList();
                baixasTransContext = DataAccessBaixaFinan.getBaixasTransacaoFinan(transacao.cd_tran_finan, 0, 0, transacao.cd_pessoa_empresa,
                                                                               BaixaTituloDataAccess.TipoConsultaBaixaEnum.HAS_BAIXA_TITULO_TRANS_FINAN_CHEQUE).ToList();
            }else
                baixasTransContext = DataAccessBaixaFinan.getBaixasTransacaoFinan(transacao.cd_tran_finan, 0, 0, transacao.cd_pessoa_empresa,
                                                                               BaixaTituloDataAccess.TipoConsultaBaixaEnum.HAS_BAIXA_TITULO_TRANS_FINAN).ToList();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED, DataAccessTitulo.DB()))
            {
                DateTime data = new DateTime();
                DateTime dataCorrente = DateTime.Now.Date;
                if (transacao.dt_tran_finan.HasValue)
                {
                    data = (DateTime)transacao.dt_tran_finan;
                    data = data.ToLocalTime().Date;
                    transacao.dt_tran_finan = data;
                }

                MovimentarContaReatroativa(transacao.isSupervisor, transacao.movimentoRetroativo, data, dataCorrente);
                IEnumerable<BaixaTitulo> baixasDeleted = baixasTransContext.Where(tc => !transacao.Baixas.Any(tv => tc.cd_baixa_titulo == tv.cd_baixa_titulo));
                if (baixasDeleted != null)
                    foreach (var itemDel in baixasDeleted)
                        if (itemDel != null)
                        {
                            Titulo tituloBacck = voltarEstadoAnteriorTitulo(itemDel, itemDel.cd_tran_finan, transacao.cd_pessoa_empresa, null);
                            if (!aperacao_sistema && itemDel.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA && (tituloBacck.id_origem_titulo !=  cd_origem ||
                                !DataAccessBaixaFinan.baixaMotivoBolsaContrato(itemDel.cd_baixa_titulo, transacao.cd_pessoa_empresa)))
                                throw new FinanceiroBusinessException(Messages.msgErroDeleteBaixaBolsaContrato, null, FinanceiroBusinessException.TipoErro.ERRO_BAIXA_BOLSA_CONTRATO, false);
                            if (!aperacao_sistema && transacao.cd_tipo_liquidacao.HasValue && transacao.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA_ADITIVO &&
                                 verificarTitulosBaixaBolsaContrato(transacao.Baixas.ToList(), transacao.cd_pessoa_empresa, true))
                                throw new FinanceiroBusinessException(string.Format(Utils.Messages.Messages.msgBaixaAditivoBolsaContratoSemAjusteManual),
                                    null, FinanceiroBusinessException.TipoErro.ERRO_BAIXA_BOLSA_CONTRATO, false);
                            DataAccessTitulo.saveChanges(false);
                            DataAccessBaixaFinan.delete(itemDel, false);
                        }
                foreach (BaixaTitulo bx in transacao.Baixas)
                {
                    bx.cd_local_movto = transacao.cd_local_movto;
                    bx.cd_tipo_liquidacao = transacao.cd_tipo_liquidacao;
                    if (transacao.dt_tran_finan != null)
                        bx.dt_baixa_titulo = ((DateTime)transacao.dt_tran_finan).ToLocalTime().Date;
                    BaixaTitulo bxContext = baixasTransContext.Where(hc => hc.cd_baixa_titulo == bx.cd_baixa_titulo).FirstOrDefault();
                    BaixaTitulo bxContextCopy = new BaixaTitulo();
                    if (bxContext != null && bxContext.cd_baixa_titulo > 0)
                    {
                        bxContextCopy.copy(bxContext);
                        bxContext = BaixaTitulo.changeValuesBaixaTitulo(bxContext, bx);
                    }
                    if (DataAccessBaixaFinan.DB().Entry(bxContext).State == System.Data.Entity.EntityState.Modified)
                    {
                        string noResponsavel = bx.Titulo.nomeResponsavel;
                        Titulo tituloBacck = voltarEstadoAnteriorTitulo(bxContextCopy, transacao.cd_tran_finan, transacao.cd_pessoa_empresa, null);
                        if (!aperacao_sistema && bxContext.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA && (tituloBacck.id_origem_titulo != cd_origem ||
                                !DataAccessBaixaFinan.baixaMotivoBolsaContrato(bxContext.cd_baixa_titulo, transacao.cd_pessoa_empresa)))
                            throw new FinanceiroBusinessException(Messages.msgErroDeleteBaixaBolsaContrato, null, FinanceiroBusinessException.TipoErro.ERRO_BAIXA_BOLSA_CONTRATO, false);

                        if (bxContext.vl_desconto_baixa == 0 || bxContext.id_baixa_parcial)
                            bxContext.cd_politica_desconto = null;

                        tituloBacck = aplicarBaixaTitulo(bxContext, tituloBacck);
                        DataAccessTitulo.saveChanges(false);
                        List<ContaCorrente> ccContext = DataAccessContaCorrente.getContaCorrenteByBaixa(bxContext.cd_baixa_titulo, transacao.cd_pessoa_empresa);
                        if (ccContext != null && ccContext.Count > 0)
                            foreach (var cc in ccContext)
                            {
                                DataAccessContaCorrente.delete(cc, false);
                            }
                        if (transacao.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.CANCELAMENTO &&
                            transacao.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA &&
                            transacao.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA_ADITIVO)
                        {
                            bxContext.Titulo = tituloBacck;
                            gerarContaCorrente(bxContext, noResponsavel, transacao, true, parametros);
                        }
                    }
                }
                DataAccessBaixaFinan.saveChanges(false);
                transContext = TransacaoFinanceira.changeValuesTransacaoFinanceira(transContext, transacao);
                DataAccessTransacaoFinanceira.saveChanges(false);
                //transacaoFinanceira = DataAccessTransacaoFinanceira.edit(transacao, false);
                //transContext = DataAccessTransacaoFinanceira.getTransacaoBaixa(transContext.cd_tran_finan, transContext.cd_pessoa_empresa);
                transContext.titulosBaixa = DataAccessTitulo.getTitulosByTransacaoFinanceira(transContext.cd_pessoa_empresa, transContext.cd_tran_finan).ToList();

                transaction.Complete();
            }
            return transContext;
        }

        private bool verificarTitulosBaixaBolsaContrato(List<BaixaTitulo> baixas, int cd_escola, bool aditivo)
        {
            bool retorno = false;
            if (baixas != null)
            {
                ComponentesWebContext dbComp = new ComponentesWebContext();
                int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                List<int> codTitulos = baixas.Where(x => x.Titulo.id_origem_titulo == cd_origem).Select(r => r.Titulo.cd_titulo).ToList();
                if (codTitulos != null && codTitulos.Count > 0)
                {
                    retorno = DataAccessTitulo.verificaTituloContratoAjusteManual(codTitulos, cd_escola, aditivo);
                }
                if (baixas.Any(x => x.Titulo.id_origem_titulo != cd_origem))
                    retorno = true;
            }
            return retorno;
        }

        private void verificaSeExisteTituloAnteriorAberto(List<BaixaTitulo> baixas, int cd_empresa)
        {
            baixas = baixas.Where(x => x.Titulo.id_natureza_titulo == (int)Titulo.NaturezaTitulo.RECEBER).ToList();
            if (baixas != null && baixas.Count() > 0)
            {
                List<Titulo> grupoTitulos = baixas.GroupBy(x => new { x.Titulo.nm_parcela_titulo, x.Titulo.cd_origem_titulo, x.Titulo.id_origem_titulo }
                                               ).Select(t => new Titulo
                                               {
                                                   nm_parcela_titulo = t.Key.nm_parcela_titulo,
                                                   cd_origem_titulo = t.Key.cd_origem_titulo,
                                                   id_origem_titulo = t.Key.id_origem_titulo
                                               }).ToList();
                if (grupoTitulos != null && grupoTitulos.Count() > 0)
                {
                    List<int> cdTitulos = new List<int>();
                    foreach (Titulo t in grupoTitulos)
                    {
                        List<BaixaTitulo> baixasAgrup = baixas.Where(x => x.Titulo.cd_origem_titulo == t.cd_origem_titulo && x.Titulo.id_origem_titulo == t.id_origem_titulo).ToList();
                        if (baixasAgrup != null && baixasAgrup.Count() > 0)
                            foreach (BaixaTitulo b in baixasAgrup)
                                cdTitulos.Add(b.Titulo.cd_titulo);
                        if (cdTitulos.Count() > 0)
                            if (DataAccessTitulo.verificarSeExisteTitulosAnterioresAberto(cdTitulos, cd_empresa))
                                throw new FinanceiroBusinessException(string.Format(Utils.Messages.Messages.msgErroTituloAnteriorAberto), null,
                           FinanceiroBusinessException.TipoErro.ERRO_TITULO_ANTERIOR_ABERTO, false);
                    }
                }
            }
        }

        public TransacaoFinanceira getTransacaoBaixaTitulo(int cd_titulo, int cd_pessoa_empresa)
        {
            return DataAccessTransacaoFinanceira.getTransacaoBaixaTitulo(cd_titulo, cd_pessoa_empresa);
        }

        public TransacaoFinanceira getTransacaoFinanceira(int cd_tran_finan, int cd_pessoa_empresa)
        {
            return DataAccessTransacaoFinanceira.getTransacaoFinanceira(cd_tran_finan, cd_pessoa_empresa);
        }

        public bool deleteTransFinanBaixa(TransacaoFinanceira transFinan)
        {
            ComponentesWebContext dbComp = new ComponentesWebContext();
            int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Contrato"].ToString());
            this.sincronizarContextos(DataAccessTitulo.DB());
            TransacaoFinanceira transContext = DataAccessTransacaoFinanceira.getTransacaoFinanceira(transFinan.cd_tran_finan, transFinan.cd_pessoa_empresa);
            List<BaixaTitulo> baixasTransContext = DataAccessBaixaFinan.getBaixasTransacaoFinan(transFinan.cd_tran_finan, 0, 0, transFinan.cd_pessoa_empresa,
                                                                                     BaixaTituloDataAccess.TipoConsultaBaixaEnum.HAS_BAIXA_TITULO_TRANS_FINAN).ToList();
            bool retorno = true;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED, DataAccessTitulo.DB(), TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                DateTime data = new DateTime();
                DateTime dataCorrente = DateTime.Now.Date;
                if (transContext != null)
                {
                    if (transContext.dt_tran_finan.HasValue)
                    {
                        data = (DateTime)transContext.dt_tran_finan;
                        data = data.Date;
                        transContext.dt_tran_finan = data;
                    }

                    #region Deleta titulo de troca financeira
                        List<Titulo> titulosTrocaFinanceira = new List<Titulo>();
                        titulosTrocaFinanceira = DataAccessTitulo.getTitulosbyTranFinan(transFinan.cd_pessoa_empresa, transFinan.cd_tran_finan).ToList();

                        foreach (Titulo t in titulosTrocaFinanceira)
                        {
                            int qtdBaixas = DataAccessTitulo.getBaixasByCdTitulo(transFinan.cd_pessoa_empresa, t.cd_titulo, transFinan.cd_tran_finan).Count();
                            if (qtdBaixas > 0)
                            {
                                throw new FinanceiroBusinessException(string.Format(Messages.msgErroTituloTrocaFinanceiraComBaixa, t.nm_titulo, t.nm_parcela_titulo, t.dc_tipo_titulo, t.dt_vcto_titulo.Date), null, FinanceiroBusinessException.TipoErro.ERRO_EXISTE_BAIXA_TITULO_TROCA_FINANCEIRA, false);
                            }
                        }

                        if (titulosTrocaFinanceira != null && titulosTrocaFinanceira.Count() > 0)
                        {
                            for (int i = titulosTrocaFinanceira.Count() - 1; i >= 0; i--)
                            {
                                DataAccessTitulo.delete(titulosTrocaFinanceira[i], false);
                                
                            }
                        }
                    #endregion

                    MovimentarContaReatroativa(transFinan.isSupervisor, transFinan.movimentoRetroativo, data, dataCorrente);

                    foreach (BaixaTitulo bt in baixasTransContext)
                    {
                        Titulo titulo = voltarEstadoAnteriorTitulo(bt, transContext.cd_tran_finan, transContext.cd_pessoa_empresa, null);
                        if (bt.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA && (titulo.id_origem_titulo != cd_origem ||
                               !DataAccessBaixaFinan.baixaMotivoBolsaContrato(bt.cd_baixa_titulo, transFinan.cd_pessoa_empresa)))
                            throw new FinanceiroBusinessException(Messages.msgErroDeleteBaixaBolsaContrato, null, FinanceiroBusinessException.TipoErro.ERRO_BAIXA_BOLSA_CONTRATO, false);
                        if (titulo.vl_saldo_titulo < 0)
                            throw new FinanceiroBusinessException(Messages.msgErroSaldoNegativo, null, FinanceiroBusinessException.TipoErro.ERRO_TITULO_SALDO_NEGATIVO, false);

                        DataAccessTitulo.edit(titulo, false);

                        if (bt.Titulo.Cheque != null){
                            if (bt.Titulo.dc_tipo_titulo == "AD"){
                                Cheque cheque = new Cheque();
                                cheque = getChequeByContrato((int)bt.Titulo.cd_origem_titulo);
                                if (cheque != null && cheque.cd_cheque > 0)
                                {
                                    deleteCheque(cheque);
                                }
                            }
                        }
                    }
                    retorno = DataAccessTransacaoFinanceira.delete(transContext, false);
                }
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<TipoLiquidacao> getTipoLiquidacao()
        {
            IEnumerable<TipoLiquidacao> retorno = new List<TipoLiquidacao>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = DataAccessTipoLiquidacao.getTipoLiquidacao().ToList();
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<TipoLiquidacao> getTipoLiquidacaoCd(int? cdTipoLiq)
        {
            return DataAccessTipoLiquidacao.getTipoLiquidacaoCd(cdTipoLiq);
        }

        private void gerarContaCorrente(BaixaTitulo baixa, string responsavel, TransacaoFinanceira transacao, bool edicao, Parametro parametros = null)
        {

            //Pega os planos de títulos do título para criação de conta corrente:
            List<PlanoTitulo> listaPlano = DataAccessPlanoTitulo.getPlanoTituloByTitulo(baixa.Titulo.cd_titulo, transacao.cd_pessoa_empresa).ToList();
            decimal resto_arrendodamento = 0;
            if (listaPlano != null && listaPlano.Count > 0)
                //Gera o registro na conta corrente para cada plano de titulo:
                for (int j = listaPlano.Count - 1; j >= 0; j--)
                    criaContaCorrente(baixa, responsavel, transacao, edicao, listaPlano[j], ref resto_arrendodamento, j == 0);
            else
                criaContaCorrente(baixa, responsavel, transacao, edicao, null, ref resto_arrendodamento, false);
            criaContaCorrenteSaidaTaxaCartao(baixa, responsavel, transacao, parametros);
            if (baixa.Titulo.vl_material_titulo > 0)
                criaContaCorrenteMaterial(baixa, responsavel, transacao, parametros);
        }

        private void criaContaCorrente(BaixaTitulo baixa, string responsavel, TransacaoFinanceira transacao, bool edicao, PlanoTitulo plano, ref decimal resto_arrendodamento, bool ultimo)
        {
            ContaCorrente contCorrente = new ContaCorrente();

            contCorrente.cd_baixa_titulo = baixa.cd_baixa_titulo;
            contCorrente.cd_local_origem = (int)transacao.cd_local_movto;
            if (baixa.cd_tipo_liquidacao.HasValue)
                contCorrente.cd_tipo_liquidacao = baixa.cd_tipo_liquidacao.Value;
            if (plano != null && baixa.Titulo.vl_titulo != 0)
            {
                decimal valor_outras = DataAccessTitulo.getValorBaixasOutras(baixa.cd_titulo);
                //valor_outras = valor_outras == null ? 0 : valor_outras;
                decimal vl_material = (baixa.Titulo.vl_titulo - (decimal)valor_outras) == 0 ? 0 : Decimal.Round(baixa.Titulo.vl_material_titulo * (baixa.vl_baixa_saldo_titulo / (baixa.Titulo.vl_titulo - (decimal)valor_outras)), 2);
                decimal valor_real = (decimal)((baixa.vl_liquidacao_baixa - vl_material) * plano.vl_plano_titulo / baixa.Titulo.vl_titulo);
                contCorrente.vl_conta_corrente = Math.Abs(valor_real);
                if (ultimo)
                    contCorrente.vl_conta_corrente += Math.Abs((decimal)resto_arrendodamento);
                else
                    resto_arrendodamento = valor_real - Math.Abs(valor_real);
                contCorrente.vl_conta_corrente = Decimal.Round((decimal)contCorrente.vl_conta_corrente, 2);
            }
            else
                contCorrente.vl_conta_corrente = baixa.vl_liquidacao_baixa;
            if (baixa.Titulo.id_natureza_titulo == (int)Titulo.NaturezaTitulo.PAGAR)
            {
                contCorrente.cd_movimentacao_financeira = (int)MovimentacaoFinanceira.TipoMovimentacaoFinanceira.PAGAMENTO;
                contCorrente.id_tipo_movimento = (int)ContaCorrente.Tipo.SAIDA;
                contCorrente.dc_obs_conta_corrente = "Pagamento do título Nº:" + baixa.Titulo.nm_titulo + "-" + baixa.Titulo.nm_parcela_titulo + ", Recibo Nº" + baixa.nm_recibo +
                ", vcto.:" + String.Format("{0:dd/MM/yyyy}", baixa.Titulo.dt_vcto_titulo) + " - " + responsavel + ".";
                //Contas a Pagar: Pagamento do Título Nº:2-0 Vcto:1/3/2010 - DANIEL FERNANDES PINTO FERREIRA .
            }
            else
            {
                contCorrente.cd_movimentacao_financeira = (int)MovimentacaoFinanceira.TipoMovimentacaoFinanceira.RECEBIMENTO;
                contCorrente.id_tipo_movimento = (int)ContaCorrente.Tipo.ENTRADA;
                contCorrente.dc_obs_conta_corrente = "Recebimento do título Nº:" + baixa.Titulo.nm_titulo + "-" + baixa.Titulo.nm_parcela_titulo  +", Recibo Nº" + baixa.nm_recibo +
                ", vcto.:" + String.Format("{0:dd/MM/yyyy}", baixa.Titulo.dt_vcto_titulo) + " - " + responsavel + ".";
            }
            contCorrente.cd_pessoa_empresa = transacao.cd_pessoa_empresa;
            if (plano != null)
                contCorrente.cd_plano_conta = plano.cd_plano_conta;
            contCorrente.dta_conta_corrente = baixa.dt_baixa_titulo.Date;
            contCorrente.cd_baixa_titulo = baixa.cd_baixa_titulo;
            if(contCorrente.vl_conta_corrente > 0)
                DataAccessContaCorrente.add(contCorrente, false);
        }

        private void criaContaCorrenteSaidaTaxaCartao(BaixaTitulo baixa, string responsavel, TransacaoFinanceira transacao, Parametro parametros)
        {
            if (baixa.Titulo.cd_tipo_financeiro == (int)TipoFinanceiro.TiposFinanceiro.CARTAO && baixa.vl_taxa_cartao > 0)//Somente gerar para baixas que possuem taxa de cartão
            {
                ContaCorrente contCorrente = new ContaCorrente();
                if (parametros == null)
                    parametros = new Parametro();

                contCorrente.cd_baixa_titulo = baixa.cd_baixa_titulo;
                contCorrente.cd_local_origem = (int)transacao.cd_local_movto;
                if (baixa.cd_tipo_liquidacao.HasValue)
                    contCorrente.cd_tipo_liquidacao = baixa.cd_tipo_liquidacao.Value;
                
                contCorrente.vl_conta_corrente = baixa.vl_taxa_cartao;
                contCorrente.vl_conta_corrente = Decimal.Round((decimal)contCorrente.vl_conta_corrente, 2);

                var localMovtoTitulo = DataAccessLocalMovto.findById(baixa.Titulo.cd_local_movto, false);

                var dc_tipo_liquidacao = localMovtoTitulo.nm_tipo_local ==
                    (byte)FundacaoFisk.SGF.GenericModel.LocalMovto.TipoLocalMovtoEnum.CARTAO_CREDITO ? "Cartão de Crédito" :
                    "Cartão de Débito";

                contCorrente.cd_movimentacao_financeira = (int)MovimentacaoFinanceira.TipoMovimentacaoFinanceira.PAGAMENTO;
                contCorrente.id_tipo_movimento = (int)ContaCorrente.Tipo.SAIDA;
                contCorrente.dc_obs_conta_corrente = "Taxa de " + baixa.Titulo.pc_taxa_cartao + "%" +
                    " referente a " + dc_tipo_liquidacao;

                contCorrente.cd_pessoa_empresa = transacao.cd_pessoa_empresa;

                //OBTER PLANO CONTA - TAXA BANCÁRIA
                if (parametros.cd_plano_conta_taxbco == null)
                    throw new FinanceiroBusinessException(Messages.msgErroPlanoTaxBancariaNull, null, FinanceiroBusinessException.TipoErro.ERRO_PLANO_TAXBANC_NULL, false);
                
                contCorrente.cd_plano_conta = parametros.cd_plano_conta_taxbco;
                contCorrente.dta_conta_corrente = baixa.dt_baixa_titulo.Date;
                contCorrente.cd_baixa_titulo = baixa.cd_baixa_titulo;
                if (contCorrente.vl_conta_corrente > 0)
                    DataAccessContaCorrente.add(contCorrente, false);
            }
        }
        private void criaContaCorrenteMaterial(BaixaTitulo baixa, string responsavel, TransacaoFinanceira transacao, Parametro parametros)
        {
            if (parametros != null && parametros.cd_plano_conta_material != null)
            {
                ContaCorrente contCorrente = new ContaCorrente();
                if (parametros == null)
                    parametros = new Parametro();

                contCorrente.cd_baixa_titulo = baixa.cd_baixa_titulo;
                contCorrente.cd_local_origem = (int)transacao.cd_local_movto;
                if (baixa.cd_tipo_liquidacao.HasValue)
                    contCorrente.cd_tipo_liquidacao = baixa.cd_tipo_liquidacao.Value;

                decimal valor_outras = DataAccessTitulo.getValorBaixasOutras(baixa.cd_titulo);
                //valor_outras = valor_outras == null ? 0 : valor_outras;
                contCorrente.vl_conta_corrente = (baixa.Titulo.vl_titulo - (decimal)valor_outras) == 0 ? 0 : Decimal.Round(baixa.Titulo.vl_material_titulo * (baixa.vl_baixa_saldo_titulo / (baixa.Titulo.vl_titulo - (decimal)valor_outras)), 2);
                var localMovtoTitulo = DataAccessLocalMovto.findById(baixa.Titulo.cd_local_movto, false);

                contCorrente.cd_movimentacao_financeira = (int)MovimentacaoFinanceira.TipoMovimentacaoFinanceira.RECEBIMENTO;
                contCorrente.id_tipo_movimento = (int)ContaCorrente.Tipo.ENTRADA;
                contCorrente.dc_obs_conta_corrente = "Recebimento referente ao material do título Nº:" + baixa.Titulo.nm_titulo + "-" + baixa.Titulo.nm_parcela_titulo + ", Recibo Nº" + baixa.nm_recibo +
                ", vcto.:" + String.Format("{0:dd/MM/yyyy}", baixa.Titulo.dt_vcto_titulo) + " - " + responsavel + ".";

                contCorrente.cd_pessoa_empresa = transacao.cd_pessoa_empresa;

                contCorrente.cd_plano_conta = parametros.cd_plano_conta_material;
                contCorrente.dta_conta_corrente = baixa.dt_baixa_titulo.Date;
                contCorrente.cd_baixa_titulo = baixa.cd_baixa_titulo;
                if (contCorrente.vl_conta_corrente > 0)
                    DataAccessContaCorrente.add(contCorrente, false);

            }
            else
                throw new FinanceiroBusinessException(Messages.msgErroPlanoTaxBancariaNull, null, FinanceiroBusinessException.TipoErro.ERRO_PLANO_MATERIAL_NULL, false);
        }
        public IEnumerable<Titulo> getTitulosByTransacaoFinanceira(int cd_pessoa_empresa, int cd_tran_finan)
        {
            IEnumerable<Titulo> titulos = new List<Titulo>();
            titulos = DataAccessTitulo.getTitulosByTransacaoFinanceira(cd_pessoa_empresa, cd_tran_finan);
            return titulos;
        }

        #endregion

        #region Plano Titulo
        public PlanoTitulo getPlanoTituloByTitulo(int cdTitulo, int cdPlanoConta, int cdEscola)
        {
            return DataAccessPlanoTitulo.getPlanoTituloByTitulo(cdTitulo, cdPlanoConta, cdEscola);
        }

        public bool deletePlanoTitulo(PlanoTitulo planoTitulo)
        {
            bool deleted = false;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                deleted = DataAccessPlanoTitulo.delete(planoTitulo, false);
                transaction.Complete();
                return deleted;
            }
        }
        #endregion

        #region Política Comercial

        public PoliticaComercial getPoliticaComercialById(int cdPoliticaComercial, int cdEscola)
        {
            return DataAccessPoliticaComercial.getPoliticaComercialById(cdPoliticaComercial, cdEscola);
        }

        public bool deletePoliticaComercial(PoliticaComercial polComercial)
        {
            return DataAccessPoliticaComercial.delete(polComercial, false);
        }

        public PoliticaComercial addPoliticaComercial(PoliticaComercial polComercial)
        {
            if (!polComercial.id_parcela_igual)
            {
                //Se não for vencimento fixo, o número de dias não pode ser null
                if (!polComercial.id_vencimento_fixo)
                    foreach (ItemPolitica p in polComercial.ItemPolitica)
                        if (p.nm_dias_politica == null)
                            throw new FinanceiroBusinessException(string.Format(Utils.Messages.Messages.msgErroNrDiasPolComNull), null, FinanceiroBusinessException.TipoErro.ERRO_DIA_POL_COMERCIAL_NULL, false);

                double? sumPolCom = 0;
                if (polComercial.ItemPolitica != null && polComercial.ItemPolitica.Count() > 0)
                {
                    sumPolCom = polComercial.ItemPolitica.Sum(i => i.pc_politica);
                    if (sumPolCom != 100)
                        throw new FinanceiroBusinessException(string.Format(Utils.Messages.Messages.msgErroSumPolCom), null, FinanceiroBusinessException.TipoErro.ERRO_SUM_DIFERENTE_POLCOM, false);
                }
                else
                    throw new FinanceiroBusinessException(string.Format(Utils.Messages.Messages.msgErroIncluirPacDif), null, FinanceiroBusinessException.TipoErro.ERRO_PARC_DIF_OBRIGATORIO, false);
            }
            return DataAccessPoliticaComercial.add(polComercial, false);
        }

        public IEnumerable<PoliticaComercial> getPoliticaComercialSearch(SearchParameters parametros, string descricao, bool inicio, bool? ativo, bool parcIguais, bool vencFixo, int cdEscola)
        {
            IEnumerable<PoliticaComercial> retorno = new List<PoliticaComercial>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "dc_politica_comercial";
                parametros.sort = parametros.sort.Replace("venc_fixo", "id_vencimento_fixo");
                parametros.sort = parametros.sort.Replace("parc_iguais", "id_parcela_igual");
                parametros.sort = parametros.sort.Replace("pol_ativa", "id_politica_ativa");
                parametros.sort = parametros.sort.Replace("periodo_intervalo", "nm_periodo_intervalo");

                retorno = DataAccessPoliticaComercial.getPoliticaComercialSearch(parametros, descricao, inicio, ativo, parcIguais, vencFixo, cdEscola);
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<PoliticaComercial> getPoliticaComercialByEmpresa(int cd_pessoa_escola, string dc_politica, bool inicio)
        {
            return DataAccessPoliticaComercial.getPoliticaComercialByEmpresa(cd_pessoa_escola, dc_politica, inicio);
        }

        public bool deleteAllPolCom(List<PoliticaComercial> politicas, int cd_escola)
        {
            bool retorno = true;

            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                foreach (var p in politicas)
                {
                    PoliticaComercial pol = DataAccessPoliticaComercial.getPoliticaComercialById(p.cd_politica_comercial, cd_escola);
                    retorno = DataAccessPoliticaComercial.delete(pol, false);
                }
                transaction.Complete();
            }
            return retorno;
        }

        public PoliticaComercial editPolCom(PoliticaComercial politica)
        {
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                PoliticaComercial polContext = DataAccessPoliticaComercial.getPoliticaComercialById(politica.cd_politica_comercial, politica.cd_pessoa_empresa);
                if (!politica.id_parcela_igual)
                {
                    //Se não for vencimento fixo, o número de dias não pode ser null
                    if (!politica.id_vencimento_fixo)
                        foreach (ItemPolitica p in politica.ItemPolitica)
                            if (p.nm_dias_politica == null)
                                throw new FinanceiroBusinessException(string.Format(Utils.Messages.Messages.msgErroNrDiasPolComNull), null, FinanceiroBusinessException.TipoErro.ERRO_DIA_POL_COMERCIAL_NULL, false);
                    double? sumPolCom = 0;
                    if (politica.ItemPolitica != null && politica.ItemPolitica.Count() > 0)
                    {
                        sumPolCom = politica.ItemPolitica.Sum(i => i.pc_politica);
                        if (sumPolCom != 100)
                            throw new FinanceiroBusinessException(string.Format(Utils.Messages.Messages.msgErroSumPolCom), null, FinanceiroBusinessException.TipoErro.ERRO_SUM_DIFERENTE_POLCOM, false);
                    }
                    else
                        throw new FinanceiroBusinessException(string.Format(Utils.Messages.Messages.msgErroIncluirPacDif), null, FinanceiroBusinessException.TipoErro.ERRO_PARC_DIF_OBRIGATORIO, false);
                }
                //Editar itens da política comercial
                List<ItemPolitica> itensDeleted = polContext.ItemPolitica.Where(tc => !politica.ItemPolitica.Any(tv => tc.cd_item_politica == tv.cd_item_politica)).ToList();
                if (itensDeleted != null && itensDeleted.Count() > 0)
                    foreach (ItemPolitica ip in itensDeleted)
                    {
                        ItemPolitica item = DataAccessItemPolitica.findById(ip.cd_item_politica, false);
                        item.PoliticaComercial = null;
                        DataAccessItemPolitica.delete(item, false);
                    }
                //Alterando ou Incluindo novos itens
                foreach (ItemPolitica ip in politica.ItemPolitica)
                    if (ip.cd_item_politica == 0)
                    {
                        ip.cd_politica_comercial = politica.cd_politica_comercial;
                        DataAccessItemPolitica.add(ip, false);
                    }
                    else
                    {
                        ItemPolitica item = polContext.ItemPolitica.Where(tc => tc.cd_item_politica == ip.cd_item_politica).FirstOrDefault();
                        item.nm_dias_politica = ip.nm_dias_politica;
                        item.pc_politica = ip.pc_politica;
                        DataAccessItemPolitica.saveChanges(false);
                    }
                politica = PoliticaComercial.changeValuePolCom(polContext, politica);

                DataAccessPoliticaComercial.saveChanges(false);
                transaction.Complete();
            }
            politica.ItemPolitica = null;
            return politica;


        }

        public PoliticaComercial getPoliticaComercialSugeridaNF(int cd_escola)
        {
            return DataAccessPoliticaComercial.getPoliticaComercialSugeridaNF(cd_escola);
        }
        #endregion

        #region Kardex
        public Kardex addKardex(Kardex kardex)
        {
            return DataAccessKardex.add(kardex, false);
        }

        public void atualizaKardex(Kardex kardex)
        {
            Kardex kardexContext = DataAccessKardex.findById(kardex.cd_kardex, false);
            kardexContext.copy(kardex);
            DataAccessKardex.saveChanges(false);
        }

        public Kardex editKardex(Kardex kardex)
        {
            return DataAccessKardex.edit(kardex, false);
        }

        public bool deleteKardex(Kardex kardex)
        {
            return DataAccessKardex.delete(kardex, false);
        }

        public IEnumerable<Kardex> getKardexByOrigem(int cd_origem, int cd_registro_origem)
        {
            return DataAccessKardex.getKardexByOrigem(cd_origem, cd_registro_origem);
        }

        public IEnumerable<KardexUI> st_Rptkardex(int cd_pessoa_escola, int cd_item, DateTime dt_ini, DateTime dt_fim, int cd_grupo, byte tipo, bool isApenasItensMovimento)
        {
            List<KardexUI> kardexSaldo = new List<KardexUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                DateTime dta_saldo = ((DateTime)dt_ini);
                dta_saldo = dta_saldo.AddDays(-1);
                kardexSaldo = DataAccessKardex.st_Rptkardex(cd_pessoa_escola, cd_item, dt_ini, dt_fim, cd_grupo, tipo).ToList();
                List<KardexUI> kardexSaldoCorpo = kardexSaldo.ToList();
                //List<KardexUI> kardexSaldoRetroativos = kardexSaldo.Where(k => k.id_saldo == 1).ToList();
                //Regras de saldo e valores para movimentos dentro do periodo.
                List<KardexUI> kardexSaldoAgrupado = kardexSaldoCorpo.GroupBy(ks => ks.cd_item).Select(kardex => (KardexUI)kardex.First().Clone()).ToList();
                List<KardexUI> kardexApenasComMovimento = new List<KardexUI>();

                if (kardexSaldoAgrupado != null) { 
                    for (int i = 0; i < kardexSaldoAgrupado.Count(); i++)
                    {
                        if ((!isApenasItensMovimento) || (kardexSaldo.Where(x => x.cd_item == kardexSaldoAgrupado[i].cd_item && x.tx_obs_kardex != "Saldo Anterior").Count() >= 1))
                        {
                            kardexApenasComMovimento.Add(kardexSaldoAgrupado[i]);

                            decimal? vl_saldo = 0;
                            double? qt_saldo = 0;
                            //decimal vl_medio = 0;
                            //decimal vl_aux = 0;
                            //decimal vl_unit = 0;


                            if (kardexSaldoAgrupado[i].id_saldo == 2 || kardexSaldoAgrupado[i].id_saldo == 1)
                            {
                                kardexSaldoAgrupado[i].dt_kardex = dt_ini.AddDays(-1);
                                //kardexSaldoAgrupado[i].qt_inicial = DataAccessKardex.getSaldoItem(kardexSaldoAgrupado[i].cd_item, dta_saldo, cd_pessoa_escola);
                                kardexSaldoAgrupado[i].qt_saldo = DataAccessKardex.getSaldoItem(kardexSaldoAgrupado[i].cd_item, dta_saldo, cd_pessoa_escola);
                                kardexSaldoAgrupado[i].qt_entrada = 0;
                                kardexSaldoAgrupado[i].qt_saida = 0;
                                kardexSaldoAgrupado[i].vl_entrada = 0;
                                //kardexSaldoAgrupado[i].vl_inicial = DataAccessKardex.getSaldoValorItem(kardexSaldoAgrupado[i].cd_item, dta_saldo, cd_pessoa_escola) ?? 0;
                                kardexSaldoAgrupado[i].vl_saldo = DataAccessKardex.getSaldoValorItem(kardexSaldoAgrupado[i].cd_item, dta_saldo, cd_pessoa_escola) ?? 0;

                                kardexSaldoAgrupado[i].vl_saida = 0;
                                kardexSaldoAgrupado[i].tx_obs_kardex = "Saldo Anterior";
                            }

                            vl_saldo = kardexSaldoAgrupado[i].vl_saldo;
                            qt_saldo = kardexSaldoAgrupado[i].qt_saldo;
                            //vl_medio = kardexSaldoAgrupado[i].vl_saldo;
                            //vl_aux = vl_saldo/qt_saldo;

                            //acertando o valores de saldo do item
                            //var ListaItens = kardexSaldoCorpo.Where(x => x.cd_item == kardexSaldoAgrupado[i].cd_item).OrderBy(k => k.no_grupo_estoque).ThenBy(k => k.no_item).ThenBy(k => k.id_saldo).ThenBy(k => k.dt_kardex).ThenBy(k => k.cd_kardex).ToList();

                            //foreach (var itens in ListaItens)
                            //{
                                //if (itens.id_tipo_movimento == (byte)Kardex.TipoMovimento.SAIDA)
                                //{
                                    //vl_aux = Decimal.Round(itens.qt_saida*vl_unit,2);
                                    //vl_medio = vl_medio - vl_aux;
                                    //qt_saldo = qt_saldo - itens.qt_saida;
                                    //if (qt_saldo == 0){
                                    //    vl_medio = 0;
                                    //    vl_unit = 0;
                                    //}
                                    //ANTIGO
                                    //var saldoEContagemAnterior = kardexSaldo.Where(x => x.cd_item == itens.cd_item && x.dt_kardex <= itens.dt_kardex &&
                                    //                                                    x.id_tipo_movimento == (byte)Kardex.TipoMovimento.ENTRADA &&
                                    //                                                    x.cd_kardex < itens.cd_kardex).OrderByDescending(o => o.dt_kardex).FirstOrDefault();
                                    //if (saldoEContagemAnterior == null)
                                    //    saldoEContagemAnterior = kardexSaldoAgrupado[i];
                                    //decimal valorVenda = (saldoEContagemAnterior.vl_saldo > 0 && saldoEContagemAnterior.qt_saldo != 0) ? (saldoEContagemAnterior.vl_saldo / saldoEContagemAnterior.qt_saldo) : 0;

                                    //itens.vl_saida = (itens.qt_saida * valorVenda);
                                    //itens.vl_saldo = vl_saldo - itens.vl_saida;
                                //}
                                //else{
                                    //qt_saldo = qt_saldo + itens.qt_entrada;
                                    //if (qt_saldo != 0)
                                    //{
                                    //    if (qt_saldo < 0)
                                    //        itens.vl_entrada = -1 * itens.vl_entrada;
                                    //    vl_aux = itens.vl_entrada;
                                    //    vl_medio = vl_medio + itens.vl_entrada;
                                    //    if (qt_saldo == 0)
                                    //        vl_unit = 0;
                                    //    else
                                    //        vl_unit = vl_medio / qt_saldo;
                                    //}
                                    //else
                                    //{
                                    //    vl_aux = -1 * vl_medio;
                                    //    vl_unit = 0;
                                    //    vl_medio = 0;
                                    //}
                                    //Antigo
                                    //itens.vl_saldo = vl_saldo + itens.vl_entrada;
                                //}
                                //itens.qt_saldo = itens.id_tipo_movimento == (byte)Kardex.TipoMovimento.ENTRADA ? qt_saldo + itens.qt_entrada : qt_saldo - itens.qt_saida;
                                //itens.vl_saldo = itens.id_tipo_movimento == 0 ? (decimal)itens.vl_inicial: (decimal)itens.vl_inicial + itens.vl_entrada - itens.vl_saida;
                                //vl_saldo = itens.vl_saldo;
                                //qt_saldo = itens.qt_saldo;
                            //}
                        }
                    }
                }
                if (isApenasItensMovimento)
                {
                    var kardexSaldoComMovimento = new List<KardexUI>();
                    foreach (var kdx in kardexApenasComMovimento)
                        kardexSaldoComMovimento.AddRange(kardexSaldo.Where(x => x.cd_item == kdx.cd_item));

                    kardexSaldoComMovimento.AddRange(kardexApenasComMovimento);
                    kardexSaldo = kardexSaldoComMovimento;
                }
                else
                    kardexSaldo.AddRange(kardexSaldoAgrupado);

                if (kardexSaldo.Any(x => x.id_saldo == 1))
                {
                    if (kardexSaldo.Count > 1)
                    {
                        //Tira as linhas sem movimento e com tx_obs = "saldo anterior"
                         kardexSaldo = kardexSaldo.Where(x => !(x.dt_kardex != dta_saldo &&
                                                                    x.tx_obs_kardex == "Saldo Anterior" &&
                                                                    x.qt_entrada == 0 &&
                                                                    x.qt_saida == 0 &&
                                                                    x.qt_saldo == 0.0 &&
                                                                    x.qt_inicial == 0)
                        ).ToList();

                         //ordena o kardex
                         kardexSaldo = kardexSaldo.OrderBy(k => k.no_item).ThenBy(k => k.dt_kardex).ThenBy(k => k.cd_kardex).ToList();
                    }
                }
                else
                {
                    kardexSaldo = kardexSaldo.OrderBy(k => k.no_item).ThenBy(k => k.dt_kardex).ThenBy(k => k.cd_kardex).ToList();
                }

                
                var grupo = kardexSaldo.GroupBy(x => x.cd_item).Select(x => x.Key).ToList();

                foreach (int cod_item in grupo)
                {
                    var kdx = kardexSaldo.Where(x => x.cd_item == cod_item && x.tx_obs_kardex == "Saldo Anterior").FirstOrDefault();

                    if (kdx != null)
                    {
                        kardexSaldo.Where(x => x.cd_item == cod_item && x.tx_obs_kardex == "Saldo Anterior").FirstOrDefault().qt_saldo_item = kardexSaldo.Where(x => x.cd_item == cod_item).LastOrDefault().qt_saldo;
                        kardexSaldo.Where(x => x.cd_item == cod_item && x.tx_obs_kardex == "Saldo Anterior").FirstOrDefault().vl_saldo_item = kardexSaldo.Where(x => x.cd_item == cod_item).LastOrDefault().vl_saldo;
                    }
                }

                kardexSaldo = kardexSaldo.OrderBy(k => k.no_grupo_estoque).ThenBy(k => k.no_item).ThenBy(k => k.id_saldo).ThenBy(k => k.dt_kardex).ThenBy(k => k.cd_kardex).ToList();
                          
                transaction.Complete();
            }
            return kardexSaldo;
        }

        public int getSaldoItem(int cd_item, DateTime dataLimite, int cd_escola)
        {
            return DataAccessKardex.getSaldoItem(cd_item, dataLimite, cd_escola);
        }

        public IEnumerable<Kardex> getKardexItensMovimentoNF(int cd_movimento, int cd_pessoa)
        {
            return DataAccessKardex.getKardexItensMovimentoNF(cd_movimento, cd_pessoa);
        }

        public bool existeKardexItemMovimentoByOrigem(int cd_origem, int cd_registro_origem)
        {
            return DataAccessKardex.existeKardexItemMovimentoByOrigem(cd_origem, cd_registro_origem);
        }
        //public List<sp_RptInventario_Result> getRtpInventario(int? cd_escola, DateTime? dt_analise, byte? id_valor)
        public DataTable getRtpInventario(int? cd_escola, DateTime? dt_analise, byte? id_valor, string tipoItem)
        {
            return DataAccessKardex.getRptInventario(cd_escola, dt_analise, id_valor, tipoItem);
        }

        #endregion

        #region Conta Corrente

        public IEnumerable<ContaCorrenteUI> getContaCorreteSearch(SearchParameters parametros, int cd_pessoa_escola, int cd_origem, int cd_destino, byte entraSaida, int cd_movimento, int cd_plano_conta, DateTime? dta_ini, DateTime? dta_fim, int cd_pessoa_usuario, bool contaSegura)
        {
            IEnumerable<ContaCorrenteUI> retorno = new List<ContaCorrenteUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "dta_conta_corrente";

                parametros.sort = parametros.sort.Replace("dt_conta_corrente", "dta_conta_corrente");
                parametros.sort = parametros.sort.Replace("tipo_movimento", "id_tipo_movimento");
                parametros.sort = parametros.sort.Replace("vlConta_corrente", "vl_conta_corrente");
                parametros.sort = parametros.sort.Replace("destino", "localDestino.no_local_movto");
                parametros.sort = parametros.sort.Replace("origem", "localOrigem.no_local_movto");

                retorno = DataAccessContaCorrente.getContaCorreteSearch(parametros, cd_pessoa_escola, cd_origem, cd_destino, entraSaida, cd_movimento, cd_plano_conta, dta_ini, dta_fim, cd_pessoa_usuario, contaSegura);
                transaction.Complete();
            }
            return retorno;
        }

        public ContaCorrenteUI incluirContaCorrente(ContaCorrenteUI contaCorrente, int cd_pessoa_escola, bool isSupervisor, int movimentoRetroativo)
        {
            ContaCorrenteUI conta = new ContaCorrenteUI();
            ContaCorrente contaCc = new ContaCorrente();
            DateTime data = new DateTime();
            DateTime dataCorrente = DateTime.Now.Date;

            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                contaCc.copy(contaCorrente);
                contaCc.cd_pessoa_empresa = cd_pessoa_escola;

                if (contaCc.dta_conta_corrente.HasValue)
                {
                    data = (DateTime)contaCc.dta_conta_corrente;
                    data = data.ToLocalTime().Date;
                    contaCc.dta_conta_corrente = data;
                }
                else
                    throw new FinanceiroBusinessException(string.Format(Messages.msgContaCorrenteDataNula), null, FinanceiroBusinessException.TipoErro.ERRO_DATA_CONTA_CORRENTE_NULA, false);

                MovimentarContaReatroativa(isSupervisor, movimentoRetroativo, data, dataCorrente);

                contaCc = DataAccessContaCorrente.add(contaCc, false);
                contaCorrente.cd_conta_corrente = contaCc.cd_conta_corrente;

                contaCorrente.localOrigem = DataAccessLocalMovto.findById(contaCorrente.cd_local_origem, false);
                if (contaCorrente.cd_local_destino != null && contaCorrente.cd_local_destino > 0)
                    contaCorrente.localDestino = DataAccessLocalMovto.findById((short)contaCorrente.cd_local_destino, false);

                conta = ContaCorrenteUI.fromContaCorrenteUI(contaCorrente);
                transaction.Complete();
            }
            return conta;
        }

        private static void MovimentarContaReatroativa(bool isSupervisor, int movimentoRetroativo, DateTime data, DateTime dataCorrente)
        {
            if (!isSupervisor)
            {
                int dia = (dataCorrente - data).Days;

                if ((movimentoRetroativo > 0) && (DateTime.Compare(data, dataCorrente) == -1))
                    if (movimentoRetroativo < dia)
                        throw new FinanceiroBusinessException(string.Format(Messages.msgMovimentarContaCorrenteRetroativa, String.Format("{0:dd/MM/yyyy}", dataCorrente.AddDays(-movimentoRetroativo)) + "."), null, FinanceiroBusinessException.TipoErro.ERRO_DATA_RETROATIVA_CONTA_CORRENTE, false);
            }
        }

        public bool deleteContaCorrente(ICollection<ContaCorrenteUI> contaCorrenteUI, bool isSupervisor, int movimentoRetroativo)
        {
            bool deleted = false;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                DateTime data = new DateTime();
                DateTime dataCorrente = DateTime.Now.Date;
                if (contaCorrenteUI != null && contaCorrenteUI.Count() > 0)
                {
                    List<ContaCorrenteUI> contasCC = contaCorrenteUI.ToList();
                    foreach (ContaCorrenteUI c in contasCC)
                    {
                        if (c.dta_conta_corrente.HasValue)
                        {
                            data = (DateTime)c.dta_conta_corrente;
                            data = data.ToLocalTime().Date;
                            c.dta_conta_corrente = data;
                        }

                        MovimentarContaReatroativa(isSupervisor, movimentoRetroativo, data, dataCorrente);

                        ContaCorrente conta = DataAccessContaCorrente.findById(c.cd_conta_corrente, false);
                        if (conta != null && conta.cd_baixa_titulo == null || conta.cd_baixa_titulo <= 0)
                            deleted = DataAccessContaCorrente.delete(conta, false);
                        else
                            throw new FinanceiroBusinessException(string.Format(Messages.msgContaCorrenteBaixaFinan), null, FinanceiroBusinessException.TipoErro.ERRO_CONTA_BAIXA_FINAN, false);
                    }
                }
                transaction.Complete();
            }
            return deleted;
        }

        public IEnumerable<ContaCorrente> getObservacoesCCBaixa(int? cd_baixa_titulo, int? cd_conta_corrente)
        {
            IEnumerable<ContaCorrente> retorno = new List<ContaCorrente>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = DataAccessContaCorrente.getObservacoesCCBaixa(cd_baixa_titulo, cd_conta_corrente);
                transaction.Complete();
            }
            return retorno;
        }

        public ContaCorrenteUI editarContaCorrente(ContaCorrenteUI contaCorrenteUI, bool isSupervisor, int movimentoRetroativo)
        {
            int editadado = 0;
            DateTime data = new DateTime();
            DateTime dataCorrente = DateTime.Now.Date;

            ContaCorrente contaCc = new ContaCorrente();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                contaCc = DataAccessContaCorrente.findById(contaCorrenteUI.cd_conta_corrente, false);
                if (contaCc.cd_baixa_titulo == null || contaCc.cd_baixa_titulo <= 0)
                {
                    contaCc.copy(contaCorrenteUI);
                    contaCc.cd_plano_conta = contaCorrenteUI.cd_plano_conta <= 0 ? null : contaCorrenteUI.cd_plano_conta;
                    contaCc.cd_local_destino = contaCorrenteUI.cd_local_destino <= 0 ? null : contaCc.cd_local_destino;

                    if (contaCc.dta_conta_corrente.HasValue)
                    {
                        data = (DateTime)contaCc.dta_conta_corrente;
                        data = data.ToLocalTime().Date;
                        contaCc.dta_conta_corrente = data;
                    }
                    else
                        throw new FinanceiroBusinessException(string.Format(Messages.msgContaCorrenteDataNula), null, FinanceiroBusinessException.TipoErro.ERRO_DATA_CONTA_CORRENTE_NULA, false);

                    MovimentarContaReatroativa(isSupervisor, movimentoRetroativo, data, dataCorrente);

                    editadado = DataAccessContaCorrente.saveChanges(false);
                    contaCorrenteUI.localOrigem = DataAccessLocalMovto.findById(contaCorrenteUI.cd_local_origem, false);
                    if (contaCorrenteUI.cd_local_destino != null && contaCorrenteUI.cd_local_destino > 0)
                        contaCorrenteUI.localDestino = DataAccessLocalMovto.findById((short)contaCorrenteUI.cd_local_destino, false);

                    contaCorrenteUI = ContaCorrenteUI.fromContaCorrenteUI(contaCorrenteUI);
                }
                else
                {
                    throw new FinanceiroBusinessException(string.Format(Messages.msgContaCorrenteBaixaFinan), null, FinanceiroBusinessException.TipoErro.ERRO_CONTA_BAIXA_FINAN, false);
                }
                transaction.Complete();
            }
            return contaCorrenteUI;
        }

        public IEnumerable<ContaCorrenteUI> getRelatorioContaCorrente(int cd_pessoa_escola, DateTime? dta_ini, DateTime? dta_fim, int cd_local_movto, int tipoLiquidacao, bool contaSegura, bool isMaster)
        {
            IEnumerable<ContaCorrenteUI> retorno = new List<ContaCorrenteUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                DateTime dta_saldo = ((DateTime)dta_ini);

                //LBM Eliminado pois o fechamanto vai lançar Saldo da diferenca
                //ContaCorrente existeMovtoAberto = DataAccessContaCorrente.existAberturaSaldoData((DateTime)dta_ini, cd_pessoa_escola, cd_local_movto, tipoLiquidacao);
                //if (existeMovtoAberto != null)
                //    throw new FinanceiroBusinessException(string.Format(Messages.msgErroSaldoAbertura, String.Format("{0:dd-MM-yyyy}", existeMovtoAberto.dta_conta_corrente)), null, FinanceiroBusinessException.TipoErro.ERRO_ABERTURA_SALDO, false);

                Decimal saldo_inicial = 0;
                if (tipoLiquidacao == 0)
                {
                    List<TipoLiquidacao> listaTpLiq = DataAccessTipoLiquidacao.getTipoLiquidacaoCCByLocalMovto(cd_local_movto).ToList();
                    foreach (TipoLiquidacao tpLiq in listaTpLiq)
                        saldo_inicial = saldo_inicial + DataAccessContaCorrente.fcSaldoContaCorrente(cd_pessoa_escola, cd_local_movto, dta_saldo, tpLiq.cd_tipo_liquidacao);
                }
                else
                    saldo_inicial = DataAccessContaCorrente.fcSaldoContaCorrente(cd_pessoa_escola, cd_local_movto, dta_saldo, tipoLiquidacao);

                IEnumerable<ContaCorrenteUI> rptContas = DataAccessContaCorrente.rtpContaCorrente(cd_pessoa_escola, cd_local_movto, (DateTime)dta_ini, (DateTime)dta_fim, saldo_inicial, tipoLiquidacao, contaSegura, isMaster).ToList();

                if (rptContas.Count() <= 0)
                {
                    string descricao = Messages.msgNotExistMov + String.Format("{0:dd/MM/yyyy}", (DateTime)dta_ini) + " á " + String.Format("{0:dd/MM/yyyy}", (DateTime)dta_fim);
                    List<ContaCorrenteUI> contas = new List<ContaCorrenteUI>();
                    List<LocalMovto> locaisMvto = DataAccessLocalMovto.getLocalMovto(cd_pessoa_escola, cd_local_movto).ToList();
                    contas.Add(
                        new ContaCorrenteUI
                        {
                            dta_conta_corrente = (DateTime)dta_ini,
                            dc_obs_conta_corrente = descricao,
                            saldo_inicial = saldo_inicial,
                            vl_entrada = 0,
                            vl_saida = 0,
                            des_local_movto = locaisMvto != null && locaisMvto.Count() > 0 ? locaisMvto.FirstOrDefault().no_local_movto : ""
                        });
                    rptContas = contas;
                }
                retorno = rptContas;
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<SaldoFinanceiro> getRelatorioSaldoFinanceiro(int cd_pessoa_escola, DateTime dt_base, byte tipoLocal, bool liquidacao)
        {
            List<SaldoFinanceiro> listaSaldosBancosLiquidacao = new List<SaldoFinanceiro>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                DateTime dta_base = ((DateTime)dt_base);
                List<SaldoFinanceiro> ContasSemMovimentoDia = new List<SaldoFinanceiro>();
                IEnumerable<SaldoFinanceiro> rptContas = DataAccessContaCorrente.rtpSaldoFinanceiro(cd_pessoa_escola, dta_base, tipoLocal, liquidacao).ToList();
                IEnumerable<SaldoFinanceiro> rptContasSemMovimento = DataAccessContaCorrente.rtpLocalMovimentoSemCCorrenteDtaBase(cd_pessoa_escola, dta_base, tipoLocal);
                // { x.id_tipo, x.banco, x.tipo, x.nm_tipo }
                listaSaldosBancosLiquidacao = rptContas.GroupBy(x => new { x.id_tipo, x.banco, x.tipo, x.nm_tipo, x.cd_tipo_liquidacao, x.cd_local }).Select(g => new FundacaoFisk.SGF.Web.Services.Financeiro.Model.SaldoFinanceiro
                {
                    nm_tipo = g.Key.nm_tipo,
                    tipo = g.Key.tipo,
                    banco = g.Key.banco,
                    cd_local = g.Key.cd_local,
                    cd_tipo_liquidacao = g.Key.cd_tipo_liquidacao,
                    //saldo_inicial = g.Sum(y => y.saldo_inicial),
                    entrada = g.Sum(y => y.entrada),
                    saida = g.Sum(y => y.saida),
                    //saldo = g.Sum(y => y.saldo)//,
                    //dta_conta_corrente = g.Key.dta_conta_corrente
                }).ToList();
                if (listaSaldosBancosLiquidacao.Count() > 0)
                    foreach (SaldoFinanceiro sf in listaSaldosBancosLiquidacao)
                    {
                        //sf.dta_conta_corrente = rptContas.Where(x => x.cd_conta_corrente == sf.cd_conta_corrente).FirstOrDefault().dta_conta_corrente;
                        //LBM Eliminado pois o fechamanto vai lançar Saldo da diferenca
                        //ContaCorrente existeMovtoAberto = DataAccessContaCorrente.existAberturaSaldoData(dt_base, cd_pessoa_escola, 0, 0);
                        //if (existeMovtoAberto != null)
                        //    throw new FinanceiroBusinessException(string.Format(Messages.msgErroSaldoAbertura, String.Format("{0:dd-MM-yyyy}", existeMovtoAberto.dta_conta_corrente)), null, FinanceiroBusinessException.TipoErro.ERRO_ABERTURA_SALDO, false);
                        sf.saldo_inicial = DataAccessContaCorrente.fcSaldoContaCorrente(cd_pessoa_escola, (int)sf.cd_local, dta_base, sf.cd_tipo_liquidacao);
                        if (sf.entrada != null && sf.saida != null)
                            sf.saldo = (sf.saldo_inicial + (decimal)sf.entrada) - (decimal)sf.saida;
                        else
                            if (sf.entrada != null)
                                sf.saldo = (sf.saldo_inicial + (decimal)sf.entrada);
                            else
                                sf.saldo = sf.saldo_inicial - (decimal)sf.saida;
                    }

                if (rptContasSemMovimento != null && rptContasSemMovimento.Count() > 0)
                    foreach (SaldoFinanceiro sf in rptContasSemMovimento)
                    {
                        if (sf.tiposLiquidacao != null && sf.tiposLiquidacao.Count() > 0)
                            foreach (TipoLiquidacao tpl in sf.tiposLiquidacao)
                            {
                                SaldoFinanceiro saldo = new SaldoFinanceiro
                                                    {
                                                        cd_local = sf.cd_local,
                                                        nm_tipo = sf.nm_tipo,
                                                        cd_tipo_liquidacao = tpl.cd_tipo_liquidacao,
                                                        banco = sf.banco,
                                                        tipo = tpl.dc_tipo_liquidacao,
                                                        entrada = 0,
                                                        saida = 0,
                                                        saldo = 0
                                                    };
                                //LBM Eliminado pois o fechamanto vai lançar Saldo da diferenca
                                //ContaCorrente existeMovtoAberto = DataAccessContaCorrente.existAberturaSaldoData(dt_base, cd_pessoa_escola, 0, 0);
                                //if (existeMovtoAberto != null)
                                //    throw new FinanceiroBusinessException(string.Format(Messages.msgErroSaldoAbertura, String.Format("{0:dd-MM-yyyy}", existeMovtoAberto.dta_conta_corrente)), null, FinanceiroBusinessException.TipoErro.ERRO_ABERTURA_SALDO, false);
                                saldo.saldo = saldo.saldo_inicial = DataAccessContaCorrente.fcSaldoContaCorrente(cd_pessoa_escola, (int)sf.cd_local, dta_base, tpl.cd_tipo_liquidacao);
                                if (saldo.saldo_inicial != 0)
                                    ContasSemMovimentoDia.Add(saldo);
                                //sf.saldo_inicial = DataAccessContaCorrente.fcSaldoContaCorrente(cd_pessoa_escola, (int)sf.cd_local, dta_base, sf.cd_tipo_liquidacao);
                            }

                    }
                listaSaldosBancosLiquidacao.AddRange(ContasSemMovimentoDia);

                transaction.Complete();
            }
            return listaSaldosBancosLiquidacao;
        }

        public ContaCorrenteUI getContaCorretePlanoConta(int cd_pessoa_escola, int cd_conta_corrente)
        {
            return DataAccessContaCorrente.getContaCorretePlanoConta(cd_pessoa_escola, cd_conta_corrente);
        }
        public IEnumerable<ContaCorrenteUI> getFechamentoCaixaTpLiquidacao(int cd_pessoa_escola, DateTime dta_fechamento, int cdUsuario, byte tipoLocal, bool mostrarZerados)
        {
            List<ContaCorrenteUI> fechamento = new List<ContaCorrenteUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                fechamento = DataAccessContaCorrente.getFechamentoCaixaTpLiquidacao(cd_pessoa_escola, dta_fechamento, cdUsuario, tipoLocal).ToList();
                if (fechamento != null)
                {
                    fechamento = fechamento.GroupBy(x => new { x.cd_local_movimento, x.cd_tipo_liquidacao, x.dc_tipo_liquidacao }).
                        Select(g => new FundacaoFisk.SGF.Web.Services.Financeiro.Model.ContaCorrenteUI
                    {
                        cd_local_movimento = g.Key.cd_local_movimento,
                        cd_tipo_liquidacao = g.Key.cd_tipo_liquidacao,
                        dc_tipo_liquidacao = g.Key.dc_tipo_liquidacao,
                        vl_entrada = g.Where(d => d.dta_conta_corrente == dta_fechamento).Sum(y => y.vl_entrada),
                        vl_saida = g.Where(d => d.dta_conta_corrente == dta_fechamento).Sum(y => y.vl_saida)
                    }).ToList();

                    if (fechamento.Count() > 0)
                        foreach (ContaCorrenteUI f in fechamento)
                        {
                            //sf.dta_conta_corrente = rptContas.Where(x => x.cd_conta_corrente == sf.cd_conta_corrente).FirstOrDefault().dta_conta_corrente;
                            //ContaCorrente existeMovtoAberto = DataAccessContaCorrente.existAberturaSaldoData(dt_base, cd_pessoa_escola, 0, 0);
                            //if (existeMovtoAberto != null)
                            //    throw new FinanceiroBusinessException(string.Format(Messages.msgErroSaldoAbertura, String.Format("{0:dd-MM-yyyy}", existeMovtoAberto.dta_conta_corrente)), null, FinanceiroBusinessException.TipoErro.ERRO_ABERTURA_SALDO, false);
                            f.saldo_inicial = DataAccessContaCorrente.fcSaldoContaCorrente(cd_pessoa_escola, (int)f.cd_local_movimento, dta_fechamento, f.cd_tipo_liquidacao);

                            //f.obsSaldoCaixa = DataAccessObsSaldoCaixa.getObsSaldoCaixaUsuario(cd_pessoa_escola, (int)f.cd_local_movimento, dta_fechamento, cdUsuario);

                            if (f.vl_entrada != null && f.vl_saida != null)
                                f.saldo_final = (f.saldo_inicial + (decimal)f.vl_entrada) - (decimal)f.vl_saida;
                            else
                                if (f.vl_entrada != null)
                                    f.saldo_final = (f.saldo_inicial + (decimal)f.vl_entrada);
                                else
                                    if (f.vl_saida != null)
                                        f.saldo_final = f.saldo_inicial - (decimal)f.vl_saida;
                        }

                    fechamento = fechamento.GroupBy(x => new { x.cd_tipo_liquidacao, x.dc_tipo_liquidacao }).Select(g => new FundacaoFisk.SGF.Web.Services.Financeiro.Model.ContaCorrenteUI
                    {
                        cd_tipo_liquidacao = g.Key.cd_tipo_liquidacao,
                        dc_tipo_liquidacao = g.Key.dc_tipo_liquidacao,
                        saldo_inicial = g.Sum(y => y.saldo_inicial),
                        saldo_final = g.Sum(y => y.saldo_final),
                        vl_entrada = g.Sum(y => y.vl_entrada),
                        vl_saida = g.Sum(y => y.vl_saida)
                    }).Where(x=>x.saldo_inicial != 0 || x.vl_entrada != 0 || x.vl_saida != 0 || mostrarZerados).ToList();
                }
                transaction.Complete();
            }
            return fechamento;
        }

        public ObsSaldoCaixa postObsSaldoCaixaUsuario(ObsSaldoCaixa obsSaldoCaixa, int cdEscola)
        {
            ObsSaldoCaixa obsSaldoCaixaRetorno = null;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                if (obsSaldoCaixa.cd_usuario > 0 && obsSaldoCaixa.dt_saldo_caixa != null)
                {
                    obsSaldoCaixa.dt_saldo_caixa = obsSaldoCaixa.dt_saldo_caixa.Date;
                    if (obsSaldoCaixa.cd_obs_saldo_caixa > 0)
                    {
                        if (obsSaldoCaixa.cd_caixa_usuario != null && obsSaldoCaixa.cd_caixa_usuario > 0)
                            obsSaldoCaixaRetorno = DataAccessObsSaldoCaixa.getObsSaldoCaixaUsuario(cdEscola, obsSaldoCaixa.cd_caixa_usuario.Value, obsSaldoCaixa.dt_saldo_caixa, obsSaldoCaixa.cd_usuario);
                        else
                            obsSaldoCaixaRetorno = DataAccessObsSaldoCaixa.getObsSaldoCaixaUsuario(cdEscola, 0, obsSaldoCaixa.dt_saldo_caixa, obsSaldoCaixa.cd_usuario);

                        if (string.IsNullOrEmpty(obsSaldoCaixa.tx_obs_saldo_caixa) && obsSaldoCaixaRetorno != null)
                        {
                            DataAccessObsSaldoCaixa.delete(obsSaldoCaixaRetorno, false);
                            obsSaldoCaixaRetorno = null;
                        }
                        else
                        {
                            obsSaldoCaixaRetorno.tx_obs_saldo_caixa = obsSaldoCaixa.tx_obs_saldo_caixa;
                            DataAccessObsSaldoCaixa.saveChanges(false);
                        }
                    }
                    else if (!string.IsNullOrEmpty(obsSaldoCaixa.tx_obs_saldo_caixa))
                    {
                        obsSaldoCaixaRetorno = DataAccessObsSaldoCaixa.add(obsSaldoCaixa, false);
                    }
                }
                transaction.Complete();
            }
            return obsSaldoCaixaRetorno;
        }

        public IEnumerable<ContaCorrenteUI> getFechamentoCaixaLocalMovto(int cd_pessoa_escola, DateTime dta_fechamento, int tipoLiquidacao, int cdUsuario, byte tipoLocal, bool mostrarZerados)
        {
            List<ContaCorrenteUI> fechamento = new List<ContaCorrenteUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                fechamento = DataAccessContaCorrente.getFechamentoCaixaLocalMovto(cd_pessoa_escola, dta_fechamento, tipoLiquidacao, cdUsuario, tipoLocal).ToList();
                if (tipoLiquidacao > 0 && fechamento != null)
                {
                    fechamento = fechamento.GroupBy(x => new { x.cd_local_movimento, x.des_local_movto }).Select(g => new FundacaoFisk.SGF.Web.Services.Financeiro.Model.ContaCorrenteUI
                    {
                        cd_local_movimento = g.Key.cd_local_movimento,
                        des_local_movto = g.Key.des_local_movto,
                        vl_entrada = g.Where(d => d.dta_conta_corrente == dta_fechamento).Sum(y => y.vl_entrada),
                        vl_saida = g.Where(d => d.dta_conta_corrente == dta_fechamento).Sum(y => y.vl_saida)
                    }).ToList();

                    if (fechamento.Count() > 0)
                    {
                        foreach (ContaCorrenteUI f in fechamento)
                        {
                            //sf.dta_conta_corrente = rptContas.Where(x => x.cd_conta_corrente == sf.cd_conta_corrente).FirstOrDefault().dta_conta_corrente;
                            //ContaCorrente existeMovtoAberto = DataAccessContaCorrente.existAberturaSaldoData(dt_base, cd_pessoa_escola, 0, 0);
                            //if (existeMovtoAberto != null)
                            //    throw new FinanceiroBusinessException(string.Format(Messages.msgErroSaldoAbertura, String.Format("{0:dd-MM-yyyy}", existeMovtoAberto.dta_conta_corrente)), null, FinanceiroBusinessException.TipoErro.ERRO_ABERTURA_SALDO, false);
                            f.saldo_inicial = DataAccessContaCorrente.fcSaldoContaCorrente(cd_pessoa_escola, (int)f.cd_local_movimento, dta_fechamento, tipoLiquidacao);
                            if (cdUsuario > 0)
                                f.obsSaldoCaixa = DataAccessObsSaldoCaixa.getObsSaldoCaixaUsuario(cd_pessoa_escola, (int)f.cd_local_movimento, dta_fechamento, cdUsuario);

                            if (f.vl_entrada != null && f.vl_saida != null)
                                f.saldo_final = (f.saldo_inicial + (decimal)f.vl_entrada) - (decimal)f.vl_saida;
                            else
                                if (f.vl_entrada != null)
                                f.saldo_final = (f.saldo_inicial + (decimal)f.vl_entrada);
                            else
                                if (f.vl_saida != null)
                                f.saldo_final = f.saldo_inicial - (decimal)f.vl_saida;
                        }
                        fechamento = fechamento.Where(f => f.saldo_inicial != 0 || f.vl_entrada != 0 || f.vl_saida != 0 || mostrarZerados).ToList();
                    }
                }
                transaction.Complete();
            }
            return fechamento;
        }

        public void postZerarSaldoFinanceiro(int cd_escola, int cd_tipo_liquidacao, Nullable<System.DateTime> dta_base, byte tipo)
        {
            DataAccessContaCorrente.postZerarSaldoFinanceiro(cd_escola, cd_tipo_liquidacao, dta_base, tipo);
        }

        public ObsSaldoCaixa getObsSaldoCaixaConsolidado(int cdEscola, DateTime dt_saldo_caixa, int cd_usuario)
        {
            return DataAccessObsSaldoCaixa.getObsSaldoCaixaUsuario(cdEscola, 0, dt_saldo_caixa, cd_usuario);
        }

        public IEnumerable<ContaCorrenteUI> getFechamentoCaixaLocalMovtoRel(int cd_pessoa_escola, DateTime dta_fechamento, int cdUsuario, byte tipoLocal, bool mostrarZerados)
        {
            List<ContaCorrenteUI> retorno = new List<ContaCorrenteUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                IEnumerable<ContaCorrenteUI> fechamento = DataAccessContaCorrente.getFechamentoCaixaLocalMovtoRel(cd_pessoa_escola, dta_fechamento, cdUsuario, tipoLocal);
                if (fechamento != null)
                {
                    IEnumerable<ContaCorrenteUI> fechamentoTpLiquidacao = fechamento.GroupBy(x => new { x.cd_tipo_liquidacao, x.dc_tipo_liquidacao }).Select(g => new FundacaoFisk.SGF.Web.Services.Financeiro.Model.ContaCorrenteUI
                    {
                        cd_tipo_liquidacao = g.Key.cd_tipo_liquidacao,
                        dc_tipo_liquidacao = g.Key.dc_tipo_liquidacao
                    }).ToList();
                    foreach (ContaCorrenteUI cc in fechamentoTpLiquidacao)
                    {
                        var listaPorTpLiq = fechamento.Where(t => t.cd_tipo_liquidacao == cc.cd_tipo_liquidacao);
                        listaPorTpLiq = listaPorTpLiq.GroupBy(x => new { x.cd_local_movimento, x.des_local_movto, x.cd_tipo_liquidacao, x.dc_tipo_liquidacao }).Select(g => new FundacaoFisk.SGF.Web.Services.Financeiro.Model.ContaCorrenteUI
                        {
                            cd_local_movimento = g.Key.cd_local_movimento,
                            des_local_movto = g.Key.des_local_movto,
                            cd_tipo_liquidacao = g.Key.cd_tipo_liquidacao,
                            dc_tipo_liquidacao = g.Key.dc_tipo_liquidacao,
                            vl_entrada = g.Where(d => d.dta_conta_corrente == dta_fechamento).Sum(y => y.vl_entrada),
                            vl_saida = g.Where(d => d.dta_conta_corrente == dta_fechamento).Sum(y => y.vl_saida)
                        }).ToList();

                        if (listaPorTpLiq.Count() > 0)
                        {
                            foreach (ContaCorrenteUI f in listaPorTpLiq)
                            {
                                //sf.dta_conta_corrente = rptContas.Where(x => x.cd_conta_corrente == sf.cd_conta_corrente).FirstOrDefault().dta_conta_corrente;
                                //ContaCorrente existeMovtoAberto = DataAccessContaCorrente.existAberturaSaldoData(dt_base, cd_pessoa_escola, 0, 0);
                                //if (existeMovtoAberto != null)
                                //    throw new FinanceiroBusinessException(string.Format(Messages.msgErroSaldoAbertura, String.Format("{0:dd-MM-yyyy}", existeMovtoAberto.dta_conta_corrente)), null, FinanceiroBusinessException.TipoErro.ERRO_ABERTURA_SALDO, false);
                                f.saldo_inicial = DataAccessContaCorrente.fcSaldoContaCorrente(cd_pessoa_escola, (int)f.cd_local_movimento, dta_fechamento, cc.cd_tipo_liquidacao);
                                if (cdUsuario > 0)
                                {
                                    f.tx_obs_saldo_caixa = DataAccessObsSaldoCaixa.getObsSaldoCaixaUsuario(cd_pessoa_escola, (int)f.cd_local_movimento, dta_fechamento, cdUsuario) != null ?
                                        DataAccessObsSaldoCaixa.getObsSaldoCaixaUsuario(cd_pessoa_escola, (int)f.cd_local_movimento, dta_fechamento, cdUsuario).tx_obs_saldo_caixa : "";
                                }

                                if (f.vl_entrada != null && f.vl_saida != null)
                                    f.saldo_final = (f.saldo_inicial + (decimal)f.vl_entrada) - (decimal)f.vl_saida;
                                else
                                    if (f.vl_entrada != null)
                                        f.saldo_final = (f.saldo_inicial + (decimal)f.vl_entrada);
                                    else
                                        if (f.vl_saida != null)
                                            f.saldo_final = f.saldo_inicial - (decimal)f.vl_saida;
                            }
                        }
                        retorno = retorno.Union(listaPorTpLiq).ToList();
                        retorno = retorno.Where(f => f.saldo_inicial != 0 || f.vl_entrada != 0 || f.vl_saida != 0 || mostrarZerados).ToList();
                    }
                }
                transaction.Complete();
            }
            return retorno;
        }

        #endregion

        #region Balancete

        public IEnumerable<RptBalanceteMensal> getBalanceteMensal(int cdEscola, int mes, int ano, int nivel, int nivel_analisar, bool mostrar_contas, bool conta_segura)
        {
            List<RptBalanceteMensal> retorno = new List<RptBalanceteMensal>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                try
                {
                    DateTime data_inicial = new DateTime(ano, mes, 1);
                    DateTime data_final = data_inicial.AddMonths(1);
                    data_final = data_final.AddDays(-1);

                    //Busca as contas correntes por subgrupo de contas:
                    List<SubgrupoConta> listaSubgrupo = new List<SubgrupoConta>();
                    if (nivel == 1)
                        listaSubgrupo = DataAccessContaCorrente.getGruposSemContaCorrenteN1(data_inicial, data_final, cdEscola, conta_segura).ToList();
                    else
                        listaSubgrupo = DataAccessContaCorrente.getGruposSemContaCorrenteN2(data_inicial, data_final, cdEscola, conta_segura).ToList();

                    //Transforma a lista de subgrupos na lista de grupos com os subgrupos e já ordena eles:
                    List<GrupoConta> gruposConta = new List<GrupoConta>();
                    foreach (SubgrupoConta subGrupo in listaSubgrupo)
                        if (nivel == 1)
                            incluiRecursivoGrupoContasN1(subGrupo, gruposConta);
                        else
                            incluiRecursivoGrupoContas(subGrupo, gruposConta);

                    //Transforma a lista de grupos na lista de objetos do relatório de balancete:
                    gruposConta = gruposConta.OrderBy(gc => gc.nm_ordem_grupo).ThenBy(gc => gc.id_tipo_grupo_conta).ThenBy(gc => gc.no_grupo_conta).ToList();
                    foreach (GrupoConta grupoConta in gruposConta)
                    {
                        grupoConta.SubGrupos = grupoConta.SubGrupos.OrderBy(sg => sg.nm_ordem_subgrupo).ThenBy(sg => sg.no_subgrupo_conta).ToList();

                        ContaCorrente existeMovtoAberto = DataAccessContaCorrente.existAberturaSaldoData(data_inicial, cdEscola, 0, 0);
                        grupoConta.vl_saldo_anterior = DataAccessContaCorrente.buscaSaldoAnteriorGrupo(cdEscola, grupoConta.cd_grupo_conta, data_inicial.AddDays(-1));

                        if (conta_segura || ((nivel == 1 && (grupoConta.SubGrupos.Where(sg => sg.SubgrupoPlanoConta.Count == 0 || sg.SubgrupoPlanoConta.Where(spc => !spc.id_conta_segura).Any()).Any()))
                                            || (nivel == 2 && (grupoConta.SubGrupos.Where(sg => sg.SubgruposFilhos == null || sg.SubgruposFilhos.Count() == 0
                                                || sg.SubgruposFilhos.Where(sf => sf.SubgrupoPlanoConta.Where(spc => !spc.id_conta_segura).Any()).Any()).Any()))))
                        {
                            if (mostrar_contas || (grupoConta.vl_credito_conta > 0 || grupoConta.vl_debito_conta > 0 || grupoConta.vl_saldo > 0 || grupoConta.vl_saldo_anterior > 0))
                                retorno.Add(transformaRptBalancete(grupoConta, grupoConta.vl_credito_conta, grupoConta.vl_debito_conta, grupoConta.vl_saldo, grupoConta.vl_saldo_anterior));

                            if (nivel_analisar > 0)
                                foreach (SubgrupoConta subGrupo in grupoConta.SubGrupos)
                                {
                                    subGrupo.no_subgrupo_conta = "  " + subGrupo.no_subgrupo_conta;
                                    subGrupo.vl_saldo_anterior = DataAccessContaCorrente.buscaSaldoAnteriorSubGrupo(cdEscola, subGrupo.cd_subgrupo_conta, data_inicial.AddDays(-1));
                                    if (conta_segura || (nivel == 1 && (subGrupo.SubgrupoPlanoConta.Where(spc => !spc.id_conta_segura).Any() || subGrupo.SubgrupoPlanoConta.Count == 0)
                                                        || (nivel == 2 && subGrupo.SubgruposFilhos.Where(sf => sf.SubgrupoPlanoConta.Count == 0 || sf.SubgrupoPlanoConta.Where(spc => !spc.id_conta_segura).Any()).Any())))
                                    {
                                        if (mostrar_contas || (subGrupo.vl_credito_conta > 0 || subGrupo.vl_debito_conta > 0 || subGrupo.vl_saldo > 0 || subGrupo.vl_saldo_anterior > 0))
                                            retorno.Add(transformaRptBalancete(subGrupo, subGrupo.vl_credito_conta, subGrupo.vl_debito_conta, subGrupo.vl_saldo, subGrupo.vl_saldo_anterior, 1));

                                        if (nivel_analisar > 1)
                                        {
                                            subGrupo.SubgruposFilhos = subGrupo.SubgruposFilhos.OrderBy(sg => sg.nm_ordem_subgrupo).ThenBy(sg => sg.no_subgrupo_conta).ToList();
                                            foreach (SubgrupoConta subGrupo2 in subGrupo.SubgruposFilhos)
                                            {
                                                subGrupo2.no_subgrupo_conta = "    " + subGrupo2.no_subgrupo_conta;
                                                subGrupo2.vl_saldo_anterior = DataAccessContaCorrente.buscaSaldoAnteriorSubGrupo(cdEscola, subGrupo2.cd_subgrupo_conta, data_inicial.AddDays(-1));

                                                if (conta_segura || (subGrupo2.SubgrupoPlanoConta.Where(spc => !spc.id_conta_segura).Any() || subGrupo2.SubgrupoPlanoConta.Count == 0))
                                                    if (mostrar_contas || (subGrupo2.vl_credito_conta > 0 || subGrupo2.vl_debito_conta > 0 || subGrupo2.vl_saldo > 0 || subGrupo2.vl_saldo_anterior > 0))
                                                        retorno.Add(transformaRptBalancete(subGrupo2, subGrupo2.vl_credito_conta, subGrupo2.vl_debito_conta, subGrupo2.vl_saldo, subGrupo2.vl_saldo_anterior, 2));
                                            }
                                        }
                                    }
                                }
                        }
                    }
                }
                catch (System.ArgumentOutOfRangeException ae)
                {
                    string parametros = "Parâmetros: cdEscola: " + cdEscola + " mes:" + mes + " ano:" + ano + " nivel:" + nivel + " nivel_analisar:" + nivel_analisar + " mostrar_contas:" + mostrar_contas + " conta_segura:" + conta_segura;
                    throw new ArgumentOutOfRangeException(parametros, ae);
                }
                transaction.Complete();
            }
            return retorno;
        }

        private RptBalanceteMensal transformaRptBalancete(GrupoConta grupoConta, decimal vl_credito_conta, decimal vl_debito_conta, decimal vl_saldo, decimal vl_saldo_anterior)
        {
            RptBalanceteMensal rptBalancete = new RptBalanceteMensal();

            rptBalancete.Conta = grupoConta.no_grupo_conta;
            rptBalancete.Anterior = vl_saldo_anterior;
            rptBalancete.Debito = vl_debito_conta;
            rptBalancete.Credito = vl_credito_conta;
            rptBalancete.Saldo = vl_saldo;
            rptBalancete.Nivel = 0;

            return rptBalancete;
        }

        private RptBalanceteMensal transformaRptBalancete(SubgrupoConta subGrupo, decimal vl_credito_conta, decimal vl_debito_conta, decimal vl_saldo, decimal vl_saldo_anterior, int nivel)
        {
            RptBalanceteMensal rptBalancete = new RptBalanceteMensal();

            rptBalancete.Conta = subGrupo.no_subgrupo_conta;
            rptBalancete.Anterior = vl_saldo_anterior;
            rptBalancete.Debito = vl_debito_conta;
            rptBalancete.Credito = vl_credito_conta;
            rptBalancete.Saldo = vl_saldo;
            rptBalancete.Nivel = nivel;

            return rptBalancete;
        }

        private void incluiRecursivoGrupoContas(SubgrupoConta subGrupoContas, List<GrupoConta> gruposConta)
        {
            if (subGrupoContas.SubgrupoPai == null)
            {
                if (subGrupoContas.SubgrupoContaGrupo != null && !gruposConta.Contains(subGrupoContas.SubgrupoContaGrupo, new FundacaoFisk.SGF.GenericModel.GrupoConta.GrupoContaComparer()))
                    gruposConta.Add(subGrupoContas.SubgrupoContaGrupo);
            }
            else
                incluiRecursivoGrupoContas(subGrupoContas.SubgrupoPai, gruposConta);
        }

        private void incluiRecursivoGrupoContasN1(SubgrupoConta subGrupoContas, List<GrupoConta> gruposConta)
        {
            if (subGrupoContas.SubgrupoPai == null)
            {
                if (subGrupoContas.SubgrupoContaGrupo != null)
                {
                    if (!gruposConta.Contains(subGrupoContas.SubgrupoContaGrupo, new FundacaoFisk.SGF.GenericModel.GrupoConta.GrupoContaComparer()))
                    {
                        subGrupoContas.SubgrupoContaGrupo.SubGrupos.Add(subGrupoContas);
                        gruposConta.Add(subGrupoContas.SubgrupoContaGrupo);
                    }
                    else
                        gruposConta.Where(gc => gc.cd_grupo_conta == subGrupoContas.cd_grupo_conta).First().SubGrupos.Add(subGrupoContas);
                }
            }
            else
                incluiRecursivoGrupoContasN1(subGrupoContas.SubgrupoPai, gruposConta);
        }
        #endregion

        #region Fechamento de Estoque
        public IEnumerable<Fechamento> getFechamentoSearch(SearchParameters parametros, int? ano, int? mes, bool balanco, DateTime? dta_ini, DateTime? dta_fim, int cd_escola)
        {
            IEnumerable<Fechamento> retorno = new List<Fechamento>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "dh_fechamento";
                parametros.sort = parametros.sort.Replace("dtf_fechamento", "dh_fechamento");
                parametros.sort = parametros.sort.Replace("balanco", "id_balanco");
                parametros.sort = parametros.sort.Replace("no_usuario", "Usuario.no_login");
                retorno = DataAccessFechamento.getFechamentoSearch(parametros, ano, mes, balanco, dta_ini, dta_fim, cd_escola);
                transaction.Complete();
            }
            return retorno;
        }

        public Fechamento getFechamentoById(int cd_fechamento, int cd_escola)
        {
            Fechamento fech = new Fechamento();
            fech = DataAccessFechamento.getFechamentoById(cd_fechamento, cd_escola);
            fech.SaldosItens = DataAccessSaldoItem.getSaldoItemById(cd_fechamento, cd_escola).ToList();
            return fech;
        }

        public bool existeFechamentoAnoMes(DateTime data, int cd_escola, int cd_fechamento)
        {
            return DataAccessFechamento.existeFechamentoAnoMes(data, cd_escola, cd_fechamento);
        }
        public bool existeFechamentoSuperior(DateTime data, int cd_escola, int cd_fechamento)
        {
            return DataAccessFechamento.existeFechamentoSuperior(data, cd_escola, cd_fechamento);
        }

        public Fechamento postFechamento(Fechamento fechamento)
        {
            Fechamento retornar = new Fechamento();
            SGFWebContext cdb = new SGFWebContext();
            DataAccessKardex.sincronizaContexto(DataAccessFechamento.DB());
            DataAccessItemEscola.sincronizaContexto(DataAccessFechamento.DB());
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                if (fechamento.SaldosItens == null || fechamento.SaldosItens.Count() <= 0)
                    throw new FechamentoBusinessException(Utils.Messages.Messages.msgErroFechamentoSemItem, null, FechamentoBusinessException.TipoErro.ERRO_SEM_ITEM_FECHAMENTO, false);

                //bool existeAnoMesIgual = existeFechamentoAnoMes(fechamento.nm_ano_fechamento, fechamento.nm_mes_fechamento, fechamento.cd_pessoa_empresa, 0);
                bool existeAnoMesIgual = existeFechamentoAnoMes(fechamento.dt_fechamento, fechamento.cd_pessoa_empresa, 0);
                if (existeAnoMesIgual)
                    throw new FechamentoBusinessException(Utils.Messages.Messages.msgErroFechamentoExistente, null, FechamentoBusinessException.TipoErro.ERRO_FECHAMENTO_EXISTENTE, false);
                //bool existeAnoMesSup = existeFechamentoSuperior(fechamento.nm_ano_fechamento, fechamento.nm_mes_fechamento, fechamento.cd_pessoa_empresa, 0);
                bool existeAnoMesSup = existeFechamentoSuperior(fechamento.dt_fechamento, fechamento.cd_pessoa_empresa, 0);
                if (existeAnoMesSup)
                    throw new FechamentoBusinessException(Utils.Messages.Messages.msgErroFechamentoSup, null, FechamentoBusinessException.TipoErro.ERRO_FECHAMENTO_SUPERIOR, false);
                fechamento = DataAccessFechamento.add(fechamento, false);
                DataAccessFechamento.saveChanges(false);
                if (fechamento.SaldosItens != null)
                    crudSaldoItensFechamento(fechamento.SaldosItens.ToList(), fechamento);
                transaction.Complete();
            }
            retornar = DataAccessFechamento.getFechamentoById(fechamento.cd_fechamento, fechamento.cd_pessoa_empresa);
            return retornar;
        }

        public Fechamento postAlterarFechamento(Fechamento fechamento)
        {
            Fechamento retornar = new Fechamento();
            sincronizarContextos(DataAccessKardex.DB());
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                //Se tiver fechamento superior não deve permitir alterar nada
                //bool existeAnoMesSup = existeFechamentoSuperior(fechamento.nm_ano_fechamento, fechamento.nm_mes_fechamento, fechamento.cd_pessoa_empresa, fechamento.cd_fechamento);
                bool existeAnoMesSup = existeFechamentoSuperior(fechamento.dt_fechamento, fechamento.cd_pessoa_empresa, fechamento.cd_fechamento);
                if (existeAnoMesSup)
                    throw new FechamentoBusinessException(Utils.Messages.Messages.msgErroFechamentoSup, null, FechamentoBusinessException.TipoErro.ERRO_FECHAMENTO_SUPERIOR, false);


                if (fechamento.SaldosItens == null || fechamento.SaldosItens.Count() <= 0)
                    throw new FechamentoBusinessException(Utils.Messages.Messages.msgErroFechamentoSemItem, null, FechamentoBusinessException.TipoErro.ERRO_SEM_ITEM_FECHAMENTO, false);

                //verificar se alterou ano e mês, caso alterou só deve permitir salvar caso não exista itens na t_saldo_item
                Fechamento fechamentoBD = DataAccessFechamento.getFechById(fechamento.cd_fechamento, fechamento.cd_pessoa_empresa);
                //if ((fechamentoBD.nm_ano_fechamento != fechamento.nm_ano_fechamento || fechamentoBD.nm_mes_fechamento != fechamento.nm_mes_fechamento)
                if ((fechamentoBD.dt_fechamento != fechamento.dt_fechamento)
                    && (fechamento.SaldosItens != null && fechamento.SaldosItens.Count() > 0))
                    throw new FechamentoBusinessException(Utils.Messages.Messages.msgErroAlterarFechamento, null, FechamentoBusinessException.TipoErro.ERRO_ALTERAR_FECHAMENTO, false);

                //Data do cadastro só será alterada qnd alterado ano, mês e balanço
                //if (fechamentoBD.nm_ano_fechamento != fechamento.nm_ano_fechamento || fechamentoBD.nm_mes_fechamento != fechamento.nm_mes_fechamento || fechamentoBD.id_balanco != fechamento.id_balanco)
                if (fechamentoBD.dt_fechamento != fechamento.dt_fechamento || fechamentoBD.id_balanco != fechamento.id_balanco)
                {
                    //se for alterar, verificar se não existe registro igual ou maior ao alterado
                    //bool existeAnoMesIgual = existeFechamentoAnoMes(fechamento.nm_ano_fechamento, fechamento.nm_mes_fechamento, fechamento.cd_pessoa_empresa, fechamento.cd_fechamento);
                    bool existeAnoMesIgual = existeFechamentoAnoMes(fechamento.dt_fechamento, fechamento.cd_pessoa_empresa, fechamento.cd_fechamento);
                    if (existeAnoMesIgual)
                        throw new FechamentoBusinessException(Utils.Messages.Messages.msgErroFechamentoExistente, null, FechamentoBusinessException.TipoErro.ERRO_FECHAMENTO_EXISTENTE, false);

                    fechamentoBD.dh_fechamento = fechamento.dh_fechamento;
                }
                fechamentoBD = Fechamento.changeValuesFechamento(fechamento, fechamentoBD);
                DataAccessFechamento.saveChanges(false);
                if (fechamento.SaldosItens != null)
                    crudSaldoItensFechamento(fechamento.SaldosItens.ToList(), fechamento);
                transaction.Complete();
            }
            return DataAccessFechamento.getFechamentoById(fechamento.cd_fechamento, fechamento.cd_pessoa_empresa);
        }

        public void crudSaldoItensFechamento(List<SaldoItem> saldoItensView, Fechamento fechamento)
        {
            //Editar Saldo Itens
            SGFWebContext cdb = new SGFWebContext();
            //Guardando saldo itens
            IEnumerable<SaldoItem> saldosBD = DataAccessSaldoItem.getSaldoItemById(fechamento.cd_fechamento, fechamento.cd_pessoa_empresa);//fechamentoBD.SaldosItens;
            DateTime dataLimite = new DateTime(fechamento.nm_ano_fechamento, fechamento.nm_mes_fechamento, DateTime.DaysInMonth(fechamento.nm_ano_fechamento, fechamento.nm_mes_fechamento));
            List<SaldoItem> insertSaldosItens = saldoItensView.Where(si => !saldosBD.Any(sibd => sibd.cd_item == si.cd_item)).ToList();
            List<SaldoItem> deleteSaldosItens = saldosBD.Where(si => !saldoItensView.Any(sibd => sibd.cd_item == si.cd_item)).ToList();
            List<SaldoItem> editSaldosItens = saldoItensView.Where(si => !insertSaldosItens.Any(sibd => sibd.cd_item == si.cd_item) && si.editado).ToList();


            if (deleteSaldosItens.Count > 0)
            {
                if (deleteSaldosItens.Count() == saldosBD.Count())
                {
                    var ret = DataAccessSaldoItem.processarSaldoItens(fechamento.cd_fechamento, (int)SaldoItem.OperacaoProcedure.EXCLUIR);//1-Excluir

                    if (!ret)
                    {
                        throw new FechamentoBusinessException(Utils.Messages.Messages.msgErroProcedureExclusaoSaldoItem, null, FechamentoBusinessException.TipoErro.ERRO_PROCEDURE_EXCLUSAO_SALDO_ITEM, false);
                    }
                }
                else
                {
                    for (int i = deleteSaldosItens.Count - 1; i >= 0; i--)
                    {

                        //var qtd_item_estoque = 0;
                        //qtd_item_estoque = this.CalculaQtdItemEstoque(qtd_item_estoque, null, deleteSaldosItens[i].cd_item, fechamento.cd_pessoa_empresa);

                        //ItemEscola itemEscola = DataAccessItemEscola.findItemEscolabyId(deleteSaldosItens[i].cd_item, fechamento.cd_pessoa_empresa);
                        //itemEscola.qt_estoque = qtd_item_estoque;
                        //A trigger td_t_saldo_item vai excluir o kardex e recalcular o kardex
                        SaldoItem saldoDelete = DataAccessSaldoItem.findById(deleteSaldosItens[i].cd_saldo_item, false);
                        DataAccessSaldoItem.delete(saldoDelete, false);
                    }
                }
                
            }

            if (insertSaldosItens.Count > 0)
            {

                foreach (var saldoItem in insertSaldosItens)
                {
                    DataAccessSaldoItem.add(saldoItem, false);
                    //Calculado pela procedure de fechamento
                    //var qtd_item_estoque = 0;

                    //qtd_item_estoque = this.CalculaQtdItemEstoque(qtd_item_estoque, null, saldoItem.cd_item, fechamento.cd_pessoa_empresa);

                    ////Refaz o cálculo da quantidade do item:
                    //ItemEscola itemEscola = DataAccessItemEscola.findItemEscolabyId(saldoItem.cd_item, fechamento.cd_pessoa_empresa);
                    //itemEscola.qt_estoque = qtd_item_estoque;
                    //itemEscola.vl_item = saldoItem.vl_venda_fechamento;
                    //itemEscola.vl_custo = saldoItem.vl_custo_fechamento;
                }

                
            }

            if (editSaldosItens.Count > 0)
            {

                foreach (var saldoItem in editSaldosItens)
                {
                    //var qtd_item_estoque = 0;

                    SaldoItem saldoItemContext = null;
                    if (saldoItem.cd_saldo_item <= 0)
                        saldoItemContext = DataAccessSaldoItem.getSaldoItemByIdItem(fechamento.cd_fechamento, saldoItem.cd_item);
                    else
                        saldoItemContext = DataAccessSaldoItem.findById(saldoItem.cd_saldo_item, false);

                    //qtd_item_estoque = this.CalculaQtdItemEstoque(qtd_item_estoque, null, saldoItem.cd_item, fechamento.cd_pessoa_empresa);

                    ////Refaz o cálculo da quantidade do item:
                    //ItemEscola itemEscola = DataAccessItemEscola.findItemEscolabyId(saldoItem.cd_item, fechamento.cd_pessoa_empresa);
                    //itemEscola.qt_estoque = qtd_item_estoque;
                    //itemEscola.vl_item = saldoItem.vl_venda_fechamento;
                    //itemEscola.vl_custo = saldoItem.vl_custo_fechamento;

                    saldoItemContext.qt_saldo_atual = saldoItem.qt_saldo_atual;
                    saldoItemContext.qt_saldo_data = saldoItem.qt_saldo_data;
                    saldoItemContext.qt_saldo_fechamento = saldoItem.qt_saldo_fechamento;
                    saldoItemContext.vl_custo_fechamento = saldoItem.vl_custo_fechamento;
                    saldoItemContext.vl_custo_atual = saldoItem.vl_custo_atual;
                    saldoItemContext.vl_venda_fechamento = saldoItem.vl_venda_fechamento;
                    saldoItemContext.vl_venda_atual = saldoItem.vl_venda_atual;
                }

                
            }
            //DataAccessItemEscola.saveChanges(false);
            DataAccessSaldoItem.saveChanges(false);

            if ((insertSaldosItens.Count > 0) || (editSaldosItens.Count > 0))
            {
                var ret = DataAccessSaldoItem.processarSaldoItens(fechamento.cd_fechamento, (int)SaldoItem.OperacaoProcedure.INCLUIR);//0-Incluir Aqui tanto faz pois vai excluir e incluir kardex novamente
                if (!ret)
                {
                    if (insertSaldosItens.Count > 0)
                            throw new FechamentoBusinessException(Utils.Messages.Messages.msgErroProcedureInclusaoSaldoItem, null, FechamentoBusinessException.TipoErro.ERRO_PROCEDURE_INCLUSAO_SALDO_ITEM, false);

                    if (editSaldosItens.Count > 0)
                            throw new FechamentoBusinessException(Utils.Messages.Messages.msgErroProcedureAlteracaoSaldoItem, null, FechamentoBusinessException.TipoErro.ERRO_PROCEDURE_ALTERACAO_SALDO_ITEM, false);
                }
            }          

        }

        private int CalculaQtdItemEstoque(int qtd_item_estoque, DateTime? dataLimite, int cd_item, int cd_pessoa_empresa)
        {
            List<SaldoFinanceiro> kardexList = DataAccessKardex.getFechamentoKardex(dataLimite, cd_item, cd_pessoa_empresa);

            var listaSaldosKardex = kardexList.GroupBy(x => new { x.id_tipo_movimento }).Select(g => new FundacaoFisk.SGF.Web.Services.Financeiro.Model.SaldoFinanceiro
            {
                entrada = g.Sum(y => y.entrada),
                saida = g.Sum(y => y.saida),
            }).ToList();

            if (listaSaldosKardex.Count > 0 && listaSaldosKardex.FirstOrDefault() != null)
                qtd_item_estoque = (int)(listaSaldosKardex.FirstOrDefault().entrada - listaSaldosKardex.FirstOrDefault().saida);

            return qtd_item_estoque;
        }

        public IEnumerable<SaldoItem> postGerarEstoque(Fechamento fechamento, int cd_escola)
        {
            //Calcula a última data do mês de fechamento:
            //DateTime dataLimite = new DateTime(fechamento.nm_ano_fechamento, fechamento.nm_mes_fechamento, DateTime.DaysInMonth(fechamento.nm_ano_fechamento, fechamento.nm_mes_fechamento));
            DateTime dataLimite = fechamento.dt_fechamento;
            if (fechamento.itensFechamento != null)
                foreach (Item item in fechamento.itensFechamento)
                {
                    //Transforma um item em saldo de item:
                    SaldoItem saldoItem = new SaldoItem { cd_item = item.cd_item, no_item = item.no_item, qt_saldo_atual = item.qt_estoque.HasValue ? item.qt_estoque.Value : 0, cd_grupo_estoque = item.cd_grupo_estoque, cd_tipo_item = item.cd_tipo_item, vl_custo_atual = item.vl_custo, vl_custo_fechamento = item.vl_custo, vl_venda_atual = item.vl_item, vl_venda_fechamento = item.vl_item, id_movto_estoque = item.id_movto_estoque };

                    //Calcula o valor do saldo do item:
                    saldoItem.qt_saldo_data = saldoItem.qt_saldo_fechamento = DataAccessKardex.getSaldoItem(item.cd_item, dataLimite, cd_escola);
                    saldoItem.vl_custo_fechamento = saldoItem.qt_saldo_data != 0 ? Math.Round((decimal)DataAccessKardex.getSaldoValorItem(item.cd_item, dataLimite, cd_escola)/saldoItem.qt_saldo_data,2, MidpointRounding.AwayFromZero) : 0;
                    fechamento.SaldosItens.Add(saldoItem);
                }

            return fechamento.SaldosItens;
        }

        public IEnumerable<Fechamento> fechamentoAnoMes(int cd_escola)
        {
            return DataAccessFechamento.fechamentoAnoMes(cd_escola);
        }

        public SaldoItem getSaldoItemLocal(SaldoItem saldos)
        {

            List<SaldoItem> result = saldos.listaSaldo;
            if (saldos.cd_grupo_estoque > 0)
                result = result.Where(s => s.cd_grupo_estoque == saldos.cd_grupo_estoque).ToList();
            if (saldos.cd_tipo_item > 0)
                result = result.Where(s => s.cd_tipo_item == saldos.cd_tipo_item).ToList();
            if (!String.IsNullOrEmpty(saldos.no_item))
                if (saldos.inicio)
                    result = result.Where(s => s.no_item.ToLower().StartsWith(saldos.no_item.ToLower())).ToList();
                else
                    result = result.Where(s => s.no_item.ToLower().Contains(saldos.no_item.ToLower())).ToList();
            saldos.listaSaldo = result;
            return saldos;
        }
        //Disparado quando exclui fechamento todo
        public bool deleteFechamentos(List<int> cdFechamentos, int cd_empresa)
        {
            bool retorno = false;
            SGFWebContext cdb = new SGFWebContext();
            DataAccessKardex.sincronizaContexto(DataAccessFechamento.DB());
            DataAccessItemEscola.sincronizaContexto(DataAccessFechamento.DB());
            DataAccessSaldoItem.sincronizaContexto(DataAccessFechamento.DB());

            if (cdFechamentos != null && cdFechamentos.Count() > 0)
            {
                List<Fechamento> fechamentos = DataAccessFechamento.getFechamentos(cdFechamentos, cd_empresa).ToList();
                using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED, DataAccessFechamento.DB(), TransactionScopeBuilder.TransactionTime.MODEREATE))
                {
                    foreach (var fechamento in fechamentos)
                    {
                        //verificar se não existe registro maior ao deletado. Como agora gera kardex somente a diferença vai travar somente para imobilizado
                        //bool existeAnoMesSup = existeFechamentoSuperior(fechamento.nm_ano_fechamento, fechamento.nm_mes_fechamento, cd_empresa, fechamento.cd_fechamento);
                        bool existeAnoMesSup = existeFechamentoSuperior(fechamento.dt_fechamento, cd_empresa, fechamento.cd_fechamento);
                        if (existeAnoMesSup)
                            throw new FechamentoBusinessException(Utils.Messages.Messages.msgErroFechamentoSup, null, FechamentoBusinessException.TipoErro.ERRO_FECHAMENTO_SUPERIOR, false);

                        crudSaldoItensFechamento(new List<SaldoItem>(), fechamento);
                        retorno = true;
                        //retorno = DataAccessFechamento.delete(fechamento, false);
                    }
                    transaction.Complete();
                }
            }
            return retorno;
        }
        
        public Fechamento getFechamentoByDta(DateTime data, int cd_escola)
        {
            return DataAccessFechamento.getFechamentoByDta(data, cd_escola);
        }

        #endregion

        #region Subgrupo Item

        public IEnumerable<ItemSubgrupo> getSubGrupoTpHasItem(int cd_item)
        {
            return DataAccessItemSubgrupo.getSubGrupoTpHasItem(cd_item);
        }
        public ItemSubgrupo getSubGrupoPlano(int cd_item, byte tipo, int cdEscola)
        {
            return DataAccessItemSubgrupo.getSubGrupoPlano(cd_item, tipo, cdEscola);
        }
        #endregion

        #region Situação Tributaria

        public IEnumerable<SituacaoTributaria> getSituacaoTributaria(SituacaoTributariaDataAccess.TipoConsultaSitTribEnum tipo, List<int> cd_situacoes, int cdTpNF)
        {
            return DataAccessSituacaoTributaria.getSituacaoTributaria(tipo, cd_situacoes, cdTpNF);
        }

        public SituacaoTributaria getSituacaoTributariaItem(int cd_grupo_estoque, int id_regime_tributario, int cdSitTrib)
        {
            return DataAccessSituacaoTributaria.getSituacaoTributariaItem(cd_grupo_estoque, id_regime_tributario, cdSitTrib);
        }
        public IEnumerable<SituacaoTributaria> getSituacaoTributariaTipo(SituacaoTributariaDataAccess.TipoConsultaSitTribEnum tipo, List<int> cd_situacoes, int tipoImp, int cd_escola,
            byte id_regime_trib, bool master_geral)
        {
            return DataAccessSituacaoTributaria.getSituacaoTributariaTipo(tipo, cd_situacoes, tipoImp, cd_escola, id_regime_trib, master_geral);
        }
        public SituacaoTributaria getSituacaoTributariaFormaTrib(int cd_situacao_trib)
        {
            return DataAccessSituacaoTributaria.getSituacaoTributariaFormaTrib(cd_situacao_trib);
        }
        #endregion

        #region Aliquota NF
        public IEnumerable<AliquotaUF> getAliquotaUFSearch(SearchParameters parametros, int cdEstadoOri, int cdEstadoDest, double? aliquota)
        {
            IEnumerable<AliquotaUF> retorno = new List<AliquotaUF>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "cd_localidade_estado_origem";
                parametros.sort = parametros.sort.Replace("no_estado_origem", "EstadoOrigem.Localidade.no_localidade");
                parametros.sort = parametros.sort.Replace("no_estado_destino", "EstadoDestino.Localidade.no_localidade");
                parametros.sort = parametros.sort.Replace("aliquotaICMS", "pc_aliq_icms_padrao");
                retorno = DataAccessAliquotaUF.getAliquotaUFSearch(parametros, cdEstadoOri, cdEstadoDest, aliquota);
                transaction.Complete();
            }
            return retorno;
        }

        public AliquotaUF getEstadosPesq()
        {
            AliquotaUF aliquota = new AliquotaUF();
            aliquota.estadosDesPes = DataAccessAliquotaUF.getEstadoDest().ToList();
            aliquota.estadosOriPes = DataAccessAliquotaUF.getEstadoOri().ToList();
            return aliquota;
        }
       
        public AliquotaUF postAliquotaUF(AliquotaUF aliquota)
        {
            AliquotaUF retornar = new AliquotaUF();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {

                aliquota = DataAccessAliquotaUF.add(aliquota, false);
                retornar = DataAccessAliquotaUF.getAliquotaUFById(aliquota.cd_aliquota_uf);
                transaction.Complete();
            }

            return retornar;
        }

        public AliquotaUF getAliquotaUFByOriDes(int cdEstadoOri, int cdEstadoDest)
        {
            return DataAccessAliquotaUF.getAliquotaUFByOriDes(cdEstadoOri, cdEstadoDest);
        }
        public AliquotaUF getAliquotaUFByEscDes(int cdEscola, int cdEstadoDest)
        {
            return DataAccessAliquotaUF.getAliquotaUFByEscDes(cdEscola, cdEstadoDest);
        }
        public AliquotaUF getAliquotaUFPorEstadoPessoa(int cdEscola, int cd_pessoa_cliente)
        {
            return DataAccessAliquotaUF.getAliquotaUFPorEstadoPessoa(cdEscola, cd_pessoa_cliente);
        }

        public AliquotaUF getAliquotaUFById(int cd_aliquota_uf)
        {
            return DataAccessAliquotaUF.findById(cd_aliquota_uf, false);
        }

        public bool posDeletetAliquotaUF(AliquotaUF aliquota)
        {
            DataAccessAliquotaUF.delete(aliquota, false);
            return true;
        }

        #endregion

        #region Dados NF
        public IEnumerable<DadosNF> getDadosNFSearch(SearchParameters parametros, int cdCidade, string natOp, double? aliquota, byte id_regime)
        {
            IEnumerable<DadosNF> retorno = new List<DadosNF>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "Localidade.no_localidade";
                parametros.sort = parametros.sort.Replace("no_cidade", "Localidade.no_localidade");

                retorno = DataAccessDadosNF.getDadosNFSearch(parametros, cdCidade, natOp, aliquota, id_regime);
                transaction.Complete();
            }
            return retorno;
        }
        public DadosNF getDadosNFById(int cdDadoNF)
        {
            return DataAccessDadosNF.findById(cdDadoNF, false);
        }
        public DadosNF postDadosNF(DadosNF dado)
        {
            DadosNF retornar = new DadosNF();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                bool existeCidade = getDadosCidade(dado.cd_cidade);
                if (existeCidade)
                    throw new FinanceiroBusinessException(Messages.msgErroCidadeDadosNF, null, FinanceiroBusinessException.TipoErro.ERRO_EXISTE_CIDADE_DADOS_NF, false);
                dado = DataAccessDadosNF.add(dado, false);
                retornar = DataAccessDadosNF.getDadosNFById(dado.cd_dados_nf);
                transaction.Complete();
            }

            return retornar;
        }
        public bool getDadosCidade(int cdCidade)
        {
            return DataAccessDadosNF.getDadosCidade(cdCidade);
        }
        public double? getISSEscola(int cdEscola)
        {
            double? aliquotaISS = DataAccessDadosNF.getISSEscola(cdEscola);
            return aliquotaISS;
        }

        public bool postDeleteDadosNF(DadosNF dado)
        {
            DadosNF retornar = new DadosNF();
            DataAccessDadosNF.delete(dado, false);
            return true;
        }

        #endregion

        #region Reajuste Anual

        public IEnumerable<ReajusteAnual> getReajusteAnualSearch(SearchParameters parametros, int cd_empresa, int cd_usuario, int status, DateTime? dtaInicial, DateTime? dtaFinal, bool cadastro, bool vctoInicial, int cd_reajuste_anual)
        {
            IEnumerable<ReajusteAnual> retorno = new List<ReajusteAnual>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "dh_cadastro_reajuste";
                parametros.sort = parametros.sort.Equals("dh_cadastro") ? "dh_cadastro_reajuste" : parametros.sort;
                parametros.sort = parametros.sort.Replace("no_login", "SysUsuario.no_login");
                parametros.sort = parametros.sort.Replace("pc_reajuste", "pc_reajuste_anual");
                if ("vl_reajuste".Equals(parametros.sort))
                    parametros.sort = parametros.sort.Replace("vl_reajuste", "vl_reajuste_anual");
                parametros.sort = parametros.sort.Replace("dt_inicial_vcto", "dt_inicial_vencimento");
                parametros.sort = parametros.sort.Replace("dt_final_vcto", "dt_final_vencimento");
                parametros.sort = parametros.sort.Replace("dc_status", "id_status_reajuste");

                retorno = DataAccessReajusteAnual.getReajusteAnualSearch(parametros, cd_empresa, cd_usuario, status, dtaInicial, dtaFinal, cadastro, vctoInicial, cd_reajuste_anual);
                transaction.Complete();
            }
            return retorno;
        }

        public ReajusteAnual addReajusteAnual(ReajusteAnual reajuste)
        {
            ReajusteAnual retornar = new ReajusteAnual();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                retornar = DataAccessReajusteAnual.add(reajuste, false);
                transaction.Complete();
            }

            return retornar;
        }

        public Boolean deleteAllReajuste(List<ReajusteAnual> reajustes, int cd_empresa)
        {
            bool retorno = false;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED, DataAccessReajusteAnual.DB()))
            {
                this.sincronizarContextos(DataAccessReajusteAnual.DB());
                foreach (ReajusteAnual reajuste in reajustes)
                {
                    ReajusteAnual reajusteContext = DataAccessReajusteAnual.getReajusteAnualFull(reajuste.cd_reajuste_anual, cd_empresa);
                    if (reajusteContext.id_status_reajuste != (int)ReajusteAnual.StatusReajuste.ABERTO)
                        throw new FinanceiroBusinessException(string.Format(Messages.msgErroReajusteFechado), null, FinanceiroBusinessException.TipoErro.ERRO_REAJUSTE_ANUAL_FECHADO, false);
                    List<ReajusteAluno> reajustesAlunos = reajusteContext.ReajustesAlunos.ToList();
                    List<ReajusteTurma> reajustesTurmas = reajusteContext.ReajustesTurmas.ToList();
                    List<ReajusteCurso> reajustesCursos = reajusteContext.ReajustesCursos.ToList();
                    for (int j = reajustesAlunos.Count()-1; j >= 0; j--)
                        retorno |= DataAccessReajusteAluno.deleteContext(reajustesAlunos[j], false);
                    for (int j = reajustesTurmas.Count() - 1; j >= 0; j--)
                        retorno |= DataAccessReajusteTurma.deleteContext(reajustesTurmas[j], false);
                    for (int j = reajustesCursos.Count() - 1; j >= 0; j--)
                        retorno |= DataAccessReajusteCurso.deleteContext(reajustesCursos[j], false);
                    retorno |= DataAccessReajusteAnual.deleteContext(reajusteContext, false);
                }
                DataAccessReajusteAnual.saveChanges(false);
                transaction.Complete();
            }
            return retorno;
        }

        public ReajusteAnual getReajusteAnualForEdit(int cd_empresa, int cd_reajuste_anual)
        {
            ReajusteAnual retorno = new ReajusteAnual();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED, DataAccessReajusteAnual.DB()))
            {
                retorno = DataAccessReajusteAnual.getReajusteAnualForEdit(cd_empresa, cd_reajuste_anual);
                transaction.Complete();
            }
            return retorno;
        }

        public ReajusteAnual postUpdateReajusteAnual(ReajusteAnual reajuste)
        {
            ReajusteAnual retornar = new ReajusteAnual();
            this.sincronizarContextos(DataAccessReajusteAnual.DB());
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED, DataAccessReajusteAnual.DB()))
            {
                crudReajusteAnual(reajuste);

                ReajusteAnual reajusteContext = DataAccessReajusteAnual.findById(reajuste.cd_reajuste_anual, false);
                if (reajusteContext.id_status_reajuste == (int)ReajusteAnual.StatusReajuste.FECHADO)
                    throw new FinanceiroBusinessException(string.Format(Messages.msgErroReajusteFechadoNotAlteracao), null, FinanceiroBusinessException.TipoErro.ERRO_REAJUSTE_ANUAL_FECHADO, false);
                reajusteContext.id_tipo_reajuste = reajuste.id_tipo_reajuste;
                reajusteContext.pc_reajuste_anual = reajuste.pc_reajuste_anual;
                reajusteContext.vl_reajuste_anual = reajuste.vl_reajuste_anual;
                reajusteContext.dt_inicial_vencimento = reajuste.dt_inicial_vencimento;
                reajusteContext.dt_final_vencimento = reajuste.dt_final_vencimento;
                reajusteContext.id_status_reajuste = reajuste.id_status_reajuste;
                reajusteContext.cd_nome_contrato = reajuste.cd_nome_contrato;
                //reajusteContext.dh_cadastro_reajuste = reajuste.dh_cadastro_reajuste;
                retornar = DataAccessReajusteAnual.edit(reajusteContext, false);
                transaction.Complete();
            }

            return retornar;
        }

        private void crudReajusteAnual(ReajusteAnual reajusteView){
            ReajusteAnual reajusteContext = DataAccessReajusteAnual.getReajusteAnualFull(reajusteView.cd_reajuste_anual, reajusteView.cd_pessoa_escola);
            
            // Reajustes Aluno:
            IEnumerable<ReajusteAluno> reajusteAlunoComCodigo = reajusteView.ReajustesAlunos.Where(ra => ra.cd_reajuste_aluno != 0);
            IEnumerable<ReajusteAluno> reajusteAlunoInsert = reajusteView.ReajustesAlunos.Where(ra => ra.cd_reajuste_aluno <= 0);
            List<ReajusteAluno> reajusteAlunoDeleted = reajusteContext.ReajustesAlunos.Where(pc => !reajusteAlunoComCodigo.Any(pv => pc.cd_reajuste_aluno == pv.cd_reajuste_aluno)).ToList();         
       
            if (reajusteAlunoDeleted != null && reajusteAlunoDeleted.Count() > 0)
                for (int i = reajusteAlunoDeleted.Count() - 1; i >= 0; i--)
                    DataAccessReajusteAluno.deleteContext(reajusteAlunoDeleted[i], false);
            if (reajusteAlunoInsert != null)
                foreach (var item in reajusteAlunoInsert)
                    if (item.cd_reajuste_aluno == 0)
                    {
                        item.cd_reajuste_anual = reajusteView.cd_reajuste_anual;
                        DataAccessReajusteAluno.addContext(item, false);
                    }

            DataAccessReajusteAnual.saveChanges(false);

            // Reajustes Turma:
            IEnumerable<ReajusteTurma> reajusteTurmaComCodigo = reajusteView.ReajustesTurmas.Where(ra => ra.cd_reajuste_turma != 0);
            IEnumerable<ReajusteTurma> reajusteTurmaInsert = reajusteView.ReajustesTurmas.Where(ra => ra.cd_reajuste_turma <= 0);
            List<ReajusteTurma> reajusteTurmaDeleted = reajusteContext.ReajustesTurmas.Where(pc => !reajusteTurmaComCodigo.Any(pv => pc.cd_reajuste_turma == pv.cd_reajuste_turma)).ToList();                

            if (reajusteTurmaDeleted != null && reajusteTurmaDeleted.Count() > 0)
                for (int i = reajusteTurmaDeleted.Count() - 1; i >= 0; i--)
                    DataAccessReajusteTurma.deleteContext(reajusteTurmaDeleted[i], false);
            if (reajusteTurmaInsert != null)
                foreach (var item in reajusteTurmaInsert)
                    if (item.cd_reajuste_turma == 0)
                    {
                        item.cd_reajuste_anual = reajusteView.cd_reajuste_anual;
                        DataAccessReajusteTurma.addContext(item, false);
                    }

            DataAccessReajusteAnual.saveChanges(false);

            // Reajustes Curso:
            IEnumerable<ReajusteCurso> reajusteCursoComCodigo = reajusteView.ReajustesCursos.Where(ra => ra.cd_reajuste_curso != 0);
            IEnumerable<ReajusteCurso> reajusteCursoInsert = reajusteView.ReajustesCursos.Where(ra => ra.cd_reajuste_curso <= 0);
            List<ReajusteCurso> reajusteCursoDeleted = reajusteContext.ReajustesCursos.Where(pc => !reajusteCursoComCodigo.Any(pv => pc.cd_reajuste_curso == pv.cd_reajuste_curso)).ToList();                
            if (reajusteCursoDeleted != null && reajusteCursoDeleted.Count() > 0)
                for (int i = reajusteCursoDeleted.Count() - 1; i >= 0; i--)
                    DataAccessReajusteCurso.deleteContext(reajusteCursoDeleted[i], false);
            if (reajusteCursoInsert != null)
                foreach (var item in reajusteCursoInsert)
                    if (item.cd_reajuste_curso == 0)
                    {
                        item.cd_reajuste_anual = reajusteView.cd_reajuste_anual;
                        DataAccessReajusteCurso.addContext(item, false);
                    }

            DataAccessReajusteAnual.saveChanges(false);
        }

        public IEnumerable<Titulo> getTitulosReajusteAnual(int cd_escola, int cd_reajuste_anual, DateTime dt_ini_venc, DateTime? dt_fim_venc)
        {
            return DataAccessTitulo.getTitulosReajusteAnual(cd_escola, cd_reajuste_anual, dt_ini_venc, dt_fim_venc);
        }

        public decimal getSaldoContratoParaReajusteAnual(int cd_escola, decimal pc_bolsa, int cd_contrato, List<int> cdsTitulos)
        {
            return DataAccessTitulo.getSaldoContratoParaReajusteAnual(cd_escola, pc_bolsa, cd_contrato, cdsTitulos);
        }

        public ReajusteAnual getReajusteAnualFull(int cd_reajuste_anual, int cd_empresa)
        {
            return DataAccessReajusteAnual.getReajusteAnualFull(cd_reajuste_anual, cd_empresa);
        }

        public IEnumerable<ReajusteTitulo> getReajusteTitulos(int cd_empresa, int cd_reajuste_anual)
        {
            return DataAccessReajusteTitulo.getReajusteTitulos(cd_empresa, cd_reajuste_anual);
        }

        public bool verificaTitulosFechamentoReajusteAnual(int cd_empresa, int cd_reajuste_anual)
        {
            return DataAccessReajusteAnual.verificaTitulosFechamentoReajusteAnual(cd_empresa, cd_reajuste_anual);
        }

        public List<int> getCodigoContratoTitulosReajusteAnual(int cd_empresa, int cd_reajuste_anual)
        {
            return DataAccessReajusteAnual.getCodigoContratoTitulosReajusteAnual(cd_empresa, cd_reajuste_anual);
        }

        public void reverterAlteracaoTituloReajusteAnual(int cd_pessoa_escola, int cd_reajuste_anual)
        {
            List<ReajusteTitulo> reajusteTitulos = this.getReajusteTitulos(cd_pessoa_escola, cd_reajuste_anual).ToList();
            List<Titulo> titulos = DataAccessTitulo.getTitulosReajusteAnual(cd_pessoa_escola, cd_reajuste_anual).ToList();
            var grupoContrato = titulos.GroupBy(p => p.cd_origem_titulo).Select(g => new { cd_origem_titulo = g.Key.Value, Titulos = g }).ToList();
            foreach (var grupo in grupoContrato)
            {
                List<BaixaTitulo> baixaBolsaTitulos =  new List<BaixaTitulo>();
                List<int> cds_titulos_reaj = grupo.Titulos.Select(x => x.cd_titulo).ToList();
                if (grupo.Titulos.Any(x => x.vl_titulo != x.vl_saldo_titulo))
                    baixaBolsaTitulos = this.getBaixaTitulosBolsaContrato((int)grupo.cd_origem_titulo, cd_pessoa_escola, cds_titulos_reaj).ToList();
                foreach (Titulo t in grupo.Titulos)
                {
                    t.vl_saldo_titulo = t.ReajustesTitulos.FirstOrDefault().vl_original_saldo_titulo;
                    t.vl_titulo = t.ReajustesTitulos.FirstOrDefault().vl_original_titulo;
                    if (t.PlanoTitulo != null && t.PlanoTitulo.Count() > 0)
                        t.PlanoTitulo.FirstOrDefault().vl_plano_titulo = t.vl_titulo;
                    Decimal vl_pc_bolsa_aplicado = t.vl_titulo - t.vl_saldo_titulo;
                    if (vl_pc_bolsa_aplicado > 0)
                    {
                        t.vl_liquidacao_titulo = vl_pc_bolsa_aplicado;
                        if (t.BaixaTitulo != null && t.BaixaTitulo.Count() > 0)
                        {
                            BaixaTitulo b = t.BaixaTitulo.FirstOrDefault();
                            b.vl_liquidacao_baixa = vl_pc_bolsa_aplicado;
                            b.vl_principal_baixa = t.vl_titulo;
                            b.vl_baixa_saldo_titulo = vl_pc_bolsa_aplicado;
                            b.vl_desconto_baixa_calculado = (t.vl_titulo - t.vl_material_titulo) - vl_pc_bolsa_aplicado;
                        }
                    }
                }
            }
            DataAccessReajusteTitulo.deleteRangeContext(reajusteTitulos, false);
            DataAccessReajusteTitulo.saveChanges(false);
        }

        public ReajusteAnual getReajusteAnualGridView(int cd_reajuste_anual, int cd_empresa)
        {
            ReajusteAnual retorno = new ReajusteAnual();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = DataAccessReajusteAnual.getReajusteAnualGridView(cd_reajuste_anual, cd_empresa);
                transaction.Complete();
            }
            return retorno;
        }

        #endregion


        #region Controle de vendas de material
        public IEnumerable<RptContVendasMaterial> getRptContVendasMaterial(int cd_escola, int cd_aluno, int cd_item, DateTime dt_inicial, DateTime dt_final, int cd_turma, bool semmaterial)
        {
            List<RptContVendasMaterial> retorno = new List<RptContVendasMaterial>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = DataAccessMovimento.getRptContVendasMaterial(cd_escola, cd_aluno, cd_item, dt_inicial, dt_final, cd_turma, semmaterial).ToList();
                transaction.Complete();
            }
            return retorno;
        }
        public int findNotaAluno(int cd_aluno, int cd_curso)
        {
            return DataAccessMovimento.findNotaAluno(cd_aluno, cd_curso);
        }

        #endregion

        #region ItemKit
        public IEnumerable<ItemUI> obterListaItemsKit(int cd_item_kit, int cdEscola)
        {
            IEnumerable<ItemUI> listaItemsKit = new List<ItemUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                //if (parametros.sort == null)
                //    parametros.sort = "no_item";
                //parametros.sort = parametros.sort.Replace("item_ativo", "id_item_ativo");
                //parametros.sort = parametros.sort.Replace("categoria_grupo", "id_categoria_grupo");
                //parametros.sort = parametros.sort.Replace("pc_aliquota_iss", "iss");
                //parametros.sort = parametros.sort.Replace("pc_aliquota_icms", "icms");

                //listaItemsKit = DataAccessItem.getItemSearch(parametros, ).ToList();

                listaItemsKit = DataAccessItem.obterListaItemsKit(cd_item_kit, cdEscola);
                transaction.Complete();
            }
            return listaItemsKit;
        }

        public IEnumerable<ItemUI> obterListaItemMovItemsKit(ItemMovimento item, int cdEscola)
        {
            IEnumerable<ItemUI> listaItemsKit = new List<ItemUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {

                listaItemsKit = DataAccessItem.obterListaItemMovItemsKit(item, cdEscola);
                transaction.Complete();
            }
            return listaItemsKit;
        }


        public IEnumerable<ItemUI> obterListaItemsKitMov(int cd_item_kit, int cdEscola, int id_tipo_movto, int? id_natureza_TPNF)
        {
            IEnumerable<ItemUI> listaItemsKit = new List<ItemUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                //if (parametros.sort == null)
                //    parametros.sort = "no_item";
                //parametros.sort = parametros.sort.Replace("item_ativo", "id_item_ativo");
                //parametros.sort = parametros.sort.Replace("categoria_grupo", "id_categoria_grupo");
                //parametros.sort = parametros.sort.Replace("pc_aliquota_iss", "iss");
                //parametros.sort = parametros.sort.Replace("pc_aliquota_icms", "icms");

                //listaItemsKit = DataAccessItem.getItemSearch(parametros, ).ToList();

                listaItemsKit = DataAccessItem.obterListaItemsKitMov(cd_item_kit, cdEscola, id_tipo_movto, id_natureza_TPNF);
                transaction.Complete();
            }
            return listaItemsKit;
        }

        public Movimento calcularQuantidadeItemKit(ItemUI item, int cdEscola)
        {
            var movimento = new Movimento();
            movimento.ItemMovimentoKit = new List<ItemMovimentoKit>();

            //Pega todos items do kit no banco de dados;
            var itemsDoKitContext = obterListaItemsKit(item.cd_item_kit, cdEscola);
            // Filtra todos items do kit com os que vieram da view;
            var itemsDoKitView = item.items.Where(i =>
                itemsDoKitContext.Any(v =>
                    v.cd_item == i.cd_item && v.cd_item_kit == item.cd_item_kit && item.cd_item_kit > 0 &&
                    v.cd_item_kit > 0) && i.cd_item_kit > 0).ToList();
            var itemsDoNotKitView = item.items.Where(i =>
                (!itemsDoKitContext.Any(v => v.cd_item == i.cd_item && v.cd_item_kit == item.cd_item_kit)) ||
                (i.cd_item_kit == 0)).ToList();

            foreach (var itemDoKit in itemsDoKitView)
            {
                var itemDoKitContext = itemsDoKitContext.Where(i => i.cd_item == itemDoKit.cd_item).FirstOrDefault();
                itemDoKit.qt_item_movimento = aplicarCalculoKit(itemDoKit.qt_item_movimento,
                    itemDoKitContext.qt_item_kit, item.qt_item_kit, item.ultimo_valor_kit);
                movimento.ItensMovimento.Add(itemDoKit);
            }

            foreach (var itemMovimento in itemsDoNotKitView)
            {
                movimento.ItensMovimento.Add(itemMovimento);
            }

            movimento.ItensMovimento.OrderBy(x => x.id);

            return movimento;
        }

        //public Movimento calcularQuantidadeItemKit(ItemUI item, int cdEscola)
        //{
        //    var movimento = new Movimento();

        //    //Pega todos items do kit no banco de dados;
        //    var itemsDoKitContext = obterListaItemsKit(item.cd_item_kit, cdEscola);
        //    // Filtra todos items do kit com os que vieram da view;
        //    var itemsDoKitView = item.items.Where(i => itemsDoKitContext.Any(v => v.cd_item == i.cd_item)).ToList();

        //    foreach (var itemDoKit in itemsDoKitView)
        //    {
        //        var itemDoKitContext = itemsDoKitContext.Where(i => i.cd_item == itemDoKit.cd_item).FirstOrDefault();
        //        itemDoKit.qt_item_movimento = aplicarCalculoKit(itemDoKit.qt_item_movimento, itemDoKitContext.qt_item_kit, item.qt_item_kit, item.ultimo_valor_kit);
        //        movimento.ItensMovimento.Add(itemDoKit);
        //    }

        //    foreach (var itemMovimento in item.items)
        //    {

        //    }

        //    return movimento;
        //}

        private int aplicarCalculoKit(int qtdItemView, int qtdItemBd, int qtdKitView, int qtdAnteriorKit)
        {
            return ((qtdItemView - (qtdAnteriorKit * qtdItemBd)) + (qtdKitView * qtdItemBd));
        }

        public Movimento excluirKitDoGrid(ItemUI item, int cdEscola) 
        {
            var itemsDoKitContext = obterListaItemsKit(item.cd_item_kit, cdEscola);
            var movimentoRetornoView = new Movimento();

            foreach (var itemMovimento in item.items)
            {
                var itemDoKitContext = itemsDoKitContext
                    .Where(i => i.cd_item == itemMovimento.cd_item && i.cd_item_kit == itemMovimento.cd_item_kit)
                    .FirstOrDefault();

                if (itemDoKitContext != null)
                {
                    itemMovimento.qt_item_movimento =
                        (itemMovimento.qt_item_movimento - (item.qt_item_kit * itemDoKitContext.qt_item_kit));
                }

                if (itemMovimento.qt_item_movimento > 0)
                {
                    movimentoRetornoView.ItensMovimento.Add(itemMovimento);
                }
            }

            return movimentoRetornoView;
        }

        //public Movimento excluirKitDoGrid(ItemUI item, int cdEscola)
        //{
        //    var itemsDoKitContext = obterListaItemsKit(item.cd_item_kit, cdEscola);
        //    var movimentoRetornoView = new Movimento();

        //    foreach (var itemMovimento in item.items)
        //    {
        //        var itemDoKitContext = itemsDoKitContext.Where(i => i.cd_item == itemMovimento.cd_item).FirstOrDefault();

        //        if (itemDoKitContext != null)
        //            itemMovimento.qt_item_movimento = (itemMovimento.qt_item_movimento - (item.qt_item_kit * itemDoKitContext.qt_item_kit));

        //        if (itemMovimento.qt_item_movimento > 0)
        //            movimentoRetornoView.ItensMovimento.Add(itemMovimento);
        //    }

        //    return movimentoRetornoView;
        //}
        #endregion

        #region Taxa Bancaria
        public TaxaBancaria getTaxaBancariaPorId(int cd_taxa_bancaria)
        {
            return DataAccessTaxaBancaria.findById(cd_taxa_bancaria, false);
        }
        #endregion

        #region Titulo

        public List<Titulo> getTitulosByTituloAditamento(int cd_escola, int cd_aditamento)
        {
            List<Titulo> titulos = new List<Titulo>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {

                titulos = DataAccessTitulo.getTitulosByTituloAditamento(cd_escola, cd_aditamento);
                transaction.Complete();
            }
            return titulos;
        }

        public bool deleteTitulosByTituloAditamento(int cd_escola, string cd_aditamento)
        {
            //bool retorno = false;
            int ret = 0;
            //using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            //{
            //    retorno = DataAccessTitulo.deleteTitulosByTituloAditamento(cd_aditamento, cd_titulos);
            ret = DataAccessTitulo.excluirAditamentoProcedure(cd_escola);
            //    transaction.Complete();
            //}
            return ret == 0;
        }

        #endregion

//        #region Orgao Financeiro

        public DataTable getRptAlunoRestricao(int? cd_escola, int?cd_orgao, DateTime? dt_inicio, DateTime? dt_final, byte? tipodata)
        {
            return DataAccessReajusteAluno.getRptAlunoRestricao(cd_escola, cd_orgao, dt_inicio, dt_final, tipodata);
        }
        //        #endregion

    }
}

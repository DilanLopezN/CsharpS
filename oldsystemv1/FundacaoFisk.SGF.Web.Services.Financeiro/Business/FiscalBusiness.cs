using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Xml;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Empresa.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Pessoa.Business;
using FundacaoFisk.SGF.Utils.Messages;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Financeiro.Model;
using Componentes.GenericBusiness;
using FundacaoFisk.SGF.GenericModel;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using System.Security.Cryptography;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Business
{
    public class FiscalBusiness : IFiscalBusiness
    {
        public IItemMovimentoDataAccess DataAccessItemMovimento { get; set; }
        public IItemMovimentoKitDataAccess DataAccessItemMovimentoKit { get; set; }
        public IItemMovItemKitDataAccess DataAccessItemMovimentoItemKit { get; set; }

        public IMovimentoDataAccess DataAccessMovimento { get; set; }
        public IItemEscolaDataAccess DataAccessItemEscola { get; set; }
        public ITituloDataAccess DataAccessTitulo { get; set; }
        public IPlanoTituloDataAccess DataAccessPlanoTitulo { get; set; }
        public ITipoItemDataAccess DataAccessTipoItem { get; set; }
        public ICFOPDataAccess DataAccessCFOP { get; set; }
        public IFinanceiroBusiness BusinessFinan { get; set; }
        public ITipoNotaFiscalDataAccess DataAccessTipoNotaFiscal { get; set; }
        public IEmpresaBusiness BusinessEmpresa { get; set; }

        public FiscalBusiness(IItemMovimentoDataAccess dataAccessItemMovimento,
                                   IItemMovimentoKitDataAccess dataAccessItemMovimentoKit,
                                   IItemMovItemKitDataAccess dataAccessItemMovimentoItemKit,
                                   IMovimentoDataAccess dataAccessMovimento,
                                   IItemEscolaDataAccess dataAccessItemEscola, ITituloDataAccess dataAccessTitulo,
                                   IPlanoTituloDataAccess dataAccessPlanoTitulo, ITipoItemDataAccess dataAccessTipoItem,
                                   IFinanceiroBusiness businessFinan, ICFOPDataAccess dataAccessCFOP,
                                   ITipoNotaFiscalDataAccess dataAccessTipoNotaFiscal, IEmpresaBusiness businessEmpresa)
        {
            if (dataAccessItemMovimento == null || dataAccessItemMovimentoKit == null || dataAccessItemMovimentoItemKit == null || dataAccessMovimento == null ||
                dataAccessTitulo == null || dataAccessPlanoTitulo == null ||
                dataAccessTipoItem == null || businessFinan == null ||
                dataAccessCFOP == null || dataAccessTipoNotaFiscal == null || businessEmpresa == null)
                throw new ArgumentNullException("repository");
            
            this.DataAccessItemMovimento = dataAccessItemMovimento;
            this.DataAccessItemMovimentoKit = dataAccessItemMovimentoKit;
            this.DataAccessItemMovimentoItemKit = dataAccessItemMovimentoItemKit;
            this.DataAccessMovimento = dataAccessMovimento;
            this.DataAccessItemEscola = dataAccessItemEscola;
            this.DataAccessTitulo = dataAccessTitulo;
            this.DataAccessPlanoTitulo = dataAccessPlanoTitulo;
            this.DataAccessTipoItem = dataAccessTipoItem;
            this.BusinessFinan = businessFinan;
            this.DataAccessCFOP = dataAccessCFOP;
            this.DataAccessTipoNotaFiscal = dataAccessTipoNotaFiscal;
            this.BusinessEmpresa = businessEmpresa;
        }

        public void configuraUsuario(int cdUsuario, int cd_empresa)
        {
            // Configura os codigos do usuário para auditorias dos DataAccess:
            ((SGFWebContext)this.DataAccessItemMovimento.DB()).IdUsuario = ((SGFWebContext)this.DataAccessItemMovimentoKit.DB()).IdUsuario = ((SGFWebContext)this.DataAccessItemMovimentoItemKit.DB()).IdUsuario =
            ((SGFWebContext)this.DataAccessMovimento.DB()).IdUsuario = ((SGFWebContext)this.DataAccessItemEscola.DB()).IdUsuario = 
            ((SGFWebContext)this.DataAccessTitulo.DB()).IdUsuario = ((SGFWebContext)this.DataAccessPlanoTitulo.DB()).IdUsuario = 
            ((SGFWebContext)this.DataAccessCFOP.DB()).IdUsuario = ((SGFWebContext) this.DataAccessTipoNotaFiscal.DB()).IdUsuario = cdUsuario;

            ((SGFWebContext)this.DataAccessItemMovimento.DB()).cd_empresa = ((SGFWebContext)this.DataAccessItemMovimentoKit.DB()).cd_empresa = ((SGFWebContext)this.DataAccessItemMovimentoItemKit.DB()).cd_empresa = ((SGFWebContext)this.DataAccessMovimento.DB()).cd_empresa =
            ((SGFWebContext)this.DataAccessItemEscola.DB()).cd_empresa = ((SGFWebContext)this.DataAccessTitulo.DB()).cd_empresa =
            ((SGFWebContext) this.DataAccessPlanoTitulo.DB()).cd_empresa = ((SGFWebContext) this.DataAccessCFOP.DB()).cd_empresa = 
            ((SGFWebContext) this.DataAccessTipoNotaFiscal.DB()).cd_empresa = cd_empresa;
            BusinessEmpresa.configuraUsuario(cdUsuario, cd_empresa);
            BusinessFinan.configuraUsuario(cdUsuario, cd_empresa);
        }

        public void sincronizarContextos(DbContext dbContext)
        {
            //this.DataAccessItemEscola.sincronizaContexto(dbContext);
            //this.DataAccessTitulo.sincronizaContexto(dbContext);
            //this.DataAccessPlanoTitulo.sincronizaContexto(dbContext);
            //this.DataAccessMovimento.sincronizaContexto(dbContext);
            //this.DataAccessItemMovimento.sincronizaContexto(dbContext);
            //this.DataAccessCFOP.sincronizaContexto(dbContext);
            //this.DataAccessTipoItem.sincronizaContexto(dbContext);
            //this.BusinessFinan.sincronizarContextos(dbContext);
            //this.DataAccessTipoNotaFiscal.sincronizaContexto(dbContext);
        }

        #region Movimento

        public IEnumerable<ItemMovimento> getItensByAluno(SearchParameters parametros, int cd_pessoa, int cd_aluno, int cd_escola)
        {
            IEnumerable<ItemMovimento> retorno = new List<ItemMovimento>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "Movimento.dt_emissao_movimento";
                parametros.sort = parametros.sort.Replace("dta_emissao_movimento", "Movimento.dt_emissao_movimento");
                parametros.sort = parametros.sort.Replace("nm_movimento", "Movimento.nm_movimento");
                parametros.sort = parametros.sort.Replace("vlr_liquido_item", "vl_liquido_item");
            
                retorno = DataAccessItemMovimento.getItensByAluno(parametros, cd_pessoa,cd_aluno, cd_escola);
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<MovimentoUI> searchMovimento(SearchParameters parametros, int id_tipo_movimento, int cd_pessoa, int cd_item, int cd_plano_conta, int numero, string serie, int cd_empresa,
                                                      bool emissao, bool movimento, DateTime? dtInicial, DateTime? dtFinal, bool nota_fiscal, int statusNF, bool contaSegura, int isImportXML, 
                                                      bool? id_material_didatico, bool? id_venda_futura)
        {
            IEnumerable<MovimentoUI> retorno = new List<MovimentoUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED,DataAccessMovimento.DB(), 
                TransactionScopeBuilder.TransactionTime.MODEREATE))
            {
                if (parametros.sort == null)
                {
                    parametros.sort = "nm_movimento";
                    parametros.sortOrder = Componentes.Utils.SortDirection.Descending;
                }
            parametros.sort = parametros.sort.Replace("dc_numero_serie", "nm_movimento");
            parametros.sort = parametros.sort.Replace("dta_emissao_movimento", "dt_emissao_movimento");
            parametros.sort = parametros.sort.Replace("dta_vcto_movimento", "dt_vcto_movimento");
            parametros.sort = parametros.sort.Replace("dta_mov_movimento", "dt_mov_movimento");
            parametros.sort = parametros.sort.Replace("status_nf_pesq", "id_status_nf");
            parametros.sort = parametros.sort.Replace("dc_natureza_tipo_nf", "id_natureza_mvto_tp_nf");
            parametros.sort = parametros.sort.Replace("vl_qtd_total_geral", "qtd_total_geral");
            retorno = DataAccessMovimento.searchMovimento(parametros, id_tipo_movimento, cd_pessoa, cd_item, cd_plano_conta, numero, serie, cd_empresa, emissao, movimento, dtInicial, dtFinal, nota_fiscal, statusNF, contaSegura, isImportXML, id_material_didatico, id_venda_futura);
                transaction.Complete();
            }
            return retorno;
        }
        public IEnumerable<MovimentoUI> searchMovimentoFK(SearchParameters parametros, int cd_pessoa, int cd_item, int cd_plano_conta, int numero, string serie, int cd_empresa,
                                                       bool emissao, bool movimento, DateTime? dtInicial, DateTime? dtFinal, int natMovto, bool idNf)
        {
            IEnumerable<MovimentoUI> retorno = new List<MovimentoUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                {
                    parametros.sort = "nm_movimento";
                    parametros.sortOrder = Componentes.Utils.SortDirection.Descending;
                }
                parametros.sort = parametros.sort.Replace("dc_numero_serie", "nm_movimento");
                parametros.sort = parametros.sort.Replace("dta_emissao_movimento", "dt_emissao_movimento");
                parametros.sort = parametros.sort.Replace("dta_vcto_movimento", "dt_vcto_movimento");
                parametros.sort = parametros.sort.Replace("dta_mov_movimento", "dt_mov_movimento");
                retorno = DataAccessMovimento.searchMovimentoFK(parametros, cd_pessoa, cd_item, cd_plano_conta, numero, serie, cd_empresa, emissao, movimento, dtInicial, dtFinal, natMovto, idNf);
                transaction.Complete();
            }
            return retorno;
        }
        
        public IEnumerable<MovimentoPerdaMaterialUI> searchMovimentoFKPerdaMaterial(SearchParameters parametros, int cd_pessoa, int cd_item, int cd_plano_conta, int numero, string serie, int cd_empresa,
                                                       bool emissao, bool movimento, DateTime? dtInicial, DateTime? dtFinal, int natMovto, bool idNf, int origem, int? cd_aluno, int? nm_contrato)
        {
            IEnumerable<MovimentoPerdaMaterialUI> retorno = new List<MovimentoPerdaMaterialUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                {
                    parametros.sort = "nm_movimento";
                    parametros.sortOrder = Componentes.Utils.SortDirection.Descending;
                }
                parametros.sort = parametros.sort.Replace("dc_numero_serie", "nm_movimento");
                parametros.sort = parametros.sort.Replace("dta_emissao_movimento", "dt_emissao_movimento");
                parametros.sort = parametros.sort.Replace("dta_vcto_movimento", "dt_vcto_movimento");
                parametros.sort = parametros.sort.Replace("dta_mov_movimento", "dt_mov_movimento");
                retorno = DataAccessMovimento.searchMovimentoFKPerdaMaterial(parametros, cd_pessoa, cd_item, cd_plano_conta, numero, serie, cd_empresa, emissao, movimento, dtInicial, dtFinal, natMovto, idNf, origem, cd_aluno, nm_contrato);
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<MovimentoUI> searchMovimentoFKVincularMaterial(SearchParameters parametros, int cd_pessoa, int cd_item, int cd_plano_conta, int numero, string serie, int cd_empresa,
            bool emissao, bool movimento, DateTime? dtInicial, DateTime? dtFinal, int natMovto, bool material_didatico_vincular_material, bool nota_fiscal_vincular_material, int cd_curso)
        {
            IEnumerable<MovimentoUI> retorno = new List<MovimentoUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                {
                    parametros.sort = "nm_movimento";
                    parametros.sortOrder = Componentes.Utils.SortDirection.Descending;
                }
                parametros.sort = parametros.sort.Replace("dc_numero_serie", "nm_movimento");
                parametros.sort = parametros.sort.Replace("dta_emissao_movimento", "dt_emissao_movimento");
                parametros.sort = parametros.sort.Replace("dta_vcto_movimento", "dt_vcto_movimento");
                parametros.sort = parametros.sort.Replace("dta_mov_movimento", "dt_mov_movimento");
                retorno = DataAccessMovimento.searchMovimentoFKVincularMaterial(parametros, cd_pessoa, cd_item, cd_plano_conta, numero, serie, cd_empresa, emissao, movimento, dtInicial, dtFinal, natMovto, material_didatico_vincular_material,  nota_fiscal_vincular_material, cd_curso);
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<ItemUI> getItemMovimentoSearch(SearchParameters parametros, string descricao, bool inicio, bool? ativo, int? tipoItem, int? cdGrupoItem, int cdEscola,
            int id_tipo_movto, Movimento.TipoVinculoMovimento tipoVinc, bool comEstoque, int? id_natureza_TPNF, bool kit, bool contaSegura, int? cd_movimento, int? cd_aluno, DateTime? dt_inicial, DateTime? dt_final, bool? vinculado_curso, int? cd_curso_material_didatico)
        {
            IEnumerable<ItemUI> retorno = new List<ItemUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "no_item";
                parametros.sort = parametros.sort.Replace("id_item_ativo", "item_ativo");
                retorno = DataAccessMovimento.getItemMovimentoSearch(parametros, descricao, inicio, ativo, tipoItem, cdGrupoItem, cdEscola, id_tipo_movto, tipoVinc, comEstoque, id_natureza_TPNF, kit, contaSegura, cd_movimento, cd_aluno,
                    dt_inicial, dt_final, vinculado_curso, cd_curso_material_didatico).ToList();
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<ItemUI> getItemMovimentoSearchPerdaMaterial(SearchParameters parametros, string descricao, bool inicio, bool? ativo, int? tipoItem, int? cdGrupoItem, int cdEscola,
            int id_tipo_movto, Movimento.TipoVinculoMovimento tipoVinc, bool comEstoque, int? id_natureza_TPNF, bool kit, bool contaSegura, int? cd_movimento, int origem, int? cd_aluno, DateTime? dt_inicial, DateTime? dt_final, bool? vinculado_curso, int? cd_curso_material_didatico)
        {
            IEnumerable<ItemUI> retorno = new List<ItemUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "no_item";
                parametros.sort = parametros.sort.Replace("id_item_ativo", "item_ativo");
                retorno = DataAccessMovimento.getItemMovimentoSearchPerdaMaterial(parametros, descricao, inicio, ativo, tipoItem, cdGrupoItem, cdEscola, id_tipo_movto, tipoVinc, comEstoque, id_natureza_TPNF, kit, contaSegura, cd_movimento, origem, cd_aluno,
                    dt_inicial, dt_final, vinculado_curso, cd_curso_material_didatico).ToList();
                transaction.Complete();
            }
            return retorno;
        }

        public Movimento getMovimentoEditView(int cd_movimento, int cd_empresa)
        {
            Movimento retorno = new Movimento();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = DataAccessMovimento.getMovimentoEditView(cd_movimento, cd_empresa);
                transaction.Complete();
        }
            return retorno;
        }

        public Movimento getMovimentoEditViewNF(int cd_movimento, int cd_empresa, int id_tipo_movimento)
        {
            Movimento retorno = new Movimento();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = DataAccessMovimento.getMovimentoEditViewNF(cd_movimento, cd_empresa, id_tipo_movimento);
                transaction.Complete();
        }
            return retorno;
        }

        public Movimento getMovimentoById(int cd_movimento) {
            return DataAccessMovimento.findById(cd_movimento, false);
        }

        public Movimento addMovimento(Movimento movimento)
        {
            validarMinDateMovimento(movimento);
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                if (!movimento.id_nf)
                {
                    if (movimento.nm_movimento == null || movimento.nm_movimento == 0)
                    {
                        int? maxNmMvto = DataAccessMovimento.getMaxMovimento(movimento.id_tipo_movimento, movimento.cd_pessoa_empresa);
                        if (maxNmMvto.HasValue)
                            movimento.nm_movimento = maxNmMvto + 1;
                        else
                            movimento.nm_movimento = 1;
                    }
                    if (movimento.dc_serie_movimento == null || movimento.dc_serie_movimento == "")
                        movimento.dc_serie_movimento = 1 + "";
                }
                if (movimento.id_nf && (movimento.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.SAIDA ||
                                       movimento.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.ENTRADA ||
                                       movimento.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.SERVICO ||
                                       movimento.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.DEVOLUCAO))
                    movimento.id_status_nf = (int)Movimento.StatusNFEnum.ABERTO;
                List<ItemMovimento> itensMovimento = movimento.ItensMovimento.ToList();
                movimento.ItensMovimento = null;
                movimento = DataAccessMovimento.add(movimento, false);


                if (!movimento.id_nf)
                {
                    if (itensMovimento != null && itensMovimento.Count() > 0)
                    {
                        crudItensMovimento(itensMovimento, movimento, true);
                    }
                }
                else
                {
                    if (itensMovimento != null && itensMovimento.Count() > 0)
                    {
                        if (movimento.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.ENTRADA)
                            crudItensMovimentoNF(itensMovimento, movimento, true);
                        else
                            crudItensMovimento(itensMovimento, movimento, true);
                    }
                }

                if (!movimento.id_nf || movimento.id_tipo_movimento != (int)Movimento.TipoMovimentoEnum.ENTRADA)
                {
                    if (movimento.titulos != null && movimento.titulos.Count() > 0)
                    {
                        crudTitulosMovimento(movimento.titulos, movimento);
                    }
                }

                if (!movimento.id_nf)
                {
                    BusinessEmpresa.postEmpresaPessoa(new PessoaEscola
                    {
                        cd_escola = movimento.cd_pessoa_empresa,
                        cd_pessoa = movimento.cd_pessoa
                    });
                }

                transaction.Complete();
            }
            return movimento;
        }

        public Movimento editMovimento(Movimento movimento)
        {
            bool alterouNMMovimento = false;
            validarMinDateMovimento(movimento);
            this.sincronizarContextos(DataAccessMovimento.DB());
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                if (movimento.id_tipo_movimento != (int)Movimento.TipoMovimentoEnum.SAIDA &&
                    movimento.id_tipo_movimento != (int)Movimento.TipoMovimentoEnum.SERVICO)
                {
                    if (movimento.nm_movimento == null || movimento.nm_movimento == 0)
                    {
                        int? maxNmMvto = DataAccessMovimento.getMaxMovimento(movimento.id_tipo_movimento, movimento.cd_pessoa_empresa);
                        if (maxNmMvto.HasValue)
                            movimento.nm_movimento = maxNmMvto + 1;
                        else
                            movimento.nm_movimento = 1;
                    }
                    if (movimento.dc_serie_movimento == null || movimento.dc_serie_movimento == "")
                        movimento.dc_serie_movimento = 1 + "";
                }

                Movimento movtoContext = DataAccessMovimento.getMovimentoComCheque(movimento.cd_movimento, movimento.cd_pessoa_empresa);
                if (movtoContext.id_nf && movtoContext.id_status_nf == (int)Movimento.StatusNFEnum.FECHADO ||
                    movtoContext.id_nf && movtoContext.id_status_nf == (int)Movimento.StatusNFEnum.CANCELADO)
                    throw new FinanceiroBusinessException(string.Format(string.Format(Utils.Messages.Messages.msgErroNFProcessadaOuCancelada, movtoContext.status_nf)), null,
                                                                   FinanceiroBusinessException.TipoErro.ERRO_NF_FECHADA_CANCELADA, false);

                int? cd_tipo_nota_fiscal = null;
                int? cd_tipo_nota_fiscal_context = null;
                if (movtoContext != null)
                {
                    if (movtoContext.cd_tipo_financeiro == (int)TipoFinanceiro.TiposFinanceiro.CHEQUE)
                        if (movimento.cd_tipo_financeiro != (int)TipoFinanceiro.TiposFinanceiro.CHEQUE)
                        {
                            Cheque cheque = movtoContext.Cheques.FirstOrDefault();
                            if(cheque != null)
                                BusinessFinan.deleteCheque(cheque);
                        }
                        else
                        {
                            if (movtoContext.Cheques != null && movtoContext.Cheques.Count() > 0 && movimento.Cheques != null && movimento.Cheques.Count() > 0)
                                Cheque.changeValuesCheque(movtoContext.Cheques.FirstOrDefault(), movimento.Cheques.FirstOrDefault());
                            else
                                if ((movtoContext.Cheques == null || movtoContext.Cheques.Count() == 0) && movimento.Cheques != null && movimento.Cheques.Count() > 0)
                                {
                                    movtoContext.Cheques.Add(movimento.Cheques.FirstOrDefault());
                                    movtoContext.Cheques.FirstOrDefault().cd_movimento = movtoContext.cd_movimento;
                                }
                        }
                    cd_tipo_nota_fiscal = movimento.cd_tipo_nota_fiscal;
                    cd_tipo_nota_fiscal_context = movtoContext.cd_tipo_nota_fiscal;
                    movimento.id_nf_movimento_antigo = movtoContext.id_nf;
                    movtoContext = Movimento.changeValuesMovimento(movtoContext, movimento);
                    if (DataAccessMovimento.DB().Entry(movtoContext).Property(m => m.nm_movimento).IsModified)
                        alterouNMMovimento = true;
                }
                DataAccessMovimento.saveChanges(false);
                //movimento = DataAccessMovimento.add(movimento, false);

                
                    if (movimento.ItensMovimento != null)
                    {
                        List<ItemMovimento> itemMovimentoContext = DataAccessItemMovimento.getItensMovimentoByMovimento(movimento.cd_movimento, movimento.cd_pessoa_empresa).ToList();

                        //Se modificou o tipo de nota fiscal
                        bool houve_troca_tipo = (cd_tipo_nota_fiscal.HasValue && !cd_tipo_nota_fiscal_context.HasValue) || (!cd_tipo_nota_fiscal.HasValue && cd_tipo_nota_fiscal_context.HasValue)
                                || (cd_tipo_nota_fiscal.HasValue && cd_tipo_nota_fiscal_context.HasValue && cd_tipo_nota_fiscal.Value != cd_tipo_nota_fiscal_context.Value);
                        if (houve_troca_tipo)
                        {
                            if (!movimento.id_nf || movimento.id_tipo_movimento != (int)Movimento.TipoMovimentoEnum.ENTRADA)
                            {
                                //Se não tinha movimentação de estoque e agora possui, tem que incluir os kardex:
                                if ((!cd_tipo_nota_fiscal_context.HasValue || !DataAccessTipoNotaFiscal.getMovimentaEstoque(cd_tipo_nota_fiscal_context.Value))
                                        && (cd_tipo_nota_fiscal.HasValue && DataAccessTipoNotaFiscal.getMovimentaEstoque(cd_tipo_nota_fiscal.Value)))
                                    foreach (ItemMovimento itemMovimento in itemMovimentoContext)
                                    {
                                        ItemEscola itemServico = DataAccessItemEscola.findItemEscolabyId(itemMovimento.cd_item, movimento.cd_pessoa_empresa);

                                        //Somente cria o kardex se o item permanece no cadastro. Se foi excluído, não faz sentido gerar kardex para ele:
                                        if (movimento.ItensMovimento.Any(im => im.cd_item == itemMovimento.cd_item))
                                        {
                                            Kardex kardexView = criaNovoKardex(itemMovimento, movimento, itemServico);

                                            // Se não incluiu ainda, inclui o kardex (deve-se verificar pois o crud pode já ter incluído o kardex na alteração).
                                            SGFWebContext cdb = new SGFWebContext();
                                            if (!BusinessFinan.existeKardexItemMovimentoByOrigem((byte)cdb.LISTA_ORIGEM_LOGS["ItemMovimento"], itemMovimento.cd_item_movimento))
                                                incluirKardexOfMovimento(itemMovimento, movimento, kardexView, itemServico);
                                        }
                                    }

                            }
                            //Se tinha movimentação de estoque e agora não possui, tem que remover os kardex (se existem):
                            if (cd_tipo_nota_fiscal_context.HasValue && DataAccessTipoNotaFiscal.getMovimentaEstoque(cd_tipo_nota_fiscal_context.Value)
                                    && (!cd_tipo_nota_fiscal.HasValue || !DataAccessTipoNotaFiscal.getMovimentaEstoque(cd_tipo_nota_fiscal.Value)))
                                foreach (ItemMovimento itemMovimento in itemMovimentoContext)
                                    deletarKardexOfMovimento(itemMovimento, movimento, true);
                        }

                        if (!movimento.id_nf)
                        {
                            crudItensMovimento(movimento.ItensMovimento.ToList(), movimento, houve_troca_tipo);
                        }
                        else
                        {
                            if (movimento.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.ENTRADA)
                                crudItensMovimentoNF(movimento.ItensMovimento.ToList(), movimento, houve_troca_tipo);
                            else
                                crudItensMovimento(movimento.ItensMovimento.ToList(), movimento, houve_troca_tipo);
                        }

                    }
                
                    if (movimento.ItemMovimentoKit != null && movimento.ItemMovimentoKit.Count > 0)
                    {
                        crudItensMovimentoKit(movimento);
                    }
                    if (movimento.titulos != null)
                        crudTitulosMovimento(movimento.titulos, movimento);
                    else
                        if (alterouNMMovimento)
                        {
                            DataAccessTitulo.sincronizaContexto(DataAccessMovimento.DB());
                            List<Titulo> titulosContext = DataAccessTitulo.getTitulosByMovimento(movimento.cd_movimento, movimento.cd_pessoa_empresa).ToList();
                            foreach (Titulo t in titulosContext)
                                t.nm_titulo = movimento.nm_movimento;
                            DataAccessTitulo.saveChanges(false);
                        }
                    if (!movimento.id_nf)
                    {
                        BusinessEmpresa.postEmpresaPessoa(new PessoaEscola
                        {
                            cd_escola = movimento.cd_pessoa_empresa,
                            cd_pessoa = movimento.cd_pessoa
                        });
                    }
                
                transaction.Complete();
            }
            return movimento;
        }

        private void validarMinDateMovimento(Movimento movimento)
        {
            if (movimento != null)
            {
                if ((movimento.dt_emissao_movimento != null && DateTime.Compare((DateTime)movimento.dt_emissao_movimento, new DateTime(1900, 1, 1)) < 0) ||
                    (movimento.dt_mov_movimento != null && DateTime.Compare((DateTime)movimento.dt_mov_movimento, new DateTime(1900, 1, 1)) < 0) ||
                    (movimento.dt_vcto_movimento != null && DateTime.Compare((DateTime)movimento.dt_vcto_movimento, new DateTime(1900, 1, 1)) < 0))
                    throw new PessoaBusinessException(Utils.Messages.Messages.msgErroDataErroMinDateMovimento, null,
                          FundacaoFisk.SGF.Web.Services.Pessoa.Business.PessoaBusinessException.TipoErro.ERRO_MINDATE_SMALLDATETIME, false);
                if ((movimento.dt_emissao_movimento != null && DateTime.Compare((DateTime)movimento.dt_emissao_movimento, new DateTime(2079, 06, 06)) > 0) ||
                    (movimento.dt_mov_movimento != null && DateTime.Compare((DateTime)movimento.dt_mov_movimento, new DateTime(2079, 06, 06)) > 0) ||
                    (movimento.dt_vcto_movimento != null && DateTime.Compare((DateTime)movimento.dt_vcto_movimento, new DateTime(2079, 06, 06)) > 0))
                    throw new PessoaBusinessException(Utils.Messages.Messages.msgErroDataErroMaxDateMovimento, null,
                          FundacaoFisk.SGF.Web.Services.Pessoa.Business.PessoaBusinessException.TipoErro.ERRO_MINDATE_SMALLDATETIME, false);
            }
        }

        private void deletarKardexOfMovimento(ItemMovimento item, Movimento movimento)
        {
            deletarKardexOfMovimento(item, movimento, false);
        }

        private void deletarKardexOfMovimento(ItemMovimento item, Movimento movimento, bool houve_troca_tipo) {
            List<ItemMovimento> itemNota = DataAccessItemMovimento.getItensMvto(movimento.cd_movimento, movimento.cd_pessoa_empresa);
            IEnumerable<ItemMovimento> itemVoucher = itemNota.Where(tc => tc.cd_item_movimento == item.cd_item_movimento);
            bool carga = (from it in itemVoucher select it.id_voucher_carga).FirstOrDefault();


            if (((DataAccessTipoItem.verificaMovimentarEstoque(item.cd_item) || carga) && (!movimento.id_nf || 
                                                                                DataAccessTipoNotaFiscal.verificarTipoNotaFiscalPermiteMovimentoEstoque(item.cd_movimento) || houve_troca_tipo)))
            {
                SGFWebContext cdb = new SGFWebContext();
                Kardex kardex = BusinessFinan.getKardexByOrigem((byte) cdb.LISTA_ORIGEM_LOGS["ItemMovimento"], item.cd_item_movimento).FirstOrDefault();
                //Atualiza quantidade do item no estoque.
                ItemEscola itemServico = DataAccessItemEscola.findItemEscolabyId(item.cd_item, movimento.cd_pessoa_empresa);

                if (itemServico == null)
                {
                    throw new FinanceiroBusinessException(string.Format(Utils.Messages.Messages.msgItemNaoEncontrado, item.cd_item), null,
                        FinanceiroBusinessException.TipoErro.ITEM_NAO_ENCONTRADO, false);
                }
                

                int tipoMovimento = movimento.id_tipo_movimento;
                //Se for devolução, o tipo do movimento deve ser o oposto do tipo de movimento da NF Devolvida, ou seja, quando devolver uma NF Saida, a NF será de entrada e vice-versa
                if ((movimento.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.DEVOLUCAO || movimento.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.SERVICO) && movimento.cd_tipo_nota_fiscal.HasValue)
                    tipoMovimento = (int)getTipoMvtoTpNF(movimento.cd_tipo_nota_fiscal.Value);
                if (tipoMovimento == (int)Movimento.TipoMovimentoEnum.SAIDA)
                    itemServico.qt_estoque += item.qt_item_movimento;
                if (tipoMovimento == (int)Movimento.TipoMovimentoEnum.ENTRADA)
                {
                    itemServico.qt_estoque -= item.qt_item_movimento;
                    //TODO: Deivid voltar ao ultimo preço aplicado
                    //itemServico.vl_item = item.vl_liquido_item;
                }
                if(kardex != null && kardex.cd_registro_origem > 0)
                    BusinessFinan.deleteKardex(kardex);
            }
        }

        private Kardex criaNovoKardex(ItemMovimento item, Movimento movimento, ItemEscola itemServico) {
            Kardex kardexView = new Kardex();
            int tipoMovimento = item.id_voucher_carga ? 1 : movimento.id_tipo_movimento;
            if ((movimento.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.DEVOLUCAO || movimento.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.SERVICO) && movimento.cd_tipo_nota_fiscal.HasValue)
                tipoMovimento = (int)getTipoMvtoTpNF(movimento.cd_tipo_nota_fiscal.Value);
            if (tipoMovimento != (int)Movimento.TipoMovimentoEnum.DESPESA || item.id_voucher_carga)
            {
                SGFWebContext cdb = new SGFWebContext(); 
                decimal vlCompra = 0;

                if (tipoMovimento == (int)Movimento.TipoMovimentoEnum.ENTRADA || item.id_voucher_carga)
                {
                    if(item.vl_liquido_item > 0)
                        vlCompra = item.vl_liquido_item;
                }
                else
                    if(itemServico != null && itemServico.vl_item > 0 && item.qt_item_movimento > 0)
                        vlCompra = itemServico.vl_item * item.qt_item_movimento;
                if (tipoMovimento == (int)Movimento.TipoMovimentoEnum.DEVOLUCAO && movimento.cd_nota_fiscal.HasValue)
                {
                    Movimento movimentoDevolucao = getMovimentoById((int)movimento.cd_nota_fiscal);
                    tipoMovimento = 3 - movimentoDevolucao.id_tipo_movimento;
                }

                kardexView = new Kardex {
                    cd_pessoa_empresa = movimento.cd_pessoa_empresa,
                    cd_item = item.cd_item,
                    cd_origem = (byte) cdb.LISTA_ORIGEM_LOGS["ItemMovimento"],
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

        private void incluirKardexOfMovimento(ItemMovimento item, Movimento movimento, Kardex kardexView, ItemEscola itemServico) {
            if((DataAccessTipoItem.verificaMovimentarEstoque(item.cd_item) && (!movimento.id_nf || DataAccessTipoNotaFiscal.verificarTipoNotaFiscalPermiteMovimentoEstoque(item.cd_movimento))) || item.id_voucher_carga)
            {
                int tipoMvto = movimento.id_tipo_movimento;
                if ((movimento.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.DEVOLUCAO || movimento.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.SERVICO) && movimento.cd_tipo_nota_fiscal.HasValue)
                    tipoMvto = (int)getTipoMvtoTpNF(movimento.cd_tipo_nota_fiscal.Value);
                if (tipoMvto == (int)Movimento.TipoMovimentoEnum.SAIDA)
                {
                    itemServico.qt_estoque -= item.qt_item_movimento;
                    //itemServico.vl_item = decimal.Round(item.vl_liquido_item / item.qt_item_movimento, 2);
                    itemServico.vl_item = decimal.Round((decimal)item.vl_unitario_item, 2);

                    //LBM Vai ser controlado pelas triggers do kardex
                    //if (movimento.id_bloquear_venda_sem_estoque && itemServico.qt_estoque < 0)
                    //    throw new FinanceiroBusinessException(string.Format(Utils.Messages.Messages.msgEstoqueNegativoItemMovimento), null,
                    //                                FinanceiroBusinessException.TipoErro.ERRO_ESTOQUE_NEGATIVO, false);
                }
                if (tipoMvto == (int)Movimento.TipoMovimentoEnum.ENTRADA || item.id_voucher_carga)
                {
                    itemServico.qt_estoque += item.qt_item_movimento;
                    itemServico.vl_custo = decimal.Round(item.vl_liquido_item / item.qt_item_movimento, 2);
                }
                //Tipo de movimento de despesa não geram kardex
                if (tipoMvto != (int)Movimento.TipoMovimentoEnum.DESPESA || item.id_voucher_carga)
                {
                    //Atualiza quantidade do item no estoque.
                    kardexView.cd_registro_origem = item.cd_item_movimento;
                    BusinessFinan.addKardex(kardexView);
                }
            }
        }

        private void crudItensMovimentoKit(Movimento movimento)
        {
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                List<ItemMovimentoKit> itemMovimentoKitContext = DataAccessItemMovimentoKit.getItensMovimentoKitByMovimento(movimento.cd_movimento).ToList();

                IEnumerable<ItemMovimentoKit> itemMovimentoKitDeleted = itemMovimentoKitContext.Where(tc => !movimento.ItemMovimentoKit.Any(tv => tc.cd_item_movimento_kit == tv.cd_item_movimento_kit));
                if (itemMovimentoKitDeleted != null && itemMovimentoKitDeleted.Count() > 0)
                {
                    foreach (ItemMovimentoKit item in itemMovimentoKitDeleted)
                    {
                        DataAccessItemMovimentoKit.delete(item, false);
                    }
                }

                foreach (var itemKit in movimento.ItemMovimentoKit)
                {
                    // Novos itens movimento kit:
                    if (itemKit.cd_item_movimento_kit == 0)
                    {
                        itemKit.cd_movimento = movimento.cd_movimento;
                        ItemMovimentoKit novoItemKit = DataAccessItemMovimentoKit.add(itemKit, false);
                    }
                    else
                    {
                        var itemMovimento = itemMovimentoKitContext.Where(hc => hc.cd_item_movimento_kit == itemKit.cd_item_movimento_kit).FirstOrDefault();
                        if (itemMovimento != null && itemMovimento.cd_movimento > 0)
                        {
                            itemMovimento.qt_item_kit = itemKit.qt_item_kit;
                        }
                    }
                }
                DataAccessItemMovimentoKit.saveChanges(false);
                transaction.Complete();
            }
        }


        private void crudItensMovimentoNF(List<ItemMovimento> itensMovimentoView, Movimento movimento, bool houve_troca_tipo)
        {
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                this.sincronizarContextos(DataAccessMovimento.DB());
                
                List<ItemMovimento> novosItensKitMovimentoView = new List<ItemMovimento>();

                List<ItemMovimento> itemMovimentoContext = DataAccessItemMovimento.getItensMovimentoByMovimento(movimento.cd_movimento, movimento.cd_pessoa_empresa).ToList();
                IEnumerable<ItemMovimento> itemMovimentoComCodigo = from it in itensMovimentoView
                                                                    where it.cd_item_movimento != 0
                                                                    select it;

                IEnumerable<ItemMovimento> itemMovimentoDeleted = itemMovimentoContext.Where(tc => !itemMovimentoComCodigo.Any(tv => tc.cd_item_movimento == tv.cd_item_movimento));

                if (itemMovimentoDeleted != null && itemMovimentoDeleted.Count() > 0)
                    foreach (ItemMovimento item in itemMovimentoDeleted)
                        if (item != null)
                        {
                            if (item.cd_item_movimento > 0 && item.cd_item_kit > 0)
                            {
                                deleteItemMovItensKit(item);
                            }
                            DataAccessItemMovimento.delete(item, false);
                        }

                foreach (var item in itensMovimentoView)
                {
                    ItemEscola itemServico = DataAccessItemEscola.findItemEscolabyId(item.cd_item, movimento.cd_pessoa_empresa);

                    // Novos itens movimento:
                    if (item.cd_item_movimento == 0)
                    {
                        item.cd_movimento = movimento.cd_movimento;
                        ItemMovimento novoItem = DataAccessItemMovimento.add(item, false);

                    }
                    else
                    {
                        var itemMovimento = itemMovimentoContext.Where(hc => hc.cd_item_movimento == item.cd_item_movimento).FirstOrDefault();
                        if (itemMovimento != null && itemMovimento.cd_movimento > 0)
                        {
                            int tipoMovimento = movimento.id_tipo_movimento;
                            if ((movimento.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.DEVOLUCAO || movimento.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.SERVICO) && movimento.cd_tipo_nota_fiscal.HasValue)
                            {
                                tipoMovimento = (int)getTipoMvtoTpNF(movimento.cd_tipo_nota_fiscal.Value);
                                if (movimento.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.DEVOLUCAO)
                                    tipoMovimento = tipoMovimento == (int)Movimento.TipoMovimentoEnum.SAIDA ? (int)Movimento.TipoMovimentoEnum.ENTRADA : (int)Movimento.TipoMovimentoEnum.SAIDA;

                            }
                            itemMovimento = ItemMovimento.changeValueItemMovimento(itemMovimento, item, tipoMovimento);
                        }
                    }
                }
                DataAccessItemMovimento.saveChanges(false);
                DataAccessItemEscola.saveChanges(false);
                transaction.Complete();
            }
        }

        private void crudItensMovimento(List<ItemMovimento> itensMovimentoView, Movimento movimento, bool houve_troca_tipo)
        {
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                this.sincronizarContextos(DataAccessMovimento.DB());

                List<ItemMovimento> novosItensKitMovimentoView = new List<ItemMovimento>();

                List<ItemMovimento> itemMovimentoContext = DataAccessItemMovimento.getItensMovimentoByMovimento(movimento.cd_movimento, movimento.cd_pessoa_empresa).ToList();
                IEnumerable<ItemMovimento> itemMovimentoComCodigo = from it in itensMovimentoView
                                                                    where it.cd_item_movimento != 0
                                                                    select it;
                IEnumerable<ItemMovimento> itemMovimentoDeleted = itemMovimentoContext.Where(tc => !itemMovimentoComCodigo.Any(tv => tc.cd_item_movimento == tv.cd_item_movimento));

                if(itemMovimentoDeleted != null && itemMovimentoDeleted.Count() > 0)
                    foreach(ItemMovimento item in itemMovimentoDeleted)
                        if (item != null)
                        {
                            if (!houve_troca_tipo)
                                deletarKardexOfMovimento(item, movimento);
                            if (item.cd_item_movimento > 0 && item.cd_item_kit > 0)
                            {
                                deleteItemMovItensKit(item);
                            }
                            DataAccessItemMovimento.delete(item, false);

                        }
                        
                foreach (var item in itensMovimentoView)
                {
                    //TODO:Deivid
                    ItemEscola itemServico = DataAccessItemEscola.findItemEscolabyId(item.cd_item, movimento.cd_pessoa_empresa);
                    
                    //Valor do Kardex: quando for venda será o valor do custo do item escola, quando for compra será o valor unitário do item
                    Kardex kardexView = criaNovoKardex(item, movimento, itemServico);

                    // Novos itens movimento:
                    if (item.cd_item_movimento == 0)
                    {
                        item.cd_movimento = movimento.cd_movimento;


                        ItemMovimento novoItem = DataAccessItemMovimento.add(item, false);
                        novosItensKitMovimentoView.Add(novoItem);

                        //Incluindo Kardex
                        //Regras de estoque serão verificadas para itens que movimentam estoque
                        //TODO:Deivid
                        if(!movimento.id_venda_futura)
                        incluirKardexOfMovimento(novoItem, movimento, kardexView, itemServico);
                    }
                    //Alteração dos itens movimento:
                    else
                    {
                        var itemMovimento = itemMovimentoContext.Where(hc => hc.cd_item_movimento == item.cd_item_movimento).FirstOrDefault();
                        if (itemMovimento != null && itemMovimento.cd_movimento > 0)
                        {
                            //Atualiza quantidade do item no estoque.
                            ItemMovimento itemOld = new ItemMovimento();
                            itemOld.copy(itemMovimento);
                            //Metodos adição;
                            int tipoMovimento = movimento.id_tipo_movimento;
                            if ((movimento.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.DEVOLUCAO || movimento.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.SERVICO) && movimento.cd_tipo_nota_fiscal.HasValue)
                            {
                                tipoMovimento = (int)getTipoMvtoTpNF(movimento.cd_tipo_nota_fiscal.Value);
                                if (movimento.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.DEVOLUCAO)
                                    tipoMovimento = tipoMovimento == (int)Movimento.TipoMovimentoEnum.SAIDA ? (int)Movimento.TipoMovimentoEnum.ENTRADA : (int)Movimento.TipoMovimentoEnum.SAIDA;

                            }
                            itemMovimento = ItemMovimento.changeValueItemMovimento(itemMovimento, item, tipoMovimento);

                            if (!movimento.id_venda_futura)
                            {

                                if ((DataAccessTipoItem.verificaMovimentarEstoque(item.cd_item) && (!movimento.id_nf || DataAccessTipoNotaFiscal.verificarTipoNotaFiscalPermiteMovimentoEstoque(item.cd_movimento))) || item.id_voucher_carga)
                                {
                                    //TODO:Deivid
                                    if (tipoMovimento == (int)Movimento.TipoMovimentoEnum.SAIDA &&
                                        (DataAccessItemMovimento.DB().Entry(itemMovimento).Property(p => p.qt_item_movimento).IsModified ||
                                            DataAccessItemMovimento.DB().Entry(itemMovimento).Property(p => p.vl_unitario_item).IsModified))
                                    {
                                        itemServico.qt_estoque += itemOld.qt_item_movimento;
                                        itemServico.qt_estoque -= item.qt_item_movimento;
                                        itemServico.vl_item = decimal.Round((decimal)item.vl_unitario_item, 2);
                                        if (movimento.id_bloquear_venda_sem_estoque && itemServico.qt_estoque < 0)
                                            throw new FinanceiroBusinessException(string.Format(Utils.Messages.Messages.msgEstoqueNegativoItemMovimento), null,
                                                                        FinanceiroBusinessException.TipoErro.ERRO_ESTOQUE_NEGATIVO, false);
                                    }
                                    if (tipoMovimento == (int)Movimento.TipoMovimentoEnum.ENTRADA &&
                                            (DataAccessItemMovimento.DB().Entry(itemMovimento).Property(p => p.qt_item_movimento).IsModified ||
                                            DataAccessItemMovimento.DB().Entry(itemMovimento).Property(p => p.vl_liquido_item).IsModified))
                                    {
                                        itemServico.qt_estoque += item.qt_item_movimento;
                                        itemServico.qt_estoque -= itemOld.qt_item_movimento;
                                        itemServico.vl_custo = decimal.Round(item.vl_liquido_item / item.qt_item_movimento, 2);
                                    }
                                    //Alterando kardex
                                    SGFWebContext cdb = new SGFWebContext();
                                    Kardex kardexContext = BusinessFinan.getKardexByOrigem((byte)cdb.LISTA_ORIGEM_LOGS["ItemMovimento"], item.cd_item_movimento).FirstOrDefault();
                                    if (kardexContext != null && kardexContext.cd_registro_origem > 0)
                                        Kardex.changeValueKardex(kardexContext, kardexView);
                                }
                            }
                        }
                    }
                }

                includeItemMovItensKit(itensMovimentoView);
                DataAccessItemMovimento.saveChanges(false);
                DataAccessItemMovimentoItemKit.saveChanges(false);
                DataAccessItemEscola.saveChanges(false);
                transaction.Complete();
            }
        }

        private void includeItemMovItensKit(List<ItemMovimento> itensMovimentoView)
        {

            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {


                List<ItemMovItemKit> itemsMovItemKit = new List<ItemMovItemKit>();

                foreach (var item in itensMovimentoView)
                {

                    // Novos itens movimento:

                    if (item.cd_item_movimento > 0 && item.cd_item_kit > 0)
                    {

                        ItemMovItemKit itemMovItemKit = new ItemMovItemKit();
                        itemMovItemKit.cd_item_kit = item.cd_item_kit;
                        itemMovItemKit.cd_item_movimento = item.cd_item_movimento;

                        itemsMovItemKit.Add(itemMovItemKit);
                    }

                }

                itemsMovItemKit = itemsMovItemKit.Distinct().ToList();
                foreach (var itemMovItemKit in itemsMovItemKit)
                {
                    DataAccessItemMovimentoItemKit.add(itemMovItemKit, false);

                }

                transaction.Complete();
            }
        }

        private void deleteItemMovItensKit(ItemMovimento item)
        {

            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {

                ItemMovItemKit itemMovItemKit = DataAccessItemMovimentoItemKit.findByCdItemMovimentoAndCdItemKit(item.cd_item_movimento, item.cd_item_kit);

                if (itemMovItemKit != null)
                {
                    DataAccessItemMovimentoItemKit.delete(itemMovItemKit, false);
                }

                transaction.Complete();
            }
        }

        private void crudTitulosMovimento(List<Titulo> titulosView, Movimento movimento)
        {
            this.sincronizarContextos(DataAccessMovimento.DB());
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                List<Titulo> titulosAdd = new List<Titulo>();
                List<Titulo> titulosContext = DataAccessTitulo.getTitulosByMovimento(movimento.cd_movimento, movimento.cd_pessoa_empresa).ToList();
                IEnumerable<Titulo> titulosComCodigo = titulosView.Where(x => x.cd_titulo != 0);
                IEnumerable<Titulo> titulosDeleted = titulosContext.Where(tc => !titulosComCodigo.Any(tv => tc.cd_titulo == tv.cd_titulo));
                //remover o titulo e o plano conta correspondente.
                BusinessFinan.deleteAllTitulo(titulosDeleted.ToList(), movimento.cd_pessoa_empresa);

                foreach (Titulo item in titulosView)
                {
                    if(item.vl_titulo <= 0)
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
                            if (Titulo.editValuesTituloEditMovimento(titulo, item))
                            {
                                if (titulo.id_status_cnab != (int)Titulo.StatusCnabTitulo.INICIAL && titulo.id_status_cnab != (int)Titulo.StatusCnabTitulo.CONFIRMADO_PEDIDO_BAIXA)
                                    throw new FinanceiroBusinessException(Messages.msgNotUpdateTituloEnviadoCNAB, null, FinanceiroBusinessException.TipoErro.ERRO_TITULO_ENVIADO_CNAB, false);
                                if (titulo.id_cnab_contrato && titulo.id_status_cnab == (int)Titulo.StatusCnabTitulo.INICIAL)
                                    throw new FinanceiroBusinessException(Messages.msgNotUpdateTituloBoleto, null, FinanceiroBusinessException.TipoErro.ERRO_TITULO_EMITIDO_BOLETO, false);
                            }
                            item.nm_titulo = movimento.nm_movimento;
                            titulo = Titulo.changeValuesTituloEditMovimento(titulo, item);
                            if (titulo.vl_titulo != titulo.vl_saldo_titulo)
                                if (titulo.vl_liquidacao_titulo > 0 && DataAccessTitulo.DB().Entry(titulo).State == EntityState.Modified)
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

        public bool deleteMovimentos(int[] cdMovimentos, int cd_empresa)
        {
            bool retorno = false;
            SGFWebContext cdb = new SGFWebContext();

            if (cdMovimentos != null && cdMovimentos.Count() > 0)
            {
                List<Movimento> MovimentosContext = DataAccessMovimento.getMovimentos(cdMovimentos, cd_empresa).ToList();
                using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
                {
                    foreach (var m in MovimentosContext)
                    {
                        //LBM eliminei a regra pois para excluir uma matricula vinculada vai ter que excluir o movimento
                        //Na procedure que exclui a matricula não vai liberar se tiver vinculada.
                        //if (m.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.SAIDA && m.id_material_didatico == true && m.cd_origem_movimento > 0)
                        //{
                        //    throw new FinanceiroBusinessException(Utils.Messages.Messages.msgErrorDeleteMovimentoMaterialWithCdOrigem, null,
                        //        FinanceiroBusinessException.TipoErro.ERRO_DELETE_NOTA_MATERIAL_WITH_CONTRATO, false);
                        //}

                        if (m.id_nf && m.id_status_nf == (int)Movimento.StatusNFEnum.FECHADO ||
                            m.id_nf && m.id_status_nf == (int)Movimento.StatusNFEnum.CANCELADO)
                            throw new FinanceiroBusinessException(string.Format(string.Format(Utils.Messages.Messages.msgErroNFProcessadaOuCancelada, m.status_nf)), null,
                                                                           FinanceiroBusinessException.TipoErro.ERRO_NF_FECHADA_CANCELADA, false);
                        crudItensMovimento(new List<ItemMovimento>(), m, false);
                        crudTitulosMovimento(new List<Titulo>(), m);
                        retorno = DataAccessMovimento.delete(m, false);
                    }
                    transaction.Complete();
                }
            }
            return retorno;
        }

        public int? getMaxMovimento(int tpMovto, int cd_empresa)
        {
            return DataAccessMovimento.getMaxMovimento(tpMovto, cd_empresa);
        }

        public Espelho getEspelhoMovimento(int cd_movimento, int cd_empresa)
        {
            Espelho retorno = new Espelho();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = DataAccessMovimento.getEspelhoMovimento(cd_movimento, cd_empresa);
                transaction.Complete();
            }
            return retorno;
        }

        public List<Espelho> getSourceCopiaEspelhoMovimento(int cd_movimento, int cd_empresa)
        {
            List<Espelho> retorno = new List<Espelho>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = DataAccessMovimento.getSourceCopiaEspelhoMovimento(cd_movimento, cd_empresa);
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<ItemMovimento> getItensMovimentoByMovimento(int cd_movimento, int cd_empresa)
        {
            return DataAccessItemMovimento.getItensMovimentoByMovimento(cd_movimento, cd_empresa);
        }

        public IEnumerable<ItemMovimento> getItensMovimento(int cd_movimento, int cd_empresa)
        {
            IEnumerable<ItemMovimento> retorno = new List<ItemMovimento>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = DataAccessItemMovimento.getItensMovimento(cd_movimento, cd_empresa);
                transaction.Complete();
            }
            return retorno;
        }

        public Movimento getMovtoById(int cd_movimento)
        {
            return DataAccessMovimento.findById(cd_movimento, false);
        }

        public List<Titulo> addTitulosERateoMovimento(List<Titulo> titulos, List<ItemMovimento> itensMovto, int cd_movimento, int cd_empresa, int? nm_movimento)
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

        public List<Movimento> getMovimentosbyOrigem(int cd_origem, int id_origem_movto, int cd_empresa)
        {
            return DataAccessMovimento.getMovimentosbyOrigem(cd_origem, id_origem_movto, cd_empresa);
        }

        public List<ItemMovimento> getItensMovimentoByCdMovimentoPerdaMaterial(int cdMovimento)
        {
            return DataAccessMovimento.getItensMovimentoByCdMovimento(cdMovimento).ToList();
        }

        public Movimento getMovimentoEditOrigem(int cd_origem, int id_origem_movto, int cd_empresa, int id_tipo_movimento)
        {
           return  DataAccessMovimento.getMovimentoEditOrigem(cd_origem, id_origem_movto, cd_empresa, id_tipo_movimento);
        }

        public bool existeMovimentoByOrigem(int cdOrigem, int id_origem_movimento)
        {
            return DataAccessMovimento.existeMovimentoByOrigem(cdOrigem, id_origem_movimento);
        }

        public List<ItemMovimento> getItensMaterialAluno(List<int> cdAlunos, int cd_empresa, int cd_turma)
        {
            return DataAccessItemMovimento.getItensMaterialAluno(cdAlunos, cd_empresa, cd_turma);
        }

        public MovimentoUI getMovimentoReturnGrade(int cd_movimento, int cd_empresa)
        {
            return DataAccessMovimento.getMovimentoReturnGrade(cd_movimento, cd_empresa);
        }

        public Movimento updateMovimentoOlny(Movimento movimento)
        {
            Movimento movtoContext = DataAccessMovimento.getMovimento(movimento.cd_movimento, movimento.cd_pessoa_empresa);
            movtoContext = Movimento.changeValuesMovimento(movtoContext, movimento);
            DataAccessMovimento.saveChanges(false);
            return movimento;
        }
        
        public bool existeMovimentoForTit(int cd_pessoa_empresa, List<int> titulos)
        {
            return DataAccessMovimento.existeMovimentoForTit(cd_pessoa_empresa, titulos);
        }

        public List<ItemMovimento> getItensMvto(int cd_movimento, int cd_escola)
        {
            return DataAccessItemMovimento.getItensMvto(cd_movimento, cd_escola);
        }

        public Movimento getRetMovimentoDevolucao(int cd_movimento, int cd_empresa, int id_tipo_movimento, bool isMaster)
        {
            Movimento retorno = new Movimento();
            List<ItemMovimento> material = new List<ItemMovimento>();
            retorno = DataAccessMovimento.getRetMovimentoDevolucao(cd_movimento, cd_empresa, id_tipo_movimento);
            // Somente Master pode devolver Saídas
            List<ItemMovimento> somaItensDev = DataAccessItemMovimento.getSomatorioValoresItensMovimentoDevolucao(cd_movimento, cd_empresa).ToList();
            if (!isMaster && retorno.id_tipo_movimento == 2)
            {
                material = retorno.ItensMovimento.Where(i => i.id_material_didatico == true).ToList();
                if(material != null && material.Count() > 0)
                {
                    throw new FiscalBusinessException(string.Format(Utils.Messages.Messages.msgErroDevolucaoNF), null,
                                            FiscalBusinessException.TipoErro.ERRO_DEVOLUCAO, false);

                }
            }
            if (somaItensDev != null && somaItensDev.Count() > 0 && retorno != null && retorno.ItensMovimento != null && retorno.ItensMovimento.Count() > 0 )
                foreach (ItemMovimento im in somaItensDev)
                {
                    ItemMovimento item = retorno.ItensMovimento.Where(x => x.cd_item == im.cd_item).FirstOrDefault();
                    if (item != null)
                    {
                        //item.vl_unitario_item -= im.vl_unitario_item;
                        item.vl_total_item -= im.vl_total_item;
                        item.vl_liquido_item -= im.vl_liquido_item;
                        //item.vl_acrescimo_item -= im.vl_acrescimo_item;
                        //item.vl_desconto_item -= im.vl_desconto_item;
                        item.vl_base_calculo_ICMS_item -= im.vl_base_calculo_ICMS_item;
                        item.vl_ICMS_item -= im.vl_ICMS_item;
                        item.vl_base_calculo_PIS_item -= im.vl_base_calculo_PIS_item;
                        item.vl_PIS_item -= im.vl_PIS_item;
                        item.vl_base_calculo_COFINS_item -= im.vl_base_calculo_COFINS_item;
                        item.vl_COFINS_item -= im.vl_COFINS_item;
                        item.vl_base_calculo_IPI_item -= im.vl_base_calculo_IPI_item;
                        item.vl_IPI_item -= im.vl_IPI_item;
                        item.vl_aproximado -= im.vl_aproximado;
                        item.pc_aliquota_aproximada -= im.pc_aliquota_aproximada;
                        item.qt_item_movimento -= im.qt_item_movimento;
                        item.qt_item_movimento_dev = item.qt_item_movimento;
                        item.qtd_item_devolvido = im.qt_item_movimento;
                        //Movimento
                        retorno.vl_base_calculo_ICMS_nf -= im.vl_base_calculo_ICMS_item;
                        retorno.vl_ICMS_nf -= im.vl_ICMS_item;
                        retorno.vl_base_calculo_PIS_nf -= im.vl_base_calculo_PIS_item;
                        retorno.vl_PIS_nf -= im.vl_PIS_item;
                        retorno.vl_base_calculo_COFINS_nf -= im.vl_base_calculo_COFINS_item;
                        retorno.vl_COFINS_nf -= im.vl_COFINS_item;
                        retorno.vl_base_calculo_IPI_nf -= im.vl_base_calculo_IPI_item;
                        retorno.vl_IPI_nf -= im.vl_IPI_item;
                        retorno.pc_aliquota_aproximada = im.pc_aliquota_aproximada;
                    }
                }
            return retorno;
        }

        public IEnumerable<ItemMovimento> getItensMovimentoReciboLeitura(int cd_movimento, int cd_empresa)
        {
            IEnumerable<ItemMovimento> retorno = new List<ItemMovimento>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = DataAccessItemMovimento.getItensMovimentoRecibo(cd_movimento, cd_empresa).ToList();
                transaction.Complete();
            }
            return retorno;
        }

        public int getUltimoNroRecibo(int? nm_ultimo_Recibo, int cd_empresa)
        {
            return DataAccessMovimento.getUltimoNroRecibo(nm_ultimo_Recibo, cd_empresa);
        }

        public bool existeICMSEstadoNota(int cdEstadoOri, int cdEstadoDes)
        {
            return DataAccessMovimento.existeICMSEstadoNota(cdEstadoOri, cdEstadoDes);
        }

        public bool existeDadosNFNota(int cdCidade)
        {
            return DataAccessMovimento.existeDadosNFNota(cdCidade);
        }

        #endregion

        #region Nota Fiscal

        public Movimento processarNF(int cdEscola, int cd_movimento, bool empresaPropria)
        {
            sincronizarContextos(DataAccessMovimento.DB());
            //using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            //{
                Movimento movtoContext = DataAccessMovimento.getMovimento(cd_movimento, cdEscola);

                if (!movtoContext.id_nf)
                    throw new FiscalBusinessException(string.Format(Utils.Messages.Messages.msgErroMovimentoNotNF), null,
                                            FiscalBusinessException.TipoErro.ERRO_PROCESSAR_NF, false);

                if (movtoContext.id_status_nf != (int)Movimento.StatusNFEnum.ABERTO)
                    throw new FiscalBusinessException(string.Format(Utils.Messages.Messages.msgErroProcessarNFAberto, movtoContext.status_nf), null,
                                                             FiscalBusinessException.TipoErro.ERRO_PROCESSAR_NF, false);

                //Verifica se existe algum item cadastrado para movimento:
                if (!DataAccessMovimento.existeItemNoMovimento(cd_movimento))
                    throw new FiscalBusinessException(Utils.Messages.Messages.msgErroProcessarMovimentoSemItem, null,
                                                             FiscalBusinessException.TipoErro.ERRO_PROCESSAR_MOVIMENTO_SEM_ITEM, false);

                //Verifica se existe algum item cadastrado para movimento:
                if (DataAccessMovimento.existeItemZeradoNoMovimento(cd_movimento))
                    throw new FiscalBusinessException(Utils.Messages.Messages.msgErroProcessarMovimentoItemZerado, null,
                                                             FiscalBusinessException.TipoErro.ERRO_PROCESSAR_MOVIMENTO_COM_ITEM_ZERADO, false);

                //Verifica se existe nota fiscal já processada com data superior a nota fiscal á processar.
                if (movtoContext.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.SAIDA || movtoContext.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.SERVICO)
                {
                    if (DataAccessMovimento.existeMovimentoComDataSuperior(movtoContext.id_tipo_movimento, movtoContext.dt_mov_movimento, movtoContext.cd_pessoa_empresa))
                        throw new FiscalBusinessException(Utils.Messages.Messages.msgErroExisteNotaFiscalComDataPosteriorProcessada, null,
                                                             FiscalBusinessException.TipoErro.ERRO_NOTAS_POSTERIORES_PROCESSADOS, false);
                    if (DataAccessMovimento.existeMovimentosAbertosComDataAnterior(movtoContext.id_tipo_movimento, movtoContext.dt_mov_movimento, movtoContext.cd_pessoa_empresa))
                        throw new FiscalBusinessException(Utils.Messages.Messages.msgErroExisteNotasAnterioresEmABerto, null,
                                                             FiscalBusinessException.TipoErro.ERRO_NOTAS_ANTERIORES_EM_ABERTAS, false);
                }
                if (movtoContext.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.DEVOLUCAO){
                    if(DataAccessMovimento.verificaSeItensNotaDevolucaoExtrapolouNotaOrig((int)movtoContext.cd_nota_fiscal,movtoContext.cd_movimento, movtoContext.cd_pessoa_empresa))
                        throw new FiscalBusinessException(Messages.msgErroExtrapolouItensNFOriginal, null, FiscalBusinessException.TipoErro.ERRO_PROCESSAR_NF, false);
                    int tipoMovimento = (int)getTipoMvtoTpNF(movtoContext.cd_tipo_nota_fiscal.Value) == (int)Movimento.TipoMovimentoEnum.SAIDA ? (int)Movimento.TipoMovimentoEnum.ENTRADA : (int)Movimento.TipoMovimentoEnum.SAIDA;
                    if (empresaPropria && DataAccessMovimento.notaFiscalDevolvidaComChaveAcesso(tipoMovimento, (int)movtoContext.cd_nota_fiscal, cdEscola))
                        throw new FiscalBusinessException(Utils.Messages.Messages.msgErrorProcNFDevolucao, null,
                                                             FiscalBusinessException.TipoErro.ERRO_PROCESSAR_NF, false);
                    }

                
                if (movtoContext.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.ENTRADA) 
                {
                    if (empresaPropria && string.IsNullOrEmpty(movtoContext.dc_key_nfe)) // Verifica se nota fiscal possui chave de acesso.
                        throw new FiscalBusinessException(Utils.Messages.Messages.msgErroExisteChaveAcessoNF, null, FiscalBusinessException.TipoErro.ERRO_CHAVE_ACESSO_NF, false);
                    else if (empresaPropria)
                    {
                        var keynfe = Regex.Replace(movtoContext.dc_key_nfe, "[^0-9]+", ""); // expressão regular que permite apenas números na string.
                        if(keynfe.Count() != 44) // Verifica se a chave de acesso é válida.
                            throw new FiscalBusinessException(Utils.Messages.Messages.msgErroChaveAcessoInvalido, null, FiscalBusinessException.TipoErro.ERRO_CHAVE_ACESSO_INVALIDA, false);
                    }
                }

                movtoContext.id_status_nf = (int)Movimento.StatusNFEnum.FECHADO;
                int retorno = DataAccessMovimento.saveChanges(false);
                //transaction.Complete();
                return movtoContext;
            //}
        }

        public XmlDocument emitirNF(int cdEscola, int cd_movimento, int id_tipo_movimento, ref int nm_nota_fiscal)
        {
            XmlDocument retorno = new XmlDocument();
            Movimento movtoContext = DataAccessMovimento.getMovimento(cd_movimento, cdEscola);

            if(movtoContext.nm_movimento.HasValue)
                nm_nota_fiscal = movtoContext.nm_movimento.Value;
            if(!movtoContext.id_nf)
                throw new FiscalBusinessException(string.Format(Utils.Messages.Messages.msgErroGerarXMLTerceiro), null,
                                        FiscalBusinessException.TipoErro.ERRO_PROCESSAR_NF, false);

            if (!movtoContext.id_status_nf.HasValue || movtoContext.id_status_nf == (int)Movimento.StatusNFEnum.ABERTO)
                throw new FiscalBusinessException(string.Format(Utils.Messages.Messages.msgErroEmitirNFAberto, movtoContext.status_nf), null,
                                                            FiscalBusinessException.TipoErro.ERRO_PROCESSAR_NF, false);

            switch(id_tipo_movimento) {
                case (byte) Movimento.TipoMovimentoEnum.SAIDA:
                case (byte) Movimento.TipoMovimentoEnum.DEVOLUCAO:
                    Movimento movimento = getMovimentosNotaProduto(cd_movimento);
                    retorno = emitirNFProduto(movimento, id_tipo_movimento);
                    break;
                case (byte)Movimento.TipoMovimentoEnum.SERVICO:
                    List<int> movimentos = new List<int>();
                    movimentos.Add(cd_movimento);
                    List<Movimento> listMovimento = getMovimentosNotaServico(movimentos);
                    retorno = emitirNFServico(listMovimento);
                    break;
            }
            retorno = XmlDocumentCreator.RemoveAllNamespaces(retorno);
             
            return retorno;
        }

        public XmlDocument emitirNFS(int cdEscola, List<int> movimentos, ref int nm_nota_fiscal) {
            XmlDocument retorno = new XmlDocument();
            List<Movimento> listMovimento = getMovimentosNotaServico(movimentos);

            if(listMovimento.Count == 1)
                nm_nota_fiscal = listMovimento[0].nm_movimento.Value;
            retorno = emitirNFServico(listMovimento);
                   
            return retorno;
        }

        private List<Movimento> getMovimentosNotaServico(List<int> cd_movimentos) {
            return DataAccessMovimento.getMovimentosNotaServico(cd_movimentos);
        }

        private Movimento getMovimentosNotaProduto(int cd_movimento){
            return DataAccessMovimento.getMovimentosNotaProduto(cd_movimento);
        }

        private XmlDocument emitirNFServico(List<Movimento> listMovimento) {
            XmlDocument doc = new XmlDocument();
            string xmlData = "<?xml version=\"1.0\"?><Notas></Notas>";

            doc.Load(new StringReader(xmlData));
            XmlElement elemNotas = (XmlElement) doc.GetElementsByTagName("Notas")[0];

            if(listMovimento != null)
                foreach(Movimento movimento in listMovimento) {
                    XmlElement elemNFS_e = XmlDocumentCreator.addElement(elemNotas, "NFS-e", null);
                    XmlElement elemPrestador = XmlDocumentCreator.addElement(elemNFS_e, "Prestador", null);

                    XmlDocumentCreator.addElement(elemPrestador, "RazaoSocialPrestador", movimento.Empresa.no_pessoa);
                    if(!String.IsNullOrEmpty(movimento.Empresa.dc_num_cgc))
                        XmlDocumentCreator.addElement(elemPrestador, "CNPJ", movimento.Empresa.dc_num_cgc.Replace(".", "").Replace("/", "").Replace("-", ""));
                    XmlDocumentCreator.addElement(elemPrestador, "InscricaoMunicipal", movimento.Empresa.dc_num_insc_municipal);

                    XmlElement elemRPS = XmlDocumentCreator.addElement(elemNFS_e, "RPS", null);
                    XmlDocumentCreator.addElement(elemRPS, "SerieRPS", movimento.dc_serie_movimento);
                    XmlDocumentCreator.addElement(elemRPS, "NumeroRPS", movimento.nm_movimento.HasValue ? movimento.nm_movimento + "" : null);
                    XmlDocumentCreator.addElement(elemRPS, "TipoRPS", "1");
                    XmlDocumentCreator.addElement(elemRPS, "CRT", movimento.TipoNF.id_regime_tributario+"");
                    
                    XmlDocumentCreator.addElement(elemRPS, "DataEmissao", movimento.dta_emissao_movimento);
                    XmlDocumentCreator.addElement(elemRPS, "StatusRPS", movimento.id_status_nf.HasValue ? movimento.id_status_nf + "": null);
                    XmlDocumentCreator.addElement(elemRPS, "TributacaoRPS", movimento.Empresa.EnderecoPrincipal.Cidade.DadosNFCidade.dc_tributacao_municipio);
                    XmlDocumentCreator.addElement(elemRPS, "CodigoServico", movimento.Empresa.EnderecoPrincipal.Cidade.DadosNFCidade.dc_item_servico);

                    XmlElement elemImpostos = XmlDocumentCreator.addElement(elemRPS, "Impostos", null);
                    XmlDocumentCreator.addElement(elemImpostos, "AliqTotTrib", movimento.pcAliquotaAproximada);
                    XmlDocumentCreator.addElement(elemImpostos, "ValorTotTrib", movimento.vlAproximado);
                    XmlDocumentCreator.addElement(elemImpostos, "ValorServicos", movimento.vl_total_geral);
                    XmlDocumentCreator.addElement(elemImpostos, "ValorDeducoes", "0.00");
                    XmlDocumentCreator.addElement(elemImpostos, "AliquotaServicos", string.Format(CultureInfo.InvariantCulture, "{0:0.0000}", movimento.Empresa.EnderecoPrincipal.Cidade.DadosNFCidade.pc_aliquota_iss));
                    XmlDocumentCreator.addElement(elemImpostos, "ValorISS", movimento.vlISS_NF);
                    XmlDocumentCreator.addElement(elemImpostos, "ISSRetido", "false");
                    

                    XmlElement elemTomador = XmlDocumentCreator.addElement(elemRPS, "Tomador", null);
                    XmlElement elemCPFCNPJTomador = XmlDocumentCreator.addElement(elemTomador, "CPFCNPJTomador", null);
                    if(!String.IsNullOrEmpty(movimento.Pessoa.nm_cpf_cgc))
                        XmlDocumentCreator.addElement(elemCPFCNPJTomador, "CPFTomador", movimento.Pessoa.nm_cpf_cgc.Replace(".", "").Replace("/", "").Replace("-", ""));
                    XmlDocumentCreator.addElement(elemTomador, "RazaoSocialTomador", movimento.Pessoa.no_pessoa);
                    XmlElement elemEnderecoTomador = XmlDocumentCreator.addElement(elemTomador, "EnderecoTomador", null);
                    XmlDocumentCreator.addElement(elemEnderecoTomador, "TipoLogradouro", movimento.Pessoa.EnderecoPrincipal.TipoLogradouro.no_tipo_logradouro);
                    XmlDocumentCreator.addElement(elemEnderecoTomador, "Logradouro", movimento.Pessoa.EnderecoPrincipal.Logradouro.no_localidade);
                    XmlDocumentCreator.addElement(elemEnderecoTomador, "NumeroEndereco", movimento.Pessoa.EnderecoPrincipal.dc_num_endereco);
                    XmlDocumentCreator.addElement(elemEnderecoTomador, "ComplementoEndereco", movimento.Pessoa.EnderecoPrincipal.dc_compl_endereco);
                    XmlDocumentCreator.addElement(elemEnderecoTomador, "Bairro", movimento.Pessoa.EnderecoPrincipal.Bairro.no_localidade);
                    XmlDocumentCreator.addElement(elemEnderecoTomador, "Cidade", movimento.Pessoa.EnderecoPrincipal.Cidade.no_localidade);
                    XmlDocumentCreator.addElement(elemEnderecoTomador, "UF", movimento.Pessoa.EnderecoPrincipal.Estado.Estado.sg_estado);
                    if(!String.IsNullOrEmpty(movimento.Pessoa.EnderecoPrincipal.Logradouro.dc_num_cep))
                        XmlDocumentCreator.addElement(elemEnderecoTomador, "CEP", movimento.Pessoa.EnderecoPrincipal.Logradouro.dc_num_cep.Replace("-", ""));

                    XmlDocumentCreator.addElement(elemTomador, "EmailTomador", movimento.Pessoa.Telefone.dc_fone_mail);
                    XmlDocumentCreator.addElement(elemTomador, "Discriminacao", movimento.tx_obs_fiscal);
                }
            return doc;
        }

        private XmlDocument emitirNFProduto(Movimento movimento, int id_movimento) {
            XmlDocument doc = new XmlDocument();
            string xmlData = "<nfeProc versao=\"4.00\"><NFe><infNFe versao=\"4.00\"></infNFe></NFe></nfeProc>";

            doc.Load(new StringReader(xmlData));

            XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", "yes");
            XmlElement root = doc.DocumentElement;
            doc.InsertBefore(docNode, root);
            
            XmlElement elemInfNFe = (XmlElement) doc.GetElementsByTagName("infNFe")[0];

            if(movimento != null) {
                XmlElement elemIde = XmlDocumentCreator.addElement(elemInfNFe, "ide", null);
                
                int id_nat_op = movimento.TipoNF.id_natureza_movimento == (byte)Movimento.TipoMovimentoEnum.ENTRADA ? 0 : 1;
                XmlDocumentCreator.addElement(elemIde, "cUF", movimento.Empresa.EnderecoPrincipal.Estado.Estado.sg_estado);
                XmlDocumentCreator.addElement(elemIde, "cNF", movimento.cd_movimento+"");
                XmlDocumentCreator.addElement(elemIde, "natOp", movimento.TipoNF.dc_natureza_operacao);
                XmlDocumentCreator.addElement(elemIde, "indPag", "2");
                XmlDocumentCreator.addElement(elemIde, "mod", "55");
                XmlDocumentCreator.addElement(elemIde, "serie", movimento.dc_serie_movimento);
                if(movimento.nm_movimento.HasValue)
                    XmlDocumentCreator.addElement(elemIde, "nNF", movimento.nm_movimento+"");
                XmlDocumentCreator.addElement(elemIde, "dhEmi", movimento.dta_emissao_movimento_produto);
                XmlDocumentCreator.addElement(elemIde, "dhSaiEnt", movimento.dta_mov_movimento_produto);
                XmlDocumentCreator.addElement(elemIde, "tpNF", id_nat_op + "");
                XmlDocumentCreator.addElement(elemIde, "idDest", "3");
                if(movimento.Empresa.EnderecoPrincipal.Cidade.nm_municipio.HasValue)
                    XmlDocumentCreator.addElement(elemIde, "cMunFG", movimento.Empresa.EnderecoPrincipal.Cidade.nm_municipio+"");
                XmlDocumentCreator.addElement(elemIde, "tpImp", "1");
                XmlDocumentCreator.addElement(elemIde, "tpEmis", "1");
                XmlDocumentCreator.addElement(elemIde, "cDV", "0");
                XmlDocumentCreator.addElement(elemIde, "tpAmb", "1");
                XmlDocumentCreator.addElement(elemIde, "finNFe", movimento.id_tipo_movimento == (byte)Movimento.TipoMovimentoEnum.DEVOLUCAO ? "1" : "4");
                XmlDocumentCreator.addElement(elemIde, "indFinal", "1");
                XmlDocumentCreator.addElement(elemIde, "indPres", "1");
                XmlDocumentCreator.addElement(elemIde, "procEmi", "0");
                XmlDocumentCreator.addElement(elemIde, "verProc", "1.0");
                if((byte) Movimento.TipoMovimentoEnum.DEVOLUCAO == id_movimento)
                    XmlDocumentCreator.addElement(elemIde, "NFref", movimento.nm_movimento+"");
                
                XmlElement elemEmit = XmlDocumentCreator.addElement(elemInfNFe, "emit", null);

                if(!String.IsNullOrEmpty(movimento.Empresa.dc_num_cgc))
                    XmlDocumentCreator.addElement(elemEmit, "CNPJ", movimento.Empresa.dc_num_cgc.Replace(".", "").Replace("/", "").Replace("-", ""));
                XmlDocumentCreator.addElement(elemEmit, "xNome", movimento.Empresa.no_pessoa);
                XmlDocumentCreator.addElement(elemEmit, "xFant", movimento.Empresa.dc_reduzido_pessoa);
                XmlElement elemEnderEmit = XmlDocumentCreator.addElement(elemEmit, "enderEmit", null);

                XmlDocumentCreator.addElement(elemEnderEmit, "xLgr", movimento.Empresa.EnderecoPrincipal.Logradouro.no_localidade);
                XmlDocumentCreator.addElement(elemEnderEmit, "nro", movimento.Empresa.EnderecoPrincipal.dc_num_endereco);
                XmlDocumentCreator.addElement(elemEnderEmit, "xBairro", movimento.Empresa.EnderecoPrincipal.Bairro.no_localidade);
                if(movimento.Empresa.EnderecoPrincipal.Cidade.nm_municipio.HasValue)
                    XmlDocumentCreator.addElement(elemEnderEmit, "cMun", movimento.Empresa.EnderecoPrincipal.Cidade.nm_municipio+"");
                XmlDocumentCreator.addElement(elemEnderEmit, "xMun", movimento.Empresa.EnderecoPrincipal.Cidade.no_localidade);
                XmlDocumentCreator.addElement(elemEnderEmit, "UF", movimento.Empresa.EnderecoPrincipal.Estado.Estado.sg_estado);
                if(!String.IsNullOrEmpty(movimento.Empresa.EnderecoPrincipal.Logradouro.dc_num_cep))
                    XmlDocumentCreator.addElement(elemEnderEmit, "CEP", movimento.Empresa.EnderecoPrincipal.Logradouro.dc_num_cep.Replace("-", ""));
                XmlDocumentCreator.addElement(elemEnderEmit, "cPais", movimento.Empresa.EnderecoPrincipal.Pais.Pais.dc_num_pais);
                XmlDocumentCreator.addElement(elemEnderEmit, "xPais", movimento.Empresa.EnderecoPrincipal.Pais.Pais.dc_pais);

                XmlDocumentCreator.addElement(elemEmit, "IE", movimento.Empresa.dc_num_insc_estadual);
                XmlDocumentCreator.addElement(elemEmit, "IM", movimento.Empresa.dc_num_insc_municipal);
                XmlDocumentCreator.addElement(elemEmit, "CRT", movimento.TipoNF.id_regime_tributario+"");

                XmlElement elemDest = XmlDocumentCreator.addElement(elemInfNFe, "dest", null);
                if(!String.IsNullOrEmpty(movimento.Pessoa.nm_cpf_cgc))
                    XmlDocumentCreator.addElement(elemDest, "CPF", movimento.Pessoa.nm_cpf_cgc.Replace(".", "").Replace("/", "").Replace("-", ""));
                XmlDocumentCreator.addElement(elemDest, "xNome", movimento.Pessoa.no_pessoa);
                XmlElement elemEnderDest = XmlDocumentCreator.addElement(elemDest, "enderDest", null);
                XmlDocumentCreator.addElement(elemEnderDest, "xLgr", movimento.Pessoa.EnderecoPrincipal.Logradouro.no_localidade);
                XmlDocumentCreator.addElement(elemEnderDest, "nro", movimento.Pessoa.EnderecoPrincipal.dc_num_endereco);
                XmlDocumentCreator.addElement(elemEnderDest, "xBairro", movimento.Pessoa.EnderecoPrincipal.Bairro.no_localidade);
                if(movimento.Pessoa.EnderecoPrincipal.Cidade.nm_municipio.HasValue)
                    XmlDocumentCreator.addElement(elemEnderDest, "cMun", movimento.Pessoa.EnderecoPrincipal.Cidade.nm_municipio+"");
                XmlDocumentCreator.addElement(elemEnderDest, "xMun", movimento.Pessoa.EnderecoPrincipal.Cidade.no_localidade);
                XmlDocumentCreator.addElement(elemEnderDest, "UF", movimento.Pessoa.EnderecoPrincipal.Estado.Estado.sg_estado);
                if(!String.IsNullOrEmpty(movimento.Pessoa.EnderecoPrincipal.Logradouro.dc_num_cep))
                    XmlDocumentCreator.addElement(elemEnderDest, "CEP", movimento.Pessoa.EnderecoPrincipal.Logradouro.dc_num_cep.Replace("-", ""));
                XmlDocumentCreator.addElement(elemEnderDest, "cPais", movimento.Pessoa.EnderecoPrincipal.Pais.Pais.dc_num_pais);
                XmlDocumentCreator.addElement(elemEnderDest, "xPais", movimento.Pessoa.EnderecoPrincipal.Pais.Pais.dc_pais);

                XmlDocumentCreator.addElement(elemDest, "indIEDest", (movimento.Pessoa.nm_natureza_pessoa == (byte) PessoaSGF.TipoPessoa.JURIDICA && !String.IsNullOrEmpty(movimento.Pessoa.dc_num_insc_estadual_PJ)) ? "1" : "9");
                XmlDocumentCreator.addElement(elemDest, "email", movimento.Pessoa.Telefone.dc_fone_mail);

                if(movimento.ItensMovimento != null) {
                    int i = 1;
                    foreach(ItemMovimento item in movimento.ItensMovimento) {
                        criaTagXMLItemProduto(doc, elemInfNFe, i, item);
                        i += 1;
                    }
                }

                XmlElement elemTotal = XmlDocumentCreator.addElement(elemInfNFe, "total", null);
                XmlElement elemICMSTot = XmlDocumentCreator.addElement(elemTotal, "ICMSTot", null);
                XmlDocumentCreator.addElement(elemICMSTot, "vBC", movimento.vlBaseCalculoICMS_NFInvariante);
                XmlDocumentCreator.addElement(elemICMSTot, "vICMS", movimento.vlICMS_NFInvariante);
                XmlDocumentCreator.addElement(elemICMSTot, "vICMSDeson", "0.00");
                XmlDocumentCreator.addElement(elemICMSTot, "vBCST", "0.00");
                XmlDocumentCreator.addElement(elemICMSTot, "vST", "0.00");
                if(movimento.ItensMovimento != null)
                    XmlDocumentCreator.addElement(elemICMSTot, "vProd", string.Format(CultureInfo.InvariantCulture, "{0:0.00}", movimento.ItensMovimento.Sum(im => im.vl_total_item)));
                XmlDocumentCreator.addElement(elemICMSTot, "vFrete", "0.00");
                XmlDocumentCreator.addElement(elemICMSTot, "vSeg", "0.00");
                XmlDocumentCreator.addElement(elemICMSTot, "vDesc", movimento.vlDescontoInvariante);
                XmlDocumentCreator.addElement(elemICMSTot, "vII", "0.00");
                XmlDocumentCreator.addElement(elemICMSTot, "vIPI", movimento.vl_IPI_Nf_Invariante);
                XmlDocumentCreator.addElement(elemICMSTot, "vPIS", movimento.vl_PIS_Nf_Invariante);
                XmlDocumentCreator.addElement(elemICMSTot, "vCOFINS", movimento.vl_COFINS_Nf_Invariante);
                XmlDocumentCreator.addElement(elemICMSTot, "vOutro", "0.00");
                if(movimento.ItensMovimento != null)
                    XmlDocumentCreator.addElement(elemICMSTot, "vNF", string.Format(CultureInfo.InvariantCulture, "{0:0.00}", movimento.ItensMovimento.Sum(im => im.vl_liquido_item)));
                //XmlDocumentCreator.addElement(elemICMSTot, "vTotTrib", movimento.vlTotTribute);
                XmlDocumentCreator.addElement(elemICMSTot, "vTotTrib", movimento.vlAproximado);

                XmlElement elemTransp = XmlDocumentCreator.addElement(elemInfNFe, "transp", null);
                XmlDocumentCreator.addElement(elemTransp, "modFrete", "9");

                XmlElement elemInfAdic = XmlDocumentCreator.addElement(elemInfNFe, "infAdic", null);
                XmlDocumentCreator.addElement(elemInfAdic, "infCpl", movimento.tx_obs_fiscal);
            }

            return doc;    
        }

        private void criaTagXMLItemProduto(XmlDocument doc, XmlElement elemInfNFe, int i, ItemMovimento item) {
            XmlElement elemDet = XmlDocumentCreator.addElement(elemInfNFe, "det", null);
            XmlDocumentCreator.addAttribute(elemDet, doc, "nItem", i+"");
            XmlElement elemProd = XmlDocumentCreator.addElement(elemDet, "prod", null);

            XmlDocumentCreator.addElement(elemProd, "cProd", item.cd_item+"");
            XmlDocumentCreator.addElement(elemProd, "cEAN", null);
            XmlDocumentCreator.addElement(elemProd, "xProd", item.dc_item_movimento);
            XmlDocumentCreator.addElement(elemProd, "NCM", item.Item.dc_classificacao_fiscal);
            string cfop = "";
            if(item.CFOP.nm_cfop != 0)
                cfop = item.dc_cfop + item.CFOP.nm_cfop;
            else
                cfop = item.dc_cfop;
//item.Item.cd_integracao != null ? item.Item.cd_integracao+ "": ""
            XmlDocumentCreator.addElement(elemProd, "CFOP", cfop);
            XmlDocumentCreator.addElement(elemProd, "uCom", item.Item.dc_sgl_item);
            XmlDocumentCreator.addElement(elemProd, "qCom", item.qtItemMovimento);
            XmlDocumentCreator.addElement(elemProd, "vUnCom", item.vlUnitarioItemInvariante);
            XmlDocumentCreator.addElement(elemProd, "vProd", string.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.vlTotalItem));
            XmlDocumentCreator.addElement(elemProd, "cEANTrib", null);
            XmlDocumentCreator.addElement(elemProd, "uTrib", item.Item.dc_sgl_item);
            XmlDocumentCreator.addElement(elemProd, "qTrib", item.qtItemMovimento);
            XmlDocumentCreator.addElement(elemProd, "vUnTrib", item.vlUnitarioItemInvariante);
            XmlDocumentCreator.addElement(elemProd, "indTot", "1");

            XmlElement elemImposto = XmlDocumentCreator.addElement(elemDet, "imposto", null);
            //XmlDocumentCreator.addElement(elemImposto, "vTotTrib", string.Format(CultureInfo.InvariantCulture, "{0:0.00}", item.vl_ICMS_item + item.vl_PIS_item + item.vl_COFINS_item + item.vl_IPI_item));
            XmlDocumentCreator.addElement(elemImposto, "vTotTrib", item.vlAproximado);
            XmlElement elemICMS = XmlDocumentCreator.addElement(elemImposto, "ICMS", null);
            XmlElement elemICMS00 = XmlDocumentCreator.addElement(elemICMS, "ICMS00", null);
            XmlDocumentCreator.addElement(elemICMS00, "orig", "0");
            XmlDocumentCreator.addElement(elemICMS00, "CST", item.SituacaoTribICMS.nm_situacao_tributaria);
            XmlDocumentCreator.addElement(elemICMS00, "modBC", "3");
            XmlDocumentCreator.addElement(elemICMS00, "vBC", item.vlBaseCalculoICMSItemInvariante);
            XmlDocumentCreator.addElement(elemICMS00, "pICMS", item.pcAliquotaICMSInvariante);
            XmlDocumentCreator.addElement(elemICMS00, "vICMS", item.vlICMSItemInvariante);

            XmlElement elemPIS = XmlDocumentCreator.addElement(elemImposto, "PIS", null);
            XmlElement elemPISNT = XmlDocumentCreator.addElement(elemPIS, "PISNT", null);
            XmlDocumentCreator.addElement(elemPISNT, "CST", item.SituacaoTribPIS.nm_situacao_tributaria);

            XmlElement elemCOFINS = XmlDocumentCreator.addElement(elemImposto, "COFINS", null);
            XmlElement elemCOFINSNT = XmlDocumentCreator.addElement(elemCOFINS, "COFINSNT", null);
            XmlDocumentCreator.addElement(elemCOFINSNT, "CST", item.SituacaoTribCOFINS.nm_situacao_tributaria);
        }

        public void cancelarNFServico(int cd_escola, int cd_movimento, string dc_justificativa_nf, bool id_empresa_propria)
        {

            BusinessFinan.sincronizarContextos(DataAccessMovimento.DB());
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                Movimento movtoContext = postVerificarCancelamentoNF(cd_escola, cd_movimento, id_empresa_propria);

                if(movtoContext.id_tipo_movimento == (int) FundacaoFisk.SGF.GenericModel.ContaCorrente.Tipo.ENTRADA)
                    movtoContext.id_status_nf = (int) Movimento.StatusNFEnum.ABERTO;
                else // Saída
                    movtoContext.id_status_nf = (int) Movimento.StatusNFEnum.CANCELADO;
                movtoContext.dc_justificativa_nf = dc_justificativa_nf;
                DataAccessMovimento.saveChanges(false);
                if (movtoContext.id_tipo_movimento != (int)Movimento.TipoMovimentoEnum.ENTRADA || movtoContext.id_nf)
                {
                    //Exclui os títulos:
                    List<Titulo> titulos = DataAccessTitulo.getTitulosByMovimento(cd_movimento, cd_escola).ToList();
                    try
                    {
                        BusinessFinan.deleteAllTitulo(titulos, cd_escola);
                    }
                    catch (FinanceiroBusinessException fbe)
                    {
                        if (fbe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_TITULO_ENVIADO_CNAB || fbe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_TITULO_COM_BAIXA)
                            throw new FinanceiroBusinessException(Messages.msgErroCancelarNFBaixado, null, FinanceiroBusinessException.TipoErro.ERRO_TITULO_COM_BAIXA, false);
                    }
                    //Exclui os kardex:
                    List<ItemMovimento> itemMovimentoContext = DataAccessItemMovimento.getItensMovimentoByMovimento(movtoContext.cd_movimento, movtoContext.cd_pessoa_empresa).ToList();
                    foreach (ItemMovimento item in itemMovimentoContext)
                        if (item != null)
                            deletarKardexOfMovimento(item, movtoContext);
                }
                transaction.Complete();
            }
        }

        public Movimento postVerificarCancelamentoNF(int cd_escola, int cd_movimento, bool id_empresa_propria) {
            Movimento movtoContext = DataAccessMovimento.getMovimento(cd_movimento, cd_escola);

            if(!movtoContext.id_nf)
                throw new FiscalBusinessException(string.Format(Utils.Messages.Messages.msgErroMovimentoNotNF), null,
                                        FiscalBusinessException.TipoErro.ERRO_CANCELAR_NF, false);

            if(movtoContext.id_status_nf != (int) Movimento.StatusNFEnum.FECHADO)
                throw new FiscalBusinessException(string.Format(Utils.Messages.Messages.msgErroCancelarNFAberta, movtoContext.status_nf), null,
                                                            FiscalBusinessException.TipoErro.ERRO_CANCELAR_NF, false);

            if (DataAccessMovimento.existeTituloBaixadoByMovimento(cd_movimento))
                throw new FiscalBusinessException(Utils.Messages.Messages.msgErroCancelarNFBaixado, null, FiscalBusinessException.TipoErro.ERRO_CANCELAR_NF, false);
            if (id_empresa_propria && ((movtoContext.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.SERVICO &&
                DataAccessMovimento.notaFiscalComNFS(movtoContext.id_tipo_movimento, movtoContext.cd_movimento, movtoContext.cd_pessoa_empresa)) ||
                (movtoContext.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.SAIDA && 
                 DataAccessMovimento.notaFiscalComProtocolo(movtoContext.id_tipo_movimento, movtoContext.cd_movimento, movtoContext.cd_pessoa_empresa))
                ))
                throw new FiscalBusinessException(Utils.Messages.Messages.msgErroCancNFNaoAuto, null, FiscalBusinessException.TipoErro.ERRO_CANCELAR_NF, false);
            if (id_empresa_propria && movtoContext.id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.DEVOLUCAO &&
                DataAccessMovimento.notaFiscalDevolucaoComProtocolo(movtoContext.id_tipo_movimento, movtoContext.cd_movimento, movtoContext.cd_pessoa_empresa))
                throw new FiscalBusinessException(Utils.Messages.Messages.msgErroCancNFNaoAuto, null, FiscalBusinessException.TipoErro.ERRO_CANCELAR_NF, false);
            return movtoContext;
        }

        public bool verificarTipoNotaFiscalPermiteMovimentoFinanceiro(int cd_tipo_nota_fiscal) {
            return DataAccessTipoNotaFiscal.verificarTipoNotaFiscalPermiteMovimentoFinanceiro(cd_tipo_nota_fiscal);
        }

        public bool spEnviarMasterSaf(int? cd_movimento)
        {
            return DataAccessMovimento.spEnviarMasterSaf(cd_movimento);
        }

        public bool postReenviarNFMasterSaf(int cd_movimento, int cdEscola, bool empresaPropria)
        {
            bool retorno = true;
            if (empresaPropria)
            {
                bool existeMovimento = DataAccessMovimento.existeMovimentoEscola(cd_movimento, cdEscola);
                if (existeMovimento)
                    retorno = DataAccessMovimento.spEnviarMasterSaf(cd_movimento);
                else
                    throw new FiscalBusinessException(string.Format(Utils.Messages.Messages.msgRegNotEnc), null, FiscalBusinessException.TipoErro.ERRO_PROCESSAR_NF, false);
            }
            return retorno;
        }

        public IEnumerable<TipoNotaFiscal> getTipoNotaFiscalSearch(SearchParameters parametros, string desc, string natOp, bool inicio, bool? status, int movimento, bool? devolucao, int cdEscola,
            byte id_regime_trib, bool? id_servico)
        {
            IEnumerable<TipoNotaFiscal> retorno = new List<TipoNotaFiscal>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "dc_tipo_nota_fiscal";
                parametros.sort = parametros.sort.Replace("nmSitTrib", "cd_situacao_tributaria");
                parametros.sort = parametros.sort.Replace("mtvoEstoque", "id_movimenta_estoque");
                parametros.sort = parametros.sort.Replace("mtvoFinanc", "id_movimenta_financeiro");
                parametros.sort = parametros.sort.Replace("devolucao", "id_devolucao");
                parametros.sort = parametros.sort.Replace("movimento", "id_natureza_movimento");
                parametros.sort = parametros.sort.Replace("ativo", "id_tipo_ativo");

                retorno = DataAccessTipoNotaFiscal.getTipoNotaFiscalSearch(parametros, desc, natOp, inicio, status, movimento, devolucao, cdEscola, id_regime_trib, id_servico);
                transaction.Complete();
            }
            return retorno;
        }

        public bool deleteAllTpNF(List<TipoNotaFiscal> tpsNF)
        {
            bool retorno = true;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {

                foreach (TipoNotaFiscal l in tpsNF)
                {
                    bool existe = DataAccessTipoNotaFiscal.getTpNFUtilizado(l.cd_tipo_nota_fiscal);
                    if (existe)
                        throw new FinanceiroBusinessException(Messages.msgErroExcluirTpNFUtilizado, null, FinanceiroBusinessException.TipoErro.ERRO_TIPO_NF_UTILIZADO, false);
                    TipoNotaFiscal tpDel = DataAccessTipoNotaFiscal.findById(l.cd_tipo_nota_fiscal, false);
                    retorno = DataAccessTipoNotaFiscal.delete(tpDel, false);
                }
                transaction.Complete();
            }
            return retorno;
        }

        public TipoNotaFiscal postTpNF(TipoNotaFiscal tipo)
        {
            TipoNotaFiscal retornar = new TipoNotaFiscal();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {

                tipo = DataAccessTipoNotaFiscal.add(tipo, false);
                retornar = DataAccessTipoNotaFiscal.findById(tipo.cd_tipo_nota_fiscal, false);
                transaction.Complete();
            }

            return retornar;
        }

        public TipoNotaFiscal putTpNF(TipoNotaFiscal tipo)
        {
            TipoNotaFiscal retornar = new TipoNotaFiscal();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                bool existe = DataAccessTipoNotaFiscal.getTpNFUtilizado(tipo.cd_tipo_nota_fiscal);
                if (existe)
                    throw new FinanceiroBusinessException(Messages.msgErroAlterarTpNFUtilizado, null, FinanceiroBusinessException.TipoErro.ERRO_TIPO_NF_UTILIZADO, false);

                TipoNotaFiscal tipoContext = DataAccessTipoNotaFiscal.findById(tipo.cd_tipo_nota_fiscal, false);
                //Quando existir NF usando esse tipo ou nos parametros, não permitir alterar.

                tipoContext.copy(tipo); ;

                DataAccessTipoNotaFiscal.saveChanges(false);
                retornar = DataAccessTipoNotaFiscal.findById(tipoContext.cd_tipo_nota_fiscal, false);
                transaction.Complete();
            }
            return retornar;
        }
        public TipoNotaFiscal getTipoNFById(int cdTpNF)
        {
            return DataAccessTipoNotaFiscal.findById(cdTpNF, false);
        }
        public byte getTipoMvtoTpNF(int cd_tipo_nota_fiscal)
        {
            return DataAccessTipoNotaFiscal.getTipoMvtoTpNF(cd_tipo_nota_fiscal);
        }
        public bool existeMovimentoTpNF(int cd_tipo_nota_fiscal)
        {
            return DataAccessMovimento.existeMovimentoTpNF(cd_tipo_nota_fiscal);
        }

        public List<int> VerificaNFESemDataAutorizacao(int id_tipo_movimento, int cd_empresa, bool nota_fiscal)
        {
            List<int> retornar ;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retornar = DataAccessMovimento.VerificaNFESemDataAutorizacao(id_tipo_movimento, cd_empresa, nota_fiscal);
                transaction.Complete();
            }

            return retornar;
        }

        public List<ContratoComboUI> getContratosSemTurmaByAlunoMovimentoSearch(int cd_aluno,
            bool semTurma, int situacaoTurma, int nmContrato, int tipo, int cdEscola, byte tipoC, bool? status)
        {
            return DataAccessMovimento.getContratosSemTurmaByAlunoMovimentoSearch(cd_aluno, semTurma, situacaoTurma, nmContrato, tipo, cdEscola, tipoC, status).ToList();
        }
        #endregion

        #region CFOP
        public CFOP getCFOPByTpNF(int cd_tipo_nota) {
            return DataAccessCFOP.getCFOPByTpNF(cd_tipo_nota);
        }

        public IEnumerable<CFOP> searchCFOP(SearchParameters parametros, string descricao,bool inicio, int nm_CFOP, byte id_natureza_CFOP)
        {
            IEnumerable<CFOP> retorno = new List<CFOP>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "dc_cfop";
                retorno = DataAccessCFOP.searchCFOP(parametros, descricao, inicio, nm_CFOP, id_natureza_CFOP);
                transaction.Complete();
            }
            return retorno;
        }
        #endregion 

        #region Dados NF

        public DadosNF putDadosNF(DadosNF dado)
        {
            DadosNF retornar = new DadosNF();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {

                DadosNF dadoContext = BusinessFinan.getDadosNFById(dado.cd_dados_nf);
                bool existe = DataAccessMovimento.existeDadosNFNota(dado.cd_cidade);
                if (existe)
                    throw new FinanceiroBusinessException(Messages.msgErroAlterarDadosICMSNF, null, FinanceiroBusinessException.TipoErro.ERRO_DADOS_UF_UTILIZADO, false);

                if (dado.cd_cidade != dadoContext.cd_cidade)
                {
                    bool existeCidade = BusinessFinan.getDadosCidade(dado.cd_cidade);
                    if (existeCidade)
                        throw new FinanceiroBusinessException(Messages.msgErroCidadeDadosNF, null, FinanceiroBusinessException.TipoErro.ERRO_EXISTE_CIDADE_DADOS_NF, false);
                }
                //Quando existir NF usando esse tipo ou nos parametros, não permitir alterar.
                if (dadoContext != null)
                {
                    dadoContext.copy(dado);

                    DataAccessMovimento.saveChanges(false);
                    retornar = BusinessFinan.getDadosNFById(dado.cd_dados_nf);
                }
                transaction.Complete();
            }
            return retornar;
        }

        public bool deleteAllDadosNF(List<DadosNF> dados)
        {
            bool retorno = true;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                foreach (DadosNF l in dados)
                {

                    DadosNF alDel = BusinessFinan.getDadosNFById(l.cd_dados_nf);
                    bool existe = DataAccessMovimento.existeDadosNFNota(alDel.cd_cidade);
                    if (existe)
                        throw new FinanceiroBusinessException(Messages.msgErroExcluirDadosICMSNF, null, FinanceiroBusinessException.TipoErro.ERRO_DADOS_UF_UTILIZADO, false);

                    retorno = BusinessFinan.postDeleteDadosNF(alDel);
                }
                transaction.Complete();
            }
            return retorno;
        }

        #endregion

        #region Aliquota NF

        public bool deleteAllAliquotaUF(List<AliquotaUF> aliquotas)
        {
            sincronizarContextos(DataAccessMovimento.DB());
            bool retorno = true;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED, DataAccessMovimento.DB()))
            {
                foreach (AliquotaUF l in aliquotas)
                {
                    AliquotaUF alDel = BusinessFinan.getAliquotaUFById(l.cd_aliquota_uf);
                    bool existe = DataAccessMovimento.existeICMSEstadoNota(alDel.cd_localidade_estado_origem, alDel.cd_localidade_estado_destino);
                    if (existe)
                        throw new FinanceiroBusinessException(Messages.msgErroExcluirDadosICMSNF, null, FinanceiroBusinessException.TipoErro.ERRO_ICMS_ESTADO_UTILIZADO, false);

                    retorno = BusinessFinan.posDeletetAliquotaUF(alDel);
                }
                transaction.Complete();
            }
            return retorno;
        }

        public AliquotaUF putAliquotaUF(AliquotaUF aliquota)
        {
            sincronizarContextos(DataAccessMovimento.DB());
            AliquotaUF retornar = new AliquotaUF();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED, DataAccessMovimento.DB()))
            {
                AliquotaUF aliquotaContext = BusinessFinan.getAliquotaUFById(aliquota.cd_aliquota_uf);
                bool existe = DataAccessMovimento.existeICMSEstadoNota(aliquotaContext.cd_localidade_estado_origem, aliquotaContext.cd_localidade_estado_destino);
                if (existe)
                    throw new FinanceiroBusinessException(Messages.msgErroAlterarDadosICMSNF, null, FinanceiroBusinessException.TipoErro.ERRO_ICMS_ESTADO_UTILIZADO, false);

                //Quando existir NF usando esse tipo ou nos parametros, não permitir alterar.
                if (aliquotaContext != null)
                {
                    aliquotaContext.copy(aliquota);

                    DataAccessMovimento.saveChanges(false);
                    retornar = BusinessFinan.getAliquotaUFById(aliquota.cd_aliquota_uf);
                }
                transaction.Complete();
            }
            return retornar;
        }

        #endregion 

    }
}

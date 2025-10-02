using Componentes.GenericBusiness.Comum;
using Componentes.GenericController;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Utils.Messages;
using FundacaoFisk.SGF.Web.Services.Empresa.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Financeiro.Model;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;


namespace FundacaoFisk.SGF.Web.Controllers
{
    public class FinanceiroController : ComponentesMVCController
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(PermissaoController));
        public FinanceiroController()
        {
        }
        //
        // GET: /Financeiro/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BaixaFinanceira()
        {
            return View();
        }

        public ActionResult BaixaAutomaticaCartao()
        {
            return View();
        }

        public ActionResult BaixaAutomaticaCheque()
        {
            return View();
        }

        public ActionResult ReajusteAnual()
        {
            return View();
        }

        // GET: /PoliticaDesconto/

        public ActionResult PoliticaDesconto()
        {
            return View();
        }
        // GET: /TabelaPreco/

        public ActionResult Item()
        {
            return View();
        }

        public ActionResult KitVendas()
        {
            return View();
        }

        public ActionResult TabelaPreco()
        {
            return View();
        }

        public ActionResult CnabBoleto()
        {
            return View();
        }

        public ActionResult ContaCorrente()
        {
            return View();
        }

        public ActionResult LocalMovto()
        {
            return View();
        }

        public ActionResult PoliticaComercial()
        {
            return View();
        }

        public ActionResult FechamentoCaixa()
        {
            return View();
        }

        public ActionResult FechamentoEstoque()
        {
            return View();
        }

        public ActionResult RetornoCnab()
        {
            return View();
        }

        public ActionResult PlanoConta()
        {
            return View();
        }

        #region Item Serviço
        //Retorna um boleano, se o usuario é master
        private bool retornaUserMaster()
        {
            IEmpresaBusiness empresaBiz = (IEmpresaBusiness)base.instanciarBusiness<IEmpresaBusiness>();
            string login = Session["loginUsuario"].ToString();
            var isMaster = empresaBiz.VerificarMasterGeral(login);
            return isMaster;
        }

        [MvcComponentesAuthorize(Roles = "item")]
        public ActionResult postItensFechamento(ICollection<int> itensFech)
        {
            ReturnResult retorno = new ReturnResult();
            Session["itensFech"] = itensFech;
            try
            {
                return new RenderJsonActionResult { Result = true };
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegNotEnc, retorno, logger, ex);
            }
        }

        [MvcComponentesAuthorize(Roles = "item")]
        [MvcComponentesAuthorize(Roles = "gest")]
        public ActionResult getItensSearchEstoque(string desc, bool inicio, int status, int tipoItemInt, int grupoItem, int ano, int mes)
        {
            List<int> itensFech = null;
            ReturnResult retorno = new ReturnResult();
            int cdEscola = int.Parse(Session["CodEscolaSelecionada"] + "");
            try
            {
                IFinanceiroBusiness BusinessFinanceiro = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                itensFech = ((List<int>)Session["itensFech"]);
                List<int> cdItensEstoque = new List<int>();
                if (itensFech != null)
                    cdItensEstoque = itensFech.Select(s => s).ToList();
                bool isMasterGeral = retornaUserMaster();
                if (desc == null)
                    desc = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers, this.Request.Params);

                IEnumerable<ItemUI> itens = BusinessFinanceiro.getItemSearchEstoque(parametros, desc, inicio, getStatus(status), tipoItemInt, grupoItem, cdEscola, false, cdItensEstoque, isMasterGeral, ano, mes);

                if (itens == null)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                var retRender = new RenderJsonActionResult { Result = itens, parameters = parametros };
                return retRender;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
            }
            finally
            {
                Session["HorariosTurma"] = null;
                Session.Remove("HorariosTurma");
            }

        }
        #endregion

        #region Tabela de Preço

        [HttpPost]
        [MvcComponentesAuthorize(Roles = "tprec.i")]
        public RenderJsonActionResult PostTabelaPreco(TabelaPreco tabela)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness BusinessFinanceiro = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { BusinessFinanceiro });

                tabela.cd_pessoa_escola = int.Parse(Session["CodEscolaSelecionada"] + "");
                var tabelapreco = BusinessFinanceiro.postTabelaPreco(tabela);
                retorno.retorno = tabelapreco;

                retorno.retorno = tabelapreco;
                if (tabelapreco.cd_tabela_preco <= 0)
                    retorno.AddMensagem(Messages.msgNotIncludReg, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return new RenderJsonActionResult { Result = retorno };
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
        }

        [HttpPost]
        [MvcComponentesAuthorize(Roles = "tprec.a")]
        public RenderJsonActionResult PostAlterarTabelaPreco(TabelaPreco tabela)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness BusinessFinanceiro = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { BusinessFinanceiro });
                tabela.cd_pessoa_escola = int.Parse(Session["CodEscolaSelecionada"] + "");
                var tabelapreco = BusinessFinanceiro.postAlterarTabelaPreco(tabela);
                retorno.retorno = tabelapreco;

                retorno.retorno = tabelapreco;
                if (tabelapreco.cd_tabela_preco <= 0)
                    retorno.AddMensagem(Messages.msgNotUpReg, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return new RenderJsonActionResult { Result = retorno };
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
        }
        #endregion

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IBusiness;
using FundacaoFisk.SGF.Utils;
using log4net;
using FundacaoFisk.SGF.Web.Services.Financeiro.Model;
using Componentes.GenericController;
using FundacaoFisk.SGF.Web.Services.InstituicaoEnsino.Comum.IBusiness;
using FundacaoFisk.SGF.Utils.Messages;
using Componentes.GenericModel;
using Componentes.GenericBusiness.Comum;
using FundacaoFisk.SGF.Web.Services.Empresa.Comum.IBusiness;
using System.Xml;
using FundacaoFisk.SGF.Web.Services.Financeiro.Business;
using System.IO;


namespace FundacaoFisk.SGF.Web.Controllers
{
    public class FiscalController : ComponentesMVCController
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(PermissaoController));
        public FiscalController()
        {
        }

        public ActionResult Index()
        {
            return View();
        }

        #region Movimento
        [HttpGet]
        [MvcComponentesAuthorize(Roles = "mvtc,mvts")]
        public ActionResult postGerarXML(int cd_movimento, byte id_tipo_movimento) {
            ReturnResult retorno = new ReturnResult();
            try {
                IFiscalBusiness BusinessFiscal = (IFiscalBusiness)base.instanciarBusiness<IFiscalBusiness>();
                int cd_escola = (int)Session["CodEscolaSelecionada"];
                string enderecoRelatorioWeb = Session["enderecoRelatorioWeb"] + "";
                configuraBusiness(new List<IGenericBusiness>() { BusinessFiscal });

                var parms = "cd_escola=" + cd_escola + "&cd_movimento=" + cd_movimento + "&id_tipo_movimento=" + id_tipo_movimento +
                    "&" + Componentes.Utils.ReportParameter.PARAMETRO_DATA_HORA_ATUAL + "=" + DateTime.Now;
                string parametros = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(Componentes.Utils.MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parms, System.Text.Encoding.UTF8), Componentes.Utils.MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);
                parametros += "&enderecoWeb=" + Session["enderecoWeb"];

                retorno.retorno = enderecoRelatorioWeb + "RelatorioApresentacao/emitirNF?" + parametros;

                return new RenderJsonActionResult { Result = retorno };
            }
            catch(Exception ex) {
                return gerarLogException(Messages.msgErrorGerarXMLNF, retorno, logger, ex);
            }
        }
                
        //[HttpComponentesAuthorize(Roles = "item")]
        [MvcComponentesAuthorize(Roles = "gest")]
        [MvcComponentesAuthorize(Roles = "mvtc,mvtd,mvtp,mvts, mvtdv")]
        public ActionResult getItemMovimentoSearch(string desc, bool inicio, int status, int? tipoItemInt, int? grupoItem, int id_tipo_movto, bool comEstoque, int? id_natureza_TPNF, bool kit)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                bool contaSegura = false;
                IFiscalBusiness BusinessFiscal = (IFiscalBusiness)base.instanciarBusiness<IFiscalBusiness>();
                if (desc == null)
                    desc = String.Empty;
                // Verifica permissão "Conta Segura" do usuário.
                if (Session["Permissoes"] != null)
                    contaSegura = Session["Permissoes"].ToString().Contains("ctsg");
                int cd_escola = int.Parse(Session["CodEscolaSelecionada"] + "");
                var isMaster = bool.Parse(Session["IdMaster"] + "");
                var parametros = new SearchParameters(this.Request.Headers, this.Request.Params);
                var ret = BusinessFiscal.getItemMovimentoSearch(parametros, desc, inicio, getStatus(status), tipoItemInt, grupoItem, cd_escola, id_tipo_movto,
                                                              Movimento.TipoVinculoMovimento.PESQUISA_ITEM_MOVIMENTO, comEstoque, id_natureza_TPNF, kit, contaSegura, null);
                return new RenderJsonActionResult { Result = ret, parameters = parametros };
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
            }
        }

        [MvcComponentesAuthorize(Roles = "gest")]
        [MvcComponentesAuthorize(Roles = "mvtc,mvtd,mvtp,mvts, mvtdv")]
        public ActionResult getItemMovimentoSearchPerdaMaterial(string desc, bool inicio, int status, int? tipoItemInt, int? grupoItem, int id_tipo_movto, bool comEstoque, int? id_natureza_TPNF, bool kit, int origem)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                bool contaSegura = false;
                IFiscalBusiness BusinessFiscal = (IFiscalBusiness)base.instanciarBusiness<IFiscalBusiness>();
                if (desc == null)
                    desc = String.Empty;
                // Verifica permissão "Conta Segura" do usuário.
                if (Session["Permissoes"] != null)
                    contaSegura = Session["Permissoes"].ToString().Contains("ctsg");
                int cd_escola = int.Parse(Session["CodEscolaSelecionada"] + "");
                var isMaster = bool.Parse(Session["IdMaster"] + "");
                var parametros = new SearchParameters(this.Request.Headers, this.Request.Params);
                var ret = BusinessFiscal.getItemMovimentoSearchPerdaMaterial(parametros, desc, inicio, getStatus(status), tipoItemInt, grupoItem, cd_escola, id_tipo_movto,
                    Movimento.TipoVinculoMovimento.PESQUISA_ITEM_MOVIMENTO, comEstoque, id_natureza_TPNF, kit, contaSegura, null, origem);
                return new RenderJsonActionResult { Result = ret, parameters = parametros };
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
            }
        }

        [MvcComponentesAuthorize(Roles = "rpttm")]
        public ActionResult getItemMovimentoSearchRelTurmaMatriculaMaterial(string desc, bool inicio, int status, int? tipoItemInt, int? grupoItem, int id_tipo_movto, bool comEstoque, int? id_natureza_TPNF)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                bool contaSegura = false;
                IFiscalBusiness BusinessFiscal = (IFiscalBusiness)base.instanciarBusiness<IFiscalBusiness>();
                if (desc == null)
                    desc = String.Empty;
                // Verifica permissão "Conta Segura" do usuário.
                if (Session["Permissoes"] != null)
                    contaSegura = Session["Permissoes"].ToString().Contains("ctsg");
                int cd_escola = int.Parse(Session["CodEscolaSelecionada"] + "");
                var isMaster = bool.Parse(Session["IdMaster"] + "");
                var parametros = new SearchParameters(this.Request.Headers, this.Request.Params);
                var ret = BusinessFiscal.getItemMovimentoSearch(parametros, desc, inicio, getStatus(status), tipoItemInt, grupoItem, cd_escola, id_tipo_movto,
                                                              Movimento.TipoVinculoMovimento.HAS_PESQ_NOTA_MATERIAL, comEstoque, id_natureza_TPNF, false, contaSegura, null);
                return new RenderJsonActionResult { Result = ret, parameters = parametros };
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
            }
        }

        [MvcComponentesAuthorize(Roles = "rpttm")]
        public ActionResult getItemMovimentoSearchVendaMaterial(string desc, bool inicio, int status, int? tipoItemInt, int? grupoItem, int id_tipo_movto, bool comEstoque, int? id_natureza_TPNF, int? cd_aluno, string dt_inicial, string dt_final)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                bool contaSegura = false;
                IFiscalBusiness BusinessFiscal = (IFiscalBusiness)base.instanciarBusiness<IFiscalBusiness>();
                if (desc == null)
                    desc = String.Empty;
                // Verifica permissão "Conta Segura" do usuário.
                if (Session["Permissoes"] != null)
                    contaSegura = Session["Permissoes"].ToString().Contains("ctsg");
                int cd_escola = int.Parse(Session["CodEscolaSelecionada"] + "");
                var isMaster = bool.Parse(Session["IdMaster"] + "");
                var dtInicial = new DateTime();
                var dtFinal = new DateTime();

                if (!string.IsNullOrEmpty(dt_inicial))
                    dtInicial = DateTime.Parse(dt_inicial);
                if (!string.IsNullOrEmpty(dt_final))
                    dtFinal = DateTime.Parse(dt_final);

                var parametros = new SearchParameters(this.Request.Headers, this.Request.Params);
                var ret = BusinessFiscal.getItemMovimentoSearch(parametros, desc, inicio, getStatus(status), tipoItemInt, grupoItem, cd_escola, id_tipo_movto,
                                                              Movimento.TipoVinculoMovimento.PESQUISA_VENDA_MATERIAL, comEstoque, id_natureza_TPNF, false, contaSegura, null, cd_aluno, dtInicial, dtFinal);
                return new RenderJsonActionResult { Result = ret, parameters = parametros };
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
            }
        }

        //[HttpComponentesAuthorize(Roles = "item")]
        [MvcComponentesAuthorize(Roles = "gest")]
        [MvcComponentesAuthorize(Roles = "mvtc,mvtd,mvtp,mvts, mvtdv")]
        public ActionResult getItemCadastroMovimentoSearch(string desc, bool inicio, int status, int? tipoItemInt, int? grupoItem, int id_tipo_movto, bool comEstoque, int? id_natureza_TPNF, int? cd_movimento, bool? vinculado_curso, int? cd_curso_material_didatico)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFiscalBusiness BusinessFiscal = (IFiscalBusiness)base.instanciarBusiness<IFiscalBusiness>();
                if (desc == null)
                    desc = String.Empty;
                // Verifica permissão "Conta Segura" do usuário.
                bool contaSegura = Session["Permissoes"].ToString().Contains("ctsg");

                int cd_escola = int.Parse(Session["CodEscolaSelecionada"] + "");
                var isMaster = bool.Parse(Session["IdMaster"] + "");
                var parametros = new SearchParameters(this.Request.Headers, this.Request.Params);
                var ret = BusinessFiscal.getItemMovimentoSearch(parametros, desc, inicio, getStatus(status), tipoItemInt, grupoItem, cd_escola, id_tipo_movto,
                                                              Movimento.TipoVinculoMovimento.PESQUISA_ITEM_CADASTRO_MOVIMENTO, comEstoque, id_natureza_TPNF, false, contaSegura, cd_movimento, null, null, null, vinculado_curso, cd_curso_material_didatico);
                return new RenderJsonActionResult { Result = ret, parameters = parametros };
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
            }
        }

        #endregion
    }
}

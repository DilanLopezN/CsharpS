using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Componentes.GenericController;
using Componentes.Utils;
using log4net;
namespace FundacaoFisk.SGF.Web.Controllers
{
    [AllowAnonymous]
    public class ErroController : ComponentesMVCController
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(ErroController));

        [AllowAnonymous]
        public ActionResult Erro()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public RenderJsonActionResult RecuperaErro()
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                retorno.retorno = Session["Erro"] + "#@@#" + Session["StackTrace"];
                return new RenderJsonActionResult { Result = retorno };
            }
            catch (Exception exe)
            {
                logger.Error(exe);
                return new RenderJsonActionResult { Result = new { erro = retorno } };
            }
            finally
            {
                Session["Erro"] = null;
                Session["StackTrace"] = null;
                Session.Remove("Erro");
                Session.Remove("StackTrace");
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index(string msgErro, string stackTrace) {
            ReturnResult retorno = new ReturnResult();

            try {
                if(HttpUtility.UrlDecode(msgErro) != null && HttpUtility.UrlDecode(Request.QueryString["msgErro"]) != null) {
                    Session["Erro"] = msgErro != null ? HttpUtility.UrlDecode(msgErro) : HttpUtility.UrlDecode(Request.QueryString["msgErro"]);
                    Session["StackTrace"] = stackTrace != null ? HttpUtility.UrlDecode(stackTrace) : HttpUtility.UrlDecode(Request.QueryString["stackTrace"]);
                }
                return View();
            }
            catch(Exception exe) {
                logger.Error(exe);
                return new RenderJsonActionResult { Result = new { erro = retorno } };
            }
            //finally
            //{
            //    //Session["Erro"] = null;
            //    //Session["StackTrace"] = null;
            //}
        }

        public class ErroUI{
            public string msgErro = "";
            public string stackTrace = "";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Componentes.GenericController;

namespace FundacaoFisk.SGF.Web.Controllers
{
    public class ConfiguracaoController : ComponentesMVCController
    {
        //
        // GET: /Configuracao/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ConfiguracaoSistema()
        {
            return View();
        }
    }
}

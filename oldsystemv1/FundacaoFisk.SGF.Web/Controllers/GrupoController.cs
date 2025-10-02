using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Componentes.GenericController;
using Componentes.Utils;

namespace FundacaoFisk.SGF.Web.Controllers
{
    public class GrupoController : ComponentesMVCController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
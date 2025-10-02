﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Componentes.GenericController;
using Componentes.GenericModel;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Utils.Messages;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IBusiness;
using log4net;

namespace FundacaoFisk.SGF.Web.Controllers
{
    public class CursoController : ComponentesMVCController
    {
        public CursoController() {
        }

        public ActionResult Index()
        {
            return View();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Componentes.Utils;
using Newtonsoft.Json;
using FundacaoFisk.SGF.Utils.Messages;
using log4net;
using Componentes.GenericModel;
using Componentes.GenericController;
using System.Net.Http.Headers;
using FundacaoFisk.SGF.Web;
using FundacaoFisk.SGF.GenericModel;
using System.Web;
using Componentes.ApresentadorRelatorio;
using System.Threading.Tasks;
using System.IO;
using Componentes.GenericBusiness.Comum;
using FundacaoFisk.SGF.Web.Services.CNAB.Comum.IBusiness;
using System.Configuration;
using System.Globalization;
using FundacaoFisk.SGF.Web.Services.CNAB.Business;


namespace FundacaoFisk.SGF.Services.CNAB.Controllers
{
    public class BoletoController : ComponentesApiController
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(BoletoController));

        public BoletoController()
        {
        }
    }
}
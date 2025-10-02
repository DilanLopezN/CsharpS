using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Pessoa.Comum;
using Componentes.ApresentadorRelatorio;
using log4net;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using FundacaoFisk.SGF.Web.Services.Pessoa.Comum.Business;
using System.IO;
using FundacaoFisk.SGF.Utils;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using FundacaoFisk.SGF.Utils.Messages;
using Componentes.GenericController;
using System.Configuration;
using FundacaoFisk.SGF.Web.Services.Secretaria.Comum;
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;
using Componentes.GenericModel;
using Componentes.GenericBusiness.Comum;
using FundacaoFisk.SGF.Web.Services.Pessoa.Business;
using FundacaoFisk.SGF.Web.Services.Secretaria.Business;

namespace FundacaoFisk.SGF.Web.Controllers
{
    public class PessoaController : ComponentesMVCController
    {
        public PessoaController()
        {
        }
        
        // GET: /Pessoa/
       
        public ActionResult Localidades()
        {
            return View();
        }
        public ActionResult AuxiliaresPessoa()
        {
            return View();
        }

        public ActionResult Funcionario()
        {
            return View();
        }

    }
}

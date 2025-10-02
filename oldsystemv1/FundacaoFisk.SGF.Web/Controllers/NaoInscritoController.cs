using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Componentes.GenericController;
using FundacaoFisk.SGF.Utils.Messages;
using FundacaoFisk.SGF.Web.Services.EmailMarketing.Comum.IBusiness;
using log4net;

namespace FundacaoFisk.SGF.Web.Controllers
{
    [AllowAnonymous]
    public class NaoInscritoController : ComponentesMVCController
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(NaoInscritoController));

        public NaoInscritoController()
        {
        }

        [HttpGet]
        [AllowAnonymous]
        public RenderJsonActionResult excluirContatoListagemEnderecos(string parametros)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                IEmailMarketingBusiness emailMarketingBusiness = (IEmailMarketingBusiness)base.instanciarBusiness<IEmailMarketingBusiness>();
                int cd_empresa = 0;
                int cd_cadastro = 0;
                int id_cadastro = 0;
                string parms = HttpUtility.UrlDecode(Request.QueryString[Componentes.Utils.ReportParameter.PARAMETROS], System.Text.Encoding.UTF8).Replace(" ", "+");
                parms = Componentes.Utils.MD5CryptoHelper.descriptografaSenha(parms, Componentes.Utils.MD5CryptoHelper.KEY);
                parms = HttpUtility.UrlDecode(parms, System.Text.Encoding.UTF8);
                string[] parametrosGet = parms.Split('&');
                for (int i = 0; i < parametrosGet.Length; i++)
                {
                    string[] parametrosHash = parametrosGet[i].Split('=');
                    if (parametrosHash[0].Equals("cd_empresa"))
                        cd_empresa = int.Parse(parametrosHash[1]);
                    if (parametrosHash[0].Equals("cd_cadastro"))
                        cd_cadastro = int.Parse(parametrosHash[1]);
                    if (parametrosHash[0].Equals("id_cadastro"))
                        id_cadastro = int.Parse(parametrosHash[1]);
                }
                emailMarketingBusiness.retirarEmailListaEndereco(cd_empresa, cd_cadastro, id_cadastro);
                retorno.AddMensagem(Messages.msgRegExcListaEndereco, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                retorno.retorno = Messages.msgRegExcListaEndereco;
                //ViewBag.Mensagem = Messages.msgRegExcListaEndereco;
                return new RenderJsonActionResult { Result = retorno };
            }
            catch (Exception exe)
            {
                logger.Error(exe);
                retorno.AddMensagem(Messages.msgNotDeletedReg, exe.Message + exe.StackTrace + exe.InnerException, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                return new RenderJsonActionResult { Result = new { erro = retorno } };
            }
        }
    }
}

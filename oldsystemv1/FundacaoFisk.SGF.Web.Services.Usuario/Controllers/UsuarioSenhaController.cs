using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using FundacaoFisk.SGF.Web.Services.Usuario.Model;
using FundacaoFisk.SGF.Web.Services.Usuario.Comum;
using Componentes.GenericController;
using Componentes.Utils.Messages;
using log4net;
using Newtonsoft.Json;
using FundacaoFisk.SGF.Web.Services.Usuario.Business;
using Componentes.GenericBusiness.Comum;


namespace FundacaoFisk.SGF.Web.Services.Usuario.Controllers
{
    public class UsuarioSenhaController : ComponentesApiController
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(UsuarioSenhaController));

        public UsuarioSenhaController()
        {
        }

        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        //// POST api/<controller>
        //public void Post([FromBody]string value)
        //{
        //}

        // PUT api/<controller>
         [HttpComponentesAuthorize(Roles = "altse")]
        public HttpResponseMessage PostAlteraUsuarioSenha(AlterarSenha senhas)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IUsuarioBusiness Business = (IUsuarioBusiness)base.instanciarBusiness<IUsuarioBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });
                AlterarSenhaStatus result = Business.PutUsuarioSenha(User.Identity.Name, senhas, false);
                HttpStatusCode ret = HttpStatusCode.OK; //string errorMsg = Messages.msgUpdateSucess;

                //retorno.AddMensagem(errorMsg, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                //return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));

                retorno.retorno = ret;

                if (!(ret == HttpStatusCode.OK))
                {

                    retorno.AddMensagem(Messages.msgNotUpDateReg, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                }
                else
                {
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                }
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));

            }
            catch (UsuarioBusinessException exe)
            {
                if (exe.tipoErro == UsuarioBusinessException.TipoErro.ERRO_SENHA_INVALIDA)
                {
                    retorno.AddMensagem(exe.Message, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                    return Request.CreateResponse(HttpStatusCode.BadRequest, JsonConvert.SerializeObject(retorno));
                }
                else
                {
                    logger.Error(exe);
                    retorno.AddMensagem(Messages.msgNotUpDateReg, exe.Message + exe.StackTrace + exe.InnerException, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                    return Request.CreateResponse(HttpStatusCode.BadRequest, JsonConvert.SerializeObject(retorno));
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                retorno.AddMensagem(Messages.msgNotUpDateReg, ex.Message + ex.StackTrace + ex.InnerException, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                return Request.CreateResponse(HttpStatusCode.BadRequest, JsonConvert.SerializeObject(retorno));
            }

        }

        //// DELETE api/<controller>/5
        //public void Delete(int id)
        //{
        //}
    }
}
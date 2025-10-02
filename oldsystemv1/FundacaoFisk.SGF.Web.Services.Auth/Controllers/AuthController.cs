using System;
using System.IO;
using System.Web;
using System.Net;
using System.Web.Http;
using System.Net.Http;
using Newtonsoft.Json;
using Componentes.GenericController;
using FundacaoFisk.SGF.Web.Services.Auth.Model;
using FundacaoFisk.SGF.Web.Services.Auth.Comum;
using FundacaoFisk.SGF.Web.Services.Usuario.Model;
using FundacaoFisk.SGF.Web.Services.Usuario.Comum;
using Componentes.GenericBusiness;
using log4net;
using Componentes.Utils.Messages;
using System.Configuration;


namespace FundacaoFisk.SGF.Web.Services.Auth.Controller {
    public class AuthController : ComponentesApiController
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(AuthController));

        public AuthController()
        {
        }

        [HttpPost, AllowAnonymous]
        public HttpResponseMessage Authenticate(UserCredential credentials)
        {
            IAuthBusiness Business = (IAuthBusiness)base.instanciarBusiness<IAuthBusiness>();
            AuthenticationResult result = Business.Authenticate(credentials);
            switch (result.Status) {
                case AuthenticationStatus.UnknownUser:
                    return Request.CreateResponse(HttpStatusCode.NotFound, result.ErrorMessage, new HttpConfiguration());
                case AuthenticationStatus.IncorrectPassword:
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, result.ErrorMessage, new HttpConfiguration());
            }
            return Request.CreateResponse(HttpStatusCode.OK, result.Token, new HttpConfiguration());
        }

        [HttpPost, AllowAnonymous]
        public HttpResponseMessage RefreshAccessToken([FromBody] string refreshToken)
        {
            IAuthBusiness Business = (IAuthBusiness)base.instanciarBusiness<IAuthBusiness>();
            AuthenticationResult result = Business.RefreshAccessToken(refreshToken);
            switch (result.Status) {
                case AuthenticationStatus.TokenNotExpired:
                    return Request.CreateResponse(HttpStatusCode.NotModified);
                    case AuthenticationStatus.UnknownUser:
                    return Request.CreateResponse(HttpStatusCode.NotFound, result.ErrorMessage, new HttpConfiguration());
                case AuthenticationStatus.IncorrectPassword:
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, result.ErrorMessage, new HttpConfiguration());
            }
            return Request.CreateResponse(HttpStatusCode.OK, result.Token, new HttpConfiguration());
        }

        [HttpGet]
        public HttpResponseMessage getTimeSession()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                string timeSession = ConfigurationManager.AppSettings["minutoSessao"] + " Æ " + ConfigurationManager.AppSettings["segundoSessao"];

                retorno.retorno = timeSession;
                if ((timeSession == ""))
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, retorno);
            }
            catch (BusinessException ex)
            {
                logger.Warn(ex);
                retorno.AddMensagem(ex.Message, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                return Request.CreateResponse(HttpStatusCode.BadRequest, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                retorno.AddMensagem(Messages.msgRegBuscError, ex.Message + ex.StackTrace + ex.InnerException, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                return Request.CreateResponse(HttpStatusCode.BadRequest, JsonConvert.SerializeObject(retorno));
            }
        }
    }
}

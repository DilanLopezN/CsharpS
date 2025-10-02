using System;
using System.Net;
using System.Web;
using System.Net.Http;
using System.Threading;
using Componentes.Utils;
using System.Web.Http.Filters;
using System.Web.Http.Controllers;
using Newtonsoft.Json;
using FundacaoFisk.SGF.Web.Services.Auth.Business;
using FundacaoFisk.SGF.GenericModel;
using Componentes.GenericBusiness;
using Componentes.GenericController;
using FundacaoFisk.SGF.Web.Services.InstituicaoEnsino.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Empresa.Comum.IBusiness;
using System.Web.Http;

namespace FundacaoFisk.SGF.Web {
    public class AccessTokenValidationFilterAttribute : AuthorizeAttribute {
        public HttpStatusCode UnauthorizedStatusCode;
        ReturnResult retorno = new ReturnResult();
        private IEscolaBusiness BusinessEscola { get; set; }
        private IAesCryptoHelper CryptoHelper { get; set; }
        
        public AccessTokenValidationFilterAttribute(IAesCryptoHelper crypto, IEscolaBusiness businessEscola) : base() {
            if(businessEscola == null || crypto == null)
                throw new ArgumentNullException("usuarioBusiness");
            BusinessEscola = businessEscola;
            CryptoHelper = crypto;
        }

        protected override bool IsAuthorized(HttpActionContext actionContext) {
            try {
                retorno = new ReturnResult();

                if(actionContext.Request.Headers.Authorization == null || string.IsNullOrEmpty(actionContext.Request.Headers.Authorization.Parameter))
                    throw new AuthBusinessException(Componentes.Utils.Messages.Messages.msgErrorAutorizacao, null, AuthBusinessException.TipoErro.AUTORIZACAO_NAO_ENCONTRADA, false);

                string accessToken = actionContext.Request.Headers.Authorization.Parameter;
                byte[] encryptedAccessToken = Convert.FromBase64String(accessToken);
                string decryptedAccessToken = CryptoHelper.DecryptStringFromBytes_Aes(encryptedAccessToken);
                DateTime tokenExpirationTime = DateTime.ParseExact(decryptedAccessToken.Split(';')[0], "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

                if(DateTime.UtcNow > tokenExpirationTime)
                    throw new AuthBusinessException(Componentes.Utils.Messages.Messages.msgSessaoExpirada, null, AuthBusinessException.TipoErro.AUTORIZACAO_EXPIRADA, false);
                else
                    actionContext.Request.Headers.Add("timeoutSessionAPI", (tokenExpirationTime - DateTime.UtcNow).TotalSeconds + "");

                bool idMaster = bool.Parse(decryptedAccessToken.Split(';')[4]);
                int? codEmpresa = int.Parse(decryptedAccessToken.Split(';')[7]);
                int idFusoHorario = int.Parse(decryptedAccessToken.Split(';')[8]);
                bool isHorarioVerao = bool.Parse(decryptedAccessToken.Split(';')[9]);
                if (codEmpresa == 0)
                    codEmpresa = null;
                Thread.CurrentPrincipal = new ComponentesPrincipal(Thread.CurrentPrincipal.Identity,
                    int.Parse(decryptedAccessToken.Split(';')[1]), int.Parse(decryptedAccessToken.Split(';')[2]), decryptedAccessToken.Split(';')[3], idMaster, codEmpresa, idFusoHorario, isHorarioVerao);

                // Verifica se está no horário de funcionamento da escola:
                // Busca as escolas por id:
                //Escola escolaSelecionada = BusinessEscola.findByIdEscola(int.Parse(decryptedAccessToken.Split(';')[4]));
                TimeSpan? hr_inicial = null;
                if(!"".Equals(decryptedAccessToken.Split(';')[5]))
                    hr_inicial = TimeSpan.Parse(decryptedAccessToken.Split(';')[5]);
                TimeSpan? hr_final = null;
                if(!"".Equals(decryptedAccessToken.Split(';')[6]))
                    hr_final = TimeSpan.Parse(decryptedAccessToken.Split(';')[6]);

                // Compara se o usuário está logando no horário de funcionamento da escola:
                if(hr_inicial.HasValue && hr_final.HasValue && !idMaster)
                    BusinessEscola.verificaHorarioFuncionamentoEscola(hr_inicial.Value, hr_final.Value, DateTime.Now.TimeOfDay);

                return true;
            }
            catch(AuthBusinessException ex) {
                if(ex.tipoErro.Equals(AuthBusinessException.TipoErro.AUTORIZACAO_EXPIRADA))
                    UnauthorizedStatusCode = HttpStatusCode.Unauthorized;
                else if(ex.tipoErro.Equals(AuthBusinessException.TipoErro.AUTORIZACAO_NAO_ENCONTRADA) || ex.tipoErro.Equals(AuthBusinessException.TipoErro.HORARIO_LOGIN_ULTRAPASSADO))
                    UnauthorizedStatusCode = HttpStatusCode.BadRequest;

                retorno.AddMensagem(ex.Message, ex.StackTrace, ReturnResult.MensagemWeb.TipoMensagem.ERROR);

                return false;
            }
            catch(BusinessException ex) {
                UnauthorizedStatusCode = HttpStatusCode.BadRequest;
                retorno.AddMensagem(ex.Message, ex.StackTrace, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                return false;
            }
            catch(Exception ex) {
                UnauthorizedStatusCode = HttpStatusCode.BadRequest;
                retorno.AddMensagem(Componentes.Utils.Messages.Messages.msgErrorAutorizacaoInvalida, ex.StackTrace, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                return false;
            }
        }

        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext) {
            if (actionContext == null) 
                throw new ArgumentNullException("actionContext");
            
            actionContext.Response = actionContext.ControllerContext.Request.CreateResponse(HttpStatusCode.BadRequest, JsonConvert.SerializeObject(retorno));
        }
    }
}

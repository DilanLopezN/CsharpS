using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;
using System.Web.Http.Controllers;
using Componentes.Utils;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;
using Componentes.GenericController.DataAccess;

namespace Componentes.GenericController {
    public class HttpComponentesAuthorizeMSGAttribute : AuthorizeAttribute {
        public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext) {
            var Request = System.Web.HttpContext.Current.Request;

            base.OnAuthorization(actionContext);
        }

        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext) {
            base.HandleUnauthorizedRequest(actionContext);

            // Returns HTTP 401 - see comment in HttpUnauthorizedResult.cs. 
            if(actionContext.Response.StatusCode == System.Net.HttpStatusCode.Unauthorized) {
                ReturnResult retorno = new ReturnResult();

                string mensagemErro = Utils.Messages.Messages.msgSessaoExpirada;
                if(!string.IsNullOrEmpty(((((System.Web.Http.ApiController) (actionContext.ControllerContext.Controller)).User).Identity).Name)) {
                    mensagemErro = Utils.Messages.Messages.msgAcessoNegado;
                    if(String.Empty != this.Roles) {
                        string[] permissoes = this.Roles.Split(',');

                        //Recupera os nomes das permissões do banco de dados:
                        Componentes.GenericBusiness.GenericBusiness genericBusiness = new Componentes.GenericBusiness.GenericBusiness(new SysMenuDataAccess(new GenericModel.ComponentesWebContext()));

                        permissoes = genericBusiness.getMensagensPermissoes(permissoes);
                        if(permissoes != null && permissoes.Length > 0) {
                            mensagemErro += "Permissão(ões) necessária(s) para efetuar esta operação:";
                            foreach(string permissao in permissoes)
                                mensagemErro += "<br/>" + permissao;
                        }
                    }
                }

                retorno.AddMensagem(mensagemErro, Utils.Messages.Messages.msgSessaoExpiradaOuAcessoNegadoComp, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                actionContext.Response = actionContext.Request.CreateResponse(System.Net.HttpStatusCode.BadRequest, JsonConvert.SerializeObjectAsync(retorno).Result);
            }
        }
    }
}
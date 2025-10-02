using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
//using FundacaoFisk.SGF.Web.Services.Usuario.Model;
using FundacaoFisk.SGF.Web.Services.Usuario.Comum;
using Componentes.GenericController;
using FundacaoFisk.SGF.Web.Services.Usuario.Model;
using Newtonsoft.Json;
using Componentes.Utils;
using Componentes.Utils.Messages;
using log4net;
using System.Net.Http.Headers;
using FundacaoFisk.SGF.GenericModel;


namespace FundacaoFisk.SGF.Web.Services.Usuario.Controllers
{
    public class UsuarioController : ComponentesApiController
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(UsuarioController));
        private int peopleUser;

        public int getPeopleUser
        {
            get { return peopleUser = ComponentesUser.CodPessoaUsuario; }
            set { peopleUser = value; }
        }

        public UsuarioController()
        {
        }

        [HttpComponentesAuthorize(Roles = "usu")]
        public HttpResponseMessage getUsuarios()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cd_escola = this.ComponentesUser.CodEmpresa.Value;
                IUsuarioBusiness Business = (IUsuarioBusiness)base.instanciarBusiness<IUsuarioBusiness>();
                IEnumerable<UsuarioWebSGF> usuarios = Business.getUsuarios(cd_escola).ToList();
                retorno.retorno = usuarios;
                
                return Request.CreateResponse(HttpStatusCode.OK, retorno);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Componentes.GenericController;
using Componentes.Utils.Messages;
using FundacaoFisk.SGF.Web.Services.Empresa.Comum.IBusiness;
using log4net;
using Newtonsoft.Json;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Empresa.Controllers
{
    using Componentes.Utils;
    
    public class FuncionarioController : ComponentesApiController {
        private static readonly ILog logger = LogManager.GetLogger(typeof(FuncionarioController));

        public FuncionarioController()
        {
        }

        // GET api/<controller>/5 
        [HttpGet]
        [HttpComponentesAuthorize(Roles = "func")]
        public HttpResponseMessage GetFuncionarioSearch(string nome, string apelido, int status, string cpf, bool inicio, byte tipo, int sexo, int cdAtividade)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = (int)ComponentesUser.CodEmpresa;
                if (nome == null)
                    nome = String.Empty;
                if (apelido == null)
                    apelido = String.Empty;
                if (cpf == null)
                    cpf = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IFuncionarioBusiness Business = (IFuncionarioBusiness)base.instanciarBusiness<IFuncionarioBusiness>();
                var ret = Business.getSearchFuncionario(parametros, nome, apelido, getStatus(status), cpf, inicio, tipo, cdEscola, sexo, cdAtividade);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, ret);
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
            }

        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "func")]
        public HttpResponseMessage GetFuncionarioComAtividadeExtraSearch(string nome, string apelido, int status, string cpf, bool inicio, byte tipo, int sexo, int cdAtividade)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = (int)ComponentesUser.CodEmpresa;
                if (nome == null)
                    nome = String.Empty;
                if (apelido == null)
                    apelido = String.Empty;
                if (cpf == null)
                    cpf = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IFuncionarioBusiness Business = (IFuncionarioBusiness)base.instanciarBusiness<IFuncionarioBusiness>();
                var ret = Business.getSearchFuncionarioComAtividadeExtra(parametros, nome, apelido, getStatus(status), cpf, inicio, tipo, cdEscola, sexo, cdAtividade);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, ret);
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
            }

        }
    }
}

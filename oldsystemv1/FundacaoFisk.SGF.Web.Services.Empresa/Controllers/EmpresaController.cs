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
    using System.Net.Http.Headers;
    using Componentes.GenericBusiness.Comum;
    using Componentes.Utils;
    using FundacaoFisk.SGF.Web.Services.Empresa.Model;
    
    public class EmpresaController : ComponentesApiController {
        private static readonly ILog logger = LogManager.GetLogger(typeof(EmpresaController));

        public EmpresaController()
        {
        }

        [HttpComponentesAuthorize(Roles = "esc.e")]
        public HttpResponseMessage postDeleteEmpresa(List<Escola> empresas)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IEmpresaBusiness Business = (IEmpresaBusiness)base.instanciarBusiness<IEmpresaBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });
                var deletedEmpresa = Business.deleteAllEmpresa(empresas);
                retorno.retorno = deletedEmpresa;
                if(!deletedEmpresa) {
                    retorno.AddMensagem(Messages.msgNotExcludReg, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                }
                else {
                    retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch(Exception ex) {
                return gerarLogException(Messages.msgNotExcludReg, retorno, logger, ex);
            }

        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "esc")]
        public HttpResponseMessage findAllEmpresaByLoginPag([FromUri] EmpresasLoginPagParam empresasLoginPagParam)
        {
            try
            {
                var empresas = new List<int>();
                bool editUser = false;
                IEmpresaBusiness Business = (IEmpresaBusiness)base.instanciarBusiness<IEmpresaBusiness>();

                empresas = Business.findAllEmpresasByUser(empresasLoginPagParam.cd_usuario);
                if (empresas != null && empresas.Count > 0)
                {
                    editUser = true;
                }


                string login = ComponentesUser.Identity.Name;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                
                var retorno = Business.findAllEmpresaByLoginPag(parametros, login, empresas, empresasLoginPagParam.nome, empresasLoginPagParam.fantasia, empresasLoginPagParam.cnpj, empresasLoginPagParam.inicio, editUser);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex); 
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "esc")]
        public HttpResponseMessage findAllEmpresaTransferencia([FromUri] EmpresasLoginPagParam empresasLoginPagParam)
        {
            try
            {
                int cd_escola = ComponentesUser.CodEmpresa.Value;
                IEmpresaBusiness Business = (IEmpresaBusiness)base.instanciarBusiness<IEmpresaBusiness>();

                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());

                var retorno = Business.findAllEmpresaTransferencia(parametros, cd_escola, empresasLoginPagParam.nome, empresasLoginPagParam.fantasia, empresasLoginPagParam.cnpj, empresasLoginPagParam.inicio);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "esc")]
        public HttpResponseMessage findAllEmpresasByUsuarioPag(string cdEmpresas, string nome, string fantasia, string cnpj, bool inicio)
        {
            try
            {
                var empresas = new List<int>();
                bool editUser = false;
                if (!string.IsNullOrEmpty(cdEmpresas))
                {
                    string[] listEmpresas = cdEmpresas.Split(',');
                    foreach (var cd_empresa in listEmpresas)
                        if (!string.IsNullOrEmpty(cd_empresa))
                            empresas.Add(int.Parse(cd_empresa));
                    editUser = true;
                }

                string login = ComponentesUser.Identity.Name;
                int cd_escola = ComponentesUser.CodEmpresa.Value;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IEmpresaBusiness Business = (IEmpresaBusiness)base.instanciarBusiness<IEmpresaBusiness>();
                var retorno = Business.findAllEmpresasByUsuarioPag(parametros, login, empresas, nome, fantasia, cnpj, inicio, editUser, cd_escola);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "esc")]
        public HttpResponseMessage findAllEmpresasUsuarioComboFK()
        {
            try
            {
                string login = ComponentesUser.Identity.Name;
                int cd_escola = ComponentesUser.CodEmpresa.Value;
                IEmpresaBusiness Business = (IEmpresaBusiness)base.instanciarBusiness<IEmpresaBusiness>();
                var retorno = Business.findAllEmpresasUsuarioComboFK(login, cd_escola).ToList();

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "esc")]
        public HttpResponseMessage findQuantidadeEmpresasVinculadasUsuario()
        {
            try
            {
                string login = ComponentesUser.Identity.Name;
                int cd_escola = ComponentesUser.CodEmpresa.Value;
                IEmpresaBusiness Business = (IEmpresaBusiness)base.instanciarBusiness<IEmpresaBusiness>();
                var retorno = Business.findQuantidadeEmpresasVinculadasUsuario(login, cd_escola);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "esc")]
        public HttpResponseMessage getHorarioFuncEmpresa()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEmpresa = (int)ComponentesUser.CodEmpresa;
                IEmpresaBusiness Business = (IEmpresaBusiness)base.instanciarBusiness<IEmpresaBusiness>();
                var parametro = Business.getHorarioFunc(cdEmpresa);
                retorno.retorno = parametro;
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "esc")]
        public HttpResponseMessage getEmpresaHasGrupoMaster(int cd_grupo_master)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IEmpresaBusiness Business = (IEmpresaBusiness)base.instanciarBusiness<IEmpresaBusiness>();
                var empresas = Business.getEmpresaHasGrupoMaster(cd_grupo_master);
                retorno.retorno = empresas;
                return Request.CreateResponse(HttpStatusCode.OK, retorno);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage getMenusAreaRestrita()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                
                IEmpresaBusiness Business = (IEmpresaBusiness)base.instanciarBusiness<IEmpresaBusiness>();
                var menus = Business.getMenusAreaRestrita();
                retorno.retorno = menus;
                return Request.CreateResponse(HttpStatusCode.OK, retorno);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }
    }
}

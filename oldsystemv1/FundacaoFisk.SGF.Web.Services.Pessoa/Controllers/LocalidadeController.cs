using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using FundacaoFisk.SGF.Web.Services.Pessoa.Comum;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using log4net;
using Newtonsoft.Json;
using Componentes.GenericController;
using Componentes.Utils.Messages;
using System.Web;
using System.Resources;
using System.Threading;
using System.Web.UI.WebControls;
using FundacaoFisk.SGF.Web.Services.Pessoa.Comum.Business;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Pessoa.Business;

using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Pessoa.Controllers
{
    public class LocalidadeController : ComponentesApiController
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(LocalidadeController));

        public LocalidadeController()
        {
        }

        #region Pais
        [HttpComponentesAuthorize(Roles = "pais")]
        public HttpResponseMessage GeturlrelatorioPais(string sort, int direction, string descricao, bool inicio)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                // Pega os parâmetros do usuário para criar a url do relatório:
                string strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@descricao=" + descricao + "&@inicio=" + inicio + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=Relatório de País&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.PaisSearch;
                //parametros = HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8);
                //var parametrosCript = ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(parametros, MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);
                string parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);

                return Request.CreateResponse(HttpStatusCode.OK, parametrosCript);
            }
            catch (Exception exe)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, exe);
            }
        }
        [HttpComponentesAuthorize(Roles = "pais")]
        public HttpResponseMessage GetPaisSearch(string descricao, bool inicio)
        {
            try
            {
                if (descricao == null)
                    descricao = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var retorno = Business.GetPaisSearch(parametros, descricao, inicio);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex); 
            }
        }

        // GET api/<controller>/1
        [HttpComponentesAuthorize(Roles = "pais")]
        public HttpResponseMessage GetPaisById(int id)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var esc = Business.GetPaisById(id);
                retorno.retorno = esc;
                if (esc.cd_localidade <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }
        // GET api/<controller>/1
        [HttpComponentesAuthorize(Roles = "pais")]
        public HttpResponseMessage GetPaisEstado()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var esc = Business.GetPaisEstado();
                retorno.retorno = esc;
                if (esc.Count() <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }
        // Put api/<controller>/5
        [HttpComponentesAuthorize(Roles = "pais.a")]
        public HttpResponseMessage PostAlterarPais(PaisUI paisUI)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var putPais = Business.PutPaisLocalidade(paisUI);
                retorno.retorno = putPais;
                if (putPais.cd_localidade <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpDateReg, retorno, logger, ex);
            }

        }
        // Post api/<controller>/5
        [HttpComponentesAuthorize(Roles = "pais.i")]
        public HttpResponseMessage PostPais(PaisUI paisUI)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var pais = Business.PostPaisLocalidade(paisUI);
                retorno.retorno = pais;

                if (pais.cd_localidade <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }

        }
        // Delte api/<controller>
        [HttpComponentesAuthorize(Roles = "pais.e")]
        public HttpResponseMessage PostDeletePais(List<PaisUI> paises)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var esc = Business.DeletePais(paises);
                retorno.retorno = esc;
                if (esc == false)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotExcludReg, retorno, logger, ex);
            }

        }

        // GET api/<controller>/1
        [HttpComponentesAuthorize(Roles = "pais")]
        public HttpResponseMessage GetAllPais()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var paises = Business.GetAllPais();
                retorno.retorno = paises;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        // GET api/<controller>/1
        [HttpComponentesAuthorize(Roles = "pais")]
        public HttpResponseMessage GetAllPaisPorSexoPessoa()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var paises = Business.GetAllPaisPorSexoPessoa();
                retorno.retorno = paises;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }
        #endregion

        #region Estado
        [HttpComponentesAuthorize(Roles = "estad")]
        public HttpResponseMessage GeturlrelatorioEstado(string sort, int direction, string descricao, bool inicio, int cdPais)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                // Pega os parâmetros do usuário para criar a url do relatório:
                string strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@descricao=" + descricao + "&@inicio=" + inicio + "&@cdPais=" + cdPais + "&" + ReportParameter.PARAMETRO_NOME_RELATORIO + "=Relatório de Estado&" + ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.EstadoSearch;
                //parametros = HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8);
                //var parametrosCript = ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(parametros, MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);
                string parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);
                return Request.CreateResponse(HttpStatusCode.OK, parametrosCript);
            }
            catch (Exception exe)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, exe);
            }
        }

        [HttpComponentesAuthorize(Roles = "estad")]
        public HttpResponseMessage getAllEstado()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var allEstado = Business.GetAllEstado();
                retorno.retorno = allEstado;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }


        }

        [HttpComponentesAuthorize(Roles = "estad")]
        public HttpResponseMessage GetEstadoEstado()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var estado = Business.GetEstadoEstado();
                retorno.retorno = estado;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }


        }

        [HttpComponentesAuthorize(Roles = "estad")]
        public HttpResponseMessage getExistLocalidade(int cdLoc, string descLoc)
        {
            var retorno = new ReturnResult();
            try
            {
                if (cdLoc != 0 || descLoc != "")
                {
                    ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                    var retornoLoc = Business.existsLocalidade(cdLoc, descLoc);
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, retornoLoc);
                    response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = retornoLoc }));
                    return response;
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception ex)
            {
                logger.Error("LocalidadeController getExistLocalidade - Erro: " + ex.Message + ex.StackTrace + ex.InnerException);
                retorno.AddMensagem(Messages.msgRegBuscError, ex.Message + ex.StackTrace + ex.InnerException, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                return Request.CreateResponse(HttpStatusCode.BadRequest, JsonConvert.SerializeObject(retorno));
            }
        }


        [HttpComponentesAuthorize(Roles = "estad")]
        public HttpResponseMessage GetEstadoSearch(string descricao, bool inicio, int cdPais)
        {
            try
            {
                if (descricao == null)
                    descricao = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var retorno = Business.GetEstadoSearch(parametros, descricao, inicio, cdPais);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex); 
            }
        }

        // GET api/<controller>/1
        [HttpComponentesAuthorize(Roles = "estad")]
        public HttpResponseMessage GetEstadoById(int id)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var esc = Business.GetEstadoById(id);
                retorno.retorno = esc;
                if (esc.cd_localidade <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }
        // Put api/<controller>/5
        [HttpComponentesAuthorize(Roles = "estad.a")]
        public HttpResponseMessage PostAlterarEstado(EstadoUI estadoUI)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var putEstado = Business.PutEstadoLocalidade(estadoUI);
                retorno.retorno = putEstado;
                if (putEstado.cd_localidade <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpDateReg, retorno, logger, ex);
            }

        }
        // Post api/<controller>/5
        [HttpComponentesAuthorize(Roles = "estad.i")]
        public HttpResponseMessage PostEstado(EstadoUI estadoUI)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var estado = Business.PostEstadoLocalidade(estadoUI);
                retorno.retorno = estado;
                if (estado.cd_localidade <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }

        }
        // Delte api/<controller>
        [HttpComponentesAuthorize(Roles = "estad.e")]
        public HttpResponseMessage PostDeleteEstado(List<EstadoUI> estados)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var esc = Business.DeleteEstado(estados);
                retorno.retorno = esc;
                if (esc == false)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotExcludReg, retorno, logger, ex);
            }

        }


        [HttpComponentesAuthorize(Roles = "estad")]
        public HttpResponseMessage GetEstadoByPais(int cd_pais)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var estado = Business.getEstadoByPais(cd_pais);
                retorno.retorno = estado;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        #endregion

        #region Localidade

        [HttpComponentesAuthorize(Roles = "cidd.a")]
        public HttpResponseMessage PostAlterarLocalidade(LocalidadeSGF localidade)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var putlocalidade = Business.PutLocalidade(localidade);
                retorno.retorno = putlocalidade;
                if (putlocalidade.cd_localidade <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpDateReg, retorno, logger, ex);
            }

        }

        [HttpComponentesAuthorize(Roles = "estad.i")]
        public HttpResponseMessage PostLocalidade(LocalidadeSGF localidade)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var postLocalidade = Business.PostLocalidade(localidade);
                retorno.retorno = postLocalidade;
                if (postLocalidade.cd_localidade <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }

        }
        // Delte api/<controller>

        [HttpComponentesAuthorize(Roles = "estad.e")]
        public HttpResponseMessage PostDeleteLocalidade(List<LocalidadeSGF> localidades)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var loc = Business.DeleteLocalidade(localidades);
                retorno.retorno = loc;
                if (loc == false)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotExcludReg, retorno, logger, ex);
            }

        }

        #endregion

        #region Bairro

        [HttpComponentesAuthorize(Roles = "bair")]
        public HttpResponseMessage GeturlrelatorioBairro(string sort, int direction, string descricao, bool inicio, int cd_cidade)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                // Pega os parâmetros do usuário para criar a url do relatório:
                string strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@descricao=" + descricao + "&@inicio=" + inicio + "&@cd_cidade=" +cd_cidade + "&" + ReportParameter.PARAMETRO_NOME_RELATORIO + "=Relatório de Bairro&" + ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.BairroSearch;
                //parametros = HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8);
                //var parametrosCript = ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(parametros, MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);

                string parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);
                return Request.CreateResponse(HttpStatusCode.OK, parametrosCript);
            }
            catch (Exception exe)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, exe);
            }
        }

        [HttpComponentesAuthorize(Roles = "bair")]
        public HttpResponseMessage GetBairro(string search)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var r = Business.FindBairro(search);
                retorno.retorno = r;

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "bair")]
        public HttpResponseMessage GetBairroSearch(string descricao, bool inicio, int cd_cidade)
        {
            try
            {
                if (descricao == null)
                    descricao = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var retorno = Business.GetBairroSearch(parametros, descricao, inicio, cd_cidade);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex); 
            }
        }

        [HttpComponentesAuthorize(Roles = "bair")]
        public HttpResponseMessage GetBairroById(int id)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var esc = Business.GetBairroById(id);
                retorno.retorno = esc;
                if (esc.cd_localidade <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "bair")]
        public HttpResponseMessage GetBairroDesc(string descricao)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var bairro = Business.GetBairroDesc(descricao);
                retorno.retorno = bairro;
                if (bairro.cd_localidade <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "bair")]
        public HttpResponseMessage PostLocalidadePessoaBairro(LocalidadeSGF localidade)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                localidade.cd_tipo_localidade = (int)TipoLocalidadeSGF.TipoLocalidadeSGFEnum.BAIRRO;
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                LocalidadeSGF localidadeContext = Business.PostLocalidade(localidade);
                LocalidadeSGF localRetorno = new LocalidadeSGF();
                localRetorno.cd_localidade_bairro = localidadeContext.cd_localidade;
                localRetorno.localidades = Business.getBairroPorCidade((int)localidadeContext.cd_loc_relacionada, 0).ToList();
                retorno.retorno = localRetorno;
                if (localRetorno.cd_loc_relacionada <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }

        }


        [HttpComponentesAuthorize(Roles = "bair")]
        public HttpResponseMessage getBairroPorCidade(int cd_cidade, int cd_bairro)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var bairros = Business.getBairroPorCidade(cd_cidade, cd_bairro);
                retorno.retorno = bairros;
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }


        [HttpComponentesAuthorize(Roles = "bair.a")]
        public HttpResponseMessage PostUpdateBairro(LocalidadeSGF localidade)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                localidade.cd_tipo_localidade = (int)LocalidadeSGF.TipoLocalidadeSGF.BAIRRO;
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var putlocalidade = Business.PutLocalidade(localidade);
                retorno.retorno = putlocalidade;
                if (putlocalidade.cd_localidade <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpDateReg, retorno, logger, ex);
            }

        }

        [HttpComponentesAuthorize(Roles = "bair.i")]
        public HttpResponseMessage PostInsertBairro(LocalidadeSGF localidade)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                localidade.cd_tipo_localidade = (int)LocalidadeSGF.TipoLocalidadeSGF.BAIRRO;
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var postLocalidade = Business.PostLocalidade(localidade);
                retorno.retorno = postLocalidade;
                if (postLocalidade.cd_localidade <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }

        }
        [HttpComponentesAuthorize(Roles = "bair.e")]
        public HttpResponseMessage PostDeleteBairro(List<LocalidadeSGF> localidades)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var loc = Business.DeleteLocalidade(localidades);
                retorno.retorno = loc;
                if (loc == false)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotExcludReg, retorno, logger, ex);
            }

        }

        #endregion

        #region Cidade

        [HttpComponentesAuthorize(Roles = "cidd")]
        public HttpResponseMessage GeturlrelatorioCidade(string sort, int direction, string descricao, bool inicio, int nmMunicipio, int cdEstado)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                // Pega os parâmetros do usuário para criar a url do relatório:
                string strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@descricao=" + descricao + "&@inicio=" + inicio + "&@nmMunicipio=" + nmMunicipio + "&@cdEstado=" + cdEstado + "&" + ReportParameter.PARAMETRO_NOME_RELATORIO + "=Relatório de Cidade&" + ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.CidadeSearch;
                //parametros = HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8);
                //var parametrosCript = ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(parametros, MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);

                string parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);
                return Request.CreateResponse(HttpStatusCode.OK, parametrosCript);
            }
            catch (Exception exe)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, exe);
            }
        }

        [HttpComponentesAuthorize(Roles = "cidd")]
        public HttpResponseMessage GetCidadeSearch(string descricao, bool inicio, int nmMunicipio, int cdEstado)
        {
            try
            {
                if (descricao == null)
                    descricao = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var retorno = Business.GetCidadeSearch(parametros, descricao, inicio, nmMunicipio, cdEstado);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex); 
            }
        }

        [HttpComponentesAuthorize(Roles = "cidd")]
        public HttpResponseMessage GetCidadeById(int id)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var esc = Business.GetCidadeById(id);
                retorno.retorno = esc;
                if (esc.cd_localidade <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        //Perciste uma cidade para um estado respectivo
        [HttpComponentesAuthorize(Roles = "cidd.i")]
        public HttpResponseMessage PostCidade(CidadeUI cidade)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var incluirCidade = Business.PostCidade(cidade);
                retorno.retorno = incluirCidade;

                if ((incluirCidade.cd_localidade <= 0))
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                else
                {
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                logger.Error("CidadeController PostCidade - Erro: " + ex.Message + ex.StackTrace + ex.InnerException);
                retorno.AddMensagem(Messages.msgRegBuscError, ex.Message + ex.StackTrace + ex.InnerException, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                return Request.CreateResponse(HttpStatusCode.BadRequest, JsonConvert.SerializeObject(retorno));
            }
        }

        [HttpComponentesAuthorize(Roles = "cidd.a")]
        public HttpResponseMessage PostAlterarCidade(CidadeUI cidade)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var putlocalidade = Business.PutCidade(cidade);
                retorno.retorno = putlocalidade;
                if (putlocalidade.cd_localidade <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpDateReg, retorno, logger, ex);
            }

        }
        [HttpComponentesAuthorize(Roles = "cidd.e")]
        public HttpResponseMessage PostDeleteCidade(List<LocalidadeSGF> localidades)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var loc = Business.DeleteLocalidade(localidades);
                retorno.retorno = loc;
                if (loc == false)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotExcludReg, retorno, logger, ex);
            }

        }

        // GET api/<controller>
        [HttpComponentesAuthorize(Roles = "cidd")]
        [HttpComponentesAuthorize(Roles = "estad")]
        public HttpResponseMessage GetCidade(int idEstado)
        {
            //return Business.GetAllCidade(id);
            ReturnResult retorno = new ReturnResult();
            try
            {
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var cidades = Business.GetAllCidade(idEstado);
                retorno.retorno = cidades;
                if (cidades.Count() <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                };
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                logger.Error("CidadeController GetCidade - Erro: " + ex.Message + ex.StackTrace + ex.InnerException);
                retorno.AddMensagem(Messages.msgRegBuscError, ex.Message + ex.StackTrace + ex.InnerException, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                return Request.CreateResponse(HttpStatusCode.BadRequest, JsonConvert.SerializeObject(retorno));
            }
        }

        // GET api/<controller>
        [HttpComponentesAuthorize(Roles = "cidd")]
        [HttpComponentesAuthorize(Roles = "estad")]
        [HttpGet]
        public HttpResponseMessage GetCidade(string search, int idEstado)
        {
            //return Business.GetAllCidade(id);
            ReturnResult retorno = new ReturnResult();
            try
            {
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var cidades = Business.GetAllCidade(search, idEstado);
                retorno.retorno = cidades;
                if (cidades.Count() <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                };
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                logger.Error("CidadeController GetCidade - Erro: " + ex.Message + ex.StackTrace + ex.InnerException);
                retorno.AddMensagem(Messages.msgRegBuscError, ex.Message + ex.StackTrace + ex.InnerException, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                return Request.CreateResponse(HttpStatusCode.BadRequest, JsonConvert.SerializeObject(retorno));
            }
        }

        // GET api/<controller>
        [HttpComponentesAuthorize(Roles = "cidd")]
        [HttpComponentesAuthorize(Roles = "estad")]
        [HttpComponentesAuthorize(Roles = "pais")]
        public HttpResponseMessage GetCidadePaisEstado(int pais, int estado, int numeroMunicipio, string cidade)
        {
            try
            {
                if (cidade == null)
                {
                    cidade = String.Empty;
                }
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var cidades = Business.GetCidadePaisEstado(parametros, pais, estado, numeroMunicipio, cidade);
                // Pega os parâmetros do usuário para criar a url do relatório:
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, cidades);
                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex); 
            }

        }

        // GET api/<controller>
        [HttpComponentesAuthorize(Roles = "cidd")]
        public HttpResponseMessage GetCidadeByDesc(string descCidade)
        {
            //return Business.GetAllCidade(id);
            ReturnResult retorno = new ReturnResult();
            try
            {
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var cidades = Business.FindCidade(descCidade);
                retorno.retorno = cidades;
                if (cidades.Count() <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                };
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                logger.Error("CidadeController GetCidade - Erro: " + ex.Message + ex.StackTrace + ex.InnerException);
                retorno.AddMensagem(Messages.msgRegBuscError, ex.Message + ex.StackTrace + ex.InnerException, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                return Request.CreateResponse(HttpStatusCode.BadRequest, JsonConvert.SerializeObject(retorno));
            }
        }

        [HttpComponentesAuthorize(Roles = "cidd")]
        public HttpResponseMessage PostLocalidadePessoaCidade(LocalidadeSGF localidade)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                localidade.cd_tipo_localidade = (int)TipoLocalidadeSGF.TipoLocalidadeSGFEnum.CIDADE;
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                LocalidadeSGF localidadeContext = Business.PostLocalidade(localidade);
                LocalidadeSGF localRetorno = new LocalidadeSGF();
                localRetorno.cd_localidade = localidadeContext.cd_localidade;
                localRetorno.cd_localidade_cidade = localidadeContext.cd_localidade;
                localRetorno.localidades = Business.GetAllCidade(localidadeContext.cd_loc_relacionada.Value).ToList();
                retorno.retorno = localRetorno;
                if (localRetorno.cd_localidade <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }

        }

        #endregion

        #region Distrito
        [HttpComponentesAuthorize(Roles = "dist")]
        public HttpResponseMessage GeturlrelatorioDistrito(string sort, int direction, string descricao, bool inicio, int cd_cidade)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                // Pega os parâmetros do usuário para criar a url do relatório:
                string strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@descricao=" + descricao + "&@inicio=" + inicio +"&@cd_cidade="+ cd_cidade + "&" + ReportParameter.PARAMETRO_NOME_RELATORIO + "=Relatório de Distrito&" + ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.DistritoSearch;
                //parametros = HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8);
                //var parametrosCript = ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(parametros, MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);

                string parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);
                return Request.CreateResponse(HttpStatusCode.OK, parametrosCript);
            }
            catch (Exception exe)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, exe);
            }
        }

        [HttpComponentesAuthorize(Roles = "dist")]
        public HttpResponseMessage GetDistritoSearch(string descricao, bool inicio, int cd_cidade)
        {
            try
            {
                if (descricao == null)
                    descricao = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var retorno = Business.GetDistritoSearch(parametros, descricao, inicio, cd_cidade);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex); 
            }
        }
        [HttpComponentesAuthorize(Roles = "dist")]
        public HttpResponseMessage GetDistritoById(int id)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var esc = Business.GetDistritoById(id);
                retorno.retorno = esc;
                if (esc.cd_localidade <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }


        [HttpComponentesAuthorize(Roles = "dist.a")]
        public HttpResponseMessage PostUpdateDistrito(LocalidadeSGF localidade)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                localidade.cd_tipo_localidade = (int)LocalidadeSGF.TipoLocalidadeSGF.DISTRITO;
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var putlocalidade = Business.PutLocalidade(localidade);
                retorno.retorno = putlocalidade;
                if (putlocalidade.cd_localidade <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpDateReg, retorno, logger, ex);
            }

        }

        [HttpComponentesAuthorize(Roles = "dist.i")]
        public HttpResponseMessage PostInsertDistrito(LocalidadeSGF localidade)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                localidade.cd_tipo_localidade = (int)LocalidadeSGF.TipoLocalidadeSGF.DISTRITO;
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var postLocalidade = Business.PostLocalidade(localidade);
                retorno.retorno = postLocalidade;
                if (postLocalidade.cd_localidade <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }

        }
        [HttpComponentesAuthorize(Roles = "dist.e")]
        public HttpResponseMessage PostDeleteDistrito(List<LocalidadeSGF> localidades)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var loc = Business.DeleteLocalidade(localidades);
                retorno.retorno = loc;
                if (loc == false)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotExcludReg, retorno, logger, ex);
            }

        }

        #endregion

        #region TipoEndereco
        [HttpComponentesAuthorize(Roles = "tpend")]
        public HttpResponseMessage GeturlrelatorioTipoEndereco(string sort, int direction, string descricao, bool inicio)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                // Pega os parâmetros do usuário para criar a url do relatório:
                string strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@descricao=" + descricao + "&@inicio=" + inicio + "&" + ReportParameter.PARAMETRO_NOME_RELATORIO + "=Relatório de Tipo de Endereço&" + ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.TipoEnderecoSearch;
                //parametros = HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8);
                //var parametrosCript = ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(parametros, MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);

                string parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);
                return Request.CreateResponse(HttpStatusCode.OK, parametrosCript);
            }
            catch (Exception exe)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, exe);
            }
        }

        //Fazer try
        [HttpComponentesAuthorize(Roles = "tpend")]
        public HttpResponseMessage getAllTipoEndereco()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var allTipoEndereco = Business.GetAllTipoEndereco();
                retorno.retorno = allTipoEndereco;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex); 
            }
        }

        [HttpComponentesAuthorize(Roles = "tpend")]
        public HttpResponseMessage GetTipoEnderecoSearch(string descricao, bool inicio)
        {
            try
            {
                if (descricao == null)
                    descricao = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var retorno = Business.GetTipoEnderecoSearch(parametros, descricao, inicio);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex); 
            }
        }
        [HttpComponentesAuthorize(Roles = "tpend")]
        public HttpResponseMessage GetTipoEnderecoById(int id)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var esc = Business.GetTipoEnderecoById(id);
                retorno.retorno = esc;
                if (esc.cd_tipo_endereco <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "tpend.a")]
        public HttpResponseMessage PostAlterarTipoEndereco(TipoEnderecoSGF tipoEndereco)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var putTipoEndereco = Business.PutTipoEndereco(tipoEndereco);
                retorno.retorno = putTipoEndereco;
                if (putTipoEndereco.cd_tipo_endereco <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpDateReg, retorno, logger, ex);
            }

        }
        [HttpComponentesAuthorize(Roles = "tpend.i")]
        public HttpResponseMessage PostTipoEndereco(TipoEnderecoSGF tipoEndereco)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var postTipoEndereco = Business.PostTipoEndereco(tipoEndereco);
                retorno.retorno = postTipoEndereco;
                if (postTipoEndereco.cd_tipo_endereco <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }

        }
        // Delte api/<controller>
        [HttpComponentesAuthorize(Roles = "tpend.e")]
        public HttpResponseMessage PostDeleteTipoEndereco(List<TipoEnderecoSGF> tiposEndereco)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var tpend = Business.DeleteTipoEndereco(tiposEndereco);
                retorno.retorno = tpend;
                if (tpend == false)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotExcludReg, retorno, logger, ex);
            }

        }
        #endregion

        #region ClasseTelefone
        [HttpComponentesAuthorize(Roles = "ctele")]
        public HttpResponseMessage GeturlrelatorioClasseTelefone(string sort, int direction, string descricao, bool inicio)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                // Pega os parâmetros do usuário para criar a url do relatório:
                string strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@descricao=" + descricao + "&@inicio=" + inicio + "&" + ReportParameter.PARAMETRO_NOME_RELATORIO + "=Relatório de Classe de Telefone&" + ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.ClasseTelefoneSearch;
                //parametros = HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8);
                //var parametrosCript = ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(parametros, MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);
                string parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);
                return Request.CreateResponse(HttpStatusCode.OK, parametrosCript);
            }
            catch (Exception exe)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, exe);
            }
        }

        [HttpComponentesAuthorize(Roles = "ctele")]
        public HttpResponseMessage GetClasseTelefoneSearch(string descricao, bool inicio)
        {
            try
            {
                if (descricao == null)
                    descricao = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var retorno = Business.GetClasseTelefoneSearch(parametros, descricao, inicio);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex); 
            }
        }
        [HttpComponentesAuthorize(Roles = "ctele")]
        public HttpResponseMessage GetClasseTelefoneById(int id)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var esc = Business.GetClasseTelefoneById(id);
                retorno.retorno = esc;
                if (esc.cd_classe_telefone <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "ctele.a")]
        public HttpResponseMessage PostAlterarClasseTelefone(ClasseTelefoneSGF classeTelefone)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var putClasseTelefone = Business.PutClasseTelefone(classeTelefone);
                retorno.retorno = putClasseTelefone;
                if (putClasseTelefone.cd_classe_telefone <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpDateReg, retorno, logger, ex);
            }

        }
        [HttpComponentesAuthorize(Roles = "ctele.i")]
        public HttpResponseMessage PostClasseTelefone(ClasseTelefoneSGF classeTelefone)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var postClasseTelefone = Business.PostClasseTelefone(classeTelefone);
                retorno.retorno = postClasseTelefone;
                if (postClasseTelefone.cd_classe_telefone <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }

        }
        // Delte api/<controller>
        [HttpComponentesAuthorize(Roles = "ctele.e")]
        public HttpResponseMessage PostDeleteClasseTelefone(List<ClasseTelefoneSGF> classesTelefone)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var tpend = Business.DeleteClasseTelefone(classesTelefone);
                retorno.retorno = tpend;
                if (tpend == false)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotExcludReg, retorno, logger, ex);
            }

        }
        [HttpComponentesAuthorize(Roles = "ctele")]
        public HttpResponseMessage getAllClasseTelefone()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var allClasseTelefone = Business.GetAllClasseTelefone();
                retorno.retorno = allClasseTelefone;
                if (allClasseTelefone.Count() <= 0)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception exe)
            {
                retorno.AddMensagem(Messages.msgRegBuscError, exe.Message + exe.StackTrace + exe.InnerException, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                return Request.CreateResponse(HttpStatusCode.BadRequest, JsonConvert.SerializeObject(retorno));
            }
        }
        #endregion

        #region TipoLogradouro

        [HttpComponentesAuthorize(Roles = "tlog")]
        public HttpResponseMessage GeturlrelatorioTipoLogradouro(string sort, int direction, string descricao, bool inicio)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                // Pega os parâmetros do usuário para criar a url do relatório:
                string strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@descricao=" + descricao + "&@inicio=" + inicio + "&" + ReportParameter.PARAMETRO_NOME_RELATORIO + "=Relatório de Tipo de Logradouro&" + ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.TipoLogradouroSearch;
                //parametros = HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8);
                //var parametrosCript = ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(parametros, MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);

                string parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);
                return Request.CreateResponse(HttpStatusCode.OK, parametrosCript);
            }
            catch (Exception exe)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, exe);
            }
        }

        [HttpComponentesAuthorize(Roles = "tlog")]
        public HttpResponseMessage getAllTipoLogradouro()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var tiposLogradouro = Business.GetAllTipoLogradouro();
                retorno.retorno = tiposLogradouro;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "tlog")]
        public HttpResponseMessage GetTipoLogradouroSearch(string descricao, bool inicio)
        {
            try
            {
                if (descricao == null)
                    descricao = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var retorno = Business.GetTipoLogradouroSearch(parametros, descricao, inicio);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex); 
            }
        }
        [HttpComponentesAuthorize(Roles = "tlog")]
        public HttpResponseMessage GetTipoLogradouroById(int id)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var esc = Business.GetTipoLogradouroById(id);
                retorno.retorno = esc;
                if (esc.cd_tipo_logradouro <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "tlog.a")]
        public HttpResponseMessage PostAlterarTipoLogradouro(TipoLogradouroSGF tipoLogradouro)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var putTipoLogradouro = Business.PutTipoLogradouro(tipoLogradouro);
                retorno.retorno = putTipoLogradouro;
                if (putTipoLogradouro.cd_tipo_logradouro <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpDateReg, retorno, logger, ex);
            }

        }
        [HttpComponentesAuthorize(Roles = "tlog.i")]
        public HttpResponseMessage PostTipoLogradouro(TipoLogradouroSGF tipoLogradouro)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var postTipoLogradouro = Business.PostTipoLogradouro(tipoLogradouro);
                retorno.retorno = postTipoLogradouro;
                if (postTipoLogradouro.cd_tipo_logradouro <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }

        }
        // Delte api/<controller>
        [HttpComponentesAuthorize(Roles = "tlog.e")]
        public HttpResponseMessage PostDeleteTipoLogradouro(List<TipoLogradouroSGF> tiposLogradouro)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var tpend = Business.DeleteTipoLogradouro(tiposLogradouro);
                retorno.retorno = tpend;
                if (tpend == false)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotExcludReg, retorno, logger, ex);
            }

        }
        #endregion

        #region TipoTelefone
        [HttpComponentesAuthorize(Roles = "ttele")]
        public HttpResponseMessage GeturlrelatorioTipoTelefone(string sort, int direction, string descricao, bool inicio)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                // Pega os parâmetros do usuário para criar a url do relatório:
                string strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@descricao=" + descricao + "&@inicio=" + inicio + "&" + ReportParameter.PARAMETRO_NOME_RELATORIO + "=Relatório de Tipo de Contato&" + ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.TipoTelefoneSearch;
                //parametros = HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8);
                //var parametrosCript = ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(parametros, MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);
                string parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);
                return Request.CreateResponse(HttpStatusCode.OK, parametrosCript);
            }
            catch (Exception exe)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, exe);
            }
        }
        [HttpComponentesAuthorize(Roles = "ttele")]
        public HttpResponseMessage GetTipoTelefoneSearch(string descricao, bool inicio)
        {
            try
            {
                if (descricao == null)
                    descricao = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var retorno = Business.GetTipoTelefoneSearch(parametros, descricao, inicio);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex); 
            }
        }
        [HttpComponentesAuthorize(Roles = "ttele")]
        public HttpResponseMessage GetTipoTelefoneById(int id)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var esc = Business.GetTipoTelefoneById(id);
                retorno.retorno = esc;
                if (esc.cd_tipo_telefone <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "ttele.a")]
        public HttpResponseMessage PostAlterarTipoTelefone(TipoTelefoneSGF tipoTelefone)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var putTipoTelefone = Business.PutTipoTelefone(tipoTelefone);
                retorno.retorno = putTipoTelefone;
                if (putTipoTelefone.cd_tipo_telefone <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpDateReg, retorno, logger, ex);
            }

        }
        [HttpComponentesAuthorize(Roles = "ttele.i")]
        public HttpResponseMessage PostTipoTelefone(TipoTelefoneSGF tipoTelefone)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var postTipoTelefone = Business.PostTipoTelefone(tipoTelefone);
                retorno.retorno = postTipoTelefone;
                if (postTipoTelefone.cd_tipo_telefone <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }

        }
        // Delte api/<controller>
        [HttpComponentesAuthorize(Roles = "ttele.e")]
        public HttpResponseMessage PostDeleteTipoTelefone(List<TipoTelefoneSGF> tiposTelefone)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var tpend = Business.DeleteTipoTelefone(tiposTelefone);
                retorno.retorno = tpend;
                if (tpend == false)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotExcludReg, retorno, logger, ex);
            }

        }
        [HttpComponentesAuthorize(Roles = "ttele")]
        public HttpResponseMessage getAllTipoTelefone()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var allTipoTelefones = Business.GetAllTipoTelefone();
                retorno.retorno = allTipoTelefones;
                if (allTipoTelefones.Count() <= 0)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception exe)
            {
                logger.Error("localidadeController getAllTipoTelefone - Erro: ", exe);
                retorno.AddMensagem(Messages.msgRegBuscError, exe.Message + exe.StackTrace + exe.InnerException, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                return Request.CreateResponse(HttpStatusCode.BadRequest, JsonConvert.SerializeObject(retorno));
            }
        }
        #endregion

        #region Operadora
        [HttpComponentesAuthorize(Roles = "oper")]
        public HttpResponseMessage GeturlrelatorioOperadora(string sort, int direction, string descricao, bool inicio, int status)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                // Pega os parâmetros do usuário para criar a url do relatório:
                string strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@descricao=" + descricao + "&@inicio=" + inicio + "&@status=" + status + "&" + ReportParameter.PARAMETRO_NOME_RELATORIO + "=Relatório de Operadora&" + ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.OperadoraSearch;
                //parametros = HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8);
                //var parametrosCript = ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(parametros, MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);
                string parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);
                return Request.CreateResponse(HttpStatusCode.OK, parametrosCript);
            }
            catch (Exception exe)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, exe);
            }
        }


        [HttpComponentesAuthorize(Roles = "oper")]
        public HttpResponseMessage GetOperadoraSearch(string descricao, bool inicio, int status)
        {
            try
            {
                if (descricao == null)
                    descricao = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var retorno = Business.GetOperadoraSearch(parametros, descricao, inicio, getStatus(status));
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex); 
            }
        }
        [HttpComponentesAuthorize(Roles = "oper")]
        public HttpResponseMessage GetOperadoraById(int id)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var esc = Business.GetOperadoraById(id);
                retorno.retorno = esc;
                if (esc.cd_operadora <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "oper.a")]
        public HttpResponseMessage PostAlterarOperadora(Operadora operadora)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var putOperadora = Business.PutOperadora(operadora);
                retorno.retorno = putOperadora;
                if (putOperadora.cd_operadora <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpDateReg, retorno, logger, ex);
            }

        }
        [HttpComponentesAuthorize(Roles = "oper.i")]
        public HttpResponseMessage PostOperadora(Operadora operadora)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var postOperadora = Business.PostOperadora(operadora);
                retorno.retorno = postOperadora;
                if (postOperadora.cd_operadora <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }

        }
        // Delte api/<controller>
        [HttpComponentesAuthorize(Roles = "oper.e")]
        public HttpResponseMessage PostDeleteOperadora(List<Operadora> operadoras)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var tpend = Business.DeleteOperadora(operadoras);
                retorno.retorno = tpend;
                if (tpend == false)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotExcludReg, retorno, logger, ex);
            }

        }
        #endregion

        #region Atividade
        [HttpComponentesAuthorize(Roles = "ativ")]
        public HttpResponseMessage GeturlrelatorioAtividade(string sort, int direction, string descricao, bool inicio, int status, int natureza, string cnae)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                // Pega os parâmetros do usuário para criar a url do relatório:

                string strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@descricao=" + descricao + "&@inicio=" + inicio + "&@status=" + status + "&@natureza=" + natureza + "&@cnae=" + cnae + "&" + ReportParameter.PARAMETRO_NOME_RELATORIO + "=Relatório de Atividade/Profissão&" + ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.AtividadeSearch;
                string parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);
                //var parametrosCript = ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(parametros, MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);

                return Request.CreateResponse(HttpStatusCode.OK, parametrosCript);
            }
            catch (Exception exe)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, exe);
            }
        }

        [HttpComponentesAuthorize(Roles = "ativ")]
        public HttpResponseMessage GetAtividadeSearch(string descricao, bool inicio, int status, int natureza, string cnae)
        {
            try
            {
                if (descricao == null)
                    descricao = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var retorno = Business.GetAtividadeSearch(parametros, descricao, inicio, getStatus(status), natureza, cnae);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex); 
            }
        }
        [HttpComponentesAuthorize(Roles = "ativ")]
        public HttpResponseMessage GetAtividadeById(int id)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var esc = Business.GetAtividadeById(id);
                retorno.retorno = esc;
                if (esc.cd_atividade <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "ativ.a")]
        public HttpResponseMessage PostAlterarAtividade(Atividade atividade)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var putAtividade = Business.PutAtividade(atividade);
                retorno.retorno = putAtividade;
                if (putAtividade.cd_atividade <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpDateReg, retorno, logger, ex);
            }

        }
        [HttpComponentesAuthorize(Roles = "ativ.i")]
        public HttpResponseMessage PostAtividade(Atividade atividade)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var postAtividade = Business.PostAtividade(atividade);
                retorno.retorno = postAtividade;
                if (postAtividade.cd_atividade <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }

        }
        // Delte api/<controller>
        [HttpComponentesAuthorize(Roles = "ativ.e")]
        public HttpResponseMessage PostDeleteAtividade(List<Atividade> atividades)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var tpend = Business.DeleteAtividade(atividades);
                retorno.retorno = tpend;
                if (tpend == false)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotExcludReg, retorno, logger, ex);
            }

        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "pes")]
        public HttpResponseMessage getAllAtividades(string search, int natureza, int status)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var atividades = Business.getAllListAtividades(search, natureza, getStatus(status));
                retorno.retorno = atividades;
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));

            }
            catch (Exception ex)
            {
                logger.Error("PessoaController getAllAtividades - Erro: ", ex);
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }
        #endregion

        #region Logradouro
        //Restrito ao uso dos metodos do cadastro de Pessoa . (autocomplete)
        [Obsolete]
        [HttpComponentesAuthorize(Roles = "pes")]
        public HttpResponseMessage GetLogradouroDesc(string descricao)
        {

            ReturnResult retorno = new ReturnResult();
            try
            {
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var logradouro = Business.GetEnderecoDesc(descricao);
                retorno.retorno = logradouro;
                if (logradouro.cd_localidade <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "pes")]
        public HttpResponseMessage GetEndereco(string searchText)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var logradouro = Business.GetAllEndereco(searchText);
                retorno.retorno = logradouro;
                if (logradouro.Equals(null))
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "pes")]
        public HttpResponseMessage PostLocalidadePessoaLogradouro(LocalidadeSGF localidade)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                localidade.cd_tipo_localidade = (int)TipoLocalidadeSGF.TipoLocalidadeSGFEnum.LOGRADOURO;
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                LocalidadeSGF localidadeRetorno = Business.PostLocalidade(localidade);
                //Localidade localRetorno = new Localidade();
                //localRetorno.cd_localidade_rua = localidadeRetorno.cd_localidade;
                //localRetorno.localidades = Business.getAllLogradouroPorBairro((int)localidadeRetorno.cd_loc_relacionada).ToList();
                retorno.retorno = localidadeRetorno;
                if (localidadeRetorno.cd_localidade <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }

        }

        [HttpComponentesAuthorize(Roles = "lgra")]
        public HttpResponseMessage getLogradouroSearch(string descricao, bool inicio, int cd_estado, int cd_cidade, int cd_bairro, string cep)
        {
            try
            {
                if (descricao == null)
                    descricao = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var retorno = Business.getLogradouroSearch(parametros, descricao, inicio, cd_estado, cd_cidade, cd_bairro, cep);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex); 
            }
        }

        [HttpComponentesAuthorize(Roles = "lgra")]
        public HttpResponseMessage getLogradouroCorreio(string descricao, string estado, string cidade, string bairro, string cep, int? numero)
        {
            try
            {
                if (descricao == null)
                    descricao = String.Empty;
                List<LocalidadeSGF> localidades = new List<LocalidadeSGF>();
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var resultado = Business.getLogradouroCorreio(descricao, estado, cidade, bairro, cep, numero);
                if (resultado != null && resultado.Count() > 0)
                    foreach (LogradouroCEP l in resultado)
                        localidades.Add(LocalidadeSGF.converterLocalidade(l));
                var retorno = localidades;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex); 
            }
        }

        [HttpComponentesAuthorize(Roles = "lgra.e")]
        public HttpResponseMessage postDeleteLogradouros(List<LocalidadeSGF> logradouros)
        {
            ReturnResult retorno = new ReturnResult();
            var cd_escola = (int)this.ComponentesUser.CodEmpresa;
            try
            {
                int[] cdLogradouros = null;
                int i;
                // Pegando códigos da Turma
                if (logradouros != null && logradouros.Count() > 0)
                {
                    i = 0;
                    int[] cdLogradourosCont = new int[logradouros.Count()];
                    foreach (var c in logradouros)
                    {
                        cdLogradourosCont[i] = c.cd_localidade;
                        i++;
                    }
                    cdLogradouros = cdLogradourosCont;
                }
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var delMovtos = Business.deleteLogradouros(cdLogradouros);
                retorno.retorno = delMovtos;
                if (!delMovtos)
                    retorno.AddMensagem(Messages.msgNotExcludReg, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                else
                    retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotExcludReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "lgra.i")]
        public HttpResponseMessage postInsertLogradouro(LocalidadeSGF logradouro)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                logradouro.cd_tipo_localidade = (int)LocalidadeSGF.TipoLocalidadeSGF.LOGRADOURO;
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var cadLog = Business.addLogradouro(logradouro);
                retorno.retorno = cadLog;
                if (cadLog.cd_localidade <= 0)
                    retorno.AddMensagem(Messages.msgNotIncludReg, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }

        }

        [HttpComponentesAuthorize(Roles = "lgra.a")]
        public HttpResponseMessage postUpdateLogradouro(LocalidadeSGF logradouro)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                logradouro.cd_tipo_localidade = (int)LocalidadeSGF.TipoLocalidadeSGF.LOGRADOURO;
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var cadLog = Business.editLogradouro(logradouro);
                retorno.retorno = cadLog;
                if (cadLog.cd_localidade <= 0)
                    retorno.AddMensagem(Messages.msgNotUpDateReg, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpDateReg, retorno, logger, ex);
            }

        }

        [HttpComponentesAuthorize(Roles = "bair")]
        public HttpResponseMessage GeturlrelatorioLogradouro(string sort, int direction, string descricao, bool inicio, int cd_estado,int cd_cidade, int cd_bairro, string cep)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                // Pega os parâmetros do usuário para criar a url do relatório:
                string strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@descricao=" + descricao + "&@inicio=" + inicio + " &@cd_estado=" + cd_estado + "&@cd_cidade=" + cd_cidade + "&@cd_bairro=" + cd_bairro +
                    "&@cep=" + cep + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=Relatório de Logradouro&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.Logradouro;
                //parametros = HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8);
                //var parametrosCript = ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(parametros, MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);

                string parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);
                return Request.CreateResponse(HttpStatusCode.OK, parametrosCript);
            }
            catch (Exception exe)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, exe);
            }
        }

        // GET api/<controller>
        [HttpComponentesAuthorize(Roles = "cidd")]
        [HttpComponentesAuthorize(Roles = "estad")]
        public HttpResponseMessage GetLogradouroPorBairro(int cd_bairro)
        {
            //return Business.GetAllCidade(id);
            ReturnResult retorno = new ReturnResult();
            try
            {
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var cidades = Business.getAllLogradouroPorBairro(cd_bairro);
                retorno.retorno = cidades;
                if (cidades.Count() <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                };
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                logger.Error("CidadeController GetCidade - Erro: " + ex.Message + ex.StackTrace + ex.InnerException);
                retorno.AddMensagem(Messages.msgRegBuscError, ex.Message + ex.StackTrace + ex.InnerException, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                return Request.CreateResponse(HttpStatusCode.BadRequest, JsonConvert.SerializeObject(retorno));
            }
        }

        // GET api/<controller>
        [HttpComponentesAuthorize(Roles = "lgra")]
        [HttpComponentesAuthorize(Roles = "bair")]
        [HttpComponentesAuthorize(Roles = "cidd")]
        [HttpComponentesAuthorize(Roles = "estad")]
        public HttpResponseMessage getEnderecoByCdLogradouro(int cd_logradouro, string nm_cep)
        {
            //return Business.GetAllCidade(id);
            ReturnResult retorno = new ReturnResult();
            try
            {
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                EnderecoSGF endereco = Business.getEnderecoByLogradouro(cd_logradouro, nm_cep);
                if (!String.IsNullOrEmpty(endereco.num_cep))
                {
                    getTipoLocalidade(endereco.num_cep, endereco);
                }
                retorno.retorno = endereco;
                if (endereco != null)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                };
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (PessoaBusinessException ex)
            {
                if (ex.tipoErro == PessoaBusinessException.TipoErro.ERRO_CONVERSAO_ENDERECO)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        // GET api/<controller>
        [HttpComponentesAuthorize(Roles = "lgra")]
        [HttpComponentesAuthorize(Roles = "bair")]
        [HttpComponentesAuthorize(Roles = "cidd")]
        [HttpComponentesAuthorize(Roles = "estad")]
        public HttpResponseMessage getEnderecoCEP(string nm_cep)
        {
            //return Business.GetAllCidade(id);
            ReturnResult retorno = new ReturnResult();
            try
            {
                EnderecoSGF endereco = new EnderecoSGF();
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                endereco = Business.getEnderecoByLogradouro(0, nm_cep);
                if (endereco == null || endereco.cd_loc_estado <= 0)
                {
                    var resultado = Business.getLogradouroCorreio(null, null, null, null, nm_cep, 0).ToList();
                    if (resultado != null && resultado.Count() > 0)
                    {
                        endereco = new EnderecoSGF();
                        //endereco.enderecotipoLogradouro = resultado[0].
                        endereco.noLocRua = resultado[0].Logradouro;
                        endereco.noLocCidade = resultado[0].Cidade;
                        endereco.noLocBairro = resultado[0].Bairro;
                        endereco.noLocEstado = resultado[0].Uf;
                        endereco.num_cep = nm_cep;
                        endereco = Business.verificaSeExisteEnderecoOuGravar(endereco);
                        endereco.num_cep = nm_cep;
                    }
                }

                getTipoLocalidade(nm_cep, endereco);


                retorno.retorno = endereco;
                if (endereco == null)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                };
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (PessoaBusinessException ex)
            {
                if (ex.tipoErro == PessoaBusinessException.TipoErro.ERRO_CONVERSAO_ENDERECO)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        private void getTipoLocalidade(string nm_cep, EnderecoSGF endereco)
        {
            ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
            if (endereco != null)
            {
                if (endereco.cd_tipo_logradouro == null)
                {
                    var tiposLogradouro = Business.GetAllTipoLogradouro();
                    var enderecoCep = Business.getLogradouroCorreio(null, null, null, null, nm_cep, 0).ToList();
                    if (enderecoCep != null && enderecoCep.Count() > 0)
                    {
                        if (!string.IsNullOrEmpty(enderecoCep[0].TipoLog))
                        {
                            var tipoLogradouro = tiposLogradouro.Where(l =>
                                l.no_tipo_logradouro.ToLower() == enderecoCep[0].TipoLog.ToLower() ||
                                l.sg_tipo_logradouro.ToLower() == enderecoCep[0].TipoLog.ToLower());

                            if (tipoLogradouro != null && tipoLogradouro.Count() > 0)
                            {
                                endereco.cd_tipo_logradouro = tipoLogradouro.FirstOrDefault().cd_tipo_logradouro;
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Endereço

        // GET api/<controller>
        [HttpComponentesAuthorize(Roles = "lgra")]
        [HttpComponentesAuthorize(Roles = "bair")]
        [HttpComponentesAuthorize(Roles = "cidd")]
        [HttpComponentesAuthorize(Roles = "estad")]
        public HttpResponseMessage postEnderecoBuscaCep(EnderecoSGF endereco)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                EnderecoSGF enderecoCad = Business.verificaSeExisteEnderecoOuGravar(endereco);
                retorno.retorno = enderecoCad;
                enderecoCad.num_cep = endereco.num_cep;
                if (enderecoCad.cd_loc_logradouro <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                };
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (PessoaBusinessException ex)
            {
                if (ex.tipoErro == PessoaBusinessException.TipoErro.ERRO_ESTADO_NAO_CADASTRADO_CEP)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "pes")]
        public HttpResponseMessage getEnderecoResponsavelCPF(int cd_pessoa)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ILocalidadeBusiness Business = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                var pessoaFisica = Business.getEnderecoResponsavelCPF(cd_pessoa);
                retorno.retorno = pessoaFisica;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        #endregion
    }
}

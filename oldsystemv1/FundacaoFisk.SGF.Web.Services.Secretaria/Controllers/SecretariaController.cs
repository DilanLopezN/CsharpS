using Componentes.GenericBusiness.Comum;
using Componentes.GenericController;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Utils;
using FundacaoFisk.SGF.Utils.Messages;
using FundacaoFisk.SGF.Web.Services.Auth.Model;
using FundacaoFisk.SGF.Web.Services.Empresa.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Financeiro.Business;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Pessoa.Business;
using FundacaoFisk.SGF.Web.Services.Pessoa.Comum.Business;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using FundacaoFisk.SGF.Web.Services.Secretaria.Business;
using FundacaoFisk.SGF.Web.Services.Secretaria.Comum;
using FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess;
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.ModelBinding;
using FundacaoFisk.SGF.Web.Services.Auth.Comum;
using FundacaoFisk.SGF.Services.Coordenacao.Business;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Controllers
{
    //[RoutePrefix("Secretaria")]
    public class SecretariaController : ComponentesApiController
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(SecretariaController));

        public SecretariaController()
        {
        }

        #region Escolaridade

        [HttpComponentesAuthorize(Roles = "escd")]
        public HttpResponseMessage GeturlrelatorioEsc(string sort, int direction, string descricao, bool inicio, int status) {
            ReturnResult retorno = new ReturnResult();
            
            try {
                // Pega os parâmetros do usuário para criar a url do relatório:
                var strParametrosSort = "@cs=" + sort + "&@so=" + direction +"&";
                string parametros = strParametrosSort+"@descricao=" + descricao + "&@inicio=" + inicio + "&@status=" + status + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=Relatório de Escolaridade&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.EscolaridadeSearch;
                var parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);
                
                HttpResponseMessage response = null;
                if(parametrosCript == null)
                    response = Request.CreateResponse(HttpStatusCode.NotFound);
                else
                    response = Request.CreateResponse(HttpStatusCode.OK, parametrosCript);
                configureHeaderResponse(response, null);
                return response;
            }
            catch(Exception ex) {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        // GET api/<controller>
        [HttpComponentesAuthorize(Roles = "escd")]
        public HttpResponseMessage GetEscolaridadeSearch(string descricao, bool inicio, int status)
        {
            try
            {
                if (descricao == null)
                    descricao = String.Empty;
                SearchParameters parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var retorno = Business.GetEscolaridadeSearch(parametros, descricao, inicio, getStatus(status));
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception exe)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, exe);
            }
        }

        // GET api/<controller>/1
        [HttpComponentesAuthorize(Roles = "escd")]
        public HttpResponseMessage GetEscolaridadeById(int id)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var esc = Business.GetEscolaridadeById(id);
                retorno.retorno = esc;
                if (esc.cd_escolaridade <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        // Put api/<controller>/5
        [HttpComponentesAuthorize(Roles = "escd.a")]
        public HttpResponseMessage PostAlterarEscolaridade(Escolaridade escolaridade)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var esc = Business.PutEscolaridade(escolaridade);
                retorno.retorno = esc;
                if (esc.cd_escolaridade <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
        }
        // Post api/<controller>/5
        [HttpComponentesAuthorize(Roles = "escd.i")]
        public HttpResponseMessage PostEscolaridade(Escolaridade escolaridade)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var esc = Business.PostEscolaridade(escolaridade);
                retorno.retorno = esc;
                if (esc.cd_escolaridade <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
        }

        // Delte api/<controller>
        [HttpComponentesAuthorize(Roles = "escd.e")]
        public HttpResponseMessage PostDeleteEscolaridade(List<Escolaridade> escolaridades)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                bool deletou = Business.DeleteEscolaridade(escolaridades);
                retorno.retorno = deletou;
                if (!deletou)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                else
                    retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotDeletedReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "escd")]
        [HttpComponentesAuthorize(Roles = "anesc")]
        public HttpResponseMessage getEscolaridade(int status)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var mot = Business.getEscolaridade(getStatus(status));
                retorno.retorno = mot;

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }


        #endregion

        #region Ano Escolar
        [HttpGet]
        [HttpComponentesAuthorize(Roles = "anesc")]
        public HttpResponseMessage GetAnoEscolarSearch(int? cdEscolaridade, string descricao, bool inicio, int status)
        {
            try
            {
                if (descricao == null)
                    descricao = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var retorno = Business.GetAnoEscolarSearch(parametros, cdEscolaridade, descricao, inicio, getStatus(status));
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
        [HttpComponentesAuthorize(Roles = "anesc")]
        public HttpResponseMessage getEscolaridadePossuiAnoEscolar()
        {
            try
            {
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var retorno = Business.getEscolaridadePossuiAnoEscolar();
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpPost]
        [HttpComponentesAuthorize(Roles = "anesc.i")]
        public HttpResponseMessage PostAnoEscolar(AnoEscolar anoEscolar)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var esc = Business.PostAnoEscolar(anoEscolar);
                retorno.retorno = esc;
                if (esc.cd_escolaridade <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
        }

        [HttpPost]
        [HttpComponentesAuthorize(Roles = "anesc.a")]
        public HttpResponseMessage PutAnoEscolar(AnoEscolar anoEscolar)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var esc = Business.PutAnoEscolar(anoEscolar);
                retorno.retorno = esc;
                if (esc.cd_ano_escolar <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
        }

        [HttpPost]
        [HttpComponentesAuthorize(Roles = "anesc.e")]
        public HttpResponseMessage DeleteAnoEscolar(List<AnoEscolar> anoEscolar)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                bool deletou = Business.DeleteAnoEscolar(anoEscolar);
                retorno.retorno = deletou;
                if (!deletou)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                else
                    retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;

            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotDeletedReg, retorno, logger, ex);
            }

        }

        [HttpComponentesAuthorize(Roles = "anesc")]
        public HttpResponseMessage GeturlrelatorioAnoEscolar(string sort, int direction, int? cdEscolaridade, string descricao, bool inicio, int status)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                // Pega os parâmetros do usuário para criar a url do relatório:
                var strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@cdEscolaridade=" + cdEscolaridade + "&@descricao=" + descricao + "&@inicio=" + inicio + "&@status=" + status + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=Relatório de Ano Escolar&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.AnoEscolar;
                //parametros = HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8);
                string parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, parametrosCript);

                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        #endregion

        #region Midia
        //Midia
        [HttpComponentesAuthorize(Roles = "mid")]
        public HttpResponseMessage GeturlrelatorioMid(string sort, int direction, string descricao, bool inicio, int status)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                // Pega os parâmetros do usuário para criar a url do relatório:
                var strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@descricao=" + descricao + "&@inicio=" + inicio + "&@status=" + status + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=Relatório de Mídia&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.MidiaSearch;
                //parametros = HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8);
                string parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, parametrosCript);

                configureHeaderResponse(response, null);

                return response;

            }
            catch (Exception exe)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, exe);
            }
        }

        [HttpComponentesAuthorize(Roles = "mid")]
        public HttpResponseMessage GetMidiaSearch(string descricao, bool inicio, int status)
        {
            try
            {
                if (descricao == null)
                    descricao = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var retorno = Business.GetMidiaSearch(parametros, descricao, inicio, getStatus(status));
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
        [HttpComponentesAuthorize(Roles = "mid")]
        public HttpResponseMessage GetMidiaById(int id)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var esc = Business.GetMidiaById(id);
                retorno.retorno = esc;
                if (esc.cd_midia <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }
        // Put api/<controller>/5
        [HttpComponentesAuthorize(Roles = "mid.a")]
        public HttpResponseMessage PostAlterarMidia(Midia Midia)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var esc = Business.PutMidia(Midia);
                retorno.retorno = esc;
                if (esc.cd_midia <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;

            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }

        }
        // Post api/<controller>/5
        [HttpComponentesAuthorize(Roles = "mid.i")]
        public HttpResponseMessage PostMidia(Midia Midia)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var esc = Business.PostMidia(Midia);
                retorno.retorno = esc;
                if (esc.cd_midia <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }

        }
        // Delte api/<controller>
        [HttpComponentesAuthorize(Roles = "mid.e")]
        public HttpResponseMessage PostDeleteMidia(List<Midia> midias)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                bool deletou = Business.DeleteMidia(midias);
                retorno.retorno = deletou;
                if(!deletou)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                else
                    retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotDeletedReg, retorno, logger, ex);
            }

        }

        [HttpComponentesAuthorize(Roles = "mid")]
        public HttpResponseMessage getMidia(int status)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var mot = Business.getMidia(getStatus(status), MidiaDataAccess.TipoConsultaMidiaEnum.HAS_ATIVO_INATIVO, null);
                retorno.retorno = mot;

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;

            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "mid")]
        public HttpResponseMessage getComponentesRelatorioAlunoCliente()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;

                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var midias = Business.getMidia(null, MidiaDataAccess.TipoConsultaMidiaEnum.HAS_ALUNO, cdEscola);
                retorno.retorno = new { midias = midias};
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;

            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "mid")]
        [Obsolete]
        public HttpResponseMessage getMidiasProspect()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var midias = Business.getMidia(null, MidiaDataAccess.TipoConsultaMidiaEnum.HAS_PROSPECT, cdEscola);
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObjectAsync(midias).Result);
                configureHeaderResponse(response, null);
                return response;

            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        #endregion

        #region TipoContato
        //TipoContato
        [HttpComponentesAuthorize(Roles = "tpctt")]
        public HttpResponseMessage GeturlrelatorioTipoContato(string sort, int direction, string descricao, bool inicio, int status)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                // Pega os parâmetros do usuário para criar a url do relatório:
                var strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@descricao=" + descricao + "&@inicio=" + inicio + "&@status=" + status + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=Relatório de Tipo de Contato&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.TipoContatoSearch;
                //parametros = HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8);
                string parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);
              
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, parametrosCript);

                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        // GET api/<controller>
        [HttpComponentesAuthorize(Roles = "tpctt")]
        public HttpResponseMessage GetTipoContatoSearch(string descricao, bool inicio, int status)
        {
            try
            {
                if (descricao == null)
                    descricao = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var retorno = Business.GetTipoContatoSearch(parametros, descricao, inicio, getStatus(status));
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
        [HttpComponentesAuthorize(Roles = "tpctt")]
        public HttpResponseMessage GetTipoContatoById(int id)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var esc = Business.GetTipoContatoById(id);
                retorno.retorno = esc;
                if (esc.cd_tipo_contato <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;

            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }
        // Put api/<controller>/5
        [HttpComponentesAuthorize(Roles = "tpctt.a")]
        public HttpResponseMessage PostAlterarTipoContato(TipoContato tipocontato)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var esc = Business.PutTipoContato(tipocontato);
                retorno.retorno = esc;
                if (esc.cd_tipo_contato <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);


                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;

            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }

        }
        // Post api/<controller>/5
        [HttpComponentesAuthorize(Roles = "tpctt.i")]
        public HttpResponseMessage PostTipoContato(TipoContato tipocontato)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var esc = Business.PostTipoContato(tipocontato);
                retorno.retorno = esc;
                if (esc.cd_tipo_contato <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);

            }

        }
        // Delte api/<controller>
        [HttpComponentesAuthorize(Roles = "tpctt.e")]
        public HttpResponseMessage PostDeleteTipoContato(List<TipoContato> tipocontato)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                bool deletou = Business.DeleteTipoContato(tipocontato);
                retorno.retorno = deletou;
                if(!deletou)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                else
                    retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotDeletedReg, retorno, logger, ex);
            }

        }
        #endregion

        #region Motivo Matricula
        //Motivo Matricula
        [HttpComponentesAuthorize(Roles = "mtmt")]
        public HttpResponseMessage GeturlrelatorioMtvMat(string sort, int direction, string descricao, bool inicio, int status)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                // Pega os parâmetros do usuário para criar a url do relatório:
                var strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@descricao=" + descricao + "&@inicio=" + inicio + "&@status=" + status + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=Relatório de Motivo da Matricula&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.MtvMatSearch;
                //parametros = HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8);
                string parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, parametrosCript);

                configureHeaderResponse(response, null);
                return response;
          
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }
        // GET api/<controller>
        [HttpComponentesAuthorize(Roles = "mtmt")]
        public HttpResponseMessage GetMotivoMatriculaSearch(string descricao, bool inicio, int status)
        {
            try
            {
                if (descricao == null)
                    descricao = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var retorno = Business.GetMotivoMatriculaSearch(parametros, descricao, inicio, getStatus(status));
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
        [HttpComponentesAuthorize(Roles = "mtmt")]
        public HttpResponseMessage GetMotivoMatriculaById(int id)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var esc = Business.GetMotivoMatriculaById(id);
                retorno.retorno = esc;
                if (esc.cd_motivo_matricula <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }
        // Put api/<controller>/5
        [HttpComponentesAuthorize(Roles = "mtmt.a")]
        public HttpResponseMessage PostAlterarMotivoMatricula(MotivoMatricula motivomatricula)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var esc = Business.PutMotivoMatricula(motivomatricula);
                retorno.retorno = esc;
                if (esc.cd_motivo_matricula <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }

        }
        // Post api/<controller>/5
        [HttpComponentesAuthorize(Roles = "mtmt.i")]
        public HttpResponseMessage PostMotivoMatricula(MotivoMatricula motivomatricula)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var esc = Business.PostMotivoMatricula(motivomatricula);
                retorno.retorno = esc;
                if (esc.cd_motivo_matricula <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }

        }
        // Delte api/<controller>
        [HttpComponentesAuthorize(Roles = "mtmt.e")]
        public HttpResponseMessage PostDeleteMotivoMatricula(List<MotivoMatricula> motivomatricula)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                bool deletou = Business.DeleteMotivoMatricula(motivomatricula);
                retorno.retorno = deletou;
                if(!deletou)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                else
                    retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotDeletedReg, retorno, logger, ex);
            }

        }
        #endregion

        #region Motivo Não Matricula

        //Motivo Não Matricula
        [HttpComponentesAuthorize(Roles = "mtnm")]
        public HttpResponseMessage GeturlrelatorioMtvNMat(string sort, int direction, string descricao, bool inicio, int status)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                // Pega os parâmetros do usuário para criar a url do relatório:
                var strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@descricao=" + descricao + "&@inicio=" + inicio + "&@status=" + status + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=Relatório de Motivo da Não Matricula&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.MtvNMatSearch;
                //parametros = HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8);
                string parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, parametrosCript); 
                
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception exe)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, exe);
            }
        }
        [HttpComponentesAuthorize(Roles = "mtnm")]
        public HttpResponseMessage GetMotivoNMatriculaSearch(string descricao, bool inicio, int status)
        {
            try
            {
                if (descricao == null)
                    descricao = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var retorno = Business.GetMotivoNaoMatriculaSearch(parametros, descricao, inicio, getStatus(status));
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
        [HttpComponentesAuthorize(Roles = "mtnm")]
        public HttpResponseMessage GetMotivoNMatriculaById(int id)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var esc = Business.GetMotivoNaoMatriculaById(id);
                retorno.retorno = esc;
                if (esc.cd_motivo_nao_matricula <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }
        [HttpComponentesAuthorize(Roles = "mtnm")]
        public HttpResponseMessage getMotivoNaoMatriculaProspect(int cdProspect)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var mtnm = Business.getMotivoNaoMatriculaProspect(cdProspect);
                var listMotivoNao = mtnm.ToList();
                for (int i = 0; i < listMotivoNao.Count(); i++)
                {
                    var mot = listMotivoNao[i].MotivoNaoMatriculaProspect.ToList();
                    for(int j = 0; j< mot.Count(); j++)
                        mot[j].MotivoNaoMatricula = null;
                }
                retorno.retorno = mtnm;
                if (mtnm == null)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        // Put api/<controller>/5
        [HttpComponentesAuthorize(Roles = "mtnm.a")]
        public HttpResponseMessage PostAlterarMotivoNMatricula(MotivoNaoMatricula motivonaomatricula)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var esc = Business.PutMotivoNaoMatricula(motivonaomatricula);
                retorno.retorno = esc;
                if (esc.cd_motivo_nao_matricula <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;

            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }

        }
        // Post api/<controller>/5
        [HttpComponentesAuthorize(Roles = "mtnm.i")]
        public HttpResponseMessage PostMotivoNMatricula(MotivoNaoMatricula motivonaomatricula)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var esc = Business.PostMotivoNaoMatricula(motivonaomatricula);
                retorno.retorno = esc;
                if (esc.cd_motivo_nao_matricula <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }

        }
        // Delte api/<controller>
        [HttpComponentesAuthorize(Roles = "mtnm.e")]
        public HttpResponseMessage PostDeleteMotivoNMatricula(List<MotivoNaoMatricula> motivonaomatricula)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                if (motivonaomatricula == null || motivonaomatricula.Count == 0)
                    logger.Warn("Tamanho da lista inválida.");
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                bool deletou = Business.DeleteMotivoNaoMatricula(motivonaomatricula);
                retorno.retorno = deletou;
                if(!deletou)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                else
                    retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotDeletedReg, retorno, logger, ex);
            }

        }
        #endregion

        #region Motivo Transferencia Aluno 
        [HttpComponentesAuthorize(Roles = "mttr")]
        public HttpResponseMessage GetMotivoTransferenciaSearch(string descricao, bool inicio, int status)
        {
            try
            {
                if (descricao == null)
                    descricao = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var retorno = Business.GetMotivoTransferenciaSearch(parametros, descricao, inicio, getStatus(status));
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
        [HttpComponentesAuthorize(Roles = "mttr")]
        public HttpResponseMessage GetMotivoTransferenciaById(int id)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var esc = Business.GetMotivoTransferenciaById(id);
                retorno.retorno = esc;
                if (esc.cd_motivo_transferencia_aluno <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }
        // Put api/<controller>/5
        [HttpComponentesAuthorize(Roles = "mttr.a")]
        public HttpResponseMessage PostAlterarMotivoTransferencia(MotivoTransferenciaAluno motivotransferencia)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var esc = Business.PutMotivoTransferencia(motivotransferencia);
                retorno.retorno = esc;
                if (esc.cd_motivo_transferencia_aluno <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;

            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }

        }
        // Post api/<controller>/5
        [HttpComponentesAuthorize(Roles = "mtnm.i")]
        public HttpResponseMessage PostMotivoTransferencia(MotivoTransferenciaAluno motivotransferencia)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var esc = Business.PostMotivoTransferencia(motivotransferencia);
                retorno.retorno = esc;
                if (esc.cd_motivo_transferencia_aluno <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }

        }
        // Delte api/<controller>
        [HttpComponentesAuthorize(Roles = "mtnm.e")]
        public HttpResponseMessage PostDeleteMotivoTransferencia(List<MotivoTransferenciaAluno> motivotransferencia)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                if (motivotransferencia == null || motivotransferencia.Count == 0)
                    logger.Warn("Tamanho da lista inválida.");
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                bool deletou = Business.DeleteMotivoTransferencia(motivotransferencia);
                retorno.retorno = deletou;
                if (!deletou)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                else
                    retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotDeletedReg, retorno, logger, ex);
            }

        }

        #endregion
        #region Motivo Bolsa

        [HttpComponentesAuthorize(Roles = "mtb")]
        public HttpResponseMessage GeturlrelatorioMtvBolsa(string sort, int direction, string descricao, bool inicio, int status)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                // Pega os parâmetros do usuário para criar a url do relatório:
                var strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@descricao=" + descricao + "&@inicio=" + inicio + "&@status=" + status + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=Relatório de Motivo da Bolsa&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.MtvBolsaSearch;
                //parametros = HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8);
                string parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, parametrosCript);
              
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception exe)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, exe);
            }
        }

        //Motivo Bolsa
        [HttpComponentesAuthorize(Roles = "mtb")]
        [HttpGet]
        public HttpResponseMessage GetMotivoBolsaSearch(string descricao, bool inicio, int status)
        {
            try
            {
                if (descricao == null)
                    descricao = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var retorno = Business.GetMotivoBolsaSearch(parametros, descricao, inicio, getStatus(status));
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "mtb")]
        public HttpResponseMessage getMotivoBolsa(int status, int? cd_motivo_bolsa)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var mot = Business.getMotivoBolsa(getStatus(status), cd_motivo_bolsa);
                retorno.retorno = mot;


                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }
              

        // GET api/<controller>/1
        [HttpComponentesAuthorize(Roles = "mtb")]
        public HttpResponseMessage GetMotivoBolsaById(int id)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var esc = Business.GetMotivoBolsaById(id);
                retorno.retorno = esc;
                if (esc.cd_motivo_bolsa <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);


                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;

            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }
        // Put api/<controller>/5
        [HttpComponentesAuthorize(Roles = "mtb.a")]
        public HttpResponseMessage PostAlterarMotivoBolsa(MotivoBolsa motivobolsa)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var esc = Business.PutMotivoBolsa(motivobolsa);
                retorno.retorno = esc;
                if (esc.cd_motivo_bolsa <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }

        }
        // Post api/<controller>/5
        [HttpComponentesAuthorize(Roles = "mtb.i")]
        public HttpResponseMessage PostMotivoBolsa(MotivoBolsa motivobolsa)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var esc = Business.PostMotivoBolsa(motivobolsa);
                retorno.retorno = esc;
                if (esc.cd_motivo_bolsa <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }

        }
        // Delte api/<controller>
        [HttpComponentesAuthorize(Roles = "mtb.e")]
        public HttpResponseMessage PostDeleteMotivoBolsa(List<MotivoBolsa> motivobolsa)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                bool deletou = Business.DeleteMotivoBolsa(motivobolsa);
                retorno.retorno = deletou;
                if(!deletou)
                    retorno.AddMensagem(Messages.msgNotDeletedReg, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                else
                    retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotDeletedReg, retorno, logger, ex);
            }

        }
        #endregion

        #region Motivo Cancelamento Bolsa

        [HttpComponentesAuthorize(Roles = "mtcb")]
        public HttpResponseMessage GeturlrelatorioMtvCancelBolsa(string sort, int direction, string descricao, bool inicio, int status)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                // Pega os parâmetros do usuário para criar a url do relatório:
                var strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@descricao=" + descricao + "&@inicio=" + inicio + "&@status=" + status + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=Relatório de Motivo do Cancelamento da Bolsa&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.MtvCancelBolsaSearch;
                //parametros = HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8);
                string parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);
               
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, parametrosCript);

                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception exe)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, exe);
            }
        }

        //Motivo Cancelamento Bolsa
        [HttpComponentesAuthorize(Roles = "mtcb")]
        public HttpResponseMessage GetMotivoCancelBolsaSearch(string descricao, bool inicio, int status)
        {
            try
            {
                if (descricao == null)
                    descricao = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var retorno = Business.GetMotivoCancelBolsaSearch(parametros, descricao, inicio, getStatus(status));
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
        [HttpComponentesAuthorize(Roles = "mtcb")]
        public HttpResponseMessage GetMotivoCancelBolsaById(int id)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var esc = Business.GetMotivoCancelBolsaById(id);
                retorno.retorno = esc;
                if (esc.cd_motivo_cancelamento_bolsa <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }
        // Put api/<controller>/5
        [HttpComponentesAuthorize(Roles = "mtcb.a")]
        public HttpResponseMessage PostAlterarMotivoCancelBolsa(MotivoCancelamentoBolsa motivocancelbolsa)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var esc = Business.PutMotivoCancelBolsa(motivocancelbolsa);
                retorno.retorno = esc;
                if (esc.cd_motivo_cancelamento_bolsa <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }

        }
        // Post api/<controller>/5
        [HttpComponentesAuthorize(Roles = "mtcb.i")]
        public HttpResponseMessage PostMotivoCancelBolsa(MotivoCancelamentoBolsa motivocancelbolsa)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var esc = Business.PostMotivoCancelBolsa(motivocancelbolsa);
                retorno.retorno = esc;
                if (esc.cd_motivo_cancelamento_bolsa <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }

        }
        // Delte api/<controller>
        [HttpComponentesAuthorize(Roles = "mtcb.e")]
        public HttpResponseMessage PostDeleteMotivoCancelBolsa(List<MotivoCancelamentoBolsa> motivocancelbolsa)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                bool deletou = Business.DeleteMotivoCancelBolsa(motivocancelbolsa);
                retorno.retorno = deletou;
                if(!deletou)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                else
                    retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;

            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotDeletedReg, retorno, logger, ex);
            }

        }

        [HttpComponentesAuthorize(Roles = "mtcb")]
        public HttpResponseMessage getMotivoCancelamentoBolsa(int status, int? cd_motivo_cancelamento_bolsa)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var mot = Business.getMotivoCancelamentoBolsa(getStatus(status), cd_motivo_cancelamento_bolsa);
                retorno.retorno = mot;

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        #endregion

        #region Prospect
//        [HttpComponentesAuthorize(Roles = "mtnm")]
        [HttpComponentesAuthorize(Roles = "pros")]
        public HttpResponseMessage getProspectMotivoNaoMatricula(int cdProspect)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cd_escola = this.ComponentesUser.CodEmpresa.Value;
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var ret = Business.getProspectMotivoNaoMatricula(cdProspect, cd_escola);
                retorno.retorno = ret;
                if (ret == null)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, retorno);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }


        [HttpPost, AllowAnonymous]
        public HttpResponseMessage postProspectIntegracao(ProspectIntegracaoUI prospectIntegracaoUI)
        {
            ReturnResult retorno = new ReturnResult();
            JsonTeste jsonTeste = new JsonTeste();
            ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
            try
            {
                if (prospectIntegracaoUI == null)
                {
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, JsonConvert.SerializeObject(retorno));
                    configureHeaderResponse(response, null);
                    return response;
                }

                #region grava Tabela TJson
                jsonTeste.id_tipo = prospectIntegracaoUI.id_tipo ?? default(byte);
                jsonTeste.id_teste = prospectIntegracaoUI.id ?? default(int); ;
                jsonTeste.dc_json = JsonConvert.SerializeObject(prospectIntegracaoUI);
                jsonTeste.id_erro = (byte)JsonTeste.ErroProcedure.SUCESSO_EXECUCAO_PROCEDURE;

                var jsonTesteRet = Business.postJsonTeste(jsonTeste);
                #endregion

                #region chamaProcedure geraProspect               
                ProspectIntegracaoRetornoUI ret = Business.postProspectIntegracao(prospectIntegracaoUI.unit_fisk_id, prospectIntegracaoUI.id_tipo, prospectIntegracaoUI.id, prospectIntegracaoUI.name, prospectIntegracaoUI.email, prospectIntegracaoUI.phone_number, prospectIntegracaoUI.cep, prospectIntegracaoUI.day_week, prospectIntegracaoUI.period, prospectIntegracaoUI.dt_cadastro, prospectIntegracaoUI.sexo, prospectIntegracaoUI.hit_percentage, prospectIntegracaoUI.phase, prospectIntegracaoUI.course_id);
                #endregion

                #region Grava em caso de erro JSonTeste
                if (ret != null && !ret.retunvalue) 
                {
                    string msg = Messages.msgProcedureError;
                    Business.editJsonTeste(jsonTeste.cd_json_teste, (byte)JsonTeste.ErroProcedure.ERRO_EXECUCAO_PROCEDURE, msg);

                    retorno.AddMensagem(Messages.msgProcedureError, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, JsonConvert.SerializeObject(retorno));
                    configureHeaderResponse(response, null);
                    return response;
                }
                #endregion

                #region Pesquisa o prospect salvo e grava a promocao

                if (ret.cd_prospect > 0)
                {
                    Business.enviaPromocao(ret.cd_prospect);
                }
                
                #endregion

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                #region Retorna Json com erro

                    var msg = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
                    Business.editJsonTeste(jsonTeste.cd_json_teste, (byte)JsonTeste.ErroProcedure.ERRO_EXECUCAO_PROCEDURE, msg);

                Business.editJsonTeste(jsonTeste.cd_json_teste, (byte)JsonTeste.ErroProcedure.ERRO_EXECUCAO_PROCEDURE, msg);

                    ExceptionHandler exceptionHandler = new ExceptionHandler(msg, "An erro has occurred.", ex.GetType().ToString(), ex.StackTrace);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, exceptionHandler);
                    return response;

                #endregion

            }
        }

        [HttpGet, AllowAnonymous]
        public HttpResponseMessage getSendPromocaoProspectsGerados()
        {
            ReturnResult retorno = new ReturnResult();
            JsonTeste jsonTeste = new JsonTeste();
            ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
            int cd_prospect_erro = 0;
            try
            {


                #region chamaProcedure geraProspect               
                ProspectGeradoIntegracaoRetornoUI ret = Business.postGetProspectsGeradosSendPromocao();
                #endregion

                #region Grava em caso de erro JSonTeste
                if (ret != null && !ret.retunvalue)
                {
                    retorno.AddMensagem(Messages.msgProcedureError, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, JsonConvert.SerializeObject(retorno));
                    configureHeaderResponse(response, null);
                    return response;
                }
                #endregion

                #region Pesquisa o prospect salvo e grava a promocao

                if (!string.IsNullOrEmpty(ret.cd_prospect))
                {
                    string[] cds_prospect = ret.cd_prospect.Split('|');
                    if (cds_prospect != null && cds_prospect.Count() > 0)
                    {
                        List<int> idsProspects = cds_prospect.ToList().Where(x => !string.IsNullOrEmpty(x)).Select(x => Int32.Parse(x)).OrderBy(x=>x).Where(x=> x > 0).ToList();
                        if (idsProspects != null && idsProspects.Count > 0)
                        {
                            foreach (int cod_prospect in idsProspects)
                            {
                                cd_prospect_erro = cod_prospect;
                                Business.enviaPromocao(cod_prospect);
                            }
                        }
                    }
                        
                    
                }

                #endregion

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            
            catch (Exception ex)
            {
                #region Retorna Json com erro

                var msg = (ex.InnerException != null ? ex.InnerException.Message : ex.Message) + "cd_prospect_erro: " + cd_prospect_erro;

                ExceptionHandler exceptionHandler = new ExceptionHandler(msg, "An erro has occurred.", ex.GetType().ToString(), ex.StackTrace);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, exceptionHandler);
                return response;

                #endregion

            }
        }

        [HttpGet, AllowAnonymous]
        public HttpResponseMessage getSendPromocaoProspect(int cd_prospect)
        {
            ReturnResult retorno = new ReturnResult();
            JsonTeste jsonTeste = new JsonTeste();
            ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
            int cd_prospect_erro = 0;
            try
            {
                #region Pesquisa o prospect salvo e grava a promocao

                if (cd_prospect > 0)
                {
                    
                    cd_prospect_erro = cd_prospect;
                    Business.enviaPromocao(cd_prospect);
                        
                }

                #endregion

                return Request.CreateResponse(HttpStatusCode.OK);
            }

            catch (Exception ex)
            {
                #region Retorna Json com erro

                var msg = (ex.InnerException != null ? ex.InnerException.Message : ex.Message) + "cd_prospect_erro: " + cd_prospect_erro;

                ExceptionHandler exceptionHandler = new ExceptionHandler(msg, "An erro has occurred.", ex.GetType().ToString(), ex.StackTrace);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, exceptionHandler);
                return response;

                #endregion

            }
        }

        [HttpComponentesAuthorize(Roles = "pros")]
        public HttpResponseMessage getProspectAlunoFKSearch(string nome, bool inicio, string email, string telefone, int tipoPesquisa, bool vinculoFollowUp)
        {
            try
            {
                
                List<Object> retorno = new List<object>();
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                if (tipoPesquisa == (int)ProspectSearchUI.TipoPesquisaEnum.PROSPECT)
                {
                    ProspectDataAccess.TipoConsultaEnum tipo = ProspectDataAccess.TipoConsultaEnum.HAS_PROSPECT_ATIVO;
                    if (vinculoFollowUp)
                        tipo = ProspectDataAccess.TipoConsultaEnum.HAS_PROSPECT_FOLLOWUP;
                    ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                    retorno = Business.getProspectFKSearch(parametros, cdEscola, nome, inicio, email, telefone, tipo).ToList<Object>();
                }
                else
                {
                    AlunoDataAccess.TipoConsultaAlunoEnum tipo = AlunoDataAccess.TipoConsultaAlunoEnum.ALUNO;
                    if (vinculoFollowUp)
                        tipo = AlunoDataAccess.TipoConsultaAlunoEnum.HAS_ALUNO_FOLLOWUP;
                    IAlunoBusiness BusinessAluno = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
                    retorno = BusinessAluno.getAlunoSearchFollowUp(parametros, nome, inicio, cdEscola, email, telefone, tipo).ToList<Object>();
                }
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch(Exception ex) {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "pros.e")]
        [HttpComponentesAuthorize(Roles = "prod")]
        [HttpComponentesAuthorize(Roles = "mtnm")]
        [HttpComponentesAuthorize(Roles = "oper")]
        [HttpComponentesAuthorize(Roles = "mid")]
        public HttpResponseMessage PostDeleteProspect(List<Prospect> prospects)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cd_escola = this.ComponentesUser.CodEmpresa.Value;
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                bool deletou = Business.deleteAllProspect(prospects, cd_escola);
                retorno.retorno = deletou;
                if (!deletou)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                else
                    retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;

            }
            
            catch (FinanceiroBusinessException exe)
            {
                logger.Error(exe);
                if (exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_TITULO_ENVIADO_CNAB)
                    return gerarLogException(exe.Message, retorno, logger, exe);
                else
                    return gerarLogException(Messages.msgNotUpReg, retorno, logger, exe); 
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotDeletedReg, retorno, logger, ex);
            }
        }

        //Traz os registros pela descrição da Entidade
        [HttpComponentesAuthorize(Roles = "pros")]
        [Obsolete]
        public HttpResponseMessage getExistsProspectEmail(String email, int cdProspect)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                var cd_escola = this.ComponentesUser.CodEmpresa.Value;
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var prospect = Business.getExistsProspectEmail(email, cd_escola, cdProspect);
                if (prospect != null)
                {
                    retorno.retorno = prospect;
                    retorno.AddMensagem(string.Format(Messages.msgExistsEmailProspect, prospect.no_pessoa), null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                }
                else
                {
                    IAlunoBusiness BusinessAluno = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
                    var aluno = BusinessAluno.verificarAlunoExistEmail(email,cd_escola, 0);
                    if (aluno != null)
                    {
                        retorno.retorno = aluno;
                        retorno.AddMensagem(string.Format(Messages.msgExistsEmailAluno, aluno.no_pessoa), null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                    }
                    else
                    {
                        var pessoaFisica = Business.verificarPessoaFisicaEmail(email);
                        if (pessoaFisica != null)
                        {
                            retorno.retorno = pessoaFisica;
                            retorno.AddMensagem(string.Format(Messages.msgExistsEmailPessoa, pessoaFisica.no_pessoa), null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                        }
                    }
                }
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "pros")]
        public HttpResponseMessage getProspectFull(int cd_prospect)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                //Pega na sessão a escola logada
                int cd_pessoa_escola = this.ComponentesUser.CodEmpresa.Value;
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var prospectEdit = Business.getProspectForAluno(cd_prospect, cd_pessoa_escola);
                retorno.retorno = prospectEdit;
                if (prospectEdit.cd_prospect <= 0)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
            }
        }

        #endregion

        #region FollowUp

        [HttpComponentesAuthorize(Roles = "flup")]
        public HttpResponseMessage getFollowUpSearch(byte id_tipo_follow, int cd_usuario_org, int cd_usuario_destino, int cd_prospect,int cd_aluno, int cd_acao, int resolvido, int lido, bool data,
                                                     bool proximo_contato, string dtInicial, string dtFinal, bool id_usuario_adm)
        {
            try
            {
                int cd_usuario = (int)this.ComponentesUser.CodUsuario;
                bool usuarioMaster = this.ComponentesUser.IdMaster;
                DateTime? dtaInicial = dtInicial == null ? null : (DateTime?)Convert.ToDateTime(dtInicial);
                DateTime? dtaFinal = dtFinal == null ? null : (DateTime?)Convert.ToDateTime(dtFinal);
                int cd_escola = this.ComponentesUser.CodEmpresa.Value;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                IEnumerable<FollowUpSearchUI> retorno = Business.getFollowUpSearch(parametros, cd_escola, id_tipo_follow, cd_usuario_org, cd_usuario_destino,
                    cd_prospect, cd_acao, resolvido, lido, data, proximo_contato, dtaInicial, dtaFinal, id_usuario_adm, cd_usuario, cd_aluno, usuarioMaster);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "pros")]
        public HttpResponseMessage GetFollowUpProspect(int CdProspect)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cd_escola = this.ComponentesUser.CodEmpresa.Value;
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                IEnumerable<FollowUp> follow = Business.getFollowUpProspect(CdProspect, cd_escola);
                retorno.retorno = follow;
                if (follow == null)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, retorno);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }

        }

        [HttpComponentesAuthorize(Roles = "alu")]
        public HttpResponseMessage GetFollowUpAluno(int cdAluno)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                var cd_escola = this.ComponentesUser.CodEmpresa.Value;
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                IEnumerable<FollowUp> follow = Business.getFollowUpByAluno(cdAluno, cd_escola);
                retorno.retorno = follow;
                if (follow == null)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }

        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "flup")]
        public HttpResponseMessage componentesPesquisaFollowUp()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                var cd_usuario = this.ComponentesUser.CodUsuario;
                FollowUp resultados = new FollowUp();
                resultados.cd_usuario = cd_usuario;
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                resultados.acaoeFollowUp = Business.getAcaoFollowUp(AcaoFollowupDataAccess.TipoPesquisaAcaoEnum.HAS_ATIVO, 0).ToList();
                retorno.retorno = resultados;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "flup")]
        public HttpResponseMessage componentesNovoFollowUp()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                //var cd_usuario = this.ComponentesUser.CodUsuario;
                FollowUp resultados = new FollowUp();
                //resultados.cd_usuario = cd_usuario;
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                resultados.acaoeFollowUp = Business.getAcaoFollowUp(AcaoFollowupDataAccess.TipoPesquisaAcaoEnum.HAS_ATIVO, 0).ToList();
                retorno.retorno = resultados;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "flup")]
        [HttpGet]
        public HttpResponseMessage getUrlRelatorioFollowUp(string sort, int direction, byte id_tipo_follow, int cd_usuario_org, int cd_usuario_destino, int cd_prospect, int cd_aluno, int cd_acao, int 
            resolvido, int lido, bool data,bool proximo_contato, string dtInicial, string dtFinal, bool id_usuario_adm)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cd_pessoa_escola = this.ComponentesUser.CodEmpresa.Value;
                DateTime? dataIncial = dtInicial == null ? null : (DateTime?)Convert.ToDateTime(dtInicial);
                DateTime? dataFinal = dtFinal == null ? null : (DateTime?)Convert.ToDateTime(dtFinal);
                int cd_usuario = (int)this.ComponentesUser.CodUsuario;
                bool idMaster = this.ComponentesUser.IdMaster;
                var strParametrosSort = "@orientation=LANDSCAPE&@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@cdEmpresa=" + cd_pessoa_escola + "&@id_tipo_follow=" + id_tipo_follow + "&@cd_usuario_org=" + cd_usuario_org + "&@cd_usuario_destino=" +
                    cd_usuario_destino + "&@cd_prospect=" + cd_prospect + "&@cd_aluno=" + cd_aluno + "&@resolvido=" + resolvido + "&@lido=" + lido + "&@data=" + data + "&@cd_usuario_logado=" + cd_usuario +
                    "&@cd_acao=" + cd_acao + "&@proximo_contato=" + proximo_contato + "&@id_usuario_adm=" + id_usuario_adm  +"&@dtInicial=" + dtInicial + "&@dtFinal=" + dtFinal +
                    "&@usuario_login_master=" +idMaster+
                    "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=" + "Relatório de Follow-Up&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO +
                    "=" + (int)FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.FollowUp;

                string parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, parametrosCript);

                configureHeaderResponse(response, null);
                return response;

            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "flup.a")]
        public HttpResponseMessage getComponentesByFollowUpEdit(int cd_follow_up, int id_tipo_follow)
        {
            ReturnResult retorno = new ReturnResult();
            int cdEscola = (int)this.ComponentesUser.CodEmpresa;
            try
            {
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                FollowUp followUp = Business.getFollowEditView(cd_follow_up, cdEscola, id_tipo_follow);
                int cd_acao = 0;
                if (followUp.cd_acao_follow_up != null && followUp.cd_acao_follow_up > 0)
                    cd_acao = (int)followUp.cd_acao_follow_up;
                followUp.acaoeFollowUp = Business.getAcaoFollowUp(AcaoFollowupDataAccess.TipoPesquisaAcaoEnum.HAS_PESQ_FOLLOW_UP, cd_acao).ToList();
                retorno.retorno = followUp;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }


        [HttpComponentesAuthorize(Roles = "flup.i")]
        public HttpResponseMessage postInsertFollowUp(FollowUp followUp)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });
                followUp.cd_escola = (int)this.ComponentesUser.CodEmpresa;
                followUp.cd_usuario = (int)this.ComponentesUser.CodUsuario;
                followUp.dt_follow_up = DateTime.UtcNow;
                if (followUp.dt_proximo_contato != null)
                    followUp.dt_proximo_contato = ((DateTime)followUp.dt_proximo_contato).ToLocalTime().Date;

                var cadFollowUp = Business.addFollowUp(followUp);
                retorno.retorno = cadFollowUp;
                if (cadFollowUp.cd_follow_up <= 0)
                    retorno.AddMensagem(Messages.msgNotIncludReg, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, retorno);
                configureHeaderResponse(response, null);
                return response;
            }
            catch (SecretariaBusinessException exe)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, exe);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "flup.a")]
        public HttpResponseMessage postUpdateFollowUp(FollowUp followUp)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                if (!this.ModelState.IsValid)
                {
                    ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                    configuraBusiness(new List<IGenericBusiness>() { Business });
                    int cd_usuario = (int)this.ComponentesUser.CodUsuario;
                    bool usuarioMaster = this.ComponentesUser.IdMaster;
                    followUp.cd_escola = (int)this.ComponentesUser.CodEmpresa;
                    followUp.dt_follow_up = DateTime.UtcNow;
                    if (followUp.dt_proximo_contato != null)
                        followUp.dt_proximo_contato = ((DateTime)followUp.dt_proximo_contato).ToLocalTime().Date;

                    var cadFollowUp = Business.editFollowUp(followUp, cd_usuario, usuarioMaster);
                    retorno.retorno = cadFollowUp;

                    if (cadFollowUp == null || cadFollowUp.cd_follow_up <= 0)
                        retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                    else
                        retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                else
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, retorno);
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "flup.e")]
        public HttpResponseMessage postDeleteFollowUps(List<FollowUp> followUps)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                int cd_usuario = (int)this.ComponentesUser.CodUsuario;
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });
                bool delFollows = Business.deleteFollowUps(followUps.Select(x => x.cd_follow_up).ToList(), cd_usuario);
                retorno.retorno = delFollows;
                if (!delFollows)
                    retorno.AddMensagem(Messages.msgNotDeletedReg, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                else
                    retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (SecretariaBusinessException exe)
            {
                logger.Error(exe);
                if (exe.tipoErro == SecretariaBusinessException.TipoErro.ERRO_FOLLOW_UP_LIDO ||
                    exe.tipoErro == SecretariaBusinessException.TipoErro.ERRO_FOLLOW_UP_USER_ORIGEM_DIFERENTE ||
                    exe.tipoErro == SecretariaBusinessException.TipoErro.ERRO_FOLLOW_UP_DE_OUTRO_USUARIO)
                    return gerarLogException(exe.Message, retorno, logger, exe);
                else
                    return gerarLogException(Messages.msgNotUpReg, retorno, logger, exe);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "flup")]
        public HttpResponseMessage postMarcarFollowUpComoLido(FollowUp followUp)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });
                 if (followUp.cd_follow_up > 0 && followUp.id_tipo_follow > 0)
                 {
                     int cd_usuario = (int)this.ComponentesUser.CodUsuario;
                     bool delFollows = Business.marcarFollowUpComoLido(followUp, cd_usuario);
                     retorno.retorno = delFollows;
                     if (!delFollows)
                         retorno.AddMensagem(Messages.msgNotDeletedReg, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                     else
                         retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                 }
                 else
                     retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (SecretariaBusinessException exe)
            {
                logger.Error(exe);
                if (exe.tipoErro == SecretariaBusinessException.TipoErro.ERRO_FOLLOW_UP_LIDO ||
                    exe.tipoErro == SecretariaBusinessException.TipoErro.ERRO_FOLLOW_UP_USER_ORIGEM_DIFERENTE)
                    return gerarLogException(exe.Message, retorno, logger, exe);
                else
                    return gerarLogException(Messages.msgNotUpReg, retorno, logger, exe);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "rpfolup")]
        public HttpResponseMessage GetUrlRelFollowUp(byte id_tipo_follow, int cd_usuario_org, string no_usuario_org, int resolvido, int lido, string dtaIni, string dtaFinal)
        {
            ReturnResult retorno = new ReturnResult();
            ISecretariaBusiness secretariaBiz = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
            base.configuraBusiness(new List<IGenericBusiness>() { secretariaBiz });
            int cdEscola = this.ComponentesUser.CodEmpresa.Value;
            try
            {
                // Pega os parâmetros do usuário para criar a url do relatório:
                DateTime? dt_inicio = null;
                if (dtaIni != null && dtaIni != "")
                    dt_inicio = Convert.ToDateTime(dtaIni);

                DateTime? dt_fim = null;
                if (dtaFinal != null && dtaIni != "")
                    dt_fim = Convert.ToDateTime(dtaFinal);

                if (string.IsNullOrEmpty(no_usuario_org))
                {
                    no_usuario_org = "Todos";
                }

                string PTitulo = "Relatório de Follow-Up";
                
                string parametros = "@orientation=LANDSCAPE&@id_tipo_follow=" + id_tipo_follow + "&@cd_usuario_org=" + cd_usuario_org + "&@no_usuario_org=" + no_usuario_org + "&@resolvido=" + resolvido + "&@lido=" + lido + "&@cdEscola=" +
                    cdEscola + "&@dtaIni=" + dt_inicio + "&@dtaFinal=" + dt_fim + "&" +
                    Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=" + PTitulo + "&" +
                    Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + (int)FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.FollowUpRel;
                string parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, parametrosCript);
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        #endregion

        #region Horário

        [HttpPost]
        [HttpComponentesAuthorize(Roles = "tur")]
        public HttpResponseMessage getHorariosOcupadosSala(Turma turma)
        {
            ReturnResult retorno = new ReturnResult();
             var cd_escola = this.ComponentesUser.CodEmpresa.Value;
             //DateTime dtaInicial = (DateTime)Convert.ToDateTime(dtInicio);
            try
            {
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                List<Horario> horariosOcupadosSalaTurma = Business.getHorarioOcupadosForSala(turma, cd_escola, HorarioDataAccess.TipoConsultaHorario.HAS_HORARIO_SALA_OCUPADO_TURMA).ToList();
                List<Horario> horariosOcupadosSalaAtivExt = Business.getHorarioOcupadosForSala(turma, cd_escola, HorarioDataAccess.TipoConsultaHorario.HAS_HORARIO_SALA_ATIVIDA_EXT).ToList();
                horariosOcupadosSalaTurma=  horariosOcupadosSalaTurma.Union(horariosOcupadosSalaAtivExt).ToList();
                retorno.retorno = horariosOcupadosSalaTurma;
                if (horariosOcupadosSalaTurma == null)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }

        }
        //to do Deivid
        [HttpComponentesAuthorize(Roles = "tur")]
        public HttpResponseMessage getHorariosProfessorTurma(Turma turma)
        {
            ReturnResult retorno = new ReturnResult();
            var cd_escola = this.ComponentesUser.CodEmpresa.Value;
            //int[] cdProf = turma.cd_professor != null ? Array.ConvertAll<string, int>(cd_prof.Split(','), int.Parse) : new int[0];
            int[] cdProf = new int[0];
            if (turma.cd_professor != null) cdProf[0] = (int)turma.cd_professor;
            TipoHorarios tipohorarios = new TipoHorarios();

            try
            {
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                tipohorarios.horariosOcupProf = Business.getHorarioOcupadosForTurma(cd_escola, turma.cd_turma, cdProf, turma.cd_turma, turma.cd_duracao, (int)turma.cd_curso, turma.dt_inicio_aula, turma.dt_final_aula, HorarioDataAccess.TipoConsultaHorario.HAS_HORARIO_PROF_OCUPADO_TURMA).ToList();
                List<Horario> hOcupadosProfAtivExt = Business.getHorarioOcupadosForTurma(cd_escola, turma.cd_turma, cdProf, 0, turma.cd_duracao, (int)turma.cd_curso, turma.dt_inicio_aula, turma.dt_final_aula, HorarioDataAccess.TipoConsultaHorario.HAS_HORARIO_PROF_OCUPADO_ATIV_EXTRA).ToList();
                tipohorarios.horariosOcupProf = tipohorarios.horariosOcupProf.Union(hOcupadosProfAtivExt).ToList();
               // Horarios = horariosOcupProf.Union(hOcupadosProfAtivExt).Union(horariosDispoProf).ToList();
                retorno.retorno = tipohorarios;
                if (tipohorarios == null)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }

        }

        #endregion

        #region XML notas

        // GET api/<controller>/1
        [HttpGet]
        [HttpComponentesAuthorize(Roles = "gerxml")]
        public HttpResponseMessage getListaXmlGerados([FromUri] XmlSearchUI notatualizaUI)
        {
            try
            {
                ISecretariaBusiness NotasBiz = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());

                var notasG = NotasBiz.getListaXmlGerados(parametros, notatualizaUI);

                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, notasG);
                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage atualizaResolverNaoResolver(NotatualizaUI notatualizaUI) 
        {
            ReturnResult retorno = new ReturnResult();
            try 
            {
                ISecretariaBusiness atualizaXMLBiz = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                List<int> setAtualizaXML = atualizaXMLBiz.setAtualizaXML(notatualizaUI.cds_xml);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, setAtualizaXML);
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }


        // chama a procedure sp_dir_xml ao abrir a tela
        [HttpGet]
        public HttpResponseMessage abrirGerarXML()
        {
            ReturnResult retorno = new ReturnResult();
            ISecretariaBusiness BusinessSecretaria = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
            try
            {
                int cd_usuario = (int)this.ComponentesUser.CodUsuario;
                List<ImportacaoXML> listImportacaoXml = null;
                HttpResponseMessage response = null;
                int ret = BusinessSecretaria.abrirGerarMXL(cd_usuario);

                if (ret == 0)
                {
                    listImportacaoXml = BusinessSecretaria.buscarGerarXML(cd_usuario).ToList();
                   
                } else if (ret == 1){
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, JsonConvert.SerializeObject(retorno));
                    configureHeaderResponse(response, null);
                    return response;
                }
                response = Request.CreateResponse(HttpStatusCode.OK, listImportacaoXml);
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                var msg = ex.InnerException.Message;

                FundacaoFisk.SGF.Utils.ExceptionHandler exceptionHandler = new FundacaoFisk.SGF.Utils.ExceptionHandler(msg, "An erro has occurred.", ex.GetType().ToString(), ex.StackTrace);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(msg));
                return response;

            }
        }

        // chama a procedure sp_import_xml ao clicar no botão "Gerar"
        [HttpPost]
        public HttpResponseMessage postGerarXmlProc()
        {
            ReturnResult retorno = new ReturnResult();
            ISecretariaBusiness BusinessSec = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
            try
            {
                int cd_usuario = (int)this.ComponentesUser.CodUsuario;
                List<ImportacaoXML> listImportacaoXml = null;
                HttpResponseMessage response = null;
                int ret = BusinessSec.postGerarXmlProc(cd_usuario);

                if (ret == 0)
                {
                    listImportacaoXml = BusinessSec.buscarGerarXML(cd_usuario).ToList();
                }
                response = Request.CreateResponse(HttpStatusCode.OK, listImportacaoXml);
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                var msg = Messages.msgRegBuscError;
                if (!String.IsNullOrEmpty(ex.InnerException.Message))
                    msg = ex.InnerException.Message;
                return gerarLogException(msg, retorno, logger, ex);
            }
        }

        #endregion

        #region Matrícula


        [HttpComponentesAuthorize(Roles = "rpttm")]
        public HttpResponseMessage getVerificarNroContrato(int nm_contrato)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                int cd_empresa = this.ComponentesUser.CodEmpresa.Value;

                IMatriculaBusiness BusinessMatricula = (IMatriculaBusiness)base.instanciarBusiness<IMatriculaBusiness>();
                if (!BusinessMatricula.getVerificarNroContrato(cd_empresa, nm_contrato))
                    retorno.AddMensagem(Messages.msgErrorSemVinculoNotaMaterial, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                
                return Request.CreateResponse(HttpStatusCode.OK, retorno);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "alu")]
        public HttpResponseMessage getMatriculaAluno(int cd_aluno)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;

                IMatriculaBusiness BusinessMatricula = (IMatriculaBusiness)base.instanciarBusiness<IMatriculaBusiness>();
                retorno.retorno = BusinessMatricula.getMatriculaAluno(cd_aluno, cdEscola);

                return Request.CreateResponse(HttpStatusCode.OK, retorno);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "mat")]
        [HttpComponentesAuthorize(Roles = "alu")]
        [HttpComponentesAuthorize(Roles = "tur")]
        public HttpResponseMessage getMatriculaSearch(string descAluno, string descTurma, bool inicio, bool semTurma,
                                                           int situacaoTurma, int nmContrato, int tipo, string dtaInicio, string dtaFim,
                                                           bool filtraMat, bool filtraDtaInicio, bool filtraDtaFim, bool renegocia,
                                                            bool transf, bool retornoEsc, int? cdNomeContrato, int nm_matricula, int? cdAnoEscolar, 
                                                            int? cdContratoAnterior, byte tipoC, int status, int vinculado)
        {
            try
            {
                DateTime? dtInicio = dtaInicio == null || dtaInicio == "null" || dtaInicio == "" ? null : (DateTime?)Convert.ToDateTime(dtaInicio);
                DateTime? dtFim = dtaFim == null || dtaFim == "null" || dtaFim == "" ? null : (DateTime?)Convert.ToDateTime(dtaFim);
                if (descAluno == null)
                    descAluno = String.Empty;
                if (descTurma == null)
                    descTurma = String.Empty;
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;

                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());

                IMatriculaBusiness BusinessMatricula = (IMatriculaBusiness)base.instanciarBusiness<IMatriculaBusiness>();
                var retorno = BusinessMatricula.getMatriculaSearch(parametros, descAluno, descTurma, inicio, semTurma, situacaoTurma, nmContrato, tipo,
                                                                     dtInicio, dtFim, filtraMat, filtraDtaInicio, filtraDtaFim, cdEscola, renegocia,
                                                                     transf, retornoEsc, cdNomeContrato, nm_matricula, cdAnoEscolar, cdContratoAnterior, tipoC, getStatus(status), vinculado);
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
        public HttpResponseMessage getContratosSemTurmaByAlunoSearch(int cd_aluno, bool semTurma, int situacaoTurma, int nmContrato, int tipo, byte tipoC, int status)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;

                IMatriculaBusiness BusinessMatricula = (IMatriculaBusiness)base.instanciarBusiness<IMatriculaBusiness>();
                var result = BusinessMatricula.getContratosSemTurmaByAlunoSearch(cd_aluno,  semTurma, situacaoTurma, nmContrato, tipo, cdEscola, tipoC, getStatus(status));
                retorno.retorno = result;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "mat")]
        [HttpComponentesAuthorize(Roles = "alu")]
        [HttpComponentesAuthorize(Roles = "tur")]

        
        public HttpResponseMessage GeturlrelatorioMatricula(string sort, int direction, string descAluno, string descTurma, bool inicio, bool semTurma,
                                                           int situacaoTurma, int nmContrato, int tipo, string dtaInicio, string dtaFim,
                                                           bool filtraMat, bool filtraDtaInicio, bool filtraDtaFim, bool renegocia,
                                                            bool transf, bool retornoEsc, int? cdNomeContrato, int nm_matricula, int? cd_ano_escolar, 
                                                           int? cdContratoAnterior, byte tipoC, int status, int vinculado)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                // Pega os parâmetros do usuário para criar a url do relatório:
                var strParametrosSort = "@orientation=LANDSCAPE&@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@descAluno=" + descAluno + "&@descTurma=" + descTurma + "&@inicio=" + inicio + "&@semTurma=" + semTurma +
                                  "&@situacaoTurma=" + situacaoTurma + "&@nmContrato=" + nmContrato + "&@nm_matricula=" + nm_matricula + "&@tipo=" + tipo + "&@dtaInicio=" + dtaInicio + "&@dtaFim=" + dtaFim + 
                                  "&@filtraMat=" + filtraMat + "&@filtraDtaInicio=" + filtraDtaInicio + "&@filtraDtaFim=" + filtraDtaFim + "&@cdEscola=" + cdEscola
                                  + "&@renegocia=" + renegocia + "&@transf=" + transf + "&@retornoEsc=" + retornoEsc + "&@cdNomeContrato=" + cdNomeContrato + "&@cd_ano_escolar=" + cd_ano_escolar + "&@cdContratoAnterior=" + cdContratoAnterior + "&@tipoC=" + tipoC +
                                  "&@status=" + status + "&@vinculado=" + vinculado +
                    "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=Relatório de Matrícula&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.MatriculaSearch;
                string parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, parametrosCript);

                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }


        //[HttpComponentesAuthorize(Roles = "mtb")]
        public HttpResponseMessage GeturlrelatorioAtividadeExtra([FromUri] AtividadeExtraReportUI atividadeExtraReportUI)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                IFuncionarioBusiness BusinessFuncionario = (IFuncionarioBusiness)base.instanciarBusiness<IFuncionarioBusiness>();
                //int cd_funcionario  = 0;
               /* if (atividadeExtraReportUI.cd_funcionario != null && atividadeExtraReportUI.cd_funcionario > 0)
                {
                    cd_funcionario = BusinessFuncionario.getFuncionarioByIdPessoa(atividadeExtraReportUI.cd_funcionario != null ? (int)atividadeExtraReportUI.cd_funcionario : 0);
                }*/
                    // Pega os parâmetros do usuário para criar a url do relatório:
                    int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                StringBuilder parametros = new StringBuilder();
                parametros.Append("@cd_escola=").Append(cdEscola)
                          .Append("&@cd_atividade_extra=").Append(atividadeExtraReportUI.cd_atividade_extra)
                          .Append("&@cd_produto=").Append(atividadeExtraReportUI.cd_produto != null ? atividadeExtraReportUI.cd_produto : 0)
                          .Append("&@cd_curso=").Append(atividadeExtraReportUI.cd_curso != null ? atividadeExtraReportUI.cd_curso : 0)
                          .Append("&@cd_aluno=").Append(atividadeExtraReportUI.cd_aluno != null ? atividadeExtraReportUI.cd_aluno : 0)
                          .Append("&@cd_funcionario=").Append(atividadeExtraReportUI.cd_funcionario > 0 ? atividadeExtraReportUI.cd_funcionario: 0)
                          .Append("&@id_participacao=").Append(atividadeExtraReportUI.id_participacao)
                          .Append("&@esconde_obs=").Append(atividadeExtraReportUI.esconde_obs)
                          .Append("&@dta_ini=").Append(atividadeExtraReportUI.dta_ini)
                          .Append("&@dta_fim=").Append(atividadeExtraReportUI.dta_fim)
                          .Append("&@id_lancada=").Append(atividadeExtraReportUI.id_lancada)
                          .Append("&").Append(Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO).Append("=Relatório de Atividade Extra")
                          .Append("&").Append(Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO).Append("=").Append(FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.rtpAtividadeExtra);

                string parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros.ToString(), System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, parametrosCript);

                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception exe)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, exe);
            }
        }

        public HttpResponseMessage GeturlrelatorioAulaReposicao([FromUri] AulaReposicaoReportUI aulaReposicaoReportUI)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                IFuncionarioBusiness BusinessFuncionario = (IFuncionarioBusiness)base.instanciarBusiness<IFuncionarioBusiness>();
                // Pega os parâmetros do usuário para criar a url do relatório:
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                StringBuilder parametros = new StringBuilder();
                parametros.Append("@cd_escola=").Append(cdEscola)
                          .Append("&@cd_turma=").Append(aulaReposicaoReportUI.cd_turma != null ? aulaReposicaoReportUI.cd_turma : 0)
                          .Append("&@cd_aluno=").Append(aulaReposicaoReportUI.cd_aluno != null ? aulaReposicaoReportUI.cd_aluno : 0)
                          .Append("&@cd_funcionario=").Append(aulaReposicaoReportUI.cd_funcionario > 0 ? aulaReposicaoReportUI.cd_funcionario : 0)
                          .Append("&@id_participacao=").Append(aulaReposicaoReportUI.id_participacao)
                          .Append("&@esconde_obs=").Append(aulaReposicaoReportUI.esconde_obs)
                          .Append("&@dta_ini=").Append(aulaReposicaoReportUI.dta_ini)
                          .Append("&@dta_fim=").Append(aulaReposicaoReportUI.dta_fim)
                          .Append("&").Append(Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO).Append("=Relatório de Aulas de Reposição")
                          .Append("&").Append(Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO).Append("=").Append(FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.rtpAulaReposicao);

                string parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros.ToString(), System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, parametrosCript);

                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception exe)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, exe);
            }
        }

        //[HttpComponentesAuthorize(Roles = "rphisal")]
        public HttpResponseMessage GeturlrelatorioHistoricoAluno([FromUri] HistoricoAlunoReportUI historicoAlunoReportUI)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {

                // Pega os parâmetros do usuário para criar a url do relatório:
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                StringBuilder parametros = new StringBuilder();
                parametros.Append("PEscola=").Append(cdEscola)
                          .Append("&@cd_aluno=").Append(historicoAlunoReportUI.cd_aluno)
                          .Append("&no_aluno=").Append(historicoAlunoReportUI.no_aluno)
                          .Append("&produtos=").Append(historicoAlunoReportUI.produtos)
                          .Append("&PTurmaAval=").Append(historicoAlunoReportUI.turmaAvaliacao)
                          .Append("&PEstagioAval=").Append(historicoAlunoReportUI.estagioAvaliacao)
                          .Append("&PTitulos=").Append(historicoAlunoReportUI.statusTitulo)
                          .Append("&Pmostrarestagio=").Append(historicoAlunoReportUI.mostrarEstagio)
                          .Append("&Pmostraratividade=").Append(historicoAlunoReportUI.mostrarAtividade)
                          .Append("&Pmostrarobs=").Append(historicoAlunoReportUI.mostrarObservacao)
                          .Append("&Pmostrarfollow=").Append(historicoAlunoReportUI.mostrarFollow)
                          .Append("&Pmostraritem=").Append(historicoAlunoReportUI.mostrarItem)
                          .Append("&").Append(Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO).Append("=Relatório de Histórico do Aluno")
                          .Append("&").Append(Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO).Append("=").Append(FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.rtpHistoricoAluno);

                string parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros.ToString(), System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, parametrosCript);

                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception exe)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, exe);
            }
        }

        //public HttpResponseMessage GetArquivoHistoricoAluno([FromUri] HistoricoAlunoReportUI historicoAlunoReportUI)
        //{
        //    ReturnResult retorno = new ReturnResult();
        //    ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();

        //    try
        //    {

        //        // Pega os parâmetros do usuário para criar a url do relatório:
        //        int cdEscola = this.ComponentesUser.CodEmpresa.Value;

        //        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, parametrosCript);

        //        configureHeaderResponse(response, null);
        //        return response;
        //    }
        //    catch (Exception exe)
        //    {
        //        return gerarLogException(Messages.msgRegBuscError, retorno, logger, exe);
        //    }
        //}



        //[HttpComponentesAuthorize(Roles = "rphisal")]
        public HttpResponseMessage GeturlrelatorioFaixaEtaria([FromUri] FaixaEtariaReportUI faixaEtariaReportUI)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {

                // Pega os parâmetros do usuário para criar a url do relatório:
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                StringBuilder parametros = new StringBuilder();
                parametros.Append("@cd_escola=").Append(cdEscola)
                          .Append("&@tipo=").Append(faixaEtariaReportUI.tipo)
                          .Append("&@idade=").Append(faixaEtariaReportUI.idade)
                          .Append("&@idade_max=").Append(faixaEtariaReportUI.idade_max)
                          .Append("&@cd_turma=").Append(faixaEtariaReportUI.cd_turma)
                          .Append("&@no_turma=").Append(faixaEtariaReportUI.no_turma)
                          .Append("&").Append(Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO).Append("=Relatório de faixa etária");
                          //.Append("&").Append(Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO).Append("=").Append(FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.rtpFaixaEtaria);

                string parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros.ToString(), System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, parametrosCript);

                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception exe)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, exe);
            }
        }

        // NOVO DELETAR MATRICULA COM A PROCEDURE sp_excluir_contrato 
        [HttpPost]
        public HttpResponseMessage postDeleteMatricula(DeletaMatriculaUI deletaMatriculaUI)
        {
            ReturnResult retorno = new ReturnResult();
            IMatriculaBusiness BusinessMatricula = (IMatriculaBusiness)base.instanciarBusiness<IMatriculaBusiness>();
            ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
            
            try
            {   
                if (deletaMatriculaUI == null)
                {
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                    return response;
                }

                //#region chamaProcedure postDeleteMatricula
                var cd_usuario = this.ComponentesUser.CodUsuario;
                var fuso = this.ComponentesUser.IdFusoHorario;
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                string nomeArquivo = Business.getNomeContratoDigitalizadoByEscolaAndCdContrato(cdEscola, ((deletaMatriculaUI.cd_contrato != null) ? (int)deletaMatriculaUI.cd_contrato : 0));
                string ret = BusinessMatricula.postDeleteMatricula(deletaMatriculaUI.cd_contrato, cd_usuario, fuso);
                string caminho_relatorios = ConfigurationManager.AppSettings["caminhoUploads"];
                if (ret != null)
                {
                    //Se o contrato deletado tiver arquivo cadastrado - deleta o arquivo
                    if (!String.IsNullOrEmpty(nomeArquivo))
                    {
                        string pathArquivoDigitalizado = "";
                        pathArquivoDigitalizado = caminho_relatorios + "\\ContratosDigitalizados\\" + cdEscola + "\\" + nomeArquivo;

                        if (System.IO.File.Exists(pathArquivoDigitalizado))
                            System.IO.File.Delete(pathArquivoDigitalizado);
                    }

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(ret));
                    return response;
                }

                return Request.CreateResponse(HttpStatusCode.OK);

            }
            catch (Exception ex)
            {
                var msg = ex.InnerException.Message;

                FundacaoFisk.SGF.Utils.ExceptionHandler exceptionHandler = new FundacaoFisk.SGF.Utils.ExceptionHandler(msg, "An erro has occurred.", ex.GetType().ToString(), ex.StackTrace);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(msg));
                return response;
            }
        }
        
        [HttpComponentesAuthorize(Roles = "mat.a")]
        public HttpResponseMessage PostAlterarMatricula(Contrato contrato)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {

                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                int cdusuario = (int)this.ComponentesUser.CodUsuario;
                int fusoHorario = (int)this.ComponentesUser.IdFusoHorario;
                string caminho_relatorios = ConfigurationManager.AppSettings["caminhoUploads"];
                //string caminho_relatorios = HttpContext.Current.Server.MapPath("caminhoUploads");
                string pathContratosEscola = caminho_relatorios + "\\ContratosDigitalizados";
                if (!String.IsNullOrEmpty(contrato.nm_arquivo_digitalizado))
                    if (contrato.nm_arquivo_digitalizado.ToLower().Substring(contrato.nm_arquivo_digitalizado.Length - 4) != ".png" &&
                        contrato.nm_arquivo_digitalizado.ToLower().Substring(contrato.nm_arquivo_digitalizado.Length - 4) != ".pdf" &&
                        contrato.nm_arquivo_digitalizado.ToLower().Substring(contrato.nm_arquivo_digitalizado.Length - 4) != ".jpg" &&
                        contrato.nm_arquivo_digitalizado.ToLower().Substring(contrato.nm_arquivo_digitalizado.Length - 5) != ".jpeg" &&
                        contrato.nm_arquivo_digitalizado.ToLower().Substring(contrato.nm_arquivo_digitalizado.Length - 5) != ".dotx")
                        throw new MatriculaBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroExtesaoContratoDigitalizadoNaoValida, null,
                            MatriculaBusinessException.TipoErro.ERRO_EXTENSAO_CONTRATRO_DIGITALIZADO_NAO_VALIDA, false);

                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                IMatriculaBusiness BusinessMatricula = (IMatriculaBusiness)base.instanciarBusiness<IMatriculaBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business, BusinessMatricula });
                contrato.cd_pessoa_escola = this.ComponentesUser.CodEmpresa.Value;
                contrato.cd_usuario = (int)this.ComponentesUser.CodUsuario;
                var cont = BusinessMatricula.editContrato(contrato, pathContratosEscola, cdusuario, fusoHorario);
                
                retorno.retorno = cont;

                if (cont.cd_contrato <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "mat")]
        [HttpComponentesAuthorize(Roles = "alu")]
        [HttpComponentesAuthorize(Roles = "tur")]
        public HttpResponseMessage getMatriculaByTurmaAluno(int cdTurma, int cdAluno)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IMatriculaBusiness BusinessMatricula = (IMatriculaBusiness)base.instanciarBusiness<IMatriculaBusiness>();
                Contrato cont = BusinessMatricula.getMatriculaByTurmaAluno(cdTurma, cdAluno);
                retorno.retorno = cont;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));

                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "mat")]
        [HttpComponentesAuthorize(Roles = "alu")]
        [HttpComponentesAuthorize(Roles = "tur")]
        public HttpResponseMessage getExisteMatriculaByProduto(int produto, int cdAluno, string dtIniAula, int curso, bool id_turma_ppt, int cd_contrato, string dtFimAula, int cd_duracao)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                DateTime? dt_inicio_aula = null;
                if (!string.IsNullOrEmpty(dtIniAula))
                    dt_inicio_aula = DateTime.Parse(dtIniAula);
                DateTime? dt_final_aula = null;
                if (!string.IsNullOrEmpty(dtFimAula))
                    dt_final_aula = DateTime.Parse(dtFimAula);
                IMatriculaBusiness BusinessMatricula = (IMatriculaBusiness)base.instanciarBusiness<IMatriculaBusiness>();
                bool cont = BusinessMatricula.existeMatriculaByProduto(produto, cdAluno, dt_inicio_aula, curso, id_turma_ppt, cd_contrato, dt_final_aula, cd_duracao);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, cont);

                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "mat")]
        public HttpResponseMessage getMatriculasAluno(int cd_aluno)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;

                IMatriculaBusiness BusinessMatricula = (IMatriculaBusiness)base.instanciarBusiness<IMatriculaBusiness>();
                retorno.retorno = BusinessMatricula.getMatriculasAluno(cd_aluno, cdEscola);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }


        [HttpComponentesAuthorize(Roles = "mat")]
        public HttpResponseMessage getMatriculaCnabBoleto(int cd_contrato, byte tipo)
        {

            ReturnResult retorno = new ReturnResult();

            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;

                IMatriculaBusiness BusinessMatricula = (IMatriculaBusiness)base.instanciarBusiness<IMatriculaBusiness>();
                retorno.retorno = BusinessMatricula.getMatriculaCnabBoleto(cdEscola, cd_contrato, tipo);

                return Request.CreateResponse(HttpStatusCode.OK, retorno);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "rpMat")]
        [HttpGet]
        public HttpResponseMessage getUrlRelatorioMat(int cd_turma, int cd_aluno, int tipo, string situacaoAlunoTurma, bool semTurma, bool tranferencia, bool retorno, int situacaoContrato, string dtIni, string dtFim, int cdAtendente, bool bolsaCem, bool ckContratoDigitalizado, int cd_produto, string no_produto, bool exibirEnderecos, int vinculado)
        {
            ReturnResult ret = new ReturnResult();
            try
            {
                int cd_pessoa_escola = this.ComponentesUser.CodEmpresa.Value;
                DateTime? dataIncial = dtIni == null ? null : (DateTime?)Convert.ToDateTime(dtIni);
                DateTime? dataFinal = dtFim == null ? null : (DateTime?)Convert.ToDateTime(dtFim);
                // Pega os parâmetros do usuário para criar a url do relatório:
                string parametros = "@cd_empresa=" + cd_pessoa_escola + "&@cd_turma=" + cd_turma + "&@cd_aluno=" + cd_aluno + "&@tipo=" + tipo + "&@situacaoAlunoTurma=" + situacaoAlunoTurma +
                                    "&@semTurma=" + semTurma + "&@tranferencia=" + tranferencia + "&@retorno=" + retorno + "&@situacaoContrato=" + situacaoContrato + "&@dtIni=" + dtIni +
                                    "&@dtFim=" + dtFim + "&@cdAtendente=" + cdAtendente + "&@bolsaCem=" + bolsaCem + "&@ckContratoDigitalizado=" + ckContratoDigitalizado + "&@cd_produto=" + cd_produto + "&@no_produto=" + no_produto + "&@exibirEnderecos=" + exibirEnderecos
                                    + "&@vinculado=" + vinculado;

                string parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, parametrosCript);

                configureHeaderResponse(response, null);
                return response;

            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, ret, logger, ex);
            }
        }

        public HttpResponseMessage getUrlRelatorioMatOut(int qt_max, string dtIni, string dtFim, int cd_produto, string no_produto, bool todasescolas)
        {
            ReturnResult ret = new ReturnResult();
            try
            {
                int cd_pessoa_escola = todasescolas ? 0 :this.ComponentesUser.CodEmpresa.Value;
                DateTime? dataIncial = dtIni == null ? null : (DateTime?)Convert.ToDateTime(dtIni);
                DateTime? dataFinal = dtFim == null ? null : (DateTime?)Convert.ToDateTime(dtFim);
                // Pega os parâmetros do usuário para criar a url do relatório:
                string parametros = "@cd_empresa=" + cd_pessoa_escola + "&@qt_max=" + qt_max +
                                    "&@dtIni=" + dtIni + "&@dtFim=" + dtFim + "&@cd_produto=" + cd_produto + "&@no_produto=" + no_produto; 

                string parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, parametrosCript);

                configureHeaderResponse(response, null);
                return response;

            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, ret, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "mat")]
        public HttpResponseMessage getSaldoMatricula(int cd_contrato, decimal pc_bolsa)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                IMatriculaBusiness BusinessMatricula = (IMatriculaBusiness)base.instanciarBusiness<IMatriculaBusiness>();
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                retorno.retorno = BusinessMatricula.getSaldoMatricula(cd_contrato, cdEscola, pc_bolsa);

                return Request.CreateResponse(HttpStatusCode.OK, retorno);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage PostGerarVenda(Contrato contrato)
        {
            ReturnResult retorno = new ReturnResult();
            IMatriculaBusiness BusinessMatricula = (IMatriculaBusiness)base.instanciarBusiness<IMatriculaBusiness>();
            try
            {
                int cd_usuario = (int)this.ComponentesUser.CodUsuario;
                int fusoHorario = this.ComponentesUser.IdFusoHorario;

                string ret = BusinessMatricula.postGerarVenda(contrato.cd_contrato, contrato.cd_curso_atual, cd_usuario, fusoHorario, (bool)contrato.id_venda_futura, 0);

                if (ret != null)
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(ret));
                    return response;
                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }

            catch (Exception ex)
            {
                var msg = ex.InnerException.Message;

//                FundacaoFisk.SGF.Utils.ExceptionHandler exceptionHandler = new FundacaoFisk.SGF.Utils.ExceptionHandler(msg, "An erro has occurred.", ex.GetType().ToString(), ex.StackTrace);
                return gerarLogException(msg, retorno, logger, ex);
 //               HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(msg));
 //               return response;
            }
        }


        [HttpPost]
        public HttpResponseMessage postVincularVendaMaterial(VincularVendaMaterialParamsUI vincularVendaMaterialParams)
        {
            ReturnResult retorno = new ReturnResult();
            IMatriculaBusiness BusinessMatricula = (IMatriculaBusiness)base.instanciarBusiness<IMatriculaBusiness>();
            try
            {
                int cd_usuario = (int)this.ComponentesUser.CodUsuario;
                int fusoHorario = this.ComponentesUser.IdFusoHorario;

                string ret = BusinessMatricula.postVincularVendaMaterial(vincularVendaMaterialParams.cd_movimento, vincularVendaMaterialParams.cd_turma_origem, vincularVendaMaterialParams.cd_contrato);

                if (ret != null)
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(ret));
                    return response;
                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }

            catch (Exception ex)
            {
                var msg = ex.InnerException.Message;

                //                FundacaoFisk.SGF.Utils.ExceptionHandler exceptionHandler = new FundacaoFisk.SGF.Utils.ExceptionHandler(msg, "An erro has occurred.", ex.GetType().ToString(), ex.StackTrace);
                return gerarLogException(msg, retorno, logger, ex);
                //               HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(msg));
                //               return response;
            }
        }
        #endregion

        #region Nome Contrato

        [HttpComponentesAuthorize(Roles = "nmCt")]
        public HttpResponseMessage getSearchNomeContrato(string desc, string layout, bool inicio,int status)
        {
            try
            {

                if (desc == null)
                    desc = String.Empty;
                if (layout == null)
                    layout = String.Empty;
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                int cdusuario = this.ComponentesUser.CodUsuario;

                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());

                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var retorno = Business.getSearchNoContrato(parametros, desc, layout, inicio, getStatus(status), cdEscola, cdusuario);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "nmCt.i")]
        public HttpResponseMessage postInsertNomeContrato(NomeContrato nomeContrato)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                int cdusuario = (int)this.ComponentesUser.CodUsuario;
                nomeContrato.cd_pessoa_escola = cdEscola;
                string caminho_relatorios = ConfigurationManager.AppSettings["caminhoUploads"];
                //string caminho_relatorios = HttpContext.Current.Server.MapPath("caminhoUploads");
                string pathContratosEscola = caminho_relatorios + "\\Contratos";
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });
                if (!String.IsNullOrEmpty(nomeContrato.no_relatorio))
                    if (nomeContrato.no_relatorio.Substring(nomeContrato.no_relatorio.Length - 5) != ".dotx")
                        throw new SecretariaBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroExtesaoLayoutNaoValida, null,
                       SecretariaBusinessException.TipoErro.ERRO_NAO_EXISTE_LAYOUT_NOME_CONTRATO, false);
                var nomContCad = Business.addNomeContrato(nomeContrato, pathContratosEscola, cdusuario);
                retorno.retorno = nomContCad;
                if (nomContCad.cd_nome_contrato <= 0)
                    retorno.AddMensagem(Messages.msgNotIncludReg, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (SecretariaBusinessException ex)
            {
                if (ex.tipoErro == SecretariaBusinessException.TipoErro.ERRO_LAYOUT_JA_EXISTE)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                if (ex.tipoErro == SecretariaBusinessException.TipoErro.ERRO_USUARIO_NAO_PODE_ESPECIALIZAR_LAYOUT_CONTRATO)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "nmCt.a")]
        public HttpResponseMessage postUpdateNomeContrato(NomeContrato nomeContrato)
        {
            ReturnResult retorno = new ReturnResult();
            //throw new Exception("sdfgasdfa");
            try
            {
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                int cdUsuario = (int)this.ComponentesUser.CodUsuario;
                nomeContrato.cd_pessoa_escola = cdEscola;
                string caminho_relatorios = ConfigurationManager.AppSettings["caminhoUploads"];
                //string caminho_relatorios = HttpContext.Current.Server.MapPath("caminhoUploads");
                string pathContratosEscola = caminho_relatorios + "\\Contratos";
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });
                if (!String.IsNullOrEmpty(nomeContrato.no_relatorio))
                    if (nomeContrato.no_relatorio.Substring(nomeContrato.no_relatorio.Length - 5) != ".dotx")
                        throw new SecretariaBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroExtesaoLayoutNaoValida, null,
                       SecretariaBusinessException.TipoErro.ERRO_NAO_EXISTE_LAYOUT_NOME_CONTRATO, false);
                var nomContCad = Business.editNomeContrato(nomeContrato,pathContratosEscola,cdUsuario);
                retorno.retorno = nomContCad;
                if (nomContCad.cd_nome_contrato <= 0)
                    retorno.AddMensagem(Messages.msgNotUpReg, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (SecretariaBusinessException ex)
            {
                if (ex.tipoErro == SecretariaBusinessException.TipoErro.ERRO_LAYOUT_JA_EXISTE)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                if (ex.tipoErro == SecretariaBusinessException.TipoErro.ERRO_SEM_PERMISAO_DELETAR_NOME_CONTRATO)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "nmCt.e")]
        public HttpResponseMessage postDeleteNomesContratos(List<NomeContrato> nomesContratos)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                int cdUsuario = (int)this.ComponentesUser.CodUsuario;
                string caminho_relatorios = ConfigurationManager.AppSettings["caminhoUploads"];
                //string caminho_relatorios = HttpContext.Current.Server.MapPath("caminhoUploads");
                string pathContratosEscola = caminho_relatorios + "\\Contratos";
                int[] cdNomesContratos = null;
                int i;
                // Pegando códigos da Turma
                if (nomesContratos != null && nomesContratos.Count() > 0)
                {
                    i = 0;
                    int[] cdNoCont = new int[nomesContratos.Count()];
                    foreach (var c in nomesContratos)
                    {
                        cdNoCont[i] = c.cd_nome_contrato;
                        i++;
                    }
                    cdNomesContratos = cdNoCont;
                }
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                bool deletou = Business.deleteNomesContratos(cdNomesContratos, pathContratosEscola, cdEscola, cdUsuario);
                retorno.retorno = deletou;
                if (!deletou)
                    retorno.AddMensagem(Messages.msgNotDeletedReg, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                else
                    retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (SecretariaBusinessException ex)
            {
                if (ex.tipoErro == SecretariaBusinessException.TipoErro.ERRO_SEM_PERMISAO_DELETAR_NOME_CONTRATO)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotDeletedReg, retorno, logger, ex);
            }

        }

        [HttpComponentesAuthorize(Roles = "nmCt")]
        public HttpResponseMessage getNomeContratoMat()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                IEnumerable<NomeContrato> ret = Business.getNomeContratoMat(cdEscola);
                retorno.retorno = ret;
                if (ret.Count() > 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, retorno);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        #endregion

        #region Desconto Contrato
        [HttpComponentesAuthorize(Roles = "mat")]
        public HttpResponseMessage getSearchDescontoContrato(int cd_contrato)
        {
             ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                IMatriculaBusiness BusinessMatricula = (IMatriculaBusiness)base.instanciarBusiness<IMatriculaBusiness>();
                var desconto = BusinessMatricula.getDescontoContratoPesq(cd_contrato, cdEscola);
                 retorno.retorno = desconto;
                if (desconto.ToList() == null || desconto.ToList().Count() <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }            
        }

        #endregion

        #region Taxa Matrícula
        [HttpComponentesAuthorize(Roles = "mat")]
        public HttpResponseMessage getSearchTaxaMatricula(int cd_contrato)
        {
             ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                IMatriculaBusiness BusinessMatricula = (IMatriculaBusiness)base.instanciarBusiness<IMatriculaBusiness>();
                var taxaMatricula = BusinessMatricula.searchTaxaMatricula(cd_contrato, cdEscola);
                 retorno.retorno = taxaMatricula;
                if (taxaMatricula == null || taxaMatricula.cd_taxa_matricula <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }            
        }

        #endregion

        #region Aditamento

        [HttpComponentesAuthorize(Roles = "mat")]
        public HttpResponseMessage getAditamentoByContrato(int cd_contrato)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                IMatriculaBusiness BusinessMatricula = (IMatriculaBusiness)base.instanciarBusiness<IMatriculaBusiness>();
                Aditamento aditamento = BusinessMatricula.getAditamentoByContratoMaxData(cd_contrato, cdEscola);
                retorno.retorno = aditamento;
                if (aditamento == null || aditamento.cd_aditamento <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "mat")]
        public HttpResponseMessage getAditamentosGridByContrato(int cd_contrato)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                IMatriculaBusiness BusinessMatricula = (IMatriculaBusiness)base.instanciarBusiness<IMatriculaBusiness>();
                IFinanceiroBusiness BusinessFinanceiro = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();

                var aditamento = BusinessMatricula.getAditamentosByContrato(cd_contrato, cdEscola);
                List<Titulo> titulosAbertos = BusinessFinanceiro.getTitulosByContratoLeitura(cd_contrato, cdEscola);

                foreach(var ad in aditamento)
                    ad.dt_aditamento = SGF.Utils.ConversorUTC.ToLocalTime(ad.dt_aditamento.Value, this.ComponentesUser.IdFusoHorario, this.ComponentesUser.IsHorarioVerao);
                retorno.retorno = aditamento;
                if (aditamento == null || aditamento.ToList().Count() <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                else                
                    aditamento.ToList()[0].vl_saldo_aditivo_em_aberto = titulosAbertos.Where(t => t.vl_saldo_titulo > 0).Sum(t => t.vl_saldo_titulo);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }
        [HttpPost]
        [HttpComponentesAuthorize(Roles = "mat")]
        public HttpResponseMessage deleteAditamentoByContrato(DeletaAditamentoUI deletaAditamentoUi)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                IMatriculaBusiness BusinessMatricula = (IMatriculaBusiness)base.instanciarBusiness<IMatriculaBusiness>();

                var aditamento = BusinessMatricula.deleteAditamentoByContrato(deletaAditamentoUi.cd_contrato, deletaAditamentoUi.cd_aditamento, cdEscola);

                retorno.retorno = aditamento;

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, retorno);
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(ex.InnerException.Message, retorno, logger, ex);
            }
        }

        #endregion

        #region Histórico Aluno

        [HttpComponentesAuthorize(Roles = "hista")]
        [HttpComponentesAuthorize(Roles = "prod")]
        public HttpResponseMessage getHistoricoAlunoSituacaoTurma(int cd_turma, int cd_contrato, string dataAvaliacao)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                DateTime dtAvaliacao = Convert.ToDateTime(dataAvaliacao);
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var avaliacao = Business.returnHistoricoSitacaoAlunoTurma(cd_turma, cdEscola);
                retorno.retorno = avaliacao;
                if (avaliacao == null || avaliacao.ToList().Count <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "hista")]
        [HttpComponentesAuthorize(Roles = "prod")]
        public HttpResponseMessage getHistoricoTurmas(int cd_aluno)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cd_escola = this.ComponentesUser.CodEmpresa.Value;
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                retorno.retorno = Business.getHistoricoTurmas(cd_aluno, cd_escola);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        public HttpResponseMessage getExisteFollowUsuario()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cd_escola = this.ComponentesUser.CodEmpresa.Value;
                int cd_usuario = this.ComponentesUser.CodUsuario;
                bool usuarioMaster = this.ComponentesUser.IdMaster;
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                retorno.retorno = Business.existeFollowNaoResolvido(cd_usuario, cd_escola, usuarioMaster);
                
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception exe)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, exe);
            }
        }
        
        [HttpComponentesAuthorize(Roles = "hista")]
        [HttpComponentesAuthorize(Roles = "alu")]
        public HttpResponseMessage getFollowAluno(int cd_aluno)
        {
            try
            {
                int cd_escola = this.ComponentesUser.CodEmpresa.Value;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var retorno = Business.getFollowAluno(parametros, cd_aluno, cd_escola);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception exe)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, exe);
            }
        }

        public HttpResponseMessage getProdutosComHistorico()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                List<ProdutoHistoricoSeachUI> produtos = Business.getProdutosComHistorico(cdEscola);
                retorno.retorno = produtos;
                if (produtos == null || produtos.Count <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        #endregion

        #region Desistência

        [HttpComponentesAuthorize(Roles = "desi")]
        [HttpComponentesAuthorize(Roles = "tur")]
        [HttpComponentesAuthorize(Roles = "mtdes")]
        [HttpComponentesAuthorize(Roles = "alu")]
        public HttpResponseMessage getDesistenciaSearch(int cd_turma, int cd_aluno, int cd_motivo_desistencia, int cd_tipo, String dta_ini, String dta_fim, int cd_produto, int cd_professor, string cursos)
        {
            try
            {
                List<int> cdsCurso = new List<int>();
                if (!String.IsNullOrEmpty(cursos))
                    cdsCurso = cursos .Split(',').Select(Int32.Parse).ToList();
                DateTime dtaInicial = DateTime.MinValue;
                DateTime dtaFinal = DateTime.MaxValue;

                if (!String.IsNullOrEmpty(dta_ini))
                    dtaInicial = Convert.ToDateTime(dta_ini);

                if (!String.IsNullOrEmpty(dta_fim))
                    dtaFinal = Convert.ToDateTime(dta_fim);

                int cd_pessoa_escola = this.ComponentesUser.CodEmpresa.Value;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var retorno = Business.getDesistenciaSearchUI(parametros, cd_turma, cd_aluno, cd_pessoa_escola, cd_motivo_desistencia, cd_tipo, dtaInicial, dtaFinal, cd_produto, cd_professor, cdsCurso);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception exe)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, exe);
            }
        }

        [HttpComponentesAuthorize(Roles = "desi")]
        [HttpComponentesAuthorize(Roles = "tur")]
        [HttpComponentesAuthorize(Roles = "mtdes")]
        [HttpComponentesAuthorize(Roles = "alu")]
        public HttpResponseMessage GeturlrelatorioDesistencia(string sort, int direction, int cd_turma, int cd_aluno, int cd_motivo_desistencia, int cd_tipo, String dta_ini, String dta_fim
            , int cd_produto, int cd_professor, string cursos)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                DateTime dtaInicial = DateTime.MinValue;
                DateTime dtaFinal = DateTime.MaxValue;

                if (!String.IsNullOrEmpty(dta_ini))
                    dtaInicial = Convert.ToDateTime(dta_ini);

                if (!String.IsNullOrEmpty(dta_fim))
                    dtaFinal = Convert.ToDateTime(dta_fim);
                int cd_pessoa_escola = this.ComponentesUser.CodEmpresa.Value;
                // Pega os parâmetros do usuário para criar a url do relatório:
                var strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@orientation=LANDSCAPE&@cd_turma=" + cd_turma + "&@cd_aluno=" + cd_aluno + "&@cd_pessoa_escola=" + cd_pessoa_escola +
                                    "&@cd_motivo_desistencia=" + cd_motivo_desistencia + "&@cd_tipo=" + cd_tipo + "&@dta_ini=" + dtaInicial + "&@dta_fim=" + dtaFinal  +
                                    "&@cd_produto=" + cd_produto + "&@cd_professor=" + cd_professor + "&@cursos=" + cursos +
                                    "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO +  "=Relatório de Desistência&" + 
                                    Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.DesistenciaSearch;
                string parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);


                HttpResponseMessage response =  Request.CreateResponse(HttpStatusCode.OK, parametrosCript);

                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }
   

        [HttpComponentesAuthorize(Roles = "desi")]
        public HttpResponseMessage getDesistenciaSearch(int cd_aluno_turma)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cd_pessoa_escola = this.ComponentesUser.CodEmpresa.Value;
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var des = Business.getDesistenciaAlunoTurma(cd_aluno_turma, cd_pessoa_escola);

                retorno.retorno = des;
                if (des.cd_desistencia <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception exe)
            {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, exe);
            }
        }

        // Post api/<controller>/5
        [Obsolete]
        [HttpComponentesAuthorize(Roles = "desi.i")]
        public HttpResponseMessage PostIncluirDesistencia(DesistenciaUI desistencia)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });
                int cd_pessoa_escola = this.ComponentesUser.CodEmpresa.Value;
                desistencia.cd_usuario = this.ComponentesUser.CodUsuario;
                var newDesistencia = Business.addDesistencia(desistencia, cd_pessoa_escola, 0, 0, true);
                retorno.retorno = newDesistencia;
                if (newDesistencia.cd_desistencia <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (AlunoBusinessException ex)
            {
                logger.Error(ex);
              
                if (ex.tipoErro == AlunoBusinessException.TipoErro.ERRO_ALUNO_STATUS_NAO_DESISTENTE)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                
                if (ex.tipoErro == AlunoBusinessException.TipoErro.ERRO_ALUNO_ESTA_CANCELADO)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                
                if (ex.tipoErro == AlunoBusinessException.TipoErro.ERRO_ALUNO_ESTA_DESISTENTE)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
            catch (SecretariaBusinessException ex)
            {
                logger.Error(ex);
                if (ex.tipoErro == SecretariaBusinessException.TipoErro.ERRO_DATA_MENOR_DESISTENCIA)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
        }

        [Obsolete]
        [HttpComponentesAuthorize(Roles = "desi.a")]
        public HttpResponseMessage PostEditarDesistencia(DesistenciaUI desistencia)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });
                int cd_pessoa_escola = this.ComponentesUser.CodEmpresa.Value;
                var newDesistencia = Business.editDesistencia(desistencia, cd_pessoa_escola, 0, true);
                retorno.retorno = newDesistencia;
                if (newDesistencia.cd_desistencia <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (SecretariaBusinessException ex)
            {
                logger.Error(ex);
                if (ex.tipoErro == SecretariaBusinessException.TipoErro.ERRO_DATA_FORA_INTERVALO_VALIDO)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
        }
       

        #endregion

        #region Ação Follow Up

        [HttpComponentesAuthorize(Roles = "acfup")]
        public HttpResponseMessage GeturlrelatorioAcaoFollowUp(string sort, int direction, string descricao, bool inicio, int status)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                // Pega os parâmetros do usuário para criar a url do relatório:
                var strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@descricao=" + descricao + "&@inicio=" + inicio + "&@status=" + status + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=Relatório de Ação Follow-up&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.AcaoFollowUp;
                //parametros = HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8);
                string parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, parametrosCript);

                configureHeaderResponse(response, null);
                return response;
             }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        #endregion

        #region Aluno
        [HttpComponentesAuthorize(Roles = "alu")]
        [Obsolete]
        public HttpResponseMessage getObservacaoAluno(int cd_aluno) 
        {
            ReturnResult response = new ReturnResult();
            try
            {
                IAlunoBusiness BusinessAluno = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
                int cd_escola = this.ComponentesUser.CodEmpresa.Value;
                response.retorno = BusinessAluno.getObservacaoAluno(cd_aluno, cd_escola);

                HttpResponseMessage msgResponse = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObjectAsync(response).Result);
                configureHeaderResponse(msgResponse, null);
                return msgResponse;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, response, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "alu")]
        public HttpResponseMessage getHorarioByEscolaForAluno(int cdAluno)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                DateTime dtInicio = DateTime.Now.Date;

                IAlunoBusiness BusinessAluno = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                List<Horario> horarios = BusinessAluno.getHorarioByEscolaForAluno(cdEscola, cdAluno).ToList<Horario>();
                List<Horario> horariosOcupados = Business.getHorarioOcupadosForTurma(cdEscola, cdAluno, new int[0], 0, 0, 0, dtInicio, null, HorarioDataAccess.TipoConsultaHorario.HAS_HORARIO_ALUNO_OCUPADO_TURMA).ToList<Horario>();
                foreach (Horario hd in horarios)
                {
                    hd.calendar = "Calendar1";
                }
                foreach (Horario ho in horariosOcupados)
                {
                    ho.calendar = "Calendar2";
                }

                var horariosL = horarios.Union(horariosOcupados);
                retorno.retorno = horariosL;

                if (horariosL.Count() <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        //Motivo Cancelamento Bolsa
        [HttpComponentesAuthorize(Roles = "acfup")]
        public HttpResponseMessage GetAcaoFollowUpSearch(string descricao, bool inicio, int status)
        {
            try
            {
                if (descricao == null)
                    descricao = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var retorno = Business.GetAcaoFollowUpSearch(parametros, descricao, inicio, getStatus(status));
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
        [HttpComponentesAuthorize(Roles = "acfup")]
        public HttpResponseMessage GetAcaoFollowUpById(int id)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var esc = Business.GetAcaoFollowUpById(id);
                retorno.retorno = esc;
                if (esc.cd_acao_follow_up <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }
        // Put api/<controller>/5
        [HttpComponentesAuthorize(Roles = "acfup.a")]
        public HttpResponseMessage PostAlterarAcaoFollowUp(AcaoFollowUp acaoFollowUp)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var esc = Business.PutAcaoFollowUp(acaoFollowUp);
                retorno.retorno = esc;
                if (esc.cd_acao_follow_up <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }

        }
        // Post api/<controller>/5
        [HttpComponentesAuthorize(Roles = "acfup.i")]
        public HttpResponseMessage PostAcaoFollowUp(AcaoFollowUp acaoFollowUp)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var esc = Business.PostAcaoFollowUp(acaoFollowUp);
                retorno.retorno = esc;
                if (esc.cd_acao_follow_up <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }

        }
        // Delte api/<controller>
        [HttpComponentesAuthorize(Roles = "acfup.e")]
        public HttpResponseMessage PostDeleteAcaoFollowUp(List<AcaoFollowUp> acoesFollowUp)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                bool deletou = Business.DeleteAcaoFollowUp(acoesFollowUp);
                retorno.retorno = deletou;
                if (!deletou)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                else
                    retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;

            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotDeletedReg, retorno, logger, ex);
            }

        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "rpla")]
        public HttpResponseMessage getUrlRelatorioListagemAniversariantes(int tipo, int cd_turma, int mes, int dia)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cd_pessoa_escola = this.ComponentesUser.CodEmpresa.Value;
                // Pega os parâmetros do usuário para criar a url do relatório:
                string parametros = "@cdEscola=" + cd_pessoa_escola + "&@tipo=" + tipo + "&@cd_turma=" + cd_turma + "&@mes=" + mes + "&@dia=" + dia + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=" + "Listagem de Aniversariantes";
                string parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, parametrosCript);

                configureHeaderResponse(response, null);
                return response;

            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage getUrlEnviarEmailAlunoRaf(string emailRafAluno, string nmRaf)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                string enderecoWebFormat = ConfigurationManager.AppSettings["enderecoAPI"];
                logger.Debug("Endereço Web");
                logger.Debug(enderecoWebFormat);

                string parametros = "nm_raf=" + nmRaf + "&dc_email=" + emailRafAluno + "&data=" + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");

                
                var parametroEncode = HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8);
                var url = MD5CryptoHelper.criptografaSenha(parametroEncode, MD5CryptoHelper.KEY);
                var urlEncode = HttpUtility.UrlEncode(url, System.Text.Encoding.UTF8);


                string urlRetorno = enderecoWebFormat + "/api/Auth/enviarSenhaRAF?parametros=" + urlEncode;
                logger.Debug("UrlRetorno");
                logger.Debug(urlRetorno);


                var result = postEnviaEmail(urlRetorno);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, result);

                configureHeaderResponse(response, null);
                return response;

            }
            catch (Exception ex)
            {
                var message = Messages.msgErroEnvioEmailAlunoRaf;
                if (ex.Message != null) message = ex.Message;
                return gerarLogException(message, retorno, logger, ex);
            }
        }


        public string postEnviaEmail(string url)
        {
            //API: https://sgfhomolog.fisk.com.br:153/SGFAPI /api/Auth/enviarSenhaRAF?parametros=Valor
            //valida comando
            var httpClient = new HttpClient();

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            string contents = null;
            bool succeeded = true;

            HttpResponseMessage response = null;
            try
            {
                //faz a requisição
                response = httpClient.PostAsync(url, null).GetAwaiter().GetResult();
                var result = response.Content.ReadAsStringAsync();
                var objJson = JObject.Parse(result.Result);
                foreach (var e in objJson)
                {
                    if (e.Key == "message")
                        contents = e.Value.ToString();
                    if (e.Key == "succeeded")
                        succeeded = (bool)e.Value;
                }
            }
            catch
            {
                throw new Exception(String.Format("Erro Interno ao tentar enviar e-mail"));
            }
            if (response.IsSuccessStatusCode && succeeded)
            {
                contents = "E-mail enviado com sucesso"; 
            }
            else
            {
                contents = "Erro ao enviar o e-mail" + (contents == null ? "" : ", " + contents);
                throw new Exception(contents);
            }

            return contents;
        }

        #endregion

        #region Prospect

        [HttpComponentesAuthorize(Roles = "alu")]
        public HttpResponseMessage getExistsEmailReturnPessoa(string email)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var prospect = Business.getProspectPorEmail(cdEscola, email);
                if (prospect != null)
                {
                    retorno.retorno = prospect;
                    retorno.AddMensagem(string.Format(Utils.Messages.Messages.msgExistsEmailProspect.Substring(0, Utils.Messages.Messages.msgExistsEmailProspect.Length - 1), prospect.PessoaFisica.no_pessoa) + Utils.Messages.Messages.msgPessoaExistByEmail, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                }
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "alu")]
        public HttpResponseMessage getExistsCpfENome(string email, string nome, int cd_pessoa_cpf)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                ISecretariaBusiness BusinessSecretaria = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                IAlunoBusiness BusinessAluno = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
                IPessoaBusiness BusinessPessoa = (IPessoaBusiness)base.instanciarBusiness<IPessoaBusiness>();
                //ProspectSearchUI prospect; 
                var prospect = Business.getProspectPorEmail(cdEscola, email);
                if (prospect != null)
                {
                    retorno.retorno = prospect;
                    retorno.AddMensagem(string.Format(Utils.Messages.Messages.msgExistsEmailProspect.Substring(0, Utils.Messages.Messages.msgExistsEmailProspect.Length - 1), prospect.PessoaFisica.no_pessoa) + Utils.Messages.Messages.msgPessoaExistByEmail, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                }
                else
                {
                    if (!string.IsNullOrEmpty(email) && string.IsNullOrEmpty(nome) && cd_pessoa_cpf == 0)
                    {
                        Aluno aluno = null;
                        aluno = BusinessAluno.buscarAlunoExistEmail(email, null, 0);

                        if (aluno != null)
                        {
                            retorno.retorno = aluno;
                            retorno.AddMensagem(string.Format(FundacaoFisk.SGF.Utils.Messages.Messages.msgExistsEmailAlunoConfirm, aluno.no_prospect), null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                        }
                        else
                        {
                            var pessoaFisica = BusinessSecretaria.verificarPessoaFisicaEmail(email);
                            if (pessoaFisica != null)
                            {
                                retorno.retorno = pessoaFisica;
                                retorno.AddMensagem(string.Format(FundacaoFisk.SGF.Utils.Messages.Messages.msgExistsEmailPessoa, pessoaFisica.no_pessoa), null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                            }
                        }
                    }
                    else
                    {
                        var pessoaFisicaUI = BusinessAluno.ExistAlunoOrPessoaFisicaByCpfAluno(null, email, nome, cd_pessoa_cpf, cdEscola);
                        if (pessoaFisicaUI != null && pessoaFisicaUI.pessoaFisica != null && pessoaFisicaUI.pessoaFisica.cd_pessoa > 0)
                        {
                            if (pessoaFisicaUI.pessoaFisica.TelefonePessoa.Count() > 0)
                            {
                                pessoaFisicaUI.contatosUI = TelefoneUI.fromTelefoneforTelefoneUI(pessoaFisicaUI.pessoaFisica.TelefonePessoa);
                                pessoaFisicaUI.pessoaFisica.TelefonePessoa = null;
                            }
                            if (pessoaFisicaUI.pessoaFisica.Telefone != null)
                            {
                                pessoaFisicaUI.pessoaFisica.Telefone.ClasseTelefone.TelefoneClasse = null;
                                pessoaFisicaUI.pessoaFisica.Telefone.TelefonePessoa = null;
                                pessoaFisicaUI.pessoaFisica.Telefone.TelefoneTipo.TipoTelefoneRef = null;
                                pessoaFisicaUI.pessoaFisica.Telefone.TelefonePessoa = null;
                            }
                            if (pessoaFisicaUI.pessoaFisica.PessoaPaiRelacionamento.Count() > 0)
                            {
                                pessoaFisicaUI.relacionamentoUI = RelacionamentoUI.fromRelacionamentoforRelacionamentoUI(pessoaFisicaUI.pessoaFisica.PessoaPaiRelacionamento, pessoaFisicaUI.pessoaFisica.cd_pessoa);
                                pessoaFisicaUI.pessoaFisica.PessoaPaiRelacionamento = null;
                            }
                            retorno.retorno = pessoaFisicaUI;
                            retorno.AddMensagem(string.Format(FundacaoFisk.SGF.Utils.Messages.Messages.msgAlunocExistReturnData, pessoaFisicaUI.pessoaFisica.no_pessoa), null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                        }
                    }
                }

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        public HttpResponseMessage getExisteProspectNaoConsultado()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cd_escola = this.ComponentesUser.CodEmpresa.Value;
                
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                retorno.retorno = Business.existeProspectNaoConsultado( cd_escola);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception exe)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, exe);
            }
        }

        public HttpResponseMessage getPossuiPermissaoPesquisaProspect()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                bool possuiPermissao = false;
                var permissoes = UserCredential.permissoes;

                if (!string.IsNullOrEmpty(permissoes))
                {
                    possuiPermissao = permissoes.ToString().Contains("pros")
                                   && permissoes.ToString().Contains("prod")
                                   && permissoes.ToString().Contains("mtnm")
                                   && permissoes.ToString().Contains("oper")
                                   && permissoes.ToString().Contains("oper")
                                   && permissoes.ToString().Contains("mid");
                }
                    
                retorno.retorno = possuiPermissao;

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception exe)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, exe);
            }
        }

        public HttpResponseMessage getPossuiPermissaoVisualizaVideo()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                bool possuiPermissao = false;
                var permissoes = UserCredential.permissoes;

                if (!string.IsNullOrEmpty(permissoes))
                {
                    possuiPermissao = permissoes.ToString().Contains("vid");
                }

                retorno.retorno = possuiPermissao;

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception exe)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, exe);
            }
        }

        public HttpResponseMessage getPossuiPermissaoVisualizaCircular()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                bool possuiPermissao = false;
                var permissoes = UserCredential.permissoes;

                if (!string.IsNullOrEmpty(permissoes))
                {
                    possuiPermissao = permissoes.ToString().Contains("cir");
                }

                retorno.retorno = possuiPermissao;

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception exe)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, exe);
            }
        }

        
        public HttpResponseMessage setProspectsConsultado()
        {
            
            try
            {
                int cd_escola = this.ComponentesUser.CodEmpresa.Value;

                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                Business.setProspectsConsultado(cd_escola);
               
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
                //configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception exe)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, exe);
            }
        }


        [HttpGet]
        [HttpComponentesAuthorize(Roles = "pros")]
        [HttpComponentesAuthorize(Roles = "prod")]
        [HttpComponentesAuthorize(Roles = "mtnm")]
        [HttpComponentesAuthorize(Roles = "oper")]
        [HttpComponentesAuthorize(Roles = "mid")]
        public HttpResponseMessage getProspectSite(int cdProspect, int tipo)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                List<ProspectSiteUI> prospectSites = Business.getProspectSite(cdProspect, tipo);

                retorno.retorno = prospectSites;
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, retorno);
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "pros")]
        [HttpComponentesAuthorize(Roles = "prod")]
        [HttpComponentesAuthorize(Roles = "mtnm")]
        [HttpComponentesAuthorize(Roles = "oper")]
        [HttpComponentesAuthorize(Roles = "mid")]
        public HttpResponseMessage returnDataProspect(int? cdOperadora, int cdProspect)
        {
            ReturnResult retorno = new ReturnResult();
            ProspectSearchUI prospectSearchUI = new ProspectSearchUI();
            Prospect prospect = new Prospect();
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                ILocalidadeBusiness BusinessLoc = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                IEmpresaBusiness BusinessEmp = (IEmpresaBusiness)base.instanciarBusiness<IEmpresaBusiness>();
                //var usuario = authBiz.GetNomeCodigoUsuario(User.Identity.Name);
                prospectSearchUI.dc_email_escola = BusinessEmp.getEmailEscola(cdEscola);
                if (cdProspect != 0)
                {
                    prospect = Business.getProspectForEdit(cdProspect, cdEscola, null);
                    prospectSearchUI.copy(prospect);
                    prospectSearchUI.no_usuario = prospect.SysUsuario.no_login;
                    prospectSearchUI.cd_usuario = prospect.SysUsuario.cd_usuario;
                    prospectSearchUI.no_pessoa = prospect.PessoaFisica.no_pessoa;
                    prospectSearchUI.nm_sexo = prospect.PessoaFisica.nm_sexo;
                    prospectSearchUI.vl_matricula = prospect.vl_matricula_prospect;
                    prospectSearchUI.dt_matricula = prospect.dt_matricula_prospect;
                    prospectSearchUI.cd_plano_conta_tit = prospect.cd_plano_conta;
                    prospectSearchUI.gerar_baixa = prospect.id_gerar_baixa;
                    prospectSearchUI.cd_local_movto = prospect.cd_local_movimento;
                    prospectSearchUI.cd_tipo_liquidacao = prospect.cd_tipo_liquidacao;
                    prospectSearchUI.no_escola = prospect.no_escola;
                    prospectSearchUI.id_faixa_etaria = prospect.id_faixa_etaria;
                    prospectSearchUI.cd_motivo_inativo = prospect.cd_motivo_inativo;
                    prospectSearchUI.vl_abatimento =  prospect.vl_abatimento;
                    if (prospect.cd_plano_conta > 0)
                        prospectSearchUI.desc_plano_conta = prospect.PlanoConta.PlanoContaSubgrupo.no_subgrupo_conta;

                    prospectSearchUI.email = prospect.PessoaFisica.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL).Select(t => t.dc_fone_mail).FirstOrDefault();

                    prospectSearchUI.telefone = prospect.PessoaFisica.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.TELEFONE).Select(t => t.dc_fone_mail).FirstOrDefault();

                    prospectSearchUI.celular = prospect.PessoaFisica.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.CELULAR).Select(t => t.dc_fone_mail).FirstOrDefault();

                    prospectSearchUI.cd_operadora = prospect.PessoaFisica.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.CELULAR).Select(t => t.cd_operadora).FirstOrDefault();

                    prospectSearchUI.id_dia_semana = prospect.id_dia_semana;
                    if (prospect.ProspectProduto != null)
                        foreach (ProspectProduto p in prospect.ProspectProduto)
                            p.Prospect = null;
                    prospectSearchUI.produtos = prospect.ProspectProduto;
                    if (prospect.ProspectPeriodo != null)
                        foreach (ProspectPeriodo p in prospect.ProspectPeriodo)
                            p.Prospect = null;
                    prospectSearchUI.periodos = prospect.ProspectPeriodo;
                    if (prospect.ProspectDia != null)
                        foreach (ProspectDia p in prospect.ProspectDia)
                            p.Prospect = null;
                    prospectSearchUI.dias = prospect.ProspectDia;
                    if (prospect.PessoaFisica.EnderecoPrincipal != null)
                    {
                        prospectSearchUI.endereco = prospect.PessoaFisica.EnderecoPrincipal;
                    }
                    prospectSearchUI.dt_nascimento_prospect = prospect.PessoaFisica.dt_nascimento;

                    prospectSearchUI = ProspectSearchUI.fromProspectUI(prospectSearchUI, prospect.cd_prospect, prospect.PessoaFisica.dt_cadastramento);
                }
                else
                {
                    prospectSearchUI.no_pessoa = Business.getNomeAtendente(this.ComponentesUser.CodUsuario, cdEscola);
                    prospectSearchUI.cd_usuario = this.ComponentesUser.CodUsuario;
                    prospectSearchUI.dt_cadastramento = DateTime.UtcNow.ToLocalTime().Date;
                }

                prospectSearchUI.midias = Business.getMidia(null, MidiaDataAccess.TipoConsultaMidiaEnum.HAS_ATIVO_INATIVO, null).ToList();
                prospectSearchUI.operadoras = BusinessLoc.GetAllOperadorasAtivas(cdOperadora).ToList();

                retorno.retorno = prospectSearchUI;
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "rppros")]
        [HttpGet]
        public HttpResponseMessage getUrlRelatorioProspectAtendido(int cd_mtv_nao_matricula, int cd_funcionario, int cd_produto, string dta_ini, string dta_fim, int tipo, string dta_ini_comp,
            string dta_fim_comp, int cd_midia, string cd_produtos, int cd_faixa_etaria)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cd_pessoa_escola = this.ComponentesUser.CodEmpresa.Value;
                DateTime? dataIncial = dta_ini == null ? null : (DateTime?)Convert.ToDateTime(dta_ini);
                DateTime? dataFinal = dta_fim == null ? null : (DateTime?)Convert.ToDateTime(dta_fim);
                DateTime? dataIncialComp = dta_ini == null ? null : (DateTime?)Convert.ToDateTime(dta_ini_comp);
                DateTime? dataFinalComp = dta_fim == null ? null : (DateTime?)Convert.ToDateTime(dta_fim_comp);
                // Pega os parâmetros do usuário para criar a url do relatório:
                string parametros = "@cdEscola=" + cd_pessoa_escola + "&@cdMotivoNaoMatricula=" + cd_mtv_nao_matricula + "&@cFuncionario=" + cd_funcionario +
                                    "&@cdProduto=" + cd_produto + "&@pDtaI=" + dataIncial + "&@pDtaF=" + dataFinal + "&@tipo=" + tipo + "&@pDtaIComp="
                                    + dataIncialComp + "&@pDtaFComp=" + dataFinalComp + "&@cd_midia=" + cd_midia + "&@cd_produtos=" + cd_produtos + "&@cd_faixa_etaria=" + cd_faixa_etaria;

                string parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, parametrosCript);

                configureHeaderResponse(response, null);
                return response;

            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }
        #endregion

        #region Pessoa

        [HttpPost]
        [HttpComponentesAuthorize(Roles = "pes.i,pes.a")]
        [Obsolete]
        public HttpResponseMessage UploadImage()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                var httpPostedFile = HttpContext.Current.Request.Files["UploadedImage"];
                if (httpPostedFile == null)
                    throw new PessoaBusinessException(Messages.msgImagemInvalida, null, PessoaBusinessException.TipoErro.ERRO_NAO_EXISTE_IMAGEM, false);
                var fileUpload = httpPostedFile;
                //limite  500kbytes
                string extensaoArq = fileUpload.FileName.Substring(fileUpload.FileName.Length - 4);
                if (fileUpload.ContentLength > 1000000)
                    throw new PessoaBusinessException(Messages.msgErroImagemExcedeuLimte, null, PessoaBusinessException.TipoErro.ERRO_NAO_EXISTE_IMAGEM, false);
                if (extensaoArq.ToLower() != ".jpg" && extensaoArq.ToLower() != "jpeg" && extensaoArq.ToLower() != ".gif" && extensaoArq.ToLower() != ".png" && extensaoArq.ToLower() != ".gif")
                    throw new PessoaBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroExtensaoImagemNaoSuportada, null, PessoaBusinessException.TipoErro.ERRO_NAO_EXISTE_IMAGEM, false);
                //Local onde vai ficar as fotos enviadas.
                var uploadPath = ConfigurationManager.AppSettings["caminhoUploads"];
                //var enderecoWeb = ConfigurationManager.AppSettings["enderecoWeb"];
                var serverUploadPath = uploadPath;
                //Faz um checagem se o arquivo veio correto.

                var uploadedFilePath = Path.Combine(serverUploadPath, fileUpload.FileName);

                //faz o upload literalmetne do arquivo.
                byte[] buffer;
                FileStream fs;
                using (fs = new FileStream(uploadedFilePath, FileMode.Create))
                {
                    buffer = new byte[fileUpload.InputStream.Length];
                    fileUpload.InputStream.Read(buffer, 0, buffer.Length);
                    fs.Write(buffer, 0, buffer.Length);
                    fs.Dispose();
                }
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObjectAsync(fileUpload.FileName).Result);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUploadImage, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "pes.i")]
        public HttpResponseMessage PostInsertPessoaFisica(PessoaFisicaUI pessoa)
        {
            //PessoaFisicaUI pessoaFisica = pessoaFisica1;
            string fullPath = null;
            string caminho = null;
            ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
            ReturnResult retorno = new ReturnResult();
            List<RelacionamentoSGF> relacionamentos = null;
            pessoa.pessoaFisica.dt_cadastramento = Utils.Utils.truncarMilissegundo(pessoa.pessoaFisica.dt_cadastramento.ToUniversalTime());
            try
            {
                configuraBusiness(new List<IGenericBusiness>() { Business });
                int cdEsc = this.ComponentesUser.CodEmpresa.Value;
                pessoa.pessoaFisica.id_pessoa_empresa = false;

                pessoa.pessoaFisica.dt_nascimento = pessoa.pessoaFisica.dt_nascimento.HasValue ? pessoa.pessoaFisica.dt_nascimento.Value.Date : pessoa.pessoaFisica.dt_nascimento;

                if (pessoa != null && !string.IsNullOrEmpty(pessoa.descFoto))
                {

                    fullPath = ConfigurationManager.AppSettings["caminhoUploads"];
                    caminho = fullPath + "/" + pessoa.descFoto;
                    pessoa.pessoaFisica.img_pessoa = ManipuladorArquivo.getPathPhoto(caminho);
                    pessoa.pessoaFisica.ext_img_pessoa = pessoa.descFoto;
                }

                if (pessoa.relacionamentosUI != null && pessoa.relacionamentosUI.Count() > 0)
                {
                    relacionamentos = new List<RelacionamentoSGF>();
                    foreach (var item in pessoa.relacionamentosUI)
                    {
                        RelacionamentoSGF relac = new RelacionamentoSGF();
                        relac = item.relacionamento;
                        if (item.nm_natureza_pessoa == 1)
                        {
                            if (item.pessoaFisicaRelac != null && !string.IsNullOrEmpty(item.pessoaFisicaRelac.no_pessoa) && item.pessoaFisicaRelac.nm_sexo > 0)
                            {
                                item.pessoaFisicaRelac.nm_natureza_pessoa = 1;
                                relac.PessoaFilho = item.pessoaFisicaRelac;
                                if (item.enderecoRelac != null && item.enderecoRelac.cd_loc_cidade > 0 && item.enderecoRelac.cd_loc_bairro > 0 && item.enderecoRelac.cd_loc_estado > 0 &&
                                item.enderecoRelac.cd_loc_logradouro > 0 && item.enderecoRelac.cd_tipo_logradouro > 0)
                                    relac.PessoaFilho.EnderecoPrincipal = item.enderecoRelac;
                            }
                            relacionamentos.Add(relac);
                        }
                        else
                        {
                            if (item.pessoaJuridicaRelac != null && !string.IsNullOrEmpty(item.pessoaJuridicaRelac.no_pessoa) && !string.IsNullOrEmpty(item.pessoaJuridicaRelac.dc_num_cgc))
                            {
                                item.pessoaJuridicaRelac.cd_tipo_sociedade = 1;
                                item.pessoaJuridicaRelac.nm_natureza_pessoa = 2;
                                relac.PessoaFilho = item.pessoaJuridicaRelac;
                                if (item.enderecoRelac != null && item.enderecoRelac.cd_loc_cidade > 0 && item.enderecoRelac.cd_loc_bairro > 0 && item.enderecoRelac.cd_loc_estado > 0 &&
                                item.enderecoRelac.cd_loc_logradouro > 0 && item.enderecoRelac.cd_tipo_logradouro > 0)
                                    relac.PessoaFilho.EnderecoPrincipal = item.enderecoRelac;
                            }
                            relacionamentos.Add(relac);
                        }

                    }
                }

                var insertPessoaFisica = Business.postInsertPessoaFisica(pessoa, relacionamentos, cdEsc);
                retorno.retorno = FundacaoFisk.SGF.Web.Services.Pessoa.Model.PessoaSearchUI.fromPessoaForPessoaSearchUI(insertPessoaFisica);

                if (insertPessoaFisica.cd_pessoa <= 0)
                    retorno.AddMensagem(Messages.msgNotIncludReg, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                return response;
            }
            catch (PessoaBusinessException ex)
            {
                if (ex.tipoErro == PessoaBusinessException.TipoErro.ERRO_CPFJAEXISTENTE || ex.tipoErro == PessoaBusinessException.TipoErro.ERRO_AUTO_RELACIONAMENTO
                    || ex.tipoErro == PessoaBusinessException.TipoErro.ERRO_MINDATE_SMALLDATETIME)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                var message = Utils.Utils.innerMessage(ex);
                if (message != "")
                {
                    PessoaBusinessException fx = new PessoaBusinessException(message, ex, 0, false);
                    return gerarLogException(message, retorno, logger, fx);
                }
                else
                {
                    return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
                }

            }
        }

        [HttpComponentesAuthorize(Roles = "pes.i")]
        public HttpResponseMessage PostInsertPessoaJuridica(PessoaJuridicaUI pessoa)
        {
            //PessoaFisicaUI pessoaFisica = pessoaFisica1;
            string fullPath = null;
            string caminho = null;
            ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
            ReturnResult retorno = new ReturnResult();
            List<RelacionamentoSGF> relacionamentos = null;
            pessoa.pessoaJuridica.dt_cadastramento = Utils.Utils.truncarMilissegundo(pessoa.pessoaJuridica.dt_cadastramento.ToUniversalTime());
            try
            {
                configuraBusiness(new List<IGenericBusiness>() { Business });
                int cdEsc = this.ComponentesUser.CodEmpresa.Value;
                pessoa.pessoaJuridica.id_pessoa_empresa = false;
                pessoa.pessoaJuridica.dt_registro_junta_comercial = pessoa.pessoaJuridica.dt_registro_junta_comercial.HasValue ? pessoa.pessoaJuridica.dt_registro_junta_comercial.Value.Date : pessoa.pessoaJuridica.dt_registro_junta_comercial;
                pessoa.pessoaJuridica.dt_baixa = pessoa.pessoaJuridica.dt_baixa.HasValue ? pessoa.pessoaJuridica.dt_baixa.Value.Date : pessoa.pessoaJuridica.dt_baixa;

                if (pessoa != null && !string.IsNullOrEmpty(pessoa.descFoto))
                {

                    fullPath = ConfigurationManager.AppSettings["caminhoUploads"];
                    caminho = fullPath + "/" + pessoa.descFoto;
                    pessoa.pessoaJuridica.img_pessoa = ManipuladorArquivo.getPathPhoto(caminho);
                    pessoa.pessoaJuridica.ext_img_pessoa = pessoa.descFoto;
                }

                if (pessoa.relacionamentosUI != null && pessoa.relacionamentosUI.Count() > 0)
                {
                    relacionamentos = new List<RelacionamentoSGF>();
                    foreach (var item in pessoa.relacionamentosUI)
                    {
                        RelacionamentoSGF relac = new RelacionamentoSGF();
                        relac = item.relacionamento;
                        if (item.nm_natureza_pessoa == 1)
                        {
                            if (item.pessoaFisicaRelac != null && !string.IsNullOrEmpty(item.pessoaFisicaRelac.no_pessoa) && item.pessoaFisicaRelac.nm_sexo > 0)
                            {
                                item.pessoaFisicaRelac.nm_natureza_pessoa = 1;
                                relac.PessoaFilho = item.pessoaFisicaRelac;
                                if (item.enderecoRelac != null && item.enderecoRelac.cd_loc_cidade > 0 && item.enderecoRelac.cd_loc_bairro > 0 && item.enderecoRelac.cd_loc_estado > 0 &&
                                item.enderecoRelac.cd_loc_logradouro > 0 && item.enderecoRelac.cd_tipo_logradouro > 0)
                                    relac.PessoaFilho.EnderecoPrincipal = item.enderecoRelac;
                            }
                            relacionamentos.Add(relac);
                        }
                        else
                        {
                            if (item.pessoaJuridicaRelac != null && !string.IsNullOrEmpty(item.pessoaJuridicaRelac.no_pessoa) && !string.IsNullOrEmpty(item.pessoaJuridicaRelac.dc_num_cgc))
                            {
                                item.pessoaJuridicaRelac.cd_tipo_sociedade = 1;
                                item.pessoaJuridicaRelac.nm_natureza_pessoa = 2;
                                relac.PessoaFilho = item.pessoaJuridicaRelac;
                                if (item.enderecoRelac != null && item.enderecoRelac.cd_loc_cidade > 0 && item.enderecoRelac.cd_loc_bairro > 0 && item.enderecoRelac.cd_loc_estado > 0 &&
                                item.enderecoRelac.cd_loc_logradouro > 0 && item.enderecoRelac.cd_tipo_logradouro > 0)
                                    relac.PessoaFilho.EnderecoPrincipal = item.enderecoRelac;
                            }
                            relacionamentos.Add(relac);
                        }

                    }
                }

                var insertPessoaJuridica = Business.postInsertPessoaJuridica(pessoa, relacionamentos, cdEsc);
                retorno.retorno = FundacaoFisk.SGF.Web.Services.Pessoa.Model.PessoaSearchUI.fromPessoaForPessoaSearchUI(insertPessoaJuridica);

                if (insertPessoaJuridica.cd_pessoa <= 0)
                    retorno.AddMensagem(Messages.msgNotIncludReg, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                return response;
            }
            catch (PessoaBusinessException ex)
            {
                if (ex.tipoErro == PessoaBusinessException.TipoErro.ERRO_CNPJJAEXISTENTE)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                var message = Utils.Utils.innerMessage(ex);
                if (message != "")
                {
                    PessoaBusinessException fx = new PessoaBusinessException(message, ex, 0, false);
                    return gerarLogException(message, retorno, logger, fx);
                }
                else
                {
                    return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
                }
            }
        }

        [HttpComponentesAuthorize(Roles = "pes.a")]
        public HttpResponseMessage postUpdatePessoaFisica(PessoaFisicaUI pessoa)
        {
            string fullPath = null;
            string caminho = null;
            ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
            ReturnResult retorno = new ReturnResult();
            try
            {
                configuraBusiness(new List<IGenericBusiness>() { Business});
                int cdEsc = this.ComponentesUser.CodEmpresa.Value;
                pessoa.pessoaFisica.id_pessoa_empresa = false;

                pessoa.pessoaFisica.dt_nascimento = pessoa.pessoaFisica.dt_nascimento.HasValue ? pessoa.pessoaFisica.dt_nascimento.Value.Date : pessoa.pessoaFisica.dt_nascimento;

                if (pessoa != null && !string.IsNullOrEmpty(pessoa.descFoto))
                {

                    fullPath = ConfigurationManager.AppSettings["caminhoUploads"];
                    caminho = fullPath + "/" + pessoa.descFoto;
                    pessoa.pessoaFisica.img_pessoa = ManipuladorArquivo.getPathPhoto(caminho);
                    pessoa.pessoaFisica.ext_img_pessoa = pessoa.descFoto;
                }

                var insertPessoaFisica = Business.postUpdatePessoaFisica(pessoa, RelacionamentoUI.toRelacionamentos(pessoa.relacionamentosUI), cdEsc);
                retorno.retorno = FundacaoFisk.SGF.Web.Services.Pessoa.Model.PessoaSearchUI.fromPessoaForPessoaSearchUI(insertPessoaFisica);
                if (insertPessoaFisica.cd_pessoa <= 0)
                {
                    retorno.AddMensagem(Messages.msgNotUpReg, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                }
                else
                {
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                return response;
            }
            catch (PessoaBusinessException ex)
            {
                if (ex.tipoErro == PessoaBusinessException.TipoErro.ERRO_CNPJJAEXISTENTE)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                var message = Utils.Utils.innerMessage(ex);
                if (message != "")
                {
                    PessoaBusinessException fx = new PessoaBusinessException(message, ex, 0, false);
                    return gerarLogException(message, retorno, logger, fx);
                }
                else
                {
                    return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
                }
            }
        }

        [HttpComponentesAuthorize(Roles = "pes.a")]
        public HttpResponseMessage postUpdatePessoaJuridica(PessoaJuridicaUI pessoa)
        {
            string fullPath = null;
            string caminho = null;
            ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
            ReturnResult retorno = new ReturnResult();
            List<RelacionamentoSGF> relacionamentos = null;
            try
            {
                configuraBusiness(new List<IGenericBusiness>() { Business});
                int cdEsc = this.ComponentesUser.CodEmpresa.Value;
                pessoa.pessoaJuridica.id_pessoa_empresa = false;
                pessoa.pessoaJuridica.dt_registro_junta_comercial = pessoa.pessoaJuridica.dt_registro_junta_comercial.HasValue ? pessoa.pessoaJuridica.dt_registro_junta_comercial.Value.Date : pessoa.pessoaJuridica.dt_registro_junta_comercial;
                pessoa.pessoaJuridica.dt_baixa = pessoa.pessoaJuridica.dt_baixa.HasValue ? pessoa.pessoaJuridica.dt_baixa.Value.Date : pessoa.pessoaJuridica.dt_baixa;

                if (pessoa != null && !string.IsNullOrEmpty(pessoa.descFoto))
                {

                    fullPath = ConfigurationManager.AppSettings["caminhoUploads"];
                    caminho = fullPath + "/" + pessoa.descFoto;
                    pessoa.pessoaJuridica.img_pessoa = ManipuladorArquivo.getPathPhoto(caminho);
                    pessoa.pessoaJuridica.ext_img_pessoa = pessoa.descFoto;
                }

                if (pessoa.relacionamentosUI != null && pessoa.relacionamentosUI.Count() > 0)
                {
                    relacionamentos = new List<RelacionamentoSGF>();
                    foreach (var item in pessoa.relacionamentosUI)
                    {
                        RelacionamentoSGF relac = new RelacionamentoSGF();
                        relac = item.relacionamento;
                        if (item.nm_natureza_pessoa == 1)
                        {
                            if (item.pessoaFisicaRelac != null && !string.IsNullOrEmpty(item.pessoaFisicaRelac.no_pessoa) && item.pessoaFisicaRelac.nm_sexo > 0)
                            {
                                item.pessoaFisicaRelac.nm_natureza_pessoa = 1;
                                relac.PessoaFilho = item.pessoaFisicaRelac;
                                if (item.enderecoRelac != null && item.enderecoRelac.cd_loc_cidade > 0 && item.enderecoRelac.cd_loc_bairro > 0 && item.enderecoRelac.cd_loc_estado > 0 &&
                                item.enderecoRelac.cd_loc_logradouro > 0 && item.enderecoRelac.cd_tipo_logradouro > 0)
                                    relac.PessoaFilho.EnderecoPrincipal = item.enderecoRelac;
                            }
                            relacionamentos.Add(relac);
                        }
                        else
                        {
                            if (item.pessoaJuridicaRelac != null && !string.IsNullOrEmpty(item.pessoaJuridicaRelac.no_pessoa) && !string.IsNullOrEmpty(item.pessoaJuridicaRelac.dc_num_cgc))
                            {
                                item.pessoaJuridicaRelac.cd_tipo_sociedade = 1;
                                item.pessoaJuridicaRelac.nm_natureza_pessoa = 2;
                                relac.PessoaFilho = item.pessoaJuridicaRelac;
                                if (item.enderecoRelac != null && item.enderecoRelac.cd_loc_cidade > 0 && item.enderecoRelac.cd_loc_bairro > 0 && item.enderecoRelac.cd_loc_estado > 0 &&
                                item.enderecoRelac.cd_loc_logradouro > 0 && item.enderecoRelac.cd_tipo_logradouro > 0)
                                    relac.PessoaFilho.EnderecoPrincipal = item.enderecoRelac;
                            }
                            relacionamentos.Add(relac);
                        }

                    }
                }

                var insertPessoaJuridica = Business.postUpdatePessoaJuridica(pessoa, relacionamentos, cdEsc);
                retorno.retorno = FundacaoFisk.SGF.Web.Services.Pessoa.Model.PessoaSearchUI.fromPessoaForPessoaSearchUI(insertPessoaJuridica);

                if (insertPessoaJuridica.cd_pessoa <= 0)
                {
                    retorno.AddMensagem(Messages.msgNotUpReg, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                }
                else
                {
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                }
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                return response;
            }
            catch (Exception ex)
            {
                var message = Utils.Utils.innerMessage(ex);
                if (message != "")
                {
                    PessoaBusinessException fx = new PessoaBusinessException(message, ex, 0, false);
                    return gerarLogException(message, retorno, logger, fx);
                }
                else
                {
                    return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
                }
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "func")]
        public HttpResponseMessage existePessoaEscolaOrByCpf(string cpf)
        {
            ReturnResult retorno = new ReturnResult();
            int cdEscola = this.ComponentesUser.CodEmpresa.Value;
            try
            {
                IAlunoBusiness BusinessAluno = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
                var pessoaFisicaUI = BusinessAluno.existePessoaEscolaOrByCpf(cpf, cdEscola);
                if (pessoaFisicaUI != null && pessoaFisicaUI.pessoaFisica != null)
                {
                    if (pessoaFisicaUI.pessoaFisica.TelefonePessoa.Count() > 0)
                    {
                        pessoaFisicaUI.contatosUI = TelefoneUI.fromTelefoneforTelefoneUI(pessoaFisicaUI.pessoaFisica.TelefonePessoa);
                        pessoaFisicaUI.pessoaFisica.TelefonePessoa = null;
                    }
                    if (pessoaFisicaUI.pessoaFisica.Telefone != null)
                    {
                        pessoaFisicaUI.pessoaFisica.Telefone.ClasseTelefone.TelefoneClasse = null;
                        pessoaFisicaUI.pessoaFisica.Telefone.TelefonePessoa = null;
                        pessoaFisicaUI.pessoaFisica.Telefone.TelefoneTipo.TipoTelefoneRef = null;
                        pessoaFisicaUI.pessoaFisica.Telefone.TelefonePessoa = null;
                    }
                    //if (pessoaFisicaUI.pessoaFisica.PessoaEndereco.Count() > 0)
                    //    pessoaFisicaUI.enderecosUI = EnderecoUI.fromEnderecoforEnderecoUI(pessoaFisicaUI.pessoaFisica.PessoaEndereco, pessoaFisicaUI.pessoaFisica.cd_endereco_principal);
                    if (pessoaFisicaUI.pessoaFisica.PessoaPaiRelacionamento.Count() > 0)
                    {
                        pessoaFisicaUI.relacionamentoUI = RelacionamentoUI.fromRelacionamentoforRelacionamentoUI(pessoaFisicaUI.pessoaFisica.PessoaPaiRelacionamento, pessoaFisicaUI.pessoaFisica.cd_pessoa);
                        pessoaFisicaUI.pessoaFisica.PessoaPaiRelacionamento = null;
                    }
                    retorno.retorno = pessoaFisicaUI;
                    retorno.AddMensagem(string.Format(Messages.msgExistePessoaRelacionadaCPF, pessoaFisicaUI.pessoaFisica.no_pessoa), null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                }
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                return response;
            }
            catch (AlunoBusinessException ex)
            {
                if (ex.tipoErro == AlunoBusinessException.TipoErro.ERRO_PESSOAEMPRESAJAEXISTE)
                {
                    retorno.AddMensagem(ex.Message, ex.StackTrace + ex.InnerException, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                    return response;
                    //return new RenderJsonActionResult { Result = new { erro = retorno } };
                }
                else
                {
                    retorno.AddMensagem(Messages.msgRegBuscError, ex.Message + ex.StackTrace + ex.InnerException, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                    return response;
                    //return new RenderJsonActionResult { Result = new { erro = retorno } };
                }
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "func")]
        public HttpResponseMessage existePessoaJuridicaEscolaOrByCpf(string cnpj)
        {
            ReturnResult retorno = new ReturnResult();
            int cdEscola = this.ComponentesUser.CodEmpresa.Value;
            try
            {
                IAlunoBusiness BusinessAluno = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
                PessoaJurdicaSearchUI pessoaJuridicaUI = BusinessAluno.existePessoaJuridicaEscolaOrByCNPJ(cnpj, cdEscola);
                if (pessoaJuridicaUI != null && pessoaJuridicaUI.pessoaJuridica != null)
                {
                    if (pessoaJuridicaUI.pessoaJuridica.TelefonePessoa.Count() > 0)
                    {
                        pessoaJuridicaUI.contatosUI = TelefoneUI.fromTelefoneforTelefoneUI(pessoaJuridicaUI.pessoaJuridica.TelefonePessoa);
                        pessoaJuridicaUI.pessoaJuridica.TelefonePessoa = null;
                    }
                    if (pessoaJuridicaUI.pessoaJuridica.Telefone != null)
                    {
                        pessoaJuridicaUI.pessoaJuridica.Telefone.ClasseTelefone.TelefoneClasse = null;
                        pessoaJuridicaUI.pessoaJuridica.Telefone.TelefonePessoa = null;
                        pessoaJuridicaUI.pessoaJuridica.Telefone.TelefoneTipo.TipoTelefoneRef = null;
                        pessoaJuridicaUI.pessoaJuridica.Telefone.TelefonePessoa = null;
                    }
                    //if (pessoaFisicaUI.pessoaFisica.PessoaEndereco.Count() > 0)
                    //    pessoaFisicaUI.enderecosUI = EnderecoUI.fromEnderecoforEnderecoUI(pessoaFisicaUI.pessoaFisica.PessoaEndereco, pessoaFisicaUI.pessoaFisica.cd_endereco_principal);
                    if (pessoaJuridicaUI.pessoaJuridica.PessoaPaiRelacionamento.Count() > 0){
                        pessoaJuridicaUI.relacionamentoUI = RelacionamentoUI.fromRelacionamentoforRelacionamentoUI(pessoaJuridicaUI.pessoaJuridica.PessoaPaiRelacionamento, pessoaJuridicaUI.pessoaJuridica.cd_pessoa);
                        pessoaJuridicaUI.pessoaJuridica.PessoaPaiRelacionamento = null;
                    }
                    retorno.retorno = pessoaJuridicaUI;
                    retorno.AddMensagem(string.Format(Messages.msgExistePessoaJuridicaRelacionadaCPF, pessoaJuridicaUI.pessoaJuridica.no_pessoa), null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                }
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                return response;
                //return new RenderJsonActionResult { Result = retorno };
            }
            catch (AlunoBusinessException ex)
            {
                if (ex.tipoErro == AlunoBusinessException.TipoErro.ERRO_PESSOAEMPRESAJAEXISTE)
                {
                    retorno.AddMensagem(ex.Message, ex.StackTrace + ex.InnerException, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                    return response;
                    //return new RenderJsonActionResult { Result = new { erro = retorno } };
                }
                else
                {
                    retorno.AddMensagem(Messages.msgRegBuscError, ex.Message + ex.StackTrace + ex.InnerException, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                    return response;
                    //return new RenderJsonActionResult { Result = new { erro = retorno } };
                }
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        #endregion

        #region Reajuste Anual

        [HttpComponentesAuthorize(Roles = "nmCt")]
        public HttpResponseMessage getComponentesReajusteAnual(int? cd_nome_contrato)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var lContratos = Business.getNomesContrato(NomeContratoDataAccess.TipoConsultaNomeContratoEnum.HAS_REAJUSTE_ANUAL, cd_nome_contrato, cdEscola).ToList();
                DateTime dataCorrente = DateTime.UtcNow;
                dataCorrente = SGF.Utils.ConversorUTC.ToLocalTime(dataCorrente,  this.ComponentesUser.IdFusoHorario, this.ComponentesUser.IsHorarioVerao);
                var dataRetorno = new
                {
                    dataPortugues = String.Format("{0:dd/MM/yyyy H:mm:ss}", dataCorrente),
                    dataIngles = String.Format("{0:yyyy/MM/dd H:mm:ss}", dataCorrente),
                    data = dataCorrente

                };
                retorno.retorno = new { nomesContrato = lContratos, dataCorrente = dataRetorno };
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }


        [HttpComponentesAuthorize(Roles = "reaa")]
        public HttpResponseMessage getReajusteAnualForEdit(int cd_reajuste_anual)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                IFinanceiroBusiness BusinessFinanceiro = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                ReajusteAnual reajuste = BusinessFinanceiro.getReajusteAnualForEdit((int)this.ComponentesUser.CodEmpresa, cd_reajuste_anual);
                if (reajuste != null)
                {
                    reajuste.dh_cadastro_reajuste = SGF.Utils.ConversorUTC.ToLocalTime(reajuste.dh_cadastro_reajuste, this.ComponentesUser.IdFusoHorario, this.ComponentesUser.IsHorarioVerao);
                }
                reajuste.nomesContrato = Business.getNomesContrato(NomeContratoDataAccess.TipoConsultaNomeContratoEnum.HAS_REAJUSTE_ANUAL, reajuste.cd_nome_contrato, cdEscola).ToList();
                retorno.retorno = reajuste;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
            }
        }

        #endregion

        #region Envio SMS

        //[HttpGet]
        //// Regra: Verifica se Escola tem os Parametros de SMS, se Não tiver apresenta
        //// mensagem de que é preciso configurar ou abrir conta na empresa indicada pela
        ////Fundação Fisk
        //[HttpComponentesAuthorize(Roles = "smsgest")]
        //public HttpResponseMessage VerificaParamsSms()
        //{
        //    ReturnResult retorno = new ReturnResult();
        //    try
        //    {
        //        ISecretariaBusiness smBusiness = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
        //        int cdEscola = (int) this.ComponentesUser.CodEmpresa;

        //        List<SmsParametrosEscola> listaEscolaSMS = smBusiness.getListaEscolaComParametro(cdEscola).ToList();
        //        retorno.retorno = listaEscolaSMS;
                
        //        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
        //        configureHeaderResponse(response, null);
        //        return response;
        //    }
        //    catch (Exception exe)
        //    {
        //        return this.Request.CreateResponse(HttpStatusCode.OK, exe);
        //    }
        //}

        //[HttpGet]
        //// Regra: Conforme regra, comentada na função anterior,
        //// Se já existir então mostra na Grid
        //[HttpComponentesAuthorize(Roles = "smsgest")]
        //public HttpResponseMessage listaParamsSmsEscola()
        //{
        //    ReturnResult retorno = new ReturnResult();
        //    try
        //    {
        //        ISecretariaBusiness smBusiness = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
        //        int cdEscola = (int) this.ComponentesUser.CodEmpresa;

        //        List<SmsParametrosEscola> listaEscolaSMS = smBusiness.getListaEscolaComParametro(cdEscola).ToList();
        //        retorno.retorno = listaEscolaSMS;
                
        //        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
        //        configureHeaderResponse(response, null);
        //        return response;
        //    }
        //    catch (Exception ex)
        //    {
        //        return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
        //    }
        //}

        //[HttpPost]
        //// Insere parametros em T_PARAMETROS_SMS
        //[HttpComponentesAuthorize(Roles = "smsgest")]
        //public HttpResponseMessage insereParamsSms(SmsParametrosEscola smsParametroUi)
        //{
        //    HttpResponseMessage retorno = new HttpResponseMessage();
        //    try
        //    {
        //        ISecretariaBusiness smBusiness = (ISecretariaBusiness) base.instanciarBusiness<ISecretariaBusiness>();
        //        int cdEscola = (int) this.ComponentesUser.CodEmpresa;
        //        smsParametroUi.cd_escola = cdEscola;
        //        smBusiness.postParamSmsEscola(smsParametroUi);

        //        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
        //        //base.configureHeaderResponse(response);
        //        configureHeaderResponse(response, null);
        //        return retorno;
        //    }
        //    catch (Exception exe)
        //    {
        //        return gerarLogException(Messages.msgNotIncludReg, new ReturnResult(), logger, exe);

        //    }
        //}

        //[HttpPost]
        //// edita parametros em T_PARAMETROS_SMS
        //[HttpComponentesAuthorize(Roles = "smsgest")]
        //public HttpResponseMessage atualizaParamsSms(SmsParametrosEscola smsParametrosEscola)
        //{
        //    ReturnResult retorno = new ReturnResult();
        //    int cdEscola = this.ComponentesUser.CodEmpresa.Value;

        //    try
        //    {
        //        ISecretariaBusiness smBusiness = (ISecretariaBusiness) base.instanciarBusiness<ISecretariaBusiness>();
        //        base.configuraBusiness(new List<IGenericBusiness>() {smBusiness});
        //        smsParametrosEscola.cd_escola = cdEscola;
                
        //        var novosDados = smBusiness.editParamSmsEscola(smsParametrosEscola, cdEscola);
        //        retorno.retorno = novosDados;

        //        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
        //        configureHeaderResponse(response, null);
        //        return response;
        //    }
        //    catch (Exception exe)
        //    {
        //        return gerarLogException(Messages.msgNotIncludReg, new ReturnResult(), logger, exe);

        //    }
        //}



        //[HttpComponentesAuthorize(Roles = "smsgest")]
        //public HttpResponseMessage PostdeleteParamescolaSms()
        //{
        //    ReturnResult retorno = new ReturnResult();
        //    int cdEscola = this.ComponentesUser.CodEmpresa.Value;
        //    try
        //    {
        //        ISecretariaBusiness smBusiness = (ISecretariaBusiness) base.instanciarBusiness<ISecretariaBusiness>();
        //        base.configuraBusiness(new List<IGenericBusiness>() {smBusiness});

        //        var deletado = smBusiness.deletarParamEscolarSms(cdEscola);
        //        retorno.retorno = deletado;
        //        if (!deletado)
        //        {
        //            retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
        //        }
        //        else
        //        {
        //            retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
        //        }

        //        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
        //        configureHeaderResponse(response, null);
        //        return response;
        //    }
        //    catch (Exception ex)
        //    {
        //        return gerarLogException(Messages.msgNotDeletedReg, retorno, logger, ex);
        //    }
        //}

        //// CRUD PARA "COMPOR MENSAGEM
        //[HttpPost]
        //[HttpComponentesAuthorize(Roles = "smsgest")]
        //public HttpResponseMessage insereNovaMensagemPadrao(SmSComporMensagemPadrao smsComporMensagem)
        //{
        //    HttpResponseMessage retorno = new HttpResponseMessage();
        //    try
        //    {
        //        ISecretariaBusiness smBusiness = (ISecretariaBusiness) base.instanciarBusiness<ISecretariaBusiness>();
        //        int cdEscola = (int) this.ComponentesUser.CodEmpresa;
        //        smsComporMensagem.cd_escola = cdEscola;
        //        smsComporMensagem.dt_cadastro = DateTime.Now;
        //        smBusiness.postNovaMensagemPadrao(smsComporMensagem);

        //        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
        //        configureHeaderResponse(response, null);
        //        return retorno;
        //    }
        //    catch (Exception exe)
        //    {
        //        return gerarLogException(Messages.msgNotIncludReg, new ReturnResult(), logger, exe);

        //    }
        //}

        //[HttpGet]
        //[HttpComponentesAuthorize(Roles = "smsgest")]
        //public HttpResponseMessage listaMensagensPadrao()
        //{
        //    ReturnResult retorno = new ReturnResult();
        //    try
        //    {
        //        ISecretariaBusiness smBusiness = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
        //        int cdEscola = (int) this.ComponentesUser.CodEmpresa;

        //        List<SmSComporMensagemPadrao> listaMensagensPadrao = smBusiness.getListaMensagensPadraobyEscola(cdEscola).ToList();
        //        retorno.retorno = listaMensagensPadrao;
                
        //        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
        //        configureHeaderResponse(response, null);
        //        return response;
        //    }
        //    catch (Exception ex)
        //    {
        //        return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
        //    }
        //}

        //[HttpComponentesAuthorize(Roles = "smsgest")]
        //public HttpResponseMessage deletaMensagemSms(int motivo)
        //{
        //    ReturnResult retorno = new ReturnResult();
        //    //int cdEscola = this.ComponentesUser.CodEmpresa.Value;
        //    try
        //    {
        //        var cd_escola = this.ComponentesUser.CodEmpresa.Value;
        //        ISecretariaBusiness smBusiness = (ISecretariaBusiness) base.instanciarBusiness<ISecretariaBusiness>();
        //        base.configuraBusiness(new List<IGenericBusiness>() {smBusiness});

        //        var deletado = smBusiness.deletaMensagemPadrao(cd_escola, motivo);
        //        retorno.retorno = deletado;
        //        if (!deletado)
        //        {
        //            retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
        //        }
        //        else
        //        {
        //            retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
        //        }

        //        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
        //        configureHeaderResponse(response, null);
        //        return response;
        //    }
        //    catch (Exception ex)
        //    {
        //        return gerarLogException(Messages.msgNotDeletedReg, retorno, logger, ex);
        //    }
        //}


        //[HttpPost]
        //[HttpComponentesAuthorize(Roles = "smsgest")]
        //public HttpResponseMessage atualizaMensagemPadrao(SmSComporMensagemPadrao smsComporMensagem)
        //{
        //    ReturnResult retorno = new ReturnResult();
        //    try
        //    {
        //        ISecretariaBusiness smBusiness = (ISecretariaBusiness) base.instanciarBusiness<ISecretariaBusiness>();
        //        base.configuraBusiness(new List<IGenericBusiness>() {smBusiness});
                
        //        var novosDados = smBusiness.editMensagemPadraoSms(smsComporMensagem);
        //        retorno.retorno = novosDados;

        //        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
        //        configureHeaderResponse(response, null);
        //        return response;
        //    }
        //    catch (Exception exe)
        //    {
        //        return gerarLogException(Messages.msgNotIncludReg, new ReturnResult(), logger, exe);

        //    }
        //}

        // monta lista de aniversariantes - envio manual sms
        //[HttpGet]
        //[HttpComponentesAuthorize(Roles = "smsgest")]
        //public HttpResponseMessage listaAniversariantesSms()
        //{
        //    ReturnResult retorno = new ReturnResult();
        //    try
        //    {
        //        ISecretariaBusiness smBusiness = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
        //        int cdEscola = (int) this.ComponentesUser.CodEmpresa;

        //        List<PessoaSGF> listaAniversariantes = smBusiness.getListaAniversariosPeriodo(cdEscola).ToList();
        //        retorno.retorno = listaAniversariantes;

        //        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
        //        configureHeaderResponse(response, null);
        //        return response;
        //    }
        //    catch (Exception ex)
        //    {
        //        return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
        //    }
        //}



        #endregion

        #region AlunoRestricao

        [HttpComponentesAuthorize(Roles = "orgfin")]
        public HttpResponseMessage getOrgaoFinanceiro(int status)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var mot = Business.getOrgaoFinanceiro(getStatus(status));
                retorno.retorno = mot;

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "orgfin")]
        public HttpResponseMessage getNomeAndCodigoUsuarioAtendente()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                IAuthBusiness authBiz = (IAuthBusiness)base.instanciarBusiness<IAuthBusiness>();
                var usuario = authBiz.GetNomeCodigoUsuario(User.Identity.Name);
                retorno.retorno = usuario;

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "orgfin")]
        public HttpResponseMessage getAlunoRestricaoByCdAluno(int cd_aluno)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var mot = Business.getAlunoRestricaoByCdAluno(cd_aluno);
                retorno.retorno = mot;

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        #endregion

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
                ILocalidadeBusiness BusinessLoc = (ILocalidadeBusiness)base.instanciarBusiness<ILocalidadeBusiness>();
                EnderecoSGF enderecoCad = BusinessLoc.verificaSeExisteEnderecoOuGravar(endereco);
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

        #region TransferenciaAluno
        [HttpGet]
        [HttpComponentesAuthorize(Roles = "envtran")]
        public HttpResponseMessage getEnviarTransferenciaAlunoSearch( int? cd_unidade_destino, int cd_aluno, string nm_raf, string cpf, int status_transferencia, string dataIni, string dataFim)
        {
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;

                DateTime? dt_inicial = string.IsNullOrEmpty(dataIni) ? null : (DateTime?)Convert.ToDateTime(dataIni);
                DateTime? dt_final = string.IsNullOrEmpty(dataFim) ? null : (DateTime?)Convert.ToDateTime(dataFim);
                if (cpf == null)
                    cpf = String.Empty;
                if (nm_raf == null)
                    nm_raf = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ISecretariaBusiness secretariaBiz = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var retorno = secretariaBiz.getEnviarTransferenciaAlunoSearch(parametros, cdEscola, cd_unidade_destino, cd_aluno, nm_raf, cpf, status_transferencia, dt_inicial, dt_final);
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
        [HttpComponentesAuthorize(Roles = "envtran")]
        public HttpResponseMessage getComponentesEnviarTransferenciaCad()
        {
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;

                
                
                ISecretariaBusiness secretariaBiz = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var retorno = secretariaBiz.getComponentesEnviarTransferenciaCad(cdEscola);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                base.configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "envtran")]
        [HttpComponentesAuthorize(Roles = "rectran")]
        public HttpResponseMessage getEmailUnidade(int cdEscola)
        {
            try
            {

                ISecretariaBusiness secretariaBiz = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var retorno = secretariaBiz.getEmailUnidade(cdEscola);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                base.configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "envtran")]
        [HttpComponentesAuthorize(Roles = "rectran")]
        public HttpResponseMessage getRafByAluno(int cdAluno)
        {
            try
            {

                ISecretariaBusiness secretariaBiz = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var retorno = secretariaBiz.getRafByAluno(cdAluno);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                base.configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpPost]
        [HttpComponentesAuthorize(Roles = "envtran.i")]
        public HttpResponseMessage postInsertEnviarTransferenciaAluno([FromBody] TransferenciaAluno transferenciaAlunoView)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ISecretariaBusiness BusinessSecretaria = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                transferenciaAlunoView.cd_escola_origem = cdEscola;
                TransferenciaAluno transferenciaAlunoSave = BusinessSecretaria.postInsertEnviarTransferenciaAluno(transferenciaAlunoView);
                retorno.retorno = transferenciaAlunoSave;
                if (transferenciaAlunoSave.cd_transferencia_aluno <= 0)
                    retorno.AddMensagem(Messages.msgNotUpReg, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "envtran")]
        public HttpResponseMessage getEnviarTransferenciaAlunoForEdit(int cd_transferencia_aluno)
        {
            try
            {
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ISecretariaBusiness secretariaBiz = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var retorno = secretariaBiz.getEnviarTransferenciaAlunoForEdit(cd_transferencia_aluno);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "alu.a")]
        public HttpResponseMessage postEditEnviarTransferenciaAluno([FromBody] TransferenciaAluno transferenciaAlunoView)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ISecretariaBusiness BusinessSecretaria = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;

                transferenciaAlunoView.cd_escola_origem = cdEscola;

                if (transferenciaAlunoView == null)
                {
                    throw new SecretariaBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgObjTransferenciaAlunoNulo, null,
                        SecretariaBusinessException.TipoErro.ERRO_OBJ_TRANFERENCIA_ALUNO_NULL, false);
                }

                if (transferenciaAlunoView.cd_transferencia_aluno <= 0)
                {
                    throw new SecretariaBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgCdTransferenciaMenoIgualZero, null,
                        SecretariaBusinessException.TipoErro.ERRO_CD_TRANSFERENCIA_MENOR_IGUAL_ZERO, false);
                }

                if (transferenciaAlunoView.cd_escola_destino <= 0)
                {
                    throw new SecretariaBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgCdEscolaDestinoMenorIgualZero, null,
                        SecretariaBusinessException.TipoErro.ERRO_CD_ESCOLA_DESTINO_MENOR_IGUAL_ZERO, false);
                }

                if (String.IsNullOrEmpty(transferenciaAlunoView.dc_email_origem))
                {
                    throw new SecretariaBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroDcEmailOrigemNuloOuVazio, null,
                        SecretariaBusinessException.TipoErro.ERRO_DC_EMAIL_ORIGEM_NULO_OR_EMPTY, false);
                }

                if (String.IsNullOrEmpty(transferenciaAlunoView.dc_email_destino))
                {
                    throw new SecretariaBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroDcEmailDestinoNuloOuVazio, null,
                        SecretariaBusinessException.TipoErro.ERRO_DC_EMAIL_DESTINO_NULO_OR_EMPTY, false);
                }

                if (transferenciaAlunoView != null && transferenciaAlunoView.id_status_transferencia == (int)TransferenciaAlunoDataAccess.TipoStatusTransferenciaAluno.EFETUADA)
                {
                    throw new SecretariaBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroEditTransferenciaEfetuada, null,
                        SecretariaBusinessException.TipoErro.ERRO_EDIT_TRANSFERENCIA_EFETUADA, false);
                }

                if (transferenciaAlunoView.cd_motivo_transferencia <= 0)
                {
                    throw new SecretariaBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroCdMotivoStatusTransferencia, null,
                        SecretariaBusinessException.TipoErro.ERRO_CD_MOTIVO_TRANSFERENCIA_MENOR_IGUAL_ZERO, false);
                }

                TransferenciaAluno transferenciaAlunoSave = BusinessSecretaria.postEditEnviarTransferenciaAluno(transferenciaAlunoView);
                retorno.retorno = transferenciaAlunoSave;
                if (transferenciaAlunoSave.cd_transferencia_aluno <= 0)
                    retorno.AddMensagem(Messages.msgNotUpReg, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "envtran.e")]
        public HttpResponseMessage PostDeleteTransferenciaAluno(List<TransferenciaAluno> transferenciaAluno)
        {
            ReturnResult retorno = new ReturnResult();
            int cdEscola = this.ComponentesUser.CodEmpresa.Value;
            try
            {
                ISecretariaBusiness secretariaBiz = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                base.configuraBusiness(new List<IGenericBusiness>() { secretariaBiz });

                if (transferenciaAluno != null && transferenciaAluno.Where(x => x.id_status_transferencia == (int)TransferenciaAlunoDataAccess.TipoStatusTransferenciaAluno.EFETUADA).Count() > 0)
                {
                    throw new SecretariaBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroDeleteTransferenciaEfetuada, null,
                        SecretariaBusinessException.TipoErro.ERRO_DELETE_TRANSFERENCIA_EFETUADA, false);
                }

                var deletado = secretariaBiz.deletarTransferenciaAlunos(transferenciaAluno);
                retorno.retorno = deletado;
                if (!deletado)
                {
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                else
                {
                    retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotDeletedReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "envtran")]
        [HttpPost]
        public HttpResponseMessage PostSendEmailSolicitarTransferenciaAluno(TransferenciaAluno transferenciaAluno)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ISecretariaBusiness secretariaBiz = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { secretariaBiz });
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                List<string> retornoSendEmail = secretariaBiz.sendEmailSolicitaTransferenciaAluno(transferenciaAluno);
                retorno.retorno = retornoSendEmail;
                if (retornoSendEmail.Count > 0)
                {
                    retorno.AddMensagem(retornoSendEmail.First(), null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                else
                {
                    retorno.AddMensagem("E-mail enviado com sucesso.", null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "envtran")]
        [HttpPost]
        public HttpResponseMessage PostTransferirAluno(TransferenciaAluno transferenciaAluno)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ISecretariaBusiness secretariaBiz = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { secretariaBiz });

                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                int cdusuario = (int)this.ComponentesUser.CodUsuario;
                TransferenciaAluno transf = transferenciaAluno;
                if (transferenciaAluno.historicoAlunoReport != null)
                {
                    StringBuilder parametros = new StringBuilder();
                    parametros.Append("PEscola=").Append(cdEscola)
                              .Append("&@cd_aluno=").Append(transferenciaAluno.historicoAlunoReport.cd_aluno)
                              .Append("&no_aluno=").Append(transferenciaAluno.historicoAlunoReport.no_aluno)
                              .Append("&produtos=").Append(transferenciaAluno.historicoAlunoReport.produtos)
                              .Append("&PTurmaAval=").Append(transferenciaAluno.historicoAlunoReport.turmaAvaliacao)
                              .Append("&PEstagioAval=").Append(transferenciaAluno.historicoAlunoReport.estagioAvaliacao)
                              .Append("&PTitulos=").Append(transferenciaAluno.historicoAlunoReport.statusTitulo)
                              .Append("&Pmostrarestagio=").Append(transferenciaAluno.historicoAlunoReport.mostrarEstagio)
                              .Append("&Pmostraratividade=").Append(transferenciaAluno.historicoAlunoReport.mostrarAtividade)
                              .Append("&Pmostrarobs=").Append(transferenciaAluno.historicoAlunoReport.mostrarObservacao)
                              .Append("&Pmostrarfollow=").Append(transferenciaAluno.historicoAlunoReport.mostrarFollow)
                              .Append("&Pmostraritem=").Append(transferenciaAluno.historicoAlunoReport.mostrarItem)
                              .Append("&").Append(Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO).Append("=Histórico do Aluno")
                              .Append("&").Append(Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO).Append("=").Append(FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.rtpHistoricoAluno);
                    var noUsuario = secretariaBiz.getNomeAtendente(cdusuario, cdEscola);
                    if (transferenciaAluno.no_unidade_origem != null)
                        parametros.Append("&").Append(Componentes.Utils.ReportParameter.PARAMETRO_NOME_EMPRESA + "=").Append(transferenciaAluno.no_unidade_origem);
                    if (noUsuario != null)
                        parametros.Append("&").Append(Componentes.Utils.ReportParameter.PARAMETRO_NOME_USUARIO + "=").Append(noUsuario);
                    parametros.Append("&").Append(Componentes.Utils.ReportParameter.PARAMETRO_DATA_HORA_ATUAL + "=").Append(DateTime.Now);

                    //string parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros.ToString(), System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);
                    //Passar só os parametros pois não tem url envolvida
                    string parametrosCript = HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros.ToString(), System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);


                    transf = secretariaBiz.getArquivoHistorico(transferenciaAluno, parametrosCript);
                }
                List<string> retornoSendEmail = secretariaBiz.transferirAluno(transf, cdusuario);
                retorno.retorno = retornoSendEmail;
                if (retornoSendEmail.Count > 0)
                {
                    retorno.AddMensagem(retornoSendEmail.First(), null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                else
                {
                    retorno.AddMensagem("Transferência concluída com sucesso. E-mail confirmando a transferência, enviado com sucesso.", null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "envtran")]
        [Obsolete]
        public HttpResponseMessage GetUrlRelatorioEnviarTransferencia(string sort, int direction, int cd_unidade_destino, int cd_aluno, string nm_raf, string cpf, int status_transferencia, string dataIni, string dataFim)
        {
            ReturnResult retorno = new ReturnResult();
            
            try
            {

                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                // Verifica permissão "Conta Segura".
                bool contaSegura = this.ComponentesUser.Permissao.Contains("ctsg");
                IEmpresaBusiness BusinessEmpresa = (IEmpresaBusiness)base.instanciarBusiness<IEmpresaBusiness>();
                bool masterGeral = BusinessEmpresa.VerificarMasterGeral(this.ComponentesUser.CodUsuario);
                // Pega os parâmetros do usuário para criar a url do relatório:
                string strParametrosSort = "@orientation=LANDSCAPE&@cs=" + sort + "&@so=" + direction ;
                string parametros = strParametrosSort + "&@cd_unidade_destino=" + cd_unidade_destino + "&@cd_aluno=" + cd_aluno + "&@nm_raf=" + nm_raf + "&@cpf=" + cpf + "&@status_transferencia=" + status_transferencia + "&@dataIni=" + dataIni + "&@dataFim=" + dataFim + "&@cdEscola=" + cdEscola + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=" + "Relatório de envio de transferência&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + (int)FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.EnviarTransferenciaAlunoSearch;
                //parametros = HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8);
                string parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, parametrosCript);

                configureHeaderResponse(response, null);
                return response;

            }
            catch (Exception exe)
            {
                return gerarLogException(Messages.msgErroRelatorio, retorno, logger, exe);
            }
        }


        #endregion

        #region Receber Transferencia

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "rectran")]
        public HttpResponseMessage getReceberTransferenciaAlunoSearch(int? cd_unidade_origem, string no_aluno, string nm_raf, string cpf, int status_transferencia, string dataIni, string dataFim)
        {
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;

                DateTime? dt_inicial = string.IsNullOrEmpty(dataIni) ? null : (DateTime?)Convert.ToDateTime(dataIni);
                DateTime? dt_final = string.IsNullOrEmpty(dataFim) ? null : (DateTime?)Convert.ToDateTime(dataFim);
                if (cpf == null)
                    cpf = String.Empty;
                if (nm_raf == null)
                    nm_raf = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ISecretariaBusiness secretariaBiz = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var retorno = secretariaBiz.getReceberTransferenciaAlunoSearch(parametros, cdEscola, cd_unidade_origem, no_aluno, nm_raf, cpf, status_transferencia, dt_inicial, dt_final);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "rectran")]
        [Obsolete]
        public HttpResponseMessage GetUrlRelatorioReceberTransferencia(string sort, int direction, int cd_unidade_origem, string no_aluno, string nm_raf, string cpf, int status_transferencia, string dataIni, string dataFim)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {

                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                // Verifica permissão "Conta Segura".
                bool contaSegura = this.ComponentesUser.Permissao.Contains("ctsg");
                IEmpresaBusiness BusinessEmpresa = (IEmpresaBusiness)base.instanciarBusiness<IEmpresaBusiness>();
                bool masterGeral = BusinessEmpresa.VerificarMasterGeral(this.ComponentesUser.CodUsuario);
                // Pega os parâmetros do usuário para criar a url do relatório:
                string strParametrosSort = "@orientation=LANDSCAPE&@cs=" + sort + "&@so=" + direction;
                string parametros = strParametrosSort + "&@cd_unidade_origem=" + cd_unidade_origem + "&@no_aluno=" + no_aluno + "&@nm_raf=" + nm_raf + "&@cpf=" + cpf + "&@status_transferencia=" + status_transferencia + "&@dataIni=" + dataIni + "&@dataFim=" + dataFim + "&@cdEscola=" + cdEscola + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=" + "Relatório de recebimento de transferência&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + (int)FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.ReceberTransferenciaAlunoSearch;
                //parametros = HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8);
                string parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, parametrosCript);

                configureHeaderResponse(response, null);
                return response;

            }
            catch (Exception exe)
            {
                return gerarLogException(Messages.msgErroRelatorio, retorno, logger, exe);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "rectran")]
        public HttpResponseMessage getComponentesReceberTransferenciaCad()
        {
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;



                ISecretariaBusiness secretariaBiz = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var retorno = secretariaBiz.getComponentesEnviarTransferenciaCad(cdEscola);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                base.configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "rectran")]
        public HttpResponseMessage getReceberTransferenciaAlunoForEdit(int cd_transferencia_aluno)
        {
            try
            {
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ISecretariaBusiness secretariaBiz = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                var retorno = secretariaBiz.getEnviarTransferenciaAlunoForEdit(cd_transferencia_aluno);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "rectran.a")]
        public HttpResponseMessage postEditReceberTransferenciaAluno([FromBody] TransferenciaAluno transferenciaAlunoView)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ISecretariaBusiness BusinessSecretaria = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;

                transferenciaAlunoView.cd_escola_origem = cdEscola;

                if (transferenciaAlunoView == null)
                {
                    throw new SecretariaBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgObjTransferenciaAlunoNulo, null,
                        SecretariaBusinessException.TipoErro.ERRO_OBJ_TRANFERENCIA_ALUNO_NULL, false);
                }

                if (transferenciaAlunoView.cd_transferencia_aluno <= 0)
                {
                    throw new SecretariaBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgCdTransferenciaMenoIgualZero, null,
                        SecretariaBusinessException.TipoErro.ERRO_CD_TRANSFERENCIA_MENOR_IGUAL_ZERO, false);
                }

                if (String.IsNullOrEmpty(transferenciaAlunoView.dc_email_origem))
                {
                    throw new SecretariaBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroDcEmailOrigemNuloOuVazio, null,
                        SecretariaBusinessException.TipoErro.ERRO_DC_EMAIL_ORIGEM_NULO_OR_EMPTY, false);
                }

                if (String.IsNullOrEmpty(transferenciaAlunoView.dc_email_destino))
                {
                    throw new SecretariaBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroDcEmailDestinoNuloOuVazio, null,
                        SecretariaBusinessException.TipoErro.ERRO_DC_EMAIL_DESTINO_NULO_OR_EMPTY, false);
                }

                if (transferenciaAlunoView != null && transferenciaAlunoView.id_status_transferencia == (int)TransferenciaAlunoDataAccess.TipoStatusTransferenciaAluno.EFETUADA)
                {
                    throw new SecretariaBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroEditTransferenciaEfetuada, null,
                        SecretariaBusinessException.TipoErro.ERRO_EDIT_TRANSFERENCIA_EFETUADA, false);
                }

                if (transferenciaAlunoView != null && transferenciaAlunoView.id_status_transferencia == (int)TransferenciaAlunoDataAccess.TipoStatusTransferenciaAluno.SOLICITADA)
                {
                    throw new SecretariaBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroStatusTransferenciaNaoAlterado, null,
                        SecretariaBusinessException.TipoErro.ERRO_STATUS_TRANSFERENCIA_NAO_ALTERADO, false);
                }

                TransferenciaAluno transferenciaAlunoSave = BusinessSecretaria.postEditReceberTransferenciaAluno(transferenciaAlunoView);
                retorno.retorno = transferenciaAlunoSave;
                if (transferenciaAlunoSave.cd_transferencia_aluno <= 0)
                    retorno.AddMensagem(Messages.msgNotUpReg, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }


        [HttpComponentesAuthorize(Roles = "rectran")]
        [HttpPost]
        public HttpResponseMessage PostSendEmailAprovarRecusarTransferenciaAluno(TransferenciaAluno transferenciaAluno)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ISecretariaBusiness secretariaBiz = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { secretariaBiz });
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                List<string> retornoSendEmail = secretariaBiz.sendEmailAprovarRecusarTransferenciaAluno(transferenciaAluno);
                retorno.retorno = retornoSendEmail;
                if (retornoSendEmail.Count > 0)
                {
                    retorno.AddMensagem(retornoSendEmail.First(), null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                else
                {
                    retorno.AddMensagem("E-mail enviado com sucesso.", null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotDeletedReg, retorno, logger, ex);
            }
        }
        #endregion
    }
}
       

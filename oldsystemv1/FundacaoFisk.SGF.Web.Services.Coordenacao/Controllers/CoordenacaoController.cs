using Componentes.GenericBusiness.Comum;
using Componentes.GenericController;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Services.Coordenacao.Model;
using FundacaoFisk.SGF.Utils.Messages;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Business;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;
using FundacaoFisk.SGF.Web.Services.Empresa.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Empresa.Model;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Secretaria.Business;
using FundacaoFisk.SGF.Web.Services.Secretaria.Comum;
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using FundacaoFisk.SGF.Services.Coordenacao.Business;
using FundacaoFisk.SGF.Services.Coordenacao.DataAccess;
using FundacaoFisk.SGF.Utils;
using static FundacaoFisk.SGF.Services.Coordenacao.DataAccess.PerdaMaterialDataAccess;
using FundacaoFisk.SGF.Web.Services.Financeiro.Business;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Controllers
{
    public class CoordenacaoController : ComponentesApiController
    {
        const int ATIVO = 1;

        //Declaração de Atributos
        private static readonly ILog logger = LogManager.GetLogger(typeof(CoordenacaoController));

        //Método construtor
        public CoordenacaoController()
        {
        }

        /// <summary>
        ///Início do controle das Entidades 
        /// </summary>
        /// 
        #region Estagio -- Métodos do controller, Persistência e Pesquisa

        [HttpComponentesAuthorize(Roles = "estag")]
        public HttpResponseMessage getAllEstagio()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var listEstagio = coordenacaoBiz.findAllEstagio();
                retorno.retorno = listEstagio;
                if (listEstagio.Count() <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                };
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }// try/catch
        }

        // Traz os registros pelo Id de uma classe
        [HttpComponentesAuthorize(Roles = "estag")]
        public HttpResponseMessage getEstagioById(int id)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var estagio = coordenacaoBiz.findByIdEstagio(id);
                retorno.retorno = estagio;
                if (estagio.cd_estagio <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        //Método responsável pela emissão do relatório:
        [HttpComponentesAuthorize(Roles = "estag")]
        public HttpResponseMessage GetUrlRelatorioEstagio(string sort, int direction, string desc, string abrev, bool inicio, int status, int CodP)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                // Pega os parâmetros do usuário para criar a url do relatório:
                var strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@descricao=" + desc + "&@abrev=" + abrev + "&@inicio=" + inicio + "&@status=" + status + "&@codp=" + CodP + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=" + "Relatório de Estagio&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.EstagioSearch;
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
        //Traz os registros pela descrição da Entidade
        [HttpComponentesAuthorize(Roles = "estag")]
        [HttpComponentesAuthorize(Roles = "prod")]
        public HttpResponseMessage getEstagioSearch(String desc, String abrev, bool inicio, int status, int CodP)
        {
            try
            {
                if (desc == null)
                {
                    desc = String.Empty;
                }
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var retorno = coordenacaoBiz.getDescEstagio(parametros, desc, abrev, inicio, getStatus(status), CodP);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }
        //Traz os registros pela Ordem da Entidade
        [HttpComponentesAuthorize(Roles = "estag")]
        public HttpResponseMessage getEstagioOrdem(int codP)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                List<Estagio> estagios = coordenacaoBiz.getOrdemEstagio(codP, null, null).ToList();//EstagioDataAccess.TipoConsultaEstagioEnum.HAS_ATIVO).ToList();

                // Coloca os números sequenciais para apresentação em tela, será salvo com a mesma numeração. Isso não evita buracos na numeração, mas minimiza.
                if (estagios.Count > int.Parse(byte.MaxValue.ToString()))
                    retorno.AddMensagem(Messages.msgQtdEstagioSuportadaError, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);

                // TIRANDO PARA RETORNAR O nm_ordem_estagio QUE ESTÁ NO BANCO DE DADOS
                for (int i = estagios.Count - 1, j = 1; i >= 0; i--, j++)
                {
                    if (estagios[i].nm_ordem_estagio > 0)
                        estagios[i].nm_ordem_estagio = byte.Parse((j).ToString());
                    else
                        j = j - 1;
                }
                retorno.retorno = estagios;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        //Traz os registros pela Ordem da Entidade
        [HttpComponentesAuthorize(Roles = "estag")]
        public HttpResponseMessage getEstagioOrdem(int codP, int? cd_estagio, int? tipoC)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {

                EstagioDataAccess.TipoConsultaEstagioEnum? tipoConsulta = EstagioDataAccess.TipoConsultaEstagioEnum.HAS_ATIVO;
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                List<Estagio> estagios = coordenacaoBiz.getOrdemEstagio(codP, cd_estagio, tipoConsulta).ToList();

                if (tipoC.HasValue)
                    tipoConsulta = (EstagioDataAccess.TipoConsultaEstagioEnum?)Enum.ToObject(typeof(EstagioDataAccess.TipoConsultaEstagioEnum), tipoC);

                // Coloca os números sequenciais para apresentação em tela, será salvo com a mesma numeração. Isso não evita buracos na numeração, mas minimiza.
                if (estagios.Count > int.Parse(byte.MaxValue.ToString()))
                    retorno.AddMensagem(Messages.msgQtdEstagioSuportadaError, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);

                // TIRANDO PARA RETORNAR O nm_ordem_estagio QUE ESTÁ NO BANCO DE DADOS
                for (int i = estagios.Count - 1, j = 1; i >= 0; i--, j++)
                    estagios[i].nm_ordem_estagio = byte.Parse((j).ToString());
                retorno.retorno = estagios;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "estag.a")]
        public HttpResponseMessage postEditEstagio(EstagioOrdem estagio)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var editEstagio = coordenacaoBiz.editEstagio(estagio);
                List<Estagio> edit = editEstagio.ToList();
                List<EstagioSearchUI> listestagioSearch = new List<EstagioSearchUI>();
                EstagioSearchUI estagioSearch;
                for (int i = 0; i < editEstagio.Count(); i++)
                {
                    estagioSearch = EstagioSearchUI.fromEstagio(edit[i], estagio.noProduto);
                    listestagioSearch.Add(estagioSearch);
                }
                retorno.retorno = listestagioSearch;
                if (listestagioSearch.Count() < 0)
                {
                    retorno.AddMensagem(Componentes.Utils.Messages.Messages.msgErrorSalvar, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                else
                {
                    retorno.AddMensagem(Componentes.Utils.Messages.Messages.msgSalvoSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Componentes.Utils.Messages.Messages.msgErrorSalvar, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "estag.e")]
        public HttpResponseMessage postDeleteEstagio(List<Estagio> estagios)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var delEstagio = coordenacaoBiz.deleteAllEstagio(estagios);
                retorno.retorno = delEstagio;
                if (!delEstagio)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
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

        [HttpComponentesAuthorize(Roles = "estag")]
        public HttpResponseMessage getAllEstagioByProduto(int id)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var estagio = coordenacaoBiz.getAllEstagioByProduto(id);
                retorno.retorno = estagio;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        #endregion

        #region Conceito -- Métodos do controller, Persistência e Pesquisa

        [HttpComponentesAuthorize(Roles = "conc")]
        public HttpResponseMessage getAllConceito()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var listConceito = coordenacaoBiz.findAllConceito();
                retorno.retorno = listConceito;
                if (listConceito.Count() <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                };
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }// try/catch
        }

        // Traz os registros pelo Id de uma classe
        [HttpComponentesAuthorize(Roles = "conc")]
        public HttpResponseMessage getConceitoById(int id)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var conceito = coordenacaoBiz.findByIdConceito(id);
                retorno.retorno = conceito;
                if (conceito.cd_conceito <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        //Método responsável pela emissão do relatório:
        [HttpComponentesAuthorize(Roles = "conc")]
        public HttpResponseMessage GetUrlRelatorioConceito(string sort, int direction, string desc, bool inicio, int status, int CodP)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                // Pega os parâmetros do usuário para criar a url do relatório:
                var strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@descricao=" + desc + "&@inicio=" + inicio + "&@status=" + status + "&@codp=" + CodP + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=" + "Relatório de Conceito&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.ConceitoSearch;
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

        //Traz os registros pela descrição da Entidade
        [HttpComponentesAuthorize(Roles = "conc")]
        [HttpComponentesAuthorize(Roles = "prod")]
        public HttpResponseMessage getConceitoSearch(String desc, bool inicio, int status, int CodP)
        {
            try
            {
                if (desc == null)
                {
                    desc = String.Empty;
                }
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var retorno = coordenacaoBiz.getDescConceito(parametros, desc, inicio, getStatus(status), CodP);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        //Métodos de persitência
        [HttpComponentesAuthorize(Roles = "conc.i")]
        public HttpResponseMessage postInsertConceito(ConceitoSearchUI conceito)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                var addConceito = coordenacaoBiz.addConceito(conceito, cdEscola);
                retorno.retorno = addConceito;

                if ((addConceito.cd_conceito <= 0))
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (CoordenacaoBusinessException ex)
            {
                if (ex.tipoErro == CoordenacaoBusinessException.TipoErro.ERRO_SOMA_CONCEITO_MAIOR)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                if (ex.tipoErro == CoordenacaoBusinessException.TipoErro.ERRO_CONCEITO_MAIOR)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "conc.a")]
        public HttpResponseMessage postEditConceito(ConceitoSearchUI conceito)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                var editConceito = coordenacaoBiz.editConceito(conceito, cdEscola);
                retorno.retorno = editConceito;
                if (editConceito.cd_conceito <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                else
                {
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (CoordenacaoBusinessException ex)
            {
                if (ex.tipoErro == CoordenacaoBusinessException.TipoErro.ERRO_SOMA_CONCEITO_MAIOR)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                if (ex.tipoErro == CoordenacaoBusinessException.TipoErro.ERRO_CONCEITO_MAIOR)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "conc.e")]
        public HttpResponseMessage postDeleteConceito(List<Conceito> conceitos)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var delConceito = coordenacaoBiz.deleteAllConceito(conceitos);
                retorno.retorno = delConceito;
                if (!delConceito)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
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

        [HttpComponentesAuthorize(Roles = "conc")]
        [HttpGet]
        public HttpResponseMessage returnConceitosAtivos(int idProduto, int idConceito)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var conceito = coordenacaoBiz.findConceitosAtivos(idProduto, idConceito);
                retorno.retorno = conceito;
                retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception exe)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, exe);
            }
        }

        #endregion

        #region Sala -- Métodos do controller, Persistência e Pesquisa

        //Método responsável pela emissão do relatório:
        [HttpComponentesAuthorize(Roles = "sala")]
        public HttpResponseMessage GetUrlRelatorioSala(string sort, int direction, string desc, bool inicio, int status, int cdEscola, bool online)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                // Pega os parâmetros do usuário para criar a url do relatório:
                var strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@descricao=" + desc + "&@inicio=" + inicio + "&@status=" + status + "&@cdEscola=" + cdEscola + "&@online=" + online + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=" + "Relatório de Sala&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.SalaSearch;
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

        // Traz todos os registros 
        [HttpComponentesAuthorize(Roles = "sala")]
        public HttpResponseMessage getAllSala()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var listSala = coordenacaoBiz.findAllSala();
                retorno.retorno = listSala;
                if (listSala.Count() <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                };
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }// try/catch

        }

        // Traz os registros pelo Id de uma classe
        [HttpComponentesAuthorize(Roles = "sala")]
        public HttpResponseMessage getSalaById(int id)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var sala = coordenacaoBiz.findByIdSala(id);
                retorno.retorno = sala;
                if (sala.cd_sala <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        //Traz os registros pela descrição da Entidade
        [HttpComponentesAuthorize(Roles = "sala")]
        public HttpResponseMessage getSalaSearch(String desc, bool inicio, int status, bool online)
        {

            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                if (desc == null) desc = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                IEnumerable<SalaSearchUI> retorno = coordenacaoBiz.getDescSala(parametros, desc, inicio, getStatus(status), cdEscola, online);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "sala.e")]
        public HttpResponseMessage postDeleteSala(List<Sala> salas)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                IEmpresaBusiness BusinessEmpresa = (IEmpresaBusiness)base.instanciarBusiness<IEmpresaBusiness>();

                bool masterGeral = BusinessEmpresa.VerificarMasterGeral(this.ComponentesUser.CodUsuario);

                if (!masterGeral && salas.Any(s => s.id_sala_online == true))
                    throw new CoordenacaoBusinessException(Messages.msgExclusaoFundacao, null, CoordenacaoBusinessException.TipoErro.ERRO_CAD_FERIADO_INATIVO, false);

                var delSala = coordenacaoBiz.deleteAllSala(salas);
                retorno.retorno = delSala;
                if (!delSala)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
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

        // Traz todos os registros 
        [HttpComponentesAuthorize(Roles = "sala")]
        [Obsolete]
        public HttpResponseMessage getSalaHorariosDisponiveis(String horaIni, String horaFim, String data, int? cd_atividade_extra)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEmpresa = this.ComponentesUser.CodEmpresa.Value;
                var dataAtividade = Convert.ToDateTime(data);
                TimeSpan horaInicial;
                TimeSpan horaFinal;
                AtividadeExtra atividadeSala;

                if (!String.IsNullOrEmpty(horaIni))
                    horaInicial = TimeSpan.Parse(horaIni);
                else
                    horaInicial = new TimeSpan(0);
                if (!String.IsNullOrEmpty(horaFim))
                    horaFinal = TimeSpan.Parse(horaFim);
                else
                    horaFinal = new TimeSpan(0);
                var idSala = 0;
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var listSala = coordenacaoBiz.findListSalasDiponiveis(horaInicial, horaFinal, dataAtividade, true, idSala, cdEmpresa, cd_atividade_extra);
                retorno.retorno = listSala;
                if (cd_atividade_extra.HasValue)
                {
                    //TO DO KAROL
                    atividadeSala = coordenacaoBiz.findByIdAtividadeExtraFull((int)cd_atividade_extra);
                    var existsSalaDisponivelAtvEx = listSala.Any(s => s.cd_sala == atividadeSala.cd_sala);
                    if (!existsSalaDisponivelAtvEx)
                        retorno.AddMensagem(string.Format(Messages.msgNotSalaDisponivelHorario, ""), null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                }
                else
                    if (listSala.Count() <= 0)
                        retorno.AddMensagem(Messages.msgSalaDisponivelHorario, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (System.FormatException ex)
            {
                ControllerException exe = new ControllerException("Hora Inicial: " + horaIni + " Hora Final: " + horaFim, ex);
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, exe);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "sala")]
        [Obsolete]
        public HttpResponseMessage getSalaHorariosDisponiveisAulaRep(String horaIni, String horaFim, String data, int? cd_aula_reposicao, int? cd_turma)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEmpresa = this.ComponentesUser.CodEmpresa.Value;
                var dataAtividade = Convert.ToDateTime(data);
                TimeSpan horaInicial;
                TimeSpan horaFinal;
                AulaReposicao atividadeSala;

                if (!String.IsNullOrEmpty(horaIni))
                    horaInicial = TimeSpan.Parse(horaIni);
                else
                    horaInicial = new TimeSpan(0);
                if (!String.IsNullOrEmpty(horaFim))
                    horaFinal = TimeSpan.Parse(horaFim);
                else
                    horaFinal = new TimeSpan(0);
                var idSala = 0;
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var listSala = coordenacaoBiz.findListSalasDiponiveisAulaRep(horaInicial, horaFinal, dataAtividade, true, idSala, cdEmpresa, cd_aula_reposicao, cd_turma);
                retorno.retorno = listSala;
                if (cd_aula_reposicao.HasValue)
                {
                    atividadeSala = coordenacaoBiz.findByIdAulaReposicaoFull((int)cd_aula_reposicao);
                    var existsSalaDisponivelAtvEx = listSala.Any(s => s.cd_sala == atividadeSala.cd_sala);
                    if (!existsSalaDisponivelAtvEx)
                        retorno.AddMensagem(string.Format(Messages.msgNotSalaDisponivelHorario, ""), null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                }
                else
                    if (listSala.Count() <= 0)
                        retorno.AddMensagem(Messages.msgSalaDisponivelHorario, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (System.FormatException ex)
            {
                ControllerException exe = new ControllerException("Hora Inicial: " + horaIni + " Hora Final: " + horaFim, ex);
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, exe);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpPost]
        [HttpComponentesAuthorize(Roles = "sala")]
        public HttpResponseMessage getSalasDisponiveisPorHorarios(Turma turma)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var listaSalas = coordenacaoBiz.getSalasDisponiveisPorHorarios(turma, this.ComponentesUser.CodEmpresa.Value).ToList();
                retorno.retorno = listaSalas;
                if (listaSalas.Count() <= 0)
                    retorno.AddMensagem(Messages.msgSalaNaoExistenteHorarios, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }// try/catch
        }

        [HttpPost]
        [HttpComponentesAuthorize(Roles = "sala")]
        public HttpResponseMessage getSalasDisponiveisPorHorariosByModalidadeOnline(Turma turma)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var listaSalas = coordenacaoBiz.getSalasDisponiveisPorHorariosByModalidadeOnline(turma, this.ComponentesUser.CodEmpresa.Value).ToList();
                retorno.retorno = listaSalas;
                if (listaSalas.Count() <= 0)
                    retorno.AddMensagem(Messages.msgSalaNaoExistenteHorarios, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }// try/catch
        }

        [HttpComponentesAuthorize(Roles = "sala.i")]
        public HttpResponseMessage postInsertSala(Sala sala)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {

                int cd_pessoa_escola = this.ComponentesUser.CodEmpresa.Value;
                sala.cd_pessoa_escola = cd_pessoa_escola;
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var editSala = coordenacaoBiz.addSala(sala);
                retorno.retorno = editSala;
                if (editSala.cd_sala <= 0)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
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

        [HttpComponentesAuthorize(Roles = "sala.a")]
        public HttpResponseMessage postEditSala(Sala sala)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {

                int cd_pessoa_escola = this.ComponentesUser.CodEmpresa.Value;
                sala.cd_pessoa_escola = cd_pessoa_escola;
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var editSala = coordenacaoBiz.editSala(sala);
                retorno.retorno = editSala;
                if (editSala.cd_sala <= 0)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
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


        //[HttpComponentesAuthorize(Roles = "sala")]
        //public HttpResponseMessage getSalaHorariosDisponiveis(String horaIni, String horaFim, String data, int? cd_atividade_extra)
        //{
        //    ReturnResult retorno = new ReturnResult();
        //    try
        //    {
        //        int cd_pessoa_escola = this.ComponentesUser.CodEmpresa.Value;
        //        var dataAtividade = Convert.ToDateTime(data);
        //        TimeSpan horaInicial;
        //        TimeSpan horaFinal;

        //        if (horaIni != "null") horaInicial = TimeSpan.Parse(horaIni);
        //        else horaInicial = new TimeSpan(0);
        //        if (horaFim != "null") horaFinal = TimeSpan.Parse(horaFim);
        //        else horaFinal = new TimeSpan(0);
        //        int? idSala = 0;
        //        IEnumerable<Sala> listSala = coordenacaoBiz.findListSalasDiponiveis(horaInicial, horaFinal, dataAtividade, true, idSala, cd_pessoa_escola, cd_atividade_extra);
        //        retorno.retorno = listSala;
        //        if (listSala.Count() <= 0)
        //            retorno.AddMensagem(Messages.msgSalaNaoExistenteHorarios, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
        //        return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
        //    }
        //    catch (Exception ex)
        //    {
        //        return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
        //    }// try/catch
        //}

        #endregion

        #region Produto -- Métodos do controller, Persintência e Pesquisa

        [HttpComponentesAuthorize(Roles = "prod")]
        public HttpResponseMessage getAllProduto()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var listProduto = coordenacaoBiz.findAllProduto();
                retorno.retorno = listProduto;
                if (listProduto.Count() <= 0)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }// try/catch

        }

        // Traz os registros pelo Id de uma classe
        [HttpComponentesAuthorize(Roles = "prod")]
        public HttpResponseMessage getProdutoById(int id)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var produto = coordenacaoBiz.findByIdProduto(id);
                retorno.retorno = produto;
                if (produto.cd_produto <= 0)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        //Método responsável pela emissão do relatório:
        [HttpComponentesAuthorize(Roles = "prod")]
        public HttpResponseMessage GetUrlRelatorioProduto(string sort, int direction, String desc, String abrev, bool inicio, int status)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                // Pega os parâmetros do usuário para criar a url do relatório:
                var strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@descricao=" + desc + "&@abrev=" + abrev + "&@inicio=" + inicio + "&@status=" + status + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=" + "Relatório de Produto&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.ProdutoSearch;
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

        //Traz os registros pela descrição da Entidade
        [HttpComponentesAuthorize(Roles = "prod")]
        public HttpResponseMessage getProdutoSearch(String desc, String abrev, bool inicio, int status)
        {
            try
            {
                if (desc == null)
                    desc = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var retorno = coordenacaoBiz.getDescProduto(parametros, desc, abrev, inicio, getStatus(status));
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "prod")]
        public HttpResponseMessage getProduto(ProdutoDataAccess.TipoConsultaProdutoEnum hasDependente, int? cd_produto)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var listProduto = coordenacaoBiz.findProduto(hasDependente, cd_produto, null);
                retorno.retorno = listProduto;
                if (listProduto.Count() <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                };
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }// try/catch
        }

        [HttpComponentesAuthorize(Roles = "prod")]
        public HttpResponseMessage getProdutoTurmaFollowUp(ProdutoDataAccess.TipoConsultaProdutoEnum hasDependente, int? cd_produto)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cd_escola = (int)this.ComponentesUser.CodEmpresa;
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var listProduto = coordenacaoBiz.findProduto(hasDependente, null, cd_escola);
                retorno.retorno = listProduto;
                if (listProduto.Count() <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                };
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }// try/catch
        }
        [HttpComponentesAuthorize(Roles = "niv")]
        public HttpResponseMessage getNivel(NivelDataAccess.TipoConsultaNivelEnum hasDependente)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var listNivel = coordenacaoBiz.findNivel(hasDependente);
                retorno.retorno = listNivel;
                if (listNivel.Count() <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                };
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }// try/catch
        }

        [HttpComponentesAuthorize(Roles = "prod")]
        public HttpResponseMessage getProdutoAulaPersonalizada(int cd_aluno)
        {
            ReturnResult retorno = new ReturnResult();
            int cd_escola = (int)this.ComponentesUser.CodEmpresa;
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var listProduto = coordenacaoBiz.findProdutoAulaPersonalizada(cd_aluno, cd_escola);
                retorno.retorno = listProduto;
                if (listProduto.Count() <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                };
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }// try/catch
        }

        //Métodos de persitência
        [HttpComponentesAuthorize(Roles = "prod.i")]
        public HttpResponseMessage postInsertProduto(Produto produto)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var editProduto = coordenacaoBiz.addProduto(produto);
                retorno.retorno = editProduto;

                if (editProduto.cd_produto <= 0)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
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

        [HttpComponentesAuthorize(Roles = "prod.a")]
        public HttpResponseMessage postEditProduto(Produto produto)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var editProduto = coordenacaoBiz.editProduto(produto);
                retorno.retorno = editProduto;
                if (editProduto.cd_produto <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                else
                {
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "prod.e")]
        public HttpResponseMessage postDeleteProduto(List<Produto> produtos)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var delProduto = coordenacaoBiz.deleteAllProduto(produtos);
                retorno.retorno = delProduto;
                if (!delProduto)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
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

        [HttpComponentesAuthorize(Roles = "prod")]
        public HttpResponseMessage getComponentesPesquisaRelatorioListagemProspect()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                //Pega na sessão a escola logada
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                ReportProspect prospect = new ReportProspect();
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                ISecretariaBusiness secBiz = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                prospect.listaProdutos = coordenacaoBiz.findProduto(ProdutoDataAccess.TipoConsultaProdutoEnum.HAS_PROSPECT, null, cdEscola).ToList();
                prospect.listaMotivosNaoMatricula = secBiz.getMotivosNaoMatricula().ToList();
                retorno.retorno = prospect;
                retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, retorno);
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotDeletedReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "prod")]
        public HttpResponseMessage getProdutoTabela()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                IEnumerable<Produto> produtos = coordenacaoBiz.findProdutoTabela(cdEscola);
                if (produtos.Count() <= 0)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }// try/catch

        }
        #endregion

        #region Evento -- Métodos do controller, Persintência e Pesquisa

        //Método responsável pela criação da url segura do relatório:
        [HttpComponentesAuthorize(Roles = "gest")]
        public HttpResponseMessage getUrlRelatorioEventos(int? cd_turma, int? cd_professor, int? cd_evento, int? qtd_faltas, bool falta_consecultiva, bool mais_turma_pagina,
            string dt_inicial, string dt_final, byte tipoRelatorio, bool lancada, bool infoPresenca)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                int cd_escola = (int)this.ComponentesUser.CodEmpresa;

                string parametros = "@cd_escola=" + cd_escola + "&@cd_turma=" + cd_turma + "&@cd_professor=" + cd_professor + "&@cd_evento=" + cd_evento + "&@qtd_faltas=" + qtd_faltas
                                    + "&falta_consecultiva=" + falta_consecultiva + "&mais_turma_pagina=" + mais_turma_pagina + "&@dt_inicial=" + dt_inicial + "&@dt_final=" + dt_final
                                    + "&@tipo_relatorio=" + tipoRelatorio + "&@lancada=" + lancada + "&@PInfoPresenca=" + infoPresenca;

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

        //Método responsável pela criação da url segura do relatório:
        [HttpComponentesAuthorize(Roles = "aup")]
        public HttpResponseMessage getUrlRelatorioAulaPersonalizadaEspecializado(int cd_aluno, int? cd_produto, int? cd_curso, string dt_inicial_agend, string dt_final_agend, string hr_inicial_agend,
            string hr_final_agend, string dt_inicial_lanc, string dt_final_lanc, string hr_inicial_lanc, string hr_final_lanc, string no_aluno)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                int cd_escola = (int)this.ComponentesUser.CodEmpresa;

                string parametros = "@cd_escola=" + cd_escola + "&@cd_aluno=" + cd_aluno + "&@cd_produto=" + cd_produto + "&@cd_curso=" + cd_curso + "&@dt_inicial_agend=" + dt_inicial_agend + "&@dt_final_agend=" + dt_final_agend
                                    + "&@hr_inicial_agend=" + hr_inicial_agend + "&@hr_final_agend=" + hr_final_agend + "&@dt_inicial_lanc=" + dt_inicial_lanc + "&@dt_final_lanc=" + dt_final_lanc
                                    + "&@hr_inicial_lanc=" + hr_inicial_lanc + "&@hr_final_lanc=" + hr_final_lanc + "&@no_aluno=" + no_aluno;

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

        [HttpComponentesAuthorize(Roles = "even")]
        public HttpResponseMessage getAllEvento()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var listEvento = coordenacaoBiz.findAllEvento();
                retorno.retorno = listEvento;
                if (listEvento.Count() <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                };
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }// try/catch

        }

        // Traz os registros pelo Id de uma classe
        [HttpComponentesAuthorize(Roles = "even")]
        public HttpResponseMessage getEventoById(int id)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var evento = coordenacaoBiz.findByIdEvento(id);
                retorno.retorno = evento;
                if (evento.cd_evento <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        //Método responsável pela emissão do relatório:
        [HttpComponentesAuthorize(Roles = "even")]
        public HttpResponseMessage GetUrlRelatorioEvento(string sort, int direction, String desc, bool inicio, int status)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                // Pega os parâmetros do usuário para criar a url do relatório:
                var strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@descricao=" + desc + "&@inicio=" + inicio + "&@status=" + status + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=" + "Relatório de Evento&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.EventoSearch;
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

        //Traz os registros pela descrição da Entidade
        [HttpComponentesAuthorize(Roles = "even")]
        public HttpResponseMessage getEventoSearch(String desc, bool inicio, int status)
        {
            try
            {
                if (desc == null)
                {
                    desc = String.Empty;
                }
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var retorno = coordenacaoBiz.getDescEvento(parametros, desc, inicio, getStatus(status));
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        //Métodos de persitência    
        [HttpComponentesAuthorize(Roles = "even.i")]
        public HttpResponseMessage postInsertEvento(Evento evento)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var editEvento = coordenacaoBiz.addEvento(evento);
                retorno.retorno = editEvento;

                if (editEvento.cd_evento <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                else
                {
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                logger.Error("CoordenacaoController postInsertEvento - Erro: ", ex);
                retorno.AddMensagem(Messages.msgNotIncludReg, ex.Message + ex.StackTrace + ex.InnerException, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                return Request.CreateResponse(HttpStatusCode.BadRequest, retorno);
            }
        }

        [HttpComponentesAuthorize(Roles = "even.a")]
        public HttpResponseMessage postEditEvento(Evento evento)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var editEvento = coordenacaoBiz.editEvento(evento);
                retorno.retorno = editEvento;
                if (editEvento.cd_evento <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                else
                {
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "even.e")]
        public HttpResponseMessage postDeleteEvento(List<Evento> eventos)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var delEvento = coordenacaoBiz.deleteAllEvento(eventos);
                retorno.retorno = delEvento;
                if (!delEvento)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
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
        #endregion

        #region Duração -- Métodos do controller, Persistência e Pesquisa

        [HttpComponentesAuthorize(Roles = "dur")]
        public HttpResponseMessage getAllDuracao()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var listDuracao = coordenacaoBiz.findAllDuracao();
                retorno.retorno = listDuracao;
                if (listDuracao.Count() <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                };
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }// try/catch

        }

        // Traz os registros pelo Id de uma classe
        [HttpComponentesAuthorize(Roles = "dur")]
        public HttpResponseMessage getDuracaoById(int id)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var duracao = coordenacaoBiz.findByIdDuracao(id);
                retorno.retorno = duracao;
                if (duracao.cd_duracao <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        //Método responsável pela emissão do relatório:
        [HttpComponentesAuthorize(Roles = "dur")]
        public HttpResponseMessage GetUrlRelatorioDuracao(string sort, int direction, String desc, bool inicio, int status)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                // Pega os parâmetros do usuário para criar a url do relatório:
                var strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@descricao=" + desc + "&@inicio=" + inicio + "&@status=" + status + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=" + "Relatório de Carga Horária&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.DuracaoSearch;
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

        //Traz os registros pela descrição da Entidade
        [HttpComponentesAuthorize(Roles = "dur")]
        public HttpResponseMessage getDuracaoSearch(String desc, bool inicio, int status)
        {
            try
            {
                if (desc == null)
                {
                    desc = String.Empty;
                }
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var retorno = coordenacaoBiz.getDescDuracao(parametros, desc, inicio, getStatus(status));
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }
        [HttpComponentesAuthorize(Roles = "dur")]
        public HttpResponseMessage getDuracaoTabelaPreco()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var Regime = coordenacaoBiz.getDuracaoTabelaPreco();
                retorno.retorno = Regime;
                if (Regime.Count() <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }
        //Métodos de persitência
        [HttpComponentesAuthorize(Roles = "dur.i")]
        public HttpResponseMessage postInsertDuracao(Duracao duracao)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var editDuracao = coordenacaoBiz.addDuracao(duracao);
                retorno.retorno = editDuracao;

                if ((editDuracao.cd_duracao <= 0))
                {

                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                }
                else
                {
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                }

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "dur.a")]
        public HttpResponseMessage postEditDuracao(Duracao duracao)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var editDuracao = coordenacaoBiz.editDuracao(duracao);
                retorno.retorno = editDuracao;
                if (editDuracao.cd_duracao <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                else
                {
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "dur.e")]
        public HttpResponseMessage postDeleteDuracao(List<Duracao> duracoes)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var delDuracao = coordenacaoBiz.deleteAllDuracao(duracoes);
                retorno.retorno = delDuracao;
                if (!delDuracao)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
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
        [HttpComponentesAuthorize(Roles = "dur")]
        public HttpResponseMessage getDuracoes(int? cd_duracao)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var duracoes = coordenacaoBiz.getDuracoes(DuracaoDataAccess.TipoConsultaDuracaoEnum.HAS_ATIVO, cd_duracao, null);
                retorno.retorno = duracoes;
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotDeletedReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "dur")]
        public HttpResponseMessage getDuracaoProgramacao()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var ge = coordenacaoBiz.getDuracaoProgramacao();
                retorno.retorno = ge;
                if (ge.Count() <= 0)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }


        #endregion

        #region Tipo Atividade Extra -- Métodos do controller, Persistência e Pesquisa

        [HttpComponentesAuthorize(Roles = "tavex")]
        public HttpResponseMessage getAllAtividadeExtra()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var listAtividadeExtra = coordenacaoBiz.findAllTipoAtv();
                retorno.retorno = listAtividadeExtra;
                if (listAtividadeExtra.Count() <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                };
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }// try/catch

        }

        // Traz os registros pelo Id de uma classe
        [HttpComponentesAuthorize(Roles = "tavex")]
        public HttpResponseMessage getAtividadeExtraById(int id)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var atividade = coordenacaoBiz.findByIdTipoAtv(id);
                retorno.retorno = atividade;
                if (atividade.cd_tipo_atividade_extra <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        //Traz os registros pela descrição da Entidade
        [HttpComponentesAuthorize(Roles = "tavex")]
        public HttpResponseMessage getTpAtividadeExtraSearch(String desc, bool inicio, int status)
        {
            try
            {
                if (desc == null)
                {
                    desc = String.Empty;
                }
                var request = this.Request;
                RangeHeaderValue rangeHeaderValue = this.Request.Headers.Range;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var retorno = coordenacaoBiz.getDescTipoAtv(parametros, desc, inicio, getStatus(status));
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        //Métodos de persitência
        [HttpComponentesAuthorize(Roles = "tavex.i")]
        public HttpResponseMessage postInsertAtividadeExtra(TipoAtividadeExtra atividade)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var editAtividade = coordenacaoBiz.addTipoAtv(atividade);
                retorno.retorno = editAtividade;

                if (editAtividade.cd_tipo_atividade_extra <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                }
                else
                {
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                }

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "tavex.a")]
        public HttpResponseMessage postEditAtividadeExtra(TipoAtividadeExtra atividade)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var editAtividade = coordenacaoBiz.editTipoAtv(atividade);
                retorno.retorno = editAtividade;
                if (editAtividade.cd_tipo_atividade_extra <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                else
                {
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "tavex.e")]
        public HttpResponseMessage postDeleteAtividadeExtra(List<TipoAtividadeExtra> atividades)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var delAtividade = coordenacaoBiz.deleteAllTipoAtividade(atividades);
                retorno.retorno = delAtividade;
                if (!delAtividade)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
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

        [HttpComponentesAuthorize(Roles = "tavex")]
        public HttpResponseMessage postGerarRecorrenciaAtividadeExtra(AtividadeExtraRecorrenciaUI atividade)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                List<AtividadeExtra> atividadesGeradas = coordenacaoBiz.gerarRecorrenciaAtividadeExtra(atividade, cdEscola);
                retorno.retorno = atividadesGeradas;

                if (atividadesGeradas.Count <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                }
                else
                {
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                }

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
        }

        //Método responsável pela emissão do relatório:
        [HttpComponentesAuthorize(Roles = "tavex")]
        public HttpResponseMessage GetUrlRelatorioTipoAtividadeExtra(string sort, int direction, String desc, bool inicio, int status)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                // Pega os parâmetros do usuário para criar a url do relatório:
                var strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@descricao=" + desc + "&@inicio=" + inicio + "&@status=" + status + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=" + "Relatório de Atividade Extra&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.TipoAtividadeExtraSearch;
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

        #region Participação -- Métodos do controller, Persistência e Pesquisa

        //Traz os registros pela descrição da Entidade
        [HttpComponentesAuthorize(Roles = "par")]
        public HttpResponseMessage getParticipacaoSearch(String desc, bool inicio, int status)
        {
            try
            {
                if (desc == null)
                {
                    desc = String.Empty;
                }
                var request = this.Request;
                RangeHeaderValue rangeHeaderValue = this.Request.Headers.Range;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var retorno = coordenacaoBiz.getParticipacaoSearch(parametros, desc, inicio, getStatus(status));
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "par")]
        public HttpResponseMessage getAllParticipacao()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var listParticipacao = coordenacaoBiz.findAllParticipacao();
                retorno.retorno = listParticipacao;
                if (listParticipacao.Count() <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                };
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }// try/catch
        }

        // Traz os registros pelo Id de uma classe
        [HttpComponentesAuthorize(Roles = "par")]
        public HttpResponseMessage getParticipacaoById(int id)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var participacao = coordenacaoBiz.findByIdParticipacao(id);
                retorno.retorno = participacao;
                if (participacao.cd_participacao <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        //Métodos de persitência
        [HttpComponentesAuthorize(Roles = "par.i")]
        public HttpResponseMessage postInsertParticipacao(Participacao participacao)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var editParticipacao = coordenacaoBiz.addParticipacao(participacao);
                retorno.retorno = editParticipacao;

                if (editParticipacao.cd_participacao <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                }
                else
                {
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                }

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "par.a")]
        public HttpResponseMessage postEditParticipacao(Participacao participacao)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var editParticipacao = coordenacaoBiz.editParticipacao(participacao);
                retorno.retorno = editParticipacao;
                if (editParticipacao.cd_participacao <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                else
                {
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "par.e")]
        public HttpResponseMessage postDeleteParticipacao(List<Participacao> participacoes)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var delParticipacao = coordenacaoBiz.deleteAllParticipacao(participacoes);
                retorno.retorno = delParticipacao;
                if (!delParticipacao)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
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

        //Método responsável pela emissão do relatório:
        [HttpComponentesAuthorize(Roles = "par")]
        public HttpResponseMessage GetUrlRelatorioParticipacao(string sort, int direction, String desc, bool inicio, int status)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                // Pega os parâmetros do usuário para criar a url do relatório:
                var strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@descricao=" + desc + "&@inicio=" + inicio + "&@status=" + status + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=" + "Relatório de Participação&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.ParticipacaoSearch;
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

        #region Nível -- Métodos do controller, Persistência e Pesquisa
        //Traz os registros pela descrição da Entidade
        [HttpComponentesAuthorize(Roles = "niv")]
        public HttpResponseMessage getNivelSearch(String desc, bool inicio, int status)
        {
            try
            {
                if (desc == null)
                {
                    desc = String.Empty;
                }
                var request = this.Request;
                RangeHeaderValue rangeHeaderValue = this.Request.Headers.Range;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var retorno = coordenacaoBiz.getNivelSearch(parametros, desc, inicio, getStatus(status));
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "niv")]
        public HttpResponseMessage getAllNivel()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var listNivel = coordenacaoBiz.findAllNivel();
                retorno.retorno = listNivel;
                if (listNivel.Count() <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                };
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }// try/catch
        }

        // Traz os registros pelo Id de uma classe
        [HttpComponentesAuthorize(Roles = "niv")]
        public HttpResponseMessage getNivelById(int id)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var nivel = coordenacaoBiz.findByIdNivel(id);
                retorno.retorno = nivel;
                if (nivel.cd_nivel <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        //Métodos de persitência
        [HttpComponentesAuthorize(Roles = "niv.i")]
        public HttpResponseMessage postInsertNivel(Nivel nivel)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var editNivel = coordenacaoBiz.addNivel(nivel);
                retorno.retorno = editNivel;

                if (editNivel.cd_nivel <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                }
                else
                {
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                }

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "niv.a")]
        public HttpResponseMessage postEditNivel(Nivel nivel)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var editNivel = coordenacaoBiz.editNivel(nivel);
                retorno.retorno = editNivel;
                if (editNivel.cd_nivel <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                else
                {
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "niv.e")]
        public HttpResponseMessage postDeleteNivel(List<Nivel> niveis)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var delNivel = coordenacaoBiz.deleteAllNivel(niveis);
                retorno.retorno = delNivel;
                if (!delNivel)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
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

        //Método responsável pela emissão do relatório:
        [HttpComponentesAuthorize(Roles = "niv")]
        public HttpResponseMessage GetUrlRelatorioNivel(string sort, int direction, String desc, bool inicio, int status)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                // Pega os parâmetros do usuário  criar a url do relatório:
                var strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@descricao=" + desc + "&@inicio=" + inicio + "&@status=" + status + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=" + "Relatório de Nível&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.NivelSearch;
                //parâmetros = HttpUtility.UrlEncode(parâmetros, System.Text.Encoding.UTF8);
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

        #region CargaProfessor -- Métodos do controller, Persistência e Pesquisa

        //Traz os registros pela descrição da Entidade
        [HttpComponentesAuthorize(Roles = "chpro")]
        public HttpResponseMessage getCargaProfessorSearch(int qtd_minutos_duracao)
        {
            try
            {
                var request = this.Request;
                RangeHeaderValue rangeHeaderValue = this.Request.Headers.Range;
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var retorno = coordenacaoBiz.getCargaProfessorSearch(parametros, qtd_minutos_duracao, cdEscola);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }


        [HttpComponentesAuthorize(Roles = "chpro")]
        public HttpResponseMessage getAllCargaProfessor()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                var listCargaProfessor = coordenacaoBiz.findCargaProfessorAll(cdEscola);
                retorno.retorno = listCargaProfessor;
                if (listCargaProfessor.Count() <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                };
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }// try/catch
        }

        // Traz os registros pelo Id de uma classe
        [HttpComponentesAuthorize(Roles = "chpro")]
        public HttpResponseMessage getCargaProfessorById(int id)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                var cargaProfessor = coordenacaoBiz.findCargaProfessorById(id, cdEscola);
                retorno.retorno = cargaProfessor;
                if (cargaProfessor.cd_carga_professor <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        //Métodos de persitência
        [HttpComponentesAuthorize(Roles = "dur")]
        [HttpComponentesAuthorize(Roles = "chpro.i")]
        public HttpResponseMessage postInsertCargaProfessor(CargaProfessor cargaProfessor)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                cargaProfessor.cd_escola = cdEscola;
                var editCargaProfessor = CargaProfessorSearchUI.fromCargaProfessor(coordenacaoBiz.addCargaProfessor(cargaProfessor, cdEscola));
                retorno.retorno = editCargaProfessor;

                if (editCargaProfessor.cd_carga_professor <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                }
                else
                {
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                }

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "dur")]
        [HttpComponentesAuthorize(Roles = "chpro.a")]
        public HttpResponseMessage postEditCargaProfessor(CargaProfessor cargaProfessor)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                cargaProfessor.cd_escola = cdEscola;
                var editCargaProfessor = CargaProfessorSearchUI.fromCargaProfessor(coordenacaoBiz.editCargaProfessor(cargaProfessor, cdEscola));
                retorno.retorno = editCargaProfessor;
                if (editCargaProfessor.cd_carga_professor <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                else
                {
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "dur")]
        [HttpComponentesAuthorize(Roles = "chpro.e")]
        public HttpResponseMessage postDeleteCargaProfessor(List<CargaProfessor> cargaProfessores)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                var delCargaProfessor = coordenacaoBiz.deleteAllCargaProfessor(cargaProfessores, cdEscola);
                retorno.retorno = delCargaProfessor;
                if (!delCargaProfessor)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
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

        //Método responsável pela emissão do relatório:
        [HttpComponentesAuthorize(Roles = "chpro")]
        public HttpResponseMessage GetUrlRelatorioCargaProfessor(string sort, int direction, int qtd_minutos_duracao)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                // Pega os parâmetros do usuário para criar a url do relatório:
                var strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@qtd_minutos_duracao=" + qtd_minutos_duracao + "&@cd_escola=" + cdEscola + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=" + "Relatório de Carga Horária Professor&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.CargaProfessorSearch;
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

        #region Motivo Desistência -- Métodos do controller, Persistência e Pesquisa

        [HttpComponentesAuthorize(Roles = "mtdes")]
        public HttpResponseMessage getAllMotivoDesistencia()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var listMotivoDesitencia = coordenacaoBiz.findAllMotivoDesistencia();
                retorno.retorno = listMotivoDesitencia;
                if (listMotivoDesitencia.Count() <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                };
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }// try/catch

        }

        // Traz os registros pelo Id de uma classe
        [HttpComponentesAuthorize(Roles = "mtdes")]
        public HttpResponseMessage getMotivoDesistenciaById(int id)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var MotivoDesitencia = coordenacaoBiz.findByIdMotivoDesistencia(id);
                retorno.retorno = MotivoDesitencia;
                if (MotivoDesitencia.cd_motivo_desistencia <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        //Método responsável pela emissão do relatório:
        [HttpComponentesAuthorize(Roles = "mtdes")]
        public HttpResponseMessage GetUrlRelatorioMotivoDesistencia(string sort, int direction, String desc, bool inicio, int status, bool isCancelamento)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                // Pega os parâmetros do usuário para criar a url do relatório:
                var strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@descricao=" + desc + "&@inicio=" + inicio + "&@status=" + status + "&@isCancelamento=" + isCancelamento + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=" + "Relatório de Motivo de Desistência&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.MotivoDesistenciaSearch;
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

        //Traz os registros pela descrição da Entidade
        [HttpComponentesAuthorize(Roles = "mtdes")]
        public HttpResponseMessage getMotivoDesistenciaSearch(String desc, bool inicio, int status, bool isCancelamento)
        {
            try
            {
                if (desc == null)
                {
                    desc = String.Empty;
                }
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var retorno = coordenacaoBiz.getDescMotivoDesistencia(parametros, desc, inicio, getStatus(status), isCancelamento);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        //Métodos de persitência
        [HttpComponentesAuthorize(Roles = "mtdes.i")]
        public HttpResponseMessage postInsertMotivoDesistencia(MotivoDesistencia motivo)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var editMotivoDesitencia = coordenacaoBiz.addMotivoDesistencia(motivo);
                retorno.retorno = editMotivoDesitencia;

                if (editMotivoDesitencia.cd_motivo_desistencia <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                else
                {
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "mtdes.a")]
        public HttpResponseMessage postEditMotivoDesistencia(MotivoDesistencia motivo)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var editMotivoDesitencia = coordenacaoBiz.editMotivoDesistencia(motivo);
                retorno.retorno = editMotivoDesitencia;
                if (editMotivoDesitencia.cd_motivo_desistencia <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                else
                {
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "mtdes.e")]
        public HttpResponseMessage postDeleteMotivoDesistencia(List<MotivoDesistencia> motivos)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var delMotivoDesitencia = coordenacaoBiz.deleteAllMotivoDesistencia(motivos);
                retorno.retorno = delMotivoDesitencia;
                if (!delMotivoDesitencia)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
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

        [HttpComponentesAuthorize(Roles = "mtdes")]
        public HttpResponseMessage getMotivosDesistencia()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var MotivoDesitencia = coordenacaoBiz.motivosDesistenciaWhitDesistencia(cdEscola).ToList();
                retorno.retorno = MotivoDesitencia;
                if (MotivoDesitencia == null)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "mtdes")]
        public HttpResponseMessage getMotivoDesistenciaByCancelamento(bool isCancelamento)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var listMotivoDesitencia = coordenacaoBiz.getMotivoDesistenciaByCancelamento(isCancelamento);
                retorno.retorno = listMotivoDesitencia;
                if (listMotivoDesitencia.Count() <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                };
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }// try/catch

        }

        #endregion

        #region Motivo Falta -- Métodos do controller, Persistência e Pesquisa

        [HttpComponentesAuthorize(Roles = "mtfal")]
        public HttpResponseMessage getAllMotivoFalta()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var listMotivoFalta = coordenacaoBiz.findAllMotivoFalta();
                retorno.retorno = listMotivoFalta;
                if (listMotivoFalta.Count() <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                };
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }// try/catch
        }
        [HttpComponentesAuthorize(Roles = "mtfal")]
        public HttpResponseMessage getMotivoFaltaAtivo(int cdDiario)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var listMotivoFalta = coordenacaoBiz.getMotivoFaltaAtivo(cdDiario);
                retorno.retorno = listMotivoFalta;
                if (listMotivoFalta.Count() <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                };
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }// try/catch
        }

        // Traz os registros pelo Id de uma classe
        [HttpComponentesAuthorize(Roles = "mtfal")]
        public HttpResponseMessage getMotivoFaltaById(int id)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var MotivoFalta = coordenacaoBiz.findByIdMotivoFalta(id);
                retorno.retorno = MotivoFalta;
                if (MotivoFalta.cd_motivo_falta <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        //Método responsável pela emissão do relatório:
        [HttpComponentesAuthorize(Roles = "mtfal")]
        public HttpResponseMessage GetUrlRelatorioMotivoFalta(string sort, int direction, String desc, bool inicio, int status)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                // Pega os parâmetros do usuário para criar a url do relatório:
                var strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@descricao=" + desc + "&@inicio=" + inicio + "&@status=" + status + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=" + "Relatório de Motivo de Falta&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.MotivoFaltaSearch;
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

        //Traz os registros pela descrição da Entidade
        [HttpComponentesAuthorize(Roles = "mtfal")]
        public HttpResponseMessage getMotivoFaltaSearch(String desc, bool inicio, int status)
        {
            try
            {
                if (desc == null)
                {
                    desc = String.Empty;
                }
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var retorno = coordenacaoBiz.getDescMotivoFalta(parametros, desc, inicio, getStatus(status));
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        //Métodos de persitência
        [HttpComponentesAuthorize(Roles = "mtfal.i")]
        public HttpResponseMessage postInsertMotivoFalta(MotivoFalta motivo)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var editMotivoFalta = coordenacaoBiz.addMotivoFalta(motivo);
                retorno.retorno = editMotivoFalta;

                if (editMotivoFalta.cd_motivo_falta <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                else
                {
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "mtfal.a")]
        public HttpResponseMessage postEditMotivoFalta(MotivoFalta motivo)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var editMotivoFalta = coordenacaoBiz.editMotivoFalta(motivo);
                retorno.retorno = editMotivoFalta;
                if (editMotivoFalta.cd_motivo_falta <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                else
                {
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "mtfal.e")]
        public HttpResponseMessage postDeleteMotivoFalta(List<MotivoFalta> motivos)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var delMotivoFalta = coordenacaoBiz.deleteAllMotivoFalta(motivos);
                retorno.retorno = delMotivoFalta;
                if (!delMotivoFalta)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
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
        #endregion

        #region Modalidade -- Métodos do controller, Persistência e Pesquisa

        [HttpComponentesAuthorize(Roles = "mod")]
        public HttpResponseMessage getAllModalidade()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var listModalidade = coordenacaoBiz.findAllModalidade();
                retorno.retorno = listModalidade;
                if (listModalidade.Count() <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                };
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }// try/catch

        }

        // Traz os registros pelo Id de uma classe
        [HttpComponentesAuthorize(Roles = "mod")]
        public HttpResponseMessage getModalidadeById(int id)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var Modalidade = coordenacaoBiz.findByIdModalidade(id);
                retorno.retorno = Modalidade;
                if (Modalidade.cd_modalidade <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        //Método responsável pela emissão do relatório:
        [HttpComponentesAuthorize(Roles = "mod")]
        public HttpResponseMessage GetUrlRelatorioModalidade(string sort, int direction, String desc, bool inicio, int status)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                // Pega os parâmetros do usuário para criar a url do relatório:
                var strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@descricao=" + desc + "&@inicio=" + inicio + "&@status=" + status + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=" + "Relatório de Modalidade&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.ModalidadeSearch;
                //parametros = HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8);
                string parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);

                HttpResponseMessage response = response = Request.CreateResponse(HttpStatusCode.OK, parametrosCript);

                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception exe)
            {
                return gerarLogException(Messages.msgErroRelatorio, retorno, logger, exe);
            }
        }

        //Traz os registros pela descrição da Entidade
        [HttpComponentesAuthorize(Roles = "mod")]
        public HttpResponseMessage getModalidadeSearch(String desc, bool inicio, int status)
        {
            try
            {
                if (desc == null)
                {
                    desc = String.Empty;
                }
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var retorno = coordenacaoBiz.getDescModalidade(parametros, desc, inicio, getStatus(status));
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "mod")]
        public HttpResponseMessage getModalidades(int? criterios, int? cd_modalidade)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ModalidadeDataAccess.TipoConsultaModalidadeEnum? tipoConsulta = null;

                if (criterios != null)
                    tipoConsulta = (ModalidadeDataAccess.TipoConsultaModalidadeEnum?)Enum.ToObject(typeof(ModalidadeDataAccess.TipoConsultaModalidadeEnum), criterios);
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var modalidades = coordenacaoBiz.getModalidades(tipoConsulta, cd_modalidade);
                retorno.retorno = modalidades;
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
        }

        //Métodos de persitência
        [HttpComponentesAuthorize(Roles = "mod.i")]
        public HttpResponseMessage postInsertModalidade(Modalidade modalidade)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var editModalidade = coordenacaoBiz.addModalidade(modalidade);
                retorno.retorno = editModalidade;

                if (editModalidade.cd_modalidade <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                else
                {
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                }

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "mod.a")]
        public HttpResponseMessage postEditModalidade(Modalidade motivo)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var editModalidade = coordenacaoBiz.editModalidade(motivo);
                retorno.retorno = editModalidade;
                if (editModalidade.cd_modalidade <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                else
                {
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "mod.e")]
        public HttpResponseMessage postDeleteModalidade(List<Modalidade> modalidades)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var delModalidade = coordenacaoBiz.deleteAllModalidade(modalidades);
                retorno.retorno = delModalidade;
                if (!delModalidade)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
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
        #endregion

        #region Regime -- Métodos do controller, Persistência e Pesquisa

        [HttpComponentesAuthorize(Roles = "reg")]
        public HttpResponseMessage getAllRegime()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var listRegime = coordenacaoBiz.findAllRegime();
                retorno.retorno = listRegime;
                if (listRegime.Count() <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                };
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }// try/catch

        }

        // Traz os registros pelo Id de uma classe
        [HttpComponentesAuthorize(Roles = "reg")]
        public HttpResponseMessage getRegimeById(int id)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var Regime = coordenacaoBiz.findByIdRegime(id);
                retorno.retorno = Regime;
                if (Regime.cd_regime <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "reg")]
        public HttpResponseMessage getRegimeTabelaPreco()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var Regime = coordenacaoBiz.getRegimeTabelaPreco();
                retorno.retorno = Regime;
                if (Regime.Count() <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        //Método responsável pela emissão do relatório:
        [HttpComponentesAuthorize(Roles = "reg")]
        public HttpResponseMessage GetUrlRelatorioRegime(string sort, int direction, string desc, string abrev, bool inicio, int status)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                // Pega os parâmetros do usuário para criar a url do relatório:
                var strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@descricao=" + desc + "&@abrev=" + abrev + "&@inicio=" + inicio + "&@status=" + status + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=" + "Relatório de Modalidade&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.RegimeSearch;
                //parametros = HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8);
                string parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);

                return Request.CreateResponse(HttpStatusCode.OK, parametrosCript);
            }
            catch (Exception exe)
            {
                return gerarLogException(Messages.msgErroRelatorio, retorno, logger, exe);
            }
        }

        //Traz os registros pela descrição da Entidade
        [HttpComponentesAuthorize(Roles = "reg")]
        public HttpResponseMessage getRegimeSearch(String desc, String abrev, bool inicio, int status)
        {
            try
            {
                if (desc == null)
                {
                    desc = String.Empty;
                }
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var retorno = coordenacaoBiz.getDescRegime(parametros, desc, abrev, inicio, getStatus(status));
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        //Métodos de persitência
        [HttpComponentesAuthorize(Roles = "reg.i")]
        public HttpResponseMessage postInsertRegime(Regime regime)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var editRegime = coordenacaoBiz.addRegime(regime);
                retorno.retorno = editRegime;

                if (editRegime.cd_regime <= 0)
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
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "reg.a")]
        public HttpResponseMessage postEditRegime(Regime regime)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var editRegime = coordenacaoBiz.editRegime(regime);
                retorno.retorno = editRegime;
                if (editRegime.cd_regime <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                else
                {
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "reg.e")]
        public HttpResponseMessage postDeleteRegime(List<Regime> regimes)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var delRegime = coordenacaoBiz.deleteAllRegime(regimes);
                retorno.retorno = delRegime;
                if (!delRegime)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                else
                {
                    retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotDeletedReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "reg")]
        public HttpResponseMessage getRegimes(int status, int? cd_regime)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var regimes = coordenacaoBiz.getRegimes(RegimeDataAccess.TipoConsultaRegimeEnum.HAS_ATIVO, cd_regime);
                retorno.retorno = regimes;
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotDeletedReg, retorno, logger, ex);
            }
        }

        #endregion

        #region Feriado -- Métodos do controller, Persistência e Pesquisa

        // Traz os registros pelo Id de uma classe
        [HttpComponentesAuthorize(Roles = "Fer")]
        public HttpResponseMessage getFeriadoById(int id)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var Feriado = coordenacaoBiz.findByIdFeriado(id);
                retorno.retorno = Feriado;
                if (Feriado.cod_feriado <= 0)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        //Método responsável pela emissão do relatório:
        [HttpComponentesAuthorize(Roles = "Fer")]
        public HttpResponseMessage GetUrlRelatorioFeriado(string sort, int direction, String desc, bool inicio, int status, int? cdEscola, int Ano, int Mes, int Dia, int AnoFim, int MesFim, int DiaFim, int somenteAno, bool idFeriadoAtivo)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                // Pega os parâmetros do usuário para criar a url do relatório:
                var strParametrosSort = "@orientation=LANDSCAPE&@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@descricao=" + desc + "&@idFeriadoAtivo=" + idFeriadoAtivo + "&@inicio=" + inicio + "&@status=" + status + "&@cdEscola=" + cdEscola + "&@Ano=" + Ano + "&@Mes=" + Mes + "&@Dia=" + Dia + "&@AnoFim=" + AnoFim + "&@MesFim=" + MesFim + "&@DiaFim=" + DiaFim + "&@SomenteAno=" + somenteAno + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=" + "Relatório de Feriado&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.FeriadoSearch;
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

        [HttpComponentesAuthorize(Roles = "fer.e")]
        public HttpResponseMessage postDeleteFeriado(List<Feriado> feriados)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                //configuraBusiness(new List<IGenericBusiness>() { coordenacaoBiz });
                Boolean refez_feriados = false;
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var delFeriado = coordenacaoBiz.deleteAllFeriado(feriados, ComponentesUser.CodEmpresa.Value, ComponentesUser.Identity.Name, ref refez_feriados, this.ComponentesUser.CodUsuario);

                if (refez_feriados)
                    retorno.AddMensagem(Messages.msgProgramacoesRefeitasPeloFeriado, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);

                retorno.retorno = delFeriado;

                if (!delFeriado)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                else
                    retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (TurmaBusinessException ex)
            {
                return gerarLogException(ex.Message, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotDeletedReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "fer")]
        public HttpResponseMessage getFeriadoSearch(String desc, bool inicio, int status, int Ano, int Mes,
            int Dia, int AnoFim, int MesFim, int DiaFim, int somenteAno, bool idFeriadoAtivo)
        {
            try
            {
                int cdEmpresa = this.ComponentesUser.CodEmpresa.Value;
                if (desc == null)
                    desc = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                IEnumerable<Feriado> retorno = coordenacaoBiz.getDescFeriado(parametros, desc, inicio, getStatus(status), cdEmpresa, Ano, Mes, Dia, AnoFim, MesFim, DiaFim, getStatus(somenteAno), idFeriadoAtivo);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }
        [HttpComponentesAuthorize(Roles = "fer")]
        public HttpResponseMessage GetUrlRelatorioFeriado(string sort, int direction, String desc, bool inicio, int status, int Ano, int Mes, int Dia, int AnoFim, int MesFim, int DiaFim, int somenteAno, bool idFeriadoAtivo)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                int cdEmpresa = this.ComponentesUser.CodEmpresa.Value;
                // Pega os parâmetros do usuário para criar a url do relatório:
                //string parametros = "@nome=" + nome + "&@apelido=" + apelido + "&@status=" + status + "&@tipoPessoa=" + tipoPessoa + "&@cnpjCpf=" + cnpjCpf + "&@papel=" + papel + "&@sexo=" + sexo + "&@inicio=" + inicio + "&@cdEscola=" + cdEscola + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=" + "Relatório de Pessoa&" +
                //    Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + (int)FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.PessoaSearch;

                // Pega os parâmetros do usuário para criar a url do relatório:
                string strParametrosSort = "@orientation=LANDSCAPE&@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@descricao=" + desc + "&@idFeriadoAtivo=" + idFeriadoAtivo + "&@inicio=" + inicio + "&@status=" + status + "&@cdEscola=" + cdEmpresa + "&@Ano=" + Ano + "&@Mes=" + Mes + "&@Dia=" + Dia + "&@AnoFim=" + AnoFim + "&@MesFim=" + MesFim + "&@DiaFim=" + DiaFim + "&@SomenteAno=" + somenteAno + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=" + "Relatório de Feriado&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.FeriadoSearch;

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

        [HttpComponentesAuthorize(Roles = "fer.i")]
        [HttpComponentesAuthorize(Roles = "tur.a")]
        public HttpResponseMessage postInsertFeriado(Feriado feriado)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                logger.Warn("Iniciou a inclusão do feriado.");
                //configuraBusiness(new List<IGenericBusiness>() { coordenacaoBiz });

                if (!feriado.id_feriado_ativo)
                    throw new CoordenacaoBusinessException(Messages.msgErroCadFeriadoInativo, null, CoordenacaoBusinessException.TipoErro.ERRO_CAD_FERIADO_INATIVO, false);

                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                //var isMaster =  BusinessEmpresa.VerificarMasterGeral(this.ComponentesUser.)
                //Pega na sessão a escola logada
                Boolean refez_feriados = false;

                feriado.cd_pessoa_escola = cdEscola;
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var feriadoAdd = coordenacaoBiz.addFeriado(feriado, cdEscola, ComponentesUser.Identity.Name, ref refez_feriados, this.ComponentesUser.CodUsuario);

                if (refez_feriados)
                    retorno.AddMensagem(Messages.msgProgramacoesRefeitasPeloFeriado, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);

                //Remove as referências circulares:
                feriadoAdd.Feriados = null;

                retorno.retorno = feriadoAdd;

                if (feriadoAdd.cod_feriado <= 0)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                logger.Warn("Finalizou a inclusão do feriado.");
                return response;
            }

            catch (CoordenacaoBusinessException ex)
            {
                if (ex.tipoErro == CoordenacaoBusinessException.TipoErro.PERIODO_EXISTENTE)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                if (ex.tipoErro == CoordenacaoBusinessException.TipoErro.ERRO_CAD_FERIADO_INATIVO)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "fer.a")]
        [HttpComponentesAuthorize(Roles = "tur.a")]
        public HttpResponseMessage postEditFeriado(Feriado feriado)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                Boolean refez_feriados = false;

                //configuraBusiness(new List<IGenericBusiness>() { coordenacaoBiz });
                feriado.cd_pessoa_escola = cdEscola;
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var editFeriado = coordenacaoBiz.editFeriado(feriado, cdEscola, ComponentesUser.Identity.Name, ref refez_feriados, this.ComponentesUser.CodUsuario);

                if (refez_feriados)
                    retorno.AddMensagem(Messages.msgProgramacoesRefeitasPeloFeriado, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);

                //Remove as referências circulares:
                editFeriado.Feriados = null;

                retorno.retorno = editFeriado;

                if (editFeriado.cod_feriado <= 0)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (CoordenacaoBusinessException ex)
            {
                if (ex.tipoErro == CoordenacaoBusinessException.TipoErro.PERIODO_EXISTENTE)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                if (ex.tipoErro == CoordenacaoBusinessException.TipoErro.ERRO_STATUS_FERIADO)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
        }
        #endregion

        #region Métodos/Regras para Avaliação

        #region Critérios de Avaliações

        // GET api/<controller>
        [HttpComponentesAuthorize(Roles = "crit")]
        public HttpResponseMessage getCriterioAvaliacaoSearch(string descricao, string abrev, bool inicio, int status, int conceito, bool IsParticipacao)
        {
            try
            {
                if (descricao == null)
                    descricao = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                List<CriterioAvaliacao> retorno = coordenacaoBiz.getCriterioAvaliacaoSearch(parametros, descricao, abrev, inicio, getStatus(status), getStatus(conceito), IsParticipacao).ToList();
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
        [HttpComponentesAuthorize(Roles = "crit")]
        public HttpResponseMessage GetCriterioAvaliacaoById(int id)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var ge = coordenacaoBiz.getCriterioAvaliacaoById(id);
                retorno.retorno = ge;
                if (ge.cd_criterio_avaliacao <= 0)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        // Put api/<controller>/5
        [HttpComponentesAuthorize(Roles = "crit.a")]
        public HttpResponseMessage PostAlterarCriterioAvaliacao(CriterioAvaliacao criterioAvaliacao)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var ge = coordenacaoBiz.putCriterioAvaliacao(criterioAvaliacao);
                retorno.retorno = ge;
                if (ge.cd_criterio_avaliacao <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (CoordenacaoBusinessException ex)
            {
                if (ex.tipoErro == CoordenacaoBusinessException.TipoErro.ERRO_IS_PARTICIPACAO_AVALIACAO)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }

        }
        // Post api/<controller>/5
        [HttpComponentesAuthorize(Roles = "crit.i")]
        public HttpResponseMessage PostCriterioAvaliacao(CriterioAvaliacao criterioAvaliacao)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var ge = coordenacaoBiz.postCriterioAvaliacao(criterioAvaliacao);
                retorno.retorno = ge;
                if (ge.cd_criterio_avaliacao <= 0)
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
        [HttpComponentesAuthorize(Roles = "crit.e")]
        public HttpResponseMessage PostDeleteCriterioAvaliacao(List<CriterioAvaliacao> criteriosAvaliacao)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var deleted = coordenacaoBiz.deleteAllCriterioAvaliacao(criteriosAvaliacao);
                retorno.retorno = deleted;
                if (!deleted)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
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

        //Método responsável pela emissão do relatório:
        [HttpComponentesAuthorize(Roles = "crit")]
        public HttpResponseMessage GetUrlRelatorioCriterioAvaliacao(string sort, int direction, string descricao, string abrev, bool inicio, int status, int conceito, bool IsParticipacao)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                // Pega os parâmetros do usuário para criar a url do relatório:
                string strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@descricao=" + descricao + "&@abrev=" + abrev + "&@inicio=" + inicio + "&@status=" + status + "&@conceito=" + conceito
                    + "&@IsParticipacao=" + IsParticipacao + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=" + "Relatório de Nome de Avaliação&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.CriterioAvaliacaoSearch;
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

        [HttpComponentesAuthorize(Roles = "crit")]
        public HttpResponseMessage getAllCriteriosAtivos(bool? ativo, int? cdCriterio)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var list = coordenacaoBiz.getAllCriteriosAtivos(ativo, cdCriterio);
                retorno.retorno = list;
                if (list.Count() <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                };
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));

            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "crit")]
        public HttpResponseMessage getAvaliacaoCriterio(int cd_tipo_avaliacao, int? cd_criterio_avaliacao)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                cd_criterio_avaliacao = cd_criterio_avaliacao == 0 ? null : cd_criterio_avaliacao;
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var list = coordenacaoBiz.getAvaliacaoCriterio(cd_tipo_avaliacao, cd_criterio_avaliacao);
                retorno.retorno = list;
                if (list.Count() <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                };
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "crit")]
        public HttpResponseMessage getNomesCriterio()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var nomeAvaliacao = coordenacaoBiz.getNomesAvaliacao();
                retorno.retorno = nomeAvaliacao;
                if (nomeAvaliacao.Count() <= 0)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "crit")]
        public HttpResponseMessage getNomesCriterio(int cd_tipo_avaliacao)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var nomeAvaliacao = coordenacaoBiz.getNomesAvaliacao(cd_tipo_avaliacao);
                retorno.retorno = nomeAvaliacao;
                if (nomeAvaliacao.Count() <= 0)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }
        #endregion

        #region Tipos de Avaliações

        // GET api/<controller>
        [HttpComponentesAuthorize(Roles = "tpav")]
        public HttpResponseMessage getTipoAvaliacaoSearch(string descricao, bool inicio, int status, int cd_tipo_avaliacao, int cd_criterio_avaliacao, int cdCurso, int cdProduto)
        {
            try
            {
                if (descricao == null)
                    descricao = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var retorno = coordenacaoBiz.getTipoAvaliacaoSearch(parametros, descricao, inicio, getStatus(status), cd_tipo_avaliacao, cd_criterio_avaliacao, cdCurso, cdProduto);
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
        [HttpComponentesAuthorize(Roles = "tpav")]
        public HttpResponseMessage GetTipoAvaliacaoById(int id)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var ge = coordenacaoBiz.getTipoAvaliacaoById(id);
                retorno.retorno = ge;
                if (ge.cd_tipo_avaliacao <= 0)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "tpav")]
        [HttpComponentesAuthorize(Roles = "cur")]
        [HttpComponentesAuthorize(Roles = "prod")]
        public HttpResponseMessage GetCursoTipoAvaliacao(int id)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICursoBusiness cursoBiz = (ICursoBusiness)base.instanciarBusiness<ICursoBusiness>();
                var ge = cursoBiz.getCursoTipoAvaliacao(id);
                retorno.retorno = ge;
                if (ge.Count() <= 0)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, retorno);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        // Put api/<controller>/5
        [HttpComponentesAuthorize(Roles = "tpav.a")]
        public HttpResponseMessage PostAlterarTipoAvaliacao(TipoAvaliacao tipoAvaliacao)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var tipoaval = coordenacaoBiz.putTipoAvaliacao(tipoAvaliacao, cdEscola);
                retorno.retorno = tipoaval;
                if (tipoaval.cd_tipo_avaliacao <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, retorno);
                configureHeaderResponse(response, null);
                return response;
            }
            catch (TurmaBusinessException ex)
            {
                if (ex.tipoErro.Equals(TurmaBusinessException.TipoErro.ERRO_EXISTE_NOTA_LANCADA))
                {
                    retorno.AddMensagem(ex.Message, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                    return Request.CreateResponse(HttpStatusCode.BadRequest, retorno);
                }
                else
                    if (ex.tipoErro.Equals(TurmaBusinessException.TipoErro.ERRO_SOMATORIO_NOTA_MAXIMA))
                    {
                        retorno.AddMensagem(ex.Message, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                        return Request.CreateResponse(HttpStatusCode.BadRequest, retorno);
                    }

                return gerarLogException(ex.Message, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }

        }
        // Post api/<controller>/5
        [HttpComponentesAuthorize(Roles = "tpav.i")]
        public HttpResponseMessage PostTipoAvaliacao(TipoAvaliacao tipoAvaliacao)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var tipoaval = coordenacaoBiz.postTipoAvaliacao(tipoAvaliacao);
                retorno.retorno = tipoaval;
                if (tipoaval.cd_tipo_avaliacao <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, retorno);
                configureHeaderResponse(response, null);
                return response;
            }
            catch (TurmaBusinessException ex)
            {
                if (ex.tipoErro.Equals(TurmaBusinessException.TipoErro.ERRO_SOMATORIO_NOTA_MAXIMA))
                {
                    retorno.AddMensagem(ex.Message, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                    return Request.CreateResponse(HttpStatusCode.OK, retorno);
                }

                return gerarLogException(ex.Message, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
        }

        // Delte api/<controller>
        [HttpComponentesAuthorize(Roles = "tpav.e")]
        public HttpResponseMessage PostDeleteTipoAvaliacao(List<TipoAvaliacao> TiposAvaliacao)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var deleted = coordenacaoBiz.deleteAllTipoAvaliacao(TiposAvaliacao, cdEscola);
                retorno.retorno = deleted;
                if (!deleted)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
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

        //Método responsável pela emissão do relatório:
        [HttpComponentesAuthorize(Roles = "tpav")]
        public HttpResponseMessage GetUrlRelatorioTipoAvaliacao(string sort, int direction, string descricao, bool inicio, int status, int cd_tipo_avaliacao, int cd_criterio_avaliacao, int cdCurso, int cdProduto)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                // Pega os parâmetros do usuário para criar a url do relatório: 
                string strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@descricao=" + descricao + "&@inicio=" + inicio + "&@status=" + status + "&@tipoAvalicao=" + cd_tipo_avaliacao + "&@criterio=" + cd_criterio_avaliacao + "&@cdCurso=" + cdCurso + "&@cdProduto=" + cdProduto + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=" + "Relatório de Avaliação de Curso&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.TipoAvaliacaoSearch;
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

        [HttpComponentesAuthorize(Roles = "tpav")]
        public HttpResponseMessage getAllTipoAvaliacao(bool? ativo, int cdTipo)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var list = coordenacaoBiz.getTipoAvaliacao(ativo, cdTipo);
                retorno.retorno = list;
                if (list.Count() <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                };
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));

            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
            }
        }
        [HttpComponentesAuthorize(Roles = "tpav")]
        public HttpResponseMessage getTipoAvaliacao()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var list = coordenacaoBiz.getTipoAvaliacao();
                retorno.retorno = list;
                if (list.Count() <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                };
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));

            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
            }
        }

        #endregion

        #region Avaliação

        [HttpComponentesAuthorize(Roles = "rptaval")]
        public HttpResponseMessage getUrlReportAvaliacao(int cd_turma, int tipoTurma, int cdCurso, int cdProduto, string dtInicial, string dtFinal, 
            int cdFuncionario, int sitTurma, bool isConceito)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;

                string parametros = "@cd_turma=" + cd_turma + "&@tipoTurma=" + tipoTurma +
                    "&@cdCurso=" + cdCurso + "&@cdProduto=" + cdProduto + "&@sitTurma=" + sitTurma +
                    "&@cdEscola=" + cdEscola + "&@cdFuncionario=" + cdFuncionario + "&@dtInicial=" + dtInicial + "&@dtFinal=" + dtFinal +
                    "&@isConceito=" + isConceito; 

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

        [HttpComponentesAuthorize(Roles = "tpav")]
        [HttpComponentesAuthorize(Roles = "crit")]
        public HttpResponseMessage getAvaliacaoByIdTipoAvaliacao(int idTipoAvaliacao)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ITurmaBusiness turmaBusiness = (ITurmaBusiness)base.instanciarBusiness<ITurmaBusiness>();
                var avaliacao = turmaBusiness.getAvaliacaoByIdTipoAvaliacao(idTipoAvaliacao);
                retorno.retorno = avaliacao;
                if (avaliacao.Count() <= 0)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                return Request.CreateResponse(HttpStatusCode.OK, retorno);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        private bool verificaTotalNota(int idTipoAvaliacao, int idCriterio, int idAvaliacao, byte? notaNova, decimal? peso)
        {
            byte? notaAvaliacao = 0;
            byte? totalNotaBD = 0;
            ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
            ITurmaBusiness turmaBusiness = (ITurmaBusiness)base.instanciarBusiness<ITurmaBusiness>();

            if (idTipoAvaliacao > 0 && idCriterio > 0)
            {
                totalNotaBD = turmaBusiness.getSomatorio(idTipoAvaliacao, idCriterio);
            }

            var totalTipoAvalicaoBD = coordenacaoBiz.getTotalNotaTipoAvaliacao(idTipoAvaliacao);

            notaAvaliacao = idAvaliacao > 0 ? turmaBusiness.getNotaAvaliacao(idAvaliacao) : 0;


            if (peso == null)
                peso = 0;
            return ((totalNotaBD - notaAvaliacao) + (notaNova * peso)) > totalTipoAvalicaoBD;
        }

        #endregion

        #endregion

        #region Programação Curso

        [HttpComponentesAuthorize(Roles = "mpc")]
        [HttpComponentesAuthorize(Roles = "tur")]
        public HttpResponseMessage postGerarModeloProgramacaoAcaoRelacionada(List<int> cd_turmas)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                coordenacaoBiz.geraModeloProgramacoesTurma(cd_turmas, this.ComponentesUser.CodEmpresa.Value);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                retorno.AddMensagem(Messages.msgProgramacaoTurmaSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                return response;
            }
            catch (ProgramacaoTurmaBusinessException ex)
            {
                return gerarLogException(ex.Message, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Componentes.Utils.Messages.Messages.msgErroProcessarAcao, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "tur")]
        public HttpResponseMessage postIncluirProgramacoesCurso(List<int> cd_turmas)
        {
            ReturnResult retorno = new ReturnResult();
            int cdEscola = this.ComponentesUser.CodEmpresa.Value;
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                coordenacaoBiz.criaProgramacoesTurmaCurso(cd_turmas, this.ComponentesUser.CodEmpresa.Value);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                retorno.AddMensagem(Messages.msgProgramacaoTurmaSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                return response;
            }
            catch (ProgramacaoTurmaBusinessException ex)
            {
                return gerarLogException(ex.Message, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Componentes.Utils.Messages.Messages.msgErroProcessarAcao, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "tur")]
        public HttpResponseMessage postIncluirProgramacoesModelo(List<int> cd_turmas)
        {
            ReturnResult retorno = new ReturnResult();
            int cdEscola = this.ComponentesUser.CodEmpresa.Value;
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                coordenacaoBiz.criaProgramacoesTurmaCurso(cd_turmas, this.ComponentesUser.CodEmpresa.Value, true);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                retorno.AddMensagem(Messages.msgProgramacaoTurmaSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                return response;
            }
            catch (ProgramacaoTurmaBusinessException ex)
            {
                return gerarLogException(ex.Message, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Componentes.Utils.Messages.Messages.msgErroProcessarAcao, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "tur")]
        public HttpResponseMessage postIncluirProgramacaoCurso(ProgramacaoHorarioUI programacao)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                retorno.retorno = coordenacaoBiz.criaProgramacaoTurmaCurso(programacao, this.ComponentesUser.CodEmpresa.Value, programacao.modelo);
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (ProgramacaoTurmaBusinessException ex)
            {
                return gerarLogException(ex.Message, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Componentes.Utils.Messages.Messages.msgNotIncludReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "mpc")]
        [HttpComponentesAuthorize(Roles = "tur")]
        public HttpResponseMessage postGerarModeloProgramacao(ProgramacaoHorarioUI programacao)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;

                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                List<ItemProgramacao> listProgramacoes = coordenacaoBiz.geraModeloProgramacoesTurma(programacao, cdEscola);
                if (listProgramacoes.Count > 0)
                    retorno.AddMensagem(Messages.msgModeloProgramacoesCriadas, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                else
                    retorno.AddMensagem(Messages.msgProgramacoesInexistentes, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Componentes.Utils.Messages.Messages.msgErroProcessarAcao, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "tur")]
        public HttpResponseMessage postProgramacoesTurma(ProgramacaoHorarioUI programacao)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                ITurmaBusiness turmaBiz = (ITurmaBusiness)base.instanciarBusiness<ITurmaBusiness>();
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                if (programacao.dt_inicio.HasValue)
                    programacao.dt_inicio = programacao.dt_inicio.Value.ToLocalTime();
                ProgramacaoTurmaAbaUI progTurma = turmaBiz.getProgramacoesTurma(programacao.cd_turma.Value, cdEscola);

                if (programacao.feriados_desconsiderados != null)
                    progTurma.FeriadosDesconsiderados = programacao.feriados_desconsiderados;

                if (programacao.tipo.HasValue && programacao.tipo.Value == (int)TipoAtualizacaoProgramacaoTurma.TIPO_REFAZER_PROGRAMACOES_CURSO && programacao.modelo.HasValue)
                    progTurma.Programacoes = coordenacaoBiz.criaProgramacaoTurmaCurso(programacao, cdEscola, programacao.modelo.Value);
                else if (programacao.tipo.HasValue && programacao.tipo.Value == (int)TipoAtualizacaoProgramacaoTurma.TIPO_REFAZER_PROGRAMACOES_DATA_INICIAL)
                    progTurma.Programacoes = coordenacaoBiz.atualizaProgramacaoTurma(progTurma.Programacoes.ToList(), programacao, cdEscola);
                else if (programacao.tipo.HasValue && (programacao.tipo.Value == (int)TipoAtualizacaoProgramacaoTurma.TIPO_REFAZER_PROGRAMACOES_HORARIO
                                                      || programacao.tipo.Value == (int)TipoAtualizacaoProgramacaoTurma.TIPO_REFAZER_PROGRAMACOES_DESC_FERIADO))
                {
                    Boolean refez_programacoes = false;

                    if (progTurma.Programacoes != null)
                        progTurma.Programacoes = coordenacaoBiz.atualizaProgramacaoTurmaAposDiarioAula(progTurma.Programacoes.ToList(), programacao, cdEscola, ref refez_programacoes);
                    if (refez_programacoes)
                    {
                        if (programacao.tipo.HasValue && programacao.tipo.Value == (int)TipoAtualizacaoProgramacaoTurma.TIPO_REFAZER_PROGRAMACOES_HORARIO)
                            retorno.AddMensagem(Messages.msgProgramacoesRefeitasPeloHorario, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                        else
                            retorno.AddMensagem(Messages.msgProgramacoesRefeitasPeloDesconsideraFeriado, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                    }
                }

                //Coloca as datas das turmas na zona de tempo correto:
                List<ProgramacaoTurma> listaProgramacoes = new List<ProgramacaoTurma>();

                if (progTurma.Programacoes != null)
                {
                    listaProgramacoes = progTurma.Programacoes.ToList();
                    foreach (ProgramacaoTurma progTurma2 in listaProgramacoes)
                        progTurma2.dta_programacao_turma = progTurma2.dta_programacao_turma.ToUniversalTime().Date;
                    progTurma.Programacoes = listaProgramacoes;
                }

                retorno.retorno = progTurma;

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (ProgramacaoTurmaBusinessException ex)
            {
                //Verifica se a exceção é de warning (ela ocorre quando o usuário quer trocar o curso ou duração da turma para uma que não tem programação de curso:
                if (ex.tipoErro == ProgramacaoTurmaBusinessException.TipoErro.ERRO_NAO_TEM_PROGRAMACAO_CURSO)
                {
                    retorno.retorno = new List<ProgramacaoTurma>();

                    retorno.AddMensagem(ex.Message, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);

                    return this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                }
                return gerarLogException(ex.Message, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [Obsolete]
        [HttpComponentesAuthorize(Roles = "tur")]
        public HttpResponseMessage postProgramacoesTurmasFilhas(ProgramacaoHorarioUI programacao)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                //throw new Exception("cehgou");
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                ITurmaBusiness turmaBiz = (ITurmaBusiness)base.instanciarBusiness<ITurmaBusiness>();
                ProgramacaoTurmaAbaUI progTurma = turmaBiz.getProgramacoesTurma(programacao.cd_turma.Value, cdEscola);
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();

                if (programacao.feriados_desconsiderados != null)
                    progTurma.FeriadosDesconsiderados = programacao.feriados_desconsiderados;
                if (programacao.tipo.Value == (int)TipoAtualizacaoProgramacaoTurma.TIPO_REFAZER_PROGRAMACOES_HORARIO)
                {
                    Boolean refez_programacoes = false;

                    progTurma.Programacoes = coordenacaoBiz.atualizaProgramacaoTurmaAposDiarioAula(progTurma.Programacoes.ToList(), programacao, cdEscola, ref refez_programacoes);
                    if (refez_programacoes)
                    {
                        if (programacao.tipo.Value == (int)TipoAtualizacaoProgramacaoTurma.TIPO_REFAZER_PROGRAMACOES_HORARIO)
                            retorno.AddMensagem(Messages.msgProgramacoesRefeitasPeloHorario, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                        else
                            retorno.AddMensagem(Messages.msgProgramacoesRefeitasPeloDesconsideraFeriado, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                    }
                }
                retorno.retorno = progTurma;
                return this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (ProgramacaoTurmaBusinessException ex)
            {
                //Verifica se a exceção é de warning (ela ocorre quando o usuário quer trocar o curso ou duração da turma para uma que não tem programação de curso:
                if (ex.tipoErro == ProgramacaoTurmaBusinessException.TipoErro.ERRO_NAO_TEM_PROGRAMACAO_CURSO)
                {
                    retorno.retorno = new List<ProgramacaoTurma>();

                    retorno.AddMensagem(ex.Message, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);

                    return this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                }
                return gerarLogException(ex.Message, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "prog")]
        public HttpResponseMessage getProgramacaoCursoSearch(int? cdCurso, int? cdDuracao)
        {
            try
            {
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var retorno = coordenacaoBiz.getProgramacaoCursoSearch(parametros, cdCurso, cdDuracao, null);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "mpc")]
        public HttpResponseMessage getModeloProgramacaoCursoSearch(int? cdCurso, int? cdDuracao)
        {
            try
            {
                int cd_escola = this.ComponentesUser.CodEmpresa.Value;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var retorno = coordenacaoBiz.getProgramacaoCursoSearch(parametros, cdCurso, cdDuracao, cd_escola);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        //Método responsável pela emissão do relatório:
        [HttpComponentesAuthorize(Roles = "mpc")]
        public HttpResponseMessage getUrlRelatorioModeloProgramacaoCursoSearch(string sort, int direction, int? cdCurso, int? cdDuracao)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                // Pega os parâmetros do usuário para criar a url do relatório:
                var strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@cdCurso=" + cdCurso + "&@cdDuracao=" + cdDuracao + "&@cdEscola=" + cdEscola + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=" + "Relatório de Modelo de Programação de Curso&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.ModeloProgramacaoCursoSearch;
                //parametros = HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8);
                var parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);

                HttpResponseMessage response = null;
                if (parametrosCript == null)
                    response = Request.CreateResponse(HttpStatusCode.NotFound);
                else
                    response = Request.CreateResponse(HttpStatusCode.OK, parametrosCript);

                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception exe)
            {
                return gerarLogException(Messages.msgErroRelatorio, retorno, logger, exe);
            }
        }

        //Método responsável pela emissão do relatório:
        [HttpComponentesAuthorize(Roles = "prog")]
        public HttpResponseMessage GetUrlRelatorioProgramacaoCursoSearch(string sort, int direction, int? cdCurso, int? cdDuracao)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                // Pega os parâmetros do usuário para criar a url do relatório:
                string strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@cdCurso=" + cdCurso + "&@cdDuracao=" + cdDuracao + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=" + "Relatório de Programação Curso&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.ProgramacaoCursoSearch;
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
        // GET api/<controller>/1
        [HttpComponentesAuthorize(Roles = "prog")]
        public HttpResponseMessage GetProgramacaoCursoById(int id)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var ge = coordenacaoBiz.GetProgramacaoCursoById(id);
                retorno.retorno = ge;
                if (ge.cd_programacao_curso <= 0)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        // Put api/<controller>/5
        [HttpComponentesAuthorize(Roles = "mpc.a")]
        public HttpResponseMessage postAlterarModeloProgramacaoCurso(Programacao programacaoCurso)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { coordenacaoBiz });
                programacaoCurso.cd_escola = this.ComponentesUser.CodEmpresa.Value;
                var programacao = coordenacaoBiz.putProgramacaoCurso(programacaoCurso);
                retorno.retorno = programacao;
                if (programacaoCurso.itens.Count() < 0)
                    retorno.AddMensagem(Componentes.Utils.Messages.Messages.msgErrorSalvar, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Componentes.Utils.Messages.Messages.msgSalvoSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Componentes.Utils.Messages.Messages.msgErrorSalvar, retorno, logger, ex);
            }
        }

        // Put api/<controller>/5
        [HttpComponentesAuthorize(Roles = "prog.a")]
        public HttpResponseMessage PostAlterarProgramacaoCurso(Programacao programacaoCurso)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdRegime = 0;
                if (programacaoCurso.cd_regime != null)
                    cdRegime = Convert.ToInt32(programacaoCurso.cd_regime);
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var programacao = coordenacaoBiz.putProgramacaoCurso(programacaoCurso);
                retorno.retorno = programacao;
                if (programacaoCurso.itens.Count() < 0)
                    retorno.AddMensagem(Componentes.Utils.Messages.Messages.msgErrorSalvar, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Componentes.Utils.Messages.Messages.msgSalvoSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Componentes.Utils.Messages.Messages.msgErrorSalvar, retorno, logger, ex);
            }

        }

        // Post api/<controller>/5
        [HttpComponentesAuthorize(Roles = "mpc.i")]
        public HttpResponseMessage postModeloProgramacaoCurso(Programacao programacao)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { coordenacaoBiz });
                programacao.cd_escola = this.ComponentesUser.CodEmpresa.Value;
                var prog = coordenacaoBiz.postProgramacaoCurso(programacao);
                retorno.retorno = prog;
                if (prog.cd_programacao_curso < 0)
                    retorno.AddMensagem(Componentes.Utils.Messages.Messages.msgErrorSalvar, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Componentes.Utils.Messages.Messages.msgSalvoSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Componentes.Utils.Messages.Messages.msgErrorSalvar, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "prog.i")]
        public HttpResponseMessage PostProgramacaoCurso(Programacao programacao)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var prog = coordenacaoBiz.postProgramacaoCurso(programacao);
                retorno.retorno = prog;
                if (prog.cd_programacao_curso < 0)
                    retorno.AddMensagem(Componentes.Utils.Messages.Messages.msgErrorSalvar, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Componentes.Utils.Messages.Messages.msgSalvoSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Componentes.Utils.Messages.Messages.msgErrorSalvar, retorno, logger, ex);
            }

        }
        // Delte api/<controller>
        [HttpComponentesAuthorize(Roles = "prog.e")]
        public HttpResponseMessage PostDeleteProgramacaoCurso(List<ProgramacaoCurso> programacaoCurso)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { coordenacaoBiz });
                var deleted = coordenacaoBiz.deleteAllProgramacaoCurso(programacaoCurso);
                retorno.retorno = deleted;
                if (!deleted)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
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

        [HttpComponentesAuthorize(Roles = "prog")]
        [HttpComponentesAuthorize(Roles = "tur")]
        public HttpResponseMessage getProgramacoesTurmaPorAluno(int cd_aluno, string dt_inicial, int cd_turma_principal, string listaProg)
        {
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                DateTime data = Convert.ToDateTime(dt_inicial);
                List<int> listaProgs = listaProg != null ? listaProg.Split(',').Select(Int32.Parse).ToList() : new List<int>();

                ITurmaBusiness turmaBiz = (ITurmaBusiness)base.instanciarBusiness<ITurmaBusiness>();
                IEnumerable<ProgramacaoTurma> progTurma = turmaBiz.getProgramacoesTurmaPorAluno(cdEscola, cd_aluno, data, cd_turma_principal, listaProgs);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, progTurma);
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, new ReturnResult(), logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "mpc.e")]
        public HttpResponseMessage PostDeleteModeloProgramacaoCurso(List<ProgramacaoCurso> programacaoCurso)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { coordenacaoBiz });
                var deleted = coordenacaoBiz.deleteAllProgramacaoCurso(programacaoCurso);
                retorno.retorno = deleted;
                if (!deleted)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
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


        [HttpComponentesAuthorize(Roles = "mpc")]
        public HttpResponseMessage getModeloCursoProgramacao(int cdCurso, int cdDuracao)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                IEnumerable<ItemProgramacao> ge = coordenacaoBiz.getCursoProg(cdCurso, cdDuracao, cdEscola);
                retorno.retorno = ge;
                if (ge.Count() <= 0)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "prog")]
        public HttpResponseMessage getCursoProgramacao(int cdCurso, int cdDuracao)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var ge = coordenacaoBiz.getCursoProg(cdCurso, cdDuracao, null);
                retorno.retorno = ge;
                if (ge.Count() <= 0)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }
        [HttpComponentesAuthorize(Roles = "prog")]
        public HttpResponseMessage getItensProgramacaoCursoById(int cdProgramacao)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var ge = coordenacaoBiz.getItensProgramacaoCursoById(cdProgramacao);
                retorno.retorno = ge;
                if (ge.Count() <= 0)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        #endregion

        #region Atividade Extra

        [HttpComponentesAuthorize(Roles = "atvex")]
        public HttpResponseMessage getAtividadeExtraSearch(String dataIni, String dataFim, String hrInicial, String hrFinal, int? tipoAtividade, int? curso, int? responsavel, int? produto, int? aluno, byte lancada, int cd_escola_combo)
        {
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                var dataInicial = Convert.ToDateTime(dataIni);
                var dataFinal = Convert.ToDateTime(dataFim);
                TimeSpan? horaInicial;
                TimeSpan? horaFinal;

                if (hrInicial != "null") horaInicial = TimeSpan.Parse(hrInicial);
                else horaInicial = null;
                if (hrFinal != "null") horaFinal = TimeSpan.Parse(hrFinal);
                else horaFinal = null;

                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var retorno = coordenacaoBiz.searchAtividadeExtra(parametros, dataInicial, dataFinal, horaInicial, horaFinal, tipoAtividade, curso, responsavel, produto, aluno, lancada, cdEscola, cd_escola_combo);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                logger.Error("CoordenacaoController searchAtividadeExtra - Erro: ", ex);
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(Messages.msgRegCritError),
                    ReasonPhrase = Messages.msgCritError
                });
            }
        }

        [HttpComponentesAuthorize(Roles = "atvex.i")]
        public HttpResponseMessage postIncluirAtividadeExtra(AtividadeExtraUI atividadeExtraUI)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { coordenacaoBiz });
                atividadeExtraUI.cd_pessoa_escola = this.ComponentesUser.CodEmpresa.Value;
                atividadeExtraUI.dt_atividade_extra = atividadeExtraUI.dt_atividade_extra.Date;
                AtividadeExtraUI inserirAtividade = coordenacaoBiz.addAtividadadeExtra(atividadeExtraUI);
                retorno.retorno = inserirAtividade;
                if (inserirAtividade.cd_atividade_extra <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }

        }

        //// Put api/<controller>/5
        [HttpComponentesAuthorize(Roles = "atvex.a")]
        public HttpResponseMessage postAlterarAtividadeExtra(AtividadeExtraUI atividadeExtraUI)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { coordenacaoBiz });
                atividadeExtraUI.cd_escola = this.ComponentesUser.CodEmpresa.Value;
                atividadeExtraUI.dt_atividade_extra = atividadeExtraUI.dt_atividade_extra.Date;
                AtividadeExtra atividadeExtraBd = coordenacaoBiz.findAtividadeExtraById(atividadeExtraUI.cd_atividade_extra);
                AtividadeExtraUI atividadeEdit = null;
                if (atividadeExtraUI.cd_escola == atividadeExtraBd.cd_pessoa_escola)
                {
                    atividadeExtraUI.cd_escola_parametro = atividadeExtraBd.cd_pessoa_escola;
                    atividadeExtraUI.cd_pessoa_escola = atividadeExtraBd.cd_pessoa_escola;
                    atividadeEdit = coordenacaoBiz.editAtividadeExtra(atividadeExtraUI);
                }
                else
                {
                    atividadeExtraUI.cd_escola_parametro = atividadeExtraBd.cd_pessoa_escola;
                    atividadeEdit = coordenacaoBiz.editAtividadeExtraOutraEscola(atividadeExtraUI, atividadeExtraUI.cd_escola);
                    //atividadeEdit.cd_pessoa_escola = atividadeExtraBd.cd_pessoa_escola;
                }

                retorno.retorno = atividadeEdit;
                if (atividadeEdit.cd_atividade_extra <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }

        }


        [HttpComponentesAuthorize(Roles = "atvex.e")]
        [HttpComponentesAuthorize(Roles = "tavex")]
        [HttpComponentesAuthorize(Roles = "cur")]
        [HttpComponentesAuthorize(Roles = "func")]
        [HttpComponentesAuthorize(Roles = "alu")]
        [HttpComponentesAuthorize(Roles = "prod")]
        [HttpComponentesAuthorize(Roles = "sala")]
        public HttpResponseMessage PostDeleteAtividadesExtras(List<AtividadeExtra> atividadeExtra)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { coordenacaoBiz });
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                var delAtividadeExtra = coordenacaoBiz.deleteAllAtividadeExtra(atividadeExtra, cdEscola);
                retorno.retorno = delAtividadeExtra;
                if (!delAtividadeExtra)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
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



        [HttpComponentesAuthorize(Roles = "atvex")]
        [HttpPost]
        public HttpResponseMessage PostDeleteRecorrencias(AtividadeExtra atividadeExtra)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { coordenacaoBiz });
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                var delAtividadeExtra = coordenacaoBiz.deleteRecorrencias(atividadeExtra, cdEscola);
                retorno.retorno = delAtividadeExtra;
                if (!delAtividadeExtra)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
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

        [HttpComponentesAuthorize(Roles = "atvex")]
        [HttpPost]
        public HttpResponseMessage PostSendEmailRecorrencias(AtividadeExtra atividadeExtra)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { coordenacaoBiz });
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                List<string> retornoSendEmail = coordenacaoBiz.enviarEmailRecorrencias(atividadeExtra, cdEscola);
                retorno.retorno = retornoSendEmail;
                if (retornoSendEmail.Count > 0)
                {
                    retorno.AddMensagem("Não foi possível enviar o(s) e-mail(s)", null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                else
                {
                    retorno.AddMensagem("E-mail(s) enviado(s) com sucesso.", null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
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

        [HttpComponentesAuthorize(Roles = "atvex")]
        [HttpPost]
        public HttpResponseMessage PostSendEmailProspectsAcaoRelacionada(List<AtividadeExtra> atividadeExtra)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { coordenacaoBiz });
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                List<string> retornoSendEmail = coordenacaoBiz.enviarEmailProspectsAcaoRelacionada(atividadeExtra, cdEscola);
                retorno.retorno = retornoSendEmail;
                if (retornoSendEmail.Count > 0)
                {
                    retorno.AddMensagem("Não foi possível enviar o(s) e-mail(s)", null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                else
                {
                    retorno.AddMensagem("E-mail(s) enviado(s) com sucesso.", null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
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

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "sala")]
        public HttpResponseMessage verificaSalaOnline(int? cd_sala)
        {
            try
            {
                int cd_escola = this.ComponentesUser.CodEmpresa.Value;
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var retorno = coordenacaoBiz.verificaSalaOnline(cd_sala, cd_escola);
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "atvex")]
        public HttpResponseMessage GetAtividadeAluno(int cdAtividadeExtra)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { coordenacaoBiz });
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                IEnumerable<AtividadeAlunoUI> atividades = coordenacaoBiz.searchAtividadeAluno(cdAtividadeExtra, cdEscola);
                retorno.retorno = atividades;
                if (atividades.Count() <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }

        }

        [HttpComponentesAuthorize(Roles = "atvex")]
        public HttpResponseMessage GetAtividadeProspect(int cdAtividadeExtra)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { coordenacaoBiz });
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                IEnumerable<AtividadeProspectUI> prospects = coordenacaoBiz.searchAtividadeProspect(cdAtividadeExtra, cdEscola);
                retorno.retorno = prospects;
                if (prospects.Count() <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }

        }

        //Método responsável pela emissão do relatório:
        [HttpComponentesAuthorize(Roles = "atvex")]
        public HttpResponseMessage GetUrlRelatorioAtividadeExtra(int cdAtividadeExtra, int cd_escola_combo)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {

                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                // Pega os parâmetros do usuário para criar a url do relatório:
                string parametros = "@cdEscola=" + cdEscola + "&@cdAtividadeExtra=" + cdAtividadeExtra + "&@cd_escola_combo=" + cd_escola_combo;
                //parametros = HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8);
                string parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);
                return Request.CreateResponse(HttpStatusCode.OK, parametrosCript);
            }
            catch (Exception exe)
            {
                return gerarLogException(Messages.msgErroRelatorio, retorno, logger, exe);
            }
        }

        [HttpComponentesAuthorize(Roles = "atvex")]
        [HttpComponentesAuthorize(Roles = "tavex")]
        [HttpComponentesAuthorize(Roles = "cur")]
        [HttpComponentesAuthorize(Roles = "func")]
        [HttpComponentesAuthorize(Roles = "alu")]
        [HttpComponentesAuthorize(Roles = "prod")]
        [HttpComponentesAuthorize(Roles = "sala")]
        public HttpResponseMessage returnDataAtividadeExtraParaPesquisa(AtividadeExtraPesquisa atividadeExtraPesq)
        {
            ReturnResult retorno = new ReturnResult();
            AtividadeExtraUI atividadeExtraUI = new AtividadeExtraUI();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                ICursoBusiness cursoBiz = (ICursoBusiness)base.instanciarBusiness<ICursoBusiness>();
                IProfessorBusiness profBiz = (IProfessorBusiness)base.instanciarBusiness<IProfessorBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { coordenacaoBiz });
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                atividadeExtraUI.tiposAtividadeExtras = coordenacaoBiz.getTipoAtividadeWhitAtividadeExtra(cdEscola).ToList();
                atividadeExtraUI.cursos = cursoBiz.getCursoWithAtividadeExtra(true, cdEscola).ToList();
                atividadeExtraUI.professores = profBiz.getFuncionariosByEscolaWithAtividadeExtra(cdEscola, true).ToList();
                atividadeExtraUI.produtos = coordenacaoBiz.getProdutosWithAtividadeExtra(cdEscola, true).ToList();
                retorno.retorno = atividadeExtraUI;

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }

        }

        #endregion

        #region Atividade Prospect
        [HttpComponentesAuthorize(Roles = "pros")]
        public HttpResponseMessage GetAtividadeProspectByCdProspect(int cdProspect)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { coordenacaoBiz });
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                IEnumerable<AtividadeProspectUI> prospects = coordenacaoBiz.searchAtividadeProspectByCdProspect(cdProspect, cdEscola);
                retorno.retorno = prospects;
                if (prospects.Count() <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }

        }


        #endregion

        #region Desconto por Antecipação
        // Post api/<controller>/5
        [HttpComponentesAuthorize(Roles = "poldes.i")]
        public HttpResponseMessage PostPoliticaDesconto(PoliticaDesconto politicaDesconto)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                IFinanceiroBusiness finanBiz = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { coordenacaoBiz, finanBiz });
                //int[] cdTurmas = null;
                //int[] cdAlunos = null;
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                politicaDesconto.cd_pessoa_escola = cdEscola;
                //int i, j;
                //Tratamento do fuso horário da data:
                politicaDesconto.dt_inicial_politica = SGF.Utils.ConversorUTC.Date(politicaDesconto.dt_inicial_politica,
                    this.ComponentesUser.IdFusoHorario, this.ComponentesUser.IsHorarioVerao);

                //// Pegando códigos da Turma
                //if (politicaDesconto.PoliticasTurmas != null && politicaDesconto.PoliticasTurmas.Count() > 0)
                //{
                //    i = 0;
                //    int[] turmas = new int[politicaDesconto.PoliticasTurmas.Count()];
                //    foreach (var c in politicaDesconto.PoliticasTurmas)
                //    {
                //        turmas[i] = c.cd_turma;
                //        i++;
                //    }
                //    cdTurmas = turmas;
                //}
                //// Pegando códigos do Aluno
                //if (politicaDesconto.PoliticasAlunos != null && politicaDesconto.PoliticasAlunos.Count() > 0)
                //{
                //    j = 0;
                //    int[] alunos = new int[politicaDesconto.PoliticasAlunos.Count()];
                //    foreach (var c in politicaDesconto.PoliticasAlunos)
                //    {
                //        alunos[j] = c.cd_aluno;
                //        j++;
                //    }
                //    cdAlunos = alunos;
                //}

                //var polDesconto = coordenacaoBiz.postPoliticaDesconto(politicaDesconto, cdTurmas, cdAlunos);
                var polDesconto = coordenacaoBiz.postPoliticaDesconto(politicaDesconto);
                retorno.retorno = polDesconto;

                if (polDesconto.cd_politica_desconto <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, retorno);
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }

        }
        #endregion

        #region Matricula

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "anesc")]
        [HttpComponentesAuthorize(Roles = "tur")]
        [HttpComponentesAuthorize(Roles = "dur")]
        [HttpComponentesAuthorize(Roles = "prod")]
        [HttpComponentesAuthorize(Roles = "reg")]
        [HttpComponentesAuthorize(Roles = "nmCt")]
        [HttpComponentesAuthorize(Roles = "mat")]
        [HttpComponentesAuthorize(Roles = "locmv")]
        public HttpResponseMessage componentesNovaMatricula(int? cdDuracao, int? cdProduto, int? cdRegime, int? cd_nome_contrato)
        {
            ReturnResult retorno = new ReturnResult();
            Contrato contrato = new Contrato();
            int cdEscola = (int)this.ComponentesUser.CodEmpresa;
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                contrato = coordenacaoBiz.componentesNovaMatricula(cdDuracao, cdProduto, cdRegime, cd_nome_contrato, null, null, cdEscola);
                retorno.retorno = contrato;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "anesc")]
        [HttpComponentesAuthorize(Roles = "mat")]
        [HttpComponentesAuthorize(Roles = "alu")]
        [HttpComponentesAuthorize(Roles = "tur")]
        [HttpComponentesAuthorize(Roles = "plct")]
        [HttpComponentesAuthorize(Roles = "prod")]
        [HttpComponentesAuthorize(Roles = "cur")]
        [HttpComponentesAuthorize(Roles = "dur")]
        [HttpComponentesAuthorize(Roles = "reg")]
        [HttpComponentesAuthorize(Roles = "tpfin")]
        [HttpComponentesAuthorize(Roles = "tpdes")]
        [HttpComponentesAuthorize(Roles = "banc")]
        public HttpResponseMessage GetMatriculaById(int id)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var cont = coordenacaoBiz.GetMatriculaByIdPesq(id, cdEscola);
                retorno.retorno = cont;
                if (cont.contrato.cd_contrato <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, retorno);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "anesc")]
        [HttpComponentesAuthorize(Roles = "mat")]
        [HttpComponentesAuthorize(Roles = "alu")]
        [HttpComponentesAuthorize(Roles = "tur")]
        [HttpComponentesAuthorize(Roles = "plct")]
        [HttpComponentesAuthorize(Roles = "prod")]
        [HttpComponentesAuthorize(Roles = "cur")]
        [HttpComponentesAuthorize(Roles = "dur")]
        [HttpComponentesAuthorize(Roles = "reg")]
        [HttpComponentesAuthorize(Roles = "tpfin")]
        [HttpComponentesAuthorize(Roles = "tpdes")]
        [HttpComponentesAuthorize(Roles = "banc")]
        public HttpResponseMessage GetMatriculaByIdComponentes(int id)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var cont = coordenacaoBiz.GetMatriculaByIdComponentesPesq(id, cdEscola);
                retorno.retorno = cont;
                if (cont.cd_contrato <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                return Request.CreateResponse(HttpStatusCode.OK, retorno);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        
        [HttpComponentesAuthorize(Roles = "banc")]
        public HttpResponseMessage GetMatriculaComponentBancoPesq()
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var cont = coordenacaoBiz.GetMatriculaComponentBancoPesq();
                retorno.retorno = cont;
                return Request.CreateResponse(HttpStatusCode.OK, retorno);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "mat")]
        public HttpResponseMessage getUrlRelatorioRecibo(int cd_contrato)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                int pEscola = (int)this.ComponentesUser.CodEmpresa;
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { coordenacaoBiz });

                SGFWebContext dbComp = new SGFWebContext();
                int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                coordenacaoBiz.getVerificaReciboConfirmacao(cd_contrato, pEscola);
                

                // Pega os parâmetros do usuário para criar a url do relatório:
                string parametros = "@pEscola=" + pEscola + "&@pCdContrato=" + cd_contrato + "&@pOrigemTitulo=" + cd_origem;
                //parametros = HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8);
                string parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, parametrosCript);

                configureHeaderResponse(response, null);
                return response;
            }
            catch (MatriculaBusinessException exe)
            {
                if (exe.tipoErro == MatriculaBusinessException.TipoErro.MATRICULA_NAO_ENCONTRADA)
                    return gerarLogException(exe.Message, retorno, logger, exe);
                else if (exe.tipoErro == MatriculaBusinessException.TipoErro.ERRO_TIPO_FINANCEIRO_DIFERENTE_CARTAO_CHEQUE)
                    return gerarLogException(exe.Message, retorno, logger, exe);
                else if (exe.tipoErro == MatriculaBusinessException.TipoErro.ERRO_TITULOS_RECIBO_CONFIRMACAO_NOT_FOUND)
                    return gerarLogException(exe.Message, retorno, logger, exe);
                else
                    return gerarLogException(Messages.msgErroGerarReciboConfirmacao, retorno, logger, exe);
            }
            catch (Exception exe)
            {
                return gerarLogException(Messages.msgErroRelatorio, retorno, logger, exe);
            }
        }

        [HttpComponentesAuthorize(Roles = "mvfin")]
        public HttpResponseMessage getUrlRelatorioReciboConfirmacaoMovimento(int cd_movimento)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                int pEscola = (int)this.ComponentesUser.CodEmpresa;
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { coordenacaoBiz });

                SGFWebContext dbComp = new SGFWebContext();
                int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Movimento"].ToString());
                coordenacaoBiz.getVerificaReciboConfirmacaoMovimento(cd_movimento, pEscola);


                // Pega os parâmetros do usuário para criar a url do relatório:
                string parametros = "@pEscola=" + pEscola + "&@pCdContrato=" + cd_movimento + "&@pOrigemTitulo=" + cd_origem;
                //parametros = HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8);
                string parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, parametrosCript);

                configureHeaderResponse(response, null);
                return response;
            }
            catch (MatriculaBusinessException exe)
            {
                if (exe.tipoErro == MatriculaBusinessException.TipoErro.MATRICULA_NAO_ENCONTRADA)
                    return gerarLogException(exe.Message, retorno, logger, exe);
                else if (exe.tipoErro == MatriculaBusinessException.TipoErro.ERRO_TIPO_FINANCEIRO_DIFERENTE_CARTAO_CHEQUE)
                    return gerarLogException(exe.Message, retorno, logger, exe);
                else if (exe.tipoErro == MatriculaBusinessException.TipoErro.ERRO_TITULOS_RECIBO_CONFIRMACAO_NOT_FOUND)
                    return gerarLogException(exe.Message, retorno, logger, exe);
                else
                    return gerarLogException(Messages.msgErroGerarReciboConfirmacao, retorno, logger, exe);
            }
            catch (Exception exe)
            {
                return gerarLogException(Messages.msgErroRelatorio, retorno, logger, exe);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "anesc")]
        [HttpComponentesAuthorize(Roles = "nmCt")]
        public HttpResponseMessage componentesPesquisaMatricula()
        {
            ReturnResult retorno = new ReturnResult();
            Contrato contrato = new Contrato();
            int cdEscola = (int)this.ComponentesUser.CodEmpresa;
            try
            {
                ISecretariaBusiness Business = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                contrato.nomesContrato = Business.getNomeContratoMat(cdEscola).ToList();
                contrato.anosEscolares = Business.getAnoEscolaresAtivos(null).ToList();
                retorno.retorno = contrato;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        #endregion

        #region Diário de Aula

        [HttpComponentesAuthorize(Roles = "daula")]
        public HttpResponseMessage getDiarioAulaSearch(int cd_turma, string no_professor, int cd_tipo_aula, byte status, byte presProf, bool substituto,
                                                       bool inicio, string dtInicial, string dtFinal, int? cdProf, int cd_escola_combo)
        {
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { coordenacaoBiz });
                DateTime? dtaInicial = dtInicial == null ? null : (DateTime?)Convert.ToDateTime(dtInicial);
                DateTime? dtaFinal = dtFinal == null ? null : (DateTime?)Convert.ToDateTime(dtFinal);

                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                if (no_professor == null)
                    no_professor = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                var retorno = coordenacaoBiz.searchDiarioAula(parametros, cd_turma, no_professor, cd_tipo_aula, status, presProf, substituto, inicio, dtaInicial, dtaFinal, cdEscola, cdProf, cd_escola_combo);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "daula")]
        public HttpResponseMessage GeturlrelatorioDiarioAula(string sort, int direction, int cd_turma, string no_professor, int cd_tipo_aula, byte status, byte presProf, bool substituto,
                                                       bool inicio, string dtInicial, string dtFinal, int? cdProf, int cd_escola_combo)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { coordenacaoBiz });
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;

                //Pega os parâmetros do usuário para criar a url do relatório:
                string strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@cd_turma=" + cd_turma + "&@no_professor=" + no_professor + "&@cd_tipo_aula=" + cd_tipo_aula + "&@status=" + status + "&@presProf=" + presProf +
                    "&@substituto=" + substituto + "&@inicio=" + inicio + "&@dtInicial=" + dtInicial + "&@dtFinal=" + dtFinal + "&@cdProf=" + cdProf + "&@cd_escola=" + cdEscola + "&@cd_escola_combo=" + cd_escola_combo + "&"
                    + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=Relatório Diário de Aula&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO +
                    "=" + (int)FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.DiarioAulaSearch;
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

        [HttpComponentesAuthorize(Roles = "tavex")]
        public HttpResponseMessage getAtividadeExtraDiarioById(int? id)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { coordenacaoBiz });
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                var atividade = coordenacaoBiz.getTipoAtividade(null, null, (int)cdEscola, TipoAtividadeExtraDataAccess.TipoConsultaAtivExtraEnum.HAS_DIARIO_AULA).ToList();
                retorno.retorno = atividade;
                if (atividade.Count() > 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "sala")]
        [HttpComponentesAuthorize(Roles = "tavex")]
        [HttpComponentesAuthorize(Roles = "daula")]
        public HttpResponseMessage componentesNovoDiarioAula()
        {
            ReturnResult retorno = new ReturnResult();
            DiarioAula componentesDiario = new DiarioAula();
            try
            {
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                componentesDiario.tipoAtividadeExtra = coordenacaoBiz.getTipoAtividade(null, null, (int)cdEscola, TipoAtividadeExtraDataAccess.TipoConsultaAtivExtraEnum.HAS_ATIVO).ToList();
                componentesDiario.salasDiario = coordenacaoBiz.getSalas(0, cdEscola, SalaDataAccess.TipoConsultaDuracaoEnum.HAS_ATIVO).ToList();
                componentesDiario.mtvoFalta = coordenacaoBiz.getMotivoFaltaAtivo(0).ToList();
                retorno.retorno = componentesDiario;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "daula")]
        [HttpComponentesAuthorize(Roles = "tavex")]
        [HttpComponentesAuthorize(Roles = "tur")]
        [HttpComponentesAuthorize(Roles = "sala")]
        [HttpComponentesAuthorize(Roles = "func")]
        public HttpResponseMessage getDiarioAulaAndComponentes(int cd_diario_aula)
        {
            ReturnResult retorno = new ReturnResult();
            int cdEscola = (int)this.ComponentesUser.CodEmpresa;
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                ITurmaBusiness turmaBiz = (ITurmaBusiness)base.instanciarBusiness<ITurmaBusiness>();

                IProfessorBusiness profBiz = (IProfessorBusiness)base.instanciarBusiness<IProfessorBusiness>();

                DiarioAula dAula = coordenacaoBiz.getEditDiarioAula(cd_diario_aula, cdEscola);
                dAula.avaliacoes = turmaBiz.getAvaliacaoECriterioTurma(dAula.cd_turma, cdEscola).ToList();
                dAula.mtvoFalta = coordenacaoBiz.getMotivoFaltaAtivo(cd_diario_aula).ToList();
                // dAula.no_prof = profBiz.getProfessorTurma(cdEscola, dAula.cd_turma).ToList().FirstOrDefault().no_pessoa;
                List<ProfessorUI> listaProfessor = profBiz.getProfessorTurma(cdEscola, dAula.cd_turma).ToList();
                if (listaProfessor != null && listaProfessor.Count() > 0 && listaProfessor.Where(p => p.cd_pessoa == dAula.cd_professor).Any())
                {
                    dAula.no_prof =  listaProfessor.Where(p => p.cd_pessoa == dAula.cd_professor).FirstOrDefault().no_pessoa ;
                }else if(dAula.cd_professor == 0)
                {
                    dAula.no_prof = "";
                }
                

                if (dAula.cd_sala != null && dAula.cd_sala > 0)
                    dAula.salasDiario = coordenacaoBiz.getSalas((int)dAula.cd_sala, cdEscola, SalaDataAccess.TipoConsultaDuracaoEnum.HAS_ATIVO).ToList();
                else
                    dAula.salasDiario = coordenacaoBiz.getSalas(0, cdEscola, SalaDataAccess.TipoConsultaDuracaoEnum.HAS_ATIVO).ToList();
                if (dAula.cd_turma > 0)
                {
                    if (dAula.ProgramacaoTurma != null)
                        dAula.ProgramacaoTurma.DiariosAula = null;
                    retorno.retorno = dAula;
                }
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "even")]
        public HttpResponseMessage getEventos()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                retorno.retorno = coordenacaoBiz.getEventos(0, EventoDataAccess.TipoConsultaEventoEnum.HAS_ATIVO).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "daula")]
        public HttpResponseMessage postEventosEAlunosDiario(DiarioAula diario)
        {
            ReturnResult retorno = new ReturnResult();
            DiarioAula componentesDiario = new DiarioAula();
            int cdEscola = (int)this.ComponentesUser.CodEmpresa;
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                IAlunoBusiness alunoBiz = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { coordenacaoBiz });
                componentesDiario.eventos = coordenacaoBiz.getEventos(0, EventoDataAccess.TipoConsultaEventoEnum.HAS_ATIVO).ToList();
                if (diario.cd_diario_aula == 0)
                    componentesDiario.alunos = alunoBiz.getAlunosTurmaAtivosDiarioAulaDiario(diario.cd_turma, cdEscola, diario.dt_aula).ToList();
                retorno.retorno = componentesDiario;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));

                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        public HttpResponseMessage postEventosEAlunosDiarioCarga(DiarioAula diario)
        {
            ReturnResult retorno = new ReturnResult();
            DiarioAula componentesDiario = new DiarioAula();
            int cdEscola = (int)this.ComponentesUser.CodEmpresa;
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                IAlunoBusiness alunoBiz = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { coordenacaoBiz });
                //if (diario.cd_diario_aula == 0)
                    componentesDiario.alunos = alunoBiz.getAlunosTurmaAtivosDiarioAulaCarga(diario.cd_turma, cdEscola, diario.dt_aula).ToList();
                retorno.retorno = componentesDiario;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));

                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "daula")]
        public HttpResponseMessage postAlunosPorEventosDiario(DiarioAula diario)
        {
            ReturnResult retorno = new ReturnResult();
            DiarioAula componentesDiario = new DiarioAula();
            int cdEscola = (int)this.ComponentesUser.CodEmpresa;
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                IAlunoBusiness alunoBiz = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { coordenacaoBiz });
                componentesDiario.alunos = alunoBiz.getAlunosPorEventoDiario(diario.cd_turma, cdEscola, diario.dt_aula, (int)diario.cd_evento_diario, diario.cd_diario_aula).ToList();
                retorno.retorno = componentesDiario;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                configureHeaderResponse(response, null);
                return response; ;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "daula.i")]
        public HttpResponseMessage postInsertDiarioAula(DiarioAula diario)
        {

            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { coordenacaoBiz });
                if (diario.cd_pessoa_empresa != (int)this.ComponentesUser.CodEmpresa)
                    throw new CoordenacaoBusinessException(Messages.msgAlterarDiarioEscola, null, CoordenacaoBusinessException.TipoErro.ERRO_ALTERAR_DIARIO_OUTRA_ESCOLA, false);
                diario.cd_pessoa_empresa = (int)this.ComponentesUser.CodEmpresa;
                diario.cd_usuario = (int)this.ComponentesUser.CodUsuario;
                diario.dt_cadastro_aula = DateTime.UtcNow;
                diario.dt_aula = diario.dt_aula.Date;
                var diarioCad = coordenacaoBiz.addDiarioAula(diario, false);
                retorno.retorno = diarioCad;
                if (diarioCad.cd_diario_aula <= 0)
                    retorno.AddMensagem(Messages.msgNotIncludReg, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));

                configureHeaderResponse(response, null);
                return response;
            }
            catch (CoordenacaoBusinessException ex)
            {
                if (ex.tipoErro == CoordenacaoBusinessException.TipoErro.INTERSECAO_HORARIOS_DIARIO_AULA ||
                    ex.tipoErro == CoordenacaoBusinessException.TipoErro.NAO_POSSIVEL_ALTERACAO_DIARIO_CANCELADO)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                if (ex.tipoErro == CoordenacaoBusinessException.TipoErro.DIARIO_SEM_ALUNO_MATRICULADO)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                var message = Utils.Utils.innerMessage(ex);
                if (message != "")
                {
                    CoordenacaoBusinessException fx = new CoordenacaoBusinessException(message, ex, 0, false);
                    return gerarLogException(message, retorno, logger, fx);
                }
                else
                    return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                var message = Utils.Utils.innerMessage(ex);
                if (message != "")
                {
                    CoordenacaoBusinessException fx = new CoordenacaoBusinessException(message, ex, 0, false);
                    return gerarLogException(message, retorno, logger, fx);
                }
                else
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }

        }


        [HttpComponentesAuthorize(Roles = "daula.a")]
        public HttpResponseMessage postUpdateDiarioAula(DiarioAula diario)
        {
            ReturnResult retorno = new ReturnResult();
            //throw new Exception("sdfgasdfa");
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                IAlunoBusiness alunoBiz = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { coordenacaoBiz });
                if (diario.cd_pessoa_empresa != (int)this.ComponentesUser.CodEmpresa)
                    throw new CoordenacaoBusinessException(Messages.msgAlterarDiarioEscola, null, CoordenacaoBusinessException.TipoErro.ERRO_ALTERAR_DIARIO_OUTRA_ESCOLA, false);
                diario.cd_pessoa_empresa = (int)this.ComponentesUser.CodEmpresa;
                diario.cd_usuario = (int)this.ComponentesUser.CodUsuario;
                diario.dt_aula = diario.dt_aula.Date;
                var diarioCad = coordenacaoBiz.editDiarioAula(diario);
                retorno.retorno = diarioCad;
                if (diario.cd_diario_aula <= 0)
                    retorno.AddMensagem(Messages.msgNotUpReg, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));

                configureHeaderResponse(response, null);
                return response;
            }
            catch (CoordenacaoBusinessException ex)
            {
                if (ex.tipoErro == CoordenacaoBusinessException.TipoErro.INTERSECAO_HORARIOS_DIARIO_AULA)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                if (ex.tipoErro == CoordenacaoBusinessException.TipoErro.NAO_POSSIVEL_ALTERACAO_DIARIO_CANCELADO)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                var message = Utils.Utils.innerMessage(ex);
                if (message != "")
                {
                    CoordenacaoBusinessException fx = new CoordenacaoBusinessException(message, ex, 0, false);
                    return gerarLogException(message, retorno, logger, fx);
                }
                else
                    return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                var message = Utils.Utils.innerMessage(ex);
                if (message != "")
                {
                    CoordenacaoBusinessException fx = new CoordenacaoBusinessException(message, ex, 0, false);
                    return gerarLogException(message, retorno, logger, fx);
                }
                else
                    return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "daula.e")]
        public HttpResponseMessage postDeleteDiarios(List<DiarioAula> diarios)
        {
            ReturnResult retorno = new ReturnResult();
            var cd_escola = (int)this.ComponentesUser.CodEmpresa;
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                IAlunoBusiness alunoBiz = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { coordenacaoBiz });
                int[] cdDiarios = null;
                int i;
                // Pegando códigos da Turma
                if (diarios != null && diarios.Count() > 0)
                {
                    i = 0;
                    int[] cdDiariosCont = new int[diarios.Count()];
                    foreach (var d in diarios)
                    {
                        cdDiariosCont[i] = d.cd_diario_aula;
                        i++;
                    }
                    cdDiarios = cdDiariosCont;
                }
                var delDiarios = coordenacaoBiz.deleteDiarios(cdDiarios, cd_escola);
                retorno.retorno = delDiarios;
                if (!delDiarios)
                    retorno.AddMensagem(Messages.msgNotDeletedReg, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                else
                    retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));

                configureHeaderResponse(response, null);
                return response;
            }
            catch (CoordenacaoBusinessException ex)
            {
                if (ex.tipoErro == CoordenacaoBusinessException.TipoErro.NAO_EXCLUIR_DIARIO_CANCELADO)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotDeletedReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "daula.a")]
        public HttpResponseMessage postCancelarDiario(DiarioAula diario)
        {
            ReturnResult retorno = new ReturnResult();
            var cd_escola = (int)this.ComponentesUser.CodEmpresa;
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                IAlunoBusiness alunoBiz = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { coordenacaoBiz });
                coordenacaoBiz.cancelarDiarioAula(diario.cd_diario_aula, cd_escola);
                retorno.retorno = true;
                retorno.AddMensagem(Messages.msgRegCanSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));

                configureHeaderResponse(response, null);
                return response;
            }
            catch (CoordenacaoBusinessException ex)
            {
                if (ex.tipoErro == CoordenacaoBusinessException.TipoErro.DIARIO_AULA_JA_CANCELADO)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegNotCanSucess, retorno, logger, ex);
            }
        }


        [HttpComponentesAuthorize(Roles = "daula")]
        public HttpResponseMessage getObsDiarioAula(int cd_diario_aula)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                IAlunoBusiness alunoBiz = (IAlunoBusiness)base.instanciarBusiness<IAlunoBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { coordenacaoBiz });
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                string obs = coordenacaoBiz.getObsDiarioAula(cd_diario_aula, cdEscola);
                retorno.retorno = obs;
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        #endregion

        #region Mudança de Turma
        // GET api/<controller>/1
        [HttpComponentesAuthorize(Roles = "alu")]
        [HttpComponentesAuthorize(Roles = "tur")]
        public HttpResponseMessage postVerificaAlunosTurma(List<AlunoTurma> alunosTurma)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {

                int cdEscola = this.ComponentesUser.CodEmpresa.Value;

                ITurmaBusiness turmaBiz = (ITurmaBusiness)base.instanciarBusiness<ITurmaBusiness>();
                bool ok = turmaBiz.getVerificaDadosMudanca(alunosTurma, cdEscola);

                retorno.retorno = ok;

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));

                configureHeaderResponse(response, null);
                return response;
            }
            catch (CoordenacaoBusinessException ex)
            {
                if (ex.tipoErro == CoordenacaoBusinessException.TipoErro.EXISTE_DESISTENCIA)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                if (ex.tipoErro == CoordenacaoBusinessException.TipoErro.ERRO_ALUNO_COM_AVALIACAO ||
                    ex.tipoErro == CoordenacaoBusinessException.TipoErro.ERRO_MUDANCAO_SEM_CURSO)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
            catch (AlunoBusinessException ex)
            {
                if (ex.tipoErro == AlunoBusinessException.TipoErro.ERRO_ALUNO_COM_DIARIO)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                if (ex.tipoErro == AlunoBusinessException.TipoErro.ERRO_ALUNO_MATRICULADO_TURMA_ATUAL)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                if (ex.tipoErro == AlunoBusinessException.TipoErro.ERRO_TURMA_POSSUI_ALUNO)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                if (ex.tipoErro == AlunoBusinessException.TipoErro.ERRO_ALUNO_HORARIO_OCUPADO)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        #endregion

        #region Histórico da Turma
        [HttpComponentesAuthorize(Roles = "hista")]
        [HttpComponentesAuthorize(Roles = "avlt")]
        public HttpResponseMessage getMediasAvaliacaoAluno(int cd_aluno, int cd_turma)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cd_escola = this.ComponentesUser.CodEmpresa.Value;
                ITurmaBusiness turmaBiz = (ITurmaBusiness)base.instanciarBusiness<ITurmaBusiness>();
                retorno.retorno = turmaBiz.getMediasAvaliacaoAluno(cd_aluno, cd_turma, cd_escola);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "hista")]
        [HttpComponentesAuthorize(Roles = "avlt")]
        public HttpResponseMessage getMediasEstagioAvaliacaoAluno(int cd_aluno, int cd_estagio)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cd_escola = this.ComponentesUser.CodEmpresa.Value;
                ITurmaBusiness turmaBiz = (ITurmaBusiness)base.instanciarBusiness<ITurmaBusiness>();
                retorno.retorno = turmaBiz.getMediasEstagioAvaliacaoAluno(cd_aluno, cd_escola, cd_estagio);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "hista")]
        [HttpComponentesAuthorize(Roles = "daula")]
        public HttpResponseMessage getEventosAvaliacaoAluno(int cd_aluno, int cd_turma)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cd_escola = this.ComponentesUser.CodEmpresa.Value;
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                retorno.retorno = coordenacaoBiz.getEventosAvaliacaoAluno(cd_aluno, cd_turma, cd_escola);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "hista")]
        [HttpComponentesAuthorize(Roles = "avlt")]
        public HttpResponseMessage getConceitosAvaliacaoAluno(int cd_aluno, int cd_turma)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cd_escola = this.ComponentesUser.CodEmpresa.Value;
                ITurmaBusiness turmaBiz = (ITurmaBusiness)base.instanciarBusiness<ITurmaBusiness>();
                retorno.retorno = turmaBiz.getConceitosAvaliacaoAluno(cd_aluno, cd_turma, cd_escola);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "hista")]
        [HttpComponentesAuthorize(Roles = "avlt")]
        public HttpResponseMessage getTurmasAvaliacoes(int cd_aluno)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cd_escola = this.ComponentesUser.CodEmpresa.Value;
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                retorno.retorno = coordenacaoBiz.getTurmasAvaliacoes(cd_aluno, cd_escola);
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "hista")]
        [HttpComponentesAuthorize(Roles = "avlt")]
        public HttpResponseMessage getEstagiosHistoricoAluno(int cd_aluno)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cd_escola = this.ComponentesUser.CodEmpresa.Value;
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                retorno.retorno = coordenacaoBiz.getEstagiosHistoricoAluno(cd_aluno, cd_escola);
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "hista")]
        [HttpComponentesAuthorize(Roles = "atvex")]
        public HttpResponseMessage getAtividadesAluno(int cd_aluno)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cd_escola = this.ComponentesUser.CodEmpresa.Value;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                List<AtividadeExtra> lista_titulos = coordenacaoBiz.getAtividadesAluno(parametros, cd_aluno, cd_escola).ToList();
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, lista_titulos);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
            }
        }
        #endregion

        #region Aula Personalizada
        [HttpComponentesAuthorize(Roles = "aulap")]
        [HttpComponentesAuthorize(Roles = "prod")]
        [HttpComponentesAuthorize(Roles = "func")]
        [HttpComponentesAuthorize(Roles = "sala")]
        public HttpResponseMessage getPesqAulaPersonalizada()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cd_escola = this.ComponentesUser.CodEmpresa.Value;
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                retorno.retorno = coordenacaoBiz.getPesqAulaPersonalizada(cd_escola);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }
        [HttpComponentesAuthorize(Roles = "aulap")]
        public HttpResponseMessage getSearchAulaPersonalizada(string dataIni, string dataFim, string hrInicial, string hrFinal, int? cdProduto, int? cdProfessor,
                                                                      int? cdSala, int? cdAluno, bool participou)
        {
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { coordenacaoBiz });
                int cd_escola = this.ComponentesUser.CodEmpresa.Value;
                DateTime? dataInicial = dataIni == null ? null : (DateTime?)Convert.ToDateTime(dataIni);
                DateTime? dataFinal = dataFim == null ? null : (DateTime?)Convert.ToDateTime(dataFim);
                TimeSpan? horaInicial = hrInicial == null ? null : (TimeSpan?)TimeSpan.Parse(hrInicial);
                TimeSpan? horaFinal = hrFinal == null ? null : (TimeSpan?)TimeSpan.Parse(hrFinal);

                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IEnumerable<AulaPersonalizadaUI> retorno = coordenacaoBiz.searchAulaPersonalizada(parametros, dataInicial, dataFinal, horaInicial, horaFinal, cdProduto, cdProfessor, cdSala, cdAluno, participou, cd_escola);

                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;


            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }
        [HttpComponentesAuthorize(Roles = "aulap")]
        public HttpResponseMessage geturlrelatorioAulaPersonalizada(string sort, int direction, string dataIni, string dataFim, string hrInicial, string hrFinal, int? cdProduto, int? cdProfessor,
                                                                      int? cdSala, int? cdAluno, bool participou)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                int cd_escola = this.ComponentesUser.CodEmpresa.Value;

                // Pega os parâmetros do usuário para criar a url do relatório:
                var strParametrosSort = "@orientation=LANDSCAPE&@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@dataIni=" + dataIni + "&@dataFim=" + dataFim + "&@hrInicial=" + hrInicial + "&@hrFinal=" + hrFinal + "&@cdEscola=" + cd_escola +
                    "&@cdProduto=" + cdProduto + "&@cdProfessor=" + cdProfessor + "&@cdSala=" + cdSala + "&@cdAluno=" + cdAluno + "&@participou=" + participou + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=" + "Relatório de Aula Personalizada&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.AulaPersonalizadaSearch;
                //parametros = HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8);
                var parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);

                HttpResponseMessage response = null;
                if (parametrosCript == null)
                    response = Request.CreateResponse(HttpStatusCode.NotFound);
                else
                    response = Request.CreateResponse(HttpStatusCode.OK, parametrosCript);

                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception exe)
            {
                return gerarLogException(Messages.msgErroRelatorio, retorno, logger, exe);
            }
        }
        [HttpComponentesAuthorize(Roles = "aulap.i")]
        public HttpResponseMessage postIncluirAulaPersonalizada(AulaPersonalizada aulaPersonalizada)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { coordenacaoBiz });
                aulaPersonalizada.dt_aula_personalizada = aulaPersonalizada.dt_aula_personalizada.Date;
                aulaPersonalizada.cd_escola = this.ComponentesUser.CodEmpresa.Value;
                aulaPersonalizada.cd_usuario = (int)this.ComponentesUser.CodUsuario;
                aulaPersonalizada.dt_cadastro_aula = DateTime.UtcNow;

                IEnumerable<AulaPersonalizada> inserirAulaPer = coordenacaoBiz.addAulaPersonalizada(aulaPersonalizada);
                retorno.retorno = inserirAulaPer;
                if (inserirAulaPer.Count() <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (CoordenacaoBusinessException ex)
            {
                if (ex.tipoErro == CoordenacaoBusinessException.TipoErro.INTERSECAO_HORARIOS_DIARIO_AULA ||
                    ex.tipoErro == CoordenacaoBusinessException.TipoErro.ERRO_NAO_INFORMADO_HR_INICIAL_FINAL ||
                    ex.tipoErro == CoordenacaoBusinessException.TipoErro.ERRO_MIN_HR_INICIAL_FINAL ||
                    ex.tipoErro == CoordenacaoBusinessException.TipoErro.ERRO_INTERVALO_HR_INICIAL_FINAL)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }

        }


        [HttpComponentesAuthorize(Roles = "alu")]
        [HttpComponentesAuthorize(Roles = "tur")]
        [HttpComponentesAuthorize(Roles = "prod")]
        [HttpComponentesAuthorize(Roles = "aulap")]
        public HttpResponseMessage getAulaPersonalizadaById(int id, int cd_aluno)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var aulaPer = coordenacaoBiz.searchAulaPersonalizadaById(id, cdEscola, cd_aluno);
                retorno.retorno = aulaPer;
                if (aulaPer.cd_aula_personalizada <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, retorno);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }
        [HttpComponentesAuthorize(Roles = "aulap.a")]
        public HttpResponseMessage postAlterarAulaPersonalizada(AulaPersonalizada aulaPersonalizada)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { coordenacaoBiz });
                aulaPersonalizada.dt_aula_personalizada = aulaPersonalizada.dt_aula_personalizada.Date;
                aulaPersonalizada.cd_escola = this.ComponentesUser.CodEmpresa.Value;
                aulaPersonalizada.cd_usuario = (int)this.ComponentesUser.CodUsuario;
                aulaPersonalizada.dt_cadastro_aula = DateTime.UtcNow;
                IEnumerable<AulaPersonalizada> inserirAulaPer = coordenacaoBiz.editAulaPersonalizada(aulaPersonalizada);
                retorno.retorno = inserirAulaPer;
                if (inserirAulaPer.Count() <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (CoordenacaoBusinessException ex)
            {
                if (ex.tipoErro == CoordenacaoBusinessException.TipoErro.INTERSECAO_HORARIOS_DIARIO_AULA ||
                    ex.tipoErro == CoordenacaoBusinessException.TipoErro.ERRO_NAO_INFORMADO_HR_INICIAL_FINAL ||
                    ex.tipoErro == CoordenacaoBusinessException.TipoErro.ERRO_MIN_HR_INICIAL_FINAL ||
                    ex.tipoErro == CoordenacaoBusinessException.TipoErro.ERRO_INTERVALO_HR_INICIAL_FINAL)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }

        }
        [HttpComponentesAuthorize(Roles = "aulap.e")]
        public HttpResponseMessage postDeleteAulaPersonalizada(List<AulaPersonalizada> aulaPersonalizada)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { coordenacaoBiz });
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                var delAulaPersonalizada = coordenacaoBiz.deleteAllAulaPersonalizada(aulaPersonalizada, cdEscola);
                retorno.retorno = delAulaPersonalizada;
                if (!delAulaPersonalizada)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
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
        #endregion

        #region Avaliação Participação
        [HttpComponentesAuthorize(Roles = "avpar")]
        [HttpComponentesAuthorize(Roles = "prod")]
        [HttpComponentesAuthorize(Roles = "par")]
        public HttpResponseMessage getReturnDadosAvalPart(ProdutoDataAccess.TipoConsultaProdutoEnum hasDependente)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                int cd_escola = this.ComponentesUser.CodEmpresa.Value;
                AvaliacaoParticipacao avalPart = new AvaliacaoParticipacao();
                avalPart.produtos = coordenacaoBiz.findProduto(hasDependente, null, cd_escola).ToList();
                avalPart.criterios = coordenacaoBiz.getCriteriosPorAvalPart(cd_escola).ToList();
                avalPart.participacoes = coordenacaoBiz.getParticipacaoAvaliacaoPart(cd_escola);
                retorno.retorno = avalPart;

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }// try/catch
        }

        [HttpComponentesAuthorize(Roles = "avpar")]
        public HttpResponseMessage getSearchAvaliacaoParticipacao(int cdCriterio, int cdParticipacao, int cdProduto, int ativo)
        {
            try
            {
                int cd_escola = this.ComponentesUser.CodEmpresa.Value;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var retorno = coordenacaoBiz.searchAvaliacaoParticipacao(parametros, cdCriterio, cdParticipacao, cdProduto, getStatus(ativo), cd_escola);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }
        [HttpComponentesAuthorize(Roles = "avpar")]
        public HttpResponseMessage GetUrlRelatorioAvaliacaoParticipacao(string sort, int direction, int cdCriterio, int cdParticipacao, int cdProduto, int ativo)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                // Pega os parâmetros do usuário para criar a url do relatório:
                int cd_escola = this.ComponentesUser.CodEmpresa.Value;
                var strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@cdCriterio=" + cdCriterio + "&@cdParticipacao=" + cdParticipacao + "&@cdProduto=" + cdProduto + "&@ativo=" + ativo + "&@cd_escola=" + cd_escola + "&" +
                    Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=" + "Relatório de Avaliação x Participação&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.AvaliacaoParticipacaoSearch;
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
        [HttpComponentesAuthorize(Roles = "avpar")]
        [HttpComponentesAuthorize(Roles = "prod")]
        [HttpComponentesAuthorize(Roles = "par")]
        public HttpResponseMessage getReturnDadosCadAvalPart(int? cdProduto, int? cdCriterio)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                int cd_escola = this.ComponentesUser.CodEmpresa.Value;
                AvaliacaoParticipacao avalPart = new AvaliacaoParticipacao();
                avalPart.produtos = coordenacaoBiz.findProduto(ProdutoDataAccess.TipoConsultaProdutoEnum.HAS_ATIVO, cdProduto, cd_escola).ToList();
                avalPart.criterios = coordenacaoBiz.getNomesAvaliacaoByAval(cdCriterio).ToList();
                retorno.retorno = avalPart;

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }// try/catch
        }


        [HttpComponentesAuthorize(Roles = "par")]
        public HttpResponseMessage getParticipacoes(string cdsPart)
        {
            try
            {
                int cd_escola = this.ComponentesUser.CodEmpresa.Value;
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var retorno = coordenacaoBiz.getParticipacoes(cdsPart);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "avpar.i")]
        public HttpResponseMessage postInsertAvaliacaoParticipacao(AvaliacaoParticipacao avalPart)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {

                int cd_pessoa_escola = this.ComponentesUser.CodEmpresa.Value;
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                IEnumerable<AvaliacaoParticipacao> addAvalPart = coordenacaoBiz.addAvaliacaoParticipacao(avalPart, cd_pessoa_escola);
                retorno.retorno = addAvalPart;
                if (addAvalPart.Count() <= 0)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (CoordenacaoBusinessException ex)
            {
                if (ex.tipoErro == CoordenacaoBusinessException.TipoErro.ERRO_AVALIACAO_PARTICIPACAO_SEM_VINC ||
                    ex.tipoErro == CoordenacaoBusinessException.TipoErro.ERRO_AVALIACAO_PARTICIPACAO_NOTA)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "avpar")]
        public HttpResponseMessage getAvaliacaoParticipacaoByEdit(int cdAvalPart)
        {
            try
            {
                int cd_escola = this.ComponentesUser.CodEmpresa.Value;
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var retorno = coordenacaoBiz.getAvaliacaoParticipacaoByEdit(cdAvalPart, cd_escola);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "avpar.a")]
        public HttpResponseMessage postAlterarAvaliacaoParticipacao(AvaliacaoParticipacao avalPart)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { coordenacaoBiz });
                int cd_escola = this.ComponentesUser.CodEmpresa.Value;
                IEnumerable<AvaliacaoParticipacao> inserirAvaliacaoParticipacao = coordenacaoBiz.editAvaliacaoParticipacao(avalPart, cd_escola);
                retorno.retorno = inserirAvaliacaoParticipacao;
                if (inserirAvaliacaoParticipacao.Count() <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgAlteracoesSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (CoordenacaoBusinessException ex)
            {
                if (ex.tipoErro == CoordenacaoBusinessException.TipoErro.ERRO_AVALIACAO_PARTICIPACAO_SEM_VINC ||
                    ex.tipoErro == CoordenacaoBusinessException.TipoErro.ERRO_AVALIACAO_PARTICIPACAO_NOTA)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }

        }
        [HttpComponentesAuthorize(Roles = "avpar.e")]
        public HttpResponseMessage postDeleteAvaliacaoParticipacao(List<AvaliacaoParticipacao> avaliacoesPart)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { coordenacaoBiz });
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                var delAvaliacaoParticipacao = coordenacaoBiz.deleteAllAvaliacaoParticipacao(avaliacoesPart, cdEscola);
                retorno.retorno = delAvaliacaoParticipacao;
                if (!delAvaliacaoParticipacao)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
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
        #endregion

        #region Relatório Pagamento Professores

        [HttpComponentesAuthorize(Roles = "func")]
        [HttpComponentesAuthorize(Roles = "rppro")]
        public HttpResponseMessage getComponentesRelatorioPagamentoProfessores()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                IProfessorBusiness profBiz = (IProfessorBusiness)base.instanciarBusiness<IProfessorBusiness>();
                List<ProfessorUI> porfs = profBiz.getProfessorReturnProfUI(ProfessorDataAccess.TipoConsultaProfessorEnum.HAS_ATIVO, cdEscola, null).ToList();
                retorno.retorno = new { professores = porfs };
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;

            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }


        [HttpComponentesAuthorize(Roles = "rppro")]
        public HttpResponseMessage GeturlrelatorioPagamentoProfessores(int cd_tipo_relatorio, int cdProf, string dtInicial, string dtFinal)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                DateTime? dataIncial = string.IsNullOrEmpty(dtInicial) ? null : (DateTime?)Convert.ToDateTime(dtInicial);
                DateTime? dataFinal = string.IsNullOrEmpty(dtFinal) ? null : (DateTime?)Convert.ToDateTime(dtFinal);
                //Pega os parâmetros do usuário para criar a url do relatório:
                string parametros = "@cd_tipo_relatorio=" + cd_tipo_relatorio + "&@cd_professor=" + cdProf + "&@dtInicial=" + dataIncial + "&@dtFinal=" + dtFinal + "&@cd_escola=" + cdEscola;
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

        #region Percentual de Término de Estágio

        [HttpComponentesAuthorize(Roles = "rpte")]
        public HttpResponseMessage GeturlrelatorioPercTerminoEstagio(int cdProf, string dtInicial, string dtFinal)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                //Pega os parâmetros do usuário para criar a url do relatório:
                string parametros = "@dtInicial=" + dtInicial + "&@dtFinal=" + dtFinal + "&@cdProf=" + cdProf + "&@cd_escola=" + cdEscola;
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

        #region Relatório Comissão Secretarias

        [HttpComponentesAuthorize(Roles = "func")]
        [HttpComponentesAuthorize(Roles = "prod")]
        public HttpResponseMessage getComponentesRelatorioComissaoSecretarias()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                IFuncionarioBusiness funcBiz = (IFuncionarioBusiness)base.instanciarBusiness<IFuncionarioBusiness>();
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                List<FuncionarioSearchUI> funcionarios = funcBiz.getFuncionarios(cdEscola, null, FuncionarioSGF.TipoConsultaFuncionarioEnum.HAS_COMISSIONADO).ToList();
                List<Produto> produtos = coordenacaoBiz.findProduto(ProdutoDataAccess.TipoConsultaProdutoEnum.HAS_ATIVO, 0, null).ToList();
                retorno.retorno = new { Funcionarios = funcionarios, Produtos = produtos };
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;

            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "rpcs")]
        public HttpResponseMessage geturlrelatorioComissaoSecretarias(int cdFunc, int cd_produto, string dtInicial, string dtFinal)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                //Pega os parâmetros do usuário para criar a url do relatório:
                string parametros = "@dtInicial=" + dtInicial + "&@dtFinal=" + dtFinal + "&@cdFunc=" + cdFunc + "&@cd_escola=" + cdEscola + "&@cd_produto=" + cd_produto;
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

        #region Calenderio Evento
        [HttpComponentesAuthorize(Roles = "caleven.i")]
        [HttpPost]
        public HttpResponseMessage adicionarCalendarioEvento(CalendarioEvento calendarioEvento)
        {

            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { coordenacaoBiz });

                calendarioEvento.cd_pessoa_escola = (int)this.ComponentesUser.CodEmpresa;
                calendarioEvento.dt_inicial_evento = calendarioEvento.dt_inicial_evento.Date;
                calendarioEvento.dt_final_evento = calendarioEvento.dt_final_evento.Date;

                var calEvento = coordenacaoBiz.addCalendarioEvento(calendarioEvento);
                retorno.retorno = calEvento;
                if (calEvento.cd_calendario_evento <= 0)
                    retorno.AddMensagem(Messages.msgNotIncludReg, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));

                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }

        }

        [HttpComponentesAuthorize(Roles = "caleven.a")]
        [HttpPost]
        public HttpResponseMessage editarCalendarioEvento(CalendarioEvento calendarioEvento)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { coordenacaoBiz });

                calendarioEvento.cd_pessoa_escola = (int)this.ComponentesUser.CodEmpresa;
                calendarioEvento.dt_inicial_evento = calendarioEvento.dt_inicial_evento.Date;
                calendarioEvento.dt_final_evento = calendarioEvento.dt_final_evento.Date;

                var calEvento = coordenacaoBiz.editarCalendarioEvento(calendarioEvento);
                retorno.retorno = calEvento;
                if (calEvento.cd_calendario_evento <= 0)
                    retorno.AddMensagem(Messages.msgNotUpReg, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));

                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "caleven.e")]
        [HttpPost]
        public HttpResponseMessage deletarCalendarioEvento(List<CalendarioEvento> calendarioEvento)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { coordenacaoBiz });
                var cd_escola = (int)this.ComponentesUser.CodEmpresa;
                bool calEvento = false;

                if (calendarioEvento != null)
                {
                    calEvento = coordenacaoBiz.deletarCalendarioEvento(calendarioEvento.Select(c => c.cd_calendario_evento).ToList(), cd_escola);
                    retorno.retorno = calEvento;
                }
                if (!calEvento)
                    retorno.AddMensagem(Messages.msgNotDeletedReg, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                else
                    retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));

                configureHeaderResponse(response, null);
                return response;
            }
            catch (CoordenacaoBusinessException ex)
            {
                if (ex.tipoErro == CoordenacaoBusinessException.TipoErro.ERRO_DT_FIM_MENOR_DT_INICIO)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                if (ex.tipoErro == CoordenacaoBusinessException.TipoErro.ERRO_HH_FIM_MENOR_HH_INICIO)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotDeletedReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "caleven")]
        [HttpGet]
        public HttpResponseMessage obterListaCalendarioEventos()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                var listCalendarioEventos = coordenacaoBiz.obterListaCalendarioEventos(cdEscola);
                retorno.retorno = listCalendarioEventos;
                if (listCalendarioEventos.Count() <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                };
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "caleven")]
        [HttpGet]
        public HttpResponseMessage obterCalendarioEventosPorFiltros(string dc_titulo_evento, bool inicio, bool? status,
            string dt_inicial_evento, string dt_final_evento, string hh_inicial_evento, string hh_final_evento)
        {
            try
            {
                var request = this.Request;
                RangeHeaderValue rangeHeaderValue = this.Request.Headers.Range;
                int cd_escola = this.ComponentesUser.CodEmpresa.Value;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();

                var retorno = coordenacaoBiz.obterCalendarioEventosPorFiltros(parametros, cd_escola, dc_titulo_evento, inicio, status,
                    dt_inicial_evento, dt_final_evento, hh_inicial_evento, hh_final_evento);

                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "caleven")]
        [HttpGet]
        public HttpResponseMessage obterCalendarioEventoPorID(int cd_calendario_evento)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                var calendarioEvento = coordenacaoBiz.obterCalendarioEventoPorID(cd_calendario_evento, cdEscola);
                retorno.retorno = calendarioEvento;

                if (calendarioEvento == null)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        //Método responsável pela emissão do relatório:
        [HttpComponentesAuthorize(Roles = "caleven")]
        public HttpResponseMessage GetUrlRelatorioCalendarioEvento(string sort, int direction, string dc_titulo_evento, bool inicio, bool? status,
            string dt_inicial_evento, string dt_final_evento, string hh_inicial_evento, string hh_final_evento)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                var strParametrosSort = "@orientation=LANDSCAPE&@cs=" + sort + "&@so=" + direction + "&";
                // Pega os parâmetros do usuário para criar a url do relatório:
                string parametros = strParametrosSort + "@cdEscola=" + cdEscola + "&@dc_titulo_evento=" + dc_titulo_evento + "&@inicio=" + inicio +
                    "&@status=" + status + "&@dt_inicial_evento=" + dt_inicial_evento + "&@dt_final_evento=" + dt_final_evento + "&@hh_inicial_evento=" + hh_inicial_evento +
                    "&@hh_final_evento=" + hh_final_evento + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=" + "Relatório de Calendário Evento&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.CalendarioEvento;

                //parametros = HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8);
                string parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);
                return Request.CreateResponse(HttpStatusCode.OK, parametrosCript);
            }
            catch (Exception exe)
            {
                return gerarLogException(Messages.msgErroRelatorio, retorno, logger, exe);
            }
        }
        #endregion

        #region Calenderio Academico
        /* [HttpComponentesAuthorize(Roles = "calaca.i")]
        [HttpPost]
        public HttpResponseMessage adicionarCalendarioAcademico(CalendarioAcademico calendarioAcademico)
        {

            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { coordenacaoBiz });

                calendarioAcademico.cd_pessoa_escola = (int)this.ComponentesUser.CodEmpresa;

                var calAcademico = coordenacaoBiz.addCalendarioAcademico(calendarioAcademico);
                retorno.retorno = calAcademico;
                if (calAcademico.cd_calendario_academico <= 0)
                    retorno.AddMensagem(Messages.msgNotIncludReg, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));

                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }

        }*/

        // [HttpComponentesAuthorize(Roles = "calaca.i")]
        [HttpPost]
        public HttpResponseMessage adicionarCalendarioAcademicoMaster(CalendarioAcademico calendarioAcademico)
        {

            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { coordenacaoBiz });


                List<int> cd_escolas = coordenacaoBiz.findEscolas();
                CalendarioAcademico calAcademico = null;
                if (cd_escolas != null)
                {
                    foreach (int cd_escola in cd_escolas)
                    {
                        CalendarioAcademico cal = calendarioAcademico;
                        calendarioAcademico.cd_pessoa_escola = cd_escola;
                        if (cd_escola == (int)this.ComponentesUser.CodEmpresa)
                        {
                            calAcademico = coordenacaoBiz.addCalendarioAcademico(calendarioAcademico);
                        }
                        else
                        {
                            coordenacaoBiz.addCalendarioAcademico(calendarioAcademico);
                        }

                    }

                }
                retorno.retorno = calAcademico;
                if (calAcademico.cd_calendario_academico <= 0)
                    retorno.AddMensagem(Messages.msgNotIncludReg, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));

                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }

        }

        //[HttpComponentesAuthorize(Roles = "calaca.a")]
        [HttpPost]
        public HttpResponseMessage editarCalendarioAcademicoMaster(CalendarioAcademico calendarioAcademico)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { coordenacaoBiz });


                List<int> cd_escolas = coordenacaoBiz.findEscolas();
                calendarioAcademico.cd_pessoa_escola = (int)this.ComponentesUser.CodEmpresa;
                var calAcademico = coordenacaoBiz.editarCalendarioMaster(cd_escolas, calendarioAcademico);

                retorno.retorno = calAcademico;
                if (calAcademico.cd_calendario_academico <= 0)
                    retorno.AddMensagem(Messages.msgNotUpReg, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));

                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "calaca.a")]
        [HttpPost]
        public HttpResponseMessage editarCalendarioAcademico(CalendarioAcademico calendarioAcademico)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { coordenacaoBiz });

                calendarioAcademico.cd_pessoa_escola = (int)this.ComponentesUser.CodEmpresa;

                var calAcademico = coordenacaoBiz.editarCalendarioAcademico(calendarioAcademico);
                retorno.retorno = calAcademico;
                if (calAcademico.cd_calendario_academico <= 0)
                    retorno.AddMensagem(Messages.msgNotUpReg, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));

                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
        }
        /*
                [HttpComponentesAuthorize(Roles = "calaca.e")]
                [HttpPost]
                public HttpResponseMessage deletarCalendarioAcademico(List<CalendarioAcademico> calendarioAcademico)
                {
                    ReturnResult retorno = new ReturnResult();
                    try
                    {
                        ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                        configuraBusiness(new List<IGenericBusiness>() { coordenacaoBiz });
                        var cd_escola = (int)this.ComponentesUser.CodEmpresa;
                        bool calCalendario = false;

                        if (calendarioAcademico != null)
                        {
                            calCalendario = coordenacaoBiz.deletarCalendarioAcademico(calendarioAcademico.Select(c => c.cd_calendario_academico).ToList(), cd_escola);
                            retorno.retorno = calCalendario;
                        }
                        if (!calCalendario)
                            retorno.AddMensagem(Messages.msgNotDeletedReg, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                        else
                            retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));

                        configureHeaderResponse(response, null);
                        return response;
                    }
                    catch (CoordenacaoBusinessException ex)
                    {
                        if (ex.tipoErro == CoordenacaoBusinessException.TipoErro.ERRO_DT_FIM_MENOR_DT_INICIO)
                            return gerarLogException(ex.Message, retorno, logger, ex);
                        if (ex.tipoErro == CoordenacaoBusinessException.TipoErro.ERRO_HH_FIM_MENOR_HH_INICIO)
                            return gerarLogException(ex.Message, retorno, logger, ex);

                        return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
                    }
                    catch (Exception ex)
                    {
                        return gerarLogException(Messages.msgNotDeletedReg, retorno, logger, ex);
                    }
                }*/

        //[HttpComponentesAuthorize(Roles = "calaca.e")]
        [HttpPost]
        public HttpResponseMessage deletarCalendarioAcademicoMaster(List<CalendarioAcademico> calendariosAcademicos)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { coordenacaoBiz });
                var cd_escola = (int)this.ComponentesUser.CodEmpresa;
                bool calCalendario = false;

                List<int> cd_escolas = coordenacaoBiz.findEscolas();

                if (calendariosAcademicos != null)
                {
                    foreach (CalendarioAcademico calendarioAcademico in calendariosAcademicos)
                    {
                        foreach (var item in cd_escolas)
                        {

                            CalendarioAcademico calAcademico = coordenacaoBiz.findCalendarioAcademico(item, calendarioAcademico.cd_tipo_calendario);
                            if (item == cd_escola)
                            {
                                calCalendario = coordenacaoBiz.deletarCalendarioAcademicoMaster(calAcademico);
                            }
                            else
                            {
                                coordenacaoBiz.deletarCalendarioAcademicoMaster(calAcademico);
                            }

                        }
                    }

                    retorno.retorno = calCalendario;
                }
                if (!calCalendario)
                    retorno.AddMensagem(Messages.msgNotDeletedReg, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                else
                    retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));

                configureHeaderResponse(response, null);
                return response;
            }
            catch (CoordenacaoBusinessException ex)
            {
                if (ex.tipoErro == CoordenacaoBusinessException.TipoErro.ERRO_DT_FIM_MENOR_DT_INICIO)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                if (ex.tipoErro == CoordenacaoBusinessException.TipoErro.ERRO_HH_FIM_MENOR_HH_INICIO)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotDeletedReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "calaca")]
        [HttpGet]
        public HttpResponseMessage obterListaCalendarioAcademicos()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                var listaCalendarios = coordenacaoBiz.obterListaCalendarioAcademicos(cdEscola);
                retorno.retorno = listaCalendarios;
                if (listaCalendarios.Count() <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                };
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "calaca")]
        [HttpGet]
        public HttpResponseMessage obterCalendarioAcademicosPorFiltros(int tipo_calendario, bool? status)
        {
            try
            {
                var request = this.Request;
                RangeHeaderValue rangeHeaderValue = this.Request.Headers.Range;
                int cd_escola = this.ComponentesUser.CodEmpresa.Value;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();

                var retorno = coordenacaoBiz.obterCalendarioAcademicosPorFiltros(parametros, cd_escola, tipo_calendario, status, false);

                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "calaca")]
        [HttpGet]
        public HttpResponseMessage obterCalendarioAcademicoPorID(int cd_calendario_academico)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                var calendario = coordenacaoBiz.obterCalendarioAcademicoPorID(cd_calendario_academico, cdEscola);
                retorno.retorno = calendario;

                if (calendario == null)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        //Método responsável pela emissão do relatório:
        [HttpComponentesAuthorize(Roles = "calaca")]
        public HttpResponseMessage GetUrlRelatorioCalendarioAcademico(string sort, int direction, int tipo_calendario, bool? status)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                if (!string.IsNullOrEmpty(sort))
                    sort = sort.Replace("dc_tipo_calendario", "cd_tipo_calendario");

                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                var strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                // Pega os parâmetros do usuário para criar a url do relatório:
                string parametros = strParametrosSort + "@cdEscola=" + cdEscola + "&@tipo_calendario=" + tipo_calendario +
                    "&@status=" + status + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=" + "Relatório de Calendário Acadêmico&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.CalendarioAcademico;

                //parametros = HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8);
                string parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);
                return Request.CreateResponse(HttpStatusCode.OK, parametrosCript);
            }
            catch (Exception exe)
            {
                return gerarLogException(Messages.msgErroRelatorio, retorno, logger, exe);
            }
        }
        #endregion

        #region Video -- Métodos do controller, Persistência e Pesquisa
        //Traz os registros pela descrição da Entidade

        public HttpResponseMessage getVideoSearch([FromUri] VideoSearchUI videoSearchUI)
        {
            try
            {
                /*
                if (desc == null)
                {
                    desc = String.Empty;
                }*/
                var request = this.Request;
                RangeHeaderValue rangeHeaderValue = this.Request.Headers.Range;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var retorno = coordenacaoBiz.getVideoSearch(parametros, videoSearchUI.no_video, videoSearchUI.nm_video, videoSearchUI.menu);

                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage adicionarVideo(Video video)
        {

            ReturnResult retorno = new ReturnResult();
            var nome_arquivo_video = video.no_arquivo_video;
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { coordenacaoBiz });



                var videoAux = coordenacaoBiz.addVideo(video);
                retorno.retorno = videoAux;
                if (videoAux.cd_video <= 0)
                    retorno.AddMensagem(Messages.msgNotIncludReg, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));

                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                var filePath = ConfigurationManager.AppSettings["caminhoUploads"] + "\\Arquivos\\Videos\\";
                FileInfo arquivo_salvo_no_sistema = new FileInfo(filePath + "\\" + nome_arquivo_video);
                if (arquivo_salvo_no_sistema.Exists)
                {
                    ManipuladorArquivo.removerArquivo(filePath + "\\" + nome_arquivo_video);
                }
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }

        }

        [HttpPost]
        public HttpResponseMessage editarVideo(Video video)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { coordenacaoBiz });


                var nome_arquivo_anterior = coordenacaoBiz.obterVideoPorID(video.cd_video).no_arquivo_video;

                Video video_existente = coordenacaoBiz.obterVideoPorNumeroParte(video.nm_video, video.nm_parte);

                Video video_nome_existente = coordenacaoBiz.obterVideoPorNome(video.no_video);


                if (video_existente != null && video_existente.cd_video != video.cd_video)
                {
                    retorno.AddMensagem(Messages.msgNotNumberAndPartVideoDuplicate, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                }
                else if (video_nome_existente != null && video_nome_existente.cd_video != video.cd_video)
                {
                    retorno.AddMensagem(Messages.msgNotNameVideoDuplicate, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                }
                else
                {
                    var videoAux = coordenacaoBiz.editarVideo(video);
                    retorno.retorno = videoAux;
                    if (videoAux.cd_video <= 0)
                    {
                        retorno.AddMensagem(Messages.msgNotUpReg, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                    }
                    else
                    {
                        var filePath = ConfigurationManager.AppSettings["caminhoUploads"] + "\\Arquivos\\Videos\\";
                        FileInfo arquivo_salvo_no_sistema = new FileInfo(filePath + "\\" + nome_arquivo_anterior);
                        if (!nome_arquivo_anterior.Equals(video.no_arquivo_video) && arquivo_salvo_no_sistema.Exists)
                        {
                            ManipuladorArquivo.removerArquivo(filePath + "\\" + nome_arquivo_anterior);
                        }
                        retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                    }

                }


                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));

                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage deletarVideo(List<Video> videos)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { coordenacaoBiz });
                bool videoAux = false;


                if (videos != null)
                {
                    videoAux = coordenacaoBiz.deletarVideo(videos.Select(c => c.cd_video).ToList());
                    retorno.retorno = videoAux;
                }
                if (!videoAux)
                {
                    retorno.AddMensagem(Messages.msgNotDeletedReg, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                }
                else
                {
                    retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                    string caminho_video = ConfigurationManager.AppSettings["caminhoUploads"];
                    foreach (var item in videos)
                    {
                        ManipuladorArquivo.removerArquivo(caminho_video + "\\Arquivos\\Videos\\" + item.no_arquivo_video);
                    }
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

        [HttpGet]
        public HttpResponseMessage obterVideoPorID(int cd_video)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();

                var videoAux = coordenacaoBiz.obterVideoPorID(cd_video);

                retorno.retorno = videoAux;

                if (videoAux == null)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }


        [HttpGet]
        public HttpResponseMessage obterVideosPorFiltros([FromUri] VideoSearchUI videoSearchUI)
        {
            try
            {
                var request = this.Request;
                RangeHeaderValue rangeHeaderValue = this.Request.Headers.Range;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();

                var retorno = coordenacaoBiz.obterVideosPorFiltros(parametros, videoSearchUI.no_video, videoSearchUI.nm_video, videoSearchUI.menu);

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
        public HttpResponseMessage obterListaVideos()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                //var listaVideos = coordenacaoBiz.obterListaVideos();
                //retorno.retorno = listaVideos;
                /*if (listaVideos.Count() <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                };*/
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }


        [HttpPost]
        [AllowAnonymous]
        public HttpResponseMessage uploadVideo()
        {
            ReturnResult retorno = new ReturnResult();
            var response = new HttpResponseMessage();
            try
            {
                var uploadPath = ConfigurationManager.AppSettings["caminhoUploads"] + "\\Arquivos\\Videos\\";

                var httpPostedFile = HttpContext.Current.Request.Files["UploadedVideo"];
                if (httpPostedFile == null)
                {
                    retorno.retorno = false;
                    retorno.AddMensagem(Messages.msgVideoInvalido, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                    return Request.CreateResponse(HttpStatusCode.BadRequest, retorno);

                }

                var fileUpload = httpPostedFile;

                FileInfo arquivo_salvo_no_sistema = new FileInfo(uploadPath + "\\" + fileUpload.FileName);
                if (arquivo_salvo_no_sistema.Exists)
                {
                    retorno.retorno = false;
                    retorno.AddMensagem(Messages.msgErroNomeArquivoExistente, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                    return Request.CreateResponse(HttpStatusCode.BadRequest, retorno);
                }

                //limite  500Megabytes
                string[] extensaoArq = fileUpload.FileName.Split('.');
                if (fileUpload.ContentLength > 524288000)
                {
                    //throw new CoordenacaoBusinessException(Messages.msgTamanhoMaximoVideoExedido, null, CoordenacaoBusinessException.TipoErro.ERRO_NAO_EXISTE_IMAGEM, false);
                    retorno.retorno = false;
                    retorno.AddMensagem(Messages.msgTamanhoMaximoVideoExedido, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                    return Request.CreateResponse(HttpStatusCode.BadRequest, retorno);
                }

                if (extensaoArq[1].ToLower() != "mp4" && extensaoArq[1].ToLower() != "ogv" &&
                    extensaoArq[1].ToLower() != "webm")
                {
                    //throw new CoordenacaoBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgExtensaoErradaArquivoVideo, null, CoordenacaoBusinessException.TipoErro.ERRO_NAO_EXISTE_IMAGEM, false);
                    retorno.retorno = false;
                    retorno.AddMensagem(Messages.msgExtensaoErradaArquivoVideo, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                    return Request.CreateResponse(HttpStatusCode.BadRequest, retorno);
                }

                //Local onde vai ficar as fotos enviadas.
                var serverUploadPath = uploadPath;

                //Faz um checagem se o diretorio existe.
                if (!Directory.Exists(serverUploadPath))
                {
                    Directory.CreateDirectory(serverUploadPath);
                }

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
                // HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObjectAsync(fileUpload.FileName).Result);
                retorno.retorno = fileUpload.FileName;
                return Request.CreateResponse(HttpStatusCode.OK, retorno);
                //return response;
            }
            catch (Exception ex)
            {
                retorno.retorno = false;
                retorno.AddMensagem(ex.Message, null,
                    ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                gerarLogException(Messages.msgNotUploadImage, retorno, logger, ex);
                return Request.CreateResponse(HttpStatusCode.BadRequest, retorno);
            }
        }

        #endregion

        #region Faq
        [HttpPost]
        public HttpResponseMessage adicionarFaq(Faq faq)
        {

            ReturnResult retorno = new ReturnResult();
            var nome_arquivo_faq = faq.no_video_faq;
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { coordenacaoBiz });


                var faqAux = coordenacaoBiz.addFaq(faq);
                retorno.retorno = faqAux;
                if (faqAux.cd_faq <= 0)
                    retorno.AddMensagem(Messages.msgNotIncludReg, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));

                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                var filePath = ConfigurationManager.AppSettings["caminhoUploads"] + "\\Arquivos\\Videos\\";
                FileInfo arquivo_salvo_no_sistema = new FileInfo(filePath + "\\" + nome_arquivo_faq);
                if (arquivo_salvo_no_sistema.Exists)
                {
                    ManipuladorArquivo.removerArquivo(filePath + "\\" + nome_arquivo_faq);
                }
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }

        }

        [HttpPost]
        public HttpResponseMessage editarFaq(Faq faq)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { coordenacaoBiz });

                var nome_arquivo_anterior = coordenacaoBiz.obterFaqPorID(faq.cd_faq).no_video_faq;
                var faqAux = coordenacaoBiz.editarFaq(faq);
                retorno.retorno = faqAux;

                if (faqAux.cd_faq <= 0)
                {
                    retorno.AddMensagem(Messages.msgNotUpReg, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                }
                else
                {
                    if (!string.IsNullOrEmpty(nome_arquivo_anterior))
                    {
                        var filePath = ConfigurationManager.AppSettings["caminhoUploads"] + "\\Arquivos\\Videos\\";
                        FileInfo arquivo_salvo_no_sistema = new FileInfo(filePath + "\\" + nome_arquivo_anterior);
                        if (!nome_arquivo_anterior.Equals(faq.no_video_faq) && arquivo_salvo_no_sistema.Exists)
                        {
                            ManipuladorArquivo.removerArquivo(filePath + "\\" + nome_arquivo_anterior);
                        }
                    }
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }

                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage deletarFaq(List<Faq> faqs)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { coordenacaoBiz });
                bool faqAux = false;


                if (faqs != null)
                {
                    faqAux = coordenacaoBiz.deletarFaq(faqs.Select(c => c.cd_faq).ToList());
                    retorno.retorno = faqAux;
                }
                if (!faqAux)
                {
                    retorno.AddMensagem(Messages.msgNotDeletedReg, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                }
                else
                {
                    retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                    string caminho_video = ConfigurationManager.AppSettings["caminhoUploads"];
                    foreach (var item in faqs)
                    {
                        ManipuladorArquivo.removerArquivo(caminho_video + "\\Arquivos\\Videos\\" + item.no_video_faq);
                    }
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

        [HttpGet]
        public HttpResponseMessage obterFaqPorID(int cd_faq)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();

                var faqAux = coordenacaoBiz.obterFaqPorID(cd_faq);

                retorno.retorno = faqAux;

                if (faqAux == null)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage obterFaqsPorFiltros([FromUri] FaqSearchUI faqSearchUI)
        {
            try
            {
                var request = this.Request;
                RangeHeaderValue rangeHeaderValue = this.Request.Headers.Range;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();

                var menus = new List<byte>();
                if (faqSearchUI.menu != null)
                    menus = JsonConvert.DeserializeObject<List<byte>>(faqSearchUI.menu);

                var retorno = coordenacaoBiz.obterFaqsPorFiltros(parametros, faqSearchUI.dc_faq_pergunta, faqSearchUI.dc_faq_pergunta_inicio, menus);

                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;


            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public HttpResponseMessage uploadVideoFaq()
        {
            ReturnResult retorno = new ReturnResult();
            var response = new HttpResponseMessage();
            try
            {
                var uploadPath = ConfigurationManager.AppSettings["caminhoUploads"] + "\\Arquivos\\Videos\\";

                var httpPostedFile = HttpContext.Current.Request.Files["UploadedVideo"];
                if (httpPostedFile == null)
                {
                    retorno.retorno = false;
                    retorno.AddMensagem(Messages.msgVideoInvalido, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                    return Request.CreateResponse(HttpStatusCode.BadRequest, retorno);
                }

                var fileUpload = httpPostedFile;

                FileInfo arquivo_salvo_no_sistema = new FileInfo(uploadPath + "\\" + fileUpload.FileName);
                if (arquivo_salvo_no_sistema.Exists)
                {
                    retorno.retorno = false;
                    retorno.AddMensagem(Messages.msgErroNomeArquivoExistente, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                    return Request.CreateResponse(HttpStatusCode.BadRequest, retorno);
                }

                //limite  500Megabytes
                string[] extensaoArq = fileUpload.FileName.Split('.');
                if (fileUpload.ContentLength > 524288000)
                {
                    //throw new CoordenacaoBusinessException(Messages.msgTamanhoMaximoVideoExedido, null, CoordenacaoBusinessException.TipoErro.ERRO_NAO_EXISTE_IMAGEM, false);
                    retorno.retorno = false;
                    retorno.AddMensagem(Messages.msgTamanhoMaximoVideoExedido, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                    return Request.CreateResponse(HttpStatusCode.BadRequest, retorno);
                }

                if (extensaoArq[1].ToLower() != "mp4" && extensaoArq[1].ToLower() != "ogv" &&
                    extensaoArq[1].ToLower() != "webm")
                {
                    //throw new CoordenacaoBusinessException(FundacaoFisk.SGF.Utils.Messages.Messages.msgExtensaoErradaArquivoVideo, null, CoordenacaoBusinessException.TipoErro.ERRO_NAO_EXISTE_IMAGEM, false);
                    retorno.retorno = false;
                    retorno.AddMensagem(Messages.msgExtensaoErradaArquivoVideo, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                    return Request.CreateResponse(HttpStatusCode.BadRequest, retorno);
                }

                //Local onde vai ficar as fotos enviadas.
                var serverUploadPath = uploadPath;

                //Faz um checagem se o diretorio existe.
                if (!Directory.Exists(serverUploadPath))
                {
                    Directory.CreateDirectory(serverUploadPath);
                }

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
                // HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObjectAsync(fileUpload.FileName).Result);
                retorno.retorno = fileUpload.FileName;
                return Request.CreateResponse(HttpStatusCode.OK, retorno);
                //return response;
            }
            catch (Exception ex)
            {
                retorno.retorno = false;
                retorno.AddMensagem(ex.Message, null,
                    ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                gerarLogException(Messages.msgNotUploadImage, retorno, logger, ex);
                return Request.CreateResponse(HttpStatusCode.BadRequest, retorno);
            }
        }
        #endregion

        #region Circular
        [HttpPost]
        public HttpResponseMessage adicionarCircular(Circular circular)
        {

            ReturnResult retorno = new ReturnResult();
            var nome_arquivo_circular = circular.no_arquivo_circular;
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { coordenacaoBiz });

                var novaCircular = coordenacaoBiz.addCircular(circular);
                retorno.retorno = novaCircular;

                if (novaCircular.cd_circular <= 0)
                    retorno.AddMensagem(Messages.msgNotIncludReg, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));

                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                var filePath = ConfigurationManager.AppSettings["caminhoUploads"] + "\\Arquivos\\Circulares\\";
                FileInfo arquivo_salvo_no_sistema = new FileInfo(filePath + "\\" + nome_arquivo_circular);
                if (arquivo_salvo_no_sistema.Exists)
                {
                    ManipuladorArquivo.removerArquivo(filePath + "\\" + nome_arquivo_circular);
                }
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }

        }

        [HttpPost]
        [AllowAnonymous]
        public HttpResponseMessage uploadCircular(bool editar)
        {
            ReturnResult retorno = new ReturnResult();
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                //Local onde vai ficar as circulares enviadas.
                var uploadPath = ConfigurationManager.AppSettings["caminhoUploads"] + "\\Arquivos\\Circulares\\";

                var httpPostedFile = HttpContext.Current.Request.Files["UploadedCircular"];
                if (httpPostedFile == null)
                {
                    retorno.retorno = false;
                    retorno.AddMensagem(Messages.msgErroNomeArquivoExistente, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                    return Request.CreateResponse(HttpStatusCode.BadRequest, retorno);
                }

                var fileUpload = httpPostedFile;
                FileInfo arquivo_salvo_no_sistema = new FileInfo(uploadPath + "\\" + fileUpload.FileName);

                if (arquivo_salvo_no_sistema.Exists)
                {
                    retorno.retorno = false;
                    retorno.AddMensagem(Messages.msgErroNomeArquivoExistente, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, retorno);
                    return response;
                }

                //limite  500Megabytes
                if (fileUpload.ContentLength > 100000000)
                {
                    retorno.retorno = false;
                    retorno.AddMensagem("Tamanho do arquivo é inválido.", null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                    return Request.CreateResponse(HttpStatusCode.BadRequest, retorno);
                }


                if (arquivo_salvo_no_sistema.Extension != ".pdf" && arquivo_salvo_no_sistema.Extension != ".docx" && arquivo_salvo_no_sistema.Extension != ".doc")
                {
                    retorno.retorno = false;
                    retorno.AddMensagem("Tipo de arquivo não suportado.", null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, retorno);
                    return response;
                }

                var serverUploadPath = uploadPath;

                //Faz um checagem se o diretorio existe.
                if (!Directory.Exists(serverUploadPath))
                {
                    Directory.CreateDirectory(serverUploadPath);
                }

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

                retorno.retorno = true;
                response = Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException("Não foi possível fazer o upload do arquivo.", retorno, logger, ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage obterCircularesPorFiltros([FromUri] CircularSearchUI circularSearchUI)
        {
            try
            {
                var request = this.Request;
                RangeHeaderValue rangeHeaderValue = this.Request.Headers.Range;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var retorno = coordenacaoBiz.obterCircularesPorFiltros(parametros, circularSearchUI.nm_ano_circular, circularSearchUI.nm_mes_circular,
                    circularSearchUI.nm_circular, circularSearchUI.no_circular, circularSearchUI.nm_menu_circular);

                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage editarCircular(Circular circular)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { coordenacaoBiz });

                var nome_arquivo_anterior = coordenacaoBiz.obterCircularPorID(circular.cd_circular).no_arquivo_circular;

                var circular_editada = coordenacaoBiz.editarCircular(circular);
                retorno.retorno = circular_editada;

                if (circular_editada.cd_circular <= 0)
                    retorno.AddMensagem(Messages.msgNotUpReg, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                {
                    var filePath = ConfigurationManager.AppSettings["caminhoUploads"] + "\\Arquivos\\Circulares\\";
                    FileInfo arquivo_salvo_no_sistema = new FileInfo(filePath + "\\" + nome_arquivo_anterior);
                    if (!nome_arquivo_anterior.Equals(circular.no_arquivo_circular) && arquivo_salvo_no_sistema.Exists)
                    {
                        ManipuladorArquivo.removerArquivo(filePath + "\\" + nome_arquivo_anterior);
                    }
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));

                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage deletarCircular(List<Circular> circulares)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { coordenacaoBiz });
                bool circular_deletada = false;

                if (circulares != null)
                {
                    circular_deletada = coordenacaoBiz.deletarCircular(circulares.Select(c => c.cd_circular).ToList());
                    retorno.retorno = circular_deletada;
                }
                if (!circular_deletada)
                    retorno.AddMensagem(Messages.msgNotDeletedReg, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                else
                {
                    retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                    string caminho_circular = ConfigurationManager.AppSettings["caminhoUploads"];
                    foreach (var item in circulares)
                        ManipuladorArquivo.removerArquivo(caminho_circular + "\\Arquivos\\Circulares\\" + item.no_arquivo_circular);
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

        [HttpGet]
        public HttpResponseMessage obterCircularPorID(int cd_circular)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();

                var circular = coordenacaoBiz.obterCircularPorID(cd_circular);
                retorno.retorno = circular;

                if (circular == null)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        #endregion


        #region Controle de Faltas
        [HttpComponentesAuthorize(Roles = "rpconfal")]
        public HttpResponseMessage GetUrlRptControleFaltas([FromUri] ControleFaltasReportUI controleFaltasReportUI)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                System.Globalization.CultureInfo ptBRCulture = new System.Globalization.CultureInfo("pt-BR");


                IFuncionarioBusiness BusinessFuncionario = (IFuncionarioBusiness)base.instanciarBusiness<IFuncionarioBusiness>();
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                StringBuilder parametros = new StringBuilder();
                parametros.Append("@cd_escola=").Append(cdEscola)
                          .Append("&@cd_tipo=").Append(controleFaltasReportUI.cd_tipo != null ? controleFaltasReportUI.cd_tipo : 0)
                          .Append("&@cd_curso=").Append(controleFaltasReportUI.cd_curso != null ? controleFaltasReportUI.cd_curso : 0)
                          .Append("&@cd_nivel=").Append(controleFaltasReportUI.cd_nivel != null ? controleFaltasReportUI.cd_nivel : 0)
                          .Append("&@cd_produto=").Append(controleFaltasReportUI.cd_produto != null ? controleFaltasReportUI.cd_produto : 0)
                          .Append("&@cd_professor=").Append(controleFaltasReportUI.cd_professor != null ? controleFaltasReportUI.cd_professor : 0)
                          .Append("&@cd_turma=").Append(controleFaltasReportUI.cd_turma != null ? controleFaltasReportUI.cd_turma : 0)
                          .Append("&@cd_sit_turma=").Append(controleFaltasReportUI.cd_sit_turma != null ? controleFaltasReportUI.cd_sit_turma : 0)
                          .Append("&@cd_sit_aluno=").Append(controleFaltasReportUI.cd_sit_aluno)
                          .Append("&@dt_inicial=").Append(controleFaltasReportUI.dt_inicial != null ? ((DateTime)controleFaltasReportUI.dt_inicial).ToString("dd/MM/yyyy") : null)
                          .Append("&@dt_final=").Append(controleFaltasReportUI.dt_final != null ? ((DateTime)controleFaltasReportUI.dt_final).ToString("dd/MM/yyyy") : null)
                          .Append("&@quebrarpagina=").Append(controleFaltasReportUI.quebrarpagina)
                          .Append("&").Append(Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO).Append("=Relatório de Controle de Faltas")
                          .Append("&").Append(Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO).Append("=").Append(FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.rtpControleFaltas);

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

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "confal")]
        public HttpResponseMessage getControleFaltasSearch(String desc, int cd_turma, int cd_aluno, int assinatura, DateTime? dataIni, DateTime? dataFim)
        {
            List<ControleFaltasUI> retorno = new List<ControleFaltasUI>();
            try
            {
                ICoordenacaoBusiness businessCoordenacao = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                DateTime? dtaInicial = DateTime.MinValue;
                DateTime? dtaFinal = DateTime.MaxValue;
                if (dataIni.HasValue)
                    dtaInicial = Convert.ToDateTime(dataIni);

                if (dataFim.HasValue)
                    dtaFinal = Convert.ToDateTime(dataFim);

                //DateTime? dataInicial = !String.IsNullOrEmpty(dataIni) ? (DateTime?)Convert.ToDateTime(dataIni) : null;
                //DateTime? dataFinal = !String.IsNullOrEmpty(dataFim) ? (DateTime?)Convert.ToDateTime(dataFim) : null;

                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                var request = this.Request;
                RangeHeaderValue rangeHeaderValue = this.Request.Headers.Range;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                retorno = businessCoordenacao.getControleFaltasSearch(parametros, desc, cd_turma, cd_aluno, assinatura, dtaInicial, dtaFinal, cdEscola).ToList();
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;

            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "confal.i")]
        public HttpResponseMessage postInsertControleFaltas(ControleFaltasUI item)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness businessCoordenacao = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { businessCoordenacao });
                int cdEscola = ComponentesUser.CodEmpresa.Value;

                item.cd_usuario = ComponentesUser.CodUsuario;
                ControleFaltasUI controleFaltasNovo = businessCoordenacao.addAlunoControleFaltas(item);
                retorno.retorno = controleFaltasNovo;
                if (controleFaltasNovo.cd_controle_faltas <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception exe)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, exe);
            }

        }

        [HttpComponentesAuthorize(Roles = "confal.a")]
        public HttpResponseMessage postEditControleFaltas(ControleFaltasUI item)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness businessCoordenacao = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { businessCoordenacao });
                int cdEscola = ComponentesUser.CodEmpresa.Value;


                ControleFaltas controleFaltas = businessCoordenacao.getControleFaltasById(item.cd_controle_faltas);
                foreach (var aluno in controleFaltas.ControleFaltasAluno)
                {
                    if (aluno.id_assinatura == true &&
                        (controleFaltas.dt_controle_faltas != item.dt_controle_faltas ||
                         controleFaltas.cd_turma != item.cd_turma))
                    {
                        retorno.AddMensagem(Messages.msgNotEditRegConfal, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                        HttpResponseMessage res = Request.CreateResponse(HttpStatusCode.BadRequest, JsonConvert.SerializeObject(retorno));
                        configureHeaderResponse(res, null);
                        return res;
                    }
                }

                ControleFaltasUI itemAlterado = businessCoordenacao.editarAlunoControleFaltas(item);
                retorno.retorno = itemAlterado;
                if (itemAlterado.cd_controle_faltas <= 0)
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

        [HttpComponentesAuthorize(Roles = "confal.e")]
        public HttpResponseMessage PostDeleteControleFaltas(List<int> itens)
        {
            ICoordenacaoBusiness businessCoordenacao = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
            configuraBusiness(new List<IGenericBusiness>() { businessCoordenacao });

            ReturnResult retorno = new ReturnResult();
            try
            {
                foreach (var iten in itens)
                {
                    ControleFaltas controleFaltas = businessCoordenacao.getControleFaltasById(iten);
                    foreach (var aluno in controleFaltas.ControleFaltasAluno)
                    {
                        if (aluno.id_assinatura == true)
                        {
                            retorno.AddMensagem(Messages.msgNotDeletedRegConfal, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                            HttpResponseMessage res = Request.CreateResponse(HttpStatusCode.BadRequest, JsonConvert.SerializeObject(retorno));
                            configureHeaderResponse(res, null);
                            return res;
                        }
                    }
                }

                var deleted = businessCoordenacao.deleteAllControleFaltas(itens);
                retorno.retorno = deleted;
                if (!deleted)
                    retorno.AddMensagem(Messages.msgNotDeletedReg, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
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

        [HttpGet]
        public HttpResponseMessage getAlunosControleFalta(int cd_controle_faltas)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness businessCoordenacao = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { businessCoordenacao });

                List<ControleFaltasAlunoUI> alunos = businessCoordenacao.getAlunosControleFalta(cd_controle_faltas);
                retorno.retorno = alunos;
                if (alunos == null || alunos.Count == 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                };
                return Request.CreateResponse(HttpStatusCode.OK, retorno);

            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }


        #region Controle de Faltas Aluno
        [HttpGet]
        [HttpComponentesAuthorize(Roles = "confal")]
        public HttpResponseMessage getAlunosTurmaControleFalta(int cd_turma, DateTime? dt_inicial, DateTime dt_final, int cd_controle_faltas)
        {
            List<ControleFaltasAlunoUI> retorno = new List<ControleFaltasAlunoUI>();
            try
            {
                ICoordenacaoBusiness businessCoordenacao = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();

                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                retorno = businessCoordenacao.getAlunosTurmaControleFalta(cd_turma, cdEscola, dt_inicial, dt_final, cd_controle_faltas).ToList();
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;

            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "confal")]
        public HttpResponseMessage getAlunoControleFalta(int cd_turma, int cd_aluno, DateTime? dt_inicial, DateTime dt_final)
        {
            ControleFaltasAlunoUI retorno = new ControleFaltasAlunoUI();
            try
            {
                ICoordenacaoBusiness businessCoordenacao = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();

                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                retorno = businessCoordenacao.getAlunoControleFalta(cd_turma, cdEscola, cd_aluno, dt_inicial, dt_final);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;

            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }


        //Método responsável pela emissão do relatório:
        [HttpComponentesAuthorize(Roles = "confal")]
        [Obsolete]
        public HttpResponseMessage GetUrlRelatorioControleFaltas(string sort, int direction, string desc, int cd_turma, int cd_aluno, int assinatura, DateTime? dataIni, DateTime? dataFim)
        {
            ReturnResult retorno = new ReturnResult();
            if (desc == null)
                desc = String.Empty;
            try
            {

                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                // Verifica permissão "Conta Segura".
                bool contaSegura = this.ComponentesUser.Permissao.Contains("ctsg");
                IEmpresaBusiness BusinessEmpresa = (IEmpresaBusiness)base.instanciarBusiness<IEmpresaBusiness>();
                bool masterGeral = BusinessEmpresa.VerificarMasterGeral(this.ComponentesUser.CodUsuario);
                // Pega os parâmetros do usuário para criar a url do relatório:
                string strParametrosSort = "@orientation=LANDSCAPE&@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@descricao=" + desc + "&@cd_turma=" + cd_turma + "&@cd_aluno=" + cd_aluno + "&@assinatura=" + assinatura + "&@dataIni=" + dataIni + "&@dataFim=" + dataFim + "&@cdEscola=" + cdEscola + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=" + "Relatório de Controle de Faltas&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + (int)FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.ControleFaltasSearch;
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
        public HttpResponseMessage componentesPesquisaReportMatricula()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                int cdPessoaUsuario = this.ComponentesUser.CodPessoaUsuario;
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                List<Produto> produtos = coordenacaoBiz.findProduto(ProdutoDataAccess.TipoConsultaProdutoEnum.HAS_ATIVO, null, cdEscola).ToList();
                retorno.retorno = produtos;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage componentesPesquisaTurmaControleFalta()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ControleFaltasUI.ComponentesTurmaControleFalta componentesTurma = new ControleFaltasUI.ComponentesTurmaControleFalta();
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                int cdPessoaUsuario = this.ComponentesUser.CodPessoaUsuario;
                IProfessorBusiness profBiz = (IProfessorBusiness)base.instanciarBusiness<IProfessorBusiness>();
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                ICursoBusiness cursoBiz = (ICursoBusiness)base.instanciarBusiness<ICursoBusiness>();
                componentesTurma.produtos = coordenacaoBiz.findProduto(ProdutoDataAccess.TipoConsultaProdutoEnum.HAS_TURMA, null, cdEscola).ToList();
                componentesTurma.cursos = cursoBiz.getCursos(CursoDataAccess.TipoConsultaCursoEnum.HAS_TURMA, null, null, cdEscola).ToList();
                componentesTurma.niveis = coordenacaoBiz.findNivel(NivelDataAccess.TipoConsultaNivelEnum.HAS_ATIVO)
                    .Where(x => (x.dc_nivel == "Básico" || x.dc_nivel == "Intermediário" || x.dc_nivel == "Avançado")).ToList();
                ProfessorUI prof = profBiz.verificaRetornaSeUsuarioLogadoEProfessor(cdPessoaUsuario, cdEscola);
                if (prof != null && prof.cd_pessoa > 0 && !this.ComponentesUser.IdMaster && !prof.id_coordenador)
                {
                    componentesTurma.professores = new List<ProfessorUI>();
                    componentesTurma.professores.Add(prof);
                    componentesTurma.usuarioSisProf = true;
                }
                else
                    componentesTurma.professores = profBiz.getProfessorReturnProfUI(ProfessorDataAccess.TipoConsultaProfessorEnum.HAS_DESISTENCIA, cdEscola, null).ToList();
                retorno.retorno = componentesTurma;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }
        #endregion

        #region Aulas de Reposicao

        [HttpComponentesAuthorize(Roles = "aurepo")]
        [HttpComponentesAuthorize(Roles = "func")]
        public HttpResponseMessage returnDataAulaReposicaoParaPesquisa(AulaReposicaoUI.AulaReposicaoPesquisa atividadeExtraPesq)
        {
            ReturnResult retorno = new ReturnResult();
            AulaReposicaoUI aulaReposicaoUi = new AulaReposicaoUI();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                IProfessorBusiness profBiz = (IProfessorBusiness)base.instanciarBusiness<IProfessorBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { coordenacaoBiz });
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                aulaReposicaoUi.professores = profBiz.getProfessoresAulaReposicao(cdEscola).ToList();
                aulaReposicaoUi.salas = coordenacaoBiz.getSalasAulaReposicao(cdEscola, SalaDataAccess.TipoConsultaDuracaoEnum.HAS_ATIVO).ToList(); ;
                retorno.retorno = aulaReposicaoUi;

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }

        }

        [HttpComponentesAuthorize(Roles = "aurepo")]
        public HttpResponseMessage getAulaReposicaoSearch(String dataIni, String dataFim, String hrInicial, String hrFinal, int? cd_turma, int? cd_aluno, int? cd_responsavel, int? cd_sala)
        {
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                var dataInicial = Convert.ToDateTime(dataIni);
                var dataFinal = Convert.ToDateTime(dataFim);
                TimeSpan? horaInicial;
                TimeSpan? horaFinal;

                if (hrInicial != "null") horaInicial = TimeSpan.Parse(hrInicial);
                else horaInicial = null;
                if (hrFinal != "null") horaFinal = TimeSpan.Parse(hrFinal);
                else horaFinal = null;

                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var retorno = coordenacaoBiz.searchAulaReposicao(parametros, dataInicial, dataFinal, horaInicial, horaFinal, cd_turma, cd_aluno, cd_responsavel, cd_sala, cdEscola);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                logger.Error("CoordenacaoController searchAulaReposicao - Erro: ", ex);
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(Messages.msgRegCritError),
                    ReasonPhrase = Messages.msgCritError
                });
            }
        }

        [HttpComponentesAuthorize(Roles = "aurepo")]
        [Obsolete]
        public HttpResponseMessage postHorariosDisponiveisAulaRep(String data, int turma, int professor, int? cdAulaReposicao, List<AlunoAulaReposicaoUI> alunos)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                var dataAtividade = Convert.ToDateTime(data);
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var listaHorarios = coordenacaoBiz.getHorariosDisponiveisAulaRep(dataAtividade, turma, professor, cdAulaReposicao, alunos).ToList();
                retorno.retorno = listaHorarios;
                if (listaHorarios.Count() <= 0)
                    retorno.AddMensagem(Messages.msgAulaRepNaoExisteHorarios, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }// try/catch
        }
        [HttpComponentesAuthorize(Roles = "aurepo")]
        [Obsolete]
        public HttpResponseMessage postVerificaHorarioAulaRep(String horaIni, String horaFim, String data, int? cd_aula_reposicao, int cd_turma, int cd_professor, List<AlunoAulaReposicaoUI> alunos)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEmpresa = this.ComponentesUser.CodEmpresa.Value;
                var dataAtividade = Convert.ToDateTime(data);
                TimeSpan horaInicial;
                TimeSpan horaFinal;
                //AulaReposicao atividadeSala;

                if (!String.IsNullOrEmpty(horaIni))
                    horaInicial = TimeSpan.Parse(horaIni);
                else
                    horaInicial = new TimeSpan(0);
                if (!String.IsNullOrEmpty(horaFim))
                    horaFinal = TimeSpan.Parse(horaFim);
                else
                    horaFinal = new TimeSpan(0);
                //var idSala = 0;
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness) base.instanciarBusiness<ICoordenacaoBusiness>();
                var regcount = coordenacaoBiz.verificaHorarioAulaRep(horaInicial, horaFinal, dataAtividade, cd_aula_reposicao, cd_turma, cd_professor, cdEmpresa, alunos);
                retorno.retorno = regcount;
                if (regcount > 0)
                    retorno.AddMensagem(Messages.msgHorarioOcupadoAulaRep, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (AulaReposicaoBusinessException exe)
            {
                return gerarLogException(exe.Message, retorno, logger, exe);
            }
            catch (System.FormatException ex)
            {
                ControllerException exe = new ControllerException("Hora Inicial: " + horaIni + " Hora Final: " + horaFim, ex);
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, exe);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "aurepo.i")]
        public HttpResponseMessage postIncluirAulaReposicao(AulaReposicaoUI aulaReposicaoUi)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { coordenacaoBiz });
                aulaReposicaoUi.cd_pessoa_escola = this.ComponentesUser.CodEmpresa.Value;
                aulaReposicaoUi.dt_aula_reposicao = aulaReposicaoUi.dt_aula_reposicao.Date;
                AulaReposicaoUI inserirAulaReposicao = coordenacaoBiz.addAulaReposicao(aulaReposicaoUi);
                retorno.retorno = inserirAulaReposicao;
                if (inserirAulaReposicao.cd_aula_reposicao <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }

        }

        [HttpComponentesAuthorize(Roles = "aurepo.a")]
        public HttpResponseMessage postAlterarAulaReposicao(AulaReposicaoUI aulaReposicaoUI)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { coordenacaoBiz });
                aulaReposicaoUI.cd_pessoa_escola = this.ComponentesUser.CodEmpresa.Value;
                aulaReposicaoUI.dt_aula_reposicao = aulaReposicaoUI.dt_aula_reposicao.Date;

                AulaReposicaoUI atividadeEdit = coordenacaoBiz.editAulaReposicao(aulaReposicaoUI);

                retorno.retorno = atividadeEdit;
                if (atividadeEdit.cd_aula_reposicao <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }

        }

        [HttpComponentesAuthorize(Roles = "aurepo.e")]
        [HttpComponentesAuthorize(Roles = "func")]
        [HttpComponentesAuthorize(Roles = "alu")]
        public HttpResponseMessage PostDeleteAulaReposicao(List<AulaReposicao> aulaReposicao)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { coordenacaoBiz });
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                var delAulaReposicao = coordenacaoBiz.deleteAllAulaReposicao(aulaReposicao, cdEscola);
                retorno.retorno = delAulaReposicao;
                if (!delAulaReposicao)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
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

        //Método responsável pela emissão do relatório:
        [HttpComponentesAuthorize(Roles = "aurepo")]
        public HttpResponseMessage GetUrlRelatorioAulaReposicao(int cdAulaReposicao)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {

                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                // Pega os parâmetros do usuário para criar a url do relatório:
                string parametros = "@cdEscola=" + cdEscola + "&@cdAulaReposicao=" + cdAulaReposicao;
                //parametros = HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8);
                string parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);
                return Request.CreateResponse(HttpStatusCode.OK, parametrosCript);
            }
            catch (Exception exe)
            {
                return gerarLogException(Messages.msgErroRelatorio, retorno, logger, exe);
            }
        }
        #endregion

        #region AlunoAulaReposicao

        [HttpComponentesAuthorize(Roles = "aurepo")]
        public HttpResponseMessage GetAlunoAulaReposicao(int cd_aula_reposicao)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { coordenacaoBiz });
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                IEnumerable<AlunoAulaReposicaoUI> alunoAulaReposicao = coordenacaoBiz.searchAlunoAulaReposicao(cd_aula_reposicao, cdEscola);
                retorno.retorno = alunoAulaReposicao;
                if (alunoAulaReposicao.Count() <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }

        }

        #endregion

        [HttpComponentesAuthorize(Roles = "mtdes")]
        public HttpResponseMessage getComponentesDesistencia()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ComponentesTurma componentesTurma = new ComponentesTurma();
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                int cdPessoaUsuario = this.ComponentesUser.CodPessoaUsuario;
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                IProfessorBusiness profBiz = (IProfessorBusiness)base.instanciarBusiness<IProfessorBusiness>();
                componentesTurma.motivosDesistencia = coordenacaoBiz.motivosDesistenciaWhitDesistencia(cdEscola).ToList();
                componentesTurma.produtos = coordenacaoBiz.findProduto(ProdutoDataAccess.TipoConsultaProdutoEnum.HAS_DESISTENCIA, null, cdEscola).ToList();
                ProfessorUI prof = profBiz.verificaRetornaSeUsuarioLogadoEProfessor(cdPessoaUsuario, cdEscola);
                if (prof != null && prof.cd_pessoa > 0 && !this.ComponentesUser.IdMaster && !prof.id_coordenador)
                {
                    componentesTurma.professores = new List<ProfessorUI>();
                    componentesTurma.professores.Add(prof);
                    componentesTurma.usuarioSisProf = true;
                }
                else
                    componentesTurma.professores = profBiz.getProfessorReturnProfUI(ProfessorDataAccess.TipoConsultaProfessorEnum.HAS_DESISTENCIA, cdEscola, null).ToList();
                retorno.retorno = componentesTurma;
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        #region ApiNewCyber
            [HttpPost]
            [AllowAnonymous]
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<HttpResponseMessage> postExecutaCyber()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
                ReturnResult retorno = new ReturnResult();
                retorno.retorno = "";
                try
                {
                    IApiNewCyberBusiness businessApiNewCyber = (IApiNewCyberBusiness) base.instanciarBusiness<IApiNewCyberBusiness>();
                    //configuraBusiness(new List<IGenericBusiness>() { businessApiNewCyber });

                    string result = businessApiNewCyber.postExecutaCyber("https://cyberhomolog.fisk.com.br:172/cyberfisk30/ws/api.aspx",
                        ApiCyberComandosNames.ATIVA_ALUNO, "JKGkgYklKjcHRAhKkv986FHhU55rGvf4tyg6Uiu66HgHggthgh", "codigo=3761");
                    retorno.retorno = result;
                    //if (alunos == null || alunos.Count == 0)
                    //{
                    //    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                    //};
                    return Request.CreateResponse(HttpStatusCode.OK, retorno);

                }
                catch (ApiNewCyberException ex)
                {
                     logger.Error("CoordenacaoController postInsertEvento - Erro: ", ex);
                     retorno.AddMensagem(Messages.msgNotIncludReg, ex.Message + ex.StackTrace + ex.InnerException, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                     return Request.CreateResponse(HttpStatusCode.BadRequest, retorno);
            }
                catch (Exception ex)
                {
                    logger.Error("CoordenacaoController postInsertEvento - Erro: ", ex);
                    retorno.AddMensagem(Messages.msgNotIncludReg, ex.Message + ex.StackTrace + ex.InnerException, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                    return Request.CreateResponse(HttpStatusCode.BadRequest, retorno);
            }
            }


        #endregion

        #region MensagemAvaliacao

        [HttpComponentesAuthorize(Roles = "mensava")]
        public HttpResponseMessage getMensagemAvaliacaoSearch(String desc, bool inicio, int status, int? produto, int? curso)
        {
            try
            {
                if (desc == null)
                {
                    desc = String.Empty;
                }
                var request = this.Request;
                RangeHeaderValue rangeHeaderValue = this.Request.Headers.Range;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var retorno = coordenacaoBiz.getMensagemAvaliacaoSearch(parametros, desc, inicio, getStatus(status), produto, curso);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }


        public HttpResponseMessage obterRecursosMensagemAvaliacao(MensagemAvaliacaoSearchUI mensageAvaliacao)
        {
            ReturnResult retorno = new ReturnResult();
            AtividadeExtraUI atividadeExtraUI = new AtividadeExtraUI();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();


                var ativoProduto = FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess.ProdutoDataAccess.TipoConsultaProdutoEnum.HAS_ATIVO;
                var ativoCurso = FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess.CursoDataAccess.TipoConsultaCursoEnum.HAS_ATIVO;

                mensageAvaliacao.listaProdutos = coordenacaoBiz.findProduto(ativoProduto, mensageAvaliacao.cd_produto, null).ToList();
                mensageAvaliacao.listaCursos = coordenacaoBiz.findCurso(ativoCurso, mensageAvaliacao.cd_curso, mensageAvaliacao.cd_produto, null).ToList();


                retorno.retorno = mensageAvaliacao;
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "mensava.i")]
        public HttpResponseMessage postIncluirMensagemAvaliacao(MensagemAvaliacaoSearchUI mensagemAvaliacao)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { coordenacaoBiz });
                List<MensagemAvaliacaoSearchUI> listaMensagem = coordenacaoBiz.addMensagemAvaliacao(mensagemAvaliacao);
                retorno.retorno = listaMensagem;
                if (listaMensagem.Count <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }

        }

        [HttpComponentesAuthorize(Roles = "mensava.a")]
        public HttpResponseMessage postEditMensagemAvaliacao(MensagemAvaliacao mensagemAvaliacao)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var editMensagemAvaliacao = coordenacaoBiz.editMensagemAvaliacao(mensagemAvaliacao);
                retorno.retorno = editMensagemAvaliacao;
                if (editMensagemAvaliacao.cd_mensagem_avaliacao <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                else
                {
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "mensava.e")]
        public HttpResponseMessage postDeleteMensagemAvaliacao(List<MensagemAvaliacao> listaMensagemAvaliacao)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var delMensagemAvaliacao = coordenacaoBiz.deleteAllMensagemAvaliacao(listaMensagemAvaliacao);
                retorno.retorno = delMensagemAvaliacao;
                if (!delMensagemAvaliacao)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
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

        [HttpComponentesAuthorize(Roles = "mensava")]
        public HttpResponseMessage GetUrlRelatorioMensagemAvaliacao(string sort, int direction, String desc, bool inicio, int status, int? produto, int? curso)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                // Pega os parâmetros do usuário  criar a url do relatório:
                var strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@descricao=" + desc + "&@inicio=" + inicio + "&@status=" + status + "&@produto=" + produto + "&@curso=" + curso + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=" + "Relatório de Mensagem - Avaliação&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.MensagemAvaliacaoSearch;
                //parâmetros = HttpUtility.UrlEncode(parâmetros, System.Text.Encoding.UTF8);
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

        [HttpComponentesAuthorize(Roles = "msgavalu.i")]
        public HttpResponseMessage postIncluirMensagemAvaliacaoAluno(MensagemAvaliacaoAlunoUI mensagemAvaliacao)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { coordenacaoBiz });
                List<MensagemAvaliacaoAlunoUI> listaMensagem = coordenacaoBiz.addMensagemAvaliacaoAluno(mensagemAvaliacao);
                retorno.retorno = listaMensagem;
                if (listaMensagem.Count <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }

        }
        [HttpComponentesAuthorize(Roles = "msgavalu.a")]
        public HttpResponseMessage postEditMensagemAvaliacaoAluno(MensagemAvaliacaoAluno mensagemAvaliacao)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var editMensagemAvaliacao = coordenacaoBiz.editMensagemAvaliacaoAluno(mensagemAvaliacao);
                retorno.retorno = editMensagemAvaliacao;
                if (editMensagemAvaliacao.cd_mensagem_avaliacao <= 0)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                else
                {
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "msgavalu.e")]
        public HttpResponseMessage postDeleteMensagemAvaliacaoAluno(List<MensagemAvaliacaoAluno> listaMensagemAvaliacao)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var delMensagemAvaliacao = coordenacaoBiz.deleteAllMensagemAvaliacaoAluno(listaMensagemAvaliacao);
                retorno.retorno = delMensagemAvaliacao;
                if (!delMensagemAvaliacao)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
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
        [HttpComponentesAuthorize(Roles = "msgavalu")]
        public HttpResponseMessage findMsgAlunobyTipo(int cdTipoAvaliacao, int cdAluno, int cdProduto, int cdCurso)
        {
            try
            {
                var request = this.Request;
                RangeHeaderValue rangeHeaderValue = this.Request.Headers.Range;
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var retorno = coordenacaoBiz.findMsgAlunobyTipo(cdTipoAvaliacao, cdAluno, cdProduto, cdCurso);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        #endregion

        #region PerdaMaterial
        [HttpComponentesAuthorize(Roles = "perdm")]
        public HttpResponseMessage getPerdaMaterialSearch(int? cd_aluno, int? nm_contrato, int? cd_movimento, int? cd_item, string dtInicial, string dtFinal, int status)
        {
            try
            {
                DateTime? dtaInicial = dtInicial == null ? null : (DateTime?)Convert.ToDateTime(dtInicial);
                DateTime? dtaFinal = dtFinal == null ? null : (DateTime?)Convert.ToDateTime(dtFinal);
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                var request = this.Request;
                RangeHeaderValue rangeHeaderValue = this.Request.Headers.Range;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                var retorno = coordenacaoBiz.getPerdaMaterialSearch(parametros, cd_aluno, nm_contrato, cd_movimento, cd_item, dtaInicial, dtaFinal, status, cdEscola);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "perdm.i")]
        public HttpResponseMessage postPerdaMaterial(PerdaMaterial perdaMaterialUI)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                PerdaMaterialUI perdaMaterial = coordenacaoBiz.addPerdaMaterial(perdaMaterialUI);
                retorno.retorno = perdaMaterial;
                if (perdaMaterial == null)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, retorno);
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
        }
        
        [HttpComponentesAuthorize(Roles = "perdm.a")]
        public HttpResponseMessage postEditPerdaMaterial(PerdaMaterial perdaMaterialUI)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                PerdaMaterialUI perdaMaterial = coordenacaoBiz.editPerdaMaterial(perdaMaterialUI);
                retorno.retorno = perdaMaterial;
                if (perdaMaterial == null)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, retorno);
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "perdm.e")]
        public HttpResponseMessage postDeletePerdaMaterial(List<PerdaMaterial> perdaMaterialList)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();


                var delPerdaMaterial = coordenacaoBiz.deletePerdaMaterial(perdaMaterialList);
                retorno.retorno = delPerdaMaterial;
                if (!delPerdaMaterial)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                else
                    retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (CoordenacaoBusinessException ex)
            {
                if (ex.tipoErro == CoordenacaoBusinessException.TipoErro.ERRO_EDICAO_PERDA_MATERIAL_FECHADO)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                if (ex.tipoErro == CoordenacaoBusinessException.TipoErro.ERRO_EXCLUSAO_PERDA_MATERIAL_FECHADO)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                return gerarLogException(Messages.msgNotDeletedReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotDeletedReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "perdm")]
        public HttpResponseMessage postProcessarPerdaMaterial(PerdaMaterial perdaMaterial)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();

                if (perdaMaterial != null && perdaMaterial.id_status_perda == (int)PerdaMaterialDataAccess.TipoPerdaMaterialStatus.FECHADO)
                {
                    throw new CoordenacaoBusinessException(Messages.msgProcessarPerdaMaterialFechado, null, CoordenacaoBusinessException.TipoErro.ERRO_PROCESSAR_PERDA_MATERIAL_FECHADO, false);
                }

                int cd_usuario = ComponentesUser.CodUsuario;
                int fuso = (int)this.ComponentesUser.IdFusoHorario;

                HttpResponseMessage response;
                int ret = coordenacaoBiz.processarPerdaMaterial(perdaMaterial, cd_usuario, fuso);

                if (ret == 1)
                {
                    string msg = Messages.msgProcedureError;

                    retorno.AddMensagem(Messages.msgProcedureError, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);

                    response = Request.CreateResponse(HttpStatusCode.BadRequest, JsonConvert.SerializeObject(retorno));
                    configureHeaderResponse(response, null);
                    return response;
                }

                retorno.retorno = new {data = "Perda de material processada com sucesso" };
                retorno.AddMensagem("Perda de material processada com sucesso", null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                 response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (CoordenacaoBusinessException ex)
            {
                if (ex.tipoErro == CoordenacaoBusinessException.TipoErro.ERRO_EDICAO_PERDA_MATERIAL_FECHADO)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                if (ex.tipoErro == CoordenacaoBusinessException.TipoErro.ERRO_EXCLUSAO_PERDA_MATERIAL_FECHADO)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                return gerarLogException(Messages.msgNotDeletedReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                var message = Utils.Utils.innerMessage(ex);
                if (message != "")
                {
                    CoordenacaoBusinessException fx = new CoordenacaoBusinessException(message, ex, 0, false);
                    //retorno.AddMensagem(message, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                    return gerarLogException(message, retorno, logger, fx);
                }
                else
                {
                    return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
                }
            }
        }

        [HttpComponentesAuthorize(Roles = "perdm")]
        public HttpResponseMessage getUrlRelatorioPerdaMaterial(string sort, int direction, int? cd_aluno, int? nm_contrato, int? cd_movimento, int? cd_item, string dtInicial, string dtFinal, int status)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                // Pega os parâmetros do usuário para criar a url do relatório:
                var strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@cd_aluno=" + cd_aluno + "&@nm_contrato=" + nm_contrato + "&@cd_movimento=" + cd_movimento + "&@cd_item=" + cd_item + "&@dtInicial=" + dtInicial + "&@dtFinal=" + dtFinal + "&@status=" + status + "&@cdEscola=" + cdEscola + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=" + "Relatório de Perda de Material&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.PerdaMaterialSearch;
                //parametros = HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8);
                var parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);

                HttpResponseMessage response = null;

                if (parametrosCript == null)
                    response = Request.CreateResponse(HttpStatusCode.NotFound);
                else
                    response = Request.CreateResponse(HttpStatusCode.OK, parametrosCript);

                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception exe)
            {
                return gerarLogException(Messages.msgErroRelatorio, retorno, logger, exe);
            }
        }


        #endregion

        #region CargaHoraria - index
        public HttpResponseMessage getExisteCargaHorariaProximaMaxima()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cd_escola = this.ComponentesUser.CodEmpresa.Value;
                int cdPessoaUsuario = this.ComponentesUser.CodPessoaUsuario;

                ICoordenacaoBusiness coordenacaoBiz = (ICoordenacaoBusiness)base.instanciarBusiness<ICoordenacaoBusiness>();
                retorno.retorno = coordenacaoBiz.getExisteCargaHorariaProximaMaxima(cdPessoaUsuario, cd_escola);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception exe)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, exe);
            }
        }
        #endregion

    }
}
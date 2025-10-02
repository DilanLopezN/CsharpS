using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess;
using Newtonsoft.Json;
using log4net;
using System.Web;
using FundacaoFisk.SGF.Web.Services.Financeiro.Model;
using FundacaoFisk.SGF.Utils.Messages;
using Componentes.GenericController;
using Componentes.GenericBusiness.Comum;
using System.Globalization;
using FundacaoFisk.SGF.Utils;
using FundacaoFisk.SGF.Web.Services.Financeiro.Business;
using FundacaoFisk.SGF.Web.Services.Empresa.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Empresa.Model;


namespace FundacaoFisk.SGF.Web.Services.Financeiro.Controllers
{
    public class FinanceiroController : ComponentesApiController
    {
        //Declaração de Atributos
        private static readonly ILog logger = LogManager.GetLogger(typeof(FinanceiroController));
        //propriedades usadas no plano de contas
        private bool was_add { get; set; }
        private bool was_del { get; set; }

        public FinanceiroController()
        {
        }

        #region Cadastros Básicos

        #region Grupo de Estoque

        // GET api/<controller>
        [HttpComponentesAuthorize(Roles = "gest")]
        public HttpResponseMessage GetGrupoEstoqueSearch(string descricao, bool inicio, int status, int categoria)
        {
            try
            {
                if (descricao == null)
                    descricao = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var retorno = Business.getGrupoEstoqueSearch(parametros, descricao, inicio, getStatus(status), categoria);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Componentes.Utils.Messages.Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "gest")]
        public HttpResponseMessage GetAllGrupoEstoqueAtivo()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IEmpresaBusiness BusinessEmpresa = (IEmpresaBusiness)base.instanciarBusiness<IEmpresaBusiness>();
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                bool masterGeral = BusinessEmpresa.VerificarMasterGeral(this.ComponentesUser.CodUsuario);
                var listGrupo = Business.findAllGrupoAtivo(0, masterGeral);
                retorno.retorno = listGrupo;
                if (listGrupo.Count() <= 0)
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


        [HttpComponentesAuthorize(Roles = "gest")]
        public HttpResponseMessage GetAllGrupoEstoqueItem()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cd_escola = (int)this.ComponentesUser.CodEmpresa;
                IEmpresaBusiness BusinessEmpresa = (IEmpresaBusiness)base.instanciarBusiness<IEmpresaBusiness>();
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                bool masterGeral = BusinessEmpresa.VerificarMasterGeral(this.ComponentesUser.CodUsuario);
                var listGrupo = Business.findAllGrupoWithItem(cd_escola, masterGeral);
                retorno.retorno = listGrupo;
                if (listGrupo.Count() <= 0)
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

        // GET api/<controller>/1
        [HttpComponentesAuthorize(Roles = "gest")]
        public HttpResponseMessage GetGrupoEstoqueById(int id)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var ge = Business.getGrupoEstoqueById(id);
                retorno.retorno = ge;
                if (ge.cd_grupo_estoque <= 0)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        // Put api/<controller>/5
        [HttpComponentesAuthorize(Roles = "gest.a")]
        public HttpResponseMessage PostAlterarGrupoEstoque(GrupoEstoque grupoEstoque)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                IEmpresaBusiness BusinessEmpresa = (IEmpresaBusiness)base.instanciarBusiness<IEmpresaBusiness>();
                bool masterGeral = BusinessEmpresa.VerificarMasterGeral(this.ComponentesUser.CodUsuario);
                var ge = Business.putGrupoEstoque(grupoEstoque, masterGeral);
                retorno.retorno = ge;
                if (ge.cd_grupo_estoque <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (FinanceiroBusinessException ex)
            {
                if (ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_CATEGORIA_GRUPO_ITEM)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }

        }
        // Post api/<controller>/5
        [HttpComponentesAuthorize(Roles = "gest.i")]
        public HttpResponseMessage PostGrupoEstoque(GrupoEstoque grupoEstoque)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {

                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                IEmpresaBusiness BusinessEmpresa = (IEmpresaBusiness)base.instanciarBusiness<IEmpresaBusiness>();
                bool masterGeral = BusinessEmpresa.VerificarMasterGeral(this.ComponentesUser.CodUsuario);
                var ge = Business.postGrupoEstoque(grupoEstoque, masterGeral);
                retorno.retorno = ge;
                if (ge.cd_grupo_estoque <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (FinanceiroBusinessException ex)
            {
                if (ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_CATEGORIA_GRUPO_ITEM)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }

        }
        // Delte api/<controller>
        [HttpComponentesAuthorize(Roles = "gest.e")]
        public HttpResponseMessage PostDeleteGrupoEstoque(List<GrupoEstoque> gruposEstoque)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                IEmpresaBusiness BusinessEmpresa = (IEmpresaBusiness)base.instanciarBusiness<IEmpresaBusiness>();
                bool masterGeral = BusinessEmpresa.VerificarMasterGeral(this.ComponentesUser.CodUsuario);
                var deleted = Business.deleteAllGrupo(gruposEstoque, masterGeral);
                retorno.retorno = deleted;
                if (!deleted)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (FinanceiroBusinessException ex)
            {
                if (ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_CATEGORIA_GRUPO_ITEM)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotDeletedReg, retorno, logger, ex);
            }

        }

        //Método responsável pela emissão do relatório:
        [HttpComponentesAuthorize(Roles = "gest")]
        public HttpResponseMessage GetUrlRelatorioGrupoEstoque(string sort, int direction, string desc, bool inicio, int status, int categoria)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                // Pega os parâmetros do usuário para criar a url do relatório:
                string strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@descricao=" + desc + "&@inicio=" + inicio + "&@status=" + status + "&@categoria=" + categoria + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=" + "Relatório de Grupo de Item&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.GrupoEstoqueSearch;
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

        #region Movimentação Financeira

        // GET api/<controller>
        [HttpComponentesAuthorize(Roles = "mvfin")]
        public HttpResponseMessage GetMovimentacaoFinanceiraSearch(string descricao, bool inicio, int status)
        {
            try
            {
                if (descricao == null)
                    descricao = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var retorno = Business.getMovimentacaoFinanceiraSearch(parametros, descricao, inicio, getStatus(status));
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
        [HttpComponentesAuthorize(Roles = "mvfin")]
        public HttpResponseMessage GetMovimentacaoFinanceiraById(int id)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var ge = Business.getMovimentacaoFinanceiraById(id);
                retorno.retorno = ge;
                if (ge.cd_movimentacao_financeira <= 0)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        // Put api/<controller>/5
        [HttpComponentesAuthorize(Roles = "mvfin.a")]
        public HttpResponseMessage PostAlterarMovimentacaoFinanceira(MovimentacaoFinanceira movimentacaofinanceira)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var ge = Business.putMovimentacaoFinanceira(movimentacaofinanceira);
                retorno.retorno = ge;
                if (ge.cd_movimentacao_financeira <= 0)
                    retorno.AddMensagem(Messages.msgNotUpReg, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
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
        [HttpComponentesAuthorize(Roles = "mvfin.i")]
        public HttpResponseMessage PostMovimentacaoFinanceira(MovimentacaoFinanceira movimentacaofinanceira)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var ge = Business.postMovimentacaoFinanceira(movimentacaofinanceira);
                retorno.retorno = ge;
                if (ge.cd_movimentacao_financeira <= 0)
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
        [HttpComponentesAuthorize(Roles = "mvfin.e")]
        public HttpResponseMessage PostDeleteMovimentacaoFinanceira(List<MovimentacaoFinanceira> movimentacoesfinanceira)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var deleted = Business.deleteAllMovimentacao(movimentacoesfinanceira);
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
        [HttpComponentesAuthorize(Roles = "mvfin")]
        public HttpResponseMessage GetUrlRelatorioMovimentacaoFinanceira(string sort, int direction, string desc, bool inicio, int status)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                // Pega os parâmetros do usuário para criar a url do relatório:
                string strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@descricao=" + desc + "&@inicio=" + inicio + "&@status=" + status + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=" + "Relatório de Movimentação Financeiro&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.MovimentacaoFinanceiraSearch;
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

        [HttpComponentesAuthorize(Roles = "coco")]
        [HttpComponentesAuthorize(Roles = "mvfin")]
        [HttpGet]
        public HttpResponseMessage getMovimentacaoTransferencia(int cd_movimentacao_financeira)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEmpresa = this.ComponentesUser.CodEmpresa.Value;
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                IEnumerable<MovimentacaoFinanceira> movimentacao = Business.getMovimentacaoTransferencia(cdEmpresa, cd_movimentacao_financeira);
                retorno.retorno = movimentacao;

                if (movimentacao.Count() <= 0)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        #endregion

        #region Tipo Liquidação

        // GET api/<controller>
        [HttpComponentesAuthorize(Roles = "tpliq")]
        public HttpResponseMessage GetTipoLiquidacaoSearch(string descricao, bool inicio, int status)
        {
            try
            {
                if (descricao == null)
                    descricao = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var retorno = Business.getTipoLiquidacaoSearch(parametros, descricao, inicio, getStatus(status));
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        // GET api/<controller>
        [HttpComponentesAuthorize(Roles = "orgfin")]
        public HttpResponseMessage getOrgaoFinanceiroSearch(string descricao, bool inicio, int status)
        {
            try
            {
                if (descricao == null)
                    descricao = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var retorno = Business.getOrgaoFinanceiroSearch(parametros, descricao, inicio, getStatus(status));
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        // Post api/<controller>/5
        [HttpComponentesAuthorize(Roles = "orgfin.i")]
        public HttpResponseMessage PostOrgaoFinanceiro(OrgaoFinanceiro orgaoFinanceiro)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var tipoF = Business.postOrgaoFinanceiro(orgaoFinanceiro);
                retorno.retorno = tipoF;
                if (tipoF.cd_orgao_financeiro <= 0)
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
        // Put api/<controller>/5
        [HttpComponentesAuthorize(Roles = "orgfin.a")]
        public HttpResponseMessage PostAlterarOrgaoFinanceiro(OrgaoFinanceiro orgaoFinanceiro)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var tipoF = Business.putOrgaoFinanceiro(orgaoFinanceiro);
                retorno.retorno = tipoF;
                if (tipoF.cd_orgao_financeiro <= 0)
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

        // Delte api/<controller>
        [HttpComponentesAuthorize(Roles = "orgfin.e")]
        public HttpResponseMessage PostDeleteOrgaoFinanceiro(List<OrgaoFinanceiro> orgaosFinanceiros)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var deleted = Business.deleteAllOrgaoFinanceiro(orgaosFinanceiros);
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
        // [HttpComponentesAuthorize(Roles = "")]
        [HttpComponentesAuthorize(Roles = "orgfin")]
        public HttpResponseMessage GetUrlRelatorioOrgaoFinanceiro(string sort, int direction, string desc, bool inicio, int status)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                // Pega os parâmetros do usuário para criar a url do relatório:
                string strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@descricao=" + desc + "&@inicio=" + inicio + "&@status=" + status + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=" + "Relatório de Orgão Financeiro&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.OrgaoFinanceiroSearch;
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
        [HttpComponentesAuthorize(Roles = "tpliq")]
        public HttpResponseMessage GetTipoLiquidacaoById(int id)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var tipoLiq = Business.getTipoLiquidacaoById(id);
                retorno.retorno = tipoLiq;
                if (tipoLiq.cd_tipo_liquidacao <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "tpliq")]
        public HttpResponseMessage getTipoLiquidacao(bool status, int? cd_tipo_liquidacao)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                var tipo_consulta = status ? TipoLiquidacaoDataAccess.TipoConsultaTipoLiquidacaoEnum.HAS_ATIVO : TipoLiquidacaoDataAccess.TipoConsultaTipoLiquidacaoEnum.HAS_BAIXA_TITULO;
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var tipoLiq = Business.getTipoLiquidacao(tipo_consulta, cd_tipo_liquidacao);
                retorno.retorno = tipoLiq;
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);

            }
        }

        // Put api/<controller>/5
        [HttpComponentesAuthorize(Roles = "tpliq.a")]
        public HttpResponseMessage PostAlterarTipoLiquidacao(TipoLiquidacao tipoliquidacao)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var tipoLiq = Business.putTipoLiquidacao(tipoliquidacao);
                retorno.retorno = tipoLiq;
                if (tipoLiq.cd_tipo_liquidacao <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (FinanceiroBusinessException exe)
            {
                if (exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_TIPO_NF_UTILIZADO)
                    return gerarLogException(exe.Message, retorno, logger, exe);

                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, exe);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgUpdateSucess, retorno, logger, ex);
            }

        }

        // Post api/<controller>/5
        [HttpComponentesAuthorize(Roles = "tpliq.i")]
        public HttpResponseMessage PostTipoLiquidacao(TipoLiquidacao tipoliquidacao)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var tipoLiq = Business.postTipoLiquidacao(tipoliquidacao);
                retorno.retorno = tipoLiq;
                if (tipoLiq.cd_tipo_liquidacao <= 0)
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
        [HttpComponentesAuthorize(Roles = "tpliq.e")]
        public HttpResponseMessage PostDeleteTipoLiquidacao(List<TipoLiquidacao> tiposliquidacoes)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var deleted = Business.deleteAllTipoLiquidacao(tiposliquidacoes);
                retorno.retorno = deleted;
                if (!deleted)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;

            }
            catch (FinanceiroBusinessException exe)
            {
                if (exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_TIPO_NF_UTILIZADO)
                    return gerarLogException(exe.Message, retorno, logger, exe);

                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, exe);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotDeletedReg, retorno, logger, ex);
            }

        }

        //Método responsável pela emissão do relatório:
        // [HttpComponentesAuthorize(Roles = "")]
        [HttpComponentesAuthorize(Roles = "tpliq")]
        public HttpResponseMessage GetUrlRelatorioTipoLiquidacao(string sort, int direction, string desc, bool inicio, int status)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                // Pega os parâmetros do usuário para criar a url do relatório:
                string strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@descricao=" + desc + "&@inicio=" + inicio + "&@status=" + status + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=" + "Relatório de Tipo Liquidação&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.TipoLiquidacaoSearch;
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

        #region Tipo Financeiro

        // GET api/<controller>
        [HttpComponentesAuthorize(Roles = "tpfin")]
        public HttpResponseMessage GetTipoFinanceiroSearch(string descricao, bool inicio, int status)
        {
            try
            {
                if (descricao == null)
                    descricao = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var retorno = Business.getTipoFinanceiroSearch(parametros, descricao, inicio, getStatus(status));
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
        [HttpComponentesAuthorize(Roles = "tpfin")]
        public HttpResponseMessage GetTipoFinanceiroById(int id)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var tipoF = Business.getTipoFinanceiroById(id);
                retorno.retorno = tipoF;
                if (tipoF.cd_tipo_financeiro <= 0)
                    retorno.AddMensagem("Nenhum registro encontrado.", null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "tpfin")]
        public HttpResponseMessage getTipoFinanceiroAtivo()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var tipoF = Business.getTipoFinanceiroAtivo();
                retorno.retorno = tipoF;


                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }
        // Put api/<controller>/5
        [HttpComponentesAuthorize(Roles = "tpfin.a")]
        public HttpResponseMessage PostAlterarTipoFinanceiro(TipoFinanceiro tipofinanceiro)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var tipoF = Business.putTipoFinanceiro(tipofinanceiro);
                retorno.retorno = tipoF;
                if (tipoF.cd_tipo_financeiro <= 0)
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
        [HttpComponentesAuthorize(Roles = "tpfin.i")]
        public HttpResponseMessage PostTipoFinanceiro(TipoFinanceiro tipofinanceiro)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var tipoF = Business.postTipoFinanceiro(tipofinanceiro);
                retorno.retorno = tipoF;
                if (tipoF.cd_tipo_financeiro <= 0)
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
        [HttpComponentesAuthorize(Roles = "tpfin.e")]
        public HttpResponseMessage PostDeleteTipoFinanceiro(List<TipoFinanceiro> tiposfinanceiros)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var deleted = Business.deleteAllTipoFinanceiro(tiposfinanceiros);
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
        // [HttpComponentesAuthorize(Roles = "")]
        [HttpComponentesAuthorize(Roles = "tpfin")]
        public HttpResponseMessage GetUrlRelatorioTipoFinanceiro(string sort, int direction, string desc, bool inicio, int status)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                // Pega os parâmetros do usuário para criar a url do relatório:
                string strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@descricao=" + desc + "&@inicio=" + inicio + "&@status=" + status + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=" + "Relatório de Tipo Financeiro&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.TipoFinanceiroSearch;
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

        #region Tipo Desconto

        // GET api/<controller>
        [HttpComponentesAuthorize(Roles = "tpdes")]
        public HttpResponseMessage getTipoDescontoSearch(string descricao, bool inicio, int status, int incideBaixa, int pparc, decimal? percentual)
        {
            try
            {
                if (descricao == null)
                    descricao = String.Empty;
                int cdEscola = ComponentesUser.CodEmpresa.Value;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var retorno = Business.getTipoDescontoSearch(parametros, descricao, inicio, getStatus(status), getStatus(incideBaixa), getStatus(pparc), percentual, cdEscola);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        // Delte api/<controller>
        [HttpComponentesAuthorize(Roles = "tpdes.e")]
        public HttpResponseMessage PostDeleteTipoDesconto(List<TipoDescontoUI> tiposdesconto)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                IEmpresaBusiness BusinessEmpresa = (IEmpresaBusiness)base.instanciarBusiness<IEmpresaBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });
                bool isMasterGeral = BusinessEmpresa.VerificarMasterGeral(this.ComponentesUser.CodUsuario);

                int cd_escola = ComponentesUser.CodEmpresa.Value;
                var idMaster = (bool)this.ComponentesUser.IdMaster;
                ICollection<EmpresaSession> listEmp = BusinessEmpresa.findEmpresaSessionByLogin(ComponentesUser.Identity.Name, idMaster, true);
                //Se é usuario master geral busca todas as escolas:
                if (idMaster)
                    if (listEmp.Count <= 0)
                        listEmp = BusinessEmpresa.findAllEmpresaSession();


                var deleted = Business.deleteAllTipoDesconto(tiposdesconto, isMasterGeral, cd_escola, listEmp);

                retorno.retorno = deleted;
                if (!deleted)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }

            catch (FinanceiroBusinessException ex)
            {
                if (ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_EXCLUIR_TP_DESC || 
                    ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_EXCLUIR_TP_DESC_ESCOLA ||
                    ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_USANDO_TIPO_DESCONTO)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotDeletedReg, retorno, logger, ex);
            }

        }

        //Método responsável pela emissão do relatório:
        [HttpComponentesAuthorize(Roles = "tpdes")]
        public HttpResponseMessage GetUrlRelatorioTipoDesconto(string sort, int direction, string desc, bool inicio, int status, int incideBaixa, int pparc, string percentual)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                int cdEscola = ComponentesUser.CodEmpresa.Value;
                // Pega os parâmetros do usuário para criar a url do relatório:
                string strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@descricao=" + desc + "&@inicio=" + inicio + "&@status=" + status + "&@incideBaixa=" + incideBaixa + "&@pparc=" + pparc + "&@percentual=" + percentual + "&@cdEscola=" + cdEscola + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=" + "Relatório de Tipo Desconto&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.TipoDescontoSearch;
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

        #region Grupo de Contas

        // GET api/<controller>
        [HttpComponentesAuthorize(Roles = "gruco")]
        public HttpResponseMessage GetGrupoContaSearch(string descricao, bool inicio, int tipoGrupo)
        {
            try
            {
                if (descricao == null)
                    descricao = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var retorno = Business.getGrupoContaSearch(parametros, descricao, inicio, tipoGrupo);
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
        [HttpComponentesAuthorize(Roles = "gruco")]
        public HttpResponseMessage GetGrupoContaById(int id)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var grupo = Business.getGrupoContaById(id);
                retorno.retorno = grupo;
                if (grupo.cd_grupo_conta <= 0)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }
        // GET api/<controller>/1
        [HttpComponentesAuthorize(Roles = "gruco")]
        public HttpResponseMessage GetAllGrupoConta()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var grupo = Business.getAllGrupoConta().ToList();
                retorno.retorno = grupo;
                if (grupo.Count() < 0)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        // Put api/<controller>/5
        [HttpComponentesAuthorize(Roles = "gruco.a")]
        public HttpResponseMessage PostAlterarGrupoConta(GrupoConta GrupoConta)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var grupo = Business.putGrupoConta(GrupoConta);
                retorno.retorno = grupo;
                if (grupo.cd_grupo_conta <= 0)
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
        [HttpComponentesAuthorize(Roles = "gruco.i")]
        public HttpResponseMessage PostGrupoConta(GrupoConta GrupoConta)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var grupo = Business.postGrupoConta(GrupoConta);
                retorno.retorno = grupo;
                if (grupo.cd_grupo_conta <= 0)
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
        [HttpComponentesAuthorize(Roles = "gruco.e")]
        public HttpResponseMessage PostDeleteGrupoConta(List<GrupoConta> grupoConta)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var deleted = Business.deleteAllGrupoConta(grupoConta);
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
        [HttpComponentesAuthorize(Roles = "gruco")]
        public HttpResponseMessage GetUrlRelatorioGrupoConta(string sort, int direction, string desc, bool inicio, int tipoGrupo)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                // Pega os parâmetros do usuário para criar a url do relatório:
                string strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@descricao=" + desc + "&@inicio=" + inicio + "&@tipoGrupo=" + tipoGrupo + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=" + "Relatório de Grupo de Contas&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.GrupoContaSearch;
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

        [HttpComponentesAuthorize(Roles = "gruco")]
        [HttpComponentesAuthorize(Roles = "plct")]
        public HttpResponseMessage getPlanoContasWithMovimento(int tipoMovimento, string descricao, bool inicio)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cd_escola = ComponentesUser.CodEmpresa.Value;
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var planoContas = Business.getPlanoContasWithMovimento(cd_escola, tipoMovimento, descricao, inicio);
                retorno.retorno = planoContas;

                if (planoContas == null || planoContas.Count() <= 0)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                return Request.CreateResponse(HttpStatusCode.OK, retorno);

            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "gruco")]
        [HttpComponentesAuthorize(Roles = "plct")]
        public HttpResponseMessage getPlanoContasWithContaCorrente(string descricao, bool inicio)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cd_escola = ComponentesUser.CodEmpresa.Value;
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var planoContas = Business.getPlanoContasTreeSearchWhitContaCorrente(cd_escola, descricao, inicio);
                retorno.retorno = planoContas;

                if (planoContas == null || planoContas.Count() <= 0)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                return Request.CreateResponse(HttpStatusCode.OK, retorno);

            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
            }
        }

        #endregion

        #region Subgrupo de Contas

        // GET api/<controller>
        [HttpComponentesAuthorize(Roles = "subgr")]
        public HttpResponseMessage GetSubgrupoContaSearch(string descricao, bool inicio, int cdGrupo, int tipoNivel)
        {
            try
            {
                if (descricao == null)
                    descricao = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var retorno = Business.getSubgrupoContaSearch(parametros, descricao, inicio, cdGrupo, SubgrupoConta.parseTipoNivel(tipoNivel));

                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, SubGrupoSort.parseSubGrupoForSubgrupoContaUI(retorno));

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        // GET api/<controller>/1
        [HttpComponentesAuthorize(Roles = "subgr")]
        public HttpResponseMessage GetSubgrupoContaById(int id)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var grupo = Business.getSubgrupoContaById(id);
                retorno.retorno = grupo;
                if (grupo.cd_subgrupo_conta <= 0)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }
        // GET api/<controller>/1
        [HttpComponentesAuthorize(Roles = "subgr")]
        public HttpResponseMessage GetAllSubgrupoConta()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var grupo = Business.getAllSubgrupoConta().ToList();
                retorno.retorno = grupo;
                if (grupo.Count() < 0)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        // Put api/<controller>/5
        [HttpComponentesAuthorize(Roles = "subgr.a")]
        public HttpResponseMessage PostAlterarSubgrupoConta(SubgrupoConta subgrupoConta)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });
                if (subgrupoConta.cd_subgrupo_pai != null && subgrupoConta.cd_subgrupo_pai > 0)
                    subgrupoConta.cd_grupo_conta = null;
                if (subgrupoConta.cd_subgrupo_pai <= 0)
                    subgrupoConta.cd_subgrupo_pai = null;
                var grupo = Business.putSubgrupoConta(subgrupoConta);
                retorno.retorno = SubGrupoSort.parseSubgrupoForSubgrupoUI(grupo);
                if (grupo.cd_subgrupo_conta <= 0)
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
        // Post api/<controller>/5
        [HttpComponentesAuthorize(Roles = "subgr.i")]
        public HttpResponseMessage PostSubgrupoConta(SubgrupoConta subgrupoConta)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });
                if (subgrupoConta.cd_subgrupo_pai != null && subgrupoConta.cd_subgrupo_pai > 0)
                    subgrupoConta.cd_grupo_conta = null;
                if (subgrupoConta.cd_subgrupo_pai <= 0)
                    subgrupoConta.cd_subgrupo_pai = null;
                //throw new Exception();
                var grupo = Business.postSubgrupoConta(subgrupoConta);
                retorno.retorno = SubGrupoSort.parseSubgrupoForSubgrupoUI(grupo);
                if (grupo.cd_subgrupo_conta <= 0)
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
        [HttpComponentesAuthorize(Roles = "subgr.e")]
        public HttpResponseMessage PostDeleteSubgrupoConta(List<SubgrupoConta> SubgrupoConta)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });
                var deleted = Business.deleteAllSubgrupoConta(SubgrupoConta);
                retorno.retorno = deleted;
                if (!deleted)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotDeletedReg, retorno, logger, ex);
            }

        }

        //Método responsável pela emissão do relatório:
        [HttpComponentesAuthorize(Roles = "subgr")]
        public HttpResponseMessage GetUrlRelatorioSubgrupoConta(string sort, int direction, string desc, bool inicio, int cdGrupo, int tipoNivel)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                // Pega os parâmetros do usuário para criar a url do relatório:
                string strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@descricao=" + desc + "&@inicio=" + inicio + "&@cdGrupo=" + cdGrupo + "&@tipo=" + tipoNivel + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=" + "Relatório de Grupo de Contas&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.SubgrupoContaSearch;
                //parametros = HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8);
                string parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);

                return Request.CreateResponse(HttpStatusCode.OK, parametrosCript);
            }
            catch (Exception exe)
            {
                return gerarLogException(Messages.msgErroRelatorio, retorno, logger, exe);
            }
        }

        // GET api/<controller>/1
        [HttpComponentesAuthorize(Roles = "subgr")]
        public HttpResponseMessage GetSubgrupoContaByGrupoConta(int cdGrupoConta)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var grupos = Business.getSubgruposPorCodGrupoContas(cdGrupoConta);
                return Request.CreateResponse(HttpStatusCode.OK, grupos);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        // GET api/<controller>/1
        [HttpComponentesAuthorize(Roles = "subgr")]
        public HttpResponseMessage getSubgrupoContaSearchFK(string descricao, bool inicio, int cdGrupo, int tipoNivel)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                bool contaSegura = this.ComponentesUser.Permissao.Contains("ctsg");
                int cdEscola = this.ComponentesUser.CodEmpresa != null ? this.ComponentesUser.CodEmpresa.Value : 0;
                var grupos = Business.getSubgrupoContaSearchFK(descricao, inicio, cdGrupo, SubgrupoConta.parseTipoNivel(tipoNivel), contaSegura, cdEscola);
                return Request.CreateResponse(HttpStatusCode.OK, grupos);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }



        #endregion

        #region Banco
        [HttpComponentesAuthorize(Roles = "banc")]
        public HttpResponseMessage getAllBanco()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var banco = Business.getAllBanco();
                retorno.retorno = banco;
                return Request.CreateResponse(HttpStatusCode.OK, retorno);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "banc")]
        public HttpResponseMessage getBancosTituloCheque()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cd_escola = (int)this.ComponentesUser.CodEmpresa;
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var bancos = Business.getBancosTituloCheque(cd_escola).ToList();
                retorno.retorno = bancos;
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "banc")]
        public HttpResponseMessage getBancoCarteira()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var banco = Business.getBancoCarteira();
                retorno.retorno = banco;
                return Request.CreateResponse(HttpStatusCode.OK, retorno);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
            }
        }
        [HttpComponentesAuthorize(Roles = "banc")]
        public HttpResponseMessage getBancobyId(int cdBanco)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var banco = Business.getBancobyId(cdBanco);
                retorno.retorno = banco;
                return Request.CreateResponse(HttpStatusCode.OK, retorno);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "banc")]
        public HttpResponseMessage getBancoSearch(string nome, string nmBanco, bool inicio)
        {
            try
            {
                if (nome == null)
                    nome = String.Empty;
                if (nmBanco == null)
                    nmBanco = String.Empty;

                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var retorno = Business.getBancoSearch(parametros, nome, nmBanco, inicio);
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
        [HttpComponentesAuthorize(Roles = "banc")]
        public HttpResponseMessage GetUrlRelatorioBanco(string sort, int direction, string nome, string nmBanco, bool inicio)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                // Pega os parâmetros do usuário para criar a url do relatório:
                string strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@nome=" + nome + "&@nmBanco=" + nmBanco + "&@inicio=" + inicio + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=" + "Relatório de Banco&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.BancoSearch;
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

        // Put api/<controller>/5
        [HttpComponentesAuthorize(Roles = "banc.a")]
        public HttpResponseMessage PostAlterarBanco(Banco banco)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });
                var bancoRet = Business.putBanco(banco);
                retorno.retorno = bancoRet;
                if (bancoRet.cd_banco <= 0)
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
        [HttpComponentesAuthorize(Roles = "banc.i")]
        public HttpResponseMessage PostBanco(Banco banco)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });
                var bancoRet = Business.postBanco(banco);
                retorno.retorno = bancoRet;
                if (bancoRet.cd_banco <= 0)
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
        [HttpComponentesAuthorize(Roles = "banc.e")]
        public HttpResponseMessage PostDeleteBanco(List<Banco> banco)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });
                var deleted = Business.deleteAllBanco(banco);
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

        #endregion

        #endregion

        #region Item
        // GET api/<controller>
        //Busca item  - esse método é usado para populara grid de materiais didáticos no curso
        [HttpComponentesAuthorize(Roles = "item")]
        [HttpComponentesAuthorize(Roles = "gest")]
        //[HttpComponentesAuthorize(Roles = "esc")]
        public HttpResponseMessage getItemSearch(string desc, bool inicio, int status, int? tipoItemInt, int? grupoItem, int categoria, bool escola)
        {
            try
            {
                int? cdEscola = this.ComponentesUser.CodEmpresa;
                if (desc == null)
                    desc = String.Empty;
                var isMaster = (bool)this.ComponentesUser.IdMaster;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                //Verifica permissão "Conta Segura".
                bool contaSegura = this.ComponentesUser.Permissao.Contains("ctsg");
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var retorno = Business.getItemSearch(parametros, desc, inicio, getStatus(status), tipoItemInt, grupoItem, cdEscola, isMaster, false, false, false, categoria, escola, contaSegura);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "item")]
        [HttpComponentesAuthorize(Roles = "gest")]
        public HttpResponseMessage getItemSearchAlunosemAula(string desc, bool inicio, int status, int? tipoItemInt, int? grupoItem, int categoria, bool escola)
        {
            try
            {
                int? cdEscola = this.ComponentesUser.CodEmpresa;
                if (desc == null)
                    desc = String.Empty;
                var isMaster = (bool)this.ComponentesUser.IdMaster;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                //Verifica permissão "Conta Segura".
                bool contaSegura = this.ComponentesUser.Permissao.Contains("ctsg");
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var retorno = Business.getItemSearchAlunosemAula(parametros, desc, inicio, getStatus(status), tipoItemInt, grupoItem, cdEscola, isMaster, false, false, false, categoria, escola, contaSegura);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "kitven")]
        //[HttpComponentesAuthorize(Roles = "gest")]
        //[HttpComponentesAuthorize(Roles = "esc")]
        public HttpResponseMessage getKitSearch(string desc, bool inicio, int status, int? tipoItemInt, int? grupoItem, int categoria, bool escola)
        {
            try
            {
                int? cdEscola = this.ComponentesUser.CodEmpresa;
                if (desc == null)
                    desc = String.Empty;
                var isMaster = (bool)this.ComponentesUser.IdMaster;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                //Verifica permissão "Conta Segura".
                bool contaSegura = this.ComponentesUser.Permissao.Contains("ctsg");
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var retorno = Business.getItemSearch(parametros, desc, inicio, getStatus(status), tipoItemInt, grupoItem, cdEscola, isMaster, false, false, false, categoria, escola, contaSegura);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        // GET api/<controller>
        //Busca item  - esse método é usado para populara grid de materiais didáticos no curso
        

        [HttpComponentesAuthorize(Roles = "bib")]
        [HttpComponentesAuthorize(Roles = "item")]
        [HttpComponentesAuthorize(Roles = "gest")]
        public HttpResponseMessage getItemFKEstoque(string desc, bool inicio, int status, int tipoItemInt, int grupoItem, bool comEstoque)
        {
            try
            {
                if (String.IsNullOrEmpty(desc))
                    desc = String.Empty;
                int? cd_pessoa_escola = this.ComponentesUser.CodEmpresa;

                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var retorno = Business.getItemSearch(parametros, desc, inicio, getStatus(status), tipoItemInt, grupoItem, (int)cd_pessoa_escola, comEstoque, 0);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "item")]
        public HttpResponseMessage getItemCurso(int cdCurso)
        {
            try
            {
                var isMaster = (bool)this.ComponentesUser.IdMaster;
                //var cdEscola = this.ComponentesUser.CodEmpresa;
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var retorno = Business.getItemCurso(cdCurso, null, isMaster);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        //O iserte do item esta sendo feito no MVC - WEB
        [HttpComponentesAuthorize(Roles = "item.i")]
        [HttpComponentesAuthorize(Roles = "esc")]
        [Obsolete]
        public HttpResponseMessage PostItemServico(ItemUI item)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                var itemNovo = item;
                retorno.retorno = itemNovo;
                if (itemNovo.cd_item <= 0)
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
        //Delete item
        [HttpComponentesAuthorize(Roles = "item.e")]
        [HttpComponentesAuthorize(Roles = "gest")]
        [HttpComponentesAuthorize(Roles = "esc")]
        public HttpResponseMessage PostDeleteItemServico(List<Item> itens)
        {
            IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
            IEmpresaBusiness BusinessEmpresa = (IEmpresaBusiness)base.instanciarBusiness<IEmpresaBusiness>();
            configuraBusiness(new List<IGenericBusiness>() { Business });

            ReturnResult retorno = new ReturnResult();
            try
            {
                bool masterGeral = BusinessEmpresa.VerificarMasterGeral(this.ComponentesUser.CodUsuario);
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                var deleted = Business.deleteAllItem(itens, masterGeral, cdEscola);
                retorno.retorno = deleted;
                if (!deleted)
                    retorno.AddMensagem(Messages.msgNotDeletedReg, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (FinanceiroBusinessException ex)
            {
                if (ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_CATEGORIA_GRUPO_ITEM ||
                    ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_ITEM_USADO_OUTRAS_ESC ||
                    ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_ITEM_POSSUI_KARDEX)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotDeletedReg, retorno, logger, ex);
            }
        }

        //Método responsável pela emissão do relatório:
        [HttpComponentesAuthorize(Roles = "item")]
        [Obsolete]
        public HttpResponseMessage GetUrlRelatorioItemServico(string sort, int direction, string desc, bool inicio, int status, int? tipoItemInt, int? grupoItem, int categoria, bool escola)
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
                string parametros = strParametrosSort + "@descricao=" + desc + "&@inicio=" + inicio + "&@status=" + status + "&@tipoItem=" + tipoItemInt + "&@grupoItem=" + grupoItem + "&@cdEscola=" + cdEscola + "&@isMaster=" + masterGeral + "&@categoria=" + categoria + "&@escola=" + escola + "&@contaSegura=" + contaSegura + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=" + "Relatório de Item&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + (int)FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.ItemServicoSearch;
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

        //Método responsável pela emissão do relatório:
        [HttpComponentesAuthorize(Roles = "item")]
        [Obsolete]
        public HttpResponseMessage GetUrlRelatorioKit(string sort, int direction, string desc, bool inicio, int status, int? tipoItemInt, int? grupoItem, int categoria, bool escola)
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
                string parametros = strParametrosSort + "@descricao=" + desc + "&@inicio=" + inicio + "&@status=" + status + "&@tipoItem=" + tipoItemInt + "&@grupoItem=" + grupoItem + "&@cdEscola=" + cdEscola + "&@isMaster=" + masterGeral + "&@categoria=" + categoria + "&@escola=" + escola + "&@contaSegura=" + contaSegura + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=" + "Relatório de Kit de Vendas&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + (int)FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.KitSearch;
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

        [HttpComponentesAuthorize(Roles = "item")]
        [HttpComponentesAuthorize(Roles = "bib")]
        [HttpComponentesAuthorize(Roles = "gest")]
        [HttpGet]
        public HttpResponseMessage getItemEstoqueSearch(string desc, bool inicio, int status, int? tipoItemInt, int? grupoItem, bool cEstoque)
        {
            try
            {
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                bool contaSegura = this.ComponentesUser.Permissao.Contains("ctsg");
                IEmpresaBusiness BusinessEmpresa = (IEmpresaBusiness)base.instanciarBusiness<IEmpresaBusiness>();
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                bool isMasterGeral = BusinessEmpresa.VerificarMasterGeral(this.ComponentesUser.CodUsuario);
                if (desc == null)
                    desc = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IEnumerable<ItemUI> itens = Business.getItemSearch(parametros, desc, inicio, getStatus(status), tipoItemInt, grupoItem, cdEscola, isMasterGeral, true, false, cEstoque, 0, false, contaSegura);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, itens);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "item")]
        [HttpComponentesAuthorize(Roles = "bib")]
        [HttpComponentesAuthorize(Roles = "gest")]
        [HttpGet]
        public HttpResponseMessage getItemEstoqueCadSearch(string desc, bool inicio, int status, int? tipoItemInt, int? grupoItem, bool cEstoque)
        {
            try
            {
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                bool contaSegura = this.ComponentesUser.Permissao.Contains("ctsg");
                IEmpresaBusiness BusinessEmpresa = (IEmpresaBusiness)base.instanciarBusiness<IEmpresaBusiness>();
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                bool isMasterGeral = BusinessEmpresa.VerificarMasterGeral(this.ComponentesUser.CodUsuario);
                if (desc == null)
                    desc = String.Empty;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IEnumerable<ItemUI> itens = Business.getItemSearch(parametros, desc, inicio, getStatus(status), tipoItemInt, grupoItem, cdEscola, isMasterGeral, false, true, cEstoque, 0, false, contaSegura);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, itens);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "gest")]
        [HttpComponentesAuthorize(Roles = "item")]
        [HttpGet]
        public HttpResponseMessage returnDataItem(int cdGrupo, int cdTipoItem, int? tipoMovimento)
        {
            ReturnResult retorno = new ReturnResult();
            ItemUI itemUI = new ItemUI();
            try
            {
                tipoMovimento = tipoMovimento != null ? tipoMovimento : null;
                IEmpresaBusiness BusinessEmpresa = (IEmpresaBusiness)base.instanciarBusiness<IEmpresaBusiness>();
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                bool isMasterGeral = BusinessEmpresa.VerificarMasterGeral(this.ComponentesUser.CodUsuario);
                itemUI.grupos = Business.findAllGrupoAtivo(cdGrupo, isMasterGeral);
                itemUI.tipos = Business.getAllTipoItem(tipoMovimento).ToList();
                retorno.retorno = itemUI;
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "kitven")]
        [HttpGet]
        public HttpResponseMessage returnDataKit(int cdGrupo, int cdTipoItem, int? tipoMovimento)
        {
            ReturnResult retorno = new ReturnResult();
            ItemUI itemUI = new ItemUI();
            try
            {
                tipoMovimento = tipoMovimento != null ? tipoMovimento : null;
                IEmpresaBusiness BusinessEmpresa = (IEmpresaBusiness)base.instanciarBusiness<IEmpresaBusiness>();
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                bool isMasterGeral = BusinessEmpresa.VerificarMasterGeral(this.ComponentesUser.CodUsuario);
                itemUI.grupos = Business.findAllGrupoAtivo(cdGrupo, true);
                itemUI.tipos = Business.getAllTipoItem(tipoMovimento).ToList();
                retorno.retorno = itemUI;
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "item")]
        [HttpComponentesAuthorize(Roles = "gest")]
        [HttpGet]
        public HttpResponseMessage returnDataItemPesq()
        {
            ReturnResult retorno = new ReturnResult();
            ItemUI itemUI = new ItemUI();
            try
            {
                int cd_pessoa_escola = (int)this.ComponentesUser.CodEmpresa;
                IEmpresaBusiness BusinessEmpresa = (IEmpresaBusiness)base.instanciarBusiness<IEmpresaBusiness>();
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                bool isMasterGeral = BusinessEmpresa.VerificarMasterGeral(this.ComponentesUser.CodUsuario);
                itemUI.grupos = Business.findAllGrupoWithItem(cd_pessoa_escola, isMasterGeral);
                itemUI.tipos = Business.getTipoItemMovimentoWithItem(cd_pessoa_escola, isMasterGeral).ToList();
                retorno.retorno = itemUI;
                retorno.retorno = itemUI;
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }
        #endregion

        #region Desconto por Antecipação
        // GET api/<controller>/1
        [HttpComponentesAuthorize(Roles = "poldes")]
        public HttpResponseMessage GetPoliticaDescontoById(int id)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var pDesc = Business.getPoliticaDescontoById(id, cdEscola);
                retorno.retorno = pDesc;
                if (pDesc.cd_politica_desconto <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        // GET api/<controller>/1

        [HttpComponentesAuthorize(Roles = "poldes")]
        public HttpResponseMessage getPoliticaEdit(int id)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                var pDesc = Business.getPoliticaEdit(id, cdEscola);
                retorno.retorno = pDesc;
                if (pDesc.cd_politica_desconto <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);


                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }
        // Put api/<controller>/5
        [HttpComponentesAuthorize(Roles = "poldes.a")]
        public HttpResponseMessage PostAlterarPoliticaDesconto(PoliticaDesconto politicaDesconto)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                politicaDesconto.cd_pessoa_escola = cdEscola;

                //Tratamento do fuso horário da data:
                politicaDesconto.dt_inicial_politica = SGF.Utils.ConversorUTC.Date(politicaDesconto.dt_inicial_politica,
                    this.ComponentesUser.IdFusoHorario, this.ComponentesUser.IsHorarioVerao); 

                var polDesc = Business.postAlterarPoliticaDesconto(politicaDesconto);
                retorno.retorno = polDesc;
                if (polDesc.cd_politica_desconto <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                configureHeaderResponse(response, null);
                return response;
            }
            catch (FinanceiroBusinessException ex)
            {
                if (ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_POLITICA_COM_BAIXA)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }

        }

        // Delte api/<controller>
        [HttpComponentesAuthorize(Roles = "poldes.e")]
        public HttpResponseMessage PostDeletePoliticaDesconto(List<PoliticaDesconto> politicasDesconto)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                var pDesc = Business.deleteAllPoliticaDesconto(politicasDesconto, cdEscola);
                retorno.retorno = pDesc;
                if (pDesc == false)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (FinanceiroBusinessException ex)
            {
                if (ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_POLITICA_COM_BAIXA)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotDeletedReg, retorno, logger, ex);
            }

        }


        [HttpComponentesAuthorize(Roles = "poldes")]
        public HttpResponseMessage GeturlrelatorioPoliticaDesconto(string sort, int direction, int cdTurma, int cdAluno, string dtaIni, string dtaFim, int ativo)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                // Pega os parâmetros do usuário para criar a url do relatório:
                string strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@cdTurma=" + cdTurma + "&@cdAluno=" + cdAluno + "&@dtaIni=" + dtaIni + "&@dtaFim=" + dtaFim + "&@ativo=" + ativo + "&@cdEsc=" + cdEscola + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=Relatório de Desconto por Antecipação&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.PoliticaDescontoSearch;
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

        [HttpComponentesAuthorize(Roles = "poldes")]
        public HttpResponseMessage GetPoliticaDescontoSearch(int cdTurma, int cdAluno, string dtaIni, string dtaFim, int ativo)
        {
            IEnumerable<PoliticaDescontoUI> ret = new List<PoliticaDescontoUI>();
            try
            {
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                DateTime? dtaInicial = null;
                DateTime? dtaFinal = null;
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                
                if (dtaIni != null && dtaIni != "null" && dtaIni != "")
                    dtaInicial = DateTime.Parse(dtaIni);
                if (dtaFim != null && dtaFim != "null" && dtaIni != "")
                    dtaFinal = DateTime.Parse(dtaFim);
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                if (cdEscola != 0)
                    ret = Business.getPoliticaDescontoSearch(parametros, cdTurma, cdAluno, dtaInicial, dtaFinal, getStatus(ativo), cdEscola);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, ret);
                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }
        
        [HttpComponentesAuthorize(Roles = "poldes")]
        public HttpResponseMessage getCriarDataPoliticaContrato(int cdTurma, int cdAluno, string dataVencto)
        {
            int ret = 0;
            try
            {
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());

                DateTime dtaPol;
                if (DateTime.TryParse(dataVencto, out dtaPol))
                    dtaPol = DateTime.Parse(dataVencto);
                else
                    throw new FinanceiroBusinessException(Messages.msgErroDtInvalida, null, FinanceiroBusinessException.TipoErro.ERRO_DATA_INVALIDA, false);

                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                if (cdEscola != 0)
                    ret = Business.getCriarDataPoliticaContrato(cdTurma, cdAluno, dtaPol, cdEscola);

                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, ret);
                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (FinanceiroBusinessException ex) 
            {
                if(ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_DATA_INVALIDA)
                    return gerarLogException(Messages.msgErroDtInvalida, new ReturnResult(), logger, ex);

                return gerarLogException(Messages.msgNotUpReg, new ReturnResult(), logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }
        
        #endregion

        #region Tipo Item

        [HttpComponentesAuthorize(Roles = "item")]
        public HttpResponseMessage GetAllTipoItem(int? tipoMovimento)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var listTipoItem = Business.getAllTipoItem(tipoMovimento);
                retorno.retorno = listTipoItem;
                if (listTipoItem.Count() <= 0)
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

        #region Tabela de Preço
        //Busca as sugestões de dia para opções de pagamento da matrícula, conforme o parâmetro da escola:
        [HttpComponentesAuthorize(Roles = "mat")]
        public HttpResponseMessage getSugestaoNroParcelaOpcoesPgto(string str_data_matricula, int? cd_curso, int? cd_duracao, int? cd_regime)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cd_escola = ComponentesUser.CodEmpresa.Value;
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                DateTime data_matricula = DateTime.Parse(str_data_matricula, new CultureInfo("pt-br", false));
                if (cd_curso.HasValue && cd_regime.HasValue && cd_duracao.HasValue)
                    retorno.retorno = Business.getNroParcelas(cd_escola, cd_curso.Value, cd_regime.Value, cd_duracao.Value, data_matricula);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "tprec.e")]
        public HttpResponseMessage PostDeleteTabelaPreco(List<TabelaPreco> tabelas)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });

                var tabela = Business.deleteAllTabelaPreco(tabelas, (int)this.ComponentesUser.CodEmpresa);
                retorno.retorno = tabela;
                if (tabela == false)
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

        [HttpComponentesAuthorize(Roles = "mat")]
        [HttpComponentesAuthorize(Roles = "tprec")]
        [HttpGet]
        public HttpResponseMessage getValoresForMatricula(string dta_matricula, int cd_curso, int cd_duracao, int cd_regime)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                DateTime data_matricula = DateTime.Parse(dta_matricula, new CultureInfo("pt-br", false));
                Valores valores = Business.getValoresForMatricula(cdEscola, cd_curso, cd_duracao, cd_regime, data_matricula);
                retorno.retorno = valores;
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }
        [HttpComponentesAuthorize(Roles = "tprec")]
        public HttpResponseMessage GeturlrelatorioTabelaPreco(string sort, int direction, int cdCurso, int cdDuracao, int cdRegime, String dtaCad, int codProduto)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                string strParametrosSort = "@orientation=LANDSCAPE&@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@cdCurso=" + cdCurso + "&@cdDuracao=" + cdDuracao + "&@cdRegime=" + cdRegime + "&@dtaCad=" + dtaCad + "&@cdEscola=" + cdEscola + "&@codProduto=" + codProduto + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=Relatório de Tabela de Preço (Curso) &" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.TabelaCursoSearch;
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
        [HttpComponentesAuthorize(Roles = "tprec")]
        [HttpComponentesAuthorize(Roles = "cur")]
        [HttpComponentesAuthorize(Roles = "reg")]
        [HttpComponentesAuthorize(Roles = "prod")]
        [HttpComponentesAuthorize(Roles = "dur")]
        public HttpResponseMessage GetTabelaPrecoSearch(int cdCurso, int cdDuracao, int cdRegime, String dtaCad, int codProduto)
        {
            IEnumerable<TabelaPrecoUI> ret = new List<TabelaPrecoUI>();
            try
            {
                DateTime? dtaCadastro = null;
                if (dtaCad != null && dtaCad != "null" && dtaCad != "")
                    dtaCadastro = DateTime.Parse(dtaCad);
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ret = Business.GetTabelaPrecoSearch(parametros, cdCurso, cdDuracao, cdRegime, dtaCadastro, cdEscola, codProduto);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, ret);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }
        [HttpComponentesAuthorize(Roles = "tprec")]
        public HttpResponseMessage GetHistoricoTabelaPreco(int cdCurso, int cdDuracao, int cdRegime)
        {
            IEnumerable<TabelaPrecoUI> ret = new List<TabelaPrecoUI>();
            try
            {
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                ret = Business.GetHistoricoTabelaPreco(parametros, cdCurso, cdDuracao, cdRegime, cdEscola);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, ret);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }
        #endregion

        #region Plano Conta

        [HttpComponentesAuthorize(Roles = "plct")]
        public HttpResponseMessage getPlanoContasTreeSearch(string descricao, bool inicio)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IEmpresaBusiness BusinessEmpresa = (IEmpresaBusiness)base.instanciarBusiness<IEmpresaBusiness>();
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                bool? isMasterGeral = BusinessEmpresa.VerificarMasterGeral(this.ComponentesUser.CodUsuario);

                int cd_escola = ComponentesUser.CodEmpresa.Value;
                bool isContaSegura = this.ComponentesUser.Permissao.Contains("ctsg");

                retorno.retorno = Business.getPlanoContasTreeSearch(cd_escola, true, isContaSegura, descricao, inicio);

                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "plct")]
        [HttpComponentesAuthorize(Roles = "subgr")]
        [HttpComponentesAuthorize(Roles = "gruco")]
        public HttpResponseMessage getPlanoContaSearch()
        {
            try
            {
                int idEmpresa = this.ComponentesUser.CodEmpresa.Value;
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                List<PlanoConta> planoContas = new List<PlanoConta>();
                planoContas = Business.getPlanoContasSearch(idEmpresa).ToList();

                var retorno = planoContas;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "plct")]
        [HttpComponentesAuthorize(Roles = "subgr")]
        [HttpComponentesAuthorize(Roles = "gruco")]
        [HttpGet]
        public PlanoContaUI getValuesForSearchDropDown(byte? nivelGrupoContas)
        {
            try
            {
                nivelGrupoContas = nivelGrupoContas ?? 0;
                int idEmpresa = this.ComponentesUser.CodEmpresa.Value;
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                bool hasGrupoContaDisp = Business.getGrupoContasWhitOutPlanoContas((byte)nivelGrupoContas, idEmpresa);
                PlanoContaUI planoContaUI = new PlanoContaUI();
                planoContaUI.nivel_plano_conta = nivelGrupoContas;
                planoContaUI.gruposContas = Business.getGrupoContasWithPlanoContas(idEmpresa).ToList();
                planoContaUI.hasGrupoSubGrupoDisponivel = verifyHasGrupoSubGrupo(nivelGrupoContas, idEmpresa);
                // se exitir grupo e subgrupo sem plano de contas o hasGrupoSubGrupo o  contas.marcado será true.
                return planoContaUI;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
        }

        private bool verifyHasGrupoSubGrupo(byte? nivelGrupoContas, int idEmpresa)
        {
            IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
            bool hasGrupoContaDisp = Business.getGrupoContasWhitOutPlanoContas((byte)nivelGrupoContas, idEmpresa);
            bool isContaSegura = this.ComponentesUser.Permissao.Contains("ctsg");

            var existsPlanoConta = Business.getPlanoContasTreeSearch(idEmpresa, false, isContaSegura, "", false).Count() > 0;
            if (!existsPlanoConta)
                return hasGrupoContaDisp;
            else
                return false;
        }


        [HttpComponentesAuthorize(Roles = "plct")]
        [HttpComponentesAuthorize(Roles = "subgr")]
        [HttpComponentesAuthorize(Roles = "gruco")]
        public HttpResponseMessage getGrupoContaArvore(int cd_grupo_conta, string no_subgrupo_conta, bool inicio, int nivel, int tipoPlanoConta, bool marcar)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                bool isContaSegura = this.ComponentesUser.Permissao.Contains("ctsg");

                if (String.IsNullOrEmpty(no_subgrupo_conta)) no_subgrupo_conta = "";
                int idEmpresa = this.ComponentesUser.CodEmpresa.Value;
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                List<List<Contas>> matriz_retorno = new List<List<Contas>>();
                List<Contas> contas = new List<Contas>();
                List<GrupoConta> grupoDisponiveis = Business.getListaContas(cd_grupo_conta, no_subgrupo_conta, inicio, nivel, tipoPlanoConta, idEmpresa).ToList();

                matriz_retorno.Add(new List<Contas>());
                matriz_retorno.Add(new List<Contas>());
                MontaGrupoContas(grupoDisponiveis, matriz_retorno[0], marcar);

                List<GrupoConta> grupoEmpresa = Business.getPlanoContasTreeSearch(idEmpresa, false, isContaSegura, "", false).ToList();

                MontaGrupoContas(grupoEmpresa, matriz_retorno[1], false);
                retorno.retorno = matriz_retorno;
                return Request.CreateResponse(HttpStatusCode.OK, retorno);
            }
            catch (Exception exe)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, exe);
            }
        }

        private Contas MontaGrupoContas(ICollection<GrupoConta> grupoContas, ICollection<Contas> retorno, bool hasMarcado)
        {
            Contas contas = new Contas();
            List<GrupoConta> gruposContas = grupoContas.ToList();
            for (int i = 0; i < gruposContas.Count; i++)
            {
                List<SubgrupoConta> subGrupos = gruposContas[i].SubGrupos.ToList();
                contas = new Contas();
                contas.id = gruposContas[i].cd_grupo_conta;
                contas.pai = (int)Contas.Hierarquia.PAI;
                contas.dc_conta = gruposContas[i].no_grupo_conta;
                contas.cd_conta = gruposContas[i].cd_grupo_conta;
                contas.marcado = hasMarcado;
                contas.cd_grupo_conta = gruposContas[i].cd_grupo_conta;
                contas.cd_subgrupo_pai = 0;
                contas.cd_subgrupo_conta = 0;
                contas.id_tipo_conta = null;
                contas.id_ativo = true;
                contas.id_conta_segura = false;
                contas.cd_plano_conta = 0;
                if (gruposContas[i].SubGrupos.Count() > 0)
                    MontaSubGrupo(gruposContas[i].SubGrupos, contas.children, hasMarcado);
                retorno.Add(contas);
            }
            return contas;
        }

        private void MontaSubGrupo(ICollection<SubgrupoConta> subGrupos, ICollection<Contas> retorno, bool hasMarcado)
        {
            Contas contas = new Contas();
            List<SubgrupoConta> subGrupoContas = subGrupos.ToList();
            for (int u = 0; u < subGrupoContas.Count; u++)
            {
                if (subGrupoContas[u].cd_subgrupo_pai == null)
                {
                    List<SubgrupoConta> subGruposFilhos = subGrupoContas[u].SubgruposFilhos.ToList();
                    contas = new Contas();
                    contas.id = subGrupoContas[u].cd_subgrupo_conta;
                    contas.pai = (int)Contas.Hierarquia.FILHO;
                    contas.dc_conta = subGrupoContas[u].no_subgrupo_conta;
                    contas.cd_conta = subGrupoContas[u].cd_subgrupo_conta;
                    contas.marcado = hasMarcado;
                    contas.cd_grupo_conta = subGrupoContas[u].cd_grupo_conta;
                    contas.cd_subgrupo_pai = subGrupoContas[u].cd_subgrupo_pai;
                    contas.cd_subgrupo_conta = subGrupoContas[u].cd_subgrupo_conta;
                    contas.id_tipo_conta = subGrupoContas[u].SubgrupoPlanoConta.Where(pl => pl.cd_subgrupo_conta == subGrupoContas[u].cd_subgrupo_conta).Select(pl => pl.id_tipo_conta).FirstOrDefault() ?? 0;
                    contas.id_ativo = subGrupoContas[u].SubgrupoPlanoConta.Where(pl => pl.cd_subgrupo_conta == subGrupoContas[u].cd_subgrupo_conta).Select(pl => pl.id_ativo).FirstOrDefault();
                    contas.id_conta_segura = subGrupoContas[u].SubgrupoPlanoConta.Where(pl => pl.cd_subgrupo_conta == subGrupoContas[u].cd_subgrupo_conta).Select(pl => pl.id_conta_segura).FirstOrDefault();
                    contas.cd_plano_conta = subGrupoContas[u].SubgrupoPlanoConta.Where(pl => pl.cd_subgrupo_conta == subGrupoContas[u].cd_subgrupo_conta).Select(pl => pl.cd_plano_conta).FirstOrDefault();
                    if (subGrupoContas[u].SubgruposFilhos.Count() > 0)
                        MontaSubGrupoFilhos(subGruposFilhos, contas.children, hasMarcado);
                    retorno.Add(contas);
                }
            }
        }

        private void MontaSubGrupoFilhos(ICollection<SubgrupoConta> subGruposFilhos, ICollection<Contas> retorno, bool hasMarcado)
        {
            Contas contas = new Contas();
            List<SubgrupoConta> subGrupoContas = subGruposFilhos.ToList();
            for (int v = 0; v < subGrupoContas.Count; v++)
            {
                contas = new Contas();
                contas.id = subGrupoContas[v].cd_subgrupo_conta;
                contas.pai = (int)Contas.Hierarquia.FILHO;
                contas.dc_conta = subGrupoContas[v].no_subgrupo_conta;
                contas.cd_conta = subGrupoContas[v].cd_subgrupo_conta;
                contas.cd_grupo_conta = subGrupoContas[v].cd_grupo_conta;
                contas.cd_subgrupo_pai = subGrupoContas[v].cd_subgrupo_pai;
                contas.cd_subgrupo_conta = subGrupoContas[v].cd_subgrupo_conta;
                contas.marcado = hasMarcado;
                contas.id_tipo_conta = subGrupoContas[v].SubgrupoPlanoConta.Where(pl => pl.cd_subgrupo_conta == subGrupoContas[v].cd_subgrupo_conta).Select(pl => pl.id_tipo_conta).FirstOrDefault() ?? 0;
                contas.id_ativo = subGrupoContas[v].SubgrupoPlanoConta.Where(pl => pl.cd_subgrupo_conta == subGrupoContas[v].cd_subgrupo_conta).Select(pl => pl.id_ativo).FirstOrDefault();
                contas.id_conta_segura = subGrupoContas[v].SubgrupoPlanoConta.Where(pl => pl.cd_subgrupo_conta == subGrupoContas[v].cd_subgrupo_conta).Select(pl => pl.id_conta_segura).FirstOrDefault();
                contas.cd_plano_conta = subGrupoContas[v].SubgrupoPlanoConta.Where(pl => pl.cd_subgrupo_conta == subGrupoContas[v].cd_subgrupo_conta).Select(pl => pl.cd_plano_conta).FirstOrDefault();
                retorno.Add(contas);
            }
        }

        [HttpComponentesAuthorize(Roles = "plct.a")]
        [HttpComponentesAuthorize(Roles = "plct.e")]
        [HttpComponentesAuthorize(Roles = "plct.i")]
        public HttpResponseMessage postPlanoConta(PlanoContaUI planoContaUI)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int idEmpresa = this.ComponentesUser.CodEmpresa.Value;
                int tipoPlano = planoContaUI.tipoPlano;
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();

                bool was_persisted;
                bool was_deleted;
                bool isContaSegura = this.ComponentesUser.Permissao.Contains("ctsg");

                ICollection<PlanoConta> planoContas = planoContaUI.planosContasEmpresa;
                ICollection<PlanoConta> planoDisponiveis = planoContaUI.planosContasDisponiveis;

                PlanoContaUI listPlanos = new PlanoContaUI();

                if (planoContas != null)
                {
                    listPlanos = Business.addPlanoConta(planoContas, (int)PlanoContasDataAccess.TipoPlanoContaConsulta.MEU_PLANO_CONTA, idEmpresa, (byte)planoContaUI.nivel_plano_conta, out was_persisted, isContaSegura);
                    was_add = was_persisted;
                }
                if (planoDisponiveis != null)
                {
                    listPlanos = Business.excluirPlanoConta(planoDisponiveis, (int)PlanoContasDataAccess.TipoPlanoContaConsulta.DISPONIVEIS, idEmpresa, (int)planoContaUI.nivel_plano_conta, out was_deleted);
                    was_del = was_deleted;
                }
                retorno.retorno = listPlanos;
                if ((planoContas != null && planoContas.Count() <= 0) || (planoDisponiveis != null && planoDisponiveis.Count() <= 0) || (!was_add && !was_del))
                    retorno.AddMensagem(Messages.msgRegNotPersist, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateAllSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, retorno);
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }

        }

        [HttpComponentesAuthorize(Roles = "plct.a")]
        [HttpComponentesAuthorize(Roles = "plct.e")]
        [HttpComponentesAuthorize(Roles = "plct.i")]
        public HttpResponseMessage postCancelarPlanoContas(byte nivel)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int idEmpresa = this.ComponentesUser.CodEmpresa.Value;
                bool isContaSegura = this.ComponentesUser.Permissao.Contains("ctsg");
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();

                List<List<Contas>> matriz_retorno = new List<List<Contas>>();
                List<Contas> contas = new List<Contas>();
                List<GrupoConta> grupoDisponiveis = Business.getListaContas(0, "", false, nivel, (int)PlanoContasDataAccess.TipoPlanoContaConsulta.DISPONIVEIS, idEmpresa).ToList();

                matriz_retorno.Add(new List<Contas>());
                matriz_retorno.Add(new List<Contas>());
                MontaGrupoContas(grupoDisponiveis, matriz_retorno[0], false);

                List<GrupoConta> grupoEmpresa = Business.getPlanoContasTreeSearch(idEmpresa, false, isContaSegura, "", false).ToList();

                MontaGrupoContas(grupoEmpresa, matriz_retorno[1], false);
                retorno.retorno = matriz_retorno;
                retorno.AddMensagem(Messages.msgUpdateAllSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

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

        #region Cheque
        [HttpComponentesAuthorize(Roles = "mat")]
        public HttpResponseMessage getChequeByContrato(int id)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {

                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var cheque = Business.getChequeByContratoPesq(id);
                retorno.retorno = cheque;
                return Request.CreateResponse(HttpStatusCode.OK, retorno);
            }

            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
            }
        }

        #endregion

        #region Título

        [HttpComponentesAuthorize(Roles = "mat")]
        [HttpComponentesAuthorize(Roles = "tit")]
        [HttpGet]
        public HttpResponseMessage getTitulosByContrato(int cdContrato)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                List<Titulo> titulos = Business.getTitulosByContratoLeitura(cdContrato, cdEscola);
                retorno.retorno = titulos.OrderBy(x => x.cd_tipo_titulo).ThenBy(y => y.nm_parcela_titulo );
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "mat")]
        [HttpComponentesAuthorize(Roles = "tit")]
        [HttpGet]
        public HttpResponseMessage getLocalMovtoGeralOuCartao(int cd_tipo_financeiro)
        {
            ReturnResult retorno = new ReturnResult();
            List<LocalMovto> locaisMovto = new List<LocalMovto>();
            List<LocalMovto> locaisMovtoConsulta = new List<LocalMovto>();
            try
            {
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                if (cd_tipo_financeiro < 0)
                {
                    cd_tipo_financeiro = Math.Abs(cd_tipo_financeiro);
                    cdEscola = -1 * cdEscola;
                }
                IFinanceiroBusiness BusinessFinanceiro = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                if (cd_tipo_financeiro == (int)FundacaoFisk.SGF.GenericModel.TipoFinanceiro.TiposFinanceiro.CARTAO)
                {
                    locaisMovto = BusinessFinanceiro.getAllLocalMovtoCartao(cdEscola).ToList();
                    retorno.retorno = locaisMovto.Distinct().OrderBy(x => x.no_local_movto);
                }
                else
                {
                    bool isMaster = (bool)this.ComponentesUser.IdMaster;
                    int cd_pessoa_usuario = isMaster ? 0 : this.ComponentesUser.CodPessoaUsuario;
                    //if (cd_tipo_financeiro == (int)FundacaoFisk.SGF.GenericModel.TipoFinanceiro.TiposFinanceiro.BOLETO)
                        locaisMovto = BusinessFinanceiro.getLocalMovimentoSomenteLeitura(cdEscola, 0, LocalMovtoDataAccess.TipoConsultaLocalMovto.HAS_ATIVO_E_BANCOS_E_CARTEIRAS, cd_pessoa_usuario).ToList();
                    //else
                    //    //locaisMovto = BusinessFinanceiro.getLocalMovtoByEscola(cdEscola, 0, true).ToList();
                    //    locaisMovto = BusinessFinanceiro.getLocalMovimentoSomenteLeitura(cdEscola, 0, LocalMovtoDataAccess.TipoConsultaLocalMovto.HAS_ATIVO_ESCOLA_SEM_CARTEIRA, cd_pessoa_usuario).ToList();
                    retorno.retorno = locaisMovto.Distinct().OrderBy(x => x.no_local_movto); 
                }
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "mat")]
        [HttpComponentesAuthorize(Roles = "tit")]
        [HttpGet]
        public HttpResponseMessage getLocalMovtoGeralOuTipoLiquidacaoCartao(int cd_tipo_liquidacao, int cd_local_movto)
        {
            ReturnResult retorno = new ReturnResult();
            List<LocalMovto> locaisMovto = new List<LocalMovto>();
            //List<LocalMovto> locaisMovtoConsulta = new List<LocalMovto>();
            try
            {
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                bool isMaster = (bool)this.ComponentesUser.IdMaster;
                int cd_pessoa_usuario = isMaster ? 0 : this.ComponentesUser.CodPessoaUsuario;
                IFinanceiroBusiness BusinessFinanceiro = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                if (cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.CARTAO_CREDITO || cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.CARTAO_DEBITO)
                {
                    locaisMovto = BusinessFinanceiro.getAllLocalMovtoTipoCartao(cdEscola, cd_tipo_liquidacao, cd_local_movto, cd_pessoa_usuario).ToList();
                    retorno.retorno = locaisMovto.Distinct().OrderBy(x => x.no_local_movto);
                }
                else
                {
                    locaisMovto = BusinessFinanceiro.getLocalMovtoByEscola(cdEscola, 0, true).ToList();
                    retorno.retorno = locaisMovto.Distinct().OrderBy(x => x.no_local_movto);
                }
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "mat")]
        [HttpComponentesAuthorize(Roles = "tit")]
        [HttpGet]
        public HttpResponseMessage getAllLocalMovtoGeralOuCartao(int cd_tipo_financeiro)
        {
            ReturnResult retorno = new ReturnResult();
            List<LocalMovto> locaisMovto = new List<LocalMovto>();
            try
            {
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                IFinanceiroBusiness BusinessFinanceiro = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                
                    locaisMovto = BusinessFinanceiro.getAllLocalMovtoByEscola(cdEscola, 0).ToList();
                
                retorno.retorno = locaisMovto;
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "mat")]
        [HttpComponentesAuthorize(Roles = "tit")]
        [HttpGet]
        public HttpResponseMessage getAllLocalMovtoTipoBanco()
        {
            ReturnResult retorno = new ReturnResult();
            List<LocalMovto> locaisMovto = new List<LocalMovto>();
            try
            {
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                IFinanceiroBusiness BusinessFinanceiro = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();

                locaisMovto = BusinessFinanceiro.getAllLocalMovtoBanco(cdEscola).ToList();
                retorno.retorno = locaisMovto;
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "mat")]
        [HttpComponentesAuthorize(Roles = "tit")]
        [HttpGet]
        public HttpResponseMessage getLocalMovtoBaixaAut()
        {
            ReturnResult retorno = new ReturnResult();
            List<LocalMovto> locaisMovto = new List<LocalMovto>();
            try
            {
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                IFinanceiroBusiness BusinessFinanceiro = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();

                var locaisMovtoConsulta = BusinessFinanceiro.getAllLocalMovtoCartao(cdEscola).ToList();

               

                retorno.retorno = locaisMovtoConsulta.Distinct().OrderBy(x => x.no_local_movto);
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "mat")]
        [HttpComponentesAuthorize(Roles = "tit")]
        [HttpGet]
        public HttpResponseMessage getLocaisMovtoBaixaAutById(int cd_local_banco)
        {
            ReturnResult retorno = new ReturnResult();
            List<LocalMovto> locaisMovto = new List<LocalMovto>();
            try
            {
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                IFinanceiroBusiness BusinessFinanceiro = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();

                var locaisMovtoCartao = BusinessFinanceiro.getAllLocalMovtoCartaoComPai(cdEscola)
                    .Where(t => t.cd_local_banco == cd_local_banco &&
                        (t.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_CREDITO || 
                        t.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_DEBITO));

                retorno.retorno = locaisMovtoCartao;
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "mat")]
        [HttpComponentesAuthorize(Roles = "tit")]
        [HttpGet]
        public HttpResponseMessage getTitulosByRenegociacao(int cdContrato, int cdAluno, int cdProduto)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                List<Titulo> titulos = Business.getTitulosByRenegociacao(cdContrato, cdAluno, cdEscola, cdProduto);
                retorno.retorno = titulos;
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }


        //cd_pessoa=0&responsavel=false&inicio=false&locMov=0&natureza=1&status=1&numeroTitulo=&parcelaTitulo=&valorTitulo=&dtInicial=&dtFinal=&emissao=true&baixa=false&vencimento=false&locMovBaixa=0
        //&cdTipoLiquidacao=0&tipoTitulo=0&nossoNumero=&cnabStatus=-1&nro_recibo=&cd_turma=&cd_situacoes_aluno=
        [HttpGet]
        [HttpComponentesAuthorize(Roles = "tit")]
        public HttpResponseMessage getTituloSearch(int cd_pessoa, bool responsavel, bool inicio, int locMov, int natureza, int status, int? numeroTitulo,
                                       int? parcelaTitulo, string valorTitulo, string dtInicial, string dtFinal, bool emissao, bool vencimento, bool baixa,
                                       int locMovBaixa, int cdTipoLiquidacao, byte tipoTitulo, string nossoNumero, int cnabStatus, int? nro_recibo, int? cd_turma,
                                       string cd_situacoes_aluno, int? cd_tipo_financeiro)
        {
            try
            {
                DateTime? dtaInicial = dtInicial == null ? null : (DateTime?)Convert.ToDateTime(dtInicial);
                DateTime? dtaFinal = dtFinal == null ? null : (DateTime?)Convert.ToDateTime(dtFinal);

                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                List<int> cd_situacoes = !string.IsNullOrEmpty(cd_situacoes_aluno) ? cd_situacoes_aluno.Split(',').Select(x => int.Parse(x)).ToList() : new List<int>();
                //Usuário tem não tem permissão de conta segura
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var retorno = Business.searchTitulo(parametros, cdEscola, cd_pessoa, responsavel, inicio, locMov, natureza, status, numeroTitulo, parcelaTitulo, valorTitulo, dtaInicial, dtaFinal,
                    emissao, vencimento, baixa, locMovBaixa, cdTipoLiquidacao, false, tipoTitulo, nossoNumero, cnabStatus, nro_recibo, cd_turma, cd_situacoes, cd_tipo_financeiro);
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
        [HttpComponentesAuthorize(Roles = "ctsg")]
        [HttpComponentesAuthorize(Roles = "tit")]
        public HttpResponseMessage getTituloSearchGeral(int cd_pessoa, bool responsavel, bool inicio, int locMov, int natureza, int status, int? numeroTitulo,
                                       int? parcelaTitulo, string valorTitulo, string dtInicial, string dtFinal, bool emissao, bool vencimento, bool baixa,
                                       int locMovBaixa, int cdTipoLiquidacao, byte tipoTitulo, string nossoNumero, int cnabStatus, int? nro_recibo,
                                       int? cd_turma, string cd_situacoes_aluno, int? cd_tipo_financeiro)
        {
            try
            {
                DateTime? dtaInicial = dtInicial == null ? null : (DateTime?)Convert.ToDateTime(dtInicial);
                DateTime? dtaFinal = dtFinal == null ? null : (DateTime?)Convert.ToDateTime(dtFinal);
                List<int> cd_situacoes = !string.IsNullOrEmpty(cd_situacoes_aluno) ? cd_situacoes_aluno.Split(',').Select(x => int.Parse(x)).ToList() : new List<int>();
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                //Usuário tem permissão de conta segura
                var retorno = Business.searchTitulo(parametros, cdEscola, cd_pessoa, responsavel, inicio, locMov, natureza, status, numeroTitulo, parcelaTitulo,
                                                    valorTitulo, dtaInicial, dtaFinal, emissao, vencimento, baixa, locMovBaixa, cdTipoLiquidacao, true, tipoTitulo, nossoNumero, cnabStatus,
                                                    nro_recibo, cd_turma, cd_situacoes, cd_tipo_financeiro);
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
        [HttpComponentesAuthorize(Roles = "tit")]
        [HttpComponentesAuthorize(Roles = "fatu")]
        public HttpResponseMessage getTituloSearchFaturamento(int cd_pessoa, bool responsavel, bool inicio, int? numeroTitulo, int? parcelaTitulo, string valorTitulo, string dtInicial,
                                                              string dtFinal, byte tipoTitulo)
        {
            try
            {
                DateTime? dtaInicial = dtInicial == null ? null : (DateTime?)Convert.ToDateTime(dtInicial);
                DateTime? dtaFinal = dtFinal == null ? null : (DateTime?)Convert.ToDateTime(dtFinal);

                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                //Usuário tem não tem permissão de conta segura
                var retorno = Business.searchTituloFaturamento(parametros, cdEscola, cd_pessoa, responsavel, inicio, numeroTitulo, parcelaTitulo, valorTitulo, dtaInicial, dtaFinal, false, tipoTitulo);
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
        [HttpComponentesAuthorize(Roles = "ctsg")]
        [HttpComponentesAuthorize(Roles = "tit")]
        [HttpComponentesAuthorize(Roles = "fatu")]
        public HttpResponseMessage getTituloSearchFaturamentoGeral(int cd_pessoa, bool responsavel, bool inicio, int? numeroTitulo, int? parcelaTitulo, string valorTitulo, string dtInicial,
                                                                    string dtFinal, byte tipoTitulo)
        {
            try
            {
                DateTime? dtaInicial = dtInicial == null ? null : (DateTime?)Convert.ToDateTime(dtInicial);
                DateTime? dtaFinal = dtFinal == null ? null : (DateTime?)Convert.ToDateTime(dtFinal);

                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                //Usuário tem permissão de conta segura
                var retorno = Business.searchTituloFaturamento(parametros, cdEscola, cd_pessoa, responsavel, inicio, numeroTitulo, parcelaTitulo, valorTitulo, dtaInicial, dtaFinal, true, tipoTitulo);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }
        [HttpComponentesAuthorize(Roles = "tit")]
        public HttpResponseMessage postTituloSearchGeralTCnab(TituloUI titulo)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                titulo.cd_pessoa_empresa = cdEscola;
                titulo.natureza = (int)Titulo.NaturezaTitulo.RECEBER;
                //titulo.status = (int)Titulo.StatusTitulo.ABERTO;
                titulo.emissao = false;
                titulo.vencimento = true;
                if (titulo.locaisEscolhidos.Count() <= 0)
                    titulo.todosLocais = true;
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var titulos = Business.searchTituloCnab(titulo);
                retorno.retorno = titulos;
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
            }
        }


        [HttpComponentesAuthorize(Roles = "tit")]
        public HttpResponseMessage postPesquisaTituloCnab(TituloUI titulo)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var titulos = Business.searchTituloCnabGrade(titulo);
                retorno.retorno = titulos;
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "tit")]
        [HttpGet]
        public HttpResponseMessage getTituloBaixaFinan(int cd_titulo)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                bool isMaster = (bool)this.ComponentesUser.IdMaster;
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();

                int cod_pessoa_usuario = isMaster ? 0 : this.ComponentesUser.CodPessoaUsuario;

                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                Titulo titulo = Business.getTituloBaixaFinan(cd_titulo, cdEscola, TituloDataAccess.TipoConsultaTituloEnum.HAS_EDIT_TITULO_VIEW);
                if ((titulo.id_status_titulo == (int)Titulo.StatusTitulo.FECHADO) || (titulo.vl_titulo != titulo.vl_saldo_titulo))
                    titulo.bancos = Business.getLocalMovimentoSomenteLeitura(cdEscola, titulo.cd_local_movto, LocalMovtoDataAccess.TipoConsultaLocalMovto.HAS_ATIVO_ESCOLA_SEM_CARTEIRA, cod_pessoa_usuario).ToList();
                else
                {
                    titulo.bancos = Business.getLocalMovimentoSomenteLeitura(cdEscola, 0, LocalMovtoDataAccess.TipoConsultaLocalMovto.HAS_ATIVO_E_BANCOS, cod_pessoa_usuario).ToList();
                    titulo.tipoDocumentos = Business.getTipoFinanceiroAtivo();
                }
                retorno.retorno = titulo;
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "tit")]
        [HttpGet]
        public HttpResponseMessage getTituloBaixaFinanComFiltrosTrocaFinanceira(int cd_titulo)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                bool isMaster = (bool)this.ComponentesUser.IdMaster;
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();

                int cod_pessoa_usuario = isMaster ? 0 : this.ComponentesUser.CodPessoaUsuario;

                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                Titulo titulo = Business.getTituloBaixaFinan(cd_titulo, cdEscola, TituloDataAccess.TipoConsultaTituloEnum.HAS_EDIT_TITULO_VIEW);
                if (titulo.cd_tipo_financeiro == (int)TipoFinanceiro.TiposFinanceiro.CARTAO) //Cartão
                {
                    if ((titulo.id_status_titulo == (int)Titulo.StatusTitulo.FECHADO) || (titulo.vl_titulo != titulo.vl_saldo_titulo))
                        titulo.bancos = Business.getLocalMovimentoSomenteLeituraComFiltrosTrocaFinanceira(cdEscola, titulo.cd_local_movto, titulo.cd_tipo_financeiro, LocalMovtoDataAccess.TipoConsultaLocalMovto.HAS_ATIVO_ESCOLA_SEM_CARTEIRA, cod_pessoa_usuario).ToList();
                    else
                    {
                        titulo.bancos = Business.getLocalMovimentoSomenteLeituraComFiltrosTrocaFinanceira(cdEscola, 0, titulo.cd_tipo_financeiro, LocalMovtoDataAccess.TipoConsultaLocalMovto.HAS_ATIVO_E_BANCOS, cod_pessoa_usuario).ToList();
                        titulo.tipoDocumentos = Business.getTipoFinanceiroAtivo();
                    }
                }
                else
                {
                    if ((titulo.id_status_titulo == (int)Titulo.StatusTitulo.FECHADO) || (titulo.vl_titulo != titulo.vl_saldo_titulo))
                        titulo.bancos = Business.getLocalMovimentoSomenteLeitura(cdEscola, titulo.cd_local_movto, LocalMovtoDataAccess.TipoConsultaLocalMovto.HAS_ATIVO_ESCOLA_SEM_CARTEIRA, cod_pessoa_usuario).ToList();
                    else
                    {
                        titulo.bancos = Business.getLocalMovimentoSomenteLeitura(cdEscola, -titulo.cd_local_movto, LocalMovtoDataAccess.TipoConsultaLocalMovto.HAS_ATIVO_E_BANCOS_NAO_CARTAO, cod_pessoa_usuario).ToList();
                        titulo.tipoDocumentos = Business.getTipoFinanceiroAtivo();
                    }
                }
                retorno.retorno = titulo;
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "tit.a")]
        public HttpResponseMessage postUpdateTituloBaixaFinan(Titulo titulo)
        {
            ReturnResult retorno = new ReturnResult();
            //throw new Exception("sdfgasdfa");
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });
                titulo.cd_pessoa_empresa = (int)this.ComponentesUser.CodEmpresa;
                titulo.dt_vcto_titulo = titulo.dt_vcto_titulo.Date;
                var tituloCad = Business.editTitulosBaixaFinan(titulo);
                retorno.retorno = tituloCad;
                if (tituloCad.cd_titulo <= 0)
                    retorno.AddMensagem(Messages.msgNotUpReg, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, retorno);
                configureHeaderResponse(response, null);
                return response;
            }
            catch (FinanceiroBusinessException exe)
            {
                if (exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_TITULO_ENVIADO_CNAB)
                    return gerarLogException(exe.Message, retorno, logger, exe);
                else
                    return gerarLogException(Messages.msgNotUpReg, retorno, logger, exe);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
        }


        [HttpComponentesAuthorize(Roles = "mat")]
        [HttpComponentesAuthorize(Roles = "tit")]
        [HttpGet]
        public HttpResponseMessage getUrlRelatorioCarne(int cdContrato)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cd_pessoa_escola = this.ComponentesUser.CodEmpresa.Value;

                // Pega os parâmetros do usuário para criar a url do relatório:
                string parametros = "@cdContrato=" + cdContrato + "&@cdEscola=" + cd_pessoa_escola + "&@contaSegura=false";

                string parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);

                HttpResponseMessage response= Request.CreateResponse(HttpStatusCode.OK, parametrosCript);

                configureHeaderResponse(response, null);
                return response;

            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }
        [HttpComponentesAuthorize(Roles = "ctsg")]
        [HttpComponentesAuthorize(Roles = "mat")]
        [HttpComponentesAuthorize(Roles = "tit")]
        [HttpGet]
        public HttpResponseMessage getUrlRelatorioCarneGeral(int cdContrato)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cd_pessoa_escola = this.ComponentesUser.CodEmpresa.Value;

                // Pega os parâmetros do usuário para criar a url do relatório:
                string parametros = "@cdContrato=" + cdContrato + "&@cdEscola=" + cd_pessoa_escola + "&@contaSegura=true";

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

        [HttpComponentesAuthorize(Roles = "mat")]
        [HttpComponentesAuthorize(Roles = "tit")]
        [HttpGet]
        public HttpResponseMessage getUrlRelatorioCarneMovto(int cdMovimento)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cd_pessoa_escola = this.ComponentesUser.CodEmpresa.Value;

                // Pega os parâmetros do usuário para criar a url do relatório:
                string parametros = "@cdMovimento=" + cdMovimento + "&@cdEscola=" + cd_pessoa_escola + "&@contaSegura=false";

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
        [HttpComponentesAuthorize(Roles = "ctsg")]
        [HttpComponentesAuthorize(Roles = "mat")]
        [HttpComponentesAuthorize(Roles = "tit")]
        [HttpGet]
        public HttpResponseMessage getUrlRelatorioCarneGeralMovto(int cdMovimento)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cd_pessoa_escola = this.ComponentesUser.CodEmpresa.Value;

                // Pega os parâmetros do usuário para criar a url do relatório:
                string parametros = "@cdMovimento=" + cdMovimento + "&@cdEscola=" + cd_pessoa_escola + "&@contaSegura=true";

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

        [HttpComponentesAuthorize(Roles = "rpcqs")]
        public HttpResponseMessage getUrlRelatorioCheques(int cd_pessoa_aluno, int cd_banco, string emitente, bool liquidados, string nm_cheques, string vl_titulo, int nm_agencia,
            int nm_ccorrente, string dt_ini_bPara, string dt_fim_bPara, string dt_ini, string dt_fim, bool emissao, bool liquidacao, byte natureza, string displayBanco)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cd_pessoa_escola = this.ComponentesUser.CodEmpresa.Value;
                DateTime? dataIncialbP = string.IsNullOrEmpty(dt_ini_bPara) ? null : (DateTime?)Convert.ToDateTime(dt_ini_bPara);
                DateTime? dataFinalbP = string.IsNullOrEmpty(dt_fim_bPara) ? null : (DateTime?)Convert.ToDateTime(dt_fim_bPara);
                DateTime? dataIncial = string.IsNullOrEmpty(dt_ini) ? null : (DateTime?)Convert.ToDateTime(dt_ini);
                DateTime? dataFinal = string.IsNullOrEmpty(dt_fim) ? null : (DateTime?)Convert.ToDateTime(dt_fim);
                //cd_empresa
                // Pega os parâmetros do usuário para criar a url do relatório:
                string parametros = "@cdPessoaAluno=" + cd_pessoa_aluno + "&@cdEscola=" + cd_pessoa_escola + "&@cd_banco=" + cd_banco + "&@emitente=" + emitente + "&@liquidados=" + liquidados +
                                    "&@nm_cheques=" + nm_cheques + "&@vl_titulo=" + vl_titulo + "&@nm_agencia=" + nm_agencia + "&@nm_ccorrente=" + nm_ccorrente + "&@dt_ini_bPara=" + dt_ini_bPara +
                                    "&@dt_fim_bPara=" + dt_fim_bPara + "&@dt_ini=" + dt_ini + "&@dt_fim=" + dt_fim + "&@emissao=" + emissao + "&@liquidacao=" + liquidacao + "&@natureza=" + natureza +
                                    "&@displayBanco=" + displayBanco;
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

        [HttpPost]
        [HttpComponentesAuthorize(Roles = "tit")]
        public HttpResponseMessage getTitulosForBaixaAutomatica(TituloUI titulo)
        {
            //ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();

                titulo.cd_pessoa_empresa = this.ComponentesUser.CodEmpresa.Value;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());

                var retorno = Business.getTitulosForBaixaAutomatica(parametros, titulo);
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
        [HttpComponentesAuthorize(Roles = "tit")]
        public HttpResponseMessage getTitulosForBaixaAutomaticaCheque(TituloChequeUI titulo)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();

                titulo.cd_pessoa_empresa = this.ComponentesUser.CodEmpresa.Value;
                var titulos = Business.getTitulosForBaixaAutomaticaCheque(titulo);
                retorno.retorno = titulos;
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "tit")]
        public HttpResponseMessage listarBaixaAutomaticasEfetuadasCheque([FromUri] BaixaAutomaticaUI baixaAutomaticaChequeUI)
        {
            //ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                baixaAutomaticaChequeUI.cd_escola = this.ComponentesUser.CodEmpresa.Value;
                var retorno = Business.listarBaixaAutomaticasEfetuadasCheque(parametros, baixaAutomaticaChequeUI);
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
        [HttpComponentesAuthorize(Roles = "tit")]
        public HttpResponseMessage listarBaixaAutomaticasEfetuadasCartao([FromUri] BaixaAutomaticaUI baixaAutomaticaChequeUI)
        {
            //ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                baixaAutomaticaChequeUI.cd_escola = this.ComponentesUser.CodEmpresa.Value;
                var retorno = Business.listarBaixaAutomaticasEfetuadasCartao(parametros, baixaAutomaticaChequeUI);
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
        [HttpComponentesAuthorize(Roles = "tit")]
        public HttpResponseMessage getBaixasEfetuadasForBaixaAutomaticaCheque([FromUri] BaixaAutomaticaUI baixaAutomaticaChequeUI)
        {
            //ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                baixaAutomaticaChequeUI.cd_escola = this.ComponentesUser.CodEmpresa.Value;
                var retorno = Business.getBaixasEfetuadasForBaixaAutomaticaCheque(parametros, baixaAutomaticaChequeUI);
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
        [HttpComponentesAuthorize(Roles = "tit")]
        public HttpResponseMessage getBaixasEfetuadasForBaixaAutomaticaCartao([FromUri] BaixaAutomaticaUI baixaAutomaticaCartaoUI)
        {
            //ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                baixaAutomaticaCartaoUI.cd_escola = this.ComponentesUser.CodEmpresa.Value;
                var retorno = Business.getBaixasEfetuadasForBaixaAutomaticaCartao(parametros, baixaAutomaticaCartaoUI);
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
        [HttpComponentesAuthorize(Roles = "tit")]
        public HttpResponseMessage gerarBaixaAutomatica(BaixaAutomaticaUI baixaAutomaticaUi)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();

                baixaAutomaticaUi.cd_escola = (int)this.ComponentesUser.CodEmpresa;
                baixaAutomaticaUi.cd_usuario = this.ComponentesUser.CodUsuario;
                int ret = Business.gerarBaixaAutomatica(baixaAutomaticaUi);

                if (ret == 1)
                {
                    string msg = Messages.msgProcedureError;

                    retorno.AddMensagem(Messages.msgProcedureError, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, JsonConvert.SerializeObject(retorno));
                    configureHeaderResponse(response, null);
                    return response;
                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                var msg = ex.InnerException.Message;

                gerarLogException(msg, retorno, logger, ex);//geraLog

                ExceptionHandler exceptionHandler = new ExceptionHandler(msg, "An erro has occurred.", ex.GetType().ToString(), ex.StackTrace);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, exceptionHandler);
                return response;
            }
        }

       

        [HttpComponentesAuthorize(Roles = "tit")]
        [HttpGet]
        public HttpResponseMessage getLocaisMovtoBaixaAutomaticaChequeById(int cd_local_banco)
        {
            ReturnResult retorno = new ReturnResult();
            List<LocalMovto> locaisMovto = new List<LocalMovto>();
            try
            {
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                IFinanceiroBusiness BusinessFinanceiro = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();

                var locaisMovtoCheque = BusinessFinanceiro.getAllLocalMovtoCheque(cdEscola)
                    .Where(t => t.cd_local_banco == cd_local_banco &&
                                (t.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.BANCO ||
                                 t.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.CAIXA));

                retorno.retorno = locaisMovtoCheque;
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "tit")]
        [HttpGet]
        public HttpResponseMessage getLocalMovtoBaixaAutomaticaCheque()
        {
            ReturnResult retorno = new ReturnResult();
            List<LocalMovto> locaisMovto = new List<LocalMovto>();
            try
            {
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                IFinanceiroBusiness BusinessFinanceiro = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();

                var locaisMovtoConsulta = BusinessFinanceiro.getAllLocalMovtoCheque(cdEscola).ToList();

                retorno.retorno = locaisMovtoConsulta.Distinct().OrderBy(x => x.no_local_movto);
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }
        [HttpComponentesAuthorize(Roles = "tit")]
        [HttpGet]
        public HttpResponseMessage getTituloAplicadoTaxaCartao(int cd_titulo, int cd_local_movto)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                IFinanceiroBusiness BusinessFinanceiro = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                Titulo titAplicado = BusinessFinanceiro.getTituloBaixaFinan(cd_titulo, cdEscola, TituloDataAccess.TipoConsultaTituloEnum.HAS_EDIT_TITULO_VIEW);
                titAplicado.cd_local_movto = cd_local_movto;
                retorno.retorno = BusinessFinanceiro.aplicarTituloTaxaBancaria(titAplicado);
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }
        #endregion

        #region Baixa

        [Obsolete]
        [HttpComponentesAuthorize(Roles = "tit.i, tit.a, tit.e")]
        [HttpComponentesAuthorize(Roles = "bfinan.a")]
        public HttpResponseMessage postUpdateTransacao(TransacaoFinanceira transacao)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });
                int cd_escola = (int)this.ComponentesUser.CodEmpresa;
                transacao.cd_pessoa_empresa = cd_escola;
                if (transacao.Baixas.Count() <= 0)
                {
                    var delTransFinan = Business.deleteTransFinanBaixa(transacao);
                    retorno.retorno = delTransFinan;
                    if (!delTransFinan)
                        retorno.AddMensagem(Messages.msgNotDeletedTrans, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                    else
                        retorno.AddMensagem(Messages.msgDeleteSucessTrans, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                else
                {

                    transacao = Business.editTransacao(transacao,false);
                    retorno.retorno = transacao;
                    if (transacao.cd_tran_finan <= 0)
                        retorno.AddMensagem(Messages.msgNotUpReg, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                    else
                        retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }

                foreach (BaixaTitulo bt in transacao.Baixas)
                {
                    bt.Titulo.BaixaTitulo = null;
                    bt.TransacaoFinanceira = null;
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

        [HttpComponentesAuthorize(Roles = "bfinan")]
        public HttpResponseMessage getBaixaTituloByCodTitulo(int cd_titulo)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                IEnumerable<BaixaTitulo> BaixasTitulo = Business.getBaixaTituloByIdTitulo(cd_titulo, cdEscola).ToList();
                retorno.retorno = BaixasTitulo;
                if (BaixasTitulo.Count() > 0)
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

        [HttpComponentesAuthorize(Roles = "bfinan")]
        public HttpResponseMessage getBaixaTituloEComponentesTrasacaoFinan(int cd_transacao_finan)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                bool isMaster = (bool)this.ComponentesUser.IdMaster;
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();

                int cod_pessoa_usuario = isMaster ? 0 : this.ComponentesUser.CodPessoaUsuario;

                TransacaoFinanceira trans = new TransacaoFinanceira();
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                trans = Business.getTransacaoFinanceira(cd_transacao_finan, cdEscola);
                if (trans.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.CHEQUE_AVISTA || trans.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.CHEQUE_PRE_DATADO)
                {
                    trans.cheque = Business.getChequeTransacao(trans.cd_tran_finan, trans.cd_pessoa_empresa);
                    trans.bancos = Business.getAllBanco().ToList();
                }
                trans.Baixas = Business.getBaixasTransacaoFinan(cd_transacao_finan, 0, 0, cdEscola, BaixaTituloDataAccess.TipoConsultaBaixaEnum.HAS_BAIXA_TITULO_EDIT).ToList();
                if (trans.cd_local_movto != null && trans.cd_local_movto > 0)
                    trans.LocaisMovimento = Business.getLocalMovimentoSomenteLeitura(cdEscola, (int)trans.cd_local_movto, LocalMovtoDataAccess.TipoConsultaLocalMovto.HAS_SIMULACAO_BAIXA, cod_pessoa_usuario).ToList();
                trans.TiposLiquidacao = Business.getTipoLiquidacao(TipoLiquidacaoDataAccess.TipoConsultaTipoLiquidacaoEnum.HAS_ATIVO, (int)trans.cd_tipo_liquidacao).ToList();
                if (trans.TiposLiquidacao != null && trans.TiposLiquidacao.Count > 0)
                    foreach (TipoLiquidacao t in trans.TiposLiquidacao)
                        t.TransacoesFinanceiras = null;
                if (trans.Baixas.Count > 0)
                {
                    var i = 0;
                    foreach (BaixaTitulo bt in trans.Baixas)
                    {
                        //Titulo t = Business.voltarEstadoAnteriorTitulo(bt, trans.cd_tran_finan, trans.cd_pessoa_empresa);
                        //bt.vl_principal_baixa = t.vl_saldo_titulo;
                        if (bt.id_baixa_parcial)
                            bt.vl_desconto_baixa = (Decimal)bt.vl_desconto_baixa_calculado;
                        bt.nm_baixa = i;
                        i++;
                    }
                }
                retorno.retorno = trans;
                if (trans.cd_tran_finan <= 0)
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

         [Obsolete]
        [HttpComponentesAuthorize(Roles = "tit.e")]
        [HttpComponentesAuthorize(Roles = "bfinan.e")]
        public HttpResponseMessage postDeleteTransFinanceiraBaixa(TransacaoFinanceira transacaoFinanceira)
        {
            ReturnResult retorno = new ReturnResult();
            var cd_escola = (int)this.ComponentesUser.CodEmpresa;
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });
                transacaoFinanceira.cd_pessoa_empresa = cd_escola;
                var delTransFinan = Business.deleteTransFinanBaixa(transacaoFinanceira);
                retorno.retorno = delTransFinan;
                if (!delTransFinan)
                    retorno.AddMensagem(Messages.msgNotDeletedReg, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                else
                    retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (FinanceiroBusinessException exe)
            {
                if (exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_BAIXA_BOLSA_CONTRATO ||
                    exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_TITULO_SALDO_NEGATIVO)
                    return gerarLogException(exe.Message, retorno, logger, exe);

                return gerarLogException(Messages.msgNotDeletedReg, retorno, logger, exe);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotDeletedReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "tit.e")]
        [HttpComponentesAuthorize(Roles = "mat.e")]
        public HttpResponseMessage postDeleteTransFinanceiraBaixaReturnTitulos(TransacaoFinanceira transacaoFinanceira)
        {
            ReturnResult retorno = new ReturnResult();
            int cd_escola = (int)this.ComponentesUser.CodEmpresa;
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });
                transacaoFinanceira.cd_pessoa_empresa = cd_escola;
                bool deleted = Business.deleteTransFinanBaixa(transacaoFinanceira);
                TransacaoFinanceira trasacao = new TransacaoFinanceira();
                if (transacaoFinanceira.cd_contrato > 0)
                    trasacao.titulosBaixa = Business.getTitulosByContratoLeitura(transacaoFinanceira.cd_contrato, cd_escola);
                if(transacaoFinanceira.cd_movimento > 0)
                    trasacao.titulosBaixa = Business.getTitulosGridByMovimento(transacaoFinanceira.cd_movimento, cd_escola, 0);
                retorno.retorno = trasacao;
                if (!deleted)
                    retorno.AddMensagem(Messages.msgNotDeletedReg, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                else
                    retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, retorno);
                configureHeaderResponse(response, null);
                return response;
            }
            catch (FinanceiroBusinessException exe)
            {
                if (exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_BAIXA_BOLSA_CONTRATO ||
                    exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_TITULO_SALDO_NEGATIVO)
                    return gerarLogException(exe.Message, retorno, logger, exe);

                return gerarLogException(Messages.msgNotDeletedReg, retorno, logger, exe);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotDeletedReg, retorno, logger, ex);
            }
        }

        //Método responsável pela emissão do relatório:
        [HttpComponentesAuthorize(Roles = "bfinan")]
        public HttpResponseMessage getUrlRelatorioReciboProspect(int cd_prospect)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                int pEscola = (int)this.ComponentesUser.CodEmpresa;
                // Pega os parâmetros do usuário para criar a url do relatório:
                string parametros = "@pEscola=" + pEscola + "&@pCodProspect=" + cd_prospect;
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

        //Método responsável pela emissão do relatório:
        [HttpComponentesAuthorize(Roles = "bfinan")]
        public HttpResponseMessage getUrlRelatorioRecibo(int cd_baixa_titulo, int id_origem_titulo)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                int pEscola = (int)this.ComponentesUser.CodEmpresa;
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });

                var existe_motivo_bolsa = Business.getVerificaReciboPagamentoByBaixa(cd_baixa_titulo, pEscola);
                if (existe_motivo_bolsa != null && (existe_motivo_bolsa.baixaTitulo.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA ||
                                                    existe_motivo_bolsa.baixaTitulo.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA_ADITIVO))
                    throw new FinanceiroBusinessException(Messages.msgErroReciboMotivoBolsa, null, FinanceiroBusinessException.TipoErro.ERRO_RECIBO_MOTIVO_BOLSA, false);

                // Pega os parâmetros do usuário para criar a url do relatório:
                string parametros = "@pEscola=" + pEscola + "&@pCodBaixaTitulo=" + cd_baixa_titulo + "&@pOrigemTitulo=" + id_origem_titulo;
                //parametros = HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8);
                string parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, parametrosCript);

                configureHeaderResponse(response, null);
                return response;
            }
            catch (FinanceiroBusinessException exe)
            {
                if (exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_RECIBO_MOTIVO_BOLSA)
                    return gerarLogException(exe.Message, retorno, logger, exe);
                else
                    return gerarLogException(Messages.msgNotUpReg, retorno, logger, exe);
            }
            catch (Exception exe)
            {
                return gerarLogException(Messages.msgErroRelatorio, retorno, logger, exe);
            }
        }

        [HttpComponentesAuthorize(Roles = "bfinan")]
        public HttpResponseMessage getUrlRelatorioReciboAgrupado(string cds_titulos_selecionados)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                int pEscola = (int)this.ComponentesUser.CodEmpresa;
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });

                List<int> titulosSelecionados = cds_titulos_selecionados.Split('|').Select(item => int.Parse(item)).ToList();

                if (Business.validaReciboAgrupadoAlunosDiferentes(titulosSelecionados, pEscola))
                    throw new FinanceiroBusinessException(Messages.msgErroAgruparTitulosAlunosDiferentes, null, FinanceiroBusinessException.TipoErro.ERRO_AGRUPAR_TITULOS_ALUNOS_DIFERENTES, false);

                if (Business.validaReciboAgrupadoResponsaveisDiferentes(titulosSelecionados, pEscola))
                    throw new FinanceiroBusinessException(Messages.msgErroAgruparTitulosResponsaveisDiferentes, null, FinanceiroBusinessException.TipoErro.ERRO_AGRUPAR_TITULOS_RESPONSAVEIS_DIFERENTES, false);

                // Pega os parâmetros do usuário para criar a url do relatório:
                string parametros = "@pEscola=" + pEscola + "&@pCdsTitulosSelecionados=" + cds_titulos_selecionados;
                //parametros = HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8);
                string parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, parametrosCript);

                configureHeaderResponse(response, null);
                return response;
            }
            catch (FinanceiroBusinessException exe)
            {
                if (exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_TITULOS_VALIDA_RECIBO_AGRUPADO_NOTFOUND ||
                    exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_AGRUPAR_TITULOS_ALUNOS_DIFERENTES ||
                    exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_AGRUPAR_TITULOS_RESPONSAVEIS_DIFERENTES )
                    return gerarLogException(exe.Message, retorno, logger, exe);
                else
                    return gerarLogException(Messages.msgNotUpReg, retorno, logger, exe);
            }
            catch (Exception exe)
            {
                return gerarLogException(Messages.msgErroRelatorio, retorno, logger, exe);
            }
        }

        //Método responsável pela emissão do relatório:
        [HttpComponentesAuthorize(Roles = "bfinan")]
        public HttpResponseMessage getUrlRelatorioReciboMovimento(int cd_baixa_titulo, int cd_movimento)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cd_escola = (int)this.ComponentesUser.CodEmpresa;
                // Pega os parâmetros do usuário para criar a url do relatório:
                string parametros = "@cd_escola=" + cd_escola + "&@cd_baixa_titulo=" + cd_baixa_titulo + "&@cd_movimento=" + cd_movimento;
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
        [HttpComponentesAuthorize(Roles = "bfinan")]
        public HttpResponseMessage componentesPesquisaBaixaFinan()
        {
            ReturnResult retorno = new ReturnResult();
            BaixaTitulo comPesquisabaixa = new BaixaTitulo();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                comPesquisabaixa.tiposLiquidacao = Business.getTipoLiquidacao(TipoLiquidacaoDataAccess.TipoConsultaTipoLiquidacaoEnum.HAS_BAIXA_TITULO, null).ToList();
                retorno.retorno = comPesquisabaixa;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }


        [HttpComponentesAuthorize(Roles = "item")]
        public HttpResponseMessage getVerificarTituloOrigemMatricula(int cd_baixa)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = this.ComponentesUser.CodEmpresa.Value;
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                bool eMatricula = Business.verificarTituloOrigemMatricula(cd_baixa, cdEscola);
                retorno.retorno = eMatricula;
                if (!eMatricula)
                {
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                };
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));

            }
            catch (FinanceiroBusinessException exe)
            {
                if (exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_ORIGEM_TITULO)
                    return gerarLogException(exe.Message, retorno, logger, exe);
                else
                    return gerarLogException(Messages.msgNotUpReg, retorno, logger, exe);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        #endregion

        #region Local de Movimento
        [HttpComponentesAuthorize(Roles = "locmv")]
        [HttpGet]
        public HttpResponseMessage getLocalMovtoSearch(string nome, string nmBanco, bool inicio, int status, int tipo, string pessoa)
        {
            try
            {
                if (nome == null)
                    nome = String.Empty;
                if (nmBanco == null)
                    nmBanco = String.Empty;

                bool isMaster = (bool)this.ComponentesUser.IdMaster;
                int cod_pessoa_usuario = isMaster ? 0 : this.ComponentesUser.CodPessoaUsuario;

                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());

                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var retorno = Business.getLocalMovtoSearch(parametros, cdEscola, nome, nmBanco, inicio, getStatus(status), tipo, pessoa, cod_pessoa_usuario);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }
        [HttpComponentesAuthorize(Roles = "locmv")]
        [HttpGet]
        public HttpResponseMessage getLocalMovtoByEscola(int cdEscola)
        {
            try
            {
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                //TO DO Michelangelo
                var retorno = Business.getLocalMovtoByEscola(cdEscola, 0, false);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "locmv")]
        [HttpGet]
        public HttpResponseMessage getLocalMovtoByEscolaLogin(int cd_local_movto)
        {
            try
            {
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                //TO DO Michelangelo
                var retorno = Business.getLocalMovtoByEscola(cdEscola, cd_local_movto, false);
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
        [HttpComponentesAuthorize(Roles = "locmv")]
        public HttpResponseMessage GetUrlRelatorioLocalMovto(string sort, int direction, string nome, string nmBanco, bool inicio, int status, int tipo, string pessoa)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                bool isMaster = (bool)this.ComponentesUser.IdMaster;
                int cod_pessoa_usuario = isMaster ? 0 : this.ComponentesUser.CodPessoaUsuario;

                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                // Pega os parâmetros do usuário para criar a url do relatório:
                string strParametrosSort = "@orientation=LANDSCAPE&@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@cdEscola=" + cdEscola + "&@nome=" + nome + "&@nmBanco=" + nmBanco + "&@inicio=" + inicio + "&@status=" + status + "&@tipo=" + tipo + "&@pessoa=" + pessoa + "&@pessoaUsuario=" + cod_pessoa_usuario + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=" + "Relatório de Local de Movimento&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.LocalMovtoSearch;
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

        [HttpComponentesAuthorize(Roles = "locmv")]
        [HttpGet]
        public HttpResponseMessage getLocalMovtoById(int cdLocalMovto)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;

                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var loc = Business.getLocalMovtoById(cdEscola, cdLocalMovto);
                retorno.retorno = loc;
                if (loc.cd_local_movto <= 0)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
            }
        }

        // Put api/<controller>/5
        [HttpComponentesAuthorize(Roles = "locmv.a")]
        public HttpResponseMessage PostAlterarLocalMovto(LocalMovto localMovto)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                validaTipoLocal(localMovto.nm_tipo_local, localMovto.cd_local_banco, localMovto.TaxaBancaria);
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                localMovto.cd_pessoa_empresa = cdEscola;
                var localMovtoRet = Business.putLocalMovto(localMovto);
                retorno.retorno = localMovtoRet;
                if (localMovtoRet.cd_local_movto <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (FinanceiroBusinessException exe)
            {
                if (exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_CARTEIRA_USADA_CNAB ||
                    exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_TRIGGER_ALTERAR_LOCAL_MOVIMENTO)
                    return gerarLogException(exe.Message, retorno, logger, exe);
                else
                    return gerarLogException(Messages.msgNotUpReg, retorno, logger, exe);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }

        }
        // Post api/<controller>/5
        [HttpComponentesAuthorize(Roles = "locmv.i")]
        public HttpResponseMessage PostLocalMovto(LocalMovto localMovto)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                validaTipoLocal(localMovto.nm_tipo_local, localMovto.cd_local_banco, localMovto.TaxaBancaria);
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                localMovto.cd_pessoa_empresa = cdEscola;
                var localMovtoRet = Business.postLocalMovto(localMovto);
                retorno.retorno = localMovtoRet;
                if (localMovtoRet.cd_local_movto <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (FinanceiroBusinessException exe)
            {
                if (exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_TRIGGER_INSERIR_LOCAL_MOVIMENTO)
                    return gerarLogException(exe.Message, retorno, logger, exe);
                else
                    return gerarLogException(Messages.msgNotUpReg, retorno, logger, exe);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }

        }

        // Delte api/<controller>
        [HttpComponentesAuthorize(Roles = "locmv.e")]
        public HttpResponseMessage PostDeleteLocalMovto(List<LocalMovto> localMovto)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });
                var deleted = Business.deleteAllLocalMovto(localMovto);
                retorno.retorno = deleted;
                if (!deleted)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (FinanceiroBusinessException exe)
            {
                if (exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_TRIGGER_DELETAR_LOCAL_MOVIMENTO)
                    return gerarLogException(exe.Message, retorno, logger, exe);
                else
                    return gerarLogException(Messages.msgNotUpReg, retorno, logger, exe);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotDeletedReg, retorno, logger, ex);
            }

        }

        [HttpComponentesAuthorize(Roles = "locmv")]
        public HttpResponseMessage getLocalMovto(int? cd_loc_mvto, int natureza)
        {
            ReturnResult retorno = new ReturnResult();
            int cdEscola = (int)this.ComponentesUser.CodEmpresa;

            bool isMaster = (bool)this.ComponentesUser.IdMaster;

            int cod_pessoa_usuario = isMaster ? 0 : this.ComponentesUser.CodPessoaUsuario;
            try
            {
                if (cd_loc_mvto == null)
                    cd_loc_mvto = 0;
                List<LocalMovto> results = new List<LocalMovto>();
                BaixaTitulo locaisMovtos = new BaixaTitulo();
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                //Local de Movimento Títulos
                if (natureza == (int)Titulo.NaturezaTitulo.RECEBER)
                    locaisMovtos.locaisMovtoTitulo = Business.getLocalMovimentoSomenteLeitura(cdEscola, (int)cd_loc_mvto, LocalMovtoDataAccess.TipoConsultaLocalMovto.HAS_TITULOS_NATUREZA_RECEBER, cod_pessoa_usuario);
                if (natureza == (int)Titulo.NaturezaTitulo.PAGAR)
                    locaisMovtos.locaisMovtoTitulo = Business.getLocalMovimentoSomenteLeitura(cdEscola, (int)cd_loc_mvto, LocalMovtoDataAccess.TipoConsultaLocalMovto.HAS_TITULOS_NATUREZA_PAGAR, cod_pessoa_usuario);
                if (natureza == (int)Titulo.NaturezaTitulo.TODAS)
                    locaisMovtos.locaisMovtoTitulo = Business.getLocalMovimentoSomenteLeitura(cdEscola, (int)cd_loc_mvto, LocalMovtoDataAccess.TipoConsultaLocalMovto.HAS_TODOS_TITULOS, cod_pessoa_usuario);
                //Local de Movimento Baixa
                if (natureza == (int)Titulo.NaturezaTitulo.RECEBER)
                    locaisMovtos.locaisMovtoBaixa = Business.getLocalMovimentoSomenteLeitura(cdEscola, (int)cd_loc_mvto, LocalMovtoDataAccess.TipoConsultaLocalMovto.HAS_BAIXA_NATUREZA_RECEBER, cod_pessoa_usuario);
                if (natureza == (int)Titulo.NaturezaTitulo.PAGAR)
                    locaisMovtos.locaisMovtoBaixa = Business.getLocalMovimentoSomenteLeitura(cdEscola, (int)cd_loc_mvto, LocalMovtoDataAccess.TipoConsultaLocalMovto.HAS_BAIXA_NATUREZA_PAGAR, cod_pessoa_usuario);
                if (natureza == (int)Titulo.NaturezaTitulo.TODAS)
                    locaisMovtos.locaisMovtoBaixa = Business.getLocalMovimentoSomenteLeitura(cdEscola, (int)cd_loc_mvto, LocalMovtoDataAccess.TipoConsultaLocalMovto.HAS_TODAS_BAIXAS, cod_pessoa_usuario);
                retorno.retorno = locaisMovtos;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }
        [HttpComponentesAuthorize(Roles = "locmv")]
        public HttpResponseMessage getLocalMovtoTituloRec()
        {
            ReturnResult retorno = new ReturnResult();
            int cdEscola = (int)this.ComponentesUser.CodEmpresa;

            bool isMaster = (bool)this.ComponentesUser.IdMaster;

            int cod_pessoa_usuario = isMaster ? 0 : this.ComponentesUser.CodPessoaUsuario;

            try
            {
                List<LocalMovto> results = new List<LocalMovto>();
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                results = Business.getLocalMovimentoSomenteLeitura(cdEscola, 0, LocalMovtoDataAccess.TipoConsultaLocalMovto.HAS_TITULOS_NATUREZA_RECEBER, cod_pessoa_usuario);
                retorno.retorno = results;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }
        [Obsolete]
        [HttpComponentesAuthorize(Roles = "locmv")]
        public HttpResponseMessage postLocalMovtoBaixa(TransacaoFinanceira transacao)
        {
            ReturnResult retorno = new ReturnResult();
            int cdEscola = (int)this.ComponentesUser.CodEmpresa;
            try
            {
                List<LocalMovto> results = new List<LocalMovto>();
                int? cd_loc_mvto = transacao.cd_local_movto;
                List<BaixaTitulo> baixaTitulo = transacao.Baixas.ToList();
                int natureza = baixaTitulo[0].id_natureza_titulo.Value;
                List<int> lista = new List<int>();

                foreach (BaixaTitulo baixa in baixaTitulo)
                    if (baixa.Titulo.LocalMovto != null && baixa.Titulo.LocalMovto.cd_pessoa_local > 0)
                        lista.Add(baixa.Titulo.LocalMovto.cd_pessoa_local.Value);

                bool isMaster = (bool)this.ComponentesUser.IdMaster;

                int cod_pessoa_usuario = isMaster ? 0 : this.ComponentesUser.CodPessoaUsuario;

                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                retorno.retorno = Business.getLocalMovtoBaixa(cdEscola, cd_loc_mvto, natureza, lista.ToArray(), cod_pessoa_usuario);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, retorno);
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "locmv")]
        [HttpGet]
        public HttpResponseMessage getLocalMovtoAndTipoLiquidacao(int cd_pessoa_responsavel, int cd_contrato)
        {
            try
            {
                LocalMovtoUI localMvtoUI = new LocalMovtoUI();
                bool isMaster = (bool)this.ComponentesUser.IdMaster;
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                int cod_pessoa_usuario = isMaster ? 0 : this.ComponentesUser.CodPessoaUsuario;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                //TO DO Michelangelo
                localMvtoUI.locaMovto = Business.getLocalMovimentoSomenteLeitura(cdEscola, 0, LocalMovtoDataAccess.TipoConsultaLocalMovto.HAS_SIMULACAO_BAIXA_GERAL_SEM_CARTAO, cod_pessoa_usuario).ToList();
                localMvtoUI.tipoLiquidacao = Business.getTipoLiquidacao().ToList();
                localMvtoUI.titulos = Business.getTituloByPessoaResponsavel(cd_pessoa_responsavel, cdEscola, cd_contrato, false).ToList();
                var retorno = localMvtoUI;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }
        [HttpComponentesAuthorize(Roles = "locmv")]
        [HttpComponentesAuthorize(Roles = "ctsg")]
        [HttpGet]
        public HttpResponseMessage getLocalMovtoAndTipoLiquidacaoGeral(int cd_pessoa_titulo, int cd_contrato)
        {
            try
            {
                LocalMovtoUI localMvtoUI = new LocalMovtoUI();
                bool isMaster = (bool)this.ComponentesUser.IdMaster;
                int cod_pessoa_usuario = isMaster ? 0 : this.ComponentesUser.CodPessoaUsuario;
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                IFinanceiroBusiness businessFinan = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                //TO DO Michelangelo
                localMvtoUI.locaMovto = Business.getLocalMovimentoSomenteLeitura(cdEscola, 0, LocalMovtoDataAccess.TipoConsultaLocalMovto.HAS_SIMULACAO_BAIXA, cod_pessoa_usuario).ToList();
                localMvtoUI.tipoLiquidacao = Business.getTipoLiquidacao().ToList();
                localMvtoUI.titulos = Business.getTituloByPessoaResponsavel(cd_pessoa_titulo, cdEscola, cd_contrato, true).ToList();
                localMvtoUI.bancos = businessFinan.getAllBanco().ToList();
                var retorno = localMvtoUI;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "locmv")]
        [HttpGet]
        public HttpResponseMessage getLocalMovtoSimulacaoBaixaTitulo()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                bool isMaster = (bool)this.ComponentesUser.IdMaster;
                int cod_pessoa_usuario = isMaster ? 0 : this.ComponentesUser.CodPessoaUsuario;
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();

                List<LocalMovto> bancos = Business.getLocalMovimentoSomenteLeitura(cdEscola, 0, LocalMovtoDataAccess.TipoConsultaLocalMovto.HAS_SIMULACAO_BAIXA, cod_pessoa_usuario).ToList();
                retorno.retorno = bancos;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "locmv")]
        [HttpGet]
        public HttpResponseMessage getLocalMovtoWithContaByEscola()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ContaCorrenteUI contaCC = new ContaCorrenteUI();

                int cdEscola = (int)this.ComponentesUser.CodEmpresa;

                bool isMaster = (bool)this.ComponentesUser.IdMaster;

                int cod_pessoa_usuario = isMaster ? 0 : this.ComponentesUser.CodPessoaUsuario;

                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                contaCC.localMovimento = Business.getLocalMovtoWithContaByEscola(cdEscola, cod_pessoa_usuario).ToList();
                contaCC.tipoLiquidacao = Business.getTipoLiquidacao().ToList();

                retorno.retorno = contaCC;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "locmv")]
        [HttpGet]
        public HttpResponseMessage getLocalMovtoWithContaCCByEscola()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ContaCorrenteUI contaCC = new ContaCorrenteUI();

                int cdEscola = (int)this.ComponentesUser.CodEmpresa;

                bool isMaster = (bool)this.ComponentesUser.IdMaster;

                int cod_pessoa_usuario = isMaster ? 0 : this.ComponentesUser.CodPessoaUsuario;

                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                contaCC.localMovimento = Business.getLocalMovtoWithContaByEscola(cdEscola, cod_pessoa_usuario).ToList();

                retorno.retorno = contaCC;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "locmv")]
        [HttpGet]
        public HttpResponseMessage getLocalMovtoETpLiquidacao(int cdLocalMovto, int? cdTpLiq, bool isCadastro)
        {
            try
            {
                LocalMovtoUI localMvtoUI = new LocalMovtoUI();

                int cdEscola = (int)this.ComponentesUser.CodEmpresa;

                bool isMaster = (bool)this.ComponentesUser.IdMaster;

                int cod_pessoa_usuario = isMaster ? 0 : this.ComponentesUser.CodPessoaUsuario;

                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                if (isCadastro)
                    localMvtoUI.locaMovto = Business.getLocalMovimentoSomenteLeitura(cdEscola, cdLocalMovto, LocalMovtoDataAccess.TipoConsultaLocalMovto.HAS_SIMULACAO_BAIXA, cod_pessoa_usuario).ToList();
                else
                    localMvtoUI.locaMovto = Business.getLocalMovtoProspect(cdEscola, cdLocalMovto, cod_pessoa_usuario).ToList();
                
                localMvtoUI.tipoLiquidacao = Business.getTipoLiquidacaoCd(cdTpLiq).ToList();
                var retorno = localMvtoUI;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }
        #endregion

        #region Taxa Bancaria
        private void validaTipoLocal(byte nm_tipo_local, int? cd_local_banco, ICollection<TaxaBancaria> TaxaBancaria)
        {
            if (nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_CREDITO ||
                    nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.CARTAO_DEBITO)
            {
                //if (!cd_local_banco.HasValue)
                //    throw new FinanceiroBusinessException(Messages.msgErroLocalBancoObrigatorio, null, FinanceiroBusinessException.TipoErro.ERRO_LOCAL_BANCO_OBRIGATORIO, false);

                if (TaxaBancaria == null || TaxaBancaria.Count == 0)
                    throw new FinanceiroBusinessException(Messages.msgErroTaxaBancariaObrigatorio, null, FinanceiroBusinessException.TipoErro.ERRO_TAXA_BANCARIA_OBRIGATORIO, false);
            }
        }

        [HttpComponentesAuthorize(Roles = "locmv")]
        [HttpGet]
        public HttpResponseMessage getTaxaBancariaPorId(int cd_taxa_bancaria)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var loc = Business.getTaxaBancariaPorId(cd_taxa_bancaria);
                retorno.retorno = loc;
                if (loc.cd_taxa_bancaria <= 0)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
            }
        }
        #endregion

        #region Histórico da Turma
        [HttpComponentesAuthorize(Roles = "hista")]
        [HttpComponentesAuthorize(Roles = "tit")]
        public HttpResponseMessage getTitulosHistoricoAluno(int cd_pessoa, int cd_tipo)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cd_escola = this.ComponentesUser.CodEmpresa.Value;
                TituloDataAccess.TipoConsultaTituloEnum tipo = (TituloDataAccess.TipoConsultaTituloEnum)cd_tipo;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                List<Titulo> lista_titulos = Business.getTituloByPessoa(parametros, cd_pessoa, cd_escola, tipo, false).ToList();
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, lista_titulos);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
            }
        }
        [HttpComponentesAuthorize(Roles = "hista")]
        [HttpComponentesAuthorize(Roles = "tit")]
        [HttpComponentesAuthorize(Roles = "ctsg")]
        public HttpResponseMessage getTitulosHistoricoAlunoGeral(int cd_pessoa, int cd_tipo)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cd_escola = this.ComponentesUser.CodEmpresa.Value;
                TituloDataAccess.TipoConsultaTituloEnum tipo = (TituloDataAccess.TipoConsultaTituloEnum)cd_tipo;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                List<Titulo> lista_titulos = Business.getTituloByPessoa(parametros, cd_pessoa, cd_escola, tipo, true).ToList();
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

        #region Política Comercial

        [HttpComponentesAuthorize(Roles = "pCom")]
        [HttpGet]
        public HttpResponseMessage getPoliticaComercialSearch(string descricao, bool inicio, int ativo, bool parcIguais, bool vencFixo)
        {
            try
            {
                if (descricao == null)
                    descricao = String.Empty;

                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());

                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var retorno = Business.getPoliticaComercialSearch(parametros, descricao, inicio, getStatus(ativo), parcIguais, vencFixo, cdEscola);
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
        [HttpComponentesAuthorize(Roles = "pCom")]
        public HttpResponseMessage getUrlRelatorioPoliticaComercial(string sort, int direction, string descricao, bool inicio, bool? ativo, bool parcIguais, bool vencFixo)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                // Pega os parâmetros do usuário para criar a url do relatório:
                string strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@cdEscola=" + cdEscola + "&@descricao=" + descricao + "&@inicio=" + inicio + "&@ativo=" + ativo + "&@parcIguais=" + parcIguais + "&@vencFixo=" + vencFixo + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=" + "Relatório de Política Comercial&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.PoliticaComercialSearch;
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

        [HttpComponentesAuthorize(Roles = "pCom")]
        [HttpGet]
        public HttpResponseMessage getPoliticaComercialById(int cdPoliticaCom)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;

                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var polCom = Business.getPoliticaComercialById(cdPoliticaCom, cdEscola);
                if (polCom.ItemPolitica.Count() > 0)
                    foreach (ItemPolitica ip in polCom.ItemPolitica)
                        if (ip.PoliticaComercial != null)
                            ip.PoliticaComercial = null;

                retorno.retorno = polCom;


                if (polCom.cd_politica_comercial <= 0)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "pCom")]
        [HttpGet]
        public HttpResponseMessage getPoliticaComercialByEmpresa(string politica, bool inicio, int? cdEscola)
        {
            try
            {
                int cd_escola = 0;
                if (!cdEscola.HasValue)
                    cd_escola = (int)this.ComponentesUser.CodEmpresa.Value;
                else
                    cd_escola = cdEscola.Value;


                if (politica == null)
                    politica = String.Empty;

                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                IEnumerable<PoliticaComercial> politicaComercial = Business.getPoliticaComercialByEmpresa(cd_escola, politica, inicio);

                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, politicaComercial);

                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "pCom.i")]
        public HttpResponseMessage postPoliticaComercial(PoliticaComercial politica)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });
                politica.cd_pessoa_empresa = (int)this.ComponentesUser.CodEmpresa;
                PoliticaComercial politicaComercial = Business.addPoliticaComercial(politica);
                if (politica.ItemPolitica != null && politica.ItemPolitica.Count() > 0)
                    foreach (ItemPolitica i in politica.ItemPolitica)
                        i.PoliticaComercial = null;
                retorno.retorno = politicaComercial;

                if (politicaComercial.cd_politica_comercial <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (FinanceiroBusinessException ex)
            {
                if (ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_SUM_DIFERENTE_POLCOM)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                if (ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_PARC_DIF_OBRIGATORIO)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                if (ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_DIA_POL_COMERCIAL_NULL)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "pCom.a")]
        public HttpResponseMessage postAlterarPoliticaComercial(PoliticaComercial politica)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });
                politica.cd_pessoa_empresa = (int)this.ComponentesUser.CodEmpresa;
                PoliticaComercial politicaComercial = Business.editPolCom(politica);
                retorno.retorno = politicaComercial;

                if (politicaComercial.cd_politica_comercial <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (FinanceiroBusinessException ex)
            {
                if (ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_SUM_DIFERENTE_POLCOM)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                if (ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_PARC_DIF_OBRIGATORIO)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                if (ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_DIA_POL_COMERCIAL_NULL)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }
        }


        [HttpComponentesAuthorize(Roles = "pCom.e")]
        public HttpResponseMessage postDeletePoliticaComercial(List<PoliticaComercial> politicas)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });
                var cdEscola = (int)this.ComponentesUser.CodEmpresa;
                var del = Business.deleteAllPolCom(politicas, cdEscola);
                retorno.retorno = del;
                if (del == false)
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

        #endregion

        #region Conta Corrente

        [HttpComponentesAuthorize(Roles = "coco")]
        [HttpComponentesAuthorize(Roles = "mvfin")]
        [HttpComponentesAuthorize(Roles = "locmv")]
        public HttpResponseMessage getCarregarCamposCadastroConta()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEmpresa = this.ComponentesUser.CodEmpresa.Value;
                ContaCorrenteList contaCorrente = new ContaCorrenteList();

                bool isMaster = (bool)this.ComponentesUser.IdMaster;
                int cod_pessoa_usuario = isMaster ? 0 : this.ComponentesUser.CodPessoaUsuario;

                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                contaCorrente.movimentacaoFinanceira = Business.getMovimentacaoAtivaWithConta(cdEmpresa, true).ToList();
                if (isMaster)
                {
                    contaCorrente.localMovimentoDestino = Business.getLocalMovtoAtivosWithConta(cdEmpresa, false, true, 0).ToList();
                    contaCorrente.localMovimentoOrigem = Business.getLocalMovtoAtivosWithConta(cdEmpresa, true, true, 0).ToList();
                }
                else
                {
                    contaCorrente.localMovimentoDestino = Business.getLocalMovtoAtivosWithContaUsuario(cdEmpresa, false, 0, cod_pessoa_usuario).ToList();
                    contaCorrente.localMovimentoOrigem = Business.getLocalMovtoAtivosWithContaUsuario(cdEmpresa, true, 0, cod_pessoa_usuario).ToList();
                }
                contaCorrente.tiposLiquidacao = Business.getTipoLiquidacao(TipoLiquidacaoDataAccess.TipoConsultaTipoLiquidacaoEnum.HAS_ATIVO, null).ToList();
                retorno.retorno = contaCorrente;

                if (contaCorrente == null)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }
        [HttpComponentesAuthorize(Roles = "coco")]
        [HttpComponentesAuthorize(Roles = "mvfin")]
        [HttpComponentesAuthorize(Roles = "locmv")]
        [HttpPost]
        public HttpResponseMessage getCarregarCamposEditarConta(ContaCorrenteUI contaCorrenteUI)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEmpresa = this.ComponentesUser.CodEmpresa.Value;
                ContaCorrenteList contaCorrente = new ContaCorrenteList();

                bool isMaster = (bool)this.ComponentesUser.IdMaster;
                int cod_pessoa_usuario = isMaster ? 0 : this.ComponentesUser.CodPessoaUsuario;

                int cdLocalDestino = contaCorrenteUI.cd_local_destino.HasValue ? contaCorrenteUI.cd_local_destino.Value : 0;
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                
                if (isMaster)
                {
                    contaCorrente.localMovimentoDestino = Business.getLocalMovtoAtivosWithConta(cdEmpresa, false, false, cdLocalDestino).ToList();//Business.getLocalMovtoAtivosWithCodigo(cdEmpresa, false, contaCorrenteUI.cd_local_destino ?? 0).ToList();
                    contaCorrente.localMovimentoOrigem = Business.getLocalMovtoAtivosWithConta(cdEmpresa, true, false, contaCorrenteUI.cd_local_origem).ToList();//Business.getLocalMovtoAtivosWithCodigo(cdEmpresa, true, contaCorrenteUI.cd_local_origem).ToList();
                }
                else
                {
                    contaCorrente.localMovimentoDestino = Business.getLocalMovtoAtivosWithContaUsuario(cdEmpresa, false, cdLocalDestino, cod_pessoa_usuario).ToList();//Business.getLocalMovtoAtivosWithCodigo(cdEmpresa, false, contaCorrenteUI.cd_local_destino ?? 0).ToList();
                    contaCorrente.localMovimentoOrigem = Business.getLocalMovtoAtivosWithContaUsuario(cdEmpresa, true, contaCorrenteUI.cd_local_origem, cod_pessoa_usuario).ToList();//Business.getLocalMovtoAtivosWithCodigo(cdEmpresa, true, contaCorrenteUI.cd_local_origem).ToList();
                }
               
                if (contaCorrenteUI.cd_local_destino > 0 && contaCorrenteUI.cd_local_destino.HasValue)
                    contaCorrente.movimentacaoFinanceira = Business.getMovimentacaoTransferencia(cdEmpresa, contaCorrenteUI.cd_movimentacao_financeira).ToList();
                else
                    contaCorrente.movimentacaoFinanceira = Business.getMovimentacaoAtivaWithConta(cdEmpresa, contaCorrenteUI.cd_movimentacao_financeira).ToList();
                
                if (contaCorrenteUI.cd_tipo_liquidacao > 0)
                    contaCorrente.tiposLiquidacao = Business.getTipoLiquidacao(TipoLiquidacaoDataAccess.TipoConsultaTipoLiquidacaoEnum.HAS_ATIVO, contaCorrenteUI.cd_tipo_liquidacao).ToList();
                else
                    contaCorrente.tiposLiquidacao = Business.getTipoLiquidacao(TipoLiquidacaoDataAccess.TipoConsultaTipoLiquidacaoEnum.HAS_ATIVO, null).ToList();
                ContaCorrenteUI ccRetorno = Business.getContaCorretePlanoConta(cdEmpresa, contaCorrenteUI.cd_conta_corrente);
                if (ccRetorno != null)
                {
                    contaCorrente.cd_plano_conta = ccRetorno.cd_plano_conta;
                    contaCorrente.planoConta = ccRetorno.planoConta;
                }
                retorno.retorno = contaCorrente;

                if (contaCorrente == null)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "coco")]
        [HttpComponentesAuthorize(Roles = "mvfin")]
        [HttpComponentesAuthorize(Roles = "locmv")]
        [HttpComponentesAuthorize(Roles = "plct")]
        [HttpGet]
        public HttpResponseMessage getContaCorrenteSearch(int cdOrigem, int cdDestino, byte entraSaida, int cdMovimento, int cdPlanoConta, string dta_ini, string dta_fim)
        {
            try
            {
                bool isMaster = (bool)this.ComponentesUser.IdMaster;
                int cod_pessoa_usuario = isMaster ? 0 : this.ComponentesUser.CodPessoaUsuario;

                DateTime dtaInicial = DateTime.MinValue;
                DateTime dtaFinal = DateTime.MaxValue;

                if (!String.IsNullOrEmpty(dta_ini))
                    dtaInicial = Convert.ToDateTime(dta_ini);

                if (!String.IsNullOrEmpty(dta_fim))
                    dtaFinal = Convert.ToDateTime(dta_fim);

                // Verifica permissão "Conta Segura" do usuário.
                bool contaSegura = this.ComponentesUser.Permissao.Contains("ctsg");

                int cdEmpresa = this.ComponentesUser.CodEmpresa.Value;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var retorno = Business.getContaCorreteSearch(parametros, cdEmpresa, cdOrigem, cdDestino, entraSaida, cdMovimento, cdPlanoConta, dtaInicial, dtaFinal, cod_pessoa_usuario, contaSegura);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "coco")]
        public HttpResponseMessage getUrlRelatorioContaCorrente(string sort, int direction, int cdOrigem, int cdDestino, byte entraSaida, int cdMovimento, int cdPlanoConta, string dta_ini, string dta_fim)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cd_pessoa_escola = this.ComponentesUser.CodEmpresa.Value;
                bool isMaster = (bool)this.ComponentesUser.IdMaster;
                int cod_pessoa_usuario = isMaster ? 0 : this.ComponentesUser.CodPessoaUsuario;
                // Verifica permissão "Conta Segura".
                bool contaSegura = this.ComponentesUser.Permissao.Contains("ctsg");
                // Pega os parâmetros do usuário para criar a url do relatório:
                string strParametrosSort = "@orientation=Portrait&@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@cdOrigem=" + cdOrigem + "&@cdDestino=" + cdDestino + "&@entraSaida=" + entraSaida +
                                    "&@cdMovimento=" + cdMovimento + "&@cdPlanoConta=" + cdPlanoConta + "&@dta_ini=" + dta_ini + "&@dta_fim=" + dta_fim + "&@cd_pessoa_escola=" + cd_pessoa_escola + "&@pessoaUsuario=" + cod_pessoa_usuario +
                                    "&@contaSegura=" + contaSegura + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=Relatório de Conta Corrente&" +
                                    Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.ContaCorrente;

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
        [HttpComponentesAuthorize(Roles = "fech")]
        [HttpGet]
        public HttpResponseMessage getFechamentoCaixaTpLiquidacao(string dta_fechamento, int cdUsuario, byte tipoLocal, bool mostrarZerados)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                DateTime dtaFechamento = dta_fechamento == null ? new DateTime() : (DateTime)Convert.ToDateTime(dta_fechamento);
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                IEnumerable<ContaCorrenteUI> contas = Business.getFechamentoCaixaTpLiquidacao(cdEscola, dtaFechamento, cdUsuario, tipoLocal, mostrarZerados);
                retorno.retorno = contas;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }
        [HttpComponentesAuthorize(Roles = "fech")]
        [HttpGet]
        public HttpResponseMessage getFechamentoCaixaLocalMovto(string dta_fechamento, int tipoLiquidacao, int cdUsuario, byte tipoLocal, bool mostrarZerados)
        {
            ReturnResult retorno = new ReturnResult();
            try
            { 
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                DateTime dtaFechamento = dta_fechamento == null ? new DateTime() : (DateTime)Convert.ToDateTime(dta_fechamento);
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                IEnumerable<ContaCorrenteUI> contas = Business.getFechamentoCaixaLocalMovto(cdEscola, dtaFechamento, tipoLiquidacao, cdUsuario, tipoLocal, mostrarZerados);
                retorno.retorno = contas;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "fech")]
        [HttpPost]
        public HttpResponseMessage postZerarSaldoFinanceiro([FromBody] SaldoFinanceiroParams saldoFinanceiroParams)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                DateTime dataBase = (DateTime)Convert.ToDateTime(saldoFinanceiroParams.dta_base);
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                Business.postZerarSaldoFinanceiro(cdEscola,saldoFinanceiroParams.cd_tipo_liquidacao, dataBase,saldoFinanceiroParams.tipo);
                retorno.retorno = new {status = "ok"};
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(ex.Message, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "fech")]
        [HttpGet]
        public HttpResponseMessage getUrlFechamentoCaixaSint(string dta_fechamento, int cdUsuario, byte tipoLocal, bool mostrarZerados)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cd_pessoa_escola = this.ComponentesUser.CodEmpresa.Value;
                DateTime dtaFechamento = dta_fechamento == null ? new DateTime() : (DateTime)Convert.ToDateTime(dta_fechamento);
                int cdUsuarioConsolidado = this.ComponentesUser.CodUsuario;
                // Pega os parâmetros do usuário para criar a url do relatório:
                string parametros = "@cdEscola=" + cd_pessoa_escola + "&@dtaFechamento=" + dtaFechamento + "&@cdUsuario=" + cdUsuario + "&@cdUsuarioConsolidado=" + cdUsuarioConsolidado + "&@tipoLocal=" + tipoLocal +
                    "&@mostrarZerados=" + mostrarZerados;
                
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

        #region Relatórios personalizados

        //Método responsável pela emissão do relatório:
        [HttpComponentesAuthorize(Roles = "rpbal")]
        public HttpResponseMessage getUrlRelatorioBalanceteMensal(int mes, int ano, int nivel, int nivel_analisar, bool mostrar_contas)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                int pEscola = (int)this.ComponentesUser.CodEmpresa;
                string parametros = "@pEmpresa=" + pEscola + "&mes=" + mes + "&ano=" + ano + "&@nivel_analisar=" + nivel_analisar + "&nivel=" + nivel + "&@mostrar_contas=" + mostrar_contas + "&@conta_segura=" + this.ComponentesUser.Permissao.Contains("ctsg");
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

        //Método responsável pela emissão do relatório:
        [HttpComponentesAuthorize(Roles = "gest")]
        public HttpResponseMessage getUrlRelatorioPosicaoFinanceira(string pDtaI, string pDtaF, int pForn, string pDtaBase, byte pNatureza, int pPlanoContas,
                                                                    int tpMovto, int ordem, bool pAnalitico, bool pMostraResp, bool pMostraDados, bool pMostraEndereco,
                                                                    bool pMostraDesconto, bool pMostraContas, bool pMostraCCManual, int cdTpLiq, int cdTpFinan, bool pMostarRecibo, bool pMostrarCpfResponsavel,
                                                                    string pSituacoes, int cdLocal)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                int pEscola = (int)this.ComponentesUser.CodEmpresa;


                if (pDtaF == null)
                    pDtaF = DateTime.Now.Date + "";
                if (pDtaI == null)
                    pDtaI = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, DateTime.Now.Day) + "";
                if (pDtaBase == null)
                    pDtaBase = DateTime.Now.Date + "";

                // Pega os parâmetros do usuário para criar a url do relatório:
                string parametros = "@pEscola=" + pEscola + "&@pDtaI=" + Convert.ToDateTime(pDtaI) + "&@pDtaF=" + Convert.ToDateTime(pDtaF) + "&@pForn=" + pForn + "&@pDtaBase=" + Convert.ToDateTime(pDtaBase) + "&@cSegura=false" + "&PNatureza=" + pNatureza +
                                    "&@pPlanoContas=" + pPlanoContas + "&@tpMovto=" + tpMovto + "&@ordem=" + ordem + "&PAnalitico=" + pAnalitico + "&PMostraResp=" + pMostraResp +
                                    "&PMostraDados=" + pMostraDados + "&PMostraEndereco=" + pMostraEndereco + "&PMostraDesconto=" + pMostraDesconto + "&PMostraContas=" + pMostraContas + "&PMostraCCManual=" + pMostraCCManual +
                                    "&@PMostrarRecibo=" + pMostarRecibo + "&@cdTpLiq=" + cdTpLiq + "&@cdTpFinan=" + cdTpFinan + "&@pMostrarCpfResponsavel=" + false + "&@pSituacoes=" + pSituacoes + "&@cdLocal=" + cdLocal;
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

        [HttpComponentesAuthorize(Roles = "ctsg")]
        [HttpComponentesAuthorize(Roles = "gest")]
        public HttpResponseMessage getUrlRelatorioPosicaoFinanceiraGeral(string pDtaI, string pDtaF, int pForn, string pDtaBase, byte pNatureza, int pPlanoContas,
                                                                    int tpMovto, int ordem, bool pAnalitico, bool pMostraResp, bool pMostraDados, bool pMostraEndereco,
                                                                    bool pMostraDesconto, bool pMostraContas, bool pMostraCCManual, int cdTpLiq, int cdTpFinan, bool pMostarRecibo,bool pMostrarCpfResponsavel,
                                                                    string pSituacoes, string pCdTurma, string pNoTurma, bool? pCancelamento, int cdLocal)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                int pEscola = (int)this.ComponentesUser.CodEmpresa;

                if (pDtaF == null)
                    pDtaF = DateTime.Now.Date + "";
                if (pDtaI == null)
                    pDtaI = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, DateTime.Now.Day) + "";
                if (pDtaBase == null)
                    pDtaBase = DateTime.Now.Date + "";

                // Pega os parâmetros do usuário para criar a url do relatório:
                string parametros = "@pEscola=" + pEscola + "&@pDtaI=" + Convert.ToDateTime(pDtaI) + "&@pDtaF=" + Convert.ToDateTime(pDtaF) + "&@pForn=" + pForn + "&@pDtaBase=" + Convert.ToDateTime(pDtaBase) + "&@cSegura=true" + "&PNatureza=" + pNatureza +
                                    "&@pPlanoContas=" + pPlanoContas + "&@tpMovto=" + tpMovto + "&@ordem=" + ordem + "&PAnalitico=" + pAnalitico + "&PMostraResp=" + pMostraResp +
                                    "&PMostraDados=" + pMostraDados + "&PMostraEndereco=" + pMostraEndereco + "&PMostraDesconto=" + pMostraDesconto + "&PMostraContas=" + pMostraContas + "&PMostraCCManual=" + pMostraCCManual +
                                    "&@PMostrarRecibo=" + pMostarRecibo + "&@cdTpLiq=" + cdTpLiq + "&@cdTpFinan=" + cdTpFinan + "&@pMostrarCpfResponsavel=" + pMostrarCpfResponsavel + "&@pSituacoes=" + pSituacoes + "&@pCdTurma=" + pCdTurma + "&@pNoTurma=" + pNoTurma
                                    + "&@pCancelamento=" + pCancelamento + "&@cdLocal=" + cdLocal;
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

        [HttpComponentesAuthorize(Roles = "coco")]
        [HttpComponentesAuthorize(Roles = "locmv")]
        [HttpGet]
        public HttpResponseMessage getUrlRelatorioContaCorrente(int cd_local_movto, string dta_ini, string dta_fim, int tipoLiquidacao, string desTipoLiquidacao)
        {
            ReturnResult retorno = new ReturnResult();

            DateTime dataIncial = new DateTime();
            DateTime dataFinal = new DateTime();
            bool contaSegura = false;
            bool isMaster = (bool)this.ComponentesUser.IdMaster;
            try
            {
                int cd_pessoa_escola = this.ComponentesUser.CodEmpresa.Value;

                if (String.IsNullOrEmpty(dta_ini))
                    dataIncial = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, DateTime.Now.Day);
                else
                    dataIncial = Convert.ToDateTime(dta_ini);

                if (!String.IsNullOrEmpty(dta_fim))
                    dataFinal = Convert.ToDateTime(dta_fim);

                // Verifica permissão "Conta Segura" do usuário.
                if (!string.IsNullOrEmpty(ComponentesUser.Permissao))
                    contaSegura = ComponentesUser.Permissao.Contains("ctsg");

                // Pega os parâmetros do usuário para criar a url do relatório:
                string parametros = "@cdEscola=" + cd_pessoa_escola + "&@pDtaI=" + dataIncial + "&@pDtaF=" + dataFinal + "&@pLocalMovto=" + cd_local_movto + "&@tipoLiquidacao=" + tipoLiquidacao + "&@desTipoLiquidacao=" + desTipoLiquidacao + "&@contaSegura=" + contaSegura + "&@isMaster=" + isMaster;

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

        [HttpPost]
        [HttpComponentesAuthorize(Roles = "bib")]
        [HttpComponentesAuthorize(Roles = "item")]
        [HttpComponentesAuthorize(Roles = "gest")]
        public HttpResponseMessage getUrlRelatorioEstoque(EstoqueUI estoqueUI)
        {
            ReturnResult retorno = new ReturnResult();

            DateTime dataIncial = new DateTime();
            DateTime dataFinal = new DateTime();

            try
            {
                string parametros = "";

                if (!String.IsNullOrEmpty(estoqueUI.dtaInicio))
                    dataIncial = Convert.ToDateTime(estoqueUI.dtaInicio);

                if (!String.IsNullOrEmpty(estoqueUI.dtaFim))
                    dataFinal = Convert.ToDateTime(estoqueUI.dtaFim);

                int cd_pessoa_escola = this.ComponentesUser.CodEmpresa.Value;

                // Pega os parâmetros do usuário para criar a url do relatório:
                switch (estoqueUI.relatorio)
                {
                    case (int)EstoqueUI.relatorioEstoque.MOVIMENTOS:
                        {
                            parametros = "@cdEscola=" + cd_pessoa_escola + "&@pRelatorio=" + estoqueUI.relatorio + "&@pTipoItem=" + estoqueUI.tipoItem
                                                      + "&@pTipoRelatorio=" + estoqueUI.tipoRpt + "&@pGrupoItem=" + estoqueUI.grupoItem + "&@pItem=" + estoqueUI.item
                                                      + "&@pDtaI=" + dataIncial + "&@pDtaF=" + dataFinal + "&@pDcGrupo=" + estoqueUI.dc_grupo
                                                      + "&@pDcItem=" + estoqueUI.dc_item + "&@pNoRelatorio=" + "Movimentação de Estoque" + "&@pisApenasItensMovimento=" + estoqueUI.isApenasItensMovimento;
                        }
                        break;
                    case (int)EstoqueUI.relatorioEstoque.ITENS:
                        {
                            parametros = "@cdEscola=" + cd_pessoa_escola + "&@pRelatorio=" + estoqueUI.relatorio + "&@pTipoItem=" + estoqueUI.tipoItem
                                                      + "&@pGrupoItem=" + estoqueUI.grupoItem + "&@pDcGrupo=" + estoqueUI.dc_grupo + "&@pItem=" + estoqueUI.item
                                                      + "&@pDcItem=" + estoqueUI.dc_item + "&@pContagem=" + estoqueUI.isColunaContagem + "&@pContado=" + estoqueUI.isColunaContatos
                                                      + "&@pAno=" + estoqueUI.ano + "&@pMes=" + estoqueUI.mes + "&@pNoRelatorio=" + "Listagem de itens" + "&@isSemColunaC=" + estoqueUI.isSemColunaC;

                        }
                        break;
                    case (int)EstoqueUI.relatorioEstoque.SALDO_REAL_BIBLIOTECA:
                        {
                            parametros = "@cdEscola=" + cd_pessoa_escola + "&@pTipoItem=" + estoqueUI.tipoItem + "&@pGrupoItem=" + estoqueUI.grupoItem
                                                      + "&@pDcGrupo=" + estoqueUI.dc_grupo + "&@pItem=" + estoqueUI.item + "&@pDcItem=" + estoqueUI.dc_item
                                                      + "&@pRelatorio=" + estoqueUI.relatorio + "&@pNoRelatorio=" + "Saldo Real de Biblioteca";
                        }
                        break;
                }

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

        [HttpPost]
        [HttpComponentesAuthorize(Roles = "rptinv")]
        public HttpResponseMessage getUrlRelatorioInventario(EstoqueUI estoqueUI)
        {
            ReturnResult retorno = new ReturnResult();

            DateTime dataBase = new DateTime();

            try
            {

                IEmpresaBusiness BusinessEmpresa = (IEmpresaBusiness)base.instanciarBusiness<IEmpresaBusiness>();
                bool masterGeral = BusinessEmpresa.VerificarMasterGeral(this.ComponentesUser.CodUsuario);
                if (!masterGeral && estoqueUI.isSemColunaC)
                {
                    throw new FinanceiroBusinessException("Todas as Escolas somente disponivel para Administradores Gerais do Sistema", null, FinanceiroBusinessException.TipoErro.ERRO_USUARIO_MASTER, false);
                };


                string parametros = "";

                if (!String.IsNullOrEmpty(estoqueUI.dtaInicio))
                    dataBase = Convert.ToDateTime(estoqueUI.dtaInicio);

                int cd_pessoa_escola = this.ComponentesUser.CodEmpresa.Value;
                if (estoqueUI.isSemColunaC)
                {
                    cd_pessoa_escola = 0;
                }
                // Pega os parâmetros do usuário para criar a url do relatório:
                parametros = "@cdEscola=" + cd_pessoa_escola + "&@pTipoRelatorio=" + estoqueUI.tipoRpt + "&@pDtaI=" + dataBase + "&@pNoRelatorio=" + "Relação de Inventário" +
                    "&@pTipoItem=" + estoqueUI.dc_item;
                    
                string parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, parametrosCript);

                configureHeaderResponse(response, null);
                return response;

            }
            catch (FinanceiroBusinessException exe)
            {
                if (exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_USUARIO_MASTER)
                    return gerarLogException(exe.Message, retorno, logger, exe);
                else
                    return gerarLogException(Messages.msgRegBuscError, retorno, logger, exe);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "coco")]
        [HttpComponentesAuthorize(Roles = "locmv")]
        [HttpComponentesAuthorize(Roles = "tpliq")]
        [HttpGet]
        public HttpResponseMessage getUrlRelatorioSaldoFinanceiro(string dta_base, int tipo, bool tipoLiquidacao)
        {
            ReturnResult retorno = new ReturnResult();

            DateTime dataBase = new DateTime();

            try
            {
                int cd_pessoa_escola = this.ComponentesUser.CodEmpresa.Value;

                if (String.IsNullOrEmpty(dta_base))
                    dataBase = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

                if (!String.IsNullOrEmpty(dta_base))
                    dataBase = Convert.ToDateTime(dta_base);

                // Pega os parâmetros do usuário para criar a url do relatório:
                string parametros = "@cdEscola=" + cd_pessoa_escola + "&@DataBase=" + dataBase + "&@tipo=" + tipo + "&@tipoLiquidacao=" + tipoLiquidacao;

                var parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, parametrosCript);

                configureHeaderResponse(response, null);
                return response;

            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }
        
        [HttpComponentesAuthorize(Roles = "tpliq")]
        [HttpComponentesAuthorize(Roles = "tpfin")]
        [HttpGet]
        public HttpResponseMessage getTipoLiquidacaoFinanceiro()
        {
            try
            {
                RptRecebidaPaga rptRecebidaPaga = new RptRecebidaPaga();

                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                rptRecebidaPaga.tiposLiquidacao = Business.getTipoLiquidacao(TipoLiquidacaoDataAccess.TipoConsultaTipoLiquidacaoEnum.HAS_REL_PAGAR_RECEBER, null).ToList();;
                rptRecebidaPaga.tiposFinanceiro = Business.getTipoFinanceiro(0, TipoFinanceiroDataAccess.TipoConsultaTipoFinanEnum.HAS_ATIVO).ToList();

                var retorno = rptRecebidaPaga;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "rptalrest")]
        [HttpGet]
        public HttpResponseMessage getUrlRelatorioAlunoRestricao(int cdOrgao, byte tipodata, string dtInicial, string dtFinal)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                string parametros = "";

                int cd_pessoa_escola = this.ComponentesUser.CodEmpresa.Value;
                // Pega os parâmetros do usuário para criar a url do relatório:
                parametros = "@cdEscola=" + cd_pessoa_escola + "&@pOrgao=" + cdOrgao +  "&@ptipodata=" + tipodata + "&@pDtaI=" + dtInicial + "&@pDtaF=" + dtFinal;

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

        [HttpComponentesAuthorize(Roles = "orgfin")]
        [HttpGet]
        public HttpResponseMessage getAllOrgaoFinanceiro()
        {
            try
            {
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();

                var retorno = Business.getAllOrgaoFinanceiro().ToList();

                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }
        #endregion

        #region TipoEGrupo
        [HttpComponentesAuthorize(Roles = "gest")]
        public HttpResponseMessage getTipoGrupoItem()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                ItemUI item = new ItemUI();
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();

                item.tipos = Business.getAllTipoItem(null).ToList();
                item.grupos = Business.findAllGrupoAtivo(0, true);
                retorno.retorno = item;
                if (item.grupos != null || item.tipos != null)
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

        #region Fechamento
        [HttpComponentesAuthorize(Roles = "feces")]
        public HttpResponseMessage getFechamentoSearch(int? ano, int? mes, bool balanco, string dta_ini, string dta_fim)
        {
            try
            {
                int cd_escola = ComponentesUser.CodEmpresa.Value;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                DateTime dtaInicial = DateTime.MinValue;
                DateTime dtaFinal = DateTime.MaxValue;

                if (!String.IsNullOrEmpty(dta_ini))
                    dtaInicial = Convert.ToDateTime(dta_ini);

                if (!String.IsNullOrEmpty(dta_fim))
                    dtaFinal = Convert.ToDateTime(dta_fim);

                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var retorno = Business.getFechamentoSearch(parametros, ano, mes, balanco, dtaInicial, dtaFinal, cd_escola);

                if (retorno != null)
                {
                    foreach (var item in retorno)
                    {
                        item.dh_fechamento = SGF.Utils.ConversorUTC.ToLocalTime(item.dh_fechamento, this.ComponentesUser.IdFusoHorario, this.ComponentesUser.IsHorarioVerao);
                    }
                }

                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }
        [HttpComponentesAuthorize(Roles = "feces")]
        public HttpResponseMessage getUrlRelatorioFechamento(string sort, int direction, int? ano, int? mes, bool balanco, string dta_ini, string dta_fim)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cd_escola = ComponentesUser.CodEmpresa.Value;

                // Pega os parâmetros do usuário para criar a url do relatório:
                string strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@ano=" + ano + "&@mes=" + mes + "&@balanco=" + balanco + "&@dta_ini=" + dta_ini + "&@dta_fim=" + dta_fim +
                                    "&@cd_escola=" + cd_escola +
                                    "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=Relatório de Fechamento Estoque&" +
                                    Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.FechamentoEstoque;

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
        [HttpComponentesAuthorize(Roles = "feces")]
        [HttpComponentesAuthorize(Roles = "item")]
        [HttpComponentesAuthorize(Roles = "gest")]
        public HttpResponseMessage getFechamentoById(int cd_fechamento)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cd_escola = ComponentesUser.CodEmpresa.Value;
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var ret = Business.getFechamentoById(cd_fechamento, cd_escola);

                retorno.retorno = ret;
                if (ret.cd_fechamento <= 0)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));

            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "feces")]
        public HttpResponseMessage postGerarEstoque(Fechamento fechamento)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });
                int cd_escola = ComponentesUser.CodEmpresa.Value;
                retorno.retorno = Business.postGerarEstoque(fechamento, cd_escola);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (FechamentoBusinessException ex)
            {
                return gerarLogException(ex.Message, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "feces.i")]
        public HttpResponseMessage postFechamento(Fechamento fechamento)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });

                fechamento.cd_pessoa_empresa = (int)this.ComponentesUser.CodEmpresa;
                fechamento.cd_usuario = this.ComponentesUser.CodUsuario;
                fechamento.dh_fechamento = DateTime.UtcNow;
                fechamento.dt_fechamento = fechamento.dt_fechamento.Date;
                fechamento.nm_ano_fechamento = (short)fechamento.dt_fechamento.Year;
                    //(short)System.Data.Entity.SqlServer.SqlFunctions.DatePart("yyyy", fechamento.dt_fechamento);
                fechamento.nm_mes_fechamento = (byte)fechamento.dt_fechamento.Month;
                    //(byte)System.Data.Entity.SqlServer.SqlFunctions.DatePart("mm", fechamento.dt_fechamento);
                
                var fechamentoRet = Business.postFechamento(fechamento);
                retorno.retorno = fechamentoRet;
                if (fechamentoRet.cd_fechamento <= 0)
                    retorno.AddMensagem(Messages.msgNotIncludReg, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (FechamentoBusinessException ex)
            {
                return gerarLogException(ex.Message, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "fech")]
        public HttpResponseMessage postObsSaldoCaixaUsuario(ObsSaldoCaixa obsSaldoCaixa) 
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var cdEscola = this.ComponentesUser.CodEmpresa.Value;

                var obs = Business.postObsSaldoCaixaUsuario(obsSaldoCaixa, cdEscola);
                retorno.retorno = obs;
                if (obs != null && obs.cd_obs_saldo_caixa <= 0)
                    retorno.AddMensagem(Messages.msgNotIncludReg, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (FechamentoBusinessException ex)
            {
                return gerarLogException(ex.Message, retorno, logger, ex);
            }
            catch (Exception ex) 
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "fech")]
        public HttpResponseMessage getObsSaldoCaixaConsolidado(string dt_saldo_caixa, int cd_usuario) 
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var cdEscola = this.ComponentesUser.CodEmpresa.Value;
                DateTime dtFechamento = string.IsNullOrEmpty(dt_saldo_caixa) ? new DateTime() : (DateTime)Convert.ToDateTime(dt_saldo_caixa);
                var obs = Business.getObsSaldoCaixaConsolidado(cdEscola, dtFechamento, cd_usuario);
                retorno.retorno = obs;
                if (obs != null && obs.cd_obs_saldo_caixa <= 0)
                    retorno.AddMensagem(Messages.msgNotIncludReg, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (FechamentoBusinessException ex)
            {
                return gerarLogException(ex.Message, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "item")]
        public HttpResponseMessage getTipoItemMovimentaEstoque()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                IEnumerable<TipoItem> tiposItem = Business.getTipoItemMovimentoEstoque().ToList();
                retorno.retorno = tiposItem;
                if (tiposItem.Count() <= 0)
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
        [HttpComponentesAuthorize(Roles = "feces.a")]
        public HttpResponseMessage postAlterarFechamento(Fechamento fechamento)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });

                fechamento.cd_pessoa_empresa = (int)this.ComponentesUser.CodEmpresa;
                fechamento.dh_fechamento = DateTime.UtcNow;
                fechamento.nm_ano_fechamento = (short)fechamento.dt_fechamento.Year;
                fechamento.nm_mes_fechamento = (byte)fechamento.dt_fechamento.Month;
                fechamento.dt_fechamento = fechamento.dt_fechamento.Date;
                var fechamentoRet = Business.postAlterarFechamento(fechamento);
                retorno.retorno = fechamentoRet;
                if (fechamentoRet.cd_fechamento <= 0)
                    retorno.AddMensagem(Messages.msgNotUpReg, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (FechamentoBusinessException ex)
            {
                return gerarLogException(ex.Message, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "feces")]
        [HttpComponentesAuthorize(Roles = "item")]
        public HttpResponseMessage getFechamentoAnoMes()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cd_escola = ComponentesUser.CodEmpresa.Value;
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var ret = Business.fechamentoAnoMes(cd_escola);
                retorno.retorno = ret;
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "feces")]
        [HttpComponentesAuthorize(Roles = "item")]
        public HttpResponseMessage postSaldoItemLocal(SaldoItem saldos)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var ret = Business.getSaldoItemLocal(saldos);
                retorno.retorno = ret;
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "feces.e")]
        [HttpComponentesAuthorize(Roles = "item")]
        public HttpResponseMessage postDeleteFechamentos(List<Fechamento> fechamentos)
        {
            ReturnResult retorno = new ReturnResult();
            var cd_escola = (int)this.ComponentesUser.CodEmpresa;
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });
                List<int> cdFechamentos = fechamentos.Select(x => x.cd_fechamento).ToList();
                var delFech = Business.deleteFechamentos(cdFechamentos, cd_escola);
                retorno.retorno = delFech;
                if (!delFech)
                    retorno.AddMensagem(Messages.msgNotDeletedReg, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                else
                    retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (FechamentoBusinessException exe)
            {
                if (exe.tipoErro == FechamentoBusinessException.TipoErro.ERRO_FECHAMENTO_SUPERIOR || exe.tipoErro == FechamentoBusinessException.TipoErro.ERRO_FECHAMENTO_EXISTENTE)
                    return gerarLogException(exe.Message, retorno, logger, exe);
                else
                    return gerarLogException(Messages.msgNotUpReg, retorno, logger, exe);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotDeletedReg, retorno, logger, ex);
            }
        }


        #endregion

        #region Subgrupo Item

        [HttpComponentesAuthorize(Roles = "item")]
        public HttpResponseMessage getSubGrupoTpHasItem(int cd_item)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var ret = Business.getSubGrupoTpHasItem(cd_item);

                retorno.retorno = ret;
                if (ret.Count() <= 0)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));

            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, retorno, logger, ex);
            }
        }

        #endregion

        #region Tipo Nota Fiscal
        [HttpGet]
        [HttpComponentesAuthorize(Roles = "tpNF")]
        public HttpResponseMessage getTipoNotaFiscalSearch(string desc, string natOp, bool inicio, int status, int movimento, bool? devolucao, bool escola, byte id_regime_trib, bool? id_servico)
        {
            try
            {
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                int cd_escola = escola ? ComponentesUser.CodEmpresa.Value : 0;
                IFiscalBusiness Business = (IFiscalBusiness)base.instanciarBusiness<IFiscalBusiness>();
                var retorno = Business.getTipoNotaFiscalSearch(parametros, desc, natOp, inicio, getStatus(status), movimento, devolucao, cd_escola, id_regime_trib, id_servico);
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
        [HttpComponentesAuthorize(Roles = "tpNF")]
        public HttpResponseMessage getUrlRelatorioTipoNotaFiscal(string sort, int direction, string desc, string natOp, bool inicio, int status, bool? devolucao, bool escola, byte id_regime_trib)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                int cd_escola = escola ? ComponentesUser.CodEmpresa.Value : 0;
                string strParametrosSort = "@orientation=LANDSCAPE&@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@desc=" + desc + "&@natOp=" + natOp + "&@inicio=" + inicio + "&@status=" + status + "&@devolucao=" + devolucao  + "&@cd_escola=" + cd_escola + "&@id_regime_trib=" + id_regime_trib +"&" +
                                    Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=" + "Relatório de Tipo de Nota Fiscal&" +
                                    Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.TipoNotaFiscalSearch;
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

        // Delte api/<controller>
        [HttpComponentesAuthorize(Roles = "tpNF.e")]
        public HttpResponseMessage postDeleteTpNF(List<TipoNotaFiscal> tipos)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFiscalBusiness FiscalBusiness = (IFiscalBusiness)base.instanciarBusiness<IFiscalBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { FiscalBusiness });
                var deleted = FiscalBusiness.deleteAllTpNF(tipos);
                retorno.retorno = deleted;
                if (!deleted)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (FinanceiroBusinessException exe)
            {
                if (exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_TIPO_NF_UTILIZADO)
                    return gerarLogException(exe.Message, retorno, logger, exe);

                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, exe);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotDeletedReg, retorno, logger, ex);
            }

        }
        #endregion 

        #region Situação Tributaria

        [HttpGet]
        public HttpResponseMessage getSituacaoTributaria(byte id_regime_trib)
        {
            try
            {
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IEmpresaBusiness BusinessEmpresa = (IEmpresaBusiness)base.instanciarBusiness<IEmpresaBusiness>();
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                bool masterGeral = BusinessEmpresa.VerificarMasterGeral(this.ComponentesUser.CodUsuario);
                int cd_escola = ComponentesUser.CodEmpresa.Value;
                var retorno = Business.getSituacaoTributariaTipo(SituacaoTributariaDataAccess.TipoConsultaSitTribEnum.HAS_ATIVO, new List<int>(),
                    (int)SituacaoTributaria.TipoImpostoEnum.ICMS, cd_escola, id_regime_trib, masterGeral);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }
        #endregion 

        #region Aliquota NF
        [HttpGet]
        [HttpComponentesAuthorize(Roles = "icmsE")]
        public HttpResponseMessage getAliquotaUFSearch(int cdEstadoOri, int cdEstadoDest, double? aliquota)
        {
            try
            {
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var retorno = Business.getAliquotaUFSearch(parametros, cdEstadoOri, cdEstadoDest, aliquota);
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
        [HttpComponentesAuthorize(Roles = "icmsE")]
        public HttpResponseMessage getEstadosPesq()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var ret = Business.getEstadosPesq();
                retorno.retorno = ret;
                if (ret == null)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }
        //Método responsável pela emissão do relatório:
        [HttpComponentesAuthorize(Roles = "icmsE")]
        public HttpResponseMessage getUrlRelatorioAliquotaUF(string sort, int direction, int cdEstadoOri, int cdEstadoDest, double? aliquota)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                string strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@cdEstadoOri=" + cdEstadoOri + "&@cdEstadoDest=" + cdEstadoDest + "&@aliquota=" + aliquota + "&" +
                                    Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=" + "Relatório de ICMS por Estado&" +
                                    Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.AliquotaUFSearch;
                //parametros = HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8
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

        [HttpComponentesAuthorize(Roles = "icmsE.a")]
        public HttpResponseMessage postAlterarAliquotaUF(AliquotaUF aliquota)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFiscalBusiness businessFiscal = (IFiscalBusiness)base.instanciarBusiness<IFiscalBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { businessFiscal });
                var aliquotaRet = businessFiscal.putAliquotaUF(aliquota);
                retorno.retorno = aliquotaRet;
                if (aliquotaRet.cd_localidade_estado_destino <= 0 && aliquotaRet.cd_localidade_estado_origem <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (FinanceiroBusinessException exe)
            {
                if (exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_ICMS_ESTADO_UTILIZADO)
                    return gerarLogException(exe.Message, retorno, logger, exe);

                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, exe);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
            }

        }
        // Post api/<controller>/5
        [HttpComponentesAuthorize(Roles = "icmsE.i")]
        public HttpResponseMessage postAliquotaUF(AliquotaUF aliquota)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });
                var aliquotaRet = Business.postAliquotaUF(aliquota);
                retorno.retorno = aliquotaRet;
                if (aliquotaRet.cd_localidade_estado_destino <= 0 && aliquotaRet.cd_localidade_estado_origem <= 0)
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
        [HttpComponentesAuthorize(Roles = "icmsE.e")]
        public HttpResponseMessage postDeleteAliquotaUF(List<AliquotaUF> aliquotas)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFiscalBusiness businessFiscal = (IFiscalBusiness)base.instanciarBusiness<IFiscalBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { businessFiscal });
                var deleted = businessFiscal.deleteAllAliquotaUF(aliquotas);
                retorno.retorno = deleted;
                if (!deleted)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (FinanceiroBusinessException exe)
            {
                if (exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_ICMS_ESTADO_UTILIZADO)
                    return gerarLogException(exe.Message, retorno, logger, exe);

                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, exe);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotDeletedReg, retorno, logger, ex);
            }

        }
        #endregion 

        #region Dados NF
        [HttpGet]
        [HttpComponentesAuthorize(Roles = "dadNF")]
        public HttpResponseMessage getDadosNFSearch(int cdCidade, string natOp, double? aliquota, byte id_regime)
        {
            try
            {
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var retorno = Business.getDadosNFSearch(parametros, cdCidade, natOp, aliquota, id_regime);
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
        [HttpComponentesAuthorize(Roles = "dadNF")]
        public HttpResponseMessage getUrlRelatorioDadosNF(string sort, int direction, int cdCidade, string natOp, double? aliquota, byte id_regime)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                string strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@cdCidade=" + cdCidade + "&@natOp=" + natOp + "&@aliquota=" + aliquota + "&@id_regime=" + id_regime + "&" +
                                    Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=" + "Relatório de ICMS por Estado&" +
                                    Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.DadosNFSearch;
                //parametros = HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8
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

        // Delte api/<controller>
        [HttpComponentesAuthorize(Roles = "dadNF.e")]
        public HttpResponseMessage postDeleteDadosNF(List<DadosNF> dados)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFiscalBusiness businessFiscal = (IFiscalBusiness)base.instanciarBusiness<IFiscalBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { businessFiscal });
                var deleted = businessFiscal.deleteAllDadosNF(dados);
                retorno.retorno = deleted;
                if (!deleted)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }

            catch (FinanceiroBusinessException exe)
            {
                if (exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_DADOS_UF_UTILIZADO)
                    return gerarLogException(exe.Message, retorno, logger, exe);

                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, exe);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotDeletedReg, retorno, logger, ex);
            }

        }

        [HttpComponentesAuthorize(Roles = "dadNF")]
        public HttpResponseMessage getISSEscola()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cd_escola = (int)this.ComponentesUser.CodEmpresa;
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                double? alISS = Business.getISSEscola(cd_escola);
                retorno.retorno = alISS;
                if (alISS != null)
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

        [HttpComponentesAuthorize(Roles = "icmsE")]
        public HttpResponseMessage getTributacaoNFProduto(int cd_pessoa_movimento)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cd_escola = (int)this.ComponentesUser.CodEmpresa;
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                AliquotaUF aliqUF = Business.getAliquotaUFPorEstadoPessoa(cd_escola, cd_pessoa_movimento);
                retorno.retorno = aliqUF;
                if (aliqUF == null || aliqUF.cd_aliquota_uf <= 0)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "mvtc,mvtp,mvts")]
        public HttpResponseMessage getSituacaoTributariaItem(int cd_grupo_estoque, int id_regime_tributario, int cdSitTrib)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                SituacaoTributaria sitTrib = Business.getSituacaoTributariaItem(cd_grupo_estoque, id_regime_tributario, cdSitTrib);
                retorno.retorno = sitTrib;
                if (sitTrib == null || sitTrib.cd_situacao_tributaria <= 0)
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

        [HttpComponentesAuthorize(Roles = "mvtc,mvtp,mvts")]
        public HttpResponseMessage getSituacaoTributariaTpNF(int cdTpNF)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cd_escola = (int)this.ComponentesUser.CodEmpresa;
                Movimento movimento = new Movimento();

                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                List<SituacaoTributaria> situacoes = Business.getSituacaoTributaria(SituacaoTributariaDataAccess.TipoConsultaSitTribEnum.HAS_ATIVO, new List<int>(), cdTpNF).ToList();
                if (situacoes != null && situacoes.Count > 0)
                {
                    movimento.situacoesTributariaICMS = situacoes.Where(x => x.id_tipo_imposto == (int)SituacaoTributaria.TipoImpostoEnum.ICMS).OrderBy(a => a.cd_situacao_tributaria).ToList();
                    movimento.situacoesTributariaPIS = situacoes.Where(x => x.id_tipo_imposto == (int)SituacaoTributaria.TipoImpostoEnum.PIS).ToList();
                    movimento.situacoesTributariaCOFINS = situacoes.Where(x => x.id_tipo_imposto == (int)SituacaoTributaria.TipoImpostoEnum.CONFINS).ToList();
                }
                retorno.retorno = movimento;
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }
        
        #endregion 

        #region Reajuste Anual
        [HttpGet]
        [HttpComponentesAuthorize(Roles = "reaa")]
        public HttpResponseMessage getReajusteAnualSearch(int cd_usuario, int status, string dtInicial, string dtFinal, bool cadastro, bool vctoInicial, int cd_reajuste_anual)
        {
            try
            {
                DateTime? dtaInicial = dtInicial == null ? null : (DateTime?)Convert.ToDateTime(dtInicial);
                DateTime? dtaFinal = dtFinal == null ? null : (DateTime?)Convert.ToDateTime(dtFinal);
                
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                int cd_empresa = ComponentesUser.CodEmpresa.Value;
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                var retorno = Business.getReajusteAnualSearch(parametros, cd_empresa, cd_usuario, status, dtaInicial, dtaFinal, cadastro, vctoInicial, cd_reajuste_anual).ToList();
                if (retorno != null)
                {
                    foreach (var item in retorno)
                    {
                        item.dh_cadastro_reajuste = SGF.Utils.ConversorUTC.ToLocalTime(item.dh_cadastro_reajuste, this.ComponentesUser.IdFusoHorario, this.ComponentesUser.IsHorarioVerao);
                    }
                }
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "reaa.a")]
        public HttpResponseMessage postUpdateReajusteAnual(ReajusteAnual reajuste)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });
                reajuste.cd_pessoa_escola = (int)this.ComponentesUser.CodEmpresa;
                reajuste.cd_usuario = (int)this.ComponentesUser.CodUsuario;
                if (reajuste != null)
                {
                    string dataCad = reajuste.dh_cadastro_reajuste.Day + "/" + reajuste.dh_cadastro_reajuste.Month + "/" + reajuste.dh_cadastro_reajuste.Year + " " + reajuste.hr_cadastro;
                    reajuste.dh_cadastro_reajuste = Utils.Utils.truncarMilissegundo(SGF.Utils.ConversorUTC.ToUniversalTime(DateTime.Parse(dataCad), this.ComponentesUser.IdFusoHorario, this.ComponentesUser.IsHorarioVerao));
                }
                ReajusteAnual reajusteInserido = Business.postUpdateReajusteAnual(reajuste);
                reajusteInserido = Business.getReajusteAnualGridView(reajusteInserido.cd_reajuste_anual, (int)this.ComponentesUser.CodEmpresa);
                
                if (reajusteInserido != null)
                {
                    reajusteInserido.dh_cadastro_reajuste = SGF.Utils.ConversorUTC.ToLocalTime(reajusteInserido.dh_cadastro_reajuste, this.ComponentesUser.IdFusoHorario, this.ComponentesUser.IsHorarioVerao);
                }
                if (reajusteInserido.cd_reajuste_anual <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgUpdateSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                retorno.retorno = reajusteInserido;
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (FinanceiroBusinessException ex)
            {
                if (ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_SUM_DIFERENTE_POLCOM)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                if (ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_PARC_DIF_OBRIGATORIO)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                if (ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_DIA_POL_COMERCIAL_NULL)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                if (ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_REAJUSTE_ANUAL_FECHADO)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
        }
                
        [HttpComponentesAuthorize(Roles = "reaa.i")]
        public HttpResponseMessage postInsertReajusteAnual(ReajusteAnual reajuste)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });
                reajuste.dt_inicial_vencimento = reajuste.dt_inicial_vencimento.Date;
                if(reajuste.dt_final_vencimento.HasValue)
                    reajuste.dt_final_vencimento = reajuste.dt_final_vencimento.Value.Date;
               
                reajuste.cd_pessoa_escola = (int)this.ComponentesUser.CodEmpresa;
                reajuste.cd_usuario = (int)this.ComponentesUser.CodUsuario;
                reajuste.id_status_reajuste = (int)ReajusteAnual.StatusReajuste.ABERTO;
                if (reajuste != null)
                {
                    string dataCad = reajuste.dh_cadastro_reajuste.Day + "/" + reajuste.dh_cadastro_reajuste.Month + "/" + reajuste.dh_cadastro_reajuste.Year + " " + reajuste.hr_cadastro;
                    reajuste.dh_cadastro_reajuste = Utils.Utils.truncarMilissegundo(SGF.Utils.ConversorUTC.ToUniversalTime(DateTime.Parse(dataCad), this.ComponentesUser.IdFusoHorario, this.ComponentesUser.IsHorarioVerao));
                }
                ReajusteAnual reajusteInserido = Business.addReajusteAnual(reajuste);
                reajusteInserido = Business.getReajusteAnualGridView(reajusteInserido.cd_reajuste_anual, (int)this.ComponentesUser.CodEmpresa);
                if (reajusteInserido != null)
                {
                    reajusteInserido.dh_cadastro_reajuste = SGF.Utils.ConversorUTC.ToLocalTime(reajusteInserido.dh_cadastro_reajuste, this.ComponentesUser.IdFusoHorario, this.ComponentesUser.IsHorarioVerao);
                }
                retorno.retorno = reajusteInserido;
                if (reajusteInserido.cd_reajuste_anual <= 0)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgIncludSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (FinanceiroBusinessException ex)
            {
                if (ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_SUM_DIFERENTE_POLCOM)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                if (ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_PARC_DIF_OBRIGATORIO)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                if (ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_DIA_POL_COMERCIAL_NULL)
                    return gerarLogException(ex.Message, retorno, logger, ex);
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotIncludReg, retorno, logger, ex);
            }
        }

        // Delte api/<controller>
        [HttpComponentesAuthorize(Roles = "reaa.e")]
        public HttpResponseMessage postDeleteReajusteAnual(List<ReajusteAnual> reajustes)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IFinanceiroBusiness Business = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                IEmpresaBusiness BusinessEmpresa = (IEmpresaBusiness)base.instanciarBusiness<IEmpresaBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { Business });
                var deleted = Business.deleteAllReajuste(reajustes, (int)this.ComponentesUser.CodEmpresa);
                retorno.retorno = deleted;
                if (!deleted)
                    retorno.AddMensagem(Messages.msgRegBuscError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                else
                    retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (FinanceiroBusinessException ex)
            {
                if (ex.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_REAJUSTE_ANUAL_FECHADO)
                    return gerarLogException(ex.Message, retorno, logger, ex);

                return gerarLogException(Messages.msgNotDeletedReg, retorno, logger, ex);
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgNotDeletedReg, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "reaa")]
        public HttpResponseMessage getUrlRelatorioReajusteAnual(string sort, int direction, int cd_usuario, int status, string dtInicial, string dtFinal, bool cadastro, bool vctoInicial, int cd_reajuste_anual)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                // Pega os parâmetros do usuário para criar a url do relatório:
                int cd_empresa = ComponentesUser.CodEmpresa.Value;
                string strParametrosSort = "@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@cd_empresa=" + cd_empresa + "&@cd_usuario=" + cd_usuario + "&@id_status=" + status + "&@dt_inicial=" + dtInicial
                    + "&@dt_final=" + dtFinal + "&@id_cadastro=" + cadastro + "&@id_vcto_inicial=" + vctoInicial + "&@cd_reajuste_anual=" + cd_reajuste_anual
                    + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=" + "Relatório de Reajuste Anual&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.ReajusteAnualSearch;
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
    }
}
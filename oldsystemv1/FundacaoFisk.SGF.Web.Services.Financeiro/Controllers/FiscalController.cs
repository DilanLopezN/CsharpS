using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using Newtonsoft.Json;
using log4net;
using System.Web;
using FundacaoFisk.SGF.Utils.Messages;
using Componentes.GenericController;
using System.Globalization;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IBusiness;
using Componentes.GenericBusiness.Comum;
using FundacaoFisk.SGF.Web.Services.Financeiro.Business;
using FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess;
using System.Xml;
using System.Web.UI.WebControls;


namespace FundacaoFisk.SGF.Web.Services.Financeiro.Controllers
{
    public class FiscalController : ComponentesApiController
    {
        //Declaração de Atributos
        private static readonly ILog logger = LogManager.GetLogger(typeof(FiscalController));


        public FiscalController()
        {
        }

        #region Movimento
        [HttpComponentesAuthorize(Roles = "alu")]
        public HttpResponseMessage getItensAluno(int cd_pessoa, int cd_aluno)
        {
            try
            {
                var cd_escola = this.ComponentesUser.CodEmpresa.Value;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IFiscalBusiness businessFiscal = (IFiscalBusiness)base.instanciarBusiness<IFiscalBusiness>();
                IEnumerable<ItemMovimento> itens = businessFiscal.getItensByAluno(parametros, cd_pessoa, cd_aluno, cd_escola);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, itens);

                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "mvtc,mvtd,mvtp,mvts, mvtdv")]
        public HttpResponseMessage getMovimentoSearch(int id_tipo_movimento, int cd_pessoa, int cd_item, int cd_plano_conta, int numero, string serie, bool emissao, bool movimento, string dtInicial, string dtFinal, bool nota_fiscal, int statusNF, int isImportXML, bool? id_material_didatico, bool? id_venda_futura)
        {
            try
            {
                DateTime? dtaInicial = dtInicial == null ? null : (DateTime?)Convert.ToDateTime(dtInicial);
                DateTime? dtaFinal = dtFinal == null ? null : (DateTime?)Convert.ToDateTime(dtFinal);

                // Verifica permissão "Conta Segura" do usuário.
                bool contaSegura = this.ComponentesUser.Permissao.Contains("ctsg");

                int cd_escola = (int)this.ComponentesUser.CodEmpresa;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IFiscalBusiness businessFiscal = (IFiscalBusiness)base.instanciarBusiness<IFiscalBusiness>();
                var retorno = businessFiscal.searchMovimento(parametros, id_tipo_movimento, cd_pessoa, cd_item, cd_plano_conta, numero, serie, cd_escola, emissao, movimento, dtaInicial, dtaFinal, nota_fiscal, statusNF, contaSegura, isImportXML, id_material_didatico, id_venda_futura);
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
        public HttpResponseMessage VerificaNFESemDataAutorizacao()
        {
            try
            {
                int cd_escola = (int)this.ComponentesUser.CodEmpresa;
                IFiscalBusiness businessFiscal = (IFiscalBusiness)base.instanciarBusiness<IFiscalBusiness>();
                var retorno = businessFiscal.VerificaNFESemDataAutorizacao((int)Movimento.TipoMovimentoEnum.SAIDA, cd_escola, true);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "mvtc,mvtd,mvtp,mvts, mvtdv")]
        public HttpResponseMessage getMovimentoSearchFK(int cd_pessoa, int cd_item, int cd_plano_conta, int numero, string serie,
                                                       bool emissao, bool movimento, string dtInicial, string dtFinal, int natMovto, bool idNf)
        {
            try
            {
                DateTime? dtaInicial = dtInicial == null ? null : (DateTime?)Convert.ToDateTime(dtInicial);
                DateTime? dtaFinal = dtFinal == null ? null : (DateTime?)Convert.ToDateTime(dtFinal);

                int cd_escola = (int)this.ComponentesUser.CodEmpresa;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IFiscalBusiness businessFiscal = (IFiscalBusiness)base.instanciarBusiness<IFiscalBusiness>();
                var retorno = businessFiscal.searchMovimentoFK(parametros, cd_pessoa, cd_item, cd_plano_conta, numero, serie, cd_escola, emissao, movimento, dtaInicial, dtaFinal, natMovto, idNf);
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
        [HttpComponentesAuthorize(Roles = "mvtc,mvtd,mvtp,mvts, mvtdv")]
        public HttpResponseMessage getMovimentoSearchFKPerdaMaterial(int cd_pessoa, int cd_item, int cd_plano_conta, int numero, string serie,
            bool emissao, bool movimento, string dtInicial, string dtFinal, int natMovto, bool idNf, int origem, int? nm_contrato, int? cd_aluno)
        {
            try
            {
                DateTime? dtaInicial = dtInicial == null ? null : (DateTime?)Convert.ToDateTime(dtInicial);
                DateTime? dtaFinal = dtFinal == null ? null : (DateTime?)Convert.ToDateTime(dtFinal);

                int cd_escola = (int)this.ComponentesUser.CodEmpresa;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IFiscalBusiness businessFiscal = (IFiscalBusiness)base.instanciarBusiness<IFiscalBusiness>();
                var retorno = businessFiscal.searchMovimentoFKPerdaMaterial(parametros, cd_pessoa, cd_item, cd_plano_conta, numero, serie, cd_escola, emissao, movimento, dtaInicial, dtaFinal, natMovto, idNf, origem, cd_aluno, nm_contrato);
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
        public HttpResponseMessage getItensMovimentoByCdMovimentoPerdaMaterial(int cd_movimento)
        {
            try
            {

                IFiscalBusiness businessFiscal = (IFiscalBusiness)base.instanciarBusiness<IFiscalBusiness>();
                var retorno = businessFiscal.getItensMovimentoByCdMovimentoPerdaMaterial(cd_movimento);
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
        [HttpComponentesAuthorize(Roles = "mvtc,mvtd,mvtp,mvts, mvtdv")]
        public HttpResponseMessage getMovimentoSearchFKVincularMaterial(int cd_pessoa, int cd_item, int cd_plano_conta, int numero, string serie,
            bool emissao, bool movimento, string dtInicial, string dtFinal, int natMovto, bool material_didatico_vincular_material, 
            bool nota_fiscal_vincular_material, int cd_curso )
        {
            try
            {
                DateTime? dtaInicial = dtInicial == null ? null : (DateTime?)Convert.ToDateTime(dtInicial);
                DateTime? dtaFinal = dtFinal == null ? null : (DateTime?)Convert.ToDateTime(dtFinal);

                int cd_escola = (int)this.ComponentesUser.CodEmpresa;
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IFiscalBusiness businessFiscal = (IFiscalBusiness)base.instanciarBusiness<IFiscalBusiness>();
                var retorno = businessFiscal.searchMovimentoFKVincularMaterial(parametros, cd_pessoa, cd_item, cd_plano_conta, numero, serie, cd_escola, emissao, movimento, dtaInicial, dtaFinal, natMovto, 
                    material_didatico_vincular_material, nota_fiscal_vincular_material, cd_curso);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                base.configureHeaderResponse(response, parametros);
                return response;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "mvtc,mvtd,mvtp,mvts, mvtdv")]
        public HttpResponseMessage GeturlrelatorioMovimento(string sort, int direction, int id_tipo_movimento, int cd_pessoa, int cd_item, int cd_plano_conta, int numero, string serie,
                                                       bool emissao, bool movimento, string dtInicial, string dtFinal, bool nota_fiscal, int statusNF, bool id_venda_futura)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                bool contaSegura = this.ComponentesUser.Permissao.Contains("ctsg");
                string nomeRelatorio = "";
                if (id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.ENTRADA)
                    nomeRelatorio = "Entradas";
                if (id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.SAIDA)
                    nomeRelatorio = "Saídas";
                if (id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.SERVICO)
                    nomeRelatorio = "Serviços";
                if (id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.DESPESA)
                    nomeRelatorio = "Despesas";
                //Pega os parâmetros do usuário para criar a url do relatório:
                string strParametrosSort = "@orientation=LANDSCAPE&@cs=" + sort + "&@so=" + direction + "&";
                string parametros = strParametrosSort + "@cdEmpresa=" + cdEscola + "&@tipoMovimento=" + id_tipo_movimento + "&@cdPessoa=" + cd_pessoa + "&@cdItem=" + cd_item
                    + "&@cdPlanoConta=" + cd_plano_conta + "&@numero=" + numero
                    + "&@contaSegura=" + contaSegura + "&@serie=" + serie + "&@emissao=" + emissao + "&@movimento=" + movimento + "&@dtInicial=" + dtInicial + "&@dtFinal=" + dtFinal + "&@notaFiscal=" + nota_fiscal + "&@statusNF=" + statusNF + "&@id_venda_futura=" + id_venda_futura
                    + "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO + "=Relatório de " + nomeRelatorio + "&" + Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO + "=" + FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.Movimento;
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

        [HttpComponentesAuthorize(Roles = "item")]
        public HttpResponseMessage getTipoItemMovimento(int id_tipo_finan)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IEnumerable<TipoItem> tiposItem = new List<TipoItem>();
                IFinanceiroBusiness businessFinan = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                switch (id_tipo_finan)
                {
                    case (int)Movimento.TipoMovimentoEnum.ENTRADA:
                        tiposItem = businessFinan.getTipoItemMovimento(TipoItemDataAccess.TipoConsultaTipoItemEnum.HAS_MOVIMENTO_ENTRADA).ToList();
                        break;
                    case (int)Movimento.TipoMovimentoEnum.SAIDA:
                        tiposItem = businessFinan.getTipoItemMovimento(TipoItemDataAccess.TipoConsultaTipoItemEnum.HAS_MOVIMENTO_SAIDA).ToList();
                        break;
                    case (int)Movimento.TipoMovimentoEnum.SERVICO:
                        tiposItem = businessFinan.getTipoItemMovimento(TipoItemDataAccess.TipoConsultaTipoItemEnum.HAS_MOVIMENTO_SERVICO).ToList();
                        break;
                    case (int)Movimento.TipoMovimentoEnum.DESPESA:
                        tiposItem = businessFinan.getTipoItemMovimento(TipoItemDataAccess.TipoConsultaTipoItemEnum.HAS_MOVIMENTO_DESPESAS).ToList();
                        break;
                }
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

        [HttpComponentesAuthorize(Roles = "tit.e")]
        [HttpComponentesAuthorize(Roles = "mvtc.e,mvtd.e,mvtp.e,mvts.e, mvtdv.e")]
        public HttpResponseMessage postDeleteMovimentos(List<Movimento> movimentos)
        {
            ReturnResult retorno = new ReturnResult();
            var cd_escola = (int)this.ComponentesUser.CodEmpresa;
            try
            {
                IFiscalBusiness businessFiscal = (IFiscalBusiness)base.instanciarBusiness<IFiscalBusiness>();
                configuraBusiness(new List<IGenericBusiness>() { businessFiscal });
                int[] cdMovimentos = null;
                int i;
                // Pegando códigos da Turma
                if (movimentos != null && movimentos.Count() > 0)
                {
                    i = 0;
                    int[] cdMovimentosCont = new int[movimentos.Count()];
                    foreach (var c in movimentos)
                    {
                        cdMovimentosCont[i] = c.cd_movimento;
                        i++;
                    }
                    cdMovimentos = cdMovimentosCont;
                }
                var delMovtos = businessFiscal.deleteMovimentos(cdMovimentos, cd_escola);
                retorno.retorno = delMovtos;
                if (!delMovtos)
                    retorno.AddMensagem(Messages.msgNotDeletedReg, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                else
                    retorno.AddMensagem(Messages.msgDeleteSucess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(retorno));
                configureHeaderResponse(response, null);
                return response;
            }
            catch (FinanceiroBusinessException exe)
            {
                if (exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_EXISTE_FECHAMENTO ||
                    exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_EXISTE_FECHAMENTO_BALANCO ||
                    exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_TITULO_ENVIADO_CNAB ||
                    exe.tipoErro == FinanceiroBusinessException.TipoErro.ERRO_NF_FECHADA_CANCELADA)
                    return gerarLogException(exe.Message, retorno, logger, exe);
                else
                    return gerarLogException(Messages.msgNotUpReg, retorno, logger, exe);
            }
            catch (Exception ex)
            {
                var message = Utils.Utils.innerMessage(ex);
                if (message != "")
                {
                    FinanceiroBusinessException fx = new FinanceiroBusinessException(message, ex, 0, false);
                    return gerarLogException(message, retorno, logger, fx);
                }
                else
                {
                    return gerarLogException(Messages.msgNotUpReg, retorno, logger, ex);
                }

            }
        }

        [HttpComponentesAuthorize(Roles = "mvtc.a,mvtd.a,mvtp.a,mvts.a, mvtdv.a")]
        public HttpResponseMessage getComponentesByMovimentoEdit(int cd_movimento,bool id_nf, int id_tipo_movimento)
        {
            ReturnResult retorno = new ReturnResult();
            int cdEscola = (int)this.ComponentesUser.CodEmpresa;
            IFiscalBusiness businessFiscal = (IFiscalBusiness)base.instanciarBusiness<IFiscalBusiness>();
            IFinanceiroBusiness businessFinan = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
            try
            {
                Movimento movimento = new Movimento();
                if (id_nf || id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.DEVOLUCAO)
                {
                    movimento = businessFiscal.getMovimentoEditViewNF(cd_movimento, cdEscola, id_tipo_movimento);
                    if (movimento != null && movimento.cd_movimento > 0 && movimento.cd_tipo_nota_fiscal != null)
                    {
                        if (id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.SAIDA ||
                        id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.ENTRADA ||
                       id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.SERVICO ||
                        id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.DEVOLUCAO)
                        {
                            List<SituacaoTributaria> situacoes = businessFinan.getSituacaoTributaria(SituacaoTributariaDataAccess.TipoConsultaSitTribEnum.HAS_ATIVO, new List<int>(), movimento.cd_tipo_nota_fiscal.Value).ToList();
                            if (situacoes != null && situacoes.Count > 0)
                            {
                                movimento.situacoesTributariaICMS = situacoes.Where(x => x.id_tipo_imposto == (int)SituacaoTributaria.TipoImpostoEnum.ICMS).OrderBy(a => a.cd_situacao_tributaria).ToList();
                                movimento.situacoesTributariaPIS = situacoes.Where(x => x.id_tipo_imposto == (int)SituacaoTributaria.TipoImpostoEnum.PIS).ToList();
                                movimento.situacoesTributariaCOFINS = situacoes.Where(x => x.id_tipo_imposto == (int)SituacaoTributaria.TipoImpostoEnum.CONFINS).ToList();
                            }
                        }
                    }
                }
                else
                    movimento = businessFiscal.getMovimentoEditView(cd_movimento, cdEscola);
                if (movimento != null && movimento.cd_movimento > 0)
                {
                    if (movimento.cd_tipo_financeiro > 0)
                        movimento.tiposFinan = businessFinan.getTipoFinanceiro(movimento.cd_tipo_financeiro, TipoFinanceiroDataAccess.TipoConsultaTipoFinanEnum.HAS_ATIVO).ToList();
                    movimento.bancosCheque = businessFinan.getAllBanco().ToList();
                }
                if (movimento == null || movimento.cd_movimento <= 0)
                    throw new FiscalBusinessException(string.Format(Utils.Messages.Messages.msgRegNotEnc), null,
                                                                   FiscalBusinessException.TipoErro.ERRO_REGISTRO_NAO_ENCONTRADO, false);

                if (movimento.id_material_didatico == true && movimento.cd_aluno != null && movimento.cd_aluno > 0)
                {
                    movimento.contratos_combo_material_didatico = businessFiscal.getContratosSemTurmaByAlunoMovimentoSearch((int)movimento.cd_aluno, true, 1, 0, 0, cdEscola, 4, true);
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

        [HttpComponentesAuthorize(Roles = "mvtc.a,mvtd.a,mvtp.a,mvts.a, mvtdv.a")]
        public HttpResponseMessage getComponentesByItemMovimentoEdit(int id_tipo_movimento, int cd_sit_ICMS, int cd_sit_PIS, int cd_sit_COFINS, int cdTpNF)
        {
            ReturnResult retorno = new ReturnResult();
            int cdEscola = (int)this.ComponentesUser.CodEmpresa;
            try
            {
                IFinanceiroBusiness businessFinan = (IFinanceiroBusiness)base.instanciarBusiness<IFinanceiroBusiness>();
                Movimento movimento = new Movimento();
                if (id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.SAIDA ||
                   id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.SERVICO ||
                   id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.ENTRADA ||
                   id_tipo_movimento == (int)Movimento.TipoMovimentoEnum.DEVOLUCAO)
                {
                    List<int> cd_situacoes = new List<int>();
                    if (cd_sit_ICMS > 0)
                        cd_situacoes.Add(cd_sit_ICMS);
                    if (cd_sit_PIS > 0)
                        cd_situacoes.Add(cd_sit_PIS);
                    if (cd_sit_COFINS > 0)
                        cd_situacoes.Add(cd_sit_COFINS);
                    List<SituacaoTributaria> situacoes = businessFinan.getSituacaoTributaria(SituacaoTributariaDataAccess.TipoConsultaSitTribEnum.HAS_ATIVO, cd_situacoes, cdTpNF).ToList();
                    if (situacoes != null && situacoes.Count > 0)
                    {
                        movimento.situacoesTributariaICMS = situacoes.Where(x => x.id_tipo_imposto == (int)SituacaoTributaria.TipoImpostoEnum.ICMS).OrderBy(a => a.cd_situacao_tributaria).ToList();
                        movimento.situacoesTributariaPIS = situacoes.Where(x => x.id_tipo_imposto == (int)SituacaoTributaria.TipoImpostoEnum.PIS).ToList();
                        movimento.situacoesTributariaCOFINS = situacoes.Where(x => x.id_tipo_imposto == (int)SituacaoTributaria.TipoImpostoEnum.CONFINS).ToList();
                    }
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

        //Método responsável pela emissão do relatório:
        [HttpComponentesAuthorize(Roles = "mvtc,mvtd,mvtp,mvts, mvtdv")]
        public HttpResponseMessage getUrlRelatorioEspelho(int cd_movimento)
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                int pEscola = (int)this.ComponentesUser.CodEmpresa;
                // Pega os parâmetros do usuário para criar a url do relatório:
                string parametros = "@pEscola=" + pEscola + "&@cd_movimento=" + cd_movimento;
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
        public HttpResponseMessage getItensMvto(int cd_movimento)
        {
            try
            {
                int cdEscola = (int)this.ComponentesUser.CodEmpresa;
                IFiscalBusiness businessFiscal = (IFiscalBusiness)base.instanciarBusiness<IFiscalBusiness>();
                IEnumerable<ItemMovimento> retorno = businessFiscal.getItensMvto(cd_movimento, cdEscola);
                HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK, retorno);
                return response;

            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "mvtc,mvtd,mvtp,mvts, mvtdv")]
        public HttpResponseMessage getRetMovimentoDevolucao(int cd_movimento, int id_tipo_movimento, bool isMaster)
        {
            ReturnResult retorno = new ReturnResult();
            int cdEscola = (int)this.ComponentesUser.CodEmpresa;

            try
            {
                IFiscalBusiness businessFiscal = (IFiscalBusiness)base.instanciarBusiness<IFiscalBusiness>();
                Movimento movimento = movimento = businessFiscal.getRetMovimentoDevolucao(cd_movimento, cdEscola, id_tipo_movimento, isMaster);
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

        #region CFOP

        [HttpGet]
        [HttpComponentesAuthorize(Roles = "mvtp")]
        public HttpResponseMessage searchCFOP(string descricao, bool inicio, int nm_CFOP, byte id_natureza_CFOP)
        {
            try
            {
                var parametros = new SearchParameters(this.Request.Headers.Range, this.Request.RequestUri.ParseQueryString());
                IFiscalBusiness businessFiscal = (IFiscalBusiness)base.instanciarBusiness<IFiscalBusiness>();
                var retorno = businessFiscal.searchCFOP(parametros, descricao, inicio, nm_CFOP, id_natureza_CFOP);
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

    }
}

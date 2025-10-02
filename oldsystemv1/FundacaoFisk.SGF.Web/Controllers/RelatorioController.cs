using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;
using Componentes.GenericController;
using Componentes.Utils;
using Componentes.Utils.Messages;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Secretaria.Business;
using log4net;
using Newtonsoft.Json;
using System.Text;
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;
using System.Web.Http;

namespace FundacaoFisk.SGF.Web.Controllers
{
    public class RelatorioController : ComponentesMVCController
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(RelatorioController));
        
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult RelatorioAtividadeExtra() { return View(); }
        public ActionResult RelatorioAulaReposicaoView() { return View(); }
        public ActionResult RelatorioAulaReposicao() { return View(); }
        public ActionResult RelatorioAlunoCliente() { return View(); }
        public ActionResult RelatorioHistoricoAluno() { return View(); }
        public ActionResult RelatorioControleFaltas() { return View(); }
        public ActionResult RelatorioAulaPersonalizada() { return View(); }
        public ActionResult RelatorioBalanceteMensal() { return View(); }
        public ActionResult RelatorioBolsistas() { return View(); }
        public ActionResult RelatorioCarne() { return View(); }
        public ActionResult RelatorioCarneMovto() { return View(); }
        public ActionResult RelatorioCheques() { return View(); }
        public ActionResult RelatorioComissaoSecretarias() { return View(); }
        public ActionResult RelatorioContaCorrente() { return View(); }
        public ActionResult RelatorioControleSala() { return View(); }
        public ActionResult RelatorioContVendasMaterial() { return View(); }
        public ActionResult RelatorioAlunosSemTituloGerado() { return View(); }
        public ActionResult RelatorioCopiaEspelhoMovimento() { return View(); }
        public ActionResult RelatorioFaixaEtaria() { return View(); }
        public ActionResult RelatorioFollowUp() { return View(); }
        public ActionResult RelatorioInventario() { return View(); }
        public ActionResult RelatorioAlunoRestricao() { return View(); }
        public ActionResult RelatorioAvaliacao() { return View(); }

        public ActionResult RelatorioDinamico() {
            //Contem os parametros do request;
            //var aux = Request.QueryString;
            return View();
        }

        public ActionResult RelatorioEspelhoMovimento() { return View(); }
        public ActionResult RelatorioEstoque() { return View(); }
        public ActionResult RelatorioEvento() { return View(); }
        public ActionResult RelatorioFechamentoCaixaSint() { return View(); }
        public ActionResult RelatorioListagemAniversariantes() { return View(); }
        public ActionResult RelatorioListagemEnderecosMMK() { return View(); }
        public ActionResult RelatorioMatriculaAnalitico() { return View(); }
        public ActionResult RelatorioMediaAlunos() { return View(); }
        public ActionResult RelatorioPagamentoProfessores() { return View(); }
        public ActionResult RelatorioPercentualTerminoEstagio() { return View(); }
        public ActionResult RelatorioPosicaoFinanceira() { return View(); }
        public ActionResult RelatorioProgramacaoAulasTurma() { return View(); }
        public ActionResult RelatorioProspectAtendido() { return View(); }
        public ActionResult RelatorioRecibo() { return View(); }
        public ActionResult RelatorioReciboAgrupado() { return View(); }
        public ActionResult RelatorioReciboConfirmacao() { return View(); }
        public ActionResult RelatorioReciboMovimento() { return View(); }
        public ActionResult RelatorioReciboProspect() { return View(); }
        public ActionResult RelatorioRetornoCNAB() { return View(); }
        public ActionResult RelatorioRetornoTitulosCNAB() { return View(); }
        public ActionResult RelatorioSaldoFinanceiro() { return View(); }
        public ActionResult RelatorioTitulosCNAB() { return View(); }
        public ActionResult RelatorioTurma() { return View(); }
        public ActionResult RelatorioTurmaMatriculaMaterial() { return View(); }
        public ActionResult RelatorioMatriculaOutros() { return View(); }
        public ActionResult RelatorioLoginEscola() { return View(); }
        public ActionResult ReportAlunoCliente() { return View(); }
        public ActionResult ReportAulaPersonalizada() { return View(); }
        public ActionResult ReportAtividadeExtra() { return View(); }
        public ActionResult ReportGestaoAtividadeExtra() { return View(); }
        public ActionResult ReportFaixaEtaria() { return View(); }
        public ActionResult ReportHistorioAluno() { return View(); }
        public ActionResult ReportControleFaltas() { return View(); }
        public ActionResult ReportBalanceteMensal() { return View(); }
        public ActionResult ReportBolsistas() { return View(); }
        public ActionResult ReportCheques() { return View(); }
        public ActionResult ReportComissaoSecretaria() { return View(); }
        public ActionResult ReportContaCorrente() { return View(); }
        public ActionResult ReportControleSala() { return View(); }
        public ActionResult ReportControleVendasMaterial() { return View(); }
        public ActionResult ReportAlunosSemTituloGerado() { return View(); }
        public ActionResult ReportDiarioAula() { return View(); }
        public ActionResult ReportEstoque() { return View(); }
        public ActionResult ReportListagemAniversariantes() { return View(); }
        public ActionResult ReportMatricula() { return View(); }
        public ActionResult ReportMediaAlunos() { return View(); }
        public ActionResult ReportPagamentoProfessores() { return View(); }
        public ActionResult ReportPercentualTerminoEstagio() { return View(); }
        public ActionResult ReportPosicaoFinanceira() { return View(); }
        public ActionResult ReportProgramacaoAulasTurma() { return View(); }
        public ActionResult ReportProspect() { return View(); }
        public ActionResult ReportSaldoFinanceiro() { return View(); }
        public ActionResult ReportSaldoFinanceiroReportProspect() { return View(); }
        public ActionResult ReportTurma() { return View(); }
        public ActionResult ReportTurmaMatriculaMaterial() { return View(); }
        public ActionResult ReportAulaReposicao() { return View(); }
        public ActionResult ReportInventario() { return View(); }
        public ActionResult ReportAlunoRestricao() { return View(); }
        public ActionResult ReportFollowUp() { return View(); }
        public ActionResult ReportAvaliacao() { return View(); }
        public ActionResult ReportMatriculaOutros() { return View(); }
        public ActionResult ReportCartaQuitacao() { return View(); }
        public ActionResult ReportLoginEscola() { return View(); }

        //public ActionResult RelatorioTurma()
        //{
        //    return View();
        //}

        [MvcComponentesAuthorize(Roles = "cnabb")]
        public ActionResult postGerarRemessa(string cd_cnab, int cd_carteira_cnab) {
            ReturnResult retorno = new ReturnResult();
            try {
                string enderecoRelatorioWeb = Session["enderecoRelatorioWeb"] + "";
                string enderecoRetorno = Session["enderecoWeb"] + "";
                int cd_escola = (int) Session["CodEscolaSelecionada"];

                var parms = "cd_cnab=" + cd_cnab + "&cd_escola=" + cd_escola + "&cd_carteira_cnab=" + cd_carteira_cnab +
                    "&" + Componentes.Utils.ReportParameter.PARAMETRO_DATA_HORA_ATUAL + "=" + DateTime.Now;
                string parametros = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(Componentes.Utils.MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parms, System.Text.Encoding.UTF8), Componentes.Utils.MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);
                parametros += "&enderecoWeb=" + Session["enderecoWeb"];
                return new RenderJsonActionResult { Result = enderecoRelatorioWeb + "RelatorioApresentacao/postGerarRemessa?" + parametros };
            }
            catch(Exception ex) {
                return gerarLogException(FundacaoFisk.SGF.Utils.Messages.Messages.msgArqContratoNaoEncontrado, retorno, logger, ex);
            }
        }

        [MvcComponentesAuthorize(Roles = "cnabb")]
        public ActionResult postGerarPedidoBaixa(string cd_cnab, int cd_carteira_cnab) {
            ReturnResult retorno = new ReturnResult();
            try {
                string enderecoRelatorioWeb = Session["enderecoRelatorioWeb"] + "";
                string enderecoRetorno = Session["enderecoWeb"] + "";
                int cd_escola = (int) Session["CodEscolaSelecionada"];

                var parms = "cd_cnab=" + cd_cnab + "&cd_escola=" + cd_escola + "&cd_carteira_cnab=" + cd_carteira_cnab +
                    "&" + Componentes.Utils.ReportParameter.PARAMETRO_DATA_HORA_ATUAL + "=" + DateTime.Now;
                string parametros = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(Componentes.Utils.MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parms, System.Text.Encoding.UTF8), Componentes.Utils.MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);
                parametros += "&enderecoWeb=" + Session["enderecoWeb"];
                return new RenderJsonActionResult { Result = enderecoRelatorioWeb + "RelatorioApresentacao/postGerarPedidoBaixa?" + parametros };
            }
            catch(Exception ex) {
                return gerarLogException(FundacaoFisk.SGF.Utils.Messages.Messages.msgArqContratoNaoEncontrado, retorno, logger, ex);
            }
        }

        [MvcComponentesAuthorize(Roles = "rtcnb")]
        public ActionResult postProcessarRetornos(RetornoCNAB retornoCnab) {
            ReturnResult retorno = new ReturnResult();
            try {
                string enderecoRelatorioWeb = Session["enderecoRelatorioWeb"] + "";
                string caminhoRelatorioWeb = "RelatorioApresentacao/getProcessarRetornos?";
                int cd_escola = (int) Session["CodEscolaSelecionada"];
                int cd_usuario = (int)Session["CodUsuario"];
                var parms = "cd_retorno_cnab=" + retornoCnab.cd_retorno_cnab + "&cd_escola=" + cd_escola + "&nro_banco=" + retornoCnab.LocalMovto.nm_banco + "&no_arquivo_retorno=" +
                            retornoCnab.no_arquivo_retorno + "&cd_local_movto=" + retornoCnab.LocalMovto.cd_local_movto + "&cd_usuario= " + cd_usuario +
                            "&" + Componentes.Utils.ReportParameter.PARAMETRO_DATA_HORA_ATUAL + "=" + DateTime.Now;
                string parametros = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(Componentes.Utils.MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parms, System.Text.Encoding.UTF8), Componentes.Utils.MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);

                //try {
                    //using(HttpClient client = new HttpClient()) {
                        enderecoRelatorioWeb = enderecoRelatorioWeb.Replace("www.sgf.datawin.com.br", "localhost");
                        retorno.retorno = enderecoRelatorioWeb + caminhoRelatorioWeb + parametros;
                        /*enderecoRelatorioWeb = enderecoRelatorioWeb.Replace("www.sgf.datawin.com.br", "localhost");
                        
                        client.BaseAddress = new Uri(enderecoRelatorioWeb); //Uri("http://localhost:28509/");
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        HttpResponseMessage response = client.GetAsync(caminhoRelatorioWeb + parametros).Result;

                        if(response.IsSuccessStatusCode)
                            retorno = (ReturnResult) JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result, typeof(ReturnResult));
                        else {
                            logger.Error(response.StatusCode + " Url: " + enderecoRelatorioWeb + caminhoRelatorioWeb + parametros);
                            retorno.AddMensagem(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroConexaoRelatorioBoleto, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                        }*/

                        return new RenderJsonActionResult { Result = retorno };
                    //}
                /*}
                catch(Exception ex) {
                    throw new Exception(" Url: " + enderecoRelatorioWeb + caminhoRelatorioWeb + parametros, ex);
                }*/
            }
            catch(Exception ex) {
                return gerarLogException(FundacaoFisk.SGF.Utils.Messages.Messages.msgNotProcessReg, retorno, logger, ex);
            }
        }

        [MvcComponentesAuthorize(Roles = "mat")]
        public ActionResult postImprimirContrato(int cd_contrato)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                // Verifica permissão "Conta Segura" do usuário.
                bool contaSegura = false;
                if (Session["Permissoes"] != null)
                    contaSegura = Session["Permissoes"].ToString().Contains("ctsg");

                string enderecoRelatorioWeb = Session["enderecoRelatorioWeb"] + "";
                int cd_escola = (int)Session["CodEscolaSelecionada"];
                string enderecoRetorno = Session["enderecoWeb"] + "";
                var parms = "cd_contrato=" + cd_contrato + "&conta_segura=" + contaSegura + "&cd_escola=" + cd_escola +
                    "&" + Componentes.Utils.ReportParameter.PARAMETRO_DATA_HORA_ATUAL + "=" + DateTime.Now;
                string parametros = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(Componentes.Utils.MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parms, System.Text.Encoding.UTF8), Componentes.Utils.MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);
                parametros += "&enderecoWeb=" + Session["enderecoWeb"];
                return new RenderJsonActionResult { Result = enderecoRelatorioWeb + "RelatorioApresentacao/getImprimirContrato?" + parametros };
            }
            catch (Exception ex)
            {
                return gerarLogException(FundacaoFisk.SGF.Utils.Messages.Messages.msgArqContratoNaoEncontrado, retorno, logger, ex);
            }
        }

        public ActionResult GerarCarne(int cd_contrato, int parcIniCarne, int parcFimCarne, bool imprimirCapaCarne)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                string enderecoRelatorioWeb = Session["enderecoRelatorioWeb"] + "";
                int cd_escola = (int)Session["CodEscolaSelecionada"];
                string enderecoRetorno = Session["enderecoWeb"] + "";
                bool contaSegura = Session["Permissoes"].ToString().Contains("ctsg");

                //var parms = "cd_contrato=" + cd_contrato + "&parcIniCarne=" + parcIniCarne + "&parcFimCarne=" + parcFimCarne +
                //    "&imprimirCapaCarne=" + imprimirCapaCarne + "&cd_escola=" + cd_escola + "&contaSegura=" + contaSegura;

                var parms = string.Empty;
                var carne = new ParametrosCarneUI
                {
                    cd_contrato = cd_contrato,
                    parcIniCarne = parcIniCarne,
                    parcFimCarne = parcFimCarne,
                    imprimirCapaCarne = imprimirCapaCarne,
                    cd_escola = cd_escola,
                    contaSegura = contaSegura,
                    sgfNew = false
                };
                var carneJson = JsonConvert.SerializeObject(carne);
                //Encode parametros
                var paramsEmBytes = Encoding.UTF8.GetBytes(carneJson);
                parms = Convert.ToBase64String(paramsEmBytes);

                return new RenderJsonActionResult { Result = enderecoRelatorioWeb + "RelatorioApresentacao/GerarCarne?parametros=" + parms };
            }
            catch (Exception ex)
            {
                return gerarLogException(FundacaoFisk.SGF.Utils.Messages.Messages.msgArqContratoNaoEncontrado, retorno, logger, ex);
            }
        }

        public ActionResult GerarHistorico([FromUri] HistoricoAlunoReportUI historicoAlunoReportUI)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                string enderecoRelatorioWeb = Session["enderecoRelatorioWeb"] + "";
                int cd_escola = (int)Session["CodEscolaSelecionada"];
                string enderecoRetorno = Session["enderecoWeb"] + "";

                StringBuilder parametros = new StringBuilder();
                parametros.Append("PEscola=").Append(cd_escola)
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
                          .Append("&").Append(Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO).Append("=Histórico do Aluno")
                          .Append("&").Append(Componentes.Utils.ReportParameter.PARAMETRO_TIPO_RELATORIO).Append("=").Append(FundacaoFisk.SGF.Utils.TipoRelatorioSGFEnum.rtpHistoricoAluno);

                if (Session["NomeEscolaSelecionada"] != null)
                    parametros.Append("&").Append(Componentes.Utils.ReportParameter.PARAMETRO_NOME_EMPRESA + "=").Append(Session["NomeEscolaSelecionada"]);
                if (Session["loginUsuario"] != null)
                    parametros.Append("&").Append(Componentes.Utils.ReportParameter.PARAMETRO_NOME_USUARIO + "=").Append(Session["loginUsuario"]);
                parametros.Append("&").Append(Componentes.Utils.ReportParameter.PARAMETRO_DATA_HORA_ATUAL + "=").Append(DateTime.Now);

                string parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros.ToString(), System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);

                parametrosCript += "&enderecoWeb=" + Session["enderecoWeb"];

                return new RenderJsonActionResult { Result = enderecoRelatorioWeb + "RelatorioApresentacao/GerarHistorico?" + parametrosCript };
            }
            catch (Exception ex)
            {
                return gerarLogException(FundacaoFisk.SGF.Utils.Messages.Messages.msgArqContratoNaoEncontrado, retorno, logger, ex);
            }
        }

        public ActionResult GerarCartas(int ano, string cdPessoa)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                SendEmail sendEmail = new SendEmail();
                SendEmail.configurarEmailSection(sendEmail);
                string enderecoRelatorioWeb = Session["enderecoRelatorioWeb"] + "";
                int cd_escola = (int)Session["CodEscolaSelecionada"];
                string enderecoRetorno = Session["enderecoWeb"] + "";

                var parms = "ano=" + ano + "&cdEscola=" + cd_escola + "&cdPessoa=" + cdPessoa + "&host=" + sendEmail.host + "&porta=" + sendEmail.porta +
                            "&ssl=" + sendEmail.ssl + "&remetente=" + sendEmail.remetente +
                            "&password=" + sendEmail.password + "&userName=" + sendEmail.userName + "&dominio=" + sendEmail.dominio;

                string parametros = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(Componentes.Utils.MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parms, System.Text.Encoding.UTF8), Componentes.Utils.MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);
                parametros += "&enderecoWeb=" + Session["enderecoWeb"];

                return new RenderJsonActionResult { Result = enderecoRelatorioWeb + "RelatorioApresentacao/GerarCartas?" + parametros };
            }
            catch (Exception ex)
            {
                return gerarLogException(FundacaoFisk.SGF.Utils.Messages.Messages.msgArqContratoNaoEncontrado, retorno, logger, ex);
            }
        }

        [HttpComponentesAuthorize(Roles = "percfagruav")]
        public ActionResult findTurmaPercentualFaltaGrupoAvancado(int cd_turma, int? cd_turma_ppt, bool id_turma_ppt)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                string enderecoRelatorioWeb = Session["enderecoRelatorioWeb"] + "";
                int cd_escola = (int)Session["CodEscolaSelecionada"];
                //string enderecoRetorno = Session["enderecoWeb"] + "";
                //bool contaSegura = Session["Permissoes"].ToString().Contains("ctsg");

                //var parms = "cd_contrato=" + cd_contrato + "&parcIniCarne=" + parcIniCarne + "&parcFimCarne=" + parcFimCarne +
                //    "&imprimirCapaCarne=" + imprimirCapaCarne + "&cd_escola=" + cd_escola + "&contaSegura=" + contaSegura;
                //int cd_turma, int? cd_turma_ppt, bool id_turma_ppt
                var parms = string.Empty;
                var percentualGrupoAvancadoUi = new ParametrosPercentualFaltaGrupoAvancadoUI
                {
                    cd_turma = cd_turma,
                    cd_turma_ppt = cd_turma_ppt,
                    id_turma_ppt = id_turma_ppt,
                    cd_escola = cd_escola
                };
                var percentualGrupoAvancadoJson = JsonConvert.SerializeObject(percentualGrupoAvancadoUi);
                //Encode parametros
                var paramsEmBytes = Encoding.UTF8.GetBytes(percentualGrupoAvancadoJson);
                parms = Convert.ToBase64String(paramsEmBytes);

                return new RenderJsonActionResult { Result = enderecoRelatorioWeb + "RelatorioApresentacao/findTurmaPercentualFaltaGrupoAvancado?parametros=" + parms };
            }
            catch (Exception ex)
            {
                return gerarLogException(FundacaoFisk.SGF.Utils.Messages.Messages.msgArqContratoNaoEncontrado, retorno, logger, ex);
            }
        }

        public ActionResult GerarEtiqueta(int cd_mala_direta)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                string enderecoRelatorioWeb = Session["enderecoRelatorioWeb"] + "";

                var parms = string.Empty;
                var cdMalaDiretaJson = JsonConvert.SerializeObject(cd_mala_direta);
                //Encode parametros
                var paramsEmBytes = Encoding.UTF8.GetBytes(cdMalaDiretaJson);
                parms = Convert.ToBase64String(paramsEmBytes);

                return new RenderJsonActionResult { Result = enderecoRelatorioWeb + "RelatorioApresentacao/GerarEtiqueta?parametros=" + parms };
            }
            catch (Exception ex)
            {
                return gerarLogException(FundacaoFisk.SGF.Utils.Messages.Messages.msgArqContratoNaoEncontrado, retorno, logger, ex);
            }
        }
    }
}
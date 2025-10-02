using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Componentes.GenericController;
using Componentes.GenericModel;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Utils.Messages;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IBusiness;
using log4net;
using FundacaoFisk.SGF.Web.Services.EmailMarketing.Comum.IBusiness;
using System.Configuration;
using FundacaoFisk.SGF.Web.Services.InstituicaoEnsino.Comum.IBusiness;
using System.IO;
using Newtonsoft.Json;

using System.Data.Entity.Core;
using Microsoft.Reporting.WebForms;
using System.Data;

namespace FundacaoFisk.SGF.Web.Controllers
{
    public class MailMarketingController : ComponentesMVCController
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(CursoController));

        public MailMarketingController()
        {
        }

        public ActionResult Index()
        {
            return View();
        }

        [MvcComponentesAuthorize(Roles = "mailm")]
        public ActionResult VisualizadorCartaoPostal(int cd_mala_direta)
        {
            try
            {
                IEmailMarketingBusiness BusinessEmailMarketing = (IEmailMarketingBusiness)base.instanciarBusiness<IEmailMarketingBusiness>();
                IEscolaBusiness BusinessEscola = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                int cd_escola = (int)Session["CodEscolaSelecionada"];
                MalaDireta malaDireta = BusinessEmailMarketing.getMalaDiretaForView(cd_mala_direta, cd_escola);
                string verso = BusinessEscola.getVersoCartaoPostal();
                ViewBag.CartaoPostal += "<div style='page-break-before: always'>";
                ViewBag.CartaoPostal += malaDireta.tx_msg_completa;
                ViewBag.CartaoPostal += "</div>";

                return View();
            }
            catch(Exception ex) {
                string url = "";

                url = Request.QueryString["enderecoWeb"] != null ? HttpUtility.UrlDecode(Request.QueryString["enderecoWeb"], System.Text.Encoding.UTF8) + "/Erro" : ConfigurationManager.AppSettings["enderecoRetornoErro"];
                string stackTrace = HttpUtility.UrlEncode(ex.Message + ex.StackTrace + ex.InnerException);

                //TODO: Melhorar o código para um método post, já que método get tem limitação de caracteres. Enquanto isso, visualizar o erro através do log de relatorios configurado no web.config.
                //Trunca o tamanho da url para o método get:
                if(stackTrace.Length > 1450)
                    stackTrace = stackTrace.Substring(0, 1450);
                logger.Error(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroImprimirBoletos, ex);
                return Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroImprimirVisualizarCartaoPostal) + "&stackTrace=" + HttpUtility.UrlEncode(stackTrace));
            }
        }

        private string trocaUrls(string texto)
        {
            //Urls de homologação:
            texto = texto.Replace("sgfhomolog.fisk.com.br:153", "homologsgf.fisk.com.br");

            //Urls de produção:
            //texto.Replace("sgf.fisk.com.br:154", "nlb-sgferp");
            texto = texto.Replace("sgf.fisk.com.br:154", "nlb-sgferp.fisk.com.br");

            return texto;
        }

        [MvcComponentesAuthorize(Roles = "mailm")]
        public ActionResult BaixarCartaoPostal(int cd_mala_direta)
        {
            try
            {
                IEmailMarketingBusiness BusinessEmailMarketing = (IEmailMarketingBusiness)base.instanciarBusiness<IEmailMarketingBusiness>();
                IEscolaBusiness BusinessEscola = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                int cd_escola = (int)Session["CodEscolaSelecionada"];
                string cartao_postal_html = "";
                MalaDireta malaDireta = BusinessEmailMarketing.getMalaDiretaForView(cd_mala_direta, cd_escola);
                //string verso = BusinessEscola.getVersoCartaoPostal();
                cartao_postal_html += "<div style='page-break-before: always'>";
                cartao_postal_html += trocaUrls(malaDireta.tx_msg_completa);
                cartao_postal_html += "</div>";

                MemoryStream ms = new MemoryStream();
                Utils.Utils.MontarPDF(cartao_postal_html,false, false).CopyTo(ms);
                Response.ContentType = "application/pdf";
                //Response.ContentEncoding = System.Text.Encoding.UTF8;
                Response.ContentEncoding = System.Text.Encoding.Unicode;
                Response.AddHeader("content-disposition", "attachment;filename=cartao_postal.pdf");
                Response.Buffer = true;
                Response.Clear();
                byte[] buf = ms.ToArray();
                Response.OutputStream.Write(buf, 0, buf.Length);
                Response.OutputStream.Flush();
                Response.End();
                ms.Dispose();
                //Deleta o arquivo:
                //System.IO.File.Delete(caminho_relatorios + "/" + file_name + ".pdf");

                return this.Content(ms.ToArray().ToString(), "application/pdf");
            }
            catch(Exception ex) {
                string url = "";

                url = Request.QueryString["enderecoWeb"] != null ? HttpUtility.UrlDecode(Request.QueryString["enderecoWeb"], System.Text.Encoding.UTF8) + "/Erro" : ConfigurationManager.AppSettings["enderecoRetornoErro"];
                string stackTrace = HttpUtility.UrlEncode(ex.Message + ex.StackTrace + ex.InnerException);

                //TODO: Melhorar o código para um método post, já que método get tem limitação de caracteres. Enquanto isso, visualizar o erro através do log de relatorios configurado no web.config.
                //Trunca o tamanho da url para o método get:
                if(stackTrace.Length > 1450)
                    stackTrace = stackTrace.Substring(0, 1450);
                if(stackTrace.Contains("UnknownNetworkError"))
                    return Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroImprimirVisualizarCartaoPostalSemPermissao) + "&stackTrace=" + HttpUtility.UrlEncode(stackTrace));
                logger.Error(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroImprimirBoletos, ex);
                return Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroImprimirVisualizarCartaoPostal) + "&stackTrace=" + HttpUtility.UrlEncode(stackTrace));
            }
        }

        [HttpComponentesAuthorize(Roles = "mailm.i,mailm.a")]
        public ActionResult postComporMensagemEnviar(MalaDireta mala_direta)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                if (mala_direta == null)
                    throw new ObjectNotFoundException(Messages.msgRegCritError);

                IEmailMarketingBusiness BusinessEmailMarketing = (IEmailMarketingBusiness)base.instanciarBusiness<IEmailMarketingBusiness>();
                mala_direta.cd_escola = (int)Session["CodEscolaSelecionada"];
                if (!String.IsNullOrEmpty(mala_direta.dc_hr_inicial))
                    mala_direta.hh_inicial = TimeSpan.Parse(mala_direta.dc_hr_inicial);
                if (!String.IsNullOrEmpty(mala_direta.dc_hr_fim))
                    mala_direta.hh_final = TimeSpan.Parse(mala_direta.dc_hr_fim);
                if (mala_direta.ListasEnderecoMalaView != null && mala_direta.ListasEnderecoMalaView.Count() > 0)
                {
                    mala_direta.ListasEnderecoMala = mala_direta.ListasEnderecoMalaView;
                    gerarUrlListaNaoInscritoMaladireta(mala_direta.ListasEnderecoMala.ToList());
                }
                if(!string.IsNullOrEmpty(mala_direta.json_nome_arquivos))
                    mala_direta.nome_arquivos_anexo = JsonConvert.DeserializeObject<Dictionary<string, string>>(mala_direta.json_nome_arquivos);

                mala_direta.cd_usuario = (int)Session["CodUsuario"];
                bool resposta = BusinessEmailMarketing.postComporMensagemEnviar(mala_direta);
                if (resposta)
                    retorno.AddMensagem(Messages.msgEnvioMalaDiretaSuccess, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                else
                    retorno.AddMensagem(Messages.msgEnvioMalaDiretaError, null, ReturnResult.MensagemWeb.TipoMensagem.WARNING);
                return new RenderJsonActionResult { Result = JsonConvert.SerializeObject(retorno) };
            }
            catch(ObjectNotFoundException ex)
            {
                retorno.AddMensagem(ex.Message, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                return new RenderJsonActionResult { Result = JsonConvert.SerializeObject(retorno) };
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegCritError, new ReturnResult(), logger, ex);
            }
        }        

        private void gerarUrlListaNaoInscritoMaladireta(List<ListaEnderecoMala> listaEnderecos)
        {
            int cd_escola = (int)Session["CodEscolaSelecionada"];
            string enderecoWeb = ConfigurationManager.AppSettings["UrlInternetMailMarketing"].ToString();

            if (listaEnderecos != null)
                foreach (ListaEnderecoMala le in listaEnderecos)
                {
                    if (cd_escola > 0 && le != null)
                    {
                        int cd_pessoa_escola = cd_escola;
                        // Pega os parâmetros do usuário para criar a url do relatório:
                        string parametros = "cd_empresa=" + cd_escola + "&cd_cadastro=" + le.cd_cadastro + "&id_cadastro=" + le.id_cadastro;
                        string parametrosCript = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);
                        le.url_nao_inscrito = enderecoWeb + "/RelatorioApresentacao/NaoInscrito?" + parametrosCript;
                    }
                }
        }
    }
}
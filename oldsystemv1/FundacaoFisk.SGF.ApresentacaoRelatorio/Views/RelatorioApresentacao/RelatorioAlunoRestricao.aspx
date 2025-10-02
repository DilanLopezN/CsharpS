﻿<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="FundacaoFisk.SGF.GenericModel" %>
<%@ Import Namespace="System.Data" %>
<%@ Register Assembly="Componentes.ApresentadorRelatorio" Namespace="Componentes.ApresentadorRelatorio" TagPrefix="cc1" %>

<!DOCTYPE html>

<html>
<head id="Head1" runat="server">
    <title>Relatório</title>
</head>
<body>
    <div>
     <script runat="server">

         private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("RelatorioAlunoRestricao.aspx");

         public void Page_Load(object sender, System.EventArgs e)
         {
             string url = Request.QueryString["enderecoWeb"] != null ? HttpUtility.UrlDecode(Request.QueryString["enderecoWeb"], System.Text.Encoding.UTF8) : ConfigurationManager.AppSettings["enderecoRetornoErro"];
             bool abortou_requisicao = false;
             try
             {
                 if (!IsPostBack)
                     bindRelatorio(url, ref abortou_requisicao);
             }
             catch (Exception exe)
             {
                 RedirecionaAplicacaoPrincipal(abortou_requisicao, url, exe);
             }
         }

         private void RedirecionaAplicacaoPrincipal(bool abortou_requisicao, string url, Exception exe)
         {
             try
             {
                 //url = Request.QueryString["enderecoWeb"] != null ? HttpUtility.UrlDecode(Request.QueryString["enderecoWeb"], System.Text.Encoding.UTF8) + "/Erro" : ConfigurationManager.AppSettings["enderecoRetornoErro"];
                 url = FundacaoFisk.SGF.ApresentacaoRelatorio.comum.UrlErro.ObterUrlErroRelatorio(url);
                 string stackTrace = HttpUtility.UrlEncode(exe.Message + exe.StackTrace + exe.InnerException);

                 //TODO: Melhorar o código para um método post, já que método get tem limitação de caracteres. Enquanto isso, visualizar o erro através do log de relatorios configurado no web.config.
                 //Trunca o tamanho da url para o método get:
                 if (stackTrace.Length > 1450)
                     stackTrace = stackTrace.Substring(0, 1450);
                 logger.Error(FundacaoFisk.SGF.Utils.Messages.Messages.msgEmissaoRelatorioError, exe);
                 if (!abortou_requisicao)
                     Response.Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroRenderizarRelatorio) + "&stackTrace=" + HttpUtility.UrlEncode(stackTrace));
             }
             catch (Exception e)
             {
                 logger.Error(FundacaoFisk.SGF.Utils.Messages.Messages.msgEmissaoRelatorioError + " url: " + url, e);
             }
         }

         private void Page_Init(object sender, System.EventArgs e)
         {
             Context.Handler = this.Page;
         }

         private void bindRelatorio(string url, ref bool abortou_requisicao)
         {
             Hashtable parametrosPesquisa = new Hashtable();
             Hashtable parametrosRelatorio = new Hashtable();
             string parms = HttpUtility.UrlDecode(Request.QueryString[Componentes.Utils.ReportParameter.PARAMETROS], System.Text.Encoding.UTF8).Replace(" ", "+");
             //parametros = "@cdEscola=" + cd_pessoa_escola + "&pOrgao=" + cdOrgao +  "&@ptipodata=" + tipodata + "&@pDtaI=" + dtaInicial + "&@pDtaF=" + dtaFinal;

             bool renderizarRelatorio = true;
             DateTime? pDtIni = null;
             DateTime? pDtFim = null;
             int cdEscola = 0;
             int cdOrgao = 0;
             byte tipodata = 0;
             string relatorioDesc = "";

             if (parms != null)
             {
                 parms = Componentes.Utils.MD5CryptoHelper.descriptografaSenha(parms, Componentes.Utils.MD5CryptoHelper.KEY);
                 parms = HttpUtility.UrlDecode(parms, System.Text.Encoding.UTF8);
                 string[] parametrosGet = parms.Split('&');

                 for (int i = 0; i < parametrosGet.Length; i++)
                 {
                     string[] parametrosHash = parametrosGet[i].Split('=');

                     if (parametrosHash[0].Equals("@cdEscola"))
                         cdEscola = int.Parse(parametrosHash[1]);
                     if (parametrosHash[0].Equals("@pOrgao"))
                         cdOrgao = int.Parse(parametrosHash[1]);
                     if (parametrosHash[0].Equals("@pDtaI") && parametrosHash[1] != "")
                         pDtIni = DateTime.Parse(parametrosHash[1]);
                     if (parametrosHash[0].Equals("@pDtaF") && parametrosHash[1] != "")
                         pDtFim = DateTime.Parse(parametrosHash[1]);
                     if (parametrosHash[0].Equals("@ptipodata"))
                         tipodata = byte.Parse(parametrosHash[1]);// sintético ou analítico

                     if (!parametrosHash[0].StartsWith("@"))
                         parametrosRelatorio.Add(parametrosHash[0], parametrosHash[1]);

                 }
             }
             // Adicionando o periodo 

             if (parametrosRelatorio != null && parametrosRelatorio["PDataHoraAtual"] != null)
             {
                 DateTime dtHoraAtualServAplic = DateTime.Parse(parametrosRelatorio["PDataHoraAtual"].ToString());
                 DateTime dtHoraAtualRelat = DateTime.Now;
                 TimeSpan t = dtHoraAtualRelat - dtHoraAtualServAplic;
                 int timeout = int.Parse(ConfigurationManager.AppSettings["timeoutRelatorio"] + "");
                 if (t.TotalSeconds > timeout)
                 {
                     //url = Request.QueryString["enderecoWeb"] != null ? HttpUtility.UrlDecode(Request.QueryString["enderecoWeb"], System.Text.Encoding.UTF8) + "/Erro" : ConfigurationManager.AppSettings["enderecoRetornoErro"];
                     url = FundacaoFisk.SGF.ApresentacaoRelatorio.comum.UrlErro.ObterUrlErroRelatorio(url);
                     Response.Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(FundacaoFisk.SGF.Utils.Messages.Messages.msgInfoSessaoRelatExp) + "&stackTrace=");
                     renderizarRelatorio = false;
                 }
             }//if

             if (renderizarRelatorio)
             {
                 FundacaoFisk.SGF.Web.Services.ApresentadorRelatorio.RelatorioController apresentadorRelatorio = new FundacaoFisk.SGF.Web.Services.ApresentadorRelatorio.RelatorioController();

                 string nomeRelatorio = "RptAlunoRestricao.rdlc";
                 string tituloRel = "Relatório de Restrições Financeiras";
                 if (tipodata == 1) relatorioDesc = "Período de Data de Entrada: ";
                 else relatorioDesc = "Período de Data de Saída: ";
                 if (pDtIni == null)
                 {
                     if (pDtFim == null) relatorioDesc = relatorioDesc + "Todos";
                     else relatorioDesc = relatorioDesc + "Até " + String.Format("{0:dd/MM/yyyy}", pDtFim);
                 }
                 else
                 {
                     if (pDtFim == null) relatorioDesc = relatorioDesc + "a partir de " + String.Format("{0:dd/MM/yyyy}", pDtIni);
                     else relatorioDesc = relatorioDesc + "De " + String.Format("{0:dd/MM/yyyy}", pDtIni) + " à " + String.Format("{0:dd/MM/yyyy}", pDtFim);
                 }
                 parametrosRelatorio.Add("PPeriodo", relatorioDesc);
                 parametrosRelatorio.Add("PNomeRelatorio", nomeRelatorio);
                 parametrosRelatorio.Add(Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO, tituloRel);

                 Hashtable sourceHash = new Hashtable();

                 DataTable dtReportData = apresentadorRelatorio.GetSourceRptAlunoRestricao(cdEscola, cdOrgao, pDtIni, pDtFim, tipodata);

                 if (dtReportData != null && dtReportData.Rows != null && dtReportData.Rows.Count > 0)
                 {
                     try
                     {
                         sourceHash["dtReportData"] = dtReportData;
                         relatorio.ExibirRelatorioEspecializado(ConfigurationManager.AppSettings["caminhoRelatorio"] + "", FundacaoFisk.SGF.Utils.ReportParameter.RELATORIO_ALUNO_RESTRICAO, sourceHash, parametrosRelatorio);
                     }
                     catch (Exception e)
                     {
                         Console.WriteLine(e);
                         throw;
                     }
                 }
                 else
                 {
                     //url = Request.QueryString["enderecoWeb"] != null ? HttpUtility.UrlDecode(Request.QueryString["enderecoWeb"], System.Text.Encoding.UTF8) + "/Erro" : ConfigurationManager.AppSettings["enderecoRetornoErro"];
                     url = FundacaoFisk.SGF.ApresentacaoRelatorio.comum.UrlErro.ObterUrlErroRelatorio(url);
                     Response.Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(Componentes.Utils.Messages.Messages.msgRegNotEnc) + "&stackTrace=");
                 }

             }
             else
             {
                 throw new Exception(Componentes.Utils.Messages.Messages.msgSessaoExpirada);
             }
         }

         protected void relatorio_DocumentMapNavigation(object sender, Microsoft.Reporting.WebForms.DocumentMapNavigationEventArgs e)
         {
             //bindRelatorio();
         }

         protected void relatorio_Search(object sender, Microsoft.Reporting.WebForms.SearchEventArgs e)
         {
             //bindRelatorio();
         }

         protected void relatorio_Drillthrough(object sender, Microsoft.Reporting.WebForms.DrillthroughEventArgs e)
         {
             //bindRelatorio();
         }

         protected void relatorio_Load(object sender, EventArgs e)
         {
             /*if(!IsPostBack) 
                 bindRelatorio();*/
         }

         protected void relatorio_BookmarkNavigation(object sender, Microsoft.Reporting.WebForms.BookmarkNavigationEventArgs e)
         {
             //bindRelatorio();
         }

         protected void relatorio_PageNavigation(object sender, Microsoft.Reporting.WebForms.PageNavigationEventArgs e)
         {
             //bindRelatorio();
         }

         protected override void OnInit(EventArgs e)
         {
             base.OnInit(e);

             StringBuilder sb = new StringBuilder();
             sb.Append("setInterval(KeepSessionAlive, " + getSessionTimeoutInMs() + ");");
             sb.Append("function KeepSessionAlive() {");
             //sb.Append(string.Format("$.post('{0}', null);", ResolveUrl("~/KeepSessionAlive.ashx")));

             //sb.Append("var xhr = new XMLHttpRequest();");
             //sb.Append(string.Format("xhr.open('GET', '{0}');", ResolveUrl("~/Relatorio/renovarSessao")));
             //sb.Append("xhr.onload = function() {");
             //sb.Append("};");
             //sb.Append("xhr.send();");
             sb.Append("window.location = window.location;");
             sb.Append("};");

             // register on page
             Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "SessionKeepAlive", sb.ToString(), true);
         }

         private int getSessionTimeoutInMs()
         {
             return (this.Session.Timeout * 60000) / 2; //(this.Session.Timeout * 60000) - 10000
         }

     </script>
     <script>
         (function (i, s, o, g, r, a, m) {
             i['GoogleAnalyticsObject'] = r; i[r] = i[r] || function () {
                 (i[r].q = i[r].q || []).push(arguments)
             }, i[r].l = 1 * new Date(); a = s.createElement(o),
             m = s.getElementsByTagName(o)[0]; a.async = 1; a.src = g; m.parentNode.insertBefore(a, m)
         })(window, document, 'script', '//www.google-analytics.com/analytics.js', 'ga');

         ga('create', 'UA-97448037-1', 'auto');
         ga('send', 'pageview');
    </script>
    <script>
        var _prum = [['id', '570d92daabe53d3930288c12'],
                     ['mark', 'firstbyte', (new Date()).getTime()]];
        (function () {
            var s = document.getElementsByTagName('script')[0]
              , p = document.createElement('script');
            p.async = 'async';
            p.src = '//rum-static.pingdom.net/prum.min.js';
            s.parentNode.insertBefore(p, s);
        })();
    </script>
    </div>
    <form id="Form1" runat="server" style="width: 100%; height: 100%;">
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <cc1:VisualizadorRelatorio ID="relatorio" runat="server" Height="100%" Width="100%" Font-Names="Verdana" DocumentMapCollapsed="false" ShowFindControls="true"
            ShowDocumentMapButton="true" ShowToolBar="true" ShowPrintButton="true" ProcessingMode="Local" Font-Size="8pt" AsyncRendering="false" SizeToReportContent="True"
            OnDocumentMapNavigation="relatorio_DocumentMapNavigation" OnSearch="relatorio_Search" OnDrillthrough="relatorio_Drillthrough" OnLoad="relatorio_Load"
            OnBookmarkNavigation="relatorio_BookmarkNavigation" OnPageNavigation="relatorio_PageNavigation" KeepSessionAlive="false">
        </cc1:VisualizadorRelatorio>
    </form>
</body>
</html>

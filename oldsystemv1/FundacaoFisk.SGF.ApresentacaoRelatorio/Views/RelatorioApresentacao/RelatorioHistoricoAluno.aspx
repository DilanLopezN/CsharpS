<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="FundacaoFisk.SGF.GenericModel" %>
<%@ Register Assembly="Componentes.ApresentadorRelatorio" Namespace="Componentes.ApresentadorRelatorio" TagPrefix="cc1" %>
<!DOCTYPE html>

<html>
<head runat="server">
    <title>Relatório</title>
</head>
<body>
    <div style="Width:100%;height:100%;">
        <script runat="server">
            private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("RelatorioHistoricoAluno.aspx");

            public int cdAluno;//Parametro Global para o subreport
            DataTable dtHistoricoTurma;
            DataTable dtHistoricoTurmaConceito;
            DataTable dtHistoricoEstagioConceito;
            DataTable dtHistoricoTurmaAvaliacao;
            DataTable dtHistoricoAvaliacaoTurma;
            DataTable dtHistoricoEstagio;
            DataTable dtHistoricoAvaliacaoEstagio;
            DataTable dtHistoricoEventoAula1;
            DataTable dtHistoricoEventoAula2;
            DataTable dtHistoricoTitulo;
            DataTable dtHistoricoObs;
            DataTable dtHistoricoAtividade;
            DataTable dtHistoricoFollow;
            DataTable dtHistoricoItem;

            public void Page_Load(object sender, System.EventArgs e)
            {
                string url = Request.QueryString["enderecoWeb"] != null ? HttpUtility.UrlDecode(Request.QueryString["enderecoWeb"], System.Text.Encoding.UTF8) : ConfigurationManager.AppSettings["enderecoRetornoErro"];

                try {
                    if(!IsPostBack)
                        bindRelatorio(url);
                }
                catch(Exception exe) {
                    RedirecionaAplicacaoPrincipal(url, exe);
                }
            }

            private void RedirecionaAplicacaoPrincipal(string url, Exception exe){
                try
                {
                    //url = Request.QueryString["enderecoWeb"] != null ? HttpUtility.UrlDecode(Request.QueryString["enderecoWeb"], System.Text.Encoding.UTF8) + "/Erro" : ConfigurationManager.AppSettings["enderecoRetornoErro"];
                    url = FundacaoFisk.SGF.ApresentacaoRelatorio.comum.UrlErro.ObterUrlErroRelatorio(url);
                    string stackTrace = HttpUtility.UrlEncode(exe.Message + exe.StackTrace + exe.InnerException);

                    //TODO: Melhorar o código para um método post, já que método get tem limitação de caracteres. Enquanto isso, visualizar o erro através do log de relatorios configurado no web.config.
                    //Trunca o tamanho da url para o método get:
                    if (stackTrace.Length > 1450)
                        stackTrace = stackTrace.Substring(0, 1450);
                    if (exe.Message != Componentes.Utils.Messages.Messages.msgSessaoExpirada)
                        logger.Error(FundacaoFisk.SGF.Utils.Messages.Messages.msgEmissaoRelatorioError, exe);
                    Response.Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(FundacaoFisk.SGF.Utils.Messages.Messages.msgErroRenderizarRelatorio) + "&stackTrace=" + HttpUtility.UrlEncode(stackTrace));
                }
                catch (Exception e)
                {
                    logger.Error(FundacaoFisk.SGF.Utils.Messages.Messages.msgEmissaoRelatorioError + " url: " + url, e);
                }
            }

            private void Page_Init(object sender, System.EventArgs e) {
                Context.Handler = this.Page;
            }

            private void bindRelatorio(string url){
                Hashtable parametrosPesquisa = new Hashtable();
                Hashtable parametrosRelatorio = new Hashtable();
                string parms = HttpUtility.UrlDecode(Request.QueryString[Componentes.Utils.ReportParameter.PARAMETROS], System.Text.Encoding.UTF8).Replace(" ", "+");
                int cdAluno = 0;
                string noAluno = null;
                string produtos = null;
                string statustitulo = null;

                bool renderizarRelatorio = true;

                if(parms != null) {
                    parms = Componentes.Utils.MD5CryptoHelper.descriptografaSenha(parms, Componentes.Utils.MD5CryptoHelper.KEY);
                    parms = HttpUtility.UrlDecode(parms, System.Text.Encoding.UTF8);
                    string[] parametrosGet = parms.Split('&');

                    for(int i = 0; i < parametrosGet.Length; i++) {
                        string[] parametrosHash = parametrosGet[i].Split('=');

                        if (parametrosHash[0].Equals("@cd_aluno"))
                            cdAluno = int.Parse(parametrosHash[1]);
                        if(parametrosHash[0].StartsWith("@"))
                            parametrosPesquisa.Add(parametrosHash[0].Substring(1, parametrosHash[0].Length - 1), parametrosHash[1]);
                        else
                        {
                            if (parametrosHash[0].Equals("no_aluno"))
                            {                   
                                noAluno = parametrosHash[1];
                            }
                            else {
                                if (parametrosHash[0].StartsWith("Pmostrar"))
                                {
                                    parametrosRelatorio.Add(parametrosHash[0], Boolean.Parse(parametrosHash[1]));
                                }
                                else
                                {
                                    parametrosRelatorio.Add(parametrosHash[0], parametrosHash[1]);
                                }

                                if (parametrosHash[0].Equals("produtos"))
                                {
                                    produtos = parametrosHash[1];
                                }

                                if (parametrosHash[0].Equals("PTitulos"))
                                {
                                    statustitulo = parametrosHash[1];
                                }
                            }

                        }
                    }

                    //Verifica se passou o tempo estipulado no web.config para espirar a sessão do usuario.
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

                    }
                    if (renderizarRelatorio)
                    {
                        FundacaoFisk.SGF.Web.Services.ApresentadorRelatorio.RelatorioController apresentadorRelatorio = new FundacaoFisk.SGF.Web.Services.ApresentadorRelatorio.RelatorioController();

                        parametrosRelatorio["PEscola"] = parametrosRelatorio["PEmpresa"];
                        parametrosRelatorio.Remove("PEmpresa");
                        parametrosRelatorio.Remove("PTipoRelatorio");
                        //parametrosRelatorio.Remove("PNomeUsuario");
                        parametrosRelatorio.Remove("produtos");

                        parametrosRelatorio.Add("PMostrarSubHTurma", true);
                        parametrosRelatorio.Add("PMostrarSubHTurmaAvaliacao", true);
                        parametrosRelatorio.Add("PMostrarSubHAvaliacaoTurma", true);
                        parametrosRelatorio.Add("PMostrarSubHEstagio", true);
                        parametrosRelatorio.Add("PMostrarSubHEstagioAvaliacao", true);
                        //parametrosRelatorio.Add("PMostrarSubHEventoAluno", true);
                        parametrosRelatorio.Add("PMostrarHSubHEventoAluno", true);
                        parametrosRelatorio.Add("PMostrarHSubTitulos", true);
                        parametrosRelatorio.Add("PMostrarHSubObs", true);
                        parametrosRelatorio.Add("PMostrarHSubAtividades", true);
                        parametrosRelatorio.Add("PMostrarHSubFollow", true);
                        parametrosRelatorio.Add("PMostrarHSubItem", true);
                        parametrosRelatorio.Add("PMostrarSubHTurmaConceito", true);
                        parametrosRelatorio.Add("PMostrarSubHEstagioConceito", true);

                        //Vai ser possível faze isto por termos somente 1 aluno por vez
                        dtHistoricoTurma = apresentadorRelatorio.GetAlunos(cdAluno, 1, produtos, statustitulo);
                        setReportParameter(dtHistoricoTurma, "PMostrarSubHTurma", false, parametrosRelatorio);
                        dtHistoricoTurmaAvaliacao = apresentadorRelatorio.GetAlunos(cdAluno, 2, produtos, statustitulo);
                        setReportParameter(dtHistoricoTurmaAvaliacao, "PMostrarSubHTurmaAvaliacao", false, parametrosRelatorio);
                        dtHistoricoAvaliacaoTurma = apresentadorRelatorio.GetAlunos(cdAluno, 3, produtos, statustitulo);
                        setReportParameter(dtHistoricoAvaliacaoTurma, "PMostrarSubHAvaliacaoTurma", false, parametrosRelatorio);
                        dtHistoricoEstagio = apresentadorRelatorio.GetAlunos(cdAluno, 4, produtos, statustitulo);
                        setReportParameter(dtHistoricoEstagio, "PMostrarSubHEstagio", false, parametrosRelatorio);
                        dtHistoricoAvaliacaoEstagio = apresentadorRelatorio.GetAlunos(cdAluno, 5, produtos, statustitulo);
                        setReportParameter(dtHistoricoAvaliacaoEstagio, "PMostrarSubHEstagioAvaliacao", false, parametrosRelatorio);
                        dtHistoricoEventoAula1 = apresentadorRelatorio.GetAlunos(cdAluno, 6, produtos, statustitulo);
                        //setReportParameter(dtHistoricoTurma, "PMostrarSubHEventoAluno", false, parametrosRelatorio);
                        dtHistoricoEventoAula2 = apresentadorRelatorio.GetAlunos(cdAluno, 7, produtos, statustitulo);
                        setReportParameter(dtHistoricoEventoAula2, "PMostrarHSubHEventoAluno", false, parametrosRelatorio);
                        dtHistoricoTitulo = apresentadorRelatorio.GetAlunos(cdAluno, 8, produtos, statustitulo);
                        setReportParameter(dtHistoricoTitulo, "PMostrarHSubTitulos", false, parametrosRelatorio);
                        dtHistoricoObs = apresentadorRelatorio.GetAlunos(cdAluno, 9, produtos, statustitulo);
                        setReportParameter(dtHistoricoObs, "PMostrarHSubObs", false, parametrosRelatorio);
                        dtHistoricoAtividade = apresentadorRelatorio.GetAlunos(cdAluno, 10, produtos, statustitulo);
                        setReportParameter(dtHistoricoAtividade, "PMostrarHSubAtividades", false, parametrosRelatorio);
                        dtHistoricoFollow = apresentadorRelatorio.GetAlunos(cdAluno, 11, produtos, statustitulo);
                        setReportParameter(dtHistoricoFollow, "PMostrarHSubFollow", false, parametrosRelatorio);
                        dtHistoricoItem = apresentadorRelatorio.GetAlunos(cdAluno, 12, produtos, statustitulo);
                        setReportParameter(dtHistoricoItem, "PMostrarHSubItem", false, parametrosRelatorio);
                        dtHistoricoTurmaConceito = apresentadorRelatorio.GetAlunos(cdAluno, 13, produtos, statustitulo);
                        setReportParameter(dtHistoricoTurmaConceito, "PMostrarSubHTurmaConceito", false, parametrosRelatorio);
                        dtHistoricoEstagioConceito = apresentadorRelatorio.GetAlunos(cdAluno, 14, produtos, statustitulo);
                        setReportParameter(dtHistoricoEstagioConceito, "PMostrarSubHEstagioConceito", false, parametrosRelatorio);

                         
                        Hashtable sourceHash = new Hashtable();

                        List<sp_RptHistoricoAlunoM_Result> dtReportData = apresentadorRelatorio.GetRtpHistoricoAlunoM(cdAluno);
                        List<Componentes.GenericModel.TO> sourceTO = null;
                        if (dtReportData.Count() > 0)
                        {
                            sourceTO = dtReportData.ToList<Componentes.GenericModel.TO>();
                        }

                        //sourceHash["DataHistoricoAluno"] = (List<sp_RptHistoricoAlunoM_Result>)FundacaoFisk.SGF.Utils.Utils.ConvertTo<sp_RptHistoricoAlunoM_Result>(dtReportData);
                        //relatorio.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("Atividades", sourceTO));


                        if (sourceTO != null && sourceTO.ToList<Componentes.GenericModel.TO>().Count > 0)
                        {
                            try
                            {
                                sourceHash["DataHistoricoAluno"] = dtReportData;
                                relatorio.LocalReport.SubreportProcessing += new Microsoft.Reporting.WebForms.SubreportProcessingEventHandler(HTurmaSubReportProcessing);
                                relatorio.LocalReport.Refresh();
                                relatorio.LocalReport.DisplayName = noAluno + " (" + DateTime.Now.ToShortDateString() + ")";
                                relatorio.ExibirRelatorioEspecializado(ConfigurationManager.AppSettings["caminhoRelatorio"] + "", FundacaoFisk.SGF.Utils.ReportParameter.RELATORIO_HISTORICO_ALUNO, sourceHash, parametrosRelatorio);
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
                }
                else
                    throw new Exception(Componentes.Utils.Messages.Messages.msgSessaoExpirada);
            }

            private static void setReportParameter(DataTable dt, string name, Boolean value, Hashtable parametrosRelatorio)
            {
                if (dt.Rows.Count == 0)
                {
                    parametrosRelatorio[name] = value;
                }
            }


            private void HTurmaSubReportProcessing(object sender, Microsoft.Reporting.WebForms.SubreportProcessingEventArgs e)
            {

                int cd_atividade = int.Parse(e.Parameters["cd_aluno"].Values[0].ToString());
                if (e.ReportPath.Contains("HistoricoTurmas"))
                {
                    DataTable dtRptSub = dtHistoricoTurma; // GetAlunos(cd_atividade, 1);
                    Microsoft.Reporting.WebForms.ReportDataSource ds = new Microsoft.Reporting.WebForms.ReportDataSource("DataHistorico", dtRptSub);
                    e.DataSources.Add(ds);
                }
                else if (e.ReportPath.Contains("HistoricoTurmaAvaliacao"))
                {
                    DataTable dtRptSub = dtHistoricoTurmaAvaliacao; //GetAlunos(cd_atividade, 2);
                    Microsoft.Reporting.WebForms.ReportDataSource ds = new Microsoft.Reporting.WebForms.ReportDataSource("DataTurmaAvaliacao", dtRptSub);
                    e.DataSources.Add(ds);
                }
                else if (e.ReportPath.Contains("HistoricoAvaliacaoTurma"))
                {
                    DataTable dtRptSub = dtHistoricoAvaliacaoTurma; // GetAlunos(cd_atividade, 3);
                    Microsoft.Reporting.WebForms.ReportDataSource ds = new Microsoft.Reporting.WebForms.ReportDataSource("DataAvaliacaoTurma", dtRptSub);
                    e.DataSources.Add(ds);
                }
                else if (e.ReportPath =="HistoricoEstagio")
                {
                    DataTable dtRptSub = dtHistoricoEstagio; // GetAlunos(cd_atividade, 4);
                    Microsoft.Reporting.WebForms.ReportDataSource ds = new Microsoft.Reporting.WebForms.ReportDataSource("DataEstagio", dtRptSub);
                    e.DataSources.Add(ds);
                }
                else if (e.ReportPath.Contains("HistoricoAvaliacaoEstagio"))
                {
                    DataTable dtRptSub = dtHistoricoAvaliacaoEstagio; // GetAlunos(cd_atividade, 5);
                    Microsoft.Reporting.WebForms.ReportDataSource ds = new Microsoft.Reporting.WebForms.ReportDataSource("DataAvaliacaoEstagio", dtRptSub);
                    e.DataSources.Add(ds);
                }
                else if (e.ReportPath.Contains("HistoricoEventoAula"))
                {
                    DataTable dtRptSub = dtHistoricoEventoAula1; // GetAlunos(cd_atividade, 6);
                    Microsoft.Reporting.WebForms.ReportDataSource ds6 = new Microsoft.Reporting.WebForms.ReportDataSource("DataAlunoEvento", dtRptSub);
                    e.DataSources.Add(ds6);
                    DataTable dtRptSub7 = dtHistoricoEventoAula2; // GetAlunos(cd_atividade, 7);
                    Microsoft.Reporting.WebForms.ReportDataSource ds7 = new Microsoft.Reporting.WebForms.ReportDataSource("DataAlunoAula", dtRptSub7);
                    e.DataSources.Add(ds7);
                }
                else if (e.ReportPath.Contains("HistoricoTitulo"))
                {
                    DataTable dtRptSub = dtHistoricoTitulo; // GetAlunos(cd_atividade, 8);
                    Microsoft.Reporting.WebForms.ReportDataSource ds = new Microsoft.Reporting.WebForms.ReportDataSource("DataHistoricoTitulo", dtRptSub);
                    e.DataSources.Add(ds);
                }
                else if (e.ReportPath.Contains("HistoricoObs"))
                {
                    DataTable dtRptSub = dtHistoricoObs; // GetAlunos(cd_atividade, 9);
                    Microsoft.Reporting.WebForms.ReportDataSource ds = new Microsoft.Reporting.WebForms.ReportDataSource("DataHistoricoObs", dtRptSub);
                    e.DataSources.Add(ds);
                }
                else if (e.ReportPath.Contains("HistoricoAtividade"))
                {
                    DataTable dtRptSub = dtHistoricoAtividade; // GetAlunos(cd_atividade, 10);
                    Microsoft.Reporting.WebForms.ReportDataSource ds = new Microsoft.Reporting.WebForms.ReportDataSource("DataHistoricoAtividade", dtRptSub);
                    e.DataSources.Add(ds);
                }
                else if (e.ReportPath.Contains("HistoricoFollow"))
                {
                    DataTable dtRptSub = dtHistoricoFollow; // GetAlunos(cd_atividade, 11);
                    Microsoft.Reporting.WebForms.ReportDataSource ds = new Microsoft.Reporting.WebForms.ReportDataSource("DataHistoricoFollow", dtRptSub);
                    e.DataSources.Add(ds);
                }
                else if (e.ReportPath.Contains("HistoricoItem"))
                {
                    DataTable dtRptSub = dtHistoricoItem; // GetAlunos(cd_atividade, 12);
                    Microsoft.Reporting.WebForms.ReportDataSource ds = new Microsoft.Reporting.WebForms.ReportDataSource("DataHistoricoItem", dtRptSub);
                    e.DataSources.Add(ds);
                }
                else if (e.ReportPath.Contains("HistoricoTurmaConceito"))
                {
                    DataTable dtRptSub = dtHistoricoTurmaConceito; // GetAlunos(cd_atividade, 13);
                    Microsoft.Reporting.WebForms.ReportDataSource ds = new Microsoft.Reporting.WebForms.ReportDataSource("DataHistoricoTurmaConceito", dtRptSub);
                    e.DataSources.Add(ds);
                }
                else if (e.ReportPath.Contains("HistoricoEstagioConceito"))
                {
                    DataTable dtRptSub = dtHistoricoEstagioConceito; // GetAlunos(cd_atividade, 14);
                    Microsoft.Reporting.WebForms.ReportDataSource ds = new Microsoft.Reporting.WebForms.ReportDataSource("DataHistoricoEstagioConceito", dtRptSub);
                    e.DataSources.Add(ds);
                }
            }

            protected void relatorio_DocumentMapNavigation(object sender, Microsoft.Reporting.WebForms.DocumentMapNavigationEventArgs e) {
                //bindRelatorio();
            }

            protected void relatorio_Search(object sender, Microsoft.Reporting.WebForms.SearchEventArgs e) {
                //bindRelatorio();
            }

            protected void relatorio_Drillthrough(object sender, Microsoft.Reporting.WebForms.DrillthroughEventArgs e) {
                //bindRelatorio();
            }

            protected void relatorio_Load(object sender, EventArgs e) {
                /*if(!IsPostBack) 
                    bindRelatorio();*/
            }

            protected void relatorio_BookmarkNavigation(object sender, Microsoft.Reporting.WebForms.BookmarkNavigationEventArgs e) {
                //bindRelatorio();
            }

            protected void relatorio_PageNavigation(object sender, Microsoft.Reporting.WebForms.PageNavigationEventArgs e) {
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
        <form id="Form1" runat="server" style="width:100%; height:100%;">
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <cc1:VisualizadorRelatorio id="relatorio" runat="server" height="100%" width="100%" Font-Names="Verdana" DocumentMapCollapsed="false"  ShowFindControls="true"
                ShowDocumentMapButton="true" ShowToolBar="true"  ShowPrintButton="true" ProcessingMode="Local" Font-Size="8pt" AsyncRendering="false" SizeToReportContent="True" 
                OnDocumentMapNavigation="relatorio_DocumentMapNavigation" OnSearch="relatorio_Search" OnDrillthrough="relatorio_Drillthrough" OnLoad="relatorio_Load"
                OnBookmarkNavigation="relatorio_BookmarkNavigation" OnPageNavigation="relatorio_PageNavigation" KeepSessionAlive="false"></cc1:VisualizadorRelatorio>
        </form>
    </div>
</body>
</html>
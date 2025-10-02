<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Register Assembly="Componentes.ApresentadorRelatorio" Namespace="Componentes.ApresentadorRelatorio" TagPrefix="cc1" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <title>Relatório</title>
</head>
<body>
    <div>
     <script runat="server">
     
         private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("RelatorioProgramacaoAulasTurma.aspx");

            public void Page_Load(object sender, System.EventArgs e)
            {
                string url = Request.QueryString["enderecoWeb"] != null ? HttpUtility.UrlDecode(Request.QueryString["enderecoWeb"], System.Text.Encoding.UTF8) : ConfigurationManager.AppSettings["enderecoRetornoErro"];
                try
                {
                    if (!IsPostBack)
                        bindRelatorio(url);
                }
                catch (Exception exe)
                {
                    RedirecionaAplicacaoPrincipal(url, exe);
                }
            }

            private void RedirecionaAplicacaoPrincipal(string url, Exception exe)
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

            private void bindRelatorio(string url) { 
                
                Hashtable parametrosPesquisa = new Hashtable();
                Hashtable parametrosRelatorio = new Hashtable();
                string parms = HttpUtility.UrlDecode(Request.QueryString[Componentes.Utils.ReportParameter.PARAMETROS], System.Text.Encoding.UTF8).Replace(" ", "+");
                DateTime dataBase = new DateTime();
                bool renderizarRelatorio = true;
                bool umaTurmaPorPagina = false;
                int cd_escola = 0;
                int cd_turma = 0;
                DateTime? pDtaI = null;
                DateTime? pDtaF = null;
                bool idMostrarFeriado = false;
                if (parms != null)
                {
                    parms = Componentes.Utils.MD5CryptoHelper.descriptografaSenha(parms, Componentes.Utils.MD5CryptoHelper.KEY);
                    parms = HttpUtility.UrlDecode(parms, System.Text.Encoding.UTF8);
                    string[] parametrosGet = parms.Split('&');

                    for (int i = 0; i < parametrosGet.Length; i++)
                    {
                        string[] parametrosHash = parametrosGet[i].Split('=');
                        if (parametrosHash[0].Equals("@cd_escola"))
                            cd_escola = int.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@DataBase"))
                            dataBase = DateTime.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@cdTurma"))
                            cd_turma = int.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@dtInicial") && parametrosHash[1] != "")
                            pDtaI = DateTime.Parse(parametrosHash[1] );
                        if (parametrosHash[0].Equals("@dtFinal") && parametrosHash[1] != "")
                            pDtaF = DateTime.Parse(parametrosHash[1] );
                        if (parametrosHash[0].Equals("@umaTurmaPorPagina"))
                            umaTurmaPorPagina = bool.Parse(parametrosHash[1]);
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
                        var caminhoRelatorio = FundacaoFisk.SGF.Utils.ReportParameter.RELATORIO_PROGRAMACAO_AULAS_TURMA;
                        IEnumerable<FundacaoFisk.SGF.Web.Services.Coordenacao.Model.TurmaSearch> turmasAlunos = new List<FundacaoFisk.SGF.Web.Services.Coordenacao.Model.TurmaSearch>();
                        Hashtable sourceHash = new Hashtable();
                        FundacaoFisk.SGF.Web.Services.ApresentadorRelatorio.RelatorioController apresentadorRelatorio = new FundacaoFisk.SGF.Web.Services.ApresentadorRelatorio.RelatorioController();
                        turmasAlunos = apresentadorRelatorio.getRptTurmasProgAula(cd_turma, cd_escola, pDtaI, pDtaF).ToList();
                        sourceHash["DataSetReportTurma"] = turmasAlunos;
                        if (turmasAlunos.Count() > 0)
                            foreach (var t in turmasAlunos)
                            {
                                string nomes_porfessores = "";
                                if (t.pessoasTurma != null)
                                {
                                    t.nro_alunos = t.pessoasTurma.Count();
                                    foreach (var p in t.pessoasTurma)
                                        nomes_porfessores += p.dc_reduzido_pessoa + ", ";
                                }
                                else
                                    t.nro_alunos = 0;
                                nomes_porfessores = nomes_porfessores.ToString().TrimEnd(',', ' ');
                                t.no_professor = nomes_porfessores;
                                t.pessoasTurma = null;
                               
                            }
                        if ((pDtaI != null && pDtaI != DateTime.MinValue) || (pDtaF != null && pDtaF != DateTime.MinValue))
                            parametrosRelatorio.Add("PPeriodo", "Período de " + String.Format("{0:dd/MM/yyyy}", pDtaI) + " à " + String.Format("{0:dd/MM/yyyy}", pDtaF));
                        parametrosRelatorio.Add(Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO, "Relatório de Programação das Aulas das Turmas");
                        parametrosRelatorio.Add("DataBase", dataBase);
                        parametrosRelatorio.Add("PUmaTurmaPagina", umaTurmaPorPagina);
                        if (turmasAlunos != null && turmasAlunos.Count() > 0)
                        {
                            relatorio.ExibirRelatorioEspecializado(ConfigurationManager.AppSettings["caminhoRelatorio"] + "", caminhoRelatorio, sourceHash, parametrosRelatorio);
                            relatorio.LocalReport.SubreportProcessing += new Microsoft.Reporting.WebForms.SubreportProcessingEventHandler(renderizaSubRelatorioProgramacoesTurma);
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

            private void renderizaSubRelatorioProgramacoesTurma(object sender, Microsoft.Reporting.WebForms.SubreportProcessingEventArgs e)
            {
                try{
                    int cd_turma = Convert.ToInt32(e.Parameters["cd_turma"].Values[0]);
                    int cd_escola = Convert.ToInt32(e.Parameters["cd_escola"].Values[0]);
                    bool idMostrarFeriado = bool.Parse(e.Parameters["idMostrarFeriado"].Values[0]);
                    FundacaoFisk.SGF.Web.Services.ApresentadorRelatorio.RelatorioController apresentadorRelatorio = new FundacaoFisk.SGF.Web.Services.ApresentadorRelatorio.RelatorioController();
                    e.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSetSubReportSubProgAulasTurma",
                      apresentadorRelatorio.getSubRptProgAulasTurma(cd_turma, cd_escola,
                      FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess.ProgramacaoTurmaDataAccess.TipoConsultaProgTurmaEnum.HAS_REPORT_PROG_TURMA, idMostrarFeriado)));
                }
                catch (Exception exe)
                {
                    logger.Error(FundacaoFisk.SGF.Utils.Messages.Messages.msgEmissaoRelatorioError, exe);
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

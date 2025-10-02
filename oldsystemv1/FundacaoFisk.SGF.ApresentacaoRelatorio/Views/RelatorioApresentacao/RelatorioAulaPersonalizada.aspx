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
     
            private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("RelatorioAulaPersonalizada.aspx");

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

                string no_aluno = String.Empty, no_turma_original = String.Empty;
                bool renderizarRelatorio = true;
                int cd_empresa = 0;
                int cd_aluno = 0;
                int? cd_produto = null;
                int? cd_curso = null;              
                DateTime? dt_inicial_agend = null;
                DateTime? dt_final_agend = null;
                DateTime? dt_inicial_lanc = null;
                DateTime? dt_final_lanc = null;
                TimeSpan? hr_inicial_agend = null;
                TimeSpan? hr_final_agend = null;
                TimeSpan? hr_inicial_lanc = null;
                TimeSpan? hr_final_lanc = null;                 
                    
                if (parms != null)
                {
                    parms = Componentes.Utils.MD5CryptoHelper.descriptografaSenha(parms, Componentes.Utils.MD5CryptoHelper.KEY);
                    parms = HttpUtility.UrlDecode(parms, System.Text.Encoding.UTF8);
                    string[] parametrosGet = parms.Split('&');

                    for (int i = 0; i < parametrosGet.Length; i++)
                    {
                        string[] parametrosHash = parametrosGet[i].Split('=');
                        if (parametrosHash[0].Equals("@cd_escola"))
                            cd_empresa = int.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@cd_aluno"))
                            cd_aluno = int.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@cd_produto") && !String.Empty.Equals(parametrosHash[1]))
                            cd_produto = int.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@cd_curso") && !String.Empty.Equals(parametrosHash[1]))
                            cd_curso = int.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@dt_inicial_agend") && !String.Empty.Equals(parametrosHash[1]))
                            dt_inicial_agend = DateTime.Parse(parametrosHash[1] + "");
                        if (parametrosHash[0].Equals("@dt_final_agend") && !String.Empty.Equals(parametrosHash[1]))
                            dt_final_agend = DateTime.Parse(parametrosHash[1] + "");
                        if (parametrosHash[0].Equals("@dt_inicial_lanc") && !String.Empty.Equals(parametrosHash[1]))
                            dt_inicial_lanc = DateTime.Parse(parametrosHash[1] + "");
                        if (parametrosHash[0].Equals("@dt_final_lanc") && !String.Empty.Equals(parametrosHash[1]))
                            dt_final_lanc = DateTime.Parse(parametrosHash[1] + "");
                        if (parametrosHash[0].Equals("@hr_inicial_agend") && !String.Empty.Equals(parametrosHash[1]))
                            hr_inicial_agend = TimeSpan.Parse(parametrosHash[1] + "");
                        if (parametrosHash[0].Equals("@hr_final_agend") && !String.Empty.Equals(parametrosHash[1]))
                            hr_final_agend = TimeSpan.Parse(parametrosHash[1] + "");
                        if (parametrosHash[0].Equals("@hr_inicial_lanc") && !String.Empty.Equals(parametrosHash[1]))
                            hr_inicial_lanc = TimeSpan.Parse(parametrosHash[1] + "");
                        if (parametrosHash[0].Equals("@hr_final_lanc") && !String.Empty.Equals(parametrosHash[1]))
                            hr_final_lanc = TimeSpan.Parse(parametrosHash[1] + "");
                        if (parametrosHash[0].Equals("@no_aluno") && !String.Empty.Equals(parametrosHash[1]))
                            no_aluno = parametrosHash[1] + "";
                        
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
                }

                if (renderizarRelatorio)
                {
                    IEnumerable<FundacaoFisk.SGF.Web.Services.Coordenacao.Model.AulaPersonalizadaReport> aulasPerson = new List<FundacaoFisk.SGF.Web.Services.Coordenacao.Model.AulaPersonalizadaReport>();
                    Hashtable sourceHash = new Hashtable();
                    FundacaoFisk.SGF.Web.Services.ApresentadorRelatorio.RelatorioController apresentadorRelatorio = new FundacaoFisk.SGF.Web.Services.ApresentadorRelatorio.RelatorioController();
                    var caminhoRelatorio = FundacaoFisk.SGF.Utils.ReportParameter.RELATORIO_AULA_PERSONALIZADA;

                    aulasPerson = apresentadorRelatorio.getReportAulaPersonalizada(cd_empresa, cd_aluno, cd_produto, cd_curso, dt_inicial_agend, dt_final_agend, dt_inicial_lanc,
                        dt_final_lanc, hr_inicial_agend, hr_final_agend, hr_inicial_lanc, hr_final_lanc).ToList();

                    no_turma_original = String.Join(", ", aulasPerson.Select(ap => ap.no_turma_original).Distinct());
                    
                    sourceHash["DataSetReport"] = aulasPerson;
                    parametrosRelatorio.Add(Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO, "Relatório de Aula Personalizada");

                    parametrosRelatorio.Add("PAluno", no_aluno);
                    parametrosRelatorio.Add("PTurma", no_turma_original);
                    if ((dt_inicial_agend != null && dt_final_agend != DateTime.MinValue) || (dt_final_agend != null && dt_final_agend != DateTime.MinValue))
                        parametrosRelatorio.Add("PPeriodo", "Período de " + String.Format("{0:dd/MM/yyyy}", dt_inicial_agend) + " à " + String.Format("{0:dd/MM/yyyy}", dt_final_agend));
                    System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("pt-BR");

                    if (aulasPerson != null && aulasPerson.Count() > 0)
                        relatorio.ExibirRelatorioEspecializado(ConfigurationManager.AppSettings["caminhoRelatorio"] + "", caminhoRelatorio, sourceHash, parametrosRelatorio);
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

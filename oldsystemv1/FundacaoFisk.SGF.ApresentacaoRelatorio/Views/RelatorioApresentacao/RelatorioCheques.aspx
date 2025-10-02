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
     
            private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("RelatorioCheques.aspx");

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
            //    int cd_pessoa_aluno, int cd_banco, string emitente, bool liquidados, string nm_cheques, string vl_titulo, int nm_agencia,
            //int nm_ccorrente, string dt_ini_bPara, string dt_fim_bPara, string dt_ini, string dt_fim, bool emissao, bool liquidacao
                Hashtable parametrosPesquisa = new Hashtable();
                Hashtable parametrosRelatorio = new Hashtable();
                string parms = HttpUtility.UrlDecode(Request.QueryString[Componentes.Utils.ReportParameter.PARAMETROS], System.Text.Encoding.UTF8).Replace(" ", "+");
                
                bool renderizarRelatorio = true;
                int cd_empresa = 0;
                int cd_pessoa_aluno = 0;
                int cd_banco = 0;
                int nm_ccorrente = 0;
                int nm_agencia = 0;
                string emitente = "";
                string nm_cheques = "";
                string vl_titulo = "";
                DateTime? pDtaIbP = null;
                DateTime? pDtaFbP = null;
                DateTime? pDtaI = null;
                DateTime? pDtaF = null;
                bool liquidados = false;
                bool emissao = false;
                bool liquidacao = false;
                string desc_banco = "";
                byte natureza = 0;
                
                if (parms != null)
                {
                    parms = Componentes.Utils.MD5CryptoHelper.descriptografaSenha(parms, Componentes.Utils.MD5CryptoHelper.KEY);
                    parms = HttpUtility.UrlDecode(parms, System.Text.Encoding.UTF8);
                    string[] parametrosGet = parms.Split('&');

                    for (int i = 0; i < parametrosGet.Length; i++)
                    {
                        string[] parametrosHash = parametrosGet[i].Split('=');

                        if (parametrosHash[0].Equals("@cd_pessoa_aluno"))
                            cd_pessoa_aluno = int.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@cdEscola"))
                            cd_empresa = int.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@cd_banco"))
                            cd_banco = int.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@nm_ccorrente"))
                            nm_ccorrente = int.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@nm_agencia"))
                            nm_agencia = int.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@emitente"))
                            emitente = (parametrosHash[1]) + "";
                        if (parametrosHash[0].Equals("@nm_cheques"))
                            nm_cheques = (parametrosHash[1]) + "";
                        if (parametrosHash[0].Equals("@vl_titulo"))
                            vl_titulo = (parametrosHash[1]) + "";
                        if (parametrosHash[0].Equals("@dt_ini_bPara") && parametrosHash[1] != "")
                            pDtaIbP = DateTime.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@dt_fim_bPara") && parametrosHash[1] != "")
                            pDtaFbP = DateTime.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@dt_ini") && parametrosHash[1] != "")
                            pDtaI = DateTime.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@dt_fim") && parametrosHash[1] != "")
                            pDtaF = DateTime.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@liquidados"))
                            liquidados = bool.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@emissao"))
                            emissao = bool.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@liquidacao"))
                            liquidacao = bool.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@natureza"))
                            natureza = byte.Parse(parametrosHash[1]);
                         if (parametrosHash[0].Equals("@displayBanco"))
                            desc_banco = (parametrosHash[1]) + "";
                        if (!parametrosHash[0].StartsWith("@"))
                            parametrosRelatorio.Add(parametrosHash[0], parametrosHash[1]);
                    }
                }
              

                    if (renderizarRelatorio)
                    {
                        FundacaoFisk.SGF.Web.Services.ApresentadorRelatorio.RelatorioController apresentadorRelatorio = new FundacaoFisk.SGF.Web.Services.ApresentadorRelatorio.RelatorioController();

                        List<Componentes.GenericModel.TO> sourceTO;
                        List<FundacaoFisk.SGF.Web.Services.Financeiro.Model.ChequeUI> carnes = new List<FundacaoFisk.SGF.Web.Services.Financeiro.Model.ChequeUI>();
                        if (liquidados)
                            carnes = apresentadorRelatorio.getRptChequesLiquidados(cd_empresa, cd_pessoa_aluno, cd_banco, emitente, liquidados,
                                nm_cheques, vl_titulo, nm_agencia, nm_ccorrente, pDtaIbP, pDtaFbP, pDtaI, pDtaF, emissao, liquidacao, natureza);
                        else
                            carnes = apresentadorRelatorio.getRptChequesAbertos(cd_empresa, cd_pessoa_aluno, cd_banco, emitente, liquidados,
                                nm_cheques, vl_titulo, nm_agencia, nm_ccorrente, pDtaIbP, pDtaFbP, pDtaI, pDtaF, emissao, liquidacao, natureza);

                        var nomeRelatorio = FundacaoFisk.SGF.Utils.ReportParameter.RELATORIO_CHEQUES;
                        
                     //   string tituloRel = "Carnê";

                        // Inclui os parâmetros especializados do relatório:
                        parametrosRelatorio.Add("PLiquidados", liquidados);
                        if (pDtaIbP != null || pDtaFbP != null)
                            parametrosRelatorio.Add("PPeriodoBomPara", "Período de bom para " + String.Format("{0:dd/MM/yyyy}", pDtaIbP) + " a " + String.Format("{0:dd/MM/yyyy}", pDtaFbP));
                        if (pDtaI != null || pDtaF != null)
                        {
                            if (!liquidados)
                                parametrosRelatorio.Add("PPeriodoEmissao", "Período de emissão " + String.Format("{0:dd/MM/yyyy}", pDtaI) + " a " + String.Format("{0:dd/MM/yyyy}", pDtaF));
                            else
                                parametrosRelatorio.Add("PPeriodoEmissao", "Período de liquidação  " + String.Format("{0:dd/MM/yyyy}", pDtaI) + " a " + String.Format("{0:dd/MM/yyyy}", pDtaF));
                        }
                        if (liquidados)
                            parametrosRelatorio.Add(Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO, "Relação de cheques liquidados");
                        else
                            parametrosRelatorio.Add(Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO, "Relação de cheques emitidos");
                        sourceTO = carnes.ToList<Componentes.GenericModel.TO>();
                        parametrosRelatorio.Add("PNatureza", natureza > 0 ? natureza == 1 ? "Receber" : "Pagar" : "");
                        parametrosRelatorio.Add("PBanco", cd_banco > 0 ? desc_banco : "");
                        if (sourceTO != null && sourceTO.Count > 0)
                            relatorio.ExibirRelatorioEspecializado(ConfigurationManager.AppSettings["caminhoRelatorio"] + "", nomeRelatorio, sourceTO, parametrosRelatorio);
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

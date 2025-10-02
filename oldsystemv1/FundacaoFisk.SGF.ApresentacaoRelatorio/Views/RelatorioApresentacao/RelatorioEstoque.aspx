<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Register Assembly="Componentes.ApresentadorRelatorio" Namespace="Componentes.ApresentadorRelatorio" TagPrefix="cc1" %>

<!DOCTYPE html>

<html>
<head id="Head1" runat="server">
    <title>Relatório</title>
</head>
<body>
    <div>
     <script runat="server">
     
            private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("RelatorioEstoque.aspx");           

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
                
                bool renderizarRelatorio = true;
                DateTime pDtaI = new DateTime();
                DateTime pDtaF = new DateTime();
                int cd_tipo_relatorio = 0;
                int cdEscola = 0;
                int cd_item = 0;
                int cd_grupo = 0;
                byte tipo = 0;
                byte tipoRelatorio = 0;
                string dc_item = "";
                string dc_grupo = "";
                string escola = "";
                string relatorioDesc = "";

                //Parametros para relatório de item
                int ano = 0;
                int mes = 0;
                bool isContado = false;
                bool isContagem = false;
                bool isSemColunaC = false;
                bool isApenasItensMovimento = false;
                
                if (parms != null)
                {
                    parms = Componentes.Utils.MD5CryptoHelper.descriptografaSenha(parms, Componentes.Utils.MD5CryptoHelper.KEY);
                    parms = HttpUtility.UrlDecode(parms, System.Text.Encoding.UTF8);
                    string[] parametrosGet = parms.Split('&');

                    for (int i = 0; i < parametrosGet.Length; i++)
                    {
                        string[] parametrosHash = parametrosGet[i].Split('=');

                        if (parametrosHash[0].Equals("@pRelatorio"))
                            cd_tipo_relatorio = int.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@cdEscola"))
                            cdEscola = int.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@pDtaI"))
                            pDtaI = DateTime.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@pDtaF"))
                            pDtaF = DateTime.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("PEmpresa"))
                            escola = parametrosHash[1];
                        if (parametrosHash[0].Equals("@pDcGrupo"))
                            dc_grupo = parametrosHash[1];
                        if (parametrosHash[0].Equals("@pDcGrupo"))
                            dc_item = parametrosHash[1];
                        if (parametrosHash[0].Equals("@pTipoRelatorio"))
                            tipoRelatorio = byte.Parse(parametrosHash[1]);// sintético ou analítico
                        if (parametrosHash[0].Equals("@pTipoItem"))
                            tipo = byte.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@pNoRelatorio"))
                            relatorioDesc = parametrosHash[1];
                        if (parametrosHash[0].Equals("@pGrupoItem"))
                            cd_grupo = int.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@pdcItem"))
                            dc_item = parametrosHash[1];  
                        
                        //Parametros para relatório de item
                        if (parametrosHash[0].Equals("@pAno"))
                            ano = int.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@pMes"))
                            mes = int.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@pContagem"))
                            isContagem = Boolean.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@pContado"))
                            isContado = Boolean.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@pItem"))
                            cd_item = int.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@isSemColunaC"))
                            isSemColunaC = Boolean.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@pisApenasItensMovimento"))
                            isApenasItensMovimento = Boolean.Parse(parametrosHash[1]);
                        
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

                        List<Componentes.GenericModel.TO> sourceTO;
                        string nomeRelatorio = "";
                        dynamic obj = new Object();
                        string tituloRel = relatorioDesc;

                        parametrosRelatorio.Add("PGrupo", "Grupo:" + dc_grupo);
                        parametrosRelatorio.Add("PItem", "Item:" + dc_item);
                      
                        switch (cd_tipo_relatorio)
                        {
                            case (int)FundacaoFisk.SGF.Web.Services.Financeiro.Model.EstoqueUI.relatorioEstoque.MOVIMENTOS:
                                {
                                    IEnumerable<FundacaoFisk.SGF.Web.Services.Financeiro.Model.KardexUI> kardexUI = apresentadorRelatorio.st_Rptkardex(cdEscola, cd_item, pDtaI, pDtaF, cd_grupo, tipo, isApenasItensMovimento).ToList();
                                    nomeRelatorio = FundacaoFisk.SGF.Utils.ReportParameter.RELATORIO_ESTOQUE;
                                    obj = kardexUI.ToList<Componentes.GenericModel.TO>();

                                    // Inclui os parâmetros especializados do relatório:
                                    parametrosRelatorio.Add("PPeriodo", "Período de " + String.Format("{0:dd/MM/yyyy}", pDtaI) + " à " + String.Format("{0:dd/MM/yyyy}", pDtaF));
                                    parametrosRelatorio.Add("PSintetico", tipoRelatorio == 0 ? false : true);
                                    parametrosRelatorio.Add("PNomeRelatorio", "RptKardex.rdlc");

                                    break;
                                }
                            case (int)FundacaoFisk.SGF.Web.Services.Financeiro.Model.EstoqueUI.relatorioEstoque.ITENS:
                                {
                                    if (isSemColunaC) isContagem = true;
                                    IEnumerable<FundacaoFisk.SGF.Web.Services.Financeiro.Model.RptItemFechamento> itemFechamento = apresentadorRelatorio.rptItemWithSaldoItem(cdEscola, cd_item, cd_grupo, tipo, ano, mes, isContagem).ToList();
                                    nomeRelatorio = FundacaoFisk.SGF.Utils.ReportParameter.RELATORIO_ESTOQUE_ITEM;
                                    obj = itemFechamento.ToList<Componentes.GenericModel.TO>();
                                    parametrosRelatorio.Add("PAno", ano);
                                    parametrosRelatorio.Add("PMes", mes);
                                    parametrosRelatorio.Add("PContagem", isContagem);
                                    parametrosRelatorio.Add("PContado", isContado);
                                    parametrosRelatorio.Add("PSemColunaC", isSemColunaC);
                                    parametrosRelatorio.Add("PNomeRelatorio", "RptListaItem.rdlc");
                                    break;
                                }
                            case (int)FundacaoFisk.SGF.Web.Services.Financeiro.Model.EstoqueUI.relatorioEstoque.SALDO_REAL_BIBLIOTECA:
                                {
                                    IEnumerable<FundacaoFisk.SGF.Web.Services.Financeiro.Model.RptItemFechamento> rptSaldoBiblioteca = apresentadorRelatorio.rptItemSaldoBiblioteca(cdEscola, cd_item, cd_grupo, tipo).ToList();
                                    nomeRelatorio = FundacaoFisk.SGF.Utils.ReportParameter.RELATORIO_ESTOQUE_BIBLIOTECA;
                                    obj = rptSaldoBiblioteca.ToList<Componentes.GenericModel.TO>();
                                    parametrosRelatorio.Add("PNomeRelatorio", "RptBiblioteca.rdlc");
                                    break;
                                }

                        }
                        parametrosRelatorio.Add(Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO, tituloRel);
                        sourceTO = obj;

                        if (sourceTO != null && sourceTO.Count > 0)
                        {
                            abortou_requisicao = true;
                            relatorio.ExibirRelatorioEspecializado(ConfigurationManager.AppSettings["caminhoRelatorio"] + "", nomeRelatorio, sourceTO, parametrosRelatorio);
                        }
                        else
                        {
                            //url = Request.QueryString["enderecoWeb"] != null ? HttpUtility.UrlDecode(Request.QueryString["enderecoWeb"], System.Text.Encoding.UTF8) + "/Erro" : ConfigurationManager.AppSettings["enderecoRetornoErro"];
                            url = FundacaoFisk.SGF.ApresentacaoRelatorio.comum.UrlErro.ObterUrlErroRelatorio(url);
                            if (!abortou_requisicao)
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

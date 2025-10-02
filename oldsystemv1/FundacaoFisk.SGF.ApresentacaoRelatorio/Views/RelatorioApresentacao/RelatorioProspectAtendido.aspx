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
     
         private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("RelatorioProspectAtendido.aspx");

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

            private void bindRelatorio(string url, ref bool abortou_requisicao) { 
                
                Hashtable parametrosPesquisa = new Hashtable();
                Hashtable parametrosRelatorio = new Hashtable();
                string parms = HttpUtility.UrlDecode(Request.QueryString[Componentes.Utils.ReportParameter.PARAMETROS], System.Text.Encoding.UTF8).Replace(" ", "+");
                
                bool renderizarRelatorio = true;
                DateTime dataBase = new DateTime(); ;
                int cdEscola = 0;
                int cdMotivoNaoMatricula = 0;
                int cFuncionario = 0;
                int cdProduto = 0;
                DateTime? pDtaI = null;
                DateTime? pDtaF = null;
                DateTime? pDtaIComp = null;
                DateTime? pDtaFComp = null;
                string periodos = "";
                List<int> cd_periodos = new List<int>();
                byte tipoRelatorio = 0;
                int cd_midia = 0;
                int cd_faixa_etaria = 0;
                
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
                        if (parametrosHash[0].Equals("@DataBase"))
                            dataBase = DateTime.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@cdMotivoNaoMatricula"))
                            cdMotivoNaoMatricula = int.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@cFuncionario"))
                            cFuncionario = int.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@cdProduto"))
                            cdProduto = int.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@cd_midia"))
                            cd_midia = int.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@cd_produtos"))
                            periodos = parametrosHash[1];
                        if (parametrosHash[0].Equals("@tipo"))
                            tipoRelatorio = byte.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@pDtaI") && parametrosHash[1] != "")
                            pDtaI = DateTime.Parse(parametrosHash[1] );
                        if (parametrosHash[0].Equals("@pDtaF") && parametrosHash[1] != "")
                            pDtaF = DateTime.Parse(parametrosHash[1] );
                        if (parametrosHash[0].Equals("@pDtaIComp") && parametrosHash[1] != "")
                            pDtaIComp = DateTime.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@pDtaFComp") && parametrosHash[1] != "")
                            pDtaFComp = DateTime.Parse(parametrosHash[1]);
                        if (!parametrosHash[0].StartsWith("@"))
                            parametrosRelatorio.Add(parametrosHash[0], parametrosHash[1]);
                        if (parametrosHash[0].Equals("@cd_faixa_etaria"))
                            cd_faixa_etaria = int.Parse(parametrosHash[1]);
                    }

                    if (!string.IsNullOrEmpty(periodos))
                    {
                        cd_periodos = periodos.Split(',').Select(x=> int.Parse(x)).ToList();
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
                        IEnumerable<FundacaoFisk.SGF.Web.Services.Secretaria.Model.ReportProspect> prospects = new List<FundacaoFisk.SGF.Web.Services.Secretaria.Model.ReportProspect>();
                        List<FundacaoFisk.SGF.Web.Services.Secretaria.Model.ReportProspect> lista = new List<FundacaoFisk.SGF.Web.Services.Secretaria.Model.ReportProspect>();
                        Hashtable sourceHash = new Hashtable();
                        FundacaoFisk.SGF.Web.Services.ApresentadorRelatorio.RelatorioController apresentadorRelatorio = new FundacaoFisk.SGF.Web.Services.ApresentadorRelatorio.RelatorioController();
                        var caminhoRelatorio = "";
                        switch (tipoRelatorio)
                        {
                            case (byte)FundacaoFisk.SGF.GenericModel.Prospect.TipoRelatorio.PROSPECTS_ATENDIDOS:
                                caminhoRelatorio = FundacaoFisk.SGF.Utils.ReportParameter.RELATORIO_PROSPECT_ATENDIDO;
                                prospects = apresentadorRelatorio.getRptProspectAtendido(cdEscola, cdMotivoNaoMatricula, cFuncionario, cdProduto, pDtaI, pDtaF, cd_midia, cd_periodos, cd_faixa_etaria).ToList();
                                sourceHash["DataSetReportProspect"] = prospects;
                                parametrosRelatorio.Add(Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO, "Relatório de Prospects Atendidos");
                                break;
                            case (byte)FundacaoFisk.SGF.GenericModel.Prospect.TipoRelatorio.PROSPECTS_ATENDIDOS_MOTIVO_NAO_MATRICULA:
                                caminhoRelatorio = FundacaoFisk.SGF.Utils.ReportParameter.RELATORIO_PROSPECT_ATENDIDO_MOTIVO_NAO_MATRICULA;
                                parametrosRelatorio.Add(Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO, "Relatório de Prospects Atendidos por Motivo de Não Matrícula");
                                List<FundacaoFisk.SGF.GenericModel.MotivoNaoMatricula> motivos = new List<FundacaoFisk.SGF.GenericModel.MotivoNaoMatricula>();
                                prospects = apresentadorRelatorio.getRptProspectAtendido(cdEscola, cdMotivoNaoMatricula, cFuncionario, cdProduto, pDtaI, pDtaF, cd_midia, cd_periodos, cd_faixa_etaria).ToList();
                                sourceHash["DataSetReportProspect"] = prospects;
                                foreach (FundacaoFisk.SGF.Web.Services.Secretaria.Model.ReportProspect p in prospects)
                                    if (p.listaMotivosNaoMatricula != null && p.listaMotivosNaoMatricula.Count() > 0)
                                        motivos = motivos.Concat(p.listaMotivosNaoMatricula).ToList();
                                var listaMtvNMat = motivos.GroupBy(x => new { x.cd_motivo_nao_matricula, x.dc_motivo_nao_matricula}).Select(g => new FundacaoFisk.SGF.GenericModel.MotivoNaoMatricula
                                {
                                    dc_motivo_nao_matricula = g.Key.dc_motivo_nao_matricula,
                                    qtd_motivo = motivos.Count(x => x.cd_motivo_nao_matricula == g.Key.cd_motivo_nao_matricula),
                                    perc_motivo = (((double)motivos.Where(x => x.cd_motivo_nao_matricula == g.Key.cd_motivo_nao_matricula).Count() * 100) / (double)motivos.Count())
                                });
                                sourceHash["DataSetReportMtvNMatriculas"] = listaMtvNMat;
                                break;
                            case (byte)FundacaoFisk.SGF.GenericModel.Prospect.TipoRelatorio.PROSPECT_ATENDIDOS_MATRÍCULAS:
                                caminhoRelatorio = FundacaoFisk.SGF.Utils.ReportParameter.RELATORIO_PROSPECT_ATENDIDO_X_MATRICULA;
                                prospects = apresentadorRelatorio.getProspectAtendidoMatricula(cdEscola, cdMotivoNaoMatricula, cFuncionario, cdProduto, pDtaI, pDtaF,cd_midia, cd_periodos).ToList();
                                sourceHash["DataSetReportProspect"] = prospects;
                                parametrosRelatorio.Add(Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO, "Relatório de Prospects Atendidos x Matrículas");
                                break;
                            case (byte)FundacaoFisk.SGF.GenericModel.Prospect.TipoRelatorio.COMPARATIVO_PROSPECTS_ATENDIDOS:
                                caminhoRelatorio = FundacaoFisk.SGF.Utils.ReportParameter.RELATORIO_COMPARATIVO_PROSPECTS_ATENDIDOS_PERIODOS;
                                List<FundacaoFisk.SGF.Web.Services.Secretaria.Model.ReportProspect> prospectPeriodo = apresentadorRelatorio.getComparativoProspectAtendido(cdEscola, cdMotivoNaoMatricula, cFuncionario, cdProduto, pDtaI, pDtaF,cd_midia, cd_periodos).ToList();
                                List<FundacaoFisk.SGF.Web.Services.Secretaria.Model.ReportProspect> prospectCompPeriodo = apresentadorRelatorio.getComparativoProspectAtendido(cdEscola, cdMotivoNaoMatricula, cFuncionario, cdProduto, pDtaIComp, pDtaFComp,cd_midia, cd_periodos).ToList();
                                lista = FundacaoFisk.SGF.Web.Services.Secretaria.Model.ReportProspect.conversaoDadosComparativoProspets(prospectPeriodo, prospectCompPeriodo);
                                parametrosRelatorio.Add("PDataPrimeiraComp", String.Format("{0:dd/MM/yyyy}", pDtaI) + " à " + String.Format("{0:dd/MM/yyyy}", pDtaF));
                                parametrosRelatorio.Add("PDataSegundaComp", String.Format("{0:dd/MM/yyyy}", pDtaIComp) + " à " + String.Format("{0:dd/MM/yyyy}", pDtaFComp));
                                parametrosRelatorio.Add("PPeriodo", "Período de " + String.Format("{0:dd/MM/yyyy}", pDtaI) + " à " + String.Format("{0:dd/MM/yyyy}", pDtaF) +" e " +
                                                          String.Format("{0:dd/MM/yyyy}", pDtaIComp) + " à " + String.Format("{0:dd/MM/yyyy}", pDtaFComp));
                                sourceHash["DataSetReportProspect"] = lista;
                                parametrosRelatorio.Add(Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO, "Relatório Comparativo de Prospects Atendidos por Períodos");
                                break;
                        }
                        parametrosRelatorio.Add("DataBase", dataBase);
                        //parametrosRelatorio.Add("PGradeSaldoLiquidacao", tipoLiquidacao);
                        if ( (tipoRelatorio != (byte)FundacaoFisk.SGF.GenericModel.Prospect.TipoRelatorio.COMPARATIVO_PROSPECTS_ATENDIDOS) &&( 
                            (pDtaI != null && pDtaI != DateTime.MinValue) || (pDtaF != null && pDtaF != DateTime.MinValue)))
                            parametrosRelatorio.Add("PPeriodo", "Período de " + String.Format("{0:dd/MM/yyyy}", pDtaI) + " à " + String.Format("{0:dd/MM/yyyy}", pDtaF));
                        if ((tipoRelatorio == (byte) FundacaoFisk.SGF.GenericModel.Prospect.TipoRelatorio.COMPARATIVO_PROSPECTS_ATENDIDOS) && (
                            (pDtaI != null && pDtaI != DateTime.MinValue) || (pDtaF != null && pDtaF != DateTime.MinValue)) && lista != null && lista.Count() > 0)
                        {
                            abortou_requisicao = true;
                            relatorio.ExibirRelatorioEspecializado(ConfigurationManager.AppSettings["caminhoRelatorio"] + "", caminhoRelatorio, sourceHash, parametrosRelatorio);
                        }
                        if (prospects != null && prospects.Count() > 0){
                            abortou_requisicao = true;
                            relatorio.ExibirRelatorioEspecializado(ConfigurationManager.AppSettings["caminhoRelatorio"] + "", caminhoRelatorio, sourceHash, parametrosRelatorio);
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

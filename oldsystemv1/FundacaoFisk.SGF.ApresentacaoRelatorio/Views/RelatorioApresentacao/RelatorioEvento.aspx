<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Register Assembly="Componentes.ApresentadorRelatorio" Namespace="Componentes.ApresentadorRelatorio" TagPrefix="cc1" %>
<!DOCTYPE html>

<html>
<head runat="server">
    <title>Relatório</title>
</head>
<body>
    <div style="Width:100%;height:100%;">
        <script runat="server">
            private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("RelatorioEvento.aspx");
            private enum TipoRelatorio {
                DIARIO_AULA = 1,
                EVENTOS = 2,
                DIARIO_AULA_PROGRAMACOES = 3
            };
            
            public void Page_Load(object sender, System.EventArgs e)
            {
                string url = Request.QueryString["enderecoWeb"] != null ? HttpUtility.UrlDecode(Request.QueryString["enderecoWeb"], System.Text.Encoding.UTF8) : ConfigurationManager.AppSettings["enderecoRetornoErro"];
                try {
                    if(!IsPostBack)
                        bindRelatorio(url);
                }
                catch(Componentes.GenericBusiness.BusinessException exe) {
                    RedirecionaAplicacaoPrincipalNegocio(url, exe);
                }
                catch(Exception exe) {
                    RedirecionaAplicacaoPrincipal(url, exe);
                }
            }

            private void RedirecionaAplicacaoPrincipalNegocio(string url, Exception exe)
            {
                try
                {
                    //url = Request.QueryString["enderecoWeb"] != null ? HttpUtility.UrlDecode(Request.QueryString["enderecoWeb"], System.Text.Encoding.UTF8) + "/Erro" : ConfigurationManager.AppSettings["enderecoRetornoErro"];
                    url = FundacaoFisk.SGF.ApresentacaoRelatorio.comum.UrlErro.ObterUrlErroRelatorio(url);
                    Response.Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(exe.Message) + "&stackTrace=");
                }
                catch (Exception e)
                {
                    logger.Error(FundacaoFisk.SGF.Utils.Messages.Messages.msgEmissaoRelatorioError + " url: " + url, e);
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

                int? cd_turma = null;
                int cd_escola = 0;
                int? cd_professor = null;
                int? cd_evento = null;
                int? qtd_faltas = null;
                bool falta_consecultiva = false;
                bool mais_turma_pagina = false;
                DateTime? dt_inicial = null;
                DateTime? dt_final = null;
                bool renderizarRelatorio = true;
                byte tipo_relatorio = 2;
                bool lancada = false;
                bool infoPresenca = false;
                
                if(parms != null) {
                    parms = Componentes.Utils.MD5CryptoHelper.descriptografaSenha(parms, Componentes.Utils.MD5CryptoHelper.KEY);
                    parms = HttpUtility.UrlDecode(parms, System.Text.Encoding.UTF8);
                    string[] parametrosGet = parms.Split('&');
                    
                    for(int i = 0; i < parametrosGet.Length; i++) {
                        string[] parametrosHash = parametrosGet[i].Split('=');

                        if(parametrosHash[0].Equals("@cd_escola") && !string.IsNullOrEmpty(parametrosHash[1]))
                            cd_escola = int.Parse(parametrosHash[1]);
                        if(parametrosHash[0].Equals("@cd_turma") && !string.IsNullOrEmpty(parametrosHash[1]))
                            cd_turma = int.Parse(parametrosHash[1]);
                        if(parametrosHash[0].Equals("@cd_professor") && !string.IsNullOrEmpty(parametrosHash[1]))
                            cd_professor = int.Parse(parametrosHash[1]);
                        if(parametrosHash[0].Equals("@cd_evento") && !string.IsNullOrEmpty(parametrosHash[1]))
                            cd_evento = int.Parse(parametrosHash[1]);
                        if(parametrosHash[0].Equals("@qtd_faltas") && !string.IsNullOrEmpty(parametrosHash[1]))
                            qtd_faltas = int.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("falta_consecultiva"))
                            falta_consecultiva = bool.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("mais_turma_pagina"))
                            mais_turma_pagina = !bool.Parse(parametrosHash[1]);
                        if(parametrosHash[0].Equals("@dt_inicial") && !string.IsNullOrEmpty(parametrosHash[1]))
                            dt_inicial = DateTime.Parse(parametrosHash[1]);
                        if(parametrosHash[0].Equals("@dt_final") && !string.IsNullOrEmpty(parametrosHash[1]))
                            dt_final = DateTime.Parse(parametrosHash[1]);
                        if(parametrosHash[0].Equals("@tipo_relatorio") && !string.IsNullOrEmpty(parametrosHash[1]))
                            tipo_relatorio = byte.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@lancada"))
                            lancada = bool.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@PInfoPresenca"))
                            infoPresenca = bool.Parse(parametrosHash[1]);

                        if (!parametrosHash[0].StartsWith("@"))
                            parametrosRelatorio.Add(parametrosHash[0], parametrosHash[1]);
                    }
                    
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
                    // Adicionando parametros adicionais:
                    parametrosRelatorio.Add("PPeriodo", "Período de " + String.Format("{0:dd/MM/yyyy}", dt_inicial) + " à " + String.Format("{0:dd/MM/yyyy}", dt_final));
                    
                    switch(tipo_relatorio){
                        case (byte) TipoRelatorio.DIARIO_AULA:
                            parametrosRelatorio.Add(Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO, "Relatório de Diário de Aulas das Turmas");
                            parametrosRelatorio.Add("PInfoPresenca", infoPresenca);
                            break;
                        case (byte) TipoRelatorio.EVENTOS:
                            parametrosRelatorio.Add(Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO, "Relatório de Eventos");
                            break;
                        case (byte) TipoRelatorio.DIARIO_AULA_PROGRAMACOES:
                            parametrosRelatorio.Add(Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO, "Relatório de Programação das Aulas das Turmas");
                            break;
                    }
                
                    if (renderizarRelatorio)
                    {
                        FundacaoFisk.SGF.Web.Services.ApresentadorRelatorio.RelatorioController apresentadorRelatorio = new FundacaoFisk.SGF.Web.Services.ApresentadorRelatorio.RelatorioController();

                        List<Componentes.GenericModel.TO> sourceTO;

                        switch (tipo_relatorio)
                        {
                            case (byte)TipoRelatorio.DIARIO_AULA:
                                Hashtable sourceHash = new Hashtable();
                                List<Componentes.GenericModel.TO> listAlunos = apresentadorRelatorio.getAlunosTurmaAtivosDiarioAulaReport(cd_turma.Value, cd_escola, dt_inicial, dt_final.Value).ToList<Componentes.GenericModel.TO>();
                                sourceHash["DataSetReport"] = apresentadorRelatorio.getRelatorioDiarioAula(cd_escola, cd_turma.Value, cd_professor.Value, dt_inicial.Value, dt_final.Value).ToList<Componentes.GenericModel.TO>();
                                sourceHash["DataSetReportProgramacoes"] = apresentadorRelatorio.getProgramacoesTurma(cd_escola, cd_turma.Value, cd_professor.Value, dt_inicial, dt_final.Value, infoPresenca, FundacaoFisk.SGF.GenericModel.AlunoTurma.FiltroSituacaoAlunoTurma.Todos).ToList<Componentes.GenericModel.TO>();
                                sourceHash["DataSetReportProgramacoesVisto"] = apresentadorRelatorio.getProgramacoesTurmaVisto(cd_escola, cd_turma.Value, cd_professor.Value, dt_inicial, dt_final.Value).ToList<Componentes.GenericModel.TO>();
                                sourceHash["DataSetReportAlunos"] = listAlunos;
                                
                                string nomeRelatorio = FundacaoFisk.SGF.Utils.ReportParameter.RELATORIO_DIARIO_AULA;
                                if(listAlunos!= null && listAlunos.Count() > 2)
                                    nomeRelatorio = FundacaoFisk.SGF.Utils.ReportParameter.RELATORIO_DIARIO_AULA_QUEBRA;                    
                                
                                if(sourceHash != null && sourceHash.Values.Count > 0)
                                    relatorio.ExibirRelatorioEspecializado(ConfigurationManager.AppSettings["caminhoRelatorio"] + "", nomeRelatorio, sourceHash, parametrosRelatorio);
                                else {
                                    //url = Request.QueryString["enderecoWeb"] != null ? HttpUtility.UrlDecode(Request.QueryString["enderecoWeb"], System.Text.Encoding.UTF8) + "/Erro" : ConfigurationManager.AppSettings["enderecoRetornoErro"];
                                    url = FundacaoFisk.SGF.ApresentacaoRelatorio.comum.UrlErro.ObterUrlErroRelatorio(url);
                                    Response.Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(Componentes.Utils.Messages.Messages.msgRegNotEnc) + "&stackTrace=");
                                }
                                break;
                            case (byte)TipoRelatorio.EVENTOS:
                                sourceTO = apresentadorRelatorio.getRelatorioEventos(cd_escola, cd_turma, cd_professor, cd_evento, qtd_faltas, falta_consecultiva, mais_turma_pagina,
                                    dt_inicial, dt_final).ToList<Componentes.GenericModel.TO>();

                                nomeRelatorio = FundacaoFisk.SGF.Utils.ReportParameter.RELATORIO_EVENTOS;

                                if(mais_turma_pagina)
                                    nomeRelatorio = FundacaoFisk.SGF.Utils.ReportParameter.RELATORIO_EVENTOS_QUEBRA_TURMA;
                            
                                if(sourceTO != null && sourceTO.Count > 0)
                                    relatorio.ExibirRelatorioEspecializado(ConfigurationManager.AppSettings["caminhoRelatorio"] + "", nomeRelatorio, sourceTO, parametrosRelatorio);
                                else {
                                    //url = Request.QueryString["enderecoWeb"] != null ? HttpUtility.UrlDecode(Request.QueryString["enderecoWeb"], System.Text.Encoding.UTF8) + "/Erro" : ConfigurationManager.AppSettings["enderecoRetornoErro"];
                                    url = FundacaoFisk.SGF.ApresentacaoRelatorio.comum.UrlErro.ObterUrlErroRelatorio(url);
                                    Response.Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(Componentes.Utils.Messages.Messages.msgRegNotEnc) + "&stackTrace=");
                                }
                                break;
                            case (byte)TipoRelatorio.DIARIO_AULA_PROGRAMACOES:
                                sourceTO = apresentadorRelatorio.getRelatorioDiarioAulaProgramacoes(cd_escola, cd_turma, mais_turma_pagina, dt_inicial, dt_final, lancada).ToList<Componentes.GenericModel.TO>();
                                if (sourceTO != null && sourceTO.Count > 0)
                                {
                                    if(!mais_turma_pagina)
                                        relatorio.ExibirRelatorioEspecializado(ConfigurationManager.AppSettings["caminhoRelatorio"] + "", FundacaoFisk.SGF.Utils.ReportParameter.RELATORIO_DIARIO_AULA_PROGRAMACAO, sourceTO, parametrosRelatorio);
                                    else
                                        relatorio.ExibirRelatorioEspecializado(ConfigurationManager.AppSettings["caminhoRelatorio"] + "", FundacaoFisk.SGF.Utils.ReportParameter.RELATORIO_DIARIO_AULA_PROGRAMACAO_QUEBRA_TURMA, sourceTO, parametrosRelatorio);
                                }
                                else
                                {
                                    //url = Request.QueryString["enderecoWeb"] != null ? HttpUtility.UrlDecode(Request.QueryString["enderecoWeb"], System.Text.Encoding.UTF8) + "/Erro" : ConfigurationManager.AppSettings["enderecoRetornoErro"];
                                    url = FundacaoFisk.SGF.ApresentacaoRelatorio.comum.UrlErro.ObterUrlErroRelatorio(url);
                                    Response.Redirect(url + "?msgErro=" + HttpUtility.UrlEncode(Componentes.Utils.Messages.Messages.msgRegNotEnc) + "&stackTrace=");
                                }
                                break;
                        }                       
                    }
                }
                else
                    throw new Exception(Componentes.Utils.Messages.Messages.msgSessaoExpirada);
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
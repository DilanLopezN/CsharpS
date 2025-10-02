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
            private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("RelatorioComissaoSecretarias.aspx");
            
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
                int cd_funcionario = 0;
                int cd_produto = 0;
                int cd_escola = 0;
                DateTime? dt_inicial = null;
                DateTime? dt_final = null;
                bool renderizarRelatorio = true;
                
                if(parms != null) {
                    parms = Componentes.Utils.MD5CryptoHelper.descriptografaSenha(parms, Componentes.Utils.MD5CryptoHelper.KEY);
                    parms = HttpUtility.UrlDecode(parms, System.Text.Encoding.UTF8);
                    string[] parametrosGet = parms.Split('&');
                    
                    for(int i = 0; i < parametrosGet.Length; i++) {
                        string[] parametrosHash = parametrosGet[i].Split('=');
                        if (parametrosHash[0].Equals("@cdFunc"))
                            cd_funcionario = int.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@cd_produto"))
                            cd_produto = int.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@cd_escola"))
                            cd_escola = int.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@dtInicial") && !string.IsNullOrEmpty(parametrosHash[1]))
                            dt_inicial = DateTime.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@dtFinal") && !string.IsNullOrEmpty(parametrosHash[1]))
                            dt_final = DateTime.Parse(parametrosHash[1]);
                        if(parametrosHash[0].StartsWith("@"))
                            parametrosPesquisa.Add(parametrosHash[0].Substring(1, parametrosHash[0].Length - 1), parametrosHash[1]);
                        else
                            parametrosRelatorio.Add(parametrosHash[0], parametrosHash[1]);
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
                        List<FundacaoFisk.SGF.GenericModel.FuncionarioComissao> lista = apresentadorRelatorio.getRptComissaoSecretarias(cd_funcionario, cd_produto, cd_escola, dt_inicial, dt_final).ToList();
                        List<Componentes.GenericModel.TO> sourceTO = new List<Componentes.GenericModel.TO>();
                        sourceTO = lista.ToList<Componentes.GenericModel.TO>();
                        parametrosRelatorio.Add("PPeriodo", "Período de " + String.Format("{0:dd/MM/yyyy}", dt_inicial) + " à " + String.Format("{0:dd/MM/yyyy}", dt_final));
                        //parametrosRelatorio.Add("PCdEmpresa", cd_escola);
                        parametrosRelatorio.Add(Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO, "Relatório de Comissão das Secretárias");
                        List<FundacaoFisk.SGF.GenericModel.FuncionarioComissao> listaResumoGeral = new List<FundacaoFisk.SGF.GenericModel.FuncionarioComissao>();
                        //Agrupamento somando os valores por funcionário
                        if (lista != null && lista.Count() > 0)
                        {
                            var listaAgrupado = lista.GroupBy(x => new {x.cd_funcionario, x.no_professor }).Select(g => new FundacaoFisk.SGF.GenericModel.FuncionarioComissao
                            {
                                //cd_produto = g.Key.cd_produto,
                                cd_funcionario = g.Key.cd_funcionario,
                                //no_produto = g.Key.no_produto,
                                no_professor = g.Key.no_professor,
                                qtd_matriculas = g.Sum(y => y.qtd_matriculas),
                                qtd_rematriculas = g.Sum(y => y.qtd_rematriculas),
                                vl_comissao_matricula = g.Sum(y => y.qtd_matriculas * y.vl_comissao_matricula),
                                vl_comissao_rematricula = g.Sum(y => y.qtd_rematriculas * y.vl_comissao_rematricula)
                            });

                            listaResumoGeral = listaAgrupado.ToList();
                            foreach (var geral in listaResumoGeral)
                            {
                                geral.vl_total_geral = sumTotalGeralComissao(geral);                            
                            }
                        }
                        Hashtable sourceHash = new Hashtable();
                        sourceHash["DataSetReport"] = sourceTO;
                        sourceHash["DataSetReportResumoGeral"] = listaResumoGeral.ToList<Componentes.GenericModel.TO>();
                        if (sourceTO != null && sourceTO.ToList<Componentes.GenericModel.TO>().Count > 0)
                        {
                            relatorio.ExibirRelatorioEspecializado(ConfigurationManager.AppSettings["caminhoRelatorio"] + "", FundacaoFisk.SGF.Utils.ReportParameter.RELATORIO_COMISSAO_SECRETARIAS, sourceHash, parametrosRelatorio);
                            //relatorio.LocalReport.SubreportProcessing += new Microsoft.Reporting.WebForms.SubreportProcessingEventHandler(renderizaSubRelatorioAlunos);
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

            private string sumTotalGeralComissao(FundacaoFisk.SGF.GenericModel.FuncionarioComissao funcC) 
            {
                if (funcC.qtd_matriculas > 0 && funcC.qtd_rematriculas > 0)
                    return string.Format("{0:#,0.00}",  funcC.vl_comissao_matricula + funcC.vl_comissao_rematricula);

                if (funcC.qtd_matriculas > 0)
                    return string.Format("{0:#,0.00}", funcC.vl_comissao_matricula);

                if (funcC.qtd_rematriculas > 0)
                    return string.Format("{0:#,0.00}", funcC.vl_comissao_rematricula);
                
                return "0,00";
            }
            
            //private void renderizaSubRelatorioAlunos(object sender, Microsoft.Reporting.WebForms.SubreportProcessingEventArgs e)
            //{
            //    try
            //    {
            //        int cd_turma = Convert.ToInt32(e.Parameters["cd_turma"].Values[0]);
            //        int cd_escola = Convert.ToInt32(e.Parameters["cd_escola"].Values[0]);
            //        FundacaoFisk.SGF.Web.Services.ApresentadorRelatorio.RelatorioController apresentadorRelatorio = new FundacaoFisk.SGF.Web.Services.ApresentadorRelatorio.RelatorioController();

            //        e.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSetReportAlunosEstagio",
            //          apresentadorRelatorio.getRptAlunosProximoCurso(cd_escola, cd_turma)));
            //    }
            //    catch (Exception exe)
            //    {
            //        logger.Error(FundacaoFisk.SGF.Utils.Messages.Messages.msgEmissaoRelatorioError, exe);
            //    }
            //}
            
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
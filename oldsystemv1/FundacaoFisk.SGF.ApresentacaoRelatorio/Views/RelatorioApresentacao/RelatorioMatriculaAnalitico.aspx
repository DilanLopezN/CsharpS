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
     
            private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("RelatorioMatriculaAnalitico.aspx");

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
                
                bool renderizarRelatorio = true;
                int cd_empresa = 0;
                int cd_turma = 0;
                int cd_aluno = 0;
                string situacaoAlunoTurma = "100";
                bool semTurma = false;
                bool tranferencia = false;
                bool retorno = false;
                bool contratoDigitalizado = false;
                bool bolsaCem = false;
                int situacaoContrato = 0;
                int cdAtendente = 0;
                int cdProduto = 0;
                string noProduto = "Todos";
                bool exibirEnderecos = false;
                int vinculado = 0;
                
                DateTime? dtIni = null;
                DateTime? dtFim = null;
                                    
                    
                int tipoRelatorio = 0;
                if (parms != null)
                {
                    parms = Componentes.Utils.MD5CryptoHelper.descriptografaSenha(parms, Componentes.Utils.MD5CryptoHelper.KEY);
                    parms = HttpUtility.UrlDecode(parms, System.Text.Encoding.UTF8);
                    string[] parametrosGet = parms.Split('&');

                    for (int i = 0; i < parametrosGet.Length; i++)
                    {
                        string[] parametrosHash = parametrosGet[i].Split('=');
                        if (parametrosHash[0].Equals("@cd_empresa"))
                            cd_empresa = int.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@cd_turma"))
                            cd_turma = int.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@cd_aluno"))
                            cd_aluno = int.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@tipo"))
                            tipoRelatorio = int.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@situacaoAlunoTurma"))
                            situacaoAlunoTurma = parametrosHash[1] + "";
                        if (parametrosHash[0].Equals("@semTurma"))
                            semTurma = bool.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@bolsaCem"))
                            bolsaCem = bool.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@tranferencia"))
                            tranferencia = bool.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@retorno"))
                            retorno = bool.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@situacaoContrato"))
                            situacaoContrato = int.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@cdAtendente"))
                            cdAtendente = int.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@ckContratoDigitalizado"))
                            contratoDigitalizado = bool.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@cd_produto"))
                            cdProduto = int.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@no_produto"))
                            noProduto = parametrosHash[1] + "";
                        if (parametrosHash[0].Equals("@exibirEnderecos"))
                            exibirEnderecos = bool.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@dtIni") && parametrosHash[1] != "")
                            dtIni = DateTime.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@dtFim") && parametrosHash[1] != "")
                            dtFim = DateTime.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@vinculado"))
                            vinculado = int.Parse(parametrosHash[1]);
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
                        IEnumerable<FundacaoFisk.SGF.Web.Services.Secretaria.Model.MatriculaRel> matriculas = new List<FundacaoFisk.SGF.Web.Services.Secretaria.Model.MatriculaRel>();
                        Hashtable sourceHash = new Hashtable();
                        FundacaoFisk.SGF.Web.Services.ApresentadorRelatorio.RelatorioController apresentadorRelatorio = new FundacaoFisk.SGF.Web.Services.ApresentadorRelatorio.RelatorioController();
                        var caminhoRelatorio = "";
                        switch (tipoRelatorio)
                        {
                            case (byte)FundacaoFisk.SGF.Web.Services.Secretaria.Model.MatriculaRel.TipoRelatorioMat.MATRICULA_ANALITICO:
                                caminhoRelatorio = FundacaoFisk.SGF.Utils.ReportParameter.RELATORIO_MATRICULA_ANALITICO;
                                matriculas = apresentadorRelatorio.getMatriculaAnalitico(cd_empresa, cd_turma, cd_aluno, situacaoAlunoTurma, semTurma, tranferencia, retorno, situacaoContrato, dtIni, dtFim, cdAtendente, bolsaCem, contratoDigitalizado, cdProduto, noProduto, exibirEnderecos, vinculado).ToList();
                                sourceHash["DataSetReport"] = matriculas;
                                parametrosRelatorio.Add(Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO, "Relatório de Matrícula Analítico");
                                parametrosRelatorio.Add("PCdProduto", cdProduto);
                                parametrosRelatorio.Add("PNoProduto", noProduto);
                                parametrosRelatorio.Add("PExibirEnderecos", exibirEnderecos);
                                break;
                            case (byte)FundacaoFisk.SGF.Web.Services.Secretaria.Model.MatriculaRel.TipoRelatorioMat.MATRICULA_ATENDENTE:
                                caminhoRelatorio = FundacaoFisk.SGF.Utils.ReportParameter.RELATORIO_MATRICULA_ANALITICO;
                                matriculas = apresentadorRelatorio.getMatriculaAnalitico(cd_empresa, cd_turma, cd_aluno, situacaoAlunoTurma, semTurma, tranferencia, retorno, situacaoContrato, dtIni, dtFim, cdAtendente, bolsaCem, contratoDigitalizado, null, null, false, vinculado).ToList();
                                sourceHash["DataSetReport"] = matriculas;
                                parametrosRelatorio.Add(Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO, "Relatório de Matrícula X Atendente");
                                parametrosRelatorio.Add("PCdProduto", 0);
                                parametrosRelatorio.Add("PNoProduto", "Todos");
                                parametrosRelatorio.Add("PExibirEnderecos", exibirEnderecos);
                                break;
                            case (byte)FundacaoFisk.SGF.Web.Services.Secretaria.Model.MatriculaRel.TipoRelatorioMat.MATRICULA_MOTIVO:
                                caminhoRelatorio = FundacaoFisk.SGF.Utils.ReportParameter.RELATORIO_MATRICULA_MOTIVO;
                                matriculas = apresentadorRelatorio.getMatriculaPorMotivo(cd_empresa, cd_turma, cd_aluno, situacaoAlunoTurma, semTurma, tranferencia, retorno, situacaoContrato, dtIni, dtFim, vinculado).ToList();
                                sourceHash["DataSetReport"] = matriculas;
                                parametrosRelatorio.Add(Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO, "Relatório de Matrículas Por Motivo");
                                break;
                        }
                        //parametrosRelatorio.Add("DataBase", dataBase);
                        //parametrosRelatorio.Add("PGradeSaldoLiquidacao", tipoLiquidacao);
                        if ((dtIni != null && dtFim != DateTime.MinValue) || (dtFim != null && dtFim != DateTime.MinValue))
                            parametrosRelatorio.Add("PPeriodo", "Período de " + String.Format("{0:dd/MM/yyyy}", dtIni) + " à " + String.Format("{0:dd/MM/yyyy}", dtFim));
                        System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("pt-BR");
         
                        int cdsSituacao = 100;
                        string[] situacao = situacaoAlunoTurma.Split('|');
                        if(situacao.Count() == 1)
                            cdsSituacao = Int32.Parse(situacao[0]);
                        else
                            cdsSituacao = 1;

                        parametrosRelatorio.Add("PSituacao", cdsSituacao);
                        parametrosRelatorio.Add("PTranferencia", tranferencia);
                        parametrosRelatorio.Add("PRetorno", retorno);
                        
                        if (matriculas != null && matriculas.Count() > 0)
                        {
                            if(tipoRelatorio != (byte)FundacaoFisk.SGF.Web.Services.Secretaria.Model.MatriculaRel.TipoRelatorioMat.MATRICULA_MOTIVO)
                                parametrosRelatorio.Add("PNoAtendente", matriculas.FirstOrDefault().no_atendente);
                            relatorio.ExibirRelatorioEspecializado(ConfigurationManager.AppSettings["caminhoRelatorio"] + "", caminhoRelatorio, sourceHash, parametrosRelatorio);
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

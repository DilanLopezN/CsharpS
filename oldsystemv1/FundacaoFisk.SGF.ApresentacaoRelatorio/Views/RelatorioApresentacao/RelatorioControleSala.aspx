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
     
         private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("RelatorioTurma.aspx");
         Hashtable parametrosRelatorio = new Hashtable();
         IEnumerable<FundacaoFisk.SGF.Web.Services.Coordenacao.Model.ReportControleSala> legendas = new List<FundacaoFisk.SGF.Web.Services.Coordenacao.Model.ReportControleSala>();
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
                string parms = HttpUtility.UrlDecode(Request.QueryString[Componentes.Utils.ReportParameter.PARAMETROS], System.Text.Encoding.UTF8).Replace(" ", "+");
                DateTime dataBase = new DateTime();
                bool renderizarRelatorio = true;
                int cd_empresa = 0;
                int cd_pessoa_professor = 0;
                int cd_sala = 0;
                int cd_turma = 0;
                string no_turma = "";
                string no_sala = "";
                TimeSpan? horaInicial = null;
                TimeSpan? horaFinal = null;
                List<int> diasSemana = new List<int>();
                DateTime dataIni = new DateTime(2016, 12, 1);
                DateTime dataFim = new DateTime(2016, 12, 31);
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
                        if (parametrosHash[0].Equals("@cd_pessoa_professor"))
                            cd_pessoa_professor = int.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@cd_sala"))
                            cd_sala = int.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@cd_turma"))
                            cd_turma = int.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@hIni") && parametrosHash[1] != "")
                            horaInicial = TimeSpan.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@hFim") && parametrosHash[1] != "")
                            horaFinal = TimeSpan.Parse(parametrosHash[1]);
                        if (parametrosHash[0].Equals("@dias"))
                        {
                            string dias = parametrosHash[1] + "";
                            string[] diasS = dias.Split(',');
                            diasSemana = diasS.Select(x => int.Parse(x)).ToList();
                        }
                        if (parametrosHash[0].Equals("@no_turma"))
                            no_turma = (parametrosHash[1]);
                        if (parametrosHash[0].Equals("@no_sala"))
                            no_sala = (parametrosHash[1]);
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
                        string descDias = "";
                        foreach (int d in diasSemana){
                            string dia = FundacaoFisk.SGF.GenericModel.Horario.retornarDiaSemana((byte)d);
                            if (string.IsNullOrEmpty(descDias))
                                descDias += dia.Substring(0, 3);
                            else
                                descDias += " - " + dia.Substring(0, 3);
                        }
                        //List<FundacaoFisk.SGF.Web.Services.Coordenacao.Model.ReportControleSala> legendas = new List<FundacaoFisk.SGF.Web.Services.Coordenacao.Model.ReportControleSala>();
                        FundacaoFisk.SGF.Web.Services.ApresentadorRelatorio.RelatorioController apresentadorRelatorio = new FundacaoFisk.SGF.Web.Services.ApresentadorRelatorio.RelatorioController();
                        //var listaHorariosSalas = apresentadorRelatorio.getRptControleSala(horaInicial, horaFinal, cd_turma, cd_pessoa_professor, cd_sala, diasSemana, cd_empresa);
                        var coresListaHorariosSalas = apresentadorRelatorio.getRptControleSalaCores(horaInicial, horaFinal, cd_turma, cd_pessoa_professor, cd_sala, diasSemana, cd_empresa);
                        List<FundacaoFisk.SGF.Web.Services.Coordenacao.Model.ReportControleSala> listaDias = new List<FundacaoFisk.SGF.Web.Services.Coordenacao.Model.ReportControleSala>();
                        foreach (int dia in diasSemana)
                            listaDias.Add(new FundacaoFisk.SGF.Web.Services.Coordenacao.Model.ReportControleSala { id_dia_semana = dia });

                        legendas = coresListaHorariosSalas.Where(w => w.no_tipo_cor != null).GroupBy(x => new { x.no_sigla_estagio, x.no_tipo_cor }).Select(g => new FundacaoFisk.SGF.Web.Services.Coordenacao.Model.ReportControleSala
                        {
                            no_tipo_cor = g.Key.no_tipo_cor,
                            no_sigla_estagio = g.Key.no_sigla_estagio
                        });
                        var caminhoRelatorio = FundacaoFisk.SGF.Utils.ReportParameter.RELATORIO_CONTROLE_SALA;
                        parametrosRelatorio.Add(Componentes.Utils.ReportParameter.PARAMETRO_NOME_RELATORIO, "Controle de Salas");
                       
                        if (horaInicial != null || horaFinal != null)
                        {
                            string horarios = null;
                            if (horaInicial != null)
                                horarios = horaInicial.Value.ToString("hh':'mm");
                            if (horaFinal != null)
                                horarios += " Até " + horaFinal.Value.ToString("hh':'mm");
                            parametrosRelatorio.Add("PHorarios", horarios);
                        }
                        if(cd_sala > 0 && !String.IsNullOrEmpty(no_sala))
                            parametrosRelatorio.Add("PSala", no_sala);
                        if (cd_turma > 0 && !String.IsNullOrEmpty(no_turma))
                            parametrosRelatorio.Add("PTurma", no_turma);
                        parametrosRelatorio.Add("PDiasSemana", descDias);

                        parametrosRelatorio.Add("PHoraInicial", horaInicial.ToString());
                        parametrosRelatorio.Add("PHoraFinal", horaFinal.ToString());
                        parametrosRelatorio.Add("PcdTurma", cd_turma);
                        parametrosRelatorio.Add("PcdPessoaProfessor", cd_pessoa_professor);
                        parametrosRelatorio.Add("PcdSala", cd_sala);
                        parametrosRelatorio.Add("PcdEmpresa", cd_empresa);
                            
                        parametrosRelatorio.Add("PDiasSelecionados", descDias);
                        //var sourceTO = carnes.ToList<Componentes.GenericModel.TO>();
                        //List<FundacaoFisk.SGF.GenericModel.Sala> sourceTO = new List<FundacaoFisk.SGF.GenericModel.Sala>();
                        Hashtable sourceHash = new Hashtable();
                        sourceHash["DataSetReport"] = listaDias;
                        sourceHash["DataSetReportLegenda"] = legendas;
                        if (listaDias != null && listaDias.Count > 0)
                        {
                            relatorio.ExibirRelatorioEspecializado(ConfigurationManager.AppSettings["caminhoRelatorio"] + "", caminhoRelatorio, sourceHash, parametrosRelatorio);
                            relatorio.LocalReport.SubreportProcessing += new Microsoft.Reporting.WebForms.SubreportProcessingEventHandler(renderizaSubRelatorioControleSala);
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

            private void renderizaSubRelatorioControleSala(object sender, Microsoft.Reporting.WebForms.SubreportProcessingEventArgs e)
            {
                try
                {
                    List<int> id_dia_semana = new List<int>();
                    id_dia_semana.Add(Convert.ToInt32(e.Parameters["id_dia_semana"].Values[0]));

                    TimeSpan? horaInicial = (parametrosRelatorio["PHoraInicial"] != null && parametrosRelatorio["PHoraInicial"] != "") ? (TimeSpan?)TimeSpan.Parse(parametrosRelatorio["PHoraInicial"].ToString()) : null;
                    TimeSpan? horaFinal = (parametrosRelatorio["PHoraFinal"] != null && parametrosRelatorio["PHoraFinal"] != "") ? (TimeSpan?)TimeSpan.Parse(parametrosRelatorio["PHoraFinal"].ToString()) : null;
                    int cd_turma = Convert.ToInt32(parametrosRelatorio["PcdTurma"]);
                    int cd_pessoa_professor = Convert.ToInt32(parametrosRelatorio["PcdPessoaProfessor"]);
                    int cd_sala = Convert.ToInt32(parametrosRelatorio["PcdSala"]);
                    int cd_empresa = Convert.ToInt32(parametrosRelatorio["PcdEmpresa"]);
                    FundacaoFisk.SGF.Web.Services.ApresentadorRelatorio.RelatorioController apresentadorRelatorio = new FundacaoFisk.SGF.Web.Services.ApresentadorRelatorio.RelatorioController();

                    e.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSetSubControleSala",
                      apresentadorRelatorio.getRptControleSala(horaInicial, horaFinal, cd_turma, cd_pessoa_professor, cd_sala, id_dia_semana, cd_empresa)));
                    e.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSetReportLegenda", legendas));
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

        ga('create', 'UA-66104609-1', 'auto');
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

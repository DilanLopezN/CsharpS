<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>
<!DOCTYPE html>

<html>
<head id="Head1" runat="server">
    <title>Relatório</title>
</head>
<body>
    <div style="Width:100%;height:100%;">
        <script runat="server">
            private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("RelatorioDinamicoWeb.aspx");
            
            public void Page_Load(object sender, System.EventArgs e)
            {
                try {
                    logger.Debug("IsPostBack");
                    if(!IsPostBack)
                        adicionaParametros();
                    logger.Debug("fim - IsPostBack");
                }
                catch(Exception exe) {
                    if (exe.ToString().Contains("Sessão expirada"))
                        logger.Warn(FundacaoFisk.SGF.Utils.Messages.Messages.msgEmissaoRelatorioError + "URL: " + Request.Url + " Origem: " + Request.ServerVariables["HTTP_REFERER"], exe);
                    else
                        logger.Debug(FundacaoFisk.SGF.Utils.Messages.Messages.msgEmissaoRelatorioError + "URL: " + Request.Url + " Origem: " + Request.ServerVariables["HTTP_REFERER"], exe);

                    Session["Erro"] = FundacaoFisk.SGF.Utils.Messages.Messages.msgErroRenderizarRelatorio;
                    Session["StackTrace"] = exe.Message + exe.StackTrace + exe.InnerException;
                    string enderecoRelativoWeb = ConfigurationManager.AppSettings["enderecoRelativoWeb"];
                    Response.Redirect(enderecoRelativoWeb + "/Erro/Index"); 
                }
            }

            private void adicionaParametros() {
                Hashtable parametrosPesquisa = new Hashtable();
                Hashtable parametrosRelatorio = new Hashtable();
                logger.Debug("UrlDecode");
                string parms = HttpUtility.UrlDecode(Request.QueryString[Componentes.Utils.ReportParameter.PARAMETROS], System.Text.Encoding.UTF8).Replace(" ", "+");
                if(parms != null && Session["enderecoRelatorioWeb"] != null) {
                    logger.Debug("Session");
                    string url = Session["enderecoRelatorioWeb"] + "";
                    url = !url.Contains("http") ? "http://" + url : url;
                    logger.Debug("url - " + url);
                    parms = Componentes.Utils.MD5CryptoHelper.descriptografaSenha(parms, Componentes.Utils.MD5CryptoHelper.KEY);
                    parms = HttpUtility.UrlDecode(parms, System.Text.Encoding.UTF8);
                    logger.Debug(parms);
                    //Adiciona o nome da escola como parâmetro: 
                    logger.Debug(Session["NomeEscolaSelecionada"]);
                    if(Session["NomeEscolaSelecionada"] != null)
                        parms += "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_EMPRESA + "=" + Session["NomeEscolaSelecionada"];
                    logger.Debug(Session["loginUsuario"]);
                    if(Session["loginUsuario"] != null)
                        parms += "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_USUARIO + "=" + Session["loginUsuario"];
                    logger.Debug("loginUsuario");
                    parms += "&" + Componentes.Utils.ReportParameter.PARAMETRO_DATA_HORA_ATUAL + "=" + DateTime.Now;
                    parms = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(Componentes.Utils.MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parms, System.Text.Encoding.UTF8), Componentes.Utils.MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);
                    logger.Debug("Redirect");
                    string url_direct = url + "RelatorioApresentacao/RelatorioDinamico?" + parms + "&enderecoWeb=" + HttpUtility.UrlEncode(Session["enderecoWeb"] + "", System.Text.Encoding.UTF8);
					//url_direct = "http://" + url_direct;
                    logger.Debug("url compelta - " + url_direct);
                    Response.Redirect(url_direct);
                }
                else
                    throw new Exception(Componentes.Utils.Messages.Messages.msgSessaoExpirada);
            }
        </script>
        <form id="Form1" runat="server" style="width:100%; height:100%;">
        </form>
    </div>
</body>
</html>
<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Relatório</title>
</head>
<body>
    <div style="width:100%; height:100%">
       <script runat="server">
           private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("RelatorioAvaliacao.aspx");

           public void Page_Load(object sender, System.EventArgs e)
           {
               try
               {
                   if (!IsPostBack)
                       adicionaParametros();
               }
               catch (Exception exe)
               {
                   logger.Error(FundacaoFisk.SGF.Utils.Messages.Messages.msgEmissaoRelatorioError, exe);
                   Session["Erro"] = FundacaoFisk.SGF.Utils.Messages.Messages.msgErroRenderizarRelatorio;
                   Session["StackTrace"] = exe.Message + exe.StackTrace + exe.InnerException;
                   string enderecoRelativoWeb = ConfigurationManager.AppSettings["enderecoRelativoWeb"];
                   Response.Redirect(enderecoRelativoWeb + "/Erro/Index");
               }
           }

           private void adicionaParametros()
           {
               Hashtable parametrosPesquisa = new Hashtable();
               Hashtable parametrosRelatorio = new Hashtable();
               string parms = HttpUtility.UrlDecode(Request.QueryString[Componentes.Utils.ReportParameter.PARAMETROS], System.Text.Encoding.UTF8).Replace(" ", "+");

               if (parms != null && Session["enderecoRelatorioWeb"] != null)
               {
                   string url = Session["enderecoRelatorioWeb"] + "";
                   url = !url.Contains("http") ? "http://" + url : url;
                   parms = Componentes.Utils.MD5CryptoHelper.descriptografaSenha(parms, Componentes.Utils.MD5CryptoHelper.KEY);
                   parms = HttpUtility.UrlDecode(parms, System.Text.Encoding.UTF8);

                   //Adiciona o nome da escola como parâmetro: 
                   if (Session["NomeEscolaSelecionada"] != null)
                       parms += "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_EMPRESA + "=" + Session["NomeEscolaSelecionada"];
                   if (Session["loginUsuario"] != null)
                       parms += "&" + Componentes.Utils.ReportParameter.PARAMETRO_NOME_USUARIO + "=" + Session["loginUsuario"];
                   parms += "&" + Componentes.Utils.ReportParameter.PARAMETRO_DATA_HORA_ATUAL + "=" + DateTime.Now;
                   parms = Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(Componentes.Utils.MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parms, System.Text.Encoding.UTF8), Componentes.Utils.MD5CryptoHelper.KEY), System.Text.Encoding.UTF8);
                   Response.Redirect(url + "RelatorioApresentacao/RelatorioAvaliacao?" + parms + "&enderecoWeb=" + HttpUtility.UrlEncode(Session["enderecoWeb"] + "", System.Text.Encoding.UTF8));
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
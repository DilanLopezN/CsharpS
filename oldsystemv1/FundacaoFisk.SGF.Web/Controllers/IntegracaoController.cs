using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Componentes.GenericBusiness.Comum;
using Componentes.GenericController;
using Componentes.GenericModel;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Empresa.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Pessoa.Business;
using FundacaoFisk.SGF.Web.Services.Usuario.Business;
using FundacaoFisk.SGF.Web.Services.Usuario.Comum;
using FundacaoFisk.SGF.Web.Services.Usuario.Model;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Services.InstituicaoEnsino.Model;
using FundacaoFisk.SGF.Utils.Messages;
using FundacaoFisk.SGF.Web.Services.InstituicaoEnsino.Comum.IBusiness;
using log4net;
using FundacaoFisk.SGF.Web.Services.Auth.Comum;


namespace FundacaoFisk.SGF.Web.Controllers
{
    public class IntegracaoController : ComponentesMVCController {
        private static readonly ILog logger = LogManager.GetLogger(typeof(PessoaController));

        public IntegracaoController()
        {
        }

        public ActionResult Index() {
            return View();
        }

        [MvcComponentesAuthorize(Roles = "arr")]
        public ActionResult getUrlAreaRestritah()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {

                IAuthBusiness authBiz = (IAuthBusiness)base.instanciarBusiness<IAuthBusiness>();
                

                IUsuarioBusiness BusinessUsuario = (IUsuarioBusiness)base.instanciarBusiness<IUsuarioBusiness>();
                IApiAreaRestritaBusiness BusinessAreRestrita = (IApiAreaRestritaBusiness)base.instanciarBusiness<IApiAreaRestritaBusiness>();
                UsuarioWebSGF usuario = authBiz.GetNomeCodigoUsuario(User.Identity.Name);
                var url = Request.Url.ToString();

                if (usuario == null)
                {
                    throw new UsuarioBusinessException(Messages.msgErrorEmailUsuarioNotFound, null, UsuarioBusinessException.TipoErro.ERRO_USUARIO_EMAIL_NOT_FOUND, false);

                }


                var usuarioAreaRestrita = BusinessUsuario.GetEmailUsuario(usuario.cd_usuario);
                string urlAreaRestrita = "";
                string tokenGerado = "";

                if (usuarioAreaRestrita != null)
                {
                    if (String.IsNullOrEmpty(usuarioAreaRestrita.email))
                    {
                        throw new UsuarioBusinessException(Messages.msgErrorEmailUsuarioNotFound, null, UsuarioBusinessException.TipoErro.ERRO_USUARIO_EMAIL_NOT_FOUND, false);
                    }

                    urlAreaRestrita = BusinessAreRestrita.GerarToken(usuarioAreaRestrita.email);
                    

                }


                retorno.retorno = urlAreaRestrita;


                return new RenderJsonActionResult { Result = retorno };
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [MvcComponentesAuthorize(Roles = "arr")]
        public ActionResult getUrlAreaRestrita()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
               
                IEscolaBusiness BusinessEscola = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                int cd_escola = int.Parse(Session["CodEscolaSelecionada"] + "");
                int cd_integracao = BusinessEscola.getCodigoFranquia(cd_escola,0);

                if (cd_integracao == 0)
                {
                    retorno.AddMensagem(Messages.msgErroCodigoFranquiaInexistente, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                }
                else
                {
                    var url = Request.Url.ToString();

                    //Gera a key e IV
                    string key, IV;
                    Encryption.GenerateKeyIV(out key, out IV);

                    //preenche com zeros a esquerda caso o cd_integração seja menor que 100
                    var codigo_integracao = cd_integracao.ToString();
                    if (cd_integracao < (int)Encryption.CodigoIntegracaoPreencheZeroEsquerda.TAMANHOMAXIMO)
                    {
                        codigo_integracao = codigo_integracao.Length > 1 ? ("0" + codigo_integracao) : ("00"+ codigo_integracao);
                    }

                    //Concatenas os tokens gerados e converte em base64
                    string tokenConcatenado = Encryption.Encrypt(codigo_integracao, key, IV) + "." + key + "." + IV;
                    string token = Base64Encode(tokenConcatenado);
                    string caminho = null;

                    if (url.Contains("homolog") || url.Contains("qas") || url.Contains("localhost"))
                    {
                        caminho = "http://restritahomolog.fisk.com.br/auth/?token=" + token;
                    }
                    else
                    {
                        caminho = "http://restrita.fisk.com.br/auth/?token=" + token;
                    }

                    if (caminho == null)
                        retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                    else
                        //retorno.retorno = parametrosCript;
                        retorno.retorno = caminho;
                }

                return new RenderJsonActionResult { Result = retorno };
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        [MvcComponentesAuthorize(Roles = "ecm")]
        public ActionResult getUrlECommerce() {
            ReturnResult retorno = new ReturnResult();
            try {
                IEscolaBusiness BusinessEscola = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                int cd_escola = int.Parse(Session["CodEscolaSelecionada"] + "");
                int cd_franquia = BusinessEscola.getCodigoFranquia(cd_escola,1);
                // Monta os parâmetros descriptografados para enviar ao ecommerce:
                string parametros = "id_franquia=" + cd_franquia + "&id_chave=M@rx5678&id_aplicacao=1&data=" + DateTime.Now.ToString("ddMMyyyyHHmmss") + "&login=" + this.User.Identity.Name;
                string caminhoEcommerce = Session["enderecoEcommerceWeb"] + ""; //ConfigurationManager.AppSettings["enderecoEcommerceWeb"];
                var parametrosCript = caminhoEcommerce + "?" + Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY_ECOMMERCE), System.Text.Encoding.UTF8);

                if(parametrosCript == null)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                else
                    retorno.retorno = parametrosCript;
                return new RenderJsonActionResult { Result = retorno };
            }
            catch(Exception ex) {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [MvcComponentesAuthorize(Roles = "prtlAlu")]
        public ActionResult getUrlPortal(int cd_aluno)
        {
            ReturnResult retorno = new ReturnResult();
            try {
                IEscolaBusiness BusinessEscola = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                int cd_escola = int.Parse(Session["CodEscolaSelecionada"] + "");
                int cd_franquia = BusinessEscola.getCodigoFranquia(cd_escola,3);
                // Monta os parâmetros descriptografados para enviar ao ecommerce:
                string parametros = "id_franquia=" + cd_franquia + "&id_chave=M@rx5678&id_aplicacao=3&data=" + DateTime.Now.ToString("ddMMyyyyHHmmss") + "&login=" + this.User.Identity.Name + "&id_aluno=" + cd_aluno;
                string caminhoEcommerce = Session["enderecoPortalWeb"] + ""; //ConfigurationManager.AppSettings["enderecoEcommerceWeb"];
                var parametrosCript = caminhoEcommerce + "?" + Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY_ECOMMERCE), System.Text.Encoding.UTF8);

                if(parametrosCript == null)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                else
                    retorno.retorno = parametrosCript;
                return new RenderJsonActionResult { Result = retorno };
            }
            catch(Exception ex) {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        [MvcComponentesAuthorize(Roles = "chat")]
        public ActionResult getUrlChat()
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IEscolaBusiness BusinessEscola = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                int cd_escola = int.Parse(Session["CodEscolaSelecionada"] + "");
                int cd_franquia = BusinessEscola.getCodigoFranquia(cd_escola,5);
                // Monta os parâmetros descriptografados para enviar ao ecommerce:
                string parametros = "id_franquia=" + cd_franquia + "&id_chave=M@rx5678&id_aplicacao=5&data=" + DateTime.Now.ToString("ddMMyyyyHHmmss") + "&login=" + this.User.Identity.Name;
                string caminhoEcommerce = Session["enderecoChatWeb"] + ""; //ConfigurationManager.AppSettings["enderecoEcommerceWeb"];
                var parametrosCript = caminhoEcommerce + "?" + Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY_ECOMMERCE), System.Text.Encoding.UTF8);

                if (parametrosCript == null)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                else
                    retorno.retorno = parametrosCript;
                return new RenderJsonActionResult { Result = retorno };
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }
        
        [MvcComponentesAuthorize(Roles = "das")]
        public ActionResult getUrlDashboard() {
            ReturnResult retorno = new ReturnResult();
            try {
                IEscolaBusiness BusinessEscola = (IEscolaBusiness)base.instanciarBusiness<IEscolaBusiness>();
                int cd_escola = int.Parse(Session["CodEscolaSelecionada"] + "");
                int cd_franquia = BusinessEscola.getCodigoFranquia(cd_escola,2);
                // Monta os parâmetros descriptografados para enviar ao ecommerce:
                string parametros = "id_franquia=" + cd_franquia + "&id_chave=M@rx5678&id_aplicacao=2&data=" + DateTime.Now.ToString("ddMMyyyyHHmmss") + "&login=" + this.User.Identity.Name;
                string caminhoEcommerce = Session["enderecoDashboradWeb"] + ""; //ConfigurationManager.AppSettings["enderecoDashboardWeb"];
                var parametrosCript = caminhoEcommerce + "?" + Componentes.Utils.ReportParameter.PARAMETROS + "=" + HttpUtility.UrlEncode(MD5CryptoHelper.criptografaSenha(HttpUtility.UrlEncode(parametros, System.Text.Encoding.UTF8), MD5CryptoHelper.KEY_ECOMMERCE), System.Text.Encoding.UTF8);

                if(parametrosCript == null)
                    retorno.AddMensagem(Messages.msgRegNotEnc, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                else
                    retorno.retorno = parametrosCript;
                return new RenderJsonActionResult { Result = retorno };
            }
            catch(Exception ex) {
                return gerarLogException(Messages.msgRegBuscError, retorno, logger, ex);
            }
        }

        public string descriptografaChat(string parametros)
        {
            string mensagem = string.Empty;
            try
            {
                string parms = HttpUtility.UrlDecode(parametros, System.Text.Encoding.UTF8).Replace(" ", "+");

                if (parms != null)
                {
                    string url = Session["enderecoRelatorioWeb"] + "";

                    parms = Componentes.Utils.MD5CryptoHelper.descriptografaSenha(parms, Componentes.Utils.MD5CryptoHelper.KEY_ECOMMERCE);
                    parms = HttpUtility.UrlDecode(parms, System.Text.Encoding.UTF8);

                    string[] parametrosGet = parms.Split('&');

                    for (int i = 0; i < parametrosGet.Length; i++)
                    {
                        string[] parametrosHash = parametrosGet[i].Split('=');

                        if (parametrosHash[0].Equals("id_franquia"))
                            mensagem += "id_franquia = " + parametrosHash[1] + "</br>";
                        if (parametrosHash[0].Equals("id_chave"))
                            mensagem += "id_chave = " + parametrosHash[1] + "</br>";
                        if (parametrosHash[0].Equals("id_aplicacao"))
                            mensagem += "id_aplicacao = " + parametrosHash[1] + "</br>";
                        if (parametrosHash[0].Equals("data"))
                            mensagem += "data = " + parametrosHash[1] + "</br>";
                        if (parametrosHash[0].Equals("login"))
                            mensagem += "login = " + parametrosHash[1] + "</br>";
                    }
                }

                return mensagem;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, null, logger, ex).ToString();
            }
        }

        public string descriptografaPortal(string parametros)
        {
            string mensagem = string.Empty;
            try
            {
                string parms = HttpUtility.UrlDecode(parametros, System.Text.Encoding.UTF8).Replace(" ", "+");

                if (parms != null)
                {
                    string url = Session["enderecoRelatorioWeb"] + "";

                    parms = Componentes.Utils.MD5CryptoHelper.descriptografaSenha(parms, Componentes.Utils.MD5CryptoHelper.KEY_ECOMMERCE);
                    parms = HttpUtility.UrlDecode(parms, System.Text.Encoding.UTF8);

                    string[] parametrosGet = parms.Split('&');

                    for (int i = 0; i < parametrosGet.Length; i++)
                    {
                        string[] parametrosHash = parametrosGet[i].Split('=');

                        if (parametrosHash[0].Equals("id_franquia"))
                            mensagem += "id_franquia = " + parametrosHash[1] + "</br>";
                        if (parametrosHash[0].Equals("id_chave"))
                            mensagem += "id_chave = " + parametrosHash[1] + "</br>";
                        if (parametrosHash[0].Equals("id_aplicacao"))
                            mensagem += "id_aplicacao = " + parametrosHash[1] + "</br>";
                        if (parametrosHash[0].Equals("data"))
                            mensagem += "data = " + parametrosHash[1] + "</br>";
                        if (parametrosHash[0].Equals("login"))
                            mensagem += "login = " + parametrosHash[1] + "</br>";
                        if (parametrosHash[0].Equals("id_aluno"))
                            mensagem += "id_aluno = " + parametrosHash[1] + "</br>";
                    }
                }

                return mensagem;
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgRegBuscError, null, logger, ex).ToString();
            }
        }

        public string descriptografaEcommerce(string parametros) {
            string mensagem = string.Empty;
            try {
                string parms = HttpUtility.UrlDecode(parametros, System.Text.Encoding.UTF8).Replace(" ", "+");

                if(parms != null) {
                    string url = Session["enderecoRelatorioWeb"] + "";

                    parms = Componentes.Utils.MD5CryptoHelper.descriptografaSenha(parms, Componentes.Utils.MD5CryptoHelper.KEY_ECOMMERCE);
                    parms = HttpUtility.UrlDecode(parms, System.Text.Encoding.UTF8);

                    string[] parametrosGet = parms.Split('&');

                    for(int i = 0; i < parametrosGet.Length; i++) {
                        string[] parametrosHash = parametrosGet[i].Split('=');

                        if(parametrosHash[0].Equals("id_franquia"))
                            mensagem += "id_franquia = " + parametrosHash[1] + "</br>";
                        if(parametrosHash[0].Equals("id_chave"))
                            mensagem += "id_chave = " + parametrosHash[1] + "</br>";
                        if(parametrosHash[0].Equals("id_aplicacao"))
                            mensagem += "id_aplicacao = " + parametrosHash[1] + "</br>";
                        if(parametrosHash[0].Equals("data"))
                            mensagem += "data = " + parametrosHash[1] + "</br>";
                        if(parametrosHash[0].Equals("login"))
                            mensagem += "login = " + parametrosHash[1] + "</br>";
                    }
                }

                return mensagem;
            }
            catch(Exception ex) {
                return gerarLogException(Messages.msgRegBuscError, null, logger, ex).ToString();
            }
        }

        public string descriptografaDashboard(string parametros) {
            string mensagem = string.Empty;
            try {
                string parms = HttpUtility.UrlDecode(parametros, System.Text.Encoding.UTF8).Replace(" ", "+");

                if(parms != null) {
                    string url = Session["enderecoRelatorioWeb"] + "";

                    parms = Componentes.Utils.MD5CryptoHelper.descriptografaSenha(parms, Componentes.Utils.MD5CryptoHelper.KEY_ECOMMERCE);
                    parms = HttpUtility.UrlDecode(parms, System.Text.Encoding.UTF8);

                    string[] parametrosGet = parms.Split('&');

                    for(int i = 0; i < parametrosGet.Length; i++) {
                        string[] parametrosHash = parametrosGet[i].Split('=');

                        if(parametrosHash[0].Equals("id_franquia"))
                            mensagem += "id_franquia = " + parametrosHash[1] + "</br>";
                        if(parametrosHash[0].Equals("id_chave"))
                            mensagem += "id_chave = " + parametrosHash[1] + "</br>";
                        if(parametrosHash[0].Equals("id_aplicacao"))
                            mensagem += "id_aplicacao = " + parametrosHash[1] + "</br>";
                        if(parametrosHash[0].Equals("data"))
                            mensagem += "data = " + parametrosHash[1] + "</br>";
                        if(parametrosHash[0].Equals("login"))
                            mensagem += "login = " + parametrosHash[1] + "</br>";
                    }
                }

                return mensagem;
            }
            catch(Exception ex) {
                return gerarLogException(Messages.msgRegBuscError, null, logger, ex).ToString();
            }
        }
    }
}

using System;
using System.Web;
using System.Net;
using System.Web.Mvc;
using System.Net.Http;
using System.Web.Caching;
using System.Web.Security;
using System.Net.Http.Formatting;
using FundacaoFisk.SGF.Web.Services.Auth.Comum;
using FundacaoFisk.SGF.Web.Services.Auth.Model;
using FundacaoFisk.SGF.GenericModel;
using System.Collections.Generic;
using FundacaoFisk.SGF.Web;
using Componentes.Utils;
using FundacaoFisk.SGF.Services.InstituicaoEnsino.Model;
using log4net;
using FundacaoFisk.SGF.Utils.Messages;
using Componentes.GenericBusiness;
using FundacaoFisk.SGF.Web.Services.Usuario.Comum;
using Componentes.GenericController;
using FundacaoFisk.SGF.Web.Services.InstituicaoEnsino.Comum.IBusiness;
using System.Configuration;
using FundacaoFisk.SGF.Web.Services.Empresa.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Empresa.Model;
using FundacaoFisk.SGF.Web.Services.Auth.Business;
using FundacaoFisk.SGF.Web.Services.Usuario.Model;
using FundacaoFisk.SGF.Web.Services.Usuario.Business;
using System.Net.Configuration;
using System.Web.Configuration;
using FundacaoFisk.SGF.Web.Services.Secretaria.Comum;
using FundacaoFisk.SGF.Web.Services.Log.Comum.IBusiness;


namespace FundacaoFisk.SGF.Web.Controllers {
    public class AuthController : ComponentesMVCController {

        private static readonly ILog logger = LogManager.GetLogger(typeof(AuthController));

        public AuthController()
        {
        }

        public string AccessToken {
            get {
                try{
                    dynamic cache = Cache();
                    return ((AccessTokenResponse) cache["AccessToken" + ";" + User.Identity.Name]).access_token;
                }
                catch(Exception exe) {
                    logger.Error(exe);
                    throw exe;
                }
            }
        }

        private System.Security.Principal.IPrincipal _user;
        public new System.Security.Principal.IPrincipal User {
            get { return _user ?? base.User; }
            set { _user = value; }
        }

        private Action<UserCredential, AccessTokenResponse, List<EmpresaSession>, int, string, int, string, bool, int, bool> _webActions;
        public  Action<UserCredential, AccessTokenResponse, List<EmpresaSession>, int, string, int, string, bool, int, bool> WebActions {
                get {
                    try{
                        return _webActions ?? LoginWebActions;
                    }
                    catch(Exception exe) {
                        logger.Error(exe);
                        throw exe;
                    }
                }
                set {
                    _webActions = value;
                }
            
        }

        public Func<object> _cache;
        public Func<object> Cache {
            get {
                return _cache ?? (() => HttpContext.Cache);
            }
            set {
                _cache = value;
            }
        }
        private void LoginWebActions(UserCredential credentials, AccessTokenResponse accessTokenResponse, List<EmpresaSession> escolas, int codPessoaUsuario,
                string loginUsuario, int cdUsuario, string permissoes, bool idMaster, int id_fuso_horario, bool id_horario_verao)
        {
            dynamic Cache = this.Cache();
            //CacheItemUpdateCallback refreshMethod = new CacheItemUpdateCallback(RefreshAccessToken);
            Cache.Insert("AccessToken" + ";" + credentials.Login, accessTokenResponse, null, DateTime.UtcNow.AddSeconds(accessTokenResponse.expires_in), System.Web.Caching.Cache.NoSlidingExpiration);
            Session["UserName"] = credentials.Login;
            Session["loginUsuario"] = loginUsuario;
            Session["CodPessoaUsuario"] = codPessoaUsuario;
            Session["IdMaster"] = idMaster;
            Session["Permissoes"] = permissoes;
            Session["CodUsuario"] = cdUsuario;
            Session["EscolasUsuario"] = escolas;
            Session["IdFusoHorario"] = id_fuso_horario;
            Session["IdHorarioVerao"] = id_horario_verao;
            UserCredential.permissoes = permissoes;

            Cache.Remove("RemoveAccessToken" + ";" + credentials.Login);
            FormsAuthentication.SetAuthCookie(credentials.Login, false);

            // Configura as permissões do usuario no cookie para o Authorize do mvc:
            /*permissoes = CompressHelper.compress(permissoes);
            FormsAuthenticationTicket permissionTicket = new FormsAuthenticationTicket(1, credentials.Login, DateTime.Now, DateTime.Now.AddMilliseconds(FormsAuthentication.Timeout.TotalMilliseconds), false, permissoes);
            HttpCookie permissionCookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(permissionTicket)) {
                Domain = FormsAuthentication.CookieDomain,
                HttpOnly = true,
                Secure = FormsAuthentication.RequireSSL,
            };
            Response.AppendCookie(permissionCookie);*/
        }

        //[AllowAnonymous]
        public ActionResult Index() {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Postescola(UsuarioEmpresaLoginUI usuarioEmpresa) {
            ReturnResult retorno = new ReturnResult();
            UserCredential credentials = new UserCredential();

            try
            {
                
                IEmpresaBusiness BusinessEmpresa = (IEmpresaBusiness)base.instanciarBusiness<IEmpresaBusiness>();
                IUsuarioBusiness BusinessUsuario = (IUsuarioBusiness)base.instanciarBusiness<IUsuarioBusiness>();
                ISecretariaBusiness BusinessSecretaria = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();

                // Busca as escolas por id:
                if(usuarioEmpresa.Empresas[0].cd_pessoa == 0)
                    throw new AuthBusinessException(Utils.Messages.Messages.msgEscolaInvalida, AuthBusinessException.TipoErro.AUTORIZACAO_NAO_ENCONTRADA);
                EmpresaSession empresaSelecionada = BusinessEmpresa.findSessionByIdEmpresa(usuarioEmpresa.Empresas[0].cd_pessoa);

                // Compara se o usuário está logando no horário de funcionamento da escola:
                UsuarioWebSGF usuario = BusinessUsuario.GetUsuarioAuthenticateByLogin(usuarioEmpresa.usuario.no_login);
#pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
                int idFuso = (Session["IdFusoHorario"] == null || Session["IdFusoHorario"] == "") ? 0 : int.Parse(Session["IdFusoHorario"] + "");
#pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast

                DateTime dataAtual = SGF.Utils.ConversorUTC.ToLocalTime(DateTime.Now, idFuso, false/*bool.Parse(Session["IdHorarioVerao"]+"")*/);
                TimeSpan horaAtual = new TimeSpan(dataAtual.TimeOfDay.Hours, dataAtual.TimeOfDay.Minutes, 0);
                //logger.Debug("Data Atual: " + horaAtual);
                //logger.Error("Data Atual: " + horaAtual);
                int diaSemanaAtual = (int)dataAtual.DayOfWeek + 1;
                //logger.Debug("Data Atual -dia -: " + diaSemanaAtual);
                //logger.Error("Data Atual -dia -: " + diaSemanaAtual);
                if(!usuario.id_admin && !usuario.id_master && !usuario.id_administrador)
                    BusinessSecretaria.verificaHorarioUsuario(empresaSelecionada.cd_pessoa, usuario.cd_usuario, horaAtual, diaSemanaAtual);
               
                Session["CodEscolaSelecionada"] = credentials.CodEmpresa = empresaSelecionada.cd_pessoa;
                Session["NomeEscolaSelecionada"] = empresaSelecionada.dc_reduzido_pessoa;
                credentials.Login = usuarioEmpresa.usuario.no_login;
                credentials.Password = usuarioEmpresa.usuario.dc_senha_usuario;
                credentials.HrInicial = empresaSelecionada.hr_inicial;
                credentials.HrFinal = empresaSelecionada.hr_final;

                if(Session["EscolasUsuario"] == null)
                    throw new Exception(Componentes.Utils.Messages.Messages.msgSessaoExpirada);
                credentials.EhMasterGeral = usuario.id_master && ((List<EmpresaSession>)Session["EscolasUsuario"]).Count == 0;
                Session["MasterGeral"] = credentials.EhMasterGeral;
                return LoginEscola(credentials, true);          
            }
            catch(BusinessException exe) {
                retorno.AddMensagem(exe.Message, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                return new RenderJsonActionResult { Result = retorno };
            }
            catch(Exception exe) {
                logger.Error(exe);
                //Session["Erro"] = Messages.msgAuthenticationError;
                //Session["StackTrace"] = exe.Message + exe.StackTrace + exe.InnerException;
                retorno.AddMensagem(Messages.msgAuthenticationError, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                return new RenderJsonActionResult { Result = retorno };
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(UserCredential credentials) {
            return LoginEscola(credentials, false);
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult LoginEscola(UserCredential credentials, bool isJson) {
            ReturnResult retorno = new ReturnResult();

            try
            {
                IEmpresaBusiness BusinessEmpresa = (IEmpresaBusiness)base.instanciarBusiness<IEmpresaBusiness>();
                IUsuarioBusiness BusinessUsuario = (IUsuarioBusiness)base.instanciarBusiness<IUsuarioBusiness>();
                ISecretariaBusiness BusinessSecretaria = (ISecretariaBusiness)base.instanciarBusiness<ISecretariaBusiness>();
                IAuthBusiness BusinessAuth = (IAuthBusiness)base.instanciarBusiness<IAuthBusiness>();
                ILogGeralBusiness BusinessLogGeral = (ILogGeralBusiness)base.instanciarBusiness<ILogGeralBusiness>();

                if (Session["MasterGeral"] == null)
                    Session["MasterGeral"] = false;

                if (Session["IdFusoHorario"] == null)
                {
                    Session["IdFusoHorario"] = credentials.IdFusoHorario;
                    Session["IdHorarioVerao"] = credentials.IdHorarioVerao;
                }
                else
                {
                    credentials.IdFusoHorario = int.Parse(Session["IdFusoHorario"] + "");
                    credentials.IdHorarioVerao = bool.Parse(Session["IdHorarioVerao"] + "");
                }

                bool masterGeral = BusinessEmpresa.VerificarMasterGeral(credentials.Login);
                credentials.nmMaxTentativas = int.Parse(ConfigurationManager.AppSettings["nmTentativaLogin"]);
                if(String.IsNullOrEmpty(credentials.Login) || String.IsNullOrEmpty(credentials.Password))
                    throw new AuthBusinessException(Messages.msgAuthenticationError, null, AuthBusinessException.TipoErro.AUTORIZACAO_NAO_ENCONTRADA, false);
                UsuarioWebSGF usuario = BusinessUsuario.GetUsuarioAuthenticateByLogin(credentials.Login);
                if(usuario == null)
                    throw new AuthBusinessException(Messages.msgAuthenticationError, null, AuthBusinessException.TipoErro.AUTORIZACAO_NAO_ENCONTRADA, false);
                if (ModelState.IsValid)
                {

                    if (usuario.dc_senha_usuario == BusinessUsuario.GeraSenhaHashSHA1(credentials.Password))
                    {
                        //Verifica se a senha esta expirada, e obriga a trocar a senha .
                        Double qtdDiasExpSenha = Double.Parse(ConfigurationManager.AppSettings["qtdDiasExpSenha"]);
                        DateTime dtNow = DateTime.UtcNow.Date;
                        TimeSpan timeDif = usuario.dt_expiracao_senha - dtNow;
                        Double totalDiasDif = timeDif.TotalDays;
                        if (Convert.ToInt32(Math.Abs(totalDiasDif)) > qtdDiasExpSenha)
                        {
                            retorno.AddMensagem(string.Format(Messages.msgInfSenhaExpirada, usuario.no_login), null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                            usuario.id_trocar_senha = true;
                            retorno.retorno = Newtonsoft.Json.JsonConvert.SerializeObject(new UsuarioWebSGF { id_trocar_senha = usuario.id_trocar_senha });
                            return new RenderJsonActionResult { Result = retorno };
                        }

                        //Verifica se o usuario precisa trocar a senha.
                        if (usuario.id_trocar_senha)
                        {
                            retorno.AddMensagem(Messages.msgInfoTrocarSenha, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                            retorno.retorno = Newtonsoft.Json.JsonConvert.SerializeObject(new UsuarioWebSGF { id_trocar_senha = usuario.id_trocar_senha });
                            return new RenderJsonActionResult { Result = retorno };
                        }
                    }

                    //var resp = (new HttpClient()).PostAsJsonAsync("http://localhost.fiddler:18043/api/auth/authenticate", credentials).Result;
                    List<EmpresaSession> empresas = new List<EmpresaSession>();
                    List<EmpresaSession> empresasinativas = new List<EmpresaSession>();
                    bool podeAutenticar = credentials.CodEmpresa.HasValue;
                    
                    empresas = BusinessEmpresa.findEmpresaSessionByLogin(credentials.Login, masterGeral, true);
                    empresasinativas = BusinessEmpresa.findEmpresaSessionByLogin(credentials.Login, masterGeral, false);
                    if (empresas.Count == 1)
                    {
                        Session["CodEscolaSelecionada"] = credentials.CodEmpresa = empresas[0].cd_pessoa;
                        Session["NomeEscolaSelecionada"] = empresas[0].dc_reduzido_pessoa;
                       
                        DateTime dataAtual = SGF.Utils.ConversorUTC.ToLocalTime(DateTime.Now, credentials.IdFusoHorario, credentials.IdHorarioVerao);
                        TimeSpan horaAtual = new TimeSpan(dataAtual.TimeOfDay.Hours, dataAtual.TimeOfDay.Minutes, 0);
                        int diaSemanaAtual = (int)dataAtual.DayOfWeek + 1;
                        if (!usuario.id_admin && !usuario.id_master && !usuario.id_administrador)
                            BusinessSecretaria.verificaHorarioUsuario(empresas[0].cd_pessoa, usuario.cd_usuario, horaAtual, diaSemanaAtual);

                        podeAutenticar = true;
                    }
                    else if (empresas.Count == 0)
                        credentials.EhMasterGeral = masterGeral;

                    AuthenticationResult result = BusinessAuth.Authenticate(credentials);
                    if (result.Status == AuthenticationStatus.OK)
                    {
                        // Verifica se o usuário é de uma ou mais escolas:
                        Session["EscolasUsuario"] = empresas;
                        Session["EscolasUsuarioInativas"] = empresasinativas;

                        if (!podeAutenticar)
                        {
                            if (empresas.Count >= 0 || result.IdMaster)
                            {
                                // Busca as escolas do sistema para apresentar para o usuário selecionar:
                                if(empresas.Count == 0)
                                {
                                    if(credentials.EhMasterGeral) {
                                        empresas = BusinessEmpresa.findAllEmpresaSession();
                                        Session["EscolasUsuarioMaster"] = empresas;
                                        // Se não tem escolas cadastradas no sistema, possibilita o usuário entrar sem escola vinculada:
                                        if(empresas.Count == 1) {
                                            Session["CodEscolaSelecionada"] = credentials.CodEmpresa = empresas[0].cd_pessoa;
                                            Session["NomeEscolaSelecionada"] = empresas[0].dc_reduzido_pessoa;
                                            
                                            //Autentica novamente com a nova escola (como já está autenticado, não tem problema):
                                            result = BusinessAuth.Authenticate(credentials);

                                            podeAutenticar = true;
                                        }
                                    }
                                    else
                                        throw new AuthBusinessException(Componentes.Utils.Messages.Messages.msgErrorAutorizacaoEscolaInativa, null, AuthBusinessException.TipoErro.AUTORIZACAO_NAO_ENCONTRADA, false);
                                }
                                if (!podeAutenticar)
                                {
                                    UsuarioEmpresaLoginUI usuarioEscola = new UsuarioEmpresaLoginUI();
                                    UsuarioWebSGF usuarioWeb = new UsuarioWebSGF();

                                    usuarioWeb.dc_senha_usuario = credentials.Password;
                                    usuarioWeb.no_login = credentials.Login;
                                    usuarioEscola.fromEmpresa(empresas);
                                    usuarioEscola.usuario = usuarioWeb;
                                    retorno.retorno = Newtonsoft.Json.JsonConvert.SerializeObject(usuarioEscola);
                                }
                            }
                            else
                                retorno.AddMensagem(Messages.msgUserNotValid, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                        }
                        if (podeAutenticar)
                        {
                            criaSessaoUrls();
                            WebActions(credentials, result.Token, (List<EmpresaSession>)Session["EscolasUsuario"], result.CodPessoaUsuario, result.loginUsuario, result.CdUsuario, result.Permissao, result.IdMaster, int.Parse(Session["IdFusoHorario"] + ""), bool.Parse(Session["IdHorarioVerao"]+""));
                            BusinessLogGeral.geraLogLogin(int.Parse(Session["CodEscolaSelecionada"] + ""), int.Parse(Session["CodUsuario"] + ""), int.Parse(Session["IdFusoHorario"] + ""));
                            return new RenderJsonActionResult { Result = true };
                        }
                    }
                    else
                        retorno.AddMensagem(result.ErrorMessage, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);

                }
                else
                    retorno.AddMensagem(Messages.msgPasswordNotValid, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
            }
            catch (BusinessException ex)
            {
                logger.Warn(ex);
                retorno.AddMensagem(ex.Message, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                return new RenderJsonActionResult { Result = retorno };
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                retorno.AddMensagem(Messages.msgAuthenticationError, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                return new RenderJsonActionResult { Result = retorno };
            }

            return new RenderJsonActionResult { Result = retorno };
        }

        // Cria as sessões de urls do sgf e do reports em pares do Web.config para o Fisk poder usar intranet e internet separadamente:
        private void criaSessaoUrls() {
            //logger.Debug("url: " + Request.Url.ToString().ToLower());
            //logger.Debug("url original: " + Request.Url.OriginalString.ToLower());
            //logger.Debug("porta: " + Request.Url.Port);
            string url = Request.Url.ToString().ToLower().Replace("/auth/postescola", "").Replace("/auth/login", "");
            string prefixo = "http://";

            url = url.Replace(prefixo, "");
            prefixo = "https://";
            url = url.Replace(prefixo, "");

            string[] url_part = url.Split(Char.Parse("/"));
            /*int port = Request.Url.Port;
            string prefixo = "http://";

            url = url.Replace(prefixo, "");

            string[] url_part = url.Split(Char.Parse("/"));

            if(!url_part[0].EndsWith(":" + port))
                url_part[0] += ":" + port;
                             
            Session["enderecoWeb"] = prefixo + string.Join("/", url_part);*/
            
            string enderecoWeb = ConfigurationManager.AppSettings["enderecoWeb"];
            string enderecoRelatorioWeb = ConfigurationManager.AppSettings["enderecoRelatorioWeb"];
            string enderecoEcommerceWeb = ConfigurationManager.AppSettings["enderecoEcommerceWeb"];
            string enderecoDashboradWeb = ConfigurationManager.AppSettings["enderecoDashboradWeb"];
            string enderecoPortalWeb = ConfigurationManager.AppSettings["enderecoPortalWeb"];
            string enderecoChatWeb = ConfigurationManager.AppSettings["enderecoChatWeb"];

            string[] enderecoWebPart = enderecoWeb.Split(Char.Parse(";"));
            string[] enderecoRelatorioWebPart = enderecoRelatorioWeb.Split(Char.Parse(";"));
            string[] enderecoEcommerceWebPart = enderecoEcommerceWeb.Split(Char.Parse(";"));
            string[] enderecoDashboradWebPart = enderecoDashboradWeb.Split(Char.Parse(";"));
            string[] enderecoPortalWebPart = enderecoPortalWeb.Split(Char.Parse(";"));
            string[] enderecoChatWebPart = enderecoChatWeb.Split(Char.Parse(";"));

            for(int i = 0; i < enderecoWebPart.Length; i++) 
                if(enderecoWebPart[i].Contains(url_part[0])) {
                    Session["enderecoWeb"] = enderecoWebPart[i];
                    Session["enderecoRelatorioWeb"] = enderecoRelatorioWebPart[i];
                    Session["enderecoEcommerceWeb"] = enderecoEcommerceWebPart[i];
                    Session["enderecoDashboradWeb"] = enderecoDashboradWebPart[i];
                    Session["enderecoPortalWeb"] = enderecoPortalWebPart[i];
                    Session["enderecoChatWeb"] = enderecoChatWebPart[i];
                }
            if (string.IsNullOrEmpty(Session["enderecoRelatorioWeb"] + ""))
            {
                logger.Error("url via HTTP (digitada cliente) - " + url);
                logger.Error("urls web.config - " + enderecoWeb);
                throw new AuthBusinessException(Messages.msgErroUrlDifParametros, AuthBusinessException.TipoErro.AUTORIZACAO_EXPIRADA);
            }
            //logger.Debug("endereco absoluto: " + Session["enderecoWeb"]);

        }


        private void RefreshAccessToken(string key,
            CacheItemUpdateReason reason,
            out object expensiveObject,
            out CacheDependency dependency,
            out DateTime absoluteExpiration,
            out TimeSpan slidingExpiration
        ) {
            IAuthBusiness BusinessAuth = (IAuthBusiness)base.instanciarBusiness<IAuthBusiness>();
            dynamic Cache = this.Cache();
            var remove = Cache["RemoveAccessToken" + ";" + key.Split(';')[1]];
            if(remove == null) {
                string refreshToken = ((AccessTokenResponse) Cache[key]).refresh_token;
                //var resp = (new HttpClient()).PostAsJsonAsync("http://localhost.fiddler:18043/api/auth/refreshaccesstoken", refreshToken).Result;;
                AuthenticationResult result = BusinessAuth.RefreshAccessToken(refreshToken);
                if(result.Status == AuthenticationStatus.OK) {
                    AccessTokenResponse accessTokenResponse = result.Token;
                    expensiveObject = accessTokenResponse;
                    dependency = null;
                    absoluteExpiration = DateTime.UtcNow.AddSeconds(accessTokenResponse.expires_in);
                    slidingExpiration = System.Web.Caching.Cache.NoSlidingExpiration;
                    CacheItemUpdateCallback refreshMethod = new CacheItemUpdateCallback(RefreshAccessToken);
                    Cache.Insert(key, expensiveObject, dependency, absoluteExpiration, slidingExpiration, refreshMethod);
                    return;
                }
            }
            expensiveObject = null;
            dependency = null;
            absoluteExpiration = System.Web.Caching.Cache.NoAbsoluteExpiration;
            slidingExpiration = System.Web.Caching.Cache.NoSlidingExpiration;
            Cache.Remove(key);
            Cache.Remove("RemoveAccessToken" + ";" + key.Split(';')[1]);
        }

        [HttpPost]
        public RenderJsonActionResult postLogout()
        {
            ReturnResult retorno = new ReturnResult();
            try {
                ILogGeralBusiness BusinessLogGeral = (ILogGeralBusiness)base.instanciarBusiness<ILogGeralBusiness>();

                /*if(Session["CodEscolaSelecionada"] != null && Session["CodUsuario"] != null)
                    BusinessLogGeral.geraLogLogout(int.Parse(Session["CodEscolaSelecionada"] + ""), int.Parse(Session["CodUsuario"] + ""));*/
                UserCredential.permissoes = null;
                FormsAuthentication.SignOut();
                try {
                    Session.Clear();
                }
                catch(Exception) { logger.Debug("erro logout: Session.Clear"); }

                try {
                    Session.Abandon();
                }
                catch(Exception) { logger.Debug("erro logout: Session.Abandon"); }

                try {
                    Session.RemoveAll();
                }
                catch(Exception) { logger.Debug("erro logout: Session.RemoveAll"); }

                try {
                    Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId") { Expires = DateTime.Now.AddYears(-1) });
                }
                catch(Exception) { logger.Debug("erro logout: Response.Cookies.Add"); }

                try {
                    Response.RemoveOutputCacheItem(ConfigurationManager.AppSettings["enderecoWeb"] + ConfigurationManager.AppSettings["enderecoWeb"].Replace("/", ""));
                }
                catch(Exception) { logger.Debug("erro logout: Response.RemoveOutputCacheItem"); }
                //Session.Abandon();

                //FormsAuthentication.RedirectToLoginPage();
                return new RenderJsonActionResult { Result = retorno };
            }
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgErroItemNaoEncontrado, retorno, logger, ex);
            }
        }

        public RenderJsonActionResult GetNomeCodigoUsuario()
        {
            ReturnResult retorno = new ReturnResult();

            try
            {
                IAuthBusiness BusinessAuth = (IAuthBusiness)base.instanciarBusiness<IAuthBusiness>();

                retorno.retorno = BusinessAuth.GetNomeCodigoUsuario(User.Identity.Name);
                return new RenderJsonActionResult { Result = retorno };
            }
            
            catch (Exception ex)
            {
                return gerarLogException(Messages.msgErroMenu, retorno, logger, ex);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult LoginChangePassword(AlterarSenha credentials)
        {
             ReturnResult retorno = new ReturnResult();
             try
             {
                 IUsuarioBusiness BusinessUsuario = (IUsuarioBusiness)base.instanciarBusiness<IUsuarioBusiness>();
                 UsuarioWebSGF usuario = BusinessUsuario.GetUsuarioAuthenticateByLogin(credentials.Login);

                 if (ModelState.IsValid && usuario.dc_senha_usuario == BusinessUsuario.GeraSenhaHashSHA1(credentials.SenhaAtual))
                 {
                     AlterarSenhaStatus result = BusinessUsuario.PutUsuarioSenha(credentials.Login, credentials,true);
                     UserCredential user = new UserCredential();
                     user.Login = credentials.Login;
                     user.Password = credentials.NovaSenha;
                     return LoginEscola(user, false);
                 }
                 else
                     retorno.AddMensagem(Messages.msgPasswordNotValid, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
             }
             catch (BusinessException ex)
             {
                 logger.Warn(ex);
                 retorno.AddMensagem(ex.Message, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                 return new RenderJsonActionResult { Result = retorno };
             }
             catch (Exception ex)
             {
                 logger.Error(ex);
                 retorno.AddMensagem(Messages.msgAuthenticationError, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                 return new RenderJsonActionResult { Result = retorno };
             }

            return new RenderJsonActionResult { Result = retorno };
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult trocarSenhaUsuario(SendEmail sendEmail)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IUsuarioBusiness BusinessUsuario = (IUsuarioBusiness)base.instanciarBusiness<IUsuarioBusiness>();

                bool sysAdmin = BusinessUsuario.verificarSysAdmin(sendEmail.login);
                if (sysAdmin)
                    throw new AuthBusinessException(Messages.msgErroAlterarSenhaSysadmin, AuthBusinessException.TipoErro.ALTERAR_SENHA_SYSADMIN);

                SendEmail.configurarEmailSection(sendEmail);

                sendEmail.destinatario = sendEmail.email;

                int tamanho = int.Parse(ConfigurationManager.AppSettings["tamanhoSenha"]);

                UsuarioWebSGF usuario = BusinessUsuario.alterarSenhaUsuario(tamanho, sendEmail);
                retorno.retorno = usuario.cd_usuario;

                if ((usuario.cd_usuario <= 0))
                    retorno.AddMensagem(Messages.msgNotEmailEnviado, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
                else
                    retorno.AddMensagem(Messages.msgEmailEnviado, null, ReturnResult.MensagemWeb.TipoMensagem.INFORMATION);
            
                return new RenderJsonActionResult { Result = retorno };
            }
            catch (BusinessException ex)
            {
                logger.Warn(ex);
                retorno.AddMensagem(ex.Message, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                return new RenderJsonActionResult { Result = new { erro = retorno } };
            }
            catch (Exception ex)
            {
                return gerarLogException(Componentes.Utils.Messages.Messages.msgErroEnviarEmail, retorno, logger, ex);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult postAlteraEmpresaLogada(int id_empresa)
        {
            ReturnResult retorno = new ReturnResult();
            try
            {
                IAuthBusiness BusinessAuth = (IAuthBusiness)base.instanciarBusiness<IAuthBusiness>();
                IEmpresaBusiness BusinessEmpresa = (IEmpresaBusiness)base.instanciarBusiness<IEmpresaBusiness>();
                int cd_usuario;
                bool is_master, is_master_geral;

                if (Session["CodUsuario"] != null)
                {
                    cd_usuario = (int)Session["CodUsuario"];
                    is_master = (bool)Session["IdMaster"];
                    is_master_geral = is_master && (Session["EscolasUsuario"] == null || ((List<EmpresaSession>)Session["EscolasUsuario"]).Count == 0);
                }
                else
                    throw new AuthBusinessException(Messages.msgInfSenhaExpirada, AuthBusinessException.TipoErro.AUTORIZACAO_EXPIRADA);

                //Monta credenciais atualizar a autenticação:
                UserCredential credentials = new UserCredential
                {
                    EhMasterGeral = is_master_geral,
                    CodEmpresa = id_empresa,
                    Login = Session["loginUsuario"] + ""
                };

                EmpresaSession novaEmpresa = BusinessEmpresa.findEmpresaSessionById(id_empresa, cd_usuario, is_master, is_master_geral);
                credentials.IdFusoHorario = (int)Session["IdFusoHorario"];
                credentials.IdHorarioVerao = (bool)Session["IdHorarioVerao"];
                AuthenticationResult authenticationResult = BusinessAuth.renovarAutenticacao(credentials);
                AccessTokenResponse accessTokenResponse = authenticationResult.Token;
                retorno.retorno = novaEmpresa;

                dynamic Cache = this.Cache();
                Cache.Insert("AccessToken" + ";" + Session["UserName"], accessTokenResponse, null, DateTime.UtcNow.AddSeconds(accessTokenResponse.expires_in), System.Web.Caching.Cache.NoSlidingExpiration);

                //Session["UserName"] = credentials.Login;
                //Session["loginUsuario"] = loginUsuario;
                //Session["CodPessoaUsuario"] = codPessoaUsuario;
                //Session["IdMaster"] = idMaster;
                Session["Permissoes"] = authenticationResult.Permissao;
                //Session["CodUsuario"] = cdUsuario;
                //Session["EscolasUsuario"] = escolas;
                Cache.Remove("RemoveAccessToken" + ";" + Session["UserName"]);
                FormsAuthentication.SetAuthCookie(Session["UserName"]+"", false);

                if(novaEmpresa != null){
                    Session["CodEscolaSelecionada"] = novaEmpresa.cd_pessoa;
                    Session["NomeEscolaSelecionada"] = novaEmpresa.dc_reduzido_pessoa;
                    Session["IdFusoHorario"] = credentials.IdFusoHorario;
                    Session["IdHorarioVerao"] = credentials.IdHorarioVerao;
                }
                Session["menusUsuario"] = null;
           
                return new RenderJsonActionResult { Result = retorno };
            }
            catch (BusinessException ex)
            {
                logger.Warn(ex);
                retorno.AddMensagem(ex.Message, null, ReturnResult.MensagemWeb.TipoMensagem.ERROR);
                return new RenderJsonActionResult { Result = new { erro = retorno } };
            }
            catch (Exception ex)
            {
                return gerarLogException(Componentes.Utils.Messages.Messages.msgErroEnviarEmail, retorno, logger, ex);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult postRenovaSessao()
        {
            if (Session["UserName"] != null)
                return new RenderJsonActionResult { Result = "" };
            else
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return new RenderJsonActionResult();
            }
        }
    }
}

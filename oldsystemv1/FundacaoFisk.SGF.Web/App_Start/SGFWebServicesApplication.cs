using System;
using System.Web.Mvc;
using System.Web.Http;
using System.Web.Routing;
using System.Web.Optimization;
using System.Web.Security;
using System.Web;
using System.Security.Principal;
using Newtonsoft.Json;
using FundacaoFisk.SGF.Web.Services.Pessoa.DataAccess;
using System.Configuration;

namespace FundacaoFisk.SGF.Web {
    public class SGFWebServicesApplication : HttpApplication
    {
        static SGFWebServicesApplication() {
            log4net.Config.XmlConfigurator.Configure();
        }

        //public override void Startup() {
        //    base.Startup();
        //}

        protected void Application_Start()
        {
            SimpleInjectorInitializer.Initialize();
            AreaRegistration.RegisterAllAreas();
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            FormattersConfig.RegisterConfigureGlobalFormatters(GlobalConfiguration.Configuration.Formatters);
            FilterConfig.RegisterConfigureGlobalFilters(GlobalFilters.Filters, GlobalConfiguration.Configuration.Filters);
            RouteConfig.Registerroutes(RouteTable.Routes);
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Formatting = Newtonsoft.Json.Formatting.Indented,
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            };
            //GlobalConfiguration.Configuration(WebApiConfig.Register);
            //GlobalConfiguration.Configure(WebApiConfig.Register);
            WebApiConfig.Register(GlobalConfiguration.Configuration);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            
            Exception exc = Server.GetLastError().GetBaseException();
            if (exc is System.Web.HttpUnhandledException || exc is System.Web.HttpException)
            {
                Server.ClearError();
                //Response.Redirect(ConfigurationManager.AppSettings["enderecoRetornoErro"].Replace("/Erro/Index", "/Auth/Login"), true);
            }

            log4net.ILog logger = log4net.LogManager.GetLogger("App_Error");
            logger.Error("Erro Crítico: ", exc);
        }

        protected void Session_Start(Object sender, EventArgs e) {
            //System.Web.Security.FormsAuthentication.RedirectToLoginPage();
            Response.Redirect(ConfigurationManager.AppSettings["enderecoRelativoWeb"] + "/Auth/Login", true);
        }

        protected void Session_End(Object sender, EventArgs e) {
            System.Web.HttpRuntime.Cache.Insert("RemoveAccessToken" + ";" + Session["UserName"], true);
        }

        //protected void Application_AuthenticateRequest(object sender, EventArgs e) {
        //    if (HttpContext.Current.User != null) {
        //        if (HttpContext.Current.User.Identity.IsAuthenticated) {
        //            if (HttpContext.Current.User.Identity is FormsIdentity) {
        //                FormsIdentity identity = (FormsIdentity)HttpContext.Current.User.Identity;
        //                FormsAuthenticationTicket ticket = identity.Ticket;
        //                int codPessoaPai = int.Parse(ticket.UserData);
        //                HttpContext.Current.User = new SGFPrincipal(identity, codPessoaPai);
        //            }
        //        }
        //    }
        //}

        //protected void Application_AuthenticateRequest(Object sender, EventArgs e) {
        //    // Extract the forms authentication cookie
        //    string cookieName = FormsAuthentication.FormsCookieName;
        //    HttpCookie authCookie = Request.Cookies[cookieName];
        //    if (null == authCookie) {
        //        // There is no authentication cookie.
        //        return;
        //    }
        //    FormsAuthenticationTicket authTicket = null;
        //    try {
        //        authTicket = FormsAuthentication.Decrypt(authCookie.Value);
        //    }
        //    catch (Exception ex) {
        //        // Log exception details (omitted for simplicity)
        //        return;
        //    }
        //    if (null == authTicket) {
        //        // Cookie failed to decrypt.
        //        return;
        //    }
        //    // When the ticket was created, the UserData property was assigned
        //    // a pipe delimited string of role names.
        //    string[] roles = new string[] { };
        //    //roles[0] = "One"
        //    //roles[1] = "Two"
        //    // Create an Identity object
        //    FormsIdentity id = new FormsIdentity(authTicket);
        //    // This principal will flow throughout the request.
        //    GenericPrincipal principal = new GenericPrincipal(id, roles);
        //    // Attach the new principal object to the current HttpContext object
        //    Context.User = principal;
        //}
    }
}
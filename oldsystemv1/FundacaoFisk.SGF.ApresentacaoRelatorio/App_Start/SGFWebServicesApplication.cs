using System;
using System.Web.Mvc;
using System.Web.Http;
using System.Web.Routing;
using System.Web.Optimization;
using System.Web.Security;
using System.Web;
using System.Security.Principal;
using System.Diagnostics;
using System.Configuration;
using log4net;

namespace FundacaoFisk.SGF.ApresentacaoRelatorio {
    public class SGFWebServicesApplication : HttpApplication {
        static SGFWebServicesApplication() {
            log4net.Config.XmlConfigurator.Configure();
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            FormattersConfig.RegisterConfigureGlobalFormatters(GlobalConfiguration.Configuration.Formatters);
            FilterConfig.RegisterConfigureGlobalFilters(GlobalFilters.Filters, GlobalConfiguration.Configuration.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            //mataProcessosWord();
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exc = Server.GetLastError().GetBaseException();
            if (exc is Microsoft.Reporting.WebForms.AspNetSessionExpiredException || exc is System.Web.HttpUnhandledException)
            {
                Server.ClearError();
                Response.Redirect(ConfigurationManager.AppSettings["enderecoRetornoErro"].Replace("/Erro/Index", "/Auth/Login"), true);
            }
            
            log4net.ILog logger = log4net.LogManager.GetLogger("App_Error");
            logger.Error("Erro Crítico: ", exc);
        }

        private void mataProcessosWord()
        {
            try
            {
                string mataProcessosWord = ConfigurationManager.AppSettings["mataProcessosWord"];

                if ("true".Equals(mataProcessosWord))
                {
                    //Iterate through all Excel processes on local machine
                    foreach (Process proc in Process.GetProcessesByName("winword"))
                       //Only work with Processes started by ASPNET
                       if (proc.StartInfo.UserName.Equals("ASPNET", StringComparison.InvariantCultureIgnoreCase))
                            //Impersonate a user that has permissions to kill the process
                            using (WindowsImpersonationContext impCtx = (HttpContext.Current.User.Identity as    WindowsIdentity).Impersonate())
                            {
                              //Kill process code
 
                              impCtx.Undo();
                            }
                }
            }
            catch(Exception e){
                ILog logger = LogManager.GetLogger(typeof(SGFWebServicesApplication));
                logger.Error(e);    
            }
        }

        protected void Session_Start(Object sender, EventArgs e) {
            string sessionId = Session.SessionID;
        }

        protected void Session_End(Object sender, EventArgs e) {
            //System.Web.HttpRuntime.Cache.Insert("RemoveAccessToken" + ";" + Session["UserName"], true);
        }
    }
}
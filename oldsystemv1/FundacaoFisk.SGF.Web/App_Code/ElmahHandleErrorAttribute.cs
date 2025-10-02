using Elmah;
using System.Web.Mvc;
using System.Net;
using log4net;

namespace FundacaoFisk.SGF.Web {
    [System.AttributeUsage(System.AttributeTargets.Method | System.AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class ElmahHandleErrorAttribute : HandleErrorAttribute {
        private static readonly ILog logger = LogManager.GetLogger(typeof(ElmahHandleErrorAttribute));

        private static bool IsFiltered(ExceptionContext context) {
            ErrorFilterConfiguration section = System.Web.HttpContext.Current.GetSection("elmah/errorFilter") as ErrorFilterConfiguration;
            if(section == null) {
                return false;
            }
            ErrorFilterModule.AssertionHelperContext context2 = new ErrorFilterModule.AssertionHelperContext((System.Exception) context.Exception, System.Web.HttpContext.Current);
            return section.Assertion.Test(context2);
        }

        private static void LogException(System.Exception e) {
            System.Web.HttpContext current = System.Web.HttpContext.Current;
            ErrorLog.GetDefault((System.Web.HttpContext) current).Log(new Error((System.Exception) e, (System.Web.HttpContext) current));
        }

        public override void OnException(ExceptionContext filterContext) {
            base.OnException(filterContext);
            System.Exception e = filterContext.Exception;
            if(!RaiseErrorSignal(e) && !IsFiltered(filterContext)) 
                LogException(e);
            if(filterContext.HttpContext != null && filterContext.HttpContext.Request != null)
                logger.Error("ERRO CRÍTICO - URL: " + ((((System.Web.Mvc.ControllerContext)(filterContext)).HttpContext).Request).Url, e);
        }

        private static bool RaiseErrorSignal(System.Exception e) {
            System.Web.HttpContext current = System.Web.HttpContext.Current;
            if(current == null) {
                return false;
            }
            ErrorSignal signal = ErrorSignal.FromContext((System.Web.HttpContext) current);
            if(signal == null) {
                return false;
            }
            signal.Raise((System.Exception) e, (System.Web.HttpContext) current);
            return true;
        }
    }
}
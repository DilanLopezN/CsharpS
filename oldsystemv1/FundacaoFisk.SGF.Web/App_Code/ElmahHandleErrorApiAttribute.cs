using Elmah;
using System.Web.Http.Filters;

namespace FundacaoFisk.SGF.Web {
    [System.AttributeUsage(System.AttributeTargets.Method | System.AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class ElmahHandleErrorApiAttribute : ExceptionFilterAttribute {
        private static bool IsFiltered(HttpActionExecutedContext context) {
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

        public override void OnException(HttpActionExecutedContext actionExecutedContext) {
            base.OnException(actionExecutedContext);
            System.Exception e = actionExecutedContext.Exception;
            if(!RaiseErrorSignal(e) && !IsFiltered(actionExecutedContext)) {
                LogException(e);
            }
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
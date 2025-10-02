using System.Web;
using System.Web.Mvc;
using Componentes.Utils;
using System.Web.Http.Filters;
using Microsoft.Practices.ServiceLocation;

namespace FundacaoFisk.SGF.Web {
    public class FilterConfig {
        public static void RegisterConfigureGlobalFilters(GlobalFilterCollection mvcFilters, HttpFilterCollection apiFilters) {
            mvcFilters.Add(new ElmahHandleErrorAttribute());
            mvcFilters.Add(new AuthorizeAttribute());
            apiFilters.Add(new ElmahHandleErrorApiAttribute());
            apiFilters.Add(new AccessTokenValidationFilterAttribute(ServiceLocator.Current.GetInstance<IAesCryptoHelper>(), ServiceLocator.Current.GetInstance<Services.InstituicaoEnsino.Comum.IBusiness.IEscolaBusiness>()));
        }
    }
}
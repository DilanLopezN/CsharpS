using System.Web;
using System.Web.Mvc;
using Componentes.Utils;
using System.Web.Http.Filters;

namespace FundacaoFisk.SGF.ApresentacaoRelatorio {
    public class FilterConfig {
        public static void RegisterConfigureGlobalFilters(GlobalFilterCollection mvcFilters, HttpFilterCollection apiFilters) {
            mvcFilters.Add(new ElmahHandleErrorAttribute());
            apiFilters.Add(new ElmahHandleErrorApiAttribute());                                                          
        }
    }
}
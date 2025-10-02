using System.Net.Http.Formatting;
using ComponentesApiController.App_Start;

namespace FundacaoFisk.SGF.Web {
    public class FormattersConfig {
        public static void RegisterConfigureGlobalFormatters(MediaTypeFormatterCollection formatters) {
            formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            formatters.Add(new ArquivoMediaTypeFormatter());
        }
    }
}
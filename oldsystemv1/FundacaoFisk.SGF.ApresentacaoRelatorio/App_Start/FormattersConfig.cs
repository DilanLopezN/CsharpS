using System.Net.Http.Formatting;

namespace FundacaoFisk.SGF.ApresentacaoRelatorio {
    public class FormattersConfig {
        public static void RegisterConfigureGlobalFormatters(MediaTypeFormatterCollection formatters) {
            formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        }
    }
}
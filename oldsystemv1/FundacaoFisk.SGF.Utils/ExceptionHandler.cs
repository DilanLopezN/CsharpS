using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.Utils
{
    public class ExceptionHandler
    {
        public string Message { get; set; }
        public string ExceptionMessage { get; set; }
        public string ExceptionType { get; set; }
        public string StackTrace { get; set; }

        public ExceptionHandler(string message, string exceptionMessage, string exceptionType, string stackTrace)
        {
            this.Message = message;
            this.ExceptionMessage = exceptionMessage;
            this.ExceptionType = exceptionType;
            this.StackTrace = stackTrace;
        }
    }
}

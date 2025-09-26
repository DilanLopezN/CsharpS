using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simjob.Framework.Domain.Models.Analyze
{
    public class AnalyzeInput
    {
        public AnalyzeInput()
        {
            Props = new List<AnalyzeProp>();
            Fields = new string[] { };
        }
        public AnalyzeInput(AnalyzeNamespace @namespace = null, string command = null, List<AnalyzeProp> props = null)
        {
            Namespace = @namespace ?? new AnalyzeNamespace();
            //Props = props ?? new List<AnalyzeProp>();
            //Command = command ;
        }

        public AnalyzeNamespace Namespace { get; set; }
        public List<AnalyzeProp> Props { get; set; }
        public string[] Fields { get; set; }
        public bool Pivot { get; set; }
        public string PivotField { get; set; }
        public string PivotProp { get; set; }
        public string PivotValue { get; set; }
        public string Command { get; set; }
    }
}

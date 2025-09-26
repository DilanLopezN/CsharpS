using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simjob.Framework.Domain.Models.Analyze
{
    public class AnalyzeFilter
    {
        public string Prop { get; set; }
        public string FieldFilter { get; set; }
        public string Value { get; set; }
    }
}

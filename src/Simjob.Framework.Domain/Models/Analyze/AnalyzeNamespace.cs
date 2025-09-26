using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simjob.Framework.Domain.Models.Analyze
{
    public class AnalyzeNamespace
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public AnalyzeAccess Access { get; set; }
    }
}

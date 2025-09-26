using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simjob.Framework.Domain.Models.Analyze
{
    public class AnalyzeProp
    {
        public AnalyzeProp()
        {
            Combo = new string[] { };
        }

        public string EntityName { get; set; }
        public string PropertyText { get; set; }
        public string Type { get; set; }
        public string Prop { get; set; }
        public string FieldFilter { get; set; }
        public string Label { get; set; }
        public string Cond { get; set; }
        public string[] Combo { get; set; }
        public bool IsRequired { get; set; }
        public string Between { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simjob.Framework.Domain.Models
{
    public class ItemRuleFilterModel
    {
        public string Local { get; set; } = "";
        public string Familia { get; set; } = "";
        public string ItemId { get; set; } = "";
        public string ItemDescription { get; set; } = "";
        public int? Page { get; set; }
        public int? Limit { get; set; }
    }
}

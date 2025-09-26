using System.Collections.Generic;

namespace Simjob.Framework.Domain.Models
{
    public class JoinsModel
    {
        public List<JoinsFields> Joins { get; set; }
        public OrderField Order { get; set; }
        public List<WhereCondition> Where { get; set; }
        public List<WhereCondition> WhereOr { get; set; }
        public int? Page { get; set; } = 1;
        public int? Limit { get; set; } = 10;

        public class JoinsFields
        {
            public string Key { get; set; }
            public string Schema { get; set; }
            public string ForeignKey { get; set; }
            public string? Fields { get; set; }
        }
        public class OrderField
        {
            public string Field { get; set; }
            public string Sort { get; set; }
        }
        public class WhereCondition
        {
            public string Field { get; set; }
            public string Operation { get; set; }
            public string Value { get; set; }
        }
    }
}

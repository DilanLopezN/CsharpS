using Simjob.Framework.Domain.Core.Entities;
using System.Collections.Generic;

namespace Simjob.Framework.Domain.Models
{
    public class Searchdefs : Entity
    {
        public string UserId { get; set; }
        public string SchemaName { get; set; }
        public string Description { get; set; }
        public Def Defs { get; set; }

        public Searchdefs(string schemaName, string userId, string description, Def def)
        {
            SchemaName = schemaName;
            UserId = userId;
            Description = description;
            Defs = def;
        }
        public Searchdefs()
        {

        }

        public class OrderBy
        {
            public string field { get; set; }
            public string direction { get; set; }
        }

        public class Filter
        {
            public string field { get; set; }
            public string @operator { get; set; }
            public string value { get; set; }
        }

        public class Def
        {
            public List<string> userColumns { get; set; }
            public List<OrderBy> orderBy { get; set; }
            public List<Filter> filters { get; set; }

            public Def(List<string> usercolumns, List<OrderBy> orderby, List<Filter> filts)
            {
                userColumns = usercolumns;
                orderBy = orderby;
                filters = filts;
            }
        }


    }

}
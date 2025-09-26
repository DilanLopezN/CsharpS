using MongoDB.Bson;
using MongoDB.Driver;
using Simjob.Framework.Domain.Core.Commands;
using Simjob.Framework.Domain.Core.Entities;
using Simjob.Framework.Infra.Schemas.Commands.Entities;
using Simjob.Framework.Infra.Schemas.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Schemas.Commands.Views
{
    public class InsertViewCommand : Command
    {
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Description { get; set; }
        public ViewParameter[] Parameters { get; set; }
        public string Query { get; set; }
        public string SchemaName { get; set; }
        public string Type { get; set; }
    }
}

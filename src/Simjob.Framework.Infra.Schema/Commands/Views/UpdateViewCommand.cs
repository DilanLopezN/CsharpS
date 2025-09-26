using MongoDB.Bson;
using MongoDB.Driver;
using Simjob.Framework.Domain.Core.Commands;
using Simjob.Framework.Domain.Core.Entities;
using Simjob.Framework.Infra.Schemas.Commands.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Schemas.Commands.Views
{
    public class UpdateViewCommand : InsertViewCommand
    {
        public string Id { get; set; }
    }
}

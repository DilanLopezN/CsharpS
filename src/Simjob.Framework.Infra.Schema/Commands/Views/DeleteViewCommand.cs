using Simjob.Framework.Domain.Core.Commands;
using Simjob.Framework.Domain.Core.Entities;

namespace Simjob.Framework.Infra.Schemas.Commands.Views
{
    public class DeleteViewCommand : InsertViewCommand
    {
        public string Id { get; set; }
    }
}

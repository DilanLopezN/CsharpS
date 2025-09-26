using Simjob.Framework.Domain.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Identity.Entities
{
    public class Module : Entity
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Type { get; set; } //Addon, Component, None, Schema, Action
        public string? Icon { get; set; }
        public bool? Active { get; set; }
        public string? ModuleId { get; set; }
        public string? ActionId { get; set; }
        public string? Path { get; set; }
        public decimal? Price { get; set; }
        public int? Order { get; set; }

    }
}

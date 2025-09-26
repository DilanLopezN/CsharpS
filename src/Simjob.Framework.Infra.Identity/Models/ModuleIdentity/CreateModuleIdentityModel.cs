using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simjob.Framework.Infra.Identity.Models.ModuleIdentity
{
    public class CreateModuleIdentityModel
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Type { get; set; } //Addon, Component, None, Schema, Action
        public string? Icon { get; set; }
        //      public bool? Active { get; set; }
        public string? ModuleId { get; set; }
        public string? ActionId { get; set; }
        public string? Path { get; set; }
        public string Tenanty { get; set; }
        public decimal? Price { get; set; }

    }
}

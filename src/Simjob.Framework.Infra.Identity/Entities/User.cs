using Simjob.Framework.Domain.Core.Entities;
using System.Collections.Generic;
using System.Security.Claims;

namespace Simjob.Framework.Infra.Identity.Entities
{
    public class User : Entity
    {
        public User()
        {
            Claims = new List<Claim>();
        }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Tenanty { get; set; }
        public string? ApiKey { get; set; }
        public string Telefone { get; set; }
        public string CompanySiteIdDefault { get; set; }
        public string[] CompanySiteIds { get; set; }
        public List<Claim> Claims { get; set; }
        public string Hash { get; set; }
        public string SuperiorUser { get; set; }
        public string Function { get; set; }
        public string Picture { get; set; }
        public int A2f { get; set; }
        public string GroupId { get; set; }
        public bool Root { get; set; }
        public bool ControlAccess { get; set; } = false;
        public bool FirstLogin { get; set; }
        public bool LogonAzure { get; set; }
        public string HashAzure { get; set; }
        public string RevendaId { get; set; }
        public string NivelId { get; set; }
        public string Cd_pessoa { get; set; }
        public string Cd_usuario { get; set; }
        public bool UsuarioMatriz { get; set; }

    }
}

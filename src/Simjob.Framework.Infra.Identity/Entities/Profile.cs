using Simjob.Framework.Domain.Core.Entities;
using System.Diagnostics.CodeAnalysis;

namespace Simjob.Framework.Infra.Identity.Entities
{
    public class Profile : Entity
    {
        public string NomeEmpresa { get; set; }
        public string Color { get; set; }
        public string Tenanty { get; set; }
        public string Logo { get; set; }
        public string Banner { get; set; }

        public string[] Dominio { get; set; }


        [ExcludeFromCodeCoverage]
        public Profile(string nomeEmpresa, string color, string banner, string tenanty, string[] dominio)
        {
            NomeEmpresa = nomeEmpresa;
            Color = color;
            Tenanty = tenanty;
            Banner = banner;
            Dominio = dominio;
        }

        public Profile(string logo, string tenanty)
        {
            Logo = logo;
            Tenanty = tenanty;
        }

    }
}

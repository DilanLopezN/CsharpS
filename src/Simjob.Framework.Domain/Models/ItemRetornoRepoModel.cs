using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simjob.Framework.Domain.Models
{
    public class ItemRetornoRepoModel
    {
        public string? Id { get; set; }
        public string? Item { get; set; }
        public string? DescricaoItem { get; set; }
        public string? LocalId { get; set; }
        public string? Local { get; set; }
        public string? DescricaoLocal { get; set; }
        public string? Unidade { get; set; }
        public double? QtdMin { get; set; }
        public double? QtdMax { get; set; }
        public double? QtdEmEstoque { get; set; }
        public double? QtdPendentePicking { get; set; }
        public string? Seg { get; set; }
        public List<OrigensRepoModel>? OrigensReposicao { get; set; } = new List<OrigensRepoModel>();


        public class OrigensRepoModel
        {
            public string? LocalId { get; set; }
            public string? LoteId { get; set; }
            public string? LoteCode { get; set; }
            public string? Local { get; set; }
            public double? QtdDisp { get; set; }
            public DateTime? Validade { get; set; }
        }
    }
}

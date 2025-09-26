using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Simjob.Framework.Domain.Models
{
    public class ItemRuleModel
    {
        public string? Id { get; set; }
        public string? CodProduto { get; set; }
        public string? Descricao { get; set; }
        public string? LocalId { get; set; }
        public string? Local { get; set; }
        public string? DescricaoLocal { get; set; }
        public string? Unidade { get; set; }
        public string? Seg { get; set; }
        public double? QtdMin { get; set; }
        public double? QtdMax { get; set; }
    }

    public class ItemQtdPend
    {
        public string? Id { get; set; }
        public string? ItemId { get; set; }
        public double TotalQtdPendente { get; set; }
        public string? Seg { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simjob.Framework.Application.Stock.Models
{
    public class SaldoModel
    {
        public double? totalQuantidade { get; set; }
        public string? itemCode { get; set; }
        public string? itemDescription { get; set; }
        public string? unidade { get; set; }
        public string? code_Lote { get; set; }
        public DateTime? validadeLote { get; set; }
        public string? stockLocalCode { get; set; }
        public string? idItem { get; set; }
        public string? idLocal { get; set; }
        public string? idLote { get; set; }
    }
}

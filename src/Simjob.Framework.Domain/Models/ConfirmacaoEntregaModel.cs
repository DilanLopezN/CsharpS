using System;

namespace Simjob.Framework.Domain.Models
{
    public class ConfirmacaoEntregaModel
    {
        public string? Id { get; set; }
        public string? Code { get; set; }
        public string? Status { get; set; }
        public string? Status_unidade { get; set; }
        public string? Nf { get; set; }
        public DateTime? CreateAt { get; set; }
    }
}

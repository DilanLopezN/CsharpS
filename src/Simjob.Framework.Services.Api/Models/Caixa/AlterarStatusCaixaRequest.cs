using System.ComponentModel.DataAnnotations;

namespace Simjob.Framework.Services.Api.Models.Caixa
{
    public class AlterarStatusCaixaRequest
    {
        [Required(ErrorMessage = "NovoStatus é obrigatório")]
        [Range(1, 3, ErrorMessage = "NovoStatus deve ser 1 (Em Aberto), 2 (Aguardando Validação) ou 3 (Fechado)")]
        public int NovoStatus { get; set; }
    }
}

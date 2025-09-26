using System.ComponentModel.DataAnnotations;

namespace Simjob.Framework.Services.Api.Models
{
    public class SolicitarFechamentoRequest
    {
        [Required(ErrorMessage = "O código da pessoa que está solicitando o fechamento é obrigatório")]
        public int cd_pessoa_solicitacao_fechamento { get; set; }

        [Required(ErrorMessage = "O valor do saldo real é obrigatório")]
        public decimal vl_saldo_real { get; set; }
    }
}

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Simjob.Framework.Services.Api.Models.Caixa
{
    public class ReceberTitulosRequest
    {
        [Required(ErrorMessage = "Código do caixa é obrigatório")]
        public int cd_caixa { get; set; }

        [Required(ErrorMessage = "Lista de títulos é obrigatória")]
        [MinLength(1, ErrorMessage = "Deve ser informado pelo menos um título")]
        public List<int> cd_titulos { get; set; }

    }
}

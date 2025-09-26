using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Simjob.Framework.Services.Api.Models.Contas
{
    public class CancelamentoMultiploTitulosModel
    {
        [Required]
        public int cd_pessoa_empresa { get; set; }

        [Required]
        public DateTime dt_cancelamento { get; set; }

        [Required]
        public List<int> cd_titulos { get; set; } = new List<int>();

        public string motivo_cancelamento { get; set; }
    }
}

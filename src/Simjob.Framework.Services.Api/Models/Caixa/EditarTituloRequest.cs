using System;
using System.ComponentModel.DataAnnotations;

namespace Simjob.Framework.Services.Api.Models.Caixa
{
    public class EditarTituloRequest
    {
        // Campos para editar T_TITULO
        [Range(1, 2, ErrorMessage = "Natureza do título deve ser 1 (Recebimento) ou 2 (Pagamento)")]
        public int? id_natureza_titulo { get; set; }

        public int? cd_local_movto { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Valor do título deve ser maior que zero")]
        public decimal? vl_titulo { get; set; }

        public int? cd_pessoa_responsavel { get; set; }

        // Campos para editar T_CAIXA_TITULO
        public DateTime? dt_recebimento { get; set; }

        public int? cd_tipo_liquidacao { get; set; }

        [MaxLength(500, ErrorMessage = "Descrição do título não pode ter mais de 500 caracteres")]
        public string dc_titulo { get; set; }
    }
}

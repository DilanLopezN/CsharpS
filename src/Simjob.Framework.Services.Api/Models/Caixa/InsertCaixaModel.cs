using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Simjob.Framework.Services.Api.Models.Caixa
{
    public class InsertCaixaModel
    {
        [Required(ErrorMessage = "cd_empresa é obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "cd_empresa deve ser maior que zero")]
        public int cd_empresa { get; set; }
        
        [Required(ErrorMessage = "cd_local_movto é obrigatório")]
        public int cd_local_movto { get; set; }
        public string dc_caixa { get; set; }
        
        public DateTime? dt_abertura { get; set; } // Se não informado, usa data atual
        
        public bool? id_caixa_central { get; set; } // Indica se é um caixa central (opcional, padrão false)
        
        public List<int> cd_pessoa_list { get; set; }
    }
}

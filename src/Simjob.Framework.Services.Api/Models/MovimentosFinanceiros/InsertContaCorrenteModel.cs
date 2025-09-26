using System;
using System.ComponentModel.DataAnnotations;

namespace Simjob.Framework.Services.Api.Models.MovimentosFinanceiros
{
    public class InsertContaCorrenteModel
    {
        public int? cd_conta_corrente { get; set; }
        
        [Required(ErrorMessage = "cd_local_origem é obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "cd_local_origem deve ser maior que zero")]
        public int cd_local_origem { get; set; }
        
        [Range(1, int.MaxValue, ErrorMessage = "cd_local_destino deve ser maior que zero")]
        public int? cd_local_destino { get; set; }
        
        [Required(ErrorMessage = "cd_movimentacao_financeira é obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "cd_movimentacao_financeira deve ser maior que zero")]
        public int cd_movimentacao_financeira { get; set; }
        
        public int? cd_baixa_titulo { get; set; }
        
        public DateTime? dta_conta_corrente { get; set; }
        
        [Required(ErrorMessage = "id_tipo_movimento é obrigatório")]
        [Range(1, 255, ErrorMessage = "id_tipo_movimento deve estar entre 1 e 255")]
        public byte id_tipo_movimento { get; set; }
        
        [Range(1, int.MaxValue, ErrorMessage = "cd_pessoa_empresa deve ser maior que zero")]
        public int? cd_pessoa_empresa { get; set; }
        
        [Range(1, int.MaxValue, ErrorMessage = "cd_plano_conta deve ser maior que zero")]
        public int? cd_plano_conta { get; set; }
        
        public decimal? vl_conta_corrente { get; set; }
        
        [MaxLength(255, ErrorMessage = "dc_obs_conta_corrente deve ter no máximo 255 caracteres")]
        public string dc_obs_conta_corrente { get; set; }
        
        [Required(ErrorMessage = "cd_tipo_liquidacao é obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "cd_tipo_liquidacao deve ser maior que zero")]
        public int cd_tipo_liquidacao { get; set; }

        public int? cd_caixa { get; set; }
    }
}

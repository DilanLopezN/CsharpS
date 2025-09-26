using System;

namespace Simjob.Framework.Services.Api.Models.Contas
{
    public class BaixaContaModel
    {
        public int? cd_tran_finan { get; set; }
        public int cd_pessoa_empresa { get; set; }
        public int? cd_local_movto { get; set; }
        public int cd_tipo_liquidacao { get; set; }
        public DateTime dt_baixa { get; set; }
        public decimal vl_total_baixa { get; set; }
        public Gridbaixaefetuada[] gridBaixaEfetuada { get; set; }
        public int cd_tipo_liquidacao_old { get; set; }
        public int? cd_caixa { get; set; }

        public class Gridbaixaefetuada
        {
            public int cd_titulo { get; set; }
            public int nm_titulo { get; set; }
            public int nm_parcela_titulo { get; set; }
            public decimal vl_liquidacao_baixa { get; set; }
            public decimal vl_liquidacao_calculado { get; set; }
            public decimal vl_juros_baixa { get; set; }
            public decimal vl_multa_baixa { get; set; }
            public decimal vl_juros_calculado { get; set; }
            public decimal vl_multa_calculada { get; set; }
            public decimal vl_desc_juros_baixa { get; set; }
            public decimal vl_desc_multa_baixa { get; set; }
            public decimal vl_desconto_baixa { get; set; }
            public decimal vl_desconto_baixa_calculado { get; set; }
            public decimal vl_principal_baixa { get; set; }
            public decimal vl_baixa_saldo_titulo { get; set; }
            public decimal vl_acr_liquidacao { get; set; }
            public decimal vl_saldo_titulo { get; set; }
            public decimal vl_taxa_cartao { get; set; }
            public decimal pc_pontualidade { get; set; }
            public bool id_parcial { get; set; }
            public string dc_natureza { get; set; }
            public DateTime dt_vcto_titulo { get; set; }
            public string txt_obs_baixa { get; set; }
            public int cd_tipo_financeiro { get; set; }
            public int? cd_politica_desconto { get; set; }
            public int cd_tipo_liquidacao { get; set; }
            public string? tx_obs_baixa { get; set; }
            public int? cd_local_movto { get; set; }
            public int id_baixa_parcial { get; set; }

        }

    }
}

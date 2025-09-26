using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Simjob.Framework.Services.Api.Models.Contas
{
    public class InsertContaAPagarModel
    {
        [Required]
        public int cd_pessoa_empresa { get; set; }
        [Required]
        public int cd_pessoa_titulo { get; set; }
        [Required]
        public int cd_pessoa_responsavel { get; set; }
        [Required]
        public int cd_local_movto { get; set; }
        public DateTime? dt_emissao_titulo { get; set; }
        public int? cd_origem_titulo { get; set; }
        [Required]
        public DateTime dt_vcto_titulo { get; set; }

        public DateTime? dh_cadastro_titulo { get; set; }
        [Required]
        public decimal vl_titulo { get; set; }
        public string? dc_codigo_barra { get; set; }
        public string? dc_nosso_numero { get; set; }
        public string? dc_num_documento_titulo { get; set; }
        public decimal? vl_saldo_titulo { get; set; }
        public int? nm_titulo { get; set; }
        public int? nm_parcela_titulo { get; set; }
        [Required]
        public int cd_tipo_financeiro { get; set; }
        public int? id_status_titulo { get; set; }

        [Required]
        public int id_status_cnab { get; set; }
        public int? id_origem_titulo { get; set; }
        public int? id_natureza_titulo { get; set; } = 2;
        [Required]
        public decimal vl_multa_titulo { get; set; }
        [Required]
        public decimal vl_juros_titulo { get; set; }

        [Required]
        public decimal vl_desconto_titulo { get; set; }
        [Required]
        public decimal vl_liquidacao_titulo { get; set; }
        [Required]
        public decimal vl_multa_liquidada { get; set; }
        [Required]
        public decimal vl_juros_liquidado { get; set; }
        [Required]
        public decimal vl_desconto_juros { get; set; }
        [Required]
        public decimal vl_desconto_multa { get; set; }
        [Required]
        public float pc_juros_titulo { get; set; }
        [Required]
        public float pc_multa_titulo { get; set; }
        [Required]
        public decimal vl_material_titulo { get; set; }
        [Required]
        public decimal vl_abatimento { get; set; }
        [Required]
        public decimal vl_desconto_contrato { get; set; }
        [Required]
        public float pc_taxa_cartao { get; set; }
        [Required]
        public int nm_dias_cartao { get; set; }
        [Required]
        public bool id_cnab_contrato { get; set; }
        [Required]
        public decimal vl_taxa_cartao { get; set; }
        public int? cd_aluno { get; set; }
        [Required]
        public decimal pc_responsavel { get; set; }
        public decimal? vl_mensalidade { get; set; }
        public decimal? pc_bolsa { get; set; }
        public decimal? vl_bolsa { get; set; }
        public decimal? pc_desconto_mensalidade { get; set; }
        public decimal? vl_desconto_mensalidade { get; set; }
        public decimal? pc_bolsa_material { get; set; }
        public decimal? vl_bolsa_material { get; set; }
        public decimal? pc_desconto_material { get; set; }
        public decimal? vl_desconto_material { get; set; }
        public decimal? pc_desconto_total { get; set; }
        public decimal? vl_desconto_total { get; set; }
        public int? opcao_venda { get; set; }
        public int? cd_curso { get; set; }

        public List<PlanoTituloContaPagarModel>? plano_titulo { get; set; }
    }

    public class PlanoTituloContaPagarModel
    {
        public int cd_plano_conta { get; set; }
        public decimal vl_plano_titulo { get; set; }
    }
}

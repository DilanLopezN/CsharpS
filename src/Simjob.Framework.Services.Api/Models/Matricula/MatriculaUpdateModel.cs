using System;
using System.Collections.Generic;

namespace Simjob.Framework.Services.Api.Models.Matricula
{
    public class MatriculaUpdateModel
    {

        public int cd_contrato { get; set; }
        public string? cd_usuario { get; set; }      
        public string? cd_pessoa_escola { get; set; }
        public DateTime? dt_vencimento_parcela_1 { get; set; }
        public DateTime? dt_vencimento_parcela_1_material { get; set; }       
        public decimal? pc_responsavel_contrato { get; set; }
        public int? id_tipo_contrato { get; set; }
        public decimal? vl_parcela_contrato { get; set; }      
        public decimal? vl_parcela_liquida { get; set; }
        public decimal? pc_desconto_contrato { get; set; }
        public decimal? vl_desconto_contrato { get; set; }
        public int? cd_nome_contrato { get; set; }       
        public int id_tipo_matricula { get; set; }
        public DateTime dt_inicial_contrato { get; set; }
        public string? id_retorno { get; set; }
        public string? id_nf_servico { get; set; }
        public int cd_aluno { get; set; }      
        public DateTime? dt_final_contrato { get; set; }
        public string? cd_produto_atual { get; set; }      
        public string? cd_fila_matricula { get; set; }
        public decimal? vl_curso_contrato { get; set; }
        public decimal? vl_liquido_contrato { get; set; }      
        public int? id_transferencia { get; set; }       
        public decimal? pc_desconto_bolsa { get; set; }       
        public int? cd_ano_escolar { get; set; }      
        public string? nm_mes_curso_inicial { get; set; }
        public string? nm_ano_curso_inicial { get; set; }
        public string? nm_mes_curso_final { get; set; }
        public string? nm_ano_curso_final { get; set; }
        public string? nm_arquivo_digitalizado { get; set; }
        public string? nm_parcelas_material { get; set; }
        public decimal? vl_parcela_material { get; set; }
        public decimal? vl_material_contrato { get; set; }
        public decimal? vl_parcela_liq_material { get; set; }
        public decimal? pc_bolsa_material { get; set; }
        public int? id_tipo_data_inicio { get; set; }
        public string? nm_dia_vcto_desconto { get; set; }
        public string? nm_previsao_inicial { get; set; }
        public decimal? vl_aula_hora { get; set; }
        public string? tx_obs_contrato { get; set; }
        public decimal? pc_desconto_material { get; set; }
        public decimal? vl_liquido_material { get; set; }
        public decimal? vl_desconto_material { get; set; }
        public int? id_opcao_venda { get; set; }
        public int? cd_tipo_financeiro_material { get; set; }
        public string? cd_pessoa_responsavel_material { get; set; }
        public string? cd_pessoa_responsavel { get; set; }
        public decimal? pc_responsavel_material { get; set; } 
        public string? cd_curso_atual { get; set; }
        public string? cd_duracao_atual { get; set; }
        public string? cd_regime_atual { get; set; }
        public string? cd_tipo_financeiro { get; set; }
        public DateTime? dt_matricula_contrato { get; set; }
        public string? nm_parcelas_mensalidade { get; set; }
        public TaxaMatriculaUpdateModel? Taxa { get; set; }
        public List<DescontoContratoUpdateModel>? Descontos { get; set; }
        public List<Aditamento>? Aditamentos { get; set; }
        public List<VendaMaterial> VendasMaterial { get; set; }
        public List<TituloModel>? TitulosMensalidade { get; set; }
        public List<TituloModel>? TitulosMaterial { get; set; }
        public string? nm_matricula_contrato { get; set; }
        public int? cd_turma { get; set; }
        public List<TurmaModel>? Turmas { get; set; }

    }

    public class TaxaMatriculaUpdateModel
    {
        public int? cd_taxa_matricula { get; set; }
        public decimal? vl_matricula_taxa { get; set; }
        public DateTime dt_vcto_taxa { get; set; }
        public int nm_parcelas_taxa { get; set; }
        public decimal pc_responsavel_taxa { get; set; }
        public int cd_pessoa_responsavel_taxa { get; set; }
        public int cd_tipo_financeiro_taxa { get; set; }
        public int? cd_plano_conta_taxa { get; set; }
        public decimal? vl_parcela_taxa { get; set; }
    }

    public class DescontoContratoUpdateModel
    {
        public int? cd_desconto_contrato { get; set; }
        public int? cd_desconto { get; set; }
        public string? dc_desconto { get; set; }
        public bool id_desconto_ativo { get; set; }
        public decimal pc_desconto { get; set; }
        public decimal vl_desconto { get; set; }
        public bool id_incide_baixa { get; set; }
        public int nm_parcela_inicial { get; set; }
        public int nm_parcela_final { get; set; }
        public bool? id_incide_matricula { get; set; }
        public bool? id_incide_material { get; set; }
        public bool? id_aditamento { get; set; }
    }

    public class Aditamento
    {
        public int? cd_aditamento { get; set; }
        public DateTime? dt_aditamento { get; set; }
        public DateTime? dt_inicio_aditamento { get; set; }
        public DateTime? dt_vcto_aditamento { get; set; }
        public DateTime? dt_vencto_inicial { get; set; }
        public int cd_nome_contrato { get; set; }
        public byte id_tipo_aditamento { get; set; }
        public byte nm_titulos_aditamento { get; set; }
        public decimal vl_aditivo { get; set; }
        public decimal vl_saldo_aberto { get; set; }
        public decimal vl_anterior { get; set; }
        public int cd_tipo_financeiro { get; set; }
        public decimal vl_parcela_titulo_aditamento { get; set; }
        public string tx_obs_aditamento { get; set; }
        // Bolsa
        public int? cd_motivo_bolsa { get; set; }
        public string? dc_validade_bolsa { get; set; }
        public DateTime? dt_comunicado_bolsa { get; set; }
        public double? pc_bolsa { get; set; }

    }
}

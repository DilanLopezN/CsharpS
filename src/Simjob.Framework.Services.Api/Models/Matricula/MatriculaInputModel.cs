using System;
using System.Collections.Generic;

namespace Simjob.Framework.Services.Api.Models.Matricula
{
    public class MatriculaInputModel
    {
        public string? cd_usuario { get; set; }
        public string? fuso_horario { get; set; }
        public string? cd_pessoa_escola { get; set; }
        public DateTime? dt_vencimento_parcela_1 { get; set; }
        public DateTime? dt_vencimento_parcela_1_material { get; set; }
        public int nm_contrato { get; set; }
        public int nm_matricula_contrato { get; set; }
        public decimal? pc_responsavel_contrato { get; set; }
        public int? id_tipo_contrato { get; set; }
        public decimal? vl_parcela_contrato { get; set; }
        public string? nm_parcelas_contrato { get; set; }
        public decimal? vl_parcela_liquida { get; set; }
        public decimal? pc_desconto_contrato { get; set; }
        public decimal? vl_desconto_contrato { get; set; }
        public int? cd_nome_contrato { get; set; }
        public DateTime? dt_inicio_aditamento { get; set; }
        public int id_tipo_matricula { get; set; }
        public DateTime dt_inicial_contrato { get; set; }
        public int? id_retorno { get; set; }
        public int? id_nf_servico { get; set; }
        public int cd_aluno { get; set; }
        public string? cd_pessoa_responsavel { get; set; }
        public string? cd_pessoa_responsavel_taxa { get; set; }
        public DateTime? dt_final_contrato { get; set; }
        public string? cd_produto_atual { get; set; }
        public string? opcao_venda { get; set; }
        public string? cd_fila_matricula { get; set; }
        public decimal? vl_curso_contrato { get; set; }

        public decimal? vl_matricula_contrato { get; set; }
        public decimal? vl_divida_contrato { get; set; }
        public decimal? vl_desc_primeira_parcela { get; set; }
        public decimal? vl_liquido_contrato { get; set; }
        public int? id_renegociacao { get; set; }
        public int? id_transferencia { get; set; }
        public int? id_venda_pacote { get; set; }
        public decimal? pc_desconto_bolsa { get; set; }
        public decimal? vl_pre_matricula { get; set; }
        public int? cd_ano_escolar { get; set; }
        public int? id_liberar_certificado { get; set; }
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
        public decimal? pc_responsavel_material { get; set; }
        public int? id_status_contrato { get; set; }

        public string? cd_curso_atual { get; set; }
        public string? cd_duracao_atual { get; set; }
        public string? cd_plano_conta { get; set; }
        public string? cd_regime_atual { get; set; }
        public string? cd_tipo_financeiro { get; set; }
        public DateTime? dt_matricula_contrato { get; set; }
        public string? id_ajuste_manual { get; set; }
        public string? id_contrato_aula { get; set; }
        public string? id_divida_primeira_parcela { get; set; }
        public string? nm_ano_vcto { get; set; }
        public string? nm_dia_vcto { get; set; }
        public string? nm_mes_vcto { get; set; }
        public string nm_parcelas_mensalidade { get; set; }

        public TaxaMatriculaModel? Taxa { get; set; }

        public List<DescontoContratoModel>? Descontos { get; set; }
        public List<TituloModel>? TitulosTaxa { get; set; }
        public List<TituloModel>? TitulosMensalidade { get; set; }
        public List<TituloModel>? TitulosMaterial { get; set; }
        public ChequeModel? Cheque { get; set; }
        public List<TurmaModel>? Turmas { get; set; }
        //public VendaMaterial? VendaMaterial { get; set; }
        public List<CursoContrato>? CursoContrato { get; set; }
        public List<VendaMaterial>? VendasMaterial { get; set; }
    }

    public class TaxaMatriculaModel
    {
        public decimal? vl_matricula_taxa { get; set; }
        public DateTime dt_vcto_taxa { get; set; }
        public int? nm_parcelas_taxa { get; set; }
        public decimal pc_responsavel_taxa { get; set; }
        public int cd_pessoa_responsavel_taxa { get; set; }
        public int cd_tipo_financeiro_taxa { get; set; }
        public int? cd_plano_conta_taxa { get; set; }
        public decimal? vl_parcela_taxa { get; set; }
    }

    public class DescontoContratoModel
    {
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
        public int? cd_tipo_desconto { get; set; }
    }

    public class TituloModel
    {
        public int cd_pessoa_titulo { get; set; }
        public int cd_pessoa_responsavel { get; set; }
        public int cd_local_movto { get; set; }
        public DateTime dt_emissao_titulo { get; set; }
        public int cd_origem_titulo { get; set; }
        public DateTime dt_vcto_titulo { get; set; }
        public decimal vl_titulo { get; set; }
        public decimal vl_saldo_titulo { get; set; }
        public string? dc_tipo_titulo { get; set; }
        public string? dc_num_documento_titulo { get; set; }
        public string? nm_parcela_titulo { get; set; }
        public int cd_tipo_financeiro { get; set; }
        public bool id_status_cnab { get; set; }
        public decimal vl_material_titulo { get; set; }
        public decimal pc_taxa_cartao { get; set; }
        public int nm_dias_cartao { get; set; }
        public bool id_cnab_contrato { get; set; }
        public decimal vl_taxa_cartao { get; set; }
        public int? cd_aluno { get; set; }
        public decimal? pc_responsavel { get; set; }
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
        public string? opcao_venda { get; set; }
        public int? cd_curso { get; set; }
    }

    public class ChequeModel
    {
        public string no_emitente_cheque { get; set; }
        public string? no_agencia_cheque { get; set; }
        public int nm_agencia_cheque { get; set; }
        public int? nm_digito_agencia_cheque { get; set; }
        public int nm_conta_corrente_cheque { get; set; }
        public int? nm_digito_cc_cheque { get; set; }
        public int nm_primeiro_cheque { get; set; }
        public int cd_banco { get; set; }
    }

    public class TurmaModel
    {
        public int cd_turma { get; set; }
        public int nm_vagas { get; set; }
        public DateTime dt_inicio_aula { get; set; }
        public int cd_curso { get; set; }
        public bool id_turma_ppt { get; set; }
        public int cd_contrato { get; set; }
        public int cd_regime { get; set; }
    }

    public class CursoContrato
    {
        public int cd_curso { get; set; }
        public int cd_duracao { get; set; }
        public int cd_tipo_financeiro_curso { get; set; }
        public int cd_pessoa_responsavel_curso { get; set; }
        public int nm_dia_vcto_curso { get; set; }
        public string nm_mes_vcto_curso { get; set; } = string.Empty;
        public int nm_ano_vcto_curso { get; set; }
        public int nm_parcelas_curso { get; set; }
        public decimal vl_curso_total { get; set; }
        public decimal pc_desconto_contrato_curso { get; set; }
        public decimal vl_matricula_curso { get; set; }
        public decimal vl_parcela_curso { get; set; }
        public decimal vl_desconto_curso { get; set; }
        public decimal pc_responsavel_curso { get; set; }
        public decimal vl_parcela_liquida_curso { get; set; }
        public bool id_liberar_certificado { get; set; }
        public decimal vl_curso_liquido { get; set; }
        public string nm_mes_curso_inicial_curso { get; set; } = string.Empty;
        public short nm_ano_curso_inicial_curso { get; set; }
        public string nm_mes_curso_final_curso { get; set; } = string.Empty;
        public short nm_ano_curso_final_curso { get; set; }
        public bool id_valor_incluso { get; set; }
        public bool id_incorporar_valor_material { get; set; }
        public int nm_parcelas_material_curso { get; set; }

        public decimal? vl_parcelas_material_curso { get; set; }
        public decimal? vl_material_curso { get; set; }
        public decimal? vl_parcela_liq_material_curso { get; set; }
        public float? pc_bolsa_material_curso { get; set; }
        public decimal? pc_desconto_material_curso { get; set; }
        public decimal? vl_liquido_material_curso { get; set; }
        public decimal? vl_desconto_material_curso { get; set; }
        public int? opcao_venda_curso { get; set; }
        public int? cd_tipo_financeiro_material_curso { get; set; }
        public int? cd_pessoa_responsavel_material_curso { get; set; }
        public decimal? pc_responsavel_material_curso { get; set; }
        public DateTime? dt_vencimento_parcela_1_curso { get; set; }
        public int? cd_regime { get; set; }
        public decimal? pc_bolsa_curso { get; set; }
        public DateTime? dt_vencimento_parcela_1_material_curso { get; set; }
    }

    public class VendaMaterial
    {
        public bool venda { get; set; }
        public int cd_item { get; set; }
        public int cd_curso { get; set; }
    }
}
using System;
using System.Collections.Generic;

namespace Simjob.Framework.Services.Api.Models
{
    public class InsertUsuarioModel
    {
        public int cd_aluno { get; set; }
        public string no_contato { get; set; }
        public string cd_contato { get; set; }
        public int cd_pessoa_escola { get; set; }
        public int cd_midia { get; set; }
        public int cd_usuario_atendente { get; set; }
        public string cd_pessoa_aluno { get; set; }
        public bool id_aluno_ativo { get; set; }
        public string no_usuario_atendente { get; set; }
        public Motivomatricula[] motivoMatricula { get; set; }
        public List<BolsaAluno>? alunoBolsas { get; set; }
        public List<RelacionamentoAluno>? relacionamentosAluno { get; set; }
        public Pessoa pessoa { get; set; } = new Pessoa();
        public Horarios? horarios { get; set; }
        public Fichasaude fichaSaude { get; set; }
        public string key { get; set; }
        public int cd_usuario { get; set; }
        public int fuso_horario { get; set; }
        public int? cd_usuario_key { get; set; }
        public string? no_login { get; set; }
        public List<Restricao>? restricoes { get; set; }
        public RAF? Raf { get; set; }

        public class Pessoa
        {
            public int? cd_pessoa { get; set; }
            public string? dc_email { get; set; }
            public string no_pessoa { get; set; }
            public string? telefone { get; set; }
            public string celular { get; set; }
            public string nm_cnpj_cgc { get; set; }
            public string? nm_cpf { get; set; }
            public int nm_sexo { get; set; }
            public string dc_reduzido_pessoa { get; set; }
            public int nm_natureza_pessoa { get; set; }
            public string img_pessoa { get; set; }
            public DateTime dt_cadastramento { get; set; }
            public int cd_atividade_principal { get; set; }
            public bool id_pessoa_empresa { get; set; }
            public object dc_num_pessoa { get; set; }
            public bool id_exportado { get; set; }
            public object txt_obs_pessoa { get; set; }
            public string ext_img_pessoa { get; set; }
            public int cd_escola { get; set; }
            public string? cd_escolaridade { get; set; }
            public DateTime dt_nascimento { get; set; }
            public int? cd_estado_civil { get; set; }
            public int? cd_loc_nacionalidade { get; set; }
            public string nm_doc_identidade { get; set; }
            public int? cd_orgao_expedidor { get; set; }
            public int? cd_estado_expedidor { get; set; }
            public string dc_num_insc_estadual { get; set; }
            public string dc_num_insc_municipal { get; set; }
            public string cd_tipo_sociedade { get; set; }
            public string cd_endereco_principal { get; set; }
            public string cd_telefone_principal { get; set; }
            public string cd_papel_principal { get; set; }
            public string cd_produto { get; set; }
            public string cd_curso_recomendado { get; set; }
            public string pessoaRelacionada { get; set; }
            public Endereco endereco { get; set; } = new Endereco();
            public string dt_registro_junta_comercial { get; set; }
            public string dc_registro_junta_comercial { get; set; }
            public string dt_baixa { get; set; }
            public string key { get; set; }
            public int cd_usuario { get; set; }
            public int fuso_horario { get; set; }
            public DateTime? dt_emis_expedidor { get; set; }
        }

        public class Endereco
        {
            public int cd_loc_cidade { get; set; }
            public int cd_loc_estado { get; set; }
            public int cd_tipo_endereco { get; set; }
            public int cd_tipo_logradouro { get; set; }
            public int cd_loc_bairro { get; set; }
            public int cd_loc_logradouro { get; set; }
            public string dc_compl_endereco { get; set; }
            public string dc_num_cep { get; set; }
            public string dc_num_endereco { get; set; }
            public int cd_loc_pais { get; set; }
            public int cd_endereco { get; set; }
            public string key { get; set; }
        }

        public class Horarios
        {
            public Gridhorario[] gridHorario { get; set; }
        }

        public class Gridhorario
        {
            public int id_dia_semana { get; set; }
            public string dc_dia_semana { get; set; }
            public int cd_horario { get; set; }
            public bool id_disponivel { get; set; }
            public int cd_registro { get; set; }
            public int id_origem { get; set; }
            public int cd_pessoa_escola { get; set; }
            public string dt_hora_ini { get; set; }
            public string dt_hora_fim { get; set; }
        }

        public class Fichasaude
        {
            public string cd_ficha_saude { get; set; }
            public bool id_problema_saude { get; set; }
            public string dc_problema_saude { get; set; }
            public string id_tratamento_medico { get; set; }
            public string dc_tratamento_medico { get; set; }
            public string id_uso_medicamento { get; set; }
            public string dc_uso_medicamento { get; set; }
            public string id_recomendacao_medica { get; set; }
            public string dc_recomendacao_medica { get; set; }
            public string id_alergico { get; set; }
            public string dc_alergico { get; set; }
            public string id_alergico_alimento_material { get; set; }
            public string dc_alergico_alimento_material { get; set; }
            public string id_epiletico { get; set; }
            public string id_epiletico_tratamento { get; set; }
            public string id_asmatico { get; set; }
            public string id_asmatico_tratamento { get; set; }
            public string id_diabetico { get; set; }
            public string id_depende_insulina { get; set; }
            public string id_medicacao_especifica { get; set; }
            public string dc_medicacao_especifica { get; set; }
            public string dt_hora_medicacao_especifica { get; set; }
            public string tx_informacoes_adicionais { get; set; }
            public string id_plano_saude { get; set; }
            public string dc_plano_saude { get; set; }
            public string dc_nm_carteirinha_plano { get; set; }
            public string dc_categoria_plano { get; set; }
            public string dc_nome_clinica_hospital { get; set; }
            public string dc_endereco_hospital_clinica { get; set; }
            public string dc_telefone_hospital_clinica { get; set; }
            public string id_aviso_emergencia { get; set; }
            public string dc_nome_pessoa_aviso_emergencia { get; set; }
            public string dc_parentesco_aviso_emergencia { get; set; }
            public string dc_telefone_residencial_aviso_emergencia { get; set; }
            public string dc_telefone_comercial_aviso_emergencia { get; set; }
            public string dc_telefone_celular_aviso_emergencia { get; set; }
            public string dc_telefone_fixo_hospital_clinica { get; set; }
        }

        public class Motivomatricula
        {
            public int cd_motivo_matricula { get; set; }
            public string dc_motivo_matricula { get; set; }
            public bool id_motivo_matricula_ativo { get; set; }
        }

        public class BolsaAluno
        {
            public int? cd_produto { get; set; }
            public int? pc_bolsa { get; set; }
            public DateTime dt_inicio_bolsa { get; set; }
            public string dc_validade_bolsa { get; set; }
            public DateTime? dt_comunicado_bolsa { get; set; }
            public DateTime? dt_cancelamento_bolsa { get; set; }
            public int cd_motivo_bolsa { get; set; }
            public int id_bolsa_material { get; set; }
            public int? pc_bolsa_material { get; set; }
        }

        public class RelacionamentoAluno
        {
            public int? cd_papel_pai { get; set; }
            public int cd_pessoa_filho { get; set; }
            public int cd_papel_filho { get; set; }
        }

        public class Restricao
        {
            public DateTime dt_inicio_restricao { get; set; }
            public DateTime? dt_fim_restricao { get; set; }
            public int cd_orgao_financeiro { get; set; }
        }

        public class RAF
        {
            public string nm_raf { get; set; } = string.Empty;
            public bool id_raf_liberado { get; set; }
            public int nm_tentativa { get; set; }
            public bool id_bloqueado { get; set; }
            public bool id_trocar_senha { get; set; }
            public DateTime dt_expiracao_senha { get; set; }
            public string? dc_senha_raf { get; set; }
            public DateTime? dt_limite_bloqueio { get; set; }
        }
    }
}
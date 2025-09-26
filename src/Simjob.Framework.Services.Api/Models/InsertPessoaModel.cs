using System;
using System.Collections.Generic;

namespace Simjob.Framework.Services.Api.Models
{
    public class InsertPessoaModel
    {
        public int cd_aluno { get; set; }
        public string no_contato { get; set; }
        public string cd_contato { get; set; }
        public int cd_pessoa_escola { get; set; }
        public int cd_midia { get; set; }
        public string cd_escolaridade { get; set; }
        public int cd_usuario_atendente { get; set; }
        public string cd_pessoa_aluno { get; set; }
        public bool id_aluno_ativo { get; set; }
        public string no_usuario_atendente { get; set; }
        public string nm_raf { get; set; }
        public string dt_expiracao_senha { get; set; }
        public string nm_tentativa { get; set; }
        public bool id_raf_liberado { get; set; }
        public bool id_bloqueado { get; set; }
        public bool id_trocar_senha { get; set; }
        public string dt_limite_bloqueio { get; set; }
        public PessoaInfos pessoa { get; set; } = new PessoaInfos();
        public FichasaudeInfos fichaSaude { get; set; } = new FichasaudeInfos();
        public string key { get; set; }
        public int cd_usuario { get; set; }
        public int fuso_horario { get; set; }
        public int? cd_usuario_key { get; set; }
        public string? no_login { get; set; }
        public List<PessoaRelacionamentoPessoa>? Dependentes { get; set; }

        public class PessoaInfos
        {
            public int? cd_pessoa { get; set; }
            public string? dc_email { get; set; }
            public string no_pessoa { get; set; }
            public string? telefone { get; set; }
            public string? celular { get; set; }
            public string nm_cnpj_cgc { get; set; }
            public string? nm_cpf { get; set; }
            public int? nm_sexo { get; set; }
            public string dc_reduzido_pessoa { get; set; }
            public int nm_natureza_pessoa { get; set; }
            public string img_pessoa { get; set; }
            public DateTime dt_cadastramento { get; set; }
            public int cd_atividade_principal { get; set; }
            public bool id_pessoa_empresa { get; set; }
            public string dc_num_pessoa { get; set; }
            public bool id_exportado { get; set; }
            public string txt_obs_pessoa { get; set; }
            public string ext_img_pessoa { get; set; }
            public int cd_escola { get; set; }
            public int cd_escolaridade { get; set; }
            public DateTime? dt_nascimento { get; set; }
            public DateTime? dt_emis_expedidor { get; set; }
            public int cd_estado_civil { get; set; }
            public int cd_loc_nacionalidade { get; set; }
            public string nm_doc_identidade { get; set; }
            public int cd_orgao_expedidor { get; set; }
            public int cd_estado_expedidor { get; set; }
            public string dc_num_insc_estadual { get; set; }
            public string dc_num_insc_municipal { get; set; }
            public string cd_tipo_sociedade { get; set; }
            public string cd_endereco_principal { get; set; }
            public string cd_telefone_principal { get; set; }
            public string cd_papel_principal { get; set; }
            public string cd_produto { get; set; }
            public string cd_curso_recomendado { get; set; }
            public string pessoaRelacionada { get; set; }
            public EnderecoInfos? endereco { get; set; }
            public string dt_registro_junta_comercial { get; set; }
            public string dc_registro_junta_comercial { get; set; }
            public string dt_baixa { get; set; }
            public string key { get; set; }
            public int cd_usuario { get; set; }
            public int fuso_horario { get; set; }

        }

        public class EnderecoInfos
        {
            public int? cd_loc_cidade { get; set; }
            public int? cd_loc_estado { get; set; }
            public int? cd_tipo_endereco { get; set; }
            public int cd_tipo_logradouro { get; set; }
            public int? cd_loc_bairro { get; set; }
            public int? cd_loc_logradouro { get; set; }
            public object dc_compl_endereco { get; set; }
            public string dc_num_cep { get; set; }
            public string dc_num_endereco { get; set; }
            public int cd_loc_pais { get; set; }
            public int cd_endereco { get; set; }
            public string key { get; set; }
        }

        public class FichasaudeInfos
        {
            public object cd_ficha_saude { get; set; }
            public bool id_problema_saude { get; set; }
            public object dc_problema_saude { get; set; }
            public object id_tratamento_medico { get; set; }
            public object dc_tratamento_medico { get; set; }
            public object id_uso_medicamento { get; set; }
            public object dc_uso_medicamento { get; set; }
            public object id_recomendacao_medica { get; set; }
            public object dc_recomendacao_medica { get; set; }
            public object id_alergico { get; set; }
            public object dc_alergico { get; set; }
            public object id_alergico_alimento_material { get; set; }
            public object dc_alergico_alimento_material { get; set; }
            public object id_epiletico { get; set; }
            public object id_epiletico_tratamento { get; set; }
            public object id_asmatico { get; set; }
            public object id_asmatico_tratamento { get; set; }
            public object id_diabetico { get; set; }
            public object id_depende_insulina { get; set; }
            public object id_medicacao_especifica { get; set; }
            public object dc_medicacao_especifica { get; set; }
            public object dt_hora_medicacao_especifica { get; set; }
            public object tx_informacoes_adicionais { get; set; }
            public object id_plano_saude { get; set; }
            public object dc_plano_saude { get; set; }
            public object dc_nm_carteirinha_plano { get; set; }
            public object dc_categoria_plano { get; set; }
            public object dc_nome_clinica_hospital { get; set; }
            public object dc_endereco_hospital_clinica { get; set; }
            public object dc_telefone_hospital_clinica { get; set; }
            public object id_aviso_emergencia { get; set; }
            public object dc_nome_pessoa_aviso_emergencia { get; set; }
            public object dc_parentesco_aviso_emergencia { get; set; }
            public object dc_telefone_residencial_aviso_emergencia { get; set; }
            public object dc_telefone_comercial_aviso_emergencia { get; set; }
            public object dc_telefone_celular_aviso_emergencia { get; set; }
            public object dc_telefone_fixo_hospital_clinica { get; set; }


        }

        public class PessoaRelacionamentoPessoa
        {
            public int cd_pessoa_filho { get; set; }
            public int cd_papel_filho { get; set; }
            public int? cd_qualif_relacionamento { get; set; }
            public int? cd_papel_pai { get; set; }
        }

    }
}

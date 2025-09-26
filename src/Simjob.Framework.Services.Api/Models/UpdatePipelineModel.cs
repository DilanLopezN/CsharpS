using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Simjob.Framework.Services.Api.Models
{
    public class UpdatePipelineModel
    {
        public int cd_pipeline { get; set; }
        public int? nm_resultado_teste { get; set; }
        public int? cd_etapa_pipeline { get; set; }
        public int? cd_motivo_perda { get; set; }
        public int? cd_curso_pipeline { get; set; }
        public int? cd_produto_pipeline { get; set; }

        [Required]
        public DateTime DataRealizada { get; set; } = DateTime.Now;

        public DateTime? Reprogramar { get; set; }
        public int? Aproveitamento { get; set; }
        public List<int>? id_dia_semana { get; set; }
        public List<int>? id_periodo { get; set; }
        public int? Temperatura { get; set; }
        public PessoaPipeLine? Pessoa { get; set; }
        public string? txt_observacao_pipeline { get; set; }
    }

    public class PessoaPipeLine
    {
        public string? dc_email { get; set; }
        public string? no_pessoa { get; set; }
        public string? telefone { get; set; }
        public string? celular { get; set; }
        public string? nm_cpf { get; set; }
        public int? nm_sexo { get; set; }
        public int? cd_escolaridade { get; set; }
        public EnderecoPessoaPipeLine? EnderecoPessoaPipeLine { get; set; }

        public int? cd_acao { get; set; }
        public int? id_posicao_contato { get; set; }
    }

    public class EnderecoPessoaPipeLine
    {
        public int? cd_endereco { get; set; }
        public int? cd_loc_cidade { get; set; }
        public int? cd_loc_estado { get; set; }
        public int? cd_loc_pais { get; set; }
        public int? cd_tipo_endereco { get; set; }
        public int? cd_tipo_logradouro { get; set; }
        public int? cd_loc_bairro { get; set; }
        public int? cd_loc_logradouro { get; set; }
        public string? dc_compl_endereco { get; set; }
        public string? dc_num_cep { get; set; }
        public string? dc_num_endereco { get; set; }
    }
}
using System;

namespace Simjob.Framework.Services.Api.Models.Contato
{
    public class ContatoInsertModel
    {
        public int? cd_contato { get; set; }
        public int? cd_pessoa_contato { get; set; }
        public int? cd_pessoa_escola { get; set; }
        public int? cd_acao { get; set; }
        public int? id_posicao_contato { get; set; }
        public string? email { get; set; }
        public string? no_pessoa { get; set; }
        public string? telefone { get; set; }
        public string? celular { get; set; }
        public string? nm_cpf { get; set; }
        public int? nm_sexo { get; set; }
        public int? cd_escolaridade { get; set; }
        public DateTime? dt_nascimento { get; set; }
        public string? no_acao { get; set; }
        public DateTime? dt_cadastramento { get; set; }
        public int? dc_posicao_contato { get; set; }
        public int? cd_email { get; set; }
        public int? cd_celular { get; set; }
        public int? cd_telefone_principal { get; set; }
        public string? no_escolaridade { get; set; }
        public int? id_pipeline { get; set; }
        public int? cd_endereco_principal { get; set; }
        public EnderecoContato? endereco { get; set; }
        public int? id_status_contato { get; set; }
        public string? key { get; set; }
        public int? cd_usuario { get; set; }
        public int? fuso_horario { get; set; }

        public class EnderecoContato
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
            public string? key { get; set; }
        }

    }
}

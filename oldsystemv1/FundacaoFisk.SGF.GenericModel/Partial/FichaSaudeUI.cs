using Componentes.GenericModel;
using System.ComponentModel.DataAnnotations;
using System;

namespace FundacaoFisk.SGF.GenericModel.Partial
{
    public class FichaSaudeUI
    {
        
        public int cd_ficha_saude { get; set; }

        public int cd_aluno { get; set; }

        public Nullable<bool> id_problema_saude { get; set; }

        public string dc_problema_saude { get; set; }

        public Nullable<bool> id_tratamento_medico { get; set; }

        public string dc_tratamento_medico { get; set; }

        public Nullable<bool> id_uso_medicamento { get; set; }

        public string dc_uso_medicamento { get; set; }

        public Nullable<bool> id_recomendacao_medica { get; set; }

        public string dc_recomendacao_medica { get; set; }

        public Nullable<bool> id_alergico { get; set; }

        public string dc_alergico { get; set; }

        public Nullable<bool> id_alergico_alimento_material { get; set; }

        public string dc_alergico_alimento_material { get; set; }

        public Nullable<bool> id_epiletico { get; set; }

        public Nullable<bool> id_epiletico_tratamento { get; set; }

        public Nullable<bool> id_asmatico { get; set; }

        public Nullable<bool> id_asmatico_tratamento { get; set; }

        public Nullable<bool> id_diabetico { get; set; }

        public Nullable<bool> id_depende_insulina { get; set; }

        public Nullable<bool> id_medicacao_especifica { get; set; }

        public string dc_medicacao_especifica { get; set; }

        public Nullable<System.TimeSpan> dt_hora_medicacao_especifica { get; set; }

        public string tx_informacoes_adicionais { get; set; }

        public Nullable<bool> id_plano_saude { get; set; }

        public string dc_plano_saude { get; set; }

        public string dc_nm_carteirinha_plano { get; set; }

        public string dc_categoria_plano { get; set; }

        public string dc_nome_clinica_hospital { get; set; }

        public string dc_endereco_hospital_clinica { get; set; }

        public string dc_telefone_hospital_clinica { get; set; }
        public string dc_telefone_fixo_hospital_clinica { get; set; }

        public Nullable<byte> id_aviso_emergencia { get; set; }

        public string dc_nome_pessoa_aviso_emergencia { get; set; }

        public string dc_parentesco_aviso_emergencia { get; set; }

        public string dc_telefone_residencial_aviso_emergencia { get; set; }

        public string dc_telefone_comercial_aviso_emergencia { get; set; }

        public string dc_telefone_celular_aviso_emergencia { get; set; }
    }
}
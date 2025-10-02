using System;
using System.Collections.Generic;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Model
{
    public class MovimentoProcessarUI : TO
    {
        public int cd_movimento { get; set; }

        public int cd_pessoa_empresa { get; set; }

        public int cd_pessoa { get; set; }

        public int cd_politica_comercial { get; set; }

        public int cd_tipo_financeiro { get; set; }

        public byte id_tipo_movimento { get; set; }

        public Nullable<int> nm_movimento { get; set; }

        public string dc_serie_movimento { get; set; }

        public System.DateTime dt_emissao_movimento { get; set; }

        public System.DateTime dt_vcto_movimento { get; set; }

        public System.DateTime dt_mov_movimento { get; set; }

        public decimal pc_acrescimo { get; set; }

        public decimal vl_acrescimo { get; set; }

        public decimal pc_desconto { get; set; }

        public decimal vl_desconto { get; set; }

        public string tx_obs_movimento { get; set; }

        public bool id_nf { get; set; }

        public bool id_nf_escola { get; set; }

        public Nullable<int> cd_tipo_nota_fiscal { get; set; }

        public Nullable<byte> id_status_nf { get; set; }

        public decimal vl_base_calculo_ICMS_nf { get; set; }

        public decimal vl_base_calculo_PIS_nf { get; set; }

        public decimal vl_base_calculo_COFINS_nf { get; set; }

        public decimal vl_base_calculo_IPI_nf { get; set; }

        public decimal vl_base_calculo_ISS_nf { get; set; }

        public decimal vl_ICMS_nf { get; set; }

        public decimal vl_PIS_nf { get; set; }

        public decimal vl_COFINS_nf { get; set; }

        public decimal vl_IPI_nf { get; set; }

        public decimal vl_ISS_nf { get; set; }

        public string tx_obs_fiscal { get; set; }

        public string dc_justificativa_nf { get; set; }

        public string dc_cfop_nf { get; set; }

        public Nullable<int> id_origem_movimento { get; set; }

        public Nullable<int> cd_origem_movimento { get; set; }

        public Nullable<int> cd_nota_fiscal { get; set; }

        public Nullable<int> cd_cfop_nf { get; set; }

        public double pc_aliquota_aproximada { get; set; }

        public decimal vl_aproximado { get; set; }

        public string nm_nfe { get; set; }

        public string ds_protocolo_nfe { get; set; }

        public Nullable<System.DateTime> dt_autorizacao_nfe { get; set; }

        public Nullable<System.DateTime> dt_nfe_cancel { get; set; }

        public string dc_key_nfe { get; set; }

        public string dc_url_nf { get; set; }

        public string dc_mensagem_retorno { get; set; }

        public string dc_protocolo_cancel { get; set; }

        public Nullable<int> cd_aluno { get; set; }

        public bool id_exportado { get; set; }

        public string dc_meio_pagamento { get; set; }

        public bool id_importacao_xml { get; set; }


        public virtual Escola Empresa { get; set; }
        public virtual TipoFinanceiro TipoFinanceiro { get; set; }
        public virtual PessoaSGF Pessoa { get; set; }
        public virtual PoliticaComercial PoliticaComercial { get; set; }
        public virtual ICollection<ItemMovimento> ItensMovimento { get; set; }
        public virtual TipoNotaFiscal TipoNF { get; set; }
        public virtual CFOP CFOP { get; set; }
        public virtual ICollection<Movimento> MovimentoDevolver { get; set; }
        public virtual Movimento MovimentoDevolucao { get; set; }
        public virtual ICollection<Cheque> Cheques { get; set; }
        public virtual Aluno Aluno { get; set; }
        public virtual ICollection<ItemMovimentoKit> ItemMovimentoKit { get; set; }
    }
}
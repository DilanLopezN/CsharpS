using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Model
{
    public class ReciboPagamentoUI : TO
    {
        public string dc_telefone_escola { get; set; }
        public string dc_num_cgc { get; set; }
        public string dc_num_insc_estadual { get; set; }
        public string no_pessoa { get; set; }
        public string no_pessoa_responsavel { get; set; }
        public string dc_cidade_estado { get; set; }
        public DateTime dt_baixa_titulo { get; set; }
        public int nm_recibo { get; set; }
        public string txt_obs_baixa { get; set; }
        public string cpf_pessoa { get; set; }
        public string dc_tipo_liquidacao { get; set; }
        public BaixaTitulo baixaTitulo { get; set; }
        public int  total_titulo { get; set; }
        public string no_pessoa_usuario { get; set; }

        public string titulo_recibo_pagamento
        {
            get
            {
                if (baixaTitulo != null)
                {
                    if (baixaTitulo.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.CANCELAMENTO)
                        return "RECIBO DE CANCELAMENTO";
                    return "RECIBO DE PAGAMENTO";
                }
                return "";
            }
        }

        public string dc_rua_escola
        {
            get
            {
                string retorno = "";

                if (endereco.Logradouro != null)
                {
                    if (endereco.TipoLogradouro != null && !String.IsNullOrEmpty(endereco.TipoLogradouro.sg_tipo_logradouro))
                        retorno += endereco.TipoLogradouro.sg_tipo_logradouro;
                    retorno += " " + endereco.Logradouro.no_localidade;
                    if (!String.IsNullOrEmpty(endereco.dc_num_endereco))
                        retorno += ", nº " + endereco.dc_num_endereco;
                }
                return retorno;
            }
        }       

        public string dc_bairro_escola
        {
            get
            {
                string retorno = "";

                if (endereco.Logradouro != null)
                {
                    if (!String.IsNullOrEmpty(endereco.Bairro.no_localidade))
                        retorno += " " + endereco.Bairro.no_localidade;
                    retorno += " - " + endereco.Cidade.no_localidade;
                    if (!String.IsNullOrEmpty(endereco.Logradouro.dc_num_cep))
                        retorno += " | " + endereco.Logradouro.dc_num_cep;
                    retorno += " " + endereco.Estado.Estado.sg_estado;
                }
                return retorno;
            }
        }

        public string dc_tipo_titulo_referente
        {
            get
            {
                if (titulo != null)
                {
                    if (titulo.dc_tipo_titulo == Titulo.convertTipoTitulo((byte)Titulo.TipoTitulo.TM) || titulo.dc_tipo_titulo == Titulo.convertTipoTitulo((byte)Titulo.TipoTitulo.TA))
                        return "Taxa de Matrícula";
                    return "Mensalidade";
                }
                return "";
            }
        }

        public int? nm_titulo 
        {
            get 
            {
                if (titulo != null)
                    return titulo.nm_titulo;
                return null;
            }
        }

        public string nm_parcela_titulo 
        {
            get
            {
                if (titulo != null && titulo.nm_parcela_titulo != null)
                    return string.Format("{0}/{1}", titulo.nm_parcela_titulo, this.total_titulo);
                return "";
            }
        }

        public string dt_vcto_titulo 
        {
            get {
                if (titulo != null)
                    return String.Format("{0:dd/MM/yyyy}", titulo.dt_vcto_titulo);
                return "";
            }
        }

        public string id_natureza_titulo 
        { 
            get 
            {
                if (titulo != null)
                    return titulo.id_natureza_titulo.ToString();
                return "";
            } 
        }

         public string vl_titulo
         { 
            get 
            {
                if (titulo != null)
                    return string.Format("R${0:0,0.00}", titulo.vl_titulo);
                return "";
            }
        }
        
        public string vl_pagar
        {
            get 
            {
                if (titulo != null)
                    return string.Format("R${0:0,0.00}", (baixaTitulo.vl_principal_baixa + ((baixaTitulo.vl_multa_baixa - baixaTitulo.vl_desc_multa_baixa) + baixaTitulo.vl_juros_baixa)) - baixaTitulo.vl_desconto_baixa);
                return "";
            }
        }

        public string vl_baixa_saldo_titulo
        {
            get
            {
                if(baixaTitulo != null)
                    return string.Format("R${0:0,0.00}", baixaTitulo.vl_baixa_saldo_titulo);
                return "";
            }
        }

        public string vl_desconto_baixa
        {
            get
            {
                if(baixaTitulo != null)
                    return string.Format("R${0:0,0.00}", baixaTitulo.vl_desconto_baixa);
                return "";
            }
        }

       public string vl_multa_baixa
        {
            get
            {
                if(baixaTitulo != null)
                    return string.Format("R${0:0,0.00}", (baixaTitulo.vl_multa_baixa - baixaTitulo.vl_desc_multa_baixa) + baixaTitulo.vl_juros_baixa);
                return "";
            }
        }

        public string vl_liquidacao_baixa
        {
            get
            {
                if (baixaTitulo != null && baixaTitulo.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.CANCELAMENTO)
                    return string.Format("R${0:0,0.00}", baixaTitulo.vl_liquidacao_baixa);
                return string.Format("R${0:0,0.00}", 0);
            }
        }

        public string vl_principal_baixa
        {
            get
            {
                if (baixaTitulo != null)
                    return string.Format("R${0:0,0.00}", baixaTitulo.vl_principal_baixa);
                return "";
            }
        }

        public EnderecoSGF endereco { get; set; }
        public Titulo titulo { get; set; }
    }
}

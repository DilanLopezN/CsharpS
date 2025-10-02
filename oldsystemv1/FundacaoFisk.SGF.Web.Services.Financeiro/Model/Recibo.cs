using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Model
{
    public class Recibo : TO
    {
        public string dc_telefone_escola  {get;set;}
        public string dc_num_cgc {get;set;}
        public string dc_num_insc_estadual {get;set;}
        public decimal vl_liquidacao_baixa {get;set;}
        public string dc_extenso {get;set;} 
        public string no_pessoa {get;set;}
        public string dc_cidade_estado {get;set;}
        public DateTime dt_baixa_titulo {get;set;}
        public int nm_recibo { get; set; }
        public string txt_obs_baixa { get; set; }
        public EnderecoSGF endereco { get; set; }
        public Titulo titulo { get; set; }
        public string dc_endereco_escola {
            get{
                 string retorno = "";

                if(endereco.Logradouro != null) {
                    if(endereco.TipoLogradouro != null && !String.IsNullOrEmpty(endereco.TipoLogradouro.sg_tipo_logradouro))
                        retorno += endereco.TipoLogradouro.sg_tipo_logradouro ;
                    retorno += " " + endereco.Logradouro.no_localidade;
                    if(!String.IsNullOrEmpty(endereco.dc_num_endereco))
                        retorno += " Nº " + endereco.dc_num_endereco;
                    if(!String.IsNullOrEmpty(endereco.Bairro.no_localidade))
                        retorno += " " + endereco.Bairro.no_localidade;
                    retorno += " " + endereco.Cidade.no_localidade;
                    if(!String.IsNullOrEmpty(endereco.Logradouro.dc_num_cep))
                        retorno += " " + endereco.Logradouro.dc_num_cep;
                     retorno += " " + endereco.Estado.Estado.sg_estado;
                }
                return retorno;
            }
        }
        public string tx_obs_baixa
        {
            get
            {
                string retorno = "";
                if (!string.IsNullOrEmpty(txt_obs_baixa))
                    retorno = txt_obs_baixa;
                if (titulo != null)
                    retorno += "\n Título Nº:" + titulo.nm_titulo + "-" + titulo.nm_parcela_titulo + " Vcto.:" +
                    String.Format("{0:dd/MM/yyyy}", titulo.dt_vcto_titulo);
                return retorno;
            }
        }

        public byte natureza_titulo
        {
            get
            {
                byte retorno = 0;
                if (titulo != null && titulo.id_natureza_titulo != null && titulo.id_natureza_titulo > 0)
                    retorno = (byte)titulo.id_natureza_titulo;
                return retorno;
            }
        }
    }
}

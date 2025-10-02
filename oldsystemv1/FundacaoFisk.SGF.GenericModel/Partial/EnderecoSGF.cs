using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace FundacaoFisk.SGF.GenericModel
{
    using Componentes.GenericModel;
    public partial class EnderecoSGF : TO
    {
        public string noLocBairro { get; set; }
        public string noLocCidade { get; set; }
        public string noLocRua { get; set; }
        public string descTipoLogradouro { get; set; }
        public string noLocEstado { get; set; }
        public string num_cep { get; set; }
        public string enderecoCompleto
        {
            get
            {
                string retorno = "";

                if(this.Logradouro != null)
                {
                    if (this.TipoLogradouro != null && !String.IsNullOrEmpty(this.TipoLogradouro.no_tipo_logradouro))
                        retorno += this.TipoLogradouro.no_tipo_logradouro + " ";
                    retorno += this.Logradouro.no_localidade;
                    if(!String.IsNullOrEmpty(this.dc_num_endereco))
                        retorno += " Nº " + this.dc_num_endereco;
                    if(!String.IsNullOrEmpty(dc_compl_endereco))
                        retorno += " / " + this.dc_compl_endereco;
                    if(!String.IsNullOrEmpty(num_cep))
                        retorno += ", CEP: " + this.num_cep;
                    if(Bairro != null)
                        if(!String.IsNullOrEmpty(Bairro.no_localidade))
                            retorno += ", Bairro: "+ Bairro.no_localidade;
                    if ((Cidade != null && !string.IsNullOrEmpty(Cidade.no_localidade)) && 
                        (Estado != null && !string.IsNullOrEmpty(Estado.no_localidade)))
                        retorno += string.Format(", Cidade: {0} - {1}", Cidade.no_localidade, Estado.no_localidade);
                }
                return retorno;
            }
        }

        public string enderecoCompletoBoleto
        {
            get
            {
                string retorno = "";

                if(this.Logradouro != null)
                {
                    if(this.TipoLogradouro != null && !String.IsNullOrEmpty(this.TipoLogradouro.no_tipo_logradouro))
                        retorno += this.TipoLogradouro.no_tipo_logradouro + " ";
                    retorno += this.Logradouro.no_localidade;
                    if(!String.IsNullOrEmpty(this.dc_num_endereco))
                        retorno += " Nº " + this.dc_num_endereco;
                    if(!String.IsNullOrEmpty(dc_compl_endereco))
                        retorno += " " + this.dc_compl_endereco;
                }
                return retorno;
            }
        }

        public string enderecoCompletoEspelho
        {
            get
            {
                string retorno = "";

                if(this.Logradouro != null) {
                    if(this.TipoLogradouro != null && !String.IsNullOrEmpty(this.TipoLogradouro.no_tipo_logradouro))
                        retorno += this.TipoLogradouro.no_tipo_logradouro + " ";
                    retorno += this.Logradouro.no_localidade;
                    if(!String.IsNullOrEmpty(this.dc_num_endereco))
                        retorno += ", " + this.dc_num_endereco;
                    if(!String.IsNullOrEmpty(dc_compl_endereco))
                        retorno += ", " + this.dc_compl_endereco;
                }
                return retorno;
            }
        }

        public string enderecoBoletoSemNumero
        {
            get
            {
                string retorno = "";

                if (this.Logradouro != null)
                {
                    if (this.TipoLogradouro != null && !String.IsNullOrEmpty(this.TipoLogradouro.no_tipo_logradouro))
                        retorno += this.TipoLogradouro.no_tipo_logradouro + " ";
                    retorno += this.Logradouro.no_localidade;
                    if (!String.IsNullOrEmpty(dc_compl_endereco))
                        retorno += " " + this.dc_compl_endereco;
                }
                return retorno;
            }
        }

        public string enderecoCompletoSimplificadoBoleto
        {
            get
            {
                string retorno = "";

                if (this.Logradouro != null)
                {
                    if (this.TipoLogradouro != null && !String.IsNullOrEmpty(this.TipoLogradouro.no_tipo_logradouro))
                        retorno += this.TipoLogradouro.no_tipo_logradouro + " ";
                    retorno += this.Logradouro.no_localidade;
                    if (!String.IsNullOrEmpty(this.dc_num_endereco))
                        retorno += " "+ this.dc_num_endereco;
                    if (!String.IsNullOrEmpty(dc_compl_endereco))
                        retorno += " / " + this.dc_compl_endereco;
                    if (Bairro != null)
                        if (!String.IsNullOrEmpty(Bairro.no_localidade))
                            retorno += ", " + Bairro.no_localidade;
                    if ((Cidade != null && !string.IsNullOrEmpty(Cidade.no_localidade)) &&
                        (Estado != null && !string.IsNullOrEmpty(Estado.Estado.sg_estado)))
                        retorno += string.Format(", {0} - {1}", Cidade.no_localidade, Estado.Estado.sg_estado);
                    if (!String.IsNullOrEmpty(num_cep) || !String.IsNullOrEmpty(this.Logradouro.dc_num_cep))
                        retorno += "- CEP: " + (!String.IsNullOrEmpty(num_cep) ? this.num_cep : this.Logradouro.dc_num_cep);
                }
                return retorno;
            }
        }

        public List<LocalidadeSGF> bairros { get; set; }
    }
}

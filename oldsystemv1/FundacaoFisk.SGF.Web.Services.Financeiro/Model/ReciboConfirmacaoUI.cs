using System;
using System.Collections.Generic;
using System.Linq;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Model
{
    public class ReciboConfirmacaoUI : TO
    {
        public string dc_telefone_escola { get; set; }
        public string dc_num_cgc { get; set; }
        public string dc_num_insc_estadual { get; set; }
        public string no_pessoa { get; set; }
        public string no_pessoa_responsavel { get; set; }
        public string dc_cidade_estado { get; set; }
        public int cd_contrato_movimento { get; set; }
        public string cpf_pessoa { get; set; }
        public string no_pessoa_usuario { get; set; }

        public string titulo_recibo_confirmacao
        {
            get
            {
                return "RECIBO DE CONFIRMAÇÃO";
            }
        }

        public string dc_tipo_titulo_referente { get; set; }
        
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

        public string tipo_financeiro
        {
            get
            {
                if (qtd_cartao > 0 && qtd_cheque == 0)
                {
                    return "Cartão";
                }
                else if (qtd_cartao == 0 && qtd_cheque > 0)
                {
                    return "Cheque";
                }
                else if (qtd_cartao > 0 && qtd_cheque > 0)
                {
                    return "Diversos";
                }

                return "";
            }
        }

        public EnderecoSGF endereco { get; set; }
        public List<Titulo> titulos { get; set; }

        //public List<String> parcelas
        //{
        //    get
        //    {
        //        if (titulos != null && titulos.Count > 0)
        //        {
        //            List<String> parc = new List<string>();
        //            foreach (var titulo in titulos)
        //            {
        //                var parcela = string.Format("{0} - Vcto.:{1:dd/MM/yyyy} - R${2:0,0.00}  {3}" , titulo.nm_parcela_titulo, titulo.dt_vcto_titulo, titulo.vl_titulo, (tipo_financeiro == "Cartão" || tipo_financeiro == "Cheque")? ("- Tipo Financeiro: " + tipo_financeiro): "");
        //                parc.Add(parcela);
        //            }

        //            return parc;
        //        }

        //        return null;
        //    }
        //}

        public int qtd_cheque
        {
            get
            {
                if (titulos != null && titulos.Count > 0)
                {
                    return titulos.Where(z => z.cd_tipo_financeiro == (int) TipoFinanceiro.TiposFinanceiro.CHEQUE).Count();
                    
                }

                return 0;
            }
        }

        public int qtd_cartao
        {
            get
            {
                
                if (titulos != null && titulos.Count > 0)
                {
                    return titulos.Where(z => z.cd_tipo_financeiro == (int)TipoFinanceiro.TiposFinanceiro.CARTAO).Count();
                    
                }

                return 0;
            }
        }

        public string total
        {
            get
            {
                if (titulos != null && titulos.Count > 0)
                    return string.Format("R${0:0,0.00}", titulos.Sum(b=> b.vl_titulo));
                return string.Format("R${0:0,0.00}", 0.0);
            }
        }

    }
}
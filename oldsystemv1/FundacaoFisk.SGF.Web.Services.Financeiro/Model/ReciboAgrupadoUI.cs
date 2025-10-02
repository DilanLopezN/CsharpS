using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Model
{
    public class ReciboAgrupadoUI : TO
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

        public List<BaixaTitulo> baixas { get; set; }

        public string titulo_recibo_pagamento
        {
            get
            {
                return "RECIBO DE PAGAMENTO";
            }
        }

        public string dc_tipo_titulo_referente
        {
            get
            {
                if (titulos != null && titulos.Count > 0)
                {
                    

                    foreach (var titulo in titulos)
                    {
                        if (titulo.dc_tipo_titulo == Titulo.convertTipoTitulo((byte)Titulo.TipoTitulo.NF))
                            return "Notas";
                    }

                    return "Mensalidade";
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

        

        public EnderecoSGF endereco { get; set; }
        public List<Titulo> titulos { get; set; }

        public List<ParcelasReciboAgrupadoUI> parcelas
        {
            get
            {
                if (titulos != null && titulos.Count > 0)
                {
                    List<ParcelasReciboAgrupadoUI> parc = new List<ParcelasReciboAgrupadoUI>();
                    foreach (var titulo in titulos)
                    {
                        List<BaixaTitulo> baixasTitulo = baixas.Where(x => x.cd_titulo == titulo.cd_titulo).ToList();
                        StringBuilder parcela = new StringBuilder()
                            .Append(string.Format("{0}-{1} - Vcto.:{2:dd/MM/yyyy} - R${3:0,0.00}  - Receb.:{4:dd/MM/yyyy} - R${5:0,0.00} ", titulo.nm_titulo, titulo.nm_parcela_titulo, titulo.dt_vcto_titulo, titulo.vlTitulo, titulo.dt_liquidacao_titulo, baixasTitulo.Any() ? baixasTitulo.Sum(x => x.vl_liquidacao_baixa) : 0));
                        //caso tenha descontos ou/e multas
                        if ((baixasTitulo.Any() && baixasTitulo.Sum(x => x.vl_desconto_baixa) > 0) || 
                            (baixasTitulo.Any() && (baixasTitulo.Sum(x => x.vl_juros_baixa) + baixasTitulo.Sum(x => x.vl_multa_baixa)) > 0 ))
                        {
                            parcela.Append("(");

                            //adiciona o desconto
                            if ((baixasTitulo.Any() && baixasTitulo.Sum(x => x.vl_desconto_baixa) > 0))
                            {
                                parcela.Append(string.Format("Desc: R${0:0,0.00}", baixasTitulo.Any() ? baixasTitulo.Sum(x => x.vl_desconto_baixa) : 0));

                            }

                            //se tiver desconto e multa adiciona a virgula e espaço
                            if ((baixasTitulo.Any() && baixasTitulo.Sum(x => x.vl_desconto_baixa) > 0) &&
                                (baixasTitulo.Any() && (baixasTitulo.Sum(x => x.vl_juros_baixa) + baixasTitulo.Sum(x => x.vl_multa_baixa)) > 0))
                            {
                                parcela.Append(", ");
                            }

                            //adicona a multa
                            if ((baixasTitulo.Any() && (baixasTitulo.Sum(x => x.vl_juros_baixa) + baixasTitulo.Sum(x => x.vl_multa_baixa)) > 0))
                            {
                                parcela.Append(string.Format("Multa: R${0:0,0.00}", baixasTitulo.Any() ? (baixasTitulo.Sum(x => x.vl_juros_baixa) + baixasTitulo.Sum(x => x.vl_multa_baixa)) : 0));

                            }

                            parcela.Append(")");
                        }
                        //var parcela = string.Format("{0}-{1} - Vcto.:{2:dd/MM/yyyy} - R${3:0,0.00}  - Receb.:{4:dd/MM/yyyy} - R${5:0,0.00} (Desc: R${6:0,0.00}, Multa: R${7:0,0.00})", titulo.nm_titulo, titulo.nm_parcela_titulo, titulo.dt_vcto_titulo, titulo.vlTitulo, titulo.dt_liquidacao_titulo, baixasTitulo.Any() ? baixasTitulo.Sum(x => x.vl_liquidacao_baixa): 0, baixasTitulo.Any()? baixasTitulo.Sum(x => x.vl_desconto_baixa): 0, baixasTitulo.Any() ? (baixasTitulo.Sum(x => x.vl_juros_baixa) + baixasTitulo.Sum(x => x.vl_multa_baixa)): 0);
                        ParcelasReciboAgrupadoUI parcelasRecibo = new ParcelasReciboAgrupadoUI();
                        parcelasRecibo.parcela = parcela.ToString();
                        parc.Add(parcelasRecibo);
                    }

                    return parc;
                }

                return null;
            }
        }


        public string totalPagar
        {
            get
            {
                if (titulos != null && titulos.Count > 0)
                    //System.Diagnostics.Debug.WriteLine("desconto_titulo:" + titulos.Sum(b => b.vl_desconto_titulo));
                    return string.Format("R${0:0,0.00}", (titulos.Sum(b => b.vl_titulo)));
                return string.Format("R${0:0,0.00}", 0.0);
            }
        }

        public string totalPago
        {
            get
            {
                if (titulos != null && titulos.Count > 0)
                {
                    decimal totalPagoRetorno = 0;
                    foreach (var titulo in titulos)
                    {
                        List<BaixaTitulo> baixasTitulo = baixas.Where(x => x.cd_titulo == titulo.cd_titulo).ToList();
                        totalPagoRetorno += baixasTitulo.Any()? (baixasTitulo.Sum(b => b.vl_liquidacao_baixa)): 0;
                    }

                    return string.Format("R${0:0,0.00}", totalPagoRetorno);
                }

                return string.Format("R${0:0,0.00}", 0.0);
            }
        }

        public string totalMulta
        {
            get
            {
                if (titulos != null && titulos.Count > 0)
                    return string.Format("R${0:0,0.00}", titulos.Sum(b => b.vl_multa_liquidada) + titulos.Sum(b => b.vl_juros_liquidado));
                return string.Format("R${0:0,0.00}", 0.0);
            }
        }

        public string totalJuros
        {
            get
            {
                if (titulos != null && titulos.Count > 0)
                    return string.Format("R${0:0,0.00}", titulos.Sum(b => b.vl_juros_liquidado));
                return string.Format("R${0:0,0.00}", 0.0);
            }
        }

        public string totalDescontos
        {
            get
            {
                if (titulos != null && titulos.Count > 0)
                    return string.Format("R${0:0,0.00}", titulos.Sum(b => b.vl_desconto_titulo ));
                return string.Format("R${0:0,0.00}", 0.0);
            }
        }
    }
}
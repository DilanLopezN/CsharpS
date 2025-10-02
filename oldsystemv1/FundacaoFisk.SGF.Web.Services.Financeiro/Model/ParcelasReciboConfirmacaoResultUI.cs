using System;
using System.Collections.Generic;
using System.Linq;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Model
{
    public class ParcelasReciboConfirmacaoResultUI
    {
        public List<Titulo> titulos { get; set; }

        public List<String> parcelas
        {
            get
            {
                if (titulos != null && titulos.Count > 0)
                {
                    List<String> parc = new List<string>();
                    foreach (var titulo in titulos)
                    {
                        var parcela = string.Format("{0} - Vcto.:{1:dd/MM/yyyy} - R${2:0,0.00}  {3}", titulo.nm_parcela_titulo, titulo.dt_vcto_titulo, titulo.vl_titulo, (tipo_financeiro == "Diversos") ? ("- Tipo Financeiro: " + (titulo.cd_tipo_financeiro == (int)TipoFinanceiro.TiposFinanceiro.CHEQUE ? "Cheque" : "Cartão")) : "");
                        parc.Add(parcela);
                    }

                    return parc;
                }

                return null;
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

        public int qtd_cheque
        {
            get
            {
                if (titulos != null && titulos.Count > 0)
                {
                    return titulos.Where(z => z.cd_tipo_financeiro == (int)TipoFinanceiro.TiposFinanceiro.CHEQUE).Count();

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
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Model
{
    public class SaldoFinanceiro : TO
    {
        public int cd_conta_corrente { get; set; }
        public int? cd_local { get; set; }
        public int cd_tipo_liquidacao {get;set;}
        public string tipo { get; set; }
        public string banco {get;set;}
        public decimal saldo_inicial {get;set;}
        //public string tipo {get;set;}
        public decimal? entrada {get;set;}
        public decimal? saida {get;set;}
        public decimal saldo {get;set;}
        public byte nm_tipo {get;set;}
        public byte id_tipo_movimento {get;set;}
        public DateTime? dta_conta_corrente { get; set; }
        public IEnumerable<TipoLiquidacao> tiposLiquidacao { get; set; }
        public string id_tipo
        {
            get
            {
                string retorno = "";
                if (nm_tipo > 0)
                {
                    if (nm_tipo == (int)LocalMovto.TipoLocalMovtoEnum.CAIXA)
                        retorno = "Caixa";
                    else
                        retorno = "Banco";
                }
                return retorno;
            }
        }

    }
}
    
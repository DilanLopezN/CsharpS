using System;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Model
{
    public class SaldoFinanceiroParams
    {
        public int cd_escola { get; set; }
        public int cd_tipo_liquidacao { get; set; }
        public string  dta_base { get; set; }
        public byte tipo { get; set; }
    }
}
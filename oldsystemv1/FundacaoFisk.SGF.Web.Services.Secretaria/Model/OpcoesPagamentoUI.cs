using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FundacaoFisk.SGF.Web.Services.Financeiro.Model;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Model {
    public class OpcoesPagamentoUI {
        public int ano { get; set; }
        public int mes { get; set; }
        public int dia { get; set; }
        public int nro_parcelas { get; set; }
    }
}

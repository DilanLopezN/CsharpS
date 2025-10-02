using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Model {
    public class SimulacaoBaixa {
        public ICollection<Titulo> titulos { get; set; }
        public string dataBaixa { get; set; }
    }
}

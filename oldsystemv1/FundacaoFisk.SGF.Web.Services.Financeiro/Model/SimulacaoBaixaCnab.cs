using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Model {
    public class SimulacaoBaixaCnab {
        public ICollection<TituloCnab> titulos { get; set; }
        public string dataBaixa { get; set; }
        public byte id_tipo_cnab { get; set; }
    }
}

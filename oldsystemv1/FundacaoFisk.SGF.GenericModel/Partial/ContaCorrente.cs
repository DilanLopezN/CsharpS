using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.GenericModel {
    public partial class ContaCorrente {
        public enum Tipo { ENTRADA = 1, SAIDA = 2 }
        public enum TiposLiquidacao { LANCAMENTOMANUAL = 100 }
    }
}

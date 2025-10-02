using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class RelacionamentoSGF
    {
        public enum TipoGrauParentesco
        {
                MAE = 1,
                PAI = 2,
                MADASTRA = 3,
                PADASTRO = 4,
                AVO = 5,
                IRMA_O = 6,
                TIO_A = 7,
                PRIMO_A = 8
        }
        public bool ehRelacInverso { get; set; }
        public string desc_qualif_relacionamento { get; set; }
    }
}

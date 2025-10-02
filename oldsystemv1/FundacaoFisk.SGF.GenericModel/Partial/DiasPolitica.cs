using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class DiasPolitica
    {
        public string desconto
        {
            get
            {
                if (this.pc_desconto == null)
                    return "";
                return string.Format("{0,00}", this.pc_desconto);
            }
        }

        public DateTime dia_limite { get; set; }

        public int id_dias
        {
            get
            {
                return this.cd_dias_politica;
            }
        }
    }
}

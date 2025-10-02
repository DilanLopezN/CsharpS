using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.GenericModel {
    public partial class FeriadoDesconsiderado {
        public bool feriado_deletado { get; set; }

        public string dta_inicial {
            get {
                return String.Format("{0:dd/MM/yyyy}", dt_inicial);
            }
        }

        public string dta_final {
            get {
                return String.Format("{0:dd/MM/yyyy}", dt_final);
            }
        }

        public string dc_programacao_feriado {
            get {
                if(id_programacao_feriado)
                    return "Sim";
                else
                    return "Não";
            }
        }
    }
}
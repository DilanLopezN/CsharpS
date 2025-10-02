using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class AlunoEvento
    {
        public string dta_aula {
            get {
                if(this.DiarioAula != null)
                    return String.Format("{0:dd/MM/yyyy}", DiarioAula.dt_aula);
                else
                    return String.Empty;
            }
        }

        public string no_evento {
            get {
                if(this.Evento != null)
                    return this.Evento.no_evento;
                else
                    return String.Empty;
            }
        }
    }
}
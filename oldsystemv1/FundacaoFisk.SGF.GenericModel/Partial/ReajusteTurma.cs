using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class ReajusteTurma
    {

        public string no_turma
        {
            get
            {
                if (this.Turma != null)
                    return this.Turma.no_turma;
                else
                    return "";
            }
        }

        public string no_apelido
        {
            get
            {
                if (this.Turma != null)
                    return this.Turma.no_apelido;
                else
                    return "";
            }
        }
    }
}

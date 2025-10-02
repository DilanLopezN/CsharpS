using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class ReajusteCurso
    {

        public string no_curso
        {
            get {
                if (this.Curso != null)
                    return this.Curso.no_curso;
                else
                    return "";
            }
        }
    }
}

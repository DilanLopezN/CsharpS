using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class ControleFaltas : TO
    {
        public string no_turma { get; set; }
        public string no_usuario { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class ControleFaltasAluno : TO
    {
        public string no_situacao_aluno_turma { get; set; }
        public string no_aluno { get; set; }
    }
}

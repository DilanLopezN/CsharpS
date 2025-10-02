using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Model
{
    public class ReportProgramacaoTurmaAluno : TO {
        public int cd_aluno { get; set; }
        public string no_aluno { get; set; }
        public string dia_mes { get; set; }
        public string dia_mes_hor_min { get; set; }
        public DateTime dt_programacao { get; set; }
        public bool is_turma_regular { get; set; }
        public string evento { get; set; }
        public bool lancada { get; set; }
        public bool cancelada { get; set; }

    }
}
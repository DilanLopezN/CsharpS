using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FundacaoFisk.SGF.Web.Services.Empresa.Model;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Model
{
    public class TurmaAlunoProfessorHorario
    {

        public int cd_turma { get; set; }
        public int? cd_turma_ppt { get; set; }
        public Horario horario { get; set; }
        public List<Aluno> alunos { get; set; }
        public int[] professores { get; set; }
        public bool validarProgramacao { get; set; }
        public List<Horario> horarios { get; set; }
        public DateTime dt_inicio { get; set; }
        public DateTime? dt_final { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class ProfessorTurma
    {
        public string no_professor { get; set; }
        public virtual IEnumerable<HorarioProfessorTurma> HorariosProfessores { get; set; }

        public List<Horario> horarios {
            get {
                if(HorariosProfessores == null)
                    return null;
                else {
                    List<Horario> retorno = new List<Horario>();

                    foreach (HorarioProfessorTurma horProf in HorariosProfessores)
                        if(horProf.Horario != null)
                            retorno.Add(horProf.Horario);

                    return retorno;
                }
            }
        }

        public static List<ProfessorTurma> clonarProfessorZerandoMemoria(List<ProfessorTurma> professoresTurma,int cd_turma)
        {
            List<ProfessorTurma> ListProfTurma = new List<ProfessorTurma>();
            foreach (ProfessorTurma pt in professoresTurma)
            {
                ProfessorTurma newProfTurma = new ProfessorTurma();
                newProfTurma.cd_turma = cd_turma;
                newProfTurma.cd_professor = pt.cd_professor;
                newProfTurma.id_professor_ativo = pt.id_professor_ativo;
                ListProfTurma.Add(newProfTurma);
            }
            return ListProfTurma;
        }
    }
}

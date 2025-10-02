using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class DiarioAula
    {
        public enum PresencaProfesssor
        {
            Presente = 0,
            Falta = 1,
            Justificada = 2
        }

        public enum StatusDiarioAula
        {
            Efetivada = 0,
            Cancelada = 1
        }
        public int? cd_evento_diario { get; set; }
        public string no_turma { get; set; }
        public string desc_prog { get; set; }
        public string desc_tipo_aula { get; set; }
        public string no_sala { get; set; }
        public string no_prof { get; set; }
        public string no_avaliacao { get; set; }
        public string no_substituto { get; set; }
        public List<Professor> professoresHorario { get; set; }
        public List<Sala> salasDiario { get; set; }
        public List<MotivoFalta> mtvoFalta { get; set; }
        public List<TipoAtividadeExtra> tipoAtividadeExtra { get; set; }
        public List<Evento> eventos { get; set; }
        public List<Aluno> alunos { get; set; }
        public List<Avaliacao> avaliacoes { get; set; }
        public bool falta_professor { get; set; }
        public bool falta_justificada { get; set; }

        public string dta_aula
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", dt_aula);
            }
        }

        public string hr_cadastro_aula {
            get {
                return String.Format(@"{0:hh\:mm\:ss}", hr_inicial_aula) + " às " + String.Format(@"{0:hh\:mm\:ss}", hr_final_aula);
            }
        }
         //progsTurma = progsTurma, 
         //professoresHorario = professoresHorariosTurma, 
         //avaliacoesTurma = avaliacoesTurma,
    }
}

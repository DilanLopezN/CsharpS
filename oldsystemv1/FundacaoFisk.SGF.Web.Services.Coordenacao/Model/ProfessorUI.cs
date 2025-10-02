using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Model
{
    public class ProfessorUI
    {
        public int cd_pessoa { get; set; }
        public string no_pessoa { get; set; }
        public string no_fantasia { get; set; }
        public bool id_coordenador { get; set; }
    }

    public class HorariosTurmasHorariosProfessores
    {
        public List<Horario> horariosTurmaFilha { get; set; }
        public List<Horario> horariosTurmaPPT { get; set; }
        public int cd_curso { get; set; }
        public bool id_liberar_habilitacao_professor { get; set; }
    }
}

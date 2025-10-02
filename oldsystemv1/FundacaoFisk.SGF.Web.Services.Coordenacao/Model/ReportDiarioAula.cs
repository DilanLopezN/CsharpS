using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Model {
    public class ReportDiarioAula : TO {
        public int cd_professor { get; set; }
        public string no_turma { get; set; }
        public string no_apelido { get; set; }
        public string no_curso { get; set; }
        public string no_professor { get; set; }
        public string dc_horarios {
            get {
                double qtd_minutos_turma = 0;

                if(horarios != null)
                    return Horario.getDescricaoCompletaHorarios(Horario.montaDiaHorario(horarios.ToList(), ref qtd_minutos_turma));
                else
                    return "";
            }
        }
        public string no_sala { get; set; }
        public IEnumerable<Horario> horarios { get; set; }
    }
}
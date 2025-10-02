using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Model {
    public enum TipoAtualizacaoProgramacaoTurma {
        TIPO_REFAZER_PROGRAMACOES_CURSO = 1,
        TIPO_REFAZER_PROGRAMACOES_DATA_INICIAL = 2,
        TIPO_REFAZER_PROGRAMACOES_HORARIO = 3,
        TIPO_REFAZER_PROGRAMACOES_FERIADO = 4,
        TIPO_REFAZER_PROGRAMACOES_DESC_FERIADO = 5
    }

    public class ProgramacaoHorarioUI {
        public DateTime? dt_inicio { get; set; }
        public ICollection<ProgramacaoTurma> programacoes { get; set; }
        public ICollection<FeriadoDesconsiderado> feriados_desconsiderados { get; set; }
        public ICollection<Horario> horarios { get; set; }
        public int? cd_curso { get; set; }
        public int? cd_duracao { get; set; }
        public int? cd_turma { get; set; }
        public int? tipo { get; set; }
        public bool? modelo { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.GenericModel {
    public partial class AulaPersonalizadaAluno
    {
        public string no_aluno { get; set; }
        public string dc_aula { get; set; }
        public int cd_programacao_turma { get; set; }
        public int cd_turma { get; set; }
        public int cd_professor { get; set; }
        public int cd_sala_prog { get; set; }
        public short? nm_aula_programacao_turma { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Model {
    public class DiarioAulaProgramcoesReport : TO
    {
        public int cd_programacao_turma { get; set; }
        public int nm_aula_programacao_turma { get; set; }
        public string dc_programacao_turma { get; set; }
        public System.DateTime dt_programacao_turma { get; set; }
        public string tx_obs_aula { get; set; }

        public string no_produto { get; set; }
        public string dc_duracao { get; set; }
        public string no_curso { get; set; }
        public string no_apelido { get; set; }
        public string no_turma { get; set; }
        public string no_aluno { get; set; }
        public string no_professores { get; set; }

        public string dta_programacao_turma {
            get {
                if(this.dt_programacao_turma != null)
                    return String.Format("{0:dd/MM/yyyy}", dt_programacao_turma);
                else
                    return String.Empty;
            }
        }
    }
}
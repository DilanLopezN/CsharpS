using Componentes.GenericModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Model
{
    public class AulaPersonalizadaReport : TO
    {
        public int cd_turma { get; set; }
        public int cd_aula_personalizada_aluno { get; set; }
        public int nm_aula_programacao_turma { get; set; }
        public System.DateTime dta_programacao_turma { get; set; } 
        public string dc_programacao_turma { get; set; }            
        public string tx_obs_aula { get; set; }
        public System.DateTime dt_aula { get; set; }
        public Nullable<System.TimeSpan> hh_inicial_aluno { get; set; }
        public Nullable<System.TimeSpan> hh_final_aluno { get; set; }
        public string no_turma { get; set; }
        public string no_pessoa { get; set; }
        public string no_sala { get; set; }
        public bool id_aula_dada { get; set; }
        public string no_turma_original { get; set; }
    }
}

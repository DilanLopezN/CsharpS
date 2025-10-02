using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Model
{
    public class ControleFaltasReportUI : TO
    {
        public Nullable<int> cd_tipo { get; set; }
        public Nullable<int> cd_curso { get; set; }
        public Nullable<int> cd_nivel { get; set; }
        public Nullable<int> cd_produto { get; set; }
        public Nullable<int> cd_professor { get; set; }
        public Nullable<int> cd_turma { get; set; }
        public Nullable<int> cd_sit_turma { get; set; }
        public string cd_sit_aluno { get; set; }
        public Nullable<DateTime> dt_inicial { get; set; }
        public Nullable<DateTime> dt_final { get; set; }
        public bool quebrarpagina { get; set; }
    }
}

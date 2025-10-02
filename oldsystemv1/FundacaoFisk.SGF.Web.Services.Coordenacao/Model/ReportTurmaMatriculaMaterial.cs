using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Model
{
    public class ReportTurmaMatriculaMaterial : TO
    {
        public int? cd_turma { get; set; }
        public int cd_aluno { get; set; }
        public int? nm_contrato { get; set; }
        public string no_turma { get; set; }
        public string no_aluno { get; set; }
        public string no_material { get; set; }
        public DateTime? dt_matricula { get; set; }
        public DateTime? dt_venda { get; set; }
    }
}
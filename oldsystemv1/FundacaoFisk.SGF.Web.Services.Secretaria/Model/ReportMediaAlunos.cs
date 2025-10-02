using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Financeiro.Model;
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Model
{
    public class ReportMediaAlunos : TO
    {
        public enum TipoRelatorioMediasAlunos
        {
            ALUNOS_ATIVOS = 1,
            EX_ALUNOS = 2
        }
        public enum TipoRelatorioMediasAlunoTipoAluno
        {
            TODOS = 0,
            CONCLUINTES = 1,
            REPROVADOS = 2
        }
        public int cd_aluno { get; set; }
        public string no_aluno { get; set; }
        public string no_curso { get; set; }
        public string no_turma { get; set; }
        public string no_professor { get; set; }
        public double? vl_media { get; set; }
        public string dc_tipo_avaliacao { get; set; }
        public int divisor { get; set; }
        public double? vl_media_total { get; set; }
        public int? nm_notas_aluno { get; set; }
        public int? nm_avaliacao { get; set; }
        public int? nm_notas_avaliacao { get; set; }
        public double? vl_soma_notas_aluno { get; set; }
        public int? nm_provas { get; set; }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Model
{
    public class AvaliacaoTurmaUI : TO
    {
        public string no_turma { get; set; }
        public string no_tipo_criterio { get; set; }
        public Boolean isConceito { get; set; }
        public int cd_avaliacao_turma { get; set; }
        public int cd_turma { get; set; }
        public int cd_tipo_avaliacao { get; set; }
        public int cd_avaliacao { get; set; }
        public string dc_observacao { get; set; }
        public double somaNotas { get; set; }
        public Boolean is_conceito_nota { get; set; }
        public int? cd_conceito { get; set; }
        public int? cd_funcionario { get; set; }
        public int cd_criterio_avaliacao { get; set; }
        public int? cd_curso { get; set; }
        public int? cd_produto { get; set; }
        public string no_curso { get; set; }
        public string no_produto { get; set; }
        public string dt_avaliacao_turma { get; set; }
        public Boolean isModified { get; set; }
        public Boolean isInFocus { get; set; }

        public ICollection<Turma> turmas { get; set; }
        public ICollection<TipoAvaliacao> tiposAvaliacoes { get; set; }
        public ICollection<AvaliacaoTurma> avalialcoesTurmas { get; set; }
        public ICollection<AvaliacaoAluno> avalialcoesAlunos { get; set; }
        public ICollection<CriterioAvaliacao> criterioAvaliacao { get; set; }
        public ICollection<Curso> cursos { get; set; }
        public ICollection<ProfessorUI> funcionarios { get; set; }

        //Método para relatório
        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();
                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                retorno.Add(new DefinicaoRelatorio("no_turma", "Turma", AlinhamentoColuna.Left, "3.8000in"));
                retorno.Add(new DefinicaoRelatorio("no_tipo_criterio", "Tipo", AlinhamentoColuna.Left, "3.8000in"));
                return retorno;
            }
        }
    }

}

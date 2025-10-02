using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Componentes.GenericDataAccess.Comum;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess
{
    public interface IAvaliacaoTurmaDataAccess : IGenericRepository<AvaliacaoTurma>
    {
        IEnumerable<AvaliacaoTurmaUI> searchAvaliacaoTurma(SearchParameters parametros, int idTurma, bool? idTipoAvaliacao, int idEscola, int cd_tipo_avaliacao, int cd_criterio_avaliacao, int cd_curso, int cd_funcionario, DateTime? dta_inicial, DateTime? dta_final, int cd_escola_combo = 0);
        List<AvaliacaoTurma> getAvaliacaoTurmaArvore(int idTurma, int idEscola, bool? idConceito, int? cdFuncionario, int cd_tipo_avaliacao);
        bool incluirAvaliacoesTurma(List<AvaliacaoTurma> novasAvaliacoes);
        List<AvaliacaoTurmaUI> returnAvaliacoesConceitoOrNotaByTurma(int cd_turma, int cd_pessoa_escola);
        IEnumerable<AvaliacaoTurma> returnAvaliacoesTurmaSemAvaliacoeAluno(int cd_turma, int cd_escola, int cd_pessoa_aluno);
        bool existsAvaliacaoTurmaByDesistencia(DateTime data, int cd_pessoa_escola, int cd_aluno, int cd_turma);
        IEnumerable<AvaliacaoTurma> GetAvaliacaoTurmaByIdAvaliacao(int cd_avaliacao, int cd_escola);
        int gerarTurmasNulas(Nullable<int> cd_turma, Nullable<int> cd_tipo_avaliacao);
        //int gerarTurmasNulas(int? cd_turma, int? cd_tipo_avaliacao);
    }
}

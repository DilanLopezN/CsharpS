using Componentes.GenericDataAccess.Comum;
using FundacaoFisk.SGF.GenericModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess
{
    public interface IAvaliacaoAlunoDataAccess : IGenericRepository<AvaliacaoAluno>
    {
        IEnumerable<AvaliacaoAluno> getNotasAvaliacaoTurma(int cd_aluno, int cd_turma, int cd_escola);
        IEnumerable<AvaliacaoAluno> getNotasAvaliacaoTurmaPorEstagio(int cd_aluno, int cd_estagio, int cd_escola);
        IEnumerable<AvaliacaoAluno> getConceitosAvaliacaoAluno(int cd_aluno, int cd_turma, int cd_escola);
        bool incluirAvaliacoesAluno(List<AvaliacaoAluno> avaliacoesAluno);
        bool verificaAvalicaoAlunoTurma(int cdTurma, int cdAluno);
        IEnumerable<AvaliacaoAluno> getAvaliacaoesAlunoByIdTurmaEscola(int cd_turma, int cd_pessoa_escola, bool isConceito);

        IEnumerable<AvaliacaoAluno> getAvaliacaoAlunoByIdAvlTurma(int cd_avaliacao_turma, int cd_pessoa_escola, int cd_turma);
        bool existeNotaOuConceitoAvalicoesAluno(int cd_turma, int cd_pessoa_escola);

    }
}

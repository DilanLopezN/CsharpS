using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess.Comum;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.GenericModel.Partial;
using FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess;
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Comum.IDataAccess
{
    public interface IAlunoTurmaDataAccess  : IGenericRepository<AlunoTurma>
    {
        IEnumerable<AlunoTurma> findAlunosTurmaPorTurmaEscola(int cd_turma, int cd_escola);
        IEnumerable<AlunoTurma> findAlunosTurma(int cd_turma, int cd_escola);
        bool deleteAlunoAguardandoTurma(int cdProduto, int cdEscola, int cdContrato, int cdAluno, int cd_turma);
        AlunoTurma findAlunoTurma(int cd_aluno, int cd_turma, int cd_escola);
        LivroAlunoApiCyberBdUI findLivroAlunoTurmaApiCyber(int cd_aluno, int cd_turma, int cd_escola);
        AlunoTurma findAlunoTurmaByCdCursoContrato(int cd_curso_contrato, int cd_escola);
        List<AlunoTurma> existsAlunosTurmaInTurmaDestino(List<int> cdsAlunosTurma, int cdTurmaDestino);
        int findAlunoTurmaProduto(int cd_aluno, int cd_turma, int cd_escola);
        bool existsAlunoTurmaByContratoEscola(int cd_contrato, int cd_pessoa_escola);
        List<AlunoTurma> findAlunoTurmasByContratoEscola(int cd_contrato, int cd_pessoa_escola);
        int existAlunoMatriculadoOuRematriculado(int cd_aluno, int cd_escola, int cd_turma);
        IEnumerable<AlunoTurma> findAlunosTurma(int cd_turma, int cd_escola, int[] cdAlunos);
        IEnumerable<AlunoTurma> getAlunosTurmaAtivosDiarioAula(int cd_turma, int cd_pessoa_escola, DateTime dtAula);
        bool getVerificaAlunosTurma(List<int> listaAlunos, int cd_turma);
        int? getStatusAlunoTurma(int cd_aluno, int cd_escola, int cd_turma);
        List<AlunoTurma> findAlunosTurmaForEncerramento(int cd_turma, int cd_escola);
        AlunoTurma findAlunoTurmaContrato(int cd_aluno, int cd_turma, int cd_escola, int cd_contrato);
        AlunoTurma findAlunoTurmaMovido(int cd_aluno, int cd_turma, int cd_escola);
        IEnumerable<AlunoTurma> findAlunosTurmaHist(int cd_turma, int cd_escola, int[] cdAlunos);
        IEnumerable<AlunoTurma> getAlunoTurmaByCdContrato(int cd_contrato);
        IEnumerable<AlunoTurma> getAlunoTurmaByCdContratoAndCdAluno(int cd_contrato, int cd_aluno);
        AlunoApiCyberBdUI findAlunoApiCyber(int cd_aluno, int cd_turma, int cd_contrato);
        PromocaoIntercambioParams findAlunoApiPromocaoIntercambio(int cd_aluno, int id_tipo_matricula);
        LivroAlunoApiCyberBdUI findLivroAlunoApiCyber(int cd_aluno, int cd_turma, int cd_contrato);
    }
}

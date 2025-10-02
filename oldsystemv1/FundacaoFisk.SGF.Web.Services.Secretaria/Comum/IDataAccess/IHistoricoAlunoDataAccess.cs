using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess.Comum;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess;
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Comum.IDataAccess
{
    public interface IHistoricoAlunoDataAccess : IGenericRepository<HistoricoAluno>
    {
        HistoricoAluno GetHistoricoAlunoById(int cdEscola, int cdAluno, int cdTurma, int cdContrato);
        IEnumerable<HistoricoAluno> GetHistoricosAlunoById(int cdEscola, int cdAluno, int cdTurma, int cdContrato);
        int GetUltimoHistoricoAluno(int cdEscola, int cdAluno, int cdProduto);
        IEnumerable<HistoricoAluno> returnHistoricoSitacaoAlunoTurma(int cd_turma, int cd_pessoa_escola);
        int retunMaxSequenciaHistoricoAluno(int cd_produto, int cd_pessoa_escola, int cd_aluno);
        HistoricoAluno GetHistoricoAlunoPrimeiraAula(int cdEscola, int cdAluno, int cdTurma, int cdContrato, DateTime dataDiario);
        HistoricoAluno GetHistoricoAlunoMovido(int cdEscola, int cdAluno, int cdTurma, int cdContrato);
        IEnumerable<HistoricoAluno> getHistoricoTurmas(int cd_aluno, int cd_escola);
        HistoricoAluno getUltimoHistoricoAlunoPorCodTurma(int cd_aluno, int cd_escola, int cd_turma);
        HistoricoAluno GetHistoricoAlunoPorDesistencia(int cdDesistencia);
        HistoricoAluno getHistoricoAlunoByMatricula(int cdEscola, int cdAluno, int cdTurma, int cdContrato);
        HistoricoAluno getSituacaoAlunoCancelEncerramento(int cd_aluno, int cd_turma, DateTime dt_historico);
        DateTime? buscarDataHistoricoDesistenciaAlteriorCancelamento(int cd_aluno, int cd_turma, DateTime dt_historico, byte nm_sequencia);
        DataTable getAlunos(int cd_aluno, int Tipo, string produtos, string statustitulo);
        List<sp_RptHistoricoAlunoM_Result> getRtpHistoricoAlunoM(int cdAluno);
        List<st_RptFaixaEtaria_Result> getRtpFaixaEtaria(int cd_escola, int tipo, int idade, int idade_max, int cd_turma);
        DataTable getRtpFaixaEtariaDT(int cd_escola, int tipo, int idade, int idade_max, int cd_turma);
        List<ProdutoHistoricoSeachUI> getProdutosComHistorico(int cdEscola);
    }
}

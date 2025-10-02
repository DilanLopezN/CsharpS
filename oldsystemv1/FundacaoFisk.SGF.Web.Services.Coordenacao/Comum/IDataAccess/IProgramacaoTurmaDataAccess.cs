using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess.Comum;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess
{
    public interface IProgramacaoTurmaDataAccess : IGenericRepository<ProgramacaoTurma>
    {
        IEnumerable<ProgramacaoTurma> getProgramacaoTurmaByTurma(int cd_turma, int cd_escola, ProgramacaoTurmaDataAccess.TipoConsultaProgTurmaEnum tipoConsulta, bool idMostrarFeriado);
        IEnumerable<ProgramacaoTurma> getProgramacoesTurma(int cd_escola, int cd_turma, int cd_professor, DateTime? dt_inicial, DateTime dt_final);
        IEnumerable<ProgramacaoTurma> getProgramacaoComDesconsideraFeriado(FeriadoDesconsiderado feriadoDesc);
        bool existeAulaEfetivadaTurma(int? cd_turma,int? cd_turma_ppt, int cd_escola);
        string existeProgInsuficiente(Turma turma);
        void atualizaProgramacaoRemovendoDesconsideraFeriado(int cd_feriado_desconsiderado);
        int getQuantidadeAulasProgramadasTurma(int cd_turma, int cd_escola);
        IEnumerable<ProgramacaoTurma> getProgramacoesTurmaPorAluno(int cd_escola, int cd_aluno, DateTime dt_inicial, int cdTurma, int cd_turma_principal, List<int> listaProgAlunos);
        bool verificaProgramacoesComDiario(List<int> cds_prog, int cd_escola);
        List<ProgramacaoTurma> getProgramacaoTurmaEditEncerramentoTurma(int cd_turma, DateTime dt_termino);
    }
}

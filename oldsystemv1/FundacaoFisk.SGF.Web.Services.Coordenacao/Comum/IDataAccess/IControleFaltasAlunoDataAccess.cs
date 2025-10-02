using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess.Comum;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Services.Coordenacao.Model;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;
using FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess;
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;

namespace FundacaoFisk.SGF.Services.Coordenacao.Comum.IDataAccess
{
    public interface IControleFaltasAlunoDataAccess : IGenericRepository<ControleFaltasAluno>
    {
        //ControleFaltasAluno
        IEnumerable<ControleFaltasAlunoUI> getAlunosTurmaControleFalta(int cd_turma, int cd_pessoa_escola, DateTime? dt_inicial, DateTime dt_final, int cd_controle_faltas, AlunoTurma.FiltroSituacaoAlunoTurma situacao_aluno);
        ControleFaltasAlunoUI getAlunoControleFalta(int cd_turma, int cd_pessoa_escola, int cd_aluno, DateTime? dt_inicial, DateTime dt_final, AlunoTurma.FiltroSituacaoAlunoTurma situacao_aluno);
        ControleFaltasAluno getAlunoControleEstoqueByCdItem(int cd_alunoControleFaltas);
        List<ControleFaltasAlunoUI> getAlunosControleFalta(int cd_controle_faltas);
    }
}


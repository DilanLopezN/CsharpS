using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess.Comum;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess
{
    public interface IProfessorTurmaDataAccess : IGenericRepository<ProfessorTurma>
    {
        IEnumerable<ProfessorTurma> findProfessoresTurmaPorTurmaEscola(int cd_turma, int cd_escola);
        IEnumerable<ReportDiarioAula> getRelatorioDiarioAula(int cd_escola, int cd_turma, int cd_professor, DateTime dt_inicial, DateTime dt_final);
    }
}

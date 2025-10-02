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
    public interface IHorarioProfessorTurmaDataAccess : IGenericRepository<HorarioProfessorTurma> {
        IEnumerable<HorarioProfessorTurma> getHorarioProfessorByHorario(int cd_horario);
        IEnumerable<HorarioProfessorTurma> getHorarioProfessorByProfessorTurmaPPT(int cd_professor, int cd_turma_ppt);
    }
}

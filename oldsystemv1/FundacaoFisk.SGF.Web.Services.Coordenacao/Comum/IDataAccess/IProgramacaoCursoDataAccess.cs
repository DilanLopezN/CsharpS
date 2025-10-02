using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using Componentes.GenericDataAccess.Comum;
using System.Data.Entity;
using Componentes.Utils;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess
{
    using FundacaoFisk.SGF.GenericModel;
    using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;
    public interface IProgramacaoCursoDataAccess : IGenericRepository<ProgramacaoCurso>
    {
        IEnumerable<ProgramacaoCursoUI> getProgramacaoCursoSearch(SearchParameters parametros, int? cdCurso, int? cdDuracao, int? cd_escola);
        bool deleteAllProgramacoesCursos(List<ProgramacaoCurso> programacoesCursos);
        ProgramacaoCursoUI GetProgramacaoById(int cdProgramacao);
        ProgramacaoCurso getProgramacao(int cdCurso, int cdDuracao, int? cd_escola);
        ProgramacaoCurso firstOrDefault();
        bool existeModeloProgramacaoByTurma(int cd_turma, int cd_escola);
    }
}

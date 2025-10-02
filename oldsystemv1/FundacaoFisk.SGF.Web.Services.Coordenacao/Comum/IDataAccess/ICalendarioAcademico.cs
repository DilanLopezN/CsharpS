using Componentes.GenericDataAccess.Comum;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.Services.Coordenacao.Comum.IDataAccess
{
    public interface ICalendarioAcademico : IGenericRepository<CalendarioAcademico>
    {
        IEnumerable<CalendarioAcademico> obterListaCalendarioAcademicos(int cd_escola);
        CalendarioAcademico findCalendarioAcademicoById(int cd_calendario_academico, int cd_escola);
        IEnumerable<CalendarioAcademico> obterCalendarioAcademicosPorFiltros(SearchParameters parametros, int cd_escola, int tipo_calendario, bool? status, bool relatorio);
        List<int> findEscolas();
        CalendarioAcademico findCalendarioAcademico(int cd_escola, byte tipo);
    }
}

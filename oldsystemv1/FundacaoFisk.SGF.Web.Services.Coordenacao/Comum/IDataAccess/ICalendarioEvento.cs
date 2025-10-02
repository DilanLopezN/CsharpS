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
    public interface ICalendarioEvento : IGenericRepository<CalendarioEvento>
    {
        IEnumerable<CalendarioEvento> obterListaCalendarioEventos(int cd_escola);
        CalendarioEvento findCalendarioEventoById(int cd_calendario_evento, int cd_escola);
        IEnumerable<CalendarioEvento> obterCalendarioEventosPorFiltros(SearchParameters parametros, int cd_escola, string dc_titulo_evento, bool inicio, bool? status, string dt_inicial_evento,
            string dt_final_evento, string hh_inicial_evento, string hh_final_evento);
    }
}

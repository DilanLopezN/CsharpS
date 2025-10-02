using Componentes.GenericDataAccess;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Services.Coordenacao.Comum.IDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.Services.Coordenacao.DataAccess
{
    public class CalendarioEventoDataAccess : GenericRepository<CalendarioEvento>, ICalendarioEvento
    {
        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }

        public IEnumerable<CalendarioEvento> obterListaCalendarioEventos(int cd_escola)
        {
            try
            {
                var sql = from c in db.CalendarioEvento
                          where c.cd_pessoa_escola == cd_escola
                          select c;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public CalendarioEvento findCalendarioEventoById(int cd_calendario_evento, int cd_escola)
        {
            try
            {
                var sql = (from c in db.CalendarioEvento
                           where c.cd_pessoa_escola == cd_escola && c.cd_calendario_evento == cd_calendario_evento
                           select c).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<CalendarioEvento> obterCalendarioEventosPorFiltros(SearchParameters parametros, int cd_escola, string dc_titulo_evento, bool inicio, bool? status, string dt_inicial_evento, 
            string dt_final_evento, string hh_inicial_evento, string hh_final_evento)
        {
            IEntitySorter<CalendarioEvento> sorter = EntitySorter<CalendarioEvento>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
            IQueryable<CalendarioEvento> sql;

            sql = from evento in db.CalendarioEvento
                  where evento.cd_pessoa_escola == cd_escola
                  select evento;

            if (!string.IsNullOrEmpty(dc_titulo_evento))
            {
                if (inicio)
                {
                    sql = from evento in sql
                          where evento.dc_titulo_evento.StartsWith(dc_titulo_evento)
                          select evento;
                }
                else
                {
                    sql = from evento in sql
                          where evento.dc_titulo_evento.Contains(dc_titulo_evento)
                          select evento;
                }
            }

            if (status != null)
            {
                sql = from evento in sql
                      where evento.id_ativo == status
                      select evento;
            }

            if (!string.IsNullOrEmpty(dt_inicial_evento))
            {
                var dtInicialEvento = DateTime.Parse(dt_inicial_evento);

                sql = from evento in sql
                      where evento.dt_inicial_evento >= dtInicialEvento
                      select evento;
            }

            if (!string.IsNullOrEmpty(dt_final_evento))
            {
                var dtFinalEvento = DateTime.Parse(dt_final_evento);

                sql = from evento in sql
                      where evento.dt_final_evento <= dtFinalEvento
                      select evento;
            }

            if (!string.IsNullOrEmpty(hh_inicial_evento))
            {
                var hhIncialEvento = TimeSpan.Parse(hh_inicial_evento);
                sql = from evento in sql
                      where evento.hh_inicial_evento >= hhIncialEvento
                      select evento;
            }

            if (!string.IsNullOrEmpty(hh_final_evento))
            {
                var hhFinalEvento = TimeSpan.Parse(hh_final_evento);
                sql = from evento in sql
                      where evento.hh_final_evento <= hhFinalEvento
                      select evento;
            }

            sql = sorter.Sort(sql);
            var retorno = from evento in sql select evento;
            int limite = retorno.Count();

            parametros.ajustaParametrosPesquisa(limite);
            parametros.qtd_limite = limite;

            retorno = retorno.Skip(parametros.from).Take(parametros.qtd_limite);
            return retorno;
        }
    }
}

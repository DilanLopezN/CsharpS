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
    public class CircularDataAccess : GenericRepository<Circular>, ICircular
    {
        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }       

        public IEnumerable<Circular> obterListaCirculares()
        {
            try
            {
                var sql = from c in db.Circular select c;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Circular findCircularById(int cd_circular)
        {
            try
            {
                var sql = (from c in db.Circular.AsNoTracking() 
                    where c.cd_circular == cd_circular select c).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Circular findDeletedCircularById(int cd_circular)
        {
            try
            {
                var sql = (from c in db.Circular
                    where c.cd_circular == cd_circular
                    select c).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Circular> obterCircularesPorFiltros(SearchParameters parametros, short nm_ano_circular, List<byte> nm_mes_circular, int nm_circular, string no_circular, List<byte> nm_menu_circular)
        {
            IEntitySorter<Circular> sorter = EntitySorter<Circular>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
            IQueryable<Circular> sql, sql_aux = Enumerable.Empty<Circular>().AsQueryable(), sql_result = Enumerable.Empty<Circular>().AsQueryable(); 

            sql = from circular in db.Circular.AsNoTracking() 
                  select circular;

            if (!String.IsNullOrEmpty(no_circular))
            {
                sql = from circular in sql
                    where circular.no_circular.Contains(no_circular)
                    select circular;
            }

            if (nm_circular > 0)
            {
                sql = from circular in sql
                    where circular.nm_circular.Equals(nm_circular)
                    select circular;
            }

            if (nm_ano_circular > 0)
            {
                sql = from circular in sql
                    where circular.nm_ano_circular.Equals(nm_ano_circular)
                    select circular;
            }

            if (nm_menu_circular != null)
            {
                if (nm_menu_circular.Count() > 0)
                {
                    sql_result = filterMenus(nm_menu_circular, sql, sql_result, sql_aux);
                }

                sql = sql_result;
            }

            

            sql_aux = Enumerable.Empty<Circular>().AsQueryable();
            sql_result = Enumerable.Empty<Circular>().AsQueryable();

            if (nm_mes_circular != null)
            {
                if (nm_mes_circular.Count() > 0)
                {
                    sql_result = filterMonths(nm_mes_circular, sql, sql_result, sql_aux);
                }
                sql = sql_result;
            }

            sql = sorter.Sort(sql);
            var retorno = from evento in sql select evento;
            int limite = retorno.Count();

            parametros.ajustaParametrosPesquisa(limite);
            parametros.qtd_limite = limite;

            retorno = retorno.Skip(parametros.from).Take(parametros.qtd_limite);
            return retorno;

        }

        private static IQueryable<Circular> filterMenus(List<byte> menu, IQueryable<Circular> sql, IQueryable<Circular> sql_result, IQueryable<Circular> sql_aux)
        {
            foreach (var item in menu)
            {
                sql_aux = from circular in sql
                    where circular.nm_menu_circular.Equals(item)
                    select circular;

                if (sql_result.Count() > 0)
                {
                    sql_result = sql_result.Union(sql_aux);
                }
                else
                {
                    sql_result = sql_aux;
                }
            }

            return sql_result;
        }

        private static IQueryable<Circular> filterMonths(List<byte> menu, IQueryable<Circular> sql, IQueryable<Circular> sql_result, IQueryable<Circular> sql_aux)
        {
            foreach (var item in menu)
            {
                sql_aux = from circular in sql
                    where circular.nm_mes_circular.Equals(item)
                    select circular;

                if (sql_result.Count() > 0)
                {
                    sql_result = sql_result.Union(sql_aux);
                }
                else
                {
                    sql_result = sql_aux;
                }
            }

            return sql_result;
        }
    }
}

using Componentes.GenericDataAccess;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Services.Coordenacao.Comum.IDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FundacaoFisk.SGF.Services.Coordenacao.DataAccess
{
    public class FaqDataAccess : GenericRepository<Faq>, IFaqDataAccess
    {
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }


        public IEnumerable<Faq> getFaqSearch(Componentes.Utils.SearchParameters parametros, string no_faq, int nm_faq, List<byte> menu)
        {
            throw new NotImplementedException();
        }

        public Faq findFaqById(int cd_faq)
        {
            try
            {
                var sql = (from c in db.Faq.AsNoTracking()
                           where c.cd_faq == cd_faq
                           select c).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Faq findDeletedFaqById(int cd_faq)
        {
            try
            {
                var sql = (from c in db.Faq
                           where c.cd_faq == cd_faq
                           select c).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Faq> obterFaqsPorFiltros(Componentes.Utils.SearchParameters parametros, string dc_faq_pergunta, bool dc_faq_inicio, List<byte> menu)
        {
            try
            {
                IEntitySorter<Faq> sorter = EntitySorter<Faq>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<Faq> sql, sql_aux = Enumerable.Empty<Faq>().AsQueryable(), sql_result = Enumerable.Empty<Faq>().AsQueryable();

                sql = from faq in db.Faq.AsNoTracking()
                      select faq;


                if (!String.IsNullOrEmpty(dc_faq_pergunta))
                {
                    if (dc_faq_inicio)
                    {
                        sql = from faq in sql
                              where faq.dc_faq_pergunta.StartsWith(dc_faq_pergunta)
                              select faq;
                    }
                    else
                    {
                        sql = from faq in sql
                              where faq.dc_faq_pergunta.Contains(dc_faq_pergunta)
                              select faq;
                    }
                }                

                if (menu != null)
                {
                    if (menu.Count() > 0)
                    {
                        sql_result = filterMenus(menu, sql, sql_result, sql_aux);
                    }
                    else
                        sql_result = sql;
                }
                else
                    sql_result = sql;

                sql = sql_result;
                sql = sorter.Sort(sql);

                int limite = sql.Count();

                parametros.ajustaParametrosPesquisa(limite);
                sql = sql.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;

                return sql.ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Faq findFaqByNumeroParte(int nm_faq, int nm_parte)
        {
            throw new NotImplementedException();
        }

        public Faq findFaqByName(string no_faq)
        {
            throw new NotImplementedException();
        }

        private static IQueryable<Faq> filterMenus(List<byte> menu, IQueryable<Faq> sql, IQueryable<Faq> sql_result, IQueryable<Faq> sql_aux)
        {
            foreach (var item in menu)
            {
                sql_aux = from faq in sql
                          where faq.nm_menu_faq.Equals(item)
                          select faq;

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

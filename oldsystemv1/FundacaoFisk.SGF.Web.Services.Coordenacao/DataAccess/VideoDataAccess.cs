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
    public class VideoDataAccess : GenericRepository<Video>, IVideoDataAccess
    {
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }

        public IEnumerable<Video> getVideoSearch(Componentes.Utils.SearchParameters parametros, string no_video, int nm_video, List<byte> menu)
        { 
             try{
                 IEntitySorter<Video> sorter = EntitySorter<Video>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<Video> sql;

                sql = from video in db.Video.AsNoTracking()
                      select video;


                 if (!String.IsNullOrEmpty(no_video)) {
                    sql = from video in sql
                                  where video.no_video.Contains(no_video)
                                  select video;
                 }

                 if (menu != null)
                 {
                     if (menu.Count() > 0)
                     {
                         foreach (var item in menu)
                         {
                             sql = from video in sql
                                   where video.nm_menu_video.Equals(item)
                                   select video;
                         }
                     }
                 }
                
               
                sql = sorter.Sort(sql);
                var retorno = from video in sql
                              select video;
               
                int limite = retorno.Count();

                parametros.ajustaParametrosPesquisa(limite);
                retorno = retorno.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        
        }

        public Video findVideoByNumeroParte(int nm_video, int nm_parte)
        {
            try
            {
                var sql = (from c in db.Video.AsNoTracking()
                    where c.nm_video == nm_video && c.nm_parte == nm_parte
                    select c).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Video findVideoById(int cd_video)
        {
            try
            {
                var sql = (from c in db.Video.AsNoTracking()
                           where c.cd_video == cd_video
                           select c).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public Video findVideoByName(string no_video)
        {
            try
            {
                var sql = (from c in db.Video.AsNoTracking()
                           where c.no_video.Equals(no_video) 
                           select c).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Video findDeletedVideoById(int cd_video)
        {
            try
            {
                var sql = (from c in db.Video
                    where c.cd_video == cd_video
                    select c).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Video> obterVideosPorFiltros(Componentes.Utils.SearchParameters parametros, string no_video, int nm_video, List<byte> menu)
        {
            try
            {
                IEntitySorter<Video> sorter = EntitySorter<Video>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<Video> sql, sql_aux = Enumerable.Empty<Video>().AsQueryable(), sql_result = Enumerable.Empty<Video>().AsQueryable(); 
                
                sql = from video in db.Video.AsNoTracking()
                      select video;


                if (!String.IsNullOrEmpty(no_video))
                {
                    sql = from video in sql
                          where video.no_video.Contains(no_video)
                          select video;
                }

                if (nm_video > 0)
                {
                    sql = from video in sql
                        where video.nm_video.Equals(nm_video)
                        select video;
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

                //if (sql_result.Count() > 0)
                //{
                    sql = sql_result;
                //}
                

                sql = sorter.Sort(sql);
                var retorno = from video in sql
                              select video;

                int limite = retorno.Count();

                parametros.ajustaParametrosPesquisa(limite);
                parametros.qtd_limite = limite;
                retorno = retorno.Skip(parametros.from).Take(parametros.qtd_limite);
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }

        }

        private static IQueryable<Video> filterMenus(List<byte> menu, IQueryable<Video> sql, IQueryable<Video> sql_result, IQueryable<Video> sql_aux)
        {
            foreach (var item in menu)
            {
                sql_aux = from video in sql
                    where video.nm_menu_video.Equals(item)
                    select video;

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

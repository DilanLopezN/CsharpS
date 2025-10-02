using System;
using System.Collections.Generic;
using System.Linq;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum;
using Componentes.GenericModel;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using Componentes.GenericDataAccess;
using FundacaoFisk.SGF.Web.Services.Coordenacao;
using System.Data.Entity;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess;
using Componentes.GenericDataAccess.GenericException;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess
{
    public class NivelDataAccess : GenericRepository<Nivel>, INivelDataAccess
    {

        public enum TipoConsultaNivelEnum
        {
            HAS_ATIVO = 0
            
        }
        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }

        public IEnumerable<Nivel> getNivelSearch(SearchParameters parametros, string desc, bool inicio, bool? status)
        {
            try
            {
                IEntitySorter<Nivel> sorter = EntitySorter<Nivel>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<Nivel> sql;

                sql = from p in db.Nivel.AsNoTracking()
                      select p;

                if (status != null)
                    sql = from p in sql
                          where p.id_ativo == status
                          select p;

                if (!String.IsNullOrEmpty(desc))
                    if (inicio)
                        sql = from p in sql
                              where p.dc_nivel.StartsWith(desc)
                              select p;
                    else
                        sql = from p in sql
                              where p.dc_nivel.Contains(desc)
                              select p;

                var retorno = sorter.Sort(sql);

                int limite = retorno.Count();

                parametros.ajustaParametrosPesquisa(limite);
                parametros.qtd_limite = limite;
                var retorno2 = retorno.Skip(parametros.from).Take(parametros.qtd_limite);

                return retorno2;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public int GetUltimoNmOrdem(int cd_nivel)
        {
            var nm_ordem = (from nivel in db.Nivel
                                select nivel.nm_ordem).Max(c => c);

            return nm_ordem;
        }

        public List<Nivel> getNiveis(List<int> cdsPart)
        {
            try
            {
                //Maior quantidade de nives 
                List<Nivel> listNivel = (from p in db.Nivel
                                                       where !cdsPart.Contains(p.cd_nivel) &&
                                                       p.id_ativo
                                                        orderby p.cd_nivel
                                                       select p).ToList();
                return listNivel;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Nivel> findNivel(TipoConsultaNivelEnum hasDependente)
        {
            try
            {
                IQueryable<Nivel> retorno = null;
                switch (hasDependente)
                {
                    case TipoConsultaNivelEnum.HAS_ATIVO:
                        retorno = from nivel in db.Nivel
                                  where nivel.id_ativo == true 
                                  orderby nivel.nm_ordem
                                  select nivel;
                        break;


                }
                return retorno.ToList<Nivel>();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        
    }
}

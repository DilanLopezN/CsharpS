using System;
using System.Collections.Generic;
using System.Linq;
using Componentes.GenericModel;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using Componentes.GenericDataAccess;
using FundacaoFisk.SGF.Web.Services.Financeiro;
using System.Data.Entity;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess;
using Componentes.GenericDataAccess.GenericException;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess
{
    public class GrupoEstoqueDataAccess : GenericRepository<GrupoEstoque>, IGrupoEstoqueDataAccess
    {
        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public IEnumerable<GrupoEstoque> GetGrupoEstoqueSearch(SearchParameters parametros, string descricao, bool inicio, bool? ativo, int categoria)
        {
            try{
                IEntitySorter<GrupoEstoque> sorter = EntitySorter<GrupoEstoque>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<GrupoEstoque> sql;
                
                if (ativo == null)
                {
                    sql = from c in db.GrupoEstoque.AsNoTracking()
                          select c;
                }
                else
                {
                    sql = from c in db.GrupoEstoque.AsNoTracking()
                          where (c.id_grupo_estoque_ativo == ativo)
                          select c;                  
                }
                if (categoria > 0)
                    sql = from c in sql
                          where c.id_categoria_grupo == categoria
                          select c;

                sql = sorter.Sort(sql);
                
                var retorno = from c in sql
                              select c;

                if (!String.IsNullOrEmpty(descricao))
                    if (inicio)
                        retorno = from c in sql
                                  where c.no_grupo_estoque.StartsWith(descricao)
                                  select c;
                    else
                        retorno = from c in sql
                                  where c.no_grupo_estoque.Contains(descricao)
                                  select c;

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


        public bool deleteAllGrupo(List<GrupoEstoque> grupos)
        {
            try{
                string strgrupo = "";
                if (grupos != null && grupos.Count > 0)
                {
                    foreach (GrupoEstoque gruposEstoque in grupos)
                    {
                        strgrupo += gruposEstoque.cd_grupo_estoque + ",";
                    }
                }
                if (strgrupo.Length > 0)
                {
                    strgrupo = strgrupo.Substring(0, strgrupo.Length - 1);
                }
                int retorno = db.Database.ExecuteSqlCommand("delete from t_grupo_estoque where cd_grupo_estoque in(" + strgrupo + ")");

                return retorno > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public List<GrupoEstoque> findAllGrupoAtivo(int cdGrupo, bool isMasterGeral)
        {
            try{
                IEnumerable<GrupoEstoque> sql = new List<GrupoEstoque>();
                if (isMasterGeral)
                    sql = (from grupos in db.GrupoEstoque
                           where (grupos.id_grupo_estoque_ativo == true                           
                           && grupos.id_categoria_grupo == (int)GrupoEstoque.CategoriaEnum.PRIVADA)
                           || (cdGrupo == 0 || grupos.cd_grupo_estoque == cdGrupo)
                           //orderby grupos.no_grupo_estoque
                           select grupos).Distinct();
                else
                    if(cdGrupo == 0)
                    sql = (from grupos in db.GrupoEstoque
                           where grupos.id_grupo_estoque_ativo == true
                           && grupos.id_categoria_grupo == (int)GrupoEstoque.CategoriaEnum.PUBLICA
                           //orderby grupos.no_grupo_estoque
                           select grupos).Distinct();
                    else
                        sql = (from grupos in db.GrupoEstoque
                               where (grupos.id_grupo_estoque_ativo == true
                               && grupos.id_categoria_grupo == (int)GrupoEstoque.CategoriaEnum.PUBLICA)
                               || (cdGrupo == 0 || grupos.cd_grupo_estoque == cdGrupo)
                               //orderby grupos.no_grupo_estoque
                               select grupos).Distinct();

                return sql.OrderBy(x => x.no_grupo_estoque).ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public List<GrupoEstoque> findAllGrupoWithItem(int cd_pessoa_escola, bool isMaster)
        {
            try
            {
                IQueryable<GrupoEstoque> sql;
                if (isMaster)
                    sql = from grupos in db.GrupoEstoque
                           where grupos.Itens.Any()
                           orderby grupos.no_grupo_estoque ascending
                           select grupos;
                else
                    sql = from grupos in db.GrupoEstoque
                           where grupos.Itens.Any(i => i.ItemEscola.Any(it => it.cd_pessoa_escola == cd_pessoa_escola))
                           orderby grupos.no_grupo_estoque ascending
                           select grupos;
                
                return sql.ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}

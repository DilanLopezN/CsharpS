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
using System.Data.SqlClient;
using FundacaoFisk.SGF.Web.Services.Financeiro.Model;
using Componentes.GenericDataAccess.GenericException;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess
{
    public class CFOPDataAccess : GenericRepository<CFOP>, ICFOPDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public CFOP getCFOPByTpNF(int cd_tipo_nota)
        {
            try{
                CFOP sql = (from i in db.CFOP
                             where i.TiposNF.Where(t => t.cd_tipo_nota_fiscal == cd_tipo_nota).Any()
                             select i).FirstOrDefault();

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<CFOP> searchCFOP(SearchParameters parametros, string descricao, bool inicio, int nm_CFOP, byte id_natureza_CFOP)
        {
            try{
                IEntitySorter<CFOP> sorter = EntitySorter<CFOP>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<CFOP> sql;
                sql = from t in db.CFOP.AsNoTracking()
                      select t;
                if (!String.IsNullOrEmpty(descricao))
                    if (inicio)
                        sql = from c in sql
                              where c.dc_cfop.StartsWith(descricao)
                              select c;
                    else
                        sql = from c in sql
                              where c.dc_cfop.Contains(descricao)
                              select c;
                if (nm_CFOP > 0)
                    sql = from c in sql
                          where c.nm_cfop == nm_CFOP
                          select c;
                if (id_natureza_CFOP > 0)
                    sql = from c in sql
                          where c.id_natureza_cfop == id_natureza_CFOP
                          select c;
                
                sql = sorter.Sort(sql);
                int limite = sql.Count();
                parametros.ajustaParametrosPesquisa(limite);
                sql = sql.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;
                return sql;
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

    }
}
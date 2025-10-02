
using System;
using System.Collections.Generic;
using System.Linq;
using FundacaoFisk.SGF.Web.Services.Secretaria.Comum;
using Componentes.GenericModel;
using Componentes.GenericDataAccess;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Secretaria;
using System.Data.Entity;
using Componentes.Utils;
using Componentes.GenericDataAccess.GenericException;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess
{
    public class TipoContatoDataAccess : GenericRepository<TipoContato>, ITipoContatoDataAccess
    {
       

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public IEnumerable<TipoContato> GetTipoContatoSearch(SearchParameters parametros, string descricao, bool inicio, bool? ativo)
        {
            try{
                IEntitySorter<TipoContato> sorter = EntitySorter<TipoContato>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<TipoContato> sql;

                if (ativo == null)
                {
                    sql = from c in db.TipoContato.AsNoTracking()
                          select c;
                }
                else
                {
                    sql = from c in db.TipoContato.AsNoTracking()
                          where (c.id_tipo_contato_ativo == ativo)
                          select c;
                }
                sql = sorter.Sort(sql);
                var retorno = from c in sql
                              select c;
                if (!String.IsNullOrEmpty(descricao))
                    if (inicio)
                        retorno = from c in sql
                                  where c.dc_tipo_contato.StartsWith(descricao)
                                  select c;
                    else
                        retorno = from c in sql
                                  where c.dc_tipo_contato.Contains(descricao)
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
       

        public bool deleteAll(List<TipoContato> tiposContato) {
            try{
                string str = "";
                if(tiposContato != null && tiposContato.Count > 0)
                    foreach(TipoContato e in tiposContato)
                        str += e.cd_tipo_contato + ",";

                // Remove o último ponto e virgula:
                if(str.Length > 0)
                    str = str.Substring(0, str.Length - 1);

                int retorno = db.Database.ExecuteSqlCommand("delete from t_tipo_contato where cd_tipo_contato in(" + str + ")");

                return retorno > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}

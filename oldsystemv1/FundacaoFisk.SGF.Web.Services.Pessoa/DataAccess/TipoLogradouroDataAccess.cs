using System;
using System.Linq;
using System.Collections.Generic;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using Componentes.Utils;
using Componentes.GenericDataAccess;
using Componentes.GenericDataAccess.GenericRepository;
using System.Data.Entity;
using FundacaoFisk.SGF.Web.Services.Pessoa.Comum.DataAccess;
using Componentes.GenericDataAccess.GenericException;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Pessoa.DataAccess
{
    public class TipoLogradouroDataAccess : GenericRepository<TipoLogradouroSGF>, ITipoLogradouroDataAccess
    {
        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }
        public IEnumerable<TipoLogradouroSGF> GetAllTipoLogradouro()
        {
            try{
                var sql = from c in db.TipoLogradouroSGF
                          orderby c.no_tipo_logradouro
                          select c;
                return sql.ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<TipoLogradouroSGF> GetTipoLogradouroSearch(SearchParameters parametros, string descricao, bool inicio)
        {
            try{
                IEntitySorter<TipoLogradouroSGF> sorter = EntitySorter<TipoLogradouroSGF>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<TipoLogradouroSGF> sql;

                sql = from c in db.TipoLogradouroSGF.AsNoTracking()
                      select c;

                sql = sorter.Sort(sql);
                var retorno = from c in sql
                              select c;
                if (!String.IsNullOrEmpty(descricao))
                    if (inicio)
                        retorno = from c in sql
                                  where c.no_tipo_logradouro.StartsWith(descricao)
                                  select c;
                    else
                        retorno = from c in sql
                                  where c.no_tipo_logradouro.Contains(descricao)
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

        public IEnumerable<TipoLogradouroSGF> FindTipoLogradouro(string searchText)
        {
            try{
                var sql = from c in db.TipoLogradouroSGF
                          where c.no_tipo_logradouro.Contains(searchText)
                          select c;
                return sql.ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public TipoLogradouroSGF GetTipoLogradouroById(int idTipoLogradouro)
        {
            try{
                var sql = (from c in db.TipoLogradouroSGF
                           where
                             c.cd_tipo_logradouro == idTipoLogradouro
                           select c).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public TipoLogradouroSGF firstOrDefault()
        {
            try{
                var sql = (from c in db.TipoLogradouroSGF
                           select c).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public bool deleteAll(List<TipoLogradouroSGF> tiposLogradouro)
        {
            try{
                string strTipoLogradouro = "";
                if (tiposLogradouro != null && tiposLogradouro.Count > 0)
                    foreach (TipoLogradouroSGF e in tiposLogradouro)
                        strTipoLogradouro += e.cd_tipo_logradouro + ",";

                // Remove o último ponto e virgula:
                if (strTipoLogradouro.Length > 0)
                    strTipoLogradouro = strTipoLogradouro.Substring(0, strTipoLogradouro.Length - 1);

                int retorno = db.Database.ExecuteSqlCommand("delete from t_tipo_logradouro where cd_tipo_logradouro in(" + strTipoLogradouro + ")");

                return retorno > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public TipoLogradouroSGF findTipoLogradouroByNome(string no_tipo_logradouro)
        {
            try{
                var sql = (from c in db.TipoLogradouroSGF
                           where c.no_tipo_logradouro == no_tipo_logradouro
                           select c).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}

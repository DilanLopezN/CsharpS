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
    public class TipoTelefoneDataAccess : GenericRepository<TipoTelefoneSGF>, ITipoTelefoneDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }
        public IEnumerable<TipoTelefoneSGF> GetAllTipoTelefone()
        {
            try{
                var sql = from c in db.TipoTelefoneSGF
                          select c;
                return sql.ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<TipoTelefoneSGF> GetTipoTelefoneSearch(SearchParameters parametros, string descricao, bool inicio)
        {
            try{
                IEntitySorter<TipoTelefoneSGF> sorter = EntitySorter<TipoTelefoneSGF>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<TipoTelefoneSGF> sql;

                sql = from c in db.TipoTelefoneSGF.AsNoTracking()
                      select c;

                sql = sorter.Sort(sql);
                var retorno = from c in sql
                              select c;
                if (!String.IsNullOrEmpty(descricao))
                    if (inicio)
                        retorno = from c in sql
                                  where c.no_tipo_telefone.StartsWith(descricao)
                                  select c;
                    else
                        retorno = from c in sql
                                  where c.no_tipo_telefone.Contains(descricao)
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

        public IEnumerable<TipoTelefoneSGF> FindTipoTelefone(string searchText)
        {
            try{
                var sql = from c in db.TipoTelefoneSGF
                          where c.no_tipo_telefone.Contains(searchText)
                          select c;
                return sql.ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public TipoTelefoneSGF GetTipoTelefoneById(int idTipoTelefone)
        {
            try{
                var sql = (from c in db.TipoTelefoneSGF
                           where
                             c.cd_tipo_telefone == idTipoTelefone
                           select c).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public TipoTelefoneSGF firstOrDefault()
        {
            try{
                var sql = (from c in db.TipoTelefoneSGF
                           select c).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public bool deleteAll(List<TipoTelefoneSGF> tiposTelefone)
        {
            try{
                string strTipoTelefone = "";
                if (tiposTelefone != null && tiposTelefone.Count > 0)
                    foreach (TipoTelefoneSGF e in tiposTelefone)
                        strTipoTelefone += e.cd_tipo_telefone + ",";

                // Remove o último ponto e virgula:
                if (strTipoTelefone.Length > 0)
                    strTipoTelefone = strTipoTelefone.Substring(0, strTipoTelefone.Length - 1);

                int retorno = db.Database.ExecuteSqlCommand("delete from t_tipo_telefone where cd_tipo_telefone in(" + strTipoTelefone + ")");

                return retorno > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}

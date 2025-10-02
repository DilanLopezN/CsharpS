using System;
using System.Linq;
using System.Collections.Generic;
using FundacaoFisk.SGF.Web.Services.Pessoa.Comum;
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
    public class TipoEnderecoDataAccess : GenericRepository<TipoEnderecoSGF>, ITipoEnderecoDataAccess
    {

         // Propriedade privada de acesso do DataAcess
         private SGFWebContext db {
             get {
                 return (SGFWebContext) base.DB();
             }
         }

        public IEnumerable<TipoEnderecoSGF> GetAllTipoEndereco()
        {
            try{
                var sql = from c in db.TipoEnderecoSGF
                          select c;
                return sql.ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<TipoEnderecoSGF> GetTipoEnderecoSearch(SearchParameters parametros, string descricao, bool inicio)
        {
            try{
                IEntitySorter<TipoEnderecoSGF> sorter = EntitySorter<TipoEnderecoSGF>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<TipoEnderecoSGF> sql;

                sql = from c in db.TipoEnderecoSGF.AsNoTracking()
                      select c;

                sql = sorter.Sort(sql);
                var retorno = from c in sql
                              select c;
                if (!String.IsNullOrEmpty(descricao))
                    if (inicio)
                        retorno = from c in sql
                                  where c.no_tipo_endereco.StartsWith(descricao)
                                  select c;
                    else
                        retorno = from c in sql
                                  where c.no_tipo_endereco.Contains(descricao)
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

        public IEnumerable<TipoEnderecoSGF> FindTipoEndereco(string searchText)
        {
            try{
                var sql = from c in db.TipoEnderecoSGF
                          where c.no_tipo_endereco.Contains(searchText)
                          select c;
                return sql.ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public TipoEnderecoSGF GetTipoEnderecoById(int idTipoEndereco)
        {
            try{
                var sql = (from c in db.TipoEnderecoSGF
                           where
                             c.cd_tipo_endereco == idTipoEndereco
                           select c).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public TipoEnderecoSGF firstOrDefault()
        {
            try{
                var sql = (from c in db.TipoEnderecoSGF
                           select c).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public bool deleteAll(List<TipoEnderecoSGF> tiposEndereco)
        {
            try{
                string strTipoEndereco = "";
                if (tiposEndereco != null && tiposEndereco.Count > 0)
                    foreach (TipoEnderecoSGF e in tiposEndereco)
                        strTipoEndereco += e.cd_tipo_endereco + ",";

                // Remove o último ponto e virgula:
                if (strTipoEndereco.Length > 0)
                    strTipoEndereco = strTipoEndereco.Substring(0, strTipoEndereco.Length - 1);

                int retorno = db.Database.ExecuteSqlCommand("delete from t_tipo_endereco where cd_tipo_endereco in(" + strTipoEndereco + ")");

                return retorno > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}

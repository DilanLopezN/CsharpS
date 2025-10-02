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
    public class ClasseTelefoneDataAccess : GenericRepository<ClasseTelefoneSGF>, IClasseTelefoneDataAccess
    {
        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public IEnumerable<ClasseTelefoneSGF> GetAllClasseTelefone()
        {
            try{
                var sql = from c in db.ClasseTelefoneSGF
                          select c;
                return sql.ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<ClasseTelefoneSGF> GetClasseTelefoneSearch(SearchParameters parametros, string descricao, bool inicio)
        {
            try{
                IEntitySorter<ClasseTelefoneSGF> sorter = EntitySorter<ClasseTelefoneSGF>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<ClasseTelefoneSGF> sql;

                sql = from c in db.ClasseTelefoneSGF.AsNoTracking()
                      select c;

                sql = sorter.Sort(sql);
                var retorno = from c in sql
                              select c;
                if (!String.IsNullOrEmpty(descricao))
                    if (inicio)
                        retorno = from c in sql
                                  where c.dc_classe_telefone.StartsWith(descricao)
                                  select c;
                    else
                        retorno = from c in sql
                                  where c.dc_classe_telefone.Contains(descricao)
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

        public IEnumerable<ClasseTelefoneSGF> FindClasseTelefone(string searchText)
        {
            try{
                var sql = from c in db.ClasseTelefoneSGF
                          where c.dc_classe_telefone.Contains(searchText)
                          select c;
                return sql.ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public ClasseTelefoneSGF GetClasseTelefoneById(int idClasseTelefone)
        {
            try{
                var sql = (from c in db.ClasseTelefoneSGF
                           where
                             c.cd_classe_telefone == idClasseTelefone
                           select c).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public ClasseTelefoneSGF firstOrDefault()
        {
            try{
                var sql = (from c in db.ClasseTelefoneSGF
                           select c).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public bool deleteAll(List<ClasseTelefoneSGF> classesTelefoneSGF)
        {
            try{
                string strClasseTelefone = "";
                if (classesTelefoneSGF != null && classesTelefoneSGF.Count > 0)
                    foreach (ClasseTelefoneSGF e in classesTelefoneSGF)
                        strClasseTelefone += e.cd_classe_telefone + ",";

                // Remove o último ponto e virgula:
                if (strClasseTelefone.Length > 0)
                    strClasseTelefone = strClasseTelefone.Substring(0, strClasseTelefone.Length - 1);

                int retorno = db.Database.ExecuteSqlCommand("delete from t_classe_telefone where cd_classe_telefone in(" + strClasseTelefone + ")");

                return retorno > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

    }
}

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
    public class OperadoraDataAccess : GenericRepository<Operadora>, IOperadoraDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public IEnumerable<Operadora> GetAllOperadora()
        {
            try{
                var sql = from c in db.Operadora
                          select c;
                return sql.ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }


        public IEnumerable<Operadora> GetAllOperadorasAtivas(int? cd_operadora)
        {
            try{
                var sql = from c in db.Operadora
                          where c.id_operadora_ativa || (cd_operadora.HasValue && c.cd_operadora == cd_operadora.Value)
                          select c;
                return sql.ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Operadora> GetOperadoraSearch(SearchParameters parametros, string descricao, bool inicio, bool? ativa)
        {
            try{
                IEntitySorter<Operadora> sorter = EntitySorter<Operadora>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<Operadora> sql;

                if (ativa == null)
                {
                    sql = from c in db.Operadora.AsNoTracking()
                          select c;
                }
                else
                {
                    sql = from c in db.Operadora.AsNoTracking()
                          where (c.id_operadora_ativa == ativa)
                          select c;
                }     


                sql = sorter.Sort(sql);
                var retorno = from c in sql
                              select c;
                if (!String.IsNullOrEmpty(descricao))
                    if (inicio)
                        retorno = from c in sql
                                  where c.no_operadora.StartsWith(descricao)
                                  select c;
                    else
                        retorno = from c in sql
                                  where c.no_operadora.Contains(descricao)
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

        public IEnumerable<Operadora> FindOperadora(string searchText)
        {
            try{
                var sql = from c in db.Operadora
                          where c.no_operadora.Contains(searchText)
                          select c;
                return sql.ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Operadora GetOperadoraById(int idOperadora)
        {
            try{
                var sql = (from c in db.Operadora
                           where
                             c.cd_operadora == idOperadora
                           select c).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public Operadora firstOrDefault()
        {
            try{
                var sql = (from c in db.Operadora
                           select c).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public bool deleteAll(List<Operadora> operadoras)
        {
            try{
                string strOperadora = "";
                if (operadoras != null && operadoras.Count > 0)
                    foreach (Operadora e in operadoras)
                        strOperadora += e.cd_operadora + ",";

                // Remove o último ponto e virgula:
                if (strOperadora.Length > 0)
                    strOperadora = strOperadora.Substring(0, strOperadora.Length - 1);

                int retorno = db.Database.ExecuteSqlCommand("delete from t_operadora where cd_operadora in(" + strOperadora + ")");

                return retorno > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

    }
}

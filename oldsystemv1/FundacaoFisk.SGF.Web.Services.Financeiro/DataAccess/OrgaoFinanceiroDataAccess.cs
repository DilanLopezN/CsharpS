using System;
using System.Collections.Generic;
using System.Linq;
using Componentes.GenericDataAccess;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess
{
    public class OrgaoFinanceiroDataAccess : GenericRepository<OrgaoFinanceiro>, IOrgaoFinanceiroDataAccess
    {
        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }

        public IEnumerable<OrgaoFinanceiro> getOrgaoFinanceiroSearch(SearchParameters parametros, string descricao, bool inicio, bool? status)
        {
            try
            {
                IEntitySorter<OrgaoFinanceiro> sorter = EntitySorter<OrgaoFinanceiro>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<OrgaoFinanceiro> sql;

                if (status == null)
                {
                    sql = from c in db.OrgaoFinanceiro.AsNoTracking()
                        select c;
                }
                else
                {
                    byte statusOrgao = (byte) Convert.ToInt32(status);
                    sql = from c in db.OrgaoFinanceiro.AsNoTracking()
                        where c.id_orgao_ativo == statusOrgao
                          select c;
                }

                sql = sorter.Sort(sql);

                var retorno = from c in sql
                    select c;
                if (!String.IsNullOrEmpty(descricao))
                    if (inicio)
                        retorno = from c in sql
                            where c.dc_orgao_financeiro.StartsWith(descricao)
                            select c;
                    else
                        retorno = from c in sql
                            where c.dc_orgao_financeiro.Contains(descricao)
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

        public bool deleteAllOrgaoFinanceiro(List<OrgaoFinanceiro> orgaosFinanceiros)
        {
            try
            {
                string strOrgaoFinanceiro = "";
                if (orgaosFinanceiros != null && orgaosFinanceiros.Count > 0)
                {
                    foreach (OrgaoFinanceiro tipo in orgaosFinanceiros)
                    {
                        strOrgaoFinanceiro += tipo.cd_orgao_financeiro + ",";
                    }
                }
                if (strOrgaoFinanceiro.Length > 0)
                {
                    strOrgaoFinanceiro = strOrgaoFinanceiro.Substring(0, strOrgaoFinanceiro.Length - 1);
                }
                int retorno = db.Database.ExecuteSqlCommand("delete from t_orgao_financeiro where cd_orgao_financeiro in(" + strOrgaoFinanceiro + ")");

                return retorno > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<OrgaoFinanceiro> getOrgaoFinanceiro(bool? status)
        {
            try
            {
                IEnumerable<OrgaoFinanceiro> sql;

                if (status == null)
                {
                    sql = from c in db.OrgaoFinanceiro
                        select c;
                }
                else
                {
                    byte statusOrgao = (byte)Convert.ToInt32(status);
                    sql = from c in db.OrgaoFinanceiro
                        where (c.id_orgao_ativo == statusOrgao)
                        select c;
                }
                    

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<OrgaoFinanceiro> getAllOrgaoFinanceiro()
        {
            try
            {
                IEnumerable<OrgaoFinanceiro> sql;

                    sql = from c in db.OrgaoFinanceiro
                          select c;

                return sql.ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}
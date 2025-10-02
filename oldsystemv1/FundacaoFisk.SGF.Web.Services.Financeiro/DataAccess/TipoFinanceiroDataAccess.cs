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
    public class TipoFinanceiroDataAccess : GenericRepository<TipoFinanceiro>, ITipoFinanceiroDataAccess
    {
        public enum TipoConsultaTipoFinanEnum
        {
            HAS_ATIVO = 0
        }


        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public IEnumerable<TipoFinanceiro> GetTipoFinanceiroSearch(SearchParameters parametros, string descricao, bool inicio, bool? ativo)
        {
            try{
                IEntitySorter<TipoFinanceiro> sorter = EntitySorter<TipoFinanceiro>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<TipoFinanceiro> sql;

                if (ativo == null)
                {
                    sql = from c in db.TipoFinanceiro.AsNoTracking()
                          select c;
                }
                else
                {
                    sql = from c in db.TipoFinanceiro.AsNoTracking()
                          where c.id_tipo_financeiro_ativo == ativo
                          select c;
                }

                sql = sorter.Sort(sql);

                var retorno = from c in sql
                              select c;
                
                if (!String.IsNullOrEmpty(descricao))
                    if (inicio)
                        retorno = from c in sql
                                  where c.dc_tipo_financeiro.StartsWith(descricao)
                                  select c;
                    else
                        retorno = from c in sql
                                  where c.dc_tipo_financeiro.Contains(descricao)
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

        public bool deleteAllTipoFinanceiro(List<TipoFinanceiro> tiposFinanceiros)
        {
            try{
                string strTipoFinanceiro = "";
                if (tiposFinanceiros != null && tiposFinanceiros.Count > 0 )
                {
                    foreach (TipoFinanceiro tipo in tiposFinanceiros)
                    {
                        strTipoFinanceiro += tipo.cd_tipo_financeiro + ",";
                    }
                }
                if (strTipoFinanceiro.Length > 0)
                {
                    strTipoFinanceiro = strTipoFinanceiro.Substring(0, strTipoFinanceiro.Length - 1);
                }
                int retorno = db.Database.ExecuteSqlCommand("delete from t_tipo_financeiro where cd_tipo_financeiro in(" + strTipoFinanceiro + ")");
                
                return retorno > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public List<TipoFinanceiro> getTipoFinanceiroAtivo()
        {
            try
            {
                var sql = from tf in db.TipoFinanceiro
                          where tf.id_tipo_financeiro_ativo == true
                          orderby tf.dc_tipo_financeiro
                          select tf;
                return sql.ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }

        }

        public IEnumerable<TipoFinanceiro> getTipoFinanceiro(int cd_tipo_finan, TipoConsultaTipoFinanEnum tipoConsulta)
        {
            try
            {
                IEnumerable<TipoFinanceiro> sql = null;
                switch (tipoConsulta)
                {
                    case TipoConsultaTipoFinanEnum.HAS_ATIVO:
                        sql = from tf in db.TipoFinanceiro
                              where tf.id_tipo_financeiro_ativo == true || (cd_tipo_finan == 0 || tf.cd_tipo_financeiro == cd_tipo_finan)
                              orderby tf.dc_tipo_financeiro
                              select tf;
                        break;
                }
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<TipoFinanceiro> getTipoFinanceiroMovimento(int cd_tipo_finan, int cd_empresa, int id_tipo_movto)
        {
            try
            {
                IEnumerable<TipoFinanceiro> sql = null;
                sql = from tf in db.TipoFinanceiro
                      where tf.id_tipo_financeiro_ativo == true || (cd_tipo_finan == 0 || tf.cd_tipo_financeiro == cd_tipo_finan)
                      && tf.Movimentos.Where(m => m.id_tipo_movimento == id_tipo_movto && (cd_empresa == 0 || m.cd_pessoa_empresa == cd_empresa)).Any()
                      orderby tf.dc_tipo_financeiro
                      select tf;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
      

    }
}

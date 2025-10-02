using System;
using System.Collections.Generic;
using System.Linq;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum;
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
    public class MovimentacaoFinanceiraDataAccess : GenericRepository<MovimentacaoFinanceira>, IMovimentacaoFinanceiraDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public IEnumerable<MovimentacaoFinanceira> GetMovimentacaoFinanceiraSearch(SearchParameters parametros, string descricao, bool inicio, bool? ativo)
        {
            try{
                IEntitySorter<MovimentacaoFinanceira> sorter = EntitySorter<MovimentacaoFinanceira>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<MovimentacaoFinanceira> sql;

                if (ativo == null)
                {
                    sql = from c in db.MovimentacaoFinanceira.AsNoTracking()
                          select c;
                }
                else
                {
                    sql = from c in db.MovimentacaoFinanceira.AsNoTracking()
                          where c.id_mov_financeira_ativa == ativo
                          select c;
                }
                sql = sorter.Sort(sql);
                var retorno = from c in sql
                              select c;
                if (!String.IsNullOrEmpty(descricao))
                    if (inicio)
                        retorno = from c in sql
                                  where c.dc_movimentacao_financeira.StartsWith(descricao)
                                  select c;
                    else
                        retorno = from c in sql
                                  where c.dc_movimentacao_financeira.Contains(descricao)
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
        

        public bool deleteAllMovimentacao(List<MovimentacaoFinanceira> movimentacoes)
        {
            try{
                string strMovimentacao = "";
                if (movimentacoes != null && movimentacoes.Count > 0)
                {
                    foreach ( MovimentacaoFinanceira movimentacao in  movimentacoes)
                    {
                        strMovimentacao += movimentacao.cd_movimentacao_financeira + ",";
                    }
                }

                if (strMovimentacao.Length > 0)
                {
                    strMovimentacao = strMovimentacao.Substring(0, strMovimentacao.Length - 1); 
                }
                int retorno = db.Database.ExecuteSqlCommand("delete from t_movimentacao_financeira where cd_movimentacao_financeira in (" + strMovimentacao + " )");

                return retorno > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<MovimentacaoFinanceira> getMovimentacaoWithContaCorrente(int cd_pessoa_escola) { 
           IQueryable<MovimentacaoFinanceira> sql;
           try
           {
               sql = from movimentacao in db.MovimentacaoFinanceira
                     where movimentacao.ContaCorrente.Any(m => m.cd_movimentacao_financeira == movimentacao.cd_movimentacao_financeira 
                                                               && m.cd_pessoa_empresa == cd_pessoa_escola)
                     select movimentacao;

               return sql.OrderBy(x => x.dc_movimentacao_financeira);
           }
           catch (Exception exe)
           {
               throw new DataAccessException(exe);
           }
        }

        public IEnumerable<MovimentacaoFinanceira> getMovimentacaoAtivaWithConta(int cd_pessoa_escola, bool isCadastro)
        {
            try
            {
                IQueryable<MovimentacaoFinanceira> sql;

                if (!isCadastro)
                    sql = from movimentacao in db.MovimentacaoFinanceira
                          where movimentacao.ContaCorrente.Any(m => m.cd_movimentacao_financeira == movimentacao.cd_movimentacao_financeira
                                                                    && m.cd_pessoa_empresa == cd_pessoa_escola)
                                                           
                          select movimentacao;
                else
                    sql = from movimentacao in db.MovimentacaoFinanceira
                          where (movimentacao.id_mov_financeira_ativa == true)
                          select movimentacao;

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<MovimentacaoFinanceira> getMovimentacaoAtivaWithConta(int cd_pessoa_escola, int cd_movimentacao_financeira)
        {
            try
            {
                IQueryable<MovimentacaoFinanceira> sql;

                sql = from movimentacao in db.MovimentacaoFinanceira
                      where (movimentacao.ContaCorrente.Any(m => m.cd_movimentacao_financeira == movimentacao.cd_movimentacao_financeira
                                                                && m.cd_pessoa_empresa == cd_pessoa_escola))
                                                                || ((movimentacao.id_mov_financeira_ativa == true)
                                                                || (movimentacao.cd_movimentacao_financeira == cd_movimentacao_financeira))
                      select movimentacao;

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<MovimentacaoFinanceira> getMovimentacaoTransferencia(int cd_pessoa_escola, int cd_movimentacao_financeira)
        {
            try
            {
                IQueryable<MovimentacaoFinanceira> sql;
                if (cd_movimentacao_financeira == 0)
                    sql = from movimentacao in db.MovimentacaoFinanceira
                          where (movimentacao.id_mov_financeira_ativa == true)
                                 && (movimentacao.cd_movimentacao_financeira == (int)(MovimentacaoFinanceira.TipoMovimentacaoFinanceira.TRANSFERENCIA))
                          select movimentacao;
                else
                    sql = from movimentacao in db.MovimentacaoFinanceira
                          where ((movimentacao.cd_movimentacao_financeira == (int)(MovimentacaoFinanceira.TipoMovimentacaoFinanceira.TRANSFERENCIA)))
                                && (movimentacao.ContaCorrente.Any(m => m.cd_movimentacao_financeira == movimentacao.cd_movimentacao_financeira
                                                                     && m.cd_pessoa_empresa == cd_pessoa_escola))
                                                                     && ((movimentacao.id_mov_financeira_ativa == true)
                                                                      || (movimentacao.cd_movimentacao_financeira == cd_movimentacao_financeira))
                          select movimentacao;

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

    }
}

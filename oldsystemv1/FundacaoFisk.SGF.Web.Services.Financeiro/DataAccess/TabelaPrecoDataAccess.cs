using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using Componentes.GenericModel;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using Componentes.GenericDataAccess;
using FundacaoFisk.SGF.Web.Services.Financeiro;
using FundacaoFisk.SGF.Web.Services.Financeiro.Model;
using System.Data.Entity;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess;
using System.Data.SqlClient;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess
{
    using FundacaoFisk.SGF.GenericModel;
    using Componentes.GenericDataAccess.GenericException;
    using System.Data.Entity.Core.Objects;
    public class TabelaPrecoDataAccess : GenericRepository<TabelaPreco>, ITabelaPrecoDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public IEnumerable<TabelaPrecoUI> GetTabelaPrecoSearch(SearchParameters parametros, int cdCurso, int cdDuracao, int cdRegime, DateTime? dtaCad, int cdEscola, int cdProduto)
        {
            try{
                IEntitySorter<TabelaPrecoUI> sorter = EntitySorter<TabelaPrecoUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<TabelaPrecoUI> sql;
                IQueryable<TabelaPreco> retorno;
                if(dtaCad == null)
                    retorno = from c in db.TabelaPreco.AsNoTracking()
                                  where c.cd_pessoa_escola == cdEscola &&
                                       ( (from tp in db.TabelaPreco
                                         where tp.cd_curso == c.cd_curso 
                                               && tp.cd_duracao == c.cd_duracao
                                               && (tp.cd_regime == c.cd_regime ||  tp.cd_regime == null)
                                               && tp.cd_pessoa_escola == c.cd_pessoa_escola
                                         orderby tp.dta_tabela_preco descending
                                         select tp.cd_tabela_preco).FirstOrDefault() == c.cd_tabela_preco )
                                  select c;
                else
                    retorno = from c in db.TabelaPreco.AsNoTracking()
                                  where c.cd_pessoa_escola == cdEscola &&
                                        (from tp in db.TabelaPreco
                                         where 
                                            tp.cd_curso == c.cd_curso && 
                                            tp.cd_duracao == c.cd_duracao && 
                                            (tp.cd_regime == c.cd_regime ||  tp.cd_regime == null) &&
                                            tp.cd_pessoa_escola == c.cd_pessoa_escola &&
                                            tp.dta_tabela_preco <= dtaCad
                                         orderby tp.dta_tabela_preco descending
                                         select tp.cd_tabela_preco).FirstOrDefault() == c.cd_tabela_preco
                                  select c;


                //Pegando apenas Tabela com os Produtos Escolhido na Pesquisa
                if (cdProduto != 0)
                    retorno = from c in retorno
                              join curso in db.Curso
                              on c.cd_curso equals curso.cd_curso
                              where curso.cd_produto == cdProduto
                              select c;

                //Pegando apenas Tabela com o Curso Escolhido na Pesquisa
                if (cdCurso != 0)
                    retorno = from c in retorno
                              where c.CursoTabelaPreco.cd_curso == cdCurso
                              select c;
                //Pegando apenas Tabela com a Duração Escolhida na Pesquisa
                if (cdDuracao != 0)
                    retorno = from c in retorno
                              where c.DuracaoTabelaPreco.cd_duracao == cdDuracao
                              select c;
                //Pegando apenas Tabela com a Regime Escolhida na Pesquisa
                if (cdRegime != 0)
                    retorno = from c in retorno
                              where c.RegimeTabelaPreco.cd_regime == cdRegime
                              select c;                
                
                sql = from c in retorno
                       orderby c.CursoTabelaPreco.Estagio.nm_ordem_estagio descending
                      select new TabelaPrecoUI
                      {
                          cd_tabela_preco = c.cd_tabela_preco,
                          cd_curso = c.cd_curso,
                          no_curso = c.CursoTabelaPreco.no_curso,
                          cd_duracao = c.cd_duracao,
                          dc_duracao = c.DuracaoTabelaPreco.dc_duracao,
                          cd_regime = c.cd_regime,
                          no_regime = c.RegimeTabelaPreco.no_regime,
                          nm_parcelas = c.nm_parcelas,
                          vl_parcela = c.vl_parcela,
                          no_produto = c.CursoTabelaPreco.Produto.no_produto,
                          vl_matricula = c.vl_matricula,
                          dta_tabela_preco = c.dta_tabela_preco,
                          vl_aula = c.vl_aula,
                          ordem = c.CursoTabelaPreco.Estagio.nm_ordem_estagio
                      };
                sql = sorter.Sort(sql);

                int limite = sql.Count();

                parametros.ajustaParametrosPesquisa(limite);
                sql = sql.Skip(parametros.from).Take(parametros.qtd_limite);

                parametros.qtd_limite = limite;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public TabelaPreco GetTabelaById(int idTabelaPreco, int cdEscola)
        {
            try
            {
                var sql = (from c in db.TabelaPreco
                           where c.cd_tabela_preco == idTabelaPreco && c.cd_pessoa_escola == cdEscola
                           select c).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public TabelaPrecoUI GetTabelaPrecoById(int idTabelaPreco, int cdEscola)
        {
            try{
                var sql = (from c in db.TabelaPreco
                           where c.cd_tabela_preco == idTabelaPreco && c.cd_pessoa_escola == cdEscola
                           select new TabelaPrecoUI
                           {
                               cd_tabela_preco = c.cd_tabela_preco,
                               cd_curso = c.cd_curso,
                               no_curso = c.CursoTabelaPreco.no_curso,
                               cd_duracao = c.cd_duracao,
                               dc_duracao = c.DuracaoTabelaPreco.dc_duracao,
                               no_produto = c.CursoTabelaPreco.Produto.no_produto,
                               cd_regime = c.cd_regime,
                               no_regime = c.RegimeTabelaPreco.no_regime,
                               nm_parcelas = c.nm_parcelas,
                               vl_parcela = c.vl_parcela,
                               vl_matricula = c.vl_matricula,
                               dta_tabela_preco = c.dta_tabela_preco,
                               vl_aula = c.vl_aula

                           }).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public IEnumerable<TabelaPrecoUI> GetHistoricoTabelaPreco(SearchParameters parametros, int cdCurso, int cdDuracao, int cdRegime, int cdEscola)
        {
            try{
                IEntitySorter<TabelaPrecoUI> sorter = EntitySorter<TabelaPrecoUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                int? cdReg = null;
                if(cdRegime > 0)
                    cdReg = cdRegime;
                IQueryable<TabelaPrecoUI> sql;

                if (cdReg == null)
                    sql = from c in db.TabelaPreco.AsNoTracking()
                          where c.cd_curso == cdCurso &&
                                c.cd_duracao == cdDuracao &&
                                c.cd_regime == null &&
                                c.cd_pessoa_escola == cdEscola
                          orderby c.dta_tabela_preco descending
                          select new TabelaPrecoUI
                          {
                              cd_tabela_preco = c.cd_tabela_preco,
                              cd_curso = c.cd_curso,
                              no_curso = c.CursoTabelaPreco.no_curso,
                              cd_duracao = c.cd_duracao,
                              dc_duracao = c.DuracaoTabelaPreco.dc_duracao,
                              cd_regime = c.cd_regime,
                              no_regime = c.RegimeTabelaPreco.no_regime,
                              no_produto = c.CursoTabelaPreco.Produto.no_produto,
                              nm_parcelas = c.nm_parcelas,
                              vl_parcela = c.vl_parcela,
                              vl_matricula = c.vl_matricula,
                              dta_tabela_preco = c.dta_tabela_preco,
                              vl_aula = c.vl_aula
                          };
                else
                    sql = from c in db.TabelaPreco.AsNoTracking()
                          orderby c.dta_tabela_preco descending
                          where c.cd_curso == cdCurso &&
                                c.cd_duracao == cdDuracao &&
                                c.cd_regime == cdReg &&
                                c.cd_pessoa_escola == cdEscola
                          select new TabelaPrecoUI
                           {
                               cd_tabela_preco = c.cd_tabela_preco,
                               cd_curso = c.cd_curso,
                               no_curso = c.CursoTabelaPreco.no_curso,
                               cd_duracao = c.cd_duracao,
                               dc_duracao = c.DuracaoTabelaPreco.dc_duracao,
                               cd_regime = c.cd_regime,
                               no_regime = c.RegimeTabelaPreco.no_regime,
                               no_produto = c.CursoTabelaPreco.Produto.no_produto,
                               nm_parcelas = c.nm_parcelas,
                               vl_parcela = c.vl_parcela,
                               vl_matricula = c.vl_matricula,
                               dta_tabela_preco = c.dta_tabela_preco,
                               vl_aula = c.vl_aula
                           };
                sql = sorter.Sort(sql);

                int limite = sql.Count();

                parametros.ajustaParametrosPesquisa(limite);
                sql = sql.Skip(parametros.from).Take(parametros.qtd_limite);

                parametros.qtd_limite = limite;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

       
        public bool deleteAllTabelaPreco(List<TabelaPreco> tabelas)
        {
            try{
                string strTabelaPreco = "";
                if (tabelas != null && tabelas.Count > 0)
                    foreach (TabelaPreco e in tabelas)
                        strTabelaPreco += e.cd_tabela_preco + ",";

                // Remove o último ponto e virgula:
                if (strTabelaPreco.Length > 0)
                    strTabelaPreco = strTabelaPreco.Substring(0, strTabelaPreco.Length - 1);

                int retorno = db.Database.ExecuteSqlCommand("delete from t_tabela_preco where cd_tabela_preco in(" + strTabelaPreco + ")");

                return retorno > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public int? getNroParcelas(int cd_escola, int cd_curso, int cd_regime, int cd_duracao, DateTime data_matricula) {
            try {
                return (from tb in db.TabelaPreco
                        where tb.cd_curso == cd_curso
                              && tb.cd_regime == cd_regime
                              && tb.cd_duracao == cd_duracao
                              && tb.cd_pessoa_escola == cd_escola
                              && DbFunctions.TruncateTime(tb.dta_tabela_preco) <= data_matricula.Date
                        orderby tb.dta_tabela_preco descending
                        select tb.nm_parcelas).FirstOrDefault();

            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

        public TabelaPreco getValoresForMatricula(int cd_pessoa_escola, int cd_curso, int cd_duracao, int cd_regime, DateTime dta_matricula)
        {
         
            try
            {
                IQueryable<TabelaPreco> sql;
               
                DateTime dta;
                // filtra os registro com regime ou sem regime
                if (cd_regime > 0)
                    sql = from tabela in db.TabelaPreco
                          where tabela.cd_regime == cd_regime
                              && tabela.cd_curso == cd_curso
                              && tabela.cd_duracao == cd_duracao
                              && tabela.cd_pessoa_escola == cd_pessoa_escola
                           select tabela;
                else
                    sql = from tabela in db.TabelaPreco
                          where tabela.cd_regime == null
                              && tabela.cd_curso == cd_curso
                              && tabela.cd_duracao == cd_duracao
                              && tabela.cd_pessoa_escola == cd_pessoa_escola
                           select tabela;
               
                // verifica se exite data menor ou igual a passada pelo usuário
                sql = from s in sql
                      where s.dta_tabela_preco <= dta_matricula.Date
                      select s;

                if (sql.Any())
                {
                    dta = (from tabela in sql
                           where DbFunctions.TruncateTime(tabela.dta_tabela_preco) <= dta_matricula.Date
                           select tabela.dta_tabela_preco).Max();

                    sql = from tabelaPreco in sql
                          where tabelaPreco.cd_curso == cd_curso
                              && tabelaPreco.cd_duracao == cd_duracao
                              && DbFunctions.TruncateTime(tabelaPreco.dta_tabela_preco) == dta.Date
                          select tabelaPreco;
                }
                
                return sql.FirstOrDefault();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}
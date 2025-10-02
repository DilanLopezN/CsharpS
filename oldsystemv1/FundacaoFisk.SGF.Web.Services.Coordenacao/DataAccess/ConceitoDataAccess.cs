using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess
{
    public class ConceitoDataAccess : GenericRepository<Conceito>, IConceitoDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }

        public IEnumerable<ConceitoSearchUI> getConceitoDesc(SearchParameters parametros, string desc, bool inicio, bool? ativo, int CodP)
        {
            try
            {
                IEntitySorter<ConceitoSearchUI> sorter = EntitySorter<ConceitoSearchUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<ConceitoSearchUI> sql;

                sql = from conceito in db.Conceito.AsNoTracking()
                      where (CodP == 0 || conceito.cd_produto == CodP)
                      select new ConceitoSearchUI
                      {
                          cd_conceito = conceito.cd_conceito,
                          no_conceito = conceito.no_conceito,
                          id_conceito_ativo = conceito.id_conceito_ativo,
                          pc_inicial_conceito = conceito.pc_inicial_conceito,
                          pc_final_conceito = conceito.pc_final_conceito,
                          cd_produto = conceito.cd_produto,
                          no_produto = conceito.Produto.no_produto,
                          vl_nota_participacao = conceito.vl_nota_participacao
                      };


                if (ativo == null)
                {
                    sql = from conceito in sql
                          select conceito;
                }
                else
                {
                    sql = from conceito in sql
                          where conceito.id_conceito_ativo == ativo
                          select conceito;
                }
                sql = sorter.Sort(sql);
                var retorno = from conceito in sql
                              select conceito;
                if (!String.IsNullOrEmpty(desc))
                {
                    if (inicio)
                    {
                        retorno = from conceito in sql
                                  where conceito.no_conceito.StartsWith(desc)
                                  select conceito;
                    }//end if
                    else
                    {
                        retorno = from conceito in sql
                                  where conceito.no_conceito.Contains(desc)
                                  select conceito;
                    }//end else

                }

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

        public bool deleteAllConceito(List<Conceito> conceitos)
        {
            try
            {
                string strConceito = "";
                if (conceitos != null && conceitos.Count > 0)
                    foreach (Conceito e in conceitos)
                        strConceito += e.cd_conceito + ",";

                // Remove o último ponto e virgula:
                if (strConceito.Length > 0)
                    strConceito = strConceito.Substring(0, strConceito.Length - 1);

                int retorno = db.Database.ExecuteSqlCommand("delete from t_conceito where cd_conceito in(" + strConceito + ")");

                return retorno > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public List<Conceito> getConceitosDisponiveisByProdutoTurma(int cd_turma)
        {
            try
            {
                var sql = (from conceito in db.Conceito
                           where conceito.Produto.Turma.Where(t => t.cd_turma == cd_turma).Any()
                                && conceito.id_conceito_ativo == true
                                && conceito.vl_nota_participacao > 0
                           orderby conceito.vl_nota_participacao descending
                           select new
                           {
                               cd_conceito = conceito.cd_conceito,
                               no_conceito = conceito.no_conceito,
                               vl_nota_participacao = conceito.vl_nota_participacao
                           }).ToList().Select(c => new Conceito
                           {
                               cd_conceito = c.cd_conceito,
                               no_conceito = c.no_conceito,
                               vl_nota_participacao = c.vl_nota_participacao
                           });
                return sql.ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Conceito> findConceitosAtivos(int idProduto, int idConceito)
        {
            try
            {
                var sql = (from conceito in db.Conceito
                           where (idProduto == 0 || conceito.cd_produto == idProduto)
                                && ((conceito.id_conceito_ativo == true)
                                || (idConceito == 0 || conceito.cd_conceito == idConceito))
                           select new
                           {
                               cd_conceito = conceito.cd_conceito,
                               no_conceito = conceito.no_conceito,
                               pc_inicial_conceito = conceito.pc_inicial_conceito,
                               pc_final_conceito = conceito.pc_final_conceito,
                           }).ToList().Select(c => new Conceito
                                   {
                                       cd_conceito = c.cd_conceito,
                                       no_conceito = c.no_conceito + " | " + c.pc_inicial_conceito + " à " + c.pc_final_conceito,
                                       pc_final_conceito = c.pc_final_conceito,
                                       pc_inicial_conceito = c.pc_inicial_conceito
                                   });
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public double somaParticipacaoPorConceito(int idProduto, int? cdConceito)
        {
            try
            {
                var sql = (from conceito in db.Conceito
                           where (idProduto == 0 || conceito.cd_produto == idProduto)
                                && conceito.id_conceito_ativo == true
                                && (!cdConceito.HasValue || conceito.cd_conceito != cdConceito)
                           orderby conceito.vl_nota_participacao descending
                           select conceito.vl_nota_participacao).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}

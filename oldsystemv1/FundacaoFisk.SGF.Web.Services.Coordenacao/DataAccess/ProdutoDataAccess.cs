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

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess
{
    public class ProdutoDataAccess : GenericRepository<Produto>, IProdutoDataAccess
    {
        public enum TipoConsultaProdutoEnum
        {
            HAS_ATIVO=0,
            HAS_CONCEITO=1,
            HAS_HISTORICO=2,
            HAS_ESTAGIO=3,
            HAS_CURSO=4,
            HAS_TABELA_PRECO = 5,
            HAS_CAD_ESTAGIO = 6,
            HAS_TURMA = 7,
            HAS_AVAL_CURSO = 8,
            HAS_ATIVO_CURSO = 9,
            HAS_PROSPECT = 10,
            HAS_AULA_PERSONALIZADA = 11,
            HAS_AVALIACAO_PARTICIPACAO = 12,
            HAS_DESISTENCIA = 13
        }
        
        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public IEnumerable<Produto> getProdutoDesc(SearchParameters parametros, string desc, string abrev, bool inicio, bool? ativo)
        {
            try{
                IQueryable<Produto> sql;
                IEntitySorter<Produto> sorter = EntitySorter<Produto>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);

                if (ativo == null)
                {
                    sql = from produto in db.Produto.AsNoTracking()
                          orderby produto.no_produto
                          select produto;
                }
                else
                {
                    sql = from produto in db.Produto.AsNoTracking()
                          where produto.id_produto_ativo == ativo
                          orderby produto.no_produto
                          select produto;
                }
                sql = sorter.Sort(sql);
                var retorno = from produto in sql
                              select produto;
                if (!String.IsNullOrEmpty(desc))
                {
                    if (inicio)
                    {
                        retorno = from produto in sql
                                  where produto.no_produto.StartsWith(desc)
                                  select produto;
                    }//end if
                    else
                    {
                        retorno = from produto in sql
                                  where produto.no_produto.Contains(desc)
                                  select produto;
                    }//end else
    
                }
                if (!String.IsNullOrEmpty(abrev))
                {
                    if (inicio)
                    {
                        retorno = from produto in retorno
                                  where produto.no_produto_abreviado.StartsWith(abrev)
                                  select produto;
                    }//end if
                    else
                    {
                        retorno = from produto in retorno
                                  where produto.no_produto_abreviado.Contains(abrev)
                                  select produto;
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

        public IEnumerable<Produto> findProdutoAulaPersonalizada(int cd_aluno, int cd_escola)
        {
            try
            {
                IQueryable<Produto> retorno = from p in db.Produto
                                  where p.AulaPersonalizada.Where(ap => ap.AulaPersonalizadaAlunos.Where(apa => apa.cd_aluno == cd_aluno).Any() && ap.cd_escola == cd_escola).Any()
                                  orderby p.no_produto
                                  select p;

                return retorno.ToList<Produto>();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Produto> findProduto(TipoConsultaProdutoEnum hasDependente, int? cd_produto,int? cd_escola)
        {
            try{
                IQueryable<Produto> retorno = null;
                switch (hasDependente)
                {
                    case TipoConsultaProdutoEnum.HAS_ATIVO:
                        retorno = from produto in db.Produto
                                  where produto.id_produto_ativo == true || (cd_produto.HasValue && produto.cd_produto == cd_produto)
                                  orderby produto.no_produto
                                  select produto;
                        break;
                    case TipoConsultaProdutoEnum.HAS_CONCEITO:
                        retorno = from produto in db.Produto
                                  where (produto.id_produto_ativo == true || (cd_produto.HasValue && produto.cd_produto == cd_produto))
                                        && (produto.Conceito.Any())
                                  orderby produto.no_produto
                                  select produto;
                        break;
                    case TipoConsultaProdutoEnum.HAS_ESTAGIO:
                        retorno = from produto in db.Produto
                                  where (produto.id_produto_ativo == true || (cd_produto.HasValue && produto.cd_produto == cd_produto))
                                        && (produto.Estagio.Any())
                                  orderby produto.no_produto
                                  select produto;
                        break;
                    case TipoConsultaProdutoEnum.HAS_CAD_ESTAGIO:
                        retorno = from produto in db.Produto
                                  where produto.id_produto_ativo == true ||
                                  (produto.Estagio.Any())
                                  orderby produto.no_produto
                                  select produto;
                        break;
                    case TipoConsultaProdutoEnum.HAS_CURSO:
                        retorno = from produto in db.Produto
                                  where produto.Cursos.Any()
                                  orderby produto.no_produto
                                  select produto;
                        break;
                    case TipoConsultaProdutoEnum.HAS_TABELA_PRECO:
                        retorno = from p in db.TabelaPreco
                                  join c in db.Curso
                                  on p.cd_curso equals c.cd_curso
                                  join produto in db.Produto
                                  on c.cd_produto equals produto.cd_produto
                                  orderby produto.no_produto
                                  select produto;
                        break;
                    case TipoConsultaProdutoEnum.HAS_TURMA:
                        retorno = from produto in db.Produto
                                   where produto.Turma.Where(x => x.cd_pessoa_escola == (int)cd_escola).Any()
                                  orderby produto.no_produto
                                  select produto;
                        break;
                    case TipoConsultaProdutoEnum.HAS_AVAL_CURSO:
                        retorno = (from produto in db.Produto
                                  join curso in db.Curso
                                  on produto.cd_produto equals curso.cd_produto
                                  where curso.AvaliacaoCurso.Any()
                                  orderby produto.no_produto
                                  select produto).Distinct();
                        break;
                    case TipoConsultaProdutoEnum.HAS_ATIVO_CURSO:
                        retorno = from produto in db.Produto
                                  where produto.Cursos.Any() && produto.id_produto_ativo == true
                                  orderby produto.no_produto
                                  select produto;
                        break;
                    case TipoConsultaProdutoEnum.HAS_PROSPECT:
                         retorno = from produto in db.Produto
                                   where produto.ProdutoProspect.Where(pp => pp.Prospect.cd_pessoa_escola == cd_escola.Value).Any()
                                  orderby produto.no_produto
                                  select produto;
                        break;
                    case TipoConsultaProdutoEnum.HAS_AULA_PERSONALIZADA:
                        retorno = from produto in db.Produto
                                  where produto.AulaPersonalizada.Where(a => a.cd_escola == cd_escola.Value).Any()
                                  orderby produto.no_produto
                                  select produto;
                        break;
                    case TipoConsultaProdutoEnum.HAS_AVALIACAO_PARTICIPACAO:
                        retorno = from produto in db.Produto
                                  where produto.AvaliacaoParticipacao.Where(a => a.AvaliacaoParticipacaoVinc.Where(av => av.cd_escola == cd_escola.Value).Any()).Any()
                                  orderby produto.no_produto
                                  select produto;
                        break;
                    case TipoConsultaProdutoEnum.HAS_DESISTENCIA:
                        retorno = from produto in db.Produto
                                  where db.AlunoTurma.Any(x => x.Turma.cd_pessoa_escola == cd_escola.Value && x.Turma.cd_produto == produto.cd_produto && x.Desistencia.Any())
                                  orderby produto.no_produto
                                  select produto;
                        break;  

                }
            return retorno.ToList<Produto>();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Produto> findProdutoTabela(int cdEscola)
        {
            try{
                IQueryable<Produto> retorno = null;
                        retorno = (from p in db.TabelaPreco
                                  join c in db.Curso
                                  on p.cd_curso equals c.cd_curso
                                  join produto in db.Produto
                                  on c.cd_produto equals produto.cd_produto
                                  where p.cd_pessoa_escola == cdEscola
                                  select produto).Distinct().OrderBy(p => p.no_produto);
                return retorno.ToList<Produto>();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool deleteAllProduto(List<Produto> produtos)
        {
            try{
                string strProduto = "";
                if (produtos != null && produtos.Count > 0)
                    foreach (Produto e in produtos)
                        strProduto += e.cd_produto + ",";

                // Remove o último ponto e virgula:
                if (strProduto.Length > 0)
                    strProduto = strProduto.Substring(0, strProduto.Length - 1);

                int retorno = db.Database.ExecuteSqlCommand("delete from t_produto where cd_produto in(" + strProduto + ")");

                return retorno > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Produto> getProdutosWithAtividadeExtra(int cd_pessoa_escola, bool isAtivo)
        {
            try
            {
                var retorno = (from produto in db.Produto
                              // join curso in db.Curso on produto.cd_produto equals curso.cd_produto
                              join atividadeExtra in db.AtividadeExtra on produto.cd_produto equals atividadeExtra.cd_produto
                              where atividadeExtra.cd_pessoa_escola == cd_pessoa_escola 
                              orderby produto.no_produto
                              select produto).Distinct();
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}

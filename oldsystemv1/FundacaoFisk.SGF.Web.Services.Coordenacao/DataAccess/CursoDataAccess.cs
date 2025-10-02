using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using System.Data.Entity;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;
using Componentes.GenericDataAccess;
using Componentes.GenericDataAccess.GenericException;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Business;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess
{
    public class CursoDataAccess : GenericRepository<Curso>, ICursoDataAccess
    {
        public enum TipoConsultaCursoEnum
        {
            HAS_ATIVO = 0,
            HAS_TURMA = 1,
            HAS_PRODUTO = 2,
            HAS_AVALIACAO_CURSO = 3,
            HAS_ATIVOPROD = 4,
            HAS_DESISTENCIA = 5
        }

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }

        public IEnumerable<Curso> getCursoSearch(SearchParameters parametros, string desc, bool inicio, bool? ativo, int? produto, int? estagio, int? modalidade, int? nivel, DateTime? dt_inicial, DateTime? dt_final)
        {
            try
            {
                IEntitySorter<CursoSearch> sorter = EntitySorter<CursoSearch>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<Curso> sql;

                sql = from curso in db.Curso.AsNoTracking()
                      select curso;

                if (ativo != null)
                    sql = from curso in sql
                          where curso.id_curso_ativo == ativo
                          select curso;

                if (!String.IsNullOrEmpty(desc))
                    if (inicio)
                        sql = from curso in sql
                              where curso.no_curso.StartsWith(desc)
                              select curso;
                    else
                        sql = from curso in sql
                              where curso.no_curso.Contains(desc)
                              select curso;

                if (estagio.HasValue)
                    sql = from r in sql
                          where r.cd_estagio == estagio
                          select r;
                else
                    if (produto.HasValue)
                        sql = from r in sql
                              where r.cd_produto == produto
                              select r;
                if (modalidade.HasValue)
                    sql = from r in sql
                          where r.cd_modalidade == modalidade
                          select r;

                if (nivel.HasValue)
                    sql = from r in sql
                          where r.cd_nivel == nivel
                          select r;

                if (dt_inicial.HasValue && dt_final.HasValue)
                {
                    //os cursos que possuem títulos abertos, que não possuem baixas e nem Cnab emitido, no período selecionado
                    SGFWebContext dbContext = new SGFWebContext();
                    int cd_origem = Int32.Parse(dbContext.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                    sql = (from cu in sql
                          join z in db.Nivel on cu.cd_curso equals z.cd_nivel 
                          join t in db.Turma on cu.cd_curso equals t.cd_curso 
                          join at in db.AlunoTurma on t.cd_turma equals at.cd_turma
                          join c in db.Contrato on at.cd_contrato equals c.cd_contrato
                          where db.Titulo.Where(ti => c.cd_contrato == ti.cd_origem_titulo &&
                                                    ti.id_origem_titulo == cd_origem &&
                                                    c.Aluno.cd_pessoa_aluno == ti.cd_pessoa_titulo &&

                                                    ti.id_status_titulo == (byte)FundacaoFisk.SGF.GenericModel.Titulo.StatusTitulo.ABERTO &&
                                                    ti.id_status_cnab == (byte)FundacaoFisk.SGF.GenericModel.Titulo.StatusCnabTitulo.INICIAL
                                                    && ti.dt_vcto_titulo >= dt_inicial.Value
                                                    && ti.dt_vcto_titulo <= dt_final.Value
                                                    && !ti.BaixaTitulo.Any(x => x.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA)
                                  ).Any()
                                  //&&
                                  //!db.Titulo.Where(ti => c.cd_contrato == ti.cd_origem_titulo &&
                                  //                  ti.id_origem_titulo == cd_origem &&
                                  //                  c.Aluno.cd_pessoa_aluno == ti.cd_pessoa_titulo &&

                                  //                  ti.id_status_titulo == (byte)FundacaoFisk.SGF.GenericModel.Titulo.StatusTitulo.FECHADO
                                  //                  && ti.dt_vcto_titulo >= dt_inicial.Value
                                  //                  && ti.dt_vcto_titulo <= dt_final.Value
                                  //).Any()
                                  //&&
                                  //!db.Titulo.Where(ti => c.cd_contrato == ti.cd_origem_titulo &&
                                  //                  ti.id_origem_titulo == cd_origem &&
                                  //                  c.Aluno.cd_pessoa_aluno == ti.cd_pessoa_titulo &&

                                  //                  ti.id_status_cnab != (byte)FundacaoFisk.SGF.GenericModel.Titulo.StatusCnabTitulo.INICIAL
                                  //                  && ti.dt_vcto_titulo >= dt_inicial.Value
                                  //                  && ti.dt_vcto_titulo <= dt_final.Value
                                  //.Any()
                          select cu).Distinct();
                }

                var retorno2 = (from r in sql
                                select new CursoSearch
                                {
                                    cd_curso = r.cd_curso,
                                    no_curso = r.no_curso,
                                    cd_nivel = r.Nivel.cd_nivel,
                                    dc_nivel = r.Nivel.dc_nivel,
                                    cd_proximo_curso = r.cd_proximo_curso,
                                    nm_carga_horaria = r.nm_carga_horaria,
                                    nm_carga_maxima = r.nm_carga_maxima,
                                    nm_faixa_etaria_maxima = r.nm_faixa_etaria_maxima,
                                    nm_faixa_etaria_minima = r.nm_faixa_etaria_minima,
                                    nm_total_nota = r.nm_total_nota,
                                    nm_vagas_curso = r.nm_vagas_curso,
                                    pc_criterio_aprovacao = r.pc_criterio_aprovacao,
                                    cd_produto = r.Produto.cd_produto,
                                    no_produto = r.Produto.no_produto,
                                    cd_estagio = r.Estagio.cd_estagio,
                                    no_estagio = r.Estagio.no_estagio,
                                    cd_modalidade = r.Modalidade.cd_modalidade,
                                    no_modalidade = r.Modalidade.no_modalidade,
                                    id_curso_ativo = r.id_curso_ativo,
                                    id_permitir_matricula = r.id_permitir_matricula,
                                    id_certificado = r.id_certificado
                                });

                var retorno3 = sorter.Sort(retorno2);

                int limite = retorno3.Count();

                parametros.ajustaParametrosPesquisa(limite);
                var retorno4 = retorno3.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;

                var retorno5 = retorno4.ToList();

                List<Curso> retornoFinal = new List<Curso>();
                if (retorno5 != null)
                    foreach (CursoSearch ret in retorno5)
                    {
                        Curso ret2 = new Curso();
                        ret2.copy(ret);
                        retornoFinal.Add(ret2);
                    }
                return retornoFinal;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Curso> getCursoByContratoSearch(SearchParameters parametros, int cd_contrato, string desc, bool inicio, bool? ativo, int? produto, int? estagio, int? modalidade, int? nivel, DateTime? dt_inicial, DateTime? dt_final)
        {
            try
            {
                IEntitySorter<CursoSearch> sorter = EntitySorter<CursoSearch>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<Curso> sql;

                sql = from curso in db.Curso.AsNoTracking()
                      join cc in db.CursoContrato on curso.cd_curso equals cc.cd_curso 
                      where cc.cd_contrato == cd_contrato
                      select curso;

                if (ativo != null)
                    sql = from curso in sql
                          where curso.id_curso_ativo == ativo
                          select curso;

                if (!String.IsNullOrEmpty(desc))
                    if (inicio)
                        sql = from curso in sql
                              where curso.no_curso.StartsWith(desc)
                              select curso;
                    else
                        sql = from curso in sql
                              where curso.no_curso.Contains(desc)
                              select curso;

                if (estagio.HasValue)
                    sql = from r in sql
                          where r.cd_estagio == estagio
                          select r;
                else
                    if (produto.HasValue)
                    sql = from r in sql
                          where r.cd_produto == produto
                          select r;
                if (modalidade.HasValue)
                    sql = from r in sql
                          where r.cd_modalidade == modalidade
                          select r;

                if (nivel.HasValue)
                    sql = from r in sql
                          where r.cd_nivel == nivel
                          select r;

                

                var retorno2 = (from r in sql
                                select new CursoSearch
                                {
                                    cd_curso = r.cd_curso,
                                    no_curso = r.no_curso,
                                    cd_nivel = r.Nivel.cd_nivel,
                                    dc_nivel = r.Nivel.dc_nivel,
                                    cd_proximo_curso = r.cd_proximo_curso,
                                    nm_carga_horaria = r.nm_carga_horaria,
                                    nm_carga_maxima = r.nm_carga_maxima,
                                    nm_faixa_etaria_maxima = r.nm_faixa_etaria_maxima,
                                    nm_faixa_etaria_minima = r.nm_faixa_etaria_minima,
                                    nm_total_nota = r.nm_total_nota,
                                    nm_vagas_curso = r.nm_vagas_curso,
                                    pc_criterio_aprovacao = r.pc_criterio_aprovacao,
                                    cd_produto = r.Produto.cd_produto,
                                    no_produto = r.Produto.no_produto,
                                    cd_estagio = r.Estagio.cd_estagio,
                                    no_estagio = r.Estagio.no_estagio,
                                    cd_modalidade = r.Modalidade.cd_modalidade,
                                    no_modalidade = r.Modalidade.no_modalidade,
                                    id_curso_ativo = r.id_curso_ativo,
                                    id_permitir_matricula = r.id_permitir_matricula,
                                    id_certificado = r.id_certificado
                                });

                var retorno3 = sorter.Sort(retorno2);

                int limite = retorno3.Count();

                parametros.ajustaParametrosPesquisa(limite);
                var retorno4 = retorno3.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;

                var retorno5 = retorno4.ToList();

                List<Curso> retornoFinal = new List<Curso>();
                if (retorno5 != null)
                    foreach (CursoSearch ret in retorno5)
                    {
                        Curso ret2 = new Curso();
                        ret2.copy(ret);
                        retornoFinal.Add(ret2);
                    }
                return retornoFinal;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<CursoUI> getCursoProduto(SearchParameters parametros, string desc, int? produto)
        {
            try
            {
                IEntitySorter<CursoUI> sorter = EntitySorter<CursoUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<CursoUI> sql;

                sql = from curso in db.Curso.AsNoTracking()
                      where curso.id_curso_ativo == true
                      select new CursoUI
                      {
                          no_produto = curso.Produto.no_produto,
                          no_curso = curso.no_curso,
                          cd_curso = curso.cd_curso,
                          cd_produto = curso.cd_produto
                      };
                sql = sorter.Sort(sql);
                var retorno = from curso in sql
                              select curso;

                if (!String.IsNullOrEmpty(desc))
                    retorno = from curso in sql
                              where curso.no_curso.StartsWith(desc)
                              select curso;
                if (produto.HasValue && produto > 0)
                    retorno = from r in retorno
                              where r.cd_produto == produto
                              select r;

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

        public IEnumerable<CursoUI> getCursoProdutoPorTipoAval(SearchParameters parametros, string desc, int? produto, int cdTipoAvaliacao)
        {
            try
            {
                IEntitySorter<CursoUI> sorter = EntitySorter<CursoUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<CursoUI> sql;

                sql = from curso in db.Curso.AsNoTracking()
                      where curso.AvaliacaoCurso.Where(p => p.cd_tipo_avaliacao == cdTipoAvaliacao).Any()
                      where curso.id_curso_ativo == true
                      select new CursoUI
                      {
                          no_produto = curso.Produto.no_produto,
                          no_curso = curso.no_curso,
                          cd_curso = curso.cd_curso,
                          cd_produto = curso.cd_produto
                      };
                sql = sorter.Sort(sql);
                var retorno = from curso in sql
                              select curso;

                if (!String.IsNullOrEmpty(desc))
                    retorno = from curso in sql
                              where curso.no_curso.StartsWith(desc)
                              select curso;
                if (produto.HasValue && produto > 0)
                    retorno = from r in retorno
                              where r.cd_produto == produto
                              select r;

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

        public IEnumerable<Curso> getCursosAulaPersonalizada(int cd_aluno, int cd_escola) {
            try
            {
                IEnumerable<Curso> sql  = (from curso in db.Curso
                                           where db.AulaPersonalizada.Where(ap => ap.AulaPersonalizadaAlunos.Where(apa => apa.cd_aluno == cd_aluno 
                                                                                    && apa.DiarioAula.Turma.cd_curso == curso.cd_curso).Any() 
                                                                                    && ap.cd_escola == cd_escola).Any()
                               orderby curso.no_curso
                               select new
                               {
                                   cd_curso = curso.cd_curso,
                                   no_curso = curso.no_curso
                               }).ToList().Select(c => new Curso
                               {
                                   cd_curso = c.cd_curso,
                                   no_curso = c.no_curso
                               });
                        
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public IEnumerable<Curso> getCursosCargaHoraria(bool todasEscolas, int cd_escola)
        {
            try
            {
                IEnumerable<Curso> sql = (from curso in db.Curso.AsNoTracking()
                                          where curso.id_curso_ativo
                                          select curso);


                if (todasEscolas == false)
                {
                    sql = from c in sql
                          where (from cc in db.vi_carga_horaria
                                 where cc.cd_curso == c.cd_curso &&
                                       cc.cd_escola == cd_escola
                                 select cc).Any()
                        select c;
                }
                else
                {
                    sql = from c in sql
                          where (from cc in db.vi_carga_horaria
                                 where cc.cd_curso == c.cd_curso
                                 select cc).Any()
                          select c;
                }

                
                sql = (from curso in sql
                orderby curso.no_curso
               select new
               {
                   cd_curso = curso.cd_curso,
                   no_curso = curso.no_curso
               }).ToList().Select(c => new Curso
               {
                   cd_curso = c.cd_curso,
                   no_curso = c.no_curso
               });
                        
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Curso> getCursos(TipoConsultaCursoEnum hasDependente, int? cd_curso, int? cd_produto, int? cd_escola)
        {
            try
            {
                IEnumerable<Curso> sql = null;
                switch (hasDependente)
                {
                    case TipoConsultaCursoEnum.HAS_ATIVOPROD:
                        sql = (from curso in db.Curso
                               where (curso.id_curso_ativo == true || (cd_curso.HasValue && curso.cd_curso == cd_curso.Value)) &&
                                     (cd_produto.HasValue && curso.cd_produto == cd_produto.Value)
                               orderby curso.no_curso
                               select new
                               {
                                   cd_curso = curso.cd_curso,
                                   no_curso = curso.no_curso
                               }).ToList().Select(c => new Curso
                               {
                                   cd_curso = c.cd_curso,
                                   no_curso = c.no_curso
                               });
                        break;

                    case TipoConsultaCursoEnum.HAS_ATIVO:
                        sql = (from curso in db.Curso
                              where (curso.id_curso_ativo == true ||
                                    (cd_curso.HasValue && curso.cd_curso == cd_curso.Value))
                              orderby curso.no_curso
                              select new
                               {
                                   cd_curso = curso.cd_curso,
                                   no_curso = curso.no_curso
                               }).ToList().Select(c => new Curso
                               {
                                   cd_curso = c.cd_curso,
                                   no_curso = c.no_curso
                               });
                        break;
                    case TipoConsultaCursoEnum.HAS_TURMA:
                        sql = (from curso in db.Curso
                              where curso.Turma.Where(c => c.cd_pessoa_escola == (int)cd_escola).Any()
                              orderby curso.no_curso
                              select new
                               {
                                   cd_curso = curso.cd_curso,
                                   no_curso = curso.no_curso
                               }).ToList().Select(c => new Curso
                               {
                                   cd_curso = c.cd_curso,
                                   no_curso = c.no_curso
                               });
                        break;
                    case TipoConsultaCursoEnum.HAS_PRODUTO:
                        sql = from curso in db.Curso
                              where curso.cd_produto == cd_produto.Value && curso.id_curso_ativo == true
                              orderby curso.no_curso
                              select curso;
                        break;
                    case TipoConsultaCursoEnum.HAS_AVALIACAO_CURSO:
                        sql = from curso in db.Curso
                              where curso.AvaliacaoCurso.Where(a => a.cd_curso == curso.cd_curso).Any()
                              orderby curso.no_curso
                              select curso;
                        break;
                    case TipoConsultaCursoEnum.HAS_DESISTENCIA:
                        sql = from curso in db.Curso
                              where db.AlunoTurma.Any(x => x.Turma.cd_pessoa_escola == cd_escola.Value && x.Turma.cd_curso == curso.cd_curso && x.Desistencia.Any()) &&
                                    (!cd_produto.HasValue || curso.cd_produto == cd_produto.Value) 
                              orderby curso.no_curso
                              select curso;
                        break;
                }
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Curso> getCursosSemMatriculaSimultanea(TipoConsultaCursoEnum hasDependente, int? cd_curso, int? cd_produto, int? cd_escola)
        {
            try
            {
                IEnumerable<Curso> sql = null;
                switch (hasDependente)
                {
                    case TipoConsultaCursoEnum.HAS_ATIVOPROD:
                        sql = (from curso in db.Curso
                               where (curso.id_curso_ativo == true || (cd_curso.HasValue && curso.cd_curso == cd_curso.Value)) &&
                                     (cd_produto.HasValue && curso.cd_produto == cd_produto.Value) && curso.id_permitir_matricula == false
                               orderby curso.no_curso
                               select new
                               {
                                   cd_curso = curso.cd_curso,
                                   no_curso = curso.no_curso,
                                   cd_proximo_curso = curso.cd_proximo_curso
                               }).ToList().Select(c => new Curso
                               {
                                   cd_curso = c.cd_curso,
                                   no_curso = c.no_curso,
                                   cd_proximo_curso = c.cd_proximo_curso
                               });
                        break;

                    case TipoConsultaCursoEnum.HAS_ATIVO:
                        sql = (from curso in db.Curso
                               where (curso.id_curso_ativo == true ||
                                     (cd_curso.HasValue && curso.cd_curso == cd_curso.Value))
                               orderby curso.no_curso
                               select new
                               {
                                   cd_curso = curso.cd_curso,
                                   no_curso = curso.no_curso
                               }).ToList().Select(c => new Curso
                               {
                                   cd_curso = c.cd_curso,
                                   no_curso = c.no_curso
                               });
                        break;
                    case TipoConsultaCursoEnum.HAS_TURMA:
                        sql = (from curso in db.Curso
                               where curso.Turma.Where(c => c.cd_pessoa_escola == (int)cd_escola).Any()
                               orderby curso.no_curso
                               select new
                               {
                                   cd_curso = curso.cd_curso,
                                   no_curso = curso.no_curso
                               }).ToList().Select(c => new Curso
                               {
                                   cd_curso = c.cd_curso,
                                   no_curso = c.no_curso
                               });
                        break;
                    case TipoConsultaCursoEnum.HAS_PRODUTO:
                        sql = from curso in db.Curso
                              where curso.cd_produto == cd_produto.Value && curso.id_curso_ativo == true
                              orderby curso.no_curso
                              select curso;
                        break;
                    case TipoConsultaCursoEnum.HAS_AVALIACAO_CURSO:
                        sql = from curso in db.Curso
                              where curso.AvaliacaoCurso.Where(a => a.cd_curso == curso.cd_curso).Any()
                              orderby curso.no_curso
                              select curso;
                        break;
                    case TipoConsultaCursoEnum.HAS_DESISTENCIA:
                        sql = from curso in db.Curso
                              where db.AlunoTurma.Any(x => x.Turma.cd_pessoa_escola == cd_escola.Value && x.Turma.cd_curso == curso.cd_curso && x.Desistencia.Any()) &&
                                    (!cd_produto.HasValue || curso.cd_produto == cd_produto.Value)
                              orderby curso.no_curso
                              select curso;
                        break;
                }
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Curso> getCursoByProdutoSemMatriculaSimultanea(int? cd_curso, int? cd_produto, int? cd_escola)
        {
            try
            {
                IEnumerable<Curso> sql = null;
                if (cd_curso.HasValue && cd_produto.HasValue)
                {
                        sql = (from curso in db.Curso
                               where (curso.id_curso_ativo == true && (cd_curso.HasValue && curso.cd_curso == cd_curso.Value)) &&
                                     (cd_produto.HasValue && curso.cd_produto == cd_produto.Value) && curso.id_permitir_matricula == false
                               orderby curso.no_curso
                               select new
                               {
                                   cd_curso = curso.cd_curso,
                                   no_curso = curso.no_curso,
                                   cd_proximo_curso = curso.cd_proximo_curso
                               }).ToList().Select(c => new Curso
                               {
                                   cd_curso = c.cd_curso,
                                   no_curso = c.no_curso,
                                   cd_proximo_curso = c.cd_proximo_curso
                               });
                }
                
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Curso> getProximoCursoPorProdutoSemMatriculaSimultanea(TipoConsultaCursoEnum hasDependente, int? cd_curso, int? cd_produto, int? cd_escola)
        {
            try
            {
                IEnumerable<Curso> sql = null;
                switch (hasDependente)
                {
                    case TipoConsultaCursoEnum.HAS_ATIVOPROD:
                        sql = (from curso in db.Curso
                               join vi in db.vi_curso_ordem on curso.cd_curso equals vi.cd_curso 
                               where (curso.id_curso_ativo == true && (cd_curso.HasValue && curso.cd_curso == cd_curso.Value)) &&
                                     (cd_produto.HasValue && curso.cd_produto == cd_produto.Value) && curso.id_permitir_matricula == false
                               orderby curso.no_curso
                               select new
                               {
                                   cd_curso = curso.cd_curso,
                                   no_curso = curso.no_curso,
                                   cd_proximo_curso = curso.cd_proximo_curso,
                                   cd_curso_ordem = vi.cd_curso_ordem
                               }).ToList().Select(c => new Curso
                               {
                                   cd_curso = c.cd_curso,
                                   no_curso = c.no_curso,
                                   cd_proximo_curso = c.cd_proximo_curso,
                                   cd_curso_ordem = c.cd_curso_ordem
                               });
                        //if (sql.Count() > 0)
                        //{
                        //    var cdProximo = sql.FirstOrDefault().cd_proximo_curso;

                        //    sql = (from curso in db.Curso
                        //           where (curso.id_curso_ativo == true && (cd_curso.HasValue && curso.cd_curso == cdProximo.Value)) &&
                        //              (cd_produto.HasValue && curso.cd_produto == cd_produto.Value) && curso.id_permitir_matricula == false
                        //           orderby curso.no_curso
                        //           select new
                        //           {
                        //               cd_curso = curso.cd_curso,
                        //               no_curso = curso.no_curso,
                        //               cd_proximo_curso = curso.cd_proximo_curso
                        //           }).ToList().Select(c => new Curso
                        //           {
                        //               cd_curso = c.cd_curso,
                        //               no_curso = c.no_curso,
                        //               cd_proximo_curso = c.cd_proximo_curso
                        //           });

                        //    if (sql.Count() == 0)
                        //    {
                        //        throw new CursoBusinessException("Não há uma uma próxima sequência de Curso para inserir.", null, CursoBusinessException.TipoErro.ERRO_NOT_EXISTS_PROXIMO_CURSO, false);
                        //    }
                        //}
                        if (sql.Count() == 0)
                        {
                            throw new CursoBusinessException("Não há uma uma próxima sequência de Curso para inserir.", null, CursoBusinessException.TipoErro.ERRO_NOT_EXISTS_PROXIMO_CURSO, false);
                        }
                        break;

                    case TipoConsultaCursoEnum.HAS_ATIVO:
                        sql = (from curso in db.Curso
                               where (curso.id_curso_ativo == true ||
                                     (cd_curso.HasValue && curso.cd_curso == cd_curso.Value))
                               orderby curso.no_curso
                               select new
                               {
                                   cd_curso = curso.cd_curso,
                                   no_curso = curso.no_curso
                               }).ToList().Select(c => new Curso
                               {
                                   cd_curso = c.cd_curso,
                                   no_curso = c.no_curso
                               });
                        break;
                    case TipoConsultaCursoEnum.HAS_TURMA:
                        sql = (from curso in db.Curso
                               where curso.Turma.Where(c => c.cd_pessoa_escola == (int)cd_escola).Any()
                               orderby curso.no_curso
                               select new
                               {
                                   cd_curso = curso.cd_curso,
                                   no_curso = curso.no_curso
                               }).ToList().Select(c => new Curso
                               {
                                   cd_curso = c.cd_curso,
                                   no_curso = c.no_curso
                               });
                        break;
                    case TipoConsultaCursoEnum.HAS_PRODUTO:
                        sql = from curso in db.Curso
                              where curso.cd_produto == cd_produto.Value && curso.id_curso_ativo == true
                              orderby curso.no_curso
                              select curso;
                        break;
                    case TipoConsultaCursoEnum.HAS_AVALIACAO_CURSO:
                        sql = from curso in db.Curso
                              where curso.AvaliacaoCurso.Where(a => a.cd_curso == curso.cd_curso).Any()
                              orderby curso.no_curso
                              select curso;
                        break;
                    case TipoConsultaCursoEnum.HAS_DESISTENCIA:
                        sql = from curso in db.Curso
                              where db.AlunoTurma.Any(x => x.Turma.cd_pessoa_escola == cd_escola.Value && x.Turma.cd_curso == curso.cd_curso && x.Desistencia.Any()) &&
                                    (!cd_produto.HasValue || curso.cd_produto == cd_produto.Value)
                              orderby curso.no_curso
                              select curso;
                        break;
                }
                return sql;
            }
            catch (CursoBusinessException exe)
            {
                throw new CursoBusinessException(exe.Message, exe, exe.tipoErro, exe.MostraStackTrace);
            }

            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool deleteCursos(List<Curso> cursos)
        {
            try
            {
                string strCurso = "";
                if (cursos != null && cursos.Count > 0)
                    foreach (Curso e in cursos)
                        strCurso += e.cd_curso + ",";

                // Remove o último ponto e virgula:
                if (strCurso.Length > 0)
                    strCurso = strCurso.Substring(0, strCurso.Length - 1);

                int retorno = db.Database.ExecuteSqlCommand("delete from t_curso where cd_curso in(" + strCurso + ")");

                return retorno > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Curso addCurso(Curso curso)
        {
            try
            {
                List<int> idsItens = new List<int>();
                List<ItemCurso> itens = curso.MateriaisDidaticos.ToList<ItemCurso>();

                for (int i = 0; i < itens.Count; i++)
                    idsItens.Add(itens[i].cd_item_curso);
                int[] ids = idsItens.ToArray<int>();
                var query = from i in db.ItemCurso
                            where ids.Contains(i.cd_item_curso)
                            select i;
                curso.MateriaisDidaticos = query.ToList();
                db.Curso.Add(curso);
                int ret = db.SaveChanges();

                if (ret > 0)
                    return this.findFullById(curso.cd_curso);
                else
                    return null;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Curso findFullById(int cdCurso)
        {
            try
            {
                          
                Curso sql = (from c in db.Curso
                             where c.cd_curso == cdCurso
                             select new
                             {
                               cd_curso = c.cd_curso,
                               no_curso = c.no_curso,
                               cd_produto = c.cd_produto,
                               cd_estagio = c.cd_estagio,
                               no_produto = c.Produto.no_produto,
                               no_estagio = c.Estagio.no_estagio,
                               no_modalidade = c.Modalidade.no_modalidade,
                               dc_nivel = c.Nivel.dc_nivel,
                               nm_carga_horaria = c.nm_carga_horaria,
                               nm_carga_maxima = c.nm_carga_maxima,
                               nm_vagas_curso = c.nm_vagas_curso,
                               pc_criterio_aprovacao = c.pc_criterio_aprovacao,
                               nm_total_nota = c.nm_total_nota,
                               nm_faixa_etaria_minima = c.nm_faixa_etaria_minima,
                               nm_faixa_etaria_maxima = c.nm_faixa_etaria_maxima,
                               cd_proximo_curso = c.cd_proximo_curso,
                               cd_modalidade = c.cd_modalidade,
                               id_curso_ativo = c.id_curso_ativo,
                               id_permitir_matricula = c.id_permitir_matricula,
                               cd_nivel = c.cd_nivel,
                               id_certificado = c.id_certificado
                             }).ToList().Select(x => new Curso { 
                               cd_curso = x.cd_curso, 
                               no_curso = x.no_curso,
                               no_produto = x.no_produto,
                               no_estagio = x.no_estagio,
                               no_modalidade = x.no_modalidade,
                               dc_nivel = x.dc_nivel,
                               cd_produto = x.cd_produto,
                               cd_estagio = x.cd_estagio,
                               nm_carga_horaria = x.nm_carga_horaria,
                               nm_carga_maxima = x.nm_carga_maxima,
                               nm_vagas_curso = x.nm_vagas_curso,
                               pc_criterio_aprovacao = x.pc_criterio_aprovacao,
                               nm_total_nota = x.nm_total_nota,
                               nm_faixa_etaria_minima = x.nm_faixa_etaria_minima,
                               nm_faixa_etaria_maxima = x.nm_faixa_etaria_maxima,
                               cd_proximo_curso = x.cd_proximo_curso,
                               cd_modalidade = x.cd_modalidade,
                               id_curso_ativo = x.id_curso_ativo,
                               id_permitir_matricula = x.id_permitir_matricula,
                               cd_nivel = x.cd_nivel,
                               id_certificado = x.id_certificado
                             }).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Curso editCurso(Curso curso)
        {
            try
            {
                //List<int> idsItens = new List<int>();
                List<ItemCurso> itens = curso.MateriaisDidaticos.ToList<ItemCurso>();
                ///curso.MateriaisDidaticos = new List<ItemCurso>();
               // this.edit(curso, false);
                //for (int i = 0; i < itens.Count; i++)
                //    idsItens.Add(itens[i].cd_item_curso);

                //int[] ids = idsItens.ToArray<int>();
                //var query = from i in db.ItemCurso
                //            where ids.Contains(i.cd_item_curso)
                //            select i;

                var ret = 0;
                if (itens.Count() > 0)
                {
                    foreach (ItemCurso i in itens)
                    {

                        db.ItemCurso.Add(i);
                    }
                    ret = db.SaveChanges();
                }

                var query = from i in db.ItemCurso
                            where i.cd_curso == curso.cd_curso
                            select i;

                curso.MateriaisDidaticos = query.ToList<ItemCurso>();

                return this.findFullById(this.edit(curso, false).cd_curso);
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public IEnumerable<Curso> getCursoTabelaPreco(int cdEscola)
        {
            try
            {
                IEnumerable<Curso> sql;

                sql = (from curso in db.Curso
                       where curso.CursoTabelaPreco.Any(t => t.cd_pessoa_escola == cdEscola)
                       orderby curso.no_curso
                       select new
                               {
                                   cd_curso = curso.cd_curso,
                                   no_curso = curso.no_curso
                               }).ToList().Select(c => new Curso
                               {
                                   cd_curso = c.cd_curso,
                                   no_curso = c.no_curso
                               });

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Curso firstOrDefault()
        {
            try
            {
                var sql = (from curso in db.Curso
                           select curso).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Curso> findAllCurso()
        {
            try
            {
                var sql = (from curso in db.Curso
                           orderby curso.no_curso
                           select new
                           {
                               cd_curso = curso.cd_curso,
                               no_curso = curso.no_curso
                           }).Select(x => new Curso { cd_curso = x.cd_curso, no_curso = x.no_curso });
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Curso> getCursoProgramacao()
        {
            try
            {
                var sql = (from curso in db.Curso
                           where curso.ProgramacaoCurso.Any()
                           orderby curso.no_curso
                           select curso);
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public IEnumerable<CursoUI> getCursoTipoAvaliacao(int cdTipoAvaliacao)
        {
            try
            {
                var retorno = from curso in db.Curso
                              where curso.AvaliacaoCurso.Where(p => p.cd_tipo_avaliacao == cdTipoAvaliacao).Any()
                              orderby curso.no_curso
                              select new CursoUI
                              {
                                  no_produto = curso.Produto.no_produto,
                                  no_curso = curso.no_curso,
                                  cd_curso = curso.cd_curso,
                                  cd_produto = curso.cd_produto
                              };
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public List<Curso> getCursosByCod(int[] cdCursos)
        {
            try
            {
                var result = from curso in db.Curso
                             where cdCursos.Contains(curso.cd_curso)
                             orderby curso.no_curso
                             select curso;
                return result.ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Curso> getCursoAvaliacaoTurma(int cd_escola)
        {
            try
            {
                var retorno = (from curso in db.Curso
                               join turma in db.Turma on curso.cd_curso equals turma.cd_curso
                               join avaliacaTurma in db.AvaliacaoTurma on turma.cd_turma equals avaliacaTurma.cd_turma
                               where turma.cd_pessoa_escola == cd_escola
                               select new
                               {
                                   cd_curso = curso.cd_curso,
                                   no_curso = curso.no_curso
                               }).Distinct().ToList().Select(c => new Curso { cd_curso = c.cd_curso, no_curso = c.no_curso }).OrderBy(c => c.no_curso);
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        //TODO fazer o teste desse método
        public IEnumerable<Curso> getCursoWithAtividadeExtra(bool isAtivo, int cd_pesso_escola)
        {
            try
            {
                var retorno = (from curso in db.Curso
                               join atividadeCurso in db.AtividadeCurso on curso.cd_curso equals atividadeCurso.cd_curso
                               join atividadeExtra in db.AtividadeExtra on atividadeCurso.cd_atividade_extra equals atividadeExtra.cd_atividade_extra
                               where atividadeExtra.cd_pessoa_escola == cd_pesso_escola
                                  && curso.id_curso_ativo == isAtivo
                               orderby curso.no_curso
                               select new
                               {
                                   cd_curso = curso.cd_curso,
                                   no_curso = curso.no_curso
                               }).Distinct().ToList().Select(c => new Curso { cd_curso = c.cd_curso, no_curso = c.no_curso }).OrderBy(c => c.no_curso);
                return retorno;
                
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Curso returExistCursoWithTurma(int cd_pesso_escola, int cd_curso)
        {
            try
            {
                var sql = (from curso in db.Curso
                           join turma in db.Turma on curso.cd_curso equals turma.cd_curso
                           where curso.cd_curso == cd_curso
                              //&& turma.cd_pessoa_escola == cd_pesso_escola
                           select curso).ToList().Select(c =>
                               new Curso
                               {
                                   cd_curso = c.cd_curso,
                                   no_curso = c.no_curso,
                                   cd_produto = c.cd_produto
                               }).FirstOrDefault(); 

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public int? findProxCurso(int cdCurso)
        {
            try
            {
                int? sql = (from curso in db.Curso
                             where curso.cd_curso == cdCurso
                             select curso.cd_proximo_curso).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Curso> getCursoProduto(List<int> cd_produtos)
        {
            try
            {
                IEnumerable<Curso> sql = (from curso in db.Curso
                            where cd_produtos.Contains(curso.cd_produto)
                            select curso).ToList().Select(c =>
                               new Curso
                               {
                                   cd_curso = c.cd_curso,
                                   no_curso = c.no_curso
                               }); 
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public long? GetCursoOrdem(int cdCurso)
        {
            return (from vi in db.vi_curso_ordem
                    where vi.cd_curso == cdCurso
                    select vi.cd_curso_ordem).FirstOrDefault();
        }
    }
}

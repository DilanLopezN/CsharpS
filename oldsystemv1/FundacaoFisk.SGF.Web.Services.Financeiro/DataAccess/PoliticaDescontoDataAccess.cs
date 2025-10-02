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
    public class PoliticaDescontoDataAccess : GenericRepository<PoliticaDesconto>, IPoliticaDescontoDataAccess
    {
       

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }

        public IEnumerable<PoliticaDesconto> GetAllPoliticaDesconto()
        {
            try
            {
                var sql = from c in db.PoliticaDesconto
                          select c;
                return sql.ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<PoliticaDescontoUI> GetPoliticaDescontoSearch(SearchParameters parametros, int cdTurma, int cdAluno, DateTime? dtaIni, DateTime? dtaFim, bool? ativo, int cdEscola)
        {
            try
            {
                IEntitySorter<PoliticaDescontoUI> sorter = EntitySorter<PoliticaDescontoUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<PoliticaDescontoUI> sql;
                List<PoliticaDescontoUI> sql2 = new List<PoliticaDescontoUI>();

                var retorno = from c in db.PoliticaDesconto
                              where c.cd_pessoa_escola == cdEscola
                              select c;

                //Pegando apenas Politica com o Aluno Escolhido na Pesquisa
                if (cdAluno > 0)
                    retorno = from c in retorno
                              where c.PoliticasAlunos.Where(l => l.cd_aluno == cdAluno && l.Aluno.cd_pessoa_escola == cdEscola).Any()
                              select c;
                //Pegando apenas Politica com a Turma Escolhida na Pesquisa
                if (cdTurma > 0)
                    retorno = from c in retorno
                              where c.PoliticasTurmas.Where(l => l.cd_turma == cdTurma && l.Turma.cd_pessoa_escola == cdEscola).Any()
                              select c;
                //Pegando apenas Politica sem Aluno
                if (cdAluno < 0)
                    retorno = from c in retorno
                              where !c.PoliticasAlunos.Where(l => l.Aluno.cd_pessoa_escola == cdEscola).Any()
                              select c;
                //Pegando apenas Politica sem Turma
                if (cdTurma < 0)
                    retorno = from c in retorno
                              where !c.PoliticasTurmas.Where(l => l.Turma.cd_pessoa_escola == cdEscola).Any()
                              select c;
                //Pegando Politica por Periodo
                if (dtaIni != null)
                    if (dtaFim != null)
                        retorno = from c in retorno
                                  where c.dt_inicial_politica >= dtaIni && c.dt_inicial_politica <= dtaFim
                                  select c;
                    else
                        retorno = from c in retorno
                                  where c.dt_inicial_politica >= dtaIni
                                  select c;
                //Pegando Politica com o Filtro do Status Escolhido
                if (ativo != null)
                {
                    retorno = from c in retorno
                              where (c.id_ativo == ativo)
                              select c;
                }

                sql = from c in retorno
                      select new PoliticaDescontoUI
                      {
                          cd_pessoa_escola = c.cd_pessoa_escola,
                          cd_politica_desconto = c.cd_politica_desconto,
                          id_ativo = c.id_ativo,
                          dt_inicial_politica = c.dt_inicial_politica,
                          turmasPol = c.PoliticasTurmas,
                          alunosPol = c.PoliticasAlunos,
                          diasPolitica = c.DiasPolitica.ToList(),
                          pessoasAlunoPol = c.PoliticasAlunos.Select(p => p.Aluno.AlunoPessoaFisica),
                          turmas = c.PoliticasTurmas.Select(t => t.Turma)
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

        public PoliticaDesconto GetPoliticaDescontoById(int idPoliticaDesconto, int cdEscola)
        {
            try
            {
                var sql = (from c in db.PoliticaDesconto
                           where c.cd_politica_desconto == idPoliticaDesconto &&
                           c.cd_pessoa_escola == cdEscola
                           select c).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool deleteAllPoliticaDesconto(List<PoliticaDesconto> politicasDesconto)
        {
            try
            {
                string strPoliticaDesconto = "";
                if (politicasDesconto != null && politicasDesconto.Count > 0)
                    foreach (PoliticaDesconto e in politicasDesconto)
                        strPoliticaDesconto += e.cd_politica_desconto + ",";

                // Remove o último ponto e virgula:
                if (strPoliticaDesconto.Length > 0)
                    strPoliticaDesconto = strPoliticaDesconto.Substring(0, strPoliticaDesconto.Length - 1);

                int retorno = db.Database.ExecuteSqlCommand("delete from t_politica_desconto where cd_politica_desconto in(" + strPoliticaDesconto + ")");

                return retorno > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool addDiasPolitica(DiasPolitica dia)
        {
            try
            {
                dia.DiasPoliticaDesc = null;
                db.DiasPolitica.Add(dia);
                return (db.SaveChanges() > 0);
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }

        }
        
        public PoliticaDescontoUI getPoliticaDesconto(int cdEscola, int cd_politica_desconto)
        {
            try
            {
                PoliticaDescontoUI sql;

                sql = (from c in db.PoliticaDesconto
                       where
                           c.cd_pessoa_escola == cdEscola
                           && c.cd_politica_desconto == cd_politica_desconto
                       select new PoliticaDescontoUI
                       {
                           cd_pessoa_escola = c.cd_pessoa_escola,
                           cd_politica_desconto = c.cd_politica_desconto,
                           id_ativo = c.id_ativo,
                           dt_inicial_politica = c.dt_inicial_politica,
                           diasPolitica = c.DiasPolitica.ToList()
                      }).FirstOrDefault();
                if (sql != null && sql.cd_politica_desconto > 0) {
                    sql.pessoasAlunoPol = (from pa in db.PoliticaAluno
                                     where
                                        pa.PoliticaDesconto.cd_pessoa_escola == cdEscola
                                        && pa.cd_politica_desconto == cd_politica_desconto
                                     select new
                                     {
                                         cd_pessoa = pa.Aluno.cd_pessoa_aluno,
                                         no_pessoa = pa.Aluno.AlunoPessoaFisica.no_pessoa
                                     }).ToList().Select(x => new PessoaSGF 
                                     {
                                              no_pessoa = x.no_pessoa,
                                              cd_pessoa = x.cd_pessoa 
                                     }).ToList();

                    sql.turmasPol = (from pt in db.PoliticaTurma
                                     where
                                        pt.PolitcaDesconto.cd_pessoa_escola == cdEscola
                                        && pt.cd_politica_desconto == cd_politica_desconto
                                     select new
                                     {
                                         cd_turma = pt.Turma.cd_turma,
                                         no_turma = pt.Turma.no_turma
                                     }).ToList().Select(x => new PoliticaTurma
                                     {
                                         Turma = new Turma
                                         {
                                             cd_turma = x.cd_turma,
                                             no_turma = x.no_turma
                                         }
                                     }).ToList();

                }



                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public PoliticaDesconto getPoliticaDescontoByTurmaAluno(int cd_turma, int cd_aluno, DateTime dt_vcto_titulo) {
            try
            {
                return ((from p in db.PoliticaDesconto
                         where p.PoliticasAlunos.Where(pa => pa.cd_aluno == cd_aluno).Any()
                            && p.PoliticasTurmas.Where(pa => pa.cd_turma == cd_turma).Any()
                            && p.id_ativo
                            && DbFunctions.TruncateTime(p.dt_inicial_politica) <= dt_vcto_titulo.Date
                         orderby p.dt_inicial_politica descending
                         select new {
                             diasPolitica = p.DiasPolitica.OrderBy(d => d.nm_dia_limite_politica),
                             cd_politica_desconto = p.cd_politica_desconto,
                             p.dt_inicial_politica
                         }).ToList().Select(x => new PoliticaDesconto {
                             cd_politica_desconto = x.cd_politica_desconto,
                             DiasPolitica = x.diasPolitica.ToList(),
                             dt_inicial_politica = x.dt_inicial_politica
                         })).FirstOrDefault();
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

        public PoliticaDesconto getPoliticaDescontoByAluno(int cd_aluno, DateTime dt_vcto_titulo) {
            try
            {
                return ((from p in db.PoliticaDesconto
                         where p.PoliticasAlunos.Where(pa => pa.cd_aluno == cd_aluno).Any()
                            && p.id_ativo
                            && p.PoliticasTurmas.Count == 0
                            && DbFunctions.TruncateTime(p.dt_inicial_politica) <= dt_vcto_titulo.Date
                         orderby p.dt_inicial_politica descending
                         select new {
                             diasPolitica = p.DiasPolitica.OrderBy(d => d.nm_dia_limite_politica),
                             cd_politica_desconto = p.cd_politica_desconto,
                             p.dt_inicial_politica
                         }).ToList().Select(x => new PoliticaDesconto {
                             cd_politica_desconto = x.cd_politica_desconto,
                             DiasPolitica = x.diasPolitica.ToList(),
                             dt_inicial_politica = x.dt_inicial_politica
                         })).FirstOrDefault();
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

        public PoliticaDesconto getPoliticaDescontoByTurma(int cd_turma, DateTime dt_vcto_titulo) {
            try
            {
                return ((from p in db.PoliticaDesconto
                         where p.PoliticasTurmas.Where(pa => pa.cd_turma == cd_turma).Any()
                            && p.id_ativo
                            && p.PoliticasAlunos.Count == 0
                            && DbFunctions.TruncateTime(p.dt_inicial_politica) <= dt_vcto_titulo.Date
                         orderby p.dt_inicial_politica descending
                         select new {
                             cd_politica_desconto = p.cd_politica_desconto,
                             diasPolitica = p.DiasPolitica.OrderBy(d => d.nm_dia_limite_politica),
                             p.dt_inicial_politica
                         }).ToList().Select(x => new PoliticaDesconto {
                             cd_politica_desconto = x.cd_politica_desconto,
                             DiasPolitica = x.diasPolitica.ToList(),
                             dt_inicial_politica = x.dt_inicial_politica
                         })).FirstOrDefault();
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

        public PoliticaDesconto getPoliticaDescontoByEscola(int cd_pessoa_escola, DateTime dt_vcto_titulo) {
            try
            {
                return ((from p in db.PoliticaDesconto
                    where p.cd_pessoa_escola == cd_pessoa_escola
                            && p.id_ativo
                            && p.PoliticasAlunos.Count == 0
                            && p.PoliticasTurmas.Count == 0
                            && DbFunctions.TruncateTime(p.dt_inicial_politica) <= dt_vcto_titulo.Date
                         orderby p.dt_inicial_politica descending
                         select new {
                             cd_politica_desconto = p.cd_politica_desconto,
                             diasPolitica = p.DiasPolitica.OrderBy(d => d.nm_dia_limite_politica),
                             p.dt_inicial_politica
                         }).ToList().Select(x => new PoliticaDesconto {
                             cd_politica_desconto = x.cd_politica_desconto,
                             DiasPolitica = x.diasPolitica.ToList(),
                             dt_inicial_politica = x.dt_inicial_politica
                         })).FirstOrDefault();
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

        public bool existPolIgual(PoliticaDesconto politica)
        {

            try
            {
                int pol = 0;
                //Politica por Aluno
                if ((politica.PoliticasAlunos != null && politica.PoliticasAlunos.Count() > 0) &&
                    (politica.PoliticasTurmas == null || !politica.PoliticasTurmas.Any() || politica.PoliticasTurmas.Count() <= 0))
                {
                    List<Int32> listaCodigosAluno = politica.PoliticasAlunos.Select(p => p.cd_aluno).ToList();
                    pol = (from p in db.PoliticaDesconto
                           where p.PoliticasAlunos.Where(pa => listaCodigosAluno.Contains(pa.cd_aluno)).Any() &&
                           p.cd_pessoa_escola == politica.cd_pessoa_escola &&
                           !p.PoliticasTurmas.Any() &&
                           p.dt_inicial_politica == politica.dt_inicial_politica &&
                           p.cd_politica_desconto != politica.cd_politica_desconto
                           select p.cd_politica_desconto).FirstOrDefault();
                }

                //Politica por Turma
                if ((politica.PoliticasTurmas != null && politica.PoliticasTurmas.Count() > 0) &&
                    (politica.PoliticasAlunos == null || !politica.PoliticasAlunos.Any() || politica.PoliticasAlunos.Count() <= 0))
                {
                    List<Int32> listaCodigosTurma = politica.PoliticasTurmas.Select(p => p.cd_turma).ToList();

                    pol = (from p in db.PoliticaDesconto
                           where p.PoliticasTurmas.Where(pa => listaCodigosTurma.Contains(pa.cd_turma)).Any() &&
                           p.cd_pessoa_escola == politica.cd_pessoa_escola &&
                           !p.PoliticasAlunos.Any() &&
                           p.dt_inicial_politica == politica.dt_inicial_politica &&
                           p.cd_politica_desconto != politica.cd_politica_desconto
                           select p.cd_politica_desconto).FirstOrDefault();
                }
                //Politica por Aluno e Turma
                if (politica.PoliticasTurmas != null && politica.PoliticasTurmas.Count() > 0 &&
                    politica.PoliticasAlunos != null && politica.PoliticasAlunos.Count() > 0)
                {
                    List<Int32> listaCodigosTurma = politica.PoliticasTurmas.Select(p => p.cd_turma).ToList();
                    List<Int32> listaCodigosAluno = politica.PoliticasAlunos.Select(p => p.cd_aluno).ToList();
                    pol = (from p in db.PoliticaDesconto
                           where p.PoliticasAlunos.Where(pa => listaCodigosAluno.Contains(pa.cd_aluno)).Any() &&
                           p.cd_pessoa_escola == politica.cd_pessoa_escola &&
                           p.PoliticasTurmas.Where(pa => listaCodigosTurma.Contains(pa.cd_turma)).Any() &&
                           p.dt_inicial_politica == politica.dt_inicial_politica &&
                           p.cd_politica_desconto != politica.cd_politica_desconto
                           select p.cd_politica_desconto).FirstOrDefault();
                }

                //Politica por Escola
                if ((politica.PoliticasTurmas == null || politica.PoliticasTurmas.Count() <= 0) &&
                   (politica.PoliticasAlunos == null || politica.PoliticasAlunos.Count() <= 0))
                    pol = (from p in db.PoliticaDesconto
                           where !p.PoliticasAlunos.Any() &&
                           p.cd_pessoa_escola == politica.cd_pessoa_escola &&
                           !p.PoliticasTurmas.Any() &&
                           p.dt_inicial_politica == politica.dt_inicial_politica &&
                           p.cd_politica_desconto != politica.cd_politica_desconto
                           select p.cd_politica_desconto).FirstOrDefault();

                return pol > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool verificaBaixaTituloByPolitica(int cd_politica_desconto, int cd_escola) {
            try {
                SGFWebContext dbContext = new SGFWebContext();
                int cd_origem = Int32.Parse(dbContext.LISTA_ORIGEM_LOGS["Contrato"].ToString());

                var sql = from p in db.PoliticaDesconto
                          where p.cd_politica_desconto == cd_politica_desconto
                                && p.cd_pessoa_escola == cd_escola
                                && p.BaixasTitulos.Any()
                          select p;
                return !sql.Any();
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }
    }
}
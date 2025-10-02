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
    public class AulaPersonalizadaDataAccess : GenericRepository<AulaPersonalizada>, IAulaPersonalizadaDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }
        public IEnumerable<AulaPersonalizadaUI> searchAulaPersonalizada(SearchParameters parametros, DateTime? dataIni, DateTime? dataFim, TimeSpan? hrInicial, TimeSpan? hrFinal, int? cdProduto, int? cdProfessor,
                                                                      int? cdSala, int? cdAluno, bool participou, int cdEscola)
        {
            try
            {
                var minDate = DateTime.MinValue;
                var maxDate = DateTime.MaxValue;

                IEntitySorter<AulaPersonalizadaUI> sorter = EntitySorter<AulaPersonalizadaUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<AulaPersonalizada> sql;
                IQueryable<AulaPersonalizadaAluno> sqlAulaPerAluno = db.AulaPersonalizadaAluno.AsNoTracking();
                sql = from aulaPersonalizada in db.AulaPersonalizada.AsNoTracking()
                      where aulaPersonalizada.cd_escola == cdEscola
                      select aulaPersonalizada;

                if (dataIni.HasValue)
                    sql = from a in sql
                          where a.dt_aula_personalizada >= dataIni.Value
                          select a;
                if (dataFim.HasValue)
                    sql = from a in sql
                          where a.dt_aula_personalizada <= dataFim.Value
                          select a;
                if (hrInicial.HasValue)
                    sql = from a in sql
                          where a.hh_inicial >= hrInicial.Value
                          select a;
                if (hrFinal.HasValue)
                    sql = from a in sql
                          where a.hh_final <= hrFinal.Value
                          select a;
                if(cdProduto > 0)
                    sql = from a in sql
                          where a.cd_produto == cdProduto.Value
                          select a;
                if (cdProfessor > 0)
                    sql = from a in sql
                          where a.Turma.TurmaProfessorTurma.Where(p => p.cd_professor == cdProfessor.Value).Any()
                          select a;
                if (cdSala > 0)
                    sql = from a in sql
                          where a.Turma.Sala.cd_sala == cdSala.Value
                          select a;
                if (cdAluno > 0)
                {
                    sql = from a in sql
                          where a.AulaPersonalizadaAlunos.Where(ap => ap.cd_aluno == cdAluno.Value).Any()
                          select a;
                    sqlAulaPerAluno = sqlAulaPerAluno.Where(ap => ap.cd_aluno == cdAluno.Value);
                }
                if (participou)
                {
                    sql = from a in sql
                          where a.AulaPersonalizadaAlunos.Where(ap => ((!cdAluno.HasValue || cdAluno <= 0) || ap.cd_aluno == cdAluno.Value) && ap.id_aula_dada).Any()
                          select a;
                    sqlAulaPerAluno = sqlAulaPerAluno.Where(ap => ((!cdAluno.HasValue || cdAluno <= 0) || ap.cd_aluno == cdAluno.Value) && ap.id_aula_dada);
                }
                
                var retorno = (from a in sql
                               join ap in sqlAulaPerAluno
                               on a.cd_aula_personalizada equals ap.cd_aula_personalizada
                               select new AulaPersonalizadaUI
                                    {
                                        cd_aula_personalizada = a.cd_aula_personalizada,
                                        dt_aula_personalizada = a.dt_aula_personalizada,
                                        hh_inicial = a.hh_inicial,
                                        hh_final = a.hh_final,
                                        cd_produto = a.cd_produto,
                                        cd_turma_personalizada = a.cd_turma_personalizada,
                                        no_aluno = ap.Aluno.AlunoPessoaFisica.no_pessoa,
                                        no_produto = a.Produto.no_produto,
                                        no_turma_personalizada = a.Turma.no_turma,
                                        no_sala = a.Turma.Sala.no_sala,
                                        no_professor = a.Turma.TurmaProfessorTurma.Where(p => p.id_professor_ativo).FirstOrDefault().Professor.FuncionarioPessoaFisica.no_pessoa,
                                        id_participou = (bool?)ap.id_aula_dada,
                                        cd_aula_personalizada_aluno = (int?)ap.cd_aula_personalizada_aluno,
                                        cd_aluno = ap.Aluno.cd_aluno
                                    });
                retorno = sorter.Sort(retorno);

                int limite = retorno.Count();

                parametros.ajustaParametrosPesquisa(limite);
                retorno = retorno.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;
                return retorno.ToList();

            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public IEnumerable<AulaPersonalizada> searchAulaPersonalizadaPesq(int cdAulaPersonalizada, int cdEscola)
        {
            IEnumerable<AulaPersonalizada> retorno = (from a in db.AulaPersonalizada
                                                      join al in db.AulaPersonalizadaAluno
                                                      on a.cd_aula_personalizada equals al.cd_aula_personalizada into x
                                                      from ap in x.DefaultIfEmpty()
                                                      where a.cd_aula_personalizada == cdAulaPersonalizada &&
                                                      a.cd_escola == cdEscola
                                                      select new
                                                      {
                                                          cd_aula_personalizada = a.cd_aula_personalizada,
                                                          ap.cd_aula_personalizada_aluno,
                                                          dt_aula_personalizada = a.dt_aula_personalizada,
                                                          hh_inicial = a.hh_inicial,
                                                          hh_final = a.hh_final,
                                                          cd_produto = a.cd_produto,
                                                          cd_turma_personalizada = a.cd_turma_personalizada,
                                                          no_aluno = ap.Aluno.AlunoPessoaFisica.no_pessoa,
                                                          no_produto = a.Produto.no_produto,
                                                          no_turma_personalizada = a.Turma.no_turma,
                                                          no_sala = a.Turma.Sala.no_sala,
                                                          no_professor = a.Turma.TurmaProfessorTurma.Where(p => p.id_professor_ativo).FirstOrDefault().Professor.FuncionarioPessoaFisica.no_pessoa,
                                                          id_participou = (bool?)ap.id_aula_dada,
                                                          cd_aluno = ap.Aluno.cd_aluno
                                                      }).ToList().Select(x => new AulaPersonalizada
                                                        {
                                                            cd_aula_personalizada = x.cd_aula_personalizada,
                                                            cd_aula_personalizada_aluno = x.cd_aula_personalizada_aluno,
                                                            dt_aula_personalizada = x.dt_aula_personalizada,
                                                            hh_inicial = x.hh_inicial,
                                                            hh_final = x.hh_final,
                                                            cd_produto = x.cd_produto,
                                                            cd_turma_personalizada = x.cd_turma_personalizada,
                                                            no_aluno = x.no_aluno,
                                                            no_produto = x.no_produto,
                                                            no_turma_personalizada = x.no_turma_personalizada,
                                                            no_sala = x.no_sala,
                                                            no_professor = x.no_professor,
                                                            id_participou = x.id_participou,
                                                            cd_aluno = x.cd_aluno

                                                        });
            return retorno;
        }

        public AulaPersonalizada searchAulaPersonalizadaById(int cdAulaPersonalizada, int cdEscola)
        {
            AulaPersonalizada retorno = (from a in db.AulaPersonalizada
                                         where a.cd_aula_personalizada == cdAulaPersonalizada &&
                                         a.cd_escola == cdEscola
                                         select new
                                         {
                                             cd_aula_personalizada = a.cd_aula_personalizada,
                                             dt_aula_personalizada = a.dt_aula_personalizada,
                                             hh_inicial = a.hh_inicial,
                                             hh_final = a.hh_final,
                                             cd_produto = a.cd_produto,
                                             cd_turma_personalizada = a.cd_turma_personalizada,
                                             no_produto = a.Produto.no_produto,
                                             no_turma_personalizada = a.Turma.no_turma,
                                             no_professor = a.Turma.TurmaProfessorTurma.Where(p => p.id_professor_ativo).FirstOrDefault().Professor.FuncionarioPessoaFisica.no_pessoa
                                         }).ToList().Select(x => new AulaPersonalizada
                                         {
                                             cd_aula_personalizada = x.cd_aula_personalizada,
                                             dt_aula_personalizada = x.dt_aula_personalizada,
                                             hh_inicial = x.hh_inicial,
                                             hh_final = x.hh_final,
                                             cd_produto = x.cd_produto,
                                             cd_turma_personalizada = x.cd_turma_personalizada,
                                             no_produto = x.no_produto,
                                             no_turma_personalizada = x.no_turma_personalizada,
                                             no_professor = x.no_professor
                                         }).FirstOrDefault();
            return retorno;
        }

        public IEnumerable<AulaPersonalizadaReport> getReportAulaPersonalizada(int cd_empresa, int cd_aluno, int? cd_produto, int? cd_curso, DateTime? dt_inicial_agend, DateTime? dt_final_agend,
                DateTime? dt_inicial_lanc, DateTime? dt_final_lanc, TimeSpan? hr_inicial_agend, TimeSpan? hr_final_agend, TimeSpan? hr_inicial_lanc, TimeSpan? hr_final_lanc)
        {
            try
            {
                IQueryable<AulaPersonalizadaAluno> sql = from apa in db.AulaPersonalizadaAluno
                                             where apa.AulaPersonalizada.cd_escola == cd_empresa 
                                                && apa.cd_aluno == cd_aluno
                                             select apa;
                
                if(cd_produto.HasValue)
                    sql = from apa in sql
                          where apa.AulaPersonalizada.cd_produto == cd_produto.Value
                    select apa;

                if(cd_curso.HasValue)
                    sql = from apa in sql
                          where apa.DiarioAula.Turma.cd_curso == cd_curso.Value
                    select apa;

                if(dt_inicial_agend.HasValue)
                    sql = from apa in sql
                          where apa.AulaPersonalizada.dt_aula_personalizada >= dt_inicial_agend.Value
                    select apa;
               
                if(dt_final_agend.HasValue)
                    sql = from apa in sql
                          where apa.AulaPersonalizada.dt_aula_personalizada <= dt_final_agend.Value
                    select apa;

                if(hr_inicial_agend.HasValue)
                    sql = from apa in sql
                          where apa.AulaPersonalizada.hh_inicial >= hr_inicial_agend.Value
                    select apa;
               
                if(hr_final_agend.HasValue)
                    sql = from apa in sql
                          where apa.AulaPersonalizada.hh_final <= hr_final_agend.Value
                    select apa;

                if(dt_inicial_lanc.HasValue)
                    sql = from apa in sql
                          where apa.DiarioAula.dt_aula >= dt_inicial_lanc.Value
                    select apa;
               
                if(dt_final_lanc.HasValue)
                    sql = from apa in sql
                          where apa.DiarioAula.dt_aula <= dt_final_lanc.Value
                    select apa;

                if(hr_inicial_lanc.HasValue)
                    sql = from apa in sql
                          where apa.hh_inicial_aluno >= hr_inicial_lanc.Value
                    select apa;
               
                if(hr_final_lanc.HasValue)
                    sql = from apa in sql
                          where apa.hh_final_aluno <= hr_final_lanc.Value
                    select apa;

                var retorno = (from apa in sql
                    select new
                    {
                        cd_turma = apa.DiarioAula.cd_turma,
                        cd_aula_personalizada_aluno = apa.cd_aula_personalizada_aluno,
                        nm_aula_programacao_turma = apa.DiarioAula.ProgramacaoTurma.nm_aula_programacao_turma,
                        dta_programacao_turma = apa.DiarioAula.ProgramacaoTurma.dta_programacao_turma,
                        dc_programacao_turma = apa.DiarioAula.ProgramacaoTurma.dc_programacao_turma,
                        tx_obs_aula = apa.tx_obs_aula,
                        dt_aula = apa.DiarioAula.dt_aula,
                        hh_inicial_aluno = apa.hh_inicial_aluno,
                        hh_final_aluno = apa.hh_final_aluno,
                        no_turma = apa.AulaPersonalizada.Turma.no_turma ,
                        no_turma_original = apa.DiarioAula.Turma.no_turma,
                        id_turma_ppt = apa.AulaPersonalizada.Turma.id_turma_ppt ,
                        //lista_no_turma_original = (from t in db.Turma
                        //                            where t.cd_turma_ppt == apa.AulaPersonalizada.Turma.cd_turma
                        //                            select t.no_turma).FirstOrDefault(),
                        no_pessoa = apa.DiarioAula.Professor.FuncionarioPessoaFisica.no_pessoa,
                        no_sala = apa.DiarioAula.Turma.Sala.no_sala,
                        id_aula_dada = apa.id_aula_dada
                    }).ToList().Select(x => new AulaPersonalizadaReport
                    {
                        cd_aula_personalizada_aluno = x.cd_aula_personalizada_aluno,
                        cd_turma = x.cd_turma,
                        nm_aula_programacao_turma = x.nm_aula_programacao_turma,
                        dta_programacao_turma = x.dta_programacao_turma,
                        dc_programacao_turma = x.dc_programacao_turma,
                        tx_obs_aula = x.tx_obs_aula,
                        dt_aula = x.dt_aula,
                        hh_inicial_aluno = x.hh_inicial_aluno,
                        hh_final_aluno = x.hh_final_aluno,
                        no_turma = x.no_turma,
                        no_pessoa = x.no_pessoa,
                        no_sala = x.no_sala,
                        id_aula_dada = x.id_aula_dada,
                        no_turma_original = x.no_turma_original
                    });

                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}

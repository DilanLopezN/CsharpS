using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Services.Coordenacao.Model;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess
{
    public class AlunoAulaReposicaoDataAccess : GenericRepository<AlunoAulaReposicao>, IAlunoAulaReposicaoDataAccess
    {
        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }

        }


        public IEnumerable<AlunoAulaReposicaoUI> getAlunosTurmaAulaReposicao(int cd_turma, int cd_pessoa_escola, DateTime? dt_inicial, DateTime dt_final, int cd_aula_reposicao)
        {
            try
            {
                IEnumerable<AlunoAulaReposicaoUI> sql;
                DateTime data_inicial = DateTime.MinValue;

                if (cd_aula_reposicao > 0)
                {

                    sql = (from a in db.Aluno
                           orderby a.AlunoPessoaFisica.no_pessoa
                           where a.cd_pessoa_escola == cd_pessoa_escola &&
                                 (a.AlunoTurma.Where(x => x.cd_turma == cd_turma || x.Turma.cd_turma_ppt == cd_turma)
                                      .FirstOrDefault().nm_faltas > 0) &&
                                 a.AlunoTurma.Any(x =>
                                     (x.cd_turma == cd_turma || x.Turma.cd_turma_ppt == cd_turma) //&&
                                     //(situacao_aluno == AlunoTurma.FiltroSituacaoAlunoTurma.Nao_Encerrado
                                     //    ? x.Turma.dt_termino_turma == null
                                     //    : true)
                                     ) &&
                                 a.HistoricoAluno.Where(ha =>
                                     (ha.cd_turma == cd_turma || ha.Turma.cd_turma_ppt == cd_turma) &&
                                     (DbFunctions.TruncateTime(ha.dt_historico) <= dt_final.Date &&
                                      (ha.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                       ha.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado)
                                      && ha.nm_sequencia == a.HistoricoAluno.Where(han =>
                                              (han.cd_turma == cd_turma || han.Turma.cd_turma_ppt == cd_turma)
                                              && DbFunctions.TruncateTime(han.dt_historico) <= dt_final.Date)
                                          .Max(x => x.nm_sequencia)
                                     )
                                 ).Any()
                           select new AlunoAulaReposicaoUI
                           {
                               //cd_aluno_aula_reposicao = a.AlunoAulaReposicao.Where(x => x.cd_aluno == a.cd_aluno && x.cd_aula_reposicao == cd_aula_reposicao).FirstOrDefault().cd_aluno_aula_reposicao,
                               cd_aula_reposicao = 0,
                               cd_aluno = a.cd_aluno,
                               id_participacao = a.AlunoAulaReposicao.Where(x => x.cd_aluno == a.cd_aluno && x.cd_aula_reposicao == cd_aula_reposicao).FirstOrDefault().id_participacao,
                               tx_observacao_aluno_aula = a.AlunoAulaReposicao.Where(x => x.cd_aluno == a.cd_aluno && x.cd_aula_reposicao == cd_aula_reposicao).FirstOrDefault().tx_observacao_aluno_aula,
                               no_aluno = a.AlunoPessoaFisica.no_pessoa
   
                           });

                }
                else
                {
                    sql = (from a in db.Aluno
                           orderby a.AlunoPessoaFisica.no_pessoa
                           where a.cd_pessoa_escola == cd_pessoa_escola &&
                                 (a.AlunoTurma.Where(x => x.cd_turma == cd_turma || x.Turma.cd_turma_ppt == cd_turma)
                                     .FirstOrDefault().nm_faltas > 0) &&
                                 a.AlunoTurma.Any(x =>
                                     (x.cd_turma == cd_turma || x.Turma.cd_turma_ppt == cd_turma) /*&&
                                     (situacao_aluno == AlunoTurma.FiltroSituacaoAlunoTurma.Nao_Encerrado
                                         ? x.Turma.dt_termino_turma == null
                                         : true)*/) &&
                                 a.HistoricoAluno.Where(ha =>
                                     (ha.cd_turma == cd_turma || ha.Turma.cd_turma_ppt == cd_turma) &&
                                     (DbFunctions.TruncateTime(ha.dt_historico) <= dt_final.Date &&
                                      (ha.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                       ha.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado)
                                      && ha.nm_sequencia == a.HistoricoAluno.Where(han =>
                                              (han.cd_turma == cd_turma || han.Turma.cd_turma_ppt == cd_turma)
                                              && DbFunctions.TruncateTime(han.dt_historico) <= dt_final.Date)
                                          .Max(x => x.nm_sequencia)
                                     )
                                 ).Any()
                           select new AlunoAulaReposicaoUI
                           {
                               cd_aula_reposicao = 0,
                               cd_aluno = a.cd_aluno,
                               id_participacao = a.AlunoAulaReposicao.Where(x => x.cd_aluno == a.cd_aluno && x.cd_aula_reposicao == cd_aula_reposicao).FirstOrDefault().id_participacao,
                               tx_observacao_aluno_aula = a.AlunoAulaReposicao.Where(x => x.cd_aluno == a.cd_aluno && x.cd_aula_reposicao == cd_aula_reposicao).FirstOrDefault().tx_observacao_aluno_aula,
                               no_aluno = a.AlunoPessoaFisica.no_pessoa
                           });
                }


                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public AlunoAulaReposicaoUI getAlunoAulaReposicao(int cd_turma, int cd_pessoa_escola, int cd_aluno, DateTime? dt_inicial, DateTime dt_final)
        {
            try
            {
                IEnumerable<AlunoAulaReposicaoUI> sql;
                DateTime data_inicial = DateTime.MinValue;


                sql = (from a in db.Aluno
                       orderby a.AlunoPessoaFisica.no_pessoa
                       where a.cd_pessoa_escola == cd_pessoa_escola && a.cd_aluno == cd_aluno &&
                             (a.AlunoTurma.Where(x => x.cd_turma == cd_turma || x.Turma.cd_turma_ppt == cd_turma)
                                 .FirstOrDefault().nm_faltas > 0) &&
                             a.AlunoTurma.Any(x =>
                                 (x.cd_turma == cd_turma || x.Turma.cd_turma_ppt == cd_turma) /*&&
                                 (situacao_aluno == AlunoTurma.FiltroSituacaoAlunoTurma.Todos
                                     ? x.Turma.dt_termino_turma == null
                                     : true)*/) &&
                             a.HistoricoAluno.Where(ha =>
                                 (ha.cd_turma == cd_turma || ha.Turma.cd_turma_ppt == cd_turma) &&
                                 (DbFunctions.TruncateTime(ha.dt_historico) <= dt_final.Date
                                  && ha.nm_sequencia == a.HistoricoAluno.Where(han =>
                                          (han.cd_turma == cd_turma || han.Turma.cd_turma_ppt == cd_turma)
                                          && DbFunctions.TruncateTime(han.dt_historico) <= dt_final.Date)
                                      .Max(x => x.nm_sequencia)
                                 )
                             ).Any()
                       select new AlunoAulaReposicaoUI
                       {
                           cd_aula_reposicao = 0,
                           cd_aluno = a.cd_aluno,
                           id_participacao = Convert.ToByte(false),
                           tx_observacao_aluno_aula = "",
                           no_aluno = a.AlunoPessoaFisica.no_pessoa
                       });

                return sql.FirstOrDefault();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }


        public AlunoAulaReposicao getAlunoAulaReposicaoByCdItem(int cd_aluno_aula_reposicao)
        {
            try
            {
                var sql = (from c in db.AlunoAulaReposicao
                    where c.cd_aluno_aula_reposicao == cd_aluno_aula_reposicao
                    select c).FirstOrDefault();

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public List<AlunoAulaReposicaoUI> getAlunosAulaReposicao(int cd_aula_reposicao)
        {
            try
            {
                var sql = (from a in db.AlunoAulaReposicao
                    join at in db.AlunoTurma on a.cd_aluno equals at.cd_aluno
                    join c in db.AulaReposicao on a.cd_aula_reposicao equals c.cd_aula_reposicao
                    where a.cd_aula_reposicao == cd_aula_reposicao
                    select new AlunoAulaReposicaoUI
                    {
                        cd_aluno_aula_reposicao = a.cd_aluno_aula_reposicao,
                        cd_aula_reposicao = a.cd_aula_reposicao,
                        cd_aluno = a.cd_aluno,
                        id_participacao = a.id_participacao,
                        tx_observacao_aluno_aula = a.tx_observacao_aluno_aula,
                        no_aluno = at.Aluno.AlunoPessoaFisica.no_pessoa,
                        no_pessoa = at.Aluno.AlunoPessoaFisica.no_pessoa
                    }).Distinct().ToList();

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        //Retorna um inteiro com a quantidade de alunos com atividade
        public long retornNumbersOfStudents(int idAulaReposicao)
        {
            try
            {
                long sql = (from al in db.AlunoAulaReposicao
                    join a in db.AulaReposicao on al.cd_aula_reposicao equals a.cd_aula_reposicao
                    where al.cd_aula_reposicao == idAulaReposicao
                    select al.cd_aula_reposicao).Count();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<AlunoAulaReposicaoUI> searchAlunoAulaReposicao(int cd_aula_reposicao, int cdEscola)
        {
            try
            {
                var sql = from alunoAulaReposicao in db.AlunoAulaReposicao
                    join aluno in db.Aluno on alunoAulaReposicao.cd_aluno equals aluno.cd_aluno
                    join pessoa in db.PessoaSGF on aluno.cd_pessoa_aluno equals pessoa.cd_pessoa
                    where alunoAulaReposicao.cd_aula_reposicao == cd_aula_reposicao
                          && aluno.cd_pessoa_escola == cdEscola
                    select new AlunoAulaReposicaoUI
                    {
                        cd_aluno_aula_reposicao = alunoAulaReposicao.cd_aluno_aula_reposicao,
                        cd_aula_reposicao = alunoAulaReposicao.cd_aula_reposicao,
                        cd_aluno = alunoAulaReposicao.cd_aluno,
                        id_participacao = alunoAulaReposicao.id_participacao,
                        tx_observacao_aluno_aula = alunoAulaReposicao.tx_observacao_aluno_aula,
                        no_aluno = alunoAulaReposicao.Aluno.AlunoPessoaFisica.no_pessoa,
                        no_pessoa = alunoAulaReposicao.Aluno.AlunoPessoaFisica.no_pessoa
                    };
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}
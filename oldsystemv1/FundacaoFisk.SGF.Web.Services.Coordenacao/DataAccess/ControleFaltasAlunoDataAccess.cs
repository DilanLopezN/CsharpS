using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Services.Coordenacao.Comum.IDataAccess;
using FundacaoFisk.SGF.Services.Coordenacao.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace FundacaoFisk.SGF.Services.Coordenacao.DataAccess
{
    public class ControleFaltasAlunoDataAccess : GenericRepository<ControleFaltasAluno>, IControleFaltasAlunoDataAccess
    {
        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }

        }


        public IEnumerable<ControleFaltasAlunoUI> getAlunosTurmaControleFalta(int cd_turma, int cd_pessoa_escola, DateTime? dt_inicial, DateTime dt_final, int cd_controle_faltas, AlunoTurma.FiltroSituacaoAlunoTurma situacao_aluno)
        {
            try
            {
                IEnumerable<ControleFaltasAlunoUI> sql;
                DateTime data_inicial = DateTime.MinValue;

                if (cd_controle_faltas > 0)
                {

                    sql = (from a in db.Aluno
                           orderby a.AlunoPessoaFisica.no_pessoa
                           where a.cd_pessoa_escola == cd_pessoa_escola &&
                                 (a.AlunoTurma.Where(x => x.cd_turma == cd_turma || x.Turma.cd_turma_ppt == cd_turma)
                                      .FirstOrDefault().nm_faltas > 0) &&
                                 a.AlunoTurma.Any(x =>
                                     (x.cd_turma == cd_turma || x.Turma.cd_turma_ppt == cd_turma) &&
                                     (situacao_aluno == AlunoTurma.FiltroSituacaoAlunoTurma.Nao_Encerrado
                                         ? x.Turma.dt_termino_turma == null
                                         : true)) &&
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
                           select new ControleFaltasAlunoUI
                           {
                               cd_aluno = a.cd_aluno,
                               cd_controle_faltas = 0,
                               cd_situacao_aluno_turma = a.AlunoTurma.Where(x => x.cd_turma == cd_turma || x.Turma.cd_turma_ppt == cd_turma)
                                   .FirstOrDefault().cd_situacao_aluno_turma,
                               id_assinatura = a.T_CONTROLE_FALTAS_ALUNO.Where(x => x.cd_aluno == a.cd_aluno && x.cd_controle_faltas == cd_controle_faltas).FirstOrDefault().id_assinatura,
                               no_aluno = a.AlunoPessoaFisica.no_pessoa,
                               nm_faltas = a.AlunoTurma.Where(x => x.cd_turma == cd_turma || x.Turma.cd_turma_ppt == cd_turma)
                                   .FirstOrDefault().nm_faltas
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
                                     (x.cd_turma == cd_turma || x.Turma.cd_turma_ppt == cd_turma) &&
                                     (situacao_aluno == AlunoTurma.FiltroSituacaoAlunoTurma.Nao_Encerrado
                                         ? x.Turma.dt_termino_turma == null
                                         : true)) &&
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
                           select new ControleFaltasAlunoUI
                           {
                               cd_aluno = a.cd_aluno,
                               cd_controle_faltas = 0,
                               cd_situacao_aluno_turma = a.AlunoTurma.Where(x => x.cd_turma == cd_turma || x.Turma.cd_turma_ppt == cd_turma)
                                   .FirstOrDefault().cd_situacao_aluno_turma,
                               id_assinatura = false,
                               no_aluno = a.AlunoPessoaFisica.no_pessoa,
                               nm_faltas = a.AlunoTurma.Where(x => x.cd_turma == cd_turma || x.Turma.cd_turma_ppt == cd_turma)
                                   .FirstOrDefault().nm_faltas
                           });
                }


                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public ControleFaltasAlunoUI getAlunoControleFalta(int cd_turma, int cd_pessoa_escola, int cd_aluno, DateTime? dt_inicial, DateTime dt_final, AlunoTurma.FiltroSituacaoAlunoTurma situacao_aluno)
        {
            try
            {
                IEnumerable<ControleFaltasAlunoUI> sql;
                DateTime data_inicial = DateTime.MinValue;


                sql = (from a in db.Aluno
                       orderby a.AlunoPessoaFisica.no_pessoa
                       where a.cd_pessoa_escola == cd_pessoa_escola && a.cd_aluno == cd_aluno &&
                             (a.AlunoTurma.Where(x => x.cd_turma == cd_turma || x.Turma.cd_turma_ppt == cd_turma)
                                 .FirstOrDefault().nm_faltas > 0) &&
                             a.AlunoTurma.Any(x =>
                                 (x.cd_turma == cd_turma || x.Turma.cd_turma_ppt == cd_turma) &&
                                 (situacao_aluno == AlunoTurma.FiltroSituacaoAlunoTurma.Todos
                                     ? x.Turma.dt_termino_turma == null
                                     : true)) &&
                             a.HistoricoAluno.Where(ha =>
                                 (ha.cd_turma == cd_turma || ha.Turma.cd_turma_ppt == cd_turma) &&
                                 (DbFunctions.TruncateTime(ha.dt_historico) <= dt_final.Date
                                  && ha.nm_sequencia == a.HistoricoAluno.Where(han =>
                                          (han.cd_turma == cd_turma || han.Turma.cd_turma_ppt == cd_turma)
                                          && DbFunctions.TruncateTime(han.dt_historico) <= dt_final.Date)
                                      .Max(x => x.nm_sequencia)
                                 )
                             ).Any()
                       select new ControleFaltasAlunoUI
                       {
                           cd_aluno = a.cd_aluno,
                           cd_controle_faltas = 0,
                           cd_situacao_aluno_turma = a.AlunoTurma.Where(x => x.cd_turma == cd_turma || x.Turma.cd_turma_ppt == cd_turma)
                               .FirstOrDefault().cd_situacao_aluno_turma,
                           id_assinatura = false,
                           no_aluno = a.AlunoPessoaFisica.no_pessoa,
                           nm_faltas = a.AlunoTurma.Where(x => x.cd_turma == cd_turma || x.Turma.cd_turma_ppt == cd_turma)
                               .FirstOrDefault().nm_faltas
                       });

                return sql.FirstOrDefault();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }


        public ControleFaltasAluno getAlunoControleEstoqueByCdItem(int cd_alunoControleFaltas)
        {
            try
            {
                var sql = (from c in db.ControleFaltasAluno
                           where c.cd_controle_faltas_aluno == cd_alunoControleFaltas
                    select c).FirstOrDefault();

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }


        public List<ControleFaltasAlunoUI> getAlunosControleFalta(int cd_controle_faltas)
        {
            try
            {
                var sql = (from ca in db.ControleFaltasAluno
                            join at in db.AlunoTurma on ca.cd_aluno equals  at.cd_aluno
                           join c in db.ControleFaltas on ca.cd_controle_faltas equals c.cd_controle_faltas
                           where ca.cd_controle_faltas == cd_controle_faltas
                           select new ControleFaltasAlunoUI
                    {
                            cd_controle_faltas_aluno = ca.cd_controle_faltas_aluno,
                            cd_controle_faltas = ca.cd_controle_faltas,
                            cd_aluno = ca.cd_aluno,
                            cd_situacao_aluno_turma = ca.cd_situacao_aluno_turma,
                            nm_faltas = ca.nm_faltas,
                            id_assinatura =  ca.id_assinatura,
                            no_aluno = at.Aluno.AlunoPessoaFisica.no_pessoa
       
                    }).Distinct().ToList();

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        
    }
}
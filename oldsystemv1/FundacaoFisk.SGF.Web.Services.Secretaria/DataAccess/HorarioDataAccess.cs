using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Secretaria.Comum.IDataAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess
{
    public class HorarioDataAccess  : GenericRepository<Horario>, IHorarioDataAccess
    {
        
        public enum TipoConsultaHorario
        {
            HAS_HORARIO_ORIGEM = 0,
            HAS_HORARIO_SALA_OCUPADO_TURMA = 1,
            HAS_HORARIO_SALA_ATIVIDA_EXT = 2,
            HAS_HORARIO_PROF_OCUPADO_TURMA = 3,
            HAS_HORARIO_PROF_OCUPADO_ATIV_EXTRA = 4,
            HAS_HORARIO_ALUNO_OCUPADO_TURMA = 5
        }


        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }

        public IEnumerable<Horario> getHorarioByEscolaForRegistro(int cdEscola, int cdRegistro, Horario.Origem origem)
        {
            try
            {
                var sql = (from horario in db.Horario
                          where (cdEscola == 0 || horario.cd_pessoa_escola == cdEscola ||
                                (from te in db.TurmaEscola
                                    where te.cd_turma == horario.cd_registro &&
                                        te.cd_escola == cdEscola
                                    select te).Any())
                               && horario.id_origem == (int)origem &&
                                 (cdRegistro == 0 || horario.cd_registro == cdRegistro)
                          orderby horario.id_dia_semana, horario.dt_hora_ini, horario.dt_hora_fim
                          select new {
                              cd_horario = horario.cd_horario,
                              cd_pessoa_escola = horario.cd_pessoa_escola,
                              cd_registro = horario.cd_registro,
                              id_origem = horario.id_origem,
                              id_disponivel = horario.id_disponivel,
                              id_dia_semana = horario.id_dia_semana,
                              dt_hora_ini = horario.dt_hora_ini,
                              dt_hora_fim = horario.dt_hora_fim,
                              HorariosProfessores = horario.HorariosProfessores
                          }).ToList().Select(x => new Horario
                            {
                                cd_horario = x.cd_horario,
                                cd_pessoa_escola = x.cd_pessoa_escola,
                                cd_registro = x.cd_registro,
                                id_origem = x.id_origem,
                                id_disponivel = x.id_disponivel,
                                id_dia_semana = x.id_dia_semana,
                                dt_hora_ini = x.dt_hora_ini,
                                dt_hora_fim = x.dt_hora_fim,
                                HorariosProfessores = x.HorariosProfessores
                            });

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool getHorarioByHorario(int cdEscola, int cdRegistro, Horario.Origem origem, TimeSpan hr_servidor, int diaSemanaAtual)
        {
            try
            {
                var sql = (from horario in db.Horario
                          where (cdEscola == 0 || horario.cd_pessoa_escola == cdEscola) && horario.id_origem == (int)origem &&
                                 (cdRegistro == 0 || horario.cd_registro == cdRegistro) &&
                                 (horario.dt_hora_ini <= hr_servidor && horario.dt_hora_fim >= hr_servidor) &&
                                 (horario.id_dia_semana == diaSemanaAtual)
                          orderby horario.id_dia_semana, horario.dt_hora_ini, horario.dt_hora_fim
                          select horario).FirstOrDefault();

                return sql != null && sql.cd_horario > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Horario> getHorarioOcupadosForTurma(int cdEscola, int cdRegistro, int[] cdProfessores, int cd_turma,
            int cd_duracao, int cd_curso, DateTime dt_inicio, DateTime? dt_final, HorarioDataAccess.TipoConsultaHorario tipoCons)
        {
            DateTime now = DateTime.Now;
            TimeSpan time = now.TimeOfDay;
            now = now.Date;
            decimal nm_duracao = 0;
            short? nm_carga_horaria = 0;
            DateTime? dt_final_carga = dt_final;
            if (cd_duracao > 0 && cd_curso > 0)
            {
                nm_duracao = (from d in db.Duracao
                                      where d.cd_duracao == cd_duracao
                                      select d.nm_duracao).FirstOrDefault();
                nm_carga_horaria = (from c in db.Curso
                                           where c.cd_curso == cd_curso
                                           select c.nm_carga_horaria).FirstOrDefault();

                dt_final_carga = dt_final == null ? dt_inicio.AddDays(nm_duracao == 0 ? 0 : (int)(nm_carga_horaria == null ? 0 : nm_carga_horaria / (double)nm_duracao * 7.0)) : dt_final;
            }
            else
                dt_final_carga = dt_final == null ? dt_inicio.AddDays(120) : dt_final;
            try
            {
                IEnumerable<Horario> sql = null;
                switch (tipoCons)
                {
                    // Busca os horários ocupados de outras turmas para uma sala numa mesma escola:
                    case TipoConsultaHorario.HAS_HORARIO_SALA_OCUPADO_TURMA:
                        sql = (from horario in db.Horario
                              join t in db.Turma on horario.cd_registro equals t.cd_turma
                              where horario.cd_pessoa_escola == cdEscola && horario.id_origem == (int)Horario.Origem.TURMA &&
                                    t.cd_sala == cdRegistro && t.cd_pessoa_escola == cdEscola &&
                                    (cd_turma == 0 || t.cd_turma != cd_turma) &&
                                    t.dt_termino_turma == null && t.cd_turma_ppt == null &&
                                     ((t.dt_final_aula == null && DbFunctions.AddDays(t.dt_inicio_aula, t.Duracao.nm_duracao == 0 ? 0 : (int)(t.Curso.nm_carga_horaria / (double)t.Duracao.nm_duracao * 7.0)) >= dt_inicio) ||
                                      (t.dt_final_aula != null && t.dt_final_aula >= dt_inicio)) &&
                                      t.dt_inicio_aula <= (dt_final_carga == null ? t.dt_inicio_aula : dt_final_carga)
                               select new
                               {
                                   no_registro = t.no_turma,
                                   cd_pessoa_escola = horario.cd_pessoa_escola,
                                   cd_registro = horario.cd_registro,
                                   id_origem = horario.id_origem,
                                   id_disponivel = horario.id_disponivel,
                                   id_dia_semana = horario.id_dia_semana,
                                   dt_hora_ini = horario.dt_hora_ini,
                                   dt_hora_fim = horario.dt_hora_fim
                               }).ToList().Select(x => new Horario
                               {
                                   no_registro = x.no_registro,
                                   cd_pessoa_escola = x.cd_pessoa_escola,
                                   cd_registro = x.cd_registro,
                                   id_origem = x.id_origem,
                                   id_disponivel = x.id_disponivel,
                                   id_dia_semana = x.id_dia_semana,
                                   dt_hora_ini = x.dt_hora_ini,
                                   dt_hora_fim = x.dt_hora_fim
                               });
                        break;
                    // Busca os horários ocupados de um determinado professor para outras turmas:
                    case TipoConsultaHorario.HAS_HORARIO_PROF_OCUPADO_TURMA:
                        sql = (from horario in db.Horario
                               join t in db.Turma on horario.cd_registro equals t.cd_turma
                               where horario.id_origem == (int)Horario.Origem.TURMA &&
                                ((!t.id_turma_ppt && t.dt_termino_turma == null) || (t.id_turma_ppt && t.id_turma_ativa)) && 
                                t.cd_pessoa_escola == cdEscola &&
                                t.cd_turma != cd_turma && t.cd_turma_ppt == null &&
                                t.TurmaProfessorTurma.Where(p => cdProfessores.Contains(p.cd_professor) && p.id_professor_ativo).Any() &&
                                horario.HorariosProfessores.Where(h => cdProfessores.Contains(h.cd_professor)).Any() &&
                                ((t.id_turma_ppt) ||
                                ((t.dt_final_aula == null && DbFunctions.AddDays(t.dt_inicio_aula, t.Duracao.nm_duracao == 0 ? 0 : (int)(t.Curso.nm_carga_horaria / (double)t.Duracao.nm_duracao * 7.0)) >= dt_inicio) ||
                                (t.dt_final_aula != null && t.dt_final_aula >= dt_inicio)) &&
                                t.dt_inicio_aula <= (dt_final_carga == null ? t.dt_inicio_aula : dt_final_carga))
                               select new
                               {
                                   no_registro = t.no_turma,
                                   //cd_horario = horario.cd_horario,
                                   cd_pessoa_escola = horario.cd_pessoa_escola,
                                   cd_registro = horario.cd_registro,
                                   id_origem = horario.id_origem,
                                   id_disponivel = horario.id_disponivel,
                                   id_dia_semana = horario.id_dia_semana,
                                   dt_hora_ini = horario.dt_hora_ini,
                                   dt_hora_fim = horario.dt_hora_fim,
                                   HorariosProfessores = horario.HorariosProfessores.Where(h => cdProfessores.Contains(h.cd_professor))
                               }).ToList().Select(x => new Horario
                               {
                                   no_registro = x.no_registro,
                                   //cd_horario = x.cd_horario,
                                   cd_pessoa_escola = x.cd_pessoa_escola,
                                   cd_registro = x.cd_registro,
                                   id_origem = x.id_origem,
                                   id_disponivel = x.id_disponivel,
                                   id_dia_semana = x.id_dia_semana,
                                   dt_hora_ini = x.dt_hora_ini,
                                   dt_hora_fim = x.dt_hora_fim,
                                   HorariosProfessores = x.HorariosProfessores.ToList().Select(y => new HorarioProfessorTurma()
                                   {
                                       cd_horario_professor_turma = y.cd_horario_professor_turma,
                                       cd_horario = y.cd_horario,
                                       cd_professor = y.cd_professor
                                   }).ToList()
                               });
                        break;
                    // Busca os horários ocupados pelo aluno em tumas e turmas ppt pai:
                    case TipoConsultaHorario.HAS_HORARIO_ALUNO_OCUPADO_TURMA:
                        sql = (from h in db.Horario
                               join turma in db.Turma on h.cd_registro equals turma.cd_turma
                               join turmaAluno in db.AlunoTurma on turma.cd_turma equals turmaAluno.cd_turma
                               where h.id_origem == (int)Horario.Origem.TURMA &&
                                     turma.dt_termino_turma == null &&
                                     (turmaAluno.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo || 
                                      turmaAluno.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado || 
                                      turmaAluno.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Aguardando) &&
                                     turma.id_turma_ppt == false &&
                                     h.cd_pessoa_escola == cdEscola &&
                                     turmaAluno.cd_aluno == cdRegistro &&
                                     ((turma.dt_final_aula == null && DbFunctions.AddDays(turma.dt_inicio_aula, turma.Duracao.nm_duracao == 0 ? 0 : (int)(turma.Curso.nm_carga_horaria / (double)turma.Duracao.nm_duracao * 7.0)) >= dt_inicio) ||
                                      (turma.dt_final_aula != null && turma.dt_final_aula >= dt_inicio)) &&
                                      turma.dt_inicio_aula <= (dt_final_carga == null ? turma.dt_inicio_aula : dt_final_carga)
                               select new
                               {
                                   no_registro = turma.no_turma,
                                   cd_horario = h.cd_horario,
                                   cd_pessoa_escola = h.cd_pessoa_escola,
                                   cd_registro = h.cd_registro,
                                   id_origem = h.id_origem,
                                   id_disponivel = h.id_disponivel,
                                   id_dia_semana = h.id_dia_semana,
                                   dt_hora_ini = h.dt_hora_ini,
                                   dt_hora_fim = h.dt_hora_fim,
                                   id_situacao = turmaAluno.cd_situacao_aluno_turma
                               }).ToList().Select(x => new Horario
                               {
                                   no_registro = x.no_registro,
                                   cd_horario = x.cd_horario,
                                   cd_pessoa_escola = x.cd_pessoa_escola,
                                   cd_registro = x.cd_registro,
                                   id_origem = x.id_origem,
                                   id_disponivel = x.id_disponivel,
                                   id_dia_semana = x.id_dia_semana,
                                   dt_hora_ini = x.dt_hora_ini,
                                   dt_hora_fim = x.dt_hora_fim,
                                   id_situacao = x.id_situacao
                               });
                        break;
                    // Busca os horários ocupados por atividades extras numa sala:
                    case TipoConsultaHorario.HAS_HORARIO_SALA_ATIVIDA_EXT:
                        sql = (from atExt in db.AtividadeExtra
                               where atExt.cd_sala == cdRegistro && atExt.cd_pessoa_escola == cdEscola &&
                               (DbFunctions.TruncateTime(atExt.dt_atividade_extra) > DbFunctions.TruncateTime(now)
                                                 || (DbFunctions.TruncateTime(atExt.dt_atividade_extra) == DbFunctions.TruncateTime(now) && atExt.hh_final >= time))
                               select new
                               {
                                   cd_registro = atExt.cd_atividade_extra,
                                   dt_hora_ini = atExt.hh_inicial,
                                   dt_hora_fim = atExt.hh_final,
                                   dt_atividade_extra = atExt.dt_atividade_extra,
                                   id_disponivel = true

                               }).ToList().Select(x => new Horario
                               {
                                   cd_registro = x.cd_registro,
                                   dt_hora_ini = x.dt_hora_ini,
                                   dt_hora_fim = x.dt_hora_fim,
                                   id_dia_semana = (byte)(x.dt_atividade_extra.DayOfWeek + 1),
                                   id_disponivel = x.id_disponivel
                               });
                        break;
                    // Busca os horários ocupados pelas atividades extras de um determinado professor:
                    case TipoConsultaHorario.HAS_HORARIO_PROF_OCUPADO_ATIV_EXTRA:

                        sql = (from atExt in db.AtividadeExtra
                               where cdProfessores.Contains(atExt.cd_funcionario) && atExt.cd_pessoa_escola == cdEscola &&
                                         (DbFunctions.TruncateTime(atExt.dt_atividade_extra) > DbFunctions.TruncateTime(now)
                                                || (DbFunctions.TruncateTime(atExt.dt_atividade_extra) == DbFunctions.TruncateTime(now) && atExt.hh_final >= time))
                               select new
                               {
                                   cd_registro = atExt.cd_atividade_extra,
                                   dt_hora_ini = atExt.hh_inicial,
                                   dt_hora_fim = atExt.hh_final,
                                   dt_atividade_extra = atExt.dt_atividade_extra,
                                   id_disponivel = true

                               }).ToList().Select(x => new Horario
                               {
                                   cd_registro = x.cd_registro,
                                   dt_hora_ini = x.dt_hora_ini,
                                   dt_hora_fim = x.dt_hora_fim,
                                   id_dia_semana = (byte)(x.dt_atividade_extra.DayOfWeek + 1),
                                   id_disponivel = x.id_disponivel
                               });
                        break;
                }

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Horario> getHorarioOcupadosForSala(Turma turma, int cdEscola,  HorarioDataAccess.TipoConsultaHorario tipoCons)
        {
            DateTime now = DateTime.Now;
            TimeSpan time = now.TimeOfDay;
            now = now.Date;
            decimal nm_duracao = (from d in db.Duracao
                                  where d.cd_duracao == turma.cd_duracao
                                  select d.nm_duracao).FirstOrDefault();
            short? nm_carga_horaria = (from c in db.Curso
                                  where c.cd_curso == turma.cd_curso
                                  select c.nm_carga_horaria).FirstOrDefault();

            DateTime? dt_final_carga = turma.dt_final_aula == null ? turma.dt_inicio_aula.AddDays(nm_duracao == 0 ? 0 : (int)(nm_carga_horaria == null ? 120 : nm_carga_horaria / (double)nm_duracao * 7.0)) : turma.dt_final_aula;
            try
            {
                IEnumerable<Horario> sql = null;
                switch (tipoCons)
                {
                    // Busca os horários ocupados de outras turmas para uma sala numa mesma escola:
                    case TipoConsultaHorario.HAS_HORARIO_SALA_OCUPADO_TURMA:
                        sql = (from horario in db.Horario
                               join t in db.Turma on horario.cd_registro equals t.cd_turma
                               where horario.cd_pessoa_escola == cdEscola && horario.id_origem == (int)Horario.Origem.TURMA &&
                                     ((t.cd_sala_online == null && t.cd_sala == turma.cd_sala) ||
                                      (t.cd_sala_online != null && t.cd_sala_online == turma.cd_sala_online))  && t.cd_pessoa_escola == cdEscola &&
                                     (turma.cd_turma == 0 || t.cd_turma != turma.cd_turma) &&
                                     t.dt_termino_turma == null && t.cd_turma_ppt == null &&
                                     ((t.dt_final_aula == null && DbFunctions.AddDays(t.dt_inicio_aula, t.Duracao.nm_duracao == 0 ? 0 : (int)(t.Curso.nm_carga_horaria / (double)t.Duracao.nm_duracao * 7.0))>= turma.dt_inicio_aula ) ||
                                      (t.dt_final_aula != null && t.dt_final_aula >= turma.dt_inicio_aula)) &&
                                      t.dt_inicio_aula <= dt_final_carga
                               select new
                               {
                                   no_registro = t.no_turma,
                                   cd_pessoa_escola = horario.cd_pessoa_escola,
                                   cd_registro = horario.cd_registro,
                                   id_origem = horario.id_origem,
                                   id_disponivel = horario.id_disponivel,
                                   id_dia_semana = horario.id_dia_semana,
                                   dt_hora_ini = horario.dt_hora_ini,
                                   dt_hora_fim = horario.dt_hora_fim
                               }).ToList().Select(x => new Horario
                               {
                                   no_registro = x.no_registro,
                                   cd_pessoa_escola = x.cd_pessoa_escola,
                                   cd_registro = x.cd_registro,
                                   id_origem = x.id_origem,
                                   id_disponivel = x.id_disponivel,
                                   id_dia_semana = x.id_dia_semana,
                                   dt_hora_ini = x.dt_hora_ini,
                                   dt_hora_fim = x.dt_hora_fim
                               });
                        break;
                    //Não existe esta chamada no sistema com este tipo de consulta
                    //// Busca os horários ocupados pelo aluno em tumas e turmas ppt pai:
                    //case TipoConsultaHorario.HAS_HORARIO_ALUNO_OCUPADO_TURMA:
                    //    sql = (from h in db.Horario
                    //           join t in db.Turma on h.cd_registro equals t.cd_turma
                    //           join turmaAluno in db.AlunoTurma on t.cd_turma equals turmaAluno.cd_turma
                    //           where h.id_origem == (int)Horario.Origem.TURMA &&
                    //                 t.dt_termino_turma == null &&
                    //                 (t.dt_final_aula == null || t.dt_final_aula >= turma.dt_inicio_aula) &&
                    //                 (turmaAluno.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                    //                  turmaAluno.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                    //                  turmaAluno.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Aguardando) &&
                    //                 t.id_turma_ppt == false &&
                    //                 h.cd_pessoa_escola == cdEscola &&
                    //                 turmaAluno.cd_aluno == cdRegistro
                    //           select new
                    //           {
                    //               no_registro = turma.no_turma,
                    //               cd_horario = h.cd_horario,
                    //               cd_pessoa_escola = h.cd_pessoa_escola,
                    //               cd_registro = h.cd_registro,
                    //               id_origem = h.id_origem,
                    //               id_disponivel = h.id_disponivel,
                    //               id_dia_semana = h.id_dia_semana,
                    //               dt_hora_ini = h.dt_hora_ini,
                    //               dt_hora_fim = h.dt_hora_fim,
                    //               id_situacao = turmaAluno.cd_situacao_aluno_turma
                    //           }).ToList().Select(x => new Horario
                    //           {
                    //               no_registro = x.no_registro,
                    //               cd_horario = x.cd_horario,
                    //               cd_pessoa_escola = x.cd_pessoa_escola,
                    //               cd_registro = x.cd_registro,
                    //               id_origem = x.id_origem,
                    //               id_disponivel = x.id_disponivel,
                    //               id_dia_semana = x.id_dia_semana,
                    //               dt_hora_ini = x.dt_hora_ini,
                    //               dt_hora_fim = x.dt_hora_fim,
                    //               id_situacao = x.id_situacao
                    //           });
                    //    break;
                    // Busca os horários ocupados por atividades extras numa sala:
                    case TipoConsultaHorario.HAS_HORARIO_SALA_ATIVIDA_EXT:
                        sql = (from atExt in db.AtividadeExtra
                               where atExt.cd_sala == (turma.cd_sala_online == null ? turma.cd_sala : turma.cd_sala_online)
                                && atExt.cd_pessoa_escola == cdEscola &&
                               (DbFunctions.TruncateTime(atExt.dt_atividade_extra) > DbFunctions.TruncateTime(now)
                                                 || (DbFunctions.TruncateTime(atExt.dt_atividade_extra) == DbFunctions.TruncateTime(now) && atExt.hh_final >= time))
                               select new
                               {
                                   cd_registro = atExt.cd_atividade_extra,
                                   dt_hora_ini = atExt.hh_inicial,
                                   dt_hora_fim = atExt.hh_final,
                                   dt_atividade_extra = atExt.dt_atividade_extra,
                                   id_disponivel = true

                               }).ToList().Select(x => new Horario
                               {
                                   cd_registro = x.cd_registro,
                                   dt_hora_ini = x.dt_hora_ini,
                                   dt_hora_fim = x.dt_hora_fim,
                                   id_dia_semana = (byte)(x.dt_atividade_extra.DayOfWeek + 1),
                                   id_disponivel = x.id_disponivel
                               });
                        break;
                }

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public String retornaDescricaoHorarioOcupado(int cd_empresa, TimeSpan hr_ini, TimeSpan hr_fim)
        {
            try
            {
                string sql = " pelo(a)";

                byte horarioRet = (from horario in db.Horario
                                   orderby horario.id_origem
                                   where horario.cd_pessoa_escola == cd_empresa &&
                                   (hr_ini > horario.dt_hora_ini || hr_fim < horario.dt_hora_fim)
                                   select horario.id_origem).FirstOrDefault();
                switch (horarioRet)
                {
                    case (byte)Horario.Origem.ALUNO:
                        {
                            sql += " aluno ";
                            sql += (from horario in db.Horario
                                    join aluno in db.Aluno on horario.cd_registro equals aluno.cd_pessoa_aluno
                                    where horario.cd_pessoa_escola == cd_empresa &&
                                    aluno.id_aluno_ativo &&
                                    horario.id_origem == (byte)Horario.Origem.ALUNO &&
                                   (hr_ini > horario.dt_hora_ini || hr_fim < horario.dt_hora_fim)
                                    select aluno.AlunoPessoaFisica.no_pessoa).FirstOrDefault();
                            break;
                        }
                    case (byte)Horario.Origem.PROFESSOR:
                        {
                            sql += " professor ";
                            sql += (from horario in db.Horario
                                    join prof in db.FuncionarioSGF on horario.cd_registro equals prof.cd_funcionario
                                    where horario.cd_pessoa_escola == cd_empresa &&
                                    prof.id_funcionario_ativo &&
                                    horario.id_origem == (byte)Horario.Origem.PROFESSOR &&
                                   (hr_ini > horario.dt_hora_ini || hr_fim < horario.dt_hora_fim)
                                    select prof.FuncionarioPessoaFisica.no_pessoa).FirstOrDefault();
                            break;
                        }
                    case (byte)Horario.Origem.TURMA:
                        {
                            sql += " Turma ";
                            sql += (from horario in db.Horario
                                    join turma in db.Turma on horario.cd_registro equals turma.cd_turma
                                    where horario.cd_pessoa_escola == cd_empresa &&
                                    horario.id_origem == (byte)Horario.Origem.TURMA &&
                                   (hr_ini > horario.dt_hora_ini || hr_fim < horario.dt_hora_fim)
                                    select turma.no_turma).FirstOrDefault();

                            break;
                        }
                    case (byte)Horario.Origem.USUARIO:
                        {
                            sql += " usuário ";
                            var teste = (
                                from horario in db.Horario
                                join usuario in db.UsuarioWebSGF on horario.cd_registro equals usuario.cd_usuario
                                join pessoa in db.PessoaSGF.OfType<PessoaFisicaSGF>() on usuario.cd_pessoa equals pessoa.cd_pessoa
                                where horario.cd_pessoa_escola == cd_empresa && usuario.id_usuario_ativo &&
                                horario.id_origem == (byte)Horario.Origem.USUARIO &&
                                (hr_ini > horario.dt_hora_ini || hr_fim < horario.dt_hora_fim)
                            select new {
                                NomeUsuario = usuario.no_login,
                                NomePessoa = pessoa.no_pessoa,
                                HoraIni = horario.dt_hora_ini,
                                HoraFim = horario.dt_hora_fim
                            }).AsEnumerable().Select(x=> new {
                                 Nome = x.NomeUsuario + "(" + x.NomePessoa + "), horário: " + x.HoraIni.ToString(@"hh\:mm") + " - " +x.HoraFim.ToString(@"hh\:mm")
                            }).ToList().FirstOrDefault();

                            sql += teste.Nome;
                            break;
                        }
                }

                return sql + ".";
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}

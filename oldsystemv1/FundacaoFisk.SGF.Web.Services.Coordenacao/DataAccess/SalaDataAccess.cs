using System;
using System.Collections.Generic;
using System.Linq;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using System.Data.Entity;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;
using Componentes.GenericDataAccess;
using Componentes.GenericDataAccess.GenericException;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess
{
    public class SalaDataAccess : GenericRepository<Sala>, ISalaDataAccess
    {
        public enum TipoConsultaDuracaoEnum 
        {
            HAS_ATIVO = 0,
            HAS_DIARIO_AULA = 1
        }


        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }


        public IEnumerable<SalaSearchUI> getSalaDesc(SearchParameters parametros, string desc, bool inicio, bool? ativo, int cdEscola, bool online)
        {
            try
            {
                IEntitySorter<SalaSearchUI> sorter = EntitySorter<SalaSearchUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<SalaSearchUI> sql;

                sql = from sala in db.Sala.AsNoTracking()
                      where sala.cd_pessoa_escola == cdEscola
                      select new SalaSearchUI
                      {
                          cd_sala = sala.cd_sala,
                          cd_pessoa = sala.cd_pessoa_escola,
                          id_sala_ativa = sala.id_sala_ativa,
                          nm_vaga_sala = sala.nm_vaga_sala,
                          no_pessoa = sala.Escola.no_pessoa,
                          no_sala = sala.no_sala,
                          id_sala_online = sala.id_sala_online,
                          id_zoom = sala.id_zoom,
                          dc_usuario_adm = sala.dc_usuario_adm,
                          dc_senha_usuario_adm = sala.dc_senha_usuario_adm,
                          dc_usuario_escola = sala.dc_usuario_escola,
                          dc_senha_usuario_escola = sala.dc_senha_usuario_escola

                      };

                if (online)
                {
                    sql = from sala in sql
                        where sala.id_sala_online == online
                        select sala;
                }

                if (ativo == null)
                {
                    sql = from sala in sql
                          select sala;
                }
                else
                {
                    sql = from sala in sql
                          where sala.id_sala_ativa == ativo
                          select sala;
                }

                sql = sorter.Sort(sql);

                var retorno = from sala in sql
                              select sala;

                if (!String.IsNullOrEmpty(desc))
                {
                    if (inicio)
                    {
                        retorno = from sala in sql
                                  where sala.no_sala.StartsWith(desc)
                                  select sala;
                    }//end if
                    else
                    {
                        retorno = from sala in sql
                                  where sala.no_sala.Contains(desc)
                                  select sala;
                    }// end else     
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

        public bool deleteAllSala(List<Sala> salas)
        {
            try
            {
                string strSala = "";
                if (salas != null && salas.Count > 0)
                    foreach (Sala e in salas)
                        strSala += e.cd_sala + ",";

                // Remove o último ponto e virgula:
                if (strSala.Length > 0)
                    strSala = strSala.Substring(0, strSala.Length - 1);

                int retorno = db.Database.ExecuteSqlCommand("delete from t_sala where cd_sala in(" + strSala + ")");

                return retorno > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        //Método que busca as salas que possuem horários disponíveis para os horários da turma, passado como parâmetro. 
        //Ou seja, busca as salas cujas outras turmas dessa sala não possuem (intersecção) horários ocupados para os horários da turma em que foi passada como parâmetro.
        public IEnumerable<Sala> getSalasDisponiveisPorHorarios(List<Horario> horarios, int cd_turma, int cd_escola, int? cd_sala, DateTime dtInicio, DateTime? dtFinal)
        {
            try
            {
                var sql = (from salas in db.Sala
                          where salas.id_sala_ativa && salas.id_sala_online == false
                          && salas.Escola.cd_pessoa == cd_escola //&&
                          //(cd_escola == null && salas.cd_sala == cd_sala)
                          select salas).AsEnumerable();
                DateTime now = DateTime.Now;
                TimeSpan time = now.TimeOfDay;
                now = now.Date;

                if (horarios != null && horarios.Count() > 0)
                {
                    var em = horarios.GetEnumerator();
                    while (em.MoveNext())
                    {
                        var h = em.Current;
                        sql = (from salas in sql
                               where !(from ht in db.Horario
                                      join tu in db.Turma on ht.cd_registro equals tu.cd_turma
                                      where ((tu.cd_turma != cd_turma && tu.cd_turma_ppt == null) || (tu.cd_turma_ppt != cd_turma && tu.cd_turma_ppt != null)) 
                                            && ht.id_origem == (int)Horario.Origem.TURMA
                                            && ((tu.dt_termino_turma == null && !tu.id_turma_ppt) ||
                                                (tu.id_turma_ativa && tu.id_turma_ppt && !tu.TurmaFilhasPPT.Any())) &&
                                            ((tu.dt_final_aula == null && DbFunctions.AddDays(tu.dt_inicio_aula, tu.Curso == null ? 120 : tu.Duracao.nm_duracao == 0 ? 0 : (int)(tu.Curso.nm_carga_horaria / (double)tu.Duracao.nm_duracao * 7.0)) >= dtInicio) || 
                                             tu.dt_final_aula >= dtInicio)
                                            && tu.dt_inicio_aula <= dtFinal
                                          && tu.Sala.cd_sala == salas.cd_sala &&
                                          ht.id_dia_semana == h.id_dia_semana &&
                                          ((ht.dt_hora_ini <= h.dt_hora_ini && h.dt_hora_ini < ht.dt_hora_fim)
                                              || (ht.dt_hora_ini < h.dt_hora_fim && h.dt_hora_fim <= ht.dt_hora_fim)
                                              || (ht.dt_hora_ini <= h.dt_hora_ini && h.dt_hora_fim <= ht.dt_hora_fim)
                                              || (ht.dt_hora_ini >= h.dt_hora_ini && h.dt_hora_fim >= ht.dt_hora_fim))
                                      select ht.cd_horario).Any()
                                    && !(from ae in db.AtividadeExtra
                                         where ae.cd_sala == salas.cd_sala
                                            && (DbFunctions.TruncateTime(ae.dt_atividade_extra) > (DbFunctions.TruncateTime(now) > dtInicio ? DbFunctions.TruncateTime(now) : dtInicio)
                                                || (DbFunctions.TruncateTime(ae.dt_atividade_extra) == (DbFunctions.TruncateTime(now) > dtInicio ? DbFunctions.TruncateTime(now) : dtInicio) && ae.hh_final >= time))
                                            && ae.dt_atividade_extra >= dtInicio
                                            && System.Data.Entity.SqlServer.SqlFunctions.DatePart("weekday", ae.dt_atividade_extra) == h.id_dia_semana
                                            && ((ae.hh_inicial <= h.dt_hora_ini && h.dt_hora_ini < ae.hh_final)
                                                || (ae.hh_inicial < h.dt_hora_fim && h.dt_hora_fim <= ae.hh_final)
                                                || (ae.hh_inicial <= h.dt_hora_ini && h.dt_hora_fim <= ae.hh_final)
                                                || (ae.hh_inicial >= h.dt_hora_ini && h.dt_hora_fim >= ae.hh_final))
                                         select ae.cd_atividade_extra).Any()
                              select salas).ToList();
                    }
                }
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        //Método que busca as salas que possuem horários disponíveis para os horários da turma, passado como parâmetro. 
        //Ou seja, busca as salas cujas outras turmas dessa sala não possuem (intersecção) horários ocupados para os horários da turma em que foi passada como parâmetro.
        public IEnumerable<Sala> getSalasDisponiveisPorHorariosByModalidadeOnline(List<Horario> horarios, int cd_turma, int cd_escola, int? cd_sala, DateTime dtInicio, DateTime? dtFinal)
        {
            try
            {
                var sql = (from salas in db.Sala
                          where salas.id_sala_ativa
                          && salas.cd_pessoa_escola == cd_escola
                          && salas.id_sala_online == true//&&//&&
                          //(cd_escola == null && salas.cd_sala == cd_sala)
                          select salas).AsEnumerable();
                DateTime now = DateTime.Now;
                TimeSpan time = now.TimeOfDay;
                now = now.Date;

                if (horarios != null && horarios.Count() > 0)
                {
                    var em = horarios.GetEnumerator();
                    while (em.MoveNext())
                    {
                        var h = em.Current;
                        sql = (from salas in sql
                              where !(from ht in db.Horario
                                      join tu in db.Turma on ht.cd_registro equals tu.cd_turma
                                      where ((tu.cd_turma != cd_turma && tu.cd_turma_ppt == null) || (tu.cd_turma_ppt != cd_turma && tu.cd_turma_ppt != null))
                                            && ht.id_origem == (int)Horario.Origem.TURMA
                                            && ((tu.dt_termino_turma == null && !tu.id_turma_ppt)  ||
                                                (tu.id_turma_ativa && tu.id_turma_ppt && !tu.TurmaFilhasPPT.Any())) &&
                                            ((tu.dt_final_aula == null && DbFunctions.AddDays(tu.dt_inicio_aula, tu.Curso == null ? 120 : tu.Duracao.nm_duracao == 0 ? 0 : (int)(tu.Curso.nm_carga_horaria / (double)tu.Duracao.nm_duracao * 7.0)) >= dtInicio) ||  
                                             tu.dt_final_aula >= dtInicio)
                                            && tu.dt_inicio_aula <= dtFinal
                                          && tu.cd_sala_online == salas.cd_sala
                                          && ht.id_dia_semana == h.id_dia_semana
                                          && ((ht.dt_hora_ini <= h.dt_hora_ini && h.dt_hora_ini < ht.dt_hora_fim)
                                          ||  (ht.dt_hora_ini < h.dt_hora_fim && h.dt_hora_fim <= ht.dt_hora_fim)
                                          ||  (ht.dt_hora_ini <= h.dt_hora_ini && h.dt_hora_fim <= ht.dt_hora_fim)
                                          ||  (ht.dt_hora_ini >= h.dt_hora_ini && h.dt_hora_fim >= ht.dt_hora_fim)
                                          )
                                      select ht.cd_horario).Any()
                                    && !(from ae in db.AtividadeExtra
                                         where ae.cd_sala == salas.cd_sala
                                            && (DbFunctions.TruncateTime(ae.dt_atividade_extra) > (DbFunctions.TruncateTime(now) > dtInicio ? DbFunctions.TruncateTime(now) : dtInicio)
                                                || (DbFunctions.TruncateTime(ae.dt_atividade_extra) == (DbFunctions.TruncateTime(now) > dtInicio ? DbFunctions.TruncateTime(now) : dtInicio) && ae.hh_final >= time))
                                            && System.Data.Entity.SqlServer.SqlFunctions.DatePart("weekday", ae.dt_atividade_extra) == h.id_dia_semana
                                            && ((ae.hh_inicial <= h.dt_hora_ini && h.dt_hora_ini < ae.hh_final)
                                                || (ae.hh_inicial < h.dt_hora_fim && h.dt_hora_fim <= ae.hh_final)
                                                || (ae.hh_inicial <= h.dt_hora_ini && h.dt_hora_fim <= ae.hh_final)
                                                || (ae.hh_inicial >= h.dt_hora_ini && h.dt_hora_fim >= ae.hh_final))
                                         select ae.cd_atividade_extra).Any()
                                    && !(from ar in db.AulaReposicao
                                         where ar.cd_sala == salas.cd_sala
                                            && (DbFunctions.TruncateTime(ar.dt_aula_reposicao) > (DbFunctions.TruncateTime(now) > dtInicio ? DbFunctions.TruncateTime(now) : dtInicio)
                                                || (DbFunctions.TruncateTime(ar.dt_aula_reposicao) == (DbFunctions.TruncateTime(now) > dtInicio ? DbFunctions.TruncateTime(now) : dtInicio) && ar.dh_final_evento >= time))
                                            && System.Data.Entity.SqlServer.SqlFunctions.DatePart("weekday", ar.dt_aula_reposicao) == h.id_dia_semana
                                            && ((ar.dh_inicial_evento <= h.dt_hora_ini && h.dt_hora_ini < ar.dh_final_evento)
                                                || (ar.dh_inicial_evento < h.dt_hora_fim && h.dt_hora_fim <= ar.dh_final_evento)
                                                || (ar.dh_inicial_evento <= h.dt_hora_ini && h.dt_hora_fim <= ar.dh_final_evento)
                                                || (ar.dh_inicial_evento >= h.dt_hora_ini && h.dt_hora_fim >= ar.dh_final_evento))
                                         select ar.cd_aula_reposicao).Any()
                              select salas).ToList();
                    }
                }
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }


        public IEnumerable<Sala> findListSalasDiponiveis(TimeSpan horaIni, TimeSpan horaFim, DateTime data, bool? status, int? cdSala, int cdEscola, int? cd_atividade_extra)
        {
            IQueryable<Sala> sql;
            try
            {
                if (!cd_atividade_extra.HasValue)
                    sql = from salas in db.Sala
                          where salas.cd_pessoa_escola == cdEscola
                          && !(salas.SalaAtividadeExtra.Where(a => ((a.hh_inicial <= horaIni && horaIni < a.hh_final)
                                                                     || (a.hh_inicial < horaFim && horaFim <= a.hh_final)
                                                                     || (a.hh_inicial <= horaIni && horaFim <= a.hh_final)
                                                                     || (a.hh_inicial >= horaIni && horaFim >= a.hh_final))
                                                                     && a.dt_atividade_extra == data).Any())
                          && !(salas.AulaReposicao.Where(a => ((a.dh_inicial_evento <= horaIni && horaIni < a.dh_final_evento)
                                                                     || (a.dh_inicial_evento < horaFim && horaFim <= a.dh_final_evento)
                                                                     || (a.dh_inicial_evento <= horaIni && horaFim <= a.dh_final_evento)
                                                                     || (a.dh_inicial_evento >= horaIni && horaFim >= a.dh_final_evento))
                                                                     && a.dt_aula_reposicao == data).Any())
                         && !(from ht in db.Horario
                              join tu in db.Turma on ht.cd_registro equals tu.cd_turma
                              where ht.id_origem == (int)Horario.Origem.TURMA
                                    && ((!(bool)salas.id_sala_online && tu.Sala.cd_sala == salas.cd_sala) ||
                                    ((bool)salas.id_sala_online && tu.T_SALA.cd_sala == salas.cd_sala))
                                    && tu.dt_termino_turma == null &&
                                    ((tu.dt_final_aula == null && DbFunctions.AddDays(tu.dt_inicio_aula, tu.Duracao.nm_duracao == 0 ? 0 : (int)(tu.Curso.nm_carga_horaria / (double)tu.Duracao.nm_duracao * 7.0)) >= data) ||
                                     tu.dt_final_aula >= data)
                                    && ht.id_dia_semana == System.Data.Entity.SqlServer.SqlFunctions.DatePart("weekday", data)
                                    && ((ht.dt_hora_ini <= horaIni && horaIni < ht.dt_hora_fim)
                                              || (ht.dt_hora_ini < horaFim && horaFim <= ht.dt_hora_fim)
                                              || (ht.dt_hora_ini <= horaIni && horaFim <= ht.dt_hora_fim)
                                              || (ht.dt_hora_ini >= horaIni && horaFim >= ht.dt_hora_fim))
                              select ht.cd_horario).Any()
                          select salas;
                else
                {
                    int cdEscolaAtividade = (from a in db.AtividadeExtra where a.cd_atividade_extra == cd_atividade_extra select a.cd_pessoa_escola).FirstOrDefault();
                    if (cdEscola != cdEscolaAtividade)
                    {
                        sql = from salas in db.Sala
                              where salas.cd_pessoa_escola == cdEscolaAtividade &&
                              salas.SalaAtividadeExtra.Any(a => a.cd_atividade_extra == cd_atividade_extra)
                              select salas;
                    }
                    else
                    {
                        sql = from salas in db.Sala
                              where salas.cd_pessoa_escola == cdEscola
                              && !(salas.SalaAtividadeExtra.Any(a => (((a.hh_inicial <= horaIni && horaIni < a.hh_final)
                                                                     || (a.hh_inicial < horaFim && horaFim <= a.hh_final)
                                                                     || (a.hh_inicial <= horaIni && horaFim <= a.hh_final)
                                                                     || (a.hh_inicial >= horaIni && horaFim >= a.hh_final))
                                                                     && a.dt_atividade_extra == data
                                                                     && a.cd_atividade_extra != cd_atividade_extra)))
                              && !(salas.AulaReposicao.Where(a => ((a.dh_inicial_evento <= horaIni && horaIni < a.dh_final_evento)
                                                                         || (a.dh_inicial_evento < horaFim && horaFim <= a.dh_final_evento)
                                                                         || (a.dh_inicial_evento <= horaIni && horaFim <= a.dh_final_evento)
                                                                         || (a.dh_inicial_evento >= horaIni && horaFim >= a.dh_final_evento))
                                                                         && a.dt_aula_reposicao == data).Any())
                             && !(from ht in db.Horario
                                  join tu in db.Turma on ht.cd_registro equals tu.cd_turma
                                  where ht.id_origem == (int)Horario.Origem.TURMA
                                        && ((!(bool)salas.id_sala_online && tu.Sala.cd_sala == salas.cd_sala) ||
                                        ((bool)salas.id_sala_online && tu.T_SALA.cd_sala == salas.cd_sala))
                                        && tu.dt_termino_turma == null &&
                                        ((tu.dt_final_aula == null && DbFunctions.AddDays(tu.dt_inicio_aula, tu.Duracao.nm_duracao == 0 ? 0 : (int)(tu.Curso.nm_carga_horaria / (double)tu.Duracao.nm_duracao * 7.0)) >= data) ||
                                         tu.dt_final_aula >= data)
                                        && ht.id_dia_semana == System.Data.Entity.SqlServer.SqlFunctions.DatePart("weekday", data)
                                        && ((ht.dt_hora_ini <= horaIni && horaIni < ht.dt_hora_fim)
                                                  || (ht.dt_hora_ini < horaFim && horaFim <= ht.dt_hora_fim)
                                                  || (ht.dt_hora_ini <= horaIni && horaFim <= ht.dt_hora_fim)
                                                  || (ht.dt_hora_ini >= horaIni && horaFim >= ht.dt_hora_fim))
                                  select ht.cd_horario).Any()
                              select salas;
                    }
                }
                if (status == null)
                    sql = from sala in sql
                          select sala;
                else
                    sql = from sala in sql
                          where sala.id_sala_ativa == status
                           || (cdSala.HasValue && sala.cd_sala == cdSala)
                          select sala;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Sala> findListSalasDiponiveisAulaRep(TimeSpan horaIni, TimeSpan horaFim, DateTime data, bool? status, int? cdSala, int cdEscola, int? cd_aula_reposicao, int? cd_turma)
        {
            IQueryable<Sala> sql;
            try
            {
                if (!cd_aula_reposicao.HasValue)
                    sql = from salas in db.Sala
                          where salas.cd_pessoa_escola == cdEscola && salas.id_sala_online == true
                          && !(salas.AulaReposicao.Where(a => ((a.dh_inicial_evento <= horaIni && horaIni < a.dh_final_evento)
                                                                     || (a.dh_inicial_evento < horaFim && horaFim <= a.dh_final_evento)
                                                                     || (a.dh_inicial_evento <= horaIni && horaFim <= a.dh_final_evento)
                                                                     || (a.dh_inicial_evento >= horaIni && horaFim >= a.dh_final_evento))
                                                                     && a.dt_aula_reposicao == data).Any())
                          && !(salas.SalaAtividadeExtra.Where(a => ((a.hh_inicial <= horaIni && horaIni < a.hh_final)
                                                                     || (a.hh_inicial < horaFim && horaFim <= a.hh_final)
                                                                     || (a.hh_inicial <= horaIni && horaFim <= a.hh_final)
                                                                     || (a.hh_inicial >= horaIni && horaFim >= a.hh_final))
                                                                     && a.dt_atividade_extra == data).Any())
                         && !(from ht in db.Horario
                              join tu in db.Turma on ht.cd_registro equals tu.cd_turma
                              where ht.id_origem == (int)Horario.Origem.TURMA //&& tu.cd_regime == 10
                                    && tu.T_SALA.cd_sala == salas.cd_sala
                                    && tu.dt_termino_turma == null &&
                                    ((tu.dt_final_aula == null && DbFunctions.AddDays(tu.dt_inicio_aula, tu.Duracao.nm_duracao == 0 ? 0 : (int)(tu.Curso.nm_carga_horaria / (double)tu.Duracao.nm_duracao * 7.0)) >= data) ||  
                                     tu.dt_final_aula >= data)
                                    && ht.id_dia_semana == System.Data.Entity.SqlServer.SqlFunctions.DatePart("weekday", data)
                                    && ((ht.dt_hora_ini <= horaIni && horaIni < ht.dt_hora_fim)
                                              || (ht.dt_hora_ini < horaFim && horaFim <= ht.dt_hora_fim)
                                              || (ht.dt_hora_ini <= horaIni && horaFim <= ht.dt_hora_fim)
                                              || (ht.dt_hora_ini >= horaIni && horaFim >= ht.dt_hora_fim))
                              select ht.cd_horario).Any()
                          select salas;
                else
                    sql = from salas in db.Sala
                          where salas.cd_pessoa_escola == cdEscola && salas.id_sala_online == true
                          && !(salas.AulaReposicao.Any(a => (((a.dh_inicial_evento <= horaIni && horaIni < a.dh_final_evento)
                                                                 || (a.dh_inicial_evento < horaFim && horaFim <= a.dh_final_evento)
                                                                 || (a.dh_inicial_evento <= horaIni && horaFim <= a.dh_final_evento)
                                                                 || (a.dh_inicial_evento >= horaIni && horaFim >= a.dh_final_evento))
                                                                 && a.dt_aula_reposicao == data
                                                                 && a.cd_aula_reposicao != cd_aula_reposicao)))
                          && !(salas.SalaAtividadeExtra.Where(a => ((a.hh_inicial <= horaIni && horaIni < a.hh_final)
                                                                     || (a.hh_inicial < horaFim && horaFim <= a.hh_final)
                                                                     || (a.hh_inicial <= horaIni && horaFim <= a.hh_final)
                                                                     || (a.hh_inicial >= horaIni && horaFim >= a.hh_final))
                                                                     && a.dt_atividade_extra == data).Any())
                         && !(from ht in db.Horario
                              join tu in db.Turma on ht.cd_registro equals tu.cd_turma
                              where ht.id_origem == (int)Horario.Origem.TURMA //&& tu.cd_regime == 10
                                    && tu.T_SALA.cd_sala == salas.cd_sala //&& tu.cd_turma != cd_turma
                                    && tu.dt_termino_turma == null &&
                                    ((tu.dt_final_aula == null && DbFunctions.AddDays(tu.dt_inicio_aula, tu.Duracao.nm_duracao == 0 ? 0 : (int)(tu.Curso.nm_carga_horaria / (double)tu.Duracao.nm_duracao * 7.0)) >= data) ||  
                                     tu.dt_final_aula >= data)
                                    && ht.id_dia_semana == System.Data.Entity.SqlServer.SqlFunctions.DatePart("weekday", data)
                                    && ((ht.dt_hora_ini <= horaIni && horaIni < ht.dt_hora_fim)
                                              || (ht.dt_hora_ini < horaFim && horaFim <= ht.dt_hora_fim)
                                              || (ht.dt_hora_ini <= horaIni && horaFim <= ht.dt_hora_fim)
                                              || (ht.dt_hora_ini >= horaIni && horaFim >= ht.dt_hora_fim))
                              select ht.cd_horario).Any()
                          select salas;

                if (status == null)
                    sql = from sala in sql
                          select sala;
                else
                    sql = from sala in sql
                          where sala.id_sala_ativa == status
                           || (cdSala.HasValue && sala.cd_sala == cdSala)
                          select sala;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Sala> findListSalas(bool? status, int? cdSala, int cdEscola)
        {
            var idSala = cdSala;
            try
            {
                var sql = from salas in db.Sala
                          where salas.cd_pessoa_escola == cdEscola
                          select salas;
                if (status == null)
                {
                    sql = from sala in sql
                          select sala;
                }
                else
                {
                    sql = from sala in sql
                          where sala.id_sala_ativa == status
                                || (cdSala == idSala && sala.cd_sala == cdSala)
                          select sala;
                }
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Sala> getSalas(int cd_sala,int cd_escola, TipoConsultaDuracaoEnum tipoConsulta)
        {
            try
            {
                var sql = from salas in db.Sala
                          //where salas.cd_pessoa_escola == cd_escola
                          where ((from t in db.Turma
                              where (t.cd_sala_online == salas.cd_sala || t.cd_sala == salas.cd_sala) && t.dt_termino_turma == null &&
                                    (t.cd_pessoa_escola == cd_escola ||
                                     (from te in db.TurmaEscola
                                         where te.cd_turma == t.cd_turma &&
                                               te.cd_escola == cd_escola
                                      select te).Any())
                              select t).Any())
                          select salas;
                switch (tipoConsulta)
                {
                    case TipoConsultaDuracaoEnum.HAS_ATIVO:
                        if (cd_sala == 0)
                        sql = from sala in sql
                          where sala.id_sala_ativa == true //&& (cd_sala == 0 || sala.cd_sala == cd_sala)
                          orderby sala.no_sala
                          select sala;
                        else
                            sql = from sala in db.Sala.AsNoTracking()
                                  where sala.id_sala_ativa == true && sala.cd_sala == cd_sala 
                                  orderby sala.no_sala
                                  select sala;
                    break;
                }
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Sala> getSalasAulaReposicao(int cd_escola, TipoConsultaDuracaoEnum tipoConsulta)
        {
            try
            {
                var sql = from salas in db.Sala.AsNoTracking()
                          //join aulareposicao in db.AulaReposicao on salas.cd_sala equals aulareposicao.cd_sala
                          where salas.cd_pessoa_escola == cd_escola
                          && salas.AulaReposicao.Any()
                          select salas;
                switch (tipoConsulta)
                {
                    case TipoConsultaDuracaoEnum.HAS_ATIVO:
                        sql = from sala in sql
                              where sala.id_sala_ativa == true
                              orderby sala.no_sala
                              select sala;
                        break;
                }
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        // to do
        public IEnumerable<Sala> findListSalasTurmas(int cdEscola)
        {
            try
            {

                //var sql = from sala in db.Sala
                //          where sala.Turma.Where(t => t.cd_sala == sala.cd_sala && t.dt_termino_turma == null).Any() //LBM&& sala.cd_pessoa_escola == cdEscola
                //          select sala;

               var sql = from sala in db.Sala
                    where ((from t in db.Turma
                        where (t.cd_sala_online == sala.cd_sala || t.cd_sala == sala.cd_sala) && t.dt_termino_turma == null &&
                              (t.cd_pessoa_escola == cdEscola ||
                               (from te in db.TurmaEscola
                                   where te.cd_turma == t.cd_turma &&
                                         te.cd_escola == cdEscola
                                select te).Any())
                        select t).Any())
                    select sala;
                return sql; 
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Sala> findSalasTurmas(int cdEscola, bool online)
        {
            try
            {
                var sql = from sala in db.Sala
                    where (sala.cd_pessoa_escola == cdEscola) &&
                           sala.id_sala_online == online &&
                          ((from t in db.Turma
                            where (t.cd_sala_online == sala.cd_sala || t.cd_sala == sala.cd_sala)
                            select t).Any())
                    select sala;
                return sql.OrderBy(x => x.no_sala);
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool verificaSalaOnline(int? cdSala, int cdEscola)
        {
            try
            {
                Sala sql = (from sala in db.Sala
                    where sala.cd_sala == cdSala //LBM&& sala.cd_pessoa_escola == cdEscola
                    select sala).FirstOrDefault();
                if (sql == null || sql.id_sala_online == false)
                {
                    return false;
                }
                else 
                {
                    return true;
                }
                
                
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public List<ReportControleSala> getHorariosRptControleSala(TimeSpan? hIni, TimeSpan? hFim, int cd_turma, int cd_professor, int cd_sala, List<int> diasSemana, int cd_escola)
        {
            try
            {
                List<int> cdsTurmasEscola = (from t in db.Turma
                    where
                        (from te in db.TurmaEscola
                            where te.cd_turma == t.cd_turma &&
                                  te.cd_escola == cd_escola
                            select te).Any()
                    select t.cd_turma).ToList();

                var sql = from h in db.Horario
                          //join t in db.Turma on h.cd_registro equals t.cd_turma
                          where h.id_origem == (int)Horario.Origem.TURMA &&
                                //t.cd_pessoa_escola == cd_escola
                                (from t in db.Turma where t.cd_turma == h.cd_registro &&
                                    (t.cd_pessoa_escola == cd_escola ||
                                     cdsTurmasEscola.Contains(t.cd_turma))
                                    && (t.cd_sala_online != null || t.cd_sala != null) && ((!t.id_turma_ppt && t.cd_turma_ppt == null && t.dt_termino_turma == null) || (t.id_turma_ppt && t.cd_turma_ppt == null && t.id_turma_ativa))
                                 select t).Any()
                          select h;
                if (cd_sala > 0)
                    sql = from h in sql
                          join t in db.Turma on h.cd_registro equals t.cd_turma
                          where (t.cd_sala == cd_sala || t.cd_sala_online == cd_sala)
                          select h;
                if (cd_turma > 0)
                    sql = from h in sql
                          join t in db.Turma on h.cd_registro equals t.cd_turma
                          where t.cd_turma == cd_turma
                          select h;
                if (cd_professor > 0)
                    sql = from h in sql
                          where h.HorariosProfessores.Any(x => x.cd_professor == cd_professor && db.ProfessorTurma.Any(pt=> pt.cd_professor == cd_professor && pt.id_professor_ativo && pt.Turma.cd_turma == x.Horario.cd_registro))
                          select h;
                if (hIni.HasValue)
                    sql = from p in sql
                          where p.dt_hora_ini >= hIni
                          select p;
                if (hFim.HasValue)
                    sql = from p in sql
                          where p.dt_hora_fim <= hFim
                          select p;
                if (diasSemana.Count() > 0)
                    sql = from p in sql
                          where diasSemana.Contains(p.id_dia_semana)
                          select p;

                var retorno = (from h in sql
                               join t in db.Turma.Include(x=> x.Curso.Estagio) on h.cd_registro equals t.cd_turma
                               select new
                               {
                                   cd_sala = t.cd_sala_online != null ? t.cd_sala_online : t.cd_sala,
                                   no_sala = t.cd_sala_online != null ? t.T_SALA.no_sala : t.Sala.no_sala,
                                   dt_hora_ini =  h.dt_hora_ini,
                                   dt_hora_fim = h.dt_hora_fim,
                                   professores = h.HorariosProfessores.Where(
                                       x => (cd_professor == 0 || x.cd_professor == cd_professor) &&
                                        x.Professor.ProfessorTurma.Any(pt => pt.cd_turma == t.cd_turma && pt.id_professor_ativo))
                                        .Select(z => new { no_professor = z.Professor.FuncionarioPessoaFisica.dc_reduzido_pessoa, cd_professor = z.cd_professor }),
                                   //professores = h.HorariosProfessores.Select(z => new { no_professor = z.Professor.FuncionarioPessoaFisica.dc_reduzido_pessoa, cd_professor = z.cd_professor }),
                                   cor_legenda = (t.Curso != null && t.Curso.Estagio != null)? t.Curso.Estagio.cor_legenda: "",
                                   no_estagio_abreviado = (t.Curso != null && t.Curso.Estagio != null) ? t.Curso.Estagio.no_estagio_abreviado: "",
                                   id_dia_semana = h.id_dia_semana,
                                   nm_aulas = (from d in db.AlunoTurma.Where(
                                                    r => r.cd_turma == t.cd_turma && (r.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                    r.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado)) select d).Count() +
                                                (from d in db.AlunoTurma.Where(
                                                    r => db.Turma.Any(f => !f.id_turma_ppt && f.cd_turma_ppt == t.cd_turma && r.cd_turma == f.cd_turma 
                                                    && f.dt_termino_turma == null) 
                                                    && (r.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo || r.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado)) select d).Count()
                               }).ToList().Select(x => new ReportControleSala
                               {
                                   cd_sala = (int)x.cd_sala,
                                   no_sala = x.no_sala,
                                   no_tipo_cor = x.cor_legenda,
                                   hora_ini = x.dt_hora_ini,
                                   hora_fim = x.dt_hora_fim,
                                   no_sigla_estagio = x.no_estagio_abreviado,
                                   id_dia_semana = x.id_dia_semana,
                                   nm_aulas = x.nm_aulas,
                                   listaProfessores = x.professores != null ? (from lp in x.professores
                                                       select new
                                                       {
                                                           cd_professor = lp.cd_professor,
                                                           no_professor = lp.no_professor
                                                       }).Select(y => new ProfessorTurma
                                                       {
                                                           cd_professor = y.cd_professor,
                                                           no_professor = y.no_professor
                                                       }).ToList(): null
                               }).ToList();

                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public IEnumerable<Sala> findListSalasAulaPer(int cdEscola)
        {
            try
            {
                var sql = from sala in db.Sala
                          where sala.Turma.Where(t => t.cd_pessoa_escola == cdEscola && t.cd_sala_online == sala.cd_sala && t.AulaPersonalizada.Any()).Any()
                          select sala;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}

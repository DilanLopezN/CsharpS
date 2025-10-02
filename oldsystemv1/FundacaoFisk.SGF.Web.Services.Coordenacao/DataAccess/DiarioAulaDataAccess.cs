using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web.UI.WebControls;
using Componentes.GenericDataAccess;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess
{
    public class DiarioAulaDataAccess : GenericRepository<DiarioAula>, IDiarioAulaDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public IEnumerable<vi_diario_aula> searchDiarioAula(SearchParameters parametros, int cd_turma, string no_professor, int cd_tipo_aula, byte status, byte presProf,
                                                       bool substituto, bool inicio, DateTime? dtInicial, DateTime? dtFinal, int cd_escola, int? cdProf, int cd_escola_combo)
        {
            try
            {
                IEntitySorter<vi_diario_aula> sorter = EntitySorter<vi_diario_aula>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<vi_diario_aula> sql;

                sql = (from da in db.vi_diario_aula.AsNoTracking()
                           //join t in db.Turma on da.cd_turma equals t.cd_turma 
                       where
                           da.id_status_aula == status &&
                           da.cd_pessoa_empresa == cd_escola
                    select da).Union(
                    (from da in db.vi_diario_aula.AsNoTracking()
                     where
                         da.id_status_aula == status &&
                        (from te in db.TurmaEscola
                         where te.cd_turma == da.cd_turma &&
                                 te.cd_escola == cd_escola
                         select te).Any()
                    select da));





                if (cd_turma > 0)
                    sql = from da in sql
                      where da.cd_turma == cd_turma
                      select da;

                if (!string.IsNullOrEmpty(no_professor))
                {
                    if (substituto)
                    {
                        if (inicio == true)
                            sql = from t in sql
                                  where t.nom_susbtituto.StartsWith(no_professor)
                                  select t;
                        else
                            sql = from t in sql
                                  where t.nom_susbtituto.Contains(no_professor)
                                  select t;
                    }
                    else
                    {
                        if (inicio == true)
                            sql = from t in sql
                                  where t.nom_professor.StartsWith(no_professor)
                                  select t;
                        else
                            sql = from t in sql
                                  where t.nom_professor.Contains(no_professor)
                                  select t;
                    }
                }

                if (cd_tipo_aula > 0)
                    sql = from da in sql
                          where da.cd_tipo_aula == cd_tipo_aula
                      select da;

                if (presProf >= 0 && presProf <= 3)
                    sql = from da in sql
                          where da.id_falta_professor == presProf
                          select da;

                if (dtInicial.HasValue)
                    sql = from t in sql
                          where t.dt_aula >= dtInicial
                          select t;

                if (dtFinal.HasValue)
                    sql = from t in sql
                          where t.dt_aula <= dtFinal
                          select t;

                if (cdProf != null && cdProf > 0)
                    sql = from da in sql
                          where (from pt in db.ProfessorTurma where pt.cd_turma == da.cd_turma && pt.cd_professor == cdProf select pt.cd_professor_turma).Any()
                          select da;

                sql = sorter.Sort(sql);
                int limite = sql.Select(x => x.cd_diario_aula).Count();
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

        public bool verificaTurmaDiarioAulaLancado(int cd_turma, int cd_escola)
        {
            try
            {
                int sql = (from d in db.DiarioAula
                          where d.cd_pessoa_empresa == cd_escola && d.cd_turma == cd_turma
                          select d.cd_diario_aula).FirstOrDefault();

                return sql > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public DiarioAula getEditDiarioAula(int cd_diario_aula,int cd_escola)
        {
            try
            {
                DiarioAula sql = (from d in db.DiarioAula
                                  where d.cd_diario_aula == cd_diario_aula //d.cd_pessoa_empresa == cd_escola && 
                                  select new
                                  {
                                      diarioAula = d,
                                      no_turma = d.Turma.no_turma,
                                      programacao = d.ProgramacaoTurma,
                                      desc_tipo_aula = d.TipoAtividade_Extra.no_tipo_atividade_extra,
                                      no_sala = d.Sala.no_sala,
                                      no_prof = d.Professor.FuncionarioPessoaFisica.dc_reduzido_pessoa,
                                      no_substituto = d.ProfessorSubstituto.FuncionarioPessoaFisica.dc_reduzido_pessoa,
                                      dc_tipo_avalicao = d.Avaliacao.TipoAvaliacao.dc_tipo_avaliacao,
                                      dc_criterio_avaliacao = d.Avaliacao.CriterioAvaliacao.dc_criterio_avaliacao,
                                      cd_motivo_falta = d.cd_motivo_falta

                                  }).ToList().Select(x => new DiarioAula
                                  {
                                      cd_diario_aula = x.diarioAula.cd_diario_aula,
                                      cd_pessoa_empresa = x.diarioAula.cd_pessoa_empresa,
                                      cd_professor = x.diarioAula.cd_professor,
                                      cd_professor_substituto = x.diarioAula.cd_professor_substituto,
                                      cd_avaliacao = x.diarioAula.cd_avaliacao,
                                      cd_programacao_turma = x.diarioAula.cd_programacao_turma,
                                      cd_sala = x.diarioAula.cd_sala,
                                      cd_tipo_aula = x.diarioAula.cd_tipo_aula,
                                      cd_turma = x.diarioAula.cd_turma,
                                      dc_obs_falta = x.diarioAula.dc_obs_falta,
                                      dt_aula = x.diarioAula.dt_aula,
                                      dt_cadastro_aula = x.diarioAula.dt_cadastro_aula,
                                      hr_inicial_aula = x.diarioAula.hr_inicial_aula,
                                      hr_final_aula = x.diarioAula.hr_final_aula,
                                      id_aula_externa = x.diarioAula.id_aula_externa,
                                      id_status_aula = x.diarioAula.id_status_aula,
                                      id_falta_professor = x.diarioAula.id_falta_professor,
                                      nm_aula_turma = x.diarioAula.nm_aula_turma,
                                      tx_obs_aula = x.diarioAula.tx_obs_aula,
                                      no_turma = x.no_turma,
                                      ProgramacaoTurma = x.programacao,
                                      desc_tipo_aula = x.desc_tipo_aula,
                                      no_sala = x.no_sala,
                                      no_prof = x.no_prof,
                                      no_substituto = x.no_substituto,
                                      Avaliacao = new Avaliacao { descTipoENomeAvalicao = x.dc_tipo_avalicao + " - " + x.dc_criterio_avaliacao},
                                      cd_motivo_falta = x.cd_motivo_falta
                                  }).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public vi_diario_aula getDiarioForGridById(int cd_diario_aula, int cd_pessoa_escola)
        {
            try
            {
                vi_diario_aula sql = (from d in db.DiarioAula
                                  where d.cd_pessoa_empresa == cd_pessoa_escola && d.cd_diario_aula == cd_diario_aula
                                  select new
                                  {
                                      cd_diario_aula = d.cd_diario_aula,
                                      cd_pessoa_empresa = d.cd_pessoa_empresa,
                                      cd_tipo_aula = d.cd_tipo_aula,
                                      no_tipo_atividade_extra = d.TipoAtividade_Extra.no_tipo_atividade_extra,
                                      cd_turma = d.cd_turma,
                                      dt_aula = d.dt_aula,
                                      id_status_aula = d.id_status_aula,
                                      id_falta_professor = d.id_falta_professor,
                                      nm_aula_turma = d.nm_aula_turma,
                                      no_turma = d.Turma.no_turma,
                                      no_sala = d.Sala.no_sala,
                                      no_prof = d.Professor.FuncionarioPessoaFisica.dc_reduzido_pessoa,
                                      no_substituto = d.ProfessorSubstituto.FuncionarioPessoaFisica.dc_reduzido_pessoa
                                  }).ToList().Select(x => new vi_diario_aula
                                  {
                                      cd_diario_aula = x.cd_diario_aula,
                                      cd_pessoa_empresa = x.cd_pessoa_empresa,
                                      cd_tipo_aula = x.cd_tipo_aula,
                                      no_tipo_atividade_extra = x.no_tipo_atividade_extra,
                                      cd_turma = x.cd_turma,
                                      dt_aula = x.dt_aula,
                                      id_status_aula = x.id_status_aula,
                                      id_falta_professor = x.id_falta_professor,
                                      nm_aula_turma = x.nm_aula_turma,
                                      no_turma = x.no_turma,
                                      nom_professor = x.no_prof,
                                      nom_susbtituto = x.no_substituto,
                                  }).FirstOrDefault();

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        //Verifica se os horários do novo diário de aula coincidem com os horários dos outros diários de aula da mesma turma
        //ou com os horários das outras programações da turma que ainda não possuem diário de aula
        public bool verificaIntersecaoHorarios(int cd_turma, int? cd_programacao_turma, TimeSpan hr_inicial_aula, TimeSpan hr_final_aula, DateTime data) {
            try {
                bool existe_intersecao = true;
                short status_cancelada = (int) DiarioAula.StatusDiarioAula.Cancelada;
                var sql = from p in db.ProgramacaoTurma
                            where
                                (from d in db.DiarioAula
                                where d.cd_turma == cd_turma
                                    && p.cd_programacao_turma == d.cd_programacao_turma
                                    && d.id_status_aula != status_cancelada
                                    && (!cd_programacao_turma.HasValue || d.cd_programacao_turma != cd_programacao_turma)
                                    && DbFunctions.TruncateTime(d.dt_aula) == data
                                    && ((d.hr_inicial_aula <= hr_inicial_aula && hr_inicial_aula < d.hr_final_aula)
                                              || (d.hr_inicial_aula < hr_final_aula && hr_final_aula <= d.hr_final_aula)
                                              || (d.hr_inicial_aula <= hr_inicial_aula && hr_final_aula <= d.hr_final_aula)
                                              || (d.hr_inicial_aula >= hr_inicial_aula && hr_final_aula >= d.hr_final_aula))
                                select d.cd_diario_aula).Any()
                                ||
                                (
                                p.cd_turma == cd_turma
                                && (!cd_programacao_turma.HasValue || p.cd_programacao_turma != cd_programacao_turma)
                                && !p.id_aula_dada
                                && data == DbFunctions.TruncateTime(p.dta_programacao_turma)
                                    && ((p.hr_inicial_programacao <= hr_inicial_aula && hr_inicial_aula < p.hr_final_programacao)
                                              || (p.hr_inicial_programacao < hr_final_aula && hr_final_aula <= p.hr_final_programacao)
                                              || (p.hr_inicial_programacao <= hr_inicial_aula && hr_final_aula <= p.hr_final_programacao)
                                              || (p.hr_inicial_programacao >= hr_inicial_aula && hr_final_aula >= p.hr_final_programacao))
                                    && (p.cd_feriado == null || p.cd_feriado_desconsiderado != null) //Não se trata de uma programação com feriado.
                                )
                            select p.cd_programacao_turma;

                existe_intersecao = sql.Count() > 0;

                return !existe_intersecao;
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            } 
        }

        public bool verificaIntersecaoTurmaPersonalizada(int cd_turma, int? cd_programacao_turma, TimeSpan hr_inicial_aula, TimeSpan hr_final_aula, DateTime data)
        {
            try
            {
                bool existe_intersecao = true;
                short status_cancelada = (int)DiarioAula.StatusDiarioAula.Cancelada;
                var sql = (from d in db.DiarioAula
                           where d.cd_turma == cd_turma
                               && (d.AulaPersonalizadaAluno.Any() || !d.AulaPersonalizadaAluno.Any())
                               && d.id_status_aula != status_cancelada
                               && (!cd_programacao_turma.HasValue || d.cd_programacao_turma != cd_programacao_turma)
                               && DbFunctions.TruncateTime(d.dt_aula) == data
                               && ((d.hr_inicial_aula <= hr_inicial_aula && hr_inicial_aula < d.hr_final_aula)
                                         || (d.hr_inicial_aula < hr_final_aula && hr_final_aula <= d.hr_final_aula)
                                         || (d.hr_inicial_aula <= hr_inicial_aula && hr_final_aula <= d.hr_final_aula)
                                         || (d.hr_inicial_aula >= hr_inicial_aula && hr_final_aula >= d.hr_final_aula))
                           select d.cd_diario_aula).Any();

                existe_intersecao = sql;

                return !existe_intersecao;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<DiarioAula> getDiarioAulas(int[] cdDiarios, int cd_escola)
        {
            try
            {
                var sql = from d in db.DiarioAula
                          where d.cd_pessoa_empresa == cd_escola && cdDiarios.Contains(d.cd_diario_aula)
                          select d;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool verificaDiarioAulaEfetivadoProf(int cd_turma, int cd_professor, int cd_escola)
        {
            try
            {
                var sql = from d in db.DiarioAula
                          where d.cd_pessoa_empresa == cd_escola && d.cd_turma == cd_turma && d.id_status_aula == (int)DiarioAula.StatusDiarioAula.Efetivada &&
                                (d.cd_professor == cd_professor || d.cd_professor_substituto == cd_professor)
                          select d;
                return sql.Count() > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool verificarDiarioAulaEfetivadoProfTurmasFilhasPPT(int cd_turma_ppt, int cd_professor, int cd_escola)
        {
            try
            {
                var sql = from d in db.DiarioAula
                          where d.cd_pessoa_empresa == cd_escola && d.Turma.cd_turma_ppt == cd_turma_ppt && d.id_status_aula == (int)DiarioAula.StatusDiarioAula.Efetivada &&
                                (d.cd_professor == cd_professor || d.cd_professor_substituto == cd_professor)
                          select d;
                return sql.Count() > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool verificaExisteDiarioEfetivoProgramacaoTurma(int cd_turma, int cd_escola, int cd_programacao)
        {
            try
            {
                var sql = from d in db.DiarioAula
                          where d.cd_pessoa_empresa == cd_escola && d.cd_turma == cd_turma && d.id_status_aula == (int)DiarioAula.StatusDiarioAula.Efetivada &&
                                d.cd_programacao_turma == cd_programacao && d.ProgramacaoTurma.id_aula_dada == true
                          select new { cd_diario = d.cd_diario_aula};
                return sql.Count() > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public int returnQuantidadeDiarioAulaByTurma(int cd_turma, int cd_escola, int cd_aluno)
        {
            try
            {
                var sql = from diario in db.DiarioAula
                          where diario.cd_pessoa_empresa == cd_escola 
                             && diario.cd_turma == cd_turma
                             && diario.id_status_aula == (int)DiarioAula.StatusDiarioAula.Efetivada
                             && diario.AlunoEvento.Where(ae => ae.cd_aluno == cd_aluno && ae.cd_diario_aula == diario.cd_diario_aula).Any()
                          select diario.cd_diario_aula;
                return sql.Count();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public DiarioAula getUltimaAulaAluno(int cd_aluno, int cd_turma, int cd_escola) {
            try {
                var sql = (from d in db.DiarioAula
                          where /*d.cd_pessoa_empresa == cd_escola &&*/
                              d.cd_turma == cd_turma
                             && d.id_status_aula == (int) DiarioAula.StatusDiarioAula.Efetivada
                             
                             && d.Turma.TurmaAluno.Where(at => at.cd_aluno == cd_aluno &&
                                   at.cd_turma == cd_turma &&
                                   /*at.Turma.cd_pessoa_escola == cd_escola &&*/
                                   at.Aluno.HistoricoAluno.Where(ha => ha.cd_aluno == cd_aluno &&
                                                          ha.cd_contrato == at.cd_contrato &&
                                                          ha.cd_turma == at.cd_turma &&
                                                          (ha.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                           ha.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                                           ha.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Encerrado) &&
                                                           DbFunctions.TruncateTime(ha.dt_historico) <= d.dt_aula

                                                          && !at.Aluno.HistoricoAluno.Where(han =>
                                                                han.cd_turma == cd_turma &&
                                                              !(han.id_situacao_historico == (int) AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                               han.id_situacao_historico == (int) AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                                               han.id_situacao_historico == (int) AlunoTurma.SituacaoAlunoTurma.Encerrado)
                                                               && DbFunctions.TruncateTime(han.dt_historico) <= d.dt_aula
                                                               && DbFunctions.TruncateTime(han.dt_historico) >= DbFunctions.TruncateTime(ha.dt_historico) && han.nm_sequencia > ha.nm_sequencia
                                                           ).Any()
                                                          ).Any()
                               ).Any()

                             && !d.AlunoEvento.Where(ae => ae.cd_aluno == cd_aluno && ae.cd_evento == (int)Evento.TiposEvento.FALTA).Any()
                          orderby d.dt_aula descending, d.hr_final_aula descending
                          select new
                          {
                              dt_aula = d.dt_aula,
                              hr_inicial_aula = d.hr_inicial_aula,
                              hr_final_aula = d.hr_final_aula,
                              tx_obs_aula = d.tx_obs_aula
                          }).ToList().Select(x => new DiarioAula {
                              dt_aula = x.dt_aula,
                              hr_inicial_aula = x.hr_inicial_aula,
                              hr_final_aula = x.hr_final_aula,
                              tx_obs_aula = x.tx_obs_aula
                          }).FirstOrDefault(); 
                return sql;
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

        public DiarioAula getUltimaAulaTurma(int cd_turma, int cd_escola) {
            try {
                var sql = (from d in db.DiarioAula
                           where /*d.cd_pessoa_empresa == cd_escola &&*/
                               d.cd_turma == cd_turma
                              && d.id_status_aula == (int) DiarioAula.StatusDiarioAula.Efetivada
                               orderby d.dt_aula descending, d.hr_final_aula descending
                           select new {
                               dt_aula = d.dt_aula,
                               hr_inicial_aula = d.hr_inicial_aula,
                               hr_final_aula = d.hr_final_aula,
                               tx_obs_aula = d.tx_obs_aula
                           }).ToList().Select(x => new DiarioAula {
                               dt_aula = x.dt_aula,
                               hr_inicial_aula = x.hr_inicial_aula,
                               hr_final_aula = x.hr_final_aula,
                               tx_obs_aula = x.tx_obs_aula
                           }).FirstOrDefault();
                return sql;
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

        public int returnDiarioByDataDesistencia(DateTime? data_desistencia, int cd_pessoa_escola, int cd_aluno, int cd_turma_aluno, int tipoDesistencia)
        {
            try
            {
                int retorno = 0;
                if (data_desistencia != null) {
                    var dt_aula_desistencia = data_desistencia.Value.Date;
                    if (tipoDesistencia == (int)DesistenciaDataAccess.tipoDesistencia.DESISTENCIA)
                        retorno = (from diario in db.DiarioAula
                                   where DbFunctions.TruncateTime(diario.dt_aula) < dt_aula_desistencia
                                  && diario.cd_pessoa_empresa == cd_pessoa_escola
                                  && diario.cd_turma == cd_turma_aluno
                                  && db.HistoricoAluno.Where(ha =>
                                                    ha.cd_aluno == cd_aluno && 
                                                    ha.cd_turma == diario.cd_turma &&
                                                    (DbFunctions.TruncateTime(ha.dt_historico) <= diario.dt_aula &&
                                                           (ha.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                            ha.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                                            ha.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Encerrado)
                                                            && ha.nm_sequencia == db.HistoricoAluno.Where(han => han.cd_turma == diario.cd_turma && han.cd_aluno == cd_aluno
                                                                                           && DbFunctions.TruncateTime(han.dt_historico) <= diario.dt_aula).Max(x=> x.nm_sequencia))
                                                    ).Any()
                               select new {diario.cd_diario_aula}).Count();
                    else
                        retorno = (from diario in db.DiarioAula
                               where diario.cd_pessoa_empresa == cd_pessoa_escola
                                  && diario.cd_turma == cd_turma_aluno
                                  && db.HistoricoAluno.Where(ha =>
                                                    ha.cd_aluno == cd_aluno && 
                                                    ha.cd_turma == diario.cd_turma &&
                                                    (DbFunctions.TruncateTime(ha.dt_historico) <= diario.dt_aula &&
                                                           (ha.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                            ha.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                                            ha.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Encerrado) &&
                                                            ha.nm_sequencia == db.HistoricoAluno.Where(han => han.cd_turma == diario.cd_turma && han.cd_aluno == cd_aluno
                                                                                           && DbFunctions.TruncateTime(han.dt_historico) <= diario.dt_aula).Max(x=> x.nm_sequencia))
                                                    ).Any()
                                   select new { diario.cd_diario_aula }).Count();
                }
                else //Se não existir data de desistência, intende-se que deve contar todos os diarios para o aluno, essa  consulta deve ser utilizada principalmente quando o usuario deletar todas a desistências
                    retorno = (from diario in db.DiarioAula
                           where diario.cd_pessoa_empresa == cd_pessoa_escola
                              && diario.cd_turma == cd_turma_aluno
                              && db.HistoricoAluno.Where(ha =>
                                                    ha.cd_aluno == cd_aluno && 
                                                    ha.cd_turma == diario.cd_turma &&
                                                    (DbFunctions.TruncateTime(ha.dt_historico) <= diario.dt_aula &&
                                                           (ha.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                            ha.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                                            ha.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Encerrado)
                                                           && ha.nm_sequencia == db.HistoricoAluno.Where(han => han.cd_turma == diario.cd_turma && han.cd_aluno == cd_aluno
                                                                                           && DbFunctions.TruncateTime(han.dt_historico) <= diario.dt_aula).Max(x => x.nm_sequencia))
                                                    ).Any()
                               select new { diario.cd_diario_aula }).Count();

                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public int quantidadeDiarioAulaTurma(int cd_turma, int cd_escola)
        {
            try
            {
                var sql = from diario in db.DiarioAula
                          where diario.cd_pessoa_empresa == cd_escola
                             && diario.cd_turma == cd_turma
                             && diario.id_status_aula == (int)DiarioAula.StatusDiarioAula.Efetivada
                          select diario.cd_diario_aula;
                return sql.Count();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public int qtdDiarioAulaTurmaData(int cd_turma, DateTime dt_diario)
        {
            try
            {
                var sql = from diario in db.DiarioAula
                          where diario.cd_turma == cd_turma
                             && diario.id_status_aula == (int)DiarioAula.StatusDiarioAula.Efetivada
                             && diario.dt_aula >= dt_diario
                          select diario.cd_diario_aula;
                return sql.Count();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public string getObsDiarioAula(int cd_diario_aula, int cd_escola)
        {
            try
            {
                var sql = (from diario in db.DiarioAula
                          where diario.cd_pessoa_empresa == cd_escola
                             && diario.cd_diario_aula == cd_diario_aula
                          select diario.tx_obs_aula).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public List<ProgramacaoTurma> verificaDiarioAposFeriado(int cd_feriado, int? cd_escola)
        {
            try
            {
                var programacoes = (from p in db.ProgramacaoTurma
                                where p.cd_feriado == cd_feriado &&
                                      (cd_escola.HasValue && p.Turma.cd_pessoa_escola == cd_escola || !cd_escola.HasValue) &&
                                       db.ProgramacaoTurma.Any(x => x.cd_turma == p.cd_turma && x.nm_aula_programacao_turma > p.nm_aula_programacao_turma &&
                                                                    ((x.id_aula_dada) || db.DiarioAula.Any(d => d.cd_programacao_turma == x.cd_programacao_turma)))
                                select p);

                foreach (var programacao in programacoes)
                    programacao.no_turma = db.Turma.Where(t => t.cd_turma == programacao.cd_turma).FirstOrDefault().no_turma;

                return programacoes.ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<DiarioAulaProgramcoesReport> getRelatorioDiarioAulaProgramacoes(int cd_escola, int? cd_turma, bool mais_turma_pagina, DateTime? dt_inicial, DateTime? dt_final, bool lancada)
        {
            try
            {
                var sql = from p in db.ProgramacaoTurma
                          where (p.Turma.cd_pessoa_escola == cd_escola ||
                                                      (from te in db.TurmaEscola
                                                       where te.cd_turma == p.Turma.cd_turma &&
                                                           te.cd_escola == cd_escola
                                                       select te).Any())
                          select p;
       
                if(cd_turma.HasValue)
                    sql = from p in sql
                          where (p.Turma.cd_turma == cd_turma.Value || p.Turma.TurmaPai.cd_turma == cd_turma.Value)
                          select p;

                if(dt_inicial.HasValue)
                    sql = from p in sql
                          where p.dta_programacao_turma >= dt_inicial
                          select p;

                if(dt_final.HasValue)
                    sql = from p in sql
                          where p.dta_programacao_turma <= dt_final
                          select p;

                if(lancada)
                    sql = from p in sql
                          where p.id_aula_dada == true
                          select p;


                IEnumerable<DiarioAulaProgramcoesReport> retorno = (from p in sql
                                                                    orderby p.Turma.Produto.no_produto, p.Turma.Curso.no_curso, p.Turma.no_turma, p.nm_aula_programacao_turma
                          select new
                           {
                               no_produto = p.Turma.Produto.no_produto,
                               dc_duracao = p.Turma.Duracao.dc_duracao,
                               no_curso = p.Turma.Curso.no_curso,
                               no_apelido = p.Turma.no_apelido,
                               no_turma = p.Turma.no_turma,
                               cd_turma_ppt = p.Turma.cd_turma_ppt,
                               no_aluno = p.Turma.cd_turma_ppt != null ? (from pe in db.PessoaSGF where pe.cd_pessoa == p.Turma.TurmaAluno.FirstOrDefault().Aluno.cd_pessoa_aluno select pe.no_pessoa).FirstOrDefault() : null,
                               TurmaProfessorTurma = (from proft in db.ProfessorTurma
                                                      
                                                      join f in db.FuncionarioSGF on proft.cd_professor equals f.cd_funcionario
                                                      join pe in db.PessoaSGF on f.cd_pessoa_funcionario equals pe.cd_pessoa
                                                      where p.cd_turma == proft.cd_turma && p.Turma.cd_pessoa_escola == cd_escola
                                                      select pe.dc_reduzido_pessoa),
                               cd_programacao_turma = p.cd_programacao_turma,
                               nm_aula_programacao_turma = p.nm_aula_programacao_turma,
                               dc_programacao_turma = p.dc_programacao_turma,
                               dt_programacao_turma = p.dta_programacao_turma,
                               tx_obs_aula = p.DiariosAula.FirstOrDefault().tx_obs_aula
                           }).ToList().Select(x => new DiarioAulaProgramcoesReport
                           {
                               no_produto = x.no_produto,
                               dc_duracao = x.dc_duracao,
                               no_curso = x.no_curso,
                               no_apelido = x.no_apelido,
                               no_turma = x.no_turma,
                               no_aluno = x.no_aluno,
                               nm_aula_programacao_turma = x.nm_aula_programacao_turma,

                               no_professores = x.TurmaProfessorTurma != null ? string.Join("/", x.TurmaProfessorTurma) : "",
                               
                               cd_programacao_turma = x.cd_programacao_turma,
                               dc_programacao_turma = x.dc_programacao_turma,
                               dt_programacao_turma = x.dt_programacao_turma,
                               tx_obs_aula = x.tx_obs_aula
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

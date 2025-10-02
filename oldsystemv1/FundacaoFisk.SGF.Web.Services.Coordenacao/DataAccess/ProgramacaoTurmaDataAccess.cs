using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Objects.SqlClient;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;
using FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess;
using System.Security.Cryptography;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess
{
    public class ProgramacaoTurmaDataAccess : GenericRepository<ProgramacaoTurma>, IProgramacaoTurmaDataAccess
    {
        public enum TipoConsultaProgTurmaEnum
        {
            HAS_PROG_TURMA = 0,
            HAS_PROG_TURMA_SEM_DIARIOAULA = 1,
            HAS_SUB_REPORT_PROG_TURMA = 2,
            HAS_REPORT_PROG_TURMA = 3,
            HAS_PROG_TURMA_SEM_LANCAR = 4
        }


        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public IEnumerable<ProgramacaoTurma> getProgramacoesTurma(int cd_escola, int cd_turma, int cd_professor, DateTime? dt_inicial, DateTime dt_final) {
            try
            {
                //DateTime firstSunday = new DateTime(1753, 1, 7); 
                var sql = from pt in db.ProgramacaoTurma
                                                    where pt.dta_programacao_turma <= dt_final
                                                    select pt;
 
                if(dt_inicial.HasValue)
                    sql = from pt in sql
                          where pt.dta_programacao_turma >= dt_inicial.Value
                          select pt;

                var sql2 = (from pt in sql
                      where (pt.cd_turma == cd_turma || pt.Turma.cd_turma_ppt == cd_turma) //&& pt.Turma.cd_pessoa_escola == cd_escola
                            && db.Horario.Any(h => h.HorariosProfessores.Where(hp => hp.cd_professor == cd_professor).Any() && (h.cd_registro == cd_turma || pt.Turma.cd_turma == cd_turma) && h.id_origem == (byte)Horario.Origem.TURMA
                                && pt.hr_inicial_programacao == h.dt_hora_ini
                                && h.cd_pessoa_escola == cd_escola
                                && pt.hr_final_programacao == h.dt_hora_fim
                                && System.Data.Entity.SqlServer.SqlFunctions.DatePart("dw", pt.dta_programacao_turma) == h.id_dia_semana &&
                                ((pt.cd_feriado == null) || (pt.cd_feriado != null && pt.cd_feriado_desconsiderado != null))
                                //&& EntityFunctions.DiffDays(firstSunday, pt.dta_cadastro_programacao) % 8 == h.id_dia_semana
                                //&& (byte)(System.Data.Objects.EntityFunctions.TruncateTime(pt.dta_cadastro_programacao).Value.DayOfWeek + 1) == h.id_dia_semana
                            )
                      orderby pt.nm_aula_programacao_turma
                      select new
                      {
                          dta_programacao_turma = pt.dta_programacao_turma,
                          dc_programacao_turma = pt.dc_programacao_turma,
                          nm_aula_programacao_turma = pt.nm_aula_programacao_turma,
                          pt.hr_inicial_programacao,                          
                          pt.hr_final_programacao,
                          cd_turma = pt.cd_turma,
                          id_turma_ppt = pt.Turma.id_turma_ppt,
                          cd_turma_ppt = pt.Turma.cd_turma_ppt,
                          id_prog_cancelada = pt.id_prog_cancelada,
                          id_aula_dada = pt.id_aula_dada,
                          cd_programacao_turma = pt.cd_programacao_turma,
                          nm_programacao_real = pt.nm_programacao_real,
                          tx_obs_aula = (
                              from d in db.DiarioAula
                              where pt.cd_programacao_turma == d.cd_programacao_turma &&
                                    pt.id_aula_dada == true && pt.id_prog_cancelada == false
                              select d.tx_obs_aula).FirstOrDefault()
                      }).ToList().Distinct().Select(x => new ProgramacaoTurma {
                          dta_programacao_turma = x.dta_programacao_turma,
                          dc_programacao_turma = x.dc_programacao_turma,
                          nm_aula_programacao_turma = x.nm_aula_programacao_turma,
                          hr_inicial_programacao = x.hr_inicial_programacao,
                          hr_final_programacao = x.hr_final_programacao,
                          //is_turma_regular = x.cd_turma == cd_turma ? true : false
                          is_turma_regular = x.cd_turma == cd_turma && x.id_turma_ppt == false && (x.cd_turma_ppt != null || x.cd_turma_ppt == null) ? true : false,
                          id_prog_cancelada = x.id_prog_cancelada,
                          id_aula_dada = x.id_aula_dada,
                          cd_programacao_turma = x.cd_programacao_turma,
                          nm_programacao_real = x.nm_programacao_real,
                          tx_obs_aula = x.tx_obs_aula
                      });

                return sql2;
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<ProgramacaoTurma> getProgramacaoTurmaByTurma(int cd_turma, int cd_escola, TipoConsultaProgTurmaEnum tipoConsulta, bool idMostrarFeriado)
        {
            try
            {
                IEnumerable<ProgramacaoTurma> sql = null;
                sql = from pt in db.ProgramacaoTurma
                      where pt.cd_turma == cd_turma //&& pt.Turma.cd_pessoa_escola == cd_escola
                      orderby pt.nm_aula_programacao_turma
                      select pt;
                switch (tipoConsulta)
                {
                    case TipoConsultaProgTurmaEnum.HAS_PROG_TURMA:
                        sql = from pt in db.ProgramacaoTurma
                              where pt.cd_turma == cd_turma //&& pt.Turma.cd_pessoa_escola == cd_escola
                              orderby pt.nm_aula_programacao_turma
                              select pt;
                        break;
                    case TipoConsultaProgTurmaEnum.HAS_REPORT_PROG_TURMA:
                        sql = from pt in db.ProgramacaoTurma
                              where pt.cd_turma == cd_turma //&& pt.Turma.cd_pessoa_escola == cd_escola
                              orderby pt.nm_aula_programacao_turma
                              select pt;
                        if (!idMostrarFeriado)
                            sql = from pt in sql
                                  where pt.cd_feriado == null
                                  select pt;
                        break;

                    case TipoConsultaProgTurmaEnum.HAS_PROG_TURMA_SEM_LANCAR:
                        sql = (from pt in db.ProgramacaoTurma
                               where pt.cd_turma == cd_turma && //pt.Turma.cd_pessoa_escola == cd_escola &&
                                     (pt.cd_feriado == null || pt.cd_feriado_desconsiderado != null) &&
                                     !pt.id_prog_cancelada && pt.id_aula_dada == false  // && pt.id_modificada == false
                               orderby pt.nm_aula_programacao_turma
                               select new
                               {
                                   cd_programacao_turma = pt.cd_programacao_turma,
                                   cd_turma = pt.cd_turma,
                                   nm_aula_programacao_turma = pt.nm_aula_programacao_turma,
                                   dc_programacao_turma = pt.dc_programacao_turma,
                                   dta_programacao_turma = pt.dta_programacao_turma,
                                   hr_inicial_programacao = pt.hr_inicial_programacao,
                                   hr_final_programacao = pt.hr_final_programacao,
                                   cd_sala = pt.Turma.cd_turma_ppt > 0 ? pt.Turma.TurmaPai.cd_sala : pt.Turma.cd_sala,
                                   id_aula_externa = pt.Turma.id_aula_externa,
                                   id_prog_cancelada = pt.id_prog_cancelada,
                                   nm_programacao_real = pt.nm_programacao_real
                               }).ToList().Select(x => new ProgramacaoTurma
                               {
                                   cd_programacao_turma = x.cd_programacao_turma,
                                   cd_turma = x.cd_turma,
                                   nm_aula_programacao_turma = x.nm_aula_programacao_turma,
                                   dc_programacao_turma = x.dc_programacao_turma,
                                   dta_programacao_turma = x.dta_programacao_turma,
                                   hr_inicial_programacao = x.hr_inicial_programacao,
                                   hr_final_programacao = x.hr_final_programacao,
                                   cd_sala_prog = x.cd_sala,
                                   aula_externa = x.id_aula_externa,
                                   id_prog_cancelada = x.id_prog_cancelada,
                                   nm_programacao_real = x.nm_programacao_real
                               });
                        break;

                    case TipoConsultaProgTurmaEnum.HAS_PROG_TURMA_SEM_DIARIOAULA:
                        sql = (from pt in db.ProgramacaoTurma
                               where pt.cd_turma == cd_turma && //pt.Turma.cd_pessoa_escola == cd_escola &&
                                     (pt.cd_feriado == null || pt.cd_feriado_desconsiderado != null) &&
                                     !pt.id_prog_cancelada && //pt.id_modificada == false
                                     (!pt.DiariosAula.Any() && pt.id_aula_dada == false || pt.DiariosAula.Where(d => d.id_status_aula == (int)DiarioAula.StatusDiarioAula.Cancelada && pt.id_aula_dada == false).Any())
                               orderby pt.nm_aula_programacao_turma
                               select new
                               {
                                   cd_programacao_turma = pt.cd_programacao_turma,
                                   cd_turma = pt.cd_turma,
                                   nm_aula_programacao_turma = pt.nm_aula_programacao_turma,
                                   dc_programacao_turma = pt.dc_programacao_turma,
                                   dta_programacao_turma = pt.dta_programacao_turma,
                                   hr_inicial_programacao = pt.hr_inicial_programacao,
                                   hr_final_programacao = pt.hr_final_programacao,
                                   cd_sala = pt.Turma.cd_turma_ppt > 0 ? pt.Turma.TurmaPai.cd_sala : pt.Turma.cd_sala,
                                   id_aula_externa = pt.Turma.id_aula_externa,
                                   id_prog_cancelada = pt.id_prog_cancelada,
                                   nm_programacao_real = pt.nm_programacao_real
                               }).ToList().Select(x => new ProgramacaoTurma {
                                   cd_programacao_turma = x.cd_programacao_turma,
                                   cd_turma = x.cd_turma,
                                   nm_aula_programacao_turma = x.nm_aula_programacao_turma,
                                   dc_programacao_turma = x.dc_programacao_turma,
                                   dta_programacao_turma = x.dta_programacao_turma,
                                   hr_inicial_programacao = x.hr_inicial_programacao,
                                   hr_final_programacao = x.hr_final_programacao,
                                   cd_sala_prog = x.cd_sala,
                                   aula_externa = x.id_aula_externa,
                                   id_prog_cancelada = x.id_prog_cancelada,
                                   nm_programacao_real = x.nm_programacao_real
                               });
                        break;
                    case TipoConsultaProgTurmaEnum.HAS_SUB_REPORT_PROG_TURMA:
                        sql = (from pt in db.ProgramacaoTurma
                              where   (cd_turma == 0 || pt.cd_turma == cd_turma) && pt.Turma.cd_pessoa_escola == cd_escola
                              orderby pt.nm_aula_programacao_turma
                               select new
                               {
                                   nm_aula_programacao_turma = pt.nm_aula_programacao_turma,
                                   dc_programacao_turma = pt.dc_programacao_turma,
                                   dta_programacao_turma = pt.dta_programacao_turma,
                                   id_prog_cancelada = pt.id_prog_cancelada,
                                   nm_programacao_real = pt.nm_programacao_real
                               }).ToList().Select(x => new ProgramacaoTurma {
                                   nm_aula_programacao_turma = x.nm_aula_programacao_turma,
                                   dc_programacao_turma = x.dc_programacao_turma,
                                   dta_programacao_turma = x.dta_programacao_turma,
                                   id_prog_cancelada = x.id_prog_cancelada
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

        public bool existeAulaEfetivadaTurma(int? cd_turma,int? cd_turma_ppt, int cd_escola) {
            try {
                var result = from pt in db.ProgramacaoTurma
                             where ((cd_turma.HasValue && pt.cd_turma == cd_turma.Value) ||
                                   (cd_turma_ppt.HasValue && pt.Turma.cd_turma_ppt == cd_turma_ppt.Value))
                             //&& pt.Turma.cd_pessoa_escola == cd_escola
                             && pt.id_aula_dada
                             select pt;
                return result.Count() > 0;
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }
        public string existeProgInsuficiente(Turma turma)
        {
            try
            {
                string resultado = "";
                var horarios = turma.horariosTurma;

                var qt_horarios = (from h in horarios
                                 group h by new
                                 {
                                     cd_registro = h.cd_registro
                                 } into g
                                 select new
                                 {
                                     qt_horario = g.Count(s => (s.cd_registro == g.Key.cd_registro))
                                 }).Select(x => x.qt_horario).FirstOrDefault();
                var qt_aulas = (from vi in db.vi_qt_programacao_curso where vi.cd_curso == turma.cd_curso && vi.cd_duracao == turma.cd_duracao select vi.qt_aulas_semana).FirstOrDefault();

                if (qt_horarios < qt_aulas)
                    resultado = " Programação e Horários não estão compatíveis. Verifique a videoaula - Cadastro de Turmas e Salas.";
                else
                    resultado = "OK";

                return resultado;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        //Método que busca as programações da turma que estão contidas no intervalo do feriado desonsiderado
        public IEnumerable<ProgramacaoTurma> getProgramacaoComDesconsideraFeriado(FeriadoDesconsiderado feriadoDesc) {
            try {
                var result = from pt in db.ProgramacaoTurma
                             where pt.cd_turma == feriadoDesc.cd_turma 
                                   && pt.dta_programacao_turma >= feriadoDesc.dt_inicial && pt.dta_programacao_turma <= feriadoDesc.dt_final
                             select pt;
                return result;
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

        public void atualizaProgramacaoRemovendoDesconsideraFeriado(int cd_feriado_desconsiderado) {
            try {
                var result = from pt in db.ProgramacaoTurma
                             where pt.cd_feriado_desconsiderado == cd_feriado_desconsiderado
                             select pt;

                if(result != null)
                    foreach(ProgramacaoTurma progTurma in result)
                        progTurma.cd_feriado_desconsiderado = null;

                db.SaveChanges();
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

        public int getQuantidadeAulasProgramadasTurma(int cd_turma, int cd_escola)
        {
            try {
                var sql = (from pt in db.ProgramacaoTurma
                           where pt.cd_turma == cd_turma && //pt.Turma.cd_pessoa_escola == cd_escola &&
                                 ((pt.cd_feriado == null) || (pt.cd_feriado != null && pt.cd_feriado_desconsiderado != null)) //and pt.id_modificada == false
                           select pt.cd_programacao_turma).Count();
                return sql;
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<ProgramacaoTurma> getProgramacoesTurmaPorAluno(int cd_escola, int cd_aluno, DateTime dt_inicial, int cdTurma, int cd_turma_principal, List<int> listaProgs)
        {
            try
            {
                IEnumerable<ProgramacaoTurma> sql = (from pt in db.ProgramacaoTurma
                                                     where //pt.Turma.cd_pessoa_escola == cd_escola &&
                                                      pt.cd_turma == cdTurma && 
                                                      (pt.cd_feriado == null || pt.cd_feriado_desconsiderado != null) &&
                                                      //pt.id_modificada == false &&
                                                      !listaProgs.Contains(pt.cd_programacao_turma) &&
                                                      pt.Turma.TurmaAluno.Any(ta => ta.cd_aluno == cd_aluno) &&
                                                     //pt.dta_programacao_turma >= dt_inicial &&
                                                     pt.id_aula_dada == false &&
                                                     pt.Turma.cd_turma_ppt > 0
                                                     orderby pt.Turma.no_turma
                                                     select new
                                                     {
                                                         cd_turma = pt.cd_turma,
                                                         cd_programacao_turma = pt.cd_programacao_turma,
                                                         dta_programacao_turma = pt.dta_programacao_turma,
                                                         dc_programacao_turma = pt.dc_programacao_turma,
                                                         nm_aula_programacao_turma = pt.nm_aula_programacao_turma,
                                                         no_turma = pt.Turma.no_turma,
                                                         cd_sala_prog = db.Turma.Where(x => x.cd_turma == cd_turma_principal).Select(s=> s.cd_sala).FirstOrDefault(),
                                                         cd_professor = db.ProfessorTurma.Any(p => p.id_professor_ativo && p.cd_turma == cd_turma_principal) ? db.ProfessorTurma.Where(p => p.id_professor_ativo && p.cd_turma == cd_turma_principal).FirstOrDefault().cd_professor : 0,
                                                         id_prog_cancelada = pt.id_prog_cancelada,
                                                         nm_programacao_real = pt.nm_programacao_real
                                                     }).OrderBy(x => x.cd_programacao_turma).Take(5).ToList().Select(x => new ProgramacaoTurma
                                                     {
                                                         cd_turma = x.cd_turma,
                                                         cd_programacao_turma = x.cd_programacao_turma,
                                                         dta_programacao_turma = x.dta_programacao_turma,
                                                         dc_programacao_turma = x.dc_programacao_turma,
                                                         nm_aula_programacao_turma = x.nm_aula_programacao_turma,
                                                         no_turma = x.no_turma,
                                                         cd_sala_prog = x.cd_sala_prog,
                                                         cd_professor = x.cd_professor,
                                                         id_prog_cancelada = x.id_prog_cancelada,
                                                         nm_programacao_real = x.nm_programacao_real
                                                     });
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool verificaProgramacoesComDiario(List<int> cds_prog, int cd_escola)
        {
            try
            {
                var sql = (from d in db.DiarioAula
                           where d.cd_programacao_turma > 0 && cds_prog.Contains((int)d.cd_programacao_turma) && d.cd_pessoa_empresa == cd_escola
                           select d.cd_diario_aula).Any();

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public List<ProgramacaoTurma> getProgramacaoTurmaEditEncerramentoTurma(int cd_turma, DateTime dt_termino)
        {
            try
            {
                var sql = (from p in db.ProgramacaoTurma
                           where p.cd_turma == cd_turma &&
                                 (p.cd_feriado == null || p.cd_feriado_desconsiderado != null) &&
                                 //p.id_modificada == false &&
                                 p.id_aula_dada == false && p.id_prog_cancelada == false &&
                                 p.dta_programacao_turma > dt_termino
                           select p).ToList();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }


        }
    }
}

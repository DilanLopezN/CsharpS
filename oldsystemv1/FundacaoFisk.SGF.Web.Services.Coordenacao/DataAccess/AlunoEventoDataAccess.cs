using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess
{
    public class AlunoEventoDataAccess : GenericRepository<AlunoEvento>, IAlunoEventoDataAccess 
    {
        public enum TipoConsultaAlunoEventoEnum
        {
            HAS_ALUNOS_EVENTO_DIARIO = 0,
        }

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public IEnumerable<AlunoEvento> getAlunoEvento(int cd_evento, int cd_aluno, int cd_pessoa_escola,int cd_diario_aula, TipoConsultaAlunoEventoEnum tipoConsulta)
        {
            try
            {
                var sql = from al in db.AlunoEvento
                          select al;
                switch (tipoConsulta)
                {
                    case TipoConsultaAlunoEventoEnum.HAS_ALUNOS_EVENTO_DIARIO:
                        sql = from ae in sql
                              where (cd_evento == 0 || ae.cd_evento == cd_evento) &&
                                    (cd_aluno == 0 || ae.cd_aluno == cd_aluno) &&
                                    ae.DiarioAula.cd_pessoa_empresa == cd_pessoa_escola &&
                                    ae.cd_diario_aula == cd_diario_aula
                              select ae;
                        break;
                }
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<AlunoEvento> getEventosAvaliacaoAluno(int cd_aluno, int cd_turma, int cd_escola)
        {
            try
            {
                var sql = (from al in db.AlunoEvento
                           where al.cd_aluno == cd_aluno
                               /*&& al.Aluno.cd_pessoa_escola == cd_escola*/
                               && al.DiarioAula.cd_turma == cd_turma
                               && al.DiarioAula.id_status_aula == (int)DiarioAula.StatusDiarioAula.Efetivada
                           orderby al.DiarioAula.dt_aula
                           select new
                            {
                                dt_aula = al.DiarioAula.dt_aula,
                                no_evento = al.Evento.no_evento,
                                cd_diario_aula = al.cd_diario_aula
                            }).ToList().Select(x => new AlunoEvento
                           {
                               DiarioAula = new DiarioAula { dt_aula = x.dt_aula },
                               Evento = new Evento { no_evento = x.no_evento },
                               cd_diario_aula = x.cd_diario_aula
                           });

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<AlunoEvento> getEventosAlunoByDataDesistencia(int cd_aluno, int cd_turma, int cd_escola, DateTime dta_Desistencia)
        {
            try
            {
                var sql = (from al in db.AlunoEvento
                           where al.cd_aluno == cd_aluno
                               && al.Aluno.cd_pessoa_escola == cd_escola
                               && al.DiarioAula.cd_turma == cd_turma
                               && DbFunctions.TruncateTime(al.DiarioAula.dt_aula) >= DbFunctions.TruncateTime(dta_Desistencia)
                           //&& al.DiarioAula.id_status_aula == (int) DiarioAula.StatusDiarioAula.Efetivada
                           select new
                            {
                                cd_evento = al.cd_evento,
                                cd_aluno_evento = al.cd_aluno_evento,
                                cd_diario_aula = al.cd_diario_aula
                            }).ToList().Select(x => new AlunoEvento
                           {
                               cd_evento = x.cd_evento,
                               cd_aluno_evento = x.cd_aluno_evento,
                               cd_diario_aula = x.cd_diario_aula
                           });

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool deleteAllAlunoEvento(List<AlunoEvento> alunoEventos)
        {
            try
            {
                string strAlunoEvento = "";
                if (alunoEventos != null && alunoEventos.Count > 0)
                    foreach (AlunoEvento e in alunoEventos)
                        strAlunoEvento += e.cd_aluno_evento + ",";

                // Remove o último ponto e virgula:
                if (strAlunoEvento.Length > 0)
                    strAlunoEvento = strAlunoEvento.Substring(0, strAlunoEvento.Length - 1);

                int retorno = db.Database.ExecuteSqlCommand("delete from t_aluno_evento where cd_aluno_evento in(" + strAlunoEvento + ")");

                return retorno > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<AlunoEvento> getAllAlunosEvento(int cd_diario_aula, int cd_empresa)
        {
            try
            {
                var retorno = from ae in db.AlunoEvento
                              where ae.DiarioAula.cd_pessoa_empresa == cd_empresa  && ae.cd_diario_aula == cd_diario_aula
                              select ae;
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        } 
    }
}

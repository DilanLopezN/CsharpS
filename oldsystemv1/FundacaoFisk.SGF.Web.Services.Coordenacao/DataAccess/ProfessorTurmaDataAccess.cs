using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess
{
    public class ProfessorTurmaDataAccess : GenericRepository<ProfessorTurma>, IProfessorTurmaDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }

        public IEnumerable<ProfessorTurma> findProfessoresTurmaPorTurmaEscola(int cd_turma, int cd_escola)
        {
            try
            {
                var sql = from professorT in db.ProfessorTurma
                          where professorT.cd_turma == cd_turma //&& professorT.Turma.cd_pessoa_escola == cd_escola
                          select professorT;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<ReportDiarioAula> getRelatorioDiarioAula(int cd_escola, int cd_turma, int cd_professor, DateTime dt_inicial, DateTime dt_final) {
            try {
                var sql = from p in db.ProfessorTurma
                          where (p.cd_turma == cd_turma || p.Turma.cd_turma_ppt == cd_turma) && p.cd_professor == cd_professor //&& p.Turma.cd_pessoa_escola == cd_escola
                          select new ReportDiarioAula
                                 {
                                     cd_professor = p.cd_professor,
                                     no_curso = p.Turma.Curso.no_curso,
                                     no_professor = p.Professor.FuncionarioPessoaFisica.no_pessoa,
                                     no_sala =  p.Turma.id_turma_ppt == false && p.Turma.cd_turma_ppt != null ? p.Turma.TurmaPai.Sala.no_sala : p.Turma.Sala.no_sala,                            
                                     no_turma = p.cd_turma == cd_turma ? p.Turma.no_turma : p.Turma.TurmaPai.no_turma,
                                     no_apelido = p.cd_turma == cd_turma ? p.Turma.no_apelido : p.Turma.TurmaPai.no_apelido,
                                     horarios = db.Horario.Where(h => h.HorariosProfessores.Where(hp => hp.cd_professor == cd_professor
                                         && hp.Horario.cd_registro == cd_turma && hp.Horario.id_origem == (int)Horario.Origem.TURMA).Any())
                                         .OrderBy(h => h.id_dia_semana).ThenBy(h => dt_inicial).ThenBy(h => h.dt_hora_fim)
                                 };

                return sql;
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }
    }
}

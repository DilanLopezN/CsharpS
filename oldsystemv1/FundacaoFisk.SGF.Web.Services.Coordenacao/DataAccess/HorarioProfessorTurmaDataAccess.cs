using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess
{
    public class HorarioProfessorTurmaDataAccess : GenericRepository<HorarioProfessorTurma>, IHorarioProfessorTurmaDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }

        public IEnumerable<HorarioProfessorTurma> getHorarioProfessorByHorario(int cd_horario) {
            try {
                return from hpt in db.HorarioProfessorTurma
                       where hpt.cd_horario == cd_horario
                       select hpt;
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<HorarioProfessorTurma> getHorarioProfessorByProfessorTurmaPPT(int cd_professor, int cd_turma_ppt)
        {
            try {
                var retorno = from hpt in db.HorarioProfessorTurma
                              where hpt.cd_professor == cd_professor &&
                                    (from t in db.Turma where t.ProfessorTurma.Where(pt => pt.cd_professor == cd_professor).Any() && t.cd_turma_ppt == cd_turma_ppt
                                     && t.dt_termino_turma == null
                                     select t.cd_turma).Any()
                              select hpt;
                return retorno;
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }
    }
}

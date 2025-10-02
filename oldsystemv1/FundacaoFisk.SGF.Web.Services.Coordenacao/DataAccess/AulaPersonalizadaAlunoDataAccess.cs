using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Componentes.GenericDataAccess.GenericException;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess
{
    public class AulaPersonalizadaAlunoDataAccess : GenericRepository<AulaPersonalizadaAluno>, IAulaPersonalizadaAlunoDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }
        public IEnumerable<AulaPersonalizadaAluno> searchAulaPersonalizadaAlunoByAulaAluno(int cdAulaPersonalizada, int cdEscola, int cd_aluno)
        {
            IEnumerable<AulaPersonalizadaAluno> retorno = (from a in db.AulaPersonalizadaAluno
                                                           where a.cd_aula_personalizada == cdAulaPersonalizada &&
                                                                 a.cd_aluno == cd_aluno &&
                                                                a.AulaPersonalizada.cd_escola == cdEscola
                                                           select new
                                                           {
                                                               cd_aula_personalizada_aluno = a.cd_aula_personalizada_aluno,
                                                               cd_aula_personalizada = a.cd_aula_personalizada,
                                                               cd_aluno = a.cd_aluno,
                                                               id_aula_dada = a.id_aula_dada,
                                                               tx_obs_aula = a.tx_obs_aula,
                                                               hh_inicial_aluno = a.hh_inicial_aluno,
                                                               hh_final_aluno = a.hh_final_aluno,
                                                               cd_diario_aula = a.cd_diario_aula,
                                                               no_aluno = a.Aluno.AlunoPessoaFisica.no_pessoa,
                                                               no_programacao = a.DiarioAula.ProgramacaoTurma.dc_programacao_turma,
                                                               cd_programacao_turma = a.DiarioAula.ProgramacaoTurma != null ? a.DiarioAula.ProgramacaoTurma.cd_programacao_turma : 0,
                                                               cd_turma = a.DiarioAula != null ? a.DiarioAula.cd_turma : 0
                                                           }).ToList().Select(x => new AulaPersonalizadaAluno
                                                           {
                                                               cd_aula_personalizada_aluno = x.cd_aula_personalizada_aluno,
                                                               cd_aula_personalizada = x.cd_aula_personalizada,
                                                               cd_aluno = x.cd_aluno,
                                                               id_aula_dada = x.id_aula_dada,
                                                               tx_obs_aula = x.tx_obs_aula,
                                                               hh_inicial_aluno = x.hh_inicial_aluno,
                                                               hh_final_aluno = x.hh_final_aluno,
                                                               cd_diario_aula = x.cd_diario_aula,
                                                               no_aluno = x.no_aluno,
                                                               dc_aula = x.no_programacao,
                                                               cd_programacao_turma = x.cd_programacao_turma,
                                                               cd_turma = x.cd_turma
                                                           });
            return retorno;
        }

        public IEnumerable<AulaPersonalizadaAluno> searchAulaPersonalizadaAlunoByAula(int cdAulaPersonalizada, int cdEscola)
        {
            IEnumerable<AulaPersonalizadaAluno> retorno = (from a in db.AulaPersonalizadaAluno
                                                           where a.cd_aula_personalizada == cdAulaPersonalizada &&
                                                                a.AulaPersonalizada.cd_escola == cdEscola
                                                           select new
                                                           {
                                                               cd_aula_personalizada_aluno = a.cd_aula_personalizada_aluno,
                                                               cd_aula_personalizada = a.cd_aula_personalizada,
                                                               cd_aluno = a.cd_aluno,
                                                               id_aula_dada = a.id_aula_dada,
                                                               tx_obs_aula = a.tx_obs_aula,
                                                               hh_inicial_aluno = a.hh_inicial_aluno,
                                                               hh_final_aluno = a.hh_final_aluno,
                                                               cd_diario_aula = a.cd_diario_aula,
                                                               no_aluno = a.Aluno.AlunoPessoaFisica.no_pessoa,
                                                               no_programacao = a.DiarioAula.ProgramacaoTurma.dc_programacao_turma,
                                                               cd_programacao_turma = a.DiarioAula.ProgramacaoTurma != null ? a.DiarioAula.ProgramacaoTurma.cd_programacao_turma : 0,
                                                               cd_turma = a.DiarioAula != null ? a.DiarioAula.cd_turma : 0
                                                           }).ToList().Select(x => new AulaPersonalizadaAluno
                                                           {
                                                               cd_aula_personalizada_aluno = x.cd_aula_personalizada_aluno,
                                                               cd_aula_personalizada = x.cd_aula_personalizada,
                                                               cd_aluno = x.cd_aluno,
                                                               id_aula_dada = x.id_aula_dada,
                                                               tx_obs_aula = x.tx_obs_aula,
                                                               hh_inicial_aluno = x.hh_inicial_aluno,
                                                               hh_final_aluno = x.hh_final_aluno,
                                                               cd_diario_aula = x.cd_diario_aula,
                                                               no_aluno = x.no_aluno,
                                                               dc_aula = x.no_programacao,
                                                               cd_programacao_turma = x.cd_programacao_turma,
                                                               cd_turma = x.cd_turma
                                                           });
            return retorno;
        }
    }
}

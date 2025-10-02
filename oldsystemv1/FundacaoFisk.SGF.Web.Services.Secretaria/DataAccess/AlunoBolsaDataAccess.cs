using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Secretaria.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess
{
    public class AlunoBolsaDataAccess : GenericRepository<AlunoBolsa>, IAlunoBolsaDataAccess
    {
        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }
        public enum TipoEventoEnum
        {
            FALTAJUST = 2
        }
        public IEnumerable<RptBolsistas> getBolsistas(int cdEscola, int cd_aluno, int cd_turma, bool cancelamento, decimal? per_bolsa, int cd_motivo_bolsa, DateTime? dtIniComunicado,
                                                        DateTime? dtFimComunicado, DateTime? dtIni, DateTime? dtFim, bool periodo_ini, bool periodo_cancel)
        {
           
            try
            {
                var sql = from ab in db.AlunoBolsa
                          where ab.Aluno.cd_pessoa_escola == cdEscola
                          select ab;
                if (cd_aluno > 0)
                    sql = from s in sql
                          where s.cd_aluno == cd_aluno
                          select s;

                if (cd_turma > 0)
                    sql = from s in sql
                          where db.AlunoTurma.Where(t => (t.cd_turma == cd_turma || t.Turma.cd_turma_ppt == cd_turma) && t.Aluno.cd_pessoa_escola == cdEscola && t.cd_aluno == s.cd_aluno).Any()
                          select s;

                if (per_bolsa != null)
                    sql = from s in sql
                          where s.pc_bolsa == per_bolsa
                          select s;
                
                if (cd_motivo_bolsa > 0)
                    if(cancelamento)
                    sql = from s in sql
                          where s.cd_motivo_cancelamento_bolsa == cd_motivo_bolsa
                          select s;
                    else
                        sql = from s in sql
                              where s.cd_motivo_bolsa == cd_motivo_bolsa
                              select s;

                if(dtIniComunicado != null)
                    sql = from s in sql
                          where s.dt_comunicado_bolsa.HasValue && DbFunctions.TruncateTime(s.dt_comunicado_bolsa) >= ((DateTime)dtIniComunicado).Date
                          select s;

                if (dtFimComunicado != null)
                    sql = from s in sql
                          where s.dt_comunicado_bolsa.HasValue && DbFunctions.TruncateTime(s.dt_comunicado_bolsa) <= ((DateTime)dtFimComunicado).Date
                          select s;

                if (periodo_ini)
                {
                    if (dtIni != null)
                        sql = from s in sql
                              where DbFunctions.TruncateTime(s.dt_inicio_bolsa) >= ((DateTime)dtIni).Date 
                              select s;

                    if (dtFim != null)
                        sql = from s in sql
                              where DbFunctions.TruncateTime(s.dt_inicio_bolsa) <= ((DateTime)dtFim).Date
                              select s;
                }

                if (periodo_cancel)
                {
                    if (dtIni != null)
                        sql = from s in sql
                              where s.dt_cancelamento_bolsa >= dtIni
                              select s;

                    if (dtFim != null)
                        sql = from s in sql
                              where s.dt_cancelamento_bolsa <= dtFim
                              select s;
                }
                if(cancelamento)
                    sql = from s in sql
                          where s.dt_cancelamento_bolsa.HasValue && 
                          s.cd_motivo_cancelamento_bolsa > 0
                          select s;


                IEnumerable<RptBolsistas> retorno = (from ab in sql
                                                     select new
                                                 {
                                                     cd_aluno = ab.cd_aluno,
                                                     no_aluno = ab.Aluno.AlunoPessoaFisica.no_pessoa,
                                                     pc_bolsa = ab.pc_bolsa,
                                                     dt_inicio_bolsa = ab.dt_inicio_bolsa,
                                                     dc_validade_bolsa = ab.dc_validade_bolsa,
                                                     pc_bolsa_material = ab.pc_bolsa_material,
                                                     dt_comunicado_bolsa = ab.dt_comunicado_bolsa,
                                                     dt_cancelamento_bolsa = ab.dt_cancelamento_bolsa,
                                                     cd_motivo_bolsa = ab.cd_motivo_bolsa,
                                                     dc_motivo_bolsa = ab.MotivoBolsa.dc_motivo_bolsa,
                                                     cd_motivo_cancelamento_bolsa = ab.cd_motivo_cancelamento_bolsa,
                                                     dc_motivo_cancelamento_bolsa = ab.MotivoCancelamento.dc_motivo_cancelamento_bolsa,
                                                     nr_faltas_justificadas = db.AlunoEvento.Where(ae => ae.cd_aluno == ab.cd_aluno && ae.cd_evento == (int)TipoEventoEnum.FALTAJUST).Count()

                                                 }).ToList().Select(x => new RptBolsistas
                                                     {
                                                         cd_aluno = x.cd_aluno,
                                                         no_aluno = x.no_aluno,
                                                         pc_bolsa = x.pc_bolsa,
                                                         dt_inicio_bolsa = x.dt_inicio_bolsa,
                                                         dc_validade_bolsa = x.dc_validade_bolsa,
                                                         pc_bolsa_material = x.pc_bolsa_material,
                                                         dt_comunicado_bolsa = x.dt_comunicado_bolsa,
                                                         dt_cancelamento_bolsa = x.dt_cancelamento_bolsa,
                                                         cd_motivo_bolsa = x.cd_motivo_bolsa,
                                                         dc_motivo_bolsa = x.dc_motivo_bolsa,
                                                         cd_motivo_cancelamento_bolsa = x.cd_motivo_cancelamento_bolsa,
                                                         dc_motivo_cancelamento_bolsa = x.dc_motivo_cancelamento_bolsa,
                                                         nr_faltas_justificadas = x.nr_faltas_justificadas
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

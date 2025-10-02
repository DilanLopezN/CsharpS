using System;
using System.Linq;
using Componentes.Utils;
using System.Data.Entity;
using System.Collections.Generic;
using FundacaoFisk.SGF.GenericModel;
using Componentes.GenericDataAccess;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess;
using Componentes.GenericDataAccess.GenericException;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;


namespace FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess
{
    public class EventoDataAccess : GenericRepository<Evento>, IEventoDataAccess
    {
        public enum TipoConsultaEventoEnum
        {
            HAS_ATIVO = 0
        }

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public IEnumerable<Evento> getEventoDesc(SearchParameters parametros, string desc, bool inicio, bool? ativo)
        {
            try{
                IQueryable<Evento> sql;
                IEntitySorter<Evento> sorter = EntitySorter<Evento>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);

                if (ativo == null)
                {
                    sql = from evento in db.Evento.AsNoTracking()
                          select evento;
                }
                else
                {
                    sql = from evento in db.Evento.AsNoTracking()
                          where evento.id_evento_ativo == ativo
                          select evento;
                }
                sql = sorter.Sort(sql);           
                var retorno = from evento in sql
                              select evento;
                
                if (!String.IsNullOrEmpty(desc))
                {
                    if (inicio)
                    {
                        retorno = from evento in sql
                                  where evento.no_evento.StartsWith(desc)
                                  select evento;
                    }//end if
                    else
                    {
                        retorno = from evento in sql
                                  where evento.no_evento.Contains(desc)
                                  select evento;
                    }// end if    
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

        public bool deleteAllEvento(List<Evento> eventos)
        {
            try{
                string strEventos = "";
                if (eventos != null && eventos.Count > 0)
                    foreach (Evento e in eventos)
                        strEventos += e.cd_evento + ",";

                // Remove o último ponto e virgula:
                if (strEventos.Length > 0)
                    strEventos = strEventos.Substring(0, strEventos.Length - 1);

                int retorno = db.Database.ExecuteSqlCommand("delete from t_evento where cd_evento in(" + strEventos + ")");

                return retorno > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Evento> getEventos(int cd_evento,TipoConsultaEventoEnum tipoConsulta)
        {
            try{

                var sql = from e in db.Evento
                          select e;

                          switch (tipoConsulta)
                {
                    case TipoConsultaEventoEnum.HAS_ATIVO:
                        sql = from evt in sql
                              where evt.id_evento_ativo == true && (cd_evento == 0 || evt.cd_evento == cd_evento)
                              select evt;
                        break;
                }
                return sql;

            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<AlunoEventoReport> getRelatorioEventos(int cd_escola, int? cd_turma, int? cd_professor, int? cd_evento, int? qtd_faltas, DateTime? dt_inicial, DateTime? dt_final) {
            try {

                var sql = from a in db.AlunoEvento
                          where a.DiarioAula.cd_pessoa_empresa == cd_escola
                            && a.DiarioAula.id_status_aula == (short)DiarioAula.StatusDiarioAula.Efetivada
                          select a;

                if(cd_turma.HasValue)
                    sql = from a in sql
                          where (a.DiarioAula.cd_turma == cd_turma.Value || a.DiarioAula.Turma.cd_turma_ppt == cd_turma.Value)
                          select a;

                if(cd_professor.HasValue)
                    sql = from a in sql
                          where (a.DiarioAula.Professor.cd_funcionario == cd_professor || a.DiarioAula.ProfessorSubstituto.cd_funcionario == cd_professor)
                          select a;

                if(cd_evento.HasValue)
                    sql = from a in sql
                          where a.Evento.cd_evento == cd_evento
                          select a;

                if(dt_inicial.HasValue)
                    sql = from a in sql
                          where DbFunctions.TruncateTime(a.DiarioAula.dt_aula) >= dt_inicial.Value
                          select a;

                if(dt_final.HasValue)
                    sql = from a in sql
                          where DbFunctions.TruncateTime(a.DiarioAula.dt_aula) <= dt_final.Value
                          select a;
                var sql2 = (from a in sql
                      select new
                           {
                               cd_funcionario = a.DiarioAula.Professor != null ? a.DiarioAula.Professor.cd_funcionario : a.DiarioAula.ProfessorSubstituto.cd_funcionario,
                               no_professor = a.DiarioAula.Professor != null ? a.DiarioAula.Professor.FuncionarioPessoaFisica.no_pessoa : a.DiarioAula.ProfessorSubstituto.FuncionarioPessoaFisica.no_pessoa,
                               cd_aluno = a.Aluno.cd_aluno,
                               no_aluno = a.Aluno.AlunoPessoaFisica.cd_pessoa_cpf == null ? a.Aluno.AlunoPessoaFisica.no_pessoa : a.Aluno.AlunoPessoaFisica.no_pessoa + " - " + a.Aluno.AlunoPessoaFisica.PessoaSGFQueUsoOCpf.no_pessoa,
                               dc_fone_mail_principal = a.Aluno.AlunoPessoaFisica.Telefone.dc_fone_mail,
                               dc_fone_mail = a.Aluno.AlunoPessoaFisica.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int) TipoTelefoneSGF.TipoTelefoneSGFEnum.CELULAR && t.id_telefone_principal).FirstOrDefault().dc_fone_mail,
                               cd_evento = a.Evento.cd_evento,
                               no_evento = a.Evento.no_evento,
                               cd_turma = a.DiarioAula.Turma.cd_turma,
                               no_turma = a.DiarioAula.Turma.no_turma,
                               cd_diario_aula = a.DiarioAula.cd_diario_aula,
                               nm_aula_turma = a.DiarioAula.nm_aula_turma,
                               hr_inicial_aula = a.DiarioAula.hr_inicial_aula,
                               dt_aula = a.DiarioAula.dt_aula
                           }).ToList().Select(x => new AlunoEventoReport {
                               cd_funcionario = x.cd_funcionario,
                               no_professor = x.no_professor,
                               cd_aluno = x.cd_aluno,
                               no_aluno = x.no_aluno,
                               dc_fone_mail_principal = x.dc_fone_mail_principal,
                               dc_fone_mail = x.dc_fone_mail,
                               cd_evento = x.cd_evento,
                               no_evento = x.no_evento,
                               cd_turma = x.cd_turma,
                               no_turma = x.no_turma,
                               cd_diario_aula = x.cd_diario_aula,
                               nm_aula_turma = x.nm_aula_turma,
                               hr_inicial_aula = x.hr_inicial_aula,
                               dt_aula = x.dt_aula
                           });

                return sql2;

            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<AlunoEvento> getEventosRtpDiarioAula(int cd_escola, int cd_aluno, int cd_professor, DateTime dataAula)
        {
            try
            {
                var sql = (from e in db.AlunoEvento
                           from d in db.DiarioAula
                           from p in db.ProgramacaoTurma
                           where e.cd_diario_aula == d.cd_diario_aula &&
                                 d.cd_programacao_turma == p.cd_programacao_turma &&
                                 e.cd_aluno == cd_aluno &&
                                 //p.cd_programacao_turma == cd_programacao_turma &&
                                 DbFunctions.TruncateTime(d.dt_aula) == dataAula.Date &&
                                                          d.cd_pessoa_empresa == cd_escola &&
                                                          d.cd_professor == cd_professor
                           select e).AsEnumerable();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }


        public IEnumerable<Aluno> getAlunoIsTurmaInDate(int cd_turma, int cd_aluno, int cd_pessoa_escola, DateTime dtAula)
        {
            try
            {
                List<int> cds_turmas_escolas = new List<int>();
                List<TurmaEscola> turmasEscola = new List<TurmaEscola>();

                int cdPessoaEscola = (from t in db.Turma
                                      where (t.cd_turma == cd_turma || t.cd_turma_ppt == cd_turma)
                                      select t.cd_pessoa_escola).FirstOrDefault();

                turmasEscola = (from t in db.Turma
                                join te in db.TurmaEscola on t.cd_turma equals te.cd_turma
                                where t.cd_turma == cd_turma
                                select te).ToList();

                cds_turmas_escolas = turmasEscola.Select(x => x.cd_escola).ToList();


                var sql = (from a in db.Aluno
                           orderby a.AlunoPessoaFisica.no_pessoa
                           where a.cd_aluno == cd_aluno && ((a.cd_pessoa_escola == cd_pessoa_escola && (cdPessoaEscola == 0 || cdPessoaEscola != cd_pessoa_escola)) ||
                                  (a.cd_pessoa_escola == cd_pessoa_escola || cds_turmas_escolas.Contains(a.cd_pessoa_escola) && cdPessoaEscola == cd_pessoa_escola)) &&
                                a.HistoricoAluno.Where(ha =>
                                                      ((ha.cd_turma == cd_turma) || (ha.Turma.cd_turma_ppt == cd_turma)) &&
                                                       (DbFunctions.TruncateTime(ha.dt_historico) <= dtAula.Date &&
                                                        (ha.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                                         ha.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                                         ha.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Encerrado)
                                                         && ha.nm_sequencia == a.HistoricoAluno.Where(han => ((han.cd_turma == cd_turma) || (han.Turma.cd_turma_ppt == cd_turma))
                                                                                           && DbFunctions.TruncateTime(han.dt_historico) <= dtAula.Date).Max(x => x.nm_sequencia)
                                                        )
                                                      ).Any()
                           select new
                           {
                               cd_aluno = a.cd_aluno,
                               no_aluno = a.AlunoPessoaFisica.no_pessoa,
                               
                           }).ToList().Select(x => new Aluno
                           {
                               cd_aluno = x.cd_aluno,
                               nomeAluno = x.no_aluno,
                               
                           });
                return sql;

            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}

using Componentes.GenericDataAccess;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Secretaria.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess
{
    public class DesistenciaDataAccess : GenericRepository<Desistencia>, IDesistenciaDataAccess
    {
        public enum tipoDesistencia
        {
            TODOS = 0,
            DESISTENCIA = 1,
            CANCELAMENTO = 2,
            NAOREMATRICULA = 3,
            CANCELAMENTONAOREMATRICULA = 4
        }

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }

        public IEnumerable<DesistenciaUI> getDesistenciaSearchUI(SearchParameters parametros, int? cd_turma, int? cd_aluno, int? cd_pessoa_escola, int? cd_motivo_desistencia, int cd_tipo, DateTime? dta_ini,
            DateTime? dta_fim, int cd_produto, int cd_professor, List<int> cdsCurso)
        {

            try
            {

                IEntitySorter<DesistenciaUI> sorter = EntitySorter<DesistenciaUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);

                var sql = from desistencia in db.Desistencia.AsNoTracking()
                          where desistencia.AlunoTurma.Aluno.cd_pessoa_escola == cd_pessoa_escola
                              && desistencia.dt_desistencia >= dta_ini
                              && desistencia.dt_desistencia <= dta_fim
                          select desistencia;

                if (cd_motivo_desistencia.Value > 0)
                    sql = from m in sql
                          where m.cd_motivo_desistencia == cd_motivo_desistencia
                          select m;

                if (cd_tipo > (byte)tipoDesistencia.TODOS)
                    switch (cd_tipo)
                    {
                        case (byte)tipoDesistencia.DESISTENCIA:
                            {
                                sql = from t in sql
                                      where t.id_tipo_desistencia == (byte)tipoDesistencia.DESISTENCIA
                                      select t;
                                break;
                            }
                        case (byte)tipoDesistencia.NAOREMATRICULA:
                            {

                                sql = from t in sql
                                      where t.id_tipo_desistencia == (byte)tipoDesistencia.NAOREMATRICULA
                                      select t;
                                break;
                            }
                        case (byte)tipoDesistencia.CANCELAMENTONAOREMATRICULA:
                            {
                                sql = from t in sql
                                      where t.id_tipo_desistencia == (byte)tipoDesistencia.CANCELAMENTONAOREMATRICULA
                                      select t;
                                break;
                            }
                        default:
                            {
                                sql = from t in sql
                                      where t.id_tipo_desistencia == (byte)tipoDesistencia.CANCELAMENTO
                                      select t;
                                break;
                            }
                    }

                if (cd_turma.Value > 0)
                    sql = from turma in sql
                          where turma.AlunoTurma.Turma.cd_turma == cd_turma
                          select turma;

                if (cd_aluno.Value > 0)
                    sql = from aluno in sql
                          where aluno.AlunoTurma.cd_aluno == cd_aluno
                          select aluno;   

                if(cd_produto > 0)
                    sql = from d in sql
                          where d.AlunoTurma.Turma.cd_produto == cd_produto
                          select d;

                if (cd_professor > 0)
                    sql = from d in sql
                          where d.AlunoTurma.Turma.TurmaProfessorTurma.Any(x => x.cd_professor == cd_professor)
                          select d;

                if (cdsCurso.Count() > 0)
                    sql = from d in sql
                          where cdsCurso.Contains((int)d.AlunoTurma.Turma.cd_curso)
                          select d;   


                var retorno = from desistencia in sql
                              select new DesistenciaUI
                              {
                                  cd_desistencia = desistencia.cd_desistencia,
                                  cd_aluno = desistencia.AlunoTurma.cd_aluno,
                                  cd_motivo_desistencia = desistencia.MotivoDesistencia.cd_motivo_desistencia,
                                  cd_pessoa = desistencia.AlunoTurma.Aluno.cd_pessoa_aluno,
                                  cd_turma = desistencia.AlunoTurma.Turma.cd_turma,
                                  dc_motivo_desistencia = desistencia.MotivoDesistencia.dc_motivo_desistencia,
                                  no_pessoa = desistencia.AlunoTurma.Aluno.AlunoPessoaFisica.no_pessoa,
                                  no_turma = desistencia.AlunoTurma.Turma.no_turma,
                                  dt_desistencia = desistencia.dt_desistencia,
                                  dc_tipo = desistencia.id_tipo_desistencia == (byte)tipoDesistencia.CANCELAMENTO ? "Cancelamento" :
                                            desistencia.id_tipo_desistencia == (byte)tipoDesistencia.DESISTENCIA  ?    "Desistência" : 
                                            desistencia.id_tipo_desistencia == (byte)tipoDesistencia.NAOREMATRICULA  ?    "Não Rematrículado" :  "Cancelamento Não Rematrícula",
                                  id_tipo_desistencia = desistencia.id_tipo_desistencia,
                                  cd_aluno_turma = desistencia.cd_aluno_turma,
                                  tx_obs_desistencia = desistencia.tx_obs_desistencia,
                                  cd_contrato = desistencia.AlunoTurma.cd_contrato,
                                  cd_pessoa_responsavel_titulo = desistencia.AlunoTurma.Contrato.cd_pessoa_responsavel,
                                  id_cancelamento = desistencia.MotivoDesistencia.id_cancelamento,
                                  id_aluno_ativo = desistencia.AlunoTurma.Aluno.id_aluno_ativo,
                                  telefone = desistencia.AlunoTurma.Aluno.AlunoPessoaFisica.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.TELEFONE).Any() ?   desistencia.AlunoTurma.Aluno.AlunoPessoaFisica.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.TELEFONE).FirstOrDefault().dc_fone_mail : desistencia.AlunoTurma.Aluno.AlunoPessoaFisica.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.CELULAR).FirstOrDefault().dc_fone_mail
                              };

                retorno = sorter.Sort(retorno);

                int limite = sql.Count();

                parametros.ajustaParametrosPesquisa(limite);
                sql = sql.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;

                return retorno;

            }
            catch (Exception ex)
            {

                throw new DataAccessException(ex);
            }
        }

        public bool deleteAllDesistencia(List<DesistenciaUI> desistencia)
        {
            try
            {
                string strDesistencia = "";
                if (desistencia != null && desistencia.Count > 0)
                    foreach (var item in desistencia)
                        strDesistencia += item.cd_desistencia + ",";

                // Remove o último ponto e virgula:
                if (strDesistencia.Length > 0)
                    strDesistencia = strDesistencia.Substring(0, strDesistencia.Length - 1);

                int retorno = db.Database.ExecuteSqlCommand("delete from t_desistencia where cd_desistencia in(" + strDesistencia + ")");
                return retorno > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Desistencia retornaDesistenciaMax(DesistenciaUI desistenciaUI, int cd_pessoa_escola)
        {
            try
            {
                //verifica o ultimo registro da desistência
                var ultimaDesistencia = (from desistencia in db.Desistencia
                                         where desistencia.AlunoTurma.cd_aluno == desistenciaUI.cd_aluno
                                            && desistencia.AlunoTurma.cd_turma == desistenciaUI.cd_turma
                                            && desistencia.AlunoTurma.Aluno.cd_pessoa_escola == cd_pessoa_escola
                                            && desistencia.cd_desistencia == (db.Desistencia.Where(d => d.cd_aluno_turma == desistenciaUI.cd_aluno_turma).Select(d => d.cd_desistencia).Max())
                                         select desistencia).FirstOrDefault();

                return ultimaDesistencia;

            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }

        }

        public DesistenciaUI getDesistenciaAlunoTurma(int cd_aluno_turma, int cd_pessoa_escola)
        {

            try
            {
                var retorno = (from desistencia in db.Desistencia
                               where desistencia.AlunoTurma.Aluno.cd_pessoa_escola == cd_pessoa_escola &&
                                     desistencia.cd_aluno_turma == cd_aluno_turma
                               select new DesistenciaUI
                               {
                                   cd_desistencia = desistencia.cd_desistencia,
                                   cd_motivo_desistencia = desistencia.MotivoDesistencia.cd_motivo_desistencia,
                                   cd_pessoa = desistencia.AlunoTurma.Aluno.cd_aluno,
                                   cd_turma = desistencia.AlunoTurma.Turma.cd_turma,
                                   dc_motivo_desistencia = desistencia.MotivoDesistencia.dc_motivo_desistencia,
                                   no_pessoa = desistencia.AlunoTurma.Aluno.AlunoPessoaFisica.no_pessoa,
                                   no_turma = desistencia.AlunoTurma.Turma.no_turma,
                                   dt_desistencia = desistencia.dt_desistencia,
                                   dc_tipo = desistencia.id_tipo_desistencia == (byte)tipoDesistencia.CANCELAMENTO ? "Cancelamento" : "Desistência",
                                   id_tipo_desistencia = desistencia.id_tipo_desistencia,
                                   cd_aluno_turma = desistencia.cd_aluno_turma,
                                   tx_obs_desistencia = desistencia.tx_obs_desistencia
                               }).FirstOrDefault();


                return retorno;

            }
            catch (Exception ex)
            {

                throw new DataAccessException(ex);
            }
        }

        public bool getExisteDesistenciaPorAlunoTurma(int cd_aluno, int cd_turma, int cd_pessoa_escola)
        {

            try
            {
                var retorno = (from desistencia in db.Desistencia
                               where desistencia.AlunoTurma.Aluno.cd_pessoa_escola == cd_pessoa_escola &&
                                     desistencia.AlunoTurma.cd_aluno == cd_aluno &&
                                     desistencia.AlunoTurma.cd_turma == cd_turma
                               select desistencia.cd_desistencia).FirstOrDefault();


                return retorno > 0;

            }
            catch (Exception ex)
            {

                throw new DataAccessException(ex);
            }
        }

        public bool getMaiorDataAposDataDesistencia(DesistenciaUI desistenciaUI, int cd_pessoa_escola)
        {
            try
            {
                DesistenciaUI sql;
                DateTime dataEditada = desistenciaUI.dt_desistencia.ToLocalTime().Date;
                if (desistenciaUI.id_tipo_desistencia == (byte)DesistenciaDataAccess.tipoDesistencia.DESISTENCIA)
                    sql = (from desistencia in db.Desistencia
                           where desistencia.AlunoTurma.cd_aluno == desistenciaUI.cd_aluno
                              && desistencia.AlunoTurma.cd_turma == desistenciaUI.cd_turma
                              && desistencia.AlunoTurma.Aluno.cd_pessoa_escola == cd_pessoa_escola
                              && desistencia.cd_desistencia != desistenciaUI.cd_desistencia
                              && DbFunctions.TruncateTime(desistencia.dt_desistencia) >= DbFunctions.TruncateTime(desistenciaUI.dt_desistencia) // maior data desistência que existe depois dessa data que esta sendo modificada
                           orderby desistencia.dt_desistencia
                           select new
                           {
                               cd_desistencia = desistencia.cd_desistencia,
                               nm_odrdem = desistencia.HistoricoAluno.Where(ha => ha.cd_desistencia == desistencia.cd_desistencia).FirstOrDefault().nm_sequencia
                           }).ToList().Select(x => new DesistenciaUI { 
                               cd_desistencia = x.cd_desistencia,
                               nm_ordem = x.nm_odrdem
                           }).FirstOrDefault();
                else
                    sql = (from desistencia in db.Desistencia
                           where desistencia.AlunoTurma.cd_aluno == desistenciaUI.cd_aluno
                              && desistencia.AlunoTurma.cd_turma == desistenciaUI.cd_turma
                              && desistencia.AlunoTurma.Aluno.cd_pessoa_escola == cd_pessoa_escola
                              && desistencia.cd_desistencia != desistenciaUI.cd_desistencia
                              && DbFunctions.TruncateTime(desistencia.dt_desistencia) >= DbFunctions.TruncateTime(desistenciaUI.dt_desistencia) // maior data desistência que existe depois dessa data que esta sendo modificada
                           orderby desistencia.dt_desistencia
                           select new
                           {
                               cd_desistencia = desistencia.cd_desistencia,
                               nm_odrdem = desistencia.HistoricoAluno.Where(ha => ha.cd_desistencia == desistencia.cd_desistencia).FirstOrDefault().nm_sequencia
                           }).ToList().Select(x => new DesistenciaUI
                           {
                               cd_desistencia = x.cd_desistencia,
                               nm_ordem = x.nm_odrdem
                           }).FirstOrDefault();


                bool existe = sql != null && sql.cd_desistencia > 0;
                if (sql != null && desistenciaUI.dt_desistencia == sql.dt_desistencia)
                    existe = (from h in db.HistoricoAluno
                                        where h.cd_desistencia != null
                                              && h.cd_aluno == desistenciaUI.cd_aluno
                                              && h.cd_turma == desistenciaUI.cd_turma
                                              && h.Aluno.cd_pessoa_escola == cd_pessoa_escola
                                              && h.cd_desistencia != desistenciaUI.cd_desistencia
                                              && h.nm_sequencia > sql.nm_ordem
                                        select h.cd_historico_aluno).Any();


                return existe;
            }
            catch (Exception ex)
            {
                throw new DataAccessException(ex);
            }
        }

        public DateTime? getMenorDataAposDataDesistencia(DesistenciaUI desistenciaUI, int cd_pessoa_escola)
        {
            try
            {
                DateTime? retorno = null;
                Desistencia sql;

                if (desistenciaUI.id_tipo_desistencia == (byte)DesistenciaDataAccess.tipoDesistencia.DESISTENCIA)
                    sql = (from desistencia in db.Desistencia
                           where desistencia.AlunoTurma.cd_aluno == desistenciaUI.cd_aluno
                              && desistencia.AlunoTurma.cd_turma == desistenciaUI.cd_turma
                              && desistencia.AlunoTurma.Aluno.cd_pessoa_escola == cd_pessoa_escola
                              && desistencia.cd_desistencia != desistenciaUI.cd_desistencia
                              && DbFunctions.TruncateTime(desistencia.dt_desistencia) < DbFunctions.TruncateTime(desistenciaUI.dt_desistencia) // maior data desistência que existe depois dessa data que esta sendo modificada
                           select desistencia).OrderByDescending(de => de.dt_desistencia).FirstOrDefault();
                else
                    sql = (from desistencia in db.Desistencia
                           where desistencia.AlunoTurma.cd_aluno == desistenciaUI.cd_aluno
                              && desistencia.AlunoTurma.cd_turma == desistenciaUI.cd_turma
                              && desistencia.AlunoTurma.Aluno.cd_pessoa_escola == cd_pessoa_escola
                              && desistencia.cd_desistencia != desistenciaUI.cd_desistencia
                              && DbFunctions.TruncateTime(desistencia.dt_desistencia) <= DbFunctions.TruncateTime(desistenciaUI.dt_desistencia) // maior data desistência que existe depois dessa data que esta sendo modificada
                           select desistencia).OrderByDescending(de => de.dt_desistencia).FirstOrDefault();


                if (sql != null)
                    retorno = sql.dt_desistencia;

                return retorno;
            }
            catch (Exception ex)
            {
                throw new DataAccessException(ex);
            }
        }

        public int retornaQuantidadeDesistencia(int cd_turma, int cd_aluno, int cd_pessoa_escola, int cd_aluno_turma)
        {
            try
            {
                var sql = (from desistencia in db.Desistencia
                           where desistencia.AlunoTurma.cd_aluno == cd_aluno
                             && desistencia.AlunoTurma.cd_turma == cd_turma
                             && desistencia.AlunoTurma.Aluno.cd_pessoa_escola == cd_pessoa_escola
                             && desistencia.cd_aluno_turma == cd_aluno_turma
                           select desistencia.cd_desistencia).Count();
                return sql;

            }
            catch (Exception ex)
            {
                throw new DataAccessException(ex);
            }
        }

        /// <summary>
        /// Pega a maior data de desistência diferente da que esta sendo passada
        /// </summary>
        /// <param name="desistenciaUI"></param>
        /// <param name="cd_pessoa_escola"></param>
        /// <returns></returns>
        public DateTime? getMaiorDataDesistencia(DesistenciaUI desistenciaUI, int cd_pessoa_escola)
        {
            try
            {
                DateTime? retorno = null;
                Desistencia sql;

                sql = (from desistencia in db.Desistencia
                       where desistencia.AlunoTurma.cd_aluno == desistenciaUI.cd_aluno
                          && desistencia.AlunoTurma.cd_turma == desistenciaUI.cd_turma
                          && desistencia.AlunoTurma.Aluno.cd_pessoa_escola == cd_pessoa_escola
                          && desistencia.cd_desistencia != desistenciaUI.cd_desistencia
                          && DbFunctions.TruncateTime(desistencia.dt_desistencia) <= DbFunctions.TruncateTime(desistenciaUI.dt_desistencia) // maior data desistência que existe depois dessa data que esta sendo modificada
                       orderby desistencia.dt_desistencia descending
                       select desistencia).FirstOrDefault();

                if (sql != null)
                    retorno = sql.dt_desistencia;

                return retorno;
            }
            catch (Exception ex)
            {
                throw new DataAccessException(ex);
            }
        }

        public DesistenciaUI getDesistenciaGridView(int cd_desistencia)
        {

            try
            {


                var sql = (from desistencia in db.Desistencia
                           where desistencia.cd_desistencia == cd_desistencia
                              select new DesistenciaUI
                              {
                                  cd_desistencia = desistencia.cd_desistencia,
                                  cd_aluno = desistencia.AlunoTurma.cd_aluno,
                                  cd_motivo_desistencia = desistencia.MotivoDesistencia.cd_motivo_desistencia,
                                  cd_pessoa = desistencia.AlunoTurma.Aluno.cd_pessoa_aluno,
                                  cd_turma = desistencia.AlunoTurma.Turma.cd_turma,
                                  dc_motivo_desistencia = desistencia.MotivoDesistencia.dc_motivo_desistencia,
                                  no_pessoa = desistencia.AlunoTurma.Aluno.AlunoPessoaFisica.no_pessoa,
                                  no_turma = desistencia.AlunoTurma.Turma.no_turma,
                                  dt_desistencia = desistencia.dt_desistencia,
                                  dc_tipo = desistencia.id_tipo_desistencia == (byte)tipoDesistencia.CANCELAMENTO ? "Cancelamento" :
                                            desistencia.id_tipo_desistencia == (byte)tipoDesistencia.DESISTENCIA ? "Desistência" :
                                            desistencia.id_tipo_desistencia == (byte)tipoDesistencia.NAOREMATRICULA ? "Não Rematrículado" : "Cancelamento Não Rematrícula",
                                  id_tipo_desistencia = desistencia.id_tipo_desistencia,
                                  cd_aluno_turma = desistencia.cd_aluno_turma,
                                  tx_obs_desistencia = desistencia.tx_obs_desistencia,
                                  cd_contrato = desistencia.AlunoTurma.cd_contrato,
                                  cd_pessoa_responsavel_titulo = desistencia.AlunoTurma.Contrato.cd_pessoa_responsavel,
                                  id_cancelamento = desistencia.MotivoDesistencia.id_cancelamento,
                                  id_aluno_ativo = desistencia.AlunoTurma.Aluno.id_aluno_ativo,
                                  telefone = desistencia.AlunoTurma.Aluno.AlunoPessoaFisica.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.TELEFONE).Any() ? desistencia.AlunoTurma.Aluno.AlunoPessoaFisica.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.TELEFONE).FirstOrDefault().dc_fone_mail : desistencia.AlunoTurma.Aluno.AlunoPessoaFisica.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.CELULAR).FirstOrDefault().dc_fone_mail
                              }).FirstOrDefault();

                return sql;

            }
            catch (Exception ex)
            {

                throw new DataAccessException(ex);
            }
        }


    }
}


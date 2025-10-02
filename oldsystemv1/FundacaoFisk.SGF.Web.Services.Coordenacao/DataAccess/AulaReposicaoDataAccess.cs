using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using Componentes.GenericDataAccess;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Services.Coordenacao.Comum.IDataAccess;
using FundacaoFisk.SGF.Services.Coordenacao.Model;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;
using DALC4NET;
using System.Data;

using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Reflection;
using System.Web.UI.WebControls;
using FundacaoFisk.SGF.Services.Coordenacao.Business;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess
{
    public class AulaReposicaoDataAccess : GenericRepository<AulaReposicao>, IAulaReposicaoDataAccess
    {
        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }

        public IEnumerable<AulaReposicaoUI> searchAulaReposicao(SearchParameters parametros, DateTime? dataIni, DateTime? dataFim, TimeSpan? hrInicial, TimeSpan? hrFinal, int? cd_turma, int? cd_aluno, int? cd_responsavel, int? cd_sala, int cdEscola)
        {
            try
            {
                var minDate = DateTime.MinValue;
                var maxDate = DateTime.MaxValue;

                IEntitySorter<AulaReposicaoUI> sorter = EntitySorter<AulaReposicaoUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<AulaReposicaoUI> sql;
                sql = from a in db.AulaReposicao.AsNoTracking()
                      where (a.dt_aula_reposicao >= dataIni)
                         && (a.dt_aula_reposicao <= dataFim)
                         && (hrInicial == null || a.dh_inicial_evento >= hrInicial)
                         && (hrFinal == null || a.dh_final_evento <= hrFinal)
                         && (cd_turma == null || a.cd_turma == cd_turma)
                         && (cd_sala == null || a.cd_sala == cd_sala)
                         && (cd_responsavel == null || a.cd_professor == cd_responsavel)
                         && (cd_aluno == null || a.AlunoAulaReposicao.Where(x => x.cd_aluno == cd_aluno).Any())
                         && a.cd_pessoa_escola == cdEscola
                      select new AulaReposicaoUI
                      {
                          cd_aula_reposicao = a.cd_aula_reposicao,
                          cd_pessoa_escola = a.cd_pessoa_escola,
                          cd_atendente = a.cd_atendente,
                          cd_professor = a.cd_professor,
                          dt_aula_reposicao = a.dt_aula_reposicao,
                          dh_inicial_evento = a.dh_inicial_evento,
                          dh_final_evento = a.dh_final_evento,
                          id_carga_horaria = a.id_carga_horaria,
                          id_pagar_professor = a.id_pagar_professor,
                          cd_turma = a.cd_turma,
                          cd_turma_destino = a.cd_turma_destino,
                          no_turma_destino = a.TurmaDestino != null? a.TurmaDestino.no_turma: null,
                          cd_sala = a.cd_sala,
                          no_turma = a.Turma.no_turma,
                          no_sala = a.Sala.no_sala,
                          tx_observacao_aula = a.tx_observacao_aula,
                          no_responsavel = a.Professor.FuncionarioPessoaFisica.no_pessoa,
                          nm_alunos = a.AlunoAulaReposicao.Count(),
                          AlunoAulaReposicao = a.AlunoAulaReposicao,
                          cd_produto = (a.cd_turma_destino != null ) ? a.Turma.cd_produto: 0,
                          cd_curso = (a.cd_turma_destino != null && a.Turma.Curso != null && !(a.Turma.cd_turma_ppt == null && a.Turma.id_turma_ppt == true)) ? a.Turma.cd_curso: null,
                          cd_estagio = (a.cd_turma_destino != null && a.Turma.Curso != null && !(a.Turma.cd_turma_ppt == null && a.Turma.id_turma_ppt == true)) ? a.Turma.Curso.cd_estagio : (int?)null

                      };

                sql = sorter.Sort(sql);
                var retorno = from b in sql
                              select b;

                int limite = retorno.Count();

                parametros.ajustaParametrosPesquisa(limite);
                retorno = retorno.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;
                return retorno.ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public AulaReposicaoUI returnAulaReposicaoUsuarioAtendente(int cd_aula_reposicao, int cd_pessoa_escola)
        {

            AulaReposicaoUI sql;
            try
            {
                sql = (from aulaReposicao in db.AulaReposicao
                    where aulaReposicao.cd_aula_reposicao == cd_aula_reposicao
                          && aulaReposicao.cd_pessoa_escola == cd_pessoa_escola
                    select new AulaReposicaoUI
                    {
                        cd_atendente = aulaReposicao.UsuarioWebSGF.cd_usuario,
                        no_usuario = aulaReposicao.UsuarioWebSGF.no_login
                    }).FirstOrDefault();

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public AulaReposicao findByIdAulaReposicaoViewFull(int cdAulaReposicao)
        {
            try
            {
                return (from a in db.AulaReposicao.Include("Professor")
                        where a.cd_aula_reposicao == cdAulaReposicao
                        select new
                        {
                            a.dh_final_evento,
                            a.dh_inicial_evento,
                            a.dt_aula_reposicao,
                            a.tx_observacao_aula,
                            a.cd_atendente,
                            a.Professor.FuncionarioPessoaFisica.no_pessoa,
                        }).ToList().Select(x => new AulaReposicao
                        {
                            dh_final_evento = x.dh_final_evento,
                            dh_inicial_evento = x.dh_inicial_evento,
                            dt_aula_reposicao = x.dt_aula_reposicao,
                            tx_observacao_aula= x.tx_observacao_aula,
                            cd_atendente = x.cd_atendente,
                            Professor = new Professor()
                            {
                                FuncionarioPessoaFisica = new PessoaFisicaSGF
                                {
                                    no_pessoa = x.no_pessoa
                                }
                            },
                        }).FirstOrDefault();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public AulaReposicao findByIdAulaReposicaoFull(int cdAulaReposicao)
        {
            try
            {
                return (from a in db.AulaReposicao
                        where a.cd_aula_reposicao == cdAulaReposicao
                        select new
                        {
                            a.dh_final_evento,
                            a.dh_inicial_evento,
                            a.dt_aula_reposicao,
                            a.tx_observacao_aula,
                            a.cd_atendente,
                            a.cd_sala
                            //a.Professor.no_professor
                        }).ToList().Select(x => new AulaReposicao
                        {
                            dh_final_evento = x.dh_final_evento,
                            dh_inicial_evento = x.dh_inicial_evento,
                            dt_aula_reposicao = x.dt_aula_reposicao,
                            tx_observacao_aula = x.tx_observacao_aula,
                            cd_atendente = x.cd_atendente,
                            cd_sala = x.cd_sala
                            //Professor = x.no_professor
                        }).FirstOrDefault();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public List<sp_RptAulaReposicao_Result> getReportAulaReposicao(Nullable<int> cd_escola, Nullable<System.DateTime> dta_ini, Nullable<System.DateTime> dta_fim, Nullable<int> cd_turma, Nullable<int> cd_funcionario, Nullable<int> cd_aluno, Nullable<byte> id_participacao)
        {
            try
            {

                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionStringReportProcedure"].ConnectionString;
                string providerName = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionStringReportProcedure"].ProviderName;
                DataTable dtReportData = new DataTable();

                DBHelper dbSql = new DBHelper(connectionString, providerName);

                DBParameter param1 = new DBParameter("@cd_escola", cd_escola, DbType.Int32);
                DBParameter param2 = new DBParameter("@dta_ini", dta_ini, DbType.DateTime);
                DBParameter param3 = new DBParameter("@dta_fim", dta_fim, DbType.DateTime);
                DBParameter param4 = new DBParameter("@cd_turma", cd_turma, DbType.Int32);
                DBParameter param5 = new DBParameter("@cd_funcionario", cd_funcionario, DbType.Int32);
                DBParameter param6 = new DBParameter("@cd_aluno", cd_aluno, DbType.Int32);
                DBParameter param7 = new DBParameter("@id_participacao", id_participacao, DbType.Int16);

                DBParameterCollection paramCollection = new DBParameterCollection();
                paramCollection.Add(param1);
                paramCollection.Add(param2);
                paramCollection.Add(param3);
                paramCollection.Add(param4);
                paramCollection.Add(param5);
                paramCollection.Add(param6);
                paramCollection.Add(param7);

                dtReportData = dbSql.ExecuteDataTable
                    ("sp_RptAulaReposicao", paramCollection, CommandType.StoredProcedure);

                var retorno = (List<sp_RptAulaReposicao_Result>)ConvertTo<sp_RptAulaReposicao_Result>(dtReportData);

                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public List<sp_RptAulaReposicaoAluno_Result> getReportAulaReposicaoAluno(Nullable<int> cd_aula_reposicao, Nullable<int> cd_aluno, Nullable<byte> id_participou)
        {
            try
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionStringReportProcedure"].ConnectionString;
                string providerName = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionStringReportProcedure"].ProviderName;


                DBHelper dbSql = new DBHelper(connectionString, providerName);
                DataTable dt = new DataTable();
                DBHelper dbAluno = new DBHelper(connectionString, providerName);
                DBParameter param1 = new DBParameter("@cd_aula_reposicao", cd_aula_reposicao, DbType.Int32);
                DBParameter param2 = new DBParameter("@cd_aluno", cd_aluno, DbType.Int32);
                DBParameter param3 = new DBParameter("@id_participou", id_participou, DbType.Int32);
                DBParameterCollection paramCollection = new DBParameterCollection();
                paramCollection.Add(param1);
                paramCollection.Add(param2);
                paramCollection.Add(param3);
                dt = dbAluno.ExecuteDataTable("sp_RptAulaReposicaoAluno", paramCollection, CommandType.StoredProcedure);

                var retorno = (List<sp_RptAulaReposicaoAluno_Result>)ConvertTo<sp_RptAulaReposicaoAluno_Result>(dt);
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public List<TimeSpan?> getHorariosDisponiveisAulaRep(DateTime data, int turma, int professor, int? cdAulaReposicao, List<AlunoAulaReposicaoUI> alunos)
        {
            try
            {
                String strAlunos = "";
                for (int i = 0; i < alunos.Count; i++)
                    strAlunos += alunos[i].cd_aluno + "|";
            
                var sql = db.sp_horario_aula_reposicao(data, professor, turma, cdAulaReposicao, strAlunos);
                var horas = sql.ToList();
                return horas;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public int? verificaHorarioAulaRep(TimeSpan horaIni, TimeSpan horaFim, DateTime data, int? cd_aula_reposicao, int cd_turma, int cd_professor, int cd_empresa, List<AlunoAulaReposicaoUI> alunos)
        {
            try
            {
                String strAlunos = "";
                for (int i = 0; i < alunos.Count; i++)
                    strAlunos += alunos[i].cd_aluno + "|";

                ObjectResult<Nullable<int>> sql = db.sp_verifica_aula_reposicao(horaIni, horaFim, data, cd_professor, cd_turma, cd_aula_reposicao, cd_empresa, strAlunos);
                List<int?> list = sql.ToList();
                int? I = list.FirstOrDefault();
                return I;
            }
            catch (Exception exe)
            {
                throw new AulaReposicaoBusinessException((exe.InnerException != null ? exe.InnerException.Message : "Erro Procedure."), exe, AulaReposicaoBusinessException.TipoErro.ERRO_PROCEDURE_SP_VERIFICA_AULA_REPOSICAO, false);
            }
        }

        private IList<T> ConvertTo<T>(DataTable table)
        {
            if (table == null)
            {
                return null;
            }

            List<DataRow> rows = new List<DataRow>();

            foreach (DataRow row in table.Rows)
            {
                rows.Add(row);
            }

            return ConvertTo<T>(rows);
        }

        private IList<T> ConvertTo<T>(IList<DataRow> rows)
        {
            IList<T> list = null;

            if (rows != null)
            {
                list = new List<T>();

                foreach (DataRow row in rows)
                {
                    T item = CreateItem<T>(row);
                    list.Add(item);
                }
            }

            return list;
        }

        private T CreateItem<T>(DataRow row)
        {
            T obj = default(T);
            if (row != null)
            {
                obj = Activator.CreateInstance<T>();

                foreach (DataColumn column in row.Table.Columns)
                {
                    PropertyInfo prop = obj.GetType().GetProperty(column.ColumnName);
                    try
                    {
                        object value = row[column.ColumnName];
                        if (value != DBNull.Value)
                        {
                            //object value = row[column.ColumnName];
                            prop.SetValue(obj, value, null);
                        }
                    }
                    catch
                    {
                        // You can log something here
                        throw;
                    }
                }
            }

            return obj;
        }
    }
}
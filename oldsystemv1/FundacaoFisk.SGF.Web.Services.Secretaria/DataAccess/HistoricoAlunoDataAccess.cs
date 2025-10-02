using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using DALC4NET;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Secretaria.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess
{

    public class HistoricoAlunoDataAccess : GenericRepository<HistoricoAluno>, IHistoricoAlunoDataAccess
    {
       

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public HistoricoAluno GetHistoricoAlunoById(int cdEscola, int cdAluno, int cdTurma, int cdContrato)
        {
            try
            {
                HistoricoAluno sql = (from c in db.HistoricoAluno
                                      where
                                         c.cd_aluno == cdAluno &&
                                         c.Aluno.cd_pessoa_escola == cdEscola &&
                                         c.cd_turma == cdTurma &&
                                         c.cd_contrato == cdContrato
                                      select c).FirstOrDefault();

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public HistoricoAluno getHistoricoAlunoByMatricula(int cdEscola, int cdAluno, int cdTurma, int cdContrato)
        {
            try
            {
                HistoricoAluno sql = (from c in db.HistoricoAluno
                                      where
                                         c.cd_aluno == cdAluno &&
                                         c.Aluno.cd_pessoa_escola == cdEscola &&
                                         c.cd_turma == cdTurma &&
                                         c.cd_contrato == cdContrato &&
                                         (c.id_situacao_historico == (byte)HistoricoAluno.SituacaoHistorico.ATIVO ||
                                         c.id_situacao_historico == (byte)HistoricoAluno.SituacaoHistorico.REMATRICULADO)
                                      select c).FirstOrDefault();

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<HistoricoAluno> GetHistoricosAlunoById(int cdEscola, int cdAluno, int cdTurma, int cdContrato)
        {
            try
            {
                IEnumerable<HistoricoAluno> sql = from c in db.HistoricoAluno
                                                  where
                                                     c.cd_aluno == cdAluno &&
                                                     c.Aluno.cd_pessoa_escola == cdEscola &&
                                                     c.cd_turma == cdTurma &&
                                                     c.cd_contrato == cdContrato
                                                  select c;

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        
        public int GetUltimoHistoricoAluno(int cdEscola, int cdAluno, int cdProduto)
        {
            try
            {
                var sql = (from c in db.HistoricoAluno
                                      where
                                         c.cd_aluno == cdAluno &&
                                         c.Aluno.cd_pessoa_escola == cdEscola &&
                                         c.cd_produto == cdProduto
                                      orderby c.nm_sequencia descending
                                      select c.nm_sequencia).FirstOrDefault();

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<HistoricoAluno> returnHistoricoSitacaoAlunoTurma(int cd_turma, int cd_pessoa_escola)
        {
            //0	Movido //1 Ativo //2	Desistente //3	Transferido //4 Encerrado //5 Dependente //6	Reprovado //7	Remanejado //8	Rematriculado //9	Aguardando
            try
            {
                var sql = (from historico in db.HistoricoAluno
                           where historico.cd_turma == cd_turma
                            //LBM&& historico.Turma.cd_pessoa_escola == cd_pessoa_escola
                           // && DbFunctions.TruncateTime(historico.dt_cadastro) <= dtAvaliacao.Date
                           select new
                           {
                               cd_aluno = historico.cd_aluno,
                               id_situacao_historico = historico.id_situacao_historico,
                               dt_cadastro = historico.dt_cadastro,
                               dt_historico = historico.dt_historico,
                               nm_sequencia = historico.nm_sequencia
                           }).ToList().Select(x => new HistoricoAluno
                           {
                               cd_aluno = x.cd_aluno,
                               id_situacao_historico = x.id_situacao_historico,
                               dt_cadastro = x.dt_cadastro,
                               dt_historico = x.dt_historico,
                               nm_sequencia = x.nm_sequencia
                           });

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public int retunMaxSequenciaHistoricoAluno(int cd_produto, int cd_pessoa_escola, int cd_aluno)
        {
            try
            {
                int? sql;
                sql = (from historico in db.HistoricoAluno
                       where historico.cd_produto == cd_produto
                        && historico.Aluno.cd_pessoa_escola == cd_pessoa_escola
                        && historico.cd_aluno == cd_aluno
                       orderby historico.nm_sequencia descending
                       select historico.nm_sequencia).FirstOrDefault();

                return sql ?? 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public HistoricoAluno GetHistoricoAlunoPrimeiraAula(int cdEscola, int cdAluno, int cdTurma, int cdContrato, DateTime dataAula)
        {
            try
            {
                HistoricoAluno sql = (from historico in db.HistoricoAluno
                                      where
                                         historico.cd_aluno == cdAluno 
                                         && historico.Aluno.cd_pessoa_escola == cdEscola 
                                         && historico.cd_turma == cdTurma 
                                         && historico.cd_contrato == cdContrato
                                         && historico.id_tipo_movimento == (byte)HistoricoAluno.TipoMovimento.PRIMEIRA_AULA
                                         &&  DbFunctions.TruncateTime(historico.dt_historico) == dataAula
                                      select historico).FirstOrDefault();

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
       
        public HistoricoAluno GetHistoricoAlunoMovido(int cdEscola, int cdAluno, int cdTurma, int cdContrato){
            try
            {
                HistoricoAluno sql = (from historico in db.HistoricoAluno
                                      where
                                         historico.cd_aluno == cdAluno
                                         && historico.Aluno.cd_pessoa_escola == cdEscola
                                         && historico.cd_turma == cdTurma
                                         && historico.cd_contrato == cdContrato
                                         && historico.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Movido
                                      select historico).FirstOrDefault();

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }


        public IEnumerable<HistoricoAluno> getHistoricoTurmas(int cd_aluno, int cd_escola) {
            try {
                return (from h in db.HistoricoAluno
                        where h.cd_aluno == cd_aluno
                            //&& h.Turma.cd_pessoa_escola == cd_escola
                        orderby h.nm_sequencia
                        select new {
                            cd_produto = h.Produto.cd_produto,
                            no_produto = h.Produto.no_produto,
                            cd_turma = h.Turma.cd_turma,
                            no_turma = h.Turma.no_turma,
                            nm_sequencia = h.nm_sequencia,
                            dt_historico = h.dt_historico,
                            nm_contrato = h.Contrato.nm_contrato,
                            id_situacao_historico = h.id_situacao_historico,
                            id_tipo_movimento = h.id_tipo_movimento,
                            dt_cadastro = h.dt_cadastro,
                            h.SysUsuario.no_login
                        }).ToList().Select(x => new HistoricoAluno {
                            Produto = new Produto { cd_produto = x.cd_produto, no_produto = x.no_produto },
                            Turma = new Turma { no_turma = x.no_turma, cd_turma = x.cd_turma },
                            nm_sequencia = x.nm_sequencia,
                            dt_historico = x.dt_historico,
                            Contrato = new Contrato { nm_contrato = x.nm_contrato },
                            id_situacao_historico = x.id_situacao_historico,
                            id_tipo_movimento = x.id_tipo_movimento,
                            dt_cadastro = x.dt_cadastro,
                            SysUsuario = new UsuarioWebSGF { no_login = x.no_login }
                        });
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

        public HistoricoAluno getUltimoHistoricoAlunoPorCodTurma(int cd_aluno, int cd_escola, int cd_turma)
        {
            try
            {
                var historicoData = (from h in db.HistoricoAluno
                                     where h.cd_aluno == cd_aluno
                                         && h.Turma.cd_pessoa_escola == cd_escola
                                         && h.Turma.cd_turma == cd_turma
                                     orderby h.nm_sequencia descending
                                     select new
                                     {
                                         cd_turma = h.Turma.cd_turma,
                                         no_turma = h.Turma.no_turma,
                                         nm_sequencia = h.nm_sequencia,
                                         dt_historico = h.dt_historico,
                                     }).FirstOrDefault();

                if (historicoData != null)
                {
                    return new HistoricoAluno
                    {
                        Turma = new Turma { no_turma = historicoData.no_turma, cd_turma = historicoData.cd_turma },
                        nm_sequencia = historicoData.nm_sequencia,
                        dt_historico = historicoData.dt_historico
                    };
                }
                return null;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public HistoricoAluno GetHistoricoAlunoPorDesistencia(int cdDesistencia)
        {
            try
            {
                HistoricoAluno sql = (from historico in db.HistoricoAluno
                                      where
                                         historico.cd_desistencia == cdDesistencia
                                      select historico).FirstOrDefault();

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public HistoricoAluno getSituacaoAlunoCancelEncerramento(int cd_aluno, int cd_turma, DateTime dt_historico)
        {
            try
            {
                int nm_seq = (from historico in db.HistoricoAluno
                                      where
                                         historico.cd_aluno == cd_aluno &&
                                         historico.cd_turma == cd_turma &&
                                         DbFunctions.TruncateTime(historico.dt_historico) == DbFunctions.TruncateTime(dt_historico) &&
                                         historico.id_tipo_movimento == (int)HistoricoAluno.TipoMovimento.ENCERRADO &&
                                         historico.id_situacao_historico == (int)HistoricoAluno.SituacaoHistorico.ENCERRADO
                                        orderby historico.nm_sequencia descending
                                      select historico.nm_sequencia).FirstOrDefault();
                HistoricoAluno historicoAluno = new HistoricoAluno();
                if(nm_seq > 0){
                    historicoAluno = (from h in db.HistoricoAluno
                                         where
                                            h.cd_aluno == cd_aluno &&
                                            h.cd_turma == cd_turma &&
                                            h.nm_sequencia < nm_seq
                                      orderby h.nm_sequencia descending
                                      select new
                                      {
                                          cd_historico_aluno = h.cd_historico_aluno,
                                          cd_contrato = h.cd_contrato,
                                          id_situacao_historico = h.id_situacao_historico
                                      }).ToList().Select(x => new HistoricoAluno
                                      {
                                          cd_historico_aluno = x.cd_historico_aluno,
                                          cd_contrato = x.cd_contrato,
                                          id_situacao_historico = x.id_situacao_historico
                                      }).FirstOrDefault();
                }
                return historicoAluno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public DateTime? buscarDataHistoricoDesistenciaAlteriorCancelamento(int cd_aluno, int cd_turma, DateTime dt_historico, byte nm_sequencia)
        {
            try
            {
                var dataHistorico = (from historico in db.HistoricoAluno
                                       where
                                         historico.cd_aluno == cd_aluno &&
                                         historico.cd_turma == cd_turma &&
                                          DbFunctions.TruncateTime(historico.dt_historico) <= DbFunctions.TruncateTime(dt_historico) &&
                                          historico.nm_sequencia < nm_sequencia &&
                                          historico.id_situacao_historico == (byte)HistoricoAluno.SituacaoHistorico.DESISTENTE
                                      select historico.dt_historico).FirstOrDefault();

                return dataHistorico;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public DataTable getAlunos(int cd_aluno, int Tipo, string produtos, string statustitulo)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionStringReportProcedure"].ConnectionString;
            string providerName = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionStringReportProcedure"].ProviderName;

            DataTable dt = new DataTable();
            DBHelper dbAluno = new DBHelper(connectionString, providerName);
            DBParameter param1 = new DBParameter("@cd_aluno", cd_aluno, DbType.Int32);
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(param1);
            if (Tipo == 1 || Tipo == 2 || Tipo == 3 || Tipo == 4 || Tipo == 5 || Tipo == 6 || Tipo == 7 || Tipo == 13 || Tipo == 14)
            {
                paramCollection.Add(new DBParameter("@tipo", produtos, DbType.String));
            }
            if (Tipo == 8)
            {
                paramCollection.Add(new DBParameter("@tipo", statustitulo, DbType.String));
            }
            switch (Tipo)
            {
                case 1:
                    dt = dbAluno.ExecuteDataTable("sp_RptHistoricoAluno", paramCollection, CommandType.StoredProcedure);
                    break;
                case 2:
                    dt = dbAluno.ExecuteDataTable("sp_RptTurmasAvaliacoes", paramCollection, CommandType.StoredProcedure);
                    break;
                case 3:
                    dt = dbAluno.ExecuteDataTable("sp_RptAvaliacaoTurma", paramCollection, CommandType.StoredProcedure);
                    break;
                case 4:
                    dt = dbAluno.ExecuteDataTable("sp_RptHistoricoEstagio", paramCollection, CommandType.StoredProcedure);
                    break;
                case 5:
                    dt = dbAluno.ExecuteDataTable("sp_RptAvaliacaoEstagio", paramCollection, CommandType.StoredProcedure);
                    break;
                case 6:
                    dt = dbAluno.ExecuteDataTable("sp_RptEventoAluno", paramCollection, CommandType.StoredProcedure);

                    break;
                case 7:
                    dt = dbAluno.ExecuteDataTable("sp_RptDiarioAluno", paramCollection, CommandType.StoredProcedure);
                    break;
                case 8:
                    dt = dbAluno.ExecuteDataTable("sp_RptHistoricoTitulos", paramCollection, CommandType.StoredProcedure);
                    break;
                case 9:
                    dt = dbAluno.ExecuteDataTable("sp_RptHistoricoObs", paramCollection, CommandType.StoredProcedure);
                   
                    break;
                case 10:
                    dt = dbAluno.ExecuteDataTable("sp_RptHistoricoAtividade", paramCollection, CommandType.StoredProcedure);
                    
                    break;
                case 11:
                    dt = dbAluno.ExecuteDataTable("sp_RptHistoricoFollow", paramCollection, CommandType.StoredProcedure);
                    
                    break;
                case 12:
                    dt = dbAluno.ExecuteDataTable("sp_RptHistoricoItem", paramCollection, CommandType.StoredProcedure);
                    
                    break;
                case 13:
                    dt = dbAluno.ExecuteDataTable("sp_RptHistoricoConceito", paramCollection, CommandType.StoredProcedure);
                    
                    break;
                case 14:
                    dt = dbAluno.ExecuteDataTable("sp_RptHistoricoConceito", paramCollection, CommandType.StoredProcedure);
                   
                    break;
            };
            return dt;
        }

        public List<sp_RptHistoricoAlunoM_Result> getRtpHistoricoAlunoM(int cdAluno)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionStringReportProcedure"].ConnectionString;
            string providerName = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionStringReportProcedure"].ProviderName;


            DataTable dtReportData = new DataTable();

            DBHelper dbSql = new DBHelper(connectionString, providerName);

            DBParameter param1 = new DBParameter("@cd_aluno", cdAluno, DbType.Int32);

            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(param1);

            dtReportData = dbSql.ExecuteDataTable("sp_RptHistoricoAlunoM", paramCollection, CommandType.StoredProcedure);
            List<sp_RptHistoricoAlunoM_Result> rtp = (List<sp_RptHistoricoAlunoM_Result>)ConvertTo<sp_RptHistoricoAlunoM_Result>(dtReportData);
            return rtp;
        }

        public List<st_RptFaixaEtaria_Result> getRtpFaixaEtaria(int cd_escola, int tipo, int idade, int idade_max, int cd_turma)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionStringReportProcedure"].ConnectionString;
            string providerName = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionStringReportProcedure"].ProviderName;


            DataTable dtReportData = new DataTable();

            DBHelper dbSql = new DBHelper(connectionString, providerName);

            DBParameter param1 = new DBParameter("@cd_escola", cd_escola, DbType.Int32);
            DBParameter param2 = new DBParameter("@tipo", tipo, DbType.Int32);
            DBParameter param3 = new DBParameter("@idade_min", idade, DbType.Int32);
            DBParameter param4 = new DBParameter("@idade_max", idade_max, DbType.Int32);
            DBParameter param5 = new DBParameter("@cd_turma", cd_turma, DbType.Int32);

            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(param1);
            paramCollection.Add(param2);
            paramCollection.Add(param3);
            paramCollection.Add(param4);
            paramCollection.Add(param5);

            dtReportData = dbSql.ExecuteDataTable("st_RptFaixaEtaria", paramCollection, CommandType.StoredProcedure);
            List<st_RptFaixaEtaria_Result> rtp = (List<st_RptFaixaEtaria_Result>)ConvertTo<st_RptFaixaEtaria_Result>(dtReportData);
            return rtp;
        }


        public DataTable getRtpFaixaEtariaDT(int cd_escola, int tipo, int idade, int idade_max, int cd_turma)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionStringReportProcedure"].ConnectionString;
            string providerName = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionStringReportProcedure"].ProviderName;


            DataTable dtReportData = new DataTable();

            DBHelper dbSql = new DBHelper(connectionString, providerName);

            DBParameter param1 = new DBParameter("@cd_escola", cd_escola, DbType.Int32);
            DBParameter param2 = new DBParameter("@tipo", tipo, DbType.Int32);
            DBParameter param3 = new DBParameter("@idade_min", idade, DbType.Int32);
            DBParameter param4 = new DBParameter("@idade_max", idade_max, DbType.Int32);
            DBParameter param5 = new DBParameter("@cd_turma", cd_turma, DbType.Int32);

            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(param1);
            paramCollection.Add(param2);
            paramCollection.Add(param3);
            paramCollection.Add(param4);
            paramCollection.Add(param5);

            dtReportData = dbSql.ExecuteDataTable("st_RptFaixaEtaria", paramCollection, CommandType.StoredProcedure);
            return dtReportData;
        }

        public List<ProdutoHistoricoSeachUI> getProdutosComHistorico(int cdEscola)
        {
            try
            {
                var sql = (from historico in db.HistoricoAluno
                    join produto in db.Produto on historico.cd_produto equals produto.cd_produto
                    join turma in db.Turma on historico.cd_turma equals turma.cd_turma
                    where historico.Turma.cd_pessoa_escola == cdEscola && (historico.id_situacao_historico ==  (byte)HistoricoAluno.SituacaoHistorico.ATIVO || historico.id_situacao_historico == (byte)HistoricoAluno.SituacaoHistorico.REMATRICULADO)
                    select new ProdutoHistoricoSeachUI
                    {
                        cd_produto = produto.cd_produto,
                        no_produto = produto.no_produto

                    }).Distinct().ToList();

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }


        public  IList<T> ConvertTo<T>(DataTable table)
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

            return ConvertToData<T>(rows);
        }

        public  IList<T> ConvertToData<T>(IList<DataRow> rows)
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

        public T CreateItem<T>(DataRow row)
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

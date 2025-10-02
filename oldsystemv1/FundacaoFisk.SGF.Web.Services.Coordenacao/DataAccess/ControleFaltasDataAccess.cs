using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;
using Componentes.Utils;
using System.Data.SqlClient;
using Componentes.GenericDataAccess.GenericException;
using System.Data;
using Componentes.GenericModel;
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;
using System.Data.Entity.Core.Objects;
using System.Net.Sockets;
using System.Reflection;
using DALC4NET;
using FundacaoFisk.SGF.Services.Coordenacao.Comum.IDataAccess;
using FundacaoFisk.SGF.Services.Coordenacao.Model;
using FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess;


namespace FundacaoFisk.SGF.Services.Coordenacao.DataAccess
{
    public class ControleFaltasDataAccess : GenericRepository<ControleFaltas>, IControleFaltasDataAccess
    {
        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }

        public enum AssinaturaControleFaltas
        {
            TODOS = 0,
            ASSINOU = 1,
            NAOASSINOU = 2
        }

        public IEnumerable<ControleFaltasUI> getControleFaltasSearch(Componentes.Utils.SearchParameters parametros, string desc, int cd_turma, int cd_aluno, AssinaturaControleFaltas assinatura, DateTime? dtInicial, DateTime? dtFinal, int cdEscola)
        {
            try
            {
                IEntitySorter<ControleFaltasUI> sorter = EntitySorter<ControleFaltasUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<ControleFaltasUI> sql;
                IQueryable<ControleFaltas> sql0;

                sql0 = from c in db.ControleFaltas
                       select c;

                if (cd_turma > 0)
                {
                    sql0 = from controleFaltas in sql0
                           where controleFaltas.cd_turma == cd_turma
                           select controleFaltas;
                }

                if (cd_aluno > 0)
                {
                    sql0 = from c in sql0
                           join ca in db.ControleFaltasAluno on c.cd_controle_faltas equals ca.cd_controle_faltas
                           join a in db.Aluno on ca.cd_aluno equals a.cd_aluno
                           where a.cd_aluno == cd_aluno
                           select c;
                }


                switch (assinatura)
                {
                    case (AssinaturaControleFaltas.TODOS):
                        sql0 = from c in sql0
                               select c;
                        break;
                    case (AssinaturaControleFaltas.ASSINOU):
                        sql0 = from c in sql0.Include(x => x.ControleFaltasAluno)
                               where c.ControleFaltasAluno.Where(f => f.id_assinatura == true).Any()
                               select c;
                        break;
                    case (AssinaturaControleFaltas.NAOASSINOU):
                        sql0 = from c in sql0.Include(x => x.ControleFaltasAluno)
                               where c.ControleFaltasAluno.Where(f => f.id_assinatura == false).Any()
                               select c;
                        break;

                }


                sql0 = from t in sql0
                       where t.Turma.cd_pessoa_escola == cdEscola
                       select t;

                sql = (from c in sql0
                       where c.dt_controle_faltas >= dtInicial
                             && c.dt_controle_faltas <= dtFinal
                       select new ControleFaltasUI
                       {
                           cd_controle_faltas = c.cd_controle_faltas,
                           cd_turma = c.cd_turma,
                           dh_controle_faltas = c.dh_controle_faltas,
                           dt_controle_faltas = c.dt_controle_faltas,
                           cd_usuario = c.cd_usuario,
                           no_turma = c.Turma.no_turma,
                           no_usuario = c.SysUsuario.no_login
                       });

                sql = from c in sql
                      select c;


                sql = sorter.Sort(sql);

                int limite = sql.Count();

                parametros.qtd_limite = limite;
                parametros.ajustaParametrosPesquisa(limite);
                sql = sql.Skip(parametros.from).Take(parametros.qtd_limite);

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public ControleFaltasUI getControleFaltasUIbyId(int cd_controle_faltas)
        {
            try
            {
                var sql = (from c in db.ControleFaltas
                           where c.cd_controle_faltas == cd_controle_faltas
                           select new
                           {
                               cd_controle_faltas = c.cd_controle_faltas,
                               cd_turma = c.cd_turma,
                               dh_controle_faltas = c.dh_controle_faltas,
                               dt_controle_faltas = c.dt_controle_faltas,
                               cd_usuario = c.cd_usuario,
                               no_turma = c.Turma.no_turma,
                               no_usuario = c.SysUsuario.no_login
                           }).ToList().Select(x => new ControleFaltasUI
                           {
                               cd_controle_faltas = x.cd_controle_faltas,
                               cd_turma = x.cd_turma,
                               dh_controle_faltas = x.dh_controle_faltas,
                               dt_controle_faltas = x.dt_controle_faltas,
                               cd_usuario = x.cd_usuario,
                               no_turma = x.no_turma,
                               no_usuario = x.no_usuario
                           }).FirstOrDefault();

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public ControleFaltas getControleFaltasEdit(int cd_controle_faltas)
        {
            try
            {
                ControleFaltas sql = (from c in db.ControleFaltas
                                      where c.cd_controle_faltas == cd_controle_faltas
                                      select new
                                      {
                                          cd_controle_faltas = c.cd_controle_faltas,
                                          cd_turma = c.cd_turma,
                                          dh_controle_faltas = c.dh_controle_faltas,
                                          dt_controle_faltas = c.dt_controle_faltas,
                                          cd_usuario = c.cd_usuario,
                                          no_turma = c.Turma.no_turma,
                                          no_usuario = c.SysUsuario.no_login,
                                          ControleFaltasAluno = c.ControleFaltasAluno
                                      }).ToList().Select(x => new ControleFaltas
                                      {
                                          cd_controle_faltas = x.cd_controle_faltas,
                                          cd_turma = x.cd_turma,
                                          dh_controle_faltas = x.dh_controle_faltas,
                                          dt_controle_faltas = x.dt_controle_faltas,
                                          cd_usuario = x.cd_usuario,
                                          no_turma = x.no_turma,
                                          no_usuario = x.no_usuario,
                                          ControleFaltasAluno = x.ControleFaltasAluno
                                      }).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }


        public List<sp_RptControleFaltas_Result> getReportControleFaltasResults(Nullable<int> cd_tipo, int cd_escola, Nullable<int> cd_curso, Nullable<int> cd_nivel,
            Nullable<int> cd_produto, Nullable<int> cd_professor, Nullable<int> cd_turma, Nullable<int> cd_sit_turma, string cd_sit_aluno, string dt_inicial,
            string dt_final, bool quebrarpagina)
        {
            try
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionStringReportProcedure"].ConnectionString;
                string providerName = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionStringReportProcedure"].ProviderName;



                DataTable dt = new DataTable();

                DBHelper dbSql = new DBHelper(connectionString, providerName);

                DBParameter param1 = new DBParameter("@cd_escola", cd_escola, DbType.Int32);
                DBParameter param2 = new DBParameter("@cd_turma", cd_turma, DbType.Int32);
                DBParameter param3 = new DBParameter("@cd_produto", cd_produto, DbType.Int32);
                DBParameter param4 = new DBParameter("@cd_curso", cd_curso, DbType.Int32);
                DBParameter param5 = new DBParameter("@tipo", cd_tipo, DbType.Int32);
                DBParameter param6 = new DBParameter("@cd_professor", cd_professor, DbType.Int32);
                DBParameter param7 = new DBParameter("@sit_turma", cd_sit_turma, DbType.Int32);
                DBParameter param8 = new DBParameter("@cd_nivel", cd_nivel, DbType.Int32);
                DBParameter param9 = new DBParameter("@dt_inicio", dt_inicial);
                DBParameter param10 = new DBParameter("@dt_fim", dt_final);
                DBParameter param11 = new DBParameter("@id_situacao", cd_sit_aluno);

                DBParameterCollection paramCollection = new DBParameterCollection();
                paramCollection.Add(param1);
                paramCollection.Add(param2);
                paramCollection.Add(param3);
                paramCollection.Add(param4);
                paramCollection.Add(param5);
                paramCollection.Add(param6);
                paramCollection.Add(param7);
                paramCollection.Add(param8);
                paramCollection.Add(param9);
                paramCollection.Add(param10);
                paramCollection.Add(param11);

                dt = dbSql.ExecuteDataTable("sp_RptControleFaltas", paramCollection, CommandType.StoredProcedure);


                /*
                DBHelper dbSql = new DBHelper(connectionString, providerName);
                DataTable dt = new DataTable();
                DBHelper dbAluno = new DBHelper(connectionString, providerName);
                DBParameter param1 = new DBParameter("@cd_atividade_extra", cd_atividade_extra, DbType.Int32);
                DBParameter param2 = new DBParameter("@cd_aluno", cd_aluno, DbType.Int32);
                DBParameter param3 = new DBParameter("@id_participou", id_participou, DbType.Int32);
                DBParameterCollection paramCollection = new DBParameterCollection();
                paramCollection.Add(param1);
                paramCollection.Add(param2);
                paramCollection.Add(param3);
                dt = dbAluno.ExecuteDataTable("sp_RptAtividadeExtraAluno", paramCollection, CommandType.StoredProcedure);
                */
                var retorno = (List<sp_RptControleFaltas_Result>)ConvertTo<sp_RptControleFaltas_Result>(dt);
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
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
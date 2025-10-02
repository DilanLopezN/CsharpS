using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data;
using DALC4NET;
using System.Reflection;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess
{
    public class AtividadeExtraDataAccess : GenericRepository<AtividadeExtra>, IAtividadeExtraDataAccess
    {
        public enum TipoConsultaAtividadeExtra
        {
            SALA = 1,
            CURSO = 2
        }

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }

        public IEnumerable<AtividadeExtraUI> searchAtividadeExtra(SearchParameters parametros, DateTime? dataIni, DateTime? dataFim, TimeSpan? hrInicial, TimeSpan? hrFinal, int? tipoAtividade, int? curso,  
            int? responsavel, int? produto, int? aluno, byte lancada, int cdEscola, int cd_escola_combo)
        {
            try{
                var minDate = DateTime.MinValue;
                var maxDate = DateTime.MaxValue;

                IEntitySorter<AtividadeExtraUI> sorter = EntitySorter<AtividadeExtraUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<AtividadeExtraUI> sql;

               
                var sqla = (from atividadeExtra in db.AtividadeExtra.AsNoTracking()
                      where (atividadeExtra.dt_atividade_extra >= dataIni)
                            && (atividadeExtra.dt_atividade_extra <= dataFim)
                            && (atividadeExtra.cd_pessoa_escola == cdEscola ||
                                (from ae in db.AtividadeEscolaAtividade
                                    where ae.cd_atividade_extra == atividadeExtra.cd_atividade_extra &&
                                          ae.cd_escola == cdEscola
                                    select ae).Any())
	                        select atividadeExtra).Union(
                                    (from atividadeExtra in db.AtividadeExtra.AsNoTracking()
                                     where
                                         (atividadeExtra.dt_atividade_extra >= dataIni)
                                         && (atividadeExtra.dt_atividade_extra <= dataFim)
                                         && (atividadeExtra.cd_pessoa_escola == cdEscola) &&
                                         (from te in db.AtividadeEscolaAtividade
                                          where te.cd_atividade_extra == atividadeExtra.cd_atividade_extra &&
                                                te.cd_escola == cdEscola
                                          select te).Any()
                                     select atividadeExtra));

                if (hrInicial != null)
                    sqla = from atividadeExtra in sqla
	                        where atividadeExtra.hh_inicial >= hrInicial
	                        select atividadeExtra;
                if (hrFinal != null)
	                    sqla = from atividadeExtra in sqla
		                        where atividadeExtra.hh_final <= hrFinal
		                        select atividadeExtra;
                if (tipoAtividade != null)
	                    sqla = from atividadeExtra in sqla
		                        where atividadeExtra.cd_tipo_atividade_extra == tipoAtividade
		                        select atividadeExtra;
                if (curso != null)
	                    sqla = from atividadeExtra in sqla
		                        where atividadeExtra.AtividadeCurso.Where(c => c.cd_curso == curso).Any()
		                        select atividadeExtra;
                if (responsavel != null)
	                    sqla = from atividadeExtra in sqla
		                        where atividadeExtra.cd_funcionario == responsavel
		                        select atividadeExtra;
                if (produto != null)
	                    sqla = from atividadeExtra in sqla
		                        where atividadeExtra.cd_produto == produto
		                        select atividadeExtra;
                if (aluno != null)
	                    sqla = from atividadeExtra in sqla
		                        where atividadeExtra.AtividadeAluno.Where(a => a.cd_aluno == aluno).Any() || atividadeExtra.AtividadeProspect.Where(a => a.cd_prospect == aluno).Any()
		                        select atividadeExtra;
                if (lancada == 0)
	                    sqla = from atividadeExtra in sqla
		                        where (!atividadeExtra.AtividadeAluno.Any()
				                        || atividadeExtra.AtividadeAluno.Where(a => (a.id_participacao == null)).Any()) ||
		   		                        (!atividadeExtra.AtividadeProspect.Any()
				                        || atividadeExtra.AtividadeProspect.Where(a => (a.id_participacao == null)).Any())
		                        select atividadeExtra;
                if (lancada == 1)
	                    sqla = from atividadeExtra in sqla
		                        where    atividadeExtra.AtividadeAluno.Where(a => (a.id_participacao == a.ind_participacao)).Any() ||
					                    atividadeExtra.AtividadeProspect.Where(a => (a.id_participacao == a.ind_participacao)).Any()
		                        select atividadeExtra;

                           
                sql = from atividadeExtra in sqla
                select new AtividadeExtraUI
                      {
                          cd_atividade_extra = atividadeExtra.cd_atividade_extra,
                          cd_funcionario = atividadeExtra.cd_funcionario,
                          cd_produto = atividadeExtra.cd_produto,
                          cd_tipo_atividade_extra = atividadeExtra.cd_tipo_atividade_extra,
                          dt_atividade_extra = atividadeExtra.dt_atividade_extra,
                          hh_final = atividadeExtra.hh_final,
                          hh_inicial = atividadeExtra.hh_inicial,
                          ind_carga_horaria = atividadeExtra.ind_carga_horaria,
                          ind_pagar_professor = atividadeExtra.ind_pagar_professor,
                          nm_vagas = atividadeExtra.nm_vagas,
                          tx_obs_atividade = atividadeExtra.tx_obs_atividade,
                          no_tipo_atividade_extra = atividadeExtra.TipoAtividadeExtra.no_tipo_atividade_extra,
                          //no_curso = atividadeExtra.Curso.no_curso,
                          //cd_curso = atividadeExtra.cd_curso,
                          id_calendario_academico = atividadeExtra.id_calendario_academico,
                          hr_limite_academico = atividadeExtra.hr_limite_academico,
                          cd_cursos = (from c in atividadeExtra.AtividadeCurso
                                       select c.cd_curso).ToList(),
                          no_produto = atividadeExtra.Produto.no_produto,
                          no_responsavel = (from p in db.PessoaSGF join f in db.FuncionarioSGF on atividadeExtra.cd_funcionario equals f.cd_funcionario where p.cd_pessoa == f.cd_pessoa_funcionario select p.no_pessoa).FirstOrDefault(),
                    //nm_alunos = (atividadeExtra.AtividadeAluno.Count() + atividadeExtra.AtividadeProspect.Count()) ,
                    nm_alunos = (((from atividadeAluno in db.AtividadeAluno
                                        //join atividade in db.AtividadeExtra on atividadeAluno.cd_atividade_extra equals atividade.cd_atividade_extra
                                         join a1 in db.Aluno on atividadeAluno.cd_aluno equals a1.cd_aluno
                                        where atividadeAluno.cd_atividade_extra == atividadeExtra.cd_atividade_extra &&
                                              a1.cd_pessoa_escola == cdEscola
                                        select atividadeAluno.cd_atividade_aluno).Count()) +
                                         ((from atividadeProspect in db.AtividadeProspect
                                          //join atividade in db.AtividadeExtra on atividadeProspect.cd_atividade_extra equals atividade.cd_atividade_extra
                                          join a2 in db.Prospect on atividadeProspect.cd_prospect equals a2.cd_prospect
                                          where atividadeProspect.cd_atividade_extra == atividadeExtra.cd_atividade_extra &&
                                                a2.cd_pessoa_escola == cdEscola
                                          select atividadeProspect.cd_atividade_prospect).Count())),
                          atividadesAluno = (from atividadeAluno in db.AtividadeAluno
                              //join atividade in db.AtividadeExtra on atividadeAluno.cd_atividade_extra equals atividade.cd_atividade_extra
                              join a3 in db.Aluno on atividadeAluno.cd_aluno equals a3.cd_aluno
                              where atividadeAluno.cd_atividade_extra == atividadeExtra.cd_atividade_extra &&
                                    a3.cd_pessoa_escola == cdEscola
                              select atividadeAluno).ToList(),
                          cd_sala = atividadeExtra.cd_sala,
                          no_sala = atividadeExtra.AtividadeSala.no_sala,
                          cd_usuario_atendente = atividadeExtra.AtividadeSysUsuario.cd_usuario,
                          no_usuario = atividadeExtra.AtividadeSysUsuario.no_login,
                          cd_atividade_recorrencia = atividadeExtra.cd_atividade_recorrrencia,
                          AtividadeRecorrencia = (atividadeExtra.cd_atividade_recorrrencia != null) ?  
                            ((from a in db.AtividadeRecorrencia where a.cd_atividade_recorrencia == atividadeExtra.cd_atividade_recorrrencia select a).FirstOrDefault()) :
                            ((from a in db.AtividadeRecorrencia where a.cd_atividade_extra == atividadeExtra.cd_atividade_extra select a).Any()) ?
                            ((from a in db.AtividadeRecorrencia where a.cd_atividade_extra == atividadeExtra.cd_atividade_extra select a).FirstOrDefault()) : null,
                            id_email_enviado = atividadeExtra.id_email_enviado,
                          cd_pessoa_escola = atividadeExtra.cd_pessoa_escola
                      };
                

                sql = sorter.Sort(sql);
                

                int limite = sql.ToList().Count();

                parametros.ajustaParametrosPesquisa(limite);
                sql = sql.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;
                return sql.ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }


        public bool deleteAllAtividadeExtra(List<AtividadeExtra> atividadesExtras)
        {
            try{
                string stratividadesExtras = "";
                if (atividadesExtras != null && atividadesExtras.Count > 0)
                    foreach (AtividadeExtra e in atividadesExtras)
                        stratividadesExtras += e.cd_atividade_extra + ",";
                // Remove o último ponto e virgula:
                if (stratividadesExtras.Length > 0)
                    stratividadesExtras = stratividadesExtras.Substring(0, stratividadesExtras.Length - 1);
                int retorno = db.Database.ExecuteSqlCommand("delete from t_atividade_extra where cd_atividade_extra in(" + stratividadesExtras + ")");
                return retorno > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public AtividadeExtra findByIdAtividadeExtraFull(int cdAtividadeExtra) {
            try{
                return (from a in db.AtividadeExtra.Include(a => a.TipoAtividadeExtra).Include(a => a.AtividadeSala).Include(a => a.Funcionario.FuncionarioPessoaFisica)
                        where a.cd_atividade_extra == cdAtividadeExtra
                        select new
                        {
                            a.cd_sala,
                            a.hh_final,
                            a.hh_inicial,
                            a.dt_atividade_extra,
                            a.nm_vagas,
                            a.tx_obs_atividade,
                            a.cd_produto,
                            //a.cd_curso,
                            a.cd_usuario_atendente,
                            a.Funcionario.FuncionarioPessoaFisica.no_pessoa,
                            no_tipo_atividade_extra = a.TipoAtividadeExtra.no_tipo_atividade_extra,
                            a.AtividadeSala.no_sala
                        }).ToList().Select(x => new AtividadeExtra
                        {
                            cd_sala = x.cd_sala,
                            hh_final = x.hh_final,
                            hh_inicial = x.hh_inicial,
                            dt_atividade_extra = x.dt_atividade_extra,
                            nm_vagas = x.nm_vagas,
                            tx_obs_atividade = x.tx_obs_atividade,
                            cd_produto = x.cd_produto,
                            //cd_curso = x.cd_curso,
                            cd_usuario_atendente = x.cd_usuario_atendente,
                            Funcionario = new FuncionarioSGF ()
                            {
                                FuncionarioPessoaFisica = new PessoaFisicaSGF
                                {
                                    no_pessoa = x.no_pessoa
                                }
                            },
                            TipoAtividadeExtra = new TipoAtividadeExtra { 
                                no_tipo_atividade_extra = x.no_tipo_atividade_extra
                            },
                            AtividadeSala = new Sala { 
                                no_sala = x.no_sala
                            }
                        }).FirstOrDefault();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }


        public List<sp_RptAtividadeExtra_Result> getReportAtividadeExtra(Nullable<int> cd_escola, Nullable<System.DateTime> dta_ini, Nullable<System.DateTime> dta_fim, Nullable<int> cd_produto, Nullable<int> cd_curso, Nullable<int> cd_funcionario, Nullable<int> cd_aluno, Nullable<byte> id_participacao, Nullable<byte> id_lancada)
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
                DBParameter param4 = new DBParameter("@cd_produto", cd_produto, DbType.Int32);
                DBParameter param5 = new DBParameter("@cd_curso", cd_curso, DbType.Int32);
                DBParameter param6 = new DBParameter("@cd_funcionario", cd_funcionario, DbType.Int32);
                DBParameter param7 = new DBParameter("@cd_aluno", cd_aluno, DbType.Int32);
                DBParameter param8 = new DBParameter("@id_participacao", id_participacao, DbType.Int16);
                DBParameter param9 = new DBParameter("@id_lancada", id_lancada, DbType.Int16);

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

                dtReportData = dbSql.ExecuteDataTable
                    ("sp_RptAtividadeExtra", paramCollection, CommandType.StoredProcedure);

                var retorno = (List<sp_RptAtividadeExtra_Result>)ConvertTo<sp_RptAtividadeExtra_Result>(dtReportData);
                
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public List<sp_RptAtividadeExtraAluno_Result> getReportAtividadeExtraAluno(Nullable<int> cd_atividade_extra, Nullable<int> cd_aluno, Nullable<byte> id_participou, Nullable<byte> id_lancada, Nullable<int> cd_escola)
        {
            try
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionStringReportProcedure"].ConnectionString;
                string providerName = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionStringReportProcedure"].ProviderName;
                

                DBHelper dbSql = new DBHelper(connectionString, providerName);
                DataTable dt = new DataTable();
                DBHelper dbAluno = new DBHelper(connectionString, providerName);
                DBParameter param1 = new DBParameter("@cd_atividade_extra", cd_atividade_extra, DbType.Int32);
                DBParameter param2 = new DBParameter("@cd_aluno", cd_aluno, DbType.Int32);
                DBParameter param3 = new DBParameter("@id_participou", id_participou, DbType.Int32);
                DBParameter param4 = new DBParameter("@id_lancada", id_lancada, DbType.Int32);
                DBParameter param5 = new DBParameter("@cd_escola", cd_escola, DbType.Int32);
                DBParameterCollection paramCollection = new DBParameterCollection();
                paramCollection.Add(param1);
                paramCollection.Add(param2);
                paramCollection.Add(param3);
                paramCollection.Add(param4);
                paramCollection.Add(param5);
                dt = dbAluno.ExecuteDataTable("sp_RptAtividadeExtraAluno", paramCollection, CommandType.StoredProcedure);

                var retorno = (List<sp_RptAtividadeExtraAluno_Result>)ConvertTo<sp_RptAtividadeExtraAluno_Result>(dt);
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }


        public AtividadeExtraUI returnAtividadeExtraUsuarioAtendente(int cd_atividade_extra, int cd_pessoa_escola) {

           AtividadeExtraUI sql;
            try
            {
                sql = (from atividadeExtra in db.AtividadeExtra
                      where atividadeExtra.cd_atividade_extra == cd_atividade_extra
                         && atividadeExtra.cd_pessoa_escola == cd_pessoa_escola
                      select new AtividadeExtraUI
                      {
                          cd_usuario_atendente = atividadeExtra.AtividadeSysUsuario.cd_usuario,
                          no_usuario = atividadeExtra.AtividadeSysUsuario.no_login
                      }).FirstOrDefault();

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<AtividadeExtra> getAtividadesAluno(SearchParameters parametros, int cd_aluno, int cd_escola) {
            try {
                IEntitySorter<AtividadeAluno> sorter = EntitySorter<AtividadeAluno>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection) (int) parametros.sortOrder);
                IQueryable<AtividadeAluno> sql = from a in db.AtividadeAluno.AsNoTracking()
                                                 where a.cd_aluno == cd_aluno
                                                   /*&& a.AtividadeExtra.cd_pessoa_escola == cd_escola*/
                                                   select a;

                sql = sorter.Sort(sql);

                int limite = sql.Count();

                parametros.ajustaParametrosPesquisa(limite);
                sql = sql.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;

                var retorno = (from a in sql
                               select new {
                                   no_tipo_atividade_extra = a.AtividadeExtra.TipoAtividadeExtra.no_tipo_atividade_extra,
                                   dt_atividade_extra = a.AtividadeExtra.dt_atividade_extra,
                                   hh_inicial = a.AtividadeExtra.hh_inicial,
                                   hh_final = a.AtividadeExtra.hh_final,
                                   id_participou = a.ind_participacao,
                                   tx_obs_atividade = a.tx_obs_atividade_aluno
                               }).ToList().Select(x => new AtividadeExtra {
                                   TipoAtividadeExtra = new TipoAtividadeExtra { no_tipo_atividade_extra = x.no_tipo_atividade_extra },
                                   dt_atividade_extra = x.dt_atividade_extra,
                                   hh_inicial = x.hh_inicial,
                                   hh_final = x.hh_final,
                                   id_participou = x.id_participou,
                                   tx_obs_atividade = x.tx_obs_atividade
                               });

                return retorno.ToList();
            }
            catch(Exception exe) {
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
                        if(value != DBNull.Value)
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

        public List<AtividadeExtra> getAtividadeExtraByCdAtividadeRecorrencia(int cd_atividade_recorrencia)
        {

            List<AtividadeExtra> sql;
            try
            {
                sql = (from atividadeExtra in db.AtividadeExtra
                    where atividadeExtra.cd_atividade_recorrrencia == cd_atividade_recorrencia
                    select atividadeExtra
                    ).ToList();

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }

   
}

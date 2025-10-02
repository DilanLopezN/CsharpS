using System;
using System.Collections.Generic;
using System.Linq;
using Componentes.GenericModel;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using Componentes.GenericDataAccess;
using FundacaoFisk.SGF.Web.Services.Financeiro;
using System.Data;
using System.Data.Entity;
using DALC4NET;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess;
using Componentes.GenericDataAccess.GenericException;
using FundacaoFisk.SGF.Web.Services.Financeiro.Model;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess
{
    public class ReajusteAlunoDataAccess : GenericRepository<ReajusteAluno>, IReajusteAlunoDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public Boolean deleteAllAluno(List<ReajusteAluno> reajustes)
        {
            try
            {
                for (int i = reajustes.Count() - 1; i > 0; i--) 
                {
                    ReajusteAluno reajusteContext = this.findById(reajustes[i].cd_reajuste_aluno, false);
                    this.deleteContext(reajusteContext, false);
                }
                return this.saveChanges(false) > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public DataTable getRptAlunoRestricao(int? cd_escola, int? cd_orgao, DateTime? dt_inicio, DateTime? dt_final, byte? tipodata)
        {
            try
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionStringReportProcedure"].ConnectionString;
                string providerName = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionStringReportProcedure"].ProviderName;


                DataTable dtReportData = new DataTable();

                DBHelper dbSql = new DBHelper(connectionString, providerName);

                DBParameter param1 = new DBParameter("@cd_escola", cd_escola, DbType.Int32);
                DBParameter param2 = new DBParameter("@cd_orgao", cd_orgao, DbType.Int32);
                DBParameter param3 = new DBParameter("@dt_ini", dt_inicio, DbType.DateTime);
                DBParameter param4 = new DBParameter("@dt_fim", dt_final, DbType.DateTime);
                DBParameter param5 = new DBParameter("@tipo_data", tipodata, DbType.Byte);

                DBParameterCollection paramCollection = new DBParameterCollection();
                paramCollection.Add(param1);
                paramCollection.Add(param2);
                paramCollection.Add(param3);
                paramCollection.Add(param4);
                paramCollection.Add(param5);

                dtReportData = dbSql.ExecuteDataTable("sp_RptAlunoRestricao", paramCollection, CommandType.StoredProcedure);
                return dtReportData;

            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }

        }

    }
}

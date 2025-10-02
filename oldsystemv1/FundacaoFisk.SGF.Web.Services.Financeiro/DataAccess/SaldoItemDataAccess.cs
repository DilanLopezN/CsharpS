using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Financeiro.Model;
using FundacaoFisk.SGF.Utils;
using Componentes.Utils;
using Componentes.GenericDataAccess;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess
{
    public class SaldoItemDataAccess : GenericRepository<SaldoItem>, ISaldoItemDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }

        public SaldoItem getSaldoItemByIdItem(int cd_fechamento, int cd_item) {
            try
            {
                return (from s in db.SaldoItem
                    where s.cd_item == cd_item
                        && s.cd_fechamento == cd_fechamento
                    select s).FirstOrDefault();
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<SaldoItem> getSaldoItemById(int cd_fechamento, int cd_escola)
        {
            try
            {
                IEnumerable<SaldoItem> sql = (from s in db.SaldoItem
                                              where s.cd_fechamento == cd_fechamento &&
                                                    s.Fechamentos.cd_pessoa_empresa == cd_escola
                                              select new
                                              {
                                                  cd_saldo_item = s.cd_saldo_item,
                                                  cd_fechamento = s.cd_fechamento,
                                                  cd_item = s.cd_item,
                                                  qt_saldo_atual = s.qt_saldo_atual,
                                                  qt_saldo_data = s.qt_saldo_data,
                                                  qt_saldo_fechamento = s.qt_saldo_fechamento,
                                                  no_item = s.Item.no_item,
                                                  vl_venda_atual = s.vl_venda_atual,
                                                  vl_venda_fechamento = s.vl_venda_fechamento,
                                                  vl_custo_atual = s.vl_custo_atual,
                                                  vl_custo_fechamento = s.vl_custo_fechamento,
                                                  cd_grupo_estoque = s.Item.cd_grupo_estoque,
                                                  cd_tipo_item = s.Item.cd_tipo_item,
                                                  id_movto_estoque = s.Item.TipoItem.id_movimentar_estoque,
                                                  id_material_didatico = s.Item.id_material_didatico,
                                                  id_voucher_carga = s.Item.id_voucher_carga

                                              }).ToList().Select(x => new SaldoItem
                                                {
                                                    cd_saldo_item = x.cd_saldo_item,
                                                    cd_fechamento = x.cd_fechamento,
                                                    cd_item = x.cd_item,
                                                    qt_saldo_atual = x.qt_saldo_atual,
                                                    qt_saldo_data = x.qt_saldo_data,
                                                    qt_saldo_fechamento = x.qt_saldo_fechamento,
                                                    no_item = x.no_item,
                                                    vl_venda_atual = x.vl_venda_atual,
                                                    vl_venda_fechamento = x.vl_venda_fechamento,
                                                    vl_custo_atual = x.vl_custo_atual,
                                                    vl_custo_fechamento = x.vl_custo_fechamento,
                                                    cd_grupo_estoque = x.cd_grupo_estoque,
                                                    cd_tipo_item = x.cd_tipo_item,
                                                    id_movto_estoque = x.id_movto_estoque,
                                                    id_material_didatico = x.id_material_didatico,
                                                    id_voucher_carga = x.id_voucher_carga
                                              });
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool processarSaldoItens(Nullable<int> cd_fechamento, Nullable<byte> id_tipo)
        {
            try
            {
                db.Database.Connection.Open();
                var command = db.Database.Connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = @"sp_fechamento_estoque";
                command.CommandTimeout = 180;

                var sqlParameters = new List<SqlParameter>();

                if (cd_fechamento != null)
                    sqlParameters.Add(new SqlParameter("@cd_fechamento", cd_fechamento));
                else
                    sqlParameters.Add(new SqlParameter("@cd_fechamento", DBNull.Value));

                if (id_tipo != null)
                    sqlParameters.Add(new SqlParameter("@id_tipo", id_tipo));
                else
                    sqlParameters.Add(new SqlParameter("@id_tipo", DBNull.Value));


                var parameter = new SqlParameter("@result", SqlDbType.Int);
                parameter.Direction = ParameterDirection.ReturnValue;
                sqlParameters.Add(parameter);

                command.Parameters.AddRange(sqlParameters.ToArray());
                command.ExecuteReader();

                //var retunvalue = command.Parameters["@retorno"].Value;
                var retunvalue = Convert.ToBoolean((int)command.Parameters["@result"].Value);
                db.Database.Connection.Close();
                return retunvalue ? Convert.ToBoolean((int)SaldoItem.StatusProcedure.SUCESSO_EXECUCAO_PROCEDURE) : Convert.ToBoolean((int)SaldoItem.StatusProcedure.ERRO_EXECUCAO_PROCEDURE);
            }
            catch (SqlException exe)
            {
                db.Database.Connection.Close();
                throw new DataAccessException(exe);
            }
            catch (Exception exe)
            {
                db.Database.Connection.Close();
                throw new DataAccessException(exe);
            }

        }

    } 
}

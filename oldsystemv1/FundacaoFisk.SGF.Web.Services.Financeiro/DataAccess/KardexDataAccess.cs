using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using DALC4NET;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Financeiro.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess
{
    public class KardexDataAccess : GenericRepository<Kardex>, IKardexDataAccess
    {
        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }

        public IEnumerable<Kardex> getKardexByOrigem(int cd_origem, int cd_registro_origem)
        {
            try
            {
                var sql = from k in db.Kardex
                          where k.cd_origem == cd_origem
                               && k.cd_registro_origem == cd_registro_origem
                          select k;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool existeKardexItemMovimentoByOrigem(int cd_origem, int cd_registro_origem)
        {
            try
            {
                var sql = (from k in db.Kardex
                           where k.cd_origem == cd_origem
                                && k.cd_registro_origem == cd_registro_origem
                           select k.cd_kardex).Any();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Kardex getKardexByOrigemItem(int cd_origem, int cd_registro_origem, int cd_item, int cd_escola)
        {
            try
            {
                var sql = (from k in db.Kardex
                           where k.cd_origem == cd_origem
                                && k.cd_registro_origem == cd_registro_origem
                                && k.cd_item == cd_item
                                && k.cd_pessoa_empresa == cd_escola
                           select k).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public int getSaldoItem(int cd_item, DateTime dataLimite, int cd_escola)
        {
            try
            {
                /* 
                              *  ENTRADA = 1, //Devlução  > 1
                                            SAIDA = 2 //Empréstimo > -1
                              * (-2 * k.id_tipo_movimento + 3) => significa que se o  kardex for 1(entrada) sera (-2 * 1) + 3 = 1
                                                                                               significa que se o  kardex for 2(saida) sera (-2 * 2) + 3 = -1
                            a soma irá variar conforme os registros de entrada e saída corforme o tipo de item.
                            */
                var sql = (from k in db.Kardex
                           where k.cd_item == cd_item
                                && k.cd_pessoa_empresa == cd_escola
                                && k.dt_kardex <= dataLimite
                           select k.qtd_kardex * (-2 * k.id_tipo_movimento + 3)).Sum();

                return sql.HasValue ? sql.Value : 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<KardexUI> st_Rptkardex(int cd_pessoa_escola, int cd_item, DateTime dt_ini, DateTime dt_fim, int cd_grupo, byte tipo)
        {
            try
            {
                var sql = from kardex in db.Kardex
                          where kardex.cd_pessoa_empresa == cd_pessoa_escola
                            && (kardex.Item.TipoItem.id_movimentar_estoque || kardex.Item.id_voucher_carga)
                            && kardex.Item.id_item_ativo
                          select kardex;

                if (cd_item > 0)
                    sql = from kardex in sql
                          where kardex.Item.cd_item == cd_item
                          select kardex;

                if (cd_grupo > 0)
                    sql = from kardex in sql
                          where kardex.Item.GrupoEstoque.cd_grupo_estoque == cd_grupo
                          select kardex;

                if (tipo > 0)
                    sql = from kardex in sql
                          where kardex.Item.cd_tipo_item == tipo
                          select kardex;

                var sql1 = (from kardex in sql
                            where (kardex.id_tipo_movimento == (byte)Kardex.TipoMovimento.ENTRADA
                                 || kardex.id_tipo_movimento == (byte)Kardex.TipoMovimento.SAIDA)
                               && kardex.dt_kardex >= dt_ini
                               && kardex.dt_kardex <= dt_fim
                            select new KardexUI
                            {
                                cd_kardex = kardex.cd_kardex,
                                cd_pessoa_escola = kardex.cd_pessoa_empresa,
                                dt_kardex = kardex.dt_kardex,
                                id_saldo = 2,
                                id_tipo_movimento = kardex.id_tipo_movimento,
                                no_grupo_estoque = kardex.Item.GrupoEstoque.no_grupo_estoque ?? "Sem Grupo de Estoque",
                                no_item = kardex.Item.no_item,
                                no_pessoa = kardex.Escola.no_pessoa,
                                tx_obs_kardex = kardex.tx_obs_kardex,
                                qt_entrada = kardex.id_tipo_movimento == (int)Kardex.TipoMovimento.ENTRADA ? kardex.qtd_kardex : 0,
                                qt_saida = kardex.id_tipo_movimento == (int)Kardex.TipoMovimento.SAIDA ? kardex.qtd_kardex : 0,
                                qt_inicial = 0,
                                vl_inicial = 0,
                                vl_entrada = kardex.id_tipo_movimento == (int)Kardex.TipoMovimento.ENTRADA ? kardex.vl_medio : 0,
                                vl_saida = kardex.id_tipo_movimento == (int)Kardex.TipoMovimento.SAIDA ? kardex.vl_medio : 0,
                                cd_item = kardex.cd_item,
                                qt_saldo = kardex.qt_saldo,
                                vl_saldo = kardex.vl_saldo
                            }).Union(from kardex in sql
                                     orderby kardex.cd_pessoa_empresa,
                                             kardex.Item.GrupoEstoque.no_grupo_estoque,
                                             kardex.Item.no_item,
                                             4,
                                             kardex.dt_kardex,
                                             kardex.cd_kardex
                                     where (kardex.id_tipo_movimento == (byte)Kardex.TipoMovimento.ENTRADA
                                              || kardex.id_tipo_movimento == (byte)Kardex.TipoMovimento.SAIDA)
                                          && kardex.dt_kardex < dt_ini
                                          && !(from k in db.Kardex
                                               where (k.id_tipo_movimento == (byte)Kardex.TipoMovimento.ENTRADA
                                                      || k.id_tipo_movimento == (byte)Kardex.TipoMovimento.SAIDA)
                                                    && k.dt_kardex >= dt_ini
                                                    && k.dt_kardex <= dt_fim
                                                    && k.cd_item == kardex.cd_item
                                                    && k.cd_pessoa_empresa == cd_pessoa_escola
                                               select k.cd_kardex).Any()
                                     select new KardexUI
                                  {
                                      cd_kardex = 0,
                                      cd_pessoa_escola = 0,
                                      dt_kardex = dt_ini,
                                      id_saldo = 1, // itens com saldo anterior
                                      id_tipo_movimento = 0,
                                      no_grupo_estoque = kardex.Item.GrupoEstoque.no_grupo_estoque ?? "Sem Grupo de Estoque",
                                      no_item = kardex.Item.no_item,
                                      no_pessoa = kardex.Escola.no_pessoa,
                                      tx_obs_kardex = "Saldo Anterior",
                                      qt_entrada = 0,
                                      qt_saida = 0,
                                      qt_inicial = 0,
                                      vl_inicial = 0,
                                      vl_entrada = 0,
                                      vl_saida = 0,
                                      cd_item = kardex.cd_item,
                                      qt_saldo = 0,
                                      vl_saldo = 0
                                  });

                return sql1;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<KardexUI> st_RptkardexAnterior(int cd_pessoa_escola, int cd_item, DateTime dt_ini, DateTime dt_fim, int cd_grupo, byte tipo)
        {
            try
            {
                var sql = from kardex in db.Kardex
                          where kardex.cd_pessoa_empresa == cd_pessoa_escola
                            && kardex.Item.TipoItem.id_movimentar_estoque
                            && kardex.Item.id_item_ativo
                          select kardex;

                if (cd_item > 0)
                    sql = from kardex in sql
                          where kardex.Item.cd_item == cd_item
                          select kardex;

                if (cd_grupo > 0)
                    sql = from kardex in sql
                          where kardex.Item.GrupoEstoque.cd_grupo_estoque == cd_grupo
                          select kardex;

                if (tipo > 0)
                    sql = from kardex in sql
                          where kardex.Item.cd_tipo_item == tipo
                          select kardex;

                var sql1 = (from kardex in sql
                            orderby kardex.cd_pessoa_empresa,
                                    kardex.Item.GrupoEstoque.no_grupo_estoque,
                                    kardex.Item.no_item,
                                    4,
                                    kardex.dt_kardex,
                                    kardex.cd_kardex
                            where (kardex.id_tipo_movimento == (byte)Kardex.TipoMovimento.ENTRADA
                                     || kardex.id_tipo_movimento == (byte)Kardex.TipoMovimento.SAIDA)
                                 && kardex.dt_kardex < dt_ini
                                 && !(from k in db.Kardex
                                      where (k.id_tipo_movimento == (byte)Kardex.TipoMovimento.ENTRADA
                                             || k.id_tipo_movimento == (byte)Kardex.TipoMovimento.SAIDA)
                                           && k.dt_kardex >= dt_ini
                                           && k.dt_kardex <= dt_fim
                                           && k.cd_item == cd_item
                                           && k.cd_pessoa_empresa == cd_pessoa_escola
                                      select k.cd_kardex).Any()
                            select new KardexUI
                         {
                             cd_kardex = 0,
                             cd_pessoa_escola = kardex.cd_pessoa_empresa,
                             dt_kardex = kardex.dt_kardex,
                             id_saldo = 1, // itens com saldo anterior
                             id_tipo_movimento = kardex.id_tipo_movimento,
                             no_grupo_estoque = kardex.Item.GrupoEstoque.no_grupo_estoque ?? "Sem Grupo de Estoque",
                             no_item = kardex.Item.no_item,
                             no_pessoa = kardex.Escola.no_pessoa,
                             tx_obs_kardex = "Saldo Anterior",
                             qt_entrada = 0,
                             qt_saida = 0,
                             qt_inicial = 0,
                             vl_inicial = 0,
                             vl_entrada = 0,
                             vl_saida = 0,
                             cd_item = kardex.cd_item,
                            qt_saldo = 0,
                            vl_saldo = 0
                         });
                return sql1;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public DateTime getMaxMovimentoKardex(int cd_pessoa_escola)
        {
            try
            {
                SGFWebContext comp = new SGFWebContext();
                byte origem = (byte)comp.LISTA_ORIGEM_LOGS["Emprestimo"];

                DateTime dataMaxReturn = (from k in db.Kardex
                                          where k.cd_pessoa_empresa == cd_pessoa_escola
                                             && k.cd_origem == origem
                                          orderby k.dt_kardex descending
                                          select k.dt_kardex).FirstOrDefault();

                return dataMaxReturn;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public decimal? getSaldoValorItem(int cd_item, DateTime dataLimite, int cd_escola)
        {
            try
            {
                /* 
                              *  ENTRADA = 1, //Devlução  > 1
                                            SAIDA = 2 //Empréstimo > -1
                              * (-2 * k.id_tipo_movimento + 3) => significa que se o  kardex for 1(entrada) sera (-2 * 1) + 3 = 1
                                                                                               significa que se o  kardex for 1(entrada) sera (-2 * 2) + 3 = -1
                            a soma irá variar conforme os registros de entrada e saída corforme o tipo de item.
                            */
                var sql = (from k in db.Kardex
                           where k.cd_item == cd_item
                                && k.cd_pessoa_empresa == cd_escola
                                && k.dt_kardex <= dataLimite
                           select k.vl_medio * (-2 * k.id_tipo_movimento + 3)).Sum();

                return sql.HasValue ? sql.Value : 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Kardex> getKardexItensMovimentoNF(int cd_movimento, int cd_pessoa)
        {
            try
            {
                SGFWebContext comp = new SGFWebContext();
                byte origem = (byte)comp.LISTA_ORIGEM_LOGS["ItemMovimento"];
                var sql = from k in db.Kardex
                          where k.cd_origem == origem &&
                                 db.ItemMovimento.Where(x => x.Movimento.cd_pessoa_empresa == cd_pessoa && x.Movimento.cd_movimento == cd_movimento &&
                                                        x.cd_item == k.cd_item && x.cd_item_movimento == k.cd_registro_origem).Any()
                          //db.Movimento.Any(m => m.cd_movimento == cd_movimento && m.cd_pessoa_empresa == cd_pessoa &&
                          //                 m.ItensMovimento.Any(it => it.cd_item == k.cd_item && 
                          //                                      it.cd_item_movimento == k.cd_registro_origem))                               
                          select k;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public bool existeKardexItem(int cd_item)
        {
            try
            {
                bool sql = (from k in db.Kardex
                            where k.cd_item == cd_item
                            select k.cd_kardex).Any();

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public bool existeKardexItemEsc(int cd_item, int cdEscola)
        {
            try
            {
                bool sql = (from k in db.Kardex
                            where k.cd_item == cd_item
                            && k.cd_pessoa_empresa == cdEscola
                            select k.cd_kardex).Any();

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public bool existeKardexItemEscolas(int cd_item, List<int> cdEscolas)
        {
            try
            {
                bool sql = (from k in db.Kardex
                            where k.cd_item == cd_item
                            && cdEscolas.Contains(k.cd_pessoa_empresa)
                            select k.cd_kardex).Any();

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }


        public List<SaldoFinanceiro> getFechamentoKardex(DateTime? dt_kardex, int cd_item, int cd_pessoa_empresa)
        {
            try
            {
                var sql = (from k in db.Kardex
                           where k.cd_pessoa_empresa == cd_pessoa_empresa
                           && k.cd_item == cd_item
                           select k).ToList();

                if(dt_kardex != null)
                    sql = sql.Where(x => x.dt_kardex > dt_kardex).ToList();

                var retorno = sql.Select(x => new SaldoFinanceiro
                           {
                               saida = x.id_tipo_movimento == (byte)ContaCorrente.Tipo.SAIDA ? x.qtd_kardex : 0,
                               entrada = x.id_tipo_movimento == (byte)ContaCorrente.Tipo.ENTRADA ? x.qtd_kardex : 0
                           }).ToList();

                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        //public List<sp_RptInventario_Result> getRptInventario(int? cd_escola, DateTime? dt_analise, byte? id_valor)
        public DataTable getRptInventario(int? cd_escola, DateTime? dt_analise, byte? id_valor, string tipoItem)
        {
            try
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionStringReportProcedure"].ConnectionString;
                string providerName = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionStringReportProcedure"].ProviderName;


                DataTable dtReportData = new DataTable();

                DBHelper dbSql = new DBHelper(connectionString, providerName);

                DBParameter param1 = new DBParameter("@cd_escola", cd_escola, DbType.Int32);
                DBParameter param2 = new DBParameter("@dt_analise", dt_analise, DbType.DateTime);
                DBParameter param3 = new DBParameter("@id_valor", id_valor, DbType.Byte);
                DBParameter param4 = new DBParameter("@tp_item", tipoItem, DbType.String);

                DBParameterCollection paramCollection = new DBParameterCollection();
                paramCollection.Add(param1);
                paramCollection.Add(param2);
                paramCollection.Add(param3);
                paramCollection.Add(param4);

                dtReportData = dbSql.ExecuteDataTable("sp_RptInventario", paramCollection, CommandType.StoredProcedure);
                //List<sp_RptInventario_Result> rtp = (List<sp_RptInventario_Result>)ConvertTo<sp_RptInventario_Result>(dtReportData);
                return dtReportData;

            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }

        }
        public IList<T> ConvertTo<T>(DataTable table)
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
        public IList<T> ConvertToData<T>(IList<DataRow> rows)
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

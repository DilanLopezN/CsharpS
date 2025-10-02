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
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Secretaria.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;
using log4net;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess
{
    public class GeraNotasXmlDataAccess : GenericRepository<ImportacaoXML>, IGeraNotasXmlDataAccess
    {
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }

        // DATAACCESS US119 PESQUISA GERAL VIDE REGRAS CHECKBOX
        public IEnumerable<ImportacaoXML> getListaXmlGerados(SearchParameters parametros, XmlSearchUI notatualizaUI)
        {
            try
            {
                List<int> listOfInts = notatualizaUI.cd_importacao_XML.Split(',').Select(Int32.Parse).ToList();
                IEntitySorter<ImportacaoXML> sorter = EntitySorter<ImportacaoXML>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<ImportacaoXML> sql;
                
                sql = from n in db.ImportacaoXML
                      select n;
              
                    sql = from n in sql
                          where listOfInts.Contains(n.id_tipo_importacao) && n.id_resolvido == notatualizaUI.id_resolvido
                              select n;
              
                if (!string.IsNullOrEmpty(notatualizaUI.no_pessoa))
                    sql = from n in sql
                          where n.no_pessoa.Contains(notatualizaUI.no_pessoa)
                          select n;

                if (!string.IsNullOrEmpty(notatualizaUI.no_escola))
                    if (notatualizaUI.inicio == 1)
                        sql = from n in sql
                              where n.no_escola.StartsWith(notatualizaUI.no_escola)
                              select n;
                    else
                        sql = from n in sql
                              where n.no_escola.Contains(notatualizaUI.no_escola)
                              select n;

                if (!string.IsNullOrEmpty(notatualizaUI.nm_nota_fiscal))
                    sql = from n in sql
                          where n.nm_nota_fiscal.Contains(notatualizaUI.nm_nota_fiscal)
                          select n;

                if (notatualizaUI.dt_emissao_nf.HasValue)
                    sql = from n in sql
                          where n.dt_emissao_nf == notatualizaUI.dt_emissao_nf
                          select n;

                if (!string.IsNullOrEmpty(notatualizaUI.itemExistente))
                    sql = from n in sql
                          where n.no_item_inexistente.Contains(notatualizaUI.itemExistente)
                          select n;

                if (notatualizaUI.data_ini.HasValue)
                    sql = from n in sql
                          where n.dt_importacao_xml >= notatualizaUI.data_ini
                          select n;

                if (notatualizaUI.dataFim.HasValue)
                    sql = from n in sql
                          where n.dt_importacao_xml <= notatualizaUI.dataFim
                          select n;

                sql = (from n in sql
                       join s in db.UsuarioWebSGF on n.cd_usuario equals s.cd_usuario
                       select new
                       {
                           cd_importacao_XML = n.cd_importacao_XML,
                           no_usuario = s.no_login,
                           no_arquivo_XML = n.no_arquivo_XML,
                           dc_path_arquivo = n.dc_path_arquivo,
                           dc_mensagem_XML = n.dc_mensagem_XML,
                           nm_nota_fiscal = n.nm_nota_fiscal,
                           dt_emissao_nf = n.dt_emissao_nf,
                           no_escola = n.no_escola,
                           no_pessoa = n.no_pessoa,
                           no_item_inexistente = n.no_item_inexistente,
                           id_tipo_importacao = n.id_tipo_importacao,
                           id_resolvido = n.id_resolvido,
                           dt_importacao_xml = n.dt_importacao_xml
                       }).Distinct().ToList().Select(x => new ImportacaoXML
                       {
                           cd_importacao_XML = x.cd_importacao_XML,
                           no_usuario = x.no_usuario,
                           no_arquivo_XML = x.no_arquivo_XML,
                           dc_path_arquivo = x.dc_path_arquivo,
                           dc_mensagem_XML = x.dc_mensagem_XML,
                           nm_nota_fiscal = x.nm_nota_fiscal,
                           dt_emissao_nf = x.dt_emissao_nf,
                           no_escola = x.no_escola,
                           no_pessoa = x.no_pessoa,
                           no_item_inexistente = x.no_item_inexistente,
                           id_tipo_importacao = x.id_tipo_importacao,
                           id_resolvido = x.id_resolvido,
                           dt_importacao_xml = x.dt_importacao_xml
                       }).AsQueryable();

                sql = sorter.Sort(sql);
                int limite = sql.Count();
                parametros.ajustaParametrosPesquisa(limite);
                sql = sql.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;

                return sql;

            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }

        }

        public int abrirGerarXML(int cd_usuario)
        {
            try
            {

                db.Database.Connection.Open();
                var command = db.Database.Connection.CreateCommand();
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = @"sp_dir_xml";

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>();

                if (cd_usuario > 0)
                    sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@cd_usuario", cd_usuario));
                else
                    sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@cd_usuario", DBNull.Value));
                
                var parameter = new System.Data.SqlClient.SqlParameter("@result", System.Data.SqlDbType.Int);
                parameter.Direction = System.Data.ParameterDirection.ReturnValue;

                sqlParameters.Add(parameter);

                command.Parameters.AddRange(sqlParameters.ToArray());
                command.ExecuteReader();

                int retunvalue = (int)command.Parameters["@result"].Value;

                db.Database.Connection.Close();
                return retunvalue;
            }
            catch (System.Data.SqlClient.SqlException exe)
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

        public int postGerarXmlProc(int cd_usuario)
        {
            try
            {
                db.Database.Connection.Open();
                var command = db.Database.Connection.CreateCommand();
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = @"sp_import_xml";
                command.CommandTimeout = 600;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>();

                if (cd_usuario > 0)
                    sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@cd_usuario", cd_usuario));
                else
                    sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@cd_usuario", DBNull.Value));

                var parameter = new System.Data.SqlClient.SqlParameter("@msg", System.Data.SqlDbType.VarChar, 1024);
                parameter.Value = "";
                parameter.Direction = System.Data.ParameterDirection.InputOutput;
                sqlParameters.Add(parameter);

                parameter = new System.Data.SqlClient.SqlParameter("@result", System.Data.SqlDbType.Int);
                parameter.Direction = System.Data.ParameterDirection.ReturnValue;
                sqlParameters.Add(parameter);

                command.Parameters.AddRange(sqlParameters.ToArray());
                command.ExecuteReader();

                string retunmsg = (string)command.Parameters["@msg"].Value;
                int retunvalue = retunmsg == "" ? 0 : 1;

                db.Database.Connection.Close();
                return retunvalue;
            }
            catch (System.Data.SqlClient.SqlException exe)
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

        public IEnumerable<ImportacaoXML> buscarGerarXML(int cd_usuario)
        {
            try
            {
                IEnumerable<ImportacaoXML> sql = from n in db.ImportacaoXML.AsNoTracking()
                                                 where n.id_tipo_importacao == 0 && n.cd_usuario == cd_usuario
                                                 select n;


                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<ImportacaoXML> setAtualizaXML(List<int> cdImportXML)
        {
            try
            {
                IEnumerable<ImportacaoXML> sql = from n in db.ImportacaoXML.AsNoTracking()
                                                 where cdImportXML.Contains(n.cd_importacao_XML)
                                             select n;

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

    }
}

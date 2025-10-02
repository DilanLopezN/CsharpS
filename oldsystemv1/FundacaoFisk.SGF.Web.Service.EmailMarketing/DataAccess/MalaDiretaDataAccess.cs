using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.EmailMarketing.Comum.IDataAccess;
using System.Data;
using DALC4NET;

namespace FundacaoFisk.SGF.Web.Service.EmailMarketing.DataAccess
{
    public class MalaDiretaDataAccess : GenericRepository<MalaDireta>, IMalaDiretaDataAccess
    {

        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }

        public IEnumerable<MalaDireta> searchHistoricoMalaDireta(Componentes.Utils.SearchParameters parametros, string dc_assunto, DateTime? dt_mala_direta, int cd_empresa, int id_tipo_mala)
        {
            try
            {
                IEntitySorter<MalaDireta> sorter = EntitySorter<MalaDireta>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<MalaDireta> sql;

                sql = from m in db.MalaDireta.AsNoTracking()
                      where m.cd_escola == cd_empresa
                      select m;

                if (!string.IsNullOrEmpty(dc_assunto))
                    sql = from m in sql
                          where m.dc_assunto.Contains(dc_assunto)
                          select m;
                if (dt_mala_direta.HasValue)
                    sql = from m in sql
                          where m.dt_mala_direta == dt_mala_direta
                          select m;
                if (id_tipo_mala > 0)
                    sql = from m in sql
                          where m.id_tipo_mala == id_tipo_mala
                          select m;
                sql = sorter.Sort(sql);
                int limite = sql.Select(x => x.cd_mala_direta).Count();
                parametros.ajustaParametrosPesquisa(limite);
                sql = sql.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;
                var retorno = (from m in sql
                       select new
                       {
                           m.cd_mala_direta,
                           m.dt_mala_direta,
                           m.dc_assunto,
                           qtd_enderecos = m.ListasEnderecoMala.Count(),
                           no_login = m.Usuario.no_login
                       }).ToList().Select(x => new MalaDireta
                                                       {
                                                           cd_mala_direta = x.cd_mala_direta,
                                                           dt_mala_direta = x.dt_mala_direta,
                                                           dc_assunto = x.dc_assunto,
                                                           qtd_enderecos_enviados = x.qtd_enderecos,
                                                           Usuario = new UsuarioWebSGF{
                                                               no_login = x.no_login
                                                           }
                                                       });
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public MalaDireta getMalaDiretaEditView(int cd_mala_direta, int cd_empresa)
        {
            try
            {
                var retorno = (from m in db.MalaDireta
                          where m.cd_escola == cd_empresa && m.cd_mala_direta == cd_mala_direta
                          select new { 
                              m.id_sexo,
                              m.dc_assunto,
                              m.tx_msg_header,
                              m.tx_msg_body,
                              m.tx_msg_footer,
                              m.nm_mes_nascimento,
                              m.hh_inicial,
                              m.hh_final,
                              m.dt_mala_direta
                          }).ToList().Select(x => new MalaDireta{
                              id_sexo = x.id_sexo,
                              dc_assunto = x.dc_assunto,
                              tx_msg_header = x.tx_msg_header,
                              tx_msg_body = x.tx_msg_body,
                              tx_msg_footer = x.tx_msg_footer,
                              nm_mes_nascimento = x.nm_mes_nascimento,
                              hh_inicial = x.hh_inicial,
                              hh_final = x.hh_inicial,
                              dt_mala_direta = x.dt_mala_direta
                          }).FirstOrDefault();
                return retorno;

            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public MalaDireta getMalaDiretaForView(int cd_mala_direta, int cd_empresa)
        {
            try
            {
                var retorno = (from m in db.MalaDireta
                               where m.cd_escola == cd_empresa && m.cd_mala_direta == cd_mala_direta
                               select new
                               {
                                   m.tx_msg_body,
                                   m.tx_msg_footer
                               }).ToList().Select(x => new MalaDireta
                               {
                                   tx_msg_body = x.tx_msg_body,
                                   tx_msg_footer = x.tx_msg_footer
                               }).FirstOrDefault();
                return retorno;

            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<MalaDiretaCurso> getCursosMalaDireta(int cd_mala_direta, int cd_empresa)
        {
            try
            {
                var retorno = (from mc in db.MalaDiretaCurso
                               where mc.MalaDireta.cd_escola == cd_empresa && mc.cd_mala_direta == cd_mala_direta
                               select new
                               {
                                   mc.cd_curso,
                                   mc.Curso.no_curso
                               }).ToList().Select(x => new MalaDiretaCurso
                               {
                                   cd_curso = x.cd_curso,
                                   no_curso = x.no_curso
                               });
                return retorno;

            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<MalaDiretaProduto> getProdutosMalaDireta(int cd_mala_direta, int cd_empresa)
        {
            try
            {
                var retorno = (from mc in db.MalaDiretaProduto
                               where mc.MalaDireta.cd_escola == cd_empresa && mc.cd_mala_direta == cd_mala_direta
                               select new
                               {
                                   mc.cd_produto,
                                   mc.Produto.no_produto
                               }).ToList().Select(x => new MalaDiretaProduto
                               {
                                   cd_produto = x.cd_produto,
                                   no_produto = x.no_produto
                               });
                return retorno;

            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<MalaDiretaPeriodo> getPeriodosMalaDireta(int cd_mala_direta, int cd_empresa)
        {
            try
            {
                var retorno = from mp in db.MalaDiretaPeriodo
                              where mp.MalaDireta.cd_escola == cd_empresa && mp.cd_mala_direta == cd_mala_direta
                               select mp;
                return retorno;

            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<MalaDiretaCadastro> getTiposPessoaMalaDireta(int cd_mala_direta, int cd_empresa)
        {
            try
            {
                var retorno = from mc in db.MalaDiretaCadastro
                              where mc.MalaDireta.cd_escola == cd_empresa && mc.cd_mala_direta == cd_mala_direta
                              select mc;
                return retorno;

            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public IEnumerable<MalaDireta> getMalaDiretaPorAluno(Componentes.Utils.SearchParameters parametros, int cd_pessoa, int cd_empresa, string assunto, DateTime? dtaIni, DateTime? dtaFim)
        {
            try
            {
                IEntitySorter<MalaDireta> sorter = EntitySorter<MalaDireta>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<MalaDireta> sql;

                sql = from m in db.MalaDireta.AsNoTracking()
                           where m.cd_escola == cd_empresa &&
                           m.ListasEnderecoMala.Where(l => (l.id_cadastro == (int)MalaDireta.TipoCadastro.ALUNO || l.id_cadastro == (int)MalaDireta.TipoCadastro.CLIENTE) && l.cd_cadastro == cd_pessoa).Any()
                           select m;
                 if (!String.IsNullOrEmpty(assunto))
                     sql = from c in sql
                           where c.dc_assunto.Contains(assunto)
                           select c;
                 if (dtaIni != null)
                     if (dtaFim != null)

                         sql = from c in sql
                               where c.dt_mala_direta >= dtaIni && c.dt_mala_direta <= dtaFim
                               select c;
                     else

                         sql = from c in sql
                               where c.dt_mala_direta >= dtaIni
                               select c;

                 //Alteração chamado 287599 - para trazer tambem do prospect
                     int? idPropect = (from a in db.Aluno
                         join prospect in db.Prospect on a.cd_prospect equals prospect.cd_prospect
                         join p in db.PessoaSGF on prospect.cd_pessoa_fisica equals p.cd_pessoa 
                         where a.cd_pessoa_aluno == cd_pessoa &&
                               a.cd_pessoa_escola == cd_empresa &&
                               prospect.cd_pessoa_escola == cd_empresa
                         select prospect.cd_pessoa_fisica).FirstOrDefault();

                    var sqlMalaProspect = from m in db.MalaDireta.AsNoTracking()
                         where m.cd_escola == cd_empresa &&
                               m.ListasEnderecoMala.Where(l => l.id_cadastro == (int)MalaDireta.TipoCadastro.PROSPECT && 
                                                               (l.cd_cadastro == ((idPropect != null) ? idPropect : 0))).Any()
                         select m;
                     if (!String.IsNullOrEmpty(assunto))
                         sqlMalaProspect = from c in sqlMalaProspect
                             where c.dc_assunto.Contains(assunto)
                             select c;
                     if (dtaIni != null)
                         if (dtaFim != null)

                             sqlMalaProspect = from c in sqlMalaProspect
                                 where c.dt_mala_direta >= dtaIni && c.dt_mala_direta <= dtaFim
                                 select c;
                         else

                             sqlMalaProspect = from c in sqlMalaProspect
                                 where c.dt_mala_direta >= dtaIni
                                 select c;

                  sql = sql.Union(sqlMalaProspect);
                 
                 sql = sorter.Sort(sql);
                 int limite = sql.Select(x => x.cd_mala_direta).Count();
                 parametros.ajustaParametrosPesquisa(limite);
                 sql = sql.Skip(parametros.from).Take(parametros.qtd_limite);
                 parametros.qtd_limite = limite;  

                 var retorno = (from m in sql
                               select new
                               {
                                   m.cd_mala_direta,
                                   m.dc_assunto,
                                   m.dt_mala_direta
                               }).ToList().Select(x => new MalaDireta
                               {
                                   cd_mala_direta = x.cd_mala_direta,
                                   dc_assunto = x.dc_assunto,
                                   dt_mala_direta = x.dt_mala_direta
                               });
                return retorno;

            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public DataTable gerarEtiqueta(int cd_mala_direta)
        {
            try
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionStringReportProcedure"].ConnectionString;
                string providerName = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionStringReportProcedure"].ProviderName;

                DBHelper dbSql = new DBHelper(connectionString, providerName);

                DBParameter param1 = new DBParameter("@cod_mala_direta", cd_mala_direta, DbType.Int32);
                DBParameterCollection paramCollection = new DBParameterCollection();
                paramCollection.Add(param1);

                return dbSql.ExecuteDataTable("st_RptEtiqueta", paramCollection, CommandType.StoredProcedure);
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}

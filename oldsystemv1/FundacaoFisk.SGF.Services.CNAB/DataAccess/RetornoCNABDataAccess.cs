using System;
using System.Collections.Generic;
using System.Linq;
using FundacaoFisk.SGF.GenericModel;
using Componentes.GenericDataAccess.GenericRepository;
using System.Data.Entity;
using Componentes.Utils;
using Componentes.GenericDataAccess;
using FundacaoFisk.SGF.Web.Services.CNAB.Comum.IDataAccess;
using Componentes.GenericDataAccess.GenericException;


namespace FundacaoFisk.SGF.Web.Services.CNAB.DataAccess
{
    public class RetornoCNABDataAccess : GenericRepository<RetornoCNAB>, IRetornoCNABDataAccess
    {


        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public IEnumerable<RetornoCNAB> searchRetornoCNAB(SearchParameters parametros, int cd_carteira, int cd_usuario, int status, string descRetorno, DateTime? dtInicial,
                                                 DateTime? dtFinal, string nossoNumero, int cd_empresa, int cd_responsavel, int cd_aluno)
        {
            try
            {
                IEntitySorter<RetornoCNAB> sorter = EntitySorter<RetornoCNAB>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<RetornoCNAB> sql;
                sql = from t in db.RetornoCNAB.AsNoTracking()
                      where t.LocalMovto.cd_pessoa_empresa == cd_empresa
                      select t;

                if (cd_carteira > 0)
                    sql = from t in sql
                          where t.cd_local_movto == cd_carteira
                          select t;

                if (cd_usuario > 0)
                    sql = from t in sql
                          where t.cd_usuario == cd_usuario
                          select t;

                if (status > 0)
                    sql = from t in sql
                          where t.id_status_cnab == status
                          select t;

                if (dtInicial.HasValue)
                {
                    var dtInicialP = dtInicial.Value;
                    sql = from t in sql
                          where DbFunctions.TruncateTime(t.dt_cadastro_cnab) >= dtInicialP.Date
                          select t;
                }

                if (dtFinal.HasValue)
                {
                    var dtFinalP = dtFinal.Value;
                    sql = from t in sql
                          where DbFunctions.TruncateTime(t.dt_cadastro_cnab) <= dtFinalP.Date
                          select t;
                }

                if (!string.IsNullOrEmpty(nossoNumero))
                {
                    sql = from t in sql
                          where t.TitulosRetornoCNAB.Any(x => x.dc_nosso_numero == nossoNumero)
                          select t;
                }

                if (descRetorno != null && descRetorno != "")
                    sql = from t in sql
                          where t.no_arquivo_retorno.Contains(descRetorno)
                          select t;

                if (cd_responsavel > 0)
                    sql = from t in sql
                        where t.TitulosRetornoCNAB.Any(x => x.Titulo.cd_pessoa_responsavel == cd_responsavel)
                        select t;

                if (cd_aluno > 0)
                {
                    SGFWebContext dbContext = new SGFWebContext();
                    int cd_origem = Int32.Parse(dbContext.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                    sql = from t in sql
                          where t.TitulosRetornoCNAB.Any(x => x.Titulo.id_origem_titulo == cd_origem &&
                                     db.Aluno.Where(a => a.cd_pessoa_aluno == x.Titulo.cd_pessoa_titulo && a.cd_aluno == cd_aluno).Any()) 
                           select t;
                }

                sql = sorter.Sort(sql);
                var retorno = (from c in sql
                               select new
                               {
                                   cd_retorno_cnab = c.cd_retorno_cnab,
                                   cd_local_movto = c.cd_local_movto,
                                   cd_usuario = c.cd_usuario,
                                   dt_cadastro_cnab = c.dt_cadastro_cnab,
                                   id_status_cnab = c.id_status_cnab,
                                   no_arquivo_retorno = c.no_arquivo_retorno,
                                   usuario = c.SysUsuario.no_login,
                                   nm_banco = c.LocalMovto.nm_banco,
                                   no_local_movto = c.LocalMovto.no_local_movto,
                                   nm_agencia = c.LocalMovto.nm_agencia,
                                   nm_conta_corrente = c.LocalMovto.nm_conta_corrente,
                                   nm_tipo_local = c.LocalMovto.nm_tipo_local,
                                   cd_carteira_cnab = c.LocalMovto.CarteiraCnab.cd_carteira_cnab,
                                   no_carteira_cnab = c.LocalMovto.CarteiraCnab.no_carteira
                               }).ToList().Select(x => new RetornoCNAB
                               {
                                   cd_retorno_cnab = x.cd_retorno_cnab,
                                   cd_local_movto = x.cd_local_movto,
                                   cd_carteira_retorno_cnab = x.cd_carteira_cnab,
                                   cd_usuario = x.cd_usuario,
                                   no_arquivo_retorno = x.no_arquivo_retorno,
                                   dt_cadastro_cnab = x.dt_cadastro_cnab,
                                   id_status_cnab = x.id_status_cnab,
                                   usuarioRetornoCNAB = x.usuario,
                                   LocalMovto = new LocalMovto
                                   {
                                       nm_banco = x.nm_banco,
                                       no_local_movto = x.no_local_movto,
                                       nm_agencia = x.nm_agencia,
                                       nm_conta_corrente = x.nm_conta_corrente,
                                       nm_tipo_local = x.nm_tipo_local,
                                       CarteiraCnab = new CarteiraCnab { 
                                           cd_carteira_cnab = x.cd_carteira_cnab,
                                           no_carteira = x.no_carteira_cnab
                                       }
                                   },
                               });

                int limite = retorno.Count();
                parametros.ajustaParametrosPesquisa(limite);
                retorno = retorno.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<UsuarioWebSGF> getUsuariosRetCNAB(int cd_empresa)
        {
            try
            {
                IEnumerable<UsuarioWebSGF> sql = new List<UsuarioWebSGF>();
                sql = (from user in db.UsuarioWebSGF
                       where user.RetornosCNAB.Where(c => c.LocalMovto.cd_pessoa_empresa == cd_empresa).Any()
                       select new
                       {
                           cd_usuario = user.cd_usuario,
                           no_login = user.no_login
                       }).ToList().Select(x => new UsuarioWebSGF
                       {
                           cd_usuario = x.cd_usuario,
                           no_login = x.no_login
                       });

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<CarteiraCnab> getCarteirasRetCNAB(int cd_empresa)
        {
            try
            {
                var sql = (from l in db.LocalMovto
                          where l.cd_pessoa_empresa == cd_empresa && l.cd_carteira_cnab != null &&
                                l.RetornosCNAB.Any()
                          select new
                          {
                              cd_carteira_cnab = l.CarteiraCnab.cd_carteira_cnab,
                              no_carteira = l.CarteiraCnab.no_carteira,
                              cd_local_movto = l.cd_local_movto,
                              no_local_movto = l.no_local_movto,
                              nm_agencia = l.nm_agencia,
                              nm_conta_corrente = l.nm_conta_corrente,
                              nm_digito_conta_corrente = l.nm_digito_conta_corrente != null ? l.nm_digito_conta_corrente : null,
                              nm_tipo_local = l.nm_tipo_local
                          }).ToList().Select(x => new CarteiraCnab {
                              cd_carteira_cnab = x.cd_carteira_cnab,
                              no_carteira = x.no_carteira,
                              cd_localMvto =  x.cd_local_movto,
                              localMovtoCateiraCnab = new LocalMovto{
                                    cd_local_movto = x.cd_local_movto,
                                    no_local_movto = x.no_local_movto,
                                    nm_agencia = x.nm_agencia,
                                    nm_conta_corrente = x.nm_conta_corrente,
                                    nm_digito_conta_corrente = x.nm_digito_conta_corrente
                              }
                          });
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public RetornoCNAB getRetornoCnabFull(int cd_retorno, int cd_escola) {
            try {
                var sql = (from cb in db.RetornoCNAB
                           where cb.LocalMovto.cd_pessoa_empresa == cd_escola && cb.cd_retorno_cnab == cd_retorno
                           select cb).FirstOrDefault();
                return sql;
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

        public RetornoCNAB getRetornoCnabEditView(int cd_retorno, int cdEscola)
        {
            try
            {
                SGFWebContext dbContext = new SGFWebContext();
                int cd_origem = Int32.Parse(dbContext.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                var sql = (from cb in db.RetornoCNAB
                           where cb.LocalMovto.cd_pessoa_empresa == cdEscola && cb.cd_retorno_cnab == cd_retorno
                           select cb).FirstOrDefault();
                if (sql.cd_retorno_cnab > 0)
                    sql.TitulosRetornoCNAB = (from tc in db.TituloRetornoCNAB
                                              where tc.RetornoCNAB.LocalMovto.cd_pessoa_empresa == cdEscola && tc.RetornoCNAB.cd_retorno_cnab == cd_retorno
                                       orderby tc.cd_titulo ascending
                                       select new
                                       {
                                           cd_titulo_retorno_cnab = tc.cd_titulo_retorno_cnab,
                                           cd_retorno_cnab = tc.cd_retorno_cnab,
                                           cd_local_movto = tc.Titulo != null ? tc.Titulo.cd_local_movto : 0,
                                           cd_pessoa_responsavel = tc.Titulo != null ? tc.Titulo.cd_pessoa_responsavel : 0,
                                           cd_titulo = tc.cd_titulo,
                                           no_local_movto =  tc.Titulo != null ?  tc.Titulo.LocalMovto.no_local_movto : null,
                                           nm_agencia =  tc.Titulo != null ? tc.Titulo.LocalMovto.nm_agencia : null,
                                           nm_conta_corrente =  tc.Titulo != null ?  tc.Titulo.LocalMovto.nm_conta_corrente : null,
                                           nm_digito_conta_corrente =  tc.Titulo != null ? tc.Titulo.LocalMovto.nm_digito_conta_corrente != null ? tc.Titulo.LocalMovto.nm_digito_conta_corrente : null : "",
                                           nm_tipo_local =  tc.Titulo != null ?  tc.Titulo.LocalMovto.nm_tipo_local : 0,
                                           nomeResponsavel =  tc.Titulo != null ?  tc.Titulo.PessoaResponsavel.no_pessoa : null,
                                           nomePessoa = tc.Titulo != null ? tc.Titulo.Pessoa.no_pessoa : null,
                                           vl_titulo =  tc.Titulo != null ?  tc.Titulo.vl_titulo : 0,
                                           dt_vencimento = tc.Titulo != null ? tc.Titulo.dt_vcto_titulo : new DateTime(),
                                           dt_emissao = tc.Titulo != null ? tc.Titulo.dt_emissao_titulo : new DateTime(),
                                           id_status_cnab =  tc.Titulo != null ?  tc.Titulo.id_status_cnab : 0,
                                           dc_nosso_numero =  tc.dc_nosso_numero,
                                           cd_aluno = tc.Titulo != null ?   tc.Titulo.id_origem_titulo == cd_origem ? db.Aluno.Where(a => a.cd_pessoa_aluno == tc.Titulo.cd_pessoa_titulo).FirstOrDefault().cd_aluno : 0 : 0,
                                           nro_contrato = tc.Titulo != null ?   tc.Titulo.id_origem_titulo == cd_origem ? db.Contrato.Where(c => c.cd_contrato == (int)tc.Titulo.cd_origem_titulo).FirstOrDefault().nm_contrato : 0 : 0,
                                           dt_baixa_retorno = tc.dt_baixa_retorno,
                                           dt_banco_retorno = tc.dt_banco_retorno,
                                           id_tipo_retorno = tc.id_tipo_retorno,
                                           vl_baixa_retorno = tc.vl_baixa_retorno,
                                           vl_juros_retorno = tc.vl_juros_retorno,
                                           vl_multa_retorno = tc.vl_multa_retorno,
                                           vl_desconto_titulo = tc.vl_desconto_titulo,
                                           tx_mensagem_retorno = tc.tx_mensagem_retorno,
                                           cd_tran_finan = (tc.cd_tran_finan != null) ? tc.cd_tran_finan : (tc.Titulo.BaixaTitulo.Any()) ? tc.Titulo.BaixaTitulo.FirstOrDefault().cd_tran_finan : (int?)null,
                                           dc_nosso_numero_titulo = tc.Titulo.dc_nosso_numero,
                                       }).ToList().Select(x => new TituloRetornoCNAB
                                       {
                                           cd_titulo_retorno_cnab = x.cd_titulo_retorno_cnab,
                                           cd_retorno_cnab = x.cd_retorno_cnab,
                                           //cd_turma = x.cd_turma,
                                           cd_pessoa_responsavel = x.cd_pessoa_responsavel,
                                           nomeResponsavel = x.nomeResponsavel,
                                           nomePessoaTitulo = x.nomePessoa,
                                          // cd_produto = x.cd_produto,
                                           cd_aluno = x.cd_aluno,
                                           nro_contrato = x.nro_contrato,
                                           cd_titulo = x.cd_titulo,
                                           dc_nosso_numero = x.dc_nosso_numero,
                                           id_status_titulo_cnab = (byte)x.id_status_cnab,
                                           dt_baixa_retorno = x.dt_baixa_retorno,
                                           dt_banco_retorno = x.dt_banco_retorno,
                                           id_tipo_retorno = x.id_tipo_retorno,
                                           vl_baixa_retorno = x.vl_baixa_retorno,
                                           vl_juros_retorno = x.vl_juros_retorno,
                                           vl_multa_retorno = x.vl_multa_retorno,
                                           vl_desconto_titulo = x.vl_desconto_titulo,
                                           tx_mensagem_retorno = x.tx_mensagem_retorno,
                                           cd_tran_finan = x.cd_tran_finan,
                                           Titulo = x.cd_titulo > 0? new Titulo
                                           {
                                               vl_titulo = x.vl_titulo,
                                               dt_vcto_titulo = x.dt_vencimento,
                                               dt_emissao_titulo = x.dt_emissao,
                                               cd_titulo = x.cd_titulo.Value,
                                               id_status_cnab = (byte)x.id_status_cnab,
                                               dc_nosso_numero = x.dc_nosso_numero_titulo,
                                               LocalMovto = new LocalMovto
                                               {
                                                   cd_local_movto = x.cd_local_movto,
                                                   no_local_movto = x.no_local_movto,
                                                   nm_agencia = x.nm_agencia,
                                                   nm_conta_corrente = x.nm_conta_corrente,
                                                   nm_digito_conta_corrente = x.nm_digito_conta_corrente,
                                                   nm_tipo_local = (byte)x.nm_tipo_local
                                               }
                                           }: null
                                       }).ToList();

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public RetornoCNAB getRetornoCnabReturnGrade(int cd_retorno, int cd_empresa)
        {
            try
            {
                var sql = (from c in db.RetornoCNAB
                           where c.LocalMovto.cd_pessoa_empresa == cd_empresa && c.cd_retorno_cnab == cd_retorno
                           select new
                           {
                               cd_retorno_cnab = c.cd_retorno_cnab,
                               cd_local_movto = c.cd_local_movto,
                               cd_usuario = c.cd_usuario,
                               dt_cadastro_cnab = c.dt_cadastro_cnab,
                               id_status_cnab = c.id_status_cnab,
                               no_arquivo_retorno = c.no_arquivo_retorno,
                               usuario = c.SysUsuario.no_login,
                               nm_banco = c.LocalMovto.nm_banco,
                               no_local_movto = c.LocalMovto.no_local_movto,
                               nm_agencia = c.LocalMovto.nm_agencia,
                               nm_conta_corrente = c.LocalMovto.nm_conta_corrente,
                               nm_tipo_local = c.LocalMovto.nm_tipo_local,
                               cd_carteira_cnab = c.LocalMovto.CarteiraCnab.cd_carteira_cnab,
                               no_carteira_cnab = c.LocalMovto.CarteiraCnab.no_carteira
                           }).ToList().Select(x => new RetornoCNAB
                           {
                               cd_retorno_cnab = x.cd_retorno_cnab,
                               cd_local_movto = x.cd_local_movto,
                               cd_carteira_retorno_cnab = x.cd_carteira_cnab,
                               cd_usuario = x.cd_usuario,
                               no_arquivo_retorno = x.no_arquivo_retorno,
                               dt_cadastro_cnab = x.dt_cadastro_cnab,
                               id_status_cnab = x.id_status_cnab,
                               usuarioRetornoCNAB = x.usuario,
                               LocalMovto = new LocalMovto
                               {
                                   nm_banco = x.nm_banco,
                                   no_local_movto = x.no_local_movto,
                                   nm_agencia = x.nm_agencia,
                                   nm_conta_corrente = x.nm_conta_corrente,
                                   nm_tipo_local = x.nm_tipo_local,
                                   CarteiraCnab = new CarteiraCnab
                                   {
                                       cd_carteira_cnab = x.cd_carteira_cnab,
                                       no_carteira = x.no_carteira_cnab
                                   }
                               },
                           }).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<RetornoCNAB> getRetornsoCNAB(int[] cdRetornosCnab, int cd_empresa)
        {
            try
            {
                var sql = from c in db.RetornoCNAB
                          where cdRetornosCnab.Contains(c.cd_retorno_cnab) && c.LocalMovto.cd_pessoa_empresa == cd_empresa
                          select c;

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool verificaStatusRetorno(int cd_retorno_cnab, int cd_escola, int status) {
            try {
                var sql = (from c in db.RetornoCNAB
                           where c.cd_retorno_cnab == cd_retorno_cnab && c.LocalMovto.cd_pessoa_empresa == cd_escola
                                && c.id_status_cnab == status
                          select c).Any();

                return sql;
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

        public int buscarTipoCNAB(int cd_retorno_cnab, int cd_pessoa_empresa) {
            try {
                var sql = (from c in db.RetornoCNAB
                           where c.cd_retorno_cnab == cd_retorno_cnab && c.LocalMovto.cd_pessoa_empresa == cd_pessoa_empresa
                           select c.LocalMovto.CarteiraCnab.nm_colunas).FirstOrDefault();

                return sql;
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<RetornoCNAB> getRetornosCnabs(int[] cdCnabs, int cd_empresa) {
            try {
                var sql = from c in db.RetornoCNAB
                          where cdCnabs.Contains(c.cd_retorno_cnab) && c.LocalMovto.cd_pessoa_empresa == cd_empresa
                          select c;

                return sql;
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<TituloRetornoCNAB> getTitulosRetornoCNAB(int[] cdCnabs, int cd_empresa)
        {
            try
            {
                var sql = from c in db.TituloRetornoCNAB
                          where cdCnabs.Contains(c.cd_retorno_cnab) && c.RetornoCNAB.LocalMovto.cd_pessoa_empresa == cd_empresa
                          select c;

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}

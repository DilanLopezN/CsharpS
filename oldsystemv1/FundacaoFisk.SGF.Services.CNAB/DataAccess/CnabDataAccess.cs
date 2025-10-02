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
    public class CnabDataAccess : GenericRepository<Cnab>, ICnabDataAccess
    {


        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public IEnumerable<Cnab> searchCnab(SearchParameters parametros, int cd_carteira, int cd_usuario, byte tipo_cnab, int status, DateTime? dtInicial,
                                                 DateTime? dtFinal, bool emissao, bool vencimento, string nossoNumero, int? nro_contrato, int cd_empresa,
                                                bool icnab, bool iboleto, int cd_responsavel, int cd_aluno)
        {
            try
            {
                IEntitySorter<Cnab> sorter = EntitySorter<Cnab>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<Cnab> sql;
                sql = from t in db.Cnab.AsNoTracking()
                      where t.LocalMovimento.cd_pessoa_empresa == cd_empresa
                      select t;

                if (cd_carteira > 0)
                    sql = from t in sql
                          where t.cd_local_movto == cd_carteira
                          select t;

                if (cd_usuario > 0)
                    sql = from t in sql
                          where t.cd_usuario == cd_usuario
                          select t;

                if (tipo_cnab > 0)
                    sql = from t in sql
                          where t.id_tipo_cnab == tipo_cnab
                          select t;

                if (status > 0)
                    sql = from t in sql
                          where t.id_status_cnab == status
                          select t;

                if (!string.IsNullOrEmpty(nossoNumero))
                {
                    sql = from t in sql
                          where t.TitulosCnab.Any(x => x.dc_nosso_numero_titulo == nossoNumero)
                          select t;
                }

                if (nro_contrato != null && nro_contrato > 0)
                {
                    sql = from t in sql
                          where db.Contrato.Any(c => c.cd_pessoa_escola == cd_empresa &&
                              c.cd_contrato == t.cd_contrato && 
                              c.nm_contrato == nro_contrato)
                          select t;
                }

                if (emissao)
                {
                    if (dtInicial.HasValue)
                    {
                        var dtInicialP = dtInicial.Value;
                        sql = from t in sql
                              where DbFunctions.TruncateTime(t.dt_emissao_cnab) >= dtInicialP.Date
                              select t;
                    }

                    if (dtFinal.HasValue)
                    {
                        var dtFinalP = dtFinal.Value;
                        sql = from t in sql
                              where DbFunctions.TruncateTime(t.dt_emissao_cnab) <= dtFinalP.Date
                              select t;
                    }
                }

                if (vencimento)
                {
                    if (dtInicial.HasValue)
                    {
                        var dtInicialP = dtInicial.Value;
                        sql = from t in sql
                              where DbFunctions.TruncateTime(t.dt_inicial_vencimento) >= dtInicialP.Date
                              select t;
                    }

                    if (dtFinal.HasValue)
                    {
                        var dtFinalP = dtFinal.Value;
                        sql = from t in sql
                              where DbFunctions.TruncateTime(t.dt_final_vencimento) <= dtFinalP.Date
                              select t;
                    }
                }

                if (icnab && !iboleto)
                    sql = from t in sql
                          where t.cd_contrato == null
                          select t;

                if (!icnab && iboleto)
                    sql = from t in sql
                          where t.cd_contrato != null
                          select t;

                if (!icnab && !iboleto)
                    sql = from t in sql
                          where t.cd_contrato == 0
                          select t;

                if (cd_responsavel > 0)
                    sql = from t in sql
                        where t.TitulosCnab.Any(x => x.Titulo.cd_pessoa_responsavel == cd_responsavel)
                        select t;

                if (cd_aluno > 0)
                {	SGFWebContext dbContext = new SGFWebContext();			
                    int cd_origem = Int32.Parse(dbContext.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                    sql = from t in sql
                        where t.TitulosCnab.Any(x=> //x.Titulo.id_origem_titulo == cd_origem && 
                                ((db.Aluno.Where(a => a.cd_pessoa_aluno == x.Titulo.cd_pessoa_titulo && a.cd_aluno == cd_aluno).Any()) ||
                                db.Movimento.Where(m => m.cd_movimento == x.Titulo.cd_origem_titulo && m.cd_aluno == cd_aluno).Any()))
                    select t;
                }
                    

                sql = sorter.Sort(sql);
                var retorno = (from c in sql
                               select new
                               {
                                   cd_cnab = c.cd_cnab,
                                   cd_local_movto = c.cd_local_movto,
                                   id_tipo_cnab = c.id_tipo_cnab,
                                   cd_usuario = c.cd_usuario,
                                   dt_inicial_vencimento = c.dt_inicial_vencimento,
                                   dt_final_vencimento = c.dt_final_vencimento,
                                   dt_emissao_cnab = c.dt_emissao_cnab,
                                   dh_cadastro_cnab = c.dh_cadastro_cnab,
                                   id_status_cnab = c.id_status_cnab,
                                   vl_total_cnab = c.vl_total_cnab,
                                   usuario = c.Usuario.no_login,
                                   CarteiraCnab = c.LocalMovimento.CarteiraCnab,
                                   nm_banco = c.LocalMovimento.nm_banco,
                                   no_local_movto = c.LocalMovimento.no_local_movto,
                                   nm_agencia = c.LocalMovimento.nm_agencia,
                                   nm_conta_corrente = c.LocalMovimento.nm_conta_corrente,
                                   nm_digito_conta_corrente = c.LocalMovimento.nm_digito_conta_corrente != null ? c.LocalMovimento.nm_digito_conta_corrente : null,
                                   nm_tipo_local = c.LocalMovimento.nm_tipo_local,
                                   cd_carteira_cnab = c.LocalMovimento.CarteiraCnab.cd_carteira_cnab,
                                   no_carteira_cnab = c.LocalMovimento.CarteiraCnab.no_carteira,
                                   id_impressao = c.LocalMovimento.CarteiraCnab.id_impressao,
                                   cd_contrato = c.cd_contrato,
                                   nro_contrato = db.Contrato.Where(cc => cc.cd_contrato == c.cd_contrato).FirstOrDefault().nm_contrato,
                               }).ToList().Select(x => new Cnab
                               {
                                   cd_cnab = x.cd_cnab,
                                   cd_local_movto = x.cd_local_movto,
                                   cd_carteira_cnab = x.cd_carteira_cnab,
                                   id_tipo_cnab = x.id_tipo_cnab,
                                   cd_usuario = x.cd_usuario,
                                   dt_inicial_vencimento = x.dt_inicial_vencimento,
                                   dt_final_vencimento = x.dt_final_vencimento,
                                   dt_emissao_cnab = x.dt_emissao_cnab,
                                   dh_cadastro_cnab = x.dh_cadastro_cnab,
                                   id_status_cnab = x.id_status_cnab,
                                   vl_total_cnab = x.vl_total_cnab,
                                   id_impressao_carteira_cnab = x.id_impressao,
                                   usuarioCnab = x.usuario,
                                   LocalMovimento = new LocalMovto
                                   {
                                       no_local_movto = x.no_local_movto,
                                       nm_agencia = x.nm_agencia,
                                       nm_conta_corrente = x.nm_conta_corrente,
                                       nm_tipo_local = x.nm_tipo_local,
                                       nm_digito_conta_corrente = x.nm_digito_conta_corrente,
                                       CarteiraCnab = new CarteiraCnab
                                       {
                                           cd_carteira_cnab = x.cd_carteira_cnab,
                                           no_carteira = x.no_carteira_cnab
                                       }
                                   },
                                   cd_contrato = x.cd_contrato,
                                   nro_contrato = x.nro_contrato
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

        public Cnab getGerarRemessa(int cd_escola, int cd_cnab) {
            try {
                var sql = (from c in db.Cnab
                           where cd_cnab == c.cd_cnab
                                 && c.LocalMovimento.cd_pessoa_empresa == cd_escola
                           select new {
                               no_arquivo_remessa = c.no_arquivo_remessa,
                                cd_contrato = c.cd_contrato,
                               nro_contrato = db.Contrato.Where(cc => cc.cd_contrato == c.cd_contrato).FirstOrDefault().nm_contrato,
                                c.LocalMovimento.CarteiraCnab.nm_colunas,
                               nm_banco = c.LocalMovimento.Banco.nm_banco,
                               no_banco = c.LocalMovimento.Banco.no_banco,
                               //no_pessoa_banco = c.LocalMovimento.PessoaSGFBanco.no_pessoa,
                               no_pessoa_banco = c.LocalMovimento.Empresa.no_pessoa,
                               nm_agencia = c.LocalMovimento.nm_agencia,
                               nm_conta_corrente = c.LocalMovimento.nm_conta_corrente,
                               nm_digito_conta_corrente = c.LocalMovimento.nm_digito_conta_corrente,
                               nm_cpf_cgc = c.LocalMovimento.Empresa.nm_natureza_pessoa == 1 ?
                                db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == c.LocalMovimento.Empresa.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_cpf :
                                    db.PessoaSGF.OfType<PessoaJuridicaSGF>().Where(pj => pj.cd_pessoa == c.LocalMovimento.Empresa.cd_pessoa).FirstOrDefault<PessoaJuridicaSGF>().dc_num_cgc,
                               /*nm_cpf_cgc = c.LocalMovimento.PessoaSGFBanco.nm_natureza_pessoa == 1 ?
                                    db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == c.LocalMovimento.PessoaSGFBanco.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_cpf :
                                        db.PessoaSGF.OfType<PessoaJuridicaSGF>().Where(pj => pj.cd_pessoa == c.LocalMovimento.PessoaSGFBanco.cd_pessoa).FirstOrDefault<PessoaJuridicaSGF>().dc_num_cgc,*/
                               nm_carteira = c.LocalMovimento.CarteiraCnab.nm_carteira,
                               dc_num_cliente_banco = c.LocalMovimento.dc_num_cliente_banco,
                               nm_sequencia = c.nm_sequencia_remessa,
                               nm_transmissao = c.LocalMovimento.nm_transmissao,
                               c.LocalMovimento.nm_digito_agencia
                           }).ToList().Select(x => new Cnab {
                               no_arquivo_remessa = x.no_arquivo_remessa,
                               nm_sequencia_remessa = x.nm_sequencia,
                                cd_contrato = x.cd_contrato,
                               nro_contrato = x.nro_contrato,
                               LocalMovimento = new LocalMovto()
                               {
                                   CarteiraCnab = new CarteiraCnab() {
                                       nm_colunas = x.nm_colunas,
                                       nm_carteira = x.nm_carteira
                                   },
                                   Banco = new Banco() {
                                       nm_banco = x.nm_banco,
                                       no_banco = x.no_banco
                                   },
                                   PessoaSGFBanco = new PessoaSGF() {
                                       nm_cpf_cgc = x.nm_cpf_cgc,
                                       no_pessoa = x.no_pessoa_banco
                                   },
                                   nm_agencia = x.nm_agencia,
                                   nm_conta_corrente = x.nm_conta_corrente,
                                   nm_digito_conta_corrente = x.nm_digito_conta_corrente,
                                   dc_num_cliente_banco = x.dc_num_cliente_banco,
                                   nm_transmissao = x.nm_transmissao,
                                   nm_digito_agencia = x.nm_digito_agencia
                               }
                           }).FirstOrDefault();

                return sql;
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<UsuarioWebSGF> getUsuariosCnab(int cd_empresa, bool adm, int cdUsuario)
        {
            try
            {
                IEnumerable<UsuarioWebSGF> sql = new List<UsuarioWebSGF>();
                if (adm)
                    sql = (from user in db.UsuarioWebSGF
                           where user.CNABs.Where(c => c.LocalMovimento.cd_pessoa_empresa == cd_empresa).Any()
                           select new
                           {
                               cd_usuario = user.cd_usuario,
                               no_login = user.no_login
                           }).ToList().Select(x => new UsuarioWebSGF
                           {
                               cd_usuario = x.cd_usuario,
                               no_login = x.no_login
                           });
                else
                    sql = (from user in db.UsuarioWebSGF
                           where user.cd_usuario == cdUsuario
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

        public IEnumerable<CarteiraCnab> getCarteirasCnab(int cd_empresa)
        {
            try
            {
                var sql = (from l in db.LocalMovto
                          where l.cd_pessoa_empresa == cd_empresa && l.cd_carteira_cnab != null &&
                                l.Cnabs.Any()
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

        public bool verificarGerouCnab(int cd_escola, Int32[] cd_cnab, Int32[] tipos_cnabs, byte status_cnab, bool is_titulo) {
            try {
                bool sql;
                
                if(!is_titulo)
                    sql = (from c in db.Cnab
                           where cd_cnab.Contains(c.cd_cnab)
                                 && c.id_status_cnab == status_cnab
                                 && tipos_cnabs.Contains(c.id_tipo_cnab)
                           select c).Any();
                else
                    sql = (from c in db.TituloCnab
                           where cd_cnab.Contains(c.cd_titulo_cnab)
                                 && c.Cnab.id_status_cnab == status_cnab
                                 && tipos_cnabs.Contains(c.Cnab.id_tipo_cnab)
                           select c).Any();
                
                return sql;
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

        public Cnab getCnabFull(int cd_cnab, int cd_escola) {
            try {
                var sql = (from cb in db.Cnab
                           where cb.LocalMovimento.cd_pessoa_empresa == cd_escola && cb.cd_cnab == cd_cnab
                           select cb).FirstOrDefault();
                return sql;
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

        public int getQtdCnabGeradoDia(string banco, string codigoBeneficiario, int cd_escola)
        {
            try
            {
                
                DateTime now = DateTime.Now;
                var sql = (from cb in db.Cnab
                    where cb.LocalMovimento.cd_pessoa_empresa == cd_escola &&
                          cb.LocalMovimento.dc_num_cliente_banco == codigoBeneficiario &&
                           cb.LocalMovimento.nm_banco == banco && 
                          (DbFunctions.TruncateTime(cb.dh_cadastro_cnab) == DbFunctions.TruncateTime(now))
                    select cb);
                return sql.Count();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Cnab getCNABFullComTitulosCNAB(int cd_cnab, int cd_escola)
        {
            try
            {
                var sql = (from cb in db.Cnab.Include("TitulosCnab").Include("TitulosCnab.Titulo")
                           where cb.LocalMovimento.cd_pessoa_empresa == cd_escola && cb.cd_cnab == cd_cnab
                           select cb).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Cnab getCnabEditView(int cd_cnab, int cdEscola)
        {
            try
            {
                SGFWebContext dbContext = new SGFWebContext();
                int cd_origem = Int32.Parse(dbContext.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                var sql = (from cb in db.Cnab
                           where cb.LocalMovimento.cd_pessoa_empresa == cdEscola && cb.cd_cnab == cd_cnab
                           select cb).FirstOrDefault();
                if (sql != null && sql.cd_cnab > 0)
                {
                    sql.TitulosCnab = (from tc in db.TituloCnab
                                       where tc.Cnab.LocalMovimento.cd_pessoa_empresa == cdEscola && tc.Cnab.cd_cnab == cd_cnab
                                       orderby tc.cd_titulo ascending
                                       select new
                                       {
                                           cd_titulo_cnab = tc.cd_titulo_cnab,
                                           cd_local_movto = tc.Titulo.cd_local_movto,
                                           cd_turma = tc.cd_turma_titulo,
                                           cd_turma_ppt = tc.Turma.TurmaPai != null ? tc.Turma.TurmaPai.cd_turma : 0,
                                           cd_pessoa_titulo = tc.Titulo.cd_pessoa_titulo,
                                           cd_pessoa_responsavel = tc.Titulo.cd_pessoa_responsavel,
                                           cd_titulo = tc.cd_titulo,
                                           no_local_movto = tc.Titulo.LocalMovto.no_local_movto,
                                           nm_agencia = tc.Titulo.LocalMovto.nm_agencia,
                                           nm_conta_corrente = tc.Titulo.LocalMovto.nm_conta_corrente,
                                           nm_digito_conta_corrente = tc.Titulo.LocalMovto.nm_digito_conta_corrente != null ? tc.Titulo.LocalMovto.nm_digito_conta_corrente : null,
                                           nm_tipo_local = tc.Titulo.LocalMovto.nm_tipo_local,
                                           nomeResponsavel = tc.Titulo.PessoaResponsavel.no_pessoa,
                                           nomePessoaTitulo = tc.Titulo.Pessoa.no_pessoa,
                                           no_turma = tc.Turma.TurmaPai != null ? tc.Turma.TurmaPai.no_turma : tc.Turma.no_turma,
                                           vl_titulo = tc.Titulo.vl_titulo,
                                           dt_vencimento = tc.Titulo.dt_vcto_titulo,
                                           dt_emissao = tc.Titulo.dt_emissao_titulo,
                                           id_status_cnab = tc.Titulo.id_status_cnab,
                                           dc_nosso_numero = tc.Titulo.dc_nosso_numero,
                                           //cd_turma_titulo = tc.cd_turma_titulo,
                                           cd_produto = tc.Titulo.id_origem_titulo == cd_origem && tc.cd_turma_titulo != null ? db.Turma.Where(t => t.cd_turma == tc.cd_turma_titulo).FirstOrDefault().cd_turma : 0,
                                           cd_produto_escola = tc.Titulo.id_origem_titulo == cd_origem && tc.cd_turma_titulo != null ? db.Turma.Where(t => t.cd_turma == tc.cd_turma_titulo).FirstOrDefault().cd_produto : 0,
                                           cd_aluno = tc.Titulo.id_origem_titulo == cd_origem ? db.Aluno.Where(a => a.cd_pessoa_aluno == tc.Titulo.cd_pessoa_titulo).FirstOrDefault().cd_aluno :
                                                                                                db.Movimento.Where(m => m.cd_movimento == tc.Titulo.cd_origem_titulo).FirstOrDefault().cd_aluno,
                                           nro_contrato = tc.Titulo.id_origem_titulo == cd_origem ? db.Contrato.Where(c => c.cd_contrato == (int)tc.Titulo.cd_origem_titulo).FirstOrDefault().nm_contrato : 0,
                                           vl_saldo_titulo = tc.Titulo.vl_saldo_titulo,
                                           cd_origem_titulo = tc.Titulo.cd_origem_titulo,
                                           nm_titulo = tc.Titulo.nm_titulo,
                                           nm_parcela_titulo = tc.Titulo.nm_parcela_titulo,
                                           id_natureza_titulo = tc.Titulo.id_natureza_titulo,
                                           id_origem_titulo = tc.Titulo.id_origem_titulo,
                                           tx_mensagem_cnab = tc.tx_mensagem_cnab,
                                           id_status_cnab_titulo = tc.id_status_cnab_titulo,
                                           vl_desconto_bolsa = tc.Titulo.vl_liquidacao_titulo > 0 && tc.Titulo.BaixaTitulo.Any(x => x.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA) ?
                                           tc.Titulo.BaixaTitulo.Where(x => x.cd_titulo == tc.cd_titulo && x.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA).FirstOrDefault().vl_baixa_saldo_titulo : 0
                                           //descontosTituloCNAB = tc.DescontoTituloCNAB
                                       }).ToList().Select(x => new TituloCnab
                                       {
                                           cd_titulo_cnab = x.cd_titulo_cnab,
                                           cd_local_movto_titulo = x.cd_local_movto,
                                           cd_turma_titulo = x.cd_turma,
                                           cd_turma_PPT = x.cd_turma_ppt,
                                           cd_pessoa_titulo = x.cd_pessoa_titulo,
                                           cd_pessoa_responsavel = x.cd_pessoa_responsavel,
                                           nomePessoaTitulo = x.nomePessoaTitulo,
                                           nomeResponsavel = x.nomeResponsavel,
                                           no_turma_titulo = x.no_turma,
                                           cd_produto = x.cd_produto,
                                           cd_produto_escola = x.cd_produto_escola,
                                           cd_aluno = x.cd_aluno,
                                           nro_contrato = x.nro_contrato,
                                           id_status_cnab_titulo = x.id_status_cnab_titulo,
                                           cd_titulo = x.cd_titulo,
                                           dc_nosso_numero = x.dc_nosso_numero,
                                           tx_mensagem_cnab = x.tx_mensagem_cnab,
                                           id_status_titulo_cnab = x.id_status_cnab,
                                           Titulo = new Titulo
                                           {
                                               vl_titulo = x.vl_titulo,
                                               vl_saldo_titulo = x.vl_desconto_bolsa > 0 ? x.vl_titulo - x.vl_desconto_bolsa : x.vl_titulo,
                                               dt_vcto_titulo = x.dt_vencimento,
                                               dt_emissao_titulo = x.dt_emissao,
                                               cd_titulo = x.cd_titulo,
                                               cd_origem_titulo = x.cd_origem_titulo,
                                               nm_titulo = x.nm_titulo,
                                               nm_parcela_titulo = x.nm_parcela_titulo,
                                               id_natureza_titulo = x.id_natureza_titulo,
                                               id_origem_titulo = x.id_origem_titulo,
                                               id_status_cnab = x.id_status_cnab,
                                               LocalMovto = new LocalMovto
                                                        {
                                                            cd_local_movto = x.cd_local_movto,
                                                            no_local_movto = x.no_local_movto,
                                                            nm_agencia = x.nm_agencia,
                                                            nm_conta_corrente = x.nm_conta_corrente,
                                                            nm_digito_conta_corrente = x.nm_digito_conta_corrente,
                                                            nm_tipo_local = x.nm_tipo_local
                                                        }
                                           }//,
                                           //DescontoTituloCNAB = x.descontosTituloCNAB
                                       }).OrderBy(x => x.no_turma_titulo).ThenBy(x => x.nomePessoaTitulo).ToList();

                    if (sql.cd_contrato != null && sql.cd_contrato > 0) {
                        var nm_contrato = db.Contrato.Where(c => c.cd_contrato == sql.cd_contrato).Select(x => x.nm_contrato).FirstOrDefault();
                        if (nm_contrato != null)
                            sql.nro_contrato = nm_contrato;
                    }
                }

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Cnab getCnabReturnGrade(int cd_cnab, int cd_empresa)
        {
            try
            {
                var sql = (from c in db.Cnab
                           where c.LocalMovimento.cd_pessoa_empresa == cd_empresa && c.cd_cnab == cd_cnab
                           select new
                           {
                               cd_cnab = c.cd_cnab,
                               cd_local_movto = c.cd_local_movto,
                               id_tipo_cnab = c.id_tipo_cnab,
                               cd_usuario = c.cd_usuario,
                               dt_inicial_vencimento = c.dt_inicial_vencimento,
                               dt_final_vencimento = c.dt_final_vencimento,
                               dt_emissao_cnab = c.dt_emissao_cnab,
                               dh_cadastro_cnab = c.dh_cadastro_cnab,
                               id_status_cnab = c.id_status_cnab,
                               vl_total_cnab = c.vl_total_cnab,
                               usuario = c.Usuario.no_login,
                               CarteiraCnab = c.LocalMovimento.CarteiraCnab,
                               nm_banco = c.LocalMovimento.nm_banco,
                               no_local_movto = c.LocalMovimento.no_local_movto,
                               nm_agencia = c.LocalMovimento.nm_agencia,
                               nm_conta_corrente = c.LocalMovimento.nm_conta_corrente,
                               nm_tipo_local = c.LocalMovimento.nm_tipo_local,
                               cd_carteira_cnab = c.LocalMovimento.CarteiraCnab.cd_carteira_cnab,
                               no_carteira_cnab = c.LocalMovimento.CarteiraCnab.no_carteira,
                               id_impressao = c.LocalMovimento.CarteiraCnab.id_impressao,
                               cd_contrato = c.cd_contrato,
                                nro_contrato = db.Contrato.Where(cc => cc.cd_contrato == c.cd_contrato).FirstOrDefault().nm_contrato,
                           }).ToList().Select(x => new Cnab
                           {
                               cd_cnab = x.cd_cnab,
                               cd_local_movto = x.cd_local_movto,
                               id_tipo_cnab = x.id_tipo_cnab,
                               cd_usuario = x.cd_usuario,
                               dt_inicial_vencimento = x.dt_inicial_vencimento,
                               dt_final_vencimento = x.dt_final_vencimento,
                               dt_emissao_cnab = x.dt_emissao_cnab,
                               dh_cadastro_cnab = x.dh_cadastro_cnab,
                               id_status_cnab = x.id_status_cnab,
                               vl_total_cnab = x.vl_total_cnab,
                               id_impressao_carteira_cnab = x.id_impressao,
                               usuarioCnab = x.usuario,
                               cd_carteira_cnab = x.cd_carteira_cnab,
                               cd_contrato = x.cd_contrato,
                               nro_contrato = x.nro_contrato,
                               LocalMovimento = new LocalMovto
                               {
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

        public IEnumerable<Cnab> getCnabs(int[] cdCnabs, int cd_empresa)
        {
            try
            {
                var sql = from c in db.Cnab
                          where cdCnabs.Contains(c.cd_cnab) && c.LocalMovimento.cd_pessoa_empresa == cd_empresa
                          select c;

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool isResponsavelCNAB(Int32[] cd_cnab)
        {
            try {
                bool retorno = (from c in db.Cnab
                      where cd_cnab.Contains(c.cd_cnab)
                      select c.id_responsavel_cnab).FirstOrDefault();
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool isResponsavelTitulosCNAB(Int32[] cd_titulos_cnab)
        {
            try
            {
                bool retorno = (from c in db.TituloCnab
                                where cd_titulos_cnab.Contains(c.cd_titulo_cnab)
                                select c.Cnab.id_responsavel_cnab).FirstOrDefault();
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}

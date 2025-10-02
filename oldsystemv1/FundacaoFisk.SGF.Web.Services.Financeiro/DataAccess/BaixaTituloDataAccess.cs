using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Financeiro.Model;
using FundacaoFisk.SGF.Utils;
using Componentes.GenericModel;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Financeiro.Business;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess
{
    public class BaixaTituloDataAccess : GenericRepository<BaixaTitulo>, IBaixaTituloDataAccess
    {
        public enum TipoConsultaBaixaEnum
        {
            HAS_BAIXA_TITULO_EDIT = 0,
            HAS_BAIXA_TITULO_TRANS_FINAN = 1,
            HAS_TODAS_BAIXAS_PARCIAL_EXETO_BAIXA_EXCLUIDA = 2,
            HAS_BAIXAS_PARCIAIS_TITULO = 3,
            HAS_BAIXA_TITULO_TRANS_FINAN_CHEQUE = 4,
        }


        public enum TipoValidacaoReciboAgrupadoEnum
        {
            ALUNO = 0,
            RESPONSAVEL = 1,
        }




        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }

        public IEnumerable<BaixaTitulo> getBaixaTituloByIdTitulo(int cd_titulo,int cd_pessoa_empresa)
        {
            try
            {
                var sql = (from bt in db.BaixaTitulo
                           where bt.cd_titulo == cd_titulo && bt.TransacaoFinanceira.cd_pessoa_empresa == cd_pessoa_empresa
                           select new 
                           {
                               cd_baixa_titulo = bt.cd_baixa_titulo,
                               dt_baixa_titulo = bt.dt_baixa_titulo,
                               vl_liquidacao_baixa = bt.vl_liquidacao_baixa,
                               vl_multa_baixa = bt.vl_multa_baixa,
                               vl_principal_baixa = bt.vl_principal_baixa,
                               vl_juros_baixa = bt.vl_juros_baixa,
                               vl_desconto_baixa = bt.vl_desconto_baixa,
                               no_banco_baixa = bt.TransacaoFinanceira.LocalMovimento.no_local_movto,
                               dc_tipo_liquidacao = bt.TransacaoFinanceira.TipoLiquidacao.dc_tipo_liquidacao,
                               cd_tran_finan = bt.cd_tran_finan,
                               cd_tipo_liquidacao = bt.TransacaoFinanceira.TipoLiquidacao.cd_tipo_liquidacao,
                               cd_local_movto = bt.TransacaoFinanceira.LocalMovimento != null ? bt.TransacaoFinanceira.LocalMovimento.cd_local_movto : 0,
                               id_origem_titulo = bt.Titulo.id_origem_titulo,
                               cd_titulo = bt.cd_titulo
                           }).ToList().Select(x => new BaixaTitulo
                           {
                               cd_baixa_titulo = x.cd_baixa_titulo,
                               dt_baixa_titulo = x.dt_baixa_titulo,
                               vl_liquidacao_baixa = x.vl_liquidacao_baixa,
                               vl_multa_baixa = x.vl_multa_baixa,
                               vl_principal_baixa = x.vl_principal_baixa,
                               vl_juros_baixa = x.vl_juros_baixa,
                               vl_desconto_baixa = x.vl_desconto_baixa,
                               no_banco_baixa = x.no_banco_baixa,
                               dc_tipo_liqui = x.dc_tipo_liquidacao,
                               cd_tran_finan = x.cd_tran_finan,
                               cd_local_movto = x.cd_local_movto,
                               cd_tipo_liquidacao = x.cd_tipo_liquidacao,
                               id_origem_titulo = x.id_origem_titulo,
                               cd_titulo = x.cd_titulo
                           });

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<BaixaEfetuadaChequeUI> getBaixasEfetuadasForBaixaAutomaticaCheque(SearchParameters parametros, BaixaAutomaticaUI automaticaChequeUi)
        {
            try
            {
                IEntitySorter<BaixaEfetuadaChequeUI> sorter = EntitySorter<BaixaEfetuadaChequeUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<BaixaTitulo> sql;

                SGFWebContext dbComp = new SGFWebContext();
                var ORIGEMMATRICULA = 22;
                var ORIGEMMOMIMENTO = 69;
                 sql = from b in db.BaixaTitulo
                        .Include("LocalMovto")
                        .Include("Titulo")
                          select b;

                 //if (automaticaChequeUi.dt_inicial != null)
                 //   sql = from b in sql
                 //         where b.dt_baixa_titulo >= automaticaChequeUi.dt_inicial
                 //         select b;

                 //if (automaticaChequeUi.dt_final != null)
                 //   sql = from b in sql
                 //         where b.dt_baixa_titulo <= automaticaChequeUi.dt_final
                 //         select b;

                 if (automaticaChequeUi.cd_local_movto > 0 && automaticaChequeUi.id_trocar_local == false)
                {
                    sql = from b in sql
                          where b.cd_local_movto == automaticaChequeUi.cd_local_movto
                          select b;
                }

                //T_TITULO t
                //inner join T_BAIXA_TITULO b   on t.cd_titulo = b.cd_titulo
                //inner join T_TRAN_FINAN tf  on b.cd_tran_finan = tf.cd_tran_finan
                //inner join T_BAIXA_AUTOMATICA tb on tb.cd_baixa_automatica = tf.cd_baixa_automatica
                //inner join T_LOCAL_MOVTO l on t.cd_local_movto = l.cd_local_movto
                //inner join T_PESSOA p on t.cd_pessoa_responsavel = p.cd_pessoa
                //left join t_cheque c on t.cd_origem_titulo = c.cd_contrato

               var retorno =  (from b in sql
                   join t in db.Titulo on b.cd_titulo equals t.cd_titulo
                   join tf in db.TransacaoFinanceira on b.cd_tran_finan equals tf.cd_tran_finan
                   join tb in db.BaixaAutomatica on tf.cd_baixa_automatica equals tb.cd_baixa_automatica
                   join l in db.LocalMovto on b.cd_local_movto equals l.cd_local_movto
                   join p in db.PessoaSGF on t.cd_pessoa_responsavel equals p.cd_pessoa 
                   join c in db.Cheque on t.cd_origem_titulo equals c.cd_contrato into leftj
                   from lfcheque in leftj.DefaultIfEmpty() //left join)
                        where tb.cd_baixa_automatica == automaticaChequeUi.cd_baixa_automatica &&
                              t.cd_pessoa_empresa == automaticaChequeUi.cd_escola &&
                              t.cd_tipo_financeiro == (int)TipoFinanceiro.TiposFinanceiro.CHEQUE &&
                              t.id_natureza_titulo == (int)Titulo.NaturezaTitulo.RECEBER &&
                              t.vl_titulo != t.vl_saldo_titulo &&
                              t.id_origem_titulo == ORIGEMMATRICULA
                 select new
                 {
                     cd_baixa_titulo =  b.cd_baixa_titulo,
                     cd_baixa_automatica = tb.cd_baixa_automatica,
                     cd_tran_finan = tf.cd_tran_finan,
                     cd_titulo = t.cd_titulo,
                     no_local_movto =  l.no_local_movto,
                     no_emitente_cheque =  lfcheque.no_emitente_cheque,
                     no_pessoa =  p.no_pessoa,
                     nm_titulo =  t.nm_titulo,
                     nm_parcela_titulo =  t.nm_parcela_titulo,
                     dt_vcto_titulo =  t.dt_vcto_titulo,
                     dt_baixa_titulo =  b.dt_baixa_titulo,
                     vl_liquidacao_baixa =  b.vl_liquidacao_baixa,
                     vl_taxa_cartao =  b.vl_taxa_cartao

                 }).Distinct().ToList().Select(x => new BaixaEfetuadaChequeUI()
                 {
                     cd_baixa_titulo =  x.cd_baixa_titulo,
                     cd_baixa_automatica = x.cd_baixa_automatica,
                     cd_tran_finan = x.cd_tran_finan,
                     cd_titulo = x.cd_titulo,
                     no_local_movto =  x.no_local_movto,
                     no_emitente_cheque =  x.no_emitente_cheque,
                     no_pessoa =  x.no_pessoa,
                     nm_titulo = x.nm_titulo,
                     nm_parcela_titulo =  x.nm_parcela_titulo,
                     dt_vcto_titulo =  x.dt_vcto_titulo,
                     dt_baixa_titulo =  x.dt_baixa_titulo,
                     vl_liquidacao_baixa =  x.vl_liquidacao_baixa,
                     vl_taxa_cartao =  x.vl_taxa_cartao
                 }).AsQueryable()
                   .Union(
                       (from b in sql
                           join t in db.Titulo on b.cd_titulo equals t.cd_titulo
                           join tf in db.TransacaoFinanceira on b.cd_tran_finan equals tf.cd_tran_finan
                           join tb in db.BaixaAutomatica on tf.cd_baixa_automatica equals tb.cd_baixa_automatica
                           join l in db.LocalMovto on t.cd_local_movto equals l.cd_local_movto
                           join p in db.PessoaSGF on t.cd_pessoa_responsavel equals p.cd_pessoa
                            join c in db.Cheque on t.cd_origem_titulo equals c.cd_movimento into leftj
                            from lfcheque in leftj.DefaultIfEmpty() //left join)
                            where tb.cd_baixa_automatica == automaticaChequeUi.cd_baixa_automatica &&
                                  t.cd_pessoa_empresa == automaticaChequeUi.cd_escola &&
                                  t.cd_tipo_financeiro == (int)TipoFinanceiro.TiposFinanceiro.CHEQUE &&
                                  t.id_natureza_titulo == (int)Titulo.NaturezaTitulo.RECEBER &&
                                  t.vl_titulo != t.vl_saldo_titulo &&
                                  t.id_origem_titulo == ORIGEMMOMIMENTO
                        select new
                        {
                            cd_baixa_titulo = b.cd_baixa_titulo,
                            cd_baixa_automatica = tb.cd_baixa_automatica,
                            cd_tran_finan = tf.cd_tran_finan,
                            cd_titulo = t.cd_titulo,
                            no_local_movto = l.no_local_movto,
                            no_emitente_cheque = lfcheque.no_emitente_cheque,
                            no_pessoa = p.no_pessoa,
                            nm_titulo = t.nm_titulo,
                            nm_parcela_titulo = t.nm_parcela_titulo,
                            dt_vcto_titulo = t.dt_vcto_titulo,
                            dt_baixa_titulo = b.dt_baixa_titulo,
                            vl_liquidacao_baixa = b.vl_liquidacao_baixa,
                            vl_taxa_cartao = b.vl_taxa_cartao

                        }).Distinct().ToList().Select(x => new BaixaEfetuadaChequeUI()
                        {
                            cd_baixa_titulo = x.cd_baixa_titulo,
                            cd_baixa_automatica = x.cd_baixa_automatica,
                            cd_tran_finan = x.cd_tran_finan,
                            cd_titulo = x.cd_titulo,
                            no_local_movto = x.no_local_movto,
                            no_emitente_cheque = x.no_emitente_cheque,
                            no_pessoa = x.no_pessoa,
                            nm_titulo = x.nm_titulo,
                            nm_parcela_titulo = x.nm_parcela_titulo,
                            dt_vcto_titulo = x.dt_vcto_titulo,
                            dt_baixa_titulo = x.dt_baixa_titulo,
                            vl_liquidacao_baixa = x.vl_liquidacao_baixa,
                            vl_taxa_cartao = x.vl_taxa_cartao
                        }).AsQueryable()).Distinct();


               retorno = sorter.Sort(retorno);
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

        public IEnumerable<BaixaEfetuadaChequeUI> getBaixasEfetuadasForBaixaAutomaticaCartao(SearchParameters parametros, BaixaAutomaticaUI automaticaChartaoUi)
        {
            try
            {
                IEntitySorter<BaixaEfetuadaChequeUI> sorter = EntitySorter<BaixaEfetuadaChequeUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<BaixaTitulo> sql;

                SGFWebContext dbComp = new SGFWebContext();
                var ORIGEMMATRICULA = 22;
                var ORIGEMMOMIMENTO = 69;
                sql = from b in db.BaixaTitulo
                       .Include("LocalMovto")
                       .Include("Titulo")
                      select b;

                //if (automaticaChartaoUi.dt_inicial != null)
                //    sql = from b in sql
                //          where b.dt_baixa_titulo >= automaticaChartaoUi.dt_inicial
                //          select b;

                //if (automaticaChartaoUi.dt_final != null)
                //    sql = from b in sql
                //          where b.dt_baixa_titulo <= automaticaChartaoUi.dt_final
                //          select b;

                if (automaticaChartaoUi.cd_local_movto > 0 && automaticaChartaoUi.id_trocar_local == false)
                {
                    sql = from b in sql
                          where b.cd_local_movto == automaticaChartaoUi.cd_local_movto
                          select b;
                }

                //T_TITULO t
                //inner join T_BAIXA_TITULO b   on t.cd_titulo = b.cd_titulo
                //inner join T_TRAN_FINAN tf  on b.cd_tran_finan = tf.cd_tran_finan
                //inner join T_BAIXA_AUTOMATICA tb on tb.cd_baixa_automatica = tf.cd_baixa_automatica
                //inner join T_LOCAL_MOVTO l on t.cd_local_movto = l.cd_local_movto
                //inner join T_PESSOA p on t.cd_pessoa_responsavel = p.cd_pessoa
                //left join t_cheque c on t.cd_origem_titulo = c.cd_contrato

                var retorno = (from b in sql
                               join t in db.Titulo on b.cd_titulo equals t.cd_titulo
                               join tf in db.TransacaoFinanceira on b.cd_tran_finan equals tf.cd_tran_finan
                               join tb in db.BaixaAutomatica on tf.cd_baixa_automatica equals tb.cd_baixa_automatica
                               join l in db.LocalMovto on t.cd_local_movto equals l.cd_local_movto
                               join p in db.PessoaSGF on t.cd_pessoa_responsavel equals p.cd_pessoa
                               where tb.cd_baixa_automatica == automaticaChartaoUi.cd_baixa_automatica &&
                                     t.cd_pessoa_empresa == automaticaChartaoUi.cd_escola &&
                                     t.cd_tipo_financeiro == (int)TipoFinanceiro.TiposFinanceiro.CARTAO &&
                                     t.id_natureza_titulo == (int)Titulo.NaturezaTitulo.RECEBER &&
                                     t.vl_titulo != t.vl_saldo_titulo &&
                                     t.id_origem_titulo == ORIGEMMATRICULA
                               select new
                               {
                                   cd_baixa_titulo = b.cd_baixa_titulo,
                                   cd_baixa_automatica = tb.cd_baixa_automatica,
                                   cd_tran_finan = tf.cd_tran_finan,
                                   cd_titulo = t.cd_titulo,
                                   dc_cartao_movto = l.no_local_movto,
                                   cd_local_banco = l.cd_local_banco,
                                   cd_local_movto = l.cd_local_movto,
                                   no_local_movto = (from n in db.LocalMovto
                                       where n.cd_local_movto == l.cd_local_banco &&
                                             n.cd_pessoa_empresa == automaticaChartaoUi.cd_escola
                                       select n.no_local_movto).FirstOrDefault(),
                                   no_pessoa = p.no_pessoa,
                                   nm_titulo = t.nm_titulo,
                                   nm_parcela_titulo = t.nm_parcela_titulo,
                                   dt_vcto_titulo = t.dt_vcto_titulo,
                                   dt_baixa_titulo = b.dt_baixa_titulo,
                                   vl_liquidacao_baixa = b.vl_liquidacao_baixa,
                                   vl_taxa_cartao = b.vl_taxa_cartao

                               }).Distinct().ToList().Select(x => new BaixaEfetuadaChequeUI()
                               {
                                   cd_baixa_titulo = x.cd_baixa_titulo,
                                   cd_baixa_automatica = x.cd_baixa_automatica,
                                   cd_tran_finan = x.cd_tran_finan,
                                   cd_titulo = x.cd_titulo,
                                   cd_local_banco = x.cd_local_banco,
                                   cd_local_mvto = x.cd_local_movto,
                                   no_local_movto = x.no_local_movto,
                                   dc_cartao_movto = x.dc_cartao_movto,
                                   no_pessoa = x.no_pessoa,
                                   nm_titulo = x.nm_titulo,
                                   nm_parcela_titulo = x.nm_parcela_titulo,
                                   dt_vcto_titulo = x.dt_vcto_titulo,
                                   dt_baixa_titulo = x.dt_baixa_titulo,
                                   vl_liquidacao_baixa = x.vl_liquidacao_baixa,
                                   vl_taxa_cartao = x.vl_taxa_cartao
                               }).AsQueryable()
                    .Union(
                        (from b in sql
                         join t in db.Titulo on b.cd_titulo equals t.cd_titulo
                         join tf in db.TransacaoFinanceira on b.cd_tran_finan equals tf.cd_tran_finan
                         join tb in db.BaixaAutomatica on tf.cd_baixa_automatica equals tb.cd_baixa_automatica
                         join l in db.LocalMovto on t.cd_local_movto equals l.cd_local_movto
                         join p in db.PessoaSGF on t.cd_pessoa_responsavel equals p.cd_pessoa
                         where tb.cd_baixa_automatica == automaticaChartaoUi.cd_baixa_automatica &&
                               t.cd_pessoa_empresa == automaticaChartaoUi.cd_escola &&
                               t.cd_tipo_financeiro == (int)TipoFinanceiro.TiposFinanceiro.CARTAO &&
                               t.id_natureza_titulo == (int)Titulo.NaturezaTitulo.RECEBER &&
                               t.vl_titulo != t.vl_saldo_titulo &&
                               t.id_origem_titulo == ORIGEMMOMIMENTO
                         select new
                         {
                             cd_baixa_titulo = b.cd_baixa_titulo,
                             cd_baixa_automatica = tb.cd_baixa_automatica,
                             cd_tran_finan = tf.cd_tran_finan,
                             cd_titulo = t.cd_titulo,
                             dc_cartao_movto = l.no_local_movto,
                             cd_local_banco = l.cd_local_banco,
                             cd_local_movto = l.cd_local_movto,
                             no_local_movto = (from n in db.LocalMovto
                                                 where n.cd_local_movto == l.cd_local_banco &&
                                                      n.cd_pessoa_empresa == automaticaChartaoUi.cd_escola
                                                    select n.no_local_movto).FirstOrDefault(),
                                 
                             no_pessoa = p.no_pessoa,
                             nm_titulo = t.nm_titulo,
                             nm_parcela_titulo = t.nm_parcela_titulo,
                             dt_vcto_titulo = t.dt_vcto_titulo,
                             dt_baixa_titulo = b.dt_baixa_titulo,
                             vl_liquidacao_baixa = b.vl_liquidacao_baixa,
                             vl_taxa_cartao = b.vl_taxa_cartao

                         }).Distinct().ToList().Select(x => new BaixaEfetuadaChequeUI()
                         {
                             cd_baixa_titulo = x.cd_baixa_titulo,
                             cd_baixa_automatica = x.cd_baixa_automatica,
                             cd_tran_finan = x.cd_tran_finan,
                             cd_titulo = x.cd_titulo,
                             cd_local_banco = x.cd_local_banco,
                             cd_local_mvto = x.cd_local_movto,
                             no_local_movto = x.no_local_movto,
                             dc_cartao_movto = x.dc_cartao_movto,
                             no_pessoa = x.no_pessoa,
                             nm_titulo = x.nm_titulo,
                             nm_parcela_titulo = x.nm_parcela_titulo,
                             dt_vcto_titulo = x.dt_vcto_titulo,
                             dt_baixa_titulo = x.dt_baixa_titulo,
                             vl_liquidacao_baixa = x.vl_liquidacao_baixa,
                             vl_taxa_cartao = x.vl_taxa_cartao
                         }).AsQueryable()).Distinct();


                retorno = sorter.Sort(retorno);
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

        public IEnumerable<BaixaTitulo> getBaixasTransacaoFinan(int cd_transacao_finan,int cd_baixa,int cd_titulo, int cd_pessoa_empresa, TipoConsultaBaixaEnum tipoConsulta)
        {
            try
            {
                IEnumerable<BaixaTitulo> sql = null;
                switch (tipoConsulta)
                {
                    case TipoConsultaBaixaEnum.HAS_BAIXA_TITULO_EDIT:
                        sql = (from bt in db.BaixaTitulo
                               where bt.cd_tran_finan == cd_transacao_finan && bt.TransacaoFinanceira.cd_pessoa_empresa == cd_pessoa_empresa
                               select new
                               {
                                   cd_baixa_titulo = bt.cd_baixa_titulo,
                                   cd_titulo = bt.cd_titulo,
                                   cd_tran_finan = bt.cd_tran_finan,
                                   cd_tipo_liquidacao = bt.cd_tipo_liquidacao,
                                   cd_local_movto = bt.cd_local_movto,
                                   dt_baixa_titulo = bt.dt_baixa_titulo,
                                   id_baixa_processada = bt.id_baixa_processada,
                                   id_baixa_parcial = bt.id_baixa_parcial,
                                   nm_dias_float = bt.nm_dias_float,
                                   vl_liquidacao_baixa = bt.vl_liquidacao_baixa,
                                   vl_juros_baixa = bt.vl_juros_baixa,
                                   vl_desconto_baixa = bt.vl_desconto_baixa,
                                   vl_principal_baixa = bt.vl_principal_baixa,
                                   vl_juros_calculado = bt.vl_juros_calculado,
                                   vl_multa_calculada = bt.vl_multa_calculada,
                                   vl_desc_multa_baixa = bt.vl_desc_multa_baixa,
                                   vl_desc_juros_baixa = bt.vl_desc_juros_baixa,
                                   vl_multa_baixa = bt.vl_multa_baixa,
                                   vl_desconto_baixa_calculado = bt.vl_desconto_baixa_calculado,
                                   pc_pontualidade = bt.pc_pontualidade,
                                   tx_obs_baixa = bt.tx_obs_baixa,
                                   //Campos Titulo
                                   nm_titulo = bt.Titulo.nm_titulo,
                                   bt.Titulo.cd_tipo_financeiro,
                                   nm_parcela_titulo = bt.Titulo.nm_parcela_titulo,
                                   dt_vcto_titulo = bt.Titulo.dt_vcto_titulo,
                                   dt_emissao_titulo = bt.Titulo.dt_emissao_titulo,
                                   id_natureza_titulo = bt.Titulo.id_natureza_titulo,
                                   nomeResponsavel = bt.Titulo.PessoaResponsavel.no_pessoa,
                                   cheque = bt.ChequeBaixa.Join(db.Cheque, c => c.cd_cheque, cb => cb.cd_cheque, (c, cb) => new
                                   {
                                       cb.cd_cheque,
                                       cb.no_emitente_cheque,
                                       cb.no_agencia_cheque,
                                       cb.nm_agencia_cheque,
                                       cb.nm_digito_agencia_cheque,
                                       cb.nm_conta_corrente_cheque,
                                       cb.nm_digito_cc_cheque,
                                       cb.nm_primeiro_cheque,
                                       c.dt_bom_para,
                                       c.nm_cheque,
                                       cb.cd_banco
                                   }).FirstOrDefault()
                               }).ToList().Select(x => new BaixaTitulo
                          {
                              cd_baixa_titulo = x.cd_baixa_titulo,
                              cd_titulo = x.cd_titulo,
                              cd_tran_finan = x.cd_tran_finan,
                              cd_tipo_liquidacao = x.cd_tipo_liquidacao,
                              cd_local_movto = x.cd_local_movto,
                              dt_baixa_titulo = x.dt_baixa_titulo,
                              id_baixa_processada = x.id_baixa_processada,
                              id_baixa_parcial = x.id_baixa_parcial,
                              nm_dias_float = x.nm_dias_float,
                              vl_liquidacao_baixa = x.vl_liquidacao_baixa,
                              vl_juros_baixa = x.vl_juros_baixa,
                              vl_desconto_baixa = x.vl_desconto_baixa,
                              vl_principal_baixa = x.vl_principal_baixa,
                              vl_juros_calculado = x.vl_juros_calculado,
                              vl_multa_calculada = x.vl_multa_calculada,
                              vl_desc_multa_baixa = x.vl_desc_multa_baixa,
                              vl_desc_juros_baixa = x.vl_desc_juros_baixa,
                              vl_multa_baixa = x.vl_multa_baixa,
                              vl_desconto_baixa_calculado = x.vl_desconto_baixa_calculado,
                              pc_pontualidade = x.pc_pontualidade,
                              tx_obs_baixa = x.tx_obs_baixa,
                              //Campos Titulo
                              nm_titulo = x.nm_titulo,
                              nm_parcela_titulo = x.nm_parcela_titulo,
                              dt_vcto_titulo = String.Format("{0:dd/MM/yyyy}", x.dt_vcto_titulo),
                              id_natureza_titulo = x.id_natureza_titulo,
                              Titulo = new Titulo
                              {
                                  cd_titulo = x.cd_titulo,
                                  cd_tipo_financeiro = x.cd_tipo_financeiro,
                                  nomeResponsavel = x.nomeResponsavel,
                                  nm_titulo = x.nm_titulo,
                                  nm_parcela_titulo = x.nm_parcela_titulo,
                                  dt_vcto_titulo = x.dt_vcto_titulo,
                                  dt_emissao_titulo = x.dt_emissao_titulo,
                                  id_natureza_titulo = x.id_natureza_titulo,
                                  Cheque = x.cheque != null ?  new Cheque
                                  {
                                      cd_cheque = x.cheque.cd_cheque,
                                      no_emitente_cheque = x.cheque.no_emitente_cheque,
                                      no_agencia_cheque = x.cheque.no_agencia_cheque,
                                      nm_agencia_cheque = x.cheque.nm_agencia_cheque,
                                      nm_digito_agencia_cheque = x.cheque.nm_digito_agencia_cheque,
                                      nm_conta_corrente_cheque = x.cheque.nm_conta_corrente_cheque,
                                      nm_digito_cc_cheque = x.cheque.nm_digito_cc_cheque,
                                      cd_banco = x.cheque.cd_banco,
                                      dt_bom_para = x.cheque.dt_bom_para,
                                      nm_primeiro_cheque = x.cheque.nm_cheque
                                  } : null
                              }
                          });
                        break;
                    case TipoConsultaBaixaEnum.HAS_BAIXA_TITULO_TRANS_FINAN:
                        sql = from bt in db.BaixaTitulo
                              where bt.cd_tran_finan == cd_transacao_finan && bt.TransacaoFinanceira.cd_pessoa_empresa == cd_pessoa_empresa
                              select bt;
                        break;
                    case TipoConsultaBaixaEnum.HAS_BAIXA_TITULO_TRANS_FINAN_CHEQUE:
                        sql = from bt in db.BaixaTitulo.Include(x => x.ChequeBaixa).Include(x => x.ChequeBaixa.Select(s => s.Cheque))
                              where bt.cd_tran_finan == cd_transacao_finan && bt.TransacaoFinanceira.cd_pessoa_empresa == cd_pessoa_empresa
                              select bt;
                        break;
                    case TipoConsultaBaixaEnum.HAS_TODAS_BAIXAS_PARCIAL_EXETO_BAIXA_EXCLUIDA:
                        sql = from bt in db.BaixaTitulo
                              where bt.TransacaoFinanceira.cd_pessoa_empresa == cd_pessoa_empresa && bt.cd_baixa_titulo != cd_baixa
                              && bt.cd_titulo == cd_titulo
                              select bt;
                        break;
                    case TipoConsultaBaixaEnum.HAS_BAIXAS_PARCIAIS_TITULO:
                        sql = (from bt in db.BaixaTitulo
                              where bt.TransacaoFinanceira.cd_pessoa_empresa == cd_pessoa_empresa && bt.cd_titulo == cd_titulo
                               select new
                               {
                                   cd_baixa_titulo = bt.cd_baixa_titulo,
                                   cd_titulo = bt.cd_titulo,
                                   cd_tran_finan = bt.cd_tran_finan,
                                   cd_tipo_liquidacao = bt.cd_tipo_liquidacao,
                                   cd_local_movto = bt.cd_local_movto,
                                   dt_baixa_titulo = bt.dt_baixa_titulo,
                                   id_baixa_processada = bt.id_baixa_processada,
                                   id_baixa_parcial = bt.id_baixa_parcial,
                                   nm_dias_float = bt.nm_dias_float,
                                   vl_liquidacao_baixa = bt.vl_liquidacao_baixa,
                                   vl_juros_baixa = bt.vl_juros_baixa,
                                   vl_desconto_baixa = bt.vl_desconto_baixa,
                                   vl_principal_baixa = bt.vl_principal_baixa,
                                   vl_juros_calculado = bt.vl_juros_calculado,
                                   vl_multa_calculada = bt.vl_multa_calculada,
                                   vl_desc_multa_baixa = bt.vl_desc_multa_baixa,
                                   vl_desc_juros_baixa = bt.vl_desc_juros_baixa,
                                   vl_multa_baixa = bt.vl_multa_baixa,
                                   vl_desconto_baixa_calculado = bt.vl_desconto_baixa_calculado,
                                   pc_pontualidade = bt.pc_pontualidade,
                                   tx_obs_baixa = bt.tx_obs_baixa,
                                   //Campos Titulo
                                   nm_titulo = bt.Titulo.nm_titulo,
                                   nm_parcela_titulo = bt.Titulo.nm_parcela_titulo,
                                   dt_vcto_titulo = bt.Titulo.dt_vcto_titulo,
                                   id_natureza_titulo = bt.Titulo.id_natureza_titulo,
                                   nomeResponsavel = bt.Titulo.PessoaResponsavel.no_pessoa
                               }).ToList().Select(x => new BaixaTitulo
                                                {
                                                    cd_baixa_titulo = x.cd_baixa_titulo,
                                                    cd_titulo = x.cd_titulo,
                                                    cd_tran_finan = x.cd_tran_finan,
                                                    cd_tipo_liquidacao = x.cd_tipo_liquidacao,
                                                    cd_local_movto = x.cd_local_movto,
                                                    dt_baixa_titulo = x.dt_baixa_titulo,
                                                    id_baixa_processada = x.id_baixa_processada,
                                                    id_baixa_parcial = x.id_baixa_parcial,
                                                    nm_dias_float = x.nm_dias_float,
                                                    vl_liquidacao_baixa = x.vl_liquidacao_baixa,
                                                    vl_juros_baixa = x.vl_juros_baixa,
                                                    vl_desconto_baixa = x.vl_desconto_baixa,
                                                    vl_principal_baixa = x.vl_principal_baixa,
                                                    vl_juros_calculado = x.vl_juros_calculado,
                                                    vl_multa_calculada = x.vl_multa_calculada,
                                                    vl_desc_multa_baixa = x.vl_desc_multa_baixa,
                                                    vl_desc_juros_baixa = x.vl_desc_juros_baixa,
                                                    vl_multa_baixa = x.vl_multa_baixa,
                                                    vl_desconto_baixa_calculado = x.vl_desconto_baixa_calculado,
                                                    pc_pontualidade = x.pc_pontualidade,
                                                    tx_obs_baixa = x.tx_obs_baixa,
                                                    //Campos Titulo
                                                    nm_titulo = x.nm_titulo,
                                                    nm_parcela_titulo = x.nm_parcela_titulo,
                                                    dt_vcto_titulo = String.Format("{0:dd/MM/yyyy}", x.dt_vcto_titulo),
                                                    id_natureza_titulo = x.id_natureza_titulo,
                                                    Titulo = new Titulo
                                                    {
                                                        cd_titulo = x.cd_titulo,
                                                        nomeResponsavel = x.nomeResponsavel,
                                                        nm_titulo = x.nm_titulo,
                                                        nm_parcela_titulo = x.nm_parcela_titulo,
                                                        dt_vcto_titulo = x.dt_vcto_titulo,
                                                        id_natureza_titulo = x.id_natureza_titulo
                                                    }
                                                
                                                });
                        break;
                        
                }

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Recibo getReciboByBaixa(int cd_baixa, int cd_empresa)
        {
            try
            {
                var sql = (from bt in db.BaixaTitulo
                           where bt.cd_baixa_titulo == cd_baixa && bt.TransacaoFinanceira.cd_pessoa_empresa == cd_empresa
                           select new
                           {
                               sg_tipo_logradouro = bt.Titulo.Empresa.EnderecoPrincipal.TipoLogradouro.sg_tipo_logradouro,
                               no_bairro = bt.Titulo.Empresa.EnderecoPrincipal.Bairro.no_localidade,
                               no_cidade = bt.Titulo.Empresa.EnderecoPrincipal.Cidade.no_localidade,
                               dc_num_cep = bt.Titulo.Empresa.EnderecoPrincipal.Logradouro.dc_num_cep,
                               sg_estado = bt.Titulo.Empresa.EnderecoPrincipal.Estado.Estado.sg_estado,
                               no_localidade = bt.Titulo.Empresa.EnderecoPrincipal.Logradouro.no_localidade,
                               dc_num_endereco = bt.Titulo.Empresa.EnderecoPrincipal.dc_num_endereco,

                               dc_telefone_escola = bt.Titulo.Empresa.Telefone.dc_fone_mail,
                               dc_num_cgc = bt.Titulo.Empresa.dc_num_cgc,
                               dc_num_insc_estadual = bt.Titulo.Empresa.dc_num_insc_estadual,
                               vl_liquidacao_baixa = bt.vl_liquidacao_baixa,
                               dc_cidade_estado = bt.Titulo.Empresa.EnderecoPrincipal.Cidade.no_localidade + " - " + bt.Titulo.Empresa.EnderecoPrincipal.Estado.Estado.sg_estado,
                               dt_baixa_titulo = bt.dt_baixa_titulo,
                               tx_obs_baixa = bt.tx_obs_baixa,
                               nm_recibo = bt.nm_recibo,
                               no_pessoa = bt.Titulo.PessoaResponsavel.no_pessoa + (bt.Titulo.PessoaResponsavel.nm_natureza_pessoa == 1 ?//Pessoa Fisica
                                    db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == bt.Titulo.PessoaResponsavel.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().cd_pessoa_cpf != null ?
                                       " CPF: " + db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == bt.Titulo.PessoaResponsavel.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().PessoaSGFQueUsoOCpf.nm_cpf + "" :
                                          " CPF: " + (db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == bt.Titulo.PessoaResponsavel.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_cpf != null ?
                                          db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == bt.Titulo.PessoaResponsavel.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_cpf : "") :
                                           " CNPJ: " + db.PessoaSGF.OfType<PessoaJuridicaSGF>().Where(pj => pj.cd_pessoa == bt.Titulo.PessoaResponsavel.cd_pessoa).FirstOrDefault<PessoaJuridicaSGF>().dc_num_cgc + ""),
                               nm_titulo = bt.Titulo.nm_titulo ,
                               nm_parcela_titulo = bt.Titulo.nm_parcela_titulo,
                               dt_vcto_titulo = bt.Titulo.dt_vcto_titulo,
                               id_natureza_titulo = bt.Titulo.id_natureza_titulo
                           }).ToList().Select( x => new Recibo {
                               endereco = new EnderecoSGF()
                               {
                                   TipoLogradouro = new TipoLogradouroSGF()
                                   {
                                       sg_tipo_logradouro = x.sg_tipo_logradouro
                                   },
                                   Bairro = new LocalidadeSGF()
                                   {
                                       no_localidade = x.no_bairro
                                   },
                                   Cidade = new LocalidadeSGF()
                                   {
                                       no_localidade = x.no_cidade
                                   },
                                   Logradouro = new LocalidadeSGF()
                                   {
                                       dc_num_cep = x.dc_num_cep,
                                       no_localidade = x.no_localidade
                                   },
                                   Estado = new LocalidadeSGF()
                                   {
                                       Estado = new EstadoSGF()
                                       {
                                           sg_estado = x.sg_estado
                                       }
                                   },
                                   dc_num_endereco = x.dc_num_endereco
                               },
                               titulo = new Titulo{
                                   nm_titulo = x.nm_titulo,
                                   nm_parcela_titulo = x.nm_parcela_titulo,
                                   dt_vcto_titulo = x.dt_vcto_titulo,
                                   id_natureza_titulo = x.id_natureza_titulo
                               },
                               dc_telefone_escola = x.dc_telefone_escola,
                               dc_num_cgc = x.dc_num_cgc,
                               dc_num_insc_estadual = x.dc_num_insc_estadual,
                               vl_liquidacao_baixa = x.vl_liquidacao_baixa,
                               dc_cidade_estado = x.dc_cidade_estado,
                               dt_baixa_titulo = x.dt_baixa_titulo,
                               txt_obs_baixa = x.tx_obs_baixa,
                               nm_recibo = x.nm_recibo,
                               no_pessoa = x.no_pessoa
                           }).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public ReciboAgrupadoUI getReciboAgrupado(string cds_titulos_selecionados, int cd_empresa)
        {
            try
            {
                if (string.IsNullOrEmpty(cds_titulos_selecionados))
                {
                    throw new FinanceiroBusinessException(Utils.Messages.Messages.msgErroTitulosValidaReciboAgrupadoNotFound, null, FinanceiroBusinessException.TipoErro.ERRO_TITULOS_VALIDA_RECIBO_AGRUPADO_NOTFOUND, false);
                }
                
                List<int> titulosSelecionados = cds_titulos_selecionados.Split('|').Select(item => int.Parse(item)).ToList();

                //Valida se a quantidade de id é menor que 2
                if (titulosSelecionados != null && titulosSelecionados.Count < 2)
                {
                    throw new FinanceiroBusinessException(Utils.Messages.Messages.msgErroTitulosValidaReciboAgrupadoNotFound, null, FinanceiroBusinessException.TipoErro.ERRO_TITULOS_VALIDA_RECIBO_AGRUPADO_NOTFOUND, false);
                }

                int cd_titulo = titulosSelecionados[0];

                
               
                ReciboAgrupadoUI sql = (from t in db.Titulo
                                        where t.cd_titulo == cd_titulo &&
                                                 t.cd_pessoa_empresa == cd_empresa
                                           select new
                                           {
                                               sg_tipo_logradouro = t.Empresa.EnderecoPrincipal.TipoLogradouro.sg_tipo_logradouro,
                                               no_bairro = t.Empresa.EnderecoPrincipal.Bairro.no_localidade,
                                               no_cidade = t.Empresa.EnderecoPrincipal.Cidade.no_localidade,
                                               dc_num_cep = t.Empresa.EnderecoPrincipal.Logradouro.dc_num_cep,
                                               sg_estado = t.Empresa.EnderecoPrincipal.Estado.Estado.sg_estado,
                                               no_localidade = t.Empresa.EnderecoPrincipal.Logradouro.no_localidade,
                                               dc_num_endereco = t.Empresa.EnderecoPrincipal.dc_num_endereco,

                                               dc_telefone_escola = t.Empresa.Telefone.dc_fone_mail,
                                               dc_num_cgc = t.Empresa.dc_num_cgc,
                                               dc_num_insc_estadual = t.Empresa.dc_num_insc_estadual,
                                               dc_cidade_estado = t.Empresa.EnderecoPrincipal.Cidade.no_localidade + " - " + t.Empresa.EnderecoPrincipal.Estado.Estado.sg_estado,
                                               //dt_baixa_titulo = bt.dt_baixa_titulo,
                                               //tx_obs_baixa = bt.tx_obs_baixa,
                                               no_pessoa = db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == t.Pessoa.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().no_pessoa,
                                               no_pessoa_responsavel = db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == t.PessoaResponsavel.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().no_pessoa,
                                               cpf_pessoa = (t.PessoaResponsavel.nm_natureza_pessoa == 1 ?//Pessoa Fisica
                                                    db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == t.PessoaResponsavel.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().cd_pessoa_cpf != null ?
                                                       db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == t.PessoaResponsavel.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().PessoaSGFQueUsoOCpf.nm_cpf + "" :
                                                          (db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == t.PessoaResponsavel.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_cpf != null ?
                                                          db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == t.PessoaResponsavel.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_cpf : "") : ""),

                                               titulos = (from t2 in db.Titulo
                                                   where titulosSelecionados.Contains(t2.cd_titulo) &&
                                                         t2.cd_pessoa_empresa == cd_empresa
                                                   select t2).ToList(),
                                               baixas = (from bt in db.BaixaTitulo
                                                where titulosSelecionados.Contains(bt.cd_titulo) && bt.TransacaoFinanceira.cd_pessoa_empresa == cd_empresa
                                                         select bt).ToList()

            }).ToList().Select(x => new ReciboAgrupadoUI()
                                           {
                                               endereco = new EnderecoSGF()
                                               {
                                                   TipoLogradouro = new TipoLogradouroSGF()
                                                   {
                                                       sg_tipo_logradouro = x.sg_tipo_logradouro
                                                   },
                                                   Bairro = new LocalidadeSGF()
                                                   {
                                                       no_localidade = x.no_bairro
                                                   },
                                                   Cidade = new LocalidadeSGF()
                                                   {
                                                       no_localidade = x.no_cidade
                                                   },
                                                   Logradouro = new LocalidadeSGF()
                                                   {
                                                       dc_num_cep = x.dc_num_cep,
                                                       no_localidade = x.no_localidade
                                                   },
                                                   Estado = new LocalidadeSGF()
                                                   {
                                                       Estado = new EstadoSGF()
                                                       {
                                                           sg_estado = x.sg_estado
                                                       }
                                                   },
                                                   dc_num_endereco = x.dc_num_endereco
                                               },
                                               titulos = x.titulos,
                                               dc_telefone_escola = x.dc_telefone_escola,
                                               dc_num_cgc = x.dc_num_cgc,
                                               dc_num_insc_estadual = x.dc_num_insc_estadual,
                                               dc_cidade_estado = x.dc_cidade_estado,
                                               no_pessoa = x.no_pessoa,
                                               no_pessoa_responsavel = x.no_pessoa_responsavel,
                                               cpf_pessoa = x.cpf_pessoa,
                                               baixas = x.baixas
                                           }).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public ReciboPagamentoUI getReciboPagamentoByBaixa(int cd_baixa, int cd_empresa)
        {
            try
            {
               var sql = (from bt in db.BaixaTitulo.Include("SysUsuario").Include("SysUsuario.PessoaFisica")
                           where bt.cd_baixa_titulo == cd_baixa && bt.TransacaoFinanceira.cd_pessoa_empresa == cd_empresa
                           select new
                           {
                               sg_tipo_logradouro = bt.Titulo.Empresa.EnderecoPrincipal.TipoLogradouro.sg_tipo_logradouro,
                               no_bairro = bt.Titulo.Empresa.EnderecoPrincipal.Bairro.no_localidade,
                               no_cidade = bt.Titulo.Empresa.EnderecoPrincipal.Cidade.no_localidade,
                               dc_num_cep = bt.Titulo.Empresa.EnderecoPrincipal.Logradouro.dc_num_cep,
                               sg_estado = bt.Titulo.Empresa.EnderecoPrincipal.Estado.Estado.sg_estado,
                               no_localidade = bt.Titulo.Empresa.EnderecoPrincipal.Logradouro.no_localidade,
                               dc_num_endereco = bt.Titulo.Empresa.EnderecoPrincipal.dc_num_endereco,

                               dc_telefone_escola = bt.Titulo.Empresa.Telefone.dc_fone_mail,
                               dc_num_cgc = bt.Titulo.Empresa.dc_num_cgc,
                               dc_num_insc_estadual = bt.Titulo.Empresa.dc_num_insc_estadual,                               
                               dc_cidade_estado = bt.Titulo.Empresa.EnderecoPrincipal.Cidade.no_localidade + " - " + bt.Titulo.Empresa.EnderecoPrincipal.Estado.Estado.sg_estado,
                               dt_baixa_titulo = bt.dt_baixa_titulo,
                               tx_obs_baixa = bt.tx_obs_baixa,
                               nm_recibo = bt.nm_recibo,
                               no_pessoa = db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == bt.Titulo.cd_pessoa_titulo).FirstOrDefault<PessoaFisicaSGF>().no_pessoa,
                               no_pessoa_responsavel = db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == bt.Titulo.PessoaResponsavel.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().no_pessoa,
                               cpf_pessoa = (bt.Titulo.PessoaResponsavel.nm_natureza_pessoa == 1 ?//Pessoa Fisica
                                    db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == bt.Titulo.PessoaResponsavel.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().cd_pessoa_cpf != null ?
                                       db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == bt.Titulo.PessoaResponsavel.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().PessoaSGFQueUsoOCpf.nm_cpf + "" :
                                          (db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == bt.Titulo.PessoaResponsavel.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_cpf != null ?
                                          db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == bt.Titulo.PessoaResponsavel.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_cpf : "") : ""),
                               //" CNPJ: " + db.PessoaSGF.OfType<PessoaJuridicaSGF>().Where(pj => pj.cd_pessoa == bt.Titulo.PessoaResponsavel.cd_pessoa).FirstOrDefault<PessoaJuridicaSGF>().dc_num_cgc + ""),
                               cd_titulo = bt.Titulo.cd_titulo,
                               nm_titulo = bt.Titulo.nm_titulo,
                               nm_parcela_titulo = bt.Titulo.nm_parcela_titulo,
                               dt_vcto_titulo = bt.Titulo.dt_vcto_titulo,
                               id_natureza_titulo = bt.Titulo.id_natureza_titulo,
                               dc_tipo_liquidacao = bt.TipoLiquidacao.dc_tipo_liquidacao,
                               dc_tipo_titulo = bt.Titulo.dc_tipo_titulo,
                               vl_titulo = bt.Titulo.vl_titulo,
                               vl_desconto_titulo = bt.Titulo.vl_desconto_titulo,
                               vl_multa_titulo = bt.Titulo.vl_multa_titulo,

                               baixaTitulo = bt,
                               total_titulo = db.Titulo.Where(tit => tit.cd_origem_titulo == bt.Titulo.cd_origem_titulo && tit.id_origem_titulo == bt.Titulo.id_origem_titulo).Count(),
                               no_pessoa_usuario = bt.SysUsuario != null && bt.SysUsuario.PessoaFisica != null ? bt.SysUsuario.PessoaFisica.no_pessoa : ""
                           }).ToList().Select(x => new ReciboPagamentoUI
                           {
                               endereco = new EnderecoSGF()
                               {
                                   TipoLogradouro = new TipoLogradouroSGF()
                                   {
                                       sg_tipo_logradouro = x.sg_tipo_logradouro
                                   },
                                   Bairro = new LocalidadeSGF()
                                   {
                                       no_localidade = x.no_bairro
                                   },
                                   Cidade = new LocalidadeSGF()
                                   {
                                       no_localidade = x.no_cidade
                                   },
                                   Logradouro = new LocalidadeSGF()
                                   {
                                       dc_num_cep = x.dc_num_cep,
                                       no_localidade = x.no_localidade
                                   },
                                   Estado = new LocalidadeSGF()
                                   {
                                       Estado = new EstadoSGF()
                                       {
                                           sg_estado = x.sg_estado
                                       }
                                   },
                                   dc_num_endereco = x.dc_num_endereco
                               },
                               titulo = new Titulo
                               {
                                   cd_titulo = x.cd_titulo,
                                   nm_titulo = x.nm_titulo,
                                   nm_parcela_titulo = x.nm_parcela_titulo,
                                   dt_vcto_titulo = x.dt_vcto_titulo,
                                   id_natureza_titulo = x.id_natureza_titulo,
                                   vl_titulo = x.vl_titulo,
                                   vl_desconto_titulo = x.vl_desconto_titulo,
                                   vl_multa_titulo = x.vl_multa_titulo,
                                   dc_tipo_titulo = x.dc_tipo_titulo
                               },
                               dc_telefone_escola = x.dc_telefone_escola,
                               dc_num_cgc = x.dc_num_cgc,
                               dc_num_insc_estadual = x.dc_num_insc_estadual,
                               dc_cidade_estado = x.dc_cidade_estado,
                               dt_baixa_titulo = x.dt_baixa_titulo,
                               txt_obs_baixa = x.tx_obs_baixa,
                               nm_recibo = x.nm_recibo,
                               no_pessoa = x.no_pessoa,
                               no_pessoa_responsavel = x.no_pessoa_responsavel,
                               cpf_pessoa = x.cpf_pessoa,
                               dc_tipo_liquidacao = x.dc_tipo_liquidacao,
                               baixaTitulo = new BaixaTitulo 
                               {
                                   cd_tipo_liquidacao = x.baixaTitulo.cd_tipo_liquidacao,
                                   vl_baixa_saldo_titulo = x.baixaTitulo.vl_baixa_saldo_titulo,
                                   vl_desconto_baixa = x.baixaTitulo.vl_desconto_baixa,
                                   vl_juros_baixa = x.baixaTitulo.vl_juros_baixa,
                                   vl_multa_baixa = x.baixaTitulo.vl_multa_baixa,
                                   vl_liquidacao_baixa = x.baixaTitulo.vl_liquidacao_baixa,
                                   vl_principal_baixa = x.baixaTitulo.vl_principal_baixa
                               },
                               total_titulo = x.total_titulo,
                               no_pessoa_usuario = x.no_pessoa_usuario,                               
                           }).FirstOrDefault();

                if(sql != null) {
                    var motivo_bolsa = db.BaixaTitulo.Where(bt => (bt.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA || 
                                                                   bt.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA_ADITIVO) &&
                                       bt.cd_titulo == sql.titulo.cd_titulo && bt.TransacaoFinanceira.cd_pessoa_empresa == cd_empresa).AsNoTracking().FirstOrDefault() ?? null;
                    if (motivo_bolsa != null)
                        sql.titulo.vl_titulo -= motivo_bolsa.vl_baixa_saldo_titulo;
                }

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public ReciboPagamentoUI getVerificaReciboPagamentoByBaixa(int cd_baixa, int cd_empresa)
        {
            try
            {
                var sql = (from bt in db.BaixaTitulo.Include("SysUsuario").Include("SysUsuario.PessoaFisica")
                           where bt.cd_baixa_titulo == cd_baixa && bt.TransacaoFinanceira.cd_pessoa_empresa == cd_empresa
                           select new
                           {
                               cd_titulo = bt.Titulo.cd_titulo,
                               cd_tipo_liquidacao = bt.cd_tipo_liquidacao,
                               vl_titulo = bt.Titulo.vl_titulo
                           }).ToList().Select(x => new ReciboPagamentoUI
                           {
                               endereco = null,
                               titulo = new Titulo
                               {
                                   cd_titulo = x.cd_titulo,
                                   vl_titulo = x.vl_titulo,
                               },
                               baixaTitulo = new BaixaTitulo
                               {
                                   cd_tipo_liquidacao = x.cd_tipo_liquidacao,
                               }
                               
                           }).FirstOrDefault();

                if (sql != null)
                {
                    var motivo_bolsa = db.BaixaTitulo.Where(bt => (bt.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA ||
                                                                   bt.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA_ADITIVO) &&
                                       bt.cd_titulo == sql.titulo.cd_titulo && bt.TransacaoFinanceira.cd_pessoa_empresa == cd_empresa).AsNoTracking().FirstOrDefault() ?? null;
                    if (motivo_bolsa != null)
                        sql.titulo.vl_titulo -= motivo_bolsa.vl_baixa_saldo_titulo;
                }

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }












        public bool validaReciboAgrupadoAlunosResponsaveisDiferentes(List<int> cds_titulos_selecionados, int cd_escola, int tipo_validacao)
        {
            try
            {
                SGFWebContext dbComp = new SGFWebContext();
                //int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Contrato"].ToString());

                var sql = (from t in db.Titulo
                    where cds_titulos_selecionados.Contains(t.cd_titulo) &&
                          t.cd_pessoa_empresa == cd_escola
                    select new
                    {
                        cd_titulo = t.cd_titulo,
                        cd_pessoa_titulo = t.cd_pessoa_titulo,
                        cd_pessoa_responsavel = t.cd_pessoa_responsavel

                    }).ToList().Select(x => new TituloValidaReciboAgrupadoUI
                    {
                        cd_titulo = x.cd_titulo,
                        cd_pessoa_responsavel = x.cd_pessoa_responsavel,
                        cd_pessoa_titulo = x.cd_pessoa_titulo
                    }).ToList();

                if (sql != null && sql.Any())
                {
                    int qtdAlunosDiferentes = 0;

                    //Valida se os titulos selecionados são de alunos diferentes
                    if (tipo_validacao == (int)BaixaTituloDataAccess.TipoValidacaoReciboAgrupadoEnum.ALUNO)
                    {
                        qtdAlunosDiferentes = sql.Select(z => z.cd_pessoa_titulo).Distinct().Count();
                    }
                    //Valida se os titulos selecionados são de responsaveis diferentes
                    else if (tipo_validacao == (int)BaixaTituloDataAccess.TipoValidacaoReciboAgrupadoEnum.RESPONSAVEL)
                    {
                        qtdAlunosDiferentes = sql.Select(z => z.cd_pessoa_responsavel).Distinct().Count();
                        
                    }
                    return (qtdAlunosDiferentes > 1) ? true : false;

                }
                else
                {
                    throw new FinanceiroBusinessException(Utils.Messages.Messages.msgErroTitulosValidaReciboAgrupadoNotFound, null, FinanceiroBusinessException.TipoErro.ERRO_TITULOS_VALIDA_RECIBO_AGRUPADO_NOTFOUND, false);
                }
                
                
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool verificarTituloOrigemMatricula(int cd_baixa, int cd_escola)
        {
            try
            {
                SGFWebContext dbComp = new SGFWebContext();
                int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                var sql = (from t in db.BaixaTitulo
                          where t.cd_baixa_titulo == cd_baixa && 
                                t.Titulo.cd_pessoa_empresa == cd_escola &&
                                t.Titulo.id_origem_titulo == cd_origem
                          select t.cd_baixa_titulo).Any();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<BaixaTitulo> getBaixaTitulosBolsaContrato(int cd_contrato, int cd_pessoa_empresa)
        {
            try
            {
                SGFWebContext dbComp = new SGFWebContext();
                int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                var sql = from bt in db.BaixaTitulo
                          where (bt.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA || bt.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA_ADITIVO) &&
                                bt.Titulo.cd_pessoa_empresa == cd_pessoa_empresa &&
                                bt.Titulo.id_origem_titulo == cd_origem &&
                                bt.Titulo.cd_origem_titulo == cd_contrato
                          select bt;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<BaixaTitulo> getBaixaTitulosBolsaContrato(int cd_contrato, int cd_pessoa_empresa, List<int> cdTitulos)
        {
            try
            {
                SGFWebContext dbComp = new SGFWebContext();
                int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                var sql = from bt in db.BaixaTitulo
                          where (bt.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA || bt.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA_ADITIVO) &&
                                bt.Titulo.cd_pessoa_empresa == cd_pessoa_empresa &&
                                bt.Titulo.id_origem_titulo == cd_origem &&
                                bt.Titulo.cd_origem_titulo == cd_contrato &&
                                cdTitulos.Contains(bt.cd_titulo)
                          select bt;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool baixaMotivoBolsaContrato(int cd_baixa, int cd_escola)
        {
            try
            {
                SGFWebContext dbComp = new SGFWebContext();
                int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                var sql = (from c in db.Contrato
                           where c.cd_pessoa_escola == cd_escola &&
                                 db.Titulo.Any(t => t.BaixaTitulo.Any(b => b.cd_baixa_titulo == cd_baixa && (b.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA || 
                                                                                                             b.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA_ADITIVO)) &&
                                                    t.cd_pessoa_empresa == cd_escola &&
                                                    t.id_origem_titulo == cd_origem &&
                                                    t.cd_origem_titulo == c.cd_contrato)
                           select c.id_ajuste_manual).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool verificaBaixaTituloByAluno(int cd_aluno, int cd_escola, DateTime dtPol)
        {
            try
            {
                SGFWebContext dbComp = new SGFWebContext();
                int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                var sql = (from p in db.BaixaTitulo
                           where db.Aluno.Where(a => a.cd_pessoa_aluno == p.Titulo.cd_pessoa_titulo && a.cd_aluno == cd_aluno && p.Titulo.id_origem_titulo == cd_origem).Any()
                                && p.Titulo.cd_pessoa_empresa == cd_escola
                                && p.dt_baixa_titulo >= dtPol
                                 && p.Titulo.id_origem_titulo == cd_origem
                                 && (p.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA || p.cd_tipo_liquidacao == (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA_ADITIVO)
                          select p).Any();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool verificaBaixaTituloByPolAluno(int cd_aluno, int cd_escola, int cd_poltica)
        {
            try
            {
                SGFWebContext dbComp = new SGFWebContext();
                int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                var sql = (from p in db.BaixaTitulo
                           where db.Aluno.Where(a => a.cd_pessoa_aluno == p.Titulo.cd_pessoa_titulo && a.cd_aluno == cd_aluno && p.Titulo.id_origem_titulo == cd_origem).Any()
                                 && p.Titulo.cd_pessoa_empresa == cd_escola 
                                 && p.cd_politica_desconto == cd_poltica
                                 && p.Titulo.id_origem_titulo == cd_origem
                                 && p.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA && p.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA_ADITIVO
                           select p).Any();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool verificaBaixaTituloByTurma(int cd_turma, int cd_escola, DateTime dtPol)
        {
            try
            {
                SGFWebContext dbComp = new SGFWebContext();
                int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                var sql = (from p in db.BaixaTitulo
                           where db.AlunoTurma.Where(a => a.Aluno.cd_pessoa_aluno == p.Titulo.cd_pessoa_titulo && a.cd_turma == cd_turma && p.Titulo.cd_origem_titulo == a.cd_contrato && p.Titulo.id_origem_titulo == cd_origem).Any()
                                 && p.Titulo.id_origem_titulo == cd_origem
                                 && p.Titulo.cd_pessoa_empresa == cd_escola                                 
                                 && p.dt_baixa_titulo >= dtPol
                                 && p.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA && p.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA_ADITIVO
                           select p).Any();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool verificaBaixaTituloByPolTurma(int cd_turma, int cd_escola, int cd_poltica)
        {
            try
            {
                SGFWebContext dbComp = new SGFWebContext();
                int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Contrato"].ToString());
                var sql = (from p in db.BaixaTitulo
                           where db.AlunoTurma.Where(a => a.Aluno.cd_pessoa_aluno == p.Titulo.cd_pessoa_titulo && a.cd_turma == cd_turma && p.Titulo.cd_origem_titulo == a.cd_contrato && p.Titulo.id_origem_titulo == cd_origem).Any()
                                 && p.Titulo.cd_pessoa_empresa == cd_escola
                                 && p.cd_politica_desconto == cd_poltica
                                 && p.Titulo.id_origem_titulo == cd_origem
                                 && p.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA && p.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA_ADITIVO
                           select p).Any();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool verificaBaixaAposDataPol(int cd_escola, DateTime dtPol)
        {
            try
            {
                var sql = (from p in db.BaixaTitulo
                           where p.Titulo.cd_pessoa_empresa == cd_escola
                                 && p.dt_baixa_titulo >= dtPol
                           select p).Any();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public int getUltimoNroRecibo(int? nm_ultimo_Recibo, int cd_empresa)
        {
            try
            {
                int ultimo = 0;
                int? ultimo_recibo = null;

                if (nm_ultimo_Recibo.HasValue)
                {

                    ultimo_recibo = (from c in db.BaixaTitulo
                                     where c.nm_recibo == nm_ultimo_Recibo.Value
                                        && c.TransacaoFinanceira.cd_pessoa_empresa == cd_empresa
                                     select c.nm_recibo).FirstOrDefault();

                    if (ultimo_recibo.HasValue)
                        //Pesquisa a primeira matrícula que possui número de matrícula maior ou igual que a do parâmetro, mas que não tenha a próxima matrícula (nro + 1):
                        ultimo_recibo = (from c in db.BaixaTitulo
                                         where !(from c2 in db.BaixaTitulo
                                                 where c2.nm_recibo == c.nm_recibo + 1
                                                 && c2.TransacaoFinanceira.cd_pessoa_empresa == cd_empresa
                                                 select c2.nm_recibo).Any()
                                         where c.nm_recibo >= nm_ultimo_Recibo.Value - 1
                                            && c.TransacaoFinanceira.cd_pessoa_empresa == cd_empresa
                                         select c.nm_recibo).Min();
                    else
                        ultimo_recibo = nm_ultimo_Recibo - 1;
                }
                else
                {
                    ultimo_recibo = (from c in db.BaixaTitulo
                                     where c.TransacaoFinanceira.cd_pessoa_empresa == cd_empresa && c.nm_recibo > 0
                                     orderby c.nm_recibo descending
                                     select c.nm_recibo).FirstOrDefault();
                }
                if (ultimo_recibo.HasValue)
                    ultimo = ultimo_recibo.Value;
                return ultimo;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

    } 
}

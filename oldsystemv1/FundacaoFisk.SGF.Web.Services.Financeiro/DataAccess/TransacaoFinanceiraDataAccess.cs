using System;
using System.Collections.Generic;
using System.Linq;
using Componentes.GenericModel;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using Componentes.GenericDataAccess;
using FundacaoFisk.SGF.Web.Services.Financeiro;
using System.Data.Entity;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess;
using System.Data.SqlClient;
using FundacaoFisk.SGF.Web.Services.Financeiro.Model;
using Componentes.GenericDataAccess.GenericException;
using System.Data.Entity.Core.Objects;
using System.Data.Objects.SqlClient;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess
{
    public class TransacaoFinanceiraDataAccess : GenericRepository<TransacaoFinanceira>, ITransacaoFinanceiraDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public TransacaoFinanceira getTransacaoFinanceira(int cd_tran_finan, int cd_pessoa_empresa)
        {
            try
            {
                TransacaoFinanceira sql = (from i in db.TransacaoFinanceira
                                           where i.cd_tran_finan == cd_tran_finan && i.cd_pessoa_empresa == cd_pessoa_empresa
                                           select i).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public TransacaoFinanceira getTransacaoFinanceira(int cd_titulo, int cd_contrato, int cd_pessoa_empresa)
        {
            try
            {
                TransacaoFinanceira sql = (from t in db.TransacaoFinanceira
                                           where t.cd_pessoa_empresa == cd_pessoa_empresa &&
                                                 t.Baixas.Any(x => x.Titulo.cd_origem_titulo == cd_contrato && x.Titulo.cd_titulo == cd_titulo)
                                           select t).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public TransacaoFinanceira getTransacaoBaixaTitulo(int cd_titulo, int cd_pessoa_empresa)
        {
            try
            {
                TransacaoFinanceira sql = (from i in db.TransacaoFinanceira                                           
                                           where i.Baixas.Where(b => b.Titulo.cd_titulo == cd_titulo).Any() && 
                                           i.cd_pessoa_empresa == cd_pessoa_empresa
                                           select i).FirstOrDefault();
                sql.Baixas = (from bt in db.BaixaTitulo
                             where bt.Titulo.cd_titulo == cd_titulo &&
                                   bt.cd_tran_finan == sql.cd_tran_finan
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
                          }).ToList();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public TransacaoFinanceira getTransacaoBaixaTituloBolsaReajusteAnual(int cd_titulo, int cd_contrato, int cd_pessoa_empresa)
        {
            try
            {
                TransacaoFinanceira sql = (from t in db.TransacaoFinanceira
                                           where t.Baixas.Where(b => b.Titulo.cd_titulo == cd_titulo).Any() &&
                                           t.cd_pessoa_empresa == cd_pessoa_empresa
                                           select new
                                           {
                                               t.cd_tran_finan,
                                               t.dt_tran_finan,
                                               t.cd_pessoa_empresa,
                                               t.cd_local_movto,
                                               t.cd_tipo_liquidacao
                                           }).ToList().Select(x => new TransacaoFinanceira {
                                               cd_tran_finan = x.cd_tran_finan,
                                               dt_tran_finan= x.dt_tran_finan,
                                               cd_pessoa_empresa= x.cd_pessoa_empresa,
                                               cd_local_movto= x.cd_local_movto,
                                               cd_tipo_liquidacao = x.cd_tipo_liquidacao
                                           }).FirstOrDefault();
                sql.Baixas = (from bt in db.BaixaTitulo
                              where bt.cd_tran_finan == sql.cd_tran_finan
                              select new
                              {
                                  cd_baixa_titulo = bt.cd_baixa_titulo,
                                  cd_titulo = bt.cd_titulo
                              }).ToList().Select(x => new BaixaTitulo
                              {
                                  cd_baixa_titulo = x.cd_baixa_titulo,
                                  cd_titulo = x.cd_titulo
                              }).ToList();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}
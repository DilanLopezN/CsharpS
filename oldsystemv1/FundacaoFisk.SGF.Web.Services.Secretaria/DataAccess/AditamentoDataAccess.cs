using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Net.Sockets;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Secretaria.Comum.IDataAccess;
using Componentes.GenericDataAccess.GenericException;
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess
{
    public class AditamentoDataAccess : GenericRepository<Aditamento>, IAditamentoDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }

        public Aditamento getAditamentoByContrato(int cd_contrato, int cd_escola)
        {
            try
            {
                var sql = (from aditamento in db.Aditamento
                           where aditamento.cd_contrato == cd_contrato
                             && aditamento.Contrato.cd_pessoa_escola == cd_escola
                           select aditamento).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {

                throw new DataAccessException(exe);
            }
        }

        public Aditamento getAditamentoByContrato(int cd_aditamento, int cd_contrato, int cd_escola)
        {
            try
            {
                var sql = (from aditamento in db.Aditamento.Include(x => x.AditamentoBolsa)
                           where aditamento.cd_contrato == cd_contrato && aditamento.cd_aditamento == cd_aditamento
                             && aditamento.Contrato.cd_pessoa_escola == cd_escola
                           select aditamento).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {

                throw new DataAccessException(exe);
            }
        }

        public Aditamento getAditamentoByContratoEData(DateTime? data_aditamento, int cd_contrato, int cd_pessoa_escola)
        {
            try
            {
                Aditamento sql = (from aditamento in db.Aditamento
                                  where aditamento.cd_contrato == cd_contrato
                                  && aditamento.Contrato.cd_pessoa_escola == cd_pessoa_escola
                                  && aditamento.dt_aditamento == data_aditamento
                                  select aditamento).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {

                throw new DataAccessException(exe);
            }
        }

        public Aditamento getUltimoAditamentoByContrato(int cd_contrato, int cd_pessoa_escola)
        {
            try
            {
                Aditamento sql = (from aditamento in db.Aditamento
                    where aditamento.cd_aditamento == ((from a in db.Aditamento
                              where a.cd_contrato == cd_contrato
                                    && a.Contrato.cd_pessoa_escola == cd_pessoa_escola
                              select a).Max(b => b.cd_aditamento))
                    select aditamento).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {

                throw new DataAccessException(exe);
            }
        }


        public List<AditamentoBolsa> getAditamentoBolsaByAditamento(int cd_aditamento)
        {
            try
            {
                List<AditamentoBolsa> sql = (from a in db.Aditamento
                    join b in db.AditamentoBolsa on a.cd_aditamento equals b.cd_aditamento 
                    where a.cd_aditamento == cd_aditamento
                    select b).ToList();
                return sql;
            }
            catch (Exception exe)
            {

                throw new DataAccessException(exe);
            }
        }

        public List<DescontoContrato> getDescontoContratoByAditamento(int cd_aditamento)
        {
            try
            {
                List<DescontoContrato> sql = (from a in db.Aditamento
                    join d in db.DescontoContrato on a.cd_aditamento equals d.cd_aditamento
                    where a.cd_aditamento == cd_aditamento
                    select d).ToList();
                return sql;
            }
            catch (Exception exe)
            {

                throw new DataAccessException(exe);
            }
        }

        public Aditamento getAditamentoByContratoMaxData(int cd_contrato, int cd_escola)
        {
            try
            {
                var sql =  (from aditamento in db.Aditamento
                           where aditamento.cd_contrato == cd_contrato
                             && aditamento.Contrato.cd_pessoa_escola == cd_escola
                           orderby aditamento.dt_aditamento descending
                           select aditamento).FirstOrDefault();
                 if (sql != null && sql.cd_aditamento > 0)
                     sql.Desconto = (from descontoContrato in db.DescontoContrato
                                     where descontoContrato.cd_aditamento == sql.cd_aditamento && 
                                           descontoContrato.Aditamento.Contrato.cd_pessoa_escola == cd_escola
                          select new
                          {
                              cd_aditamento = descontoContrato.cd_aditamento,
                              cd_contrato = descontoContrato.cd_contrato,
                              dc_tipo_desconto = descontoContrato.dc_desconto_contrato,
                              pc_desconto = descontoContrato.pc_desconto_contrato,
                              vl_desconto_contrato = descontoContrato.vl_desconto_contrato,
                              id_incide_baixa = descontoContrato.id_incide_baixa,
                              id_desconto_ativo = descontoContrato.id_desconto_ativo,
                              cd_tipo_desconto = descontoContrato.cd_tipo_desconto,
                              cd_desconto_contrato = descontoContrato.cd_desconto_contrato
                          }).ToList().Select(x => new DescontoContrato
                          {
                              cd_aditamento = x.cd_aditamento,
                              cd_contrato = x.cd_contrato,
                              dc_tipo_desconto = x.dc_tipo_desconto,
                              pc_desconto = x.pc_desconto,
                              vl_desconto_contrato = x.vl_desconto_contrato,
                              id_incide_baixa = x.id_incide_baixa,
                              id_desconto_ativo = x.id_desconto_ativo,
                              cd_tipo_desconto = x.cd_tipo_desconto,
                              cd_desconto_contrato = x.cd_desconto_contrato
                          }).ToList();

                return sql;

            }
            catch (Exception exe)
            {

                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Aditamento> getAditamentosByContrato(int cd_contrato, int cd_escola)
        {
            try
            {
              List<Aditamento> sql = (from aditamento in db.Aditamento//.Include(a => a.NomeContrato)
                           where aditamento.cd_contrato == cd_contrato
                             && aditamento.Contrato.cd_pessoa_escola == cd_escola
                           orderby aditamento.dt_aditamento descending
                           select new
                           {
                               cd_aditamento = aditamento.cd_aditamento,
                               cd_contrato = aditamento.cd_contrato,
                               dt_inicio_aditamento = aditamento.dt_inicio_aditamento,
                               dt_aditamento = aditamento.dt_aditamento,
                               dt_vencto_inicial = aditamento.dt_vencto_inicial,
                               nm_dia_vcto_desconto = aditamento.nm_dia_vcto_desconto,
                               cd_nome_contrato = aditamento.cd_nome_contrato,
                               id_tipo_aditamento = aditamento.id_tipo_aditamento,
                               nm_previsao_inicial = aditamento.nm_previsao_inicial,
                               vl_aula_hora = aditamento.vl_aula_hora,
                               id_tipo_pagamento = aditamento.id_tipo_pagamento,
                               nm_titulos_aditamento = aditamento.nm_titulos_aditamento,
                               dt_vcto_aditamento = aditamento.dt_vcto_aditamento,
                               id_tipo_data_inicio = aditamento.id_tipo_data_inicio,
                               no_contrato = aditamento.NomeContrato.no_contrato,
                               no_usuario = aditamento.SysUsuario.no_login,
                               cd_usuario = aditamento.cd_usuario,
                               vl_aditivo = aditamento.vl_aditivo,
                               txt_obs = aditamento.tx_obs_aditamento,
                               aditamento.vl_parcela_titulo_aditamento,
                               aditamento.id_ajuste_manual,
                               aditamento.cd_tipo_financeiro,
                               possui_descontos = aditamento.Desconto.Any()
                }).ToList().Select(x => new Aditamento {
                               cd_aditamento = x.cd_aditamento,
                               cd_contrato = x.cd_contrato,
                               dt_aditamento = x.dt_aditamento,
                               dt_vencto_inicial = x.dt_vencto_inicial,
                               dt_inicio_aditamento = x.dt_inicio_aditamento,
                               nm_dia_vcto_desconto = x.nm_dia_vcto_desconto,
                               cd_nome_contrato = x.cd_nome_contrato,
                               id_tipo_aditamento = x.id_tipo_aditamento,
                               nm_previsao_inicial = x.nm_previsao_inicial,
                               vl_aula_hora = x.vl_aula_hora,
                               id_tipo_pagamento = x.id_tipo_pagamento,
                               nm_titulos_aditamento = x.nm_titulos_aditamento,
                               dt_vcto_aditamento = x.dt_vcto_aditamento,
                               id_tipo_data_inicio = x.id_tipo_data_inicio,
                               no_contrato = x.no_contrato,
                               no_usuario = x.no_usuario,
                               cd_usuario = x.cd_usuario,
                               vl_aditivo = x.vl_aditivo,
                               tx_obs_aditamento = x.txt_obs,
                               vl_parcela_titulo_aditamento = x.vl_parcela_titulo_aditamento,
                               id_ajuste_manual = x.id_ajuste_manual,
                               possui_descontos = x.possui_descontos,
                               cd_tipo_financeiro = x.cd_tipo_financeiro
                           }).ToList();

              if (sql != null && sql.Count() > 0)
              {
                  List<int> cdAditamentos = new List<int>();
                  cdAditamentos = sql.Select(x => x.cd_aditamento).ToList();
                  var sql2 = ((from descontoContrato in db.DescontoContrato
                               where cdAditamentos.Contains((int)descontoContrato.cd_aditamento) && 
                                     descontoContrato.Aditamento.Contrato.cd_pessoa_escola == cd_escola
                          select new
                          {
                              cd_aditamento = descontoContrato.cd_aditamento,
                              cd_contrato = descontoContrato.cd_contrato,
                              dc_tipo_desconto = descontoContrato.dc_desconto_contrato,
                              pc_desconto = descontoContrato.pc_desconto_contrato,
                              vl_desconto_contrato = descontoContrato.vl_desconto_contrato,
                              id_incide_baixa = descontoContrato.id_incide_baixa,
                              id_desconto_ativo = descontoContrato.id_desconto_ativo,
                              cd_tipo_desconto = descontoContrato.cd_tipo_desconto,
                              cd_desconto_contrato = descontoContrato.cd_desconto_contrato,
                              descontoContrato.nm_parcela_ini,
                              descontoContrato.nm_parcela_fim
                          }).ToList().Select(x => new DescontoContrato
                          {
                              cd_aditamento = x.cd_aditamento,
                              cd_contrato = x.cd_contrato,
                              dc_tipo_desconto = x.dc_tipo_desconto,
                              pc_desconto = x.pc_desconto,
                              vl_desconto_contrato = x.vl_desconto_contrato,
                              id_incide_baixa = x.id_incide_baixa,
                              id_desconto_ativo = x.id_desconto_ativo,
                              cd_tipo_desconto = x.cd_tipo_desconto,
                              cd_desconto_contrato = x.cd_desconto_contrato,
                              desc_incluso_valor = true,
                              nm_parcela_ini = x.nm_parcela_ini,
                              nm_parcela_fim = x.nm_parcela_fim
                          }));
                  foreach (Aditamento ad in sql)
                      ad.Desconto = sql2.Where(x => x.cd_aditamento == ad.cd_aditamento).ToList();
                  cdAditamentos = sql.Where(x=> x.id_tipo_aditamento == (int)Aditamento.TipoAditamento.ADITIVO_BOLSA).Select(x => x.cd_aditamento).ToList();
                  var sql3 = (from ab in db.AditamentoBolsa
                              where cdAditamentos.Contains(ab.cd_aditamento)
                              select new
                              {
                                  ab.cd_aditamento,
                                  ab.cd_aditamento_bolsa,
                                  ab.cd_motivo_bolsa,
                                  ab.dc_validade_bolsa,
                                  ab.pc_bolsa,
                                  ab.dt_comunicado_bolsa,
                                  ab.MotivoBolsa.dc_motivo_bolsa
                              }).ToList().Select(x => new AditamentoBolsa { 
                                  cd_aditamento = x.cd_aditamento,
                                  cd_aditamento_bolsa = x.cd_aditamento_bolsa,
                                  cd_motivo_bolsa = x.cd_motivo_bolsa,
                                  dc_validade_bolsa = x.dc_validade_bolsa,
                                  pc_bolsa = x.pc_bolsa,
                                  dt_comunicado_bolsa = x.dt_comunicado_bolsa,
                                  MotivoBolsa = new MotivoBolsa { dc_motivo_bolsa = x.dc_motivo_bolsa }
                              });
                  foreach (Aditamento ad in sql.Where(x => x.id_tipo_aditamento == (int)Aditamento.TipoAditamento.ADITIVO_BOLSA))
                      ad.AditamentoBolsa = sql3.Where(x => x.cd_aditamento == ad.cd_aditamento).ToList();
              }

                return sql;

            }
            catch (Exception exe)
            {

                throw new DataAccessException(exe);
            }
        }

        public Aditamento getPenultimoAditamentoByContrato(int cd_contrato, int cd_escola, DateTime dataUltimoAdt)
        {
            try
            {
                var sql = (from aditamento in db.Aditamento
                           where aditamento.cd_contrato == cd_contrato
                             && aditamento.Contrato.cd_pessoa_escola == cd_escola
                             && aditamento.dt_inicio_aditamento <= dataUltimoAdt
                           orderby aditamento.dt_aditamento descending
                           select aditamento).FirstOrDefault();
                return sql;

            }
            catch (Exception exe)
            {

                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Aditamento> getAditamentosByIds(int cd_contrato, int cd_escola, List<int> cdAditamentos)
        {
            try
            {
                var sql = from aditamento in db.Aditamento
                           where aditamento.cd_contrato == cd_contrato
                             && aditamento.Contrato.cd_pessoa_escola == cd_escola
                             && cdAditamentos.Contains(aditamento.cd_aditamento)
                           select aditamento;
                return sql;
            }
            catch (Exception exe)
            {

                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Aditamento> getAllDataAditamentosByContrato(int cd_contrato, int cd_escola)
        {
            try
            {
                var sql = from aditamento in db.Aditamento
                          where aditamento.cd_contrato == cd_contrato
                            && aditamento.Contrato.cd_pessoa_escola == cd_escola
                          select aditamento;
                           
                return sql;

            }
            catch (Exception exe)
            {

                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<DescontoContrato> getDescontosAplicadosAditamento(int cd_contrato, int cd_escola)
        {
            try
            {
                List<DescontoContrato> descontos = new List<DescontoContrato>();
                descontos = (from d in db.DescontoContrato
                             where db.Aditamento.Any(aditamento => aditamento.cd_contrato == cd_contrato && aditamento.Contrato.cd_pessoa_escola == cd_escola &&
                                                                          aditamento.id_tipo_aditamento != null && aditamento.id_tipo_aditamento != (int)Aditamento.TipoAditamento.ADICIONAR_PARCELAS) &&
                                    d.cd_aditamento == db.Aditamento.Where(aditamento => aditamento.cd_contrato == cd_contrato && aditamento.Contrato.cd_pessoa_escola == cd_escola &&
                                                                          aditamento.id_tipo_aditamento != null && aditamento.id_tipo_aditamento != (int)Aditamento.TipoAditamento.ADICIONAR_PARCELAS)
                                                                          .OrderByDescending(x => x.dt_aditamento).FirstOrDefault().cd_aditamento
                             select new
                             {
                                 d.dc_desconto_contrato
                             }).ToList().Select(x => new DescontoContrato
                             {
                                 dc_tipo_desconto = x.dc_desconto_contrato
                             }).ToList();
                if (descontos == null || descontos.Count() <= 0)
                    descontos = (from d in db.DescontoContrato
                                 where d.cd_contrato == cd_contrato && d.Contratos.cd_pessoa_escola == cd_escola
                                 select new
                                 {
                                     d.dc_desconto_contrato
                                 }).ToList().Select(x => new DescontoContrato
                                 {
                                     dc_tipo_desconto = x.dc_desconto_contrato
                                 }).ToList();
                return descontos;
            }
            catch (Exception exe)
            {

                throw new DataAccessException(exe);
            }
        }

        public Decimal getValorMaterialImpressaoContrato(int cd_contrato, int cd_escola)
        {
            try
            {
                var sql = (from aditamento in db.Contrato //LBM Os valores de material passaram para a tabela de contrato
                           where aditamento.cd_contrato == cd_contrato && aditamento.cd_pessoa_escola == cd_escola &&
                                 (bool)aditamento.id_incorporar_valor_material && (decimal)aditamento.vl_material_contrato > 0
                           //orderby aditamento.dt_aditamento descending
                           select (decimal)aditamento.vl_material_contrato).FirstOrDefault();

                return sql;

            }
            catch (Exception exe)
            {

                throw new DataAccessException(exe);
            }
        }

        public bool verificaAditamentoAposReajusteAnual(int cd_empresa, int cd_reajuste_anual)
        {
            try
            {
                var sql = (from aditamento in db.Aditamento
                           where aditamento.Contrato.cd_pessoa_escola == cd_empresa &&
                                 aditamento.cd_reajuste_anual == cd_reajuste_anual &&
                                 aditamento.id_tipo_aditamento == (int)Aditamento.TipoAditamento.REAJUSTE_ANUAL &&
                                 db.Aditamento.Any(x=> x.Contrato.cd_pessoa_escola == cd_empresa && 
                                                        x.cd_contrato == aditamento.cd_contrato &&
                                                        x.cd_aditamento > aditamento.cd_aditamento)
                           select aditamento.cd_aditamento).Any();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Aditamento> getAditamentosByIdsContrato(int cd_escola, int cd_reajuste_anual)
        {
            try
            {
                var sql = from at in db.Aditamento
                          where
                            at.Contrato.cd_pessoa_escola == cd_escola && 
                            at.cd_reajuste_anual == cd_reajuste_anual && 
                            at.id_tipo_aditamento == (int)Aditamento.TipoAditamento.REAJUSTE_ANUAL
                          select at;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<DescontoContrato> getDescontosAplicadosContratoOrAditamento(int cd_contrato, int cd_escola)
        {
            try
            {
                List<DescontoContrato> descontos = new List<DescontoContrato>();
                descontos = (from d in db.DescontoContrato
                             where d.id_desconto_ativo == true &&
                                   db.Aditamento.Any(aditamento => aditamento.cd_contrato == cd_contrato && aditamento.Contrato.cd_pessoa_escola == cd_escola &&
                                                                          aditamento.id_tipo_aditamento != null && aditamento.id_tipo_aditamento != (int)Aditamento.TipoAditamento.ADICIONAR_PARCELAS) &&
                                    d.cd_aditamento == db.Aditamento.Where(aditamento => aditamento.cd_contrato == cd_contrato && aditamento.Contrato.cd_pessoa_escola == cd_escola &&
                                                                          aditamento.id_tipo_aditamento != null && aditamento.id_tipo_aditamento != (int)Aditamento.TipoAditamento.ADICIONAR_PARCELAS)
                                                                          .OrderByDescending(x => x.dt_aditamento).FirstOrDefault().cd_aditamento
                             select new
                             {
                                 d.dc_desconto_contrato,
                                 d.pc_desconto_contrato,
                                 d.vl_desconto_contrato,
                                 d.nm_parcela_ini,
                                 d.nm_parcela_fim
                             }).ToList().Select(x => new DescontoContrato
                             {
                                 dc_tipo_desconto = x.dc_desconto_contrato,
                                 pc_desconto_contrato = x.pc_desconto_contrato,
                                 vl_desconto_contrato = x.vl_desconto_contrato,
                                 nm_parcela_ini = x.nm_parcela_ini,
                                 nm_parcela_fim = x.nm_parcela_fim
                             }).ToList();
                if (descontos == null || descontos.Count() <= 0)
                    descontos = (from d in db.DescontoContrato
                                 where d.cd_contrato == cd_contrato && d.Contratos.cd_pessoa_escola == cd_escola &&
                                       d.id_desconto_ativo == true
                                 select new
                                 {
                                     d.dc_desconto_contrato,
                                     d.pc_desconto_contrato,
                                     d.vl_desconto_contrato,
                                     d.nm_parcela_ini,
                                     d.nm_parcela_fim
                                 }).ToList().Select(x => new DescontoContrato
                                 {
                                     dc_tipo_desconto = x.dc_desconto_contrato,
                                     pc_desconto_contrato = x.pc_desconto_contrato,
                                     vl_desconto_contrato = x.vl_desconto_contrato,
                                     nm_parcela_ini = x.nm_parcela_ini,
                                     nm_parcela_fim = x.nm_parcela_fim
                                 }).ToList();
                return descontos;

            }
            catch (Exception exe)
            {

                throw new DataAccessException(exe);
            }
        }


    }
}
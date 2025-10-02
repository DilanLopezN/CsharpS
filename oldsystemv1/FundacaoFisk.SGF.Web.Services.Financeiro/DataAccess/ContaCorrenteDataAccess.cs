using System;
using System.Collections.Generic;
using System.Linq;
using Componentes.GenericDataAccess;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Financeiro.Model;
using System.Data.Entity;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess
{
    public class ContaCorrenteDataAccess : GenericRepository<ContaCorrente>, IContaCorrenteDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public List<ContaCorrente> getContaCorrenteByBaixa(int cd_baixa_titulo, int cd_pessoa_empresa)
        {
            try
            {
                var sql = (from cc in db.ContaCorrente
                          where cc.cd_baixa_titulo == cd_baixa_titulo && cc.cd_pessoa_empresa == cd_pessoa_empresa
                          select cc);

                return sql.ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }


        public IEnumerable<ContaCorrenteUI> getContaCorreteSearch(SearchParameters parametros, int cd_pessoa_escola, int cd_origem, int cd_destino, byte entraSaida, int cd_movimento,
            int cd_plano_conta, DateTime? dta_ini, DateTime? dta_fim, int cd_pessoa_usuario, bool contaSegura)
        {
            try
            {
                IQueryable<ContaCorrenteUI> sql;
                IQueryable<ContaCorrente> sql1;
                IEntitySorter<ContaCorrenteUI> sorter = EntitySorter<ContaCorrenteUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
           
                if (cd_pessoa_usuario <= 0)
                    sql1 = from conta in db.ContaCorrente.AsNoTracking()
                           where conta.cd_pessoa_empresa == cd_pessoa_escola
                              && conta.dta_conta_corrente >= dta_ini
                              && conta.dta_conta_corrente <= dta_fim
                           select conta;
                else
                    sql1 = from conta in db.ContaCorrente.Include(p => p.PlanoContas)
                           where conta.cd_pessoa_empresa == cd_pessoa_escola
                              && conta.dta_conta_corrente >= dta_ini
                              && conta.dta_conta_corrente <= dta_fim
                              && (
                                   ((conta.LocalMovtoOrigem.cd_pessoa_local == cd_pessoa_usuario && conta.LocalMovtoOrigem.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.CAIXA) ||
                                    (conta.LocalMovtoOrigem.cd_pessoa_local == null))
                              ||
                                  conta.cd_local_destino > 0 && ((conta.LocalMovtoDestino.cd_pessoa_local == cd_pessoa_usuario && conta.LocalMovtoDestino.nm_tipo_local == (int)LocalMovto.TipoLocalMovtoEnum.CAIXA) ||
                                   (conta.LocalMovtoDestino.cd_pessoa_local == null && conta.LocalMovtoDestino.nm_tipo_local != (int)LocalMovto.TipoLocalMovtoEnum.CAIXA))
                                 )
                           select conta;

                //Usuário não possui permissão de conta segura 
                //cd_movimento = 3 (Transferência, não entra na condição pois não possui relacionamento com plano de contas.)
                if (!contaSegura && cd_movimento != 3)
                    sql1 = from conta in sql1
                           where conta.PlanoContas.cd_pessoa_empresa == cd_pessoa_escola && !conta.PlanoContas.id_conta_segura
                           select conta;

                if (cd_origem > 0)
                    sql1 = from conta in sql1
                           where conta.cd_local_origem == cd_origem
                           select conta;

                if (cd_destino > 0)
                    sql1 = from conta in sql1
                           where conta.cd_local_destino == cd_destino
                           select conta;

                if (cd_movimento > 0)
                    sql1 = from conta in sql1
                           where conta.cd_movimentacao_financeira == cd_movimento
                           select conta;

                if (cd_plano_conta > 0)
                    sql1 = from conta in sql1
                           where conta.cd_plano_conta == cd_plano_conta
                           select conta;


                if (entraSaida != 0)
                    sql1 = from conta in sql1
                           where conta.id_tipo_movimento == entraSaida
                           select conta;                

                sql = from contaC in sql1
                      select new ContaCorrenteUI
                      {
                          cd_conta_corrente = contaC.cd_conta_corrente,
                          cd_movimentacao_financeira = contaC.cd_movimentacao_financeira,
                          cd_local_destino = contaC.cd_local_destino,
                          cd_local_origem = contaC.cd_local_origem,
                          cd_tipo_liquidacao = contaC.cd_tipo_liquidacao,
                          id_tipo_movimento = contaC.id_tipo_movimento,
                          movimento = contaC.MovimentacaoFinanceira.dc_movimentacao_financeira,
                          dta_conta_corrente = contaC.dta_conta_corrente,
                          vl_conta_corrente = contaC.vl_conta_corrente,
                          cd_plano_conta = contaC.cd_plano_conta,
                          planoConta = contaC.PlanoContas.PlanoContaSubgrupo.no_subgrupo_conta,
                          dc_obs_conta_corrente = contaC.dc_obs_conta_corrente,
                          localOrigem = contaC.LocalMovtoOrigem ,
                          localDestino = contaC.LocalMovtoDestino
                      };

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

        public IEnumerable<ContaCorrenteUI> rtpContaCorrente(int cd_pessoa_escola, int cd_local_movimento, DateTime dta_inicial, DateTime dta_final, decimal saldo_inicial, int tipoLiquidacao, bool contaSegura, bool isMaster)
        {
            try
            {
                IQueryable<ContaCorrenteUI> sql;

                if (tipoLiquidacao == 0){
                    sql = (from conta in db.ContaCorrente
                           where conta.cd_pessoa_empresa == cd_pessoa_escola
                               && conta.dta_conta_corrente >= dta_inicial
                               && conta.dta_conta_corrente <= dta_final
                               && conta.cd_local_origem == cd_local_movimento
                               && ((isMaster == true || conta.cd_plano_conta == null) || (!contaSegura ? conta.PlanoContas.id_conta_segura == false : contaSegura == true))
                           select new ContaCorrenteUI
                           {
                               cd_conta_corrente = conta.cd_conta_corrente,
                               cd_local_origem = conta.cd_local_origem,
                               dta_conta_corrente = conta.dta_conta_corrente,
                               id_tipo_movimento = conta.id_tipo_movimento,
                               vl_conta_corrente = conta.vl_conta_corrente,
                               dc_obs_conta_corrente = conta.dc_obs_conta_corrente,
                               no_pessoa_local = conta.Empresa.no_pessoa,
                               nm_tipo_local = conta.LocalMovtoOrigem.nm_tipo_local,
                               vl_entrada = conta.id_tipo_movimento == (int)ContaCorrente.Tipo.ENTRADA ? conta.vl_conta_corrente : 0,
                               vl_saida = conta.id_tipo_movimento == (int) ContaCorrente.Tipo.SAIDA ? conta.vl_conta_corrente : 0,
                               saldo_inicial = saldo_inicial,
                               cd_pessoa_escola = cd_pessoa_escola,
                               cd_local_movimento = cd_local_movimento,
                               des_local_movto = conta.LocalMovtoOrigem.no_local_movto,
                               cd_movimentacao_financeira = conta.cd_movimentacao_financeira,
                               dc_tipo_liquidacao = conta.TipoLiquidacao.dc_tipo_liquidacao
                           }).Concat(from conta in db.ContaCorrente
                                     where conta.cd_pessoa_empresa == cd_pessoa_escola
                                         && conta.dta_conta_corrente >= dta_inicial
                                         && conta.dta_conta_corrente <= dta_final
                                         && conta.cd_local_destino == cd_local_movimento
                                     select new ContaCorrenteUI
                                     {
                                         cd_conta_corrente = conta.cd_conta_corrente,
                                         cd_local_origem = conta.cd_local_origem,
                                         dta_conta_corrente = conta.dta_conta_corrente,
                                         id_tipo_movimento = conta.id_tipo_movimento == (Byte) ContaCorrente.Tipo.ENTRADA ? (Byte) ContaCorrente.Tipo.SAIDA : (Byte) ContaCorrente.Tipo.ENTRADA,
                                         vl_conta_corrente = conta.vl_conta_corrente,
                                         dc_obs_conta_corrente = conta.dc_obs_conta_corrente,
                                         no_pessoa_local = conta.Empresa.no_pessoa,
                                         nm_tipo_local = conta.LocalMovtoOrigem.nm_tipo_local,
                                         vl_entrada = conta.id_tipo_movimento == (int) ContaCorrente.Tipo.SAIDA ? conta.vl_conta_corrente : 0,
                                         vl_saida = conta.id_tipo_movimento == (int) ContaCorrente.Tipo.ENTRADA ? conta.vl_conta_corrente : 0,
                                         saldo_inicial = saldo_inicial,
                                         cd_pessoa_escola = cd_pessoa_escola,
                                         cd_local_movimento = cd_local_movimento,
                                         des_local_movto = conta.LocalMovtoDestino.no_local_movto,
                                         cd_movimentacao_financeira = conta.cd_movimentacao_financeira,
                                         dc_tipo_liquidacao = conta.TipoLiquidacao.dc_tipo_liquidacao
                                     }).OrderBy(x => x.dta_conta_corrente).ThenBy(y => y.cd_movimentacao_financeira == (int)MovimentacaoFinanceira.TipoMovimentacaoFinanceira.ABERTURA_SALDO ? 0 : y.id_tipo_movimento); 
                }
                else
                {
                    sql = (from conta in db.ContaCorrente
                           where conta.cd_pessoa_empresa == cd_pessoa_escola
                               && conta.dta_conta_corrente >= dta_inicial
                               && conta.dta_conta_corrente <= dta_final
                               && conta.cd_local_origem == cd_local_movimento
                               && conta.cd_tipo_liquidacao == tipoLiquidacao
                               && ((isMaster == true || conta.cd_plano_conta == null) || (!contaSegura ? conta.PlanoContas.id_conta_segura == false : contaSegura == true))
                           select new ContaCorrenteUI
                           {
                               cd_conta_corrente = conta.cd_conta_corrente,
                               cd_local_origem = conta.cd_local_origem,
                               dta_conta_corrente = conta.dta_conta_corrente,
                               id_tipo_movimento = conta.id_tipo_movimento,
                               vl_conta_corrente = conta.vl_conta_corrente,
                               dc_obs_conta_corrente = conta.dc_obs_conta_corrente,
                               no_pessoa_local = conta.Empresa.no_pessoa,
                               nm_tipo_local = conta.LocalMovtoOrigem.nm_tipo_local,
                               vl_entrada = conta.id_tipo_movimento == (int) ContaCorrente.Tipo.ENTRADA ? conta.vl_conta_corrente : 0,
                               vl_saida = conta.id_tipo_movimento == (int) ContaCorrente.Tipo.SAIDA ? conta.vl_conta_corrente : 0,
                               saldo_inicial = saldo_inicial,
                               cd_pessoa_escola = cd_pessoa_escola,
                               cd_local_movimento = cd_local_movimento,
                               des_local_movto = conta.LocalMovtoOrigem.no_local_movto,
                               cd_movimentacao_financeira = conta.cd_movimentacao_financeira,
                               dc_tipo_liquidacao = conta.TipoLiquidacao.dc_tipo_liquidacao
                           }).Concat(from conta in db.ContaCorrente
                                     where conta.cd_pessoa_empresa == cd_pessoa_escola
                                         && conta.dta_conta_corrente >= dta_inicial
                                         && conta.dta_conta_corrente <= dta_final
                                         && conta.cd_local_destino == cd_local_movimento
                                         && (conta.cd_tipo_liquidacao == tipoLiquidacao)
                                     select new ContaCorrenteUI
                                     {
                                         cd_conta_corrente = conta.cd_conta_corrente,
                                         cd_local_origem = conta.cd_local_origem,
                                         dta_conta_corrente = conta.dta_conta_corrente,
                                         id_tipo_movimento = conta.id_tipo_movimento == (Byte) ContaCorrente.Tipo.ENTRADA ? (Byte) ContaCorrente.Tipo.SAIDA : (Byte) ContaCorrente.Tipo.ENTRADA,
                                         vl_conta_corrente = conta.vl_conta_corrente,
                                         dc_obs_conta_corrente = conta.dc_obs_conta_corrente,
                                         no_pessoa_local = conta.Empresa.no_pessoa,
                                         nm_tipo_local = conta.LocalMovtoOrigem.nm_tipo_local,
                                         vl_entrada = conta.id_tipo_movimento == (int) ContaCorrente.Tipo.SAIDA ? conta.vl_conta_corrente : 0,
                                         vl_saida = conta.id_tipo_movimento == (int) ContaCorrente.Tipo.ENTRADA ? conta.vl_conta_corrente : 0,
                                         saldo_inicial = saldo_inicial,
                                         cd_pessoa_escola = cd_pessoa_escola,
                                         cd_local_movimento = cd_local_movimento,
                                         des_local_movto = conta.LocalMovtoDestino.no_local_movto,
                                         cd_movimentacao_financeira = conta.cd_movimentacao_financeira,
                                         dc_tipo_liquidacao = conta.TipoLiquidacao.dc_tipo_liquidacao
                                     }).OrderBy(x => x.dta_conta_corrente).ThenBy(y => y.cd_movimentacao_financeira == (int)MovimentacaoFinanceira.TipoMovimentacaoFinanceira.ABERTURA_SALDO ? 0 : y.id_tipo_movimento);

                }
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Decimal fcSaldoContaCorrente(int cd_pessoa_escola, int cd_local_movimento, DateTime dta_saldo, int tipoLiquidacao)
        {
            try
            {
                DateTime dta_base = DateTime.Parse("01/01/1900");// Menor valor p/Smalldatetime
                decimal saldoIni = 0;
                decimal saldoIniSaida = 0;
                DateTime? dataAberturaS = null;
                DateTime? dta_abertura = (from conta in db.ContaCorrente
                                where conta.cd_pessoa_empresa == cd_pessoa_escola &&
                                conta.LocalMovtoOrigem.cd_local_movto == cd_local_movimento &&
                                (conta.cd_tipo_liquidacao == tipoLiquidacao || tipoLiquidacao == 0) &&
                                conta.cd_movimentacao_financeira == (int)MovimentacaoFinanceira.TipoMovimentacaoFinanceira.ABERTURA_SALDO &&
                                conta.dta_conta_corrente <= dta_saldo
                                orderby conta.dta_conta_corrente descending
                                select conta.dta_conta_corrente).FirstOrDefault();

                if (dta_abertura != null)
                {
                    var abertura = (from conta in db.ContaCorrente
                                    where conta.cd_pessoa_empresa == cd_pessoa_escola &&
                                    conta.LocalMovtoOrigem.cd_local_movto == cd_local_movimento &&
                                    (conta.cd_tipo_liquidacao == tipoLiquidacao || tipoLiquidacao == 0) &&
                                    conta.cd_movimentacao_financeira == (int)MovimentacaoFinanceira.TipoMovimentacaoFinanceira.ABERTURA_SALDO &&
                                    conta.dta_conta_corrente == dta_abertura
                                    select conta).GroupBy(x => new { x.dta_conta_corrente }).
                                  Select(g => new {
                                      dta_conta_corrente = g.Key.dta_conta_corrente,
                                      vl_conta_corrente = g.Sum(y => y.id_tipo_movimento == (int)ContaCorrente.Tipo.ENTRADA ? y.vl_conta_corrente : (y.vl_conta_corrente * -1))
                                  });

                    if (abertura != null)
                        dataAberturaS = abertura.ToList()[0].dta_conta_corrente;
                    if (dataAberturaS != null)
                    {
                        dta_base = dataAberturaS.Value;
                        saldoIni = (decimal)abertura.ToList()[0].vl_conta_corrente;
                    }
                }
                if (dataAberturaS != null && dataAberturaS == dta_saldo)
                        saldoIni = 0;
                else
                {
                    dta_saldo = dta_saldo.AddDays(-1);
                    if (tipoLiquidacao == 0)
                    {
                        saldoIni = saldoIni + ((db.ContaCorrente.Where(c => c.cd_pessoa_empresa == cd_pessoa_escola
                                                    && (c.dta_conta_corrente >= dta_base && c.dta_conta_corrente <= dta_saldo)
                                                    && c.cd_movimentacao_financeira != (int)MovimentacaoFinanceira.TipoMovimentacaoFinanceira.ABERTURA_SALDO
                                                    && (c.cd_local_origem == cd_local_movimento)).
                                                    Sum(c => c.id_tipo_movimento == (int)ContaCorrente.Tipo.ENTRADA ? c.vl_conta_corrente : (c.vl_conta_corrente * -1))) ?? 0);

                        saldoIniSaida = (db.ContaCorrente.Where(c => c.cd_pessoa_empresa == cd_pessoa_escola
                                                    && (c.dta_conta_corrente >= dta_base && c.dta_conta_corrente <= dta_saldo)
                                                    && c.cd_movimentacao_financeira != (int)MovimentacaoFinanceira.TipoMovimentacaoFinanceira.ABERTURA_SALDO
                                                    && (c.cd_local_destino == cd_local_movimento)).
                                                    Sum(c => c.id_tipo_movimento == (int)ContaCorrente.Tipo.SAIDA ? c.vl_conta_corrente : (c.vl_conta_corrente * -1))) ?? 0;

                    }
                    else
                    {

                        saldoIni = saldoIni + ((db.ContaCorrente.Where(c => c.cd_pessoa_empresa == cd_pessoa_escola
                                                && (c.dta_conta_corrente >= dta_base && c.dta_conta_corrente <= dta_saldo)
                                                && c.cd_movimentacao_financeira != (int)MovimentacaoFinanceira.TipoMovimentacaoFinanceira.ABERTURA_SALDO
                                                && (c.cd_local_origem == cd_local_movimento)
                                                && (c.cd_tipo_liquidacao == tipoLiquidacao)).
                                                Sum(c => c.id_tipo_movimento == (int)ContaCorrente.Tipo.ENTRADA ? c.vl_conta_corrente : (c.vl_conta_corrente * -1))) ?? 0);

                        saldoIniSaida = (db.ContaCorrente.Where(c => c.cd_pessoa_empresa == cd_pessoa_escola
                                                    && (c.dta_conta_corrente >= dta_base && c.dta_conta_corrente <= dta_saldo)
                                                    && c.cd_movimentacao_financeira != (int)MovimentacaoFinanceira.TipoMovimentacaoFinanceira.ABERTURA_SALDO
                                                    && (c.cd_local_destino == cd_local_movimento)
                                                    && (c.cd_tipo_liquidacao == tipoLiquidacao)).
                                                    Sum(c => c.id_tipo_movimento == (int)ContaCorrente.Tipo.SAIDA ? c.vl_conta_corrente : (c.vl_conta_corrente * -1))) ?? 0;

                    }
                        saldoIni = saldoIni + saldoIniSaida;
                }
                return saldoIni;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public ContaCorrenteUI getContaCorrente(int cd_local_movimento, int cd_pessoa_escola)
        {
            try
            {
                var sql = (from conta in db.ContaCorrente
                           where conta.cd_pessoa_empresa == cd_pessoa_escola
                             && conta.LocalMovtoOrigem.cd_local_movto == cd_local_movimento
                           select new ContaCorrenteUI
                           {
                               no_pessoa_local = conta.Empresa.no_pessoa,
                               movimento = conta.LocalMovtoOrigem.no_local_movto
                           }).FirstOrDefault();

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<SubgrupoConta> getGruposSemContaCorrenteN1(DateTime data_inicial, DateTime data_final, int cd_empresa, bool conta_segura) {
            try {
                var sql = (from sg in db.SubgrupoConta
                           where !sg.SubgruposFilhos.Any() && sg.cd_subgrupo_pai == null
                                  && sg.SubgrupoPlanoConta.Where(sp => sp.cd_pessoa_empresa == cd_empresa).Any()
                           select new {
                               cd_subgrupo_conta = sg.cd_subgrupo_conta,
                               no_subgrupo_conta = sg.no_subgrupo_conta,
                               cd_grupo_conta = sg.SubgrupoContaGrupo.cd_grupo_conta,
                               no_grupo_conta = sg.SubgrupoContaGrupo.no_grupo_conta,
                               SubgrupoPlanoConta = sg.SubgrupoPlanoConta.Where(spc =>
                                   spc.cd_pessoa_empresa == cd_empresa
                                   )
                           }).ToList().Select(x => new SubgrupoConta {
                               cd_subgrupo_conta = x.cd_subgrupo_conta,
                               no_subgrupo_conta = x.no_subgrupo_conta,
                               cd_grupo_conta = x.cd_grupo_conta,
                               SubgrupoContaGrupo = new GrupoConta {no_grupo_conta = x.no_grupo_conta, cd_grupo_conta = x.cd_grupo_conta},
                               SubgrupoPlanoConta = x.SubgrupoPlanoConta.ToList(),
                           });

                //TODO: Verificar se é possível buscar somente o valor da conta corrente para associar na consulta acima:
                var sqlContaCorrente = (from cc in db.ContaCorrente
                                       where cc.dta_conta_corrente >= data_inicial
                                        && cc.dta_conta_corrente <= data_final
                                        && cc.cd_pessoa_empresa == cd_empresa
                                        && (conta_segura || !cc.PlanoContas.id_conta_segura)
                                       select cc).ToList();
                return sql;
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<SubgrupoConta> getGruposSemContaCorrenteN2(DateTime data_inicial, DateTime data_final, int cd_empresa, bool conta_segura) {
            try {
                var sql = (from sg in db.SubgrupoConta
                           where !sg.SubgruposFilhos.Any() && sg.cd_grupo_conta == null
                                 && sg.SubgrupoPlanoConta.Where(sp => sp.cd_pessoa_empresa == cd_empresa).Any()
                           select new {
                               cd_subgrupo_conta = sg.cd_subgrupo_conta,
                               no_subgrupo_conta = sg.no_subgrupo_conta,
                               SubgrupoPlanoConta = sg.SubgrupoPlanoConta.Where(spc =>
                                   spc.cd_pessoa_empresa == cd_empresa),
                               SubgrupoPai = sg.SubgrupoPai,
                               SubgrupoContaGrupo = sg.SubgrupoPai.SubgrupoContaGrupo,
                               SubgruposFilhos = sg.SubgrupoPai.SubgruposFilhos.Where(sf => sf.SubgrupoPlanoConta.Where(sp => sp.cd_pessoa_empresa == cd_empresa).Any())
                           }).ToList().Select(x => new SubgrupoConta {
                               cd_subgrupo_conta = x.cd_subgrupo_conta,
                               no_subgrupo_conta = x.no_subgrupo_conta,
                               SubgrupoPlanoConta = x.SubgrupoPlanoConta.ToList(),
                               SubgrupoPai = x.SubgrupoPai,
                               SubgruposFilhos = x.SubgruposFilhos.ToList()
                           });


                //TODO: Verificar se é possível buscar somente o valor da conta corrente para associar na consulta acima:
                var sqlContaCorrente = (from cc in db.ContaCorrente
                                        where cc.dta_conta_corrente >= data_inicial
                                         && cc.dta_conta_corrente <= data_final
                                         && cc.cd_pessoa_empresa == cd_empresa
                                         && (conta_segura || !cc.PlanoContas.id_conta_segura)
                                        select cc).ToList();

                return sql;
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }
        
        public decimal buscaSaldoAnteriorGrupo(int cd_escola, int cd_grupo_conta, DateTime data_anterior) {
            try {
                DateTime data_inicial = new DateTime(1900, 1, 1);
                
                var sql = (from cc in db.ContaCorrente
                           where (cc.PlanoContas.PlanoContaSubgrupo.cd_grupo_conta == cd_grupo_conta || cc.PlanoContas.PlanoContaSubgrupo.SubgrupoPai.cd_grupo_conta == cd_grupo_conta)
                                        && cc.dta_conta_corrente >= data_inicial
                                        && cc.dta_conta_corrente <= data_anterior
                                        && cc.cd_pessoa_empresa == cd_escola
                           select new {
                               vl_conta_corrente = cc.vl_conta_corrente,
                               id_tipo_movimento = cc.id_tipo_movimento,
                               cd_local_destino = cc.cd_local_destino
                           }).ToList().Select(x => x.vl_conta_corrente * (x.cd_local_destino.HasValue ? 0 : (x.id_tipo_movimento == (byte)ContaCorrente.Tipo.ENTRADA ? 1 : -1))).Sum();
                if(sql.HasValue)
                    return sql.Value;
                else
                    return 0;
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

        public decimal buscaSaldoAnteriorSubGrupo(int cd_escola, int cd_subgrupo_conta, DateTime data_anterior) {
            try {
                DateTime data_inicial = new DateTime(1900, 1, 1);

                var sql = (from cc in db.ContaCorrente
                           where (cc.PlanoContas.PlanoContaSubgrupo.cd_subgrupo_conta == cd_subgrupo_conta || cc.PlanoContas.PlanoContaSubgrupo.SubgrupoPai.cd_subgrupo_conta == cd_subgrupo_conta)
                                        && cc.dta_conta_corrente >= data_inicial
                                        && cc.dta_conta_corrente <= data_anterior
                                        && cc.cd_pessoa_empresa == cd_escola
                           select new {
                               vl_conta_corrente = cc.vl_conta_corrente,
                               id_tipo_movimento = cc.id_tipo_movimento,
                               cd_local_destino = cc.cd_local_destino
                           }).ToList().Select(x => x.vl_conta_corrente * (x.cd_local_destino.HasValue ? 0 :  (x.id_tipo_movimento == (byte) ContaCorrente.Tipo.ENTRADA ? 1 : -1))).Sum();
                if(sql.HasValue)
                    return sql.Value;
                else
                    return 0;
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<ContaCorrente> getObservacoesCCBaixa(int? cd_baixa_titulo, int? cd_conta_corrente)
        {
            try {
                IEnumerable<ContaCorrente> sql = from cc in db.ContaCorrente
                          select cc;
                
                if(cd_baixa_titulo.HasValue)
                    sql = (from cc in db.ContaCorrente
                           where cc.cd_baixa_titulo == cd_baixa_titulo.Value
                           select new {
                               dc_obs_conta_corrente = cc.dc_obs_conta_corrente,
                               tx_obs_baixa = cc.BaixaTitulo != null ? cc.BaixaTitulo.tx_obs_baixa : null
                           }).ToList().Select(x => new ContaCorrente {
                               dc_obs_conta_corrente = x.dc_obs_conta_corrente + "  " + x.tx_obs_baixa
                           });
                else
                    sql = (from cc in db.ContaCorrente
                           where cc.cd_conta_corrente == cd_conta_corrente.Value
                           select new
                           {
                               dc_obs_conta_corrente = cc.dc_obs_conta_corrente,
                               tx_obs_baixa = cc.BaixaTitulo != null ? cc.BaixaTitulo.tx_obs_baixa : null
                           }).ToList().Select(x => new ContaCorrente
                           {
                               dc_obs_conta_corrente = x.dc_obs_conta_corrente + "  " + x.tx_obs_baixa
                           });

                return sql;
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<RptRecebidaPaga> recebidaPagaStoreProcedure(int cdEscola, DateTime pDtaI, DateTime pDtaF, int pForn, byte pNatureza, int pPlanoContas, bool cSegura, int cdTpLiq, int cdTpFinan,  int tipo, string situacoes, int cdTurma, int cdLocal)
        {
            try
            {
                SGFWebContext dbComp = new SGFWebContext();
                int cd_origem = Int32.Parse(dbComp.LISTA_ORIGEM_LOGS["Contrato"].ToString());

                string[] situacao = situacoes.Split('|');
                List<int> cdsSituacoesList = new List<int>();
                for (int i = 0; i < situacao.Count(); i++)
                    cdsSituacoesList.Add(Int32.Parse(situacao[i]));

                var sqlContasCorrentes = (from c in db.ContaCorrente
                    //where DbFunctions.TruncateTime(c.dta_conta_corrente) >= pDtaI.Date && DbFunctions.TruncateTime(c.dta_conta_corrente) <= pDtaF.Date
                    where c.dta_conta_corrente >= pDtaI.Date && c.dta_conta_corrente <= pDtaF.Date
                            && c.cd_pessoa_empresa == cdEscola
                            && (c.cd_plano_conta == pPlanoContas || pPlanoContas == 0)
                            && c.id_tipo_movimento == pNatureza
                            && c.cd_baixa_titulo == null
                            && (c.cd_movimentacao_financeira == (byte) MovimentacaoFinanceira.TipoMovimentacaoFinanceira.PAGAMENTO || c.cd_movimentacao_financeira == (byte) MovimentacaoFinanceira.TipoMovimentacaoFinanceira.RECEBIMENTO)
                            && ((!cSegura && c.PlanoContas.id_conta_segura == false) || cSegura)
                            && (c.cd_tipo_liquidacao == cdTpLiq || cdTpLiq == 0)
                            && (c.BaixaTitulo.Titulo.cd_tipo_financeiro == cdTpFinan || cdTpFinan == 0)
                            && (c.BaixaTitulo.cd_local_movto == cdLocal || cdLocal == 0)
                                          select c);

                if (tipo == (int)Titulo.TipoMovimento.RECEBIDAS)
                {
                    if (cdTurma > 0)
                    {

                        sqlContasCorrentes = from b in sqlContasCorrentes
                                    where
                                     (from h in db.HistoricoAluno
                                      where h.Aluno.cd_pessoa_aluno == b.BaixaTitulo.Titulo.cd_pessoa_titulo &&
                                            h.cd_contrato == b.BaixaTitulo.Titulo.cd_origem_titulo &&
                                            b.BaixaTitulo.Titulo.id_origem_titulo == cd_origem &&
                                            h.dt_historico <= pDtaF &&
                                            h.cd_turma == cdTurma &&
                                            h.nm_sequencia == db.HistoricoAluno.Where(han => han.cd_aluno == h.cd_aluno && han.cd_contrato == h.cd_contrato
                                                                                                                        && DbFunctions.TruncateTime(han.dt_historico) <= pDtaF
                                            ).Max(x => x.nm_sequencia)
                                      select h.cd_turma).Any()
                                    select b;
                    }

                    if (!cdsSituacoesList.Contains(100) && cdsSituacoesList.Count() > 0 && cdTpFinan > 0)
                    {
                        sqlContasCorrentes = from b in sqlContasCorrentes
                                    where cdsSituacoesList.Contains((from histA in db.HistoricoAluno
                                                                     where histA.Aluno.cd_pessoa_escola == cdEscola &&
                                                                           histA.Aluno.cd_pessoa_aluno == b.BaixaTitulo.Titulo.cd_pessoa_titulo &&
                                                                           histA.dt_historico <= pDtaF
                                                                     orderby histA.nm_sequencia descending
                                                                     select histA.id_situacao_historico).FirstOrDefault())
                                    select b;
                    }

                }
                IEnumerable<RptRecebidaPaga> sql = (from c in sqlContasCorrentes
                                                    select new {
                                                        cd_conta_corrente = c.cd_conta_corrente,
                                                        cd_pessoa_empresa = c.cd_pessoa_empresa,
                                                        nm_escola = c.Empresa.no_pessoa,
                                                        dt_vcto_titulo = c.dta_conta_corrente,
                                                        vl_saldo_titulo = c.vl_conta_corrente,
                                                        vl_titulo = c.vl_conta_corrente,
                                                        dt_baixa_titulo = c.dta_conta_corrente,
                                                        vl_principal_baixa = c.vl_conta_corrente,
                                                        id_tipo_movimento = c.id_tipo_movimento,
                                                        dc_tipo_liquidacao = c.TipoLiquidacao.dc_tipo_liquidacao,
                                                        cd_subgrupo_conta = c.PlanoContas != null ? c.PlanoContas.PlanoContaSubgrupo.cd_subgrupo_conta : 0,
                                                        no_subgrupo_conta = c.PlanoContas != null ? c.PlanoContas.PlanoContaSubgrupo.no_subgrupo_conta : "",
                                                        vl_liquidacao_baixa = (decimal)c.vl_conta_corrente,
                                                        cd_turma = (tipo == (int)Titulo.TipoMovimento.RECEBIDAS && cdTurma > 0) ?
                                                            (from h in db.HistoricoAluno
                                                                where h.Aluno.cd_pessoa_aluno == c.BaixaTitulo.Titulo.cd_pessoa_titulo &&
                                                                      h.cd_contrato == c.BaixaTitulo.Titulo.cd_origem_titulo &&
                                                                      c.BaixaTitulo.Titulo.id_origem_titulo == cd_origem &&
                                                                      h.cd_turma == cdTurma &&
                                                                      h.dt_historico <= pDtaF &&
                                                                      h.nm_sequencia == db.HistoricoAluno.Where(han => han.cd_aluno == h.cd_aluno && han.cd_contrato == h.cd_contrato
                                                                                                                                                  && DbFunctions.TruncateTime(han.dt_historico) <= pDtaF
                                                                      ).Max(x => x.nm_sequencia)
                                                                select h.cd_turma).FirstOrDefault() : 0,
                                                        tx_obs_baixa = c.dc_obs_conta_corrente

                                                    }).ToList().Select(x => new RptRecebidaPaga {
                                                        cd_conta_corrente = x.cd_conta_corrente,
                                                        cd_pessoa_empresa = x.cd_pessoa_empresa,
                                                        no_pessoa = x.id_tipo_movimento == (int)ContaCorrente.Tipo.ENTRADA ? "Recebimento Manual" : "Pagamento Manual",
                                                        nm_escola = x.nm_escola,
                                                        nm_titulo = null,
                                                        nm_parcela_titulo = null,
                                                        dt_emissao_titulo = null,
                                                        dt_vcto_titulo = x.dt_vcto_titulo,
                                                        dc_tipo_financeiro = "Manual",
                                                        vl_saldo_titulo = x.vl_saldo_titulo.HasValue ? x.vl_saldo_titulo.Value : 0,
                                                        vl_titulo = x.vl_titulo,
                                                        dt_baixa_titulo = x.dt_baixa_titulo,
                                                        vl_principal_baixa = x.vl_principal_baixa,
                                                        dc_tipo_liquidacao = x.dc_tipo_liquidacao,
                                                        cd_subgrupo_conta = x.cd_subgrupo_conta,
                                                        no_subgrupo_conta = x.no_subgrupo_conta,
                                                        vl_liquidacao_baixa = x.vl_liquidacao_baixa,
                                                        no_turma = x.cd_turma > 0 ? (from t in db.Turma where (t.cd_turma == x.cd_turma || t.cd_turma_ppt == x.cd_turma ) select t.no_turma).FirstOrDefault() : "",
                                                        cd_turma = x.cd_turma,
                                                        tx_obs_baixa = x.tx_obs_baixa
                                                    });
                return sql;
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

        public List<SaldoFinanceiro> rtpSaldoFinanceiro(int cd_empresa, DateTime dta_base, byte tipoLocal, bool liquidacao)
        {
            try
            {
                int[] tiposLocal = null;
                if (tipoLocal == 0)
                    tiposLocal = new int[] { (int)LocalMovto.TipoLocalMovtoEnum.CAIXA, (int)LocalMovto.TipoLocalMovtoEnum.BANCO };
                else
                    if (tipoLocal == (int)LocalMovto.TipoLocalMovtoEnum.CAIXA)
                    {
                        tiposLocal = new int[] { (int)LocalMovto.TipoLocalMovtoEnum.CAIXA };
                    }
                    else
                        tiposLocal = new int[] { (int)LocalMovto.TipoLocalMovtoEnum.BANCO };
                //Busca contas correntes que tenham local de movimento de origem.
                var sql = (from cc in db.ContaCorrente
                           join lco in db.LocalMovto on cc.cd_local_origem equals lco.cd_local_movto
                           join tl in db.TipoLiquidacao on cc.cd_tipo_liquidacao equals tl.cd_tipo_liquidacao
                           where tiposLocal.Contains(lco.nm_tipo_local) && cc.cd_pessoa_empresa == cd_empresa && DbFunctions.TruncateTime(cc.dta_conta_corrente) == dta_base
                           group cc by new
                           {
                               cd_local = cc.cd_local_origem,
                               cd_tipo_liquidacao = cc.cd_tipo_liquidacao,
                               nm_tipo = cc.LocalMovtoOrigem.nm_tipo_local,
                               no_local = cc.LocalMovtoOrigem.no_local_movto,
                               dc_tipo_liquidacao = cc.TipoLiquidacao.dc_tipo_liquidacao,
                               id_tipo_movimento = cc.id_tipo_movimento
                           } into lco
                           select new SaldoFinanceiro
                           {
                               cd_local = lco.Key.cd_local,
                               nm_tipo = lco.Key.nm_tipo,
                               cd_tipo_liquidacao = lco.Key.cd_tipo_liquidacao,
                               banco = lco.Key.no_local,
                               tipo = lco.Key.dc_tipo_liquidacao,
                               entrada = lco.Key.id_tipo_movimento == (int)ContaCorrente.Tipo.ENTRADA ? lco.Sum(s => s.vl_conta_corrente) : 0,
                               saida = lco.Key.id_tipo_movimento == (int)ContaCorrente.Tipo.SAIDA ? lco.Sum(s => s.vl_conta_corrente) : 0
                           }).Union(
                    //Busca contas correntes que tenham local de movimento de destino.
                                      from cc in db.ContaCorrente
                                      join lcd in db.LocalMovto on cc.cd_local_destino equals lcd.cd_local_movto
                                      join tl in db.TipoLiquidacao on cc.cd_tipo_liquidacao equals tl.cd_tipo_liquidacao
                                      where tiposLocal.Contains(lcd.nm_tipo_local) && cc.cd_pessoa_empresa == cd_empresa && cc.cd_local_destino != null &&
                                            DbFunctions.TruncateTime(cc.dta_conta_corrente) == dta_base
                                      group cc by new
                                      {
                                          cd_local = cc.cd_local_destino,
                                          cd_tipo_liquidacao = cc.cd_tipo_liquidacao,
                                          nm_tipo = lcd.nm_tipo_local,
                                          no_local = lcd.no_local_movto,
                                          dc_tipo_liquidacao = tl.dc_tipo_liquidacao,
                                          id_tipo_movimento = cc.id_tipo_movimento
                                      } into lco
                                      select new SaldoFinanceiro
                                      {
                                          cd_local = lco.Key.cd_local,
                                          nm_tipo = lco.Key.nm_tipo,
                                          cd_tipo_liquidacao = lco.Key.cd_tipo_liquidacao,
                                          banco = lco.Key.no_local,
                                          tipo = lco.Key.dc_tipo_liquidacao,
                                          entrada = lco.Key.id_tipo_movimento == (int)ContaCorrente.Tipo.SAIDA ? lco.Sum(s => s.vl_conta_corrente) : 0,
                                          saida = lco.Key.id_tipo_movimento == (int)ContaCorrente.Tipo.ENTRADA ? lco.Sum(s => s.vl_conta_corrente) : 0
                                      });
                return sql.ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public List<SaldoFinanceiro> rtpLocalMovimentoSemCCorrenteDtaBase(int cd_empresa, DateTime dta_base, byte tipoLocal)
        {
            try
            {
                int[] tiposLocal = null;
                if (tipoLocal == 0)
                    tiposLocal = new int[] { (int)LocalMovto.TipoLocalMovtoEnum.CAIXA, (int)LocalMovto.TipoLocalMovtoEnum.BANCO };
                else
                    if (tipoLocal == (int)LocalMovto.TipoLocalMovtoEnum.CAIXA)
                    {
                        tiposLocal = new int[] { (int)LocalMovto.TipoLocalMovtoEnum.CAIXA };
                    }
                    else
                        tiposLocal = new int[] { (int)LocalMovto.TipoLocalMovtoEnum.BANCO };
                         var sql = from lcm in db.LocalMovto
                                   where (lcm.ContaCorrenteDestino.Any() || lcm.ContaCorrenteOrigem.Any()) && tiposLocal.Contains(lcm.nm_tipo_local) && lcm.cd_pessoa_empresa == cd_empresa
                          select new SaldoFinanceiro
                          {
                              cd_local = lcm.cd_local_movto,
                              nm_tipo = lcm.nm_tipo_local,
                              cd_tipo_liquidacao = 0,
                              banco = lcm.no_local_movto,
                              tipo = "",
                              entrada = 0,
                              saida =0,
                              tiposLiquidacao = from tl in db.TipoLiquidacao where tl.ContasCorrentes.Where(x => x.cd_pessoa_empresa == cd_empresa &&
                                                                                   DbFunctions.TruncateTime(x.dta_conta_corrente) < dta_base && 
                                                                                   (x.cd_local_destino == lcm.cd_local_movto || x.cd_local_origem == lcm.cd_local_movto)).Any() &&
                                                                                   !tl.ContasCorrentes.Where(x => x.cd_pessoa_empresa == cd_empresa &&
                                                                                   DbFunctions.TruncateTime(x.dta_conta_corrente) == dta_base &&
                                                                                   (x.cd_local_destino == lcm.cd_local_movto || x.cd_local_origem == lcm.cd_local_movto)).Any()
                                                select tl
                          };

                return sql.ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public ContaCorrente existAberturaSaldoData(DateTime dtaInicial, int cd_pessoa_escola, int cd_local_movto, int tipoLiquidacao)
        {
            try
            {
                //Verifica se existe abertura de saldo com data maior ou igual a data inicial pedida
                var sql = from conta in db.ContaCorrente
                           where conta.cd_pessoa_empresa == cd_pessoa_escola && 
                           (conta.LocalMovtoOrigem.cd_local_movto == cd_local_movto || cd_local_movto == 0)  &&
                           (conta.cd_tipo_liquidacao == tipoLiquidacao || tipoLiquidacao == 0) && 
                           conta.cd_movimentacao_financeira == (int)MovimentacaoFinanceira.TipoMovimentacaoFinanceira.ABERTURA_SALDO &&
                           conta.dta_conta_corrente > dtaInicial
                           select conta;

                return sql.OrderBy(x => x.dta_conta_corrente).ToList().FirstOrDefault();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public ContaCorrenteUI getContaCorretePlanoConta(int cd_pessoa_escola, int cd_conta_corrente)
        {
            try
            {

                ContaCorrenteUI sql = (from contaC in db.ContaCorrente
                                      where contaC.cd_pessoa_empresa == cd_pessoa_escola 
                                       && contaC.cd_conta_corrente == cd_conta_corrente
                                      select new ContaCorrenteUI
                                      {
                                          cd_conta_corrente = contaC.cd_conta_corrente,
                                          cd_plano_conta = contaC.cd_plano_conta,
                                          planoConta = contaC.PlanoContas.PlanoContaSubgrupo.no_subgrupo_conta
                                      }).FirstOrDefault();

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<ContaCorrenteUI> getFechamentoCaixaTpLiquidacao(int cd_pessoa_escola, DateTime dta_fechamento, int cdUsuario, byte tipoLocal)
        {
            try
            {
                IQueryable<ContaCorrenteUI> sql;
                if (cdUsuario > 0)
                    sql = (from conta in db.ContaCorrente
                           join lco in db.LocalMovto
                           on conta.cd_local_origem equals lco.cd_local_movto
                           where conta.cd_pessoa_empresa == cd_pessoa_escola
                               && conta.dta_conta_corrente <= dta_fechamento
                               && db.UsuarioWebSGF.Where(u => u.cd_usuario == cdUsuario && u.cd_pessoa == conta.LocalMovtoOrigem.cd_pessoa_local).Any() 
                               && lco.nm_tipo_local == tipoLocal //(int)LocalMovto.TipoLocalMovtoEnum.CAIXA
                           group conta by new
                           {
                               cd_tipo_liquidacao = conta.cd_tipo_liquidacao,
                               dc_tipo_liquidacao = conta.TipoLiquidacao.dc_tipo_liquidacao,
                               id_tipo_movimento = conta.id_tipo_movimento,
                               cd_local = conta.cd_local_origem,
                               dta_conta_corrente = conta.dta_conta_corrente
                           } into g
                           select new ContaCorrenteUI
                           {
                               //cd_conta_corrente = g.Key.cd_conta_corrente,
                               cd_tipo_liquidacao = g.Key.cd_tipo_liquidacao,
                               dc_tipo_liquidacao = g.Key.dc_tipo_liquidacao,
                               vl_entrada = g.Key.id_tipo_movimento == (int)ContaCorrente.Tipo.ENTRADA ? g.Sum(s => s.vl_conta_corrente) : 0,
                               vl_saida = g.Key.id_tipo_movimento == (int)ContaCorrente.Tipo.SAIDA ? g.Sum(s => s.vl_conta_corrente) : 0,
                               cd_local_movimento = g.Key.cd_local,
                               dta_conta_corrente = g.Key.dta_conta_corrente
                           }).Union(
                        //Busca contas correntes que tenham local de movimento de destino.
                                                      from conta in db.ContaCorrente
                                                      join lcd in db.LocalMovto
                                                      on conta.cd_local_destino equals lcd.cd_local_movto
                                                      where conta.cd_pessoa_empresa == cd_pessoa_escola
                                                          && conta.dta_conta_corrente <= dta_fechamento
                                                          && db.UsuarioWebSGF.Where(u => u.cd_usuario == cdUsuario && u.cd_pessoa == conta.LocalMovtoDestino.cd_pessoa_local).Any() 
                                                          && lcd.nm_tipo_local == tipoLocal //(int)LocalMovto.TipoLocalMovtoEnum.CAIXA
                                                      group conta by new
                                                      {
                                                          cd_tipo_liquidacao = conta.cd_tipo_liquidacao,
                                                          dc_tipo_liquidacao = conta.TipoLiquidacao.dc_tipo_liquidacao,
                                                          id_tipo_movimento = conta.id_tipo_movimento,
                                                          cd_local = conta.cd_local_destino,
                                                          dta_conta_corrente = conta.dta_conta_corrente
                                                      } into g
                                                      select new ContaCorrenteUI
                                                      {
                                                          //cd_conta_corrente = g.Key.cd_conta_corrente,
                                                          cd_tipo_liquidacao = g.Key.cd_tipo_liquidacao,
                                                          dc_tipo_liquidacao = g.Key.dc_tipo_liquidacao,
                                                          vl_entrada = g.Key.id_tipo_movimento == (int)ContaCorrente.Tipo.SAIDA ? g.Sum(s => s.vl_conta_corrente) : 0,
                                                          vl_saida = g.Key.id_tipo_movimento == (int)ContaCorrente.Tipo.ENTRADA ? g.Sum(s => s.vl_conta_corrente) : 0,
                                                          cd_local_movimento = (int)g.Key.cd_local,
                                                          dta_conta_corrente = g.Key.dta_conta_corrente
                                                      });
                else
                    sql = (from conta in db.ContaCorrente
                           join lco in db.LocalMovto
                           on conta.cd_local_origem equals lco.cd_local_movto
                           where conta.cd_pessoa_empresa == cd_pessoa_escola
                               && conta.dta_conta_corrente <= dta_fechamento
                               && (tipoLocal == 0 || lco.nm_tipo_local == tipoLocal) //(int)LocalMovto.TipoLocalMovtoEnum.CAIXA
                           group conta by new
                           {
                               cd_tipo_liquidacao = conta.cd_tipo_liquidacao,
                               dc_tipo_liquidacao = conta.TipoLiquidacao.dc_tipo_liquidacao,
                               id_tipo_movimento = conta.id_tipo_movimento,
                               cd_local = conta.cd_local_origem,
                               dta_conta_corrente = conta.dta_conta_corrente
                           } into g
                           select new ContaCorrenteUI
                           {
                               //cd_conta_corrente = g.Key.cd_conta_corrente,
                               cd_tipo_liquidacao = g.Key.cd_tipo_liquidacao,
                               dc_tipo_liquidacao = g.Key.dc_tipo_liquidacao,
                               vl_entrada = g.Key.id_tipo_movimento == (int)ContaCorrente.Tipo.ENTRADA ? g.Sum(s => s.vl_conta_corrente) : 0,
                               vl_saida = g.Key.id_tipo_movimento == (int)ContaCorrente.Tipo.SAIDA ? g.Sum(s => s.vl_conta_corrente) : 0,
                               cd_local_movimento = g.Key.cd_local,
                               dta_conta_corrente = g.Key.dta_conta_corrente
                           }).Union(
                        //Busca contas correntes que tenham local de movimento de destino.
                                                  from conta in db.ContaCorrente
                                                  join lcd in db.LocalMovto
                                                  on conta.cd_local_destino equals lcd.cd_local_movto
                                                  where conta.cd_pessoa_empresa == cd_pessoa_escola
                                                      && conta.dta_conta_corrente <= dta_fechamento
                                                      && (tipoLocal == 0 || lcd.nm_tipo_local == tipoLocal) //(int)LocalMovto.TipoLocalMovtoEnum.CAIXA
                                                  group conta by new
                                                  {
                                                      cd_tipo_liquidacao = conta.cd_tipo_liquidacao,
                                                      dc_tipo_liquidacao = conta.TipoLiquidacao.dc_tipo_liquidacao,
                                                      id_tipo_movimento = conta.id_tipo_movimento,
                                                      cd_local = conta.cd_local_destino,
                                                      dta_conta_corrente = conta.dta_conta_corrente
                                                  } into g
                                                  select new ContaCorrenteUI
                                                  {
                                                      //cd_conta_corrente = g.Key.cd_conta_corrente,
                                                      cd_tipo_liquidacao = g.Key.cd_tipo_liquidacao,
                                                      dc_tipo_liquidacao = g.Key.dc_tipo_liquidacao,
                                                      vl_entrada = g.Key.id_tipo_movimento == (int)ContaCorrente.Tipo.SAIDA ? g.Sum(s => s.vl_conta_corrente) : 0,
                                                      vl_saida = g.Key.id_tipo_movimento == (int)ContaCorrente.Tipo.ENTRADA ? g.Sum(s => s.vl_conta_corrente) : 0,
                                                      cd_local_movimento = (int)g.Key.cd_local,
                                                      dta_conta_corrente = g.Key.dta_conta_corrente
                                                  });
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public IEnumerable<ContaCorrenteUI> getFechamentoCaixaLocalMovto(int cd_pessoa_escola, DateTime dta_fechamento, int tipoLiquidacao, int cdUsuario, byte tipoLocal)
        {
            try
            {
                IQueryable<ContaCorrenteUI> sql;


                if (cdUsuario > 0)
                    sql = (from conta in db.ContaCorrente
                           join lco in db.LocalMovto
                           on conta.cd_local_origem equals lco.cd_local_movto
                           where db.UsuarioWebSGF.Where(u => u.cd_usuario == cdUsuario && u.cd_pessoa == conta.LocalMovtoOrigem.cd_pessoa_local).Any() 
                             && conta.cd_pessoa_empresa == cd_pessoa_escola //&& DbFunctions.TruncateTime(conta.dta_conta_corrente) == dta_fechamento
                             && conta.cd_tipo_liquidacao == tipoLiquidacao
                             && conta.dta_conta_corrente <= dta_fechamento
                             && lco.nm_tipo_local == tipoLocal //(int)LocalMovto.TipoLocalMovtoEnum.CAIXA
                           group conta by new
                           {
                               cd_local = conta.cd_local_origem,
                               cd_tipo_liquidacao = conta.cd_tipo_liquidacao,
                               no_local = conta.LocalMovtoOrigem.no_local_movto,
                               id_tipo_movimento = conta.id_tipo_movimento,
                               dta_conta_corrente = conta.dta_conta_corrente
                           } into lco
                           select new ContaCorrenteUI
                           {
                               cd_local_movimento = lco.Key.cd_local,
                               cd_tipo_liquidacao = lco.Key.cd_tipo_liquidacao,
                               des_local_movto = lco.Key.no_local,
                               vl_entrada = lco.Key.id_tipo_movimento == (int)ContaCorrente.Tipo.ENTRADA ? lco.Sum(s => s.vl_conta_corrente) : 0,
                               vl_saida = lco.Key.id_tipo_movimento == (int)ContaCorrente.Tipo.SAIDA ? lco.Sum(s => s.vl_conta_corrente) : 0,
                               dta_conta_corrente = lco.Key.dta_conta_corrente
                           }).Union(
                        //Busca contas correntes que tenham local de movimento de destino.
                                    from cc in db.ContaCorrente
                                    join lcd in db.LocalMovto on cc.cd_local_destino equals lcd.cd_local_movto
                                    join tl in db.TipoLiquidacao on cc.cd_tipo_liquidacao equals tl.cd_tipo_liquidacao
                                    where db.UsuarioWebSGF.Where(u => u.cd_usuario == cdUsuario && u.cd_pessoa == cc.LocalMovtoDestino.cd_pessoa_local).Any() 
                                          && cc.cd_pessoa_empresa == cd_pessoa_escola && cc.cd_local_destino != null 
                                          //&& DbFunctions.TruncateTime(cc.dta_conta_corrente) == dta_fechamento
                                          && cc.cd_tipo_liquidacao == tipoLiquidacao
                                         && cc.dta_conta_corrente <= dta_fechamento
                                         && lcd.nm_tipo_local == tipoLocal //(int)LocalMovto.TipoLocalMovtoEnum.CAIXA
                                    group cc by new
                                    {
                                        cd_local = cc.cd_local_destino,
                                        cd_tipo_liquidacao = cc.cd_tipo_liquidacao,
                                        nm_tipo = lcd.nm_tipo_local,
                                        no_local = lcd.no_local_movto,
                                        dc_tipo_liquidacao = tl.dc_tipo_liquidacao,
                                        id_tipo_movimento = cc.id_tipo_movimento,
                                        dta_conta_corrente = cc.dta_conta_corrente
                                    } into lco
                                    select new ContaCorrenteUI
                                    {
                                        cd_local_movimento = (int)lco.Key.cd_local,
                                        cd_tipo_liquidacao = lco.Key.cd_tipo_liquidacao,
                                        des_local_movto = lco.Key.no_local,
                                        vl_entrada = lco.Key.id_tipo_movimento == (int)ContaCorrente.Tipo.SAIDA ? lco.Sum(s => s.vl_conta_corrente) : 0,
                                        vl_saida = lco.Key.id_tipo_movimento == (int)ContaCorrente.Tipo.ENTRADA ? lco.Sum(s => s.vl_conta_corrente) : 0,
                                        dta_conta_corrente = lco.Key.dta_conta_corrente
                                    });
                else
                    sql = (from conta in db.ContaCorrente
                           join lco in db.LocalMovto
                           on conta.cd_local_origem equals lco.cd_local_movto
                           where conta.cd_pessoa_empresa == cd_pessoa_escola //&& DbFunctions.TruncateTime(conta.dta_conta_corrente) == dta_fechamento
                             && conta.cd_tipo_liquidacao == tipoLiquidacao
                             && conta.dta_conta_corrente <= dta_fechamento
                             && (tipoLocal == 0 || lco.nm_tipo_local == tipoLocal) //(int)LocalMovto.TipoLocalMovtoEnum.CAIXA
                           group conta by new
                           {
                               cd_local = conta.cd_local_origem,
                               cd_tipo_liquidacao = conta.cd_tipo_liquidacao,
                               no_local = conta.LocalMovtoOrigem.no_local_movto,
                               id_tipo_movimento = conta.id_tipo_movimento,
                               dta_conta_corrente = conta.dta_conta_corrente
                           } into lco
                           select new ContaCorrenteUI
                           {
                               cd_local_movimento = lco.Key.cd_local,
                               cd_tipo_liquidacao = lco.Key.cd_tipo_liquidacao,
                               des_local_movto = lco.Key.no_local,
                               vl_entrada = lco.Key.id_tipo_movimento == (int)ContaCorrente.Tipo.ENTRADA ? lco.Sum(s => s.vl_conta_corrente) : 0,
                               vl_saida = lco.Key.id_tipo_movimento == (int)ContaCorrente.Tipo.SAIDA ? lco.Sum(s => s.vl_conta_corrente) : 0,
                               dta_conta_corrente = lco.Key.dta_conta_corrente
                           }).Union(
                        //Busca contas correntes que tenham local de movimento de destino.
                                   from cc in db.ContaCorrente
                                   join lcd in db.LocalMovto on cc.cd_local_destino equals lcd.cd_local_movto
                                   join tl in db.TipoLiquidacao on cc.cd_tipo_liquidacao equals tl.cd_tipo_liquidacao
                                   where cc.cd_pessoa_empresa == cd_pessoa_escola && cc.cd_local_destino != null
                                   //&& DbFunctions.TruncateTime(cc.dta_conta_corrente) == dta_fechamento
                                        && cc.cd_tipo_liquidacao == tipoLiquidacao
                                        && cc.dta_conta_corrente <= dta_fechamento
                                        && (tipoLocal == 0 || lcd.nm_tipo_local == tipoLocal) //(int)LocalMovto.TipoLocalMovtoEnum.CAIXA
                                   group cc by new
                                   {
                                       cd_local = cc.cd_local_destino,
                                       cd_tipo_liquidacao = cc.cd_tipo_liquidacao,
                                       nm_tipo = lcd.nm_tipo_local,
                                       no_local = lcd.no_local_movto,
                                       dc_tipo_liquidacao = tl.dc_tipo_liquidacao,
                                       id_tipo_movimento = cc.id_tipo_movimento,
                                       dta_conta_corrente = cc.dta_conta_corrente
                                   } into lco
                                   select new ContaCorrenteUI
                                   {
                                       cd_local_movimento = (int)lco.Key.cd_local,
                                       cd_tipo_liquidacao = lco.Key.cd_tipo_liquidacao,
                                       des_local_movto = lco.Key.no_local,
                                       vl_entrada = lco.Key.id_tipo_movimento == (int)ContaCorrente.Tipo.SAIDA ? lco.Sum(s => s.vl_conta_corrente) : 0,
                                       vl_saida = lco.Key.id_tipo_movimento == (int)ContaCorrente.Tipo.ENTRADA ? lco.Sum(s => s.vl_conta_corrente) : 0,
                                       dta_conta_corrente = lco.Key.dta_conta_corrente
                                   });
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public void postZerarSaldoFinanceiro(int cd_escola, int cd_tipo_liquidacao, Nullable<System.DateTime> dta_base, byte tipo)
        {
            try
            {
                db.sp_zerar_saldos_financeiros(cd_escola, cd_tipo_liquidacao, dta_base, tipo);
            }
            catch (Exception exe)
            {
                throw new DataAccessException((exe.InnerException != null ? exe.InnerException.Message : "Erro Procedure."), exe);
                
            }
           
        }

        public IEnumerable<ContaCorrenteUI> getFechamentoCaixaLocalMovtoRel(int cd_pessoa_escola, DateTime dta_fechamento, int cdUsuario, byte tipoLocal)
        {
            try
            {
                IQueryable<ContaCorrenteUI> sql;


                if (cdUsuario > 0)
                    sql = (from conta in db.ContaCorrente
                           join lco in db.LocalMovto
                           on conta.cd_local_origem equals lco.cd_local_movto
                           where db.UsuarioWebSGF.Where(u => u.cd_usuario == cdUsuario 
                           && u.cd_pessoa == conta.LocalMovtoOrigem.cd_pessoa_local).Any() 
                           && lco.nm_tipo_local == tipoLocal //(int)LocalMovto.TipoLocalMovtoEnum.CAIXA
                           && conta.cd_pessoa_empresa == cd_pessoa_escola //&& DbFunctions.TruncateTime(conta.dta_conta_corrente) == dta_fechamento
                           && conta.dta_conta_corrente <= dta_fechamento
                           group conta by new
                           {
                               cd_local = conta.cd_local_origem,
                               cd_tipo_liquidacao = conta.cd_tipo_liquidacao,
                               no_local = conta.LocalMovtoOrigem.no_local_movto,
                               id_tipo_movimento = conta.id_tipo_movimento,
                               dta_conta_corrente = conta.dta_conta_corrente,
                               dc_tipo_liquidacao = conta.TipoLiquidacao.dc_tipo_liquidacao
                           } into lco
                           select new ContaCorrenteUI
                           {
                               cd_local_movimento = lco.Key.cd_local,
                               cd_tipo_liquidacao = lco.Key.cd_tipo_liquidacao,
                               dc_tipo_liquidacao = lco.Key.dc_tipo_liquidacao,
                               des_local_movto = lco.Key.no_local,
                               vl_entrada = lco.Key.id_tipo_movimento == (int)ContaCorrente.Tipo.ENTRADA ? lco.Sum(s => s.vl_conta_corrente) : 0,
                               vl_saida = lco.Key.id_tipo_movimento == (int)ContaCorrente.Tipo.SAIDA ? lco.Sum(s => s.vl_conta_corrente) : 0,
                               dta_conta_corrente = lco.Key.dta_conta_corrente
                           }).Union(
                        //Busca contas correntes que tenham local de movimento de destino.
                                    from cc in db.ContaCorrente
                                    join lcd in db.LocalMovto on cc.cd_local_destino equals lcd.cd_local_movto
                                    join tl in db.TipoLiquidacao on cc.cd_tipo_liquidacao equals tl.cd_tipo_liquidacao
                                    where db.UsuarioWebSGF.Where(u => u.cd_usuario == cdUsuario 
                                    && u.cd_pessoa == cc.LocalMovtoDestino.cd_pessoa_local).Any() 
                                        && lcd.nm_tipo_local == tipoLocal //(int)LocalMovto.TipoLocalMovtoEnum.CAIXA
                                        && cc.cd_pessoa_empresa == cd_pessoa_escola 
                                        && cc.dta_conta_corrente <= dta_fechamento
                                        //&& cc.cd_local_destino != null
                                        //&& DbFunctions.TruncateTime(cc.dta_conta_corrente) == dta_fechamento
                                    group cc by new
                                    {
                                        cd_local = cc.cd_local_destino,
                                        cd_tipo_liquidacao = cc.cd_tipo_liquidacao,
                                        nm_tipo = lcd.nm_tipo_local,
                                        no_local = lcd.no_local_movto,
                                        dc_tipo_liquidacao = tl.dc_tipo_liquidacao,
                                        id_tipo_movimento = cc.id_tipo_movimento,
                                        dta_conta_corrente = cc.dta_conta_corrente
                                    } into lco
                                    select new ContaCorrenteUI
                                    {
                                        cd_local_movimento = (int)lco.Key.cd_local,
                                        cd_tipo_liquidacao = lco.Key.cd_tipo_liquidacao,
                                        dc_tipo_liquidacao = lco.Key.dc_tipo_liquidacao,
                                        des_local_movto = lco.Key.no_local,
                                        vl_entrada = lco.Key.id_tipo_movimento == (int)ContaCorrente.Tipo.SAIDA ? lco.Sum(s => s.vl_conta_corrente) : 0,
                                        vl_saida = lco.Key.id_tipo_movimento == (int)ContaCorrente.Tipo.ENTRADA ? lco.Sum(s => s.vl_conta_corrente) : 0,
                                        dta_conta_corrente = lco.Key.dta_conta_corrente
                                    });
                else
                    sql = (from conta in db.ContaCorrente
                           join lco in db.LocalMovto
                           on conta.cd_local_origem equals lco.cd_local_movto
                           where (tipoLocal == 0 || lco.nm_tipo_local == tipoLocal)  //(int)LocalMovto.TipoLocalMovtoEnum.CAIXA
                             && conta.cd_pessoa_empresa == cd_pessoa_escola //&& DbFunctions.TruncateTime(conta.dta_conta_corrente) == dta_fechamento
                             && conta.dta_conta_corrente <= dta_fechamento
                           group conta by new
                           {
                               cd_local = conta.cd_local_origem,
                               cd_tipo_liquidacao = conta.cd_tipo_liquidacao,
                               no_local = conta.LocalMovtoOrigem.no_local_movto,
                               id_tipo_movimento = conta.id_tipo_movimento,
                               dta_conta_corrente = conta.dta_conta_corrente,
                               dc_tipo_liquidacao = conta.TipoLiquidacao.dc_tipo_liquidacao
                           } into lco
                           select new ContaCorrenteUI
                           {
                               cd_local_movimento = lco.Key.cd_local,
                               cd_tipo_liquidacao = lco.Key.cd_tipo_liquidacao,
                               dc_tipo_liquidacao = lco.Key.dc_tipo_liquidacao,
                               des_local_movto = lco.Key.no_local,
                               vl_entrada = lco.Key.id_tipo_movimento == (int)ContaCorrente.Tipo.ENTRADA ? lco.Sum(s => s.vl_conta_corrente) : 0,
                               vl_saida = lco.Key.id_tipo_movimento == (int)ContaCorrente.Tipo.SAIDA ? lco.Sum(s => s.vl_conta_corrente) : 0,
                               dta_conta_corrente = lco.Key.dta_conta_corrente
                           }).Union(
                        //Busca contas correntes que tenham local de movimento de destino.
                                   from cc in db.ContaCorrente
                                   join lcd in db.LocalMovto on cc.cd_local_destino equals lcd.cd_local_movto
                                   join tl in db.TipoLiquidacao on cc.cd_tipo_liquidacao equals tl.cd_tipo_liquidacao
                                   where (tipoLocal == 0 || lcd.nm_tipo_local == tipoLocal) //(int)LocalMovto.TipoLocalMovtoEnum.CAIXA 
                                   && cc.cd_pessoa_empresa == cd_pessoa_escola && cc.cd_local_destino != null
                                   && cc.dta_conta_corrente <= dta_fechamento
                                       //&& DbFunctions.TruncateTime(cc.dta_conta_corrente) == dta_fechamento
                                   group cc by new
                                   {
                                       cd_local = cc.cd_local_destino,
                                       cd_tipo_liquidacao = cc.cd_tipo_liquidacao,
                                       nm_tipo = lcd.nm_tipo_local,
                                       no_local = lcd.no_local_movto,
                                       dc_tipo_liquidacao = tl.dc_tipo_liquidacao,
                                       id_tipo_movimento = cc.id_tipo_movimento,
                                       dta_conta_corrente = cc.dta_conta_corrente
                                   } into lco
                                   select new ContaCorrenteUI
                                   {
                                       cd_local_movimento = (int)lco.Key.cd_local,
                                       cd_tipo_liquidacao = lco.Key.cd_tipo_liquidacao,
                                       dc_tipo_liquidacao = lco.Key.dc_tipo_liquidacao,
                                       des_local_movto = lco.Key.no_local,
                                       vl_entrada = lco.Key.id_tipo_movimento == (int)ContaCorrente.Tipo.SAIDA ? lco.Sum(s => s.vl_conta_corrente) : 0,
                                       vl_saida = lco.Key.id_tipo_movimento == (int)ContaCorrente.Tipo.ENTRADA ? lco.Sum(s => s.vl_conta_corrente) : 0,
                                       dta_conta_corrente = lco.Key.dta_conta_corrente
                                   });
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

    }
}

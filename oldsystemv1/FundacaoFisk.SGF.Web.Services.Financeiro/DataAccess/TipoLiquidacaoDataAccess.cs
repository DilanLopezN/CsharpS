using System;
using System.Collections.Generic;
using System.Linq;
using Componentes.GenericModel;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Financeiro;
using System.Data.Entity;
using Componentes.Utils;
using Componentes.GenericDataAccess;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess;
using Componentes.GenericDataAccess.GenericException;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess
{
    public class TipoLiquidacaoDataAccess : GenericRepository<TipoLiquidacao>, ITipoLiquidacaoDataAccess
    {
        public enum TipoConsultaTipoLiquidacaoEnum
        {
            HAS_ATIVO = 0,
            HAS_BAIXA_TITULO = 1,
            HAS_CONTA_CORRENTE = 2,
            HAS_REL_PAGAR_RECEBER = 3
        }
        public enum TipoLiquidacaoEnum
        {
            TP_LIQUIDACAO_MOTIVO_BOLSA = 100
        }

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public IEnumerable<TipoLiquidacao> GetTipoLiquidacaoSearch(SearchParameters parametros, string descricao, bool inicio, bool? status)
        {
            try{
                IEntitySorter<TipoLiquidacao> sorter = EntitySorter<TipoLiquidacao>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<TipoLiquidacao> sql;

                if (status == null)
                {
                    sql = from c in db.TipoLiquidacao.AsNoTracking()
                          select c;
                }
                else
                {
                    sql = from c in db.TipoLiquidacao.AsNoTracking()
                          where c.id_tipo_liquidacao_ativa == status
                          select c;
                }

                sql = sorter.Sort(sql);

                var retorno = from c in sql
                              select c;
                if (!String.IsNullOrEmpty(descricao))
                    if (inicio)
                        retorno = from c in sql
                                  where c.dc_tipo_liquidacao.StartsWith(descricao)
                                  select c;
                    else
                        retorno = from c in sql
                                  where c.dc_tipo_liquidacao.Contains(descricao)
                                  select c;

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
        
        public bool deleteAllTipoLiquidacao(List<TipoLiquidacao> tiposLiquidacao)
        {
            try{
                var strTipoLiquidacao = "";
                foreach (TipoLiquidacao tipo in tiposLiquidacao)
                    strTipoLiquidacao += tipo.cd_tipo_liquidacao + ",";

                if (strTipoLiquidacao.Length > 0)
                    strTipoLiquidacao = strTipoLiquidacao.Substring(0, strTipoLiquidacao.Length - 1);

                int retono = db.Database.ExecuteSqlCommand("delete from t_tipo_liquidacao where cd_tipo_liquidacao in(" + strTipoLiquidacao + ")");
                return retono > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<TipoLiquidacao> getTipoLiquidacao(TipoConsultaTipoLiquidacaoEnum hasDependente, int? cd_tipo_liquidacao)
        {
            try
            {
                IQueryable<TipoLiquidacao> sql = null;
                switch (hasDependente)
                {
                    case TipoConsultaTipoLiquidacaoEnum.HAS_ATIVO:
                        sql = from t in db.TipoLiquidacao
                              where ((t.id_tipo_liquidacao_ativa == true)|| 
                                     (cd_tipo_liquidacao.HasValue && t.cd_tipo_liquidacao == cd_tipo_liquidacao.Value))
                              orderby t.dc_tipo_liquidacao
                              select t;
                        break;
                    case TipoConsultaTipoLiquidacaoEnum.HAS_BAIXA_TITULO:
                        sql = from t in db.TipoLiquidacao
                              where t.BaixoTitulo.Any() && t.id_tipo_liquidacao_ativa
                              orderby t.dc_tipo_liquidacao
                              select t;
                        break;
                    case TipoConsultaTipoLiquidacaoEnum.HAS_REL_PAGAR_RECEBER:
                        sql = from t in db.TipoLiquidacao
                              where t.BaixoTitulo.Any() && (t.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.CANCELAMENTO && 
                                                            t.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA &&
                                                            t.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA_ADITIVO)
                              orderby t.dc_tipo_liquidacao
                              select t;
                        break;
                }
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<TipoLiquidacao> getTipoLiquidacao()
        {
            try
            {
                var sql = from t in db.TipoLiquidacao
                          where t.id_tipo_liquidacao_ativa == true &&
                                t.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA && t.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA_ADITIVO
                          orderby t.dc_tipo_liquidacao
                          select t;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
       
        public IEnumerable<TipoLiquidacao> getTipoLiquidacaoCd(int? cdTipoLiq)
        {
            try
            {
                var sql = from t in db.TipoLiquidacao
                          where t.id_tipo_liquidacao_ativa == true &&
                          (cdTipoLiq == null || t.cd_tipo_liquidacao == cdTipoLiq) &&
                                     t.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA && t.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA_ADITIVO
                          orderby t.dc_tipo_liquidacao
                          select t;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public IEnumerable<TipoLiquidacao> getTipoLiquidacaoCCByLocalMovto(int cd_local_movto)
        {
            try
            {
                var sql = (from t in db.TipoLiquidacao
                           where t.ContasCorrentes.Where(cc => cc.cd_local_destino == cd_local_movto || cc.cd_local_origem == cd_local_movto).Any() 
                           //&& t.cd_tipo_liquidacao != (int)TipoLiquidacaoEnum.TP_LIQUIDACAO_MOTIVO_BOLSA
                           select new {
                               cd_tipo_liquidacao = t.cd_tipo_liquidacao
                           }).ToList().Select(x => new TipoLiquidacao
                           {
                               cd_tipo_liquidacao = x.cd_tipo_liquidacao
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

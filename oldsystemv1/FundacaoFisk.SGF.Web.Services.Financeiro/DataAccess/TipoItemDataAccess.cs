using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess
{
    public class TipoItemDataAccess : GenericRepository<TipoItem>, ITipoItemDataAccess
    {
        public enum TipoConsultaTipoItemEnum
        {
            HAS_ATIVO = 0,
            HAS_MOVIMENTO_ENTRADA = 1,
            HAS_MOVIMENTO_DESPESAS = 3,
            HAS_MOVIMENTO_SAIDA = 2,
            HAS_MOVIMENTO_SERVICO = 4
           
        }

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public IQueryable<TipoItem> getTipoItemSearch(int? tipoMovimento)
        {
            //IQueryable<TipoItem> sql;
            try{
                //TODO:Deivid
                var sql = from tiposItem in db.TipoItem
                      orderby tiposItem.dc_tipo_item
                      select tiposItem;
                if (tipoMovimento != null && tipoMovimento > 0)
                    switch (tipoMovimento)
                    {
                        case (int)TipoConsultaTipoItemEnum.HAS_ATIVO:
                            sql = from ti in sql
                                  where ti.id_tipo_item_ativo == true
                                  orderby ti.dc_tipo_item
                                  select ti;
                            break;
                        case (int)TipoConsultaTipoItemEnum.HAS_MOVIMENTO_ENTRADA:
                        case (int)TipoConsultaTipoItemEnum.HAS_MOVIMENTO_SAIDA:
                            sql = from ti in sql
                                  where ti.id_tipo_item_ativo == true &&
                                  ti.cd_tipo_item != (int)TipoItem.TipoItemEnum.SERVICO &&
                                  ti.cd_tipo_item != (int)TipoItem.TipoItemEnum.CUSTOSDESPESAS
                                  orderby ti.dc_tipo_item
                                  select ti;
                            break;
                        case (int)TipoConsultaTipoItemEnum.HAS_MOVIMENTO_DESPESAS:
                            sql = from ti in sql
                                  where ti.id_tipo_item_ativo == true &&
                                  (ti.cd_tipo_item == (int)TipoItem.TipoItemEnum.SERVICO || ti.cd_tipo_item == (int)TipoItem.TipoItemEnum.CUSTOSDESPESAS)
                                  orderby ti.dc_tipo_item
                                  select ti;
                            break;  
                        case (int)TipoConsultaTipoItemEnum.HAS_MOVIMENTO_SERVICO:
                            sql = from ti in sql
                                  where ti.id_tipo_item_ativo == true && ti.cd_tipo_item == (int)TipoItem.TipoItemEnum.SERVICO
                                  orderby ti.dc_tipo_item
                                  select ti;
                            break;
                    }

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<TipoItem> getTipoItemMovimento(TipoConsultaTipoItemEnum tipoConsulta)
        {
            try
            {
                var sql = from ti in db.TipoItem
                          orderby ti.dc_tipo_item
                          select ti;
                switch (tipoConsulta)
                {
                    case TipoConsultaTipoItemEnum.HAS_ATIVO:
                        sql = from ti in sql
                              where ti.id_tipo_item_ativo == true
                              orderby ti.dc_tipo_item
                              select ti;
                        break;
                    case TipoConsultaTipoItemEnum.HAS_MOVIMENTO_ENTRADA:
                    case TipoConsultaTipoItemEnum.HAS_MOVIMENTO_SAIDA:
                        sql = from ti in sql
                              where ti.id_tipo_item_ativo == true &&
                              ti.cd_tipo_item != (int)TipoItem.TipoItemEnum.SERVICO &&
                              ti.cd_tipo_item != (int)TipoItem.TipoItemEnum.CUSTOSDESPESAS
                              orderby ti.dc_tipo_item
                              select ti;
                        break;
                    case TipoConsultaTipoItemEnum.HAS_MOVIMENTO_DESPESAS:
                        sql = from ti in sql
                              where ti.id_tipo_item_ativo == true &&
                              (ti.cd_tipo_item == (int)TipoItem.TipoItemEnum.SERVICO || ti.cd_tipo_item == (int)TipoItem.TipoItemEnum.CUSTOSDESPESAS)
                              orderby ti.dc_tipo_item
                              select ti;
                        break;
                    case TipoConsultaTipoItemEnum.HAS_MOVIMENTO_SERVICO:
                        sql = from ti in sql
                              where ti.id_tipo_item_ativo == true && ti.cd_tipo_item == (int)TipoItem.TipoItemEnum.SERVICO
                              orderby ti.dc_tipo_item
                              select ti;
                        break;
                }
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<TipoItem> getTipoItemMovimentoWithItem(int cd_pessoa_escola, bool isMaster)
        {
            try
            {
                IQueryable<TipoItem> sql;

                if (isMaster)
                    sql = from ti in db.TipoItem
                           where ti.Item.Any()
                           orderby ti.dc_tipo_item
                           select ti;
                else
                    sql = from ti in db.TipoItem
                           where ti.Item.Any(i => i.ItemEscola.Any(it => it.cd_pessoa_escola == cd_pessoa_escola))
                           select ti;
                
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<TipoItem> getTipoItemMovimentoEstoque()
        {
            try
            {
                var sql = from tiposItem in db.TipoItem
                          where tiposItem.id_movimentar_estoque
                          orderby tiposItem.dc_tipo_item
                          select tiposItem;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool verificaMovimentarEstoque(int cd_item) {
            try {
                var sql = (from item in db.Item
                          where item.TipoItem.id_movimentar_estoque 
                            && item.cd_item == cd_item
                          select item.cd_item).Any();
                return sql;
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }
    }
}

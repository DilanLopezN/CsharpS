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

namespace FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess
{
    public class ItemSubgrupoDataAccess : GenericRepository<ItemSubgrupo>, IItemSubgrupoDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }
        public IEnumerable<ItemSubgrupo> getSubGrupoTpHasItem(int cd_item)
        {
            try
            {
                var sql = (from i in db.ItemSubgrupo
                          where i.cd_item == cd_item
                          orderby i.id_tipo_movimento
                          select new 
                          {
                              cd_item_subgrupo = i.cd_item_subgrupo,
                              cd_subgrupo_conta = i.cd_subgrupo_conta,
                              cd_item = i.cd_item,
                              id_tipo_movimento = i.id_tipo_movimento,
                              no_subgrupo = i.Subgrupo.no_subgrupo_conta
                          }).ToList().Select(x => new ItemSubgrupo
                          {
                              cd_item_subgrupo = x.cd_item_subgrupo,
                              cd_subgrupo_conta = x.cd_subgrupo_conta,
                              cd_item = x.cd_item,
                              id_tipo_movimento = x.id_tipo_movimento,
                              no_subgrupo = x.no_subgrupo
                          });

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public ItemSubgrupo getSubGrupoPlano(int cd_item, byte tipo, int cdEscola)
        {
            try
            {
                var sql = (from i in db.ItemSubgrupo
                           where i.cd_item == cd_item &&
                           i.id_tipo_movimento == tipo &&
                           i.Subgrupo.SubgrupoPlanoConta.Where(p => p.cd_pessoa_empresa == cdEscola).Any()
                           select new
                           {
                               cd_item_subgrupo = i.cd_item_subgrupo,
                               cd_plano_conta = i.Subgrupo.SubgrupoPlanoConta.Where(p => p.cd_pessoa_empresa == cdEscola).Select(e => e.cd_plano_conta).FirstOrDefault(),
                               cd_item = i.cd_item,
                               no_subgrupo = i.Subgrupo.no_subgrupo_conta
                           }).ToList().Select(x => new ItemSubgrupo
                           {
                               cd_item_subgrupo = x.cd_item_subgrupo,
                               cd_item = x.cd_item,
                               no_subgrupo = x.no_subgrupo,
                               cd_plano_conta = x.cd_plano_conta
                           }).FirstOrDefault();

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}
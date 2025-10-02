using System;
using System.Linq;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess
{
    public class ItemMovItemKitDataAccess : GenericRepository<ItemMovItemKit>, IItemMovItemKitDataAccess
    {
        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }

        public ItemMovItemKit findByCdItemMovimentoAndCdItemKit(int cd_item_movimento, int cd_item_kit)
        {
            try
            {
                var sql = from i in db.ItemMovItemKit
                    where i.cd_item_movimento == cd_item_movimento && i.cd_item_kit == cd_item_kit
                    select i;
                return sql.FirstOrDefault();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}
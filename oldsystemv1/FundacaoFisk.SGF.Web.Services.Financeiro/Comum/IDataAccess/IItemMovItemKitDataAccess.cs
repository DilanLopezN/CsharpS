using Componentes.GenericDataAccess.Comum;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess
{
    public interface IItemMovItemKitDataAccess : IGenericRepository<ItemMovItemKit>
    {
        ItemMovItemKit findByCdItemMovimentoAndCdItemKit(int cd_item_movimento, int cd_item_kit);
    }
}
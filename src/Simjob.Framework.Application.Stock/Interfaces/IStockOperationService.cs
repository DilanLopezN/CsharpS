using Simjob.Framework.Application.Stock.Models;
using Simjob.Framework.Domain.Models;
using Simjob.Framework.Domain.Util;
using System;
using System.Threading.Tasks;

namespace Simjob.Framework.Application.Stock.Interfaces
{
    public interface IStockOperationService
    {
        Task Insert(InsertStockOperationModel model);
        Task UpdateStockItemBalance(string itemId);

        //Task SetReal(string stockOperationId);
        Task SetRealNew(string stockOperationId, string userName);

        Task UndoSetReal(string stockOperationId);

        Task UpdateUndoStockItemBalance(string itemId);
        Task<double> GetSumItemBalanceAsync(string itemId, bool input, bool? isReal = false, bool? isReserved = null, bool? isAvailable = null, DateTime? dtStart = null, DateTime? dtEnd = null);
        Task ResetBalance();
        Task<dynamic> GetVol(VolumeFilterModel model);
        Task<dynamic> GetLote(LoteFilterModel model);
        Task<dynamic> GetAllItems(ItemFilterModel model);
        Task<dynamic> GetOperMov(OperMovFilterModel model);
        Task<dynamic> GetLocalByCode(string code, int? page, int? limit);
        Task<dynamic> GetAllLocal(LocalFilterModel model);
        Task<dynamic> GetItemRule(ItemRuleFilterModel model);
        Task<dynamic> GetItemRuleRepo(ItemRuleFilterModel model);
        Task<dynamic> GetItemRuleRepoReab(ItemRuleFilterModel model);

        Task<PaginationData<dynamic>> GetFluxoSaida(StockFilterSaidaModel model);
        Task<PaginationData<dynamic>> GetFluxoEntrada(StockFilterEntradaModel model);

        //picking
        Task UpdatePickingOrder(string codigoPicking, string userName);
    }
}

using MediatR;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;

using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MoreLinq;
using Newtonsoft.Json;
using SendGrid.Helpers.Errors.Model;
using Simjob.Framework.Application.Stock.Constants;
using Simjob.Framework.Application.Stock.Interfaces;
using Simjob.Framework.Application.Stock.Models;
using Simjob.Framework.Domain.Core.Bus;
using Simjob.Framework.Domain.Core.Notifications;
using Simjob.Framework.Domain.Interfaces.Repositories;
using Simjob.Framework.Domain.Models;
using Simjob.Framework.Domain.Models.StockReturnModels;
using Simjob.Framework.Domain.Util;
using Simjob.Framework.Infra.Data.Context;
using Simjob.Framework.Infra.Domain.Models;
using Simjob.Framework.Infra.Identity.Interfaces;
using Simjob.Framework.Infra.Schemas.Commands.Entities;
using Simjob.Framework.Infra.Schemas.Entities;
using Simjob.Framework.Infra.Schemas.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using static MongoDB.Driver.WriteConcern;
using static Simjob.Framework.Domain.Models.ItemRetornoRepoModel;

namespace Simjob.Framework.Application.Stock.Services
{
    [ExcludeFromCodeCoverage]
    public class StockOperationService : IStockOperationService
    {
        private readonly IMediatorHandler _bus;
        private readonly ISchemaBuilder _schemaBuilder;
        private readonly IRepository<MongoDbContext, Schema> _schemaRepository;
        private readonly DomainNotificationHandler _notifications;
        private readonly IServiceProvider _serviceProvider;
        private readonly MongoDbContext _context;
        private readonly IEntityService _entityService;
        private readonly IStockMovBsonRepository _stockMovBsonRepository;
        private readonly IStockOperationBsonRepository _stockOperationBsonRepository;
        private readonly IStockItemBalanceBsonRepository _stockItemBalanceBsonRepository;
        private readonly IStockLotBsonRepository _stockLotBsonRepository;
        private readonly IStockLotBalanceBsonRepository _stockLotBalanceBsonRepository;
        private readonly IStockBalanceBsonRepository _stockBalanceRepository;
        private readonly IGeneratorsService _generatorsService;




        public StockOperationService(IMediatorHandler bus, ISchemaBuilder schemaBuilder, IRepository<MongoDbContext, Schema> schemaRepository,
                                        INotificationHandler<DomainNotification> notifications, IServiceProvider serviceProvider, MongoDbContext context, IEntityService entityService, IStockMovBsonRepository stockMovRepository, IStockOperationBsonRepository stockOperationBsonRepository, IStockItemBalanceBsonRepository stockItemBalanceBsonRepository, IStockLotBsonRepository stockLotBsonRepository, IStockLotBalanceBsonRepository stockLotBalanceBsonRepository, IStockBalanceBsonRepository stockBalanceRepository, IGeneratorsService generatorsService)
        {
            _bus = bus;
            _schemaBuilder = schemaBuilder;
            _schemaRepository = schemaRepository;
            _notifications = (DomainNotificationHandler)notifications;
            _serviceProvider = serviceProvider;
            _context = context;
            _entityService = entityService;
            _stockMovBsonRepository = stockMovRepository;
            _stockOperationBsonRepository = stockOperationBsonRepository;
            _stockItemBalanceBsonRepository = stockItemBalanceBsonRepository;
            _stockLotBsonRepository = stockLotBsonRepository;
            _stockLotBalanceBsonRepository = stockLotBalanceBsonRepository;
            _stockBalanceRepository = stockBalanceRepository;
            _generatorsService = generatorsService;
        }

        //public async Task SetReal(string stockOperationId)
        //{
        //    List<UpdateEntityCommand> updateCommandList = new List<UpdateEntityCommand>();
        //    DateTime now = DateTime.Now;

        //    var stockOperationType = await _schemaBuilder.GetSchemaType("StockOperation");
        //    var stockMovType = await _schemaBuilder.GetSchemaType("StockMov");
        //    var stockOperationRepository = _entityService.GetRepository(stockOperationType);
        //    var stockMovRepository = _entityService.GetRepository(stockMovType);
        //    var stockMovSchema = _entityService.GetSchemaByName("StockMov");
        //    var stockOperationSchema = _entityService.GetSchemaByName("StockOperation");

        //    var stockOperation = _stockOperationBsonRepository.GetById(stockOperationId);

        //    if (stockOperation == null)
        //    {
        //        await _bus.RaiseEvent(new DomainNotification("StockService", "Stock Operation not found"));
        //        return;
        //    }

        //    stockOperation["dateReal"] = new BsonDateTime(now);

        //    var stockOperationDes = BsonSerializer.Deserialize<Dictionary<string, object>>(stockOperation);


        //    var updateStockOperationCommand = new UpdateEntityCommand
        //    {
        //        Id = (string)stockOperationDes["_id"],
        //        SchemaName = stockOperationSchema.Name,
        //        Data = stockOperationDes,
        //        SchemaJson = stockOperationSchema.JsonValue
        //    };

        //    updateCommandList.Add(updateStockOperationCommand);
        //    List<object> idsSplit = (List<object>)stockOperationDes["stockMovIds"];
        //    List<string> ids = new List<string>();
        //    foreach (var id in idsSplit)
        //    {
        //        ids.Add(id.ToString());
        //    }

        //    var stockMovs = _stockMovBsonRepository.GetByIds(ids);



        //    foreach (var stockMov in stockMovs)
        //    {
        //        stockMov["isReal"] = new BsonBoolean(true);
        //        stockMov["date"] = new BsonDateTime(now);

        //        var stockMovDes = BsonSerializer.Deserialize<Dictionary<string, object>>(stockMov);
        //        stockMovDes = stockMovDes.Where(x => x.Value != null).ToDictionary(x => x.Key, x => x.Value);
        //        //if (stockMovDes.ContainsKey("stockOperationId")) stockMovDes.Remove("stockOperationId");
        //        var updateCommand = new UpdateEntityCommand
        //        {
        //            Id = (string)stockMov["_id"],
        //            SchemaName = stockMovSchema.Name,
        //            Data = stockMovDes,
        //            SchemaJson = stockMovSchema.JsonValue
        //        };

        //        updateCommandList.Add(updateCommand);

        //        await UpdateStockItemBalance(stockMov["itemId"].AsString);

        //        if (stockMov["stockLotId"] != null)
        //            await UpdateStockLotBalance(stockMov["stockLotId"].AsString, stockMov["itemId"].AsString);
        //    }

        //    foreach (var updateCommand in updateCommandList)
        //    {

        //        await _bus.SendCommand(updateCommand);
        //    }
        //}

        #region getStock

        private async Task<PaginationData<dynamic>> RunItemRepoCommand(string command, int? page, int? limit)
        {
            var listRes = await _context.RunCommandAsync<dynamic>(command);

            if (page == null) page = 1;
            if (limit == null) limit = 30;
            int skipCount = (int)((page - 1) * limit);
            var limitedList = listRes;
            var jsonItemRepo = JsonConvert.SerializeObject(limitedList);
            List<ItemRuleModel> listItemRuleModel = JsonConvert.DeserializeObject<List<ItemRuleModel>>(jsonItemRepo);
            List<string?> itemIds = listItemRuleModel.Select(x => x.Id).ToList();
  
            var stockBalance = await _entityService.GetAllStockBalanceForItemRepo("StockBalance", itemIds);

            if (stockBalance == null)
            {
                await _bus.RaiseEvent(new DomainNotification("Schema", "stockBalance not found"));
                return default;
            }

            var jsonStockBalance = JsonConvert.SerializeObject(stockBalance);
            List<StockBalanceModel> stockBalanceList = JsonConvert.DeserializeObject<List<StockBalanceModel>>(jsonStockBalance);

            List<string?> localIds = stockBalanceList.Select(x => x.StockLocId).Distinct().ToList();
            var locais = await _entityService.GetByIdsList("StockLocal", localIds);
            if (locais == null)
            {
                await _bus.RaiseEvent(new DomainNotification("Schema", "local not found"));
                return default;
            }
            var jsonLocals = JsonConvert.SerializeObject(locais);
            List<StockLocalModel> stockLocalListDes = JsonConvert.DeserializeObject<List<StockLocalModel>>(jsonLocals);

            //Lista de locais do estoque atual (local de picking)
            var pickingLocalList = stockLocalListDes.Where(x => x.IsAvailable == true && x.IsPickingLocation == true).Select(x => x.Id).ToList();
            var stockBalanceDes = stockBalanceList.Where(x => pickingLocalList.Contains(x.StockLocId)).ToList();

            //Lista de locais para abastecer o item
            var availableLocalist = stockLocalListDes.Where(x => x.IsAvailable == true && x.IsPickingLocation == false).Select(x=> x.Id).ToList();
            var stockBalanceDesAvailableLocals = stockBalanceList.Where(x => availableLocalist.Contains(x.StockLocId)).ToList();

            stockBalanceDes = stockBalanceDes
                .OrderByDescending(x => x.Date)    
                .GroupBy(x => new { x.ItemId, x.StockLotId })
                .Select(g => g.First())  
                .ToList();

            stockBalanceDesAvailableLocals = stockBalanceDesAvailableLocals
                .OrderByDescending(x => x.Date)
                .GroupBy(x => new { x.ItemId, x.StockLotId })
                .Select(g => g.First())
                .ToList();

            var stockLotIds = stockBalanceDesAvailableLocals.Select(x => x.StockLotId).Distinct().ToList();
            var lotes = await _entityService.GetByIdsList("StockLot", stockLotIds);
            if (lotes == null)
            {
                await _bus.RaiseEvent(new DomainNotification("Schema", "stockLot not found"));
                return default;
            }
            var jsonlotes = JsonConvert.SerializeObject(lotes);
            List<StockLotModel> lotesDes = JsonConvert.DeserializeObject<List<StockLotModel>>(jsonlotes);
            var retornoList = new List<ItemRetornoRepoModel>();
            foreach (var item in listItemRuleModel)
            {
                ItemRetornoRepoModel? itemRetorno = new ItemRetornoRepoModel();

      
                var stockBalanceAtual = item.LocalId != null ? stockBalanceDes.Where(x => x.ItemId == item.Id && x.StockLocId == item.LocalId).ToList() : stockBalanceDes.Where(x => x.ItemId == item.Id).ToList();
                if(stockBalanceAtual == null || stockBalanceAtual.Count() == 0) 
                {
                     itemRetorno = new ItemRetornoRepoModel
                    {
                        Id = item.Id,
                        Item = item.CodProduto,
                        Unidade = item.Unidade,
                        DescricaoItem = item.Descricao,
                        LocalId = item.LocalId,
                        Local = item.Local,
                        DescricaoLocal = item.DescricaoLocal,
                        QtdMin = item.QtdMin,
                        QtdMax = item.QtdMax,
                        QtdEmEstoque = 0,
                        OrigensReposicao = new List<OrigensRepoModel>() { }
                     };
                }
                else
                {
                    var origemRepos = stockBalanceDesAvailableLocals.Where(x => x.ItemId == item.Id && x.StockLocId != item.LocalId && x.QtTotal > 0).ToList();
                    itemRetorno = new ItemRetornoRepoModel
                    {
                        Id = item.Id,
                        Item = item.CodProduto,
                        Unidade = item.Unidade,
                        DescricaoItem = item.Descricao,
                        LocalId = item.LocalId,
                        Local = item.Local,
                        DescricaoLocal = item.DescricaoLocal,
                        QtdMin = item.QtdMin,
                        QtdMax = item.QtdMax,
                        QtdEmEstoque = stockBalanceAtual != null ? stockBalanceAtual.Sum(x=> x.QtTotal) : 0,
                        OrigensReposicao = new List<OrigensRepoModel>() { }
                    };
                    for (var i = 0; i < 3; i++)
                    {
                        if (i < origemRepos.Count() && origemRepos[i] != null)
                        {
                            var lote = lotesDes.Where(x => x.Id == origemRepos[i].StockLotId).FirstOrDefault();
                            if (lote == null)
                            {
                                await _bus.RaiseEvent(new DomainNotification("Schema", "stockLot not found"));
                                return default;
                            }
                            itemRetorno.OrigensReposicao.Add(new OrigensRepoModel
                            {
                                LocalId = origemRepos[i].StockLocId,
                                LoteId = origemRepos[i].StockLotId,
                                LoteCode = origemRepos[i].StockLotCode,
                                Local = origemRepos[i].StockLocCode,
                                QtdDisp = origemRepos[i].QtTotal,
                                Validade = lote.ExpirateDate
                            });
                        }

                    }

                }
                
                retornoList.Add(itemRetorno);

            }
            foreach (var item in retornoList)
            {
                item.OrigensReposicao = item.OrigensReposicao.OrderBy(y => y.Validade).ToList();
            }

            retornoList = retornoList.Skip(skipCount).Take((int)limit).ToList();

            retornoList = retornoList
               .OrderBy(x => x.OrigensReposicao
               .OrderBy(y => y.Validade)
               .FirstOrDefault()?.Validade)
               .ToList();
            
            return new PaginationData<dynamic>(retornoList, page, limit, listItemRuleModel.Count());
        }
        private async Task<PaginationData<dynamic>> RunItemRepoReabCommand(string command, string commandQtdPend, int? page, int? limit)
        {
            var listQtdPendReturn = await _context.RunCommandAsync<ItemQtdPend>(commandQtdPend);


            var listRes = await _context.RunCommandAsync<dynamic>(command);

            if (page == null) page = 1;
            if (limit == null) limit = 30;
            int skipCount = (int)((page - 1) * limit);
            var limitedList = listRes;
            var jsonItemRepo = JsonConvert.SerializeObject(limitedList);
            List<ItemRuleModel> listItemRuleModel = JsonConvert.DeserializeObject<List<ItemRuleModel>>(jsonItemRepo);
            List<string?> itemIds = listItemRuleModel.Select(x => x.Id).ToList();

            var stockBalance = await _entityService.GetAllStockBalanceForItemRepo("StockBalance", itemIds);

            if (stockBalance == null)
            {
                await _bus.RaiseEvent(new DomainNotification("Schema", "stockBalance not found"));
                return default;
            }

            var jsonStockBalance = JsonConvert.SerializeObject(stockBalance);
            List<StockBalanceModel> stockBalanceList = JsonConvert.DeserializeObject<List<StockBalanceModel>>(jsonStockBalance);

            List<string?> localIds = stockBalanceList.Select(x => x.StockLocId).Distinct().ToList();
            var locais = await _entityService.GetByIdsList("StockLocal", localIds);
            if (locais == null)
            {
                await _bus.RaiseEvent(new DomainNotification("Schema", "local not found"));
                return default;
            }
            var jsonLocals = JsonConvert.SerializeObject(locais);
            List<StockLocalModel> stockLocalListDes = JsonConvert.DeserializeObject<List<StockLocalModel>>(jsonLocals);

            //Lista de locais do estoque atual (local de picking)
            var pickingLocalList = stockLocalListDes.Where(x => x.IsAvailable == true && x.IsPickingLocation == true).Select(x => x.Id).ToList();
            var stockBalanceDes = stockBalanceList.Where(x => pickingLocalList.Contains(x.StockLocId)).ToList();

            //Lista de locais para abastecer o item
            var availableLocalist = stockLocalListDes.Where(x => x.IsAvailable == true && x.IsPickingLocation == false).Select(x => x.Id).ToList();
            var stockBalanceDesAvailableLocals = stockBalanceList.Where(x => availableLocalist.Contains(x.StockLocId)).ToList();

            stockBalanceDes = stockBalanceDes
                .OrderByDescending(x => x.Date)
                .GroupBy(x => new { x.ItemId, x.StockLotId })
                .Select(g => g.First())
                .ToList();

            stockBalanceDesAvailableLocals = stockBalanceDesAvailableLocals
                .OrderByDescending(x => x.Date)
                .GroupBy(x => new { x.ItemId, x.StockLotId })
                .Select(g => g.First())
                .ToList();

            var stockLotIds = stockBalanceDesAvailableLocals.Select(x => x.StockLotId).Distinct().ToList();
            var lotes = await _entityService.GetByIdsList("StockLot", stockLotIds);
            if (lotes == null)
            {
                await _bus.RaiseEvent(new DomainNotification("Schema", "stockLot not found"));
                return default;
            }
            var jsonlotes = JsonConvert.SerializeObject(lotes);
            List<StockLotModel> lotesDes = JsonConvert.DeserializeObject<List<StockLotModel>>(jsonlotes);
            var retornoList = new List<ItemRetornoRepoModel>();
            foreach (var item in listItemRuleModel)
            {
                ItemRetornoRepoModel? itemRetorno = new ItemRetornoRepoModel();


                var stockBalanceAtual = item.LocalId != null ? stockBalanceDes.Where(x => x.ItemId == item.Id && x.StockLocId == item.LocalId).ToList() : stockBalanceDes.Where(x => x.ItemId == item.Id).ToList();
                var origemRepos = stockBalanceDesAvailableLocals.Where(x => x.ItemId == item.Id && x.StockLocId != item.LocalId && x.QtTotal > 0).ToList();
                if (stockBalanceAtual != null && stockBalanceAtual.Count > 0)
                {
                    itemRetorno = new ItemRetornoRepoModel
                    {
                        Id = item.Id,
                        Item = item.CodProduto,
                        Unidade = item.Unidade,
                        DescricaoItem = item.Descricao,
                        LocalId = item.LocalId,
                        Local = item.Local,
                        DescricaoLocal = item.DescricaoLocal,
                        QtdMin = item.QtdMin,
                        QtdMax = item.QtdMax,
                        Seg = item.Seg,
                        QtdEmEstoque = stockBalanceAtual.Sum(x => x.QtTotal),
                        QtdPendentePicking = listQtdPendReturn.FirstOrDefault(i  => i.ItemId == item.Id)?.TotalQtdPendente ?? 0,
                        OrigensReposicao = new List<OrigensRepoModel>() { }
                        //data da proxima necessidade pendente,
                    };
                    if(itemRetorno.QtdMin > itemRetorno.QtdEmEstoque)
                    {
                        for (var i = 0; i < 3; i++)
                        {
                            if (i < origemRepos.Count() && origemRepos[i] != null)
                            {
                                var lote = lotesDes.Where(x => x.Id == origemRepos[i].StockLotId).FirstOrDefault();
                                if (lote == null)
                                {
                                    await _bus.RaiseEvent(new DomainNotification("Schema", "stockLot not found"));
                                    return default;
                                }
                                itemRetorno.OrigensReposicao.Add(new OrigensRepoModel
                                {
                                    LocalId = origemRepos[i].StockLocId,
                                    LoteId = origemRepos[i].StockLotId,
                                    LoteCode = origemRepos[i].StockLotCode,
                                    Local = origemRepos[i].StockLocCode,
                                    QtdDisp = origemRepos[i].QtTotal,
                                    Validade = lote.ExpirateDate
                                });
                            }

                        }
                        retornoList.Add(itemRetorno);
                    }
                }


            }
            foreach (var item in retornoList)
            {
                item.OrigensReposicao = item.OrigensReposicao.OrderBy(y => y.Validade).ToList();
            }

            retornoList = retornoList
                .OrderByDescending(item => item.OrigensReposicao != null && item.OrigensReposicao.Any()) // Itens com OrigensReposicao primeiro
                .ThenByDescending(item => item.QtdPendentePicking)
                .ToList();

            retornoList = retornoList.Skip(skipCount).Take((int)limit).ToList();

            return new PaginationData<dynamic>(retornoList, page, limit, listItemRuleModel.Count());
        }
        private async Task<PaginationData<dynamic>> RunStockCommands(string command, int? page, int? limit)
        {
            var listRes = await _context.RunCommandAsync<dynamic>(command);
            if (page == null) page = 1;
            if (limit == null) limit = 30;
            int skipCount = (int)((page - 1) * limit);
            var limitedList = listRes.Skip(skipCount).Take((int)limit).ToList();
            return new PaginationData<dynamic>(limitedList, page, limit, listRes.Count());
        }

        private async Task<PaginationData<dynamic>> RunStockVolCommand(string command, int? page, int? limit)
        {
            var listRes = await _context.RunCommandAsync<dynamic>(command);
            if (page == null) page = 1;
            if (limit == null) limit = 30;
            int skipCount = (int)((page - 1) * limit);
            var limitedList = listRes.Skip(skipCount).Take((int)limit).ToList();
            if (limitedList.Count > 0)
            {
                var sumQtTotal = limitedList.Sum(x => x.Quantidade);
                return new PaginationData<dynamic>(new List<dynamic>() { new { QuantidadeTotal = sumQtTotal }, limitedList }, page, limit, listRes.Count());
            }
            return new PaginationData<dynamic>(new List<dynamic>() { new { QuantidadeTotal = 0 }, limitedList }, page, limit, listRes.Count());
        }
        private static object GetPropertyValue(object obj, string propertyName)
        {
            var propertyInfo = obj.GetType().GetProperty(propertyName);
            if (propertyInfo != null)
            {
                return propertyInfo.GetValue(obj, null);
            }
            return null; // Handle the case where the property is not found
        }
        private async Task<PaginationData<dynamic>> RunStockCommandsMovimento(OperMovFilterModel model, string command, int? page, int? limit)
        {
            var listRes = await _context.RunCommandAsync<dynamic>(command);
            if (page == null) page = 1;
            if (limit == null) limit = 30;
            int skipCount = (int)((page - 1) * limit);
            if (model.GroupBy.HasValue)
            {
                if (model.GroupBy == GroupBy.Volume)
                {
                    var jsonOperMov = JsonConvert.SerializeObject(listRes);
                    List<GetOperMovModel> listOperMov = JsonConvert.DeserializeObject<List<GetOperMovModel>>(jsonOperMov);
                    var limitedListOper = listOperMov.Skip(skipCount).Take((int)limit).ToList();
                    var groupedResult = limitedListOper.GroupBy(x => x.QtdVolumes).ToList();


                    //var limitedListGroup = groupedResult.Skip(skipCount).Take((int)limit).ToList();
                    var retorno = groupedResult.Select(x => new
                    {
                        Total = x.Sum(q => q.QtdVolumes),
                        Movimentos = x.Select(x => new GetOperMovModel
                        {
                            Id = x.Id,
                            CodigoOperacao = x.CodigoOperacao ?? "",
                            DataReal = x.DataReal ?? null,
                            CodigoItem = x.CodigoItem ?? "",
                            DescricaoItem = x.DescricaoItem ?? "",
                            DataMovimento = x.DataMovimento ?? null,
                            Quantidade = x.Quantidade ?? 0.0,
                            Valor = x.Valor ?? 0.0,
                            Unidade = x.Unidade ?? "",
                            CodigoVolume = x.CodigoVolume ?? "",
                            QtdVolumes = x.QtdVolumes ?? 0.0,
                            CodigoLote = x.CodigoLote ?? "",
                            CodigoLocal = x.CodigoLocal ?? "",
                            DescricaoLocal = x.DescricaoLocal ?? ""
                        })
                    }).ToList();

                    return new PaginationData<dynamic>(retorno, page, limit, retorno.Count());
                }
            }

            var limitedList = listRes.Skip(skipCount).Take((int)limit).ToList();
            return new PaginationData<dynamic>(limitedList, page, limit, listRes.Count());
        }
        public async Task<dynamic> GetVol(VolumeFilterModel model)
        {
            string command = ConstantQueries.GetAllVolume(model);
            return await RunStockVolCommand(command, model.Page, model.Limit);
        }

        public async Task<dynamic> GetLote(LoteFilterModel model)
        {
            string command = ConstantQueries.GetAllLote(model);
            return await RunStockVolCommand(command, model.Page, model.Limit);
        }

        public async Task<dynamic> GetAllItems(ItemFilterModel model)
        {
            string command = ConstantQueries.GetAllItem(model);
            return await RunStockVolCommand(command, model.Page, model.Limit);
        }

        public async Task<dynamic> GetOperMov(OperMovFilterModel model)
        {
            string command = ConstantQueries.GetOperMov(model);
            return await RunStockCommandsMovimento(model, command, model.Page, model.Limit);
        }
        public async Task<dynamic> GetAllLocal(LocalFilterModel model)
        {
            string command = ConstantQueries.GetAllLocal(model);
            return await RunStockVolCommand(command, model.Page, model.Limit);
        }
        public async Task<dynamic> GetItemRule(ItemRuleFilterModel model)
        {
            string command = ConstantQueries.GetAllItemRule(model);
            return await RunStockCommands(command, model.Page, model.Limit);
        }

        public async Task<dynamic> GetItemRuleRepo(ItemRuleFilterModel model)
        {
            string command = ConstantQueries.GetAllItemRule(model);
            return await RunItemRepoCommand(command, model.Page, model.Limit);
        }
        public async Task<dynamic> GetItemRuleRepoReab(ItemRuleFilterModel model)
        {
            string command = ConstantQueries.GetAllItemRuleReopReab(model);
            string commandQtdPend = ConstantQueries.GetQtdPendingPicking(model.Familia);
            return await RunItemRepoReabCommand(command, commandQtdPend, model.Page, model.Limit);
        }
        public async Task<dynamic> GetLocalByCode(string code, int? page, int? limit)
        {
            List<dynamic> retorno = new List<dynamic>();
            dynamic stockLocal = await _entityService.GetByField("StockLocal", "code", code);
            if (stockLocal == null)
            {
                await _bus.RaiseEvent(new DomainNotification("Schema", "StockLocal not found"));
                return null;
            }
            var jsonLocal = JsonConvert.SerializeObject(stockLocal);
            StockLocalModel stockLocalDes = JsonConvert.DeserializeObject<StockLocalModel>(jsonLocal);

            var listStockVol = await _entityService.GetListByField("StockVol", "stockLocalCode", code);
            if (listStockVol.Count == 0)
            {
                var newRetorno = new
                {
                    Id = stockLocalDes.Id,
                    Codigo = stockLocalDes.Code,
                    Descricao = stockLocalDes.Description,
                    Disponivel = stockLocalDes.IsAvailable,
                    stockVolId = "",
                    volume = "",
                    DataVolume = "",
                    Fechado = ""

                };
                return new PaginationData<dynamic>(new List<dynamic>() { newRetorno }, page, limit, 1);
            }
            var jsonStockVol = JsonConvert.SerializeObject(listStockVol);
            List<StockVolModel> listStockVolDes = JsonConvert.DeserializeObject<List<StockVolModel>>(jsonStockVol);
            foreach (var vol in listStockVolDes)
            {
                retorno.Add(new
                {
                    Id = stockLocalDes.Id,
                    Codigo = stockLocalDes.Code,
                    Descricao = stockLocalDes.Description,
                    Disponivel = stockLocalDes.IsAvailable,
                    stockVolId = vol.Id,
                    volume = vol.Code,
                    DataVolume = vol.DateVol,
                    Fechado = vol.Closed
                });
            }

            if (page == null) page = 1;
            if (limit == null) limit = 30;
            int skipCount = (int)((page - 1) * limit);
            var limitedList = retorno.Skip(skipCount).Take((int)limit).ToList();
            return new PaginationData<dynamic>(limitedList, page, limit, retorno.Count());
        }

        #endregion
        public async Task<PaginationData<dynamic>> GetFluxoEntrada(StockFilterEntradaModel model)
        {
            string command = ConstantQueries.GetFluxoEntrada(model);
            return await RunStockCommands(command, model.Page, model.Limit);
        }

        public async Task<PaginationData<dynamic>> GetFluxoSaida(StockFilterSaidaModel model)
        {
            string command = ConstantQueries.GetFluxoSaida(model);
            return await RunStockCommands(command, model.Page, model.Limit);
        }
        public async Task ResetBalance()
        {
            await _entityService.DeleteAll("StockBalance");
            string command = ConstantQueries.GetAllSaldoReset();

            var listRes = await _context.RunCommandAsync<dynamic>(command);

            var jsonSaldo = JsonConvert.SerializeObject(listRes);
            List<SaldoModel> listSaldo = JsonConvert.DeserializeObject<List<SaldoModel>>(jsonSaldo);

            var stockBalanceList = listSaldo.Select(saldo => new StockBalanceResetModel
            {
                Id = Guid.NewGuid().ToString(),
                Date = DateTime.Now,
                QtTotal = saldo.totalQuantidade ?? 0,          
                StockLocId = saldo.idLocal,
                StockLocCode = saldo.stockLocalCode,
                StockLotId = saldo.idLote,
                StockLotCode = saldo.code_Lote,
                StockLotVencimento = saldo.validadeLote,
                ItemCode = saldo.itemCode,
                ItemUn = saldo.unidade,
                ItemDescription = saldo.itemDescription,
                ItemId = saldo.idItem,                
                EstoqueAtual = "Sim"
            }).ToList();

            List<Dictionary<string, object>> dataNewBalances = stockBalanceList.Select(model =>
            {
                var dataNewBalance = model.GetType()
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(prop => prop.GetValue(model, null) != null) 
                    .ToDictionary(
                        prop => char.ToLowerInvariant(prop.Name[0]) + prop.Name.Substring(1), 
                        prop => (object)prop.GetValue(model, null)
                    );

                if (!dataNewBalance.ContainsKey("createBy"))
                {
                    dataNewBalance.Add("createBy", "admin@admin.com");  
                }

                return dataNewBalance;
            }).ToList();
            await _entityService.InsertManyBalance("StockBalance", dataNewBalances);
        }
        private async Task UpdateStockVol(string userName, StockOperationModel stockOperationDes, string stockVolId)
        {
            var stockVol = await _entityService.GetById("StockVol", stockVolId);

            if (stockVol == null)
            {
                await _bus.RaiseEvent(new DomainNotification("Schema", "stockVol not found"));
                return;
            }

            var jsonVol = JsonConvert.SerializeObject(stockVol);
            StockVolModel stockVolDes = JsonConvert.DeserializeObject<StockVolModel>(jsonVol);

            stockVolDes.StockLocalId = stockOperationDes.StockLocalToId;
            Dictionary<string, object> dataVol = stockVolDes.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToDictionary(prop => prop.Name, prop => (object)prop.GetValue(stockVolDes, null));
            if (dataVol.Keys.FirstOrDefault(key => key == "updateBy") == null) dataVol.Add("updateBy", userName);
            await _entityService.Update("StockVol", dataVol);

            if (!stockVolDes.StockVolCont.IsNullOrEmpty())
            {
                foreach (var stockvolContId in stockVolDes.StockVolCont)
                {
                    var stockVolCont = await _entityService.GetById("StockVolCont", stockvolContId);
                    if (stockVolCont == null)
                    {
                        await _bus.RaiseEvent(new DomainNotification("Schema", "stockVolCont not found"));
                        return;
                    }
                    var jsonVolCont = JsonConvert.SerializeObject(stockVolCont);
                    StockVolContModel stockVolContDes = JsonConvert.DeserializeObject<StockVolContModel>(jsonVolCont);
                    //verifica saldo de estoque no local origem
                    var stockBalance = _stockBalanceRepository.GetByFields(stockVolContDes.IdItem, stockVolContDes.IdLote, stockOperationDes.StockLocalFromId);
                    if (stockBalance != null)
                    {
                        StockBalanceModel stockBalanceDes = BsonSerializer.Deserialize<StockBalanceModel>(stockBalance);
                        //atualiza saldo de estoque local origem
                        stockBalanceDes.QtTotal -= stockVolContDes.Quantidade;
                 

                        //verifica saldo de estoque do local destino
                        var stockBalanceTo = _stockBalanceRepository.GetByFields(stockVolContDes.IdItem, stockVolContDes.IdLote, stockOperationDes.StockLocalToId);
                        if (stockBalanceTo != null)
                        {
                            StockBalanceModel stockBalanceDesTo = BsonSerializer.Deserialize<StockBalanceModel>(stockBalanceTo);
                            var dataAtual = DateTime.Today.ToString("d");
                            //verifica se saldo do estoque do local destino está na data atual, caso contrário cria-se um novo stockBalance com a data de hoje
                            if (stockBalanceDesTo.Date.Value.ToString("d") == dataAtual)
                            {
                                stockBalanceDesTo.QtTotal += stockVolContDes.Quantidade;
                                Dictionary<string, object> dataBalanceTo = stockBalanceDesTo.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToDictionary(prop => prop.Name, prop => (object)prop.GetValue(stockBalanceDesTo, null));
                                if (dataBalanceTo.Keys.FirstOrDefault(key => key == "updateBy") == null) dataBalanceTo.Add("updateBy", userName);
                                await _entityService.Update("StockBalance", dataBalanceTo);
                            }
                            else
                            {
                                StockBalanceModel model = new StockBalanceModel();
                                model.Id = Guid.NewGuid().ToString();
                                model.Date = DateTime.Now;
                                model.QtTotal = stockVolContDes.Quantidade + stockBalanceDesTo.QtTotal;
                                model.StockLocId = stockOperationDes.StockLocalToId;
                                model.StockLotId = stockVolContDes.IdLote;
                                model.ItemId = stockVolContDes.IdItem;
                                model.EstoqueAtual = "Sim";

                                Dictionary<string, object> dataNewBalance = model.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToDictionary(prop => prop.Name, prop => (object)prop.GetValue(model, null));
                                if (dataNewBalance.Keys.FirstOrDefault(key => key == "createBy") == null) dataNewBalance.Add("createBy", userName);
                                await _entityService.Insert("StockBalance", dataNewBalance);

                                stockBalanceDesTo.EstoqueAtual = "Não";

                                Dictionary<string, object> dataBalanceTo = stockBalanceDesTo.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToDictionary(prop => prop.Name, prop => (object)prop.GetValue(stockBalanceDesTo, null));
                                if (dataBalanceTo.Keys.FirstOrDefault(key => key == "updateBy") == null) dataBalanceTo.Add("updateBy", userName);
                                await _entityService.Update("StockBalance", dataBalanceTo);
                            }

                        }

                        else
                        {
                            StockBalanceModel model = new StockBalanceModel();
                            model.Id = Guid.NewGuid().ToString();
                            model.Date = DateTime.Now;
                            model.QtTotal = stockVolContDes.Quantidade;
                            model.StockLocId = stockOperationDes.StockLocalToId;
                            model.StockLotId = stockVolContDes.IdLote;
                            model.ItemId = stockVolContDes.IdItem;
                            model.EstoqueAtual = "Sim";

                            Dictionary<string, object> dataNewBalance = model.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToDictionary(prop => prop.Name, prop => (object)prop.GetValue(model, null));
                            if (dataNewBalance.Keys.FirstOrDefault(key => key == "createBy") == null) dataNewBalance.Add("createBy", userName);
                            await _entityService.Insert("StockBalance", dataNewBalance);

                            stockBalanceDes.EstoqueAtual = "Não";

                            Dictionary<string, object> dataBalance = stockBalanceDes.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToDictionary(prop => prop.Name, prop => (object)prop.GetValue(stockBalanceDes, null));
                            if (dataBalance.Keys.FirstOrDefault(key => key == "updateBy") == null) dataBalance.Add("updateBy", userName);
                            await _entityService.Update("StockBalance", dataBalance);
                        }

               
                    }
                    else
                    {
                        await _bus.RaiseEvent(new DomainNotification("Item", $"{stockVolContDes.DescriptionItem} não possui saldo no estoque"));
                        return;
                    }
                }
            }


            if (!stockVolDes.StockVolIds.IsNullOrEmpty())
            {
                foreach (var volId in stockVolDes.StockVolIds)
                {
                    var stockVolParent = await _entityService.GetById("StockVolParent", volId);
                    if (stockVolParent == null)
                    {
                        await _bus.RaiseEvent(new DomainNotification("Schema", "stockVolParent not found"));
                        return;
                    }
                    var jsonVolParent = JsonConvert.SerializeObject(stockVolParent);
                    StockVolParentModel stockVolParentDes = JsonConvert.DeserializeObject<StockVolParentModel>(jsonVolParent);
                    if (stockVolParentDes.StockVolId != null)
                    {
                        await UpdateStockVol(userName, stockOperationDes, stockVolParentDes.StockVolId);
                    }

                }
            }
        }
        public async Task UpdatePickingOrder(string codigoPicking, string userName)
        {
            var picking = await _entityService.GetByField("Picking", "codigo", codigoPicking);
            if (picking == null)
            {
                await _bus.RaiseEvent(new DomainNotification("Schema", "Picking not found"));
                return;
            }

            var jsonPicking = JsonConvert.SerializeObject(picking);
            PickingModel pickingDes = JsonConvert.DeserializeObject<PickingModel>(jsonPicking);

            var directive = await _entityService.GetByField("DIRECTIVE", "chave", "WMS_ORDEMPICKING_VOLUME");
            if (directive != null)
            {
                var jsonDirective = JsonConvert.SerializeObject(directive);
                DirectiveModel directiveDes = JsonConvert.DeserializeObject<DirectiveModel>(jsonDirective);
                if (!pickingDes.ItemsPicking.IsNullOrEmpty())
                {
                    var itemsPicking = await _entityService.GetByIdsPickingItem("PickingItem", pickingDes.ItemsPicking, null);

                    if (itemsPicking != null && itemsPicking.Count > 0)
                    {
                        var jsonItemsPicking = JsonConvert.SerializeObject(itemsPicking);
                        List<PickingItemModel> pickingItemsDes = JsonConvert.DeserializeObject<List<PickingItemModel>>(jsonItemsPicking);
                        List<Dictionary<string, object>> listPickingItemUpdate = new List<Dictionary<string, object>>();

                        var localCodes = directiveDes.Valor.Split(',');
                        var stockBalance = await _entityService.GetStockBalanceByLocalCodes("StockBalance", localCodes);


                        if (stockBalance != null && stockBalance.Count > 0)
                        {
                            var jsonStockBalance = JsonConvert.SerializeObject(stockBalance);
                            List<StockBalanceModel> stockBalanceDes = JsonConvert.DeserializeObject<List<StockBalanceModel>>(jsonStockBalance);

                            foreach (var pickingItem in pickingItemsDes)
                            {
                                string localAtual = "";
                                var stockBalanceAtual = stockBalanceDes.Where(x => x.ItemId == pickingItem.ItemId).FirstOrDefault();
                                if (stockBalanceAtual != null) localAtual = stockBalanceAtual.StockLocCode;
                                var indexAtual = localCodes.ToList().IndexOf(localAtual);
                                if (indexAtual == -1)
                                {
                                    var getStockBalance = await _entityService.GetStockBalanceByItemId("StockBalance", pickingItem.ItemId);
                                    if (getStockBalance != null)
                                    {
                                        var getJsonStockBalance = JsonConvert.SerializeObject(getStockBalance);
                                        StockBalanceModel getStockBalanceDes = JsonConvert.DeserializeObject<StockBalanceModel>(getJsonStockBalance);
                                        pickingItem.StockLocCode = getStockBalanceDes.StockLocCode;
                                    }
                                    pickingItem.OrderItem = "";
                                }
                                else
                                {
                                    pickingItem.OrderItem = (indexAtual + 1).ToString();
                                    pickingItem.StockLocCode = localAtual;
                                }

                                Dictionary<string, object> dataPickingItem = pickingItem.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToDictionary(prop => prop.Name, prop => (object)prop.GetValue(pickingItem, null));
                                if (dataPickingItem.Keys.FirstOrDefault(key => key == "updateBy") == null) dataPickingItem.Add("updateBy", userName);
                                listPickingItemUpdate.Add(dataPickingItem);
                                //await _entityService.Update("PickingItem", dataPickingItem);
                            }
                            if (listPickingItemUpdate.Count() > 0)
                            {
                                await _entityService.UpdateMany("PickingItem", listPickingItemUpdate);

                                pickingDes.Status = "Em Execução";
                                pickingDes.UserInitPicking = userName;
                                pickingDes.DateInitPicking = DateTime.Now;
                                Dictionary<string, object> dataPicking = pickingDes.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToDictionary(prop => prop.Name, prop => (object)prop.GetValue(pickingDes, null));
                                if (dataPicking.Keys.FirstOrDefault(key => key == "updateBy") == null) dataPicking.Add("updateBy", userName);
                                await _entityService.Update("Picking", dataPicking);
                            }
                        }
                    }
                }
            }

        }
        public async Task SetRealNew(string stockOperationId, string userName)
        {
            List<string> stockVolContIds = new List<string>();
            var stockOperation = await _entityService.GetById("StockOperation", stockOperationId); //operação de estoque

            if (stockOperation == null)
            {
                await _bus.RaiseEvent(new DomainNotification("Schema", "stockOperation not found"));
                return;
            }

            var jsonstockOperation = JsonConvert.SerializeObject(stockOperation);
            StockOperationModel stockOperationDes = JsonConvert.DeserializeObject<StockOperationModel>(jsonstockOperation);
            if (stockOperationDes.Status != "Pendente")
            {
                await _bus.RaiseEvent(new DomainNotification("Schema", "status not pending"));
                return;
            }
            var stockOperationType = await _entityService.GetById("StockOperationType", stockOperationDes.StockOperationTypeId);

            if (stockOperationType == null)
            {
                await _bus.RaiseEvent(new DomainNotification("Schema", "stockOperationType not found"));
                return;
            }
            var jsonStockOperationType = JsonConvert.SerializeObject(stockOperationType);
            StockOperationTypeModel stockOperationTypeDes = JsonConvert.DeserializeObject<StockOperationTypeModel>(jsonStockOperationType);

            var stockMovs = await _entityService.GetByIdsList("StockMov", stockOperationDes.StockMovIds.ToList());
            if (stockMovs == null || stockMovs.Count == 0)
            {
                await _bus.RaiseEvent(new DomainNotification("Schema", "stockMov not found"));
                return;
            }
            var jsonstockMovs = JsonConvert.SerializeObject(stockMovs);
            List<StockMovModel> stockMovDesList = JsonConvert.DeserializeObject<List<StockMovModel>>(jsonstockMovs);

            foreach (var stockMovDes in stockMovDesList)
            {
                if (stockOperationTypeDes.Type == "Transfer" && stockOperationTypeDes.Description == "Desmontagem de Volume")
                {

                    var stockVol = await _entityService.GetById("StockVol", stockMovDes.StockVolIdOrigem);

                    if (stockVol == null)
                    {
                        await _bus.RaiseEvent(new DomainNotification("Schema", "stockVol not found"));
                        return;
                    }

                    var jsonVol = JsonConvert.SerializeObject(stockVol);
                    StockVolModel stockVolDes = JsonConvert.DeserializeObject<StockVolModel>(jsonVol);
                    if (stockVolDes.Type == "Único")
                    {
                        await _bus.RaiseEvent(new DomainNotification("StockVol", "Não é possível desmontar volume único"));
                        return;
                    }

                    //insere stockvolCont
                    StockVolContModel contModel = new StockVolContModel();
                    contModel.Id = Guid.NewGuid().ToString();
                    contModel.IdLote = stockMovDes.StockLotId;
                    contModel.IdItem = stockMovDes.ItemId;
                    contModel.Quantidade = stockMovDes.Qty;
                    Dictionary<string, object> data = contModel.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToDictionary(prop => prop.Name, prop => (object)prop.GetValue(contModel, null));
                    if (data.Keys.FirstOrDefault(key => key == "createBy") == null) data.Add("createBy", userName);
                    await _entityService.Insert("StockVolCont", data);


                    //consultar volume com #CodeNewVol, verificar se volume consultado pussui itens vinculados, caso possua criar um volume novo caso contrário vincular item ao volume existente
                    StockVolModel? stockVolByCodeModel = null;
                    if (!stockMovDes.CodeNewVol.IsNullOrEmpty())
                    {
                        var stockVolByCode = await _entityService.GetByField("StockVol", "code", stockMovDes.CodeNewVol);

                        if (stockVolByCode != null)
                        {
                            var jsonVolByCode = JsonConvert.SerializeObject(stockVolByCode);
                            stockVolByCodeModel = JsonConvert.DeserializeObject<StockVolModel>(jsonVolByCode);
                        }
                    }
                    stockVolContIds.Add(contModel.Id);
                    string stockVolId = "";
                    // se o volume possuir item e VirginTag == "Sim" retornar erro Etiqueta Virgem já vinculada
                    if (stockVolByCodeModel != null && !stockVolByCodeModel.StockVolCont.IsNullOrEmpty() && stockVolByCodeModel.VirginTag == "Sim")
                    {
                        await _bus.RaiseEvent(new DomainNotification("StockVol", "Etiqueta Virgem já vinculada"));
                        return;
                    }

                    // se o volume possuir item e VirginTag == "Não"  criar volume
                    // se volume não possuir item e VirginTag == "Não" Criar Volume
                    // se o volume não possuir item e VirginTag == "Sim" vincular item no volume

                    if ((stockVolByCodeModel != null && (stockVolByCodeModel.VirginTag == "Não" || stockVolByCodeModel.VirginTag == "")) || stockVolByCodeModel == null)
                    {
                        StockVolModel volModel = new StockVolModel();
                        volModel.Id = Guid.NewGuid().ToString();
                        volModel.Type = stockVolDes.Type;
                        volModel.Code = stockMovDes.CodeNewVol != null ? stockMovDes.CodeNewVol : await _generatorsService.GetAutoincAsync("StockVol", "code", "");
                        volModel.StockLocalId = stockOperationDes.StockLocalFromId;
                        volModel.StockVolCont = stockVolContIds.ToArray();
                        volModel.DateVol = DateTime.Now;
                        volModel.BlockedDate = null;
                        volModel.OrigemId = stockVolDes.OrigemId;
                        volModel.OrigemCode = stockVolDes.OrigemCode;
                        volModel.VirginTag = "Não";
                        Dictionary<string, object> dataVol = volModel.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToDictionary(prop => prop.Name, prop => (object)prop.GetValue(volModel, null));
                        if (dataVol.Keys.FirstOrDefault(key => key == "createBy") == null) dataVol.Add("createBy", userName);
                        await _entityService.Insert("StockVol", dataVol);
                        stockVolId = volModel.Id;
                    }

                    else if (stockVolByCodeModel != null && stockVolByCodeModel.StockVolCont.IsNullOrEmpty() && stockVolByCodeModel.VirginTag == "Sim")
                    {
                        stockVolByCodeModel.Type = stockVolDes.Type;
                        stockVolByCodeModel.Code = stockMovDes.CodeNewVol != null ? stockMovDes.CodeNewVol : await _generatorsService.GetAutoincAsync("StockVol", "code", "");
                        stockVolByCodeModel.OrigemId = stockVolDes.OrigemId;
                        stockVolByCodeModel.OrigemCode = stockVolDes.OrigemCode;
                        //  stockVolByCodeModel.StockLocalId = stockOperationDes.StockLocalFromId;
                        stockVolByCodeModel.StockVolCont = stockVolContIds.ToArray();
                        stockVolByCodeModel.DateVol = DateTime.Now;
                        stockVolByCodeModel.BlockedDate = null;

                        Dictionary<string, object> dataVolByCode = stockVolByCodeModel.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToDictionary(prop => prop.Name, prop => (object)prop.GetValue(stockVolByCodeModel, null));
                        if (dataVolByCode.Keys.FirstOrDefault(key => key == "updateBy") == null) dataVolByCode.Add("updateBy", userName);
                        await _entityService.Update("StockVol", dataVolByCode);
                        stockVolId = stockVolByCodeModel.Id;
                    }
                    if (stockVolDes.StockVolCont == null) stockVolDes.StockVolCont = stockVolContIds.ToArray();
                    stockVolContIds.Clear();

                    foreach (var stockVolContId in stockVolDes.StockVolCont)
                    {
                        var stockVolCont = await _entityService.GetById("StockVolCont", stockVolContId);
                        if (stockVolCont == null)
                        {
                            await _bus.RaiseEvent(new DomainNotification("Schema", "stockVolCont not found"));
                            return;
                        }
                        var jsonVolCont = JsonConvert.SerializeObject(stockVolCont);
                        StockVolContModel stockVolContDes = JsonConvert.DeserializeObject<StockVolContModel>(jsonVolCont);


                        if (stockMovDes.ItemId == stockVolContDes.IdItem)
                        {
                            var qtdVol = stockVolContDes.Quantidade - contModel.Quantidade;
                            //if (qtdVol == 0)
                            //{
                            //    //verificar a possibiliadade de excluir o volume
                            //}
                            stockVolContDes.Quantidade = qtdVol;
                            Dictionary<string, object> dataVolCont = stockVolContDes.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToDictionary(prop => prop.Name, prop => (object)prop.GetValue(stockVolContDes, null));
                            if (dataVolCont.Keys.FirstOrDefault(key => key == "updateBy") == null) dataVolCont.Add("updateBy", userName);
                            await _entityService.Update("StockVolCont", dataVolCont);
                        }
                    }
                    //atualiza stockMov
                    stockMovDes.StockVolId = stockVolId;
                    stockMovDes.DateMov = DateTime.Now;
                    Dictionary<string, object> dataMov = stockMovDes.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToDictionary(prop => prop.Name, prop => (object)prop.GetValue(stockMovDes, null));
                    if (dataMov.Keys.FirstOrDefault(key => key == "updateBy") == null) dataMov.Add("updateBy", userName);
                    await _entityService.Update("StockMov", dataMov);
                }

                else if (stockOperationTypeDes.Type == "Transfer" && stockOperationTypeDes.Description == "Transferência de Local")
                {
                    if (stockOperationDes.StockLocalToId == null)
                    {
                        await _bus.RaiseEvent(new DomainNotification("Schema", "stockLocalTo not found"));
                        return;
                    }
                    var stockVol = await _entityService.GetById("StockVol", stockMovDes.StockVolIdOrigem);

                    if (stockVol == null)
                    {
                        await _bus.RaiseEvent(new DomainNotification("Schema", "stockVol not found"));
                        return;
                    }

                    var jsonVol = JsonConvert.SerializeObject(stockVol);
                    StockVolModel stockVolDes = JsonConvert.DeserializeObject<StockVolModel>(jsonVol);
                    if (!stockVolDes.StockVolIds.IsNullOrEmpty())
                    {
                        await UpdateStockVol(userName, stockOperationDes, stockMovDes.StockVolIdOrigem);
                    }
                }
                else if (stockOperationTypeDes.Type == "Transfer")
                {
                    if (stockOperationDes.StockLocalToId == null)
                    {
                        await _bus.RaiseEvent(new DomainNotification("Schema", "stockLocalTo not found"));
                        return;
                    }
                    var stockVol = await _entityService.GetById("StockVol", stockMovDes.StockVolId);

                    if (stockVol == null)
                    {
                        await _bus.RaiseEvent(new DomainNotification("Schema", "stockVol not found"));
                        return;
                    }

                    var jsonVol = JsonConvert.SerializeObject(stockVol);
                    StockVolModel stockVolDes = JsonConvert.DeserializeObject<StockVolModel>(jsonVol);


                    StockVolContModel contModel = new StockVolContModel();
                    contModel.Id = Guid.NewGuid().ToString();
                    contModel.IdLote = stockMovDes.StockLotId;
                    contModel.IdItem = stockMovDes.ItemId;
                    contModel.Quantidade = stockMovDes.Qty;


                    Dictionary<string, object> data = contModel.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToDictionary(prop => prop.Name, prop => (object)prop.GetValue(contModel, null));
                    if (data.Keys.FirstOrDefault(key => key == "createBy") == null) data.Add("createBy", userName);
                    await _entityService.Insert("StockVolCont", data);


                    stockVolContIds.Add(contModel.Id);
                    StockVolModel volModel = new StockVolModel();
                    volModel.Id = Guid.NewGuid().ToString();
                    volModel.Type = stockVolDes.Type;

                    volModel.Code = stockVolDes.Code;
                    //volModel.Code = !string.IsNullOrEmpty(stockMovDes.CodeNewVol) ? stockMovDes.CodeNewVol : await _generatorsService.GetAutoincAsync("StockVol", "code", "");

                    //else { volModel.Code = stockVolDes.Code; }
                    volModel.StockLocalId = stockOperationDes.StockLocalToId;
                    volModel.StockVolCont = stockVolContIds.ToArray();
                    volModel.DateVol = DateTime.Now;
                    volModel.BlockedDate = null;
                    volModel.OrigemId = stockVolDes.OrigemId;
                    volModel.OrigemCode = stockVolDes.OrigemCode;
                    volModel.VirginTag = "Não";
                    Dictionary<string, object> dataVol = volModel.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToDictionary(prop => prop.Name, prop => (object)prop.GetValue(volModel, null));
                    if (dataVol.Keys.FirstOrDefault(key => key == "createBy") == null) dataVol.Add("createBy", userName);
                    await _entityService.Insert("StockVol", dataVol);

                    stockVolContIds.Clear();

                    foreach (var stockVolContId in stockVolDes.StockVolCont)
                    {
                        var stockVolCont = await _entityService.GetById("StockVolCont", stockVolContId);
                        if (stockVolCont == null)
                        {
                            await _bus.RaiseEvent(new DomainNotification("Schema", "stockVolCont not found"));
                            return;
                        }
                        var jsonVolCont = JsonConvert.SerializeObject(stockVolCont);
                        StockVolContModel stockVolContDes = JsonConvert.DeserializeObject<StockVolContModel>(jsonVolCont);


                        if (stockMovDes.ItemId == stockVolContDes.IdItem)
                        {
                            var qtdVol = stockVolContDes.Quantidade - contModel.Quantidade;
                            //if (qtdVol == 0)
                            //{
                            //    //verificar a possibiliadade de excluir o volume
                            //}
                            stockVolContDes.Quantidade = qtdVol;
                            Dictionary<string, object> dataVolCont = stockVolContDes.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToDictionary(prop => prop.Name, prop => (object)prop.GetValue(stockVolContDes, null));
                            if (dataVolCont.Keys.FirstOrDefault(key => key == "updateBy") == null) dataVolCont.Add("updateBy", userName);
                            await _entityService.Update("StockVolCont", dataVolCont);
                        }
                    }


                    var stockBalance = _stockBalanceRepository.GetByFields(stockMovDes.ItemId, stockMovDes.StockLotId, stockMovDes.StockLocalFromId);
                    if (stockBalance != null)
                    {
                        StockBalanceModel stockBalanceDesOld = BsonSerializer.Deserialize<StockBalanceModel>(stockBalance);

                        var stockBalance2 = _stockBalanceRepository.GetByFields(stockMovDes.ItemId, stockMovDes.StockLotId, stockOperationDes.StockLocalToId);
                        if (stockBalance2 != null)
                        {
                            StockBalanceModel stockBalanceDes2 = BsonSerializer.Deserialize<StockBalanceModel>(stockBalance2);

                            stockBalanceDes2.QtTotal += stockMovDes.Qty;
                            Dictionary<string, object> data2 = stockBalanceDes2.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToDictionary(prop => prop.Name, prop => (object)prop.GetValue(stockBalanceDes2, null));
                            if (data2.Keys.FirstOrDefault(key => key == "updateBy") == null) data2.Add("updateBy", userName);
                            await _entityService.Update("StockBalance", data2);
                        }
                        else
                        {
                            StockBalanceModel stockBalanceDes = BsonSerializer.Deserialize<StockBalanceModel>(stockBalance);

                            StockBalanceModel model = new StockBalanceModel();
                            model.Id = Guid.NewGuid().ToString();
                            model.Date = DateTime.Now;
                            model.StockLocId = stockOperationDes.StockLocalToId;
                            model.StockLotId = stockMovDes.StockLotId;
                            model.ItemId = stockMovDes.ItemId;
                            model.QtTotal = stockMovDes.Qty;
                            model.EstoqueAtual = "Sim";

                            Dictionary<string, object> dataBalance = model.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToDictionary(prop => prop.Name, prop => (object)prop.GetValue(model, null));
                            if (dataBalance.Keys.FirstOrDefault(key => key == "createBy") == null) dataBalance.Add("createBy", userName);
                            await _entityService.Insert("StockBalance", dataBalance);

                            stockBalanceDesOld.EstoqueAtual = "Não";
                        }

                        //atualiza stockMov
                        stockMovDes.DateMov = DateTime.Now;
                        Dictionary<string, object> dataMov1 = stockMovDes.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToDictionary(prop => prop.Name, prop => (object)prop.GetValue(stockMovDes, null));
                        if (dataMov1.Keys.FirstOrDefault(key => key == "updateBy") == null) dataMov1.Add("updateBy", userName);
                        await _entityService.Update("StockMov", dataMov1);

            
                        stockBalanceDesOld.QtTotal = stockBalanceDesOld.QtTotal - stockMovDes.Qty;
           
                        Dictionary<string, object> dataOld = stockBalanceDesOld.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToDictionary(prop => prop.Name, prop => (object)prop.GetValue(stockBalanceDesOld, null));
                        if (dataOld.Keys.FirstOrDefault(key => key == "updateBy") == null) dataOld.Add("updateBy", userName);
                        await _entityService.Update("StockBalance", dataOld);
                    }

                }
                else
                {

                    bool isInsert = false;
                    var stockBalance = _stockBalanceRepository.GetByFields(stockMovDes.ItemId, stockMovDes.StockLotId, stockOperationDes.StockLocalFromId);
                    if (stockBalance == null)
                    {
                        isInsert = true;
                        stockBalance = _stockBalanceRepository.GetByFields(stockMovDes.ItemId, stockMovDes.StockLotId, stockOperationDes.StockLocalToId);
                    }
                    if (stockBalance != null)
                    {
                        StockBalanceModel stockBalanceDes = BsonSerializer.Deserialize<StockBalanceModel>(stockBalance);

                        if (stockOperationTypeDes.Type == "Input")
                        {
                            stockBalanceDes.QtTotal += stockMovDes.Qty;

                            var nf = await _entityService.GetByField("FisDocRec", "stockOperation", stockOperationId);
                            if (nf != null)
                            {
                                var jsonNf = JsonConvert.SerializeObject(nf);
                                NotaFiscalModel nfDes = JsonConvert.DeserializeObject<NotaFiscalModel>(jsonNf);
                                if (!nfDes.fisDocRecItems.IsNullOrEmpty())
                                {
                                    var nfItems = await _entityService.GetByIdsList("FisDocRecItem", nfDes.fisDocRecItems.ToList());
                                    var jsonNfItem = JsonConvert.SerializeObject(nfItems);
                                    if (nfItems.Count > 0)
                                    {
                                        List<NotaFiscalItemModel> nfItemsDes = JsonConvert.DeserializeObject<List<NotaFiscalItemModel>>(jsonNfItem);
                                        nfItemsDes = nfItemsDes
                                            .Where(x => x.ItemId == stockMovDes.ItemId)
                                            .Select(x =>
                                            {
                                                if (x.Qtd_recebido != null) x.Qtd_recebido += stockMovDes.Qty;
                                                else x.Qtd_recebido = stockMovDes.Qty;
                                                return x;
                                            })
                                            .ToList();
                                        foreach (var nfItem in nfItemsDes)
                                        {
                                            Dictionary<string, object> dataNfItem = nfItem.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToDictionary(prop => prop.Name, prop => (object)prop.GetValue(nfItem, null));
                                            if (dataNfItem.Keys.FirstOrDefault(key => key == "updateBy") == null) dataNfItem.Add("updateBy", userName);
                                            await _entityService.Update("FisDocRecItem", dataNfItem);
                                        }
                                        if (nfItemsDes.Any(x => x.Qtd_recebido < x.Qty))
                                        {
                                            nfDes.Status = "Parcialmente Realizado";
                                            Dictionary<string, object> dataNf = nfDes.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToDictionary(prop => prop.Name, prop => (object)prop.GetValue(nfDes, null));
                                            if (dataNf.Keys.FirstOrDefault(key => key == "updateBy") == null) dataNf.Add("updateBy", userName);
                                            await _entityService.Update("FisDocRec", dataNf);
                                        }
                                    }

                                }
                            }

                        }
                        if (stockOperationTypeDes.Type == "Output")
                        {

                            var stockVol = await _entityService.GetById("StockVol", stockMovDes.StockVolId);

                            if (stockVol == null)
                            {
                                await _bus.RaiseEvent(new DomainNotification("Schema", "stockVol not found"));
                                return;
                            }

                            var jsonVol = JsonConvert.SerializeObject(stockVol);
                            StockVolModel stockVolDes = JsonConvert.DeserializeObject<StockVolModel>(jsonVol);

                            foreach (var stockVolContId in stockVolDes.StockVolCont)
                            {
                                var stockVolCont = await _entityService.GetById("StockVolCont", stockVolContId);
                                if (stockVolCont == null)
                                {
                                    await _bus.RaiseEvent(new DomainNotification("Schema", "stockVolCont not found"));
                                    return;
                                }
                                var jsonVolCont = JsonConvert.SerializeObject(stockVolCont);
                                StockVolContModel stockVolContDes = JsonConvert.DeserializeObject<StockVolContModel>(jsonVolCont);


                                if (stockMovDes.ItemId == stockVolContDes.IdItem)
                                {
                                    stockVolContDes.Quantidade -= stockMovDes.Qty;
                                    Dictionary<string, object> dataVolCont = stockVolContDes.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToDictionary(prop => prop.Name, prop => (object)prop.GetValue(stockVolContDes, null));
                                    if (dataVolCont.Keys.FirstOrDefault(key => key == "updateBy") == null) dataVolCont.Add("updateBy", userName);
                                    await _entityService.Update("StockVolCont", dataVolCont);
                                }
                            }

                            stockBalanceDes.QtTotal -= stockMovDes.Qty;
                            Dictionary<string, object> dataBalanceTo = stockBalanceDes.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToDictionary(prop => prop.Name, prop => (object)prop.GetValue(stockBalanceDes, null));
                            if (dataBalanceTo.Keys.FirstOrDefault(key => key == "updateBy") == null) dataBalanceTo.Add("updateBy", userName);
                            await _entityService.Update("StockBalance", dataBalanceTo);

                        }

                        if (isInsert && stockOperationTypeDes.Type != "Output")
                        {
                            var schema = _schemaRepository.GetByField("name", "StockBalance");
                   

                            //criaStockBalance
                            StockBalanceModel model = new StockBalanceModel();
                            model.Id = Guid.NewGuid().ToString();
                            model.Date = DateTime.Now;
                            model.QtTotal = stockBalanceDes.QtTotal;
                            model.StockLocId = stockBalanceDes.StockLocId;
                            model.StockLotId = stockBalanceDes.StockLotId;
                            model.ItemId = stockBalanceDes.ItemId;
                            model.EstoqueAtual = "Sim";

                      
                            var schemaProp = JsonConvert.DeserializeObject<Simjob.Framework.Infra.Domain.Models.SchemaModel>(schema.JsonValue).Properties;

                            Dictionary<string, object> data = model.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToDictionary(prop => prop.Name, prop => (object)prop.GetValue(model, null));
                            foreach (var prop in schemaProp)
                            {
                                if (prop.Value.AutoInc)
                                {
                                    var autoIncField = prop.Key.Replace(prop.Key[0], char.ToUpper(prop.Key[0]));
                                    if (!data.ContainsKey(prop.Key))
                                    {
                                        var generatedAutoInc = await _generatorsService.GetAutoincAsync("StockBalance", autoIncField, "");
                                        if (generatedAutoInc != null) data.Add(prop.Key, generatedAutoInc);
                                    }
                                }
                            }
                            if (data.Keys.FirstOrDefault(key => key == "createBy") == null) data.Add("createBy", userName);
                            await _entityService.Insert("StockBalance", data);

                            //atualiza stockBalanceOld
                            stockBalanceDes.EstoqueAtual = "Não";
                            Dictionary<string, object> dataOld = stockBalanceDes.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToDictionary(prop => prop.Name, prop => (object)prop.GetValue(stockBalanceDes, null));
                            if (dataOld.Keys.FirstOrDefault(key => key == "updateBy") == null) dataOld.Add("updateBy", userName);
                            await _entityService.Update("StockBalance", dataOld);
                        }
                        else
                        {
                            var schema = _schemaRepository.GetByField("name", "StockBalance");
                            Dictionary<string, object> data = stockBalanceDes.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToDictionary(prop => prop.Name, prop => (object)prop.GetValue(stockBalanceDes, null));
                            if (data.Keys.FirstOrDefault(key => key == "updateBy") == null) data.Add("updateBy", userName);
                            await _entityService.Update("StockBalance", data);
                        }


                    }
                    else
                    {
                        //criaStockBalance
                        StockBalanceModel model = new StockBalanceModel();
                        model.Id = Guid.NewGuid().ToString();
                        model.Date = DateTime.Now;
                        model.QtTotal = stockMovDes.Qty;
                        model.StockLocId = stockOperationDes.StockLocalToId;
                        model.StockLotId = stockMovDes.StockLotId;
                        model.ItemId = stockMovDes.ItemId;
                        model.EstoqueAtual = "Sim";

                        Dictionary<string, object> data = model.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToDictionary(prop => prop.Name, prop => (object)prop.GetValue(model, null));
                        if (data.Keys.FirstOrDefault(key => key == "createBy") == null) data.Add("createBy", userName);
                        await _entityService.Insert("StockBalance", data);


                        if (stockOperationTypeDes.Type == "Input")
                        {

                            var nf = await _entityService.GetByField("FisDocRec", "stockOperation", stockOperationId);
                            if (nf != null)
                            {
                                var jsonNf = JsonConvert.SerializeObject(nf);
                                NotaFiscalModel nfDes = JsonConvert.DeserializeObject<NotaFiscalModel>(jsonNf);
                                if (!nfDes.fisDocRecItems.IsNullOrEmpty())
                                {
                                    var nfItems = await _entityService.GetByIdsList("FisDocRecItem", nfDes.fisDocRecItems.ToList());
                                    var jsonNfItem = JsonConvert.SerializeObject(nfItems);
                                    if (nfItems.Count > 0)
                                    {
                                        List<NotaFiscalItemModel> nfItemsDes = JsonConvert.DeserializeObject<List<NotaFiscalItemModel>>(jsonNfItem);
                                        nfItemsDes = nfItemsDes
                                            .Where(x => x.ItemId == stockMovDes.ItemId)
                                            .Select(x =>
                                            {
                                                if (x.Qtd_recebido != null) x.Qtd_recebido += stockMovDes.Qty;
                                                else x.Qtd_recebido = stockMovDes.Qty;
                                                return x;
                                            })
                                            .ToList();
                                        foreach (var nfItem in nfItemsDes)
                                        {
                                            Dictionary<string, object> dataNfItem = nfItem.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToDictionary(prop => prop.Name, prop => (object)prop.GetValue(nfItem, null));
                                            if (dataNfItem.Keys.FirstOrDefault(key => key == "updateBy") == null) dataNfItem.Add("updateBy", userName);
                                            await _entityService.Update("FisDocRecItem", dataNfItem);
                                        }
                                        if (nfItemsDes.Any(x => x.Qtd_recebido < x.Qty))
                                        {
                                            nfDes.Status = "Parcialmente Realizado";
                                            Dictionary<string, object> dataNf = nfDes.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToDictionary(prop => prop.Name, prop => (object)prop.GetValue(nfDes, null));
                                            if (dataNf.Keys.FirstOrDefault(key => key == "updateBy") == null) dataNf.Add("updateBy", userName);
                                            await _entityService.Update("FisDocRec", dataNf);
                                        }
                                    }

                                }
                            }

                        }
                    }
                    //atualiza stockMov
                    stockMovDes.DateMov = DateTime.Now;
                    Dictionary<string, object> dataMov = stockMovDes.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToDictionary(prop => prop.Name, prop => (object)prop.GetValue(stockMovDes, null));
                    if (dataMov.Keys.FirstOrDefault(key => key == "updateBy") == null) dataMov.Add("updateBy", userName);
                    await _entityService.Update("StockMov", dataMov);
                }

            }
            int documentCount = 0;
            //se for Input verifica se todos os items foram processados e atualiza o status
            if (stockOperationTypeDes.Type == "Input")
            {
                string command = ConstantQueries.GetAllNfByStockOperation(stockOperationId);
                var listRes = await _context.RunCommandAsync<dynamic>(command);
                if (listRes.Count > 0) documentCount = (int)listRes[0].totalDocumentCount;
            }
            stockOperationDes.Status = documentCount > 0 ? "Parcialmente Realizado" : "Realizado";
            stockOperationDes.DateReal = DateTime.Now;
            Dictionary<string, object> dataOperation = stockOperationDes.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToDictionary(prop => prop.Name, prop => (object)prop.GetValue(stockOperationDes, null));
            if (dataOperation.Keys.FirstOrDefault(key => key == "updateBy") == null) dataOperation.Add("updateBy", userName);
            await _entityService.Update("StockOperation", dataOperation);
        }


        public async Task UpdateStockLotBalance(string stockLotId, string itemId)
        {

            var stockLotBalanceSchema = _entityService.GetSchemaByName("StockLotBalance");

            if (stockLotBalanceSchema == null)
            {
                await _bus.RaiseEvent(new DomainNotification("Schema", "Schema type not exists"));
                return;
            }

            var inputAvailable = await GetSumLotBalanceAsync(stockLotId, true, true, null, true);
            var outputAvailable = await GetSumLotBalanceAsync(stockLotId, false, true, null, true);

            var inputTotal = await GetSumLotBalanceAsync(stockLotId, true, true, null, null);
            var outputTotal = await GetSumLotBalanceAsync(stockLotId, false, true, null, null);

            var inputReserved = await GetSumLotBalanceAsync(stockLotId, true, false, true, null);
            var outputReserved = await GetSumLotBalanceAsync(stockLotId, false, false, true, null);

            var inputCommitted = await GetSumLotBalanceAsync(stockLotId, true, false, false, null);
            var outputCommitted = await GetSumLotBalanceAsync(stockLotId, false, false, false, null);

            double resIsAvailable = inputAvailable - outputAvailable;
            double resTotal = inputTotal - outputTotal;
            double resIsReserved = inputReserved - outputReserved;
            double resIsReal = inputCommitted - outputCommitted;

            var stockLotBalanceLast = _stockLotBalanceBsonRepository.GetByStockLotIdLastDiffToday(stockLotId);

            var stockLotBalance = _stockLotBalanceBsonRepository.GetByStockLotIdToday(stockLotId);

            if (stockLotBalanceLast != null)
            {
                var lastQtAvailable = stockLotBalanceLast["qtAvailable"].AsDouble;
                var lastQtTotal = stockLotBalanceLast["qtTotal"].AsDouble;
                var lastQtReserved = stockLotBalanceLast["qtReserved"].AsDouble;
                var lastQtCommitted = stockLotBalanceLast["qtCommitted"].AsDouble;

                resIsAvailable += lastQtAvailable;
                resTotal += lastQtTotal;
                resIsReserved += lastQtReserved;
                resIsReal += lastQtCommitted;
            }

            if (stockLotBalance == null)
            {
                Dictionary<string, object> stockLotBalanceDic = new Dictionary<string, object>();
                stockLotBalanceDic.Add("stockLotId", stockLotId);
                stockLotBalanceDic.Add("itemId", itemId);
                stockLotBalanceDic.Add("date", DateTime.Now.Date);

                stockLotBalanceDic.Add("qtAvailable", resIsAvailable);
                stockLotBalanceDic.Add("qtTotal", resTotal);
                stockLotBalanceDic.Add("qtReserved", resIsReserved);
                stockLotBalanceDic.Add("qtCommitted", resIsReal);

                var insertCommand = new InsertEntityCommand
                {
                    SchemaName = stockLotBalanceSchema.Name,
                    Data = stockLotBalanceDic,
                    SchemaJson = stockLotBalanceSchema.JsonValue
                };

                await _bus.SendCommand(insertCommand);
                return;
            }

            stockLotBalance["qtAvailable"] = resIsAvailable;
            stockLotBalance["qtTotal"] = resTotal;
            stockLotBalance["qtReserved"] = resIsReserved;
            stockLotBalance["qtCommitted"] = resIsReal;

            Dictionary<string, object> dicUpdate = (Dictionary<string, object>)BsonTypeMapper.MapToDotNetValue(stockLotBalance);

            var updateCommand = new UpdateEntityCommand
            {
                Id = (string)dicUpdate["_id"],
                SchemaName = stockLotBalanceSchema.Name,
                Data = dicUpdate,
                SchemaJson = stockLotBalanceSchema.JsonValue
            };

            await _bus.SendCommand(updateCommand);
        }

        public async Task UpdateStockItemBalance(string itemId)
        {

            var stockItemBalanceSchema = _entityService.GetSchemaByName("StockItemBalance");

            if (stockItemBalanceSchema == null)
            {
                await _bus.RaiseEvent(new DomainNotification("Schema", "Schema type not exists"));
                return;
            }

            var stockItemBalanceType = await _schemaBuilder.GetSchemaType("StockItemBalance");

            var inputAvailable = await GetSumItemBalanceAsync(itemId, true, true, null, true);
            var outputAvailable = await GetSumItemBalanceAsync(itemId, false, true, null, true);

            var inputTotal = await GetSumItemBalanceAsync(itemId, true, true, null, null);
            var outputTotal = await GetSumItemBalanceAsync(itemId, false, true, null, null);

            var inputReserved = await GetSumItemBalanceAsync(itemId, true, false, true, null);
            var outputReserved = await GetSumItemBalanceAsync(itemId, false, false, true, null);

            var inputCommitted = await GetSumItemBalanceAsync(itemId, true, false, false, null);
            var outputCommitted = await GetSumItemBalanceAsync(itemId, false, false, false, null);

            double resIsAvailable = inputAvailable - outputAvailable;
            double resTotal = inputTotal - outputTotal;
            double resIsReserved = inputReserved - outputReserved;
            double resIsReal = inputCommitted - outputCommitted;

            var stockItemBalanceLast = _stockItemBalanceBsonRepository.GetByItemIdLastDiffToday(itemId);

            var stockItemBalance = _stockItemBalanceBsonRepository.GetByItemIdToday(itemId);

            if (stockItemBalanceLast != null)
            {
                var lastQtAvailable = stockItemBalanceLast["qtAvailable"].AsDouble;
                var lastQtTotal = stockItemBalanceLast["qtTotal"].AsDouble;
                var lastQtReserved = stockItemBalanceLast["qtReserved"].AsDouble;
                var lastQtCommitted = stockItemBalanceLast["qtCommitted"].AsDouble;

                resIsAvailable += lastQtAvailable;
                resTotal += lastQtTotal;
                resIsReserved += lastQtReserved;
                resIsReal += lastQtCommitted;
            }

            if (stockItemBalance == null)
            {
                Dictionary<string, object> stockItemBalanceDic = new Dictionary<string, object>();
                stockItemBalanceDic.Add("itemId", itemId);
                stockItemBalanceDic.Add("date", DateTime.Now.Date);

                stockItemBalanceDic.Add("qtAvailable", resIsAvailable);
                stockItemBalanceDic.Add("qtTotal", resTotal);
                stockItemBalanceDic.Add("qtReserved", resIsReserved);
                stockItemBalanceDic.Add("qtCommitted", resIsReal);

                var insertCommand = new InsertEntityCommand
                {
                    SchemaName = stockItemBalanceSchema.Name,
                    Data = stockItemBalanceDic,
                    SchemaJson = stockItemBalanceSchema.JsonValue
                };

                await _bus.SendCommand(insertCommand);
                return;
            }

            stockItemBalance["qtAvailable"] = resIsAvailable;
            stockItemBalance["qtTotal"] = resTotal;
            stockItemBalance["qtReserved"] = resIsReserved;
            stockItemBalance["qtCommitted"] = resIsReal;

            Dictionary<string, object> dicUpdate = (Dictionary<string, object>)BsonTypeMapper.MapToDotNetValue(stockItemBalance);

            var updateCommand = new UpdateEntityCommand
            {
                Id = (string)dicUpdate["_id"],
                SchemaName = stockItemBalanceSchema.Name,
                Data = dicUpdate,
                SchemaJson = stockItemBalanceSchema.JsonValue
            };

            await _bus.SendCommand(updateCommand);
        }

        public async Task<double> GetSumLotBalanceAsync(string stockLotId, bool input, bool? isReal = null, bool? isReserved = null,
                                                           bool? isAvailable = null, DateTime? dtStart = null, DateTime? dtEnd = null)
        {

            var dateStart = dtStart.HasValue ? dtStart.Value.Date : DateTime.Now.Date;
            var dateEnd = dtEnd.HasValue ? dtEnd.Value.Date : dateStart.AddDays(1);

            var nowDateString = dateStart.ToString("yyyy-MM-dd");
            var tomorowString = dateEnd.ToString("yyyy-MM-dd");

            string stockOperationType = input ? "Input" : "Output";
            string filterIsAvailable = isAvailable.HasValue ? $"'stocklocal.isAvailable': {isAvailable.ToString().ToLower()}," : "";
            string filterIsReserved = isReserved.HasValue ? $"'stockoperation.isReserved': {isReserved.ToString().ToLower()}," : "";
            string filterIsReal = isReal.HasValue ? isReal.Value ? "'stockoperation.dateReal': { $ne: null }," : "  'stockoperation.dateReal': { $eq:  null }," : "";


            string filter = $@"
                                    {{           {filterIsAvailable} {filterIsReserved} {filterIsReal}
                                                'stockoperationtype.type' : '{stockOperationType}', 
                                                'stockLotId':'{stockLotId}', 
                                                'date':{{ $gte: ISODate('{nowDateString}'), $lt: ISODate('{tomorowString}')}} 
                                    }}";


            string command = ConstantQueries.GetStockMovSumAmountGroupByItem(filter, "stockLotId");

            var res = await _context.RunCommandAsync<dynamic>(command);

            if (!res.Any())
                return 0;
            else
                return (double)res[0].sum;
        }

        public async Task<double> GetSumItemBalanceAsync(string itemId, bool input, bool? isReal = null, bool? isReserved = null,
                                                            bool? isAvailable = null, DateTime? dtStart = null, DateTime? dtEnd = null)
        {

            var dateStart = dtStart.HasValue ? dtStart.Value.Date : DateTime.Now.Date;
            var dateEnd = dtEnd.HasValue ? dtEnd.Value.Date : dateStart.AddDays(1);

            var nowDateString = dateStart.ToString("yyyy-MM-dd");
            var tomorowString = dateEnd.ToString("yyyy-MM-dd");

            string stockOperationType = input ? "Input" : "Output";
            string filterIsAvailable = isAvailable.HasValue ? $"'stocklocal.isAvailable': {isAvailable.ToString().ToLower()}," : "";
            string filterIsReserved = isReserved.HasValue ? $"'stockoperation.isReserved': {isReserved.ToString().ToLower()}," : "";
            string filterIsReal = isReal.HasValue ? isReal.Value ? "'stockoperation.dateReal': { $ne: null }," : "  'stockoperation.dateReal': { $eq:  null }," : "";


            string filter = $@"
                                    {{           {filterIsAvailable} {filterIsReserved} {filterIsReal}
                                                'stockoperationtype.type' : '{stockOperationType}', 
                                                'itemId':'{itemId}', 
                                                'date':{{ $gte: ISODate('{nowDateString}'), $lt: ISODate('{tomorowString}')}} 
                                    }}";


            string command = ConstantQueries.GetStockMovSumAmountGroupByItem(filter);

            var res = await _context.RunCommandAsync<dynamic>(command);

            if (!res.Any())
                return 0;
            else
                return (double)res[0].sum;
        }

        public async Task Insert(InsertStockOperationModel model)
        {
            DateTime now = DateTime.Now;
            var stockOperationType = await _schemaBuilder.GetSchemaType("StockOperation");
            var stockLotType = await _schemaBuilder.GetSchemaType("StockLot");
            var stockMovType = await _schemaBuilder.GetSchemaType("StockMov");
            var stockMovListType = typeof(List<>).MakeGenericType(stockMovType);
            string stockOperationJson = JsonConvert.SerializeObject(model.StockOperation);
            string stockMovsJson = JsonConvert.SerializeObject(model.StockMovs);

            var stockOperation = JsonConvert.DeserializeObject(stockOperationJson, stockOperationType);
            var stockMovs = JsonConvert.DeserializeObject(stockMovsJson, stockMovListType);

            string stockOperationId = Guid.NewGuid().ToString();
            model.StockOperation.TryGetValue("stockOperationTypeId", out object stockOperationTypeId);
            model.StockOperation.TryGetValue("stockLocalToId", out object stockLocalToId);

            if (model.StockOperation.Keys.FirstOrDefault(key => key == "id") == null)
                model.StockOperation["id"] = stockOperationId;

            var schemaStockLot = _schemaRepository.GetByField("name", "StockLot");
            var schemaStockOperation = _schemaRepository.GetByField("name", "StockOperation");
            var schemaStockMov = _schemaRepository.GetByField("name", "StockMov");

            var stockOperationRepository = _entityService.GetRepository(stockOperationType);
            var stockMovRepository = _entityService.GetRepository(stockOperationType);

            if (model.IsReal)
            {
                if (!model.StockOperation.TryAdd("dateReal", now))
                    model.StockOperation.Add("dateReal", now);


            }

            var stockOperationCommand = new InsertEntityCommand()
            {
                SchemaName = "StockOperation",
                SchemaJson = schemaStockOperation.JsonValue,
                Data = model.StockOperation

            };

            await _bus.SendCommand(stockOperationCommand);

            if (_notifications.HasNotification()) return;


            foreach (var stockMov in model.StockMovs)
            {
                string stockMovId = Guid.NewGuid().ToString();
                stockMov["id"] = stockMovId;
                stockMov["date"] = now;
                stockMov["stockOperationId"] = stockOperationId;
                stockMov["stockOperationTypeId"] = stockOperationTypeId;
                stockMov["stockLocalId"] = stockLocalToId;
                stockMov["isReal"] = model.IsReal;

                if (stockMov.TryGetValue("stockLotCode", out object stockLoteCodeValue))
                {
                    string stockLotCode = (string)stockLoteCodeValue;

                    var stockLotExist = _stockLotBsonRepository.GetByCode(stockLotCode);
                    if (stockLotExist == null)
                    {
                        Dictionary<string, object> newStockLot = new Dictionary<string, object>();
                        string newIdStockLot = Guid.NewGuid().ToString();
                        newStockLot.Add("id", newIdStockLot);
                        newStockLot.Add("code", stockLotCode);
                        newStockLot.Add("stockOperationOriginId", stockOperationId);

                        var insertStockLotCommand = new InsertEntityCommand()
                        {
                            SchemaName = "StockLot",
                            SchemaJson = schemaStockLot.JsonValue,
                            Data = newStockLot
                        };

                        await _bus.SendCommand(insertStockLotCommand);

                        if (_notifications.HasNotification()) return;

                        stockMov["stockLotId"] = newIdStockLot;
                    }
                    else
                    {
                        string stockLotIdExist = stockLotExist["_id"].AsString;
                        stockMov["stockLotId"] = stockLotIdExist;
                    }


                };


                var stockMovCommand = new InsertEntityCommand()
                {
                    SchemaName = "StockMov",
                    SchemaJson = schemaStockMov.JsonValue,
                    Data = stockMov

                };
                await _bus.SendCommand(stockMovCommand);

                if (_notifications.HasNotification()) break;

                await UpdateStockItemBalance((string)stockMov["itemId"]);

                if (stockMov.TryGetValue("stockLotId", out object stockLotId))
                    await UpdateStockLotBalance((string)stockLotId, (string)stockMov["itemId"]);
            }

            if (_notifications.HasNotification()) return;



        }
        //Undo updates
        public async Task UndoSetReal(string stockOperationId)
        {
            List<UpdateEntityCommand> updateCommandList = new List<UpdateEntityCommand>();
            DateTime now = DateTime.Now;

            var stockOperationType = await _schemaBuilder.GetSchemaType("StockOperation");
            var stockMovType = await _schemaBuilder.GetSchemaType("StockMov");
            var stockOperationRepository = _entityService.GetRepository(stockOperationType);
            var stockMovRepository = _entityService.GetRepository(stockMovType);
            var stockMovSchema = _entityService.GetSchemaByName("StockMov");
            var stockOperationSchema = _entityService.GetSchemaByName("StockOperation");

            var stockOperation = _stockOperationBsonRepository.GetById(stockOperationId);

            if (stockOperation == null)
            {
                await _bus.RaiseEvent(new DomainNotification("StockService", "Stock Operation not found"));
                return;
            }

            stockOperation["dateReal"] = new BsonDateTime(now);

            var stockOperationDes = BsonSerializer.Deserialize<Dictionary<string, object>>(stockOperation);


            var updateStockOperationCommand = new UpdateEntityCommand
            {
                Id = (string)stockOperationDes["_id"],
                SchemaName = stockOperationSchema.Name,
                Data = stockOperationDes,
                SchemaJson = stockOperationSchema.JsonValue
            };

            updateCommandList.Add(updateStockOperationCommand);

            var stockMovs = _stockMovBsonRepository.GetByStockOperationId(stockOperationId);

            foreach (var stockMov in stockMovs)
            {
                stockMov["isReal"] = new BsonBoolean(true);
                stockMov["date"] = new BsonDateTime(now);

                var stockMovDes = BsonSerializer.Deserialize<Dictionary<string, object>>(stockMov);

                var updateCommand = new UpdateEntityCommand
                {
                    Id = (string)stockMov["_id"],
                    SchemaName = stockMovSchema.Name,
                    Data = stockMovDes,
                    SchemaJson = stockMovSchema.JsonValue
                };

                updateCommandList.Add(updateCommand);

                await UpdateUndoStockItemBalance(stockMov["itemId"].AsString);

                if (stockMov["stockLotId"] != null)
                    await UpdateUndoStockLotBalance(stockMov["stockLotId"].AsString, stockMov["itemId"].AsString);
            }

            foreach (var updateCommand in updateCommandList)
                await _bus.SendCommand(updateCommand);
        }
        public async Task UpdateUndoStockItemBalance(string itemId)
        {
            var stockItemBalanceSchema = _entityService.GetSchemaByName("StockItemBalance");

            if (stockItemBalanceSchema == null)
            {
                await _bus.RaiseEvent(new DomainNotification("Schema", "Schema type not exists"));
                return;
            }

            var stockItemBalanceType = await _schemaBuilder.GetSchemaType("StockItemBalance");

            var inputAvailable = await GetSumItemBalanceAsync(itemId, true, true, null, true);
            var outputAvailable = await GetSumItemBalanceAsync(itemId, false, true, null, true);

            var inputTotal = await GetSumItemBalanceAsync(itemId, true, true, null, null);
            var outputTotal = await GetSumItemBalanceAsync(itemId, false, true, null, null);

            var inputReserved = await GetSumItemBalanceAsync(itemId, true, false, true, null);
            var outputReserved = await GetSumItemBalanceAsync(itemId, false, false, true, null);

            var inputCommitted = await GetSumItemBalanceAsync(itemId, true, false, false, null);
            var outputCommitted = await GetSumItemBalanceAsync(itemId, false, false, false, null);

            double resIsAvailable = inputAvailable + outputAvailable;
            double resTotal = inputTotal + outputTotal;
            double resIsReserved = inputReserved + outputReserved;
            double resIsReal = inputCommitted + outputCommitted;

            var stockItemBalanceLast = _stockItemBalanceBsonRepository.GetByItemIdLastDiffToday(itemId);

            var stockItemBalance = _stockItemBalanceBsonRepository.GetByItemIdToday(itemId);

            if (stockItemBalanceLast != null)
            {
                var lastQtAvailable = stockItemBalanceLast["qtAvailable"].AsDouble;
                var lastQtTotal = stockItemBalanceLast["qtTotal"].AsDouble;
                var lastQtReserved = stockItemBalanceLast["qtReserved"].AsDouble;
                var lastQtCommitted = stockItemBalanceLast["qtCommitted"].AsDouble;

                resIsAvailable -= lastQtAvailable;
                resTotal -= lastQtTotal;
                resIsReserved -= lastQtReserved;
                resIsReal -= lastQtCommitted;
            }

            if (stockItemBalance == null)
            {
                Dictionary<string, object> stockItemBalanceDic = new Dictionary<string, object>();
                stockItemBalanceDic.Add("itemId", itemId);
                stockItemBalanceDic.Add("date", DateTime.Now.Date);

                stockItemBalanceDic.Add("qtAvailable", resIsAvailable);
                stockItemBalanceDic.Add("qtTotal", resTotal);
                stockItemBalanceDic.Add("qtReserved", resIsReserved);
                stockItemBalanceDic.Add("qtCommitted", resIsReal);

                var insertCommand = new InsertEntityCommand
                {
                    SchemaName = stockItemBalanceSchema.Name,
                    Data = stockItemBalanceDic,
                    SchemaJson = stockItemBalanceSchema.JsonValue
                };

                await _bus.SendCommand(insertCommand);
                return;
            }

            stockItemBalance["qtAvailable"] = resIsAvailable;
            stockItemBalance["qtTotal"] = resTotal;
            stockItemBalance["qtReserved"] = resIsReserved;
            stockItemBalance["qtCommitted"] = resIsReal;

            Dictionary<string, object> dicUpdate = (Dictionary<string, object>)BsonTypeMapper.MapToDotNetValue(stockItemBalance);

            var updateCommand = new UpdateEntityCommand
            {
                Id = (string)dicUpdate["_id"],
                SchemaName = stockItemBalanceSchema.Name,
                Data = dicUpdate,
                SchemaJson = stockItemBalanceSchema.JsonValue
            };

            await _bus.SendCommand(updateCommand);
        }

        public async Task UpdateUndoStockLotBalance(string stockLotId, string itemId)
        {

            var stockLotBalanceSchema = _entityService.GetSchemaByName("StockLotBalance");

            if (stockLotBalanceSchema == null)
            {
                await _bus.RaiseEvent(new DomainNotification("Schema", "Schema type not exists"));
                return;
            }

            var inputAvailable = await GetSumLotBalanceAsync(stockLotId, true, true, null, true);
            var outputAvailable = await GetSumLotBalanceAsync(stockLotId, false, true, null, true);

            var inputTotal = await GetSumLotBalanceAsync(stockLotId, true, true, null, null);
            var outputTotal = await GetSumLotBalanceAsync(stockLotId, false, true, null, null);

            var inputReserved = await GetSumLotBalanceAsync(stockLotId, true, false, true, null);
            var outputReserved = await GetSumLotBalanceAsync(stockLotId, false, false, true, null);

            var inputCommitted = await GetSumLotBalanceAsync(stockLotId, true, false, false, null);
            var outputCommitted = await GetSumLotBalanceAsync(stockLotId, false, false, false, null);

            double resIsAvailable = inputAvailable + outputAvailable;
            double resTotal = inputTotal + outputTotal;
            double resIsReserved = inputReserved + outputReserved;
            double resIsReal = inputCommitted + outputCommitted;

            var stockLotBalanceLast = _stockLotBalanceBsonRepository.GetByStockLotIdLastDiffToday(stockLotId);

            var stockLotBalance = _stockLotBalanceBsonRepository.GetByStockLotIdToday(stockLotId);

            if (stockLotBalanceLast != null)
            {
                var lastQtAvailable = stockLotBalanceLast["qtAvailable"].AsDouble;
                var lastQtTotal = stockLotBalanceLast["qtTotal"].AsDouble;
                var lastQtReserved = stockLotBalanceLast["qtReserved"].AsDouble;
                var lastQtCommitted = stockLotBalanceLast["qtCommitted"].AsDouble;

                resIsAvailable -= lastQtAvailable;
                resTotal -= lastQtTotal;
                resIsReserved -= lastQtReserved;
                resIsReal -= lastQtCommitted;
            }

            if (stockLotBalance == null)
            {
                Dictionary<string, object> stockLotBalanceDic = new Dictionary<string, object>();
                stockLotBalanceDic.Add("stockLotId", stockLotId);
                stockLotBalanceDic.Add("itemId", itemId);
                stockLotBalanceDic.Add("date", DateTime.Now.Date);

                stockLotBalanceDic.Add("qtAvailable", resIsAvailable);
                stockLotBalanceDic.Add("qtTotal", resTotal);
                stockLotBalanceDic.Add("qtReserved", resIsReserved);
                stockLotBalanceDic.Add("qtCommitted", resIsReal);

                var insertCommand = new InsertEntityCommand
                {
                    SchemaName = stockLotBalanceSchema.Name,
                    Data = stockLotBalanceDic,
                    SchemaJson = stockLotBalanceSchema.JsonValue
                };

                await _bus.SendCommand(insertCommand);
                return;
            }

            stockLotBalance["qtAvailable"] = resIsAvailable;
            stockLotBalance["qtTotal"] = resTotal;
            stockLotBalance["qtReserved"] = resIsReserved;
            stockLotBalance["qtCommitted"] = resIsReal;

            Dictionary<string, object> dicUpdate = (Dictionary<string, object>)BsonTypeMapper.MapToDotNetValue(stockLotBalance);

            var updateCommand = new UpdateEntityCommand
            {
                Id = (string)dicUpdate["_id"],
                SchemaName = stockLotBalanceSchema.Name,
                Data = dicUpdate,
                SchemaJson = stockLotBalanceSchema.JsonValue
            };

            await _bus.SendCommand(updateCommand);
        }

        
    }
}

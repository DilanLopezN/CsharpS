using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Simjob.Framework.Application.Controllers;
using Simjob.Framework.Application.Stock.Interfaces;
using Simjob.Framework.Application.Stock.Models;
using Simjob.Framework.Domain.Core.Bus;
using Simjob.Framework.Domain.Core.Notifications;
using Simjob.Framework.Domain.Core.Utils;
using Simjob.Framework.Domain.Interfaces.Repositories;
using Simjob.Framework.Domain.Models;
using Simjob.Framework.Infra.Data.Context;
using Simjob.Framework.Infra.Schemas.Entities;
using Simjob.Framework.Infra.Schemas.Interfaces;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Simjob.Framework.Application.Stock.Controllers
{
    [ExcludeFromCodeCoverage]
    [Authorize]
    public class StockController : BaseController
    {
        private readonly ISchemaBuilder _schemaBuilder;
        private readonly IRepository<MongoDbContext, Schema> _schemaRepository;
        private readonly IServiceProvider _serviceProvider;
        private readonly IStockOperationService _stockOperatorService;
        public StockController(IMediatorHandler bus, INotificationHandler<DomainNotification> notifications, ISchemaBuilder schemaBuilder, IRepository<MongoDbContext, Schema> schemaRepository, IServiceProvider serviceProvider, IStockOperationService stockOperatorService) : base(bus, notifications)
        {
            _schemaBuilder = schemaBuilder;
            _schemaRepository = schemaRepository;
            _serviceProvider = serviceProvider;
            _stockOperatorService = stockOperatorService;
        }
        private object GetRepository(Type schemaType)
        {
            var typeRepo = typeof(IRepository<,>).MakeGenericType(typeof(MongoDbContext), schemaType);
            return _serviceProvider.GetService(typeRepo);
        }

        [HttpPost("stock-operation")]
        public async Task<IActionResult> Post([FromBody] InsertStockOperationModel model)
        {
            await _stockOperatorService.Insert(model);

            return ResponseDefault();
        }


        //[HttpPost("stock-operation/real/{stockOperationId}")]
        //public async Task<IActionResult> SetReal(string stockOperationId)
        //{
        //    await _stockOperatorService.SetReal(stockOperationId);

        //    return ResponseDefault();
        //}

        [HttpPost("stock-operation/real/{stockOperationId}")]
        public async Task<IActionResult> SetReal(string stockOperationId)
        {
            var userName = "";
            try
            {
                var accessToken = Request.Headers[HeaderNames.Authorization];

                var tokenInfo = Util.GetUserInfoFromToken(accessToken);

                if (tokenInfo.Count > 0)
                {

                    userName = tokenInfo["username"];
                }

            }
            catch (Exception)
            {
                throw;
            }
            await _stockOperatorService.SetRealNew(stockOperationId, userName);

            return ResponseDefault();
        }

        [HttpPost("stock-operation/undoreal/{stockOperationId}")]
        public async Task<IActionResult> UndoSetReal(string stockOperationId)
        {
            await _stockOperatorService.UndoSetReal(stockOperationId);

            return ResponseDefault();
        }




        [HttpGet("StockVol")]
        public async Task<IActionResult> GetStockVol([FromQuery] VolumeFilterModel model)
        {
            return ResponseDefault(await _stockOperatorService.GetVol(model));
        }


        [HttpGet("StockLot")]
        public async Task<IActionResult> GetLote([FromQuery] LoteFilterModel model)
        {
            return ResponseDefault(await _stockOperatorService.GetLote(model));
        }

        [HttpGet("Item")]
        public async Task<IActionResult> GetItems([FromQuery] ItemFilterModel model)
        {
            return ResponseDefault(await _stockOperatorService.GetAllItems(model));
        }

        [HttpGet("StockOperation")]
        public async Task<IActionResult> GetOperMov([FromQuery] OperMovFilterModel model)
        {
            return ResponseDefault(await _stockOperatorService.GetOperMov(model));
        }


        [HttpGet("StockLocal")]
        public async Task<IActionResult> GetLocal([FromQuery] LocalFilterModel model)
        {
            return ResponseDefault(await _stockOperatorService.GetAllLocal(model));
        }

        [HttpGet("ItemRule")]
        public async Task<IActionResult> GetItemRuleStockLocal([FromQuery] ItemRuleFilterModel model)
        {
            return ResponseDefault(await _stockOperatorService.GetItemRule(model));
        }

        [HttpGet("ItemRuleRepo")]
        public async Task<IActionResult> GetItemRuleRepo([FromQuery] ItemRuleFilterModel model)
        {
            return ResponseDefault(await _stockOperatorService.GetItemRuleRepo(model));
        }

        [HttpGet("ItemRuleRepoReab")]
        public async Task<IActionResult> GetItemRuleRepoReab([FromQuery] ItemRuleFilterModel model)
        {
            return ResponseDefault(await _stockOperatorService.GetItemRuleRepoReab(model));
        }


        //[HttpGet("stock-item-balance")]
        //public async Task<IActionResult> Get()
        //{
        //    await _stockOperatorService.UpdateStockItemBalance("0d0954ec-6990-40ce-a7be-7a7231fe59ab");
        //    var teste = await _stockOperatorService.GetSumItemBalanceAsync("0d0954ec-6990-40ce-a7be-7a7231fe59ab", true, null, null, true, new DateTime(2021, 6, 28), new DateTime(2021, 6, 29));

        //    return ResponseDefault();
        //}


        [HttpGet("fluxo-saida")]
        public async Task<IActionResult> GetFluxoSaida([FromQuery] StockFilterSaidaModel model)
        {
            return ResponseDefault(await _stockOperatorService.GetFluxoSaida(model));
        }

        [HttpGet("fluxo-entrada")]
        public async Task<IActionResult> GetFluxoEntrada([FromQuery] StockFilterEntradaModel model)
        {
            return ResponseDefault(await _stockOperatorService.GetFluxoEntrada(model));
        }
        [HttpPost("PickingOrder/{codigoPicking}")]
        public async Task<IActionResult> UpdatePickingOrder(string codigoPicking)
        {
            var userName = "";
            try
            {
                var accessToken = Request.Headers[HeaderNames.Authorization];

                var tokenInfo = Util.GetUserInfoFromToken(accessToken);

                if (tokenInfo.Count > 0)
                {

                    userName = tokenInfo["username"];
                }

            }
            catch (Exception)
            {
                throw;
            }

            await _stockOperatorService.UpdatePickingOrder(codigoPicking, userName);

            return ResponseDefault();
        }


        [HttpPost("ResetBalance")]
        public async Task<IActionResult> ResetBalance()
        {
            await _stockOperatorService.ResetBalance();
            return ResponseDefault();
        }

    }
}

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Simjob.Framework.Application.Controllers;
using Simjob.Framework.Domain.Core.Bus;
using Simjob.Framework.Domain.Core.Notifications;
using Simjob.Framework.Domain.Interfaces.Repositories;
using Simjob.Framework.Infra.Data.Context;
using Simjob.Framework.Infra.Identity.Contexts;
using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Schemas.Entities;
using Simjob.Framework.Services.Api.Enums;
using Simjob.Framework.Services.Api.Services;
using System;
using System.Threading.Tasks;

namespace Simjob.Framework.Services.Api.Controllers
{
    public class LocalidadeController : BaseController
    {
        private readonly IRepository<SourceContext, Source> _sourceRepository;
        private readonly IRepository<MongoDbContext, Schema> _schemaRepository;

        public LocalidadeController(IMediatorHandler bus, INotificationHandler<DomainNotification> notifications, IRepository<SourceContext, Source> sourceRepository, IRepository<MongoDbContext, Schema> schemaRepository) : base(bus, notifications)
        {
            _sourceRepository = sourceRepository;
            _schemaRepository = schemaRepository;
        }

        [Authorize]
        [HttpGet("logradouro")]
        public async Task<IActionResult> GetAll(string value, SearchModeEnum mode, int? page, int? limit, string sortField, bool sortDesc = false, string ids = "", string searchFields = null)
        {
            try
            {
                var schemaName = "V_Localidade";
                if (schemaName.Contains("V_")) schemaName = schemaName.Replace("V_", "");
                
                var schema = _schemaRepository.GetSchemaByField("name", schemaName);
                if (schema == null)
                    return BadRequest("Schema não encontrado");

                var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
                var source = _sourceRepository.GetByField("description", schemaModel.Source);

                if (source != null && source.Active != null && source.Active == true)
                {
                    // Define ordenação padrão se não informada
                    if (string.IsNullOrEmpty(sortField))
                    {
                        sortField = "no_bairro,no_localidade";
                    }

                    var localidadeResult = await SQLServerService.GetList("V_LOCALIDADE_LOGRADOURO", page, limit, sortField, sortDesc, ids, searchFields, value, source, mode);

                    if (localidadeResult.success)
                    {
                        var retorno = new
                        {
                            data = localidadeResult.data,
                            localidadeResult.total,
                            page,
                            limit,
                            pages = limit != null ? (int)Math.Ceiling((double)localidadeResult.total / limit.Value) : 0
                        };

                        return ResponseDefault(retorno);
                    }
                    else
                    {
                        return BadRequest(new
                        {
                            error = localidadeResult.error
                        });
                    }
                }

                return BadRequest(new
                {
                    error = "Fonte de dados não configurada ou inativa."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    error = $"Erro interno: {ex.Message}"
                });
            }
        }
    }
}
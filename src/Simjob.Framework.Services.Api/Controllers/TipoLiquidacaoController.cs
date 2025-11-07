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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Simjob.Framework.Services.Api.Controllers
{
    public class TipoLiquidacaoController : BaseController
    {
        private readonly IRepository<SourceContext, Source> _sourceRepository;
        private readonly IRepository<MongoDbContext, Schema> _schemaRepository;

        public TipoLiquidacaoController(
            IMediatorHandler bus,
            INotificationHandler<DomainNotification> notifications,
            IRepository<SourceContext, Source> sourceRepository,
            IRepository<MongoDbContext, Schema> schemaRepository) : base(bus, notifications)
        {
            _sourceRepository = sourceRepository;
            _schemaRepository = schemaRepository;
        }

        /// <summary>
        /// Busca lista de tipos de liquidação
        /// </summary>
        /// <param name="value">Texto para busca (opcional)</param>
        /// <param name="mode">Modo de busca: Contains, Equals, StartsWith (default: Contains)</param>
        /// <param name="page">Número da página (default: 1)</param>
        /// <param name="limit">Limite de registros por página (default: 50)</param>
        /// <param name="sortField">Campo para ordenação (default: dc_tipo_liquidacao)</param>
        /// <param name="sortDesc">Ordenação decrescente (default: false)</param>
        /// <returns>Lista de tipos de liquidação</returns>
        [Authorize]
        [HttpGet()]
        public async Task<IActionResult> GetAll(
            string value = null,
            SearchModeEnum mode = SearchModeEnum.Contains,
            int? page = 1,
            int? limit = 50,
            string sortField = "dc_tipo_liquidacao",
            bool sortDesc = false)
        {
            try
            {
                // Buscar configuração do schema
                var schemaName = "T_Tipo_Liquidacao";
                if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "").Replace("_", "");

                var schema = _schemaRepository.GetSchemaByField("name", schemaName);
                if (schema == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        error = "Schema não encontrado"
                    });
                }

                var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
                var source = _sourceRepository.GetByField("description", schemaModel.Source);

                if (source == null || source.Active != true)
                {
                    return BadRequest(new
                    {
                        success = false,
                        error = "Fonte de dados não configurada ou inativa."
                    });
                }

                // Montar campos de busca
                string searchFields = null;
                string searchValue = null;

                if (!string.IsNullOrEmpty(value))
                {
                    searchFields = "dc_tipo_liquidacao";
                    searchValue = value;
                }

                // Buscar tipos de liquidação
                var tipoLiquidacaoResult = await SQLServerService.GetList(
                    "T_TIPO_LIQUIDACAO",
                    page,
                    limit,
                    sortField,
                    sortDesc,
                    "",
                    searchFields,
                    searchValue,
                    source,
                    mode
                );

                if (tipoLiquidacaoResult.success)
                {
                    var tiposLiquidacao = tipoLiquidacaoResult.data;

                    var retorno = new
                    {
                        data = tiposLiquidacao.Select(x => new
                        {
                            cd_tipo_liquidacao = x["cd_tipo_liquidacao"],
                            dc_tipo_liquidacao = x["dc_tipo_liquidacao"]
                        }).ToList(),
                        total = tipoLiquidacaoResult.total,
                        page,
                        limit,
                        pages = limit != null && limit > 0 ? (int)Math.Ceiling((double)tipoLiquidacaoResult.total / limit.Value) : 0
                    };

                    return ResponseDefault(retorno);
                }

                return BadRequest(new
                {
                    success = false,
                    error = tipoLiquidacaoResult.error ?? "Erro ao buscar tipos de liquidação"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    error = $"Erro interno: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Busca um tipo de liquidação específico por ID
        /// </summary>
        /// <param name="cd_tipo_liquidacao">Código do tipo de liquidação</param>
        /// <returns>Tipo de liquidação específico</returns>
        [Authorize]
        [HttpGet("{cd_tipo_liquidacao}")]
        public async Task<IActionResult> GetById(int cd_tipo_liquidacao)
        {
            try
            {
                // Buscar configuração do schema
                var schemaName = "T_Tipo_Liquidacao";
                if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "").Replace("_", "");

                var schema = _schemaRepository.GetSchemaByField("name", schemaName);
                if (schema == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        error = "Schema não encontrado"
                    });
                }

                var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
                var source = _sourceRepository.GetByField("description", schemaModel.Source);

                if (source == null || source.Active != true)
                {
                    return BadRequest(new
                    {
                        success = false,
                        error = "Fonte de dados não configurada ou inativa."
                    });
                }

                // Buscar tipo de liquidação por ID
                var filtros = new List<(string campo, object valor)>
                {
                    ("cd_tipo_liquidacao", cd_tipo_liquidacao)
                };

                var tipoLiquidacao = await SQLServerService.GetFirstByFields(source, "T_TIPO_LIQUIDACAO", filtros);

                if (tipoLiquidacao == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        error = "Tipo de liquidação não encontrado"
                    });
                }

                var retorno = new
                {
                    data = new
                    {
                        cd_tipo_liquidacao = tipoLiquidacao["cd_tipo_liquidacao"],
                        dc_tipo_liquidacao = tipoLiquidacao["dc_tipo_liquidacao"]
                    }
                };

                return ResponseDefault(retorno);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    error = $"Erro interno: {ex.Message}"
                });
            }
        }
    }
}

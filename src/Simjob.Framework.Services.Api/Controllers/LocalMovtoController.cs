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
    public class LocalMovtoController : BaseController
    {
        private readonly IRepository<SourceContext, Source> _sourceRepository;
        private readonly IRepository<MongoDbContext, Schema> _schemaRepository;

        public LocalMovtoController(
            IMediatorHandler bus,
            INotificationHandler<DomainNotification> notifications,
            IRepository<SourceContext, Source> sourceRepository,
            IRepository<MongoDbContext, Schema> schemaRepository) : base(bus, notifications)
        {
            _sourceRepository = sourceRepository;
            _schemaRepository = schemaRepository;
        }

        /// <summary>
        /// Busca lista de locais de movimento
        /// </summary>
        /// <param name="cd_empresa">Código da empresa (obrigatório)</param>
        /// <param name="value">Texto para busca (opcional)</param>
        /// <param name="mode">Modo de busca: Contains, Equals, StartsWith (default: Contains)</param>
        /// <param name="page">Número da página (default: 1)</param>
        /// <param name="limit">Limite de registros por página (default: 100)</param>
        /// <param name="sortField">Campo para ordenação (default: no_local_movto)</param>
        /// <param name="sortDesc">Ordenação decrescente (default: false)</param>
        /// <returns>Lista de locais de movimento</returns>
        [Authorize]
        [HttpGet()]
        public async Task<IActionResult> GetAll(
            string cd_empresa,
            string value = null,
            SearchModeEnum mode = SearchModeEnum.Contains,
            int? page = 1,
            int? limit = 100,
            string sortField = "no_local_movto",
            bool sortDesc = false)
        {
            // Validação do parâmetro obrigatório
            if (string.IsNullOrEmpty(cd_empresa))
            {
                return BadRequest(new
                {
                    success = false,
                    error = "Campo cd_empresa é obrigatório"
                });
            }

            try
            {
                // Buscar configuração do schema
                var schemaName = "T_Local_Movto";
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
                    searchFields = "no_local_movto";
                    searchValue = value;
                }

                // Buscar locais de movimento
                var localMovtoResult = await SQLServerService.GetList(
                    "T_LOCAL_MOVTO",
                    page,
                    limit,
                    sortField,
                    sortDesc,
                    "",
                    searchFields,
                    searchValue,
                    source,
                    mode,
                    "cd_pessoa_empresa",
                    cd_empresa
                );

                if (localMovtoResult.success)
                {
                    var locaisMovto = localMovtoResult.data;

                    var retorno = new
                    {
                        data = locaisMovto.Select(x => new
                        {
                            cd_local_movto = x["cd_local_movto"],
                            no_local_movto = x["no_local_movto"],
                            cd_pessoa_local = x.ContainsKey("cd_pessoa_local") ? x["cd_pessoa_local"] : null,
                            cd_pessoa_empresa = x.ContainsKey("cd_pessoa_empresa") ? x["cd_pessoa_empresa"] : null,
                            nm_tipo_local = x.ContainsKey("nm_tipo_local") ? x["nm_tipo_local"] : null,
                            id_local_ativo = x.ContainsKey("id_local_ativo") ? x["id_local_ativo"] : null
                        }).ToList(),
                        total = localMovtoResult.total,
                        page,
                        limit,
                        pages = limit != null && limit > 0 ? (int)Math.Ceiling((double)localMovtoResult.total / limit.Value) : 0
                    };

                    return ResponseDefault(retorno);
                }

                return BadRequest(new
                {
                    success = false,
                    error = localMovtoResult.error ?? "Erro ao buscar locais de movimento"
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
        /// Busca um local de movimento específico por ID
        /// </summary>
        /// <param name="cd_local_movto">Código do local de movimento</param>
        /// <param name="cd_empresa">Código da empresa (obrigatório)</param>
        /// <returns>Local de movimento específico</returns>
        [Authorize]
        [HttpGet("{cd_local_movto}")]
        public async Task<IActionResult> GetById(int cd_local_movto, string cd_empresa)
        {
            // Validação do parâmetro obrigatório
            if (string.IsNullOrEmpty(cd_empresa))
            {
                return BadRequest(new
                {
                    success = false,
                    error = "Campo cd_empresa é obrigatório"
                });
            }

            try
            {
                // Buscar configuração do schema
                var schemaName = "T_Local_Movto";
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

                // Buscar local de movimento por ID
                var filtros = new List<(string campo, object valor)>
                {
                    ("cd_local_movto", cd_local_movto),
                    ("cd_pessoa_empresa", cd_empresa)
                };

                var localMovto = await SQLServerService.GetFirstByFields(source, "T_LOCAL_MOVTO", filtros);

                if (localMovto == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        error = "Local de movimento não encontrado"
                    });
                }

                var retorno = new
                {
                    data = new
                    {
                        cd_local_movto = localMovto["cd_local_movto"],
                        no_local_movto = localMovto["no_local_movto"],
                        cd_pessoa_local = localMovto.ContainsKey("cd_pessoa_local") ? localMovto["cd_pessoa_local"] : null,
                        cd_pessoa_empresa = localMovto.ContainsKey("cd_pessoa_empresa") ? localMovto["cd_pessoa_empresa"] : null,
                        nm_tipo_local = localMovto.ContainsKey("nm_tipo_local") ? localMovto["nm_tipo_local"] : null,
                        id_local_ativo = localMovto.ContainsKey("id_local_ativo") ? localMovto["id_local_ativo"] : null
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

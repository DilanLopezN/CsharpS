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
    public class PlanoContaController : BaseController
    {
        private readonly IRepository<SourceContext, Source> _sourceRepository;
        private readonly IRepository<MongoDbContext, Schema> _schemaRepository;

        public PlanoContaController(
            IMediatorHandler bus,
            INotificationHandler<DomainNotification> notifications,
            IRepository<SourceContext, Source> sourceRepository,
            IRepository<MongoDbContext, Schema> schemaRepository) : base(bus, notifications)
        {
            _sourceRepository = sourceRepository;
            _schemaRepository = schemaRepository;
        }

        /// <summary>
        /// Busca lista de planos de conta
        /// </summary>
        /// <param name="cd_empresa">Código da empresa (obrigatório)</param>
        /// <param name="value">Texto para busca (opcional)</param>
        /// <param name="mode">Modo de busca: Contains, Equals, StartsWith (default: Contains)</param>
        /// <param name="page">Número da página (default: 1)</param>
        /// <param name="limit">Limite de registros por página (default: 50)</param>
        /// <param name="sortField">Campo para ordenação (default: no_subgrupo_conta)</param>
        /// <param name="sortDesc">Ordenação decrescente (default: false)</param>
        /// <returns>Lista de planos de conta</returns>
        [Authorize]
        [HttpGet()]
        public async Task<IActionResult> GetAll(
            string cd_empresa,
            string value = null,
            SearchModeEnum mode = SearchModeEnum.Contains,
            int? page = 1,
            int? limit = 50,
            string sortField = "no_subgrupo_conta",
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
                var schemaName = "T_Plano_Conta";
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
                    searchFields = "no_subgrupo_conta";
                    searchValue = value;
                }

                // Buscar planos de conta usando a view vi_plano_conta
                var planoContaResult = await SQLServerService.GetList(
                    "vi_plano_conta",
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

                if (planoContaResult.success)
                {
                    var planosContas = planoContaResult.data;

                    var retorno = new
                    {
                        data = planosContas.Select(x => new
                        {
                            cd_plano_conta = x["cd_plano_conta"],
                            no_subgrupo_conta = x["no_subgrupo_conta"],
                            cd_subgrupo_conta = x.ContainsKey("cd_subgrupo_conta") ? x["cd_subgrupo_conta"] : null,
                            no_grupo_conta = x.ContainsKey("no_grupo_conta") ? x["no_grupo_conta"] : null,
                            cd_grupo_conta = x.ContainsKey("cd_grupo_conta") ? x["cd_grupo_conta"] : null
                        }).ToList(),
                        total = planoContaResult.total,
                        page,
                        limit,
                        pages = limit != null && limit > 0 ? (int)Math.Ceiling((double)planoContaResult.total / limit.Value) : 0
                    };

                    return ResponseDefault(retorno);
                }

                return BadRequest(new
                {
                    success = false,
                    error = planoContaResult.error ?? "Erro ao buscar planos de conta"
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
        /// Busca um plano de conta específico por ID
        /// </summary>
        /// <param name="cd_plano_conta">Código do plano de conta</param>
        /// <param name="cd_empresa">Código da empresa (obrigatório)</param>
        /// <returns>Plano de conta específico</returns>
        [Authorize]
        [HttpGet("{cd_plano_conta}")]
        public async Task<IActionResult> GetById(int cd_plano_conta, string cd_empresa)
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
                var schemaName = "T_Plano_Conta";
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

                // Buscar plano de conta por ID
                var filtros = new List<(string campo, object valor)>
                {
                    ("cd_plano_conta", cd_plano_conta),
                    ("cd_pessoa_empresa", cd_empresa)
                };

                var planoConta = await SQLServerService.GetFirstByFields(source, "vi_plano_conta", filtros);

                if (planoConta == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        error = "Plano de conta não encontrado"
                    });
                }

                var retorno = new
                {
                    data = new
                    {
                        cd_plano_conta = planoConta["cd_plano_conta"],
                        no_subgrupo_conta = planoConta["no_subgrupo_conta"],
                        cd_subgrupo_conta = planoConta.ContainsKey("cd_subgrupo_conta") ? planoConta["cd_subgrupo_conta"] : null,
                        no_grupo_conta = planoConta.ContainsKey("no_grupo_conta") ? planoConta["no_grupo_conta"] : null,
                        cd_grupo_conta = planoConta.ContainsKey("cd_grupo_conta") ? planoConta["cd_grupo_conta"] : null
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

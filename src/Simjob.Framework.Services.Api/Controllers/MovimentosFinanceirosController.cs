using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using MongoDB.Driver;
using Newtonsoft.Json;
using Simjob.Framework.Application.Controllers;
using Simjob.Framework.Domain.Core.Bus;
using Simjob.Framework.Domain.Core.Commands;
using Simjob.Framework.Domain.Core.Notifications;
using Simjob.Framework.Domain.Interfaces.Repositories;
using Simjob.Framework.Infra.Data.Context;
using Simjob.Framework.Infra.Identity.Contexts;
using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Infra.Schemas.Entities;
using Simjob.Framework.Services.Api.Enums;
using Simjob.Framework.Services.Api.Models;
using Simjob.Framework.Services.Api.Models.Contas;
using Simjob.Framework.Services.Api.Models.MovimentosFinanceiros;
using Simjob.Framework.Services.Api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using static MongoDB.Driver.WriteConcern;

namespace Simjob.Framework.Services.Api.Controllers
{
    public class MovimentosFinanceirosController : BaseController
    {
        private readonly IRepository<SourceContext, Source> _sourceRepository;
        private readonly IRepository<MongoDbContext, Schema> _schemaRepository;

        public MovimentosFinanceirosController(IMediatorHandler bus, INotificationHandler<DomainNotification> notifications, IRepository<SourceContext, Source> sourceRepository, IRepository<MongoDbContext, Schema> schemaRepository) : base(bus, notifications)
        {
            _sourceRepository = sourceRepository;
            _schemaRepository = schemaRepository;
        }

        [Authorize]
        [HttpGet()]
        public async Task<IActionResult> GetAll(string value, SearchModeEnum mode, int? page, int? limit, string sortField, bool sortDesc = false, string ids = "", string searchFields = null, string cd_empresa = null)
        {
            if (cd_empresa == null) return BadRequest("campo cd_empresa não informado");

            var schemaName = "T_Conta_Corrente";
            if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "").Replace("_", "");

            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
            var source = _sourceRepository.GetByField("description", schemaModel.Source);

            if (source != null && source.Active != null && source.Active == true)
            {
                try
                {
                    // Aplicar filtros na tabela T_CONTA_CORRENTE para filtrar por empresa
                    var filtrosEmpresa = new List<(string campo, object valor)> { ("cd_pessoa_empresa", cd_empresa) };

                    // Buscar todos os registros da empresa primeiro (como a procedure faz com @filtrostmp)
                    var contaCorrenteResult = await SQLServerService.GetList(
                        "T_CONTA_CORRENTE",
                        page,
                        limit,
                        sortField ?? schemaModel.PrimaryKey,
                        sortDesc,
                        ids,
                        searchFields,
                        value,
                        source,
                        mode,
                        "cd_pessoa_empresa",
                        cd_empresa
                    );

                    if (contaCorrenteResult.success && contaCorrenteResult.data != null)
                    {
                        var totalRecords = contaCorrenteResult.total;
                        var pageNumber = page ?? 1;
                        var pageSize = limit ?? 10;

                        // Ajustar pageNumber para começar em 1 (como na procedure)
                        pageNumber = pageNumber < 1 ? 1 : pageNumber;
                        pageSize = pageSize == 0 ? 10 : pageSize;

                        // Enriquecer dados com JOINs como na procedure
                        var enrichedData = new List<Dictionary<string, object>>();

                        foreach (var item in contaCorrenteResult.data)
                        {
                            var enrichedItem = new Dictionary<string, object>(item);

                            // INNER JOIN [vi_local_corrente] lo ON c.cd_local_origem = lo.cd_local_movto
                            if (item.ContainsKey("cd_local_origem") && item["cd_local_origem"] != null)
                            {
                                var localOrigem = await SQLServerService.GetFirstByFields(
                                    source,
                                    "vi_local_corrente",
                                    new List<(string campo, object valor)> { ("cd_local_movto", item["cd_local_origem"]) }
                                );
                                enrichedItem["no_local_origem"] = localOrigem?["no_local_movto"]?.ToString() ?? "";
                            }

                            // LEFT JOIN [vi_local_corrente] ld ON c.cd_local_destino = ld.cd_local_movto
                            if (item.ContainsKey("cd_local_destino") && item["cd_local_destino"] != null)
                            {
                                var localDestino = await SQLServerService.GetFirstByFields(
                                    source,
                                    "vi_local_corrente",
                                    new List<(string campo, object valor)> { ("cd_local_movto", item["cd_local_destino"]) }
                                );
                                enrichedItem["no_local_destino"] = localDestino?["no_local_movto"]?.ToString();
                            }
                            else
                            {
                                enrichedItem["no_local_destino"] = null;
                            }

                            // INNER JOIN [T_MOVIMENTACAO_FINANCEIRA] m ON c.cd_movimentacao_financeira = m.cd_movimentacao_financeira
                            if (item.ContainsKey("cd_movimentacao_financeira") && item["cd_movimentacao_financeira"] != null)
                            {
                                var movimentacao = await SQLServerService.GetFirstByFields(
                                    source,
                                    "T_MOVIMENTACAO_FINANCEIRA",
                                    new List<(string campo, object valor)> { ("cd_movimentacao_financeira", item["cd_movimentacao_financeira"]) }
                                );
                                enrichedItem["dc_movimentacao_financeira"] = movimentacao?["dc_movimentacao_financeira"]?.ToString() ?? "";
                            }

                            // INNER JOIN [T_TIPO_LIQUIDACAO] l ON c.cd_tipo_liquidacao = l.cd_tipo_liquidacao
                            if (item.ContainsKey("cd_tipo_liquidacao") && item["cd_tipo_liquidacao"] != null)
                            {
                                var tipoLiquidacao = await SQLServerService.GetFirstByFields(
                                    source,
                                    "T_TIPO_LIQUIDACAO",
                                    new List<(string campo, object valor)> { ("cd_tipo_liquidacao", item["cd_tipo_liquidacao"]) }
                                );
                                enrichedItem["dc_tipo_liquidacao"] = tipoLiquidacao?["dc_tipo_liquidacao"]?.ToString() ?? "";
                            }

                            // LEFT JOIN [vi_plano_conta] p ON c.cd_plano_conta = p.cd_plano_conta
                            if (item.ContainsKey("cd_plano_conta") && item["cd_plano_conta"] != null)
                            {
                                var planoConta = await SQLServerService.GetFirstByFields(
                                    source,
                                    "vi_plano_conta",
                                    new List<(string campo, object valor)> { ("cd_plano_conta", item["cd_plano_conta"]) }
                                );
                                enrichedItem["no_subgrupo_conta"] = planoConta?["no_subgrupo_conta"]?.ToString() ?? "";
                            }

                            enrichedData.Add(enrichedItem);
                        }

                        // Retorno no formato da procedure: totalRecords, pageNumber, pageSize, succeeded, data
                        var retorno = new
                        {
                            totalRecords = totalRecords,
                            pageNumber = pageNumber,
                            pageSize = pageSize,
                            succeeded = true,
                            data = enrichedData
                        };

                        return ResponseDefault(retorno);
                    }

                    return BadRequest(new
                    {
                        succeeded = false,
                        error = contaCorrenteResult.error ?? "Erro ao executar consulta"
                    });
                }
                catch (Exception ex)
                {
                    return BadRequest(new
                    {
                        succeeded = false,
                        error = $"Erro interno: {ex.Message}"
                    });
                }
            }

            return BadRequest(new
            {
                error = "Fonte de dados não configurada ou inativa."
            });
        }

        [Authorize]
        [HttpGet("{cd_conta_corrente}")]
        public async Task<IActionResult> GetById(int cd_conta_corrente, string cd_empresa = null)
        {
            var schemaName = "T_Conta_Corrente";
            if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "").Replace("_", "");

            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
            var source = _sourceRepository.GetByField("description", schemaModel.Source);

            if (source != null && source.Active != null && source.Active == true)
            {
                try
                {
                    // Filtros para buscar a conta corrente
                    var filtros = new List<(string campo, object valor)> { ("cd_conta_corrente", cd_conta_corrente) };

                    // Se a empresa for especificada, adicionar filtro por empresa
                    if (!string.IsNullOrEmpty(cd_empresa))
                    {
                        filtros.Add(("cd_pessoa_empresa", cd_empresa));
                    }

                    // Buscar a conta corrente pelo ID (e empresa se especificada)
                    var contaCorrente = await SQLServerService.GetFirstByFields(
                        source,
                        "T_CONTA_CORRENTE",
                        filtros
                    );

                    if (contaCorrente == null)
                    {
                        return NotFound(new
                        {
                            succeeded = false,
                            message = "Conta corrente não encontrada"
                        });
                    }

                    // Enriquecer os dados com informações das tabelas relacionadas
                    var enrichedData = new Dictionary<string, object>(contaCorrente);

                    // INNER JOIN [vi_local_corrente] lo ON c.cd_local_origem = lo.cd_local_movto
                    if (contaCorrente.ContainsKey("cd_local_origem") && contaCorrente["cd_local_origem"] != null)
                    {
                        var localOrigem = await SQLServerService.GetFirstByFields(
                            source,
                            "vi_local_corrente",
                            new List<(string campo, object valor)> { ("cd_local_movto", contaCorrente["cd_local_origem"]) }
                        );
                        enrichedData["no_local_origem"] = localOrigem?["no_local_movto"]?.ToString() ?? "";
                    }
                    else
                    {
                        enrichedData["no_local_origem"] = "";
                    }

                    // LEFT JOIN [vi_local_corrente] ld ON c.cd_local_destino = ld.cd_local_movto
                    if (contaCorrente.ContainsKey("cd_local_destino") && contaCorrente["cd_local_destino"] != null)
                    {
                        var localDestino = await SQLServerService.GetFirstByFields(
                            source,
                            "vi_local_corrente",
                            new List<(string campo, object valor)> { ("cd_local_movto", contaCorrente["cd_local_destino"]) }
                        );
                        enrichedData["no_local_destino"] = localDestino?["no_local_movto"]?.ToString();
                    }
                    else
                    {
                        enrichedData["no_local_destino"] = null;
                    }

                    // INNER JOIN [T_MOVIMENTACAO_FINANCEIRA] m ON c.cd_movimentacao_financeira = m.cd_movimentacao_financeira
                    if (contaCorrente.ContainsKey("cd_movimentacao_financeira") && contaCorrente["cd_movimentacao_financeira"] != null)
                    {
                        var movimentacao = await SQLServerService.GetFirstByFields(
                            source,
                            "T_MOVIMENTACAO_FINANCEIRA",
                            new List<(string campo, object valor)> { ("cd_movimentacao_financeira", contaCorrente["cd_movimentacao_financeira"]) }
                        );
                        enrichedData["dc_movimentacao_financeira"] = movimentacao?["dc_movimentacao_financeira"]?.ToString() ?? "";
                    }
                    else
                    {
                        enrichedData["dc_movimentacao_financeira"] = "";
                    }

                    // INNER JOIN [T_TIPO_LIQUIDACAO] l ON c.cd_tipo_liquidacao = l.cd_tipo_liquidacao
                    if (contaCorrente.ContainsKey("cd_tipo_liquidacao") && contaCorrente["cd_tipo_liquidacao"] != null)
                    {
                        var tipoLiquidacao = await SQLServerService.GetFirstByFields(
                            source,
                            "T_TIPO_LIQUIDACAO",
                            new List<(string campo, object valor)> { ("cd_tipo_liquidacao", contaCorrente["cd_tipo_liquidacao"]) }
                        );
                        enrichedData["dc_tipo_liquidacao"] = tipoLiquidacao?["dc_tipo_liquidacao"]?.ToString() ?? "";
                    }
                    else
                    {
                        enrichedData["dc_tipo_liquidacao"] = "";
                    }

                    // LEFT JOIN [vi_plano_conta] p ON c.cd_plano_conta = p.cd_plano_conta
                    if (contaCorrente.ContainsKey("cd_plano_conta") && contaCorrente["cd_plano_conta"] != null)
                    {
                        var planoConta = await SQLServerService.GetFirstByFields(
                            source,
                            "vi_plano_conta",
                            new List<(string campo, object valor)> { ("cd_plano_conta", contaCorrente["cd_plano_conta"]) }
                        );
                        enrichedData["no_subgrupo_conta"] = planoConta?["no_subgrupo_conta"]?.ToString() ?? "";
                    }
                    else
                    {
                        enrichedData["no_subgrupo_conta"] = "";
                    }

                    return ResponseDefault(new
                    {
                        succeeded = true,
                        data = enrichedData
                    });
                }
                catch (Exception ex)
                {
                    return BadRequest(new
                    {
                        succeeded = false,
                        error = $"Erro interno: {ex.Message}"
                    });
                }
            }

            return BadRequest(new
            {
                succeeded = false,
                error = "Fonte de dados não configurada ou inativa."
            });
        }


        [Authorize]
        [HttpPost()]
        public async Task<IActionResult> Insert([FromBody] InsertContaCorrenteModel command)
        {
            var schemaName = "T_Conta_Corrente";
            if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "").Replace("_", "");

            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
            var source = _sourceRepository.GetByField("description", schemaModel.Source);

            if (source == null || source.Active != true)
            {
                return BadRequest("Fonte de dados não configurada ou inativa.");
            }

            // Validar ModelState
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest($"Dados inválidos: {string.Join(", ", errors)}");
            }

            try
            {
                // Preparar dados para inserção (seguindo a lógica da procedure sp_add_conta_corrente)
                var insertData = new Dictionary<string, object>
                {
                    ["cd_local_origem"] = command.cd_local_origem,
                    ["cd_movimentacao_financeira"] = command.cd_movimentacao_financeira,
                    ["id_tipo_movimento"] = command.id_tipo_movimento,
                    ["cd_tipo_liquidacao"] = command.cd_tipo_liquidacao
                };

                // Campos opcionais
                if (command.cd_local_destino.HasValue)
                    insertData["cd_local_destino"] = command.cd_local_destino.Value;

                if (command.cd_baixa_titulo.HasValue)
                    insertData["cd_baixa_titulo"] = command.cd_baixa_titulo.Value;

                if (command.dta_conta_corrente.HasValue)
                    insertData["dta_conta_corrente"] = command.dta_conta_corrente.Value;

                if (command.cd_pessoa_empresa.HasValue)
                    insertData["cd_pessoa_empresa"] = command.cd_pessoa_empresa.Value;

                if (command.cd_plano_conta.HasValue)
                    insertData["cd_plano_conta"] = command.cd_plano_conta.Value;

                if (command.vl_conta_corrente.HasValue)
                    insertData["vl_conta_corrente"] = command.vl_conta_corrente.Value;

                if (!string.IsNullOrWhiteSpace(command.dc_obs_conta_corrente))
                    insertData["dc_obs_conta_corrente"] = command.dc_obs_conta_corrente.Trim();

                // Inserir no banco
                var insertResult = await SQLServerService.Insert("T_CONTA_CORRENTE", insertData, source);

                if (!insertResult.success)
                {
                    return BadRequest($"Erro ao inserir: {insertResult.error}");
                }

                // Buscar o registro inserido para retornar com o ID gerado
                var searchFilters = new List<(string campo, object valor)>
                {
                    ("cd_local_origem", command.cd_local_origem),
                    ("cd_movimentacao_financeira", command.cd_movimentacao_financeira),
                    ("id_tipo_movimento", command.id_tipo_movimento),
                    ("cd_tipo_liquidacao", command.cd_tipo_liquidacao)
                };

                if (command.vl_conta_corrente.HasValue)
                    searchFilters.Add(("vl_conta_corrente", command.vl_conta_corrente.Value));

                if (command.cd_pessoa_empresa.HasValue)
                    searchFilters.Add(("cd_pessoa_empresa", command.cd_pessoa_empresa.Value));

                var insertedRecord = await SQLServerService.GetFirstByFields(source, "T_CONTA_CORRENTE", searchFilters);

                if (insertedRecord == null)
                {
                    // Se não encontrou, tentar buscar pelos últimos registros
                    var recentRecords = await SQLServerService.GetList(
                        "T_CONTA_CORRENTE", 1, 5, "cd_conta_corrente", true, "", "", "",
                        source, SearchModeEnum.Contains, "", ""
                    );

                    if (recentRecords.success && recentRecords.data?.Count > 0)
                    {
                        // Procurar manualmente nos últimos registros
                        foreach (var record in recentRecords.data)
                        {
                            bool matches = true;

                            if (!record.ContainsKey("cd_local_origem") || record["cd_local_origem"]?.ToString() != command.cd_local_origem.ToString())
                                matches = false;
                            if (!record.ContainsKey("cd_movimentacao_financeira") || record["cd_movimentacao_financeira"]?.ToString() != command.cd_movimentacao_financeira.ToString())
                                matches = false;
                            if (!record.ContainsKey("id_tipo_movimento") || record["id_tipo_movimento"]?.ToString() != command.id_tipo_movimento.ToString())
                                matches = false;
                            if (!record.ContainsKey("cd_tipo_liquidacao") || record["cd_tipo_liquidacao"]?.ToString() != command.cd_tipo_liquidacao.ToString())
                                matches = false;

                            if (matches)
                            {
                                insertedRecord = record;
                                break;
                            }
                        }
                    }
                }

                //caso o tipo de movimento seja um caixa, deve relacionar com o caixa
                //Origem
                var localMovtoOrigem = await SQLServerService.GetFirstByFields(
                    source,
                    "T_LOCAL_MOVTO",
                    new List<(string campo, object valor)> { ("cd_local_movto", command.cd_local_origem), ("nm_tipo_local", 3) }
                );
                if( localMovtoOrigem != null)
                {
                    var caixaOrigem = await SQLServerService.GetFirstByFields(
                        source,
                        "T_CAIXA",
                        new List<(string campo, object valor)> { ("cd_local_movto", command.cd_local_origem), ("id_status_caixa", 0) }
                    );
                    //se nao tiver um caixa aberto, abrir
                    if(caixaOrigem == null)
                    {
                        var insertCaixaData = new Dictionary<string, object>
                        {
                            ["dc_caixa"] = $"{localMovtoOrigem["no_local_movto"]} ({DateTime.Now.ToString("dd/MM/yyyy")})",
                            ["cd_local_movto"] = localMovtoOrigem["cd_local_movto"],
                            ["dt_abertura"] = DateTime.Now,
                            ["id_status_caixa"] = 0,
                            ["cd_empresa"] = localMovtoOrigem["cd_pessoa_empresa"],
                            ["id_caixa_central"] = false
                        };

                        var insertCaixaResult = await SQLServerService.InsertWithResult("T_CAIXA", insertCaixaData, source);
                        caixaOrigem = insertCaixaResult.inserted;
                    }

                    var caixaTituloDict = new Dictionary<string, object>
                    {
                        { "cd_caixa", caixaOrigem["cd_caixa"] },
                        { "cd_conta_corrente", insertedRecord["cd_conta_corrente"] },
                        { "dt_recebimento", command.dta_conta_corrente ?? DateTime.Today }
                    };
                    var insertCaixaTitulo = await SQLServerService.Insert("T_CAIXA_TITULO", caixaTituloDict, source);
                    if (!insertCaixaTitulo.success) return BadRequest(insertCaixaTitulo.error);
                }
                //Destino
                if (command.cd_local_destino.HasValue)
                {
                    var localMovtoDestino = await SQLServerService.GetFirstByFields(
                        source,
                        "T_LOCAL_MOVTO",
                        new List<(string campo, object valor)> { ("cd_local_movto", command.cd_local_destino), ("nm_tipo_local", 3) }
                    );
                    if (localMovtoDestino != null)
                    {
                        var caixaDestino = await SQLServerService.GetFirstByFields(
                            source,
                            "T_CAIXA",
                            new List<(string campo, object valor)> { ("cd_local_movto", command.cd_local_destino), ("id_status_caixa", 0) }
                        );
                        //se nao tiver um caixa aberto, abrir
                        if (caixaDestino == null)
                        {
                            var insertCaixaData = new Dictionary<string, object>
                            {
                                ["dc_caixa"] = $"{localMovtoDestino["no_local_movto"]} ({DateTime.Now.ToString("dd/MM/yyyy")})",
                                ["cd_local_movto"] = localMovtoDestino["cd_local_movto"],
                                ["dt_abertura"] = DateTime.Now,
                                ["id_status_caixa"] = 0,
                                ["cd_empresa"] = localMovtoDestino["cd_pessoa_empresa"],
                                ["id_caixa_central"] = false
                            };

                            var insertCaixaResult = await SQLServerService.InsertWithResult("T_CAIXA", insertCaixaData, source);
                            caixaDestino = insertCaixaResult.inserted;
                        }

                        var caixaTituloDict = new Dictionary<string, object>
                        {
                        { "cd_caixa", caixaDestino["cd_caixa"] },
                        { "cd_conta_corrente", insertedRecord["cd_conta_corrente"] },
                        { "dt_recebimento", command.dta_conta_corrente ?? DateTime.Today }
                        };
                        var insertCaixaTitulo = await SQLServerService.Insert("T_CAIXA_TITULO", caixaTituloDict, source);
                        if (!insertCaixaTitulo.success) return BadRequest(insertCaixaTitulo.error);
                    }

                }


                if (command.cd_caixa != null)
                {
                    if (insertedRecord != null)
                    {
                        var caixaTituloDict = new Dictionary<string, object>
                        {
                            { "cd_caixa", command.cd_caixa },
                            { "cd_conta_corrente", insertedRecord["cd_conta_corrente"] },
                            { "dt_recebimento", command.dta_conta_corrente ?? DateTime.Today }
                        };
                        var insertCaixaTitulo = await SQLServerService.Insert("T_CAIXA_TITULO", caixaTituloDict, source);
                        if (!insertCaixaTitulo.success) return BadRequest(insertCaixaTitulo.error);
                    }
                }

                if (insertedRecord == null)
                {
                    return BadRequest("Registro inserido mas não foi possível recuperá-lo");
                }

                // Enriquecer dados com informações relacionadas (JOINs da procedure)
                var enrichedData = await EnrichContaCorrenteData(insertedRecord, source);

                return ResponseDefault(new
                {
                    success = true,
                    message = "Registro inserido com sucesso",
                    data = enrichedData
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro interno: {ex.Message}");
            }
        }

        
        [Authorize]
        [HttpPut("{cd_conta_corrente}")]
        public async Task<IActionResult> Update(int cd_conta_corrente, [FromBody] InsertContaCorrenteModel command)
        {
            var schemaName = "T_Conta_Corrente";
            if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "").Replace("_", "");
            
            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
            var source = _sourceRepository.GetByField("description", schemaModel.Source);
            
            if (source == null || source.Active != true)
            {
                return BadRequest("Fonte de dados não configurada ou inativa.");
            }

            // Validar ModelState
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest($"Dados inválidos: {string.Join(", ", errors)}");
            }

            try
            {
                // Verificar se o registro existe
                var existingRecord = await SQLServerService.GetFirstByFields(
                    source, "T_CONTA_CORRENTE", 
                    new List<(string, object)> { ("cd_conta_corrente", cd_conta_corrente) }
                );

                if (existingRecord == null)
                {
                    return NotFound("Registro não encontrado");
                }

                // Preparar dados para atualização (seguindo a lógica da procedure sp_edit_conta_corrente)
                var updateData = new Dictionary<string, object>();

                // Campos obrigatórios
                updateData["cd_local_origem"] = command.cd_local_origem;
                updateData["cd_movimentacao_financeira"] = command.cd_movimentacao_financeira;
                updateData["id_tipo_movimento"] = command.id_tipo_movimento;
                updateData["cd_tipo_liquidacao"] = command.cd_tipo_liquidacao;

                // Campos opcionais
                if (command.cd_local_destino.HasValue)
                    updateData["cd_local_destino"] = command.cd_local_destino.Value;
                
                if (command.cd_baixa_titulo.HasValue)
                    updateData["cd_baixa_titulo"] = command.cd_baixa_titulo.Value;
                
                if (command.dta_conta_corrente.HasValue)
                    updateData["dta_conta_corrente"] = command.dta_conta_corrente.Value;
                
                if (command.cd_pessoa_empresa.HasValue)
                    updateData["cd_pessoa_empresa"] = command.cd_pessoa_empresa.Value;
                
                if (command.cd_plano_conta.HasValue)
                    updateData["cd_plano_conta"] = command.cd_plano_conta.Value;
                
                if (command.vl_conta_corrente.HasValue)
                    updateData["vl_conta_corrente"] = command.vl_conta_corrente.Value;
                
                if (!string.IsNullOrWhiteSpace(command.dc_obs_conta_corrente))
                    updateData["dc_obs_conta_corrente"] = command.dc_obs_conta_corrente.Trim();

                // Atualizar no banco
                var updateResult = await SQLServerService.Update(
                    "T_CONTA_CORRENTE", 
                    updateData, 
                    source,
                    "cd_conta_corrente", 
                    cd_conta_corrente
                );

                if (!updateResult.success)
                {
                    return BadRequest($"Erro ao atualizar: {updateResult.error}");
                }

                // Buscar o registro atualizado
                var updatedRecord = await SQLServerService.GetFirstByFields(
                    source, "T_CONTA_CORRENTE", 
                    new List<(string, object)> { ("cd_conta_corrente", cd_conta_corrente) }
                );

                if (updatedRecord == null)
                {
                    return BadRequest("Registro atualizado mas não foi possível recuperá-lo");
                }

                // Enriquecer dados com informações relacionadas (JOINs da procedure)
                var enrichedData = await EnrichContaCorrenteData(updatedRecord, source);

                return ResponseDefault(new
                {
                    success = true,
                    message = "Registro alterado com sucesso",
                    data = enrichedData
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro interno: {ex.Message}");
            }
        }

        [Authorize]
        [HttpDelete("{cd_conta_corrente}")]
        public async Task<IActionResult> Delete(int cd_conta_corrente)
        {
            var schemaName = "T_Conta_Corrente";
            if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "").Replace("_", "");
            
            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
            var source = _sourceRepository.GetByField("description", schemaModel.Source);
            
            if (source == null || source.Active != true)
            {
                return BadRequest("Fonte de dados não configurada ou inativa.");
            }

            try
            {
                // Verificar se o registro existe
                var existingRecord = await SQLServerService.GetFirstByFields(
                    source, "T_CONTA_CORRENTE", 
                    new List<(string, object)> { ("cd_conta_corrente", cd_conta_corrente) }
                );

                if (existingRecord == null)
                {
                    return NotFound("Registro não encontrado");
                }

                // Deletar registro
                var deleteResult = await SQLServerService.Delete(
                    "T_CONTA_CORRENTE", 
                    "cd_conta_corrente", 
                    cd_conta_corrente.ToString(),
                    source
                );

                if (!deleteResult.success)
                {
                    return BadRequest($"Erro ao deletar: {deleteResult.error}");
                }

                return ResponseDefault(new
                {
                    success = true,
                    message = "Registro excluído com sucesso"
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro interno: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPost("estorno")]
        public async Task<IActionResult> Estorno([FromBody] EstornoContaCorrenteModel command)
        {
            var schemaName = "T_Conta_Corrente";
            if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "").Replace("_", "");

            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
            var source = _sourceRepository.GetByField("description", schemaModel.Source);

            if (source == null || source.Active != true)
            {
                return BadRequest("Fonte de dados não configurada ou inativa.");
            }

            // Validar ModelState
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest($"Dados inválidos: {string.Join(", ", errors)}");
            }

            try
            {
                // Buscar o registro original pelo cd_conta_corrente
                var registroOriginal = await SQLServerService.GetFirstByFields(
                    source,
                    "T_CONTA_CORRENTE",
                    new List<(string campo, object valor)> { ("cd_conta_corrente", command.cd_conta_corrente) }
                );

                if (registroOriginal == null)
                {
                    return NotFound("Conta corrente não encontrada");
                }

                // Preparar dados para o estorno baseado no registro original
                var estornoData = new Dictionary<string, object>(registroOriginal);
                
                // Remover o campo cd_conta_corrente para permitir que seja gerado automaticamente
                if (estornoData.ContainsKey("cd_conta_corrente"))
                    estornoData.Remove("cd_conta_corrente");
                if (estornoData.ContainsKey("cd_baixa_titulo"))
                    estornoData.Remove("cd_baixa_titulo");
                
                // Alterar apenas os campos específicos do estorno
                estornoData["dta_conta_corrente"] = DateTime.Today;
                estornoData["dc_obs_conta_corrente"] = $"Estorno - {registroOriginal["dc_obs_conta_corrente"]?.ToString() ?? ""}";
                estornoData["cd_conta_corrente_estorno"] = registroOriginal["cd_conta_corrente"];
                
                // Valor deve ser negativo (inverter sinal)
                if (estornoData.ContainsKey("vl_conta_corrente") && estornoData["vl_conta_corrente"] != null)
                {
                    if (decimal.TryParse(estornoData["vl_conta_corrente"].ToString(), out decimal valor))
                    {
                        estornoData["vl_conta_corrente"] = -valor;
                    }
                }

                // Inserir o registro de estorno
                var insertResult = await SQLServerService.Insert("T_CONTA_CORRENTE", estornoData, source);

                if (!insertResult.success)
                {
                    return BadRequest($"Erro ao inserir estorno: {insertResult.error}");
                }

                var inserted = await SQLServerService.GetFirstByFields(
                    source,
                    "T_CONTA_CORRENTE",
                    new List<(string campo, object valor)> { ("cd_conta_corrente_estorno", estornoData["cd_conta_corrente_estorno"]) }
                );

                // Buscar o registro de estorno inserido para retornar
                var cdContaCorrenteEstorno = inserted["cd_conta_corrente"];
                var registroEstorno = await SQLServerService.GetFirstByFields(
                    source,
                    "T_CONTA_CORRENTE",
                    new List<(string campo, object valor)> { ("cd_conta_corrente", cdContaCorrenteEstorno) }
                );

                if (registroEstorno == null)
                {
                    return BadRequest("Estorno inserido mas não foi possível recuperá-lo");
                }

                // Verificar se a conta corrente original pertence a um caixa
                var caixaTituloOriginal = await SQLServerService.GetFirstByFields(
                    source,
                    "T_CAIXA_TITULO",
                    new List<(string campo, object valor)> { ("cd_conta_corrente", registroOriginal["cd_conta_corrente"]) }
                );

                if (caixaTituloOriginal != null)
                {
                    // Criar registro na T_CAIXA_TITULO para o estorno
                    var caixaTituloEstorno = new Dictionary<string, object>
                    {
                        { "cd_caixa", caixaTituloOriginal["cd_caixa"] },
                        { "cd_titulo", caixaTituloOriginal["cd_titulo"] },
                        { "cd_conta_corrente", cdContaCorrenteEstorno },
                        { "dt_recebimento", DateTime.Today }
                    };

                    var insertCaixaTituloResult = await SQLServerService.Insert("T_CAIXA_TITULO", caixaTituloEstorno, source);
                    
                    if (!insertCaixaTituloResult.success)
                    {
                        // Log do erro mas não falha o estorno
                        return ResponseDefault(new
                        {
                            success = true,
                            message = "Estorno realizado com sucesso, mas houve erro ao criar registro no caixa",
                            warning = $"Erro ao inserir T_CAIXA_TITULO: {insertCaixaTituloResult.error}",
                            data = await EnrichContaCorrenteData(registroEstorno, source),
                            cd_conta_corrente_estornada = registroOriginal["cd_conta_corrente"],
                            cd_estorno = cdContaCorrenteEstorno
                        });
                    }
                }

                // Se o registro original possui uma baixa, marcar como estornada
                if (registroOriginal.ContainsKey("cd_baixa_titulo") && registroOriginal["cd_baixa_titulo"] != null)
                {
                    var cdBaixaTitulo = registroOriginal["cd_baixa_titulo"];
                    
                    // Buscar o cd_titulo relacionado à baixa
                    var baixaTitulo = await SQLServerService.GetFirstByFields(
                        source,
                        "T_BAIXA_TITULO",
                        new List<(string campo, object valor)> { ("cd_baixa_titulo", cdBaixaTitulo) }
                    );
                    
                    // Atualizar a baixa como estornada
                    var updateBaixaResult = await SQLServerService.Update(
                        "T_BAIXA_TITULO",
                        new Dictionary<string, object> { { "id_baixa_estornada", 1 } },
                        source,
                        "cd_baixa_titulo",
                        cdBaixaTitulo
                    );

                    if (!updateBaixaResult.success)
                    {
                        // Log do erro mas não falha o estorno, apenas avisa
                        // O estorno já foi inserido com sucesso
                        return ResponseDefault(new
                        {
                            success = true,
                            message = "Estorno realizado com sucesso, mas houve erro ao marcar baixa como estornada",
                            warning = $"Erro ao atualizar baixa: {updateBaixaResult.error}",
                            data = await EnrichContaCorrenteData(registroEstorno, source),
                            cd_conta_corrente_estornada = registroOriginal["cd_conta_corrente"],
                            cd_estorno = cdContaCorrenteEstorno
                        });
                    }
                    
                    // Atualizar o título relacionado à baixa (id_status_titulo = 1) e recalcular saldos
                    if (baixaTitulo != null && baixaTitulo.ContainsKey("cd_titulo") && baixaTitulo["cd_titulo"] != null)
                    {
                        var cdTitulo = baixaTitulo["cd_titulo"];
                        
                        // Lógica inversa da baixa: recalcular saldos do título após estorno
                        string connectionString = $"Server={source.Host};Database={source.DbName};User Id={source.User};Password={source.Password};MultipleActiveResultSets=True;";
                        
                        try
                        {
                            using (var connection = new SqlConnection(connectionString))
                            {
                                await connection.OpenAsync();
                                
                                // Recalcular os valores do título baseado nas baixas restantes (não estornadas)
                                var updateTitulo = new SqlCommand(@"
                                UPDATE t SET
                                    t.vl_saldo_titulo = t.vl_titulo - ISNULL((SELECT SUM(vl_baixa_saldo_titulo) FROM T_BAIXA_TITULO b WHERE b.cd_titulo = t.cd_titulo AND ISNULL(b.id_baixa_estornada, 0) = 0),0),
                                    t.vl_desconto_titulo = ISNULL((SELECT SUM(vl_desconto_baixa) FROM T_BAIXA_TITULO b WHERE b.cd_titulo = t.cd_titulo AND ISNULL(b.id_baixa_estornada, 0) = 0),0),
                                    t.vl_juros_liquidado = ISNULL((SELECT SUM(vl_juros_baixa) FROM T_BAIXA_TITULO b WHERE b.cd_titulo = t.cd_titulo AND ISNULL(b.id_baixa_estornada, 0) = 0),0),
                                    t.vl_multa_liquidada = ISNULL((SELECT SUM(vl_multa_baixa) FROM T_BAIXA_TITULO b WHERE b.cd_titulo = t.cd_titulo AND ISNULL(b.id_baixa_estornada, 0) = 0),0),
                                    t.vl_desconto_multa = ISNULL((SELECT SUM(vl_desc_multa_baixa) FROM T_BAIXA_TITULO b WHERE b.cd_titulo = t.cd_titulo AND ISNULL(b.id_baixa_estornada, 0) = 0),0),
                                    t.vl_desconto_juros = ISNULL((SELECT SUM(vl_desc_juros_baixa) FROM T_BAIXA_TITULO b WHERE b.cd_titulo = t.cd_titulo AND ISNULL(b.id_baixa_estornada, 0) = 0),0),
                                    t.vl_liquidacao_titulo = ISNULL((SELECT SUM(vl_baixa_saldo_titulo) FROM T_BAIXA_TITULO b WHERE b.cd_titulo = t.cd_titulo AND ISNULL(b.id_baixa_estornada, 0) = 0),0),
                                    t.dt_liquidacao_titulo = CASE 
                                        WHEN (t.vl_titulo - ISNULL((SELECT SUM(vl_baixa_saldo_titulo) FROM T_BAIXA_TITULO b WHERE b.cd_titulo = t.cd_titulo AND ISNULL(b.id_baixa_estornada, 0) = 0),0)) = 0 
                                        THEN (SELECT MAX(dt_baixa_titulo) FROM T_BAIXA_TITULO b WHERE b.cd_titulo = t.cd_titulo AND ISNULL(b.id_baixa_estornada, 0) = 0)
                                        ELSE NULL
                                    END
                                FROM T_TITULO t
                                WHERE t.cd_titulo = @cd_titulo", connection);
                                
                                updateTitulo.Parameters.AddWithValue("@cd_titulo", cdTitulo);
                                await updateTitulo.ExecuteNonQueryAsync();
                                
                                // Atualizar status do título baseado no novo saldo
                                var updateStatus = new SqlCommand(@"
                                UPDATE t SET
                                    t.id_status_titulo = CASE
                                        WHEN t.vl_saldo_titulo = 0 THEN 2  -- Liquidado
                                        ELSE 1  -- Em aberto
                                    END
                                FROM T_TITULO t
                                WHERE t.cd_titulo = @cd_titulo", connection);
                                
                                updateStatus.Parameters.AddWithValue("@cd_titulo", cdTitulo);
                                await updateStatus.ExecuteNonQueryAsync();
                            }
                        }
                        catch (Exception ex)
                        {
                            // Log do erro mas não falha o estorno
                            return ResponseDefault(new
                            {
                                success = true,
                                message = "Estorno realizado com sucesso, baixa marcada como estornada, mas houve erro ao recalcular saldos do título",
                                warning = $"Erro ao recalcular título: {ex.Message}",
                                data = await EnrichContaCorrenteData(registroEstorno, source),
                                cd_conta_corrente_estornada = registroOriginal["cd_conta_corrente"],
                                cd_estorno = cdContaCorrenteEstorno
                            });
                        }
                    }
                }

                // Enriquecer dados com informações relacionadas (JOINs)
                var enrichedData = await EnrichContaCorrenteData(registroEstorno, source);

                return ResponseDefault(new
                {
                    success = true,
                    message = "Estorno realizado com sucesso",
                    data = enrichedData,
                    cd_conta_corrente_estornada = registroOriginal["cd_conta_corrente"],
                    cd_estorno = cdContaCorrenteEstorno
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro interno: {ex.Message}");
            }
        }

        private async Task<Dictionary<string, object>> EnrichContaCorrenteData(Dictionary<string, object> record, Source source)
        {
            var enrichedData = new Dictionary<string, object>(record);

            // INNER JOIN vi_local_corrente (origem)
            if (record.ContainsKey("cd_local_origem") && record["cd_local_origem"] != null)
            {
                var localOrigem = await SQLServerService.GetFirstByFields(
                    source, "vi_local_corrente",
                    new List<(string, object)> { ("cd_local_movto", record["cd_local_origem"]) });
                enrichedData["no_local_origem"] = localOrigem?["no_local_movto"]?.ToString() ?? "";
            }

            // LEFT JOIN vi_local_corrente (destino)
            if (record.ContainsKey("cd_local_destino") && record["cd_local_destino"] != null)
            {
                var localDestino = await SQLServerService.GetFirstByFields(
                    source, "vi_local_corrente",
                    new List<(string, object)> { ("cd_local_movto", record["cd_local_destino"]) });
                enrichedData["no_local_destino"] = localDestino?["no_local_movto"]?.ToString();
            }

            // INNER JOIN T_MOVIMENTACAO_FINANCEIRA
            if (record.ContainsKey("cd_movimentacao_financeira") && record["cd_movimentacao_financeira"] != null)
            {
                var movimentacao = await SQLServerService.GetFirstByFields(
                    source, "T_MOVIMENTACAO_FINANCEIRA",
                    new List<(string, object)> { ("cd_movimentacao_financeira", record["cd_movimentacao_financeira"]) });
                enrichedData["dc_movimentacao_financeira"] = movimentacao?["dc_movimentacao_financeira"]?.ToString() ?? "";
            }

            // INNER JOIN T_TIPO_LIQUIDACAO
            if (record.ContainsKey("cd_tipo_liquidacao") && record["cd_tipo_liquidacao"] != null)
            {
                var tipoLiquidacao = await SQLServerService.GetFirstByFields(
                    source, "T_TIPO_LIQUIDACAO",
                    new List<(string, object)> { ("cd_tipo_liquidacao", record["cd_tipo_liquidacao"]) });
                enrichedData["dc_tipo_liquidacao"] = tipoLiquidacao?["dc_tipo_liquidacao"]?.ToString() ?? "";
            }

            // LEFT JOIN vi_plano_conta
            if (record.ContainsKey("cd_plano_conta") && record["cd_plano_conta"] != null)
            {
                var planoConta = await SQLServerService.GetFirstByFields(
                    source, "vi_plano_conta",
                    new List<(string, object)> { ("cd_plano_conta", record["cd_plano_conta"]) });
                enrichedData["no_subgrupo_conta"] = planoConta?["no_subgrupo_conta"]?.ToString() ?? "";
            }

            return enrichedData;
        }
    }
    
}

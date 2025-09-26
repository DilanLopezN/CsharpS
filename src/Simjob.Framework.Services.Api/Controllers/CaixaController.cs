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
using Simjob.Framework.Services.Api.Models.Caixa;
using Simjob.Framework.Services.Api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Simjob.Framework.Services.Api.Controllers
{
    public class CaixaController : BaseController
    {
        private readonly IRepository<SourceContext, Source> _sourceRepository;
        private readonly IRepository<MongoDbContext, Schema> _schemaRepository;

        public CaixaController(IMediatorHandler bus, INotificationHandler<DomainNotification> notifications, IRepository<SourceContext, Source> sourceRepository, IRepository<MongoDbContext, Schema> schemaRepository) : base(bus, notifications)
        {
            _sourceRepository = sourceRepository;
            _schemaRepository = schemaRepository;
        }

        [Authorize]
        [HttpPost("aberturaCaixa")]
        public async Task<IActionResult> AberturaCaixa([FromBody] InsertCaixaModel command)
        {
            var fonteDados = ValidarFonteDados();
            if (!fonteDados.success)
            {
                return BadRequest(fonteDados.error);
            }

            // Validar ModelState
            var validacaoModel = ValidarModelState();
            if (!validacaoModel.success)
            {
                return BadRequest(validacaoModel.error);
            }

            try
            {
                var empresa = await ValidarEmpresa(command.cd_empresa, fonteDados.source);
                if (empresa == null)
                {
                    return BadRequest("Empresa não encontrada");
                }

                if (command.id_caixa_central == true)
                {
                    var caixaCentral = await BuscarCaixaCentralDaEmpresa(command.cd_empresa, fonteDados.source);
                    if(caixaCentral != null)
                    {
                        return BadRequest("Já existe um caixa central para esta empresa");
                    }
                }


                if (command.cd_pessoa_list.Any())
                {
                    var validacaoFuncionarios = await ValidarFuncionariosParaAbertura(command.cd_pessoa_list, command.cd_empresa, fonteDados.source);
                    if (!validacaoFuncionarios.success)
                    {
                        return BadRequest(validacaoFuncionarios.error);
                    }
                }
                else
                {
                    if (command.id_caixa_central != true)
                    {
                        return BadRequest("É necessário pelo menos uma pessoa responsável pelo caixa");
                    }
                }
                
                var localMovto = await ValidarLocalMovto(command.cd_local_movto, fonteDados.source);
                if (localMovto == null)
                {
                    return BadRequest("Local de Movimento não encontrado");
                }
                if (command.dt_abertura == null)
                {
                    command.dt_abertura = DateTime.Now;
                }
                command.dc_caixa = $"{localMovto["no_local_movto"]}";
                if(command.id_caixa_central != true)
                {
                    command.dc_caixa = $"{localMovto["no_local_movto"]} ({command.dt_abertura?.ToString("dd/MM/yyyy")})";
                }

                var cdCaixa = await CriarCaixa(command, fonteDados.source);
                if (cdCaixa == null)
                {
                    return BadRequest("Erro ao criar caixa");
                }

                if (command.cd_pessoa_list.Any())
                {
                    var resultadoFuncionarios = await VincularFuncionariosCaixa(cdCaixa.Value, command.cd_pessoa_list, fonteDados.source);
                    if (!resultadoFuncionarios.success)
                    {
                        return BadRequest(resultadoFuncionarios.error);
                    }
                }

                var caixaCriado = await BuscarCaixaPorId(cdCaixa.Value, fonteDados.source);
                var funcionarios = await BuscarFuncionariosCaixa(cdCaixa.Value, fonteDados.source);

                var responseData = new Dictionary<string, object>(caixaCriado);
                responseData["funcionarios"] = funcionarios?.Select(f => new
                {
                    cd_pessoa = f["cd_pessoa"],
                    cd_caixa = f["cd_caixa"],
                    no_pessoa = f["no_pessoa"]
                }).ToList();

                return ResponseDefault(new
                {
                    success = true,
                    message = "Caixa aberto com sucesso",
                    data = responseData
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro interno: {ex.Message}");
            }
        }

        [Authorize]
        [HttpGet("funcionario/{cd_pessoa}/caixaAberto")]
        public async Task<IActionResult> GetCaixaAbertoByPessoa(int cd_pessoa, int cd_empresa)
        {
            var fonteDados = ValidarFonteDados();
            if (!fonteDados.success)
            {
                return BadRequest(fonteDados.error);
            }

            try
            {
                // Validar se o funcionário existe
                var pessoa = await SQLServerService.GetFirstByFields(
                    fonteDados.source, "T_PESSOA", 
                    new List<(string, object)> { ("cd_pessoa", cd_pessoa) }
                );
                if (pessoa == null)
                {
                    return BadRequest("Funcionário não encontrado");
                }

                // Buscar diretamente o caixa ativo (aberto ou aguardando validação) do funcionário
                var query = @"
                    SELECT c.* FROM T_CAIXA c
                    INNER JOIN T_CAIXA_PESSOA cp ON cp.cd_caixa = c.cd_caixa
                    WHERE cp.cd_pessoa = @cd_pessoa 
                    AND c.cd_empresa = @cd_empresa
                    AND (c.id_status_caixa = @status_aberto)";

                Dictionary<string, object> caixaAtivo = null;
                var connectionString = $"Server={fonteDados.source.Host};Database={fonteDados.source.DbName};User Id={fonteDados.source.User};Password={fonteDados.source.Password};";
                
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@cd_pessoa", cd_pessoa);
                        command.Parameters.AddWithValue("@cd_empresa", cd_empresa);
                        command.Parameters.AddWithValue("@status_aberto", (int)StatusCaixa.EmAberto);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                caixaAtivo = new Dictionary<string, object>();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    caixaAtivo[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                                }
                            }
                        }
                    }
                }

                if (caixaAtivo == null)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Funcionário não possui caixa ativo",
                        data = (object)null
                    });
                }

                // Buscar funcionários do caixa
                var cdCaixa = Convert.ToInt32(caixaAtivo["cd_caixa"]);
                var funcionarios = await BuscarFuncionariosCaixa(cdCaixa, fonteDados.source);

                // Buscar informações financeiras do caixa
                var informacoesFinanceiras = await BuscarInformacoesFinanceirasCaixa(cdCaixa, fonteDados.source);

                // Buscar títulos do caixa
                var titulos = await BuscarTitulosCaixa(cdCaixa, fonteDados.source);

                // Montar resposta enriquecida
                var statusCaixa = (StatusCaixa)Convert.ToInt32(caixaAtivo["id_status_caixa"]);
                var responseData = new Dictionary<string, object>
                {
                    ["cd_caixa"] = caixaAtivo["cd_caixa"],
                    ["dc_caixa"] = caixaAtivo["dc_caixa"],
                    ["dt_abertura"] = caixaAtivo["dt_abertura"],
                    ["dt_fechamento"] = caixaAtivo.ContainsKey("dt_fechamento") ? caixaAtivo["dt_fechamento"] : null,
                    ["id_status_caixa"] = caixaAtivo["id_status_caixa"],
                    ["status"] = statusCaixa.GetDescription(),
                    ["cd_empresa"] = caixaAtivo["cd_empresa"],
                    ["id_caixa_central"] = (caixaAtivo.ContainsKey("id_caixa_central") && caixaAtivo["id_caixa_central"] != null) ? caixaAtivo["id_caixa_central"] : false,
                    ["funcionarios"] = funcionarios?.Select(f => new
                    {
                        cd_pessoa = f["cd_pessoa"],
                        cd_caixa = f["cd_caixa"],
                        no_pessoa = f["no_pessoa"]
                    }).ToList(),
                    ["financeiro"] = new
                    {
                        total_entradas = informacoesFinanceiras.totalEntradas,
                        total_saidas = informacoesFinanceiras.totalSaidas,
                        saldo_atual = informacoesFinanceiras.saldoAtual
                    },
                    ["titulos"] = titulos
                };

                return ResponseDefault(new
                {
                    success = true,
                    message = "Caixa ativo encontrado",
                    data = responseData
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro interno: {ex.Message}");
            }
        }

        [Authorize]
        [HttpGet("empresa/{cd_empresa}/caixas")]
        public async Task<IActionResult> GetCaixasByEmpresa(int cd_empresa, string dc_caixa = null, string value = null, SearchModeEnum mode = SearchModeEnum.Contains, int? page = 1, int? limit = 10, string sortField = "dt_abertura", bool sortDesc = true, string ids = "", string searchFields = null)
        {
            var fonteDados = ValidarFonteDados();
            if (!fonteDados.success)
            {
                return BadRequest(fonteDados.error);
            }

            try
            {
                // Validar se a empresa existe
                var empresa = await SQLServerService.GetFirstByFields(
                    fonteDados.source, "T_EMPRESA", 
                    new List<(string, object)> { ("cd_pessoa_empresa", cd_empresa) }
                );
                if (empresa == null)
                {
                    return BadRequest("Empresa não encontrada");
                }

                // Usar SQLServerService.GetList para buscar caixas com filtros
                if (string.IsNullOrEmpty(sortField)) sortField = "dt_abertura";
                
                // Adicionar filtro por dc_caixa se fornecido
                string finalSearchFields = searchFields;
                string finalValue = value;
                
                if (!string.IsNullOrWhiteSpace(dc_caixa))
                {
                    if (string.IsNullOrWhiteSpace(finalSearchFields))
                    {
                        finalSearchFields = "[dc_caixa]";
                        finalValue = $"[{dc_caixa}]";
                    }
                    else
                    {
                        finalSearchFields += ",[dc_caixa]";
                        finalValue += $",{dc_caixa}";
                    }
                }
                
                var caixasResult = await SQLServerService.GetList(
                    "T_CAIXA", 
                    page, 
                    limit, 
                    sortField, 
                    sortDesc, 
                    ids, 
                    finalSearchFields, 
                    finalValue, 
                    fonteDados.source, 
                    mode, 
                    "cd_empresa", 
                    cd_empresa.ToString()
                );

                if (!caixasResult.success)
                {
                    return BadRequest(new
                    {
                        success = false,
                        error = caixasResult.error
                    });
                }

                var caixas = caixasResult.data;
                var caixasProcessados = new List<Dictionary<string, object>>();
                var contadores = new { abertos = 0, aguardandoValidacao = 0, fechados = 0 };

                foreach (var caixa in caixas)
                {
                    var statusCaixa = (StatusCaixa)Convert.ToInt32(caixa["id_status_caixa"]);
                    var cdCaixa = Convert.ToInt32(caixa["cd_caixa"]);
                    
                    // Contar por status
                    switch (statusCaixa)
                    {
                        case StatusCaixa.EmAberto:
                            contadores = new { abertos = contadores.abertos + 1, contadores.aguardandoValidacao, contadores.fechados };
                            break;
                        case StatusCaixa.AguardandoValidacao:
                            contadores = new { contadores.abertos, aguardandoValidacao = contadores.aguardandoValidacao + 1, contadores.fechados };
                            break;
                        case StatusCaixa.Fechado:
                            contadores = new { contadores.abertos, contadores.aguardandoValidacao, fechados = contadores.fechados + 1 };
                            break;
                    }

                    // Buscar informações financeiras do caixa
                    var informacoesFinanceiras = await BuscarInformacoesFinanceirasCaixa(cdCaixa, fonteDados.source);

                    // Montar dados do caixa com informações financeiras
                    var caixaCompleto = new Dictionary<string, object>
                    {
                        ["cd_caixa"] = caixa["cd_caixa"],
                        ["dc_caixa"] = caixa["dc_caixa"],
                        ["dt_abertura"] = caixa["dt_abertura"],
                        ["dt_fechamento"] = caixa.ContainsKey("dt_fechamento") ? caixa["dt_fechamento"] : null,
                        ["id_status_caixa"] = caixa["id_status_caixa"],
                        ["status"] = statusCaixa.GetDescription(),
                        ["cd_empresa"] = caixa["cd_empresa"],
                        ["id_caixa_central"] = (caixa.ContainsKey("id_caixa_central") && caixa["id_caixa_central"] != null) ? caixa["id_caixa_central"] : false,
                        ["financeiro"] = new
                        {
                            total_entradas = informacoesFinanceiras.totalEntradas,
                            total_saidas = informacoesFinanceiras.totalSaidas,
                            saldo_atual = informacoesFinanceiras.saldoAtual
                        }
                    };

                    caixasProcessados.Add(caixaCompleto);
                }

                return ResponseDefault(new
                {
                    data = new
                    {
                        total = caixasProcessados.Count,
                        resumo = contadores,
                        caixas = caixasProcessados
                    },
                    total = caixasResult.total,
                    page,
                    limit,
                    pages = limit != null ? (int)Math.Ceiling((double)caixasResult.total / limit.Value) : 0
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro interno: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPut("solicitarFechamento/{cd_caixa}")]
        public async Task<IActionResult> SolicitarFechamento(int cd_caixa, [FromBody] SolicitarFechamentoRequest request)
        {
            var fonteDados = ValidarFonteDados();
            if (!fonteDados.success)
            {
                return BadRequest(fonteDados.error);
            }

            // Validar ModelState
            var validacaoModel = ValidarModelState();
            if (!validacaoModel.success)
            {
                return BadRequest(validacaoModel.error);
            }

            // Obter cd_pessoa do token JWT
            var cdPessoaValidacao = GetCdPessoaFromToken();
            if (cdPessoaValidacao == null)
            {
                return BadRequest("Não foi possível identificar a pessoa que está validando o fechamento");
            }


            try
            {
                // Validar se o caixa existe e está em status ativo (EmAberto, Rejeitado ou Reaberto)
                var caixa = await ValidarCaixaComStatusAtivo(cd_caixa, fonteDados.source);
                if (caixa == null)
                {
                    return BadRequest("Caixa não encontrado ou não está em status ativo para fechamento");
                }

                // Verificar se é um caixa central (não pode ter status alterado)
                if (IsCaixaCentral(caixa))
                {
                    return BadRequest("Caixas centrais não podem ter seu status alterado");
                }

                // Validar se a pessoa existe
                var pessoa = await ValidarPessoa(cdPessoaValidacao ?? 0, fonteDados.source);
                if (pessoa == null)
                {
                    return BadRequest("Pessoa não encontrada");
                }

                // Preparar dados para atualização
                var updateData = new Dictionary<string, object>
                {
                    ["id_status_caixa"] = (int)StatusCaixa.AguardandoValidacao,
                    ["cd_pessoa_solicitacao_fechamento"] = cdPessoaValidacao,
                    ["vl_saldo_real"] = request.vl_saldo_real
                };

                var updateResult = await SQLServerService.Update(
                    "T_CAIXA",
                    updateData,
                    fonteDados.source,
                    "cd_caixa",
                    cd_caixa
                );

                if (!updateResult.success)
                {
                    return BadRequest($"Erro ao alterar status do caixa: {updateResult.error}");
                }

                // Buscar dados atualizados do caixa
                var caixaAtualizado = await BuscarCaixaPorId(cd_caixa, fonteDados.source);

                // Buscar funcionários do caixa
                var funcionarios = await BuscarFuncionariosCaixa(cd_caixa, fonteDados.source);

                // Buscar dados da pessoa que solicitou o fechamento
                var pessoaSolicitante = await ValidarPessoa(request.cd_pessoa_solicitacao_fechamento, fonteDados.source);

                // Montar resposta
                var statusFinal = (StatusCaixa)Convert.ToInt32(caixaAtualizado["id_status_caixa"]);
                var responseData = new Dictionary<string, object>
                {
                    ["cd_caixa"] = caixaAtualizado["cd_caixa"],
                    ["dc_caixa"] = caixaAtualizado["dc_caixa"],
                    ["dt_abertura"] = caixaAtualizado["dt_abertura"],
                    ["dt_fechamento"] = caixaAtualizado.ContainsKey("dt_fechamento") ? caixaAtualizado["dt_fechamento"] : null,
                    ["id_status_caixa"] = caixaAtualizado["id_status_caixa"],
                    ["status"] = statusFinal.GetDescription(),
                    ["cd_empresa"] = caixaAtualizado["cd_empresa"],
                    ["cd_pessoa_solicitacao_fechamento"] = caixaAtualizado["cd_pessoa_solicitacao_fechamento"],
                    ["pessoa_solicitante"] = new
                    {
                        cd_pessoa = pessoaSolicitante?["cd_pessoa"],
                        no_pessoa = pessoaSolicitante?["no_pessoa"]
                    },
                    ["funcionarios"] = funcionarios?.Select(f => new
                    {
                        cd_pessoa = f["cd_pessoa"],
                        cd_caixa = f["cd_caixa"],
                        no_pessoa = f["no_pessoa"]
                    }).ToList()
                };

                return ResponseDefault(new
                {
                    success = true,
                    message = "Fechamento de caixa solicitado com sucesso",
                    data = responseData
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro interno: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPut("validarFechamento/{cd_caixa}")]
        public async Task<IActionResult> ValidarFechamento(int cd_caixa)
        {
            var fonteDados = ValidarFonteDados();
            if (!fonteDados.success)
            {
                return BadRequest(fonteDados.error);
            }

            try
            {
                var caixa = await BuscarCaixaPorId(cd_caixa, fonteDados.source);
                if (caixa == null)
                {
                    return BadRequest("Caixa não encontrado");
                }

                if (caixa["cd_pessoa_solicitacao_fechamento"] == null)
                {
                    return BadRequest("Não foi possível identificar a pessoa que solicitou o fechamento do caixa");
                }

                // Obter cd_pessoa do token JWT
                var cdPessoaValidacao = GetCdPessoaFromToken();
                if (cdPessoaValidacao == null)
                {
                    return BadRequest("Não foi possível identificar a pessoa que está validando o fechamento");
                }

                // Verificar se é um caixa central (não pode ter status alterado)
                if (IsCaixaCentral(caixa))
                {
                    return BadRequest("Caixas centrais não podem ter seu status alterado");
                }

                var statusAtual = (StatusCaixa)Convert.ToInt32(caixa["id_status_caixa"]);
                if (statusAtual != StatusCaixa.AguardandoValidacao)
                {
                    return BadRequest($"Caixa não pode ser validado. Status atual: {statusAtual.GetDescription()}");
                }

                // TODO: criar conta corrente no fechamento do caixa
                var caixaCentral = await BuscarCaixaCentralDaEmpresa(Convert.ToInt32(caixa["cd_empresa"]), fonteDados.source);
                if (caixaCentral == null)
                {
                    return BadRequest("Caixa central da empresa não encontrado");
                }

                var informacoesPorTipoLiquidacao = await BuscarInformacoesFinanceirasPorTipoLiquidacao(cd_caixa, fonteDados.source);

                if (informacoesPorTipoLiquidacao != null && informacoesPorTipoLiquidacao.Any())
                {
                    try
                    {
                        await CriarContasCorrentesFechamentoCaixa(caixa, caixaCentral, informacoesPorTipoLiquidacao, fonteDados.source);
                    }
                    catch (Exception ex)
                    {
                        return BadRequest($"Erro ao validar fechamento do caixa: {ex.Message}");
                    }
                }

                // Preparar dados para atualização incluindo a pessoa que validou
                var updateData = new Dictionary<string, object>
                {
                    ["id_status_caixa"] = (int)StatusCaixa.Fechado,
                    ["dt_fechamento"] = DateTime.Now,
                    ["cd_pessoa_validacao"] = cdPessoaValidacao
                };

                var updateResult = await SQLServerService.Update(
                    "T_CAIXA",
                    updateData,
                    fonteDados.source,
                    "cd_caixa",
                    cd_caixa
                );

                if (!updateResult.success)
                {
                    return BadRequest($"Erro ao validar fechamento do caixa: {updateResult.error}");
                }

                return ResponseDefault(new
                {
                    success = true,
                    message = "Fechamento de caixa validado com sucesso"
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro interno: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPut("rejeitarFechamento/{cd_caixa}")]
        public async Task<IActionResult> RejeitarFechamento(int cd_caixa)
        {
            return await AlterarStatusCaixa(cd_caixa, StatusCaixa.AguardandoValidacao, StatusCaixa.Rejeitado, "Fechamento de caixa rejeitado com sucesso");
        }

        [Authorize]
        [HttpGet("empresa/{cd_empresa}/funcionariosDisponiveis")]
        public async Task<IActionResult> GetFuncionariosDisponiveis(int cd_empresa, int? page = 1, int? limit = 50, string no_pessoa = null)
        {
            var fonteDados = ValidarFonteDados();
            if (!fonteDados.success)
            {
                return BadRequest(fonteDados.error);
            }

            try
            {
                // Validar se a empresa existe
                var empresa = await SQLServerService.GetFirstByFields(
                    fonteDados.source, "T_EMPRESA", 
                    new List<(string, object)> { ("cd_pessoa_empresa", cd_empresa) }
                );
                if (empresa == null)
                {
                    return BadRequest("Empresa não encontrada");
                }

                var query = @"
                    SELECT DISTINCT f.cd_funcionario, f.cd_pessoa_funcionario, p.no_pessoa, @cd_empresa as cd_empresa
                        FROM T_FUNCIONARIO f
                        INNER JOIN T_PESSOA p ON p.cd_pessoa = f.cd_pessoa_funcionario
                        WHERE f.cd_pessoa_empresa = @cd_empresa
                          AND NOT EXISTS (
                            SELECT 1 FROM T_CAIXA_PESSOA cp 
                            INNER JOIN T_CAIXA c ON c.cd_caixa = cp.cd_caixa 
                            WHERE cp.cd_pessoa = f.cd_pessoa_funcionario 
                            AND c.id_status_caixa = @status_aberto
                          )";

                // Adicionar filtro por nome se informado
                if (!string.IsNullOrEmpty(no_pessoa))
                {
                    query += " AND p.no_pessoa LIKE @nome";
                }

                query += " ORDER BY p.no_pessoa";

                var funcionariosDisponiveis = new List<Dictionary<string, object>>();
                var connectionString = $"Server={fonteDados.source.Host};Database={fonteDados.source.DbName};User Id={fonteDados.source.User};Password={fonteDados.source.Password};";
                
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@cd_empresa", cd_empresa);
                        command.Parameters.AddWithValue("@status_aberto", (int)StatusCaixa.EmAberto);
                        
                        // Adicionar parâmetro do filtro por nome se informado
                        if (!string.IsNullOrEmpty(no_pessoa))
                        {
                            command.Parameters.AddWithValue("@nome", $"%{no_pessoa}%");
                        }
                        
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var funcionario = new Dictionary<string, object>
                                {
                                    ["cd_pessoa"] = reader.IsDBNull(1) ? null : reader.GetValue(1),
                                    ["no_pessoa"] = reader.IsDBNull(2) ? "Nome não encontrado" : reader.GetValue(2),
                                    ["cd_empresa"] = reader.IsDBNull(3) ? null : reader.GetValue(3)
                                };
                                funcionariosDisponiveis.Add(funcionario);
                            }
                        }
                    }
                }

                // Implementar paginação manual nos resultados
                var totalRecords = funcionariosDisponiveis.Count;
                var pagedData = funcionariosDisponiveis;

                if (page.HasValue && limit.HasValue && page > 0 && limit > 0)
                {
                    var skip = (page.Value - 1) * limit.Value;
                    pagedData = funcionariosDisponiveis.Skip(skip).Take(limit.Value).ToList();
                }

                // Montar mensagem informativa
                var mensagem = $"Encontrados {totalRecords} funcionários disponíveis para abertura de caixa";
                if (!string.IsNullOrEmpty(no_pessoa))
                {
                    mensagem += $" com nome contendo '{no_pessoa}'";
                }

                return ResponseDefault(new
                {
                    data = new
                    {
                        total = pagedData.Count,
                        funcionarios = pagedData
                    },
                    page,
                    limit,
                    total_geral = totalRecords,
                    pages = limit != null ? (int)Math.Ceiling((double)totalRecords / limit.Value) : 0,
                    message = mensagem
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro interno: {ex.Message}");
            }
        }

        [Authorize]
        [HttpGet("buscarTitulos")]
        public async Task<IActionResult> GetAllTitulos(string value = "", SearchModeEnum mode = SearchModeEnum.Contains, int? page = 1, int? limit = 50, string sortField = "dt_vcto_titulo", bool sortDesc = false, string ids = "", string searchFields = null, string cd_empresa = null)
        {
            var fonteDados = ValidarFonteDados();
            if (!fonteDados.success)
            {
                return BadRequest(fonteDados.error);
            }

            try
            {
                // Query personalizada para buscar títulos disponíveis
                var query = @"
                    SELECT t.cd_titulo,
                           t.dt_vcto_titulo,
                           t.dt_emissao_titulo,
                           t.id_natureza_titulo,
                           t.dc_tipo_titulo,
                           t.vl_titulo,
                           t.cd_pessoa_empresa,
                           t.cd_pessoa_responsavel,
                           p.no_pessoa
                    FROM T_TITULO t
                    INNER JOIN T_PESSOA p ON p.cd_pessoa = t.cd_pessoa_responsavel
                    WHERE t.id_status_titulo = 1";

                // Adicionar filtro por empresa se informado
                if (!string.IsNullOrEmpty(cd_empresa))
                {
                    query += $" AND t.cd_pessoa_empresa = {cd_empresa}";
                }

                // Adicionar filtro de busca se informado
                if (!string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(searchFields))
                {
                    var whereClause = "";
                    var fields = searchFields.Split(',');
                    
                    foreach (var field in fields)
                    {
                        if (!string.IsNullOrEmpty(whereClause))
                            whereClause += " OR ";
                        
                        switch (mode)
                        {
                            case SearchModeEnum.Contains:
                                whereClause += $"t.{field.Trim()} LIKE '%{value}%' OR p.no_pessoa LIKE '%{value}%'";
                                break;
                            case SearchModeEnum.Equals:
                                whereClause += $"t.{field.Trim()} = '{value}' OR p.no_pessoa = '{value}'";
                                break;
                        }
                    }
                    
                    if (!string.IsNullOrEmpty(whereClause))
                    {
                        query += $" AND ({whereClause})";
                    }
                }
                else if (!string.IsNullOrEmpty(value))
                {
                    // Busca padrão nos campos principais
                    query += $" AND (p.no_pessoa LIKE '%{value}%' OR CAST(t.cd_titulo AS VARCHAR) LIKE '%{value}%')";
                }

                // Filtrar por IDs específicos se informado
                if (!string.IsNullOrEmpty(ids))
                {
                    query += $" AND t.cd_titulo IN ({ids})";
                }

                // Adicionar ordenação
                var orderDirection = sortDesc ? "DESC" : "ASC";
                if (!string.IsNullOrEmpty(sortField))
                {
                    // Mapear campos de ordenação
                    var validSortFields = new[] { "cd_titulo", "dt_vcto_titulo", "vl_titulo", "no_pessoa" };
                    var mappedSortField = validSortFields.Contains(sortField) ? sortField : "dt_vcto_titulo";
                    
                    if (mappedSortField == "no_pessoa")
                    {
                        query += $" ORDER BY p.{mappedSortField} {orderDirection}";
                    }
                    else
                    {
                        query += $" ORDER BY t.{mappedSortField} {orderDirection}";
                    }
                }
                else
                {
                    query += $" ORDER BY t.dt_vcto_titulo {orderDirection}";
                }

                // Executar query usando SqlConnection diretamente
                var titulos = new List<Dictionary<string, object>>();
                var connectionString = $"Server={fonteDados.source.Host};Database={fonteDados.source.DbName};User Id={fonteDados.source.User};Password={fonteDados.source.Password};";
                
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(query, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var titulo = new Dictionary<string, object>();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    titulo[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                                }
                                titulos.Add(titulo);
                            }
                        }
                    }
                }

                // Implementar paginação manual
                var totalRecords = titulos.Count;
                var pagedData = titulos;

                if (page.HasValue && limit.HasValue && page > 0 && limit > 0)
                {
                    var skip = (page.Value - 1) * limit.Value;
                    pagedData = titulos.Skip(skip).Take(limit.Value).ToList();
                }

                // Enriquecer dados dos títulos
                var titulosEnriquecidos = pagedData.Select(titulo => {
                    // Criar descrição concatenada: tipo + número + vcto + pessoa
                    var tipoTitulo = Convert.ToInt32(titulo["id_natureza_titulo"]) == 1 ? "Recebimento" : "Pagamento";
                    var numeroTitulo = titulo["cd_titulo"].ToString();
                    var vctoTitulo = Convert.ToDateTime(titulo["dt_vcto_titulo"]).ToString("dd/MM/yyyy");
                    var nomePessoa = titulo["no_pessoa"].ToString();

                    return new Dictionary<string, object>
                    {
                        ["cd_titulo"] = titulo["cd_titulo"],
                        ["nm_titulo"] = titulo["nm_titulo"],
                        ["nm_parcela_titulo"] = titulo["nm_parcela_titulo"],
                        ["dc_tipo_titulo"] = titulo["dc_tipo_titulo"],
                        ["dc_tipo_financeiro"] = titulo["dc_tipo_financeiro"],
                        ["dt_vcto_titulo"] = vctoTitulo,
                        ["vl_titulo"] = titulo["vl_titulo"],
                        ["vl_saldo_titulo"] = titulo["vl_saldo_titulo"],
                        ["id_natureza_titulo"] = titulo["id_natureza_titulo"],
                        ["tipo_natureza_titulo"] = tipoTitulo,
                        ["no_pessoa"] = titulo["no_pessoa"],
                    };
                }).ToList();

                return ResponseDefault(new
                {
                    data = new
                    {
                        total = titulosEnriquecidos.Count,
                        titulos = titulosEnriquecidos
                    },
                    total = totalRecords,
                    page = page ?? 1,
                    limit = limit ?? 50,
                    pages = limit != null ? (int)Math.Ceiling((double)totalRecords / limit.Value) : 0,
                    message = $"Encontrados {titulosEnriquecidos.Count} títulos disponíveis para recebimento"
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro interno: {ex.Message}");
            }
        }

        [Authorize]
        [HttpGet("{cd_caixa}")]
        public async Task<IActionResult> GetCaixaByCodigo(int cd_caixa)
        {
            var fonteDados = ValidarFonteDados();
            if (!fonteDados.success)
            {
                return BadRequest(fonteDados.error);
            }

            try
            {
                // Buscar o caixa
                var caixa = await SQLServerService.GetFirstByFields(
                    fonteDados.source, "T_CAIXA", 
                    new List<(string, object)> { ("cd_caixa", cd_caixa) }
                );

                if (caixa == null)
                {
                    return BadRequest("Caixa não encontrado");
                }

                // Buscar funcionários do caixa
                var funcionarios = await BuscarFuncionariosCaixa(cd_caixa, fonteDados.source);

                // Buscar informações financeiras do caixa
                var informacoesFinanceiras = await BuscarInformacoesFinanceirasCaixa(cd_caixa, fonteDados.source);

                // Buscar informações detalhadas por tipo de liquidação
                var informacoesPorTipoLiquidacao = await BuscarInformacoesFinanceirasPorTipoLiquidacao(cd_caixa, fonteDados.source);

                // Buscar títulos do caixa
                var titulos = await BuscarTitulosCaixa(cd_caixa, fonteDados.source);

                // Montar resposta enriquecida
                var statusCaixa = (StatusCaixa)Convert.ToInt32(caixa["id_status_caixa"]);
                var responseData = new Dictionary<string, object>
                {
                    ["cd_caixa"] = caixa["cd_caixa"],
                    ["dc_caixa"] = caixa["dc_caixa"],
                    ["cd_local_movto"] = caixa["cd_local_movto"],
                    ["dt_abertura"] = caixa["dt_abertura"],
                    ["dt_fechamento"] = caixa.ContainsKey("dt_fechamento") ? caixa["dt_fechamento"] : null,
                    ["id_status_caixa"] = caixa["id_status_caixa"],
                    ["status"] = statusCaixa.GetDescription(),
                    ["cd_empresa"] = caixa["cd_empresa"],
                    ["id_caixa_central"] = (caixa.ContainsKey("id_caixa_central") && caixa["id_caixa_central"] != null) ? caixa["id_caixa_central"] : false,
                    ["funcionarios"] = funcionarios?.Select(f => new
                    {
                        cd_pessoa = f["cd_pessoa"],
                        cd_caixa = f["cd_caixa"],
                        no_pessoa = f["no_pessoa"]
                    }).ToList(),
                    ["financeiro"] = new
                    {
                        total_entradas = informacoesFinanceiras.totalEntradas,
                        total_saidas = informacoesFinanceiras.totalSaidas,
                        saldo_atual = informacoesFinanceiras.saldoAtual,
                        tipos_liquidacao = informacoesPorTipoLiquidacao?.Select(tl => new
                        {
                            cd_tipo_liquidacao = tl["cd_tipo_liquidacao"],
                            dc_tipo_liquidacao = tl["dc_tipo_liquidacao"],
                            total_valor = tl["total_valor"],
                            quantidade_titulos = tl["quantidade_titulos"],
                            total_entradas = tl["total_entradas"],
                            total_saidas = tl["total_saidas"]
                        }).ToList()
                    },
                    ["titulos"] = titulos
                };

                return ResponseDefault(new
                {
                    success = true,
                    message = "Caixa encontrado",
                    data = responseData
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro interno: {ex.Message}");
            }
        }

        // Métodos auxiliares para refatoração
        private async Task<Dictionary<string, object>> BuscarCaixaPorId(int cd_caixa, Source source)
        {
            return await SQLServerService.GetFirstByFields(
                source, "T_CAIXA", 
                new List<(string, object)> { ("cd_caixa", cd_caixa) }
            );
        }

        private async Task<Dictionary<string, object>> ValidarCaixaAberto(int cd_caixa, Source source)
        {
            // Buscar caixa que esteja em status que se comporta como "aberto" (EmAberto, Rejeitado, Reaberto)
            return await SQLServerService.GetFirstByFields(
                source, "T_CAIXA", 
                new List<(string, object)> 
                { 
                    ("cd_caixa", cd_caixa)
                }
            );
        }

        private async Task<Dictionary<string, object>> ValidarEmpresa(int cd_empresa, Source source)
        {
            return await SQLServerService.GetFirstByFields(
                source, "T_EMPRESA", 
                new List<(string, object)> { ("cd_pessoa_empresa", cd_empresa) }
            );
        }
        
        private bool IsCaixaCentral(Dictionary<string, object> caixa)
        {
            return (caixa.ContainsKey("id_caixa_central") && caixa["id_caixa_central"] != null) 
                ? Convert.ToBoolean(caixa["id_caixa_central"]) 
                : false;
        }
        
        private int? GetCdPessoaFromToken()
        {
            var cdPessoaClaim = User.Claims.FirstOrDefault(c => c.Type == "cd_pessoa");
            if (cdPessoaClaim != null && int.TryParse(cdPessoaClaim.Value, out int cdPessoa))
            {
                return cdPessoa;
            }
            return null;
        }
        
        private async Task<Dictionary<string, object>> BuscarCaixaCentralDaEmpresa(int cd_empresa, Source source)
        {
            return await SQLServerService.GetFirstByFields(
                source, "T_CAIXA", 
                new List<(string, object)> 
                { 
                    ("cd_empresa", cd_empresa),
                    ("id_caixa_central", true)
                }
            );
        }
        
        private async Task CriarContasCorrentesFechamentoCaixa(Dictionary<string, object> caixa, Dictionary<string, object> caixaCentral, List<Dictionary<string, object>> informacoesPorTipoLiquidacao, Source source)
        {
            foreach (var info in informacoesPorTipoLiquidacao)
            {
                var valorContaCorrente = Convert.ToDecimal(info["total_valor"]);
                if (valorContaCorrente > 0)
                {
                    var contaCorrenteData = new Dictionary<string, object>
                    {
                        ["cd_local_origem"] = caixa["cd_local_movto"],
                        ["cd_local_destino"] = caixaCentral["cd_local_movto"],
                        ["cd_movimentacao_financeira"] = 3, //transferencia
                        ["dta_conta_corrente"] = DateTime.Today,
                        ["id_tipo_movimento"] = 2, // Saída do local origem (transferência para o central)
                        ["cd_pessoa_empresa"] = caixa["cd_empresa"],
                        ["cd_plano_conta"] = null,
                        ["vl_conta_corrente"] = valorContaCorrente,
                        ["dc_obs_conta_corrente"] = $"Fechamento do Caixa {caixa["dc_caixa"]}",
                        ["cd_tipo_liquidacao"] = info["cd_tipo_liquidacao"]
                    };

                    var res = await SQLServerService.Insert("T_CONTA_CORRENTE", contaCorrenteData, source);

                    if (!res.success)
                    {
                        throw new Exception(res.error);
                    }
                    var cc = await SQLServerService.GetFirstByFields(source,"T_CONTA_CORRENTE",
                        new List<(string, object)> { 
                            ("dta_conta_corrente", contaCorrenteData["dta_conta_corrente"]), 
                            ("cd_tipo_liquidacao", contaCorrenteData["cd_tipo_liquidacao"]),
                            ("cd_local_destino", contaCorrenteData["cd_local_destino"]),
                            ("cd_local_origem", contaCorrenteData["cd_local_origem"]),
                            ("vl_conta_corrente", contaCorrenteData["vl_conta_corrente"]),
                            ("id_tipo_movimento", contaCorrenteData["id_tipo_movimento"])});

                    if(cc != null)
                    {
                        var caixaTituloDict = new Dictionary<string, object>
                            {
                                { "cd_caixa", caixa["cd_caixa"] },
                                { "cd_conta_corrente", cc["cd_conta_corrente"] },
                                { "dt_recebimento", contaCorrenteData["dta_conta_corrente"] }
                            };
                        var insertCaixaTitulo = await SQLServerService.Insert("T_CAIXA_TITULO", caixaTituloDict, source);
                        var caixaCentralTituloDict = new Dictionary<string, object>
                            {
                                { "cd_caixa", caixaCentral["cd_caixa"] },
                                { "cd_conta_corrente", cc["cd_conta_corrente"] },
                                { "dt_recebimento", contaCorrenteData["dta_conta_corrente"] }
                            };
                        var insertCaixaCentralTitulo = await SQLServerService.Insert("T_CAIXA_TITULO", caixaCentralTituloDict, source);
                    }
                }
            }
        }

        private async Task CriarContasCorrentesReaberturaCaixa(Dictionary<string, object> caixa, Dictionary<string, object> caixaCentral, Source source)
        {
            // Buscar as transferências feitas no último fechamento deste caixa
            var query = @"
                SELECT cc.*, tl.dc_tipo_liquidacao
                FROM T_CONTA_CORRENTE cc
                INNER JOIN T_CAIXA_TITULO ct ON ct.cd_conta_corrente = cc.cd_conta_corrente
                INNER JOIN T_TIPO_LIQUIDACAO tl ON tl.cd_tipo_liquidacao = cc.cd_tipo_liquidacao
                WHERE ct.cd_caixa = @cd_caixa 
                AND cc.cd_local_origem = @cd_local_origem 
                AND cc.cd_local_destino = @cd_local_destino
                AND cc.cd_movimentacao_financeira = 3
                AND cc.id_tipo_movimento = 2
                AND cc.dc_obs_conta_corrente LIKE 'Fechamento do Caixa%'";

            var transferenciasOriginais = new List<Dictionary<string, object>>();
            var connectionString = $"Server={source.Host};Database={source.DbName};User Id={source.User};Password={source.Password};";

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@cd_caixa", caixa["cd_caixa"]);
                    command.Parameters.AddWithValue("@cd_local_origem", caixa["cd_local_movto"]);
                    command.Parameters.AddWithValue("@cd_local_destino", caixaCentral["cd_local_movto"]);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var transferencia = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                transferencia[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                            }
                            transferenciasOriginais.Add(transferencia);
                        }
                    }
                }
            }

            // Criar transferências de volta (do caixa central para o caixa)
            foreach (var transferenciaOriginal in transferenciasOriginais)
            {
                var valorTransferencia = Convert.ToDecimal(transferenciaOriginal["vl_conta_corrente"]);

                var contaCorrenteData = new Dictionary<string, object>
                {
                    ["cd_local_origem"] = caixaCentral["cd_local_movto"], // Agora origem é o caixa central
                    ["cd_local_destino"] = caixa["cd_local_movto"], // Destino é o caixa sendo reaberto
                    ["cd_movimentacao_financeira"] = 3, // transferência
                    ["dta_conta_corrente"] = DateTime.Today,
                    ["id_tipo_movimento"] = 2, // Saída do caixa central
                    ["cd_pessoa_empresa"] = caixa["cd_empresa"],
                    ["cd_plano_conta"] = null,
                    ["vl_conta_corrente"] = valorTransferencia,
                    ["dc_obs_conta_corrente"] = $"Reabertura do Caixa {caixa["dc_caixa"]}",
                    ["cd_tipo_liquidacao"] = transferenciaOriginal["cd_tipo_liquidacao"]
                };

                var res = await SQLServerService.Insert("T_CONTA_CORRENTE", contaCorrenteData, source);

                if (!res.success)
                {
                    throw new Exception($"Erro ao criar transferência de reabertura para tipo {transferenciaOriginal["dc_tipo_liquidacao"]}: {res.error}");
                }

                // Buscar a conta corrente recém-criada
                var cc = await SQLServerService.GetFirstByFields(source, "T_CONTA_CORRENTE",
                    new List<(string, object)> {
                        ("dta_conta_corrente", contaCorrenteData["dta_conta_corrente"]),
                        ("cd_tipo_liquidacao", contaCorrenteData["cd_tipo_liquidacao"]),
                        ("cd_local_origem", contaCorrenteData["cd_local_origem"]),
                        ("cd_local_destino", contaCorrenteData["cd_local_destino"]),
                        ("vl_conta_corrente", contaCorrenteData["vl_conta_corrente"])
                    });

                if (cc != null)
                {
                    var caixaTituloDict = new Dictionary<string, object>
                    {
                        { "cd_caixa", caixa["cd_caixa"] },
                        { "cd_conta_corrente", cc["cd_conta_corrente"] },
                        { "dt_recebimento", contaCorrenteData["dta_conta_corrente"] }
                    };

                    var insertCaixaTitulo = await SQLServerService.Insert("T_CAIXA_TITULO", caixaTituloDict, source);

                    if (!insertCaixaTitulo.success)
                    {
                        throw new Exception($"Erro ao inserir caixa título na reabertura: {insertCaixaTitulo.error}");
                    }
                    var caixaCentralTituloDict = new Dictionary<string, object>
                    {
                        { "cd_caixa", caixaCentral["cd_caixa"] },
                        { "cd_conta_corrente", cc["cd_conta_corrente"] },
                        { "dt_recebimento", contaCorrenteData["dta_conta_corrente"] }
                    };

                    var insertCaixaCentralTitulo = await SQLServerService.Insert("T_CAIXA_TITULO", caixaCentralTituloDict, source);

                    if (!insertCaixaTitulo.success)
                    {
                        throw new Exception($"Erro ao inserir caixa título na reabertura: {insertCaixaTitulo.error}");
                    }
                }
            }
        }

        private async Task<Dictionary<string, object>> ValidarLocalMovto(int cd_local_movto, Source source)
        {
            return await SQLServerService.GetFirstByFields(
                source, "T_LOCAL_MOVTO", 
                new List<(string, object)> { ("cd_local_movto", cd_local_movto) }
            );
        }


        private async Task<Dictionary<string, object>> ValidarPessoa(int cd_pessoa, Source source)
        {
            return await SQLServerService.GetFirstByFields(
                source, "T_PESSOA",
                new List<(string, object)> { ("cd_pessoa", cd_pessoa) }
            );
        }

        private async Task<int?> CriarLocalMovimento(Dictionary<string, object> caixa, Source source)
        {
            var localMovtoData = new Dictionary<string, object>
            {
                ["cd_pessoa_local"] = caixa["cd_pessoa_solicitacao_fechamento"],
                ["cd_pessoa_empresa"] = caixa["cd_empresa"],
                ["no_local_movto"] = caixa["dc_caixa"],
                ["nm_tipo_local"] = 3,
                ["id_local_ativo"] = 1
            };

            var insertResult = await SQLServerService.InsertWithResult("T_LOCAL_MOVTO", localMovtoData, source);
            if (!insertResult.success)
            {
                return null;
            }

            return Convert.ToInt32(insertResult.inserted["cd_local_movto"]);
        }

        private async Task<bool> ValidarFuncionarioTemCaixaAberto(int cd_pessoa, int cd_empresa, Source source)
        {
            // Verificar apenas se o funcionário tem algum caixa com status "Em Aberto" ou "Reaberto"
            // Permitir abertura se houver caixas em outros status (Aguardando Validação, Rejeitado, Fechado)
            var query = @"
                SELECT COUNT(*) as count 
                FROM T_CAIXA_PESSOA cp
                INNER JOIN T_CAIXA c ON c.cd_caixa = cp.cd_caixa
                WHERE cp.cd_pessoa = @cd_pessoa 
                AND c.cd_empresa = @cd_empresa 
                AND (c.id_status_caixa = @status_aberto)";

            var connectionString = $"Server={source.Host};Database={source.DbName};User Id={source.User};Password={source.Password};";
            
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@cd_pessoa", cd_pessoa);
                    command.Parameters.AddWithValue("@cd_empresa", cd_empresa);
                    command.Parameters.AddWithValue("@status_aberto", (int)StatusCaixa.EmAberto);
                    
                    var result = await command.ExecuteScalarAsync();
                    return Convert.ToInt32(result) > 0;
                }
            }
        }

        private (bool success, string error) ValidarModelState()
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return (false, $"Dados inválidos: {string.Join(", ", errors)}");
            }
            return (true, null);
        }

        private (Source source, bool success, string error) ValidarFonteDados()
        {
            var schemaName = "T_Caixa";
            if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "").Replace("_", "");
            
            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
            var source = _sourceRepository.GetByField("description", schemaModel.Source);
            
            if (source == null || source.Active != true)
            {
                return (null, false, "Fonte de dados não configurada ou inativa.");
            }

            return (source, true, null);
        }

        private async Task<List<Dictionary<string, object>>> BuscarFuncionariosCaixa(int cd_caixa, Source source)
        {
            var query = @"
                SELECT cp.cd_pessoa, cp.cd_caixa, p.no_pessoa
                FROM T_CAIXA_PESSOA cp
                INNER JOIN T_PESSOA p ON p.cd_pessoa = cp.cd_pessoa
                WHERE cp.cd_caixa = @cd_caixa";

            var funcionarios = new List<Dictionary<string, object>>();
            var connectionString = $"Server={source.Host};Database={source.DbName};User Id={source.User};Password={source.Password};";
            
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@cd_caixa", cd_caixa);
                    
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var funcionario = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                funcionario[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                            }
                            funcionarios.Add(funcionario);
                        }
                    }
                }
            }
            return funcionarios;
        }

        private async Task<(decimal totalEntradas, decimal totalSaidas, decimal saldoAtual)> BuscarInformacoesFinanceirasCaixa(int cd_caixa, Source source)
        {
            // Primeiro buscar o cd_local_movto do caixa
            var caixa = await BuscarCaixaPorId(cd_caixa, source);
            if (caixa == null)
            {
                return (0, 0, 0);
            }
            
            var cdLocalMovto = Convert.ToInt32(caixa["cd_local_movto"]);
            
            var query = @"
                SELECT 
                    SUM(CASE 
                        WHEN (cc.cd_local_origem = @cd_local_movto AND cc.id_tipo_movimento = 1) 
                        OR (cc.cd_local_destino = @cd_local_movto AND cc.id_tipo_movimento = 2) 
                        THEN cc.vl_conta_corrente 
                        ELSE 0 
                    END) as total_entradas,
                    SUM(CASE 
                        WHEN (cc.cd_local_origem = @cd_local_movto AND cc.id_tipo_movimento = 2) 
                        OR (cc.cd_local_destino = @cd_local_movto AND cc.id_tipo_movimento = 1) 
                        THEN cc.vl_conta_corrente 
                        ELSE 0 
                    END) as total_saidas
                FROM T_CAIXA_TITULO ct
                INNER JOIN T_CONTA_CORRENTE cc ON cc.cd_conta_corrente = ct.cd_conta_corrente
                WHERE ct.cd_caixa = @cd_caixa
                ";

            var connectionString = $"Server={source.Host};Database={source.DbName};User Id={source.User};Password={source.Password};";
            
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@cd_caixa", cd_caixa);
                    command.Parameters.AddWithValue("@cd_local_movto", cdLocalMovto);
                    
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            var totalEntradas = reader.IsDBNull(0) ? 0 : Convert.ToDecimal(reader.GetValue(0));
                            var totalSaidas = reader.IsDBNull(1) ? 0 : Convert.ToDecimal(reader.GetValue(1));
                            return (totalEntradas, totalSaidas, totalEntradas - totalSaidas);
                        }
                    }
                }
            }
            return (0, 0, 0);
        }

        private async Task<List<Dictionary<string, object>>> BuscarInformacoesFinanceirasPorTipoLiquidacao(int cd_caixa, Source source)
        {
            // Primeiro buscar o cd_local_movto do caixa
            var caixa = await BuscarCaixaPorId(cd_caixa, source);
            if (caixa == null)
            {
                return new List<Dictionary<string, object>>();
            }
            
            var cdLocalMovto = Convert.ToInt32(caixa["cd_local_movto"]);
            
            var query = @"
                SELECT 
                    tl.cd_tipo_liquidacao,
                    tl.dc_tipo_liquidacao,
                    SUM(CASE 
                        WHEN (cc.cd_local_origem = @cd_local_movto AND cc.id_tipo_movimento = 1) 
                        OR (cc.cd_local_destino = @cd_local_movto AND cc.id_tipo_movimento = 2) 
                        THEN cc.vl_conta_corrente 
                        ELSE 0 
                    END) - 
                    SUM(CASE 
                        WHEN (cc.cd_local_origem = @cd_local_movto AND cc.id_tipo_movimento = 2) 
                        OR (cc.cd_local_destino = @cd_local_movto AND cc.id_tipo_movimento = 1) 
                        THEN cc.vl_conta_corrente 
                        ELSE 0 
                    END) as total_valor,
                    COUNT(ct.cd_titulo) as quantidade_titulos,
                    SUM(CASE 
                        WHEN (cc.cd_local_origem = @cd_local_movto AND cc.id_tipo_movimento = 1) 
                        OR (cc.cd_local_destino = @cd_local_movto AND cc.id_tipo_movimento = 2) 
                        THEN cc.vl_conta_corrente 
                        ELSE 0 
                    END) as total_entradas,
                    SUM(CASE 
                        WHEN (cc.cd_local_origem = @cd_local_movto AND cc.id_tipo_movimento = 2) 
                        OR (cc.cd_local_destino = @cd_local_movto AND cc.id_tipo_movimento = 1) 
                        THEN cc.vl_conta_corrente 
                        ELSE 0 
                    END) as total_saidas
                FROM T_CAIXA_TITULO ct
                INNER JOIN T_CONTA_CORRENTE cc ON cc.cd_conta_corrente = ct.cd_conta_corrente
                INNER JOIN T_TIPO_LIQUIDACAO tl ON tl.cd_tipo_liquidacao = cc.cd_tipo_liquidacao
                WHERE ct.cd_caixa = @cd_caixa
                GROUP BY tl.cd_tipo_liquidacao, tl.dc_tipo_liquidacao
                ORDER BY total_valor DESC";

            var tiposLiquidacao = new List<Dictionary<string, object>>();
            var connectionString = $"Server={source.Host};Database={source.DbName};User Id={source.User};Password={source.Password};";
            
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@cd_caixa", cd_caixa);
                    command.Parameters.AddWithValue("@cd_local_movto", cdLocalMovto);
                    
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var tipoLiquidacao = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                tipoLiquidacao[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                            }
                            tiposLiquidacao.Add(tipoLiquidacao);
                        }
                    }
                }
            }
            return tiposLiquidacao;
        }

        private async Task<List<Dictionary<string, object>>> BuscarTitulosCaixa(int cd_caixa, Source source)
        {
            var query = @"
                SELECT cc.*, p.no_pessoa, sc.no_subgrupo_conta
                    FROM T_CAIXA_TITULO ct
                    INNER JOIN T_CONTA_CORRENTE cc ON cc.cd_conta_corrente = ct.cd_conta_corrente
                    LEFT JOIN T_TITULO t ON t.cd_titulo = ct.cd_titulo
                    LEFT JOIN T_PESSOA p ON p.cd_pessoa = t.cd_pessoa_responsavel
                    LEFT JOIN T_PLANO_CONTA pc ON cc.cd_plano_conta = pc.cd_plano_conta
                    LEFT JOIN T_SUBGRUPO_CONTA sc ON pc.cd_subgrupo_conta = sc.cd_subgrupo_conta
                    WHERE ct.cd_caixa = @cd_caixa
                    ORDER BY cc.dta_conta_corrente DESC";

            var titulos = new List<Dictionary<string, object>>();
            var connectionString = $"Server={source.Host};Database={source.DbName};User Id={source.User};Password={source.Password};";
            
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@cd_caixa", cd_caixa);
                    
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var titulo = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                titulo[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                            }
                            titulos.Add(titulo);
                        }
                    }
                }
            }
            return titulos;
        }

        private async Task<IActionResult> AlterarStatusCaixa(int cd_caixa, StatusCaixa statusOrigem, StatusCaixa statusDestino, string mensagem, bool incluirDataFechamento = false)
        {
            var fonteDados = ValidarFonteDados();
            if (!fonteDados.success)
            {
                return BadRequest(fonteDados.error);
            }

            var caixa = await SQLServerService.GetFirstByFields(
                fonteDados.source, "T_CAIXA", 
                new List<(string, object)> 
                { 
                    ("cd_caixa", cd_caixa),
                    ("id_status_caixa", (int)statusOrigem)
                }
            );

            if (caixa == null)
            {
                return BadRequest($"Caixa não encontrado ou não está no status esperado");
            }

            // Verificar se é um caixa central (não pode ter status alterado)
            if (IsCaixaCentral(caixa))
            {
                return BadRequest("Caixas centrais não podem ter seu status alterado");
            }

            var updateData = new Dictionary<string, object>
            {
                ["id_status_caixa"] = (int)statusDestino
            };

            if (incluirDataFechamento)
            {
                updateData["dt_fechamento"] = DateTime.Now;
            }

            var updateResult = await SQLServerService.Update(
                "T_CAIXA",
                updateData,
                fonteDados.source,
                "cd_caixa",
                cd_caixa
            );

            if (!updateResult.success)
            {
                return BadRequest($"Erro ao alterar status do caixa: {updateResult.error}");
            }

            var caixaAtualizado = await SQLServerService.GetFirstByFields(
                fonteDados.source, "T_CAIXA", 
                new List<(string, object)> { ("cd_caixa", cd_caixa) }
            );

            var funcionarios = await BuscarFuncionariosCaixa(cd_caixa, fonteDados.source);

            var statusFinal = (StatusCaixa)Convert.ToInt32(caixaAtualizado["id_status_caixa"]);
            var responseData = new Dictionary<string, object>
            {
                ["cd_caixa"] = caixaAtualizado["cd_caixa"],
                ["dc_caixa"] = caixaAtualizado["dc_caixa"],
                ["dt_abertura"] = caixaAtualizado["dt_abertura"],
                ["dt_fechamento"] = caixaAtualizado.ContainsKey("dt_fechamento") ? caixaAtualizado["dt_fechamento"] : null,
                ["id_status_caixa"] = caixaAtualizado["id_status_caixa"],
                ["status"] = statusFinal.GetDescription(),
                ["cd_empresa"] = caixaAtualizado["cd_empresa"],
                ["funcionarios"] = funcionarios?.Select(f => new
                {
                    cd_pessoa = f["cd_pessoa"],
                    cd_caixa = f["cd_caixa"],
                    no_pessoa = f["no_pessoa"]
                }).ToList()
            };

            return ResponseDefault(new
            {
                success = true,
                message = mensagem,
                data = responseData
            });
        }

        private async Task<(bool success, string error)> ValidarFuncionariosParaAbertura(List<int> funcionarios, int cd_empresa, Source source)
        {
            foreach (var cd_pessoa in funcionarios)
            {
                var funcionario = await SQLServerService.GetFirstByFields(
                    source, "T_PESSOA", 
                    new List<(string, object)> { ("cd_pessoa", cd_pessoa) }
                );
                if (funcionario == null)
                {
                    return (false, $"Pessoa {cd_pessoa} não encontrada");
                }

                var temCaixaAberto = await ValidarFuncionarioTemCaixaAberto(cd_pessoa, cd_empresa, source);
                if (temCaixaAberto)
                {
                    return (false, $"Pessoa {cd_pessoa} já possui um caixa aberto");
                }
            }
            return (true, null);
        }

        private async Task<int?> CriarCaixa(InsertCaixaModel command, Source source)
        {
            var insertCaixaData = new Dictionary<string, object>
            {
                ["dc_caixa"] = command.dc_caixa,
                ["cd_local_movto"] = command.cd_local_movto,
                ["dt_abertura"] = command.dt_abertura ?? DateTime.Now,
                ["id_status_caixa"] = 0,
                ["cd_empresa"] = command.cd_empresa,
                ["id_caixa_central"] = command.id_caixa_central ?? false
            };

            var insertResult = await SQLServerService.InsertWithResult("T_CAIXA", insertCaixaData, source);
            
            if (!insertResult.success) return null;

            return Convert.ToInt32(insertResult.inserted["cd_caixa"]);
        }

        private async Task<(bool success, string error)> VincularFuncionariosCaixa(int cdCaixa, List<int> funcionarios, Source source)
        {
            foreach (var cdFuncionario in funcionarios)
            {
                var insertFuncionarioData = new Dictionary<string, object>
                {
                    ["cd_caixa"] = cdCaixa,
                    ["cd_pessoa"] = cdFuncionario
                };

                var insertResult = await SQLServerService.Insert("T_CAIXA_PESSOA", insertFuncionarioData, source);
                if (!insertResult.success)
                {
                    return (false, $"Erro ao inserir funcionário {cdFuncionario} no caixa: {insertResult.error}");
                }
            }
            return (true, null);
        }

        [Authorize]
        [HttpGet("")]
        public async Task<IActionResult> GetCaixas(
            int? cd_empresa = null,
            int? cd_pessoa = null, 
            int? status = null,
            string dc_caixa = null,
            DateTime? dt_inicio = null,
            DateTime? dt_fim = null,
            int? page = 1,
            int? limit = 10,
            string sortField = "dt_abertura",
            bool sortDesc = true)
        {
            var fonteDados = ValidarFonteDados();
            if (!fonteDados.success)
            {
                return BadRequest(fonteDados.error);
            }

            try
            {
                // Construir query base
                var query = @"
                    SELECT c.*, 
                           COUNT(*) OVER() as total_records
                    FROM T_CAIXA c";

                var whereConditions = new List<string>();
                var parameters = new List<(string, object)>();

                // Filtro por empresa
                if (cd_empresa.HasValue)
                {
                    whereConditions.Add("c.cd_empresa = @cd_empresa");
                    parameters.Add(("@cd_empresa", cd_empresa.Value));
                }

                // Filtro por funcionário (pessoa)
                if (cd_pessoa.HasValue)
                {
                    query += " INNER JOIN T_CAIXA_PESSOA cp ON cp.cd_caixa = c.cd_caixa";
                    whereConditions.Add("cp.cd_pessoa = @cd_pessoa");
                    parameters.Add(("@cd_pessoa", cd_pessoa.Value));
                }

                // Filtro por status
                if (status.HasValue)
                {
                    whereConditions.Add("c.id_status_caixa = @status");
                    parameters.Add(("@status", status.Value));
                }

                // Filtro por dc_caixa
                if (!string.IsNullOrWhiteSpace(dc_caixa))
                {
                    whereConditions.Add("c.dc_caixa LIKE @dc_caixa");
                    parameters.Add(("@dc_caixa", $"%{dc_caixa}%"));
                }

                // Filtro por data de início
                if (dt_inicio.HasValue)
                {
                    whereConditions.Add("c.dt_abertura >= @dt_inicio");
                    parameters.Add(("@dt_inicio", dt_inicio.Value.Date));
                }

                // Filtro por data de fim
                if (dt_fim.HasValue)
                {
                    whereConditions.Add("c.dt_abertura <= @dt_fim");
                    parameters.Add(("@dt_fim", dt_fim.Value.Date.AddDays(1).AddSeconds(-1)));
                }

                // Aplicar filtros WHERE
                if (whereConditions.Any())
                {
                    query += " WHERE " + string.Join(" AND ", whereConditions);
                }

                // Aplicar ordenação
                var validSortFields = new[] { "cd_caixa", "dc_caixa", "dt_abertura", "dt_fechamento", "id_status_caixa" };
                var mappedSortField = validSortFields.Contains(sortField) ? sortField : "dt_abertura";
                var orderDirection = sortDesc ? "DESC" : "ASC";
                
                // Ordenação em três níveis: id_caixa_central, id_status_caixa (com status 2 por último), depois sortField
                query += $" ORDER BY c.id_caixa_central DESC, CASE WHEN c.id_status_caixa = 2 THEN 999 ELSE c.id_status_caixa END ASC, c.{mappedSortField} {orderDirection}";

                // Aplicar paginação
                if (page.HasValue && limit.HasValue && page > 0 && limit > 0)
                {
                    var offset = (page.Value - 1) * limit.Value;
                    query += $" OFFSET {offset} ROWS FETCH NEXT {limit.Value} ROWS ONLY";
                }

                // Executar query
                var caixas = new List<Dictionary<string, object>>();
                int totalRecords = 0;
                var connectionString = $"Server={fonteDados.source.Host};Database={fonteDados.source.DbName};User Id={fonteDados.source.User};Password={fonteDados.source.Password};";

                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(query, connection))
                    {
                        // Adicionar parâmetros
                        foreach (var param in parameters)
                        {
                            command.Parameters.AddWithValue(param.Item1, param.Item2);
                        }

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var caixa = new Dictionary<string, object>();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    var columnName = reader.GetName(i);
                                    if (columnName == "total_records")
                                    {
                                        totalRecords = Convert.ToInt32(reader.GetValue(i));
                                    }
                                    else
                                    {
                                        caixa[columnName] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                                    }
                                }
                                if (caixa.Any()) // Só adicionar se não for linha apenas com total_records
                                    caixas.Add(caixa);
                            }
                        }
                    }
                }

                // Processar e enriquecer dados dos caixas
                var caixasProcessados = new List<Dictionary<string, object>>();
                var contadores = new { abertos = 0, aguardandoValidacao = 0, fechados = 0, rejeitados = 0, reabertos = 0 };

                foreach (var caixa in caixas)
                {
                    var statusCaixa = (StatusCaixa)Convert.ToInt32(caixa["id_status_caixa"]);
                    var cdCaixa = Convert.ToInt32(caixa["cd_caixa"]);

                    // Contar por status
                    switch (statusCaixa)
                    {
                        case StatusCaixa.EmAberto:
                            contadores = new { abertos = contadores.abertos + 1, contadores.aguardandoValidacao, contadores.fechados, contadores.rejeitados, contadores.reabertos };
                            break;
                        case StatusCaixa.AguardandoValidacao:
                            contadores = new { contadores.abertos, aguardandoValidacao = contadores.aguardandoValidacao + 1, contadores.fechados, contadores.rejeitados, contadores.reabertos };
                            break;
                        case StatusCaixa.Fechado:
                            contadores = new { contadores.abertos, contadores.aguardandoValidacao, fechados = contadores.fechados + 1, contadores.rejeitados, contadores.reabertos };
                            break;
                        case StatusCaixa.Rejeitado:
                            contadores = new { contadores.abertos, contadores.aguardandoValidacao, contadores.fechados, rejeitados = contadores.rejeitados + 1, contadores.reabertos };
                            break;
                        case StatusCaixa.Reaberto:
                            contadores = new { contadores.abertos, contadores.aguardandoValidacao, contadores.fechados, contadores.rejeitados, reabertos = contadores.reabertos + 1 };
                            break;
                    }

                    // Buscar funcionários do caixa
                    var funcionarios = await BuscarFuncionariosCaixa(cdCaixa, fonteDados.source);

                    // Buscar informações financeiras do caixa
                    var informacoesFinanceiras = await BuscarInformacoesFinanceirasCaixa(cdCaixa, fonteDados.source);

                    // Montar dados do caixa enriquecido
                    var caixaCompleto = new Dictionary<string, object>
                    {
                        ["cd_caixa"] = caixa["cd_caixa"],
                        ["dc_caixa"] = caixa["dc_caixa"],
                        ["dt_abertura"] = caixa["dt_abertura"],
                        ["dt_fechamento"] = caixa.ContainsKey("dt_fechamento") ? caixa["dt_fechamento"] : null,
                        ["id_status_caixa"] = caixa["id_status_caixa"],
                        ["status"] = statusCaixa.GetDescription(),
                        ["cd_empresa"] = caixa["cd_empresa"],
                        ["cd_local_movto"] = caixa["cd_local_movto"],
                        ["id_caixa_central"] = (caixa.ContainsKey("id_caixa_central") && caixa["id_caixa_central"] != null) ? caixa["id_caixa_central"] : false,
                        ["funcionarios"] = funcionarios?.Select(f => new
                        {
                            cd_pessoa = f["cd_pessoa"],
                            cd_caixa = f["cd_caixa"],
                            no_pessoa = f["no_pessoa"]
                        }).ToList(),
                        ["financeiro"] = new
                        {
                            total_entradas = informacoesFinanceiras.totalEntradas,
                            total_saidas = informacoesFinanceiras.totalSaidas,
                            saldo_atual = informacoesFinanceiras.saldoAtual
                        }
                    };

                    caixasProcessados.Add(caixaCompleto);
                }

                // Montar resposta com informações de filtros aplicados
                var filtrosAplicados = new Dictionary<string, object>();
                if (cd_empresa.HasValue) filtrosAplicados["cd_empresa"] = cd_empresa.Value;
                if (cd_pessoa.HasValue) filtrosAplicados["cd_pessoa"] = cd_pessoa.Value;
                if (!string.IsNullOrWhiteSpace(dc_caixa)) filtrosAplicados["dc_caixa"] = dc_caixa;
                if (dt_inicio.HasValue) filtrosAplicados["dt_inicio"] = dt_inicio.Value.ToString("yyyy-MM-dd");
                if (dt_fim.HasValue) filtrosAplicados["dt_fim"] = dt_fim.Value.ToString("yyyy-MM-dd");
                if (status.HasValue) 
                {
                    var statusEnum = (StatusCaixa)status.Value;
                    filtrosAplicados["status"] = new { id = status.Value, descricao = statusEnum.GetDescription() };
                }

                return ResponseDefault(new
                {
                    data = new
                    {
                        total = caixasProcessados.Count,
                        resumo = contadores,
                        filtros_aplicados = filtrosAplicados,
                        caixas = caixasProcessados
                    },
                    total = totalRecords,
                    page,
                    limit,
                    pages = limit != null ? (int)Math.Ceiling((double)totalRecords / limit.Value) : 0
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro interno: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPut("reabrirCaixa/{cd_caixa}")]
        public async Task<IActionResult> ReabrirCaixa(int cd_caixa)
        {
            var fonteDados = ValidarFonteDados();
            if (!fonteDados.success)
            {
                return BadRequest(fonteDados.error);
            }

            try
            {
                // Buscar o caixa para validações
                var caixa = await BuscarCaixaPorId(cd_caixa, fonteDados.source);
                if (caixa == null)
                {
                    return BadRequest("Caixa não encontrado");
                }

                var statusAtual = (StatusCaixa)Convert.ToInt32(caixa["id_status_caixa"]);

                // Verificar se é um caixa central (não pode ter status alterado)
                if (IsCaixaCentral(caixa))
                {
                    return BadRequest("Caixas centrais não podem ser reabertos");
                }

                // Verificar se o caixa pode ser reaberto (deve estar fechado ou rejeitado)
                if (statusAtual != StatusCaixa.Fechado && statusAtual != StatusCaixa.Rejeitado)
                {
                    return BadRequest($"Caixa não pode ser reaberto. Status atual: {statusAtual.GetDescription()}");
                }

                // Se o caixa estava fechado, fazer a transferência de volta do caixa central
                if (statusAtual == StatusCaixa.Fechado)
                {
                    var caixaCentral = await BuscarCaixaCentralDaEmpresa(Convert.ToInt32(caixa["cd_empresa"]), fonteDados.source);
                    if (caixaCentral == null)
                    {
                        return BadRequest("Caixa central da empresa não encontrado");
                    }

                    try
                    {
                        await CriarContasCorrentesReaberturaCaixa(caixa, caixaCentral, fonteDados.source);
                    }
                    catch (Exception ex)
                    {
                        return BadRequest($"Erro ao processar transferências de reabertura: {ex.Message}");
                    }
                }
                // Alterar status para reaberto
                return await AlterarStatusCaixa(cd_caixa, statusAtual, StatusCaixa.Reaberto, "Caixa reaberto com sucesso");
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro interno: {ex.Message}");
            }
        }

        private bool IsStatusCaixaAtivo(StatusCaixa status)
        {
            // Status que permitem operações no caixa (comportam-se como "aberto")
            return status == StatusCaixa.EmAberto || 
                   status == StatusCaixa.Rejeitado || 
                   status == StatusCaixa.Reaberto;
        }

        private async Task<Dictionary<string, object>> ValidarCaixaComStatusAtivo(int cd_caixa, Source source)
        {
            var caixa = await BuscarCaixaPorId(cd_caixa, source);
            if (caixa == null) return null;

            var status = (StatusCaixa)Convert.ToInt32(caixa["id_status_caixa"]);
            return IsStatusCaixaAtivo(status) ? caixa : null;
        }
    }
}

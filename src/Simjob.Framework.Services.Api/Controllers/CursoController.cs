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
using Simjob.Framework.Services.Api.Models.Curso;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Simjob.Framework.Services.Api.Controllers
{
    public class CursoController : BaseController
    {
        private readonly IRepository<SourceContext, Source> _sourceRepository;
        private readonly IRepository<MongoDbContext, Schema> _schemaRepository;

        public CursoController(IMediatorHandler bus, INotificationHandler<DomainNotification> notifications, IRepository<SourceContext, Source> sourceRepository, IRepository<MongoDbContext, Schema> schemaRepository) : base(bus, notifications)
        {
            _sourceRepository = sourceRepository;
            _schemaRepository = schemaRepository;
        }

        [Authorize]
        [HttpPost("GetTabelaCurso")]
        public async Task<IActionResult> GetTabelaCurso([FromBody] GetTabelaCursoModel model)
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
                var resultado = await ExecutarStoredProcedure(model, fonteDados.source);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao executar consulta: {ex.Message}");
            }
        }

        [Authorize]
        [HttpGet]
        [Route("material")]
        public async Task<IActionResult> GetMaterialCurso([FromQuery] string cd_curso, [FromQuery] int cd_escola)
        {
            var fonteDados = ValidarFonteDados();
            if (!fonteDados.success)
            {
                return BadRequest(fonteDados.error);
            }
            var source = fonteDados.source;
            
            var cursos = cd_curso
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => int.Parse(x.Trim()))
                .ToList();

            if (!cursos.Any())
                return BadRequest("Nenhum curso informado.");

            string connectionString = $"Server={source.Host};Database={source.DbName};User Id={source.User};Password={source.Password};MultipleActiveResultSets=True;";

            var itens = new List<(int cd_curso,int cd_item, string no_item, int qt_estoque, int id_ppt)>();
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                // monta um IN (@curso0,@curso1,...)
                var inParams = string.Join(",", cursos.Select((c, i) => $"@curso{i}"));
                var query = $@"
                    SELECT i.cd_item, i.no_item, ie.qt_estoque, ic.id_ppt,ic.cd_curso
                    FROM T_ITEM_CURSO ic
                    INNER JOIN T_ITEM i ON i.cd_item = ic.cd_item
                    INNER JOIN T_ITEM_ESCOLA ie ON ie.cd_item = ic.cd_item
                    WHERE ic.cd_curso IN ({inParams})
                      AND cd_pessoa_escola = @cd_escola";

                var selectCmd = new SqlCommand(query, connection);
                selectCmd.Parameters.AddWithValue("@cd_escola", cd_escola);

                for (int i = 0; i < cursos.Count; i++)
                {
                    selectCmd.Parameters.AddWithValue($"@curso{i}", cursos[i]);
                }

                using (var reader = await selectCmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        itens.Add((
                            cd_curso: Convert.ToInt32(reader["cd_curso"]),
                            cd_item: Convert.ToInt32(reader["cd_item"]),
                            no_item: reader["no_item"].ToString(),
                            qt_estoque: Convert.ToInt32(reader["qt_estoque"]),
                            id_ppt: Convert.ToInt32(reader["id_ppt"])
                        ));
                    }
                }
            }

            return ResponseDefault(itens.Select(x => new
            {
                x.cd_curso,
                x.cd_item,
                x.no_item,
                x.qt_estoque,
                x.id_ppt
            }));
        }

        private async Task<TabelaCursoResponseModel> ExecutarStoredProcedure(GetTabelaCursoModel model, Source source)
        {
            var connectionString = $"Server={source.Host},{source.Port};Database={source.DbName};User Id={source.User};Password={source.Password};TrustServerCertificate=True";

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                var response = new TabelaCursoResponseModel
                {
                    PageNumber = model.PageNumber,
                    PageSize = model.PageSize,
                    Succeeded = true,
                    Data = new List<TabelaCursoData>()
                };

                try
                {
                    // Variáveis da stored procedure
                    int cdEscola = model.CdPessoaEscola;
                    int cdCurso = model.CdCurso;
                    int cdDuracao = model.CdDuracao;
                    int cdRegime = model.CdRegime;

                    // Validar e ajustar a data se necessário
                    DateTime dtTabela;
                    if (model.DtTabela.HasValue)
                    {
                        dtTabela = model.DtTabela.Value;
                        DateTime minSqlDate = new DateTime(1753, 1, 1);
                        DateTime maxSqlDate = new DateTime(9999, 12, 31);

                        if (dtTabela < minSqlDate)
                            dtTabela = minSqlDate;
                        else if (dtTabela > maxSqlDate)
                            dtTabela = maxSqlDate;
                    }
                    else
                    {
                        dtTabela = DateTime.Now;
                    }

                    int cdTabela = 0;
                    decimal vlPrecoMaterial = 0;
                    decimal vlPrecoMaterialPpt = 0;
                    int cdItem = 0;
                    bool semTabelaC = false;
                    bool semTabelaM = false;
                    string errMsg = "";

                    // Buscar tabela de preço do curso com regime específico
                    var queryTabelaComRegime = @"
                        SELECT cd_tabela_preco
                        FROM T_TABELA_PRECO p
                        WHERE cd_pessoa_escola = @cd_escola
                          AND cd_curso = @cd_curso
                          AND cd_duracao = @cd_duracao
                          AND cd_regime = @cd_regime
                          AND dta_tabela_preco = (
                              SELECT MAX(dta_tabela_preco)
                              FROM T_TABELA_PRECO
                              WHERE cd_pessoa_escola = p.cd_pessoa_escola
                                AND cd_curso = p.cd_curso
                                AND cd_duracao = p.cd_duracao
                                AND cd_regime = p.cd_regime
                                AND dta_tabela_preco <= @dt_tabela
                          )";

                    using (var cmd = new SqlCommand(queryTabelaComRegime, connection))
                    {
                        cmd.Parameters.AddWithValue("@cd_escola", cdEscola);
                        cmd.Parameters.AddWithValue("@cd_curso", cdCurso);
                        cmd.Parameters.AddWithValue("@cd_duracao", cdDuracao);
                        cmd.Parameters.AddWithValue("@cd_regime", cdRegime);
                        cmd.Parameters.AddWithValue("@dt_tabela", dtTabela);

                        var result = await cmd.ExecuteScalarAsync();
                        if (result != null && result != DBNull.Value)
                        {
                            cdTabela = Convert.ToInt32(result);
                        }
                    }

                    // Se não encontrou, buscar tabela sem regime
                    if (cdTabela == 0)
                    {
                        var queryTabelaSemRegime = @"
                            SELECT cd_tabela_preco
                            FROM T_TABELA_PRECO p
                            WHERE cd_pessoa_escola = @cd_escola
                              AND cd_curso = @cd_curso
                              AND cd_duracao = @cd_duracao
                              AND cd_regime IS NULL
                              AND dta_tabela_preco = (
                                  SELECT MAX(dta_tabela_preco)
                                  FROM T_TABELA_PRECO
                                  WHERE cd_pessoa_escola = p.cd_pessoa_escola
                                    AND cd_curso = p.cd_curso
                                    AND cd_duracao = p.cd_duracao
                                    AND cd_regime IS NULL
                                    AND dta_tabela_preco <= @dt_tabela
                              )";

                        using (var cmd = new SqlCommand(queryTabelaSemRegime, connection))
                        {
                            cmd.Parameters.AddWithValue("@cd_escola", cdEscola);
                            cmd.Parameters.AddWithValue("@cd_curso", cdCurso);
                            cmd.Parameters.AddWithValue("@cd_duracao", cdDuracao);
                            cmd.Parameters.AddWithValue("@dt_tabela", dtTabela);

                            var result = await cmd.ExecuteScalarAsync();
                            if (result != null && result != DBNull.Value)
                            {
                                cdTabela = Convert.ToInt32(result);
                            }
                        }
                    }

                    if (cdTabela == 0)
                    {
                        semTabelaC = true;
                    }

                    // Buscar preço do material
                    var queryItemCurso = "SELECT cd_item FROM T_ITEM_CURSO WHERE cd_curso = @cd_curso";
                    using (var cmd = new SqlCommand(queryItemCurso, connection))
                    {
                        cmd.Parameters.AddWithValue("@cd_curso", cdCurso);
                        var itemExists = await cmd.ExecuteScalarAsync();

                        if (itemExists != null && itemExists != DBNull.Value)
                        {
                            // Buscar item normal (id_ppt = 0)
                            var queryItemNormal = "SELECT cd_item FROM T_ITEM_CURSO WHERE cd_curso = @cd_curso AND id_ppt = 0";
                            using (var cmdItem = new SqlCommand(queryItemNormal, connection))
                            {
                                cmdItem.Parameters.AddWithValue("@cd_curso", cdCurso);
                                var itemResult = await cmdItem.ExecuteScalarAsync();
                                if (itemResult != null && itemResult != DBNull.Value)
                                {
                                    cdItem = Convert.ToInt32(itemResult);

                                    // Buscar preço do material normal
                                    var queryPrecoMaterial = @"
                                        SELECT vl_preco_tabela
                                        FROM T_TABELA_PRECO_MATERIAL
                                        WHERE cd_pessoa_escola = @cd_escola
                                          AND cd_item = @cd_item
                                          AND dt_tabela_preco_material = (
                                              SELECT MAX(dt_tabela_preco_material)
                                              FROM T_TABELA_PRECO_MATERIAL
                                              WHERE cd_pessoa_escola = @cd_escola
                                                AND cd_item = @cd_item
                                                AND dt_tabela_preco_material <= @dt_tabela
                                          )";

                                    using (var cmdPreco = new SqlCommand(queryPrecoMaterial, connection))
                                    {
                                        cmdPreco.Parameters.AddWithValue("@cd_escola", cdEscola);
                                        cmdPreco.Parameters.AddWithValue("@cd_item", cdItem);
                                        cmdPreco.Parameters.AddWithValue("@dt_tabela", dtTabela);

                                        var precoResult = await cmdPreco.ExecuteScalarAsync();
                                        if (precoResult != null && precoResult != DBNull.Value)
                                        {
                                            vlPrecoMaterial = Convert.ToDecimal(precoResult);
                                        }
                                        else
                                        {
                                            semTabelaM = true;
                                        }
                                    }
                                }
                            }

                            // Se regime = 2, buscar também item PPT (id_ppt = 1)
                            if (cdRegime == 2)
                            {
                                var queryItemPpt = "SELECT cd_item FROM T_ITEM_CURSO WHERE cd_curso = @cd_curso AND id_ppt = 1";
                                using (var cmdItemPpt = new SqlCommand(queryItemPpt, connection))
                                {
                                    cmdItemPpt.Parameters.AddWithValue("@cd_curso", cdCurso);
                                    var itemPptResult = await cmdItemPpt.ExecuteScalarAsync();
                                    if (itemPptResult != null && itemPptResult != DBNull.Value)
                                    {
                                        int cdItemPpt = Convert.ToInt32(itemPptResult);

                                        var queryPrecoMaterialPpt = @"
                                            SELECT vl_preco_tabela
                                            FROM T_TABELA_PRECO_MATERIAL
                                            WHERE cd_pessoa_escola = @cd_escola
                                              AND cd_item = @cd_item
                                              AND dt_tabela_preco_material = (
                                                  SELECT MAX(dt_tabela_preco_material)
                                                  FROM T_TABELA_PRECO_MATERIAL
                                                  WHERE cd_pessoa_escola = @cd_escola
                                                    AND cd_item = @cd_item
                                                    AND dt_tabela_preco_material <= @dt_tabela
                                              )";

                                        using (var cmdPrecoPpt = new SqlCommand(queryPrecoMaterialPpt, connection))
                                        {
                                            cmdPrecoPpt.Parameters.AddWithValue("@cd_escola", cdEscola);
                                            cmdPrecoPpt.Parameters.AddWithValue("@cd_item", cdItemPpt);
                                            cmdPrecoPpt.Parameters.AddWithValue("@dt_tabela", dtTabela);

                                            var precoPptResult = await cmdPrecoPpt.ExecuteScalarAsync();
                                            if (precoPptResult != null && precoPptResult != DBNull.Value)
                                            {
                                                vlPrecoMaterialPpt = Convert.ToDecimal(precoPptResult);
                                            }
                                            else
                                            {
                                                semTabelaM = true;
                                            }
                                        }
                                    }
                                }
                                vlPrecoMaterial += vlPrecoMaterialPpt;
                            }
                        }
                        else
                        {
                            semTabelaM = true;
                        }
                    }

                    if (semTabelaM) vlPrecoMaterial = 0;

                    // Validações de erro
                    if (semTabelaC && semTabelaM)
                    {
                        errMsg = "Tabela de preço do curso e material não encontradas com os filtros passados.";
                        response.Succeeded = true;
                        response.Message = errMsg;
                        response.TotalRecords = 0;
                        return response;
                    }

                    if (semTabelaC)
                        errMsg += " Tabela de preço de material encontrada, mas de curso não.";
                    else if (semTabelaM)
                        errMsg = "Tabela de preço encontrada, mas material não.";
                    else
                        errMsg = "Tabelas de preços encontradas para o curso e material.";

                    // Buscar dados da tabela de preço
                    if (!semTabelaC)
                    {
                        var queryDadosTabela = @"
                            SELECT cd_tabela_preco, cd_curso, cd_duracao, cd_regime, dta_tabela_preco,
                                   nm_parcelas, vl_parcela, vl_matricula, cd_pessoa_escola, vl_aula
                            FROM T_TABELA_PRECO
                            WHERE cd_tabela_preco = @cd_tabela";

                        using (var cmd = new SqlCommand(queryDadosTabela, connection))
                        {
                            cmd.Parameters.AddWithValue("@cd_tabela", cdTabela);

                            using (var reader = await cmd.ExecuteReaderAsync())
                            {
                                if (await reader.ReadAsync())
                                {
                                    var item = new TabelaCursoData
                                    {
                                        CdTabelaPreco = reader.IsDBNull("cd_tabela_preco") ? (int?)null : reader.GetInt32("cd_tabela_preco"),
                                        CdCurso = reader.IsDBNull("cd_curso") ? (int?)null : reader.GetInt32("cd_curso"),
                                        CdDuracao = reader.IsDBNull("cd_duracao") ? (int?)null : reader.GetInt32("cd_duracao"),
                                        CdRegime = reader.IsDBNull("cd_regime") ? (int?)null : reader.GetInt32("cd_regime"),
                                        DtaTabelaPreco = reader.IsDBNull("dta_tabela_preco") ? (DateTime?)null : reader.GetDateTime("dta_tabela_preco"),
                                        NmParcelas = reader.IsDBNull("nm_parcelas") ? 0 : reader.GetInt32("nm_parcelas"),
                                        VlParcela = reader.IsDBNull("vl_parcela") ? 0 : reader.GetDecimal("vl_parcela"),
                                        VlMatricula = reader.IsDBNull("vl_matricula") ? 0 : reader.GetDecimal("vl_matricula"),
                                        CdPessoaEscola = reader.IsDBNull("cd_pessoa_escola") ? (int?)null : reader.GetInt32("cd_pessoa_escola"),
                                        VlAula = reader.IsDBNull("vl_aula") ? 0 : reader.GetDecimal("vl_aula"),
                                        VlPrecoMaterial = vlPrecoMaterial
                                    };
                                    response.Data.Add(item);
                                }
                            }
                        }
                    }
                    else
                    {
                        // Caso sem tabela de curso, mas com material
                        var item = new TabelaCursoData
                        {
                            CdTabelaPreco = null,
                            CdCurso = null,
                            CdDuracao = null,
                            CdRegime = null,
                            DtaTabelaPreco = null,
                            NmParcelas = 0,
                            VlParcela = 0,
                            VlMatricula = 0,
                            CdPessoaEscola = null,
                            VlAula = 0,
                            VlPrecoMaterial = vlPrecoMaterial
                        };
                        response.Data.Add(item);
                    }

                    response.TotalRecords = 1;
                    response.Message = errMsg;
                    response.Succeeded = true;
                }
                catch (Exception ex)
                {
                    response.Succeeded = false;
                    response.Message = $"Erro ao executar consulta: {ex.Message}";
                    response.TotalRecords = 0;
                    response.Data = new List<TabelaCursoData>();
                }

                return response;
            }
        }

        private (Source source, bool success, string error) ValidarFonteDados()
        {
            var schemaName = "TabelaPreco";

            var schema = _schemaRepository.GetSchemaByField("name", schemaName);
            var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
            var source = _sourceRepository.GetByField("description", schemaModel.Source);

            if (source == null || source.Active != true)
            {
                return (null, false, "Fonte de dados não configurada ou inativa.");
            }

            return (source, true, null);
        }

        private (bool success, string error) ValidarModelState()
        {
            if (!ModelState.IsValid)
            {
                var errors = new List<string>();
                foreach (var modelError in ModelState.Values)
                {
                    foreach (var error in modelError.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                return (false, string.Join(", ", errors));
            }
            return (true, null);
        }
    }
}
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Simjob.Framework.Application.Controllers;
using Simjob.Framework.Domain.Core.Bus;
using Simjob.Framework.Domain.Core.Notifications;
using Simjob.Framework.Domain.Interfaces.Repositories;
using Simjob.Framework.Infra.Data.Context;
using Simjob.Framework.Infra.Schemas.Entities;
using Simjob.Framework.Services.Api.Modules.BibliotecaModule.Services;
using Simjob.Framework.Services.Api.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Simjob.Framework.Domain.Core.Utils;
using Simjob.Framework.Infra.Identity.Contexts;
using Simjob.Framework.Infra.Identity.Entities;



namespace Simjob.Framework.Services.Api.Controllers
{
  /// <summary>
  /// Controller para gestão de biblioteca e empréstimos
  /// </summary>
  public class BibliotecaController : BaseController
  {
    private readonly IRepository<SourceContext, Source> _sourceRepository;
    private readonly IRepository<MongoDbContext, Schema> _schemaRepository;
    private readonly BibliotecaService _bibliotecaService;

    public BibliotecaController(
        IMediatorHandler bus,
        INotificationHandler<DomainNotification> notifications,
        IRepository<SourceContext, Source> sourceRepository,
        IRepository<MongoDbContext, Schema> schemaRepository) : base(bus, notifications)
    {
      _sourceRepository = sourceRepository;
      _schemaRepository = schemaRepository;
      _bibliotecaService = new BibliotecaService();
    }

    private (Source source, bool valid) GetSource()
    {
      var schemaName = "T_Biblioteca";
      if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");

      var schema = _schemaRepository.GetSchemaByField("name", schemaName);
      if (schema == null)
      {
        return (null, false);
      }

      var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
      var source = _sourceRepository.GetByField("description", schemaModel.Source);

      if (source != null && source.Active != null && source.Active == true)
      {
        return (source, true);
      }

      return (null, false);
    }

    /// <summary>
    /// Busca pessoas para empréstimo na biblioteca
    /// </summary>
    [Authorize]
    [HttpGet("pessoaBiblioteca")]
    public async Task<IActionResult> GetPessoaBibliotecaSearch(
        [FromQuery] string nome = "",
        [FromQuery] string apelido = "",
        [FromQuery] bool inicio = false,
        [FromQuery] int tipoPessoa = 0,
        [FromQuery] string cnpjCpf = "",
        [FromQuery] int sexo = 0,
        [FromQuery] int cd_empresa = 0,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 20,
        [FromQuery] string sort = "no_pessoa",
        [FromQuery] string sortDirection = "asc")
    {
      try
      {
        var accessToken = Request.Headers[HeaderNames.Authorization];
        var tokenInfo = Util.GetUserInfoFromToken(accessToken);

        if (cd_empresa == 0 && tokenInfo.ContainsKey("cd_pessoa_escola"))
        {
          cd_empresa = int.Parse(tokenInfo["cd_pessoa_escola"]);
        }

        var (source, valid) = GetSource();
        if (!valid)
        {
          return BadRequest(new { error = "Fonte de dados não configurada ou inativa." });
        }

        var results = await _bibliotecaService.GetPessoaBibliotecaSearch(
            source, nome ?? "", apelido ?? "", inicio, tipoPessoa,
            cnpjCpf ?? "", sexo, cd_empresa, skip, take, sort, sortDirection);

        return ResponseDefault(new
        {
          success = true,
          data = results,
          total = results.Count
        });
      }
      catch (Exception ex)
      {
        return BadRequest(new { error = ex.Message });
      }
    }

    /// <summary>
    /// Busca empréstimos com filtros
    /// </summary>
    [Authorize]
    [HttpGet("emprestimos")]
    public async Task<IActionResult> GetEmprestimoSearch(
        [FromQuery] int? cd_pessoa = null,
        [FromQuery] int? cd_item = null,
        [FromQuery] bool? pendentes = null,
        [FromQuery] string dt_inicial = null,
        [FromQuery] string dt_final = null,
        [FromQuery] bool? emprestimos = null,
        [FromQuery] bool? devolucao = null,
        [FromQuery] int cd_empresa = 0,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 20,
        [FromQuery] string sort = "dt_emprestimo",
        [FromQuery] string sortDirection = "desc")
    {
      try
      {
        var accessToken = Request.Headers[HeaderNames.Authorization];
        var tokenInfo = Util.GetUserInfoFromToken(accessToken);

        if (cd_empresa == 0 && tokenInfo.ContainsKey("cd_pessoa_escola"))
        {
          cd_empresa = int.Parse(tokenInfo["cd_pessoa_escola"]);
        }

        var (source, valid) = GetSource();
        if (!valid)
        {
          return BadRequest(new { error = "Fonte de dados não configurada ou inativa." });
        }

        DateTime? dtInicial = null;
        DateTime? dtFinal = null;

        if (!string.IsNullOrEmpty(dt_inicial))
        {
          dtInicial = DateTime.Parse(dt_inicial);
        }

        if (!string.IsNullOrEmpty(dt_final))
        {
          dtFinal = DateTime.Parse(dt_final);
        }

        var results = await _bibliotecaService.GetEmprestimoSearch(
            source, cd_pessoa, cd_item, pendentes, dtInicial, dtFinal,
            emprestimos, devolucao, cd_empresa, skip, take, sort, sortDirection);

        return ResponseDefault(new
        {
          success = true,
          data = results,
          total = results.Count
        });
      }
      catch (Exception ex)
      {
        return BadRequest(new { error = ex.Message });
      }
    }

    /// <summary>
    /// Busca um empréstimo específico por ID
    /// </summary>
    [Authorize]
    [HttpGet("{cd_biblioteca}")]
    public async Task<IActionResult> GetEmprestimo(
        int cd_biblioteca,
        [FromQuery] int cd_empresa = 0)
    {
      try
      {
        var accessToken = Request.Headers[HeaderNames.Authorization];
        var tokenInfo = Util.GetUserInfoFromToken(accessToken);

        if (cd_empresa == 0 && tokenInfo.ContainsKey("cd_pessoa_escola"))
        {
          cd_empresa = int.Parse(tokenInfo["cd_pessoa_escola"]);
        }

        var (source, valid) = GetSource();
        if (!valid)
        {
          return BadRequest(new { error = "Fonte de dados não configurada ou inativa." });
        }

        var result = await _bibliotecaService.GetEmprestimo(source, cd_biblioteca, cd_empresa);

        if (result == null)
        {
          return NotFound(new { error = "Empréstimo não encontrado" });
        }

        return ResponseDefault(new
        {
          success = true,
          data = result
        });
      }
      catch (Exception ex)
      {
        return BadRequest(new { error = ex.Message });
      }
    }

    /// <summary>
    /// Cria um novo empréstimo
    /// </summary>
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Dictionary<string, object> emprestimo)
    {
      try
      {
        var accessToken = Request.Headers[HeaderNames.Authorization];
        var tokenInfo = Util.GetUserInfoFromToken(accessToken);

        int cd_empresa = 0;
        if (tokenInfo.ContainsKey("cd_pessoa_escola"))
        {
          cd_empresa = int.Parse(tokenInfo["cd_pessoa_escola"]);
        }

        var (source, valid) = GetSource();
        if (!valid)
        {
          return BadRequest(new { error = "Fonte de dados não configurada ou inativa." });
        }

        // Validações
        if (!emprestimo.ContainsKey("cd_pessoa") || !emprestimo.ContainsKey("cd_item"))
        {
          return BadRequest(new { error = "cd_pessoa e cd_item são obrigatórios" });
        }

        if (!emprestimo.ContainsKey("dt_emprestimo"))
        {
          emprestimo["dt_emprestimo"] = DateTime.Now;
        }

        if (!emprestimo.ContainsKey("dt_prevista_devolucao"))
        {
          return BadRequest(new { error = "dt_prevista_devolucao é obrigatório" });
        }

        // Buscar saldo do item
        var cd_item = Convert.ToInt32(emprestimo["cd_item"]);
        var filtroItem = new List<(string, object)>
                {
                    ("cd_item", cd_item),
                    ("cd_pessoa_escola", cd_empresa)
                };

        var itemEscola = await SQLServerService.GetFirstByFields(source, "T_ITEM_ESCOLA", filtroItem);
        if (itemEscola == null)
        {
          return BadRequest(new { error = "Item não encontrado na escola" });
        }

        int saldo = Convert.ToInt32(itemEscola["qt_estoque"]);

        // Buscar dados da pessoa e item para mensagens
        var filtroPessoa = new List<(string, object)> { ("cd_pessoa", emprestimo["cd_pessoa"]) };
        var pessoa = await SQLServerService.GetFirstByFields(source, "T_PESSOA", filtroPessoa);
        emprestimo["no_pessoa"] = pessoa != null ? pessoa["no_pessoa"].ToString() : "";

        var filtroItemNome = new List<(string, object)> { ("cd_item", cd_item) };
        var item = await SQLServerService.GetFirstByFields(source, "T_ITEM", filtroItemNome);
        emprestimo["no_item"] = item != null ? item["no_item"].ToString() : "";

        var (success, error, cd_biblioteca) = await _bibliotecaService.AddEmprestimo(
            source, emprestimo, cd_empresa, saldo);

        if (!success)
        {
          return BadRequest(new { error = error });
        }

        return ResponseDefault(new
        {
          success = true,
          message = "Empréstimo cadastrado com sucesso",
          cd_biblioteca = cd_biblioteca
        });
      }
      catch (Exception ex)
      {
        return BadRequest(new { error = ex.Message });
      }
    }

    /// <summary>
    /// Atualiza um empréstimo (principalmente para registrar devolução)
    /// </summary>
    [Authorize]
    [HttpPut("{cd_biblioteca}")]
    public async Task<IActionResult> Put(int cd_biblioteca, [FromBody] Dictionary<string, object> emprestimo)
    {
      try
      {
        var accessToken = Request.Headers[HeaderNames.Authorization];
        var tokenInfo = Util.GetUserInfoFromToken(accessToken);

        int cd_empresa = 0;
        if (tokenInfo.ContainsKey("cd_pessoa_escola"))
        {
          cd_empresa = int.Parse(tokenInfo["cd_pessoa_escola"]);
        }

        var (source, valid) = GetSource();
        if (!valid)
        {
          return BadRequest(new { error = "Fonte de dados não configurada ou inativa." });
        }

        emprestimo["cd_biblioteca"] = cd_biblioteca;

        var (success, error) = await _bibliotecaService.EditEmprestimo(source, emprestimo, cd_empresa);

        if (!success)
        {
          return BadRequest(new { error = error });
        }

        return ResponseDefault(new
        {
          success = true,
          message = "Empréstimo atualizado com sucesso"
        });
      }
      catch (Exception ex)
      {
        return BadRequest(new { error = ex.Message });
      }
    }

    /// <summary>
    /// Exclui um empréstimo
    /// </summary>
    [Authorize]
    [HttpDelete("{cd_biblioteca}")]
    public async Task<IActionResult> Delete(int cd_biblioteca, [FromQuery] int cd_empresa = 0)
    {
      try
      {
        var accessToken = Request.Headers[HeaderNames.Authorization];
        var tokenInfo = Util.GetUserInfoFromToken(accessToken);

        if (cd_empresa == 0 && tokenInfo.ContainsKey("cd_pessoa_escola"))
        {
          cd_empresa = int.Parse(tokenInfo["cd_pessoa_escola"]);
        }

        var (source, valid) = GetSource();
        if (!valid)
        {
          return BadRequest(new { error = "Fonte de dados não configurada ou inativa." });
        }

        var (success, error) = await _bibliotecaService.DeleteEmprestimo(source, cd_biblioteca, cd_empresa);

        if (!success)
        {
          return BadRequest(new { error = error });
        }

        return ResponseDefault(new
        {
          success = true,
          message = "Empréstimo excluído com sucesso"
        });
      }
      catch (Exception ex)
      {
        return BadRequest(new { error = ex.Message });
      }
    }
  }
}

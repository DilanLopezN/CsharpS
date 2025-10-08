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
using Simjob.Framework.Services.Api.Models.Turmas;
using Simjob.Framework.Services.Api.Modules.TurmaModule.Services;
using System;
using System.Threading.Tasks;

namespace Simjob.Framework.Services.Api.Controllers
{
  /// <summary>
  /// Controller responsável pelos endpoints de Turma
  /// Segue padrão modular similar ao NestJS
  /// </summary>
  public class TurmaController : BaseController
  {
    private readonly IRepository<SourceContext, Source> _sourceRepository;
    private readonly IRepository<MongoDbContext, Schema> _schemaRepository;
    private readonly TurmaService _turmaService;
    private readonly ProfessorTurmaService _professorTurmaService;
    private readonly AlunoTurmaService _alunoTurmaService;
    private readonly ProgramacaoTurmaService _programacaoTurmaService;

    public TurmaController(
      IMediatorHandler bus,
      INotificationHandler<DomainNotification> notifications,
      IRepository<SourceContext, Source> sourceRepository,
      IRepository<MongoDbContext, Schema> schemaRepository) : base(bus, notifications)
    {
      _sourceRepository = sourceRepository;
      _schemaRepository = schemaRepository;
      _turmaService = new TurmaService();
      _professorTurmaService = new ProfessorTurmaService();
      _alunoTurmaService = new AlunoTurmaService();
      _programacaoTurmaService = new ProgramacaoTurmaService();
    }

    #region Métodos Auxiliares

    private (Source source, bool valid) GetSource()
    {
      var schemaName = "T_Turma";
      if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");

      var schema = _schemaRepository.GetSchemaByField("name", schemaName);
      var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
      var source = _sourceRepository.GetByField("description", schemaModel.Source);

      bool valid = source != null && source.Active != null && source.Active == true;
      return (source, valid);
    }

    #endregion

    #region CRUD Básico - Turma

    /// <summary>
    /// Criar nova turma
    /// </summary>
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Insert([FromBody] InsertTurmaModel command)
    {
      var (source, valid) = GetSource();
      if (!valid)
        return BadRequest(new { error = "Fonte de dados não configurada ou inativa." });

      var resultado = await _turmaService.CriarTurma(command, source);

      if (!resultado.success)
        return BadRequest(new { success = false, error = resultado.error });

      return ResponseDefault(new
      {
        success = true,
        cd_turma = resultado.cdTurma,
        message = "Turma criada com sucesso"
      });
    }

    /// <summary>
    /// Atualizar turma existente
    /// </summary>
    [Authorize]
    [HttpPut("{cd_turma}")]
    public async Task<IActionResult> Update([FromBody] InsertTurmaModel command, int cd_turma)
    {
      var (source, valid) = GetSource();
      if (!valid)
        return BadRequest(new { error = "Fonte de dados não configurada ou inativa." });

      var resultado = await _turmaService.AtualizarTurma(cd_turma, command, source);

      if (!resultado.success)
        return BadRequest(new { success = false, error = resultado.error });

      return ResponseDefault(new
      {
        success = true,
        message = "Turma atualizada com sucesso"
      });
    }

    /// <summary>
    /// Listar turmas com filtros
    /// </summary>
    [Authorize]
    [HttpGet()]
    public async Task<IActionResult> GetAll(
       string value,
       SearchModeEnum mode,
       int? page,
       int? limit,
       string sortField,
       bool sortDesc = false,
       string ids = "",
       string searchFields = null,
       string? cd_empresa = null,
       bool filtrarProgramacao = false,
       DateTime? dataInicio = null,
       DateTime? dataFim = null,
       int? professorId = null,
       string? horario = null)
    {

      var schemaName = "T_Turma";
      if (schemaName.Contains("T_"))
        schemaName = schemaName.Replace("T_", "");

      var schema = _schemaRepository.GetSchemaByField("name", schemaName);
      var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
      var source = _sourceRepository.GetByField("description", schemaModel.Source);

      if (source != null && source.Active != null && source.Active == true)
      {
        // ✅ CHAMA O REPOSITORY
        var resultado = await _turmaService.BuscarTurmas(
            value,
            mode,
            page,
            limit,
            sortField,
            sortDesc,
            ids,
            searchFields,
            cd_empresa,
            source,
            filtrarProgramacao,
            dataInicio,
            dataFim,
            professorId,
            horario
        );

        if (!resultado.success)
        {
          return BadRequest(new
          {
            sucess = false,
            error = resultado.error
          });
        }

        return ResponseDefault(resultado.data);
      }

      return BadRequest(new
      {
        error = "Fonte de dados não configurada ou inativa."
      });
    }

    #endregion

    #region Professores

    /// <summary>
    /// Listar todos os professores
    /// </summary>
    [Authorize]
    [HttpGet("Professores")]
    public async Task<IActionResult> GetAllProfessores(
      string value,
      SearchModeEnum mode,
      int? page,
      int? limit,
      string sortField,
      bool sortDesc = false,
      string ids = "",
      string searchFields = null,
      string cd_empresa = null)
    {
      if (cd_empresa == null)
        return BadRequest("campo cd_empresa não informado");

      var (source, valid) = GetSource();
      if (!valid)
        return BadRequest(new { error = "Fonte de dados não configurada ou inativa." });

      var resultado = await _professorTurmaService.BuscarProfessores(
        value, mode, page, limit, sortField, sortDesc, ids, searchFields, cd_empresa, source);

      if (!resultado.success)
        return BadRequest(new { success = false, error = resultado.error });

      return ResponseDefault(new
      {
        data = resultado.data,
        total = resultado.total,
        page,
        limit,
        pages = limit != null ? (int)Math.Ceiling((double)resultado.total / limit.Value) : 0
      });
    }

    /// <summary>
    /// Buscar disponibilidade de professores
    /// </summary>
    [Authorize]
    [HttpGet("Professores/Disponibilidade")]
    public async Task<IActionResult> GetDisponibilidadeProfessores(
      string cd_empresa,
      DateTime data)
    {
      if (string.IsNullOrEmpty(cd_empresa))
        return BadRequest(new { error = "campo cd_empresa não informado" });

      var (source, valid) = GetSource();
      if (!valid)
        return BadRequest(new { error = "Fonte de dados não configurada ou inativa." });

      var resultado = await _professorTurmaService.BuscarDisponibilidadeProfessores(cd_empresa, data, source);

      return ResponseDefault(resultado);
    }

    #endregion

    #region Alunos

    /// <summary>
    /// Listar alunos de uma turma
    /// </summary>
    [Authorize]
    [HttpGet("AlunosByTurma")]
    public async Task<IActionResult> GetAllAlunosByTurma(string cd_turma = null)
    {
      if (cd_turma == null)
        return BadRequest("campo cd_turma não informado");

      var (source, valid) = GetSource();
      if (!valid)
        return BadRequest(new { error = "Fonte de dados não configurada ou inativa." });

      var resultado = await _alunoTurmaService.BuscarAlunosPorTurma(cd_turma, source);

      if (!resultado.success)
        return BadRequest(new { success = false, error = resultado.error });

      return ResponseDefault(new
      {
        data = resultado.data,
        total = resultado.total
      });
    }

    /// <summary>
    /// Inserir aluno em pré-turma
    /// </summary>
    [Authorize]
    [HttpPost("PreTurma")]
    public async Task<IActionResult> InsertAlunoTurma([FromBody] InsertAlunoTurmaPreTurma model)
    {
      var (source, valid) = GetSource();
      if (!valid)
        return BadRequest(new { error = "Fonte de dados não configurada ou inativa." });

      var resultado = await _alunoTurmaService.InserirAlunoPreTurma(model, source);

      if (!resultado.success)
        return BadRequest(new { error = resultado.error });

      return ResponseDefault(new { success = true, message = "Aluno inserido com sucesso" });
    }

    #endregion

    #region Programações

    /// <summary>
    /// Buscar próxima programação de uma turma
    /// </summary>
    [Authorize]
    [HttpGet("programacao/{cd_turma}")]
    public async Task<IActionResult> GetProgramacao(int cd_turma)
    {
      var (source, valid) = GetSource();
      if (!valid)
        return BadRequest(new { error = "Fonte de dados não configurada ou inativa." });

      var resultado = await _turmaService.BuscarProximaProgramacao(cd_turma, source);

      if (!resultado.success)
        return NotFound(new { error = resultado.error });

      return ResponseDefault(resultado.data);
    }

    /// <summary>
    /// Buscar programação por ID
    /// </summary>
    [Authorize]
    [HttpGet("programacao")]
    public async Task<IActionResult> GetProgramacaoId(int cd_programacao_turma)
    {
      var (source, valid) = GetSource();
      if (!valid)
        return BadRequest(new { error = "Fonte de dados não configurada ou inativa." });

      var resultado = await _programacaoTurmaService.BuscarProgramacaoPorId(cd_programacao_turma, source);

      if (!resultado.success)
        return NotFound(new { error = resultado.error });

      return ResponseDefault(resultado.data);
    }

    /// <summary>
    /// Listar todas programações de uma turma com informações enriquecidas
    /// </summary>
    [Authorize]
    [HttpGet("programacoes/{cd_turma}")]
    public async Task<IActionResult> ListarProgramacoes(int cd_turma)
    {
      var (source, valid) = GetSource();
      if (!valid)
        return BadRequest(new { error = "Fonte de dados não configurada ou inativa." });

      var resultado = await _programacaoTurmaService.ListarProgramacoesEnriquecidas(cd_turma, source);

      if (!resultado.success)
        return NotFound(new { error = resultado.error });

      return ResponseDefault(resultado.data);
    }

    /// <summary>
    /// Gerar programação automática para turma
    /// Implementa regra 3.2 (prioridade: Matriz → Franqueado → Manual)
    /// </summary>
    [Authorize]
    [HttpPost("gerar-programacao")]
    public async Task<IActionResult> GerarProgramacao([FromBody] GerarProgramacaoRequest request)
    {
      var (source, valid) = GetSource();
      if (!valid)
        return BadRequest(new { error = "Fonte de dados não configurada ou inativa." });

      var resultado = await _programacaoTurmaService.GerarProgramacaoAutomatica(request, source);

      if (!resultado.success)
        return BadRequest(new { error = resultado.error });

      return ResponseDefault(new
      {
        programacoes = resultado.programacoes,
        total_aulas = resultado.total_aulas,
        carga_horaria_total = resultado.carga_horaria_total
      });
    }

    /// <summary>
    /// Busca programação da matriz (fundação)
    /// </summary>
    [Authorize]
    [HttpGet("programacao-matriz")]
    public async Task<IActionResult> GetProgramacaoMatriz(
      int cd_curso,
      int cd_duracao,
      int? cd_escola = null)
    {
      var (source, valid) = GetSource();
      if (!valid)
        return BadRequest(new { error = "Fonte de dados não configurada ou inativa." });

      var resultado = await _programacaoTurmaService.BuscarProgramacaoMatriz(
        cd_curso, cd_duracao, cd_escola, source);

      if (!resultado.success)
        return NotFound(new { error = resultado.error });

      return ResponseDefault(resultado.data);
    }

    /// <summary>
    /// Busca feriados do ano
    /// </summary>
    [Authorize]
    [HttpGet("feriados")]
    public async Task<IActionResult> GetFeriados(int ano, int? cd_escola = null)
    {
      var (source, valid) = GetSource();
      if (!valid)
        return BadRequest(new { error = "Fonte de dados não configurada ou inativa." });

      var resultado = await _programacaoTurmaService.BuscarFeriados(ano, cd_escola, source);

      if (!resultado.success)
        return NotFound(new { error = resultado.error });

      return ResponseDefault(resultado.data);
    }

    #endregion
  }
}

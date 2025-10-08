using Microsoft.IdentityModel.Tokens;
using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Services.Api.Enums;
using Simjob.Framework.Services.Api.Models.Turmas;
using Simjob.Framework.Services.Api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Simjob.Framework.Services.Api.Modules.TurmaModule.Services
{
  /// <summary>
  /// Serviço responsável pela lógica de negócio de Turmas
  /// Seguindo padrão de módulos similar ao NestJS
  /// </summary>
  public class TurmaService
  {
    #region Constantes
    private const string SCHEMA_NAME = "T_Turma";
    private const int ORIGEM_HORARIO_TURMA = 19;
    private const string FONTE_DADOS_INATIVA = "Fonte de dados não configurada ou inativa.";
    #endregion

    #region Validação e Criação de Turma

    /// <summary>
    /// Valida e cria uma nova turma no sistema
    /// </summary>
    public async Task<(bool success, string error, int? cdTurma)> CriarTurma(
      InsertTurmaModel command,
      Source source)
    {
      try
      {
        // 1. Criar registro da turma
        var cdTurmaId = await InserirTurmaBase(command, source);
        if (cdTurmaId == 0)
          return (false, "Erro ao criar turma", null);

        // 2. Processar horários
        var horarioIds = await ProcessarHorarios(command, cdTurmaId, source);
        if (horarioIds == null)
          return (false, "Erro ao processar horários", null);

        // 3. Processar professores
        if (!command.ProfessoresTurma.IsNullOrEmpty())
        {
          var sucessoProfessores = await ProcessarProfessores(command, cdTurmaId, horarioIds, source);
          if (!sucessoProfessores)
            return (false, "Erro ao processar professores", null);
        }

        // 4. Processar alunos
        if (!command.AlunosTurma.IsNullOrEmpty())
        {
          var sucessoAlunos = await ProcessarAlunos(command, cdTurmaId, source);
          if (!sucessoAlunos)
            return (false, "Erro ao processar alunos", null);
        }

        // 5. Processar programações
        if (!command.ProgramacaoTurma.IsNullOrEmpty())
        {
          var sucessoProgramacao = await ProcessarProgramacoes(command, cdTurmaId, source);
          if (!sucessoProgramacao)
            return (false, "Erro ao processar programações", null);
        }

        // 6. Processar feriados desconsiderados
        if (command.FeriadoDesconsiderado != null)
        {
          var sucessoFeriado = await ProcessarFeriadoDesconsiderado(command, cdTurmaId, source);
          if (!sucessoFeriado)
            return (false, "Erro ao processar feriado desconsiderado", null);
        }

        // 7. Atualizar nome da turma
        var nomeAtualizado = await AtualizarNomeTurma(cdTurmaId, command.cd_pessoa_escola, source);
        if (!nomeAtualizado)
          return (false, "Erro ao atualizar nome da turma", null);

        return (true, string.Empty, cdTurmaId);
      }
      catch (Exception ex)
      {
        return (false, $"Erro ao criar turma: {ex.Message}", null);
      }
    }

    /// <summary>
    /// Atualiza uma turma existente
    /// </summary>
    public async Task<(bool success, string error)> AtualizarTurma(
      int cdTurma,
      InsertTurmaModel command,
      Source source)
    {
      try
      {
        // 1. Atualizar dados base da turma
        var sucessoUpdate = await AtualizarTurmaBase(cdTurma, command, source);
        if (!sucessoUpdate)
          return (false, "Erro ao atualizar dados da turma");

        // 2. Remover horários antigos
        var horariosAntigos = await BuscarHorariosAntigos(cdTurma, source);

        // 3. Processar novos horários
        var horarioIds = await ProcessarHorarios(command, cdTurma, source);
        if (horarioIds == null)
          return (false, "Erro ao processar horários");

        // 4. Deletar horários antigos
        if (horariosAntigos != null && command.Horarios != null)
        {
          await DeletarHorariosAntigos(horariosAntigos, source);
        }

        // 5. Atualizar professores
        await SQLServerService.Delete("T_PROFESSOR_TURMA", "cd_turma", cdTurma.ToString(), source);
        await SQLServerService.Delete("T_HORARIO_PROFESSOR_TURMA", "cd_turma", cdTurma.ToString(), source);

        if (!command.ProfessoresTurma.IsNullOrEmpty())
        {
          var sucessoProfessores = await ProcessarProfessores(command, cdTurma, horarioIds, source);
          if (!sucessoProfessores)
            return (false, "Erro ao processar professores");
        }

        return (true, string.Empty);
      }
      catch (Exception ex)
      {
        return (false, $"Erro ao atualizar turma: {ex.Message}");
      }
    }

    #endregion

    #region Métodos Privados - Turma Base

    private async Task<int> InserirTurmaBase(InsertTurmaModel command, Source source)
    {
      var turmaDict = ConstruirDicionarioTurma(command);
      var resultado = await SQLServerService.InsertWithResult("T_TURMA", turmaDict, source);

      if (!resultado.success || resultado.inserted == null)
        return 0;

      return (int)resultado.inserted["cd_turma"];
    }

    private async Task<bool> AtualizarTurmaBase(int cdTurma, InsertTurmaModel command, Source source)
    {
      var turmaDict = ConstruirDicionarioTurma(command);
      var resultado = await SQLServerService.Update("T_TURMA", turmaDict, source, "cd_turma", cdTurma);
      return resultado.success;
    }

    private Dictionary<string, object> ConstruirDicionarioTurma(InsertTurmaModel command)
    {
      var dict = new Dictionary<string, object>
      {
        { "cd_turma_ppt", command.cd_turma_ppt ?? null },
        { "no_turma", Guid.NewGuid().ToString() },
        { "cd_pessoa_escola", command.cd_pessoa_escola },
        { "id_turma_ativa", command.id_turma_ativa },
        { "cd_sala", command.cd_sala ?? null },
        { "cd_duracao", command.cd_duracao },
        { "cd_regime", command.cd_regime },
        { "dt_inicio_aula", command.dt_inicio_aula },
        { "dt_final_aula", command.dt_final_aula },
        { "id_aula_externa", command.id_aula_externa },
        { "nro_aulas_programadas", command.nro_aulas_programadas },
        { "id_turma_ppt", command.id_turma_ppt },
        { "cd_produto", command.cd_produto },
        { "nm_turma", command.nm_turma },
        { "dt_termino_turma", command.dt_termino_turma },
        { "no_apelido", command.no_apelido },
        { "cd_turma_enc", command.cd_turma_enc ?? null },
        { "id_percentual_faltas", command.id_percentual_faltas },
        { "cd_sala_online", command.cd_sala_online ?? null },
        { "nm_carga_horaria_aula", command.nm_carga_horaria_aula ?? null }
      };

      if (command.cd_curso != null && command.cd_curso > 0)
        dict.Add("cd_curso", command.cd_curso);

      return dict;
    }

    #endregion

    #region Métodos Privados - Horários

    private async Task<List<int>> ProcessarHorarios(InsertTurmaModel command, int cdTurma, Source source)
    {
      var horarioIds = new List<int>();
      if (command.Horarios.IsNullOrEmpty()) return horarioIds;

      foreach (var horario in command.Horarios)
      {
        var horarioDict = new Dictionary<string, object>
        {
          { "cd_registro", cdTurma },
          { "cd_pessoa_escola", command.cd_pessoa_escola },
          { "id_dia_semana", horario.id_dia_semana },
          { "id_disponivel", horario.id_disponivel },
          { "id_origem", ORIGEM_HORARIO_TURMA },
          { "dt_hora_ini", horario.dt_hora_ini },
          { "dt_hora_fim", horario.dt_hora_fim }
        };

        var resultado = await SQLServerService.InsertWithResult("T_HORARIO", horarioDict, source);
        if (resultado.success && resultado.inserted != null)
          horarioIds.Add((int)resultado.inserted["cd_horario"]);
      }

      return horarioIds;
    }

    private async Task<List<Dictionary<string, object>>> BuscarHorariosAntigos(int cdTurma, Source source)
    {
      var resultado = await SQLServerService.GetListIn(
        "vi_horario_turma", 1, 10000000, "cd_turma", true,
        null, null, null, source, SearchModeEnum.Contains,
        null, null, "cd_turma", new List<string> { cdTurma.ToString() }
      );
      return resultado.success ? resultado.data : null;
    }

    private async Task DeletarHorariosAntigos(List<Dictionary<string, object>> horariosAntigos, Source source)
    {
      foreach (var horario in horariosAntigos)
        await SQLServerService.Delete("T_HORARIO", "cd_horario", horario["cd_horario"].ToString(), source);
    }

    #endregion

    #region Métodos Privados - Professores

    private async Task<bool> ProcessarProfessores(InsertTurmaModel command, int cdTurma, List<int> horarioIds, Source source)
    {
      if (command.ProfessoresTurma.IsNullOrEmpty() || horarioIds.IsNullOrEmpty())
        return true;

      // Inserir professores na turma
      foreach (var professor in command.ProfessoresTurma)
      {
        var professorDict = new Dictionary<string, object>
        {
          { "cd_turma", cdTurma },
          { "cd_professor", professor.cd_professor },
          { "id_professor_ativo", professor.id_professor_ativo }
        };

        var resultado = await SQLServerService.Insert("T_PROFESSOR_TURMA", professorDict, source);
        if (!resultado.success) return false;
      }

      // Vincular professores aos horários
      var professorIds = command.ProfessoresTurma.Select(x => x.cd_professor).Distinct().ToList();
      foreach (var professorId in professorIds)
      {
        foreach (var horarioId in horarioIds)
        {
          var vinculoDict = new Dictionary<string, object>
          {
            { "cd_horario", horarioId },
            { "cd_professor", professorId },
            { "cd_turma", cdTurma }
          };

          var resultado = await SQLServerService.Insert("T_HORARIO_PROFESSOR_TURMA", vinculoDict, source);
          if (!resultado.success) return false;
        }
      }

      return true;
    }

    #endregion

    #region Métodos Privados - Alunos

    private async Task<bool> ProcessarAlunos(InsertTurmaModel command, int cdTurma, Source source)
    {
      foreach (var aluno in command.AlunosTurma)
      {
        var alunoDict = new Dictionary<string, object>
        {
          { "cd_turma", cdTurma },
          { "cd_aluno", aluno.cd_aluno },
          { "cd_situacao_aluno_turma", aluno.cd_situacao_aluno_turma },
          { "cd_curso", aluno.cd_curso },
          { "dt_inicio", aluno.dt_inicio },
          { "dt_movimento", aluno.dt_movimento }
        };

        var resultado = await SQLServerService.Insert("T_ALUNO_TURMA", alunoDict, source);
        if (!resultado.success) return false;
      }
      return true;
    }

    #endregion

    #region Métodos Privados - Programações

    private async Task<bool> ProcessarProgramacoes(InsertTurmaModel command, int cdTurma, Source source)
    {
      await SQLServerService.Delete("T_PROGRAMACAO_TURMA", "cd_turma", cdTurma.ToString(), source);

      foreach (var programacao in command.ProgramacaoTurma)
      {
        var programacaoDict = new Dictionary<string, object>
        {
          { "cd_turma", cdTurma },
          { "nm_aula_programacao_turma", programacao.nm_aula_programacao_turma },
          { "dta_programacao_turma", programacao.dta_programacao_turma },
          { "dc_programacao_turma", programacao.dc_programacao_turma },
          { "hr_inicial_programacao", programacao.hr_inicial_programacao },
          { "hr_final_programacao", programacao.hr_final_programacao },
          { "nm_programacao_aux", programacao.nm_programacao_aux },
          { "id_aula_dada", programacao.id_aula_dada },
          { "id_programacao_manual", programacao.id_programacao_manual },
          { "id_reprogramada", programacao.id_reprogramada },
          { "id_provisoria", programacao.id_provisoria },
          { "cd_feriado", programacao.cd_feriado },
          { "id_mostrar_calendario", programacao.id_mostrar_calendario },
          { "dta_cadastro_programacao", programacao.dta_cadastro_programacao },
          { "nm_programacao_real", programacao.nm_programacao_real },
          { "id_prog_cancelada", programacao.id_prog_cancelada },
          { "id_modificada", programacao.id_modificada }
        };

        var resultado = await SQLServerService.Insert("T_PROGRAMACAO_TURMA", programacaoDict, source);
        if (!resultado.success) return false;
      }
      return true;
    }

    #endregion

    #region Métodos Privados - Feriados

    private async Task<bool> ProcessarFeriadoDesconsiderado(InsertTurmaModel command, int cdTurma, Source source)
    {
      await SQLServerService.Delete("T_FERIADO_DESCONSIDERADO", "cd_turma", cdTurma.ToString(), source);

      var feriadoDict = new Dictionary<string, object>
      {
        { "cd_turma", cdTurma },
        { "dt_inicial", command.FeriadoDesconsiderado.dt_inicial },
        { "dt_final", command.FeriadoDesconsiderado.dt_final },
        { "id_programacao_feriado", command.FeriadoDesconsiderado.id_programacao_feriado }
      };

      var resultado = await SQLServerService.Insert("T_FERIADO_DESCONSIDERADO", feriadoDict, source);
      return resultado.success;
    }

    #endregion

    #region Métodos Privados - Nome da Turma

    private async Task<bool> AtualizarNomeTurma(int cdTurma, int cdPessoaEscola, Source source)
    {
      try
      {
        var viewNomeTurma = await SQLServerService.GetFirstByFields(source, "vi_nome_turma",
          new List<(string campo, object valor)> { ("cd_turma", cdTurma), ("cd_pessoa_escola", cdPessoaEscola) });

        if (viewNomeTurma == null) return false;

        string nomeTurmaBase = viewNomeTurma["no_turma_formatado"]?.ToString() ?? "";
        if (string.IsNullOrEmpty(nomeTurmaBase)) return false;

        int numeroSequencial = 1;
        Dictionary<string, object> turmaExistente;

        do
        {
          turmaExistente = await SQLServerService.GetFirstByFields(source, "T_TURMA",
            new List<(string campo, object valor)>
            {
              ("no_turma", nomeTurmaBase + numeroSequencial),
              ("cd_pessoa_escola", cdPessoaEscola)
            });

          if (turmaExistente != null && (int)turmaExistente["cd_turma"] != cdTurma)
            numeroSequencial++;
          else
            break;

        } while (turmaExistente != null);

        var nomeCompleto = nomeTurmaBase + numeroSequencial;
        var turmaUpdate = new Dictionary<string, object>
        {
          { "nm_turma", numeroSequencial },
          { "no_turma", nomeCompleto }
        };

        var resultado = await SQLServerService.Update("T_TURMA", turmaUpdate, source, "cd_turma", cdTurma);
        return resultado.success;
      }
      catch { return false; }
    }

    #endregion

    public async Task<(bool success, object data, int total, int pages, string error)> BuscarTurmas(
      string value,
      SearchModeEnum mode,
      int? page,
      int? limit,
      string sortField,
      bool sortDesc,
      string ids,
      string searchFields,
      string cdEmpresa,
      Source source,
      bool filtrarProgramacao = false,
      DateTime? dataInicio = null,
      DateTime? dataFim = null,
      int? professorId = null,
      string horario = null)
    {
      try
      {
        if (string.IsNullOrEmpty(sortField)) sortField = "cd_turma";

        // 1. Adicionar filtro de programação se necessário
        if (filtrarProgramacao)
        {
          if (string.IsNullOrEmpty(searchFields) && string.IsNullOrEmpty(value))
          {
            searchFields = "[possui_programacao]";
            value = "[1]";
          }
          else
          {
            searchFields = searchFields + ",[possui_programacao]";
            value = value + ",[1]";
          }
        }

        // 2. Buscar turmas com filtros (CORREÇÃO: usar cd_pessoa_escola)
        // Assinatura correta do GetListFiltroData (20 parâmetros):
        var turmasResult = await SQLServerService.GetListFiltroData(
            "vi_turma",           // schemaName
            page,                 // page
            limit,                // limit
            sortField,            // sortField
            sortDesc,             // sortDesc
            ids,                  // ids
            "cd_turma",           // idField
            searchFields,         // searchFields
            value,                // value
            source,               // source
            mode,                 // mode
            "cd_pessoa_escola",   // ✅ cd_empresa_field (CORREÇÃO: era cd_empresa)
            cdEmpresa,            // cd_empresa_value
            "dt_inicio_aula",     // dateFieldStart
            "dt_final_aula",      // dateFieldEnd
            dataInicio,           // dateStart
            dataFim,              // dateEnd
            null,                 // dateField2
            null,                 // dateStart2
            null,                 // dateEnd2
            null                  // campoDiaAtual
        );

        if (!turmasResult.success)
        {
          return (false, null, 0, 0, turmasResult.error);
        }

        // 3. Aplicar filtros adicionais de professor e horário (pós-processamento)
        var turmasFiltradas = turmasResult.data;

        // Filtro por professor (se especificado)
        if (professorId != null)
        {
          var turmaIdsComProfessor = new List<string>();

          var professorTurmasResult = await SQLServerService.GetList(
              "T_PROFESSOR_TURMA",
              null,
              null,
              "cd_professor_turma",
              false,
              "",
              "[cd_professor]",
              $"[{professorId}]",
              source,
              SearchModeEnum.Equals,
              null,
              null
          );

          if (professorTurmasResult.success && professorTurmasResult.data != null)
          {
            turmaIdsComProfessor = professorTurmasResult.data
                .Where(x => x.ContainsKey("cd_turma"))
                .Select(x => x["cd_turma"].ToString())
                .Distinct()
                .ToList();

            turmasFiltradas = turmasFiltradas
                .Where(t => turmaIdsComProfessor.Contains(t["cd_turma"].ToString()))
                .ToList();
          }
        }

        // Filtro por horário (se especificado)
        if (!string.IsNullOrEmpty(horario))
        {
          var horarios = horario.Split(',').Select(h => h.Trim()).ToList();
          var turmaIdsComHorario = new List<string>();

          foreach (var h in horarios)
          {
            var horarioResult = await SQLServerService.GetList(
                "T_HORARIO",
                null,
                null,
                "cd_horario",
                false,
                "",
                "[dt_hora_ini],[dt_hora_fim]",
                $"[{h}],[{h}]",
                source,
                SearchModeEnum.Equals,
                null,
                null
            );

            if (horarioResult.success && horarioResult.data != null)
            {
              var horarioIds = horarioResult.data
                  .Where(x => x.ContainsKey("cd_registro"))
                  .Select(x => x["cd_registro"].ToString())
                  .ToList();

              turmaIdsComHorario.AddRange(horarioIds);
            }
          }

          if (turmaIdsComHorario.Any())
          {
            turmasFiltradas = turmasFiltradas
                .Where(t => turmaIdsComHorario.Contains(t["cd_turma"].ToString()))
                .ToList();
          }
        }

        // 4. Recalcular total e páginas após filtros
        var totalFiltrado = turmasFiltradas.Count;
        var pagesFiltrado = limit != null ? (int)Math.Ceiling((double)totalFiltrado / limit.Value) : 0;

        // 5. Extrair IDs das turmas filtradas para buscar dados relacionados
        var turmaIds = turmasFiltradas.Select(x => x["cd_turma"].ToString()).ToList();

        if (!turmaIds.Any())
        {
          var retornoVazio = new
          {
            data = new List<object>(),
            total = 0,
            page = page,
            limit = limit,
            pages = 0
          };
          return (true, retornoVazio, 0, 0, null);
        }

        // 6. Buscar horários das turmas
        var horariosResult = await SQLServerService.GetListIn(
            "T_HORARIO",
            1,
            10000000,
            "cd_registro",
            true,
            null,
            null,
            null,
            source,
            mode,
            null,
            null,
            "cd_registro",
            turmaIds
        );

        // 7. Buscar professores das turmas
        var professoresTurmaResult = await SQLServerService.GetListIn(
            "vi_professor_turma",
            1,
            10000000,
            "cd_turma",
            true,
            null,
            null,
            null,
            source,
            mode,
            null,
            null,
            "cd_turma",
            turmaIds
        );

        // 8. Buscar alunos das turmas
        var alunosResult = await SQLServerService.GetListIn(
            "vi_turma_aluno",
            1,
            10000000,
            "cd_turma",
            true,
            null,
            null,
            null,
            source,
            mode,
            null,
            null,
            "cd_turma",
            turmaIds
        );

        // 9. Montar o retorno EXATAMENTE como o controller fazia
        var retorno = new
        {
          data = turmasFiltradas.Select(x => new
          {
            cd_turma = x["cd_turma"],
            cd_turma_ppt = x["cd_turma_ppt"],
            no_turma = x["no_turma"],
            cd_pessoa_escola = x["cd_pessoa_escola"],
            id_turma_ativa = x["id_turma_ativa"],
            cd_curso = x["cd_curso"],
            no_curso = x["no_curso"],
            cd_sala = x["cd_sala"],
            cd_duracao = x["cd_duracao"],
            cd_regime = x["cd_regime"],
            dt_inicio_aula = x["dt_inicio_aula"],
            dt_final_aula = x["dt_inicio_aula"],  // Controller usa dt_inicio_aula aqui
            id_aula_externa = x["id_aula_externa"],
            nro_aulas_programadas = x["nro_aulas_programadas"],
            id_turma_ppt = x["id_turma_ppt"],
            cd_produto = x["cd_produto"],
            no_produto = x["no_produto"],
            nm_turma = x["nm_turma"],
            dt_termino_turma = x["dt_termino_turma"],
            no_apelido = x["no_apelido"],
            id_percentual_faltas = x["id_percentual_faltas"],
            cd_sala_online = x["cd_sala_online"],
            dc_situacao_turma = x["dc_situacao_turma"],
            nm_vaga_sala = x["nm_vaga_sala"],
            alunos_matriculados = alunosResult.data?.Where(a =>
                a.ContainsKey("cd_turma") &&
                (int)a["cd_turma"] == (int)x["cd_turma"]
              ).Count(),
            professores = professoresTurmaResult.data?.Where(z =>
                z.ContainsKey("cd_turma") &&
                (int)z["cd_turma"] == (int)x["cd_turma"]
              ).Select(y => new
              {
                cd_professor_turma = y["cd_professor_turma"],
                cd_professor = y["cd_professor"],
                no_professor = y["no_professor"],
                cd_turma = y["cd_turma"],
              }).ToList(),
            horarios = horariosResult.data?.Where(h =>
                h.ContainsKey("cd_registro") &&
                (int)h["cd_registro"] == (int)x["cd_turma"]
              ).Select(h => new
              {
                id_dia_semana = h["id_dia_semana"],
                cd_horario = h["cd_horario"],
                dt_hora_ini = h["dt_hora_ini"],
                dt_hora_fim = h["dt_hora_fim"],
              }).ToList()
          }),
          total = totalFiltrado,
          page = page,
          limit = limit,
          pages = pagesFiltrado
        };

        return (true, retorno, totalFiltrado, pagesFiltrado, null);
      }
      catch (Exception ex)
      {
        return (false, null, 0, 0, $"Erro ao buscar turmas: {ex.Message}");
      }
    }

    #region Métodos Públicos - Consultas


    public async Task<(bool success, Dictionary<string, object> data, string error)> BuscarProximaProgramacao(int cdTurma, Source source)
    {
      var campos = "[cd_turma],[id_aula_dada]";
      var valores = $"[{cdTurma}],[0]";

      var resultado = await SQLServerService.GetList("T_PROGRAMACAO_TURMA", 1, 1,
        "dta_programacao_turma", false, null, campos, valores, source, SearchModeEnum.Equals, null, null);

      if (!resultado.success || resultado.data.IsNullOrEmpty())
        return (false, null, "Programação não encontrada");

      var programacao = resultado.data.First();

      var professorResult = await SQLServerService.GetFirstByFields(source, "vi_professor_turma",
        new List<(string campo, object valor)> { ("cd_turma", cdTurma) });

      if (professorResult != null)
      {
        programacao.Add("cd_professor", professorResult["cd_professor"]);
        programacao.Add("no_professor", professorResult["no_professor"]);
      }

      return (true, programacao, null);
    }

    #endregion
  }
}

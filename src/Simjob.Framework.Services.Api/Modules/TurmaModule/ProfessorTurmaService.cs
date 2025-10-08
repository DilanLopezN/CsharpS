using Simjob.Framework.Infra.Identity.Entities;
using Simjob.Framework.Services.Api.Enums;
using Simjob.Framework.Services.Api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Simjob.Framework.Services.Api.Modules.TurmaModule.Services
{
  /// <summary>
  /// Serviço responsável pelas operações relacionadas a Professores de Turma
  /// </summary>
  public class ProfessorTurmaService
  {
    /// <summary>
    /// Busca lista de professores com filtros
    /// </summary>
    public async Task<(bool success, List<Dictionary<string, object>> data, int total, string error)> BuscarProfessores(
      string value,
      SearchModeEnum mode,
      int? page,
      int? limit,
      string sortField,
      bool sortDesc,
      string ids,
      string searchFields,
      string cdEmpresa,
      Source source)
    {
      if (string.IsNullOrEmpty(sortField))
        sortField = "cd_pessoa_escola";

      var resultado = await SQLServerService.GetList(
        "vi_professor",
        page,
        limit,
        sortField,
        sortDesc,
        ids,
        searchFields,
        value,
        source,
        mode,
        "cd_empresa",
        cdEmpresa
      );

      return (resultado.success, resultado.data, resultado.total, resultado.error);
    }

    /// <summary>
    /// Busca disponibilidade de professores para uma data específica
    /// </summary>
    public async Task<object> BuscarDisponibilidadeProfessores(
      string cdEmpresa,
      DateTime data,
      Source source)
    {
      try
      {
        // 1. Buscar todos os professores da empresa
        var professorResult = await SQLServerService.GetList(
          "vi_professor",
          null,
          null,
          "cd_professor",
          false,
          "",
          null,
          "",
          source,
          SearchModeEnum.Contains,
          "cd_empresa",
          cdEmpresa
        );

        if (!professorResult.success)
        {
          return new
          {
            success = false,
            error = $"Erro ao buscar professores: {professorResult.error}",
            data = new List<object>()
          };
        }

        if (professorResult.data == null || !professorResult.data.Any())
        {
          return new
          {
            success = true,
            message = "Nenhum professor encontrado para esta empresa",
            data = new List<object>(),
            cd_empresa = cdEmpresa
          };
        }

        var listaProfessoresDisponiveis = new List<object>();

        // 2. Para cada professor, verificar disponibilidade
        foreach (var professor in professorResult.data)
        {
          if (!professor.ContainsKey("cd_professor")) continue;

          var cdProfessor = professor["cd_professor"];
          var noPessoa = professor.ContainsKey("no_pessoa") ? professor["no_pessoa"] : "";
          var cdPessoa = professor.ContainsKey("cd_pessoa") ? professor["cd_pessoa"] : null;

          // Verificar se o professor está disponível na data
          var disponivel = await VerificarDisponibilidadeProfessor(cdPessoa, data, source, cdEmpresa);

          listaProfessoresDisponiveis.Add(new
          {
            cd_professor = cdProfessor,
            cd_pessoa = cdPessoa,
            no_pessoa = noPessoa,
            cd_empresa = professor.ContainsKey("cd_empresa") ? professor["cd_empresa"] : null,
            disponivel = !disponivel // Invertido: true se NÃO está ocupado
          });
        }

        return new
        {
          success = true,
          data = listaProfessoresDisponiveis,
          total = listaProfessoresDisponiveis.Count,
          data_consultada = data.ToString("yyyy-MM-dd")
        };
      }
      catch (Exception ex)
      {
        return new
        {
          success = false,
          error = $"Erro ao processar disponibilidade: {ex.Message}",
          data = new List<object>()
        };
      }
    }

    /// <summary>
    /// Verifica se um professor está ocupado em uma data específica
    /// </summary>
    private async Task<bool> VerificarDisponibilidadeProfessor(
      object cdPessoa,
      DateTime data,
      Source source,
      string cdEmpresa)
    {
      if (cdPessoa == null) return false;

      // Verificar se tem vínculo com turma
      var vinculoResult = await SQLServerService.GetList(
        "vi_professor_turma",
        null,
        null,
        "cd_professor",
        false,
        "",
        "[cd_professor]",
        $"[{cdPessoa}]",
        source,
        SearchModeEnum.Equals,
        null,
        null
      );

      if (!vinculoResult.success || vinculoResult.data == null || !vinculoResult.data.Any())
        return false; // Não tem vínculo = disponível

      // Se tem vínculo, verificar se há programação para esta data
      foreach (var vinculo in vinculoResult.data)
      {
        if (!vinculo.ContainsKey("cd_turma")) continue;

        var cdTurma = vinculo["cd_turma"];

        // Verificar se existe programação nesta data
        var programacaoResult = await SQLServerService.GetList(
          "T_PROGRAMACAO_TURMA",
          1,
          1,
          "cd_programacao_turma",
          false,
          null,
          "[cd_turma],[dta_programacao_turma]",
          $"[{cdTurma}],[{data:yyyy-MM-dd}]",
          source,
          SearchModeEnum.Equals,
          null,
          null
        );

        if (programacaoResult.success &&
            programacaoResult.data != null &&
            programacaoResult.data.Any())
        {
          return true; // Tem programação = ocupado
        }
      }

      return false; // Não tem programação = disponível
    }
  }
}

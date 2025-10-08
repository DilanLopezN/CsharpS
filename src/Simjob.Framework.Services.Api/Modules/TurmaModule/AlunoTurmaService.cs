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
  /// Serviço responsável pelas operações relacionadas a Alunos de Turma
  /// </summary>
  public class AlunoTurmaService
  {
    /// <summary>
    /// Busca alunos de uma turma específica
    /// </summary>
    public async Task<(bool success, List<object> data, int total, string error)> BuscarAlunosPorTurma(
      string cdTurma,
      Source source)
    {
      try
      {
        var resultado = await SQLServerService.GetListIn(
          "vi_turma_aluno",
          1,
          1000000,
          "cd_aluno",
          false,
          null,
          null,
          null,
          source,
          SearchModeEnum.Contains,
          null,
          null,
          "cd_turma",
          new List<string> { cdTurma }
        );

        if (!resultado.success)
          return (false, null, 0, resultado.error);

        var alunosFormatados = resultado.data.Select(x => new
        {
          cd_aluno_turma = x["cd_aluno_turma"],
          cd_aluno = x["cd_aluno"],
          cd_turma = x["cd_turma"],
          cd_situacao_aluno_origem = x["cd_situacao_aluno_origem"],
          no_aluno = x["no_aluno"]
        }).Cast<object>().ToList();

        return (true, alunosFormatados, resultado.total, null);
      }
      catch (Exception ex)
      {
        return (false, null, 0, $"Erro ao buscar alunos: {ex.Message}");
      }
    }

    /// <summary>
    /// Insere ou atualiza aluno em pré-turma
    /// </summary>
    public async Task<(bool success, string error)> InserirAlunoPreTurma(
      InsertAlunoTurmaPreTurma model,
      Source source)
    {
      try
      {
        // Verificar se aluno já está na turma
        var filtros = new List<(string campo, object valor)>
        {
          ("cd_turma", model.cd_turma),
          ("cd_aluno", model.cd_aluno)
        };

        var alunoTurmaExistente = await SQLServerService.GetFirstByFields(
          source,
          "T_ALUNO_TURMA",
          filtros
        );

        if (alunoTurmaExistente == null)
        {
          // Inserir novo registro
          var alunoTurmaDict = new Dictionary<string, object>
          {
            { "cd_turma", model.cd_turma },
            { "cd_aluno", model.cd_aluno },
            { "cd_contrato", model.cd_contrato },
            { "id_tipo_movimento", 1 },
            { "id_reprovado", 0 },
            { "nm_aulas_dadas", 0 },
            { "nm_faltas", 0 },
            { "id_manter_contrato", 1 },
            { "id_renegociacao", 0 },
            { "cd_curso_contrato", model.cd_curso }
          };

          var resultado = await SQLServerService.Insert("T_ALUNO_TURMA", alunoTurmaDict, source);

          if (!resultado.success)
            return (false, resultado.error);
        }
        else
        {
          // Atualizar registro existente
          var cdAlunoTurma = alunoTurmaExistente["cd_aluno_turma"];
          alunoTurmaExistente.Remove("cd_aluno_turma");
          alunoTurmaExistente["cd_contrato"] = model.cd_contrato;

          var resultado = await SQLServerService.Update(
            "T_ALUNO_TURMA",
            alunoTurmaExistente,
            source,
            "cd_aluno_turma",
            cdAlunoTurma
          );

          if (!resultado.success)
            return (false, resultado.error);
        }

        return (true, null);
      }
      catch (Exception ex)
      {
        return (false, $"Erro ao inserir aluno em pré-turma: {ex.Message}");
      }
    }

    /// <summary>
    /// Busca todos os alunos com filtros (para listagens gerais)
    /// </summary>
    public async Task<(bool success, List<Dictionary<string, object>> data, int total, string error)> BuscarAlunos(
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
        sortField = "cd_aluno";

      var resultado = await SQLServerService.GetList(
        "vi_aluno",
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
    /// Remove aluno de uma turma
    /// </summary>
    public async Task<(bool success, string error)> RemoverAlunoDaTurma(
      int cdAlunoTurma,
      Source source)
    {
      try
      {
        var resultado = await SQLServerService.Delete(
          "T_ALUNO_TURMA",
          "cd_aluno_turma",
          cdAlunoTurma.ToString(),
          source
        );

        if (!resultado.success)
          return (false, resultado.error);

        return (true, null);
      }
      catch (Exception ex)
      {
        return (false, $"Erro ao remover aluno da turma: {ex.Message}");
      }
    }

    /// <summary>
    /// Atualiza situação do aluno na turma
    /// </summary>
    public async Task<(bool success, string error)> AtualizarSituacaoAluno(
      int cdAlunoTurma,
      int cdSituacaoAlunoTurma,
      Source source)
    {
      try
      {
        var updateDict = new Dictionary<string, object>
        {
          { "cd_situacao_aluno_turma", cdSituacaoAlunoTurma }
        };

        var resultado = await SQLServerService.Update(
          "T_ALUNO_TURMA",
          updateDict,
          source,
          "cd_aluno_turma",
          cdAlunoTurma
        );

        if (!resultado.success)
          return (false, resultado.error);

        return (true, null);
      }
      catch (Exception ex)
      {
        return (false, $"Erro ao atualizar situação do aluno: {ex.Message}");
      }
    }

    /// <summary>
    /// Busca histórico de turmas do aluno
    /// </summary>
    public async Task<(bool success, List<Dictionary<string, object>> data, string error)> BuscarHistoricoTurmasAluno(
      int cdAluno,
      Source source)
    {
      try
      {
        var resultado = await SQLServerService.GetList(
          "T_ALUNO_TURMA",
          1,
          10000,
          "dt_inicio",
          true,
          null,
          "[cd_aluno]",
          $"[{cdAluno}]",
          source,
          SearchModeEnum.Equals,
          null,
          null
        );

        if (!resultado.success)
          return (false, null, resultado.error);

        return (true, resultado.data, null);
      }
      catch (Exception ex)
      {
        return (false, null, $"Erro ao buscar histórico: {ex.Message}");
      }
    }

    /// <summary>
    /// Verifica se aluno já está em alguma turma ativa
    /// </summary>
    public async Task<(bool possuiTurmaAtiva, int? cdTurma, string error)> VerificarAlunoEmTurmaAtiva(
      int cdAluno,
      Source source)
    {
      try
      {
        // Buscar turmas ativas do aluno
        var resultado = await SQLServerService.GetList(
          "vi_turma_aluno",
          1,
          1,
          "cd_turma",
          true,
          null,
          "[cd_aluno]",
          $"[{cdAluno}]",
          source,
          SearchModeEnum.Equals,
          null,
          null
        );

        if (!resultado.success)
          return (false, null, resultado.error);

        if (resultado.data != null && resultado.data.Any())
        {
          var cdTurma = (int)resultado.data.First()["cd_turma"];
          return (true, cdTurma, null);
        }

        return (false, null, null);
      }
      catch (Exception ex)
      {
        return (false, null, $"Erro ao verificar turma ativa: {ex.Message}");
      }
    }
  }
}

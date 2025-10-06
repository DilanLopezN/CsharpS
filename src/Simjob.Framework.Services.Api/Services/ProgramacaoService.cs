using Simjob.Framework.Services.Api.Models.Turmas;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Simjob.Framework.Infra.Identity.Entities;

namespace Simjob.Framework.Services.Api.Services
{
  public class ProgramacaoService
  {
    /// <summary>
    /// Busca programação da matriz (T_PROGRAMACAO_CURSO + T_ITEM_PROGRAMACAO)
    /// Regra 3.2 - Primeira prioridade
    /// </summary>
    public static async Task<(bool success, List<ProgramacaoCursoModel> data, string error)>
        BuscarProgramacaoMatriz(
            int cursoId,
            int duracaoId,
            int? escolaId,
            Source source)
    {
      try
      {
        // Buscar programação do curso
        var searchFields = "[cd_curso],[cd_duracao]";
        var values = $"[{cursoId}],[{duracaoId}]";

        // Se escola for informada, adicionar no filtro
        if (escolaId.HasValue)
        {
          searchFields += ",[cd_escola]";
          values += $",[{escolaId.Value}]";
        }

        var result = await SQLServerService.GetList(
            "T_PROGRAMACAO_CURSO",
            null,
            searchFields,
            values,
            source,
            Enums.SearchModeEnum.Equals
        );

        if (!result.success || !result.data.Any())
          return (false, null, "Nenhuma programação da matriz encontrada");

        var programacoes = new List<ProgramacaoCursoModel>();

        foreach (var prog in result.data)
        {
          int cdProgramacaoCurso = (int)prog["cd_programacao_curso"];

          // Buscar itens da programação
          var itensResult = await SQLServerService.GetList(
              "T_ITEM_PROGRAMACAO",
              null,
              "[cd_programacao_curso]",
              $"[{cdProgramacaoCurso}]",
              source,
              Enums.SearchModeEnum.Equals
          );

          var itens = new List<ItemProgramacaoModel>();
          if (itensResult.success && itensResult.data.Any())
          {
            itens = itensResult.data.Select(item => new ItemProgramacaoModel
            {
              cd_item_programacao = (int)item["cd_item_programacao"],
              cd_programacao_curso = (int)item["cd_programacao_curso"],
              nm_aula_programacao = (int)item["nm_aula_programacao"],
              dc_aula_programacao = item["dc_aula_programacao"]?.ToString()
            }).ToList();
          }

          programacoes.Add(new ProgramacaoCursoModel
          {
            cd_programacao_curso = cdProgramacaoCurso,
            cd_curso = (int)prog["cd_curso"],
            cd_duracao = (int)prog["cd_duracao"],
            cd_escola = prog["cd_escola"] != null ? (int?)prog["cd_escola"] : null,
            Itens = itens
          });
        }

        return (true, programacoes, null);
      }
      catch (Exception ex)
      {
        return (false, null, $"Erro ao buscar programação da matriz: {ex.Message}");
      }
    }

    /// <summary>
    /// Busca feriados do ano (T_FERIADO)
    /// Regra 3.3 - Tratamento de Feriados
    /// </summary>
    public static async Task<(bool success, List<FeriadoModel> data, string error)>
        BuscarFeriados(int ano, int? cdEscola, Source source)
    {
      try
      {
        var searchFields = "[aa_feriado],[id_feriado_ativo]";
        var values = $"[{ano}],[1]";

        // Adicionar filtro de escola se informado
        if (cdEscola.HasValue)
        {
          searchFields += ",[cd_pessoa_escola]";
          values += $",[{cdEscola.Value}]";
        }

        var result = await SQLServerService.GetList(
            "T_FERIADO",
            null,
            searchFields,
            values,
            source,
            Enums.SearchModeEnum.Equals
        );

        if (!result.success)
          return (false, null, result.error);

        var feriados = result.data.Select(item => new FeriadoModel
        {
          cod_feriado = (int)item["cod_feriado"],
          cd_pessoa_escola = item["cd_pessoa_escola"] != null ? (int?)item["cd_pessoa_escola"] : null,
          dd_feriado = (int)item["dd_feriado"],
          aa_feriado = (int)item["aa_feriado"],
          mm_feriado = (int)item["mm_feriado"],
          dc_feriado = item["dc_feriado"]?.ToString(),
          id_feriado_financeiro = (bool)item["id_feriado_financeiro"],
          aa_feriado_fim = item["aa_feriado_fim"] != null ? (int?)item["aa_feriado_fim"] : null,
          mm_feriado_fim = item["mm_feriado_fim"] != null ? (int?)item["mm_feriado_fim"] : null,
          dd_feriado_fim = item["dd_feriado_fim"] != null ? (int?)item["dd_feriado_fim"] : null,
          id_feriado_ativo = (bool)item["id_feriado_ativo"],
          cd_ferias = item["cd_ferias"] != null ? (int?)item["cd_ferias"] : null
        }).ToList();

        return (true, feriados, null);
      }
      catch (Exception ex)
      {
        return (false, null, $"Erro ao buscar feriados: {ex.Message}");
      }
    }
  }
}


using Simjob.Framework.Services.Api.Models.Turmas;
using Simjob.Framework.Infra.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Simjob.Framework.Services.Api.Services
{
  public class ProgramacaoTurmaService
  {
    /// <summary>
    /// Busca programação da matriz (fundação) - T_PROGRAMACAO_CURSO
    /// Primeira prioridade conforme regra 3.2
    /// </summary>
    public static async Task<(bool success, object data, string error)> BuscarProgramacaoMatriz(
        int cd_curso,
        int cd_produto,
        int cd_regime,
        int cd_duracao,
        int cd_duracao_aula,
        Source source)
    {
      try
      {
        // Buscar programação da matriz que corresponda aos parâmetros
        var campos = "[cd_curso],[cd_produto],[cd_regime],[cd_duracao]";
        var valores = $"[{cd_curso}],[{cd_produto}],[{cd_regime}],[{cd_duracao}]";

        var programacaoGet = await SQLServerService.GetList(
            "T_PROGRAMACAO_CURSO",
            1,
            1000,
            "cd_programacao_curso",
            false,
            null,
            campos,
            valores,
            source,
            Enums.SearchModeEnum.Equals,
            null,
            null);

        if (!programacaoGet.success || programacaoGet.data == null || !programacaoGet.data.Any())
        {
          return (false, null, "Programação da matriz não encontrada");
        }

        // Buscar itens da programação
        var cd_programacao_curso = programacaoGet.data.First()["cd_programacao_curso"];
        var itensGet = await SQLServerService.GetList(
            "T_ITEM_PROGRAMACAO",
            1,
            10000,
            "nm_item_programacao",
            false,
            null,
            "[cd_programacao_curso]",
            $"[{cd_programacao_curso}]",
            source,
            Enums.SearchModeEnum.Equals,
            null,
            null);

        if (!itensGet.success || itensGet.data == null || !itensGet.data.Any())
        {
          return (false, null, "Itens da programação não encontrados");
        }

        return (true, new { programacao = programacaoGet.data.First(), itens = itensGet.data }, null);
      }
      catch (Exception ex)
      {
        return (false, null, $"Erro ao buscar programação da matriz: {ex.Message}");
      }
    }

    /// <summary>
    /// Busca modelo de programação do franqueado - T_PROGRAMACAO_CURSO (cd_escola preenchido)
    /// Segunda prioridade conforme regra 3.2
    /// </summary>
    public static async Task<(bool success, object data, string error)> BuscarModeloFranqueado(
        int cd_curso,
        int cd_escola,
        int cd_regime,
        int cd_duracao,
        Source source)
    {
      try
      {
        var campos = "[cd_curso],[cd_escola],[cd_regime],[cd_duracao]";
        var valores = $"[{cd_curso}],[{cd_escola}],[{cd_regime}],[{cd_duracao}]";

        var programacaoGet = await SQLServerService.GetList(
            "T_PROGRAMACAO_CURSO",
            1,
            1000,
            "cd_programacao_curso",
            false,
            null,
            campos,
            valores,
            source,
            Enums.SearchModeEnum.Equals,
            null,
            null);

        if (!programacaoGet.success || programacaoGet.data == null || !programacaoGet.data.Any())
        {
          return (false, null, "Modelo de programação do franqueado não encontrado");
        }

        var cd_programacao_curso = programacaoGet.data.First()["cd_programacao_curso"];
        var itensGet = await SQLServerService.GetList(
            "T_ITEM_PROGRAMACAO",
            1,
            10000,
            "nm_item_programacao",
            false,
            null,
            "[cd_programacao_curso]",
            $"[{cd_programacao_curso}]",
            source,
            Enums.SearchModeEnum.Equals,
            null,
            null);

        if (!itensGet.success || itensGet.data == null || !itensGet.data.Any())
        {
          return (false, null, "Itens da programação não encontrados");
        }

        return (true, new { programacao = programacaoGet.data.First(), itens = itensGet.data }, null);
      }
      catch (Exception ex)
      {
        return (false, null, $"Erro ao buscar modelo do franqueado: {ex.Message}");
      }
    }

    /// <summary>
    /// Gera programação manual automática
    /// Terceira prioridade conforme regra 3.2
    /// </summary>
    public static List<ProgramacaoTurmaInsertModel> GerarProgramacaoManualAutomatica(
        DateTime dt_inicio,
        List<HorarioTurmaModel> horarios,
        int cargaHorariaTotalCurso,
        int cargaHorariaPorAula)
    {
      var programacoes = new List<ProgramacaoTurmaInsertModel>();
      var dataAtual = dt_inicio;
      int numeroAula = 1;
      int numeroAux = 1;
      int cargaHorariaAcumulada = 0;

      // Ordenar horários por dia da semana
      var horariosOrdenados = horarios.OrderBy(h => h.cd_dia_semana).ToList();

      while (cargaHorariaAcumulada < cargaHorariaTotalCurso)
      {
        foreach (var horario in horariosOrdenados)
        {
          // Calcular próxima data do dia da semana
          while ((int)dataAtual.DayOfWeek != horario.cd_dia_semana)
          {
            dataAtual = dataAtual.AddDays(1);
          }

          var programacao = new ProgramacaoTurmaInsertModel
          {
            nm_aula_programacao_turma = numeroAula,
            dta_programacao_turma = dataAtual.ToString("yyyy-MM-dd"),
            dc_programacao_turma = $"Aula {numeroAula}",
            hr_inicial_programacao = horario.hr_inicial,
            hr_final_programacao = horario.hr_final,
            nm_programacao_aux = numeroAux,
            id_aula_dada = false,
            id_programacao_manual = false,
            id_reprogramada = false,
            id_provisoria = false,
            id_mostrar_calendario = true,
            dta_cadastro_programacao = DateTime.Now.ToString("yyyy-MM-dd"),
            nm_programacao_real = numeroAula,
            id_prog_cancelada = false,
            id_modificada = false
          };

          programacoes.Add(programacao);

          cargaHorariaAcumulada += cargaHorariaPorAula;
          numeroAula++;
          numeroAux++;

          if (cargaHorariaAcumulada >= cargaHorariaTotalCurso)
            break;

          dataAtual = dataAtual.AddDays(1);
        }

        if (cargaHorariaAcumulada >= cargaHorariaTotalCurso)
          break;
      }

      return programacoes;
    }

    /// <summary>
    /// Gera programação baseada em ordem de prioridade (Regra 3.2)
    /// 1. Matriz → 2. Franqueado → 3. Manual Automática
    /// </summary>
    public static async Task<(bool success, List<ProgramacaoTurmaInsertModel> programacoes, string error)> GerarProgramacaoTurma(
        int cd_curso,
        int cd_produto,
        int cd_regime,
        int cd_duracao,
        int cd_duracao_aula,
        int cd_escola,
        DateTime dt_inicio,
        List<HorarioTurmaModel> horarios,
        int cargaHorariaTotalCurso,
        Source source)
    {
      try
      {
        // 1ª Prioridade: Programação da Matriz
        var resultadoMatriz = await BuscarProgramacaoMatriz(
            cd_curso, cd_produto, cd_regime, cd_duracao, cd_duracao_aula, source);

        if (resultadoMatriz.success)
        {
          var programacoes = ConverterItemsProgramacaoParaTurma(
              resultadoMatriz.data, dt_inicio, horarios);
          return (true, programacoes, null);
        }

        // 2ª Prioridade: Modelo do Franqueado
        var resultadoFranqueado = await BuscarModeloFranqueado(
            cd_curso, cd_escola, cd_regime, cd_duracao, source);

        if (resultadoFranqueado.success)
        {
          var programacoes = ConverterItemsProgramacaoParaTurma(
              resultadoFranqueado.data, dt_inicio, horarios);
          return (true, programacoes, null);
        }

        // 3ª Prioridade: Geração Manual Automática
        var programacoesManual = GerarProgramacaoManualAutomatica(
            dt_inicio, horarios, cargaHorariaTotalCurso, cd_duracao_aula);

        return (true, programacoesManual, null);
      }
      catch (Exception ex)
      {
        return (false, null, $"Erro ao gerar programação: {ex.Message}");
      }
    }

    /// <summary>
    /// Converte itens de programação (matriz ou franqueado) para programação de turma
    /// </summary>
    private static List<ProgramacaoTurmaInsertModel> ConverterItemsProgramacaoParaTurma(
        dynamic dadosProgramacao,
        DateTime dt_inicio,
        List<HorarioTurmaModel> horarios)
    {
      var programacoes = new List<ProgramacaoTurmaInsertModel>();
      var itens = dadosProgramacao.itens as IEnumerable<dynamic>;

      if (itens == null) return programacoes;

      var dataAtual = dt_inicio;
      int numeroAux = 1;
      var horariosOrdenados = horarios.OrderBy(h => h.cd_dia_semana).ToList();

      foreach (var item in itens)
      {
        foreach (var horario in horariosOrdenados)
        {
          while ((int)dataAtual.DayOfWeek != horario.cd_dia_semana)
          {
            dataAtual = dataAtual.AddDays(1);
          }

          var programacao = new ProgramacaoTurmaInsertModel
          {
            nm_aula_programacao_turma = (int)item["nm_item_programacao"],
            dta_programacao_turma = dataAtual.ToString("yyyy-MM-dd"),
            dc_programacao_turma = item["dc_item_programacao"]?.ToString() ?? "",
            hr_inicial_programacao = horario.hr_inicial,
            hr_final_programacao = horario.hr_final,
            nm_programacao_aux = numeroAux,
            id_aula_dada = false,
            id_programacao_manual = false,
            id_reprogramada = false,
            id_provisoria = false,
            id_mostrar_calendario = true,
            dta_cadastro_programacao = DateTime.Now.ToString("yyyy-MM-dd"),
            nm_programacao_real = (int)item["nm_item_programacao"],
            id_prog_cancelada = false,
            id_modificada = false
          };

          programacoes.Add(programacao);
          numeroAux++;
          dataAtual = dataAtual.AddDays(1);
          break;
        }
      }

      return programacoes;
    }

    /// <summary>
    /// Valida se programação atinge carga horária mínima do curso
    /// </summary>
    public static bool ValidarCargaHorariaMinima(
        List<ProgramacaoTurmaInsertModel> programacoes,
        int cargaHorariaTotalCurso,
        int cargaHorariaPorAula)
    {
      int cargaTotal = programacoes.Count * cargaHorariaPorAula;
      return cargaTotal >= cargaHorariaTotalCurso;
    }

    /// <summary>
    /// Marca feriados nas programações
    /// </summary>
    public static async Task<List<Dictionary<string, object>>> MarcarFeriadosNaProgramacao(
        List<Dictionary<string, object>> programacoes,
        int cd_escola,
        Source source)
    {
      try
      {
        // Buscar feriados da escola
        var feriadosGet = await SQLServerService.GetList(
            "T_FERIADO",
            1,
            10000,
            "dt_feriado",
            false,
            null,
            "[cd_escola]",
            $"[{cd_escola}]",
            source,
            Enums.SearchModeEnum.Equals,
            null,
            null);

        if (!feriadosGet.success || feriadosGet.data == null || !feriadosGet.data.Any())
          return programacoes;

        var feriados = feriadosGet.data
            .Select(f => DateTime.Parse(f["dt_feriado"].ToString()))
            .ToList();

        foreach (var prog in programacoes)
        {
          var dataProg = DateTime.Parse(prog["dta_programacao_turma"].ToString());
          var feriado = feriados.FirstOrDefault(f => f.Date == dataProg.Date);

          if (feriado != default(DateTime))
          {
            // Buscar cd_feriado
            var feriadoData = feriadosGet.data.FirstOrDefault(f =>
                DateTime.Parse(f["dt_feriado"].ToString()).Date == dataProg.Date);

            if (feriadoData != null)
            {
              prog["cd_feriado"] = feriadoData["cd_feriado"];
              prog["is_feriado"] = true;
            }
          }
        }

        return programacoes;
      }
      catch (Exception)
      {
        return programacoes;
      }
    }
  }
}

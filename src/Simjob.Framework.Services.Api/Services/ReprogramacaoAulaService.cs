using Simjob.Framework.Services.Api.Services;
using Simjob.Framework.Services.Api.Enums;
using Simjob.Framework.Services.Api.Models.Turmas;
using Simjob.Framework.Infra.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Simjob.Framework.Services.Api.Services
{
  /// <summary>
  /// Service para reprogramação de aulas
  /// Implementa regras 3.4, 3.5, 3.6 e 3.7 da documentação
  /// </summary>
  public class ReprogramacaoAulaService
  {
    /// <summary>
    /// Valida se a reprogramação é permitida conforme regra 3.4
    /// </summary>
    public static async Task<(bool valido, string erro)> ValidarReprogramacao(
        int cd_programacao_turma,
        DateTime data_original,
        DateTime nova_data,
        int cd_turma,
        Source source)
    {
      // Regra: Aula só pode ser reprogramada para até 10 dias futuros
      if (nova_data > data_original.AddDays(10))
      {
        return (false, "Aula só pode ser reprogramada para até 10 dias futuros da data original");
      }

      // Buscar última programação com diário lançado
      var ultimoDiarioGet = await SQLServerService.GetList(
          "T_PROGRAMACAO_TURMA",
          1,
          1,
          "dta_programacao_turma",
          true,
          null,
          "[cd_turma],[id_aula_dada]",
          $"[{cd_turma}],[1]",
          source,
          SearchModeEnum.Equals,
          null,
          null);

      if (ultimoDiarioGet.success && ultimoDiarioGet.data != null && ultimoDiarioGet.data.Any())
      {
        var ultimoDiario = DateTime.Parse(ultimoDiarioGet.data.First()["dta_programacao_turma"].ToString());

        // Não pode reprogramar para antes do último diário lançado
        if (nova_data < ultimoDiario)
        {
          return (false, "Não é possível reprogramar para data anterior ao último diário de aula lançado");
        }
      }

      // Buscar data de início da turma
      var turmaGet = await SQLServerService.GetFirstByFields(
          source,
          "T_TURMA",
          new List<(string campo, object valor)> { ("cd_turma", cd_turma) });

      if (turmaGet != null)
      {
        var dtInicioTurma = DateTime.Parse(turmaGet["dt_inicio_aula"].ToString());

        if (nova_data < dtInicioTurma)
        {
          return (false, "Não é possível reprogramar para data anterior ao início da turma");
        }
      }

      // Verificar se já foi reprogramada
      var programacaoGet = await SQLServerService.GetFirstByFields(
          source,
          "T_PROGRAMACAO_TURMA",
          new List<(string campo, object valor)> { ("cd_programacao_turma", cd_programacao_turma) });

      if (programacaoGet != null && (bool)programacaoGet["id_reprogramada"])
      {
        return (false, "Esta aula já foi reprogramada. Não é possível reprogramar mais de uma vez");
      }

      // Verificar se tem diário lançado
      if (programacaoGet != null && (bool)programacaoGet["id_aula_dada"])
      {
        return (false, "Não é possível reprogramar aula que já possui diário lançado");
      }

      return (true, null);
    }

    /// <summary>
    /// Valida disponibilidade de sala e professor para reprogramação
    /// </summary>
    public static async Task<ValidacaoDisponibilidadeModel> ValidarDisponibilidade(
        DateTime nova_data,
        string hr_inicial,
        string hr_final,
        int cd_sala,
        int? cd_professor,
        int cd_turma,
        Source source)
    {
      var resultado = new ValidacaoDisponibilidadeModel
      {
        sala_disponivel = true,
        professor_disponivel = true
      };

      // Validar disponibilidade de sala
      if (cd_sala > 0)
      {
        var salaOcupada = await VerificarSalaOcupada(nova_data, hr_inicial, hr_final, cd_sala, cd_turma, source);
        if (salaOcupada)
        {
          resultado.sala_disponivel = false;
          resultado.mensagem_erro = "Sala não disponível no horário solicitado";
          return resultado;
        }
      }

      // Validar disponibilidade de professor
      if (cd_professor.HasValue && cd_professor.Value > 0)
      {
        var professorOcupado = await VerificarProfessorOcupado(nova_data, hr_inicial, hr_final, cd_professor.Value, cd_turma, source);
        if (professorOcupado)
        {
          resultado.professor_disponivel = false;
          resultado.mensagem_erro = "Professor não disponível no horário solicitado";
          return resultado;
        }
      }

      return resultado;
    }

    /// <summary>
    /// Verifica se sala está ocupada no horário
    /// </summary>
    private static async Task<bool> VerificarSalaOcupada(
        DateTime data,
        string hr_inicial,
        string hr_final,
        int cd_sala,
        int cd_turma_atual,
        Source source)
    {
      try
      {
        var dataStr = data.ToString("yyyy-MM-dd");

        // Buscar programações na mesma data e sala
        var programacoesGet = await SQLServerService.GetList(
            "T_PROGRAMACAO_TURMA",
            1,
            1000,
            "cd_programacao_turma",
            false,
            null,
            "[dta_programacao_turma]",
            $"[{dataStr}]",
            source,
            SearchModeEnum.Equals,
            null,
            null);

        if (!programacoesGet.success || programacoesGet.data == null || !programacoesGet.data.Any())
          return false;

        foreach (var prog in programacoesGet.data)
        {
          int cd_turma_prog = (int)prog["cd_turma"];

          // Buscar turma para verificar sala
          var turmaGet = await SQLServerService.GetFirstByFields(
              source,
              "T_TURMA",
              new List<(string campo, object valor)> { ("cd_turma", cd_turma_prog) });

          if (turmaGet != null && turmaGet.ContainsKey("cd_sala"))
          {
            int cd_sala_turma = turmaGet["cd_sala"] != null ? (int)turmaGet["cd_sala"] : 0;

            if (cd_sala_turma == cd_sala && cd_turma_prog != cd_turma_atual)
            {
              // Verificar se há conflito de horário
              string hr_ini_prog = prog["hr_inicial_programacao"].ToString();
              string hr_fim_prog = prog["hr_final_programacao"].ToString();

              if (HorariosConflitam(hr_inicial, hr_final, hr_ini_prog, hr_fim_prog))
                return true;
            }
          }
        }

        return false;
      }
      catch
      {
        return false;
      }
    }

    /// <summary>
    /// Verifica se professor está ocupado no horário
    /// </summary>
    private static async Task<bool> VerificarProfessorOcupado(
        DateTime data,
        string hr_inicial,
        string hr_final,
        int cd_professor,
        int cd_turma_atual,
        Source source)
    {
      try
      {
        var dataStr = data.ToString("yyyy-MM-dd");

        // Buscar programações na mesma data
        var programacoesGet = await SQLServerService.GetList(
            "T_PROGRAMACAO_TURMA",
            1,
            1000,
            "cd_programacao_turma",
            false,
            null,
            "[dta_programacao_turma]",
            $"[{dataStr}]",
            source,
            SearchModeEnum.Equals,
            null,
            null);

        if (!programacoesGet.success || programacoesGet.data == null || !programacoesGet.data.Any())
          return false;

        foreach (var prog in programacoesGet.data)
        {
          int cd_turma_prog = (int)prog["cd_turma"];

          if (cd_turma_prog == cd_turma_atual)
            continue;

          // Buscar professor da turma
          var profTurmaGet = await SQLServerService.GetFirstByFields(
              source,
              "T_TURMA_PROFESSOR",
              new List<(string campo, object valor)>
              {
                            ("cd_turma", cd_turma_prog),
                            ("cd_professor", cd_professor)
              });

          if (profTurmaGet != null)
          {
            // Verificar se há conflito de horário
            string hr_ini_prog = prog["hr_inicial_programacao"].ToString();
            string hr_fim_prog = prog["hr_final_programacao"].ToString();

            if (HorariosConflitam(hr_inicial, hr_final, hr_ini_prog, hr_fim_prog))
              return true;
          }
        }

        return false;
      }
      catch
      {
        return false;
      }
    }

    /// <summary>
    /// Verifica se dois horários conflitam
    /// </summary>
    private static bool HorariosConflitam(string hr1_ini, string hr1_fim, string hr2_ini, string hr2_fim)
    {
      try
      {
        var h1_ini = TimeSpan.Parse(hr1_ini);
        var h1_fim = TimeSpan.Parse(hr1_fim);
        var h2_ini = TimeSpan.Parse(hr2_ini);
        var h2_fim = TimeSpan.Parse(hr2_fim);

        // Verifica se há sobreposição
        return (h1_ini < h2_fim && h1_fim > h2_ini);
      }
      catch
      {
        return false;
      }
    }

    /// <summary>
    /// Executa reprogramação COM reordenação (quando move para próxima aula disponível)
    /// Regra: quando a programação é movida para a próxima aula disponível da turma
    /// </summary>
    public static async Task<(bool success, string error)> ReprogramarComReordenacao(
        int cd_programacao_turma,
        DateTime nova_data,
        string novo_hr_inicial,
        string novo_hr_final,
        int cd_turma,
        int cd_usuario,
        Source source)
    {
      try
      {
        // Buscar programação original
        var progOriginalGet = await SQLServerService.GetFirstByFields(
            source,
            "T_PROGRAMACAO_TURMA",
            new List<(string campo, object valor)> { ("cd_programacao_turma", cd_programacao_turma) });

        if (progOriginalGet == null)
          return (false, "Programação não encontrada");

        var data_original = DateTime.Parse(progOriginalGet["dta_programacao_turma"].ToString());
        var nm_aux_original = (int)progOriginalGet["nm_programacao_aux"];

        // Buscar todas programações futuras (após a original) sem diário lançado
        var todasProgsGet = await SQLServerService.GetList(
            "T_PROGRAMACAO_TURMA",
            1,
            10000,
            "nm_programacao_aux",
            false,
            null,
            "[cd_turma],[id_aula_dada]",
            $"[{cd_turma}],[0]",
            source,
            SearchModeEnum.Equals,
            null,
            null);

        if (!todasProgsGet.success || todasProgsGet.data == null)
          return (false, "Erro ao buscar programações");

        var programacoesFuturas = todasProgsGet.data
            .Where(p => (int)p["nm_programacao_aux"] > nm_aux_original)
            .OrderBy(p => (int)p["nm_programacao_aux"])
            .ToList();

        // Buscar horários da turma
        var horariosGet = await SQLServerService.GetList(
            "T_HORARIO",
            1,
            100,
            "cd_dia_semana",
            false,
            null,
            "[cd_turma]",
            $"[{cd_turma}]",
            source,
            SearchModeEnum.Equals,
            null,
            null);

        if (!horariosGet.success || horariosGet.data == null || !horariosGet.data.Any())
          return (false, "Horários da turma não encontrados");

        var horarios = horariosGet.data
            .Select(h => new HorarioTurmaModel
            {
              cd_dia_semana = (int)h["cd_dia_semana"],
              hr_inicial = h["hr_inicial"].ToString(),
              hr_final = h["hr_final"].ToString()
            })
            .OrderBy(h => h.cd_dia_semana)
            .ToList();

        // 1. Marcar programação original como "Reprogramada"
        var updateOriginal = new Dictionary<string, object>
        {
          ["id_reprogramada"] = true,
          ["dc_programacao_turma"] = $"Reprogramada para o dia {nova_data:dd/MM/yyyy}",
          ["id_mostrar_calendario"] = false
        };

        await SQLServerService.Update(
            "T_PROGRAMACAO_TURMA",
            updateOriginal,
            source,
            "cd_programacao_turma",
            cd_programacao_turma);

        // 2. Inserir programação reprogramada
        var novoAux = nm_aux_original + 1;
        var novaProg = new Dictionary<string, object>
        {
          ["cd_turma"] = cd_turma,
          ["nm_aula_programacao_turma"] = progOriginalGet["nm_aula_programacao_turma"],
          ["dta_programacao_turma"] = nova_data.ToString("yyyy-MM-dd"),
          ["dc_programacao_turma"] = progOriginalGet["dc_programacao_turma"],
          ["hr_inicial_programacao"] = novo_hr_inicial,
          ["hr_final_programacao"] = novo_hr_final,
          ["nm_programacao_aux"] = novoAux,
          ["id_aula_dada"] = false,
          ["id_programacao_manual"] = false,
          ["id_reprogramada"] = false,
          ["id_provisoria"] = false,
          ["id_mostrar_calendario"] = true,
          ["dta_cadastro_programacao"] = DateTime.Now.ToString("yyyy-MM-dd"),
          ["nm_programacao_real"] = progOriginalGet["nm_programacao_real"],
          ["id_prog_cancelada"] = false,
          ["id_modificada"] = true
        };

        await SQLServerService.Insert("T_PROGRAMACAO_TURMA", novaProg, source);

        // 3. Reordenar todas as programações futuras (incrementar nm_programacao_aux)
        foreach (var prog in programacoesFuturas)
        {
          int cd_prog = (int)prog["cd_programacao_turma"];
          int nm_aux_atual = (int)prog["nm_programacao_aux"];

          var updateAux = new Dictionary<string, object>
          {
            ["nm_programacao_aux"] = nm_aux_atual + 1
          };

          await SQLServerService.Update(
              "T_PROGRAMACAO_TURMA",
              updateAux,
              source,
              "cd_programacao_turma",
              cd_prog);
        }

        // 4. Adicionar nova aula no final para manter carga horária
        var ultimaProg = programacoesFuturas.LastOrDefault() ?? progOriginalGet;
        var ultima_data = DateTime.Parse(ultimaProg["dta_programacao_turma"].ToString());
        var proximo_numero_aula = programacoesFuturas.Any()
            ? programacoesFuturas.Max(p => (int)p["nm_programacao_real"]) + 1
            : (int)progOriginalGet["nm_programacao_real"] + 1;

        // Calcular próxima data baseada nos horários da turma
        var proxima_data = CalcularProximaDataAula(ultima_data, horarios);
        var proximo_aux = programacoesFuturas.Any()
            ? programacoesFuturas.Max(p => (int)p["nm_programacao_aux"]) + 2
            : nm_aux_original + 2;

        var horario_turma = horarios.First();
        var novaAulaFinal = new Dictionary<string, object>
        {
          ["cd_turma"] = cd_turma,
          ["nm_aula_programacao_turma"] = proximo_numero_aula,
          ["dta_programacao_turma"] = proxima_data.ToString("yyyy-MM-dd"),
          ["dc_programacao_turma"] = $"Aula {proximo_numero_aula}",
          ["hr_inicial_programacao"] = horario_turma.hr_inicial,
          ["hr_final_programacao"] = horario_turma.hr_final,
          ["nm_programacao_aux"] = proximo_aux,
          ["id_aula_dada"] = false,
          ["id_programacao_manual"] = false,
          ["id_reprogramada"] = false,
          ["id_provisoria"] = false,
          ["id_mostrar_calendario"] = true,
          ["dta_cadastro_programacao"] = DateTime.Now.ToString("yyyy-MM-dd"),
          ["nm_programacao_real"] = proximo_numero_aula,
          ["id_prog_cancelada"] = false,
          ["id_modificada"] = false
        };

        await SQLServerService.Insert("T_PROGRAMACAO_TURMA", novaAulaFinal, source);

        // 5. Registrar no histórico
        await RegistrarHistoricoReprogramacao(
            cd_programacao_turma,
            cd_turma,
            cd_usuario,
            data_original,
            nova_data,
            "Reprogramação com reordenação",
            source);

        // 6. Atualizar dt_final_aula da turma
        await AtualizarDataFinalTurma(cd_turma, source);

        return (true, null);
      }
      catch (Exception ex)
      {
        return (false, $"Erro ao reprogramar com reordenação: {ex.Message}");
      }
    }

    /// <summary>
    /// Executa reprogramação SEM reordenação (quando não interfere em aulas futuras)
    /// </summary>
    public static async Task<(bool success, string error)> ReprogramarSemReordenacao(
        int cd_programacao_turma,
        DateTime nova_data,
        string novo_hr_inicial,
        string novo_hr_final,
        int cd_turma,
        int cd_usuario,
        Source source)
    {
      try
      {
        // Buscar programação original
        var progOriginalGet = await SQLServerService.GetFirstByFields(
            source,
            "T_PROGRAMACAO_TURMA",
            new List<(string campo, object valor)> { ("cd_programacao_turma", cd_programacao_turma) });

        if (progOriginalGet == null)
          return (false, "Programação não encontrada");

        var data_original = DateTime.Parse(progOriginalGet["dta_programacao_turma"].ToString());
        var nm_aux_original = (int)progOriginalGet["nm_programacao_aux"];

        // 1. Marcar programação original como "Reprogramada"
        var updateOriginal = new Dictionary<string, object>
        {
          ["id_reprogramada"] = true,
          ["dc_programacao_turma"] = $"Reprogramada para o dia {nova_data:dd/MM/yyyy}",
          ["id_mostrar_calendario"] = false
        };

        await SQLServerService.Update(
            "T_PROGRAMACAO_TURMA",
            updateOriginal,
            source,
            "cd_programacao_turma",
            cd_programacao_turma);

        // 2. Calcular novo auxiliar baseado na data
        var todasProgsGet = await SQLServerService.GetList(
            "T_PROGRAMACAO_TURMA",
            1,
            10000,
            "nm_programacao_aux",
            false,
            null,
            "[cd_turma]",
            $"[{cd_turma}]",
            source,
            SearchModeEnum.Equals,
            null,
            null);

        int novo_aux = nm_aux_original;
        if (todasProgsGet.success && todasProgsGet.data != null)
        {
          // Encontrar posição correta baseada na nova data
          var progs_antes = todasProgsGet.data
              .Where(p => DateTime.Parse(p["dta_programacao_turma"].ToString()) < nova_data)
              .OrderBy(p => (int)p["nm_programacao_aux"])
              .ToList();

          novo_aux = progs_antes.Any()
              ? progs_antes.Max(p => (int)p["nm_programacao_aux"]) + 1
              : 1;
        }

        // 3. Inserir programação reprogramada
        var novaProg = new Dictionary<string, object>
        {
          ["cd_turma"] = cd_turma,
          ["nm_aula_programacao_turma"] = progOriginalGet["nm_aula_programacao_turma"],
          ["dta_programacao_turma"] = nova_data.ToString("yyyy-MM-dd"),
          ["dc_programacao_turma"] = progOriginalGet["dc_programacao_turma"],
          ["hr_inicial_programacao"] = novo_hr_inicial,
          ["hr_final_programacao"] = novo_hr_final,
          ["nm_programacao_aux"] = novo_aux,
          ["id_aula_dada"] = false,
          ["id_programacao_manual"] = false,
          ["id_reprogramada"] = false,
          ["id_provisoria"] = false,
          ["id_mostrar_calendario"] = true,
          ["dta_cadastro_programacao"] = DateTime.Now.ToString("yyyy-MM-dd"),
          ["nm_programacao_real"] = progOriginalGet["nm_programacao_real"],
          ["id_prog_cancelada"] = false,
          ["id_modificada"] = true
        };

        await SQLServerService.Insert("T_PROGRAMACAO_TURMA", novaProg, source);

        // 4. Registrar no histórico
        await RegistrarHistoricoReprogramacao(
            cd_programacao_turma,
            cd_turma,
            cd_usuario,
            data_original,
            nova_data,
            "Reprogramação sem reordenação",
            source);

        return (true, null);
      }
      catch (Exception ex)
      {
        return (false, $"Erro ao reprogramar sem reordenação: {ex.Message}");
      }
    }

    /// <summary>
    /// Calcula próxima data de aula baseada nos horários da turma
    /// </summary>
    private static DateTime CalcularProximaDataAula(DateTime ultima_data, List<HorarioTurmaModel> horarios)
    {
      var data_atual = ultima_data.AddDays(1);

      // Procurar próximo dia que tenha aula
      for (int i = 0; i < 14; i++) // Máximo 2 semanas
      {
        int dia_semana = (int)data_atual.DayOfWeek;
        if (horarios.Any(h => h.cd_dia_semana == dia_semana))
        {
          return data_atual;
        }
        data_atual = data_atual.AddDays(1);
      }

      return ultima_data.AddDays(7); // Fallback
    }

    /// <summary>
    /// Registra histórico de reprogramação
    /// </summary>
    private static async Task RegistrarHistoricoReprogramacao(
        int cd_programacao_turma,
        int cd_turma,
        int cd_usuario,
        DateTime data_original,
        DateTime data_nova,
        string tipo_reprogramacao,
        Source source)
    {
      try
      {
        var historico = new Dictionary<string, object>
        {
          ["cd_programacao_turma"] = cd_programacao_turma,
          ["cd_turma"] = cd_turma,
          ["cd_usuario"] = cd_usuario,
          ["tipo_acao"] = tipo_reprogramacao,
          ["data_original"] = data_original.ToString("yyyy-MM-dd"),
          ["data_nova"] = data_nova.ToString("yyyy-MM-dd"),
          ["descricao_alteracao"] = $"Aula reprogramada de {data_original:dd/MM/yyyy} para {data_nova:dd/MM/yyyy}",
          ["data_hora_alteracao"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };

        await SQLServerService.Insert("T_HISTORICO_PROGRAMACAO", historico, source);
      }
      catch
      {
        // Não falhar a operação se o histórico não for salvo
      }
    }

    /// <summary>
    /// Atualiza dt_final_aula da turma baseado na última programação
    /// </summary>
    private static async Task AtualizarDataFinalTurma(int cd_turma, Source source)
    {
      try
      {
        var ultimaProgGet = await SQLServerService.GetList(
            "T_PROGRAMACAO_TURMA",
            1,
            1,
            "dta_programacao_turma",
            true,
            null,
            "[cd_turma]",
            $"[{cd_turma}]",
            source,
            SearchModeEnum.Equals,
            null,
            null);

        if (ultimaProgGet.success && ultimaProgGet.data != null && ultimaProgGet.data.Any())
        {
          var ultima_data = ultimaProgGet.data.First()["dta_programacao_turma"];

          var updateTurma = new Dictionary<string, object>
          {
            ["dt_final_aula"] = ultima_data
          };

          await SQLServerService.Update(
              "T_TURMA",
              updateTurma,
              source,
              "cd_turma",
              cd_turma);
        }
      }
      catch
      {
        // Não falhar se não conseguir atualizar
      }
    }

    /// <summary>
    /// Gera automaticamente 2 programações provisórias quando penúltima aula é lançada
    /// Regra 3.5
    /// </summary>
    public static async Task<(bool success, string error)> GerarProgramacoesProvisórias(
        int cd_turma,
        Source source)
    {
      try
      {
        // Buscar última programação
        var ultimaProgGet = await SQLServerService.GetList(
            "T_PROGRAMACAO_TURMA",
            1,
            1,
            "nm_programacao_aux",
            true,
            null,
            "[cd_turma]",
            $"[{cd_turma}]",
            source,
            SearchModeEnum.Equals,
            null,
            null);

        if (!ultimaProgGet.success || ultimaProgGet.data == null || !ultimaProgGet.data.Any())
          return (false, "Última programação não encontrada");

        var ultimaProg = ultimaProgGet.data.First();
        var ultima_data = DateTime.Parse(ultimaProg["dta_programacao_turma"].ToString());
        var ultimo_aux = (int)ultimaProg["nm_programacao_aux"];
        var ultimo_num_aula = (int)ultimaProg["nm_programacao_real"];

        // Buscar horários da turma
        var horariosGet = await SQLServerService.GetList(
            "T_HORARIO",
            1,
            100,
            "cd_dia_semana",
            false,
            null,
            "[cd_turma]",
            $"[{cd_turma}]",
            source,
            SearchModeEnum.Equals,
            null,
            null);

        if (!horariosGet.success || horariosGet.data == null || !horariosGet.data.Any())
          return (false, "Horários não encontrados");

        var horarios = horariosGet.data
            .Select(h => new HorarioTurmaModel
            {
              cd_dia_semana = (int)h["cd_dia_semana"],
              hr_inicial = h["hr_inicial"].ToString(),
              hr_final = h["hr_final"].ToString()
            })
            .OrderBy(h => h.cd_dia_semana)
            .ToList();

        // Gerar 2 programações provisórias
        var data_provisoria = ultima_data;
        for (int i = 0; i < 2; i++)
        {
          data_provisoria = CalcularProximaDataAula(data_provisoria, horarios);
          var horario = horarios.First(h => h.cd_dia_semana == (int)data_provisoria.DayOfWeek);

          var progProvisoria = new Dictionary<string, object>
          {
            ["cd_turma"] = cd_turma,
            ["nm_aula_programacao_turma"] = ultimo_num_aula + i + 1,
            ["dta_programacao_turma"] = data_provisoria.ToString("yyyy-MM-dd"),
            ["dc_programacao_turma"] = "Programação Provisória",
            ["hr_inicial_programacao"] = horario.hr_inicial,
            ["hr_final_programacao"] = horario.hr_final,
            ["nm_programacao_aux"] = ultimo_aux + i + 1,
            ["id_aula_dada"] = false,
            ["id_programacao_manual"] = false,
            ["id_reprogramada"] = false,
            ["id_provisoria"] = true,
            ["id_mostrar_calendario"] = true,
            ["dta_cadastro_programacao"] = DateTime.Now.ToString("yyyy-MM-dd"),
            ["nm_programacao_real"] = ultimo_num_aula + i + 1,
            ["id_prog_cancelada"] = false,
            ["id_modificada"] = false
          };

          await SQLServerService.Insert("T_PROGRAMACAO_TURMA", progProvisoria, source);
        }

        // Atualizar dt_final_aula da turma
        await AtualizarDataFinalTurma(cd_turma, source);

        return (true, null);
      }
      catch (Exception ex)
      {
        return (false, $"Erro ao gerar programações provisórias: {ex.Message}");
      }
    }
  }
}

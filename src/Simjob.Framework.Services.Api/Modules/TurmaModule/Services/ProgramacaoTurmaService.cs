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
  public class ProgramacaoTurmaService
  {
    public async Task<(bool success, Dictionary<string, object> data, string error)> BuscarProgramacaoPorId(
      int cdProgramacaoTurma, Source source)
    {
      try
      {
        var resultado = await SQLServerService.GetList("T_PROGRAMACAO_TURMA", 1, 1,
          "dta_programacao_turma", false, null, "[cd_programacao_turma]",
          $"[{cdProgramacaoTurma}]", source, SearchModeEnum.Equals, null, null);

        if (!resultado.success || !resultado.data.Any())
          return (false, null, "Programação não encontrada");

        return (true, resultado.data.First(), null);
      }
      catch (Exception ex)
      {
        return (false, null, $"Erro: {ex.Message}");
      }
    }

    public async Task<(bool success, List<Dictionary<string, object>> data, string error)> ListarProgramacoesEnriquecidas(
      int cdTurma, Source source)
    {
      try
      {
        var programacoesGet = await SQLServerService.GetList("T_PROGRAMACAO_TURMA", 1, 10000,
          "nm_programacao_aux", false, null, "[cd_turma]", $"[{cdTurma}]",
          source, SearchModeEnum.Equals, null, null);

        if (!programacoesGet.success || programacoesGet.data == null)
          return (false, null, "Programações não encontradas");

        var programacoesEnriquecidas = new List<Dictionary<string, object>>();

        foreach (var prog in programacoesGet.data)
        {
          var progEnriquecida = new Dictionary<string, object>(prog);
          bool idAulaDada = (bool)prog["id_aula_dada"];
          bool idProgCancelada = prog.ContainsKey("id_prog_cancelada") && (bool)prog["id_prog_cancelada"];
          bool idReprogramada = (bool)prog["id_reprogramada"];
          bool idProvisoria = prog.ContainsKey("id_provisoria") && (bool)prog["id_provisoria"];
          bool isFeriado = prog.ContainsKey("cd_feriado") && prog["cd_feriado"] != null;

          string statusVisual = "aberto";
          if (idProgCancelada) statusVisual = "cancelado";
          else if (idAulaDada) statusVisual = "lancado";
          else if (isFeriado) statusVisual = "feriado";
          else if (idReprogramada) statusVisual = "reprogramado";
          else if (idProvisoria) statusVisual = "provisorio";

          progEnriquecida["status_visual"] = statusVisual;
          progEnriquecida["is_feriado"] = isFeriado;
          progEnriquecida["pode_editar"] = !idAulaDada && !idProgCancelada && !idReprogramada;
          progEnriquecida["pode_reprogramar"] = !idAulaDada && !idProgCancelada && !idReprogramada;
          progEnriquecida["pode_excluir"] = !idAulaDada && !idProgCancelada && !idProvisoria;
          programacoesEnriquecidas.Add(progEnriquecida);
        }

        var turmaInfo = await SQLServerService.GetFirstByFields(source, "T_TURMA",
          new List<(string campo, object valor)> { ("cd_turma", cdTurma) });

        if (turmaInfo != null)
        {
          var cdPessoaEscola = (int)turmaInfo["cd_pessoa_escola"];
          var comFeriados = await MarcarFeriadosNaProgramacao(programacoesEnriquecidas, cdPessoaEscola, source);
          return (true, comFeriados, null);
        }

        return (true, programacoesEnriquecidas, null);
      }
      catch (Exception ex)
      {
        return (false, null, $"Erro: {ex.Message}");
      }
    }

    public async Task<(bool success, List<ProgramacaoTurmaInsertModel> programacoes, int total_aulas, int carga_horaria_total, string error)> GerarProgramacaoAutomatica(
      GerarProgramacaoRequest request, Source source)
    {
      try
      {
        var dtInicio = DateTime.Parse(request.dt_inicio);

        var resultadoMatriz = await BuscarProgramacaoMatrizInterno(
          request.cd_curso, request.cd_produto, request.cd_regime,
          request.cd_duracao, request.cd_duracao_aula, source);

        if (resultadoMatriz.success)
        {
          var programacoes = ConverterItemsProgramacaoParaTurma(resultadoMatriz.data, dtInicio, request.horarios);
          return MontarResultado(programacoes, request.cd_duracao_aula, request.carga_horaria_total_curso);
        }

        var resultadoFranqueado = await BuscarModeloFranqueadoInterno(
          request.cd_curso, request.cd_escola, request.cd_regime, request.cd_duracao, source);

        if (resultadoFranqueado.success)
        {
          var programacoes = ConverterItemsProgramacaoParaTurma(resultadoFranqueado.data, dtInicio, request.horarios);
          return MontarResultado(programacoes, request.cd_duracao_aula, request.carga_horaria_total_curso);
        }

        var programacoesManual = GerarProgramacaoManualAutomatica(
          dtInicio, request.horarios, request.carga_horaria_total_curso, request.cd_duracao_aula);

        return MontarResultado(programacoesManual, request.cd_duracao_aula, request.carga_horaria_total_curso);
      }
      catch (Exception ex)
      {
        return (false, null, 0, 0, $"Erro: {ex.Message}");
      }
    }

    public async Task<(bool success, object data, string error)> BuscarProgramacaoMatriz(
      int cdCurso, int cdDuracao, int? cdEscola, Source source)
    {
      try
      {
        var campos = cdEscola.HasValue ? "[cd_curso],[cd_duracao],[cd_escola]" : "[cd_curso],[cd_duracao]";
        var valores = cdEscola.HasValue ? $"[{cdCurso}],[{cdDuracao}],[{cdEscola}]" : $"[{cdCurso}],[{cdDuracao}]";

        var programacaoGet = await SQLServerService.GetList("T_PROGRAMACAO_CURSO", 1, 1000,
          "cd_programacao_curso", false, null, campos, valores, source, SearchModeEnum.Equals, null, null);

        if (!programacaoGet.success || programacaoGet.data == null || !programacaoGet.data.Any())
          return (false, null, "Programação da matriz não encontrada");

        var cdProgramacaoCurso = programacaoGet.data.First()["cd_programacao_curso"];
        var itensGet = await BuscarItensProgramacao(cdProgramacaoCurso, source);

        if (!itensGet.success) return (false, null, itensGet.error);

        return (true, new { programacao = programacaoGet.data.First(), itens = itensGet.data }, null);
      }
      catch (Exception ex)
      {
        return (false, null, $"Erro: {ex.Message}");
      }
    }

    private async Task<(bool success, object data, string error)> BuscarProgramacaoMatrizInterno(
      int cdCurso, int cdProduto, int cdRegime, int cdDuracao, int cdDuracaoAula, Source source)
    {
      try
      {
        var campos = "[cd_curso],[cd_produto],[cd_regime],[cd_duracao],[cd_duracao_aula]";
        var valores = $"[{cdCurso}],[{cdProduto}],[{cdRegime}],[{cdDuracao}],[{cdDuracaoAula}]";

        var programacaoGet = await SQLServerService.GetList("T_PROGRAMACAO_CURSO", 1, 1000,
          "cd_programacao_curso", false, null, campos, valores, source, SearchModeEnum.Equals, null, null);

        if (!programacaoGet.success || programacaoGet.data == null || !programacaoGet.data.Any())
          return (false, null, "Programação da matriz não encontrada");

        var cdProgramacaoCurso = programacaoGet.data.First()["cd_programacao_curso"];
        var itensGet = await BuscarItensProgramacao(cdProgramacaoCurso, source);

        if (!itensGet.success) return (false, null, itensGet.error);

        return (true, new { programacao = programacaoGet.data.First(), itens = itensGet.data }, null);
      }
      catch (Exception ex)
      {
        return (false, null, $"Erro: {ex.Message}");
      }
    }

    private async Task<(bool success, object data, string error)> BuscarModeloFranqueadoInterno(
      int cdCurso, int cdEscola, int cdRegime, int cdDuracao, Source source)
    {
      try
      {
        var campos = "[cd_curso],[cd_escola],[cd_regime],[cd_duracao]";
        var valores = $"[{cdCurso}],[{cdEscola}],[{cdRegime}],[{cdDuracao}]";

        var programacaoGet = await SQLServerService.GetList("T_PROGRAMACAO_CURSO", 1, 1000,
          "cd_programacao_curso", false, null, campos, valores, source, SearchModeEnum.Equals, null, null);

        if (!programacaoGet.success || programacaoGet.data == null || !programacaoGet.data.Any())
          return (false, null, "Modelo de programação do franqueado não encontrado");

        var cdProgramacaoCurso = programacaoGet.data.First()["cd_programacao_curso"];
        var itensGet = await BuscarItensProgramacao(cdProgramacaoCurso, source);

        if (!itensGet.success) return (false, null, itensGet.error);

        return (true, new { programacao = programacaoGet.data.First(), itens = itensGet.data }, null);
      }
      catch (Exception ex)
      {
        return (false, null, $"Erro: {ex.Message}");
      }
    }

    private async Task<(bool success, List<Dictionary<string, object>> data, string error)> BuscarItensProgramacao(
      object cdProgramacaoCurso, Source source)
    {
      var itensGet = await SQLServerService.GetList("T_ITEM_PROGRAMACAO", 1, 10000,
        "nm_aula_programacao", false, null, "[cd_programacao_curso]",
        $"[{cdProgramacaoCurso}]", source, SearchModeEnum.Equals, null, null);

      if (!itensGet.success || itensGet.data == null || !itensGet.data.Any())
        return (false, null, "Itens da programação não encontrados");

      return (true, itensGet.data, null);
    }

    private List<ProgramacaoTurmaInsertModel> ConverterItemsProgramacaoParaTurma(
      dynamic dadosProgramacao, DateTime dtInicio, List<HorarioTurmaModel> horarios)
    {
      var programacoes = new List<ProgramacaoTurmaInsertModel>();
      var itens = dadosProgramacao.itens as IEnumerable<dynamic>;

      if (itens == null) return programacoes;

      var dataAtual = dtInicio;
      int numeroAux = 1;
      var horariosOrdenados = horarios.OrderBy(h => h.cd_dia_semana).ToList();

      foreach (var item in itens)
      {
        foreach (var horario in horariosOrdenados)
        {
          while ((int)dataAtual.DayOfWeek != horario.cd_dia_semana)
            dataAtual = dataAtual.AddDays(1);

          var programacao = new ProgramacaoTurmaInsertModel
          {
            nm_aula_programacao_turma = (int)item["nm_aula_programacao"],
            dta_programacao_turma = dataAtual.ToString("yyyy-MM-dd"),
            dc_programacao_turma = item["dc_aula_programacao"]?.ToString() ?? "",
            hr_inicial_programacao = horario.hr_inicial,
            hr_final_programacao = horario.hr_final,
            nm_programacao_aux = numeroAux,
            id_aula_dada = false,
            id_programacao_manual = false,
            id_reprogramada = false,
            id_provisoria = false,
            id_mostrar_calendario = true,
            dta_cadastro_programacao = DateTime.Now.ToString("yyyy-MM-dd"),
            nm_programacao_real = (int)item["nm_aula_programacao"],
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

    private List<ProgramacaoTurmaInsertModel> GerarProgramacaoManualAutomatica(
      DateTime dtInicio, List<HorarioTurmaModel> horarios, int cargaHorariaTotalCurso, int cargaHorariaPorAula)
    {
      var programacoes = new List<ProgramacaoTurmaInsertModel>();
      var dataAtual = dtInicio;
      int numeroAula = 1;
      int numeroAux = 1;
      int cargaHorariaAcumulada = 0;
      var horariosOrdenados = horarios.OrderBy(h => h.cd_dia_semana).ToList();

      while (cargaHorariaAcumulada < cargaHorariaTotalCurso)
      {
        foreach (var horario in horariosOrdenados)
        {
          while ((int)dataAtual.DayOfWeek != horario.cd_dia_semana)
            dataAtual = dataAtual.AddDays(1);

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

          if (cargaHorariaAcumulada >= cargaHorariaTotalCurso) break;

          dataAtual = dataAtual.AddDays(1);
        }

        if (cargaHorariaAcumulada >= cargaHorariaTotalCurso) break;
      }

      return programacoes;
    }

    private (bool success, List<ProgramacaoTurmaInsertModel> programacoes, int total_aulas, int carga_horaria_total, string error) MontarResultado(
      List<ProgramacaoTurmaInsertModel> programacoes, int cdDuracaoAula, int cargaHorariaTotalCurso)
    {
      int cargaTotal = programacoes.Count * cdDuracaoAula;

      if (cargaTotal < cargaHorariaTotalCurso)
        return (false, null, 0, 0, "Programação gerada não atinge a carga horária mínima do curso");

      return (true, programacoes, programacoes.Count, cargaTotal, null);
    }

    public async Task<(bool success, List<Dictionary<string, object>> data, string error)> BuscarFeriados(
      int ano, int? cdEscola, Source source)
    {
      try
      {
        var campos = "[aa_feriado],[id_feriado_ativo]";
        var valores = $"[{ano}],[1]";

        if (cdEscola.HasValue)
        {
          campos += ",[cd_pessoa_escola]";
          valores += $",[{cdEscola.Value}]";
        }

        var resultado = await SQLServerService.GetList("T_FERIADO", 1, 10000,
          "aa_feriado", false, null, campos, valores, source, SearchModeEnum.Equals, null, null);

        if (!resultado.success) return (false, null, resultado.error);

        return (true, resultado.data, null);
      }
      catch (Exception ex)
      {
        return (false, null, $"Erro: {ex.Message}");
      }
    }

    private async Task<List<Dictionary<string, object>>> MarcarFeriadosNaProgramacao(
      List<Dictionary<string, object>> programacoes, int cdEscola, Source source)
    {
      try
      {
        var feriadosGet = await SQLServerService.GetList("T_FERIADO", 1, 10000,
          "aa_feriado", false, null, "[cd_pessoa_escola],[id_feriado_ativo]",
          $"[{cdEscola}],[1]", source, SearchModeEnum.Equals, null, null);

        if (!feriadosGet.success || feriadosGet.data == null || !feriadosGet.data.Any())
          return programacoes;

        var feriados = new List<(DateTime data, int cod_feriado)>();

        foreach (var f in feriadosGet.data)
        {
          try
          {
            int ano = Convert.ToInt32(f["aa_feriado"]);
            int mes = Convert.ToInt32(f["mm_feriado"]);
            int dia = Convert.ToInt32(f["dd_feriado"]);
            int codFeriado = Convert.ToInt32(f["cod_feriado"]);

            var dataFeriado = new DateTime(ano, mes, dia);
            feriados.Add((dataFeriado, codFeriado));
          }
          catch
          {
            continue;
          }
        }

        foreach (var prog in programacoes)
        {
          var dataProg = DateTime.Parse(prog["dta_programacao_turma"].ToString());
          var feriado = feriados.FirstOrDefault(f => f.data.Date == dataProg.Date);

          if (feriado != default)
          {
            prog["cd_feriado"] = feriado.cod_feriado;
            prog["is_feriado"] = true;
          }
        }

        return programacoes;
      }
      catch (Exception)
      {
        return programacoes;
      }
    }

    public bool ValidarCargaHorariaMinima(
      List<ProgramacaoTurmaInsertModel> programacoes, int cargaHorariaTotalCurso, int cargaHorariaPorAula)
    {
      int cargaTotal = programacoes.Count * cargaHorariaPorAula;
      return cargaTotal >= cargaHorariaTotalCurso;
    }
  }
}

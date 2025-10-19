using System.Collections.Generic;

namespace Simjob.Framework.Services.Api.Models.Turmas
{
  /// <summary>
  /// Model para inserção de programação de turma
  /// </summary>
  public class ProgramacaoTurmaInsertModel
  {
    public int? cd_turma { get; set; }
    public int nm_aula_programacao_turma { get; set; }
    public string dta_programacao_turma { get; set; }
    public string dc_programacao_turma { get; set; }
    public string hr_inicial_programacao { get; set; }
    public string hr_final_programacao { get; set; }
    public int nm_programacao_aux { get; set; }
    public bool id_aula_dada { get; set; }
    public bool id_programacao_manual { get; set; }
    public bool id_reprogramada { get; set; }
    public bool id_provisoria { get; set; }
    public int? cd_feriado { get; set; }
    public bool id_mostrar_calendario { get; set; }
    public string dta_cadastro_programacao { get; set; }
    public int nm_programacao_real { get; set; }
    public bool id_prog_cancelada { get; set; }
    public bool id_modificada { get; set; }
  }

  /// <summary>
  /// Model para reprogramação de aula
  /// Regra 3.4
  /// </summary>
  public class ReprogramacaoAulaModel
  {
    public int cd_programacao_turma { get; set; }
    public string nova_data { get; set; }
    public string novo_hr_inicial { get; set; }
    public string novo_hr_final { get; set; }
    public int cd_turma { get; set; }
    public int cd_escola { get; set; }
    public string descricao_reprogramacao { get; set; }
  }

  /// <summary>
  /// Model para inclusão manual de programação
  /// Regra 3.6
  /// </summary>
  public class InclusaoManualProgramacaoModel
  {
    public int cd_turma { get; set; }
    public int nm_programacao_aux { get; set; }
    public string dc_programacao_turma { get; set; }
    public int cd_escola { get; set; }
  }

  /// <summary>
  /// Model para edição de programação
  /// Regra 3.7
  /// </summary>
  public class EdicaoProgramacaoModel
  {
    public int cd_programacao_turma { get; set; }
    public string dc_programacao_turma { get; set; }
    public string nova_data { get; set; }
    public string novo_hr_inicial { get; set; }
    public string novo_hr_final { get; set; }
  }

  /// <summary>
  /// Model para desconsiderar feriados
  /// Regra 3.8
  /// </summary>
  public class DesconsiderarFeriadoModel
  {
    public int cd_turma { get; set; }
    public string dt_inicial { get; set; }
    public string dt_final { get; set; }
    public bool id_programacao_feriado { get; set; }
  }

  /// <summary>
  /// Model para cancelamento de programações
  /// Regra 3.9
  /// </summary>
  public class CancelamentoProgramacaoModel
  {
    public int cd_turma { get; set; }
    public string dt_cancelamento { get; set; }
    public string motivo { get; set; }
    public int cd_usuario { get; set; }
  }

  /// <summary>
  /// Model de resposta da programação com informações adicionais
  /// </summary>
  public class ProgramacaoTurmaResponseModel
  {
    public int cd_programacao_turma { get; set; }
    public int cd_turma { get; set; }
    public int nm_aula_programacao_turma { get; set; }
    public string dta_programacao_turma { get; set; }
    public string dc_programacao_turma { get; set; }
    public string hr_inicial_programacao { get; set; }
    public string hr_final_programacao { get; set; }
    public int nm_programacao_aux { get; set; }
    public bool id_aula_dada { get; set; }
    public bool id_programacao_manual { get; set; }
    public bool id_reprogramada { get; set; }
    public bool id_provisoria { get; set; }
    public int? cd_feriado { get; set; }
    public bool is_feriado { get; set; }
    public bool id_mostrar_calendario { get; set; }
    public string dta_cadastro_programacao { get; set; }
    public int nm_programacao_real { get; set; }
    public bool id_prog_cancelada { get; set; }
    public bool id_modificada { get; set; }
    public string status_visual { get; set; } // "aberto", "lancado", "feriado", "cancelado"
    public bool pode_editar { get; set; }
    public bool pode_reprogramar { get; set; }
    public bool pode_excluir { get; set; }
  }

  /// <summary>
  /// Model para horários da turma
  /// </summary>
  public class HorarioTurmaModel
  {
    public int cd_dia_semana { get; set; } // 0=Domingo, 1=Segunda, etc
    public string hr_inicial { get; set; }
    public string hr_final { get; set; }
  }

  /// <summary>
  /// Model para validação de disponibilidade
  /// </summary>
  public class ValidacaoDisponibilidadeModel
  {
    public bool sala_disponivel { get; set; }
    public bool professor_disponivel { get; set; }
    public string mensagem_erro { get; set; }
  }

  /// <summary>
  /// Model para geração de programações provisórias
  /// Regra 3.5
  /// </summary>
  public class GerarProgramacoesProvisoriasModel
  {
    public int cd_turma { get; set; }
    public int quantidade { get; set; } = 2; // Sempre 2 conforme regra
  }

  /// <summary>
  /// Model para histórico de alterações de programação
  /// Regra 6
  /// </summary>
  public class HistoricoProgramacaoModel
  {
    public int cd_historico_programacao { get; set; }
    public int cd_programacao_turma { get; set; }
    public int cd_turma { get; set; }
    public int cd_usuario { get; set; }
    public string tipo_acao { get; set; } // "reprogramacao", "edicao", "cancelamento", "inclusao"
    public string data_original { get; set; }
    public string data_nova { get; set; }
    public string descricao_alteracao { get; set; }
    public string data_hora_alteracao { get; set; }
  }

  /// <summary>
  /// Model para request de geração de programação
  /// </summary>
  public class GerarProgramacaoRequest
  {
    public int cd_curso { get; set; }
    public int cd_produto { get; set; }
    public int cd_regime { get; set; }
    public int cd_duracao { get; set; }
    public int cd_duracao_aula { get; set; }
    public int cd_escola { get; set; }
    public string dt_inicio { get; set; }
    public List<HorarioTurmaModel> horarios { get; set; }
    public int carga_horaria_total_curso { get; set; }
  }
}

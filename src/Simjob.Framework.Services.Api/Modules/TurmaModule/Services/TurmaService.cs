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
    /// Cria e valida uma turma
    /// </summary>
    public async Task<(bool sucess, string error)> ValidateAndCreateTurma(InsertTurmaModel command, Source source)
    {
      try
      {
        //var turmaByCdPessoaEscolaTurma = await SQLServerService.GetFirstByFields(source, "T_TURMA", new List<(string campo, object valor)>
        //    {
        //        ("no_turma", command.no_turma),
        //        ("cd_pessoa_escola", command.cd_pessoa_escola)
        //    });

        //if (turmaByCdPessoaEscolaTurma != null) return new(false, $"id duplicado cd_pessoa_escola: {command.cd_pessoa_escola} para no_turma: {command.no_turma}");

        //cadastra turma
        var turmaDict = new Dictionary<string, object>
                {
                    { "cd_turma_ppt", command.cd_turma_ppt ?? null }, //id Pai
                    { "no_turma", Guid.NewGuid().ToString() }, //montar a string com base no que e recebido no corpo,padrão: REG/BRF - SEG/QUA-09:00/10:00-1S/15-1
                    { "cd_pessoa_escola", command.cd_pessoa_escola },
                    { "id_turma_ativa", command.id_turma_ativa },
                    { "cd_sala", command.cd_sala ?? null },
                    { "cd_duracao", command.cd_duracao },
                    { "cd_regime",command.cd_regime },
                    { "dt_inicio_aula",command.dt_inicio_aula },
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
                };
        if (command.cd_curso != null && command.cd_curso > 0) turmaDict.Add("cd_curso", command.cd_curso);

        var t_turma_insert = await SQLServerService.Insert("T_TURMA", turmaDict, source);
        if (!t_turma_insert.success) return new(t_turma_insert.success, t_turma_insert.error);

        var turmaCadastradaGet = await SQLServerService.GetList("T_TURMA", 1, 1, "cd_turma", true, null, null, "", source, SearchModeEnum.Equals, null, null);
        var turmaCadastrada = turmaCadastradaGet.data.First();
        int cdTurmaId = (int)turmaCadastrada["cd_turma"];
        var horarioIds = new List<int>();

        //cadastra horario
        if (!command.Horarios.IsNullOrEmpty())
        {
          foreach (var horario in command.Horarios)
          {
            var horarioDict = new Dictionary<string, object>
                        {
                            { "cd_registro", cdTurmaId},
                            { "cd_pessoa_escola", command.cd_pessoa_escola},
                            { "id_dia_semana", horario.id_dia_semana},
                            { "id_disponivel", horario.id_disponivel},
                            { "id_origem", 19},
                            { "dt_hora_ini", horario.dt_hora_ini},
                            { "dt_hora_fim", horario.dt_hora_fim},
                        };

            var t_horario_insert = await SQLServerService.InsertWithResult("T_HORARIO", horarioDict, source);
            if (!t_horario_insert.success) return new(t_horario_insert.success, t_horario_insert.error);
            var horarioInserted = t_horario_insert.inserted;

            horarioIds.Add((int)horarioInserted["cd_horario"]);
          }
        }

        //cadastra turma escola compartilhada T_TURMA_ESCOLA
        int cdEscolaId = command.cd_pessoa_escola;
        var turmaEscolaDict = new Dictionary<string, object>
                {
                    { "cd_turma", cdTurmaId},
                    { "cd_escola", cdEscolaId},
                };
        var t_turma_escola_insert = await SQLServerService.Insert("T_TURMA_ESCOLA", turmaEscolaDict, source);
        if (!t_turma_escola_insert.success) return new(t_turma_escola_insert.success, t_turma_escola_insert.error);

        // cadastra professor da turma T_PROFESSOR_TURMA
        if (!command.ProfessoresTurma.IsNullOrEmpty())
        {
          foreach (var professorTurma in command.ProfessoresTurma)
          {
            var professorTurmaDict = new Dictionary<string, object>
                        {
                            { "cd_turma", cdTurmaId},
                            { "cd_professor", professorTurma.cd_professor},
                            { "id_professor_ativo", professorTurma.id_professor_ativo},
                        };

            var t_professor_turma_insert = await SQLServerService.Insert("T_PROFESSOR_TURMA", professorTurmaDict, source);
            if (!t_professor_turma_insert.success) return new(t_professor_turma_insert.success, t_professor_turma_insert.error);
          }
        }

        // cadastra alunos da turma T_ALUNO_TURMA
        if (!command.AlunosTurma.IsNullOrEmpty())
        {
          foreach (var alunoTurma in command.AlunosTurma)
          {
            var alunoTurmaDict = new Dictionary<string, object>
                        {
                            { "cd_turma", cdTurmaId},
                            { "cd_aluno", alunoTurma.cd_aluno},
                            { "cd_situacao_aluno_turma", alunoTurma.cd_situacao_aluno_turma},
                            { "cd_curso", alunoTurma.cd_curso},
                            { "dt_inicio", alunoTurma.dt_inicio},
                            { "dt_movimento", alunoTurma.dt_movimento},
                        };

            var t_aluno_turma_insert = await SQLServerService.Insert("T_ALUNO_TURMA", alunoTurmaDict, source);
            if (!t_aluno_turma_insert.success) return new(t_aluno_turma_insert.success, t_aluno_turma_insert.error);
          }
        }

        if (!command.ProgramacaoTurma.IsNullOrEmpty())
        {
          foreach (var programacaoTurma in command.ProgramacaoTurma)
          {
            //cadastra programação turma T_PROGRAMACAO_TURMA
            var programacaoTurmaDict = new Dictionary<string, object>
                        {
                            { "cd_turma", cdTurmaId},
                            { "nm_aula_programacao_turma", programacaoTurma.nm_aula_programacao_turma},
                            { "dta_programacao_turma", programacaoTurma.dta_programacao_turma},
                            { "dc_programacao_turma", programacaoTurma.dc_programacao_turma},
                            { "hr_inicial_programacao", programacaoTurma.hr_inicial_programacao},
                            { "hr_final_programacao", programacaoTurma.hr_final_programacao},
                            { "nm_programacao_aux", programacaoTurma.nm_programacao_aux},
                            { "id_aula_dada", programacaoTurma.id_aula_dada},
                            { "id_programacao_manual", programacaoTurma.id_programacao_manual},
                            { "id_reprogramada", programacaoTurma.id_reprogramada},
                            { "id_provisoria", programacaoTurma.id_provisoria},
                            { "cd_feriado", programacaoTurma.cd_feriado},
                            { "id_mostrar_calendario", programacaoTurma.id_mostrar_calendario},
                            { "dta_cadastro_programacao", programacaoTurma.dta_cadastro_programacao},
                            { "nm_programacao_real", programacaoTurma.nm_programacao_real},
                            { "id_prog_cancelada", programacaoTurma.id_prog_cancelada},
                            { "id_modificada", programacaoTurma.id_modificada},
                        };

            var t_programacao_turma_insert = await SQLServerService.Insert("T_PROGRAMACAO_TURMA", programacaoTurmaDict, source);
            if (!t_programacao_turma_insert.success) return new(t_programacao_turma_insert.success, t_programacao_turma_insert.error);
          }
        }

        //  insert feriado desconsiderado T_FERIADO_DESCONSIDERADO
        if (command.FeriadoDesconsiderado != null)
        {
          var feriadoDesconsideradoDict = new Dictionary<string, object>
                    {
                        { "cd_turma", cdTurmaId},
                        { "dt_inicial", command.FeriadoDesconsiderado.dt_inicial},
                        { "dt_final", command.FeriadoDesconsiderado.dt_final},
                        { "id_programacao_feriado", command.FeriadoDesconsiderado.id_programacao_feriado},
                    };

          var t_feriado_desconsiderado_insert = await SQLServerService.Insert("T_FERIADO_DESCONSIDERADO", feriadoDesconsideradoDict, source);
          if (!t_feriado_desconsiderado_insert.success) return new(t_feriado_desconsiderado_insert.success, t_feriado_desconsiderado_insert.error);
        }

        //  insert Professor Horario Turma T_HORARIO_PROFESSOR_TURMA
        if (!horarioIds.IsNullOrEmpty() && !command.ProfessoresTurma.IsNullOrEmpty())
        {
          var professorIds = command.ProfessoresTurma.Select(x => x.cd_professor).Distinct().ToList();
          foreach (var professorId in professorIds)
          {
            foreach (var horarioId in horarioIds)
            {
              var horarioProfessorTurmaDict = new Dictionary<string, object>
                            {
                                { "cd_horario", horarioId},
                                { "cd_professor", professorId},
                            };

              var t_horario_professor_turma_insert = await SQLServerService.Insert("T_HORARIO_PROFESSOR_TURMA", horarioProfessorTurmaDict, source);
              if (!t_horario_professor_turma_insert.success) return new(t_horario_professor_turma_insert.success, t_horario_professor_turma_insert.error);
            }
          }
        }

        #region atualiza nome da turma

        // pega view do nome da turma filtrando cd_turma e monta nome da turma
        var viewNomeTurma = await SQLServerService.GetFirstByFields(source, "vi_nome_turma", new List<(string campo, object valor)>
                {
                    ("cd_turma", cdTurmaId),
                    ("cd_pessoa_escola", command.cd_pessoa_escola)
                });
        if (viewNomeTurma == null) return new(false, "turma não encontrada na view vi_nome_turma");
        string no_turma = viewNomeTurma["no_turma_formatado"] != null ? viewNomeTurma["no_turma_formatado"].ToString() : "";
        // verifica se existe turma com nome montado
        var turmaByName = new Dictionary<string, object>();
        int nm_turma = 1;
        if (!string.IsNullOrEmpty(no_turma))
        {
          while (turmaByName != null)
          {
            turmaByName = await SQLServerService.GetFirstByFields(source, "T_TURMA", new List<(string campo, object valor)>
                        {
                            ("no_turma", no_turma + nm_turma.ToString()),
                            ("cd_pessoa_escola", command.cd_pessoa_escola)
                        });

            if (turmaByName != null) nm_turma++;
          }
          // atualiza no_turma e nm_turma
          no_turma = no_turma + nm_turma.ToString();

          var turmaDictUpdate = new Dictionary<string, object>
                    {
                        { "nm_turma", nm_turma },
                        { "no_turma", no_turma },
                    };

          var t_turma_update = await SQLServerService.Update("T_TURMA", turmaDictUpdate, source, "cd_turma", cdTurmaId);
          if (!t_turma_update.success) return new(t_turma_update.success, t_turma_update.error);
        }

        #endregion atualiza nome da turma
      }
      catch (Exception ex)
      {
        return (false, $"Erro: {ex.Message}");
      }

      return (true, string.Empty);
    }

    public async Task<(bool sucess, string error)> UpdateTurma(int cd_turma, InsertTurmaModel command, Source source)
    {
      try
      {
        //var turmaExiste = await SQLServerService.GetFirstByFields(source, "T_TURMA", new List<(string campo, object valor)>
        //    {
        //        ("cd_turma", cd_turma),
        //    });
        //if (turmaExiste == null) return new(false, "Turma não encontrada");

        //var turmaByCdPessoaEscolaTurma = await SQLServerService.GetFirstByFields(source, "T_TURMA", new List<(string campo, object valor)>
        //    {
        //        ("no_turma", command.no_turma),
        //        ("cd_pessoa_escola", command.cd_pessoa_escola)
        //    });

        //if (turmaByCdPessoaEscolaTurma != null && (int)turmaByCdPessoaEscolaTurma["cd_turma"] != (int)turmaExiste["cd_turma"]) return new(false, $"id duplicado cd_pessoa_escola: {command.cd_pessoa_escola} para no_turma: {command.no_turma}");

        //update turma
        var turmaDict = new Dictionary<string, object>
                {
                    { "cd_turma_ppt", command.cd_turma_ppt ?? null }, //id Pai
                    //  { "no_turma", command.no_turma },
                    { "cd_pessoa_escola", command.cd_pessoa_escola },
                    { "id_turma_ativa", command.id_turma_ativa },
                    { "cd_sala", command.cd_sala ?? null },
                    { "cd_duracao", command.cd_duracao },
                    { "cd_regime",command.cd_regime },
                    { "dt_inicio_aula",command.dt_inicio_aula },
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
                };
        if (command.cd_curso != null && command.cd_curso > 0) turmaDict.Add("cd_curso", command.cd_curso);

        var t_titulo_update = await SQLServerService.Update("T_TURMA", turmaDict, source, "cd_turma", cd_turma);
        if (!t_titulo_update.success) return new(t_titulo_update.success, t_titulo_update.error);

        var horarioIds = new List<int>();
        //cadastra horario

        var horariosToDeleteResult = await SQLServerService.GetListIn("vi_horario_turma", 1, 10000000, "cd_turma", true, null, null, null, source, SearchModeEnum.Contains, null, null, "cd_turma", new List<string>() { cd_turma.ToString() });
        if (!command.Horarios.IsNullOrEmpty())
        {
          foreach (var horario in command.Horarios)
          {
            var horarioDict = new Dictionary<string, object>
                        {
                            { "cd_registro", cd_turma},
                            { "cd_pessoa_escola", command.cd_pessoa_escola},
                            { "id_dia_semana", horario.id_dia_semana},
                            { "id_disponivel", horario.id_disponivel},
                            { "id_origem", 19}, //19
                            { "dt_hora_ini", horario.dt_hora_ini},
                            { "dt_hora_fim", horario.dt_hora_fim},
                        };

            var t_horario_insert = await SQLServerService.InsertWithResult("T_HORARIO", horarioDict, source);
            if (!t_horario_insert.success) return new(t_horario_insert.success, t_horario_insert.error);
            var horarioInserted = t_horario_insert.inserted;

            horarioIds.Add((int)horarioInserted["cd_horario"]);
          }
        }

        // cadastra professor da turma T_PROFESSOR_TURMA
        var turmaCadastradaGet = await SQLServerService.GetList("T_TURMA", 1, 1, "cd_turma", true, null, null, "", source, SearchModeEnum.Equals, null, null);
        var turmaCadastrada = turmaCadastradaGet.data.First();
        int cdTurmaId = (int)turmaCadastrada["cd_turma"];

        if (!command.ProfessoresTurma.IsNullOrEmpty())
        {
          //remover todas os professorTurmas por cd_turma e cd_professor
          await SQLServerService.Delete("T_PROFESSOR_TURMA", "cd_turma", cdTurmaId.ToString(), source);

          foreach (var professorTurma in command.ProfessoresTurma)
          {
            var professorTurmaDict = new Dictionary<string, object>
                        {
                            { "cd_turma", cdTurmaId},
                            { "cd_professor", professorTurma.cd_professor},
                            { "id_professor_ativo", professorTurma.id_professor_ativo},
                        };

            var t_professor_turma_insert = await SQLServerService.Insert("T_PROFESSOR_TURMA", professorTurmaDict, source);
            if (!t_professor_turma_insert.success) return new(t_professor_turma_insert.success, t_professor_turma_insert.error);
          }
        }

        // cadastra alunos da turma T_ALUNO_TURMA
        if (!command.AlunosTurma.IsNullOrEmpty())
        {
          //remover todas os alunoTurmas por cd_turma
          await SQLServerService.Delete("T_ALUNO_TURMA", "cd_turma", cdTurmaId.ToString(), source);
          foreach (var alunoTurma in command.AlunosTurma)
          {
            var alunoTurmaDict = new Dictionary<string, object>
                        {
                            { "cd_turma", cdTurmaId},
                            { "cd_aluno", alunoTurma.cd_aluno},
                            { "cd_situacao_aluno_turma", alunoTurma.cd_situacao_aluno_turma},
                            { "cd_curso", alunoTurma.cd_curso},
                            { "dt_inicio", alunoTurma.dt_inicio},
                            { "dt_movimento", alunoTurma.dt_movimento},
                        };

            var t_aluno_turma_insert = await SQLServerService.Insert("T_ALUNO_TURMA", alunoTurmaDict, source);
            if (!t_aluno_turma_insert.success) return new(t_aluno_turma_insert.success, t_aluno_turma_insert.error);
          }
        }

        if (!command.ProgramacaoTurma.IsNullOrEmpty())
        {
          //remover programação existente pelo id da turma
          await SQLServerService.Delete("T_PROGRAMACAO_TURMA", "cd_turma", cdTurmaId.ToString(), source);

          foreach (var programacaoTurma in command.ProgramacaoTurma)
          {
            //cadastra programação turma T_PROGRAMACAO_TURMA
            var programacaoTurmaDict = new Dictionary<string, object>
                        {
                            { "cd_turma", cdTurmaId},
                            { "nm_aula_programacao_turma", programacaoTurma.nm_aula_programacao_turma},
                            { "dta_programacao_turma", programacaoTurma.dta_programacao_turma},
                            { "dc_programacao_turma", programacaoTurma.dc_programacao_turma},
                            { "hr_inicial_programacao", programacaoTurma.hr_inicial_programacao},
                            { "hr_final_programacao", programacaoTurma.hr_final_programacao},
                            { "nm_programacao_aux", programacaoTurma.nm_programacao_aux},
                            { "id_aula_dada", programacaoTurma.id_aula_dada},
                            { "id_programacao_manual", programacaoTurma.id_programacao_manual},
                            { "id_reprogramada", programacaoTurma.id_reprogramada},
                            { "id_provisoria", programacaoTurma.id_provisoria},
                            { "cd_feriado", programacaoTurma.cd_feriado},
                            { "id_mostrar_calendario", programacaoTurma.id_mostrar_calendario},
                            { "dta_cadastro_programacao", programacaoTurma.dta_cadastro_programacao},
                            { "nm_programacao_real", programacaoTurma.nm_programacao_real},
                            { "id_prog_cancelada", programacaoTurma.id_prog_cancelada},
                            { "id_modificada", programacaoTurma.id_modificada},
                        };

            var t_programacao_turma_insert = await SQLServerService.Insert("T_PROGRAMACAO_TURMA", programacaoTurmaDict, source);
            if (!t_programacao_turma_insert.success) return new(t_programacao_turma_insert.success, t_programacao_turma_insert.error);
          }
        }

        //  insert feriado desconsiderado T_FERIADO_DESCONSIDERADO
        if (command.FeriadoDesconsiderado != null)
        {
          //remover feriado considerado pelo id da turma
          await SQLServerService.Delete("T_FERIADO_DESCONSIDERADO", "cd_turma", cdTurmaId.ToString(), source);

          var feriadoDesconsideradoDict = new Dictionary<string, object>
                    {
                        { "cd_turma", cdTurmaId},
                        { "dt_inicial", command.FeriadoDesconsiderado.dt_inicial},
                        { "dt_final", command.FeriadoDesconsiderado.dt_final},
                        { "id_programacao_feriado", command.FeriadoDesconsiderado.id_programacao_feriado},
                    };

          var t_feriado_desconsiderado_insert = await SQLServerService.Insert("T_FERIADO_DESCONSIDERADO", feriadoDesconsideradoDict, source);
          if (!t_feriado_desconsiderado_insert.success) return new(t_feriado_desconsiderado_insert.success, t_feriado_desconsiderado_insert.error);
        }

        //  insert Professor Horario Turma T_HORARIO_PROFESSOR_TURMA
        if (!horarioIds.IsNullOrEmpty() && !command.ProfessoresTurma.IsNullOrEmpty())
        {
          var professorIds = command.ProfessoresTurma.Select(x => x.cd_professor).Distinct().ToList();

          //remover todas os professorTurmas por cd_turma
          await SQLServerService.Delete("T_HORARIO_PROFESSOR_TURMA", "cd_turma", cdTurmaId.ToString(), source);

          foreach (var professorId in professorIds)
          {
            foreach (var horarioId in horarioIds)
            {
              var horarioProfessorTurmaDict = new Dictionary<string, object>
                            {
                                { "cd_horario", horarioId},
                                { "cd_professor", professorId},
                            };

              var t_horario_professor_turma_insert = await SQLServerService.Insert("T_HORARIO_PROFESSOR_TURMA", horarioProfessorTurmaDict, source);
              if (!t_horario_professor_turma_insert.success) return new(t_horario_professor_turma_insert.success, t_horario_professor_turma_insert.error);
            }
          }
        }
        //deleta horarios anteriores se existir
        if (horariosToDeleteResult.data.Count() > 0 && command.Horarios != null)
        {
          foreach (var horario in horariosToDeleteResult.data)
          {
            await SQLServerService.Delete("T_HORARIO", "cd_horario", horario["cd_horario"].ToString(), source);
          }
        }
      }
      catch (Exception ex)
      {
        return (false, $"Erro: {ex.Message}");
      }

      return (true, string.Empty);
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

        // 2. Buscar turmas compartilhadas
        var query = @"
        SELECT t.*, COUNT(*) OVER() as total_records
        FROM vi_turma t
        WHERE (
            t.cd_pessoa_escola = '" + cdEmpresa.Replace("'", "''") + @"'
            OR EXISTS (
                SELECT 1
                FROM T_TURMA_ESCOLA te
                WHERE te.cd_turma = t.cd_turma
                  AND te.cd_escola = '" + cdEmpresa.Replace("'", "''") + @"'
            )
        )";

        // Adicionar filtros adicionais
        var whereConditions = new List<string>();

        // Filtro por IDs específicos
        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => $"'{id.Trim()}'");
            whereConditions.Add($"t.cd_turma IN ({string.Join(",", idList)})");
        }

        // Filtro por campos de busca (usando a mesma lógica do GetList)
        if (!string.IsNullOrWhiteSpace(searchFields) && !string.IsNullOrWhiteSpace(value))
        {
            // Extrai cada "[item1,item2,...]" em listas separadas
            var fieldGroups = System.Text.RegularExpressions.Regex.Matches(searchFields, @"\[(.*?)\]")
                                    .Cast<System.Text.RegularExpressions.Match>()
                                    .Select(m => m.Groups[1].Value.Split(',')
                                                                      .Select(f => f.Trim())
                                                                      .ToList())
                                    .ToList();

            var valueGroups = System.Text.RegularExpressions.Regex.Matches(value, @"\[(.*?)\]")
                                    .Cast<System.Text.RegularExpressions.Match>()
                                    .Select(m => m.Groups[1].Value.Split(',')
                                                                      .Select(v => v.Trim())
                                                                      .ToList())
                                    .ToList();

            // Para cada grupo (até o menor número de grupos entre fields e values)
            var groupCount = Math.Min(fieldGroups.Count, valueGroups.Count);
            for (int i = 0; i < groupCount; i++)
            {
                var fields = fieldGroups[i];
                var vals = valueGroups[i];
                var innerConds = new List<string>();

                // Cross‑product: para cada campo e cada valor
                foreach (var f in fields)
                {
                    // Se o campo começa com dt_, tratar o valor completo como um range de data
                    if (f.StartsWith("dt_", StringComparison.OrdinalIgnoreCase))
                    {
                        // Para campos de data, usar o valor original completo (pode conter vírgula para range)
                        var originalValue = valueGroups[i].Count > 0 ? string.Join(",", valueGroups[i]) : "";
                        
                        if (originalValue == "null")
                        {
                            innerConds.Add($"t.[{f}] is null");
                        }
                        else if (originalValue == "not null")
                        {
                            innerConds.Add($"t.[{f}] is not null");
                        }
                        else
                        {
                            var dates = originalValue.Split(',');
                            if (dates.Length == 2)
                            {
                                // Busca entre duas datas
                                var dateStart = dates[0].Trim();
                                var dateEnd = dates[1].Trim();
                                innerConds.Add($"(CAST(t.[{f}] AS DATE) >= '{dateStart}' AND CAST(t.[{f}] AS DATE) <= '{dateEnd}')");
                            }
                            else
                            {
                                // Busca por data específica
                                innerConds.Add($"CAST(t.[{f}] AS DATE) = '{originalValue.Trim()}'");
                            }
                        }
                    }
                    else
                    {
                        foreach (var v in vals)
                        {
                            if (v == "null")
                            {
                                innerConds.Add($"t.[{f}] is null");
                            }
                            else if (v == "not null")
                            {
                                innerConds.Add($"t.[{f}] is not null");
                            }
                            else if (f.StartsWith("cd_", StringComparison.OrdinalIgnoreCase))
                            {
                                // Se o campo inicia com "cd_", sempre usar igualdade
                                innerConds.Add($"t.[{f}] = '{v.Replace("'", "''")}'");
                            }
                            else
                            {
                                // Aplicar o modo de busca
                                if (mode == SearchModeEnum.Contains)
                                {
                                    innerConds.Add($"t.[{f}] LIKE '%{v.Replace("'", "''")}%'");
                                }
                                else if (mode == SearchModeEnum.Equals)
                                {
                                    innerConds.Add($"t.[{f}] = '{v.Replace("'", "''")}'");
                                }
                            }
                        }
                    }
                }

                if (innerConds.Any())
                {
                    // Agrupa com OR e envolve em parênteses
                    whereConditions.Add($"({string.Join(" OR ", innerConds)})");
                }
            }
        }

        // Aplicar condições WHERE adicionais
        if (whereConditions.Any())
        {
            query += " AND (" + string.Join(" AND ", whereConditions) + ")";
        }

        // Filtro por data (se não foi incluído nos searchFields)
        if (dataInicio.HasValue)
        {
            query += $" AND t.dt_inicio_aula >= '{dataInicio.Value:yyyy-MM-dd}'";
        }

        if (dataFim.HasValue)
        {
            query += $" AND t.dt_final_aula <= '{dataFim.Value:yyyy-MM-dd}'";
        }

        // Aplicar ordenação
        query += $" ORDER BY t.{sortField} {(sortDesc ? "DESC" : "ASC")}";

        // Aplicar paginação
        if (page.HasValue && limit.HasValue && page > 0 && limit > 0)
        {
            var offset = (page.Value - 1) * limit.Value;
            query += $" OFFSET {offset} ROWS FETCH NEXT {limit.Value} ROWS ONLY";
        }

        // Executar a query customizada
        var turmasResult = await SQLServerService.ExecuteQuery(source, query);

        if (!turmasResult.Success)
        {
          return (false, null, 0, 0, "Erro ao buscar turmas");
        }

        // Calcular total de registros do primeiro resultado (se houver)
        int totalRecords = 0;
        if (turmasResult.Data.Any())
        {
          totalRecords = turmasResult.Data.First().ContainsKey("total_records") 
            ? Convert.ToInt32(turmasResult.Data.First()["total_records"]) 
            : turmasResult.Data.Count;
        }

        // 3. Aplicar filtros adicionais de professor e horário (pós-processamento)
        var turmasFiltradas = turmasResult.Data;

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

        // 4. Recalcular páginas com base no total original
        var totalFiltrado = totalRecords; // Usar o total da query original
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
            searchFields: "[id_professor_ativo]",
            value: "[1]",
            source,
            mode,
            null,
            null,
            "cd_turma",
            turmaIds
        );

        // 8. Buscar alunos das turmas
        var alunosResult = await SQLServerService.GetListIn(
            schemaName:"vi_turma_aluno",
            page:1,
            limit:10000000,
            sortField:"cd_turma",
            sortDesc:true,
            ids: null,
            searchFields:"[cd_situacao_aluno_turma]",
            value:"[1]",
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

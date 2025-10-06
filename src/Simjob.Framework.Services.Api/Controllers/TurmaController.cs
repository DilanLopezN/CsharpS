using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
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
using Simjob.Framework.Services.Api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Simjob.Framework.Services.Api.Controllers
{
  public class TurmaController : BaseController
  {
    private readonly IRepository<SourceContext, Source> _sourceRepository;
    private readonly IRepository<MongoDbContext, Schema> _schemaRepository;

    public TurmaController(IMediatorHandler bus, INotificationHandler<DomainNotification> notifications, IRepository<SourceContext, Source> sourceRepository, IRepository<MongoDbContext, Schema> schemaRepository) : base(bus, notifications)
    {
      _sourceRepository = sourceRepository;
      _schemaRepository = schemaRepository;
    }

    private async Task<(bool sucess, string error)> ValidateCommand(InsertTurmaModel command, Source source)
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
                    { "nm_carga_horaria_aula", command.nm_carga_horaria_aula ?? null },
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

    [Authorize]
    [HttpPost()]
    public async Task<IActionResult> Insert([FromBody] InsertTurmaModel command)
    {
      var schemaName = "T_Turma";
      if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
      var schema = _schemaRepository.GetSchemaByField("name", schemaName);
      var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
      var source = _sourceRepository.GetByField("description", schemaModel.Source);
      if (source != null && source.Active != null && source.Active == true)
      {
        var resultReturn = await ValidateCommand(command, source);
        var result = new
        {
          resultReturn.sucess,
          resultReturn.error
        };
        return resultReturn.sucess ? ResponseDefault(result) : BadRequest(result);
      }
      return BadRequest(new
      {
        error = "Fonte de dados não configurada ou inativa."
      });
    }

    private async Task<(bool sucess, string error)> ProcessaUpdate(int cd_turma, InsertTurmaModel command, Source source)
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
                    { "nm_carga_horaria_aula", command.nm_carga_horaria_aula ?? null }
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

    [Authorize]
    [HttpPut()]
    [Route("{cd_turma}")]
    public async Task<IActionResult> Update([FromBody] InsertTurmaModel command, int cd_turma)
    {
      var schemaName = "T_Turma";
      if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
      var schema = _schemaRepository.GetSchemaByField("name", schemaName);
      var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
      var source = _sourceRepository.GetByField("description", schemaModel.Source);
      if (source != null && source.Active != null && source.Active == true)
      {
        var resultReturn = await ProcessaUpdate(cd_turma, command, source);
        var result = new
        {
          resultReturn.sucess,
          resultReturn.error
        };
        return resultReturn.sucess ? ResponseDefault(result) : BadRequest(result);
      }
      return BadRequest(new
      {
        error = "Fonte de dados não configurada ou inativa."
      });
    }

    [Authorize]
    [HttpPost()]
    [Route("PreTurma")]
    public async Task<IActionResult> InsertAlunoTurma([FromBody] InsertAlunoTurmaPreTurma model)
    {
      var schemaName = "T_Turma";
      if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
      var schema = _schemaRepository.GetSchemaByField("name", schemaName);
      var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
      var source = _sourceRepository.GetByField("description", schemaModel.Source);
      if (source != null && source.Active != null && source.Active == true)
      {
        var filtros = new List<(string campo, object valor)> { new("cd_turma", model.cd_turma), new("cd_aluno", model.cd_aluno) };
        var alunoTurma = await SQLServerService.GetFirstByFields(source, "T_ALUNO_TURMA", filtros);
        if (alunoTurma == null)
        {
          var alunoTurmaDict = new Dictionary<string, object>
                     {
                         { "cd_turma", model.cd_turma},
                         { "cd_aluno", model.cd_aluno},
                         { "cd_contrato", model.cd_contrato},
                         { "id_tipo_movimento", 1},
                         { "id_reprovado", 0},
                         { "nm_aulas_dadas", 0},
                         { "nm_faltas", 0},
                         { "id_manter_contrato", 1},
                         { "id_renegociacao", 0},
                         { "cd_curso_contrato", model.cd_curso}
                     };

          var t_aluno_turma_insert = await SQLServerService.Insert("T_ALUNO_TURMA", alunoTurmaDict, source);
          if (!t_aluno_turma_insert.success) return BadRequest(t_aluno_turma_insert.error);
        }
        else
        {
          var cd_aluno_turma = alunoTurma["cd_aluno_turma"];
          alunoTurma.Remove("cd_aluno_turma");
          alunoTurma["cd_contrato"] = model.cd_contrato;
          var t_aluno_turma_update = await SQLServerService.Update("T_ALUNO_TURMA", alunoTurma, source, "cd_aluno_turma", cd_aluno_turma);
          if (!t_aluno_turma_update.success) return BadRequest(t_aluno_turma_update.error);
        }
        return ResponseDefault();
      }
      return BadRequest(new
      {
        error = "Fonte de dados não configurada ou inativa."
      });
    }

    [Authorize]
    [HttpGet()]
    public async Task<IActionResult> GetAll(string value, SearchModeEnum mode, int? page, int? limit, string sortField, bool sortDesc = false, string ids = "", string searchFields = null, string? cd_empresa = null, bool filtrarProgramacao = false, DateTime? dataInicio = null, DateTime? dataFim = null)
    {
      if (cd_empresa == null) return BadRequest("campo cd_empresa não informado");

      var schemaName = "T_Turma";
      if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
      var schema = _schemaRepository.GetSchemaByField("name", schemaName);
      var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
      var source = _sourceRepository.GetByField("description", schemaModel.Source);
      if (string.IsNullOrEmpty(sortField)) sortField = "cd_turma";
      if (source != null && source.Active != null && source.Active == true)
      {
        if (filtrarProgramacao)
        {
          if (string.IsNullOrEmpty(searchFields) && string.IsNullOrEmpty(value))
          {
            searchFields = "[possui_programacao]";

            value = $"[1]";
          }
          else
          {
            searchFields = searchFields + ",[possui_programacao]";

            value = value + $",[1]";
          }
        }
        var turmasResult = await SQLServerService.GetListFiltroData("vi_turma", page, limit, sortField, sortDesc, ids, "cd_turma", searchFields, value, source, mode, "cd_pessoa_escola", cd_empresa, "dt_inicio_aula", "dt_final_aula", dataInicio, dataFim);
        var turmaIds = turmasResult.data.Select(x => x["cd_turma"].ToString()).ToList();

        var horarios = await SQLServerService.GetListIn("T_HORARIO", 1, 10000000, "cd_registro", true, null, null, null, source, mode, null, null, "cd_registro", turmaIds);
        var professoresTurmaResult = await SQLServerService.GetListIn("vi_professor_turma", 1, 10000000, "cd_turma", true, null, null, null, source, mode, null, null, "cd_turma", turmaIds);

        var alunosResult = await SQLServerService.GetListIn("vi_turma_aluno", 1, 10000000, "cd_turma", true, null, null, null, source, mode, null, null, "cd_turma", turmaIds);

        if (turmasResult.success)
        {
          var turmas = turmasResult.data;

          var retorno = new
          {
            data = turmas.Select(x => new
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
              dt_final_aula = x["dt_inicio_aula"],
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
              nm_carga_horaria_aula = x["nm_carga_horaria_aula"],
              dc_situacao_turma = x["dc_situacao_turma"],
              nm_vaga_sala = x["nm_vaga_sala"],
              alunos_matriculados = alunosResult.data?.Where(a => a.ContainsKey("cd_turma") && (int)a["cd_turma"] == (int)x["cd_turma"]).Count(),
              professores = professoresTurmaResult.data?.Where(z => z.ContainsKey("cd_turma") && (int)z["cd_turma"] == (int)x["cd_turma"]).Select(y => new
              {
                cd_professor_turma = y["cd_professor_turma"],
                cd_professor = y["cd_professor"],
                no_professor = y["no_professor"],
                cd_turma = y["cd_turma"],
              }).ToList(),
              horarios = horarios.data?.Where(h => h.ContainsKey("cd_registro") && (int)h["cd_registro"] == (int)x["cd_turma"]).Select(h => new
              {
                id_dia_semana = h["id_dia_semana"],
                cd_horario = h["cd_horario"],
                dt_hora_ini = h["dt_hora_ini"],
                dt_hora_fim = h["dt_hora_fim"],
              }).ToList()
            }),
            turmasResult.total,
            page,
            limit,
            pages = limit != null ? (int)Math.Ceiling((double)turmasResult.total / limit.Value) : 0
          };

          return ResponseDefault(retorno);
        }
        return BadRequest(new
        {
          sucess = false,
          error = turmasResult.error
        });
      }
      return BadRequest(new
      {
        error = "Fonte de dados não configurada ou inativa."
      });
    }

    [Authorize]
    [HttpGet()]
    [Route("{cd_turma}")]
    public async Task<IActionResult> Get(int cd_turma)
    {
      var schemaName = "T_Turma";
      if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
      var schema = _schemaRepository.GetSchemaByField("name", schemaName);
      var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
      var source = _sourceRepository.GetByField("description", schemaModel.Source);
      if (source != null && source.Active != null && source.Active == true)
      {
        var filtros = new List<(string campo, object valor)> { new("cd_turma", cd_turma) };
        var turma = await SQLServerService.GetFirstByFields(source, "vi_turma", filtros);
        if (turma == null) return NotFound();
        var horariosResult = await SQLServerService.GetListIn("vi_horario_turma", 1, 10000000, "cd_turma", true, null, null, null, source, SearchModeEnum.Contains, null, null, "cd_turma", new List<string>() { turma["cd_turma"].ToString() });

        var programacoesResult = await SQLServerService.GetListIn("T_PROGRAMACAO_TURMA", 1, 10000000, "cd_turma", true, null, null, null, source, SearchModeEnum.Contains, null, null, "cd_turma", new List<string>() { turma["cd_turma"].ToString() });
        var retorno = new
        {
          cd_turma = turma["cd_turma"],
          cd_turma_ppt = turma["cd_turma_ppt"],
          no_turma = turma["no_turma"],
          cd_pessoa_escola = turma["cd_pessoa_escola"],
          id_turma_ativa = turma["id_turma_ativa"],
          cd_curso = turma["cd_curso"],
          cd_sala = turma["cd_sala"],
          cd_duracao = turma["cd_duracao"],
          cd_regime = turma["cd_regime"],
          dt_inicio_aula = turma["dt_inicio_aula"],
          dt_final_aula = turma["dt_inicio_aula"],
          id_aula_externa = turma["id_aula_externa"],
          nro_aulas_programadas = turma["nro_aulas_programadas"],
          id_turma_ppt = turma["id_turma_ppt"],
          cd_produto = turma["cd_produto"],
          nm_turma = turma["nm_turma"],
          dt_termino_turma = turma["dt_termino_turma"],
          no_apelido = turma["no_apelido"],
          id_percentual_faltas = turma["id_percentual_faltas"],
          cd_sala_online = turma["cd_sala_online"],
          dc_situacao_turma = turma["dc_situacao_turma"],

          horariosAulas = horariosResult.data?.Where(x => x.ContainsKey("cd_turma")).Select(x => new
          {
            dia_semana = x["diaSemana"],
            cd_horario = x["cd_horario"],
            dt_hora_ini = x["dt_hora_ini"],
            dt_hora_fim = x["dt_hora_fim"],
          }).ToList(),
          programacoesTurma = programacoesResult.data?.Where(x => x.ContainsKey("cd_turma")).ToList()
        };
        return ResponseDefault(retorno);
      }
      return BadRequest(new
      {
        error = "Fonte de dados não configurada ou inativa."
      });
    }

    [Authorize]
    [HttpGet("PreTurma")]
    public async Task<IActionResult> GetAllPreTurma(string value, SearchModeEnum mode, int? page, int? limit, string sortField, bool sortDesc = false, string ids = "", string searchFields = null, string? cd_empresa = null)
    {
      if (cd_empresa == null) return BadRequest("campo cd_empresa não informado");

      var schemaName = "T_Turma";
      if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
      var schema = _schemaRepository.GetSchemaByField("name", schemaName);
      var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
      var source = _sourceRepository.GetByField("description", schemaModel.Source);
      if (string.IsNullOrEmpty(sortField)) sortField = "cd_pessoa_escola";
      if (source != null && source.Active != null && source.Active == true)
      {
        var turmasResult = await SQLServerService.GetList("vi_pre_turma", page, limit, sortField, sortDesc, ids, searchFields, value, source, mode, "cd_pessoa_escola", cd_empresa);
        if (turmasResult.success)
        {
          var turmas = turmasResult.data;

          var retorno = new
          {
            data = turmas.Select(x => new
            {
              cd_pessoa_escola = x["cd_pessoa_escola"],
              cd_aluno = x["cd_aluno"],
              cd_contrato = x["cd_contrato"],
              nm_contrato = x["nm_contrato"],
              no_aluno = x["no_aluno"],
              cd_curso = x["cd_curso"],
              no_curso = x["no_curso"],
              cd_duracao = x["cd_duracao"],
              dc_duracao = x["dc_duracao"],
              cd_regime = x["cd_regime"],
              no_regime = x["no_regime"],
              cd_produto = x["cd_produto"],
              no_produto = x["no_produto"],
              dt_inicial_contrato = x["dt_inicial_contrato"],
              id_tipo_matricula = x["id_tipo_matricula"],
              id_status_contrato = x["id_status_contrato"],
            }),
            turmasResult.total,
            page,
            limit,
            pages = limit != null ? (int)Math.Ceiling((double)turmasResult.total / limit.Value) : 0
          };

          return ResponseDefault(retorno);
        }
        return BadRequest(new
        {
          sucess = false,
          error = turmasResult.error
        });
      }
      return BadRequest(new
      {
        error = "Fonte de dados não configurada ou inativa."
      });
    }

    [Authorize]
    [HttpGet("ProfessoresByTurma")]
    public async Task<IActionResult> GetAllProfessoresByTurma(string? cd_turma = null, string? cd_empresa = null)
    {
      if (cd_empresa == null) return BadRequest("campo cd_empresa não informado");
      if (cd_turma == null) return BadRequest("campo cd_turma não informado");

      var schemaName = "T_Turma";
      if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
      var schema = _schemaRepository.GetSchemaByField("name", schemaName);
      var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
      var source = _sourceRepository.GetByField("description", schemaModel.Source);
      if (source != null && source.Active != null && source.Active == true)
      {
        var fieldIn = "cd_turma";
        var sortField = "no_professor";
        var valueIn = cd_turma;
        var turmasResult = await SQLServerService.GetListIn("vi_professor_turma", 1, 1000000, sortField, false, null, null, null, source, SearchModeEnum.Contains, "cd_pessoa_escola", cd_empresa, fieldIn, new List<string>() { valueIn });
        if (turmasResult.success)
        {
          var turmas = turmasResult.data;

          var retorno = new
          {
            data = turmas.Select(x => new
            {
              cd_professor = x["cd_professor"],
              no_professor = x["no_professor"],
              cd_turma = x["cd_turma"],
              cd_turma_ppt = x["cd_turma_ppt"],
              no_turma = x["no_turma"],
              //tipo_funcionario = x["tipo_funcionario"],
              //tipo_funcionario_desc = x["tipo_funcionario_desc"],
              cd_pessoa_escola = x["cd_pessoa_escola"],
              id_turma_ativa = x["id_turma_ativa"],
              cd_curso = x["cd_curso"],
              cd_sala = x["cd_sala"],
              cd_duracao = x["cd_duracao"],
              cd_regime = x["cd_regime"],
              dt_inicio_aula = x["dt_inicio_aula"],
              dt_final_aula = x["dt_final_aula"],
              id_aula_externa = x["id_aula_externa"],
              nro_aulas_programadas = x["nro_aulas_programadas"],
              id_turma_ppt = x["id_turma_ppt"],
              cd_produto = x["cd_produto"],
              nm_turma = x["nm_turma"],
              no_apelido = x["no_apelido"],
              id_percentual_faltas = x["id_percentual_faltas"],
            }),
            turmasResult.total,
          };

          return ResponseDefault(retorno);
        }
        return BadRequest(new
        {
          sucess = false,
          error = turmasResult.error
        });
      }
      return BadRequest(new
      {
        error = "Fonte de dados não configurada ou inativa."
      });
    }

    [Authorize]
    [HttpGet("Professores")]
    public async Task<IActionResult> GetAllProfessores(string value, SearchModeEnum mode, int? page, int? limit, string sortField, bool sortDesc = false, string ids = "", string searchFields = null, string? cd_empresa = null)
    {
      if (cd_empresa == null) return BadRequest("campo cd_empresa não informado");

      var schemaName = "T_Turma";
      if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
      var schema = _schemaRepository.GetSchemaByField("name", schemaName);
      var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
      var source = _sourceRepository.GetByField("description", schemaModel.Source);
      if (string.IsNullOrEmpty(sortField)) sortField = "cd_pessoa_escola";
      if (source != null && source.Active != null && source.Active == true)
      {
        var professorResult = await SQLServerService.GetList("vi_professor", page, limit, sortField, sortDesc, ids, searchFields, value, source, mode, "cd_empresa", cd_empresa);
        if (professorResult.success)
        {
          var professores = professorResult.data;

          var retorno = new
          {
            data = professores,
            professorResult.total,
            page,
            limit,
            pages = limit != null ? (int)Math.Ceiling((double)professorResult.total / limit.Value) : 0
          };

          return ResponseDefault(retorno);
        }
        return BadRequest(new
        {
          sucess = false,
          error = professorResult.error
        });
      }
      return BadRequest(new
      {
        error = "Fonte de dados não configurada ou inativa."
      });
    }

    [Authorize]
    [HttpGet("AlunosByTurma")]
    public async Task<IActionResult> GetAllAlunosByTurma(string? cd_turma = null)
    {
      if (cd_turma == null) return BadRequest("campo cd_turma não informado");

      var schemaName = "T_Turma";
      if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
      var schema = _schemaRepository.GetSchemaByField("name", schemaName);
      var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
      var source = _sourceRepository.GetByField("description", schemaModel.Source);
      if (source != null && source.Active != null && source.Active == true)
      {
        var fieldIn = "cd_turma";
        var sortField = "cd_aluno";
        var valueIn = cd_turma;
        var alunosResult = await SQLServerService.GetListIn("vi_turma_aluno", 1, 1000000, sortField, false, null, null, null, source, SearchModeEnum.Contains, null, null, fieldIn, new List<string>() { valueIn });
        if (alunosResult.success)
        {
          var turmas = alunosResult.data;

          var retorno = new
          {
            data = turmas.Select(x => new
            {
              cd_aluno_turma = x["cd_aluno_turma"],
              cd_aluno = x["cd_aluno"],
              cd_turma = x["cd_turma"],
              cd_situacao_aluno_origem = x["cd_situacao_aluno_origem"],
              no_aluno = x["no_aluno"]
            }),
            alunosResult.total,
          };

          return ResponseDefault(retorno);
        }
        return BadRequest(new
        {
          sucess = false,
          error = alunosResult.error
        });
      }
      return BadRequest(new
      {
        error = "Fonte de dados não configurada ou inativa."
      });
    }

    [Authorize]
    [HttpPatch()]
    [Route("{cd_turma}")]
    public async Task<IActionResult> Patch(int cd_turma)
    {
      var schemaName = "T_Turma";
      if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
      var schema = _schemaRepository.GetSchemaByField("name", schemaName);
      var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
      var source = _sourceRepository.GetByField("description", schemaModel.Source);
      if (source != null && source.Active != null && source.Active == true)
      {
        var filtrosTurma = new List<(string campo, object valor)> { new("cd_turma", cd_turma) };
        var turmaExists = await SQLServerService.GetFirstByFields(source, "T_TURMA", filtrosTurma);
        if (turmaExists == null) return NotFound("turma não encontrada");
        var turmaDict = new Dictionary<string, object>();

        if ((bool)turmaExists["id_turma_ativa"] == false)
        {
          turmaDict = new Dictionary<string, object>
                    {
                        { "id_turma_ativa", true },
                        { "dt_termino_turma", null }
                    };
        }
        else
        {
          turmaDict = new Dictionary<string, object>
                    {
                        { "id_turma_ativa", false }
                    };
        }

        var t_pessoa = await SQLServerService.Update("T_TURMA", turmaDict, source, "cd_turma", cd_turma);
        if (!t_pessoa.success) return BadRequest(t_pessoa.error);
        return ResponseDefault();
      }
      return BadRequest(new
      {
        error = "Fonte de dados não configurada ou inativa."
      });
    }

    [Authorize]
    [HttpPost("AlunoTurma")]
    public async Task<IActionResult> InsertAlunoTurma([FromBody] InsertAlunoTurmaRelationModel command)
    {
      var schemaName = "T_Turma";
      if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
      var schema = _schemaRepository.GetSchemaByField("name", schemaName);
      var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
      var source = _sourceRepository.GetByField("description", schemaModel.Source);
      if (source != null && source.Active != null && source.Active == true)
      {
        var alunoTurmaDict = new Dictionary<string, object>
                {
                    { "cd_turma", command.cd_turma},
                    { "cd_aluno", command.cd_aluno},
                    { "cd_situacao_aluno_turma", command.cd_situacao_aluno_turma},
                    { "cd_curso", command.cd_curso},
                    { "dt_inicio", command.dt_inicio},
                    { "dt_movimento", command.dt_movimento},
                };

        var t_aluno_turma_insert = await SQLServerService.Insert("T_ALUNO_TURMA", alunoTurmaDict, source);

        var result = new
        {
          sucess = t_aluno_turma_insert.success,
          error = t_aluno_turma_insert.error
        };
        return result.sucess ? ResponseDefault(result) : BadRequest(result);
      }
      return BadRequest(new
      {
        error = "Fonte de dados não configurada ou inativa."
      });
    }

    /// <summary>
    /// retorna proxima programação de uma turma
    /// </summary>
    [Authorize]
    [HttpGet()]
    [Route("programacao/{cd_turma}")]
    public async Task<IActionResult> GetProgramacao(int cd_turma)
    {
      var schemaName = "T_Turma";
      if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
      var schema = _schemaRepository.GetSchemaByField("name", schemaName);
      var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
      var source = _sourceRepository.GetByField("description", schemaModel.Source);
      if (source != null && source.Active != null && source.Active == true)
      {
        var campos = "[cd_turma],[id_aula_dada]";
        var valores = $"[{cd_turma}],[0]";
        var proximaProgramacaoGet = await SQLServerService.GetList("T_PROGRAMACAO_TURMA", 1, 1, "dta_programacao_turma", false, null, campos, valores, source, SearchModeEnum.Equals, null, null);
        if (!proximaProgramacaoGet.success || proximaProgramacaoGet.data.IsNullOrEmpty()) ResponseDefault();

        var proximaProgramacao = proximaProgramacaoGet.data.First();

        var filtrosTurma = new List<(string campo, object valor)> { new("cd_turma", cd_turma) };
        var professorTurmaGet = await SQLServerService.GetFirstByFields(source, "vi_professor_turma", filtrosTurma);
        if (professorTurmaGet != null)
        {
          proximaProgramacao.Add("cd_professor", professorTurmaGet["cd_professor"]);
          proximaProgramacao.Add("no_professor", professorTurmaGet["no_professor"]);
        }

        return ResponseDefault(proximaProgramacao);
      }

      return BadRequest(new
      {
        error = "Fonte de dados não configurada ou inativa."
      });
    }

    /// <summary>
    /// retorna programação de turma por Id
    /// </summary>
    [Authorize]
    [HttpGet()]
    [Route("programacao")]
    public async Task<IActionResult> GetProgramacaoId(int cd_programacao_turma)
    {
      var schemaName = "T_Turma";
      if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
      var schema = _schemaRepository.GetSchemaByField("name", schemaName);
      var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
      var source = _sourceRepository.GetByField("description", schemaModel.Source);
      if (source != null && source.Active != null && source.Active == true)
      {
        var campos = "[cd_programacao_turma]";
        var valores = $"[{cd_programacao_turma}]";
        var programacaoGet = await SQLServerService.GetList("T_PROGRAMACAO_TURMA", 1, 1, "dta_programacao_turma", false, null, campos, valores, source, SearchModeEnum.Equals, null, null);
        if (!programacaoGet.success || programacaoGet.data.IsNullOrEmpty()) return NotFound();

        var proximaProgramacao = programacaoGet.data.First();
        return ResponseDefault(proximaProgramacao);
      }

      return BadRequest(new
      {
        error = "Fonte de dados não configurada ou inativa."
      });
    }

    /// <summary>
    /// Busca programação da matriz (fundação) - T_PROGRAMACAO_CURSO
    /// Regra 3.2 - Primeira prioridade para geração automática
    /// </summary>
    [Authorize]
    [HttpGet]
    [Route("programacao-matriz")]
    public async Task<IActionResult> GetProgramacaoMatriz(
        int cd_curso,
        int cd_duracao,
        int? cd_escola = null)
    {
      var schemaName = "T_Turma";
      if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
      var schema = _schemaRepository.GetSchemaByField("name", schemaName);
      var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
      var source = _sourceRepository.GetByField("description", schemaModel.Source);

      if (source != null && source.Active != null && source.Active == true)
      {
        var result = await ProgramacaoService.BuscarProgramacaoMatriz(
            cd_curso,
            cd_duracao,
            cd_escola,
            source
        );

        if (!result.success)
          return NotFound(new { error = result.error });

        return ResponseDefault(result.data);
      }

      return BadRequest(new { error = "Fonte de dados não configurada ou inativa." });
    }

    /// <summary>
    /// Busca feriados do ano - T_FERIADO
    /// Regra 3.3 - Tratamento de Feriados
    /// </summary>
    [Authorize]
    [HttpGet]
    [Route("feriados")]
    public async Task<IActionResult> GetFeriados(int ano, int? cd_escola = null)
    {
      var schemaName = "T_Turma";
      if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
      var schema = _schemaRepository.GetSchemaByField("name", schemaName);
      var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
      var source = _sourceRepository.GetByField("description", schemaModel.Source);

      if (source != null && source.Active != null && source.Active == true)
      {
        var result = await ProgramacaoService.BuscarFeriados(ano, cd_escola, source);

        if (!result.success)
          return NotFound(new { error = result.error });

        return ResponseDefault(result.data);
      }

      return BadRequest(new { error = "Fonte de dados não configurada ou inativa." });
    }
    private async Task<List<object>> VerificarDisponibilidadeProfessor(
    object cd_pessoa,
    DateTime data,
    Source source,
    string cd_empresa)
    {
      int diaSemana = MapearDiaSemana(data);

      // 1. Buscar cd_professor a partir de cd_pessoa
      var professorResult = await SQLServerService.GetList(
          "vi_professor",
          null,
          null,
          "cd_professor",
          false,
          "",
          "[cd_pessoa]",
          $"[{cd_pessoa}]",
          source,
          SearchModeEnum.Equals,
          null,
          null
      );

      if (!professorResult.success || professorResult.data == null || !professorResult.data.Any())
      {
        return new List<object>();
      }

      var cd_professor = professorResult.data.First()["cd_professor"];

      // 2. Buscar TODOS os horários cadastrados do professor para o dia da semana
      // T_HORARIO com id_origem = 18 (professor) e cd_pessoa_escola = cd_pessoa
      var horariosResult = await SQLServerService.GetList(
          "T_HORARIO",
          null,
          null,
          "dt_hora_ini",
          false,
          "",
          "[cd_pessoa_escola],[id_origem],[id_dia_semana]",
          $"[{cd_pessoa}],[18],[{diaSemana}]",
          source,
          SearchModeEnum.Equals,
          null,
          null
      );

      if (!horariosResult.success || horariosResult.data == null || !horariosResult.data.Any())
      {
        return new List<object>(); // Professor não tem horários cadastrados neste dia
      }

      var horariosDisponiveis = new List<object>();

      // 3. Para cada horário do professor neste dia, verificar se está livre
      foreach (var horario in horariosResult.data)
      {
        var cd_horario = horario["cd_horario"];

        // Verificar se este horário está vinculado a alguma turma ativa
        bool estaOcupado = await VerificarSeHorarioTemTurmaAtiva(
            cd_horario,
            cd_professor,
            source
        );

        horariosDisponiveis.Add(new
        {
          cd_horario = cd_horario,
          dia_semana = diaSemana,
          dia_semana_nome = ObterNomeDiaSemana(diaSemana),
          dt_hora_ini = horario.ContainsKey("dt_hora_ini") ? horario["dt_hora_ini"] : null,
          dt_hora_fim = horario.ContainsKey("dt_hora_fim") ? horario["dt_hora_fim"] : null,
          disponivel = !estaOcupado,
          id_disponivel = horario.ContainsKey("id_disponivel") ? horario["id_disponivel"] : null
        });
      }

      return horariosDisponiveis;
    }







    private async Task<bool> VerificarSeHorarioTemTurmaAtiva(
object cd_horario,
object cd_professor,
Source source)
    {
      // 1. Verificar se este horário está em T_HORARIO_PROFESSOR_TURMA
      // Isso significa que o horário foi alocado para alguma turma
      var vinculoResult = await SQLServerService.GetList(
          "T_HORARIO_PROFESSOR_TURMA",
          null,
          null,
          "cd_horario_professor_turma",
          false,
          "",
          "[cd_horario],[cd_professor]",
          $"[{cd_horario}],[{cd_professor}]",
          source,
          SearchModeEnum.Equals,
          null,
          null
      );

      if (!vinculoResult.success || vinculoResult.data == null || !vinculoResult.data.Any())
      {
        return false; // Horário não está vinculado a nenhuma turma = DISPONÍVEL
      }

      // 2. Se o horário está vinculado, verificar se o professor está ativo em alguma turma
      var professorTurmaResult = await SQLServerService.GetList(
          "T_PROFESSOR_TURMA",
          null,
          null,
          "cd_professor_turma",
          false,
          "",
          "[cd_professor],[id_professor_ativo]",
          $"[{cd_professor}],[1]", // id_professor_ativo = 1 (ativo)
          source,
          SearchModeEnum.Equals,
          null,
          null
      );

      if (!professorTurmaResult.success || professorTurmaResult.data == null || !professorTurmaResult.data.Any())
      {
        return false; // Professor não está ativo em nenhuma turma = DISPONÍVEL
      }

      // 3. Verificar se alguma das turmas ativas está usando este horário
      foreach (var professorTurma in professorTurmaResult.data)
      {
        if (!professorTurma.ContainsKey("cd_turma")) continue;

        var cd_turma = professorTurma["cd_turma"];

        // Verificar se esta turma usa este horário (via vi_horario_turma)
        var horarioTurmaResult = await SQLServerService.GetList(
            "vi_horario_turma",
            null,
            null,
            "cd_horario",
            false,
            "",
            "[cd_turma],[cd_horario]",
            $"[{cd_turma}],[{cd_horario}]",
            source,
            SearchModeEnum.Equals,
            null,
            null
        );

        if (horarioTurmaResult.success &&
            horarioTurmaResult.data != null &&
            horarioTurmaResult.data.Any())
        {
          // Verificar se a turma está ativa (id_turma_ativa = 1)
          var turmaResult = await SQLServerService.GetList(
              "T_TURMA",
              null,
              null,
              "cd_turma",
              false,
              "",
              "[cd_turma],[id_turma_ativa]",
              $"[{cd_turma}],[1]", // id_turma_ativa = 1 (ativa)
              source,
              SearchModeEnum.Equals,
              null,
              null
          );

          if (turmaResult.success &&
              turmaResult.data != null &&
              turmaResult.data.Any())
          {
            return true; // OCUPADO - horário está sendo usado por turma ativa
          }
        }
      }

      return false; // DISPONÍVEL - horário não está sendo usado ativamente
    }
    private string ObterNomeDiaSemana(int diaSemana)
    {
      return diaSemana switch
      {
        1 => "Segunda-feira",
        2 => "Terça-feira",
        3 => "Quarta-feira",
        4 => "Quinta-feira",
        5 => "Sexta-feira",
        6 => "Sábado",
        7 => "Domingo",
        _ => "Desconhecido"
      };
    }
    private async Task<bool> VerificarSeHorarioEstaOcupado(
        object cd_horario,
        DateTime data,
        Source source)
    {
      // Verifica na tabela T_HORARIO_PROFESSOR_TURMA se existe vínculo com turma
      var vinculoResult = await SQLServerService.GetList(
          "T_HORARIO_PROFESSOR_TURMA",
          1,
          1,
          "cd_horario_professor_turma",
          false,
          null,
          "[cd_horario]",
          $"[{cd_horario}]",
          source,
          SearchModeEnum.Equals,
          null,
          null
      );

      if (!vinculoResult.success || vinculoResult.data == null || !vinculoResult.data.Any())
      {
        return false; // Não tem vínculo = disponível
      }

      // Se tem vínculo, verifica se há programação para esta data
      foreach (var vinculo in vinculoResult.data)
      {
        if (!vinculo.ContainsKey("cd_turma")) continue;

        var cd_turma = vinculo["cd_turma"];

        // Verifica se existe programação nesta data
        var programacaoResult = await SQLServerService.GetList(
            "T_PROGRAMACAO_TURMA",
            1,
            1,
            "cd_programacao_turma",
            false,
            null,
            "[cd_turma],[dta_programacao_turma]",
            $"[{cd_turma}],[{data:yyyy-MM-dd}]",
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




    private async Task<object> ProcessarDisponibilidadeProfessores(
     string cd_empresa,
     DateTime data,
     Source source)
    {
      try
      {
        // 1. Buscar todos os professores usando vi_professor (que tem as colunas certas)
        // Colunas disponíveis: cd_professor, cd_empresa, no_pessoa, cd_pessoa
        var professorResult = await SQLServerService.GetList(
            "vi_professor",      // View que tem os dados completos
            null,                // page
            null,                // limit
            "cd_professor",      // sortField - existe na view!
            false,               // sortDesc
            "",                  // ids
            null,                // searchFields
            "",                  // value
            source,
            SearchModeEnum.Contains,
            "cd_empresa",        // Campo correto de filtro
            cd_empresa
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
            cd_empresa = cd_empresa
          };
        }

        var listaProfessoresDisponiveis = new List<object>();

        // 2. Para cada professor, verificar disponibilidade
        foreach (var professor in professorResult.data)
        {
          if (!professor.ContainsKey("cd_professor")) continue;

          var cd_professor = professor["cd_professor"];
          var no_pessoa = professor.ContainsKey("no_pessoa") ? professor["no_pessoa"] : "";
          var cd_pessoa = professor.ContainsKey("cd_pessoa") ? professor["cd_pessoa"] : null;

          // Verificar disponibilidade do professor
          var disponibilidade = await VerificarDisponibilidadeProfessor(
              cd_pessoa,  // Usar cd_pessoa pois T_HORARIO usa cd_pessoa_escola
              data,
              source,
              cd_empresa
          );

          listaProfessoresDisponiveis.Add(new
          {
            cd_professor = cd_professor,
            cd_pessoa = cd_pessoa,
            no_pessoa = no_pessoa,
            cd_empresa = professor.ContainsKey("cd_empresa") ? professor["cd_empresa"] : null,
            horarios_disponiveis = disponibilidade,
            total_horarios = disponibilidade.Count
          });
        }

        return new
        {
          success = true,
          data = listaProfessoresDisponiveis,
          data_consultada = data.ToString("yyyy-MM-dd"),
          dia_semana = ObterNomeDiaSemana(MapearDiaSemana(data)),
          total_professores = listaProfessoresDisponiveis.Count,
          cd_empresa = cd_empresa
        };
      }
      catch (Exception ex)
      {
        return new
        {
          success = false,
          error = $"Erro ao processar disponibilidade: {ex.Message}",
          stack_trace = ex.StackTrace
        };
      }
    }

    private int MapearDiaSemana(DateTime data)
    {
      // Mapear DayOfWeek do C# para o formato do banco
      // DayOfWeek: Sunday=0, Monday=1, Tuesday=2, Wednesday=3, Thursday=4, Friday=5, Saturday=6
      // Banco: Segunda=1, Terça=2, Quarta=3, Quinta=4, Sexta=5, Sábado=6, Domingo=7

      return data.DayOfWeek switch
      {
        DayOfWeek.Sunday => 7,    // Domingo
        DayOfWeek.Monday => 1,    // Segunda-feira
        DayOfWeek.Tuesday => 2,   // Terça-feira
        DayOfWeek.Wednesday => 3, // Quarta-feira
        DayOfWeek.Thursday => 4,  // Quinta-feira
        DayOfWeek.Friday => 5,    // Sexta-feira
        DayOfWeek.Saturday => 6,  // Sábado
        _ => 1
      };
    }


    [Authorize]
    [HttpGet("Professores/Disponibilidade")]
    public async Task<IActionResult> GetDisponibilidadeProfessores(
    string cd_empresa,
    DateTime data)
    {
      if (string.IsNullOrEmpty(cd_empresa))
        return BadRequest("campo cd_empresa não informado");

      var schemaName = "T_Turma";
      if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
      var schema = _schemaRepository.GetSchemaByField("name", schemaName);
      var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
      var source = _sourceRepository.GetByField("description", schemaModel.Source);

      if (source == null || source.Active != true)
      {
        return BadRequest(new { error = "Fonte de dados não configurada ou inativa." });
      }

      // Chama o método de processamento
      var resultado = await ProcessarDisponibilidadeProfessores(cd_empresa, data, source);

      return ResponseDefault(resultado);
    }


    /// <summary>
    /// Gera programação automática para turma
    /// Implementa regra 3.2 (prioridade: Matriz → Franqueado → Manual)
    /// </summary>
    [Authorize]
    [HttpPost()]
    [Route("gerar-programacao")]
    public async Task<IActionResult> GerarProgramacao([FromBody] GerarProgramacaoRequest request)
    {
      var schemaName = "T_Turma";
      if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
      var schema = _schemaRepository.GetSchemaByField("name", schemaName);
      var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
      var source = _sourceRepository.GetByField("description", schemaModel.Source);

      if (source != null && source.Active != null && source.Active == true)
      {
        var dt_inicio = DateTime.Parse(request.dt_inicio);

        var resultado = await ProgramacaoTurmaService.GerarProgramacaoTurma(
            request.cd_curso,
            request.cd_produto,
            request.cd_regime,
            request.cd_duracao,
            request.cd_duracao_aula,
            request.cd_escola,
            dt_inicio,
            request.horarios,
            request.carga_horaria_total_curso,
            source
        );

        if (!resultado.success)
          return BadRequest(new { error = resultado.error });

        // Validar carga horária mínima
        var validaCarga = ProgramacaoTurmaService.ValidarCargaHorariaMinima(
            resultado.programacoes,
            request.carga_horaria_total_curso,
            request.cd_duracao_aula
        );

        if (!validaCarga)
          return BadRequest(new { error = "Programação gerada não atinge a carga horária mínima do curso" });

        return ResponseDefault(new
        {
          programacoes = resultado.programacoes,
          total_aulas = resultado.programacoes.Count,
          carga_horaria_total = resultado.programacoes.Count * request.cd_duracao_aula
        });
      }

      return BadRequest(new { error = "Fonte de dados não configurada ou inativa." });
    }


    /// <summary>
    /// Lista todas programações de uma turma com informações adicionais
    /// </summary>
    [Authorize]
    [HttpGet()]
    [Route("programacoes/{cd_turma}")]
    public async Task<IActionResult> ListarProgramacoes(int cd_turma)
    {
      var schemaName = "T_Turma";
      if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
      var schema = _schemaRepository.GetSchemaByField("name", schemaName);
      var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
      var source = _sourceRepository.GetByField("description", schemaModel.Source);

      if (source != null && source.Active != null && source.Active == true)
      {
        // Buscar todas programações da turma
        var programacoesGet = await SQLServerService.GetList(
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

        if (!programacoesGet.success || programacoesGet.data == null)
          return NotFound(new { error = "Programações não encontradas" });

        // Enriquecer dados com informações visuais
        var programacoesEnriquecidas = new List<Dictionary<string, object>>();

        foreach (var prog in programacoesGet.data)
        {
          var progEnriquecida = new Dictionary<string, object>(prog);

          bool id_aula_dada = (bool)prog["id_aula_dada"];
          bool id_prog_cancelada = prog.ContainsKey("id_prog_cancelada") && (bool)prog["id_prog_cancelada"];
          bool id_reprogramada = (bool)prog["id_reprogramada"];
          bool id_provisoria = prog.ContainsKey("id_provisoria") && (bool)prog["id_provisoria"];
          bool is_feriado = prog.ContainsKey("cd_feriado") && prog["cd_feriado"] != null;

          // Definir status visual
          string status_visual = "aberto";
          if (id_prog_cancelada) status_visual = "cancelado";
          else if (id_aula_dada) status_visual = "lancado";
          else if (is_feriado) status_visual = "feriado";
          else if (id_reprogramada) status_visual = "reprogramado";
          else if (id_provisoria) status_visual = "provisorio";

          progEnriquecida["status_visual"] = status_visual;
          progEnriquecida["is_feriado"] = is_feriado;
          progEnriquecida["pode_editar"] = !id_aula_dada && !id_prog_cancelada && !id_reprogramada;
          progEnriquecida["pode_reprogramar"] = !id_aula_dada && !id_prog_cancelada && !id_reprogramada;
          progEnriquecida["pode_excluir"] = !id_aula_dada && !id_prog_cancelada && !id_provisoria;

          programacoesEnriquecidas.Add(progEnriquecida);
        }

        // Marcar feriados
        var comFeriados = await ProgramacaoTurmaService.MarcarFeriadosNaProgramacao(
            programacoesEnriquecidas,
            (int)(await SQLServerService.GetFirstByFields(source, "T_TURMA",
                new List<(string campo, object valor)> { ("cd_turma", cd_turma) }))["cd_pessoa_escola"],
            source
        );

        return ResponseDefault(comFeriados);
      }

      return BadRequest(new { error = "Fonte de dados não configurada ou inativa." });
    }


    /// <summary>
    /// Reprograma uma aula (com ou sem reordenação)
    /// Implementa regra 3.4
    /// </summary>
    [Authorize]
    [HttpPost()]
    [Route("reprogramar-aula")]
    public async Task<IActionResult> ReprogramarAula([FromBody] ReprogramacaoAulaModel request)
    {
      var schemaName = "T_Turma";
      if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
      var schema = _schemaRepository.GetSchemaByField("name", schemaName);
      var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
      var source = _sourceRepository.GetByField("description", schemaModel.Source);

      if (source != null && source.Active != null && source.Active == true)
      {
        // Buscar programação original
        var progOriginalGet = await SQLServerService.GetFirstByFields(
            source,
            "T_PROGRAMACAO_TURMA",
            new List<(string campo, object valor)> { ("cd_programacao_turma", request.cd_programacao_turma) });

        if (progOriginalGet == null)
          return NotFound(new { error = "Programação não encontrada" });

        var data_original = DateTime.Parse(progOriginalGet["dta_programacao_turma"].ToString());
        var nova_data = DateTime.Parse(request.nova_data);

        // Validar reprogramação
        var validacao = await ReprogramacaoAulaService.ValidarReprogramacao(
            request.cd_programacao_turma,
            data_original,
            nova_data,
            request.cd_turma,
            source
        );

        if (!validacao.valido)
          return BadRequest(new { error = validacao.erro });

        // Buscar turma para validar disponibilidade
        var turmaGet = await SQLServerService.GetFirstByFields(
            source,
            "T_TURMA",
            new List<(string campo, object valor)> { ("cd_turma", request.cd_turma) });

        if (turmaGet == null)
          return NotFound(new { error = "Turma não encontrada" });

        int cd_sala = turmaGet.ContainsKey("cd_sala") && turmaGet["cd_sala"] != null ? (int)turmaGet["cd_sala"] : 0;

        // Buscar professor da turma
        var profTurmaGet = await SQLServerService.GetFirstByFields(
            source,
            "T_TURMA_PROFESSOR",
            new List<(string campo, object valor)> { ("cd_turma", request.cd_turma) });

        int? cd_professor = profTurmaGet != null ? (int?)profTurmaGet["cd_professor"] : null;

        // Validar disponibilidade
        var disponibilidade = await ReprogramacaoAulaService.ValidarDisponibilidade(
            nova_data,
            request.novo_hr_inicial,
            request.novo_hr_final,
            cd_sala,
            cd_professor,
            request.cd_turma,
            source
        );

        if (!disponibilidade.sala_disponivel || !disponibilidade.professor_disponivel)
          return BadRequest(new { error = disponibilidade.mensagem_erro });

        // Buscar próxima programação disponível para decidir tipo de reprogramação
        var proximaProgGet = await SQLServerService.GetList(
            "T_PROGRAMACAO_TURMA",
            1,
            1,
            "dta_programacao_turma",
            false,
            null,
            "[cd_turma],[id_aula_dada],[dta_programacao_turma]",
            $"[{request.cd_turma}],[0],[{data_original:yyyy-MM-dd}]",
            source,
            SearchModeEnum.GreaterThan,
            null,
            null);

        bool tem_proxima_aula = proximaProgGet.success && proximaProgGet.data != null && proximaProgGet.data.Any();
        bool move_para_proxima = false;

        if (tem_proxima_aula)
        {
          var proxima_data = DateTime.Parse(proximaProgGet.data.First()["dta_programacao_turma"].ToString());
          // Se a nova data é igual ou posterior à próxima aula, precisa reordenar
          move_para_proxima = nova_data >= proxima_data;
        }

        // Obter cd_usuario do token/contexto (implementar conforme sua autenticação)
        int cd_usuario = 1; // TODO: Obter do token JWT

        // Executar reprogramação
        (bool success, string error) resultado;

        if (move_para_proxima)
        {
          resultado = await ReprogramacaoAulaService.ReprogramarComReordenacao(
              request.cd_programacao_turma,
              nova_data,
              request.novo_hr_inicial,
              request.novo_hr_final,
              request.cd_turma,
              cd_usuario,
              source
          );
        }
        else
        {
          resultado = await ReprogramacaoAulaService.ReprogramarSemReordenacao(
              request.cd_programacao_turma,
              nova_data,
              request.novo_hr_inicial,
              request.novo_hr_final,
              request.cd_turma,
              cd_usuario,
              source
          );
        }

        if (!resultado.success)
          return BadRequest(new { error = resultado.error });

        return ResponseDefault(new
        {
          message = "Aula reprogramada com sucesso",
          tipo_reprogramacao = move_para_proxima ? "com_reordenacao" : "sem_reordenacao"
        });
      }

      return BadRequest(new { error = "Fonte de dados não configurada ou inativa." });
    }


    /// <summary>
    /// Inclui programação manual
    /// Implementa regra 3.6
    /// </summary>
    [Authorize]
    [HttpPost()]
    [Route("incluir-programacao-manual")]
    public async Task<IActionResult> IncluirProgramacaoManual([FromBody] InclusaoManualProgramacaoModel request)
    {
      var schemaName = "T_Turma";
      if (schemaName.Contains("T_")) schemaName = schemaName.Replace("T_", "");
      var schema = _schemaRepository.GetSchemaByField("name", schemaName);
      var schemaModel = JsonConvert.DeserializeObject<Infra.Domain.Models.SchemaModel>(schema.JsonValue);
      var source = _sourceRepository.GetByField("description", schemaModel.Source);

      if (source != null && source.Active != null && source.Active == true)
      {
        // Buscar todas programações da turma
        var todasProgsGet = await SQLServerService.GetList(
            "T_PROGRAMACAO_TURMA",
            1,
            10000,
            "nm_programacao_aux",
            false,
            null,
            "[cd_turma]",
            $"[{request.cd_turma}]",
            source,
            SearchModeEnum.Equals,
            null,
            null);

        if (!todasProgsGet.success || todasProgsGet.data == null)
          return BadRequest(new { error = "Erro ao buscar programações" });

        // Validar que não há diários lançados após o auxiliar informado
        var progsPosteriores = todasProgsGet.data
            .Where(p => (int)p["nm_programacao_aux"] >= request.nm_programacao_aux)
            .ToList();

        if (progsPosteriores.Any(p => (bool)p["id_aula_dada"]))
        {
          return BadRequest(new { error = "Não é possível incluir programação manual entre programações com diário lançado" });
        }

        // Buscar programação anterior para definir data e horário
        var progAnteriorGet = await SQLServerService.GetList(
            "T_PROGRAMACAO_TURMA",
            1,
            1,
            "nm_programacao_aux",
            true,
            null,
            "[cd_turma],[nm_programacao_aux]",
            $"[{request.cd_turma}],[{request.nm_programacao_aux}]",
            source,
            SearchModeEnum.LessThan,
            null,
            null);

        DateTime data_programacao;
        string hr_inicial, hr_final;

        if (progAnteriorGet.success && progAnteriorGet.data != null && progAnteriorGet.data.Any())
        {
          var progAnterior = progAnteriorGet.data.First();
          data_programacao = DateTime.Parse(progAnterior["dta_programacao_turma"].ToString()).AddDays(1);
          hr_inicial = progAnterior["hr_inicial_programacao"].ToString();
          hr_final = progAnterior["hr_final_programacao"].ToString();
        }
        else
        {
          // Buscar turma para pegar horários
          var turmaGet = await SQLServerService.GetFirstByFields(
              source,
              "T_TURMA",
              new List<(string campo, object valor)> { ("cd_turma", request.cd_turma) });

          if (turmaGet == null)
            return NotFound(new { error = "Turma não encontrada" });

          data_programacao = DateTime.Parse(turmaGet["dt_inicio_aula"].ToString());

          var horarioGet = await SQLServerService.GetFirstByFields(
              source,
              "T_HORARIO",
              new List<(string campo, object valor)> { ("cd_turma", request.cd_turma) });

          if (horarioGet == null)
            return BadRequest(new { error = "Horários da turma não encontrados" });

          hr_inicial = horarioGet["hr_inicial"].ToString();
          hr_final = horarioGet["hr_final"].ToString();
        }

        // Reordenar programações posteriores (incrementar auxiliar)
        foreach (var prog in progsPosteriores)
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

        // Inserir nova programação manual
        var novaProg = new Dictionary<string, object>
        {
          ["cd_turma"] = request.cd_turma,
          ["nm_aula_programacao_turma"] = 0, // Aulas manuais não incrementam numeração
          ["dta_programacao_turma"] = data_programacao.ToString("yyyy-MM-dd"),
          ["dc_programacao_turma"] = request.dc_programacao_turma,
          ["hr_inicial_programacao"] = hr_inicial,
          ["hr_final_programacao"] = hr_final,
          ["nm_programacao_aux"] = request.nm_programacao_aux,
          ["id_aula_dada"] = false,
          ["id_programacao_manual"] = true,
          ["id_reprogramada"] = false,
          ["id_provisoria"] = false,
          ["id_mostrar_calendario"] = true,
          ["dta_cadastro_programacao"] = DateTime.Now.ToString("yyyy-MM-dd"),
          ["nm_programacao_real"] = 0,
          ["id_prog_cancelada"] = false,
          ["id_modificada"] = false
        };

        var insertResult = await SQLServerService.Insert("T_PROGRAMACAO_TURMA", novaProg, source);

        if (!insertResult.success)
          return BadRequest(new { error = insertResult.error });

        return ResponseDefault(new { message = "Programação manual incluída com sucesso" });
      }

      return BadRequest(new { error = "Fonte de dados não configurada ou inativa." });
    }

  }
}

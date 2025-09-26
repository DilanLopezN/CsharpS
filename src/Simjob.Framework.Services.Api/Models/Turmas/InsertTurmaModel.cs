using System.Collections.Generic;

namespace Simjob.Framework.Services.Api.Models.Turmas
{
    public class InsertTurmaModel
    {
        public int? cd_turma_ppt { get; set; }
        public string? no_turma { get; set; }
        public int cd_pessoa_escola { get; set; }
        public bool id_turma_ativa { get; set; }
        public int? cd_curso { get; set; }
        public int? cd_sala { get; set; }
        public int cd_duracao { get; set; }
        public int cd_regime { get; set; }
        public string dt_inicio_aula { get; set; }
        public string dt_final_aula { get; set; }
        public bool id_aula_externa { get; set; }
        public int nro_aulas_programadas { get; set; } //smallint
        public bool id_turma_ppt { get; set; }
        public int cd_produto { get; set; }
        public int nm_turma { get; set; }
        public string dt_termino_turma { get; set; }
        public string no_apelido { get; set; }
        public int? cd_turma_enc { get; set; }
        public bool id_percentual_faltas { get; set; }
        public int? cd_sala_online { get; set; }
        public List<HorarioModel>? Horarios { get; set; } = new List<HorarioModel>();
        public List<ProfessorTurmaModel>? ProfessoresTurma { get; set; } = new List<ProfessorTurmaModel>();
        public List<AlunoTurmaModel>? AlunosTurma { get; set; } = new List<AlunoTurmaModel>();
        public List<ProgramacaoTurmaModel>? ProgramacaoTurma { get; set; }
        public FeriadoDesconsideradoModel? FeriadoDesconsiderado { get; set; }
        public class HorarioModel
        {
            public int? cd_registro { get; set; }
            public int? id_origem { get; set; }
            public int id_dia_semana { get; set; }
            public bool id_disponivel { get; set; }
            public string dt_hora_ini { get; set; }
            public string dt_hora_fim { get; set; }
        }

        public class ProfessorTurmaModel
        {
            public int cd_professor { get; set; }
            public bool id_professor_ativo { get; set; }
        }

        public class AlunoTurmaModel
        {
            public int cd_aluno { get; set; }
            public bool id_professor_ativo { get; set; }
            public int cd_situacao_aluno_turma { get; set; }
            public int cd_curso { get; set; }
            public string dt_inicio { get; set; }
            public string dt_movimento { get; set; }
            
        }
        public class ProgramacaoTurmaModel
        {
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

        public class FeriadoDesconsideradoModel
        {
            public string dt_inicial { get; set; }
            public string dt_final { get; set; }
            public bool id_programacao_feriado { get; set; }
        }
    }
}

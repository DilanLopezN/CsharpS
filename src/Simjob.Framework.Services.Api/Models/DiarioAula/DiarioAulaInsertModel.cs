using static Simjob.Framework.Services.Api.Controllers.DiarioAulaController;
using System.Collections.Generic;
using System;

namespace Simjob.Framework.Services.Api.Models.DiarioAula
{
    public class DiarioAulaInsertModel
    {
        public int cd_pessoa_empresa { get; set; }
        public int cd_turma { get; set; }
        public int cd_professor { get; set; }
        public int cd_tipo_aula { get; set; }
        public int cd_usuario { get; set; }
        public int cd_programacao_turma { get; set; }
        public DateTime dt_aula { get; set; }
        public TimeSpan hr_inicial_aula { get; set; }
        public TimeSpan hr_final_aula { get; set; }
        public bool id_aula_externa { get; set; }
        public bool id_falta_professor { get; set; }
        public int? cd_professor_substituto { get; set; }
        public int? cd_sala { get; set; }
        public int? cd_avaliacao { get; set; }
        public short? id_status_aula { get; set; }
        public string? dc_obs_falta { get; set; }
        public string? tx_obs_aula { get; set; }
        public int? cd_motivo_falta { get; set; }
        public List<AlunoTurma>? grid_aluno { get; set; }
    }
    public class AlunoTurma
    {
        public int cd_aluno_turma { get; set; }
        public bool flg_falta { get; set; }
        public bool flg_falta_justificada { get; set; }
        public string? dc_obs_justificada { get; set; }
    }
}

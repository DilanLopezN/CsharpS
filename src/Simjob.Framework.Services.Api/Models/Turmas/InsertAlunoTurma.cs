namespace Simjob.Framework.Services.Api.Models.Turmas
{
    public class InsertAlunoTurmaRelationModel
    {
        public int cd_turma { get; set; }
        public int cd_aluno { get; set; }
        public bool id_professor_ativo { get; set; }
        public int cd_situacao_aluno_turma { get; set; }
        public int cd_curso { get; set; }
        public string dt_inicio { get; set; }
        public string dt_movimento { get; set; }
    }
}

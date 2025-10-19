namespace Simjob.Framework.Services.Api.Models.Turmas
{
    public class InsertAlunoTurmaPreTurma
    {
        public int cd_aluno { get; set; }
        public int cd_turma { get; set; }
        public int cd_contrato { get; set; }
        public int? cd_curso { get; set; }
        //id_tipo_movimento
        //id_reprovado
        //nm_aulas_dadas
        //nm_faltas
        //id_manter_contrato
        //id_renegociacao
    }
}

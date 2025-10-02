using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;
using System;

namespace FundacaoFisk.SGF.Services.Coordenacao.Model
{
    public class AlunoAulaReposicaoUI : TO
    {
        public int cd_aluno_aula_reposicao { get; set; }
        public int cd_aula_reposicao { get; set; }
        public int cd_aluno { get; set; }
        public byte id_participacao { get; set; }
        public string tx_observacao_aluno_aula { get; set; }
        public string no_aluno { get; set; }
        public string no_pessoa { get; set; }

        public virtual Aluno Aluno { get; set; }
        public virtual AulaReposicao AulaReposicao { get; set; }

        public string participou
        {
            get
            {
                if (Convert.ToBoolean(this.id_participacao))
                    return "Sim";
                else
                    return "Não";
            }
        }
        public static AlunoAulaReposicaoUI fromItem(AlunoAulaReposicao alunoAulaReposicao)
        {
            AlunoAulaReposicaoUI alunoAulaReposicaoUi = new AlunoAulaReposicaoUI()
            {
                cd_aluno_aula_reposicao = alunoAulaReposicao.cd_aluno_aula_reposicao,
                cd_aula_reposicao = alunoAulaReposicao.cd_aula_reposicao,
                cd_aluno = alunoAulaReposicao.cd_aluno,
                id_participacao = alunoAulaReposicao.id_participacao,
                tx_observacao_aluno_aula = alunoAulaReposicao.tx_observacao_aluno_aula,
                Aluno = alunoAulaReposicao.Aluno,
                AulaReposicao = alunoAulaReposicao.AulaReposicao

            };
            return alunoAulaReposicaoUi;
        }
    }
}
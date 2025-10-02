using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;
namespace FundacaoFisk.SGF.Services.Coordenacao.Model
{
    public class ControleFaltasAlunoUI : TO
    {
        public int cd_controle_faltas_aluno { get; set; }
        public int cd_controle_faltas { get; set; }
        public int cd_aluno { get; set; }
        public Nullable<int> cd_situacao_aluno_turma { get; set; }
        public byte nm_faltas { get; set; }
        public bool id_assinatura { get; set; }
        public string no_aluno { get; set; }
        public string no_situacao_aluno_turma { get; set; }

        public Aluno Aluno { get; set; }
        public ControleFaltas ControleFaltas { get; set; }

        public static ControleFaltasAlunoUI fromItem(ControleFaltasAluno controleFaltasAluno)
        {
            ControleFaltasAlunoUI controleFaltasAlunoUi = new ControleFaltasAlunoUI
            {
                cd_controle_faltas_aluno = controleFaltasAluno.cd_controle_faltas_aluno,
                cd_controle_faltas = controleFaltasAluno.cd_controle_faltas,
                cd_aluno = controleFaltasAluno.cd_aluno,
                cd_situacao_aluno_turma = controleFaltasAluno.cd_situacao_aluno_turma,
                nm_faltas = controleFaltasAluno.nm_faltas,
                id_assinatura = controleFaltasAluno.id_assinatura,
                no_aluno = controleFaltasAluno.no_aluno,
                no_situacao_aluno_turma = controleFaltasAluno.no_situacao_aluno_turma,
                Aluno = controleFaltasAluno.Aluno,
                ControleFaltas = controleFaltasAluno.ControleFaltas
            };
            return controleFaltasAlunoUi;
        }


    }
}
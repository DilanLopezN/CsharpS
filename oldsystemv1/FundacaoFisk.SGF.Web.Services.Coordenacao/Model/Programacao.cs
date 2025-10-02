using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Model
{
    public class Programacao : TO
    {
        public int? cd_escola { get; set; }
        public int cd_programacao_curso { get; set; }
        public Nullable<int> cd_regime { get; set; }
        public int cd_curso { get; set; }
        public int cd_duracao { get; set; }
        public List<ItemProgramacao> itens { get; set; }
    }

    public class ProgramacoesTurmaSemDiarioAula : TO
    {
        public List<ProfessorUI> professoresHorariosTurma { get; set; }
        public List<Aluno> alunos { get; set; }
        public IEnumerable<ProgramacaoTurma> progsTurma { get; set; }
        public IEnumerable<Avaliacao> avaliacoesTurma { get; set; }
        public string nomeAlunosPendencia { get; set; }
    }
}

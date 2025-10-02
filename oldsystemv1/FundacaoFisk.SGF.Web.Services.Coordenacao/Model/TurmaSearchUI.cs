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
    public partial class TurmaSearchUI : TO
    {
        public int cd_turma { get; set; }
        public int cd_pessoa_escola { get; set; }
        public string no_turma { get; set; }
        public bool id_turma_ativa { get; set; }

        public string turma_ativa
        {
            get
            {
                return this.id_turma_ativa ? "Sim" : "Não";
            }
        }
    }

    public class ComponentesTurma
    {
        public Turma turma { get; set; }
        public List<Produto> produtos { get; set; }
        public List<Regime> regimes { get; set; }
        public List<Duracao> duracoes { get; set; }
        public List<Curso> cursos { get; set; }
        public List<ProfessorUI> professores { get; set; }
        public List<MotivoDesistencia> motivosDesistencia { get; set; }
        public bool usuarioSisProf { get; set; }
        public List<FuncionarioUI> avaliadores { get; set; }
    }
}

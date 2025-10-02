using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Model
{
    public partial class ProgramacaoCursoUI : TO
    {
        public int cd_programacao_curso { get; set; }
        public Nullable<int> cd_regime { get; set; }
        public int cd_curso { get; set; }
        public int cd_duracao { get; set; }
        public String dc_aula_programacao { get; set; }
        public Nullable<int> nm_aula_programacao { get; set; }
        public String no_curso { get; set; }
        public String dc_duracao { get; set; }

      

        public List<DefinicaoRelatorio> ColunasRelatorio {
            get {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();

                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                retorno.Add(new DefinicaoRelatorio("no_curso", "Curso", AlinhamentoColuna.Left, "2.3000in"));
                retorno.Add(new DefinicaoRelatorio("dc_duracao", "Carga Horária", AlinhamentoColuna.Left, "2.3000in"));
                retorno.Add(new DefinicaoRelatorio("nm_aula_programacao", "Nr. Aulas", AlinhamentoColuna.Center, "2.3000in"));
                return retorno;
            }
        }

        public static ProgramacaoCursoUI fromProgramacaoCurso(ProgramacaoCurso programacao)
        {
            ProgramacaoCursoUI programacaoUI = new ProgramacaoCursoUI
            {
                cd_programacao_curso = programacao.cd_programacao_curso,
                cd_curso = programacao.cd_curso,
                cd_duracao = programacao.cd_duracao,
                no_curso = programacao.Curso.no_curso,
                dc_duracao = programacao.Duracao.dc_duracao
            };
            return programacaoUI;
        }
    }
}

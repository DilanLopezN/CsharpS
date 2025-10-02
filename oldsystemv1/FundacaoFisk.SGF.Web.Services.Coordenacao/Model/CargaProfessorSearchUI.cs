using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Model
{
    public partial class CargaProfessorSearchUI : TO
    {
        public int cd_carga_professor { get; set; }
        public int cd_escola { get; set; }
        public int cd_carga_horaria_duracao { get; set; }
        public string no_carga_horaria_duracao { get; set; }
        public short nm_carga_horaria { get; set; }
        public short nm_carga_professor { get; set; }
        public bool id_carga_professor_duracao_ativo { get; set; }

        public string cargahorariaduracao_ativo
        {
            get
            {
                return this.id_carga_professor_duracao_ativo ? "Sim" : "Não";
            }
        }

        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();

                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_carga_professor", "Código"));
                retorno.Add(new DefinicaoRelatorio("nm_carga_horaria", "Carga Horária(min)", AlinhamentoColuna.Left, "2.0000in"));
                retorno.Add(new DefinicaoRelatorio("nm_carga_professor", "Carga Professor(min)", AlinhamentoColuna.Left, "2.0000in"));
                //retorno.Add(new DefinicaoRelatorio("cargahorariaduracao_ativo", "Ativo", AlinhamentoColuna.Left, "0.7000in"));

                return retorno;
            }
        }

        public static CargaProfessorSearchUI fromCargaProfessor(CargaProfessor cargaprofessor)
        {
            CargaProfessorSearchUI cargaprofessorUI = new CargaProfessorSearchUI
            {
                cd_carga_professor = cargaprofessor.cd_carga_professor,
                cd_escola = cargaprofessor.cd_escola,
                nm_carga_horaria =  cargaprofessor.nm_carga_horaria,
                nm_carga_professor = cargaprofessor.nm_carga_professor
            };
            return cargaprofessorUI;
        }
    }
}

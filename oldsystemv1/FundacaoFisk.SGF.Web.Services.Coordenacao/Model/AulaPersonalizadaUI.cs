using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Model
{
    public class AulaPersonalizadaUI : TO
    {

        public int cd_aula_personalizada { get; set; }
        public System.DateTime dt_aula_personalizada { get; set; }
        public System.TimeSpan hh_inicial { get; set; }
        public System.TimeSpan hh_final { get; set; }
        public int cd_produto { get; set; }
        public int cd_turma_personalizada { get; set; }
        public string no_produto { get; set; }
        public string no_turma_personalizada { get; set; }
        public string no_sala { get; set; }
        public string no_professor { get; set; }
        public string no_aluno { get; set; }
        public int? cd_aula_personalizada_aluno { get; set; }
        public bool? id_participou { get; set; }
        public int cd_aluno { get; set; }

        public string dta_aula_personalizada
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", dt_aula_personalizada);
            }
        }
        public string participou
        {
            get
            {
                string retorno = "Não";
                if (this.id_participou.HasValue && this.id_participou.Value)
                    retorno = "Sim";
                else
                    if (!this.id_participou.HasValue)
                        retorno = "";
                return retorno;
            }
        }

        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();

                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_atividade_extra", "Código"));
                retorno.Add(new DefinicaoRelatorio("no_aluno", "Aluno", AlinhamentoColuna.Left, "1.1000in"));
                retorno.Add(new DefinicaoRelatorio("no_produto", "Produto", AlinhamentoColuna.Left, "0.8000in"));
                retorno.Add(new DefinicaoRelatorio("dta_aula_personalizada", "Data Aula", AlinhamentoColuna.Center, "0.9000in"));
                retorno.Add(new DefinicaoRelatorio("hh_inicial", "Hora Inicial", AlinhamentoColuna.Center, "1.0000in"));
                retorno.Add(new DefinicaoRelatorio("hh_final", "Hora Final", AlinhamentoColuna.Center, "0.9000in"));
                retorno.Add(new DefinicaoRelatorio("no_sala", "Sala", AlinhamentoColuna.Left, "0.700in"));
                retorno.Add(new DefinicaoRelatorio("no_professor", "Professor", AlinhamentoColuna.Left, "1.1000in"));
                retorno.Add(new DefinicaoRelatorio("no_turma_personalizada", "Personalizada", AlinhamentoColuna.Left, "1.8000in"));
                retorno.Add(new DefinicaoRelatorio("participou", "Participou", AlinhamentoColuna.Center, "0.8500in"));
                return retorno;
            }
        }
    }
}

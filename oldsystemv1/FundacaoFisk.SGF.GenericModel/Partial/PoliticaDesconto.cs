using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class PoliticaDesconto
    {
        public int qtd_turmas { get; set; }
        public int qtd_alunos { get; set; }
        public string politica_desconto_ativo
        {
            get
            {
                return this.id_ativo ? "Sim" : "Não";
            }
        }
        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();

                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_politica_desconto", "Código"));
              //  retorno.Add(new DefinicaoRelatorio("no_turma", "Turmas", AlinhamentoColuna.Left, "1.5000in"));
                retorno.Add(new DefinicaoRelatorio("no_aluno", "Alunos", AlinhamentoColuna.Left, "1.5000in"));
               // retorno.Add(new DefinicaoRelatorio("dt_inicial", "Data", AlinhamentoColuna.Right));
                retorno.Add(new DefinicaoRelatorio("politica_desconto_ativo", "Ativo", AlinhamentoColuna.Center));

                return retorno;
            }
        }
    }
}

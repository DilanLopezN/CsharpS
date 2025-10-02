using System.Collections.Generic;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class MensagemAvaliacao
    {
        public string mensagemAtiva
        {
            get
            {
                return this.id_mensagem_ativa ? "Sim" : "Não";
            }
        }

        public string no_produto { get; set; }

        public string no_curso { get; set; }

        public ICollection<int> cursos { get; set; }

        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();

                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_evento", "Código"));
                retorno.Add(new DefinicaoRelatorio("no_produto", "Produto", AlinhamentoColuna.Left, "0.9000in"));
                retorno.Add(new DefinicaoRelatorio("no_curso", "Curso", AlinhamentoColuna.Left, "0.9000in"));
                retorno.Add(new DefinicaoRelatorio("tx_mensagem_avaliacao", "Mensagem", AlinhamentoColuna.Left, "4.5000in"));
                retorno.Add(new DefinicaoRelatorio("mensagemAtiva", "Ativo", AlinhamentoColuna.Center, "0.9000in"));

                return retorno;
            }
        }
    }
}
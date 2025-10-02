using Componentes.GenericModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.Services.Coordenacao.Model
{
    public class AvaliacaoParticipacaoUI : TO
    {
        public int cd_avaliacao_participacao_vinc { get; set; }
        public int cd_avaliacao_participacao { get; set; }
        public int cd_produto { get; set; }
        public string no_produto { get; set; }
        public int cd_criterio_avaliacao { get; set; }
        public string dc_criterio_avaliacao { get; set; }
        public string no_participacao_avaliacao { get; set; }
        public string id_avaliacao_participacao_ativa { get; set; }
        public byte? nm_ordem { get; set; }


        //Método para relatório
        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();

                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_atividade_extra", "Código"));
                retorno.Add(new DefinicaoRelatorio("dc_criterio_avaliacao", "Nome Avaliação", AlinhamentoColuna.Left, "2.0000in"));
                retorno.Add(new DefinicaoRelatorio("no_participacao_avaliacao", "Participação", AlinhamentoColuna.Left, "2.8000in"));
                retorno.Add(new DefinicaoRelatorio("no_produto", "Produto", AlinhamentoColuna.Left, "1.1000in"));
                retorno.Add(new DefinicaoRelatorio("id_avaliacao_participacao_ativa", "Ativo", AlinhamentoColuna.Center, "1.0000in"));
                return retorno;
            }
        }
    }
}

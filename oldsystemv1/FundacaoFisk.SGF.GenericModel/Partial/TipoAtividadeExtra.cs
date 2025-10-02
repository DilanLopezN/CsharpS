using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class TipoAtividadeExtra : TO
    {
        public string atividadeExtraAtiva
        {
            get
            {
                return this.id_tipo_atividade_extra_ativa ? "Sim" : "Não";
            }
        }

        public List<DefinicaoRelatorio> ColunasRelatorio {
            get {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();

                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_tipo_atividade_extra", "Código"));
                retorno.Add(new DefinicaoRelatorio("no_tipo_atividade_extra", "Tipo de Atividade Extra", AlinhamentoColuna.Left, "3.8000in"));
                retorno.Add(new DefinicaoRelatorio("atividadeExtraAtiva", "Ativa", AlinhamentoColuna.Center));

                return retorno;
            }
        }
    }
}

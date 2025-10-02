using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class Modalidade
    {
        public string modalidadeAtiva
        {
            get
            {
                return this.id_modalidade_ativa ? "Sim" : "Não";
            }
        }

        public List<DefinicaoRelatorio> ColunasRelatorio {
            get {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();

                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_modalidade", "Código"));
                retorno.Add(new DefinicaoRelatorio("no_modalidade", "Série", AlinhamentoColuna.Left, "3.8000in"));
                retorno.Add(new DefinicaoRelatorio("modalidadeAtiva", "Ativa", AlinhamentoColuna.Center));

                return retorno;
            }
        }
    }
}

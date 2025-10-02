using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.GenericModel {
    public partial class Midia {

        public enum TipoMidiaEnum { TESTECLASSIFICACAO = 1, MATRICULAONLINE = 2}

        public string midia_ativa {
            get {
                return this.id_midia_ativa ? "Sim" : "Não";
            }
        }
        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();

                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_midia", "Código"));
                retorno.Add(new DefinicaoRelatorio("no_midia", "Mídia", AlinhamentoColuna.Left, "3.8000in"));
                retorno.Add(new DefinicaoRelatorio("midia_ativa", "Ativa", AlinhamentoColuna.Center));

                return retorno;
            }
        }
    }
}

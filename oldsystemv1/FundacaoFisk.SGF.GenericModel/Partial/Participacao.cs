using Componentes.GenericModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.GenericModel {
    public partial class Participacao {
        public string participacao_ativa {
            get
            {
                return this.id_participacao_ativa ? "Sim" : "Não";
            }
        }
        public bool participacao_selecionada { get; set; }
        public bool id_avaliacao_participacao_vinc_ativa { get; set; }

        public List<DefinicaoRelatorio> ColunasRelatorio
        {
            get
            {
                List<DefinicaoRelatorio> retorno = new List<DefinicaoRelatorio>();

                // Fórumla para calcular o tamanho em in e passar para o metodo construtor segundo o padrão: (tamCaracteres * 1.5) / 15
                //retorno.Add(new DefinicaoRelatorio("cd_participacao", "Código"));
                retorno.Add(new DefinicaoRelatorio("no_participacao", "Descrição", AlinhamentoColuna.Left, "2.0000in"));
                retorno.Add(new DefinicaoRelatorio("participacao_ativa", "Ativa", AlinhamentoColuna.Left, "0.7000in"));

                return retorno;
            }
        }  
    }
}

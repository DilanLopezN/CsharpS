using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.GenericModel {
    public partial class AvaliacaoParticipacaoVinc
    {
        public string no_participacao_avaliacao { get; set; }

        public string avaliacao_participacao_ativa 
        {
            get 
            {
                return this.id_avaliacao_participacao_ativa ? "Sim" : "Não";
            }
        }
    }
}

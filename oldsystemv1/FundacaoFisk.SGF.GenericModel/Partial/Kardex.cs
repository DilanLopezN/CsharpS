using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.GenericModel {
    public partial class Kardex {
      
        public enum TipoMovimento {
            ENTRADA = 1, //Devlução
            SAIDA = 2 //Empréstimo
        }
        
        public byte id_tipo { get; set; }

        public static Kardex changeValueKardex(Kardex kardexContext, Kardex kardexView)
        {
            kardexContext.cd_item = kardexView.cd_item;
            kardexContext.dt_kardex = kardexView.dt_kardex;
            kardexContext.qtd_kardex = kardexView.qtd_kardex;
            kardexContext.nm_documento = kardexView.nm_documento;
            kardexContext.tx_obs_kardex = kardexView.tx_obs_kardex;
            kardexContext.vl_kardex = kardexView.vl_kardex;

            return kardexContext;
        }


    }
}

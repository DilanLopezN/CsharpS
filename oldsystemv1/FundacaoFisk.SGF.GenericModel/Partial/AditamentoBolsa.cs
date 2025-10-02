using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class AditamentoBolsa
    {

        public string dtaComunicadoBolsa
        {
            get
            {
                if (dt_comunicado_bolsa.HasValue)
                    return String.Format("{0:dd/MM/yyyy}", dt_comunicado_bolsa.Value);
                else
                    return String.Empty;
            }
        }

        public static AditamentoBolsa changeValuesAditamentoBolsa(AditamentoBolsa bolsaContext, AditamentoBolsa bolsaView)
        {
            bolsaContext.pc_bolsa = bolsaView.pc_bolsa;
            bolsaContext.dt_comunicado_bolsa = bolsaView.dt_comunicado_bolsa != null ? bolsaView.dt_comunicado_bolsa.Value.Date : bolsaView.dt_comunicado_bolsa;
            bolsaContext.dc_validade_bolsa = bolsaView.dc_validade_bolsa;
            bolsaContext.cd_motivo_bolsa = bolsaView.cd_motivo_bolsa;

            return bolsaContext;
        }
    }
}

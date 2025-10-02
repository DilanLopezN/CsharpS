using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class AlunoBolsa
    {
        public string dtaInicioBolsa {
            get {
                if(dt_inicio_bolsa != null)
                    return String.Format("{0:dd/MM/yyyy}", dt_inicio_bolsa);
                else
                    return String.Empty;
            }
        }

        public string dtaComunicadoBolsa {
            get {
                if(dt_comunicado_bolsa.HasValue)
                    return String.Format("{0:dd/MM/yyyy}", dt_comunicado_bolsa.Value);
                else
                    return String.Empty;
            }
        }        
        
        public string dtaCancelamentoBolsa {
            get {
                if(dt_cancelamento_bolsa.HasValue)
                    return String.Format("{0:dd/MM/yyyy}", dt_cancelamento_bolsa.Value);
                else
                    return String.Empty;
            }
        } 

        public static AlunoBolsa changeValuesAlunoBolsa(AlunoBolsa bolsaContext, AlunoBolsa bolsaView){
            bolsaContext.pc_bolsa = bolsaView.pc_bolsa;
            bolsaContext.dt_inicio_bolsa = bolsaView.dt_inicio_bolsa;
            bolsaContext.dc_validade_bolsa = bolsaView.dc_validade_bolsa;
            bolsaContext.pc_bolsa_material = bolsaView.pc_bolsa_material;
            bolsaContext.dt_comunicado_bolsa = bolsaView.dt_comunicado_bolsa;
            bolsaContext.dt_cancelamento_bolsa = bolsaView.dt_cancelamento_bolsa;
            bolsaContext.cd_motivo_bolsa = bolsaView.cd_motivo_bolsa;
            bolsaContext.cd_motivo_cancelamento_bolsa = bolsaView.cd_motivo_cancelamento_bolsa;
        
            return bolsaContext;
        }
    }
}
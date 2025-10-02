using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.GenericModel {
    public partial class AtividadeExtra {
         
         public enum EnumTipoAtividade {
             AULAEXPERIMENTAL = 600
         };

        public bool id_participou { get; set; }
        public bool hasClickEscola { get; set; }
        public string no_tipo_atividade_extra {
            get {
                string retorno = "";
                if(this.TipoAtividadeExtra != null)
                    retorno = this.TipoAtividadeExtra.no_tipo_atividade_extra;
                return retorno;
            }
        }
        public string participou {
            get {
                string retorno = "Não";
                if(this.id_participou)
                    retorno = "Sim";
                return retorno;
            }
        }
        public string dta_atividade_extra
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", dt_atividade_extra);
            }
        }

        public string tempo
        {
            get 
            {
                return string.Format("{0} - {1}", hh_inicial.ToString(@"hh\:mm"), hh_final.ToString(@"hh\:mm"));
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.GenericModel {
    public partial class Parametro {
        public enum TipoNumeroMatricula { 
            IGUAL_CONTRATO = 1,
            INCREMENTAL_AUTOMATICO = 2,
            MANUAL = 3
        }
        public enum REGIME_TRIBUTARIO
        {
            SIMPLES_NACIONAL = 1,
            SIMPLES_NACIONAL_RB = 2,
            REGIME_NORMAL = 3
        }
        public ICollection<LocalMovto> localMovto { get; set; }
        public string desc_plano_conta_mat { get; set; }
        public string desc_plano_conta_tax { get; set; }
        public string desc_politica_comercial_nf { get; set; }
        public string desc_item_taxa_matricula { get; set; }
        public string desc_item_mensalidade { get; set; }
        public string desc_item_biblioteca { get; set; }
        public Nullable<bool> id_empresa_propria { get; set; }
        public string dc_local_movto { get; set; }

                            
    }
}

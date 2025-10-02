using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class SituacaoTributaria
    {
        public double pc_reducao { get; set; }
        public enum FormaTrib{
            ISENTO = 1,
            TRIBUTADO = 2,
            REDUZIDO = 3,
            SUBSTITUTO = 4
        }

        public enum TipoImpostoEnum
        {
            ICMS = 1,
            PIS = 3,
            CONFINS = 4
        }
        public enum SitTrib
        {
            OPERACAO_SEM_INCIDENCIA_CONT_PIS = 65,
            OPERACAO_SEM_INCIDENCIA_CONT_COFINS = 107
        }
        public string dcSituacao
        {
            get
            {
                string formaTrib = "";

                switch (this.id_forma_tributacao)
                {
                    case (int)FormaTrib.ISENTO: formaTrib = "Isento";
                        break;
                    case (int)FormaTrib.TRIBUTADO: formaTrib = "Tributado";
                        break;
                    case (int)FormaTrib.REDUZIDO: formaTrib = "Reduzido";
                        break;
                    case (int)FormaTrib.SUBSTITUTO: formaTrib = "Substituto";
                        break;
                }
                return this.nm_situacao_tributaria + " - "  + this.dc_situacao_tributaria  + " (" +  formaTrib + ")";
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class Conceito
    {
        public string conceito_ativo
        {
            get
            {
                return this.id_conceito_ativo ? "Sim" : "Não";
            }
        }
        public string pc_inicial
        {
            get
            {
                if(this.pc_inicial_conceito == null)
                    return "";
                return string.Format("{0,00}", this.pc_inicial_conceito);
            }
        }
        public string pc_final
        {
            get
            {
                if(this.pc_final_conceito == null)
                    return "";
                return string.Format("{0,00}", this.pc_final_conceito);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class Sala
    {
        public string salaAtiva
        {
            get
            {
                return this.id_sala_ativa ? "Sim" : "Não";
            }
        }
    }
}

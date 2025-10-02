using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class LocalMovto
    {
        public string nomeLocal { get; set; }
        public enum TipoLocalMovtoEnum
        {
            CARTEIRA = 1, 
            BANCO = 2,
            CAIXA = 3,
            CARTAO_CREDITO = 4, 
            CARTAO_DEBITO = 5
        }
        
    }
}

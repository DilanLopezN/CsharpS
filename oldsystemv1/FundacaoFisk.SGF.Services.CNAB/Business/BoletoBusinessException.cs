using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Componentes.GenericBusiness;

namespace FundacaoFisk.SGF.Web.Services.CNAB.Business
{
    public class BoletoBusinessException : BusinessException{
        public enum TipoErro {
            ERRO_BANCO_SEM_LAYOUT_BOLETO = 1
        }
        public TipoErro tipoErro;

        public BoletoBusinessException(String msg, Exception ex, TipoErro erro, bool mostraStackTrace)
            : base(msg, ex, mostraStackTrace)
        {
            tipoErro = erro;
        }
    }
}

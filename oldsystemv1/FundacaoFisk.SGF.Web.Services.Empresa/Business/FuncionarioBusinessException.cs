using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Componentes.GenericBusiness;

namespace FundacaoFisk.SGF.Web.Services.Empresa.Business
{
    public class FuncionarioBusinessException : BusinessException{
        public enum TipoErro {
            ERRO_FUNCIONARIOJAEXISTE = 1,
        }
        public TipoErro tipoErro;

        public FuncionarioBusinessException(String msg, Exception ex, TipoErro erro, bool mostraStackTrace)
            : base(msg, ex, mostraStackTrace)
        {
            tipoErro = erro;
        }
    }
}

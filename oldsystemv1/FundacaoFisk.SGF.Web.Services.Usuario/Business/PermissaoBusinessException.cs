using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericBusiness;

namespace FundacaoFisk.SGF.Web.Services.Usuario.Business
{
    public class PermissaoBusinessException : BusinessException{
        public enum TipoErro {
            ERRO_GRUPO_MASTER = 1,
            ERRO_PERMISSAO_NEGADA = 2
        }
        public TipoErro tipoErro;

        public PermissaoBusinessException(String msg, Exception ex, TipoErro erro, bool mostraStackTrace)
            : base(msg, ex, mostraStackTrace)
        {
            tipoErro = erro;
        }
    }
}

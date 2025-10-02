using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Componentes.GenericBusiness;
using System.Net;

namespace FundacaoFisk.SGF.Web.Services.Auth.Business {
    public class AuthBusinessException : BusinessException {
        public enum TipoErro {
            // AccessTokenValidation
            AUTORIZACAO_EXPIRADA = 1, // Sessão expirada
            AUTORIZACAO_NAO_ENCONTRADA = 2,
            HORARIO_LOGIN_ULTRAPASSADO = 3,
            ALTERAR_SENHA_SYSADMIN = 4
        }
        public TipoErro tipoErro;

        public AuthBusinessException(String msg, Exception ex, TipoErro erro, bool mostraStackTrace)
            : base(msg, ex, mostraStackTrace) {
            tipoErro = erro;
        }
        public AuthBusinessException(String msg, TipoErro erro)
            : base(msg) {
            tipoErro = erro;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Componentes.GenericBusiness;

namespace FundacaoFisk.SGF.Web.Services.Usuario.Business
{
    public class UsuarioBusinessException : BusinessException{
        public enum TipoErro {
            ERRO_USUARIOEXISTENTE = 1,
            ERRO_COMBINACOES = 2,
            ERRO_SENHA_INVALIDA = 3,
            USUARIO_BLOQUEADO = 4,
            ERRO_TAMANHO_SENHA_MENOR = 5,
            ERRO_TAMANHO_SENHA_MAIOR = 6,
            ERRO_REMENTENTE_NOT_EXISTENTE = 7,
            ERRO_USUARIO_NOT_EXISTENTE = 8,
            ERROR_USUARIO_SEM_PERMISSAO = 9,
            ERRO_FALTA_DUAS_TENTATIVAS_SENHA = 10,
            ERRO_JA_EXISTE_SYSADMIN_ESCOLAS = 11,
            ERRO_USUARIO_COMUM_SYSADMIN = 12,
            ERRO_SYSADMIN_USUARIO = 13,
            ERRO_ENVIO_EMAIL = 14,
            ERRO_EMAIL_INVALIDO = 15,
            ERRO_USUARIO_EMAIL_NOT_FOUND = 16,
            ERRO_TOKEN_NAO_GERADO_AREA_RESTRITA = 17,
        }
        public TipoErro tipoErro;

        public UsuarioBusinessException(String msg, Exception ex, TipoErro erro, bool mostraStackTrace)
            : base(msg, ex, mostraStackTrace)
        {
            tipoErro = erro;
        }
    }
}

using System;
using Componentes.GenericBusiness;

namespace FundacaoFisk.SGF.Web.Services.Usuario.Business
{
    public class ApiAreaRestritaBusinessException : BusinessException
    {
        public enum TipoErro
        {
            ERRO_API_AREA_RESTRITA_GET_DETALHES_USUARIO = 1,
        }
        public TipoErro tipoErro;

        public ApiAreaRestritaBusinessException(String msg, Exception ex, TipoErro erro, bool mostraStackTrace)
            : base(msg, ex, mostraStackTrace)
        {
            tipoErro = erro;
        }
    }
}
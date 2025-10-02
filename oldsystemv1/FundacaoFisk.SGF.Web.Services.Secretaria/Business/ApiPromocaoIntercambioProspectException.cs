using System;
using Componentes.GenericBusiness;

namespace FundacaoFisk.SGF.Services.Coordenacao.Business
{
    public class ApiPromocaoIntercambioProspectException : BusinessException
    {
        public enum TipoErro
        {
            BEARER_NULO_VAZIO = 1,
            URL_NULO_VAZIO = 2,
            ERROR_REQUEST = 3,
            ERRO_PARAMETROS = 4
            
        }
        public ApiPromocaoIntercambioProspectException.TipoErro tipoErro;

        public ApiPromocaoIntercambioProspectException(String msg, Exception ex, ApiPromocaoIntercambioProspectException.TipoErro erro, bool mostraStackTrace)
            : base(msg, ex, mostraStackTrace)
        {
            tipoErro = erro;
        }
    }
}
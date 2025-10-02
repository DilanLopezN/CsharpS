using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericBusiness;

namespace FundacaoFisk.SGF.Services.EmailMarketing.Business
{
    public class EmailMarketingBusinessException : BusinessException
    {
        public enum TipoErro
        {
            ERRO_NAO_EXISTE_IMAGEM = 1,
            ERRO_JA_EXISTE_NAO_INSCRITO = 2,
            ERRO_REGISTRO_NAO_ENCONTRADO = 3,
            ERRO_EXTENSAO_ARQ_ANEXO = 4,
            ERRO_LIMITE_ARQ_ANEXO = 5

        }
        public TipoErro tipoErro;

        public EmailMarketingBusinessException(String msg, Exception ex, TipoErro erro, bool mostraStackTrace)
            : base(msg, ex, mostraStackTrace)
        {
            tipoErro = erro;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericBusiness;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Business
{
    public class FechamentoBusinessException  : BusinessException
    {
         public enum TipoErro {
            ERRO_FECHAMENTO_EXISTENTE = 16,
            ERRO_ALTERAR_FECHAMENTO = 17,
            ERRO_FECHAMENTO_SUPERIOR = 18,
            ERRO_SEM_ITEM_FECHAMENTO = 19,
            ERRO_FECHAMENTO_CONCORRENCIA = 20,
            ERRO_PROCEDURE_INCLUSAO_SALDO_ITEM= 21,
            ERRO_PROCEDURE_ALTERACAO_SALDO_ITEM= 22,
            ERRO_PROCEDURE_EXCLUSAO_SALDO_ITEM= 23
        }

        public TipoErro tipoErro;

        public FechamentoBusinessException(String msg, Exception ex, TipoErro erro, bool mostraStackTrace)
            : base(msg, ex, mostraStackTrace)
        {
            tipoErro = erro;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericBusiness;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Business
{
    public class FiscalBusinessException: BusinessException
    {
         public enum TipoErro {
            ERRO_PROCESSAR_NF = 1,
            ERRO_CANCELAR_NF = 2,
            ERRO_PROCESSAR_MOVIMENTO_SEM_ITEM = 3,
            ERRO_PROCESSAR_MOVIMENTO_COM_ITEM_ZERADO = 4,
            ERRO_NOTAS_POSTERIORES_PROCESSADOS = 5,
            ERRO_NOTAS_ANTERIORES_EM_ABERTAS = 6,
            ERRO_REGISTRO_NAO_ENCONTRADO = 7,
            ERRO_CHAVE_ACESSO_NF = 8,
            ERRO_CHAVE_ACESSO_INVALIDA = 9,
            ERRO_DEVOLUCAO = 10
        }

        public TipoErro tipoErro;
         
        public FiscalBusinessException(String msg, Exception ex, TipoErro erro, bool mostraStackTrace)
            : base(msg, ex, mostraStackTrace)
        {
            tipoErro = erro;
        }
    }
}

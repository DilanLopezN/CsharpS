using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericBusiness;

namespace FundacaoFisk.SGF.Services.Biblioteca.Business
{
    public class BibliotecaBusinessException : BusinessException
    {
        public enum TipoErro
        {
            DATA_DEVOLUCAO_MENOR_EMPRESTIMO = 1,
            DATA_PREV_DEVOLUCAO_MENOR_EMPRESTIMO = 2,
            SEM_SALDO_ESTOQUE = 3
        }
        public TipoErro tipoErro;

        public BibliotecaBusinessException(String msg, Exception ex, TipoErro erro, bool mostraStackTrace)
            : base(msg, ex, mostraStackTrace)
        {
            tipoErro = erro;
        }
    }
}

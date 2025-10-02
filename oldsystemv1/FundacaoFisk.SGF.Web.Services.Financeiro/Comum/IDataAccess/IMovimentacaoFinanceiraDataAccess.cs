using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using Componentes.GenericDataAccess.Comum;
using System.Data.Entity;
using Componentes.Utils;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess
{
    using FundacaoFisk.SGF.GenericModel;
    public interface IMovimentacaoFinanceiraDataAccess : IGenericRepository<MovimentacaoFinanceira>
    {       
        IEnumerable<MovimentacaoFinanceira> GetMovimentacaoFinanceiraSearch(SearchParameters parametros, String descricao, Boolean inicio, Boolean? ativo);
        Boolean deleteAllMovimentacao(List<MovimentacaoFinanceira> movimentacoes);
        IEnumerable<MovimentacaoFinanceira> getMovimentacaoWithContaCorrente(int cd_pessoa_escola);
        IEnumerable<MovimentacaoFinanceira> getMovimentacaoAtivaWithConta(int cd_pessoa_escola, bool isCadastrar);
        IEnumerable<MovimentacaoFinanceira> getMovimentacaoAtivaWithConta(int cd_pessoa_escola, int cd_movimentacao_financeira);
        IEnumerable<MovimentacaoFinanceira> getMovimentacaoTransferencia(int cd_pessoa_escola, int cd_movimentacao_financeira);
    }
}

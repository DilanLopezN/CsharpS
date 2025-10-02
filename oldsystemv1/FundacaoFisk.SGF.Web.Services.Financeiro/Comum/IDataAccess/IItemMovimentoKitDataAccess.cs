using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess.Comum;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess
{
    public interface IItemMovimentoKitDataAccess : IGenericRepository<ItemMovimentoKit>
    {
        IEnumerable<ItemMovimentoKit> getItensMovimentoKitByMovimento(int cd_movimento);
    }
}

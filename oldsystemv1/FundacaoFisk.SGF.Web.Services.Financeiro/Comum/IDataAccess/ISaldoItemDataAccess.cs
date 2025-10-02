using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using System.Data.Entity;
using Componentes.Utils;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess
{
    using FundacaoFisk.SGF.GenericModel;
    using Componentes.GenericDataAccess.Comum;
    using FundacaoFisk.SGF.Web.Services.Financeiro.Model;

    public interface ISaldoItemDataAccess : IGenericRepository<SaldoItem>
    {
        IEnumerable<SaldoItem> getSaldoItemById(int cd_fechamento, int cd_escola);
        SaldoItem getSaldoItemByIdItem(int cd_fechamento, int cd_item);
        bool processarSaldoItens(Nullable<int> cd_fechamento, Nullable<byte> id_tipo);
    }
}

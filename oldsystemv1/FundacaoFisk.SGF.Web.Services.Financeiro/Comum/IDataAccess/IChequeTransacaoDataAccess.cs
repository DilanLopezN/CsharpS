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

    public interface IChequeTransacaoDataAccess : IGenericRepository<ChequeTransacao>
    {
        IEnumerable<ChequeTransacao> getChequeTrasacao(int cd_tran_finan);
        //IEnumerable<ChequeTransacao> getChequeTrasacao(int cd_tran_finan, int cd_cheque);
    }
}

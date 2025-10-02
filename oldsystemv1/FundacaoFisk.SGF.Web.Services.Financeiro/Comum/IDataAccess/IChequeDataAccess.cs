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

    public interface IChequeDataAccess : IGenericRepository<Cheque>
    {
        Cheque getChequeByContrato(int id);
        List<Cheque> getChequesByTitulosContrato(List<int> cdTitulos, int cd_empresa);
        Cheque getChequeTransacao(int cd_tran_finan, int cd_empresa);
    }
}

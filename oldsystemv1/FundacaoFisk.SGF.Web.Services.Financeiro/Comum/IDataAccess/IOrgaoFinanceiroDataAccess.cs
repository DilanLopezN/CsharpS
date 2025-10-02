using System.Collections.Generic;
using Componentes.GenericDataAccess.Comum;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess
{
    public interface IOrgaoFinanceiroDataAccess : IGenericRepository<OrgaoFinanceiro>
    {
        IEnumerable<OrgaoFinanceiro> getOrgaoFinanceiroSearch(SearchParameters parametros, string descricao, bool inicio, bool? status);
        bool deleteAllOrgaoFinanceiro(List<OrgaoFinanceiro> orgaosFinanceiros);
        IEnumerable<OrgaoFinanceiro> getOrgaoFinanceiro(bool? status);
        IEnumerable<OrgaoFinanceiro> getAllOrgaoFinanceiro();
    }
}
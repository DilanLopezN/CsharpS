using Componentes.GenericDataAccess.Comum;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using System.Collections.Generic;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess
{
    public interface IEmpresaValorServicoDataAccess : IGenericRepository<EmpresaValorServico>
    {
        IEnumerable<EmpresaValorServico> getEmpresaValorServicoByEscola(int cdEscola);

        EmpresaValorServico getEmpresaValorServicoByIdAndEscola(int itemCdEmpresaValorServico, int itemCdPessoaEmpresa);
    }
}
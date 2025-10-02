using System.Collections.Generic;
using Componentes.GenericDataAccess.Comum;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Services.Coordenacao.Comum.IDataAccess
{
    public interface IProdutoFuncionarioDataAccess : IGenericRepository<ProdutoFuncionario>
    {

        IEnumerable<ProdutoFuncionario> searchProdutosFuncionario(int cdFuncionario, int cdEscola);
    }
}
using System.Collections.Generic;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using Componentes.Utils;
using Componentes.GenericDataAccess.Comum;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Pessoa.Comum.DataAccess
{
    public interface IPaisDataAccess : IGenericRepository<PaisSGF>
    {
        IEnumerable<PaisUI> GetAllPais();
        IEnumerable<PaisUI> GetPaisSearch(SearchParameters parametros, string descricao, bool inicio);
        IEnumerable<PaisUI> FindPais(string searchText);
        PaisUI GetPaisById(int idPais);
        PaisUI firstOrDefault();
        bool deleteAll(List<PaisUI> paises);
        IEnumerable<PaisUI> getPaisEstado();
        IEnumerable<PaisUI> GetAllPaisPorSexoPessoa();
    }
}

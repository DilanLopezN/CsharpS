using System.Collections.Generic;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using System.Data.Entity;
using Componentes.GenericDataAccess.Comum;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Pessoa.Comum.DataAccess
{
    public interface ITipoEnderecoDataAccess : IGenericRepository<TipoEnderecoSGF>
    {
        IEnumerable<TipoEnderecoSGF> GetAllTipoEndereco();
        IEnumerable<TipoEnderecoSGF> GetTipoEnderecoSearch(SearchParameters parametros, string descricao, bool inicio);
        IEnumerable<TipoEnderecoSGF> FindTipoEndereco(string searchText);
        TipoEnderecoSGF GetTipoEnderecoById(int idTipoEndereco);
        TipoEnderecoSGF firstOrDefault();
        bool deleteAll(List<TipoEnderecoSGF> tiposEndereco);
    }
}

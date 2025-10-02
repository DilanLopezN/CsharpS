using System.Collections.Generic;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using Componentes.Utils;
using Componentes.GenericDataAccess.Comum;
using FundacaoFisk.SGF.GenericModel;


namespace FundacaoFisk.SGF.Web.Services.Pessoa.Comum.DataAccess
{
    public interface ITipoTelefoneDataAccess : IGenericRepository<TipoTelefoneSGF>
    {
        #region TipoTelefone
        IEnumerable<TipoTelefoneSGF> GetAllTipoTelefone();
        IEnumerable<TipoTelefoneSGF> GetTipoTelefoneSearch(SearchParameters parametros, string descricao, bool inicio);
        IEnumerable<TipoTelefoneSGF> FindTipoTelefone(string searchText);
        TipoTelefoneSGF GetTipoTelefoneById(int idTipoTelefone);
        TipoTelefoneSGF firstOrDefault();
        bool deleteAll(List<TipoTelefoneSGF> tiposTelefone);
        #endregion

    }
}

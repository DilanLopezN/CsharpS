using System;
using System.Collections.Generic;
using Componentes.Utils;
using Componentes.GenericDataAccess.Comum;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Pessoa.Comum.DataAccess
{
    public interface ITipoLogradouroDataAccess : IGenericRepository<TipoLogradouroSGF>
    {
        IEnumerable<TipoLogradouroSGF> GetAllTipoLogradouro();
        IEnumerable<TipoLogradouroSGF> GetTipoLogradouroSearch(SearchParameters parametros, string descricao, bool inicio);
        IEnumerable<TipoLogradouroSGF> FindTipoLogradouro(string searchText);
        TipoLogradouroSGF GetTipoLogradouroById(int idTipoLogradouro);
        TipoLogradouroSGF firstOrDefault();
        bool deleteAll(List<TipoLogradouroSGF> tiposLogradouro);
        TipoLogradouroSGF findTipoLogradouroByNome(string no_tipo_logradouro);
    }
}

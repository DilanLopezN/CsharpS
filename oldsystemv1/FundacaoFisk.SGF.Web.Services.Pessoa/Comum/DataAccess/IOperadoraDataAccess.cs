using System.Collections.Generic;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using Componentes.Utils;
using Componentes.GenericDataAccess.Comum;
using FundacaoFisk.SGF.GenericModel;


namespace FundacaoFisk.SGF.Web.Services.Pessoa.Comum.DataAccess
{
    public interface IOperadoraDataAccess : IGenericRepository<Operadora>
    {
        #region Operadora
        IEnumerable<Operadora> GetAllOperadora();
        IEnumerable<Operadora> GetOperadoraSearch(SearchParameters parametros, string descricao, bool inicio, bool? ativa);
        IEnumerable<Operadora> FindOperadora(string searchText);
        Operadora GetOperadoraById(int idOperadora);
        Operadora firstOrDefault();
        bool deleteAll(List<Operadora> operadoras);
        IEnumerable<Operadora> GetAllOperadorasAtivas(int? cd_operadora);
        #endregion

    }
}

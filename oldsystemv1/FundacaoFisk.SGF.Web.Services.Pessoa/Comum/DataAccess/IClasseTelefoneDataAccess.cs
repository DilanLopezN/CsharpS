using System.Collections.Generic;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using Componentes.Utils;
using Componentes.GenericDataAccess.Comum;
using FundacaoFisk.SGF.GenericModel;


namespace FundacaoFisk.SGF.Web.Services.Pessoa.Comum.DataAccess
{
    public interface IClasseTelefoneDataAccess : IGenericRepository<ClasseTelefoneSGF>
    {
        #region ClasseTelefone
        IEnumerable<ClasseTelefoneSGF> GetAllClasseTelefone();
        IEnumerable<ClasseTelefoneSGF> GetClasseTelefoneSearch(SearchParameters parametros, string descricao, bool inicio);
        IEnumerable<ClasseTelefoneSGF> FindClasseTelefone(string searchText);
        ClasseTelefoneSGF GetClasseTelefoneById(int idClasseTelefone);
        ClasseTelefoneSGF firstOrDefault();
        bool deleteAll(List<ClasseTelefoneSGF> classesTelefone);
        #endregion

    }
}

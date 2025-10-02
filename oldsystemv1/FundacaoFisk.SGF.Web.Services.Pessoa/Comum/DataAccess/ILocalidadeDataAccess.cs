using System.Collections.Generic;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using System.Data.Entity;
using Componentes.GenericDataAccess.Comum;
using Componentes.Utils;
using System;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Pessoa.Comum.DataAccess
{
    public interface ILocalidadeDataAccess : IGenericRepository<LocalidadeSGF>
    {
        #region Endereco
        IEnumerable<LocalidadeSGF> GetAllEndereco();
        IEnumerable<LocalidadeSGF> GetAllEndereco(string searchText);
        LocalidadeSGF GetEnderecoDesc(string descricao);      
        #endregion

        #region Bairro
        IEnumerable<LocalidadeSGF> GetAllBairro();
        IEnumerable<LocalidadeSGF> GetBairroSearch(SearchParameters parametros, string descricao, bool inicio, int cd_cidade);
        IEnumerable<LocalidadeSGF> FindBairro(string searchText);
        LocalidadeSGF GetBairroDesc(string descricao);
        LocalidadeSGF GetBairroById(int idEstado);
        IEnumerable<LocalidadeSGF> getBairroPorCidade(int cd_cidade, int cd_bairro);
        LocalidadeSGF getBairroByNome(string no_loc_bairro, int cd_cidade, int cd_estado);
        #endregion

        #region Cidade
        IEnumerable<LocalidadeSGF> GetAllCidade();
        IEnumerable<LocalidadeSGF> GetAllCidade(int estado);
        IEnumerable<LocalidadeSGF> GetAllCidade(string search, int idEstado);
        IEnumerable<CidadeUI> GetCidadeSearch(SearchParameters parametros, string descricao, bool inicio, int nmMunicipio, int cdEstado);
        IEnumerable<LocalidadeSGF> FindCidade(string searchText);
        LocalidadeSGF GetCidadeById(int idCidade);
        IEnumerable<LocalidadeUI> GetCidadePaisEstado(SearchParameters parametros, int pais, int estado, int numMunicipio, String desCidade);
        LocalidadeSGF getCidadeByNomePorEstado(string no_loc_cidade, int cd_estado);
        #endregion

        #region Distrito
        IEnumerable<LocalidadeSGF> GetAllDistrito();
        IEnumerable<LocalidadeSGF> GetDistritoSearch(SearchParameters parametros, string descricao, bool inicio, int cd_cidade);
        IEnumerable<LocalidadeSGF> FindDistrito(string searchText);
        LocalidadeSGF GetDistritoById(int idDistrito);
        #endregion

        #region Logradouro

        IEnumerable<LocalidadeSGF> getLogradouroSearch(SearchParameters parametros, string descricao, bool inicio, int cd_estado, int cd_cidade, int cd_bairro, string cep);
        IEnumerable<LocalidadeSGF> getLogradouros(int[] cdLogradouros);
        IEnumerable<LocalidadeSGF> getAllLogradouroPorBairro(int cd_bairro);
        LocalidadeSGF getRuaByNome(string no_loc_rua, int cd_bairro, int cd_cidade, int cd_estado);
        IEnumerable<LogradouroCEP> getLogradouroCorreio(string descricao, string estado, string cidade, string bairro, string cep, int? numero);
        #endregion

        LocalidadeSGF firstOrDefault(byte tipo);
        LocalidadeSGF findFistOrDefaultLocalidade(byte tipoLoc, int? cdLocRel);
        LocalidadeSGF findFirtsEstadoWithCidade();
        Boolean existsLocalidade(int cdLoc, String no_rua);
        LocalidadeSGF GetLocalidade(int cd);
        bool deleteAll(List<LocalidadeSGF> localidades);
    }
}

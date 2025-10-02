using System;
using System.Collections.Generic;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using Componentes.Utils;
using Componentes.GenericBusiness.Comum;
using System.Data.Entity;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Pessoa.Comum.Business
{
    public interface ILocalidadeBusiness : IGenericBusiness
    {
        void sincronizaContexto(DbContext db);

        #region Endereço
        IEnumerable<LocalidadeSGF> GetAllEndereco();
        IEnumerable<EnderecoSGF> GetAllEnderecoByPessoa(int cdPessoa, int cd_endereco);
        EnderecoSGF PostEndereco(EnderecoSGF endereco);
        EnderecoSGF EditEndereco(EnderecoSGF endereco);
        LocalidadeSGF GetEnderecoDesc(string descricao);
        EnderecoSGF FindById(int codEndereco);
        EnderecoSGF saveChangesEndereco(EnderecoSGF endereco);
        Boolean deleteEndereco(EnderecoSGF endereco);
        EnderecoSGF getEnderecoByLogradouro(int cd_logradouro,string nm_cep);
        EnderecoSGF verificaSeExisteEnderecoOuGravar(EnderecoSGF endereco);
        EnderecoSGF getEnderecoResponsavelCPF(int cd_pessoa);
        EnderecoSGF getEnderecoByCdEndereco(int cd_endereco);
        #endregion
        #region Bairro
        IEnumerable<LocalidadeSGF> GetAllBairro();
        IEnumerable<LocalidadeSGF> GetBairroSearch(SearchParameters parametros, string descricao, bool inicio, int cd_cidade);
        IEnumerable<LocalidadeSGF> FindBairro(string searchText);
        LocalidadeSGF GetBairroById(int id);
        LocalidadeSGF GetBairroDesc(string descricao);
        IEnumerable<LocalidadeSGF> getBairroPorCidade(int cd_cidade, int cd_bairro);
        LocalidadeSGF addLogradouro(LocalidadeSGF logradouro);
        LocalidadeSGF editLogradouro(LocalidadeSGF logradouro);
        bool deleteLogradouros(int[] cdLogradouros);
        #endregion
        #region Distrito
        IEnumerable<LocalidadeSGF> GetAllDistrito();
        IEnumerable<LocalidadeSGF> GetDistritoSearch(SearchParameters parametros, string descricao, bool inicio, int cd_cidade);
        IEnumerable<LocalidadeSGF> FindDistrito(string searchText);
        LocalidadeSGF GetDistritoById(int id);
        #endregion
        #region Cidade
        IEnumerable<LocalidadeSGF> GetAllCidade();
        IEnumerable<CidadeUI> GetCidadeSearch(SearchParameters parametros, string descricao, bool inicio, int nmMunicipio, int cdEstado);
        IEnumerable<LocalidadeSGF> FindCidade(string searchText);
        LocalidadeSGF GetCidadeById(int id);
        IEnumerable<LocalidadeSGF> GetAllCidade(int idEstado);
        IEnumerable<LocalidadeSGF> GetAllCidade(string search, int idEstado);
        CidadeUI PostCidade(CidadeUI cidade);
        CidadeUI PutCidade(CidadeUI cidade);
        IEnumerable<LocalidadeUI> GetCidadePaisEstado(SearchParameters parametros, int pais, int estado, int numMunicipio, String desCidade);
        #endregion
        #region Localidade
        LocalidadeSGF PostLocalidade(LocalidadeSGF localidade);
        LocalidadeSGF PutLocalidade(LocalidadeSGF localidade);
        bool DeleteLocalidade(List<LocalidadeSGF> localidades);
        IEnumerable<Atividade> getAllListAtividades(string searchText, int natureza, bool? status);
        #endregion
        #region Pais
        IEnumerable<PaisUI> GetAllPais();
        IEnumerable<PaisUI> GetPaisSearch(SearchParameters parametros, string descricao, bool inicio);
        IEnumerable<PaisUI> FindPais(string searchText);
        PaisUI GetPaisById(int id);
        PaisUI PostPaisLocalidade(PaisUI paisUI);
        PaisUI PutPaisLocalidade(PaisUI paisUI);
        bool DeletePais(List<PaisUI> paises);
        IEnumerable<PaisUI> GetPaisEstado();
        IEnumerable<PaisUI> GetAllPaisPorSexoPessoa();
        #endregion
        #region Estado
        IEnumerable<EstadoUI> GetAllEstado();
        IEnumerable<EstadoUI> GetEstadoSearch(SearchParameters parametros, string descricao, bool inicio, int cdPais);
        IEnumerable<EstadoUI> FindEstado(string searchText);
        EstadoUI GetEstadoById(int id);
        EstadoUI PostEstadoLocalidade(EstadoUI estadoUI);
        EstadoUI PutEstadoLocalidade(EstadoUI estadoUI);
        bool DeleteEstado(List<EstadoUI> estados);
        IEnumerable<EstadoUI> GetEstadoEstado();
        IEnumerable<EstadoUI> getEstadoByPais(int cd_pais);
        #endregion
        #region TipoEndereco
        IEnumerable<TipoEnderecoSGF> GetAllTipoEndereco();
        IEnumerable<TipoEnderecoSGF> GetTipoEnderecoSearch(SearchParameters parametros, string descricao, bool inicio);
        IEnumerable<TipoEnderecoSGF> FindTipoEndereco(string searchText);
        TipoEnderecoSGF GetTipoEnderecoById(int id);
        TipoEnderecoSGF PostTipoEndereco(TipoEnderecoSGF tipoEndereco);
        TipoEnderecoSGF PutTipoEndereco(TipoEnderecoSGF tipoEndereco);
        bool DeleteTipoEndereco(List<TipoEnderecoSGF> tiposEndereco);
        #endregion
        #region ClasseTelefone
        IEnumerable<ClasseTelefoneSGF> GetAllClasseTelefone();
        IEnumerable<ClasseTelefoneSGF> GetClasseTelefoneSearch(SearchParameters parametros, string descricao, bool inicio);
        IEnumerable<ClasseTelefoneSGF> FindClasseTelefone(string searchText);
        ClasseTelefoneSGF GetClasseTelefoneById(int id);
        ClasseTelefoneSGF PostClasseTelefone(ClasseTelefoneSGF classeTelefone);
        ClasseTelefoneSGF PutClasseTelefone(ClasseTelefoneSGF classeTelefone);
        bool DeleteClasseTelefone(List<ClasseTelefoneSGF> classeTelefone);
        #endregion
        #region TipoLogradouro
        IEnumerable<TipoLogradouroSGF> GetAllTipoLogradouro();
        IEnumerable<TipoLogradouroSGF> GetTipoLogradouroSearch(SearchParameters parametros, string descricao, bool inicio);
        IEnumerable<TipoLogradouroSGF> FindTipoLogradouro(string searchText);
        TipoLogradouroSGF GetTipoLogradouroById(int id);
        TipoLogradouroSGF PostTipoLogradouro(TipoLogradouroSGF tipoLogradouro);
        TipoLogradouroSGF PutTipoLogradouro(TipoLogradouroSGF tipoLogradouro);
        bool DeleteTipoLogradouro(List<TipoLogradouroSGF> tiposLogradouro);
        #endregion
        #region TipoTelefone
        IEnumerable<TipoTelefoneSGF> GetAllTipoTelefone();
        IEnumerable<TipoTelefoneSGF> GetTipoTelefoneSearch(SearchParameters parametros, string descricao, bool inicio);
        IEnumerable<TipoTelefoneSGF> FindTipoTelefone(string searchText);
        TipoTelefoneSGF GetTipoTelefoneById(int id);
        TipoTelefoneSGF PostTipoTelefone(TipoTelefoneSGF tipoTelefone);
        TipoTelefoneSGF PutTipoTelefone(TipoTelefoneSGF tipoTelefone);
        bool DeleteTipoTelefone(List<TipoTelefoneSGF> tiposTelefone);
        #endregion
        #region Operadora
        IEnumerable<Operadora> GetAllOperadora();
        IEnumerable<Operadora> GetAllOperadorasAtivas(int? cd_operadora);
        IEnumerable<Operadora> GetOperadoraSearch(SearchParameters parametros, string descricao, bool inicio, bool? ativa);
        IEnumerable<Operadora> FindOperadora(string searchText);
        Operadora GetOperadoraById(int id);
        Operadora PostOperadora(Operadora operadora);
        Operadora PutOperadora(Operadora operadora);
        bool DeleteOperadora(List<Operadora> operadoras);
        #endregion
        #region Atividade
             IEnumerable<Atividade> GetAllAtividade();
             IEnumerable<Atividade> GetAtividadeSearch(SearchParameters parametros, string descricao, bool inicio, bool? status, int natureza, string cnae);
             Atividade GetAtividadeById(int id);
             Atividade PostAtividade(Atividade atividadePessoa);
             Atividade PutAtividade(Atividade atividadePessoa);
             bool DeleteAtividade(List<Atividade> atividadePessoa);
        #endregion 
        #region Logradouro
        Boolean existsLocalidade(int cdLoc, String no_rua);
        IEnumerable<LocalidadeSGF> GetAllEndereco(string searchText);
        IEnumerable<LocalidadeSGF> getLogradouroSearch(SearchParameters parametros, string descricao, bool inicio, int cd_estado, int cd_cidade, int cd_bairro, string cep);
        IEnumerable<LocalidadeSGF> getAllLogradouroPorBairro(int cd_bairro);
        IEnumerable<LogradouroCEP> getLogradouroCorreio(string descricao, string estado, string cidade, string bairro, string cep, int? numero);
        #endregion

    }
}

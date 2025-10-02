using System;
using System.Collections.Generic;
using Componentes.GenericBusiness.Comum;
using Componentes.GenericDataAccess.Comum;
using System.Data.Entity;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.GenericModel.Partial;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;

namespace FundacaoFisk.SGF.Web.Services.Empresa.Comum.IDataAccess
{
    using FundacaoFisk.SGF.Web.Services.Empresa.Model;
    public interface IEmpresaDataAccess : IGenericRepository<Escola> {
        Escola getHorarioFunc(int cd_empresa);
        bool deleteAllEmpresa(List<Escola> empresas);
        Escola existsEmpresaWithCNPJ(string cnpj);
        int insertPessoaWithEmpresa(int cdEmpresa, int? nmIntegracaoCliente, int? nmEmpresaIntegracao, TimeSpan? hrInicial, TimeSpan? hrFinal, DateTime? dt_abertura, DateTime? dt_inicio);
        IEnumerable<EmpresaSession> findAllEmpresaSession();
        Escola findByIdEmpresa(int id);
        EmpresaSession findSessionByIdEmpresa(int id);
        IEnumerable<Escola> findAllEmpresaByUsuario(int codUsuario);
        IEnumerable<Escola> findAllEmpresaByUsuario(string login, bool masterGeral);
        IEnumerable<Escola> findAllEmpresaByCdUsuario(int cd_usuario, bool masterGeral);
        IEnumerable<EmpresaSession> findEmpresaSessionByLogin(string login, bool ehMaster, bool ativos);
        EmpresaSession findEmpresaSessionById(int id_empresa, int cd_usuario, bool is_master, bool is_master_geral);
        Escola firstOrDefault();
        List<Escola> findAllEmpresaByArray(int[] cdEmpresas);
        bool addEmpresaPessoa(PessoaEscola pessoaEmpresa);
        IEnumerable<EmpresaUIUsuario> findAllEmpresaByLoginPag(SearchParameters parametros, string login, bool masterGeral, List<int> empresas, String nome, string fantasia, string cnpj, bool inicio, bool editUser);
        IEnumerable<PessoaSearchUI> findAllEmpresasByUsuarioPag(SearchParameters parametros, string login, bool masterGeral, List<int> empresas, String nome, string fantasia, string cnpj, bool inicio, bool editUser, int cdEscola);
        IEnumerable<PessoaSearchComboUI> findAllEmpresasUsuarioComboFK(string login, bool masterGeral, int cdEscola);
        int findQuantidadeEmpresasVinculadasUsuario(string login, bool masterGeral, int cdEscola);
        IEnumerable<EmpresaUI> findAllEmpresaByUsuario(SearchParameters parametros, string login, bool masterGeral, string desc, int cditem);
        List<int> findAllEmpresasByUsuario(string login, bool masterGeral, int cdEscola);
        IEnumerable<Escola> findAllEmpresaAnterior(int codUsuario, bool masterGeral, int cdEmpresaAnt);
        IEnumerable<Escola> findAllEmpresa();
        IEnumerable<EmpresaUI> getEmpresaGrupo(int cd_grupo);
        IEnumerable<EmpresaUI> getEmpresaHasGrupoMaster(int cd_grupo_master);
        bool getEmpresaPropria(int cd_empresa);
        string getEmailEscola(int cd_empresa);
        bool addEmpresaPessoaBiblioteca(PessoaEscola pessoaEmpresa);
        string findByNomeEscolaComboReportView(int cdEscolaCombo);
        EscolaApiCyberBdUI getEscola(int cd_escola);

        List<int> findAllEmpresasByUser(int cd_user);
        IEnumerable<Escola> findAllEmpresaColigada(int cdEscola);

        PessoaCoordenadorCyberBdUI findPessoaCoordenadorCyberByCdPessoa(int cd_pessoa, int cd_empresa);
        IEnumerable<RelacionamentoSGF> findRelacionamentosCoordenadorByEmpresa(int cd_empresa);

        IEnumerable<EmpresaUIUsuario> findAllEmpresaTransferencia(SearchParameters parametros, int cd_empresa, String nome, string fantasia, string cnpj, bool inicio);

        EscolaApiAreaRestritaBdUI getEscolaApiAreaRestrita(int cd_escola);
    }
}

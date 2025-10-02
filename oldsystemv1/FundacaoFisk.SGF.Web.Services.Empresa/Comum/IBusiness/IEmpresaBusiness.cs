using System;
using System.Collections.Generic;
using Componentes.GenericBusiness.Comum;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;

namespace FundacaoFisk.SGF.Web.Services.Empresa.Comum.IBusiness {
    using System.Data.Entity;
    using Componentes.GenericBusiness;
    using Componentes.GenericModel;
    using FundacaoFisk.SGF.Web.Services.Empresa.Model;
    using FundacaoFisk.SGF.Web.Services.Usuario.Model;

    public interface IEmpresaBusiness : IGenericBusiness {
        //Empresa
        void sincronizaContexto(DbContext db);
        Escola getHorarioFunc(int cd_empresa);
        bool deleteAllEmpresa(List<Escola> empresas);
        EscolaApiCyberBdUI getEscola(int cd_escola);
        int insertPessoaWithEmpresa(int cd_empresa, int? nmIntegracaoCliente, int? nm_empresa_integracao, TimeSpan? hrInicial, TimeSpan? hrFinal, DateTime? dt_abertura, DateTime? dt_inicio);
        Escola existsEmpresaWithCNPJ(string cnpj);
        List<EmpresaSession> findEmpresaSessionByLogin(string login, bool ehMaster, bool ativos);
        List<EmpresaSession> findEmpresaSessionByLogin(string login, bool ehMaster, bool ativos, TransactionScopeBuilder.TransactionType TransactionType);
		IEnumerable<Escola> findAllEmpresaByUsuario(int codUsuario);
        IEnumerable<Escola> findAllEmpresaByCdUsuario(int cd_usuario);
        List<EmpresaSession> findAllEmpresaSession();
        List<EmpresaSession> findAllEmpresaSession(TransactionScopeBuilder.TransactionType TransactionType);
        Escola findByIdEmpresa(int id);
        EmpresaSession findSessionByIdEmpresa(int id);
        IEnumerable<Escola> findAllEmpresaByUsuario(string login);
        List<Escola> findAllEmpresaAnterior(int codUsuario, bool masterGeral, int cdEmpresaAnt);
        IEnumerable<Escola> findAllEmpresa();
        IEnumerable<EmpresaUI> getEmpresaGrupo(int cd_grupo);
        bool getEmpresaPropria(int cd_empresa);
        string getEmailEscola(int cd_empresa);

        //Usuario
        bool verifUsuarioAdmin(int cd_usuario);
        int getIdUsuario(string login);
        bool DeleteUsuario(List<UsuarioWebSGF> usuariosWebSGF);
        void DeleteUsuarioApiAreaRestrita(List<UsuarioWebSGF> usuariosDeleteApi);
        IEnumerable<UsuarioWebSGF> findUsuarioByEmpresaLogin(int cdEmpresa, int codUsuario, bool admGeral, bool? ativo, int? cdGrupo);
        bool VerificarMasterGeral(string login);
        UsuarioUISearch PostInsertUsuario(UsuarioWebSGF usuario, PessoaFisicaSGF pessoaFisica, int cdEmp);
        UsuarioUISearch PostEditUsuario(UsuarioWebSGF usuario, int cdEmp);
        bool VerificarMasterGeral(int cdUsuario);
        bool postEmpresaPessoa(PessoaEscola pessoaEmp);
        IEnumerable<EmpresaUI> findAllEmpresaByUsuario(SearchParameters parametros, string login, string desc, int cdItem);
        IEnumerable<EmpresaUIUsuario> findAllEmpresaByLoginPag(SearchParameters parametros, string login, List<int> empresas, String nome, string fantasia, string cnpj, bool inicio, bool editUser);
        IEnumerable<PessoaSearchUI> findAllEmpresasByUsuarioPag(SearchParameters parametros, string login, List<int> empresas, String nome, string fantasia, string cnpj, bool inicio, bool editUser, int cdEscola);

        IEnumerable<EmpresaUIUsuario> findAllEmpresaTransferencia(SearchParameters parametros, int cd_empresa, String nome, string fantasia, string cnpj, bool inicio);
        List<int> findAllEmpresasByUsuario(string login, int cdEscola);
        IEnumerable<PessoaSearchComboUI> findAllEmpresasUsuarioComboFK(string login, int cdEscola);
        int findQuantidadeEmpresasVinculadasUsuario(string login, int cdEscola);
        IEnumerable<EmpresaUI> getEmpresaHasGrupoMaster(int cd_grupo_master);
        EmpresaSession findEmpresaSessionById(int id_empresa, int cd_usuario, bool is_master, bool is_master_geral);
        bool postEmpresaPessoaBiblioteca(PessoaEscola pessoaEmp);
        string findByNomeEscolaComboReportView(int cdEscolaCombo);
        List<int> findAllEmpresasByUser(int cd_user);
        List<Escola> findAllEmpresaColigada(int cdEscola);
    
        MenusAreaRestritaRetorno getMenusAreaRestrita();
    }
}

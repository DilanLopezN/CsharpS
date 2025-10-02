using System.Collections.Generic;
using FundacaoFisk.SGF.Web.Services.Usuario.Model;
using Componentes.Utils;
using Componentes.GenericDataAccess.Comum;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.GenericModel.Partial;

namespace FundacaoFisk.SGF.Web.Services.Usuario.Comum
{
    using System;
    public interface IUsuarioDataAccess : IGenericRepository<UsuarioWebSGF>
    {
        IEnumerable<UsuarioWebSGF> GetUsuario();
        IEnumerable<UsuarioWebSGF> getUsuarios(int cd_escola);
        bool verifUsuarioAdmin(int cd_usuario);
        IEnumerable<UsuarioWebSGF> GetUsuarioByLogin(string login);
        IEnumerable<UsuarioWebSGF> GetUsuarioAuthenticateByLogin(string login);
        bool DeleteUsuario(int id);
        UsuarioWebSGF PutUsuarioSenha(UsuarioWebSGF usuario);
        PessoaFisicaSGF findIdPessoa(int? codPessoa);
        bool isValidLogin(string login);

        Boolean DeleteUsuario(List<UsuarioWebSGF> usuariosWebSGF);
        IEnumerable<UsuarioUISearch> GetUsuarioSearch(SearchParameters parametros, string descricao, string nome, bool inicio, bool? ativo, bool masterGeral, Int32[] cdEmpresas,
                                                      int empresa, bool master, bool sysAdmin, bool filtroSysAdmin);
        IEnumerable<UsuarioUISearch> getUsuarioSearchFK(SearchParameters parametros, string descricao, string nome, bool inicio, int cd_empresa);
        void postUsuarioEmpresa(UsuarioWebSGF usuarioEmpresa);
        bool VerificarMasterGeral(string login);
        bool VerificarSuperfisor(string login);
        IEnumerable<UsuarioWebSGF> findUsuarioByEmpresaLogin(int cdEmpresa, int codUsuario, bool admGeral, bool? ativo, int? cdGrupo);
        int getIdUsuario(string login);
        UsuarioWebSGF firstOrDefault();
        UsuarioWebSGF getusuarioForEdit(int idUser);
        bool VerificarMasterGeral(int cdUsuario);
        UsuarioWebSGF existsUsuarioByLoginEmail(string login, string email);
        UsuarioUISearch getUsuarioFromViewGrid(int cd_usuario, int countCdEmpresas, bool masterGeral);
        bool VerificarSupervisorByEscola(int cd_login, int cd_pessoa_empresa);
        List<UsuarioWebSGF> findUsuarioByGrupo(int cdEmpresa, int cdGrupo);
        bool verificarSysAdmin(string login);
        bool verificaExisteSysAdminAtivosEscolas(int[] cdEscolas, string no_login);
        bool verificaExisteSysAdminAtivosEscolas(string no_login);
        IEnumerable<UsuarioWebSGF> getUsuarios(int[] cdUsers);
        IEnumerable<UsuarioUISearch> getUsuarioSearchGeralFK(SearchParameters parametros, string descricao, string nome, bool inicio, int cd_empresa);
        IEnumerable<UsuarioUISearch> getUsuarioSearchAtendenteFK(SearchParameters parametros, string descricao, string nome, bool inicio, int cd_empresa);
        bool verificarTravaProfessor(int cdPessoa, int cdEscola);

        UsuarioAreaRestritaUI GetEmailUsuario(int usuario);
    }
}

using System.Collections.Generic;
using FundacaoFisk.SGF.Web.Services.Usuario.Model;
using Componentes.Utils;
using Componentes.GenericBusiness.Comum;
using System;
using System.Data.Entity;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.GenericModel.Partial;

namespace FundacaoFisk.SGF.Web.Services.Usuario.Comum
{
    public interface IUsuarioBusiness : IGenericBusiness
    {
        void sincronizaContexto(DbContext db);
        IEnumerable<UsuarioWebSGF> GetUsuario();
        IEnumerable<UsuarioWebSGF> getUsuarios(int cd_escola);
        bool verifUsuarioAdmin(int cd_usuario);
        IEnumerable<UsuarioWebSGF> GetUsuarioByLogin(string login);
        UsuarioWebSGF GetUsuarioAuthenticateByLogin(string login);
        UsuarioWebSGF PostUsuario(UsuarioWebSGF usuario);
        UsuarioWebSGF PutUsuario(UsuarioWebSGF usuario);
        void verificaSenha(string senha, bool alteracaoUsuario, string login);
        AlterarSenhaStatus PutUsuarioSenha(string login, AlterarSenha senhas, bool zerarTrocarSenha);
        void verifyExistLoginOK(string login, string nomePessoa);
        string GeraSenhaHashSHA1(string senha);
        IEnumerable<UsuarioUISearch> GetUsuarioSearch(SearchParameters parametros, string descricao, string nome, bool inicio, bool? status, string usuarioLogado, Int32[] codEscolas, int cd_empresa, bool master, bool sysAdmin, bool filtroSysAdmin);
        IEnumerable<UsuarioUISearch> getUsuarioSearchFK(SearchParameters parametros, string descricao, string nome, bool inicio, int cd_empresa);
        UsuarioWebSGF getusuarioForEdit(int idUser);
        bool VerificarMasterGeral(int cdUsuario);
        void incrementaNmTentativa(UsuarioWebSGF usuario, int maxTentativa, bool senhaValida);
        UsuarioWebSGF alterarSenhaUsuario(int tamanho, SendEmail sendEmail);
        void crudEmpresasUsuario(List<UsuarioEmpresaSGF> usuarioEmpresaView, int cd_usuario);
        void crudDireitosUsuario(List<SysDireitoUsuario> sysDireitoUsuarioView, int cd_usuario);
        void crudGrupoUsuarioUsuario(List<SysGrupoUsuario> gruposUsuarioView, int cd_usuario);
        bool VerificarSupervisorByEscola(int cd_login, int cd_pessoa_empresa);
        List<UsuarioWebSGF> findUsuarioByGrupo(int cdEmpresa, int cdGrupo);
        bool verificarSysAdmin(string login);
        bool verificaExisteSysAdminAtivosEscolas(int[] cdEscolas, string no_login);
        bool verificaExisteSysAdminAtivosEscolas(string no_login);
        IEnumerable<UsuarioUISearch> getUsuarioSearchGeralFK(SearchParameters parametros, string descricao, string nome, bool inicio, int cd_empresa);
        IEnumerable<UsuarioUISearch> getUsuarioSearchAtendenteFK(SearchParameters parametros, string descricao, string nome, bool inicio, int cd_empresa);
        bool VerificarMasterGeral(string login);
        IEnumerable<UsuarioWebSGF> findUsuarioByEmpresaLogin(int cdEmpresa, int codUsuario, bool admGeral, bool? ativo, int? cdGrupo);
        Boolean DeleteUsuario(List<UsuarioWebSGF> usuariosWebSGF);
        int getIdUsuario(string login);
        UsuarioUISearch getUsuarioFromViewGrid(int cd_usuario, int countCdEmpresas, bool masterGeral);
        bool verificarTravaProfessor(int cdPessoa, int cdEscola);

        UsuarioWebSGF findUsuarioById(int cd_usuario);
        UsuarioAreaRestritaUI GetEmailUsuario(int usuario);
    }
}

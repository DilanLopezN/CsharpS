using System;
using System.Collections.Generic;
using Componentes.Utils;
using Componentes.GenericBusiness.Comum;
using FundacaoFisk.SGF.Web.Services.Usuario.Model;
using System.Data.Entity;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Usuario.Comum
{
    public interface IPermissaoBusiness : IGenericBusiness
    {
        void sincronizaContexto(DbContext db);
        //SysMenu
        IEnumerable<SysMenuUI> GetMenusUsuario(int? codEscola, int cd_usuario, bool masterGeral);
        string GetFuncionalidadesUsuario(string login, bool masterGeral, int? cd_escola, int cd_usuario, bool eh_master);
        List<SysMenu> GetFuncionalidadesUsuarioArvore(int? cdUsuario);
        List<SysMenu> GetFuncionalidadesGrupoArvore(int? cdGrupo);
        
        //SysGrupo
        SysGrupo GetGrupoById(int id);
        SysGrupo PostGrupo(SysGrupo grupo);
        SysGrupo postGrupoEscolas(SysGrupo grupo);
        SysGrupo PutGrupo(SysGrupo grupo);
        void postSysDireitoGrupo(ICollection<SysDireitoGrupo> direitogrupo, SysGrupo grupo);
        bool DeleteGrupo(List<SysGrupo> grupos);
        void DeletePermissaoGrupo(SysGrupo grupo);
        IEnumerable<SysGrupo> GetGrupoSearch(SearchParameters parametros, string descricao, bool inicio, int? cd_pessoa_escola, int tipoGrupo);
        IEnumerable<SysGrupo> getSysGrupoByCdUsuario(int cdUsuario, int cd_empresa);
        IEnumerable<SysGrupo> GetGrupoSearch(int cd_pessoa_escola);
        SysGrupo editarGrupoEscola(SysGrupo grupo, List<Permissao> permissoes);
        IEnumerable<SysGrupo> GetGrupoFilhoById(int cd_grupo_master, bool comDireito);
        bool deleteGrupoEscola(List<SysGrupo> grupos);
        IEnumerable<SysDireitoGrupo> findAllGrupoDireito(int[] cdGrupos);
    }
}

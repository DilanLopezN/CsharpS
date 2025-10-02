using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess.Comum;
using System.Data.Entity;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Usuario.Comum
{
    public interface IGrupoDataAccess : IGenericRepository<SysGrupo>
    {
        IEnumerable<SysGrupo> GetGrupoSearch(int cd_pessoa_escola);
        List<SysGrupo> getGrupoByArray(int[] cdGrupos);
        bool DeletePermissaoGrupo(SysGrupo grupo);
        bool DeleteUsuarioGrupo(SysGrupo grupo);
        bool DeleteGrupo(List<SysGrupo> grupos);
        SysGrupo addGrupo(SysGrupo grupo);
        SysGrupo editGrupo(SysGrupo grupo);
        SysGrupo firstOrDefault();
        SysGrupo GetGrupoById(int cdGrupo, int? cdEscola);
        IEnumerable<SysGrupo> GetGrupoSearch(SearchParameters parametros, string descricao, bool inicio, int? cd_pessoa_escola, int tipoGrupo);
        IEnumerable<SysGrupo> getSysGrupoByCdUsuario(int cdUsuario, int cd_empresa);
        IEnumerable<SysGrupo> getGrupoFilhosSearch(int cd_grupo_pai);
        SysGrupo getGruposMasterById(int cdGrupo);
        IEnumerable<SysGrupo> getGrupoFilhoById(int cd_grupo_master, bool comDireito);
        SysGrupo GetGrupoMasterById(int cd_grupo);
        IEnumerable<SysGrupo> getGrupoFilhoDesmarcados(int cd_grupo_master, bool comDireito);
    }
}

using System.Collections.Generic;
using FundacaoFisk.SGF.Web.Services.Usuario.Model;
using Componentes.Utils;
using Componentes.GenericDataAccess.Comum;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Usuario.Comum
{
    using System;
    using System.Linq;
    public interface ISysDireitoGrupoDataAccess : IGenericRepository<SysDireitoGrupo>
    {
        SysDireitoGrupo findGrupoDireito(int cdGrupo, int cdMenu);
        IQueryable<SysDireitoGrupo> findGrupoDireito(int cdGrupo);
        IEnumerable<SysDireitoGrupo> findAllGrupoDireito(int[] cdGrupos);
    }
}

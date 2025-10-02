using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using System.Data.Entity;
using Componentes.Utils;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess
{
    using FundacaoFisk.SGF.GenericModel;
    using Componentes.GenericDataAccess.Comum;

    public interface IGrupoEstoqueDataAccess : IGenericRepository<GrupoEstoque>
    {
        IEnumerable<GrupoEstoque> GetGrupoEstoqueSearch(SearchParameters parametros, String descricao, Boolean inicio, Boolean? ativo, int categoria);
        Boolean deleteAllGrupo(List<GrupoEstoque> grupos);
        List<GrupoEstoque> findAllGrupoAtivo(int cdGrupo, bool isMasterGeral);
        List<GrupoEstoque> findAllGrupoWithItem(int cd_pessoa_escola, bool isMaster);
    }
}

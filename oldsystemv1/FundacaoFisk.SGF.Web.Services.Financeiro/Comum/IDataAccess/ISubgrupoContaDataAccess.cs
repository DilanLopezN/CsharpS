using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using Componentes.GenericDataAccess.Comum;
using System.Data.Entity;
using Componentes.Utils;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess
{
    using FundacaoFisk.SGF.GenericModel;
    using FundacaoFisk.SGF.Web.Services.Financeiro.Model;
    public interface ISubgrupoContaDataAccess : IGenericRepository<SubgrupoConta>
    {
        IEnumerable<SubGrupoSort> GetSubgrupoContaSearch(SearchParameters parametros, string descricao, Boolean inicio, int cdGrupo, SubgrupoConta.TipoNivelConsulta tipo);
        Boolean deleteAllSubgrupoConta(List<SubgrupoConta> subgrupoConta);
        IEnumerable<SubgrupoConta> getSubgruposPorCodGrupoContas(int cdGrupoContas);
        SubGrupoSort getSubgrupoPorNivelEId(SubgrupoConta.TipoNivelConsulta nivel, int cdSubgrupo);
        IEnumerable<SubgrupoConta> getSubgruposContaAll(List<int> codigosSubGrupos);
    }
}

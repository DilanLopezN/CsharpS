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
    using FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess;
    public interface ITipoLiquidacaoDataAccess : IGenericRepository<TipoLiquidacao>
    {
        IEnumerable<TipoLiquidacao> GetTipoLiquidacaoSearch(SearchParameters parametros, String descricao, Boolean inicio, Boolean? ativo);
        Boolean deleteAllTipoLiquidacao(List<TipoLiquidacao> tiposLiquidacao);
        IEnumerable<TipoLiquidacao> getTipoLiquidacao(TipoLiquidacaoDataAccess.TipoConsultaTipoLiquidacaoEnum hasDependente, int? cd_tipo_liquidacao);
        IEnumerable<TipoLiquidacao> getTipoLiquidacao();
        IEnumerable<TipoLiquidacao> getTipoLiquidacaoCd(int? cdTipoLiq);
        IEnumerable<TipoLiquidacao> getTipoLiquidacaoCCByLocalMovto(int cd_local_movto);
    }
}

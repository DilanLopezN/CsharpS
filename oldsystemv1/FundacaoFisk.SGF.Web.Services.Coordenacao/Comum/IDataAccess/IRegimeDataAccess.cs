using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Componentes.GenericDataAccess.Comum;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess
{
    public interface IRegimeDataAccess : IGenericRepository<Regime>
    {
        IEnumerable<Regime> getRegimeDesc(SearchParameters parametros, String desc, String abrev, Boolean inicio, Boolean? ativo);
        Boolean deleteAllRegime(List<Regime> regimes);
        IEnumerable<Regime> getRegimeTabelaPreco();
        IEnumerable<Regime> getRegimes(RegimeDataAccess.TipoConsultaRegimeEnum hasDependente, int? cd_regime);
    }
}

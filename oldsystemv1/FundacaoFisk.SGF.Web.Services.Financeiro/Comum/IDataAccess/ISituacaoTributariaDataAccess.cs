using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using System.Data.Entity;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess
{
    using FundacaoFisk.SGF.GenericModel;
    using Componentes.GenericDataAccess.Comum;
    using FundacaoFisk.SGF.Web.Services.Financeiro.Model;
    using FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess;

    public interface ISituacaoTributariaDataAccess : IGenericRepository<SituacaoTributaria>
    {
        IEnumerable<SituacaoTributaria> getSituacaoTributaria(SituacaoTributariaDataAccess.TipoConsultaSitTribEnum tipo, List<int> cd_situacoes, int cdTpNF);
        SituacaoTributaria getSituacaoTributariaItem(int cd_grupo_estoque, int id_regime_tributario, int cdSitTrib);
        IEnumerable<SituacaoTributaria> getSituacaoTributariaTipo(SituacaoTributariaDataAccess.TipoConsultaSitTribEnum tipo, List<int> cd_situacoes, int tipoImp, int cd_escola,
            byte id_regime_trib,bool master_geral);
        SituacaoTributaria getSituacaoTributariaFormaTrib(int cd_situacao_trib);
    }
}

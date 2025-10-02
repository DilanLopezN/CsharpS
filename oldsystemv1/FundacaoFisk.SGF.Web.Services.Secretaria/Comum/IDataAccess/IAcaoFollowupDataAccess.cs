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

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Comum
{
    using FundacaoFisk.SGF.GenericModel;
    using FundacaoFisk.SGF.Web.Services.Secretaria.Business;
    using FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess;
    public interface IAcaoFollowupDataAccess : IGenericRepository<AcaoFollowUp>
    {
        IEnumerable<AcaoFollowUp> GetAcaoFollowUpSearch(SearchParameters parametros, string descricao, bool inicio, bool? ativo);
        IEnumerable<AcaoFollowUp> getAcaoFollowUp(AcaoFollowupDataAccess.TipoPesquisaAcaoEnum tipo, int cd_acao_follow_up);
    }
}

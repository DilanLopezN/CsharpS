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
    public interface IMotivoCancelamentoBolsaDataAccess : IGenericRepository<MotivoCancelamentoBolsa>
    {
        //Motivo Cancelamento Bolsa
        IEnumerable<MotivoCancelamentoBolsa> GetMotivoCancelBolsaSearch(SearchParameters parametros, string descricao, bool inicio, bool? ativo);
        bool deleteAll(List<MotivoCancelamentoBolsa> motivosCancelamentos);
        IEnumerable<MotivoCancelamentoBolsa> getMotivoCancelamentoBolsa(bool? status, int? cd_motivo_cancelamento_bolsa);
    }
}

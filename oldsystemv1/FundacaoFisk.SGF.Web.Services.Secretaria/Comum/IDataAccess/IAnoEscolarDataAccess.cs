using Componentes.GenericDataAccess.Comum;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Comum.IDataAccess
{
    public interface IAnoEscolarDataAccess : IGenericRepository<AnoEscolar>
    {
        IEnumerable<AnoEscolar> GetAnoEscolarSearch(SearchParameters parametros, int? cdEscolaridade, string descricao, bool inicio, bool? ativo);
        IEnumerable<Escolaridade> GetEscolaridadePossuiAnoEscolar();
        AnoEscolar GetAnoEscolarById(int id);
        int GetUltimoNmOrdem(int cd_escolaridade);
        IEnumerable<AnoEscolar> getAnoEscolaresAtivos(int? cdAnoEscolar);
    }
}

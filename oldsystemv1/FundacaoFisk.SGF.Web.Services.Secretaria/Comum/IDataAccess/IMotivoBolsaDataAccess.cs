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
    public interface IMotivoBolsaDataAccess : IGenericRepository<MotivoBolsa>
    {
        //Motivo Bolsa
        IEnumerable<MotivoBolsa> GetMotivoBolsaSearch(SearchParameters parametros, string descricao, bool inicio, bool? ativo);
        bool deleteAll(List<MotivoBolsa> midias);
        IEnumerable<MotivoBolsa> getMotivoBolsa(bool? status, int? cd_motivo_bolsa);
    }
}

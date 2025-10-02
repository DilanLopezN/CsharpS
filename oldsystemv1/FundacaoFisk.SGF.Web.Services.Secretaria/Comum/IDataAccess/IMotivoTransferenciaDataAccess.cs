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
    public interface IMotivoTransferenciaDataAccess : IGenericRepository<MotivoTransferenciaAluno>
    {
        //Motivo Não Matricula
        IEnumerable<MotivoTransferenciaAluno> GetMotivoTransferenciaSearch(SearchParameters parametros, string descricao, bool inicio, bool? ativo);
        bool deleteAll(List<MotivoTransferenciaAluno> motivosTransferencia);
        IEnumerable<MotivoTransferenciaAluno> getMotivosTransferencia();

    }
}

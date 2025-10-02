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

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess
{
    using FundacaoFisk.SGF.GenericModel;
    public interface IAvaliacaoParticipacaoVincDataAccess : IGenericRepository<AvaliacaoParticipacaoVinc>
    {
        int verificaMaiorQntParticipacao(int idProduto, int cdEscola);
        IEnumerable<AvaliacaoParticipacaoVinc> getAvalPartVinc(int cdAvalPart, int cdEscola);
    }
}

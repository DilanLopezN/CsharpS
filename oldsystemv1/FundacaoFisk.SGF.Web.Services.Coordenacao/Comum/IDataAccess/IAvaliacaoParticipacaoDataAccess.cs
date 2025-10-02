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
    using FundacaoFisk.SGF.Services.Coordenacao.Model;
    public interface IAvaliacaoParticipacaoDataAccess : IGenericRepository<AvaliacaoParticipacao>
    {
        IEnumerable<AvaliacaoParticipacaoUI> searchAvaliacaoParticipacao(Componentes.Utils.SearchParameters parametros, int cdCriterio, int cdParticipacao, int cdProduto, bool? ativo, int cdEscola);
        IEnumerable<AvaliacaoParticipacao> getAvaliacaoParticipacaoById(int cdAvalPart, int cdEscola);
        AvaliacaoParticipacao getAvaliacaoParticipacaoByEditar(int cdAvalPart, int cdEscola);
        bool verificaAvaliacaoParticipacaoByCriterio(int cd_criterio_avaliacao);
    }
}

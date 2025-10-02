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
    public interface IParticipacaoDataAccess : IGenericRepository<Participacao>
    {
        List<Participacao> getParticipacaoAvaliacaoPart(int cdEscola);
        List<Participacao> getParticipacaoByAvaliacao(int cdAvalPart, int cdEscola);
        List<Participacao> getParticipacoes(List<int> cdsPart);
        IEnumerable<Participacao> getParticipacaoSearch(SearchParameters parametros, string desc, bool inicio, bool? status);
    }
}

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
    public interface ICriterioAvaliacaoDataAccess : IGenericRepository<CriterioAvaliacao>
    {
        IEnumerable<CriterioAvaliacao> GetCriterioAvaliacaoSearch(SearchParameters parametros, string descricao, string abrev, bool inicio, bool? ativo, bool? conceito, bool IsParticipacao);
        CriterioAvaliacao firstOrDefault();
        bool deleteAllCriterioAvaliacao(List<CriterioAvaliacao> criteriosAvaliacao);
        IEnumerable<CriterioAvaliacao> getAllCriteriosAtivos();
        List<CriterioAvaliacao> getAvaliacaoCriterio(int? cd_tipo_avaliacao, int? cd_criterio_avaliacao);
        IEnumerable<CriterioAvaliacao> getNomesAvaliacao();
        IEnumerable<CriterioAvaliacao> getNomesAvaliacao(int cd_tipo_avaliacao);
        IEnumerable<CriterioAvaliacao> getCriteriosPorAvalPart(int cdEscola);
        IEnumerable<CriterioAvaliacao> getNomesAvaliacaoByAval(int? cdCriterio);
        
    }
}

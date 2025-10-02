using System.Collections.Generic;
using Componentes.GenericDataAccess.Comum;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Services.Coordenacao.Model;

namespace FundacaoFisk.SGF.Services.Coordenacao.Comum.IDataAccess
{
    public interface IMensagemAvaliacaoDataAccess : IGenericRepository<MensagemAvaliacao>
    {
        IEnumerable<MensagemAvaliacaoSearchUI> getMensagemAvaliacaoSearch(SearchParameters parametros, string desc, bool inicio, bool? status, int? produto, int? curso);
        List<MensagemAvaliacaoSearchUI> getMensagensAvaliacaoByIds(List<int> cdsMensagens);
        List<MensagemAvaliacaoSearchUI> getMensagensAvaliacaoById(int cd_mensagem_avaliacao);
    }
}
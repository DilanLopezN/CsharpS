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
    using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;
    public interface ITipoAvaliacaoDataAccess : IGenericRepository<TipoAvaliacao>
    {
        IEnumerable<TipoAvaliacao> GetTipoAvaliacaoSearch(SearchParameters parametros, string descricao, bool inicio, bool? conceito, int? cd_tipo_avaliacao, int? cd_criterio_avaliacao, int cdCurso, int cdProduto);
        TipoAvaliacao firstOrDefault();
        bool deleteAllTipoAvaliacao(List<TipoAvaliacao> TiposAvaliacao);
        bool deleteCursoTipoAvaliacao(TipoAvaliacao tipoAvaliacao);
        int? getTotalNotaTipoAvaliacao(int idtipoAvaliacao);
        List<TipoAvaliacao> getTipoAvaliacao();
        IEnumerable<TipoAvaliacao> getTipoAvaliacao(bool? ativo, int idTipoAvaliacao);
        IEnumerable<TipoAvaliacao> getTipoAvaliacaoAvaliacaoTurma();
        List<TipoAvaliacaoTurma> tiposAvaliacao(int cd_turma, int cd_escola, bool id_conceito);
    }
}

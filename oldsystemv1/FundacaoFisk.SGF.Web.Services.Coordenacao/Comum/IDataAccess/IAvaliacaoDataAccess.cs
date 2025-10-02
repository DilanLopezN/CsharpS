using Componentes.GenericDataAccess.Comum;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;
using System;
using System.Collections.Generic;
using System.Data;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess
{
    public interface IAvaliacaoDataAccess: IGenericRepository<Avaliacao>
    {
        IEnumerable<AvaliacaoUI> searchAvaliacao(SearchParameters parametros, string descAbreviado, int? idTipoAvaliacao, int? idCriterio, bool inicio, bool? status);
        IEnumerable<Avaliacao> getAvaliacaoOrdem(int idTipoAvaliacao, int idCriterio,  bool? status);
        byte? getOrdem(int tipoAvaliacao, int criterio);
        void editOrdemAvaliacao(Avaliacao avaliacao);
        bool deleteAllAvaliacao(List<Avaliacao> avaliacoes);
        byte? getSomatorio(int idTipoAvaliacao, int idCriterio);
        byte? getNotaAvaliacao(int idAvaliacao);
        IEnumerable<AvaliacaoUI> getAvaliacaoByIdTipoAvaliacao(int idTipoAvaliacao);
        IEnumerable<Avaliacao> getAvaliacoesCursoSemAvaliacaoTurma(int idTurma);
        bool existNotaLancadaAvaliacaoTurma(int cd_avaliacao, int cd_escola);
        bool existNotaLancadaAvaliacaoTurma(List<int> cdsAvaliacoes);
        IEnumerable<Avaliacao> findByIdTipoAvaliacao(int cd_tipo_avaliacao);
        int getAvaliacaoCursoExistsTurmaWithCurso(int cd_turma, int cd_escola);
        IEnumerable<Avaliacao> getAvaliacaoECriterioTurma(int cd_turma, int cd_escola);
        DataTable getRptAvaliacao(int cd_turma, int cdCurso, int cdProduto, int cdEscola, int cdFuncionario, int tipoTurma, byte sitTurma, DateTime? pDtIni, DateTime? pDtFim, bool isConceito);
        DataTable getRptAvaliacaoTurma(int cd_turma);
        DataTable getRptAvaliacaoTurmaConceito(int cd_turma);
    }
}

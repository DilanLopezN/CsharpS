using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Componentes.GenericDataAccess.Comum;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess
{
    public interface IConceitoDataAccess : IGenericRepository<Conceito>
    {
        IEnumerable<ConceitoSearchUI> getConceitoDesc(SearchParameters parametros, String nome, Boolean inicio, Boolean? ativo, int cdProduto);
        Boolean deleteAllConceito(List<Conceito> conceitos);
        IEnumerable<Conceito> findConceitosAtivos(int idProduto, int idConceito);
        double somaParticipacaoPorConceito(int idProduto, int? cdConceito);
        List<Conceito> getConceitosDisponiveisByProdutoTurma(int cd_turma);
    }
}

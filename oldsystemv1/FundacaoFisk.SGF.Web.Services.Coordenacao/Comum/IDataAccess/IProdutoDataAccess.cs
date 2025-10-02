using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess.Comum;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess
{
    public interface IProdutoDataAccess : IGenericRepository<Produto>
    {
        IEnumerable<Produto> getProdutoDesc(SearchParameters parametros, String desc, string abrev, Boolean inicio, Boolean? ativo);
        IEnumerable<Produto> findProduto(ProdutoDataAccess.TipoConsultaProdutoEnum hasDependente, int? cd_produto, int? cd_escola);
        IEnumerable<Produto> findProdutoAulaPersonalizada(int cd_aluno, int cd_escola);
        Boolean deleteAllProduto(List<Produto> produtos);
        IEnumerable<Produto> findProdutoTabela(int cdEscola);
        IEnumerable<Produto> getProdutosWithAtividadeExtra(int cd_pessoa_escola, bool isAtivo);
    }
}

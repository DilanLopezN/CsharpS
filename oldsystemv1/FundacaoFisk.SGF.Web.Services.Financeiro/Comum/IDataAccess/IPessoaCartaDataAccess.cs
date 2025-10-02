using Componentes.GenericDataAccess.Comum;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess
{
    public interface IPessoaCartaDataAccess : IGenericRepository<PessoaCarta>
    {
        PessoaCarta findByIdPessoaAndAno( int idPessoa, int ano);
    }
}
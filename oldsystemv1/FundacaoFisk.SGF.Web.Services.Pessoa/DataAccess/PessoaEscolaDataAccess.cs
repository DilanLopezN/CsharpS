using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Pessoa.Comum.DataAccess;

namespace FundacaoFisk.SGF.Web.Services.Pessoa.DataAccess
{
    public class PessoaEscolaDataAccess : GenericRepository<PessoaEscola>, IPessoaEscolaDataAccess
    {
        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }
    }
}
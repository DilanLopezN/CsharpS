using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Pessoa.Comum.DataAccess;
using log4net;

namespace FundacaoFisk.SGF.Web.Services.Pessoa.DataAccess
{
    public class PessoaRafDataAccess : GenericRepository<PessoaRaf>, IPessoaRafDataAccess
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(PessoaDataAccess));

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
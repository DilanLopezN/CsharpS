using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Secretaria.Comum.IDataAccess;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess
{
    public class PessoaPromocaoDataAccess : GenericRepository<PessoaPromocao>, IPessoaPromocaoDataAccess
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
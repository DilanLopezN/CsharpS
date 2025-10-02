using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.EmailMarketing.Comum.IDataAccess;
using Componentes.GenericDataAccess.GenericException;

namespace FundacaoFisk.SGF.Web.Service.EmailMarketing.DataAccess
{
    class MalaDiretaCadastroDataAccess : GenericRepository<MalaDiretaCadastro>, IMalaDiretaCadastroDataAccess
    {

        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }
    }
}

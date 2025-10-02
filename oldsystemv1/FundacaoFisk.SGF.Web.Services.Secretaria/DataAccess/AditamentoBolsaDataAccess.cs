using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Secretaria.Comum.IDataAccess;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess
{
    public class AditamentoBolsaDataAccess : GenericRepository<AditamentoBolsa>, IAditamentoBolsaDataAccess
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

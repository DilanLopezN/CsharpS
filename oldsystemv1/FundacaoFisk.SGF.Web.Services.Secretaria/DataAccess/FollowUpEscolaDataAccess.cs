using System.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using Componentes.GenericModel;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Secretaria.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess
{
    public class FollowUpEscolaDataAccess : GenericRepository<FollowUpEscola>, IFollowUpEscolaDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }

        public IEnumerable<FollowUpEscola> getFollowUpEscola(int cd_follow_up)
        {
            try
            {
                var sql = from flwe in db.FollowUpEscola
                          where flwe.cd_follow_up == cd_follow_up
                          select flwe;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        
    }
}

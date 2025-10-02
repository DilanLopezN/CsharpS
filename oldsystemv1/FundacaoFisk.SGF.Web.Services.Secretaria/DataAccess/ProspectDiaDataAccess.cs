using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Secretaria.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Componentes.GenericDataAccess.GenericException;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess
{
    public class ProspectDiaDataAccess : GenericRepository<ProspectDia>, IProspectDiaDataAccess
    {
        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }

        public List<ProspectDia> searchProspectDia(int cdProspect)
        {
            try
            {
                var sql = from prospectDia in db.ProspectDia
                          join prospect in db.Prospect on prospectDia.cd_prospect equals prospect.cd_prospect
                          where prospectDia.cd_prospect == cdProspect
                          select prospectDia;
                return sql.ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

    }
}

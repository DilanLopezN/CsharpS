using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Secretaria.Comum.IDataAcess;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess
{
    public class ProspectPeriodoDataAccess: GenericRepository<ProspectPeriodo>, IProspectPeriodoDataAccess
    {
       

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }

        /// <summary>
        /// Retona uma lista de Periodos do prospect
        /// </summary>
        /// <param name="cdProspect"></param>
        /// <returns></returns>
        public List<ProspectPeriodo> getPeriodoProspect(int cdProspect)
        {
            try
            {
                var sql = from p in db.ProspectPeriodo
                          where p.cd_prospect == cdProspect
                          select p;
                return sql.ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
      
    }

}

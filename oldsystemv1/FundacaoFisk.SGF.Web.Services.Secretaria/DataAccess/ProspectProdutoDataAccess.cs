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
    public class ProspectProdutoDataAccess: GenericRepository<ProspectProduto>, IProspectProdutoDataAccess
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
        /// Retona uma lista de produtos do prospect
        /// </summary>
        /// <param name="cdProspect"></param>
        /// <returns></returns>
        public List<ProspectProduto> getProdutoProspect(int cdProspect)
        {
            try
            {
                var sql = from p in db.ProspectProduto
                          where p.cd_prospect == cdProspect
                          select p;
                return sql.ToList();

            }
            catch (Exception e)
            {

                throw new DataAccessException(e);
            }
        }
    }

}

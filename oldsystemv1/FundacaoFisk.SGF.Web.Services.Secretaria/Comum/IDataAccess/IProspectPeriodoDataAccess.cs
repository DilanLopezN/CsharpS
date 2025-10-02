using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Componentes.GenericDataAccess.Comum;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Comum.IDataAcess
{
    public interface IProspectPeriodoDataAccess : IGenericRepository<ProspectPeriodo>
    {
        List<ProspectPeriodo> getPeriodoProspect(int cdProspect);
    }
}

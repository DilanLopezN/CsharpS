using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using Componentes.GenericDataAccess.Comum;
using System.Data.Entity;
using Componentes.Utils;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Comum.IDataAccess
{
    using FundacaoFisk.SGF.GenericModel;
    using FundacaoFisk.SGF.Web.Services.Secretaria.Model;
    public interface IFollowUpEscolaDataAccess : IGenericRepository<FollowUpEscola>
    {
        IEnumerable<FollowUpEscola> getFollowUpEscola(int cd_follow_up);
        
    }
}

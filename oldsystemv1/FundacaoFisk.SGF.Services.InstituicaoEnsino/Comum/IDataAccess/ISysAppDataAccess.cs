using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess.Comum;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.InstituicaoEnsino.DataAccess;

namespace FundacaoFisk.SGF.Web.Services.InstituicaoEnsino.Comum.IDataAccess
{
    public interface ISysAppDataAccess : IGenericRepository<SysApp>
    {
        String getRodapeSysApp();
        SysApp getConfigEmailMarketingSysApp();
        SysApp getSysApp();
        string getVersoCartaoPostal();
    }
}

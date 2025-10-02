using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvcTurbine.ComponentModel;
using System.Collections.Generic;
using FundacaoFisk.SGF.GenericModel;
using Componentes.GenericBusiness.Comum;
using Componentes.GenericBusiness;
using FundacaoFisk.SGF.Web.Services.CNAB.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.CNAB.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.CNAB.DataAccess;
using FundacaoFisk.SGF.Web.Services.CNAB.Business;

namespace FundacaoFisk.SGF.Web.Services.CNAB.Registration
{
    public class CNABServiceRegistration : IServiceRegistration
    {
        public void Register(IServiceLocator locator)
        {
            locator.Register<ICnabDataAccess, CnabDataAccess>();
            locator.Register<ITituloCnabDataAccess, TituloCnabDataAccess>();
            locator.Register<IBoletoBusiness, BoletoBusiness>();
            locator.Register<ICnabBusiness, CnabBusiness>();
            locator.Register<ICarteiraCnabDataAccess, CarteiraCnabDataAccess>();
            locator.Register<IRetornoCNABDataAccess, RetornoCNABDataAccess>();
            locator.Register<ITituloRetornoCnabDataAccess, TituloRetornoCNABDataAccess>();
        }
    }
}

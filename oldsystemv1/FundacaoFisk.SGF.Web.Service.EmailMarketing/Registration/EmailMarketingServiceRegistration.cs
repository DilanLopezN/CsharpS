using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvcTurbine.ComponentModel;
using System.Collections.Generic;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.EmailMarketing.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.EmailMarketing.Business;
//using FundacaoFisk.SGF.Web.Services.EmailMarketing.DataAccess;
using FundacaoFisk.SGF.Web.Services.EmailMarketing.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Service.EmailMarketing.DataAccess;



namespace FundacaoFisk.SGF.Web.Services.EmailMarketing.Registration
{
    public class EmailMarketingServiceRegistration : IServiceRegistration
    {
        public void Register(IServiceLocator locator)
        {
            // Business
            locator.Register<IEmailMarketingBusiness, EmailMarketingBusiness>();
            //DataAccess
            locator.Register<IListaEnderecoMalaDataAccess, ListaEnderecoMalaDataAccess>();
            locator.Register<IListaNaoInscritoDataAccess, ListaNaoInscritoDataAccess>();
            locator.Register<IMalaDiretaDataAccess, MalaDiretaDataAccess>();
        }
    }
}

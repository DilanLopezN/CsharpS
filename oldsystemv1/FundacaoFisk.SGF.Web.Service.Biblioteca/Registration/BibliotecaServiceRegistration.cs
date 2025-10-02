using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvcTurbine.ComponentModel;
using System.Collections.Generic;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Biblioteca.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Biblioteca.Business;
using FundacaoFisk.SGF.Web.Services.Biblioteca.DataAccess;
using FundacaoFisk.SGF.Web.Services.Biblioteca.Comum.IDataAccess;



namespace FundacaoFisk.SGF.Web.Services.Biblioteca.Registration
{
    public class BibliotecaServiceRegistration : IServiceRegistration
    {
        public void Register(IServiceLocator locator)
        {
            // Business
            locator.Register<IBibliotecaBusiness, BibliotecaBusiness>();
            locator.Register<IEmprestimoDataAccess, EmprestimoDataAccess>();
        }
    }
}

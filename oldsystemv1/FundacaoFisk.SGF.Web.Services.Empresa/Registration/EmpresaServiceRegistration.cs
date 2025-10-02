using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvcTurbine.ComponentModel;
using System.Collections.Generic;
using Componentes.GenericBusiness.Comum;
using Componentes.GenericBusiness;
using FundacaoFisk.SGF.Web.Services.Empresa.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Empresa.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Empresa.DataAccess;
using FundacaoFisk.SGF.Web.Services.Empresa.Business;

namespace FundacaoFisk.SGF.Web.Services.Empresa.Registration {
    public class EmpresaServiceRegistration : IServiceRegistration {
        public void Register(IServiceLocator locator) {
            locator.Register<IEmpresaDataAccess, EmpresaDataAccess>();
            locator.Register<IFuncionarioDataAccess, FuncionarioDataAccess>();
            locator.Register<IFuncionarioComissaoDataAccess, FuncionarioComissaoDataAccess>();

            locator.Register<IEmpresaBusiness, EmpresaBusiness>();
            locator.Register<IFuncionarioBusiness, FuncionarioBusiness>();
        }
    }
}

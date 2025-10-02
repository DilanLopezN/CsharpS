using System.Data;
using MvcTurbine.ComponentModel;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Auth.Comum;
using FundacaoFisk.SGF.Web.Services.Auth.Business;

namespace FundacaoFisk.SGF.Web.Services.Auth.Registration {
    public class AuthServiceRegistration : IServiceRegistration {
        public void Register(IServiceLocator locator) {
            locator.Register<IAuthBusiness, AuthBusiness>();
            locator.Register<IAesCryptoHelper, AesCryptoHelper>();
        }
    }
}
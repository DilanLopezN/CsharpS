using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvcTurbine.ComponentModel;
using System.Collections.Generic;
using FundacaoFisk.SGF.GenericModel;
using Componentes.GenericBusiness.Comum;
using Componentes.GenericBusiness;
using FundacaoFisk.SGF.Web.Services.Pessoa.Comum;
using FundacaoFisk.SGF.Web.Services.Pessoa.DataAccess;
using FundacaoFisk.SGF.Web.Services.InstituicaoEnsino.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.InstituicaoEnsino.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.InstituicaoEnsino.DataAccess;
using FundacaoFisk.SGF.Web.Services.InstituicaoEnsino.Business;
using FundacaoFisk.SGF.Web.Services.Empresa.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Empresa.Business;

namespace FundacaoFisk.SGF.Web.Services.InstituicaoEnsino.Registration
{
    public class EscolaServiceRegistration : IServiceRegistration
    {
        public void Register(IServiceLocator locator)
        {
            //Escola
            locator.Register<IEscolaDataAccess, EscolaDataAccess>();
            locator.Register<IEscolaBusiness, EscolaBusiness>();
           
            //parametros
            locator.Register<IParametrosDataAccess, ParametrosDataAccess>();
            locator.Register<ISysAppDataAccess, SysAppDataAccess>();
        }
    }
}

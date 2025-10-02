using System.Reflection;
using System.Web;
using System.Web.Mvc;
using SimpleInjector;
using SimpleInjector.Advanced;
using SimpleInjector.Integration.Web;
using SimpleInjector.Integration.Web.Mvc;
using WebActivator;
using FundacaoFisk.SGF.Infra.IoC;

[assembly: PostApplicationStartMethod(typeof(FundacaoFisk.SGF.Web.SimpleInjectorInitializer), "Initialize")]

namespace FundacaoFisk.SGF.Web
{
    public static class SimpleInjectorInitializer
    {
        public static void Initialize()
        {
            var container = new Container();
            container.Options.DefaultScopedLifestyle = new WebRequestLifestyle();
            
            // Chamada dos módulos do Simple Injector
            InitializeContainer(container);

            // Necessário para registrar o ambiente do Owin que é dependência do Identity
            // Feito fora da camada de IoC para não levar o System.Web para fora
            //container.RegisterPerWebRequest(() =>
            //{
            //    if (HttpContext.Current != null && HttpContext.Current.Items["owin.Environment"] == null && container.IsVerifying())
            //    {
            //        return new OwinContext().Authentication;
            //    }
            //    return HttpContext.Current.GetOwinContext().Authentication;

            //});

            container.RegisterMvcControllers(Assembly.GetExecutingAssembly());
            
            container.Verify();
            
            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));
        }
     
        private static void InitializeContainer(Container container)
        {
            IoC.Start(container);
            //IOC.RegisterServices(container);
        }
    }
}
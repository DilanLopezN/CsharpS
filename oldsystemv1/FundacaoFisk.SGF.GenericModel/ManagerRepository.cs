using Componentes.GenericDataAccess.Comum;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;
using System.Data.Entity;
using System.Web;

namespace FundacaoFisk.SGF.GenericModel
{
    public class ManagerRepositoryHttp : IManagerRepository
    {
        public const string ContextoHttp = "ContextoHttp";

        public DbContext getContexto()
        {
            if (HttpContext.Current.Items[ContextoHttp] == null)
            {
                HttpContext.Current.Items[ContextoHttp] = new SGFWebContext();
            }
            return HttpContext.Current.Items[ContextoHttp] as SGFWebContext;
        }

        public DbContext getContextoComponent()
        {
            if (HttpContext.Current.Items[ContextoHttp] == null)
            {
                HttpContext.Current.Items[ContextoHttp] = new ComponentesWebContext();
            }
            return HttpContext.Current.Items[ContextoHttp] as ComponentesWebContext;
        }

        #region IGerenciadorDeRepositorio Members

        public void Finalizar()
        {
            if (HttpContext.Current.Items[ContextoHttp] != null)
            {
                (HttpContext.Current.Items[ContextoHttp] as SGFWebContext).Dispose();
            }
        }

        #endregion
    }
}
using Componentes.GenericBusiness.Comum;
using FundacaoFisk.SGF.GenericModel.Partial;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IBusiness
{
    public interface IApiPromocaoIntercambioBusiness : IGenericBusiness
    {

        string postExecutaRequestPromocaoIntercambio(PromocaoIntercambioParams parametros);
        void ValidaParametros(PromocaoIntercambioParams parametros);
        bool aplicaApiPromocao();
    }
}
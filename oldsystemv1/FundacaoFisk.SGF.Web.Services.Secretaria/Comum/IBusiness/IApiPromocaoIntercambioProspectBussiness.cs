using Componentes.GenericBusiness.Comum;
using FundacaoFisk.SGF.GenericModel.Partial;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Comum
{
    public interface IApiPromocaoIntercambioProspectBussiness : IGenericBusiness
    {

        string postExecutaRequestPromocaoIntercambio(PromocaoIntercambioParams parametros);
        void ValidaParametros(PromocaoIntercambioParams parametros);
        bool aplicaApiPromocao();
    }
}
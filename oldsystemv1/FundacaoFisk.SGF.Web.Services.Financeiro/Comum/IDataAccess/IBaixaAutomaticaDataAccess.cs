using System.Collections.Generic;
using Componentes.GenericDataAccess.Comum;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Financeiro.Model;
using Componentes.Utils;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess
{
    public interface IBaixaAutomaticaDataAccess : IGenericRepository<BaixaAutomatica>
    {
        IEnumerable<BaixaAutomatica> listarBaixaAutomaticasEfetuadasCheque(SearchParameters parametros, BaixaAutomaticaUI baixaAutomaticaChequeUI);
        IEnumerable<BaixaAutomatica> listarBaixaAutomaticasEfetuadasCartao(SearchParameters parametros, BaixaAutomaticaUI baixaAutomaticaChequeUI);
    }
}
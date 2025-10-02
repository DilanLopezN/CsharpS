using Componentes.GenericDataAccess.Comum;
using FundacaoFisk.SGF.GenericModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess
{
    public interface ITituloAditamento : IGenericRepository<TituloAditamento>
    {
        IEnumerable<TituloAditamento> ObterTitulosAditamentoPorId(int aditamentoId);
        bool ExisteTitulo(int cd_titulo);
    }
}

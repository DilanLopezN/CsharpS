using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess
{
    using Componentes.GenericDataAccess.Comum;
    using FundacaoFisk.SGF.GenericModel;

    public interface IObsSaldoCaixa : IGenericRepository<ObsSaldoCaixa>
    {
        ObsSaldoCaixa getObsSaldoCaixaUsuario(int cd_pessoa_escola, int cd_local_movimento, DateTime dta_fechamento, int cdUsuario);
    }
}

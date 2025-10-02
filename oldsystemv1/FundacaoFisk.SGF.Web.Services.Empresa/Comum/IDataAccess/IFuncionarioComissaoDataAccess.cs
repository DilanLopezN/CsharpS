using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess.Comum;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Empresa.Comum.IDataAccess
{
    public interface IFuncionarioComissaoDataAccess : IGenericRepository<FuncionarioComissao>
    {
        List<FuncionarioComissao> getFuncionarioComissao(int cd_func);
    }
}

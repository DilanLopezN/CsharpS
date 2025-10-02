using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Componentes.GenericDataAccess.Comum;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Comum.IDataAcess
{
    public interface IProspectProdutoDataAccess : IGenericRepository<ProspectProduto>
    {
        List<ProspectProduto> getProdutoProspect(int cdProspect);
    }
}

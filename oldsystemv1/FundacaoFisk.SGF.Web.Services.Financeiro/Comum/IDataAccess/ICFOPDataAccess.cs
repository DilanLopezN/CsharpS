using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using System.Data.Entity;
using Componentes.Utils;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess
{
    using FundacaoFisk.SGF.GenericModel;
    using Componentes.GenericDataAccess.Comum;
    using FundacaoFisk.SGF.Web.Services.Financeiro.Model;

    public interface ICFOPDataAccess : IGenericRepository<CFOP>
    {
        CFOP getCFOPByTpNF(int cd_tipo_nota);
        IEnumerable<CFOP> searchCFOP(SearchParameters parametros, string descricao,bool inicio, int nm_CFOP, byte id_natureza_CFOP);
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using Componentes.GenericDataAccess.Comum;
using System.Data.Entity;
using Componentes.Utils;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess
{
    using FundacaoFisk.SGF.GenericModel;
    using FundacaoFisk.SGF.Web.Services.Financeiro.Model;
    public interface ITipoDescontoEscolaDataAccess : IGenericRepository<TipoDescontoEscola>
    {
        IEnumerable<int> getTipoDescontoWithEscola(int cdTpDesc);
        TipoDescontoEscola findTpDescEscolabyId(int cdTpDesc, int cdPessoa);
        IEnumerable<TipoDescontoEscola> getTpDescEscolaByTpDesc(int cdTpDesc);
        bool existeContatoDesconto(int cdTpDesc, int cdEscola);
    }
}

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

    public interface IDadosNFDataAccess : IGenericRepository<DadosNF>
    {
        IEnumerable<DadosNF> getDadosNFSearch(SearchParameters parametros, int cdCidade, string natOp, double? aliquota, byte id_regime);
        DadosNF getDadosNFById(int cdAliquota);
        bool getDadosCidade(int cdCidade);
        double? getISSEscola(int cdEscola);


    }
}

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

    public interface IAliquotaUFDataAccess : IGenericRepository<AliquotaUF>
    {
        IEnumerable<AliquotaUF> getAliquotaUFSearch(SearchParameters parametros, int cdEstadoOri, int cdEstadoDest, double? aliquota);
        AliquotaUF getAliquotaUFByOriDes(int cdEstadoOri, int cdEstadoDest);
        IEnumerable<EstadoSGF> getEstadoOri();
        IEnumerable<EstadoSGF> getEstadoDest();
        AliquotaUF getAliquotaUFById(int cdAliquota);
        AliquotaUF getAliquotaUFByEscDes(int cdEscola, int cdEstadoDest);
        AliquotaUF getAliquotaUFPorEstadoPessoa(int cdEscola, int cd_pessoa_cliente);
    }
}

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
    public interface ITipoDescontoDataAccess : IGenericRepository<TipoDesconto>
    {
        IEnumerable<TipoDescontoUI> GetTipoDescontoSearch(SearchParameters parametros, string descricao, Boolean inicio, Boolean? ativo, bool? incideBaixa, bool? pparc, decimal? percentual, int cdEscola);
        Boolean deleteAllTipoDesconto(List<TipoDesconto> tiposDesconto);
        TipoDesconto getTipoDescontoByContrato(int cd_desconto_contrato);
        TipoDesconto getTipoDescontoByIdComTipoDescontoEscola(int cdTipoDesconto);
        TipoDesconto getTipoDescontoComTipoDescontoEscola(int cd_escola, int cd_tipo_desconto);
        TipoDescontoUI getTipoDescontoUIById(int cdTipoDesconto, int cdEscola, bool masterGeral);
        bool getTipoDescontoNome(string dcTpDesc);
        bool getTipoDescontoNomeEsc(string dcTpDesc, int cdEscola);
        bool getTipoDescontoMaster(int cdTpDesc);
    }
}

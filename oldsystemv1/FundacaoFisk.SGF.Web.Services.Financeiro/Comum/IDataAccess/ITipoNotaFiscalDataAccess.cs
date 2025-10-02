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

    public interface ITipoNotaFiscalDataAccess : IGenericRepository<TipoNotaFiscal>
    {
        IEnumerable<TipoNotaFiscal> getTipoNotaFiscalSearch(SearchParameters parametros, string desc, string natOp, bool inicio, bool? status, int movimento, bool? devolucao, int cdEscola, byte id_regime_trib, bool? id_servico);
        bool getTpNFUtilizado(int cdNota);
        bool verificarTipoNotaFiscalPermiteMovimentoFinanceiro(int cd_tipo_nota_fiscal);
        bool verificarTipoNotaFiscalPermiteMovimentoEstoque(int cd_movimento);
        bool getMovimentaEstoque(int cd_tipo_nota_fiscal);
        byte getTipoMvtoTpNF(int cd_tipo_nota_fiscal);
    }
}

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
using FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess;
    public interface ITipoFinanceiroDataAccess : IGenericRepository<TipoFinanceiro>
    {       
        IEnumerable<TipoFinanceiro> GetTipoFinanceiroSearch(SearchParameters parametros, String descricao, Boolean inicio, Boolean? status);
        Boolean deleteAllTipoFinanceiro(List<TipoFinanceiro> tiposFinanceiros);
        List<TipoFinanceiro> getTipoFinanceiroAtivo();
        IEnumerable<TipoFinanceiro> getTipoFinanceiroMovimento(int cd_tipo_finan, int cd_empresa, int id_tipo_movto);
        IEnumerable<TipoFinanceiro> getTipoFinanceiro(int cd_tipo_finan, TipoFinanceiroDataAccess.TipoConsultaTipoFinanEnum tipoConsulta);
    }
}

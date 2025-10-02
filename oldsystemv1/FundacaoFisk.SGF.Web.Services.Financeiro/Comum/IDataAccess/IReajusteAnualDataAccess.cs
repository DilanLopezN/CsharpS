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
    public interface IReajusteAnualDataAccess : IGenericRepository<ReajusteAnual>
    {
        IEnumerable<ReajusteAnual> getReajusteAnualSearch(SearchParameters parametros, int cd_empresa, int cd_usuario, int status, DateTime? dtaInicial, DateTime? dtaFinal, bool cadastro, bool vctoInicial, int cd_reajuste_anual);
        Boolean deleteAllReajustes(List<ReajusteAnual> reajustes);
        ReajusteAnual getReajusteAnualFull(int cd_reajuste_anual, int cd_empresa);
        ReajusteAnual getReajusteAnualForEdit(int cd_empresa, int cd_reajuste_anual);
        bool verificaTitulosFechamentoReajusteAnual(int cd_empresa, int cd_reajuste_anual);
        List<int> getCodigoContratoTitulosReajusteAnual(int cd_empresa, int cd_reajuste_anual);
        ReajusteAnual getReajusteAnualGridView(int cd_reajuste_anual, int cd_empresa);
    }
}

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

    public interface IPlanoTituloDataAccess : IGenericRepository<PlanoTitulo>
    {
        PlanoTitulo getPlanoTituloByTitulo(int cdTitulo, int cdPlanoConta, int cdEscola);
        IEnumerable<PlanoTitulo> getPlanoTituloByTitulo(int cdTitulo, int cd_escola);
        IEnumerable<PlanoTitulo> getPlanoTituloByTitulos(int[] cdTitulos, int cd_empresa);
        PlanoTitulo getPlanoTituloByCdTitulo(int cd_titulo, int cd_pessoa_empresa);
        IEnumerable<RptPlanoTitulo> getPlanosContaPosicaoFinanceira(int cd_titulo);
    }
}

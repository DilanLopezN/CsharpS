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

    public interface IPoliticaComercialDataAccess : IGenericRepository<PoliticaComercial>
    {
        PoliticaComercial getPoliticaComercialById(int cdPoliticaComercial, int cdEscola);
        IEnumerable<PoliticaComercial> getPoliticaComercialSearch(SearchParameters parametros, string descricao, bool inicio, bool? ativo, bool parcIguais, bool vencFixo, int cdEscola);
        IEnumerable<PoliticaComercial> getPoliticaComercialByEmpresa(int cd_pessoa_escola, string dc_politica, bool inicio);
        PoliticaComercial getPoliticaComercialSugeridaNF(int cdEscola);
    }
}

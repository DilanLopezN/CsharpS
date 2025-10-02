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

    public interface IPoliticaTurmaDataAccess : IGenericRepository<PoliticaTurma>
    {
        IEnumerable<PoliticaTurma> getTurmaPolitica(int cdPolitica, int cdEscola);
        IEnumerable<PoliticaTurma> getTurmaPoliticaFull(int cdPolitica, int cdEscola);
        IEnumerable<Feriado> getFeriadosEscola(int cd_escola, bool feriado_financeiro);
        Contrato getContratoBaixa(int cd_escola, int cd_contrato);
        Parametro getParametrosBaixa(int cd_escola);

        Parametro getParametrosEscola(int cd_escola);
    }
}

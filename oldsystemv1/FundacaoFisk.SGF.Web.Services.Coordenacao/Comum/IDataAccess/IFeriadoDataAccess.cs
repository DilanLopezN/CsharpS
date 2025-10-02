using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Componentes.GenericDataAccess.Comum;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess
{
    public interface IFeriadoDataAccess: IGenericRepository<Feriado>
    {
        IEnumerable<Feriado> getFeriadoDesc(SearchParameters parametros, String desc, Boolean inicio, Boolean? ativo, int cdEscola, int Ano, int Mes, int Dia, int AnoFim, int MesFim, int DiaFim, bool? somenteAno, bool idFeriadoAtivo);
        Boolean deleteAllFeriado(List<Feriado> feriados);
        IEnumerable<Feriado> getFeriadosEscola(int cd_escola, bool feriado_financeiro);
        List<Feriado> getFeriadosPorPeriodo(Feriado feriado, int? cd_escola);
        IEnumerable<Feriado> getAllFeriados(List<int> feriados);
        bool spRefazerProgramacoesFeriado(int cd_feriado, Feriado.TipoOperacaoSPFeriado operacao);
    }
}

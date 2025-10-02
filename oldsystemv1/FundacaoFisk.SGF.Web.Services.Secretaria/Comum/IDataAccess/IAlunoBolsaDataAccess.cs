using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess.Comum;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Financeiro.Model;
using FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess;
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Comum.IDataAccess
{
    public interface IAlunoBolsaDataAccess : IGenericRepository<AlunoBolsa>
    {
        IEnumerable<RptBolsistas> getBolsistas(int cdEscola, int cd_aluno, int cd_turma, bool cancelamento, decimal? per_bolsa, int cd_motivo_bolsa, DateTime? dtIniComunicado,
                                                        DateTime? dtFimComunicado, DateTime? dtIni, DateTime? dtFim, bool periodo_ini, bool periodo_cancel);
    }
}

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Componentes.GenericDataAccess.Comum;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess
{
    public interface IAulaPersonalizadaDataAccess : IGenericRepository<AulaPersonalizada>
    {
        IEnumerable<AulaPersonalizadaUI> searchAulaPersonalizada(SearchParameters parametros, DateTime? dataIni, DateTime? dataFim, TimeSpan? hrInicial, TimeSpan? hrFinal, int? cdProduto, int? cdProfessor,
                                                                      int? cdSala, int? cdAluno, bool participou, int cdEscola);
        IEnumerable<AulaPersonalizada> searchAulaPersonalizadaPesq(int cdAulaPersonalizada, int cdEscola);
        AulaPersonalizada searchAulaPersonalizadaById(int cdAulaPersonalizada, int cdEscola);
        IEnumerable<AulaPersonalizadaReport> getReportAulaPersonalizada(int cd_empresa, int cd_aluno, int? cd_produto, int? cd_curso, DateTime? dt_inicial_agend, DateTime? dt_final_agend,
        DateTime? dt_inicial_lanc, DateTime? dt_final_lanc, TimeSpan? hr_inicial_agend, TimeSpan? hr_final_agend, TimeSpan? hr_inicial_lanc, TimeSpan? hr_final_lanc);
    }
}

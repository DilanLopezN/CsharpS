using Componentes.GenericDataAccess.Comum;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;
using System;
using System.Collections.Generic;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess
{
    public interface ISalaDataAccess : IGenericRepository<Sala>
    {
        IEnumerable<SalaSearchUI> getSalaDesc(SearchParameters parametros, String desc, Boolean inicio, Boolean? ativo, int cdEscola, bool online);
        Boolean deleteAllSala(List<Sala> salas);
        IEnumerable<Sala> findListSalasDiponiveis(TimeSpan horaIni, TimeSpan horaFim, DateTime data, bool? status, int? cdSala, int cdEscola, int? cd_atividade_extra);
        IEnumerable<Sala> findListSalas(bool? status, int? cdSala, int cdEscola);
        IEnumerable<Sala> findListSalasTurmas(int cdEscola);
        IEnumerable<Sala> findSalasTurmas(int cdEscola, bool online);
        IEnumerable<Sala> getSalasDisponiveisPorHorarios(List<Horario> horarios, int cd_turma, int cd_escola, int? cd_sala, DateTime dtInicio, DateTime? dtFinal);
        IEnumerable<Sala> getSalasDisponiveisPorHorariosByModalidadeOnline(List<Horario> horarios, int cd_turma, int cd_escola, int? cd_sala, DateTime dtInicio, DateTime? dtFinal);
        IEnumerable<Sala> getSalas(int cd_sala, int cd_escola, SalaDataAccess.TipoConsultaDuracaoEnum tipoConsulta);
        List<ReportControleSala> getHorariosRptControleSala(TimeSpan? hIni, TimeSpan? hFim, int cd_turma, int cd_professor, int cd_sala, List<int> diasSemana, int cd_escola);
        IEnumerable<Sala> findListSalasAulaPer(int cdEscola);
        IEnumerable<Sala> getSalasAulaReposicao(int cd_escola, SalaDataAccess.TipoConsultaDuracaoEnum tipoConsulta);
        IEnumerable<Sala> findListSalasDiponiveisAulaRep(TimeSpan horaIni, TimeSpan horaFim, DateTime data, bool? status, int? cdSala, int cdEscola, int? cd_aula_reposicao, int? cd_turma);
        bool verificaSalaOnline(int? cdSala, int cdEscola);
    }
}

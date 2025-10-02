using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess.Comum;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;
using FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess
{
    public interface IEventoDataAccess : IGenericRepository<Evento>
    {
        IEnumerable<Evento> getEventoDesc(SearchParameters parametros, String desc, Boolean inicio, Boolean? ativo);
        Boolean deleteAllEvento(List<Evento> eventos);
        IEnumerable<Evento> getEventos(int cd_evento, EventoDataAccess.TipoConsultaEventoEnum tipoConsulta);
        IEnumerable<AlunoEventoReport> getRelatorioEventos(int cd_escola, int? cd_turma, int? cd_professor, int? cd_evento, int? qtd_faltas, DateTime? dt_inicial, DateTime? dt_final);
        IEnumerable<AlunoEvento> getEventosRtpDiarioAula(int cd_escola, int cd_aluno, int cd_professor, DateTime dataAula);
        IEnumerable<Aluno> getAlunoIsTurmaInDate(int cd_turma, int cd_aluno, int cd_pessoa_escola, DateTime dtAula);
    }
}

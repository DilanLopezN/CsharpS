using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess.Comum;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess
{
    public interface IAlunoEventoDataAccess : IGenericRepository<AlunoEvento>
    {
        IEnumerable<AlunoEvento> getAlunoEvento(int cd_evento, int cd_aluno, int cd_pessoa_escola, int cd_diario_aula, AlunoEventoDataAccess.TipoConsultaAlunoEventoEnum tipoConsulta);
        IEnumerable<AlunoEvento> getEventosAvaliacaoAluno(int cd_aluno, int cd_turma, int cd_escola);
        IEnumerable<AlunoEvento> getEventosAlunoByDataDesistencia(int cd_aluno, int cd_turma, int cd_escola, DateTime dta_Desistencia);
        bool deleteAllAlunoEvento(List<AlunoEvento> alunoEventos);
        IEnumerable<AlunoEvento> getAllAlunosEvento(int cd_diario_aula, int cd_empresa);
    }
}

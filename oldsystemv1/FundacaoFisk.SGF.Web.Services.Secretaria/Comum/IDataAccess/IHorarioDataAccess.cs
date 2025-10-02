using System;
using System.Collections.Generic;
using System.Linq;
using Componentes.GenericDataAccess.Comum;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Comum.IDataAccess
{
    public interface IHorarioDataAccess : IGenericRepository<Horario>
    {
        IEnumerable<Horario> getHorarioByEscolaForRegistro(int cdEscola, int cdRegistro,Horario.Origem origem);
        IEnumerable<Horario> getHorarioOcupadosForTurma(int cdEscola, int cdRegistro, int[] cdProfessores, int cd_turma,
            int cd_duracao, int cd_curso, DateTime dt_inicio, DateTime? dt_final, HorarioDataAccess.TipoConsultaHorario tipoCons);
        IEnumerable<Horario> getHorarioOcupadosForSala(Turma turma, int cdEscola, HorarioDataAccess.TipoConsultaHorario tipoCons);
        bool getHorarioByHorario(int cdEscola, int cdRegistro, Horario.Origem origem, TimeSpan hr_servidor, int diaSemanaAtual);
        String retornaDescricaoHorarioOcupado(int cd_empresa, TimeSpan hr_ini, TimeSpan hr_fim);
    }
}

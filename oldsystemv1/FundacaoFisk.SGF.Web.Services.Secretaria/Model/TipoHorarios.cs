using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Model
{
    public class TipoHorarios
    {
        public List<Horario> horarioOcupTurma { get; set; }
        public List<Horario> horarioOcupTurmaPPT { get; set; }
        public List<Horario> horairosOcupSala { get; set; }
        public List<Horario> horariosOcupProf { get; set; }
        public List<Horario> horariosDispoProf { get; set; }
    }
}

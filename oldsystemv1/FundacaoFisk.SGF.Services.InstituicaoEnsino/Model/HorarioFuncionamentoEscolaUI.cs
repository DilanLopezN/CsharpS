using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.Services.InstituicaoEnsino.Model
{
    public class HorarioFuncionamentoEscolaUI
    {
        public Nullable<System.TimeSpan> hr_inicial { get; set; }
        public Nullable<System.TimeSpan> hr_final { get; set; }

        public int horaIncial
        {
            get
            {
                //DateTime horaI = DateTime.Now.Add(hr_inicial.Value);
                string dataInformada = "07/01/2012 " + hr_inicial;
                DateTime horaI = DateTime.Parse(dataInformada);
                return horaI.Hour;
            }
        }

        public int horaFinal
        {
            get
            {
                //DateTime horaF = DateTime.Now.Add(hr_final.Value);
                string dataInformada = "07/01/2012 " + hr_final;
                DateTime horaF = DateTime.Parse(dataInformada);
                return horaF.Hour;
            }
        }
    }
}

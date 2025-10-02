using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Model
{
    public class ReportControleSala : TO
    {

        public int cd_sala { get; set; }
        public string no_sala { get; set; }
        public System.TimeSpan hora_ini { get; set; }
        public System.TimeSpan hora_fim { get; set; }
        public int nm_hora { get; set; }
        public int cd_professor { get; set; }
        //public string no_professor { get; set; }
        public string no_tipo_cor { get; set; }
        public string no_sigla_estagio { get; set; }
        //public DateTime dt_programacao_professor { get; set; }
        public List<ProfessorTurma> listaProfessores { get; set; }
        public int id_dia_semana { get; set; }
        public string no_turma { get; set; }
        public int nm_aulas { get; set; }
        public TimeSpan horaMin { get; set; }

        public string no_professor
        {
            get
            {
                string retorno = "";

                if (listaProfessores != null && listaProfessores.Count == 0)                    
                        retorno = "Sem Professor";

                if (listaProfessores != null && listaProfessores.Count() > 0)
                    foreach (ProfessorTurma pt in listaProfessores)
                    {
                        retorno += string.IsNullOrEmpty(retorno) ? pt.no_professor : ", " + pt.no_professor;
                        retorno = "(" + this.nm_aulas + ")" + retorno + (this.no_sigla_estagio == "" ? "" : " - ") + this.no_sigla_estagio;
                    }

                return retorno;
            }
            
        }

       
      
        public string hora
        {
            get
            {
                return string.Format("{0:c}", horaMin).Substring(0, 5);
                //if (this.nm_hora < 10)
                //    return string.Format("0{0}:00", nm_hora);
                //return string.Format("{0}:00", nm_hora);
            }
        }

        public string no_dia_semana
        {
            get
            {
                if (id_dia_semana > 0)
                    return FundacaoFisk.SGF.GenericModel.Horario.retornarDiaSemana((byte)id_dia_semana);
                return "";
            }
        }

    }
}

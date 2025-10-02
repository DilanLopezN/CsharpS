using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Model
{
    public class ReportPercentualTerminoEstagio : TO
    {
        public int cd_professor { get; set; }
        public int cd_turma { get; set; }
        public int cd_aluno { get; set; }
        //public DateTime dt_programacao_professor { get; set; }
        public List<ProfessorTurma> listaProfessores { get; set; }
        public string no_professor { get; set; }
        //public string no_professor
        //{
        //    get
        //    {
        //        string retorno = "";
        //        if (listaProfessores != null && listaProfessores.Count() > 0)
        //            foreach (ProfessorTurma pt in listaProfessores)
        //                retorno += string.IsNullOrEmpty(retorno) ? pt.no_professor : ", " + pt.no_professor;
        //        return retorno;
        //    }
        //}

        public string no_turma { get; set; }
        public string no_estagio { get; set; }
        public string dias_horario
        {
            get
            {
                string diasHorario = "";
                double qtd_minutos_turma = 0;
                if(horarios != null){
                    Hashtable horariosFormat = Horario.montaDiaHorario(horarios, ref qtd_minutos_turma);
                    diasHorario = Horario.getDescricaoCompletaHorarios(horariosFormat);
                }
                return diasHorario;
            }
        }
        public DateTime dt_inicio_estagio { get; set; }
        public DateTime dt_termino_estagio { get; set; }
        public int qtd_aluno_1_mes { get; set; }
        public int qtd_alunos_ini_term_estagio { get; set; }
        public double pc_estagio
        {
            get
            {
                decimal retorno = 0;
                if (qtd_alunos_ini_term_estagio > 0 && qtd_aluno_1_mes > 0)
                {
                    retorno = ((decimal)qtd_alunos_ini_term_estagio / qtd_aluno_1_mes) * 100;
                }
                return (double)retorno;
            }
        }
        public int qtd_alunos_terminaram_estagio { get; set; }
        public List<Horario> horarios { get; set; }
        // SubReport Percentual Terminal Estágio
        public string no_aluno { get; set; }
        public bool id_material { get; set; }

        //Para Uso Da Direção
        public decimal valor_alunos_ini_term_estagio
        {
            get {
                decimal valor = 0;
                if (this.pc_estagio >= this.pc_termino_estagio)
                    valor = vl_termino_estagio;
                return valor;
            }
        }
        public int qtd_alunos_matricularam_proximo_estagio { get; set; }
        public double pc_termino_estagio { get; set; }
        public decimal vl_termino_estagio { get; set; }
        public decimal vl_rematricula { get; set; }

        public string dta_inicio_estagio
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", dt_inicio_estagio);
            }
        }

        public string dta_termino_estagio
        {
            get
            {
                return String.Format("{0:dd/MM/yyyy}", dt_termino_estagio);
            }
        }
    }
}

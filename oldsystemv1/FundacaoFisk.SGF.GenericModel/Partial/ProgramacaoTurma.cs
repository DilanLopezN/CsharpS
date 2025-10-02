using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class ProgramacaoTurma
    {
        public enum TipoConsultaProgramacaoEnum
        {
            PESQUISAR_PELO_CODIGO_HORARIO = 0,
            PESQUISAR_POR_ARRAY_CODIGO_HORARIO = 1,
            HAS_GERADAS = 2,
            HAS_NAO_GERADAS = 3,
            HAS_TODAS = 4
        }

        public enum TipoProgramacaoManual
        {
            PROGRAMACAO_GERADA_CURSO = 0,
            PROGRAMACAO_MANUAL = 1,
            PROGRAMACAO_GERADA_MODELO = 2
        }

        public bool programacaoTurmaEdit { get; set; }
        public bool aula_efetivada { get; set; }
        public bool id_feriado_desconsiderado { get; set; } //Propriedade para sinalizar que a programação contém desconsidera feriado em memória, antes do usuário solicitar o botão salvar.
        public int? cd_sala_prog { get; set; }
        public bool? aula_externa { get; set; }
        public int? cd_professor { get; set; }
        public string no_turma { get; set; }
        public string tx_obs_aula { get; set; }

        public string dt_programacao_turma {
            get {
                if(this.dta_programacao_turma != null)
                    return String.Format("{0:dd/MM/yyyy}", dta_programacao_turma);
                else
                    return String.Empty;
            }
        }

        public string hr_aula
        {
            get
            {
                if(this.cd_feriado.HasValue && !this.cd_feriado_desconsiderado.HasValue && !this.id_feriado_desconsiderado)
                    return "...";
                if (this.hr_inicial_programacao != null && this.hr_final_programacao != null)
                    return String.Format(this.hr_inicial_programacao + " " + this.hr_final_programacao);
                else
                    return String.Empty;
            }
        }

        public string dia_mes {
            get {
                string retorno = "";
                
                if(dta_programacao_turma != null)
                {
                    int dia = dta_programacao_turma.Day;
                    int mes = dta_programacao_turma.Month;
                    string strMes = dta_programacao_turma.Month+"";

                    if (mes < 10)
                        strMes = "0" + strMes;

                    if(dta_programacao_turma.Day < 10)
                        retorno = "0" + dia + "/" + strMes;
                    else
                        retorno = "" + dia + "/" + strMes;
                }
                return retorno;
            }
        }

        public string dia_mes_hor_min
        {
            get
            {
                return dia_mes + "/" + hr_aula;
            }
        }

        public static TipoConsultaProgramacaoEnum parseTipoConsultaProg(int tipoProg){
            TipoConsultaProgramacaoEnum tipoProgRetorno = new TipoConsultaProgramacaoEnum();
            if (tipoProg == 0)
                tipoProgRetorno = TipoConsultaProgramacaoEnum.HAS_TODAS;
            if (tipoProg == 1)
                tipoProgRetorno = TipoConsultaProgramacaoEnum.HAS_GERADAS;
            if (tipoProg == 2)
                tipoProgRetorno = TipoConsultaProgramacaoEnum.HAS_NAO_GERADAS;
            return tipoProgRetorno;
        }

        public string no_programacao
        {
            get
            {
                string retorno = "";
                if (dta_programacao_turma != null)
                {
                    retorno = "Aula: " + nm_aula_programacao_turma + " Data: " + String.Format("{0:dd/MM/yyyy}", dta_programacao_turma);
                    if (hr_inicial_programacao != null && hr_final_programacao != null)
                        retorno = retorno + " " + String.Format(this.hr_inicial_programacao + " - " + this.hr_final_programacao);
                }
                return retorno;
            }
        }

        //Para o relatório de diário de aula:
        public string dia_semana { get; set; }
        public bool is_turma_regular { get; set; }
    }
}
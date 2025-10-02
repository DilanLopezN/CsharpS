using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.GenericModel
{
    public partial class Horario
    {
        public enum Origem
        {
            TURMA = 19,
            ALUNO = 10,
            PROFESSOR = 20,
            USUARIO = 4
        }
        public String no_registro;
        public string desc_concat_dias_semena { get; set; }
        public DateTime startTime {get;set;}
        public string summary { get; set; }
        public int? id_situacao {get;set;}
        //{
        //    get
        //    {
        //        string dataInformada = "07/0"+id_dia_semana+"/2012 " + dt_hora_ini;
        //        DateTime horaI = DateTime.Parse(dataInformada);
        //        String.Format("{0:dd/MM/yyyy HH:mm:ss}", horaI);
        //        return horaI;
        //    }
        //}

        public DateTime endTime { get; set; }
        //{
        //    get
        //    {
        //        string dataInformada = "07/0"+ id_dia_semana + "/2012 " + dt_hora_fim;
        //        DateTime horaFim = DateTime.Parse(dataInformada);
        //        String.Format("{0:dd/MM/yyyy HH:mm:ss}", horaFim);
        //        return horaFim;
        //    }
        //}

        public int id
        {
            get
            {
                return cd_horario;
            }
        }

        public string no_datas
        {
            get{
               return   dt_hora_ini + " / " + dt_hora_fim + " - " + dia_semana;
            }
        }

        public string dia_semana {
            get { return retornarDiaSemana(id_dia_semana);}
        }

        public static byte getDiaPorDiaSemana(string diaSemana)
        {
            switch (diaSemana)
            {
                case "domingo":
                    return 1;
                case "segunda-feira":
                    return 2;
                case "terça-feira":
                    return 3;
                case "quarta-feira":
                    return 4;
                case "quinta-feira":
                    return 5;
                case "sexta-feira":
                    return 6;
                case "sábado":
                    return 7;
                default:
                    return 0;
            }
        }

        public static string getDiaSemanaPorDia(string diaSemana)
        {
            switch (diaSemana)
            {
                case "1":
                    return "domingo";
                case "2":
                    return "segunda-feira";
                case "3":
                    return "terça-feira";
                case "4":
                    return "quarta-feira";
                case "5":
                    return "quinta-feira";
                case "6":
                    return "sexta-feira";
                default:
                    return "sábado";
            }
        }

        public string calendar { get; set; }

        public static string getDescricaoCompletaHorarios(Hashtable hashtableHorarios) {
            List<Horario> listaHorariosOrdenados = new List<Horario>();
            string dias_horarios = "";
            foreach(DictionaryEntry entry in hashtableHorarios) 
                listaHorariosOrdenados.Add(new Horario { desc_concat_dias_semena = entry.Value.ToString(), no_registro = entry.Key.ToString() });

            listaHorariosOrdenados = listaHorariosOrdenados.OrderBy(h => h.desc_concat_dias_semena).ThenBy(h => h.no_registro).ToList();
            foreach (Horario entry in listaHorariosOrdenados)
            {
                string[] array = entry.desc_concat_dias_semena.Split(',');
                for (int i = 0; i < array.Length; i++)
                    array[i] = getDiaSemanaPorDia(array[i]);
                entry.desc_concat_dias_semena = string.Join(", ", array);
            }

            foreach(Horario entry in listaHorariosOrdenados)
                dias_horarios += "; " + entry.desc_concat_dias_semena + " das " + entry.no_registro;

            if(dias_horarios.Length >= 1)
                dias_horarios = dias_horarios.Substring(2, dias_horarios.Length - 2);
            return dias_horarios;
        }

        public static string getDescricaoSimplificadaHorarios(Hashtable hashtableHorarios){
            List<Horario> listaHorariosOrdenados = new List<Horario>();
            string dias_horarios = "";

            foreach(DictionaryEntry entry in hashtableHorarios)
                listaHorariosOrdenados.Add(new Horario { desc_concat_dias_semena = entry.Value.ToString(), no_registro = entry.Key.ToString() });
            
            listaHorariosOrdenados = listaHorariosOrdenados.OrderBy(h => h.desc_concat_dias_semena).ThenBy(h => h.no_registro).ToList(); 
            foreach (Horario entry in listaHorariosOrdenados)
                dias_horarios += "; " + entry.no_registro;

            if (dias_horarios.Length >= 1)
                dias_horarios = dias_horarios.Substring(2, dias_horarios.Length - 2);
            return dias_horarios;
        }

        /*
            Monta os horários em lista de string, exemplo:
            seg-ter-qui 08:00 às 10:00
            qua-sex 11:00 às 12:00
        */
        public static Hashtable montaDiaHorario(List<Horario> horariosTurma, ref double qtd_minutos_turma)
        {
            Hashtable hashtableHorarios = new Hashtable();
            Hashtable hashtableDias = new Hashtable();
            SortedDictionary<string, string> dictDias = new SortedDictionary<string, string>();

            //Ajusta os dias da semana para posterior ordenação:
            //for (int i = 0; i < horariosTurma.Count; i++)
            //    horariosTurma[i].dia_semana = Horario.getDiaPorDiaSemana(horariosTurma[i].id_dia_semana);

            horariosTurma = horariosTurma.OrderBy(ht => ht.id_dia_semana).ThenBy(ht => ht.dt_hora_ini).ToList();

            for (int i = 0; i < horariosTurma.Count; i++)
            {
                string horario = horariosTurma[i].dt_hora_ini.ToString(@"hh\:mm") + " às " + horariosTurma[i].dt_hora_fim.ToString(@"hh\:mm");

                if (!hashtableDias.Contains(horariosTurma[i].id_dia_semana))
                    hashtableDias[horariosTurma[i].id_dia_semana] = horario;
                else
                    hashtableDias[horariosTurma[i].id_dia_semana] += ", " + horario;
                qtd_minutos_turma += horariosTurma[i].dt_hora_fim.TotalMinutes - horariosTurma[i].dt_hora_ini.TotalMinutes;
            }

            foreach (DictionaryEntry entry in hashtableDias)
                dictDias.Add(entry.Key + "", entry.Value + "");

            foreach (KeyValuePair<string, string> entry in dictDias)
            {
                if (!hashtableHorarios.Contains(entry.Value))
                    hashtableHorarios[entry.Value] = entry.Key;
                else
                    hashtableHorarios[entry.Value] += "," + entry.Key;
            }

            return hashtableHorarios;
        }

        public static string formatDias(Hashtable hashtableHorarios) 
        {
            var dias_horarios = "";
            if (hashtableHorarios != null)
            {
                List<Horario> listaHorariosOrdenados = new List<Horario>();
                foreach (DictionaryEntry entry in hashtableHorarios)
                    listaHorariosOrdenados.Add(new Horario { desc_concat_dias_semena = entry.Value.ToString() });

                listaHorariosOrdenados = listaHorariosOrdenados.OrderBy(h => h.desc_concat_dias_semena).ThenBy(h => h.no_registro).ToList();
                foreach (Horario entry in listaHorariosOrdenados)
                {
                    string[] array = entry.desc_concat_dias_semena.Split(',');
                    for (int i = 0; i < array.Length; i++)
                        array[i] = Horario.getDiaSemanaPorDia(array[i]);
                    entry.desc_concat_dias_semena = string.Join(", ", array);
                }

                List<string> listaStrHorariosOrdenados = (from h in listaHorariosOrdenados orderby h.id_dia_semana ascending select h.desc_concat_dias_semena).Distinct().ToList();
                foreach (string s in listaStrHorariosOrdenados)
                    dias_horarios += "; " + s;

                if (dias_horarios.Length >= 2)
                    dias_horarios = dias_horarios.Substring(2, dias_horarios.Length - 2);

                return dias_horarios.Replace(", ", "/");
            }
            return "";
        }

        public static Horario changeValueHorario(Horario horarioContext,Horario horarioView){
            horarioContext.cd_registro = horarioView.cd_registro;
            horarioContext.dt_hora_fim = horarioView.dt_hora_fim;
            horarioContext.dt_hora_ini = horarioView.dt_hora_ini;
            horarioContext.id_dia_semana = horarioView.id_dia_semana;
            horarioContext.id_disponivel = horarioView.id_disponivel;
            
            return horarioContext;
        }

        public void loadHorario() {
            if (cd_horario <= 0)
            {
                this.dt_hora_ini = new TimeSpan(this.startTime.ToLocalTime().Hour, this.startTime.Minute, this.startTime.Second);
                this.dt_hora_fim = new TimeSpan(this.endTime.ToLocalTime().Hour, this.endTime.Minute, this.endTime.Second);
            }
        }

        public static List<Horario> SortList(List<Horario> horarios) {
            if(horarios != null)
                horarios.Sort(delegate(Horario a, Horario b) {
                    if(a != null && b != null) {
                        if(a.id_dia_semana > b.id_dia_semana)
                            return 1;
                        else if(a.id_dia_semana < b.id_dia_semana)
                            return -1;
                        else if(a.dt_hora_ini > b.dt_hora_ini)
                            return 1;
                        else return -1;
                    }
                    else return -1;
                });
            return horarios;
        }

        public static List<Horario> clonarHorariosZerandoMemoria(List<Horario> listHorarios,String calendar)
        {
            List<Horario> horariosLimpos = new List<Horario>();
            foreach (Horario h in listHorarios)
            {
                Horario newHorario = new Horario();
                newHorario.copy(h);
                newHorario.calendar = calendar;
                newHorario.cd_horario = 0;
                newHorario.startTime = new DateTime(2012, 7, h.id_dia_semana, h.dt_hora_ini.Hours, h.dt_hora_ini.Minutes, h.dt_hora_ini.Seconds).ToUniversalTime();
                newHorario.endTime = new DateTime(2012, 7, h.id_dia_semana, h.dt_hora_fim.Hours, h.dt_hora_fim.Minutes, h.dt_hora_fim.Seconds).ToUniversalTime();
                newHorario.HorariosProfessores = new List<HorarioProfessorTurma>();
                if (h.HorariosProfessores != null)
                    foreach (HorarioProfessorTurma hp in h.HorariosProfessores)
                    {
                        HorarioProfessorTurma pt = new HorarioProfessorTurma();
                        pt.cd_professor = hp.cd_professor;
                        newHorario.HorariosProfessores.Add(pt);
                    }
                horariosLimpos.Add(newHorario);
            }
            return horariosLimpos;
        }

        public static string retornarDiaSemana(byte id_dia_semana)
        {
            switch (id_dia_semana)
            {
                case 1:
                    return "Domingo";
                case 2:
                    return "Segunda-Feira";
                case 3:
                    return "Terça-Feira";
                case 4:
                    return "Quarta-Feira";
                case 5:
                    return "Quinta-Feira";
                case 6:
                    return "Sexta-Feira";
                case 7:
                    return "Sábado";
                default:
                    return "";
            }
        }
    }
}

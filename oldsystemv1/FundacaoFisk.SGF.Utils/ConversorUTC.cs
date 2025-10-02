using System;

namespace FundacaoFisk.SGF.Utils
{
    public class ConversorUTC
    {
        public static DateTime ToUniversalTime(DateTime data, int id_fuso_horario, bool horario_verao)
        {
            if (!DateTime.MinValue.Equals(data))
            {
                if (!horario_verao && TimeZone.CurrentTimeZone.IsDaylightSavingTime(DateTime.Now))
                    horario_verao = true;
                data = data.ToUniversalTime();
                //if (!horario_verao && )
                //    data = data.AddHours(1);
                //data = data.AddHours(-3 + id_fuso_horario);
                data = data + new TimeSpan(-3 + id_fuso_horario, 0, 0);

                /*TimeZoneInfo timeZoneInfo = null;
                switch (id_fuso_horario)
                {
                    case 3:
                        timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
                        break;
                    case 4:
                        timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Pacific SA Standard Time");
                        break;
                }
                data = TimeZoneInfo.ConvertTimeToUtc(data, timeZoneInfo);*/
                
                // Caso o cliente possua horário de verão configurado e, sabido que o servidor de aplicações terá horário de verão, deve-se remover o horário de verão, uma vez que foi aplicado no id_fuso_horario.
                var myTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("GTB Standard Time");
                //TimeZoneInfo.CreateCustomTimeZone(TimeZoneInfo.Local.Id, TimeZoneInfo.Local.BaseUtcOffset, TimeZoneInfo.Local.DisplayName,
                //TimeZoneInfo.Local.StandardName, "", null, true);

                data = AjustDSTFromDateTime(data, myTimeZoneInfo, false, horario_verao,false);
            }
            return data;
        }

        public static DateTime ToLocalTime(DateTime data, int id_fuso_horario, bool horario_verao)
        {
            if (!DateTime.MinValue.Equals(data))
            {
                data = data.ToLocalTime();
                //data = data.AddHours(3 - id_fuso_horario);
                data = data + new TimeSpan(3 - id_fuso_horario, 0, 0);

                /*TimeZoneInfo timeZoneInfo = null;
                switch (id_fuso_horario)
                {
                    case 3:
                        timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
                        break;
                    case 4:
                        timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Pacific SA Standard Time");
                        break;
                }
                data = TimeZoneInfo.ConvertTimeFromUtc(data, timeZoneInfo);*/

                // Caso o cliente possua horário de verão configurado e, sabido que o servidor de aplicações terá horário de verão, deve-se remover o horário de verão, uma vez que foi aplicado no id_fuso_horario.
                var myTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("GTB Standard Time");
                //TimeZoneInfo.CreateCustomTimeZone(TimeZoneInfo.Local.Id, TimeZoneInfo.Local.BaseUtcOffset, TimeZoneInfo.Local.DisplayName,
                //TimeZoneInfo.Local.StandardName, "", null, true);

                data = AjustDSTFromDateTime(data, myTimeZoneInfo, true, horario_verao, true);
            }
            return data;
        }

        public static DateTime Date(DateTime data, int id_fuso_horario, bool horario_verao)
        {
            return ConversorUTC.ToLocalTime(data, id_fuso_horario, horario_verao).Date;
        }

        private static DateTime AjustDSTFromDateTime(DateTime dateTime, TimeZoneInfo timeZoneInfo, bool subtract, bool horario_verao, bool localTime)
        {
            if ((localTime && !dateTime.IsDaylightSavingTime()) || (!localTime && !horario_verao))
                return dateTime;
            //if (!horario_verao)
            //    return dateTime;

            var result = dateTime;

            foreach (var adjustmentRule in timeZoneInfo.GetAdjustmentRules())
                if(subtract)
                    result = result.Subtract(adjustmentRule.DaylightDelta);
                else
                    result = result.Add(adjustmentRule.DaylightDelta);
            return result;
        }
    }
}

using System;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace BoletoNet.Util
{
    public static class Extensions
    {
        public static string Modulo11(this string str, int @base)
        {
            var fats = Enumerable.Repeat(Enumerable.Range(2, @base - 1), 10).SelectMany(x => x).Take(str.Length);
            var mod = 11 - str.Reverse().Zip(fats, (x, a) => (char.GetNumericValue(x) * a)).Sum() % 11;
            return mod > 9 || mod <= 1 ? "1" : mod.ToString().Substring(0, 1);
        }

        public static T GetFirstAttribute<T>(this Type type)
        {
            return (T)type.GetCustomAttributes(typeof(T), false).FirstOrDefault();
        }
        public static T GetFirstAttribute<T>(this MemberInfo memberInfo)
        {
            return (T)memberInfo.GetCustomAttributes(typeof(T), false).FirstOrDefault();
        }

         /// <summary>
        /// Retorna o valor atual removendo a vírgula
        /// </summary>
        /// <param name="valor"></param>
        /// <returns></returns>
        public static string ApenasNumeros(this decimal valor)
        {
            return valor.ToString("0.00", CultureInfo.GetCultureInfo("pt-BR")).Replace(",", "");
        }

        /// <summary>
        /// Retorna o valor atual removendo a vírgula
        /// </summary>
        /// <param name="valor"></param>
        /// <returns></returns>
        public static string ApenasNumeros(this decimal? valor)
        {
            if (valor != null)
            {
                return valor.Value.ToString("0.00", CultureInfo.GetCultureInfo("pt-BR")).Replace(",", "");
            }

            return string.Empty;
        }
        
    }
}

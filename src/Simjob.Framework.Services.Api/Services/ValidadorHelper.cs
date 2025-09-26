using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Simjob.Framework.Services.Api.Services
{
    public static class ValidadorHelper
    {
        public static bool ValidarCamposCd(object obj, out List<string> camposInvalidos, string prefixo = "")
        {
            camposInvalidos = new List<string>();

            if (obj == null)
                return true;

            var tipo = obj.GetType();
            var propriedades = tipo.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in propriedades)
            {
                var nomeCompleto = string.IsNullOrEmpty(prefixo) ? prop.Name : $"{prefixo}.{prop.Name}";
                var valor = prop.GetValue(obj);

                // Ignora métodos e propriedades sem getter
                if (!prop.CanRead)
                    continue;

                // Verifica se a propriedade começa com "cd_"
                if (prop.Name.StartsWith("cd_", StringComparison.OrdinalIgnoreCase))
                {
                    var tipoProp = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

                    if (valor == null)
                    {
                        // Campo obrigatório (não-nullable) sem valor
                        if (!IsNullable(prop))
                            camposInvalidos.Add(nomeCompleto);
                    }
                    else
                    {
                        if (tipoProp == typeof(int) && (int)valor == 0)
                            camposInvalidos.Add(nomeCompleto);
                        else if (tipoProp == typeof(string) && string.IsNullOrWhiteSpace((string)valor))
                            camposInvalidos.Add(nomeCompleto);
                    }
                }

                // Se for classe customizada (exclui string), entra recursivamente
                else if (valor != null && !IsSimpleType(prop.PropertyType))
                {
                    // Se for lista, percorre os itens
                    if (typeof(IEnumerable).IsAssignableFrom(prop.PropertyType) && prop.PropertyType != typeof(string))
                    {
                        var enumerable = (IEnumerable)valor;
                        int i = 0;
                        foreach (var item in enumerable)
                        {
                            if (item != null && !IsSimpleType(item.GetType()))
                            {
                                if (!ValidarCamposCd(item, out var errosInternos, $"{nomeCompleto}[{i}]"))
                                    camposInvalidos.AddRange(errosInternos);
                            }
                            i++;
                        }
                    }
                    else
                    {
                        // Objeto aninhado
                        if (!ValidarCamposCd(valor, out var errosInternos, nomeCompleto))
                            camposInvalidos.AddRange(errosInternos);
                    }
                }
            }

            return !camposInvalidos.Any();
        }

        private static bool IsNullable(PropertyInfo prop)
        {
            return Nullable.GetUnderlyingType(prop.PropertyType) != null || !prop.PropertyType.IsValueType;
        }

        private static bool IsSimpleType(Type type)
        {
            return
                type.IsPrimitive ||
                type.IsEnum ||
                type == typeof(string) ||
                type == typeof(decimal) ||
                type == typeof(DateTime) ||
                type == typeof(Guid);
        }
    }
}


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Exiges.Negocios.Library
{
    public static class EnumHelper
    {
        /// <summary>
        /// Retorna Enumeradores com seus valore: Nome, Valor e Descrição
        /// </summary>
        /// <typeparam name="TEnum">Enumerado que será Analisado</typeparam>
        /// <typeparam name="TValue">Tipo do Valor associado no Enum <see cref="Char"/> ou <see cref="long"/></typeparam>
        /// <param name="pEnumValue"></param>
        /// <returns>
        /// Retorna Enumerador que está sendo Analisado <c>(Nome, Valor e Descrição)</c>
        /// </returns>
        //public static EnumModel<TValue> GetEnumModel<TEnum, TValue>(TEnum pEnumValue)
        //{
        //    var model = new EnumModel<TValue>
        //    {
        //        Nome = pEnumValue.ToString(),
        //        Valor = (TValue)Convert.ChangeType(pEnumValue.ValueToString(), typeof(TValue)),
        //        Descricao = pEnumValue.Description()
        //    };

        //    return model;
        //}

        /// <summary>
        /// Retorna Lista De Enumeradores com seus valore: Nome, Valor e Descrição
        /// </summary>
        /// <typeparam name="EnumType">Enumerado que será Analisado</typeparam>
        /// <typeparam name="TypeValueEnum">Tipo do Valor associado no Enum <see cref="Char"/> ou <see cref="long"/></typeparam>
        /// <returns>
        /// Retorna uma lista com os valores do Enumerador que está sendo Analisado <c>(Nome, Valor e Descrição)</c>
        /// </returns>
        //public static IEnumerable<EnumModel<TypeValueEnum>> ToList<EnumType, TypeValueEnum>()
        //{
        //    var typeEnums = typeof(EnumType);
        //    var fieldsArray = typeEnums.GetFields(BindingFlags.Public | BindingFlags.Static);
        //    var model = (from fInfo in fieldsArray
        //                 let valor = Convert.ChangeType(fInfo.GetValue(null), typeof(TypeValueEnum)).ToString()
        //                 let descricao = ((DescriptionAttribute)fInfo.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault())?.Description
        //                 let nome = fInfo.Name
        //                 orderby valor
        //                 select new EnumModel<TypeValueEnum>
        //                 {
        //                     Valor = (TypeValueEnum)Convert.ChangeType(valor, typeof(TypeValueEnum)),
        //                     Descricao = descricao,
        //                     Nome = nome
        //                 }).ToList();
        //    return model.ToList();
        //}

        #region Extension para Enumeradores

        /// <summary>
        /// Obtém o valor configurado no Enumerado no Padrão <c>Char</c>
        /// </summary>
        public static string ValueToString<EnumType>(this EnumType value)
        {
            var typeEnums = typeof(EnumType);
            var field = typeEnums.GetField(value.ToString());
            var valor = Convert.ChangeType(field.GetValue(null), typeof(char)).ToString();
            return valor;
        }

        /// <summary>
        /// Obtém o valor configurado no Enumerado no Padrão <c>Númerico</c>
        /// </summary>
        /// <typeparam name="EnumType">Enumerado que será obtido o seu valor</typeparam>
        /// <param name="value">Enumerado que será obtido o seu valor</param>
        /// <returns>
        /// Retorna o valor configuração no Enumerador
        /// </returns>
        public static short Value<EnumType>(this EnumType value) => ValueToNumber(value);

        public static List<string> ToListString(this IEnumerable<long?> value)
        {
            var list = new List<string>();
            if (value == null) return list;
            foreach (var item in value)
                list.Add(item.ToString());
            return list;
        }

        public static List<long?> ToListLong(this IEnumerable<string> value) {
            var list = new List<long?>();
            foreach (var item in value)
                list.Add(Convert.ToInt64(item));
            return list;
        }

        /// <summary>
        /// Obtém o valor configurado no Enumerado no Padrão <c>Númerico</c>
        /// </summary>
        public static short ValueToNumber<EnumType>(this EnumType value)
        {

            var typeEnums = typeof(EnumType);
            FieldInfo field;

            //verifica se o tipo é um Nullable
            if (typeEnums.IsGenericType && typeEnums.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var underlyingType = Nullable.GetUnderlyingType(typeEnums);
                field = underlyingType.GetField(value.ToString());
            } else
                field = typeEnums.GetField(value.ToString());
            var valor = Convert.ChangeType(field?.GetValue(null), typeof(short)).ToString();
            return Convert.ToInt16(valor);
        }

        /// <summary>
        /// Retorna o texto que descreve o Enumerador
        /// </summary>
        public static string Description<T>(this T pValue)
        {
            var field = pValue.GetType().GetField(pValue.ToString());
            var attribute = field.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault();
            return attribute != null ? ((DescriptionAttribute)attribute).Description : pValue.ToString();
        }

        #endregion
    }
}

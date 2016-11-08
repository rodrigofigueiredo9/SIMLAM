using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace Tecnomapas.Blocos.Etx.ModuloExtensao.Entities
{
	public static class DictionaryEtx
	{
		public static T GetValue<T>(this Dictionary<string, object> dictionary, string key)
		{
			T value;

			if (!dictionary.ContainsKey(key) || dictionary[key] == null)
			{
				return default(T);
			}

			value = (T)Convert.ChangeType(dictionary[key], typeof(T));

			return value;
		}

		public static List<T> GetListValue<T>(this Dictionary<string, object> dictionary, string key)
		{
			List<T> listValue = new List<T>();

			if (!dictionary.ContainsKey(key) || dictionary[key] == null || !dictionary[key].GetType().Name.Contains("ArrayList"))
			{
				return new List<T>();
			}

			ArrayList arrayList = dictionary[key] as ArrayList;			

			foreach (T item in arrayList)
			{
				listValue.Add(item);
			}

			return listValue;
		}

		public static T ParseJSON<T>(this Dictionary<string, object> dictionary, string key)
		{
			T value = Activator.CreateInstance<T>();

			if (!dictionary.ContainsKey(key) || dictionary[key] == null)
			{
				return default(T);
			}

			JavaScriptSerializer jss = new JavaScriptSerializer();
			value = jss.Deserialize<T>(dictionary[key].ToString());

			return value;
		}
	}
}

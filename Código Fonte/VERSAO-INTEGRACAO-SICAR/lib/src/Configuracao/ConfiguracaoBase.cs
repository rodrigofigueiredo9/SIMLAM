using System;
using System.Collections.Generic;
using System.Reflection;
using Tecnomapas.Blocos.Entities.Configuracao;

namespace Tecnomapas.EtramiteX.Configuracao
{
	public class ConfiguracaoBase : IConfiguracao
	{
		private Dictionary<String, Object> _dic;
		public Dictionary<String, Object> Dicionario { get { return _dic; } }

		protected void LoadDictionary(Dictionary<String, Object> dictionayr)
		{
			_dic = dictionayr;
		}

		public object this[string idx]
		{
			get { return Obter<object>(idx); }
		}

		public T Obter<T>(String key)
		{
			Object valor = null;
			Type tipo = GetType();
			String keyCache = tipo.FullName + key;

            if (GerenciadorCache.Cache[keyCache] != null)
                return (T)GerenciadorCache.Cache[keyCache];

			if (_dic.ContainsKey(key))
			{
				valor = _dic[key];
			}
			else
			{
				PropertyInfo propInfo = GetType().GetProperty(key);
				if (propInfo == null)
					throw new Exception(String.Format("Propriedade {0} não encontrada no objeto de configuração {1}", key, GetType().Name));

				valor = propInfo.GetValue(this, null);
			}

			GerenciadorCache.SetCache(keyCache, valor);

			return (T)valor;
		}

		public object Atual(String key)
		{
			Object valor = null;

			if (_dic.ContainsKey(key))
			{
				valor = _dic[key];
			}
			else
			{
				PropertyInfo propInfo = GetType().GetProperty(key);
				if (propInfo == null)
					throw new Exception(String.Format("Propriedade {0} não encontrada no objeto de configuração {1}", key, GetType().Name));

				valor = propInfo.GetValue(this, null);
			}
			return valor;
		}

		public ConfiguracaoBase()
		{
			_dic = new Dictionary<string, object>();
		}

	}
}

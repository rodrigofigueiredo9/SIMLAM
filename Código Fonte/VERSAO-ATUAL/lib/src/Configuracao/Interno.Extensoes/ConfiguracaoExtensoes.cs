using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Configuracao;

namespace Tecnomapas.EtramiteX.Configuracao.Interno.Extensoes
{
	public static class ConfiguracaoExtensoes
	{
		public const String OUTRO = "outro";

		public static List<T> MoverItemFinal<T>(this List<T> lst, String valor = OUTRO)
		{
			T item;

			if (typeof(T).GetInterface(typeof(IListaValor).Name) != null)
			{
				item = lst.Single(x => String.Equals(((IListaValor)x).Texto, valor, StringComparison.InvariantCultureIgnoreCase));
				
			}
			else
			{
				item = lst.Single(x => String.Equals(((IListaValorString)x).Texto, valor, StringComparison.InvariantCultureIgnoreCase));
			}

			lst.Remove(item);
			lst.Add(item);

			return lst;
		}
	}
}
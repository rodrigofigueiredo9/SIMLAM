using System;
using System.Collections.Generic;
using System.Linq;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		public static String Concatenar(List<String> lista, char separador = ',')
		{
			if (lista.Count == 0)
				return String.Empty;

			if (lista.Count == 1)
				return lista[0];

			string lastItem = lista.Last();
			lista.RemoveAt(lista.Count - 1);
			return String.Join(separador + " ", lista) + " e " + lastItem;
			
		}

		public static String Concatenar(string textoStragg, char separador = ';')
		{
			List<String> lista = textoStragg.Split(separador).ToList();

			if (lista.Count == 0)
				return String.Empty;

			if (lista.Count == 1)
				return lista[0];

			string lastItem = lista.Last();
			lista.RemoveAt(lista.Count - 1);
			return String.Join(", ", lista) + " e " + lastItem;
		}
	}
}

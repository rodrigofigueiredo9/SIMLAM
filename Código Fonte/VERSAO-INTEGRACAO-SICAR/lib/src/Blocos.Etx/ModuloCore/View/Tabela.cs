

using System.Collections.Generic;

namespace Tecnomapas.Blocos.Etx.ModuloCore.View
{
	public class Tabela
	{
		public List<string> Colunas { get; private set; }
		public List<Dictionary<string, string>> Linhas { get; private set; }

		public Tabela()
		{
			Colunas = new List<string>();
			Linhas = new List<Dictionary<string, string>>();
		}
	}
}

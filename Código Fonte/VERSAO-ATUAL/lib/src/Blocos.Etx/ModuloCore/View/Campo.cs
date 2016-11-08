

using System.Collections.Generic;

namespace Tecnomapas.Blocos.Etx.ModuloCore.View
{
	public class Campo
	{
		public int Ordem { get; set; }
		public string Alias { get; set; }
		public string Valor { get; set; }
		public string Classe { get; set; }
		public List<Link> Links { get; private set; }

		public Campo()
		{
			Links = new List<Link>();
		}

		public override string ToString()
		{
			return string.Format("[Alias = {0}; Valor = {1}, Links = {2}]", Alias, Valor, Links.Count);
		}
	}
}

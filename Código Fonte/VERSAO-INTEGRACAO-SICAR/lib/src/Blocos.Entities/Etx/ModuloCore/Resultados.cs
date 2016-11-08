

using System.Collections.Generic;

namespace Tecnomapas.Blocos.Entities.Etx.ModuloCore
{
	public class Resultados<T>
	{
		private List<T> _itens = new List<T>();

		public List<T> Itens
		{
			get { return _itens; }
			set { _itens = value; }
		}

		public int Quantidade { get; set; }
	}
}
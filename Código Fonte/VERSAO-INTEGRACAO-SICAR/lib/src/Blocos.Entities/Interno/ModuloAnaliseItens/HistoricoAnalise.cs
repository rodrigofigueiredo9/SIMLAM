using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.ModuloRoteiro;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloAnaliseItens
{
	public class HistoricoAnalise
	{
		public int TipoHist { get; set; }
		public string NomeHist { get; set; }
		private List<Item> _analises = new List<Item>();
		public List<Item> Analises
		{
			get { return _analises; }
			set { _analises = value; }
		}
	}
}

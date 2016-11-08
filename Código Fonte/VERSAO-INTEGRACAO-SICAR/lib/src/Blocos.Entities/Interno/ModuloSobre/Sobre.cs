

using System.Collections.Generic;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloSobre
{
	public class Sobre
	{
		public int Id { get; set; }
		public string Versao { get; set; }
		public string Data { get; set; }
		public string Licenciado { get; set; }
		public List<SobreItem> Itens { get; set; }

		public Sobre()
		{
			this.Versao = "2.8";
			this.Data = "06/03/2012";
			this.Licenciado = "IDAF - Instituto de Defesa Agropecuária e Florestal do ES";
			/*
			this.Versao =
			this.Data =
			this.Licenciado = string.Empty;
			this.Itens = new List<SobreItem>();
			*/
		}
	}
}

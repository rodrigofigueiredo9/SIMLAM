using System;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloSilviculturaPPFF
{
	public class SilviculturaPPFFItem
	{
		public Int32 Id { get; set; }
		public Municipio Municipio { get; set; }
		public String Tid { get; set; }

		public SilviculturaPPFFItem()
		{
			this.Municipio = new Municipio();
		}
	}
}
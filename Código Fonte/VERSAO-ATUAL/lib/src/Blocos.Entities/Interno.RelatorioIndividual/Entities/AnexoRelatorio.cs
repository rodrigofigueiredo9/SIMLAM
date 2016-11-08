

using System;

namespace Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.Entities
{
	public class AnexoRelatorio
	{
		public String Descricao { get; set; }
		public Arquivo.Arquivo Arquivo { get; set; }

		public AnexoRelatorio() 
		{
			Arquivo = new Arquivo.Arquivo();
		}
	}
}

using System;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Praga;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado
{
	public class PragaHabilitarEmissao1
	{
		public int Id { get; set; }
		public String Tid { get; set; }
		public Praga Praga { get; set; }
		public String Cultura { get; set; }
		public String DataInicialHabilitacao { get; set; }
		public String DataFinalHabilitacao { get; set; }

		public PragaHabilitarEmissao1()
		{
			Praga = new Praga();
		}
	}
}
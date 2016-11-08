using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Autenticacao;
using Tecnomapas.Blocos.Entities.Interno.ModuloPapel;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Arquivo;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Praga;

namespace Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloCredenciado
{
	public class PragaHabilitarEmissaoRelatorio2
	{
		public int Id { get; set; }
		public String Tid { get; set; }
		public Praga Praga { get; set; }
		public String Cultura { get; set; }
		public String DataInicialHabilitacao { get; set; }
		public String DataFinalHabilitacao { get; set; }

		public PragaHabilitarEmissaoRelatorio2()
		{
			Praga = new Praga();
		}
	}
}
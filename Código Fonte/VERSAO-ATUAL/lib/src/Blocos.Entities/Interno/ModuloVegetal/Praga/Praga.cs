using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Cultura;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Praga
{
	public class Praga
	{
		public int Id { get; set; }
		public string Tid { get; set; }
		public int IdRelacionamento { get; set; }
		public string NomeCientifico { get; set; }
		public string NomeComum { get; set; }
		public string CulturasTexto { get; set; }
		public List<Cultura.Cultura> Culturas { get; set; }

		public Praga()
		{
			Culturas = new List<Cultura.Cultura>();
		}
	}
}
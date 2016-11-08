using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Cultura
{
	public class Cultura
	{
		public int IdRelacionamento { get; set; }
		public int Id { get; set; }

		public string Nome { get; set; }

		public string NomeCultivar { get; set; }

		public string Tid { get; set; }

		private Cultivar _Cultivar = new Cultivar();

		public Cultivar Cultivar
		{
			get { return _Cultivar; }
			set { _Cultivar = value; }
		}

		private List<Cultivar> _lstCultivar = new List<Cultivar>();

		public List<Cultivar> LstCultivar
		{
			get { return _lstCultivar; }
			set { _lstCultivar = value; }
		}		
	}
}

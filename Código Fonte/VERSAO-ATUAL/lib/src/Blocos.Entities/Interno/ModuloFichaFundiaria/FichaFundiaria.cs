

using System;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloFichaFundiaria
{
	public class FichaFundiaria
	{
		public Int32 Id { get; set; }
		public String Codigo { get; set; }
		public String ProtocoloGeral { get; set; }
		public String ProtocoloRegional { get; set; }

		private Requerente _requerente = new Requerente();
		public Requerente Requerente
		{
			get { return _requerente; }
			set { _requerente = value; }
		}

		private Terreno _terreno = new Terreno();
		public Terreno Terreno
		{
			get { return _terreno; }
			set { _terreno = value; }
		}

		public String ConfrontanteNorte { get; set; }
		public String ConfrontanteSul { get; set; }
		public String ConfrontanteLeste { get; set; }
		public String ConfrontanteOeste { get; set; }

		private Escritura _escrituraCondicional = new Escritura();
		public Escritura EscrituraCondicional
		{
			get { return _escrituraCondicional; }
			set { _escrituraCondicional = value; }
		}

		private Escritura _escrituraDefinitiva = new Escritura();
		public Escritura EscrituraDefinitiva
		{
			get { return _escrituraDefinitiva; }
			set { _escrituraDefinitiva = value; }
		}

		public String Observacoes { get; set; }
	}
}

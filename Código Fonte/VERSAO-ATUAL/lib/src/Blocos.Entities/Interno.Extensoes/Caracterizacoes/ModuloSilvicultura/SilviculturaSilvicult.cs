using System;
using System.Collections.Generic;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloSilvicultura
{
	public class SilviculturaSilvicult
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		public String Identificacao { get; set; }
		public Int32 GeometriaTipo { get; set; }
		public String GeometriaTipoTexto { get; set; }
		public Decimal AreaCroqui { get; set; }
		public Decimal AreaCroquiHa { get; set; }

		private List<CulturaFlorestal> _culturas = new List<CulturaFlorestal>();
		public List<CulturaFlorestal> Culturas
		{
			get { return _culturas; }
			set { _culturas = value; }
		}
	}
}



using System;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloSilvicultura
{
	public class CulturaFlorestal
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		public Int32 CulturaTipo { get; set; }
		public String CulturaTipoTexto { get; set; }
		public String EspecificarTexto { get; set; }
		public Decimal AreaCultura { get; set; }
		public Decimal AreaCulturaHa { get; set; }
		public String AreaCulturaTexto { get; set; }
	}
}
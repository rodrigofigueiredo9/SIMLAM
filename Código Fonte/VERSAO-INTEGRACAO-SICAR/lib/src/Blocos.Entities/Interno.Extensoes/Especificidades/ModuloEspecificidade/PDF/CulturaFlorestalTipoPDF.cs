using System;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF
{
	public class CulturaFlorestalTipoPDF
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		public Int32 CulturaTipo { get; set; }
		public String CulturaTipoTexto { get; set; }
		public String EspecificarTexto { get; set; }
		public String AreaCultura { get; set; }
		public String AreaCulturaHa { get; set; }
		public String AreaCulturaTexto { get; set; }
	}
}
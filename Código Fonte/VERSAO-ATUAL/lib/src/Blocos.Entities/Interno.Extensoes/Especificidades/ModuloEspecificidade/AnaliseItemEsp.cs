using System;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade
{	
	public class AnaliseItemEsp
	{
		public Int32 Id { get; set; }
		public string Tid { get; set; }
		public String Nome { get; set; }
		public Int32 Situacao { get; set; }
		public String SituacaoTexto { get; set; }

		public AnaliseItemEsp() { }
	}
}
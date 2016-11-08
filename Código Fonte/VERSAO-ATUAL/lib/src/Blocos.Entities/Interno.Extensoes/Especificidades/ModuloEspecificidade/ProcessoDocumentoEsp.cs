using System;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade
{
	public class ProtocoloEsp
	{
		public int Id { get; set; }
		public String Numero { get; set; }
		public int Tipo { get; set; }
		public int RequerimentoId { get; set; }
		public String TipoTexto { get; set; }
		public bool TemEmpreendimento { get; set; }
		public bool IsProcesso { get; set; }
		public String NumeroTipo { get { return this.Numero + " - " + this.TipoTexto; } }

		public ProtocoloEsp()
		{
			IsProcesso = true;
		}
	}
}
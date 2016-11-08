using System;

namespace Tecnomapas.Blocos.Entities.Etx.ModuloRelatorio
{
	public class AssinanteDefault: IAssinante
	{
		public int Id { get; set; }
		public String Nome { get; set; }
		public String Cargo { get; set; }
		public String TipoTexto { get; set; }
	}
}
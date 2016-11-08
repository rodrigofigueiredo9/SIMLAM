using System.Collections.Generic;

namespace Tecnomapas.Blocos.RelatorioPersonalizado.Entities
{
	public class GrupoRelatorio
	{
		public List<Relatorio> RelatorioPersonalizado { get; set; }
		public List<Usuario> Usuario { get; set; }

		public GrupoRelatorio()
		{
			RelatorioPersonalizado = new List<Relatorio>();
			Usuario = new List<Usuario>();
		}
	}
}
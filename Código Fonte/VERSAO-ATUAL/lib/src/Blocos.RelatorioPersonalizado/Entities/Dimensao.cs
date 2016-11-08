using System.Collections.Generic;
using System.Linq;

namespace Tecnomapas.Blocos.RelatorioPersonalizado.Entities
{
	public class Dimensao
	{
		public int Id { get; set; }
		public string Nome { get; set; }
		public string Tabela { get; set; }

		public List<Campo> Campos { get; private set; }
		public IEnumerable<Campo> CamposExibicao
		{
			get
			{
				return Campos.Where(c => c.CampoExibicao);
			}
		}

		public Dimensao()
		{
			Campos = new List<Campo>();
		}
	}
}
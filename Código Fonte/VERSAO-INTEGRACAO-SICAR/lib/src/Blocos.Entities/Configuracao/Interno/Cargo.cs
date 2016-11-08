

using System;

namespace Tecnomapas.Blocos.Entities.Configuracao.Interno
{
	public class Cargo : IListaValor
	{
		public int Id { get; set; }
		public String Nome { get; set; }
		public String Texto { get { return Nome; } }
		public int IdRelacao { get; set; }
		public int Codigo { get; set; }
		public bool IsAtivo { get; set; }
	}
}
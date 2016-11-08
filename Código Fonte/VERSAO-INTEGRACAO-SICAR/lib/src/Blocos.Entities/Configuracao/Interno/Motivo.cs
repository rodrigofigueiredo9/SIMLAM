

using System;

namespace Tecnomapas.Blocos.Entities.Configuracao.Interno
{
	public class Motivo : IListaValor
	{
		public int Id { get; set; }
		public int IdRelacionamento { get; set; }
		public String Nome { get; set; }
		public String Texto { get { return Nome; } }
		public bool IsAtivo { get; set; }
	}
}
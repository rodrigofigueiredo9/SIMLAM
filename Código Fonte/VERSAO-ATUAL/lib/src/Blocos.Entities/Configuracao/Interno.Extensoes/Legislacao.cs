

using System;

namespace Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes
{
	public class Legislacao : IListaValor
	{
		public int Id { get; set; }
		public int IdRelacionamento { get; set; }
		public String Nome { get; set; }
		public String Texto { get { return Nome; } }
		public bool IsAtivo { get; set; }
	}
}
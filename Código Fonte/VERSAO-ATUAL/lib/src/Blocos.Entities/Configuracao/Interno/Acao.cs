

using System;

namespace Tecnomapas.Blocos.Entities.Configuracao.Interno
{
	public class Acao : IListaValor
	{
		public int Id { get; set; }
		public String Texto { get; set; }
		public bool IsAtivo { get; set; }
		public bool Mostrar { get; set; }
		public bool Habilitado { get; set; }
	}
}
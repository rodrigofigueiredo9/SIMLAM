

using System;

namespace Tecnomapas.Blocos.Entities.Configuracao.Interno
{
	public class TituloCondicionanteSituacao : IListaValor
	{
		public int Id { get; set; }
		public String Texto { get; set; }
		public bool IsAtivo { get; set; }
	}
}
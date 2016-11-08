

using System;

namespace Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes
{
	public class Dia : IListaValor
	{
		public int Id { get; set; }
		public String Texto { get; set; }
		public bool IsAtivo { get; set; }
		public int Codigo { get; set; }
	}
}
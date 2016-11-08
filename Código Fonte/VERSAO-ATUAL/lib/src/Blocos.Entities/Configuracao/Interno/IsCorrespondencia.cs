

using System;

namespace Tecnomapas.Blocos.Entities.Configuracao.Interno
{
	public class IsCorrespondencia : IListaValor
	{
		public const int SIM = 1;
		public const int NAO = 0;

		public int Id { get; set; }
		public String Texto { get; set; }
		public bool IsAtivo { get; set; }
	}
}
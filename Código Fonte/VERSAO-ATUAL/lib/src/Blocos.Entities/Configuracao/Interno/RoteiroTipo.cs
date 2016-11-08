

using System;

namespace Tecnomapas.Blocos.Entities.Configuracao.Interno
{
	public class RoteiroTipo : IListaValor
	{
		public const int TECNICO = 1;
		public const int ADMINISTRATIVO = 2;
		public const int PROJETODIGITAL = 3;

		public int Id { get; set; }
		public String Texto { get; set; }
		public bool IsAtivo { get; set; }
	}
}
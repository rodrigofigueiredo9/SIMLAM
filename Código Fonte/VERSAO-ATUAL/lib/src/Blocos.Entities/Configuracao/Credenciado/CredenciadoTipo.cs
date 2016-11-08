

using System;

namespace Tecnomapas.Blocos.Entities.Configuracao.Credenciado
{
	public class CredenciadoTipo : IListaValor
	{
		public const int RESPONSAVEL_TECNICO = 1;
		public const int INTERESSADO = 2;

		public int Id { get; set; }
		public String Texto { get; set; }
		public bool IsAtivo { get; set; }
	}
}
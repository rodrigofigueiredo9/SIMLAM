

using System;

namespace Tecnomapas.Blocos.Entities.Configuracao.Interno
{
	public class TramitacaoArquivoTipo : IListaValor
	{
		public int Id { get; set; }
		public String Texto { get; set; }
		public bool IsAtivo { get; set; }
	}
}
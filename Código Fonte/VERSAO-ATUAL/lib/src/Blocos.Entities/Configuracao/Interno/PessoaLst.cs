

using System;

namespace Tecnomapas.Blocos.Entities.Configuracao.Interno
{
	public class PessoaLst : IListaValor
	{
		public int Id { get; set; }
		public String Texto { get; set; }
		public String CPFCNPJ { get; set; }
		public int VinculoTipo { get; set; }
		public bool IsAtivo { get; set; }
	}
}
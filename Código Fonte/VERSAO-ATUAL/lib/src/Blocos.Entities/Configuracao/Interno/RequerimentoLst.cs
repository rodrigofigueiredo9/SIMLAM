

using System;

namespace Tecnomapas.Blocos.Entities.Configuracao.Interno
{
	public class RequerimentoLst : IListaValor
	{
		public int Id { get; set; }
		public String Texto { get; set; }
		public String Tipo { get; set; }
		public bool IsAtivo { get; set; }
	}
}

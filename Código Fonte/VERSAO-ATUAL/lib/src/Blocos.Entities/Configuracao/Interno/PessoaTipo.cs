using System;
using System.ComponentModel.DataAnnotations;

namespace Tecnomapas.Blocos.Entities.Configuracao.Interno
{
	public class PessoaTipo : IListaValor
	{
		public const int FISICA = 1;
		public const int JURIDICA = 2;
		public const int EXPORTACAO = 3;

		public int Id { get; set; }
		[Display(Name = "Tipo")]
		public String Texto { get; set; }
		public bool IsAtivo { get; set; }
	}
}
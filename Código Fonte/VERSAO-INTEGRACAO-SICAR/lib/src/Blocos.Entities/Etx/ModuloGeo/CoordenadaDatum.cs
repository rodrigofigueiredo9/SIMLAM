using System.ComponentModel.DataAnnotations;
using Tecnomapas.Blocos.Entities.Configuracao;

namespace Tecnomapas.Blocos.Entities.Etx.ModuloGeo
{
	public class CoordenadaDatum : IListaValor
	{
		public int Id { get; set; }
		public bool IsAtivo { get; set; }
		public string Sigla { get; set; }
		[Display(Name = "Datum")]
		public string Texto { get; set; }

		public CoordenadaDatum() { }
	}
}
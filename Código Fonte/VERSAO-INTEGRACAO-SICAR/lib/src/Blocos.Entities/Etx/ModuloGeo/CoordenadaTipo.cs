using System.ComponentModel.DataAnnotations;
using Tecnomapas.Blocos.Entities.Configuracao;

namespace Tecnomapas.Blocos.Entities.Etx.ModuloGeo
{
	public class CoordenadaTipo : IListaValor
	{
		public int Id { get; set; }
		public bool IsAtivo { get; set; }
		[Display(Name = "Sistema de coordenada")]
		public string Texto { get; set; }

		public CoordenadaTipo() { }
	}
}
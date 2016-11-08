

using System.Data;

namespace Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.Entities
{
	public class DadoFiltroRelatorio
	{
		public DbType Tipo { get; set; }
		public object Valor { get; set; }

		public DadoFiltroRelatorio() { }

		public DadoFiltroRelatorio(DbType tipo, object valor)
		{
			Tipo = tipo;
			Valor = valor;
		}
	}
}
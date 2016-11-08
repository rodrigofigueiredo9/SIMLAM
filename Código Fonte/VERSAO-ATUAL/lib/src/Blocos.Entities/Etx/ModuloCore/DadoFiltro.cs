

using System.Data;

namespace Tecnomapas.Blocos.Entities.Etx.ModuloCore
{
	public class DadoFiltro
	{
		public DbType Tipo { get; set; }
		public object Valor { get; set; }

		public DadoFiltro() { }

		public DadoFiltro(DbType tipo, object valor)
		{
			Tipo = tipo;
			Valor = valor;
		}
	}
}
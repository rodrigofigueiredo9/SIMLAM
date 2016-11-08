

using System;

namespace Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloCaracterizacoes
{
	public class DominialidadeAreaRelatorio
	{
		public Int32? Id { get; set; }
		public String Tid { get; set; }
		public Int32 Tipo { get; set; }
		public String TipoTexto { get; set; }
		public Decimal Valor { get; set; }
		public String Descricao { get; set; }

		public DominialidadeAreaRelatorio() { }
	}
}
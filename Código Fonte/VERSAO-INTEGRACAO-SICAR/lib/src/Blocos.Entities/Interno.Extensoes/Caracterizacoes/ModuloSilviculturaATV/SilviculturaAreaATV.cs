

using System;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloSilviculturaATV
{
	public class SilviculturaAreaATV
	{
		public Int32? Id { get; set; }
		public String Tid { get; set; }
		public Int32 Tipo { get; set; }
		public String TipoTexto { get; set; }
		public Decimal Valor { get; set; }
		public String ValorTexto { get; set; }
		public String Descricao { get; set; }

		public SilviculturaAreaATV()
		{
			this.Tid =
			this.TipoTexto =
			this.ValorTexto =
			this.Descricao = String.Empty;
		}
	}
}
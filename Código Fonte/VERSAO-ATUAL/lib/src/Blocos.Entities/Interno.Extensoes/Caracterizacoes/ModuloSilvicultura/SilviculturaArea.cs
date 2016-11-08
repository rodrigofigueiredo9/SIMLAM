

using System;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloSilvicultura
{
	public class SilviculturaArea
	{
		public Int32? Id { get; set; }
		public String Tid { get; set; }
		public Int32 Tipo { get; set; }
		public String TipoTexto { get; set; }
		public Decimal Valor { get; set; }
		public String ValorTexto { get { return Valor.ToStringTrunc(); } }
		public String Descricao { get; set; }

		public SilviculturaArea()
		{
			this.Tid =
			this.TipoTexto =
			this.Descricao = String.Empty;
		}
	}
}
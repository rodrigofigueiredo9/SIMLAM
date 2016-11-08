

using System;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao
{
	public class Area
	{
		public Int32? Id { get; set; }
		public String Tid { get; set; }
		public Int32 Tipo { get; set; }
		public String TipoTexto { get; set; }
		public Decimal Valor { get; set; }
		public Decimal ValorHA { get { return (Valor > 0) ? Valor.Convert(eMetrica.M2ToHa) : 0m; } }
		public String Descricao { get; set; }

		public Area()
		{
			this.Tid =
			this.TipoTexto =
			this.Descricao = String.Empty;
		}
	}
}
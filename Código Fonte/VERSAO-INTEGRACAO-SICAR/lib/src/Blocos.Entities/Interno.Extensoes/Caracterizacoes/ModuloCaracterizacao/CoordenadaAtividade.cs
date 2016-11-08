

using System;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao
{
	public class CoordenadaAtividade
	{
		public Int32 Id { get; set; }
		public Int32 Tipo { get; set; }
		public String TipoTexto { get; set; }
		public String CoordenadasAtividade 
		{
			get 
			{
				return this.CoordX.ToString("N2") + " - " + CoordY.ToString("N2");
			} 
		}
		public decimal CoordX { get; set; }
		public decimal CoordY { get; set; }
	}
}

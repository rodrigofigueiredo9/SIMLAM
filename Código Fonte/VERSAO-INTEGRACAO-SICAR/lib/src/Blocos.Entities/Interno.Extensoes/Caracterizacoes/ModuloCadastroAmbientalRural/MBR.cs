

using System;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCadastroAmbientalRural
{
	public class MBR
	{
		public Decimal MenorX { get; set; }
		public Decimal MenorY { get; set; }
		public Decimal MaiorX { get; set; }
		public Decimal MaiorY { get; set; }

		public void Corrigir()
		{
			if (MenorX > MaiorX)
			{
				decimal aux = MenorX;
				MenorX = MaiorX;
				MaiorX = aux;
			}

			if (MenorY > MaiorY)
			{
				decimal aux = MenorY;
				MenorY = MaiorY;
				MaiorY = aux;
			}
		}
	}
}

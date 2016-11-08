using System.Collections.Generic;

namespace Tecnomapas.Blocos.RelatorioPersonalizado.Entities
{
	public class Sumario
	{
		public Campo Campo { get; set; }
		public int CampoId { get; set; }
		public int CampoCodigo { get; set; }
		public bool Contar { get; set; }
		public bool Somar { get; set; }
		public bool Media { get; set; }
		public bool Maximo { get; set; }
		public bool Minimo { get; set; }

		public Sumario()
		{
			Campo = new Campo();
		}

		public Dictionary<eTipoSumario, string> ListarSumarios()
		{
			Dictionary<eTipoSumario, string> itens = new Dictionary<eTipoSumario, string>();

			if (Contar)
			{
				itens.Add(eTipoSumario.Contar, "Itens");
			}
			if (Somar)
			{
				itens.Add(eTipoSumario.Somar, "Total");
			}
			if (Media)
			{
				itens.Add(eTipoSumario.Media, "Média");
			}
			if (Maximo)
			{
				itens.Add(eTipoSumario.Maximo, "Máximo");
			}
			if (Minimo)
			{
				itens.Add(eTipoSumario.Minimo, "Mínimo");
			}

			return itens;
		}
	}
}

using System;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloInformacaoCorte
{
	public class InformacaoCorteTipo
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }

		public Int32 TipoCorte { get; set; }
		public Int32 EspecieInformada { get; set; }

		public Decimal AreaCorte { get; set; }
		public String IdadePlantio { get; set; }
	}
}

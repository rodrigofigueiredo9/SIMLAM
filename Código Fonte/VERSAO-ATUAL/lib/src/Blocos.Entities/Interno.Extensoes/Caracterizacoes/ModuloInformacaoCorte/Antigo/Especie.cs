

using System;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloInformacaoCorte.Antigo
{
	public class Especie
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }

		public Int32 EspecieTipo { get; set; }
		public String EspecieTipoTexto { get; set; }
		public String EspecieEspecificarTexto { get; set; }

		public String ArvoresIsoladas { get; set; }
		public String AreaCorte { get; set; }
		public String IdadePlantio { get; set; }
	}
}

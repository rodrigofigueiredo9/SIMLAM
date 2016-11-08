using System;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloRegistroAtividadeFlorestal
{
	public class FonteEnergia
	{
		public Int32 Id { get; set; }
		public Int32 TipoId { get; set; }
		public String TipoTexto { get; set; }
		public Int32 UnidadeId { get; set; }
		public String UnidadeTexto { get; set; }
		public String Qde { get; set; }
		public String QdeFlorestaPlantada { get; set; }
		public String QdeFlorestaNativa { get; set; }
		public String QdeOutroEstado { get; set; }
		public String Tid { get; set; }
	}
}
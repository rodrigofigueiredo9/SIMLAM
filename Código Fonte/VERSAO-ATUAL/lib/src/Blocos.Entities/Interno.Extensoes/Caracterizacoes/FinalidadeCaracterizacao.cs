using System;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes
{
	public class FinalidadeCaracterizacao
	{
		public int Id { get; set; }
		public Boolean? EmitidoPorInterno { get; set; }
		public Int32? TituloId { get; set; }
		public String TituloNumero { get; set; }
		public Int32? TituloModelo { get; set; }
		public String TituloModeloTexto { get; set; }
		public DateTecno TituloValidadeData { get; set; }
		public String ProtocoloNumero { get; set; }
		public DateTecno ProtocoloRenovacaoData { get; set; }
		public String ProtocoloRenovacaoNumero { get; set; }
		public String OrgaoExpedidor { get; set; }

		public FinalidadeCaracterizacao()
		{
			TituloValidadeData = new DateTecno();
			ProtocoloRenovacaoData = new DateTecno();
		}
	}
}
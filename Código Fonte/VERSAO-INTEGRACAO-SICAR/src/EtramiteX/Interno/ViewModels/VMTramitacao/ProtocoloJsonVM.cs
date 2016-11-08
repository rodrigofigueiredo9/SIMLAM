using System;
using Tecnomapas.Blocos.Entities.Interno.ModuloTramitacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMTramitacao
{
	public class ProtocoloJsonVM
	{
		public Boolean IsProcesso { get; set; }		
		public Int32? Id { get; set; }
		public String Numero { get; set; }
		public Int32 HistorioId { get; set; }
		public Int32 TramitacaoId { get; set; }

		public ProtocoloJsonVM(Tramitacao tramitacao)
		{
			Id = tramitacao.Protocolo.Id;
			IsProcesso = tramitacao.Protocolo.IsProcesso;
			Numero = tramitacao.Protocolo.Numero;
			HistorioId = tramitacao.HistoricoId;
			TramitacaoId = tramitacao.Id;
		}
	}
}
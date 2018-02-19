using System;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao
{
	public class NotificacaoVM
	{
		public Int32 fiscalizacaoId { get; set; }
		public Boolean PodeCriar { get; set; }
		public Boolean PodeEditar { get; set; }
		public Notificacao Notificacao { get; set; }

		public NotificacaoVM()
		{
			Notificacao = new Notificacao();
		}
	}
}
using System;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao
{
	public class NotificacaoVM
	{
		public Int32 fiscalizacaoId { get; set; }
		public Boolean PodeCriar { get; set; }
		public Boolean PodeEditar { get; set; }
		public Notificacao Notificacao { get; set; }
		public ArquivoVM ArquivoVM { get; set; }

		public NotificacaoVM()
		{
			Notificacao = new Notificacao();
			ArquivoVM = new ArquivoVM();
		}

		public String ArquivoJSon { get; set; }
		public String GetJson(object obj) => ViewModelHelper.Json(obj);
	}
}
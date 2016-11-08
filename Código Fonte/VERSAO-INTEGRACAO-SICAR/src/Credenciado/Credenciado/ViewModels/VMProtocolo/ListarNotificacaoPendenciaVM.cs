using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;

namespace Tecnomapas.EtramiteX.Credenciado.ViewModels.VMProtocolo
{
	public class ListarNotificacaoPendenciaVM
	{
		public bool PodeVisualizar { get; set; }

		private List<NotificacaoPendencia> _resultados = new List<NotificacaoPendencia>();
		public List<NotificacaoPendencia> Resultados
		{
			get { return _resultados; }
			set { _resultados = value; }
		}

		public ListarNotificacaoPendenciaVM()
		{

		}
	}
}
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Interno.ModuloRoteiro;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMRoteiro
{
	public class ItemConfirmarVM
	{
		public string MensagemRoteiro { get; set; }
		
		public ItemConfirmarVM(){  }

		public void CarregarMensagem(List<Roteiro> roteiros)
		{
			if (roteiros.Count < 1)
				return;

			MensagemRoteiro = Mensagem.Roteiro.AtualizarRoteiro(string.Join(",", roteiros.Select(x => x.Numero).ToList())).Texto;
		}
	}
}
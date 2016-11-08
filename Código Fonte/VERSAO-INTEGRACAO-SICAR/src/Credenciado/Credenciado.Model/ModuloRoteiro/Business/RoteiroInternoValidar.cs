using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloLista.Business;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloRoteiro.Business
{
	public class RoteiroInternoValidar
	{
		public bool RoteiroIsPadrao(int roteiroId)
		{
			return ListaCredenciadoBus.RoteiroPadrao.Exists(x => x.Id == roteiroId);
		}

		public bool PossuiModelosAtividades(List<TituloModeloLst> lista, int roteiro = 0)
		{
			if (lista.Count < 1 && !RoteiroIsPadrao(roteiro))
			{
				Validacao.Add(Mensagem.Roteiro.NenhumTituloEncontrado);
			}

			return Validacao.EhValido;
		}
	}
}

using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloProtocolo.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloProtocolo.Business
{
	public class ProtocoloInternoValidar
	{
		ProtocoloInternoDa _da;

		public ProtocoloInternoValidar()
		{
			_da = new ProtocoloInternoDa();
		}

		public int ProtocoloAssociado(int protocolo)
		{
			ProtocoloNumero retorno = _da.VerificarProtocoloAssociado(protocolo);
			if (retorno != null)
			{
				return retorno.Id;
			}
			return 0;
		}

		public bool ExisteAtividade(int protocolo)
		{
			return _da.ExisteAtividade(protocolo);
		}

		public bool ExisteProtocolo(int id)
		{
			return _da.ExisteProtocolo(id);
		}
	}
}
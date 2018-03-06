using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Business
{
	public class CobrancaDUAValidar
	{
		public bool Salvar(CobrancaDUA cobrancaDUA)
		{
			if (cobrancaDUA.VRTE == 0)
				Validacao.Add(Mensagem.CobrancaDUAMsg.VrteObrigatorio);

			return Validacao.EhValido;
		}
	}
}

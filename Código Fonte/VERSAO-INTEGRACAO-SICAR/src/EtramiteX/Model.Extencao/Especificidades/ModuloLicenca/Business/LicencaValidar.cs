using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloLicenca;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloLicenca.Business
{
	public class LicencaValidar
	{
		LicencaBus _bus = new LicencaBus();

		public bool Validar(ILicenca licenca)
		{
			if (licenca.BarragemId.GetValueOrDefault() == 0)
			{
				Validacao.Add(Mensagem.BarragemMsg.SelecioneEspBarragem);
			}

			if (licenca.BarragemId.GetValueOrDefault() > 0 && !_bus.ExisteBarragem(licenca.BarragemId.GetValueOrDefault()))
			{
				Validacao.Add(Mensagem.BarragemMsg.BarragemExluida);
			}

			return Validacao.EhValido;
		}
	}
}

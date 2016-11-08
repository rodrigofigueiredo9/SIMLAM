using Tecnomapas.Blocos.Data;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloLicenca.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloLicenca.Business
{
	public class LicencaBus
	{
		LicencaDa _da = new LicencaDa();

		public bool ExisteBarragem(int barragemId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				return _da.ExisteBarragem(barragemId, bancoDeDados);
			}
		}
	}
}

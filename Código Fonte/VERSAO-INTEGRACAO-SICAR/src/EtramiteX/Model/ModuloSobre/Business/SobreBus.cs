using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.ModuloSobre;
using Tecnomapas.EtramiteX.Interno.Model.ModuloSobre.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloSobre.Business
{
	public class SobreBus
	{
		#region Propriedades

		private SobreDa _daSobre;

		#endregion	

		public SobreBus()
		{
			this._daSobre = new SobreDa();
		}

		public Sobre Obter()
		{
			var sobre = _daSobre.Obter();
			sobre.Licenciado = FormatarTextoLicenciado(sobre.Licenciado);
			return sobre;
		}

		public string FormatarTextoLicenciado(string strLicenciado)
		{
			return strLicenciado.Replace(" Do ", " do ").Replace(" De ", " de ").Replace(" E ", " e ");
		}

		public List<SobreItem> ObterSobreItens(int versaoId)
		{
			return _daSobre.ObterSobreItens(versaoId);
		}

		public List<Sobre> ObterVersoes()
		{
			var lstVersoes = _daSobre.ObterVersoes();
			lstVersoes.ForEach(x => { x.Licenciado = FormatarTextoLicenciado(x.Licenciado); });
			return lstVersoes;
		}
	}
}
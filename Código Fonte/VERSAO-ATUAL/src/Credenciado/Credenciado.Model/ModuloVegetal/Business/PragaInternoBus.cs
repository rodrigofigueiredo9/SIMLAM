using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Cultura;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloVegetal.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloVegetal.Business
{
	public class PragaInternoBus
	{
		#region Propriedades

		PragaInternoDa _da = new PragaInternoDa();

		#endregion

		public PragaInternoBus() { }

		#region Obter/Filtrar

		public List<Cultura> ObterCulturas(int pragaId)
		{
			List<Cultura> culturas = null;
			try
			{
				culturas = _da.ObterCulturas(pragaId);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return culturas;
		}

		#endregion
	}
}
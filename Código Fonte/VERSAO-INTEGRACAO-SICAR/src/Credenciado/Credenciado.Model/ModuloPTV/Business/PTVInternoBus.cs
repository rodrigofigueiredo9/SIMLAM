using System;
using Tecnomapas.Blocos.Entities.Interno.ModuloPTV;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloPTV.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloPTV.Business
{
	public class PTVInternoBus
	{
		#region Propriedades

		PTVInternoDa _da = new PTVInternoDa();

		#endregion

		#region Obter

		public PTV Obter(int id, bool simplificado = false)
		{
			try
			{
				return _da.Obter(id, simplificado);
			}
			catch (Exception ex)
			{
				Validacao.AddErro(ex);
			}

			return null;
		}


		internal PTV ObterPorNumero(long numero, bool simplificado = false)
		{
			try
			{
				return _da.ObterPorNumero(numero, simplificado);
			}
			catch (Exception ex)
			{
				Validacao.AddErro(ex);
			}

			return null;
		}
	
		#endregion
	}
}
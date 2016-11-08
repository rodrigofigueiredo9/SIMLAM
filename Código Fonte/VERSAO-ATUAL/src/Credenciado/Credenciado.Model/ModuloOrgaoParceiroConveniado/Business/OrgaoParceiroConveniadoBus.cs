using System;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Interno.ModuloOrgaoParceiroConveniado;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloOrgaoParceiroConveniado.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloOrgaoParceiroConveniado.Business
{
	public class OrgaoParceiroConveniadoBus
	{
		#region Propriedades

		public OrgaoParceiroConveniadoDa _da = new OrgaoParceiroConveniadoDa();

		#endregion

		#region  Obter/Filtrar

		public OrgaoParceiroConveniado Obter(int id)
		{
			try
			{
				return _da.Obter(id);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		#endregion

		#region Validacoes

		public bool ExisteUnidade(int orgao, BancoDeDados banco = null)
		{
			bool existe = false;

			try
			{
				existe = _da.ExisteUnidade(orgao, banco);
			}
			catch (Exception e)
			{
				Validacao.AddErro(e);
			}

			return existe;

		}

		#endregion
	}
}


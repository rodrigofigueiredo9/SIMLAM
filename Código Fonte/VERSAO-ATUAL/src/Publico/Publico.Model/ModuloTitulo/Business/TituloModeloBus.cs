using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Publico.Model.ModuloTitulo.Data;

namespace Tecnomapas.EtramiteX.Publico.Model.ModuloTitulo.Business
{
	public class TituloModeloBus
	{
		TituloModeloDa _da = new TituloModeloDa();
		public List<TituloModeloLst> ObterModelos(int exceto = 0, bool todos = false)
		{
			try
			{
				List<TituloModeloLst> modelos = _da.ObterModelos(exceto, todos);

				return modelos;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}
	}
}

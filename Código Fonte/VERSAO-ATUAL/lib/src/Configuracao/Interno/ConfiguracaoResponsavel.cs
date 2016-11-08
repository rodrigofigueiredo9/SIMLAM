using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.EtramiteX.Configuracao.Interno.Data;

namespace Tecnomapas.EtramiteX.Configuracao.Interno
{
	public class ConfiguracaoResponsavel: ConfiguracaoBase
	{
		private ListaValoresDa _daLista = new ListaValoresDa();

		public const string KeyResponsavelFuncoes = "ResponsavelFuncoes";
		public List<ResponsavelFuncoes> ResponsavelFuncoes { get { return _daLista.ObterResponsavelFuncoes(); } }
	}
}
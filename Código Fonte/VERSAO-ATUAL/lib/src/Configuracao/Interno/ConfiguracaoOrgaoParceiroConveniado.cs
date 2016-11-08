using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.EtramiteX.Configuracao.Interno.Data;

namespace Tecnomapas.EtramiteX.Configuracao.Interno
{
	public class ConfiguracaoOrgaoParceiroConveniado : ConfiguracaoBase
	{
		private ListaValoresDa _daLista = new ListaValoresDa();

		public const string KeyOrgaoParceirosConveniadosSituacoes = "OrgaoParceirosConveniadosSituacoes";
		public List<Lista> OrgaoParceirosConveniadosSituacoes { get { return _daLista.OrgaoParceirosConveniadosSituacoes(); } }
	}
}
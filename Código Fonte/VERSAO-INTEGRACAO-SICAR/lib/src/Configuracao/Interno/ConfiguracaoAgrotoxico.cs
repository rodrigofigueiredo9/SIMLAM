using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.EtramiteX.Configuracao.Interno.Data;

namespace Tecnomapas.EtramiteX.Configuracao.Interno
{
	public class ConfiguracaoAgrotoxico : ConfiguracaoBase
	{
		private ListaValoresDa _daLista = new ListaValoresDa();

		public const string KeyAgrotoxicoIngredienteAtivoUnidadeMedida = "AgrotoxicoIngredienteAtivoUnidadeMedida";
		public List<Lista> AgrotoxicoIngredienteAtivoUnidadeMedida { get { return _daLista.ObterAgrotoxicoIngredienteAtivoUnidadeMedida(); } }
	}
}
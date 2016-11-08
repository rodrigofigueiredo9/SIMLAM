using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.EtramiteX.Configuracao.Interno.Data;

namespace Tecnomapas.EtramiteX.Configuracao.Interno
{
	public class ConfiguracaoVegetal: ConfiguracaoBase
	{
		private ListaValoresDa _daLista = new ListaValoresDa();

		public const string KeyIngredienteAtivoSituacoes = "IngredienteAtivoSituacoes";
		public List<Situacao> IngredienteAtivoSituacoes { get { return _daLista.ObterIngredienteAtivoSituacoes(); } }

		public const string KeyMensagensAgrotoxicoDesativados = "MensagensAgrotoxicoDesativados";

		public List<ListaValor> MensagensAgrotoxicoDesativados { get { return _daLista.ObterMensagensAgrotoxicosDesativados(); } }

		public const string KeyCultivarTipos = "CultivarTipos";

		public List<Lista> CultivarTipos { get { return _daLista.ObterCultivarTipo(); } }

		public const string KeyDeclaracaoAdicional = "DeclaracaoAdicional";

		public List<Lista> DeclaracaoAdicional { get { return _daLista.ObterDeclaracaoAdicional(); } }
	}
}
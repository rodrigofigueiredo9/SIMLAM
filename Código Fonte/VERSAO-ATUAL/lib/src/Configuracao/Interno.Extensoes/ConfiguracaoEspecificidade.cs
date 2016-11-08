using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using Tecnomapas.EtramiteX.Configuracao.Interno.Extensoes.Data;

namespace Tecnomapas.EtramiteX.Configuracao.Interno.Extensoes
{
	public class ConfiguracaoEspecificidade : ConfiguracaoBase
	{
		private ListaValoresDa _daLista = new ListaValoresDa();

		public const string KeyParecerManifestacaoLegislacoes = "ParecerManifestacaoLegislacoes";
		public List<Legislacao> ParecerManifestacaoLegislacoes { get { return _daLista.ParecerManifestacaoLegislacoes(); } }

		public const string KeyEspecificidadeConclusoes = "EspecificidadeConclusoes";
		public List<Lista> EspecificidadeConclusoes { get { return _daLista.ObterEspecificidadeConclusoes(); } }

		public const string KeyEspecificidadeResultados = "EspecificidadeResultados";
		public List<Lista> EspecificidadeResultados { get { return _daLista.ObterEspecificidadeResultados(); } }

		public const string KeyVinculoPropriedade = "VinculoPropriedade";
		public List<Lista> VinculoPropriedade { get { return _daLista.ObterVinculoPropriedade(); } }

	}
}
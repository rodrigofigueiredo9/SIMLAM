using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using Tecnomapas.EtramiteX.Configuracao.Interno.Extensoes.Data;

namespace Tecnomapas.EtramiteX.Configuracao.Interno.Extensoes
{
	public class ConfiguracaoProjetoGeo : ConfiguracaoBase
	{
		private ListaValoresDa _daLista = new ListaValoresDa();

		public const string KeyObterNiveisPrecisao = "ObterNiveisPrecisao";
		public List<NivelPrecisao> ObterNiveisPrecisao { get { return _daLista.ObterNiveisPrecisao(); } }

		public const string KeyObterSistemaCoordenada = "ObterSistemaCoordenada";
		public List<SistemaCoordenada> ObterSistemaCoordenada { get { return _daLista.ObterSistemaCoordenada(); } }

		public const string KeyTipoSobreposicao = "TipoSobreposicao";
		public List<SobreposicaoTipo> TipoSobreposicao { get { return _daLista.ObterSobreposicaoTipo(); } }

		public const string KeyObterSituacoesProcessamento = "ObterSituacoesProcessamento";
		public List<Situacao> ObterSituacoesProcessamento { get { return _daLista.ObterSituacoesProcessamento(); } }
	}
}
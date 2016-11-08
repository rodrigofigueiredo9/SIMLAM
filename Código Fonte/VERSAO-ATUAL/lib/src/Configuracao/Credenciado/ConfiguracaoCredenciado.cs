using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.EtramiteX.Configuracao.Interno.Data;

namespace Tecnomapas.EtramiteX.Configuracao
{
	public class ConfiguracaoCredenciado : ConfiguracaoBase
	{
		private ListaValoresDa _daLista = new ListaValoresDa();
		
		public const string KeyNumTentativas = "NumTentativas";
		public Int32 NumTentativas { get { return 5; } }

		public const string KeyCredenciadoSituacoes = "CredenciadoSituacoes";
		public List<Situacao> CredenciadoSituacoes { get { return _daLista.ObterCredenciadoSituacoes(); } }

		public const string KeyCredenciadoTipos = "CredenciadoTipos";
		public List<Situacao> CredenciadoTipos { get { return _daLista.ObterCredenciadoTipos(); } }

		public const string KeyHabilitacaoCFOSituacoes = "HabilitacaoCFOSituacoes";
		public List<Situacao> HabilitacaoCFOSituacoes { get { return _daLista.ObterHabilitacaoCFOSituacoes(); } }

		public const string KeyHabilitacaoCFOMotivos = "HabilitacaoCFOMotivos";
		public List<Lista> HabilitacaoCFOMotivos { get { return _daLista.ObterHabilitacaoCFOMotivos(); } }

		public const string KeyLoteSituacoes = "LoteSituacoes";
		public List<Lista> LoteSituacoes { get { return _daLista.ObterLoteSituacoes(); } }
	}
}
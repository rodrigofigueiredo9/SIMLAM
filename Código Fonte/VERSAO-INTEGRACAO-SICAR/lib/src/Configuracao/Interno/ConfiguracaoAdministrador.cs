using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.EtramiteX.Configuracao.Interno.Data;

namespace Tecnomapas.EtramiteX.Configuracao.Interno
{
	public class ConfiguracaoAdministrador : ConfiguracaoBase
	{
		private ListaValoresDa _daLista = new ListaValoresDa();

		public const string KeyCargos = "Cargos";
		public List<Cargo> Cargos { get { return _daLista.ObterCargos(); } }

		public const string KeySetores = "Setores";
		public List<Setor> Setores { get { return _daLista.ObterSetores(); } }

		public const string KeyNumTentativas = "NumTentativas";
		public Int32 NumTentativas { get { return 5; } }
	}
}
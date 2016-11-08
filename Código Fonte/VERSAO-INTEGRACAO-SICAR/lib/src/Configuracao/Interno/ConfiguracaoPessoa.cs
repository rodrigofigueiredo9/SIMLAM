using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.EtramiteX.Configuracao.Interno.Data;

namespace Tecnomapas.EtramiteX.Configuracao.Interno
{
	public class ConfiguracaoPessoa: ConfiguracaoBase
	{

		private ListaValoresDa _daLista = new ListaValoresDa();

		public const string KeyEstadosCivis = "EstadosCivis";
		public List<EstadoCivil> EstadosCivis { get { return _daLista.ObterEstadosCivis(); } }

		public const string KeyProfissoes = "Profissoes";
		public List<ProfissaoLst> Profissoes { get { return _daLista.ObterProfissoes(); } }

		public const string KeyOrgaoClasses = "OrgaoClasses";
		public List<OrgaoClasse> OrgaoClasses { get { return _daLista.ObterOrgaoClasses(); } }

		public const string KeySexos = "Sexos";
		public List<Sexo> Sexos { get { return _daLista.ObterSexos(); } }
	}
}
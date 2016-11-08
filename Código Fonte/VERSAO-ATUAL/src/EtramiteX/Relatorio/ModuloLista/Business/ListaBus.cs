using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloLista.Business
{
	public class ListaBus
	{
		#region Propriedades

		GerenciadorConfiguracao<ConfiguracaoCoordenada> _configCoordenada = new GerenciadorConfiguracao<ConfiguracaoCoordenada>(new ConfiguracaoCoordenada());
		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

		#endregion

		public String EstadoDefault
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyEstadoDefault); }
		}

		public String MunicipioDefault
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyMunicipioDefault); }
		}

		public List<Datum> Datuns
		{
			get { return _configCoordenada.Obter<List<Datum>>(ConfiguracaoCoordenada.KeyDatuns); }
		}

		public List<Fuso> Fusos
		{
			get { return _configCoordenada.Obter<List<Fuso>>(ConfiguracaoCoordenada.KeyFusos); }
		}

		public List<CoordenadaHemisferio> Hemisferios
		{
			get { return _configCoordenada.Obter<List<CoordenadaHemisferio>>(ConfiguracaoCoordenada.KeyHemisferios); }
		}
	}
}
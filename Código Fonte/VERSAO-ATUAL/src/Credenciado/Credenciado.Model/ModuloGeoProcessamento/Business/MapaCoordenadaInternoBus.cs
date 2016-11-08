using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.ModuloGeoProcessamento;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloGeoProcessamento.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloGeoProcessamento.Business
{
	public class MapaCoordenadaInternoBus
	{
		private MapaCoordenadaInternoDa _da = null;
		private GerenciadorConfiguracao<ConfiguracaoSistema> _configSys;

		public String UsuarioGeo
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioGeo); }
		}

		public MapaCoordenadaInternoBus()
		{
			_configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
			_da = new MapaCoordenadaInternoDa(UsuarioGeo);
		}

		public Lote ObterLote(string codfiscal, string codquadra, string codlote)
		{
			return _da.ObterLote(codfiscal, codquadra, codlote);
		}

		public List<Logradouro> ObterLogradouros(string logradouro)
		{
			return _da.ObterLogradouros(logradouro);
		}
	}
}
using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.ModuloGeoProcessamento;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloGeoProcessamento.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloGeoProcessamento.Business
{
	public class MapaCoordenadaBus
	{
		MapaCoordenadaDa _da = null;
		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());		

		public String UsuarioGeo
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioGeo); }
		}

		public MapaCoordenadaBus()
		{
			_da = new MapaCoordenadaDa(strBancoDeDadosGeo: UsuarioGeo);
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

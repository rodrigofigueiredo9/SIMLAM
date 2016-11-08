using System;
using System.Collections.Generic;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Publico.Model.ModuloGeoProcessamento.Data;
using Tecnomapas.EtramiteX.Publico.Model.ModuloGeoProcessamento.Entities;

namespace Tecnomapas.EtramiteX.Publico.Model.ModuloGeoProcessamento.Business
{
	public class PontoEmpreendimentoBus
	{
		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
		PontoEmpreendimentoDa _da = null;

		public String UsuarioGeo
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioGeo); }
		}

		public List<PontoEmpreendimento> listarEmpreendimentos(List<String> empreendimentos)
		{
			_da = new PontoEmpreendimentoDa(strBancoDeDadosGeo: UsuarioGeo);
			return _da.listarEmpreendimentos(empreendimentos);
		}
		public List<PontoEmpreendimento> listarEmpreendimentos(String empreendimento, String pessoa, String processo, String segmento, String municipio, String atividade)
		{
			_da = new PontoEmpreendimentoDa(strBancoDeDadosGeo: UsuarioGeo);
			return _da.listarEmpreendimentos(empreendimento, pessoa, processo, segmento, municipio, atividade);
		}
	}
}
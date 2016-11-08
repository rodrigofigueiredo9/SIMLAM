using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloAtividade.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloAtividade.Business
{
	public class AtividadeConfiguracaoInternoBus
	{
		#region Propriedades

		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys;
		AtividadeConfiguracaoInternoDa _da;

		public String UsuarioInterno
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioInterno); }
		}

		#endregion

		public AtividadeConfiguracaoInternoBus()
		{
			_configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
			_da = new AtividadeConfiguracaoInternoDa(UsuarioInterno);
		}

		public bool AtividadeConfigurada(int atividadeId, int modeloId)
		{
			return _da.AtividadeConfigurada(atividadeId, modeloId);
		}

		public List<TituloModeloLst> ObterModelosAtividades(List<AtividadeSolicitada> atividades, bool renovacao = false)
		{
			return _da.ObterModelosAtividades(atividades, renovacao);
		}

		public List<TituloModeloLst> ObterModelosSolicitadoExterno()
		{
			try
			{
				return _da.ObterModelosSolicitadoExterno();
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public Resultados<AtividadeConfiguracao> Filtrar(ListarConfigurarcaoFiltro filtrosListar, Paginacao paginacao)
		{
			try
			{
				Filtro<ListarConfigurarcaoFiltro> filtro = new Filtro<ListarConfigurarcaoFiltro>(filtrosListar, paginacao);
				Resultados<AtividadeConfiguracao> resultados = _da.Filtrar(filtro);

				if (resultados.Quantidade < 1)
				{
					Validacao.Add(Mensagem.Padrao.NaoEncontrouRegistros);
				}

				return resultados;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return null;
		}

	}
}
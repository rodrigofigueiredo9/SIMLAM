using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.EtramiteX.WindowsService.Utilitarios;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.Blocos.Data;
using Tecnomapas.EtramiteX.WindowsService.SVCCredenciado.Entities;
using Tecnomapas.EtramiteX.WindowsService.SVCCredenciado.Data;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.EtramiteX.WindowsService.SVCCredenciado.Business
{
	class CredenciadoBus : ServicoTimerBase
	{
		CredenciadoDa _da;
		ConfiguracaoSistema _configSys;

		public CredenciadoBus()
		{
			_configSys = new ConfiguracaoSistema();
			_da = new CredenciadoDa(_configSys.UsuarioCredenciado);
		}

		public void Teste()
		{
			Executar();
		}

		protected override void Executar()
		{
			try
			{
				List<ConfiguracaoServico> configuracoes = _da.Configuracoes(eServico.Todos);

				Executor.Current = new Executor()
				{
					Id = (int)eExecutorId.CredenciadoServico,
					Login = "SVCCredenciado",
					Nome = GetType().Assembly.ManifestModule.Name,
					Tid = GetType().Assembly.ManifestModule.ModuleVersionId.ToString(),
					Tipo = eExecutorTipo.Credenciado
				};

				GerenciadorTransacao.ObterIDAtual();
				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(_configSys.UsuarioCredenciado))
				{
					bancoDeDados.IniciarTransacao();

					CredenciadoInutilizadoExcluir(configuracoes, bancoDeDados);

					bancoDeDados.Commit();
				}
			}
			catch (Exception exc)
			{
				Log.GerarLog(exc);
			}
		}

		private void CredenciadoInutilizadoExcluir(List<ConfiguracaoServico> configuracoes, BancoDeDados banco)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				ConfiguracaoServico configuracao = configuracoes.SingleOrDefault(x => x.Id == (int)eServico.CredenciadoInutilizadoExcluir) ?? new ConfiguracaoServico();

				try
				{
					DateTime inicio = configuracao.DataInicioExecucao ?? DateTime.MinValue;

					if (configuracao == null || configuracao.Id <= 0 || configuracao.EmExecucao || (DateTime.Now - inicio) < configuracao.Intervalo)
					{
						return;
					}

					//Coloca o serviço em execução
					configuracao.EmExecucao = true;
					_da.EditarConfiguracao(configuracao, bancoDeDados);

					_da.CredenciadoInutilizadoExcluir(bancoDeDados);

					configuracao.EmExecucao = false;
					_da.EditarConfiguracao(configuracao, bancoDeDados);
				}
				catch (Exception exc)
				{
					//finaliza o serviço em execução
					//configuracao.EmExecucao = false;
					//_da.EditarConfiguracao(configuracao, bancoDeDados);
					throw exc;
				}
			}
		}
	}
}
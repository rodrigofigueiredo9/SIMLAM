using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.WindowsService.SVCCARSolicitacao.Data;
using Tecnomapas.EtramiteX.WindowsService.SVCEmissaoCFOCFOC.Entities;
using Tecnomapas.EtramiteX.WindowsService.Utilitarios;

namespace Tecnomapas.EtramiteX.WindowsService.SVCEmissaoCFOCFOC.Business
{
	class EmissaoCFOCFOCBus : ServicoTimerBase
	{
		#region Propriedades

		EmissaoCFOCFOCDa _da;
		ConfiguracaoSistema _configSys;
		GerenciadorConfiguracao<ConfiguracaoCredenciado> _configCredenciado = new GerenciadorConfiguracao<ConfiguracaoCredenciado>(new ConfiguracaoCredenciado());

		public EmissaoCFOCFOCBus()
		{
			_configSys = new ConfiguracaoSistema();
			_da = new EmissaoCFOCFOCDa(_configSys.UsuarioInterno);
		}

		#endregion

		public void Teste()
		{
			Executar();
		}

		protected override void Executar()
		{
			try
			{
				List<ConfiguracaoServico> configuracoes = _da.Configuracoes(eServico.HabilitarEmissaoCFOCFOC);

				Executor.Current = new Executor()
				{
					Id = (int)eExecutorId.EmissaoCFOCFOCServico,
					Login = "SVCEmissaoCFOCFOC",
					Nome = GetType().Assembly.ManifestModule.Name,
					Tid = GetType().Assembly.ManifestModule.ModuleVersionId.ToString(),
					Tipo = eExecutorTipo.Interno
				};

				GerenciadorTransacao.ObterIDAtual();
				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
				{
					bancoDeDados.IniciarTransacao();

					GerenciarSituacaoEmissaoCFOCFOC(configuracoes, bancoDeDados);

					bancoDeDados.Commit();
				}
			}
			catch (Exception exc)
			{
				Log.GerarLog(exc);
			}
		}

		public void GerenciarSituacaoEmissaoCFOCFOC(List<ConfiguracaoServico> configuracoes, BancoDeDados banco)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				ConfiguracaoServico configuracao = configuracoes.SingleOrDefault(x => x.Id == (int)eServico.HabilitarEmissaoCFOCFOC) ?? new ConfiguracaoServico();

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

					List<HabilitarEmissaoCFOCFOC> solicitacoes = _da.ObterEmissoesComValidadeVencida(bancoDeDados);

					if (solicitacoes != null)
					{
						foreach (var item in solicitacoes)
						{
							item.Situacao = (int)eHabilitacaoCFOCFOCSituacao.Inativo;
							item.Observacao = "Atualizado pelo sistema";
							_da.AlterarSituacao(item, bancoDeDados);
						}
					}

					//Para a execução do serviço
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
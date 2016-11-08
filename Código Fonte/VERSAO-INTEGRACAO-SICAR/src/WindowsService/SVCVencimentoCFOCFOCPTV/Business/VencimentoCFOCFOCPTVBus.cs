using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.WindowsService.SVCVencimentoCFOCFOCPTV.Data;
using Tecnomapas.EtramiteX.WindowsService.SVCVencimentoCFOCFOCPTV.Entities;
using Tecnomapas.EtramiteX.WindowsService.Utilitarios;

namespace Tecnomapas.EtramiteX.WindowsService.SVCVencimentoCFOCFOCPTV.Business
{
	class VencimentoCFOCFOCPTVBus : ServicoTimerBase
	{
		#region Propriedades

		VencimentoCFOCFOCPTVDa _da;
		ConfiguracaoSistema _configSys;
		GerenciadorConfiguracao<ConfiguracaoCredenciado> _configCredenciado = new GerenciadorConfiguracao<ConfiguracaoCredenciado>(new ConfiguracaoCredenciado());


		public VencimentoCFOCFOCPTVBus()
		{
			_configSys = new ConfiguracaoSistema();
			_da = new VencimentoCFOCFOCPTVDa(_configSys.UsuarioInterno);
		}

		public Int32 SolicitacaoNumMaxDiasValido
		{
			get { return int.Parse(_configSys.SolicitacaoNumeroMaxDiasValido); }
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
				List<ConfiguracaoServico> configuracoes = _da.Configuracoes(eServico.VencimentoCFOCFOCPTV);

				Executor.Current = new Executor()
				{
					Id = 1/*sistema*/,
					Login = "SVCVencimentoCFOCFOCPTV",
					Nome = GetType().Assembly.ManifestModule.Name,
					Tid = GetType().Assembly.ManifestModule.ModuleVersionId.ToString(),
					Tipo = eExecutorTipo.Interno
				};

				_da.TID = GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
				{
					bancoDeDados.IniciarTransacao();

					GerenciarVencimentoCFOCFOCPTV(configuracoes, bancoDeDados);

					bancoDeDados.Commit();
				}
			}
			catch (Exception exc)
			{
				Log.GerarLog(exc);
			}
		}

		public void GerenciarVencimentoCFOCFOCPTV(List<ConfiguracaoServico> configuracoes, BancoDeDados banco)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				ConfiguracaoServico configuracao = configuracoes.SingleOrDefault(x => x.Id == (int)eServico.VencimentoCFOCFOCPTV) ?? new ConfiguracaoServico();

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

					#region Alterar situação CFO/CFOC/PTV

					var list = _da.ObterVencimentosCFOCFOCPTV(bancoDeDados);
					object tipo = "";
					object id = 0;
					var itemId = 0;

					foreach (var item in list)
					{
						item.TryGetValue("TIPO", out tipo);
						item.TryGetValue("ID", out id);
						itemId = Convert.ToInt32(id);

						switch (tipo.ToString())
						{
							case "CFO":

								//GerenciadorTransacao.ObterIDAtual();
								//Não sei se precisa disso

								using (var conDB = BancoDeDados.ObterInstancia(_da.UsuarioCredenciado))
								{
									conDB.IniciarTransacao();

									_da.AlterarSituacaoCFO(itemId, conDB);

									conDB.Commit();
								}

								break;

							case "CFOC":

								//GerenciadorTransacao.ObterIDAtual();
								//Não sei se precisa disso

								using (var conDB = BancoDeDados.ObterInstancia(_da.UsuarioCredenciado))
								{
									conDB.IniciarTransacao();

									_da.AlterarSituacaoCFOC(itemId, conDB);

									conDB.Commit();
								}

								break;

							case "PTV":

								_da.AlterarSituacaoPTV(itemId, bancoDeDados);

								break;

							case "PTV_UF":

								_da.AlterarSituacaoPTVOutroUF(itemId, bancoDeDados);

								break;
						}
					}

					#endregion

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
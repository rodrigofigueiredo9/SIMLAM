using Tecnomapas.EtramiteX.WindowsService.SVCCARSolicitacao.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloCadastroAmbientalRural;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.WindowsService.SVCCARSolicitacao.Entities;
using Tecnomapas.EtramiteX.WindowsService.Utilitarios;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;

namespace Tecnomapas.EtramiteX.WindowsService.SVCCARSolicitacao.Business
{
	class CARSolicitacaoBus : ServicoTimerBase
	{
		#region Propriedades

		CARSolicitacaoDa _da;
		ConfiguracaoSistema _configSys;
		ConsultaCredenciado _consultaCredenciado = null;

		internal ConsultaCredenciado ConsultaCredenciado { get { return _consultaCredenciado; } }

		public CARSolicitacaoBus()
		{
			_configSys = new ConfiguracaoSistema();
            _da = new CARSolicitacaoDa(_configSys.UsuarioCredenciado);
			_consultaCredenciado = new ConsultaCredenciado();
		}

		public Int32 SolicitacaoNumMaxDiasValido
		{
			get { return int.Parse(_configSys.SolicitacaoNumeroMaxDiasValido); }
		}

		public String SolicitacaoMotivoSuspensaoPeriodoProtocolo
		{
			get { return _configSys.SolicitacaoMotivoSuspensaoPeriodoProtocolo; }
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
				List<ConfiguracaoServico> configuracoes = _da.Configuracoes(eServico.CARSolicitacao);

				Executor.Current = new Executor()
				{
					Id = (int)eExecutorId.CARSolicitacaoServico,
					Login = "SVCCARSolicitacao",
					Nome = GetType().Assembly.ManifestModule.Name,
					Tid = GetType().Assembly.ManifestModule.ModuleVersionId.ToString(),
					Tipo = eExecutorTipo.Credenciado
				};

				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(_configSys.UsuarioCredenciado))
				{
					bancoDeDados.IniciarTransacao();

					GerenciarSituacaoCARSolicitacao(configuracoes, bancoDeDados);

					bancoDeDados.Commit();
				}
			}
			catch (Exception exc)
			{
				Log.GerarLog(exc);
			}
		}

		public void GerenciarSituacaoCARSolicitacao(List<ConfiguracaoServico> configuracoes, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				ConfiguracaoServico configuracao = configuracoes.SingleOrDefault(x => x.Id == (int)eServico.CARSolicitacao) ?? new ConfiguracaoServico();

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

					List<CARSolicitacao> solicitacoes = _da.ObterSolicitacoesValidasPorNumDiasPassados(SolicitacaoNumMaxDiasValido, bancoDeDados);
					solicitacoes = _da.ObterSolicitacoesPorRequerimentoNaoProtocolado(solicitacoes, bancoDeDados);

					if (solicitacoes != null)
					{
						for (int i = 0; i < solicitacoes.Count; i++)
						{
							solicitacoes[i].SituacaoId = (int)eCARSolicitacaoSituacao.Suspenso;
							solicitacoes[i].Motivo = SolicitacaoMotivoSuspensaoPeriodoProtocolo;
							_da.AlterarSituacao(solicitacoes[i], bancoDeDados);

							ConsultaCredenciado.Gerar(solicitacoes[i].Id, eHistoricoArtefato.carsolicitacao, bancoDeDados);
						}
					}

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
using System;
using System.Collections.Generic;
using System.Threading;
using Tecnomapas.Blocos.Data;
using Tecnomapas.EtramiteX.WindowsService.ProcessControl;
using Tecnomapas.EtramiteX.WindowsService.Utilitarios;

namespace Tecnomapas.EtramiteX.WindowsService.SVCOperacoesGeo
{
	public class TimerOperacoesGeo : ServiceTimerBase
	{
		private string strMutexServico = "MutexServicoOperacoesGeo";
		private string strMutexProcesso = "MutexProcessoOperacoesGeo";

		private ConfigurationParams _config;

		private List<Processo> fila { get; set; }

		public override void Inicializar(int timeout)
		{
			throw new Exception("Utilize a chamada passando ConfigurationParams");
		}

		public void Inicializar(ConfigurationParams config)
		{
			_config = config;

			fila = new List<Processo>();
			for (int i = 0; i < _config.queuesInfo.Count; i++)
			{
				fila.Add(null);
			}

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				DaOperacoesGeo dataAccess = new DaOperacoesGeo(bancoDeDados);
				dataAccess.InvalidarExecucoes();
			}

			base.Inicializar(_config.timerInterval * 1000);
		}

		protected override void Executar()
		{
			try
			{
				DaOperacoesGeo dataAccess = null;
				Processo processo;
				string mutexServico;
				string mutexProcesso;
				Ticket ticket;

				// Alocando novos Processos
				int count = fila.Count;
				for (int i = 0; i < count; i++)
				{
					if (fila[i] == null || !fila[i].IsExecutando)
					{
						//slot livre

						ticket = null;
						using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
						{
							dataAccess = new DaOperacoesGeo(bancoDeDados);
							ticket = dataAccess.ReservarTicket(_config.queuesInfo[i]);
						}

						if (ticket == null)
							break;

						mutexServico = strMutexServico + i;
						mutexProcesso = strMutexProcesso + i;
						processo = new Processo(@"ProcessOperacoesGeo.exe", ticket.id + "," + ticket.tipo + "," + ticket.etapa + "," + mutexServico + "," + mutexProcesso, mutexServico, mutexProcesso);

						processo.TempoLimiteExecucao = _config.processMaxDuration * 1000;

						fila[i] = processo;

						new Thread(new ThreadStart(() =>
						{
							processo.AguardarExecutar();
						})).Start();
					}
				}
			}
			catch (Exception exc)
			{
				Log.GerarLog(exc);
			}
		}

		protected override void Encerrar()
		{
			// Encerrando Processos Abertos
			if (fila != null)
			{
				for (int i = fila.Count - 1; i >= 0; i--)
				{
					if (fila[i] != null && !fila[i].IsExecutando)
						fila[i].Encerrar(15);
				}
			}
		}
	}
}
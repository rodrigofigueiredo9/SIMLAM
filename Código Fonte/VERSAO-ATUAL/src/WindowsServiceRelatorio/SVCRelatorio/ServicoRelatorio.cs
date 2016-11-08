using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;
using Tecnomapas.EtramiteX.WindowsService.Relatorio.Model.Data;
using Tecnomapas.EtramiteX.WindowsService.Relatorio.Model.Entities;
using Tecnomapas.EtramiteX.WindowsService.Utilitarios;

namespace Tecnomapas.EtramiteX.WindowsService.SVCRelatorio
{
	partial class ServicoRelatorio : ServiceBase
	{
		#region Propriedades

		List<ConfiguracaoRelatorio> _configuracoes;
		Int32 _slotsMaximoDisponiveis;
		Timer _timer;
		Process[] _processos;
		RelatorioDa _da;

		#endregion

		public ServicoRelatorio()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			InicializarServico();
		}

		protected override void OnStop()
		{
			DestruirServico();
		}

		void Tick(object obj)
		{
			try
			{
				int slotsDisponiveis = LiberarProcessosFinalizados();

				if (slotsDisponiveis == 0)
				{
					return;
				}

				_configuracoes = _da.Configuracoes(eFato.Todos);

				List<ConfiguracaoRelatorio> eleitos = EncontrarEleitos();

				if (eleitos.Count == 0)
				{
					return;
				}

				ConfiguracaoRelatorio eleito = Eleger(eleitos);

				IniciarProcesso(eleito);

				//Tick(obj);
			}
			catch (Exception exc)
			{
				Log.GerarLog(exc);
			}
		}

		private void IniciarProcesso(ConfiguracaoRelatorio eleito)
		{
			Process processo = new Process();
			processo.StartInfo = new ProcessStartInfo();
			processo.StartInfo.FileName = eleito.Processo;
			processo.StartInfo.UseShellExecute = false;
			processo.StartInfo.CreateNoWindow = true;

			try
			{
				for (int i = 0; i < _processos.Length; i++)
				{
					if (_processos[i] == null)
					{
						processo.Start();
						_processos[i] = processo;
						break;
					}
				}
			}
			catch (Exception exc)
			{
				Log.GerarLog(exc);
			}
		}

		private ConfiguracaoRelatorio Eleger(List<ConfiguracaoRelatorio> eleitos)
		{
			ConfiguracaoRelatorio eleito = null;
			TimeSpan menor = TimeSpan.MaxValue;

			foreach (var c in eleitos)
			{
				if (c.DadosAte == null)
				{
					throw new Exception(c.Fato + " - DadosAte não pode ser nula.");
				}

				if ((DateTime.Now - c.DadosAte) < menor)
				{
					eleito = c;
				}
			}

			return eleito;
		}

		private List<ConfiguracaoRelatorio> EncontrarEleitos()
		{
			List<ConfiguracaoRelatorio> retorno = new List<ConfiguracaoRelatorio>();

			foreach (var conf in _configuracoes)
			{
				if (conf.EmExecucao || conf.Erro)
				{
					continue;
				}

				if ((DateTime.Now - conf.DadosAte) < conf.Intervalo)
				{
					continue;
				}

				retorno.Add(conf);
			}

			return retorno;
		}

		private int LiberarProcessosFinalizados()
		{
			int i = 0;
			int disponiveis = 0;
			try
			{
				for (i = 0; i < _processos.Length; i++)
				{
					if (_processos[i] == null)
					{
						disponiveis++;
						continue;
					}

					if (_processos[i].HasExited)
					{
						_processos[i].Dispose();
						_processos[i] = null;
						disponiveis++;
					}
				}
			}
			catch (Exception exc)
			{
				Log.GerarLog(exc);

				_processos[i] = null;
				disponiveis++;
			}

			return disponiveis;
		}

		public void InicializarServico()
		{
			_da = new RelatorioDa();

			_slotsMaximoDisponiveis = 1;

			_processos = new Process[_slotsMaximoDisponiveis];

			TimerCallback callback = new TimerCallback(Tick);
			_timer = new Timer(callback, null, 0, 60000);
		}

		private void DestruirServico()
		{
			_timer.Dispose();
		}
	}
}
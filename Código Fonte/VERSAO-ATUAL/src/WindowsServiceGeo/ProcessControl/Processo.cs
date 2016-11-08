using System.Diagnostics;
using System.Threading;

namespace Tecnomapas.EtramiteX.WindowsService.ProcessControl
{
	public class Processo
	{
		private Process process;
		private Timer endTimer;
		//private Semaphore mutexServico;
		//private Semaphore mutexProcesso;

		public bool IsExecutando { get; private set; }
		public int TempoLimiteExecucao { get; set; }

		public Processo(string nomeArquivo, string parametros, string mutexServico, string mutexProcesso)
		{
			//this.mutexServico = new Semaphore(1, 1, mutexServico);
			//this.mutexServico.WaitOne();
			//this.mutexProcesso = new Semaphore(1, 1, mutexProcesso);
			this.process = new Process();
			this.process.StartInfo = new ProcessStartInfo();
			this.process.StartInfo.FileName = nomeArquivo;
			this.process.StartInfo.UseShellExecute = false;
			this.process.StartInfo.CreateNoWindow = true;
			this.process.StartInfo.Arguments = parametros;
		}

		public void IniciarExecutar()
		{
			if (IsExecutando) return;
			process.Start();
			IsExecutando = true;
		}

		public void AguardarExecutar()
		{
			AguardarExecutar(TempoLimiteExecucao);
		}

		public void AguardarExecutar(int milliseconds)
		{
			if (IsExecutando) return;

			process.Start();

			IsExecutando = true;

			if (milliseconds <= 0)
			{
				process.WaitForExit();
			}
			else
			{
				process.WaitForExit(milliseconds);
			}

			//mutexServico.Release();

			if (!process.HasExited)
			{
				process.Kill();
			}

			//mutexProcesso.Close();
			process.Dispose();

			IsExecutando = false;
		}

		public void Encerrar()
		{
			if (endTimer != null)
			{
				endTimer.Dispose();
				endTimer = null;
			}

			if (IsExecutando)
			{
				process.Kill();
				process.Dispose();
				IsExecutando = false;
			}
		}

		public void Encerrar(int timeout)
		{
			if (IsExecutando && endTimer == null)
			{
				if (timeout <= 0)
				{
					Encerrar();
				}
				else
				{
					TimerCallback callback = new TimerCallback(OnTimer);
					endTimer = new Timer(callback, null, 0, timeout * 1000);
				}
			}
		}

		void OnTimer(object obj)
		{
			Encerrar();
		}
	}
}
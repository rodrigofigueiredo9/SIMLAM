using System.Threading;

namespace Tecnomapas.EtramiteX.WindowsService.Utilitarios
{
	public abstract class ServiceTimerBase
	{
		Timer timer;

		#region Running

		// Flag de execucao
		// true se o servico esta executando alguma atividade

		bool running;
		readonly object lockRunning = new object();

		bool Running
		{
			get
			{
				lock (lockRunning)
				{
					return running;
				}
			}
			set
			{
				lock (lockRunning)
				{
					running = value;
				}
			}
		}

		#endregion

		public virtual void Inicializar(int timeout)
		{
			TimerCallback callback = new TimerCallback(OnTimer);
			timer = new Timer(callback, null, 0, timeout);
		}

		public void Finalizar()
		{
			timer.Dispose();

			Encerrar();
		}

		public void Acordar()
		{
			if (Running) return;

			Running = true;

			Executar();

			Running = false;
		}

		void OnTimer(object obj)
		{
			Acordar();
		}

		protected abstract void Executar();

		protected virtual void Encerrar()
		{
		}
	}
}
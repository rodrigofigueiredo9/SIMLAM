using System.Threading;

namespace Tecnomapas.EtramiteX.WindowsService.ProcessControl
{
	public abstract class ExecucaoProcesso
	{
		//private Semaphore mutexServico;
		//private Semaphore mutexProcesso;

		public ExecucaoProcesso(string mutexServico, string mutexProcesso)
		{
			//this.mutexProcesso = new Semaphore(1, 1, mutexProcesso);
			//this.mutexProcesso.WaitOne();
			//this.mutexServico = new Semaphore(1, 1, mutexServico);
		}

		public void Executar()
		{
			try
			{
				ExecutarProcesso();
			}
			finally
			{
				LiberarRecursos();
			}
		}

		protected abstract void ExecutarProcesso();

		protected bool Encerrar()
		{
			//return mutexServico.WaitOne(500);
			return true;
		}

		void LiberarRecursos()
		{
			//mutexServico.Close();
			//mutexProcesso.Release();
		}
	}
}
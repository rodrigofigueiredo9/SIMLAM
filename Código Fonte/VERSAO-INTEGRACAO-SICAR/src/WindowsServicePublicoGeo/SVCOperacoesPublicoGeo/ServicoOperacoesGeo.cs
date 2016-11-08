using System.ServiceProcess;
using Tecnomapas.Blocos.Data;
using System.Linq;
using System.Collections.Generic;

namespace Tecnomapas.EtramiteX.WindowsService.SVCOperacoesPublicoGeo
{
	public partial class ServicoOperacoesGeo : ServiceBase
	{
		private TimerOperacoesGeo timerOperacoes = new TimerOperacoesGeo();

		public ServicoOperacoesGeo()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{	
			ConfigurationParams config = null;
			List<string> lstCultura = new List<string>();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
                DaOperacoesGeo dataAccess = new DaOperacoesGeo(bancoDeDados);
                config = dataAccess.BuscarConfiguracoes();

				System.Collections.Generic.Dictionary<string, string> dic = dataAccess.ObterParameters();

				foreach (var item in dic)
				{
					lstCultura.Add(string.Format("\n{0}: {1}", item.Key, item.Value));
				}
            }

			Utilitarios.Log.GerarLog(string.Format("Cultura da Thread: {0}\n {1}", 
				System.Threading.Thread.CurrentThread.CurrentCulture.DisplayName, 
				string.Concat(lstCultura)));

			timerOperacoes = new TimerOperacoesGeo();
            timerOperacoes.Inicializar(config);
		}

		protected override void OnStop()
		{
			timerOperacoes.Finalizar();
		}

		protected override void OnCustomCommand(int command)
		{
			// Método para executar um comando remotamente
			// Ver aplicacao Cliente para entender a chamada ao servico
			// O código passado como parametro é customizado pelo cliente

			if (command == 42)
			{
				timerOperacoes.Acordar();
			}
		}

    }
}

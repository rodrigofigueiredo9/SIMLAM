using log4net;
using Quartz;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tecnomapas.EtramiteX.Scheduler.jobs.Class;

namespace Tecnomapas.EtramiteX.Scheduler.jobs
{
	[DisallowConcurrentExecution]
	public class GerarPdfSolicitacaoCarJob : IJob
	{
		private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public void Execute(IJobExecutionContext context)
		{
			Gerar();
		}

		private void Gerar ()
		{
			CARSolicitacaoFunc _busSolicitacaoCar = new CARSolicitacaoFunc();
			List<CARSolicita> solicitacoes = _busSolicitacaoCar.ObterSolicitacaoValida();

			solicitacoes.Where(x => x.Origem == 1).ToList().ForEach(x => _busSolicitacaoCar.GerarPdfSolicitacaoCarInstitucional(x)) ;
			solicitacoes.Where(x => x.Origem == 2).ToList().ForEach(x => _busSolicitacaoCar.GerarPdfSolicitacaoCarCredenciado(x)) ;
		}
	}
}

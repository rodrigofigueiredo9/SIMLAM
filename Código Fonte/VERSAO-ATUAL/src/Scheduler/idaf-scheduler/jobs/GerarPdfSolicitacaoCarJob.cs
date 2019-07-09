using log4net;
using Oracle.ManagedDataAccess.Client;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx;
using Tecnomapas.EtramiteX.Scheduler.jobs.Class;
using Tecnomapas.EtramiteX.Scheduler.models.misc;

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
			GeradorAspose.Autorizacao();
			CARSolicitacaoFunc _busSolicitacaoCar = new CARSolicitacaoFunc();
			List<CARSolicita> solicitacoes = _busSolicitacaoCar.ObterSolicitacaoValida();

			solicitacoes.Where(x => x.Origem == 1).ToList().ForEach(x => _busSolicitacaoCar.GerarPdfSolicitacaoCarInstitucional(x)) ;
			solicitacoes.Where(x => x.Origem == 2).ToList().ForEach(x => _busSolicitacaoCar.GerarPdfSolicitacaoCarCredenciado(x)) ;
		}
	}
}

using GeoJSON.Net.Geometry;
using Ionic.Zip;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Oracle.ManagedDataAccess.Client;
using Quartz;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Data;
using System.Threading.Tasks;
using Tecnomapas.EtramiteX.Scheduler.misc;
using Tecnomapas.EtramiteX.Scheduler.misc.WKT;
using Tecnomapas.EtramiteX.Scheduler.models;
using Tecnomapas.EtramiteX.Scheduler.models.misc;
using Tecnomapas.EtramiteX.Scheduler.models.simlam;

namespace Tecnomapas.EtramiteX.Scheduler.jobs
{
	[DisallowConcurrentExecution]
	public class ReenvioCarJob : IJob
	{
		private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public void Execute(IJobExecutionContext context)
		{
			try
			{
				Log.Info($"COMECOU O REENVIO - {DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss.fff")}");
				using (var conn = new OracleConnection(CarUtils.GetBancoInstitucional()))
				{
					conn.Open();
					using (var cmd = new OracleCommand(@"
						UPDATE TAB_CAR_SOLICITACAO CS SET PASSIVO_ENVIADO = NULL
						WHERE CS.ID IN (
								SELECT SOLICITACAO_CAR FROM TAB_CONTROLE_SICAR SI 
									WHERE SI.MENSAGEM_RESPOSTA LIKE '%sending the request%' AND SOLICITACAO_CAR_ESQUEMA = 1 AND SITUACAO_ENVIO = 4
								UNION
								SELECT SOLICITACAO_CAR FROM TAB_CONTROLE_SICAR SI
									WHERE SI.MENSAGEM_RESPOSTA LIKE '%Unexpected character%' AND SOLICITACAO_CAR_ESQUEMA = 1 AND SITUACAO_ENVIO = 4
								UNION 
								SELECT SOLICITACAO_CAR FROM TAB_CONTROLE_SICAR SI
									WHERE SI.MENSAGEM_RESPOSTA LIKE '%A task was canceled%' AND SOLICITACAO_CAR_ESQUEMA = 1 AND SITUACAO_ENVIO = 4                  
								)", conn))
					{
						cmd.ExecuteNonQuery();

						cmd.CommandText = @"
						UPDATE IDAFCREDENCIADO.TAB_CAR_SOLICITACAO CS SET PASSIVO_ENVIADO = NULL
						WHERE CS.ID IN (
								SELECT SOLICITACAO_CAR FROM TAB_CONTROLE_SICAR SI 
									WHERE SI.MENSAGEM_RESPOSTA LIKE '%sending the request%' AND SOLICITACAO_CAR_ESQUEMA = 1 AND SITUACAO_ENVIO = 4
								UNION
								SELECT SOLICITACAO_CAR FROM TAB_CONTROLE_SICAR SI
									WHERE SI.MENSAGEM_RESPOSTA LIKE '%Unexpected character%' AND SOLICITACAO_CAR_ESQUEMA = 1 AND SITUACAO_ENVIO = 4
								UNION 
								SELECT SOLICITACAO_CAR FROM TAB_CONTROLE_SICAR SI
									WHERE SI.MENSAGEM_RESPOSTA LIKE '%A task was canceled%' AND SOLICITACAO_CAR_ESQUEMA = 1 AND SITUACAO_ENVIO = 4                  
								)         ";
						cmd.ExecuteNonQuery();
					}
					conn.Close();
				}
				Log.Info($"TERMINOU O REENVIO - {DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss.fff")}");
			}
			catch (Exception ex)
			{
				Log.Error($@"CATCH REENVIO: \n
							Mensagem: {ex.Message} \n
							___ex____: {ex} ");
			}
		}
	}
}

using log4net;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using Quartz;
using System;
using System.Linq;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Tecnomapas.EtramiteX.Scheduler.misc;
using Tecnomapas.EtramiteX.Scheduler.models.misc;
using System.Collections.Generic;
using System.Threading;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace Tecnomapas.EtramiteX.Scheduler.jobs
{
	[DisallowConcurrentExecution]
	public class EnviarArquivoCarJob : IJob
	{
		private static HttpClientHandler _httpClientHandler = new HttpClientHandler()
		{
			PreAuthenticate = true,
			UseDefaultCredentials = false,
		};
		private static HttpClient _client;
		private static int count = 1;

		private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		/// <summary>
		/// Called by the <see cref="T:Quartz.IScheduler" /> when a <see cref="T:Quartz.ITrigger" />
		/// fires that is associated with the <see cref="T:Quartz.IJob" />.
		/// </summary>
		/// <param name="context">The execution context.</param>
		/// <remarks>
		/// The implementation may wish to set a  result object on the
		/// JobExecutionContext before this method exits.  The result itself
		/// is meaningless to Quartz, but may be informative to
		/// <see cref="T:Quartz.IJobListener" />s or
		/// <see cref="T:Quartz.ITriggerListener" />s that are watching the job's
		/// execution.
		/// </remarks>
		public async void Execute(IJobExecutionContext context)
		{
			//logging
			var jobKey = context.JobDetail.Key;
			Log.InfoFormat("BEGIN {0} executing at {1}", jobKey, DateTime.Now.ToString("r"));

			using (var conn = new OracleConnection(CarUtils.GetBancoInstitucional()))
			{
				conn.Open();
				string tid = string.Empty;

				var pathArquivoTemporario = new ArquivoManager().BuscarDiretorioArquivoTemporario(conn);

				if (!pathArquivoTemporario.EndsWith("\\"))
					pathArquivoTemporario += "\\";
				pathArquivoTemporario += "SICAR\\";

				//Veja se 
				var nextItem = LocalDB.PegarProximoItemFila(conn, "enviar-car");

				while (nextItem != null)
				{
					//Update item as Started
					LocalDB.MarcarItemFilaIniciado(conn, nextItem.Id);

					var item = LocalDB.PegarItemFilaPorId(conn, nextItem.Requisitante);


					if (String.IsNullOrEmpty(item.Requisicao))
					{
						nextItem = LocalDB.PegarProximoItemFila(conn, "enviar-car");
						continue;
					}
					var requisicao = JsonConvert.DeserializeObject<RequisicaoJobCar>(item.Requisicao);
					tid = Blocos.Data.GerenciadorTransacao.ObterIDAtual();

					if (ControleCarDB.VerificarCarValido(conn, requisicao.solicitacao_car))
					{
						nextItem = LocalDB.PegarProximoItemFila(conn, "enviar-car");
						continue;
					}

					string resultado = "";
					try
					{
						//Atualizar controle de envio do SICAR
						ControleCarDB.AtualizarSolicitacaoCar(conn, requisicao.origem, requisicao.solicitacao_car, ControleCarDB.SITUACAO_ENVIO_AGUARDANDO_ENVIO, tid);
						ControleCarDB.AtualizarControleSICAR(conn, null, requisicao, ControleCarDB.SITUACAO_ENVIO_ENVIANDO, tid);
						//var controleCar = ControleCarDB.ObterItemControleCar(conn, requisicao);

						var dataCadastroEstadual = ControleCarDB.ObterDataSolicitacao(conn, requisicao.solicitacao_car, requisicao.origem);

						resultado = await EnviarArquivoCAR(pathArquivoTemporario + nextItem.Requisicao, dataCadastroEstadual, requisicao.solicitacao_car);
						if (String.IsNullOrWhiteSpace(resultado) || resultado.Contains("task was canceled"))
						{
							throw new System.ArgumentException("Erro de conexão com o SICAR, será feita uma nova tentativa ;", "resultado");
						}
						var resultadoEnvio = JsonConvert.DeserializeObject<MensagemRetorno>(resultado);


						//Salvar no diretorio de arquivos do SIMLAM Institucional
						string arquivoFinal;
						using (var stream = File.Open(pathArquivoTemporario + nextItem.Requisicao, FileMode.Open, FileAccess.Read, FileShare.Read))
						{
							var arquivoManager = new ArquivoManager();
							arquivoFinal = arquivoManager.Salvar(nextItem.Requisicao, stream, conn);
						}

						//Retificação
						ItemControleCar itemSicar = ControleCarDB.ObterItemControleCarRetificacao(conn, requisicao);

						if (itemSicar != null)
						{
							if (itemSicar.solicitacao_car_anterior > 0 && resultadoEnvio.codigoResposta == MensagemRetorno.CodigoRespostaSucesso)
							{
								ControleCarDB.AtualizarSolicitacaoCarRetificacao(conn, itemSicar.solicitacao_car_anterior_esquema, itemSicar.solicitacao_car_anterior, itemSicar.solicitacao_car_anterior_tid);
								ControleCarDB.AtualizarControleSICarRetificacao(conn, resultadoEnvio, itemSicar, ControleCarDB.SITUACAO_ENVIO_ARQUIVO_ENTREGUE, requisicao.solicitacao_car, tid, arquivoFinal);
							}
						}
						//Atualiza a Solicitacao do CAR
						var situacaoSolicitacao = (resultadoEnvio.codigoResposta == MensagemRetorno.CodigoRespostaSucesso) ? ControleCarDB.SITUACAO_SOLICITACAO_VALIDO : ControleCarDB.SITUACAO_SOLICITACAO_PENDENTE;
						ControleCarDB.AtualizarSolicitacaoCar(conn, requisicao, situacaoSolicitacao, tid);

						//Atualizar controle de envio do SICAR
						ControleCarDB.AtualizarControleSICAR(conn, resultadoEnvio, requisicao, ControleCarDB.SITUACAO_ENVIO_ARQUIVO_ENTREGUE, tid, arquivoCar: arquivoFinal);
						//}

						//Marcar como processado
						LocalDB.MarcarItemFilaTerminado(conn, nextItem.Id, true, resultado);
					}
					catch (Exception ex)
					{
						//Marcar como processado registrando a mensagem de erro
						var msg = " +++---+++ RESULTADO  :::: " +
							(resultado ?? "noMessage ++++   00----000  " + ex.Message +
							Environment.NewLine +
							Environment.NewLine);

						LocalDB.MarcarItemFilaTerminado(conn, nextItem.Id, false, msg);
						ControleCarDB.AtualizarSolicitacaoCar(conn, requisicao.origem, requisicao.solicitacao_car, ControleCarDB.SITUACAO_SOLICITACAO_PENDENTE, tid);
						ControleCarDB.AtualizarControleSICAR(conn, new MensagemRetorno() { mensagensResposta = new List<string> { ex.Message, ex.ToString(), resultado ?? "" } }, requisicao, ControleCarDB.SITUACAO_ENVIO_ARQUIVO_REPROVADO, tid, catchEnviar: true);
						Log.Error("CATCH:" + nextItem.Requisicao + " ===== ++  " + ex.Message, ex);
					}

					System.Threading.Thread.Sleep(TimeSpan.FromSeconds(30));

					nextItem = LocalDB.PegarProximoItemFila(conn, "enviar-car");

					//Apagar arquivo do diretorio temporário
					//TODO:NÃO VAI 
					//try
					//{
					//	File.Delete(pathArquivoTemporario + nextItem.Requisicao);
					//}
					//catch (Exception) { /*ignored*/ }
				}

				//using (var cmd = new OracleCommand(@"UPDATE IDAF.TAB_SCHEDULER_FILA SET DATA_CRIACAO = null
				//							WHERE resultado like '%Não está na hora especificada para o sincronismo do seu sistema. %'", conn))
				//{
				//	cmd.ExecuteNonQuery();
				//}
				//using (var cmd = new OracleCommand(@"UPDATE IDAF.TAB_SCHEDULER_FILA SET DATA_CRIACAO = null
				//							WHERE resultado like '%Value cannot be null.%'", conn))
				//{
				//	cmd.ExecuteNonQuery();
				//}
				//using (var cmd = new OracleCommand(@"UPDATE IDAF.TAB_SCHEDULER_FILA SET DATA_CRIACAO = null
				//							WHERE resultado like '%TCP%'", conn))
				//{
				//	cmd.ExecuteNonQuery();
				//}
				//using (var cmd = new OracleCommand(@"UPDATE IDAF.TAB_SCHEDULER_FILA SET DATA_CRIACAO = null
				//							WHERE resultado like '%Object reference%' AND TIPO = 'enviar-car'", conn))
				//{
				//	cmd.ExecuteNonQuery();
				//}
				//using (var cmd = new OracleCommand(@"UPDATE IDAF.TAB_SCHEDULER_FILA SET DATA_CRIACAO = null
				//							WHERE resultado like '%Houve um problema%'", conn))
				//{
				//	cmd.ExecuteNonQuery();
				//}
				//using (var cmd = new OracleCommand(@"UPDATE IDAF.TAB_SCHEDULER_FILA SET DATA_CRIACAO = null
				//							WHERE resultado like '%Erro de conexão com o SICAR%'", conn))
				//{
				//	cmd.ExecuteNonQuery();
				//}

				conn.Close();
			}



			Log.InfoFormat("ENDING {0} executing at {1}", jobKey, DateTime.Now.ToString("r"));
		}
		/// <summary>
		/// Enviars the arquivo car.
		/// </summary>
		/// <param name="localArquivoCar">The local arquivo car.</param>
		/// <returns></returns>
		private async Task<String> EnviarArquivoCAR(string localArquivoCar, string dataCadastroEstadual, int solicitacao)
		{
			var proxyUrl = ConfigurationManager.AppSettings["ProxyUrl"];
			if (!String.IsNullOrWhiteSpace(proxyUrl))
			{
				_httpClientHandler.Proxy = new WebProxy(proxyUrl, false)
				{
					UseDefaultCredentials = false,
					Credentials = CredentialCache.DefaultCredentials
				};
			}

			Log.Info($"Verifica se httpClient já foi instanciado - {DateTime.Now }");

			//verifica se objeto já foi instanciado
			var sicarUrl = ConfigurationManager.AppSettings["SicarUrl"];
			Log.Info($"URL::: {sicarUrl} \n\n ---");
			if (_client == null)
			{
				Log.Info($"Instanciando HTTP Client N.º {count} - {DateTime.Now} ");
				_client = new HttpClient(_httpClientHandler) { BaseAddress = new Uri(sicarUrl) };
				_client.Timeout = TimeSpan.FromMinutes(20);
				_client.DefaultRequestHeaders.Clear();
				_client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Mozilla", "5.0"));

				count++;
			}

			try
			{
				Log.Info($"Preparando para Enviar o Arquivo - {DateTime.Now}");
				using (var fs = File.OpenRead(localArquivoCar))
				{
					using (var streamContent = new StreamContent(fs))
					{
						var imageContent = new ByteArrayContent(await streamContent.ReadAsByteArrayAsync());
						imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");

						Log.Info($"Iniciando DataContent - {DateTime.Now}");
						using (var form = new MultipartFormDataContent())
						{
							Log.Info($"Token: {ConfigurationManager.AppSettings["SicarToken"]} - {DateTime.Now}");
							form.Headers.Add("token", ConfigurationManager.AppSettings["SicarToken"]);

							Log.Info($"Nome do Arquivo: {localArquivoCar} - {DateTime.Now}");
							form.Add(imageContent, "car", Path.GetFileName(localArquivoCar));

							Log.Info($"DataCadastroEstadual: {dataCadastroEstadual} - {dataCadastroEstadual}");
							form.Add(new StringContent(dataCadastroEstadual), "dataCadastroEstadual");

							Log.Info($"Iniciando enviado de Arquivo - {DateTime.Now}");
							var response = await _client.PostAsync("/sincronia/quick", form);
							Log.Info($"Concluído envio do Arquivo - {DateTime.Now}");


							Log.Info($"Verificando se envio foi realizado com sucesso - {DateTime.Now}");
							if (response.IsSuccessStatusCode)
							{
								var content = await response.Content.ReadAsStringAsync();
								var jsonFormat = JToken.Parse(content).ToString();
								Log.Info($"RETORNO ENVIO: {jsonFormat}");

								return jsonFormat;
							}

							Log.Error("ERRO RESPONSE: " + response);
							throw new ArgumentException("Erro de conexão com o SICAR, será feita uma nova tentativa ;", "resultado");
						}
					}
				}
			}
			catch (Exception ex)
			{
				Log.Error("*************************************************************************************************************************");
				Log.Error("*************************************************************************************************************************");
				Log.Error($"Ocorreu um erro ao tentar enviar Arquivo CAR - {DateTime.Now}");
				Log.Error($"Arquivo que gerou o Erro: {localArquivoCar }");
				Log.Error($"Error Message: {ex.Message}");
				Log.Error($"Error InnerException: {ex.InnerException?.ToString()}");
				Log.Error($"ERROR FULL: {ex.ToString()}");
				Log.Error("*************************************************************************************************************************");
				Log.Error("*************************************************************************************************************************");
				return ex.Message;
			}
		}
	}
}
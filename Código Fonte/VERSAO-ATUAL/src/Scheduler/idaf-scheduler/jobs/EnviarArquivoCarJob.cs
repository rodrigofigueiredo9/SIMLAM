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

namespace Tecnomapas.EtramiteX.Scheduler.jobs
{
	[DisallowConcurrentExecution]
	public class EnviarArquivoCarJob : IJob
	{
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
					//LocalDB.MarcarItemFilaIniciado(conn, nextItem.Id);

					var item = LocalDB.PegarItemFilaPorId(conn, nextItem.Requisitante);

                    if (item.Requisicao == null)
                    {
                        nextItem = LocalDB.PegarProximoItemFila(conn, "enviar-car");
                        continue;
                    }

					var requisicao = JsonConvert.DeserializeObject<RequisicaoJobCar>(item.Requisicao);
					tid = Blocos.Data.GerenciadorTransacao.ObterIDAtual();

					string resultado = "";
					try
					{
						//Atualizar controle de envio do SICAR
                        ControleCarDB.AtualizarSolicitacaoCar(conn, requisicao.origem, requisicao.solicitacao_car, ControleCarDB.SITUACAO_ENVIO_AGUARDANDO_ENVIO, tid);
						ControleCarDB.AtualizarControleSICAR(conn, null, requisicao, ControleCarDB.SITUACAO_ENVIO_ENVIANDO, tid);
						var controleCar = ControleCarDB.ObterItemControleCar(conn, requisicao);

						var dataCadastroEstadual = ControleCarDB.ObterDataSolicitacao(conn, requisicao.solicitacao_car, requisicao.origem);

						resultado = await EnviarArquivoCAR(pathArquivoTemporario + nextItem.Requisicao, dataCadastroEstadual);
						var resultadoEnvio = JsonConvert.DeserializeObject<MensagemRetorno>(resultado);

						if (resultadoEnvio.codigoResposta == MensagemRetorno.CodigoRespostaErro)
						{
							resultado = await EnviarArquivoCAR(pathArquivoTemporario + nextItem.Requisicao, dataCadastroEstadual);
							if(String.IsNullOrWhiteSpace(resultado))
							{
								throw new System.ArgumentException("Resultado do SICAR is null", "resultado");
							}
							resultadoEnvio = JsonConvert.DeserializeObject<MensagemRetorno>(resultado);
						}
                        //resultadoEnvio.codigoResposta = 200;
                        
						//Salvar no diretorio de arquivos do SIMLAM Institucional
						string arquivoFinal;
						using (var stream = File.Open(pathArquivoTemporario + nextItem.Requisicao, FileMode.Open, FileAccess.Read, FileShare.Read))
						{
							var arquivoManager = new ArquivoManager();
							arquivoFinal = arquivoManager.Salvar(nextItem.Requisicao, stream, conn);
                        }

       

                        /*if (resultadoEnvio.codigoResposta == MensagemRetorno.CodigoRespostaErro
							|| (resultadoEnvio.codigoResposta == MensagemRetorno.CodigoRespostaInconformidade
								&& resultadoEnvio.mensagensResposta.Any( r=> r.Equals("Foi encontrada sobreposição de 100,00% com outro imóvel já inscrito no CAR que possui os mesmos documentos (CPF e/ou CNPJ).", StringComparison.CurrentCultureIgnoreCase))))
						{
							LocalDB.AdicionarItemFila(conn, "revisar-resposta-car", item.Id, nextItem.Requisicao.Substring(0, nextItem.Requisicao.Length - 4), requisicao.empreendimento);
						}
						else
						{*/

						//Retificação
                        ItemControleCar itemSicar = ControleCarDB.ObterItemControleCarRetificacao(conn, requisicao);

                        if(itemSicar != null)
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
						var msg = ex.Message +
							Environment.NewLine +
							Environment.NewLine +
							resultado;

						LocalDB.MarcarItemFilaTerminado(conn, nextItem.Id, false, msg);
						ControleCarDB.AtualizarSolicitacaoCar(conn, requisicao.origem, requisicao.solicitacao_car, ControleCarDB.SITUACAO_SOLICITACAO_PENDENTE, tid);
						ControleCarDB.AtualizarControleSICAR(conn, new MensagemRetorno() { mensagensResposta = new List<string> { ex.Message, ex.ToString(), resultado } }, requisicao, ControleCarDB.SITUACAO_ENVIO_ARQUIVO_REPROVADO, tid, catchEnviar: true);
						Log.Error("CATCH:" + ex.Message, ex);
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

                 using (var cmd = new OracleCommand(@"UPDATE IDAF.TAB_SCHEDULER_FILA SET DATA_CRIACAO = null
                                                WHERE resultado like '%Não está na hora especificada para o sincronismo do seu sistema. %'", conn))
                    {
                        cmd.ExecuteNonQuery();                            
                    }
                 using (var cmd = new OracleCommand(@"UPDATE IDAF.TAB_SCHEDULER_FILA SET DATA_CRIACAO = null
                                                WHERE resultado like '%Value cannot be null.%'", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                 using (var cmd = new OracleCommand(@"UPDATE IDAF.TAB_SCHEDULER_FILA SET DATA_CRIACAO = null
                                                WHERE resultado like '%TCP%'", conn))
                 {
                     cmd.ExecuteNonQuery();
                 }
                 using (var cmd = new OracleCommand(@"UPDATE IDAF.TAB_SCHEDULER_FILA SET DATA_CRIACAO = null
                                                WHERE resultado like '%Object reference%' AND TIPO = 'enviar-car'", conn))
                 {
                     cmd.ExecuteNonQuery();
                 }
                 using (var cmd = new OracleCommand(@"UPDATE IDAF.TAB_SCHEDULER_FILA SET DATA_CRIACAO = null
                                                WHERE resultado like '%Houve um problema%'", conn))
                 {
                     cmd.ExecuteNonQuery();
                 }

				conn.Close();
			}

		

			Log.InfoFormat("ENDING {0} executing at {1}", jobKey, DateTime.Now.ToString("r"));
		}
		/// <summary>
		/// Enviars the arquivo car.
		/// </summary>
		/// <param name="localArquivoCar">The local arquivo car.</param>
		/// <returns></returns>
		private async Task<String> EnviarArquivoCAR(string localArquivoCar, string dataCadastroEstadual)
		{
			var httpClientHandler = new HttpClientHandler()
			{
				PreAuthenticate = true,
				UseDefaultCredentials = false,
			};

            var proxyUrl = ConfigurationManager.AppSettings["ProxyUrl"];

            if (!String.IsNullOrWhiteSpace(proxyUrl))
            {
                httpClientHandler.Proxy = new WebProxy(proxyUrl, false)
                {
                    UseDefaultCredentials = false,
                    Credentials = CredentialCache.DefaultCredentials
                };
            }

			using (var stream = File.Open(localArquivoCar, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				var fileName = Path.GetFileName(localArquivoCar);

				using (var client = new HttpClient(httpClientHandler))
				{
					var sicarUrl = ConfigurationManager.AppSettings["SicarUrl"];
					client.BaseAddress = new Uri(sicarUrl);
					client.DefaultRequestHeaders.Add("token", ConfigurationManager.AppSettings["SicarToken"]);

                    try
                    {
                        using (var content = new MultipartFormDataContent())
                        {
                            content.Add(new StreamContent(stream), "car", fileName);
                            content.Add(new StringContent(dataCadastroEstadual), "dataCadastroEstadual");
                            var response = await client.PostAsync("/sincronia/quick", content);

                            return await response.Content.ReadAsStringAsync();
                        }
                    }
                    catch (Exception ex) 
                    {
						Log.Error("EnviarArquivoCAR: " + ex.Message, ex);
                        return ex.Message;
					}
				}
			}
		}
	}
}
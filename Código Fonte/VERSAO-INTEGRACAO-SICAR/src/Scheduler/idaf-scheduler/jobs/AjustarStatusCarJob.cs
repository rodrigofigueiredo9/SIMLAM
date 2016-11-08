using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using Quartz;
using Tecnomapas.EtramiteX.Scheduler.misc;
using Tecnomapas.EtramiteX.Scheduler.models.misc;

namespace Tecnomapas.EtramiteX.Scheduler.jobs
{
	[DisallowConcurrentExecution]
	public class AjustarStatusCarJob : IJob
	{
		public async void Execute(IJobExecutionContext context)
		{
			using (var conn = new OracleConnection(CarUtils.GetBancoInstitucional()))
			{
				string strResposta = string.Empty;
				ItemFila nextItem = null;
				var tid = Blocos.Data.GerenciadorTransacao.ObterIDAtual();
				try
				{
					conn.Open();

					nextItem = LocalDB.PegarProximoItemFila(conn, "revisar-resposta-car");

					while (nextItem != null)
					{
						strResposta = await ConsultarStatusSicar(nextItem.Requisicao);

						var resposta = JsonConvert.DeserializeObject<ConsultaSicarResposta>(strResposta);
						
						var resultado = GerarMensagemRetorno(resposta);

						if (resultado.codigoResposta == 500)
						{
							var strResultadoEnviarCar = LocalDB.PegarResultadoItemFilaPorRequisitante(conn, nextItem.Requisitante, "enviar-car");
							var resultadoEnviarCar = JsonConvert.DeserializeObject<MensagemRetorno>(strResultadoEnviarCar);
							if (resultadoEnviarCar.codigoResposta != 500)
							{
								resultado = resultadoEnviarCar;
							}
						}

						AtualizarRegistros(conn, tid, nextItem, resultado);

						nextItem = LocalDB.PegarProximoItemFila(conn, "revisar-resposta-car");
					}
				}
				catch (Exception ex)
				{
					//Marcar como processado registrando a mensagem de erro
					var msg = ex.Message +
						Environment.NewLine +
						Environment.NewLine +
						strResposta;

					if (nextItem != null)
					{
						var item = LocalDB.PegarItemFilaPorId(conn, nextItem.Requisitante);
						var requisicao = JsonConvert.DeserializeObject<RequisicaoJobCar>(item.Requisicao);

					
						ControleCarDB.AtualizarSolicitacaoCar(conn, requisicao.origem, requisicao.solicitacao_car, ControleCarDB.SITUACAO_SOLICITACAO_PENDENTE, tid);
						ControleCarDB.AtualizarControleSICAR(conn, null, requisicao, ControleCarDB.SITUACAO_ENVIO_ARQUIVO_REPROVADO, tid);
					}

					LocalDB.MarcarItemFilaTerminado(conn, nextItem.Id, false, msg);					
				}
			}
		}

		private async Task<string> ConsultarStatusSicar(string protocolo)
		{
			var httpClientHandler = new HttpClientHandler()
			{
				PreAuthenticate = true,
				UseDefaultCredentials = false,
			};

			var proxyUrl = ConfigurationManager.AppSettings["ProxyUrl"];

			if (proxyUrl != string.Empty)
			{
				httpClientHandler.Proxy = new WebProxy(proxyUrl, false)
				{
					UseDefaultCredentials = false,
					Credentials = CredentialCache.DefaultCredentials
				};
			}

			using (var client = new HttpClient())
			{
				var sicarUrl = ConfigurationManager.AppSettings["SicarUrl"];

				client.BaseAddress = new Uri(sicarUrl);
				client.DefaultRequestHeaders.Accept.Clear();
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

				HttpResponseMessage response = await client.GetAsync("/imovel/" + protocolo + "/false");

				if (response.IsSuccessStatusCode)
				{
					var strResposta = await response.Content.ReadAsStringAsync();

					return strResposta;
				}

				return string.Empty;
			}
		}

		private MensagemRetorno GerarMensagemRetorno(ConsultaSicarResposta resposta)
		{
			var resultado = new MensagemRetorno();

			if (resposta.dados == null)
			{
				resultado.codigoResposta = 500;
				resultado.mensagensResposta = new List<string> { "Ocorreu um erro ao processar o arquivo do CAR." };
				resultado.idtImovel = null;
				resultado.diretorioTemp = null;
			}
			else
			{
				var sicarUrl = ConfigurationManager.AppSettings["SicarUrl"];


				resultado.codigoResposta = 200;
				resultado.idtImovel = resposta.dados.id;
				resultado.codigoImovel = resposta.dados.codigoImovel;
				resultado.codigoImovelComMascara = Regex.Replace(resposta.dados.codigoImovel, @"([A-Z]{2}-\d{7}-)([A-Z0-9]{4})([A-Z0-9]{4})([A-Z0-9]{4})([A-Z0-9]{4})([A-Z0-9]{4})([A-Z0-9]{4})([A-Z0-9]{4})([A-Z0-9]{4})", @"${1}${2}.${3}.${4}.${5}.${6}.${7}.${8}.${9}");
				resultado.diretorioTemp = null;
				resultado.statusImovel = resposta.dados.statusImovel;
				resultado.urlReciboInscricao = sicarUrl + "/pdf/" + resposta.dados.codigoImovel + "/gerar";
				resultado.mensagensResposta = new List<string> { "Imóvel inserido com sucesso no banco de dados do Sicar - Sistema Nacional de Cadastro Ambiental Rural." };
				resultado.imoveisImpactados = null;
				resultado.protocoloImovel = resposta.dados.protocolo;
			}

			return resultado;
		}


		private void AtualizarRegistros(OracleConnection conn, string tid, ItemFila itemFila, MensagemRetorno resultadoEnvio)
		{
			string resultado = string.Empty;
			RequisicaoJobCar requisicao = null;
			try
			{
				//LocalDB.MarcarItemFilaIniciado(conn, itemFila.Id);

				resultado = JsonConvert.SerializeObject(resultadoEnvio);
				var item = LocalDB.PegarItemFilaPorId(conn, itemFila.Requisitante);
				requisicao = JsonConvert.DeserializeObject<RequisicaoJobCar>(item.Requisicao);

				var pathArquivoTemporario = new ArquivoManager().BuscarDiretorioArquivoTemporario(conn);

				if (!pathArquivoTemporario.EndsWith("\\"))
					pathArquivoTemporario += "\\";
				pathArquivoTemporario += "SICAR\\";

				//Salvar no diretorio de arquivos do SIMLAM Institucional
				string arquivoFinal;
				using (var stream = File.Open(pathArquivoTemporario + itemFila.Requisicao + ".car", FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					var arquivoManager = new ArquivoManager();
					arquivoFinal = arquivoManager.Salvar(itemFila.Requisicao, stream, conn);
				}



				var situacaoSolicitacao = (resultadoEnvio.codigoResposta == MensagemRetorno.CodigoRespostaSucesso) ? ControleCarDB.SITUACAO_SOLICITACAO_VALIDO : ControleCarDB.SITUACAO_SOLICITACAO_PENDENTE;
				ControleCarDB.AtualizarSolicitacaoCar(conn, requisicao, situacaoSolicitacao, tid);

				//Atualizar controle de envio do SICAR
				ControleCarDB.AtualizarControleSICAR(conn, resultadoEnvio, requisicao, ControleCarDB.SITUACAO_ENVIO_ARQUIVO_ENTREGUE, tid, arquivoFinal);

				//Marcar como processado
				LocalDB.MarcarItemFilaTerminado(conn, itemFila.Id, true, resultado);

			}
			catch (Exception ex)
			{
				//Marcar como processado registrando a mensagem de erro
				var msg = ex.Message +
					Environment.NewLine +
					Environment.NewLine +
					resultado;

				LocalDB.MarcarItemFilaTerminado(conn, itemFila.Id, false, msg);
				ControleCarDB.AtualizarSolicitacaoCar(conn, requisicao.origem, requisicao.solicitacao_car, ControleCarDB.SITUACAO_SOLICITACAO_PENDENTE, tid);
				ControleCarDB.AtualizarControleSICAR(conn, null, requisicao, ControleCarDB.SITUACAO_ENVIO_ARQUIVO_REPROVADO, tid);
			}
		}
	}
}
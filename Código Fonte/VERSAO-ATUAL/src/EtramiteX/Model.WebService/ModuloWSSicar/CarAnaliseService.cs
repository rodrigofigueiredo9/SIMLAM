using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;
using System.IO;
using Tecnomapas.Blocos.Entities.Interno.ModuloCadastroAmbientalRural;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Entities;
using Newtonsoft.Json;

namespace Interno.Model.WebService.ModuloWSSicar
{
	public class CarAnaliseService
	{
		private readonly HttpWebRequest _webRequest;
		private static HttpClientHandler _httpClientHandler = new HttpClientHandler()
		{
			PreAuthenticate = true,
			UseDefaultCredentials = false,
		};
		private static HttpClient _client = new HttpClient(_httpClientHandler);

		private bool producao = false;
		private static string urlSicarHomolog = "http://homolog.car.gov.br";
		private string urlSicarProd = "http://www.car.gov.br";

		public static SicarAnaliseRetornoDTO AlterarSituacaoSicar(CARSolicitacao solicitacao)
		{
			//var proxyUrl = ConfigurationManager.AppSettings["ProxyUrl"];
			//if (!String.IsNullOrWhiteSpace(proxyUrl))
			//{
			//	_httpClientHandler.Proxy = new WebProxy(proxyUrl, false)
			//	{
			//		UseDefaultCredentials = false,
			//		Credentials = CredentialCache.DefaultCredentials
			//	};
			//}

			var sicarUrl = $"{urlSicarHomolog}/imovel/analise/atualizar";
			//ConfigurationManager.AppSettings["apiSicar"];
			//if (_client == null)
			//{
			//	_client = new HttpClient(_httpClientHandler) { BaseAddress = new Uri(sicarUrl) };
			//	_client.DefaultRequestHeaders.Clear();
			//}
			_client.BaseAddress = new Uri(sicarUrl);
			try
			{
				using (var form = new MultipartFormDataContent())
				{
					form.Headers.Add("token", "04-C1-9F-A1-E7-72-AB-66-F0-AA-D2-EF-E6-1F-25-CD"); //ConfigurationManager.AppSettings["SicarToken"]);

					if(solicitacao.Status == eStatusImovelSicar.Cancelado)
					{
						var imageContent = new ByteArrayContent(solicitacao.ArquivoCancelamento.Conteudo);
						imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
						form.Add(imageContent, "documentoCancelamento", solicitacao.ArquivoCancelamento.Nome);
					}

					form.Add(new StringContent(solicitacao.Status.ToDescription()), "status");
					form.Add(new StringContent(solicitacao.Motivo.ToDescription()), "condicao");
					form.Add(new StringContent(solicitacao.Status == eStatusImovelSicar.Cancelado ? "false" : "true"), "retificavel");
					form.Add(new StringContent(solicitacao.SICAR.CodigoImovel), "codigoImovel");
					form.Add(new StringContent("true"), "completo");
					form.Add(new StringContent(solicitacao.DescricaoMotivo), "motivoMudancaStatus");
					form.Add(new StringContent(solicitacao.AutorCpf), "cpfResponsavel");
					form.Add(new StringContent("false"), "vistoriaRealizada");
					form.Add(new StringContent("false"), "documentoCancelamento");

					var response = _client.PostAsync("/sincronia/quick", form).Result;


					if (response.IsSuccessStatusCode)
					{
						var content = response.Content.ReadAsStringAsync().Result;
						var jsonFormat = JsonConvert.DeserializeObject<SicarAnaliseRetornoDTO>(content);
						
						return jsonFormat;
					}

					throw new ArgumentException("Erro de conexão com o SICAR");
				}


			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public static void SolicitarSuspensao(CARSolicitacao solicitacao)
		{

		}

		public static void ReverterSuspensao(CARSolicitacao solicitacao)
		{

		}
	}
}

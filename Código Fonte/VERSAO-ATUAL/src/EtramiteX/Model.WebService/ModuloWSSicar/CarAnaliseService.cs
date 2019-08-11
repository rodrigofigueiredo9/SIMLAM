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
		private static HttpClient _client;

		public CarAnaliseService(SicarAnalise sicar)
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

			//verifica se objeto já foi instanciado
			var sicarUrl = ConfigurationManager.AppSettings["apiSicar"];
			if (_client == null)
			{
				_client = new HttpClient(_httpClientHandler) { BaseAddress = new Uri(sicarUrl) };
				_client.DefaultRequestHeaders.Clear();

			}

			try
			{


				//var imageContent = new ByteArrayContent(streamContent.ReadAsByteArrayAsync());
				//imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");

				using (var form = new MultipartFormDataContent())
				{
					form.Headers.Add("token", ConfigurationManager.AppSettings["SicarToken"]);

					//form.Add(imageContent, "car", Path.GetFileName(localArquivoCar));

					form.Add(new StringContent(sicar.status.ToDescription()), "status");
					form.Add(new StringContent(sicar.condicao.ToDescription()), "condicao");
					form.Add(new StringContent(sicar.retificavel.ToString()), "retificavel");
					form.Add(new StringContent(sicar.codigoImovel), "codigoImovel");
					form.Add(new StringContent(sicar.completo.ToString()), "completo");

					var response = _client.PostAsync("/sincronia/quick", form).Result;


					if (response.IsSuccessStatusCode)
					{
						var content = response.Content.ReadAsStringAsync();
						//var jsonFormat = JToken.Parse(content).ToString();

						//return jsonFormat;
					}

					throw new ArgumentException("Erro de conexão com o SICAR, será feita uma nova tentativa ;", "resultado");
				}


			}
			catch (Exception ex)
			{

			}
		}
	}
}

using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;
using System.IO;

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

		public CarAnaliseService()
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
			var sicarUrl = ConfigurationManager.AppSettings["SicarUrl"];
			if (_client == null)
			{
				_client = new HttpClient(_httpClientHandler) { BaseAddress = new Uri(sicarUrl) };
				_client.DefaultRequestHeaders.Clear();

			}

			try
			{
				using (var fs = File.OpenRead(	))
				{
					using (var streamContent = new StreamContent(fs))
					{
						var imageContent = new ByteArrayContent( streamContent.ReadAsByteArrayAsync());
						imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");

						using (var form = new MultipartFormDataContent())
						{
							form.Headers.Add("token", ConfigurationManager.AppSettings["SicarToken"]);

							form.Add(imageContent, "car", Path.GetFileName(localArquivoCar));

							form.Add(new StringContent(dataCadastroEstadual), "dataCadastroEstadual");

							var response =  _client.PostAsync("/sincronia/quick", form).Result;


							if (response.IsSuccessStatusCode)
							{
								//var content =  response.Content.ReadAsStringAsync();
								//var jsonFormat = JToken.Parse(content).ToString();

								return jsonFormat;
							}

							throw new ArgumentException("Erro de conexão com o SICAR, será feita uma nova tentativa ;", "resultado");
						}
					}
				}
			}
			catch (Exception ex)
			{

			}
		}
	}
}

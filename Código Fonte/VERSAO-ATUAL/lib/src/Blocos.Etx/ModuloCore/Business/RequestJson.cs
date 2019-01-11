using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Routing;
using System.Web.Script.Serialization;
using Tecnomapas.Blocos.Etx.ModuloCore.Entities;

namespace Tecnomapas.Blocos.Etx.ModuloCore.Business
{
	public static class RequestJsonExtension
	{
		public static string ToQueryString(this RouteValueDictionary source)
		{
			if (source == null || source.Count == 0)
			{
				return string.Empty;
			}
			return "?" + String.Join("&", source.Select(x => String.Format("{0}={1}", HttpUtility.UrlEncode(x.Key), HttpUtility.UrlEncode(x.Value.ToString()))).ToArray());
		}
	}

	public class RequestJson
	{
		JavaScriptSerializer _jsSerializer = new JavaScriptSerializer();
		public const string POST = "POST";
		public const string GET = "GET";
		public CookieCollection AutenticacaoCookie { get; set; }
		

		public string Executar(string url, string method = GET, object postData = null)
		{
			StreamReader reader = null;
			Stream dataStream = null;
			HttpWebResponse response = null;
			string jonsResponseFromServer = null;

			string jSon = _jsSerializer.Serialize(postData);

			try
			{
				HttpWebRequest request = null;
				if (method == GET)
				{
					RouteValueDictionary rDic = new RouteValueDictionary(postData);
					request = (HttpWebRequest)HttpWebRequest.Create(url + rDic.ToQueryString());
				}
				else
				{
					request = (HttpWebRequest)HttpWebRequest.Create(url);
				}
				
				request.Method = method;
				request.CookieContainer = new CookieContainer();

				if (AutenticacaoCookie != null && AutenticacaoCookie.Count > 0)
				{
					request.CookieContainer.Add(AutenticacaoCookie);
				}

				if (method == POST)
				{
					byte[] byteArray = Encoding.UTF8.GetBytes(jSon);
					request.ContentType = "application/json; charset=UTF-8";
					request.ContentLength = byteArray.Length;
					request.UseDefaultCredentials = true;


					dataStream = request.GetRequestStream();
					dataStream.Write(byteArray, 0, byteArray.Length);
					dataStream.Close();
				}

				response = (HttpWebResponse)request.GetResponse();

				if (response.Cookies != null && response.Cookies.Count > 0)
				{
					AutenticacaoCookie = response.Cookies;
				}
				
				//Console.WriteLine(((HttpWebResponse)response).StatusDescription);
				
				// Get the stream containing content returned by the server.
				dataStream = response.GetResponseStream();
				// Open the stream using a StreamReader for easy access.
				reader = new StreamReader(dataStream);
				// Read the content.
				jonsResponseFromServer = reader.ReadToEnd();
				// Display the content.
				//Console.WriteLine(responseFromServer);
				// Clean up the streams.
			}
			catch (WebException webExc)
			{
				dataStream = webExc.Response.GetResponseStream();
				reader = new StreamReader(dataStream);
				jonsResponseFromServer = reader.ReadToEnd();

				//Validacao.AddErro(webExc);
				//Validacao.Add(new Mensagem() { Tipo = eTipoMensagem.Erro, Texto = jonsResponseFromServer });
			}
			finally
			{
				if (reader != null)
				{
					reader.Close();
				}

				if (dataStream != null)
				{
					dataStream.Close();
				}

				if (response != null)
				{
					response.Close();
				}
			}

			return jonsResponseFromServer;
		}

		public ResponseJsonData<T> Executar<T>(string url, string method = GET, object postData = null)
		{
			return _jsSerializer.Deserialize<ResponseJsonData<T>>(Executar(url, method, postData));
		}

		public T Deserializar<T>(string data)
		{
			return _jsSerializer.Deserialize<T>(data);
		}
	}
}

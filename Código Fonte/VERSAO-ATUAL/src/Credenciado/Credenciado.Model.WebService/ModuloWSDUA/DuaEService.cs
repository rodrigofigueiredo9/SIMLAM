using System;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;
using Tecnomapas.Blocos.Entities.WebService;

namespace Tecnomapas.EtramiteX.Credenciado.Model.WebService.ModuloWSDUA
{
	public class DuaEService
	{
		private readonly HttpWebRequest _webRequest;

		public DuaEService(string arquivoCertificado, string senhaCertificado, string urlProxy = "")
		{
			//Requisições feitas para o ambiente de Produção do DuaE
			_webRequest = (HttpWebRequest) WebRequest.Create(@"https://app.sefaz.es.gov.br/WsDua/DuaService.asmx");
			_webRequest.Headers.Add(@"SOAP:Action");
			_webRequest.ContentType = "text/xml;charset=\"utf-8\"";
			_webRequest.Accept = "text/xml";
			_webRequest.Method = "POST";

			var certificado = new X509Certificate2(arquivoCertificado, senhaCertificado,
				X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.PersistKeySet |
				X509KeyStorageFlags.Exportable);

			_webRequest.ClientCertificates.Add(certificado);

			if (urlProxy != "")
			{
				_webRequest.Proxy = new WebProxy(urlProxy, false)
				{
					UseDefaultCredentials = false,
					Credentials = CredentialCache.DefaultCredentials
				};

			}
			_webRequest.PreAuthenticate = true;
			_webRequest.UseDefaultCredentials = false;
		}

		public string ConsultarDua(string numeroDua, string cpfCnpj, eTipoPessoa tipoPessoa)
		{
			var xmlBuilder = new StringBuilder();

			xmlBuilder.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
			xmlBuilder.Append("<soap:Envelope xmlns:soap=\"http://www.w3.org/2003/05/soap-envelope\">");
			xmlBuilder.Append("<soap:Header>");
			xmlBuilder.Append("<DuaServiceHeader xmlns=\"http://www.sefaz.es.gov.br/duae\">");
			xmlBuilder.Append("<versao>1.00</versao>");
			xmlBuilder.Append("</DuaServiceHeader>");
			xmlBuilder.Append("</soap:Header>");
			xmlBuilder.Append("<soap:Body>");
			xmlBuilder.Append("<duaConsulta xmlns=\"http://www.sefaz.es.gov.br/duae\">");
			xmlBuilder.Append("<duaDadosMsg>");
			xmlBuilder.Append("<consDua versao=\"1.00\">");
			xmlBuilder.Append("<tpAmb>1</tpAmb>");
			xmlBuilder.Append($"<nDua>{numeroDua}</nDua>");
			if (tipoPessoa == eTipoPessoa.Fisica)
				xmlBuilder.Append("<cpf>" + cpfCnpj + "</cpf>");
			else
				xmlBuilder.Append("<cnpj>" + cpfCnpj + "</cnpj>");
			xmlBuilder.Append("</consDua>");
			xmlBuilder.Append("</duaDadosMsg>");
			xmlBuilder.Append("</duaConsulta>");
			xmlBuilder.Append("</soap:Body>");
			xmlBuilder.Append("</soap:Envelope>");

			return RealizarConsultaDuaSefaz(xmlBuilder);
		}

		private string RealizarConsultaDuaSefaz(StringBuilder xml)
		{

			var soapEnvelopeXml = new XmlDocument();
			soapEnvelopeXml.LoadXml(xml.ToString());

			var resultado = string.Empty;

			try
			{
				using (var stream = _webRequest.GetRequestStream())
				{
					soapEnvelopeXml.Save(stream);
				}

				using (var response = _webRequest.GetResponse())
				{
					using (var rd = new StreamReader(response.GetResponseStream()))
					{
						resultado = rd.ReadToEnd();
					}
				}
			}
			catch (Exception ex)
			{
				resultado = ex.Message;
			}

			return resultado;
		}

	}
}
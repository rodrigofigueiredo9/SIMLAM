using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using log4net;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using Quartz;
using Tecnomapas.EtramiteX.Scheduler.misc;
using Tecnomapas.EtramiteX.Scheduler.models.misc;
using Tecnomapas.EtramiteX.Scheduler.models.simlam;

namespace Tecnomapas.EtramiteX.Scheduler.jobs
{
	[DisallowConcurrentExecution]
	public class ConsultarDUAJob : IJob
	{
		private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public void Execute(IJobExecutionContext context)
		{
			//logging
			var jobKey = context.JobDetail.Key;
			Log.InfoFormat("BEGIN {0} executing at {1}", jobKey, DateTime.Now.ToString("r"));

			using (var conn = new OracleConnection(CarUtils.GetBancoInstitucional()))
			{
				conn.Open();

				//Veja se 
				var nextItem = LocalDB.PegarProximoItemFila(conn, "consultar-dua");

				while (nextItem != null)
				{
					//Update item as Started
					LocalDB.MarcarItemFilaIniciado(conn, nextItem.Id);

					try
					{
						//Buscar senha do certificado A3
						var senha = BuscarSenha(conn);

						//Consultar DUA
						var resultado = ConsultarDUA(nextItem.Requisicao, senha);

						//Marcar como processado
						LocalDB.MarcarItemFilaTerminado(conn, nextItem.Id, true, resultado);
					}
					catch (Exception ex)
					{
						//Marcar como processado registrando a mensagem de erro
						LocalDB.MarcarItemFilaTerminado(conn, nextItem.Id, false, ex.Message);
					}

					nextItem = LocalDB.PegarProximoItemFila(conn, "consultar-dua");
				}
			}

			Log.InfoFormat("ENDING {0} executing at {1}", jobKey, DateTime.Now.ToString("r"));
		}

		private string BuscarSenha(OracleConnection conn)
		{
			var schema = ConfigurationManager.AppSettings["esquemaInstitucional"];

			var senha = string.Empty;

			using (var cmd = new OracleCommand("SELECT valor FROM " + schema + ".CNF_SISTEMA t WHERE t.campo = 'duaSenhaCertificado'", conn))
			{
				using (var dr = cmd.ExecuteReader())
				{
					while (dr.Read())
					{
						return Convert.ToString(dr["valor"]);
					}
				}
			}

			return senha;
		}

		private String ConsultarDUA(string requisicao, string senhaCertificado)
		{
			//Requisições feitas para o ambiente de Produção do DuaE
			var webRequest = (HttpWebRequest) WebRequest.Create(@"https://app.sefaz.es.gov.br/WsDua/DuaService.asmx");
			webRequest.Headers.Add(@"SOAP:Action");
			webRequest.ContentType = "text/xml;charset=\"utf-8\"";
			webRequest.Accept = "text/xml";
			webRequest.Method = "POST";

			var certificado = new X509Certificate2(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Chaves Pública e Privada.pfx"), senhaCertificado,
				X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.PersistKeySet |
				X509KeyStorageFlags.Exportable);

			webRequest.ClientCertificates.Add(certificado);

			var proxyUrl = ConfigurationManager.AppSettings["ProxyUrl"];

			if (proxyUrl != string.Empty)
			{
				webRequest.Proxy = new WebProxy(proxyUrl, false)
				{
					UseDefaultCredentials = false,
					Credentials = CredentialCache.DefaultCredentials
				};
			}

			webRequest.PreAuthenticate = true;
			webRequest.UseDefaultCredentials = false;

			//Criando XML de requisicao
			var valores = JsonConvert.DeserializeObject<Dictionary<string, string>>(requisicao);

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
			xmlBuilder.Append("<nDua>" + valores["dua"] + "</nDua>");

			if (valores.ContainsKey("cpf"))
			{
				xmlBuilder.Append("<cpf>" + valores["cpf"] + "</cpf>");
			}
			else
			{
				xmlBuilder.Append("<cnpj>" + valores["cnpj"] + "</cnpj>");	
			}
			xmlBuilder.Append("</consDua>");
			xmlBuilder.Append("</duaDadosMsg>");
			xmlBuilder.Append("</duaConsulta>");
			xmlBuilder.Append("</soap:Body>");
			xmlBuilder.Append("</soap:Envelope>");

			var soapEnvelopeXml = new XmlDocument();
			soapEnvelopeXml.LoadXml(xmlBuilder.ToString());

			string resultado;

			try
			{
				using (var stream = webRequest.GetRequestStream())
				{
					soapEnvelopeXml.Save(stream);
				}

				using (var response = webRequest.GetResponse())
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
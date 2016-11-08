using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Data;
using System.Data;
using System.Text;
using System.IO;
using System.Xml;

namespace Tecnomapas.DesenhadorWS.Models.DataAcess
{
	public class LogDa
	{
		public static void Gerar(object objLog)
		{
			Gerar(GerarString(objLog));
		}

		public static string GerarString(object objLog)
		{
			try
			{
				System.Web.Script.Serialization.JavaScriptSerializer ser = new System.Web.Script.Serialization.JavaScriptSerializer();
				ser.RecursionLimit = 15;
				return ser.Serialize(objLog);
			}
			catch 
			{
			}

			XmlWriter xmlWrt = null;
			StringBuilder str = new StringBuilder();
			try
			{	
				xmlWrt = XmlWriter.Create(str);
				System.Xml.Serialization.XmlSerializer serXml = new System.Xml.Serialization.XmlSerializer(objLog.GetType());
				serXml.Serialize(xmlWrt, objLog);

				return str.ToString();
			}
			catch
			{
			}
			finally
			{
				if (xmlWrt != null)
				{
					xmlWrt.Close();
				}
				
				if (str != null)
				{
					str.Clear();
				}
			}

			try
			{
				dynamic dyn = objLog;

				if (dyn.Exc != null)
				{
					return dyn.Exc.ToString();
				}

				return string.Empty;
			}
			catch
			{
			}

			return string.Empty;
		}

		public static void Gerar(string log)
		{
			try
			{
				Comando comando = null;
				try
				{
					BancoDeDados bancoDeDadosGeo = BancoDeDadosFactory.CriarBancoDeDados("StringConexaoGeo");

					comando = bancoDeDadosGeo.GetComandoSql(@"insert into log_servicos (id, data, source, mensagem) values (seq_log_servicos.nextval, sysdate, 'DesenhadorWs', :log)"); ;
					comando.AdicionarParametroEntrada("log", DbType.String, 4000, log);

					bancoDeDadosGeo.ExecutarNonQuery(comando);
				}
				finally
				{
					if (comando != null)
					{
						comando.Dispose();
					}
				}
			}
			catch 
			{	
			}
		}
	}
}
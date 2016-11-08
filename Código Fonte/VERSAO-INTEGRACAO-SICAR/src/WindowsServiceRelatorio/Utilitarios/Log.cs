using System;
using System.Data;
using System.IO;
using System.Text;
using Tecnomapas.Blocos.Data;

namespace Tecnomapas.EtramiteX.WindowsService.Utilitarios
{
	public class Log
	{
		private static string _path = string.Empty;
		private static bool data = false;

		public static string PathAppData
		{
			get
			{
				if (string.IsNullOrEmpty(_path.Trim()))
				{
					_path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Tecnomapas\\EtramiteX\\IDAF\\";
				}

				if (!Directory.Exists(_path))
				{
					Directory.CreateDirectory(_path);
				}

				return _path;
			}
			set
			{
				_path = value;
			}
		}

		public static void GerarLogBanco(Exception exc)
		{
			string strMsg = GerarTextoExc(exc);

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"insert into log_servicos (id, data, source, machine, mensagem) 
					values (seq_log_servicos.nextval, sysdate, :source, :machine, :mensagem)");

				comando.AdicionarParametroEntrada("source", DbType.String, 1000, exc.Source);
				comando.AdicionarParametroEntrada("machine", DbType.String, 200, Environment.MachineName);

				if (!String.IsNullOrEmpty(strMsg))
				{
					comando.AdicionarParametroEntrada("mensagem", DbType.AnsiStringFixedLength, strMsg.Length, strMsg);
				}
				else
				{
					comando.AdicionarParametroEntrada("mensagem", DBNull.Value, DbType.String);
				}

				bancoDeDados.ExecutarNonQuery(comando);

				comando = bancoDeDados.CriarComando(@"delete from log_servicos t where t.id < ( select (max(a.id)-500) from log_servicos a )");
				bancoDeDados.ExecutarNonQuery(comando);
			}
		}

		public static void GerarLog(Exception exc)
		{
			try
			{
				GerarLogBanco(exc);
			}
			catch (Exception logExc)
			{
				#region Log no disco

				try
				{
					string excMsg = String.Format("Exception serviço: {0} \r\n Log:{1}", GerarTextoExc(exc), GerarTextoExc(logExc));
					GerarLog(excMsg);
				}
				catch
				{
				}

				#endregion
			}
		}

		private static void GerarLog(string Mensagem)
		{
			FileStream fs = null;
			StreamWriter w = null;

			try
			{
				string dir = PathAppData;
				string nomeExe = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;

				if (!data)
				{
					fs = new FileStream(dir + nomeExe + ".log", FileMode.OpenOrCreate, FileAccess.ReadWrite);
					data = (fs.Length > 10485760);
				}
				else
				{
					data = (File.Exists(dir + nomeExe + ".log"));
					fs = new FileStream(dir + nomeExe + DateTime.Now.ToString(".dd.MM.yyyy") + ".log", FileMode.OpenOrCreate, FileAccess.ReadWrite);
					int count = 0;

					while ((fs.Length > 10485760))
					{
						count++;
						fs.Close();
						fs = new FileStream(dir + nomeExe + DateTime.Now.ToString(".dd.MM.yyyy") + "-Arq" + count + ".log", FileMode.OpenOrCreate, FileAccess.ReadWrite);
					}
				}

				w = new StreamWriter(fs); // create a stream writer 
				w.BaseStream.Seek(0, SeekOrigin.End); // set the file pointer to the end of file 

				w.Write("-----------------------------------------------------------------------------\r\n");
				w.Write(DateTime.Now.ToString());
				w.Write("\r\n" + Mensagem);
				w.Write("\r\n-----------------------------------------------------------------------------\r\n");

				w.Flush();
			}
			catch
			{
			}
			finally
			{
				if (w != null)
				{
					w.Close();
					w = null;
				}

				if (fs != null)
				{
					fs.Close();
					fs = null;
				}
			}
		}

		private static string GerarTextoExc(Exception exc)
		{
			StringBuilder strMsg = new StringBuilder();

			strMsg.Append(exc.Message);
			strMsg.Append("\r\n" + exc.StackTrace);
			strMsg.Append("\r\n");

			Exception excTemp = exc.InnerException;

			while (excTemp != null)
			{
				strMsg.Append("\r\nInnerException: ");
				strMsg.Append(excTemp.Message);
				strMsg.Append("\r\n" + excTemp.StackTrace);
				strMsg.Append("\r\n");
				excTemp = excTemp.InnerException;
			}

			return strMsg.ToString();
		}
	}
}
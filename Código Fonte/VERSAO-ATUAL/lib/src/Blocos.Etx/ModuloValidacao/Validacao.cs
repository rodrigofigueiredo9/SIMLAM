using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Routing;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public class Validacao
	{
		private static List<Mensagem> _itens = new List<Mensagem>();

		public static List<Mensagem> Erros
		{
			get
			{
				if (HttpContext.Current != null)
				{
					if (HttpContext.Current.Items["ValidacaoErros"] == null)
					{
						HttpContext.Current.Items["ValidacaoErros"] = new List<Mensagem>();
					}
					return HttpContext.Current.Items["ValidacaoErros"] as List<Mensagem>;
				}
				else
				{
					return _itens;
				}
			}
			set 
			{
				if (HttpContext.Current != null)
				{
					HttpContext.Current.Items["ValidacaoErros"] = value;
				}
				else
				{
					_itens = value;
				}
			}
		}

		public static bool EhValido
		{
			get { return !Erros.Any(x => x.Tipo == eTipoMensagem.Erro || x.Tipo == eTipoMensagem.Advertencia || x.Tipo == eTipoMensagem.Confirmacao); }
		}

		public static void Add(Mensagem msg)
		{
			if (!Erros.Any(x => x.Campo == msg.Campo  && x.Tipo == msg.Tipo && x.Texto == msg.Texto))
			{
				Erros.Add(msg);
			}
		}

		public static void Add(eTipoMensagem tipo, String campo, String texto)
		{
			Mensagem msg = new Mensagem();
			msg.Tipo = tipo;
			msg.Campo = campo;
			msg.Texto = texto;

			Erros.Add(msg);
		}

		public static void Add(eTipoMensagem tipo, String texto)
		{
			Mensagem msg = new Mensagem();
			msg.Tipo = tipo;
			msg.Campo = String.Empty;
			msg.Texto = texto;

			Erros.Add(msg);
		}

		public static void AddErro(Exception exc, eTipoMensagem tipo = eTipoMensagem.Erro)
		{
			Mensagem msg = new Mensagem();
			msg.Tipo = tipo;
			msg.Campo = String.Empty;
			msg.Texto = exc.Message + " \n Trace: " + exc.StackTrace;
			Erros.Add(msg);
			int idx = Erros.Count - 1;

			Exception innerExc = exc;

			while (innerExc.InnerException != null)
			{
				innerExc = innerExc.InnerException;

				msg = new Mensagem();
				msg.Tipo = eTipoMensagem.Erro;
				msg.Campo = String.Empty;
				msg.Texto = innerExc.Message + " \n Trace: " + innerExc.StackTrace;

				Erros.Insert(idx, msg);
			}
		}

		public static void AddInformacao(String texto)
		{
			Mensagem msg = new Mensagem();
			msg.Tipo = eTipoMensagem.Informacao;
			msg.Campo = String.Empty;
			msg.Texto = texto;

			Erros.Add(msg);
		}

		public static void AddAdvertencia(String texto)
		{
			Mensagem msg = new Mensagem();
			msg.Tipo = eTipoMensagem.Advertencia;
			msg.Campo = String.Empty;
			msg.Texto = texto;

			Erros.Add(msg);
		}

		public static void AddAdvertencia(String campo, String texto)
		{
			Mensagem msg = new Mensagem();
			msg.Tipo = eTipoMensagem.Advertencia;
			msg.Campo = campo;
			msg.Texto = texto;

			Erros.Add(msg);
		}

		public static string QueryParam()
		{
			if (Erros == null || Erros.Count == 0)
				return String.Empty;

			return QueryParam(Erros);
		}

		public static string QueryParam(List<Mensagem> mensagens)
		{
			if (mensagens == null || mensagens.Count == 0)
				return String.Empty;

			//Tem Exception
			if (mensagens.Any(x => x.Texto.IndexOf("Trace: ") >= 0))
			{
				Mensagem msg = mensagens.First(x => x.Texto.IndexOf("Trace: ") >= 0);

				if (msg.Texto.Length > 1500)
				{
					msg.Texto = msg.Texto.Substring(0, 1500);
				}

				mensagens.RemoveAll(x => x.Texto.IndexOf("Trace: ") >= 0);

				mensagens.Insert(0, msg);
			}
			
			string strQueryParam = String.Join("^",
				 mensagens.Select(x => string.Format("ca*{0}|tipo*{1}|te*{2}", x.Campo, x.Tipo, x.Texto)));

			return EncodeTo64(strQueryParam);
		}

		static string EncodeTo64(string toEncode)
		{
			return HttpServerUtility.UrlTokenEncode(Encoding.UTF8.GetBytes(toEncode));
		}

		static string DecodeFrom64(string encodedData)
		{
			return Encoding.UTF8.GetString(HttpServerUtility.UrlTokenDecode(encodedData));
		}

		public static string QueryParamSerializer(string url, List<Mensagem> mensagens)
		{
			if (String.IsNullOrEmpty(url))
				return String.Empty;

			String query = QueryParam(mensagens);

			if (String.IsNullOrEmpty(query))
				return url;

			return String.Format("{0}{1}msg={2}", url, (url.Contains("?") ? "&" : "?"), query);
		}

		public static RouteValueDictionary QueryParamSerializer(Object values = null)
		{
			String query = QueryParam();
			RouteValueDictionary rDic = (values == null)? new RouteValueDictionary() : new RouteValueDictionary(values);
			rDic.Add("msg", query);
			return rDic;
		}

		public static string QueryParamSerializer(string url)
		{
			if (String.IsNullOrEmpty(url))
				return String.Empty;

			String query = QueryParam();

			if (String.IsNullOrEmpty(query))
				return url;

			return String.Format("{0}{1}msg={2}", url, (url.Contains("?") ? "&" : "?"), query);
		}

		public static void QueryParamDeserializer(string msg)
		{
			if (String.IsNullOrEmpty(msg))
				return;

			msg = DecodeFrom64(msg);

			string[] msgArray = msg.Split('^');

			foreach (string itemArray in msgArray)
			{
				string item = HttpUtility.UrlDecode(itemArray);
				string[] msgItemArray = item.Split('|');

				Add(new Mensagem()
				{
					Campo = msgItemArray[0].Substring(msgItemArray[0].IndexOf('*') + 1),
					Tipo = (eTipoMensagem)Enum.Parse(typeof(eTipoMensagem), msgItemArray[1].Substring(msgItemArray[1].IndexOf('*') + 1), true),
					Texto = msgItemArray[2].Substring(msgItemArray[2].IndexOf('*') + 1)
				});
			}
		}

		#region Alerta de EPTV

		private static List<Mensagem> _itensAlertaEPTV = new List<Mensagem>();

		public static List<Mensagem> MensagensAlertaEPTV
		{
			get
			{
				return _itensAlertaEPTV;
			}
			set
			{
				_itensAlertaEPTV = value;
			}
		}

		public static void AddAlertaEPTV(Mensagem msg)
		{
			if (!MensagensAlertaEPTV.Any(x => x.Campo == msg.Campo && x.Tipo == msg.Tipo && x.Texto == msg.Texto))
			{
				MensagensAlertaEPTV.Add(msg);
			}
		}

		#endregion Alerta de EPTV
	}
}



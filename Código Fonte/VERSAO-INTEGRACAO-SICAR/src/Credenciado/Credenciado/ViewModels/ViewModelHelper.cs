using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Script.Serialization;
using Tecnomapas.Blocos.Arquivo;
using Tecnomapas.Blocos.Arquivo.Data;
using Tecnomapas.Blocos.Entities.Configuracao;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Etx.ModuloArquivo.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloLista.Business;
using nameFile = System.Web.Mvc;

namespace Tecnomapas.EtramiteX.Credenciado.ViewModels
{
	public enum TextoDefaultDropDow
	{
		Selecione,
		Todos
	}

	public class ViewModelHelper
	{
		private static GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
		public static SelectListItem SelecionePadrao = new SelectListItem() { Text = "*** Selecione ***", Value = "0" };
		public static SelectListItem TodosPadrao = new SelectListItem() { Text = "*** Todos ***", Value = "0" };
		
		private static JavaScriptSerializer _jsSerializer = new JavaScriptSerializer();
		public static JavaScriptSerializer JsSerializer
		{
			get { return _jsSerializer; }
		}

		public static List<SelectListItem> CriarSelectList<T>(List<T> lista, bool? isFiltrarAtivo = null, bool itemTextoPadrao = true, string selecionado = null, string selecionadoTexto = null, TextoDefaultDropDow textoPadrao = TextoDefaultDropDow.Selecione) //where T : IListaValor, IListaValorString
		{
			List<SelectListItem> lstRetorno = new List<SelectListItem>();

			if (itemTextoPadrao)
			{
				ObterTextoPadrao(lstRetorno, textoPadrao);
			}

			if (typeof(T).GetInterface(typeof(IListaValor).Name) != null)
			{
				foreach (IListaValor item in lista)
				{
					if ((isFiltrarAtivo == null || item.IsAtivo == isFiltrarAtivo) || (item.Id.ToString() == selecionado))
					{
						lstRetorno.Add(new SelectListItem() { Value = item.Id.ToString(), Text = item.Texto, Selected = (item.Id.ToString() == selecionado) || (item.Texto.ToString() == selecionadoTexto) });
					}
				}
			}
			else
			{
				foreach (IListaValorString item in lista)
				{
					if ((isFiltrarAtivo == null || item.IsAtivo == isFiltrarAtivo) || (item.Id.ToString() == selecionado))
					{
						lstRetorno.Add(new SelectListItem() { Value = item.Id, Text = item.Texto, Selected = (item.Id.ToString() == selecionado) || (item.Texto.ToString() == selecionadoTexto) });
					}
				}
			}

			return lstRetorno;
		}

		private static void ObterTextoPadrao(List<SelectListItem> lstRetorno, TextoDefaultDropDow texto)
		{
			switch (texto)
			{
				case TextoDefaultDropDow.Selecione:
					lstRetorno.Add(SelecionePadrao);
					break;

				case TextoDefaultDropDow.Todos:
					lstRetorno.Add(TodosPadrao);
					break;
			}
		}

		public static List<SelectListItem> SelecionarSelectList(List<SelectListItem> lista, object selectedValue)
		{
			List<SelectListItem> lstRetorno = new List<SelectListItem>();

			foreach (SelectListItem item in lista)
			{
				lstRetorno.Add(new SelectListItem() { Value = item.Value.ToString(), Text = item.Text, Selected = item.Value.Equals(Convert.ToString(selectedValue)) });
			}

			return lstRetorno;
		}

		public static string Json(object objeto)
		{
			JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
			return jsSerializer.Serialize(objeto);
		}

		public static string CampoVazioListar(String str)
		{
			return String.IsNullOrEmpty(str) ? " - " : str;
		}

		public static string StringFit(String valor, int tam)
		{
			if (String.IsNullOrEmpty(valor) || valor.Length <= tam)
			{
				return valor;
			}

			return valor.Substring(0, tam - 3) + "...";
		}

		public static string CookieQuantidadePorPagina
		{
			get { return (HttpContext.Current.Request.Cookies.Get("QuantidadePorPagina") != null) ? HttpContext.Current.Request.Cookies.Get("QuantidadePorPagina").Value : "5"; }
		}

		public static int EstadoDefaultId()
		{
			Estado estado = ListaCredenciadoBus.Estados.SingleOrDefault(x => String.Equals(x.Texto, _configSys.Obter<String>(ConfiguracaoSistema.KeyEstadoDefault), StringComparison.InvariantCultureIgnoreCase));

			if (estado != null)
			{
				return estado.Id;
			}

			return 0;
		}

		public static string EstadoDefaultSigla()
		{
			return _configSys.Obter<String>(ConfiguracaoSistema.KeyEstadoDefault);
		}

		public static int MunicipioDefaultId()
		{
			return (
				ListaCredenciadoBus.Municipios(_configSys.Obter<String>(ConfiguracaoSistema.KeyEstadoDefault)).
					SingleOrDefault(x => String.Equals(x.Texto, _configSys.Obter<String>(ConfiguracaoSistema.KeyMunicipioDefault),
						StringComparison.InvariantCultureIgnoreCase)) ?? new Municipio()).Id;
		}

		public static RedirectResult RedirectDefaultMsg()
		{
			return new RedirectResult("~/Home/SistemaMensagem?Msg=" + Validacao.QueryParam());
		}

		public static IDictionary<string, Object> SetaDisabled(bool disabled, object atributos)
		{
			var listaAtributos = new RouteValueDictionary(atributos);

			if (disabled)
			{
				listaAtributos.Add("disabled", "disabled");
				listaAtributos["class"] = (listaAtributos["class"] ?? "").ToString() + " disabled";
			}

			return listaAtributos;
		}

		public static IDictionary<string, Object> SetaDisabledReadOnly(bool disabled, object atributos)
		{
			var listaAtributos = new RouteValueDictionary(atributos);
			if (disabled)
			{
				listaAtributos.Add("readonly", "readonly");
				listaAtributos["class"] = (listaAtributos["class"] ?? "").ToString() + " disabled";
			}
			return listaAtributos;
		}

		#region Controles de Arquivo

		public static FileResult BaixarArquivoTemporario(string nomeTemporario, string contentType)
		{
			try
			{
				ArquivoBus _busArquivo = new ArquivoBus(eExecutorTipo.Credenciado);
				return GerarArquivo(_busArquivo.ObterTemporario(new Arquivo() { TemporarioNome = nomeTemporario, ContentType = contentType }));
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public static FileResult BaixarArquivoInterno(int id)
		{
			try
			{
				ArquivoBus _busArquivo = new ArquivoBus(eExecutorTipo.Interno);

				return GerarArquivo(_busArquivo.Obter(id));
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public static FileResult BaixarArquivo(int id)
		{
			try
			{
				ArquivoBus _busArquivo = new ArquivoBus(eExecutorTipo.Credenciado);

				return GerarArquivo(_busArquivo.Obter(id));
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

        public static FileContentResult GerarArquivoPdf(MemoryStream memoryStream, string nomeArquivo, bool dataHoraControleAcesso = false)
        {
            return GerarArquivo(nomeArquivo + ".pdf", memoryStream, ContentType.PDF, dataHoraControleAcesso);
        }

		public static FileContentResult GerarArquivoPdf(MemoryStream memoryStream, string nomeArquivo)
		{
			return GerarArquivo(nomeArquivo + ".pdf", memoryStream, "application/pdf");
		}

        public static nameFile.FileContentResult GerarArquivo(string nomeExtensaoArquivo, MemoryStream memoryStream, string contentType, bool dataHoraControleAcesso = false)
        {
            try
            {
                if (dataHoraControleAcesso)
                {
                    using (MemoryStream msTemp = new MemoryStream(memoryStream.ToArray()))
                    {
                        memoryStream.Close();
                        memoryStream.Dispose();
                        memoryStream = Tecnomapas.Blocos.Etx.ModuloRelatorio.ITextSharpEtx.PdfMetodosAuxiliares.AdicionarDataHoraControleAcesso(msTemp);
                    }
                }

                nameFile.FileContentResult pdf = new nameFile.FileContentResult(memoryStream.ToArray(), contentType);
                pdf.FileDownloadName = nomeExtensaoArquivo;
                return pdf;
            }
            catch (Exception exc)
            {
                Validacao.AddErro(exc);
            }
            finally
            {
                memoryStream.Close();
                memoryStream.Dispose();
            }
            return null;
        }

		public static FileResult GerarArquivo(Arquivo arquivo)
		{
			if (arquivo.Buffer.CanSeek)
			{
				return GerarArquivo(arquivo.Nome, arquivo.Buffer, arquivo.ContentType);
			}
			else if (arquivo.Buffer is MemoryStream)
			{
				// Memorystreams criados por itext sao fechados, portanto use array de bytes
				return GerarArquivo(arquivo.Nome, arquivo.Buffer as MemoryStream, arquivo.ContentType);
			}

			return null;
		}

		public static FileContentResult GerarArquivo(string nomeExtensaoArquivo, MemoryStream memoryStream, string contentType)
		{
			try
			{
				FileContentResult pdf = new FileContentResult(memoryStream.ToArray(), contentType);
				pdf.FileDownloadName = nomeExtensaoArquivo;

				return pdf;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			finally
			{
				memoryStream.Close();
				memoryStream.Dispose();
			}

			return null;
		}

		public static FileStreamResult GerarArquivo(string nomeArquivo, Stream stream, string ContentType)
		{
			try
			{
				stream.Seek(0, SeekOrigin.Begin);
				FileStreamResult fileResult = new FileStreamResult(stream, ContentType);
				fileResult.FileDownloadName = nomeArquivo;

				return fileResult;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		#endregion

		#region Renderiza partial em uma string

		public static string RenderPartialViewToString(ControllerContext controllerContext)
		{
			return RenderPartialViewToString(controllerContext, null, null);
		}

		public static string RenderPartialViewToString(ControllerContext controllerContext, string viewName)
		{
			return RenderPartialViewToString(controllerContext, viewName, null);
		}

		public static string RenderPartialViewToString(ControllerContext controllerContext, object model)
		{
			return RenderPartialViewToString(controllerContext, null, model);
		}

		public static string RenderPartialViewToString(ControllerContext controllerContext, string viewName, object model)
		{
			ViewDataDictionary ViewData = new ViewDataDictionary(model);
			TempDataDictionary TempData = new TempDataDictionary();

			if (string.IsNullOrEmpty(viewName))
			{
				viewName = controllerContext.RouteData.GetRequiredString("action");
			}

			ViewData.Model = model;

			using (StringWriter sw = new StringWriter())
			{
				ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(controllerContext, viewName);
				ViewContext viewContext = new ViewContext(controllerContext, viewResult.View, ViewData, TempData, sw);
				viewResult.View.Render(viewContext, sw);

				return sw.GetStringBuilder().ToString();
			}
		}

		#endregion
	}
}
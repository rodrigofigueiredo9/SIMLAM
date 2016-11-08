using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx.CabecalhoRodape;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.ITextSharpEtx;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx
{
	public class PdfPadraoRelatorio
	{
		protected String ArquivoDocCaminho { get; set; }
		
		private ConfiguracaoDefault _confDefault = new ConfiguracaoDefault();
		protected ConfiguracaoDefault ConfiguracaoDefault
		{
			get { return _confDefault; }
			set { _confDefault = value; }
		}
		
		protected GeradorAspose Gerador { get; set; }
		protected Arquivo.Arquivo TemplatePdf { get; set; }
		
		protected Arquivo.Arquivo ObterArquivoTemplate()
		{
			FileStream file = File.OpenRead(HttpContext.Current.Server.MapPath(ArquivoDocCaminho));
			Arquivo.Arquivo templatePdf = new Arquivo.Arquivo();
			templatePdf.Buffer = (Stream)file;
			TemplatePdf = templatePdf;
			return templatePdf;
		}

		protected MemoryStream GerarPdf(Object dataSource, string documento)
		{
			Gerador = new GeradorAspose(ConfiguracaoDefault);
			return Gerador.PdfAnexo(TemplatePdf, dataSource, documento);
		}

		protected Int32 GerarPdfDoc(Object dataSource, string documento)
		{
			Gerador = new GeradorAspose(ConfiguracaoDefault);
			return Gerador.PdfAnexoDoc(TemplatePdf, dataSource, documento);
		}

		protected MemoryStream GerarPdf(Object dataSource)
		{
			Gerador = new GeradorAspose(ConfiguracaoDefault);
			return Gerador.Pdf(TemplatePdf, dataSource);
		}

		protected void ConfigurarCabecarioRodape(int setorId, bool isCredenciado = false)
		{
			ConfiguracaoDefault.CabecalhoRodape = CabecalhoRodapeFactory.Criar(setorId, isCredenciado: isCredenciado);
		}

		public MemoryStream MergePdf(MemoryStream pdfAspose, Arquivo.Arquivo arquivo)
		{
			return MergePdf(pdfAspose, new List<Arquivo.Arquivo>() { arquivo });
		}

		public MemoryStream MergePdf(MemoryStream pdfAspose, List<Arquivo.Arquivo> arquivo)
		{
			if (arquivo == null || arquivo.Count == 0)
			{
				return pdfAspose;
			}

			Document doc = new Document(PageSize.A4);

			MemoryStream ms = new MemoryStream();
			PdfWriter wrt = PdfWriter.GetInstance(doc, ms);

			doc.Open();

			PdfMetodosAuxiliares.AnexarPdf(pdfAspose, doc, wrt);

			PdfMetodosAuxiliares.AnexarPdf(arquivo, doc, wrt);

			doc.Close();
			doc.Dispose();

			//cria um Stream de saida que mantenha o fluxo aberto.
			MemoryStream msOut = new MemoryStream(ms.ToArray());
			ms.Close();
			ms.Dispose();

			return msOut;
		}
	}
}

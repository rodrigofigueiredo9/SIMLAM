using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tecnomapas.Blocos.Arquivo;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.ITextSharpEtx;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloGeoProcessamento.Pdf
{
	public class PdfAnaliseGeografica
	{
		public MemoryStream UnirPdfs(List<String> lstPath)
		{
			Document doc = new Document(PageSize.A4.Rotate(), 0, 0, 0, 0);
			MemoryStream str = new MemoryStream();

			PdfWriter wrt = PdfWriter.GetInstance(doc, str);
			wrt.SetEncryption(PdfWriter.STRENGTH128BITS, null, null, PdfWriter.AllowPrinting);

			doc.Open();

			List<Arquivo> arquivos = lstPath.Select(x => new Arquivo(){ Caminho = x, Extensao=".pdf" }).ToList();
			PdfMetodosAuxiliares.AnexarPdf(arquivos, doc, wrt);

			doc.Close();

			return str;
		}
	}
}

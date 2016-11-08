using System.Collections.Generic;
using System.IO;
using System.Linq;
using Aspose.Words.Tables;
using Tecnomapas.Blocos.Entities.Etx.ModuloRelatorio;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx;
using Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloLiberacaoNumeroCFOCFOC.Data;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloLiberacaoNumeroCFOCFOC;
using System;
using System.Web;

namespace Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloLiberacaoNumeroCFOCFOC.Pdf
{
	public class PdfComprovanteLiberacaoNumeroCFOCFOC : PdfPadraoRelatorio
	{
		LiberacaoNumeroCFOCFOCDa _da = new LiberacaoNumeroCFOCFOCDa();

		public MemoryStream Gerar(int id)
		{
			ArquivoDocCaminho = @"~/Content/_pdfAspose/Comprovante_Liberacao_Numero_CFO_CFOC.docx";

			ComprovanteLiberacaoNumeroCFOCFOCRelatorio dataSource = new ComprovanteLiberacaoNumeroCFOCFOCRelatorio();

			dataSource = _da.Obter(id);

			ObterArquivoTemplate();

			ConfigurarCabecarioRodape(0);

	

			var pagecount = GerarPdfDoc(dataSource, "DocumentoAnexo");


			return GerarPdf(dataSource);
		}
	}
}
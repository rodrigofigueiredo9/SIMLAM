using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloChecagemPendencia;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.ITextSharpEtx;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.ITextSharpEtx.CabecalhoRodape;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloChecagemPendencia.Pdf
{
	public class PdfChecagemPendencia
	{
		public MemoryStream ChecagemPendenciaPdf(ChecagemPendenciaRelatorioRelatorio checagem)
		{
			Document doc = new Document(PageSize.A4, 45, 45, 85, 50);
			MemoryStream str = new MemoryStream();

			PdfWriter wrt = PdfWriter.GetInstance(doc, str);

			wrt.SetEncryption(PdfWriter.STRENGTH128BITS, null, null, PdfWriter.AllowPrinting);

			wrt.PageEvent = CabecalhoRodapeFactory.Criar();

			doc.Open();

			PdfPTable tabelaDocumento = PdfMetodosAuxiliares.CriarTabela(5, new float[] { 5, 17, 50, 16, 12 });
			PdfMetodosAuxiliares.AddTituloData(doc, "Relatório de Pendências");

			#region CheckList Roteiro

			tabelaDocumento.DefaultCell.Colspan = 5;
			PdfMetodosAuxiliares.AddCabecarioTabela(tabelaDocumento, "Título de Pendência");

			tabelaDocumento.DefaultCell.Colspan = 2;
			tabelaDocumento.AddCell(new Phrase("Número:", PdfMetodosAuxiliares.arial10));

			tabelaDocumento.DefaultCell.Colspan = 3;
			tabelaDocumento.AddCell(new Phrase(checagem.TituloTipoSigla + " - " + checagem.TituloNumero, PdfMetodosAuxiliares.arial10));

			tabelaDocumento.DefaultCell.Colspan = 2;
			tabelaDocumento.AddCell(new Phrase("Vencimento: ", PdfMetodosAuxiliares.arial10));

			tabelaDocumento.DefaultCell.Colspan = 3;
			tabelaDocumento.AddCell(new Phrase(checagem.TituloVencimento.DataTexto, PdfMetodosAuxiliares.arial10));

			tabelaDocumento.DefaultCell.Colspan = 2;
			tabelaDocumento.AddCell(new Phrase("Processo/Documento: ", PdfMetodosAuxiliares.arial10));

			tabelaDocumento.DefaultCell.Colspan = 3;
			tabelaDocumento.AddCell(new Phrase(checagem.ProtocoloNumero, PdfMetodosAuxiliares.arial10));

			tabelaDocumento.DefaultCell.Colspan = 2;
			tabelaDocumento.AddCell(new Phrase("Interessado: ", PdfMetodosAuxiliares.arial10));

			tabelaDocumento.DefaultCell.Colspan = 3;
			tabelaDocumento.AddCell(new Phrase(checagem.InteressadoNome, PdfMetodosAuxiliares.arial10));

			#region Itens

			tabelaDocumento.DefaultCell.Colspan = 5;
			PdfMetodosAuxiliares.PularLinha(tabelaDocumento);
			PdfMetodosAuxiliares.AddCabecarioTabela(tabelaDocumento, "Itens Não Conferidos");

			List<ChecagemPendenciaItemRelatorio> itens = checagem.Itens.Where(x => x.SituacaoId == 1).ToList();

			for (int i = 0; i < itens.Count; i++)
			{
				tabelaDocumento.DefaultCell.Colspan = 1;
				tabelaDocumento.AddCell(new Phrase((i+1).ToString(), PdfMetodosAuxiliares.arial10));

				tabelaDocumento.DefaultCell.Colspan = 4;
				tabelaDocumento.AddCell(new Phrase(itens[i].Nome, PdfMetodosAuxiliares.arial10));
			}

			tabelaDocumento.DefaultCell.Colspan = 5;
			PdfMetodosAuxiliares.PularLinha(tabelaDocumento);
			PdfMetodosAuxiliares.AddCabecarioTabela(tabelaDocumento, "Itens Conferidos");

			itens = checagem.Itens.Where(x => x.SituacaoId == 2).ToList();

			for (int i = 0; i < itens.Count; i++)
			{
				tabelaDocumento.DefaultCell.Colspan = 1;
				tabelaDocumento.AddCell(new Phrase((i+1).ToString(), PdfMetodosAuxiliares.arial10));

				tabelaDocumento.DefaultCell.Colspan = 4;
				tabelaDocumento.AddCell(new Phrase(itens[i].Nome, PdfMetodosAuxiliares.arial10));
			}

			#endregion

			#endregion

			doc.Add(tabelaDocumento);

			doc.Close();
			doc.Dispose();

			return str;
		}
	}
}
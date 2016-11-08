using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloChecagemRoteiro;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloRoteiro;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.ITextSharpEtx;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.ITextSharpEtx.CabecalhoRodape;
using Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloChecagemRoteiro.Data;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloChecagemRoteiro.Pdf.RoteiroPdf
{
	public class PdfCheckListRoteiro
	{
		public static MemoryStream GerarCheckListRoteiroPdf(int id)
		{
			ChecagemRoteiroDa _da = new ChecagemRoteiroDa();
			ChecagemRoteiroRelatorio checkListRoteiro = _da.Obter(id);

			return GerarCheckListRoteiroPdf(checkListRoteiro);
		}

		public static MemoryStream GerarCheckListRoteiroPdf(ChecagemRoteiroRelatorio checkListRoteiro)
		{
			Document doc = new Document(PageSize.A4, 45, 45, 85, 50);
			MemoryStream str = new MemoryStream();

			PdfWriter wrt = PdfWriter.GetInstance(doc, str);

			wrt.SetEncryption(PdfWriter.STRENGTH128BITS, null, null, PdfWriter.AllowPrinting);

			wrt.PageEvent = CabecalhoRodapeFactory.Criar();

			doc.Open();

			
			PdfPTable tabelaDocumento = PdfMetodosAuxiliares.CriarTabela(2, new float[] { 20, 80 });
			PdfMetodosAuxiliares.AddTituloData(doc, "Relatório de Pendências de Itens de Roteiro");

			#region CheckList Roteiro

			tabelaDocumento.DefaultCell.Colspan = 2;

			if (checkListRoteiro.Id != 0)
			{
				tabelaDocumento.DefaultCell.BorderWidth = 0;
				tabelaDocumento.AddCell(PdfMetodosAuxiliares.AddCampoValor("Nº da checagem", checkListRoteiro.Id.ToString()));
			}

			tabelaDocumento.DefaultCell.BorderWidth = 0.5f;
			tabelaDocumento.DefaultCell.BackgroundColor = PdfMetodosAuxiliares.celulaCorPrata;
			tabelaDocumento.AddCell(new Phrase("Interessado", PdfMetodosAuxiliares.arial10Negrito));

			tabelaDocumento.DefaultCell.BackgroundColor = BaseColor.WHITE;
			tabelaDocumento.AddCell(new Phrase(checkListRoteiro.Interessado, PdfMetodosAuxiliares.arial10));

			tabelaDocumento.DefaultCell.BackgroundColor = PdfMetodosAuxiliares.celulaCorPrata;
			tabelaDocumento.AddCell(new Phrase("Roteiro Orientativo", PdfMetodosAuxiliares.arial10Negrito));

			tabelaDocumento.DefaultCell.BackgroundColor = BaseColor.WHITE;
			tabelaDocumento.DefaultCell.Padding = 0;

			PdfPTable tabelaInterior = PdfMetodosAuxiliares.CriarTabela(2, new float[] { 10, 80 });
			tabelaInterior.DefaultCell.BorderWidth = 0.5f;

			for (int i = 0; i < checkListRoteiro.Roteiros.Count; i++)
			{
				tabelaInterior.AddCell(PdfMetodosAuxiliares.AddCampoValor(" Versão", checkListRoteiro.Roteiros[i].Versao.ToString(), PdfMetodosAuxiliares.arial10, PdfMetodosAuxiliares.arial10));
				tabelaInterior.AddCell(new Phrase(checkListRoteiro.Roteiros[i].Nome, PdfMetodosAuxiliares.arial10));
			}
			tabelaDocumento.AddCell(tabelaInterior);

			//#region Itens

			PdfMetodosAuxiliares.PularLinha(tabelaDocumento);

			tabelaDocumento.AddCell(CarregarItens(checkListRoteiro.Itens.Where(x => x.SituacaoId == 1).ToList(), "Itens Pendentes"));

			PdfMetodosAuxiliares.PularLinha(tabelaDocumento);

			tabelaDocumento.AddCell(CarregarItens(checkListRoteiro.Itens.Where(x => x.SituacaoId == 2).ToList(), "Itens Conferidos"));

			PdfMetodosAuxiliares.PularLinha(tabelaDocumento);

			tabelaDocumento.AddCell(CarregarItens(checkListRoteiro.Itens.Where(x => x.SituacaoId == 3).ToList(), "Itens Dispensados"));

			#endregion

			doc.Add(tabelaDocumento);

			doc.Close();
			doc.Dispose();

			return str;
		}

		private static PdfPTable CarregarItens(List<ItemRelatorio> Itens, string tituloTabela)
		{
			int count = 1;
			PdfPTable tabelaInterior = PdfMetodosAuxiliares.CriarTabela(2, new float[] { 5, 95 });
			tabelaInterior.DefaultCell.BorderWidth = 0.5f;

			tabelaInterior.DefaultCell.BackgroundColor = PdfMetodosAuxiliares.celulaCorPrata;
			tabelaInterior.DefaultCell.Colspan = 2;
			tabelaInterior.AddCell(new Phrase(tituloTabela, PdfMetodosAuxiliares.arial10Negrito));

			tabelaInterior.DefaultCell.Colspan = 1;

			tabelaInterior.DefaultCell.BackgroundColor = BaseColor.WHITE;
			tabelaInterior.DefaultCell.Padding = 0;

			foreach (var item in Itens)
			{
				PdfPTable tabelaAux = PdfMetodosAuxiliares.CriarTabela();
				tabelaAux.DefaultCell.BorderWidth = 0.5f;

				tabelaInterior.AddCell(new Phrase(count.ToString(), PdfMetodosAuxiliares.arial10));

				tabelaAux.AddCell(new Phrase(item.Nome, PdfMetodosAuxiliares.arial10));

				if (!string.IsNullOrWhiteSpace(item.Condicionante))
				{
					tabelaAux.AddCell(new Phrase(item.Condicionante, PdfMetodosAuxiliares.arial10));
				}

				if (item.SituacaoId == 3)
				{
					tabelaAux.AddCell(new Phrase("Motivo da dispensa: "+item.Motivo, PdfMetodosAuxiliares.arial10));
				}

				tabelaInterior.AddCell(tabelaAux);

				count++;
			}
			return tabelaInterior;
		}
	}
}
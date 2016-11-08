using System;
using System.IO;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloTitulo;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.ITextSharpEtx;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.ITextSharpEtx.CabecalhoRodape;
using Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloTitulo.Data;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloTitulo.Pdf
{
	public class PdfEntrega
	{
		RelatorioEntregaDa _da = new RelatorioEntregaDa();

		public int ExisteEntregaProtocolo(int id)
		{
			return _da.ExisteEntregaProtocolo(id);
		}

		public MemoryStream Gerar(Int32 id)
		{	
			EntregaRelatorio entrega = _da.Obter(id);

			Document doc = new Document(PageSize.A4, 45, 45, 85, 50);
			MemoryStream str = new MemoryStream();
			
			PdfWriter wrt = PdfWriter.GetInstance(doc, str);
			wrt.SetEncryption(PdfWriter.STRENGTH128BITS, null, null, PdfWriter.AllowPrinting);

			wrt.PageEvent = CabecalhoRodapeFactory.Criar(entrega.ProtocoloSetorCriacao);

			doc.Open();

			PdfMetodosAuxiliares.PularLinha(doc);
			PdfMetodosAuxiliares.PularLinha(doc);

			PdfPTable tabela = PdfMetodosAuxiliares.CriarTabela();
			tabela.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
			tabela.DefaultCell.Border = 0;
			tabela.AddCell(new Phrase(new Chunk("Registro de Entrega de Título", PdfMetodosAuxiliares.arial16Negrito)));
			doc.Add(tabela);

			
			PdfMetodosAuxiliares.PularLinha(doc);
			PdfMetodosAuxiliares.PularLinha(doc);

			tabela = PdfMetodosAuxiliares.CriarTabela();
			tabela.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
			tabela.DefaultCell.Border = 0;

			Phrase frase = PdfMetodosAuxiliares.AddTexto("Nesta data foram retirados os seguintes títulos referentes ao ", PdfMetodosAuxiliares.arial10);
			frase.Add(new Chunk(entrega.ProtocoloTipo + " " + entrega.ProtocoloNumero + ":", PdfMetodosAuxiliares.arial10Negrito));
			tabela.AddCell(frase);
			
			doc.Add(tabela);

			tabela = PdfMetodosAuxiliares.CriarTabela(2, new float[] { 20, 80 });
			tabela.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;

			PdfMetodosAuxiliares.AddCabecarioTabela(tabela, "Número");
			PdfMetodosAuxiliares.AddCabecarioTabela(tabela, "Modelo");

			foreach (var titulo in entrega.Titulos)
			{
				tabela.AddCell(PdfMetodosAuxiliares.AddTexto(titulo.Numero, PdfMetodosAuxiliares.arial10));
				tabela.AddCell(PdfMetodosAuxiliares.AddTexto(titulo.Modelo, PdfMetodosAuxiliares.arial10));
			}

			doc.Add(tabela);

			PdfMetodosAuxiliares.PularLinha(doc);
			PdfMetodosAuxiliares.PularLinha(doc);
			PdfMetodosAuxiliares.PularLinha(doc);

			tabela = PdfMetodosAuxiliares.CriarTabela();
			tabela.DefaultCell.Border = 0;

			tabela.AddCell(PdfMetodosAuxiliares.AddTexto(PdfMetodosAuxiliares.AssinaturaCampo, PdfMetodosAuxiliares.arial10));
			tabela.AddCell(PdfMetodosAuxiliares.AddTexto("Nome: " + entrega.Nome, PdfMetodosAuxiliares.arial10));
			tabela.AddCell(PdfMetodosAuxiliares.AddTexto("CPF: " + entrega.CPF, PdfMetodosAuxiliares.arial10));

			doc.Add(tabela);

			PdfMetodosAuxiliares.PularLinha(doc);
			PdfMetodosAuxiliares.PularLinha(doc);
			PdfMetodosAuxiliares.PularLinha(doc);

			tabela = PdfMetodosAuxiliares.CriarTabela();
			tabela.DefaultCell.HorizontalAlignment = Element.ALIGN_RIGHT;
			tabela.DefaultCell.Border = 0;

			tabela.AddCell(PdfMetodosAuxiliares.AdicionarLocalDataEmissao(entrega.LocalEntrega, entrega.DataEntrega));

			doc.Add(tabela);

			doc.Close();
			doc.Dispose();

			return str;
		}
	}
}
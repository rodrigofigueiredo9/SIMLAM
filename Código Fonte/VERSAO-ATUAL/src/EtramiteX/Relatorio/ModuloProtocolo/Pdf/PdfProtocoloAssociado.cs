using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloProtocolo;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.ITextSharpEtx;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.ITextSharpEtx.CabecalhoRodape;
using Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloProtocolo.Data;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloProtocolo.Pdf
{
	public class PdfProtocoloAssociado
	{
		#region Propriedades

		ProtocoloDa _da = new ProtocoloDa();

		#endregion

		public int Existe(int protocoloId)
		{
			return _da.ExisteProtocoloAssociado(protocoloId);
		}

		public MemoryStream GerarPdfProtocoloAssociado(int protocoloId)
		{
			ProtocoloRelatorio protocolo = _da.Obter(protocoloId);

			Document doc = new Document(PageSize.A4, 45, 45, 85, 50);
			MemoryStream str = new MemoryStream();

			PdfWriter wrt = PdfWriter.GetInstance(doc, str);

			wrt.SetEncryption(PdfWriter.STRENGTH128BITS, null, null, PdfWriter.AllowPrinting);

			wrt.PageEvent = CabecalhoRodapeFactory.Criar(protocolo.SetorCriacaoId);

			doc.Open();

			PdfPTable tabelaDocumento = PdfMetodosAuxiliares.CriarTabela();
			tabelaDocumento.DefaultCell.Padding = 0;
			PdfMetodosAuxiliares.AddTituloData(doc, "Documentos Juntados/Processos Apensados");
			tabelaDocumento.DefaultCell.BorderWidth = 0;

			#region Processo/Documento

			tabelaDocumento.AddCell(PdfMetodosAuxiliares.AddTexto("Processo", PdfMetodosAuxiliares.arial10Negrito));
			PdfMetodosAuxiliares.PularLinha(tabelaDocumento);

			PdfPTable tabelaInterior = PdfMetodosAuxiliares.CriarTabela(2, new float[] { 20, 80 });

			tabelaInterior.AddCell(PdfMetodosAuxiliares.AddTexto("Tipo:", PdfMetodosAuxiliares.arial10));
			tabelaInterior.AddCell(PdfMetodosAuxiliares.AddTexto(protocolo.TipoTexto, PdfMetodosAuxiliares.arial10));
			tabelaInterior.AddCell(PdfMetodosAuxiliares.AddTexto("Número:", PdfMetodosAuxiliares.arial10));
			tabelaInterior.AddCell(PdfMetodosAuxiliares.AddTexto(protocolo.Numero, PdfMetodosAuxiliares.arial10));

			tabelaDocumento.AddCell(tabelaInterior);

			#endregion

			List<ProtocoloRelatorio> protocolosAux = new List<ProtocoloRelatorio>();

			#region Documentos Juntados

			protocolosAux = protocolo.ProtocolosAssociados.Where(x => x.ProtocoloTipo == 2).ToList();
			if (protocolosAux != null && protocolosAux.Count > 0)
			{
				GerarTabelas(protocolosAux, "Documentos Juntados", tabelaDocumento);
			}

			#endregion

			#region Processos Apensados

			protocolosAux = protocolo.ProtocolosAssociados.Where(x => x.ProtocoloTipo == 1).ToList();
			if (protocolosAux != null && protocolosAux.Count > 0) 
			{
				GerarTabelas(protocolosAux, "Processos Apensados", tabelaDocumento);
			}

			#endregion

			doc.Add(tabelaDocumento);

			doc.Close();
			doc.Dispose();

			return str;
		}

		private static void GerarTabelas(List<ProtocoloRelatorio> protocolos, string titulo, PdfPTable tabelaDocumento)
		{
			PdfMetodosAuxiliares.PularLinha(tabelaDocumento);
			tabelaDocumento.DefaultCell.BorderWidth = 0;
			tabelaDocumento.AddCell(PdfMetodosAuxiliares.AddTexto(titulo, PdfMetodosAuxiliares.arial10Negrito));
			PdfMetodosAuxiliares.PularLinha(tabelaDocumento);

			for (int i = 0; i < protocolos.Count; i++)
			{
				PdfPTable tabelaInterior = PdfMetodosAuxiliares.CriarTabela(2, new float[] { 20, 80 });

				tabelaInterior.AddCell(PdfMetodosAuxiliares.AddTexto("Número:", PdfMetodosAuxiliares.arial10));
				tabelaInterior.AddCell(PdfMetodosAuxiliares.AddTexto(protocolos[i].Numero, PdfMetodosAuxiliares.arial10));
				
				tabelaInterior.AddCell(PdfMetodosAuxiliares.AddTexto("Tipo:", PdfMetodosAuxiliares.arial10));
				tabelaInterior.AddCell(PdfMetodosAuxiliares.AddTexto(protocolos[i].TipoTexto, PdfMetodosAuxiliares.arial10));
				
				tabelaInterior.AddCell(PdfMetodosAuxiliares.AddTexto("Data:", PdfMetodosAuxiliares.arial10));
				tabelaInterior.AddCell(PdfMetodosAuxiliares.AddTexto(protocolos[i].Data, PdfMetodosAuxiliares.arial10));
				
				tabelaInterior.AddCell(PdfMetodosAuxiliares.AddTexto("Setor:", PdfMetodosAuxiliares.arial10));
				tabelaInterior.AddCell(PdfMetodosAuxiliares.AddTexto(protocolos[i].Setor, PdfMetodosAuxiliares.arial10));
				
				tabelaInterior.AddCell(PdfMetodosAuxiliares.AddTexto("Executor", PdfMetodosAuxiliares.arial10));
				tabelaInterior.AddCell(PdfMetodosAuxiliares.AddTexto(protocolos[i].Executor, PdfMetodosAuxiliares.arial10));

				tabelaDocumento.AddCell(tabelaInterior);
				PdfMetodosAuxiliares.PularLinha(tabelaDocumento);
			}
		}
	}
}
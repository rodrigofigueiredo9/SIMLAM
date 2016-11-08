using System;
using System.IO;
using Tecnomapas.Blocos.Arquivo;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloAnalise;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.ITextSharpEtx.CabecalhoRodape;
using Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloAnalise.Data;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloAnalise.Pdf
{
	public class PdfPecaTecnica
	{
		PecaTecnicaDa _da = new PecaTecnicaDa();

		#region Peça Tecnica

		public MemoryStream GerarPdf(Arquivo arquivo, Int32 id)
		{

			PecaTecnicaRelatorio pecaTecnica = _da.Obter(id);

			return ComplementarPdf(arquivo.Caminho, pecaTecnica);
		}

		internal MemoryStream ComplementarPdf(string path, PecaTecnicaRelatorio pecaTecnica)
		{
			Document doc = new Document(PageSize.A4, 85, 40, 73, 50);
			MemoryStream str = new MemoryStream();

			PdfWriter wrt = PdfWriter.GetInstance(doc, str);
			wrt.SetEncryption(PdfWriter.STRENGTH128BITS, null, null, PdfWriter.AllowPrinting);

			wrt.PageEvent = CabecalhoRodapeFactory.Criar(pecaTecnica.SetorId);
			(wrt.PageEvent as IPageMirroring).IsPageMirroring = true;

			doc.Open();

			//------------------------------------------------------
			PdfReader reader = new PdfReader(path);

			PdfContentByte cb;
			PdfImportedPage page;
			Rectangle psizeOrg = doc.PageSize;

			Font arial7Normal = new Font(Font.FontFamily.HELVETICA, 7, Font.NORMAL, BaseColor.BLACK);
			Font arial5Normal = new Font(Font.FontFamily.HELVETICA, 6, Font.NORMAL, BaseColor.BLACK);

			float TopMargin = doc.TopMargin;
			float BottomMargin = doc.BottomMargin;
			float LeftMargin = doc.LeftMargin;
			float RightMargin = doc.RightMargin;

			for (int i = 1; i <= reader.NumberOfPages; i++)
			{
				Rectangle paginaMedidas = reader.GetPageSize(i);
				float px = (psizeOrg.Width - paginaMedidas.Width) / 2;
				float py = (psizeOrg.Height - paginaMedidas.Height) / 2;

				if (px < 0 || py < 0)
				{
					px = 0;
					py = 0;
				}

				IPageMirroring pageEventMirroring = wrt.PageEvent as IPageMirroring;
				if (pageEventMirroring != null && pageEventMirroring.IsPageMirroring)
				{
					px = px - (doc.LeftMargin - (((i % 2) != 0) ? doc.LeftMargin : doc.RightMargin));
				}

				page = wrt.GetImportedPage(reader, i);
				Rectangle psize = reader.GetPageSizeWithRotation(i);

				doc.SetPageSize(psize);
				doc.NewPage();

				cb = wrt.DirectContent;
				cb.SaveState();


				if (psize.Rotation == 0)
				{
					cb.AddTemplate(page, 1f, 0, 0, 1f, 0, 0);
				}
				else if (psize.Rotation == 90)
				{
					cb.AddTemplate(page, 0, -1f, 1f, 0, 0, psize.Height);
				}
				else if (psize.Rotation == 180)
				{
					cb.AddTemplate(page, -1f, 0, 0, -1f, psize.Width, psize.Height);
				}
				else if (psize.Rotation == 270)
				{
					cb.AddTemplate(page, 0, 1.0F, -1.0F, 0, psize.Width, 0);
				}
				else
				{
					cb.AddTemplate(page, px, py);
				}


				cb.RestoreState();

				//pegando a primiera pagina


				if (i <= 2)
				{
					//Distancia do Topo

					float topGrid = 330;
					float topDadosImovel			= topGrid;
					float topInteressado			= topGrid - 23.8f;
					float topBairro					= topGrid - 57f;
					float topMunicipioUf			= topGrid - 77.3f;
					float topResponsavel			= topGrid - 95;
					float topAssinaturaResponsavel	= topGrid - 188;

					#region Linha Processo

					PdfPTable linhaProcesso = new PdfPTable(3);
					linhaProcesso.TotalWidth = doc.PageSize.Width;
					linhaProcesso.SetWidths(new float[] { 17, 10, 73 });
					linhaProcesso.DefaultCell.Padding = 0;
					linhaProcesso.DefaultCell.SetLeading(0.5f, 0.5f);
					linhaProcesso.DefaultCell.VerticalAlignment = Rectangle.ALIGN_BOTTOM;
					linhaProcesso.DefaultCell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
					linhaProcesso.DefaultCell.Border = Rectangle.NO_BORDER;
					//BoardEnable(linhaProcesso);
					linhaProcesso.AddCell(" "); //Margin
					linhaProcesso.AddCell(new Phrase(pecaTecnica.Protocolo, arial7Normal)); //Processo
					linhaProcesso.AddCell(" "); //Padding + spacing + padding
					linhaProcesso.WriteSelectedRows(0, -1, px, topDadosImovel, cb);

					#endregion

					#region Interessado

					PdfPTable linhaInteressados = new PdfPTable(3);
					linhaInteressados.TotalWidth = doc.PageSize.Width;
					linhaInteressados.SetWidths(new float[] { 17, 73, 10 });
					linhaInteressados.DefaultCell.Padding = 0;
					linhaInteressados.DefaultCell.Border = Rectangle.NO_BORDER;
					//linhaInteressados.DefaultCell.SetLeading(0.5f, 0.5f);
					linhaInteressados.DefaultCell.VerticalAlignment = Rectangle.ALIGN_BOTTOM;
					linhaProcesso.DefaultCell.Border = Rectangle.NO_BORDER;
					//BoardEnable(linhaInteressados);
					linhaInteressados.AddCell(" "); //Margin
					linhaInteressados.AddCell(new Phrase(string.Join(", ", pecaTecnica.Destinatarios.ToArray()), arial7Normal)); //Interessado Nome
					linhaInteressados.AddCell(" "); //Padding + spacing + padding
					linhaInteressados.WriteSelectedRows(0, -1, px, topInteressado, cb);

					#endregion

					#region Bairro

					PdfPTable linhaBairro = new PdfPTable(5);
					linhaBairro.TotalWidth = doc.PageSize.Width;
					linhaBairro.SetWidths(new float[] { 17, 35, 2, 35, 11 }); 
					linhaBairro.DefaultCell.Padding = 0;
					linhaBairro.DefaultCell.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
					linhaBairro.DefaultCell.Border = Rectangle.NO_BORDER;
					//BoardEnable(linhaBairro);
					linhaBairro.AddCell(" "); //Margin
					linhaBairro.AddCell(new Phrase(pecaTecnica.Bairro, arial7Normal));
					linhaBairro.AddCell(" "); //Padding + spacing + padding
					linhaBairro.AddCell(new Phrase(pecaTecnica.Distrito, arial7Normal));
					linhaBairro.AddCell(" "); //Padding + spacing + padding
					linhaBairro.WriteSelectedRows(0, -1, px, topBairro, cb);

					#endregion

					#region Municipio Uf

					PdfPTable linhaMunicipioEstado = new PdfPTable(3);
					linhaMunicipioEstado.TotalWidth = doc.PageSize.Width;
					linhaMunicipioEstado.SetWidths(new float[] { 17, 43, 40 }); 
					linhaMunicipioEstado.DefaultCell.Padding = 0;
					linhaMunicipioEstado.DefaultCell.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
					linhaMunicipioEstado.DefaultCell.Border = Rectangle.NO_BORDER;
					//BoardEnable(linhaMunicipioEstado);
					linhaMunicipioEstado.AddCell(" "); //Margin
					linhaMunicipioEstado.AddCell(new Phrase(String.Format("{0} - {1}", pecaTecnica.Municipio, pecaTecnica.Uf), arial7Normal)); //Municipio
					linhaMunicipioEstado.AddCell(" "); //Padding + spacing + padding
					linhaMunicipioEstado.WriteSelectedRows(0, -1, px, topMunicipioUf, cb);

					#endregion

					#region Linha Assinatura REsponsavel

					PdfPTable linhaAssinaturaResponsavel = new PdfPTable(3);
					linhaAssinaturaResponsavel.TotalWidth = doc.PageSize.Width;
					linhaAssinaturaResponsavel.SetWidths(new float[] { 23, 24, 53 }); //Proporcao 21,0 cm * 10 
					linhaAssinaturaResponsavel.DefaultCell.Padding = 0;
					linhaAssinaturaResponsavel.DefaultCell.SetLeading(0.6f, 0.6f);
					linhaAssinaturaResponsavel.DefaultCell.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
					linhaAssinaturaResponsavel.DefaultCell.HorizontalAlignment = Rectangle.ALIGN_CENTER;
					linhaAssinaturaResponsavel.DefaultCell.Border = Rectangle.NO_BORDER;
					//BoardEnable(linhaAssinaturaResponsavel);

					linhaAssinaturaResponsavel.AddCell(" "); 
					linhaAssinaturaResponsavel.AddCell(new Phrase(pecaTecnica.Elaborador, arial5Normal));
					linhaAssinaturaResponsavel.AddCell(" "); 

					linhaAssinaturaResponsavel.AddCell(" "); 
					linhaAssinaturaResponsavel.AddCell(new Phrase(pecaTecnica.ElaboradorProfissao, arial5Normal)); 
					linhaAssinaturaResponsavel.AddCell(" "); //Padding + spacing + padding

					linhaAssinaturaResponsavel.AddCell(" "); //Margin
					linhaAssinaturaResponsavel.AddCell(new Phrase(pecaTecnica.ElaboradorOrgaoClasseRegistro, arial5Normal)); 
					linhaAssinaturaResponsavel.AddCell(" "); //Padding + spacing + padding

					linhaAssinaturaResponsavel.CalculateHeights();


					linhaAssinaturaResponsavel.WriteSelectedRows(0, -1, px, topAssinaturaResponsavel, cb);

					#endregion
				}
			}

			doc.Close();

			return str;
		}

		#endregion
	}
}
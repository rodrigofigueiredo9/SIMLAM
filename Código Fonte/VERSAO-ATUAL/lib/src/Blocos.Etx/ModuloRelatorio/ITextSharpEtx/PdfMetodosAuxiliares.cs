using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Tecnomapas.Blocos.Entities.Etx.ModuloGeo;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Tecnomapas.Blocos.Etx.ModuloRelatorio.ITextSharpEtx
{
	public enum tipoFonte
	{
		ArialNarrow,
		ArialBlack,
		Arial
	}

	public class PdfMetodosAuxiliares
	{
		#region fontes e cores

		public static Font arial6 = GerarFonte(tipoFonte.Arial, 6, 0, BaseColor.BLACK);
		public static Font arial8 = GerarFonte(tipoFonte.Arial, 8, 0, BaseColor.BLACK);
		public static Font arial9 = GerarFonte(tipoFonte.Arial, 9, 0, BaseColor.BLACK);
		public static Font arial10 = GerarFonte(tipoFonte.Arial, 10, 0, BaseColor.BLACK);
		public static Font arial12 = GerarFonte(tipoFonte.Arial, 12, 0, BaseColor.BLACK);
		public static Font arial16 = GerarFonte(tipoFonte.Arial, 16, 0, BaseColor.BLACK);

		public static Font arial6Negrito = GerarFonte(tipoFonte.Arial, 6, Font.BOLD, BaseColor.BLACK);
		public static Font arial8Negrito = GerarFonte(tipoFonte.Arial, 8, Font.BOLD, BaseColor.BLACK);
		public static Font arial9Negrito = GerarFonte(tipoFonte.Arial, 9, Font.BOLD, BaseColor.BLACK);
		public static Font arial10Negrito = GerarFonte(tipoFonte.Arial, 10, Font.BOLD, BaseColor.BLACK);
		public static Font arial16Negrito = GerarFonte(tipoFonte.Arial, 16, Font.BOLD, BaseColor.BLACK);

		public static BaseColor celulaCorPrata = new BaseColor(242, 242, 242);
		public static Font arial10Vermelha = GerarFonte(tipoFonte.Arial, 10, 0, BaseColor.RED);
		private static BaseFont helv = BaseFont.CreateFont("Helvetica", BaseFont.WINANSI, false);

		#endregion

		#region Propriedade

		public static String AssemblyPath
		{
			get { return Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase); }
		}

		public static String AssinaturaCampo = "__________________________________________";

		public static ListaBus _listaBus = new ListaBus();

		#endregion

		#region Gerador Fontes Dinamicas

		private static void ExportarFontes(string path)
		{
			Type tipoClasse = typeof(PdfMetodosAuxiliares);
			string nameSpace = tipoClasse.Namespace;

			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}

			using (Stream stream = tipoClasse.Assembly.GetManifestResourceStream(nameSpace + ".Fontes.ARIALN.TTF"))
			{
				using (FileStream fileStr = new FileStream(path + "ARIALN.TTF", FileMode.Create))
				{
					stream.CopyTo(fileStr);

					stream.Close();
					fileStr.Close();
				}
			}

			using (Stream stream = tipoClasse.Assembly.GetManifestResourceStream(nameSpace + ".Fontes.ariblk.ttf"))
			{
				using (FileStream fileStr = new FileStream(path + "ariblk.ttf", FileMode.Create))
				{
					stream.CopyTo(fileStr);

					stream.Close();
					fileStr.Close();
				}
			}

			using (Stream stream = tipoClasse.Assembly.GetManifestResourceStream(nameSpace + ".Fontes.ARIAL.TTF"))
			{
				using (FileStream fileStr = new FileStream(path + "ARIAL.TTF", FileMode.Create))
				{
					stream.CopyTo(fileStr);

					stream.Close();
					fileStr.Close();
				}
			}
		}

		public static Font GerarFonte(tipoFonte tipo, int tamanho, int style, BaseColor corFonte)
		{
			Font fonteRetorno = null;

			string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Etx\\";
			if (!File.Exists(path + "ARIALN.TTF") || !File.Exists(path + "ariblk.ttf") || !File.Exists(path + "ARIAL.TTF"))
			{
				ExportarFontes(path);
			}

			switch (tipo)
			{
				case tipoFonte.ArialNarrow:
					path += "ARIALN.TTF";
					if (!FontFactory.IsRegistered(tipoFonte.ArialNarrow.ToString()))
					{
						FontFactory.Register(path, tipoFonte.ArialNarrow.ToString());
					}
					fonteRetorno = FontFactory.GetFont(tipoFonte.ArialNarrow.ToString(), tamanho, style, corFonte);
					break;

				case tipoFonte.ArialBlack:
					path += "ariblk.ttf";
					if (!FontFactory.IsRegistered("Arial Black"))
					{
						FontFactory.Register(path, "Arial Black");
					}
					fonteRetorno = FontFactory.GetFont("Arial Black", BaseFont.CP1252, BaseFont.NOT_EMBEDDED, tamanho, style, corFonte);
					break;

				case tipoFonte.Arial:
					path += "ARIAL.TTF";
					if (!FontFactory.IsRegistered("ARIAL"))
					{
						FontFactory.Register(path, "ARIAL");
					}
					fonteRetorno = FontFactory.GetFont("ARIAL", BaseFont.CP1252, BaseFont.NOT_EMBEDDED, tamanho, style, corFonte);
					break;
			}
			return fonteRetorno;
		}

		public static Font GerarFonte(tipoFonte tipo, int estilo, int tamanho)
		{
			return GerarFonte(tipo, tamanho, estilo, BaseColor.BLACK);
		}

		public static Font MudarCorFonte(Font fonte, BaseColor novaCor)
		{
			Font NovaFonte = new Font(fonte);
			NovaFonte.Color = novaCor;
			return NovaFonte;
		}

		#endregion

		#region Métodos

		public static void AddTituloData(Document doc, string titulo)
		{
			PularLinha(doc);

			Paragraph para;

			para = new Paragraph();
			para.Leading = 12f;
			para.Alignment = Element.ALIGN_RIGHT;

			para.Add(AddTextoChunk("Data da emissão: " + DateTime.Now.ToShortDateString(), arial8));

			doc.Add(para);

			para = new Paragraph();
			para.Leading = 12f;
			para.Alignment = Element.ALIGN_CENTER;

			para.Add(AddTextoChunk("\n" + titulo, arial16Negrito));

			doc.Add(para);
			PularLinha(doc);
		}

		public static void AddTituloValor(Document doc, string titulo, String campoNome, Object campoValor)
		{
			PularLinha(doc);

			PdfPTable tabela = CriarTabela(2, new float[] { 80, 20 });
			tabela.DefaultCell.BorderWidth = 0;

			tabela.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
			tabela.AddCell(new Phrase(AddTextoChunk(titulo, arial16Negrito)));

			tabela.DefaultCell.HorizontalAlignment = Element.ALIGN_RIGHT;
			tabela.AddCell(new Phrase(AddTextoChunk(campoNome + ": " + Convert.ToString(campoValor), arial12)));

			doc.Add(tabela);
			PularLinha(doc);
		}

		public static PdfPTable CriarTabela()
		{
			return CriarTabela(1, new float[] { 100 });
		}

		public static PdfPTable CriarTabela(int colunas, float[] larguraColunas)
		{
			PdfPTable pdfPTable = new PdfPTable(colunas);
			pdfPTable.SetWidths(larguraColunas);
			pdfPTable.WidthPercentage = 100;
			pdfPTable.DefaultCell.BorderWidth = 0.5f;

			return pdfPTable;
		}

		public static void PularLinha(Document documento)
		{
			documento.Add(AddTexto(" ", arial10));
		}

		public static void PularLinha(PdfPTable tabela)
		{
			tabela.DefaultCell.BorderWidth = 0;
			tabela.DefaultCell.BackgroundColor = BaseColor.WHITE;
			tabela.AddCell(" ");

			tabela.DefaultCell.BorderWidth = 0.5f;
		}

		public static void PularLinhaTabelaSemBorda(PdfPTable tabela)
		{
			tabela.DefaultCell.BackgroundColor = BaseColor.WHITE;
			tabela.AddCell(" ");
		}

		public static Phrase AddCampoValor(string campo, string valor)
		{
			return AddCampoValor(campo, valor, arial10Negrito, arial10);
		}

		public static Phrase AddTexto(string texto, Font fontTexto)
		{
			return new Phrase(AddTextoChunk(texto, fontTexto));
		}

		public static Phrase AddCampoValor(string campo, string valor, Font fontCampo, Font fontValor)
		{
			Phrase frase = AddTexto(campo + ": ", fontCampo);
			frase.Add(AddTextoChunk(valor, fontValor));
			return frase;
		}

		public static Chunk AddTextoChunk(string texto, Font fontTexto)
		{
			return new Chunk(texto, fontTexto);
		}

		public static void AddCabecarioTabela(PdfPTable tabela, string tituloTabela)
		{
			AddCabecarioTabela(tabela, celulaCorPrata, tituloTabela, arial10Negrito);
		}

		public static void AddCabecarioTabela(PdfPTable tabela, BaseColor fundo, string tituloTabela, Font font)
		{
			tabela.DefaultCell.BackgroundColor = fundo;
			tabela.AddCell(new Phrase(tituloTabela, font));
			tabela.DefaultCell.BackgroundColor = BaseColor.WHITE;
		}

		public static String BuscarDenominadorEmpreendimento(Int32? segmento)
		{
			String retorno = string.Empty;

			switch (segmento)
			{
				case 1:
				case 2:
				case 3:
					retorno = "Razão Social";
					break;

				case 0:
				case 4:
				case 6:
					retorno = "Denominação";
					break;

				case 5:
					retorno = "Nome da Propriedade/Ímovel";
					break;

				default:
					retorno = "Denominação";
					break;
			}

			return retorno;
		}

		public static String BuscarSiglaDatum(Int32? datum)
		{
			return _listaBus.Datuns.SingleOrDefault(x => Equals(x.Id, datum)).Sigla;
		}

		public static String BuscarFusoTexto(Int32? fuso)
		{
			return _listaBus.Fusos.SingleOrDefault(x => Equals(x.Id, fuso)).Texto;
		}

		public static String BuscarHemisfericoTexto(Int32? hemisferico)
		{
			return _listaBus.Hemisferios.SingleOrDefault(x => Equals(x.Id, hemisferico)).Texto;
		}

		public static String[] BuscarLongitudeCoordenada(Coordenada coordenada)
		{
			String[] LongitudeEasting = new String[2];

			switch (coordenada.Tipo.Id)
			{
				case 1:
					LongitudeEasting.SetValue("Longitude (GMS)", 0);
					LongitudeEasting.SetValue(coordenada.LongitudeGms, 1);
					break;

				case 2:
					LongitudeEasting.SetValue("Longitude (GDEC)", 0);
					LongitudeEasting.SetValue(coordenada.LongitudeGdec.ToString(), 1);
					break;

				case 3:
					LongitudeEasting.SetValue("Easting", 0);
					LongitudeEasting.SetValue(coordenada.EastingUtm.ToString(), 1);
					break;
			}

			return LongitudeEasting;
		}

		public static String[] BuscarLatitudeCoordenada(Coordenada coordenada)
		{
			String[] LongitudeEasting = new String[2];

			switch (coordenada.Tipo.Id)
			{
				case 1:
					LongitudeEasting.SetValue("Latitude (GMS)", 0);
					LongitudeEasting.SetValue(coordenada.LongitudeGms, 1);
					break;

				case 2:
					LongitudeEasting.SetValue("Latitude (GDEC)", 0);
					LongitudeEasting.SetValue(coordenada.LongitudeGdec.ToString(), 1);
					break;

				case 3:
					LongitudeEasting.SetValue("Northing", 0);
					LongitudeEasting.SetValue(coordenada.EastingUtm.ToString(), 1);
					break;
			}

			return LongitudeEasting;
		}

		public static void SetarMunicipioData(PdfPTable tabela, DateTime? data = null)
		{
			tabela.AddCell(AddTexto(_listaBus.MunicipioDefault + ", " + (data.HasValue ? data.Value.ToLongDateString().Split(',').GetValue(1) : DateTime.Now.ToLongDateString().Split(',').GetValue(1)), arial8Negrito));
		}

		public static void SetarMunicipioEstadoData(PdfPTable tabela, DateTime? data = null)
		{
			tabela.AddCell(AddTexto(ObterMunicipioEstadoDefault() + ", " + (data.HasValue ? data.Value.ToLongDateString().Split(',').GetValue(1) : DateTime.Now.ToLongDateString().Split(',').GetValue(1)), arial8Negrito));
		}

		public static string ObterMunicipioEstadoDefault()
		{
			return _listaBus.MunicipioDefault + " - " + _listaBus.EstadoDefault;
		}

		public static Phrase AdicionarLocalDataEmissao(string local, DateTime? data)
		{
			return AddTexto(local + ", " + (data.HasValue ? data.Value.ToLongDateString().Split(',').GetValue(1) : DateTime.Now.ToLongDateString().Split(',').GetValue(1)), arial8Negrito);
		}

		#endregion

		#region  Anexar Pdf

		public static bool ValidarPdf(Arquivo.Arquivo arquivo)
		{
			if (arquivo.Extensao.ToLower().Equals(".pdf"))
			{
				byte[] anexo = File.ReadAllBytes(arquivo.Caminho);
				string valor = string.Empty;

				for (int i = 0; i < anexo.Length; ++i)
				{
					valor += (char)anexo[i];

					if (i == 3)
					{
						break;
					}
				}

				anexo = null;

				if (valor != "%PDF")
				{
					return false;
				}
			}
			else
			{
				return false;
			}

			return true;
		}

		public static void AnexarPdf(List<Arquivo.Arquivo> arquivos, Document doc, PdfWriter wrt)
		{
			for (int i = 0; i < arquivos.Count; i++)
			{
				AnexarPdf(arquivos[i], doc, wrt);
			}
		}

		public static void AnexarPdf(Arquivo.Arquivo arquivo, Document doc, PdfWriter wrt)
		{
			try
			{
				//* Essa validacao nao serve para aquivos temporarios
				//* Desenvolvedor deve garantir a passagem de arquivos temporarios
				//* ou tratar a excessao 
				//if (arquivo.Extensao.ToLower() != ".pdf")
				//{
				//    return;
				//}

				PdfReader reader = new PdfReader(File.ReadAllBytes(arquivo.Caminho));

				//if (!reader.IsOpenedWithFullPermissions)
				//{
				//    return;
				//}

				PdfContentByte cb;
				PdfImportedPage page;
				Rectangle psize;
				Rectangle psizeOrg = doc.PageSize;

				float TopMargin = doc.TopMargin;
				float BottomMargin = doc.BottomMargin;
				float LeftMargin = doc.LeftMargin;
				float RightMargin = doc.RightMargin;

				doc.SetMargins(0, 0, 0, 0);
				IPdfPageEvent pageEvent = wrt.PageEvent;

				for (int i = 1; i <= reader.NumberOfPages; i++)
				{
					psize = reader.GetPageSizeWithRotation(i);

					doc.SetPageSize(psize);
					doc.NewPage();
					wrt.PageEvent = null;

					cb = wrt.DirectContent;
					cb.SaveState();

					page = wrt.GetImportedPage(reader, i);

					if (psize.Rotation == 0)
					{
						cb.AddTemplate(page, 1f, 0, 0, 1f, 0, 0);
					}

					if (psize.Rotation == 90)
					{
						cb.AddTemplate(page, 0, -1f, 1f, 0, 0, psize.Height);
					}

					if (psize.Rotation == 180)
					{
						cb.AddTemplate(page, -1f, 0, 0, -1f, psize.Width, psize.Height);
					}

					if (psize.Rotation == 270)
					{
						cb.AddTemplate(page, 0, 1.0F, -1.0F, 0, psize.Width, 0);
					}

					cb.RestoreState();
				}

				doc.SetPageSize(psizeOrg);
				doc.SetMargins(LeftMargin, RightMargin, TopMargin, BottomMargin);
				doc.NewPage();
				wrt.PageEvent = pageEvent;

			}
			catch (Exception exc)
			{
				throw new Exception("Erro ao gerar anexo de pdf", exc);
			}
		}

		public static void AnexarPdf(MemoryStream ms, Document doc, PdfWriter wrt)
		{
			if (ms == null)
			{
				return;
			}

			AnexarArquivos(new PdfReader(ms.ToArray()), doc, wrt);
		}

		public static MemoryStream AnexarPdf(MemoryStream msi, MemoryStream msj)
		{
			//PdfCopyFields copy = new PdfCopyFields(msPdf);
			Document document = new Document(PageSize.A4);
			MemoryStream ms = new MemoryStream();

			PdfWriter writer = PdfWriter.GetInstance(document, ms);

			// Open document to write
			document.Open();

			PdfMetodosAuxiliares.AnexarPdf(msi, document, writer);
			PdfMetodosAuxiliares.AnexarPdf(msj, document, writer);

			document.Close();
			document.Dispose();

			MemoryStream msOut = new MemoryStream(ms.ToArray());
			ms.Close();
			ms.Dispose();

			return msOut;
		}

		private static void AnexarArquivos(PdfReader reader, Document doc, PdfWriter wrt)
		{
			try
			{
				PdfContentByte cb;
				PdfImportedPage page;
				Rectangle psize;
				Rectangle psizeOrg = doc.PageSize;

				float TopMargin = doc.TopMargin;
				float BottomMargin = doc.BottomMargin;
				float LeftMargin = doc.LeftMargin;
				float RightMargin = doc.RightMargin;

				doc.SetMargins(0, 0, 0, 0);
				IPdfPageEvent pageEvent = wrt.PageEvent;

				for (int i = 1; i <= reader.NumberOfPages; i++)
				{
					psize = reader.GetPageSizeWithRotation(i);

					doc.SetPageSize(psize);
					doc.NewPage();
					wrt.PageEvent = null;

					cb = wrt.DirectContent;
					cb.SaveState();

					page = wrt.GetImportedPage(reader, i);

					if (psize.Rotation == 0)
					{
						cb.AddTemplate(page, 1f, 0, 0, 1f, 0, 0);
					}

					if (psize.Rotation == 90)
					{
						cb.AddTemplate(page, 0, -1f, 1f, 0, 0, psize.Height);
					}

					if (psize.Rotation == 180)
					{
						cb.AddTemplate(page, -1f, 0, 0, -1f, psize.Width, psize.Height);
					}

					if (psize.Rotation == 270)
					{
						cb.AddTemplate(page, 0, 1.0F, -1.0F, 0, psize.Width, 0);
					}

					cb.RestoreState();
				}

				doc.SetPageSize(psizeOrg);
				doc.SetMargins(LeftMargin, RightMargin, TopMargin, BottomMargin);
				doc.NewPage();
				wrt.PageEvent = pageEvent;
				//reader.Close();
			}
			catch (Exception exc)
			{
				throw new Exception("Erro ao gerar anexo de pdf", exc);
			}
		}

		internal void GerarPdf(Document doc, PdfWriter wrt, bool isUsePageEvent, PdfPTable legenda)
		{
			PdfReader reader = new PdfReader("");

			PdfContentByte cb;
			PdfImportedPage page;
			Rectangle psize;
			Rectangle psizeOrg = doc.PageSize;

			float TopMargin = doc.TopMargin;
			float BottomMargin = doc.BottomMargin;
			float LeftMargin = doc.LeftMargin;
			float RightMargin = doc.RightMargin;
			IPdfPageEvent pageEvent = wrt.PageEvent;

			doc.SetMargins(0, 0, 0, 0);

			for (int i = 1; i <= reader.NumberOfPages; i++)
			{
				psize = reader.GetPageSize(i);

				doc.SetPageSize(psize);
				doc.NewPage();

				if (!isUsePageEvent)
					wrt.PageEvent = null;

				cb = wrt.DirectContent;
				cb.SaveState();

				page = wrt.GetImportedPage(reader, i);

				cb.AddTemplate(page, 0, 0);

				legenda.TotalWidth = (doc.Right - doc.Left) - 85;
				legenda.WriteSelectedRows(0, -1, doc.Left + 42.5f, doc.Bottom + 212, cb);

				cb.RestoreState();
			}

			doc.SetPageSize(psizeOrg);
			doc.SetMargins(LeftMargin, RightMargin, TopMargin, BottomMargin);

			wrt.PageEvent = pageEvent;
		}

		#endregion

		public static MemoryStream AdicionarTarjaPdf(Stream pdf, string texto, BaseColor corFundo, BaseColor corTexto)
		{
			MemoryStream ms = new MemoryStream();
			PdfImportedPage page;
			PdfReader reader = new PdfReader(pdf);
			Document doc = new Document(reader.GetPageSizeWithRotation(1));
			PdfContentByte cb = null;

			PdfWriter wrt = PdfWriter.GetInstance(doc, ms);
			
			wrt.PageEvent = null;
			
			doc.Open();

			#region Páginas do Pdf

			for (int i = 1; i <= reader.NumberOfPages; i++)
			{
				doc.SetPageSize(reader.GetPageSizeWithRotation(i));
				doc.NewPage();

				cb = wrt.DirectContentUnder;
				cb.SaveState();

				cb.SetColorFill(corFundo);
				cb.Rectangle(doc.PageSize.Right - 22, 0, 22, doc.PageSize.Height);
				cb.Fill();

				cb.RestoreState();
				cb.SaveState();

				cb.SetColorFill(corTexto);
				cb.BeginText();
				cb.SetFontAndSize(arial16.BaseFont, 16);				
				cb.ShowTextAligned(Element.ALIGN_RIGHT, texto, doc.PageSize.Right-8, doc.PageSize.Height-10, 90);
				cb.EndText();
				
				cb.RestoreState();
				cb.SaveState();

				cb.ResetGrayFill();

				cb.RestoreState();
				cb.SaveState();

				page = wrt.GetImportedPage(reader, i);

				if (doc.PageSize.Rotation == 0)
				{
					cb.AddTemplate(page, 1f, 0, 0, 1f, 0, 0);
				}

				if (doc.PageSize.Rotation == 90)
				{
					cb.AddTemplate(page, 0, -1f, 1f, 0, 0, doc.PageSize.Height);
				}

				if (doc.PageSize.Rotation == 180)
				{
					cb.AddTemplate(page, -1f, 0, 0, -1f, doc.PageSize.Width, doc.PageSize.Height);
				}

				if (doc.PageSize.Rotation == 270)
				{
					cb.AddTemplate(page, 0, 1.0F, -1.0F, 0, doc.PageSize.Width, 0);
				}

				cb.RestoreState();
			}

			#endregion
			doc.Close();

			MemoryStream msOut = new MemoryStream(ms.ToArray());
			ms.Close();
			ms.Dispose();

			return msOut;
		}

		public static MemoryStream AdicionarTarjaPdf(Stream pdf, string texto1, string texto2, BaseColor corFundo, BaseColor corTexto)
		{
			MemoryStream ms = new MemoryStream();
			PdfImportedPage page;
			PdfReader reader = new PdfReader(pdf);
			Document doc = new Document(reader.GetPageSizeWithRotation(1));
			PdfContentByte cb = null;

			PdfWriter wrt = PdfWriter.GetInstance(doc, ms);

			wrt.PageEvent = null;

			doc.Open();

			#region Páginas do Pdf

			for (int i = 1; i <= reader.NumberOfPages; i++)
			{
				doc.SetPageSize(reader.GetPageSizeWithRotation(i));
				doc.NewPage();

				cb = wrt.DirectContentUnder;
				cb.SaveState();

				cb.SetColorFill(corFundo);
				cb.Rectangle(doc.PageSize.Right - 22, 0, 22, doc.PageSize.Height);
				cb.Fill();

				cb.RestoreState();
				cb.SaveState();

				cb.SetColorFill(corTexto);
				cb.BeginText();
				cb.SetFontAndSize(arial16.BaseFont, 16);
				cb.ShowTextAligned(Element.ALIGN_LEFT, texto1, doc.PageSize.Right - 8, 10, 90);
				cb.ShowTextAligned(Element.ALIGN_RIGHT, texto2, doc.PageSize.Right - 8, doc.PageSize.Height - 10, 90);
				cb.EndText();

				cb.RestoreState();
				cb.SaveState();

				cb.ResetGrayFill();

				cb.RestoreState();
				cb.SaveState();

				page = wrt.GetImportedPage(reader, i);

				if (doc.PageSize.Rotation == 0)
				{
					cb.AddTemplate(page, 1f, 0, 0, 1f, 0, 0);
				}

				if (doc.PageSize.Rotation == 90)
				{
					cb.AddTemplate(page, 0, -1f, 1f, 0, 0, doc.PageSize.Height);
				}

				if (doc.PageSize.Rotation == 180)
				{
					cb.AddTemplate(page, -1f, 0, 0, -1f, doc.PageSize.Width, doc.PageSize.Height);
				}

				if (doc.PageSize.Rotation == 270)
				{
					cb.AddTemplate(page, 0, 1.0F, -1.0F, 0, doc.PageSize.Width, 0);
				}

				cb.RestoreState();
			}

			#endregion

			doc.Close();

			MemoryStream msOut = new MemoryStream(ms.ToArray());
			ms.Close();
			ms.Dispose();

			return msOut;
		}

		public static MemoryStream AdicionarDataHoraControleAcesso(Stream pdf)
		{
			//Ignora PASSWORD de PDFs protegidos
			PdfReader.unethicalreading = true;

			BaseColor corTexto = BaseColor.BLACK;
			MemoryStream ms = new MemoryStream();
			PdfImportedPage page;
			PdfReader reader = new PdfReader(pdf);
			Document doc = new Document(reader.GetPageSizeWithRotation(1));
			PdfContentByte cb = null;

			PdfWriter wrt = PdfWriter.GetInstance(doc, ms);

			wrt.PageEvent = null;

			doc.Open();

			var parser = new iTextSharp.text.pdf.parser.PdfReaderContentParser(reader);

			#region Páginas do Pdf

			for (int i = 1; i <= reader.NumberOfPages; i++)
			{
				doc.SetPageSize(reader.GetPageSizeWithRotation(i));
				doc.NewPage();

				cb = wrt.DirectContentUnder;

				cb.SaveState();

				cb.SetColorFill(corTexto);
				cb.BeginText();
				cb.SetFontAndSize(arial16.BaseFont, 5);

				page = wrt.GetImportedPage(reader, i);

				float y = doc.PageSize.Bottom + 5f;
				float x = doc.PageSize.Width / 2;
				string texto = DateTime.Now.ToString("dd/M/yyyy H:mm:ss");

				switch (doc.PageSize.Rotation)
				{
					case 0:
						cb.ShowTextAligned(Element.ALIGN_LEFT, texto, x, y, 0);//Rodape
						cb.AddTemplate(page, 1f, 0, 0, 1f, 0, 0);
						break;

					case 90:
						cb.ShowTextAligned(Element.ALIGN_LEFT, texto, x, y, 0);//Rodape
						cb.AddTemplate(page, 0, -1f, 1f, 0, 0, doc.PageSize.Height);
						break;

					case 180:
						cb.ShowTextAligned(Element.ALIGN_LEFT, texto, x, y, 0);//Rodape
						cb.AddTemplate(page, -1f, 0, 0, -1f, doc.PageSize.Width, doc.PageSize.Height);
						break;

					case 270:
						cb.ShowTextAligned(Element.ALIGN_LEFT, texto, x, y, 0);//Rodape
						cb.AddTemplate(page, 0, 1.0F, -1.0F, 0, doc.PageSize.Width, 0);
						break;
				}

				cb.EndText();
				cb.RestoreState();

				cb.SaveState();
				cb.ResetGrayFill();

				cb.RestoreState();
			}

			#endregion

			doc.Close();

			MemoryStream msOut = new MemoryStream(ms.ToArray());
			ms.Close();
			ms.Dispose();

			return msOut;
		}

		public static MemoryStream AdicionarDataHoraControleAcesso(Stream pdf, int tituloModeloCodigo)
		{
			//Ignora PASSWORD de PDFs protegidos
			PdfReader.unethicalreading = true;

			BaseColor corTexto = BaseColor.BLACK;
			MemoryStream ms = new MemoryStream();
			PdfImportedPage page;
			PdfReader reader = new PdfReader(pdf);
			Document doc = new Document(reader.GetPageSizeWithRotation(1));
			PdfContentByte cb = null;

			PdfWriter wrt = PdfWriter.GetInstance(doc, ms);

			wrt.PageEvent = null;

			doc.Open();

			var parser = new iTextSharp.text.pdf.parser.PdfReaderContentParser(reader);

			#region Páginas do Pdf

			for (int i = 1; i <= reader.NumberOfPages; i++)
			{
				doc.SetPageSize(reader.GetPageSizeWithRotation(i));
				doc.NewPage();

				cb = wrt.DirectContentUnder;

				cb.SaveState();

				cb.SetColorFill(corTexto);
				cb.BeginText();
				cb.SetFontAndSize(arial16.BaseFont, 5);

				page = wrt.GetImportedPage(reader, i);

				float x1, y, x2;
				y = x1 = x2 = 0f;

				switch (tituloModeloCodigo)
				{
					case 19 /*Certificado de Registro de Atividade Florestal*/:
						y = doc.PageSize.Bottom + doc.BottomMargin * 3 - 10f;
						x1 = doc.PageSize.Width / 4;
						x2 = (doc.PageSize.Width / 4) * 3;

						break;

					case 20 /*Licença de Porte e Uso de Motosserra*/:
						y = doc.PageSize.Bottom + doc.BottomMargin * 3 - 15f;
						x1 = doc.PageSize.Width / 4;
						x2 = (doc.PageSize.Width / 4) * 3 - 20f;

						break;

					default:
						break;
				}

				string texto = DateTime.Now.ToString("dd/M/yyyy H:mm:ss");

				switch (doc.PageSize.Rotation)
				{
					case 0:
						cb.ShowTextAligned(Element.ALIGN_LEFT, texto, x1, y, 0);//Rodape
						cb.ShowTextAligned(Element.ALIGN_LEFT, texto, x2, y, 0);//Rodape
						cb.AddTemplate(page, 1f, 0, 0, 1f, 0, 0);
						break;

					case 90:
						cb.ShowTextAligned(Element.ALIGN_LEFT, texto, x1, y, 0);//Rodape
						cb.ShowTextAligned(Element.ALIGN_LEFT, texto, x2, y, 0);//Rodape
						cb.AddTemplate(page, 0, -1f, 1f, 0, 0, doc.PageSize.Height);
						break;

					case 180:
						cb.ShowTextAligned(Element.ALIGN_LEFT, texto, x1, y, 0);//Rodape
						cb.ShowTextAligned(Element.ALIGN_LEFT, texto, x2, y, 0);//Rodape
						cb.AddTemplate(page, -1f, 0, 0, -1f, doc.PageSize.Width, doc.PageSize.Height);
						break;

					case 270:
						cb.ShowTextAligned(Element.ALIGN_LEFT, texto, x1, y, 0);//Rodape
						cb.ShowTextAligned(Element.ALIGN_LEFT, texto, x2, y, 0);//Rodape
						cb.AddTemplate(page, 0, 1.0F, -1.0F, 0, doc.PageSize.Width, 0);
						break;
				}

				cb.EndText();
				cb.RestoreState();

				cb.SaveState();
				cb.ResetGrayFill();

				cb.RestoreState();
			}

			#endregion

			doc.Close();

			MemoryStream msOut = new MemoryStream(ms.ToArray());
			ms.Close();
			ms.Dispose();

			return msOut;
		}

		public static byte[] LerArquivoFileStream(FileStream fs)
		{
			try
			{
				using (MemoryStream ms = new MemoryStream())
				{
					fs.CopyTo(ms);
					return ms.ToArray();
				}
			}
			finally			
			{
				if (fs != null)
				{
					fs.Close();
					fs.Dispose();
				}
			}
		}

		public static byte[] CorrigirBytesPdf(byte[] bytesPdf)
		{
			for (int j = bytesPdf.Length - 1; j > 0; j--)
			{
				if (bytesPdf[j] != 0)
				{
					byte[] newPdf = new byte[j];
					Array.Copy(bytesPdf, newPdf, j);

					return newPdf;
				}
			}

			return bytesPdf;
		}

		public static MemoryStream TarjaVerde(Stream stream, string texto)
		{
			return AdicionarTarjaPdf(stream, texto, new BaseColor(0, 176, 80), BaseColor.WHITE);
		}

		public static MemoryStream TarjaVermelha(Stream stream, string texto)
		{
			return AdicionarTarjaPdf(stream, texto, BaseColor.RED, BaseColor.WHITE);
		}

		public static MemoryStream TarjaLaranjaEscuro(Stream stream, string texto)
		{
			return AdicionarTarjaPdf(stream, texto, new BaseColor(255, 69, 0), BaseColor.WHITE);
		}

		public static MemoryStream TarjaLaranja(Stream stream, string texto)
		{
			return AdicionarTarjaPdf(stream, texto, new BaseColor(255, 165, 0), BaseColor.WHITE);
		}

		public static MemoryStream TarjaVerde(Stream stream, string texto1, string texto2)
		{
			return AdicionarTarjaPdf(stream, texto1, texto2, new BaseColor(0, 176, 80), BaseColor.WHITE);
		}

		public static MemoryStream TarjaVermelha(Stream stream, string texto1, string texto2)
		{
			return AdicionarTarjaPdf(stream, texto1, texto2, BaseColor.RED, BaseColor.WHITE);
		}

		public static MemoryStream TarjaLaranjaEscuro(Stream stream, string texto1, string texto2)
		{
			return AdicionarTarjaPdf(stream, texto1, texto2, new BaseColor(255, 69, 0), BaseColor.WHITE);
		}

		public static MemoryStream TarjaLaranja(Stream stream, string texto1, string texto2)
		{
			return AdicionarTarjaPdf(stream, texto1, texto2, new BaseColor(255, 165, 0), BaseColor.WHITE);
		}

		public static String Concatenar(List<String> lista)
		{
			if (lista.Count == 1)
				return lista[0];

			string lastItem = lista.Last();
			lista.RemoveAt(lista.Count - 1);
			return String.Join(", ", lista) + " e " + lastItem;
		}
	}
}
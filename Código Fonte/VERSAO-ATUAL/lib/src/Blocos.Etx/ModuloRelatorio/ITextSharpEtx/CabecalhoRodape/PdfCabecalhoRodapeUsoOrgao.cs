using System;
using System.Web;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Tecnomapas.Blocos.Etx.ModuloRelatorio.ITextSharpEtx.CabecalhoRodape
{
	public class PdfCabecalhoRodapeUsoOrgao : PdfPageEventHelper, ICabecalhoRodape
	{
		#region Atributos

		public PdfPTable tabelaCabecalho;
		public PdfPTable tabelaRodape;
		public Image headerImage;
		public Image logoSimlam;

		public PdfGState gstate;
		public PdfTemplate tpPag;
		public BaseFont helv;

		private int pagina = 1;
		private int count = 1;

		private float alturaRodape = 790;

		private string textoBackground = "PARA SIMPLES CONFERÊNCIA";
		private string textoSetor = string.Empty;
		private BaseColor corTextoSetor = BaseColor.BLACK;

		public string GovernoNome { get; set; }
		public string SecretariaNome { get; set; }
		
		public string OrgaoNome { get; set; }
		public string OrgaoSigla { get; set; }

		public string OrgaoEndereco { get; set; }
		public string OrgaoMunicipio { get; set; }
		public string OrgaoUF { get; set; }
		public string OrgaoCep { get; set; }
		public string OrgaoContato { get; set; }

		public string SetorNome { get; set; }

		public int Pagina
		{
			get { return pagina; }
			set { pagina = value; }
		}

		public int Total
		{
			get { return count; }
			set { count = value; }
		}

		public float AlturaRodape
		{
			get { return alturaRodape; }
			set { alturaRodape = value; }
		}

		public string TextoBackground
		{
			get { return textoBackground; }
			set { textoBackground = value; }
		}

		public string TextoSetor
		{
			get { return textoSetor; }
			set { textoSetor = value; }
		}

		public BaseColor CorTextoSetor
		{
			get { return corTextoSetor; }
			set { corTextoSetor = value; }
		}

		#endregion

		private static String ImagemOrgao { get { return HttpContext.Current.Server.MapPath("~/Content/_imgLogo/logomarca.png"); } }

		private static String ImagemSimlam { get { return HttpContext.Current.Server.MapPath("~/Content/_imgLogo/logomarca_simlam_pb.png"); } }

		public override void OnOpenDocument(PdfWriter writer, Document document)
		{
			#region CABECALHO

			PdfPTable tabelaLinha;
			PdfPCell celula;
			Paragraph para;

			tabelaCabecalho = new PdfPTable(3);
			tabelaCabecalho.WidthPercentage = 100;
			tabelaCabecalho.SetWidths(new float[] { 78, 2, 20 });
			tabelaCabecalho.DefaultCell.Border = 0;
			tabelaCabecalho.DefaultCell.Padding = 0;
			tabelaCabecalho.SplitLate = true;
			tabelaCabecalho.SplitRows = true;

			PdfPTable tabelaInterna = new PdfPTable(1);
			tabelaInterna.WidthPercentage = 100;
			tabelaInterna.SetWidths(new float[] { 100 });
			tabelaInterna.DefaultCell.BorderWidth = 1;
			tabelaInterna.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;

			PdfPCell celulaInterna = new PdfPCell();
			celulaInterna.BorderWidth = 1;

			tabelaLinha = new PdfPTable(2);
			tabelaLinha.WidthPercentage = 100;
			tabelaLinha.SetWidths(new float[] { 25, 85 });
			tabelaLinha.DefaultCell.Border = 0;
			tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
			tabelaLinha.DefaultCell.Padding = 0;

			headerImage = Image.GetInstance(ImagemOrgao);
			headerImage.ScaleToFit(90, 90);

			PdfPTable tabelaImagem = new PdfPTable(2);
			tabelaImagem.WidthPercentage = 100;
			tabelaImagem.SetWidths(new float[] { 90, 10 });
			tabelaImagem.DefaultCell.Border = 0;
			tabelaImagem.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
			tabelaImagem.DefaultCell.Padding = 0;

			celula = new PdfPCell();
			celula.BorderWidth = 0;
			celula.HorizontalAlignment = Element.ALIGN_CENTER;

			celula.AddElement(headerImage);

			tabelaImagem.AddCell(celula);
			tabelaImagem.AddCell("");

			celula = new PdfPCell();
			celula.BorderWidth = 0;
			celula.HorizontalAlignment = Element.ALIGN_CENTER;

			celula.AddElement(tabelaImagem);
			tabelaLinha.AddCell(celula);

			celula = new PdfPCell();
			celula.BorderWidth = 0;
			para = new Paragraph();
			para.Leading = 10f;
			para.Alignment = Element.ALIGN_CENTER;

			para.Add(new Chunk(GovernoNome, PdfMetodosAuxiliares.arial10Negrito));
			para.Add(new Chunk("\n" + SecretariaNome, PdfMetodosAuxiliares.arial9Negrito));
			para.Add(new Chunk("\n" + OrgaoNome, PdfMetodosAuxiliares.arial9Negrito));

			if (!String.IsNullOrEmpty(SetorNome))
			{
				para.Add(new Chunk("\n" + SetorNome, PdfMetodosAuxiliares.arial9));
			}

			celula.AddElement(para);
			tabelaLinha.AddCell(celula);

			celulaInterna.AddElement(tabelaLinha);
			tabelaInterna.AddCell(celulaInterna);

			tabelaCabecalho.AddCell(tabelaInterna);

			#region Uso Orgao

			celula = new PdfPCell();
			celula.BorderWidth = 0;
			tabelaCabecalho.AddCell(celula);

			celula = new PdfPCell();
			celula.BorderWidth = 1;
			celula.AddElement(new Chunk(string.Format("Para uso do {0}.", OrgaoSigla), PdfMetodosAuxiliares.arial6));
			tabelaCabecalho.AddCell(celula);

			#endregion

			#endregion

			#region RODAPE

			logoSimlam = Image.GetInstance(ImagemSimlam);
			logoSimlam.ScaleToFit(65, 65);

			tabelaRodape = new PdfPTable(3);
			tabelaRodape.DefaultCell.BorderWidthTop = 1;
			tabelaRodape.DefaultCell.BorderWidth = 0;

			float[] larguraColunas2 = { 15, 70, 15 };
			tabelaRodape.SetWidths(larguraColunas2);
			tabelaRodape.WidthPercentage = 100;
			tabelaRodape.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
			tabelaRodape.DefaultCell.VerticalAlignment = Element.ALIGN_TOP;

			celula = new PdfPCell();
			celula.AddElement(new Chunk(logoSimlam, 0, -5));
			celula.HorizontalAlignment = Element.ALIGN_LEFT;
			celula.VerticalAlignment = Element.ALIGN_TOP;
			celula.Border = 0;
			celula.BorderWidthTop = 1;

			tabelaRodape.AddCell(celula);

			Paragraph frase = new Paragraph();

			frase.Add(new Chunk(string.Format("{0} {1}/{2} {3}", OrgaoEndereco, OrgaoMunicipio, OrgaoUF, OrgaoCep), PdfMetodosAuxiliares.arial8));
			frase.Add(new Chunk("\n" + OrgaoContato, PdfMetodosAuxiliares.arial8));

			tabelaRodape.AddCell(frase);

			celula = new PdfPCell();
			celula.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
			celula.Border = 0;
			celula.BorderWidthTop = 1;

			celula.AddElement(new Chunk(""));

			tabelaRodape.AddCell(celula);

			#endregion

			#region NUMEROS DE PAGINAS

			Pagina = 1;
			Total = 1;
			gstate = new PdfGState();
			gstate.FillOpacity = (0.3f);
			gstate.StrokeOpacity = (0.3f);


			//writer.DirectContent.SaveState();
			tpPag = writer.DirectContent.CreateTemplate(100, 100);
			tpPag.BoundingBox = (new Rectangle(-20, -20, 100, 100));
			//writer.DirectContent.RestoreState();

			// initialization of the font
			helv = BaseFont.CreateFont("Helvetica", BaseFont.WINANSI, false);

			#endregion
		}

		public override void OnEndPage(PdfWriter writer, Document document)
		{
			PdfContentByte cb = writer.DirectContent;
			cb.SaveState();

			// Cabeçalho
			tabelaCabecalho.TotalWidth = (document.Right - document.Left);
			tabelaCabecalho.WriteSelectedRows(0, -1, document.Left, document.PageSize.Height - 15, cb);


			// Rodapé
			tabelaRodape.TotalWidth = (document.Right - document.Left);
			tabelaRodape.WriteSelectedRows(0, -1, document.Left, document.PageSize.Height - 795, cb);

			cb.RestoreState();

			// Numero de Páginas
			String text = pagina.ToString() + "/";

			if (pagina == count)
			{
				count++;
			}
			pagina++;

			float textSize = helv.GetWidthPoint(text, 10);
			float textBase = document.Bottom - 17;

			cb.SaveState();
			cb.BeginText();
			cb.SetFontAndSize(helv, 10);
			cb.SetTextMatrix((document.Right - 20), textBase);
			cb.ShowText(text);
			cb.EndText();
			cb.AddTemplate(tpPag, (document.Right - 20) + textSize, textBase);
			cb.RestoreState();
		}

		public override void OnCloseDocument(PdfWriter writer, Document document)
		{
			tpPag.BeginText();
			tpPag.SetFontAndSize(helv, 10);
			tpPag.SetTextMatrix(0, 0);
			tpPag.ShowText("" + (Total - 1));
			tpPag.EndText();
		}
	}
}
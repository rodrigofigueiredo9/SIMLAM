using System;
using Aspose.Words;
using Aspose.Words.Tables;

namespace Tecnomapas.Blocos.RelatorioPersonalizado.Entities
{
	public class Estilo
	{
		public String Id { get; set; }
		public ParagraphFormat Paragrafo { get; set; }
		public RowFormat Linha { get; set; }
		public CellFormat Celula { get; set; }
		public Style ParagrafoEstilo { get; set; }
		public String Texto { get; set; }
		
		public Estilo() { }

		public Estilo(String id, Font fonte, Style para, ParagraphFormat paragrafo, RowFormat linha, CellFormat celula, String texto)
		{
			Id = id;
			Paragrafo = paragrafo;
			Linha = linha;
			Celula = celula;

			if(!string.IsNullOrEmpty(texto))
			{
				Texto = texto.Replace("\r", "").Replace("\n", "");
			}

			ParagrafoEstilo = para;
			ParagrafoEstilo.Font.AllCaps = fonte.AllCaps;
			ParagrafoEstilo.Font.Bold = fonte.Bold;
			ParagrafoEstilo.Font.Bidi = fonte.Bidi;
			ParagrafoEstilo.Font.BoldBi = fonte.BoldBi;
			ParagrafoEstilo.Font.Color = fonte.Color;
			ParagrafoEstilo.Font.HighlightColor = fonte.HighlightColor;
			ParagrafoEstilo.Font.Italic = fonte.Italic;
			ParagrafoEstilo.Font.ItalicBi = fonte.ItalicBi;
			ParagrafoEstilo.Font.Name = fonte.Name;
			ParagrafoEstilo.Font.Spacing = fonte.Spacing;
			ParagrafoEstilo.Font.Size = fonte.Size;
			ParagrafoEstilo.Font.StyleName = fonte.StyleName;
			ParagrafoEstilo.Font.TextEffect = fonte.TextEffect;
			ParagrafoEstilo.Font.Underline = fonte.Underline;
			ParagrafoEstilo.Font.UnderlineColor = fonte.UnderlineColor;
		}
	}
}
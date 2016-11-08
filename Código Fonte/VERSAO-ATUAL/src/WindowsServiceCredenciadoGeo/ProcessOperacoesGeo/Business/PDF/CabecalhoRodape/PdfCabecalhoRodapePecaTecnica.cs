using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Tecnomapas.EtramiteX.WindowsService.ProcessOperacoesGeo.Business.PDF.CabecalhoRodape
{
	public class PdfCabecalhoRodapePecaTecnica : PdfPageEventHelper, IPageMirroring
	{
		private int _pagina = 1;
		private bool _isPageMirroring = true;

		public bool IsPageMirroring
		{
			get { return _isPageMirroring; }
			set { _isPageMirroring = value; }
		}
		public float LeftMarginOrg { get; set; }
		public float RightMarginOrg { get; set; }

		public override void OnOpenDocument(PdfWriter writer, Document document)
		{
			LeftMarginOrg = document.LeftMargin;
			RightMarginOrg = document.RightMargin;
		}

		public override void OnEndPage(PdfWriter writer, Document document)
		{
			_pagina++;

			if (IsPageMirroring)
			{
				//pagina			
				if ((_pagina) % 2 == 0)
				{
					//Invert
					document.SetMargins(document.RightMargin, document.LeftMargin, document.TopMargin, document.BottomMargin);
				}
				else
				{
					document.SetMargins(LeftMarginOrg, RightMarginOrg, document.TopMargin, document.BottomMargin);
				}
			}
		}
	}
}
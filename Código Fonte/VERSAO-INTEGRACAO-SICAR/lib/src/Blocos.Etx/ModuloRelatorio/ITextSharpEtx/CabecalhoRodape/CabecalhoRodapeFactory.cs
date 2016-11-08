

using iTextSharp.text.pdf;

namespace Tecnomapas.Blocos.Etx.ModuloRelatorio.ITextSharpEtx.CabecalhoRodape
{
	public static class CabecalhoRodapeFactory
	{
		public static PdfPageEventHelper Criar(int setor = 0, bool carimbo = false)
		{
			ICabecalhoRodape cab = new PdfCabecalhoRodape();

			if (carimbo)
			{
				cab = new PdfCabecalhoRodapeUsoOrgao();
			}
			
			CabecalhoRodapeBus bus = new CabecalhoRodapeBus();
			
			if (setor > 0)
			{
				cab = bus.ObterEnderecoSetor(cab, setor);
				return cab as PdfPageEventHelper;
			}

			cab = bus.ObterEnderecoDefault(cab);
			return cab as PdfPageEventHelper;
		}
	}
}

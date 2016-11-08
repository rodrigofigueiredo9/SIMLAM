using System;

namespace Tecnomapas.EtramiteX.WindowsService.ProcessOperacoesGeo.Business.PDF.CabecalhoRodape
{
	public interface IPageMirroring
	{
		bool IsPageMirroring { get; set; }
		float LeftMarginOrg { get; set; }
		float RightMarginOrg { get; set; }
	}
}
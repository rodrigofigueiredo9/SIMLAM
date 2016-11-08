using System;
using System.IO;
using Tecnomapas.Blocos.Entities.Interno.ModuloFichaFundiaria;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFichaFundiaria.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloFichaFundiaria.Pdf
{
	public class FichaFundiariaPDF : PdfPadraoRelatorio
	{
		public MemoryStream Gerar(int id)
		{
			ArquivoDocCaminho = @"~/Content/_pdfAspose/Ficha fundiaria.docx";

			ObterArquivoTemplate();

			FichaFundiaria dataSource = new FichaFundiariaDa().Obter(id);

			#region Configurar Cabecario Rodapé

			Int32 setorId = new EnderecoBus().ObterSetorId("DTCAR");

			ConfigurarCabecarioRodape(setorId);

			#endregion

			return GerarPdf(dataSource);
		}
	}
}
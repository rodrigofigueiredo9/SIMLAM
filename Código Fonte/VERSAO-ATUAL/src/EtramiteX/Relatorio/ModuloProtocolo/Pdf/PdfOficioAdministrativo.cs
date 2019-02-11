using System.IO;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloProtocolo;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx;
using Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloProtocolo.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloProtocolo.Pdf
{
	public class PdfOficioAdministrativo : PdfPadraoRelatorio
	{
		public MemoryStream Gerar(int id)
		{
			ArquivoDocCaminho = @"~/Content/_pdfAspose/Oficio_Administrativo.docx";

			OficioAdministrativoDa _da = new OficioAdministrativoDa();

			ObterArquivoTemplate();

			ProtocoloRelatorio dataSource = _da.Obter(id);

			#region Configurar Cabecario Rodapé

			ConfigurarCabecarioRodape(dataSource.SetorId);

			#endregion

			dataSource.Titulo = "Ofício (Administrativo)";
			
			return GerarPdf(dataSource);
		}
	}
}

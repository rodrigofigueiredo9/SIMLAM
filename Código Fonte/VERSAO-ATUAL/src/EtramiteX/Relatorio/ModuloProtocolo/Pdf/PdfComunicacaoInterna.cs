using System.Collections.Generic;
using System.IO;
using Aspose.Words.Tables;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloProtocolo;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx;
using Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloProtocolo.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloProtocolo.Pdf
{
	public class PdfComunicacaoInterna : PdfPadraoRelatorio
	{
		public MemoryStream Gerar(int id)
		{
			ArquivoDocCaminho = @"~/Content/_pdfAspose/ComunicacaoInterna.docx";

			ComunicacaoInternaDa _da = new ComunicacaoInternaDa();

			ObterArquivoTemplate();

			ProtocoloRelatorio dataSource = _da.Obter(id);

			#region Configurar Cabecario Rodapé

			ConfigurarCabecarioRodape(dataSource.SetorId);

			#endregion

			dataSource.Titulo = "Comunicação Interna";
			
			return GerarPdf(dataSource);
		}
	}
}

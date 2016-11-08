using System.Collections.Generic;
using System.IO;
using System.Linq;
using Aspose.Words.Tables;
using Tecnomapas.Blocos.Entities.Etx.ModuloRelatorio;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx;
using Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloCredenciado.Data;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloCredenciado;
using System;
using System.Web;

namespace Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloCredenciado.Pdf
{
	public class PdfHabilitarEmissaoCFOCFOC : PdfPadraoRelatorio
	{
		RelatorioHabilitarEmissaoCFOCFOCDa _da;

		public PdfHabilitarEmissaoCFOCFOC(RelatorioHabilitarEmissaoCFOCFOCDa da = null)
		{
			if (da == null)
			{
				_da = new RelatorioHabilitarEmissaoCFOCFOCDa();
			}
			else
			{
				_da = da;
			}
		}

		public MemoryStream Gerar(int id)
		{
			ArquivoDocCaminho = @"~/Content/_pdfAspose/Termo_Habilitacao_RT.docx";

			HabilitarEmissaoCFOCFOCRelatorio dataSource = new HabilitarEmissaoCFOCFOCRelatorio();

			dataSource = _da.Obter(id);

			ObterArquivoTemplate();

			ConfigurarCabecarioRodape(0);

			#region Configurar

			ConfiguracaoDefault.AddLoadAcao((doc, dataSourceCnf) =>
			{
				List<Row> itensRemover = new List<Row>();

				if (dataSource.Foto != null && dataSource.Foto.Buffer != null && !String.IsNullOrWhiteSpace(dataSource.Foto.Caminho))
				{
					dataSource.Foto.Conteudo = AsposeImage.RedimensionarImagem(File.ReadAllBytes(dataSource.Foto.Caminho), 3.5f, eAsposeImageDimensao.Ambos);
				}
				else
				{
					var cam = HttpContext.Current.Server.MapPath(@"~/Content/_img/foto3x4.jpg");

					dataSource.Foto.Conteudo = AsposeImage.RedimensionarImagem(File.ReadAllBytes(cam), 3.5f, eAsposeImageDimensao.Ambos);
				}
				itensRemover.ForEach(x => x.Remove());
			});

			#endregion

			var pagecount = GerarPdfDoc(dataSource, "DocumentoAnexo");

			dataSource.NumeroPaginasAnexo = (pagecount - 1).ToString();

			return GerarPdf(dataSource, "DocumentoAnexo");
		}
	}
}
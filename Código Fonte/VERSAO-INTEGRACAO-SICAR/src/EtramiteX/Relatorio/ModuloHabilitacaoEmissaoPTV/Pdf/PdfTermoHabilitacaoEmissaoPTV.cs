using Aspose.Words.Tables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloHabilitacaoEmissaoPTV;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx;
using Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloHabilitacaoEmissaoPTV.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloHabilitacaoEmissaoPTV.Pdf
{
	public class PdfTermoHabilitacaoEmissaoPTV : PdfPadraoRelatorio
	{
		HabilitacaoEmissaoDa _da = new HabilitacaoEmissaoDa();

		public MemoryStream Gerar(int id)
		{
			ArquivoDocCaminho = @"~/Content/_pdfAspose/Termo_Habilitacao_Func.docx";

			HabilitacaoEmissaoPTVRelatorio dataSource = new HabilitacaoEmissaoPTVRelatorio();

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

			return GerarPdf(dataSource);
		}
	}
}
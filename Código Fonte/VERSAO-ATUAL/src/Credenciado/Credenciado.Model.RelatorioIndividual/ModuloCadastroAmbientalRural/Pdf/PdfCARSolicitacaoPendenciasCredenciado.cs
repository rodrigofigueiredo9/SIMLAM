using System;
using System.IO;
using Tecnomapas.Blocos.Entities.Etx.ModuloRelatorio.AsposeEtx;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade.PDF;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloCadastroAmbientalRural;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.ITextSharpEtx;
using Tecnomapas.EtramiteX.Credenciado.Model.RelatorioIndividual.ModuloCadastroAmbientalRural.Data;
using Tecnomapas.EtramiteX.Credenciado.Model.RelatorioIndividual.ModuloCaracterizacao.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.RelatorioIndividual.ModuloCadastroAmbientalRural.Pdf
{
	public class PdfCARSolicitacaoPendenciasCredenciado : PdfPadraoRelatorio
	{
		CARSolicitacaoCredenciadoDa _da;

		public PdfCARSolicitacaoPendenciasCredenciado(CARSolicitacaoCredenciadoDa da = null)
		{
			_da = da ?? new CARSolicitacaoCredenciadoDa();
		}

		public MemoryStream Gerar(int id)
		{
			ArquivoDocCaminho = @"~/Content/_pdfAspose/Pendencias_SICAR.doc";

			CARSolicitacaoRelatorio dataSource = new CARSolicitacaoRelatorio();

			var situacao = _da.ObterSituacao(id);
			if (situacao == (int)eCARSolicitacaoSituacaoRelatorio.EmCadastro
				|| situacao == (int)eCARSolicitacaoSituacaoRelatorio.Pendente)
			{
				dataSource = _da.Obter(id);
			}
			else
			{
				dataSource = _da.ObterHistorico(id);
			}

			ObterArquivoTemplate();

			ConfigurarCabecarioRodape(0, true);

			#region Configurar Tabelas

			ConfiguracaoDefault.ExibirSimplesConferencia = false;
			ConfiguracaoDefault.AddLoadAcao((doc, dataSrc) =>
			{
			});

			#endregion

			MemoryStream stream = GerarPdf(dataSource);

			return stream;
		}
	}
}
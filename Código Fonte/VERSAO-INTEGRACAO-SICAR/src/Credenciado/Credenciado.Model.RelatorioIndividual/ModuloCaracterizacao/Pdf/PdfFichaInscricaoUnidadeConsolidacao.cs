using Aspose.Words.Tables;
using System.Collections.Generic;
using System.IO;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloProjetoDigital;
using Tecnomapas.Blocos.Entities.Etx.ModuloRelatorio;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloEmpreendimento;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloUnidadeConsolidacao;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx;
using Tecnomapas.EtramiteX.Credenciado.Model.RelatorioIndividual.ModuloCaracterizacao.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.RelatorioIndividual.ModuloCaracterizacao.Pdf
{
	public class PdfFichaInscricaoUnidadeConsolidacao : PdfPadraoRelatorio
	{
		UnidadeConsolidacaoDa _da;

		public PdfFichaInscricaoUnidadeConsolidacao(UnidadeConsolidacaoDa da = null)
		{
			_da = da ?? new UnidadeConsolidacaoDa();
		}

		public MemoryStream Gerar(int projetoDigitalId)
		{
			UnidadeConsolidacaoRelatorio dataSource = _da.Obter(projetoDigitalId);

			ArquivoDocCaminho = @"~/Content/_pdfAspose/Ficha_Inscrição_UC.docx";

			ObterArquivoTemplate();

			#region Configurar Assinantes

			ConfiguracaoDefault.TextoTagAssinante = "«Assinante.Nome»";
			ConfiguracaoDefault.TextoTagAssinantes1 = "«TableStart:Assinantes1»";
			ConfiguracaoDefault.TextoTagAssinantes2 = "«TableStart:Assinantes2»";

			#endregion

			ConfiguracaoDefault.ExibirSimplesConferencia = (dataSource.Situacao.Equals((int)eProjetoDigitalSituacao.EmElaboracao));
			ConfigurarCabecarioRodape(0, true);

			#region Configurar Tabelas

			ConfiguracaoDefault.AddLoadAcao((doc, dataSrc) =>
			{
			});

			#endregion

			#region Assinantes

			AssinanteDefault assinante = null;

			foreach (ResponsavelRelatorio responsavel in dataSource.Empreendimento.Responsaveis)
			{
				assinante = new AssinanteDefault();
				assinante.Nome = responsavel.NomeRazao;
				assinante.TipoTexto = "Representante Legal";
				ConfiguracaoDefault.Assinantes.Add(assinante);
			}

			foreach (ResponsavelRelatorio responsavel in dataSource.ResponsaveisTecnicos)
			{
				assinante = new AssinanteDefault();
				assinante.Nome = responsavel.NomeRazao;
				assinante.TipoTexto = "Responsável Técnico";
				ConfiguracaoDefault.Assinantes.Add(assinante);
			}

			#endregion

			return GerarPdf(dataSource);
		}
	}
}
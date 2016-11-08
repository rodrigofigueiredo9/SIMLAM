using System.Collections.Generic;
using System.IO;
using System.Linq;
using Aspose.Words.Tables;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloProjetoDigital;
using Tecnomapas.Blocos.Entities.Etx.ModuloRelatorio;
using Tecnomapas.Blocos.Entities.Interno.ModuloRequerimento;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloRequerimento;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx;
using Tecnomapas.EtramiteX.Credenciado.Model.RelatorioIndividual.ModuloRequerimento.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.RelatorioIndividual.ModuloRequerimento.Pdf
{
	public class PdfRequerimentoPadraoCredenciado : PdfPadraoRelatorio
	{
		public MemoryStream Gerar(int id, out bool isCredenciado, bool isInterno = false)
		{
			RequerimentoRelatorio dataSource;

			if(isInterno)
			{
				RequerimentoRelatorioInternoDa da = new RequerimentoRelatorioInternoDa();
				dataSource = da.Obter(id);

				ConfiguracaoDefault.ExibirSimplesConferencia = dataSource.SituacaoId == (int)eRequerimentoSituacao.EmAndamento;
			}
			else
			{
				RequerimentoRelatorioCredenciadoDa da = new RequerimentoRelatorioCredenciadoDa();
				dataSource = da.Obter(id);

				ConfiguracaoDefault.ExibirSimplesConferencia = dataSource.ProjetoDigitalSituacaoId == (int)eProjetoDigitalSituacao.EmElaboracao;
			}

			if (dataSource.IsCredenciado)
			{
				ArquivoDocCaminho = @"~/Content/_pdfAspose/Requerimento_Digital.doc";
			}
			else
			{
				ArquivoDocCaminho = @"~/Content/_pdfAspose/Requerimento.doc";
			}

			ObterArquivoTemplate();

			#region Configurar Assinantes

			ConfiguracaoDefault.TextoTagAssinante = "«Assinante.Nome»";
			ConfiguracaoDefault.TextoTagAssinantes1 = "«TableStart:Assinantes1»";
			ConfiguracaoDefault.TextoTagAssinantes2 = "«TableStart:Assinantes2»";

			#endregion

			#region Configurar Cabecalho Rodapé

			ConfigurarCabecarioRodape(dataSource.SetorId, dataSource.IsCredenciado);

			#endregion

			#region Configurar Tabelas

			ConfiguracaoDefault.AddLoadAcao((doc, dataSrc) =>
			{
				List<Table> tabelas = new List<Table>();

				List<RequerimentoAtividadeRelatorio> atividades = dataSource.Atividades.Where(x => !string.IsNullOrEmpty(x.Conclusao)).ToList();

				if (atividades == null || atividades.Count <= 0)
				{
					tabelas.Add(doc.LastTable("«TableStart:Atividades»"));
				}

				AsposeExtensoes.RemoveTables(tabelas);
			});

			ConfiguracaoDefault.AddExecutedAcao((doc, dataSrc) =>
			{
				List<Table> tabelas = new List<Table>();

				doc.FindTable("OBJETIVO DO PEDIDO").RemoverParagrafos();

				doc.FindTable("IDENTIFICAÇÃO DO RESPONSÁVEL TÉCNICO").RemoverParagrafos();

				if (dataSource.Interessado.Id == 0)
				{
					tabelas.Add(doc.FindTable("IDENTIFICAÇÃO DO INTERESSADO"));
				}

				if (dataSource.Responsaveis == null || dataSource.Responsaveis.Count == 0)
				{
					tabelas.Add(doc.FindTable("IDENTIFICAÇÃO DO RESPONSÁVEL TÉCNICO"));
				}

				if (dataSource.Empreendimento.Id == 0)
				{
					tabelas.Add(doc.FindTable("IDENTIFICAÇÃO DO EMPREENDIMENTO"));
				}

				AsposeExtensoes.RemoveTables(tabelas);
			});

			#endregion

			#region Assinantes

			AssinanteDefault assinante = null;

			if (dataSource.Interessado.Id > 0)
			{
				assinante = new AssinanteDefault();
				assinante.Nome = dataSource.Interessado.NomeRazaoSocial;
				assinante.TipoTexto = "Interessado";
				ConfiguracaoDefault.Assinantes.Add(assinante);
			}

			foreach (ResponsavelTecnicoRelatorio responsavel in dataSource.Responsaveis)
			{
				assinante = new AssinanteDefault();
				assinante.Nome = responsavel.NomeRazao;
				assinante.TipoTexto = "Responsável Técnico";
				ConfiguracaoDefault.Assinantes.Add(assinante);
			}

			#endregion

			isCredenciado = dataSource.IsCredenciado;
			return GerarPdf(dataSource);
		}
	}
}
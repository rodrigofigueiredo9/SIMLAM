using System.Collections.Generic;
using System.IO;
using System.Linq;
using Aspose.Words.Tables;
using Tecnomapas.Blocos.Entities.Etx.ModuloRelatorio;
using Tecnomapas.Blocos.Entities.Interno.ModuloRequerimento;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloRequerimento;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx;
using Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloRequerimento.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloRequerimento.Pdf
{
	public class PdfRequerimentoDigital : PdfPadraoRelatorio
	{
		IRelatorioRequerimentoDa _da;

		public PdfRequerimentoDigital(IRelatorioRequerimentoDa da = null)
		{
			if (da == null)
			{
				_da = new RelatorioRequerimentoDa();
			}
			else
			{
				_da = da;
			}
		}

		public MemoryStream Gerar(int id)
		{
			ArquivoDocCaminho = @"~/Content/_pdfAspose/Requerimento_Digital.doc";

			RequerimentoRelatorio dataSource = _da.Obter(id);

			if (_da.ObterSituacao(id) == (int)eRequerimentoSituacao.EmAndamento)
			{
				dataSource = _da.Obter(id);
			}
			else
			{
				ConfiguracaoDefault.ExibirSimplesConferencia = false;
				dataSource = _da.ObterHistorico(id);
			}

			ObterArquivoTemplate();

			#region Configurar Assinantes

			ConfiguracaoDefault.TextoTagAssinante = "«Assinante.Nome»";
			ConfiguracaoDefault.TextoTagAssinantes1 = "«TableStart:Assinantes1»";
			ConfiguracaoDefault.TextoTagAssinantes2 = "«TableStart:Assinantes2»";

			#endregion

			#region Configurar Cabecalho Rodapé

			ConfigurarCabecarioRodape(dataSource.SetorId, true);

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

			return GerarPdf(dataSource);
		}
	}
}

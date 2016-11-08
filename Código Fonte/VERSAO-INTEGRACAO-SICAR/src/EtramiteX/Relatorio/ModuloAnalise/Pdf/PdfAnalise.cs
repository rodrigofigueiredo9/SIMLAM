using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloAnalise;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.ITextSharpEtx;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.ITextSharpEtx.CabecalhoRodape;
using Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloAnalise.Data;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx;
using Aspose.Words.Tables;

namespace Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloAnalise.Pdf
{
	public class PdfAnalise : PdfPadraoRelatorio
	{
		AnaliseDa _da = new AnaliseDa();

		public int Existe(int protocolo)
		{
			return _da.Existe(protocolo);
		}

		public MemoryStream GerarPDFAnaliseProtocolo(int protocolo, int tipo)
		{
			return GerarPDFAnalise(Existe(protocolo));
		}

		public MemoryStream GerarPDFAnalise(int analiseId)
		{
			ArquivoDocCaminho = @"~/Content/_pdfAspose/Analise_itens_Processo_Documento.doc";
			AnaliseItemRelatorio analise = _da.Obter(analiseId);

			if (analise.Itens != null)
			{
				analise.ItensTecnicos = analise.Itens.Where(x => x.Tipo == 1).ToList();
				analise.ItensAdministrativos = analise.Itens.Where(x => x.Tipo == 2).ToList();
				analise.ItensProjetoDigital = analise.Itens.Where(x => x.Tipo == 3).ToList();
			}


			ObterArquivoTemplate();

			#region Configurar Cabecalho Rodapé

			ConfigurarCabecarioRodape(analise.SetorId);

			#endregion

			#region Configurar Tabelas

			ConfiguracaoDefault.AddLoadAcao((doc, dataSrc) =>
			{
				List<Table> tabelas = new List<Table>();

				if (analise.ItensTecnicos == null || analise.ItensTecnicos.Count <= 0)
				{
					tabelas.Add(doc.LastTable("ITENS TÉCNICOS"));
					tabelas.Add(doc.LastTable("«TableStart:ItensTecnicos»"));
				}

				if (analise.ItensAdministrativos == null || analise.ItensAdministrativos.Count <= 0)
				{
					tabelas.Add(doc.LastTable("ITENS ADMINISTRATIVOS"));
					tabelas.Add(doc.LastTable("«TableStart:ItensAdministrativos»"));
				}

				if (analise.ItensProjetoDigital == null || analise.ItensProjetoDigital.Count <= 0)
				{
					tabelas.Add(doc.LastTable("ITENS DO PROJETO DIGITAL"));
					tabelas.Add(doc.LastTable("«TableStart:ItensProjetoDigital»"));
				}

				AsposeExtensoes.RemoveTables(tabelas);
			});

			#endregion


			return GerarPdf(analise);
		}


	}
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Aspose.Words.Tables;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloRoteiro;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Credenciado.Model.RelatorioIndividual.ModuloRoteiro.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.RelatorioIndividual.ModuloRoteiro.Pdf
{
	public class PdfRoteiroOrientativoInterno : PdfPadraoRelatorio
	{
		#region Propriedades

		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

		public String EsquemaBanco
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioInterno); }
		}

		#endregion

		public MemoryStream Gerar(int id, string tid)
		{
			ArquivoDocCaminho = @"~/Content/_pdfAspose/Roteiro.doc";

			RoteiroRelatorioInternoDa _da = new RoteiroRelatorioInternoDa(EsquemaBanco);
			RoteiroRelatorio dataSource = (tid == string.Empty) ? _da.Obter(id) : _da.Obter(id, tid);
			dataSource.ObservacoesHtml = dataSource.Observacoes;

			dataSource.ItensTecnicos = dataSource.Itens.Where(x => x.Tipo == 1).OrderBy(x => x.Ordem).ToList();
			dataSource.ItensAdministrativos = dataSource.Itens.Where(x => x.Tipo == 2).OrderBy(x => x.Ordem).ToList();

			int num = 1;
			dataSource.ItensTecnicos.ForEach(x => x.Numero = num++);
			num = 1;
			dataSource.ItensAdministrativos.ForEach(x => x.Numero = num++);

			ObterArquivoTemplate();

			#region Configurar Assinantes

			ConfiguracaoDefault.TextoTagAssinante = "«Assinante.Nome»";
			ConfiguracaoDefault.TextoTagAssinantes1 = "«TableStart:Assinantes1»";
			ConfiguracaoDefault.TextoTagAssinantes2 = "«TableStart:Assinantes2»";

			#endregion

			#region Configurar Cabecalho Rodapé

			ConfigurarCabecarioRodape(dataSource.SetorId);

			#endregion

			#region Configurar Tabelas

			ConfiguracaoDefault.AddLoadAcao((doc, dataSrc) =>
			{
				List<Table> tabelas = new List<Table>();

				if (dataSource.ItensTecnicos == null || dataSource.ItensTecnicos.Count <= 0)
				{
					tabelas.Add(doc.LastTable("«TableStart:ItensTecnicos»"));
				}

				if (dataSource.ItensAdministrativos == null || dataSource.ItensAdministrativos.Count <= 0)
				{
					tabelas.Add(doc.LastTable("«TableStart:ItensAdministrativos»"));
				}

				if (dataSource.Anexos == null || dataSource.Anexos.Count <= 0)
				{
					tabelas.Add(doc.FindTable("«TableEnd:Anexos»"));
				}

				if (string.IsNullOrEmpty(dataSource.ObservacoesHtml))
				{
					tabelas.Add(doc.FindTable("«ObservacoesHtml»"));
				}

				AsposeExtensoes.RemoveTables(tabelas);
			});

			#endregion

			return MergePdf(GerarPdf(dataSource), dataSource.Anexos.Select(x => x.Arquivo).ToList());
		}
	}
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using Aspose.Words.Tables;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloTramitacao;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloTramitacao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloTramitacao.Pdf
{
	public class PdfTramitacao : PdfPadraoRelatorio
	{
		#region Propriedades

		private TramitacaoDa _da = new TramitacaoDa();
		private TramitacaoDa _HistoricoDa = new TramitacaoDa();
		private GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

		#endregion

		public MemoryStream Gerar(Int32 id, Int32 tipo, Boolean obterHistorico)
		{
			ArquivoDocCaminho = @"~/Content/_pdfAspose/Despacho.doc";
			TramitacaoRelatorioPDF dataSource;

			if (obterHistorico)
			{
				dataSource = _da.ObterHistorico(id);
			}
			else
			{
				dataSource = _da.Obter(id);
			}

			ObterArquivoTemplate();

			dataSource.GovernoNome = _configSys.Obter<String>(ConfiguracaoSistema.KeyGovernoNome);
			dataSource.SecretariaNome = _configSys.Obter<String>(ConfiguracaoSistema.KeySecretariaNome);
			dataSource.OrgaoNome = _configSys.Obter<String>(ConfiguracaoSistema.KeyOrgaoNome);

			string pathImg = HttpContext.Current.Request.MapPath("~/Content/_imgLogo/logobrasao.jpg");
			dataSource.LogoBrasao = File.ReadAllBytes(pathImg);

			dataSource.LogoBrasao = AsposeImage.RedimensionarImagem(dataSource.LogoBrasao, 1);

			return GerarPdf(dataSource);
		}

		//cria um Stream de saida que mantenha o fluxo aberto.
		public MemoryStream Gerar2(Int32 id, Int32 tipo, Boolean obterHistorico)
		{
			ArquivoDocCaminho = @"~/Content/_pdfAspose/Despacho.doc";
			TramitacaoRelatorioPDF dataSource;

			if (obterHistorico)
			{
				dataSource = _da.ObterHistorico(id);
			}
			else
			{
				dataSource = _da.Obter(id);
			}

			ObterArquivoTemplate();

			dataSource.GovernoNome = _configSys.Obter<String>(ConfiguracaoSistema.KeyGovernoNome);
			dataSource.SecretariaNome = _configSys.Obter<String>(ConfiguracaoSistema.KeySecretariaNome);
			dataSource.OrgaoNome = _configSys.Obter<String>(ConfiguracaoSistema.KeyOrgaoNome);

			string pathImg = HttpContext.Current.Request.MapPath("~/Content/_imgLogo/logobrasao.jpg");
			dataSource.LogoBrasao = File.ReadAllBytes(pathImg);

			dataSource.LogoBrasao = AsposeImage.RedimensionarImagem(dataSource.LogoBrasao, 1);

			MemoryStream ms = GerarPdf(dataSource);
			ms.Seek(0, SeekOrigin.Begin);

			return ms;
		}

		public MemoryStream GerarHistorico(Int32 protocolo, Int32 tipo)
		{
			ArquivoDocCaminho = @"~/Content/_pdfAspose/Historico_Tramitacao.doc";
			TramitacaoHistoricoRelatorioPDF dataSource;

			dataSource = _HistoricoDa.ObterTramitacaoHistorico(protocolo);

			ObterArquivoTemplate();

			dataSource.GovernoNome = _configSys.Obter<String>(ConfiguracaoSistema.KeyGovernoNome);
			dataSource.SecretariaNome = _configSys.Obter<String>(ConfiguracaoSistema.KeySecretariaNome);
			dataSource.OrgaoNome = _configSys.Obter<String>(ConfiguracaoSistema.KeyOrgaoNome);

			string pathImg = HttpContext.Current.Request.MapPath("~/Content/_imgLogo/logobrasao.jpg");
			dataSource.LogoBrasao = File.ReadAllBytes(pathImg);

			dataSource.LogoBrasao = AsposeImage.RedimensionarImagem(dataSource.LogoBrasao, 1);

			#region Remover

			ConfiguracaoDefault.AddExecutedAcao((doc, dataSrc) =>
			{
				List<Table> tabelasRemover = new List<Table>();
				tabelasRemover.AddRange(doc.Any<Table>("«remover»", isDeep: true));

				AsposeExtensoes.RemoveTables(tabelasRemover);
			});

			#endregion Remover

			return GerarPdf(dataSource);
		}
	}
}
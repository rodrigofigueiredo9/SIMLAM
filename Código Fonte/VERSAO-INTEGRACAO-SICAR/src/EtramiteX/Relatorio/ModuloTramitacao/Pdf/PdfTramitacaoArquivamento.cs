using System;
using System.IO;
using System.Web;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloTramitacao;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloTramitacao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloTramitacao.Pdf
{
	public class PdfTramitacaoArquivamento : PdfPadraoRelatorio
	{
		#region Propriedades

		private TramitacaoDa _da = new TramitacaoDa();
		private GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

		#endregion

		public int Existe(int protocolo)
		{
			return _da.ExisteProcDocArquivado(protocolo);
		}

		public MemoryStream Gerar(Int32 id, Int32 tipo, Boolean obterHistorico)
		{
			TramitacaoRelatorioPDF dataSource;

			ArquivoDocCaminho = @"~/Content/_pdfAspose/Arquivamento.doc";

			if (obterHistorico)
			{
				dataSource = _da.ObterHistorico(id);
			}
			else
			{
				dataSource = _da.Obter(Existe(id));
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
	}
}

using System.IO;
using System.Linq;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloProjetoDigital;
using Tecnomapas.Blocos.Entities.Etx.ModuloRelatorio;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloEmpreendimento;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloRequerimento;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloUnidadeProducao;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx;
using Tecnomapas.EtramiteX.Credenciado.Model.RelatorioIndividual.ModuloCaracterizacao.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.RelatorioIndividual.ModuloCaracterizacao.Pdf
{
	public class PdfFichaInscricaoUnidadeProducao : PdfPadraoRelatorio
	{
		UnidadeProducaoDa _da;

		public PdfFichaInscricaoUnidadeProducao(UnidadeProducaoDa da = null)
		{
			_da = da ?? new UnidadeProducaoDa();
		}

		public MemoryStream Gerar(int projetoDigitalId)
		{
			UnidadeProducaoRelatorio dataSource = _da.Obter(projetoDigitalId);

			ArquivoDocCaminho = @"~/Content/_pdfAspose/Ficha_Inscricao_UP.docx";

			ObterArquivoTemplate();

			#region Configurar Assinantes

			ConfiguracaoDefault.TextoTagAssinante = "«Assinante.Nome»";
			ConfiguracaoDefault.TextoTagAssinantes1 = "«TableStart:Assinantes1»";
			ConfiguracaoDefault.TextoTagAssinantes2 = "«TableStart:Assinantes2»";

			#endregion

			ConfiguracaoDefault.ExibirSimplesConferencia = (dataSource.Situacao == eProjetoDigitalSituacao.EmElaboracao);
			ConfigurarCabecarioRodape(0, true);

			#region Configurar Tabelas

			ConfiguracaoDefault.AddLoadAcao((doc, dataSrc) =>
			{
			});

			#endregion

			#region Assinantes

			AssinanteDefault assinante = null;

			foreach (PessoaRelatorio item in dataSource.Produtores)
			{
				assinante = new AssinanteDefault();
				assinante.Nome = item.NomeRazaoSocial;
				assinante.TipoTexto = "Produtor";
				ConfiguracaoDefault.Assinantes.Add(assinante);
			}

			foreach (ResponsavelRelatorio item in dataSource.Empreendimento.Responsaveis)
			{
				assinante = new AssinanteDefault();
				assinante.Nome = item.NomeRazao;
				assinante.TipoTexto = "Representante Legal";
				ConfiguracaoDefault.Assinantes.Add(assinante);
			}

			foreach (ResponsavelTecnicoRelatorio item in dataSource.Responsaveis)
			{
				assinante = new AssinanteDefault();
				assinante.Nome = item.NomeRazao;
				assinante.TipoTexto = "Responsável Técnico";
				ConfiguracaoDefault.Assinantes.Add(assinante);
			}

			#endregion

			return GerarPdf(dataSource);
		}
	}
}
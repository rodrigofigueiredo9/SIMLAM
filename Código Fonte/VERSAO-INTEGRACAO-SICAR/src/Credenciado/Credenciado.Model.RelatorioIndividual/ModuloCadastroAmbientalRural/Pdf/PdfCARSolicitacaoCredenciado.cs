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
	public class PdfCARSolicitacaoCredenciado: PdfPadraoRelatorio
	{
		CARSolicitacaoCredenciadoDa _da;

		public PdfCARSolicitacaoCredenciado(CARSolicitacaoCredenciadoDa da = null)
		{
			_da = da ?? new CARSolicitacaoCredenciadoDa();
		}

		public MemoryStream Gerar(int id, int situacao, string situacaoTexto)
		{
			ArquivoDocCaminho = @"~/Content/_pdfAspose/SolicitacaoInscricaoCAR_Credenciado.docx";

			CARSolicitacaoRelatorio dataSource = new CARSolicitacaoRelatorio();

			dataSource = _da.ObterHistorico(id);

			dataSource.DominialidadePDF = new DominialidadePDF(new DominialidadeRelatorioDa().Obter(dataSource.DominialidadeId, tid: dataSource.DominialidadeTid));

			ObterArquivoTemplate();

			ConfigurarCabecarioRodape(0, true);

			#region Configurar Tabelas

			ConfiguracaoDefault.ExibirSimplesConferencia = (situacao == (int)eCARSolicitacaoSituacaoRelatorio.EmCadastro || situacao == (int)eCARSolicitacaoSituacaoRelatorio.Suspenso || situacao == (int)eCARSolicitacaoSituacaoRelatorio.Pendente);
			ConfiguracaoDefault.AddLoadAcao((doc, dataSrc) =>
			{
				if (string.IsNullOrEmpty(dataSource.Declarante.TipoTexto))
				{
					dataSource.Declarante.TipoTexto = AsposeData.Empty;
					dataSource.DoisPontos = AsposeData.Empty;
				}

				if (!dataSource.DeclarantePossuiOutros)
				{
					dataSource.DeclaranteOutros = AsposeData.Empty;
				}
			});

			#endregion

			MemoryStream stream = GerarPdf(dataSource);

			#region Adicionar Tarja

			try
			{
				DateTime dataAtual = DateTime.Now;
				String mensagemTarja = "Consultado em " + dataAtual.ToShortDateString() + " às " + dataAtual.ToString(@"HH\hmm\min");

				switch ((eCARSolicitacaoSituacaoRelatorio)situacao)
				{
					case eCARSolicitacaoSituacaoRelatorio.Valido:
						using (MemoryStream msTemp = new MemoryStream(stream.ToArray()))
						{
							stream.Close();
							stream.Dispose();
							stream = PdfMetodosAuxiliares.TarjaVerde(msTemp, mensagemTarja, situacaoTexto);
						}
						break;

					case eCARSolicitacaoSituacaoRelatorio.Suspenso:
						using (MemoryStream msTemp = new MemoryStream(stream.ToArray()))
						{
							stream.Close();
							stream.Dispose();
							stream = PdfMetodosAuxiliares.TarjaLaranjaEscuro(msTemp, mensagemTarja, situacaoTexto);
						}
						break;

					case eCARSolicitacaoSituacaoRelatorio.SubstituidoTituloCAR:
						using (MemoryStream msTemp = new MemoryStream(stream.ToArray()))
						{
							stream.Close();
							stream.Dispose();
							stream = PdfMetodosAuxiliares.TarjaLaranja(msTemp, mensagemTarja, situacaoTexto);
						}
						break;

					case eCARSolicitacaoSituacaoRelatorio.Invalido:
						using (MemoryStream msTemp = new MemoryStream(stream.ToArray()))
						{
							stream.Close();
							stream.Dispose();
							stream = PdfMetodosAuxiliares.TarjaVermelha(msTemp, mensagemTarja, situacaoTexto);
						}
						break;

					default:
						break;
				}
			}
			catch
			{
				if (stream != null)
				{
					stream.Close();
					stream.Dispose();
				}
				throw;
			}

			#endregion

			return stream;
		}
	}
}
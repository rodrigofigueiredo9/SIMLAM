using System;
using System.IO;
using Tecnomapas.Blocos.Arquivo;
using Tecnomapas.Blocos.Arquivo.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloRelatorio.AsposeEtx;
using Tecnomapas.Blocos.Entities.Interno.ModuloCadastroAmbientalRural;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloCadastroAmbientalRural;
using Tecnomapas.Blocos.Etx.ModuloArquivo.Business;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.ITextSharpEtx;
using Tecnomapas.EtramiteX.Credenciado.Model.RelatorioIndividual.ModuloCadastroAmbientalRural.Data;
using Tecnomapas.EtramiteX.Credenciado.Model.RelatorioIndividual.ModuloCaracterizacao.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.RelatorioIndividual.ModuloCadastroAmbientalRural.Pdf
{
	public class PdfCARSolicitacaoInterno : PdfPadraoRelatorio
	{
		CARSolicitacaoInternoDa _da;
		ArquivoBus _busArquivo;

		public PdfCARSolicitacaoInterno(CARSolicitacaoInternoDa da = null)
		{
			_da = da ?? new CARSolicitacaoInternoDa();
			_busArquivo = new ArquivoBus(eExecutorTipo.Interno);
		}

		public MemoryStream Gerar(CARSolicitacao solicitacao)
		{
			MemoryStream stream = new MemoryStream();

			if (solicitacao.Arquivo > 0)
			{
				Arquivo arquivo = _busArquivo.Obter(solicitacao.Arquivo);
				CopyStream(arquivo.Buffer, stream);
			}
			else
			{
				ArquivoDocCaminho = @"~/Content/_pdfAspose/SolicitacaoInscricaoCAR.docx";

				CARSolicitacaoRelatorio dataSource = new CARSolicitacaoRelatorio();
				if (_da.ObterSituacao(solicitacao.Id) == (int)eCARSolicitacaoSituacaoRelatorio.EmCadastro)
					dataSource = _da.Obter(solicitacao.Id);
				else
					dataSource = _da.ObterHistorico(solicitacao.Id);

				dataSource.Dominialidade = new DominialidadeInternoRelatorioDa().Obter(dataSource.DominialidadeId, tid: dataSource.DominialidadeTid);

				ObterArquivoTemplate();
				ConfigurarCabecarioRodape(0);

				dataSource = ConfigurarTabelas(dataSource, solicitacao.SituacaoId);
				stream = GerarPdf(dataSource);

				if (solicitacao.SituacaoId == (int)eCARSolicitacaoSituacao.Valido || solicitacao.SituacaoId == (int)eCARSolicitacaoSituacao.SubstituidoPeloTituloCAR)
					SalvarPdfSolicitacaoCar(stream, solicitacao.Id);
			}
			stream = InserirTarjaPdf(stream, solicitacao);

			return stream;
		}

		private CARSolicitacaoRelatorio ConfigurarTabelas(CARSolicitacaoRelatorio dataSource, int situacaoId)
		{
			ConfiguracaoDefault.ExibirSimplesConferencia = (situacaoId == (int)eCARSolicitacaoSituacaoRelatorio.EmCadastro ||
															situacaoId == (int)eCARSolicitacaoSituacaoRelatorio.Suspenso ||
															situacaoId == (int)eCARSolicitacaoSituacaoRelatorio.Pendente);
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

			return dataSource;
		}

		private MemoryStream InserirTarjaPdf(MemoryStream stream, CARSolicitacao solicitacao)
		{

			try
			{
				DateTime dataAtual = DateTime.Now;
				String mensagemTarja = "Consultado em " + dataAtual.ToShortDateString() + " às " + dataAtual.ToString(@"HH\hmm\min");

				switch ((eCARSolicitacaoSituacaoRelatorio)solicitacao.SituacaoId)
				{
					case eCARSolicitacaoSituacaoRelatorio.Valido:
						using (MemoryStream msTemp = new MemoryStream(stream.ToArray()))
						{
							stream.Close();
							stream.Dispose();
							stream = PdfMetodosAuxiliares.TarjaVerde(msTemp, mensagemTarja, solicitacao.SituacaoTexto);
						}
						break;

					case eCARSolicitacaoSituacaoRelatorio.Suspenso:
						using (MemoryStream msTemp = new MemoryStream(stream.ToArray()))
						{
							stream.Close();
							stream.Dispose();
							stream = PdfMetodosAuxiliares.TarjaLaranjaEscuro(msTemp, mensagemTarja, solicitacao.SituacaoTexto);
						}
						break;

					case eCARSolicitacaoSituacaoRelatorio.SubstituidoTituloCAR:
						using (MemoryStream msTemp = new MemoryStream(stream.ToArray()))
						{
							stream.Close();
							stream.Dispose();
							stream = PdfMetodosAuxiliares.TarjaLaranja(msTemp, mensagemTarja, solicitacao.SituacaoTexto);
						}
						break;

					case eCARSolicitacaoSituacaoRelatorio.Invalido:
						using (MemoryStream msTemp = new MemoryStream(stream.ToArray()))
						{
							string situacaoTarja = solicitacao.SICAR.SituacaoEnvio == eStatusArquivoSICAR.Cancelado ?
								solicitacao.SituacaoTexto + " - Canelado por: " + solicitacao.DescricaoMotivo.Substring(0, 40) : solicitacao.SituacaoTexto;

							stream.Close();
							stream.Dispose();
							stream = PdfMetodosAuxiliares.TarjaVermelha(msTemp, mensagemTarja, situacaoTarja);
						}
						break;

					default:
						break;

				}
				return stream;
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
		}

		private void SalvarPdfSolicitacaoCar(MemoryStream pdf, int solicitacaoId)
		{
			ArquivoDa _arquivoDa = new ArquivoDa();
			Arquivo arquivo = new Arquivo();
			arquivo.Nome = "Solicitação CAR";
			arquivo.Extensao = ".pdf";
			arquivo.ContentType = "application/pdf";
			arquivo.Buffer = pdf;

			_busArquivo.Salvar(arquivo);

			Executor executor = Executor.Current;
			_arquivoDa.Salvar(arquivo, executor.Id, executor.Nome, executor.Login, (int)executor.Tipo, executor.Tid);

			_da.SalvarPdfSolicitacaoCar(solicitacaoId, arquivo.Id ?? 0);
		}

		// Merged From linked CopyStream below and Jon Skeet's ReadFully example
		public static void CopyStream(Stream input, Stream output)
		{
			byte[] buffer = new byte[16 * 1024];
			int read;
			while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
			{
				output.Write(buffer, 0, read);
			}
		}
	}
}
using iTextSharp.text;
using System;
using System.IO;
using Tecnomapas.Blocos.Entities.Etx.ModuloRelatorio.AsposeEtx;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloCadastroAmbientalRural;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.ITextSharpEtx;
using Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloCadastroAmbientalRural.Data;
using Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloCaracterizacoes.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloCadastroAmbientalRural.Pdf
{
	public class PdfCARSolicitacao : PdfPadraoRelatorio
	{
		CARSolicitacaoDa _da;

		public PdfCARSolicitacao(CARSolicitacaoDa da = null)
		{
			_da = da ?? new CARSolicitacaoDa();
		}

		public MemoryStream Gerar(int id, int situacao, string situacaoTexto)
		{
			ArquivoDocCaminho = @"~/Content/_pdfAspose/SolicitacaoInscricaoCAR.docx";

			CARSolicitacaoRelatorio dataSource = new CARSolicitacaoRelatorio();

			if(_da.ObterSituacao(id) == (int)eCARSolicitacaoSituacaoRelatorio.EmCadastro)
			{
				dataSource = _da.Obter(id);
			}
			else
			{
				dataSource = _da.ObterHistorico(id);
			}

			dataSource.Dominialidade = new DominialidadeDa().Obter(dataSource.DominialidadeId, tid: dataSource.DominialidadeTid);

			ObterArquivoTemplate();

			ConfigurarCabecarioRodape(0);

			#region Configurar Tabelas

			ConfiguracaoDefault.ExibirSimplesConferencia = (situacao == (int)eCARSolicitacaoSituacaoRelatorio.EmCadastro);
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

			try 
			{
				DateTime dataAtual = DateTime.Now;
				String mensagemTarja = "Consultado em " + dataAtual.ToShortDateString() + " às " + dataAtual.ToString(@"HH\hmm\min");

				using (MemoryStream msTemp = new MemoryStream(stream.ToArray()))
				{
					stream.Close();
					stream.Dispose();

					switch ((eCARSolicitacaoSituacaoRelatorio)situacao)
					{
						case eCARSolicitacaoSituacaoRelatorio.Valido:
							stream = PdfMetodosAuxiliares.TarjaVerde(msTemp, mensagemTarja, situacaoTexto);
							break;

						case eCARSolicitacaoSituacaoRelatorio.Suspenso:
							stream = PdfMetodosAuxiliares.TarjaLaranjaEscuro(msTemp, mensagemTarja, situacaoTexto);
							break;

						case eCARSolicitacaoSituacaoRelatorio.SubstituidoTituloCAR:
							stream = PdfMetodosAuxiliares.TarjaLaranja(msTemp, mensagemTarja, situacaoTexto);
							break;

						case eCARSolicitacaoSituacaoRelatorio.Invalido:
							stream = PdfMetodosAuxiliares.TarjaVermelha(msTemp, mensagemTarja, situacaoTexto);
							break;

						default:
							break;
					}
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
	}
}
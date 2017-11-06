using Aspose.Words.Tables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Entities.Etx.ModuloRelatorio.AsposeEtx;
using Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloCFOCFOC;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.ITextSharpEtx;
using Tecnomapas.EtramiteX.Credenciado.Model.RelatorioIndividual.ModuloCFOCFOC.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.RelatorioIndividual.ModuloCFOCFOC.Pdf
{
	public class PdfCFOC : PdfPadraoRelatorio
	{
		CFOCDa _da = new CFOCDa();

		public MemoryStream Gerar(int id, int credenciadoID)
		{
			ArquivoDocCaminho = @"~/Content/_pdfAspose/CFOC.doc";

			CFOCRelatorio dataSource = new CFOCRelatorio();

			dataSource = _da.Obter(id, credenciadoID);

			if (dataSource.Situacao != (int)eDocumentoFitossanitarioSituacao.EmElaboracao)
			{
				dataSource = _da.ObterHistorico(id, dataSource.Tid, credenciadoID);
			}
			else
			{
				dataSource.DataAtivacao = "--/--/--";
			}

            foreach (IdentificacaoProdutoRelatorio prod in dataSource.Produtos)
            {
                if (prod.ExibeQtdKg)
                {
                    prod.Quantidade *= 1000;
                    prod.UnidadeMedida = "KG";
                }
            }

			if (dataSource.TratamentosFitossanitarios.Count <= 0)
			{
				dataSource.TratamentosFitossanitarios.Add(new TratamentoFitossanitarioRelatorio());
			}

			if (dataSource.PartidaLacradaOrigem)
			{
				dataSource.PartidaLacradaOrigemSim = "X";
				dataSource.PartidaLacradaOrigemNao = AsposeData.Empty;
			}
			else
			{
				dataSource.PartidaLacradaOrigemSim = AsposeData.Empty;
				dataSource.PartidaLacradaOrigemNao = "X";
			}

			#region TODO

			if ((1 & dataSource.ProdutoEspecificacao) != 0)
			{
				dataSource.ProdutoEspecificacao1 = "X";
			}
			else
			{
				dataSource.ProdutoEspecificacao1 = AsposeData.Empty;
			}

			if ((2 & dataSource.ProdutoEspecificacao) != 0)
			{
				dataSource.ProdutoEspecificacao2 = "X";
			}
			else
			{
				dataSource.ProdutoEspecificacao2 = AsposeData.Empty;
			}

			if ((4 & dataSource.ProdutoEspecificacao) != 0)
			{
				dataSource.ProdutoEspecificacao3 = "X";
			}
			else
			{
				dataSource.ProdutoEspecificacao3 = AsposeData.Empty;
			}

			if ((8 & dataSource.ProdutoEspecificacao) != 0)
			{
				dataSource.ProdutoEspecificacao4 = "X";
			}
			else
			{
				dataSource.ProdutoEspecificacao4 = AsposeData.Empty;
			}

			#endregion TODO

			ObterArquivoTemplate();

			ConfiguracaoDefault.ExibirSimplesConferencia = (dataSource.Situacao == (int)eDocumentoFitossanitarioSituacao.EmElaboracao);
			ConfigurarCabecarioRodape(0, true);

			#region Configurar Tabelas

			ConfiguracaoDefault.AddLoadAcao((doc, dataSrc) =>
			{
			});

			#endregion

			MemoryStream stream = GerarPdf(dataSource);

			#region Adicionar Tarja

			try
			{
				DateTime dataAtual = DateTime.Now;
				String mensagemTarja = "Consultado em " + dataAtual.ToShortDateString() + " às " + dataAtual.ToString(@"HH\hmm\min");

				switch ((eDocumentoFitossanitarioSituacao)dataSource.Situacao)
				{
					case eDocumentoFitossanitarioSituacao.Invalido:
						using (MemoryStream msTemp = new MemoryStream(stream.ToArray()))
						{
							stream.Close();
							stream.Dispose();
							stream = PdfMetodosAuxiliares.TarjaVermelha(msTemp, mensagemTarja, "Inválido");
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
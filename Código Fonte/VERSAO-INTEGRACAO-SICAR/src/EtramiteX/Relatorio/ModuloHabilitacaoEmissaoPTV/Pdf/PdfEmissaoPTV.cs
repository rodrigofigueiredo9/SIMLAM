using System.Drawing;
using Aspose.Words.Tables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using iTextSharp.text;
using Tecnomapas.Blocos.Entities.Interno.ModuloPTV;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloHabilitacaoEmissaoPTV;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloHabilitacaoEmissaoPTV.Data;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.ITextSharpEtx;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using System.Linq;
using Tecnomapas.Blocos.Entities.Etx.ModuloRelatorio.AsposeEtx;
using ThoughtWorks.QRCode.Codec;

namespace Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloHabilitacaoEmissaoPTV.Pdf
{
	public class PdfEmissaoPTV : PdfPadraoRelatorio
	{

		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

		String UrlPDFPublico
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUrlPDFPublico); }
		}

		EmissaoDa _da = new EmissaoDa();

		public MemoryStream Gerar(int id, string tid, int situacao, string situacaoTexto)
		{
			ArquivoDocCaminho = @"~/Content/_pdfAspose/PTV.doc";

			EmissaoPTVRelatorio dataSource = new EmissaoPTVRelatorio();

			if (situacao != (int)ePTVSituacao.EmElaboracao)
			{
				dataSource = _da.ObterHistorico(id, tid);
			}
			else
			{
				dataSource = _da.Obter(id);
				dataSource.DataAtivacao = "--/--/--";
			}

			#region Imagem Vazia

			MemoryStream memory = new MemoryStream();

			Bitmap img = new Bitmap(1, 1);

			img.SetPixel(0, 0, System.Drawing.Color.White);

			img.Save(memory, System.Drawing.Imaging.ImageFormat.Gif);

			#endregion

			if (dataSource.AssinaturaDigital.Id.HasValue && dataSource.AssinaturaDigital.Id.Value > 0 && dataSource.IsEPTV)
			{
				dataSource.AssinaturaDigital.Conteudo = AsposeImage.RedimensionarImagemPNG(File.ReadAllBytes(dataSource.AssinaturaDigital.Caminho), 4);
			}
			else
			{
				dataSource.AssinaturaDigital.Conteudo = memory.ToArray();
			}

			if (dataSource.IsEPTV)
			{
				QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
				qrCodeEncoder.QRCodeScale = 2;

				System.Drawing.Image imageQRCode = qrCodeEncoder.Encode(String.Format("{0}/{1}/{2}", UrlPDFPublico, ("ptv/GerarPdfInterno"), id));

				MemoryStream msQRCode = new MemoryStream();

				imageQRCode.Save(msQRCode, System.Drawing.Imaging.ImageFormat.Gif);

				dataSource.QRCode.Conteudo = msQRCode.ToArray();
			}
			else
			{
				dataSource.QRCode.Conteudo = memory.ToArray();
			}

			#region Condicional

			if (dataSource.PartidaLacrada == (int)ePartidaLacradaOrigem.Sim)
			{
				dataSource.IsLacrada = "X";
				dataSource.IsNaoLacrada = AsposeData.Empty;
			}
			else
			{
				dataSource.IsLacrada = AsposeData.Empty;
				dataSource.IsNaoLacrada = "X";
			}
			if (dataSource.Rota_transito_definida == (int)eRotaTransitoDefinida.Sim)
			{
				dataSource.IsRota = "X";
				dataSource.IsNaoRota = AsposeData.Empty;
			}
			else
			{
				dataSource.IsRota = AsposeData.Empty;
				dataSource.IsNaoRota = "X";
			}
			if (dataSource.ApresentacaoNotaFiscal == (int)eApresentacaoNotaFiscal.Sim)
			{
				dataSource.IsNota = "X";
				dataSource.IsNaoNota = AsposeData.Empty;
			}
			else
			{
				dataSource.IsNota = AsposeData.Empty;
				dataSource.IsNaoNota = "X";
			}

			switch (dataSource.TipoTransporte)
			{
				case 1: dataSource.IsRod = "X"; dataSource.IsAer = AsposeData.Empty; dataSource.IsFer = AsposeData.Empty; dataSource.IsHid = AsposeData.Empty; dataSource.IsOut = AsposeData.Empty; break;
				case 2: dataSource.IsRod = AsposeData.Empty; dataSource.IsAer = "X"; dataSource.IsFer = AsposeData.Empty; dataSource.IsHid = AsposeData.Empty; dataSource.IsOut = AsposeData.Empty; break;
				case 3: dataSource.IsRod = AsposeData.Empty; dataSource.IsAer = AsposeData.Empty; dataSource.IsFer = "X"; dataSource.IsHid = AsposeData.Empty; dataSource.IsOut = AsposeData.Empty; break;
				case 4: dataSource.IsRod = AsposeData.Empty; dataSource.IsAer = AsposeData.Empty; dataSource.IsFer = AsposeData.Empty; dataSource.IsHid = "X"; dataSource.IsOut = AsposeData.Empty; break;
				case 5: dataSource.IsRod = AsposeData.Empty; dataSource.IsAer = AsposeData.Empty; dataSource.IsFer = AsposeData.Empty; dataSource.IsHid = AsposeData.Empty; dataSource.IsOut = "X"; break;
			}

			#endregion

			ObterArquivoTemplate();

			ConfigurarCabecarioRodape(0);

			#region Configurar

			ConfiguracaoDefault.ExibirSimplesConferencia = (situacao == (int)ePTVSituacao.EmElaboracao);

			ConfiguracaoDefault.AddLoadAcao((doc, dataSourceCnf) =>
			{
				List<Row> itensRemover = new List<Row>();

				itensRemover.ForEach(x => x.Remove());
			});

			#endregion

			MemoryStream stream = GerarPdf(dataSource);

			#region Adicionar Tarja

			try
			{
				DateTime dataAtual = DateTime.Now;
				String mensagemTarja = "Consultado em " + dataAtual.ToShortDateString() + " às " + dataAtual.ToString(@"HH\hmm\min");

				switch ((ePTVSituacao)situacao)
				{
					case ePTVSituacao.Cancelado:
						using (MemoryStream msTemp = new MemoryStream(stream.ToArray()))
						{
							stream.Close();
							stream.Dispose();
							stream = PdfMetodosAuxiliares.TarjaLaranja(msTemp, mensagemTarja, situacaoTexto);
						}
						break;
					case ePTVSituacao.Invalido:
						using (MemoryStream msTemp = new MemoryStream(stream.ToArray()))
						{
							stream.Close();
							stream.Dispose();
							stream = PdfMetodosAuxiliares.TarjaVermelha(msTemp, mensagemTarja, "Inválida");
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
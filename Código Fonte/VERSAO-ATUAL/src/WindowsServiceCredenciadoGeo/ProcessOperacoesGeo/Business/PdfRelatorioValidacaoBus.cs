using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Tecnomapas.EtramiteX.WindowsService.ProcessOperacoesGeo.Business.PDF;
using Tecnomapas.EtramiteX.WindowsService.ProcessOperacoesGeo.Business.PDF.CabecalhoRodape;

namespace Tecnomapas.EtramiteX.WindowsService.ProcessOperacoesGeo
{
	class PdfRelatorioValidacao
	{
		internal static MemoryStream GerarPdf(bool temErros, Hashtable htImportacao, Hashtable htConfiguracoes)
		{
			Document doc = new Document(PageSize.A4, 30, 25, 73, 40);
			MemoryStream str = new MemoryStream();

			PdfWriter wrt = PdfWriter.GetInstance(doc, str);
			wrt.SetEncryption(PdfWriter.STRENGTH128BITS, null, null, PdfWriter.AllowPrinting);

			PdfCabecalhoRodape headerFooter = new PdfCabecalhoRodape();

			headerFooter.GovernoNome = hasKey(htConfiguracoes, "GOVERNO_NOME") ? htConfiguracoes["GOVERNO_NOME"].ToString() : null;
			headerFooter.OrgaoCep = hasKey(htConfiguracoes, "ORGAO_CEP") ? htConfiguracoes["ORGAO_CEP"].ToString() : null;
			headerFooter.OrgaoContato = hasKey(htConfiguracoes, "ORGAO_CONTATO") ? htConfiguracoes["ORGAO_CONTATO"].ToString() : null;
			headerFooter.OrgaoEndereco = hasKey(htConfiguracoes, "ORGAO_ENDERECO") ? htConfiguracoes["ORGAO_ENDERECO"].ToString() : null;
			headerFooter.OrgaoMunicipio = hasKey(htConfiguracoes, "ORGAO_MUNICIPIO") ? htConfiguracoes["ORGAO_MUNICIPIO"].ToString() : null;
			headerFooter.OrgaoNome = hasKey(htConfiguracoes, "ORGAO_NOME") ? htConfiguracoes["ORGAO_NOME"].ToString() : null;
			headerFooter.OrgaoSigla = hasKey(htConfiguracoes, "ORGAO_SIGLA") ? htConfiguracoes["ORGAO_SIGLA"].ToString() : null;
			headerFooter.OrgaoUF = hasKey(htConfiguracoes, "ORGAO_UF") ? htConfiguracoes["ORGAO_UF"].ToString() : null;
			headerFooter.SecretariaNome = hasKey(htConfiguracoes, "SECRETARIA_NOME") ? htConfiguracoes["SECRETARIA_NOME"].ToString() : null;
			headerFooter.SetorNome = hasKey(htConfiguracoes, "SETOR_NOME") ? htConfiguracoes["SETOR_NOME"].ToString() : null;

			wrt.PageEvent = headerFooter;

			doc.Open();

			loadContents(doc, htImportacao, temErros);

			doc.Close();
			doc.Dispose();

			return str;
		}

		private static Boolean hasKey(Hashtable ht, string key)
		{
			return ((ht != null) && ht.ContainsKey(key) && ht[key] != null && !(ht[key] is DBNull));
		}

		private static void loadContents(Document doc, Hashtable htImportacao, bool temErros)
		{
			string strAux = string.Empty;

			Font arial14BoldBlack = PdfMetodosAuxiliares.GerarFonte(tipoFonte.Arial, 14, Font.BOLD, BaseColor.BLACK);
			Font arial14BoldGreen = PdfMetodosAuxiliares.GerarFonte(tipoFonte.Arial, 14, Font.BOLD, BaseColor.GREEN);
			Font arial14BoldRed = PdfMetodosAuxiliares.GerarFonte(tipoFonte.Arial, 14, Font.BOLD, BaseColor.RED);

			Font arial10BoldRed = PdfMetodosAuxiliares.GerarFonte(tipoFonte.Arial, 10, Font.BOLD, BaseColor.RED);
			Font arial10BoldBlue = PdfMetodosAuxiliares.GerarFonte(tipoFonte.Arial, 10, Font.BOLD, new BaseColor(0x0000EE));

			BaseColor corCinzaClaro = new BaseColor(220, 220, 220);

			#region Dados

			List<Hashtable> alErrosEspaciais = hasKey(htImportacao, "ERROS_ESPACIAIS") ? htImportacao["ERROS_ESPACIAIS"] as List<Hashtable> : null;
			List<Hashtable> alObrigatoriedades = hasKey(htImportacao, "OBRIGATORIEDADES") ? htImportacao["OBRIGATORIEDADES"] as List<Hashtable> : null;
			List<Hashtable> alAtributos = hasKey(htImportacao, "ATRIBUTOS") ? htImportacao["ATRIBUTOS"] as List<Hashtable> : null;
			List<Hashtable> alGeometrias = hasKey(htImportacao, "GEOMETRIAS") ? htImportacao["GEOMETRIAS"] as List<Hashtable> : null;

			#endregion

			PdfPTable tabelaLinha = null;
			PdfPTable tabelaDocumento = null;

			tabelaDocumento = new PdfPTable(1);
			tabelaDocumento.WidthPercentage = 100;
			tabelaDocumento.SetWidths(new float[] { 100 });
			tabelaDocumento.SplitLate = false;
			tabelaDocumento.SplitRows = true;

			tabelaDocumento.DefaultCell.Border = 0;
			tabelaDocumento.DefaultCell.PaddingLeft = 2;
			tabelaDocumento.DefaultCell.PaddingRight = 2;
			tabelaDocumento.DefaultCell.PaddingBottom = 2;
			tabelaDocumento.DefaultCell.PaddingTop = 2;
			tabelaDocumento.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
			tabelaDocumento.HeaderRows = 1;

			#region Título

			tabelaDocumento.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
			tabelaDocumento.AddCell(new Phrase(new Chunk("Relatório de Importação", arial14BoldBlack)));
			tabelaDocumento.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
			tabelaDocumento.AddCell("\n");

			#endregion

			#region Situação

			tabelaDocumento.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;

			if (!temErros)
			{
				tabelaDocumento.AddCell(new Phrase(new Chunk("Dados Aprovados", arial14BoldGreen)));
			}
			else
			{
				tabelaDocumento.AddCell(new Phrase(new Chunk("Dados Inválidos", arial14BoldRed)));
			}

			tabelaDocumento.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;

			tabelaDocumento.AddCell("\n");

			#endregion

			#region Erros Espaciais

			if (alErrosEspaciais != null && alErrosEspaciais.Count > 0)
			{
				tabelaDocumento.AddCell(new Phrase(new Chunk("Erros Espaciais", arial10BoldRed)));

				tabelaLinha = new PdfPTable(new float[] { 30, 70 });
				tabelaLinha.DefaultCell.PaddingLeft = 3;
				tabelaLinha.DefaultCell.PaddingRight = 3;
				tabelaLinha.DefaultCell.PaddingBottom = 3;
				tabelaLinha.DefaultCell.PaddingTop = 3;
				tabelaLinha.DefaultCell.BackgroundColor = corCinzaClaro;

				tabelaLinha.AddCell(new Phrase(new Chunk("Tabela", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.AddCell(new Phrase(new Chunk("Erro", PdfMetodosAuxiliares.arial8Negrito)));

				tabelaLinha.DefaultCell.BackgroundColor = null;

				foreach (Hashtable htErroEspacial in alErrosEspaciais)
				{
					strAux = hasKey(htErroEspacial, "SIGLA_TABELA") ? htErroEspacial["SIGLA_TABELA"].ToString() : string.Empty;
					tabelaLinha.AddCell(new Phrase(new Chunk(strAux, PdfMetodosAuxiliares.arial8)));

					strAux = hasKey(htErroEspacial, "DESCRICAO_MENSAGEM") ? htErroEspacial["DESCRICAO_MENSAGEM"].ToString() : string.Empty;
					tabelaLinha.AddCell(new Phrase(new Chunk(strAux, PdfMetodosAuxiliares.arial8)));
				}

				tabelaDocumento.AddCell(tabelaLinha);
			}
			else
			{
				tabelaDocumento.AddCell(new Phrase(new Chunk("Erros Espaciais", arial10BoldBlue)));
				tabelaDocumento.AddCell(new Phrase(new Chunk("- Nenhum erro espacial foi encontrado.", PdfMetodosAuxiliares.arial8)));
			}

			tabelaDocumento.AddCell("\n");

			#endregion

			#region Obrigatoriedades

			if (alObrigatoriedades != null && alObrigatoriedades.Count > 0)
			{
				tabelaDocumento.AddCell(new Phrase(new Chunk("Obrigatoriedades", arial10BoldRed)));

				tabelaLinha = new PdfPTable(new float[] { 100 });
				tabelaLinha.DefaultCell.PaddingLeft = 3;
				tabelaLinha.DefaultCell.PaddingRight = 3;
				tabelaLinha.DefaultCell.PaddingBottom = 3;
				tabelaLinha.DefaultCell.PaddingTop = 3;
				tabelaLinha.DefaultCell.BackgroundColor = corCinzaClaro;

				tabelaLinha.AddCell(new Phrase(new Chunk("Validações", PdfMetodosAuxiliares.arial8Negrito)));

				tabelaLinha.DefaultCell.BackgroundColor = null;

				foreach (Hashtable htObrigatoriedade in alObrigatoriedades)
				{
					strAux = hasKey(htObrigatoriedade, "DESCRICAO_MENSAGEM") ? htObrigatoriedade["DESCRICAO_MENSAGEM"].ToString() : string.Empty;
					tabelaLinha.AddCell(new Phrase(new Chunk(strAux, PdfMetodosAuxiliares.arial8)));
				}

				tabelaDocumento.AddCell(tabelaLinha);
			}
			else
			{
				tabelaDocumento.AddCell(new Phrase(new Chunk("Obrigatoriedades", arial10BoldBlue)));
				tabelaDocumento.AddCell(new Phrase(new Chunk("- Todas as obrigatoriedades foram atendidas.", PdfMetodosAuxiliares.arial8)));
			}

			tabelaDocumento.AddCell("\n");

			#endregion

			#region Atributos

			if (alAtributos != null && alAtributos.Count > 0)
			{
				tabelaDocumento.AddCell(new Phrase(new Chunk("Atributos", arial10BoldRed)));

				tabelaLinha = new PdfPTable(new float[] { 20, 80 });
				tabelaLinha.DefaultCell.PaddingLeft = 3;
				tabelaLinha.DefaultCell.PaddingRight = 3;
				tabelaLinha.DefaultCell.PaddingBottom = 3;
				tabelaLinha.DefaultCell.PaddingTop = 3;
				tabelaLinha.DefaultCell.BackgroundColor = corCinzaClaro;

				tabelaLinha.AddCell(new Phrase(new Chunk("Tabela", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.AddCell(new Phrase(new Chunk("Erro", PdfMetodosAuxiliares.arial8Negrito)));

				tabelaLinha.DefaultCell.BackgroundColor = null;

				foreach (Hashtable htAtributo in alAtributos)
				{
					strAux = hasKey(htAtributo, "SIGLA_TABELA") ? htAtributo["SIGLA_TABELA"].ToString() : string.Empty;
					tabelaLinha.AddCell(new Phrase(new Chunk(strAux, PdfMetodosAuxiliares.arial8)));

					strAux = hasKey(htAtributo, "DESCRICAO_MENSAGEM") ? htAtributo["DESCRICAO_MENSAGEM"].ToString() : string.Empty;
					tabelaLinha.AddCell(new Phrase(new Chunk(strAux, PdfMetodosAuxiliares.arial8)));
				}

				tabelaDocumento.AddCell(tabelaLinha);
			}
			else
			{
				tabelaDocumento.AddCell(new Phrase(new Chunk("Atributos", arial10BoldBlue)));
				tabelaDocumento.AddCell(new Phrase(new Chunk("- Nenhum erro nos atributos foi encontrado.", PdfMetodosAuxiliares.arial8)));
			}

			tabelaDocumento.AddCell("\n");

			#endregion

			#region Geometrias (Quantidade)

			tabelaDocumento.AddCell(new Phrase(new Chunk("Geometrias", arial10BoldBlue)));

			tabelaLinha = new PdfPTable(new float[] { 70, 30 });
			tabelaLinha.DefaultCell.PaddingLeft = 3;
			tabelaLinha.DefaultCell.PaddingRight = 3;
			tabelaLinha.DefaultCell.PaddingBottom = 3;
			tabelaLinha.DefaultCell.PaddingTop = 3;
			tabelaLinha.DefaultCell.BackgroundColor = corCinzaClaro;

			tabelaLinha.AddCell(new Phrase(new Chunk("Tabela", PdfMetodosAuxiliares.arial8Negrito)));
			tabelaLinha.AddCell(new Phrase(new Chunk("Quantidade", PdfMetodosAuxiliares.arial8Negrito)));

			tabelaLinha.DefaultCell.BackgroundColor = null;

			if (alGeometrias != null)
			{
				foreach (Hashtable htGeometria in alGeometrias)
				{
					strAux = htGeometria["SIGLA_TABELA"].ToString();
					strAux += hasKey(htGeometria, "NOME_TABELA") ? " - " + htGeometria["NOME_TABELA"].ToString() : string.Empty;

					tabelaLinha.AddCell(new Phrase(new Chunk(strAux, PdfMetodosAuxiliares.arial8)));

					strAux = hasKey(htGeometria, "DESCRICAO_MENSAGEM") ? htGeometria["DESCRICAO_MENSAGEM"].ToString() : string.Empty;
					tabelaLinha.AddCell(new Phrase(new Chunk(strAux, PdfMetodosAuxiliares.arial8)));
				}
			}

			tabelaDocumento.AddCell(tabelaLinha);

			#endregion

			doc.Add(tabelaDocumento);
		}
	}
}
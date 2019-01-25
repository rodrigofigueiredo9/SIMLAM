using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Tecnomapas.Blocos.Data;
using Tecnomapas.EtramiteX.WindowsService.ProcessOperacoesGeo.ArcGIS;
using Tecnomapas.EtramiteX.WindowsService.ProcessOperacoesGeo.Business.PDF;
using Tecnomapas.EtramiteX.WindowsService.ProcessOperacoesGeo.Business.PDF.CabecalhoRodape;
using Tecnomapas.EtramiteX.WindowsService.ProcessOperacoesGeo.Data;
using Tecnomapas.EtramiteX.WindowsService.ProcessOperacoesGeo.Entities;

namespace Tecnomapas.EtramiteX.WindowsService.ProcessOperacoesGeo.Business
{
	class PdfRelatorioCroquiBus
	{
		PdfRelatorioDa _dataAccess;
		CoordenadaBus _busCoord = new CoordenadaBus();

		public Projeto Project { get; set; }

		public PdfRelatorioCroquiBus(BancoDeDados banco, Projeto project)
		{
			Project = project;

			//_banco = banco;
			_dataAccess = new PdfRelatorioDa(banco);
		}

		#region Obter

		public Hashtable ObterDadosCabecalhoRodapePDF()
		{
			return _dataAccess.BuscarDadosCabecalhoRodapePDF(Project.Id, Project.Type);
		}

		public Hashtable ObterDadosPDF()
		{
			return _dataAccess.BuscarDadosPDF(Project.Id, Project.Type, Project.Step);
		}

		#endregion

		#region Dominialidade e PecaTecnica

		internal MemoryStream GerarPdfPecaTecnica(MxdLayout mxd)
		{
			Document doc = new Document(mxd.MxdPageSize, 85, 40, 73, 50);

			MemoryStream ms = new MemoryStream();
			PdfWriter wrt = PdfWriter.GetInstance(doc, ms);
			//wrt.SetEncryption(PdfWriter.STRENGTH128BITS, null, null, PdfWriter.AllowCopy | PdfWriter.AllowPrinting);

			wrt.PageEvent = new PdfCabecalhoRodapePecaTecnica();

			doc.Open();

			Hashtable hashData = ObterDadosPDF();

			mxd.GerarPdf(doc, wrt, Project.Id, hashData);
			GerarVersoDominialidade(doc, wrt, hashData);

			doc.Close();

			mxd.ApagarTempFile();

			return ms;
		}

		internal MemoryStream GerarPdfDominialidade(MxdLayout mxd)
		{
			Document doc = new Document(mxd.MxdPageSize, 85, 40, 73, 50);

			MemoryStream ms = new MemoryStream();
			PdfWriter wrt = PdfWriter.GetInstance(doc, ms);
			//wrt.SetEncryption(PdfWriter.STRENGTH128BITS, null, null, PdfWriter.AllowCopy | PdfWriter.AllowPrinting);


			//Cabecalho e Rodape
			Hashtable htConfiguracoes = ObterDadosCabecalhoRodapePDF();

			PdfCabecalhoRodape headerFooter = new PdfCabecalhoRodape();

			headerFooter.GovernoNome = HasKey(htConfiguracoes, "GOVERNO_NOME") ? htConfiguracoes["GOVERNO_NOME"].ToString() : null;
			headerFooter.OrgaoCep = HasKey(htConfiguracoes, "ORGAO_CEP") ? htConfiguracoes["ORGAO_CEP"].ToString() : null;
			headerFooter.OrgaoContato = HasKey(htConfiguracoes, "ORGAO_CONTATO") ? htConfiguracoes["ORGAO_CONTATO"].ToString() : null;
			headerFooter.OrgaoEndereco = HasKey(htConfiguracoes, "ORGAO_ENDERECO") ? htConfiguracoes["ORGAO_ENDERECO"].ToString() : null;
			headerFooter.OrgaoMunicipio = HasKey(htConfiguracoes, "ORGAO_MUNICIPIO") ? htConfiguracoes["ORGAO_MUNICIPIO"].ToString() : null;
			headerFooter.OrgaoNome = HasKey(htConfiguracoes, "ORGAO_NOME") ? htConfiguracoes["ORGAO_NOME"].ToString() : null;
			headerFooter.OrgaoSigla = HasKey(htConfiguracoes, "ORGAO_SIGLA") ? htConfiguracoes["ORGAO_SIGLA"].ToString() : null;
			headerFooter.OrgaoUF = HasKey(htConfiguracoes, "ORGAO_UF") ? htConfiguracoes["ORGAO_UF"].ToString() : null;
			headerFooter.SecretariaNome = HasKey(htConfiguracoes, "SECRETARIA_NOME") ? htConfiguracoes["SECRETARIA_NOME"].ToString() : null;
			headerFooter.SetorNome = HasKey(htConfiguracoes, "SETOR_NOME") ? htConfiguracoes["SETOR_NOME"].ToString() : null;

			wrt.PageEvent = headerFooter;
			//------------------------

			doc.Open();

			Hashtable hashData = ObterDadosPDF();

			mxd.GerarPdf(doc, wrt, Project.Id, hashData);
			GerarVersoDominialidade(doc, wrt, hashData);

			doc.Close();

			mxd.ApagarTempFile();

			return ms;
		}

		private void GerarVersoDominialidade(Document doc, PdfWriter wrt, Hashtable hashData)
		{
			BaseColor corCinzaClaro = new BaseColor(220, 220, 220);

			PdfPTable tabelaDocumento;
			PdfPTable tabelaLinha;
			List<Hashtable> hashList = null;

			#region Quadros de Áreas

			if ((hashData["QUADRO_TOTAL"] != null) && (hashData["QUADRO_TOTAL"] != DBNull.Value))
				hashList = hashData["QUADRO_TOTAL"] as List<Hashtable>;
			else
				hashList = new List<Hashtable>();

			tabelaDocumento = new PdfPTable(1);
			tabelaDocumento.WidthPercentage = 100;
			tabelaDocumento.SetWidths(new float[] { 100 });
			tabelaDocumento.SplitLate = true;
			tabelaDocumento.SplitRows = true;

			tabelaDocumento.DefaultCell.Border = 0;
			tabelaDocumento.DefaultCell.PaddingLeft = 0;
			tabelaDocumento.DefaultCell.PaddingRight = 0;
			tabelaDocumento.DefaultCell.PaddingBottom = 2;
			tabelaDocumento.DefaultCell.PaddingTop = 2;
			tabelaDocumento.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
			tabelaDocumento.HeaderRows = 1;

			//Titulo
			tabelaDocumento.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
			tabelaDocumento.AddCell(new Phrase(new Chunk("QUADROS DE ÁREAS", PdfMetodosAuxiliares.arial16Negrito)));
			tabelaDocumento.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;

			List<Hashtable> matriculas = null;
			if ((hashData["QUADRO_DE_AREAS"] != null) && (hashData["QUADRO_DE_AREAS"] != DBNull.Value))
				matriculas = hashData["QUADRO_DE_AREAS"] as List<Hashtable>;
			else
				matriculas = new List<Hashtable>();

			if (matriculas.Count == 1)
			{
				//Quadro da Matricula
				tabelaDocumento.AddCell("\n");
				tabelaLinha = new PdfPTable(new float[] { 19, 52, 16, 13 });
				tabelaLinha.DefaultCell.PaddingLeft = 3;
				tabelaLinha.DefaultCell.PaddingRight = 3;
				tabelaLinha.DefaultCell.PaddingBottom = 3;
				tabelaLinha.DefaultCell.PaddingTop = 3;

				tabelaLinha.AddCell(new Phrase(new Chunk(FormatStringField(matriculas[0], "APMP_TIPO") + ':', PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.AddCell(new Phrase(new Chunk(FormatStringField(matriculas[0], "APMP_NOME"), PdfMetodosAuxiliares.arial8)));
				tabelaLinha.AddCell(new Phrase(new Chunk("Perímetro (m):", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_RIGHT;
				tabelaLinha.AddCell(new Phrase(new Chunk(FormatNumberField(matriculas[0], "APMP_PERIMETER", 3), PdfMetodosAuxiliares.arial8)));

				tabelaDocumento.AddCell(tabelaLinha);
			}

			//Quadro Total
			tabelaDocumento.AddCell("\n");
			tabelaDocumento.DefaultCell.PaddingBottom = 10;
			tabelaDocumento.AddCell(new Phrase(new Chunk("Área Total", PdfMetodosAuxiliares.arial10Negrito)));
			tabelaDocumento.DefaultCell.PaddingBottom = 2;

			tabelaLinha = new PdfPTable(new float[] { 19, 55, 13, 13 });
			tabelaLinha.DefaultCell.PaddingLeft = 3;
			tabelaLinha.DefaultCell.PaddingRight = 3;
			tabelaLinha.DefaultCell.PaddingBottom = 3;
			tabelaLinha.DefaultCell.PaddingTop = 3;
			tabelaLinha.DefaultCell.BackgroundColor = corCinzaClaro;
			tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;

			tabelaLinha.AddCell(new Phrase(new Chunk("Classe", PdfMetodosAuxiliares.arial8Negrito)));
			tabelaLinha.AddCell(new Phrase(new Chunk("Descrição", PdfMetodosAuxiliares.arial8Negrito)));
			tabelaLinha.AddCell(new Phrase(new Chunk("Área (m²)", PdfMetodosAuxiliares.arial8Negrito)));
			tabelaLinha.AddCell(new Phrase(new Chunk("Área (ha)", PdfMetodosAuxiliares.arial8Negrito)));

			tabelaLinha.DefaultCell.BackgroundColor = null;
			tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
			tabelaLinha.DefaultCell.VerticalAlignment = Element.ALIGN_MIDDLE;

			decimal areaTotal = 0;
			int count = hashList.Count;
			for (int i = 0; i < count; i++)
			{
				Hashtable ht = hashList[i];

				string descricao = "";
				int countClasse = 1;

				int otherClasse = i + 1;
				while ((otherClasse < count) && (hashList[otherClasse]["CLASSE"].ToString() == ht["CLASSE"].ToString()))
				{
					if (descricao != FormatStringField(hashList[otherClasse], "DESCRICAO"))
					{
						descricao = FormatStringField(hashList[otherClasse], "DESCRICAO");
						countClasse++;
					}

					otherClasse++;
				}

				tabelaLinha.DefaultCell.Rowspan = countClasse;
				tabelaLinha.AddCell(new Phrase(new Chunk(FormatStringField(ht, "CLASSE"), PdfMetodosAuxiliares.arial8)));
				tabelaLinha.DefaultCell.Rowspan = 1;
				tabelaLinha.AddCell(new Phrase(new Chunk(FormatStringField(ht, "DESCRICAO"), PdfMetodosAuxiliares.arial8)));

				AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumberField(ht, "AREA_M2"));
				AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumber(Convert.ToDecimal(ht["AREA_M2"]) / 10000, 4));

				descricao = "";

				List<Hashtable> subTipos = new List<Hashtable>();
				while (++i <= otherClasse)
				{
					if (i == otherClasse || descricao != FormatStringField(hashList[i], "DESCRICAO"))
					{
						if (descricao != "")
						{
							if (subTipos.Count != 1 || subTipos[0]["SUBTIPO"].ToString() != "")
							{
								descricao += "\n[";
								foreach (Hashtable hash in subTipos)
								{
									if (Convert.ToDecimal(hash["AREA_M2"]) > 0)
										descricao += Math.Round(100 * Convert.ToDecimal(hash["AREA_M2"]) / areaTotal) + "% " + hash["SUBTIPO"] + ", ";
								}
								descricao += "]";
								descricao = descricao.Replace(", ]", "]");
							}

							tabelaLinha.AddCell(new Phrase(new Chunk(descricao, PdfMetodosAuxiliares.arial8)));

							AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumber(areaTotal));
							AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumber(areaTotal / 10000, 4));
						}

						if (i == otherClasse)
							break;

						descricao = FormatStringField(hashList[i], "DESCRICAO");
						subTipos = new List<Hashtable>();
						areaTotal = 0;
					}

					areaTotal += Convert.ToDecimal(hashList[i]["AREA_M2"]);

					subTipos.Add(hashList[i]);
				}

				i--;
			}

			tabelaDocumento.AddCell(tabelaLinha);

			#endregion Quadros de Áreas

			doc.NewPage();
			doc.Add(tabelaDocumento);

			if (matriculas.Count > 1)
			{
				doc.SetPageSize(doc.PageSize.Rotate());
				doc.NewPage();
				hashList = matriculas;

				#region Áreas e Perímetros de Matrícula/Posse

				tabelaDocumento = new PdfPTable(1);
				tabelaDocumento.WidthPercentage = 100;
				tabelaDocumento.SetWidths(new float[] { 100 });
				tabelaDocumento.SplitLate = false;
				tabelaDocumento.SplitRows = true;

				tabelaDocumento.DefaultCell.Border = 0;
				tabelaDocumento.DefaultCell.PaddingLeft = 0;
				tabelaDocumento.DefaultCell.PaddingRight = 0;
				tabelaDocumento.DefaultCell.PaddingBottom = 2;
				tabelaDocumento.DefaultCell.PaddingTop = 2;
				tabelaDocumento.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
				tabelaDocumento.HeaderRows = 1;

				tabelaDocumento.DefaultCell.PaddingBottom = 10;
				tabelaDocumento.AddCell(new Phrase(new Chunk("Áreas e Perímetros de Matrícula/Posse", PdfMetodosAuxiliares.arial10Negrito)));
				tabelaDocumento.DefaultCell.PaddingBottom = 2;

				tabelaLinha = new PdfPTable(new float[] { 20, 20, 20, 20, 20 });
				tabelaLinha.DefaultCell.PaddingLeft = 3;
				tabelaLinha.DefaultCell.PaddingRight = 3;
				tabelaLinha.DefaultCell.PaddingBottom = 3;
				tabelaLinha.DefaultCell.PaddingTop = 3;
				tabelaLinha.DefaultCell.BackgroundColor = corCinzaClaro;
				tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;

				tabelaLinha.AddCell(new Phrase(new Chunk("Nome", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.AddCell(new Phrase(new Chunk("Tipo", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.AddCell(new Phrase(new Chunk("Área (m²)", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.AddCell(new Phrase(new Chunk("Área (ha)", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.AddCell(new Phrase(new Chunk("Perímetro (m)", PdfMetodosAuxiliares.arial8Negrito)));

				tabelaLinha.DefaultCell.BackgroundColor = null;
				tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;

				foreach (Hashtable ht in hashList)
				{
					tabelaLinha.AddCell(new Phrase(new Chunk(FormatStringField(ht, "APMP_NOME"), PdfMetodosAuxiliares.arial8)));
					tabelaLinha.AddCell(new Phrase(new Chunk(FormatStringField(ht, "APMP_TIPO"), PdfMetodosAuxiliares.arial8)));

					AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumberField(ht, "APMP_AREA"));
					AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumber(Convert.ToDecimal(ht["APMP_AREA"]) / 10000, 4));
					AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumberField(ht, "APMP_PERIMETER", 3));
				}

				tabelaDocumento.AddCell(tabelaLinha);
				tabelaDocumento.AddCell("\n");
				doc.Add(tabelaDocumento);

				#endregion Áreas e Perímetros de Matrícula/Posse

				#region Áreas de Uso do Solo por Matrícula/Posse

				tabelaDocumento = new PdfPTable(1);
				tabelaDocumento.WidthPercentage = 100;
				tabelaDocumento.SetWidths(new float[] { 100 });
				tabelaDocumento.SplitLate = false;
				tabelaDocumento.SplitRows = true;

				tabelaDocumento.DefaultCell.Border = 0;
				tabelaDocumento.DefaultCell.PaddingLeft = 0;
				tabelaDocumento.DefaultCell.PaddingRight = 0;
				tabelaDocumento.DefaultCell.PaddingBottom = 2;
				tabelaDocumento.DefaultCell.PaddingTop = 2;
				tabelaDocumento.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
				tabelaDocumento.HeaderRows = 1;

				tabelaDocumento.DefaultCell.PaddingBottom = 10;
				tabelaDocumento.AddCell(new Phrase(new Chunk("Áreas de Uso do Solo por Matrícula/Posse", PdfMetodosAuxiliares.arial10Negrito)));
				tabelaDocumento.DefaultCell.PaddingBottom = 2;

				tabelaLinha = new PdfPTable(new float[] { 1, 1, 1, 1.2f, 1, 1, 1, 1, 1 });
				tabelaLinha.DefaultCell.PaddingLeft = 3;
				tabelaLinha.DefaultCell.PaddingRight = 3;
				tabelaLinha.DefaultCell.PaddingBottom = 3;
				tabelaLinha.DefaultCell.PaddingTop = 3;
				tabelaLinha.DefaultCell.BackgroundColor = corCinzaClaro;
				tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;

				tabelaLinha.AddCell(new Phrase(new Chunk("Matrícula/Posse", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.AddCell(new Phrase(new Chunk("AFS (m²)", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.AddCell(new Phrase(new Chunk("Rocha (m²)", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.AddCell(new Phrase(new Chunk("Massa d'água (m²)", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.AddCell(new Phrase(new Chunk("AVN (m²)", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.AddCell(new Phrase(new Chunk("AA (m²)", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.AddCell(new Phrase(new Chunk("RPPN (m²)", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.AddCell(new Phrase(new Chunk("ARL (m²)", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.AddCell(new Phrase(new Chunk("APP (m²)", PdfMetodosAuxiliares.arial8Negrito)));

				tabelaLinha.DefaultCell.BackgroundColor = null;
				tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;

				foreach (Hashtable ht in hashList)
				{
					tabelaLinha.AddCell(new Phrase(new Chunk(FormatStringField(ht, "APMP_NOME"), PdfMetodosAuxiliares.arial8)));

					AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumberField(ht, "AFS_AREA"));
					AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumberField(ht, "ROCHA_AREA"));
					AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumberField(ht, "MASSA_DAGUA_AREA"));
					AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumber(Convert.ToDecimal(ht["AVN_I_AREA"]) + Convert.ToDecimal(ht["AVN_M_AREA"]) + Convert.ToDecimal(ht["AVN_A_AREA"]) + Convert.ToDecimal(ht["AVN_D_AREA"])));
					AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumber(Convert.ToDecimal(ht["AA_REC_AREA"]) + Convert.ToDecimal(ht["AA_USO_AREA"]) + Convert.ToDecimal(ht["AA_D_AREA"])));
					AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumberField(ht, "RPPN_AREA"));
					AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumber(Convert.ToDecimal(ht["ARL_PRESERV_AREA"]) + Convert.ToDecimal(ht["ARL_REC_AREA"]) + Convert.ToDecimal(ht["ARL_USO_AREA"]) + Convert.ToDecimal(ht["ARL_D_AREA"])));
					AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumberField(ht, "APP_APMP_AREA"));
				}

				tabelaDocumento.AddCell(tabelaLinha);
				tabelaDocumento.AddCell("\n");
				doc.Add(tabelaDocumento);

				#endregion Áreas de Uso do Solo por Matrícula/Posse

				#region Detalhamento da cobertura vegetal em AVN e em AA por Matrícula/Posse

				tabelaDocumento = new PdfPTable(1);
				tabelaDocumento.WidthPercentage = 100;
				tabelaDocumento.SetWidths(new float[] { 100 });
				tabelaDocumento.SplitLate = false;
				tabelaDocumento.SplitRows = true;

				tabelaDocumento.DefaultCell.Border = 0;
				tabelaDocumento.DefaultCell.PaddingLeft = 0;
				tabelaDocumento.DefaultCell.PaddingRight = 0;
				tabelaDocumento.DefaultCell.PaddingBottom = 2;
				tabelaDocumento.DefaultCell.PaddingTop = 2;
				tabelaDocumento.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
				tabelaDocumento.HeaderRows = 1;

				tabelaDocumento.DefaultCell.PaddingBottom = 10;
				tabelaDocumento.AddCell(new Phrase(new Chunk("Detalhamento da cobertura vegetal em AVN e em AA por Matrícula/Posse", PdfMetodosAuxiliares.arial10Negrito)));
				tabelaDocumento.DefaultCell.PaddingBottom = 2;

				tabelaLinha = new PdfPTable(new float[] { 1, 1, 1, 1, 1, 1, 1, 1 });
				tabelaLinha.DefaultCell.PaddingLeft = 3;
				tabelaLinha.DefaultCell.PaddingRight = 3;
				tabelaLinha.DefaultCell.PaddingBottom = 3;
				tabelaLinha.DefaultCell.PaddingTop = 3;
				tabelaLinha.DefaultCell.BackgroundColor = corCinzaClaro;
				tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
				tabelaLinha.DefaultCell.VerticalAlignment = Element.ALIGN_MIDDLE;

				tabelaLinha.DefaultCell.Rowspan = 2;
				tabelaLinha.AddCell(new Phrase(new Chunk("Matrícula/Posse", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.DefaultCell.Rowspan = 1;

				tabelaLinha.DefaultCell.Colspan = 4;
				tabelaLinha.AddCell(new Phrase(new Chunk("Estágio da AVN (m²)", PdfMetodosAuxiliares.arial8Negrito)));

				tabelaLinha.DefaultCell.Colspan = 3;
				tabelaLinha.AddCell(new Phrase(new Chunk("Uso da AA (m²)", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.DefaultCell.Colspan = 1;

				tabelaLinha.AddCell(new Phrase(new Chunk("Inicial", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.AddCell(new Phrase(new Chunk("Médio", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.AddCell(new Phrase(new Chunk("Avançado", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.AddCell(new Phrase(new Chunk("Não caracterizado", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.AddCell(new Phrase(new Chunk("Em Recuperação", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.AddCell(new Phrase(new Chunk("Em Uso", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.AddCell(new Phrase(new Chunk("Não caracterizada", PdfMetodosAuxiliares.arial8Negrito)));

				tabelaLinha.DefaultCell.BackgroundColor = null;
				tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;

				foreach (Hashtable ht in hashList)
				{
					tabelaLinha.AddCell(new Phrase(new Chunk(FormatStringField(ht, "APMP_NOME"), PdfMetodosAuxiliares.arial8)));

					AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumberField(ht, "AVN_I_AREA"));
					AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumberField(ht, "AVN_M_AREA"));
					AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumberField(ht, "AVN_A_AREA"));
					AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumberField(ht, "AVN_D_AREA"));
					AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumberField(ht, "AA_REC_AREA"));
					AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumberField(ht, "AA_USO_AREA"));
					AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumberField(ht, "AA_D_AREA"));
				}

				tabelaDocumento.AddCell(tabelaLinha);
				tabelaDocumento.AddCell("\n");
				try
				{
					doc.Add(tabelaDocumento);
				}
				catch
				{
					doc.NewPage();
					doc.Add(tabelaDocumento);
				}

				#endregion Detalhamento da cobertura vegetal em AVN e em AA por Matrícula/Posse

				#region Situação Ambiental por Matrícula/Posse

				tabelaDocumento = new PdfPTable(1);
				tabelaDocumento.WidthPercentage = 100;
				tabelaDocumento.SetWidths(new float[] { 100 });
				tabelaDocumento.SplitLate = false;
				tabelaDocumento.SplitRows = true;

				tabelaDocumento.DefaultCell.Border = 0;
				tabelaDocumento.DefaultCell.PaddingLeft = 0;
				tabelaDocumento.DefaultCell.PaddingRight = 0;
				tabelaDocumento.DefaultCell.PaddingBottom = 2;
				tabelaDocumento.DefaultCell.PaddingTop = 2;
				tabelaDocumento.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
				tabelaDocumento.HeaderRows = 1;

				tabelaDocumento.DefaultCell.PaddingBottom = 10;
				tabelaDocumento.AddCell(new Phrase(new Chunk("Situação Ambiental por Matrícula/Posse", PdfMetodosAuxiliares.arial10Negrito)));
				tabelaDocumento.DefaultCell.PaddingBottom = 2;

				tabelaLinha = new PdfPTable(new float[] { 1, 0.8F, 1, 0.8F, 1.2F, 0.8F, 1, 0.8F, 1.25F, 1.25F });
				tabelaLinha.DefaultCell.PaddingLeft = 3;
				tabelaLinha.DefaultCell.PaddingRight = 3;
				tabelaLinha.DefaultCell.PaddingBottom = 3;
				tabelaLinha.DefaultCell.PaddingTop = 3;
				tabelaLinha.DefaultCell.BackgroundColor = corCinzaClaro;
				tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
				tabelaLinha.DefaultCell.VerticalAlignment = Element.ALIGN_MIDDLE;

				tabelaLinha.DefaultCell.Rowspan = 2;
				tabelaLinha.AddCell(new Phrase(new Chunk("Matrícula/Posse", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.DefaultCell.Rowspan = 1;

				tabelaLinha.DefaultCell.Colspan = 5;
				tabelaLinha.AddCell(new Phrase(new Chunk("ARL (m²)", PdfMetodosAuxiliares.arial8Negrito)));

				tabelaLinha.DefaultCell.Colspan = 4;
				tabelaLinha.AddCell(new Phrase(new Chunk("APP (m²)", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.DefaultCell.Colspan = 1;

				//APMP ARL
				tabelaLinha.AddCell(new Phrase(new Chunk("Preservada", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.AddCell(new Phrase(new Chunk("Em Recuperação", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.AddCell(new Phrase(new Chunk("Em Uso", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.AddCell(new Phrase(new Chunk("Em Preservação Permanente", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.AddCell(new Phrase(new Chunk("Não caracterizada", PdfMetodosAuxiliares.arial8Negrito)));

				//APMP APP
				tabelaLinha.AddCell(new Phrase(new Chunk("Preservada", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.AddCell(new Phrase(new Chunk("Em Recuperação", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.AddCell(new Phrase(new Chunk("Em Uso", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.AddCell(new Phrase(new Chunk("Não caracterizada", PdfMetodosAuxiliares.arial8Negrito)));

				tabelaLinha.DefaultCell.BackgroundColor = null;
				tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;

				foreach (Hashtable ht in hashList)
				{
					tabelaLinha.AddCell(new Phrase(new Chunk(FormatStringField(ht, "APMP_NOME"), PdfMetodosAuxiliares.arial8)));

					//APMP ARL
					AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumberField(ht, "ARL_PRESERV_AREA"));
					AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumberField(ht, "ARL_REC_AREA"));
					AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumberField(ht, "ARL_USO_AREA"));
					AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumberField(ht, "APP_ARL_AREA"));
					AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumberField(ht, "ARL_D_AREA"));

					//APMP APP
					AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumberField(ht, "APP_AVN_AREA"));
					AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumberField(ht, "APP_AA_REC_AREA"));
					AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumberField(ht, "APP_AA_USO_AREA"));
					AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumber(Convert.ToDecimal(ht["APP_APMP_AREA"]) - Convert.ToDecimal(ht["APP_AVN_AREA"]) - Convert.ToDecimal(ht["APP_AA_REC_AREA"]) - Convert.ToDecimal(ht["APP_AA_USO_AREA"])));
				}

				tabelaDocumento.AddCell(tabelaLinha);
				try
				{
					doc.Add(tabelaDocumento);
				}
				catch
				{
					doc.NewPage();
					doc.Add(tabelaDocumento);
				}

				#endregion Situação Ambiental por Matrícula/Posse
			}

			PdfPCell vazia = new PdfPCell();
			vazia.Border = 0;
			vazia.BorderWidth = 0;

			if (matriculas.Count > 1)
			{
				doc.SetPageSize(doc.PageSize.Rotate());
			}
			doc.NewPage();

			#region Lista de Coordenadas

			tabelaDocumento = new PdfPTable(1);
			tabelaDocumento.WidthPercentage = 100;
			tabelaDocumento.SetWidths(new float[] { 100 });
			tabelaDocumento.SplitLate = false;
			tabelaDocumento.SplitRows = true;

			tabelaDocumento.DefaultCell.Border = 0;
			tabelaDocumento.DefaultCell.PaddingLeft = 0;
			tabelaDocumento.DefaultCell.PaddingRight = 0;
			tabelaDocumento.DefaultCell.PaddingBottom = 2;
			tabelaDocumento.DefaultCell.PaddingTop = 2;
			tabelaDocumento.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
			//tabelaDocumento.HeaderRows = 1;

			//Coordenadas da ATP
			tabelaDocumento.AddCell("\n");
			tabelaDocumento.DefaultCell.PaddingBottom = 10;
			tabelaDocumento.AddCell(new Phrase(new Chunk("Lista de Coordenadas da ATP (SIRGAS 2000 / UTM zone 24S)", PdfMetodosAuxiliares.arial10Negrito)));
			tabelaDocumento.DefaultCell.PaddingBottom = 2;

			doc.Add(tabelaDocumento);

			tabelaLinha = new PdfPTable(new float[] { 1, 1, 1, 1, 1, 1 });
			tabelaLinha.HeaderRows = 1;
			tabelaLinha.DefaultCell.PaddingLeft = 3;
			tabelaLinha.DefaultCell.PaddingRight = 3;
			tabelaLinha.DefaultCell.PaddingBottom = 3;
			tabelaLinha.DefaultCell.PaddingTop = 3;
			tabelaLinha.DefaultCell.BackgroundColor = corCinzaClaro;
			tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
			tabelaLinha.DefaultCell.VerticalAlignment = Element.ALIGN_MIDDLE;
			tabelaLinha.SpacingBefore = 15f;

			tabelaLinha.AddCell(new Phrase(new Chunk("Coordenada Nº", PdfMetodosAuxiliares.arial8Negrito)));
			tabelaLinha.AddCell(new Phrase(new Chunk("Norte", PdfMetodosAuxiliares.arial8Negrito)));
			tabelaLinha.AddCell(new Phrase(new Chunk("Este", PdfMetodosAuxiliares.arial8Negrito)));
			tabelaLinha.AddCell(new Phrase(new Chunk("Azimute", PdfMetodosAuxiliares.arial8Negrito)));
			tabelaLinha.AddCell(new Phrase(new Chunk("Distância (m)", PdfMetodosAuxiliares.arial8Negrito)));
			tabelaLinha.AddCell(vazia);

			tabelaLinha.DefaultCell.BackgroundColor = null;
			tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_RIGHT;

			List<Decimal> ordenadas = hashData["ORDENADAS_ATP"] as List<Decimal>;

			if (ordenadas.Count > 1)
			{
				Decimal lastX = Convert.ToDecimal(ordenadas[0]);
				Decimal lastY = Convert.ToDecimal(ordenadas[1]);

				tabelaLinha.AddCell(new Phrase(new Chunk('1', PdfMetodosAuxiliares.arial8)));
				tabelaLinha.AddCell(new Phrase(new Chunk(FormatNumber(lastY), PdfMetodosAuxiliares.arial8)));
				tabelaLinha.AddCell(new Phrase(new Chunk(FormatNumber(lastX), PdfMetodosAuxiliares.arial8)));

				tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
				tabelaLinha.AddCell("-");
				tabelaLinha.AddCell("-");
				tabelaLinha.AddCell(vazia);
				tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_RIGHT;

				Double azimute;
				Double grau;
				Double minuto;
				Double segundo;

				string gms;

				for (int k = 3; k <= ordenadas.Count; k += 2)
				{
					tabelaLinha.AddCell(new Phrase(new Chunk(((k + 1) / 2).ToString(), PdfMetodosAuxiliares.arial8)));
					tabelaLinha.AddCell(new Phrase(new Chunk(FormatNumber(ordenadas[k]), PdfMetodosAuxiliares.arial8)));
					tabelaLinha.AddCell(new Phrase(new Chunk(FormatNumber(ordenadas[k - 1]), PdfMetodosAuxiliares.arial8)));

					azimute = Decimal.ToDouble(CoordenadaBus.CalcularAzimute(lastX, lastY, ordenadas[k - 1], ordenadas[k]));
					if (azimute < 0)
					{
						gms = "-";
						azimute = -azimute;
					}
					else
					{
						gms = "";
					}

					grau = Math.Floor(azimute);
					azimute = (azimute - grau) * 60;
					minuto = Math.Floor(azimute);
					azimute = (azimute - minuto) * 60;
					segundo = azimute;

					gms += ((grau < 10) ? "0" : "") + grau.ToString() + "°";
					gms += ((minuto < 10) ? "0" : "") + minuto.ToString() + "'";
					gms += ((segundo < 10) ? "0" : "") + FormatNumber(segundo, 0) + "\"";

					tabelaLinha.AddCell(new Phrase(new Chunk(gms, PdfMetodosAuxiliares.arial8)));
					tabelaLinha.AddCell(new Phrase(new Chunk(FormatNumber(CoordenadaBus.CalcularDistancia(lastX, lastY, ordenadas[k - 1], ordenadas[k]), 3), PdfMetodosAuxiliares.arial8)));
					tabelaLinha.AddCell(vazia);

					lastX = Convert.ToDecimal(ordenadas[k - 1]);
					lastY = Convert.ToDecimal(ordenadas[k]);
				}
			}

			doc.Add(tabelaLinha);

			tabelaDocumento = new PdfPTable(1);
			tabelaDocumento.WidthPercentage = 100;
			tabelaDocumento.SetWidths(new float[] { 100 });
			tabelaDocumento.SplitLate = false;
			tabelaDocumento.SplitRows = true;

			tabelaDocumento.DefaultCell.Border = 0;
			tabelaDocumento.DefaultCell.PaddingLeft = 0;
			tabelaDocumento.DefaultCell.PaddingRight = 0;
			tabelaDocumento.DefaultCell.PaddingBottom = 2;
			tabelaDocumento.DefaultCell.PaddingTop = 2;
			tabelaDocumento.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;

			//Coordenadas da ARL
			tabelaDocumento.AddCell("\n");
			tabelaDocumento.DefaultCell.PaddingBottom = 10;
			tabelaDocumento.AddCell(new Phrase(new Chunk("Lista de Coordenadas da ARL (SIRGAS 2000 / UTM zone 24S)", PdfMetodosAuxiliares.arial10Negrito)));
			tabelaDocumento.DefaultCell.PaddingBottom = 2;

			doc.Add(tabelaDocumento);

			tabelaLinha = new PdfPTable(new float[] { 0.9F, 0.9F, 1.2F, 1, 1, 1, 1 });
			tabelaLinha.HeaderRows = 1;
			tabelaLinha.DefaultCell.PaddingLeft = 3;
			tabelaLinha.DefaultCell.PaddingRight = 3;
			tabelaLinha.DefaultCell.PaddingBottom = 3;
			tabelaLinha.DefaultCell.PaddingTop = 3;
			tabelaLinha.DefaultCell.BackgroundColor = corCinzaClaro;
			tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
			tabelaLinha.DefaultCell.VerticalAlignment = Element.ALIGN_MIDDLE;
			tabelaLinha.SpacingBefore = 15f;

			tabelaLinha.AddCell(new Phrase(new Chunk("ARL", PdfMetodosAuxiliares.arial8Negrito)));
			tabelaLinha.AddCell(new Phrase(new Chunk("Coordenada Nº", PdfMetodosAuxiliares.arial8Negrito)));
			tabelaLinha.AddCell(new Phrase(new Chunk("Norte", PdfMetodosAuxiliares.arial8Negrito)));
			tabelaLinha.AddCell(new Phrase(new Chunk("Este", PdfMetodosAuxiliares.arial8Negrito)));
			tabelaLinha.AddCell(new Phrase(new Chunk("Azimute", PdfMetodosAuxiliares.arial8Negrito)));
			tabelaLinha.AddCell(new Phrase(new Chunk("Distância (m)", PdfMetodosAuxiliares.arial8Negrito)));
			tabelaLinha.AddCell(vazia);

			tabelaLinha.DefaultCell.BackgroundColor = null;
			tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_RIGHT;

			Dictionary<String, List<Decimal>> dicOrdArls = hashData["ORDENADAS_ARL"] as Dictionary<String, List<Decimal>>;

			foreach (var arlkey in dicOrdArls.Keys)
			{
				List<Decimal> lstOrdenadasArls = dicOrdArls[arlkey];

				if (lstOrdenadasArls != null && lstOrdenadasArls.Count > 1)
				{
					Decimal lastX = Convert.ToDecimal(lstOrdenadasArls[0]);
					Decimal lastY = Convert.ToDecimal(lstOrdenadasArls[1]);

					tabelaLinha.AddCell(new Phrase(new Chunk(arlkey, PdfMetodosAuxiliares.arial8)));
					tabelaLinha.AddCell(new Phrase(new Chunk('1', PdfMetodosAuxiliares.arial8)));
					tabelaLinha.AddCell(new Phrase(new Chunk(FormatNumber(lastY), PdfMetodosAuxiliares.arial8)));
					tabelaLinha.AddCell(new Phrase(new Chunk(FormatNumber(lastX), PdfMetodosAuxiliares.arial8)));

					tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
					tabelaLinha.AddCell("-");
					tabelaLinha.AddCell("-");
					tabelaLinha.AddCell(vazia);
					tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_RIGHT;

					Double azimute;
					Double grau;
					Double minuto;
					Double segundo;

					string gms;

					for (int k = 3; k <= lstOrdenadasArls.Count; k += 2)
					{
						tabelaLinha.AddCell(new Phrase(new Chunk(arlkey, PdfMetodosAuxiliares.arial8)));
						tabelaLinha.AddCell(new Phrase(new Chunk(((k + 1) / 2).ToString(), PdfMetodosAuxiliares.arial8)));
						tabelaLinha.AddCell(new Phrase(new Chunk(FormatNumber(lstOrdenadasArls[k]), PdfMetodosAuxiliares.arial8)));
						tabelaLinha.AddCell(new Phrase(new Chunk(FormatNumber(lstOrdenadasArls[k - 1]), PdfMetodosAuxiliares.arial8)));

						azimute = Decimal.ToDouble(CoordenadaBus.CalcularAzimute(lastX, lastY, lstOrdenadasArls[k - 1], lstOrdenadasArls[k]));
						if (azimute < 0)
						{
							gms = "-";
							azimute = -azimute;
						}
						else
						{
							gms = "";
						}

						grau = Math.Floor(azimute);
						azimute = (azimute - grau) * 60;
						minuto = Math.Floor(azimute);
						azimute = (azimute - minuto) * 60;
						segundo = azimute;

						gms += ((grau < 10) ? "0" : "") + grau.ToString() + "°";
						gms += ((minuto < 10) ? "0" : "") + minuto.ToString() + "'";
						gms += ((segundo < 10) ? "0" : "") + FormatNumber(segundo, 0) + "\"";

						tabelaLinha.AddCell(new Phrase(new Chunk(gms, PdfMetodosAuxiliares.arial8)));
						tabelaLinha.AddCell(new Phrase(new Chunk(FormatNumber(CoordenadaBus.CalcularDistancia(lastX, lastY, lstOrdenadasArls[k - 1], lstOrdenadasArls[k]), 3), PdfMetodosAuxiliares.arial8)));
						tabelaLinha.AddCell(vazia);

						lastX = Convert.ToDecimal(lstOrdenadasArls[k - 1]);
						lastY = Convert.ToDecimal(lstOrdenadasArls[k]);
					}
				}
			}

			doc.Add(tabelaLinha);

			#endregion Lista de Coordenadas
		}

		#endregion

		#region Atividade

		internal MemoryStream GerarPdfAtividade(MxdLayout mxd)
		{
			Document doc = new Document(mxd.MxdPageSize, 85, 40, 73, 50);

			MemoryStream ms = new MemoryStream();
			PdfWriter wrt = PdfWriter.GetInstance(doc, ms);
			//wrt.SetEncryption(PdfWriter.STRENGTH128BITS, null, null, PdfWriter.AllowCopy | PdfWriter.AllowPrinting);


			//Cabecalho e Rodape
			Hashtable htConfiguracoes = ObterDadosCabecalhoRodapePDF();

			PdfCabecalhoRodape headerFooter = new PdfCabecalhoRodape();

			headerFooter.GovernoNome = HasKey(htConfiguracoes, "GOVERNO_NOME") ? htConfiguracoes["GOVERNO_NOME"].ToString() : null;
			headerFooter.OrgaoCep = HasKey(htConfiguracoes, "ORGAO_CEP") ? htConfiguracoes["ORGAO_CEP"].ToString() : null;
			headerFooter.OrgaoContato = HasKey(htConfiguracoes, "ORGAO_CONTATO") ? htConfiguracoes["ORGAO_CONTATO"].ToString() : null;
			headerFooter.OrgaoEndereco = HasKey(htConfiguracoes, "ORGAO_ENDERECO") ? htConfiguracoes["ORGAO_ENDERECO"].ToString() : null;
			headerFooter.OrgaoMunicipio = HasKey(htConfiguracoes, "ORGAO_MUNICIPIO") ? htConfiguracoes["ORGAO_MUNICIPIO"].ToString() : null;
			headerFooter.OrgaoNome = HasKey(htConfiguracoes, "ORGAO_NOME") ? htConfiguracoes["ORGAO_NOME"].ToString() : null;
			headerFooter.OrgaoSigla = HasKey(htConfiguracoes, "ORGAO_SIGLA") ? htConfiguracoes["ORGAO_SIGLA"].ToString() : null;
			headerFooter.OrgaoUF = HasKey(htConfiguracoes, "ORGAO_UF") ? htConfiguracoes["ORGAO_UF"].ToString() : null;
			headerFooter.SecretariaNome = HasKey(htConfiguracoes, "SECRETARIA_NOME") ? htConfiguracoes["SECRETARIA_NOME"].ToString() : null;
			headerFooter.SetorNome = HasKey(htConfiguracoes, "SETOR_NOME") ? htConfiguracoes["SETOR_NOME"].ToString() : null;

			wrt.PageEvent = headerFooter;
			//------------------------

			doc.Open();

			Hashtable hashData = ObterDadosPDF();

			mxd.GerarPdf(doc, wrt, Project.Id, hashData);
			GerarVersoAtividade(doc, wrt, hashData);

			doc.Close();

			mxd.ApagarTempFile();

			return ms;
		}

		internal MemoryStream GerarPdfAtividadePorTitulo(MxdLayout mxd, out int titulo)
		{
			Document doc = new Document(mxd.MxdPageSize, 85, 40, 73, 50);

			MemoryStream ms = new MemoryStream();
			PdfWriter wrt = PdfWriter.GetInstance(doc, ms);

			//Cabecalho e Rodape
			Hashtable htConfiguracoes = ObterDadosCabecalhoRodapePDF();

			PdfCabecalhoRodape headerFooter = new PdfCabecalhoRodape();

			headerFooter.GovernoNome = HasKey(htConfiguracoes, "GOVERNO_NOME") ? htConfiguracoes["GOVERNO_NOME"].ToString() : null;
			headerFooter.OrgaoCep = HasKey(htConfiguracoes, "ORGAO_CEP") ? htConfiguracoes["ORGAO_CEP"].ToString() : null;
			headerFooter.OrgaoContato = HasKey(htConfiguracoes, "ORGAO_CONTATO") ? htConfiguracoes["ORGAO_CONTATO"].ToString() : null;
			headerFooter.OrgaoEndereco = HasKey(htConfiguracoes, "ORGAO_ENDERECO") ? htConfiguracoes["ORGAO_ENDERECO"].ToString() : null;
			headerFooter.OrgaoMunicipio = HasKey(htConfiguracoes, "ORGAO_MUNICIPIO") ? htConfiguracoes["ORGAO_MUNICIPIO"].ToString() : null;
			headerFooter.OrgaoNome = HasKey(htConfiguracoes, "ORGAO_NOME") ? htConfiguracoes["ORGAO_NOME"].ToString() : null;
			headerFooter.OrgaoSigla = HasKey(htConfiguracoes, "ORGAO_SIGLA") ? htConfiguracoes["ORGAO_SIGLA"].ToString() : null;
			headerFooter.OrgaoUF = HasKey(htConfiguracoes, "ORGAO_UF") ? htConfiguracoes["ORGAO_UF"].ToString() : null;
			headerFooter.SecretariaNome = HasKey(htConfiguracoes, "SECRETARIA_NOME") ? htConfiguracoes["SECRETARIA_NOME"].ToString() : null;
			headerFooter.SetorNome = HasKey(htConfiguracoes, "SETOR_NOME") ? htConfiguracoes["SETOR_NOME"].ToString() : null;

			wrt.PageEvent = headerFooter;
			//------------------------

			doc.Open();

			Hashtable hashData = ObterDadosPDF();

			titulo = (int)hashData["TITULO"];

			mxd.GerarPdf(doc, wrt, Project.Id, hashData);
			GerarVersoAtividade(doc, wrt, hashData);

			doc.Close();

			mxd.ApagarTempFile();

			return ms;
		}

		private void GerarVersoAtividade(Document doc, PdfWriter wrt, Hashtable hashData)
		{

			BaseColor corCinzaEscuro = new BaseColor(102, 102, 102);
			BaseColor corCinzaClaro = new BaseColor(220, 220, 220);

			Font arial8BoldBlue = PdfMetodosAuxiliares.GerarFonte(tipoFonte.Arial, 8, iTextSharp.text.Font.BOLD, BaseColor.BLUE);
			Font arial8BoldRed = PdfMetodosAuxiliares.GerarFonte(tipoFonte.Arial, 8, iTextSharp.text.Font.BOLD, BaseColor.RED);

			PdfPTable tabelaDocumento;
			PdfPTable tabelaLinha;

			List<Hashtable> hashList = null;

			tabelaDocumento = new PdfPTable(1);
			tabelaDocumento.WidthPercentage = 100;
			tabelaDocumento.SetWidths(new float[] { 100 });
			tabelaDocumento.SplitLate = true;
			tabelaDocumento.SplitRows = true;

			tabelaDocumento.DefaultCell.Border = 0;
			tabelaDocumento.DefaultCell.PaddingLeft = 0;
			tabelaDocumento.DefaultCell.PaddingRight = 0;
			tabelaDocumento.DefaultCell.PaddingBottom = 2;
			tabelaDocumento.DefaultCell.PaddingTop = 2;
			tabelaDocumento.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
			//tabelaDocumento.HeaderRows = 0;


			//Titulo
			tabelaDocumento.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
			tabelaDocumento.AddCell(new Phrase(new Chunk("Quadros de medidas da atividade", PdfMetodosAuxiliares.arial16Negrito)));
			tabelaDocumento.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;


			if ((hashData["QUADRO_TOTAL"] != null) && (hashData["QUADRO_TOTAL"] != DBNull.Value))
				hashList = hashData["QUADRO_TOTAL"] as List<Hashtable>;
			else
				hashList = new List<Hashtable>();

			//Áreas e Perímetros de Matrícula/Posse
			tabelaDocumento.AddCell("\n");
			tabelaDocumento.DefaultCell.PaddingBottom = 10;
			tabelaDocumento.AddCell(new Phrase(new Chunk("Informações por Matrícula/Posse", PdfMetodosAuxiliares.arial10Negrito)));
			tabelaDocumento.DefaultCell.PaddingBottom = 2;


			tabelaLinha = new PdfPTable(new float[] { 1, 1, 1, 1, 1, 1 });
			tabelaLinha.DefaultCell.PaddingLeft = 3;
			tabelaLinha.DefaultCell.PaddingRight = 3;
			tabelaLinha.DefaultCell.PaddingBottom = 3;
			tabelaLinha.DefaultCell.PaddingTop = 3;
			tabelaLinha.DefaultCell.BackgroundColor = corCinzaClaro;
			tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
			tabelaDocumento.HeaderRows = 1;

			tabelaLinha.AddCell(new Phrase(new Chunk("Nome", PdfMetodosAuxiliares.arial8Negrito)));
			tabelaLinha.AddCell(new Phrase(new Chunk("Tipo", PdfMetodosAuxiliares.arial8Negrito)));
			tabelaLinha.AddCell(new Phrase(new Chunk("Quantidade de PATIV (unid.)", PdfMetodosAuxiliares.arial8Negrito)));
			tabelaLinha.AddCell(new Phrase(new Chunk("Comprimento de LATIV (m)", PdfMetodosAuxiliares.arial8Negrito)));
			tabelaLinha.AddCell(new Phrase(new Chunk("Área de AATIV (m²)", PdfMetodosAuxiliares.arial8Negrito)));
			tabelaLinha.AddCell(new Phrase(new Chunk("Área de AIATIV (m²)", PdfMetodosAuxiliares.arial8Negrito)));

			tabelaLinha.DefaultCell.BackgroundColor = null;
			tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;

			decimal totalPATIV = 0;
			decimal totalLATIV = 0;
			decimal totalAATIV = 0;
			decimal totalAIATIV = 0;

			foreach (Hashtable ht in hashList)
			{
				if (FormatStringField(ht, "APMP_NOME") == "-")
					tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;

				tabelaLinha.AddCell(new Phrase(new Chunk(FormatStringField(ht, "APMP_NOME"), PdfMetodosAuxiliares.arial8)));
				tabelaLinha.AddCell(new Phrase(new Chunk(FormatStringField(ht, "APMP_TIPO"), PdfMetodosAuxiliares.arial8)));

				totalPATIV += Convert.ToDecimal(ht["PATIV_QUANTIDADE"]);
				totalLATIV += Convert.ToDecimal(ht["LATIV_COMPRIMENTO"]);
				totalAATIV += Convert.ToDecimal(ht["AATIV_AREA_M2"]);
				totalAIATIV += Convert.ToDecimal(ht["AIATIV_AREA_M2"]);

				tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_RIGHT;
				tabelaLinha.AddCell(new Phrase(new Chunk(FormatNumberField(ht, "PATIV_QUANTIDADE", 0), PdfMetodosAuxiliares.arial8)));

				AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumberField(ht, "LATIV_COMPRIMENTO", 3));
				AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumberField(ht, "AATIV_AREA_M2"));
				AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumberField(ht, "AIATIV_AREA_M2"));

				tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
			}

			tabelaLinha.DefaultCell.BackgroundColor = corCinzaClaro;
			tabelaLinha.AddCell(new Phrase(new Chunk("Total", PdfMetodosAuxiliares.arial8Negrito)));
			tabelaLinha.AddCell(new Phrase(new Chunk("", PdfMetodosAuxiliares.arial8Negrito)));

			tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_RIGHT;
			tabelaLinha.AddCell(new Phrase(new Chunk(FormatNumber(totalPATIV, 0), PdfMetodosAuxiliares.arial8Negrito)));

			AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8Negrito, FormatNumber(totalLATIV, 3));
			AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8Negrito, FormatNumber(totalAATIV));
			AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8Negrito, FormatNumber(totalAIATIV));

			//tabelaLinha.AddCell(new Phrase(new Chunk(formatNumber(totalLATIV, 3), PdfMetodosAuxiliares.arial8Negrito)));
			//tabelaLinha.AddCell(new Phrase(new Chunk(formatNumber(totalAATIV), PdfMetodosAuxiliares.arial8Negrito)));
			//tabelaLinha.AddCell(new Phrase(new Chunk(formatNumber(totalAIATIV), PdfMetodosAuxiliares.arial8Negrito)));
			tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;


			tabelaDocumento.AddCell(tabelaLinha);


			//------------------------------------------------------------------------------------
			string[] ativArray = new string[] { "QUADRO_PATIV", "QUADRO_LATIV", "QUADRO_AATIV", "QUADRO_AIATIV" };
			string[] titleArray = new string[] { "PATIV - Pontos da Atividade e suas relações", "LATIV - Linhas da Atividade e suas relações", "AATIV - Áreas da Atividade e suas relações", "AIATIV - Áreas de Influência da Atividade e suas relações" };
			string[] headerArray = new string[] { "Sobreposição com PATIV", "Sobreposição com coordenada do ponto médio da LATIV", "Sobreposição com coordenada interna da AATIV", "Sobreposição com coordenada interna da AIATIV" };
			string[] header2Array = new string[] { "Sobreposição com PATIV", "Sobreposição com LATIV", "Sobreposição com AATIV", "Sobreposição com AIATIV" };

			bool showCoordinates = false;
			for (int i = 0; i < 4; i++)
			{
				string quadroAtividade = ativArray[i];

				if ((hashData[quadroAtividade] != null) && (hashData[quadroAtividade] != DBNull.Value))
					hashList = hashData[quadroAtividade] as List<Hashtable>;
				else
					hashList = new List<Hashtable>();


				//Pontos da atividade - PATIV
				tabelaDocumento.AddCell("\n");
				tabelaDocumento.DefaultCell.PaddingBottom = 10;


				//tabelaDocumento.AddCell(new Phrase(new Chunk(titleArray[i], PdfMetodosAuxiliares.arial10Negrito)));
				tabelaDocumento.AddCell(new Phrase(new Chunk(titleArray[i], PdfMetodosAuxiliares.arial10Negrito)));

				if (hashList.Count == 0)
				{
					tabelaDocumento.AddCell(new Phrase(new Chunk("- Nenhum registro encontrado.", PdfMetodosAuxiliares.arial8)));
				}
				else
				{
					showCoordinates = (totalLATIV > 0 || totalAATIV > 0 || totalAIATIV > 0);
					tabelaDocumento.DefaultCell.PaddingBottom = 2;

					if (i == 0)
						tabelaLinha = new PdfPTable(new float[] { 8, 15, 10, 10, 6F, 12F, 18.5F, 18.5F, 6F, 6F, 6F, 6F, 10 });
					else
						tabelaLinha = new PdfPTable(new float[] { 8, 15, 8, 6, 7, 13, 18.5F, 18.5F, 7, 7, 7, 7, 10 });

					tabelaLinha.DefaultCell.PaddingLeft = 3;
					tabelaLinha.DefaultCell.PaddingRight = 3;
					tabelaLinha.DefaultCell.PaddingBottom = 3;
					tabelaLinha.DefaultCell.PaddingTop = 3;
					tabelaLinha.DefaultCell.BackgroundColor = corCinzaClaro;
					tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
					tabelaDocumento.HeaderRows = 2;

					tabelaLinha.DefaultCell.Rowspan = 2;
					tabelaLinha.DefaultCell.VerticalAlignment = Element.ALIGN_MIDDLE;
					tabelaLinha.AddCell(new Phrase(new Chunk("Código", PdfMetodosAuxiliares.arial8Negrito)));
					tabelaLinha.AddCell(new Phrase(new Chunk("Matrícula/Posse", PdfMetodosAuxiliares.arial8Negrito)));
					//coluna opcional

					tabelaLinha.DefaultCell.Colspan = 2;
					if (i > 0)
					{
						tabelaLinha.AddCell(new Phrase(new Chunk((i == 1) ? "Comprimento (m)" : "Área (m²)", PdfMetodosAuxiliares.arial8Negrito)));
						tabelaLinha.DefaultCell.Rowspan = 1;
					}
					else
					{
						tabelaLinha.DefaultCell.Rowspan = 1;
						tabelaLinha.AddCell(new Phrase(new Chunk("Coordenada", PdfMetodosAuxiliares.arial8Negrito)));
					}
					tabelaLinha.DefaultCell.Colspan = 1;


					if (i == 0)
					{
						tabelaLinha.DefaultCell.Colspan = 9;
						tabelaLinha.AddCell(new Phrase(new Chunk(headerArray[i], PdfMetodosAuxiliares.arial8Negrito)));
						tabelaLinha.DefaultCell.Colspan = 1;
					}
					else
					{
						tabelaLinha.DefaultCell.Colspan = 6;
						tabelaLinha.AddCell(new Phrase(new Chunk(headerArray[i], PdfMetodosAuxiliares.arial8Negrito)));
						tabelaLinha.DefaultCell.Colspan = 3;
						tabelaLinha.AddCell(new Phrase(new Chunk(header2Array[i], PdfMetodosAuxiliares.arial8Negrito)));
						tabelaLinha.DefaultCell.Colspan = 1;
					}

					if (i == 0)
					{
						tabelaLinha.AddCell(new Phrase(new Chunk("Norte", PdfMetodosAuxiliares.arial8Negrito)));
						tabelaLinha.AddCell(new Phrase(new Chunk("Este", PdfMetodosAuxiliares.arial8Negrito)));
					}

					tabelaLinha.AddCell(new Phrase(new Chunk("Rocha", PdfMetodosAuxiliares.arial8Negrito)));
					tabelaLinha.AddCell(new Phrase(new Chunk("Massa d'água", PdfMetodosAuxiliares.arial8Negrito)));
					tabelaLinha.AddCell(new Phrase(new Chunk("AVN", PdfMetodosAuxiliares.arial8Negrito)));
					tabelaLinha.AddCell(new Phrase(new Chunk("AA", PdfMetodosAuxiliares.arial8Negrito)));
					tabelaLinha.AddCell(new Phrase(new Chunk("AFS", PdfMetodosAuxiliares.arial8Negrito)));
					tabelaLinha.AddCell(new Phrase(new Chunk("RPPN", PdfMetodosAuxiliares.arial8Negrito)));

					tabelaLinha.AddCell(new Phrase(new Chunk("ARL", PdfMetodosAuxiliares.arial8Negrito)));
					tabelaLinha.AddCell(new Phrase(new Chunk("APP", PdfMetodosAuxiliares.arial8Negrito)));
					tabelaLinha.AddCell(new Phrase(new Chunk("Declividade", PdfMetodosAuxiliares.arial8Negrito)));

					tabelaLinha.DefaultCell.BackgroundColor = null;
					tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;

					/*
					if (i == 2)
					{
						//hashList.RemoveAt(0);
						Hashtable hs = new Hashtable();
						hs["CODIGO"] = "TST1";
						hs["AVN"] = "I";
						hashList.Add(hs);
                        
						hs = new Hashtable();
						hs["CODIGO"] = "TST2";
						hs["AVN"] = "M";
						hashList.Add(hs);

						hs = new Hashtable();
						hs["CODIGO"] = "TST3";
						hs["AVN"] = "A";
						hashList.Add(hs);

						hs = new Hashtable();
						hs["CODIGO"] = "TST4";
						hs["AVN"] = "D";
						hashList.Add(hs);

						//for (int k = 0; k<35; k++)
						//    hashList.Add(hashList[0]);
					}
					*/



					foreach (Hashtable ht in hashList)
					{
						tabelaLinha.AddCell(new Phrase(new Chunk(FormatStringField(ht, "CODIGO"), PdfMetodosAuxiliares.arial8)));

						tabelaLinha.AddCell(new Phrase(new Chunk(FormatStringField(ht, "APMP_NOME"), PdfMetodosAuxiliares.arial8)));

						tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_RIGHT;
						if (i > 0)
						{
							tabelaLinha.DefaultCell.Colspan = 2;
							tabelaLinha.AddCell(new Phrase(new Chunk(FormatNumberField(ht, (i > 1) ? "AREA_M2" : "COMPRIMENTO"), PdfMetodosAuxiliares.arial8)));
							tabelaLinha.DefaultCell.Colspan = 1;
						}
						else
						{

							tabelaLinha.AddCell(new Phrase(new Chunk(FormatNumber(((List<Decimal>)ht["ORDENADAS"])[1]), PdfMetodosAuxiliares.arial8)));
							tabelaLinha.AddCell(new Phrase(new Chunk(FormatNumber(((List<Decimal>)ht["ORDENADAS"])[0]), PdfMetodosAuxiliares.arial8)));
						}

						tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
						tabelaLinha.AddCell(new Phrase(new Chunk(FormatSNField(ht, "ROCHA"), PdfMetodosAuxiliares.arial8)));
						tabelaLinha.AddCell(new Phrase(new Chunk(FormatSNField(ht, "MASSA_DAGUA"), PdfMetodosAuxiliares.arial8)));
						tabelaLinha.AddCell(new Phrase(new Chunk(FormatSNField(ht, "AVN"), PdfMetodosAuxiliares.arial8)));
						tabelaLinha.AddCell(new Phrase(new Chunk(FormatSNField(ht, "AA"), PdfMetodosAuxiliares.arial8)));
						tabelaLinha.AddCell(new Phrase(new Chunk(FormatSNField(ht, "AFS"), PdfMetodosAuxiliares.arial8)));
						tabelaLinha.AddCell(new Phrase(new Chunk(FormatSNField(ht, "RPPN"), PdfMetodosAuxiliares.arial8)));
						tabelaLinha.AddCell(new Phrase(new Chunk(FormatSNField(ht, "ARL"), PdfMetodosAuxiliares.arial8)));
						tabelaLinha.AddCell(new Phrase(new Chunk(FormatSNField(ht, "APP"), PdfMetodosAuxiliares.arial8)));
						tabelaLinha.AddCell(new Phrase(new Chunk(FormatSNField(ht, "REST_DECLIVIDADE"), PdfMetodosAuxiliares.arial8)));


						tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
					}


					tabelaDocumento.AddCell(tabelaLinha);
				}
			}




			doc.SetPageSize(doc.PageSize.Rotate());
			doc.NewPage();
			doc.Add(tabelaDocumento);

			//------------------------------------------------------------------------------------


			if (showCoordinates)
			{
				//------------------------------------------------------------------------------------
				doc.SetPageSize(doc.PageSize.Rotate());
				doc.NewPage();

				tabelaDocumento = new PdfPTable(1);
				tabelaDocumento.WidthPercentage = 100;
				tabelaDocumento.SetWidths(new float[] { 100 });
				tabelaDocumento.SplitLate = false;
				tabelaDocumento.SplitRows = true;

				tabelaDocumento.DefaultCell.Border = 0;
				tabelaDocumento.DefaultCell.PaddingLeft = 0;
				tabelaDocumento.DefaultCell.PaddingRight = 0;
				tabelaDocumento.DefaultCell.PaddingBottom = 2;
				tabelaDocumento.DefaultCell.PaddingTop = 2;
				tabelaDocumento.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;


				//Coordenadas da ATP
				tabelaDocumento.AddCell("\n");
				tabelaDocumento.DefaultCell.PaddingBottom = 0;
				tabelaDocumento.AddCell(new Phrase(new Chunk("Lista de Coordenadas das Atividades (SIRGAS 2000 / UTM zone 24S)", PdfMetodosAuxiliares.arial10Negrito)));
				tabelaDocumento.AddCell("\n");

				doc.Add(tabelaDocumento);


				for (int i = 1; i < 4; i++)
				{
					if ((hashData[ativArray[i]] != null) && (hashData[ativArray[i]] != DBNull.Value))
						hashList = hashData[ativArray[i]] as List<Hashtable>;
					else
						hashList = new List<Hashtable>();

					foreach (Hashtable ht in hashList)
					{
						doc.Add(new Chunk("\n"));
						doc.Add(GerarListaDeCoordenadasDaAtividade(ht, ativArray[i].Replace("QUADRO_", ""), corCinzaClaro));
					}
				}
			}
		}

		#region Regularização Fundiária

		internal MemoryStream GerarPdfRegularizacao(MxdLayout mxd)
		{
			Document doc = new Document(mxd.MxdPageSize, 85, 40, 73, 50);

			MemoryStream ms = new MemoryStream();
			PdfWriter wrt = PdfWriter.GetInstance(doc, ms);
			
			//Cabecalho e Rodape
			Hashtable htConfiguracoes = ObterDadosCabecalhoRodapePDF();

			PdfCabecalhoRodape headerFooter = new PdfCabecalhoRodape();

			headerFooter.GovernoNome = HasKey(htConfiguracoes, "GOVERNO_NOME") ? htConfiguracoes["GOVERNO_NOME"].ToString() : null;
			headerFooter.OrgaoCep = HasKey(htConfiguracoes, "ORGAO_CEP") ? htConfiguracoes["ORGAO_CEP"].ToString() : null;
			headerFooter.OrgaoContato = HasKey(htConfiguracoes, "ORGAO_CONTATO") ? htConfiguracoes["ORGAO_CONTATO"].ToString() : null;
			headerFooter.OrgaoEndereco = HasKey(htConfiguracoes, "ORGAO_ENDERECO") ? htConfiguracoes["ORGAO_ENDERECO"].ToString() : null;
			headerFooter.OrgaoMunicipio = HasKey(htConfiguracoes, "ORGAO_MUNICIPIO") ? htConfiguracoes["ORGAO_MUNICIPIO"].ToString() : null;
			headerFooter.OrgaoNome = HasKey(htConfiguracoes, "ORGAO_NOME") ? htConfiguracoes["ORGAO_NOME"].ToString() : null;
			headerFooter.OrgaoSigla = HasKey(htConfiguracoes, "ORGAO_SIGLA") ? htConfiguracoes["ORGAO_SIGLA"].ToString() : null;
			headerFooter.OrgaoUF = HasKey(htConfiguracoes, "ORGAO_UF") ? htConfiguracoes["ORGAO_UF"].ToString() : null;
			headerFooter.SecretariaNome = HasKey(htConfiguracoes, "SECRETARIA_NOME") ? htConfiguracoes["SECRETARIA_NOME"].ToString() : null;
			headerFooter.SetorNome = HasKey(htConfiguracoes, "SETOR_NOME") ? htConfiguracoes["SETOR_NOME"].ToString() : null;

			wrt.PageEvent = headerFooter;
			//------------------------

			doc.Open();

			Hashtable hashData = ObterDadosPDF();

			mxd.GerarPdf(doc, wrt, Project.Id, hashData);
			GerarVersaoRegularizacao(doc, wrt, hashData);

			doc.Close();

			mxd.ApagarTempFile();

			return ms;
		}

		private void GerarVersaoRegularizacao(Document doc, PdfWriter wrt, Hashtable hashData)
		{

			BaseColor corCinzaEscuro = new BaseColor(102, 102, 102);
			BaseColor corCinzaClaro = new BaseColor(220, 220, 220);

			Font arial8BoldBlue = PdfMetodosAuxiliares.GerarFonte(tipoFonte.Arial, 8, iTextSharp.text.Font.BOLD, BaseColor.BLUE);
			Font arial8BoldRed = PdfMetodosAuxiliares.GerarFonte(tipoFonte.Arial, 8, iTextSharp.text.Font.BOLD, BaseColor.RED);

			PdfPTable tabelaDocumento;
			PdfPTable tabelaLinha;

			List<Hashtable> hashList = null;

			tabelaDocumento = new PdfPTable(1);
			tabelaDocumento.WidthPercentage = 100;
			tabelaDocumento.SetWidths(new float[] { 100 });
			tabelaDocumento.SplitLate = true;
			tabelaDocumento.SplitRows = true;

			tabelaDocumento.DefaultCell.Border = 0;
			tabelaDocumento.DefaultCell.PaddingLeft = 0;
			tabelaDocumento.DefaultCell.PaddingRight = 0;
			tabelaDocumento.DefaultCell.PaddingBottom = 2;
			tabelaDocumento.DefaultCell.PaddingTop = 2;
			tabelaDocumento.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
			//tabelaDocumento.HeaderRows = 0;

			//Titulo
			tabelaDocumento.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
			tabelaDocumento.AddCell(new Phrase(new Chunk("Quadros de medidas da atividade", PdfMetodosAuxiliares.arial16Negrito)));
			tabelaDocumento.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;


			if ((hashData["QUADRO_TOTAL"] != null) && (hashData["QUADRO_TOTAL"] != DBNull.Value))
				hashList = hashData["QUADRO_TOTAL"] as List<Hashtable>;
			else
				hashList = new List<Hashtable>();

			//Áreas e Perímetros de Matrícula/Posse
			//tabelaDocumento.AddCell("\n");
			//tabelaDocumento.DefaultCell.PaddingBottom = 10;
			//tabelaDocumento.AddCell(new Phrase(new Chunk("Informações por Matrícula/Posse", PdfMetodosAuxiliares.arial10Negrito)));
			//tabelaDocumento.DefaultCell.PaddingBottom = 2;

			decimal totalAATIV = 0;

			foreach (Hashtable ht in hashList)
			{
				totalAATIV += Convert.ToDecimal(ht["AATIV_AREA_M2"]);
			}		

			//------------------------------------------------------------------------------------

			bool showCoordinates = false;
			string ativ = "QUADRO_AATIV";
			string title = "AATIV - Áreas da Atividade e suas relações";
			string header = "Sobreposição com coordenada interna da AATIV";
			string header2 = "Sobreposição com AATIV";
			int i = 0;

			if ((hashData[ativ] != null) && (hashData[ativ] != DBNull.Value))
				hashList = hashData[ativ] as List<Hashtable>;
			else
				hashList = new List<Hashtable>();


			//Pontos da atividade - PATIV
			tabelaDocumento.AddCell("\n");
			tabelaDocumento.DefaultCell.PaddingBottom = 10;


			//tabelaDocumento.AddCell(new Phrase(new Chunk(titleArray[i], PdfMetodosAuxiliares.arial10Negrito)));
			tabelaDocumento.AddCell(new Phrase(new Chunk(title, PdfMetodosAuxiliares.arial10Negrito)));

			if (hashList.Count == 0)
			{
				tabelaDocumento.AddCell(new Phrase(new Chunk("- Nenhum registro encontrado.", PdfMetodosAuxiliares.arial8)));
			}
			else
			{
				showCoordinates = totalAATIV > 0;
				tabelaDocumento.DefaultCell.PaddingBottom = 2;

				if (i == 0)
					tabelaLinha = new PdfPTable(new float[] { 8, 15, 10, 10, 6F, 12F, 18.5F, 18.5F, 6F, 6F, 6F, 6F, 10 });
				else
					tabelaLinha = new PdfPTable(new float[] { 8, 15, 8, 6, 7, 13, 18.5F, 18.5F, 7, 7, 7, 7, 10 });

				tabelaLinha.DefaultCell.PaddingLeft = 3;
				tabelaLinha.DefaultCell.PaddingRight = 3;
				tabelaLinha.DefaultCell.PaddingBottom = 3;
				tabelaLinha.DefaultCell.PaddingTop = 3;
				tabelaLinha.DefaultCell.BackgroundColor = corCinzaClaro;
				tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
				tabelaDocumento.HeaderRows = 2;

				tabelaLinha.DefaultCell.Rowspan = 2;
				tabelaLinha.DefaultCell.VerticalAlignment = Element.ALIGN_MIDDLE;
				tabelaLinha.AddCell(new Phrase(new Chunk("Código", PdfMetodosAuxiliares.arial8Negrito)));
				
				//coluna opcional

				tabelaLinha.DefaultCell.Colspan = 2;
				tabelaLinha.AddCell(new Phrase(new Chunk("Área (m²)", PdfMetodosAuxiliares.arial8Negrito)));
				
				tabelaLinha.DefaultCell.Colspan = 1;
				tabelaLinha.AddCell(new Phrase(new Chunk("Perímetro", PdfMetodosAuxiliares.arial8Negrito)));

				tabelaLinha.DefaultCell.Rowspan = 1;

				tabelaLinha.DefaultCell.Colspan = 6;
				tabelaLinha.AddCell(new Phrase(new Chunk(header, PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.DefaultCell.Colspan = 3;
				tabelaLinha.AddCell(new Phrase(new Chunk(header2, PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.DefaultCell.Colspan = 1;

				tabelaLinha.AddCell(new Phrase(new Chunk("Rocha", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.AddCell(new Phrase(new Chunk("Massa d'água", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.AddCell(new Phrase(new Chunk("AVN", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.AddCell(new Phrase(new Chunk("AA", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.AddCell(new Phrase(new Chunk("AFS", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.AddCell(new Phrase(new Chunk("RPPN", PdfMetodosAuxiliares.arial8Negrito)));

				tabelaLinha.AddCell(new Phrase(new Chunk("ARL", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.AddCell(new Phrase(new Chunk("APP", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.AddCell(new Phrase(new Chunk("Declividade", PdfMetodosAuxiliares.arial8Negrito)));

				tabelaLinha.DefaultCell.BackgroundColor = null;
				tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
							   				 
				foreach (Hashtable ht in hashList)
				{
					tabelaLinha.AddCell(new Phrase(new Chunk(FormatStringField(ht, "CODIGO"), PdfMetodosAuxiliares.arial8)));
					tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_RIGHT;
										
					tabelaLinha.DefaultCell.Colspan = 2;
					tabelaLinha.AddCell(new Phrase(new Chunk(FormatNumberField(ht, "AREA_M2"), PdfMetodosAuxiliares.arial8)));
					tabelaLinha.DefaultCell.Colspan = 1;

					tabelaLinha.AddCell(new Phrase(new Chunk(FormatNumberField(ht, "PERIMETRO",3), PdfMetodosAuxiliares.arial8)));

					tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
					tabelaLinha.AddCell(new Phrase(new Chunk(FormatSNField(ht, "ROCHA"), PdfMetodosAuxiliares.arial8)));
					tabelaLinha.AddCell(new Phrase(new Chunk(FormatSNField(ht, "MASSA_DAGUA"), PdfMetodosAuxiliares.arial8)));
					tabelaLinha.AddCell(new Phrase(new Chunk(FormatSNField(ht, "AVN"), PdfMetodosAuxiliares.arial8)));
					tabelaLinha.AddCell(new Phrase(new Chunk(FormatSNField(ht, "AA"), PdfMetodosAuxiliares.arial8)));
					tabelaLinha.AddCell(new Phrase(new Chunk(FormatSNField(ht, "AFS"), PdfMetodosAuxiliares.arial8)));
					tabelaLinha.AddCell(new Phrase(new Chunk(FormatSNField(ht, "RPPN"), PdfMetodosAuxiliares.arial8)));
					tabelaLinha.AddCell(new Phrase(new Chunk(FormatSNField(ht, "ARL"), PdfMetodosAuxiliares.arial8)));
					tabelaLinha.AddCell(new Phrase(new Chunk(FormatSNField(ht, "APP"), PdfMetodosAuxiliares.arial8)));
					tabelaLinha.AddCell(new Phrase(new Chunk(FormatSNField(ht, "REST_DECLIVIDADE"), PdfMetodosAuxiliares.arial8)));


					tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
				}

				tabelaDocumento.AddCell(tabelaLinha);
			}
			doc.SetPageSize(doc.PageSize.Rotate());
			doc.NewPage();
			doc.Add(tabelaDocumento);

			//------------------------------------------------------------------------------------


			if (showCoordinates)
			{
				//------------------------------------------------------------------------------------
				doc.SetPageSize(doc.PageSize.Rotate());
				doc.NewPage();

				tabelaDocumento = new PdfPTable(1);
				tabelaDocumento.WidthPercentage = 100;
				tabelaDocumento.SetWidths(new float[] { 100 });
				tabelaDocumento.SplitLate = false;
				tabelaDocumento.SplitRows = true;

				tabelaDocumento.DefaultCell.Border = 0;
				tabelaDocumento.DefaultCell.PaddingLeft = 0;
				tabelaDocumento.DefaultCell.PaddingRight = 0;
				tabelaDocumento.DefaultCell.PaddingBottom = 2;
				tabelaDocumento.DefaultCell.PaddingTop = 2;
				tabelaDocumento.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;


				//Coordenadas da ATP
				tabelaDocumento.AddCell("\n");
				tabelaDocumento.DefaultCell.PaddingBottom = 0;
				tabelaDocumento.AddCell(new Phrase(new Chunk("Lista de Coordenadas das Atividades (SIRGAS 2000 / UTM zone 24S)", PdfMetodosAuxiliares.arial10Negrito)));
				tabelaDocumento.AddCell("\n");

				doc.Add(tabelaDocumento);

				if ((hashData[ativ[i]] != null) && (hashData[ativ] != DBNull.Value))
					hashList = hashData[ativ] as List<Hashtable>;
				else
					hashList = new List<Hashtable>();

				foreach (Hashtable ht in hashList)
				{
					doc.Add(new Chunk("\n"));
					doc.Add(GerarListaDeCoordenadasDaAtividade(ht, ativ.Replace("QUADRO_", ""), corCinzaClaro));
				}				
			}
		}

		#endregion

		private PdfPTable GerarListaDeCoordenadasDaAtividade(Hashtable hashData, string tipo, BaseColor headerColor)
		{

			PdfPTable tabelaLinha = new PdfPTable(new float[] { 1, 1, 1, 1, 1, 1 });
			tabelaLinha.HeaderRows = 2;
			tabelaLinha.DefaultCell.PaddingLeft = 3;
			tabelaLinha.DefaultCell.PaddingRight = 3;
			tabelaLinha.DefaultCell.PaddingBottom = 3;
			tabelaLinha.DefaultCell.PaddingTop = 3;
			tabelaLinha.DefaultCell.BackgroundColor = headerColor;
			tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
			tabelaLinha.DefaultCell.VerticalAlignment = Element.ALIGN_MIDDLE;
			tabelaLinha.SpacingBefore = 15f;

			tabelaLinha.SplitRows = true;
			tabelaLinha.SplitLate = true;

			PdfPCell vazia = new PdfPCell();
			vazia.Border = 0;
			vazia.BorderWidth = 0;

			tabelaLinha.DefaultCell.Colspan = 5;
			tabelaLinha.AddCell(new Phrase(new Chunk(tipo + " de Código " + FormatStringField(hashData, "CODIGO"), PdfMetodosAuxiliares.arial8Negrito)));
			tabelaLinha.DefaultCell.Colspan = 1;
			tabelaLinha.AddCell(vazia);

			tabelaLinha.AddCell(new Phrase(new Chunk("Coordenada Nº", PdfMetodosAuxiliares.arial8Negrito)));
			tabelaLinha.AddCell(new Phrase(new Chunk("Norte", PdfMetodosAuxiliares.arial8Negrito)));
			tabelaLinha.AddCell(new Phrase(new Chunk("Este", PdfMetodosAuxiliares.arial8Negrito)));
			tabelaLinha.AddCell(new Phrase(new Chunk("Azimute", PdfMetodosAuxiliares.arial8Negrito)));
			tabelaLinha.AddCell(new Phrase(new Chunk("Distância (m)", PdfMetodosAuxiliares.arial8Negrito)));
			tabelaLinha.AddCell(vazia);

			tabelaLinha.DefaultCell.BackgroundColor = null;
			tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_RIGHT;

			List<Decimal> ordenadas = hashData["ORDENADAS"] as List<Decimal>;

			if (ordenadas.Count > 1)
			{
				Decimal lastX = Convert.ToDecimal(ordenadas[0]);
				Decimal lastY = Convert.ToDecimal(ordenadas[1]);

				tabelaLinha.AddCell(new Phrase(new Chunk('1', PdfMetodosAuxiliares.arial8)));
				tabelaLinha.AddCell(new Phrase(new Chunk(FormatNumber(lastY), PdfMetodosAuxiliares.arial8)));
				tabelaLinha.AddCell(new Phrase(new Chunk(FormatNumber(lastX), PdfMetodosAuxiliares.arial8)));

				tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
				tabelaLinha.AddCell("-");
				tabelaLinha.AddCell("-");
				tabelaLinha.AddCell(vazia);
				tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_RIGHT;

				Double azimute;
				Double grau;
				Double minuto;
				Double segundo;

				string gms;

				for (int k = 3; k <= ordenadas.Count; k += 2)
				{
					tabelaLinha.AddCell(new Phrase(new Chunk(((k + 1) / 2).ToString(), PdfMetodosAuxiliares.arial8)));
					tabelaLinha.AddCell(new Phrase(new Chunk(FormatNumber(ordenadas[k]), PdfMetodosAuxiliares.arial8)));
					tabelaLinha.AddCell(new Phrase(new Chunk(FormatNumber(ordenadas[k - 1]), PdfMetodosAuxiliares.arial8)));

					azimute = Decimal.ToDouble(CoordenadaBus.CalcularAzimute(lastX, lastY, ordenadas[k - 1], ordenadas[k]));
					if (azimute < 0)
					{
						gms = "-";
						azimute = -azimute;
					}
					else
					{
						gms = "";
					}

					grau = Math.Floor(azimute);
					azimute = (azimute - grau) * 60;
					minuto = Math.Floor(azimute);
					azimute = (azimute - minuto) * 60;
					segundo = azimute;

					gms += ((grau < 10) ? "0" : "") + grau.ToString() + "°";
					gms += ((minuto < 10) ? "0" : "") + minuto.ToString() + "'";
					gms += ((segundo < 10) ? "0" : "") + FormatNumber(segundo, 0) + "\"";

					tabelaLinha.AddCell(new Phrase(new Chunk(gms, PdfMetodosAuxiliares.arial8)));
					tabelaLinha.AddCell(new Phrase(new Chunk(FormatNumber(CoordenadaBus.CalcularDistancia(lastX, lastY, ordenadas[k - 1], ordenadas[k]), 3), PdfMetodosAuxiliares.arial8)));
					tabelaLinha.AddCell(vazia);

					lastX = Convert.ToDecimal(ordenadas[k - 1]);
					lastY = Convert.ToDecimal(ordenadas[k]);
				}

			}

			return tabelaLinha;
		}

		#endregion

		#region Fiscalizacao

		internal MemoryStream GerarPdfFiscal(MxdLayout mxd)
		{
			Document doc = new Document(mxd.MxdPageSize, 85, 40, 73, 50);

			MemoryStream ms = new MemoryStream();
			PdfWriter wrt = PdfWriter.GetInstance(doc, ms);
			//wrt.SetEncryption(PdfWriter.STRENGTH128BITS, null, null, PdfWriter.AllowCopy | PdfWriter.AllowPrinting);


			//Cabecalho e Rodape
			Hashtable htConfiguracoes = ObterDadosCabecalhoRodapePDF();

			PdfCabecalhoRodape headerFooter = new PdfCabecalhoRodape();

			headerFooter.GovernoNome = HasKey(htConfiguracoes, "GOVERNO_NOME") ? htConfiguracoes["GOVERNO_NOME"].ToString() : null;
			headerFooter.OrgaoCep = HasKey(htConfiguracoes, "ORGAO_CEP") ? htConfiguracoes["ORGAO_CEP"].ToString() : null;
			headerFooter.OrgaoContato = HasKey(htConfiguracoes, "ORGAO_CONTATO") ? htConfiguracoes["ORGAO_CONTATO"].ToString() : null;
			headerFooter.OrgaoEndereco = HasKey(htConfiguracoes, "ORGAO_ENDERECO") ? htConfiguracoes["ORGAO_ENDERECO"].ToString() : null;
			headerFooter.OrgaoMunicipio = HasKey(htConfiguracoes, "ORGAO_MUNICIPIO") ? htConfiguracoes["ORGAO_MUNICIPIO"].ToString() : null;
			headerFooter.OrgaoNome = HasKey(htConfiguracoes, "ORGAO_NOME") ? htConfiguracoes["ORGAO_NOME"].ToString() : null;
			headerFooter.OrgaoSigla = HasKey(htConfiguracoes, "ORGAO_SIGLA") ? htConfiguracoes["ORGAO_SIGLA"].ToString() : null;
			headerFooter.OrgaoUF = HasKey(htConfiguracoes, "ORGAO_UF") ? htConfiguracoes["ORGAO_UF"].ToString() : null;
			headerFooter.SecretariaNome = HasKey(htConfiguracoes, "SECRETARIA_NOME") ? htConfiguracoes["SECRETARIA_NOME"].ToString() : null;
			headerFooter.SetorNome = HasKey(htConfiguracoes, "SETOR_NOME") ? htConfiguracoes["SETOR_NOME"].ToString() : null;

			wrt.PageEvent = headerFooter;
			//------------------------

			doc.Open();

			Hashtable hashData = ObterDadosPDF();

			mxd.GerarPdf(doc, wrt, Project.Id, hashData);
			GerarVersoFiscal(doc, wrt, hashData);

			doc.Close();

			mxd.ApagarTempFile();

			return ms;
		}

		private void GerarVersoFiscal(Document doc, PdfWriter wrt, Hashtable hashData)
		{
			BaseColor corCinzaEscuro = new BaseColor(192, 192, 192);
			BaseColor corCinzaClaro = new BaseColor(220, 220, 220);

			Font arial8BoldBlue = PdfMetodosAuxiliares.GerarFonte(tipoFonte.Arial, 8, iTextSharp.text.Font.BOLD, BaseColor.BLUE);
			Font arial8BoldRed = PdfMetodosAuxiliares.GerarFonte(tipoFonte.Arial, 8, iTextSharp.text.Font.BOLD, BaseColor.RED);

			PdfPTable tabelaDocumento;

			List<Hashtable> hashList = null;

			tabelaDocumento = new PdfPTable(1);
			tabelaDocumento.WidthPercentage = 100;
			tabelaDocumento.SetWidths(new float[] { 100 });
			tabelaDocumento.SplitLate = true;
			tabelaDocumento.SplitRows = true;

			tabelaDocumento.DefaultCell.Border = 0;
			tabelaDocumento.DefaultCell.PaddingLeft = 0;
			tabelaDocumento.DefaultCell.PaddingRight = 0;
			tabelaDocumento.DefaultCell.PaddingBottom = 2;
			tabelaDocumento.DefaultCell.PaddingTop = 2;
			tabelaDocumento.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;

			string[] typeArray = new string[] { "QUADRO_PONTO", "QUADRO_LINHA", "QUADRO_AREA" };

			//Coordenadas da ATP
			tabelaDocumento.AddCell(" ");
			tabelaDocumento.DefaultCell.PaddingBottom = 0;
			tabelaDocumento.AddCell(new Phrase(new Chunk("Detalhamento dos locais fiscalizados (SIRGAS 2000 / UTM zone 24S)", PdfMetodosAuxiliares.arial10Negrito)));
			tabelaDocumento.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
			tabelaDocumento.SpacingAfter = 30f;

			doc.Add(tabelaDocumento);

			for (int i = 0; i < 3; i++)
			{
				if ((hashData[typeArray[i]] == null) || (hashData[typeArray[i]] == DBNull.Value || (hashData[typeArray[i]] as List<Hashtable>).Count == 0))
				{
					continue;
				}

				hashList = hashData[typeArray[i]] as List<Hashtable>;

				foreach (Hashtable ht in hashList)
				{
					doc.Add(new Chunk("\n"));
					doc.Add(new Chunk("\n"));

					doc.Add(GerarListaDeCoordenadasFiscalizacao(ht, typeArray[i], corCinzaEscuro, corCinzaClaro));
				}
			}
		}

		private PdfPTable GerarListaDeCoordenadasFiscalizacao(Hashtable hashData, string tipo, BaseColor darkHeaderColor, BaseColor lightHeaderColor)
		{
			PdfPTable tabelaLinha;
			PdfPTable tabelaAux;

			if (tipo == "QUADRO_PONTO")
			{
				tabelaLinha = new PdfPTable(new float[] { 4, 2, 2 });
				tabelaLinha.HeaderRows = 0;
				tabelaLinha.DefaultCell.PaddingLeft = 3;
				tabelaLinha.DefaultCell.PaddingRight = 3;
				tabelaLinha.DefaultCell.PaddingBottom = 3;
				tabelaLinha.DefaultCell.PaddingTop = 3;
				tabelaLinha.DefaultCell.BackgroundColor = darkHeaderColor;
				tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
				tabelaLinha.DefaultCell.VerticalAlignment = Element.ALIGN_MIDDLE;

				tabelaLinha.DefaultCell.Rowspan = 2;
				tabelaLinha.DefaultCell.BackgroundColor = darkHeaderColor;
				tabelaLinha.AddCell(new Phrase(new Chunk("Código: " + FormatStringField(hashData, "CODIGO"), PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.DefaultCell.Rowspan = 1;

				tabelaLinha.DefaultCell.BackgroundColor = lightHeaderColor;
				tabelaLinha.AddCell(new Phrase(new Chunk("Norte", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.AddCell(new Phrase(new Chunk("Este", PdfMetodosAuxiliares.arial8Negrito)));


				tabelaLinha.DefaultCell.BackgroundColor = null;
				tabelaLinha.AddCell(new Phrase(new Chunk(FormatNumber(((List<Decimal>)hashData["ORDENADAS"])[1]), PdfMetodosAuxiliares.arial8)));
				tabelaLinha.AddCell(new Phrase(new Chunk(FormatNumber(((List<Decimal>)hashData["ORDENADAS"])[0]), PdfMetodosAuxiliares.arial8)));
			}
			else
			{
				tabelaLinha = new PdfPTable(new float[] { 1, 1, 1, 1, 1 });
				tabelaLinha.HeaderRows = 2;
				tabelaLinha.DefaultCell.PaddingLeft = 3;
				tabelaLinha.DefaultCell.PaddingRight = 3;
				tabelaLinha.DefaultCell.PaddingBottom = 8;
				tabelaLinha.DefaultCell.PaddingTop = 8;
				tabelaLinha.DefaultCell.BackgroundColor = darkHeaderColor;
				tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
				tabelaLinha.DefaultCell.VerticalAlignment = Element.ALIGN_MIDDLE;

				tabelaLinha.SplitLate = true;
				tabelaLinha.SplitRows = false;

				tabelaLinha.DefaultCell.Colspan = 5;
				tabelaLinha.AddCell(new Phrase(new Chunk("Código: " + FormatStringField(hashData, "CODIGO"), PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.DefaultCell.Colspan = 1;

				tabelaLinha.DefaultCell.PaddingBottom = 5;
				tabelaLinha.DefaultCell.PaddingTop = 5;
				tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
				tabelaLinha.DefaultCell.BackgroundColor = lightHeaderColor;
				tabelaLinha.DefaultCell.Colspan = 5;
				if (tipo == "QUADRO_LINHA")
				{
					tabelaLinha.AddCell(new Phrase(new Chunk("Comprimento (m): " + FormatNumberField(hashData, "COMPRIMENTO", 3), PdfMetodosAuxiliares.arial8Negrito)));
				}
				else
				{
					tabelaAux = new PdfPTable(new float[] { 1, 1 });
					tabelaAux.HeaderRows = 0;
					tabelaAux.DefaultCell.BackgroundColor = lightHeaderColor;
					tabelaAux.DefaultCell.Border = Rectangle.NO_BORDER;
					tabelaAux.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
					tabelaAux.DefaultCell.VerticalAlignment = Element.ALIGN_MIDDLE;
					tabelaAux.DefaultCell.PaddingLeft = 0;
					tabelaAux.DefaultCell.PaddingRight = 0;
					tabelaAux.DefaultCell.PaddingBottom = 0;
					tabelaAux.DefaultCell.PaddingTop = 0;

					tabelaAux.AddCell(new Phrase(new Chunk("Área (m²): " + FormatNumberField(hashData, "AREA_M2", 2), PdfMetodosAuxiliares.arial8Negrito)));
					tabelaAux.AddCell(new Phrase(new Chunk("Área (ha): " + FormatNumber(Convert.ToDecimal(hashData["AREA_M2"]) / 10000, 4), PdfMetodosAuxiliares.arial8Negrito)));

					tabelaLinha.AddCell(tabelaAux);
				}

				tabelaLinha.DefaultCell.Colspan = 1;
				tabelaLinha.DefaultCell.BackgroundColor = null;
				tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
				tabelaLinha.DefaultCell.PaddingBottom = 3;
				tabelaLinha.DefaultCell.PaddingTop = 3;

				tabelaLinha.AddCell(new Phrase(new Chunk("Coordenada Nº", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.AddCell(new Phrase(new Chunk("Norte", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.AddCell(new Phrase(new Chunk("Este", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.AddCell(new Phrase(new Chunk("Azimute", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.AddCell(new Phrase(new Chunk("Distância (m)", PdfMetodosAuxiliares.arial8Negrito)));

				tabelaLinha.DefaultCell.BackgroundColor = null;
				tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_RIGHT;

				tabelaLinha.SplitRows = false;
				tabelaLinha.SplitLate = true;

				List<Decimal> ordenadas = hashData["ORDENADAS"] as List<Decimal>;

				if (ordenadas.Count > 1)
				{
					Decimal lastX = Convert.ToDecimal(ordenadas[0]);
					Decimal lastY = Convert.ToDecimal(ordenadas[1]);

					tabelaLinha.AddCell(new Phrase(new Chunk('1', PdfMetodosAuxiliares.arial8)));
					tabelaLinha.AddCell(new Phrase(new Chunk(FormatNumber(lastY), PdfMetodosAuxiliares.arial8)));
					tabelaLinha.AddCell(new Phrase(new Chunk(FormatNumber(lastX), PdfMetodosAuxiliares.arial8)));

					tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
					tabelaLinha.AddCell("-");
					tabelaLinha.AddCell("-");
					tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_RIGHT;

					Double azimute;
					Double grau;
					Double minuto;
					Double segundo;

					string gms;

					for (int k = 3; k <= ordenadas.Count; k += 2)
					{
						tabelaLinha.AddCell(new Phrase(new Chunk(((k + 1) / 2).ToString(), PdfMetodosAuxiliares.arial8)));
						tabelaLinha.AddCell(new Phrase(new Chunk(FormatNumber(ordenadas[k]), PdfMetodosAuxiliares.arial8)));
						tabelaLinha.AddCell(new Phrase(new Chunk(FormatNumber(ordenadas[k - 1]), PdfMetodosAuxiliares.arial8)));

						azimute = Decimal.ToDouble(CoordenadaBus.CalcularAzimute(lastX, lastY, ordenadas[k - 1], ordenadas[k]));
						if (azimute < 0)
						{
							gms = "-";
							azimute = -azimute;
						}
						else
						{
							gms = "";
						}

						grau = Math.Floor(azimute);
						azimute = (azimute - grau) * 60;
						minuto = Math.Floor(azimute);
						azimute = (azimute - minuto) * 60;
						segundo = azimute;

						gms += ((grau < 10) ? "0" : "") + grau.ToString() + "°";
						gms += ((minuto < 10) ? "0" : "") + minuto.ToString() + "'";
						gms += ((segundo < 10) ? "0" : "") + FormatNumber(segundo, 0) + "\"";

						tabelaLinha.AddCell(new Phrase(new Chunk(gms, PdfMetodosAuxiliares.arial8)));
						tabelaLinha.AddCell(new Phrase(new Chunk(FormatNumber(CoordenadaBus.CalcularDistancia(lastX, lastY, ordenadas[k - 1], ordenadas[k]), 3), PdfMetodosAuxiliares.arial8)));

						lastX = Convert.ToDecimal(ordenadas[k - 1]);
						lastY = Convert.ToDecimal(ordenadas[k]);
					}

				}

			}

			PdfPTable tabelaRet = new PdfPTable(new float[] { 11, 1 });
			tabelaRet.DefaultCell.Border = Rectangle.NO_BORDER;
			tabelaRet.DefaultCell.PaddingLeft = 0;
			tabelaRet.DefaultCell.PaddingRight = 0;
			tabelaRet.DefaultCell.PaddingBottom = 2;
			tabelaRet.DefaultCell.PaddingTop = 2;

			tabelaRet.SplitLate = false;
			tabelaRet.SplitRows = true;

			PdfPCell cellAux = new PdfPCell();
			cellAux.Border = Rectangle.NO_BORDER;
			cellAux.BackgroundColor = null;
			cellAux.AddElement(tabelaLinha);

			tabelaRet.AddCell(cellAux);
			tabelaRet.AddCell(" ");
			tabelaRet.SpacingAfter = 30f;

			return tabelaRet;
		}

		#endregion

		#region CAR

		internal MemoryStream GerarPdfCAR(MxdLayout mxd)
		{
			Document doc = new Document(mxd.MxdPageSize, 85, 40, 73, 50);

			MemoryStream ms = new MemoryStream();
			PdfWriter wrt = PdfWriter.GetInstance(doc, ms);
			//wrt.SetEncryption(PdfWriter.STRENGTH128BITS, null, null, PdfWriter.AllowCopy | PdfWriter.AllowPrinting);


			//Cabecalho e Rodape
			Hashtable htConfiguracoes = ObterDadosCabecalhoRodapePDF();

			PdfCabecalhoRodape headerFooter = new PdfCabecalhoRodape();

			headerFooter.GovernoNome = HasKey(htConfiguracoes, "GOVERNO_NOME") ? htConfiguracoes["GOVERNO_NOME"].ToString() : null;
			headerFooter.OrgaoCep = HasKey(htConfiguracoes, "ORGAO_CEP") ? htConfiguracoes["ORGAO_CEP"].ToString() : null;
			headerFooter.OrgaoContato = HasKey(htConfiguracoes, "ORGAO_CONTATO") ? htConfiguracoes["ORGAO_CONTATO"].ToString() : null;
			headerFooter.OrgaoEndereco = HasKey(htConfiguracoes, "ORGAO_ENDERECO") ? htConfiguracoes["ORGAO_ENDERECO"].ToString() : null;
			headerFooter.OrgaoMunicipio = HasKey(htConfiguracoes, "ORGAO_MUNICIPIO") ? htConfiguracoes["ORGAO_MUNICIPIO"].ToString() : null;
			headerFooter.OrgaoNome = HasKey(htConfiguracoes, "ORGAO_NOME") ? htConfiguracoes["ORGAO_NOME"].ToString() : null;
			headerFooter.OrgaoSigla = HasKey(htConfiguracoes, "ORGAO_SIGLA") ? htConfiguracoes["ORGAO_SIGLA"].ToString() : null;
			headerFooter.OrgaoUF = HasKey(htConfiguracoes, "ORGAO_UF") ? htConfiguracoes["ORGAO_UF"].ToString() : null;
			headerFooter.SecretariaNome = HasKey(htConfiguracoes, "SECRETARIA_NOME") ? htConfiguracoes["SECRETARIA_NOME"].ToString() : null;
			headerFooter.SetorNome = HasKey(htConfiguracoes, "SETOR_NOME") ? htConfiguracoes["SETOR_NOME"].ToString() : null;

			wrt.PageEvent = headerFooter;
			//------------------------

			doc.Open();

			Hashtable hashData = ObterDadosPDF();

			mxd.GerarPdf(doc, wrt, Project.Id, hashData);
			GerarVersoCAR(doc, wrt, hashData);

			doc.Close();

			mxd.ApagarTempFile();

			return ms;
		}

		private void GerarVersoCAR(Document doc, PdfWriter wrt, Hashtable hashData)
		{
			BaseColor corCinzaClaro = new BaseColor(220, 220, 220);

			PdfPTable tabelaDocumento;
			PdfPTable tabelaLinha;
			List<Hashtable> hashList = null;

			#region QUADROS DE ÁREAS

			if ((hashData["QUADRO_TOTAL"] != null) && (hashData["QUADRO_TOTAL"] != DBNull.Value))
				hashList = hashData["QUADRO_TOTAL"] as List<Hashtable>;
			else
				hashList = new List<Hashtable>();

			tabelaDocumento = new PdfPTable(1);
			tabelaDocumento.WidthPercentage = 100;
			tabelaDocumento.SetWidths(new float[] { 100 });
			tabelaDocumento.SplitLate = true;
			tabelaDocumento.SplitRows = true;

			tabelaDocumento.DefaultCell.Border = 0;
			tabelaDocumento.DefaultCell.PaddingLeft = 0;
			tabelaDocumento.DefaultCell.PaddingRight = 0;
			tabelaDocumento.DefaultCell.PaddingBottom = 2;
			tabelaDocumento.DefaultCell.PaddingTop = 2;
			tabelaDocumento.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
			tabelaDocumento.HeaderRows = 1;

			//Titulo
			tabelaDocumento.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
			tabelaDocumento.AddCell(new Phrase(new Chunk("QUADROS DE ÁREAS", PdfMetodosAuxiliares.arial16Negrito)));
			tabelaDocumento.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;

			tabelaDocumento.AddCell("\n");

			//Valores da CAR
			tabelaLinha = new PdfPTable(new float[] { 33, 33, 33 });
			tabelaLinha.DefaultCell.PaddingLeft = 3;
			tabelaLinha.DefaultCell.PaddingRight = 3;
			tabelaLinha.DefaultCell.PaddingBottom = 3;
			tabelaLinha.DefaultCell.PaddingTop = 3;

			Hashtable htCAR = null;
			if ((hashData["CAR"] != null) && (hashData["CAR"] != DBNull.Value))
				htCAR = hashData["CAR"] as Hashtable;
			else
				htCAR = new Hashtable();

			tabelaLinha.DefaultCell.BackgroundColor = corCinzaClaro;
			tabelaLinha.AddCell(new Phrase(new Chunk("Município do Módulo Fiscal", PdfMetodosAuxiliares.arial8Negrito)));

			tabelaLinha.AddCell(new Phrase(new Chunk("Qtd. Módulo Fiscal da ATP", PdfMetodosAuxiliares.arial8Negrito)));
			tabelaLinha.AddCell(new Phrase(new Chunk("ATP em 22/07/2008 (m²)", PdfMetodosAuxiliares.arial8Negrito)));
			tabelaLinha.DefaultCell.BackgroundColor = null;

			tabelaLinha.AddCell(new Phrase(new Chunk(FormatStringField(htCAR, "MUNICIPIO"), PdfMetodosAuxiliares.arial8)));
			tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_RIGHT;

			AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumberField(htCAR, "ATP_QTD_MODULO_FISCAL"));

			if (HasKey(htCAR, "ATP_DOCUMENTO_2008") && Convert.ToDecimal(htCAR["ATP_DOCUMENTO_2008"]) > 0)
			{
				AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumberField(htCAR, "ATP_DOCUMENTO_2008"));
			}
			else
			{
				AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumberField(hashData, "ATP_CROQUI_M2"));
			}

			tabelaDocumento.AddCell(tabelaLinha);

			List<Hashtable> matriculas = null;
			if ((hashData["QUADRO_DE_AREAS"] != null) && (hashData["QUADRO_DE_AREAS"] != DBNull.Value))
				matriculas = hashData["QUADRO_DE_AREAS"] as List<Hashtable>;
			else
				matriculas = new List<Hashtable>();

			if (matriculas.Count == 1)
			{
				//Quadro da Matricula
				tabelaDocumento.AddCell("\n");
				tabelaLinha = new PdfPTable(new float[] { 19, 52, 16, 13 });
				tabelaLinha.DefaultCell.PaddingLeft = 3;
				tabelaLinha.DefaultCell.PaddingRight = 3;
				tabelaLinha.DefaultCell.PaddingBottom = 3;
				tabelaLinha.DefaultCell.PaddingTop = 3;

				tabelaLinha.AddCell(new Phrase(new Chunk(FormatStringField(matriculas[0], "APMP_TIPO") + ':', PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.AddCell(new Phrase(new Chunk(FormatStringField(matriculas[0], "APMP_NOME"), PdfMetodosAuxiliares.arial8)));
				tabelaLinha.AddCell(new Phrase(new Chunk("Perímetro (m):", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_RIGHT;
				tabelaLinha.AddCell(new Phrase(new Chunk(FormatNumberField(matriculas[0], "APMP_PERIMETER", 3), PdfMetodosAuxiliares.arial8)));

				tabelaDocumento.AddCell(tabelaLinha);
			}

			//Quadro Total
			tabelaDocumento.AddCell("\n");
			tabelaDocumento.DefaultCell.PaddingBottom = 10;
			tabelaDocumento.AddCell(new Phrase(new Chunk("Área Total", PdfMetodosAuxiliares.arial10Negrito)));
			tabelaDocumento.DefaultCell.PaddingBottom = 2;

			tabelaLinha = new PdfPTable(new float[] { 19, 55, 13, 13 });
			tabelaLinha.DefaultCell.PaddingLeft = 3;
			tabelaLinha.DefaultCell.PaddingRight = 3;
			tabelaLinha.DefaultCell.PaddingBottom = 3;
			tabelaLinha.DefaultCell.PaddingTop = 3;
			tabelaLinha.DefaultCell.BackgroundColor = corCinzaClaro;
			tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;

			tabelaLinha.AddCell(new Phrase(new Chunk("Classe", PdfMetodosAuxiliares.arial8Negrito)));
			tabelaLinha.AddCell(new Phrase(new Chunk("Descrição", PdfMetodosAuxiliares.arial8Negrito)));
			tabelaLinha.AddCell(new Phrase(new Chunk("Área (m²)", PdfMetodosAuxiliares.arial8Negrito)));
			tabelaLinha.AddCell(new Phrase(new Chunk("Área (ha)", PdfMetodosAuxiliares.arial8Negrito)));

			tabelaLinha.DefaultCell.BackgroundColor = null;
			tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
			tabelaLinha.DefaultCell.VerticalAlignment = Element.ALIGN_MIDDLE;

			decimal areaTotal = 0;
			int count = hashList.Count;
			for (int i = 0; i < count; i++)
			{
				Hashtable ht = hashList[i];

				string descricao = "";
				int countClasse = 1;

				int otherClasse = i + 1;
				while ((otherClasse < count) && (hashList[otherClasse]["CLASSE"].ToString() == ht["CLASSE"].ToString()))
				{
					if (descricao != FormatStringField(hashList[otherClasse], "DESCRICAO"))
					{
						descricao = FormatStringField(hashList[otherClasse], "DESCRICAO");
						countClasse++;
					}

					otherClasse++;
				}

				tabelaLinha.DefaultCell.Rowspan = countClasse;
				tabelaLinha.AddCell(new Phrase(new Chunk(FormatStringField(ht, "CLASSE"), PdfMetodosAuxiliares.arial8)));
				tabelaLinha.DefaultCell.Rowspan = 1;
				tabelaLinha.AddCell(new Phrase(new Chunk(FormatStringField(ht, "DESCRICAO"), PdfMetodosAuxiliares.arial8)));

				AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumberField(ht, "AREA_M2"));
				AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumber(Convert.ToDecimal(ht["AREA_M2"]) / 10000, 4));

				descricao = "";

				List<Hashtable> subTipos = new List<Hashtable>();
				while (++i <= otherClasse)
				{
					if (i == otherClasse || descricao != FormatStringField(hashList[i], "DESCRICAO"))
					{
						if (descricao != "")
						{
							if (subTipos.Count != 1 || subTipos[0]["SUBTIPO"].ToString() != "")
							{
								descricao += "\n[";
								foreach (Hashtable hash in subTipos)
								{
									if (Convert.ToDecimal(hash["AREA_M2"]) > 0)
										descricao += Math.Round(100 * Convert.ToDecimal(hash["AREA_M2"]) / areaTotal) + "% " + hash["SUBTIPO"] + ", ";
								}
								descricao += "]";
								descricao = descricao.Replace(", ]", "]");
							}

							tabelaLinha.AddCell(new Phrase(new Chunk(descricao, PdfMetodosAuxiliares.arial8)));

							AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumber(areaTotal));
							AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumber(areaTotal / 10000, 4));
						}

						if (i == otherClasse)
							break;

						descricao = FormatStringField(hashList[i], "DESCRICAO");
						subTipos = new List<Hashtable>();
						areaTotal = 0;
					}

					areaTotal += Convert.ToDecimal(hashList[i]["AREA_M2"]);

					subTipos.Add(hashList[i]);
				}

				i--;
			}

			tabelaDocumento.AddCell(tabelaLinha);

			#endregion QUADROS DE ÁREAS

			//------------------------------------------------------------------------------------

			doc.NewPage();
			doc.Add(tabelaDocumento);

			if (matriculas.Count > 1)
			{
				//------------------------------------------------------------------------------------

				doc.SetPageSize(doc.PageSize.Rotate());
				doc.NewPage();
				hashList = matriculas;

				#region Áreas e Perímetros de Matrícula/Posse

				tabelaDocumento = new PdfPTable(1);
				tabelaDocumento.WidthPercentage = 100;
				tabelaDocumento.SetWidths(new float[] { 100 });
				tabelaDocumento.SplitLate = false;
				tabelaDocumento.SplitRows = true;

				tabelaDocumento.DefaultCell.Border = 0;
				tabelaDocumento.DefaultCell.PaddingLeft = 0;
				tabelaDocumento.DefaultCell.PaddingRight = 0;
				tabelaDocumento.DefaultCell.PaddingBottom = 2;
				tabelaDocumento.DefaultCell.PaddingTop = 2;
				tabelaDocumento.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
				tabelaDocumento.HeaderRows = 1;

				tabelaDocumento.DefaultCell.PaddingBottom = 10;
				tabelaDocumento.AddCell(new Phrase(new Chunk("Áreas e Perímetros de Matrícula/Posse", PdfMetodosAuxiliares.arial10Negrito)));
				tabelaDocumento.DefaultCell.PaddingBottom = 2;

				tabelaLinha = new PdfPTable(new float[] { 20, 20, 20, 20, 20 });
				tabelaLinha.DefaultCell.PaddingLeft = 3;
				tabelaLinha.DefaultCell.PaddingRight = 3;
				tabelaLinha.DefaultCell.PaddingBottom = 3;
				tabelaLinha.DefaultCell.PaddingTop = 3;
				tabelaLinha.DefaultCell.BackgroundColor = corCinzaClaro;
				tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;

				tabelaLinha.AddCell(new Phrase(new Chunk("Nome", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.AddCell(new Phrase(new Chunk("Tipo", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.AddCell(new Phrase(new Chunk("Área (m²)", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.AddCell(new Phrase(new Chunk("Área (ha)", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.AddCell(new Phrase(new Chunk("Perímetro (m)", PdfMetodosAuxiliares.arial8Negrito)));

				tabelaLinha.DefaultCell.BackgroundColor = null;
				tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;

				foreach (Hashtable ht in hashList)
				{
					tabelaLinha.AddCell(new Phrase(new Chunk(FormatStringField(ht, "APMP_NOME"), PdfMetodosAuxiliares.arial8)));
					tabelaLinha.AddCell(new Phrase(new Chunk(FormatStringField(ht, "APMP_TIPO"), PdfMetodosAuxiliares.arial8)));

					AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumberField(ht, "APMP_AREA"));
					AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumber(Convert.ToDecimal(ht["APMP_AREA"]) / 10000, 4));
					AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumberField(ht, "APMP_PERIMETER", 3));
				}

				tabelaDocumento.AddCell(tabelaLinha);
				tabelaDocumento.AddCell("\n");
				doc.Add(tabelaDocumento);

				//------------------------------------------------------------------------------------

				#endregion

				#region Áreas de Uso do Solo por Matrícula/Posse

				tabelaDocumento = new PdfPTable(1);
				tabelaDocumento.WidthPercentage = 100;
				tabelaDocumento.SetWidths(new float[] { 100 });
				tabelaDocumento.SplitLate = false;
				tabelaDocumento.SplitRows = true;

				tabelaDocumento.DefaultCell.Border = 0;
				tabelaDocumento.DefaultCell.PaddingLeft = 0;
				tabelaDocumento.DefaultCell.PaddingRight = 0;
				tabelaDocumento.DefaultCell.PaddingBottom = 2;
				tabelaDocumento.DefaultCell.PaddingTop = 2;
				tabelaDocumento.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
				tabelaDocumento.HeaderRows = 1;

				tabelaDocumento.DefaultCell.PaddingBottom = 10;
				tabelaDocumento.AddCell(new Phrase(new Chunk("Áreas de Uso do Solo por Matrícula/Posse", PdfMetodosAuxiliares.arial10Negrito)));
				tabelaDocumento.DefaultCell.PaddingBottom = 2;

				tabelaLinha = new PdfPTable(new float[] { 1, 1, 1, 1.2f, 1, 1, 1, 1, 1 });
				tabelaLinha.DefaultCell.PaddingLeft = 3;
				tabelaLinha.DefaultCell.PaddingRight = 3;
				tabelaLinha.DefaultCell.PaddingBottom = 3;
				tabelaLinha.DefaultCell.PaddingTop = 3;
				tabelaLinha.DefaultCell.BackgroundColor = corCinzaClaro;
				tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;

				tabelaLinha.AddCell(new Phrase(new Chunk("Matrícula/Posse", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.AddCell(new Phrase(new Chunk("AFS (m²)", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.AddCell(new Phrase(new Chunk("Rocha (m²)", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.AddCell(new Phrase(new Chunk("Massa d'água (m²)", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.AddCell(new Phrase(new Chunk("AVN (m²)", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.AddCell(new Phrase(new Chunk("AA (m²)", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.AddCell(new Phrase(new Chunk("RPPN (m²)", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.AddCell(new Phrase(new Chunk("ARL (m²)", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.AddCell(new Phrase(new Chunk("APP (m²)", PdfMetodosAuxiliares.arial8Negrito)));

				tabelaLinha.DefaultCell.BackgroundColor = null;
				tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;

				foreach (Hashtable ht in hashList)
				{
					tabelaLinha.AddCell(new Phrase(new Chunk(FormatStringField(ht, "APMP_NOME"), PdfMetodosAuxiliares.arial8)));

					AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumberField(ht, "AFS_AREA"));
					AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumberField(ht, "ROCHA_AREA"));
					AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumberField(ht, "MASSA_DAGUA_AREA"));
					AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumber(Convert.ToDecimal(ht["AVN_I_AREA"]) + Convert.ToDecimal(ht["AVN_M_AREA"]) + Convert.ToDecimal(ht["AVN_A_AREA"]) + Convert.ToDecimal(ht["AVN_D_AREA"])));
					AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumber(Convert.ToDecimal(ht["AA_REC_AREA"]) + Convert.ToDecimal(ht["AA_USO_AREA"]) + Convert.ToDecimal(ht["AA_D_AREA"])));
					AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumberField(ht, "RPPN_AREA"));
					AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumber(Convert.ToDecimal(ht["ARL_PRESERV_AREA"]) + Convert.ToDecimal(ht["ARL_REC_AREA"]) + Convert.ToDecimal(ht["ARL_USO_AREA"]) + Convert.ToDecimal(ht["ARL_D_AREA"])));
					AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumberField(ht, "APP_APMP_AREA"));
				}

				tabelaDocumento.AddCell(tabelaLinha);
				tabelaDocumento.AddCell("\n");
				try
				{
					doc.Add(tabelaDocumento);
				}
				catch
				{
					doc.NewPage();
					doc.Add(tabelaDocumento);
				}

				//------------------------------------------------------------------------------------

				#endregion

				#region Detalhamento da cobertura vegetal em AVN e em AA por Matrícula/Posse

				tabelaDocumento = new PdfPTable(1);
				tabelaDocumento.WidthPercentage = 100;
				tabelaDocumento.SetWidths(new float[] { 100 });
				tabelaDocumento.SplitLate = false;
				tabelaDocumento.SplitRows = true;

				tabelaDocumento.DefaultCell.Border = 0;
				tabelaDocumento.DefaultCell.PaddingLeft = 0;
				tabelaDocumento.DefaultCell.PaddingRight = 0;
				tabelaDocumento.DefaultCell.PaddingBottom = 2;
				tabelaDocumento.DefaultCell.PaddingTop = 2;
				tabelaDocumento.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
				tabelaDocumento.HeaderRows = 1;

				tabelaDocumento.DefaultCell.PaddingBottom = 10;
				tabelaDocumento.AddCell(new Phrase(new Chunk("Detalhamento da cobertura vegetal em AVN e em AA por Matrícula/Posse", PdfMetodosAuxiliares.arial10Negrito)));
				tabelaDocumento.DefaultCell.PaddingBottom = 2;

				tabelaLinha = new PdfPTable(new float[] { 1, 1, 1, 1, 1, 1, 1, 1 });
				tabelaLinha.DefaultCell.PaddingLeft = 3;
				tabelaLinha.DefaultCell.PaddingRight = 3;
				tabelaLinha.DefaultCell.PaddingBottom = 3;
				tabelaLinha.DefaultCell.PaddingTop = 3;
				tabelaLinha.DefaultCell.BackgroundColor = corCinzaClaro;
				tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
				tabelaLinha.DefaultCell.VerticalAlignment = Element.ALIGN_MIDDLE;

				tabelaLinha.DefaultCell.Rowspan = 2;
				tabelaLinha.AddCell(new Phrase(new Chunk("Matrícula/Posse", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.DefaultCell.Rowspan = 1;

				tabelaLinha.DefaultCell.Colspan = 4;
				tabelaLinha.AddCell(new Phrase(new Chunk("Estágio da AVN (m²)", PdfMetodosAuxiliares.arial8Negrito)));

				tabelaLinha.DefaultCell.Colspan = 3;
				tabelaLinha.AddCell(new Phrase(new Chunk("Uso da AA (m²)", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.DefaultCell.Colspan = 1;

				tabelaLinha.AddCell(new Phrase(new Chunk("Inicial", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.AddCell(new Phrase(new Chunk("Médio", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.AddCell(new Phrase(new Chunk("Avançado", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.AddCell(new Phrase(new Chunk("Não caracterizado", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.AddCell(new Phrase(new Chunk("Em Recuperação", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.AddCell(new Phrase(new Chunk("Em Uso", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.AddCell(new Phrase(new Chunk("Não caracterizada", PdfMetodosAuxiliares.arial8Negrito)));

				tabelaLinha.DefaultCell.BackgroundColor = null;
				tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;

				foreach (Hashtable ht in hashList)
				{
					tabelaLinha.AddCell(new Phrase(new Chunk(FormatStringField(ht, "APMP_NOME"), PdfMetodosAuxiliares.arial8)));

					AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumberField(ht, "AVN_I_AREA"));
					AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumberField(ht, "AVN_M_AREA"));
					AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumberField(ht, "AVN_A_AREA"));
					AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumberField(ht, "AVN_D_AREA"));
					AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumberField(ht, "AA_REC_AREA"));
					AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumberField(ht, "AA_USO_AREA"));
					AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumberField(ht, "AA_D_AREA"));
				}

				tabelaDocumento.AddCell(tabelaLinha);
				tabelaDocumento.AddCell("\n");
				try
				{
					doc.Add(tabelaDocumento);
				}
				catch
				{
					doc.NewPage();
					doc.Add(tabelaDocumento);
				}

				//------------------------------------------------------------------------------------

				#endregion

				#region Situação Ambiental por Matrícula/Posse

				tabelaDocumento = new PdfPTable(1);
				tabelaDocumento.WidthPercentage = 100;
				tabelaDocumento.SetWidths(new float[] { 100 });
				tabelaDocumento.SplitLate = false;
				tabelaDocumento.SplitRows = true;

				tabelaDocumento.DefaultCell.Border = 0;
				tabelaDocumento.DefaultCell.PaddingLeft = 0;
				tabelaDocumento.DefaultCell.PaddingRight = 0;
				tabelaDocumento.DefaultCell.PaddingBottom = 2;
				tabelaDocumento.DefaultCell.PaddingTop = 2;
				tabelaDocumento.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
				tabelaDocumento.HeaderRows = 1;

				tabelaDocumento.DefaultCell.PaddingBottom = 10;
				tabelaDocumento.AddCell(new Phrase(new Chunk("Situação Ambiental por Matrícula/Posse", PdfMetodosAuxiliares.arial10Negrito)));
				tabelaDocumento.DefaultCell.PaddingBottom = 2;

				tabelaLinha = new PdfPTable(new float[] { 1, 0.8F, 1, 0.8F, 1.2F, 0.8F, 1, 0.8F, 1.25F, 1.25F });
				tabelaLinha.DefaultCell.PaddingLeft = 3;
				tabelaLinha.DefaultCell.PaddingRight = 3;
				tabelaLinha.DefaultCell.PaddingBottom = 3;
				tabelaLinha.DefaultCell.PaddingTop = 3;
				tabelaLinha.DefaultCell.BackgroundColor = corCinzaClaro;
				tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
				tabelaLinha.DefaultCell.VerticalAlignment = Element.ALIGN_MIDDLE;

				tabelaLinha.DefaultCell.Rowspan = 2;
				tabelaLinha.AddCell(new Phrase(new Chunk("Matrícula/Posse", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.DefaultCell.Rowspan = 1;

				tabelaLinha.DefaultCell.Colspan = 4;
				tabelaLinha.AddCell(new Phrase(new Chunk("ARL (m²)", PdfMetodosAuxiliares.arial8Negrito)));

				tabelaLinha.DefaultCell.Colspan = 5;
				tabelaLinha.AddCell(new Phrase(new Chunk("APP (m²)", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.DefaultCell.Colspan = 1;

				//APMP ARL
				tabelaLinha.AddCell(new Phrase(new Chunk("Preservada", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.AddCell(new Phrase(new Chunk("Em Recuperação", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.AddCell(new Phrase(new Chunk("Em Uso", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.AddCell(new Phrase(new Chunk("Em Preservação Permanente", PdfMetodosAuxiliares.arial8Negrito)));

				//APMP APP
				tabelaLinha.AddCell(new Phrase(new Chunk("Preservada", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.AddCell(new Phrase(new Chunk("Em Recuperação", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.AddCell(new Phrase(new Chunk("Em Uso", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.AddCell(new Phrase(new Chunk("A Recuperar (Calculado)", PdfMetodosAuxiliares.arial8Negrito)));
				tabelaLinha.AddCell(new Phrase(new Chunk("Com Uso Consolidado", PdfMetodosAuxiliares.arial8Negrito)));

				tabelaLinha.DefaultCell.BackgroundColor = null;
				tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;

				foreach (Hashtable ht in hashList)
				{
					tabelaLinha.AddCell(new Phrase(new Chunk(FormatStringField(ht, "APMP_NOME"), PdfMetodosAuxiliares.arial8)));

					//APMP ARL
					AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumberField(ht, "ARL_PRESERV_AREA"));
					AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumberField(ht, "ARL_REC_AREA"));
					AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumberField(ht, "ARL_USO_AREA"));
					AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumberField(ht, "APP_ARL_AREA"));

					//APMP APP
					AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumberField(ht, "APP_AVN_AREA"));
					AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumberField(ht, "APP_AA_REC_AREA"));
					AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumberField(ht, "APP_AA_USO_AREA"));
					AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumberField(ht, "CAR_APP_AA_USO_AREA"));
					AdicionarNumeroNaTabela(tabelaLinha, PdfMetodosAuxiliares.arial8, FormatNumber(Convert.ToDecimal(ht["APP_AA_USO_AREA"]) - Convert.ToDecimal(ht["CAR_APP_AA_USO_AREA"])));
				}

				tabelaDocumento.AddCell(tabelaLinha);
				try
				{
					doc.Add(tabelaDocumento);
				}
				catch
				{
					doc.NewPage();
					doc.Add(tabelaDocumento);
				}

				//------------------------------------------------------------------------------------

				#endregion
			}

			PdfPCell vazia = new PdfPCell();
			vazia.Border = 0;
			vazia.BorderWidth = 0;

			//------------------------------------------------------------------------------------

			if (matriculas.Count > 1)
				doc.SetPageSize(doc.PageSize.Rotate());

			doc.NewPage();

			#region Lista de Coordenadas

			tabelaDocumento = new PdfPTable(1);
			tabelaDocumento.WidthPercentage = 100;
			tabelaDocumento.SetWidths(new float[] { 100 });
			tabelaDocumento.SplitLate = false;
			tabelaDocumento.SplitRows = true;

			tabelaDocumento.DefaultCell.Border = 0;
			tabelaDocumento.DefaultCell.PaddingLeft = 0;
			tabelaDocumento.DefaultCell.PaddingRight = 0;
			tabelaDocumento.DefaultCell.PaddingBottom = 2;
			tabelaDocumento.DefaultCell.PaddingTop = 2;
			tabelaDocumento.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
			//tabelaDocumento.HeaderRows = 1;

			//Coordenadas da ATP
			tabelaDocumento.AddCell("\n");
			tabelaDocumento.DefaultCell.PaddingBottom = 10;
			tabelaDocumento.AddCell(new Phrase(new Chunk("Lista de Coordenadas da ATP (SIRGAS 2000 / UTM zone 24S)", PdfMetodosAuxiliares.arial10Negrito)));
			tabelaDocumento.DefaultCell.PaddingBottom = 2;

			doc.Add(tabelaDocumento);

			tabelaLinha = new PdfPTable(new float[] { 1, 1, 1, 1, 1, 1 });
			tabelaLinha.HeaderRows = 1;
			tabelaLinha.DefaultCell.PaddingLeft = 3;
			tabelaLinha.DefaultCell.PaddingRight = 3;
			tabelaLinha.DefaultCell.PaddingBottom = 3;
			tabelaLinha.DefaultCell.PaddingTop = 3;
			tabelaLinha.DefaultCell.BackgroundColor = corCinzaClaro;
			tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
			tabelaLinha.DefaultCell.VerticalAlignment = Element.ALIGN_MIDDLE;
			tabelaLinha.SpacingBefore = 15f;

			tabelaLinha.AddCell(new Phrase(new Chunk("Coordenada Nº", PdfMetodosAuxiliares.arial8Negrito)));
			tabelaLinha.AddCell(new Phrase(new Chunk("Norte", PdfMetodosAuxiliares.arial8Negrito)));
			tabelaLinha.AddCell(new Phrase(new Chunk("Este", PdfMetodosAuxiliares.arial8Negrito)));
			tabelaLinha.AddCell(new Phrase(new Chunk("Azimute", PdfMetodosAuxiliares.arial8Negrito)));
			tabelaLinha.AddCell(new Phrase(new Chunk("Distância (m)", PdfMetodosAuxiliares.arial8Negrito)));
			tabelaLinha.AddCell(vazia);

			tabelaLinha.DefaultCell.BackgroundColor = null;
			tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_RIGHT;

			List<Decimal> ordenadas = hashData["ORDENADAS_ATP"] as List<Decimal>;

			if (ordenadas.Count > 1)
			{
				Decimal lastX = Convert.ToDecimal(ordenadas[0]);
				Decimal lastY = Convert.ToDecimal(ordenadas[1]);

				tabelaLinha.AddCell(new Phrase(new Chunk('1', PdfMetodosAuxiliares.arial8)));
				tabelaLinha.AddCell(new Phrase(new Chunk(FormatNumber(lastY), PdfMetodosAuxiliares.arial8)));
				tabelaLinha.AddCell(new Phrase(new Chunk(FormatNumber(lastX), PdfMetodosAuxiliares.arial8)));

				tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
				tabelaLinha.AddCell("-");
				tabelaLinha.AddCell("-");
				tabelaLinha.AddCell(vazia);
				tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_RIGHT;

				Double azimute;
				Double grau;
				Double minuto;
				Double segundo;

				string gms;

				for (int k = 3; k <= ordenadas.Count; k += 2)
				{
					tabelaLinha.AddCell(new Phrase(new Chunk(((k + 1) / 2).ToString(), PdfMetodosAuxiliares.arial8)));
					tabelaLinha.AddCell(new Phrase(new Chunk(FormatNumber(ordenadas[k]), PdfMetodosAuxiliares.arial8)));
					tabelaLinha.AddCell(new Phrase(new Chunk(FormatNumber(ordenadas[k - 1]), PdfMetodosAuxiliares.arial8)));

					azimute = Decimal.ToDouble(CoordenadaBus.CalcularAzimute(lastX, lastY, ordenadas[k - 1], ordenadas[k]));
					if (azimute < 0)
					{
						gms = "-";
						azimute = -azimute;
					}
					else
					{
						gms = "";
					}

					grau = Math.Floor(azimute);
					azimute = (azimute - grau) * 60;
					minuto = Math.Floor(azimute);
					azimute = (azimute - minuto) * 60;
					segundo = azimute;

					gms += ((grau < 10) ? "0" : "") + grau.ToString() + "°";
					gms += ((minuto < 10) ? "0" : "") + minuto.ToString() + "'";
					gms += ((segundo < 10) ? "0" : "") + FormatNumber(segundo, 0) + "\"";

					tabelaLinha.AddCell(new Phrase(new Chunk(gms, PdfMetodosAuxiliares.arial8)));
					tabelaLinha.AddCell(new Phrase(new Chunk(FormatNumber(CoordenadaBus.CalcularDistancia(lastX, lastY, ordenadas[k - 1], ordenadas[k]), 3), PdfMetodosAuxiliares.arial8)));
					tabelaLinha.AddCell(vazia);

					lastX = Convert.ToDecimal(ordenadas[k - 1]);
					lastY = Convert.ToDecimal(ordenadas[k]);
				}

			}

			doc.Add(tabelaLinha);

			tabelaDocumento = new PdfPTable(1);
			tabelaDocumento.WidthPercentage = 100;
			tabelaDocumento.SetWidths(new float[] { 100 });
			tabelaDocumento.SplitLate = false;
			tabelaDocumento.SplitRows = true;

			tabelaDocumento.DefaultCell.Border = 0;
			tabelaDocumento.DefaultCell.PaddingLeft = 0;
			tabelaDocumento.DefaultCell.PaddingRight = 0;
			tabelaDocumento.DefaultCell.PaddingBottom = 2;
			tabelaDocumento.DefaultCell.PaddingTop = 2;
			tabelaDocumento.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
			//Coordenadas da ARL
			tabelaDocumento.AddCell("\n");
			tabelaDocumento.DefaultCell.PaddingBottom = 10;
			tabelaDocumento.AddCell(new Phrase(new Chunk("Lista de Coordenadas da ARL (SIRGAS 2000 / UTM zone 24S)", PdfMetodosAuxiliares.arial10Negrito)));
			tabelaDocumento.DefaultCell.PaddingBottom = 2;

			doc.Add(tabelaDocumento);


			tabelaLinha = new PdfPTable(new float[] { 0.9F, 0.9F, 1.2F, 1, 1, 1, 1 });
			tabelaLinha.HeaderRows = 1;
			tabelaLinha.DefaultCell.PaddingLeft = 3;
			tabelaLinha.DefaultCell.PaddingRight = 3;
			tabelaLinha.DefaultCell.PaddingBottom = 3;
			tabelaLinha.DefaultCell.PaddingTop = 3;
			tabelaLinha.DefaultCell.BackgroundColor = corCinzaClaro;
			tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
			tabelaLinha.DefaultCell.VerticalAlignment = Element.ALIGN_MIDDLE;
			tabelaLinha.SpacingBefore = 15f;

			tabelaLinha.AddCell(new Phrase(new Chunk("ARL", PdfMetodosAuxiliares.arial8Negrito)));
			tabelaLinha.AddCell(new Phrase(new Chunk("Coordenada Nº", PdfMetodosAuxiliares.arial8Negrito)));
			tabelaLinha.AddCell(new Phrase(new Chunk("Norte", PdfMetodosAuxiliares.arial8Negrito)));
			tabelaLinha.AddCell(new Phrase(new Chunk("Este", PdfMetodosAuxiliares.arial8Negrito)));
			tabelaLinha.AddCell(new Phrase(new Chunk("Azimute", PdfMetodosAuxiliares.arial8Negrito)));
			tabelaLinha.AddCell(new Phrase(new Chunk("Distância (m)", PdfMetodosAuxiliares.arial8Negrito)));
			tabelaLinha.AddCell(vazia);

			tabelaLinha.DefaultCell.BackgroundColor = null;
			tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_RIGHT;

			Dictionary<String, List<Decimal>> dicOrdArls = hashData["ORDENADAS_ARL"] as Dictionary<String, List<Decimal>>;

			foreach (var arlkey in dicOrdArls.Keys)
			{
				List<Decimal> lstOrdenadasArls = dicOrdArls[arlkey];

				if (lstOrdenadasArls != null && lstOrdenadasArls.Count > 1)
				{
					Decimal lastX = Convert.ToDecimal(lstOrdenadasArls[0]);
					Decimal lastY = Convert.ToDecimal(lstOrdenadasArls[1]);

					tabelaLinha.AddCell(new Phrase(new Chunk(arlkey, PdfMetodosAuxiliares.arial8)));
					tabelaLinha.AddCell(new Phrase(new Chunk('1', PdfMetodosAuxiliares.arial8)));
					tabelaLinha.AddCell(new Phrase(new Chunk(FormatNumber(lastY), PdfMetodosAuxiliares.arial8)));
					tabelaLinha.AddCell(new Phrase(new Chunk(FormatNumber(lastX), PdfMetodosAuxiliares.arial8)));

					tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
					tabelaLinha.AddCell("-");
					tabelaLinha.AddCell("-");
					tabelaLinha.AddCell(vazia);
					tabelaLinha.DefaultCell.HorizontalAlignment = Element.ALIGN_RIGHT;

					Double azimute;
					Double grau;
					Double minuto;
					Double segundo;

					string gms;

					for (int k = 3; k <= lstOrdenadasArls.Count; k += 2)
					{
						tabelaLinha.AddCell(new Phrase(new Chunk(arlkey, PdfMetodosAuxiliares.arial8)));
						tabelaLinha.AddCell(new Phrase(new Chunk(((k + 1) / 2).ToString(), PdfMetodosAuxiliares.arial8)));
						tabelaLinha.AddCell(new Phrase(new Chunk(FormatNumber(lstOrdenadasArls[k]), PdfMetodosAuxiliares.arial8)));
						tabelaLinha.AddCell(new Phrase(new Chunk(FormatNumber(lstOrdenadasArls[k - 1]), PdfMetodosAuxiliares.arial8)));

						azimute = Decimal.ToDouble(CoordenadaBus.CalcularAzimute(lastX, lastY, lstOrdenadasArls[k - 1], lstOrdenadasArls[k]));
						if (azimute < 0)
						{
							gms = "-";
							azimute = -azimute;
						}
						else
						{
							gms = "";
						}

						grau = Math.Floor(azimute);
						azimute = (azimute - grau) * 60;
						minuto = Math.Floor(azimute);
						azimute = (azimute - minuto) * 60;
						segundo = azimute;

						gms += ((grau < 10) ? "0" : "") + grau.ToString() + "°";
						gms += ((minuto < 10) ? "0" : "") + minuto.ToString() + "'";
						gms += ((segundo < 10) ? "0" : "") + FormatNumber(segundo, 0) + "\"";

						tabelaLinha.AddCell(new Phrase(new Chunk(gms, PdfMetodosAuxiliares.arial8)));
						tabelaLinha.AddCell(new Phrase(new Chunk(FormatNumber(CoordenadaBus.CalcularDistancia(lastX, lastY, lstOrdenadasArls[k - 1], lstOrdenadasArls[k]), 3), PdfMetodosAuxiliares.arial8)));
						tabelaLinha.AddCell(vazia);

						lastX = Convert.ToDecimal(lstOrdenadasArls[k - 1]);
						lastY = Convert.ToDecimal(lstOrdenadasArls[k]);
					}
				}
			}

			doc.Add(tabelaLinha);

			#endregion Lista de Coordenadas
		}

		#endregion

		#region Auxiliares

		private Boolean HasKey(Hashtable ht, string key)
		{
			return ((ht != null) && ht.ContainsKey(key) && ht[key] != null && !(ht[key] is DBNull));
		}

		private string FormatStringField(Hashtable ht, string key)
		{
			return HasKey(ht, key) ? ht[key].ToString() : string.Empty;
		}

		private string FormatNumberField(Hashtable ht, string key, int precision = 2)
		{
			return HasKey(ht, key) ? FormatNumber(ht[key], precision) : string.Empty;
		}

		private string FormatNumber(object value, int precision = 2)
		{
			return (value != null) ? Convert.ToDecimal(value).ToString("N" + precision) : "";
		}

		private string FormatSNField(Hashtable ht, string key)
		{
			string value = HasKey(ht, key) ? ht[key].ToString() : string.Empty;

			return (value == "S") ? "Sim" : (value == "N") ? "-" : value.Replace(" [", "\n[");
		}

		private void AdicionarNumeroNaTabela(PdfPTable tabela, Font fonte, string valor)
		{
			int align = tabela.DefaultCell.HorizontalAlignment;

			if ("0,0000000".IndexOf(valor) < 0)
			{
				tabela.DefaultCell.HorizontalAlignment = Element.ALIGN_RIGHT;
			}
			else
			{
				tabela.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
				valor = "-";
			}

			tabela.AddCell(new Phrase(new Chunk(valor, fonte)));
			tabela.DefaultCell.HorizontalAlignment = align;
		}

		#endregion
	}
}
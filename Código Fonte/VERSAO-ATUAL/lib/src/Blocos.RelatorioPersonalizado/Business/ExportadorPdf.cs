using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using Aspose.Words;
using Aspose.Words.Tables;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Entities;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx;
using Tecnomapas.Blocos.RelatorioPersonalizado.Entities;
using Font = Aspose.Words.Font;

namespace Tecnomapas.Blocos.RelatorioPersonalizado.Business
{
	public class ExportadorPdf : IExportador
	{
		Document Documento { get; set; }
		DocumentBuilder Builder { get; set; }
		Dictionary<string, Estilo> Estilos { get; set; }
		Double Largura { get; set; }

		public ExportadorPdf()
		{
			Documento = new Document(HttpContext.Current.Server.MapPath(@"~/Content/_pdfAspose/RelatorioPaisagem.doc"));
			Builder = new DocumentBuilder(Documento);
			Estilos = new Dictionary<string, Estilo>();
		}

		public byte[] Exportar(DadosRelatorio dados)
		{
			byte[] bytes;

			using (MemoryStream ms = new MemoryStream())
			{
				Exportar(dados, ms);
				bytes = ms.ToArray();
			}

			return bytes;
		}

		public void Exportar(DadosRelatorio dados, out Arquivo.Arquivo arquivo)
		{
			arquivo = new Arquivo.Arquivo();
			MemoryStream ms = new MemoryStream();
			Exportar(dados, ms);
			arquivo.Buffer = ms;
			arquivo.ContentType = "application/pdf";
			arquivo.Nome = "relatorio.pdf";
		}

		public void Exportar(DadosRelatorio dados, Stream stream)
		{
			Documento = new Document(dados.ConfiguracaoDocumentoPDF.CaminhoDocumento);
			Builder = new DocumentBuilder(Documento);

			Dictionary<int, string> colunas = new Dictionary<int, string>();

			#region Tabelas e Estilos

			Table tabela = FindTable(Documento, "Filtro1");

			if (tabela != null)
			{
				ConfigurarEstilo(tabela);
				tabela.Rows.Clear();
			}

			tabela = FindTable(Documento, "Linha1Coluna1");

			if (tabela != null)
			{
				ConfigurarEstilo(tabela);
				tabela.Rows.Clear();
			}

			#endregion Tabelas e Estilos

			Builder.MoveToDocumentEnd();

			Builder.StartTable();

			#region Filtros

			if (dados.Filtros != null && dados.Filtros.Count > 0)
			{
				int cont = 2;
				foreach (var item in dados.Filtros)
				{
					if (!string.IsNullOrEmpty(item.Campo.Alias))
					{
						string filtro = item.Campo.Alias + " " + dados.Operadores.Single(x => Convert.ToInt32(x.Id) == item.Operador).Texto + " ";

						if (item.Campo.PossuiListaDeValores)
						{
							if (item.Campo.TipoDadosEnum == eTipoDados.Bitand)
							{
								int valor = Convert.ToInt32(item.Valor);

								foreach (var itemLista in item.Campo.Lista)
								{
									if ((valor & Convert.ToInt32(itemLista.Codigo)) > 0)
									{
										filtro += itemLista.Texto + '/';
									}
								}

								filtro = filtro.Substring(0, filtro.Length - 1);
							}
							else
							{
								filtro += (item.Campo.Lista.FirstOrDefault(x => x.Id == item.Valor) ?? new Lista()).Texto;
							}
						}
						else
						{
							filtro += item.Valor;
						}

						if ((cont % 2) == 0)
						{
							InserirCelula(filtro, 50, Estilos["Estilo_0_0"]);
						}
						else
						{
							InserirCelula(filtro, 50, Estilos["Estilo_0_1"]);
							Builder.EndRow();
						}

						cont++;
					}
				}

				if ((cont % 2) != 0)
				{
					Builder.EndRow();
				}
			}
			else
			{
				InserirCelula("Nenhum Filtro Adicionado", 100, Estilos["Estilo_0_0"]);
				Builder.EndRow();
			}

			EscreverLinhaEmBranco();

			#endregion Filtros

			#region Escrever Dados

			if (dados.ComAgrupamento)
			{
				if (dados.Grupos.Count == 0)
				{
					InserirLinha(dados.Campos);
					EscreverDados();
				}
				else
				{
					foreach (var grupo in dados.Grupos)
					{
						EscreverLinhaEmBranco(16);
						EscreverTituloGrupo(grupo.Campo, grupo.Valor);
						EscreverLinhaEmBranco();
						InserirLinha(dados.Campos, Estilos["Estilo_2_0"]);
						InserirLinha(grupo.Dados, dados.Campos, Estilos["Estilo_3_0"], Estilos["Estilo_4_0"]);

						if (grupo.Sumarizacoes.Linhas.Count > 0 || dados.Totalizar)
						{
							EscreverLinhaEmBranco();
						}

						if (grupo.Sumarizacoes.Linhas.Count > 0)
						{
							colunas = grupo.Sumarizacoes.Colunas;
							EscreverDadosSumario(grupo.Sumarizacoes, colunas);
						}

						if (dados.Totalizar)
						{
							EscreverTotal(grupo.Total.ToString(), Estilos["Estilo_12_0"], Estilos["Estilo_12_1"]);
						}
					}

					if (dados.Sumarizacoes.Linhas.Count > 0)
					{
						EscreverDadosSumarioTotal(dados.Sumarizacoes, colunas);
					}

					if (dados.Totalizar)
					{
						EscreverTotal(dados.Total.ToString());
					}
				}
			}
			else
			{
				if (dados.Dados.Linhas.Count == 0)
				{
					InserirLinha(dados.Campos, Estilos["Estilo_3_0"]);
				}
				else
				{
					InserirLinha(dados.Campos, Estilos["Estilo_2_0"]);
					InserirLinha(dados.Dados, dados.Campos, Estilos["Estilo_3_0"], Estilos["Estilo_4_0"]);

					if (dados.Sumarizacoes.Linhas.Count > 0 || dados.Totalizar)
					{
						EscreverLinhaEmBranco();
					}

					if (dados.Sumarizacoes.Linhas.Count > 0)
					{
						EscreverDadosSumarioTotal(dados.Sumarizacoes, dados.Sumarizacoes.Colunas);
					}

					if (dados.Totalizar)
					{
						EscreverTotal(dados.Total.ToString());
					}
				}
			}

			#endregion Escrever Dados

			Builder.EndTable();
			dados.ConfiguracaoDocumentoPDF.RelatorioNome = dados.Nome;
			dados.ConfiguracaoDocumentoPDF.RelatorioDataHoraImpressao = DateTime.Now.ToString();
			dados.ConfiguracaoDocumentoPDF.RelatorioDataHoraAtualizacao = String.IsNullOrEmpty(dados.Data) ? DateTime.Now.ToString() : Convert.ToDateTime(dados.Data).ToString();
			ConfigurarCabecalhoRodape(dados.ConfiguracaoDocumentoPDF);

			//Salvar o arquivo
			Builder.Document.Save(stream, SaveFormat.Pdf);
		}

		#region Aspose

		public void ConfigurarCabecalhoRodape(ConfiguracaoDocumentoPDF configuracaoDocumento)
		{
			Documento.MailMerge.FieldMergingCallback = new HandleField();

			ObjectMailMerge objDataSource = new ObjectMailMerge(configuracaoDocumento, eMailMergeMode.keepTag);
			Builder.Document.MailMerge.Execute(objDataSource);

			ObjectMailMerge objDataSourceCabecalhoRodape = new ObjectMailMerge(configuracaoDocumento.CabecalhoRodape);
			Builder.Document.MailMerge.Execute(objDataSourceCabecalhoRodape);
		}

		public static bool FindTexto(Document doc, string textoContido)
		{
			Node[] nodes = doc.GetChildNodes(NodeType.Table, true).ToArray();

			Node retorno = nodes.LastOrDefault(x =>
			{
				Table item = x as Table;
				string valor = item.ToTxt();

				if (String.IsNullOrEmpty(valor))
				{
					return false;
				}

				valor = valor.Replace("\n", "").Replace("\r", "");

				if (valor.IndexOf(textoContido, StringComparison.InvariantCultureIgnoreCase) < 0)
				{
					return false;
				}

				return true;
			});

			return false;
		}

		public static Table FindTable(Document doc, string textoContido)
		{
			Node[] nodes = doc.GetChildNodes(NodeType.Table, true).ToArray();

			Node retorno = nodes.LastOrDefault(x =>
			{
				Table item = x as Table;
                string valor = item.ToTxt();

				if (String.IsNullOrEmpty(valor))
				{
					return false;
				}

				valor = valor.Replace("\n", "").Replace("\r", "");

				if (valor.IndexOf(textoContido, StringComparison.InvariantCultureIgnoreCase) < 0)
				{
					return false;
				}

				return true;
			});

			return retorno as Table;
		}

		private void CriarEstilo(Row linha, int indice, int celula)
		{
			Style para = Documento.Styles.Add(StyleType.Paragraph, "Estilo_" + indice.ToString() + "_" + celula.ToString());

			Estilos.Add("Estilo_" + indice.ToString() + "_" + celula.ToString(), new Estilo("Estilo_" + indice.ToString() + "_" + celula.ToString(), linha.Cells[celula].FirstParagraph.Runs[0].Font,
            para, linha.Cells[celula].FirstParagraph.ParagraphFormat, linha.RowFormat, linha.Cells[celula].CellFormat, linha.Cells[celula].FirstParagraph.ToTxt()));
		}

		private void ConfigurarEstilo(Table tabela)
		{
			int ultimaLinha = 0;

			if (Estilos.Count > 0)
			{
				ultimaLinha = Convert.ToInt32(Estilos.Keys.Last().Split('_').GetValue(1)) + 1;
			}

			if (tabela != null)
			{
				for (int i = 0; i < tabela.Rows.Count; i++)
				{
					CriarEstilo(tabela.Rows[i], i + ultimaLinha, 0);
					if (tabela.Rows[i].Cells.Count > 1)
					{
						CriarEstilo(tabela.Rows[i], i + ultimaLinha, 1);
					}
				}
			}

			Largura = (Builder.PageSetup.PageWidth - Builder.PageSetup.LeftMargin - Builder.PageSetup.RightMargin);
		}

		private void InserirCelula(string campo, double largura, Estilo estilo)
		{
			Builder.InsertCell();
			Builder.ParagraphFormat.Alignment = estilo.Paragrafo.Alignment;
			Builder.ParagraphFormat.LineSpacing = estilo.Paragrafo.LineSpacing;
			Builder.ParagraphFormat.LineSpacingRule = estilo.Paragrafo.LineSpacingRule;
			Builder.ParagraphFormat.OutlineLevel = estilo.Paragrafo.OutlineLevel;
			Builder.ParagraphFormat.SuppressAutoHyphens = estilo.Paragrafo.SuppressAutoHyphens;
			//
			Builder.ParagraphFormat.LeftIndent = estilo.Paragrafo.LeftIndent;
			Builder.ParagraphFormat.RightIndent = estilo.Paragrafo.RightIndent;
			Builder.ParagraphFormat.SpaceAfter = estilo.Paragrafo.SpaceAfter;
			Builder.ParagraphFormat.SpaceBefore = estilo.Paragrafo.SpaceBefore;
			Builder.ParagraphFormat.Style = estilo.ParagrafoEstilo;

			//
			Builder.RowFormat.HeadingFormat = estilo.Linha.HeadingFormat;
			Builder.CellFormat.Width = (Largura / 100) * largura;
			Builder.CellFormat.Shading.BackgroundPatternColor = estilo.Celula.Shading.BackgroundPatternColor;

			try
			{
				Builder.CellFormat.Borders.Left.LineStyle = estilo.Celula.Borders.Left.LineStyle;
				Builder.CellFormat.Borders.Left.LineWidth = estilo.Celula.Borders.Left.LineWidth;
				Builder.CellFormat.Borders.Left.Color = estilo.Celula.Borders.Left.Color;
				//
				Builder.CellFormat.Borders.Right.LineStyle = estilo.Celula.Borders.Right.LineStyle;
				Builder.CellFormat.Borders.Right.LineWidth = estilo.Celula.Borders.Right.LineWidth;
				Builder.CellFormat.Borders.Right.Color = estilo.Celula.Borders.Right.Color;
				//
				Builder.CellFormat.Borders.Bottom.LineStyle = estilo.Celula.Borders.Bottom.LineStyle;
				Builder.CellFormat.Borders.Bottom.LineWidth = estilo.Celula.Borders.Bottom.LineWidth;
				Builder.CellFormat.Borders.Bottom.Color = estilo.Celula.Borders.Bottom.Color;
				//
				Builder.CellFormat.Borders.Top.LineStyle = estilo.Celula.Borders.Top.LineStyle;
				Builder.CellFormat.Borders.Top.LineWidth = estilo.Celula.Borders.Top.LineWidth;
				Builder.CellFormat.Borders.Top.Color = estilo.Celula.Borders.Top.Color;
			}
			catch
			{
				Builder.CellFormat.Borders.Color = Color.Empty;
				Builder.CellFormat.Borders.LineStyle = LineStyle.None;
				Builder.CellFormat.Borders.LineWidth = 0;
			}

			Paragraph para = Builder.InsertParagraph();

			para.Runs.Add(new Run(Documento, campo));
		}

		private void InserirCelula(List<ValoresBanco> campos, Estilo estilo)
		{
			foreach (var campo in campos)
			{
				InserirCelula(campo.Chave, Convert.ToDouble(campo.Valor), estilo);
			}
		}

		private void InserirLinha(ColecaoDados dados, List<ValoresBanco> campos, Estilo estilo, Estilo alternado = null)
		{
			bool flag = true;

			dados.Linhas.Each((linha, i) =>
			{
				campos.Each((campo, j) =>
				{
					InserirCelula(dados[i, j].ToString(), Convert.ToDouble(campo.Valor), ((flag && alternado != null) ? estilo : alternado));//J é o indice da coluna

				});
				flag = !flag;
				Builder.EndRow();

			});
		}

		private Row InserirLinha(string campo, int largura, Estilo estilo = null)
		{
			List<ValoresBanco> lista = new List<ValoresBanco>();

			lista.Add(new ValoresBanco() { Chave = campo, Valor = largura.ToString() });

			return InserirLinha(lista, estilo);
		}

		private Row InserirLinha(List<ValoresBanco> campos, Estilo estilo = null)
		{
			if (campos == null || campos.Count == 0)
			{
				return null;
			}

			InserirCelula(campos, estilo ?? Estilos["Estilo_3_0"]);

			return Builder.EndRow();
		}

		private void EscreverLinhaEmBranco(int size = 5)
		{
			Builder.EndTable();

			Font font = Builder.Font;
			font.Size = 0;

			Builder.Writeln(ControlChar.SpaceChar.ToString());

			font.Size = size;

			Builder.StartTable();
		}

		private void EscreverDados(string nome = null, Estilo estilo = null)
		{
			InserirLinha(nome ?? "Dados Não Encontrados", 100, estilo ?? Estilos["Estilo_3_0"]);
		}

		private void Sumario(ColecaoDados dados, Dictionary<int, string> colunas, int estilo)
		{
			#region Cabeçalho

			double largura = ((Largura / (colunas.Count + 1)) / Largura) * 100;

			double larguraAux = largura + (100 - (largura * (colunas.Count + 1)));

			EscreverDados(Estilos["Estilo_" + estilo + "_0"].Texto, Estilos["Estilo_" + estilo + "_0"]);

			estilo++;

			InserirCelula(Estilos["Estilo_" + estilo + "_0"].Texto, larguraAux, Estilos["Estilo_" + estilo + "_0"]);

			foreach (var col in colunas)
			{
				InserirCelula(col.Value, largura, Estilos["Estilo_" + estilo + "_1"]);
			}
			Builder.EndRow();

			#endregion

			#region Corpo do sumário

			dados.Linhas.Each((linha, i) =>
			{
				int aux = estilo + linha;

				InserirCelula(Estilos["Estilo_" + (aux) + "_0"].Texto, larguraAux, Estilos["Estilo_" + (aux) + "_0"]);

				colunas.OrderBy(x => x.Key).ToList().Each((coluna, j) =>
				{
					InserirCelula(Convert.ToString(dados[linha, coluna.Key]), largura, Estilos["Estilo_" + (aux) + "_1"]);
				});

				Builder.EndRow();
			});

			#endregion
		}

		private void EscreverDadosSumario(ColecaoDados dados, Dictionary<int, string> colunas)
		{
			Sumario(dados, colunas, 5);//Sumario de grupo
		}

		private void EscreverDadosSumarioTotal(ColecaoDados dados, Dictionary<int, string> colunas)
		{
			Sumario(dados, colunas, 13);//Sumario de total
		}

		private void EscreverTotal(string valor, Estilo estiloCampo = null, Estilo estilovalor = null)
		{
			Estilo estilo1 = estiloCampo ?? Estilos["Estilo_20_0"];
			Estilo estilo2 = estilovalor ?? Estilos["Estilo_20_1"];

			double dif = (estilo1.Celula.Width + estilo2.Celula.Width) - Largura;

			InserirCelula(estilo1.Texto, ((estilo1.Celula.Width - (dif / 2)) / Largura) * 100, estilo1);
			InserirCelula(valor, ((estilo2.Celula.Width - (dif / 2)) / Largura) * 100, estilo2);

			Builder.EndRow();
		}

		private void EscreverTituloGrupo(string campo, string valor)
		{
			double dif = (Estilos["Estilo_1_0"].Celula.Width + Estilos["Estilo_1_1"].Celula.Width) - Largura;
			InserirCelula(Estilos["Estilo_1_0"].Texto, ((Estilos["Estilo_1_0"].Celula.Width - (dif / 2)) / Largura) * 100, Estilos["Estilo_1_0"]);
			InserirCelula(campo + ": " +valor, ((Estilos["Estilo_1_1"].Celula.Width - (dif / 2)) / Largura) * 100, Estilos["Estilo_1_1"]);
			Builder.EndRow();
		}

		#endregion
	}
}
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Entities;
using Tecnomapas.Blocos.RelatorioPersonalizado.Entities;
using Yogesh.ExcelXml;

namespace Tecnomapas.Blocos.RelatorioPersonalizado.Business
{
	public class ExportadorXls : IExportador
	{
		XmlStyle Normal { get; set; }
		XmlStyle Destaque { get; set; }

		DadosRelatorio _dados { get; set; }

		public ExportadorXls()
		{
			Normal = new XmlStyle();
			Destaque = new XmlStyle();
			Destaque.Font.Bold = true;
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
			arquivo.ContentType = "application/vnd.ms-excel";
			arquivo.Nome = "relatorio.xls";
		}

		public void Exportar(DadosRelatorio dados, Stream stream)
		{
			_dados = dados;
			ExcelXmlWorkbook pasta = new ExcelXmlWorkbook();

			pasta.Properties.Author = "Sistema Integrado de Monitoramento e Licenciamento Ambiental";
			pasta.Properties.Company = "Instituto de Defesa Agropecuária e Florestal";
			pasta.Properties.Title = dados.Nome;

			Worksheet planilha = pasta[0];

			IniciarRelatorio(dados, planilha);

			int linha = 2;
			if (dados.ComAgrupamento)
			{
				if (dados.Grupos.Count == 0)
				{
					linha = EscreverColunas(dados.Colunas.Select(x => x.Value).ToList(), planilha, linha);
					linha = EscreverSemDados(planilha, linha);
				}
				else
				{
					foreach (var grupo in dados.Grupos)
					{
						linha = EscreverTituloGrupo(grupo.Campo, grupo.Valor, planilha, linha);
						linha = EscreverColunas(dados.Colunas.Select(x => x.Value).ToList(), planilha, linha);
						linha = EscreverDados(grupo.Dados, planilha, linha);

						if (grupo.Sumarizacoes.Linhas.Count > 0)
						{
							linha = EscreverTituloSumario(planilha, linha, "Grupo");
							linha = EscreverSumario(grupo.Sumarizacoes, planilha, linha);
						}

						if (dados.Totalizar)
						{
							linha = EscreverTotalGrupo(grupo.Total, planilha, linha);
						}
						linha++;
					}

					if (dados.Sumarizacoes.Linhas.Count > 0)
					{
						linha = EscreverTituloSumario(planilha, linha);
						linha = EscreverColunas(dados.Colunas.Select(x => x.Value).ToList(), planilha, linha);
						linha = EscreverSumario(dados.Sumarizacoes, planilha, linha);
					}

					if (dados.Totalizar)
					{
						linha = EscreverTotal(dados.Total, planilha, linha);
					}
				}
			}
			else
			{
				if (dados.Dados.Linhas.Count == 0)
				{
					linha = EscreverColunas(dados.Colunas.Select(x => x.Value).ToList(), planilha, linha);
					linha = EscreverSemDados(planilha, linha);
				}
				else
				{
					linha = EscreverColunas(dados.Colunas.Select(x => x.Value).ToList(), planilha, linha);
					linha = EscreverDados(dados.Dados, planilha, linha);

					if (dados.Sumarizacoes.Linhas.Count > 0)
					{
						linha = EscreverTituloSumario(planilha, linha);
						linha = EscreverSumario(dados.Sumarizacoes, planilha, linha);
					}

					if (dados.Totalizar)
					{
						linha = EscreverTotal(dados.Total, planilha, linha);
					}
				}
			}

			pasta.Export(stream);
		}

		private int EscreverSemDados(Worksheet planilha, int linha)
		{
			planilha[0, linha].Style = Normal;
			planilha[0, linha].Value = "Dados Não Encontrados";
			return linha + 1;
		}

		private int EscreverTotal(int quant, Worksheet planilha, int linhaInicial)
		{
			planilha[0, linhaInicial].Style = Destaque;
			planilha[0, linhaInicial].Value = "Total Geral";
			planilha[1, linhaInicial].Style = Normal;
			planilha[1, linhaInicial].Value = quant;
			return linhaInicial + 1;
		}

		private int EscreverTotalGrupo(int quant, Worksheet planilha, int linhaInicial)
		{
			planilha[0, linhaInicial].Style = Destaque;
			planilha[0, linhaInicial].Value = "Total Grupo";
			planilha[1, linhaInicial].Style = Normal;
			planilha[1, linhaInicial].Value = quant;
			return linhaInicial + 1;
		}

		private int EscreverTituloGrupo(string campo, string valor, Worksheet planilha, int linhaInicial)
		{
			planilha[0, linhaInicial].Style = Destaque;
			planilha[0, linhaInicial].Value = campo;
			planilha[1, linhaInicial].Style = Normal;
			planilha[1, linhaInicial].Value = valor;
			return linhaInicial + 1;
		}

		private int EscreverTituloSumario(Worksheet planilha, int linhaInicial, string sumarioTexto = "Geral")
		{
			planilha[0, linhaInicial].Style = Destaque;
			planilha[0, linhaInicial].Value = "Sumário " + sumarioTexto;
			return linhaInicial + 1;
		}

		private int EscreverColunas(List<string> colunas, Worksheet planilha, int linhaInicial)
		{
			planilha[0, linhaInicial].Style = Destaque;
			planilha[0, linhaInicial].Value = "Item";

			// Titulos de Colunas
			colunas.Each((nome, i) =>
			{
				planilha[i + 1, linhaInicial].Style = Destaque;
				planilha[i + 1, linhaInicial].Value = nome;
			});

			return linhaInicial + 1;
		}

		private void IniciarRelatorio(DadosRelatorio dados, Worksheet planilha)
		{
			planilha.Name = "Relatório";
			planilha[0, 0].Value = dados.Nome;
			planilha[0, 0].Style = Destaque;
		}

		private int EscreverDados(ColecaoDados dados, Worksheet planilha, int linhaInicial)
		{
			if (dados.Linhas.Count == 0) return linhaInicial;

			// Dados
			dados.Linhas.Each((linhaAtual, l) =>
			{
				planilha[0, l + linhaInicial].Style = Destaque;
				planilha[0, l + linhaInicial].Value = linhaAtual + 1;

				_dados.Colunas.Each((colunaAtual, c) =>
				{
					planilha[c + 1, l + linhaInicial].Style = Normal;
					planilha[c + 1, l + linhaInicial].Value = dados[linhaAtual, c];
				});
			});

			return linhaInicial + dados.Linhas.Count;
		}

		private int EscreverSumario(ColecaoDados dados, Worksheet planilha, int linhaInicial)
		{
			if (dados.Linhas.Count == 0) return linhaInicial;

			// Dados
			dados.Linhas.Each((linhaAtual, l) =>
			{
				planilha[0, l + linhaInicial].Style = Destaque;
				planilha[0, l + linhaInicial].Value = ObterSumario(linhaAtual);

				_dados.Colunas.Each((colunaAtual, c) =>
				{
					planilha[c + 1, l + linhaInicial].Style = Normal;
					planilha[c + 1, l + linhaInicial].Value = dados[linhaAtual, c];
				});
			});

			return linhaInicial + dados.Linhas.Count;
		}

		private string ObterSumario(int sumario)
		{
			switch ((eTipoSumario)sumario)
			{
				case eTipoSumario.Contar:
					return "Contar";

				case eTipoSumario.Somar:
					return "Somar";

				case eTipoSumario.Media:
					return "Média";

				case eTipoSumario.Maximo:
					return "Máximo";

				case eTipoSumario.Minimo:
					return "Mínimo";
			}

			return string.Empty;
		}
	}
}
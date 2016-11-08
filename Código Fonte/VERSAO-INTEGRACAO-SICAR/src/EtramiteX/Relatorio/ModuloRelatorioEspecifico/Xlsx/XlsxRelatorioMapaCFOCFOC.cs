﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloRelatorioEspecifico.Data;
using Tecnomapas.Blocos.Entities.Interno.ModuloRelatorioEspecifico;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx.CabecalhoRodape;
using Tecnomapas.Blocos.Entities.Etx.ModuloRelatorio;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;


namespace Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloRelatorioEspecifico.Xlsx
{
	public class XlsxRelatorioMapaCFOCFOC
	{
		public static MemoryStream Gerar(RelatorioMapaFiltroeResultado filtro)
			{
				RelatorioMapaDa _da = new RelatorioMapaDa();
				using (var pck = new ExcelPackage())
				{

					RelatorioMapaFiltroeResultado dataSource = new RelatorioMapaFiltroeResultado();

					dataSource = _da.RelatorioCFOCFOC(filtro);
					dataSource.DataInicial = filtro.DataInicial;
					dataSource.DataFinal = filtro.DataFinal;
					dataSource.tipoRelatorio = filtro.tipoRelatorio;
					dataSource.LocalRelatorio = filtro.LocalRelatorio;
					dataSource.DataRelatorio = filtro.DataRelatorio;
					dataSource.NomeFuncionario = filtro.NomeFuncionario;

					SetorEndereco endereco = new CabecalhoRodapeDa().ObterEndSetor(filtro.IdSetor);
					dataSource.LocalRelatorio = endereco.MunicipioTexto;



   					//Criando a Planilha
					var ws = pck.Workbook.Worksheets.Add("Relatório Mapa CFOCFOC");

					//Criando Cabeçalho da Tabela
					ws.Cells[1, 1].Value = "Data";
					ws.Cells[1, 2].Value = "Produto(s)";
					ws.Cells[1, 3].Value = "Nº CFO";
					ws.Cells[1, 4].Value = "Nº CFOC";
					ws.Cells[1, 5].Value = "Quantidade";
					ws.Cells[1, 6].Value = "Unidade";
					ws.Cells[1, 7].Value = "Unidade de Origem";
					ws.Cells[1, 8].Value = "Município Emissão";

					using (var header = ws.Cells[1, 1, 1, 8])
					{
						header.Style.Font.Bold = true;
						header.Style.Fill.PatternType = ExcelFillStyle.Solid; //Set Pattern for the background to Solid
						header.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189)); //Set color to dark blue
						header.Style.Font.Color.SetColor(Color.White);
					}

					var planilhaResultados = new List<Object[]>();
					foreach (var item in dataSource.ItensRelatorioMapaCFOCFOC)
					{
						var Linhas = new List<Object>();

						Linhas.Add(item.DataEmissao);
						Linhas.Add(item.CulturaCultivar);
						Linhas.Add(item.NumeroCFO);
						Linhas.Add(item.NumeroCFOC);
						Linhas.Add(item.Quantidade);
						Linhas.Add(item.UnidadeMedida);
						Linhas.Add(item.UnidadeOrigem);
						Linhas.Add(item.Municipio);

						planilhaResultados.Add(Linhas.ToArray());
					}

					ws.Cells[2, 1].LoadFromArrays(planilhaResultados);

					var stream = new MemoryStream(pck.GetAsByteArray());
					return stream;
					
				}

			}
	}
}

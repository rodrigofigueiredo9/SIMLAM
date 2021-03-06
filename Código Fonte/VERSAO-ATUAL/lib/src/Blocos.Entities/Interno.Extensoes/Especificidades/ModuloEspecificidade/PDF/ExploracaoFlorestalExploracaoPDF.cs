﻿using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloExploracaoFlorestal;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF
{
	public class ExploracaoFlorestalExploracaoPDF
	{
		public String IdentificacaoGeo { set; get; }
		public String Geometria { set; get; }
		public String FinalidadeExploracao { set; get; }
		public String ParecerFavoravel { set; get; }
		public String ClassificacaoVegetal { set; get; }

		public Int32 GeometriaTipoId { set; get; }
		public String TipoExploracao { set; get; }
		public String VegetacaoTipo { get; set; }
		public Int32 VegetacaoTipoId { get; set; }
		public String ArvoresRequeridas { set; get; }
		
		public String QuantidadeArvores { set; get; }

		public String UnidadeMedida
		{
			get
			{
				if (this.GeometriaTipoId == (int)eExploracaoFlorestalGeometria.Poligono &&
					!string.IsNullOrWhiteSpace(this.AreaCroqui))
				{
					return "m²";
				}
				else
				{
					return "un";
				}
			}
		}

		public String AreaRequerida { get { return (VegetacaoTipoId == (int)eExploracaoFlorestalClassificacaoVegetacao.ArvoresIsoladas) ? AreaRequeridaDecimal.ToString() : AreaRequeridaDecimal.ToStringTrunc(); } }
		public Decimal AreaRequeridaDecimal { set; get; }

		public String AreaCroqui { get { return (VegetacaoTipoId == (int)eExploracaoFlorestalClassificacaoVegetacao.ArvoresIsoladas) ? AreaCroquiDecimal.ToString() : AreaCroquiDecimal.ToStringTrunc(); } }
		public Decimal AreaCroquiDecimal { set; get; }

		public String AreaCroquiQuantidadeArvores
		{
			get
			{
				return AreaCroqui;
			}
		}

		public String UnidadeMedidaCroquiArvores
		{
			get
			{
				if (this.GeometriaTipoId != (int)eExploracaoFlorestalGeometria.Poligono)
				{
					return "un";
				}
				else
				{
					return "m²";
				}
			}
		}

		private List<ExploracaoFlorestalExploracaoProdutoPDF> _produtos = new List<ExploracaoFlorestalExploracaoProdutoPDF>();
		public List<ExploracaoFlorestalExploracaoProdutoPDF> Produtos
		{
			get { return _produtos; }
			set { _produtos = value; }
		}

		#region Valores em Hectares

		public String UnidadeMedidaHa
		{
			get
			{
				if (this.GeometriaTipoId == (int)eExploracaoFlorestalGeometria.Poligono &&
					!string.IsNullOrWhiteSpace(this.AreaCroquiHa))
				{
					return "ha";
				}
				else
				{
					return "un";
				}
			}
		}

		public String AreaRequeridaHa
		{
			get
			{
				if (UnidadeMedida == "un") 
				{
					return AreaRequerida;
				}

				if (AreaRequeridaDecimal > 0)
				{
					return AreaRequeridaDecimal.Convert(eMetrica.M2ToHa).ToStringTrunc(4);
				}

				return String.Empty;
			}
		}

		public String AreaCroquiHa
		{
			get
			{
				if (UnidadeMedida == "un")
				{
					return AreaCroqui;
				}

				if (AreaCroquiDecimal > 0)
				{
					return AreaCroquiDecimal.Convert(eMetrica.M2ToHa).ToStringTrunc(4);
				}

				return String.Empty;
			}
		}

		#endregion

		public ExploracaoFlorestalExploracaoPDF() { }

		public ExploracaoFlorestalExploracaoPDF(ExploracaoFlorestalExploracao exploracao)
		{
			GeometriaTipoId = exploracao.GeometriaTipoId;
			VegetacaoTipo = exploracao.ClassificacaoVegetacaoTexto;
			VegetacaoTipoId = exploracao.ClassificacaoVegetacaoId;
			ArvoresRequeridas = exploracao.ArvoresRequeridas;

			AreaCroquiDecimal = (exploracao.GeometriaTipoId == (int)eExploracaoFlorestalGeometria.Ponto && exploracao.ParecerFavoravel == true) ? Convert.ToDecimal(exploracao.QuantidadeArvores) : exploracao.AreaCroqui;
			if (exploracao.ParecerFavoravel == false)
				AreaCroquiDecimal = 0;
			AreaRequeridaDecimal = (exploracao.GeometriaTipoId == (int)eExploracaoFlorestalGeometria.Ponto) ? Convert.ToDecimal(exploracao.ArvoresRequeridas) : exploracao.AreaRequerida;
			
			QuantidadeArvores = exploracao.QuantidadeArvores;
			Produtos = exploracao.Produtos.Select(x => new ExploracaoFlorestalExploracaoProdutoPDF(x)).ToList();
			if(Produtos.Count == 0)
				Produtos.Add(new ExploracaoFlorestalExploracaoProdutoPDF());
			IdentificacaoGeo = exploracao.Identificacao;
			Geometria = exploracao.GeometriaTipoTexto;
			FinalidadeExploracao = String.IsNullOrWhiteSpace(exploracao.FinalidadeExploracaoTexto) ? exploracao.FinalidadeEspecificar : exploracao.FinalidadeExploracaoTexto;
			ParecerFavoravel = Convert.ToBoolean(exploracao.ParecerFavoravel) ? "Favorável" : "Não Favorável";
			ClassificacaoVegetal = exploracao.ClassificacaoVegetacaoTexto;
		}
	}
}

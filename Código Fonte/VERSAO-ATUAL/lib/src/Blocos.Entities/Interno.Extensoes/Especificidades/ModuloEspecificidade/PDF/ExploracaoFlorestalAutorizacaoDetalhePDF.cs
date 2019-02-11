using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloExploracaoFlorestal;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF
{
	public class ExploracaoFlorestalAutorizacaoDetalhePDF
	{
		public Int32 GeometriaTipoId { set; get; }
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

		public ExploracaoFlorestalAutorizacaoDetalhePDF() { }

		public ExploracaoFlorestalAutorizacaoDetalhePDF(ExploracaoFlorestalExploracao exploracao)
		{
			GeometriaTipoId = exploracao.GeometriaTipoId;
			VegetacaoTipo = exploracao.ClassificacaoVegetacaoTexto;
			VegetacaoTipoId = exploracao.ClassificacaoVegetacaoId;
			ArvoresRequeridas = exploracao.ArvoresRequeridas;

			AreaCroquiDecimal = exploracao.AreaCroqui;
			AreaRequeridaDecimal = exploracao.AreaRequerida;

			QuantidadeArvores = exploracao.QuantidadeArvores;
		}
	}
}

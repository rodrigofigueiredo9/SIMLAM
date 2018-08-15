using System;
using System.Collections.Generic;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloExploracaoFlorestal
{
	public class ExploracaoFlorestalExploracao
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		public String Identificacao { get; set; }
		public String GeometriaTipoTexto { get; set; }
		public String ClassificacaoVegetacaoTexto { get; set; }
		public String ExploracaoTipoTexto { get; set; }
		public Decimal AreaCroqui { get; set; }
		public String QuantidadeArvores { get; set; }
		public Decimal AreaRequerida { get; set; }
		public String AreaRequeridaTexto { get; set; }
		public String ArvoresRequeridas { get; set; }
		public Int32 ExploracaoTipoId { get; set; }
		public Int32 ClassificacaoVegetacaoId { get; set; }
		public Int32? FinalidadeExploracao { get; set; }
		public bool ParecerFavoravel { get; set; }
		private Int32 _geometriaTipGeo = 0;
		public Int32 GeometriaTipoId
		{
			get 
			{
				return _geometriaTipGeo;
			}
			set 
			{
				switch (value)
				{
					case 1:
					case 2001:
						_geometriaTipGeo = (int)eExploracaoFlorestalGeometria.Ponto;
						break;

					case 2:
					case 2002:
						_geometriaTipGeo = (int)eExploracaoFlorestalGeometria.Linha;
						break;

					case 3:
					case 2003:
						_geometriaTipGeo = (int)eExploracaoFlorestalGeometria.Poligono;
						break;
				}
			}
		}

		private List<ExploracaoFlorestalProduto> _produtos = new List<ExploracaoFlorestalProduto>();
		public List<ExploracaoFlorestalProduto> Produtos
		{
			get { return _produtos; }
			set { _produtos = value; }
		}

		public string UnidadeMedida { get; set; }
	}
}
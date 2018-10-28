using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloExploracaoFlorestal;
using Tecnomapas.Blocos.Entities.Model.Business;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF
{
	public class ExploracaoFlorestalAutorizacaoPDF
	{
		public String TipoExploracao { set; get; }
		public String TotalPoligono { set; get; }

		private List<ExploracaoFlorestalAutorizacaoDetalhePDF> _detalhe = new List<ExploracaoFlorestalAutorizacaoDetalhePDF>();
		public List<ExploracaoFlorestalAutorizacaoDetalhePDF> Detalhe
		{
			get { return _detalhe; }
			set { _detalhe = value; }
		}

		public ExploracaoFlorestalAutorizacaoPDF() { }

		public ExploracaoFlorestalAutorizacaoPDF(ExploracaoFlorestal exploracaoFlorestal)
		{
			TipoExploracao = exploracaoFlorestal.TipoExploracaoTexto;
			if (exploracaoFlorestal.Exploracoes.FirstOrDefault().GeometriaTipoId == (int)eExploracaoFlorestalGeometria.Ponto)
			{
				TotalPoligono = exploracaoFlorestal.Exploracoes.Sum(x => Convert.ToDecimal(string.IsNullOrWhiteSpace(x.QuantidadeArvores) ? "0" : x.QuantidadeArvores)).ToString("N2");
				Detalhe = exploracaoFlorestal.Exploracoes.GroupBy(x => new { x.ClassificacaoVegetacaoTexto, x.GeometriaTipoId }, x => Convert.ToDecimal(x.QuantidadeArvores),
					(key, g) => new ExploracaoFlorestalAutorizacaoDetalhePDF()
					{
						VegetacaoTipo = key.ClassificacaoVegetacaoTexto,
						GeometriaTipoId = key.GeometriaTipoId,
						AreaCroquiDecimal = g.Sum(x => x)
					}).ToList();
			}
			else
			{
				TotalPoligono = exploracaoFlorestal.Exploracoes.Sum(x => x.AreaCroqui).ToString("N2");
				Detalhe = exploracaoFlorestal.Exploracoes.GroupBy(x => new { x.ClassificacaoVegetacaoTexto, x.GeometriaTipoId }, x => x.AreaCroqui,
					(key, g) => new ExploracaoFlorestalAutorizacaoDetalhePDF()
					{
						VegetacaoTipo = key.ClassificacaoVegetacaoTexto,
						GeometriaTipoId = key.GeometriaTipoId,
						AreaCroquiDecimal = g.Sum(x => x)
					}).ToList();
			}
		}
	}
}

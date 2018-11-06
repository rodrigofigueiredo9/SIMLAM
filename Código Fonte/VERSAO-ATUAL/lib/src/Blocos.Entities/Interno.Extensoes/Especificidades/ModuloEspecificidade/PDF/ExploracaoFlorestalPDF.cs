using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloExploracaoFlorestal;
using Tecnomapas.Blocos.Entities.Model.Business;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF
{
	public class ExploracaoFlorestalPDF
	{
		public List<String> Finalidades { set; get; }
		public String CodigoExploracao { set; get; }
		public String TipoExploracao { set; get; }
		public String DataCadastro { set; get; }
		public String FinalidadesStragg 
		{
			get{ return (Finalidades is null ? "" : String.Join(",", Finalidades?.ToArray()));}
		}

		private List<ExploracaoFlorestalExploracaoPDF> _exploracoes = new List<ExploracaoFlorestalExploracaoPDF>();

		public List<ExploracaoFlorestalExploracaoPDF> Exploracoes
		{
			get { return _exploracoes; }
			set { _exploracoes = value; }
		}

		public List<ExploracaoFlorestalExploracaoPDF> ExploracoesTotaisTipo
		{
			get
			{
				return (
				   from exp in Exploracoes
				   group exp by exp.TipoExploracao into exp
				   select new ExploracaoFlorestalExploracaoPDF()
				   {
					   TipoExploracao = exp.Key,
					   GeometriaTipoId = exp.First().GeometriaTipoId,
					   AreaRequeridaDecimal = exp.Sum(e => e.AreaRequeridaDecimal),
					   AreaCroquiDecimal = exp.Sum(e => e.AreaCroquiDecimal),
					   QuantidadeArvores = exp.Sum(e => String.IsNullOrEmpty(e.QuantidadeArvores) ? Decimal.Zero : Convert.ToDecimal(e.QuantidadeArvores)).ToString()
				   }).ToList();
			}
		}
		
		public List<ExploracaoFlorestalExploracaoPDF> CorteRasoExploracoes
		{
			get 
			{
				return (
					from corteRaso in this.Exploracoes.Where(x => x.TipoExploracao == "Corte raso")
					group corteRaso by corteRaso.VegetacaoTipoId into corteRaso
					select new ExploracaoFlorestalExploracaoPDF()
					{
						VegetacaoTipo = corteRaso.First().VegetacaoTipo,
						GeometriaTipoId = corteRaso.First().GeometriaTipoId,

						AreaRequeridaDecimal = (corteRaso.Key == (int)eExploracaoFlorestalClassificacaoVegetacao.ArvoresIsoladas) ?
										 (corteRaso.Sum(e => String.IsNullOrEmpty(e.ArvoresRequeridas) ? Decimal.Zero : Convert.ToDecimal(e.ArvoresRequeridas))) :
										(corteRaso.Sum(e => e.AreaRequeridaDecimal)),

						AreaCroquiDecimal = (corteRaso.Key == (int)eExploracaoFlorestalClassificacaoVegetacao.ArvoresIsoladas) ?
									(corteRaso.Sum(e => String.IsNullOrEmpty(e.QuantidadeArvores) ? Decimal.Zero : Convert.ToDecimal(e.QuantidadeArvores))) :
									(corteRaso.Sum(e => e.AreaCroquiDecimal) ),
						
						Produtos = (from prod in corteRaso.SelectMany(x => x.Produtos) 
									group prod by prod.Nome into prod 
									select new ExploracaoFlorestalExploracaoProdutoPDF() { 
										Nome = prod.Key,
										UnidadeMedida = prod.First().UnidadeMedida,
										Quantidade = prod.Sum(p => p.Quantidade)
									}).ToList()
					}).ToList();
			}
		}

		public List<ExploracaoFlorestalExploracaoProdutoPDF> CorteRasoProdutos
		{
			get
			{
				return (
					from prod in this.Exploracoes.Where(x => x.TipoExploracao == "Corte raso").SelectMany(x => x.Produtos)
					group prod by prod.Nome into prod
					select new ExploracaoFlorestalExploracaoProdutoPDF()
					{
						Nome = prod.Key,
						UnidadeMedida = prod.First().UnidadeMedida,
						Quantidade = prod.Sum(p => p.Quantidade)
					}).ToList();

			}
		}

		public List<ExploracaoFlorestalExploracaoPDF> CorteSeletivoExploracoes
		{
			get
			{
				return (
					from corteRaso in this.Exploracoes.Where(x => x.TipoExploracao == "Corte seletivo")
					group corteRaso by corteRaso.VegetacaoTipoId into corteRaso
					select new ExploracaoFlorestalExploracaoPDF()
					{
						VegetacaoTipo = corteRaso.First().VegetacaoTipo,
						GeometriaTipoId = corteRaso.First().GeometriaTipoId,

						AreaRequeridaDecimal = (corteRaso.Key == (int)eExploracaoFlorestalClassificacaoVegetacao.ArvoresIsoladas) ?
										 (corteRaso.Sum(e => String.IsNullOrEmpty(e.ArvoresRequeridas) ? Decimal.Zero : Convert.ToDecimal(e.ArvoresRequeridas))) :
										(corteRaso.Sum(e => e.AreaRequeridaDecimal)),

						AreaCroquiDecimal = (corteRaso.Key == (int)eExploracaoFlorestalClassificacaoVegetacao.ArvoresIsoladas) ?
									(corteRaso.Sum(e => String.IsNullOrEmpty(e.QuantidadeArvores) ? Decimal.Zero : Convert.ToDecimal(e.QuantidadeArvores))) :
									(corteRaso.Sum(e => e.AreaCroquiDecimal))
					}).ToList();
			}
		}

		public List<ExploracaoFlorestalExploracaoProdutoPDF> CorteSeletivoProdutos
		{
			get
			{
				return (
					from prod in this.Exploracoes.Where(x => x.TipoExploracao == "Corte seletivo").SelectMany(x => x.Produtos)
					group prod by prod.Nome into prod
					select new ExploracaoFlorestalExploracaoProdutoPDF()
					{
						Nome = prod.Key,
						UnidadeMedida = prod.First().UnidadeMedida,
						Quantidade = prod.Sum(p => p.Quantidade)
					}).ToList();

			}
		}

		public String AreaTotalCorte
		{
			get { return _exploracoes.Sum(x => x.AreaRequeridaDecimal).ToStringTrunc(); }
		}

		public String AreaTotalCroqui 
		{
			get { return _exploracoes.Sum(x => x.AreaCroquiDecimal).ToStringTrunc(); }
		}

		public ExploracaoFlorestalPDF() { }

		public ExploracaoFlorestalPDF(ExploracaoFlorestal exploracaoFlorestal)
		{
			_exploracoes = exploracaoFlorestal.Exploracoes.Select(x => new ExploracaoFlorestalExploracaoPDF(x)).ToList();
			var exploracoesFirst = exploracaoFlorestal.Exploracoes.FirstOrDefault() ?? new ExploracaoFlorestalExploracao();
			int auxFinalidades = (exploracoesFirst.FinalidadeExploracao.HasValue) ? exploracoesFirst.FinalidadeExploracao.Value : 0;
			Finalidades = EntitiesBus.ObterFinalidades(auxFinalidades);
			CodigoExploracao = exploracaoFlorestal.CodigoExploracaoTexto;
			TipoExploracao = exploracaoFlorestal.TipoExploracaoTexto;
			DataCadastro = exploracaoFlorestal.DataCadastro.DataTexto;
		}
	}
}

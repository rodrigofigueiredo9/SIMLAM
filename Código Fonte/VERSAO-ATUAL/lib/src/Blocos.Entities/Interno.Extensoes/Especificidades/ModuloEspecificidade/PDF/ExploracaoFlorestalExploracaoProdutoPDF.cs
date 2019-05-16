using System;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloExploracaoFlorestal;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF
{
	public class ExploracaoFlorestalExploracaoProdutoPDF
	{
		public String Nome { get; set; }
		public Decimal Quantidade { get; set; }
		public String UnidadeMedida { get; set; }
		public String Especie { get; set; }
		public String DestinacaoMaterial { get; set; }

		public String QuantidadeTexto
		{
			get
			{
				return (Quantidade <= 0) ? " - " : (UnidadeMedida.Contains("und") ? Quantidade.ToStringTrunc(0) : Quantidade.ToStringTrunc(2));
			}
		}

		public ExploracaoFlorestalExploracaoProdutoPDF() { }

		public ExploracaoFlorestalExploracaoProdutoPDF(ExploracaoFlorestalProduto produto)
		{
			Especie = produto.EspeciePopularTexto;
			DestinacaoMaterial = produto.DestinacaoMaterialTexto;
			if (produto.ProdutoId == (int)eProduto.SemRendimento)
			{
				Nome = produto.ProdutoTexto;
				Quantidade = 0;
				UnidadeMedida = null;
				return;
			}

			String[] produtoTexto = produto.ProdutoTexto.Split(' ');
			Decimal quantidade = 0M;
			Decimal.TryParse(produto.Quantidade, out quantidade);

			Nome = produtoTexto[0];
			UnidadeMedida = produtoTexto[1].Substring(1, produtoTexto[1].Length - 2);
			Quantidade = quantidade;
		}
	}
}
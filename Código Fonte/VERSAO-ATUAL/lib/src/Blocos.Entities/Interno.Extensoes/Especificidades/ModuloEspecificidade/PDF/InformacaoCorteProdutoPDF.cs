using System;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloInformacaoCorte;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF
{
	public class InformacaoCorteProdutoPDF
	{
		public String Produto { get; set; }
		public String QdeComercializacao { get; set; }
		public String QdeUsoProprio { get; set; }

		public InformacaoCorteProdutoPDF(InformacaoCorteDestinacao produto)
		{
			this.Produto = produto.ProdutoTexto;

			switch (produto.DestinacaoMaterial)
			{
				case 1:
					this.QdeComercializacao = produto.Quantidade.ToString();
					break;
				case 2:
					this.QdeUsoProprio = produto.Quantidade.ToString();
					break;
			}
		}
	}
}
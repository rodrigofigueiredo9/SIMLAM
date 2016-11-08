using System;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloInformacaoCorte;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF
{
	public class InformacaoCorteProdutoPDF
	{
		public String Produto { get; set; }
		public String QdeComercializacao { get; set; }
		public String QdeUsoProprio { get; set; }

		public InformacaoCorteProdutoPDF(Produto produto)
		{
			this.Produto = produto.ProdutoTipoTexto;

			switch (produto.DestinacaoTipo)
			{
				case 1:
					this.QdeComercializacao = produto.Quantidade;
					break;
				case 2:
					this.QdeUsoProprio = produto.Quantidade;
					break;
			}
		}
	}
}
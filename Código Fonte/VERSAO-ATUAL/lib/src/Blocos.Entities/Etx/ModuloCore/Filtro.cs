

using System;

namespace Tecnomapas.Blocos.Entities.Etx.ModuloCore
{
	public class Filtro<T>
	{
		public T Dados { get; set; }

		public Paginacao Paginacao { get; set; }
		public Int32 Maior { get; set; }
		public Int32 Menor { get; set; }
		public Int32 OdenarPor { get { return Paginacao.OrdenarPor; } }

		public Filtro(T dados)
		{
			Dados = dados;
			Paginacao = new Paginacao();
			Paginacao.OrdenarPor = 0;
		}
		public Filtro(T dados, Paginacao paginacao)
		{
			Dados = dados;
			Paginacao = paginacao;
			Menor = ((paginacao.PaginaAtual) * paginacao.QuantPaginacao) - (paginacao.QuantPaginacao - 1);
			Maior = ((paginacao.PaginaAtual) * paginacao.QuantPaginacao);
		}
	}
}

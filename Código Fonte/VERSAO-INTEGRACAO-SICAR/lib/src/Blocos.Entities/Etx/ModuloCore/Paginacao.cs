using System.Collections.Generic;
using System.Web.Mvc;

namespace Tecnomapas.Blocos.Entities.Etx.ModuloCore
{
	public class Paginacao
	{
		private int _quantPaginacao = 5;
		private int _numeroPaginas = 1;
		private int _paginasNavegacao = 10;
		private int _paginaAtual = 1;
		private int _paginaInicial = 1;
		private int _paginaFinal = 1;
		private int _quantidadeRegistros = 0;
		private int _ordenarPor = 1;
		private string _ordenarPorValor = "";
		private List<string> _colunas = new List<string>();
		public List<SelectListItem> _listaQuantPaginacao = new List<SelectListItem>();

		public void EfetuarPaginacao()
		{
			NumeroPaginas = QuantidadeRegistros - (QuantidadeRegistros % QuantPaginacao);

			if ((NumeroPaginas / QuantPaginacao) < 1)
			{
				NumeroPaginas = 1;
			}
			else
			{
				NumeroPaginas = NumeroPaginas / QuantPaginacao;

				if (QuantidadeRegistros % QuantPaginacao > 0)
				{
					NumeroPaginas += 1;
				}
			}
			
			PaginaFinal = PaginaAtual + 5;
			PaginaInicial = PaginaFinal - 9;

			if (PaginaAtual <= 5)
			{
				PaginaFinal = 10;
			}
			
			if (PaginaFinal > NumeroPaginas)
			{
				PaginaFinal = NumeroPaginas;
				PaginaInicial = PaginaFinal - 9;
			}
			
			if (PaginaInicial <= 0)
			{
				PaginaInicial = 1;
			}

			if (PaginaAtual < PaginaInicial)
			{
				PaginaAtual = PaginaInicial;
			}
			else if (PaginaAtual > PaginaFinal)
			{
				PaginaAtual = PaginaFinal;
			}
		}

		public int QuantPaginacao
		{
			get { return _quantPaginacao; }
			set { _quantPaginacao = value; }
		}

		public int NumeroPaginas
		{
			get { return _numeroPaginas; }
			set { _numeroPaginas = value; }
		}

		public int PaginasNavegacao
		{
			get { return _paginasNavegacao; }
			set { _paginasNavegacao = value; }
		}

		public int PaginaAtual
		{
			get { return _paginaAtual; }
			set { _paginaAtual = value; }
		}

		public int PaginaInicial
		{
			get { return _paginaInicial; }
			set { _paginaInicial = value; }
		}

		public int PaginaFinal
		{
			get { return _paginaFinal; }
			set { _paginaFinal = value; }
		}

		public int QuantidadeRegistros
		{
			get { return _quantidadeRegistros; }
			set { _quantidadeRegistros = value; }
		}

		public int OrdenarPor
		{
			get {
				return _ordenarPor; 
			}
			set {
				if (OrdenarPor < 1 || OrdenarPor > Colunas.Count) _ordenarPor = 1;
				_ordenarPor = value; 
			}
		}

		public string OrdenarPorValor
		{
			get {
				if (OrdenarPor < 1 || OrdenarPor > Colunas.Count) _ordenarPor = 1;
				if (Colunas.Count > 0) {
					_ordenarPorValor = _colunas[OrdenarPor-1];
				}
				return _ordenarPorValor; 
			}
		}

		public List<string> Colunas
		{
			get { return _colunas; }
			set { _colunas = value; }
		}

		public List<SelectListItem> ListaQuantPaginacao
		{
			get { return _listaQuantPaginacao; }
			set { _listaQuantPaginacao = value; }
		}
	}
}
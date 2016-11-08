using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo
{
	public class TituloModeloListarVM
	{
		public string UltimaBusca { get; set; }
		public Paginacao Paginacao { get; set; }
		public List<TituloModelo> Resultados { get; set; }
		public TituloModeloListarFiltro Filtros { get; set; }

		public List<SelectListItem> Tipos { get; set; }
		public List<SelectListItem> Situacoes { get; set; }
		public List<SelectListItem> Setores { get; set; }


		public bool PodeEditar { get; set; }
		public bool PodeVisualizar { get; set; }

		public bool PodeAlterarSituacao { get; set; }

		public TituloModeloListarVM() 
		{
			Paginacao = new Paginacao();
			Filtros = new TituloModeloListarFiltro();
			Resultados = new List<TituloModelo>();
		}

		public TituloModeloListarVM(List<QuantPaginacao> quantPaginacao, List<Setor> setores, List<TituloModeloTipo> tipos, List<Situacao> situacoes)
		{
			Paginacao = new Paginacao();
			Filtros = new TituloModeloListarFiltro();
			Resultados = new List<TituloModelo>();

			Tipos = ViewModelHelper.CriarSelectList(tipos, true);
			Situacoes = ViewModelHelper.CriarSelectList(situacoes, true);
			Setores = ViewModelHelper.CriarSelectList(setores, true);
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false);
		}

		public void SetListItens(List<QuantPaginacao> quantPaginacao, int quantidadePagina = 5)
		{
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false, selecionadoTexto: quantidadePagina.ToString());
		}
	}
}
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloAdministrador;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMAdministrador
{
	public class ListarVM
	{
		public AdministradorListarFiltro Filtros { get; set; }
		public List<SelectListItem> Situacoes { get; set; }
		public Paginacao Paginacao { get; set; }
		public List<Administrador> Resultados { get; set; }

		public string UltimaBusca { get; set; }
		public bool PodeEditar { get; set; }
		public bool PodeVisualizar { get; set; }

		public ListarVM()
		{
			Paginacao = new Paginacao();
			Filtros = new AdministradorListarFiltro();
			Resultados = new List<Administrador>();
		}

		public ListarVM(List<Situacao> situacoes, List<QuantPaginacao> quantPaginacao)
		{
			Paginacao = new Paginacao();
			Filtros = new AdministradorListarFiltro();
			Resultados = new List<Administrador>();

			Situacoes = ViewModelHelper.CriarSelectList(situacoes);
		}

		public void SetListItens(List<QuantPaginacao> quantPaginacao, int quantidadePagina = 5)
		{
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false, selecionadoTexto: quantidadePagina.ToString());
		}
	}
}
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloFuncionario;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMFuncionario
{
	public class ListarVM
	{
		public FuncionarioListarFiltro Filtros { get; set; }
		public List<SelectListItem> Situacoes { get; set; }
		public List<SelectListItem> Cargos { get; set; }
		public List<SelectListItem> Setores { get; set; }
		public List<SelectListItem> ListaQuantPaginacao { get; set; }
		public Paginacao Paginacao { get; set; }
		public String UltimaBusca { get; set; }
		public Boolean PodeVisualizar { get; set; }
		public Boolean PodeEditar { get; set; }
		public Boolean PodeAssociar { get; set; }
		public Boolean PodeAlterarSituacao { get; set; }
		public List<Funcionario> Resultados { get; set; }

		public ListarVM()
		{
			Paginacao = new Paginacao();
			Filtros = new FuncionarioListarFiltro();
			Resultados = new List<Funcionario>();
		}

		public ListarVM(List<Situacao> situacoes, List<Cargo> cargos, List<Setor> setores, List<QuantPaginacao> quantPaginacao)
		{
			Paginacao = new Paginacao();
			Filtros = new FuncionarioListarFiltro();
			Resultados = new List<Funcionario>();

			Situacoes = ViewModelHelper.CriarSelectList(situacoes);
			Cargos = ViewModelHelper.CriarSelectList(cargos);
			Setores = ViewModelHelper.CriarSelectList(setores);
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false);
		}

		public void SetListItens(List<QuantPaginacao> quantPaginacao, int quantidadePagina = 5)
		{
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false, selecionadoTexto: quantidadePagina.ToString());
		}
	}
}
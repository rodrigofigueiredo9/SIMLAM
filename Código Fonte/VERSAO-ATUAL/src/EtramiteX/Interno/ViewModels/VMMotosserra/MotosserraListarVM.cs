using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloMotosserra;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMMotosserra
{
	public class MotosserraListarVM
	{
		public MotosserraListarFiltros Filtros { get; set; }
		public Paginacao Paginacao { get; set; }
		public List<Motosserra> Resultados { get; set; }

		public string UltimaBusca { get; set; }
		public Boolean PodeVisualizar { get; set; }
		public Boolean PodeEditar { get; set; }
		public Boolean PodeExcluir { get; set; }
		public Boolean PodeAssociar { get; set; }
		public Boolean PodeAlterarSituacao { get; set; }

		public MotosserraListarVM() : this(new List<QuantPaginacao>()) { }

		private List<SelectListItem> _situacoes = new List<SelectListItem>();
		public List<SelectListItem> Situacoes
		{
			get { return _situacoes; }
			set { _situacoes = value; }
		}

		public MotosserraListarVM(List<QuantPaginacao> quantPaginacao)
		{
			Filtros = new MotosserraListarFiltros();
			Paginacao = new Paginacao();
			Resultados = new List<Motosserra>();
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false);
			
			List<Lista> situacoes = new List<Lista>();
			situacoes.Add(new Lista() { Id = "1", Texto = "Ativo", IsAtivo = true });
			situacoes.Add(new Lista() { Id = "2", Texto = "Desativo", IsAtivo = true });
			Situacoes = ViewModelHelper.CriarSelectList(situacoes, true, true);
		}

		public void SetListItens(List<QuantPaginacao> quantPaginacao, int quantidadePagina = 5)
		{
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false, selecionadoTexto: quantidadePagina.ToString());
		}
	}
}
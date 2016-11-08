using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMAtividade
{
	public class ListarVM
	{
		public List<SelectListItem> ListaQuantPaginacao { get; set; }
		public Paginacao Paginacao { get; set; }
		public AtividadeListarFiltro Filtros { get; set; }
		public List<EmpreendimentoAtividade> Resultados { get; set; }
		public String UltimaBusca { get; set; }

		public ListarVM() 
		{
			ListaQuantPaginacao = new List<SelectListItem>();
			Paginacao = new Paginacao();

			Filtros = new AtividadeListarFiltro();
			Resultados = new List<EmpreendimentoAtividade>();
		}

		public ListarVM(List<QuantPaginacao> quantPaginacao)
		{
			ListaQuantPaginacao = new List<SelectListItem>();
			Paginacao = new Paginacao();
			Filtros = new AtividadeListarFiltro();
			Resultados = new List<EmpreendimentoAtividade>();
			ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false);
		}

		public void SetResultados(List<EmpreendimentoAtividade> resultados)
		{
			Resultados = resultados;
		}
	}
}
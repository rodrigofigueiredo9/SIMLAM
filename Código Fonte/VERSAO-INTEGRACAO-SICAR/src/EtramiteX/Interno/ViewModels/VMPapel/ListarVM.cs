using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloPapel;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMPapel
{
	public class ListarVM
	{
		public PapelListarFiltro Filtros { get; set; }
		public Paginacao Paginacao { get; set; }
		public List<Papel> Resultados { get; set; }

		public string UltimaBusca { get; set; }
		public Boolean PodeVisualizar { get; set; }
		public Boolean PodeEditar { get; set; }
		public Boolean PodeExcluir { get; set; }
		public Boolean PodeAssociar { get; set; }

		public ListarVM()
		{
			Filtros = new PapelListarFiltro();
			Paginacao = new Paginacao();
			Resultados = new List<Papel>();
		}

		public ListarVM(List<QuantPaginacao> quantPaginacao)
		{
			Filtros = new PapelListarFiltro();
			Paginacao = new Paginacao();
			Resultados = new List<Papel>();
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false);
		}

		public void SetListItens(List<QuantPaginacao> quantPaginacao, int quantidadePagina = 5)
		{
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false, selecionadoTexto: quantidadePagina.ToString());
		}
	}
}
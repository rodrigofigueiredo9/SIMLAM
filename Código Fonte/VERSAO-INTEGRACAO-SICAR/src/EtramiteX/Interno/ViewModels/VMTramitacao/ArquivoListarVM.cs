using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloTramitacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMTramitacao
{
	public class ArquivoListarVM
	{
		public List<SelectListItem> ListaSetores { get; set; }
		public Paginacao Paginacao { get; set; }
		public TramitacaoArquivoFiltro Filtros { get; set; }
		public List<TramitacaoArquivo> Resultados { get; set; }
		public Boolean PodeVisualizar { get; set; }
		public Boolean PodeEditar { get; set; }
		public Boolean PodeExcluir { get; set; }
		public String UltimaBusca { get; set; }

		public ArquivoListarVM()
		{
			Paginacao = new Paginacao();
			Filtros = new TramitacaoArquivoFiltro();
			Resultados = new List<TramitacaoArquivo>();
		}

		public ArquivoListarVM(List<QuantPaginacao> quantPaginacao, List<Setor> setores)
		{
			Paginacao = new Paginacao();
			Filtros = new TramitacaoArquivoFiltro();
			Resultados = new List<TramitacaoArquivo>();

			ListaSetores = ViewModelHelper.CriarSelectList(setores, true, true);
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false);
		}

		public void SetListItens(List<QuantPaginacao> quantPaginacao, int quantidadePagina = 5)
		{
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false, selecionadoTexto: quantidadePagina.ToString());
		}
	}
}